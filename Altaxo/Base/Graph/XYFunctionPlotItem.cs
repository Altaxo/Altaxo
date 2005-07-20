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
using Altaxo.Graph.Axes;

namespace Altaxo.Graph
{
  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYFunctionPlotItem.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYFunctionPlotItem : PlotItem, System.Runtime.Serialization.IDeserializationCallback, Graph.I2DPlotItemStyle
  {
    protected XYFunctionPlotData m_PlotData;
    protected XYPlotStyleCollection  m_PlotStyle;

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
        XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
        info.AddValue("Data",s.m_PlotData);  
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
        XYFunctionPlotItem s = (XYFunctionPlotItem)obj;

        s.m_PlotData = (XYFunctionPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        s.m_PlotStyle = (XYPlotStyleCollection)info.GetValue("Style", typeof(XYPlotStyleCollection));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotItem),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
        info.AddValue("Data",s.m_PlotData);  
        info.AddValue("Style",s.m_PlotStyle); 
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYFunctionPlotData pa  = (XYFunctionPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        XYLineScatterPlotStyle lsps = (XYLineScatterPlotStyle)info.GetValue("Style",typeof(XYLineScatterPlotStyle));

        XYPlotStyleCollection ps = new XYPlotStyleCollection(new I2DPlotStyle[] { lsps.XYLineStyle, lsps.XYScatterStyle });
        
        if(null==o)
        {
          return new XYFunctionPlotItem(pa,ps);
        }
        else
        {
          XYFunctionPlotItem s = (XYFunctionPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotItem), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
        info.AddValue("Data", s.m_PlotData);
        info.AddValue("Style", s.m_PlotStyle);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYFunctionPlotData pa = (XYFunctionPlotData)info.GetValue("Data", typeof(XYColumnPlotData));
        XYPlotStyleCollection ps = (XYPlotStyleCollection)info.GetValue("Style", typeof(XYPlotStyleCollection));

        if (null == o)
        {
          return new XYFunctionPlotItem(pa, ps);
        }
        else
        {
          XYFunctionPlotItem s = (XYFunctionPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }

      }
    }

    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // Restore the event chain

      if(null!=m_PlotData)
      {
        m_PlotData.Changed += new EventHandler(OnDataChangedEventHandler);
      }

      if(null!=m_PlotStyle)
      {
        ((Main.IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
      }
    }
    #endregion



    public XYFunctionPlotItem(XYFunctionPlotData pa, XYPlotStyleCollection ps)
    {
      this.Data = pa;
      this.Style = ps;
    }

    public XYFunctionPlotItem(XYFunctionPlotItem from)
    {
      this.Data = from.Data;   // also wires the event
      this.Style = from.Style; // also wires the event
    }

    public override object Clone()
    {
      return new XYFunctionPlotItem(this);
    }


    public XYFunctionPlotData Data
    {
      get { return m_PlotData; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else
        {
          if(!object.ReferenceEquals(m_PlotData,value))
          {
            if(null!=m_PlotData)
            {
              m_PlotData.Changed -= new EventHandler(OnDataChangedEventHandler);
            }

            m_PlotData = (XYFunctionPlotData)value;
          
            if(null!=m_PlotData)
            {
              m_PlotData.Changed += new EventHandler(OnDataChangedEventHandler);
            }

            OnDataChanged();
          }
        }
      }
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
      return m_PlotData.ToString();
    }
    public override string GetName(string style)
    {
      return GetName(0);
    }
    public override string ToString()
    {
      return GetName(int.MaxValue);
    }

    

    public override void Paint(Graphics g, IPlotArea layer)
    {
      const int functionPoints = 1000;
      const double MaxRelativeValue = 1E6;


      // allocate an array PointF to hold the line points
      PointF[] ptArray = new PointF[functionPoints];

      // double xorg = layer.XAxis.Org;
      // double xend = layer.XAxis.End;
      // Fill the array with values
      // only the points where x and y are not NaNs are plotted!

      int i, j;

      bool bInPlotSpace = true;
      int rangeStart = 0;
      PlotRangeList rangeList = new PlotRangeList();
      I2DTo2DConverter logicalToArea = layer.LogicalToAreaConversion;

      NumericalAxis xaxis = layer.XAxis as NumericalAxis;
      NumericalAxis yaxis = layer.YAxis as NumericalAxis;
      if (xaxis == null || yaxis == null)
        return;

      for (i = 0, j = 0; i < functionPoints; i++)
      {
        double x_rel = ((double)i) / (functionPoints - 1);
        double x = xaxis.NormalToPhysical(x_rel);
        double y = this.m_PlotData.Evaluate(x);

        if (Double.IsNaN(x) || Double.IsNaN(y))
        {
          if (!bInPlotSpace)
          {
            bInPlotSpace = true;
            rangeList.Add(new PlotRange(rangeStart, j));
          }
          continue;
        }


        // double x_rel = layer.XAxis.PhysicalToNormal(x);
        double y_rel = yaxis.PhysicalToNormal(y);

        // chop relative values to an range of about -+ 10^6
        if (y_rel > MaxRelativeValue)
          y_rel = MaxRelativeValue;
        if (y_rel < -MaxRelativeValue)
          y_rel = -MaxRelativeValue;

        // after the conversion to relative coordinates it is possible
        // that with the choosen axis the point is undefined 
        // (for instance negative values on a logarithmic axis)
        // in this case the returned value is NaN
        double xcoord, ycoord;
        if (logicalToArea.Convert(x_rel, y_rel, out xcoord, out ycoord))
        {
          if (bInPlotSpace)
          {
            bInPlotSpace = false;
            rangeStart = j;
          }
          ptArray[j].X = (float)xcoord;
          ptArray[j].Y = (float)ycoord;
          j++;
        }
        else
        {
          if (!bInPlotSpace)
          {
            bInPlotSpace = true;
            rangeList.Add(new PlotRange(rangeStart, j));
          }
        }
      } // end for
      if (!bInPlotSpace)
      {
        bInPlotSpace = true;
        rangeList.Add(new PlotRange(rangeStart, j)); // add the last range
      }



      // ------------------ end of creation of plot array -----------------------------------------------
      // in the plot array ptArray now we have the coordinates of each point

      // now plot the point array

      if (null != this.m_PlotStyle)
      {
        m_PlotStyle.Paint(g, layer, rangeList, ptArray);
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
      // nothing really to do here
    }
    #region I2DPlotStyle Members

    public bool IsColorProvider
    {
      get
      {
        return true;
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
        return true;
      }
    }

    public XYPlotLineStyle XYLineStyle
    {
      get
      {
        return this.m_PlotStyle.XYPlotLineStyle;
      }
    }

    public bool IsXYScatterStyleSupported
    {
      get
      {
        return false;
      }
    }

    public XYPlotScatterStyle XYScatterStyle
    {
      get
      {
        
        return null;
      }
    }

    public void SetIncrementalStyle(I2DPlotItemStyle pstemplate, Altaxo.Graph.PlotGroupStyle style, int step)
    {
        ((XYPlotStyleCollection)m_PlotStyle).SetIncrementalStyle(pstemplate,style,step);
    }

    #endregion
  }
}
