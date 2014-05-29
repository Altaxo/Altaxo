#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.DataConnection
{
	/// <summary>
	/// Interaction logic for QueryPropertiesControl.xaml
	/// </summary>
	public partial class QueryPropertiesControl : UserControl
	{
		private QueryBuilder _builder;

		public QueryPropertiesControl()
		{
			InitializeComponent();
		}

		public QueryBuilder QueryBuilder
		{
			get { return _builder; }
			set
			{
				if (_builder != value)
				{
					_builder = value;
					UpdateDialogValues();
				}
			}
		}

		private void _btnOK_Click(object sender, EventArgs e)
		{
			UpdateBuilderValues();
		}

		// copy QueryBuilder values to form
		private void UpdateDialogValues()
		{
			_numTopN.Text = _builder.Top.ToString();
			if (_builder.GroupBy)
			{
				_cmbGroupBy.SelectedIndex = (int)_builder.GroupByExtension;
			}
			else
			{
				_cmbGroupBy.IsEnabled = false;
			}
			_chkDistinct.IsChecked = _builder.Distinct;
		}

		// copy form values to QueryBuilder
		private void UpdateBuilderValues()
		{
			_builder.Top = int.Parse(_numTopN.Text);
			if (_builder.GroupBy)
			{
				_builder.GroupByExtension = (GroupByExtension)_cmbGroupBy.SelectedIndex;
			}
			_builder.Distinct = true == _chkDistinct.IsChecked;
		}
	}
}