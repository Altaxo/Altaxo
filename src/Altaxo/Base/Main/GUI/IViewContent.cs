// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// IViewContent is the base interface for all editable data
	/// inside SharpDevelop.
	/// </summary>
	public interface IViewContent : IDisposable
	{
#if !LINUX
		/// <summary>
		/// This is the Windows.Forms control for the view.
		/// </summary>
		Control Control 
		{
			get;
		}
#endif
		/// <summary>
		/// The workbench window in which this view is displayed.
		/// </summary>
		IWorkbenchWindow  WorkbenchWindow 
		{
			get;
			set;
		}
		
		/// <summary>
		/// A generic name for the file, when it does have no file name
		/// (e.g. newly created files)
		/// </summary>
		string UntitledName 
		{
			get;
			set;
		}
		
		/// <summary>
		/// This is the whole name of the content, e.g. the file name or
		/// the url depending on the type of the content.
		/// </summary>
		string ContentName 
		{
			get;
			set;
		}
		
		/// <summary>
		/// If this property returns true the view is untitled.
		/// </summary>
		bool IsUntitled 
		{
			get;
		}
		
		/// <summary>
		/// If this property returns true the content has changed since
		/// the last load/save operation.
		/// </summary>
		bool IsDirty 
		{
			get;
			set;
		}
		
		/// <summary>
		/// If this property returns true the content could not be altered.
		/// </summary>
		bool IsReadOnly 
		{
			get;
		}
		
		/// <summary>
		/// If this property returns true the content can't be written.
		/// </summary>
		bool IsViewOnly 
		{
			get;
		}
		
		/// <summary>
		/// Reinitializes the content. (Re-initializes all add-in tree stuff)
		/// and redraws the content. Call this not directly unless you know
		/// what you do.
		/// </summary>
		void RedrawContent();
		
		/// <summary>
		/// Saves this content to the last load/save location.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Saves the content to the location <code>fileName</code>
		/// </summary>
		void Save(string fileName);
		
		/// <summary>
		/// Loads the content from the location <code>fileName</code>
		/// </summary>
		void Load(string fileName);
		
		/// <summary>
		/// Is called each time the name for the content has changed.
		/// </summary>
		event EventHandler ContentNameChanged;
		
		/// <summary>
		/// Is called when the content is changed after a save/load operation
		/// and this signals that changes could be saved.
		/// </summary>
		event EventHandler DirtyChanged;
	}
}
