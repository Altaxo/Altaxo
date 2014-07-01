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

using Altaxo.Collections;
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
	/// Interaction logic for Form1.xaml
	/// </summary>
	public partial class OleDbDataQueryControl : UserControl, IOleDbDataQueryView
	{
		public event Action SelectedTabChanged;

		public event Action CmdChooseConnectionStringFromDialog;

		public event Action ConnectionStringSelectedFromList;

		public event Action<string> ConnectionStringChangedByUser;

		public OleDbDataQueryControl()
		{
			InitializeComponent();
		}

		private static IndexToImageConverter _treeImageConverter;

		public static IValueConverter TreeImageConverter
		{
			get
			{
				if (null == _treeImageConverter)
				{
					_treeImageConverter = new IndexToImageConverter(
							new string[]{
														"Icons.16x16.DataConnection.Table",
														"Icons.16x16.DataConnection.View",
														"Icons.16x16.DataConnection.Procedure",
														"Icons.16x16.DataConnection.Column",
													});
				}
				return _treeImageConverter;
			}
		}

		public void SetConnectionListSource(SelectableListNodeList list, string currentItem)
		{
			GuiHelper.Initialize(_cmbConnString, list);
			_cmbConnString.Text = currentItem;
		}

		private void EhConnectionStringSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cmbConnString);
			var ev = ConnectionStringSelectedFromList;
			if (null != ev)
			{
				ev();
			}
		}

		private void EhConnStringKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				var ev = ConnectionStringChangedByUser;
				if (null != ev)
				{
					e.Handled = true;
					ev(_cmbConnString.Text);
				}
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

		private void _tab_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_tab);

			var ev = SelectedTabChanged;
			if (null != ev)
				ev();
		}

		// pick a new connection
		private void _btnConnPicker_Click(object sender, RoutedEventArgs e)
		{
			var ev = CmdChooseConnectionStringFromDialog;
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

		public void SetTabItemsSource(SelectableListNodeList tabItems)
		{
			GuiHelper.Initialize(_tab, tabItems);
		}

		public void SetConnectionStatus(bool isValidConnectionSource)
		{
			_guiConnectionInvalid.Visibility = isValidConnectionSource ? Visibility.Collapsed : System.Windows.Visibility.Visible;
		}
	}
}