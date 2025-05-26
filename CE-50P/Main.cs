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
			if (backgroundWorker.IsBusy) {
				// Background worker is still running...
				return false;
			} else if (Array.FindIndex(SerialPort.GetPortNames(), portName => portName == serialPortName) < 0) {
				// We haven't selected a valid serial port yet.
				MessageBox.Show(this, "Please select a serial port from the options menu.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			} else if (!PaBus.Open(serialPortName)) {
				// There is a serial port in the settings, but it couldn't be selected.
				MessageBox.Show(this, string.Format("Could not use serial port '{0}'.", serialPortName), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			} else {
				// All good!
				backgroundWorker.RunWorkerAsync();
				return true;
			}
		}

		/// <summary>
		/// Switch off the virtual printer/cassette interface.
		/// </summary>
		private void SwitchOffInterface() {
			PaBus.Close();
			backgroundWorker.CancelAsync();
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

		#region Protocol handler

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {

			byte[] cassetteHeader = new byte[130];
			byte[] cassetteData = new byte[64 * 1024 + 2];
			ushort cassetteDataSize;

			int idleLoops = 0;

			uint length;
			for (; ; ) {

				if (backgroundWorker.CancellationPending) {
					e.Cancel = true;
					return;
				}

				if (PaBus.ReadByte(out byte value)) {
					idleLoops = 0;
					switch (value) {
						case 0xAA:
						case 0xA5:
							Debug.WriteLine("<- 0xAA [Identify]");
							if (PaBus.WriteByte(0xF0)) Debug.WriteLine("-> 0xF0");
							if (PaBus.WriteByte(0x01)) Debug.WriteLine("-> 0x01");
							break;
						case 0xC0:
							Debug.WriteLine("<- 0xC0 [Printer Init]");
							if (PaBus.WriteByte(0xFA)) Debug.WriteLine("-> 0xFA");
							PaBus.End();
							Invoke(new MethodInvoker(PrinterStartedJob));
							break;
						case 0x59:
							Debug.WriteLine("<- 0x59 [Printer row]");
							var row = new byte[128];
							length = PaBus.ReadBytes(row, 128);
							Debug.WriteLine(string.Format("<- [{0:D} bytes]", length));
							if (PaBus.WriteByte(0xFA)) Debug.WriteLine("-> 0xFA");
							Invoke(new Action<byte[]>(PrinterPrintedRow), row);
							break;
						case 0x70:
							Debug.WriteLine("<- 0x70 [End print]");
							if (PaBus.WriteByte(0xFA)) Debug.WriteLine("-> 0xFA");
							PaBus.End();
							Invoke(new MethodInvoker(PrinterFinishedJob));
							break;
						case 0x44:
							Debug.WriteLine("<- 0x44 [Save to cassette]");
							length = PaBus.ReadBytes(cassetteHeader, 130);
							Debug.WriteLine(string.Format("<- [{0:D} bytes]", length));
							cassetteDataSize = (ushort)((cassetteHeader[18] << 8) | (cassetteHeader[19] << 0));
							length = PaBus.ReadBytes(cassetteData, (uint)(cassetteDataSize + 2));
							Debug.WriteLine(string.Format("<- [{0:D} bytes]", length));
							if (PaBus.WriteByte(0xFA)) Debug.WriteLine("-> 0xFA");
							PaBus.End();
							break;
						default:
							Debug.WriteLine(string.Format("<- 0x{0:X2} [???]", value));
							PaBus.End();
							break;
					}
				} else if (idleLoops > 10) {
					Thread.Sleep(10);
				} else {
					idleLoops++;
				}
			}

		}

		private readonly List<byte[]> printerRows = new List<byte[]>(8);

		private void PrinterStartedJob() {
			printerRows.Clear();
			if (paper.Controls.Count > 0 && paper.Controls[paper.Controls.Count - 1] is PictureBox pictureBox && pictureBox.Image != null) {
				FeedPaper(1);
			}
		}

		private void PrinterPrintedRow(byte[] data) {
			printerRows.Add(data);
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
