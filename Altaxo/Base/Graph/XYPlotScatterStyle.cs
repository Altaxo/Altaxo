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
  namespace XYPlotScatterStyles
  {
    [Serializable]
    public enum Shape
    {
      NoSymbol,
      Square,
      Circle,
      UpTriangle,
      DownTriangle,
      Diamond,
      CrossPlus,
      CrossTimes,
      Star,
      BarHorz,
      BarVert
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Shape), 0)]
    public class ShapeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(Shape), val, true);
      }
    }

    [Serializable]
    public enum Style
    {
      Solid,
      Open,
      DotCenter,
      Hollow,
      Plus,
      Times,
      BarHorz,
      BarVert
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Style), 0)]
    public class StyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(Style), val, true);
      }
    }

    public class ShapeAndStyle
    {
      public Shape Shape;
      public Style Style;

      public ShapeAndStyle()
      {
        this.Shape = Shape.NoSymbol;
        this.Style = Style.Solid;
      }

      public ShapeAndStyle(Shape shape, Style style)
      {
        this.Shape = shape;
        this.Style = style;
      }

      public static ShapeAndStyle Empty
      {
        get
        {
          return new ShapeAndStyle(Shape.NoSymbol, Style.Solid);
        }
      }

      public void SetToNextStyle(ShapeAndStyle template)
      {
        SetToNextStyle(template, 1);
      }
      public void SetToNextStyle(ShapeAndStyle template, int steps)
      {
        int wraps;
        SetToNextStyle(template, steps, out  wraps);
      }
      public void SetToNextStyle(ShapeAndStyle template, int step, out int wraps)
      {
        this.Shape = template.Shape;
        this.Style = template.Style;

        if (template.Shape == Shape.NoSymbol)
        {
          wraps = 0;
          return;
        }

        // first increase the shape value,
        // if this is not possible set shape to first shape, and increase the
        // style value
        // note that the first member of the shape enum is NoSymbol, which should not be 
        // used here

        int nshapes = System.Enum.GetValues(typeof(XYPlotScatterStyles.Shape)).Length - 1;
        int nstyles = System.Enum.GetValues(typeof(XYPlotScatterStyles.Style)).Length;

        int current = ((int)template.Style) * nshapes + ((int)template.Shape) - 1;

        int next = Calc.BasicFunctions.PMod(current + step, nshapes * nstyles);
        wraps = Calc.BasicFunctions.NumberOfWraps(nshapes * nstyles, current, step);

        int nstyle = Calc.BasicFunctions.PMod(next / nshapes, nstyles);
        int nshape = Calc.BasicFunctions.PMod(next, nshapes);

        Shape = (XYPlotScatterStyles.Shape)(nshape+1);
        Style = (XYPlotScatterStyles.Style)nstyle;
      }
    }

    [Flags]
    [Serializable]
    public enum DropLine
    {
      NoDrop = 0,
      Top = 1,
      Bottom = 2,
      Left = 4,
      Right = 8,
      All = Top | Bottom | Left | Right
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DropLine), 0)]
    public class DropLineXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(DropLine), val, true);
      }
    }

  } // end of class XYPlotScatterStyles



  [SerializationSurrogate(0, typeof(XYPlotScatterStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYPlotScatterStyle
    :
    I2DPlotStyle,
    ICloneable,
    Main.IChangedEventSource,
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChildChangedEventSink
  {
    protected XYPlotScatterStyles.Shape _shape;
    protected XYPlotScatterStyles.Style _style;
    protected XYPlotScatterStyles.DropLine _dropLine;
    protected PenHolder _pen;
    protected bool _independentColor;

    protected float _symbolSize;
    protected bool _independentSymbolSize;
    protected float _relativePenWidth;
    protected int _skipFreq;

    // cached values:
    protected GraphicsPath _cachedPath;
    protected bool _cachedFillPath;
    protected BrushHolder _cachedFillBrush;


    #region Serialization
    /// <summary>Used to serialize the XYPlotScatterStyle Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYPlotScatterStyle Version 0.
      /// </summary>
      /// <param name="obj">The XYPlotScatterStyle to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        info.AddValue("Shape", s._shape);
        info.AddValue("Style", s._style);
        info.AddValue("DropLine", s._dropLine);
        info.AddValue("Pen", s._pen);
        info.AddValue("SymbolSize", s._symbolSize);
        info.AddValue("RelativePenWidth", s._relativePenWidth);
      }
      /// <summary>
      /// Deserializes the XYPlotScatterStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty XYPlotScatterStyle object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYPlotScatterStyle.</returns>
      public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        s._shape = (XYPlotScatterStyles.Shape)info.GetValue("Shape", typeof(XYPlotScatterStyles.Shape));
        s._style = (XYPlotScatterStyles.Style)info.GetValue("Style", typeof(XYPlotScatterStyles.Style));
        s._dropLine = (XYPlotScatterStyles.DropLine)info.GetValue("DropLine", typeof(XYPlotScatterStyles.DropLine));
        s._pen = (PenHolder)info.GetValue("Pen", typeof(PenHolder));
        s._symbolSize = info.GetSingle("SymbolSize");
        s._relativePenWidth = info.GetSingle("RelativePenWidth");
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotScatterStyle), 0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotScatterStyle), 1)] // by accident this was never different from 0
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        info.AddValue("Shape", s._shape);
        info.AddValue("Style", s._style);
        info.AddValue("DropLine", s._dropLine);
        info.AddValue("Pen", s._pen);
        info.AddValue("SymbolSize", s._symbolSize);
        info.AddValue("RelativePenWidth", s._relativePenWidth);
      }

      protected virtual XYPlotScatterStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotScatterStyle s = null != o ? (XYPlotScatterStyle)o : new XYPlotScatterStyle();

        s._shape = (XYPlotScatterStyles.Shape)info.GetValue("Shape", typeof(XYPlotScatterStyles.Shape));
        s._style = (XYPlotScatterStyles.Style)info.GetValue("Style", typeof(XYPlotScatterStyles.Style));
        s._dropLine = (XYPlotScatterStyles.DropLine)info.GetValue("DropLine", typeof(XYPlotScatterStyles.DropLine));
        s._pen = (PenHolder)info.GetValue("Pen", typeof(PenHolder));
        s._symbolSize = info.GetSingle("SymbolSize");
        s._relativePenWidth = info.GetSingle("RelativePenWidth");

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotScatterStyle s = SDeserialize(o, info, parent);
       
        // restore the cached values
        s.SetCachedValues();
        s.CreateEventChain();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotScatterStyle), 2)]
      public class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SkipFreq", s._skipFreq);
      }


      protected override XYPlotScatterStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotScatterStyle s = base.SDeserialize(o, info, parent);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._skipFreq = info.GetInt32("SkipFreq");
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


    public void CopyFrom(XYPlotScatterStyle from, bool suppressChangeEvent)
    {
      this._shape = from._shape;
      this._style = from._style;
      this._dropLine = from._dropLine;
      this._pen = null == from._pen ? null : (PenHolder)from._pen.Clone();
      this._independentColor = from._independentColor;
      this._independentSymbolSize = from._independentSymbolSize;


      this._cachedPath = null == from._cachedPath ? null : (GraphicsPath)from._cachedPath.Clone();
      this._cachedFillPath = from._cachedFillPath;
      this._cachedFillBrush = null == from._cachedFillBrush ? null : (BrushHolder)from._cachedFillBrush.Clone();
      this._symbolSize = from._symbolSize;
      this._relativePenWidth = from._relativePenWidth;
      this._skipFreq = from._skipFreq;

      if (!suppressChangeEvent)
        OnChanged();
    }

    public XYPlotScatterStyle(XYPlotScatterStyle from)
    {
      CopyFrom(from, true);
      CreateEventChain();
    }

    public XYPlotScatterStyle(XYPlotScatterStyles.Shape shape, XYPlotScatterStyles.Style style, float size, float penWidth, Color penColor)
    {
      _shape = shape;
      _style = style;
      _dropLine = XYPlotScatterStyles.DropLine.NoDrop;
      _pen = new PenHolder(penColor, penWidth);
      _symbolSize = size;
      this._independentSymbolSize = true;

      _relativePenWidth = penWidth / size;
      _skipFreq = 1;

      // Cached values
      SetCachedValues();
      CreateEventChain();
    }


    public XYPlotScatterStyle()
    {
      this._shape = XYPlotScatterStyles.Shape.Square;
      this._style = XYPlotScatterStyles.Style.Solid;
      this._dropLine = XYPlotScatterStyles.DropLine.NoDrop;
      this._pen = new PenHolder(Color.Black);
      this._independentColor = false;

      this._symbolSize = 8;
      this._independentSymbolSize = true;

      this._relativePenWidth = 0.1f;
      this._skipFreq = 1;
      this._cachedFillPath = true; // since default is solid
      this._cachedFillBrush = new BrushHolder(Color.Black);
      this._cachedPath = GetPath(_shape, _style, _symbolSize);
      CreateEventChain();
    }

    protected void CreateEventChain()
    {
      if (null != _pen)
        _pen.Changed += new EventHandler(this.EhChildChanged);
    }

    public void SetToNextStyle(XYPlotScatterStyle template)
    {
      SetToNextStyle(template, 1);
    }
    public void SetToNextStyle(XYPlotScatterStyle template, int steps)
    {
      int wraps;
      SetToNextStyle(template, steps, out  wraps);
    }
    public void SetToNextStyle(XYPlotScatterStyle template, int step, out int wraps)
    {
      CopyFrom(template, true);
      // first increase the shape value,
      // if this is not possible set shape to first shape, and increase the
      // style value
      // note that the first member of the shape enum is NoSymbol, which should not be 
      // used here

      int nshapes = System.Enum.GetValues(typeof(XYPlotScatterStyles.Shape)).Length - 1;
      int nstyles = System.Enum.GetValues(typeof(XYPlotScatterStyles.Style)).Length;

      int current = ((int)template.Style) * nshapes + ((int)template.Shape) - 1;

      int next = Calc.BasicFunctions.PMod(current + step, nshapes * nstyles);
      wraps = Calc.BasicFunctions.NumberOfWraps(nshapes * nstyles, current, step);

      int nstyle = Calc.BasicFunctions.PMod(next / nshapes, nstyles);
      int nshape = Calc.BasicFunctions.PMod(next, nshapes);

      Shape = (XYPlotScatterStyles.Shape)nshape;
      Style = (XYPlotScatterStyles.Style)nstyle;
    }

    public XYPlotScatterStyles.Shape Shape
    {
      get { return this._shape; }
      set
      {
        if (value != this._shape)
        {
          this._shape = value;

          // ensure that a pen is set if Shape is other than nosymbol
          if (value != XYPlotScatterStyles.Shape.NoSymbol && null == this._pen)
            _pen = new PenHolder(Color.Black);

          SetCachedValues();

          OnChanged(); // Fire Changed event
        }
      }
    }





    public XYPlotScatterStyles.Style Style
    {
      get { return this._style; }
      set
      {
        if (value != this._style)
        {
          this._style = value;
          SetCachedValues();

          OnChanged(); // Fire Changed event
        }
      }
    }

    public void SetShapeAndStyle(XYPlotScatterStyles.ShapeAndStyle shapeandstyle)
    {
      this.Style = shapeandstyle.Style;
      this.Shape = shapeandstyle.Shape;
    }

    public XYPlotScatterStyles.DropLine DropLine
    {
      get { return _dropLine; }
      set
      {
        if (_dropLine != value)
        {
          _dropLine = value;
          OnChanged(); // Fire Changed event
        }
      }
    }

    public bool IsVisible
    {
      get
      {
        if (_shape != XYPlotScatterStyles.Shape.NoSymbol)
          return true;
        if (_dropLine != DropLine.NoDrop)
          return true;

        return false;
      }
    }

    public PenHolder Pen
    {
      get { return this._pen; }
      set
      {
        // ensure pen can be only set to null if NoSymbol
        if (value != null || XYPlotScatterStyles.Shape.NoSymbol == this._shape)
        {
          _pen = null == value ? null : (PenHolder)value.Clone();
          _pen.Changed += new EventHandler(this.EhChildChanged);
          OnChanged(); // Fire Changed event
        }
      }
    }


    public System.Drawing.Color Color
    {
      get { return this._pen.Color; }
      set
      {
        this._pen.Color = value;
        
        OnChanged(); // Fire Changed event
      }
    }

    public bool IndependentColor
    {
      get
      {
        return _independentColor;
      }
      set
      {
        bool oldValue = _independentColor;
        _independentColor = value;
        if (value != oldValue)
          OnChanged();
      }
    }



    public float SymbolSize
    {
      get { return _symbolSize; }
      set
      {
        if (value != _symbolSize)
        {
          _symbolSize = value;
          _cachedPath = GetPath(this._shape, this._style, this._symbolSize);
          _pen.Width = _symbolSize * _relativePenWidth;
          OnChanged(); // Fire Changed event
        }
      }
    }

    public bool IndependentSymbolSize
    {
      get
      {
        return _independentSymbolSize;
      }
      set
      {
        bool oldValue = _independentSymbolSize;
        _independentSymbolSize = value;
        if (value != oldValue)
          OnChanged();
      }
    }

    public int SkipFrequency
    {
      get { return _skipFreq; }
      set
      {
        if (value != _skipFreq)
        {
          _skipFreq = value;
          OnChanged(); // Fire Changed event
        }
      }
    }

    protected void SetCachedValues()
    {
      _cachedPath = GetPath(this._shape, this._style, this._symbolSize);

      _cachedFillPath = _style == XYPlotScatterStyles.Style.Solid || _style == XYPlotScatterStyles.Style.Open || _style == XYPlotScatterStyles.Style.DotCenter;

      if (this._style != XYPlotScatterStyles.Style.Solid)
        _cachedFillBrush = new BrushHolder(Color.White);
      else if (this._pen.PenType == PenType.SolidColor)
        _cachedFillBrush = new BrushHolder(_pen.Color);
      else
        _cachedFillBrush = new BrushHolder(_pen.BrushHolder);
    }


    public void Paint(Graphics g)
    {
      if (_cachedFillPath)
        g.FillPath(_cachedFillBrush, _cachedPath);

      g.DrawPath(_pen, _cachedPath);
    }

    public object Clone()
    {
      return new XYPlotScatterStyle(this);
    }

    public static GraphicsPath GetPath(XYPlotScatterStyles.Shape sh, XYPlotScatterStyles.Style st, float size)
    {
      float sizeh = size / 2;
      GraphicsPath gp = new GraphicsPath();


      switch (sh)
      {
        case Shape.Square:
          gp.AddRectangle(new RectangleF(-sizeh, -sizeh, size, size));
          gp.StartFigure();
          break;
        case Shape.Circle:
          gp.AddEllipse(-sizeh, -sizeh, size, size);
          gp.StartFigure();
          break;
        case Shape.UpTriangle:
          gp.AddLine(0, -sizeh, 0.3301270189f * size, 0.5f * sizeh);
          gp.AddLine(0.43301270189f * size, 0.5f * sizeh, -0.43301270189f * size, 0.5f * sizeh);
          gp.CloseFigure();
          break;
        case Shape.DownTriangle:
          gp.AddLine(-0.43301270189f * sizeh, -0.5f * sizeh, 0.43301270189f * size, -0.5f * sizeh);
          gp.AddLine(0.43301270189f * size, -0.5f * sizeh, 0, sizeh);
          gp.CloseFigure();
          break;
        case Shape.Diamond:
          gp.AddLine(0, -sizeh, sizeh, 0);
          gp.AddLine(sizeh, 0, 0, sizeh);
          gp.AddLine(0, sizeh, -sizeh, 0);
          gp.CloseFigure();
          break;
        case Shape.CrossPlus:
          gp.AddLine(-sizeh, 0, sizeh, 0);
          gp.StartFigure();
          gp.AddLine(0, sizeh, 0, -sizeh);
          gp.StartFigure();
          break;
        case Shape.CrossTimes:
          gp.AddLine(-sizeh, -sizeh, sizeh, sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh, sizeh, sizeh, -sizeh);
          gp.StartFigure();
          break;
        case Shape.Star:
          gp.AddLine(-sizeh, 0, sizeh, 0);
          gp.StartFigure();
          gp.AddLine(0, sizeh, 0, -sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh, -sizeh, sizeh, sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh, sizeh, sizeh, -sizeh);
          gp.StartFigure();
          break;
        case Shape.BarHorz:
          gp.AddLine(-sizeh, 0, sizeh, 0);
          gp.StartFigure();
          break;
        case Shape.BarVert:
          gp.AddLine(0, -sizeh, 0, sizeh);
          gp.StartFigure();
          break;
      }

      switch (st)
      {
        case Style.DotCenter:
          gp.AddEllipse(-0.125f * sizeh, -0.125f * sizeh, 0.125f * size, 0.125f * size);
          gp.StartFigure();
          break;
        case Style.Plus:
          gp.AddLine(-sizeh, 0, sizeh, 0);
          gp.StartFigure();
          gp.AddLine(0, sizeh, 0, -sizeh);
          gp.StartFigure();
          break;
        case Style.Times:
          gp.AddLine(-sizeh, -sizeh, sizeh, sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh, sizeh, sizeh, -sizeh);
          gp.StartFigure();
          break;
        case Style.BarHorz:
          gp.AddLine(-sizeh, 0, sizeh, 0);
          gp.StartFigure();
          break;
        case Style.BarVert:
          gp.AddLine(0, -sizeh, 0, sizeh);
          gp.StartFigure();
          break;
      }
      return gp;
    }
    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    protected virtual void OnChanged()
    {
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

    #region I2DPlotItem Members

    public bool IsColorProvider
    {
      get
      {
        return true;
      }
    }

    public bool IsColorReceiver
    {
      get { return !this._independentColor; }
    }

    public bool IsSymbolSizeProvider
    {
      get
      {
        return this._shape != Shape.NoSymbol;
      }
    }

    public bool IsSymbolSizeReceiver
    {
      get
      {
        return this._shape != Shape.NoSymbol && !this._independentSymbolSize;
      }
    }



    #endregion


    public void Paint(Graphics g, IPlotArea layer, PlotRangeList rangeList, PointF[] ptArray)
    {
      // adjust the skip frequency if it was not set appropriate
      if (_skipFreq <= 0)
        _skipFreq = 1;

      // paint the drop style
      if (this.DropLine != XYPlotScatterStyles.DropLine.NoDrop)
      {
        PenHolder ph = this.Pen;
        ph.Cached = true;
        Pen pen = ph.Pen; // do not dispose this pen, since it is cached
        //       float xe=layer.Size.Width;
        //       float ye=layer.Size.Height;

        double xleft, xright, ytop, ybottom;
        layer.LogicalToAreaConversion.Convert(0, 0, out xleft, out ybottom);
        layer.LogicalToAreaConversion.Convert(1, 1, out xright, out ytop);
        float xe = (float)xright;
        float ye = (float)ybottom;

        if ((0 != (this.DropLine & XYPlotScatterStyles.DropLine.Top)) && (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Bottom)))
        {
          for (int j = 0; j < ptArray.Length; j+=_skipFreq)
          {
            float x = ptArray[j].X;
            g.DrawLine(pen, x, 0, x, ye);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Top))
        {
          for (int j = 0; j < ptArray.Length; j+=_skipFreq)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, x, 0, x, y);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Bottom))
        {
          for (int j = 0; j < ptArray.Length; j+=_skipFreq)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, x, y, x, ye);
          }
        }

        if ((0 != (this.DropLine & XYPlotScatterStyles.DropLine.Left)) && (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Right)))
        {
          for (int j = 0; j < ptArray.Length; j+=_skipFreq)
          {
            float y = ptArray[j].Y;
            g.DrawLine(pen, 0, y, xe, y);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Right))
        {
          for (int j = 0; j < ptArray.Length; j+=_skipFreq)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, x, y, xe, y);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Left))
        {
          for (int j = 0; j < ptArray.Length; j+=_skipFreq)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, 0, y, x, y);
          }
        }
      } // end paint the drop style


      // paint the scatter style
      if (this.Shape != XYPlotScatterStyles.Shape.NoSymbol)
      {
        // save the graphics stat since we have to translate the origin
        System.Drawing.Drawing2D.GraphicsState gs = g.Save();


        float xpos = 0, ypos = 0;
        float xdiff, ydiff;
        for (int j = 0; j < ptArray.Length; j+=_skipFreq)
        {
          xdiff = ptArray[j].X - xpos;
          ydiff = ptArray[j].Y - ypos;
          xpos = ptArray[j].X;
          ypos = ptArray[j].Y;
          g.TranslateTransform(xdiff, ydiff);
          this.Paint(g);
        } // end for

        g.Restore(gs); // Restore the graphics state

      }
    }
  }
}

