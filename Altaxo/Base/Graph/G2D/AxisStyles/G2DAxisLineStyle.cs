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
using Altaxo.Graph.Scales;


namespace Altaxo.Graph
{

  /// <summary>
  /// XYAxisStyle is responsible for painting the axes on rectangular two dimensional layers.
  /// </summary>
  [SerializationSurrogate(0,typeof(G2DAxisLineStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class G2DAxisLineStyle 
    : 
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource,
    ICloneable   
  {

    /// <summary>Pen used for painting of the axis.</summary>
    protected PenHolder _axisPen = new PenHolder(Color.Black,1);
    /// <summary>Pen used for painting of the major ticks.</summary>
    protected PenHolder _majorTickPen =  new PenHolder(Color.Black,1);
    /// <summary>Pen used for painting of the minor ticks.</summary>
    protected PenHolder _minorTickPen =  new PenHolder(Color.Black,1);
    /// <summary>Length of the major ticks in points (1/72 inch).</summary>
    protected float _majorTickLength = 8;
    /// <summary>Length of the minor ticks in points (1/72 inch).</summary>
    protected float _minorTickLength = 4;
    /// <summary>True if major ticks should be painted outside of the layer.</summary>
    protected bool  _showRightMajorTicks=true; // true if right major ticks should be visible
    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool  _showLeftMajorTicks=true; // true if left major ticks should be visible
    /// <summary>True if minor ticks should be painted outside of the layer.</summary>
    protected bool  _showRightMinorTicks=true; // true if right minor ticks should be visible
    /// <summary>True if major ticks should be painted inside of the layer.</summary>
    protected bool  _showLeftMinorTicks=true; // true if left minor ticks should be visible
    /// <summary>Axis shift position, either provide as absolute values in point units, or as relative value relative to the layer size.</summary>
    protected Calc.RelativeOrAbsoluteValue _axisPosition; // if relative, then relative to layer size, if absolute then in points

    protected A2DAxisStyleInformation _cachedAxisStyleInfo;

    /// <summary>Fired if anything on this style changed</summary>
    public event System.EventHandler Changed;


  
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
        G2DAxisLineStyle s = (G2DAxisLineStyle)obj;
        info.AddValue("AxisPen",s._axisPen);  
        info.AddValue("MajorPen",s._majorTickPen);  
        info.AddValue("MinorPen",s._minorTickPen);
        info.AddValue("MajorLength",s._majorTickLength);
        info.AddValue("MinorLength",s._minorTickLength);
        info.AddValue("MajorRight",s._showRightMajorTicks);
        info.AddValue("MajorLeft",s._showLeftMajorTicks);
        info.AddValue("MinorRight",s._showRightMinorTicks);
        info.AddValue("MinorLeft",s._showLeftMinorTicks);
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
        G2DAxisLineStyle s = (G2DAxisLineStyle)obj;

        s._axisPen      = (PenHolder)info.GetValue("AxisPen",typeof(PenHolder));
        s._majorTickPen = (PenHolder)info.GetValue("MajorPen",typeof(PenHolder));
        s._minorTickPen = (PenHolder)info.GetValue("MinorPen",typeof(PenHolder));

        s._majorTickLength = (float)info.GetSingle("MajorLength");
        s._minorTickLength = (float)info.GetSingle("MinorLength");
        s._showRightMajorTicks = (bool)info.GetBoolean("MajorRight");
        s._showLeftMajorTicks = (bool)info.GetBoolean("MajorLeft");
        s._showRightMinorTicks = (bool)info.GetBoolean("MinorRight");
        s._showLeftMinorTicks = (bool)info.GetBoolean("MinorLeft");
        s._axisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition",typeof(Calc.RelativeOrAbsoluteValue));
    
        return s;
      }
    }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisStyle",0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
        
        G2DAxisLineStyle s = null!=o ? (G2DAxisLineStyle)o : new G2DAxisLineStyle();
        
        Edge edge         = (Edge)info.GetValue("Edge",s);
        s._axisPen      = (PenHolder)info.GetValue("AxisPen",s);
        s._majorTickPen = (PenHolder)info.GetValue("MajorPen",s);
        s._minorTickPen = (PenHolder)info.GetValue("MinorPen",s);

        s._majorTickLength = (float)info.GetSingle("MajorLength");
        s._minorTickLength = (float)info.GetSingle("MinorLength");
        bool bOuterMajorTicks = (bool)info.GetBoolean("MajorOuter");
        bool bInnerMajorTicks = (bool)info.GetBoolean("MajorInner");
        bool bOuterMinorTicks = (bool)info.GetBoolean("MinorOuter");
        bool bInnerMinorTicks = (bool)info.GetBoolean("MinorInner");
        s._axisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition",s);

        if (edge.TypeOfEdge == EdgeType.Bottom || edge.TypeOfEdge == EdgeType.Right)
        {
          s._showRightMajorTicks = bOuterMajorTicks;
          s._showLeftMajorTicks = bInnerMajorTicks;
          s._showRightMinorTicks = bOuterMinorTicks;
          s._showLeftMinorTicks = bInnerMinorTicks;
        }
        else
        {
          s._showRightMajorTicks = bInnerMajorTicks;
          s._showLeftMajorTicks = bOuterMajorTicks;
          s._showRightMinorTicks = bInnerMinorTicks;
          s._showLeftMinorTicks = bOuterMinorTicks;
        }

        s.WireEventChain(true);
 
        return s;
      }
    }


    // 2006-09-08 Renaming XYAxisStyle in G2DAxisLineStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DAxisLineStyle), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DAxisLineStyle s = (G2DAxisLineStyle)obj;
        info.AddValue("AxisPen", s._axisPen);
        info.AddValue("MajorPen", s._majorTickPen);
        info.AddValue("MinorPen", s._minorTickPen);
        info.AddValue("MajorLength", s._majorTickLength);
        info.AddValue("MinorLength", s._minorTickLength);
        info.AddValue("MajorRight", s._showRightMajorTicks);
        info.AddValue("MajorLeft", s._showLeftMajorTicks);
        info.AddValue("MinorRight", s._showRightMinorTicks);
        info.AddValue("MinorLeft", s._showLeftMinorTicks);
        info.AddValue("AxisPosition", s._axisPosition);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        G2DAxisLineStyle s = null != o ? (G2DAxisLineStyle)o : new G2DAxisLineStyle();

        s._axisPen = (PenHolder)info.GetValue("AxisPen", s);
        s._majorTickPen = (PenHolder)info.GetValue("MajorPen", s);
        s._minorTickPen = (PenHolder)info.GetValue("MinorPen", s);

        s._majorTickLength = (float)info.GetSingle("MajorLength");
        s._minorTickLength = (float)info.GetSingle("MinorLength");
        s._showRightMajorTicks = (bool)info.GetBoolean("MajorRight");
        s._showLeftMajorTicks = (bool)info.GetBoolean("MajorLeft");
        s._showRightMinorTicks = (bool)info.GetBoolean("MinorRight");
        s._showLeftMinorTicks = (bool)info.GetBoolean("MinorLeft");
        s._axisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition", s);

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
   
    public G2DAxisLineStyle()
    {
    
      WireEventChain(true);

    }

  
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The AxisStyle to copy from</param>
    public G2DAxisLineStyle(G2DAxisLineStyle from)
    {
      CopyFrom(from);
    }
    /// <summary>
    /// Copy operation.
    /// </summary>
    /// <param name="from">The AxisStyle to copy from</param>
    public void CopyFrom(G2DAxisLineStyle from)
    {
      if (_axisPen != null)
        WireEventChain(false);
     
      this._axisPen = null == from._axisPen ? null : (PenHolder)from._axisPen.Clone();
      this._axisPosition = from._axisPosition;
      this._showLeftMajorTicks = from._showLeftMajorTicks;
      this._showLeftMinorTicks = from._showLeftMinorTicks;
      this._showRightMajorTicks = from._showRightMajorTicks;
      this._showRightMinorTicks = from._showRightMinorTicks;
      this._majorTickLength  = from._majorTickLength;
      this._majorTickPen     = null==from._majorTickPen ? null : (PenHolder)from._majorTickPen;
      this._minorTickLength  = from._minorTickLength;
      this._minorTickPen     = (null==from._minorTickPen) ? null : (PenHolder)from._minorTickPen;

      // Rewire the event chain
      WireEventChain(true);
    }

   

    /// <summary>
    /// Creates a clone of this object.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public  object Clone()
    {
      return new G2DAxisLineStyle(this);
    }

    public A2DAxisStyleIdentifier AxisStyleID
    {
      get
      {
        return _cachedAxisStyleInfo == null ? null : _cachedAxisStyleInfo.Identifier;
      }
    }
    public virtual IHitTestObject HitTest(XYPlotLayer layer, PointF pt, bool withTicks)
    {
      GraphicsPath gp = GetSelectionPath(layer,withTicks);
      return gp.IsVisible(pt) ? new HitTestObject(gp,this) : null;
    }

    /// <summary>
    /// Returns the used space from the middle line of the axis
    /// to the last outer object (either the outer major thicks or half
    /// of the axis thickness)
    /// </summary>
    /// <param name="side">The side of the axis at which the outer distance is returned.</param>
    public float GetOuterDistance(A2DAxisSide side)
    {
        float retVal = _axisPen.Width/2; // half of the axis thickness
        if (A2DAxisSide.Right == side)
        {
          retVal = System.Math.Max(retVal, _showRightMajorTicks ? _majorTickLength : 0);
          retVal = System.Math.Max(retVal, _showRightMinorTicks ? _minorTickLength : 0);
        }
        else
        {
          retVal = System.Math.Max(retVal, _showLeftMajorTicks ? _majorTickLength : 0);
          retVal = System.Math.Max(retVal, _showLeftMinorTicks ? _minorTickLength : 0);
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
    public bool RightSideMajorTicks
    {
      get { return this._showRightMajorTicks; }
      set 
      {
        if(value!=_showRightMajorTicks)
        {
          this._showRightMajorTicks = value; 
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if inner major ticks are drawn.</summary>
    /// <value>True if inner major ticks are drawn.</value>
    public bool LeftSideMajorTicks
    {
      get { return this._showLeftMajorTicks; }
      set
      {
        if(value!=_showLeftMajorTicks)
        {
          this._showLeftMajorTicks = value; 
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if outer minor ticks are drawn.</summary>
    /// <value>True if outer minor ticks are drawn.</value>
    public bool RightSideMinorTicks
    {
      get { return this._showRightMinorTicks; }
      set 
      { 
        if(value!=_showRightMinorTicks)
        {
          this._showRightMinorTicks = value;
          OnChanged(); // fire the changed event
        }
      }
    }

    /// <summary>Get/sets if inner minor ticks are drawn.</summary>
    /// <value>True if inner minor ticks are drawn.</value>
    public bool LeftSideMinorTicks
    {
      get { return this._showLeftMinorTicks; }
      set 
      { 
        if(value!=_showLeftMinorTicks)
        {
          this._showLeftMinorTicks = value;
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
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="withTicks">If true, the selection path is not only drawn around the axis, but around the axis and the ticks.</param>
    /// <returns>The graphics path of the selection rectangle.</returns>
    public virtual GraphicsPath GetSelectionPath(XYPlotLayer layer, bool withTicks)
    {

      double rx0 = 0, rx1 = 1;
      double ry0 = 0, ry1 = 1;

      if (_cachedAxisStyleInfo.Identifier.AxisNumber == 0)
      {
        ry0 = ry1 = _cachedAxisStyleInfo.Identifier.LogicalValue;
      }
      else
      {
        rx0 = rx1 = _cachedAxisStyleInfo.Identifier.LogicalValue;
      }
      GraphicsPath gp = new GraphicsPath();
      layer.CoordinateSystem.GetIsoline(gp, rx0, ry0, rx1, ry1);
     
      float inflateby = 3;
      if(withTicks)
      {
        if(this._showLeftMajorTicks || this._showRightMajorTicks)
          inflateby = Math.Max(inflateby,this._majorTickLength);
        if(this._showLeftMinorTicks || this._showRightMinorTicks)
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
    /// <param name="axis">The axis this axis style is used for.</param>
    public void Paint(Graphics g, XYPlotLayer layer, A2DAxisStyleInformation styleInfo)
    {
      A2DAxisStyleIdentifier styleID = styleInfo.Identifier;
      _cachedAxisStyleInfo = styleInfo.Clone();
      Scale axis = styleID.AxisNumber == 0 ? layer.XAxis : layer.YAxis;
      
      double rx0 = 0, rx1 = 1;
      double ry0 = 0, ry1 = 1;
      if (styleID.AxisNumber == 0)
        ry0 = ry1 = styleID.LogicalValue;
      else
        rx0 = rx1 = styleID.LogicalValue;

      layer.CoordinateSystem.DrawIsoline(g, _axisPen, rx0,ry0,rx1,ry1);

      double outer = 0;
     


      // now the major ticks
      PointF outVector;
      double[] majorticks = axis.GetMajorTicksNormal();
      for(int i=0;i<majorticks.Length;i++)
      {
        double r = majorticks[i];

        if(_showRightMajorTicks)
        {
          outer = -90;
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(rx0, ry0, rx1, ry1, r, outer, out outVector);
          PointF tickend = tickorg;
          tickend.X += outVector.X * _majorTickLength;
          tickend.Y += outVector.Y * _majorTickLength;
          g.DrawLine(_majorTickPen,tickorg,tickend);
        }
        if(_showLeftMajorTicks)
        {
          outer = 90;
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(rx0, ry0, rx1, ry1, r, outer, out outVector);
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
        
        if(_showRightMinorTicks)
        {
          outer = -90;
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(rx0, ry0, rx1, ry1, r, outer, out outVector);
          PointF tickend = tickorg;
          tickend.X += outVector.X * _minorTickLength;
          tickend.Y += outVector.Y * _minorTickLength;
          g.DrawLine(_minorTickPen,tickorg,tickend);
        }
        if(_showLeftMinorTicks)
        {
          outer = 90;
          PointF tickorg = layer.CoordinateSystem.GetNormalizedDirection(rx0, ry0, rx1, ry1, r, outer, out outVector);
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
      if(null!=Changed)
        Changed(this,new EventArgs());
    }
  }
}
