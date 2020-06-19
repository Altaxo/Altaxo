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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Altaxo.Geometry
{
  using Altaxo.Graph;

  public class BezierCurveFlattener
  {
    private double _angleCriterium;
    private double _toleranceCriterium; // tolerance here is 16*tol^2 (tol is the maximum absolute allowed deviation of the curve from the approximation)

    public BezierCurveFlattener(double angleInDegree, double absoluteTolerance)
    {
      if (!(angleInDegree > 0) || !(angleInDegree < 90))
        throw new ArgumentOutOfRangeException("Value should be > 0 and < 90", nameof(angleInDegree));

      if (!(absoluteTolerance > 0))
        throw new ArgumentOutOfRangeException("Value should be >0", nameof(absoluteTolerance));

      _toleranceCriterium = 16 * absoluteTolerance * absoluteTolerance;
      _angleCriterium = Math.Cos(Math.PI * angleInDegree / 180);
    }

    public IList<PointD2D> FlattenPolyBezierCurve(PointD2D[] bezierPoints)
    {
      if (null == bezierPoints)
        throw new ArgumentNullException(nameof(bezierPoints));
      if (bezierPoints.Length < 4 && 0 != (bezierPoints.Length - 1) % 3)
        throw new ArgumentException("Array length has to be >=4 and has to be expressable as 1+3*k", nameof(bezierPoints));

      var list = new List<PointD2D>();

      var segments = (bezierPoints.Length - 1) / 3;

      for (int i = 0; i < bezierPoints.Length; i += 3)
        list.Add(bezierPoints[i]); // add all curve points first

      for (int i = segments - 1; i > 0; --i) // backward in order to have the insertionPoint not moved by the flattening
      {
        int idx = i * 3;
        FlattenBezierSegment(0, bezierPoints[idx], bezierPoints[idx + 1], bezierPoints[idx + 2], bezierPoints[idx + 3], list, 1 + i);
      }

      return list;
    }

    public void FlattenPolyBezierCurve(PointD2D startPoint, IList<PointD2D> bezierPoints, List<PointD2D> flattenedList)
    {
      if (null == bezierPoints)
        throw new ArgumentNullException(nameof(bezierPoints));
      if (bezierPoints.Count < 3 && 0 != (bezierPoints.Count) % 3)
        throw new ArgumentException("Array length has to be >=4 and has to be expressable as 1+3*k", nameof(bezierPoints));

      // Flatten the first segment
      flattenedList.Add(bezierPoints[2]);
      FlattenBezierSegment(0, startPoint, bezierPoints[0], bezierPoints[1], bezierPoints[2], flattenedList, flattenedList.Count - 1);

      // now flatten the other segments
      for (int idx = 5; idx < bezierPoints.Count; idx += 3) // backward in order to have the insertionPoint not moved by the flattening
      {
        flattenedList.Add(bezierPoints[idx]);
        FlattenBezierSegment(0, bezierPoints[idx - 3], bezierPoints[idx - 2], bezierPoints[idx - 1], bezierPoints[idx], flattenedList, flattenedList.Count - 1);
      }
    }

    public bool FlattenBezierSegment(int recursionLevel, PointD2D p0_0, PointD2D p1_0, PointD2D p2_0, PointD2D p3_0, List<PointD2D> flattenedList, int insertIdx)
    {
      if (!IsBezierSegmentFlatEnough(p0_0, p1_0, p2_0, p3_0))
      {
        var p0_1 = new PointD2D(0.5 * (p0_0.X + p1_0.X), 0.5 * (p0_0.Y + p1_0.Y));
        var p1_1 = new PointD2D(0.5 * (p1_0.X + p2_0.X), 0.5 * (p1_0.Y + p2_0.Y));
        var p2_1 = new PointD2D(0.5 * (p2_0.X + p3_0.X), 0.5 * (p2_0.Y + p3_0.Y));

        var p0_2 = new PointD2D(0.5 * (p0_1.X + p1_1.X), 0.5 * (p0_1.Y + p1_1.Y));
        var p1_2 = new PointD2D(0.5 * (p1_1.X + p2_1.X), 0.5 * (p1_1.Y + p2_1.Y));

        var p0_3 = new PointD2D(0.5 * (p0_2.X + p1_2.X), 0.5 * (p0_2.Y + p1_2.Y));

        flattenedList.Insert(insertIdx, p0_3);

        if (recursionLevel < 24)
        {
          // now flatten the right side first
          FlattenBezierSegment(recursionLevel + 1, p0_3, p1_2, p2_1, p3_0, flattenedList, insertIdx + 1);

          // and the left side
          FlattenBezierSegment(recursionLevel + 1, p0_0, p0_1, p0_2, p0_3, flattenedList, insertIdx);
        }

        return true;
      }

      return false;
    }

    public bool IsBezierSegmentFlatEnough(PointD2D p0, PointD2D p1, PointD2D p2, PointD2D p3)
    {
      // First, test for absolute deviation of the curve
      if (_toleranceCriterium < double.MaxValue)
      {
        double ux = 3 * p1.X - 2 * p0.X - p3.X;
        ux *= ux;

        double uy = 3 * p1.Y - 2 * p0.Y - p3.Y;
        uy *= uy;

        double vx = 3 * p2.X - 2 * p3.X - p0.X;
        vx *= vx;

        double vy = 3 * p2.Y - 2 * p3.Y - p0.Y;
        vy *= vy;

        if (ux < vx)
          ux = vx;
        if (uy < vy)
          uy = vy;
        if ((ux + uy) > _toleranceCriterium) // tolerance here is 16*tol^2 (tol is the maximum absolute allowed deviation of the curve from the approximation)
          return false;
      }

      // now, test for the angle deviation of the curve
      var v30 = p3 - p0;
      if (0 == v30.X && 0 == v30.Y)
        return true; // there

      var v10 = p1 - p0;
      if (0 == v10.X && 0 == v10.Y)
        if ((0 != v10.X || 0 != v10.Y) && !IsAngleBelowCriterium(v30, v10))
          return false;

      var v20 = p2 - p0;
      if ((0 != v20.X || 0 != v20.Y) && !IsAngleBelowCriterium(v30, v20))
        return false;

      var v32 = p3 - p2;
      if ((0 != v32.X || 0 != v32.Y) && !IsAngleBelowCriterium(v30, v32))
        return false;

      var v31 = p3 - p1;
      if ((0 != v31.X || 0 != v31.Y) && !IsAngleBelowCriterium(v30, v31))
        return false;

      return true;
    }

    public bool IsAngleBelowCriterium(PointD2D v1, PointD2D v2)
    {
      double r = v1.X * v2.X + v1.Y * v2.Y;
      if (r <= 0)
        return false;

      r = r * r / ((v1.X * v1.X + v1.Y * v1.Y) * (v2.X * v2.X + v2.Y * v2.Y));
      return r > _angleCriterium; // 1 Degree
    }
  }
}
