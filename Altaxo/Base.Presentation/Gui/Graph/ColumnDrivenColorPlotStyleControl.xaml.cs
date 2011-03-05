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
	public partial class ColumnDrivenColorPlotStyleControl : UserControl, IColumnDrivenColorPlotStyleView
	{
		public ColumnDrivenColorPlotStyleControl()
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

		#region IColumnDrivenColorPlotStyleView

		public IDensityScaleView ScaleView
		{
			get { return _ctrlScale; }
		}

		public IColorProviderView ColorProviderView
		{
			get { return _colorProviderControl; }
		}

		public event Action ChooseDataColumn;

		public event Action ClearDataColumn;

		public string DataColumnName
		{
			set { _edDataColumn.Text = value; }
		}

		#endregion
	}
}
