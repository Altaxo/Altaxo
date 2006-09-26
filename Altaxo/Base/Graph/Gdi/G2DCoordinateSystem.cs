using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi
{
  public abstract class G2DCoordinateSystem : ICloneable
  {
    [NonSerialized]
    IPlotArea _parent;
    /// <summary>
    /// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
    /// </summary>
    protected bool _isXYInterchanged;
    /// <summary>
    /// Is the direction of the x axis reverse, for instance runs from right to left.
    /// </summary>
    protected bool _isXreverse;
    /// <summary>
    /// Is the direction of the y axis reverse, for instance runs from top to bottom.
    /// </summary>
    protected bool _isYreverse;

    protected double _layerWidth;
    protected double _layerHeight;

    protected List<A2DAxisStyleInformation> _axisStyleInformation = new List<A2DAxisStyleInformation>();

    /// <summary>
    /// Copies the member variables from another coordinate system.
    /// </summary>
    /// <param name="from">The coordinate system to copy from.</param>
    public virtual void CopyFrom(G2DCoordinateSystem from)
    {
      this._isXYInterchanged = from._isXYInterchanged;
      this._isXreverse = from._isXreverse;
      this._isYreverse = from._isYreverse;
      this._layerWidth = from._layerWidth;
      this._layerHeight = from._layerHeight;
      this._axisStyleInformation.Clear();
      foreach (A2DAxisStyleInformation info in from._axisStyleInformation)
        this._axisStyleInformation.Add(info.Clone());
    }



    /// <summary>
    /// Returns true if the plot area is orthogonal, i.e. if the x and the y axis are orthogonal to each other.
    /// </summary>
    public abstract bool IsOrthogonal { get; }

    /// <summary>
    /// Returns true if the plot coordinates can be calculated as a linear transformation of the physical values. This means that all lines
    /// will keep being lines.
    /// Returns false if this is for instance a polar diagram. 
    /// </summary>
    public abstract bool IsAffine { get; }

    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public abstract bool LogicalToLayerCoordinates(double rx, double ry, out double xlocation, out double ylocation);

    /// <summary>
    /// Converts logical coordinates along an isoline to layer coordinates and the appropriate derivative.
    /// </summary>
    /// <param name="rx0">Logical x of starting point of the isoline.</param>
    /// <param name="ry0">Logical y of starting point of the isoline.</param>
    /// <param name="rx1">Logical x of end point of the isoline.</param>
    /// <param name="ry1">Logical y of end point of the isoline.</param>
    /// <param name="t">Parameter between 0 and 1 that determines the point on the isoline.
    /// A value of 0 denotes the starting point of the isoline, a value of 1 the end point. The logical
    /// coordinates are linear interpolated between starting point and end point.</param>
    /// <param name="ax">Layer coordinate x of the isoline point.</param>
    /// <param name="ay">Layer coordinate y of the isoline point.</param>
    /// <param name="adx">Derivative of layer coordinate x with respect to parameter t at the point (ax,ay).</param>
    /// <param name="ady">Derivative of layer coordinate y with respect to parameter t at the point (ax,ay).</param>
    /// <returns>True if the conversion was sucessfull, otherwise false.</returns>
    public abstract bool LogicalToLayerCoordinatesAndDirection(
      double rx0, double ry0, double rx1, double ry1,
      double t,
      out double ax, out double ay, out double adx, out double ady);

    /// <summary>
    /// Calculates from two  coordinates of a point the logical values (values between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">The x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">The y coordinate of the converted value (for instance location).</param>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public abstract bool LayerToLogicalCoordinates(double xlocation, double ylocation, out double rx, out double ry);

    /// <summary>
    /// Gets a iso line in a path object.
    /// </summary>
    /// <param name="path">The graphics path.</param>
    /// <param name="rx0">Relative starting point x.</param>
    /// <param name="ry0">Relative starting point y.</param>
    /// <param name="rx1">Relative end point x.</param>
    /// <param name="ry1">Relative end point y.</param>
    public abstract void GetIsoline(System.Drawing.Drawing2D.GraphicsPath path, double rx0, double ry0, double rx1, double ry1);

    /// <summary>
    /// Fills the list of axis information with new values.
    /// </summary>
    protected abstract void UpdateAxisInfo();

    #region ICloneable Members

    public abstract object Clone();

    #endregion

    /// <summary>
    /// Get a region object, which describes the plotting area. Used to clip the plotting to
    /// the plotting area.
    /// </summary>
    /// <returns>A region object describing the plotting area.</returns>
    public abstract Region GetRegion();


    /// <summary>
    /// Get/ sets the parent of this object.
    /// </summary>
    public IPlotArea Parent
    {
      get { return _parent; }
      set
      {
        _parent = value;
      }
    }


    /// <summary>
    /// Updates the internal storage of the rectangular area size to a new value.
    /// </summary>
    /// <param name="size">The new size.</param>
    public virtual void UpdateAreaSize(System.Drawing.SizeF size)
    {
      _layerWidth = size.Width;
      _layerHeight = size.Height;
    }


    /// <summary>
    /// Draws an isoline on the plot area.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="pen">The style of the pen used to draw the line.</param>
    /// <param name="rx0">Relative coordinate x of the starting point.</param>
    /// <param name="ry0">Relative coordinate y of the starting point.</param>
    /// <param name="rx1">Relative coordinate x of the end point.</param>
    /// <param name="ry1">Relative coordinate y of the end point.</param>
    public virtual void DrawIsoline(System.Drawing.Graphics g, System.Drawing.Pen pen, double rx0, double ry0, double rx1, double ry1)
    {
      using (GraphicsPath path = new GraphicsPath())
      {
        GetIsoline(path, rx0, ry0, rx1, ry1);
        g.DrawPath(pen, path);
      }
    }

    /// <summary>
    /// Draws an isoline beginning from the axis to the given point.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="id">The axis to start drawing.</param>
    /// <param name="rx">Logical x coordinate of the end point.</param>
    /// <param name="ry">Logical y coordinate of the end point.</param>
    public virtual void GetIsolineFromAxisToPoint(GraphicsPath path, A2DAxisStyleIdentifier id, double rx, double ry)
    {
      if (id.AxisNumber == 0)
      {
        GetIsoline(path, rx, id.LogicalValue, rx, ry);
      }
      else
      {
        GetIsoline(path, id.LogicalValue, ry, rx, ry);
      }
    }

    /// <summary>
    /// Draws an isoline beginning from a given point to the axis.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="rx">Logical x coordinate of the start point.</param>
    /// <param name="ry">Logical y coordinate of the start point.</param>
    /// <param name="id">The axis to end the isoline.</param>
    public virtual void GetIsolineFromPointToAxis(GraphicsPath path, double rx, double ry, A2DAxisStyleIdentifier id)
    {
      if (id.AxisNumber == 0)
      {
        GetIsoline(path, rx, ry, rx, id.LogicalValue);
      }
      else
      {
        GetIsoline(path, rx, ry, id.LogicalValue, ry);
      }
    }

    /// <summary>
    /// Draws an isoline beginning from a given point to the axis.
    /// </summary>
    /// <param name="g">Graphics to draw the isoline to.</param>
    /// <param name="pen">The pen to use.</param>
    /// <param name="rx">Logical x coordinate of the start point.</param>
    /// <param name="ry">Logical y coordinate of the start point.</param>
    /// <param name="id">The axis to end the isoline.</param>
    public virtual void DrawIsolineFromPointToAxis(Graphics g, System.Drawing.Pen pen, double rx, double ry, A2DAxisStyleIdentifier id)
    {
      if (id.AxisNumber == 0)
      {
        DrawIsoline(g, pen, rx, ry, rx, id.LogicalValue);
      }
      else
      {
        DrawIsoline(g, pen, rx, ry, id.LogicalValue, ry);
      }
    }

    /// <summary>
    /// Draws an isoline on the axis beginning from r0 to r1. For r0,r1 either ry0,ry1 is used (if it is an x-axis),
    /// otherwise rx0,ry1 is used. The other parameter pair is not used.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="rx0">Logical x coordinate of the start point.</param>
    /// <param name="ry1">Logical y coordinate of the start point.</param>
    /// <param name="rx0">Logical x coordinate of the start point.</param>
    /// <param name="ry1">Logical y coordinate of the start point.</param>
    /// <param name="id">The axis to end the isoline.</param>
    public virtual void GetIsolineOnAxis(GraphicsPath path, A2DAxisStyleIdentifier id, double rx0, double ry0, double rx1, double ry1)
    {
      if (id.AxisNumber == 0)
      {
        GetIsoline(path, rx0, id.LogicalValue, rx1, id.LogicalValue);
      }
      else
      {
        GetIsoline(path, id.LogicalValue, ry0, id.LogicalValue, ry1);
      }
    }

    public PointF GetPointOnAxis(A2DAxisStyleIdentifier id, double rx, double ry)
    {
      if (id.AxisNumber == 0)
        return LogicalToLayerCoordinates(rx, id.LogicalValue);
      else
        return LogicalToLayerCoordinates(id.LogicalValue, ry);
    }

    /// <summary>
    /// Get a line along the axis designated by the argument id from the logical values r0 to r1.
    /// </summary>
    /// <param name="path">Graphics path.</param>
    /// <param name="id">Axis to draw the isoline along.</param>
    /// <param name="r0">Start point of the isoline. The logical value of the other coordinate.</param>
    /// <param name="r1">End point of the isoline. The logical value of the other coordinate.</param>
    public virtual void GetIsolineFromTo(GraphicsPath path, A2DAxisStyleIdentifier id, double r0, double r1)
    {
      if (id.AxisNumber == 0)
      {
        GetIsoline(path, r0, id.LogicalValue, r1, id.LogicalValue);
      }
      else
      {
        GetIsoline(path, id.LogicalValue, r0, id.LogicalValue, r1);
      }
    }




    public PointF LogicalToLayerCoordinates(double rx, double ry)
    {
      double ax, ay;
      LogicalToLayerCoordinates(rx, ry, out ax, out ay);
      return new PointF((float)ax, (float)ay);
    }



    /// <summary>
    /// Converts logical coordinates along an isoline to layer coordinates and returns the direction of the isoline at this point.
    /// </summary>
    /// <param name="rx0">Logical x of starting point of the isoline.</param>
    /// <param name="ry0">Logical y of starting point of the isoline.</param>
    /// <param name="rx1">Logical x of end point of the isoline.</param>
    /// <param name="ry1">Logical y of end point of the isoline.</param>
    /// <param name="t">Parameter between 0 and 1 that determines the point on the isoline.
    /// A value of 0 denotes the starting point of the isoline, a value of 1 the end point. The logical
    /// coordinates are linear interpolated between starting point and end point.</param>
    /// <param name="angle">Angle between direction of the isoline and returned normalized direction vector.</param>
    /// <param name="normalizeddirection">Returns the normalized direction vector,i.e. a vector of norm 1, that
    /// has the angle <paramref name="angle"/> to the tangent of the isoline. </param>
    /// <returns>The location (in layer coordinates) of the isoline point.</returns>
    public PointF GetNormalizedDirection(
      double rx0, double ry0, double rx1, double ry1,
      double t,
      double angle,
      out PointF normalizeddirection)
    {
      double ax, ay, adx, ady;
      this.LogicalToLayerCoordinatesAndDirection(
        rx0, ry0, rx1, ry1,
        t,
        out ax, out ay, out adx, out ady);


      if (angle != 0)
      {
        double phi = Math.PI * angle / 180;
        double hdx = adx * Math.Cos(phi) + ady * Math.Sin(phi);
        ady = -adx * Math.Sin(phi) + ady * Math.Cos(phi);
        adx = hdx;
      }


      // Normalize the vector
      double rr = Calc.RMath.Hypot(adx, ady);
      if (rr > 0)
      {
        adx /= rr;
        ady /= rr;
      }

      normalizeddirection = new PointF((float)adx, (float)ady);


      return new PointF((float)ax, (float)ay);
    }

    /// <summary>
    /// Enumerators all axis style information.
    /// </summary>
    public IEnumerable<A2DAxisStyleInformation> AxisStyles
    {
      get
      {
        if (_axisStyleInformation == null || _axisStyleInformation.Count == 0)
          UpdateAxisInfo();

        return _axisStyleInformation;
      }
    }

    /// <summary>
    /// Find the axis style with the given id. If found, returns the index of this style, or -1 otherwise.
    /// </summary>
    /// <param name="id">The id to find.</param>
    /// <returns>Index of the style, or -1 if not found.</returns>
    public int IndexOfAxisStyle(A2DAxisStyleIdentifier id)
    {
      if (id == null)
        return -1;

      if (_axisStyleInformation == null || _axisStyleInformation.Count == 0)
        UpdateAxisInfo();

      for (int i = 0; i < _axisStyleInformation.Count; i++)
        if (_axisStyleInformation[i].Identifier == id)
          return i;

      return -1;
    }

    public A2DAxisStyleInformation GetAxisStyleInformation(A2DAxisStyleIdentifier styleID)
    {
      if (_axisStyleInformation == null || _axisStyleInformation.Count == 0)
        UpdateAxisInfo();

      // search for the same axis first, then for the style with the nearest logical value
      double minDistance = double.MaxValue;
      A2DAxisStyleInformation nearestInfo = null;

      foreach (A2DAxisStyleInformation info in this._axisStyleInformation)
      {
        if (styleID.AxisNumber == info.Identifier.AxisNumber)
        {
          double dist = Math.Abs(styleID.LogicalValue - info.Identifier.LogicalValue);
          if (dist < minDistance)
          {
            minDistance = dist;
            nearestInfo = info;
            if (0 == minDistance)
              break; // it can not be smaller than 0
          }
        }

      }

      A2DAxisStyleInformation result = new A2DAxisStyleInformation(styleID);
      if (nearestInfo == null)
      {
        result.SetDefaultValues();
      }
      else
      {
        result.CopyWithoutIdentifierFrom(nearestInfo);
      }

      return result;
    }

    public IEnumerable<A2DAxisStyleIdentifier> GetJoinedAxisStyleIdentifier(IEnumerable<A2DAxisStyleIdentifier> list1, IEnumerable<A2DAxisStyleIdentifier> list2)
    {
      Dictionary<A2DAxisStyleIdentifier, object> dict = new Dictionary<A2DAxisStyleIdentifier, object>();

      foreach (A2DAxisStyleInformation info in AxisStyles)
      {
        dict.Add(info.Identifier, null);
        yield return info.Identifier;
      }

      if (list1 != null)
      {
        foreach (A2DAxisStyleIdentifier id in list1)
        {
          if (!dict.ContainsKey(id))
          {
            dict.Add(id, null);
            yield return id;
          }
        }
      }

      if (list2 != null)
      {
        foreach (A2DAxisStyleIdentifier id in list2)
        {
          if (!dict.ContainsKey(id))
          {
            dict.Add(id, null);
            yield return id;
          }
        }
      }
    }
  }
      
    

  public class G2DCartesicCoordinateSystem : G2DCoordinateSystem
  {
    public G2DCartesicCoordinateSystem()
    {
    }

    /// <summary>
    /// Returns true if the plot area is orthogonal, i.e. if the x and the y axis are orthogonal to each other.
    /// </summary>
    public override bool IsOrthogonal { get { return true; } }

    /// <summary>
    /// Returns true if the plot coordinates can be calculated as a linear transformation of the physical values.
    /// Returns false if this is for instance a polar diagram. 
    /// </summary>
    public override bool IsAffine { get { return true; } }

    protected override void UpdateAxisInfo()
    {
      int horzAx;
      int vertAx ;
      bool vertRev;
      bool horzRev;

      if (_isXYInterchanged)
      {
         horzAx = 1;
         vertAx = 0;
        vertRev = _isXreverse;
        horzRev = _isYreverse;
      }
      else
      {
         horzAx = 0;
         vertAx = 1;
         vertRev = _isYreverse;
        horzRev = _isXreverse;
      }

      _axisStyleInformation.Clear();

      A2DAxisStyleInformation info;

      // Left
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(vertAx, horzRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Left";
      info.NameOfLeftSide = vertRev ? "Inner" : "Outer";
      info.NameOfRightSide = vertRev ? "Outer": "Inner";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Right : A2DAxisSide.Left;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;
      

      // Right
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(vertAx, horzRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Right";
      info.NameOfLeftSide = vertRev ? "Outer" : "Inner";
      info.NameOfRightSide = vertRev ? "Inner" : "Outer";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Left : A2DAxisSide.Right;

      // Bottom
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(horzAx, vertRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Bottom";
      info.NameOfLeftSide = horzRev ? "Outer" : "Inner";
      info.NameOfRightSide = horzRev ? "Inner" : "Outer";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Left : A2DAxisSide.Right;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Top
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(horzAx, vertRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Top";
      info.NameOfLeftSide = horzRev ? "Inner" : "Outer";
      info.NameOfRightSide = horzRev ? "Outer" : "Inner";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Right : A2DAxisSide.Left;
    }

    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LogicalToLayerCoordinates(double rx, double ry, out double xlocation, out double ylocation)
    {
      if (_isXreverse)
        rx = 1 - rx;
      if (_isYreverse)
        ry = 1 - ry;
      if (_isXYInterchanged)
      {
        double hr = rx;
        rx = ry;
        ry = hr;
      }


      xlocation = _layerWidth * rx;
      ylocation = _layerHeight * (1 - ry);
      return !double.IsNaN(xlocation) && !double.IsNaN(ylocation);
    }


     public override bool LogicalToLayerCoordinatesAndDirection(
      double rx0, double ry0, double rx1, double ry1,
      double t,
      out double ax, out double ay, out double adx, out double ady)
    {
      if (_isXreverse)
      {
        rx0 = 1 - rx0;
        rx1 = 1 - rx1;
      }
      if (_isYreverse)
      {
        ry0 = 1 - ry0;
        ry1 = 1 - ry1;
      }
      if (_isXYInterchanged)
      {
        double hr0 = rx0;
        rx0 = ry0;
        ry0 = hr0;

        double hr1 = rx1;
        rx1 = ry1;
        ry1 = hr1;
      }

      double rx = rx0 + t * (rx1 - rx0);
      double ry = ry0 + t * (ry1 - ry0);
     
       
      ax = _layerWidth * rx;
      ay = _layerHeight * (1 - ry);

      adx = _layerWidth * (rx1-rx0);
      ady = _layerHeight *(ry0-ry1);
   
      return !double.IsNaN(ax) && !double.IsNaN(ay);
    }


   

    /// <summary>
    /// Calculates from two layer coordinate values (in points usually) the relative coordinates of the point (between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LayerToLogicalCoordinates(double xlocation, double ylocation, out double rx, out double ry)
    {
      rx = xlocation / _layerWidth;
      ry = 1 - ylocation / _layerHeight;

      if (_isXreverse)
        rx = 1 - rx;
      if (_isYreverse)
        ry = 1 - ry;
      if (_isXYInterchanged)
      {
        double hr = rx;
        rx = ry;
        ry = hr;
      }

      return !double.IsNaN(rx) && !double.IsNaN(ry);
    }

    public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath g, double rx0, double ry0, double rx1, double ry1)
    {
      double ax0, ax1, ay0, ay1;
      if (LogicalToLayerCoordinates(rx0, ry0, out ax0, out ay0) && LogicalToLayerCoordinates(rx1, ry1, out ax1, out ay1))
      {
        g.AddLine((float)ax0, (float)ay0, (float)ax1, (float)ay1);
      }
    }

    /// <summary>
    /// Get a region object, which describes the plotting area. Used to clip the plotting to
    /// the plotting area.
    /// </summary>
    /// <returns>A region object describing the plotting area.</returns>
    public override Region GetRegion()
    {
      return new Region(new RectangleF(0, 0, (float)_layerWidth, (float)_layerHeight));
    }

    public override object Clone()
    {
      G2DCartesicCoordinateSystem result = new G2DCartesicCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }
  }

  public class G2DPolarCoordinateSystem : G2DCoordinateSystem
  {
    protected double _radius;
    protected double _midX;
    protected double _midY;

    public G2DPolarCoordinateSystem()
    {
    }

    /// <summary>
    /// Returns true if the plot area is orthogonal, i.e. if the x and the y axis are orthogonal to each other.
    /// </summary>
    public override bool IsOrthogonal { get { return true; } }

    /// <summary>
    /// Returns true if the plot coordinates can be calculated as a linear transformation of the physical values.
    /// Returns false if this is for instance a polar diagram. 
    /// </summary>
    public override bool IsAffine { get { return false; } }

    protected override void UpdateAxisInfo()
    {
      int horzAx;
      int vertAx;
      bool vertRev;
      bool horzRev;

      if (_isXYInterchanged)
      {
        horzAx = 1;
        vertAx = 0;
        vertRev = _isXreverse;
        horzRev = _isYreverse;
      }
      else
      {
        horzAx = 0;
        vertAx = 1;
        vertRev = _isYreverse;
        horzRev = _isXreverse;
      }

      _axisStyleInformation.Clear();

      A2DAxisStyleInformation info;

      // Right
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(vertAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "RightDirection";
      info.NameOfLeftSide = vertRev ? "Below" : "Above";
      info.NameOfRightSide = vertRev ? "Above" : "Below";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Left : A2DAxisSide.Right;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Left
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(vertAx, 0.5));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "LeftDirection";
      info.NameOfLeftSide = vertRev ? "Above" : "Below";
      info.NameOfRightSide = vertRev ? "Below" : "Above";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Right : A2DAxisSide.Left;

      // Top
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(vertAx, horzRev ? 0.75 : 0.25));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopDirection";
      info.NameOfLeftSide = vertRev ? "Right" : "Left";
      info.NameOfRightSide = vertRev ? "Left" : "Right";
      info.PreferedLabelSide = vertRev ?  A2DAxisSide.Right : A2DAxisSide.Left;

      // Bottom
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(vertAx, horzRev ? 0.25 : 0.75));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomDirection";
      info.NameOfLeftSide = vertRev ? "Left" : "Right";
      info.NameOfRightSide = vertRev ? "Right" : "Left";
      info.PreferedLabelSide = vertRev ?  A2DAxisSide.Left : A2DAxisSide.Right ;

      // Outer circle
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(horzAx, vertRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "OuterCircle";
      info.NameOfLeftSide = horzRev ? "Outer" : "Inner";
      info.NameOfRightSide = horzRev ? "Inner" : "Outer";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Left : A2DAxisSide.Right;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Inner circle
      info = new A2DAxisStyleInformation(new A2DAxisStyleIdentifier(horzAx, vertRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Inner circle";
      info.NameOfLeftSide = horzRev ? "Inner" : "Outer";
      info.NameOfRightSide = horzRev ? "Outer" : "Inner";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Right : A2DAxisSide.Left;
    }

    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LogicalToLayerCoordinates(double rx, double ry, out double xlocation, out double ylocation)
    {
      if (_isXreverse)
        rx = 1 - rx;
      if (_isYreverse)
        ry = 1 - ry;
      if (_isXYInterchanged)
      {
        double hr = rx;
        rx = ry;
        ry = hr;
      }

      double phi = rx * 2 * Math.PI;
      double rad = _radius * ry;
      xlocation = _midX + rad*Math.Cos(phi);
      ylocation = _midY - rad*Math.Sin(phi);
      return !double.IsNaN(xlocation) && !double.IsNaN(ylocation);
    }

    public override bool LogicalToLayerCoordinatesAndDirection(
      double rx0, double ry0, double rx1, double ry1,
      double t,
      out double ax, out double ay, out double adx, out double ady)
    {
      if (_isXreverse)
      {
        rx0 = 1 - rx0;
        rx1 = 1 - rx1;
      }
      if (_isYreverse)
      {
        ry0 = 1 - ry0;
        ry1 = 1 - ry1;
      }
      if (_isXYInterchanged)
      {
        double hr0 = rx0;
        rx0 = ry0;
        ry0 = hr0;

        double hr1 = rx1;
        rx1 = ry1;
        ry1 = hr1;
      }

      double rx = rx0 + t * (rx1 - rx0);
      double ry = ry0 + t * (ry1 - ry0);
      double phi = -2* Math.PI * rx;
      double rad = _radius * ry;

      ax = _midX + rad * Math.Cos(phi);
      ay = _midY + rad * Math.Sin(phi);

      adx = _radius * ((ry1 - ry0) * Math.Cos(phi) + 2 * Math.PI * (rx1 - rx0) * ry * Math.Sin(phi));
      ady = _radius * ((ry1 - ry0) * Math.Sin(phi) - 2 * Math.PI * (rx1 - rx0) * ry * Math.Cos(phi));
   
      return !double.IsNaN(ax) && !double.IsNaN(ay);
    }



    /// <summary>
    /// Calculates from two layer coordinate values (in points usually) the relative coordinates of the point (between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LayerToLogicalCoordinates(double xlocation, double ylocation, out double rx, out double ry)
    {
      double wx = xlocation -_midX;
      double wy = -ylocation + _midY;
      if (wx == 0 && wy == 0)
      {
        rx = 0;
        ry = 0;
      }
      else
      {
        rx = Math.Atan2(wy, wx)/(2*Math.PI);
        ry = 2*Math.Sqrt(wx * wx + wy * wy)/_radius;
       }

      if (_isXreverse)
        rx = 1 - rx;
      if (_isYreverse)
        ry = 1 - ry;
      if (_isXYInterchanged)
      {
        double hr = rx;
        rx = ry;
        ry = hr;
      }

      return !double.IsNaN(rx) && !double.IsNaN(ry);
    }

    public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath g, double rx0, double ry0, double rx1, double ry1)
    {
      double ax0, ax1, ay0, ay1;
      if (LogicalToLayerCoordinates(rx0, ry0, out ax0, out ay0) && LogicalToLayerCoordinates(rx1, ry1, out ax1, out ay1))
      {
        if (((rx0 == rx1) && !_isXYInterchanged) || ((ry0 == ry1) && _isXYInterchanged))
        {
          g.AddLine((float)ax0, (float)ay0, (float)ax1, (float)ay1);
        }
        if (((ry0 == ry1) && !_isXYInterchanged) || ((rx0 == rx1) && _isXYInterchanged))
        {
          double startAngle = 180 * Math.Atan2(_midY - ay0, ax0 - _midX) / Math.PI ;
          double sweepAngle;
          if (_isXYInterchanged)
          {
            sweepAngle = (ry1 - ry0) * 360;
            if (_isYreverse)
              sweepAngle = -sweepAngle;
          }
          else
          {
            sweepAngle = (rx1 - rx0) * 360;
            if (_isXreverse)
              sweepAngle = -sweepAngle;
          }
          double r = Calc.RMath.Hypot(_midY - ay0, ax0 - _midX);
          if(r>0)
            g.AddArc((float)(_midX - r), (float)(_midY - r), (float)(2 * r), (float)(2 * r), (float)startAngle, (float)sweepAngle);
        }
        else
        {
          int points = _isXYInterchanged ? (int)(Math.Abs(ry1 - ry0) * 360) : (int)(Math.Abs(rx1 - rx0) * 360);
          points = Math.Max(1,Math.Min(points, 3600)); // in case there is a rotation more than one turn limit the number of points
          PointF[] pts = new PointF[points + 1];
          for (int i = 0; i <= points; i++)
          {
            double rx = rx0 + i * (rx1 - rx0) / points;
            double ry = ry0 + i * (ry1 - ry0) / points;
            double ax, ay;
            LogicalToLayerCoordinates(rx, ry, out ax, out ay);
            pts[i] = new PointF((float)ax, (float)ay);
          }
          g.AddLines(pts);
        }
      }
    }

 

    /// <summary>
    /// Get a region object, which describes the plotting area. Used to clip the plotting to
    /// the plotting area.
    /// </summary>
    /// <returns>A region object describing the plotting area.</returns>
    public override Region GetRegion()
    {
      GraphicsPath path = new GraphicsPath();
      path.AddEllipse((float)(_midX-_radius),(float)(_midY-_radius),(float)(2*_radius),(float)(2*_radius));
      return new Region(path);
    }

    public override void UpdateAreaSize(SizeF size)
    {
      base.UpdateAreaSize(size);
      _midX = _layerWidth / 2;
      _midY = _layerHeight / 2;
      _radius = Math.Min(_midX,_midY);
    }

    public override void CopyFrom(G2DCoordinateSystem bfrom)
    {
      base.CopyFrom(bfrom);

      G2DPolarCoordinateSystem from = (G2DPolarCoordinateSystem)bfrom;
      _radius = from._radius;
      _midX = from._midX;
      _midY = from._midY;
    }

    public override object Clone()
    {
      G2DPolarCoordinateSystem result = new G2DPolarCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }
  }
}
