using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class StatisticsDataEditor : Form, IEditorForm {

		#region Variable Count

		private int variableCount = 2;
		public int VariableCount {
			get => variableCount;
			set {
				if (value < 1 || value > 2) {
					throw new ArgumentException("Variable count can only be 1 or 2.");
				} else {
					if (value != variableCount) {
						variableCount = value;
						dataGridView.Columns["Y"].Visible = variableCount > 1;
						oneVarRadioButton.Checked = variableCount == 1;
						twoVarRadioButton.Checked = variableCount == 2;
					}
				}
			}
		}

		private void VariableCountRadioButton_CheckedChanged(object sender, EventArgs e) {
			if (oneVarRadioButton.Checked) {
				VariableCount = 1;
			} else if (twoVarRadioButton.Checked) {
				VariableCount = 2;
			}
		}

		#endregion

		#region Variable Weighting

		private bool variableWeighted = true;
		public bool VariableWeighted {
			get => variableWeighted;
			set {
				if (value != variableWeighted) {
					variableWeighted = value;
					dataGridView.Columns["W"].Visible = variableWeighted;
					weightedVarCheckBox.Checked = variableWeighted;
				}
			}
		}

		private void WeightedVarCheckBox_CheckedChanged(object sender, EventArgs e) {
			VariableWeighted = weightedVarCheckBox.Checked;
		}

		#endregion

		public StatisticsDataEditor() {
			InitializeComponent();

			var dataTable = new DataTable();

			var allColumns = new[] { "X", "Y", "W" };

			foreach (var column in allColumns) {
				dataTable.Columns.Add(new DataColumn(column, typeof(RealNumber)));
			}

			dataGridView.DataSource = dataTable;
			
			foreach (var column in allColumns) {
				dataGridView.Columns[column].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
				dataGridView.Columns[column].SortMode = DataGridViewColumnSortMode.NotSortable;
			}

			VariableCount = 1;
			VariableWeighted = false;

		}

		#region IEditorForm

		void UpdateText() {
			string text = "Untitled";
			if (!string.IsNullOrEmpty(FileName)) {
				text = Path.GetFileName(FileName);
			}
			if (Dirty) text += "*";
			if (Text != text) Text = text;
		}

		string fileName;
		public string FileName { get => fileName; set { fileName = value; UpdateText(); } }

		bool dirty;
		public bool Dirty { get => dirty; set { dirty = value; UpdateText(); } }

		public string FileDialogFilter => "EL-9300 Statistics Data (*.g1l)|*.g1l";
		public string FileDialogFileName => "DATA";

		public LinkTransfer Save() {
			var dataTable = (DataTable)dataGridView.DataSource;

			// Convert the data to a matrix
			var matrix = new MatrixVariable('@', VariableCount + (VariableWeighted ? 1 : 0), dataTable.Rows.Count) {
				Name = '@',
				Mode = MatrixMode.Statistics | (VariableWeighted ? MatrixMode.Weighted : MatrixMode.None),
			};

			// Populate the matrix, bearing in mind the rows and columns are flipped
			for (int row = 0; row < dataTable.Rows.Count; ++row) {
				var dataTableRow = dataTable.Rows[row];
				for (int col = 0; col < matrix.RowCount; ++col) {
					matrix.Items[col, row] = (RealNumber)dataTableRow[col];
				}
			}

			// Save as a link transfer
			return matrix.ToLinkTransfer();
		}
		
		public void Open(LinkTransfer transfer) {

			// Decode the data as a matrix
			var matrix = new MatrixVariable(transfer);

			// Check the matrix has statistics data in it
			if ((matrix.Mode & MatrixMode.Statistics) == 0) throw new InvalidDataException("Specified matrix is not flagged as statistics data.");
			if (matrix.Name != '@') throw new InvalidDataException("Specified matrix is not named '@' for statistics data.");

			// Set the weighting/variable count
			VariableWeighted = (matrix.Mode & MatrixMode.Weighted) != 0;
			VariableCount = matrix.RowCount - (VariableWeighted ? 1 : 0);

			// Pull the data out of the matrix and into the editor
			var dataTable = (DataTable)dataGridView.DataSource;
			dataTable.Rows.Clear();

			// Stats data has one column per card
			for (int col = 0; col < matrix.ColumnCount; ++col) {
				var dataRow = dataTable.Rows.Add();
				dataRow.ItemArray = Array.ConvertAll(matrix.GetColumn(col), n => (object)n);
			}
		}

		#endregion





	}
}
