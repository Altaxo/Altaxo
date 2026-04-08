/* Poly2Tri
 * Copyright (c) 2009-2010, Poly2Tri Contributors
 * http://code.google.com/p/poly2tri/
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Poly2Tri nor the names of its contributors may be
 *   used to endorse or promote products derived from this software without specific
 *   prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

// Changes from the Java version
//   attributification
// Future possibilities
//   Flattening out the number of indirections
//     Replacing arrays of 3 with fixed-length arrays?
//     Replacing bool[3] with a bit array of some sort?
//     Bundling everything into an AoS mess?
//     Hardcode them all as ABC ?

#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Poly2Tri
{
  /// <summary>
  /// Represents a triangle in the Delaunay triangulation.
  /// </summary>
  public class DelaunayTriangle
  {
    /// <summary>
    /// The triangle vertices.
    /// </summary>
    public FixedArray3<TriangulationPoint> Points;

    /// <summary>
    /// Neighbor triangles across the three triangle edges.
    /// </summary>
    public FixedArray3<DelaunayTriangle> Neighbors;

    /// <summary>
    /// Flags indicating whether the triangle edges are constrained or marked as Delaunay edges.
    /// </summary>
    public FixedBitArray3 EdgeIsConstrained, EdgeIsDelaunay;

    /// <summary>
    /// Gets or sets a value indicating whether this triangle is part of the interior result.
    /// </summary>
    public bool IsInterior { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelaunayTriangle"/> class.
    /// </summary>
    /// <param name="p1">The first vertex.</param>
    /// <param name="p2">The second vertex.</param>
    /// <param name="p3">The third vertex.</param>
    public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3)
    {
      Points[0] = p1;
      Points[1] = p2;
      Points[2] = p3;
    }

    /// <summary>
    /// Gets the index of the specified point within the triangle.
    /// </summary>
    /// <param name="p">The point to locate.</param>
    /// <returns>The point index.</returns>
    public int IndexOf(TriangulationPoint p)
    {
      int i = Points.IndexOf(p);
      if (i == -1)
        throw new Exception("Calling index with a point that doesn't exist in triangle");
      return i;
    }

    /// <summary>
    /// Gets the clockwise index relative to the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The clockwise index.</returns>
    public int IndexCWFrom(TriangulationPoint p)
    {
      return (IndexOf(p) + 2) % 3;
    }

    /// <summary>
    /// Gets the counterclockwise index relative to the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The counterclockwise index.</returns>
    public int IndexCCWFrom(TriangulationPoint p)
    {
      return (IndexOf(p) + 1) % 3;
    }

    /// <summary>
    /// Determines whether the specified point is one of the triangle vertices.
    /// </summary>
    /// <param name="p">The point to test.</param>
    /// <returns><see langword="true"/> if the point is a vertex of the triangle; otherwise, <see langword="false"/>.</returns>
    public bool Contains(TriangulationPoint p)
    {
      return Points.Contains(p);
    }

    /// <summary>
    /// Update neighbor pointers
    /// </summary>
    /// <param name="p1">Point 1 of the shared edge</param>
    /// <param name="p2">Point 2 of the shared edge</param>
    /// <param name="t">This triangle's new neighbor</param>
    private void MarkNeighbor(TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t)
    {
      int i = EdgeIndex(p1, p2);
      if (i == -1)
        throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
      Neighbors[i] = t;
    }

    /// <summary>
    /// Exhaustive search to update neighbor pointers
    /// </summary>
    public void MarkNeighbor(DelaunayTriangle t)
    {
      // Points of this triangle also belonging to t
      bool a = t.Contains(Points[0]);
      bool b = t.Contains(Points[1]);
      bool c = t.Contains(Points[2]);

      if (b && c)
      { Neighbors[0] = t; t.MarkNeighbor(Points[1], Points[2], this); }
      else if (a && c)
      { Neighbors[1] = t; t.MarkNeighbor(Points[0], Points[2], this); }
      else if (a && b)
      { Neighbors[2] = t; t.MarkNeighbor(Points[0], Points[1], this); }
      else
        throw new Exception("Failed to mark neighbor, doesn't share an edge!");
    }

    /// <summary>
    /// Gets the point in this triangle opposite to the point <paramref name="p"/> in the neighboring triangle <paramref name="t"/>.
    /// </summary>
    /// <param name="t">The neighboring triangle.</param>
    /// <param name="p">The non-shared point in <paramref name="t"/>.</param>
    /// <returns>The point opposite the shared edge in this triangle.</returns>
    public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p)
    {
      if (!(t != this))
        throw new InvalidProgramException("self pointer error");
      return PointCWFrom(t.PointCWFrom(p));
    }

    /// <summary>
    /// Gets the clockwise neighbor relative to the specified point.
    /// </summary>
    /// <param name="point">The reference point.</param>
    /// <returns>The clockwise neighbor triangle.</returns>
    public DelaunayTriangle NeighborCWFrom(TriangulationPoint point)
    {
      return Neighbors[(Points.IndexOf(point) + 1) % 3];
    }

    /// <summary>
    /// Gets the counterclockwise neighbor relative to the specified point.
    /// </summary>
    /// <param name="point">The reference point.</param>
    /// <returns>The counterclockwise neighbor triangle.</returns>
    public DelaunayTriangle NeighborCCWFrom(TriangulationPoint point)
    {
      return Neighbors[(Points.IndexOf(point) + 2) % 3];
    }

    /// <summary>
    /// Gets the neighbor across from the specified point.
    /// </summary>
    /// <param name="point">The reference point.</param>
    /// <returns>The opposite neighbor triangle.</returns>
    public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point)
    {
      return Neighbors[Points.IndexOf(point)];
    }

    /// <summary>
    /// Gets the counterclockwise point relative to the specified point.
    /// </summary>
    /// <param name="point">The reference point.</param>
    /// <returns>The counterclockwise point.</returns>
    public TriangulationPoint PointCCWFrom(TriangulationPoint point)
    {
      return Points[(IndexOf(point) + 1) % 3];
    }

    /// <summary>
    /// Gets the clockwise point relative to the specified point.
    /// </summary>
    /// <param name="point">The reference point.</param>
    /// <returns>The clockwise point.</returns>
    public TriangulationPoint PointCWFrom(TriangulationPoint point)
    {
      return Points[(IndexOf(point) + 2) % 3];
    }

    private void RotateCW()
    {
      var t = Points[2];
      Points[2] = Points[1];
      Points[1] = Points[0];
      Points[0] = t;
    }

    /// <summary>
    /// Legalize triangle by rotating clockwise around oPoint
    /// </summary>
    /// <param name="oPoint">The origin point to rotate around</param>
    /// <param name="nPoint">The point that replaces the counterclockwise vertex from <paramref name="oPoint"/> after the rotation.</param>
    public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint)
    {
      RotateCW();
      Points[IndexCCWFrom(oPoint)] = nPoint;
    }

    /// <summary>
    /// Returns a string representation of this triangle.
    /// </summary>
    /// <returns>A string representation of this triangle.</returns>
    public override string ToString()
    {
      return Points[0] + "," + Points[1] + "," + Points[2];
    }

    /// <summary>
    /// Finalize edge marking
    /// </summary>
    public void MarkNeighborEdges()
    {
      for (int i = 0; i < 3; i++)
        if (EdgeIsConstrained[i] && Neighbors[i] is not null)
        {
          Neighbors[i].MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
        }
    }

    /// <summary>
    /// Marks all constrained edges on the specified triangle.
    /// </summary>
    /// <param name="triangle">The triangle to update.</param>
    public void MarkEdge(DelaunayTriangle triangle)
    {
      for (int i = 0; i < 3; i++)
        if (EdgeIsConstrained[i])
        {
          triangle.MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
        }
    }

    /// <summary>
    /// Marks constrained edges based on the specified triangles.
    /// </summary>
    /// <param name="tList">The triangles providing constrained-edge information.</param>
    public void MarkEdge(List<DelaunayTriangle> tList)
    {
      foreach (DelaunayTriangle t in tList)
        for (int i = 0; i < 3; i++)
          if (t.EdgeIsConstrained[i])
          {
            MarkConstrainedEdge(t.Points[(i + 1) % 3], t.Points[(i + 2) % 3]);
          }
    }

    /// <summary>
    /// Marks the edge at the specified index as constrained.
    /// </summary>
    /// <param name="index">The edge index.</param>
    public void MarkConstrainedEdge(int index)
    {
      EdgeIsConstrained[index] = true;
    }

    /// <summary>
    /// Marks the specified constrained edge.
    /// </summary>
    /// <param name="edge">The constrained edge.</param>
    public void MarkConstrainedEdge(DTSweepConstraint edge)
    {
      MarkConstrainedEdge(edge.P, edge.Q);
    }

    /// <summary>
    /// Mark edge as constrained
    /// </summary>
    public void MarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q)
    {
      int i = EdgeIndex(p, q);
      if (i != -1)
        EdgeIsConstrained[i] = true;
    }

    /// <summary>
    /// Computes the triangle area.
    /// </summary>
    /// <returns>The triangle area.</returns>
    public double Area()
    {
      double b = Points[0].X - Points[1].X;
      double h = Points[2].Y - Points[1].Y;

      return Math.Abs((b * h * 0.5f));
    }

    /// <summary>
    /// Computes the triangle centroid.
    /// </summary>
    /// <returns>The centroid point.</returns>
    public TriangulationPoint Centroid()
    {
      double cx = (Points[0].X + Points[1].X + Points[2].X) / 3f;
      double cy = (Points[0].Y + Points[1].Y + Points[2].Y) / 3f;
      return new TriangulationPoint(cx, cy);
    }

    /// <summary>
    /// Get the index of the neighbor that shares this edge (or -1 if it isn't shared)
    /// </summary>
    /// <returns>index of the shared edge or -1 if edge isn't shared</returns>
    public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2)
    {
      int i1 = Points.IndexOf(p1);
      int i2 = Points.IndexOf(p2);

      // Points of this triangle in the edge p1-p2
      bool a = (i1 == 0 || i2 == 0);
      bool b = (i1 == 1 || i2 == 1);
      bool c = (i1 == 2 || i2 == 2);

      if (b && c)
        return 0;
      if (a && c)
        return 1;
      if (a && b)
        return 2;
      return -1;
    }

    /// <summary>
    /// Gets the constrained-edge flag counterclockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The constrained-edge flag.</returns>
    public bool GetConstrainedEdgeCCW(TriangulationPoint p)
    {
      return EdgeIsConstrained[(IndexOf(p) + 2) % 3];
    }

    /// <summary>
    /// Gets the constrained-edge flag clockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The constrained-edge flag.</returns>
    public bool GetConstrainedEdgeCW(TriangulationPoint p)
    {
      return EdgeIsConstrained[(IndexOf(p) + 1) % 3];
    }

    /// <summary>
    /// Gets the constrained-edge flag across from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The constrained-edge flag.</returns>
    public bool GetConstrainedEdgeAcross(TriangulationPoint p)
    {
      return EdgeIsConstrained[IndexOf(p)];
    }

    /// <summary>
    /// Sets the constrained-edge flag counterclockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <param name="ce">The constrained-edge flag to set.</param>
    public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce)
    {
      EdgeIsConstrained[(IndexOf(p) + 2) % 3] = ce;
    }

    /// <summary>
    /// Sets the constrained-edge flag clockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <param name="ce">The constrained-edge flag to set.</param>
    public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce)
    {
      EdgeIsConstrained[(IndexOf(p) + 1) % 3] = ce;
    }

    /// <summary>
    /// Sets the constrained-edge flag across from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <param name="ce">The constrained-edge flag to set.</param>
    public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce)
    {
      EdgeIsConstrained[IndexOf(p)] = ce;
    }

    /// <summary>
    /// Gets the Delaunay-edge flag counterclockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The Delaunay-edge flag.</returns>
    public bool GetDelaunayEdgeCCW(TriangulationPoint p)
    {
      return EdgeIsDelaunay[(IndexOf(p) + 2) % 3];
    }

    /// <summary>
    /// Gets the Delaunay-edge flag clockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The Delaunay-edge flag.</returns>
    public bool GetDelaunayEdgeCW(TriangulationPoint p)
    {
      return EdgeIsDelaunay[(IndexOf(p) + 1) % 3];
    }

    /// <summary>
    /// Gets the Delaunay-edge flag across from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <returns>The Delaunay-edge flag.</returns>
    public bool GetDelaunayEdgeAcross(TriangulationPoint p)
    {
      return EdgeIsDelaunay[IndexOf(p)];
    }

    /// <summary>
    /// Sets the Delaunay-edge flag counterclockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <param name="ce">The Delaunay-edge flag to set.</param>
    public void SetDelaunayEdgeCCW(TriangulationPoint p, bool ce)
    {
      EdgeIsDelaunay[(IndexOf(p) + 2) % 3] = ce;
    }

    /// <summary>
    /// Sets the Delaunay-edge flag clockwise from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <param name="ce">The Delaunay-edge flag to set.</param>
    public void SetDelaunayEdgeCW(TriangulationPoint p, bool ce)
    {
      EdgeIsDelaunay[(IndexOf(p) + 1) % 3] = ce;
    }

    /// <summary>
    /// Sets the Delaunay-edge flag across from the specified point.
    /// </summary>
    /// <param name="p">The reference point.</param>
    /// <param name="ce">The Delaunay-edge flag to set.</param>
    public void SetDelaunayEdgeAcross(TriangulationPoint p, bool ce)
    {
      EdgeIsDelaunay[IndexOf(p)] = ce;
    }
  }
}
