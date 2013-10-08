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
	/// Interaction logic for GraphicItemsControl.xaml
	/// </summary>
	public partial class GraphicItemsControl : UserControl, IGraphicItemsView
	{
		public event Action SelectedItemsUp;

		public event Action SelectedItemsDown;

		public event Action SelectedItemsRemove;

		public GraphicItemsControl()
		{
			InitializeComponent();
		}

		private void EhItemsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiItemsList);
		}

		public Collections.SelectableListNodeList ItemsList
		{
			set
			{
				GuiHelper.Initialize(_guiItemsList, value);
			}
		}

		private void EhSelectedItemsUp_Click(object sender, RoutedEventArgs e)
		{
			if (null != SelectedItemsUp)
				SelectedItemsUp();
		}

		private void EhSelectedItemsDown_Click(object sender, RoutedEventArgs e)
		{
			if (null != SelectedItemsDown)
				SelectedItemsDown();
		}

		private void EhSelectedItemsRemove_Click(object sender, RoutedEventArgs e)
		{
			if (null != SelectedItemsRemove)
				SelectedItemsRemove();
		}
	}
}