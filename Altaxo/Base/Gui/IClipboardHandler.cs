using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui
{
	/// <summary>
	/// This interface is meant for controllers that want to handle clipboard events.
	/// WPF AddIns should handle the routed commands 'Copy', 'Cut', 'Paste', 'Delete' and 'SelectAll' instead.
	/// </summary>
	public interface IClipboardHandler
	{
		bool EnableCut
		{
			get;
		}

		bool EnableCopy
		{
			get;
		}

		bool EnablePaste
		{
			get;
		}

		bool EnableDelete
		{
			get;
		}

		bool EnableSelectAll
		{
			get;
		}

		void Cut();

		void Copy();

		void Paste();

		void Delete();

		void SelectAll();
	}
}