using System;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Summary description for IWorkbench.
	/// </summary>
	public interface IWorkbench
	{
		/// <summary>Gets the corresponding workbench GUI object, i.e for Windows the main windows form.</summary>
		object ViewObject { get; }

    /// <summary>Gets the active view content, i.e. in most cases it returns the controller that controls the content.</summary>
		object ActiveViewContent { get; }
		
    /// <summary>The view content collection.</summary>
    System.Collections.ICollection ViewContentCollection { get; }

    /// <summary>
    /// Shows the view content. The type of object content depends on the GUI type. SharpDevelop's GUI
    /// requires an object of type IViewContent; 
    /// </summary>
    /// <param name="content">The view content that should be shown.</param>
		void ShowView(object content);

    /// <summary>
    /// Closes the view content. The type of object content depends on the GUI type. SharpDevelop's GUI
    /// requires an object of type IViewContent; 
    /// </summary>
    /// <param name="content">The view content that should be shown.</param>
    void CloseContent(object content);

    /// <summary>
    /// Closes all views.
    /// </summary>
		void CloseAllViews();

    /// <summary>Fired if the current view (and so the view content) changed.</summary>
    event EventHandler ActiveViewContentChanged;
	}

}
