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


namespace Altaxo.Graph
{
  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYColumnPlotItem.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYColumnPlotItem : PlotItem, System.Runtime.Serialization.IDeserializationCallback
  {
    protected XYColumnPlotData m_PlotAssociation;
    protected AbstractXYPlotStyle       m_PlotStyle;

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
        s.Style = (AbstractXYPlotStyle)info.GetValue("Style",typeof(AbstractXYPlotStyle));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotItem),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYColumnPlotItem s = (XYColumnPlotItem)obj;
        info.AddValue("Data",s.m_PlotAssociation);  
        info.AddValue("Style",s.m_PlotStyle);  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYColumnPlotData pa = (XYColumnPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
        AbstractXYPlotStyle ps  = (AbstractXYPlotStyle)info.GetValue("Style",typeof(AbstractXYPlotStyle));

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
    }

    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // Restore the event chain

      if(null!=m_PlotAssociation)
      {
        m_PlotAssociation.Changed += new EventHandler(OnDataChangedEventHandler);
      }

      if(null!=m_PlotStyle && m_PlotStyle is Main.IChangedEventSource)
      {
        ((Main.IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
      }
    }
    #endregion



    public XYColumnPlotItem(XYColumnPlotData pa, AbstractXYPlotStyle ps)
    {
      this.Data = pa;
      this.Style = ps;
    }

    public XYColumnPlotItem(XYColumnPlotItem from)
    {
      this.Data = from.Data;   // also wires the event
      this.Style = from.Style; // also wires the event
    }

    public override object Clone()
    {
      return new XYColumnPlotItem(this);
    }

    public XYColumnPlotData XYColumnPlotData
    {
      get { return m_PlotAssociation; }
    }

    public override object Data
    {
      get { return m_PlotAssociation; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else if(!(value is XYColumnPlotData))
          throw new System.ArgumentException("The provided data object is not of the type " + m_PlotAssociation.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
        else
        {
          if(!object.ReferenceEquals(m_PlotAssociation,value))
          {
            if(null!=m_PlotAssociation)
            {
              m_PlotAssociation.Changed -= new EventHandler(OnDataChangedEventHandler);
            }

            m_PlotAssociation = (XYColumnPlotData)value;
          
            if(null!=m_PlotAssociation )
            {
              m_PlotAssociation.ParentObject = this;
              m_PlotAssociation.Changed += new EventHandler(OnDataChangedEventHandler);
            }

            OnDataChanged();
          }
        }
      }
    }
    public override object Style
    {
      get { return m_PlotStyle; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else if(!(value is AbstractXYPlotStyle))
          throw new System.ArgumentException("The provided data object is not of the type " + m_PlotAssociation.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
        else
        {
          if(!object.ReferenceEquals(m_PlotStyle,value))
          {
            // delete event wiring to old AbstractXYPlotStyle
            if(null!=m_PlotStyle && m_PlotStyle is Main.IChangedEventSource)
            {
              ((Main.IChangedEventSource)m_PlotStyle).Changed -= new EventHandler(OnStyleChangedEventHandler);
            }
          
            m_PlotStyle = (AbstractXYPlotStyle)value;

            // create event wire to new Plotstyle
            if(null!=m_PlotStyle && m_PlotStyle is Main.IChangedEventSource)
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
      return m_PlotAssociation.ToString();
    }

    public override string ToString()
    {
      return GetName(int.MaxValue);
    }

    public override void Paint(Graphics g, Graph.XYPlotLayer layer)
    {
      if(null!=this.m_PlotStyle)
      {
        m_PlotStyle.Paint(g,layer,m_PlotAssociation);
      }
    }

    /// <summary>
    /// Test wether the mouse hits a plot item. The default implementation here returns null.
    /// If you want to have a reaction on mouse click on a curve, implement this function.
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>Null if no hit, or a <see>IHitTestObject</see> if there was a hit.</returns>
    public override IHitTestObject HitTest(XYPlotLayer layer, PointF hitpoint)
    {
      if(null!=this.m_PlotStyle)
        return m_PlotStyle.HitTest(layer,m_PlotAssociation,hitpoint);
      else
        return null;
    }

  }
}
