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
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Plot
{
  using System.Diagnostics.CodeAnalysis;
  using Data;
  using Graph.Plot.Data;
  using Styles;

  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  public class XYFunctionPlotItem : G2DPlotItem
  {
    protected IXYFunctionPlotData _plotData;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYFunctionPlotItem", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYFunctionPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (XYFunctionPlotData)info.GetValue("Data", null);
        var lsps = (XYLineScatterPlotStyle)info.GetValue("Style", null);

        var ps = new G2DPlotStyleCollection();
        if (lsps.ScatterStyle is not null)
          ps.Add(new ScatterPlotStyle(lsps.ScatterStyle));
        if (lsps.XYPlotLineStyle is not null)
          ps.Add(new LinePlotStyle(lsps.XYPlotLineStyle));

        if (o is null)
        {
          return new XYFunctionPlotItem(pa, ps);
        }
        else
        {
          var s = (XYFunctionPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYFunctionPlotItem", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotItem), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYFunctionPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (IXYFunctionPlotData)info.GetValue("Data", null);
        var ps = (G2DPlotStyleCollection)info.GetValue("Style", null);

        if (o is null)
        {
          return new XYFunctionPlotItem(pa, ps);
        }
        else
        {
          var s = (XYFunctionPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      }
    }

    #endregion Serialization

    private System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetLocalDocumentNodeChildrenWithName()
    {
      if (_plotData is not null)
        yield return new Main.DocumentNodeAndName(_plotData, () => _plotData = null!, "Data");
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return GetLocalDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
    }

    public XYFunctionPlotItem(IXYFunctionPlotData pa, G2DPlotStyleCollection ps) : base(ps)
    {
      ChildSetMember(ref _plotData, pa);
    }
    /*
    /// <summary>
    /// Initializes a new instance of the <see cref="XYFunctionPlotItem"/> class. Intended for use in derived classes.
    /// </summary>
    protected XYFunctionPlotItem()
    {
    }
    */

    public XYFunctionPlotItem(XYFunctionPlotItem from) : base(from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_plotData))]
    protected void CopyFrom(XYFunctionPlotItem from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      ChildCopyToMember(ref _plotData, from._plotData);
    }


    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      if (obj is XYFunctionPlotItem from)
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
      return new XYFunctionPlotItem(this);
    }

    public override Main.IDocumentLeafNode DataObject
    {
      get { return _plotData; }
    }

    public virtual IXYFunctionPlotData Data
    {
      get { return _plotData; }
      set
      {
          if(ChildSetMember(ref _plotData, value ?? throw new ArgumentNullException(nameof(Data))))
            EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
      }
    }

    public override string GetName(int level)
    {
      return _plotData.ToString() ?? string.Empty;
    }

    public override string GetName(string style)
    {
      return GetName(0);
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
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public override void PrepareScales(IPlotArea layer)
    {
      // nothing really to do here
    }
  }
}
