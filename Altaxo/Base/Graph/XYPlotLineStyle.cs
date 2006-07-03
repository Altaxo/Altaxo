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
//using System.Runtime.InteropServices;


namespace Altaxo.Graph
{
  /// <summary>
  /// PlotRange represents a range of plotting points from index 
  /// lowerBound to (upperBound-1)
  /// I use a class instead of a struct because it is intended to use with
  /// <see cref="System.Collections.ArrayList" />.
  /// </summary>
  /// <remarks>For use in a list, the UpperBound property is somewhat useless, since it should be equal
  /// to the LowerBound property of the next item.</remarks>
  [Serializable]
  public class PlotRange
  {
    int m_LowerBound;
    int m_UpperBound;
    int m_OffsetToOriginal;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotRange),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PlotRange s = (PlotRange)obj;
        info.AddValue("LowerBound",s.m_LowerBound);  
        info.AddValue("UpperBound",s.m_UpperBound);  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        PlotRange s = null!=o ? (PlotRange)o : new PlotRange(0,0);
        
        s.m_LowerBound = info.GetInt32("LowerBound");
        s.m_UpperBound = info.GetInt32("UpperBound");

        return s;
      }
    }
    #endregion



    public PlotRange(int lower, int upper)
    {
      m_LowerBound = lower;
      m_UpperBound = upper;
      m_OffsetToOriginal = 0;
    }

    public PlotRange(int lower, int upper, int offset)
    {
      m_LowerBound = lower;
      m_UpperBound = upper;
      m_OffsetToOriginal = offset;
    }

    public PlotRange(PlotRange a)
    {
      m_LowerBound = a.m_LowerBound;
      m_UpperBound = a.m_UpperBound;
      m_OffsetToOriginal = a.m_OffsetToOriginal;
    }

    /// <summary>
    /// Number of points in this plot range.
    /// </summary>
    public int Length
    {
      get { return m_UpperBound - m_LowerBound; }
    }

    /// <summary>
    /// First index in the plot point array, that appears as a plot range.
    /// </summary>
    public int LowerBound
    {
      get { return m_LowerBound; }
      set { m_LowerBound = value; }
    }

    /// <summary>
    /// One more than the last index in the plot poin array, that appeas as a plot range.
    /// </summary>
    public int UpperBound
    {
      get { return m_UpperBound; }
      set { m_UpperBound = value; }
    }

    /// <summary>
    /// This gives the offset to the original row index.
    /// </summary>
    /// <example>If the LowerBound=4 and UpperBound==6, this means points 4 to 6 in the array of plot point locations (!)
    /// will be a contiguous range of plot points, and for this, they are connected by a line etc.
    /// If OffsetToOriginal is 5, this means the point at index 4 in the plot point location array was created
    /// from row index 4+5=9, i.e. from row index 9 in the DataColumn.</example>
    public int OffsetToOriginal
    {
      get { return m_OffsetToOriginal; }
      set { m_OffsetToOriginal = value; }
    }


  }

  /// <summary>
  /// Holds a list of plot ranges. The list is not sorted automatically, but is assumed to be sorted.
  /// </summary>
  [Serializable]
  public class PlotRangeList : Altaxo.Data.CollectionBase
  {
    /// <summary>
    /// Getter to a plot range at index i.
    /// </summary>
    public PlotRange this[int i]
    {
      get { return (PlotRange)InnerList[i]; }
    }

    /// <summary>
    /// Adds a plot range. This plot range should be above the previous added plot range.
    /// </summary>
    /// <param name="a"></param>
    public void Add(PlotRange a)
    {
      InnerList.Add(a);
    }


    /// <summary>
    /// This will get the row index into the data row belonging to a given plot index.
    /// </summary>
    /// <param name="idx">Index of point in the plot array.</param>
    /// <returns>Index into the original data.</returns>
    /// <remarks>Returns -1 if the point is not found.</remarks>
    public int GetRowIndexForPlotIndex(int idx)
    {
      for(int i=0;i<Count;i++)
      {
        if(this[i].LowerBound<=idx && idx<this[i].UpperBound)
          return idx+this[i].OffsetToOriginal;
      }
      return -1;
    }
  }


  namespace XYPlotLineStyles
  {
    [Serializable]
    public enum FillDirection
    {
      Left=0,
      Bottom=1,
      Right=2,
      Top=3
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FillDirection),0)]
    public class FillDirectionXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString()); 
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(FillDirection),val,true);
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConnectionStyle),0)]
    public class ConnectionStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(ConnectionStyle),val,true);
      }
    }

  }


  /// <summary>
  /// Summary description for XYPlotLineStyle.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYPlotLineStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYPlotLineStyle
    :
    ICloneable,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    System.Runtime.Serialization.IDeserializationCallback, 
    I2DPlotStyle
  {
    public delegate void PaintOneRangeTemplate(
      Graphics g, 
      PointF[] linepts, 
      PlotRange range, 
      SizeF layerSize, 
      float symbolGap);


   
    protected PenHolder       _penHolder;
    protected XYPlotLineStyles.ConnectionStyle _connectionStyle;
    protected bool            _useLineSymbolGap;
    protected float _symbolGap;
    protected bool            _ignoreMissingPoints; // treat missing points as if not present (connect lines over missing points) 
    protected bool            _fillArea;
    protected BrushHolder     _fillBrush; // brush to fill the area under the line
    protected XYPlotLineStyles.FillDirection _fillDirection; // the direction to fill
    protected bool            _independentColor;
   

    // cached values
    protected PaintOneRangeTemplate _cachedPaintOneRange; // subroutine to paint a single range

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
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        XYPlotLineStyle s = (XYPlotLineStyle)obj;
        info.AddValue("Pen",s._penHolder);  
        info.AddValue("Connection",s._connectionStyle);
        info.AddValue("LineSymbolGap",s._useLineSymbolGap);
        info.AddValue("IgnoreMissingPoints",s._ignoreMissingPoints);
        info.AddValue("FillArea",s._fillArea);
        info.AddValue("FillBrush",s._fillBrush);
        info.AddValue("FillDirection",s._fillDirection);
      }
      /// <summary>
      /// Deserializes the XYPlotLineStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty XYPlotLineStyle object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYPlotLineStyle.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYPlotLineStyle s = (XYPlotLineStyle)obj;

        s._penHolder = (PenHolder)info.GetValue("Pen",typeof(PenHolder));  
        s.Connection = (XYPlotLineStyles.ConnectionStyle)info.GetValue("Connection",typeof(XYPlotLineStyles.ConnectionStyle));
        s._useLineSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingPoints = info.GetBoolean("IgnoreMissingPoints");
        s._fillArea = info.GetBoolean("FillArea");
        s._fillBrush = (BrushHolder)info.GetValue("FillBrush",typeof(BrushHolder));
        s._fillDirection = (XYPlotLineStyles.FillDirection)info.GetValue("FillDirection",typeof(XYPlotLineStyles.FillDirection));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLineStyle),0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLineStyle),1)] // by accident, it was never different from 0
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLineStyle s = (XYPlotLineStyle)obj;
        info.AddValue("Pen",s._penHolder);  
        info.AddValue("Connection",s._connectionStyle);
        info.AddValue("LineSymbolGap",s._useLineSymbolGap);
        info.AddValue("IgnoreMissingPoints",s._ignoreMissingPoints);
        info.AddValue("FillArea",s._fillArea);
        info.AddValue("FillBrush",s._fillBrush);
        info.AddValue("FillDirection",s._fillDirection);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLineStyle s = SDeserialize(o, info, parent);
        s.CreateEventChain();
        return s;          
      }

      public virtual XYPlotLineStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotLineStyle s = null!=o ? (XYPlotLineStyle)o : new XYPlotLineStyle();

        s._penHolder = (PenHolder)info.GetValue("Pen",typeof(PenHolder));  
        s.Connection = (XYPlotLineStyles.ConnectionStyle)info.GetValue("Connection",typeof(XYPlotLineStyles.ConnectionStyle));
        s._useLineSymbolGap = info.GetBoolean("LineSymbolGap");
        s._ignoreMissingPoints = info.GetBoolean("IgnoreMissingPoints");
        s._fillArea = info.GetBoolean("FillArea");
        s._fillBrush = (BrushHolder)info.GetValue("FillBrush",typeof(BrushHolder));
        s._fillDirection = (XYPlotLineStyles.FillDirection)info.GetValue("FillDirection",typeof(XYPlotLineStyles.FillDirection));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLineStyle),2)]
      public class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj,info);
        XYPlotLineStyle s = (XYPlotLineStyle)obj;
        info.AddValue("IndependentColor",s._independentColor);
        
      }
      public override XYPlotLineStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLineStyle s =  base.SDeserialize(o,info,parent);
        s._independentColor = info.GetBoolean("IndependentColor");
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



    public XYPlotLineStyle()
    {
      _penHolder = new PenHolder(Color.Black);
      _useLineSymbolGap = true;
      _ignoreMissingPoints=false;
      _fillArea = false;
      _fillBrush = new BrushHolder(Color.Black);
      _fillDirection = XYPlotLineStyles.FillDirection.Bottom;
      _connectionStyle    = XYPlotLineStyles.ConnectionStyle.Straight;
      _cachedPaintOneRange = new PaintOneRangeTemplate(StraightConnection_PaintOneRange);
      _independentColor = false;
     
      CreateEventChain();
    }


    public void CopyFrom(XYPlotLineStyle from, bool suppressChangeEvent)
    {
      this._penHolder                  = null==from._penHolder?null:(PenHolder)from._penHolder.Clone();
      this._useLineSymbolGap       = from._useLineSymbolGap;
      this._symbolGap = from._symbolGap;
      this._ignoreMissingPoints = from._ignoreMissingPoints;
      this._fillArea            = from._fillArea;
      this._fillBrush            = null==from._fillBrush?null:(BrushHolder)from._fillBrush.Clone();
      this._fillDirection        = from._fillDirection;
      this.Connection             = from._connectionStyle; // beachte links nur Connection, damit das Template mit gesetzt wird
      this._independentColor      = from._independentColor;
      

      if(!suppressChangeEvent)
        OnChanged();
    }

    public XYPlotLineStyle(XYPlotLineStyle from)
    {
     
      CopyFrom(from,true);
      CreateEventChain();
    }

    protected virtual void CreateEventChain()
    {
      if(null!=_penHolder)
        _penHolder.Changed += new EventHandler(this.EhChildChanged);
      
      if(null!=_fillBrush)
        _fillBrush.Changed += new EventHandler(this.EhChildChanged);
    }

    public XYPlotLineStyles.ConnectionStyle Connection
    {
      get { return _connectionStyle; }
      set 
      {
        _connectionStyle = value;
        switch(_connectionStyle)
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
        if(value!=oldValue)
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
        if(value!=oldValue)
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

      int len =  System.Enum.GetValues(typeof(DashStyle)).Length;
      int next = step+(int)template;
      this.PenHolder.DashStyle = (DashStyle)Calc.BasicFunctions.PMod(next,len-1);

      OnChanged(); // Fire Changed event
    }

    public PenHolder PenHolder
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
        if(true==value && null==this._fillBrush)
          this._fillBrush = new BrushHolder(Color.White);

        OnChanged(); // Fire Changed event
      }
    }
 
    public XYPlotLineStyles.FillDirection FillDirection
    {
      get { return this._fillDirection; }
      set 
      {
        this._fillDirection = value; 
        OnChanged(); // Fire Changed event
      }
    }

    public BrushHolder FillBrush
    {
      get { return this._fillBrush; }
      set 
      {
        // copy the brush only if not null
        if(null!=value)
        {
          this._fillBrush = (BrushHolder)value.Clone();
          this._fillBrush.Changed += new EventHandler(this.EhChildChanged);
          OnChanged(); // Fire Changed event
        }
        else
          throw new ArgumentNullException("FillBrush","FillBrush must not be set to null, instead set FillArea to false");
      }
    }

    public bool IsVisible
    {
      get
      {
        if (_connectionStyle!= Altaxo.Graph.XYPlotLineStyles.ConnectionStyle.NoLine)
          return true;
        if (_fillArea)
          return true;

        return false;
      }
    }

    public object Clone()
    {
      return new XYPlotLineStyle(this);
    }

    public virtual void PaintLine(Graphics g, PointF beg, PointF end)
    {
      if(null!=_penHolder)
      {
        g.DrawLine(_penHolder,beg,end);
      }
    }



    public virtual void Paint(
      Graphics g, 
      PointF[] linePoints,
      PlotRangeList rangeList,
      IPlotArea layer, // Size of layer in Points (1/72 inch), the upper left corner of layer is assumed to have coordinates (0,0)
      float symbolGap // symbol gap in worlds coordinates, i.e. in Points (1/72 inch)
      )
    {
      
      // ensure that brush and pen are cached
      if(null!=_penHolder) _penHolder.Cached=true;
     

      int rangelistlen = rangeList.Count;

      double xleft,xright,ytop,ybottom;
      layer.LogicalToAreaConversion.Convert(0,0,out xleft, out ybottom);
      layer.LogicalToAreaConversion.Convert(1,1,out xright, out ytop);
      SizeF layerSize = new SizeF((float)Math.Abs(xright-xleft),(float)Math.Abs(ybottom-ytop));

      if(this._ignoreMissingPoints)
      {
        // in case we ignore the missing points, all ranges can be plotted
        // as one range, i.e. continuously
        // for this, we create the totalRange, which contains all ranges
        PlotRange totalRange = new PlotRange(rangeList[0].LowerBound,rangeList[rangelistlen-1].UpperBound);
        _cachedPaintOneRange(g,linePoints,totalRange,layerSize,symbolGap);
      }
      else // we not ignore missing points, so plot all ranges separately
      {
        for(int i=0;i<rangelistlen;i++)
        {
          _cachedPaintOneRange(g,linePoints,rangeList[i],layerSize,symbolGap);
        }
      }
    }


    public void NoConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
    }

    public void StraightConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints,range.LowerBound,linepts,0,range.Length); // Extract
      int lastIdx = range.Length-1;
      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        /*
        double xleft,xright,ytop,ybottom;
        layer.LogicalToAreaConversion.Convert(0,0,out xleft, out ybottom);
        layer.LogicalToAreaConversion.Convert(1,1,out xright, out ytop);
        SizeF layerSize = new SizeF(Math.Abs(xright-xleft),Math.Abs(ybottom-ytop));
        */
        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[0].X,layerSize.Height,linepts[0].X,linepts[0].Y);
            gp.AddLines(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,linepts[lastIdx].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[0].X,0,linepts[0].X,linepts[0].Y);
            gp.AddLines(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,linepts[lastIdx].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[0].Y,linepts[0].X,linepts[0].Y);
            gp.AddLines(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,0,linepts[lastIdx].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[0].Y,linepts[0].X,linepts[0].Y);
            gp.AddLines(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,layerSize.Width,linepts[lastIdx].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      if(this._useLineSymbolGap && symbolGap>0)
      {
        float xdiff,ydiff,rel,startx, starty, stopx, stopy;
        for(int i=0;i<lastIdx;i++)
        {
          xdiff = linepts[i+1].X - linepts[i].X;
          ydiff = linepts[i+1].Y - linepts[i].Y;
          rel = (float)( symbolGap / System.Math.Sqrt(xdiff*xdiff+ydiff*ydiff));
          if(rel<0.5) // a line only appears if the relative gap is smaller 1/2
          {
            startx = linepts[i].X + rel*xdiff;
            starty = linepts[i].Y + rel*ydiff;
            stopx  = linepts[i+1].X - rel*xdiff;
            stopy  = linepts[i+1].Y - rel*ydiff;
          
            gp.AddLine(startx, starty, stopx, stopy);
            gp.StartFigure();
          }
        } // end for
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        if(linepts.Length>1) // we don't want to have a drawing exception if number of points is only one
        {
          g.DrawLines(this._penHolder, linepts);
        }
      }
    } // end function PaintOneRange


    //[DllImport("gdiplus.dll")]
    //protected static extern int GdipDrawLines(System.IntPtr g, System.IntPtr p, PointF[] points, int count);


    public void SplineConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints,range.LowerBound,linepts,0,range.Length); // Extract
      int lastIdx = range.Length-1;
      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[0].X,layerSize.Height,linepts[0].X,linepts[0].Y);
            gp.AddCurve(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,linepts[lastIdx].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[0].X,0,linepts[0].X,linepts[0].Y);
            gp.AddCurve(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,linepts[lastIdx].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[0].Y,linepts[0].X,linepts[0].Y);
            gp.AddCurve(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,0,linepts[lastIdx].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[0].Y,linepts[0].X,linepts[0].Y);
            gp.AddCurve(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,layerSize.Width,linepts[lastIdx].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      // unfortuately, there is no easy way to support line/symbol gaps
      // thats why I ignore this value and draw a curve through the points
      g.DrawCurve(this._penHolder, linepts);

    } // end function PaintOneRange (Spline)


    public void BezierConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange rangeRaw, 
      SizeF layerSize,
      float symbolGap)
    {
      // Bezier is only supported with point numbers n=4+3*k
      // so trim the range appropriately
      PlotRange range = new PlotRange(rangeRaw);
      
      range.UpperBound = range.LowerBound + 3*((range.Length+2)/3)-2;
      if(range.Length<4)
        return; // then to less points are in this range

      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints,range.LowerBound,linepts,0,range.Length); // Extract
      int lastIdx = range.Length-1;
      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[0].X,layerSize.Height,linepts[0].X,linepts[0].Y);
            gp.AddBeziers(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,linepts[lastIdx].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[0].X,0,linepts[0].X,linepts[0].Y);
            gp.AddBeziers(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,linepts[lastIdx].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[0].Y,linepts[0].X,linepts[0].Y);
            gp.AddBeziers(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,0,linepts[lastIdx].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[0].Y,linepts[0].X,linepts[0].Y);
            gp.AddBeziers(linepts);
            gp.AddLine(linepts[lastIdx].X,linepts[lastIdx].Y,layerSize.Width,linepts[lastIdx].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      // unfortuately, there is no easy way to support line/symbol gaps
      // thats why I ignore this value and draw a curve through the points
      g.DrawBeziers(this._penHolder, linepts);

    } // end function PaintOneRange BezierLineStyle


    public void StepHorzConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      if(range.Length<2)
        return;

      PointF[] linepts = new PointF[range.Length*2-1];
      int end = range.UpperBound-1;
      int i,j;
      for(i=0,j=range.LowerBound;j<end;i+=2,j++)
      {
        linepts[i]=linePoints[j];
        linepts[i+1].X = linePoints[j+1].X;
        linepts[i+1].Y = linePoints[j].Y;
      }
      linepts[i] = linePoints[j];
      int lastIdx=i;

      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        gp.AddLines(linepts);

        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[lastIdx].X,layerSize.Height,linepts[0].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[lastIdx].X,0,linepts[0].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[lastIdx].Y,0,linepts[0].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[lastIdx].Y,layerSize.Width,linepts[0].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      if(this._useLineSymbolGap && symbolGap>0)
      {
        end = range.UpperBound-1;
        float symbolGapSquared = symbolGap*symbolGap;
        for(j=range.LowerBound;j<end;j++)
        {
          float xmiddle = linePoints[j+1].X;
          float ymiddle = linePoints[j].Y;

          // decide if horz line is necessary
          float xdiff = System.Math.Abs(linePoints[j+1].X - linePoints[j].X);
          float ydiff = System.Math.Abs(linePoints[j+1].Y - linePoints[j].Y);

          float xrel1 = symbolGap/xdiff;
          float xrel2 = ydiff>symbolGap ? 1 : (float)(1-System.Math.Sqrt(symbolGapSquared-ydiff*ydiff)/xdiff); 

          float yrel1 = xdiff>symbolGap ? 0 : (float)(System.Math.Sqrt(symbolGapSquared-xdiff*xdiff)/ydiff);
          float yrel2 = 1-symbolGap/ydiff;

          xdiff = linePoints[j+1].X - linePoints[j].X;
          ydiff = linePoints[j+1].Y - linePoints[j].Y;

          if(xrel1<xrel2)
            gp.AddLine(linePoints[j].X+xrel1*xdiff,linePoints[j].Y,linePoints[j].X+xrel2*xdiff,linePoints[j].Y);

          if(yrel1<yrel2)
            gp.AddLine(linePoints[j+1].X,linePoints[j].Y+yrel1*ydiff,linePoints[j+1].X,linePoints[j].Y+yrel2*ydiff);
        
          if(xrel1<xrel2 || yrel1<yrel2)
            gp.StartFigure();
        }
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepHorzLineStyle

    public  void StepVertConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      if(range.Length<2)
        return;

      PointF[] linepts = new PointF[range.Length*2-1];
      int end = range.UpperBound-1;
      int i,j;
      for(i=0,j=range.LowerBound;j<end;i+=2,j++)
      {
        linepts[i]=linePoints[j];
        linepts[i+1].X = linePoints[j].X;
        linepts[i+1].Y = linePoints[j+1].Y;
      }
      linepts[i] = linePoints[j];
      int lastIdx=i;

      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        gp.AddLines(linepts);

        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[lastIdx].X,layerSize.Height,linepts[0].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[lastIdx].X,0,linepts[0].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[lastIdx].Y,0,linepts[0].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[lastIdx].Y,layerSize.Width,linepts[0].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      if(this._useLineSymbolGap && symbolGap>0)
      {
        end = range.UpperBound-1;
        float symbolGapSquared = symbolGap*symbolGap;
        for(j=range.LowerBound;j<end;j++)
        {
          float xmiddle = linePoints[j+1].X;
          float ymiddle = linePoints[j].Y;

          // decide if horz line is necessary
          float xdiff = System.Math.Abs(linePoints[j+1].X - linePoints[j].X);
          float ydiff = System.Math.Abs(linePoints[j+1].Y - linePoints[j].Y);

          float yrel1 = symbolGap/ydiff;
          float yrel2 = xdiff>symbolGap ? 1 : (float)(1-System.Math.Sqrt(symbolGapSquared-xdiff*xdiff)/ydiff); 

          float xrel1 = ydiff>symbolGap ? 0 : (float)(System.Math.Sqrt(symbolGapSquared-ydiff*ydiff)/xdiff);
          float xrel2 = 1-symbolGap/xdiff;

          xdiff = linePoints[j+1].X - linePoints[j].X;
          ydiff = linePoints[j+1].Y - linePoints[j].Y;

          if(yrel1<yrel2)
            gp.AddLine(linePoints[j].X,linePoints[j].Y+yrel1*ydiff,linePoints[j].X,linePoints[j].Y+yrel2*ydiff);

          if(xrel1<xrel2)
            gp.AddLine(linePoints[j].X+xrel1*ydiff,linePoints[j+1].Y,linePoints[j].X+xrel2*xdiff,linePoints[j+1].Y);
        
          if(xrel1<xrel2 || yrel1<yrel2)
            gp.StartFigure();
        }
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepVertLineStyle

    public void StepVertCenterConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      if(range.Length<2)
        return;

      PointF[] linepts = new PointF[range.Length*3-2];
      int end = range.UpperBound-1;
      int i,j;
      for(i=0,j=range.LowerBound;j<end;i+=3,j++)
      {
        linepts[i]=linePoints[j];
        linepts[i+1].X = linePoints[j].X;
        linepts[i+1].Y = 0.5f*(linePoints[j].Y+linePoints[j+1].Y);
        linepts[i+2].X = linePoints[j+1].X;
        linepts[i+2].Y = linepts[i+1].Y;
      }
      linepts[i] = linePoints[j];
      int lastIdx=i;

      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        gp.AddLines(linepts);

        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[lastIdx].X,layerSize.Height,linepts[0].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[lastIdx].X,0,linepts[0].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[lastIdx].Y,0,linepts[0].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[lastIdx].Y,layerSize.Width,linepts[0].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      if(this._useLineSymbolGap && symbolGap>0)
      {
        end = linepts.Length-1;
        float symbolGapSquared = symbolGap*symbolGap;
        for(j=0;j<end;j+=3)
        {
          float ydiff = linepts[j+1].Y-linepts[j].Y;
          if(System.Math.Abs(ydiff)>symbolGap) // then the two vertical lines are visible, and full visible horz line
          {
            gp.AddLine(linepts[j].X,linepts[j].Y + (ydiff>0 ? symbolGap : -symbolGap),linepts[j+1].X,linepts[j+1].Y);
            gp.AddLine(linepts[j+1].X,linepts[j+1].Y,linepts[j+2].X,linepts[j+2].Y);
            gp.AddLine(linepts[j+2].X,linepts[j+2].Y,linepts[j+3].X,linepts[j+3].Y - (ydiff>0 ? symbolGap : -symbolGap));
            gp.StartFigure();
          }
          else // no vertical lines visible, and horz line can be shortened
          {
            // Calculate, how much of the horz line is invisible on both ends
            float xoffs=(float)(System.Math.Sqrt(symbolGapSquared - ydiff*ydiff));
            if( 2*xoffs < System.Math.Abs(linepts[j+2].X - linepts[j+1].X))
            {
              xoffs = (linepts[j+2].X>linepts[j+1].X) ? xoffs : -xoffs; 
              gp.AddLine(linepts[j+1].X+xoffs,linepts[j+1].Y,linepts[j+2].X-xoffs,linepts[j+2].Y);
              gp.StartFigure();
            }
          }
        } // for loop
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepVertMiddleLineStyle

    public void StepHorzCenterConnection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      if(range.Length<2)
        return;

      PointF[] linepts = new PointF[range.Length*3-2];
      int end = range.UpperBound-1;
      int i,j;
      for(i=0,j=range.LowerBound;j<end;i+=3,j++)
      {
        linepts[i]=linePoints[j];
        linepts[i+1].Y = linePoints[j].Y;
        linepts[i+1].X = 0.5f*(linePoints[j].X+linePoints[j+1].X);
        linepts[i+2].Y = linePoints[j+1].Y;
        linepts[i+2].X = linepts[i+1].X;
      }
      linepts[i] = linePoints[j];
      int lastIdx=i;

      GraphicsPath gp = new GraphicsPath();

      if(_fillArea)
      {
        gp.AddLines(linepts);

        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            // create a graphics path with the linePoints
            gp.AddLine(linepts[lastIdx].X,layerSize.Height,linepts[0].X,layerSize.Height);
            break;
          case XYPlotLineStyles.FillDirection.Top:
            gp.AddLine(linepts[lastIdx].X,0,linepts[0].X,0);
            break;
          case XYPlotLineStyles.FillDirection.Left:
            gp.AddLine(0,linepts[lastIdx].Y,0,linepts[0].Y);
            break;
          case XYPlotLineStyles.FillDirection.Right:
            gp.AddLine(layerSize.Width,linepts[lastIdx].Y,layerSize.Width,linepts[0].Y);
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      if(this._useLineSymbolGap && symbolGap>0)
      {
        end = linepts.Length-1;
        float symbolGapSquared = symbolGap*symbolGap;
        for(j=0;j<end;j+=3)
        {
          float xdiff = linepts[j+1].X-linepts[j].X;
          if(System.Math.Abs(xdiff)>symbolGap) // then the two horz lines are visible, and full visible vert line
          {
            gp.AddLine(linepts[j].X + (xdiff>0 ? symbolGap : -symbolGap),linepts[j].Y,linepts[j+1].X,linepts[j+1].Y);
            gp.AddLine(linepts[j+1].X,linepts[j+1].Y,linepts[j+2].X,linepts[j+2].Y);
            gp.AddLine(linepts[j+2].X,linepts[j+2].Y,linepts[j+3].X - (xdiff>0 ? symbolGap : -symbolGap),linepts[j+3].Y);
            gp.StartFigure();
          }
          else // no horizontal lines visible, and vertical line may be shortened
          {
            // Calculate, how much of the horz line is invisible on both ends
            float yoffs=(float)(System.Math.Sqrt(symbolGapSquared - xdiff*xdiff));
            if( 2*yoffs < System.Math.Abs(linepts[j+2].Y - linepts[j+1].Y))
            {
              yoffs = (linepts[j+2].Y>linepts[j+1].Y) ? yoffs : -yoffs; 
              gp.AddLine(linepts[j+1].X,linepts[j+1].Y+yoffs,linepts[j+2].X,linepts[j+2].Y-yoffs);
              gp.StartFigure();
            }
          }
        } // for loop
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else
      {
        g.DrawLines(this._penHolder, linepts);
      }
    } // end function PaintOneRange StepHorzMiddleLineStyle

  
    public void Segment2Connection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints,range.LowerBound,linepts,0,range.Length); // Extract
      int lastIdx = range.Length-1;
      GraphicsPath gp = new GraphicsPath();
      int i;

      if(_fillArea)
      {
        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            for(i=0;i<lastIdx;i+=2)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(linepts[i+1].X,layerSize.Height,linepts[i].X,layerSize.Height);
              gp.StartFigure();
            }
            break;
          case XYPlotLineStyles.FillDirection.Top:
            for(i=0;i<lastIdx;i+=2)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(linepts[i+1].X,0,linepts[i].X,0);
              gp.StartFigure();
            }
            break;
          case XYPlotLineStyles.FillDirection.Left:
            for(i=0;i<lastIdx;i+=2)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(0,linepts[i+1].Y,0,linepts[i].Y);
              gp.StartFigure();
            }
            break;
          case XYPlotLineStyles.FillDirection.Right:
            for(i=0;i<lastIdx;i+=2)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(layerSize.Width,linepts[i+1].Y,layerSize.Width,linepts[i].Y);
              gp.StartFigure();
            }
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      if(this._useLineSymbolGap && symbolGap>0)
      {
        float xdiff,ydiff,rel,startx, starty, stopx, stopy;
        for(i=0;i<lastIdx;i+=2)
        {
          xdiff = linepts[i+1].X - linepts[i].X;
          ydiff = linepts[i+1].Y - linepts[i].Y;
          rel = (float)( symbolGap / System.Math.Sqrt(xdiff*xdiff+ydiff*ydiff));
          if(rel<0.5) // a line only appears if the relative gap is smaller 1/2
          {
            startx = linepts[i].X + rel*xdiff;
            starty = linepts[i].Y + rel*ydiff;
            stopx  = linepts[i+1].X - rel*xdiff;
            stopy  = linepts[i+1].Y - rel*ydiff;
          
            gp.AddLine(startx, starty, stopx, stopy);
            gp.StartFigure();
          }
        } // end for
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        for(i=0;i<lastIdx;i+=2)
        {
          gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
          gp.StartFigure();
        } // end for
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
    } // end function PaintOneRange Segment2LineStyle


    public void Segment3Connection_PaintOneRange(
      Graphics g,
      PointF[] linePoints,
      PlotRange range, 
      SizeF layerSize,
      float symbolGap)
    {
      PointF[] linepts = new PointF[range.Length];
      Array.Copy(linePoints,range.LowerBound,linepts,0,range.Length); // Extract
      int lastIdx;
      GraphicsPath gp = new GraphicsPath();
      int i;

      if(_fillArea)
      {
        lastIdx = range.Length-2;
        switch(this._fillDirection)
        {
          case XYPlotLineStyles.FillDirection.Bottom:
            for(i=0;i<lastIdx;i+=3)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(linepts[i+1].X,linepts[i+1].Y,linepts[i+2].X,linepts[i+2].Y);
              gp.AddLine(linepts[i+2].X,layerSize.Height,linepts[i].X,layerSize.Height);
              gp.StartFigure();
            }
            break;
          case XYPlotLineStyles.FillDirection.Top:
            for(i=0;i<lastIdx;i+=3)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(linepts[i+1].X,linepts[i+1].Y,linepts[i+2].X,linepts[i+2].Y);
              gp.AddLine(linepts[i+2].X,0,linepts[i].X,0);
              gp.StartFigure();
            }
            break;
          case XYPlotLineStyles.FillDirection.Left:
            for(i=0;i<lastIdx;i+=3)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(linepts[i+1].X,linepts[i+1].Y,linepts[i+2].X,linepts[i+2].Y);
              gp.AddLine(0,linepts[i+2].Y,0,linepts[i].Y);
              gp.StartFigure();
            }
            break;
          case XYPlotLineStyles.FillDirection.Right:
            for(i=0;i<lastIdx;i+=3)
            {
              gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
              gp.AddLine(linepts[i+1].X,linepts[i+1].Y,linepts[i+2].X,linepts[i+2].Y);
              gp.AddLine(layerSize.Width,linepts[i+2].Y,layerSize.Width,linepts[i].Y);
              gp.StartFigure();
            }
            break;
        }
        
        gp.CloseFigure();
        g.FillPath(this._fillBrush,gp);
        gp.Reset();
      }

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      lastIdx = range.Length-1;

      if(this._useLineSymbolGap && symbolGap>0)
      {
        float xdiff,ydiff,rel,startx, starty, stopx, stopy;
        for(i=0;i<lastIdx;i++)
        {
          if(2!=(i%3))
          {
            xdiff = linepts[i+1].X - linepts[i].X;
            ydiff = linepts[i+1].Y - linepts[i].Y;
            rel = (float)( symbolGap / System.Math.Sqrt(xdiff*xdiff+ydiff*ydiff));
            if(rel<0.5) // a line only appears if the relative gap is smaller 1/2
            {
              startx = linepts[i].X + rel*xdiff;
              starty = linepts[i].Y + rel*ydiff;
              stopx  = linepts[i+1].X - rel*xdiff;
              stopy  = linepts[i+1].Y - rel*ydiff;
          
              gp.AddLine(startx, starty, stopx, stopy);
              gp.StartFigure();
            }
          }
        } // end for
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        for(i=0;i<lastIdx;i+=3)
        {
          gp.AddLine(linepts[i].X,linepts[i].Y,linepts[i+1].X,linepts[i+1].Y);
          gp.AddLine(linepts[i+1].X,linepts[i+1].Y,linepts[i+2].X,linepts[i+2].Y);
          gp.StartFigure();
        } // end for
        g.DrawPath(this._penHolder,gp);
        gp.Reset();
      }
    } // end function PaintOneRange Segment3LineStyle
    #region IChangedEventSource Members

    public event System.EventHandler Changed;
    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new EventArgs());
    }

    public virtual void EhChildChanged(object child, EventArgs e)
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

    public bool IsXYLineStyleSupported
    {
      get
      {
        return true;
      }
    }

    public Altaxo.Graph.XYPlotLineStyle XYLineStyle
    {
      get
      {
        return this;
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

    public void SetIncrementalStyle(I2DGroupablePlotStyle pstemplate, Altaxo.Graph.PlotGroupStyle style, int step)
    {
      
        
      if((0!= (style & PlotGroupStyle.Line)) && pstemplate.IsXYLineStyleSupported)
        this.SetToNextLineStyle(pstemplate.XYLineStyle,step);
     
      
      // Color has to be the last, since during the previous operations the styles are cloned, 
      // inclusive the color
      if((0!= (style & PlotGroupStyle.Color)) && pstemplate.IsColorSupported)
        this._penHolder.Color = PlotColors.Colors.GetNextPlotColor(pstemplate.Color, step);
    }

    #endregion

    #region I2DPlotItem Members

    public bool IsColorReceiver
    {
      get { return !this._independentColor; }
    }

    public bool IsSymbolSizeProvider
    {
      get { return false; }
    }

    public bool IsSymbolSizeReceiver
    {
      get { return true; } // we need the symbol size to determine the gap
    }

    float I2DPlotStyle.SymbolSize
    {
      get
      {
        throw new Exception("The method or operation is not implemented.");
      }
      set
      {
        this._symbolGap = value;
      }
    }

    public void Paint(Graphics g, IPlotArea layer, PlotRangeList rangeList, PointF[] ptArray)
    {
      this.Paint(g, ptArray, rangeList, layer, this._symbolGap);
    }

    #endregion

  



  } // end class XYPlotLineStyle
}
