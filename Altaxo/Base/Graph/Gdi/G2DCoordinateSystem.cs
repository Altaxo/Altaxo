using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi
{
  public abstract class G2DCoordinateSystem 
    :
    ICloneable,
    Main.IDocumentNode,
    Main.IChangedEventSource
  {
  
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

    [NonSerialized]
    object _parent;

    [field: NonSerialized]
    event EventHandler _changed;


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

    public virtual void GetIsolineFromPlaneToPoint(GraphicsPath path, CSPlaneID id, Logical3D r)
    {
      GetIsolineFromPlaneToPoint(path, id, r.RX, r.RY, r.RZ);
    }

    /// <summary>
    /// Draws an isoline beginning from a plane to the given point.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="id">The axis to start drawing.</param>
    /// <param name="rx">Logical x coordinate of the end point.</param>
    /// <param name="ry">Logical y coordinate of the end point.</param>
    /// <param name="rz">Logical z coordinate of the end point (not used for 2D coordinate systems).</param>
    public virtual void GetIsolineFromPlaneToPoint(GraphicsPath path, CSPlaneID id, double rx, double ry, double rz)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        GetIsoline(path, id.LogicalValue, ry, rx, ry);
      }
      else
      {
        GetIsoline(path, rx, id.LogicalValue, rx, ry);
      }
    }


    public virtual void GetIsolineFromPointToPlane(GraphicsPath path, CSPlaneID id, Logical3D r)
    {
      GetIsolineFromPointToPlane(path, r.RX, r.RY, id);
    }

    /// <summary>
    /// Draws an isoline beginning from a given point to the axis.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="rx">Logical x coordinate of the start point.</param>
    /// <param name="ry">Logical y coordinate of the start point.</param>
    /// <param name="id">The axis to end the isoline.</param>
    public virtual void GetIsolineFromPointToPlane(GraphicsPath path, double rx, double ry, CSPlaneID id)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        GetIsoline(path, rx, ry, id.LogicalValue, ry);
      }
      else
      {
        GetIsoline(path, rx, ry, rx, id.LogicalValue);
      }
    }

    public virtual void DrawIsolineFromPointToPlane(Graphics g, System.Drawing.Pen pen, CSPlaneID id, Logical3D r)
    {
      DrawIsolineFromPointToPlane(g, pen, r.RX, r.RY, id);
    }


    /// <summary>
    /// Draws an isoline beginning from a given point to a plane.
    /// </summary>
    /// <param name="g">Graphics to draw the isoline to.</param>
    /// <param name="pen">The pen to use.</param>
    /// <param name="rx">Logical x coordinate of the start point.</param>
    /// <param name="ry">Logical y coordinate of the start point.</param>
    /// <param name="id">The plane to end the isoline.</param>
    public virtual void DrawIsolineFromPointToPlane(Graphics g, System.Drawing.Pen pen, double rx, double ry, CSPlaneID id)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        DrawIsoline(g, pen, rx, ry, id.LogicalValue, ry);
      }
      else
      {
        DrawIsoline(g, pen, rx, ry, rx, id.LogicalValue);
      }
    }


    public virtual void GetIsolineOnPlane(GraphicsPath path, CSPlaneID id, Logical3D r0, Logical3D r1)
    {
      GetIsolineOnPlane(path, id, r0.RX, r0.RY, r1.RX, r1.RY);
    }


    /// <summary>
    /// Draws an isoline on a plane beginning from r0 to r1. For r0,r1 either ry0,ry1 is used (if it is an x-axis),
    /// otherwise rx0,ry1 is used. The other parameter pair is not used.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="rx0">Logical x coordinate of the start point.</param>
    /// <param name="ry1">Logical y coordinate of the start point.</param>
    /// <param name="rx0">Logical x coordinate of the start point.</param>
    /// <param name="ry1">Logical y coordinate of the start point.</param>
    /// <param name="id">The axis to end the isoline.</param>
    public virtual void GetIsolineOnPlane(GraphicsPath path, CSPlaneID id, double rx0, double ry0, double rx1, double ry1)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        GetIsoline(path, id.LogicalValue, ry0, id.LogicalValue, ry1);
      }
      else
      {
        GetIsoline(path, rx0, id.LogicalValue, rx1, id.LogicalValue);
      }
    }

    public PointF GetPointOnPlane(CSPlaneID id, Logical3D r)
    {
      if (id.PerpendicularAxisNumber == 0)
        return LogicalToLayerCoordinates(id.LogicalValue, r.RY);
      else
        return LogicalToLayerCoordinates(r.RX, id.LogicalValue);
    }


    /// <summary>
    /// Get a line along the axis designated by the argument id from the logical values r0 to r1.
    /// </summary>
    /// <param name="path">Graphics path.</param>
    /// <param name="id">Axis to draw the isoline along.</param>
    /// <param name="r0">Start point of the isoline. The logical value of the other coordinate.</param>
    /// <param name="r1">End point of the isoline. The logical value of the other coordinate.</param>
    public virtual void GetIsolineFromTo(GraphicsPath path, CSLineID id, double r0, double r1)
    {
      if (id.ParallelAxisNumber == 0)
      {
        GetIsoline(path, r0, id.LogicalValueOtherFirst, r1, id.LogicalValueOtherFirst);
      }
      else
      {
        GetIsoline(path, id.LogicalValueOtherFirst, r0, id.LogicalValueOtherFirst, r1);
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
    public int IndexOfAxisStyle(CSLineID id)
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

    public A2DAxisStyleInformation GetAxisStyleInformation(CSLineID styleID)
    {
      if (_axisStyleInformation == null || _axisStyleInformation.Count == 0)
        UpdateAxisInfo();

      // search for the same axis first, then for the style with the nearest logical value
      double minDistance = double.MaxValue;
      A2DAxisStyleInformation nearestInfo = null;

      foreach (A2DAxisStyleInformation info in this._axisStyleInformation)
      {
        if (styleID.ParallelAxisNumber == info.Identifier.ParallelAxisNumber)
        {
          if (styleID == info.Identifier)
            return info; // this covers also situations when the axis uses physical values

          double dist = Math.Abs(styleID.LogicalValueOtherFirst - info.Identifier.LogicalValueOtherFirst);
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


    public CSPlaneInformation GetPlaneInformation(CSPlaneID planeID)
    {
      CSLineID lineID = (CSLineID)planeID;
      A2DAxisStyleInformation lineInfo = GetAxisStyleInformation(lineID);

      CSPlaneInformation result = new CSPlaneInformation(planeID);
      result.Name = lineInfo.NameOfAxisStyle;
      return result;
    }

    public IEnumerable<CSLineID> GetJoinedAxisStyleIdentifier(IEnumerable<CSLineID> list1, IEnumerable<CSLineID> list2)
    {
      Dictionary<CSLineID, object> dict = new Dictionary<CSLineID, object>();

      foreach (A2DAxisStyleInformation info in AxisStyles)
      {
        dict.Add(info.Identifier, null);
        yield return info.Identifier;
      }

      if (list1 != null)
      {
        foreach (CSLineID id in list1)
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
        foreach (CSLineID id in list2)
        {
          if (!dict.ContainsKey(id))
          {
            dict.Add(id, null);
            yield return id;
          }
        }
      }
    }



    public IEnumerable<CSPlaneID> GetJoinedPlaneIdentifier(IEnumerable<CSLineID> list1, IEnumerable<CSPlaneID> list2)
    {
      Dictionary<CSPlaneID, object> dict = new Dictionary<CSPlaneID, object>();

      foreach (A2DAxisStyleInformation info in AxisStyles)
      {
        CSPlaneID p1 = (CSPlaneID)info.Identifier;
        dict.Add(p1, null);
        yield return p1;
      }

      if (list1 != null)
      {
        foreach (CSLineID id in list1)
        {
          CSPlaneID p2 = (CSPlaneID)id;
          if (!dict.ContainsKey(p2))
          {
            dict.Add(p2, null);
            yield return p2;
          }
        }
      }

      if (list2 != null)
      {
        foreach (CSPlaneID id in list2)
        {
          if (null!=id && !dict.ContainsKey(id))
          {
            dict.Add(id, null);
            yield return id;
          }
        }
      }
    }

    #region IDocumentNode Members

    /// <summary>
    /// Get/ sets the parent of this object.
    /// </summary>
    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return this.GetType().ToString(); }
    }

    #endregion

    #region IChangedEventSource Members

    event EventHandler Altaxo.Main.IChangedEventSource.Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }

    public virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != _changed)
        _changed(this, EventArgs.Empty);
    }

    #endregion
  }
}
