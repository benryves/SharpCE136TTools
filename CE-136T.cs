using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sharp.CE136T {
	public static class PaBus {

		[DllImport("CE-136T.dll", EntryPoint = "paBusOpen")]
		public static extern bool Open([MarshalAs(UnmanagedType.LPTStr)] string portName);

		[DllImport("CE-136T.dll", EntryPoint = "paBusClose")]
		public static extern void Close();

		[DllImport("CE-136T.dll", EntryPoint = "paBusIsOpen")]
		public static extern bool IsOpen();

		[DllImport("CE-136T.dll", EntryPoint = "paBusReadByte")]
		public static extern bool Read(out byte value);

		[DllImport("CE-136T.dll", EntryPoint = "paBusReadBytes")]
		public static extern int Read(byte[] buffer, int offset, int length, int timeout);

		[DllImport("CE-136T.dll", EntryPoint = "paBusWriteByte")]
		public static extern bool Write(byte value);

		[DllImport("CE-136T.dll", EntryPoint = "paBusWriteBytes")]
		public static extern int Write(byte[] buffer, int offset, int length, int timeout);

		[DllImport("CE-136T.dll", EntryPoint = "paBusEnd")]
		public static extern bool End();

	}

	public class PaBusStream : Stream {

		/// <summary>
		/// Creates an instance of a <see cref="PaBusStream"/> from a particular serial port name.
		/// </summary>
		/// <param name="portName">The name of the serial port to use.</param>
		/// <exception cref="UnauthorizedAccessException">Thrown if the port is already open.</exception>
		/// <exception cref="IOException">Thrown if the serial port could not be opened for any other reason.</exception>
		public PaBusStream(string portName) {
			if (PaBus.IsOpen()) {
				throw new UnauthorizedAccessException(portName + " is already open.");
			} else if (!PaBus.Open(portName)) {
				throw new IOException("Could not open serial port " + portName);
			}
		}

		protected override void Dispose(bool disposing) {
			if (PaBus.IsOpen()) PaBus.Close();
			base.Dispose(disposing);
		}

		#region Timeout

		public override bool CanTimeout => true;

		public override int ReadTimeout { get; set; } = 2000;

		public override int WriteTimeout { get; set; } = 2000;

		#endregion

		#region Seeking (Not applicable)

		public override bool CanSeek => false;

		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotImplementedException();
		}

		#endregion

		#region Length (not applicable)

		public override long Length => throw new NotImplementedException();

		public override void SetLength(long value) {
			throw new NotImplementedException();
		}

		#endregion

		#region Reading

		public override bool CanRead => true;

		public override int Read(byte[] buffer, int offset, int count) {
			if (!PaBus.IsOpen()) throw new ObjectDisposedException("PA bus is not open.");
			return PaBus.Read(buffer, offset, count, ReadTimeout);
		}

		public void ReadAcknowledgement() {
			try {
				int ack = ReadByte();
				if (ack != 0xFA) throw new IOException(string.Format("Could not read acknowledgement: Received 0x{0:X2}.", ack));
			} catch (TimeoutException ex) {
				throw new TimeoutException("Could not read acknowledgement: " + ex.Message);
			} catch (Exception ex) {
				throw new IOException("Could not read acknowledgement: " + ex.Message);
			}

		}

		#endregion

		#region Writing

		public override bool CanWrite => true;

		public override void Write(byte[] buffer, int offset, int count) {
			if (!PaBus.IsOpen()) throw new ObjectDisposedException("PA bus is not open.");
			if (PaBus.Write(buffer, offset, count, WriteTimeout) != count) {
				throw new TimeoutException();
			}
		}

		public override void Flush() {
			if (!PaBus.IsOpen()) throw new ObjectDisposedException("PA bus is not open.");
			DateTime endTime = DateTime.Now + TimeSpan.FromMilliseconds(ReadTimeout);
			while (!PaBus.End()) {
				if (DateTime.Now >= endTime) throw new TimeoutException();
			}
		}

		public void WriteAcknowledgement() {
			WriteByte(0xFA);
		}

		#endregion

	}
}