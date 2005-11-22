#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Graph.Axes.Boundaries;


namespace Altaxo.Graph
{
 
  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYColumnPlotItem.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYColumnPlotItem : PlotItem, System.Runtime.Serialization.IDeserializationCallback, IXBoundsHolder, IYBoundsHolder
  {

    protected XYColumnPlotData m_PlotAssociation;
    protected XYPlotStyleCollection m_PlotStyle;

    // TODO : here should be a collection of PlotData, which can be accessed
    // by name, for instance "LabelData"

    // TODO : here should be a collection of PlotStyles

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
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data",s.m_PlotAssociation);  
        info.AddValue("Style",s.m_PlotStyle);  
      }
      /// <summary>
      /// Deserializes the XYColumnPlotItem Version 0.
      /// </summary>
      /// <param name="obj">The empty XYColumnPlotItem object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYColumnPlotItem.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;

        s.Data = (XYColumnPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        s.Style = (XYPlotStyleCollection)info.GetValue("Style",typeof(XYPlotStyleCollection));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotItem),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
        XYColumnPlotData _item;
      XYPlotLabelStyle _label;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data",s.m_PlotAssociation);  
        info.AddValue("Style",s.m_PlotStyle);  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYColumnPlotData pa = (XYColumnPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        XYLineScatterPlotStyle lsps  = (XYLineScatterPlotStyle)info.GetValue("Style",typeof(XYLineScatterPlotStyle));
        if (lsps.XYPlotLineStyle != null)
          lsps.XYPlotLineStyle.LineSymbolGap = lsps.LineSymbolGap; // this has changed and is now hosted in the LineStyle itself
        
        XYPlotStyleCollection ps = new XYPlotStyleCollection(new I2DPlotStyle[] { lsps.XYPlotLineStyle, lsps.ScatterStyle, lsps.XYPlotLabelStyle });
          if (lsps.XYPlotLabelStyle != null)
          {
            XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
            surr._item = pa;
            surr._label = lsps.XYPlotLabelStyle;
            info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.info_DeserializationFinished);
          }
         
        


        if(null==o)
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


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotItem), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data", s.m_PlotAssociation);
        info.AddValue("Style", s.m_PlotStyle);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYColumnPlotData pa = (XYColumnPlotData)info.GetValue("Data", typeof(XYColumnPlotData));
        XYPlotStyleCollection ps = (XYPlotStyleCollection)info.GetValue("Style", typeof(XYPlotStyleCollection));

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

      if(null!=m_PlotAssociation)
      {
        m_PlotAssociation.Changed += new EventHandler(OnDataChangedEventHandler);
      }

      if(null!=m_PlotStyle)
      {
        ((Main.IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
      }
    }
    #endregion



    public XYColumnPlotItem(XYColumnPlotData pa, XYPlotStyleCollection ps)
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
    this.Data = from.Data;   // also wires the event
    this.Style = (XYPlotStyleCollection)from.Style.Clone(); // also wires the event
  }

    public override object Clone()
    {
      return new XYColumnPlotItem(this);
    }

    public XYColumnPlotData XYColumnPlotData
    {
      get { return m_PlotAssociation; }
    }

    public  XYColumnPlotData Data
    {
      get { return m_PlotAssociation; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else
        {
          if(!object.ReferenceEquals(m_PlotAssociation,value))
          {
            if(null!=m_PlotAssociation)
            {
              m_PlotAssociation.Changed -= new EventHandler(OnDataChangedEventHandler);
              m_PlotAssociation.XBoundariesChanged -= new BoundaryChangedHandler(EhXBoundariesChanged);
              m_PlotAssociation.YBoundariesChanged -= new BoundaryChangedHandler(EhYBoundariesChanged);
            }

            m_PlotAssociation = (XYColumnPlotData)value;
          
            if(null!=m_PlotAssociation )
            {
              m_PlotAssociation.ParentObject = this;
              m_PlotAssociation.Changed += new EventHandler(OnDataChangedEventHandler);
              m_PlotAssociation.XBoundariesChanged += new BoundaryChangedHandler(EhXBoundariesChanged);
              m_PlotAssociation.YBoundariesChanged += new BoundaryChangedHandler(EhYBoundariesChanged);
            }

            OnDataChanged();
          }
        }
      }
    }

    public override object StyleObject
    {
      get { return m_PlotStyle; }
      set { this.Style = (XYPlotStyleCollection)value; }
    }
    public XYPlotStyleCollection Style
    {
      get { return m_PlotStyle; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else
        {
          if(!object.ReferenceEquals(m_PlotStyle,value))
          {
            // delete event wiring to old AbstractXYPlotStyle
            if(null!=m_PlotStyle)
            {
              ((Main.IChangedEventSource)m_PlotStyle).Changed -= new EventHandler(OnStyleChangedEventHandler);
            }
          
            m_PlotStyle = (XYPlotStyleCollection)value;

            // create event wire to new Plotstyle
            if(null!=m_PlotStyle)
            {
              m_PlotStyle.ParentObject = this;
              ((Main.IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
            }

            // indicate the style has changed
            OnStyleChanged();
          }
        }
      }
    }


    public override string GetName(int level)
    {
      switch(level)
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
      XYColumnPlotItemLabelTextStyle result=XYColumnPlotItemLabelTextStyle.YS;
      try
      {
        result = (XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(XYColumnPlotItemLabelTextStyle),style,true);
      }
      catch(Exception)
      {
      }
      return GetName(result);
    }


    public virtual string GetName(XYColumnPlotItemLabelTextStyle style)
    {
      int st = (int)style;
      int sx = st&0x0F;
      int sy = (st&0xF0)>>4;

      System.Text.StringBuilder stb = new System.Text.StringBuilder();
      if(sx>0)
      {
        stb.Append(this.GetName(m_PlotAssociation.XColumn,sx-1));
        if(sx>0 && sy>0)
          stb.Append("(X)");
        if(sy>0)
          stb.Append(",");
      }
      if(sy>0)
      {
        stb.Append(this.GetName(m_PlotAssociation.YColumn,sy-1));
        if(sx>0 && sy>0)
          stb.Append("(Y)");
      }

      return stb.ToString();
    }

    private string GetName(Data.IReadableColumn col, int level)
    {
      if (col is Data.DataColumn)
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

    public override void Paint(Graphics g, IPlotArea layer)
    {
      if(null!=this.m_PlotStyle)
      {
        PlotRangeList rangeList;
        PointF[] plotPoints;
        this.m_PlotAssociation.GetRangesAndPoints(layer, out rangeList, out plotPoints);
        if (rangeList != null)
          this.m_PlotStyle.Paint(g, layer, rangeList, plotPoints);
      }
    }

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public override void UpdateCachedData(IPlotArea layer)
    {
      if(null!=this.m_PlotAssociation)
        m_PlotAssociation.CalculateCachedData();
    }

    /// <summary>
    /// Test wether the mouse hits a plot item. 
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
    public override IHitTestObject HitTest(IPlotArea layer, PointF hitpoint)
    {
      XYColumnPlotData myPlotAssociation = this.m_PlotAssociation;
      if(null==myPlotAssociation)
        return null;

      PlotRangeList rangeList;
      PointF[] ptArray;
      if(myPlotAssociation.GetRangesAndPoints(layer,out rangeList,out ptArray))
      {
        if(ptArray.Length<2048)
        {
          GraphicsPath gp = new GraphicsPath();
          gp.AddLines(ptArray);
          if(gp.IsOutlineVisible(hitpoint.X,hitpoint.Y,new Pen(Color.Black,5)))
          {
            gp.Widen(new Pen(Color.Black,5));
            return new HitTestObject(gp,this);
          }
        }
        else // we have too much points for the graphics path, so make a hit test first
        {
         
          int hitindex = -1;
          for(int i=1;i<ptArray.Length;i++)
          {
            if(Drawing2DRelated.IsPointIntoDistance(ptArray[i-1],ptArray[i],hitpoint,5))
            {
              hitindex =i;
              break;
            }
          }
          if(hitindex<0)
            return null;
          GraphicsPath gp = new GraphicsPath();
          int start = Math.Max(0,hitindex-1);
          gp.AddLine(ptArray[start],ptArray[start+1]);
          gp.AddLine(ptArray[start+1],ptArray[start+2]);
          gp.Widen(new Pen(Color.Black,5));
          return new HitTestObject(gp,this);
        }
      }
    

      return null;
    }


    /// <summary>
    /// Returns the index of a scatter point that is nearest to the location <c>hitpoint</c>
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>The information about the point that is nearest to the location, or null if it can not be determined.</returns>
    public XYScatterPointInformation GetNearestPlotPoint(IPlotArea layer, PointF hitpoint)
    {

   
      XYColumnPlotData myPlotAssociation = this.m_PlotAssociation;
      if(null==myPlotAssociation)
        return null;

      PlotRangeList rangeList;
      PointF[] ptArray;
      if(myPlotAssociation.GetRangesAndPoints(layer,out rangeList,out ptArray))
      {
        double mindistance = double.MaxValue;
        int minindex = -1;
        for(int i=1;i<ptArray.Length;i++)
        {
          double distance = Drawing2DRelated.SquareDistanceLineToPoint(ptArray[i-1],ptArray[i],hitpoint);
          if(distance<mindistance)
          {
            mindistance = distance;
            minindex = Drawing2DRelated.Distance(ptArray[i-1],hitpoint)<Drawing2DRelated.Distance(ptArray[i],hitpoint) ? i-1 : i;
          }
        }
        // ok, minindex is the point we are looking for
        // so we have a look in the rangeList, what row it belongs to
        int rowindex = rangeList.GetRowIndexForPlotIndex(minindex);

        return new XYScatterPointInformation(ptArray[minindex],rowindex,minindex);
      }


      return null;
    }

     

    /// <summary>
    /// For a given plot point of index oldplotindex, finds the index and coordinates of a plot point
    /// of index oldplotindex+increment.
    /// </summary>
    /// <param name="layer">The layer this plot belongs to.</param>
    /// <param name="oldplotindex">Old plot index.</param>
    /// <param name="increment">Increment to the plot index.</param>
    /// <returns>Information about the new plot point find at position (oldplotindex+increment). Returns null if no such point exists.</returns>
    public XYScatterPointInformation GetNextPlotPoint(IPlotArea layer, int oldplotindex, int increment)
    {
 
      XYColumnPlotData myPlotAssociation = this.m_PlotAssociation;
      if(null==myPlotAssociation)
        return null;

      PlotRangeList rangeList;
      PointF[] ptArray;
      if(myPlotAssociation.GetRangesAndPoints(layer,out rangeList,out ptArray))
      {
        if(ptArray.Length==0)
          return null;

        int minindex = oldplotindex + increment;
        minindex = Math.Max(minindex,0);
        minindex = Math.Min(minindex,ptArray.Length-1);
        // ok, minindex is the point we are looking for
        // so we have a look in the rangeList, what row it belongs to
        int rowindex = rangeList.GetRowIndexForPlotIndex(minindex);
        return new XYScatterPointInformation(ptArray[minindex],rowindex,minindex);
      }


      return null;
    }
    
    #region IXBoundsHolder Members

    void EhXBoundariesChanged(object sender, BoundariesChangedEventArgs args)
    {
      if(null!=XBoundariesChanged)
        XBoundariesChanged(this,args);
    }

    public event BoundaryChangedHandler XBoundariesChanged;

    public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      this.m_PlotAssociation.SetXBoundsFromTemplate(val);
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      this.m_PlotAssociation.MergeXBoundsInto(pb);
    }

    #endregion

    #region IYBoundsHolder Members

    void EhYBoundariesChanged(object sender, BoundariesChangedEventArgs args)
    {
      if(null!=YBoundariesChanged)
        YBoundariesChanged(this,args);
    }

    public event BoundaryChangedHandler YBoundariesChanged;

    public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      this.m_PlotAssociation.SetYBoundsFromTemplate(val);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      this.m_PlotAssociation.MergeYBoundsInto(pb);
    }

    #endregion

    /*
    #region I2DGroupablePlotStyle Members

    object I2DGroupablePlotStyle.PlotStyle
    {
      get
      {
        return this.m_PlotStyle;
      }
    }

    public bool IsColorSupported
    {
      get
      {
        return this.m_PlotStyle.IsColorProvider;
      }
    }

    public Color Color
    {
      get
      {
        return this.m_PlotStyle.Color;
      }
    }

    public bool IsXYLineStyleSupported
    {
      get
      {
        return this.m_PlotStyle.IsXYLineStyleSupported;
      }
    }

    public System.Drawing.Drawing2D.DashStyle XYLineStyle
    {
      get
      {
        return this.m_PlotStyle.XYLineStyle;
      }
    }

    public bool IsXYScatterStyleSupported
    {
      get
      {
        return this.m_PlotStyle.IsXYScatterStyleSupported;
      }
    }

    public XYPlotScatterStyles.ShapeAndStyle XYScatterStyle
    {
      get
      {
        return this.m_PlotStyle.XYScatterStyle;
      }
    }

    public void SetIncrementalStyle(I2DGroupablePlotStyle pstemplate, Altaxo.Graph.PlotGroupStyle style, bool concurrently, bool strict, int step)
    {
      this.m_PlotStyle.SetIncrementalStyle(pstemplate,style,concurrently,strict,step);
    }

    #endregion
     */
  }
}
