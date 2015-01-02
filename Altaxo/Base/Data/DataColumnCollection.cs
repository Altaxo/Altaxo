#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Scripting;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;

namespace Altaxo.Data
{
	[SerializationSurrogate(0, typeof(Altaxo.Data.DataColumnCollection.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DataColumnCollection :
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<DataColumnCollectionChangedEventArgs>,
		IList<DataRow>,
		System.Runtime.Serialization.IDeserializationCallback,
		IDisposable,
		ICloneable
	{
		// Types
		//  public delegate void OnDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow);   // delegate declaration
		//  public delegate void OnDirtySet(Altaxo.Data.DataColumnCollection sender);

		#region ColumnInfo

		[Serializable]
		protected class DataColumnInfo : ICloneable
		{
			/// <summary>
			/// The column number, i.e. it's position in the array
			/// </summary>
			public int Number;

			/// <summary>
			/// The name of the column.
			/// </summary>
			public string Name;

			/// <summary>
			/// The kind of the column.
			/// </summary>
			public ColumnKind Kind;

			/// <summary>
			/// The group this column belongs to.
			/// </summary>
			public int Group;

			/// <summary>
			/// Constructs a DataColumnInfo object.
			/// </summary>
			/// <param name="name">The name of the column.</param>
			/// <param name="number">The position of the column in the collection.</param>
			public DataColumnInfo(string name, int number)
			{
				this.Name = name;
				this.Number = number;
			}

			/// <summary>
			/// Constructs a DataColumnInfo object.
			/// </summary>
			/// <param name="name">The name of the column.</param>
			public DataColumnInfo(string name)
			{
				this.Name = name;
			}

			/// <summary>
			/// Constructs a DataColumnInfo object.
			/// </summary>
			/// <param name="name">The name of the column.</param>
			/// <param name="kind">The kind of the column.</param>
			public DataColumnInfo(string name, ColumnKind kind)
			{
				this.Name = name;
				this.Kind = kind;
			}

			/// <summary>
			/// Constructs a DataColumnInfo object.
			/// </summary>
			/// <param name="name">The name of the column.</param>
			/// <param name="kind">The kind of the column.</param>
			/// <param name="groupNumber">The group number of the column.</param>
			public DataColumnInfo(string name, ColumnKind kind, int groupNumber)
			{
				this.Name = name;
				this.Kind = kind;
				this.Group = groupNumber;
			}

			/// <summary>
			/// Copy constructor.
			/// </summary>
			/// <param name="from">Another object to copy from.</param>
			public DataColumnInfo(DataColumnInfo from)
			{
				this.Name = from.Name;
				this.Number = from.Number;
				this.Group = from.Group;
				this.Kind = from.Kind;
			}

			public bool IsIndependentVariable
			{
				get { return Kind == ColumnKind.X || Kind == ColumnKind.Y || Kind == ColumnKind.Z; }
			}

			#region ICloneable Members

			/// <summary>
			/// Clones the object.
			/// </summary>
			/// <returns>The cloned object.</returns>
			public object Clone()
			{
				return new DataColumnInfo(this);
			}

			#endregion ICloneable Members
		}

		#endregion ColumnInfo

		#region Member data

		/// <summary>
		/// The collection of the columns (accessible by number).
		/// </summary>
		protected List<DataColumn> _columnsByNumber = new List<DataColumn>();

		/// <summary>
		/// Holds the columns (value) accessible by name (key).
		/// </summary>
		protected Dictionary<string, DataColumn> _columnsByName = new Dictionary<string, DataColumn>();

		/// <summary>
		/// This hashtable has the <see cref="DataColumn" /> as keys and <see cref="DataColumnInfo" /> objects as values.
		/// It stores information like the position of the column, the kind of the column.
		/// </summary>
		protected Dictionary<DataColumn, DataColumnInfo> _columnInfoByColumn = new Dictionary<DataColumn, DataColumnInfo>();

		/// <summary>
		/// Cached number of rows. This is the maximum of the Count of all DataColumns contained in this collection.
		/// </summary>
		protected int _numberOfRows = 0; // the max. Number of Rows of the columns of the table

		/// <summary>
		/// Indicates that the cached row count is no longer valid and can be lesser than the actual value in m_NumberOfRows
		/// (but not greater).
		/// </summary>
		protected bool _hasNumberOfRowsDecreased = false;

		/// <summary>
		/// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
		/// </summary>
		protected Dictionary<DataColumn, IColumnScriptText> _columnScripts = new Dictionary<DataColumn, IColumnScriptText>();

		/// <summary>
		/// Name of the last column added to this collection.
		/// </summary>
		protected string _nameOfLastColumnAdded = ""; // name of the last column wich was added to the table

		/// <summary>
		/// The last column name autogenerated by this class.
		/// </summary>
		protected string _lastColumnNameGenerated = ""; // name of the last column name that was automatically generated

		/// <summary>
		/// If true, we recently have tested the column names A-ZZ and all column names were in use.
		/// This flag should be resetted if a column deletion or renaming operation took place.
		/// </summary>
		protected bool _triedOutRegularNaming = false;

		/// <summary>
		/// Signals the change of the parent of the collection.
		/// </summary>
		[field: NonSerialized]
		public event Main.ParentChangedEventHandler ParentChanged;

		/// <summary>
		/// Flag to signal if deserialization is finished.
		/// </summary>
		private bool _isDeserializationFinished = false;

		#endregion Member data

		#region Serialization

		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

				info.AddValue("Columns", s._columnsByNumber);
				// serialize the column scripts
				info.AddValue("ColumnScripts", s._columnScripts);
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

				s._columnsByNumber = (List<DataColumn>)(info.GetValue("Columns", typeof(List<DataColumn>)));
				s._columnScripts = (Dictionary<DataColumn, IColumnScriptText>)(info.GetValue("ColumnScripts", typeof(Dictionary<DataColumn, IColumnScriptText>)));

				// set up helper objects
				s._columnsByName = new Dictionary<string, DataColumn>();
				s._nameOfLastColumnAdded = "";
				s._lastColumnNameGenerated = "";
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DataColumnCollection), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Altaxo.Data.DataColumnCollection s = (Altaxo.Data.DataColumnCollection)obj;

				info.CreateArray("ColumnArray", s._columnsByNumber.Count);
				for (int i = 0; i < s._columnsByNumber.Count; i++)
				{
					info.CreateElement("Column");

					DataColumnInfo colinfo = s.GetColumnInfo(i);
					info.AddValue("Name", colinfo.Name);
					info.AddValue("Kind", (int)colinfo.Kind);
					info.AddValue("Group", colinfo.Group);
					info.AddValue("Data", s._columnsByNumber[i]);

					info.CommitElement();
				}
				info.CommitArray();

				// serialize the column scripts
				info.CreateArray("ColumnScripts", s._columnScripts.Count);
				foreach (KeyValuePair<DataColumn, IColumnScriptText> entry in s._columnScripts)
				{
					info.CreateElement("Script");
					info.AddValue("ColName", s.GetColumnName(entry.Key));
					info.AddValue("Content", entry.Value);
					info.CommitElement();
				}
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				Altaxo.Data.DataColumnCollection s = null != o ? (Altaxo.Data.DataColumnCollection)o : new Altaxo.Data.DataColumnCollection();

				// deserialize the columns
				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					info.OpenElement(); // Column

					string name = info.GetString("Name");
					ColumnKind kind = (ColumnKind)info.GetInt32("Kind");
					int group = info.GetInt32("Group");
					var col = (DataColumn)info.GetValue("Data", s);
					if (col != null)
						s.Add(col, new DataColumnInfo(name, kind, group));

					info.CloseElement(); // Column
				}
				info.CloseArray(count);

				// deserialize the scripts
				count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					info.OpenElement();
					string name = info.GetString();
					IColumnScriptText script = (IColumnScriptText)info.GetValue(s);
					info.CloseElement();
					s.ColumnScripts.Add(s[name], script);
				}
				info.CloseArray(count); // end script array
				return s;
			}
		}

		/// <summary>
		/// Final measures for the object after deserialization.
		/// </summary>
		/// <param name="obj">The object for which the deserialization has finished.</param>
		public virtual void OnDeserialization(object obj)
		{
			if (!_isDeserializationFinished && obj is DeserializationFinisher)
			{
				_isDeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);

				// 1. Set the parent Data table of the columns,
				// because they may be feel lonesome
				int nCols = _columnsByNumber.Count;
				_numberOfRows = 0;
				DataColumn dc;
				for (int i = 0; i < nCols; i++)
				{
					dc = (DataColumn)_columnsByNumber[i];
					dc.ParentObject = this;
					dc.OnDeserialization(finisher);

					// add it also to the column name cache
					_columnsByName.Add(dc.Name, dc);

					// count the maximumn number of rows
					if (dc.Count > _numberOfRows)
						_numberOfRows = dc.Count;
				}
			}
		}

		/// <summary>
		/// This class is responsible for the special purpose to serialize a data table for clipboard. Do not use
		/// it for permanent serialization purposes, since it does not contain version handling.
		/// </summary>
		[Serializable]
		public class ClipboardMemento : System.Runtime.Serialization.ISerializable
		{
			private DataColumnCollection _collection;
			private IAscendingIntegerCollection _selectedColumns;
			private IAscendingIntegerCollection _selectedRows;
			private bool _useOnlySelections;

			/// <summary>
			/// Constructor. Besides the table, the current selections must be provided. Only the areas that corresponds to the selections are
			/// serialized. The serialization process has to occur immediately after this constructor, because only a reference
			/// to the table is hold by this object.
			/// </summary>
			/// <param name="collection">The collection to serialize.</param>
			/// <param name="selectedColumns">The selected data columns.</param>
			/// <param name="selectedRows">The selected data rows.</param>
			/// <param name="useOnlySelections">If true, only the selections are serialized. If false and there is no selection, the whole collection is serialized.</param>
			public ClipboardMemento(
				DataColumnCollection collection,
				IAscendingIntegerCollection selectedColumns,
				IAscendingIntegerCollection selectedRows,
				bool useOnlySelections)
			{
				this._collection = collection;
				this._selectedColumns = selectedColumns;
				this._selectedRows = selectedRows;
				this._useOnlySelections = useOnlySelections;
			}

			/// <summary>
			/// Returns the (deserialized) DataColumnCollection.
			/// </summary>
			public DataColumnCollection Collection
			{
				get { return _collection; }
			}

			#region ISerializable Members

			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				int numberOfColumns;
				bool useColumnSelection;

				// special case - no selection
				if (_selectedColumns.Count == 0 && _selectedRows.Count == 0 && _useOnlySelections)
				{
					numberOfColumns = 0;
					useColumnSelection = false;
				}
				else if (_selectedColumns.Count == 0)
				{
					numberOfColumns = _collection.ColumnCount;
					useColumnSelection = false;
				}
				else
				{
					numberOfColumns = _selectedColumns.Count;
					useColumnSelection = true;
				}

				int numberOfRows = 0;
				bool useRowSelection;
				if (_selectedRows.Count == 0)
				{
					numberOfRows = _collection.RowCount;
					useRowSelection = false;
				}
				else
				{
					numberOfRows = _selectedRows.Count;
					useRowSelection = true;
				}

				info.AddValue("ColumnCount", numberOfColumns);

				for (int nCol = 0; nCol < numberOfColumns; nCol++)
				{
					int colidx = useColumnSelection ? _selectedColumns[nCol] : nCol;
					DataColumnInfo columninfo = _collection.GetColumnInfo(colidx);
					info.AddValue("ColumnName_" + nCol.ToString(), columninfo.Name);
					info.AddValue("ColumnGroup_" + nCol.ToString(), columninfo.Group);
					info.AddValue("ColumnKind_" + nCol.ToString(), columninfo.Kind);

					// now create an instance of this column and copy the data
					DataColumn column = _collection[colidx];
					DataColumn newcolumn = (DataColumn)Activator.CreateInstance(_collection[colidx].GetType());
					for (int nRow = 0; nRow < numberOfRows; nRow++)
					{
						int rowidx = useRowSelection ? _selectedRows[nRow] : nRow;
						newcolumn[nRow] = column[rowidx];
					} // for all rows
					info.AddValue("Column_" + nCol.ToString(), newcolumn);
				} // for all columns
			} // end serialization

			public ClipboardMemento(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				_collection = new DataColumnCollection();

				int numberOfColumns = info.GetInt32("ColumnCount");
				for (int nCol = 0; nCol < numberOfColumns; nCol++)
				{
					string name = info.GetString("ColumnName_" + nCol.ToString());
					int group = info.GetInt32("ColumnGroup_" + nCol.ToString());
					ColumnKind kind = (ColumnKind)info.GetValue("ColumnKind_" + nCol.ToString(), typeof(ColumnKind));

					DataColumn column = (DataColumn)info.GetValue("Column_" + nCol.ToString(), typeof(DataColumn));

					_collection.Add(column, name, kind, group);
				}
			}

			#endregion ISerializable Members
		}

		#endregion Serialization

		#region Constructors

		/// <summary>
		/// Constructs an empty collection with no parent.
		/// </summary>
		public DataColumnCollection()
		{
			this._parent = null;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The column collection to copy this data column collection from.</param>
		public DataColumnCollection(DataColumnCollection from)
		{
			this._nameOfLastColumnAdded = from._nameOfLastColumnAdded;
			this._lastColumnNameGenerated = from._lastColumnNameGenerated;

			// Copy all Columns
			for (int i = 0; i < from.ColumnCount; i++)
			{
				DataColumn newCol = (DataColumn)from[i].Clone();
				DataColumnInfo newInfo = (DataColumnInfo)from.GetColumnInfo(from[i]).Clone();
				this.Add(newCol, newInfo);
			}
			// Copy all Column scripts
			foreach (KeyValuePair<DataColumn, IColumnScriptText> d in from.ColumnScripts)
			{
				DataColumn srccol = d.Key; // the original column this script belongs to
				DataColumn destcol = this[srccol.Name]; // the new (cloned) column the script now belongs to
				var destscript = (IColumnScriptText)d.Value.Clone(); // the cloned script

				// add the cloned script to the own collection
				this.ColumnScripts.Add(destcol, destscript);
			}
		}

		/// <summary>
		/// Clones the collection and all columns in it (deep copy).
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			return new DataColumnCollection(this);
		}

		/// <summary>
		/// Disposes the collection and all columns in it.
		/// </summary>
		protected override void Dispose(bool isDisposing)
		{
			if (null != _parent)
			{
				// first relase all column scripts
				foreach (KeyValuePair<DataColumn, IColumnScriptText> d in this._columnScripts)
				{
					if (d.Value is IDisposable)
						((IDisposable)d.Value).Dispose();
				}
				_columnScripts.Clear();

				// release all owned Data columns
				this._columnsByName.Clear();

				for (int i = _columnsByNumber.Count - 1; i >= 0; --i) // iterate downwards, because the dispose action can trigger the column to be removed from the collection
					this[i].Dispose();

				_columnsByNumber.Clear();
				this._numberOfRows = 0;

				base.Dispose(isDisposing);
			}
		}

		#endregion Constructors

		#region Add / Replace Column

		/// <summary>
		/// Gets the data columns as an enumeration.
		/// </summary>
		/// <value>
		/// The data columns as Enumeration.
		/// </value>
		public IEnumerable<Altaxo.Data.DataColumn> Columns
		{
			get
			{
				foreach (var c in _columnsByNumber)
					yield return c;
			}
		}

		/// <summary>
		/// Adds a column by choosing a new unused name for that column automatically.
		/// </summary>
		/// <param name="datac"></param>
		public void Add(Altaxo.Data.DataColumn datac)
		{
			Add(datac, datac.Name);
		}

		/// <summary>
		/// Add a column under the name <code>name</code>.
		/// </summary>
		/// <param name="datac">The column to add.</param>
		/// <param name="name">The name under which the column to add.</param>
		public void Add(Altaxo.Data.DataColumn datac, string name)
		{
			Add(datac, name, ColumnKind.V);
		}

		/// <summary>
		/// Adds a column with a given name and kind.
		/// </summary>
		/// <param name="datac">The column to add.</param>
		/// <param name="name">The name of the column.</param>
		/// <param name="kind">The kind of the column.</param>
		public void Add(Altaxo.Data.DataColumn datac, string name, ColumnKind kind)
		{
			Add(datac, name, kind, 0);
		}

		/// <summary>
		/// Add a column with a given name, kind, and group number.
		/// </summary>
		/// <param name="datac">The column to add.</param>
		/// <param name="name">The name of the column.</param>
		/// <param name="kind">The kind of the column.</param>
		/// <param name="groupNumber">The group number of the column.</param>
		public void Add(Altaxo.Data.DataColumn datac, string name, ColumnKind kind, int groupNumber)
		{
			if (name == null)
				name = this.FindNewColumnName();
			else if (_columnsByName.ContainsKey(name))
				name = this.FindUniqueColumnName(name);

			Add(datac, new DataColumnInfo(name, kind, groupNumber));
		}

		/// <summary>
		/// Either returns an existing column with name <c>columnname</c> and type <c>expectedtype</c> or creates a new column of the provided type.
		/// </summary>
		/// <param name="columnname">The column name.</param>
		/// <param name="expectedcolumntype">Expected type of the column.</param>
		/// <param name="kind">Only used when a new column is created.</param>
		/// <param name="groupNumber">Only used when a new column is created.</param>
		/// <returns>Either an existing column of name <c>columnname</c> and type <c>expectedtype</c>. If this is
		/// not the case, a new column with a similar name and the <c>expectedtype</c> is created and added to the collection. The new column is returned then.</returns>
		public DataColumn EnsureExistence(string columnname, System.Type expectedcolumntype, ColumnKind kind, int groupNumber)
		{
			if (IsColumnOfType(columnname, expectedcolumntype))
				return this[columnname];

			object o = System.Activator.CreateInstance(expectedcolumntype);
			if (!(o is DataColumn))
				throw new ApplicationException("The type you provided is not compatible with DataColumn, provided type: " + expectedcolumntype.GetType().ToString());

			Add((DataColumn)o, columnname, kind, groupNumber);
			return (DataColumn)o;
		}

		/// <summary>
		/// Ensures the existence of a column with exactly the provided properties at the provided position.
		/// </summary>
		/// <param name="columnNumber">The column number. Have to be in the range (0..ColumnCount). If the value is ColumnCount, a new column is added.</param>
		/// <param name="columnName">Name of the column. If another column with the same name exists, the existing column with the same name will be renamed (if the existing column has a higher column number).
		/// If the existing column with the same name has a lower column number, an exception is thrown.</param>
		/// <param name="expectedColumnType">Expected type of the column. If a column with the provided type exists at the provided position, this column is used. If the column at the provided position
		/// is of a different type, a new column with the provided type is created, and is then used to replace the column at the provided position.</param>
		/// <param name="columnKind">Kind of the column.</param>
		/// <param name="groupNumber">The group number of the column.</param>
		/// <returns>A column with exactly the provided properties at exactly the provided position.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// If columnNumber is either less than 0 or greater than <see cref="ColumnCount"/>
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// If the provided type is not a subclass of <see cref="DataColumn"/> or is an abstract type.
		/// or
		/// A column with the same name already exists to the left of the provided position.
		/// </exception>
		public DataColumn EnsureExistenceAtPositionStrictly(int columnNumber, string columnName, System.Type expectedColumnType, ColumnKind columnKind, int groupNumber)
		{
			if (columnNumber < 0)
				throw new ArgumentOutOfRangeException("columnNumber must not be < 0");
			if (columnNumber > ColumnCount)
				throw new ArgumentOutOfRangeException("columnNumber must not be > ColumnCount");

			if (columnNumber == ColumnCount) // create a new column
			{
				object o = System.Activator.CreateInstance(expectedColumnType);
				if (!(o is DataColumn))
					throw new InvalidOperationException("The type you provided is not compatible with DataColumn, provided type: " + expectedColumnType.GetType().ToString());

				Add((DataColumn)o, columnName, columnKind, groupNumber);
			}

			// now we can expect that the column always exist at this position
			var col = this[columnNumber];

			// first test if we have the appropriate type
			if (!(col.GetType() == expectedColumnType || col.GetType().IsSubclassOf(expectedColumnType)))
			{
				// then replace the column at this position with the right type
				object o = System.Activator.CreateInstance(expectedColumnType);
				if (!(o is DataColumn))
					throw new InvalidOperationException("The type you provided is not compatible with DataColumn, provided type: " + expectedColumnType.GetType().ToString());

				col = (DataColumn)o;
				this.Replace(columnNumber, col);
			}

			// now we can be sure that we have the right column type
			// next task is to set the name
			// by convention it if the name already exist to the left of the new column, we throw an exception (because we assume that we build the table from left to right)
			// else if the name already exist to the right of the column, we rename the right column

			if (GetColumnName(columnNumber) != columnName)
			{
				if (!this.ContainsColumn(columnName)) // Fine, the name doesn't exist, thus we can set it straightforward
				{
					SetColumnName(columnNumber, columnName);
				}
				else // ColumnName exists already
				{
					int otherColumnNumber = GetColumnNumber(this[columnName]);
					if (otherColumnNumber < columnNumber)
						throw new InvalidOperationException("A column with the same name already exists to the left of the current column.");
					// Create an arbitrary name
					string newName = FindUniqueColumnName(columnName);
					SetColumnName(otherColumnNumber, newName);
					SetColumnName(columnNumber, columnName);
				}
			}

			// Set group number and kind
			SetColumnGroup(columnNumber, groupNumber);
			SetColumnKind(columnNumber, columnKind);

			return col;
		}

		/// <summary>
		/// Ensures the existence of a column with exactly the provided properties at the provided position.
		/// </summary>
		/// <typeparam name="TDataCol">The type of the data column. Has to be a type derived from <see cref="DataColumn"/>.</typeparam>
		/// <param name="columnNumber">The column number. Have to be in the range (0..ColumnCount). If the value is ColumnCount, a new column is added.</param>
		/// <param name="columnName">Name of the column. If another column with the same name exists, the existing column with the same name will be renamed (if the existing column has a higher column number).
		/// If the existing column with the same name has a lower column number, an exception is thrown.</param>
		/// <param name="columnKind">Kind of the column.</param>
		/// <param name="groupNumber">The group number of the column.</param>
		/// <returns>A column with exactly the provided properties at exactly the provided position.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// If columnNumber is either less than 0 or greater than <see cref="ColumnCount"/>
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// If the provided type is not a subclass of <see cref="DataColumn"/> or is an abstract type.
		/// or
		/// A column with the same name already exists to the left of the provided position.
		/// </exception>
		public TDataCol EnsureExistenceAtPositionStrictly<TDataCol>(int columnNumber, string columnName, ColumnKind columnKind, int groupNumber) where TDataCol : DataColumn
		{
			return (TDataCol)EnsureExistenceAtPositionStrictly(columnNumber, columnName, typeof(TDataCol), columnKind, groupNumber);
		}

		/// <summary>
		/// Add a column using a DataColumnInfo object. The provided info must not be used elsewhere, since it is used directly.
		/// </summary>
		/// <param name="datac">The column to add.</param>
		/// <param name="info">The DataColumnInfo object for the column to add.</param>
		private void Add(Altaxo.Data.DataColumn datac, DataColumnInfo info)
		{
			System.Diagnostics.Debug.Assert(this.ContainsColumn(datac) == false);
			System.Diagnostics.Debug.Assert(datac.ParentObject == null, "This private function should be only called with fresh DataColumns, if not, alter the behaviour of the calling function");
			System.Diagnostics.Debug.Assert(false == this._columnsByName.ContainsKey(info.Name), "Trying to add a column with a name that is already present (this error must be fixed in the calling function)");

			info.Number = this._columnsByNumber.Count;

			this._columnsByNumber.Add(datac);
			this._columnsByName[info.Name] = datac;
			this._columnInfoByColumn[datac] = info;
			datac.ParentObject = this;

			if (info.IsIndependentVariable)
				this.EnsureUniqueColumnKindsForIndependentVariables(info.Group, datac);

			this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnAddArgs(info.Number, datac.Count));
		}

		/// <summary>
		/// Copies the data of the column (columns have same type, index is inside bounds), or replaces
		/// the column (columns of different types, index inside bounds), or adds the column (index outside bounds).
		/// </summary>
		/// <param name="index">The column position where to replace or add.</param>
		/// <param name="datac">The column from which the data should be copied or which should replace the existing column or which should be added.</param>
		/// <param name="name">The name under which the column should be stored.</param>
		public void CopyOrReplaceOrAdd(int index, DataColumn datac, string name)
		{
			if (index < ColumnCount)
			{
				if (this[index].GetType().Equals(datac.GetType()))
				{
					this[index].CopyDataFrom(datac);
				}
				else
				{
					// if the column to add has a parent, we can not add the column directly (we are then not the owner), so we clone it
					Replace(index, datac.ParentObject == null ? datac : (DataColumn)datac.Clone());
				}
			}
			else
			{
				// if the column to add has a parent, we can not add the column directly (we are then not the owner), so we clone it
				Add(datac.ParentObject == null ? datac : (DataColumn)datac.Clone(), name);
			}
		}

		/// <summary>
		/// Replace the column at index <code>index</code> (if index is inside bounds) or add the column.
		/// </summary>
		/// <param name="index">The position of the column which should be replaced.</param>
		/// <param name="datac">The column which replaces the existing column or which is be added to the collection.</param>
		public void ReplaceOrAdd(int index, DataColumn datac)
		{
			if (index < ColumnCount)
				Replace(index, datac);
			else
				Add(datac);
		}

		/// <summary>
		/// Replace an existing column at index <code>index</code> by a new column <code>newCol.</code>
		/// </summary>
		/// <param name="index">The position of the column which should be replaced.</param>
		/// <param name="newCol">The new column that replaces the old one.</param>
		public void Replace(int index, DataColumn newCol)
		{
			if (index >= ColumnCount)
				throw new System.IndexOutOfRangeException(string.Format("Index ({0})for replace operation was outside the bounds, the actual column count is {1}", index, ColumnCount));

			DataColumn oldCol = this[index];
			if (!oldCol.Equals(newCol))
			{
				int oldRowCount = oldCol.Count;
				DataColumnInfo info = GetColumnInfo(index);
				_columnsByName[info.Name] = newCol;
				_columnsByNumber[index] = newCol;
				_columnInfoByColumn.Remove(oldCol);
				_columnInfoByColumn.Add(newCol, info);
				oldCol.ParentObject = null;
				newCol.ParentObject = this;

				IColumnScriptText script;
				_columnScripts.TryGetValue(oldCol, out script);
				if (null != script)
				{
					_columnScripts.Remove(oldCol);
					_columnScripts.Add(newCol, script);
				}

				this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnCopyOrReplaceArgs(index, oldRowCount, newCol.Count));
			}
		}

		/// <summary>
		/// Inserts multiple DataColumns into the collection at index <c>nDestinationIndex</c>. The caller must garantuee, that the names are not already be present!
		/// </summary>
		/// <param name="columns">The array of data columns to insert.</param>
		/// <param name="info">The corresponding column information for the columns to insert.</param>
		/// <param name="nDestinationIndex">The index into the collection where the columns are inserted.</param>
		protected void Insert(DataColumn[] columns, DataColumnInfo[] info, int nDestinationIndex)
		{
			Insert(columns, info, nDestinationIndex, false);
		}

		/// <summary>
		/// Inserts multiple DataColumns into the collection at index <c>nDestinationIndex</c>. The caller must garantuee, that the names are not already be present,
		/// or the argument <c>renameColumnsIfNecessary</c> must be set to true!
		/// </summary>
		/// <param name="columns">The array of data columns to insert.</param>
		/// <param name="info">The corresponding column information for the columns to insert.</param>
		/// <param name="nDestinationIndex">The index into the collection where the columns are inserted.</param>
		/// <param name="renameColumnsIfNecessary">If set to true, the columns to insert are automatically renamed if already be present in the destination collection. If false,
		/// an exception is thrown if a column with the same name is already be present in the destination table.</param>
		protected void Insert(DataColumn[] columns, DataColumnInfo[] info, int nDestinationIndex, bool renameColumnsIfNecessary)
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				int indexOfAddedColumns = this.ColumnCount;
				int numberToAdd = columns.Length;

				// first add the columns to the collection
				for (int i = 0; i < numberToAdd; i++)
				{
					if (renameColumnsIfNecessary && this.ContainsColumn(info[i].Name))
						info[i].Name = this.FindUniqueColumnNameWithBase(info[i].Name);

					this.Add(columns[i], info[i]);
				}

				// then move the columns to the desired position
				this.ChangeColumnPosition(Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(indexOfAddedColumns, numberToAdd), nDestinationIndex);

				suspendToken.Dispose();
			}
		}

		/// <summary>
		/// Deletes all columns in the collection, and then copy all columns from the source table.
		/// </summary>
		/// <param name="src">The source collection to copy the columns from.</param>
		public void CopyAllColumnsFrom(DataColumnCollection src)
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				this.RemoveColumnsAll();
				for (int i = 0; i < src.ColumnCount; i++)
				{
					this.Add((DataColumn)src[i].Clone(), src.GetColumnName(i), src.GetColumnKind(i), src.GetColumnGroup(i));
				}
				suspendToken.Dispose();
			}
		}

		/// <summary>
		/// Deletes all columns in the collection, and then copy all columns (but without data) from the source table.
		/// </summary>
		/// <param name="src">The source collection to copy the columns from.</param>
		public void CopyAllColumnsWithoutDataFrom(DataColumnCollection src)
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				this.RemoveColumnsAll();
				for (int i = 0; i < src.ColumnCount; i++)
				{
					var newCol = (DataColumn)Activator.CreateInstance(src[i].GetType());
					this.Add(newCol, src.GetColumnName(i), src.GetColumnKind(i), src.GetColumnGroup(i));
				}
				suspendToken.Dispose();
			}
		}

		/// <summary>
		/// Appends data columns from DataTable src to the data in this table.
		/// </summary>
		/// <param name="src">Source table.</param>
		/// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
		/// the data columns in this table and in src table are compared by their name.</param>
		public void AppendAllColumns(DataColumnCollection src, bool ignoreNames)
		{
			AppendAllColumnsToPosition(src, ignoreNames, RowCount);
		}

		/// <summary>
		/// Appends data columns from DataTable src to the data in this table leaving some rows free inbetween.
		/// </summary>
		/// <param name="src">Source table.</param>
		/// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
		/// the data columns in this table and in src table are compared by their name.</param>
		/// <param name="rowSpace">Number of rows to leave free between data in this table and newly appended data.</param>
		public void AppendAllColumnsWithSpace(DataColumnCollection src, bool ignoreNames, int rowSpace)
		{
			AppendAllColumnsToPosition(src, ignoreNames, RowCount + rowSpace);
		}

		/// <summary>
		/// Appends data columns from DataTable src to the data in this table by copying the new data to a specified row.
		/// </summary>
		/// <param name="src">Source table.</param>
		/// <param name="ignoreNames">If true, the data columns in this table and in src table are compared by index. If false,
		/// the data columns in this table and in src table are compared by their name.</param>
		/// <param name="appendPosition">Row number of first row where the new data is copied to.</param>
		public void AppendAllColumnsToPosition(DataColumnCollection src, bool ignoreNames, int appendPosition)
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				// test structure
				if (!IsColumnStructureCompatible(this, src, ignoreNames))
					throw new ArgumentException(string.Format("DataColumnCollection {0} has another structure than {1}. Append is not possible", src.Name, this.Name));

				if (ignoreNames)
				{
					for (int i = 0; i < ColumnCount; i++)
					{
						DataColumn destCol = this[i];
						DataColumn srcCol = src[i];
						destCol.AppendToPosition(srcCol, appendPosition);
						_numberOfRows = Math.Max(_numberOfRows, destCol.Count);
					}
				}
				else
				{
					for (int i = 0; i < ColumnCount; i++)
					{
						DataColumn destCol = this[i];
						DataColumn srcCol = src[this[i].Name];
						destCol.AppendToPosition(srcCol, appendPosition);
						_numberOfRows = Math.Max(_numberOfRows, destCol.Count);
					}
				}
				suspendToken.Dispose();
			}
		}

		public static bool IsColumnStructureCompatible(DataColumnCollection a, DataColumnCollection b, bool ignoreColumnNames)
		{
			if (a.ColumnCount != b.ColumnCount)
				return false;

			if (ignoreColumnNames)
			{
				for (int i = 0; i < a.ColumnCount; i++)
				{
					if (a[i].GetType() != b[i].GetType())
						return false;
				}
			}
			else
			{
				for (int i = 0; i < a.ColumnCount; i++)
				{
					string ainame = a[i].Name;
					if (!b.ContainsColumn(ainame))
						return false;
					if (a[i].GetType() != b[ainame].GetType())
						return false;
				}
			}
			return true;
		}

		#endregion Add / Replace Column

		#region Column removal and move to another collection

		/// <summary>
		/// Removes a number of columns of the collection.
		/// </summary>
		/// <param name="nFirstColumn">The first number of column to remove.</param>
		/// <param name="nDelCount">The number of columns to remove.</param>
		public virtual void RemoveColumns(int nFirstColumn, int nDelCount)
		{
			RemoveColumns(ContiguousIntegerRange.FromStartAndCount(nFirstColumn, nDelCount));
		}

		/// <summary>
		/// Removes all columns of the collection.
		/// </summary>
		public virtual void RemoveColumnsAll()
		{
			RemoveColumns(ContiguousIntegerRange.FromStartAndCount(0, this.ColumnCount));
		}

		/// <summary>
		/// Removes the column at the given index.
		/// </summary>
		/// <param name="nFirstColumn">The index of the column to remove.</param>
		public void RemoveColumn(int nFirstColumn)
		{
			RemoveColumns(nFirstColumn, 1);
		}

		/// <summary>
		/// Remove the column given as the argument.
		/// </summary>
		/// <param name="datac">The column to remove.</param>
		public void RemoveColumn(DataColumn datac)
		{
			RemoveColumns(this.GetColumnNumber(datac), 1);
		}

		public void RemoveColumns(IAscendingIntegerCollection selectedColumns)
		{
			RemoveColumns(selectedColumns, true);
		}

		public void RemoveColumns(IAscendingIntegerCollection selectedColumns, bool disposeColumns)
		{
			int nOriginalColumnCount = ColumnCount;

			int lastRangeStart = 0;

			foreach (var range in selectedColumns.RangesDescending)
			{
				// first, Dispose the columns and set the places to null
				for (int i = range.LastInclusive; i >= range.Start; i--)
				{
					string columnName = GetColumnName(this[i]);
					this._columnScripts.Remove(this[i]);
					this._columnInfoByColumn.Remove(_columnsByNumber[i]);
					this._columnsByName.Remove(columnName);
					this[i].ParentObject = null;
					if (disposeColumns)
						this[i].Dispose();
				}
				this._columnsByNumber.RemoveRange(range.Start, range.Count);
				lastRangeStart = range.Start;
			}

			// renumber the remaining columns
			for (int i = _columnsByNumber.Count - 1; i >= lastRangeStart; i--)
				((DataColumnInfo)_columnInfoByColumn[_columnsByNumber[i]]).Number = i;

			// raise datachange event that some columns have changed
			this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnRemoveArgs(lastRangeStart, nOriginalColumnCount, this._numberOfRows));

			// reset the TriedOutRegularNaming flag, maybe one of the regular column names is now free again
			this._triedOutRegularNaming = false;
		}

		/// <summary>
		/// Moves some columns of this collection to another collection.
		/// </summary>
		/// <param name="destination">The destination collection where the columns are moved to.</param>
		/// <param name="destindex">The index in the destination collection where the columns are moved to.</param>
		/// <param name="selectedColumns">The indices of the column of the source collection that are moved.</param>
		public void MoveColumnsTo(DataColumnCollection destination, int destindex, IAscendingIntegerCollection selectedColumns)
		{
			int nOriginalColumnCount = ColumnCount;

			int numberMoved = selectedColumns.Count;

			DataColumn[] tmpColumn = new DataColumn[numberMoved];
			DataColumnInfo[] tmpInfo = new DataColumnInfo[numberMoved];
			IColumnScriptText[] tmpScript = new IColumnScriptText[numberMoved];

			for (int i = 0; i < numberMoved; i++)
			{
				tmpColumn[i] = this[selectedColumns[i]];
				tmpInfo[i] = (DataColumnInfo)this._columnInfoByColumn[_columnsByNumber[i]];
				this._columnScripts.TryGetValue(tmpColumn[i], out tmpScript[i]);
			}

			this.RemoveColumns(selectedColumns, false);

			destination.Insert(tmpColumn, tmpInfo, 0, true);

			// Move the column scripts also
			for (int i = 0; i < numberMoved; i++)
			{
				if (tmpScript[i] != null)
					destination._columnScripts.Add(tmpColumn[i], tmpScript[i]);
			}
		}

		/// <summary>
		/// Appends a cloned column from another DataColumnCollection to this collection.
		/// </summary>
		/// <param name="from">The DataColumnCollection to copy from.</param>
		/// <param name="idx">Index of the source column to clone and then append.</param>
		/// <param name="doCopyData">If true, the original source column data a preserved. Otherwise, the column will be cleared after cloning.</param>
		/// <param name="doCopyProperties">If true, the column script will be cloned too.</param>
		/// <returns>The index of the newly appended column.</returns>
		public int AppendCopiedColumnFrom(DataColumnCollection from, int idx, bool doCopyData, bool doCopyProperties)
		{
			string name = from.GetColumnName(idx);
			DataColumn newCol = (DataColumn)from[idx].Clone();

			if (!doCopyData)
			{
				newCol.Clear();
			}

			int newIdx = this.ColumnCount;
			this.Add(newCol, name, from.GetColumnKind(idx), from.GetColumnGroup(idx));

			if (doCopyProperties)
			{
				IColumnScriptText script = null;
				from.ColumnScripts.TryGetValue(from[idx], out script);
				if (null != script)
					this.ColumnScripts.Add(newCol, (IColumnScriptText)script.Clone());
			}

			return newIdx;
		}

		/// <summary>
		/// Appends a row to this table, which is copied from a source table.
		/// </summary>
		/// <param name="src">Source table to copy the row from.</param>
		/// <param name="srcRow">Index of the row of the source table.</param>
		public void AppendRowFrom(DataColumnCollection src, int srcRow)
		{
			int cols = Math.Min(this.ColumnCount, src.ColumnCount);
			int destRow = this.RowCount;
			for (int i = 0; i < cols; i++)
			{
				var destCol = this[i];
				destCol[destRow] = src[i][srcRow];
				if (destCol.Count > _numberOfRows)
					_numberOfRows = destCol.Count;		// we silently update the row count here, otherwise we cannot append more columns
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public IAscendingIntegerCollection MergeColumnTypesFrom(DataColumnCollection from)
		{
			AscendingIntegerCollection coll = new AscendingIntegerCollection();

			for (int i = 0; i < from.ColumnCount; i++)
			{
				string name = from.GetColumnName(i);
				System.Type coltype = from[i].GetType();

				if (this.ContainsColumn(name) && (this[name].GetType() == from[i].GetType()))
				{
					// then the column is already present with the right name and can be used
					coll.Add(this.GetColumnNumber(this[name]));
				}
				else // name is there - but type is different, or name is not here at all
				{
					int idx = AppendCopiedColumnFrom(from, i, false, true);
					coll.Add(idx);
				}
			}
			return coll;
		}

		#endregion Column removal and move to another collection

		#region Column Information getting/setting

		/// <summary>
		/// Returns whether the column is contained in this collection.
		/// </summary>
		/// <param name="datac">The column in question.</param>
		/// <returns>True if the column is contained in this collection.</returns>
		public bool ContainsColumn(DataColumn datac)
		{
			return _columnInfoByColumn.ContainsKey(datac);
		}

		/// <summary>
		/// Returns whether the column is contained in this collection.
		/// </summary>
		/// <param name="datac">The column in question.</param>
		/// <returns>True if the column is contained in this collection.</returns>
		public bool Contains(DataColumn datac)
		{
			return _columnInfoByColumn.ContainsKey(datac);
		}

		/// <summary>
		/// Test if a column of a given name is present in this collection.
		/// </summary>
		/// <param name="columnname">The columnname to test for presence.</param>
		/// <returns>True if the column with the name is contained in the collection.</returns>
		public bool ContainsColumn(string columnname)
		{
			return _columnsByName.ContainsKey(columnname);
		}

		/// <summary>
		/// Test if a column of a given name is present in this collection.
		/// </summary>
		/// <param name="columnname">The columnname to test for presence.</param>
		/// <returns>True if the column with the name is contained in the collection.</returns>
		public bool Contains(string columnname)
		{
			return _columnsByName.ContainsKey(columnname);
		}

		/// <summary>
		/// Check if the named column is of the expected type.
		/// </summary>
		/// <param name="columnname">Name of the column.</param>
		/// <param name="expectedtype">Expected type of column.</param>
		/// <returns>True if the column exists and is exactly! of the expeced type. False otherwise.</returns>
		public bool IsColumnOfType(string columnname, System.Type expectedtype)
		{
			if (_columnsByName.ContainsKey(columnname) && _columnsByName[columnname].GetType() == expectedtype)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns the position of the column in the Collection.
		/// </summary>
		/// <param name="datac">The column.</param>
		/// <returns>The position of the column, or -1 if the column is not contained.</returns>
		public int GetColumnNumber(Altaxo.Data.DataColumn datac)
		{
			DataColumnInfo info;
			if (_columnInfoByColumn.TryGetValue(datac, out info))
				return info.Number;
			else
				return -1;
		}

		/// <summary>
		/// Returns the name of a column.
		/// </summary>
		/// <param name="datac">The column..</param>
		/// <returns>The name of the column.</returns>
		public string GetColumnName(DataColumn datac)
		{
			DataColumnInfo info;
			if (_columnInfoByColumn.TryGetValue(datac, out info))
				return info.Name;
			else
				return null;
		}

		/// <summary>
		/// Returns the name of the column at position idx.
		/// </summary>
		/// <param name="idx">The position of the column (column number).</param>
		/// <returns>The name of the column.</returns>
		public string GetColumnName(int idx)
		{
			if (idx >= 0 && idx < _columnsByNumber.Count)
				return GetColumnInfo(idx).Name;
			else
				throw new ArgumentOutOfRangeException(string.Format("The column [{0}] in table \"{1}\" does not exist. The current number of columns is {2}.", idx, Main.DocumentPath.GetAbsolutePath(this).ToString(), ColumnCount));
		}

		/// <summary>
		/// Sets the name of the column <code>datac</code>.
		/// </summary>
		/// <param name="datac">The column which name is to set.</param>
		/// <param name="newName">The new name of the column.</param>
		public void SetColumnName(DataColumn datac, string newName)
		{
			string oldName = GetColumnInfo(datac).Name;
			if (oldName != newName)
			{
				if (this.ContainsColumn(newName))
				{
					throw new System.ApplicationException("Try to set column name to the name of a already present column: " + newName);
				}
				else
				{
					GetColumnInfo(datac).Name = newName;
					_columnsByName.Remove(oldName);
					_columnsByName.Add(newName, datac);

					this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnRenameArgs(this.GetColumnNumber(datac)));

					// Inform also the data column itself that the name has changed
					datac.EhSelfTunnelingEventHappened(Main.DocumentPathChangedEventArgs.Empty);

					// reset the TriedOutRegularNames flag, maybe one of the regular columns has been renamed
					this._triedOutRegularNaming = false;
				}
			}
		}

		/// <summary>
		/// Sets the name of the column at position<code>index</code>.
		/// </summary>
		/// <param name="index">The column number.</param>
		/// <param name="newName">The new name of the column.</param>
		public void SetColumnName(int index, string newName)
		{
			SetColumnName(this[index], newName);
		}

		/// <summary>
		/// Rename the column with the name <code>oldName</code> to <code>newName</code>.
		/// </summary>
		/// <param name="oldName">The old name of the column.</param>
		/// <param name="newName">The new name of the column.</param>
		public void SetColumnName(string oldName, string newName)
		{
			SetColumnName(this[oldName], newName);
		}

		/// <summary>
		/// Returns the goup number of the column <code>datac</code>.
		/// </summary>
		/// <param name="datac">The column.</param>
		/// <returns>The group number of this column.</returns>
		public int GetColumnGroup(DataColumn datac)
		{
			return GetColumnInfo(datac).Group;
		}

		/// <summary>
		/// Returns the goup number of the column at index <code>idx</code>
		/// </summary>
		/// <param name="idx">The column number of the column.</param>
		/// <returns>The group number of this column.</returns>
		public int GetColumnGroup(int idx)
		{
			return GetColumnInfo(idx).Group;
		}

		/// <summary>
		/// This function will return the smallest possible group number, which is currently not in use.
		/// </summary>
		/// <returns>The smallest unused group number (starting at 0).</returns>
		public int GetUnusedColumnGroupNumber()
		{
			System.Collections.SortedList groupNums = new System.Collections.SortedList();
			for (int i = 0; i < ColumnCount; i++)
			{
				int group = this.GetColumnGroup(i);
				if (!groupNums.ContainsKey(group))
					groupNums.Add(group, null);
			}

			for (int i = 0; i < int.MaxValue; i++)
			{
				if (!groupNums.Contains(i))
					return i;
			}
			return 0;
		}

		/// <summary>
		/// Determines if this collections has Columns only with one group number, or if it has columns with multiple group numbers.
		/// </summary>
		/// <returns>False if all the columns have the same group number. True if the columns have more than one group number.</returns>
		public bool HaveMultipleGroups()
		{
			if (ColumnCount <= 1)
				return false;
			int firstgroup = this.GetColumnGroup(0);
			for (int i = 1; i < ColumnCount; i++)
				if (firstgroup != this.GetColumnGroup(i))
					return true;

			return false;
		}

		/// <summary>
		/// Sets the group number of the column with the given column number <code>idx</code>.
		/// </summary>
		/// <param name="idx">The column number of the column.</param>
		/// <param name="groupNumber">The group number to set for this column.</param>
		public void SetColumnGroup(int idx, int groupNumber)
		{
			GetColumnInfo(idx).Group = groupNumber;
			EnsureUniqueColumnKindsForIndependentVariables(groupNumber, this[idx]);
		}

		/// <summary>
		/// Returns the kind of the column <code>datac</code>.
		/// </summary>
		/// <param name="datac">The column for which the kind is returned.</param>
		/// <returns>The kind of the provided column.</returns>
		public ColumnKind GetColumnKind(DataColumn datac)
		{
			return GetColumnInfo(datac).Kind;
		}

		/// <summary>
		/// Returns the column kind of the column at index <code>idx</code>
		/// </summary>
		/// <param name="idx">The column number of the column.</param>
		/// <returns>The kind of this column.</returns>
		public ColumnKind GetColumnKind(int idx)
		{
			return GetColumnInfo(idx).Kind;
		}

		/// <summary>
		/// Sets the kind of the column.
		/// </summary>
		/// <param name="datac">The column for which to set the kind (have to be member of this collection).</param>
		/// <param name="columnKind">The new kind of the column.</param>
		public void SetColumnKind(DataColumn datac, ColumnKind columnKind)
		{
			DataColumnInfo info = GetColumnInfo(datac);
			info.Kind = columnKind;
			EnsureUniqueColumnKindsForIndependentVariables(info.Group, datac);
		}

		/// <summary>
		/// Sets the kind of the column with column number <code>idx</code>.
		/// </summary>
		/// <param name="idx">The column number of the column.</param>
		/// <param name="columnKind">The new kind of the column.</param>
		public void SetColumnKind(int idx, ColumnKind columnKind)
		{
			SetColumnKind(this[idx], columnKind);
		}

		/// <summary>
		/// Sets the kind of the column with name <paramref name="columnName"/>.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="columnKind">The new kind of the column.</param>
		public void SetColumnKind(string columnName, ColumnKind columnKind)
		{
			SetColumnKind(this[columnName], columnKind);
		}

		/// <summary>
		/// Ensures that for a given group number there is only one column for each independent variable (X,Y,Z).
		/// </summary>
		/// <param name="groupNumber">The group number of the columns which are checked for this rule.</param>
		/// <param name="exceptThisColumn">If not null, this column is treated with priority. If this column is a independent variable column, it can
		/// keep its kind.</param>
		protected void EnsureUniqueColumnKindsForIndependentVariables(int groupNumber, DataColumn exceptThisColumn)
		{
			bool X_present = false;
			bool Y_present = false;
			bool Z_present = false;

			if (exceptThisColumn != null)
			{
				switch (GetColumnInfo(exceptThisColumn).Kind)
				{
					case ColumnKind.X:
						X_present = true;
						break;

					case ColumnKind.Y:
						Y_present = true;
						break;

					case ColumnKind.Z:
						Z_present = true;
						break;
				}
			}

			foreach (KeyValuePair<DataColumn, DataColumnInfo> entry in this._columnInfoByColumn)
			{
				if ((entry.Value).Group == groupNumber && !entry.Key.Equals(exceptThisColumn))
				{
					DataColumnInfo info = entry.Value;
					switch (info.Kind)
					{
						case ColumnKind.X:
							if (X_present)
								info.Kind = ColumnKind.V;
							else
								X_present = true;
							break;

						case ColumnKind.Y:
							if (Y_present)
								info.Kind = ColumnKind.V;
							else
								Y_present = true;
							break;

						case ColumnKind.Z:
							if (Z_present)
								info.Kind = ColumnKind.V;
							else
								Z_present = true;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Removes the x-property from all columns in the group nGroup.
		/// </summary>
		/// <param name="nGroup">the group number for the columns from which to remove the x-property</param>
		/// <param name="exceptThisColumn">If not null, this column is treated with priority, it can keep its kind.</param>
		public void DeleteXProperty(int nGroup, DataColumn exceptThisColumn)
		{
			int len = this.ColumnCount;
			for (int i = len - 1; i >= 0; i--)
			{
				DataColumn dc = (DataColumn)_columnsByNumber[i];
				DataColumnInfo info = (DataColumnInfo)this._columnInfoByColumn[dc];
				if (info.Group == nGroup && info.Kind == ColumnKind.X && !dc.Equals(exceptThisColumn))
				{
					info.Kind = ColumnKind.V;
				}
			}
		}

		/// <summary>
		/// Using a given column, find the related X column of this.
		/// </summary>
		/// <param name="datac">The column for which to find the related X column.</param>
		/// <returns>The related X column, or null if it is not found.</returns>
		public Altaxo.Data.DataColumn FindXColumnOf(DataColumn datac)
		{
			return FindXColumnOfGroup(GetColumnGroup(datac));
		}

		/// <summary>
		/// Returns the X column of the column group <code>nGroup</code>.
		/// </summary>
		/// <param name="nGroup">The column group number.</param>
		/// <returns>The X column of the provided group, or null if it is not found.</returns>
		public Altaxo.Data.DataColumn FindXColumnOfGroup(int nGroup)
		{
			int len = this.ColumnCount;
			for (int i = len - 1; i >= 0; i--)
			{
				DataColumn dc = (DataColumn)_columnsByNumber[i];
				DataColumnInfo info = (DataColumnInfo)this._columnInfoByColumn[dc];
				if (info.Group == nGroup && info.Kind == ColumnKind.X)
				{
					return dc;
				}
			}
			return null;
		}

		/// <summary>
		/// Get the column info for the column <code>datac</code>.
		/// </summary>
		/// <param name="datac">The column for which the column information is returned.</param>
		/// <returns>The column information of the provided column.</returns>
		private DataColumnInfo GetColumnInfo(DataColumn datac)
		{
			return (DataColumnInfo)_columnInfoByColumn[datac];
		}

		/// <summary>
		/// Get the column info for the column with index<code>idx</code>.
		/// </summary>
		/// <param name="idx">The column index of the column for which the column information is returned.</param>
		/// <returns>The column information of the column.</returns>
		private DataColumnInfo GetColumnInfo(int idx)
		{
			return (DataColumnInfo)_columnInfoByColumn[_columnsByNumber[idx]];
		}

		/// <summary>
		/// Get the column info for the column with name <code>columnName</code>.
		/// </summary>
		/// <param name="columnName">The column name of the column for which the column information is returned.</param>
		/// <returns>The column information of the column.</returns>
		private DataColumnInfo GetColumnInfo(string columnName)
		{
			return (DataColumnInfo)_columnInfoByColumn[_columnsByName[columnName]];
		}

		#endregion Column Information getting/setting

		#region Row insertion/removal

		/// <summary>
		/// Insert an empty row at the provided row number.
		/// </summary>
		/// <param name="atRowNumber">The row number at which to insert.</param>
		public void InsertRow(int atRowNumber)
		{
			InsertRows(atRowNumber, 1);
		}

		/// <summary>
		/// Insert a number of empty rows in all columns.
		/// </summary>
		/// <param name="nBeforeRow">The row number before which the additional rows should be inserted.</param>
		/// <param name="nRowsToInsert">The number of rows to insert.</param>
		public void InsertRows(int nBeforeRow, int nRowsToInsert)
		{
			using (var suspendToken = this.SuspendGetToken())
			{
				for (int i = 0; i < ColumnCount; i++)
					this[i].InsertRows(nBeforeRow, nRowsToInsert);

				suspendToken.Dispose();
			}
		}

		/// <summary>
		/// Removes a number of rows from all columns.
		/// </summary>
		/// <param name="nFirstRow">Number of first row to remove.</param>
		/// <param name="nCount">Number of rows to remove, starting from nFirstRow.</param>
		public void RemoveRows(int nFirstRow, int nCount)
		{
			RemoveRows(ContiguousIntegerRange.FromStartAndCount(nFirstRow, nCount));
		}

		/// <summary>
		/// Removes the <code>selectedRows</code> from the table.
		/// </summary>
		/// <param name="selectedRows">Collection of indizes to the rows that should be removed.</param>
		public void RemoveRows(IAscendingIntegerCollection selectedRows)
		{
			RemoveRowsInColumns(ContiguousIntegerRange.FromStartAndCount(0, ColumnCount), selectedRows);
		}

		/// <summary>
		/// Removes all rows.
		/// </summary>
		public void RemoveRowsAll()
		{
			RemoveRows(0, this.RowCount);
		}

		/// <summary>
		/// Removes all rows. Same function as <see cref="RemoveRowsAll" />.
		/// </summary>
		public void ClearData()
		{
			RemoveRowsAll();
		}

		/// <summary>
		/// Removes the <code>selectedRows</code> from the table.
		/// </summary>
		/// <param name="selectedColumns">Collection of indices to the columns that should be removed.</param>
		/// <param name="selectedRows">Collection of indizes to the rows that should be removed.</param>
		public void RemoveRowsInColumns(IAscendingIntegerCollection selectedColumns, IAscendingIntegerCollection selectedRows)
		{
			// if we remove rows, we have to do that in reverse order
			using (var suspendToken = this.SuspendGetToken())
			{
				for (int selcol = 0; selcol < selectedColumns.Count; selcol++)
				{
					int colidx = selectedColumns[selcol];

					foreach (var range in selectedRows.RangesDescending)
					{
						this[colidx].RemoveRows(range.Start, range.Count);
					}
				}

				suspendToken.Dispose();
			}
		}

		/// <summary>
		/// Removes a single row of all columns.
		/// </summary>
		/// <param name="nFirstRow">The row to remove.</param>
		public void RemoveRow(int nFirstRow)
		{
			RemoveRows(nFirstRow, 1);
		}

		#endregion Row insertion/removal

		#region Column and row position manipulation

		public void SwapColumnPositions(int i, int j)
		{
			if (i == j)
				return;
			if (i < 0 || i >= ColumnCount)
				throw new ArgumentOutOfRangeException("Index i is out of rage");
			if (j < 0 || j >= ColumnCount)
				throw new ArgumentOutOfRangeException("Index j is out of rage");

			DataColumn coli = (DataColumn)_columnsByNumber[i];
			DataColumn colj = (DataColumn)_columnsByNumber[j];

			_columnsByNumber[i] = colj;
			_columnsByNumber[j] = coli;
			((DataColumnInfo)_columnInfoByColumn[coli]).Number = j;
			((DataColumnInfo)_columnInfoByColumn[colj]).Number = i;

			this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnMoveArgs(Math.Min(i, j), Math.Max(i, j)));
		}

		/// <summary>
		/// Moves one or more columns to a new position.
		/// </summary>
		/// <param name="selectedColumns">The indices of the columns to move to the new position.</param>
		/// <param name="newPosition">The new position where the columns are moved to.</param>
		/// <remarks>An exception is thrown if newPosition is negative or higher than possible.</remarks>
		public void ChangeColumnPosition(Altaxo.Collections.IAscendingIntegerCollection selectedColumns, int newPosition)
		{
			int numberSelected = selectedColumns.Count;
			if (numberSelected == 0)
				return;

			int oldPosition = selectedColumns[0];
			if (oldPosition == newPosition)
				return;

			// check that the newPosition is ok
			if (newPosition < 0)
				throw new ArgumentException("New column position is negative!");
			if (newPosition + numberSelected > ColumnCount)
				throw new ArgumentException(string.Format("New column position too high: ColsToMove({0})+NewPosition({1})>ColumnCount({2})", numberSelected, newPosition, ColumnCount));

			// Allocated tempory storage for the datacolumns
			Altaxo.Data.DataColumn[] columnsMoved = new Altaxo.Data.DataColumn[selectedColumns.Count];
			// fill temporary storage
			for (int i = 0; i < selectedColumns.Count; i++)
				columnsMoved[i] = this[selectedColumns[i]];

			int firstAffectedColumn = 0;
			int maxAffectedColumn = ColumnCount;
			if (newPosition < oldPosition) // move down to lower indices
			{
				firstAffectedColumn = newPosition;
				maxAffectedColumn = Math.Max(newPosition + numberSelected, selectedColumns[numberSelected - 1] + 1);

				for (int i = selectedColumns[numberSelected - 1], offset = 0; i >= firstAffectedColumn; i--)
				{
					if (numberSelected > offset && i == selectedColumns[numberSelected - 1 - offset])
						offset++;
					else
						_columnsByNumber[i + offset] = _columnsByNumber[i];
				}
			}
			else // move up to higher
			{
				firstAffectedColumn = selectedColumns[0];
				maxAffectedColumn = newPosition + numberSelected;
				for (int i = selectedColumns[0], offset = 0; i < maxAffectedColumn; i++)
				{
					if (offset < numberSelected && i == selectedColumns[offset])
						offset++;
					else
						_columnsByNumber[i - offset] = _columnsByNumber[i];
				}
			}

			// Fill in temporary stored columns on new position
			for (int i = 0; i < numberSelected; i++)
				_columnsByNumber[newPosition + i] = columnsMoved[i];

			RefreshColumnIndices();
			this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateColumnMoveArgs(firstAffectedColumn, maxAffectedColumn));
		}

		/// <summary>
		/// Moves on or more rows in to a new position.
		/// </summary>
		/// <param name="selectedIndices">The indices of the rows to move.</param>
		/// <param name="newPosition">The new position of the rows.</param>
		public void ChangeRowPosition(Altaxo.Collections.IAscendingIntegerCollection selectedIndices, int newPosition)
		{
			int numberSelected = selectedIndices.Count;
			if (numberSelected == 0)
				return;

			int oldPosition = selectedIndices[0];
			if (oldPosition == newPosition)
				return;

			// check that the newPosition is ok
			if (newPosition < 0)
				throw new ArgumentException("New row position is negative!");

			// Allocated tempory storage for the datacolumns
			Altaxo.Data.AltaxoVariant[] tempMoved = new Altaxo.Data.AltaxoVariant[numberSelected];

			int firstAffected;
			int maxAffected;

			if (newPosition < oldPosition) // move down to lower indices
			{
				firstAffected = newPosition;
				maxAffected = Math.Max(newPosition + numberSelected, selectedIndices[numberSelected - 1] + 1);
			}
			else
			{
				firstAffected = selectedIndices[0];
				maxAffected = newPosition + numberSelected;
			}

			for (int nColumn = ColumnCount - 1; nColumn >= 0; nColumn--)
			{
				Altaxo.Data.DataColumn thiscolumn = this[nColumn];
				// fill temporary storage
				for (int i = 0; i < numberSelected; i++)
					tempMoved[i] = thiscolumn[selectedIndices[i]];

				if (newPosition < oldPosition) // move down to lower indices
				{
					for (int i = selectedIndices[numberSelected - 1], offset = 0; i >= firstAffected; i--)
					{
						if (numberSelected > offset && i == selectedIndices[numberSelected - 1 - offset])
							offset++;
						else
							thiscolumn[i + offset] = thiscolumn[i];
					}
				}
				else // move up to higher
				{
					for (int i = selectedIndices[0], offset = 0; i < maxAffected; i++)
					{
						if (offset < numberSelected && i == selectedIndices[offset])
							offset++;
						else
							thiscolumn[i - offset] = thiscolumn[i];
					}
				}

				// Fill in temporary stored columns on new position
				for (int i = 0; i < numberSelected; i++)
					thiscolumn[newPosition + i] = tempMoved[i];
			}
			this.EhChildChanged(null, DataColumnCollectionChangedEventArgs.CreateRowMoveArgs(ColumnCount, firstAffected, maxAffected));
		}

		/// <summary>
		/// This will refresh the column number information in the m_ColumnInfo collection of <see cref="DataColumnInfo" />.
		/// </summary>
		protected void RefreshColumnIndices()
		{
			for (int i = ColumnCount - 1; i >= 0; i--)
			{
				((DataColumnInfo)_columnInfoByColumn[_columnsByNumber[i]]).Number = i;
			}
		}

		#endregion Column and row position manipulation

		#region Indexer

		/// <summary>
		/// Returns the column with name <code>s</code>. Sets the column with name <code>s</code> by copying data from
		/// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
		/// </summary>
		public Altaxo.Data.DataColumn this[string s]
		{
			get
			{
				DataColumn result;
				if (_columnsByName.TryGetValue(s, out result))
					return result;
				else
					throw new ArgumentOutOfRangeException(string.Format("The column \"{0}\" in \"{1}\" does not exist.", s, Main.DocumentPath.GetAbsolutePath(this).ToString()));
			}
			set
			{
				// setting a column should not change its name nor its other properties
				// only the data array and the related parameters should be changed
				Altaxo.Data.DataColumn c;
				if (_columnsByName.TryGetValue(s, out c))
					c.CopyDataFrom(value);
				else
					throw new ArgumentOutOfRangeException(string.Format("The column \"{0}\" in \"{1}\" does not exist.", s, Main.DocumentPath.GetAbsolutePath(this).ToString()));
			}
		}

		/// <summary>
		/// Returns the column with name <code>s</code>. Sets the column with name <code>s</code> by copying data from
		/// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
		/// </summary>
		/// <param name="s">The name of the column to retrieve.</param>
		/// <returns>Either the column with the given name, or Null if such a column don't exist.</returns>
		public Altaxo.Data.DataColumn TryGetColumn(string s)
		{
			DataColumn result;
			if (_columnsByName.TryGetValue(s, out result))
				return result;
			else
				return null;
		}

		/// <summary>
		/// Returns the column at index <code>idx</code>. Sets the column at index<code>idx</code> by copying data from
		/// the other column (not by replacing). An exception is thrown if the two columns are not of the same type.
		/// </summary>
		public Altaxo.Data.DataColumn this[int idx]
		{
			get
			{
				try
				{
					return (Altaxo.Data.DataColumn)_columnsByNumber[idx];
				}
				catch (Exception)
				{
					throw new ArgumentOutOfRangeException(string.Format("The column [{0}] in table \"{1}\" does not exist. The current number of columns is {2}.", idx, Main.DocumentPath.GetAbsolutePath(this).ToString(), ColumnCount));
				}
			}
			set
			{
				// setting a column should not change its name nor its other properties
				// only the data array and the related parameters should be changed

				if (idx < _columnsByNumber.Count)
				{
					Altaxo.Data.DataColumn c = (Altaxo.Data.DataColumn)_columnsByNumber[idx];
					c.CopyDataFrom(value);
				}
				else
				{
					throw new ArgumentOutOfRangeException(string.Format("The column [{0}] in table \"{1}\" does not exist. The current number of columns is {2}.", idx, Main.DocumentPath.GetAbsolutePath(this).ToString(), ColumnCount));
				}
			}
		}

		#endregion Indexer

		#region Collection Properties

		/// <summary>
		/// The row count, i.e. the maximum of the row counts of all columns.
		/// </summary>
		public int RowCount
		{
			get
			{
				if (this._hasNumberOfRowsDecreased)
					this.RefreshRowCount(true);

				return _numberOfRows;
			}
		}

		/// <summary>
		/// The number of columns in the collection.
		/// </summary>
		public int ColumnCount
		{
			get
			{
				return this._columnsByNumber.Count;
			}
		}

		/// <summary>
		/// The parent of this collection.
		/// </summary>
		public override Main.IDocumentNode ParentObject
		{
			get { return _parent; }
			set
			{
				object oldParent = _parent;
				_parent = value;

				if (!object.ReferenceEquals(oldParent, _parent))
					OnParentChanged(oldParent, _parent);
			}
		}

		protected virtual void OnParentChanged(object oldParent, object newParent)
		{
			if (null != ParentChanged)
				ParentChanged(this, new Altaxo.Main.ParentChangedEventArgs(oldParent, newParent));
		}

		/// <summary>
		/// Returns the collection of column scripts.
		/// </summary>
		public Dictionary<DataColumn, IColumnScriptText> ColumnScripts
		{
			get { return this._columnScripts; }
		}

		/// <summary>
		/// Returns an array containing all column names that are contained in this collection.
		/// </summary>
		/// <returns>An array containing all column names that are contained in this collection.</returns>
		public string[] GetColumnNames()
		{
			string[] arr = new string[this.ColumnCount];
			for (int i = 0; i < arr.Length; i++)
				arr[i] = GetColumnName(i);

			return arr;
		}

		/// <summary>
		/// Tests whether or not all columns in this collection have the same type.
		/// </summary>
		/// <param name="firstdifferentcolumnindex">Out: returns the first column that has a different type from the first column</param>.
		/// <returns>True if all columns are of the same type.</returns>
		public bool AreAllColumnsOfTheSameType(out int firstdifferentcolumnindex)
		{
			firstdifferentcolumnindex = 0;

			if (0 == this.ColumnCount)
				return true;

			System.Type t1st = this[0].GetType();

			int len = this.ColumnCount;
			for (int i = 0; i < len; i++)
			{
				if (this[i].GetType() != t1st)
				{
					firstdifferentcolumnindex = i;
					return false;
				}
			}

			return true;
		}

		#endregion Collection Properties

		#region Event handling

		/// <summary>
		/// Returns true if this object has outstanding changed not reported yet to the parent.
		/// </summary>
		public virtual bool IsDirty
		{
			get
			{
				return null != _accumulatedEventData;
			}
		}

		/// <summary>
		/// Accumulates the changes reported by the DataColumns.
		/// </summary>
		/// <param name="sender">One of the columns of this collection.</param>
		/// <param name="e">The change details.</param>
		/// <param name="accumulatedEventData">The instance were the event arg e is accumulated. If this parameter is <c>null</c>, a new instance of <see cref="DataColumnCollectionChangedEventArgs"/> is created and returned into this parameter.</param>
		protected void AccumulateChangeData(object sender, EventArgs e, ref DataColumnCollectionChangedEventArgs accumulatedEventData)
		{
			DataColumnChangedEventArgs dataColumnChangeEventArgs = e as DataColumnChangedEventArgs;
			DataColumnCollectionChangedEventArgs dataColumnCollectionChangeEventArgs;
			DataColumn senderAsDataColumn;

			if (null != (senderAsDataColumn = sender as DataColumn) && (null != (dataColumnChangeEventArgs = e as DataColumnChangedEventArgs))) // ChangeEventArgs from a DataColumn
			{
				int columnNumberOfSender = GetColumnNumber(senderAsDataColumn);
				int rowCountOfSender = senderAsDataColumn.Count;

				if (accumulatedEventData == null)
					accumulatedEventData = new DataColumnCollectionChangedEventArgs(columnNumberOfSender, dataColumnChangeEventArgs.MinRowChanged, dataColumnChangeEventArgs.MaxRowChanged, dataColumnChangeEventArgs.HasRowCountDecreased);
				else
					accumulatedEventData.Accumulate(columnNumberOfSender, dataColumnChangeEventArgs.MinRowChanged, dataColumnChangeEventArgs.MaxRowChanged, dataColumnChangeEventArgs.HasRowCountDecreased);

				// update the row count
				if (this._numberOfRows < rowCountOfSender)
					_numberOfRows = rowCountOfSender;
				this._hasNumberOfRowsDecreased |= dataColumnChangeEventArgs.HasRowCountDecreased;
			}
			else if (null != (dataColumnCollectionChangeEventArgs = e as DataColumnCollectionChangedEventArgs)) // ChangeEventArgs from a DataColumnCollection, i.e. from myself
			{
				if (null == accumulatedEventData)
					accumulatedEventData = dataColumnCollectionChangeEventArgs;
				else
					accumulatedEventData.Accumulate(dataColumnCollectionChangeEventArgs);

				// update the row count
				if (this._numberOfRows < dataColumnCollectionChangeEventArgs.MaxRowChanged)
					this._numberOfRows = dataColumnCollectionChangeEventArgs.MaxRowChanged;
				this._hasNumberOfRowsDecreased |= dataColumnCollectionChangeEventArgs.HasRowCountDecreased;
			}
		}

		/// <summary>
		/// Accumulates the changes reported by the DataColumns.
		/// </summary>
		/// <param name="sender">One of the columns of this collection.</param>
		/// <param name="e">The change details.</param>
		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			AccumulateChangeData(sender, e, ref _accumulatedEventData);
		}

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			Main.ParentChangedEventArgs parentChangedEventArgs;
			if (null != (parentChangedEventArgs = e as Main.ParentChangedEventArgs))
			{
				if (object.ReferenceEquals(this, parentChangedEventArgs.OldParent) && this.ContainsColumn((DataColumn)sender))
					this.RemoveColumn((DataColumn)sender);
				else if (object.ReferenceEquals(this, parentChangedEventArgs.NewParent) && !this.ContainsColumn((DataColumn)sender))
					throw new ApplicationException("Not allowed to set child's parent to this collection before adding it to the collection");

				return true;
			}

			return false;
		}

		/// <summary>
		/// Handles the cases when a child changes, but a reaction is neccessary only if the table is not suspended currently.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <returns>
		/// True if the event has not changed the state of the table (i.e. it requires no further action).
		/// </returns>
		/// <remarks>
		/// This instance is always sending EventArgs of type <see cref="DataColumnCollectionChangedEventArgs"/>. This means, that event args of any other type should be transformed in <see cref="DataColumnCollectionChangedEventArgs"/>.
		/// </remarks>
		protected override bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			DataColumnCollectionChangedEventArgs result = null;
			AccumulateChangeData(sender, e, ref result); // Get ChangeEventArgs in the result.
			e = result;

			return false;
		}

		#endregion Event handling

		/// <summary>
		/// Refreshes the row count by observing all columns.
		/// </summary>
		public void RefreshRowCount()
		{
			RefreshRowCount(true);
		}

		/// <summary>
		/// Refreshes the row count.
		/// </summary>
		/// <param name="bSearchOnlyUntilOldRowCountReached">If false, all columns are observed. If true, the columns are
		/// observed only until one column is reached, which has the same value of the row count as this collection.</param>
		public void RefreshRowCount(bool bSearchOnlyUntilOldRowCountReached)
		{
			int rowCount = 0;

			if (bSearchOnlyUntilOldRowCountReached)
			{
				foreach (DataColumn c in this._columnsByNumber)
				{
					rowCount = System.Math.Max(rowCount, c.Count);
					if (rowCount >= _numberOfRows)
						break;
				}
			}
			else
			{
				foreach (DataColumn c in this._columnsByNumber)
					rowCount = System.Math.Max(rowCount, c.Count);
			}

			// now take over the new row count
			this._numberOfRows = rowCount;
			this._hasNumberOfRowsDecreased = false; // row count is now actual
		}

		#region Automatic column naming

		/// <summary>
		/// Finds a new unique column name.
		/// </summary>
		/// <returns>The new unique column name.</returns>
		public string FindNewColumnName()
		{
			return FindUniqueColumnName(null);
		}

		/// <summary>
		/// Calculates a new column name dependend on the last name. You have to check whether the returned name is already in use by yourself.
		/// </summary>
		/// <param name="lastName">The last name that was used to name a column.</param>
		/// <returns>The logical next name of a column calculated from the previous name. This name is in the range "A" to "ZZ". If
		/// no further name can be found, this function returns null.</returns>
		public static string GetNextColumnName(string lastName)
		{
			int lastNameLength = null == lastName ? 0 : lastName.Length;

			if (0 == lastNameLength)
				return "A";
			else if (1 == lastNameLength)
			{
				char _1st = lastName[0];
				_1st++;
				if (_1st >= 'A' && _1st <= 'Z')
					return _1st.ToString();
				else
					return "AA";
			}
			else if (2 == lastNameLength)
			{
				char _1st = lastName[0];
				char _2nd = lastName[1];
				_2nd++;

				if (_2nd < 'A' || _2nd > 'Z')
				{
					_1st++;
					_2nd = 'A';
				}

				if (_1st >= 'A' && _1st <= 'Z')
					return _1st.ToString() + _2nd;
				else
					return null;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Get a unique column name based on regular naming from A to ZZ.
		/// </summary>
		/// <returns>An unique column name based on regular naming.</returns>
		protected string FindUniqueColumnNameWithoutBase()
		{
			string tryName;
			if (_triedOutRegularNaming)
			{
				for (; ; )
				{
					tryName = ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
					if (!_columnsByName.ContainsKey(tryName))
						return tryName;
				}
			}
			else
			{
				// First try it with the next name after the last column
				tryName = GetNextColumnName(_columnsByNumber.Count > 0 ? GetColumnName(ColumnCount - 1) : "");
				if (!string.IsNullOrEmpty(tryName) && !_columnsByName.ContainsKey(tryName))
					return tryName;

				// then try it with all names from A-ZZ
				for (tryName = "A"; tryName != null; tryName = GetNextColumnName(tryName))
				{
					if (!_columnsByName.ContainsKey(tryName))
						return tryName;
				}
				// if no success, set the _TriedOutRegularNaming
				_triedOutRegularNaming = true;
				return FindUniqueColumnNameWithoutBase();
			}
		}

		/// <summary>
		/// Get a unique column name based on a provided string. If a column with the name of the provided string
		/// already exists, a new name is created by appending a dot and then A-ZZ.
		/// </summary>
		/// <param name="sbase">A string which is the base of the new name. Must not be null!</param>
		/// <returns>An unique column name based on the provided string.</returns>
		protected string FindUniqueColumnNameWithBase(string sbase)
		{
			// try the name directly
			if (!_columnsByName.ContainsKey(sbase))
				return sbase;

			sbase = sbase + ".";

			// then try it with all names from A-ZZ

			for (string tryAppendix = "A"; tryAppendix != null; tryAppendix = GetNextColumnName(tryAppendix))
			{
				if (!_columnsByName.ContainsKey(sbase + tryAppendix))
					return sbase + tryAppendix;
			}

			// if no success, append a hex string
			for (; ; )
			{
				string tryName = sbase + ((uint)System.Guid.NewGuid().GetHashCode()).ToString("X8");
				if (!_columnsByName.ContainsKey(tryName))
					return tryName;
			}
		}

		/// <summary>
		/// Get a unique column name based on a provided string.
		/// </summary>
		/// <param name="sbase">The base name.</param>
		/// <returns>An unique column name based on the provided string.</returns>
		public string FindUniqueColumnName(string sbase)
		{
			return sbase == null ? FindUniqueColumnNameWithoutBase() : FindUniqueColumnNameWithBase(sbase);
		}

		#endregion Automatic column naming

		#region Special Collection methods

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <returns>null if succeeded, error string otherwise</returns>
		public virtual string Transpose()
		{
			int firstdifferent;

			if (!AreAllColumnsOfTheSameType(out firstdifferent))
			{
				return String.Format("Column[{0}] ({1}) has a different type than the first column transpose is not possible!", firstdifferent, GetColumnName(firstdifferent));
			}

			// now we can start by adding additional columns of the row count is greater
			// than the column count
			using (var suspendToken = this.SuspendGetToken())
			{
				int originalrowcount = this.RowCount;
				int originalcolcount = this.ColumnCount;
				if (this.RowCount > this.ColumnCount)
				{
					int addcols = this.RowCount - this.ColumnCount;
					// this is a little tricky - we have to add the same type of
					// column like the first one
					System.Type coltype = this[0].GetType();
					for (int ii = 0; ii < addcols; ii++)
					{
						Altaxo.Data.DataColumn dt = (Altaxo.Data.DataColumn)Activator.CreateInstance(coltype);
						this.Add(dt);
					}
				} // if RowCount>ColumnCount

				// now we can exchange the data

				int tocol;
				int i, j;
				if (originalcolcount >= originalrowcount)
				{
					for (i = 0; i < originalrowcount; i++)
					{
						for (j = i + 1; j < originalcolcount; j++)
						{
							Altaxo.Data.AltaxoVariant hlp = this[j][i];
							this[j][i] = this[i][j];
							this[i][j] = hlp;
						}
					}
				}
				else // originalrowcount>originalcolcount
				{
					tocol = originalcolcount;
					for (i = 0; i < originalcolcount; i++)
					{
						for (j = i + 1; j < originalrowcount; j++)
						{
							Altaxo.Data.AltaxoVariant hlp = this[i][j];
							this[i][j] = this[j][i];
							this[j][i] = hlp;
						}
					}
				}

				// now we should delete the superfluous columns when originalcolcount>originalrowcount
				if (originalcolcount > originalrowcount)
				{
					this.RemoveColumns(originalrowcount, originalcolcount - originalrowcount);
				}

				suspendToken.Dispose();
			}

			return null; // no error message
		}

		#endregion Special Collection methods

		#region INamedObjectCollection Members

		/// <summary>
		/// Returns the column with the name <code>name</code>.
		/// </summary>
		/// <param name="name">The name of the column to retrieve.</param>
		/// <returns>The column with name <code>name</code>, or null if not found.</returns>
		public override Main.IDocumentLeafNode GetChildObjectNamed(string name)
		{
			DataColumn col;
			if (_columnsByName.TryGetValue(name, out col))
				return col;
			else
				return null;
		}

		/// <summary>
		/// Retrieves the name of a child column <code>o</code>.
		/// </summary>
		/// <param name="o">The child column.</param>
		/// <returns>The name of the column.</returns>
		public override string GetNameOfChildObject(Main.IDocumentLeafNode o)
		{
			DataColumnInfo info;
			if ((o is DataColumn) && this._columnInfoByColumn.TryGetValue((DataColumn)o, out info))
				return info.Name;
			else
				return null;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			for (int i = _columnsByNumber.Count - 1; i >= 0; --i)
			{
				var col = _columnsByNumber[i];
				DataColumnInfo info;
				if (_columnInfoByColumn.TryGetValue(col, out info))
					yield return new Main.DocumentNodeAndName(col, info.Name);
			}
		}

		#endregion INamedObjectCollection Members

		#region IList<DataRow> support

		#region IEnumerable<DataRow> Members

		public IEnumerator<DataRow> GetEnumerator()
		{
			for (int i = 0; i < RowCount; i++)
				yield return new DataRow(this, i);
		}

		#endregion IEnumerable<DataRow> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < RowCount; i++)
				yield return new DataRow(this, i);
		}

		#endregion IEnumerable Members

		#region IList<DataRow> Members

		int IList<DataRow>.IndexOf(DataRow item)
		{
			return item.RowIndex;
		}

		void IList<DataRow>.Insert(int index, DataRow item)
		{
			InsertRows(index, 1);

			// if by insertion of a row we changed to row that was meant for insertion, we have to correct the index;
			if (index <= item.RowIndex && object.ReferenceEquals(this, item.ColumnCollection))
				item = new DataRow(item.ColumnCollection, item.RowIndex + 1);

			for (int i = 0; i < ColumnCount; i++)
				this[i][index] = item[i];
		}

		void IList<DataRow>.RemoveAt(int index)
		{
			RemoveRow(index);
		}

		DataRow IList<DataRow>.this[int index]
		{
			get
			{
				return new DataRow(this, index);
			}
			set
			{
				int nCols = Math.Min(this.ColumnCount, value.ColumnCollection.ColumnCount);
				for (int i = nCols - 1; i >= 0; i--)
					this[i][index] = value[i];
			}
		}

		#endregion IList<DataRow> Members

		#region ICollection<DataRow> Members

		void ICollection<DataRow>.Add(DataRow item)
		{
			int nCols = Math.Min(this.ColumnCount, item.ColumnCollection.ColumnCount);
			int index = this.RowCount;
			for (int i = nCols - 1; i >= 0; i--)
				this[i][index] = item[i];
		}

		void ICollection<DataRow>.Clear()
		{
			ClearData();
		}

		bool ICollection<DataRow>.Contains(DataRow item)
		{
			return object.ReferenceEquals(this, item.ColumnCollection);
		}

		void ICollection<DataRow>.CopyTo(DataRow[] array, int arrayIndex)
		{
			for (int i = 0; i < this.RowCount; i++)
				array[i + arrayIndex] = new DataRow(this, i);
		}

		int ICollection<DataRow>.Count
		{
			get { return RowCount; }
		}

		bool ICollection<DataRow>.IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<DataRow>.Remove(DataRow item)
		{
			RemoveRow(item.RowIndex);
			return true;
		}

		#endregion ICollection<DataRow> Members

		#endregion IList<DataRow> support

		/// <summary>
		/// Gets the parent column collection of a column.
		/// </summary>
		public static Altaxo.Data.DataColumnCollection GetParentDataColumnCollectionOf(Altaxo.Data.DataColumn column)
		{
			if (column.ParentObject is DataColumnCollection)
				return (DataColumnCollection)column.ParentObject;
			else
				return (DataColumnCollection)Main.DocumentPath.GetRootNodeImplementing(column, typeof(DataColumnCollection));
		}
	} // end class Altaxo.Data.DataColumnCollection
}