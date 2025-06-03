using Sharp.CE136T;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace Sharp.CE50P {

	public delegate TResult Func<out TResult>();

	public partial class Main : Form {

		#region Startup

		public Main() {
			InitializeComponent();
		}

		private void Main_Shown(object sender, EventArgs e) {
			ScrollPaperToBottom();
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
			backgroundWorker.RunWorkerAsync();
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
			enablePrinterCassetteInterfaceToolStripMenuItem.Checked = PaBus.IsOpen();
			// Mark whether the "print immediately" setting is active.
			printImmediatelyToolStripMenuItem.Checked = Properties.Settings.Default.PrintImmediately;
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

		private void EnablePrinterCassetteInterfaceToolStripMenuItem_Click(object sender, EventArgs e) {
			// If we're open, close; if we're close, open.
			ToggleInterface();
		}

		private void PrintImmediatelyToolStripMenuItem_Click(object sender, EventArgs e) {
			// Toggle the "print immediately" setting.
			Properties.Settings.Default.PrintImmediately ^= true;
		}

		#endregion

		#region Printer menu

		private void FeedPaperToolStripMenuItem_Click(object sender, EventArgs e) {
			FeedPaper(1);
		}

		private void PaperCutToolStripMenuItem_Click(object sender, EventArgs e) {
			paper.Controls.Clear();
			paper.Height = paper.MinimumSize.Height;
			UpdatePaperScrollBar();
		}

		#endregion

		#region Cassette menu

		private void SaveRecordingsToolStripMenuItem_Click(object sender, EventArgs e) {

			// What are we saving?
			var blocks = this.tapeBlocks.ToArray();
			if (blocks.Length < 1) {
				MessageBox.Show(this, "There are no recordings to save.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// Is there a suitable default filename?
			string filename = null;
			foreach (var block in tapeBlocks) {
				if (filename == null) {
					filename = block.InfoName;
				} else if (filename != block.InfoName) {
					filename = null;
					break;
				}
			}

			if (filename != null) {
				saveCassetteRecordingsDialog.FileName = filename + ".tap";
			}

			// Save all of the blocks
			bool saving;
			do {
				if (saving = (saveCassetteRecordingsDialog.ShowDialog(this) == DialogResult.OK)) {
					try {
						using (var cassette = File.Create(saveCassetteRecordingsDialog.FileName)) {
							foreach (var block in blocks) {
								block.WriteToStream(cassette);
							}
						}
						saving = false;
					} catch (Exception ex) {
						saving = MessageBox.Show(this, string.Format("Could not save file: {0}", ex.Message), Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
					}
				}
			} while (saving);
		}

		private void OpenRecordingsToolStripMenuItem_Click(object sender, EventArgs e) {

			bool loading;
			do {
				if (loading = (openCassetteRecordingsDialog.ShowDialog(this) == DialogResult.OK)) {
					try {
						// Load all of the blocks
						var blocks = new List<TapeBlock>();
						using (var cassette = File.OpenRead(openCassetteRecordingsDialog.FileName)) {
							while (cassette.Position < cassette.Length) {
								blocks.Add(TapeBlock.FromStream(cassette));
							}
						}
						loading = false;
						// Success!
						this.tapeBlocks.Clear();
						foreach (var block in blocks) this.tapeBlocks.Enqueue(block);
					} catch (Exception ex) {
						loading = MessageBox.Show(this, string.Format("Could not load file: {0}", ex.Message), Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
					}
				}
			} while (loading);
		}

		#endregion

		#region Protocol handler

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {

			int idleLoops = 0;

			for (; ; ) {

				if (backgroundWorker.CancellationPending) {
					e.Cancel = true;
					return;
				}

				if (PaBus.Read(out byte value)) {
					
					idleLoops = 0;

					switch (value) {
						case 0xAA:
						case 0xA5:
							Debug.WriteLine(string.Format("<- 0x{0:X2} [Identify]", value));
							paBusWriter.Write((ushort)0x01F0);
							break;
						case 0xC0:
							Debug.WriteLine("<- 0xC0 [Printer Init]");
							// Report that a print job has been started
							Invoke(new MethodInvoker(PrinterStartedJob));
							// Acknowledge
							paBusStream.WriteAcknowledgement();
							paBusStream.Flush();
							break;
						case 0x59:
							Debug.WriteLine("<- 0x59 [Printer row]");
							Invoke(new MethodInvoker(PrinterPrintingRow));
							// Fetch 128 bytes of row data
							var row = paBusReader.ReadBytes(128);
							// Report that a row has been printed
							Invoke(new Action<byte[]>(PrinterPrintedRow), row);
							// Acknowledge
							paBusStream.WriteAcknowledgement();
							break;
						case 0x70:
							Debug.WriteLine("<- 0x70 [End print]");
							// Report that the job has finished
							Invoke(new MethodInvoker(PrinterFinishedJob));
							// Acknowledge
							paBusStream.WriteAcknowledgement();
							paBusStream.Flush();
							break;
						case 0x44:
							Debug.WriteLine("<- 0x44 [Save to cassette]");
							// Report that the recording has started
							Invoke(new MethodInvoker(CassetteRecordingBlock));
							// Read the information
							var cassetteInfo = paBusReader.ReadBytes(130);
							// Read the data
							ushort cassetteDataSize = (ushort)((cassetteInfo[18] << 8) | (cassetteInfo[19] << 0));
							var cassetteData = paBusReader.ReadBytes(cassetteDataSize + 2);
							// Build the tape block
							TapeBlock recordBlock = TapeBlock.FromBytesWithChecksum(cassetteInfo, cassetteData);
							// Report that we've received the tape block
							Invoke(new Action<TapeBlock>(CassetteRecordedBlock), recordBlock);
							// Acknowledge
							paBusStream.WriteAcknowledgement();
							paBusStream.Flush();
							break;
						case 0x33:
							Debug.WriteLine("<- 0x33 [Load from cassette]");
							// Get the 18-byte request (not sure what this is?)
							var tapeLoadRequest = paBusReader.ReadBytes(18);
							
							// Display the request
							Debug.Write(string.Format("<- [{0:D} bytes]:", tapeLoadRequest.Length));
							foreach (var b in tapeLoadRequest) Debug.Write(string.Format(" 0x{0:X2}", b));
							Debug.WriteLine("");

							// Try to load the block to play back
							if (Invoke(new Func<TapeBlock>(CassettePlayingBlock)) is TapeBlock playBlock) {

								var playData = TapeBlock.GetBytesWithChecksum(playBlock.Info);

								Debug.WriteLine(string.Format("-> [Info: {0:D} bytes]", playData.Length));

								// Acknowledge the load request
								paBusStream.WriteAcknowledgement();

								// Start sending data
								paBusWriter.Write(playData);

								paBusStream.ReadAcknowledgement();

								playData = TapeBlock.GetBytesWithChecksum(playBlock.Data);
								Debug.WriteLine(string.Format("-> [Data: {0:D} bytes]", playData.Length));

								paBusWriter.Write(playData);

								// We have success!
								Invoke(new Action<TapeBlock>(CassettePlayedBlock), playBlock);
								Debug.WriteLine("-> Load from cassette success!");

							}
							paBusStream.Flush();
							break;
						default:
							Debug.WriteLine(string.Format("<- 0x{0:X2} [???]", value));
							paBusStream.Flush();
							break;
					}
				} else {
					if (idleLoops > 10) {
						// Clear any active status
						var statusChanged = false;
						if (isPrinting) {
							isPrinting = false;
							statusChanged = true;
						}
						if (isRecording) {
							isRecording = false;
							statusChanged = true;
						}
						if (isPlaying) {
							isPlaying = false;
							statusChanged = true;
						}
						if (statusChanged) {
							Invoke(new MethodInvoker(UpdateStatus));
						}
						Thread.Sleep(10);
					} else {
						idleLoops++;
					}
				}
			}

		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			// Can't do much if we've been disposed.
			if (IsDisposed) return;
			// Switch the interface off.
			SwitchOffInterface();
			// Update the status.
			isPrinting = isPlaying = isRecording = false;
			Invoke(new MethodInvoker(UpdateStatus));
			// Was it intentionally cancelled?
			if (e.Cancelled) return;
			// No, so there was some sort of problem.
			MessageBox.Show(e.Error == null ? "There was a problem." : e.Error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			// Start the background worker again.
			SwitchOnInterface();
		}

		#endregion

		#region Printer handling

		private readonly List<byte[]> printerRows = new List<byte[]>(8);

		private void PrinterStartedJob() {
			isPrinting = true;
			isPlaying = isRecording = false;
			printerRows.Clear();
			if (paper.Controls.Count > 0 && paper.Controls[paper.Controls.Count - 1] is PictureBox pictureBox && pictureBox.Image != null) {
				FeedPaper(1);
			}
			UpdateStatus();
		}

		private void PrinterPrintingRow() {
			isPrinting = true;
			isPlaying = isRecording = false;
			UpdateStatus();
		}

		private void PrinterPrintedRow(byte[] data) {
			isPrinting = true;
			isPlaying = isRecording = false;
			printerRows.Add(data);
			UpdateStatus();
			if (Properties.Settings.Default.PrintImmediately) PrinterFinishedJob();
		}

		private void PrinterFinishedJob() {
			if (printerRows.Count > 0) {

				var printBmp = new Bitmap(128, printerRows.Count * 8);
				for (int r = 0; r < printerRows.Count; ++r) {
					var top = r * 8;
					for (int x = 0; x < 128; ++x) {
						for (int y = 0, mask = 0x80; y < 8; ++y, mask >>= 1) {
							var c = (printerRows[r][x] & mask) != 0 ? Color.Black : Color.White;
							printBmp.SetPixel(x, y + top, c);
						}
					}
				}

				printerRows.Clear();
				AppendBitmapToPaper(printBmp);
			}

			isPrinting = isPlaying = isRecording = false;
			UpdateStatus();
		}

		private void FeedPaper(int rows) {

			if (paper.Controls.Count > 0) {

				// Remove previous separator if required for neatness
				if (paper.Controls.Count > 1) {
					if (paper.Controls[paper.Controls.Count - 1] is PictureBox a && a.Image == null &&
						paper.Controls[paper.Controls.Count - 2] is PictureBox b && b.Image == null
					) {
						paper.Controls.Remove(a);
						a.Dispose();
					}
				}

				// Create a new separator
				var separator = new PictureBox {
					Width = paper.Width,
					Height = 1,
					Top = (paper.Height - paper.Padding.Bottom) + 6,
				};
				separator.Paint += Separator_Paint;
				paper.Controls.Add(separator);
			}

			paper.Height += rows * 16;

			ScrollPaperToBottom();

		}

		private void Separator_Paint(object sender, PaintEventArgs e) {
			if (sender is PictureBox pictureBox) {
				e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				e.Graphics.Clear(paper.BackColor);
				Pen dashedPen = new Pen(Color.Black, 1) {
					DashPattern = new float[] { 1, 2, 1 }
				};
				e.Graphics.DrawLine(dashedPen, 0, 0, pictureBox.Width, 0);
			}
		}

		private void AppendBitmapToPaper(Bitmap bitmap) {

			var pictureBox = new PictureBox {
				Image = bitmap,
				Width = bitmap.Width * 2,
				Height = bitmap.Height * 2,
				Left = 16 * 2,
				Top = paper.Height - paper.Padding.Bottom,
				SizeMode = PictureBoxSizeMode.StretchImage,
				ContextMenuStrip = printContextMenu,
			};
			pictureBox.Paint += PrinterRow_Paint;
			paper.Controls.Add(pictureBox);

			paper.Height += bitmap.Height * 2;
			ScrollPaperToBottom();
		}

		private void PrinterRow_Paint(object sender, PaintEventArgs e) {
			if (sender is PictureBox pictureBox) {
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
				e.Graphics.DrawImage(pictureBox.Image, 0, 0, pictureBox.Width, pictureBox.Height);
			}
		}

		#endregion

		#region Cassette handling

		private readonly Queue<TapeBlock> tapeBlocks = new Queue<TapeBlock>();

		private void CassetteRecordingBlock() {
			isRecording = true;
			isPlaying = isPrinting = false;
			UpdateStatus();
		}

		private void CassetteRecordedBlock(TapeBlock block) {
			tapeBlocks.Enqueue(block);
			isRecording = isPlaying = isPrinting = false;
			UpdateStatus();
		}

		private TapeBlock CassettePlayingBlock() {

			// Persuade the user to open a tape file.
			if (this.tapeBlocks.Count < 1) {
				this.openRecordingsToolStripMenuItem.PerformClick();
			}

			if (this.tapeBlocks.Count > 0) {
				// We have a tape block to play.
				isPlaying = true;
				isRecording = isPrinting = false;
				UpdateStatus();
				return this.tapeBlocks.Dequeue();
			} else {
				// We don't have any tape blocks to play.
				isPlaying = isRecording = isPrinting = false;
				UpdateStatus();
				return null;
			}
		}

		private void CassettePlayedBlock(TapeBlock block) {
			// Advance to the next block?
		}

		#endregion

		#region Status

		private bool isPrinting = false;
		private bool isRecording = false;
		private bool isPlaying = false;

		private void UpdateStatus() {
			if (!PaBus.IsOpen()) {
				statusLabel.Text = "Disconnected";
				statusLabel.Image = Properties.Resources.IconDisconnect;
			} else if (isPrinting) {
				statusLabel.Text = "Printing";
				statusLabel.Image = Properties.Resources.IconPrinter;
			} else if (isRecording) {
				statusLabel.Text = "Recording";
				statusLabel.Image = Properties.Resources.IconCassetteRecord;
			} else if (isPlaying) {
				statusLabel.Text = "Playing";
				statusLabel.Image = Properties.Resources.IconCassettePlay;
			} else {
				statusLabel.Text = "Ready";
				statusLabel.Image = Properties.Resources.IconConnect;
			}
		}

		#endregion

		#region Paper scrolling

		/// <summary>
		/// Updates the paper vertical scrollbar to reflect the current form size and length of printout.
		/// </summary>
		void UpdatePaperScrollBar() {

			// Snap the paper to the bottom if need be
			if (paper.Bottom < paperTray.ClientSize.Height) {
				ScrollPaperToBottom();
				return;
			}

			// Align the paper to the centre of the tray
			paper.Left = (paperTray.Width - paper.Width) / 2;

			// How much scrollable material is there?
			var scrollableAmount = paper.Height - paperTray.ClientSize.Height + paperTray.Padding.Top;
			if (scrollableAmount <= 0) {
				// Nothing to scroll
				paperScrollBar.Enabled = false;
				paperScrollBar.Value = 0;
				paperScrollBar.Maximum = 0;
				paperScrollBar.LargeChange = 1;
				paper.Top = paperTray.ClientSize.Height - paper.Height;
			} else {
				// Something to scroll

				// The maximum is offset by the large change value - 1, so calculate that based on the current tray size
				var largeChange = Math.Max(paperScrollBar.SmallChange, paperTray.ClientSize.Height / 4);
				paperScrollBar.LargeChange = largeChange;
				var maximum = scrollableAmount + largeChange - 1;

				// Calculate the current scroll position based on the paper position
				if (paper.Top > paperTray.Padding.Top) paper.Top = paperTray.Padding.Top;
				var value = paperTray.Padding.Top - paper.Top;

				// Update the scroll bar
				if (value > paperScrollBar.Maximum) {
					paperScrollBar.Maximum = maximum;
					paperScrollBar.Value = value;
				} else {
					paperScrollBar.Value = value;
					paperScrollBar.Maximum = maximum;
				}

				paperScrollBar.Enabled = true;
			}

		}

		/// <summary>
		/// Scroll the printed paper to the bottom to show the most-recently printed content.
		/// </summary>
		void ScrollPaperToBottom() {
			paper.Top = paperTray.ClientSize.Height - paper.Height;
			UpdatePaperScrollBar();
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			UpdatePaperScrollBar();
			var newValue = Math.Max(paperScrollBar.Minimum, Math.Min(paperScrollBar.Maximum - paperScrollBar.LargeChange + 1, paperScrollBar.Value - e.Delta / 5));
			if (newValue != paperScrollBar.Value) {
				paperScrollBar.Value = newValue;
				paper.Top = paperTray.Padding.Top - newValue;
			}
			base.OnMouseWheel(e);
		}

		private void PaperTray_Resize(object sender, EventArgs e) {
			UpdatePaperScrollBar();
		}

		private void PaperScrollBar_Scroll(object sender, ScrollEventArgs e) {
			paper.Top = paperTray.Padding.Top - e.NewValue;
		}


		#endregion

		#region Printer region selection

		private PictureBox[] GetPictureBoxesFromPaper(PictureBox initial) {
			
			var initialIndex = paper.Controls.GetChildIndex(initial);
			if (initialIndex < 0) return null;

			var firstIndex = initialIndex;			
			while (firstIndex > 0 && paper.Controls[firstIndex - 1] is PictureBox firstPicture && firstPicture.Image != null) {
				--firstIndex;
			}

			var lastindex = initialIndex;
			while (lastindex < paper.Controls.Count - 1 && paper.Controls[lastindex + 1] is PictureBox lastPicture && lastPicture.Image != null) {
				++lastindex;
			}

			var result = new PictureBox[lastindex - firstIndex + 1];
			for (int i = 0; i < result.Length; ++i) {
				result[i] = (PictureBox)paper.Controls[i + firstIndex];
			}

			return result;

		}

		#endregion

		#region Printer context menu

		private PictureBox[] GetPrintContextPictureBoxes() {
			if (printContextMenu.SourceControl is PictureBox pictureBox) {
				return GetPictureBoxesFromPaper(pictureBox);
			} else {
				return null;
			}
		}

		private Bitmap GetPrintContextBitmap() {
			// Get all of the picture boxes that form the selection.
			var pictureBoxes = GetPrintContextPictureBoxes();
			if (pictureBoxes == null || pictureBoxes.Length < 1) {
				return null;
			}

			// What's the final height of the printout?
			int height = 0;
			foreach (var pictureBox in pictureBoxes) {
				height += pictureBox.Image.Height;
			}

			// Create the resulting Bitmap
			var bitmap = new Bitmap(128, height);

			// Copy all of the printer rows to the output.
			int y = 0;
			using (var graphics = Graphics.FromImage(bitmap)) {
				graphics.PixelOffsetMode = PixelOffsetMode.None;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				foreach (var pictureBox in pictureBoxes) {
					graphics.DrawImage(pictureBox.Image, 0, y);
					y += pictureBox.Image.Height;
				}
			}

			return bitmap;
		}


		private void CopyPrintToolStripMenuItem_Click(object sender, EventArgs e) {
			using (var bitmap = GetPrintContextBitmap()) {
				if (bitmap != null) {
					Clipboard.SetImage(bitmap);
				}
			}
		}

		private void SavePrintToolStripMenuItem_Click(object sender, EventArgs e) {
			using (var bitmap = GetPrintContextBitmap()) {
				if (bitmap != null) {
					bool saving;
					do {
						if (saving = (savePrintDialog.ShowDialog(this) == DialogResult.OK)) {
							try {
								var printFormat = ImageFormat.Png;
								switch (Path.GetExtension(savePrintDialog.FileName).ToLowerInvariant()) {
									case ".gif":
										printFormat = ImageFormat.Gif;
										break;
									case ".bmp":
										printFormat = ImageFormat.Bmp;
										break;
								}
								bitmap.Save(savePrintDialog.FileName, printFormat);
								saving = false;
							} catch (Exception ex) {
								saving = MessageBox.Show(this, string.Format("Could not save file: {0}", ex.Message), Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry;
							}
						}
					} while (saving);
				}
			}
		}

		#endregion

	}
}
