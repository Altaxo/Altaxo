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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Holds reference to bundles of one or multiple data columns from the same group of a <see cref="DataTable"/>. The bundles are identified by a string identifier.
	/// </summary>
	public class DataTableMultipleColumnProxy
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		Main.ICopyFrom
	{
		#region Inner classes

		internal class ColumnBundleInfo : ICloneable
		{
			private List<IReadableColumnProxy> _dataColumns = new List<IReadableColumnProxy>();
			private int? _maximumNumberOfColumns;

			internal ColumnBundleInfo(int? maximumNumberOfColumns)
			{
				if (maximumNumberOfColumns < 1)
					throw new ArgumentOutOfRangeException("MaximumNumberOfColumns has to be a value>=1");
				_maximumNumberOfColumns = maximumNumberOfColumns;
			}

			internal ColumnBundleInfo()
				: this(int.MaxValue)
			{
			}

			internal int? MaximumNumberOfColumns { get { return _maximumNumberOfColumns; } }

			internal List<IReadableColumnProxy> DataColumns { get { return _dataColumns; } }

			public object Clone()
			{
				var result = new ColumnBundleInfo();
				result._maximumNumberOfColumns = this._maximumNumberOfColumns;
				foreach (var p in this._dataColumns)
					result._dataColumns.Add((IReadableColumnProxy)p.Clone());
				return result;
			}
		}

		#endregion Inner classes

		/// <summary><c>True</c> if the data are inconsistent. To bring the data in a consistent state <see cref="Update"/> method must be called then.</summary>
		protected bool _isDirty;

		/// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
		protected DataTableProxy _dataTable;

		/// <summary>The group number of the data columns. All data columns must be columns of ColumnKind.V and must have this group number. Data columns having other group numbers will be removed.</summary>
		protected int _groupNumber;

		/// <summary>If <c>true</c>, all available rows (of the columns that contribute to the matrix) will be included in the matrix.</summary>
		protected bool _useAllAvailableDataRows;

		/// <summary>The indices of the data rows that contribute to the matrix.</summary>
		protected AscendingIntegerCollection _participatingDataRows = new AscendingIntegerCollection();

		/// <summary>
		/// Bundles of data columns. Key is an identifier for the bundle (a name or a Guid). Value is the bundle of data columns.
		/// </summary>
		private Dictionary<string, ColumnBundleInfo> _dataColumnBundles = new Dictionary<string, ColumnBundleInfo>();

		/// <summary>
		/// Copies data from another instance of <see cref="DataTableMultipleColumnProxy"/>.
		/// </summary>
		/// <param name="obj">The instance.</param>
		/// <returns><c>True</c> if any data could be copyied.</returns>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as DataTableMultipleColumnProxy;
			if (null == from)
				return false;

			InternalSetDataTable((DataTableProxy)from._dataTable.Clone());
			InternalSetDataColumnsWithCloning(from._dataColumnBundles);
			this._groupNumber = from._groupNumber;
			this._useAllAvailableDataRows = from._useAllAvailableDataRows;
			_participatingDataRows = (AscendingIntegerCollection)from._participatingDataRows.Clone();

			_isDirty = from._isDirty;

			return true;
		}

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2014-1-30 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableMultipleColumnProxy), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DataTableMultipleColumnProxy)obj;
				info.AddValue("Table", s._dataTable);
				info.AddValue("Group", s._groupNumber);
				info.AddValue("UseAllAvailableDataRows", s._useAllAvailableDataRows);

				info.CreateArray("DataColumnsBundles", s._dataColumnBundles.Count);
				foreach (var entry in s._dataColumnBundles)
				{
					info.CreateElement("e");

					info.AddValue("Identifier", entry.Key);
					info.AddValue("MaximumNumberOfColumns", entry.Value.MaximumNumberOfColumns);
					info.CreateArray("DataColumns", entry.Value.DataColumns.Count);

					foreach (var proxy in entry.Value.DataColumns)
						info.AddValue("e", proxy);

					info.CommitArray();

					info.CommitElement();
				}
				info.CommitArray();

				if (!s._useAllAvailableDataRows)
				{
					info.AddValue("DataRows", s._participatingDataRows);
				}
			}

			protected virtual DataTableMultipleColumnProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new DataTableMultipleColumnProxy() : (DataTableMultipleColumnProxy)o);

				s.InternalSetDataTable((DataTableProxy)info.GetValue("Table", s));
				s._groupNumber = info.GetInt32("Group");

				s._useAllAvailableDataRows = info.GetBoolean("UseAllAvailableDataRows");

				int countBundles = info.OpenArray("DataColumnsBundles");
				for (int b = 0; b < countBundles; b++)
				{
					info.OpenElement();
					string identifier = info.GetString("Identifier");
					int? MaximumNumberOfColumns = info.GetNullableInt32("MaximumNumberOfColumns");

					var columnBundleInfo = new ColumnBundleInfo(MaximumNumberOfColumns);

					int countColumns = info.OpenArray();
					for (int i = 0; i < countColumns; i++)
					{
						s.InternalAddDataColumnNoClone(columnBundleInfo, (IReadableColumnProxy)info.GetValue("e", s));
					}
					info.CloseArray(countColumns);

					s._dataColumnBundles.Add(identifier, columnBundleInfo);

					info.CloseElement();
				}
				info.CloseArray(countBundles);

				if (!s._useAllAvailableDataRows)
				{
					s._participatingDataRows = (AscendingIntegerCollection)info.GetValue("DataRows", s);
				}
				else
				{
					s._participatingDataRows = new AscendingIntegerCollection();
				}

				s._isDirty = true;

				s._parent = parent as Main.IDocumentNode;
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
			var result = new DataTableMultipleColumnProxy();
			result.CopyFrom(this);
			return result;
		}

		/// <summary>
		/// Deserialization constructor
		/// </summary>
		protected DataTableMultipleColumnProxy()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTableMultipleColumnProxy"/> class.
		/// The table and group number of this instance are set, but no columns are set with this constructor.
		/// </summary>
		/// <param name="table">The underlying table.</param>
		/// <param name="groupNumber">The group number of the data columns this instance should hold.</param>
		/// <exception cref="System.ArgumentNullException">table must not be null.</exception>
		public DataTableMultipleColumnProxy(DataTable table, int groupNumber)
		{
			if (null == table)
				throw new ArgumentNullException("table");

			_dataTable = new DataTableProxy(table) { ParentObject = this };

			_groupNumber = groupNumber;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTableMultipleColumnProxy"/> class. The selected collections determine which columns and rows contribute to this instance.
		/// The group number is determined by the first selected column (or, if no column is selected, by the first column of the data table).
		/// </summary>
		/// <param name="identifier">The identifier of the bundle of columns that are initially set with this constructor.</param>
		/// <param name="table">The underlying table.</param>
		/// <param name="selectedDataRows">The selected data rows.</param>
		/// <param name="selectedDataColumns">The selected data columns.</param>
		/// <exception cref="System.ArgumentNullException">table must not be null.</exception>
		public DataTableMultipleColumnProxy(string identifier, DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
		{
			if (null == identifier)
				throw new ArgumentNullException("identifier");

			if (null == table)
				throw new ArgumentNullException("table");

			_dataColumnBundles = new Dictionary<string, ColumnBundleInfo>();

			_dataTable = new DataTableProxy(table) { ParentObject = this };

			_groupNumber = 0;

			if (null != selectedDataColumns && selectedDataColumns.Count > 0)
				_groupNumber = table.DataColumns.GetColumnGroup(table[selectedDataColumns[0]]);

			var bundle = new ColumnBundleInfo();
			_dataColumnBundles.Add(identifier, bundle);

			int maxRowCount = 0;
			if (selectedDataColumns != null && selectedDataColumns.Count > 0)
			{
				for (int i = 0; i < selectedDataColumns.Count; ++i)
				{
					var col = table[selectedDataColumns[i]];
					if (table.DataColumns.GetColumnGroup(col) == _groupNumber)
					{
						InternalAddDataColumnNoClone(bundle, ReadableColumnProxyBase.FromColumn(col));
						maxRowCount = Math.Max(maxRowCount, col.Count);
					}
				}
			}
			else // nothing selected - use all columns of group number 0
			{
				for (int i = 0; i < table.DataColumnCount; ++i)
				{
					var col = table[i];
					if (table.DataColumns.GetColumnGroup(col) == _groupNumber)
					{
						InternalAddDataColumnNoClone(bundle, ReadableColumnProxyBase.FromColumn(col));
						maxRowCount = Math.Max(maxRowCount, col.Count);
					}
				}
			}

			_useAllAvailableDataRows = null == selectedDataRows || selectedDataRows.Count == 0;

			_participatingDataRows = new AscendingIntegerCollection(_useAllAvailableDataRows ? ContiguousIntegerRange.FromStartAndCount(0, maxRowCount) : selectedDataRows);
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="T:Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
		{
			using (var suspendToken = SuspendGetToken()) // Suspend important here because otherwise Table reports a changed event, which will delete all column proxies not belonging to the new table
			{
				Report(_dataTable, this, "DataTable");

				foreach (var entry in _dataColumnBundles)
				{
					var bundle = entry.Value;
					for (int i = 0; i < bundle.DataColumns.Count; ++i)
						Report(bundle.DataColumns[i], this, string.Format("DataColumns[{0}]", i));
				}

				suspendToken.Resume();
			}
		}

		#region Setters for event wired members

		private void InternalSetDataTable(DataTableProxy proxy)
		{
			ChildSetMember(ref _dataTable, proxy ?? new DataTableProxy((DataTable)null));
		}

		/// <summary>
		/// Clear data columns collection, removes the event handlers from the data column proxies.
		/// </summary>
		private void InternalClearDataColumnBundles()
		{
			if (null == _dataColumnBundles)
				return;

			var arr = _dataColumnBundles.ToArray();
			_dataColumnBundles.Clear();

			foreach (var entry in arr)
			{
				foreach (var proxy in entry.Value.DataColumns)
					proxy.Dispose();
			}
		}

		/// <summary>
		/// Clear data columns collection, removes the event handlers from the data column proxies.
		/// </summary>
		private void InternalClearDataColumns(ColumnBundleInfo bundle)
		{
			var arr = bundle.DataColumns.ToArray();
			bundle.DataColumns.Clear();
			foreach (var proxy in arr)
				proxy.Dispose();
		}

		/// <summary>
		/// Adds a data column proxy to the data column collection without cloning it (i.e. the proxy is directly added).
		/// </summary>
		/// <param name="info">The bundle to which to add the column proxy.</param>
		/// <param name="proxy">The proxy.</param>
		private void InternalAddDataColumnNoClone(ColumnBundleInfo info, IReadableColumnProxy proxy)
		{
			if (null != proxy)
			{
				info.DataColumns.Add(proxy);
				proxy.ParentObject = this;
			}
		}

		/// <summary>
		/// Removes the data column proxy at index <paramref name="idx"/> from the bundle of DataColumnProxies.
		/// </summary>
		/// <param name="bundle">Bundle of column proxies.</param>
		/// <param name="idx">The index.</param>
		private void InternalRemoveDataColumnAt(ColumnBundleInfo bundle, int idx)
		{
			var col = bundle.DataColumns[idx];
			bundle.DataColumns.RemoveAt(idx);
			if (null != col)
				col.Dispose();
		}

		/// <summary>
		/// Clears the data column collection, then adds data column proxies, using a list of existing data column proxies. The proxies are cloned before adding them to the collection.
		/// </summary>
		/// <param name="fromList">The enumeration of data proxies to clone.</param>
		private void InternalSetDataColumnsWithCloning(Dictionary<string, ColumnBundleInfo> fromList)
		{
			if (null != _dataColumnBundles)
			{
				InternalClearDataColumnBundles();
			}
			else
			{
				_dataColumnBundles = new Dictionary<string, ColumnBundleInfo>();
			}

			this._dataColumnBundles.Clear();
			foreach (var entry in fromList)
			{
				ColumnBundleInfo fromB = entry.Value;

				var thisB = new ColumnBundleInfo(fromB.MaximumNumberOfColumns);
				foreach (var fromMember in fromB.DataColumns)
				{
					var clone = (IReadableColumnProxy)fromMember.Clone();
					clone.ParentObject = this;
					thisB.DataColumns.Add(clone);
				}

				this._dataColumnBundles.Add(entry.Key, thisB);
			}
		}

		/// <summary>
		/// Clears the data column collection of the provided bundle, then adds data column proxies, using a list of existing data column proxies. The proxies are cloned before adding them to the collection.
		/// </summary>
		/// <param name="bundle">Bundle in which to set the column proxies.</param>
		/// <param name="fromList">The enumeration of data proxies to clone.</param>
		private void InternalSetDataColumnsWithCloning(ColumnBundleInfo bundle, IEnumerable<IReadableColumnProxy> fromList)
		{
			if (null == bundle)
				throw new ArgumentNullException("bundle");

			InternalClearDataColumns(bundle);

			foreach (var fromMember in fromList)
			{
				var clone = (IReadableColumnProxy)fromMember.Clone();
				clone.ParentObject = this;
				bundle.DataColumns.Add(clone);
			}
		}

		#endregion Setters for event wired members

		#region Properties

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

		public void EnsureExistenceOfIdentifier(string identifier, int maximumNumberOfColumns)
		{
			if (null == identifier)
				throw new ArgumentNullException("identifier");
			if (!_dataColumnBundles.ContainsKey(identifier))
				_dataColumnBundles.Add(identifier, new ColumnBundleInfo(maximumNumberOfColumns));
		}

		public void EnsureExistenceOfIdentifier(string identifier)
		{
			EnsureExistenceOfIdentifier(identifier, int.MaxValue);
		}

		/// <summary>
		/// Adds a column to a bundle identified by a string <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">Identifier (key) to identifiy the bundle to which the data column is added.</param>
		/// <param name="column">Column to add. Must have ColumnKind.V and a group number equal to <see cref="GroupNumber"/>. Otherwise, this column will be removed in the next call to <see cref="Update"/>.</param>
		public void AddDataColumn(string identifier, IReadableColumn column)
		{
			if (null == identifier)
				throw new ArgumentNullException("identifier");

			if (identifier == "" && !_dataColumnBundles.ContainsKey(identifier))
				_dataColumnBundles.Add("", new ColumnBundleInfo());

			if (!_dataColumnBundles.ContainsKey(identifier))
				throw new InvalidOperationException(string.Format("Identifier {0} was not found in the collection", identifier));

			var bundle = _dataColumnBundles[identifier];

			if (null != column)
			{
				InternalAddDataColumnNoClone(bundle, ReadableColumnProxyBase.FromColumn(column));
				_isDirty = true;
			}
		}

		/// <summary>
		/// Sets the data column as the only data column of the bundle identified by <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">The identifier of the bundle in which to set the data column.</param>
		/// <param name="column">The data column to set.</param>
		/// <exception cref="System.ArgumentNullException">
		/// identifier
		/// or
		/// column
		/// </exception>
		public void SetDataColumn(string identifier, DataColumn column)
		{
			if (null == identifier)
				throw new ArgumentNullException("identifier");
			if (null == column)
				throw new ArgumentNullException("column");

			SetDataColumns(identifier, new IReadableColumnProxy[] { ReadableColumnProxyBase.FromColumn(column) });
		}

		/// <summary>
		/// Sets the data columns in the bundle identified by <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">The identifier of the bundle in which to set the data columns.</param>
		/// <param name="columns">The columns.</param>
		/// <exception cref="System.ArgumentNullException">
		/// identifier
		/// or
		/// column
		/// </exception>
		public void SetDataColumns(string identifier, IEnumerable<DataColumn> columns)
		{
			if (null == identifier)
				throw new ArgumentNullException("identifier");
			if (null == columns)
				throw new ArgumentNullException("column");

			SetDataColumns(identifier, columns.Select(column => ReadableColumnProxyBase.FromColumn(column)));
		}

		/// <summary>
		/// Clears the bundle identified by <paramref name="identifier"/>, and then sets the data columns in this bundle using an enumeration of data column proxies.
		/// </summary>
		/// <param name="identifier">Identifier (key) to identifiy the bundle in which the data columns are set.</param>
		/// <param name="dataColumnProxies">The enumeration of data column proxies. The proxies will be cloned before they are added to the data column collection.</param>
		public void SetDataColumns(string identifier, IEnumerable<IReadableColumnProxy> dataColumnProxies)
		{
			if (null == identifier)
				throw new ArgumentNullException("identifier");

			if (identifier == "" && !_dataColumnBundles.ContainsKey(identifier))
				_dataColumnBundles.Add("", new ColumnBundleInfo());

			if (!_dataColumnBundles.ContainsKey(identifier))
				throw new InvalidOperationException(string.Format("Identifier {0} was not found in the collection", identifier));

			var bundle = _dataColumnBundles[identifier];

			InternalClearDataColumns(bundle);
			InternalSetDataColumnsWithCloning(bundle, dataColumnProxies);
			_isDirty = true;
		}

		/// <summary>
		/// Determines whether this instance contains a column bundle with the provided <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">The identifier of the bundle.</param>
		/// <returns><c>True</c> if this instance contains a column bundle with the provided <paramref name="identifier"/>; otherwise <c>false</c>.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public bool ContainsIdentifier(string identifier)
		{
			if (null == identifier)
				throw new ArgumentNullException();

			return _dataColumnBundles.ContainsKey(identifier);
		}

		/// <summary>
		/// Gets the data column proxy at index <paramref name="idx"/> in the bundle identified by <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">Identifier (key) to identifiy the bundle to use.</param>
		/// <param name="idx">The index.</param>
		/// <returns>The data column proxy at index <paramref name="idx"/>.</returns>
		public IReadableColumnProxy GetDataColumnProxy(string identifier, int idx)
		{
			if (_isDirty)
				Update();

			if (null == identifier)
				throw new ArgumentNullException();

			if (!_dataColumnBundles.ContainsKey(identifier))
				throw new InvalidOperationException(string.Format("The identifier {0} is not contained in this collection", identifier));

			return _dataColumnBundles[identifier].DataColumns[idx];
		}

		/// <summary>
		/// Gets the data column proxies in the bundle identified by <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">Identifier (key) to identifiy the bundle with the column proxies.</param>
		/// <returns>The data column proxies in the bundle identified by <paramref name="identifier"/>.</returns>
		public IList<IReadableColumnProxy> GetDataColumnProxies(string identifier)
		{
			if (_isDirty)
				Update();

			if (null == identifier)
				throw new ArgumentNullException();

			if (!_dataColumnBundles.ContainsKey(identifier))
				throw new InvalidOperationException(string.Format("The identifier {0} is not contained in this collection", identifier));

			return _dataColumnBundles[identifier].DataColumns.AsReadOnly();
		}

		/// <summary>
		/// Gets the data columns in the bundle identified by <paramref name="identifier"/> which could be resolved, i.e. whose proxy contained a valid column.
		/// </summary>
		/// <param name="identifier">The identifier to identify the column bundle.</param>
		/// <returns>All data column of the bundle which could be resolved, i.e. whose proxy contained a valid column.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public IList<DataColumn> GetDataColumns(string identifier)
		{
			if (_isDirty)
				Update();

			if (null == identifier)
				throw new ArgumentNullException();
			if (!_dataColumnBundles.ContainsKey(identifier))
				throw new InvalidOperationException(string.Format("The identifier {0} is not contained in this collection", identifier));

			var src = _dataColumnBundles[identifier].DataColumns;

			return new List<DataColumn>(src.Where(x => x.Document is DataColumn).Select(x => (DataColumn)x.Document));
		}

		/// <summary>
		/// Gets the first valid data column of the bundle identified by <paramref name="identifier"/>, or null if no such column exists.
		/// </summary>
		/// <param name="identifier">The identifier to identify the column bundle.</param>
		/// <returns>The first valid data column of the bundle identified by <paramref name="identifier"/>, or null if no such column exists.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public DataColumn GetDataColumnOrNull(string identifier)
		{
			if (_isDirty)
				Update();

			if (null == identifier)
				throw new ArgumentNullException();
			if (!_dataColumnBundles.ContainsKey(identifier))
				throw new InvalidOperationException(string.Format("The identifier {0} is not contained in this collection", identifier));

			var bundle = _dataColumnBundles[identifier];
			var src = _dataColumnBundles[identifier].DataColumns;

			return src.Where(x => x.Document is DataColumn).Select(x => (DataColumn)x.Document).FirstOrDefault();
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

		/// <summary>
		/// Sets the data columns from an enumeration of data column proxies.
		/// </summary>
		/// <param name="dataRows">The enumeration of data rows.</param>
		public void SetDataRows(IAscendingIntegerCollection dataRows)
		{
			_participatingDataRows.Clear();

			foreach (var range in dataRows.RangesAscending)
				_participatingDataRows.AddRange(range.Start, range.Count);

			_isDirty = true;
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

		#endregion Properties

		/// <summary>
		/// Removes all data columns, whose parent is not the data table <paramref name="table"/>, or whose column kind is not ColumnKind.V, or whose group number is not equal to <see cref="GroupNumber"/>.
		/// </summary>
		/// <param name="table">The table to compare the parents of the columns with.</param>
		protected void InternalRemoveDataColumnsWithDeviatingParentOrKindOrGroupNumber(DataTable table)
		{
			var tableDataColumns = table.DataColumns;

			var indicesToRemove = new List<int>();

			foreach (var entry in _dataColumnBundles)
			{
				var bundle = entry.Value;
				var dataColumns = bundle.DataColumns;

				for (int i = dataColumns.Count - 1; i >= 0; --i)
				{
					if (dataColumns[i].IsEmpty)
					{
						InternalRemoveDataColumnAt(bundle, i);
						continue;
					}

					var c = dataColumns[i].Document as DataColumn;
					if (c == null)
						continue; // not yet resolved, leave it as it is

					var coll = DataColumnCollection.GetParentDataColumnCollectionOf(c);
					if (null == coll || !object.ReferenceEquals(coll, tableDataColumns))
					{
						InternalRemoveDataColumnAt(bundle, i);
						continue;
					}

					var group = tableDataColumns.GetColumnGroup(c);
					var kind = tableDataColumns.GetColumnKind(c);
					if (group != _groupNumber && kind != ColumnKind.V)
					{
						InternalRemoveDataColumnAt(bundle, i);
						continue;
					}
				}
			}
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
		/// Brings the members of this instance in a consistent state. After the state is consistent, the member <see cref="_isDirty"/> is set to <c>false</c>.
		/// This fails if the underlying data table is null, and can not be determined from the data columns.
		/// </summary>
		protected void Update()
		{
			if (!_isDirty)
				return;

			DataTable table = _dataTable.Document;
			if (null == table)
				return;

			InternalRemoveDataColumnsWithDeviatingParentOrKindOrGroupNumber(table);

			_isDirty = false;
		}

		/// <summary>
		/// Gets all column proxies from all column bundles in this instance.
		/// </summary>
		/// <returns>All column proxies from all column bundles in this instance.</returns>
		private IEnumerable<IReadableColumnProxy> GetAllColumnProxies()
		{
			foreach (var entry in _dataColumnBundles)
				foreach (var proxy in entry.Value.DataColumns)
					yield return proxy;
		}

		/// <summary>
		/// Gets the maximum row count of the data columns in all bundles.
		/// </summary>
		/// <returns>Maximum row count of all resolveable data columns in all column bundles.</returns>
		private int GetMaximumRowCountNow()
		{
			return GetAllColumnProxies().Where(p => p.Document != null).MaxOrDefault(p => p.Document.Count ?? 0, 0);
		}

		#region Change event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			_isDirty = true;
			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Change event handling

		#region Document Node functions

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataTable)
				yield return new Main.DocumentNodeAndName(_dataTable, "DataTable");

			if (null != _dataColumnBundles)
			{
				foreach (var entry in _dataColumnBundles)
				{
					int idx = -1;
					foreach (var proxy in entry.Value.DataColumns)
					{
						++idx;
						string name = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\\Col{1}", entry.Key, idx);
						yield return new Main.DocumentNodeAndName(proxy, name);
					}
				}
			}
		}

		#endregion Document Node functions
	}
}