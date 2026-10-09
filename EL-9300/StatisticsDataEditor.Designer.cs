namespace Sharp.EL9300 {
	partial class StatisticsDataEditor {
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
			this.variableCountGroupBox = new System.Windows.Forms.GroupBox();
			this.variableCountTable = new System.Windows.Forms.TableLayoutPanel();
			this.twoVarRadioButton = new System.Windows.Forms.RadioButton();
			this.oneVarRadioButton = new System.Windows.Forms.RadioButton();
			this.variableWeightGroupBox = new System.Windows.Forms.GroupBox();
			this.weightedVarCheckBox = new System.Windows.Forms.CheckBox();
			this.variableTypeTable = new System.Windows.Forms.TableLayoutPanel();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.variableCountGroupBox.SuspendLayout();
			this.variableCountTable.SuspendLayout();
			this.variableWeightGroupBox.SuspendLayout();
			this.variableTypeTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// variableCountGroupBox
			// 
			this.variableCountGroupBox.Controls.Add(this.variableCountTable);
			this.variableCountGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.variableCountGroupBox.Location = new System.Drawing.Point(3, 3);
			this.variableCountGroupBox.Name = "variableCountGroupBox";
			this.variableCountGroupBox.Size = new System.Drawing.Size(150, 42);
			this.variableCountGroupBox.TabIndex = 0;
			this.variableCountGroupBox.TabStop = false;
			this.variableCountGroupBox.Text = "Variables";
			// 
			// variableCountTable
			// 
			this.variableCountTable.ColumnCount = 2;
			this.variableCountTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
			this.variableCountTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
			this.variableCountTable.Controls.Add(this.twoVarRadioButton, 1, 0);
			this.variableCountTable.Controls.Add(this.oneVarRadioButton, 0, 0);
			this.variableCountTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.variableCountTable.Location = new System.Drawing.Point(3, 16);
			this.variableCountTable.Margin = new System.Windows.Forms.Padding(0);
			this.variableCountTable.Name = "variableCountTable";
			this.variableCountTable.RowCount = 1;
			this.variableCountTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.variableCountTable.Size = new System.Drawing.Size(144, 23);
			this.variableCountTable.TabIndex = 0;
			// 
			// twoVarRadioButton
			// 
			this.twoVarRadioButton.AutoSize = true;
			this.twoVarRadioButton.Location = new System.Drawing.Point(67, 3);
			this.twoVarRadioButton.Name = "twoVarRadioButton";
			this.twoVarRadioButton.Size = new System.Drawing.Size(60, 17);
			this.twoVarRadioButton.TabIndex = 1;
			this.twoVarRadioButton.TabStop = true;
			this.twoVarRadioButton.Text = "&2 (X, Y)";
			this.twoVarRadioButton.UseVisualStyleBackColor = true;
			this.twoVarRadioButton.CheckedChanged += new System.EventHandler(this.VariableCountRadioButton_CheckedChanged);
			// 
			// oneVarRadioButton
			// 
			this.oneVarRadioButton.AutoSize = true;
			this.oneVarRadioButton.Location = new System.Drawing.Point(3, 3);
			this.oneVarRadioButton.Name = "oneVarRadioButton";
			this.oneVarRadioButton.Size = new System.Drawing.Size(47, 17);
			this.oneVarRadioButton.TabIndex = 0;
			this.oneVarRadioButton.TabStop = true;
			this.oneVarRadioButton.Text = "&1 (X)";
			this.oneVarRadioButton.UseVisualStyleBackColor = true;
			this.oneVarRadioButton.CheckedChanged += new System.EventHandler(this.VariableCountRadioButton_CheckedChanged);
			// 
			// variableWeightGroupBox
			// 
			this.variableWeightGroupBox.Controls.Add(this.weightedVarCheckBox);
			this.variableWeightGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.variableWeightGroupBox.Location = new System.Drawing.Point(159, 3);
			this.variableWeightGroupBox.Name = "variableWeightGroupBox";
			this.variableWeightGroupBox.Padding = new System.Windows.Forms.Padding(7);
			this.variableWeightGroupBox.Size = new System.Drawing.Size(122, 42);
			this.variableWeightGroupBox.TabIndex = 1;
			this.variableWeightGroupBox.TabStop = false;
			this.variableWeightGroupBox.Text = "Weighting";
			// 
			// weightedVarCheckBox
			// 
			this.weightedVarCheckBox.AutoSize = true;
			this.weightedVarCheckBox.Location = new System.Drawing.Point(7, 20);
			this.weightedVarCheckBox.Name = "weightedVarCheckBox";
			this.weightedVarCheckBox.Size = new System.Drawing.Size(92, 17);
			this.weightedVarCheckBox.TabIndex = 0;
			this.weightedVarCheckBox.Text = "&Weighted (W)";
			this.weightedVarCheckBox.UseVisualStyleBackColor = true;
			this.weightedVarCheckBox.CheckedChanged += new System.EventHandler(this.WeightedVarCheckBox_CheckedChanged);
			// 
			// variableTypeTable
			// 
			this.variableTypeTable.AutoSize = true;
			this.variableTypeTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.variableTypeTable.ColumnCount = 2;
			this.variableTypeTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
			this.variableTypeTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
			this.variableTypeTable.Controls.Add(this.variableCountGroupBox, 0, 0);
			this.variableTypeTable.Controls.Add(this.variableWeightGroupBox, 1, 0);
			this.variableTypeTable.Dock = System.Windows.Forms.DockStyle.Top;
			this.variableTypeTable.Location = new System.Drawing.Point(0, 0);
			this.variableTypeTable.MaximumSize = new System.Drawing.Size(400, 48);
			this.variableTypeTable.Name = "variableTypeTable";
			this.variableTypeTable.RowCount = 1;
			this.variableTypeTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.variableTypeTable.Size = new System.Drawing.Size(284, 48);
			this.variableTypeTable.TabIndex = 2;
			// 
			// dataGridView
			// 
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 48);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(284, 313);
			this.dataGridView.TabIndex = 3;
			// 
			// StatisticsDataEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 361);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.variableTypeTable);
			this.MinimumSize = new System.Drawing.Size(300, 150);
			this.Name = "StatisticsDataEditor";
			this.Text = "Statistics Data";
			this.variableCountGroupBox.ResumeLayout(false);
			this.variableCountTable.ResumeLayout(false);
			this.variableCountTable.PerformLayout();
			this.variableWeightGroupBox.ResumeLayout(false);
			this.variableWeightGroupBox.PerformLayout();
			this.variableTypeTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox variableCountGroupBox;
		private System.Windows.Forms.TableLayoutPanel variableCountTable;
		private System.Windows.Forms.RadioButton twoVarRadioButton;
		private System.Windows.Forms.RadioButton oneVarRadioButton;
		private System.Windows.Forms.GroupBox variableWeightGroupBox;
		private System.Windows.Forms.CheckBox weightedVarCheckBox;
		private System.Windows.Forms.TableLayoutPanel variableTypeTable;
		private System.Windows.Forms.DataGridView dataGridView;
	}
}