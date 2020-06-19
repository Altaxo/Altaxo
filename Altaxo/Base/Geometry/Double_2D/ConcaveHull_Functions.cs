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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Geometry.Double_2D
{
  public partial class ConcaveHull
  {

    private static bool verticalIntersection(LineD2DAnnotated lineA, LineD2DAnnotated lineB)
    {
      /* lineA is vertical */
      double y_intersection;
      if ((lineB.P0.X > lineA.P0.X) && (lineA.P0.X > lineB.P1.X) ||
              ((lineB.P1.X > lineA.P0.X) && (lineA.P0.X > lineB.P0.X)))
      {
        y_intersection = (((lineB.P1.Y - lineB.P0.Y) * (lineA.P0.X - lineB.P0.X)) / (lineB.P1.X - lineB.P0.X)) + lineB.P0.Y;
        return ((lineA.P0.Y > y_intersection) && (y_intersection > lineA.P1.Y))
            || ((lineA.P1.Y > y_intersection) && (y_intersection > lineA.P0.Y));
      }
      else
      {
        return false;
      }
    }

    private static bool intersection(LineD2DAnnotated lineA, LineD2DAnnotated lineB)
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
      double X;

      if (Math.Max(lineA.P0.X, lineA.P1.X) < Math.Min(lineB.P0.X, lineB.P1.X))
      {
        return false; //Not a chance of intersection
      }

      dif = lineA.P0.X - lineA.P1.X;
      if (dif != 0)
      { //Avoids dividing by 0
        A1 = (lineA.P0.Y - lineA.P1.Y) / dif;
      }
      else
      {
        //Segment is vertical
        A1 = 9999999;
      }

      dif = lineB.P0.X - lineB.P1.X;
      if (dif != 0)
      { //Avoids dividing by 0
        A2 = (lineB.P0.Y - lineB.P1.Y) / dif;
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

      b1 = lineA.P0.Y - (A1 * lineA.P0.X);
      b2 = lineB.P0.Y - (A2 * lineB.P0.X);
      X = Math.Round((b2 - b1) / (A1 - A2), 4);
      if ((X <= Math.Max(Math.Min(lineA.P0.X, lineA.P1.X), Math.Min(lineB.P0.X, lineB.P1.X))) ||
          (X >= Math.Min(Math.Max(lineA.P0.X, lineA.P1.X), Math.Max(lineB.P0.X, lineB.P1.X))))
      {
        return false; //Out of bound
      }
      else
      {
        return true;
      }
    }

    private static List<LineD2DAnnotated> setConcave(LineD2DAnnotated line, List<PointD2DAnnotated> nearbyPoints, List<LineD2DAnnotated> concave_hull, double concavity, bool isSquareGrid)
    {
      /* Adds a middlepoint to a line (if there can be one) to make it concave */
      var concave = new List<LineD2DAnnotated>();
      double cos1, cos2;
      double sumCos = -2;
      PointD2DAnnotated? middle_point = null;
      bool edgeIntersects;
      var count = 0;
      var count_line = 0;

      while (count < nearbyPoints.Count)
      {
        edgeIntersects = false;
        cos1 = getCos(nearbyPoints[count], line.P0, line.P1);
        cos2 = getCos(nearbyPoints[count], line.P1, line.P0);
        if (cos1 + cos2 >= sumCos && (cos1 > concavity && cos2 > concavity))
        {
          count_line = 0;
          while (!edgeIntersects && count_line < concave_hull.Count)
          {
            edgeIntersects = (intersection(concave_hull[count_line], new LineD2DAnnotated(nearbyPoints[count], line.P0))
                || (intersection(concave_hull[count_line], new LineD2DAnnotated(nearbyPoints[count], line.P1))));
            count_line++;
          }
          if (!edgeIntersects)
          {
            // Prevents from getting sharp angles between middlepoints
            var nearNodes = getHullNearbyNodes(line, concave_hull);
            if ((getCos(nearbyPoints[count], nearNodes[0], line.P0) < -concavity) &&
                (getCos(nearbyPoints[count], nearNodes[1], line.P1) < -concavity))
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
        concave.Add(new LineD2DAnnotated(middle_point.Value, line.P0));
        concave.Add(new LineD2DAnnotated(middle_point.Value, line.P1));
      }
      return concave;
    }

    private static bool tangentToHull(LineD2DAnnotated line_treated, PointD2DAnnotated node, double cos1, double cos2, List<LineD2DAnnotated> concave_hull)
    {
      /* A new middlepoint could (rarely) make a segment that's tangent to the hull.
       * This method detects these situations
       * I suggest turning this method of if you are not using square grids or if you have a high dot density
       * */
      var isTangent = false;
      double current_cos1;
      double current_cos2;
      double edge_length;
      var nodes_searched = new List<int>();
      LineD2DAnnotated line;
      PointD2DAnnotated node_in_hull;
      var count_line = 0;
      var count_node = 0;

      edge_length = LineD2DAnnotated.GetDistance(node, line_treated.P0) + LineD2DAnnotated.GetDistance(node, line_treated.P1);


      while (!isTangent && count_line < concave_hull.Count)
      {
        line = concave_hull[count_line];
        while (!isTangent && count_node < 2)
        {
          node_in_hull = line[count_node];
          if (!nodes_searched.Contains(node_in_hull.ID))
          {
            if (node_in_hull.ID != line_treated.P0.ID && node_in_hull.ID != line_treated.P1.ID)
            {
              current_cos1 = getCos(node_in_hull, line_treated.P0, line_treated.P1);
              current_cos2 = getCos(node_in_hull, line_treated.P1, line_treated.P0);
              if (current_cos1 == cos1 || current_cos2 == cos2)
              {
                isTangent = (LineD2DAnnotated.GetDistance(node_in_hull, line_treated.P0) + LineD2DAnnotated.GetDistance(node_in_hull, line_treated.P1) < edge_length);
              }
            }
          }
          nodes_searched.Add(node_in_hull.ID);
          count_node++;
        }
        count_node = 0;
        count_line++;
      }
      return isTangent;
    }

    private static double Pow2(double x) => x * x;

    private static double getCos(PointD2DAnnotated a, PointD2DAnnotated b, PointD2DAnnotated o)
    {
      /* Law of cosines */
      var aPow2 = Pow2(a.X - o.X) + Pow2(a.Y - o.Y);
      var bPow2 = Pow2(b.X - o.X) + Pow2(b.Y - o.Y);
      var cPow2 = Pow2(a.X - b.X) + Pow2(a.Y - b.Y);
      return Math.Round((aPow2 + bPow2 - cPow2) / (2 * Math.Sqrt(aPow2 * bPow2)), 4);
    }

    private static (int minx, int miny, int maxx, int maxy) getBoundary(LineD2DAnnotated line, int scaleFactor)
    {
      /* Giving a scaleFactor it returns an area around the line
       * where we will search for nearby points
       * */
      var aNode = line.P0;
      var bNode = line.P1;
      var min_x_position = (int)Math.Floor(Math.Min(aNode.X, bNode.X) / scaleFactor);
      var min_y_position = (int)Math.Floor(Math.Min(aNode.Y, bNode.Y) / scaleFactor);
      var max_x_position = (int)Math.Floor(Math.Max(aNode.X, bNode.X) / scaleFactor);
      var max_y_position = (int)Math.Floor(Math.Max(aNode.Y, bNode.Y) / scaleFactor);

      return (min_x_position, min_y_position, max_x_position, max_y_position);
    }

    private static List<PointD2DAnnotated> getNearbyPoints(LineD2DAnnotated line, List<PointD2DAnnotated> nodeList, int scaleFactor)
    {
      /* The bigger the scaleFactor the more points it will return
       * Inspired by this precious algorithm:
       * http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf
       * Be carefull: if it's too small it will return very little points (or non!),
       * if it's too big it will add points that will not be used and will consume time
       * */
      var nearbyPoints = new List<PointD2DAnnotated>();
      var tries = 0;
      int node_x_rel_pos;
      int node_y_rel_pos;

      while (tries < 2 && nearbyPoints.Count == 0)
      {
        var boundary = getBoundary(line, scaleFactor);
        foreach (var node in nodeList)
        {
          //Not part of the line
          if (!(node.X == line.P0.X && node.Y == line.P0.Y ||
              node.X == line.P1.X && node.Y == line.P1.Y))
          {
            node_x_rel_pos = (int)Math.Floor(node.X / scaleFactor);
            node_y_rel_pos = (int)Math.Floor(node.Y / scaleFactor);
            //Inside the boundary
            if (node_x_rel_pos >= boundary.minx && node_x_rel_pos <= boundary.maxx &&
                node_y_rel_pos >= boundary.miny && node_y_rel_pos <= boundary.maxy)
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

    private static PointD2DAnnotated[] getHullNearbyNodes(LineD2DAnnotated line, List<LineD2DAnnotated> concave_hull)
    {
      /* Return previous and next nodes to a line in the hull */
      var nearbyHullNodes = new PointD2DAnnotated[2];
      var leftNodeID = line.P0.ID;
      var rightNodeID = line.P1.ID;
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
          currentID = concave_hull[line_count][position].ID;
          if (currentID == leftNodeID &&
              concave_hull[line_count][opposite_position].ID != rightNodeID)
          {
            nearbyHullNodes[0] = concave_hull[line_count][opposite_position];
            nodesFound++;
          }
          else if (currentID == rightNodeID &&
             concave_hull[line_count][opposite_position].ID != leftNodeID)
          {
            nearbyHullNodes[1] = concave_hull[line_count][opposite_position];
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
