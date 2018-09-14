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
using System.Drawing;
using System.Linq;
using System.Text;
using Altaxo.Geometry;
using Poly2Tri;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Represents the solid geometry of a text string.
  /// </summary>
  public class SolidText
  {
    private string _text;
    private double _fontSize = 12;
    private double _depth = 3;
    private FontX3D _font;
    private const double distanceCutThreshold = 0.0001;

    public SolidText()
    {
    }

    public SolidText(string text, FontX3D font)
    {
      _text = text;
      _fontSize = font.Size;
      _depth = font.Depth;
      _font = font;
    }

    private double GetPolygonArea(List<PolygonPoint> ptArrayList)
    {
      PolygonPoint pt1 = ptArrayList[0];
      PolygonPoint firstPt = pt1;
      var lastPt = default(PolygonPoint);
      double area = 0.0;
      for (int i = 1; i < ptArrayList.Count; ++i)
      {
        var pt2 = ptArrayList[i];
        area += (((pt2.X - pt1.X) * (pt2.Y + pt1.Y)) / 2);
        pt1 = pt2;
        lastPt = pt1;
      }
      area += (((firstPt.X - lastPt.X) * (firstPt.Y + lastPt.Y)) / 2);
      return area;
    }

    private double GetPolygonArea(PointF[] ptArrayList, int start, int count)
    {
      double area = 0.0;
      var pt1 = ptArrayList[start];
      for (int i = start + 1; i < count; ++i)
      {
        var pt2 = ptArrayList[i + start];
        area += ((pt2.X - pt1.X) * (pt2.Y + pt1.Y));
        pt1 = pt2;
      }
      pt1 = ptArrayList[start];
      var ptl = ptArrayList[start + count - 1];
      area += ((pt1.X - ptl.X) * (pt1.Y + ptl.Y));
      return 0.5 * area;
    }

    private double GetDistance(PolygonPoint a, PolygonPoint b)
    {
      double dx = b.X - a.X;
      double dy = b.Y - a.Y;
      return Math.Sqrt(dx * dx + dy * dy);
    }

    public void AddWithNormals(Func<FontX, char, CharacterGeometry> GetCharacterGeometry, Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int startIndex)
    {
      double positionX = 0;
      double positionY = 0;

      bool isFirstChar = true;

      foreach (var textChar in _text)
      {
        var charGeo = GetCharacterGeometry(_font.Font, textChar);

        double scale = _fontSize / charGeo.FontSize;

        if (isFirstChar)
        {
          isFirstChar = false;
          positionY = scale * (charGeo.LineSpacing - charGeo.BaseLine);
          positionX = scale * (charGeo.LeftSideBearing < 0 ? -charGeo.LeftSideBearing : 0);
        }

        // ------ Front face ------------------
        var frontFace = charGeo.FrontFace;
        var frontFaceIndices = frontFace.TriangleIndices;

        foreach (var pt in frontFace.Vertices)
        {
          AddPositionAndNormal(new PointD3D(pt.X * scale + positionX, pt.Y * scale + positionY, _depth), new VectorD3D(0, 0, 1)); // Front face vertices having z=_depth
        }

        for (int i = 0; i < frontFaceIndices.Length; i += 3)
        {
          AddIndices(frontFaceIndices[i + 0] + startIndex, frontFaceIndices[i + 1] + startIndex, frontFaceIndices[i + 2] + startIndex); // Front face indices
        }

        startIndex += frontFace.Vertices.Length;

        // ------ Back face ------------------

        foreach (var pt in frontFace.Vertices)
        {
          AddPositionAndNormal(new PointD3D(pt.X * scale + positionX, pt.Y * scale + positionY, 0), new VectorD3D(0, 0, -1)); // Back face vertices having z=0
        }

        for (int i = 0; i < frontFaceIndices.Length; i += 3)
        {
          AddIndices(frontFaceIndices[i + 0] + startIndex, frontFaceIndices[i + 2] + startIndex, frontFaceIndices[i + 1] + startIndex); // Front face indices
        }

        startIndex += frontFace.Vertices.Length;

        // ------------ Walls ----------------------------

        foreach (var contour in charGeo.CharacterContour)
        {
          var contourPoints = contour.Points;
          var contourNormals = contour.Normals;

          for (int i = 0; i < contourPoints.Length; ++i)
          {
            var pt = contourPoints[i];
            var nm = contourNormals[i];
            AddPositionAndNormal(new PointD3D(pt.X * scale + positionX, pt.Y * scale + positionY, _depth), new VectorD3D(nm.X, nm.Y, 0));
            AddPositionAndNormal(new PointD3D(pt.X * scale + positionX, pt.Y * scale + positionY, 0), new VectorD3D(nm.X, nm.Y, 0));
          }

          int wallVertexCount = contourPoints.Length * 2;
          for (int i = 0; i < contourPoints.Length; ++i)
          {
            var pt0 = contourPoints[i];
            var pt1 = contourPoints[(i + 1) % contourPoints.Length];

            if (pt0 == pt1)
              continue;

            var i0 = (2 * i);
            var i1 = (2 * i + 1);
            var i2 = (2 * i + 2) % wallVertexCount;
            var i3 = (2 * i + 3) % wallVertexCount;
            AddIndices(i0 + startIndex, i1 + startIndex, i2 + startIndex);
            AddIndices(i1 + startIndex, i3 + startIndex, i2 + startIndex);
          }

          startIndex += wallVertexCount;
        }

        positionX += charGeo.AdvanceWidth * scale;
      }
    }
  }
}
