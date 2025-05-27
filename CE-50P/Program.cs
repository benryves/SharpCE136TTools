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
