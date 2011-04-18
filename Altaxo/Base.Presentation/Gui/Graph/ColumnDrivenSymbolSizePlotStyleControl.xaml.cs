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
	/// Interaction logic for ColumnDrivenColorPlotStyleControl.xaml
	/// </summary>
	public partial class ColumnDrivenSymbolSizePlotStyleControl : UserControl, IColumnDrivenSymbolSizePlotStyleView
	{
		public ColumnDrivenSymbolSizePlotStyleControl()
		{
			InitializeComponent();
		}

		private void _btSelectDataColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != ChooseDataColumn)
				ChooseDataColumn();
		}

		private void _btClearDataColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != ClearDataColumn)
				ClearDataColumn();
		}

		#region  IColumnDrivenSymbolSizePlotStyleView
	

		public IDensityScaleView ScaleView
		{
			get { return _ctrlScale; }
		}

		public event Action ChooseDataColumn;

		public event Action ClearDataColumn;

		public string DataColumnName
		{
			set { _edDataColumn.Text = value; }
		}

			public double SymbolSizeAt0
		{
			get
			{
				return _cbSymbolSizeAt0.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeAt0.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeAt1
		{
			get
			{
				return _cbSymbolSizeAt1.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeAt1.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeAbove
		{
			get
			{
				return _cbSymbolSizeAbove.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeAbove.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeBelow
		{
			get
			{
				return _cbSymbolSizeBelow.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeBelow.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeInvalid
		{
			get
			{
				return _cbSymbolSizeInvalid.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeInvalid.SelectedQuantityInPoints = value;
			}
		}

		public int NumberOfSteps
		{
			get
			{
				return (int)_edNumberOfSteps.Value;
			}
			set
			{
				_edNumberOfSteps.Value = value;
			}
		}

		#endregion
	}
}
