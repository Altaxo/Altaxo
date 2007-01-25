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


namespace Altaxo.Graph.Gdi.Axis
{

  /// <summary>
  /// XYAxisStyle is responsible for painting the axes on rectangular two dimensional layers.
  /// </summary>
  [SerializationSurrogate(0,typeof(AxisLineStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class AxisLineStyle 
    : 
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource,
    Main.IDocumentNode,
    ICloneable   
  {

    /// <summary>Pen used for painting of the axis.</summary>
    protected PenX _axisPen = new PenX(Color.Black,1);
    /// <summary>Pen used for painting of the major ticks.</summary>
    protected PenX _majorTickPen =  new PenX(Color.Black,1);
    /// <summary>Pen used for painting of the minor ticks.</summary>
    protected PenX _minorTickPen =  new PenX(Color.Black,1);
    /// <summary>Length of the major ticks in points (1/72 inch).</summary>
    protected float _majorTickLength = 8;
    /// <summary>Length of the minor ticks in points (1/72 inch).</summary>
    protected float _minorTickLength = 4;
    /// <summary>True if major ticks should be painted outside of the layer.</summary>
    protected bool  _showFirstUpMajorTicks=true; // true if right major ticks should be visible
    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool  _showFirstDownMajorTicks=true; // true if left major ticks should be visible
    /// <summary>True if minor ticks should be painted outside of the layer.</summary>
    protected bool  _showFirstUpMinorTicks=true; // true if right minor ticks should be visible
    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool  _showFirstDownMinorTicks=true; // true if left minor ticks should be visible
    /// <summary>Axis shift position, either provide as absolute values in point units, or as relative value relative to the layer size.</summary>
    protected Calc.RelativeOrAbsoluteValue _axisPosition; // if relative, then relative to layer size, if absolute then in points

    protected CSAxisInformation _cachedAxisStyleInfo;

    /// <summary>Fired if anything on this style changed</summary>
    [field:NonSerialized]
    event System.EventHandler _changed;

    [NonSerialized]
    object _parent;


  
    #region Serialization
    /// <summary>Used to serialize the axis style Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes the axis style (version 0).
      /// </summary>
      /// <param name="obj">The axis style to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        AxisLineStyle s = (AxisLineStyle)obj;
        info.AddValue("AxisPen",s._axisPen);  
        info.AddValue("MajorPen",s._majorTickPen);  
        info.AddValue("MinorPen",s._minorTickPen);
        info.AddValue("MajorLength",s._majorTickLength);
        info.AddValue("MinorLength",s._minorTickLength);
        info.AddValue("MajorRight",s._showFirstUpMajorTicks);
        info.AddValue("MajorLeft",s._showFirstDownMajorTicks);
        info.AddValue("MinorRight",s._showFirstUpMinorTicks);
        info.AddValue("MinorLeft",s._showFirstDownMinorTicks);
        info.AddValue("AxisPosition",s._axisPosition);
      }
      /// <summary>
      /// Deserializes the axis style (version 0).
      /// </summary>
      /// <param name="obj">The empty axis object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized linear axis.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        AxisLineStyle s = (AxisLineStyle)obj;

        s._axisPen      = (PenX)info.GetValue("AxisPen",typeof(PenX));
        s._majorTickPen = (PenX)info.GetValue("MajorPen",typeof(PenX));
        s._minorTickPen = (PenX)info.GetValue("MinorPen",typeof(PenX));

        s._majorTickLength = (float)info.GetSingle("MajorLength");
        s._minorTickLength = (float)info.GetSingle("MinorLength");
        s._showFirstUpMajorTicks = (bool)info.GetBoolean("MajorRight");
        s._showFirstDownMajorTicks = (bool)info.GetBoolean("MajorLeft");
        s._showFirstUpMinorTicks = (bool)info.GetBoolean("MinorRight");
        s._showFirstDownMinorTicks = (bool)info.GetBoolean("MinorLeft");
        s._axisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition",typeof(Calc.RelativeOrAbsoluteValue));
    
        return s;
      }
    }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisStyle",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions not supported - probably a programming error");
        /*
        XYAxisStyle s = (XYAxisStyle)obj;
        info.AddValue("Edge",s.m_Edge);
        info.AddValue("AxisPen",s.m_AxisPen);  
        info.AddValue("MajorPen",s.m_MajorTickPen);  
        info.AddValue("MinorPen",s.m_MinorTickPen);
        info.AddValue("MajorLength",s.m_MajorTickLength);
        info.AddValue("MinorLength",s.m_MinorTickLength);
        info.AddValue("MajorOuter",s.m_bOuterMajorTicks);
        info.AddValue("MajorInner",s.m_bInnerMajorTicks);
        info.AddValue("MinorOuter",s.m_bOuterMinorTicks);
        info.AddValue("MinorInner",s.m_bInnerMinorTicks);
        info.AddValue("AxisPosition",s.m_AxisPosition);
        */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        AxisLineStyle s = null!=o ? (AxisLineStyle)o : new AxisLineStyle();
        
        Edge edge         = (Edge)info.GetValue("Edge",s);
        s._axisPen      = (PenX)info.GetValue("AxisPen",s);
        s._majorTickPen = (PenX)info.GetValue("MajorPen",s);
        s._minorTickPen = (PenX)info.GetValue("MinorPen",s);

        s._majorTickLength = (float)info.GetSingle("MajorLength");
        s._minorTickLength = (float)info.GetSingle("MinorLength");
        bool bOuterMajorTicks = (bool)info.GetBoolean("MajorOuter");
        bool bInnerMajorTicks = (bool)info.GetBoolean("MajorInner");
        bool bOuterMinorTicks = (bool)info.GetBoolean("MinorOuter");
        bool bInnerMinorTicks = (bool)info.GetBoolean("MinorInner");
        s._axisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition",s);

        if (edge.TypeOfEdge == EdgeType.Top || edge.TypeOfEdge == EdgeType.Right)
        {
          s._showFirstUpMajorTicks = bOuterMajorTicks;
          s._showFirstDownMajorTicks = bInnerMajorTicks;
          s._showFirstUpMinorTicks = bOuterMinorTicks;
          s._showFirstDownMinorTicks = bInnerMinorTicks;
        }
        else
        {
          s._showFirstUpMajorTicks = bInnerMajorTicks;
          s._showFirstDownMajorTicks = bOuterMajorTicks;
          s._showFirstUpMinorTicks = bInnerMinorTicks;
          s._showFirstDownMinorTicks = bOuterMinorTicks;
        }

        s.WireEventChain(true);
 
        return s;
      }
    }


    // 2006-09-08 Renaming XYAxisStyle in G2DAxisLineStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLineStyle), 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AxisLineStyle s = (AxisLineStyle)obj;
        info.AddValue("AxisPen", s._axisPen);
        info.AddValue("MajorPen", s._majorTickPen);
        info.AddValue("MinorPen", s._minorTickPen);
        info.AddValue("MajorLength", s._majorTickLength);
        info.AddValue("MinorLength", s._minorTickLength);
        info.AddValue("AxisPosition", s._axisPosition);
        info.AddValue("Major1Up", s._showFirstUpMajorTicks);
        info.AddValue("Major1Dw", s._showFirstDownMajorTicks);
        info.AddValue("Minor1Up", s._showFirstUpMinorTicks);
        info.AddValue("Minor1Dw", s._showFirstDownMinorTicks);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        AxisLineStyle s = null != o ? (AxisLineStyle)o : new AxisLineStyle();

        s._axisPen = (PenX)info.GetValue("AxisPen", s);
        s._majorTickPen = (PenX)info.GetValue("MajorPen", s);
        s._minorTickPen = (PenX)info.GetValue("MinorPen", s);

        s._majorTickLength = (float)info.GetSingle("MajorLength");
        s._minorTickLength = (float)info.GetSingle("MinorLength");
        s._axisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition", s);
        s._showFirstUpMajorTicks = (bool)info.GetBoolean("Major1Up");
        s._showFirstDownMajorTicks = (bool)info.GetBoolean("Major1Dw");
        s._showFirstUpMinorTicks = (bool)info.GetBoolean("Minor1Up");
        s._showFirstDownMinorTicks = (bool)info.GetBoolean("Minor1Dw");

        s.WireEventChain(true);

        return s;
      }
    }
  

    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      WireEventChain(true);
    }
    #endregion


    /// <summary>
    /// Wires the neccessary child events with event handlers in this class.
    /// </summary>
    protected virtual void WireEventChain(bool wire)
    {
      if (wire)
      {
        _axisPen.Changed += new EventHandler(OnPenChangedEventHandler);
        _majorTickPen.Changed += new EventHandler(OnPenChangedEventHandler);
        _minorTickPen.Changed += new EventHandler(OnPenChangedEventHandler);
      }
      else
      {
        _axisPen.Changed -= new EventHandler(OnPenChangedEventHandler);
        _majorTickPen.Changed -= new EventHandler(OnPenChangedEventHandler);
        _minorTickPen.Changed -= new EventHandler(OnPenChangedEventHandler);
      }
    }

    /// <summary>
    /// Creates a default axis style.
    /// </summary>
   
    public AxisLineStyle()
    {
    
      WireEventChain(true);

    }

  
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The AxisStyle to copy from</param>
    public AxisLineStyle(AxisLineStyle from)
    {
      CopyFrom(from);
    }
    /// <summary>
    /// Copy operation.
    /// </summary>
    /// <param name="from">The AxisStyle to copy from</param>
    public void CopyFrom(AxisLineStyle from)
    {
      if (_axisPen != null)
        WireEventChain(false);
     
      this._axisPen = null == from._axisPen ? null : (PenX)from._axisPen.Clone();
      this._axisPosition = from._axisPosition;
      this._showFirstDownMajorTicks = from._showFirstDownMajorTicks;
      this._showFirstDownMinorTicks = from._showFirstDownMinorTicks;
      this._showFirstUpMajorTicks = from._showFirstUpMajorTicks;
      this._showFirstUpMinorTicks = from._showFirstUpMinorTicks;
      this._majorTickLength  = from._majorTickLength;
      this._majorTickPen     = null==from._majorTickPen ? null : (PenX)from._majorTickPen;
      this._minorTickLength  = from._minorTickLength;
      this._minorTickPen     = (null==from._minorTickPen) ? null : (PenX)from._minorTickPen;

      this._cachedAxisStyleInfo = from._cachedAxisStyleInfo;
      this._parent = from._parent;

      // Rewire the event chain
      WireEventChain(true);
    }

   

    /// <summary>
    /// Creates a clone of this object.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public  object Clone()
    {
      return new AxisLineStyle(this);
    }

    public CSLineID AxisStyleID
    {
      get
      {
        return _cachedAxisStyleInfo == null ? null : _cachedAxisStyleInfo.Identifier;
      }
    }

    public CSAxisInformation CachedAxisInformation
    {
      get
      {
          return _cachedAxisStyleInfo;
      }
      set
      {
        _cachedAxisStyleInfo = value;
      }
    }
    public virtual IHitTestObject HitTest(XYPlotLayer layer, PointF pt, bool withTicks)
    {

      GraphicsPath selectionPath = GetSelectionPath(layer,withTicks);
      return selectionPath.IsVisible(pt) ? new HitTestObject(GetObjectPath(layer,withTicks),selectionPath,this) : null;
    }

    /// <summary>
    /// Returns the used space from the middle line of the axis
    /// to the last outer object (either the outer major thicks or half
    /// of the axis thickness)
    /// </summary>
    /// <param name="side">The side of the axis at which the outer distance is returned.</param>
    public float GetOuterDistance(CSAxisSide side)
    {
        float retVal = _axisPen.Width/2; // half of the axis thickness
        if (CSAxisSide.FirstUp == side)
        {
          retVal = System.Math.Max(retVal, _showFirstUpMajorTicks ? _majorTickLength : 0);
          retVal = System.Math.Max(retVal, _showFirstUpMinorTicks ? _minorTickLength : 0);
        }
        else if (CSAxisSide.FirstDown == side)
        {
          retVal = System.Math.Max(retVal, _showFirstDownMajorTicks ? _majorTickLength : 0);
          retVal = System.Math.Max(retVal, _showFirstDownMinorTicks ? _minorTickLength : 0);
        }
        else
        {
          retVal = 0;
        }
        return retVal;
    }

    /// <summary>
    /// GetOffset returns the distance of the axis to the layer edge in points
    /// in most cases, the axis position is exactly onto the layer edge and offset is zero,
    /// if the axis is outside the layer, offset is a positive value, 
    /// if the axis is shifted inside the layer, offset is negative 
    /// </summary>
    public float GetOffset(SizeF layerSize)
    {
      throw new NotImplementedException("Old stuff");
      //return (float)m_AxisPosition.GetValueRelativeTo(m_Edge.GetOppositeEdgeLength(layerSize));
    }

    public PenX AxisPen
    {
      get { return _axisPen; }
      set
      {
        PenX oldvalue = _axisPen;
        _axisPen = value;
        if (!object.ReferenceEquals(oldvalue, value))
        {
          if (null != oldvalue)
            oldvalue.Changed -= OnPenChangedEventHandler;
          if (null != value)
            value.Changed += OnPenChangedEventHandler;

          OnChanged();
        }
      }
    }

    public PenX MajorPen
    {
      get { return _majorTickPen; }
      set
      {
        PenX oldvalue = _majorTickPen;
        _majorTickPen = value;
        if (!object.ReferenceEquals(oldvalue, value))
        {
          if (null != oldvalue)
            oldvalue.Changed -= OnPenChangedEventHandler;
          if (null != value)
            value.Changed += OnPenChangedEventHandler;

          OnChanged();
        }
      }
    }

    public PenX MinorPen
    {
      get { return _minorTickPen; }
      set
      {
        PenX oldvalue = _minorTickPen;
        _minorTickPen = value;
        if (!object.ReferenceEquals(oldvalue, value))
        {
          if (null != oldvalue)
            oldvalue.Changed -= OnPenChangedEventHandler;
          if (null != value)
            value.Changed += OnPenChangedEventHandler;

          OnChanged();
        }
      }
    }

    /// <summary>Get/sets the major tick length.</summary>
    /// <value>The major tick length in point units (1/72 inch).</value>
    public float MajorTickLength
    {
      get { return this._majorTickLength; }
      set
      {
        if(value!=_majorTickLength)
        {
          _majorTickLength = value; 
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets the minor tick length.</summary>
    /// <value>The minor tick length in point units (1/72 inch).</value>
    public float MinorTickLength
    {
      get { return this._minorTickLength; }
      set 
      {
        if(value!=_minorTickLength)
        {
          _minorTickLength = value;
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if outer major ticks are drawn.</summary>
    /// <value>True if outer major ticks are drawn.</value>
    public bool FirstUpMajorTicks
    {
      get { return this._showFirstUpMajorTicks; }
      set 
      {
        if(value!=_showFirstUpMajorTicks)
        {
          this._showFirstUpMajorTicks = value; 
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if inner major ticks are drawn.</summary>
    /// <value>True if inner major ticks are drawn.</value>
    public bool FirstDownMajorTicks
    {
      get { return this._showFirstDownMajorTicks; }
      set
      {
        if(value!=_showFirstDownMajorTicks)
        {
          this._showFirstDownMajorTicks = value; 
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if outer minor ticks are drawn.</summary>
    /// <value>True if outer minor ticks are drawn.</value>
    public bool FirstUpMinorTicks
    {
      get { return this._showFirstUpMinorTicks; }
      set 
      { 
        if(value!=_showFirstUpMinorTicks)
        {
          this._showFirstUpMinorTicks = value;
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if inner minor ticks are drawn.</summary>
    /// <value>True if inner minor ticks are drawn.</value>
    public bool FirstDownMinorTicks
    {
      get { return this._showFirstDownMinorTicks; }
      set 
      { 
        if(value!=_showFirstDownMinorTicks)
        {
          this._showFirstDownMinorTicks = value;
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>
    /// Gets/sets the axis thickness.
    /// </summary>
    /// <value>Returns the thickness of the axis pen. On setting this value, it sets
    /// the thickness of the axis pen, the tickness of the major ticks pen, and the
    /// thickness of the minor ticks pen together.</value>
    public float Thickness
    {
      get { return this._axisPen.Width; }
      set
      { 
        this._axisPen.Width = value;
        this._majorTickPen.Width = value;
        this._minorTickPen.Width = value;
        OnChanged(); // fire the changed event
      }
    }

    /// <summary>
    /// Get/sets the axis color.
    /// </summary>
    /// <value>Returns the color of the axis pen. On setting this value, it sets
    /// the color of the axis pen along with the color of the major ticks pen and the
    /// color of the minor ticks pen together.</value>
    public Color Color
    {
      get { return this._axisPen.Color; }
      set
      {
        this._axisPen.Color = value;
        this._majorTickPen.Color = value;
        this._minorTickPen.Color = value;
        OnChanged(); // fire the changed event
      }
    }

    /// <summary>
    /// Get/set the axis shift position value.
    /// </summary>
    /// <value>Zero if the axis is not shifted (normal case). Else the shift value, either as
    /// absolute value in point units (1/72 inch), or relative to the corresponding layer dimension (i.e layer width for bottom axis).</value>
    public Calc.RelativeOrAbsoluteValue Position
    {
      get { return this._axisPosition; }
      set 
      {
        if(value!=_axisPosition)
        {
          _axisPosition = value;
          OnChanged(); // fire the changed event
        }
      }
    }


    /// <summary>
    /// Gives the path which encloses the axis line only.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <returns>The graphics path of the axis line.</returns>
    public virtual GraphicsPath GetObjectPath(XYPlotLayer layer, bool withTicks)
    {
      return GetPath(layer, withTicks, 0);
    }

     /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <returns>The graphics path of the selection rectangle.</returns>
    public virtual GraphicsPath GetSelectionPath(XYPlotLayer layer, bool withTicks)
    {
      return GetPath(layer, withTicks, 3);
    }

    /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <returns>The graphics path of the selection rectangle.</returns>
    protected GraphicsPath GetPath(XYPlotLayer layer, bool withTicks, float inflateby)
    {

      Logical3D r0 = _cachedAxisStyleInfo.Identifier.Begin;
      Logical3D r1 = _cachedAxisStyleInfo.Identifier.End;
      GraphicsPath gp = new GraphicsPath();
      layer.CoordinateSystem.GetIsoline(gp, r0, r1);
     
      if(withTicks)
      {
        if(this._showFirstDownMajorTicks || this._showFirstUpMajorTicks)
          inflateby = Math.Max(inflateby,this._majorTickLength);
        if(this._showFirstDownMinorTicks || this._showFirstUpMinorTicks)
          inflateby = Math.Max(inflateby,this._minorTickLength);
      }

      Pen widenPen = new Pen(Color.Black, 2*inflateby);

      gp.Widen(widenPen);

      return gp;
    }


    /// <summary>
    /// Paint the axis in the Graphics context.
    /// </summary>
    /// <param name="g">The graphics context painting to.</param>
    /// <param name="layer">The layer the axis belongs to.</param>
    /// <param name="styleInfo">The axis information of the axis to paint.</param>
    public void Paint(Graphics g, XYPlotLayer layer, CSAxisInformation styleInfo)
    {
      CSLineID styleID = styleInfo.Identifier;
      _cachedAxisStyleInfo = styleInfo.Clone();
      Scale axis = styleID.ParallelAxisNumber == 0 ? layer.XAxis : layer.YAxis;
      
      Logical3D r0 = styleID.Begin;
      Logical3D r1 = styleID.End;

      layer.CoordinateSystem.DrawIsoline(g, _axisPen, r0, r1);

      Logical3D outer;
     


      // now the major ticks
      PointF outVector;
      double[] majorticks = axis.GetMajorTicksNormal();
      for(int i=0;i<majorticks.Length;i++)
      {
        double r = majorticks[i];

        if(_showFirstUpMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber,CSAxisSide.FirstUp);
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          PointF tickend = tickorg;
          tickend.X += outVector.X * _majorTickLength;
          tickend.Y += outVector.Y * _majorTickLength;
          g.DrawLine(_majorTickPen,tickorg,tickend);
        }
        if(_showFirstDownMajorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstDown);
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          PointF tickend = tickorg;
          tickend.X += outVector.X * _majorTickLength;
          tickend.Y += outVector.Y * _majorTickLength;
          g.DrawLine(_majorTickPen,tickorg,tickend);
        }
      }
      // now the major ticks
      double[] minorticks = axis.GetMinorTicksNormal();
      for(int i=0;i<minorticks.Length;i++)
      {
        double r = minorticks[i];
        
        if(_showFirstUpMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstUp);
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          PointF tickend = tickorg;
          tickend.X += outVector.X * _minorTickLength;
          tickend.Y += outVector.Y * _minorTickLength;
          g.DrawLine(_minorTickPen,tickorg,tickend);
        }
        if(_showFirstDownMinorTicks)
        {
          outer = layer.CoordinateSystem.GetLogicalDirection(styleID.ParallelAxisNumber, CSAxisSide.FirstDown);
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(r0, r1, r, outer, out outVector);
          PointF tickend = tickorg;
          tickend.X += outVector.X * _minorTickLength;
          tickend.Y += outVector.Y * _minorTickLength;
          g.DrawLine(_minorTickPen,tickorg,tickend);
        }
      }
    }

    

    protected virtual void OnPenChangedEventHandler(object sender, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if(null!=_changed)
        _changed(this,new EventArgs());
    }

    public event EventHandler Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return this.GetType().Name; }
    }

    #endregion
  }
}
