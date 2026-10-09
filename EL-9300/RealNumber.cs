using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Sharp.EL9300 {

	[Flags]
	public enum RealNumberFlags : byte {
		None = 0,
		Negative = 0x01,
		Complex = 0x04,
	}

	public class RealNumberConverter : TypeConverter {

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			} else {
				return base.CanConvertFrom(context, sourceType);
			}
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			} else {
				return base.CanConvertTo(context, destinationType);
			}
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string s) {
				return RealNumber.Parse(s);
			} else {
				return base.ConvertFrom(context, culture, value);
			}
		}

	}

	[TypeConverter(typeof(RealNumberConverter))]
	public struct RealNumber : IFormattable {

		public const int SizeInBytes = 9;

		public RealNumberFlags Flags { get; set; }
		public ulong Mantissa { get; set; }
		public int Exponent { get; set; }

		public RealNumber(ulong mantissa, int exponent, RealNumberFlags flags) {
			// Range check
			if (exponent < -99 || exponent > 99) throw new OverflowException();
			Mantissa = mantissa;
			Exponent = exponent;
			Flags = flags;
		}

		public RealNumber(ulong mantissa, int exponent) : this(mantissa, exponent, RealNumberFlags.None) {
		}

		public static RealNumber Zero {
			get => new RealNumber(0, 0);
		}

		public bool IsZero {
			get => Mantissa == 0;
		}

		public bool IsPlusOrMinusOne {
			get => Mantissa == 10000000000000 && Exponent == 0;
		}

		public bool IsComplex {
			get => (Flags & RealNumberFlags.Complex) != 0;
		}

		public bool IsNegative {
			get => (Flags & RealNumberFlags.Negative) != 0;
		}

		public static RealNumber FromBytes(byte[] bytes, int offset) {
			
			var flags = (RealNumberFlags)(bytes[offset + 1] & 0xF);

			// Unpack the mantissa from the BCD
			ulong mantissa = 0;
			for (int part = 0; part < 7; ++part) {
				mantissa *= 100;
				mantissa += (ulong)(((bytes[offset + 2 + part] & 0xF0) >> 4) * 10 + ((bytes[offset + 2 + part] & 0x0F) >> 0) * 1);
			}

			int exponent = ((bytes[offset + 0] & 0xF0) >> 4) * 100 + ((bytes[offset + 0] & 0x0F) >> 0) * 10 + ((bytes[offset + 1] & 0xF0) >> 4) * 1;
			while (exponent >= 500) {
				exponent -= 1000;
			}

			return new RealNumber { Flags = flags, Mantissa = mantissa, Exponent = exponent };
		}

		public static RealNumber FromBytes(byte[] bytes) {
			return FromBytes(bytes, 0);
		}

		public int GetBytes(byte[] bytes, int offset) {

			// If exponent is negative it is stored as (exponent + 1000), so check if it's in range and adjust
			int exponent = Exponent;
			if (exponent < -99 || exponent > +99) throw new InvalidOperationException("Exponent is out of range.");
			if (exponent < 0) exponent += 1000;

			// Check flags are in range
			if ((RealNumberFlags)((byte)Flags & 0x0F) != Flags) throw new InvalidOperationException("Flags are out of range.");

			// BCD-pack and store the exponent
			bytes[offset + 0] = (byte)((((exponent / 100) % 10) << 4) | (((exponent / 10) % 10) << 0));
			bytes[offset + 1] = (byte)((((exponent / 1) % 10) << 4) | ((byte)Flags & 0x0F) << 0);

			// Store the mantissa
			ulong mantissa = Mantissa;
			for (int part = 7 - 1; part >= 0; --part) {
				bytes[offset + 2 + part] = (byte)((((mantissa / 10) % 10) << 4) | (((mantissa / 1) % 10) << 0));
				mantissa /= 100;
			}

			// We should have nothing left after encoding the mantissa
			if (mantissa != 0) throw new InvalidOperationException("Mantissa is out of range.");

			return SizeInBytes;
		}

		public int GetBytes(byte[] bytes) {
			return GetBytes(bytes, 0);
		}

		public byte[] GetBytes() {
			var result = new byte[SizeInBytes];
			GetBytes(result);
			return result;
		}

		public static explicit operator double(RealNumber number) => (number.IsNegative ? -1 : +1) * (double)number.Mantissa * Math.Pow(10d, number.Exponent - 13);

		public static explicit operator RealNumber(double number) {
			if (number == 0) return Zero;
			
			// Get the flags
			RealNumberFlags flags = RealNumberFlags.None;
			if (number < 0) {
				flags |= RealNumberFlags.Negative;
				number = -number;
			}
			
			// Calculate the exponent and mantissa
			int exponent = (int)Math.Floor(Math.Log10(number));
			ulong mantissa = (ulong)Math.Round(number * Math.Pow(10d, 13 - exponent));

			return new RealNumber(mantissa, exponent, flags);
		}


		public string ToString(string format, IFormatProvider formatProvider) {
			
			// Default formatting rule = G10
			int maxDigits = 10;
			char formatType = 'G';

			// If format is specified, change the formatting rule
			if (format != null) {
				switch (formatType = char.ToUpperInvariant(format[0])) {
					case 'G': // General format
					case 'E': // Exponential (scientific)
						if (format.Length > 1) {
							maxDigits = int.Parse(format.Substring(1), CultureInfo.InvariantCulture);
							if (formatType == 'E') ++maxDigits;
							if (maxDigits < 1) throw new FormatException();
							if (maxDigits > 14) maxDigits = 14;
						}
						break;
					default:
						throw new FormatException();
				}
			}

			var result = new StringBuilder(maxDigits);

			// Expand the mantissa into digits
			var digits = new int[14];
			var mantissa = Mantissa;
			for (var digit = 13; digit >= 0; --digit) {
				digits[digit] = (int)(mantissa % 10);
				mantissa /= 10;
			}

			// Leading sign
			if (IsNegative) {
				result.Append("-");
			}

			if (formatType == 'E' || Exponent < -(maxDigits - 1) || Exponent > (maxDigits - 1)) {
				// Scientific notation
				var maxDigitIndex = maxDigits - 1;
				while (maxDigitIndex > 0 && digits[maxDigitIndex] == 0) {
					--maxDigitIndex;
				}
				for (var digitIndex = 0; digitIndex <= maxDigitIndex; ++digitIndex) {
					result.Append((char)('0' + digits[digitIndex]));
					if (digitIndex == 0 && digitIndex < maxDigitIndex) result.Append('.');
				}

				// Exponent prefix
				result.Append("E");

				// Exponent sign
				var exponent = Exponent;
				if (exponent < 0) {
					exponent = checked(-exponent);
					result.Append("-");
				}

				// Sanity check for exponent
				if (exponent > 999) throw new InvalidOperationException(string.Format("Exponent {0} is out of range.", Exponent));

				// Exponent digits
				var exponentDigit = exponent / 100;
				if (exponentDigit > 0) {
					result.Append((char)('0' + exponentDigit));
				}
				exponentDigit = (exponent % 100) / 10;
				if (exponent > 99 || exponentDigit > 0) {
					result.Append((char)('0' + exponentDigit));
				}
				exponentDigit = exponent % 10;
				result.Append((char)('0' + exponentDigit));
			} else if (Exponent <= 0) {
				// Negative (or zero) exponent
				var maxDigitIndex = maxDigits - 1;
				maxDigitIndex += Exponent;
				while (maxDigitIndex > 0 && digits[maxDigitIndex] == 0) {
					--maxDigitIndex;
				}
				for (var digitIndex = Exponent; digitIndex <= maxDigitIndex; ++digitIndex) {
					if (digitIndex < 0) {
						result.Append('0');
					} else {
						result.Append((char)('0' + digits[digitIndex]));
					}
					if (digitIndex == Exponent && digitIndex < maxDigitIndex) result.Append('.');
				}
			} else {
				// Positive exponent
				var maxDigitIndex = maxDigits - 1;
				while (maxDigitIndex > Exponent && digits[maxDigitIndex] == 0) {
					--maxDigitIndex;
				}
				for (var digitIndex = 0; digitIndex <= maxDigitIndex; ++digitIndex) {
					result.Append((char)('0' + digits[digitIndex]));
					if (digitIndex == Exponent && digitIndex < maxDigitIndex) result.Append('.');
				}
			}

			return result.ToString();
		}

		public override string ToString() {
			return ToString(null, CultureInfo.InvariantCulture);
		}

		public static RealNumber Parse(string s) {

			// Quick check for null/empty value
			if (s == null) throw new ArgumentNullException();

			// Clean up string before parsing
			s = s.Replace(",", "").Replace(" ", "").Trim();

			// Build up two lists of digits
			var mantissaPresent = false;
			var mantissaSign = 0;
			var mantissaDigits = new List<int>(s.Length);

			var exponentIndex = -1;
			var exponentPresent = false;
			var exponentSign = 0;
			var exponentDigits = new List<int>(2);

			// Where is the radix point?
			var radixIndex = -1;

			for (var sourceIndex = 0; sourceIndex < s.Length; ++sourceIndex) {

				char mantissaChar = s[sourceIndex];
				switch (mantissaChar) {
					case 'E':
					case 'e':
						// Decoding the exponent
						exponentIndex = sourceIndex;
						for (++sourceIndex; sourceIndex < s.Length; ++sourceIndex) {
							char exponentChar = s[sourceIndex];
							switch (exponentChar) {
								case '+':
									if (exponentSign != 0 || exponentDigits.Count > 0) throw new FormatException();
									exponentSign = +1;
									break;
								case '-':
									if (exponentSign != 0 || exponentDigits.Count > 0) throw new FormatException();
									exponentSign = -1;
									break;
								default:
									if (exponentChar < '0' || exponentChar > '9') {
										throw new FormatException();
									} else {
										exponentDigits.Add((int)(exponentChar - '0'));
										exponentPresent = true;
									}
									break;
							}
						}
						break;
					case '+':
						if (mantissaSign != 0 || mantissaDigits.Count > 0) throw new FormatException();
						mantissaSign = +1;
						break;
					case '-':
						if (mantissaSign != 0 || mantissaDigits.Count > 0) throw new FormatException();
						mantissaSign = -1;
						break;
					case '.':
						if (radixIndex >= 0) throw new FormatException();
						radixIndex = mantissaDigits.Count;
						break;
					default:
						if (mantissaChar < '0' || mantissaChar > '9') {
							throw new FormatException();
						} else {
							mantissaDigits.Add((int)(mantissaChar - '0'));
							mantissaPresent = true;
						}
						break;
				}
			}

			// If we never saw the radix point, assume it's at the end of the mantissa
			if (radixIndex < 0) {
				radixIndex = mantissaDigits.Count;
			}

			// Is the mantissa missing?
			if (!mantissaPresent) throw new FormatException();

			// Strip leading zeroes from the mantissa
			while (mantissaDigits.Count > 0 && mantissaDigits[0] == 0) {
				mantissaDigits.RemoveAt(0);
				--radixIndex;
			}

			// Convert the mantissa from a series of digits into an encoded ulong
			ulong mantissa = 0;
			for (var mantissaDigitIndex = 0; mantissaDigitIndex < 14; ++mantissaDigitIndex) checked {
				mantissa *= 10;
				if (mantissaDigitIndex < mantissaDigits.Count) mantissa += checked((ulong)mantissaDigits[mantissaDigitIndex]);
			}

			// Is the result zero?
			if (mantissa == 0) return Zero;

			// Decode the exponent
			int exponent = 0;
			if (exponentIndex >= 0) {
				
				// Is the exponent denoted but missing?
				if (!exponentPresent) throw new FormatException();

				// Strip leading zeroes from the exponent
				while (exponentDigits.Count > 0 && exponentDigits[0] == 0) {
					exponentDigits.RemoveAt(0);
				}

				// Is it still too big?
				if (exponentDigits.Count > 3) throw new FormatException();

				foreach (var exponentDigit in exponentDigits) checked {
					exponent *= 10;
					exponent += exponentDigit;
				}

				// Negative exponent?
				if (exponentSign < 0) exponent = -exponent;
			}

			// Offset the exponent by the index of the radix point
			exponent += (radixIndex - 1);

			return new RealNumber(mantissa,exponent, (mantissaSign < 0) ? RealNumberFlags.Negative : RealNumberFlags.None);
		}
		
		public static bool TryParse(string s, out RealNumber result) {
			try {
				result = Parse(s);
				return true;
			} catch {
				result = default;
				return false;
			}
		}


	}
}
