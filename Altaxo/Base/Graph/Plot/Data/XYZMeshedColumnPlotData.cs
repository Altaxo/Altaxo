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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  [Serializable]
  public class XYZMeshedColumnPlotData
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    System.ICloneable
  {
    protected DataTableMatrixProxy _matrixProxy;

    // cached or temporary data
    [NonSerialized]
    protected IPhysicalBoundaries _xBoundaries;

    [NonSerialized]
    protected IPhysicalBoundaries _yBoundaries;

    [NonSerialized]
    protected IPhysicalBoundaries _vBoundaries;

    [NonSerialized]
    protected bool _isCachedDataValid = false;

    /// <summary>
    /// Gets or sets the plot range start. Currently, this value is always 0.
    /// </summary>
    public int PlotRangeStart { get { return 0; } set { } }

    /// <summary>
    /// Gets or sets the plot range length. Currently, this value is always <c>int.MaxValue</c>.
    /// </summary>
    public int PlotRangeLength { get { return int.MaxValue; } set { } }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYZEquidistantMeshColumnPlotData", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling a deprecated serialization handler for XYZMeshedColumnPlotData");
        /*
                XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;

                if(s.m_XColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_XColumn).ParentObject))
                {
                    info.AddValue("XColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_XColumn));
                }
                else
                {
                    info.AddValue("XColumn",s.m_XColumn);
                }

                if(s.m_YColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_YColumn).ParentObject))
                {
                    info.AddValue("YColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_YColumn));
                }
                else
                {
                    info.AddValue("YColumn",s.m_YColumn);
                }

                info.CreateArray("DataColumns",s.m_DataColumns.Length);
                for(int i=0;i<s.m_DataColumns.Length;i++)
                {
                    Altaxo.Data.IReadableColumn col = s.m_DataColumns[i];
                    if(col is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)col).ParentObject))
                    {
                        info.AddValue("e",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)col));
                    }
                    else
                    {
                        info.AddValue("e",col);
                    }
                }
                info.CommitArray();

                info.AddValue("XBoundaries",s.m_xBoundaries);
                info.AddValue("YBoundaries",s.m_yBoundaries);
                info.AddValue("VBoundaries",s.m_vBoundaries);
                */
      }

      private Main.AbsoluteDocumentPath? _xColumnPath = null;
      private Main.AbsoluteDocumentPath? _yColumnPath = null;
      private Main.AbsoluteDocumentPath?[]? _vColumnPaths = null;

      private IReadableColumnProxy? _xColumnProxy = null;
      private IReadableColumnProxy? _yColumnProxy = null;
      private IReadableColumnProxy?[]? _vColumnProxies = null;

      private XYZMeshedColumnPlotData? _plotAssociation = null;

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool bSurrogateUsed = false;

        var s = (XYZMeshedColumnPlotData?)o ?? new XYZMeshedColumnPlotData(info);

        var surr = new XmlSerializationSurrogate0();

#pragma warning disable 618
        s._matrixProxy = DataTableMatrixProxy.CreateEmptyInstance(); // this instance is replaced later in the deserialization callback function and is intended to avoid null reference errors
#pragma warning restore 618

        object deserobj;
        deserobj = info.GetValue("XColumn", s);
        if (deserobj is Main.AbsoluteDocumentPath)
        {
          surr._xColumnPath = (Main.AbsoluteDocumentPath)deserobj;
          bSurrogateUsed = true;
        }
        else
        {
          surr._xColumnProxy = ReadableColumnProxyBase.FromColumn((Altaxo.Data.INumericColumn)deserobj);
        }

        deserobj = info.GetValue("YColumn", s);
        if (deserobj is Main.AbsoluteDocumentPath)
        {
          surr._yColumnPath = (Main.AbsoluteDocumentPath)deserobj;
          bSurrogateUsed = true;
        }
        else
        {
          surr._yColumnProxy = ReadableColumnProxyBase.FromColumn((Altaxo.Data.INumericColumn)deserobj);
        }

        int count = info.OpenArray();
        surr._vColumnPaths = new Main.AbsoluteDocumentPath[count];
        surr._vColumnProxies = new IReadableColumnProxy[count];
        for (int i = 0; i < count; i++)
        {
          deserobj = info.GetValue("e", s);
          if (deserobj is Main.AbsoluteDocumentPath)
          {
            surr._vColumnPaths[i] = (Main.AbsoluteDocumentPath)deserobj;
            bSurrogateUsed = true;
          }
          else
          {
            surr._vColumnProxies[i] = ReadableColumnProxyBase.FromColumn((Altaxo.Data.IReadableColumn)deserobj);
          }
        }
        info.CloseArray(count);

        s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
        s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
        s._vBoundaries = (IPhysicalBoundaries)info.GetValue("VBoundaries", s);

        s._xBoundaries.ParentObject = s;
        s._yBoundaries.ParentObject = s;
        s._vBoundaries.ParentObject = s;

        if (bSurrogateUsed)
        {
          surr._plotAssociation = s;
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        }

        return s;
      }

      public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
      {
        bool bAllResolved = true;

        if (_plotAssociation is not null)
        {
          if (_xColumnPath is not null)
          {
            var xColumn = Main.AbsoluteDocumentPath.GetObject(_xColumnPath, _plotAssociation, documentRoot);
            bAllResolved &= (xColumn is not null);
            if (xColumn is Altaxo.Data.INumericColumn xn)
            {
              _xColumnPath = null;
              _xColumnProxy = ReadableColumnProxyBase.FromColumn(xn);
            }
          }

          if (_yColumnPath is not null)
          {
            var yColumn = Main.AbsoluteDocumentPath.GetObject(_yColumnPath, _plotAssociation, documentRoot);
            bAllResolved &= (yColumn is not null);
            if (yColumn is Altaxo.Data.INumericColumn yn)
            {
              _yColumnPath = null;
              _yColumnProxy = ReadableColumnProxyBase.FromColumn(yn);
            }
          }

          if (_vColumnPaths is not null && _vColumnProxies is not null)
          {
            for (int i = 0; i < _vColumnPaths.Length; i++)
            {
              if (_vColumnPaths[i] is { } vpath)
              {
                var vColumn = Main.AbsoluteDocumentPath.GetObject(vpath, _plotAssociation, documentRoot);
                bAllResolved &= (vColumn is not null);
                if (vColumn is Altaxo.Data.IReadableColumn vn)
                {
                  _vColumnPaths[i] = null;
                  _vColumnProxies[i] = ReadableColumnProxyBase.FromColumn(vn);
                }
              }
            }
          }


          if (bAllResolved || isFinallyCall)
          {
            info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhDeserializationFinished);
#pragma warning disable 618
            _plotAssociation._matrixProxy = new DataTableMatrixProxy(_xColumnProxy!, _yColumnProxy!, _vColumnProxies!) { ParentObject = _plotAssociation };
#pragma warning restore 618
          }
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions not supported.");
        /*

                XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;

                info.AddValue("XColumn", s._xColumn);
                info.AddValue("YColumn", s._yColumn);

                info.CreateArray("DataColumns", s._dataColumns.Length);
                for (int i = 0; i < s._dataColumns.Length; i++)
                {
                    info.AddValue("e", s._dataColumns[i]);
                }
                info.CommitArray();

                info.AddValue("XBoundaries", s._xBoundaries);
                info.AddValue("YBoundaries", s._yBoundaries);
                info.AddValue("VBoundaries", s._vBoundaries);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        XYZMeshedColumnPlotData s = o is not null ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData(info);

        var _xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
        var _yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);

        int count = info.OpenArray();
        var _dataColumns = new IReadableColumnProxy[count];
        for (int i = 0; i < count; i++)
        {
          _dataColumns[i] = (IReadableColumnProxy)info.GetValue("e", s);
        }
        info.CloseArray(count);

#pragma warning disable 618
        s._matrixProxy = new DataTableMatrixProxy(_xColumn, _yColumn, _dataColumns);
#pragma warning restore 618

        s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
        s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
        s._vBoundaries = (IPhysicalBoundaries)info.GetValue("VBoundaries", s);

        s._matrixProxy.ParentObject = s;
        s._xBoundaries.ParentObject = s;
        s._yBoundaries.ParentObject = s;
        s._vBoundaries.ParentObject = s;

        s._isCachedDataValid = false;

        return s;
      }
    }

    /// <summary>2014-07-08 using _matrixProxy instead of single proxies for columns</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYZMeshedColumnPlotData)obj;

        info.AddValue("MatrixProxy", s._matrixProxy);

        info.AddValue("XBoundaries", s._xBoundaries);
        info.AddValue("YBoundaries", s._yBoundaries);
        info.AddValue("VBoundaries", s._vBoundaries);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        XYZMeshedColumnPlotData s = o is not null ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData(info);

        s._matrixProxy = (DataTableMatrixProxy)info.GetValue("MatrixProxy", s);

        s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
        s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
        s._vBoundaries = (IPhysicalBoundaries)info.GetValue("VBoundaries", s);

        s._matrixProxy.ParentObject = s;
        s._xBoundaries.ParentObject = s;
        s._yBoundaries.ParentObject = s;
        s._vBoundaries.ParentObject = s;

        s._isCachedDataValid = false;

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected XYZMeshedColumnPlotData(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public XYZMeshedColumnPlotData(DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns, IAscendingIntegerCollection selectedPropertyColumns)
    {
      _matrixProxy = new DataTableMatrixProxy(table, selectedDataRows, selectedDataColumns, selectedPropertyColumns) { ParentObject = this };
      SetXBoundsFromTemplate(new FiniteNumericalBoundaries());
      SetYBoundsFromTemplate(new FiniteNumericalBoundaries());
      SetVBoundsFromTemplate(new FiniteNumericalBoundaries());
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy from.</param>
    /// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
    public XYZMeshedColumnPlotData(XYZMeshedColumnPlotData from)
    {
      CopyHelper.Copy(ref _matrixProxy, from._matrixProxy);
      _matrixProxy.ParentObject = this;

      SetXBoundsFromTemplate(new FiniteNumericalBoundaries());
      SetYBoundsFromTemplate(new FiniteNumericalBoundaries());
      SetVBoundsFromTemplate(new FiniteNumericalBoundaries());
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_matrixProxy is not null)
        yield return new Main.DocumentNodeAndName(_matrixProxy, "Matrix");

      if (_xBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_xBoundaries, "XBoundaries");

      if (_yBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_yBoundaries, "YBoundaries");

      if (_vBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_vBoundaries, "VBoundaries");
    }

    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    /// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
    public object Clone()
    {
      return new XYZMeshedColumnPlotData(this);
    }

    public DataTableMatrixProxy DataTableMatrix
    {
      get
      {
        return _matrixProxy;
      }
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if (!_isCachedDataValid)
        CalculateCachedData();
      pb.Add(_xBoundaries);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (!_isCachedDataValid)
        CalculateCachedData();
      pb.Add(_yBoundaries);
    }

    public void MergeVBoundsInto(IPhysicalBoundaries pb)
    {
      if (!_isCachedDataValid)
        CalculateCachedData();
      pb.Add(_vBoundaries);
    }

    [MemberNotNull(nameof(_xBoundaries))]
    public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_xBoundaries is null || val.GetType() != _xBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _xBoundaries, val))
        {
          _isCachedDataValid = false;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    [MemberNotNull(nameof(_yBoundaries))]
    public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_yBoundaries is null || val.GetType() != _yBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _yBoundaries, val))
        {
          _isCachedDataValid = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    [MemberNotNull(nameof(_vBoundaries))]
    public void SetVBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_vBoundaries is null || val.GetType() != _vBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _vBoundaries, val))
        {
          _isCachedDataValid = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public int RowCount
    {
      get
      {
        return _matrixProxy.RowCount;
      }
    }

    public int ColumnCount
    {
      get
      {
        return _matrixProxy.ColumnCount;
      }
    }

    public Altaxo.Data.IReadableColumn? GetDataColumn(int i)
    {
      return _matrixProxy.GetDataColumnProxy(i).Document();
    }

    public Altaxo.Data.IReadableColumn? XColumn
    {
      get
      {
        return _matrixProxy.RowHeaderColumn;
      }
    }

    public Altaxo.Data.IReadableColumn? YColumn
    {
      get
      {
        return _matrixProxy.ColumnHeaderColumn;
      }
    }

    public override string ToString()
    {
      var colCount = _matrixProxy.ColumnCount;

      if (colCount > 0)
        return string.Format("PictureData {0}-{1}", _matrixProxy.GetDataColumnProxy(0).GetName(2), _matrixProxy.GetDataColumnProxy(colCount - 1).GetName(2));
      else
        return "Empty (no data)";
    }

    public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds, IPhysicalBoundaries? zBounds = null)
    {
      if (IsDisposeInProgress)
        return;

      if (_xBoundaries is null || (xBounds is not null && _xBoundaries.GetType() != xBounds.GetType()))
        SetXBoundsFromTemplate(xBounds);

      if (_yBoundaries is null || (yBounds is not null && _yBoundaries.GetType() != yBounds.GetType()))
        SetYBoundsFromTemplate(yBounds);

      if (zBounds is not null && (_vBoundaries is null || _vBoundaries.GetType() != zBounds.GetType()))
        SetVBoundsFromTemplate(zBounds);

      CalculateCachedData();
    }

    public void CalculateCachedData()
    {
      if (IsDisposeInProgress)
        return;

      if (0 == RowCount || 0 == ColumnCount)
        return;

      using (var suspendTokenX = _xBoundaries.SuspendGetToken())
      {
        using (var suspendTokenY = _yBoundaries.SuspendGetToken())
        {
          using (var suspendTokenV = _vBoundaries.SuspendGetToken())
          {
            _xBoundaries.Reset();
            _yBoundaries.Reset();
            _vBoundaries.Reset();

            _matrixProxy.ForEachMatrixElementDo((col, idx) => _vBoundaries.Add(col, idx));
            _matrixProxy.ForEachRowHeaderElementDo((col, idx) => _xBoundaries.Add(col, idx));
            _matrixProxy.ForEachColumnHeaderElementDo((col, idx) => _yBoundaries.Add(col, idx));

            // now the cached data are valid
            _isCachedDataValid = true;

            // now when the cached data are valid, we can reenable the events
            suspendTokenV.Resume();
          }
          suspendTokenY.Resume();
        }
        suspendTokenX.Resume();
      }
    }

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
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
        else if (object.ReferenceEquals(sender, _vBoundaries))
        {
          eAsBCEA.SetVBoundaryChangedFlag();
        }
      }

      if (object.ReferenceEquals(sender, _matrixProxy))
      {
        _isCachedDataValid = false;
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed event handling

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      _matrixProxy.VisitDocumentReferences(Report);
    }
  }
} // end name space
