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

#endregion Copyright

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	public class NGBrowserTreeNode : NGTreeNode
	{
		public NGBrowserTreeNode()
		{
		}

		public NGBrowserTreeNode(string txt)
			: base(txt)
		{
		}

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
		public BrowserListItem(string name, object item, bool sel)
			: base(name, item, sel)
		{
		}

		public new ProjectBrowseItemImage Image;
		private DateTime _creationDate;

		public override int ImageIndex
		{
			get
			{
				return (int)Image;
			}
		}

		public System.Windows.Media.ImageSource ImageSource { get { return WpfBrowserTreeNode.Images[ImageIndex]; } }

		public DateTime CreationDate
		{
			get { return _creationDate; }
			set
			{
				var oldValue = _creationDate;
				_creationDate = value;
				if (oldValue != _creationDate)
				{
					OnPropertyChanged("CreationDate");
					OnPropertyChanged("Text1");
				}
			}
		}

		public override string Text1
		{
			get
			{
				if (_creationDate == default(DateTime))
					return null;
				else
					return _creationDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
			}
		}

		public enum SortKind { None, Name, CreationDate }

		public class Comparer : IComparer<SelectableListNode>
		{
			private Tuple<SortKind, bool>[] _sort;

			public Comparer(SortKind sort, bool descending)
			{
				_sort = new Tuple<SortKind, bool>[] { new Tuple<SortKind, bool>(sort, descending) };
			}

			public Comparer(SortKind sort1, bool descending1, SortKind sort2, bool descending2)
			{
				_sort = new Tuple<SortKind, bool>[] { new Tuple<SortKind, bool>(sort1, descending1), new Tuple<SortKind, bool>(sort2, descending2) };
			}

			public int Compare(SelectableListNode x, SelectableListNode y)
			{
				var xx = (BrowserListItem)x;
				var yy = (BrowserListItem)y;
				int result = 0;

				// before doing the real comparison, we handle a special case that ensures that folders are always first
				// if one of the both is a ProjectFolder, this folder is always "first", i.e. returns a -1
				if ((xx.Tag is Altaxo.Main.ProjectFolder) ^ (yy.Tag is Altaxo.Main.ProjectFolder))
					return (xx.Tag is Altaxo.Main.ProjectFolder) ? -1 : 1;

				// now the "real" comparison

				foreach (var tuple in _sort) // Tuple.Item1 is SortKind, Tuple.Item2 is descending
				{
					switch (tuple.Item1)
					{
						case SortKind.Name:
							result = tuple.Item2 ? string.Compare(y.Text, x.Text) : string.Compare(x.Text, y.Text);
							break;

						case SortKind.CreationDate:
							result = tuple.Item2 ? DateTime.Compare(yy._creationDate, xx._creationDate) : DateTime.Compare(xx._creationDate, yy._creationDate);
							break;
					}
					if (0 != result)
						return result;
				}

				// wenn die Sort-Kriterien nicht reichen, entscheiden wir anhand des Tags
				if (null != x.Tag && null == y.Tag)
					return 1;
				else if (null == x.Tag && null != y.Tag)
					return -1;

				result = string.Compare(x.Tag.GetType().ToString(), y.Tag.GetType().ToString());
				if (0 != result)
					return result;

				return 1;
			}
		}

		public static void Sort(SelectableListNodeList list, IComparer<SelectableListNode> comparer)
		{
			SortedSet<SelectableListNode> sset = new SortedSet<SelectableListNode>(list, comparer);
			list.Clear();
			foreach (var item in sset)
				list.Add(item);
		}
	}

	public enum ProjectBrowseItemImage
	{
		Project = 0,
		ClosedFolder = 1,
		OpenFolder = 2,
		Worksheet = 3,
		Graph = 4,
		PropertyBag = 5
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