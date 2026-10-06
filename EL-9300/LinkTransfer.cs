using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sharp.EL9300 {
	public class LinkTransfer {

		private byte[] head = new byte[32];
		public byte[] Head {
			get => head;
			set {
				if (value.Length != 32) throw new InvalidOperationException("Head must be 32 bytes in size.");
				head = value;
			}
		}

		private byte[] body = new byte[0];
		public byte[] Body {
			get => body;
			set {
				if (body == null) throw new NullReferenceException();
				body = value;
			}
		}

		private void CheckBodySize() {
			var bodySize = head[3] + (head[4] << 8);
			if (body.Length != bodySize) throw new InvalidDataException("Body size does not match size specified in header.");
		}

		public LinkTransfer(byte[] head, byte[] body) {
			this.head = head;
			this.body = body;
			CheckBodySize();
		}

		public LinkTransfer(Stream stream) {
			var reader = new BinaryReader(stream);
			head = reader.ReadBytes(32);
			var bodySize = head[3] + (head[4] << 8);
			body = reader.ReadBytes(bodySize);
		}

		public LinkTransfer(string path) {
			using (var reader = new BinaryReader(File.OpenRead(path))) {
				head = reader.ReadBytes(32);
				var bodySize = head[3] + (head[4] << 8);
				body = reader.ReadBytes(bodySize);
			}
		}

		public void Save(Stream stream) {
			CheckBodySize();
			var writer = new BinaryWriter(stream);
			writer.Write(head);
			writer.Write(body);
		}

		public void Save(string path) {
			using (var stream = File.Create(path)) {
				Save(stream);
			}
		}

		public string GetFileName() {
			switch (head[0x00]) {
				case 0: // Matrices, stats
					if (head[0x11] == 0xFF) {
						return Encoding.ASCII.GetString(body, 1, 8).TrimEnd() + ".gam";
					} else if (head[0x11] == 0x40) {
						return "DATA.g1l";
					} else {
						return Encoding.ASCII.GetString(head, 0x11, 1) + ".g1m";
					}
				case 1: // Graph equations
				case 2: // Programs
				case 3: // Solver equations
					var extension = "?ype"[head[0x00]];
					if (head[0x02] == 0) {
						return Encoding.ASCII.GetString(body, 9, 16).TrimEnd('\0') + ".g1" + extension;
					} else {
						return Encoding.ASCII.GetString(body, 1, 8).TrimEnd() + ".ga" + extension;
					}
				case 4: // Backups
					return "BACKUP.gcb";
				default:
					return null;
			}
		}

		public override string ToString() {
			return GetFileName();
		}

		public bool HasSubItems {
			get {
				// Body must start 0xFB <file name>
				if (body.Length < 32 || body[0x00] != 0xFB) return false;
				// Check head
				switch (head[0x00]) {
					case 0: // Matrices, stats
						return head[0x11] == 0xFF;
					case 1: // Graph equations
					case 2: // Programs
					case 3: // Solver equations
						return head[0x02] == 0x01;
					case 4: // Backups
						return true;
					default:
						return false;
				}

			}
		}

		public LinkTransfer[] GetSubItems() {
			
			if (!HasSubItems) throw new InvalidOperationException("This file does not have any sub-items.");
			var subItems = new List<LinkTransfer>();

			var stream = new MemoryStream(body);
			var reader = new BinaryReader(stream);

			if (reader.ReadByte() != 0xFB) throw new InvalidDataException("The file is not a valid group.");

			var groupFileName = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd(' ');
			var groupFileExtension = Encoding.ASCII.GetString(reader.ReadBytes(4)).TrimEnd(' ');
			if (groupFileExtension != "1") throw new InvalidDataException("Group extension is not 1.");

			if (reader.ReadInt32() != 0) throw new InvalidDataException();

			var totalSize = reader.ReadUInt16();
			
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			if (reader.ReadUInt16() != totalSize) throw new InvalidDataException(string.Format("Size records do not agree ({0}).", totalSize));

			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			if (reader.ReadUInt16() != totalSize) throw new InvalidDataException(string.Format("Size records do not agree ({0}).", totalSize));


			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			if (reader.ReadUInt16() != 0) throw new InvalidDataException();
			if (reader.ReadUInt16() != 0) throw new InvalidDataException();
			if (reader.ReadUInt16() != 0) throw new InvalidDataException();

			if (reader.ReadByte() != 0xFF) throw new InvalidDataException();

			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadByte() != 0x09) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadByte() != 0x29) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			ushort previousVariableSize = 0;

			for (; ; ) {
				// Variable sizes
				ushort nextVariableSize = reader.ReadUInt16();
				if (reader.ReadByte() != 0x00) throw new InvalidDataException();
				if (reader.ReadUInt16() != previousVariableSize) throw new InvalidDataException();
				if (reader.ReadByte() != 0x00) throw new InvalidDataException();
				if (nextVariableSize == 0) break;

				// Read the subitem's body
				stream.Seek(-6, SeekOrigin.Current);
				var subItemBody = reader.ReadBytes(nextVariableSize);

				if (groupFileName == "MATRIX") {

					// Matrices are handled differently to all other variable types, and stored backwards
					for (int offset = subItemBody.Length; offset > 12;) {

						var matrixRows = MatrixVariable.DecodeBcd((ushort)(subItemBody[offset - 3] + (subItemBody[offset - 4] << 8)));
						var matrixCols = MatrixVariable.DecodeBcd((ushort)(subItemBody[offset - 5] + (subItemBody[offset - 6] << 8)));

						var matrixDataSize = matrixRows * matrixCols * RealNumber.SizeInBytes;
						var matrixTotalSize = checked((ushort)(matrixDataSize + 6));

						// Generate a new matrix head
						var matrixItemHead = new byte[32];

						matrixItemHead[0x02] = 0x01;
						matrixItemHead[0x03] = (byte)(matrixTotalSize >> 0);
						matrixItemHead[0x04] = (byte)(matrixTotalSize >> 8);

						matrixItemHead[0x11] = subItemBody[offset - 1];
						matrixItemHead[0x12] = subItemBody[offset - 6];
						matrixItemHead[0x13] = subItemBody[offset - 5];
						matrixItemHead[0x14] = subItemBody[offset - 4];
						matrixItemHead[0x15] = subItemBody[offset - 3];


						// Copy the matrix body
						var matrixItemBody  = new byte[matrixTotalSize];
						offset -= matrixTotalSize;
						Array.Copy(subItemBody, offset, matrixItemBody, 0, matrixTotalSize);

						subItems.Add(new LinkTransfer(matrixItemHead, matrixItemBody));
					}

				} else {

					// Generate the subitem's head
					var subItemHead = new byte[32];
					switch (groupFileName) {
						case "GRAPH":
							subItemHead[0x00] = 0x01;
							break;
						case "PROGRAM":
							subItemHead[0x00] = 0x02;
							break;
						case "SOLVER":
							subItemHead[0x00] = 0x03;
							break;
						default:
							throw new NotImplementedException(string.Format("Group file type '{0}' not supported.", groupFileName));
					}
					subItemHead[0x03] = (byte)(subItemBody.Length >> 0);
					subItemHead[0x04] = (byte)(subItemBody.Length >> 8);

					subItems.Add(new LinkTransfer(subItemHead, subItemBody));
				}

				previousVariableSize = nextVariableSize;
			}
			return subItems.ToArray();
		}
	}
}
