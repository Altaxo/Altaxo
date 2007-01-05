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
using Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles;
using Altaxo.Serialization;
using Altaxo.Graph.Gdi.Background;


namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Graph.Plot.Groups;
  using Plot.Groups;
  using Plot.Data;
  using Graph.Plot.Data;

  public class LabelPlotStyle :
    ICloneable,
    Main.IChangedEventSource,
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChildChangedEventSink,
    IG2DPlotStyle
  {
    /// <summary>The font of the label.</summary>
    protected System.Drawing.Font _font;

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>The brush for the label.</summary>
    protected BrushX _brush;

    /// <summary>The x offset in EM units.</summary>
    protected double _xOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double _yOffset;

    /// <summary>The rotation of the label.</summary>
    protected double _rotation;
   
    /// <summary>The style for the background.</summary>
    protected Gdi.Background.IBackgroundStyle _backgroundStyle;

    /// <summary>The axis where the label is attached to (if it is attached).</summary>
    protected CSPlaneID _attachedPlane;

    protected Altaxo.Data.ReadableColumnProxy _labelColumn;

    // cached values:
    [NonSerialized]
    protected System.Drawing.StringFormat _cachedStringFormat;

    [NonSerialized]
    protected object _parent;

    [field: NonSerialized]
    public event System.EventHandler Changed;

    #region Serialization


    private CSLineID GetDirection(EdgeType fillDir)
    {
      switch (fillDir)
      {
        case EdgeType.Bottom:
          return CSLineID.X0;
        case EdgeType.Top:
          return CSLineID.X1;
        case EdgeType.Left:
          return CSLineID.Y0;
        case EdgeType.Right:
          return CSLineID.Y1;
      }
      return null;
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLabelStyle", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public static CSPlaneID GetDirection(EdgeType fillDir)
      {
        switch (fillDir)
        {
          case EdgeType.Bottom:
            return CSPlaneID.Bottom;
          case EdgeType.Top:
            return CSPlaneID.Top;
          case EdgeType.Left:
            return CSPlaneID.Left;
          case EdgeType.Right:
            return CSPlaneID.Right;
        }
        return null;
      }

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SSerialize(obj, info);
      }
      public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");
        /*
        XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
        info.AddValue("Font", s.m_Font);
        info.AddValue("IndependentColor", s.m_IndependentColor);
        info.AddValue("Brush", s.m_Brush);
        info.AddValue("XOffset", s.m_XOffset);
        info.AddValue("YOffset", s.m_YOffset);
        info.AddValue("Rotation", s.m_Rotation);
        info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
        info.AddEnum("VerticalAlignment", s.VerticalAlignment);
        info.AddValue("AttachToAxis", s.m_AttachToAxis);
        info.AddValue("AttachedAxis", s.m_AttachedAxis);
        //info.AddValue("WhiteOut",s.m_WhiteOut);
        //info.AddValue("BackgroundBrush",s.m_BackgroundBrush);  
         */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return SDeserialize(o, info, parent, true);
      }
      public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
      {

        LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle();

        s._font = (Font)info.GetValue("Font", s);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._brush = (BrushX)info.GetValue("Brush", s);
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");
        s._rotation = info.GetDouble("Rotation");
        s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
        s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
        bool attachToAxis = info.GetBoolean("AttachToAxis");
        EdgeType attachedAxis = (EdgeType)info.GetValue("AttachedAxis", parent);
        bool whiteOut = info.GetBoolean("WhiteOut");
        BrushX backgroundBrush = (BrushX)info.GetValue("BackgroundBrush", s);

        if (attachToAxis)
          s._attachedPlane = GetDirection(attachedAxis);
        else
          s._attachedPlane = null;

        if (whiteOut)
          s._backgroundStyle = new FilledRectangle(backgroundBrush.Color);

        if (nativeCall)
        {
          // restore the cached values
          s.SetCachedValues();
          s.CreateEventChain();
        }

        return s;
      }


    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLabelStyle", 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");

        /*
        XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
        XmlSerializationSurrogate0.SSerialize(obj, info);
        info.AddValue("LabelColumn", s.m_LabelColumn);
        */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        LabelPlotStyle s = (LabelPlotStyle)XmlSerializationSurrogate0.SDeserialize(o, info, parent, false);

        s._labelColumn = (Altaxo.Data.ReadableColumnProxy)info.GetValue("LabelColumn", parent);

        // restore the cached values
        s.SetCachedValues();
        s.CreateEventChain();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLabelStyle", 2)]
    class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SSerialize(obj, info);
      }
      public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");
        /*
        XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
        info.AddValue("Font", s.m_Font);
        info.AddValue("IndependentColor", s.m_IndependentColor);
        info.AddValue("Brush", s.m_Brush);
        info.AddValue("XOffset", s.m_XOffset);
        info.AddValue("YOffset", s.m_YOffset);
        info.AddValue("Rotation", s.m_Rotation);
        info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
        info.AddEnum("VerticalAlignment", s.VerticalAlignment);
        info.AddValue("AttachToAxis", s.m_AttachToAxis);
        info.AddValue("AttachedAxis", s.m_AttachedAxis);
        info.AddValue("Background", s._backgroundStyle);
        info.AddValue("LabelColumn", s.m_LabelColumn);
        */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return SDeserialize(o, info, parent, true);
      }
      public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
      {

        LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle();

        s._font = (Font)info.GetValue("Font", s);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._brush = (BrushX)info.GetValue("Brush", s);
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");
        s._rotation = info.GetDouble("Rotation");
        s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
        s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
        bool attachToAxis = info.GetBoolean("AttachToAxis");
        EdgeType attachedAxis = (EdgeType)info.GetValue("AttachedAxis", parent);
        s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
        s._labelColumn = (Altaxo.Data.ReadableColumnProxy)info.GetValue("LabelColumn", parent);

        if (attachToAxis)
          s._attachedPlane = XmlSerializationSurrogate0.GetDirection(attachedAxis);
        else
          s._attachedPlane = null;

        if (nativeCall)
        {
          // restore the cached values
          s.SetCachedValues();
          s.CreateEventChain();
        }

        return s;
      }
    }



    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelPlotStyle), 3)]
    class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SSerialize(obj, info);
      }
      public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LabelPlotStyle s = (LabelPlotStyle)obj;
        info.AddValue("Font", s._font);
        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("Brush", s._brush);
        info.AddValue("XOffset", s._xOffset);
        info.AddValue("YOffset", s._yOffset);
        info.AddValue("Rotation", s._rotation);
        info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
        info.AddEnum("VerticalAlignment", s.VerticalAlignment);
        info.AddValue("AttachedAxis", s._attachedPlane);
        info.AddValue("Background", s._backgroundStyle);
        info.AddValue("LabelColumn", s._labelColumn);

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return SDeserialize(o, info, parent, true);
      }
      public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
      {

        LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle();

        s._font = (Font)info.GetValue("Font", s);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._brush = (BrushX)info.GetValue("Brush", s);
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");
        s._rotation = info.GetDouble("Rotation");
        s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
        s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
        s.AttachedAxis = (CSPlaneID)info.GetValue("AttachedAxis", s);
        s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
        s._labelColumn = (Altaxo.Data.ReadableColumnProxy)info.GetValue("LabelColumn", parent);


        if (nativeCall)
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
    protected LabelPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      this._cachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
      this._cachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
      this._cachedStringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
    }

    public LabelPlotStyle(LabelPlotStyle from)
    {
      this._font = (Font)from._font.Clone();
      this._independentColor = from._independentColor;
      this._brush = (BrushX)from._brush.Clone();
      this._xOffset = from._xOffset;
      this._yOffset = from._yOffset;
      this._rotation = from._rotation;
      this._backgroundStyle = null == from._backgroundStyle ? null : (IBackgroundStyle)from._backgroundStyle.Clone();
      this._cachedStringFormat = (System.Drawing.StringFormat)from._cachedStringFormat.Clone();
      this._attachedPlane = null==from._attachedPlane ? null : from._attachedPlane.Clone();
      this._labelColumn = (Altaxo.Data.ReadableColumnProxy)from._labelColumn.Clone();
      this._parent = from._parent;

      CreateEventChain();
    }

    public LabelPlotStyle()
      : this((Altaxo.Data.IReadableColumn)null)
    {
    }

    public LabelPlotStyle(Altaxo.Data.IReadableColumn labelColumn)
    {
      this._font = new Font(System.Drawing.FontFamily.GenericSansSerif, 8, GraphicsUnit.World);
      this._independentColor = false;
      this._brush = new BrushX(Color.Black);
      this._xOffset = 0;
      this._yOffset = 0;
      this._rotation = 0;
      this._backgroundStyle = null;
      this._cachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
      this._cachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
      this._cachedStringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
      this._attachedPlane = null;
      this._labelColumn = new Altaxo.Data.ReadableColumnProxy(labelColumn);

      CreateEventChain();
    }


    protected void CreateEventChain()
    {
      this._labelColumn.Changed += new EventHandler(LabelColumnProxy_Changed);
    }

    void LabelColumnProxy_Changed(object sender, EventArgs e)
    {
      this.OnChanged();
    }

    public Altaxo.Data.IReadableColumn LabelColumn
    {
      get
      {
        return _labelColumn == null ? null : _labelColumn.Document;
      }
      set
      {
        _labelColumn.SetDocNode(value);
        OnChanged();
      }
    }

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
    public float FontSize
    {
      get { return _font.Size; }
      set
      {
        float oldValue = FontSize;
        float newValue = Math.Max(0, value);

        if (newValue != oldValue)
        {
          Font oldFont = _font;
          _font = new Font(oldFont.FontFamily.Name, newValue, oldFont.Style, GraphicsUnit.World);
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
      get { return _independentColor; }
      set
      {
        bool oldValue = _independentColor;
        _independentColor = value;
        if (value != oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>The brush color.</summary>
    public System.Drawing.Color Color
    {
      get { return this._brush.Color; }
      set
      {
        Color oldColor = this.Color;
        if (value != oldColor)
        {
          this._brush.SetSolidBrush(value);
          OnChanged(); // Fire Changed event
        }
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
        if (!object.ReferenceEquals(value, oldValue))
        {
          this._backgroundStyle = value;
          OnChanged(); // Fire Changed event
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
        if (value != oldValue)
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
        if (value != oldValue)
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
        if (value != oldValue)
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
        return this._cachedStringFormat.Alignment;
      }
      set
      {
        System.Drawing.StringAlignment oldValue = this.HorizontalAlignment;
        this._cachedStringFormat.Alignment = value;
        if (value != oldValue)
        {
          OnChanged();
        }
      }
    }

    /// <summary>Vertical aligment of the label.</summary>
    public System.Drawing.StringAlignment VerticalAlignment
    {
      get { return this._cachedStringFormat.LineAlignment; }
      set
      {
        System.Drawing.StringAlignment oldValue = this.VerticalAlignment;
        this._cachedStringFormat.LineAlignment = value;
        if (value != oldValue)
        {
          OnChanged();
        }
      }
    }



    /// <summary>Gets/sets the axis this label is attached to. If set to null, the label is positioned normally.</summary>
    public CSPlaneID AttachedAxis
    {
      get { return this._attachedPlane; }
      set
      {
        CSPlaneID oldValue = this._attachedPlane;
        this._attachedPlane = value;
        if (value != oldValue)
        {
          OnChanged();
        }
      }
    }


    protected void SetCachedValues()
    {
    }


    /// <summary>
    /// Paints one label.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="label"></param>
    public void Paint(Graphics g, string label)
    {
      float fontSize = this.FontSize;
      float xpos = (float)(_xOffset * fontSize);
      float ypos = (float)(-_yOffset * fontSize);
      SizeF stringsize = g.MeasureString(label, this._font, new PointF(xpos, ypos), _cachedStringFormat);

      if (this._backgroundStyle != null)
      {
        float x = xpos, y = ypos;
        switch (_cachedStringFormat.Alignment)
        {
          case StringAlignment.Center:
            x -= stringsize.Width / 2;
            break;
          case StringAlignment.Far:
            x -= stringsize.Width;
            break;
        }
        switch (_cachedStringFormat.LineAlignment)
        {
          case StringAlignment.Center:
            y -= stringsize.Height / 2;
            break;
          case StringAlignment.Far:
            y -= stringsize.Height;
            break;
        }
        this._backgroundStyle.Draw(g, new RectangleF(x, y, stringsize.Width, stringsize.Height));
      }

      _brush.Rectangle = new RectangleF(new PointF(xpos, ypos), stringsize);
      g.DrawString(label, _font, _brush, xpos, ypos, _cachedStringFormat);
    }


    public void Paint(Graphics g,
     IPlotArea layer,
     Processed2DPlotData pdata)
    {
      if (this._labelColumn.Document == null)
        return;

      if (null != _attachedPlane)
        layer.UpdateCSPlaneID(_attachedPlane);


      PlotRangeList rangeList = pdata.RangeList;
      PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
      Altaxo.Data.IReadableColumn labelColumn = this._labelColumn.Document;


      // save the graphics stat since we have to translate the origin
      System.Drawing.Drawing2D.GraphicsState gs = g.Save();
      /*
      double bottomPosition = 0;
      double topPosition = 0;
      double leftPosition = 0;
      double rightPosition = 0;

      layer.CoordinateSystem.LogicalToLayerCoordinates(0, 0, out leftPosition, out bottomPosition);
      layer.CoordinateSystem.LogicalToLayerCoordinates(1, 1, out rightPosition, out topPosition);
 */
      double xpos = 0, ypos = 0;
      double xpre, ypre;
      double xdiff, ydiff;
      for (int r = 0; r < rangeList.Count; r++)
      {
        int lower = rangeList[r].LowerBound;
        int upper = rangeList[r].UpperBound;
        int offset = rangeList[r].OffsetToOriginal;
        for (int j = lower; j < upper; j++)
        {
          string label = labelColumn[j + offset].ToString();
          if (label == null || label == string.Empty)
            continue;

          xpre = ptArray[j].X;
          ypre = ptArray[j].Y;

          if (null!=this._attachedPlane)
          {
            Logical3D r3d = layer.GetLogical3D(pdata, j + offset);
            PointF pp = layer.CoordinateSystem.GetPointOnPlane(this._attachedPlane,r3d);
            xpre = pp.X;
            ypre = pp.Y;
          }


          xdiff = xpre - xpos;
          ydiff = ypre - ypos;
          xpos = xpre;
          ypos = ypre;
          g.TranslateTransform((float)xdiff, (float)ydiff);
          if (this._rotation != 0)
            g.RotateTransform((float)-this._rotation);
          this.Paint(g, label);
          if (this._rotation != 0)
            g.RotateTransform((float)this._rotation);
        } // end for
      }

      g.Restore(gs); // Restore the graphics state
    }


    public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      return bounds;
    }

    public object Clone()
    {
      return new LabelPlotStyle(this);
    }

    #region IChangedEventSource Members

   

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != Changed)
        Changed(this, new EventArgs());
    }

    #endregion

    #region IChildChangedEventSink Members


    public void EhChildChanged(object child, EventArgs e)
    {

      if (null != Changed)
        Changed(this, e);
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

   

    #region IG2DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      if (this.IsColorProvider)
        ColorGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if(this.IsColorProvider)
        ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      if (this.IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate() { return PlotColors.Colors.GetPlotColor(this.Color); });

    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if (this.IsColorReceiver)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(PlotColor c) { this.Color = c; });
    }

    #endregion


    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "LabelStyle"; }
    }

    #endregion
  }
}
