using System;
using System.Runtime.InteropServices;

namespace Sharp.CE136T {
	public static class PaBus {
		
		[DllImport("CE-136T.dll", EntryPoint = "paBusOpen")]
		public static extern bool Open([MarshalAs(UnmanagedType.LPTStr)] string portName);

		[DllImport("CE-136T.dll", EntryPoint = "paBusClose")]
		public static extern void Close();

		[DllImport("CE-136T.dll", EntryPoint = "paBusIsOpen")]
		public static extern bool IsOpen();

		[DllImport("CE-136T.dll", EntryPoint = "paBusDelay")]
		public static extern void Delay(uint microseconds);

		[DllImport("CE-136T.dll", EntryPoint = "paBusReadByte")]
		public static extern bool ReadByte(out byte value);
		
		[DllImport("CE-136T.dll", EntryPoint = "paBusReadBytes")]
		public static extern uint ReadBytes(byte[] buffer, uint length);

		[DllImport("CE-136T.dll", EntryPoint = "paBusWriteByte")]
		public static extern bool WriteByte(byte value);
		
		[DllImport("CE-136T.dll", EntryPoint = "paBusWriteBytes")]
		public static extern uint WriteBytes(byte[] buffer, uint length);

		[DllImport("CE-136T.dll", EntryPoint = "paBusEnd")]
		public static extern bool End();

	}
}