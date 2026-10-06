using System;

namespace Sharp.EL9300 {
	public class MatrixVariable {

		public int Rows { get; set; }
		public int Columns { get; set; }

		public static ushort DecodeBcd(ushort value) {
			var decoded = checked((ushort)((((value >> 0) & 0xF) * 1) + (((value >> 4) & 0xF) * 10) + (((value >> 8) & 0xF) * 100) + (((value >> 12) & 0xF) * 1000)));
			if (EncodeBcd(decoded) != value) throw new InvalidOperationException();
			return decoded;
		}

		public static ushort EncodeBcd(ushort value) {
			if (value > 9999) throw new InvalidOperationException();
			return checked((ushort)((((value / 1) % 10) << 0) | (((value / 10) % 10) << 4) | (((value / 100) % 10) << 8) | (((value / 1000) % 10) << 12)));
		}

	}
}
