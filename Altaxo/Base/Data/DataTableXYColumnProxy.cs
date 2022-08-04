#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Altaxo.Collections;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds reference to bundles of one y and the corresponding x column from the same group of a <see cref="DataTable"/>.
  /// </summary>
  public class DataTableXYColumnProxy
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    Main.ICopyFrom
  {
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

    protected IReadableColumnProxy _xColumn;

    protected IReadableColumnProxy _yColumn;

    /// <summary>
    /// Copies data from another instance of <see cref="DataTableXYColumnProxy"/>.
    /// </summary>
    /// <param name="obj">The instance.</param>
    /// <returns><c>True</c> if any data could be copyied.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is DataTableXYColumnProxy from)
      {
        ChildSetMember(ref _dataTable, (DataTableProxy)from._dataTable.Clone());
        ChildSetMember(ref _xColumn, (IReadableColumnProxy)from._xColumn.Clone());
        ChildSetMember(ref _yColumn, (IReadableColumnProxy)from._yColumn.Clone());
        _groupNumber = from._groupNumber;
        _useAllAvailableDataRows = from._useAllAvailableDataRows;
        _participatingDataRows = (AscendingIntegerCollection)from._participatingDataRows.Clone();
        _isDirty = from._isDirty;

        return true;
      }
      return false;
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-08-03 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableXYColumnProxy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTableXYColumnProxy)obj;
        info.AddValue("Table", s._dataTable);
        info.AddValue("Group", s._groupNumber);
        info.AddValue("UseAllAvailableDataRows", s._useAllAvailableDataRows);
        info.AddValue("XColumn", s._xColumn);
        info.AddValue("YColumn", s._yColumn);
        if (!s._useAllAvailableDataRows)
        {
          info.AddValue("DataRows", s._participatingDataRows);
        }
      }

      protected virtual DataTableXYColumnProxy SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (o is null ? new DataTableXYColumnProxy() : (DataTableXYColumnProxy)o);

        s.ChildSetMember(ref s._dataTable, (DataTableProxy)info.GetValue("Table", s));
        s._groupNumber = info.GetInt32("Group");
        s._useAllAvailableDataRows = info.GetBoolean("UseAllAvailableDataRows");
        s.ChildSetMember(ref s._xColumn, (IReadableColumnProxy)info.GetValue("XColumn", s));
        s.ChildSetMember(ref s._yColumn, (IReadableColumnProxy)info.GetValue("YColumn", s));
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
      var result = new DataTableXYColumnProxy();
      result.CopyFrom(this);
      return result;
    }

    /// <summary>
    /// Deserialization constructor
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DataTableXYColumnProxy()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.


    /// <summary>
    /// Initializes a new instance of the <see cref="DataTableXYColumnProxy"/> class.
    /// The table and group number of this instance are set, but no columns are set with this constructor.
    /// </summary>
    /// <param name="table">The underlying table.</param>
    /// <param name="xColumn">The x-column that belongs to the y-column.</param>
    /// <param name="yColumn">The y-column.</param>
    /// <param name="selectedDataRows">The selected data rows. If all data rows should be included, this argument can be null.</param>
    /// <exception cref="System.ArgumentNullException">table must not be null.</exception>
    public DataTableXYColumnProxy(DataTable table, DataColumn xColumn, DataColumn yColumn, IAscendingIntegerCollection? selectedDataRows = null)
    {
      if (table is null)
        throw new ArgumentNullException(nameof(table));
      var yGroupNumber = table.DataColumns.GetColumnGroup(yColumn);
      var xGroupNumber =table.DataColumns.GetColumnGroup(xColumn);

      if (xGroupNumber != yGroupNumber)
        throw new ArgumentException($"X-column belongs to another group ({xGroupNumber}) than y-column ({yGroupNumber})");

      _dataTable = new DataTableProxy(table) { ParentObject = this };
      _groupNumber = yGroupNumber;

      ChildSetMember(ref _xColumn, ReadableColumnProxy.FromColumn(xColumn));
      ChildSetMember(ref _yColumn, ReadableColumnProxy.FromColumn(yColumn));

      _useAllAvailableDataRows = selectedDataRows is null || selectedDataRows.Count == 0;
      _participatingDataRows = new AscendingIntegerCollection(_useAllAvailableDataRows ? ContiguousIntegerRange.FromStartAndCount(0, Math.Min(xColumn.Count, yColumn.Count)) : selectedDataRows!);
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
        Report(_xColumn, this, "XColumn");
        Report(_yColumn, this, "YColumn");
        suspendToken.Resume();
      }
    }

    #region Setters for event wired members

    private void InternalSetDataTable(DataTableProxy proxy)
    {
      ChildSetMember(ref _dataTable, proxy);
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
        return _dataTable.Document;
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

    public IReadableColumnProxy XColumnProxy
    {
      get
      {
        return _xColumn;
      }
    }

    public IReadableColumnProxy YColumnProxy
    {
      get
      {
        return _yColumn;
      }
    }


    public IReadableColumn? XColumn
    {
      get
      {
        return _xColumn.Document();
      }
    }

    public IReadableColumn? YColumn
    {
      get
      {
        return _yColumn.Document();
      }
    }

    /// <summary>If <c>true</c>, all available rows (of the columns that contribute to the matrix) will be included in the matrix.</summary>
    public bool UseAllAvailableDataRows
    {
      get { return _useAllAvailableDataRows; }
    }

    /// <summary>Gets the indices of the data rows that contribute to the matrix.</summary>
    public IAscendingIntegerCollection ParticipatingDataRows
    {
      get
      {
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
          InternalUpdateParticipatingDataRows();
        }

        return _isDirty ? 0 : _participatingDataRows.Count;
      }
    }

    #endregion Properties


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
    /// Gets the maximum row count of the data columns in all bundles.
    /// </summary>
    /// <returns>Maximum row count of all resolveable data columns in all column bundles.</returns>
    private int GetMaximumRowCountNow()
    {
      int result = 0;

      if (_xColumn.Document() is IReadableColumn rcx)
        result = Math.Max(result, rcx.Count??0);
      if (_yColumn.Document() is IReadableColumn rcy)
        result = Math.Max(result, rcy.Count??0);

      return result;
    }

    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      _isDirty = true;
      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Change event handling

    #region Document Node functions

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataTable is not null)
        yield return new Main.DocumentNodeAndName(_dataTable, "DataTable");
      if(_xColumn is not null)
        yield return new Main.DocumentNodeAndName(_xColumn, "XColumn");
      if (_xColumn is not null)
        yield return new Main.DocumentNodeAndName(_yColumn, "YColumn");
    }

    #endregion Document Node functions
  }
}
