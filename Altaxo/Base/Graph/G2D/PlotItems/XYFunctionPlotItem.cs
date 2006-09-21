#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Graph.Scales;

namespace Altaxo.Graph
{
  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYFunctionPlotItem.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYFunctionPlotItem : G2DPlotItem, System.Runtime.Serialization.IDeserializationCallback
  {
    protected XYFunctionPlotData _plotData;
    

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
        info.AddValue("Data",s._plotData);  
        info.AddValue("Style",s._plotStyles);  
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

        s._plotData = (XYFunctionPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        s._plotStyles = (XYPlotStyleCollection)info.GetValue("Style", typeof(XYPlotStyleCollection));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotItem),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
        info.AddValue("Data",s._plotData);  
        info.AddValue("Style",s._plotStyles); 
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYFunctionPlotData pa  = (XYFunctionPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        XYLineScatterPlotStyle lsps = (XYLineScatterPlotStyle)info.GetValue("Style", typeof(XYLineScatterPlotStyle));

        // TODO this must be implemented again
        throw new NotImplementedException("This must be implemented here");
        XYPlotStyleCollection ps = new XYPlotStyleCollection();
        //G2DPlotStyleCollection ps = new G2DPlotStyleCollection(new I2DPlotStyle[] { lsps.XYLineStyle, lsps.XYScatterStyle });
        
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
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
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

      if(null!=_plotData)
      {
        _plotData.Changed += new EventHandler(OnDataChangedEventHandler);
      }

      if(null!=_plotStyles)
      {
        ((Main.IChangedEventSource)_plotStyles).Changed += new EventHandler(OnStyleChangedEventHandler);
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
      CopyFrom((PlotItem)from);
    }

    public void CopyFrom(XYFunctionPlotItem from)
    {
      CopyFrom((PlotItem)from);
    }

    protected override void CopyFrom(PlotItem fromb)
    {
      base.CopyFrom(fromb);
      XYFunctionPlotItem from = fromb as XYFunctionPlotItem;
      if (from != null)
      {
        this.Data = from.Data;
      }
    }

    public override object Clone()
    {
      return new XYFunctionPlotItem(this);
    }

    public override object DataObject
    {
      get { return _plotData; }
    }

    public XYFunctionPlotData Data
    {
      get { return _plotData; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else
        {
          if(!object.ReferenceEquals(_plotData,value))
          {
            if(null!=_plotData)
            {
              _plotData.Changed -= new EventHandler(OnDataChangedEventHandler);
            }

            _plotData = (XYFunctionPlotData)value;
          
            if(null!=_plotData)
            {
              _plotData.Changed += new EventHandler(OnDataChangedEventHandler);
            }

            OnDataChanged();
          }
        }
      }
    }

   
    


    public override string GetName(int level)
    {
      return _plotData.ToString();
    }
    public override string GetName(string style)
    {
      return GetName(0);
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
    public override void PreparePainting(IPlotArea layer)
    {
      // nothing really to do here
    }
   
  }
}
