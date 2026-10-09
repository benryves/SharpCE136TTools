using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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
						Dirty = true;
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
					Dirty = true;
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

			// Initialise suitable defaults
			VariableCount = 1;
			VariableWeighted = false;

			// Clear the dirty flag
			Dirty = false;

		}

		#region Data Grid

		private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) {
			if (sender is DataGridView grid) {
				string rowIndex = (e.RowIndex + 1).ToString(CultureInfo.InvariantCulture);

				if (dataGridView.Rows[e.RowIndex].IsNewRow) return;

				var centerFormat = new StringFormat() {
					Alignment = StringAlignment.Far,
					LineAlignment = StringAlignment.Center
				};

				var textSize = TextRenderer.MeasureText(rowIndex, grid.Font);
				if (grid.RowHeadersWidth < textSize.Width + 20)
					grid.RowHeadersWidth = textSize.Width + 20;

				var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth - 4, e.RowBounds.Height);

				e.Graphics.DrawString(rowIndex, grid.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
			}
		}


		private void DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
			if (dataGridView.Rows[e.RowIndex].IsNewRow && string.IsNullOrEmpty(e.FormattedValue as string)) {
				return;
			} else if (!RealNumber.TryParse(e.FormattedValue as string, out _)) {
				MessageBox.Show(this, "Please enter a valid numeric value.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Cancel = true;
			}
		}

		private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			Dirty = true;
		}

		private void DataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e) {
			if (e.Row != null) {
				e.Row.Cells["X"].Value = (RealNumber)0;
				e.Row.Cells["Y"].Value = (RealNumber)0;
				e.Row.Cells["W"].Value = (RealNumber)1;
			}
		}

		#endregion

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

			// Ensure any pending changes are committed
			dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

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
				var items = new object[] { (RealNumber)0, (RealNumber)0, (RealNumber)1 };
				for (int row = 0; row < matrix.RowCount; ++row) {
					items[row] = matrix.Items[row, col];
				}
				dataRow.ItemArray = items;
			}
		}

		#endregion
	}
}
