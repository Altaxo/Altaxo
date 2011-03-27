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
	/// Interaction logic for G2DPlotItemControl.xaml
	/// </summary>
	public partial class G2DPlotItemControl : UserControl, IG2DPlotItemView
	{
		public G2DPlotItemControl()
		{
			InitializeComponent();
		}

		private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.OriginalSource == _tabControl)
			{
				TabItem newItem = null;
				TabItem oldItem = null;

				if (e.AddedItems.Count > 0)
					newItem = (TabItem)e.AddedItems[0];
				if (e.RemovedItems.Count > 0)
					oldItem = (TabItem)e.RemovedItems[0];

				object newContent = null == newItem ? null : newItem.Content;
				object oldContent = null == oldItem ? null : oldItem.Content;

				if (null != SelectedPage_Changed)
					SelectedPage_Changed(this, new Main.InstanceChangedEventArgs<object>(oldContent, newContent));
			}
		}

		#region IG2DPlotItemView

		public void ClearTabs()
		{
			// decouple controls from the tabitems
			foreach (TabItem tabItem in _tabControl.Items)
				tabItem.Content = null;
			_tabControl.Items.Clear();
		}

		public void AddTab(string title, object view)
		{
			var tabItem = new TabItem() { Header = title, Content = view };
			_tabControl.Items.Add(tabItem);
		}

		public void BringTabToFront(int index)
		{
			_tabControl.SelectedIndex = index;
		}

		public event Action<object, Main.InstanceChangedEventArgs<object>> SelectedPage_Changed;

		public void SetPlotStyleView(object view)
		{
			_plotStyleCollectionControlHost.Content = (UIElement)view;
		}

		public void SetPlotGroupCollectionView(object view)
		{
			_plotGroupCollectionControlHost.Content = (UIElement)view;
		}

		#endregion

	
	}
}
