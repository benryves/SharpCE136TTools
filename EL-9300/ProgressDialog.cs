using System.Drawing;
using System.Windows.Forms;

namespace Sharp.EL9300 {
	public partial class ProgressDialog : Form {

		public int Minimum {
			get => progressBar.Minimum;
			set => progressBar.Minimum = value;
		}

		public int Maximum {
			get => progressBar.Maximum;
			set => progressBar.Maximum = value;
		}

		public int Value {
			get => progressBar.Value;
			set {
				if (progressBar.Value == value) return;
				if (value < progressBar.Maximum) {
					progressBar.Value = value + 1;
					--progressBar.Value;
				} else {
					++progressBar.Maximum;
					progressBar.Value = value + 1;
					--progressBar.Value;
					--progressBar.Maximum;
				}
			}
		}

		public ProgressBarStyle Style {
			get => progressBar.Style;
			set => progressBar.Style = value;
		}

		public bool CanCancel {
			get => cancelButton.Enabled;
			set => cancelButton.Enabled = value;
		}

		public ProgressDialog() {
			InitializeComponent();
			this.ClientSize = new Size(300, 64);
		}

	}
}
