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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Interaction logic for ScriptControl.xaml
	/// </summary>
	[UserControlForController(typeof(IScriptViewEventSink))]
	public partial class ScriptControl : UserControl, IScriptView
	{
		private IScriptViewEventSink _controller;
		private Control _scriptView;

		public ScriptControl()
		{
			InitializeComponent();
		}

		public IScriptViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void AddPureScriptView(object scriptView)
		{
			if (object.Equals(_scriptView, scriptView))
			{
				return;
			}

			if (null != _scriptView)
				this._grid.Children.Remove(_scriptView);

			_scriptView = (Control)scriptView;
			if (null != _scriptView)
			{
				_scriptView.SetValue(Grid.RowProperty, 0);
				_grid.Children.Add(_scriptView);
				_scriptView.Focus();
			}
		}

		public void ClearCompilerErrors()
		{
			lbCompilerErrors.Items.Clear();
		}

		public void AddCompilerError(string s)
		{
			this.lbCompilerErrors.Items.Add(s);
		}

		private void EhCompilerErrorDoubleClick(object sender, MouseButtonEventArgs e)
		{
			string msg = lbCompilerErrors.SelectedItem as string;

			if (null != _controller && null != msg)
				_controller.EhView_GotoCompilerError(msg);
		}
	}
}