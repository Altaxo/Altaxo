// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Kr�ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The IWorkbenchWindow is the basic interface to a window which
	/// shows a view (represented by the IViewContent object).
	/// </summary>
	public interface IWorkbenchWindow
	{
		/// <summary>
		/// The window title.
		/// </summary>
		string Title 
		{
			get;
			set;
		}
		
		/// <summary>
		/// The current view content which is shown inside this window.
		/// </summary>
		IViewContent ViewContent 
		{
			get;
		}
		
		/// <summary>
		/// Closes the window, if force == true it closes the window
		/// without ask, even the content is dirty.
		/// </summary>
		void CloseWindow(bool force);
		
		/// <summary>
		/// Brings this window to front and sets the user focus to this
		/// window.
		/// </summary>
		void SelectWindow();
		
		//		void OnWindowSelected(EventArgs e);
		void OnWindowDeselected(EventArgs e);
		
		/// <summary>
		/// Is called when the window is selected.
		/// </summary>
		event EventHandler WindowSelected;
		
		/// <summary>
		/// Is called when the window is deselected.
		/// </summary>
		event EventHandler WindowDeselected;
		
		/// <summary>
		/// Is called when the title of this window has changed.
		/// </summary>
		event EventHandler TitleChanged;
		
		/// <summary>
		/// Is called after the window closes.
		/// </summary>
		event EventHandler CloseEvent;
	}
}
