using System;
using System.IO;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class ProgramEditor : Form, IEditorForm {

		public ProgramEditor() {
			InitializeComponent();
			foreach (ProgramMode mode in Enum.GetValues(typeof(ProgramMode))) {
				programTypeComboBox.Items.Add(mode);
			}
			UpdateText();
		}

		#region Program properties

		public string ProgramName {
			get => programNameTextBox.Text;
			set => programNameTextBox.Text = value;
		}

		public ProgramMode ProgramMode {
			get => (ProgramMode)programTypeComboBox.SelectedItem;
			set => programTypeComboBox.SelectedItem = value;
		}

		public string ProgramCode {
			get => programCodeTextBox.Text;
			set => programCodeTextBox.Text = value;
		}

		public string[] ProgramLines {
			get => programCodeTextBox.Lines;
			set => programCodeTextBox.Lines = value;
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
		public bool Dirty { get=>dirty; set { dirty = value; UpdateText(); } }

		public string FileDialogFilter => "EL-9300 Program (*.g1p)|*.g1p";
		public string FileDialogFileName => ProgramName;

		public LinkTransfer Save() {
			return new ProgramVariable(ProgramMode, ProgramName, Array.ConvertAll(ProgramLines, l => Tokeniser.GetBytes(ProgramMode, l))).ToLinkTransfer();
		}

		public void Open(LinkTransfer transfer) {
			var programFile = new ProgramVariable(transfer);
			ProgramLines = Array.ConvertAll(programFile.Lines, l => Symbols.FromPlainText(Tokeniser.GetString(l)));
			ProgramMode = programFile.Mode;
			ProgramName = programFile.Name;
		}

		#endregion

		#region Editing

		private void ProgramCodeTextBox_TextChanged(object sender, EventArgs e) {
			Dirty = true;
		}

		private void ProgramNameTextBox_TextChanged(object sender, EventArgs e) {
			Dirty = true;
		}

		private void ProgramTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			Dirty = true;
		}

		#endregion


	}
}
