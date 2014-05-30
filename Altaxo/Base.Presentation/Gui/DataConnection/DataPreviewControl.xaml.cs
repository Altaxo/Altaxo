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
	/// Interaction logic for DataPreviewControl.xaml
	/// </summary>
	public partial class DataPreviewControl : UserControl, IDataPreviewView
	{
		public DataPreviewControl()
		{
			InitializeComponent();
		}

		public void SetTableSource(System.Data.DataTable table)
		{
			_grid.ItemsSource = System.Data.DataTableExtensions.AsDataView(table);
		}
	}
}