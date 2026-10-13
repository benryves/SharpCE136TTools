using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class MatrixEditor : Form, IEditorForm {
		public MatrixEditor() {
			InitializeComponent();

			// Populate name dropdown
			for (char name = 'A'; name <= 'Z'; ++name) {
				matrixNameComboBox.Items.Add(name);
			}
			matrixNameComboBox.SelectedIndex = 0;

			// Set a suitable default matrix
			MatrixItems = new RealNumber[1, 1];

			// Prevent mouse wheel from affecting dimensions
			matrixRowCountUpDown.MouseWheel += MatrixDimensionCountUpDown_MouseWheel;
			matrixColumnCountUpDown.MouseWheel += MatrixDimensionCountUpDown_MouseWheel;

			// Clear dirty flag
			Dirty = false;
		}

		#region Matrix properties

		public char MatrixName {
			get => (char)matrixNameComboBox.SelectedItem;
			set {
				if (value < 'A' || value > 'Z') throw new ArgumentOutOfRangeException("Name must be between 'A' and 'Z'.");
				matrixNameComboBox.SelectedItem = value;
				Dirty = true;
			}
		}

		bool updatingRows = false;
		public int MatrixRowCount {
			get {
				var rowCount = 0;
				foreach (DataGridViewRow row in dataGridView.Rows) {
					if (!row.IsNewRow && row.Visible) ++rowCount;
				}
				return rowCount;
			}
			set {
				if (!updatingRows && dataGridView.DataSource is DataTable dataTable) {
					dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
					if (value != MatrixRowCount) {
						try {
							updatingRows = true;

							// Add new rows as required
							while (dataTable.Rows.Count < value) {
								dataTable.Rows.Add();
							}

							// Ensure all rows are visible
							for (int rowIndex = MatrixRowCount; rowIndex < value; ++rowIndex) {
								dataGridView.Rows[rowIndex].Visible = true;
							}

							// Hiding selected rows throws an exception from the CurrencyManager binding, so temporarily suspend binding
							var currencyManager = BindingContext[dataTable] as CurrencyManager;
							currencyManager?.SuspendBinding();
							
							// Hide old rows as required
							while (MatrixRowCount > value) {
								dataGridView.Rows[MatrixRowCount - 1].Visible = false;
							}

							// Resume binding with the currency manager
							currencyManager?.ResumeBinding();

							// Update row count box
							matrixRowCountUpDown.Value = MatrixRowCount;

						} finally {
							updatingRows = false;
						}
						Dirty = true;
					}
				}
			}
		}

		bool updatingColumns = false;
		public int MatrixColumnCount {
			get {
				var columnCount = 0;
				foreach (DataGridViewColumn column in dataGridView.Columns) {
					if (column.Visible) ++columnCount;
				}
				return columnCount;
			}
			set {
				if (!updatingColumns && dataGridView.DataSource is DataTable dataTable) {
					dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
					if (value != MatrixColumnCount) {
						try {
							updatingColumns = true;

							// Add new columns as required
							while (dataTable.Columns.Count < value) {
								var column = dataTable.Columns.Count;
								var columnName = "Column" + column.ToString(CultureInfo.InvariantCulture);
								dataTable.Columns.Add(new DataColumn(columnName, typeof(RealNumber)));
								dataGridView.Columns[column].SortMode = DataGridViewColumnSortMode.NotSortable;
							}

							// Ensure all columns are visible
							for (int columnIndex = MatrixColumnCount; columnIndex < value; ++columnIndex) {
								dataGridView.Columns[columnIndex].Visible = true;
							}

							// Remove old columns as required
							while (MatrixColumnCount > value) {
								dataGridView.Columns[MatrixColumnCount - 1].Visible = false;
							}

							// Update column count box
							matrixColumnCountUpDown.Value = MatrixColumnCount;

						} finally {
							updatingColumns = false;
						}
						Dirty = true;
					}
				}
			}
		}

		public RealNumber[,] MatrixItems {
			get {
				// Ensure any pending changes are committed
				dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

				// Grab the data
				var dataTable = (DataTable)dataGridView.DataSource;

				// Pull all of the data into the array
				int rows = MatrixRowCount, columns = MatrixColumnCount;
				var items = new RealNumber[rows, columns];
				for (int row = 0; row < rows; ++row) {
					var tableRow = dataTable.Rows[row];
					for (int column = 0; column < columns; ++column) {
						items[row, column] = (RealNumber)tableRow[column];
					}
				}

				return items;
			}
			set {
				// Build a new data table
				var dataTable = new DataTable();

				var rowCount = value.GetLength(0);
				var columnCount = value.GetLength(1);

				for (int column = 0; column < columnCount; ++column) {
					dataTable.Columns.Add(new DataColumn("Column" + column.ToString(CultureInfo.InvariantCulture), typeof(RealNumber)));
				}

				for (int row = 0; row < rowCount; ++row) {
					var cells = new object[columnCount];
					for (int column = 0; column < columnCount; ++column) {
						cells[column] = value[row, column];
					}
					dataTable.Rows.Add(cells);
				}

				// Set the data source to the new table
				dataGridView.DataSource = dataTable;

				// Prevent columns from being sortable
				for (int column = 0; column < dataGridView.ColumnCount; ++column) {
					dataGridView.Columns[column].SortMode = DataGridViewColumnSortMode.NotSortable;
				}

				// Update row/column count boxes
				matrixRowCountUpDown.Value = rowCount;
				matrixColumnCountUpDown.Value = columnCount;

				Dirty = true;

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

		public string FileDialogFilter => "EL-9300 Matrix (*.g1m)|*.g1m";
		public string FileDialogFileName => MatrixName.ToString();

		public LinkTransfer Save() {
			return new MatrixVariable(MatrixName, MatrixItems).ToLinkTransfer();
		}

		public void Open(LinkTransfer transfer) {
			var matrixFile = new MatrixVariable(transfer);
			MatrixName = matrixFile.Name;
			MatrixItems = matrixFile.Items;
		}

		#endregion

		#region Editing

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

		private void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
			if (sender is DataGridView grid) {
				if (e.RowIndex == -1 && e.ColumnIndex >= 0) {

					e.PaintBackground(e.ClipBounds, true);

					var columnIndex = (e.ColumnIndex + 1).ToString(CultureInfo.InvariantCulture);

					var centerFormat = new StringFormat() {
						Alignment = StringAlignment.Far,
						LineAlignment = StringAlignment.Center
					};

					var textSize = TextRenderer.MeasureText(columnIndex, grid.Font);
					if (grid.RowHeadersWidth < textSize.Width + 20)
						grid.RowHeadersWidth = textSize.Width + 20;

					var headerBounds = new Rectangle(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width - 4, e.CellBounds.Height);

					e.Graphics.DrawString(columnIndex, grid.Font, SystemBrushes.ControlText, headerBounds, centerFormat);

					e.Handled = true;
				}
			}
		}

		private void MatrixRowCountUpDown_ValueChanged(object sender, EventArgs e) {
			MatrixRowCount = (int)matrixRowCountUpDown.Value;
		}

		private void MatrixColumnCountUpDown_ValueChanged(object sender, EventArgs e) {
			MatrixColumnCount = (int)matrixColumnCountUpDown.Value;
		}

		private void MatrixNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			MatrixName = (char)matrixNameComboBox.SelectedItem;
			Dirty = true;
		}

		private void MatrixDimensionCountUpDown_MouseWheel(object sender, MouseEventArgs e) {
			if (e is HandledMouseEventArgs he) {
				he.Handled = true;
			}
		}

		#endregion
	}
}
