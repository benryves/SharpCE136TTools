namespace Sharp.EL9300 {
	partial class MatrixEditor {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.matrixDimensionsGroupBox = new System.Windows.Forms.GroupBox();
			this.matrixDimensionsTable = new System.Windows.Forms.TableLayoutPanel();
			this.matrixRowCountUpDown = new System.Windows.Forms.NumericUpDown();
			this.matrixDimensionsLabel = new System.Windows.Forms.Label();
			this.matrixColumnCountUpDown = new System.Windows.Forms.NumericUpDown();
			this.matrixNameGroupBox = new System.Windows.Forms.GroupBox();
			this.matrixNameComboBox = new System.Windows.Forms.ComboBox();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.matrixTypeTable = new System.Windows.Forms.TableLayoutPanel();
			this.matrixDimensionsGroupBox.SuspendLayout();
			this.matrixDimensionsTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.matrixRowCountUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.matrixColumnCountUpDown)).BeginInit();
			this.matrixNameGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.matrixTypeTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// matrixDimensionsGroupBox
			// 
			this.matrixDimensionsGroupBox.Controls.Add(this.matrixDimensionsTable);
			this.matrixDimensionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixDimensionsGroupBox.Location = new System.Drawing.Point(103, 3);
			this.matrixDimensionsGroupBox.Name = "matrixDimensionsGroupBox";
			this.matrixDimensionsGroupBox.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.matrixDimensionsGroupBox.Size = new System.Drawing.Size(144, 42);
			this.matrixDimensionsGroupBox.TabIndex = 1;
			this.matrixDimensionsGroupBox.TabStop = false;
			this.matrixDimensionsGroupBox.Text = "Dimensions";
			// 
			// matrixDimensionsTable
			// 
			this.matrixDimensionsTable.ColumnCount = 3;
			this.matrixDimensionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.matrixDimensionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.matrixDimensionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.matrixDimensionsTable.Controls.Add(this.matrixRowCountUpDown, 0, 0);
			this.matrixDimensionsTable.Controls.Add(this.matrixDimensionsLabel, 1, 0);
			this.matrixDimensionsTable.Controls.Add(this.matrixColumnCountUpDown, 2, 0);
			this.matrixDimensionsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixDimensionsTable.Location = new System.Drawing.Point(3, 13);
			this.matrixDimensionsTable.Margin = new System.Windows.Forms.Padding(0);
			this.matrixDimensionsTable.Name = "matrixDimensionsTable";
			this.matrixDimensionsTable.RowCount = 1;
			this.matrixDimensionsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.matrixDimensionsTable.Size = new System.Drawing.Size(138, 26);
			this.matrixDimensionsTable.TabIndex = 0;
			// 
			// matrixRowCountUpDown
			// 
			this.matrixRowCountUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixRowCountUpDown.Location = new System.Drawing.Point(3, 3);
			this.matrixRowCountUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.matrixRowCountUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.matrixRowCountUpDown.Name = "matrixRowCountUpDown";
			this.matrixRowCountUpDown.Size = new System.Drawing.Size(53, 20);
			this.matrixRowCountUpDown.TabIndex = 0;
			this.matrixRowCountUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.matrixRowCountUpDown.ValueChanged += new System.EventHandler(this.MatrixRowCountUpDown_ValueChanged);
			// 
			// matrixDimensionsLabel
			// 
			this.matrixDimensionsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixDimensionsLabel.Location = new System.Drawing.Point(62, 0);
			this.matrixDimensionsLabel.Name = "matrixDimensionsLabel";
			this.matrixDimensionsLabel.Size = new System.Drawing.Size(14, 26);
			this.matrixDimensionsLabel.TabIndex = 1;
			this.matrixDimensionsLabel.Text = "×";
			this.matrixDimensionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// matrixColumnCountUpDown
			// 
			this.matrixColumnCountUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixColumnCountUpDown.Location = new System.Drawing.Point(82, 3);
			this.matrixColumnCountUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.matrixColumnCountUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.matrixColumnCountUpDown.Name = "matrixColumnCountUpDown";
			this.matrixColumnCountUpDown.Size = new System.Drawing.Size(53, 20);
			this.matrixColumnCountUpDown.TabIndex = 1;
			this.matrixColumnCountUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.matrixColumnCountUpDown.ValueChanged += new System.EventHandler(this.MatrixColumnCountUpDown_ValueChanged);
			// 
			// matrixNameGroupBox
			// 
			this.matrixNameGroupBox.Controls.Add(this.matrixNameComboBox);
			this.matrixNameGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixNameGroupBox.Location = new System.Drawing.Point(3, 3);
			this.matrixNameGroupBox.Name = "matrixNameGroupBox";
			this.matrixNameGroupBox.Size = new System.Drawing.Size(94, 42);
			this.matrixNameGroupBox.TabIndex = 0;
			this.matrixNameGroupBox.TabStop = false;
			this.matrixNameGroupBox.Text = "Name";
			// 
			// matrixNameComboBox
			// 
			this.matrixNameComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matrixNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.matrixNameComboBox.FormattingEnabled = true;
			this.matrixNameComboBox.Location = new System.Drawing.Point(3, 16);
			this.matrixNameComboBox.Name = "matrixNameComboBox";
			this.matrixNameComboBox.Size = new System.Drawing.Size(88, 21);
			this.matrixNameComboBox.TabIndex = 0;
			this.matrixNameComboBox.SelectedIndexChanged += new System.EventHandler(this.MatrixNameComboBox_SelectedIndexChanged);
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.NullValue = "0";
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.NullValue = "0";
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 48);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridView.Size = new System.Drawing.Size(384, 213);
			this.dataGridView.TabIndex = 1;
			this.dataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.DataGridView_CellPainting);
			this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DataGridView_CellValidating);
			this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValueChanged);
			this.dataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.DataGridView_RowPostPaint);
			// 
			// matrixTypeTable
			// 
			this.matrixTypeTable.AutoSize = true;
			this.matrixTypeTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.matrixTypeTable.ColumnCount = 2;
			this.matrixTypeTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.matrixTypeTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.matrixTypeTable.Controls.Add(this.matrixNameGroupBox, 0, 0);
			this.matrixTypeTable.Controls.Add(this.matrixDimensionsGroupBox, 1, 0);
			this.matrixTypeTable.Dock = System.Windows.Forms.DockStyle.Top;
			this.matrixTypeTable.Location = new System.Drawing.Point(0, 0);
			this.matrixTypeTable.MaximumSize = new System.Drawing.Size(250, 48);
			this.matrixTypeTable.Name = "matrixTypeTable";
			this.matrixTypeTable.RowCount = 1;
			this.matrixTypeTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.matrixTypeTable.Size = new System.Drawing.Size(250, 48);
			this.matrixTypeTable.TabIndex = 1;
			// 
			// MatrixEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 261);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.matrixTypeTable);
			this.Name = "MatrixEditor";
			this.Text = "Matrix Editor";
			this.matrixDimensionsGroupBox.ResumeLayout(false);
			this.matrixDimensionsTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.matrixRowCountUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.matrixColumnCountUpDown)).EndInit();
			this.matrixNameGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.matrixTypeTable.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox matrixDimensionsGroupBox;
		private System.Windows.Forms.TableLayoutPanel matrixDimensionsTable;
		private System.Windows.Forms.GroupBox matrixNameGroupBox;
		private System.Windows.Forms.ComboBox matrixNameComboBox;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.TableLayoutPanel matrixTypeTable;
		private System.Windows.Forms.NumericUpDown matrixRowCountUpDown;
		private System.Windows.Forms.NumericUpDown matrixColumnCountUpDown;
		private System.Windows.Forms.Label matrixDimensionsLabel;
	}
}