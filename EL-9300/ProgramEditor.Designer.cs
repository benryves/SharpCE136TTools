namespace Sharp.EL9300 {
	partial class ProgramEditor {
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
			this.programNameTextBox = new System.Windows.Forms.TextBox();
			this.programCodeTextBox = new System.Windows.Forms.TextBox();
			this.programTypeComboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// programNameTextBox
			// 
			this.programNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.programNameTextBox.Location = new System.Drawing.Point(0, 0);
			this.programNameTextBox.MaxLength = 16;
			this.programNameTextBox.Name = "programNameTextBox";
			this.programNameTextBox.Size = new System.Drawing.Size(398, 20);
			this.programNameTextBox.TabIndex = 0;
			this.programNameTextBox.TextChanged += new System.EventHandler(this.ProgramNameTextBox_TextChanged);
			// 
			// programCodeTextBox
			// 
			this.programCodeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.programCodeTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.programCodeTextBox.Location = new System.Drawing.Point(0, 41);
			this.programCodeTextBox.Multiline = true;
			this.programCodeTextBox.Name = "programCodeTextBox";
			this.programCodeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.programCodeTextBox.Size = new System.Drawing.Size(398, 412);
			this.programCodeTextBox.TabIndex = 1;
			this.programCodeTextBox.TextChanged += new System.EventHandler(this.ProgramCodeTextBox_TextChanged);
			// 
			// programTypeComboBox
			// 
			this.programTypeComboBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.programTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.programTypeComboBox.FormattingEnabled = true;
			this.programTypeComboBox.Location = new System.Drawing.Point(0, 20);
			this.programTypeComboBox.Name = "programTypeComboBox";
			this.programTypeComboBox.Size = new System.Drawing.Size(398, 21);
			this.programTypeComboBox.TabIndex = 2;
			this.programTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ProgramTypeComboBox_SelectedIndexChanged);
			// 
			// ProgramEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(398, 453);
			this.Controls.Add(this.programCodeTextBox);
			this.Controls.Add(this.programTypeComboBox);
			this.Controls.Add(this.programNameTextBox);
			this.Name = "ProgramEditor";
			this.Text = "Program";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox programNameTextBox;
		private System.Windows.Forms.TextBox programCodeTextBox;
		private System.Windows.Forms.ComboBox programTypeComboBox;
	}
}