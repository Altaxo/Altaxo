#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ClipperLib;

namespace Altaxo.Geometry.Int64_2D
{
  /// <summary>
  /// Represents the concave hull of a set of 2D points.
  /// </summary>
  /// <remarks>For the algorithms calculating the concave hull, see article by E. Rosen et al. 'Implementation of a fast and
  /// efficient concave hull algorithm" here: <see href="http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf"/>.
  /// For the intersection tests, see web site of Martin Thoma: <see href="https://martin-thoma.com/how-to-check-if-two-line-segments-intersect/"/>.
  /// </remarks>
  public partial class ConcaveHull
  {
    /// <summary>
    /// Stores the convex edges of a hull (is calculated once in the constructor).
    /// </summary>
    private IReadOnlyList<Int64LineD2DAnnotated> _hull_convex_edges;

    /// <summary>
    /// Gets the points that form the convex hull.
    /// </summary>
    public IReadOnlyList<(IntPoint point, int index)> ConvexHullPoints => _convexHullPoints;
    private IReadOnlyList<(IntPoint point, int index)> _convexHullPoints;

    /// <summary>
    /// Gets the points that are not part of the convex hull.
    /// </summary>
    public IReadOnlyList<(IntPoint point, int index)> PointsNotOnConvexHull { get; private set; }

    /// <summary>
    /// Gets the points that form the concave hull.
    /// </summary>
    public IReadOnlyList<(IntPoint point, int index)> ConcaveHullPoints => _concaveHullPoints;
    private IReadOnlyList<(IntPoint point, int index)> _concaveHullPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcaveHull"/> class.
    /// </summary>
    /// <param name="points">The points for which to calculate the concave hull.</param>
    /// <param name="concavity">The concavity is a value in the interval [-1, 1] that represents
    /// the cosine of the allowed angle between a line on the convex hull and a potential concave 'bend'.
    /// Thus a value of 1 is the limit case of a convex hull, a value of -1 allows sharp and deep bends.
    /// </param>
    /// <param name="minimalEdgeLength">The minimal length of an edge of the hull that is considered as a candidate edge for having a concave bend.</param>
    public ConcaveHull(IReadOnlyList<IntPoint> points, double concavity, double minimalEdgeLength)
    {
      concavity = Math.Min(1, Math.Max(concavity, -1));

      _convexHullPoints = GrahamScan.GetConvexHull(points);
      _hull_convex_edges = CalculateConvexHullAsLineList(points);

      var pointsNotOnConvexHull = new List<(IntPoint point, int index)>();
      // from pointsNotOnConvexHull remove all points that are part of the convex hull
      // so that only those points that are not part of the convex hull remain
      // note: the Graham scan does not include colinear points in the convex hull!
      var convexHullIDs = new HashSet<int>(ConvexHullPoints.Select(hullpoint => hullpoint.index));
      for (var i = 0; i < points.Count; ++i)
      {
        if (!convexHullIDs.Contains(i))
        {
          pointsNotOnConvexHull.Add((points[i], i));
        }
      }
      PointsNotOnConvexHull = pointsNotOnConvexHull;

      CalculateConcaveHull(concavity, minimalEdgeLength);
    }



    /// <summary>
    /// Sets a concave hull with parameters that could be different from those provided in the constructor.
    /// </summary>
    /// <param name="concavity">The concavity is a value in the interval [-1, 1] that represents
    /// the cosine of the allowed angle between a line on the convex hull and a potential concave 'bend'.
    /// Thus a value of 1 is the limit case of a convex hull, a value of -1 allows sharp and deep bends.
    /// </param>
    /// <param name="minimalEdgeLength">The minimal length of an edge of the hull that is considered as a candidate edge for having a concave bend.</param>
    [MemberNotNull(nameof(_concaveHullPoints))]
    public void CalculateConcaveHull(double concavity, double minimalEdgeLength)
    {
      concavity = Math.Min(1, Math.Max(concavity, -1));
      var angle_concavity = Math.Acos(concavity);

      if (!(concavity < 1))
      {
        // if concavity is 1, the concave hull is equal to the convex hull
        _concaveHullPoints = ConvexHullPoints;
        return;
      }

      var pointsNotOnHull = new List<(IntPoint point, int index)>(PointsNotOnConvexHull); // make a copy of the unused points
      var hull_concave_edges_temp = new List<Int64LineD2DAnnotated>(_hull_convex_edges); // list A in the paper
      hull_concave_edges_temp.Sort(LengthComparer.Instance); // the edge with greatest length is at the end of the list!
      var hull_concave_edges_final = new List<Int64LineD2DAnnotated>(); // list B in the paper

      var maximumAngleOfOuterVertex = Math.Acos(concavity); // the maximum angle between one segment of the convex hull and the new segment of the concave hull
      var maximumAngleOfOuterVertexLimitedToPiHalf = Math.Min(maximumAngleOfOuterVertex, Math.PI / 2); // same angle, but clamped to Pi/2

      // not in the paper: in order to limit the search for potential points of the concave hull, we can take advantage
      // of the fact that when the maximum angle of the outer vertex is less than Pi/2, the angle of the inner vertex of the new
      // two segments of the concave hull is greater than Pi-2*maximumAngleOfOuterVertex
      // thus we can use an inscribed circle that goes through the vertices of the convex hull segment, and has the property
      // that any point on this circle, when connected to the two vertices of the convex hull segment,
      // then those two connection lines form an angle of Pi-2*maximumAngleOfOuterVertex
      // the bounding circle reaches its smallest diameter if maximumAngleOfOuterVertex is Pi/4 (then the diameter of the circle is equal to the length of the convex hull segment)
      // if maximumAngleOfOuterVertex is smaller than Pi/4, the circle gets bigger, but its center is to the right (i.e. outside) the hull
      // if maximumAngleOfOuterVertex is greater than Pi/4 and less than Pi/2, the circle gets bigger, with its center to the left (i.e. inside) the hull
      // for maximumAngleOfOuterVertex greater then or equal Pi/2, the bounding circle would be infinite, and thus is useless
      // in order to save computing time, instead of a bounding circle we are using a bounding rectangle

      // boundingCircleRelativeRadius and boundingCircleRelativeOffset are relative to length/2 of the convex hull segment
      var (boundingCircleRelativeRadius, boundingCircleRelativeOffset) = GetBoundingCircleParametersFor(maximumAngleOfOuterVertex);

      Int64LineD2DAnnotated selected_edge;

#if CheckHullAfterEachStep
      ExecuteTestsAfterInsertion(hull_concave_edges_temp, hull_concave_edges_final, unused_nodes);
#endif
      var _debug_LoopCounter = 0;
      while (hull_concave_edges_temp.Count > 0 && pointsNotOnHull.Count > 0)
      {
        ++_debug_LoopCounter;
        selected_edge = hull_concave_edges_temp[hull_concave_edges_temp.Count - 1];

        if (selected_edge.Length < minimalEdgeLength)
        {
          break;
        }

        hull_concave_edges_temp.RemoveAt(hull_concave_edges_temp.Count - 1);
        var wereNewEdgesCreated = false;

        // gets the bounding box for potential concave hull points. If bounding box would be infinite,
        // a value of null is returned
        var boundingBox = GetBoundingBox(selected_edge, boundingCircleRelativeRadius, boundingCircleRelativeOffset);
        var nearPoints = GetPointsNearby(selected_edge, pointsNotOnHull);

        // paper: foreach of nearPoints, calculate the two angles and select that point with the minimum angles
        // note instead of the angle, we use the cosine, thus instead of minimum angle, we need maximum cosine
        var bestAngle = double.PositiveInfinity;
        (IntPoint point, int index) bestPoint = default;
        foreach (var np in nearPoints)
        {
          if (boundingBox is null || boundingBox.Value.IsPointWithin(np.point))
          {
            var angle1 = Int64LineSegment.GetAngle(selected_edge.P0, selected_edge.P1, np.point, returnPositiveValueIf180Degrees: true);
            var angle2 = Int64LineSegment.GetAngle(selected_edge.P1, np.point, selected_edge.P0, returnPositiveValueIf180Degrees: true);
            var maxAngle = Math.Max(angle1, angle2);
            if (maxAngle >= 0 && maxAngle < bestAngle)
            {
              bestAngle = maxAngle;
              bestPoint = np;


              if (bestAngle == 0)
              {
                break;
              }
              else if (bestAngle < maximumAngleOfOuterVertexLimitedToPiHalf)  // update bounding box if useful
              {
                // if our best angle was updated and is less than Pi/2,
                // we can recalculate the bounding box using the new angle, in order to make it smaller
                // and to limit the search further
                boundingBox = GetBoundingBox(selected_edge, bestAngle);
              }

            }
          }
        }

        if (bestAngle <= angle_concavity)
        {
          // paper: Create edges e2 and e3 between point p and endpoints of edge e;
          var e2 = new Int64LineD2DAnnotated((selected_edge.P0, selected_edge.I0), bestPoint);
          var e3 = new Int64LineD2DAnnotated(bestPoint, (selected_edge.P1, selected_edge.I1));

          // paper: if edge e2 and e3 don't intersect any other edge
          // note: it is ok if e2.P0 has a common point with an existing edge's P1
          // and if e3.P1 is the same as an existing edge's P0
          if (!DoLinesIntersectAnyOtherLine(e2, e3, hull_concave_edges_temp) && !DoLinesIntersectAnyOtherLine(e2, e3, hull_concave_edges_final))
          {
            // Add edge e2 and e3 to list A;
            InsertSortedByLengthAscending(e2, hull_concave_edges_temp);
            InsertSortedByLengthAscending(e3, hull_concave_edges_temp);
            pointsNotOnHull.Remove(bestPoint);
            wereNewEdgesCreated = true;

#if CheckHullAfterEachStep
            ExecuteTestsAfterInsertion(hull_concave_edges_temp, hull_concave_edges_final, unused_nodes);
#endif
          }
        }

        if (!wereNewEdgesCreated) // paper: if edge e2 and e3 was not added to list A
        {
          // paper: Add edge e to list B;
          hull_concave_edges_final.Add(selected_edge);

#if CheckHullAfterEachStep
          ExecuteTestsAfterInsertion(hull_concave_edges_temp, hull_concave_edges_final, unused_nodes);
#endif
        }
      }

      hull_concave_edges_final.AddRange(hull_concave_edges_temp);

      // calculate the hull points from the edges
      _concaveHullPoints = GetHullPoints(hull_concave_edges_final);
    }


    /// <summary>
    /// Gets the parameters relative radius and offset for a bounding circle that limit the search for potential candidates for the concave hull.
    /// </summary>
    /// <param name="maximumAngleOfOuterVertex">The maximum allowed angle between the (old) convex hull segment and the new segment of the concave hull.</param>
    /// <returns>The parameters relative radius and offset for a bounding circle.</returns>
    /// <remarks>
    /// Not in the paper: in order to limit the search for potential points of the concave hull, we can take advantage
    /// of the fact that when the maximum angle of the outer vertex is less than Pi/2, the angle of the inner vertex of the new
    /// two segments of the concave hull is greater than Pi-2*maximumAngleOfOuterVertex
    /// thus we can use an inscribed circle that goes through the vertices of the convex hull segment, and has the property
    /// that any point on this circle, when connected to the two vertices of the convex hull segment,
    /// then those two connection lines form an angle of Pi-2*maximumAngleOfOuterVertex.
    /// The bounding circle reaches its smallest diameter if maximumAngleOfOuterVertex is Pi/4 (then the diameter of the circle is equal to the length of the convex hull segment)
    /// If maximumAngleOfOuterVertex is smaller than Pi/4, the circle gets bigger, but its center is to the right (i.e. outside) the hull
    /// If maximumAngleOfOuterVertex is greater than Pi/4 and less than Pi/2, the circle gets bigger, with its center to the left (i.e. inside) the hull
    /// For maximumAngleOfOuterVertex greater then or equal to Pi/2, the bounding circle would be infinite, and thus is useless (we return PositiveInfinity for both values).
    /// </remarks>
    private static (double relativeRadius, double relativeOffset) GetBoundingCircleParametersFor(double maximumAngleOfOuterVertex)
    {
      var boundingCircleRelativeRadius = double.PositiveInfinity;
      var boundingCircleRelativeOffset = double.PositiveInfinity;
      if (2 * maximumAngleOfOuterVertex < Math.PI)
      {
        boundingCircleRelativeRadius = 1 / Math.Sin(2 * maximumAngleOfOuterVertex);
        boundingCircleRelativeOffset = -1 / Math.Tan(2 * maximumAngleOfOuterVertex);
      }

      return (boundingCircleRelativeRadius, boundingCircleRelativeOffset);

    }

    /// <summary>
    /// Gets the bounding box for a convex hull segment, given the maximum allowed angle between the (old) convex hull segment and the new segment of the concave hull.
    /// </summary>
    /// <param name="segment">The convex hull segment.</param>
    /// <param name="maximumAngleOfOuterVertex">The maximum allowed angle between the (old) convex hull segment and the new segment of the concave hull.</param>
    /// <returns>A bounding box to limit the search for potential candidates for a concave hull.</returns>
    private static Int64BoundingBox? GetBoundingBox(in Int64LineD2DAnnotated segment, in double maximumAngleOfOuterVertex)
    {
      var (radius, offset) = GetBoundingCircleParametersFor(maximumAngleOfOuterVertex);
      return GetBoundingBox(segment, radius, offset);
    }

    /// <summary>
    /// Gets the bounding box for a convex hull segment, given the precalculated relative circle radius and offset
    /// </summary>
    /// <param name="segment">The convex hull segment.</param>
    /// <param name="boundingCircleRelativeRadius">The bounding circle relative radius.</param>
    /// <param name="boundingCircleRelativeOffset">The bounding circle relative offset.</param>
    /// <returns>A bounding box to limit the search for potential candidates for a concave hull.</returns>
    private static Int64BoundingBox? GetBoundingBox(in Int64LineD2DAnnotated segment, in double boundingCircleRelativeRadius, in double boundingCircleRelativeOffset)
    {
      if (double.IsPositiveInfinity(boundingCircleRelativeRadius))
      {
        return null;
      }

      // note: we assume here that the inside of our hull is to the left of the segment
      var centerX = 0.5 * (segment.P0.X + segment.P1.X + (segment.P0.Y - segment.P1.Y) * boundingCircleRelativeOffset);
      var centerY = 0.5 * (segment.P0.Y + segment.P1.Y + (segment.P1.X - segment.P0.X) * boundingCircleRelativeOffset);
      var dim2 = 0.5 * segment.Length * boundingCircleRelativeRadius;

      // Note: the bounding box must at least include the convex hull segment itself,
      // because any point on this segment would fulfill the requirments for a potential hull point
      var leftX = Math.Min(centerX - dim2, Math.Min(segment.P0.X, segment.P1.X));
      var lowerY = Math.Min(centerY - dim2, Math.Min(segment.P0.Y, segment.P1.Y));
      var rightX = Math.Max(centerX + dim2, Math.Max(segment.P0.X, segment.P1.X));
      var upperY = Math.Max(centerY + dim2, Math.Max(segment.P0.Y, segment.P1.Y));

      return new Int64BoundingBox(
        new IntPoint(Math.Floor(leftX), Math.Floor(lowerY)),
        new IntPoint(Math.Ceiling(rightX), Math.Ceiling(upperY))
        );
    }

    /// <summary>
    /// Calculates the convex hull as a list of line segments.
    /// </summary>
    /// <param name="points">The points that form the convex hull.</param>
    /// <returns></returns>
    private List<Int64LineD2DAnnotated> CalculateConvexHullAsLineList(IReadOnlyList<IntPoint> points)
    {
      var hull_convex_edges = new List<Int64LineD2DAnnotated>();

      for (var i = 0; i < ConvexHullPoints.Count - 1; i++)
      {
        hull_convex_edges.Add(new Int64LineD2DAnnotated(ConvexHullPoints[i], ConvexHullPoints[i + 1]));
      }
      hull_convex_edges.Add(new Int64LineD2DAnnotated(ConvexHullPoints[ConvexHullPoints.Count - 1], ConvexHullPoints[0]));

      return hull_convex_edges;
    }


    /// <summary>
    /// Inserts the line in the line list. The line list must already be sorted by line length, ascending.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="lineList">The line list.</param>
    private static void InsertSortedByLengthAscending(Int64LineD2DAnnotated line, List<Int64LineD2DAnnotated> lineList)
    {
      var idx = lineList.BinarySearch(line, LengthComparer.Instance);
      if (idx >= 0)
      {
        lineList.Insert(idx, line);
      }
      else
      {
        idx = ~idx;
        lineList.Insert(idx, line);
      }
    }

    /// <summary>
    /// Tests if a given point touches any of the line segments given in the list.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="otherLines">The other lines.</param>
    /// <returns></returns>
    private bool DoesPointTouchAnyOtherLine((IntPoint point, int index) point, List<Int64LineD2DAnnotated> otherLines)
    {
      foreach (var otherEdge in otherLines)
      {
        if (DoesPointTouchLine(point.point, otherEdge.Line))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Tests if either segment e2 or e3 touches or crosses any other segment given in the list. Touches with the
    /// neighboring segments (e2.P0 with linesegment.P1, and e3.P1 with lineSegment.P0 is allowed).
    /// </summary>
    /// <param name="e2">The first segment to test.</param>
    /// <param name="e3">The other segment to test.</param>
    /// <param name="otherEdges">List of edges to test against.</param>
    /// <returns></returns>
    private bool DoLinesIntersectAnyOtherLine(Int64LineD2DAnnotated e2, Int64LineD2DAnnotated e3, List<Int64LineD2DAnnotated> otherEdges)
    {
      foreach (var otherEdge in otherEdges)
      {
        if (DoLinesIntersectOrTouch(e2.Line, otherEdge.Line))
        {
          if (e2.I0 != otherEdge.I1) // touching is ok if e2.P0 is the same point as the other edge's P1, because the adjacent edges have to touch
          {
            return true;
          }
        }
        if (DoLinesIntersectOrTouch(e3.Line, otherEdge.Line))
        {
          if (e3.I1 != otherEdge.I0) // touching is ok if e3.P1 is the same point as the other edge's P0, because the adjacent edges have to touch
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Gets the points in the vicinity of the given segment.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <param name="pointsNotOnHull">The points not part of the hull.</param>
    /// <returns>Currently, all points are returned. May be changed in future versions when implementing partitioning, as described in the paper by E. Rosén et al.</returns>
    private List<(IntPoint point, int index)> GetPointsNearby(Int64LineD2DAnnotated segment, List<(IntPoint point, int index)> pointsNotOnHull)
    {
      return pointsNotOnHull;
    }

    /// <summary>
    /// Gets the hull points from the calculated line segments. The challenge here is that the line segments that forms
    /// the concave hull are in arbitrary order. So for each line segment we need to find another line segment where the end point of the first segment
    /// is contained in the other line segment (either as start or end point).
    /// </summary>
    /// <returns>A fresh list of points that form the concave hull.</returns>
    private static List<(IntPoint point, int index)> GetHullPoints(List<Int64LineD2DAnnotated> hull_concave_edges)
    {
      // Dictionary where the key is the point.ID.
      // The value is a tuple of the two indices of the line segments which have a point with this ID either as start or end point
      var dict = new Dictionary<int, (int idx1, int idx2)>();

      // fill this dictionary
      for (var i = 0; i < hull_concave_edges.Count; ++i)
      {
        var id = hull_concave_edges[i].I0;
        if (!dict.Keys.Contains(id))
        {
          dict.Add(id, (i, -1));
        }
        else
        {
          var (idx1, _) = dict[id];
          dict[id] = (idx1, i);
        }

        id = hull_concave_edges[i].I1;
        if (!dict.Keys.Contains(id))
        {
          dict.Add(id, (i, -1));
        }
        else
        {
          var (idx1, _) = dict[id];
          dict[id] = (idx1, i);
        }
      }


      // now build the list of the points that forms the concave hull

      var hullPoints = new List<(IntPoint point, int index)>(hull_concave_edges.Count);
      var subidx0 = 0; // the index of the point of the current line segment that should be added next
      var subidx1 = 1; // the index of the other point of the current line segment
      for (int i = 0, j = 0; i < hull_concave_edges.Count; ++i)
      {
        hullPoints.Add(hull_concave_edges[j].GetPointWithIndex(subidx0)); // add the point of the current line segment to the concave hull.

        var id = hull_concave_edges[j].GetIndex(subidx1); // ID of the other point of the current line segment

        // find another segment with a point which has this id
        var tuple = dict[id];
        var new_j = tuple.idx1;

        j = (j != tuple.idx1) ? tuple.idx1 : tuple.idx2; // use this new index of the tuple, which is not our own current segment index

        // find the index of the point in the next segment which has the id
        if (hull_concave_edges[j].I0 == id)
        {
          subidx0 = 0;
          subidx1 = 1;
        }
        else
        {
          subidx0 = 1;
          subidx1 = 0;
        }
      }

      return hullPoints;
    }

    /// <summary>
    /// Executes test that will throw an exception if the current hull is not in an allowed state.
    /// </summary>
    /// <param name="hull_concave_edges_temp">The hull concave edges temporary.</param>
    /// <param name="hull_concave_edges_final">The hull concave edges final.</param>
    /// <param name="allPoints">All points.</param>
    private void Debug_ExecuteTestsAfterInsertion(List<Int64LineD2DAnnotated> hull_concave_edges_temp, List<Int64LineD2DAnnotated> hull_concave_edges_final, List<(IntPoint point, int index)> allPoints)
    {
      var listAllEdged = new List<Int64LineD2DAnnotated>();
      listAllEdged.AddRange(hull_concave_edges_temp);
      listAllEdged.AddRange(hull_concave_edges_final);

      var hull = GetHullPoints(listAllEdged);

      // Test that the convex hull does not contain doubled points
      var hash = new HashSet<(int, int)>();
      for (var i = 0; i < hull.Count; ++i)
      {
        if (!(false == hash.Contains(((int)hull[i].point.X, (int)hull[i].point.Y))))
        {
          throw new InvalidProgramException();
        }

        hash.Add(((int)hull[i].point.X, (int)hull[i].point.Y));
      }


      // Various tests with clipper
      var clipperPoly = new List<ClipperLib.IntPoint>(hull.Select(dp => new ClipperLib.IntPoint(dp.point.X, dp.point.Y)));

      // The area should be != 0
      if (!(Math.Abs(ClipperLib.Clipper.Area(clipperPoly)) > 0)) // Area should be != 0
      {
        throw new InvalidProgramException();
      }

      if (!(ClipperLib.Clipper.Area(clipperPoly) > 0)) // Polygon should be positive oriented
      {
        throw new InvalidProgramException();
      }

      // The polygon should be simple
      var clipperPolys = ClipperLib.Clipper.SimplifyPolygon(clipperPoly);
      if (0 == clipperPolys.Count)
      {
        throw new Exception();
      }

      if (!(1 == clipperPolys.Count)) // if polygon is simple, it can not be transformed into two or more polygons
      {
        // note: Clipper simplifies polygons even if the segments do not touch, but are near enough to each other
        // thus when clipper has created two or more polygons, we need some check with
        // or own library if this is "wrong" alarm

        Debug_CheckHullForIntersections(hull);
      }

      if (!(clipperPolys[0].Count <= clipperPoly.Count)) // the resulting polygon should have the same number of points than the original one
      {
        throw new InvalidProgramException();
      }


      // Test whether all points are on the hull or inside the hull
      for (var i = 0; i < allPoints.Count; ++i)
      {
        if (!(0 != ClipperLib.Clipper.PointInPolygon(new ClipperLib.IntPoint(allPoints[i].point.X, allPoints[i].point.Y), clipperPoly)))
        {
          throw new InvalidProgramException();
        }
      }
    }

    /// <summary>
    /// Checks the hull segments for intersections with each other. The computational effort is in the order of n².
    /// </summary>
    /// <param name="hull">The hull to check.</param>
    private static void Debug_CheckHullForIntersections(IReadOnlyList<(IntPoint point, int index)> hull)
    {
      var segments = new Int64LineD2DAnnotated[hull.Count];

      for (var i = 0; i < hull.Count - 1; ++i)
      {
        segments[i] = new Int64LineD2DAnnotated(hull[i], hull[i + 1]);
      }

      segments[segments.Length - 1] = new Int64LineD2DAnnotated(hull[hull.Count - 1], hull[0]);

      for (var i = 0; i < segments.Length - 1; ++i)
      {
        for (var j = i + 2; j < segments.Length; ++j)
        {

          // two adjacent segments are allowed to touch,
          // thus either j==i+1, or i==0 and j==segments.Count-1 are allowed
          if (!(i == 0 && j == segments.Length - 1))
          {
            if (ConcaveHull.DoLinesIntersectOrTouch(segments[i].Line, segments[j].Line))
            {
              throw new Exception();
            }
          }
        }
      }
    }

  }
}
