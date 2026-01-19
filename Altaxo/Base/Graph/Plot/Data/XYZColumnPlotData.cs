#region Copyright

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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using Altaxo.Serialization.Xml;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  public class XYZColumnPlotData
    :
    XYAndZColumn,
    IColumnPlotData,
    System.ICloneable
  {
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
    /// 2016-05-31 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Plot.Data.XYZColumnPlotData", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYZColumnPlotData)obj;
        throw new InvalidOperationException("Serialization of old version");

        /*
        info.AddValue("DataTable", s._dataTable);
        info.AddValue("GroupNumber", s._groupNumber);

        info.AddValue("RowSelection", s._dataRowSelection);

        info.AddValue("XColumn", s._xColumn);
        info.AddValue("YColumn", s._yColumn);
        info.AddValue("ZColumn", s._zColumn);

        info.AddValueOrNull("XBoundaries", s._xBoundaries);
        info.AddValueOrNull("YBoundaries", s._yBoundaries);
        info.AddValueOrNull("ZBoundaries", s._zBoundaries);
        */
      }

      public virtual XYZColumnPlotData SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var dataTable = (DataTableProxy)info.GetValue("DataTable", null);
        var groupNumber = info.GetInt32("GroupNumber");
        var dataRowSelection = (IRowSelection)info.GetValue("RowSelection", null);
        var xColumn = (IReadableColumnProxy)info.GetValue("XColumn", null);
        var yColumn = (IReadableColumnProxy)info.GetValue("YColumn", null);
        var zColumn = (IReadableColumnProxy)info.GetValue("ZColumn", null);

        var s = (XYZColumnPlotData?)o ?? new XYZColumnPlotData(dataTable, groupNumber, xColumn, yColumn, zColumn);
        s.DataRowSelection = dataRowSelection;

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



    /// <summary>
    /// 2016-05-31 initial version
    /// 2026-01-19 class now derived from XYZColumnPlotData
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZColumnPlotData), 1)]
    protected class SerializationSurrogate1 : XYAndZColumn.XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);

        var s = (XYZColumnPlotData)obj;
        info.AddValueOrNull("XBoundaries", s._xBoundaries);
        info.AddValueOrNull("YBoundaries", s._yBoundaries);
        info.AddValueOrNull("ZBoundaries", s._zBoundaries);
      }


      public override object? Deserialize(object? o, IXmlDeserializationInfo info, object? parentobject)
      {
        if (o is XYZColumnPlotData s)
          s.DeserializeSurrogate1(info);
        else
          s = new XYZColumnPlotData(info, 1);
        return s;
      }
    }

    protected XYZColumnPlotData(IXmlDeserializationInfo info, int version)
      : base(info, 0)
    {
      ChildSetMember(ref _xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", this));
      ChildSetMember(ref _yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", this));
      ChildSetMember(ref _zBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("ZBoundaries", this));
    }


    protected void DeserializeSurrogate1(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      base.DeserializeSurrogate0(info);
      ChildSetMember(ref _xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", this));
      ChildSetMember(ref _yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", this));
      ChildSetMember(ref _zBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("ZBoundaries", this));
    }

    #endregion Serialization

    public XYZColumnPlotData(Altaxo.Data.DataTable dataTable, int groupNumber, Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn, Altaxo.Data.IReadableColumn zColumn)
      : base(dataTable, groupNumber, xColumn, yColumn, zColumn)
    {
    }

    protected XYZColumnPlotData(DataTableProxy dataTableProxy, int groupNumber, IReadableColumnProxy xColumnProxy, IReadableColumnProxy yColumnProxy, IReadableColumnProxy zColumnProxy)
      : base(dataTableProxy, groupNumber, xColumnProxy, yColumnProxy, zColumnProxy)
    {
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy from.</param>
    /// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
    public XYZColumnPlotData(XYZColumnPlotData from)
      : base(from)
    {
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
      foreach (var entry in base.GetDocumentNodeChildrenWithName())
        yield return entry;

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
    public override object Clone()
    {
      return new XYZColumnPlotData(this);
    }
    /// <summary>
    /// The selection of data rows to be plotted.
    /// </summary>
    public override IRowSelection DataRowSelection
    {
      get
      {
        return base.DataRowSelection;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(base.DataRowSelection, value))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false;
          base.DataRowSelection = value;
        }
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
    public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
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
    public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
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
    public void SetZBoundsFromTemplate(IPhysicalBoundaries val)
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
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    public override IEnumerable<GroupOfColumnsInformation> GetAdditionallyUsedColumns()
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
      yield return new ColumnInformation("X", XColumn, _independentVariables[0]?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { if (col is not null) XColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
      yield return new ColumnInformation("Y", YColumn, _independentVariables[1]?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { if (col is not null) YColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
      yield return new ColumnInformation("Z", ZColumn, _dependentVariables[0]?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { if (col is not null) ZColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
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
    public override Altaxo.Data.IReadableColumn XColumn
    {
      get
      {
        return base.XColumn;
      }
      set
      {
        if (!object.ReferenceEquals(base.XColumn, value))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
          base.XColumn = value;
        }
      }
    }

    [MaybeNull]
    public override Altaxo.Data.IReadableColumn YColumn
    {
      get
      {
        return base.YColumn;
      }
      set
      {
        if (!object.ReferenceEquals(base.YColumn, value))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
          base.YColumn = value;
        }
      }
    }

    [MaybeNull]
    public override Altaxo.Data.IReadableColumn ZColumn
    {
      get
      {
        return base.ZColumn;
      }
      set
      {
        if (!object.ReferenceEquals(base.ZColumn, value))
        {
          _isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
          base.ZColumn = value;
        }
      }
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
          foreach (var segment in DataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, _pointCount, dataTable.DataColumns, _pointCount))
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

    public IEnumerable<(AltaxoVariant x, AltaxoVariant y, AltaxoVariant z, int rowIndex)> GetDataPoints(bool ensureZIsNotEmpty)
    {
      var xColumn = XColumn;
      var yColumn = YColumn;
      var zColumn = ZColumn;
      if (xColumn is null || yColumn is null || zColumn is null || DataTable is null)
        yield break;

      int maxRowIndex = GetMaximumRowIndexFromDataColumns();
      foreach ((int start, int endExclusive) in DataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, maxRowIndex, DataTable.DataColumns, maxRowIndex))
      {
        for (int dataRowIdx = start; dataRowIdx < endExclusive; ++dataRowIdx)
        {
          if (xColumn.IsElementEmpty(dataRowIdx) || yColumn.IsElementEmpty(dataRowIdx) || (ensureZIsNotEmpty && zColumn.IsElementEmpty(dataRowIdx)))
            continue;
          yield return (xColumn[dataRowIdx], yColumn[dataRowIdx], zColumn[dataRowIdx], dataRowIdx);
        }
      }
    }

    public (bool success, double[] x, double[] y, Matrix<double> z) TryGetMesh(Func<AltaxoVariant, double> xTransformation, Func<AltaxoVariant, double> yTransformation, Func<AltaxoVariant, double> zTransformation)
    {
      // Find out if the plot data can be treated as meshed column data
      var xHash = new HashSet<double>();
      var yHash = new HashSet<double>();

      int numberOfValidPoints = 0;
      foreach (var (x, y, _, _) in GetDataPoints(ensureZIsNotEmpty: true))
      {
        xHash.Add(xTransformation(x));
        yHash.Add(yTransformation(y));
        ++numberOfValidPoints;
      }
      if (xHash.Count * yHash.Count != numberOfValidPoints)
        return (false, null, null, null);

      var xValues = xHash.ToArray();
      Array.Sort(xValues);
      var xDict = new Dictionary<double, int>(xValues.Select((v, i) => new KeyValuePair<double, int>(v, i)));
      var yValues = yHash.ToArray();
      Array.Sort(yValues);
      var yDict = new Dictionary<double, int>(yValues.Select((v, i) => new KeyValuePair<double, int>(v, i)));

      var zValues = CreateMatrix.Dense<double>(xValues.Length, yValues.Length);

      foreach (var (x, y, z, _) in GetDataPoints(ensureZIsNotEmpty: true))
      {
        var xx = xTransformation(x);
        var yy = yTransformation(y);

        int ix = xDict[xx];
        int iy = yDict[yy];
        zValues[ix, iy] = zTransformation(z);
      }
      return (true, xValues, yValues, zValues);
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
        foreach ((int start, int endExclusive) in DataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, maxRowIndex, dataTable.DataColumns, maxRowIndex))
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
      if (object.ReferenceEquals(sender, _independentVariables[0]) || object.ReferenceEquals(sender, _independentVariables[1]) || object.ReferenceEquals(sender, _dependentVariables[0]))
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
