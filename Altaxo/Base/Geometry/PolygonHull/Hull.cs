/*
 *
Adapted from: https://github.com/Liagson/ConcaveHullGenerator
The MIT License (MIT)
Copyright (c) Alberto Aliaga

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

using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Geometry.PolygonHull
{
  public class Hull
  {
    public int _scaleFactor;
    public List<Node> _unused_nodes = new List<Node>();
    public List<Line> _hull_edges = new List<Line>();
    public List<Line> _hull_concave_edges = new List<Line>();

    private static List<Line> GetHullAsLineList(IEnumerable<Node> nodes)
    {
      // var convexH = new List<Node>();
      var exitLines = new List<Line>();

      var convexH = GrahamScan.GetConvexHull(nodes);
      for (var i = 0; i < convexH.Count - 1; i++)
      {
        exitLines.Add(new Line(convexH[i], convexH[i + 1]));
      }
      exitLines.Add(new Line(convexH[0], convexH[convexH.Count - 1]));
      return exitLines;
    }

    public Hull(IEnumerable<Node> nodes)
    {
      _unused_nodes = new List<Node>(nodes);
      _hull_edges = GetHullAsLineList(nodes);

      // from _unused_nodes remove all nodes that are part of the convex hull
      // so that only those nodes that are not part of the convex hull remain
      foreach (var line in _hull_edges)
      {
        foreach (var node in line.nodes)
        {
          if (_unused_nodes.Find(a => a.id == node.id) != null)
          {
            _unused_nodes.Remove(_unused_nodes.Where(a => a.id == node.id).First());
          }
        }
      }
    }

    public void SetConcaveHull(decimal concavity, int scaleFactor, bool isSquareGrid)
    {
      /* Run setConvHull before! 
       * Concavity is a value used to restrict the concave angles 
       * it can go from -1 to 1 (it wont crash if you go further)
       * */
      _scaleFactor = scaleFactor;
      _hull_concave_edges = new List<Line>(_hull_edges.OrderByDescending(a => Line.getLength(a.nodes[0], a.nodes[1])).ToList());
      Line selected_edge;
      var aux = new List<Line>();
      ;
      int list_original_size;
      var count = 0;
      var listIsModified = false;
      do
      {
        listIsModified = false;
        count = 0;
        list_original_size = _hull_concave_edges.Count;
        while (count < list_original_size)
        {
          selected_edge = _hull_concave_edges[0];
          _hull_concave_edges.RemoveAt(0);
          aux = new List<Line>();
          if (!selected_edge.isChecked)
          {
            var nearby_points = HullFunctions.getNearbyPoints(selected_edge, _unused_nodes, scaleFactor);
            aux.AddRange(HullFunctions.setConcave(selected_edge, nearby_points, _hull_concave_edges, concavity, isSquareGrid));
            listIsModified = listIsModified || (aux.Count > 1);

            if (aux.Count > 1)
            {
              foreach (var node in aux[0].nodes)
              {
                if (_unused_nodes.Find(a => a.id == node.id) != null)
                {
                  _unused_nodes.Remove(_unused_nodes.Where(a => a.id == node.id).First());
                }
              }
            }
            else
            {
              aux[0].isChecked = true;
            }
          }
          else
          {
            aux.Add(selected_edge);
          }
          _hull_concave_edges.AddRange(aux);
          count++;
        }
        _hull_concave_edges = _hull_concave_edges.OrderByDescending(a => Line.getLength(a.nodes[0], a.nodes[1])).ToList();
        list_original_size = _hull_concave_edges.Count;
      } while (listIsModified);
    }
  }
}
