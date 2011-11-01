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
	/// Interaction logic for SingleTreeViewItemChoiceControl.xaml
	/// </summary>
	public partial class SingleTreeViewItemChoiceControl : UserControl, Altaxo.Gui.Worksheet.ISingleTreeViewItemChoiceView
	{
		public event Action<Collections.NGTreeNode> SelectionChanged;

		public SingleTreeViewItemChoiceControl()
		{
			InitializeComponent();
		}

		private void EhSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is Collections.NGTreeNode)
			{
				if (null != SelectionChanged)
					SelectionChanged((Collections.NGTreeNode)e.NewValue);
			}
		}

		public void Initialize(Collections.NGTreeNodeCollection nodes)
		{
			_guiTreeView.ItemsSource = null;
			_guiTreeView.ItemsSource = nodes;
		}

		
	}
}
