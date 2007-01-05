#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;


namespace Altaxo.Graph.Gdi.Plot
{
  using Styles;
  using Data;
  using Graph.Plot.Data;

  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
 
  [Serializable]
  public class XYColumnPlotItem 
    :
    G2DPlotItem,
    IXBoundsHolder, 
    IYBoundsHolder
  {

    protected XYColumnPlotData _plotData;


    #region Serialization
    /// <summary>Used to serialize theXYDataPlot Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYColumnPlotItem Version 0.
      /// </summary>
      /// <param name="obj">The XYColumnPlotItem to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }
      /// <summary>
      /// Deserializes the XYColumnPlotItem Version 0.
      /// </summary>
      /// <param name="obj">The empty XYColumnPlotItem object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYColumnPlotItem.</returns>
      public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;

        s.Data = (XYColumnPlotData)info.GetValue("Data", typeof(XYColumnPlotData));
        s.Style = (G2DPlotStyleCollection)info.GetValue("Style", typeof(G2DPlotStyleCollection));

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotItem", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      XYColumnPlotData _item;
      LabelPlotStyle _label;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYColumnPlotData pa = (XYColumnPlotData)info.GetValue("Data", typeof(XYColumnPlotData));
        XYLineScatterPlotStyle lsps = (XYLineScatterPlotStyle)info.GetValue("Style", typeof(XYLineScatterPlotStyle));
        if (lsps.XYPlotLineStyle != null)
          lsps.XYPlotLineStyle.LineSymbolGap = lsps.LineSymbolGap; // this has changed and is now hosted in the LineStyle itself

        G2DPlotStyleCollection ps = new G2DPlotStyleCollection(new IG2DPlotStyle[] { lsps.XYPlotLineStyle, lsps.ScatterStyle, lsps.XYPlotLabelStyle });
        if (lsps.XYPlotLabelStyle != null)
        {
          XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
          surr._item = pa;
          surr._label = lsps.XYPlotLabelStyle;
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.info_DeserializationFinished);
        }




        if (null == o)
        {
          return new XYColumnPlotItem(pa, ps);
        }
        else
        {
          XYColumnPlotItem s = (XYColumnPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }

      }

      void info_DeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        if (_item.LabelColumn != null)
        {
          _label.LabelColumn = _item.LabelColumn;
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.info_DeserializationFinished);
        }
      }

    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYColumnPlotItem", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotItem), 2)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYColumnPlotData pa = (XYColumnPlotData)info.GetValue("Data", typeof(XYColumnPlotData));
        G2DPlotStyleCollection ps = (G2DPlotStyleCollection)info.GetValue("Style", typeof(G2DPlotStyleCollection));

        if (null == o)
        {
          return new XYColumnPlotItem(pa, ps);
        }
        else
        {
          XYColumnPlotItem s = (XYColumnPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }

      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // Restore the event chain

      if (null != _plotData)
      {
        _plotData.Changed += new EventHandler(OnDataChangedEventHandler);
      }

      if (null != _plotStyles)
      {
        ((Main.IChangedEventSource)_plotStyles).Changed += new EventHandler(OnStyleChangedEventHandler);
      }
    }
    #endregion
  


    public XYColumnPlotItem(XYColumnPlotData pa, G2DPlotStyleCollection ps)
    {
      this.Data = pa;
      this.Style = ps;
    }

    public XYColumnPlotItem(XYColumnPlotItem from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(XYColumnPlotItem from)
    {
      CopyFrom((PlotItem)from);
    }
    protected override void CopyFrom(PlotItem fromb)
    {
      base.CopyFrom(fromb);

      XYColumnPlotItem from = fromb as XYColumnPlotItem;
      if (null != from)
      {
        this.Data = (XYColumnPlotData)from.Data.Clone(); // also wires the event
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

    public override object DataObject
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
        else
        {
          XYColumnPlotData oldvalue = _plotData;
          _plotData = value;
          if (!object.ReferenceEquals(value, oldvalue))
          {
            if (null != oldvalue)
            {
              oldvalue.Changed -= new EventHandler(OnDataChangedEventHandler);
              oldvalue.XBoundariesChanged -= new BoundaryChangedHandler(EhXBoundariesChanged);
              oldvalue.YBoundariesChanged -= new BoundaryChangedHandler(EhYBoundariesChanged);
              oldvalue.ParentObject = null;
            }
            if (null != value)
            {
              value.ParentObject = this;
              value.Changed += new EventHandler(OnDataChangedEventHandler);
              value.XBoundariesChanged += new BoundaryChangedHandler(EhXBoundariesChanged);
              value.YBoundariesChanged += new BoundaryChangedHandler(EhYBoundariesChanged);
            }

            OnDataChanged();
          }
        }
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

      System.Text.StringBuilder stb = new System.Text.StringBuilder();
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
        Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
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

    public override Processed2DPlotData GetRangesAndPoints(IPlotArea layer)
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
      if (null != this._plotData)
        _plotData.CalculateCachedData(layer.XAxis.DataBoundsObject, layer.YAxis.DataBoundsObject);
    }

   

   

    #region IXBoundsHolder Members

    void EhXBoundariesChanged(object sender, BoundariesChangedEventArgs args)
    {
      if (null != XBoundariesChanged)
        XBoundariesChanged(this, args);
    }

    [field:NonSerialized]
    public event BoundaryChangedHandler XBoundariesChanged;



    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      this._plotData.MergeXBoundsInto(pb);
    }

    #endregion

    #region IYBoundsHolder Members

    void EhYBoundariesChanged(object sender, BoundariesChangedEventArgs args)
    {
      if (null != YBoundariesChanged)
        YBoundariesChanged(this, args);
    }

    [field:NonSerialized]
    public event BoundaryChangedHandler YBoundariesChanged;



    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      this._plotData.MergeYBoundsInto(pb);
    }

    #endregion

   
  }
}
