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
		WeakReference _currentActiveViewContent = new WeakReference(null);

		System.Windows.Data.BindingExpressionBase _textBinding;

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

		void _view_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			var tb = _textBinding;
			if (null != tb)
				tb.UpdateSource();
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

			_currentActiveViewContent = new WeakReference(Current.Workbench.ActiveViewContent);

			bool enable = true;

			var gvc = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
			var wvc = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDWorksheetViewContent;
			if (null != wvc && null != wvc.Controller)
			{
				UpdateTextAndBinding(wvc.Controller.DataTable.Notes);
			}
			else if (null != gvc && null != gvc.Controller)
			{
				UpdateTextAndBinding(gvc.Controller.Doc.Notes);
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

		private void UpdateTextAndBinding(Altaxo.Main.ITextBackedConsole con)
		{
			_textBinding = null; // to avoid updates when the text changed in the next line, and then the TextChanged event of the TextBox is triggered
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
		void StoreCurrentText()
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
