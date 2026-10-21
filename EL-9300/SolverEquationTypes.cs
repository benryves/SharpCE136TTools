using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sharp.EL9300 {

	public enum SolverEquationMode : byte {
		OneLineEditor = 0x00,
		EquationEditor = 0x01,
	}

	public class SolverEquationVariable {

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

		public SolverEquationMode Mode {
			get; set;
		}

		private byte[] equation = new byte[] { 0xFF };
		public byte[] Equation {
			get {
				var value = new byte[equation.Length];
				Array.Copy(equation, value, equation.Length);
				return value;
			}
			set {
				if (value.Length < 1) {
					throw new ArgumentException();
				} else {
					equation = new byte[value.Length];
					Array.Copy(value, equation, value.Length);
				}
			}
		}

		public SolverEquationVariable(string name, SolverEquationMode mode, byte[] equation) {
			Name = name;
			Mode = mode;
			Equation = equation;
		}

		public SolverEquationVariable(LinkTransfer transfer) {

			if (transfer.Head[0] != 3 || transfer.Head[1] != 0) throw new InvalidDataException("File is not an EL-9300 solver equation.");

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

			var equationName = reader.ReadBytes(16);
			var equationMode = (SolverEquationMode)reader.ReadByte();
			reader.ReadUInt16(); // Program size

			if (reader.ReadByte() != 0x00) throw new InvalidDataException();
			if (reader.ReadUInt16() != variableSize) throw new InvalidDataException();
			if (reader.ReadByte() != 0x00) throw new InvalidDataException();

			var equation = new List<byte>();

			byte equationByte;
			do {
				equationByte = reader.ReadByte();
				equation.Add(equationByte);
			} while (equationByte != 0xFF);

			// Populate the variable
			Mode = equationMode;
			Name = Encoding.ASCII.GetString(equationName, 0, 16).TrimEnd('\0');
			Equation = equation.ToArray();

		}
	}
}
