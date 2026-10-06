using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sharp.EL9300 {
	
	[Flags]
	public enum ProgramMode : byte {
		Real = 0x01,
		NBase = 0x02,
		Matrix = 0x04,
		Complex = 0x08,
		Stat = 0x10,
	}

	public class ProgramVariable {

		private ProgramMode mode;
		public ProgramMode Mode {
			get => mode;
			set {
				if (!Enum.IsDefined(typeof(ProgramMode), value)) {
					throw new ArgumentException();
				} else {
					mode = value;
				}
			}
		}

		private string name;
		public string Name {
			get => name;
			set {
				if (value.Length < 1 || value.Length > 16 || Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value)) != value) {
					throw new ArgumentException();
				} else {
					name = value;
				}
			}
		}

		private byte[][] lines;
		public byte[][] Lines {
			get => lines;
			set {
				lines = value;
			}
		}

		public ProgramVariable(ProgramMode mode, string name, byte[][] lines) {
			Mode = mode;
			Name = name;
			Lines = lines;
		}

		public ProgramVariable(LinkTransfer transfer) {

			if (transfer.Head[0] != 2 || transfer.Head[1] != 0) throw new InvalidDataException("File is not an EL-9300 program.");

			var stream = new MemoryStream(transfer.Body);
			var reader = new BinaryReader(stream);

			var variableSize = reader.ReadUInt16();

			if (reader.ReadByte() != 0) throw new InvalidDataException();
			reader.ReadUInt16(); // Previous variable size
			if (reader.ReadByte() != 0) throw new InvalidDataException();

			// End of file?
			if (variableSize == 0) throw new InvalidDataException();

			if (reader.ReadByte() != 0) throw new InvalidDataException();


			if (reader.ReadByte() != 0x09) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			var programName = reader.ReadBytes(16);
			var programMode = (ProgramMode)reader.ReadByte();
			reader.ReadUInt16(); // Program size

			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadUInt16() != variableSize) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			var lines = new List<byte[]>();
			byte previousLineLength = 0;
			for (; ; ) {
				var lineLength = reader.ReadByte();
				if (reader.ReadByte() != previousLineLength) throw new InvalidDataException();

				// End of program?
				if (lineLength == 0) break;

				// Get the line
				var line = reader.ReadBytes(lineLength - 3);
				if (reader.ReadByte() != 0xFD) throw new InvalidDataException();

				lines.Add(line);

				previousLineLength = lineLength;
			}

			// Populate the variable
			Mode = programMode;
			Name = Encoding.ASCII.GetString(programName, 0, 16).TrimEnd('\0');
			Lines = lines.ToArray();

		}

		public LinkTransfer ToLinkTransfer() {

			// Combine the line data
			// Each line is stored as <line length> <previous line length> <line data> <0xFD>
			var programLineData = new List<byte>();
			{
				byte previousLineLength = 0;
				if (Lines != null && Lines.Length > 0) {
					foreach (var line in Lines) {
						byte thisLineLength = (byte)(line.Length + 3);
						programLineData.Add(thisLineLength);
						programLineData.Add(previousLineLength);
						programLineData.AddRange(line);
						programLineData.Add(0xFD);
						previousLineLength = thisLineLength;
					}
				}
				programLineData.Add(0);
				programLineData.Add(previousLineLength);
			}

			// Create the rest of the body
			var body = new List<byte>{
				0xFF, 0xFF, 0x00, // Current variable size
				0x00, 0x00, 0x00, // Previous variable size
				0x00, 0x09, 0x00
			};
			body.AddRange(Encoding.ASCII.GetBytes(Name.PadRight(16, '\0').Substring(0, 16)));
			body.Add((byte)Mode);

			body.Add(0xFF); body.Add(0xFF); // Will be program data size later

			body.Add(0x00);

			body.Add(0xFF); body.Add(0xFF); // Will be total variable size later

			body.Add(0x00);

			// Program data
			body.AddRange(programLineData);
			body.Add(0x00);

			// How big is the variable entry in total?
			var bodySize = (ushort)body.Count;

			// Slot it into the variable entry
			body[0] = (byte)(bodySize >> 0);
			body[1] = (byte)(bodySize >> 8);

			body[26] = (byte)((bodySize - 1) >> 0); // Program size minus 0x00 terminator, maybe?
			body[27] = (byte)((bodySize - 1) >> 8);

			body[29] = (byte)(bodySize >> 0);
			body[30] = (byte)(bodySize >> 8);

			// Create the head
			var head = new byte[] {
				0x02, 0x00, 0x00,
				(byte)(bodySize >> 0),(byte)(bodySize >> 8)
			};
			Array.Resize(ref head, 32);

			return new LinkTransfer(head, body.ToArray());

		}
	}

}
