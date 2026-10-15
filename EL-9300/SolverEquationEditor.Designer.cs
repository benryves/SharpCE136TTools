namespace Sharp.EL9300 {
	partial class SolverEquationEditor {
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
			this.equationNameTextBox = new System.Windows.Forms.TextBox();
			this.equationNameGroupBox = new System.Windows.Forms.GroupBox();
			this.equationNameTableView = new System.Windows.Forms.TableLayoutPanel();
			this.equationEditor = new Sharp.EL9300.EquationEditor();
			this.equationNameGroupBox.SuspendLayout();
			this.equationNameTableView.SuspendLayout();
			this.SuspendLayout();
			// 
			// equationNameTextBox
			// 
			this.equationNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.equationNameTextBox.Location = new System.Drawing.Point(3, 16);
			this.equationNameTextBox.MaxLength = 16;
			this.equationNameTextBox.Name = "equationNameTextBox";
			this.equationNameTextBox.Size = new System.Drawing.Size(381, 20);
			this.equationNameTextBox.TabIndex = 1;
			this.equationNameTextBox.TextChanged += new System.EventHandler(this.EquationNameTextBox_TextChanged);
			// 
			// equationNameGroupBox
			// 
			this.equationNameGroupBox.Controls.Add(this.equationNameTextBox);
			this.equationNameGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.equationNameGroupBox.Location = new System.Drawing.Point(3, 3);
			this.equationNameGroupBox.Name = "equationNameGroupBox";
			this.equationNameGroupBox.Size = new System.Drawing.Size(387, 42);
			this.equationNameGroupBox.TabIndex = 2;
			this.equationNameGroupBox.TabStop = false;
			this.equationNameGroupBox.Text = "Name";
			// 
			// equationNameTableView
			// 
			this.equationNameTableView.ColumnCount = 1;
			this.equationNameTableView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.equationNameTableView.Controls.Add(this.equationNameGroupBox, 0, 0);
			this.equationNameTableView.Dock = System.Windows.Forms.DockStyle.Top;
			this.equationNameTableView.Location = new System.Drawing.Point(0, 0);
			this.equationNameTableView.MaximumSize = new System.Drawing.Size(400, 48);
			this.equationNameTableView.MinimumSize = new System.Drawing.Size(0, 48);
			this.equationNameTableView.Name = "equationNameTableView";
			this.equationNameTableView.RowCount = 1;
			this.equationNameTableView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.equationNameTableView.Size = new System.Drawing.Size(393, 48);
			this.equationNameTableView.TabIndex = 3;
			// 
			// equationEditor
			// 
			this.equationEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.equationEditor.Equation = new byte[] {
        ((byte)(255))};
			this.equationEditor.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.equationEditor.Location = new System.Drawing.Point(0, 48);
			this.equationEditor.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.equationEditor.Name = "equationEditor";
			this.equationEditor.Size = new System.Drawing.Size(393, 206);
			this.equationEditor.TabIndex = 4;
			// 
			// SolverEquationEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(393, 254);
			this.Controls.Add(this.equationEditor);
			this.Controls.Add(this.equationNameTableView);
			this.Name = "SolverEquationEditor";
			this.Text = "Solver Equation";
			this.equationNameGroupBox.ResumeLayout(false);
			this.equationNameGroupBox.PerformLayout();
			this.equationNameTableView.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox equationNameTextBox;
		private System.Windows.Forms.GroupBox equationNameGroupBox;
		private System.Windows.Forms.TableLayoutPanel equationNameTableView;
		private EquationEditor equationEditor;
	}
}