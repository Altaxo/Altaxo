using System;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Summary description for IWorkbench.
	/// </summary>
	public interface IWorkbench
	{
		/// <summary>Gets the corresponding GUI element, i.e for Windows the main windows form.</summary>
		object ViewObject { get; }

		object ActiveViewContent { get; }
		System.Collections.ICollection ViewContentCollection { get; }

		void ShowView(object o);
    void CloseContent(object o);
		void CloseAllViews();
	}

}
