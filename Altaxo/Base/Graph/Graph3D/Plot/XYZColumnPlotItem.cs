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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D.Plot
{
  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  public class XYZColumnPlotItem
    :
    G3DPlotItem,
    IXBoundsHolder,
    IYBoundsHolder,
    IZBoundsHolder
  {
    protected XYZColumnPlotData _plotData;

    #region Serialization

    /// <summary>
    /// 2016-05-31 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZColumnPlotItem), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYZColumnPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (XYZColumnPlotData)info.GetValue("Data", null);
        var ps = (G3DPlotStyleCollection)info.GetValue("Style", null);

        if (o is null)
        {
          return new XYZColumnPlotItem(pa, ps);
        }
        else
        {
          var s = (XYZColumnPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      }
    }

    #endregion Serialization

    private System.Collections.Generic.IEnumerable<DocumentNodeAndName> GetLocalDocumentNodeChildrenWithName()
    {
      if (_plotData is not null)
        yield return new DocumentNodeAndName(_plotData, () => _plotData = null!, "Data");
    }

    protected override System.Collections.Generic.IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return GetLocalDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
    }

    public XYZColumnPlotItem(XYZColumnPlotData pa, G3DPlotStyleCollection ps) : base(ps)
    {
      ChildSetMember(ref _plotData, pa);
    }

    public XYZColumnPlotItem(XYZColumnPlotItem from) : base(from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_plotData))]
    public void CopyFrom(XYZColumnPlotItem from, bool withBaseMembers)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      ChildCopyToMember(ref _plotData, from._plotData);
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is XYZColumnPlotItem from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    public override object Clone()
    {
      return new XYZColumnPlotItem(this);
    }

    public XYZColumnPlotData XYZColumnPlotData
    {
      get { return _plotData; }
    }

    public override Main.IDocumentLeafNode DataObject
    {
      get { return _plotData; }
    }

    public XYZColumnPlotData Data
    {
      get
      {
        return _plotData;
      }
      set
      {
        if (value is null)
          throw new System.ArgumentNullException();

        if (ChildSetMember(ref _plotData, value))
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
      }
    }

    public override string GetName(int level)
    {
      switch (level)
      {
        case 0:
          return GetNameByStyle(0x0100);

        case 1:
          return GetNameByStyle(0x0200);

        case 2:
          return GetNameByStyle(0x0211);

        default:
          return GetNameByStyle(0x0222);
      }
    }

    public virtual string GetNameByStyle(int style)
    {
      int st = style;
      int sx = (st & 0x000F);
      int sy = (st & 0x00F0) >> 4;
      int sz = (st & 0x0F00) >> 8;

      var stb = new System.Text.StringBuilder();
      if (sx > 0)
      {
        stb.Append(_plotData.GetXName(sx - 1));
        if (sx > 0 && sy > 0)
          stb.Append("(X)");
        if (sy > 0)
          stb.Append(",");
      }
      if (sy > 0)
      {
        stb.Append(_plotData.GetYName(sy - 1));
        if (sx > 0 && sy > 0)
          stb.Append("(Y)");
      }
      if (sz > 0)
      {
        stb.Append(_plotData.GetZName(sz - 1));
        if (sx > 0 && sy > 0 && sz > 0)
          stb.Append("(Z)");
      }

      return stb.ToString();
    }

    private string GetName(Altaxo.Data.IReadableColumn col, int level)
    {
      if (col is Altaxo.Data.DataColumn)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
        string tablename = table is null ? string.Empty : table.Name + "\\";
        string collectionname = table is null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return ((DataColumn)col).Name;
        else if (level == 1)
          return tablename + ((DataColumn)col).Name;
        else
          return tablename + collectionname + ((DataColumn)col).Name;
      }
      else if (col is not null)
        return col.FullName;
      else
        return string.Empty;
    }

    public override string ToString()
    {
      return GetName(int.MaxValue);
    }

    public override Processed3DPlotData? GetRangesAndPoints(IPlotArea layer)
    {
      return _plotData.GetRangesAndPoints(layer);
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public override void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      _plotData.VisitDocumentReferences(Report);
      base.VisitDocumentReferences(Report);
    }

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public override void PrepareScales(IPlotArea layer)
    {
      if (_plotData is not null)
        _plotData.CalculateCachedData(layer.XAxis.DataBoundsObject, layer.YAxis.DataBoundsObject, layer.ZAxis.DataBoundsObject);

      _plotStyles.PrepareScales(layer);
    }

    #region IXBoundsHolder Members

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeXBoundsInto(pb);
    }

    #endregion IXBoundsHolder Members

    #region IYBoundsHolder Members

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeYBoundsInto(pb);
    }

    #endregion IYBoundsHolder Members

    #region IZBoundsHolder Members

    public void MergeZBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeZBoundsInto(pb);
    }

    #endregion IZBoundsHolder Members
  }
}
