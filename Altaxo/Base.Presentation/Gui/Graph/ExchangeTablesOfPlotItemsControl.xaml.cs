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
	/// Interaction logic for ExchangeTablesOfPlotItemsControl.xaml
	/// </summary>
	public partial class ExchangeTablesOfPlotItemsControl : UserControl, IExchangeTablesOfPlotItemsView
	{
		public event Action ChooseTableForSelectedItems;
		public event Action ChooseFolderForSelectedItems;



		public ExchangeTablesOfPlotItemsControl()
		{
			InitializeComponent();
		}

		public void InitializeExchangeTableList(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiTableList, list);
		}

		private void EhTableList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void EhChooseTable(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiTableList);
			if(null!=ChooseTableForSelectedItems)
				ChooseTableForSelectedItems();
		}

		private void EhChooseFolder(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiTableList);
			if (null != ChooseFolderForSelectedItems)
				ChooseFolderForSelectedItems();
		}
	}
}
