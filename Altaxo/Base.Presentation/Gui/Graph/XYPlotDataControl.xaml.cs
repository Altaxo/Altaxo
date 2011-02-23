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
			if (null != Controller)
				Controller.EhView_TableSelectionChanged(this.m_cbTables.SelectedIndex, (string)this.m_cbTables.SelectedItem);

		}

		private void EhToX_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_ToX(this.m_cbTables.SelectedIndex, (string)this.m_cbTables.SelectedItem, m_lbColumns.SelectedIndex, (string)this.m_lbColumns.SelectedItem);

		}

		private void EhEraseX_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_EraseX();

		}

		private void EhToY_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_ToY(this.m_cbTables.SelectedIndex, (string)this.m_cbTables.SelectedItem, m_lbColumns.SelectedIndex, (string)this.m_lbColumns.SelectedItem);

		}

		private void EhEraseY_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_EraseY();

		}

		private void EhPlotRangeFrom_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != Controller)
				Controller.EhView_RangeFrom(this.m_nudPlotRangeFrom.Value);

		}

		private void EhPlotRangeTo_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != Controller)
				Controller.EhView_RangeTo(this.m_nudPlotRangeTo.Value);

		}

		#region IXYColumnPlotDataView

		public IXYColumnPlotDataViewEventSink Controller
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

		public void Tables_Initialize(string[] tables, int selectedTable)
		{
			this.m_cbTables.Items.Clear();
			this.m_cbTables.ItemsSource = tables;
			this.m_cbTables.SelectedIndex = selectedTable;

		}

		public void Columns_Initialize(string[] colnames, int selectedColumn)
		{
			this.m_lbColumns.Items.Clear();
			this.m_lbColumns.ItemsSource=colnames;
			if (selectedColumn < colnames.Length)
				this.m_lbColumns.SelectedIndex = selectedColumn;
		}

		public void XColumn_Initialize(string colname)
		{
			this.m_edXColumn.Text = colname;
		}

		public void YColumn_Initialize(string colname)
		{
			this.m_edYColumn.Text = colname;
		}

		public void PlotRangeFrom_Initialize(int from)
		{
			this.m_nudPlotRangeFrom.Minimum = 0;
			this.m_nudPlotRangeFrom.Maximum = int.MaxValue;
			this.m_nudPlotRangeFrom.Value = from;
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
