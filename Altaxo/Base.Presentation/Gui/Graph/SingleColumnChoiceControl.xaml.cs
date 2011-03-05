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

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for SingleColumnChoiceControl.xaml
	/// </summary>
	[UserControlForController(typeof(ISingleColumnChoiceViewEventSink))]
	public partial class SingleColumnChoiceControl : UserControl, ISingleColumnChoiceView
	{
		ISingleColumnChoiceViewEventSink _controller;

		public SingleColumnChoiceControl()
		{
			InitializeComponent();
		}

		private void _tvColumns_AfterSelect(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (_controller != null)
			{
				var selitem = _tvColumns.SelectedItem as NGTreeNode;
				if(null!=selitem)
					_controller.EhView_AfterSelectNode(selitem);
			}
		}

		#region  ISingleColumnChoiceView

		public ISingleColumnChoiceViewEventSink Controller
		{
			get
			{

				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void Initialize(Collections.NGTreeNode[] nodes)
		{
			_tvColumns.ItemsSource = nodes;
		}

		public void SelectNode(Collections.NGTreeNode node)
		{
			node.IsSelected = true;
			node.IsExpanded = true;
		}

		public void ExpandNode(Collections.NGTreeNode node)
		{
			node.IsExpanded = true;
		}

		public void InitializeNewNodes(Collections.NGTreeNode[] nodes)
		{
			
		}

		public void EhNodesCleared(Collections.NGTreeNodeCollection nodes)
		{
		}

		#endregion
	}
}
