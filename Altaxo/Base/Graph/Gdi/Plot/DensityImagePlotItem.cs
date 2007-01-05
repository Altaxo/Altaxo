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
using Altaxo.Graph.Scales.Boundaries;


namespace Altaxo.Graph.Gdi.Plot
{
  using Groups;
  using Styles;
  using Data;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;

  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  [SerializationSurrogate(0,typeof(DensityImagePlotItem.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class DensityImagePlotItem : PlotItem, System.Runtime.Serialization.IDeserializationCallback, IXBoundsHolder, IYBoundsHolder
  {
    protected XYZMeshedColumnPlotData m_PlotAssociation;
    protected DensityImagePlotStyle       m_PlotStyle;

    #region Serialization
    /// <summary>Used to serialize the DensityImagePlotItem Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes DensityImagePlotItem Version 0.
      /// </summary>
      /// <param name="obj">The DensityImagePlotItem to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        DensityImagePlotItem s = (DensityImagePlotItem)obj;
        info.AddValue("Data",s.m_PlotAssociation);  
        info.AddValue("Style",s.m_PlotStyle);  
      }
      /// <summary>
      /// Deserializes the DensityImagePlotItem Version 0.
      /// </summary>
      /// <param name="obj">The empty DensityImagePlotItem object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized DensityImagePlotItem.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        DensityImagePlotItem s = (DensityImagePlotItem)obj;

        s.m_PlotAssociation = (XYZMeshedColumnPlotData)info.GetValue("Data",typeof(XYZMeshedColumnPlotData));
        s.m_PlotStyle = (DensityImagePlotStyle)info.GetValue("Style",typeof(DensityImagePlotStyle));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.DensityImagePlotItem", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotItem),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DensityImagePlotItem s = (DensityImagePlotItem)obj;
        info.AddValue("Data",s.m_PlotAssociation);  
        info.AddValue("Style",s.m_PlotStyle);  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        
        XYZMeshedColumnPlotData pa = (XYZMeshedColumnPlotData)info.GetValue("Data",o);
        DensityImagePlotStyle ps = (DensityImagePlotStyle)info.GetValue("Style",o);

        if(o==null)
        {
          return new DensityImagePlotItem(pa,ps);
        }
        else
        {
          DensityImagePlotItem s = (DensityImagePlotItem)o;
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

      if(null!=m_PlotStyle)
      {
        m_PlotStyle.Changed += new EventHandler(OnStyleChangedEventHandler);
      }
    }
    #endregion



    public DensityImagePlotItem(XYZMeshedColumnPlotData pa, DensityImagePlotStyle ps)
    {
      this.Data = pa;
      this.Style = ps;
    }

    public DensityImagePlotItem(DensityImagePlotItem from)
    {
      CopyFrom(from);
    }

    protected override void CopyFrom(PlotItem fromb)
    {
      base.CopyFrom(fromb);

      DensityImagePlotItem from = fromb as DensityImagePlotItem;
      if (null != from)
      {
        this.Data = from.Data;   // also wires the event
        this.Style = from.Style; // also wires the event
      }
    }

    public override object Clone()
    {
      return new DensityImagePlotItem(this);
    }


    public object Data
    {
      get { return m_PlotAssociation; }
      set
      {
        if(null==value)
          throw new System.ArgumentNullException();
        else if(!(value is XYZMeshedColumnPlotData))
          throw new System.ArgumentException("The provided data object is not of the type " + m_PlotAssociation.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
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

            m_PlotAssociation = (XYZMeshedColumnPlotData)value;
          
            if(null!=m_PlotAssociation )
            {
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
      set { this.Style = (DensityImagePlotStyle)value; }
    }
    public DensityImagePlotStyle Style
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
              m_PlotStyle.Changed -= new EventHandler(OnStyleChangedEventHandler);
            }
          
            m_PlotStyle = (DensityImagePlotStyle)value;

            // create event wire to new Plotstyle
            if(null!=m_PlotStyle)
            {
              m_PlotStyle.Changed += new EventHandler(OnStyleChangedEventHandler);
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
      if(null!=this.m_PlotStyle)
      {
        m_PlotStyle.Paint(g,layer,m_PlotAssociation);
      }
    }

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public override void PrepareScales(IPlotArea layer)
    {
      if(null!=this.m_PlotAssociation)
        m_PlotAssociation.CalculateCachedData();
    }


    /// <summary>
    /// Intended to used by derived classes, fires the DataChanged event and the Changed event
    /// </summary>
    public override void OnDataChanged()
    {
      // first inform our AbstractXYPlotStyle of the change, so it can invalidate its cached data
      if(null!=this.m_PlotStyle)
        m_PlotStyle.EhDataChanged(this);

      base.OnDataChanged();
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
      if(val is NumericalBoundaries)
        this.m_PlotAssociation.SetXBoundsFromTemplate(val as NumericalBoundaries);
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if(pb is NumericalBoundaries)
        this.m_PlotAssociation.MergeXBoundsInto(pb as NumericalBoundaries);
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
      if(val is NumericalBoundaries)
        this.m_PlotAssociation.SetYBoundsFromTemplate(val as NumericalBoundaries);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if(pb is NumericalBoundaries)
        this.m_PlotAssociation.MergeYBoundsInto(pb as NumericalBoundaries);
    }

    #endregion

    public override void CollectStyles(PlotGroupStyleCollection styles)
    {
      
    }

    public override void PrepareStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer)
    {
      
    }

    public override void ApplyStyles(PlotGroupStyleCollection externalGroups)
    {
      
    }

    /// <summary>
    /// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
    /// </summary>
    /// <param name="template">The template item to copy the plot styles from.</param>
    /// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
    public override void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
    {
      if (!(template is DensityImagePlotItem))
        return;
      DensityImagePlotItem from = (DensityImagePlotItem)template;
//      m_PlotStyle.CopyFrom(from.m_PlotStyle);
    }

  }
}
