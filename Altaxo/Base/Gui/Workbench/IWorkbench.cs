#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Summary description for IWorkbench.
	/// </summary>
	public interface IWorkbench : System.ComponentModel.INotifyPropertyChanged
	{
		/// <summary>Gets the corresponding workbench GUI object, i.e for Windows the main windows.</summary>
		object ViewObject { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the workbench is shown full screen, or not.
		/// </summary>
		/// <value>
		///   <c>true</c> if the workbench is shown full screen; otherwise, <c>false</c>.
		/// </value>
		bool FullScreen { get; set; }

		/// <summary>
		/// A collection in which all opened view contents (including all secondary view contents) are saved.
		/// </summary>
		IList<IViewContent> ViewContentCollection { get; }

		/// <summary>
		/// Closes the workbench by closing the main window of the workbench.
		/// </summary>
		void Close();

		/// <summary>
		/// A collection in which all content which is shown as pads (not as documents).
		/// </summary>
		IList<IPadContent> PadContentCollection
		{
			get;
		}

		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		IViewContent ActiveViewContent { get; }

		/// <summary>
		/// Occurs when the active view content changed. When active view content changed, first, the PropertyChanged event
		/// will be raised. Only after that, the event here will be raised.
		/// </summary>
		event EventHandler ActiveViewContentChanged;

		/// <summary>
		/// The active content, depending on where the focus currently is.
		/// If a document is currently active, this will be equal to ActiveViewContent,
		/// if a pad has the focus, this property will return the IPadContent instance.
		/// </summary>
		IWorkbenchContent ActiveContent
		{
			get;
		}

		/// <summary>
		/// Occurs when the active view content changed. When active view content changed, first, the corresponding PropertyChanged event
		/// will be raised. Only after that, the event here will be raised.
		/// </summary>
		event EventHandler ActiveContentChanged;

		/// <summary>
		/// Shows the view content. The type of object content depends on the GUI type. SharpDevelop's GUI
		/// requires an object of type IViewContent;
		/// </summary>
		/// <param name="content">The view content that should be shown.</param>
		void ShowView(object content);

		/// <summary>
		/// Shows the provided pad content in the pad area.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="switchToPad">If true, the pad is made the active content, i.e. it is selected.</param>
		void ShowPad(IPadContent content, bool switchToPad);

		/// <summary>
		/// Closes the workbench view content.
		/// </summary>
		/// <param name="content">The view content that should be closed.</param>
		void CloseContent(IViewContent content);

		/// <summary>
		/// Gets the content for the provided document.
		/// </summary>
		/// <typeparam name="T">The type of controller to search for. The argument <see cref="IMVCController"/> will find any controller that uses the provided document.
		/// If you provide a more specific type, only such a controller that implements this type and has the provided document will be returned.
		/// </typeparam>
		/// <param name="document">The document to search for.</param>
		/// <returns>The content (either a <see cref="IPadContent"/> or a <see cref="IViewContent"/> whose <see cref="IMVCController.ModelObject"/> is the provided document.</returns>
		T GetViewModel<T>(object document) where T : IMVCController;

		/// <summary>
		/// Gets the content for the provided document.
		/// </summary>
		/// <typeparam name="T">The type of controller to search for. The argument <see cref="IMVCController"/> will find any controller that uses the provided document.
		/// If you provide a more specific type, only such a controller that implements this type and has the provided document will be returned.
		/// </typeparam>
		/// <param name="document">The document to search for.</param>
		/// <returns>Enumeration of content (either a <see cref="IPadContent"/> or a <see cref="IViewContent"/> whose <see cref="IMVCController.ModelObject"/> is the provided document.</returns>
		IEnumerable<T> GetViewModels<T>(object document) where T : IMVCController;

		/// <summary>
		/// Closes all views.
		/// </summary>
		void CloseAllViews();

		/// <summary>
		/// Informs the workbench that the project instance has changed.
		/// </summary>
		/// <param name="sender">Sender of this message.</param>
		/// <param name="e">Information about the project change.</param>
		void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e);

		void SaveCompleteWorkbenchStateAndLayoutInPropertyService();
	}
}