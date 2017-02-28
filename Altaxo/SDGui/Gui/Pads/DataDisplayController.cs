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
	/// Controls the data display window pad that shows the data obtained from the data reader.
	/// </summary>
	public class DataDisplayController :
		IPadContent,
		Altaxo.Main.Services.IDataDisplayService
	{
		private System.Windows.Controls.TextBox _view;

		public DataDisplayController()
		{
			_view = new System.Windows.Controls.TextBox();
			_view.TextWrapping = System.Windows.TextWrapping.NoWrap;
			_view.AcceptsReturn = true;
			_view.AcceptsTab = true;
			_view.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			_view.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			_view.FontFamily = new System.Windows.Media.FontFamily("Global Monospace");

			Current.SetDataDisplayService(this);
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

		private void InternalWrite(string text)
		{
			var activeWin = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

			_view.Text = text;
			var pad = WorkbenchSingleton.Workbench.GetPad(this.GetType());
			pad.BringPadToFront();

			activeWin.SelectWindow();
		}

		#region IDataDisplayService Members

		/// <summary>Writes a string to the output.</summary>
		/// <param name="text">The text to write to the output.</param>
		public void WriteOneLine(string text)
		{
			InternalWrite(text);
		}

		/// <summary>
		/// Writes two lines to the window.
		/// </summary>
		/// <param name="line1">First line.</param>
		/// <param name="line2">Second line.</param>
		public void WriteTwoLines(string line1, string line2)
		{
			InternalWrite(line1 + System.Environment.NewLine + line2);
		}

		/// <summary>
		/// Writes three lines to the output.
		/// </summary>
		/// <param name="line1">First line.</param>
		/// <param name="line2">Second line.</param>
		/// <param name="line3">Three line.</param>
		public void WriteThreeLines(string line1, string line2, string line3)
		{
			InternalWrite(line1 + System.Environment.NewLine + line2 + System.Environment.NewLine + line3);
		}

		#endregion IDataDisplayService Members
	}
}