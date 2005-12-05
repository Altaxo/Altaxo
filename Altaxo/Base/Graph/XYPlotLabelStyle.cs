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
using Altaxo.Graph.XYPlotScatterStyles;
using Altaxo.Serialization;

namespace Altaxo.Graph
{

  public class XYPlotLabelStyle :
    ICloneable,
    Main.IChangedEventSource,
    System.Runtime.Serialization.IDeserializationCallback, 
    Main.IChildChangedEventSink,
    I2DPlotStyle
  {
    /// <summary>The font of the label.</summary>
    protected System.Drawing.Font m_Font;

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    protected bool m_IndependentColor;

    /// <summary>The brush for the label.</summary>
    protected BrushHolder  m_Brush;
  
    /// <summary>The x offset in EM units.</summary>
    protected double m_XOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double m_YOffset;

    /// <summary>The rotation of the label.</summary>
    protected double m_Rotation;

    // <summary>If true, the label is painted on a background.</summary>
    //protected bool m_WhiteOut;

    /// <summary>The style for the background.</summary>
    protected BackgroundStyles.IBackgroundStyle  _backgroundStyle;

    // <summary>The brush for the background.</summary>
    //protected BrushHolder  m_BackgroundBrush;

    /// <summary>If true, the label is attached to one of the four edges of the layer.</summary>
    protected bool m_AttachToAxis;

    /// <summary>The axis where the label is attached to (if it is attached).</summary>
    protected Graph.EdgeType m_AttachedAxis;

    protected Altaxo.Data.ReadableColumnProxy m_LabelColumn;

    // cached values:
    protected System.Drawing.StringFormat m_CachedStringFormat;


    #region Serialization
  

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLabelStyle),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SSerialize(obj,info);
      }
      public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
        info.AddValue("Font",s.m_Font);  
        info.AddValue("IndependentColor",s.m_IndependentColor);
        info.AddValue("Brush",s.m_Brush);  
        info.AddValue("XOffset",s.m_XOffset);
        info.AddValue("YOffset",s.m_YOffset);
        info.AddValue("Rotation",s.m_Rotation);
        info.AddEnum("HorizontalAlignment",s.HorizontalAlignment);
        info.AddEnum("VerticalAlignment",s.VerticalAlignment);
        info.AddValue("AttachToAxis",s.m_AttachToAxis);
        info.AddValue("AttachedAxis",s.m_AttachedAxis);
        //info.AddValue("WhiteOut",s.m_WhiteOut);
        //info.AddValue("BackgroundBrush",s.m_BackgroundBrush);  
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return SDeserialize(o,info,parent,true);
      }
      public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
      {
        
        XYPlotLabelStyle s = null!=o ? (XYPlotLabelStyle)o : new XYPlotLabelStyle();

        s.m_Font = (Font)info.GetValue("Font",s);  
        s.m_IndependentColor = info.GetBoolean("IndependentColor");
        s.m_Brush = (BrushHolder)info.GetValue("Brush",s);
        s.m_XOffset = info.GetDouble("XOffset");
        s.m_YOffset = info.GetDouble("YOffset");
        s.m_Rotation = info.GetDouble("Rotation");
        s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment",typeof(System.Drawing.StringAlignment));
        s.VerticalAlignment   = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment",typeof(System.Drawing.StringAlignment));
        s.m_AttachToAxis = info.GetBoolean("AttachToAxis");
        s.m_AttachedAxis = (EdgeType)info.GetValue("AttachedAxis",parent);
        bool whiteOut = info.GetBoolean("WhiteOut");
        BrushHolder backgroundBrush = (BrushHolder)info.GetValue("BackgroundBrush",s);

        if(whiteOut)
          s._backgroundStyle = new BackgroundStyles.BackgroundColorStyle(backgroundBrush.Color);
      
        if(nativeCall)
        {
          // restore the cached values
          s.SetCachedValues();
          s.CreateEventChain();
        }

        return s;
      }

      
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLabelStyle),1)]
      public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
        XmlSerializationSurrogate0.SSerialize(obj,info);
        info.AddValue("LabelColumn",s.m_LabelColumn);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotLabelStyle s = (XYPlotLabelStyle) XmlSerializationSurrogate0.SDeserialize(o,info,parent,false);

        s.m_LabelColumn = (Altaxo.Data.ReadableColumnProxy)info.GetValue("LabelColumn",parent);

        // restore the cached values
        s.SetCachedValues();
        s.CreateEventChain();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLabelStyle),2)]
      public class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SSerialize(obj,info);
      }
      public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
        info.AddValue("Font",s.m_Font);  
        info.AddValue("IndependentColor",s.m_IndependentColor);
        info.AddValue("Brush",s.m_Brush);  
        info.AddValue("XOffset",s.m_XOffset);
        info.AddValue("YOffset",s.m_YOffset);
        info.AddValue("Rotation",s.m_Rotation);
        info.AddEnum("HorizontalAlignment",s.HorizontalAlignment);
        info.AddEnum("VerticalAlignment",s.VerticalAlignment);
        info.AddValue("AttachToAxis",s.m_AttachToAxis);
        info.AddValue("AttachedAxis",s.m_AttachedAxis);
        info.AddValue("Background",s._backgroundStyle);  
        info.AddValue("LabelColumn",s.m_LabelColumn);

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return SDeserialize(o,info,parent,true);
      }
      public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
      {
        
        XYPlotLabelStyle s = null!=o ? (XYPlotLabelStyle)o : new XYPlotLabelStyle();

        s.m_Font = (Font)info.GetValue("Font",s);  
        s.m_IndependentColor = info.GetBoolean("IndependentColor");
        s.m_Brush = (BrushHolder)info.GetValue("Brush",s);
        s.m_XOffset = info.GetDouble("XOffset");
        s.m_YOffset = info.GetDouble("YOffset");
        s.m_Rotation = info.GetDouble("Rotation");
        s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment",typeof(System.Drawing.StringAlignment));
        s.VerticalAlignment   = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment",typeof(System.Drawing.StringAlignment));
        s.m_AttachToAxis = info.GetBoolean("AttachToAxis");
        s.m_AttachedAxis = (EdgeType)info.GetValue("AttachedAxis",parent);
        s._backgroundStyle = (BackgroundStyles.IBackgroundStyle)info.GetValue("Background",s);
        s.m_LabelColumn = (Altaxo.Data.ReadableColumnProxy)info.GetValue("LabelColumn",parent);


        if(nativeCall)
        {
          // restore the cached values
          s.SetCachedValues();
          s.CreateEventChain();
        }

        return s;
      }
    }


    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the cached values
      SetCachedValues();
      CreateEventChain();
    }
    #endregion

    /// <summary>
    /// For deserialization purposes.
    /// </summary>
    protected XYPlotLabelStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      this.m_CachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
      this.m_CachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
      this.m_CachedStringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
    }

    public XYPlotLabelStyle(XYPlotLabelStyle from)
    {
      this.m_Font       = (Font)from.m_Font.Clone();
      this.m_IndependentColor = from.m_IndependentColor;
      this.m_Brush      = (BrushHolder)from.m_Brush.Clone();
      this.m_XOffset    = from.m_XOffset;
      this.m_YOffset    = from.m_YOffset;
      this.m_Rotation   = from.m_Rotation;
      this._backgroundStyle      = null==from._backgroundStyle ? null : (BackgroundStyles.IBackgroundStyle)from._backgroundStyle.Clone();
      this.m_CachedStringFormat = (System.Drawing.StringFormat)from.m_CachedStringFormat.Clone();
      this.m_AttachToAxis        = from.m_AttachToAxis;
      this.m_AttachedAxis        = from.m_AttachedAxis;
      this.m_LabelColumn = (Data.ReadableColumnProxy)from.m_LabelColumn.Clone();

      CreateEventChain();
    }

    public XYPlotLabelStyle()
      : this((Altaxo.Data.IReadableColumn)null)
    {
    }

    public XYPlotLabelStyle(Altaxo.Data.IReadableColumn labelColumn)
    {
      this.m_Font = new Font(System.Drawing.FontFamily.GenericSansSerif,8,GraphicsUnit.World);
      this.m_IndependentColor = false;
      this.m_Brush = new BrushHolder(Color.Black);
      this.m_XOffset = 0;
      this.m_YOffset = 0;
      this.m_Rotation = 0;
      this._backgroundStyle = null;
      this.m_CachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
      this.m_CachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
      this.m_CachedStringFormat.LineAlignment   = System.Drawing.StringAlignment.Center;
      this.m_AttachToAxis = false;
      this.m_AttachedAxis = EdgeType.Bottom;
      this.m_LabelColumn = new Altaxo.Data.ReadableColumnProxy(labelColumn);

      CreateEventChain();
    }


    protected void CreateEventChain()
    {
      this.m_LabelColumn.Changed += new EventHandler(LabelColumnProxy_Changed);
    }

    void LabelColumnProxy_Changed(object sender, EventArgs e)
    {
      this.OnChanged();
    }

    public Altaxo.Data.IReadableColumn LabelColumn
    {
      get
      {
        return m_LabelColumn == null ? null : m_LabelColumn.Document;
      }
      set
      {
        m_LabelColumn.SetDocNode(value);
        OnChanged();
      }
    }

    /// <summary>The font of the label.</summary>
    public Font Font
    {
      get { return m_Font; }
      set
      {
        m_Font = value;
        OnChanged();
      }
    }

    /// <summary>The font size of the label.</summary>
    public float FontSize
    {
      get { return m_Font.Size; }
      set
      {
        float oldValue = FontSize;
        float newValue = Math.Max(0,value);

        if(newValue != oldValue)
        {
          Font oldFont = m_Font;
          m_Font = new Font(oldFont.FontFamily.Name,newValue,oldFont.Style,GraphicsUnit.World);
          oldFont.Dispose();

          OnChanged(); // Fire Changed event
        }
      }
    }

    /// <summary>
    /// Determines whether or not the color of the label is independent of the color of the parent plot style.
    /// </summary>
    public bool IndependentColor
    {
      get { return m_IndependentColor; }
      set
      {
        bool oldValue = m_IndependentColor;
        m_IndependentColor = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>The brush color.</summary>
    public System.Drawing.Color Color
    {
      get { return this.m_Brush.Color;; }
      set 
      {
        Color oldColor = this.Color;
        if(value!=oldColor)
        {
          this.m_Brush.SetSolidBrush( value );
          OnChanged(); // Fire Changed event
        }
      }
    }

    /// <summary>The background style.</summary>
    public BackgroundStyles.IBackgroundStyle BackgroundStyle
    {
      get
      {
        return _backgroundStyle;
      }
      set 
      {
        BackgroundStyles.IBackgroundStyle oldValue = this._backgroundStyle;
        if(!object.ReferenceEquals(value,oldValue))
        {
          this._backgroundStyle = value;
          OnChanged(); // Fire Changed event
        }
      }
    }


    /// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double XOffset
    {
      get { return this.m_XOffset; }
      set
      {
        double oldValue = this.m_XOffset;
        this.m_XOffset = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double YOffset
    {
      get { return this.m_YOffset; }
      set
      {
        double oldValue = this.m_YOffset;
        this.m_YOffset = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>The angle of the label.</summary>
    public double Rotation
    {
      get { return this.m_Rotation; }
      set
      {
        double oldValue = this.m_Rotation;
        this.m_Rotation = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

   

    /// <summary>Horizontal alignment of the label.</summary>
    public System.Drawing.StringAlignment HorizontalAlignment
    {
      get 
      {
        return this.m_CachedStringFormat.Alignment; 
      }
      set
      {
        System.Drawing.StringAlignment oldValue = this.HorizontalAlignment;
        this.m_CachedStringFormat.Alignment = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>Vertical aligment of the label.</summary>
    public System.Drawing.StringAlignment VerticalAlignment
    {
      get { return this.m_CachedStringFormat.LineAlignment; }
      set
      {
        System.Drawing.StringAlignment oldValue = this.VerticalAlignment;
        this.m_CachedStringFormat.LineAlignment = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>If true, the label is attached to one of the 4 axes.</summary>
    public bool AttachToAxis
    {
      get { return this.m_AttachToAxis; }
      set
      {
        bool oldValue = this.m_AttachToAxis;
        this.m_AttachToAxis = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }


    /// <summary>If axis the label is attached to if the value of <see cref="AttachToAxis" /> is true.</summary>
    public EdgeType AttachedAxis
    {
      get { return this.m_AttachedAxis; }
      set
      {
        EdgeType oldValue = this.m_AttachedAxis;
        this.m_AttachedAxis = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }


    protected void SetCachedValues()
    {
    }


    public void Paint(Graphics g, string label)
    {
      float fontSize = this.FontSize;
      float xpos = (float)(m_XOffset*fontSize);
      float ypos = (float)(-m_YOffset*fontSize);
      if(this._backgroundStyle!=null)
      {
        float x = xpos, y = ypos;
        SizeF stringsize = g.MeasureString(label,this.m_Font,new PointF(xpos,ypos),m_CachedStringFormat);
        switch(m_CachedStringFormat.Alignment)
        {
          case StringAlignment.Center:
            x -= stringsize.Width/2;
            break;
          case StringAlignment.Far:
            x -= stringsize.Width;
            break;
        }
        switch(m_CachedStringFormat.LineAlignment)
        {
          case StringAlignment.Center:
            y -= stringsize.Height/2;
            break;
          case StringAlignment.Far:
            y -= stringsize.Height;
            break;
        }
        this._backgroundStyle.Draw(g,new RectangleF(x,y,stringsize.Width,stringsize.Height));
      }
      g.DrawString(label,m_Font,m_Brush,xpos,ypos,m_CachedStringFormat);
    }

    public void Paint(Graphics g,
      IPlotArea layer,
      Graph.AbstractXYPlotStyle parentStyle,
      PlotRangeList rangeList,
      PointF[] ptArray,
      Altaxo.Data.IReadableColumn labelColumn)
    {
      // force color to be that of the parent style if not independent
      if(!this.m_IndependentColor && parentStyle!=null && this.m_Brush.Color!=parentStyle.Color)
        this.m_Brush.SetSolidBrush(parentStyle.Color);
        

      // save the graphics stat since we have to translate the origin
      System.Drawing.Drawing2D.GraphicsState gs = g.Save();

      double bottomPosition= 0; 
      double topPosition   = 0;
      double leftPosition  = 0;
      double rightPosition = 0;

      if(layer.IsAffine && layer.IsOrthogonal)
      {
        layer.LogicalToAreaConversion.Convert(0,0, out leftPosition, out bottomPosition);
        layer.LogicalToAreaConversion.Convert(1,1, out rightPosition, out topPosition);
      }

      double xpos=0, ypos=0;
      double xpre,ypre;
      double xdiff,ydiff;
      for(int r=0;r<rangeList.Count;r++)
      {
        int lower = rangeList[r].LowerBound;
        int upper = rangeList[r].UpperBound;
        int offset = rangeList[r].OffsetToOriginal;
        for(int j=lower;j<upper;j++)
        {
          string label = labelColumn[j+offset].ToString();
          if(label==null || label==string.Empty)
            continue;
          
          xpre = ptArray[j].X;
          ypre = ptArray[j].Y;
          
          if(this.m_AttachToAxis)
          {
            switch(this.m_AttachedAxis)
            {
              case Graph.EdgeType.Bottom:
                ypre = bottomPosition;
                break;
              case Graph.EdgeType.Left:
                xpre = leftPosition;
                break;
              case Graph.EdgeType.Right:
                xpre = rightPosition;
                break;
              case Graph.EdgeType.Top:
                ypre = topPosition;
                break;
            }
          }
          

          xdiff = xpre - xpos;
          ydiff = ypre - ypos;
          xpos =  xpre;
          ypos =  ypre;
          g.TranslateTransform((float)xdiff,(float)ydiff);
          if(this.m_Rotation!=0)
            g.RotateTransform((float)-this.m_Rotation);
          this.Paint(g,label);
          if(this.m_Rotation!=0)
            g.RotateTransform((float)this.m_Rotation);
        } // end for
      }

      g.Restore(gs); // Restore the graphics state
    }


    public object Clone()
    {
      return new XYPlotLabelStyle(this);
    }
  
    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new EventArgs());
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
      get { return true; }
    }

    public bool IsColorReceiver
    {
      get { return this.IndependentColor == false; }
    }

    public bool IsSymbolSizeProvider
    {
      get { return false; }
    }

    public bool IsSymbolSizeReceiver
    {
      get { return false; }
    }

    public float SymbolSize
    {
      get
      {
        return 0;
      }
      set
      {
        
      }
    }

    public void Paint(Graphics g,
      IPlotArea layer,
      PlotRangeList rangeList,
      PointF[] ptArray)
    {
      if(this.m_LabelColumn.Document!=null)
        this.Paint(g, layer, null, rangeList, ptArray, this.m_LabelColumn.Document);
    }

    #endregion
  }
}
