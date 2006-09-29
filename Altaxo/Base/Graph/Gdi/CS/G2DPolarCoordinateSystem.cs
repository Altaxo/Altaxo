using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.CS
{
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
      info = new A2DAxisStyleInformation(new CSLineID(vertAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "RightDirection";
      info.NameOfLeftSide = vertRev ? "Below" : "Above";
      info.NameOfRightSide = vertRev ? "Above" : "Below";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Left : A2DAxisSide.Right;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Left
      info = new A2DAxisStyleInformation(new CSLineID(vertAx, 0.5));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "LeftDirection";
      info.NameOfLeftSide = vertRev ? "Above" : "Below";
      info.NameOfRightSide = vertRev ? "Below" : "Above";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Right : A2DAxisSide.Left;

      // Top
      info = new A2DAxisStyleInformation(new CSLineID(vertAx, horzRev ? 0.75 : 0.25));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopDirection";
      info.NameOfLeftSide = vertRev ? "Right" : "Left";
      info.NameOfRightSide = vertRev ? "Left" : "Right";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Right : A2DAxisSide.Left;

      // Bottom
      info = new A2DAxisStyleInformation(new CSLineID(vertAx, horzRev ? 0.25 : 0.75));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomDirection";
      info.NameOfLeftSide = vertRev ? "Left" : "Right";
      info.NameOfRightSide = vertRev ? "Right" : "Left";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Left : A2DAxisSide.Right;

      // Outer circle
      info = new A2DAxisStyleInformation(new CSLineID(horzAx, vertRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "OuterCircle";
      info.NameOfLeftSide = horzRev ? "Outer" : "Inner";
      info.NameOfRightSide = horzRev ? "Inner" : "Outer";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Left : A2DAxisSide.Right;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Inner circle
      info = new A2DAxisStyleInformation(new CSLineID(horzAx, vertRev ? 1 : 0));
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
      xlocation = _midX + rad * Math.Cos(phi);
      ylocation = _midY - rad * Math.Sin(phi);
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
      double phi = -2 * Math.PI * rx;
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
      double wx = xlocation - _midX;
      double wy = -ylocation + _midY;
      if (wx == 0 && wy == 0)
      {
        rx = 0;
        ry = 0;
      }
      else
      {
        rx = Math.Atan2(wy, wx) / (2 * Math.PI);
        ry = 2 * Math.Sqrt(wx * wx + wy * wy) / _radius;
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
          double startAngle = 180 * Math.Atan2(_midY - ay0, ax0 - _midX) / Math.PI;
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
          if (r > 0)
            g.AddArc((float)(_midX - r), (float)(_midY - r), (float)(2 * r), (float)(2 * r), (float)startAngle, (float)sweepAngle);
        }
        else
        {
          int points = _isXYInterchanged ? (int)(Math.Abs(ry1 - ry0) * 360) : (int)(Math.Abs(rx1 - rx0) * 360);
          points = Math.Max(1, Math.Min(points, 3600)); // in case there is a rotation more than one turn limit the number of points
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
