#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;

namespace Altaxo.Main.Properties
{
	public class ProjectFolderPropertyDocumentCollection :
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		Main.IParentOfINameOwnerChildNodes,
		IEnumerable<ProjectFolderPropertyDocument>,
		Altaxo.Main.INamedObjectCollection
	{
		// Data
		protected SortedDictionary<string, ProjectFolderPropertyDocument> _itemsByName = new SortedDictionary<string, ProjectFolderPropertyDocument>();

		protected bool _isDirty = false;

		// Events

		/// <summary>
		/// Fired when one or more items are added, deleted or renamed. Not fired when the content of an item has changed.
		/// </summary>
		public event EventHandler<NamedObjectCollectionChangedEventArgs> CollectionChanged;

		public ProjectFolderPropertyDocumentCollection(AltaxoDocument parent)
		{
			this._parent = parent;
		}

		public override Main.IDocumentNode ParentObject
		{
			get { return this._parent; }
			set
			{
				if (null != value)
					throw new InvalidOperationException("ParentObject of ProjectFolderPropertyDocumentCollection is fixed and cannot be set");

				base.ParentObject = value; // allow setting Parent to null (required for dispose)
			}
		}

		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}

		public string[] GetSortedItemNames()
		{
			string[] arr = new string[_itemsByName.Count];
			this._itemsByName.Keys.CopyTo(arr, 0);
			System.Array.Sort(arr);
			return arr;
		}

		public ProjectFolderPropertyDocument this[string name]
		{
			get
			{
				return _itemsByName[name];
			}
		}

		public bool Contains(string itemName)
		{
			return _itemsByName.ContainsKey(itemName);
		}

		public bool TryGetValue(string itemName, out ProjectFolderPropertyDocument doc)
		{
			return _itemsByName.TryGetValue(itemName, out doc);
		}

		public void Add(ProjectFolderPropertyDocument item)
		{
			if (!string.IsNullOrEmpty(item.Name) && _itemsByName.ContainsKey(item.Name) && object.ReferenceEquals(item, _itemsByName[item.Name]))
				return; // do silently nothing if the graph (the same!) is already registered
			else if (_itemsByName.ContainsKey(item.Name)) // else if this table name is already in use
				throw new ArgumentException(string.Format("Another property bag with the same name is already present in the collection! Name of the property bag: {0}", item.Name));

			// now the table has a unique name in any case
			_itemsByName.Add(item.Name, item);
			item.ParentObject = this;
			this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(item));
		}

		public void Remove(ProjectFolderPropertyDocument item)
		{
			if (item != null && item.Name != null)
			{
				var gr = (ProjectFolderPropertyDocument)_itemsByName[item.Name];

				if (object.ReferenceEquals(gr, item))
				{
					var changeEventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(item);
					_itemsByName.Remove(item.Name);
					item.Dispose();
					this.EhSelfChanged(changeEventArgs);
				}
			}
		}

		bool Main.IParentOfINameOwnerChildNodes.EhChild_CanBeRenamed(Main.INameOwner childNode, string newName)
		{
			if (_itemsByName.ContainsKey(newName) && !object.ReferenceEquals(_itemsByName[newName], childNode))
				return false;
			else
				return true;
		}

		void Main.IParentOfINameOwnerChildNodes.EhChild_HasBeenRenamed(Main.INameOwner item, string oldName)
		{
			if (_itemsByName.ContainsKey(item.Name))
			{
				if (object.ReferenceEquals(_itemsByName[item.Name], item))
					return; // Table alredy renamed
				else
					throw new ApplicationException("Table with name " + item.Name + " already exists!");
			}

			if (_itemsByName.ContainsKey(oldName))
			{
				if (!object.ReferenceEquals(_itemsByName[oldName], item))
					throw new ApplicationException("Name between Collection and Item not in sync");

				_itemsByName.Remove(oldName);
				_itemsByName.Add(item.Name, (ProjectFolderPropertyDocument)item);

				EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
			}
			else
			{
				throw new ApplicationException("Error renaming property bag " + oldName + " : this name was not found in the collection!");
			}
		}

		void Main.IParentOfINameOwnerChildNodes.EhChild_ParentChanged(Main.INameOwner childNode, Main.IDocumentNode oldParent)
		{
			if (object.ReferenceEquals(this, oldParent) && _itemsByName.ContainsKey(childNode.Name))
				throw new InvalidProgramException("Unauthorized change of the child's parent");
		}

		public override IDocumentLeafNode GetChildObjectNamed(string name)
		{
			ProjectFolderPropertyDocument result = null;
			if (_itemsByName.TryGetValue(name, out result))
				return result;
			else return null;
		}

		public override string GetNameOfChildObject(IDocumentLeafNode o)
		{
			if (o is ProjectFolderPropertyDocument)
			{
				ProjectFolderPropertyDocument gr = (ProjectFolderPropertyDocument)o;
				if (_itemsByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			foreach (var entry in _itemsByName)
				yield return new Main.DocumentNodeAndName(entry.Value, entry.Key);
		}

		#region Change event handling

		protected override void OnChanged(EventArgs e)
		{
			if (e is NamedObjectCollectionChangedEventArgs && null != CollectionChanged)
			{
				CollectionChanged(this, (NamedObjectCollectionChangedEventArgs)e);
			}

			base.OnChanged(e);
		}

		#endregion Change event handling

		/// <summary>
		/// Gets the parent ProjectFolderPropertyBagCollection of a child graph.
		/// </summary>
		/// <param name="child">A graph for which the parent collection is searched.</param>
		/// <returns>The parent ProjectFolderPropertyBagCollection, if it exists, or null otherwise.</returns>
		public static ProjectFolderPropertyDocumentCollection GetParentProjectFolderPropertyBagCollectionOf(Main.IDocumentLeafNode child)
		{
			return (ProjectFolderPropertyDocumentCollection)Main.DocumentPath.GetRootNodeImplementing(child, typeof(ProjectFolderPropertyDocumentCollection));
		}

		#region IEnumerable<ProjectFolderPropertyBag> Members

		public IEnumerator<ProjectFolderPropertyDocument> GetEnumerator()
		{
			return _itemsByName.Values.GetEnumerator();
		}

		#endregion IEnumerable<ProjectFolderPropertyBag> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _itemsByName.Values.GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}