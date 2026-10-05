using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp.EL9300 {
	public interface IEditorForm {

		void Open(LinkTransfer transfer);
		
		LinkTransfer Save();

		string FileName { get; set; }
		bool Dirty { get; set; }

		string FileDialogFilter { get; }
		string FileDialogFileName { get; }

	}
}
