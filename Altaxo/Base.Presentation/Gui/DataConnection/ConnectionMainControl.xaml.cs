using Altaxo.Collections;
using Altaxo.DataConnection;
using System;

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
	/// Interaction logic for Form1.xaml
	/// </summary>
	public partial class ConnectionMainControl : UserControl, IConnectionMainView
	{
		public event Action SelectedTabChanged;

		public event Action ShowSqlBuilder;

		public event Action PreviewTableData;

		public event Action ChooseConnection;

		public ConnectionMainControl()
		{
			InitializeComponent();
		}

		public string SqlText
		{
			get
			{
				return _txtSql.Text;
			}
			set
			{
				_txtSql.Text = value;
			}
		}

		public void UpdateUI(bool enableSqlBuilder, bool enablePreviewData)
		{
			// enable sql builder button if we have some tables
			_btnSqlBuilder.IsEnabled = enableSqlBuilder;

			// enable data preview if we a select statement
			_btnPreviewData.IsEnabled = enablePreviewData;
		}

		public void SetTreeSource(NGTreeNode rootNode)
		{
			_treeTables.ItemsSource = rootNode.Nodes;
		}

		public NGTreeNode SelectedTreeItem
		{
			get
			{
				return _treeTables.SelectedItem as NGTreeNode;
			}
		}

		public void ShowTableTabItem()
		{
			_tab.SelectedIndex = 0;
		}

		public void ShowSqlTextTabItem()
		{
			_tab.SelectedIndex = 1;
		}

		public bool IsTableTabItemSelected
		{
			get { return 0 == _tab.SelectedIndex; }
		}

		private void _tab_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			var ev = SelectedTabChanged;
			if (null != ev)
				ev();
		}

		private void EhTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var ev = SelectedTabChanged;
			if (null != ev)
				ev();
		}

		private void _btnSqlBuilder_Click(object sender, RoutedEventArgs e)
		{
			var ev = ShowSqlBuilder;
			if (null != ev)
				ev();
		}

		private void _treeTables_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			var ev = PreviewTableData;
			if (null != ev)
				ev();
		}

		// pick a new connection
		private void _btnConnPicker_Click(object sender, RoutedEventArgs e)
		{
			var ev = ChooseConnection;
			if (null != ev)
				ev();
		}

		public void SetWaitCursor()
		{
			this.Cursor = Cursors.Wait;
		}

		public void SetNormalCursor()
		{
			this.Cursor = Cursors.Arrow;
		}
	}
}