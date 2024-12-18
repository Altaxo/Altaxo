﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  public class XYZColumnPlotData
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IColumnPlotData,
    System.ICloneable
  {
    /// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
    protected DataTableProxy _dataTable;

    /// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
    protected int _groupNumber;

    /// <summary>
    /// The selection of data rows to be plotted.
    /// </summary>
    protected IRowSelection _dataRowSelection;

    protected Altaxo.Data.IReadableColumnProxy _xColumn; // the X-Column
    protected Altaxo.Data.IReadableColumnProxy _yColumn; // the Y-Column
    protected Altaxo.Data.IReadableColumnProxy _zColumn; // the Z-Column

    // cached or temporary data
    protected IPhysicalBoundaries? _xBoundaries;

    protected IPhysicalBoundaries? _yBoundaries;

    protected IPhysicalBoundaries? _zBoundaries;

    /// <summary>List of plot points that is allocated once per thread (as thread local storage variable).</summary>
    [ThreadStatic]
    [NonSerialized]
    protected static List<PointD3D>? _tlsBufferedPlotData;

    /// <summary>
    /// One more that the index to the last valid pair of plot data.
    /// </summary>
    protected int _pointCount;

    protected bool _isCachedDataValidX = false;
    protected bool _isCachedDataValidY = false;
    protected bool _isCachedDataValidZ = false;

    #region Serialization

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="XYZColumnPlotData"/> class without any member initialization.
    /// </summary>
    /// <param name="info">The information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected XYZColumnPlotData(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// 2016-05-31 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZColumnPlotData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYZColumnPlotData)obj;

        info.AddValue("DataTable", s._dataTable);
        info.AddValue("GroupNumber", s._groupNumber);

        info.AddValue("RowSelection", s._dataRowSelection);

        info.AddValue("XColumn", s._xColumn);
        info.AddValue("YColumn", s._yColumn);
        info.AddValue("ZColumn", s._zColumn);

        info.AddValueOrNull("XBoundaries", s._xBoundaries);
        info.AddValueOrNull("YBoundaries", s._yBoundaries);
        info.AddValueOrNull("ZBoundaries", s._zBoundaries);
      }

      public virtual XYZColumnPlotData SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYZColumnPlotData?)o ?? new XYZColumnPlotData(info);

        s._dataTable = (DataTableProxy)info.GetValue("DataTable", s);
        if (s._dataTable is not null)
          s._dataTable.ParentObject = s;

        s._groupNumber = info.GetInt32("GroupNumber");

        s.ChildSetMember(ref s._dataRowSelection, (IRowSelection)info.GetValue("RowSelection", s));

        s._xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
        if (s._xColumn is not null)
          s._xColumn.ParentObject = s;

        s._yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);
        if (s._yColumn is not null)
          s._yColumn.ParentObject = s;

        s._zColumn = (IReadableColumnProxy)info.GetValue("ZColumn", s);
        if (s._zColumn is not null)
          s._zColumn.ParentObject = s;

        s.ChildSetMember(ref s._xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", s));
        s.ChildSetMember(ref s._yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", s));
        s.ChildSetMember(ref s._zBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("ZBoundaries", s));


        return s;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    public XYZColumnPlotData(Altaxo.Data.DataTable dataTable, int groupNumber, Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn, Altaxo.Data.IReadableColumn zColumn)
    {
      DataTable = dataTable;
      ChildSetMember(ref _dataRowSelection, new AllRows());
      _groupNumber = groupNumber;
      XColumn = xColumn;
      YColumn = yColumn;
      ZColumn = zColumn;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy from.</param>
    /// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
    public XYZColumnPlotData(XYZColumnPlotData from)
    {
      ChildCopyToMember(ref _dataTable, from._dataTable);
      _groupNumber = from._groupNumber;
      ChildCloneToMember(ref _dataRowSelection, from._dataRowSelection);

      ChildCopyToMember(ref _xColumn, from._xColumn);
      ChildCopyToMember(ref _yColumn, from._yColumn);
      ChildCopyToMember(ref _zColumn, from._zColumn);

      // cached or temporary data

      if (from._xBoundaries is not null)
        ChildCopyToMember(ref _xBoundaries, from._xBoundaries);

      if (from._yBoundaries is not null)
        ChildCopyToMember(ref _yBoundaries, from._yBoundaries);

      if (from._zBoundaries is not null)
        ChildCopyToMember(ref _zBoundaries, from._zBoundaries);

      _pointCount = from._pointCount;
      _isCachedDataValidX = from._isCachedDataValidX;
      _isCachedDataValidY = from._isCachedDataValidY;
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataTable is not null)
        yield return new DocumentNodeAndName(_dataTable, "DataTable");

      if (_dataRowSelection is not null)
        yield return new DocumentNodeAndName(_dataRowSelection, nameof(DataRowSelection));

      if (_xColumn is not null)
        yield return new Main.DocumentNodeAndName(_xColumn, "XColumn");

      if (_yColumn is not null)
        yield return new Main.DocumentNodeAndName(_yColumn, "YColumn");

      if (_zColumn is not null)
        yield return new Main.DocumentNodeAndName(_zColumn, "VColumn");

      if (_xBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_xBoundaries, "XBoundaries");

      if (_yBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_yBoundaries, "YBoundaries");

      if (_zBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_zBoundaries, "VBoundaries");
    }

    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    /// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
    public object Clone()
    {
      return new XYZColumnPlotData(this);
    }

    [MaybeNull]
    public DataTable DataTable
    {
      get
      {
        return _dataTable?.Document;
      }
      [MemberNotNull(nameof(_dataTable))]
      set
      {
        if (ChildSetMember(ref _dataTable, new DataTableProxy(value ?? throw new ArgumentNullException(nameof(DataTable)))))
        {
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public int GroupNumber
    {
      get
      {
        return _groupNumber;
      }
      set
      {
        if (!(_groupNumber == value))
        {
          _groupNumber = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// The selection of data rows to be plotted.
    /// </summary>
    public IRowSelection DataRowSelection
    {
      get
      {
        return _dataRowSelection;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!_dataRowSelection.Equals(value))
        {
          ChildSetMember(ref _dataRowSelection, value);
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the name of the x column, depending on the provided level.
    /// </summary>
    /// <param name="level">The level (0..2).</param>
    /// <returns>The name of the x-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
    public string GetXName(int level)
    {
      IReadableColumn? col = _xColumn.Document();
      if (col is Altaxo.Data.DataColumn dataCol)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataCol);
        string tablename = table is null ? string.Empty : table.Name + "\\";
        string collectionname = table is null ? string.Empty : (table.PropertyColumns.ContainsColumn(dataCol) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return dataCol.Name;
        else if (level == 1)
          return tablename + dataCol.Name;
        else
          return tablename + collectionname + dataCol.Name;
      }
      else if (col is not null)
      {
        return col.FullName;
      }
      else
      {
        return _xColumn.GetName(level) + " (broken)";
      }
    }

    /// <summary>
    /// Gets the name of the y column, depending on the provided level.
    /// </summary>
    /// <param name="level">The level (0..2).</param>
    /// <returns>The name of the y-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
    public string GetYName(int level)
    {
      IReadableColumn? col = _yColumn.Document();
      if (col is Altaxo.Data.DataColumn dataCol)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataCol);
        string tablename = table is null ? string.Empty : table.Name + "\\";
        string collectionname = table is null ? string.Empty : (table.PropertyColumns.ContainsColumn(dataCol) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return dataCol.Name;
        else if (level == 1)
          return tablename + dataCol.Name;
        else
          return tablename + collectionname + dataCol.Name;
      }
      else if (col is not null)
      {
        return col.FullName;
      }
      else
      {
        return _yColumn.GetName(level) + " (broken)";
      }
    }

    /// <summary>
    /// Gets the name of the z column, depending on the provided level.
    /// </summary>
    /// <param name="level">The level (0..2).</param>
    /// <returns>The name of the z-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
    public string GetZName(int level)
    {
      IReadableColumn? col = _zColumn.Document();
      if (col is Altaxo.Data.DataColumn dataCol)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataCol);
        string tablename = table is null ? string.Empty : table.Name + "\\";
        string collectionname = table is null ? string.Empty : (table.PropertyColumns.ContainsColumn(dataCol) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return dataCol.Name;
        else if (level == 1)
          return tablename + dataCol.Name;
        else
          return tablename + collectionname + dataCol.Name;
      }
      else if (col is not null)
      {
        return col.FullName;
      }
      else
      {
        return _zColumn.GetName(level) + " (broken)";
      }
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if (_xBoundaries is null || pb.GetType() != _xBoundaries.GetType())
        SetXBoundsFromTemplate(pb);

      if (!_isCachedDataValidX)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CalculateCachedData();
        }
      }
      pb.Add(_xBoundaries);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (_yBoundaries is null || pb.GetType() != _yBoundaries.GetType())
        SetYBoundsFromTemplate(pb);

      if (!_isCachedDataValidY)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CalculateCachedData();
        }
      }
      pb.Add(_yBoundaries);
    }

    public void MergeZBoundsInto(IPhysicalBoundaries pb)
    {
      if (_zBoundaries is null || pb.GetType() != _zBoundaries.GetType())
        SetZBoundsFromTemplate(pb);

      if (!_isCachedDataValidY)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CalculateCachedData();
        }
      }
      pb.Add(_zBoundaries);
    }

    /// <summary>
    /// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new x boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    [MemberNotNull(nameof(_xBoundaries))]
    protected void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_xBoundaries is null || val.GetType() != _xBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _xBoundaries, val))
        {
          _isCachedDataValidX = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new y boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    [MemberNotNull(nameof(_yBoundaries))]
    protected void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_yBoundaries is null || val.GetType() != _yBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _yBoundaries, val))
        {
          _isCachedDataValidY = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// This sets the v boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new y boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    [MemberNotNull(nameof(_zBoundaries))]
    protected void SetZBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_zBoundaries is null || val.GetType() != _zBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _zBoundaries, val))
        {
          _isCachedDataValidZ = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      Report(_dataTable, this, "DataTable");
      Report(_xColumn, this, "XColumn");
      Report(_yColumn, this, "YColumn");
      Report(_zColumn, this, "ZColumn");

      _dataRowSelection.VisitDocumentReferences(Report);
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    public IEnumerable<GroupOfColumnsInformation> GetAdditionallyUsedColumns()
    {
      yield return new GroupOfColumnsInformation("#0: X-Y-Z-Data", GetColumns());
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    private IEnumerable<ColumnInformation> GetColumns()
    {
      yield return new ColumnInformation("X", XColumn, _xColumn?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { if (col is not null) XColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
      yield return new ColumnInformation("Y", YColumn, _yColumn?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { if (col is not null) YColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
      yield return new ColumnInformation("Z", ZColumn, _zColumn?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { if (col is not null) ZColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
    }

    public IReadableColumn? GetDependentVariable(int i)
    {
      return i == 0 ? _zColumn?.Document() : null;
    }


    /// <summary>
    /// One more than the index to the last valid plot data point. This is <b>not</b>
    /// the number of plottable points!
    /// </summary>
    /// <remarks>This is not neccessarily (PlotRangeStart+PlotRangeLength), but always less or equal than this. This is because
    /// the underlying arrays can be smaller than the proposed plot range.</remarks>
    public int PlotRangeEnd
    {
      get
      {
        if (!_isCachedDataValidX || !_isCachedDataValidY || !_isCachedDataValidZ)
          CalculateCachedData();
        return _pointCount;
      }
    }

    [MaybeNull]
    public Altaxo.Data.IReadableColumn XColumn
    {
      get
      {
        return _xColumn?.Document();
      }
      [MemberNotNull(nameof(_xColumn))]
      set
      {
        if (ChildSetMember(ref _xColumn, ReadableColumnProxyBase.FromColumn(value)))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public string XColumnName
    {
      get
      {
        return _xColumn?.DocumentPath()?.LastPartOrDefault ?? string.Empty;
      }
    }

    [MaybeNull]
    public Altaxo.Data.IReadableColumn YColumn
    {
      get
      {
        return _yColumn is null ? null : _yColumn.Document();
      }
      [MemberNotNull(nameof(_yColumn))]
      set
      {
        if (ChildSetMember(ref _yColumn, ReadableColumnProxyBase.FromColumn(value)))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public string YColumnName
    {
      get
      {
        return _yColumn?.DocumentPath()?.LastPartOrDefault ?? string.Empty;
      }
    }

    [MaybeNull]
    public Altaxo.Data.IReadableColumn ZColumn
    {
      get
      {
        return _zColumn?.Document();
      }
      [MemberNotNull(nameof(_zColumn))]
      set
      {
        if (ChildSetMember(ref _zColumn, ReadableColumnProxyBase.FromColumn(value)))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public string ZColumnName
    {
      get
      {
        return _zColumn?.DocumentPath()?.LastPartOrDefault ?? string.Empty;
      }
    }

    public override string ToString()
    {
      return string.Format("{0}(X), {1}(Y), {2}(V)", _xColumn.ToString(), _yColumn.ToString(), _zColumn.ToString());
    }

    /// <summary>
    /// Gets the maximum row index that can be deduced from the data columns. The calculation does <b>not</b> include the DataRowSelection.
    /// </summary>
    /// <returns>The maximum row index that can be deduced from the data columns.</returns>
    public int GetMaximumRowIndexFromDataColumns()
    {
      var xColumn = XColumn;
      var yColumn = YColumn;
      var zColumn = ZColumn;

      int maxRowIndex;

      if (xColumn is null || yColumn is null || zColumn is null)
      {
        maxRowIndex = 0;
      }
      else
      {
        maxRowIndex = int.MaxValue;

        if (xColumn.Count.HasValue)
          maxRowIndex = System.Math.Min(maxRowIndex, xColumn.Count.Value);
        if (yColumn.Count.HasValue)
          maxRowIndex = System.Math.Min(maxRowIndex, yColumn.Count.Value);
        if (zColumn.Count.HasValue)
          maxRowIndex = System.Math.Min(maxRowIndex, zColumn.Count.Value);

        // if both columns are indefinite long, we set the length to zero
        if (maxRowIndex == int.MaxValue || maxRowIndex < 0)
          maxRowIndex = 0;
      }

      return maxRowIndex;
    }

    public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds, IPhysicalBoundaries vBounds)
    {
      if (IsDisposeInProgress)
        return;

      if (_xBoundaries is null || (xBounds is not null && _xBoundaries.GetType() != xBounds.GetType()))
      {
        _isCachedDataValidX = false;
        SetXBoundsFromTemplate(xBounds);
      }

      if (_yBoundaries is null || (yBounds is not null && _yBoundaries.GetType() != yBounds.GetType()))
      {
        _isCachedDataValidY = false;
        SetYBoundsFromTemplate(yBounds);
      }

      if (_zBoundaries is null || (vBounds is not null && _zBoundaries.GetType() != vBounds.GetType()))
      {
        _isCachedDataValidZ = false;
        SetZBoundsFromTemplate(vBounds);
      }

      if (!_isCachedDataValidX || !_isCachedDataValidY || !_isCachedDataValidZ)
        CalculateCachedData();
    }

    public void CalculateCachedData()
    {
      if (IsDisposeInProgress)
        return;

      // we can calulate the bounds only if they are set before
      if (_xBoundaries is null && _yBoundaries is null && _zBoundaries is null)
        return;



      var suspendTokenX = _xBoundaries?.SuspendGetToken();
      var suspendTokenY = _yBoundaries?.SuspendGetToken();
      var suspendTokenZ = _zBoundaries?.SuspendGetToken();

      try
      {
        _xBoundaries?.Reset();
        _yBoundaries?.Reset();
        _zBoundaries?.Reset();

        _pointCount = GetMaximumRowIndexFromDataColumns();

        if (XColumn is { } xColumn && YColumn is { } yColumn && ZColumn is { } zColumn && DataTable is { } dataTable)
        {
          foreach (var segment in _dataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, _pointCount, dataTable.DataColumns, _pointCount))
          {
            for (int rowIdx = segment.start; rowIdx < segment.endExclusive; ++rowIdx)
            {
              if (!xColumn.IsElementEmpty(rowIdx) && !yColumn.IsElementEmpty(rowIdx) && !zColumn.IsElementEmpty(rowIdx))
              {
                _xBoundaries?.Add(xColumn, rowIdx);
                _yBoundaries?.Add(yColumn, rowIdx);
                _zBoundaries?.Add(zColumn, rowIdx);
              }
            }
          }
        }

        // now the cached data are valid
        _isCachedDataValidX = _xBoundaries is not null;
        _isCachedDataValidY = _yBoundaries is not null;
        _isCachedDataValidZ = _zBoundaries is not null;

        // now when the cached data are valid, we can reenable the events
      }
      finally
      {
        suspendTokenX?.Resume();
        suspendTokenY?.Resume();
        suspendTokenZ?.Resume();
      }
    }

    private class MyPlotData
    {
      private IReadableColumn _xColumn;
      private IReadableColumn _yColumm;
      private IReadableColumn _zColumm;

      public MyPlotData(IReadableColumn xc, IReadableColumn yc, IReadableColumn zc)
      {
        _xColumn = xc;
        _yColumm = yc;
        _zColumm = zc;
      }

      public AltaxoVariant GetXPhysical(int originalRowIndex)
      {
        return _xColumn[originalRowIndex];
      }

      public AltaxoVariant GetYPhysical(int originalRowIndex)
      {
        return _yColumm[originalRowIndex];
      }

      public AltaxoVariant GetZPhysical(int originalRowIndex)
      {
        return _zColumm[originalRowIndex];
      }
    }

    /// <summary>
    /// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
    /// the function must have knowledge how to calculate the points out of the data. This will be done
    /// by a function provided by the calling function.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <returns>An array of plot points in layer coordinates.</returns>
    public Processed3DPlotData? GetRangesAndPoints(
      IPlotArea layer)
    {
      const double MaxRelativeValue = 1E2;

      var xColumn = XColumn;
      var yColumn = YColumn;
      var zColumn = ZColumn;

      if (xColumn is null || yColumn is null || zColumn is null)
        return null; // this plotitem is only for x and y double columns

      var result = new Processed3DPlotData();
      var myPlotData = new MyPlotData(xColumn, yColumn, zColumn);
      result.XPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetXPhysical);
      result.YPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetYPhysical);
      result.ZPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetZPhysical);
      PlotRangeList? rangeList = null;

      // allocate an array PointF to hold the line points
      // _tlsBufferedPlotData is a static buffer that is allocated per thread
      // and thus is only used temporary here in this routine
      if (_tlsBufferedPlotData is null)
        _tlsBufferedPlotData = new List<PointD3D>();
      else
        _tlsBufferedPlotData.Clear();

      // Fill the array with values
      // only the points where x and y are not NaNs are plotted!

      bool weAreInsideSegment = false;
      int rangeStart = 0;
      int rangeOffset = 0;
      rangeList = new PlotRangeList();
      result.RangeList = rangeList;

      Scale xAxis = layer.XAxis;
      Scale yAxis = layer.YAxis;
      Scale zAxis = layer.ZAxis;
      G3DCoordinateSystem coordsys = layer.CoordinateSystem;

      int maxRowIndex = GetMaximumRowIndexFromDataColumns();
      int plotArrayIdx = 0;

      if (DataTable is { } dataTable)
      {
        foreach ((int start, int endExclusive) in _dataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, maxRowIndex, dataTable.DataColumns, maxRowIndex))
        {
          for (int dataRowIdx = start; dataRowIdx < endExclusive; ++dataRowIdx)
          {
            if (xColumn.IsElementEmpty(dataRowIdx) || yColumn.IsElementEmpty(dataRowIdx) || zColumn.IsElementEmpty(dataRowIdx))
            {
              if (weAreInsideSegment)
              {
                weAreInsideSegment = false;
                rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
              }
              continue;
            }

            double x_rel, y_rel, z_rel;

            x_rel = xAxis.PhysicalVariantToNormal(xColumn[dataRowIdx]);
            y_rel = yAxis.PhysicalVariantToNormal(yColumn[dataRowIdx]);
            z_rel = zAxis.PhysicalVariantToNormal(zColumn[dataRowIdx]);

            // chop relative values to an range of about -+ 10^6
            if (x_rel > MaxRelativeValue)
              x_rel = MaxRelativeValue;
            if (x_rel < -MaxRelativeValue)
              x_rel = -MaxRelativeValue;
            if (y_rel > MaxRelativeValue)
              y_rel = MaxRelativeValue;
            if (y_rel < -MaxRelativeValue)
              y_rel = -MaxRelativeValue;
            if (z_rel > MaxRelativeValue)
              z_rel = MaxRelativeValue;
            if (z_rel < -MaxRelativeValue)
              z_rel = -MaxRelativeValue;

            // after the conversion to relative coordinates it is possible
            // that with the choosen axis the point is undefined
            // (for instance negative values on a logarithmic axis)
            // in this case the returned value is NaN
            if (coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel, y_rel, z_rel), out var coord))
            {
              if (!weAreInsideSegment)
              {
                weAreInsideSegment = true;
                rangeStart = plotArrayIdx;
                rangeOffset = dataRowIdx - plotArrayIdx;
              }
              _tlsBufferedPlotData.Add(coord);
              plotArrayIdx++;
            }
            else
            {
              if (weAreInsideSegment)
              {
                weAreInsideSegment = false;
                rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
              }
            }
          } // end for
          if (weAreInsideSegment)
          {
            weAreInsideSegment = false;
            rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset)); // add the last range
          }
        } // end foreach
      }
      result.PlotPointsInAbsoluteLayerCoordinates = _tlsBufferedPlotData.ToArray();

      return result;
    }


    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _xColumn) || object.ReferenceEquals(sender, _yColumn) || object.ReferenceEquals(sender, _zColumn))
        _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false;

      // If it is BoundaryChangedEventArgs, we have to set a flag for which boundary is affected
      var eAsBCEA = e as BoundariesChangedEventArgs;
      if (eAsBCEA is not null)
      {
        if (object.ReferenceEquals(sender, _xBoundaries))
        {
          eAsBCEA.SetXBoundaryChangedFlag();
        }
        else if (object.ReferenceEquals(sender, _yBoundaries))
        {
          eAsBCEA.SetYBoundaryChangedFlag();
        }
        else if (object.ReferenceEquals(sender, _zBoundaries))
        {
          eAsBCEA.SetZBoundaryChangedFlag();
        }
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    /// <summary>
    /// Looks whether one of data data columns have changed their data. If this is the case, we must recalculate the boundaries,
    /// and trigger the boundary changed event if one of the boundaries have changed.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
    /// <returns>
    /// The return value of the base handling function
    /// </returns>
    protected override void OnChanged(EventArgs e)
    {
      /* 2019-09-12 Outcommented for new data deserialization: the next lines will cause the XColumn and YColumn to be instantiated
       * from the corresponding proxies,
       * this is unwanted, because it will require to load the table. It should happen only if the graph is really needed

      if (!_isCachedDataValidX || !_isCachedDataValidY || !_isCachedDataValidZ)
        CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

      */

      base.OnChanged(e);
    }

    #endregion Change event handling
  }
}
