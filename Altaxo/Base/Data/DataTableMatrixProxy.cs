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

		#endregion Inner classes

		[NonSerialized]
		protected object _parent;

		[field: NonSerialized]
		public event EventHandler Changed;

		protected bool _isDirty;
		protected DataTableProxy _dataTable;
		protected List<ReadableColumnProxy> _dataColumns; // the columns that are involved in the matrix
		protected ReadableColumnProxy _rowHeaderColumn;
		protected ReadableColumnProxy _columnHeaderColumn;
		protected int _groupNumber;
		protected bool _useAllAvailableColumnsOfGroup;
		protected bool _useAllAvailableDataRows;

		protected AscendingIntegerCollection _participatingDataRows;
		protected AscendingIntegerCollection _participatingDataColumns;

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as DataTableMatrixProxy;
			if (null == from)
				return false;

			InternalSetDataTable((DataTableProxy)from._dataTable.Clone());
			InternalCopyDataColumns(from._dataColumns);
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

		public object Clone()
		{
			var result = new DataTableMatrixProxy();
			result.CopyFrom(this);
			return result;
		}

		protected DataTableMatrixProxy()
		{
		}

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

		#region Setters for event wired members

		private void InternalSetDataTable(DataTableProxy proxy)
		{
			if (null != _dataTable)
				_dataTable.Changed -= EhColumnDataChangedEventHandler;

			_dataTable = proxy;

			if (null != _dataTable)
				_dataTable.Changed += EhColumnDataChangedEventHandler;
		}

		private void InternalSetRowHeaderColumn(ReadableColumnProxy proxy)
		{
			if (null != _rowHeaderColumn)
				_rowHeaderColumn.Changed -= EhColumnDataChangedEventHandler;

			_rowHeaderColumn = proxy;

			if (null != _rowHeaderColumn)
				_rowHeaderColumn.Changed += EhColumnDataChangedEventHandler;
		}

		private void InternalSetColumnHeaderColumn(ReadableColumnProxy proxy)
		{
			if (null != _columnHeaderColumn)
				_columnHeaderColumn.Changed -= EhColumnDataChangedEventHandler;

			_columnHeaderColumn = proxy;

			if (null != _columnHeaderColumn)
				_columnHeaderColumn.Changed += EhColumnDataChangedEventHandler;
		}

		private void InternalClearDataColumns()
		{
			if (null != _dataColumns)
			{
				foreach (var proxy in _dataColumns)
					proxy.Changed -= EhColumnDataChangedEventHandler;

				_dataColumns.Clear();
			}
		}

		private void InternalAddDataColumnNoClone(ReadableColumnProxy proxy)
		{
			_dataColumns.Add(proxy);
			proxy.Changed += EhColumnDataChangedEventHandler;
		}

		private void InternalRemoveDataColumnAt(int idx)
		{
			_dataColumns[idx].Changed -= EhColumnDataChangedEventHandler;
			_dataColumns.RemoveAt(idx);
		}

		private void InternalCopyDataColumns(IEnumerable<ReadableColumnProxy> fromList)
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

		public object ParentObject { get { return _parent; } set { _parent = value; } }

		public IReadableColumn RowHeaderColumn
		{
			get
			{
				return _rowHeaderColumn.Document;
			}
		}

		public IReadableColumn ColumnHeaderHeaderColumn
		{
			get
			{
				return _columnHeaderColumn.Document;
			}
		}

		public IAscendingIntegerCollection ParticipatingDataColumns
		{
			get
			{
				if (_isDirty)
				{
					Update();
				}

				return _participatingDataColumns;
			}
		}

		#endregion Properties

		private void EhColumnDataChangedEventHandler(object sender, EventArgs e)
		{
			_isDirty = true;
			OnChanged();
		}

		protected virtual void OnChanged()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

			if (null != Changed)
				Changed(this, EventArgs.Empty);
		}

		protected void PruneColumnsWithDeviatingParentOrKindOrGroupNumber(DataTable table)
		{
			var tableDataColumns = table.DataColumns;

			var indicesToRemove = new List<int>();

			for (int i = _dataColumns.Count - 1; i >= 0; --i)
			{
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

		private void InsertMissingDataColumns(DataTable table)
		{
			var dataColumns = table.DataColumns;
			var existing = new HashSet<DataColumn>(_dataColumns.Select(x => x.Document as DataColumn).Where(c => null != c));
			var toInsert = dataColumns.Columns.Where(c => dataColumns.GetColumnGroup(c) == _groupNumber && dataColumns.GetColumnKind(c) == ColumnKind.V && !existing.Contains(c));
			foreach (var ins in toInsert)
			{
				InternalAddDataColumnNoClone(new ReadableColumnProxy(ins));
			}
		}

		private void SortDataColumnsByColumnNumber(DataTable table)
		{
			// now sort the columns according to their occurence
			_dataColumns.Sort(new ColumnPositionComparer(table.DataColumns));
		}

		private void UpdateParticipatingDataRows()
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

		private void UpdateCollectionOfParticipatingDataColumns(DataTable table)
		{
			// evaluate the participating data columns (we hereby assume that all DataColumns are part of our DataTable
			_participatingDataColumns.Clear();
			for (int i = 0; i < _dataColumns.Count; ++i)
			{
				var col = _dataColumns[i].Document as DataColumn;
				_participatingDataColumns.Add(table.DataColumns.GetColumnNumber(col));
			}
		}

		private int GetMaximumRowCountNow()
		{
			return _dataColumns.Where(p => p.Document != null).Max(p => p.Document is IDefinedCount ? ((IDefinedCount)p.Document).Count : 0);
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
					return Comparer<int>.Default.Compare(_coll.GetColumnGroup(ca), _coll.GetColumnGroup(cb));
				if (ca == null && cb == null)
					return 0;
				else if (ca == null)
					return -1;
				else return 1;
			}
		}

		protected void Update()
		{
			if (!_isDirty)
				return;

			DataTable table = _dataTable.Document;
			if (null == table)
				return;

			PruneColumnsWithDeviatingParentOrKindOrGroupNumber(table);

			if (_useAllAvailableColumnsOfGroup)
			{
				InsertMissingDataColumns(table);
			}

			SortDataColumnsByColumnNumber(table);

			UpdateParticipatingDataRows();

			UpdateCollectionOfParticipatingDataColumns(table);

			_isDirty = false;
		}

		public IMatrix GetMatrix(Func<int, int, IMatrix> matrixGenerator)
		{
			if (_isDirty)
			{
				Update();
			}

			var table = _dataTable.Document;
			int rowCount = _participatingDataRows.Count;
			int columnCount = _participatingDataColumns.Count;

			var matrix = matrixGenerator(rowCount, columnCount);

			for (int c = 0; c < columnCount; ++c)
			{
				var col = table.DataColumns[_participatingDataColumns[c]];

				for (int r = 0; r < rowCount; ++r)
					matrix[r, c] = col[_participatingDataRows[r]];
			}
			return matrix;
		}

		#region Helper functions

		public static bool TryGetColumnDataIncrement(ReadableColumnProxy proxy, string rowOrCol, IAscendingIntegerCollection selectedIndices, out double incrementValue, out string errorOrWarningMessage)
		{
			incrementValue = 1;

			if (null == proxy)
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

		public bool TryGetRowHeaderIncrement(out double incrementValue, out string errorOrWarningMessage)
		{
			if (_isDirty)
			{
				Update();
			}

			return TryGetColumnDataIncrement(_rowHeaderColumn, "row", _participatingDataRows, out incrementValue, out errorOrWarningMessage);
		}

		public bool TryGetColumnHeaderIncrement(out double incrementValue, out string errorOrWarningMessage)
		{
			if (_isDirty)
			{
				Update();
			}

			return TryGetColumnDataIncrement(_columnHeaderColumn, "column", _participatingDataColumns, out incrementValue, out errorOrWarningMessage);
		}

		#endregion Helper functions
	}
}