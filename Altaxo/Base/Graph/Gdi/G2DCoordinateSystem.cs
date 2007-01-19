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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi
{
  [Serializable]
  public abstract class G2DCoordinateSystem 
    :
    ICloneable,
    Main.IDocumentNode,
    Main.IChangedEventSource
  {
    protected double _layerWidth;
    protected double _layerHeight;

    protected List<CSAxisInformation> _axisStyleInformation = new List<CSAxisInformation>();

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
      this._layerWidth = from._layerWidth;
      this._layerHeight = from._layerHeight;
      this._axisStyleInformation.Clear();
      
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
    /// Returns true when this is a 3D coordinate system. Returns false in all other cases.
    /// </summary>
    public abstract bool Is3D { get; }


    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="r">The logical position value.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public abstract bool LogicalToLayerCoordinates(Logical3D r, out double xlocation, out double ylocation);
  

    /// <summary>
    /// Converts logical coordinates along an isoline to layer coordinates and the appropriate derivative.
    /// </summary>
    /// <param name="r0">Logical position of starting point of the isoline.</param>
    /// <param name="r1">Logical position of end point of the isoline.</param>
    /// <param name="t">Parameter between 0 and 1 that determines the point on the isoline.
    /// A value of 0 denotes the starting point of the isoline, a value of 1 the end point. The logical
    /// coordinates are linear interpolated between starting point and end point.</param>
    /// <param name="ax">Layer coordinate x of the isoline point.</param>
    /// <param name="ay">Layer coordinate y of the isoline point.</param>
    /// <param name="adx">Derivative of layer coordinate x with respect to parameter t at the point (ax,ay).</param>
    /// <param name="ady">Derivative of layer coordinate y with respect to parameter t at the point (ax,ay).</param>
    /// <returns>True if the conversion was sucessfull, otherwise false.</returns>
    public abstract bool LogicalToLayerCoordinatesAndDirection(
      Logical3D r0, Logical3D r1,
      double t,
      out double ax, out double ay, out double adx, out double ady);

    /// <summary>
    /// Calculates from two  coordinates of a point the logical values (values between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">The x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">The y coordinate of the converted value (for instance location).</param>
    /// <param name="r">The computed logical position value.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible. For 3D coordinate systems,
    /// the relative values of x and y with z=0 should be returned.</returns>
    public abstract bool LayerToLogicalCoordinates(double xlocation, double ylocation, out Logical3D r);

    /// <summary>
    /// Gets a iso line in a path object.
    /// </summary>
    /// <param name="path">The graphics path.</param>
    /// <param name="r0">Starting position in logical coordinates.</param>
    /// <param name="r1">End position in logical coordinates.</param>
    public abstract void GetIsoline(System.Drawing.Drawing2D.GraphicsPath path, Logical3D r0, Logical3D r1);

    /// <summary>
    /// Fills the list of axis information with new values.
    /// </summary>
    protected abstract void UpdateAxisInfo();

    protected virtual void ClearCachedObjects()
    {
      this._axisStyleInformation = null;
    }

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
    /// <param name="r0">Starting point in logical coordinates.</param>
    /// <param name="r1">End point in logical coordinates.</param>
    public virtual void DrawIsoline(System.Drawing.Graphics g, System.Drawing.Pen pen, Logical3D r0, Logical3D r1)
    {
      using (GraphicsPath path = new GraphicsPath())
      {
        GetIsoline(path, r0, r1);
        g.DrawPath(pen, path);
      }
    }

    

    /// <summary>
    /// Draws an isoline beginning from a plane to the given point.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="id">The logical plane to start drawing from.</param>
    /// <param name="r">Logical coordinates of the end point.</param>
    public virtual void GetIsolineFromPlaneToPoint(GraphicsPath path, CSPlaneID id, Logical3D r)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        GetIsoline(path, new Logical3D(id.LogicalValue, r.RY, r.RZ), r);
      }
      else if (id.PerpendicularAxisNumber == 1)
      {
        GetIsoline(path, new Logical3D(r.RX, id.LogicalValue, r.RZ), r);
      }
      else
      {
        GetIsoline(path, new Logical3D(r.RX, r.RY, id.LogicalValue), r);
      }
    }


   

    /// <summary>
    /// Draws an isoline beginning from a given point to the axis.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="r">Logical coordinate of the start point.</param>
    /// <param name="id">The logical plane to end the isoline.</param>
    public virtual void GetIsolineFromPointToPlane(GraphicsPath path, Logical3D r, CSPlaneID id)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        GetIsoline(path, r, new Logical3D(id.LogicalValue, r.RY, r.RZ));
      }
      else if (id.PerpendicularAxisNumber == 1)
      {
        GetIsoline(path, r, new Logical3D(r.RX, id.LogicalValue,r.RZ));
      }
      else
      {
        GetIsoline(path, r, new Logical3D(r.RX, r.RY, id.LogicalValue));
      }
    }

  


    /// <summary>
    /// Draws an isoline beginning from a given point to a plane.
    /// </summary>
    /// <param name="g">Graphics to draw the isoline to.</param>
    /// <param name="pen">The pen to use.</param>
    /// <param name="r">Logical coordinate of the start point.</param>
    /// <param name="id">The logical plane to end the isoline.</param>
    public virtual void DrawIsolineFromPointToPlane(Graphics g, System.Drawing.Pen pen, Logical3D r, CSPlaneID id)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        DrawIsoline(g, pen, r, new Logical3D(id.LogicalValue, r.RY, r.RZ));
      }
      else if (id.PerpendicularAxisNumber == 1)
      {
        DrawIsoline(g, pen, r, new Logical3D(r.RX, id.LogicalValue, r.RZ));
      }
      else
      {
        DrawIsoline(g, pen, r, new Logical3D(r.RX, r.RY, id.LogicalValue));
      }
    }


  


    /// <summary>
    /// Draws an isoline on a plane beginning from r0 to r1. For r0,r1 either ry0,ry1 is used (if it is an x-axis),
    /// otherwise rx0,rx1 is used. The other parameter pair is not used.
    /// </summary>
    /// <param name="path">Graphics path to fill with the isoline.</param>
    /// <param name="r0">Logical coordinate of the start point.</param>
    /// <param name="r1">Logical coordinate of the end point.</param>
    /// <param name="id">The axis to end the isoline.</param>
    public virtual void GetIsolineOnPlane(GraphicsPath path, CSPlaneID id, Logical3D r0, Logical3D r1)
    {
      if (id.PerpendicularAxisNumber == 0)
      {
        GetIsoline(path, new Logical3D(id.LogicalValue, r0.RY, r0.RZ), new Logical3D(id.LogicalValue, r1.RY, r1.RZ));
      }
      else if (id.PerpendicularAxisNumber == 1)
      {
        GetIsoline(path, new Logical3D(r0.RX, id.LogicalValue, r0.RZ), new Logical3D( r1.RX, id.LogicalValue, r1.RZ));
      }
      else
      {
        GetIsoline(path, new Logical3D(r0.RX, r0.RY, id.LogicalValue), new Logical3D(r1.RX, r1.RY, id.LogicalValue));
      }
    }

    public PointF GetPointOnPlane(CSPlaneID id, Logical3D r)
    {
      double x, y;
      if (id.PerpendicularAxisNumber == 0)
        LogicalToLayerCoordinates(new Logical3D(id.LogicalValue, r.RY,r.RZ), out x, out y);
      else if(id.PerpendicularAxisNumber == 1)
        LogicalToLayerCoordinates(new Logical3D(r.RX, id.LogicalValue,r.RZ), out x, out y);
      else
        LogicalToLayerCoordinates(new Logical3D(r.RX, r.RY, id.LogicalValue), out x, out y);

      return new PointF((float)x, (float)y);
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
        GetIsoline(path, new Logical3D(r0, id.LogicalValueOtherFirst,id.LogicalValueOtherSecond), new Logical3D(r1, id.LogicalValueOtherFirst,id.LogicalValueOtherSecond));
      }
      else if (id.ParallelAxisNumber == 1)
      {
        GetIsoline(path, new Logical3D(id.LogicalValueOtherFirst, r0, id.LogicalValueOtherSecond), new Logical3D(id.LogicalValueOtherFirst, r1,id.LogicalValueOtherSecond));
      }
      else
      {
        GetIsoline(path, new Logical3D(id.LogicalValueOtherFirst, id.LogicalValueOtherSecond, r0), new Logical3D(id.LogicalValueOtherFirst, id.LogicalValueOtherSecond, r1));
      }
    }
    /// <summary>
    /// Get a line along the axis designated by the argument id from the logical values r0 to r1.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="pen">The pen required to draw the line.</param>
    /// <param name="id">Axis to draw the isoline along.</param>
    /// <param name="r0">Start point of the isoline. The logical value of the other coordinate.</param>
    /// <param name="r1">End point of the isoline. The logical value of the other coordinate.</param>
    public virtual void DrawIsolineFromTo(Graphics g, Pen pen, CSLineID id, double r0, double r1)
    {
      if (id.ParallelAxisNumber == 0)
      {
        DrawIsoline(g, pen, new Logical3D(r0, id.LogicalValueOtherFirst, id.LogicalValueOtherSecond), new Logical3D(r1, id.LogicalValueOtherFirst, id.LogicalValueOtherSecond));
      }
      else if (id.ParallelAxisNumber == 1)
      {
        DrawIsoline(g, pen, new Logical3D(id.LogicalValueOtherFirst, r0, id.LogicalValueOtherSecond), new Logical3D(id.LogicalValueOtherFirst, r1, id.LogicalValueOtherSecond));
      }
      else
      {
        DrawIsoline(g, pen, new Logical3D(id.LogicalValueOtherFirst, id.LogicalValueOtherSecond, r0), new Logical3D(id.LogicalValueOtherFirst, id.LogicalValueOtherSecond, r1));
      }
    }


    /// <summary>
    /// Converts logical coordinates along an isoline to layer coordinates and returns the direction of the isoline at this point.
    /// </summary>
    /// <param name="r0">Logical coordinates of starting point of the isoline.</param>
    /// <param name="r1">Logical coordinates of end point of the isoline.</param>
    /// <param name="t">Parameter between 0 and 1 that determines the point on the isoline.
    /// A value of 0 denotes the starting point of the isoline, a value of 1 the end point. The logical
    /// coordinates are linear interpolated between starting point and end point.</param>
    /// <param name="angle">Angle between direction of the isoline and returned normalized direction vector.</param>
    /// <param name="normalizeddirection">Returns the normalized direction vector,i.e. a vector of norm 1, that
    /// has the angle <paramref name="angle"/> to the tangent of the isoline. </param>
    /// <returns>The location (in layer coordinates) of the isoline point.</returns>
    public PointF GetNormalizedDirection(
      Logical3D r0, Logical3D r1,
      double t,
      double angle,
      out PointF normalizeddirection)
    {
      double ax, ay, adx, ady;
      this.LogicalToLayerCoordinatesAndDirection(
        r0, r1,
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
    /// Converts logical coordinates along an isoline to layer coordinates and returns the direction of the isoline at this point.
    /// </summary>
    /// <param name="r0">Logical starting point of the isoline.</param>
    /// <param name="r1">Logical end point of the isoline.</param>
    /// <param name="t">Parameter between 0 and 1 that determines the point on the isoline.
    /// A value of 0 denotes the starting point of the isoline, a value of 1 the end point. The logical
    /// coordinates are linear interpolated between starting point and end point.</param>
    /// <param name="direction">Logical direction vector.</param>
    /// <param name="normalizeddirection">Returns the normalized direction vector,i.e. a vector of norm 1, that
    /// goes in the logical direction provided by the previous argument. </param>
    /// <returns>The location (in layer coordinates) of the isoline point.</returns>
    public virtual PointF GetNormalizedDirection(
        Logical3D r0, Logical3D r1,
        double t,
        Logical3D direction,
        out PointF normalizeddirection)
    {
      double ax, ay, adx, ady;
      Logical3D rn0 = Logical3D.Interpolate(r0, r1, t);
      Logical3D rn1 = rn0 + direction;
      this.LogicalToLayerCoordinatesAndDirection(rn0, rn1, 0, out ax, out ay, out adx, out ady);
      double hypot = Calc.RMath.Hypot(adx, ady);
      if (0==hypot)
      {
        // then we look a little bit displaced - we might be at the midpoint where the directions are undefined
        double displT = t;
        if (displT < 0.5)
          displT += 1E-6;
        else
          displT -= 1E-6;

        Logical3D displR = Logical3D.Interpolate(r0, r1, displT);
        Logical3D displD = displR + direction;
        double dummyx, dummyy;
        LogicalToLayerCoordinatesAndDirection(displR, displD, 0, out dummyx, out dummyy, out adx, out ady);
        hypot = Calc.RMath.Hypot(adx, ady);
      }

      // Normalize the vector
      if (hypot > 0)
      {
        adx /= hypot;
        ady /= hypot;
      }

      normalizeddirection = new PointF((float)adx, (float)ady);


      return new PointF((float)ax, (float)ay);
    }
    public Logical3D GetLogicalDirection(int parallelAxisNumber, CSAxisSide side)
    {
      switch (side)
      {
        default:
        case CSAxisSide.FirstDown:
          return 0 == parallelAxisNumber ? new Logical3D(0, -1, 0) : new Logical3D(-1, 0, 0);
        case CSAxisSide.FirstUp:
          return 0 == parallelAxisNumber ? new Logical3D(0, 1, 0) : new Logical3D(1, 0, 0);
        case CSAxisSide.SecondDown:
          return 2 == parallelAxisNumber ? new Logical3D(0, -1, 0) : new Logical3D(0, 0, -1);
        case CSAxisSide.SecondUp:
          return 2 == parallelAxisNumber ? new Logical3D(0, 1, 0) : new Logical3D(0, 0, 1);
      }
    }

    /// <summary>
    /// Enumerators all axis style information.
    /// </summary>
    public IEnumerable<CSAxisInformation> AxisStyles
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

    public CSAxisInformation GetAxisStyleInformation(CSLineID styleID)
    {
      if (_axisStyleInformation == null || _axisStyleInformation.Count == 0)
        UpdateAxisInfo();

      // search for the same axis first, then for the style with the nearest logical value
      double minDistance = double.MaxValue;
      CSAxisInformation nearestInfo = null;

      foreach (CSAxisInformation info in this._axisStyleInformation)
      {
        if (styleID.ParallelAxisNumber == info.Identifier.ParallelAxisNumber)
        {
          if (styleID == info.Identifier)
          {
            minDistance = 0;
            nearestInfo = info;
            break;
          }

          double dist = Math.Abs(styleID.LogicalValueOtherFirst - info.Identifier.LogicalValueOtherFirst);
          if(styleID.Is3DIdentifier && info.Identifier.Is3DIdentifier)
            dist += Math.Abs(styleID.LogicalValueOtherSecond - info.Identifier.LogicalValueOtherSecond);

          if (dist < minDistance)
          {
            minDistance = dist;
            nearestInfo = info;
            if (0 == minDistance)
              break; // it can not be smaller than 0
          }
        }

      }

      CSAxisInformation result = new CSAxisInformation(styleID);
      if (nearestInfo == null)
      {
        result.SetDefaultValues();
      }
      else
      {
        result.CopyWithoutIdentifierFrom(nearestInfo);
        if(minDistance!=0)
          result.NameOfAxisStyle += string.Format(" ({0}% offs.)",minDistance*100);

      }

      return result;
    }


    public CSPlaneInformation GetPlaneInformation(CSPlaneID planeID)
    {
      CSLineID lineID = (CSLineID)planeID;
      CSAxisInformation lineInfo = GetAxisStyleInformation(lineID);

      CSPlaneInformation result = new CSPlaneInformation(planeID);
      result.Name = lineInfo.NameOfAxisStyle;
      return result;
    }

    public IEnumerable<CSLineID> GetJoinedAxisStyleIdentifier(IEnumerable<CSLineID> list1, IEnumerable<CSLineID> list2)
    {
      Dictionary<CSLineID, object> dict = new Dictionary<CSLineID, object>();

      foreach (CSAxisInformation info in AxisStyles)
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
          if (null!=id && !dict.ContainsKey(id))
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

      foreach (CSAxisInformation info in AxisStyles)
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
