#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Altaxo.Graph.Graph3D;

namespace Altaxo.Gui
{
  using Altaxo.Geometry;
  using Altaxo.Graph;

  public static class PathGeometryHelper
  {
    public static PolygonClosedD2D GetGlyphPolygon(PathFigure figure, bool reverseY, double deviationAngleInDegrees, double deviationAbsolute)
    {
      PolyBezierSegment polyBezierSegment;
      PolyLineSegment polyLineSegment;

      var flattenedFigure = figure.GetFlattenedPathFigure();

      if (flattenedFigure.Segments.Count != 1)
        throw new NotSupportedException();

      var points = (flattenedFigure.Segments[0] as PolyLineSegment).Points.Select(p => ToAltaxo(p, reverseY)).ToArray();

      var sharpPoints = new HashSet<PointD2D>();

      Vector endVector;

      GetStartAndEndVector(figure.StartPoint, figure.Segments[0], out var prevEndPoint, out var startVector, out var prevEndVector);
      for (int i = 1; i <= figure.Segments.Count; ++i)
      {
        if (i == figure.Segments.Count && prevEndPoint != figure.StartPoint) // if the end point of the very last segment is not the same than the start point of the figure
        {
          // Consider the straight line segment from the prevPoint to figure.StartPoint in the case that the startpoint and the endpoint of the figure are different
          startVector = endVector = figure.StartPoint - prevEndPoint;
          if (!IsSmoothJoint(startVector, prevEndVector))
            sharpPoints.Add(ToAltaxo(prevEndPoint, reverseY));
          prevEndVector = endVector;
          prevEndPoint = figure.StartPoint;
        }

        PathSegment seg = figure.Segments[i % figure.Segments.Count];
        GetStartAndEndVector(prevEndPoint, seg, out var endPoint, out startVector, out endVector);

        if (!IsSmoothJoint(startVector, prevEndVector))
          sharpPoints.Add(ToAltaxo(prevEndPoint, reverseY));

        if ((polyLineSegment = (seg as PolyLineSegment)) is not null)
        {
          var preP = prevEndPoint;
          for (int j = 0; j < polyLineSegment.Points.Count - 1; ++j)
          {
            if (!IsSmoothJoint(polyLineSegment.Points[j] - preP, polyLineSegment.Points[j + 1] - polyLineSegment.Points[j]))
              sharpPoints.Add(ToAltaxo(polyLineSegment.Points[j], reverseY));
            preP = polyLineSegment.Points[j];
          }

          prevEndPoint = polyLineSegment.Points[polyLineSegment.Points.Count - 1];
        }
        if ((polyBezierSegment = (seg as PolyBezierSegment)) is not null)
        {
          for (int j = 2; j < polyBezierSegment.Points.Count - 1; j += 3)
            if (!IsSmoothJoint(polyBezierSegment.Points[j] - polyBezierSegment.Points[j - 1], polyBezierSegment.Points[j + 1] - polyBezierSegment.Points[j]))
              sharpPoints.Add(ToAltaxo(polyBezierSegment.Points[j], reverseY));
        }

        prevEndVector = endVector;
        prevEndPoint = endPoint;
      }

      return new PolygonClosedD2D(points, sharpPoints);
    }

    private static PointD2D ToAltaxo(Point pt, bool reverseY)
    {
      if (reverseY)
        return new PointD2D(pt.X, -pt.Y);
      else
        return new PointD2D(pt.X, pt.Y);
    }

    public static bool IsSmoothJoint(Vector v1, Vector v2)
    {
      double r = v1.X * v2.X + v1.Y * v2.Y;
      if (r <= 0)
        return false;

      r = r * r / ((v1.X * v1.X + v1.Y * v1.Y) * (v2.X * v2.X + v2.Y * v2.Y));
      return r > 0.9997; // 1 Degree
    }

    public static void GetStartAndEndVector(Point startPoint, PathSegment seg, out Point endPoint, out Vector startVector, out Vector endVector)
    {
      if (seg is LineSegment lineSegment)
      {
        startVector = endVector = lineSegment.Point - startPoint;
        endPoint = lineSegment.Point;
      }
      else if (seg is PolyLineSegment polyLineSegment)
      {
        var pts = polyLineSegment.Points;
        var len = pts.Count;
        startVector = pts[0] - startPoint;
        if (len >= 2)
          startPoint = pts[len - 2];
        endVector = pts[len - 1] - startPoint;
        endPoint = pts[len - 1];
      }
      else if (seg is BezierSegment bezierSegment)
      {
        startVector = bezierSegment.Point1 - startPoint;
        endVector = bezierSegment.Point3 - bezierSegment.Point2;
        endPoint = bezierSegment.Point3;
      }
      else if (seg is PolyBezierSegment polyBezierSegment)
      {
        var pts = polyBezierSegment.Points;
        var len = pts.Count;
        startVector = pts[0] - startPoint;
        endVector = pts[len - 1] - pts[len - 2];
        endPoint = pts[len - 1];
      }
      else
      {
        throw new NotImplementedException();
      }
    }
  }
}
