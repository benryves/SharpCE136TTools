using System;
using System.IO;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class SolverEquationEditor : Form, IEditorForm {
		public SolverEquationEditor() {
			InitializeComponent();
		}

		#region Equation Properties

		public string EquationName {
			get => equationNameTextBox.Text;
			set => equationNameTextBox.Text = value;
		}

		public byte[] Equation {
			get => equationEditor.Equation;
			set => equationEditor.Equation = value;
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

		public string FileDialogFilter => "EL-9300 Solver Equation (*.g1e)|*.g1e";
		public string FileDialogFileName => EquationName;

		public LinkTransfer Save() {
			throw new NotImplementedException();
		}

		public void Open(LinkTransfer transfer) {
			var solverEquationFile = new SolverEquationVariable(transfer);
			EquationName = solverEquationFile.Name;
			Equation = solverEquationFile.Equation;
		}

		#endregion

		#region Editing

		private void EquationNameTextBox_TextChanged(object sender, EventArgs e) {
			Dirty = true;
		}

		#endregion

	}
}
