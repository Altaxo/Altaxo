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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for SingleChoiceComboBoxControl.xaml
	/// </summary>
	[UserControlForController(typeof(ISingleChoiceViewEventSink))]
	public partial class SingleChoiceComboBoxControl : UserControl, ISingleChoiceView
	{
		public SingleChoiceComboBoxControl()
		{
			InitializeComponent();
		}

		private void EhSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (null != _controller)
				_controller.EhChoiceChanged(_comboBox.SelectedIndex);
		}

		#region ISingleChoiceView

		private ISingleChoiceViewEventSink _controller;

		public ISingleChoiceViewEventSink Controller
		{
			set { _controller = value; }
		}

		public void InitializeDescription(string value)
		{
			_label.Content = value;
		}

		public void InitializeChoice(string[] values, int initialchoice)
		{
			_comboBox.ItemsSource = values;
			_comboBox.SelectedIndex = initialchoice;
		}

		#endregion ISingleChoiceView
	}
}