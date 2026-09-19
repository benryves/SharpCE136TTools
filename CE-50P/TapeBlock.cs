using System;
using System.IO;
using System.Text;

namespace Sharp.CE50P {
	
	/// <summary>
	/// Represents a block of data with its information header on a tape.
	/// </summary>
	public class TapeBlock {

		private byte[] info = new byte[128];

		/// <summary>
		/// The 128-byte information header.
		/// </summary>
		public byte[] Info {
			get { return info; }
			set {
				if (value == null) throw new NullReferenceException("Info cannot be null.");
				if (value.Length != 128) throw new ArgumentException("Info must be 128 bytes long.");
				info = value;
			}
		}
		private byte[] data = new byte[0];

		/// <summary>
		/// Gets the size of the data in the block as referenced in the info block.
		/// </summary>
		public ushort InfoDataSize {
			get {
				return (ushort)((info[18] << 8) | (info[19] << 0));
			}
		}

		/// <summary>
		/// Gets the name of the file stored on the tape in the info block.
		/// </summary>
		/// <remarks>This is the name entered when a user loads or saves a file from their device and may not match the name of any files stored within the tape's data.</remarks>
		public string InfoName {
			get {
				return Encoding.ASCII.GetString(info, 1, 8).TrimEnd();
			}
		}

		/// <summary>
		/// The data contained within the block.
		/// </summary>
		public byte[] Data {
			get { return data; }
			set {
				data = value ?? throw new NullReferenceException("Data cannot be null");
			}
		}

		/// <summary>
		/// Creates an instance of a <see cref="TapeBlock"/> from a plain array of bytes for the info and a plain array of bytes for the data.
		/// </summary>
		/// <param name="info">The info header at the start of the block.</param>
		/// <param name="data">The data bytes attached to the block.</param>
		public TapeBlock(byte[] info, byte[] data) {
			Info = info;
			Data = data;
			if (Data.Length != InfoDataSize) throw new ArgumentException("Size of block's data does not match expected size in block's info.");
		}

		public static TapeBlock FromBytesWithChecksum(byte[] infoWithChecksum, byte[] dataWithChecksum) {
			return new TapeBlock(GetBytesWithoutChecksum(infoWithChecksum), GetBytesWithoutChecksum(dataWithChecksum));
		}

		/// <summary>
		/// Convert an array of bytes with a checksum on the end into the plain array of bytes.
		/// </summary>
		/// <param name="bytesWithChecksum">The array of bytes with a checksum on the end.</param>
		/// <returns>A plain array of bytes without the checksum on the end.</returns>
		/// <exception cref="InvalidDataException">Thrown if there is a problem with the checksum.</exception>
		public static byte[] GetBytesWithoutChecksum(byte[] bytesWithChecksum) {

			// Check that we have enough data
			if (bytesWithChecksum == null) throw new ArgumentException("bytesWithChecksum cannot be null", "bytesWithChecksum");
			if (bytesWithChecksum.Length < 2) throw new ArgumentException("bytesWithChecksum must have at least two bytes of data", "bytesWithChecksum");

			// Copy the raw data over
			byte[] bytesWithoutChecksum = new byte[bytesWithChecksum.Length - 2];
			Array.Copy(bytesWithChecksum, bytesWithoutChecksum, bytesWithoutChecksum.Length);

			// Check the checksum
			var checksum = (ushort)(((bytesWithChecksum[bytesWithChecksum.Length - 1] << 0)) | (bytesWithChecksum[bytesWithChecksum.Length - 2] << 8));
			foreach (var b in bytesWithoutChecksum) checksum -= b;

			if (checksum != 0) {
				throw new InvalidDataException("Invalid checksum.");
			} else {
				return bytesWithoutChecksum;
			}

		}

		/// <summary>
		/// Convert a plain array of bytes into an array of bytes with a checksum on the end.
		/// </summary>
		/// <param name="bytesWithoutChecksum">A plain array of bytes without a checksum on the end.</param>
		/// <returns>The array of bytes with a checksum on the end.</returns>
		public static byte[] GetBytesWithChecksum(byte[] bytesWithoutChecksum) {

			// Check that we have enough data
			if (bytesWithoutChecksum == null) throw new ArgumentException("bytesWithChecksum cannot be null", "bytesWithChecksum");

			// Copy the raw data over
			byte[] bytesWithChecksum = new byte[bytesWithoutChecksum.Length + 2];
			Array.Copy(bytesWithoutChecksum, bytesWithChecksum, bytesWithoutChecksum.Length);

			// Append the checksum
			ushort checksum = 0;
			foreach (var b in bytesWithoutChecksum) checksum += b;

			bytesWithChecksum[bytesWithChecksum.Length - 2] = (byte)(checksum >> 8);
			bytesWithChecksum[bytesWithChecksum.Length - 1] = (byte)(checksum >> 0);

			return bytesWithChecksum;

		}

		/// <summary>
		/// Save this <see cref="TapeBlock"/> to a <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> to save to.</param>
		public void WriteToStream(Stream stream) {
			var infoChecksum = GetBytesWithChecksum(info);
			var dataChecksum = GetBytesWithChecksum(data);
			stream.Write(infoChecksum, 0, infoChecksum.Length);
			stream.Write(dataChecksum, 0, dataChecksum.Length);
		}

		/// <summary>
		/// Loads a <see cref="TapeBlock"/> from a <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> to load the <see cref="TapeBlock"/> from.</param>
		/// <returns>The <see cref="TapeBlock"/> at the current position of the <see cref="Stream"/>.</returns>
		public static TapeBlock FromStream(Stream stream) {
			var infoChecksum = new byte[128 + 2];
			for (int i = 0; i < infoChecksum.Length;) {
				i += stream.Read(infoChecksum, i, infoChecksum.Length - i);
			}

			var dataSize = (ushort)((infoChecksum[18] << 8) | (infoChecksum[19] << 0));
			var dataChecksum = new byte[dataSize + 2];
			for (int i = 0; i < dataChecksum.Length;) {
				i += stream.Read(dataChecksum, i, dataChecksum.Length - i);
			}

			return FromBytesWithChecksum(infoChecksum, dataChecksum);

		}

	}
}
