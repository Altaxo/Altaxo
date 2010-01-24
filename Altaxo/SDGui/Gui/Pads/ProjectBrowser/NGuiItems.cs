using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
namespace Altaxo.Gui.Pads.ProjectBrowser
{
  public class NGBrowserTreeNode : NGTreeNode
  {
    public NGBrowserTreeNode() { }
    public NGBrowserTreeNode(string txt) : base(txt) { }

    public ProjectBrowseItemImage Image;
    public override int? ImageIndex
    {
      get { return (int)Image; }
    }
    public override int? SelectedImageIndex
    {
      get { return (int)Image; }
    }

		public object ContextMenu;
		public void SetContextMenuRecursively(object contextMenu)
		{
			ContextMenu = contextMenu;
			foreach (NGBrowserTreeNode node in Nodes)
				node.SetContextMenuRecursively(contextMenu);
		}

  }

  public class BrowserListItem : SelectableListNode
  {
    public BrowserListItem(string name, object item, bool sel) : base(name, item, sel) { }
    public ProjectBrowseItemImage Image;
    public override int ImageIndex
    {
      get
      {
        return (int)Image;
      }
    }
  }

  public enum ProjectBrowseItemImage
  {
    Project = 0,
    ClosedFolder = 1,
    OpenFolder = 2,
    Worksheet = 3,
    Graph = 4
  }

	public enum ViewOnSelect
	{
		Off,
		ItemsInFolder,
		ItemsInFolderAndSubfolders
	}

  public interface IGuiBrowserTreeNode
  {
    void OnNodeAdded(NGBrowserTreeNode node);
    void OnNodeRemoved(NGBrowserTreeNode node);
    void OnNodeMultipleChanges();
  }

	/// <summary>
	/// Helper class to distinguish between 'real' folders and the '..' folder.
	/// </summary>
	public class ParentProjectFolder
	{
		public string Name { get; private set; }
		public ParentProjectFolder(string name) { Name = name; }
	}
}
