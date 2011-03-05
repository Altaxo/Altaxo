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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for TabbedElementControl.xaml
	/// </summary>
	public partial class TabbedElementControl : UserControl, ITabbedElementView
	{
		public TabbedElementControl()
		{
			InitializeComponent();
		}

		public void ClearTabs()
		{
			foreach (TabItem item in _tabControl.Items)
			{
				item.LostFocus -= item_LostFocus;
				item.GotFocus -= item_GotFocus;
				item.Content = null;
			}

			_tabControl.Items.Clear();
		}

		public void AddTab(string title, object view)
		{
			var item = new TabItem();
			item.Header = title;

			
				item.Content = (UIElement)view;
			

			item.LostFocus += item_LostFocus;
			item.GotFocus += item_GotFocus;

			_tabControl.Items.Add(item);
		}

		void item_GotFocus(object sender, RoutedEventArgs e)
		{
			if (null != ChildControl_Entered)
				ChildControl_Entered(sender, EventArgs.Empty);
		}

		void item_LostFocus(object sender, RoutedEventArgs e)
		{
			if (null != ChildControl_Validated)
				ChildControl_Validated(sender, EventArgs.Empty);
		}

		public void BringTabToFront(int index)
		{
			_tabControl.SelectedIndex = index;
			var selItem = _tabControl.SelectedItem as TabItem;
			if (selItem != null)
				selItem.Focus();
		}

		public event EventHandler ChildControl_Entered;

		public event EventHandler ChildControl_Validated;
	}
}
