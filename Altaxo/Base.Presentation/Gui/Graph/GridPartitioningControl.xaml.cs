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
	/// Interaction logic for GridPartitioningControl.xaml
	/// </summary>
	public partial class GridPartitioningControl : UserControl, IGridPartitioningView
	{
		public GridPartitioningControl()
		{
			InitializeComponent();
		}

		public QuantityWithUnitGuiEnvironment XPartitionEnvironment { set { _guiColumnDefinitions.Environment = value; } }

		public QuantityWithUnitGuiEnvironment YPartitionEnvironment { set { _guiRowDefinitions.Environment = value; } }

		public Units.DimensionfulQuantity DefaultXQuantity { set { _guiColumnDefinitions.DefaultQuantity = value; } }

		public Units.DimensionfulQuantity DefaultYQuantity { set { _guiRowDefinitions.DefaultQuantity = value; } }

		public System.Collections.ObjectModel.ObservableCollection<Units.DimensionfulQuantity> ColumnCollection
		{
			set
			{
				_guiColumnDefinitions.ItemsSource = value;
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Units.DimensionfulQuantity> RowCollection
		{
			set
			{
				_guiRowDefinitions.ItemsSource = value;
			}
		}
	}
}