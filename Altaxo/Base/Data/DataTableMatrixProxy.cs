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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds reference to a matrix-like arrangement of data from a <see cref="DataTable"/>. The matrix data consist of 2 or more <see cref="DataColumn"/>s and all or selected data rows.
  /// Furthermore, a row header column and a column header column can deliver corresponding physical values for each matrix row and column, respectively.
  /// </summary>
  public class DataTableMatrixProxy : Main.SuspendableDocumentNodeWithEventArgs, Main.ICopyFrom
  {
    #region Inner classes

    private class DoubleMatrixAsNullDevice : Altaxo.Calc.LinearAlgebra.IMatrix<double>
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

      public int RowCount
      {
        get { return _rows; }
      }

      public int ColumnCount
      {
        get { return _columns; }
      }
    }

    private class HeaderColumnWrapper : IROVector<double>, IReadableColumn
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

      public int Count
      {
        get { return _participatingDataRows.Count; }
      }

      int? IReadableColumn.Count
      {
        get { return Length; }
      }

      /// <summary>
      /// Gets the type of the colum's items.
      /// </summary>
      /// <value>
      /// The type of the item.
      /// </value>
      public Type ItemType { get { return typeof(double); } }

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
        get { return GetType().ToString(); }
      }

      public object Clone()
      {
        throw new NotImplementedException();
      }

      public IEnumerator<double> GetEnumerator()
      {
        var length = Length;
        for (int i = 0; i < length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        var length = Length;
        for (int i = 0; i < length; ++i)
          yield return this[i];
      }
    }

    protected class ColumnPositionComparer : IComparer<IReadableColumnProxy>
    {
      private DataColumnCollection _coll;

      public ColumnPositionComparer(DataColumnCollection coll)
      {
        _coll = coll;
      }

      public int Compare(IReadableColumnProxy? a, IReadableColumnProxy? b)
      {
        var ca = a?.Document() as DataColumn;
        var cb = b?.Document() as DataColumn;

        if (ca is not null && cb is not null)
        {
          int na = _coll.GetColumnNumber(ca);
          int nb = _coll.GetColumnNumber(cb);
          return Comparer<int>.Default.Compare(na, nb);
        }
        if (ca is null && cb is null)
          return 0;
        else if (ca is null)
          return -1;
        else
          return 1;
      }
    }

    private class MyMatrixWrapper : IROMatrix<double>
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

      public int RowCount
      {
        get { return _participatingRows.Count; }
      }

      public int ColumnCount
      {
        get { return _participatingCols.Count; }
      }
    }

    #endregion Inner classes

    /// <summary><c>True</c> if the data are inconsistent. To bring the data in a consistent state <see cref="Update"/> method must be called then.</summary>
    protected bool _isDirty;

    /// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
    protected DataTableProxy? _dataTable;

    protected List<IReadableColumnProxy> _dataColumns; // the columns that are involved in the matrix

    /// <summary>The group number of the data columns. All data columns must be columns of ColumnKind.V and must have this group number. Data columns having other group numbers will be removed.</summary>
    protected int _groupNumber;

    /// <summary>Column that correlate each row of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
    protected IReadableColumnProxy _rowHeaderColumn;

    /// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
    protected IReadableColumnProxy _columnHeaderColumn;

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
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is DataTableMatrixProxy from)
      {
        ChildCloneToMember(ref _dataTable, from._dataTable);
        InternalSetDataColumnsWithCloning(from._dataColumns);
        InternalSetRowHeaderColumn((IReadableColumnProxy)from._rowHeaderColumn.Clone());
        InternalSetColumnHeaderColumn((IReadableColumnProxy)from._columnHeaderColumn.Clone());
        _groupNumber = from._groupNumber;
        _useAllAvailableColumnsOfGroup = from._useAllAvailableColumnsOfGroup;
        _useAllAvailableDataRows = from._useAllAvailableDataRows;
        _participatingDataRows = (AscendingIntegerCollection)from._participatingDataRows.Clone();
        _participatingDataColumns = (AscendingIntegerCollection)from._participatingDataColumns.Clone();
        _isDirty = from._isDirty;

        return true;
      }

      return false;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataTable is not null)
        yield return new Main.DocumentNodeAndName(_dataTable, "DataTable");

      if (_columnHeaderColumn is not null)
        yield return new Main.DocumentNodeAndName(_columnHeaderColumn, "ColumnHeaderColumn");

      if (_rowHeaderColumn is not null)
        yield return new Main.DocumentNodeAndName(_rowHeaderColumn, "RowHeaderColumn");

      if (_dataColumns is not null)
      {
        for (int i = 0; i < _dataColumns.Count; ++i)
        {
          if (_dataColumns[i] is not null)
            yield return new Main.DocumentNodeAndName(_dataColumns[i], "DataColumn" + i.ToString(System.Globalization.CultureInfo.CurrentCulture));
        }
      }
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
        info.AddValueOrNull("Table", s._dataTable);
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

      protected virtual DataTableMatrixProxy SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataTableMatrixProxy?)o ?? new DataTableMatrixProxy();

        s.ChildSetMember(ref s._dataTable, info.GetValueOrNull<DataTableProxy>("Table", s));
        s._groupNumber = info.GetInt32("Group");
        s.InternalSetRowHeaderColumn((IReadableColumnProxy)info.GetValue("RowHeaderColumn", s));
        s.InternalSetColumnHeaderColumn((IReadableColumnProxy)info.GetValue("ColumnHeaderColumn", s));

        s._useAllAvailableColumnsOfGroup = info.GetBoolean("UseAllAvailableColumnsOfGroup");
        s._useAllAvailableDataRows = info.GetBoolean("UseAllAvailableDataRows");

        if (!s._useAllAvailableColumnsOfGroup)
        {
          int count = info.OpenArray();
          s._dataColumns = new List<IReadableColumnProxy>(count);
          for (int i = 0; i < count; i++)
          {
            s.InternalAddDataColumnNoClone((IReadableColumnProxy)info.GetValue("e", s));
          }
          info.CloseArray(count);
        }
        else
        {
          s._dataColumns = new List<IReadableColumnProxy>();
        }

        if (!s._useAllAvailableDataRows)
        {
          s._participatingDataRows = (AscendingIntegerCollection)info.GetValue("DataRows", s);
        }
        else
        {
          s._participatingDataRows = new AscendingIntegerCollection();
        }

        s._participatingDataColumns = new AscendingIntegerCollection(); // this is just to avoid NullExceptions

        s._isDirty = true;

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DataTableMatrixProxy()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

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
      if (table is null)
        throw new ArgumentNullException("table");

      _dataTable = new DataTableProxy(table)
      {
        ParentObject = this
      };

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

      _rowHeaderColumn = ReadableColumnProxyBase.FromColumn(converter.RowHeaderColumn);
      _rowHeaderColumn.ParentObject = this;

      _columnHeaderColumn = ReadableColumnProxyBase.FromColumn(converter.ColumnHeaderColumn);
      _columnHeaderColumn.ParentObject = this;

      _dataColumns = new List<IReadableColumnProxy>();
      _participatingDataColumns = new AscendingIntegerCollection(converter.GetParticipatingDataColumns());
      for (int i = 0; i < _participatingDataColumns.Count; i++)
      {
        _dataColumns.Add(ReadableColumnProxyBase.FromColumn(table.DataColumns[_participatingDataColumns[i]]));

        // set the event chain
        _dataColumns[i].ParentObject = this;
      }

      _participatingDataRows = new AscendingIntegerCollection(converter.GetParticipatingDataRows());
    }

    [Obsolete("This is intended for legacy deserialization (of XYZMeshedColumnPlotData) only.")]
    public static DataTableMatrixProxy CreateEmptyInstance()
    {
      var result = new DataTableMatrixProxy
      {
        _participatingDataColumns = new AscendingIntegerCollection(),
        _participatingDataRows = new AscendingIntegerCollection(),
        _dataColumns = new List<IReadableColumnProxy>(),

        _dataTable = null!
      };
      result.InternalSetRowHeaderColumn(ReadableColumnProxyBase.FromColumn(null));
      result.InternalSetColumnHeaderColumn(ReadableColumnProxyBase.FromColumn(null));

      return result;
    }

    [Obsolete("This is intended for legacy deserialization (of XYZMeshedColumnPlotData) only.")]
    public DataTableMatrixProxy(IReadableColumnProxy xColumn, IReadableColumnProxy yColumn, IReadableColumnProxy[] dataColumns)
    {
      _participatingDataColumns = new AscendingIntegerCollection();
      _participatingDataRows = new AscendingIntegerCollection();
      _dataColumns = new List<IReadableColumnProxy>();

      _dataTable = null!;
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
    /// <param name="Report">Function that reports the found <see cref="T:Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
    {
      using (var suspendToken = SuspendGetToken()) // Suspend important here because otherwise Table reports a changed event, which will delete all column proxies not belonging to the new table
      {
        if (_dataTable is not null)
          Report(_dataTable, this, "DataTable");

        Report(_rowHeaderColumn, this, "RowHeaderColumn");
        Report(_columnHeaderColumn, this, "ColumnHeaderColumn");
        for (int i = 0; i < _dataColumns.Count; ++i)
          Report(_dataColumns[i], this, string.Format("DataColumns[{0}]", i));

        suspendToken.Resume();
      }
    }

    #region Setters for event wired members

    private void InternalSetDataTable(DataTableProxy proxy)
    {
      ChildSetMember(ref _dataTable, proxy);
    }

    [MemberNotNull(nameof(_rowHeaderColumn))]
    private void InternalSetRowHeaderColumn(IReadableColumnProxy proxy)
    {
      ChildSetMember(ref _rowHeaderColumn, proxy ?? ReadableColumnProxyBase.FromColumn(null)); // always ensure to have a proxy != null
    }

    [MemberNotNull(nameof(_columnHeaderColumn))]
    private void InternalSetColumnHeaderColumn(IReadableColumnProxy proxy)
    {
      ChildSetMember(ref _columnHeaderColumn, proxy ?? ReadableColumnProxyBase.FromColumn(null));
    }

    /// <summary>
    /// Adds a data column proxy to the data column collection without cloning it (i.e. the proxy is directly added).
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    private void InternalAddDataColumnNoClone(IReadableColumnProxy proxy)
    {
      if (proxy is not null)
      {
        _dataColumns.Add(proxy);
        proxy.ParentObject = this;
      }
    }

    /// <summary>
    /// Removes the data column proxy at index <paramref name="idx"/>, removing the Changed event handler.
    /// </summary>
    /// <param name="idx">The index.</param>
    private void InternalRemoveDataColumnAt(int idx)
    {
      var col = _dataColumns[idx];
      _dataColumns.RemoveAt(idx);
      if (col is not null)
        col.Dispose();
    }

    /// <summary>
    /// Clears the data column collection, then adds data column proxies, using a list of existing data column proxies. The proxies are cloned before adding them to the collection.
    /// </summary>
    /// <param name="fromList">The enumeration of data proxies to clone.</param>
    private void InternalSetDataColumnsWithCloning(IEnumerable<IReadableColumnProxy> fromList)
    {
      var oldDataColumns = _dataColumns;
      _dataColumns = new List<IReadableColumnProxy>();

      foreach (var fromMember in fromList)
      {
        var clone = (IReadableColumnProxy)fromMember.Clone();
        clone.ParentObject = this;
        _dataColumns.Add(clone);
      }

      // dispose old columns __after__ (!) cloning, because it is possible that they are identical to some data column in fromList
      if (oldDataColumns is not null)
      {
        foreach (var col in oldDataColumns)
          if (col is not null)
            col.Dispose();
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
    [MaybeNull]
    public DataTable DataTable
    {
      get
      {
        if (_isDirty)
          Update();

        return _dataTable?.Document;
      }
      set
      {
        var oldValue = _dataTable?.Document;
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
    [MaybeNull]
    public IReadableColumn RowHeaderColumn
    {
      get
      {
        return _rowHeaderColumn.Document();
      }
      set
      {
        var oldValue = _rowHeaderColumn.Document();
        if (!object.ReferenceEquals(oldValue, value))
        {
          InternalSetRowHeaderColumn(ReadableColumnProxyBase.FromColumn(value));
          _isDirty = true;
        }
      }
    }

    /// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
    [MaybeNull]
    public IReadableColumn ColumnHeaderColumn
    {
      get
      {
        return _columnHeaderColumn.Document();
      }
      set
      {
        var oldValue = _columnHeaderColumn.Document();
        if (!object.ReferenceEquals(oldValue, value))
        {
          InternalSetColumnHeaderColumn(ReadableColumnProxyBase.FromColumn(value));
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
      if (column is not null)
      {
        InternalAddDataColumnNoClone(ReadableColumnProxyBase.FromColumn(column));
        _isDirty = true;
      }
    }

    /// <summary>
    /// Sets the data columns from an enumeration of data column proxies.
    /// </summary>
    /// <param name="dataColumnProxies">The enumeration of data column proxies. The proxies will be cloned before they are added to the data column collection.</param>
    public void SetDataColumns(IEnumerable<IReadableColumnProxy> dataColumnProxies)
    {
      InternalSetDataColumnsWithCloning(dataColumnProxies);
      _isDirty = true;
    }

    /// <summary>
    /// Gets the data column proxy at index <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The index.</param>
    /// <returns>The data column proxy at index <paramref name="idx"/>.</returns>
    public IReadableColumnProxy GetDataColumnProxy(int idx)
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

        if (_participatingDataColumns is null)
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
    /// Removes all data columns, whose parent is not the data table <paramref name="table"/>, or whose column kind is not ColumnKind.V, or whose group number is not equal to <see cref="GroupNumber"/>.
    /// </summary>
    /// <param name="table">The table to compare the parents of the columns with.</param>
    protected void InternalRemoveDataColumnsWithDeviatingParentOrKindOrGroupNumber(DataTable table)
    {
      var tableDataColumns = table.DataColumns;

      //var indicesToRemove = new List<int>();

      for (int i = _dataColumns.Count - 1; i >= 0; --i)
      {
        if (_dataColumns[i].IsEmpty)
        {
          InternalRemoveDataColumnAt(i);
          continue;
        }

        if (!(_dataColumns[i].Document() is DataColumn c))
        {
          continue; // not yet resolved, leave it as it is
        }

        if (c.IsDisposeInProgress)
        {
          // column is disposed or is about to be disposed, thus remove it
          InternalRemoveDataColumnAt(i);
          continue;
        }

        var coll = DataColumnCollection.GetParentDataColumnCollectionOf(c);
        if (coll is null || !object.ReferenceEquals(coll, tableDataColumns))
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
    /// Should be called only if <see cref="_useAllAvailableColumnsOfGroup"/> is <c>true</c>. Removes all data columns that are unresolved.
    /// </summary>
    /// <param name="table">The table to search.</param>
    protected virtual void InternalRemoveUnresolvedDataColumnsIfAllDataColumnsShouldBeIncluded(DataTable table)
    {
      _dataColumns.RemoveAll(proxy => proxy.Document() is null);
    }

    /// <summary>
    /// Should be called only if <see cref="_useAllAvailableColumnsOfGroup"/> is <c>true</c>. Adds all missing data columns that have a group number of <see cref="GroupNumber"/> and ColumnKind.V.
    /// </summary>
    /// <param name="table">The table to search.</param>
    protected virtual void InternalAddMissingDataColumnsIfAllDataColumnsShouldBeIncluded(DataTable table)
    {
      var dataColumns = table.DataColumns;
      var existing = new HashSet<DataColumn>(_dataColumns.Select(x => x.Document()).OfType<DataColumn>());
      var toInsert = dataColumns.Columns.Where(c => dataColumns.GetColumnGroup(c) == _groupNumber && dataColumns.GetColumnKind(c) == ColumnKind.V && !existing.Contains(c));
      foreach (var ins in toInsert)
      {
        InternalAddDataColumnNoClone(ReadableColumnProxyBase.FromColumn(ins));
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
        if (_dataColumns[i].Document() is DataColumn col)
        {
          _participatingDataColumns.Add(table.DataColumns.GetColumnNumber(col));
        }
      }
    }

    /// <summary>
    /// Gets the maximum row count of the data columns in the data column collection <see cref="_dataColumns"/>.
    /// </summary>
    /// <returns></returns>
    private int GetMaximumRowCountNow()
    {
      return _dataColumns.Select(p => p.Document()).MaxOrDefault(d => d?.Count ?? 0, 0);
    }

    /// <summary>
    /// Tries to get a reference to the underlying table from the data columns. Used if no table was known beforehand (mainly after legacy deserialization).
    /// </summary>
    private void TryGetDataTableProxyFromColumns()
    {
      DataColumn? col;
      DataTable? table;
      foreach (var colproxy in _dataColumns)
      {
        col = colproxy.Document() as DataColumn;
        if (col is not null)
        {
          table = DataTable.GetParentDataTableOf(col);
          if (table is not null)
            _dataTable = new DataTableProxy(table) { ParentObject = this };
        }
      }

      foreach (var colproxy in new IReadableColumnProxy[] { _rowHeaderColumn, _columnHeaderColumn })
      {
        col = colproxy.Document() as DataColumn;
        if (col is not null)
        {
          table = DataTable.GetParentDataTableOf(col);
          if (table is not null)
            _dataTable = new DataTableProxy(table) { ParentObject = this };
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

      if (IsDisposeInProgress)
        return;

      if (_dataTable is null)
        TryGetDataTableProxyFromColumns(); // legacy, for instance from old XYZMeshedColumnPlotData, we have not stored the table reference

      if (_dataTable is null)
        return;

      var table = _dataTable.Document;
      if (table is null || table.IsDisposeInProgress)
        return;

      InternalRemoveDataColumnsWithDeviatingParentOrKindOrGroupNumber(table);

      if (_useAllAvailableColumnsOfGroup)
      {
        InternalRemoveUnresolvedDataColumnsIfAllDataColumnsShouldBeIncluded(table);
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
    public IMatrix<double> GetMatrix(Func<int, int, IMatrix<double>> matrixGenerator)
    {
      if (_isDirty)
      {
        Update();
      }

      var table = _dataTable?.Document;
      if (table is null)
        throw new InvalidOperationException("DataTableProxy or DataTable is null");

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
    public IROMatrix<double> GetMatrixWrapper()
    {
      if (_isDirty)
      {
        Update();
      }

      var table = _dataTable?.Document;

      if (table is null)
        throw new InvalidOperationException("DataTableProxy or DataTable is null");

      return new MyMatrixWrapper(table.DataColumns, _participatingDataRows, _participatingDataColumns);
    }

    /// <summary>
    /// Gets a wrapper vector around the row header data.
    /// </summary>
    /// <returns>Wrapper vector around the row header data. Each element of this vector corresponds to the row with the same index of the matrix.</returns>
    public IROVector<double> GetRowHeaderWrapper()
    {
      if (!_rowHeaderColumn.IsEmpty && _rowHeaderColumn.Document() is { } rowHeaderColumn)
        return new HeaderColumnWrapper(rowHeaderColumn, _participatingDataRows);
      else
        return VectorMath.CreateEquidistantSequenceByStartStepLength(0.0, 1.0, _participatingDataRows.Count);
    }

    /// <summary>
    /// Gets a wrapper vector around the column header data.
    /// </summary>
    /// <returns>Wrapper vector around the column header data. Each element of this vector corresponds to the column with the same index of the matrix.</returns>
    public IROVector<double> GetColumnHeaderWrapper()
    {
      if (!_columnHeaderColumn.IsEmpty && _columnHeaderColumn.Document() is { } columnHeaderColumn)
        return new HeaderColumnWrapper(columnHeaderColumn, _participatingDataColumns);
      else
        return VectorMath.CreateEquidistantSequenceByStartStepLength(0.0, 1.0, _participatingDataColumns.Count);
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
    public void GetWrappers(Func<AltaxoVariant, double> TransformRowHeaderValues, Func<double, bool> SelectTransformedRowHeaderValues, Func<AltaxoVariant, double> TransformColumnHeaderValues, Func<double, bool> SelectTransformedColumnHeaderValues, out IROMatrix<double> resultantMatrix, out IROVector<double> resultantTransformedRowHeaderValues, out IROVector<double> resultantTransformedColumnHeaderValues)
    {
      if (_isDirty)
      {
        Update();
      }

      var table = _dataTable?.Document;

      if (table is null)
      {
        resultantMatrix = new Matrix(0, 0);
        resultantTransformedColumnHeaderValues = new DoubleVector();
        resultantTransformedRowHeaderValues = new DoubleVector();
        return;
      }

      var transformedAndSelectedRowHeaderValues = new List<double>();
      var participatingDataRowsSelectedNow = new AscendingIntegerCollection();

      var numRows = _participatingDataRows.Count;
      var rowHeaderWrapper = GetRowHeaderWrapper();
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
      var colHeaderWrapper = GetColumnHeaderWrapper();
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

      if (!(participatingDataRowsSelectedNow.Count == transformedAndSelectedRowHeaderValues.Count))
        throw new InvalidProgramException();
      if (!(participatingDataColumnsSelectedNow.Count == transformedAndSelectedColumnHeaderValues.Count))
        throw new InvalidProgramException();

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

      var table = _dataTable?.Document;
      if (table is null)
        return; // nothing to do if we can not resolve the table

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

      var col = _rowHeaderColumn.Document();

      if (col is not null)
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

      var col = _columnHeaderColumn.Document();

      if (col is not null)
      {
        int columnCount = _participatingDataColumns.Count;
        for (int c = 0; c < columnCount; ++c)
          action(col, _participatingDataColumns[c]);
      }
    }

    #endregion Result functions

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      _isDirty = true;
      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed event handling

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
    public static bool TryGetColumnDataIncrement(IReadableColumnProxy proxy, string rowOrCol, IAscendingIntegerCollection selectedIndices, out double incrementValue, [MaybeNullWhen(true)] out string errorOrWarningMessage)
    {
      incrementValue = 1;

      if (proxy is null || proxy.IsEmpty)
      {
        errorOrWarningMessage = string.Format("No {0} header column chosen.", rowOrCol);
        return false;
      }

      var col = proxy.Document();
      if (col is null)
      {
        errorOrWarningMessage = string.Format("Link to {0} header column is lost.", rowOrCol);
        return false;
      }

      if (!(col is INumericColumn xCol))
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
        errorOrWarningMessage = string.Format("Warning: The {0} header column is not strictly equally spaced, the relative deviation is {1}", rowOrCol, spacing.RelativeSpaceDeviation);
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
    public bool TryGetRowHeaderIncrement(out double incrementValue, [MaybeNullWhen(true)] out string errorOrWarningMessage)
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
    public bool TryGetColumnHeaderIncrement(out double incrementValue, [MaybeNullWhen(true)] out string errorOrWarningMessage)
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
