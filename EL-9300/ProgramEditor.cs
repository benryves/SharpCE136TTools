using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class ProgramEditor : Form, IEditorForm {

		public ProgramEditor() {
			InitializeComponent();
			foreach (ProgramMode mode in Enum.GetValues(typeof(ProgramMode))) {
				programTypeComboBox.Items.Add(mode);
			}
			ProgramMode = ProgramMode.Real;
			Dirty = false;
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

		#region Validation

		private void ProgramNameTextBox_Validating(object sender, CancelEventArgs e) {
			if (programNameTextBox.Text.Length < 1) {
				MessageBox.Show(this, "Please enter a program name.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
			} else if (programNameTextBox.Text.Length > 16) {
				MessageBox.Show(this, "The program name is too long.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
			} else if (Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(programNameTextBox.Text)) != programNameTextBox.Text) {
				MessageBox.Show(this, "The program name contains invalid characters.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
			}
			if (e.Cancel) {
				programNameTextBox.SelectAll();
				programNameTextBox.Focus();
			}
		}

		private void ProgramCodeTextBox_Validating(object sender, CancelEventArgs e) {
			for (int i = 0; i < programCodeTextBox.Lines.Length; ++i) {
				try {
					Tokeniser.GetBytes(ProgramMode, programCodeTextBox.Lines[i]);
				} catch (Exception ex) {
					programCodeTextBox.Select(programCodeTextBox.GetFirstCharIndexFromLine(i), programCodeTextBox.Lines[i].TrimEnd().Length);
					programCodeTextBox.ScrollToCaret();
					programNameTextBox.Focus();
					MessageBox.Show(this, string.Format("Line {0}: {1}", i + 1, ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					e.Cancel = true;
					return;
				}
			}
		}

		#endregion


	}
}
