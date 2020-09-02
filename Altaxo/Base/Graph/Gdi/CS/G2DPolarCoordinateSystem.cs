#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.CS
{
  [Serializable]
  public class G2DPolarCoordinateSystem : G2DCoordinateSystem
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

    protected double _radius;
    protected double _midX;
    protected double _midY;

    /// <summary>
    /// Copies the member variables from another coordinate system.
    /// </summary>
    /// <param name="fromb">The coordinate system to copy from.</param>
    public override void CopyFrom(G2DCoordinateSystem fromb)
    {
      if (object.ReferenceEquals(this, fromb))
        return;

      base.CopyFrom(fromb);
      if (fromb is G2DPolarCoordinateSystem)
      {
        var from = (G2DPolarCoordinateSystem)fromb;
        _isXYInterchanged = from._isXYInterchanged;
        _isXreverse = from._isXreverse;
        _isYreverse = from._isYreverse;

        _radius = from._radius;
        _midX = from._midX;
        _midY = from._midY;
      }
    }

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPolarCoordinateSystem), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (G2DPolarCoordinateSystem)obj;

        info.AddValue("Rotation", 0.0);
        info.AddValue("XYInterchanged", s._isXYInterchanged);
        info.AddValue("XReverse", s._isXreverse);
        info.AddValue("YReverse", s._isYreverse);
      }

      protected virtual G2DPolarCoordinateSystem SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (G2DPolarCoordinateSystem?)o ?? new G2DPolarCoordinateSystem();

        double rotation = info.GetDouble("Rotation");
        s._isXYInterchanged = info.GetBoolean("XYInterchanged");
        s._isXreverse = info.GetBoolean("XReverse");
        s._isYreverse = info.GetBoolean("YReverse");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        G2DPolarCoordinateSystem s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public G2DPolarCoordinateSystem()
    {
    }

    /// <summary>
    /// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
    /// </summary>
    public bool IsXYInterchanged
    {
      get { return _isXYInterchanged; }
      set
      {
        if (_isXYInterchanged != value)
        {
          _isXYInterchanged = value;
          ClearCachedObjects();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Is the direction of the x axis reverse, for instance runs from right to left.
    /// </summary>
    public bool IsXReverse
    {
      get { return _isXreverse; }
      set
      {
        if (_isXreverse != value)
        {
          _isXreverse = value;
          ClearCachedObjects();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Is the direction of the y axis reverse, for instance runs from top to bottom.
    /// </summary>
    public bool IsYReverse
    {
      get { return _isYreverse; }
      set
      {
        if (_isYreverse != value)
        {
          _isYreverse = value;
          ClearCachedObjects();
          EhSelfChanged(EventArgs.Empty);
        }
      }
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

    /// <summary>
    /// Returns true when this is a 3D coordinate system. Returns false in all other cases.
    /// </summary>
    public override bool Is3D { get { return false; } }

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

      if (_axisStyleInformation is not null)
        _axisStyleInformation.Clear();
      else
        _axisStyleInformation = new List<CSAxisInformation>();

      CSAxisInformation info;

      // Right
      info = new CSAxisInformation(
        Identifier: new CSLineID(vertAx, 0),
      NameOfAxisStyle: "RightDirection",
      NameOfFirstUpSide: horzRev ? "Below" : "Above",
      NameOfFirstDownSide: horzRev ? "Above" : "Below",
      PreferredLabelSide: horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown,
      IsShownByDefault: true,
      HasTitleByDefault: true);
      _axisStyleInformation.Add(info);

      // Left
      info = new CSAxisInformation(
        Identifier: new CSLineID(vertAx, 0.5),
      NameOfAxisStyle: "LeftDirection",
      NameOfFirstUpSide: horzRev ? "Above" : "Below",
      NameOfFirstDownSide: horzRev ? "Below" : "Above",
      PreferredLabelSide: horzRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp);
      _axisStyleInformation.Add(info);

      // Top
      info = new CSAxisInformation(
        Identifier: new CSLineID(vertAx, horzRev ? 0.75 : 0.25),
      NameOfAxisStyle: "TopDirection",
      NameOfFirstUpSide: horzRev ? "Right" : "Left",
      NameOfFirstDownSide: horzRev ? "Left" : "Right",
      PreferredLabelSide: horzRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp);
      _axisStyleInformation.Add(info);

      // Bottom
      info = new CSAxisInformation(
        Identifier: new CSLineID(vertAx, horzRev ? 0.25 : 0.75),
      NameOfAxisStyle: "BottomDirection",
      NameOfFirstUpSide: horzRev ? "Left" : "Right",
      NameOfFirstDownSide: horzRev ? "Right" : "Left",
      PreferredLabelSide: horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown);
      _axisStyleInformation.Add(info);

      // Outer circle
      info = new CSAxisInformation(
        Identifier: new CSLineID(horzAx, vertRev ? 0 : 1),
      NameOfAxisStyle: "OuterCircle",
      NameOfFirstDownSide: vertRev ? "Outer" : "Inner",
      NameOfFirstUpSide: vertRev ? "Inner" : "Outer",
      PreferredLabelSide: vertRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp,
      IsShownByDefault: true,
      HasTitleByDefault: true);
      _axisStyleInformation.Add(info);

      // Inner circle
      info = new CSAxisInformation(
        Identifier: new CSLineID(horzAx, vertRev ? 1 : 0),
      NameOfAxisStyle: "Inner circle",
      NameOfFirstDownSide: vertRev ? "Inner" : "Outer",
      NameOfFirstUpSide: vertRev ? "Outer" : "Inner",
      PreferredLabelSide: vertRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown);
      _axisStyleInformation.Add(info);
    }

    private static readonly string[,] _axisNamesNormal = new string[,] { { "Outer", "Inner" }, { "CCW", "CW" } };
    private static readonly string[,] _axisNamesOuterLines = new string[,] { { "Outer", "Inner" }, { "CCW", "CW" } };

    /// <summary>Gets the name of the axis side.</summary>
    /// <param name="id">The axis identifier.</param>
    /// <param name="side">The axis side.</param>
    /// <returns>The name of the axis side for the axis line given by the identifier.</returns>
    public override string GetAxisSideName(CSLineID id, CSAxisSide side)
    {
      bool isX = id.ParallelAxisNumber == 0;
      isX ^= _isXYInterchanged;

      bool isUp = side == CSAxisSide.FirstUp;
      isUp ^= isX ? _isXreverse : _isYreverse;

      bool isOuterLine = (id.LogicalValueOtherFirst == 0 || id.LogicalValueOtherFirst == 1);

      if (isOuterLine)
        isUp ^= (0 == id.LogicalValueOtherFirst);

      if (isOuterLine)
        return _axisNamesOuterLines[isX ? 0 : 1, isUp ? 0 : 1];
      else
        return _axisNamesNormal[isX ? 0 : 1, isUp ? 0 : 1];
    }

    private Matrix2x2 VectorTransformation
    {
      get
      {
        if (_isXYInterchanged)
          return new Matrix2x2(
            0, _isXreverse ? -1 : 1,
            _isYreverse ? -1 : 1, 0);
        else
          return new Matrix2x2(
            _isXreverse ? -1 : 1, 0,
            0, _isYreverse ? -1 : 1);
      }
    }

    public override string GetNameOfPlane(CSPlaneID planeId)
    {
      string name = "";
      if (planeId.UsePhysicalValue)
      {
        switch (planeId.PerpendicularAxisNumber)
        {
          case 0:
            name = string.Format("X = {0}", planeId.PhysicalValue);
            break;

          case 1:
            name = string.Format("Y = {0}", planeId.PhysicalValue);
            break;

          case 2:
            name = string.Format("Z = {0}", planeId.PhysicalValue);
            break;

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        var uv = GetUntransformedAxisPlaneVector(planeId);
        var tv = VectorTransformation.Transform(uv);

        var lv = planeId.LogicalValue;

        if (tv.X == -1 || tv.Y == -1)
          lv = 1 - lv;
        if (Math.Abs(tv.X) == 1) // vector in x-direction
        {
          if (lv == 0)
            name = "Zero degrees";
          else if (lv == 1)
            name = "360 degrees";
          else
            name = string.Format("{0}% between 0 and 360 degrees", lv * 100);
        }
        else if (Math.Abs(tv.Y) == 1) // vector in y-direction
        {
          if (lv == 0)
            name = "Inner circle";
          else if (lv == 1)
            name = "Outer circle";
          else
            name = string.Format("{0}% between inner and outer circle", lv * 100);
        }
        else
        {
          throw new NotImplementedException();
        }
      }
      return name;
    }

    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="r">The logical point to convert.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LogicalToLayerCoordinates(Logical3D r, out double xlocation, out double ylocation)
    {
      if (_isXreverse)
        r.RX = 1 - r.RX;
      if (_isYreverse)
        r.RY = 1 - r.RY;
      if (_isXYInterchanged)
      {
        double hr = r.RX;
        r.RX = r.RY;
        r.RY = hr;
      }

      double phi = r.RX * 2 * Math.PI;
      double rad = _radius * r.RY;
      xlocation = _midX + rad * Math.Cos(phi);
      ylocation = _midY - rad * Math.Sin(phi);
      return !double.IsNaN(xlocation) && !double.IsNaN(ylocation);
    }

    public override bool LogicalToLayerCoordinatesAndDirection(
      Logical3D r0, Logical3D r1,
      double t,
      out double ax, out double ay, out double adx, out double ady)
    {
      if (_isXreverse)
      {
        r0.RX = 1 - r0.RX;
        r1.RX = 1 - r1.RX;
      }
      if (_isYreverse)
      {
        r0.RY = 1 - r0.RY;
        r1.RY = 1 - r1.RY;
      }
      if (_isXYInterchanged)
      {
        double hr0 = r0.RX;
        r0.RX = r0.RY;
        r0.RY = hr0;

        double hr1 = r1.RX;
        r1.RX = r1.RY;
        r1.RY = hr1;
      }

      double rx = r0.RX + t * (r1.RX - r0.RX);
      double ry = r0.RY + t * (r1.RY - r0.RY);
      double phi = -2 * Math.PI * rx;
      double rad = _radius * ry;

      ax = _midX + rad * Math.Cos(phi);
      ay = _midY + rad * Math.Sin(phi);

      adx = _radius * ((r1.RY - r0.RY) * Math.Cos(phi) + 2 * Math.PI * (r1.RX - r0.RX) * ry * Math.Sin(phi));
      ady = _radius * ((r1.RY - r0.RY) * Math.Sin(phi) - 2 * Math.PI * (r1.RX - r0.RX) * ry * Math.Cos(phi));

      return !double.IsNaN(ax) && !double.IsNaN(ay);
    }

    /// <summary>
    /// Calculates from two layer coordinate values (in points usually) the relative coordinates of the point (between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <param name="r">The logical coordinate as the result of the conversion.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LayerToLogicalCoordinates(double xlocation, double ylocation, out Logical3D r)
    {
      r = new Logical3D();
      double wx = xlocation - _midX;
      double wy = -ylocation + _midY;
      if (wx == 0 && wy == 0)
      {
        r.RX = 0;
        r.RY = 0;
      }
      else
      {
        r.RX = Math.Atan2(wy, wx) / (2 * Math.PI);
        r.RY = Math.Sqrt(wx * wx + wy * wy) / _radius;
      }

      if (_isXreverse)
        r.RX = 1 - r.RX;
      if (_isYreverse)
        r.RY = 1 - r.RY;
      if (_isXYInterchanged)
      {
        double hr = r.RX;
        r.RX = r.RY;
        r.RY = hr;
      }

      return !double.IsNaN(r.RX) && !double.IsNaN(r.RY);
    }

    public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath g, Logical3D r0, Logical3D r1)
    {
      if (LogicalToLayerCoordinates(r0, out var ax0, out var ay0) && LogicalToLayerCoordinates(r1, out var ax1, out var ay1))
      {
        // add a line when this is a radial ray
        if (((r0.RX == r1.RX) && !_isXYInterchanged) || ((r0.RY == r1.RY) && _isXYInterchanged))
        {
          g.AddLine((float)ax0, (float)ay0, (float)ax1, (float)ay1);
        }
        // add an arc if this is a tangential ray
        else if (((r0.RY == r1.RY) && !_isXYInterchanged) || ((r0.RX == r1.RX) && _isXYInterchanged))
        {
          double startAngle = 180 * Math.Atan2(_midY - ay0, ax0 - _midX) / Math.PI;
          double sweepAngle;
          if (_isXYInterchanged)
          {
            sweepAngle = (r1.RY - r0.RY) * 360;
            if (_isYreverse)
              sweepAngle = -sweepAngle;
          }
          else
          {
            sweepAngle = (r1.RX - r0.RX) * 360;
            if (_isXreverse)
              sweepAngle = -sweepAngle;
          }
          double r = Calc.RMath.Hypot(_midY - ay0, ax0 - _midX);
          if (r > 0)
            g.AddArc((float)(_midX - r), (float)(_midY - r), (float)(2 * r), (float)(2 * r), (float)-startAngle, (float)-sweepAngle);
        }
        else // if it is neither radial nor tangential
        {
          int points = _isXYInterchanged ? (int)(Math.Abs(r1.RY - r0.RY) * 360) : (int)(Math.Abs(r1.RX - r0.RX) * 360);
          points = Math.Max(1, Math.Min(points, 3600)); // in case there is a rotation more than one turn limit the number of points
          var pts = new PointF[points + 1];
          for (int i = 0; i <= points; i++)
          {
            var r = new Logical3D(r0.RX + i * (r1.RX - r0.RX) / points, r0.RY + i * (r1.RY - r0.RY) / points);
            LogicalToLayerCoordinates(r, out var ax, out var ay);
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
      var path = new GraphicsPath();
      path.AddEllipse((float)(_midX - _radius), (float)(_midY - _radius), (float)(2 * _radius), (float)(2 * _radius));
      return new Region(path);
    }

    public override void UpdateAreaSize(PointD2D size)
    {
      base.UpdateAreaSize(size);
      _midX = _layerWidth / 2;
      _midY = _layerHeight / 2;
      _radius = Math.Min(_midX, _midY);
    }

    public override object Clone()
    {
      var result = new G2DPolarCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }
  }
}
