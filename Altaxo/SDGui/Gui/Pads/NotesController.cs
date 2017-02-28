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

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using System;

namespace Altaxo.Gui.Pads
{
	/// <summary>
	/// Responsible for showing the notes of worksheets and graph windows.
	/// </summary>
	public class NotesController :
		IPadContent
	{
		private System.Windows.Controls.TextBox _view;

		/// <summary>The currently active view content to which the text belongs.</summary>
		private WeakReference _currentActiveViewContent = new WeakReference(null);

		private System.Windows.Data.BindingExpressionBase _textBinding;

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
			_view.TextChanged += new System.Windows.Controls.TextChangedEventHandler(_view_TextChanged);
			_view.IsEnabled = false;
		}

		private void _view_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			var tb = _textBinding;
			if (null != tb)
				tb.UpdateSource();
		}

		private void _view_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			SaveTextBoxTextToNotes();
		}

		private void _view_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			SaveTextBoxTextToNotes();
		}

		private void EhActiveWorkbenchWindowChanged(object sender, EventArgs e)
		{
			SaveTextBoxTextToNotes(); // Saves the old text

			if (null == _view)
				return; // can happen during shutdown

			// Clears the old binding
			_textBinding = null; // to avoid updates when the text changed in the next line, and then the TextChanged event of the TextBox is triggered
			System.Windows.Data.BindingOperations.ClearBinding(_view, System.Windows.Controls.TextBox.TextProperty);

			_currentActiveViewContent = new WeakReference(Current.Workbench.ActiveViewContent);

			bool enable = true;

			var gvc = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
			var wvc = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDWorksheetViewContent;
			if (null != wvc && null != wvc.Controller)
			{
				GetTextFromNotesAndSetBinding(wvc.Controller.DataTable.Notes);
			}
			else if (null != gvc && null != gvc.Controller)
			{
				GetTextFromNotesAndSetBinding(gvc.Controller.Doc.Notes);
			}
			else
			{
				_view.Text = string.Empty;
				enable = false;
			}

			_view.IsEnabled = enable;

			if (enable && _view.Text.Length > 0)
			{
				var ww = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

				var pad = WorkbenchSingleton.Workbench.GetPad(this.GetType());
				WorkbenchSingleton.Workbench.ActivatePad(pad);

				// now focus back to the formerly active workbench window.
				ww.SelectWindow();
			}
		}

		private void GetTextFromNotesAndSetBinding(Altaxo.Main.ITextBackedConsole con)
		{
			_view.Text = con.Text;

			var binding = new System.Windows.Data.Binding();
			binding.Source = con;
			binding.Path = new System.Windows.PropertyPath("Text");
			binding.Mode = System.Windows.Data.BindingMode.TwoWay;
			_textBinding = _view.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
		}

		/// <summary>
		/// Stores the text in the text control back to the graph document or worksheet.
		/// </summary>
		private void SaveTextBoxTextToNotes()
		{
			if (null != _view)
			{
				var wvc = _currentActiveViewContent.Target as Altaxo.Gui.SharpDevelop.SDWorksheetViewContent;
				var gvc = _currentActiveViewContent.Target as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
				if (null != wvc && null != wvc.Controller)
					wvc.Controller.DataTable.Notes.Text = _view.Text;
				else if (null != gvc && null != gvc.Controller)
					gvc.Controller.Doc.Notes.Text = _view.Text;
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

		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		#endregion IPadContent Members

		#region IDisposable Members

		public void Dispose()
		{
			if (_view != null)
			{
				_view = null;
			}
		}

		#endregion IDisposable Members
	}
}