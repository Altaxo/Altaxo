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

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ProjectBrowseControl : UserControl, IProjectBrowseView, ICSharpCode.SharpDevelop.Gui.IPadContent
	{
		ProjectBrowseController _controller;

		ContextMenu _treeNodeContextMenu;



		public ProjectBrowseControl()
		{
			InitializeComponent();

			_controller = new ProjectBrowseController();

			WpfBrowserTreeNode.Images.Clear();
			WpfBrowserTreeNode.Images.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.Desktop"));
			WpfBrowserTreeNode.Images.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.ClosedFolderBitmap"));
			WpfBrowserTreeNode.Images.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.OpenFolderBitmap"));
			WpfBrowserTreeNode.Images.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.StandardWorksheet"));
			WpfBrowserTreeNode.Images.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.PlotLineScatter"));

			ContextMenu mnu1 = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/ItemList/ContextMenu");
			_listView.ContextMenu = mnu1;

			ContextMenu mnu2 = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/TreeView/ContextMenu");
			_treeView.ContextMenu = mnu2;

			_treeNodeContextMenu = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/TreeNode/ContextMenu");

			_controller.ViewObject = this;

		}

		private void EhTreeNodeAfterSelect(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				TreeViewItem src = (TreeViewItem)e.OriginalSource;
				_controller.EhTreeNodeAfterSelect((NGBrowserTreeNode)src.Tag);
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			//_controller.ViewObject = this;
		}



		public object Control
		{
			get { return this; }
		}

		public object InitiallyFocusedControl
		{
			get { return null; }
		}

		public void Dispose()
		{
		}

		public void InitializeTree(Collections.NGTreeNode root)
		{

			_treeView.Items.Clear();
			WpfRootNode guiRoot = new WpfRootNode(root, _treeView.Items);
			// expand and select the first node
			var firstNode = (TreeViewItem)_treeView.Items[0];

			firstNode.IsExpanded = true;
			firstNode.IsSelected = true;
		}

		public void SilentSelectTreeNode(Collections.NGTreeNode node)
		{
			TreeViewItem tnode = (TreeViewItem)node.GuiTag;

			// Trick to silently select the node: disable the controller temporarily
			var helper = _controller;
			_controller = null;
			tnode.IsSelected = true;
			_controller = helper;
		}

		public object TreeNodeContextMenu
		{
			get { return _treeNodeContextMenu; }
		}

		public void InitializeList(Collections.SelectableListNodeList list)
		{
			_listView.ItemsSource = null;
			_listView.Items.Clear();
			_listView.ItemsSource = list;
		}

		public void SynchronizeListSelection()
		{
			foreach (Collections.SelectableListNode node in _listView.Items)
				node.Selected = false;
			foreach (Collections.SelectableListNode node in _listView.SelectedItems)
				node.Selected = true;
		}

		private void EhListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
		{
			SynchronizeListSelection();
			if (null != _controller)
				_controller.EhListViewDoubleClick();
		}

		private void EhListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SynchronizeListSelection();
			if (null != _controller)
				_controller.EhListViewAfterSelect();
		}
	}
}
