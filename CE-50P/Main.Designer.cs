namespace Sharp.CE50P {
	partial class Main {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.printerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.feedPaperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutPaperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enablePrinterCassetteInterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.serialPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolsStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.printImmediatelyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.paper = new System.Windows.Forms.Panel();
			this.paperTray = new System.Windows.Forms.Panel();
			this.paperScrollBar = new System.Windows.Forms.VScrollBar();
			this.printContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.savePrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyPrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.savePrintDialog = new System.Windows.Forms.SaveFileDialog();
			this.menuStrip.SuspendLayout();
			this.paperTray.SuspendLayout();
			this.printContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.printerToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(490, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
			// 
			// printerToolStripMenuItem
			// 
			this.printerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.feedPaperToolStripMenuItem,
            this.cutPaperToolStripMenuItem});
			this.printerToolStripMenuItem.Name = "printerToolStripMenuItem";
			this.printerToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.printerToolStripMenuItem.Text = "&Printer";
			// 
			// feedPaperToolStripMenuItem
			// 
			this.feedPaperToolStripMenuItem.Name = "feedPaperToolStripMenuItem";
			this.feedPaperToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
			this.feedPaperToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.feedPaperToolStripMenuItem.Text = "&Feed Paper";
			this.feedPaperToolStripMenuItem.Click += new System.EventHandler(this.FeedPaperToolStripMenuItem_Click);
			// 
			// cutPaperToolStripMenuItem
			// 
			this.cutPaperToolStripMenuItem.Name = "cutPaperToolStripMenuItem";
			this.cutPaperToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
			this.cutPaperToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.cutPaperToolStripMenuItem.Text = "&Cut Paper";
			this.cutPaperToolStripMenuItem.Click += new System.EventHandler(this.PaperCutToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enablePrinterCassetteInterfaceToolStripMenuItem,
            this.serialPortToolStripMenuItem,
            this.optionsToolsStripSeparator,
            this.printImmediatelyToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			this.optionsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.OptionsToolStripMenuItem_DropDownOpening);
			// 
			// enablePrinterCassetteInterfaceToolStripMenuItem
			// 
			this.enablePrinterCassetteInterfaceToolStripMenuItem.Name = "enablePrinterCassetteInterfaceToolStripMenuItem";
			this.enablePrinterCassetteInterfaceToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
			this.enablePrinterCassetteInterfaceToolStripMenuItem.Text = "&Enable Printer/Cassette Interface";
			this.enablePrinterCassetteInterfaceToolStripMenuItem.Click += new System.EventHandler(this.EnablePrinterCassetteInterfaceToolStripMenuItem_Click);
			// 
			// serialPortToolStripMenuItem
			// 
			this.serialPortToolStripMenuItem.Name = "serialPortToolStripMenuItem";
			this.serialPortToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
			this.serialPortToolStripMenuItem.Text = "&Serial Port";
			// 
			// optionsToolsStripSeparator
			// 
			this.optionsToolsStripSeparator.Name = "optionsToolsStripSeparator";
			this.optionsToolsStripSeparator.Size = new System.Drawing.Size(242, 6);
			// 
			// printImmediatelyToolStripMenuItem
			// 
			this.printImmediatelyToolStripMenuItem.Name = "printImmediatelyToolStripMenuItem";
			this.printImmediatelyToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
			this.printImmediatelyToolStripMenuItem.Text = "Print &Immediately";
			this.printImmediatelyToolStripMenuItem.Click += new System.EventHandler(this.PrintImmediatelyToolStripMenuItem_Click);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
			// 
			// paper
			// 
			this.paper.BackColor = System.Drawing.Color.White;
			this.paper.Location = new System.Drawing.Point(16, 16);
			this.paper.Margin = new System.Windows.Forms.Padding(0);
			this.paper.MaximumSize = new System.Drawing.Size(320, 0);
			this.paper.MinimumSize = new System.Drawing.Size(320, 48);
			this.paper.Name = "paper";
			this.paper.Padding = new System.Windows.Forms.Padding(0, 0, 0, 16);
			this.paper.Size = new System.Drawing.Size(320, 48);
			this.paper.TabIndex = 1;
			// 
			// paperTray
			// 
			this.paperTray.BackColor = System.Drawing.SystemColors.ControlDark;
			this.paperTray.Controls.Add(this.paper);
			this.paperTray.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paperTray.Location = new System.Drawing.Point(0, 24);
			this.paperTray.Name = "paperTray";
			this.paperTray.Padding = new System.Windows.Forms.Padding(16, 16, 0, 0);
			this.paperTray.Size = new System.Drawing.Size(473, 303);
			this.paperTray.TabIndex = 2;
			this.paperTray.Resize += new System.EventHandler(this.PaperTray_Resize);
			// 
			// paperScrollBar
			// 
			this.paperScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.paperScrollBar.Location = new System.Drawing.Point(473, 24);
			this.paperScrollBar.Name = "paperScrollBar";
			this.paperScrollBar.Size = new System.Drawing.Size(17, 303);
			this.paperScrollBar.TabIndex = 2;
			this.paperScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.PaperScrollBar_Scroll);
			// 
			// printContextMenu
			// 
			this.printContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.savePrintToolStripMenuItem,
            this.copyPrintToolStripMenuItem});
			this.printContextMenu.Name = "printContextMenu";
			this.printContextMenu.Size = new System.Drawing.Size(144, 48);
			// 
			// savePrintToolStripMenuItem
			// 
			this.savePrintToolStripMenuItem.Name = "savePrintToolStripMenuItem";
			this.savePrintToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.savePrintToolStripMenuItem.Text = "&Save Image...";
			this.savePrintToolStripMenuItem.Click += new System.EventHandler(this.SavePrintToolStripMenuItem_Click);
			// 
			// copyPrintToolStripMenuItem
			// 
			this.copyPrintToolStripMenuItem.Name = "copyPrintToolStripMenuItem";
			this.copyPrintToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.copyPrintToolStripMenuItem.Text = "&Copy Image";
			this.copyPrintToolStripMenuItem.Click += new System.EventHandler(this.CopyPrintToolStripMenuItem_Click);
			// 
			// savePrintDialog
			// 
			this.savePrintDialog.Filter = "PNG (*.png)|*.png|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 327);
			this.Controls.Add(this.paperTray);
			this.Controls.Add(this.paperScrollBar);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.MinimumSize = new System.Drawing.Size(420, 320);
			this.Name = "Main";
			this.Text = "CE-50P";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.Shown += new System.EventHandler(this.Main_Shown);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.paperTray.ResumeLayout(false);
			this.printContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem serialPortToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator optionsToolsStripSeparator;
		private System.Windows.Forms.ToolStripMenuItem enablePrinterCassetteInterfaceToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.Panel paper;
		private System.Windows.Forms.Panel paperTray;
		private System.Windows.Forms.VScrollBar paperScrollBar;
		private System.Windows.Forms.ToolStripMenuItem printImmediatelyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem printerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem feedPaperToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutPaperToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip printContextMenu;
		private System.Windows.Forms.ToolStripMenuItem copyPrintToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem savePrintToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog savePrintDialog;
	}
}

