using Sharp.CE136T;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class Main : Form {

		#region Startup

		public Main() {
			InitializeComponent();
		}

		private void Main_Shown(object sender, System.EventArgs e) {
			// By default, open the serial port specified in the settings.
			SwitchOnInterface();
		}

		#endregion

		#region Shutdown

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}

		private void Main_FormClosed(object sender, FormClosedEventArgs e) {
			// Close the serial port and save our settings.
			SwitchOffInterface();
			Properties.Settings.Default.Save();
		}

		#endregion

		#region Interface enabling/disabling

		PaBusStream paBusStream;
		BinaryReader paBusReader;
		BinaryWriter paBusWriter;

		/// <summary>
		/// Switch on the virtual printer/cassette interface using the serial port name stored in the settings.
		/// </summary>
		/// <returns><c>true</c> if the interface was switched on successfully, <c>false</c> otherwise.</returns>
		private bool SwitchOnInterface() {
			return SwitchOnInterface(Properties.Settings.Default.SerialPort);
		}

		/// <summary>
		/// Switch on the virtual printer/cassette interface using a specific serial port name.
		/// </summary>
		/// <param name="serialPortName">The name of the serial port to use.</param>
		/// <returns><c>true</c> if the interface was switched on successfully, <c>false</c> otherwise.</returns>
		private bool SwitchOnInterface(string serialPortName) {
			// Ensure the status is appropriate set to start with.
			UpdateStatus();
			if (backgroundWorker.IsBusy) {
				// Background worker is still running...
				return false;
			} else if (Array.FindIndex(SerialPort.GetPortNames(), portName => portName == serialPortName) < 0) {
				// We haven't selected a valid serial port yet.
				MessageBox.Show(this, "Please select a serial port from the options menu.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}

			try {
				// Open the stream and connect a reader and writer to it.
				paBusStream = new PaBusStream(serialPortName);
				paBusReader = new BinaryReader(paBusStream);
				paBusWriter = new BinaryWriter(paBusStream);
			} catch (Exception ex) {
				// There is a serial port in the settings, but it couldn't be selected.
				UpdateStatus();
				MessageBox.Show(this, string.Format("Could not use serial port '{0}': {1}", serialPortName, ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			// All good!
			UpdateStatus();
			return true;
		}

		/// <summary>
		/// Switch off the virtual printer/cassette interface.
		/// </summary>
		private void SwitchOffInterface() {
			if (backgroundWorker.IsBusy) backgroundWorker.CancelAsync();
			paBusStream?.Close();
			paBusStream?.Dispose();
			UpdateStatus();
			paBusStream = null;
			paBusReader = null;
			paBusWriter = null;
		}

		/// <summary>
		/// Check if the virtual printer/cassette interface is currently switched on or not.
		/// </summary>
		/// <returns><c>true</c> if it's switched on, <c>false</c> otherwise.</returns>
		private bool InterfaceIsOn() {
			return PaBus.IsOpen();
		}

		/// <summary>
		/// Toggle whether the interface is currently switched on or not.
		/// </summary>
		private void ToggleInterface() {
			if (InterfaceIsOn()) {
				SwitchOffInterface();
			} else {
				SwitchOnInterface();
			}
		}

		#endregion

		#region Options menu

		private void OptionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
			// Populate the list of serial ports.
			serialPortToolStripMenuItem.DropDownItems.Clear();
			serialPortToolStripMenuItem.Enabled = false;
			foreach (var portName in SerialPort.GetPortNames()) {
				var serialPortMenuItem = new ToolStripMenuItem(portName) {
					Tag = portName,
					Checked = portName == Properties.Settings.Default.SerialPort,
				};
				serialPortToolStripMenuItem.DropDownItems.Add(serialPortMenuItem);
				serialPortMenuItem.Click += SerialPortMenuItem_Click;
			}
			serialPortToolStripMenuItem.Enabled = serialPortToolStripMenuItem.DropDownItems.Count > 0;
			// Mark whether the port is currently open or not.
			enableEL9300InterfaceToolStripMenuItem.Checked = PaBus.IsOpen();
		}

		private void SerialPortMenuItem_Click(object sender, EventArgs e) {
			// Switch to the new serial port.
			if (sender is ToolStripMenuItem serialPortMenuItem && serialPortMenuItem.Tag is string serialPortName) {
				if (InterfaceIsOn() && !backgroundWorker.CancellationPending) {
					backgroundWorker.CancelAsync();
					while (backgroundWorker.IsBusy) {
						Application.DoEvents();
					}
				}
				if (SwitchOnInterface(serialPortName)) {
					Properties.Settings.Default.SerialPort = serialPortName;
				} else {
					MessageBox.Show(this, string.Format("Could not use serial port '{0}'.", serialPortName), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void EnableEL9300InterfaceToolStripMenuItem_Click(object sender, EventArgs e) {
			// If we're open, close; if we're close, open.
			ToggleInterface();
		}

		#endregion

		#region Status

		private bool isReceiving = false;
		private bool isSending = false;

		private void UpdateStatus() {
			if (!PaBus.IsOpen()) {
				statusLabel.Text = "Disconnected";
				statusLabel.Image = Properties.Resources.IconDisconnect;
			} else if (isReceiving) {
				statusLabel.Text = "Receiving";
				statusLabel.Image = Properties.Resources.IconReceive;
			} else if (isSending) {
				statusLabel.Text = "Sending";
				statusLabel.Image = Properties.Resources.IconSend;
			} else {
				statusLabel.Text = "Ready";
				statusLabel.Image = Properties.Resources.IconConnect;
			}
		}

		#endregion

		#region Protocol handler

		const ushort CalculatorDeviceID = 0x00C0;
		const int ProgressStepBytes = 64;

		private bool LinkQueryDevice() {
			if (!PaBus.Write(0xA5)) return false;
			try {
				return paBusReader.ReadUInt16() == CalculatorDeviceID;
			} catch {
				return false;
			}
		}

		private bool LinkReportDevice() {
			if (!PaBus.Read(out byte command)) return false;
			if (command != 0xA5) return false;
			try {
				paBusWriter.Write(CalculatorDeviceID);
				return true;
			} catch {
				return false;
			}
		}

		private byte[] LinkReadPacket(int size, int progressStep, Action<int> progressCallback) {
			// Read the packet
			var packet = new byte[size];
			for (int offset = 0; offset < size; ) {
				progressCallback?.Invoke(offset);
				int read = paBusStream.Read(packet, offset, Math.Min(size - offset, progressStep));
				if (read > 0) {
					offset += read;
					progressCallback?.Invoke(offset);
				} else {
					throw new TimeoutException("Could not read data.");
				}
			}
			progressCallback?.Invoke(size);

			// Read the checksum
			var checksum = paBusReader.ReadUInt16();

			// Validate the checksum
			for (int i = 0; i < packet.Length; ++i) {
				checksum -= packet[i];
			}
			if (checksum != 0) throw new InvalidDataException("Checksum mismatch.");
			return packet;
		}

		private byte[] LinkReadPacket(int size) {
			return LinkReadPacket(size, size, null);
		}

		private void LinkWritePacket(byte[] packet, int progressStep, Action<int> progressCallback) {
			
			// Write the packet data
			for (int offset = 0; offset < packet.Length; offset += progressStep) {
				progressCallback?.Invoke(offset);
				int written = Math.Min(packet.Length - offset, progressStep);
				paBusStream.Write(packet, offset, written);
			}
			progressCallback?.Invoke(packet.Length);

			// Write the checksum
			ushort checksum = 0;
			for (int i = 0; i < packet.Length; ++i) {
				checksum += packet[i];
			}
			paBusWriter.Write(checksum);
		}

		private void LinkWritePacket(byte[] packet) {
			LinkWritePacket(packet, packet.Length, null);
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			// Ensure we know who's working
			var worker = (BackgroundWorker)sender;

			// What's being transferred?
			LinkTransfer linkTransfer = e.Argument as LinkTransfer;
			if (e.Argument != null) {
				isSending = true;
				isReceiving = false;
			} else {
				isReceiving = true;
				isSending = false;
			}

			Invoke((MethodInvoker)UpdateStatus);

			// Start from -1
			worker.ReportProgress(-1);

			// Handle sending
			if (isSending) {

				// Synchronise
				Debug.Write("Waiting to send...");
				while (!LinkQueryDevice()) {
					if (worker.CancellationPending) {
						Debug.WriteLine("Cancelled");
						return;
					}
					Debug.Write(".");
				}
				Debug.WriteLine("OK");
				paBusStream.Flush();

				// Send device ID
				Debug.Write("Reporting device ID...");
				while (!LinkReportDevice()) {
					if (worker.CancellationPending) {
						Debug.WriteLine("Cancelled");
						return;
					}
					Debug.Write(".");
				}
				Debug.WriteLine("OK");

				// Write the head packet
				Debug.Write("Writing head packet...");
				LinkWritePacket(linkTransfer.Head);
				paBusStream.ReadAcknowledgement();
				Debug.WriteLine("OK");

				// Re-synchronise
				Debug.Write("Waiting to send...");
				while (!LinkQueryDevice()) {
					if (worker.CancellationPending) {
						Debug.WriteLine("Cancelled");
						return;
					}
					Debug.Write(".");
				}
				Debug.WriteLine("OK");

				Debug.Write("Writing body packet...");
				LinkWritePacket(linkTransfer.Body, ProgressStepBytes, p => {
					if (linkTransfer.Body.Length > 0) {
						worker.ReportProgress(p * 100 / linkTransfer.Body.Length);
					}
				});
				paBusStream.ReadAcknowledgement();
				paBusStream.Flush();
				Debug.WriteLine("OK");
			}

			// Handle receiving
			if (isReceiving) {

				// Synchronise
				Debug.Write("Waiting to receive...");
				for (; ; ) {
					// Try to respond to a device request
					while (!LinkReportDevice()) {
						if (worker.CancellationPending) {
							Debug.WriteLine("Cancelled");
							return;
						}
						Debug.Write(".");
					}
					// Request the sender's device
					if (LinkQueryDevice()) break;
				}
				Debug.WriteLine("OK");
				paBusStream.Flush();

				// Ready to start reading
				worker.ReportProgress(0);

				// Read the head packet
				Debug.Write("Reading head packet...");
				var headPacket = LinkReadPacket(32);
				paBusStream.WriteAcknowledgement();
				paBusStream.Flush();
				Debug.WriteLine("OK");

				// Re-synchronise
				Debug.Write("Waiting for body packet...");
				if (!LinkReportDevice()) {
					Debug.WriteLine("Failed");
					throw new InvalidDataException("Could not get data for variable.");
				} else {
					paBusStream.Flush();
					Debug.WriteLine("OK");
				}

				Debug.Write("Reading body packet...");
				int bodySize = headPacket[3] + (headPacket[4] << 8);
				var bodyPacket = LinkReadPacket(bodySize, ProgressStepBytes, p => {
					if (bodySize > 0) {
						worker.ReportProgress(p * 100 / bodySize);
					}
				});
				paBusStream.WriteAcknowledgement();
				paBusStream.Flush();
				Debug.WriteLine("OK");

				// Return the transfer
				e.Result = new LinkTransfer(headPacket, bodyPacket);
			}

		}

		private ProgressDialog progressDialog;

		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			var showProgressDialog = false;
			if (progressDialog == null) {
				progressDialog = new ProgressDialog();
				progressDialog.FormClosing += ProgressDialog_FormClosing;
				showProgressDialog = true;
			}
			if (e.ProgressPercentage < 0) {
				if (isReceiving) progressDialog.Text = "Waiting to Receive...";
				if (isSending) progressDialog.Text = "Waiting to Send...";
				progressDialog.Style = ProgressBarStyle.Marquee;
			} else if (e.ProgressPercentage <= 100) {
				if (isReceiving) progressDialog.Text = "Receiving";
				if (isSending) progressDialog.Text = "Sending";
				progressDialog.Style = ProgressBarStyle.Continuous;
				progressDialog.Value = e.ProgressPercentage;
			}
			if (showProgressDialog) {
				progressDialog.ShowDialog(this);
			}
		}

		private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e) {
			if (backgroundWorker.IsBusy) {
				backgroundWorker.CancelAsync();
				e.Cancel = true;
			}
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			// Can't do much if we've been disposed.
			if (IsDisposed) return;

			// Clean up the progress dialog
			if (progressDialog != null) {
				progressDialog.FormClosing -= ProgressDialog_FormClosing;
				progressDialog.Close();
				progressDialog.Dispose();
				progressDialog = null;
			}

			// Switch the interface off.
			SwitchOffInterface();
			// Update the status.
			isSending = isReceiving = false;
			Invoke(new MethodInvoker(UpdateStatus));
			// Was it intentionally cancelled?
			if (e.Cancelled) return;
			// No, so there was some sort of problem.
			if (e.Error != null) {
				MessageBox.Show(e.Error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			// Is there a file to save?
			if (e.Result is LinkTransfer linkTransfer) {
				var saved = false;
				while (!saved && saveFileDialog.ShowDialog(this) == DialogResult.OK) {
					try {
						linkTransfer.Save(saveFileDialog.FileName);
						saved = true;
					} catch (Exception ex) {
						MessageBox.Show("Could not save the file: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}

			// Start the background worker again.
			SwitchOnInterface();
		}


		private void LinkToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
			sendToolStripMenuItem.Enabled = sendToolStripMenuItem.Enabled = InterfaceIsOn();
		}

		private void SendToolStripMenuItem_Click(object sender, EventArgs e) {
			if (backgroundWorker.IsBusy || !InterfaceIsOn()) return;
			// Try to get the variable to transfer
			LinkTransfer linkTransfer = null;
			while (linkTransfer == null && openFileDialog.ShowDialog(this) == DialogResult.OK) {
				try {
					linkTransfer = new LinkTransfer(openFileDialog.FileName);
				} catch (Exception ex) {
					MessageBox.Show("Could not load the file: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			// If we have something to send, kick off the background worker
			if (linkTransfer != null) {
				backgroundWorker.RunWorkerAsync(linkTransfer);
			}
		}

		private void ReceiveToolStripMenuItem_Click(object sender, EventArgs e) {
			if (backgroundWorker.IsBusy || !InterfaceIsOn()) return;
			// Kick off the background worker without a variable to transfer; this will receive a file
			backgroundWorker.RunWorkerAsync();
		}

		#endregion

	}
}
