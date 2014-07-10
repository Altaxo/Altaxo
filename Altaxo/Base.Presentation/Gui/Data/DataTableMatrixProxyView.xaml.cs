#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Data
{
	using Altaxo.Collections;

	/// <summary>
	/// Interaction logic for XYPlotDataControl.xaml
	/// </summary>
	public partial class DataTableMatrixProxyView : UserControl, IDataTableMatrixProxyView
	{
		public event Action SelectedTableChanged;

		public event Action SelectedColumnKindChanged;

		public event Action UseSelectedItemAsXColumn;

		public event Action UseSelectedItemAsYColumn;

		public event Action UseSelectedItemAsVColumns;

		public event Action SelectedGroupNumberChanged;

		public event Action UseAllAvailableDataColumnsChanged;

		public event Action UseAllAvailableDataRowsChanged;

		public event Action ClearXColumn;

		public event Action ClearYColumn;

		public event Action ClearVColumns;

		public DataTableMatrixProxyView()
		{
			InitializeComponent();
		}

		private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(this._guiAvailableTables);
			if (null != SelectedTableChanged)
				SelectedTableChanged();
		}

		private void EhToX_Click(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAvailableColumnNames);
			if (null != UseSelectedItemAsXColumn)
				UseSelectedItemAsXColumn();
		}

		private void EhEraseX_Click(object sender, RoutedEventArgs e)
		{
			if (null != ClearXColumn)
				ClearXColumn();
		}

		private void EhToY_Click(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAvailableColumnNames);
			if (null != UseSelectedItemAsYColumn)
				UseSelectedItemAsYColumn();
		}

		private void EhEraseY_Click(object sender, RoutedEventArgs e)
		{
			if (null != ClearYColumn)
				ClearYColumn();
		}

		private void EhToV_Click(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAvailableColumnNames);
			if (null != UseSelectedItemAsVColumns)
				UseSelectedItemAsVColumns();
		}

		private void EhEraseV_Click(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiVColumnNames);
			if (null != ClearVColumns)
				ClearVColumns();
		}

		private void EhDataColumnsSelected(object sender, RoutedEventArgs e)
		{
			if (null != SelectedColumnKindChanged)
				SelectedColumnKindChanged();
		}

		private void EhPropertyColumnsSelected(object sender, RoutedEventArgs e)
		{
			if (null != SelectedColumnKindChanged)
				SelectedColumnKindChanged();
		}

		public void InitializeAvailableTables(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiAvailableTables, items);
		}

		public void InitializeAvailableColumns(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiAvailableColumnNames, items);
		}

		public void Initialize_XColumn(string colname)
		{
			this._guiXColumnName.Text = colname;
		}

		public void Initialize_YColumn(string colname)
		{
			this._guiYColumName.Text = colname;
		}

		public void Initialize_PlotRangeFrom(int from)
		{
			this._guiPlotRangeFrom.Minimum = 0;
			this._guiPlotRangeFrom.Maximum = int.MaxValue;
			this._guiPlotRangeFrom.Value = from;
		}

		public void Initialize_PlotRangeTo(int to)
		{
			this._guiPlotRangeTo.Minimum = 0;
			this._guiPlotRangeTo.Maximum = int.MaxValue;
			this._guiPlotRangeTo.Value = Math.Max(0, to);
		}

		public bool AreDataColumnsShown
		{
			get { return true == _guiDataColumnsSelection.IsChecked; }
		}

		public void Initialize_VColumns(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiVColumnNames, items);
		}

		public void EnableUseButtons(bool enableUseAsXColumn, bool enableUseAsYColumn, bool enableUseAsVColumns)
		{
			_guiTakeAsXColumn.IsEnabled = enableUseAsXColumn;
			_guiTakeAsYColumn.IsEnabled = enableUseAsYColumn;
			_guiTakeAsVColumns.IsEnabled = enableUseAsVColumns;
		}

		private void EhUseAllAvailableColumnsOfGroupChanged(object sender, RoutedEventArgs e)
		{
			var ev = UseAllAvailableDataColumnsChanged;
			if (null != ev)
				ev();
		}

		private void EhUseAllAvailableDataRowsChanged(object sender, RoutedEventArgs e)
		{
			var ev = UseAllAvailableDataRowsChanged;
			if (null != ev)
				ev();
		}

		private void EhGroupNumberChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var ev = SelectedGroupNumberChanged;
			if (null != ev)
				ev();
		}

		public int GroupNumber
		{
			get
			{
				return _guiGroupNumber.Value;
			}
			set
			{
				_guiGroupNumber.Value = value;
			}
		}

		public bool UseAllAvailableDataColumns
		{
			get
			{
				return _guiUseAllAvailableColumnsOfGroup.IsChecked == true;
			}
			set
			{
				_guiUseAllAvailableColumnsOfGroup.IsChecked = value;
			}
		}

		public bool UseAllAvailableDataRows
		{
			get
			{
				return _guiUseAllAvailableDataRows.IsChecked == true;
			}
			set
			{
				_guiUseAllAvailableDataRows.IsChecked = value;
			}
		}
	}
}