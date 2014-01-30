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
	public class ProjectFolderPropertyBagCollection :
		IEnumerable<ProjectFolderPropertyBag>,
		Altaxo.Main.IDocumentNode,
		Altaxo.Main.IChangedEventSource,
		Altaxo.Main.IChildChangedEventSink,
		Altaxo.Main.INamedObjectCollection
	{
		#region ChangedEventArgs

		/// <summary>
		/// Holds information about what has changed in the table.
		/// </summary>
		protected class ChangedEventArgs : System.EventArgs
		{
			/// <summary>
			/// If true, one or more tables where added.
			/// </summary>
			public bool ItemAdded;

			/// <summary>
			/// If true, one or more table where removed.
			/// </summary>
			public bool ItemRemoved;

			/// <summary>
			/// If true, one or more tables where renamed.
			/// </summary>
			public bool ItemRenamed;

			/// <summary>
			/// Empty constructor.
			/// </summary>
			public ChangedEventArgs()
			{
			}

			/// <summary>
			/// Returns an empty instance.
			/// </summary>
			public static new ChangedEventArgs Empty
			{
				get { return new ChangedEventArgs(); }
			}

			/// <summary>
			/// Returns an instance with TableAdded set to true;.
			/// </summary>
			public static ChangedEventArgs IfItemAdded
			{
				get
				{
					ChangedEventArgs e = new ChangedEventArgs();
					e.ItemAdded = true;
					return e;
				}
			}

			/// <summary>
			/// Returns an instance with TableRemoved set to true.
			/// </summary>
			public static ChangedEventArgs IfItemRemoved
			{
				get
				{
					ChangedEventArgs e = new ChangedEventArgs();
					e.ItemRemoved = true;
					return e;
				}
			}

			/// <summary>
			/// Returns an  instance with TableRenamed set to true.
			/// </summary>
			public static ChangedEventArgs IfItemRenamed
			{
				get
				{
					ChangedEventArgs e = new ChangedEventArgs();
					e.ItemRenamed = true;
					return e;
				}
			}

			/// <summary>
			/// Merges information from another instance in this ChangedEventArg.
			/// </summary>
			/// <param name="from"></param>
			public void Merge(ChangedEventArgs from)
			{
				this.ItemAdded |= from.ItemAdded;
				this.ItemRemoved |= from.ItemRemoved;
				this.ItemRenamed |= from.ItemRenamed;
			}

			/// <summary>
			/// Returns true when the collection has changed (addition, removal or renaming of tables).
			/// </summary>
			public bool CollectionChanged
			{
				get { return ItemAdded | ItemRemoved | ItemRenamed; }
			}
		}

		#endregion ChangedEventArgs

		// Data
		protected SortedDictionary<string, ProjectFolderPropertyBag> _itemsByName = new SortedDictionary<string, ProjectFolderPropertyBag>();

		protected bool _isDirty = false;

		[NonSerialized]
		protected object _parentObject = null;

		[NonSerialized]
		protected ChangedEventArgs _changeData = null;

		[NonSerialized()]
		protected bool _isResumeInProgress = false;

		[NonSerialized()]
		protected System.Collections.ArrayList _suspendedChildCollection = new System.Collections.ArrayList();

		// Events

		/// <summary>
		/// Fired when one or more graphs are added, deleted or renamed. Not fired when content in the graph has changed.
		/// </summary>
		public event Action<Main.NamedObjectCollectionChangeType, object, string, string> CollectionChanged;

		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		#endregion IChangedEventSource Members

		public ProjectFolderPropertyBagCollection(AltaxoDocument parent)
		{
			this._parentObject = parent;
		}

		public object Parent
		{
			get { return this._parentObject; }
			set { this._parentObject = value; }
		}

		public object ParentObject
		{
			get { return this._parentObject; }
		}

		public string Name
		{
			get { return "FolderProperties"; }
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

		public ProjectFolderPropertyBag this[string name]
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

		public bool TryGetValue(string itemName, out ProjectFolderPropertyBag doc)
		{
			return _itemsByName.TryGetValue(itemName, out doc);
		}

		public void Add(ProjectFolderPropertyBag item)
		{
			if (!string.IsNullOrEmpty(item.Name) && _itemsByName.ContainsKey(item.Name) && object.ReferenceEquals(item, _itemsByName[item.Name]))
				return; // do silently nothing if the graph (the same!) is already registered
			else if (_itemsByName.ContainsKey(item.Name)) // else if this table name is already in use
				throw new ArgumentException(string.Format("Another property bag with the same name is already present in the collection! Name of the property bag: {0}", item.Name));

			// now the table has a unique name in any case
			_itemsByName.Add(item.Name, item);
			item.ParentObject = this;
			item.NameChanged += EhChild_NameChanged;
			this.OnSelfChanged(ChangedEventArgs.IfItemAdded);
			OnCollectionChanged(Main.NamedObjectCollectionChangeType.ItemAdded, item, item.Name);
		}

		public void Remove(ProjectFolderPropertyBag item)
		{
			if (item != null && item.Name != null)
			{
				var gr = (ProjectFolderPropertyBag)_itemsByName[item.Name];

				if (null != Current.ComManager && object.ReferenceEquals(gr, Current.ComManager.EmbeddedObject)) // test if the graph is currently the embedded Com object
					return; // it is not allowed to remove the current embedded graph object.

				if (object.ReferenceEquals(gr, item))
				{
					_itemsByName.Remove(item.Name);
					item.ParentObject = null;
					item.NameChanged -= EhChild_NameChanged;
					this.OnSelfChanged(ChangedEventArgs.IfItemRemoved);
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
			_itemsByName[item.Name] = (ProjectFolderPropertyBag)item;
			this.OnSelfChanged(ChangedEventArgs.IfItemRenamed);
			OnCollectionChanged(Main.NamedObjectCollectionChangeType.ItemRenamed, item, oldName);
		}

		public object GetChildObjectNamed(string name)
		{
			ProjectFolderPropertyBag result = null;
			if (_itemsByName.TryGetValue(name, out result))
				return result;
			else return null;
		}

		public string GetNameOfChildObject(object o)
		{
			if (o is ProjectFolderPropertyBag)
			{
				ProjectFolderPropertyBag gr = (ProjectFolderPropertyBag)o;
				if (_itemsByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}

		#region Change event handling

		public bool IsSuspended
		{
			get
			{
				return false; // m_SuspendCount>0;
			}
		}

		private void AccumulateChildChangeData(object sender, EventArgs e)
		{
			if (_changeData == null)
				this._changeData = ChangedEventArgs.Empty;

			if (e is ChangedEventArgs)
				_changeData.Merge((ChangedEventArgs)e);
		}

		protected bool HandleImmediateChildChangeCases(object sender, EventArgs e)
		{
			return false; // not handled
		}

		protected virtual void OnSelfChanged(EventArgs e)
		{
			EhChildChanged(null, e);
		}

		/// <summary>
		/// Handle the change notification from the child layers.
		/// </summary>
		/// <param name="sender">The sender of the change notification.</param>
		/// <param name="e">The change details.</param>
		public void EhChildChanged(object sender, System.EventArgs e)
		{
			if (HandleImmediateChildChangeCases(sender, e))
				return;

			if (this.IsSuspended && sender is Main.ISuspendable)
			{
				_suspendedChildCollection.Add(sender); // add sender to suspended child
				((Main.ISuspendable)sender).Suspend();
				return;
			}

			AccumulateChildChangeData(sender, e);  // AccumulateNotificationData

			if (_isResumeInProgress || IsSuspended)
				return;

			if (_parentObject is Main.IChildChangedEventSink)
			{
				((Main.IChildChangedEventSink)_parentObject).EhChildChanged(this, _changeData);
				if (IsSuspended) // maybe parent has suspended us now
				{
					this.EhChildChanged(sender, e); // we call the function recursively, but now we are suspended
					return;
				}
			}

			OnChanged(); // Fire the changed event
		}

		protected virtual void OnChanged()
		{
			if (null != Changed)
				Changed(this, _changeData);

			_changeData = null;
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
		public static ProjectFolderPropertyBagCollection GetParentProjectFolderPropertyBagCollectionOf(Main.IDocumentNode child)
		{
			return (ProjectFolderPropertyBagCollection)Main.DocumentPath.GetRootNodeImplementing(child, typeof(ProjectFolderPropertyBagCollection));
		}

		#region IEnumerable<ProjectFolderPropertyBag> Members

		public IEnumerator<ProjectFolderPropertyBag> GetEnumerator()
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