namespace Sharp.EL9300 {
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
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.linkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.receiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enableEL9300InterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.serialPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.statusStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 204);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(351, 22);
			this.statusStrip.TabIndex = 1;
			this.statusStrip.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.Image = global::Sharp.EL9300.Properties.Resources.IconDisconnect;
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(95, 17);
			this.statusLabel.Text = "Disconnected";
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.linkToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(351, 24);
			this.menuStrip.TabIndex = 2;
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
			// linkToolStripMenuItem
			// 
			this.linkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToolStripMenuItem,
            this.receiveToolStripMenuItem});
			this.linkToolStripMenuItem.Name = "linkToolStripMenuItem";
			this.linkToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.linkToolStripMenuItem.Text = "&Link";
			this.linkToolStripMenuItem.DropDownOpening += new System.EventHandler(this.LinkToolStripMenuItem_DropDownOpening);
			// 
			// sendToolStripMenuItem
			// 
			this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
			this.sendToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.sendToolStripMenuItem.Text = "&Send...";
			this.sendToolStripMenuItem.Click += new System.EventHandler(this.SendToolStripMenuItem_Click);
			// 
			// receiveToolStripMenuItem
			// 
			this.receiveToolStripMenuItem.Name = "receiveToolStripMenuItem";
			this.receiveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.receiveToolStripMenuItem.Text = "&Receive...";
			this.receiveToolStripMenuItem.Click += new System.EventHandler(this.ReceiveToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableEL9300InterfaceToolStripMenuItem,
            this.serialPortToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			this.optionsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.OptionsToolStripMenuItem_DropDownOpening);
			// 
			// enableEL9300InterfaceToolStripMenuItem
			// 
			this.enableEL9300InterfaceToolStripMenuItem.Name = "enableEL9300InterfaceToolStripMenuItem";
			this.enableEL9300InterfaceToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.enableEL9300InterfaceToolStripMenuItem.Text = "&Enable EL-9300 Interface";
			this.enableEL9300InterfaceToolStripMenuItem.Click += new System.EventHandler(this.EnableEL9300InterfaceToolStripMenuItem_Click);
			// 
			// serialPortToolStripMenuItem
			// 
			this.serialPortToolStripMenuItem.Name = "serialPortToolStripMenuItem";
			this.serialPortToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.serialPortToolStripMenuItem.Text = "&Serial Port";
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
			this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(351, 226);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menuStrip;
			this.Name = "Main";
			this.Text = "EL-9300";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.Shown += new System.EventHandler(this.Main_Shown);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem linkToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sendToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem receiveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem enableEL9300InterfaceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem serialPortToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
	}
}

