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
      base.CopyFrom(fromb);
      if (fromb is G2DPolarCoordinateSystem)
      {
        G2DPolarCoordinateSystem from = (G2DPolarCoordinateSystem)fromb;
        this._isXYInterchanged = from._isXYInterchanged;
        this._isXreverse = from._isXreverse;
        this._isYreverse = from._isYreverse;

        this._radius = from._radius;
        this._midX = from._midX;
        this._midY = from._midY;
      }
    }

    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPolarCoordinateSystem), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DPolarCoordinateSystem s = (G2DPolarCoordinateSystem)obj;

        info.AddValue("Rotation", 0.0);
        info.AddValue("XYInterchanged", s._isXYInterchanged);
        info.AddValue("XReverse", s._isXreverse);
        info.AddValue("YReverse", s._isYreverse);
      }
      protected virtual G2DPolarCoordinateSystem SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DPolarCoordinateSystem s = (o == null ? new G2DPolarCoordinateSystem() : (G2DPolarCoordinateSystem)o);

        double rotation = info.GetDouble("Rotation");
        s._isXYInterchanged = info.GetBoolean("XYInterchanged");
        s._isXreverse = info.GetBoolean("XReverse");
        s._isYreverse = info.GetBoolean("YReverse");

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        G2DPolarCoordinateSystem s = SDeserialize(o, info, parent);
        return s;
      }
    }
       #endregion
    #endregion

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
        _isXYInterchanged = value;
        ClearCachedObjects();
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
        _isXreverse = value;
        ClearCachedObjects();
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
        _isYreverse = value;
        ClearCachedObjects();
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

      if (null != _axisStyleInformation)
        _axisStyleInformation.Clear();
      else
        _axisStyleInformation = new List<CSAxisInformation>();

      CSAxisInformation info;

      // Right
      info = new CSAxisInformation(new CSLineID(vertAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "RightDirection";
      info.NameOfFirstUpSide = horzRev ? "Below" : "Above";
      info.NameOfFirstDownSide = horzRev ? "Above" : "Below";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Left
      info = new CSAxisInformation(new CSLineID(vertAx, 0.5));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "LeftDirection";
      info.NameOfFirstUpSide = horzRev ? "Above" : "Below";
      info.NameOfFirstDownSide = horzRev ? "Below" : "Above";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp;

      // Top
      info = new CSAxisInformation(new CSLineID(vertAx, horzRev ? 0.75 : 0.25));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopDirection";
      info.NameOfFirstUpSide = horzRev ? "Right" : "Left";
      info.NameOfFirstDownSide = horzRev ? "Left" : "Right";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp;

      // Bottom
      info = new CSAxisInformation(new CSLineID(vertAx, horzRev ? 0.25 : 0.75));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomDirection";
      info.NameOfFirstUpSide = horzRev ? "Left" : "Right";
      info.NameOfFirstDownSide = horzRev ? "Right" : "Left";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;

      // Outer circle
      info = new CSAxisInformation(new CSLineID(horzAx, vertRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "OuterCircle";
      info.NameOfFirstDownSide = vertRev ? "Outer" : "Inner";
      info.NameOfFirstUpSide = vertRev ? "Inner" : "Outer";
      info.PreferedLabelSide = vertRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Inner circle
      info = new CSAxisInformation(new CSLineID(horzAx, vertRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Inner circle";
      info.NameOfFirstDownSide = vertRev ? "Inner" : "Outer";
      info.NameOfFirstUpSide = vertRev ? "Outer" : "Inner";
      info.PreferedLabelSide = vertRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;
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
        r.RY = 2 * Math.Sqrt(wx * wx + wy * wy) / _radius;
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
      double ax0, ax1, ay0, ay1;
      if (LogicalToLayerCoordinates(r0, out ax0, out ay0) && LogicalToLayerCoordinates(r1, out ax1, out ay1))
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
          PointF[] pts = new PointF[points + 1];
          for (int i = 0; i <= points; i++)
          {
            Logical3D r = new Logical3D(r0.RX + i * (r1.RX - r0.RX) / points, r0.RY + i * (r1.RY - r0.RY) / points);
            double ax, ay;
            LogicalToLayerCoordinates(r, out ax, out ay);
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
      path.AddEllipse((float)(_midX - _radius), (float)(_midY - _radius), (float)(2 * _radius), (float)(2 * _radius));
      return new Region(path);
    }

    public override void UpdateAreaSize(SizeF size)
    {
      base.UpdateAreaSize(size);
      _midX = _layerWidth / 2;
      _midY = _layerHeight / 2;
      _radius = Math.Min(_midX, _midY);
    }

   

    public override object Clone()
    {
      G2DPolarCoordinateSystem result = new G2DPolarCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }
  }
}
