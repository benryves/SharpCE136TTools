using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace Sharp.CE50P {
	internal static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {

			/*if (PaBus.Open(@"COM2")) {

				byte[] printer_row = new byte[128];

				byte[] cassette_header = new byte[130];
				byte[] cassette_data = new byte[64 * 1024 + 2];
				ushort cassete_data_size;

				uint length;
				for (; ; ) {
					if (PaBus.ReadByte(out byte value)) {
						switch (value) {
							case 0xAA:
								Console.WriteLine("<- 0xAA [Identify]");
								PaBus.Delay(1000);
								if (PaBus.WriteByte(0xF0)) Console.WriteLine("-> 0xF0");
								if (PaBus.WriteByte(0x01)) Console.WriteLine("-> 0x01");
								break;
							case 0xC0:
								Console.WriteLine("<- 0xC0 [Printer Init]");
								PaBus.Delay(250 * 1000);
								if (PaBus.WriteByte(0xFA)) Console.WriteLine("-> 0xFA");
								PaBus.End();
								break;
							case 0x59:
								Console.WriteLine("<- 0x59 [Printer row]");
								length = PaBus.ReadBytes(printer_row, 128);
								Console.WriteLine("<- [{0:D} bytes]", length);
								if (PaBus.WriteByte(0xFA)) Console.WriteLine("-> 0xFA");
								break;
							case 0x70:
								Console.WriteLine("<- 0x70 [End print]");
								if (PaBus.WriteByte(0xFA)) Console.WriteLine("-> 0xFA");
								PaBus.End();
								break;
							case 0x44:
								Console.WriteLine("<- 0x44 [Save to cassette]");
								length = PaBus.ReadBytes(cassette_header, 130);
								Console.WriteLine("<- [{0:D} bytes]", length);
								cassete_data_size = (ushort)((cassette_header[18] << 8) | (cassette_header[19] << 0));
								length = PaBus.ReadBytes(cassette_data, (uint)(cassete_data_size + 2));
								Console.WriteLine("<- [{0:D} bytes]", length);
								if (PaBus.WriteByte(0xFA)) Console.WriteLine("-> 0xFA");
								PaBus.End();
								break;
							default:
								Console.WriteLine("<- 0x{0:X2} [???]", value);
								PaBus.End();
								break;
						}
					}
				}

				PaBus.Close();
			}*/

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Ensure there's at least one serial port on the machine.
			if (SerialPort.GetPortNames().Length < 1) {
				MessageBox.Show("There are no serial ports on this computer.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Application.Run(new Main());
		}
	}
}
