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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Plot.Data
{
	using Altaxo.Collections;

	/// <summary>
	/// Interaction logic for XYPlotDataControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYColumnPlotDataViewEventSink))]
	public partial class XYPlotDataControl : UserControl, IXYColumnPlotDataView
	{
		private IXYColumnPlotDataViewEventSink _controller;

		public XYPlotDataControl()
		{
			InitializeComponent();
		}

		private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != _controller)
			{
				GuiHelper.SynchronizeSelectionFromGui(this._cbTables);
				_controller.EhView_TableSelectionChanged();
			}
		}

		private void EhToX_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				GuiHelper.SynchronizeSelectionFromGui(_lbColumns);
				_controller.EhView_ToX();
			}
		}

		private void EhEraseX_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhView_EraseX();
		}

		private void EhToY_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				GuiHelper.SynchronizeSelectionFromGui(_lbColumns);
				_controller.EhView_ToY();
			}
		}

		private void EhEraseY_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhView_EraseY();
		}

		private void EhPlotRangeFrom_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != _controller)
				_controller.EhView_RangeFrom(this._nudPlotRangeFrom.Value);
		}

		private void EhPlotRangeTo_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != _controller)
				_controller.EhView_RangeTo(this.m_nudPlotRangeTo.Value);
		}

		#region IXYColumnPlotDataView

		public IXYColumnPlotDataViewEventSink Controller
		{
			set
			{
				_controller = value;
			}
		}

		public void Tables_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_cbTables, items);
		}

		public void Columns_Initialize(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_lbColumns, items);
		}

		public void XColumn_Initialize(string colname)
		{
			this._edXColumn.Text = colname;
		}

		public void YColumn_Initialize(string colname)
		{
			this._edYColumn.Text = colname;
		}

		public void PlotRangeFrom_Initialize(int from)
		{
			this._nudPlotRangeFrom.Minimum = 0;
			this._nudPlotRangeFrom.Maximum = int.MaxValue;
			this._nudPlotRangeFrom.Value = from;
		}

		public void PlotRangeTo_Initialize(int to)
		{
			this.m_nudPlotRangeTo.Minimum = 0;
			this.m_nudPlotRangeTo.Maximum = int.MaxValue;
			this.m_nudPlotRangeTo.Value = Math.Max(0, to);
		}

		#endregion IXYColumnPlotDataView
	}
}