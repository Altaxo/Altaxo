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

using Altaxo;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Main.Properties
{
	public class ProjectFolderPropertyDocumentCollection :
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<ProjectFolderPropertyDocumentCollectionChangedEventArgs>,
		IEnumerable<ProjectFolderPropertyDocument>,
		Altaxo.Main.IDocumentNode,
		Altaxo.Main.IChangedEventSource,
		Altaxo.Main.IChildChangedEventSink,
		Altaxo.Main.INamedObjectCollection
	{
		// Data
		protected SortedDictionary<string, ProjectFolderPropertyDocument> _itemsByName = new SortedDictionary<string, ProjectFolderPropertyDocument>();

		protected bool _isDirty = false;

		// Events

		/// <summary>
		/// Fired when one or more graphs are added, deleted or renamed. Not fired when content in the graph has changed.
		/// </summary>
		public event Action<Main.NamedObjectCollectionChangeType, object, string, string> CollectionChanged;

		public ProjectFolderPropertyDocumentCollection(AltaxoDocument parent)
		{
			this._parent = parent;
		}

		public override object ParentObject
		{
			get { return this._parent; }
			set
			{
				throw new InvalidOperationException("ParentObject of ProjectFolderPropertyDocumentCollection is fixed and cannot be set");
			}
		}

		public override string Name
		{
			get { return "FolderProperties"; }
			set
			{
				throw new InvalidOperationException("Name of ProjectFolderPropertyDocumentCollection is fixed and cannot be set");
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
			item.NameChanged += EhChild_NameChanged;
			this.EhSelfChanged(ProjectFolderPropertyDocumentCollectionChangedEventArgs.IfItemAdded);
			OnCollectionChanged(Main.NamedObjectCollectionChangeType.ItemAdded, item, item.Name);
		}

		public void Remove(ProjectFolderPropertyDocument item)
		{
			if (item != null && item.Name != null)
			{
				var gr = (ProjectFolderPropertyDocument)_itemsByName[item.Name];

				if (null != Current.ComManager && object.ReferenceEquals(gr, Current.ComManager.EmbeddedObject)) // test if the graph is currently the embedded Com object
					return; // it is not allowed to remove the current embedded graph object.

				if (object.ReferenceEquals(gr, item))
				{
					_itemsByName.Remove(item.Name);
					item.ParentObject = null;
					item.NameChanged -= EhChild_NameChanged;
					this.EhSelfChanged(ProjectFolderPropertyDocumentCollectionChangedEventArgs.IfItemRemoved);
					OnCollectionChanged(Main.NamedObjectCollectionChangeType.ItemRemoved, item, item.Name);
				}
			}
		}

		protected void EhChild_NameChanged(Main.INameOwner item, string oldName)
		{
			if (_itemsByName.ContainsKey(item.Name))
			{
				if (object.ReferenceEquals(_itemsByName[item.Name], item))
					return;
				else
					throw new ApplicationException(string.Format("The collection contains already a property bag named {0}, renaming the old property bag {1} fails therefore.", item.Name, oldName));
			}
			_itemsByName.Remove(oldName);
			_itemsByName[item.Name] = (ProjectFolderPropertyDocument)item;
			this.EhSelfChanged(ProjectFolderPropertyDocumentCollectionChangedEventArgs.IfItemRenamed);
			OnCollectionChanged(Main.NamedObjectCollectionChangeType.ItemRenamed, item, oldName);
		}

		public object GetChildObjectNamed(string name)
		{
			ProjectFolderPropertyDocument result = null;
			if (_itemsByName.TryGetValue(name, out result))
				return result;
			else return null;
		}

		public string GetNameOfChildObject(object o)
		{
			if (o is ProjectFolderPropertyDocument)
			{
				ProjectFolderPropertyDocument gr = (ProjectFolderPropertyDocument)o;
				if (_itemsByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}

		#region Change event handling

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			if (_accumulatedEventData == null)
				this._accumulatedEventData = ProjectFolderPropertyDocumentCollectionChangedEventArgs.Empty;

			if (e is ProjectFolderPropertyDocumentCollectionChangedEventArgs)
				_accumulatedEventData.Merge((ProjectFolderPropertyDocumentCollectionChangedEventArgs)e);
		}

		protected virtual void OnCollectionChanged(Main.NamedObjectCollectionChangeType changeType, Main.INameOwner item, string oldName)
		{
			if (this.CollectionChanged != null)
				CollectionChanged(changeType, item, oldName, item.Name);
		}

		#endregion Change event handling

		/// <summary>
		/// Gets the parent ProjectFolderPropertyBagCollection of a child graph.
		/// </summary>
		/// <param name="child">A graph for which the parent collection is searched.</param>
		/// <returns>The parent ProjectFolderPropertyBagCollection, if it exists, or null otherwise.</returns>
		public static ProjectFolderPropertyDocumentCollection GetParentProjectFolderPropertyBagCollectionOf(Main.IDocumentNode child)
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