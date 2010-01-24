using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
using System.Windows.Forms;
namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Extends the WinForms tree node for special interaction features with their non-Gui equivalents.
  /// </summary>
  public class GuiBrowserTreeNode : TreeNode, IGuiBrowserTreeNode
  {
    public GuiBrowserTreeNode(NGBrowserTreeNode nguinode)
    {
      this.Tag = nguinode;
      this.Text = nguinode.Text;
      nguinode.GuiTag = this;
    }

    public virtual void OnNodeAdded(NGBrowserTreeNode node)
    {
      AddNode(this.Nodes, node);
    }


    public virtual void OnNodeRemoved(NGBrowserTreeNode node)
    {
      TreeNode n = (TreeNode)node.GuiTag;
      Nodes.Remove(n);
    }

    public virtual void OnNodeMultipleChanges()
    {
      var nguinode = (NGTreeNode)this.Tag;
      this.Nodes.Clear();
      foreach (var n in nguinode.Nodes)
        AddNode(this.Nodes, (NGBrowserTreeNode)n);
    }

		public static void AddNode(TreeNodeCollection parNodes, NGBrowserTreeNode nguinode)
		{
			var curNode = new GuiBrowserTreeNode(nguinode);
			curNode.ContextMenuStrip = (ContextMenuStrip)nguinode.ContextMenu;

			if (null != nguinode.ImageIndex)
				curNode.ImageIndex = (int)nguinode.ImageIndex;
			if (null != nguinode.SelectedImageIndex)
				curNode.SelectedImageIndex = (int)nguinode.ImageIndex;

			foreach (var n in nguinode.Nodes)
				AddNode(curNode.Nodes, (NGBrowserTreeNode)n);
			parNodes.Add(curNode);
		}
	}


	/// <summary>
	/// Simulate the equivalent of the non-Gui root node. Note that in the TreeList there is no such thing than a root node.
	/// There is only the collection of nodes at the root.
	/// </summary>
  public class GuiRootNode : IGuiBrowserTreeNode
  {
    TreeNodeCollection _rootCollection;
    NGTreeNode _nguiRoot;

    public GuiRootNode(NGTreeNode nguiroot, TreeNodeCollection rootCollection)
    {
      _rootCollection = rootCollection;
      foreach (var n in nguiroot.Nodes)
        GuiBrowserTreeNode.AddNode(_rootCollection, (NGBrowserTreeNode)n);

      _nguiRoot = nguiroot;
      nguiroot.GuiTag = this;
    }

    public void OnNodeAdded(NGBrowserTreeNode node)
    {
      GuiBrowserTreeNode.AddNode(_rootCollection, (NGBrowserTreeNode)node);
    }

    public void OnNodeRemoved(NGBrowserTreeNode node)
    {
      TreeNode n = (TreeNode)node.GuiTag;
      _rootCollection.Remove(n);
    }

    public virtual void OnNodeMultipleChanges()
    {
     _rootCollection.Clear();
     foreach (var n in _nguiRoot.Nodes)
       GuiBrowserTreeNode.AddNode(_rootCollection, (NGBrowserTreeNode)n);
    }

  }
}
