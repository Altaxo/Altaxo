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

		public object ContextMenu { get; set; }

		public void SetContextMenuRecursively(object contextMenu)
		{
			ContextMenu = contextMenu;
			foreach (NGBrowserTreeNode node in Nodes)
				node.SetContextMenuRecursively(contextMenu);
		}

		public virtual bool IsRenamingEnabled { get { return false; } }

		/// <summary>
		/// Bind the validation to this property and use a <see cref="Altaxo.Gui.Common.ConverterStringFuncToValidationRule"/> converter to convert it into a validation rule.
		/// </summary>
		/// <value>
		/// The renaming validation function.
		/// </value>
		public Func<object, System.Globalization.CultureInfo, string> RenamingValidationFunction
		{
			get
			{
				return ValidateRenaming;
			}
		}

		protected virtual string ValidateRenaming(object obj, System.Globalization.CultureInfo info)
		{
			return "Item renaming not supported!";
		}
	}

	public class NGProjectFolderTreeNode : NGBrowserTreeNode
	{
		public NGProjectFolderTreeNode(string txt)
			: base(txt)
		{
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (TryRenaming(value))
					base.Text = value;
			}
		}

		#region Renaming

		public override bool IsRenamingEnabled { get { return true; } }

		protected override string ValidateRenaming(object obj, System.Globalization.CultureInfo info)
		{
			string newShortName = (string)obj;

			var oldFolderFullName = Tag as string; // Full name with trailing directory separator char
			if (null == oldFolderFullName)
				return "Item renaming not supported!";

			return null;
		}

		/// <summary>
		/// Try renaming the item. Returns <c>true</c> if the renaming was successful.
		/// </summary>
		/// <param name="name">The new name.</param>
		/// <returns></returns>
		private bool TryRenaming(string name)
		{
			string newShortName = name;
			var oldFolderFullName = Tag as string;

			string oldParentFolder, oldLastPart;

			Altaxo.Main.ProjectFolder.SplitFolderIntoParentFolderAndLastFolderPart(oldFolderFullName, out oldParentFolder, out oldLastPart);

			string newFolderFullName = oldParentFolder + newShortName + Altaxo.Main.ProjectFolder.DirectorySeparatorString;

			if (!Current.Project.Folders.CanRenameFolder(oldFolderFullName, newFolderFullName))
			{
				if (false == Current.Gui.YesNoMessageBox(
					"Some of the new item names conflict with existing items. Those items will be renamed with " +
					"a generated name based on the old name. Do you want to continue?", "Attention", false))
					return false;
			}

			Current.Project.Folders.RenameFolder(oldFolderFullName, newFolderFullName);

			return true;
		}

		#endregion Renaming
	}

	public class BrowserListItem : SelectableListNode
	{
		public new ProjectBrowseItemImage Image;
		private DateTime _creationDate;

		/// <summary>True when the text in the 'Text' property is the full name of the item. False if this text is only the short name (without the folder part).
		/// </summary>
		private bool _nameIsFullName;

		public BrowserListItem(string name, bool nameIsFullName, object item, bool sel)
			: base(name, item, sel)
		{
			_nameIsFullName = nameIsFullName;
		}

		public override int ImageIndex
		{
			get
			{
				return (int)Image;
			}
		}

		//public System.Windows.Media.ImageSource ImageSource { get { return WpfBrowserTreeNode.Images[ImageIndex]; } }

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

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (TryRenaming(value))
					base.Text = value;
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

		#region Renaming

		/// <summary>
		/// Bind the validation to this property and use a <see cref="Altaxo.Gui.Common.ConverterStringFuncToValidationRule"/> converter to convert it into a validation rule.
		/// </summary>
		/// <value>
		/// The renaming validation function.
		/// </value>
		public Func<object, System.Globalization.CultureInfo, string> RenamingValidationFunction
		{
			get
			{
				return ValidateRenaming;
			}
		}

		private string ValidateRenaming(object obj, System.Globalization.CultureInfo info)
		{
			string name = (string)obj;

			var item = Tag as Altaxo.Main.INamedObject;
			if (null == item)
				return "Item renaming not supported!";

			string fullName = GetResultingName(name, _nameIsFullName, item);

			if (fullName == item.Name)
				return null;

			if (item is Altaxo.Graph.Gdi.GraphDocument)
			{
				if (Current.Project.GraphDocumentCollection.ContainsAnyName(name))
					return "A graph with the same name is already present in the project";
			}
			else if (item is Altaxo.Graph.Graph3D.GraphDocument)
			{
				if (Current.Project.Graph3DDocumentCollection.ContainsAnyName(name))
					return "A graph with the same name is already present in the project";
			}
			else if (item is Altaxo.Data.DataTable)
			{
				if (Current.Project.DataTableCollection.ContainsAnyName(name))
					return "A table with the same name is already present in the project";
			}
			else if (item is Altaxo.Text.TextDocument)
			{
				if (Altaxo.Main.ProjectFolder.IsValidFolderName(item.Name)) // if it is a project folder note
					return "A project folder note can not be renamed";

				if (Current.Project.TextDocumentCollection.ContainsAnyName(name))
					return "A text document with the same name is already present in the project";
			}
			else if (item is Altaxo.Main.ProjectFolder)
			{
				// nothing to do, any name is possible here
			}
			else
			{
				return "Item renaming not supported!";
			}

			return null;
		}

		/// <summary>
		/// Try renaming the item. Returns <c>true</c> if the renaming was successful.
		/// </summary>
		/// <param name="name">The new name.</param>
		/// <returns></returns>
		private bool TryRenaming(string name)
		{
			// if the text has changed, test if this is because the item was renamed or has to be renamed
			var item = Tag as Altaxo.Main.INamedObject;
			if (null != item)
			{
				string fullName = GetResultingName(name, _nameIsFullName, item);
				if (fullName == item.Name)
					return true; // nothing to do

				if (item is Altaxo.Graph.Gdi.GraphDocument)
				{
					if (!Current.Project.GraphDocumentCollection.ContainsAnyName(name))
					{
						((Altaxo.Graph.Gdi.GraphDocument)item).Name = fullName;
						return true;
					}
				}
				if (item is Altaxo.Graph.Graph3D.GraphDocument)
				{
					if (!Current.Project.Graph3DDocumentCollection.ContainsAnyName(name))
					{
						((Altaxo.Graph.Graph3D.GraphDocument)item).Name = fullName;
						return true;
					}
				}
				else if (item is Altaxo.Data.DataTable)
				{
					if (!Current.Project.DataTableCollection.ContainsAnyName(name))
					{
						((Altaxo.Data.DataTable)item).Name = fullName;
						return true;
					}
				}
				else if (item is Altaxo.Text.TextDocument)
				{
					if (!Current.Project.TextDocumentCollection.ContainsAnyName(name))
					{
						((Altaxo.Text.TextDocument)item).Name = fullName;
						return true;
					}
				}
				else if (item is Altaxo.Main.ProjectFolder)
				{
					Current.Project.Folders.RenameFolder(item.Name, fullName);
					return true;
				}
			}
			return false;
		}

		private static string GetResultingName(string value, bool nameIsFullName, Altaxo.Main.INamedObject item)
		{
			if (nameIsFullName)
				return value;

			if (item is Altaxo.Main.ProjectFolder)
			{
				var folder = Altaxo.Main.ProjectFolder.GetFoldersParentFolder(item.Name);
				return folder + value + Altaxo.Main.ProjectFolder.DirectorySeparatorChar;
			}
			else // any other item
			{
				var folder = Altaxo.Main.ProjectFolder.GetFolderPart(item.Name);
				return folder + value;
			}
		}

		#endregion Renaming
	}

	public enum ProjectBrowseItemImage
	{
		Project = 0,
		ClosedFolder = 1,
		OpenFolder = 2,
		Worksheet = 3,
		Graph = 4,
		PropertyBag = 5,
		TextDocument = 6,
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
