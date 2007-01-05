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
//using System.Runtime.InteropServices;


namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Graph.Plot.Groups;
  using Plot.Groups;
  using Plot.Data;
  using Graph.Plot.Data;

  namespace XYPlotLineStyles
  {
    [Serializable]
    public enum FillDirection
    {
      Left = 0,
      Bottom = 1,
      Right = 2,
      Top = 3
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLineStyles.FillDirection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FillDirection), 1)]
    public class FillDirectionXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(FillDirection), val, true);
      }
    }

    [Serializable]
    public enum ConnectionStyle
    {
      NoLine,
      Straight,
      Segment2,
      Segment3,
      Spline,
      Bezier,
      StepHorz,
      StepVert,
      StepHorzCenter,
      StepVertCenter
    }
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLineStyles.ConnectionStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConnectionStyle), 1)]
    public class ConnectionStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(ConnectionStyle), val, true);
      }
    }

  }


  /// <summary>
  /// Summary description for XYPlotLineStyle.
  /// </summary>
  [SerializationSurrogate(0, typeof(LinePlotStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class LinePlotStyle
    :
    ICloneable,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    Main.IDocumentNode,
    System.Runtime.Serialization.IDeserializationCallback,
    IG2DPlotStyle
  {
    /// <summary>
    /// Template to make a line draw.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="pdata">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
    /// <param name="overriderange">The plot range to use.</param>
    /// <param name="layer">Graphics layer.</param>
    /// <param name="symbolGap">The size of the symbol gap.</param>
    public delegate void PaintOneRangeTemplate(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange overriderange,
      IPlotArea layer,
      float symbolGap);



    protected PenX _penHolder;
    protected XYPlotLineStyles.ConnectionStyle _connectionStyle;
    protected bool _useLineSymbolGap;
    protected float _symbolGap;
    protected bool _ignoreMissingPoints; // treat missing points as if not present (connect lines over missing points) 
    protected bool _fillArea;
    protected BrushX _fillBrush; // brush to fill the area under the line
    protected CSPlaneID _fillDirection; // the direction to fill
    protected bool _independentColor;
    /// <summary>If true, the fill color is not set by the color group style, but is independent.</summary>
    protected bool _independentFillColor;
    /// <summary>If true, the start and the end point of the line are connected too.</summary>
    protected bool _connectCircular;


    // cached values
    [NonSerialized]
    protected PaintOneRangeTemplate _cachedPaintOneRange; // subroutine to paint a single range
    [NonSerialized]
    protected Main.IDocumentNode _parentObject;
    [field: NonSerialized]
    public event System.EventHandler Changed;

    #region Serialization
    /// <summary>Used to serialize the XYPlotLineStyle Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYPlotLineStyle Version 0.
      /// </summary>
      /// <param name="obj">The XYPlotLineStyle to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
        LinePlotStyle s = (LinePlotStyle)obj;
        info.AddValue("Pen", s._penHolder);
        info.AddValue("Connection", s._connectionStyle);
        info.AddValue("LineSymbolGap", s._useLineSymbolGap);
        info.AddValue("IgnoreMissingPoints", s._ignoreMissingPoints);
        info.AddValue("FillArea", s._fillArea);
        info.AddValue("FillBrush", s._fillBrush);
        info.AddValue("FillDirection", s._fillDirection);
      }
      /// <summary>
      /// Deserializes the XYPlotLineStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty XYPlotLineStyle object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYPlotLineStyle.</returns>
      public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
      {
        LinePlotStyle s = (LinePlotStyle)obj;

        s._penHolder = (PenX)info.GetValue("Pen", typeof(PenX));
        s.Connection = (XYPlotLineStyles.ConnectionStyle)info.GetValue("Connection", typeof(XYPlotLineStyles.ConnectionStyle));
        s._useLineSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingPoints = info.GetBoolean("IgnoreMissingPoints");
        s._fillArea = info.GetBoolean("FillArea");
        s._fillBrush = (BrushX)info.GetValue("FillBrush", typeof(BrushX));
        s._fillDirection = (CSPlaneID)info.GetValue("FillDirection", typeof(CSPlaneID));

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLineStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyle", 1)] // by accident, it was never different from 0
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinePlotStyle s = (LinePlotStyle)obj;
        info.AddValue("Pen", s._penHolder);
        info.AddValue("Connection", s._connectionStyle);
        info.AddValue("LineSymbolGap", s._useLineSymbolGap);
        info.AddValue("IgnoreMissingPoints", s._ignoreMissingPoints);
        info.AddValue("FillArea", s._fillArea);
        info.AddValue("FillBrush", s._fillBrush);
        info.AddValue("FillDirection", s._fillDirection);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinePlotStyle s = SDeserialize(o, info, parent);
        s.CreateEventChain();
        return s;
      }

      public virtual LinePlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        LinePlotStyle s = null != o ? (LinePlotStyle)o : new LinePlotStyle();

        s._penHolder = (PenX)info.GetValue("Pen", typeof(PenX));
        s.Connection = (XYPlotLineStyles.ConnectionStyle)info.GetValue("Connection", typeof(XYPlotLineStyles.ConnectionStyle));
        s._useLineSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingPoints = info.GetBoolean("IgnoreMissingPoints");
        s._fillArea = info.GetBoolean("FillArea");
        s._fillBrush = (BrushX)info.GetValue("FillBrush", typeof(BrushX));
        XYPlotLineStyles.FillDirection fillDir = (XYPlotLineStyles.FillDirection)info.GetValue("FillDirection", typeof(XYPlotLineStyles.FillDirection));
        if (s._fillArea)
          s._fillDirection = GetFillDirection(fillDir);
        
        return s;
      }

      protected CSPlaneID GetFillDirection(XYPlotLineStyles.FillDirection fillDir)
      {
        switch (fillDir)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            return CSPlaneID.Bottom;
          case XYPlotLineStyles.FillDirection.Top:
            return CSPlaneID.Top;
          case XYPlotLineStyles.FillDirection.Left:
            return CSPlaneID.Left;
          case XYPlotLineStyles.FillDirection.Right:
            return CSPlaneID.Right;
        }
        return null;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLineStyle", 2)]
    class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);
        LinePlotStyle s = (LinePlotStyle)obj;
        info.AddValue("IndependentColor", s._independentColor);

      }
      public override LinePlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinePlotStyle s = null != o ? (LinePlotStyle)o : new LinePlotStyle();

        base.SDeserialize(o, info, parent);
        s._independentColor = info.GetBoolean("IndependentColor");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinePlotStyle), 3)]
    class XmlSerializationSurrogate3 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);
        LinePlotStyle s = (LinePlotStyle)obj;
        info.AddValue("IndependentColor", s._independentColor);
        info.AddValue("IndependentFillColor", s._independentFillColor);
        info.AddValue("ConnectCircular", s._connectCircular);
      }
      public override LinePlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinePlotStyle s = null != o ? (LinePlotStyle)o : new LinePlotStyle();

        s._penHolder = (PenX)info.GetValue("Pen", typeof(PenX));
        s.Connection = (XYPlotLineStyles.ConnectionStyle)info.GetValue("Connection", typeof(XYPlotLineStyles.ConnectionStyle));
        s._useLineSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingPoints = info.GetBoolean("IgnoreMissingPoints");
        s._fillArea = info.GetBoolean("FillArea");
        s._fillBrush = (BrushX)info.GetValue("FillBrush", typeof(BrushX));
        s._fillDirection = (CSPlaneID)info.GetValue("FillDirection", s);
        s._independentColor = info.GetBoolean("IndependentColor");
        s._independentFillColor = info.GetBoolean("IndependentFillColor");
        s._connectCircular = info.GetBoolean("ConnectCircular");
        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      CreateEventChain();
    }
    #endregion



    public LinePlotStyle()
    {
      _penHolder = new PenX(Color.Black);
      _useLineSymbolGap = true;
      _ignoreMissingPoints = false;
      _fillArea = false;
      _fillBrush = new BrushX(Color.Black);
      _fillDirection = null;
      _connectionStyle = XYPlotLineStyles.ConnectionStyle.Straight;
      _cachedPaintOneRange = new PaintOneRangeTemplate(StraightConnection_PaintOneRange);
      _independentColor = false;

      CreateEventChain();
    }


    public void CopyFrom(LinePlotStyle from, bool suppressChangeEvent)
    {
      this._penHolder = null == from._penHolder ? null : (PenX)from._penHolder.Clone();
      this._useLineSymbolGap = from._useLineSymbolGap;
      this._symbolGap = from._symbolGap;
      this._ignoreMissingPoints = from._ignoreMissingPoints;
      this._fillArea = from._fillArea;
      this._fillBrush = null == from._fillBrush ? null : (BrushX)from._fillBrush.Clone();
      this._fillDirection = null==from._fillDirection ? null : from._fillDirection.Clone();
      this.Connection = from._connectionStyle; // beachte links nur Connection, damit das Template mit gesetzt wird
      this._independentColor = from._independentColor;
      this._independentFillColor = from._independentFillColor;
      this._connectCircular = from._connectCircular;

      this._parentObject = from._parentObject;

      if (!suppressChangeEvent)
        OnChanged();
    }

    public LinePlotStyle(LinePlotStyle from)
    {

      CopyFrom(from, true);
      CreateEventChain();
    }

    protected virtual void CreateEventChain()
    {
      if (null != _penHolder)
        _penHolder.Changed += new EventHandler(this.EhChildChanged);

      if (null != _fillBrush)
        _fillBrush.Changed += new EventHandler(this.EhChildChanged);
    }

    public XYPlotLineStyles.ConnectionStyle Connection
    {
      get { return _connectionStyle; }
      set
      {
        _connectionStyle = value;
        switch (_connectionStyle)
        {
          case XYPlotLineStyles.ConnectionStyle.NoLine:
            _cachedPaintOneRange = new PaintOneRangeTemplate(NoConnection_PaintOneRange);
            break;
          default:
          case XYPlotLineStyles.ConnectionStyle.Straight:
            _cachedPaintOneRange = new PaintOneRangeTemplate(StraightConnection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.Segment2:
            _cachedPaintOneRange = new PaintOneRangeTemplate(Segment2Connection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.Segment3:
            _cachedPaintOneRange = new PaintOneRangeTemplate(Segment3Connection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.Spline:
            _cachedPaintOneRange = new PaintOneRangeTemplate(SplineConnection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.Bezier:
            _cachedPaintOneRange = new PaintOneRangeTemplate(BezierConnection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.StepHorz:
            _cachedPaintOneRange = new PaintOneRangeTemplate(StepHorzConnection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.StepVert:
            _cachedPaintOneRange = new PaintOneRangeTemplate(StepVertConnection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.StepHorzCenter:
            _cachedPaintOneRange = new PaintOneRangeTemplate(StepHorzCenterConnection_PaintOneRange);
            break;
          case XYPlotLineStyles.ConnectionStyle.StepVertCenter:
            _cachedPaintOneRange = new PaintOneRangeTemplate(StepVertCenterConnection_PaintOneRange);
            break;
        } // end switch
        OnChanged(); // Fire Changed event
      }
    }

    public bool LineSymbolGap
    {
      get
      {
        return _useLineSymbolGap;
      }
      set
      {
        bool oldValue = _useLineSymbolGap;
        _useLineSymbolGap = value;
        if (value != oldValue)
          OnChanged();
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

    public bool IndependentFillColor
    {
      get
      {
        return _independentFillColor;
      }
      set
      {
        bool oldValue = _independentFillColor;
        _independentFillColor = value;
        if (value != oldValue)
          OnChanged();
      }
    }

    public bool ConnectCircular
    {
      get
      {
        return _connectCircular;
      }
      set
      {
        bool oldValue = _connectCircular;
        _connectCircular = value;
        if (value != oldValue)
          OnChanged();
      }
    }




    public void SetToNextLineStyle(System.Drawing.Drawing2D.DashStyle template)
    {
      SetToNextLineStyle(template, 1);
    }

    public void SetToNextLineStyle(System.Drawing.Drawing2D.DashStyle template, int step)
    {
      // this.CopyFrom(template,true);

      // note a exception: since the last dashstyle is "Custom", not only the next dash
      // style has to be defined, but also the overnect to avoid the selection of "Custom"

      int len = System.Enum.GetValues(typeof(DashStyle)).Length;
      int next = step + (int)template;
      this.PenHolder.DashStyle = (DashStyle)Calc.BasicFunctions.PMod(next, len - 1);


      OnChanged(); // Fire Changed event
    }

    public PenX PenHolder
    {
      get { return _penHolder; }
    }

    public bool FillArea
    {
      get { return this._fillArea; }
      set
      {
        this._fillArea = value;
        // ensure that if value is true, there is a fill brush which is not null
        if (true == value && null == this._fillBrush)
          this._fillBrush = new BrushX(Color.White);

        OnChanged(); // Fire Changed event
      }
    }

    public CSPlaneID FillDirection
    {
      get { return this._fillDirection; }
      set
      {
        CSPlaneID oldvalue = _fillDirection;
        _fillDirection = value;
        if (oldvalue != value)
        {
          OnChanged(); // Fire Changed event
        }
      }
    }

    public BrushX FillBrush
    {
      get { return this._fillBrush; }
      set
      {
        // copy the brush only if not null
        if (null != value)
        {
          this._fillBrush = (BrushX)value.Clone();
          this._fillBrush.Changed += new EventHandler(this.EhChildChanged);
          OnChanged(); // Fire Changed event
        }
        else
          throw new ArgumentNullException("FillBrush", "FillBrush must not be set to null, instead set FillArea to false");
      }
    }

    public bool IsVisible
    {
      get
      {
        if (_connectionStyle != XYPlotLineStyles.ConnectionStyle.NoLine)
          return true;
        if (_fillArea)
          return true;

        return false;
      }
    }

    public object Clone()
    {
      return new LinePlotStyle(this);
    }

    public virtual void PaintLine(Graphics g, PointF beg, PointF end)
    {
      if (null != _penHolder)
      {
        g.DrawLine(_penHolder, beg, end);
      }
    }

    public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      if (this.Connection != XYPlotLineStyles.ConnectionStyle.NoLine)
      {
        GraphicsState gs = g.Save();
        g.TranslateTransform(bounds.X + 0.5f * bounds.Width, bounds.Y + 0.5f * bounds.Height);
        float halfwidth = bounds.Width / 2;
        float symsize = _symbolGap;

        if (this.LineSymbolGap == true)
        {
          // plot a line with the length of symbolsize from 
          PaintLine(g, new PointF(-halfwidth, 0), new PointF(-symsize, 0));
          PaintLine(g, new PointF(symsize, 0), new PointF(halfwidth, 0));
        }
        else // no gap
        {
          PaintLine(g, new PointF(-halfwidth, 0), new PointF(halfwidth, 0));
        }
        g.Restore(gs);
      }
    
      return bounds;
    }


    public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata)
    {
      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      PlotRangeList rangeList = pdata.RangeList;
      float symbolGap = this._symbolGap;

      // ensure that brush and pen are cached
      if (null != _penHolder) _penHolder.Cached = true;
    
      if (_fillArea)
      {
        if (null != _fillBrush)
          _fillBrush.Rectangle = new RectangleF(PointF.Empty, layer.Size);

        layer.UpdateCSPlaneID(_fillDirection);
      }

      int rangelistlen = rangeList.Count;
     
      if (this._ignoreMissingPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        PlotRange totalRange = new PlotRange(rangeList[0].LowerBound, rangeList[rangelistlen - 1].UpperBound);
        _cachedPaintOneRange(g, pdata, totalRange, layer, symbolGap);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for (int i = 0; i < rangelistlen; i++)
        {
          _cachedPaintOneRange(g, pdata, rangeList[i], layer, symbolGap);
        }
      }
    }


    public void NoConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
    }

    public void StraightConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      PointF[] linepts = new PointF[range.Length+(_connectCircular?1:0)];
      Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
      if (_connectCircular) linepts[linepts.Length - 1] = linepts[0];
      int lastIdx = range.Length - 1 + (_connectCircular ? 1 : 0);
      GraphicsPath gp = new GraphicsPath();
      SizeF layerSize = layer.Size;
   
      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddLines(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, _connectCircular ? range.OriginalFirstPoint: range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      if (this._useLineSymbolGap && symbolGap > 0)
      {
        float xdiff, ydiff, rel, startx, starty, stopx, stopy;
        for (int i = 0; i < lastIdx; i++)
        {
          xdiff = linepts[i + 1].X - linepts[i].X;
          ydiff = linepts[i + 1].Y - linepts[i].Y;
          rel = (float)(symbolGap / System.Math.Sqrt(xdiff * xdiff + ydiff * ydiff));
          if (rel < 0.5) // a line only appears if the relative gap is smaller 1/2
          {
            startx = linepts[i].X + rel * xdiff;
            starty = linepts[i].Y + rel * ydiff;
            stopx = linepts[i + 1].X - rel * xdiff;
            stopy = linepts[i + 1].Y - rel * ydiff;

            gp.AddLine(startx, starty, stopx, stopy);
            gp.StartFigure();
          }
        } // end for
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        if (linepts.Length > 1) // we don't want to have a drawing exception if number of points is only one
        {
          g.DrawLines(this._penHolder, linepts);
        }
      }
    } // end function PaintOneRange

    public void SplineConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
      int lastIdx = range.Length - 1;
      GraphicsPath gp = new GraphicsPath();
      SizeF layerSize = layer.Size;

      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddCurve(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      // unfortuately, there is no easy way to support line/symbol gaps
      // thats why I ignore this value and draw a curve through the points
      g.DrawCurve(this._penHolder, linepts);

    } // end function PaintOneRange (Spline)


    public void BezierConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange rangeRaw,
      IPlotArea layer,
      float symbolGap)
    {
      // Bezier is only supported with point numbers n=4+3*k
      // so trim the range appropriately
      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      PlotRange range = new PlotRange(rangeRaw);
      SizeF layerSize = layer.Size;
      range.UpperBound = range.LowerBound + 3 * ((range.Length + 2) / 3) - 2;
      if (range.Length < 4)
        return; // then to less points are in this range

      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
      int lastIdx = range.Length - 1;
      GraphicsPath gp = new GraphicsPath();

      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddBeziers(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      // unfortuately, there is no easy way to support line/symbol gaps
      // thats why I ignore this value and draw a curve through the points
      g.DrawBeziers(this._penHolder, linepts);

    } // end function PaintOneRange BezierLineStyle


    public void StepHorzConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      if (range.Length < 2)
        return;

      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      SizeF layerSize = layer.Size;
      PointF[] linepts = new PointF[range.Length * 2 - 1];
      int end = range.UpperBound - 1;
      int i, j;
      for (i = 0, j = range.LowerBound; j < end; i += 2, j++)
      {
        linepts[i] = linePoints[j];
        linepts[i + 1].X = linePoints[j + 1].X;
        linepts[i + 1].Y = linePoints[j].Y;
      }
      linepts[i] = linePoints[j];
      int lastIdx = i;

      GraphicsPath gp = new GraphicsPath();

      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddLines(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      if (this._useLineSymbolGap && symbolGap > 0)
      {
        end = range.UpperBound - 1;
        float symbolGapSquared = symbolGap * symbolGap;
        for (j = range.LowerBound; j < end; j++)
        {
          float xmiddle = linePoints[j + 1].X;
          float ymiddle = linePoints[j].Y;

          // decide if horz line is necessary
          float xdiff = System.Math.Abs(linePoints[j + 1].X - linePoints[j].X);
          float ydiff = System.Math.Abs(linePoints[j + 1].Y - linePoints[j].Y);

          float xrel1 = symbolGap / xdiff;
          float xrel2 = ydiff > symbolGap ? 1 : (float)(1 - System.Math.Sqrt(symbolGapSquared - ydiff * ydiff) / xdiff);

          float yrel1 = xdiff > symbolGap ? 0 : (float)(System.Math.Sqrt(symbolGapSquared - xdiff * xdiff) / ydiff);
          float yrel2 = 1 - symbolGap / ydiff;

          xdiff = linePoints[j + 1].X - linePoints[j].X;
          ydiff = linePoints[j + 1].Y - linePoints[j].Y;

          if (xrel1 < xrel2)
            gp.AddLine(linePoints[j].X + xrel1 * xdiff, linePoints[j].Y, linePoints[j].X + xrel2 * xdiff, linePoints[j].Y);

          if (yrel1 < yrel2)
            gp.AddLine(linePoints[j + 1].X, linePoints[j].Y + yrel1 * ydiff, linePoints[j + 1].X, linePoints[j].Y + yrel2 * ydiff);

          if (xrel1 < xrel2 || yrel1 < yrel2)
            gp.StartFigure();
        }
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepHorzLineStyle

    public void StepVertConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      if (range.Length < 2)
        return;

      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      SizeF layerSize = layer.Size;
      PointF[] linepts = new PointF[range.Length * 2 - 1];
      int end = range.UpperBound - 1;
      int i, j;
      for (i = 0, j = range.LowerBound; j < end; i += 2, j++)
      {
        linepts[i] = linePoints[j];
        linepts[i + 1].X = linePoints[j].X;
        linepts[i + 1].Y = linePoints[j + 1].Y;
      }
      linepts[i] = linePoints[j];
      int lastIdx = i;

      GraphicsPath gp = new GraphicsPath();

      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddLines(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      if (this._useLineSymbolGap && symbolGap > 0)
      {
        end = range.UpperBound - 1;
        float symbolGapSquared = symbolGap * symbolGap;
        for (j = range.LowerBound; j < end; j++)
        {
          float xmiddle = linePoints[j + 1].X;
          float ymiddle = linePoints[j].Y;

          // decide if horz line is necessary
          float xdiff = System.Math.Abs(linePoints[j + 1].X - linePoints[j].X);
          float ydiff = System.Math.Abs(linePoints[j + 1].Y - linePoints[j].Y);

          float yrel1 = symbolGap / ydiff;
          float yrel2 = xdiff > symbolGap ? 1 : (float)(1 - System.Math.Sqrt(symbolGapSquared - xdiff * xdiff) / ydiff);

          float xrel1 = ydiff > symbolGap ? 0 : (float)(System.Math.Sqrt(symbolGapSquared - ydiff * ydiff) / xdiff);
          float xrel2 = 1 - symbolGap / xdiff;

          xdiff = linePoints[j + 1].X - linePoints[j].X;
          ydiff = linePoints[j + 1].Y - linePoints[j].Y;

          if (yrel1 < yrel2)
            gp.AddLine(linePoints[j].X, linePoints[j].Y + yrel1 * ydiff, linePoints[j].X, linePoints[j].Y + yrel2 * ydiff);

          if (xrel1 < xrel2)
            gp.AddLine(linePoints[j].X + xrel1 * ydiff, linePoints[j + 1].Y, linePoints[j].X + xrel2 * xdiff, linePoints[j + 1].Y);

          if (xrel1 < xrel2 || yrel1 < yrel2)
            gp.StartFigure();
        }
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepVertLineStyle

    public void StepVertCenterConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      if (range.Length < 2)
        return;

      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      SizeF layerSize = layer.Size;
      PointF[] linepts = new PointF[range.Length * 3 - 2];
      int end = range.UpperBound - 1;
      int i, j;
      for (i = 0, j = range.LowerBound; j < end; i += 3, j++)
      {
        linepts[i] = linePoints[j];
        linepts[i + 1].X = linePoints[j].X;
        linepts[i + 1].Y = 0.5f * (linePoints[j].Y + linePoints[j + 1].Y);
        linepts[i + 2].X = linePoints[j + 1].X;
        linepts[i + 2].Y = linepts[i + 1].Y;
      }
      linepts[i] = linePoints[j];
      int lastIdx = i;

      GraphicsPath gp = new GraphicsPath();

      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddLines(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      if (this._useLineSymbolGap && symbolGap > 0)
      {
        end = linepts.Length - 1;
        float symbolGapSquared = symbolGap * symbolGap;
        for (j = 0; j < end; j += 3)
        {
          float ydiff = linepts[j + 1].Y - linepts[j].Y;
          if (System.Math.Abs(ydiff) > symbolGap) // then the two vertical lines are visible, and full visible horz line
          {
            gp.AddLine(linepts[j].X, linepts[j].Y + (ydiff > 0 ? symbolGap : -symbolGap), linepts[j + 1].X, linepts[j + 1].Y);
            gp.AddLine(linepts[j + 1].X, linepts[j + 1].Y, linepts[j + 2].X, linepts[j + 2].Y);
            gp.AddLine(linepts[j + 2].X, linepts[j + 2].Y, linepts[j + 3].X, linepts[j + 3].Y - (ydiff > 0 ? symbolGap : -symbolGap));
            gp.StartFigure();
          }
          else // no vertical lines visible, and horz line can be shortened
          {
            // Calculate, how much of the horz line is invisible on both ends
            float xoffs = (float)(System.Math.Sqrt(symbolGapSquared - ydiff * ydiff));
            if (2 * xoffs < System.Math.Abs(linepts[j + 2].X - linepts[j + 1].X))
            {
              xoffs = (linepts[j + 2].X > linepts[j + 1].X) ? xoffs : -xoffs;
              gp.AddLine(linepts[j + 1].X + xoffs, linepts[j + 1].Y, linepts[j + 2].X - xoffs, linepts[j + 2].Y);
              gp.StartFigure();
            }
          }
        } // for loop
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepVertMiddleLineStyle

    public void StepHorzCenterConnection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      if (range.Length < 2)
        return;

      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      SizeF layerSize = layer.Size;
      PointF[] linepts = new PointF[range.Length * 3 - 2];
      int end = range.UpperBound - 1;
      int i, j;
      for (i = 0, j = range.LowerBound; j < end; i += 3, j++)
      {
        linepts[i] = linePoints[j];
        linepts[i + 1].Y = linePoints[j].Y;
        linepts[i + 1].X = 0.5f * (linePoints[j].X + linePoints[j + 1].X);
        linepts[i + 2].Y = linePoints[j + 1].Y;
        linepts[i + 2].X = linepts[i + 1].X;
      }
      linepts[i] = linePoints[j];
      int lastIdx = i;

      GraphicsPath gp = new GraphicsPath();

      if (_fillArea)
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
        gp.AddLines(linepts);
        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      if (this._useLineSymbolGap && symbolGap > 0)
      {
        end = linepts.Length - 1;
        float symbolGapSquared = symbolGap * symbolGap;
        for (j = 0; j < end; j += 3)
        {
          float xdiff = linepts[j + 1].X - linepts[j].X;
          if (System.Math.Abs(xdiff) > symbolGap) // then the two horz lines are visible, and full visible vert line
          {
            gp.AddLine(linepts[j].X + (xdiff > 0 ? symbolGap : -symbolGap), linepts[j].Y, linepts[j + 1].X, linepts[j + 1].Y);
            gp.AddLine(linepts[j + 1].X, linepts[j + 1].Y, linepts[j + 2].X, linepts[j + 2].Y);
            gp.AddLine(linepts[j + 2].X, linepts[j + 2].Y, linepts[j + 3].X - (xdiff > 0 ? symbolGap : -symbolGap), linepts[j + 3].Y);
            gp.StartFigure();
          }
          else // no horizontal lines visible, and vertical line may be shortened
          {
            // Calculate, how much of the horz line is invisible on both ends
            float yoffs = (float)(System.Math.Sqrt(symbolGapSquared - xdiff * xdiff));
            if (2 * yoffs < System.Math.Abs(linepts[j + 2].Y - linepts[j + 1].Y))
            {
              yoffs = (linepts[j + 2].Y > linepts[j + 1].Y) ? yoffs : -yoffs;
              gp.AddLine(linepts[j + 1].X, linepts[j + 1].Y + yoffs, linepts[j + 2].X, linepts[j + 2].Y - yoffs);
              gp.StartFigure();
            }
          }
        } // for loop
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepHorzMiddleLineStyle


    public void Segment2Connection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      SizeF layerSize = layer.Size;
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
      int lastIdx = range.Length - 1;
      GraphicsPath gp = new GraphicsPath();
      int i;

      if (_fillArea)
      {
            int offs = range.LowerBound;
            for (i = 0; i < lastIdx; i += 2)
            {
              Logical3D r0 = layer.GetLogical3D(pdata, i + range.OriginalFirstPoint);
              layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
              gp.AddLine(linepts[i].X, linepts[i].Y, linepts[i+1].X, linepts[i+1].Y);
              Logical3D r1 = layer.GetLogical3D(pdata, i + 1 + range.OriginalFirstPoint);
              layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
              layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);
              gp.StartFigure();
            }

        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      if (this._useLineSymbolGap && symbolGap > 0)
      {
        float xdiff, ydiff, rel, startx, starty, stopx, stopy;
        for (i = 0; i < lastIdx; i += 2)
        {
          xdiff = linepts[i + 1].X - linepts[i].X;
          ydiff = linepts[i + 1].Y - linepts[i].Y;
          rel = (float)(symbolGap / System.Math.Sqrt(xdiff * xdiff + ydiff * ydiff));
          if (rel < 0.5) // a line only appears if the relative gap is smaller 1/2
          {
            startx = linepts[i].X + rel * xdiff;
            starty = linepts[i].Y + rel * ydiff;
            stopx = linepts[i + 1].X - rel * xdiff;
            stopy = linepts[i + 1].Y - rel * ydiff;

            gp.AddLine(startx, starty, stopx, stopy);
            gp.StartFigure();
          }
        } // end for
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        for (i = 0; i < lastIdx; i += 2)
        {
          gp.AddLine(linepts[i].X, linepts[i].Y, linepts[i + 1].X, linepts[i + 1].Y);
          gp.StartFigure();
        } // end for
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
    } // end function PaintOneRange Segment2LineStyle


    public void Segment3Connection_PaintOneRange(
      Graphics g,
      Processed2DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      float symbolGap)
    {
      PointF[] linePoints = pdata.PlotPointsInAbsoluteLayerCoordinates;
      SizeF layerSize = layer.Size;
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
      int lastIdx;
      GraphicsPath gp = new GraphicsPath();
      int i;

      if (_fillArea)
      {
        lastIdx = range.Length - 2;
        int offs = range.LowerBound;
            for (i = 0; i < lastIdx; i += 3)
            {
              Logical3D r0 = layer.GetLogical3D(pdata, i + range.OriginalFirstPoint);
              layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, _fillDirection, r0);
              gp.AddLine(linepts[i].X, linepts[i].Y, linepts[i + 1].X, linepts[i + 1].Y);
              gp.AddLine(linepts[i+1].X, linepts[i+1].Y, linepts[i + 2].X, linepts[i + 2].Y);

              Logical3D r1 = layer.GetLogical3D(pdata, i + 2 + range.OriginalFirstPoint);
              layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, _fillDirection);
              layer.CoordinateSystem.GetIsolineOnPlane(gp, _fillDirection, r1, r0);
              gp.StartFigure();
            }
        gp.CloseFigure();
        g.FillPath(this._fillBrush, gp);
        gp.Reset();
      }

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      lastIdx = range.Length - 1;

      if (this._useLineSymbolGap && symbolGap > 0)
      {
        float xdiff, ydiff, rel, startx, starty, stopx, stopy;
        for (i = 0; i < lastIdx; i++)
        {
          if (2 != (i % 3))
          {
            xdiff = linepts[i + 1].X - linepts[i].X;
            ydiff = linepts[i + 1].Y - linepts[i].Y;
            rel = (float)(symbolGap / System.Math.Sqrt(xdiff * xdiff + ydiff * ydiff));
            if (rel < 0.5) // a line only appears if the relative gap is smaller 1/2
            {
              startx = linepts[i].X + rel * xdiff;
              starty = linepts[i].Y + rel * ydiff;
              stopx = linepts[i + 1].X - rel * xdiff;
              stopy = linepts[i + 1].Y - rel * ydiff;

              gp.AddLine(startx, starty, stopx, stopy);
              gp.StartFigure();
            }
          }
        } // end for
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        for (i = 0; i < lastIdx; i += 3)
        {
          gp.AddLine(linepts[i].X, linepts[i].Y, linepts[i + 1].X, linepts[i + 1].Y);
          gp.AddLine(linepts[i + 1].X, linepts[i + 1].Y, linepts[i + 2].X, linepts[i + 2].Y);
          gp.StartFigure();
        } // end for
        g.DrawPath(this._penHolder, gp);
        gp.Reset();
      }
    } // end function PaintOneRange Segment3LineStyle
    #region IChangedEventSource Members

   

    protected virtual void OnChanged()
    {
      if (_parentObject is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parentObject).EhChildChanged(this, EventArgs.Empty);

      if (null != Changed)
        Changed(this, new EventArgs());
    }

    public virtual void EhChildChanged(object child, EventArgs e)
    {
      if (null != Changed)
        Changed(this, e);
    }

    #endregion

  
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
        return this._penHolder.Color;
      }
      set
      {
        this._penHolder.Color = value;
      }
    }


    public bool IsColorReceiver
    {
      get { return !this._independentColor; }
    }

    /// <summary>
    /// Sets the fill brush color from a color, but leaves the opacity value unchanged.
    /// </summary>
    /// <param name="c">The color to set. Only the RGB values are used from here; the A value (opacity) is ignored and remains unchanged.</param>
    public void SetFillColorRGB(Color c)
    {
      _fillBrush.Color = Color.FromArgb(_fillBrush.Color.A, c.R, c.G, c.B);
    }
  

    float SymbolSize
    {
      set
      {
        this._symbolGap = value;
      }
    }

   

  




    #region IG2DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {

    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
      LineStyleGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      if (this.IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate() { return PlotColors.Colors.GetPlotColor(this.Color); });

      LineStyleGroupStyle.PrepareStyle(externalGroups, localGroups, delegate { return this.PenHolder.DashStyle; });
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if (this.IsColorReceiver)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(PlotColor c) { this.Color = c; });

      if (this._fillArea && !this._independentFillColor)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(PlotColor c) { this.SetFillColorRGB(c.Color); });

      LineStyleGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(DashStyle c) { this.PenHolder.DashStyle = c; });

      if (!SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(float size) { this._symbolGap = size; }))
      {
        this._symbolGap = 0;
      }
    }

   

    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parentObject; }
      set { _parentObject = (Main.IDocumentNode)value; }
    }

    public string Name
    {
      get { return this.GetType().Name; }
    }

    #endregion
  } // end class XYPlotLineStyle
}
