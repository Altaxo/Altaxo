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
	/// Interaction logic for PlotGroupCollectionControl.xaml
	/// </summary>
	public partial class PlotGroupCollectionControl : UserControl, IPlotGroupCollectionView
	{
		public PlotGroupCollectionControl()
		{
			InitializeComponent();
		}

		private void EhGotoAdvanced(object sender, RoutedEventArgs e)
		{
			if (null != GotoAdvanced)
				GotoAdvanced();
		}

		private void EhGotoSimple(object sender, RoutedEventArgs e)
		{
			if (null != GotoSimple)
				GotoSimple();
		}

		#region IPlotGroupCollectionView

		public event Action GotoAdvanced;
		public event Action GotoSimple;


		public void SetSimpleView(object viewObject)
		{
			_controlHost.Child = null;
			_controlHost.Child = (UIElement)viewObject;
			_btGotoSimple.Visibility = System.Windows.Visibility.Collapsed;
			_btGotoAdvanced.Visibility = System.Windows.Visibility.Visible;
		}

		public void SetAdvancedView(object viewObject)
		{
			_controlHost.Child = null;
			_controlHost.Child = (UIElement)viewObject;
			_btGotoAdvanced.Visibility = System.Windows.Visibility.Collapsed;
			_btGotoSimple.Visibility = System.Windows.Visibility.Visible;
		}


		#endregion
	}
}
