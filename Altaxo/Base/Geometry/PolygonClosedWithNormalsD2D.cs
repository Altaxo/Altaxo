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

using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Stores a closed polygon and the normals. In order to distinguish between soft vertices and sharp vertices, the sharp vertices are
  /// stored twice, because every sharp vertex has two normals. Thus there is a 1:1 relationship between the indices of the normals and the points.
  /// </summary>
  public class PolygonClosedWithNormalsD2D
  {
    private PointD2D[] _points;
    private PointD2D[] _normals;

    public PointD2D[] Points { get { return _points; } }
    public PointD2D[] Normals { get { return _normals; } }

    public PolygonClosedWithNormalsD2D(PolygonClosedD2D template)
    {
      var numPoints = template.Points.Length + template.SharpPoints.Count;

      _points = new PointD2D[numPoints];
      _normals = new PointD2D[numPoints];

      var srcPoints = template.Points;
      var srcCount = srcPoints.Length;
      var startPoint = srcPoints[srcCount - 1];

      int destIdx = 0;
      for (int i = 0; i < srcCount; ++i)
      {
        var toHereVector = srcPoints[i] - startPoint;
        var fromHereVector = srcPoints[(i + 1) % srcCount] - srcPoints[i];

        if (template.SharpPoints.Contains(srcPoints[i]))
        {
          var normal = GetNormal(toHereVector, template.IsHole);
          _points[destIdx] = srcPoints[i];
          _normals[destIdx] = normal;
          ++destIdx;

          normal = GetNormal(fromHereVector, template.IsHole);
          _points[destIdx] = srcPoints[i];
          _normals[destIdx] = normal;
          ++destIdx;
        }
        else
        {
          var normal = GetNormal(toHereVector + fromHereVector, template.IsHole);
          _points[destIdx] = srcPoints[i];
          _normals[destIdx] = normal;
          ++destIdx;
        }

        startPoint = srcPoints[i];
      }

      if (!(numPoints == destIdx))
        throw new InvalidProgramException();
    }

    private PointD2D GetNormal(PointD2D polygonVector, bool isHole)
    {
      return new PointD2D(polygonVector.Y, -polygonVector.X) / polygonVector.VectorLength;
    }
  }
}
