/*
 *
Adapted from: https://github.com/Liagson/ConcaveHullGenerator
The MIT License (MIT)
Copyright (c) Alberto Aliaga
Copyright (c) Dr. Dirk Lellinger

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*
*/

#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Altaxo.Geometry.Double_2D
{
  /// <summary>
  /// Represents the concave hull of a set of 2D points.
  /// </summary>
  public partial class ConcaveHull
  {
    /// <summary>
    /// The convex hull edges as a list of annotated lines.
    /// </summary>
    private IReadOnlyList<LineD2DAnnotated> _hull_convex_edges;

    /// <summary>
    /// The points that form the convex hull.
    /// </summary>
    private IReadOnlyList<PointD2DAnnotated> _convexHullPoints;

    /// <summary>
    /// Gets the points that form the convex hull.
    /// </summary>
    public IReadOnlyList<PointD2DAnnotated> ConvexHullPoints
    {
      get => _convexHullPoints;
    }

    /// <summary>
    /// Gets the points that are not part of the convex hull.
    /// </summary>
    public IReadOnlyList<PointD2DAnnotated> PointsNotOnConvexHull { get; private set; }

    /// <summary>
    /// The points that form the concave hull.
    /// </summary>
    private IReadOnlyList<PointD2DAnnotated> _concaveHullPoints;

    /// <summary>
    /// Gets the points that form the concave hull.
    /// </summary>
    public IReadOnlyList<PointD2DAnnotated> ConcaveHullPoints => _concaveHullPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcaveHull"/> class.
    /// </summary>
    /// <param name="nodes">The points for which to calculate the concave hull.</param>
    /// <param name="concavity">The concavity.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <param name="isSquareGrid">Indicates whether a square grid is used.</param>
    public ConcaveHull(IEnumerable<PointD2DAnnotated> nodes, double concavity, int scaleFactor, bool isSquareGrid)
    {
      CalculateConvexHullAsLineList(nodes);

      var unused_nodes = new List<PointD2DAnnotated>();
      // from _unused_nodes remove all nodes that are part of the convex hull
      // so that only those nodes that are not part of the convex hull remain
      var convexHullIDs = new HashSet<int>(ConvexHullPoints.Select(hullpoint => hullpoint.ID));
      foreach (var point in nodes)
      {
        if (!convexHullIDs.Contains(point.ID))
        {
          unused_nodes.Add(point);
        }
      }
      PointsNotOnConvexHull = unused_nodes;

      SetConcaveHull(concavity, scaleFactor, isSquareGrid);
    }

    /// <summary>
    /// Calculates the convex hull as a list of annotated lines.
    /// </summary>
    /// <param name="nodes">The points for which to calculate the convex hull.</param>
    [MemberNotNull(nameof(_convexHullPoints))]
    [MemberNotNull(nameof(_hull_convex_edges))]
    private void CalculateConvexHullAsLineList(IEnumerable<PointD2DAnnotated> nodes)
    {
      _convexHullPoints = GrahamScan.GetConvexHull(nodes);
      var hull_convex_edges = new List<LineD2DAnnotated>();

      for (var i = 0; i < ConvexHullPoints.Count - 1; i++)
      {
        hull_convex_edges.Add(new LineD2DAnnotated(ConvexHullPoints[i], ConvexHullPoints[i + 1]));
      }
      hull_convex_edges.Add(new LineD2DAnnotated(ConvexHullPoints[ConvexHullPoints.Count - 1], ConvexHullPoints[0]));

      _hull_convex_edges = hull_convex_edges;
    }

    /// <summary>
    /// Sets a concave hull with parameters that could be different from those provided in the constructor.
    /// </summary>
    /// <param name="concavity">The concavity.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <param name="isSquareGrid">Indicates whether a square grid is used.</param>
    [MemberNotNull(nameof(_concaveHullPoints))]
    public void SetConcaveHull(double concavity, int scaleFactor, bool isSquareGrid)
    {


      var unused_nodes = new List<PointD2DAnnotated>(PointsNotOnConvexHull);
      var hull_concave_edges = new List<LineD2DAnnotated>(_hull_convex_edges.OrderByDescending(a => LineD2DAnnotated.GetDistance(a.P0, a.P1)).ToList());
      LineD2DAnnotated selected_edge;
      var aux = new List<LineD2DAnnotated>();
      int list_original_size;
      var count = 0;
      var listIsModified = false;
      do
      {
        listIsModified = false;
        count = 0;
        list_original_size = hull_concave_edges.Count;
        while (count < list_original_size)
        {
          selected_edge = hull_concave_edges[0];
          hull_concave_edges.RemoveAt(0);
          aux = new List<LineD2DAnnotated>();
          if (!selected_edge.IsChecked)
          {
            var nearby_points = getNearbyPoints(selected_edge, unused_nodes, scaleFactor);
            aux.AddRange(setConcave(selected_edge, nearby_points, hull_concave_edges, concavity, isSquareGrid));
            listIsModified = listIsModified || (aux.Count > 1);

            if (aux.Count > 1)
            {
              foreach (var node in aux[0].Points)
              {
                for (var i = unused_nodes.Count - 1; i >= 0; --i)
                {
                  if (unused_nodes[i].ID == node.ID)
                  {
                    unused_nodes.RemoveAt(i);
                  }
                }
              }
            }
            else
            {
              aux[0].IsChecked = true;
            }
          }
          else
          {
            aux.Add(selected_edge);
          }
          hull_concave_edges.AddRange(aux);
          count++;
        }
        hull_concave_edges = hull_concave_edges.OrderByDescending(a => LineD2DAnnotated.GetDistance(a.P0, a.P1)).ToList();
        list_original_size = hull_concave_edges.Count;
      } while (listIsModified);


      _concaveHullPoints = GetHullPoints(hull_concave_edges);

      // free temporary allocations
      unused_nodes = null;
      hull_concave_edges = null; // no longer needed
    }

    /// <summary>
    /// Gets the hull points from the calculated line segments. The challenge here is that the line segments that forms
    /// the concave hull are in arbitrary order. So for each line segment we need to find another line segment where the end point of the first segment
    /// is contained in the other line segment (either as start or end point).
    /// </summary>
    /// <param name="hull_concave_edges">The list of line segments forming the concave hull.</param>
    /// <returns>A fresh list of points that form the concave hull.</returns>
    private List<PointD2DAnnotated> GetHullPoints(List<LineD2DAnnotated> hull_concave_edges)
    {
      // Dictionary where the key is the point.ID.
      // The value is a tuple of the two indices of the line segments which have a point with this ID either as start or end point
      var dict = new Dictionary<int, (int idx1, int idx2)>();

      // fill this dictionary
      for (var i = 0; i < hull_concave_edges.Count; ++i)
      {
        var id = hull_concave_edges[i].P0.ID;
        if (!dict.Keys.Contains(id))
        {
          dict.Add(id, (i, -1));
        }
        else
        {
          var (idx1, _) = dict[id];
          dict[id] = (idx1, i);
        }

        id = hull_concave_edges[i].P1.ID;
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

      var hullPoints = new List<PointD2DAnnotated>(hull_concave_edges.Count);
      var subidx0 = 0; // the index of the point of the current line segment that should be added next
      var subidx1 = 1; // the index of the other point of the current line segment
      for (int i = 0, j = 0; i < hull_concave_edges.Count; ++i)
      {
        hullPoints.Add(hull_concave_edges[j][subidx0]); // add the point of the current line segment to the concave hull.

        var id = hull_concave_edges[j][subidx1].ID; // ID of the other point of the current line segment

        // find another segment with a point which has this id
        var tuple = dict[id];
        var new_j = tuple.idx1;

        j = (j != tuple.idx1) ? tuple.idx1 : tuple.idx2; // use this new index of the tuple, which is not our own current segment index

        // find the index of the point in the next segment which has the id
        if (hull_concave_edges[j].P0.ID == id)
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
  }
}
