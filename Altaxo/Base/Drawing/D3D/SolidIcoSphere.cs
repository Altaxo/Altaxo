#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  using TriangleIndices = Tuple<int, int, int>;

  /// <summary>
  /// Creates a sphere as from an icosahedron by recursive refinement of the initial triangles.
  /// </summary>
  /// <remarks>
  /// The code originates from Andreas Kahler, see <see href="http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html"/>.
  /// </remarks>
  public class SolidIcoSphere
  {
    private List<PointD3D> _geometry_Positions;
    private List<TriangleIndices> _geometry_TriangleIndices;

    private int _index;

    public SolidIcoSphere(int recursionLevel)
    {
      var middlePointIndexCache = new Dictionary<long, int>();
      _index = 0;
      _geometry_Positions = new List<PointD3D>();

      // create 12 vertices of a icosahedron
      var t = (1.0 + Math.Sqrt(5.0)) / 2.0;

      AddVertex(new PointD3D(-1, t, 0));
      AddVertex(new PointD3D(1, t, 0));
      AddVertex(new PointD3D(-1, -t, 0));
      AddVertex(new PointD3D(1, -t, 0));

      AddVertex(new PointD3D(0, -1, t));
      AddVertex(new PointD3D(0, 1, t));
      AddVertex(new PointD3D(0, -1, -t));
      AddVertex(new PointD3D(0, 1, -t));

      AddVertex(new PointD3D(t, 0, -1));
      AddVertex(new PointD3D(t, 0, 1));
      AddVertex(new PointD3D(-t, 0, -1));
      AddVertex(new PointD3D(-t, 0, 1));

      // create 20 triangles of the icosahedron
      var faces = new List<TriangleIndices>
      {

        // 5 faces around point 0
        new TriangleIndices(0, 11, 5),
        new TriangleIndices(0, 5, 1),
        new TriangleIndices(0, 1, 7),
        new TriangleIndices(0, 7, 10),
        new TriangleIndices(0, 10, 11),

        // 5 adjacent faces
        new TriangleIndices(1, 5, 9),
        new TriangleIndices(5, 11, 4),
        new TriangleIndices(11, 10, 2),
        new TriangleIndices(10, 7, 6),
        new TriangleIndices(7, 1, 8),

        // 5 faces around point 3
        new TriangleIndices(3, 9, 4),
        new TriangleIndices(3, 4, 2),
        new TriangleIndices(3, 2, 6),
        new TriangleIndices(3, 6, 8),
        new TriangleIndices(3, 8, 9),

        // 5 adjacent faces
        new TriangleIndices(4, 9, 5),
        new TriangleIndices(2, 4, 11),
        new TriangleIndices(6, 2, 10),
        new TriangleIndices(8, 6, 7),
        new TriangleIndices(9, 8, 1)
      };

      // refine triangles
      for (int i = 0; i < recursionLevel; i++)
      {
        var faces2 = new List<TriangleIndices>();
        foreach (var tri in faces)
        {
          // replace triangle by 4 triangles
          int a = GetMiddlePoint(tri.Item1, tri.Item2, middlePointIndexCache);
          int b = GetMiddlePoint(tri.Item2, tri.Item3, middlePointIndexCache);
          int c = GetMiddlePoint(tri.Item3, tri.Item1, middlePointIndexCache);

          faces2.Add(new TriangleIndices(tri.Item1, a, c));
          faces2.Add(new TriangleIndices(tri.Item2, b, a));
          faces2.Add(new TriangleIndices(tri.Item3, c, b));
          faces2.Add(new TriangleIndices(a, b, c));
        }
        faces = faces2;
      }

      _geometry_TriangleIndices = faces;
    }

    public IEnumerable<PointD3D> VerticesForSphere
    {
      get
      {
        return _geometry_Positions;
      }
    }

    public IEnumerable<Tuple<PointD3D, VectorD3D>> VerticesAndNormalsForSphere
    {
      get
      {
        foreach (var pt in _geometry_Positions)
          yield return new Tuple<PointD3D, VectorD3D>(pt, (VectorD3D)pt);
      }
    }

    public IEnumerable<Tuple<int, int, int>> TriangleIndicesForSphere
    {
      get
      {
        return _geometry_TriangleIndices;
      }
    }

    public IEnumerable<Tuple<PointD3D, VectorD3D>> VerticesAndNormalsForPolyhedron
    {
      get
      {
        foreach (var ti in _geometry_TriangleIndices)
        {
          var p0 = _geometry_Positions[ti.Item1];
          var p1 = _geometry_Positions[ti.Item2];
          var p2 = _geometry_Positions[ti.Item3];

          var normal = new VectorD3D((p0.X + p1.X + p2.X) / 3, (p0.Y + p1.Y + p2.Y) / 3, (p0.Z + p1.Z + p2.Z) / 3);

          yield return new Tuple<PointD3D, VectorD3D>(p0, normal);
          yield return new Tuple<PointD3D, VectorD3D>(p1, normal);
          yield return new Tuple<PointD3D, VectorD3D>(p2, normal);
        }
      }
    }

    public IEnumerable<Tuple<int, int, int>> TriangleIndicesForPolyhedron
    {
      get
      {
        int idx = 0;
        foreach (var ti in _geometry_TriangleIndices) // we don't use the _geometry_TriangleIndices here. We only use it as a measure of the number of triangles
        {
          yield return new TriangleIndices(idx, idx + 1, idx + 2);
          idx += 3;
        }
      }
    }

    // add vertex to mesh, fix position to be on unit sphere, return index
    private int AddVertex(PointD3D p)
    {
      double length = Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
      _geometry_Positions.Add(new PointD3D(p.X / length, p.Y / length, p.Z / length));
      return _index++;
    }

    // return index of point in the middle of p1 and p2
    private int GetMiddlePoint(int p1, int p2, Dictionary<long, int> middlePointIndexCache)
    {
      // first check if we have it already
      bool firstIsSmaller = p1 < p2;
      long smallerIndex = firstIsSmaller ? p1 : p2;
      long greaterIndex = firstIsSmaller ? p2 : p1;
      Int64 key = (smallerIndex << 32) + greaterIndex;

      if (middlePointIndexCache.TryGetValue(key, out var ret))
      {
        return ret;
      }

      // not in cache, calculate it
      PointD3D point1 = _geometry_Positions[p1];
      PointD3D point2 = _geometry_Positions[p2];
      var middle = new PointD3D(
          (point1.X + point2.X) / 2.0,
          (point1.Y + point2.Y) / 2.0,
          (point1.Z + point2.Z) / 2.0);

      // add vertex makes sure point is on unit sphere
      int i = AddVertex(middle);

      // store it, return index
      middlePointIndexCache.Add(key, i);
      return i;
    }
  }
}
