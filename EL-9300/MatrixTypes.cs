using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sharp.EL9300 {

	[Flags]
	public enum MatrixMode : byte {
		None = 0x00,
		Statistics = 0x20,
		Weighted = 0x40,
	}

	public class MatrixVariable {

		public char Name { get; set; }

		public MatrixMode Mode { get; set; }

		private RealNumber[,] items;
		public RealNumber[,] Items {
			get { return items; }
			set {
				if (value == null) {
					throw new InvalidOperationException();
				} else if (value.GetLength(0) < 1 || value.GetLength(1) < 1) {
					throw new InvalidOperationException();
				} else {
					items = value;
				}
			}
		}

		public int RowCount { get => items.GetLength(0); }
		public int ColumnCount { get => items.GetLength(1); }

		public override string ToString() {
			return string.Format("{0}[{1},{2}]", Name, RowCount, ColumnCount);
		}

		public RealNumber[] GetRow(int rowNumber) {
			var result = new RealNumber[ColumnCount];
			for (int col = 0; col < result.Length; ++col) {
				result[col] = items[rowNumber, col];
			}
			return result;
		}

		public RealNumber[] GetColumn(int columnNumber) {
			var result = new RealNumber[RowCount];
			for (int row = 0; row < result.Length; ++row) {
				result[row] = items[row, columnNumber];
			}
			return result;
		}

		public static ushort DecodeBcd(ushort value) {
			var decoded = checked((ushort)((((value >> 0) & 0xF) * 1) + (((value >> 4) & 0xF) * 10) + (((value >> 8) & 0xF) * 100) + (((value >> 12) & 0xF) * 1000)));
			if (EncodeBcd(decoded) != value) throw new InvalidOperationException();
			return decoded;
		}

		public static ushort EncodeBcd(ushort value) {
			if (value > 9999) throw new InvalidOperationException();
			return checked((ushort)((((value / 1) % 10) << 0) | (((value / 10) % 10) << 4) | (((value / 100) % 10) << 8) | (((value / 1000) % 10) << 12)));
		}

		public MatrixVariable(char name, int rowCount, int columnCount) {
			Name = name;
			Items = new RealNumber[rowCount, columnCount];
		}

		public MatrixVariable(LinkTransfer transfer) {

			if (transfer.Head[0] != 0 || transfer.Head[1] != 0 || transfer.Head[2] != 1) throw new InvalidDataException("File is not an EL-9300 matrix.");

			// Pull the data from the body
			var matrixName = Encoding.ASCII.GetString(transfer.Body, transfer.Body.Length - 1, 1)[0];
			var matrixMode = (MatrixMode)transfer.Body[transfer.Body.Length - 2];
			var matrixRows = DecodeBcd((ushort)(transfer.Body[transfer.Body.Length - 3] + (transfer.Body[transfer.Body.Length - 4] << 8)));
			var matrixCols = DecodeBcd((ushort)(transfer.Body[transfer.Body.Length - 5] + (transfer.Body[transfer.Body.Length - 6] << 8)));


			// Check this matches the head
			if (transfer.Head[0x11] != transfer.Body[transfer.Body.Length - 1]) throw new InvalidDataException("Matrix name count does not match between head and body.");
			if (DecodeBcd((ushort)((transfer.Head[0x14] << 8) + transfer.Head[0x15])) != matrixRows) throw new InvalidDataException("Matrix row count does not match between head and body.");
			if (DecodeBcd((ushort)((transfer.Head[0x12] << 8) + transfer.Head[0x13])) != matrixCols) throw new InvalidDataException("Matrix column count does not match between head and body.");

			// Name and allocate 
			Name = matrixName;
			Mode = matrixMode;
			Items = new RealNumber[matrixRows, matrixCols];

			// Populate the item array
			for (int row = 0; row < matrixRows; ++row) {
				for (int col = 0; col < matrixCols; ++col) {
					items[row, col] = RealNumber.FromBytes(transfer.Body, ((matrixRows - row - 1) + (matrixCols - col - 1) * matrixRows) * RealNumber.SizeInBytes);
				}
			}
		}

		public LinkTransfer ToLinkTransfer() {

			var matrixRows = EncodeBcd(checked((ushort)RowCount));
			var matrixCols = EncodeBcd(checked((ushort)ColumnCount));

			// Build up the body
			var body = new List<byte>(matrixRows * matrixCols * RealNumber.SizeInBytes + 6);

			// Items appear first, in reverse 
			for (int col = ColumnCount - 1; col >= 0; col--) {
				for (int row = RowCount- 1; row >= 0; row--) {
					body.AddRange(items[row, col].GetBytes());
				}
			}

			// Column and row count
			body.Add((byte)(matrixCols >> 8)); body.Add((byte)(matrixCols >> 0));
			body.Add((byte)(matrixRows >> 8)); body.Add((byte)(matrixRows >> 0));

			// Mode and name
			body.Add((byte)Mode);
			body.Add((byte)Name);
			
			// Generate the head
			var bodySize = checked((ushort)body.Count);
			var head = new byte[] {
				0x00, 0x00, 0x01,
				(byte)(bodySize >> 0),(byte)(bodySize >> 8),
			};
			Array.Resize(ref head, 32);

			// Mode, name and dimensions also appear in the head
			head[0x10] = (byte)Mode;
			head[0x11] = (byte)Name;
			head[0x12] = (byte)(matrixCols >> 8); head[0x13] = (byte)(matrixCols >> 0);
			head[0x14] = (byte)(matrixRows >> 8); head[0x15] = (byte)(matrixRows >> 0);

			return new LinkTransfer(head, body.ToArray());
		}

	}
}
