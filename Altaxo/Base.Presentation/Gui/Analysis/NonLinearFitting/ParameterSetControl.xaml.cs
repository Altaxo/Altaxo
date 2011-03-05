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

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	/// <summary>
	/// Interaction logic for ParameterSetControl.xaml
	/// </summary>
	public partial class ParameterSetControl : UserControl, IParameterSetView
	{
		public ParameterSetControl()
		{
			InitializeComponent();
		}


		#region IParameterSetView

		List<ParameterSetViewItem> _itemsSource;
		public void Initialize(List<ParameterSetViewItem> list)
		{
			_itemsSource = list;
			_dataGrid.ItemsSource = _itemsSource;
		}

		public List<ParameterSetViewItem> GetList()
		{
			return _itemsSource;
		}

		#endregion
	}


	
}
