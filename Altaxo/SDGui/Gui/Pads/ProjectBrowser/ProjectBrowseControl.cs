using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Altaxo.Collections;
using ICSharpCode.Core.WinForms;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  public partial class ProjectBrowseControl : UserControl, IProjectBrowseView, ICSharpCode.SharpDevelop.Gui.IPadContent
  {
    ProjectBrowseController _controller;

		ContextMenuStrip _treeNodeContextMenu;

    public ProjectBrowseControl()
    {
      InitializeComponent();
      _controller = new ProjectBrowseController();

      _treeView.AfterSelect += new TreeViewEventHandler(EhTreeNodeAfterSelect);
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;

			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Desktop"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.StandardWorksheet"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PlotLineScatter"));

			_treeView.ImageList = imglist;
			_listView.SmallImageList = imglist;

			ContextMenuStrip mnu1 = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/ItemList/ContextMenu");
			_listView.ContextMenuStrip = mnu1;

      ContextMenuStrip mnu2 = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/TreeView/ContextMenu");
      _treeView.ContextMenuStrip = mnu2;

			_treeNodeContextMenu = MenuService.CreateContextMenu(this._controller, "/Altaxo/Pads/ProjectBrowser/TreeNode/ContextMenu");

    }

		public void SilentSelectTreeNode(Altaxo.Collections.NGTreeNode node)
		{
			TreeNode tnode = (TreeNode)node.GuiTag;

			// Trick to silently select the node: disable the controller temporarily
			var helper = _controller;
			_controller = null;
			_treeView.SelectedNode = tnode;
			_controller = helper;
		}


    void EhTreeNodeAfterSelect(object sender, TreeViewEventArgs e)
    {
      if (null != _controller)
        _controller.EhTreeNodeAfterSelect((NGBrowserTreeNode)e.Node.Tag);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      _controller.ViewObject = this;
    }

    public void InitializeTree(Altaxo.Collections.NGTreeNode root)
    {
      _treeView.Nodes.Clear();
      GuiRootNode guiRoot = new GuiRootNode(root, _treeView.Nodes);
			// expand and select the first node
			var firstNode = _treeView.Nodes[0];

			firstNode.Expand();
			_treeView.SelectedNode = firstNode;
    }

		public object TreeNodeContextMenu
		{
			get { return _treeNodeContextMenu; }
		}

    public void InitializeList(SelectableListNodeList list)
    {
      GuiHelper.UpdateList(_listView, list);
    }

		public void SynchronizeListSelection()
		{
			GuiHelper.SynchronizeSelectionFromGui(_listView);
		}

    #region IPadContent Members

    Control ICSharpCode.SharpDevelop.Gui.IPadContent.Control
    {
      get { return this; }
    }

    void ICSharpCode.SharpDevelop.Gui.IPadContent.RedrawContent()
    {
    }

    #endregion


    private void EhListViewItemSelecctionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_listView);
      if (null != _controller)
        _controller.EhListViewAfterSelect();
    }

    private void EhListViewItemDoubleClick(object sender, EventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_listView);
      if (null != _controller)
        _controller.EhListViewDoubleClick();
    }
  }



}
