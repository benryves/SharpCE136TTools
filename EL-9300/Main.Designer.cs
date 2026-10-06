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
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.programToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.linkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.receiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enableEL9300InterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.serialPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.arrangeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ungroupFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 539);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(784, 22);
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
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.windowToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(784, 24);
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.programToolStripMenuItem});
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.newToolStripMenuItem.Text = "&New";
			// 
			// programToolStripMenuItem
			// 
			this.programToolStripMenuItem.Name = "programToolStripMenuItem";
			this.programToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
			this.programToolStripMenuItem.Text = "&Program";
			this.programToolStripMenuItem.Click += new System.EventHandler(this.ProgramToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
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
			this.sendToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.sendToolStripMenuItem.Text = "&Send...";
			this.sendToolStripMenuItem.Click += new System.EventHandler(this.SendToolStripMenuItem_Click);
			// 
			// receiveToolStripMenuItem
			// 
			this.receiveToolStripMenuItem.Name = "receiveToolStripMenuItem";
			this.receiveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
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
			// windowToolStripMenuItem
			// 
			this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arrangeIconsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.cascadeToolStripMenuItem,
            this.horizontalToolStripMenuItem,
            this.verticalToolStripMenuItem,
            this.toolStripMenuItem3,
            this.closeAllToolStripMenuItem});
			this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
			this.windowToolStripMenuItem.Text = "&Window";
			this.windowToolStripMenuItem.DropDownOpening += new System.EventHandler(this.WindowToolStripMenuItem_DropDownOpening);
			// 
			// arrangeIconsToolStripMenuItem
			// 
			this.arrangeIconsToolStripMenuItem.Name = "arrangeIconsToolStripMenuItem";
			this.arrangeIconsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.arrangeIconsToolStripMenuItem.Text = "Arrange &Icons";
			this.arrangeIconsToolStripMenuItem.Click += new System.EventHandler(this.ArrangeIconsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
			// 
			// cascadeToolStripMenuItem
			// 
			this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
			this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.cascadeToolStripMenuItem.Text = "&Cascade";
			this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
			// 
			// horizontalToolStripMenuItem
			// 
			this.horizontalToolStripMenuItem.Name = "horizontalToolStripMenuItem";
			this.horizontalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.horizontalToolStripMenuItem.Text = "&Horizontal";
			this.horizontalToolStripMenuItem.Click += new System.EventHandler(this.HorizontalToolStripMenuItem_Click);
			// 
			// verticalToolStripMenuItem
			// 
			this.verticalToolStripMenuItem.Name = "verticalToolStripMenuItem";
			this.verticalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.verticalToolStripMenuItem.Text = "&Vertical";
			this.verticalToolStripMenuItem.Click += new System.EventHandler(this.VerticalToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(177, 6);
			// 
			// closeAllToolStripMenuItem
			// 
			this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
			this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.closeAllToolStripMenuItem.Text = "Close &All";
			this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
			this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupFilesToolStripMenuItem,
            this.ungroupFilesToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// groupFilesToolStripMenuItem
			// 
			this.groupFilesToolStripMenuItem.Enabled = false;
			this.groupFilesToolStripMenuItem.Name = "groupFilesToolStripMenuItem";
			this.groupFilesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.groupFilesToolStripMenuItem.Text = "&Group Files...";
			// 
			// ungroupFilesToolStripMenuItem
			// 
			this.ungroupFilesToolStripMenuItem.Name = "ungroupFilesToolStripMenuItem";
			this.ungroupFilesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.ungroupFilesToolStripMenuItem.Text = "&Ungroup Files...";
			this.ungroupFilesToolStripMenuItem.Click += new System.EventHandler(this.UngroupFilesToolStripMenuItem_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
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
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem programToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem arrangeIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem horizontalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticalToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem groupFilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ungroupFilesToolStripMenuItem;
	}
}

