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
using Altaxo.Graph.Scales;
using Altaxo.Data;
using Altaxo.Graph.Gdi.Background;


namespace Altaxo.Graph.Gdi.Axis
{
  using Gdi.LabelFormatting;

  /// <summary>
  /// Summary description for AbstractLabelFormatting.
  /// </summary>
  public class AxisLabelStyle : AxisLabelStyleBase,
    ICloneable,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink
  {
    protected Font _font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);
    
    protected StringAlignment _horizontalAlignment;
    protected StringAlignment _verticalAlignment;

    protected StringFormat _stringFormat;
    protected BrushX _brush = new BrushX(Color.Black);
    
    /// <summary>The x offset in EM units.</summary>
    protected double _xOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double _yOffset;

    /// <summary>The rotation of the label.</summary>
    protected double _rotation;

    /// <summary>The style for the background.</summary>
    protected Gdi.Background.IBackgroundStyle _backgroundStyle;

    protected bool _automaticRotationShift=true;

    ILabelFormatting _labelFormatting = new Gdi.LabelFormatting.NumericLabelFormattingAuto();

    #region Serialization
    /// <summary>Used to serialize the XYAxisLabelStyle Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYAxisLabelStyle Version 0.
      /// </summary>
      /// <param name="obj">The XYAxisLabelStyle to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        AxisLabelStyle s = (AxisLabelStyle)obj;
        info.AddValue("Font",s._font);  
      }
      /// <summary>
      /// Deserializes the XYAxisLabelStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty XYAxisLabelStyle object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYAxisLabelStyle.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        AxisLabelStyle s = (AxisLabelStyle)obj;

        s._font = (Font)info.GetValue("Font",typeof(Font));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisLabelStyle", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported - probably a programming error");
        /*
        XYAxisLabelStyle s = (XYAxisLabelStyle)obj;
        info.AddValue("Edge",s._edge);  
        info.AddValue("Font",s._font);  
        */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        AxisLabelStyle s = null!=o ? (AxisLabelStyle)o : new AxisLabelStyle();

        Edge edge = (Edge)info.GetValue("Edge",s);
        s._font = (Font)info.GetValue("Font",s);
        s.SetStringFormat();
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisLabelStyle", 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported - probably a programming error");
        /*
        XYAxisLabelStyle s = (XYAxisLabelStyle)obj;
        info.AddValue("Edge",s._edge);  
        info.AddValue("Font",s._font);  
        info.AddValue("Brush",s._brush);
        info.AddValue("Background",s._backgroundStyle);

        info.AddValue("AutoAlignment",s._automaticRotationShift);
        info.AddEnum("HorzAlignment",s._horizontalAlignment);
        info.AddEnum("VertAlignment",s._verticalAlignment);

        info.AddValue("Rotation",s._rotation);
        info.AddValue("XOffset",s._xOffset);
        info.AddValue("YOffset",s._yOffset);

        info.AddValue("LabelFormat",s._labelFormatting);
        */

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        AxisLabelStyle s = null!=o ? (AxisLabelStyle)o : new AxisLabelStyle();

        Edge edge = (Edge)info.GetValue("Edge",s);
        s._font = (Font)info.GetValue("Font",s);
        s._brush = (BrushX)info.GetValue("Brush",s);
        s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background");
        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment",typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment",typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s._labelFormatting = (ILabelFormatting)info.GetValue("LabelFormat",s);




        // Modification of StringFormat is necessary to avoid 
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;


        
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisLabelStyle", 2)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle), 3)]
    class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {

        AxisLabelStyle s = (AxisLabelStyle)obj;
        info.AddValue("Font",s._font);  
        info.AddValue("Brush",s._brush);
        info.AddValue("Background",s._backgroundStyle);

        info.AddValue("AutoAlignment",s._automaticRotationShift);
        info.AddEnum("HorzAlignment",s._horizontalAlignment);
        info.AddEnum("VertAlignment",s._verticalAlignment);

        info.AddValue("Rotation",s._rotation);
        info.AddValue("XOffset",s._xOffset);
        info.AddValue("YOffset",s._yOffset);

        info.AddValue("LabelFormat",s._labelFormatting);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        AxisLabelStyle s = null != o ? (AxisLabelStyle)o : new AxisLabelStyle();

        s._font = (Font)info.GetValue("Font", s);
        s._brush = (BrushX)info.GetValue("Brush", s);
        s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background");
        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s._labelFormatting = (ILabelFormatting)info.GetValue("LabelFormat", s);




        // Modification of StringFormat is necessary to avoid 
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;



        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
    }
    #endregion



    public AxisLabelStyle()
    {
      SetStringFormat();
    }

    public AxisLabelStyle(AxisLabelStyle from)
    {
      _font = null==from._font ? null : (Font)from._font.Clone();
      _stringFormat = (StringFormat)from._stringFormat.Clone(); 
      _horizontalAlignment = from._horizontalAlignment;
      _verticalAlignment = from._verticalAlignment;
      _brush = (BrushX)from._brush.Clone();
      _automaticRotationShift = from._automaticRotationShift;
      _xOffset = from._xOffset;
      _yOffset = from._yOffset;
      _rotation = from._rotation;
      _backgroundStyle = null==from._backgroundStyle ? null : (IBackgroundStyle)from._backgroundStyle.Clone();
      _labelFormatting = (ILabelFormatting)from._labelFormatting.Clone();
    }

    public override object Clone()
    {
      return new AxisLabelStyle(this);
    }

    private void SetStringFormat()
    {
      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      _stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
      _stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      /*
      // set the alignment and line alignment of the strings
      switch(this._edge.TypeOfEdge)
      {
        case EdgeType.Bottom:
          _verticalAlignment = StringAlignment.Near;
          _horizontalAlignment = StringAlignment.Center;
          break;
        case EdgeType.Top:
          _verticalAlignment = StringAlignment.Far;
          _horizontalAlignment = StringAlignment.Center;
          break;
        case EdgeType.Left:
          _verticalAlignment = StringAlignment.Center;
          _horizontalAlignment = StringAlignment.Far;
          break;
        case EdgeType.Right:
          _verticalAlignment = StringAlignment.Center;
          _horizontalAlignment = StringAlignment.Near;
          break;
      }
       */
    }
    #region Properties

    /// <summary>The font of the label.</summary>
    public Font Font
    {
      get { return _font; }
      set
      {
        _font = value;
        OnChanged();
      }
    }

   
    /// <summary>The font size of the label.</summary>
    public override float FontSize
    {
      get { return _font.Size; }
      set
      {
        float oldValue = FontSize;
        float newValue = Math.Max(0,value);

        if(newValue != oldValue)
        {
          Font oldFont = _font;
          _font = new Font(oldFont.FontFamily.Name,newValue,oldFont.Style,GraphicsUnit.World);
          oldFont.Dispose();

          OnChanged(); // Fire Changed event
        }
      }
    }
    /// <summary>The brush color. If you set this, the font brush will be set to a solid brush.</summary>
    public System.Drawing.Color Color
    {
      get { return this._brush.Color;; }
      set 
      {
        Color oldColor = this.Color;
        if(value!=oldColor)
        {
          this._brush.SetSolidBrush( value );
          OnChanged(); // Fire Changed event
        }
      }
    }

    /// <summary>The brush. During setting, the brush is cloned.</summary>
    public BrushX Brush
    {
      get 
      {
        return this._brush; ; 
      }
      set
      {
       
        this._brush = (BrushX)value.Clone();
        OnChanged(); // Fire Changed event
      }
    }

    /// <summary>The background style.</summary>
    public Gdi.Background.IBackgroundStyle BackgroundStyle
    {
      get
      {
        return _backgroundStyle;
      }
      set 
      {
        IBackgroundStyle oldValue = this._backgroundStyle;
        if(!object.ReferenceEquals(value,oldValue))
        {
          this._backgroundStyle = value;
          OnChanged(); // Fire Changed event
        }
      }
    }

    public ILabelFormatting LabelFormat
    {
      get
      {
        return this._labelFormatting;
      }
      set
      {
        if(null==value)
          throw new ArgumentNullException("value");
        
        ILabelFormatting oldValue = this._labelFormatting;
        if(!object.ReferenceEquals(value,oldValue))
        {
          _labelFormatting = value;
          OnChanged();
        }
      }
    }

    /// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double XOffset
    {
      get { return this._xOffset; }
      set
      {
        double oldValue = this._xOffset;
        this._xOffset = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double YOffset
    {
      get { return this._yOffset; }
      set
      {
        double oldValue = this._yOffset;
        this._yOffset = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>The angle of the label.</summary>
    public double Rotation
    {
      get { return this._rotation; }
      set
      {
        double oldValue = this._rotation;
        this._rotation = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

  

    public bool AutomaticAlignment
    {
      get
      {
        return this._automaticRotationShift;
      }
      set
      {
        bool oldValue = this.AutomaticAlignment;
        this._automaticRotationShift = value;
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
        return this._horizontalAlignment; 
      }
      set
      {
        System.Drawing.StringAlignment oldValue = this.HorizontalAlignment;
        this._horizontalAlignment = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>Vertical aligment of the label.</summary>
    public System.Drawing.StringAlignment VerticalAlignment
    {
      get { return this._verticalAlignment; }
      set
      {
        System.Drawing.StringAlignment oldValue = this.VerticalAlignment;
        this._verticalAlignment = value;
        if(value!=oldValue)
        {
          OnChanged();
        }
      }
    }

    #endregion

    CSLineID _cachedStyleID;
    public CSLineID AxisStyleID
    {
      get
      {
        return _cachedStyleID;
      }
    }
    public override IHitTestObject HitTest(XYPlotLayer layer, PointF pt)
    {
      GraphicsPath gp = GetSelectionPath(layer);
      if(gp.IsVisible(pt))
        return new HitTestObject(gp,this);
      else
        return null;
    }


    public void AdjustRectangle(ref RectangleF r, StringAlignment horz, StringAlignment vert)
    {
      switch(vert)
      {
        case StringAlignment.Near:
          break;
        case StringAlignment.Center:
          r.Y -= 0.5f*r.Height;
          break;
        case StringAlignment.Far:
          r.Y -= r.Height;
          break;
      }
      switch(horz)
      {
        case StringAlignment.Near:
          break;
        case StringAlignment.Center:
          r.X -= 0.5f*r.Width;
          break;
        case StringAlignment.Far:
          r.X -= r.Width;
          break;
      }
    }
    /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public virtual GraphicsPath GetSelectionPath(XYPlotLayer layer)
    {
      return (GraphicsPath)_enclosingPath.Clone();
    }

    private GraphicsPath _enclosingPath = new GraphicsPath(); // with Winding also overlapping rectangles are selected
    public override void Paint(Graphics g, XYPlotLayer layer, CSAxisInformation styleInfo, AxisLineStyle axisstyle, bool useMinorTicks)
    {
      _cachedStyleID = styleInfo.Identifier;
      CSLineID styleID = styleInfo.Identifier;
      Scale raxis = styleID.ParallelAxisNumber==0 ? layer.XAxis : layer.YAxis;

      _enclosingPath.Reset();
      _enclosingPath.FillMode = FillMode.Winding; // with Winding also overlapping rectangles are selected
      GraphicsPath helperPath = new GraphicsPath();
      Matrix math = new Matrix();

      Logical3D r0 = _cachedStyleID.Begin;
      Logical3D r1 = _cachedStyleID.End;

      SizeF layerSize = layer.Size;
      PointF outVector;
      Logical3D outer;
      float outerDistance = null==axisstyle? 0 : axisstyle.GetOuterDistance(styleInfo.PreferedLabelSide);
      float dist_x = outerDistance; // Distance from axis tick point to label
      float dist_y = outerDistance; // y distance from axis tick point to label

      // dist_x += this._font.SizeInPoints/3; // add some space to the horizontal direction in order to separate the chars a little from the ticks

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      double[] relpositions;
      AltaxoVariant[] ticks;
      if(useMinorTicks)
      {
        relpositions = raxis.GetMinorTicksNormal();
        ticks = raxis.GetMinorTicksAsVariant();
      }
      else
      {
        relpositions = raxis.GetMajorTicksNormal();
        ticks = raxis.GetMajorTicksAsVariant();
      }
      
      IMeasuredLabelItem[] labels = _labelFormatting.GetMeasuredItems(g,_font,_stringFormat,ticks);

      float emSize = _font.SizeInPoints;
      for(int i=0;i<ticks.Length;i++)
      {
        double r = relpositions[i];

        outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, styleInfo.PreferedLabelSide);
        PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
        PointF tickend = tickorg;
        tickend.X += outVector.X * outerDistance;
        tickend.Y += outVector.Y * outerDistance;
       

        SizeF msize = labels[i].Size;
        PointF morg = tickend;

        if (_automaticRotationShift)
        {
          double alpha = _rotation * Math.PI / 180 - Math.Atan2(outVector.Y, outVector.X);
          double shift = msize.Height * 0.5 * Math.Abs(Math.Sin(alpha)) + (msize.Width + _font.SizeInPoints / 2) * 0.5 * Math.Abs(Math.Cos(alpha));
          morg.X += (float)(outVector.X * shift);
          morg.Y += (float)(outVector.Y * shift);
        }
        else
        {
          morg.X += (float)(outVector.X * _font.SizeInPoints/3);
        }

       
        RectangleF mrect = new RectangleF(morg,msize);
        if(_automaticRotationShift)
          AdjustRectangle(ref mrect, StringAlignment.Center, StringAlignment.Center);
        else
          AdjustRectangle(ref mrect, _horizontalAlignment, _verticalAlignment);

        math.Reset();
        math.Translate((float)morg.X, (float)morg.Y);
        if (this._rotation != 0)
        {
          math.Rotate((float)-this._rotation);
        }
        math.Translate((float)(mrect.X - morg.X + emSize * _xOffset), (float)(mrect.Y - morg.Y + emSize * _yOffset));


        System.Drawing.Drawing2D.GraphicsState gs = g.Save();
        g.MultiplyTransform(math);

        if(this._backgroundStyle!=null)
          _backgroundStyle.Draw(g,new RectangleF(PointF.Empty,msize));

        _brush.Rectangle = new RectangleF(PointF.Empty, msize);      
        labels[i].Draw(g,_brush,new PointF(0,0));
        g.Restore(gs); // Restore the graphics state

        helperPath.Reset();
        helperPath.AddRectangle(new RectangleF(PointF.Empty, msize));
        helperPath.Transform(math);

        _enclosingPath.AddPath(helperPath,true);

       
      }
    

    }
    #region IChangedEventSource Members

  

    #endregion

    #region IChildChangedEventSink Members

  
    public void EhChildChanged(object child, EventArgs e)
    {
      OnChanged();
    }

    #endregion

  }
}

