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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Data
{
	/// <summary>
	/// Summary description for Altaxo.Data.DataTableCollection.
	/// </summary>
	public class DataTableCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		Main.IParentOfINameOwnerChildNodes,
		ICollection<DataTable>
	{
		// Data
		protected SortedDictionary<string, DataTable> _itemsByName = new SortedDictionary<string, DataTable>();

		/// <summary>
		/// Fired when one or more tables are added, deleted or renamed. Not fired when content in the table has changed.
		/// Arguments are the type of change, the item that changed, the old name (if renamed), and the new name (if renamed).
		/// This event can not be suspended.
		/// </summary>
		public event EventHandler<Main.NamedObjectCollectionChangedEventArgs> CollectionChanged;

		public DataTableCollection(AltaxoDocument parent)
		{
			this._parent = parent;
		}

		#region ICollection<DataTable> Members

		public void Clear()
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				var tables = _itemsByName.Values.ToArray();

				foreach (DataTable table in tables)
				{
					_itemsByName.Remove(table.Name);
					table.Dispose();
					this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(table));
				}

				_itemsByName.Clear(); // only for safety, should be done in call to Remove

				suspendToken.Resume();
			}
		}

		public bool Contains(DataTable item)
		{
			if (null == item)
				throw new ArgumentNullException("item");

			DataTable r;
			if (_itemsByName.TryGetValue(item.Name, out r))
				return object.ReferenceEquals(r, item);
			else
				return false;
		}

		public void CopyTo(DataTable[] array, int arrayIndex)
		{
			_itemsByName.Values.CopyTo(array, arrayIndex);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<DataTable>.Remove(DataTable item)
		{
			return Remove(item);
		}

		public int Count
		{
			get { return _itemsByName.Count; }
		}

		#endregion ICollection<DataTable> Members

		#region IEnumerable<DataTable> Members

		IEnumerator<DataTable> IEnumerable<DataTable>.GetEnumerator()
		{
			return _itemsByName.Values.GetEnumerator();
		}

		#endregion IEnumerable<DataTable> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _itemsByName.Values.GetEnumerator();
		}

		#endregion IEnumerable Members

		#region Suspend and resume

		protected override void OnChanged(EventArgs e)
		{
			if (null != CollectionChanged && (e is Main.NamedObjectCollectionChangedEventArgs))
			{
				CollectionChanged(this, (Main.NamedObjectCollectionChangedEventArgs)e);
			}

			base.OnChanged(e);
		}

		#endregion Suspend and resume

		public bool IsDirty
		{
			get
			{
				return _accumulatedEventData != null;
			}
		}

		public string[] GetSortedTableNames()
		{
			string[] arr = new string[_itemsByName.Count];
			this._itemsByName.Keys.CopyTo(arr, 0);
			return arr;
		}

		public Altaxo.Data.DataTable this[string name]
		{
			get
			{
				DataTable result;
				if (_itemsByName.TryGetValue(name, out result))
					return result;
				else
					throw new ArgumentOutOfRangeException(string.Format("The table \"{0}\" don't exist!", name));
			}
		}

		public bool Contains(string tablename)
		{
			return _itemsByName.ContainsKey(tablename);
		}

		public void Add(Altaxo.Data.DataTable theTable)
		{
			if (null == theTable)
				throw new ArgumentNullException("theTable");

			if (null == theTable.Name) // if no table name provided (an empty string is a valid table name)
				theTable.Name = FindNewTableName();                 // find a new one
			else if (_itemsByName.ContainsKey(theTable.Name)) // else if this table name is already in use
				theTable.Name = FindNewTableName(theTable.Name); // find a new table name based on the original name

			// now the table has a unique name in any case
			_itemsByName.Add(theTable.Name, theTable);
			theTable.ParentObject = this;

			// raise data event to all listeners
			this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(theTable));
		}

		/// <summary>
		/// Removes the table from the collection and disposes it. Only tables that belong to this collection will be removed and disposed.
		/// </summary>
		/// <param name="theTable">The table to remove and dispose.</param>
		/// <returns>True if the table was found in the collection and thus removed successfully.</returns>
		public bool Remove(DataTable theTable)
		{
			if (null == theTable)
				throw new ArgumentNullException("theTable");

			if (_itemsByName.Remove(theTable.Name))
			{
				var eventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(theTable);
				theTable.Dispose();
				EhSelfChanged(eventArgs);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Ensures the existence of a DataTable with the given name. Returns the table with the given name if it exists,
		/// otherwise a table with that name will be created and returned.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <returns>The data table with the provided name.</returns>
		public DataTable EnsureExistence(string tableName)
		{
			if (Contains(tableName))
			{
				return this[tableName];
			}
			else
			{
				var newTable = new DataTable(tableName);
				Add(newTable);
				return newTable;
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
					throw new ApplicationException("Names between DataTableCollection and Tables not in sync");

				_itemsByName.Remove(oldName);
				_itemsByName.Add(item.Name, (DataTable)item);

				EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
			}
			else
			{
				throw new ApplicationException("Error renaming table " + oldName + " : this table name was not found in the collection!");
			}
		}

		void Main.IParentOfINameOwnerChildNodes.EhChild_ParentChanged(Main.INameOwner childNode, Main.IDocumentNode oldParent)
		{
			if (object.ReferenceEquals(this, oldParent) && _itemsByName.ContainsKey(childNode.Name))
				throw new InvalidProgramException("Unauthorized change of the DataTable's parent");
		}

		/// <summary>
		/// Looks for the next free standard table name in the root folder.
		/// </summary>
		/// <returns>A new table name unique for this data set.</returns>
		public string FindNewTableName()
		{
			return FindNewTableNameInFolder(null);
		}

		/// <summary>
		/// Looks for the next free standard table name in the specified folder.
		/// </summary>
		/// <param name="folder">The folder where to find a unique table name.</param>
		/// <returns></returns>
		public string FindNewTableNameInFolder(string folder)
		{
			return FindNewTableName(Main.ProjectFolder.Combine(folder, "WKS"));
		}

		/// <summary>
		/// Looks for the next unique table name base on a basic name.
		/// </summary>
		/// <returns>A new table name unique for this data set.</returns>
		public string FindNewTableName(string basicname)
		{
			for (int i = 0; ; i++)
			{
				if (!_itemsByName.ContainsKey(basicname + i))
					return basicname + i;
			}
		}

		public override Main.IDocumentLeafNode GetChildObjectNamed(string name)
		{
			DataTable result;
			if (_itemsByName.TryGetValue(name, out result))
				return result;

			return null;
		}

		public override string GetNameOfChildObject(Main.IDocumentLeafNode o)
		{
			if (o is DataTable)
			{
				DataTable gr = (DataTable)o;
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

		/// <summary>
		/// Gets the parent DataTableCollection of a child table, a child ColumnCollection, or a child column.
		/// </summary>
		/// <param name="child">Can be a DataTable, a DataColumnCollection, or a DataColumn for which the parent table collection is searched.</param>
		/// <returns>The parent DataTableCollection, if it exists, or null otherwise.</returns>
		public static Altaxo.Data.DataTableCollection GetParentDataTableCollectionOf(Main.IDocumentLeafNode child)
		{
			return (DataTableCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(DataTableCollection));
		}
	}
}