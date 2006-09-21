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


namespace Altaxo.Graph
{
  using Scales;

  /// <summary>
  /// Used for constructor of <see cref="XYLineScatterPlotStyle" /> to choose between Line, Scatter and both.
  /// </summary>
  [Flags]
  public enum LineScatterPlotStyleKind
  {
    /// <summary>
    /// Neither line nor scatter used.
    /// </summary>
    Empty=0,
    /// <summary>Line only. No symbol plotted, no line-symbol-gap.</summary>
    Line=1,
    /// <summary>Scatter only. No line is plotted, no line-symbol gap.</summary>
    Scatter=2,
    /// <summary>Both line and symbol are plotted, line symbol gap is on by default.</summary>
    LineAndScatter=3
  }

#if true // must be kept for old deserialization
  [SerializationSurrogate(0,typeof(XYLineScatterPlotStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYLineScatterPlotStyle : 
   
    System.Runtime.Serialization.IDeserializationCallback
    
  {

    protected XYPlotLineStyle     m_LineStyle;
    protected XYPlotScatterStyle  m_ScatterStyle;
    protected bool          m_LineSymbolGap;

    /// <summary>The label style (is null if there is no label).</summary>
    protected XYPlotLabelStyle    m_LabelStyle;



    #region Serialization
    /// <summary>Used to serialize the XYLineScatterPlotStyle Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYLineScatterPlotStyle Version 0.
      /// </summary>
      /// <param name="obj">The XYLineScatterPlotStyle to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
        info.AddValue("XYPlotLineStyle",s.m_LineStyle);  
        info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
        info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
      }
      /// <summary>
      /// Deserializes the XYLineScatterPlotStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty axis object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYLineScatterPlotStyle.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;

        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.m_LineStyle = (XYPlotLineStyle)info.GetValue("XYPlotLineStyle",typeof(XYPlotLineStyle));
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.m_ScatterStyle = (XYPlotScatterStyle)info.GetValue("XYPlotScatterStyle",typeof(XYPlotScatterStyle));
        s.m_LineSymbolGap = info.GetBoolean("LineSymbolGap");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYLineScatterPlotStyle),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
        info.AddValue("XYPlotLineStyle",s.m_LineStyle);  
        info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
        info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYLineScatterPlotStyle s = null!=o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle();
        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.XYPlotLineStyle = (XYPlotLineStyle)info.GetValue("XYPlotLineStyle",typeof(XYPlotLineStyle));
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.ScatterStyle = (XYPlotScatterStyle)info.GetValue("XYPlotScatterStyle",typeof(XYPlotScatterStyle));
        s.LineSymbolGap = info.GetBoolean("LineSymbolGap");

        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYLineScatterPlotStyle),1)]
      public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
        info.AddValue("XYPlotLineStyle",s.m_LineStyle);  
        info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
        info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
        info.AddValue("LabelStyle",s.m_LabelStyle);      // new in this version
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYLineScatterPlotStyle s = null!=o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle();
        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.XYPlotLineStyle = (XYPlotLineStyle)info.GetValue("XYPlotLineStyle",typeof(XYPlotLineStyle));
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.ScatterStyle = (XYPlotScatterStyle)info.GetValue("XYPlotScatterStyle",typeof(XYPlotScatterStyle));
        s.LineSymbolGap = info.GetBoolean("LineSymbolGap");
        s.XYPlotLabelStyle  = (XYPlotLabelStyle)info.GetValue("XYPlotLabelStyle",typeof(XYPlotLabelStyle)); // new in this version

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYLineScatterPlotStyle),2)]
      public class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
        info.AddValue("XYPlotLineStyle",s.m_LineStyle);  
        info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
        info.AddValue("LineSymbolGap",s.m_LineSymbolGap);

        int nCount = null==s.m_LabelStyle ? 0 : 1;
        info.CreateArray("OptionalStyles",nCount);
        if(null!=s.m_LabelStyle)  info.AddValue("LabelStyle",s.m_LabelStyle); // new in this version
        info.CommitArray();

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYLineScatterPlotStyle s = null!=o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle();
        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.XYPlotLineStyle = (XYPlotLineStyle)info.GetValue("XYPlotLineStyle",typeof(XYPlotLineStyle));
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.ScatterStyle = (XYPlotScatterStyle)info.GetValue("XYPlotScatterStyle",typeof(XYPlotScatterStyle));
        s.LineSymbolGap = info.GetBoolean("LineSymbolGap");
      
        int nCount = info.OpenArray(); // OptionalStyles
        
        if(nCount==1)
          s.XYPlotLabelStyle  = (XYPlotLabelStyle)info.GetValue("LabelStyle",typeof(XYPlotLabelStyle)); // new in this version

        info.CloseArray(nCount); // OptionalStyles

        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the event chain here
      if(null!=m_LineStyle)
        m_LineStyle.Changed += new EventHandler(this.OnLineStyleChanged);
      if(null!=m_ScatterStyle)
        m_ScatterStyle.Changed += new EventHandler(this.OnScatterStyleChanged);
    }
    #endregion



    public XYLineScatterPlotStyle(XYLineScatterPlotStyle from)
    {
      
      this.XYPlotLineStyle        = from.m_LineStyle;
      this.ScatterStyle     = from.m_ScatterStyle;
      // this.m_PlotAssociation = null; // do not clone the plotassociation!
      this.LineSymbolGap    = from.m_LineSymbolGap;
    }

    public XYLineScatterPlotStyle()
      : this(LineScatterPlotStyleKind.LineAndScatter)
    {
    }

    public XYLineScatterPlotStyle(LineScatterPlotStyleKind kind)
    {
      if(0!=(kind&LineScatterPlotStyleKind.Line))
        this.XYPlotLineStyle = new XYPlotLineStyle();
      
      if(0!=(kind&LineScatterPlotStyleKind.Scatter))
        this.ScatterStyle = new XYPlotScatterStyle();

      this.LineSymbolGap = kind==LineScatterPlotStyleKind.LineAndScatter;
    }

    public XYLineScatterPlotStyle(XYColumnPlotData pa)
    {
      this.XYPlotLineStyle = new XYPlotLineStyle();
      this.ScatterStyle = new XYPlotScatterStyle();
      // this.m_PlotAssociation = pa;
      this.LineSymbolGap = true;
    }




  


    

    public System.Drawing.Color Color
    {
      get
      {
        if(m_LineStyle!=null)
          return this.m_LineStyle.PenHolder.Color;
        if(m_ScatterStyle!=null)
          return this.m_ScatterStyle.Color;
        else
          return Color.Black;
      }
      set
      {
        if(m_LineStyle!=null)
          this.m_LineStyle.PenHolder.Color = value;
        if(m_ScatterStyle!=null)
          this.m_ScatterStyle.Color = value;
      }
    }

    public  XYPlotLineStyle XYPlotLineStyle
    {
      get { return m_LineStyle; }
      set 
      {
        if(null!=m_LineStyle)
          m_LineStyle.Changed -= new EventHandler(OnLineStyleChanged);
  
        m_LineStyle = null==value ? null : (XYPlotLineStyle)value.Clone();

        if(null!=m_LineStyle)
          m_LineStyle.Changed += new EventHandler(OnLineStyleChanged);
        
        OnChanged(); // Fire changed event
      }
    }

    public XYPlotScatterStyles.ShapeAndStyle XYPlotScatterStyle
    {
      get { return new XYPlotScatterStyles.ShapeAndStyle(m_ScatterStyle.Shape, m_ScatterStyle.Style); }
      set 
      {
        if(null!=m_ScatterStyle)
          m_ScatterStyle.Changed -= new EventHandler(OnScatterStyleChanged);

        m_ScatterStyle = new XYPlotScatterStyle();
        m_ScatterStyle.Shape = value.Shape;
        m_ScatterStyle.Style = value.Style;

        if(null!=m_ScatterStyle)
          m_ScatterStyle.Changed += new EventHandler(OnScatterStyleChanged);
        
        OnChanged(); // Fire Changed event
      }
    }

    public XYPlotScatterStyle ScatterStyle
    {
      get { return m_ScatterStyle; }
      set
      {
        if (null != m_ScatterStyle)
          m_ScatterStyle.Changed -= new EventHandler(OnScatterStyleChanged);

        m_ScatterStyle = null==value ? null : (XYPlotScatterStyle)value.Clone();;

        if (null != m_ScatterStyle)
          m_ScatterStyle.Changed += new EventHandler(OnScatterStyleChanged);

        OnChanged(); // Fire Changed event
      }
    }

    public  XYPlotLabelStyle XYPlotLabelStyle
    {
      get { return m_LabelStyle; }
      set 
      {
        XYPlotLabelStyle oldValue = m_LabelStyle;
        m_LabelStyle = value==null ? null : (XYPlotLabelStyle)value.Clone();

        if(null!=oldValue)
          oldValue.Changed -= new EventHandler(EhLabelStyleChanged);

        if(null!=m_LabelStyle)
          m_LabelStyle.Changed += new EventHandler(EhLabelStyleChanged);
        
        OnChanged(); // Fire Changed event
      }
    }

    public float SymbolSize 
    {
      get 
      {
        return null==m_ScatterStyle ? 0 : m_ScatterStyle.SymbolSize;
      }
      set 
      {
        bool bChanged = false;

        if(null==m_ScatterStyle && value!=0)
        {
          m_ScatterStyle = new XYPlotScatterStyle();
          bChanged = true;
        }

        if(null!=m_ScatterStyle)
        {
          m_ScatterStyle.SymbolSize = value;
          bChanged = true;
        }

        if(bChanged)
          OnChanged();
      }
    }

    
    public bool LineSymbolGap 
    {
      get { return m_LineSymbolGap; }
      set
      { 
        m_LineSymbolGap = value; 
        OnChanged();
      }
    }

  

  
    
  
    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new EventArgs());
    }

    protected virtual void OnLineStyleChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void OnScatterStyleChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void EhLabelStyleChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    #endregion

    #region IChildChangedEventSink Members

    public void EhChildChanged(object child, EventArgs e)
    {
      if(null!=Changed)
        Changed(this,e);

    }

    #endregion

    #region I2DPlotStyle Members

    public bool IsColorProvider
    {
      get
      {
        return true;
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
        return this.m_LineStyle;
      }
    }



    public bool IsXYScatterStyleSupported
    {
      get
      {
        return true;
      }
    }

    public XYPlotScatterStyle XYScatterStyle
    {
      get
      {
        return this.m_ScatterStyle;
      }
    }

   

    #endregion
  } // end of class XYLineScatterPlotStyle

#endif
}
