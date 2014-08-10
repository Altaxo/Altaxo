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

using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Holds reference to a matrix-like arrangement of data from a <see cref="DataTable"/>. The matrix data consist of 2 or more <see cref="DataColumn"/>s and all or selected data rows.
	/// Furthermore, a row header column and a column header column can deliver corresponding physical values for each matrix row and column, respectively.
	/// </summary>
	public class DataTableMatrixProxy : Main.IChangedEventSource, Main.ICopyFrom
	{
		#region Inner classes

		private class DoubleMatrixAsNullDevice : Altaxo.Calc.LinearAlgebra.IMatrix
		{
			private int _rows, _columns;

			public DoubleMatrixAsNullDevice(int rows, int columns)
			{
				_rows = rows;
				_columns = columns;
			}

			public static DoubleMatrixAsNullDevice GetMatrix(int rows, int columns)
			{
				return new DoubleMatrixAsNullDevice(rows, columns);
			}

			public double this[int row, int col]
			{
				get
				{
					throw new InvalidOperationException("This is a matrix that act as null device for incoming data, thus it doesn't have any elements stored.");
				}
				set
				{
				}
			}

			public int Rows
			{
				get { return _rows; }
			}

			public int Columns
			{
				get { return _columns; }
			}
		}

		private class HeaderColumnWrapper : IROVector, IReadableColumn
		{
			private IReadableColumn _col;
			private IAscendingIntegerCollection _participatingDataRows;

			internal HeaderColumnWrapper(IReadableColumn r, IAscendingIntegerCollection participatingDataRows)
			{
				_col = r;
				_participatingDataRows = participatingDataRows;
			}

			public int Length
			{
				get { return _participatingDataRows.Count; }
			}

			public double this[int i]
			{
				get { return _col[_participatingDataRows[i]]; }
			}

			AltaxoVariant IReadableColumn.this[int i]
			{
				get
				{
					if (i < 0 || i >= _participatingDataRows.Count)
						throw new ArgumentOutOfRangeException("Index");

					return _col[_participatingDataRows[i]];
				}
			}

			public bool IsElementEmpty(int i)
			{
				return false;
			}

			public string FullName
			{
				get { return this.GetType().ToString(); }
			}

			public object Clone()
			{
				throw new NotImplementedException();
			}
		}

		protected class ColumnPositionComparer : IComparer<ReadableColumnProxy>
		{
			private DataColumnCollection _coll;

			public ColumnPositionComparer(DataColumnCollection coll)
			{
				_coll = coll;
			}

			public int Compare(ReadableColumnProxy a, ReadableColumnProxy b)
			{
				var ca = a.Document as DataColumn;
				var cb = b.Document as DataColumn;

				if (ca != null && cb != null)
				{
					int na = _coll.GetColumnNumber(ca);
					int nb = _coll.GetColumnNumber(cb);
					return Comparer<int>.Default.Compare(na, nb);
				}
				if (ca == null && cb == null)
					return 0;
				else if (ca == null)
					return -1;
				else return 1;
			}
		}

		private class MyMatrixWrapper : IROMatrix
		{
			private DataColumnCollection _data;
			private IAscendingIntegerCollection _participatingCols;
			private IAscendingIntegerCollection _participatingRows;

			internal MyMatrixWrapper(DataColumnCollection coll, IAscendingIntegerCollection participatingRows, IAscendingIntegerCollection participatingCols)
			{
				_data = coll;
				_participatingRows = participatingRows;
				_participatingCols = participatingCols;
			}

			public double this[int row, int col]
			{
				get { return _data[_participatingCols[col]][_participatingRows[row]]; }
			}

			public int Rows
			{
				get { return _participatingRows.Count; }
			}

			public int Columns
			{
				get { return _participatingCols.Count; }
			}
		}

		#endregion Inner classes

		[NonSerialized]
		protected object _parent;

		[field: NonSerialized]
		public event EventHandler Changed;

		/// <summary><c>True</c> if the data are inconsistent. To bring the data in a consistent state <see cref="Update"/> method must be called then.</summary>
		protected bool _isDirty;

		/// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
		protected DataTableProxy _dataTable;

		protected List<ReadableColumnProxy> _dataColumns; // the columns that are involved in the matrix

		/// <summary>The group number of the data columns. All data columns must be columns of ColumnKind.V and must have this group number. Data columns having other group numbers will be removed.</summary>
		protected int _groupNumber;

		/// <summary>Column that correlate each row of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
		protected ReadableColumnProxy _rowHeaderColumn;

		/// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
		protected ReadableColumnProxy _columnHeaderColumn;

		/// <summary>If <c>true</c>, all available columns (of ColumnKind.V) with the group number of <see cref="_groupNumber"/> will be used for the data matrix. If columns with this group number are removed or added from/to the table, the number of columns of the matrix will be adjusted.</summary>
		protected bool _useAllAvailableColumnsOfGroup;

		/// <summary>If <c>true</c>, all available rows (of the columns that contribute to the matrix) will be included in the matrix.</summary>
		protected bool _useAllAvailableDataRows;

		/// <summary>The indices of the data columns that contribute to the matrix.</summary>
		protected AscendingIntegerCollection _participatingDataColumns;

		/// <summary>The indices of the data rows that contribute to the matrix.</summary>
		protected AscendingIntegerCollection _participatingDataRows;

		/// <summary>
		/// Copies data from another instance of <see cref="DataTableMatrixProxy"/>.
		/// </summary>
		/// <param name="obj">The instance.</param>
		/// <returns><c>True</c> if any data could be copyied.</returns>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as DataTableMatrixProxy;
			if (null == from)
				return false;

			InternalSetDataTable((DataTableProxy)from._dataTable.Clone());
			InternalSetDataColumnsWithCloning(from._dataColumns);
			InternalSetRowHeaderColumn((ReadableColumnProxy)from._rowHeaderColumn.Clone());
			InternalSetColumnHeaderColumn((ReadableColumnProxy)from._columnHeaderColumn.Clone());
			this._groupNumber = from._groupNumber;
			this._useAllAvailableColumnsOfGroup = from._useAllAvailableColumnsOfGroup;
			this._useAllAvailableDataRows = from._useAllAvailableDataRows;
			_participatingDataRows = (AscendingIntegerCollection)from._participatingDataRows.Clone();
			_participatingDataColumns = (AscendingIntegerCollection)from._participatingDataColumns.Clone();
			_isDirty = from._isDirty;

			return true;
		}

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2014-07-08 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableMatrixProxy), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DataTableMatrixProxy)obj;
				info.AddValue("Table", s._dataTable);
				info.AddValue("Group", s._groupNumber);
				info.AddValue("RowHeaderColumn", s._rowHeaderColumn);
				info.AddValue("ColumnHeaderColumn", s._columnHeaderColumn);
				info.AddValue("UseAllAvailableColumnsOfGroup", s._useAllAvailableColumnsOfGroup);
				info.AddValue("UseAllAvailableDataRows", s._useAllAvailableDataRows);

				if (!s._useAllAvailableColumnsOfGroup)
				{
					info.CreateArray("DataColumns", s._dataColumns.Count);
					for (int i = 0; i < s._dataColumns.Count; ++i)
					{
						info.AddValue("e", s._dataColumns[i]);
					}
					info.CommitArray();
				}

				if (!s._useAllAvailableDataRows)
				{
					info.AddValue("DataRows", s._participatingDataRows);
				}
			}

			protected virtual DataTableMatrixProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new DataTableMatrixProxy() : (DataTableMatrixProxy)o);

				s.InternalSetDataTable((DataTableProxy)info.GetValue("Table"));
				s._groupNumber = info.GetInt32("Group");
				s.InternalSetRowHeaderColumn((ReadableColumnProxy)info.GetValue("RowHeaderColumn"));
				s.InternalSetColumnHeaderColumn((ReadableColumnProxy)info.GetValue("ColumnHeaderColumn"));

				s._useAllAvailableColumnsOfGroup = info.GetBoolean("UseAllAvailableColumnsOfGroup");
				s._useAllAvailableDataRows = info.GetBoolean("UseAllAvailableDataRows");

				if (!s._useAllAvailableColumnsOfGroup)
				{
					int count = info.OpenArray();
					s._dataColumns = new List<ReadableColumnProxy>(count);
					for (int i = 0; i < count; i++)
					{
						s.InternalAddDataColumnNoClone((ReadableColumnProxy)info.GetValue("e", parent));
					}
					info.CloseArray(count);
				}
				else
				{
					s._dataColumns = new List<ReadableColumnProxy>();
				}

				if (!s._useAllAvailableDataRows)
				{
					s._participatingDataRows = (AscendingIntegerCollection)info.GetValue("DataRows");
				}
				else
				{
					s._participatingDataRows = new AscendingIntegerCollection();
				}

				s._participatingDataColumns = new AscendingIntegerCollection(); // this is just to avoid NullExceptions

				s._isDirty = true;

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			var result = new DataTableMatrixProxy();
			result.CopyFrom(this);
			return result;
		}

		/// <summary>
		/// Deserialization constructor
		/// </summary>
		/// <param name="info">The information.</param>
		protected DataTableMatrixProxy()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTableMatrixProxy"/> class. The selected collections determine which columns and rows contribute to the matrix, and which
		/// row header column and column header column is used. The group number is determined by the first selected column (or, if no column is selected, by the first column of the data table).
		/// </summary>
		/// <param name="table">The underlying table.</param>
		/// <param name="selectedDataRows">The selected data rows.</param>
		/// <param name="selectedDataColumns">The selected data columns.</param>
		/// <param name="selectedPropertyColumns">The selected property columns.</param>
		/// <exception cref="System.ArgumentNullException">table must not be null.</exception>
		public DataTableMatrixProxy(DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns, IAscendingIntegerCollection selectedPropertyColumns)
		{
			if (null == table)
				throw new ArgumentNullException("table");

			_dataTable = new DataTableProxy(table);
			_dataTable.Changed += this.EhColumnDataChangedEventHandler;

			var converter = new DataTableToMatrixConverter(table)
			{
				SelectedDataRows = selectedDataRows,
				SelectedDataColumns = selectedDataColumns,
				SelectedPropertyColumns = selectedPropertyColumns,
				ReplacementValueForNaNMatrixElements = 0,
				ReplacementValueForInfiniteMatrixElements = 0,
				MatrixGenerator = DoubleMatrixAsNullDevice.GetMatrix // the data are not needed, thus we send it into a NullDevice
			};

			converter.Execute();

			_groupNumber = converter.DataColumnsGroupNumber;
			_useAllAvailableColumnsOfGroup = converter.AreAllAvailableColumnsOfGroupIncluded();
			_useAllAvailableDataRows = converter.AreAllAvailableRowsIncluded();

			_rowHeaderColumn = new ReadableColumnProxy(converter.RowHeaderColumn);
			_rowHeaderColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

			_columnHeaderColumn = new ReadableColumnProxy(converter.ColumnHeaderColumn);
			_columnHeaderColumn.Changed += new EventHandler(EhColumnDataChangedEventHandler);

			_dataColumns = new List<ReadableColumnProxy>();
			_participatingDataColumns = new AscendingIntegerCollection(converter.GetParticipatingDataColumns());
			for (int i = 0; i < _participatingDataColumns.Count; i++)
			{
				_dataColumns.Add(new ReadableColumnProxy(table.DataColumns[_participatingDataColumns[i]]));

				// set the event chain
				_dataColumns[i].Changed += new EventHandler(EhColumnDataChangedEventHandler);
			}

			_participatingDataRows = new AscendingIntegerCollection(converter.GetParticipatingDataRows());
		}

		[Obsolete("This is intended for legacy deserialization (of XYZMeshedColumnPlotData) only.")]
		public static DataTableMatrixProxy CreateEmptyInstance()
		{
			var result = new DataTableMatrixProxy();
			result._participatingDataColumns = new AscendingIntegerCollection();
			result._participatingDataRows = new AscendingIntegerCollection();
			result._dataColumns = new List<ReadableColumnProxy>();

			result.InternalSetDataTable(new DataTableProxy((DataTable)null));
			result.InternalSetRowHeaderColumn(new ReadableColumnProxy((IReadableColumn)null));
			result.InternalSetColumnHeaderColumn(new ReadableColumnProxy((IReadableColumn)null));

			return result;
		}

		[Obsolete("This is intended for legacy deserialization (of XYZMeshedColumnPlotData) only.")]
		public DataTableMatrixProxy(ReadableColumnProxy xColumn, ReadableColumnProxy yColumn, ReadableColumnProxy[] dataColumns)
		{
			_participatingDataColumns = new AscendingIntegerCollection();
			_participatingDataRows = new AscendingIntegerCollection();
			_dataColumns = new List<ReadableColumnProxy>();

			InternalSetDataTable(new DataTableProxy((DataTable)null));
			InternalSetRowHeaderColumn(xColumn);
			InternalSetColumnHeaderColumn(yColumn);
			InternalSetDataColumnsWithCloning(dataColumns);
			_useAllAvailableDataRows = true;
			_isDirty = true;
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
		{
			Report(_dataTable, this, "DataTable");
			Report(_rowHeaderColumn, this, "RowHeaderColumn");
			Report(_columnHeaderColumn, this, "ColumnHeaderColumn");
			for (int i = 0; i < _dataColumns.Count; ++i)
				Report(_dataColumns[i], this, string.Format("DataColumns[{0}]", i));
		}

		#region Setters for event wired members

		private void InternalSetDataTable(DataTableProxy proxy)
		{
			if (null != _dataTable)
				_dataTable.Changed -= EhColumnDataChangedEventHandler;

			_dataTable = proxy ?? new DataTableProxy((DataTable)null);

			if (null != _dataTable)
				_dataTable.Changed += EhColumnDataChangedEventHandler;
		}

		private void InternalSetRowHeaderColumn(ReadableColumnProxy proxy)
		{
			if (null != _rowHeaderColumn)
				_rowHeaderColumn.Changed -= EhColumnDataChangedEventHandler;

			_rowHeaderColumn = proxy ?? new ReadableColumnProxy((IReadableColumn)null); // always ensure to have a proxy != null

			if (null != _rowHeaderColumn)
				_rowHeaderColumn.Changed += EhColumnDataChangedEventHandler;
		}

		private void InternalSetColumnHeaderColumn(ReadableColumnProxy proxy)
		{
			if (null != _columnHeaderColumn)
				_columnHeaderColumn.Changed -= EhColumnDataChangedEventHandler;

			_columnHeaderColumn = proxy ?? new ReadableColumnProxy((IReadableColumn)null);

			if (null != _columnHeaderColumn)
				_columnHeaderColumn.Changed += EhColumnDataChangedEventHandler;
		}

		/// <summary>
		/// Clear data columns collection, removes the event handlers from the data column proxies.
		/// </summary>
		private void InternalClearDataColumns()
		{
			if (null != _dataColumns)
			{
				foreach (var proxy in _dataColumns)
					proxy.Changed -= EhColumnDataChangedEventHandler;

				_dataColumns.Clear();
			}
		}

		/// <summary>
		/// Adds a data column proxy to the data column collection without cloning it (i.e. the proxy is directly added).
		/// </summary>
		/// <param name="proxy">The proxy.</param>
		private void InternalAddDataColumnNoClone(ReadableColumnProxy proxy)
		{
			if (null != proxy)
			{
				_dataColumns.Add(proxy);
				proxy.Changed += EhColumnDataChangedEventHandler;
			}
		}

		/// <summary>
		/// Removes the data column proxy at index <paramref name="idx"/>, removing the Changed event handler.
		/// </summary>
		/// <param name="idx">The index.</param>
		private void InternalRemoveDataColumnAt(int idx)
		{
			_dataColumns[idx].Changed -= EhColumnDataChangedEventHandler;
			_dataColumns.RemoveAt(idx);
		}

		/// <summary>
		/// Clears the data column collection, then adds data column proxies, using a list of existing data column proxies. The proxies are cloned before adding them to the collection.
		/// </summary>
		/// <param name="fromList">The enumeration of data proxies to clone.</param>
		private void InternalSetDataColumnsWithCloning(IEnumerable<ReadableColumnProxy> fromList)
		{
			if (null != _dataColumns)
			{
				InternalClearDataColumns();
			}
			else
			{
				_dataColumns = new List<ReadableColumnProxy>();
			}

			foreach (var fromMember in fromList)
			{
				var clone = (ReadableColumnProxy)fromMember.Clone();
				clone.Changed += this.EhColumnDataChangedEventHandler;
				_dataColumns.Add(clone);
			}
		}

		#endregion Setters for event wired members

		#region Properties

		/// <summary>
		/// Gets or sets the parent object.
		/// </summary>
		/// <value>
		/// The parent object.
		/// </value>
		public object ParentObject { get { return _parent; } set { _parent = value; } }

		/// <summary>
		/// Gets or sets the underlying data table.
		/// </summary>
		/// <value>
		/// The data table.
		/// </value>
		public DataTable DataTable
		{
			get
			{
				if (_isDirty)
					Update();

				return _dataTable.Document;
			}
			set
			{
				var oldValue = _dataTable.Document;
				if (!object.ReferenceEquals(oldValue, value))
				{
					InternalSetDataTable(new DataTableProxy(value));
					_isDirty = true;
				}
			}
		}

		/// <summary>The group number of the data columns. All data columns must be columns of ColumnKind.V and must have this group number. Data columns having other group numbers will be removed.</summary>
		public int GroupNumber
		{
			get
			{
				return _groupNumber;
			}
			set
			{
				var oldValue = _groupNumber;
				_groupNumber = value;
				if (oldValue != value)
				{
					_isDirty = true;
				}
			}
		}

		/// <summary>Column that correlate each row of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
		public IReadableColumn RowHeaderColumn
		{
			get
			{
				return _rowHeaderColumn.Document;
			}
			set
			{
				var oldValue = _rowHeaderColumn.Document;
				if (!object.ReferenceEquals(oldValue, value))
				{
					InternalSetRowHeaderColumn(new ReadableColumnProxy(value));
					_isDirty = true;
				}
			}
		}

		/// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
		public IReadableColumn ColumnHeaderColumn
		{
			get
			{
				return _columnHeaderColumn.Document;
			}
			set
			{
				var oldValue = _columnHeaderColumn.Document;
				if (!object.ReferenceEquals(oldValue, value))
				{
					InternalSetColumnHeaderColumn(new ReadableColumnProxy(value));
					_isDirty = true;
				}
			}
		}

		/// <summary>
		/// Adds a column that contributes to the matrix.
		/// </summary>
		/// <param name="column">Column to add. Must have ColumnKind.V and a group number equal to <see cref="GroupNumber"/>. Otherwise, this column will be removed in the next call to <see cref="Update"/>.</param>
		public void AddDataColumn(IReadableColumn column)
		{
			if (null != column)
			{
				InternalAddDataColumnNoClone(new ReadableColumnProxy(column));
				_isDirty = true;
			}
		}

		/// <summary>
		/// Sets the data columns from an enumeration of data column proxies.
		/// </summary>
		/// <param name="dataColumnProxies">The enumeration of data column proxies. The proxies will be cloned before they are added to the data column collection.</param>
		public void SetDataColumns(IEnumerable<ReadableColumnProxy> dataColumnProxies)
		{
			InternalClearDataColumns();
			InternalSetDataColumnsWithCloning(dataColumnProxies);
			_isDirty = true;
		}

		/// <summary>
		/// Gets the data column proxy at index <paramref name="idx"/>.
		/// </summary>
		/// <param name="idx">The index.</param>
		/// <returns>The data column proxy at index <paramref name="idx"/>.</returns>
		public ReadableColumnProxy GetDataColumnProxy(int idx)
		{
			if (_isDirty)
				Update();

			return _dataColumns[idx];
		}

		/// <summary>If <c>true</c>, all available rows (of the columns that contribute to the matrix) will be included in the matrix.</summary>
		public bool UseAllAvailableDataRows
		{
			get { return _useAllAvailableDataRows; }
			set
			{
				var oldValue = _useAllAvailableDataRows;
				_useAllAvailableDataRows = value;
				if (oldValue != value)
					_isDirty = true;
			}
		}

		/// <summary>The indices of the data columns that contribute to the matrix.</summary>
		public bool UseAllAvailableDataColumnsOfGroup
		{
			get { return _useAllAvailableColumnsOfGroup; }
			set
			{
				var oldValue = _useAllAvailableColumnsOfGroup;
				_useAllAvailableColumnsOfGroup = value;
				if (oldValue != value)
					_isDirty = true;
			}
		}

		/// <summary>Get the indices of the data columns that contribute to the matrix.</summary>
		public IAscendingIntegerCollection ParticipatingDataColumns
		{
			get
			{
				if (_isDirty)
				{
					Update();
				}

				if (null == _participatingDataColumns)
					_participatingDataColumns = new AscendingIntegerCollection();

				return _participatingDataColumns;
			}
		}

		/// <summary>Gets the indices of the data rows that contribute to the matrix.</summary>
		public IAscendingIntegerCollection ParticipatingDataRows
		{
			get
			{
				if (_isDirty)
				{
					Update();
				}

				return _participatingDataRows;
			}
		}

		/// <summary>
		/// Gets the number of rows of the resulting matrix.
		/// </summary>
		/// <value>
		/// The number of rows of the resulting matrix.
		/// </value>
		public int RowCount
		{
			get
			{
				if (_isDirty)
				{
					Update();
				}

				return _isDirty ? 0 : _participatingDataRows.Count;
			}
		}

		/// <summary>
		/// Gets the number of columns of the resulting matrix.
		/// </summary>
		/// <value>
		/// The number of columns of the resulting matrix.
		/// </value>
		public int ColumnCount
		{
			get
			{
				if (_isDirty)
				{
					Update();
				}
				return _isDirty ? 0 : _participatingDataColumns.Count;
			}
		}

		#endregion Properties

		/// <summary>
		/// Called when any of the data column proxies or the table proxies reports a change.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void EhColumnDataChangedEventHandler(object sender, EventArgs e)
		{
			_isDirty = true;
			OnChanged();
		}

		/// <summary>
		/// Called when anything inside this proxy has changed.
		/// </summary>
		protected virtual void OnChanged()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

			if (null != Changed)
				Changed(this, EventArgs.Empty);
		}

		/// <summary>
		/// Removes all data columns, whose parent is not the data table <paramref name="table"/>, or whose column kind is not ColumnKind.V, or whose group number is not equal to <see cref="GroupNumber"/>.
		/// </summary>
		/// <param name="table">The table to compare the parents of the columns with.</param>
		protected void InternalRemoveDataColumnsWithDeviatingParentOrKindOrGroupNumber(DataTable table)
		{
			var tableDataColumns = table.DataColumns;

			var indicesToRemove = new List<int>();

			for (int i = _dataColumns.Count - 1; i >= 0; --i)
			{
				if (_dataColumns[i].IsEmpty)
				{
					InternalRemoveDataColumnAt(i);
					continue;
				}

				var c = _dataColumns[i].Document as DataColumn;
				if (c == null)
					continue; // not yet resolved, leave it as it is

				var coll = DataColumnCollection.GetParentDataColumnCollectionOf(c);
				if (null == coll || !object.ReferenceEquals(coll, tableDataColumns))
				{
					InternalRemoveDataColumnAt(i);
					continue;
				}

				var group = tableDataColumns.GetColumnGroup(c);
				var kind = tableDataColumns.GetColumnKind(c);
				if (group != _groupNumber && kind != ColumnKind.V)
				{
					InternalRemoveDataColumnAt(i);
					continue;
				}
			}
		}

		/// <summary>
		/// Should be called only if <see cref="_useAllAvailableColumnsOfGroup"/> is <c>true</c>. Adds all missing data columns that have a group number of <see cref="GroupNumber"/> and ColumnKind.V.
		/// </summary>
		/// <param name="table">The table to search.</param>
		protected virtual void InternalAddMissingDataColumnsIfAllDataColumnsShouldBeIncluded(DataTable table)
		{
			var dataColumns = table.DataColumns;
			var existing = new HashSet<DataColumn>(_dataColumns.Select(x => x.Document as DataColumn).Where(c => null != c));
			var toInsert = dataColumns.Columns.Where(c => dataColumns.GetColumnGroup(c) == _groupNumber && dataColumns.GetColumnKind(c) == ColumnKind.V && !existing.Contains(c));
			foreach (var ins in toInsert)
			{
				InternalAddDataColumnNoClone(new ReadableColumnProxy(ins));
			}
		}

		/// <summary>
		/// Sorts our internal data column collection by group number (ascending).
		/// </summary>
		/// <param name="table">The table.</param>
		private void InternalSortDataColumnsByColumnNumber(DataTable table)
		{
			// now sort the columns according to their occurence
			_dataColumns.Sort(new ColumnPositionComparer(table.DataColumns));
		}

		/// <summary>
		/// Updates the indices of the participating data rows. This means for instance, that some of the indices are removed, if the column count of the participating columns becomes lesser. Also,
		/// indices could be added, if <see cref="_useAllAvailableDataRows"/> is <c>true</c> and some of the data columns get expanded.
		/// </summary>
		private void InternalUpdateParticipatingDataRows()
		{
			// see if the row data range is still valid
			int maxRowCountNow = GetMaximumRowCountNow();
			var maxRowCountPrev = _participatingDataRows.Count > 0 ? _participatingDataRows[_participatingDataRows.Count - 1] + 1 : 0;
			if (_useAllAvailableDataRows)
			{
				if (maxRowCountNow > maxRowCountPrev)
					_participatingDataRows.AddRange(maxRowCountPrev, maxRowCountNow - maxRowCountPrev);
				else if (maxRowCountNow < maxRowCountPrev)
					_participatingDataRows.RemoveAllAbove(maxRowCountNow - 1);
			}
			else
			{
				// we make the row selection only smaller, but never wider
				if (maxRowCountNow < maxRowCountPrev)
					_participatingDataRows.RemoveAllAbove(maxRowCountNow - 1);
			}
		}

		/// <summary>
		/// Updates the collection of indices of the participating data columns.
		/// </summary>
		/// <param name="table">The table.</param>
		private void InternalUpdateParticipatingDataColumnIndices(DataTable table)
		{
			_participatingDataColumns.Clear();

			// evaluate the participating data columns (we hereby assume that all DataColumns are part of our DataTable

			for (int i = 0; i < _dataColumns.Count; ++i)
			{
				var col = _dataColumns[i].Document as DataColumn;
				_participatingDataColumns.Add(table.DataColumns.GetColumnNumber(col));
			}
		}

		/// <summary>
		/// Gets the maximum row count of the data columns in the data column collection <see cref="_dataColumns"/>.
		/// </summary>
		/// <returns></returns>
		private int GetMaximumRowCountNow()
		{
			return _dataColumns.Where(p => p.Document != null).Max(p => p.Document is IDefinedCount ? ((IDefinedCount)p.Document).Count : 0);
		}

		/// <summary>
		/// Tries to get a reference to the underlying table from the data columns. Used if no table was known beforehand (mainly after legacy deserialization).
		/// </summary>
		private void TryGetDataTableProxyFromColumns()
		{
			DataColumn col;
			DataTable table;
			foreach (var colproxy in _dataColumns)
			{
				col = colproxy.Document as DataColumn;
				if (null != col)
				{
					table = DataTable.GetParentDataTableOf(col);
					if (null != table)
						_dataTable = new DataTableProxy(table);
				}
			}

			foreach (var colproxy in new ReadableColumnProxy[] { _rowHeaderColumn, _columnHeaderColumn })
			{
				col = colproxy.Document as DataColumn;
				if (null != col)
				{
					table = DataTable.GetParentDataTableOf(col);
					if (null != table)
						_dataTable = new DataTableProxy(table);
				}
			}
		}

		/// <summary>
		/// Brings the members of this instance in a consistent state. After the state is consistent, the member <see cref="_isDirty"/> is set to <c>false</c>.
		/// This fails if the underlying data table is null, and can not be determined from the data columns.
		/// </summary>
		protected void Update()
		{
			if (!_isDirty)
				return;

			if (_dataTable.IsEmpty)
				TryGetDataTableProxyFromColumns(); // legacy, for instance from old XYZMeshedColumnPlotData, we have not stored the table reference

			DataTable table = _dataTable.Document;
			if (_dataTable.IsEmpty)
				return;

			InternalRemoveDataColumnsWithDeviatingParentOrKindOrGroupNumber(table);

			if (_useAllAvailableColumnsOfGroup)
			{
				InternalAddMissingDataColumnsIfAllDataColumnsShouldBeIncluded(table);
			}

			InternalSortDataColumnsByColumnNumber(table);

			InternalUpdateParticipatingDataRows();

			InternalUpdateParticipatingDataColumnIndices(table);

			_isDirty = false;
		}

		#region Result functions

		/// <summary>
		/// Gets the matrix as writeable matrix of <see cref="T:System.Double"/> values. The parameter <paramref name="matrixGenerator"/> is used to generate a matrix.
		/// </summary>
		/// <param name="matrixGenerator">The matrix generator. The two parameters are the number of rows and the number of columns of the matrix. The function has to return a writeable matrix.</param>
		/// <returns>A matrix with the data this proxy refers to.</returns>
		public IMatrix GetMatrix(Func<int, int, IMatrix> matrixGenerator)
		{
			if (_isDirty)
			{
				Update();
			}

			var table = _dataTable.Document;
			int rowCount = RowCount;
			int columnCount = ColumnCount;

			var matrix = matrixGenerator(rowCount, columnCount);

			for (int c = 0; c < columnCount; ++c)
			{
				var col = table.DataColumns[_participatingDataColumns[c]];

				for (int r = 0; r < rowCount; ++r)
					matrix[r, c] = col[_participatingDataRows[r]];
			}
			return matrix;
		}

		/// <summary>
		/// Gets the matrix as read-only matrix of <see cref="T:System.Double"/> values. The returned matrix is only a wrapper around data hold by this instance, so that the data will change, if anything
		/// in this instance is changed. Intended for short time usage only.
		/// </summary>
		/// <returns>A readonly matrix with the data this proxy refers to.</returns>
		public IROMatrix GetMatrixWrapper()
		{
			if (_isDirty)
			{
				Update();
			}

			var table = _dataTable.Document;
			return new MyMatrixWrapper(table.DataColumns, _participatingDataRows, _participatingDataColumns);
		}

		/// <summary>
		/// Gets a wrapper vector around the row header data.
		/// </summary>
		/// <returns>Wrapper vector around the row header data. Each element of this vector corresponds to the row with the same index of the matrix.</returns>
		public IROVector GetRowHeaderWrapper()
		{
			if (_rowHeaderColumn.IsEmpty || _rowHeaderColumn.Document == null)
				return VectorMath.CreateEquidistantSequenceByStartStepLength(0, 1, _participatingDataRows.Count);
			else
				return new HeaderColumnWrapper(_rowHeaderColumn.Document, _participatingDataRows);
		}

		/// <summary>
		/// Gets a wrapper vector around the column header data.
		/// </summary>
		/// <returns>Wrapper vector around the column header data. Each element of this vector corresponds to the column with the same index of the matrix.</returns>
		public IROVector GetColumnHeaderWrapper()
		{
			if (_columnHeaderColumn.IsEmpty || _columnHeaderColumn.Document == null)
				return VectorMath.CreateEquidistantSequenceByStartStepLength(0, 1, _participatingDataColumns.Count);
			else
				return new HeaderColumnWrapper(_columnHeaderColumn.Document, _participatingDataColumns);
		}

		/// <summary>
		/// Gets wrappers for the matrix and for the row and column header values. The row and column header values are transformed (for instance to logical values and selected by means of corresponding functions.
		/// </summary>
		/// <param name="TransformRowHeaderValues">The function to transform row header values.</param>
		/// <param name="SelectTransformedRowHeaderValues">The function to select the transformed row header values.</param>
		/// <param name="TransformColumnHeaderValues">The function to transform column header values.</param>
		/// <param name="SelectTransformedColumnHeaderValues">The function to select the transformed column header values.</param>
		/// <param name="resultantMatrix">The resultant matrix.</param>
		/// <param name="resultantTransformedRowHeaderValues">The resultant transformed row header values.</param>
		/// <param name="resultantTransformedColumnHeaderValues">The resultant transformed column header values.</param>
		public void GetWrappers(Func<AltaxoVariant, double> TransformRowHeaderValues, Func<double, bool> SelectTransformedRowHeaderValues, Func<AltaxoVariant, double> TransformColumnHeaderValues, Func<double, bool> SelectTransformedColumnHeaderValues, out IROMatrix resultantMatrix, out IROVector resultantTransformedRowHeaderValues, out IROVector resultantTransformedColumnHeaderValues)
		{
			if (_isDirty)
			{
				Update();
			}

			var table = _dataTable.Document;

			var transformedAndSelectedRowHeaderValues = new List<double>();
			var participatingDataRowsSelectedNow = new AscendingIntegerCollection();

			var numRows = _participatingDataRows.Count;
			var rowHeaderWrapper = (IReadableColumn)GetRowHeaderWrapper();
			for (int i = 0; i < numRows; ++i)
			{
				var transformed = TransformRowHeaderValues(rowHeaderWrapper[i]);
				var included = SelectTransformedRowHeaderValues(transformed);
				if (included)
				{
					participatingDataRowsSelectedNow.Add(_participatingDataRows[i]);
					transformedAndSelectedRowHeaderValues.Add(transformed);
				}
			}

			var transformedAndSelectedColumnHeaderValues = new List<double>();
			var participatingDataColumnsSelectedNow = new AscendingIntegerCollection();

			int numColumns = _participatingDataColumns.Count;
			var colHeaderWrapper = (IReadableColumn)GetColumnHeaderWrapper();
			for (int i = 0; i < numColumns; ++i)
			{
				var transformed = TransformColumnHeaderValues(colHeaderWrapper[i]);
				var included = SelectTransformedColumnHeaderValues(transformed);
				if (included)
				{
					participatingDataColumnsSelectedNow.Add(_participatingDataColumns[i]);
					transformedAndSelectedColumnHeaderValues.Add(transformed);
				}
			}

			System.Diagnostics.Debug.Assert(participatingDataRowsSelectedNow.Count == transformedAndSelectedRowHeaderValues.Count);
			System.Diagnostics.Debug.Assert(participatingDataColumnsSelectedNow.Count == transformedAndSelectedColumnHeaderValues.Count);

			resultantMatrix = new MyMatrixWrapper(table.DataColumns, participatingDataRowsSelectedNow, participatingDataColumnsSelectedNow);
			resultantTransformedRowHeaderValues = VectorMath.ToROVector(transformedAndSelectedRowHeaderValues.ToArray());
			resultantTransformedColumnHeaderValues = VectorMath.ToROVector(transformedAndSelectedColumnHeaderValues.ToArray());
		}

		/// <summary>
		/// Performs an action on each matrix element.
		/// </summary>
		/// <param name="action">The action to perform. The first parameter is a column of the matrix, the second parameter is the index into this columnm. The indices correspond to the list of participating row indices.</param>
		public void ForEachMatrixElementDo(Action<IReadableColumn, int> action)
		{
			if (_isDirty)
			{
				Update();
			}

			var table = _dataTable.Document;
			int rowCount = _participatingDataRows.Count;
			int columnCount = _participatingDataColumns.Count;

			for (int c = 0; c < columnCount; ++c)
			{
				var col = table.DataColumns[_participatingDataColumns[c]];

				for (int r = 0; r < rowCount; ++r)
					action(col, _participatingDataRows[r]);
			}
		}

		/// <summary>
		/// Performs an action on each row header element.
		/// </summary>
		/// <param name="action">The action to perform. First parameter is the row header colum, the second is the index into this column. The indices correspond to the list of participating row indices.</param>
		public void ForEachRowHeaderElementDo(Action<IReadableColumn, int> action)
		{
			if (_isDirty)
			{
				Update();
			}

			var col = _rowHeaderColumn.Document;

			if (null != col)
			{
				int rowCount = _participatingDataRows.Count;
				for (int r = 0; r < rowCount; ++r)
					action(col, _participatingDataRows[r]);
			}
		}

		/// <summary>
		/// Performs an action on each element of the column header column.
		/// </summary>
		/// <param name="action">The action to perform. The first parameter is the column header column, the second parameter is the index into this column. The indices correspond to the list of participating column indices.</param>
		public void ForEachColumnHeaderElementDo(Action<IReadableColumn, int> action)
		{
			if (_isDirty)
			{
				Update();
			}

			var col = _columnHeaderColumn.Document;

			if (null != col)
			{
				int columnCount = _participatingDataColumns.Count;
				for (int c = 0; c < columnCount; ++c)
					action(col, _participatingDataColumns[c]);
			}
		}

		#endregion Result functions

		#region Public helper functions

		/// <summary>
		/// Tries the get the uniform space between elements of a header column. This will fail if the data of the column are not uniformly spaced. In this case a user friendly error message is returned.
		/// </summary>
		/// <param name="proxy">The proxy of the column to investigate.</param>
		/// <param name="rowOrCol">Indicates if this column is a row header column (value: "row") or a column header column (value: "column").</param>
		/// <param name="selectedIndices">The indices into the provided column.</param>
		/// <param name="incrementValue">If the function is successfull, the value of the spacing between each element is returned.</param>
		/// <param name="errorOrWarningMessage">If the function is not successfull, a user friendly error or warning message is returned here.</param>
		/// <returns><c>True</c> if successfull, otherwise <c>false</c>.</returns>
		public static bool TryGetColumnDataIncrement(ReadableColumnProxy proxy, string rowOrCol, IAscendingIntegerCollection selectedIndices, out double incrementValue, out string errorOrWarningMessage)
		{
			incrementValue = 1;

			if (null == proxy || proxy.IsEmpty)
			{
				errorOrWarningMessage = string.Format("No {0} header column chosen.", rowOrCol);
				return false;
			}

			var col = proxy.Document;
			if (null == col)
			{
				errorOrWarningMessage = string.Format("Link to {0} header column is lost.", rowOrCol);
				return false;
			}

			var xCol = col as INumericColumn;
			if (null == xCol)
			{
				errorOrWarningMessage = string.Format("The {0} header column is not a numeric column, thus the increment value could not be evaluated.", rowOrCol);
				return false;
			}

			var vector = xCol.ToROVector(selectedIndices);
			var spacing = new Calc.LinearAlgebra.VectorSpacingEvaluator(vector);

			if (!spacing.IsStrictlyMonotonicIncreasing)
			{
				errorOrWarningMessage = string.Format("The {0} header column is not strictly monotonically increasing", rowOrCol);
				incrementValue = spacing.SpaceMeanValue;
				return false;
			}

			incrementValue = spacing.SpaceMeanValue;

			if (!spacing.IsStrictlyEquallySpaced)
			{
				errorOrWarningMessage = string.Format("Warning: The {0} header column is not strictly equally spaced, the relative deviation is " + spacing.RelativeSpaceDeviation.ToString());
				return false;
			}
			else
			{
				errorOrWarningMessage = null;
				return true;
			}
		}

		/// <summary>
		/// Tries to the get the uniform spacing value between elements of the row header column.
		/// </summary>
		/// <param name="incrementValue">If the function is successfull, the value of the spacing between each element is returned.</param>
		/// <param name="errorOrWarningMessage">If the function is not successfull, a user friendly error or warning message is returned here.</param>
		/// <returns><c>True</c> if successfull, otherwise <c>false</c>.</returns>
		public bool TryGetRowHeaderIncrement(out double incrementValue, out string errorOrWarningMessage)
		{
			if (_isDirty)
			{
				Update();
			}

			return TryGetColumnDataIncrement(_rowHeaderColumn, "row", _participatingDataRows, out incrementValue, out errorOrWarningMessage);
		}

		/// <summary>
		/// Tries to the get the uniform spacing value between elements of the column header column.
		/// </summary>
		/// <summary>
		/// Tries to the get the uniform spacing value between elements of a row header column.
		/// </summary>
		/// <param name="incrementValue">If the function is successfull, the value of the spacing between each element is returned.</param>
		/// <param name="errorOrWarningMessage">If the function is not successfull, a user friendly error or warning message is returned here.</param>
		/// <returns><c>True</c> if successfull, otherwise <c>false</c>.</returns>
		public bool TryGetColumnHeaderIncrement(out double incrementValue, out string errorOrWarningMessage)
		{
			if (_isDirty)
			{
				Update();
			}

			return TryGetColumnDataIncrement(_columnHeaderColumn, "column", _participatingDataColumns, out incrementValue, out errorOrWarningMessage);
		}

		#endregion Public helper functions
	}
}