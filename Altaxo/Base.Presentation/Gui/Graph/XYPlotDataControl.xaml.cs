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

namespace Altaxo.Gui.Graph
{
	using Altaxo.Collections;

	/// <summary>
	/// Interaction logic for XYPlotDataControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYColumnPlotDataViewEventSink))]
	public partial class XYPlotDataControl : UserControl, IXYColumnPlotDataView
	{
		IXYColumnPlotDataViewEventSink _controller;

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
