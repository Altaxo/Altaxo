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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Geometry.PolygonHull
{
  public class HullFunctions
  {

    public static bool verticalIntersection(Line lineA, Line lineB)
    {
      /* lineA is vertical */
      double y_intersection;
      if ((lineB.nodes[0].X > lineA.nodes[0].X) && (lineA.nodes[0].X > lineB.nodes[1].X) ||
              ((lineB.nodes[1].X > lineA.nodes[0].X) && (lineA.nodes[0].X > lineB.nodes[0].X)))
      {
        y_intersection = (((lineB.nodes[1].Y - lineB.nodes[0].Y) * (lineA.nodes[0].X - lineB.nodes[0].X)) / (lineB.nodes[1].X - lineB.nodes[0].X)) + lineB.nodes[0].Y;
        return ((lineA.nodes[0].Y > y_intersection) && (y_intersection > lineA.nodes[1].Y))
            || ((lineA.nodes[1].Y > y_intersection) && (y_intersection > lineA.nodes[0].Y));
      }
      else
      {
        return false;
      }
    }

    public static bool intersection(Line lineA, Line lineB)
    {
      /* Returns true if segments collide
       * If they have in common a segment edge returns false
       * Algorithm obtained from: 
       * http://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
       * Thanks OMG_peanuts !
       * */
      double dif;
      double A1, A2;
      double b1, b2;
      decimal X;

      if (Math.Max(lineA.nodes[0].X, lineA.nodes[1].X) < Math.Min(lineB.nodes[0].X, lineB.nodes[1].X))
      {
        return false; //Not a chance of intersection
      }

      dif = lineA.nodes[0].X - lineA.nodes[1].X;
      if (dif != 0)
      { //Avoids dividing by 0
        A1 = (lineA.nodes[0].Y - lineA.nodes[1].Y) / dif;
      }
      else
      {
        //Segment is vertical
        A1 = 9999999;
      }

      dif = lineB.nodes[0].X - lineB.nodes[1].X;
      if (dif != 0)
      { //Avoids dividing by 0
        A2 = (lineB.nodes[0].Y - lineB.nodes[1].Y) / dif;
      }
      else
      {
        //Segment is vertical
        A2 = 9999999;
      }

      if (A1 == A2)
      {
        return false; //Parallel
      }
      else if (A1 == 9999999)
      {
        return verticalIntersection(lineA, lineB);
      }
      else if (A2 == 9999999)
      {
        return verticalIntersection(lineB, lineA);
      }

      b1 = lineA.nodes[0].Y - (A1 * lineA.nodes[0].X);
      b2 = lineB.nodes[0].Y - (A2 * lineB.nodes[0].X);
      X = Math.Round(System.Convert.ToDecimal((b2 - b1) / (A1 - A2)), 4);
      if ((X <= System.Convert.ToDecimal(Math.Max(Math.Min(lineA.nodes[0].X, lineA.nodes[1].X), Math.Min(lineB.nodes[0].X, lineB.nodes[1].X)))) ||
          (X >= System.Convert.ToDecimal(Math.Min(Math.Max(lineA.nodes[0].X, lineA.nodes[1].X), Math.Max(lineB.nodes[0].X, lineB.nodes[1].X)))))
      {
        return false; //Out of bound
      }
      else
      {
        return true;
      }
    }

    public static List<Line> setConcave(Line line, List<Node> nearbyPoints, List<Line> concave_hull, decimal concavity, bool isSquareGrid)
    {
      /* Adds a middlepoint to a line (if there can be one) to make it concave */
      var concave = new List<Line>();
      decimal cos1, cos2;
      decimal sumCos = -2;
      Node? middle_point = null;
      bool edgeIntersects;
      var count = 0;
      var count_line = 0;

      while (count < nearbyPoints.Count)
      {
        edgeIntersects = false;
        cos1 = getCos(nearbyPoints[count], line.nodes[0], line.nodes[1]);
        cos2 = getCos(nearbyPoints[count], line.nodes[1], line.nodes[0]);
        if (cos1 + cos2 >= sumCos && (cos1 > concavity && cos2 > concavity))
        {
          count_line = 0;
          while (!edgeIntersects && count_line < concave_hull.Count)
          {
            edgeIntersects = (intersection(concave_hull[count_line], new Line(nearbyPoints[count], line.nodes[0]))
                || (intersection(concave_hull[count_line], new Line(nearbyPoints[count], line.nodes[1]))));
            count_line++;
          }
          if (!edgeIntersects)
          {
            // Prevents from getting sharp angles between middlepoints
            var nearNodes = getHullNearbyNodes(line, concave_hull);
            if ((getCos(nearbyPoints[count], nearNodes[0], line.nodes[0]) < -concavity) &&
                (getCos(nearbyPoints[count], nearNodes[1], line.nodes[1]) < -concavity))
            {
              // Prevents inner tangent lines to the concave hull
              if (!(tangentToHull(line, nearbyPoints[count], cos1, cos2, concave_hull) && isSquareGrid))
              {
                sumCos = cos1 + cos2;
                middle_point = nearbyPoints[count];
              }
            }
          }
        }
        count++;
      }
      if (middle_point == null)
      {
        concave.Add(line);
      }
      else
      {
        concave.Add(new Line(middle_point.Value, line.nodes[0]));
        concave.Add(new Line(middle_point.Value, line.nodes[1]));
      }
      return concave;
    }

    public static bool tangentToHull(Line line_treated, Node node, decimal cos1, decimal cos2, List<Line> concave_hull)
    {
      /* A new middlepoint could (rarely) make a segment that's tangent to the hull.
       * This method detects these situations
       * I suggest turning this method of if you are not using square grids or if you have a high dot density
       * */
      var isTangent = false;
      decimal current_cos1;
      decimal current_cos2;
      double edge_length;
      var nodes_searched = new List<int>();
      Line line;
      Node node_in_hull;
      var count_line = 0;
      var count_node = 0;

      edge_length = Line.getLength(node, line_treated.nodes[0]) + Line.getLength(node, line_treated.nodes[1]);


      while (!isTangent && count_line < concave_hull.Count)
      {
        line = concave_hull[count_line];
        while (!isTangent && count_node < 2)
        {
          node_in_hull = line.nodes[count_node];
          if (!nodes_searched.Contains(node_in_hull.Id))
          {
            if (node_in_hull.Id != line_treated.nodes[0].Id && node_in_hull.Id != line_treated.nodes[1].Id)
            {
              current_cos1 = getCos(node_in_hull, line_treated.nodes[0], line_treated.nodes[1]);
              current_cos2 = getCos(node_in_hull, line_treated.nodes[1], line_treated.nodes[0]);
              if (current_cos1 == cos1 || current_cos2 == cos2)
              {
                isTangent = (Line.getLength(node_in_hull, line_treated.nodes[0]) + Line.getLength(node_in_hull, line_treated.nodes[1]) < edge_length);
              }
            }
          }
          nodes_searched.Add(node_in_hull.Id);
          count_node++;
        }
        count_node = 0;
        count_line++;
      }
      return isTangent;
    }

    public static decimal getCos(Node a, Node b, Node o)
    {
      /* Law of cosines */
      var aPow2 = Math.Pow(a.X - o.X, 2) + Math.Pow(a.Y - o.Y, 2);
      var bPow2 = Math.Pow(b.X - o.X, 2) + Math.Pow(b.Y - o.Y, 2);
      var cPow2 = Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
      var cos = (aPow2 + bPow2 - cPow2) / (2 * Math.Sqrt(aPow2 * bPow2));
      return Math.Round(System.Convert.ToDecimal(cos), 4);
    }

    public static int[] getBoundary(Line line, int scaleFactor)
    {
      /* Giving a scaleFactor it returns an area around the line 
       * where we will search for nearby points 
       * */
      var boundary = new int[4];
      var aNode = line.nodes[0];
      var bNode = line.nodes[1];
      var min_x_position = (int)Math.Floor(Math.Min(aNode.X, bNode.X) / scaleFactor);
      var min_y_position = (int)Math.Floor(Math.Min(aNode.Y, bNode.Y) / scaleFactor);
      var max_x_position = (int)Math.Floor(Math.Max(aNode.X, bNode.X) / scaleFactor);
      var max_y_position = (int)Math.Floor(Math.Max(aNode.Y, bNode.Y) / scaleFactor);

      boundary[0] = min_x_position;
      boundary[1] = min_y_position;
      boundary[2] = max_x_position;
      boundary[3] = max_y_position;

      return boundary;
    }

    public static List<Node> getNearbyPoints(Line line, List<Node> nodeList, int scaleFactor)
    {
      /* The bigger the scaleFactor the more points it will return
       * Inspired by this precious algorithm:
       * http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf
       * Be carefull: if it's too small it will return very little points (or non!), 
       * if it's too big it will add points that will not be used and will consume time
       * */
      var nearbyPoints = new List<Node>();
      int[] boundary;
      var tries = 0;
      int node_x_rel_pos;
      int node_y_rel_pos;

      while (tries < 2 && nearbyPoints.Count == 0)
      {
        boundary = getBoundary(line, scaleFactor);
        foreach (var node in nodeList)
        {
          //Not part of the line
          if (!(node.X == line.nodes[0].X && node.Y == line.nodes[0].Y ||
              node.X == line.nodes[1].X && node.Y == line.nodes[1].Y))
          {
            node_x_rel_pos = (int)Math.Floor(node.X / scaleFactor);
            node_y_rel_pos = (int)Math.Floor(node.Y / scaleFactor);
            //Inside the boundary
            if (node_x_rel_pos >= boundary[0] && node_x_rel_pos <= boundary[2] &&
                node_y_rel_pos >= boundary[1] && node_y_rel_pos <= boundary[3])
            {
              nearbyPoints.Add(node);
            }
          }
        }
        //if no points are found we increase the area
        scaleFactor = scaleFactor * 4 / 3;
        tries++;
      }
      return nearbyPoints;
    }

    public static Node[] getHullNearbyNodes(Line line, List<Line> concave_hull)
    {
      /* Return previous and next nodes to a line in the hull */
      var nearbyHullNodes = new Node[2];
      var leftNodeID = line.nodes[0].Id;
      var rightNodeID = line.nodes[1].Id;
      int currentID;
      var nodesFound = 0;
      var line_count = 0;
      var position = 0;
      var opposite_position = 1;

      while (nodesFound < 2)
      {
        position = 0;
        opposite_position = 1;
        while (position < 2)
        {
          currentID = concave_hull[line_count].nodes[position].Id;
          if (currentID == leftNodeID &&
              concave_hull[line_count].nodes[opposite_position].Id != rightNodeID)
          {
            nearbyHullNodes[0] = concave_hull[line_count].nodes[opposite_position];
            nodesFound++;
          }
          else if (currentID == rightNodeID &&
             concave_hull[line_count].nodes[opposite_position].Id != leftNodeID)
          {
            nearbyHullNodes[1] = concave_hull[line_count].nodes[opposite_position];
            nodesFound++;
          }
          position++;
          opposite_position--;
        }
        line_count++;
      }
      return nearbyHullNodes;
    }
  }
}
