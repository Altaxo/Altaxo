#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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

		public override int ImageIndex
		{
			get { return (int)Image; }
		}

		public override int SelectedImageIndex
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

		public System.Windows.Media.ImageSource ImageSource { get { return WpfBrowserTreeNode.Images[ImageIndex]; } }
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

	
}
