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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Gdi.Plot
{
  using System.Diagnostics.CodeAnalysis;
  using Data;
  using Graph.Plot.Data;
  using Styles;

  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>

  public class XYColumnPlotItem
    :
    G2DPlotItem,
    IXBoundsHolder,
    IYBoundsHolder
  {
    protected XYColumnPlotData _plotData;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotItem", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private XYColumnPlotData? _item;
      private LabelPlotStyle? _label;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYColumnPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (XYColumnPlotData)info.GetValue("Data", null);
        var lsps = (XYLineScatterPlotStyle)info.GetValue("Style", null);
        if (lsps.XYPlotLineStyle != null)
          lsps.XYPlotLineStyle.UseSymbolGap = lsps.LineSymbolGap; // this has changed and is now hosted in the LineStyle itself

        var ps = new G2DPlotStyleCollection(new IG2DPlotStyle?[] { lsps.XYPlotLineStyle, lsps.ScatterStyle, lsps.XYPlotLabelStyle });
        if (lsps.XYPlotLabelStyle != null)
        {
          var surr = new XmlSerializationSurrogate0
          {
            _item = pa,
            _label = lsps.XYPlotLabelStyle
          };
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.info_DeserializationFinished);
        }

        if (null == o)
        {
          return new XYColumnPlotItem(pa, ps);
        }
        else
        {
          var s = (XYColumnPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      }

      private void info_DeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
      {
        if (_label is not null && _item?.LabelColumn is not null)
        {
          _label.LabelColumn = _item.LabelColumn;
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(info_DeserializationFinished);
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotItem", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotItem), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYColumnPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (XYColumnPlotData)info.GetValue("Data", null);
        var ps = (G2DPlotStyleCollection)info.GetValue("Style", null);

        if (null == o)
        {
          return new XYColumnPlotItem(pa, ps);
        }
        else
        {
          var s = (XYColumnPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      }
    }

    #endregion Serialization

    private System.Collections.Generic.IEnumerable<DocumentNodeAndName> GetLocalDocumentNodeChildrenWithName()
    {
      if (null != _plotData)
        yield return new DocumentNodeAndName(_plotData, () => _plotData = null!, "Data");
    }

    protected override System.Collections.Generic.IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return GetLocalDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
    }

    public XYColumnPlotItem(XYColumnPlotData pa, G2DPlotStyleCollection ps) : base(ps)
    {
      ChildSetMember(ref _plotData, pa);
    }

    public XYColumnPlotItem(XYColumnPlotItem from) : base(from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_plotData))]
    public void CopyFrom(XYColumnPlotItem from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      ChildCopyToMember(ref _plotData, from._plotData);
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      if (obj is XYColumnPlotItem from)
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
      return new XYColumnPlotItem(this);
    }

    public XYColumnPlotData XYColumnPlotData
    {
      get { return _plotData; }
    }

    public override Main.IDocumentLeafNode DataObject
    {
      get { return _plotData; }
    }

    public XYColumnPlotData Data
    {
      get
      {
        return _plotData;
      }
      set
      {
        if (null == value)
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
          return GetName(XYColumnPlotItemLabelTextStyle.YS);

        case 1:
          return GetName(XYColumnPlotItemLabelTextStyle.YM);

        case 2:
          return GetName(XYColumnPlotItemLabelTextStyle.XSYM);

        default:
          return GetName(XYColumnPlotItemLabelTextStyle.XMYM);
      }
    }

    public override string GetName(string style)
    {
      XYColumnPlotItemLabelTextStyle result = XYColumnPlotItemLabelTextStyle.YS;
      try
      {
        result = (XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(XYColumnPlotItemLabelTextStyle), style, true);
      }
      catch (Exception)
      {
      }
      return GetName(result);
    }

    public virtual string GetName(XYColumnPlotItemLabelTextStyle style)
    {
      int st = (int)style;
      int sx = st & 0x0F;
      int sy = (st & 0xF0) >> 4;

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

      return stb.ToString();
    }

    private string GetName(Altaxo.Data.IReadableColumn col, int level)
    {
      if (col is Altaxo.Data.DataColumn)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
        string tablename = table == null ? string.Empty : table.Name + "\\";
        string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return ((DataColumn)col).Name;
        else if (level == 1)
          return tablename + ((DataColumn)col).Name;
        else
          return tablename + collectionname + ((DataColumn)col).Name;
      }
      else if (col != null)
        return col.FullName;
      else
        return string.Empty;
    }

    public override string ToString()
    {
      return GetName(int.MaxValue);
    }

    public override Processed2DPlotData? GetRangesAndPoints(IPlotArea layer)
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
      if (null != _plotData)
        _plotData.CalculateCachedData(layer.XAxis.DataBoundsObject, layer.YAxis.DataBoundsObject);

      _plotStyles?.PrepareScales(layer);
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
  }
}
