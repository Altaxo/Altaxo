#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion
using System;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;


namespace Altaxo.Gui.Pads
{
	/// <summary>
	/// Responsible for showing the notes of worksheets and graph windows.
	/// </summary>
	public class NotesController :
		ICSharpCode.SharpDevelop.Gui.IPadContent
	{
		System.Windows.Controls.TextBox _view;

		/// <summary>The currently active view content to which the text belongs.</summary>
		object _currentActiveViewContent;

		public NotesController()
		{
			_view = new System.Windows.Controls.TextBox();

			_view.TextWrapping = System.Windows.TextWrapping.NoWrap;
			_view.AcceptsReturn = true;
			_view.AcceptsTab = true;
			_view.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			_view.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			_view.FontFamily = new System.Windows.Media.FontFamily("Global Monospace");

			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += EhActiveWorkbenchWindowChanged;
			_view.LostFocus += new System.Windows.RoutedEventHandler(_view_LostFocus);
			_view.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(_view_LostKeyboardFocus);
			_view.IsEnabled = false;
		}

		void _view_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			StoreCurrentText();
		}

		void _view_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			StoreCurrentText();
		}

		void EhActiveWorkbenchWindowChanged(object sender, EventArgs e)
		{
			StoreCurrentText(); // Saves the old text

			_currentActiveViewContent = Current.Workbench.ActiveViewContent;

			bool enable = true;
			if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)
			{
				_view.Text = ((Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)_currentActiveViewContent).Controller.DataTable.Notes;
			}
			else if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDGraphViewContent)
			{
				_view.Text = ((Altaxo.Gui.SharpDevelop.SDGraphViewContent)_currentActiveViewContent).Controller.Doc.Notes;
			}
			else
			{
				_view.Text = string.Empty;
				enable = false;
			}

			_view.IsEnabled = enable;

			if (enable && _view.Text.Length > 0)
			{
				ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow ww = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

				var pad = WorkbenchSingleton.Workbench.GetPad(this.GetType());
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(pad);

				// now focus back to the formerly active workbench window.
				ww.SelectWindow();
			}
		}



		/// <summary>
		/// Stores the text in the text control back to the graph document or worksheet.
		/// </summary>
		void StoreCurrentText()
		{
			if (null != _view)
			{
				if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)
					((Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)_currentActiveViewContent).Controller.DataTable.Notes = _view.Text;
				else if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDGraphViewContent)
					((Altaxo.Gui.SharpDevelop.SDGraphViewContent)_currentActiveViewContent).Controller.Doc.Notes = _view.Text;
			}
		}

		#region IPadContent Members

		public object Control
		{
			get
			{
				return this._view;
			}
		}

		public object InitiallyFocusedControl
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_view != null)
			{
				_view = null;
			}
		}

		#endregion

	}
}
