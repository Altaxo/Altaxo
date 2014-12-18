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

namespace Altaxo.Data
{
	/// <summary>
	/// Summary description for Altaxo.Data.DataTableCollection.
	/// </summary>
	[SerializationSurrogate(0, typeof(Altaxo.Data.DataTableCollection.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataTableCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		System.Runtime.Serialization.IDeserializationCallback,
		ICollection<DataTable>,
		Main.INamedObjectCollection
	{
		// Data
		protected SortedDictionary<string, DataTable> _tablesByName = new SortedDictionary<string, DataTable>();

		/// <summary>
		/// Fired when one or more tables are added, deleted or renamed. Not fired when content in the table has changed.
		/// Arguments are the type of change, the item that changed, the old name (if renamed), and the new name (if renamed).
		/// This event can not be suspended.
		/// </summary>
		public event EventHandler<Main.NamedObjectCollectionChangedEventArgs> CollectionChanged;

		[NonSerialized()]
		private bool _isDeserializationFinished = false;

		public DataTableCollection(AltaxoDocument parent)
		{
			this._parent = parent;
		}

		public override string Name
		{
			get { return "Tables"; }
			set
			{
				throw new InvalidOperationException("The name is fixed and cannot be set");
			}
		}

		#region Serialization

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				Altaxo.Data.DataTableCollection s = (Altaxo.Data.DataTableCollection)obj;
				// info.AddValue("Parent",s._parent);
				info.AddValue("Tables", s._tablesByName);
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataTableCollection s = (Altaxo.Data.DataTableCollection)obj;
				// s._parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
				s._tablesByName = (SortedDictionary<string, DataTable>)(info.GetValue("Tables", typeof(SortedDictionary<string, DataTable>)));

				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if (!_isDeserializationFinished && obj is DeserializationFinisher) // if deserialization has completely finished now
			{
				_isDeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				// set the _parent object for the data tables
				foreach (DataTable dt in _tablesByName.Values)
				{
					dt.ParentObject = this;
					dt.OnDeserialization(finisher);
				}
			}
		}

		#endregion Serialization

		#region ICollection<DataTable> Members

		public void Clear()
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				foreach (DataTable table in _tablesByName.Values)
				{
					Detach(table);
					this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(table));
				}
				_tablesByName.Clear();

				suspendToken.Resume();
			}
		}

		public bool Contains(DataTable item)
		{
			if (null == item)
				throw new ArgumentNullException("item");

			DataTable r;
			if (_tablesByName.TryGetValue(item.Name, out r))
				return object.ReferenceEquals(r, item);
			else
				return false;
		}

		public void CopyTo(DataTable[] array, int arrayIndex)
		{
			_tablesByName.Values.CopyTo(array, arrayIndex);
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
			get { return _tablesByName.Count; }
		}

		#endregion ICollection<DataTable> Members

		#region IEnumerable<DataTable> Members

		IEnumerator<DataTable> IEnumerable<DataTable>.GetEnumerator()
		{
			return _tablesByName.Values.GetEnumerator();
		}

		#endregion IEnumerable<DataTable> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _tablesByName.Values.GetEnumerator();
		}

		#endregion IEnumerable Members

		#region Suspend and resume

		protected override void OnChanged(EventArgs e)
		{
			if (e is Main.NamedObjectCollectionChangedEventArgs && null != CollectionChanged)
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
			string[] arr = new string[_tablesByName.Count];
			this._tablesByName.Keys.CopyTo(arr, 0);
			// System.Array.Sort(arr);
			return arr;
		}

		public Altaxo.Data.DataTable this[string name]
		{
			get
			{
				DataTable result;
				if (_tablesByName.TryGetValue(name, out result))
					return result;
				else
					throw new ArgumentOutOfRangeException(string.Format("The table \"{0}\" don't exist!", name));
			}
		}

		public bool Contains(string tablename)
		{
			return _tablesByName.ContainsKey(tablename);
		}

		public void Add(Altaxo.Data.DataTable theTable)
		{
			if (null == theTable.Name || 0 == theTable.Name.Length) // if no table name provided
				theTable.Name = FindNewTableName();                 // find a new one
			else if (_tablesByName.ContainsKey(theTable.Name)) // else if this table name is already in use
				theTable.Name = FindNewTableName(theTable.Name); // find a new table name based on the original name

			// now the table has a unique name in any case
			_tablesByName.Add(theTable.Name, theTable);
			Attach(theTable);

			// raise data event to all listeners
			this.EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemAdded(theTable));
		}

		/// <summary>
		/// Attaches the table to this object but not adds it to the collection. You should do this as soon as possible.
		/// </summary>
		/// <param name="theTable"></param>
		private void Attach(DataTable theTable)
		{
			theTable.ParentObject = this;
			theTable.NameChanged += this.EhItemNameChanged;
			theTable.PreviewNameChange += this.EhPreviewItemNameChange;
			theTable.ParentChanged += this.EhTableParentChanged;
		}

		/// <summary>
		/// Detaches the table but not removes it from the collection. You should remove the table as soon as possible.
		/// </summary>
		/// <param name="theTable"></param>
		private void Detach(DataTable theTable)
		{
			theTable.ParentChanged -= this.EhTableParentChanged;
			theTable.NameChanged -= this.EhItemNameChanged;
			theTable.PreviewNameChange -= this.EhPreviewItemNameChange;
			theTable.ParentObject = null;
		}

		/// <summary>
		/// Removes the table from the collection and disposes it. Only tables that belong to this collection will be removed and disposed.
		/// </summary>
		/// <param name="theTable">The table to remove and dispose.</param>
		/// <returns>True if the table was found in the collection and thus removed successfully.</returns>
		public bool Remove(DataTable theTable)
		{
			if (!_tablesByName.ContainsValue(theTable))
				return false;

			var eventArgs = Main.NamedObjectCollectionChangedEventArgs.FromItemRemoved(theTable);

			_tablesByName.Remove(theTable.Name);
			Detach(theTable);
			theTable.Dispose();

			//OnCollectionChanged(Main.NamedObjectCollectionChangeType.ItemRemoved, theTable, theTable.Name);
			EhSelfChanged(eventArgs);
			return true;
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
				return this[tableName];
			else
			{
				var newTable = new DataTable(tableName);
				Add(newTable);
				return newTable;
			}
		}

		protected void EhTableParentChanged(object sender, Main.ParentChangedEventArgs pce)
		{
			if (object.ReferenceEquals(this, pce.OldParent) && this.Contains((DataTable)sender))
				this.Remove((DataTable)sender);
			else
				if (!this.Contains((DataTable)sender))
					throw new ApplicationException("Not allowed to set child's parent to this collection before adding it to the collection");
		}

		protected void EhPreviewItemNameChange(Main.INameOwner item, string newName, System.ComponentModel.CancelEventArgs e)
		{
			if (_tablesByName.ContainsKey(newName) && !object.ReferenceEquals(_tablesByName[newName], item))
				e.Cancel = true;
		}

		protected void EhItemNameChanged(Main.INameOwner item, string oldName)
		{
			if (_tablesByName.ContainsKey(item.Name))
			{
				if (object.ReferenceEquals(_tablesByName[item.Name], item))
					return; // Table alredy renamed
				else
					throw new ApplicationException("Table with name " + item.Name + " already exists!");
			}

			if (_tablesByName.ContainsKey(oldName))
			{
				if (!object.ReferenceEquals(_tablesByName[oldName], item))
					throw new ApplicationException("Names between DataTableCollection and Tables not in sync");

				_tablesByName.Remove(oldName);
				_tablesByName.Add(item.Name, (DataTable)item);

				EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(item, oldName));
			}
			else
			{
				throw new ApplicationException("Error renaming table " + oldName + " : this table name was not found in the collection!");
			}
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
				if (!_tablesByName.ContainsKey(basicname + i))
					return basicname + i;
			}
		}

		public object GetChildObjectNamed(string name)
		{
			DataTable result;
			if (_tablesByName.TryGetValue(name, out result))
				return result;

			return null;
		}

		public string GetNameOfChildObject(object o)
		{
			if (o is DataTable)
			{
				DataTable gr = (DataTable)o;
				if (_tablesByName.ContainsKey(gr.Name))
					return gr.Name;
			}
			return null;
		}

		/// <summary>
		/// Gets the parent DataTableCollection of a child table, a child ColumnCollection, or a child column.
		/// </summary>
		/// <param name="child">Can be a DataTable, a DataColumnCollection, or a DataColumn for which the parent table collection is searched.</param>
		/// <returns>The parent DataTableCollection, if it exists, or null otherwise.</returns>
		public static Altaxo.Data.DataTableCollection GetParentDataTableCollectionOf(Main.IDocumentLeafNode child)
		{
			return (DataTableCollection)Main.DocumentPath.GetRootNodeImplementing(child, typeof(DataTableCollection));
		}
	}
}