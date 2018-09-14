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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Contains code to generate triangle geometry for solid straight lines.
  /// </summary>
  public struct SolidStraightLine
  {
    private SolidStraightLineDashSegment _dashSegment;

    public void AddGeometry(
    Action<PointD3D, VectorD3D> AddPositionAndNormal,
    Action<int, int, int, bool> AddIndices,
    ref int vertexIndexOffset,
    PenX3D pen,
    LineD3D line
    )
    {
      var westnorth = PolylineMath3D.GetWestNorthVectors(line);
      var westVector = westnorth.Item1;
      var northVector = westnorth.Item2;

      if (pen.DashPattern is DashPatterns.Solid)
      {
        // draw without a dash pattern - we consider the whole line as one dash segment, but instead of dash caps, with line caps
        _dashSegment.Initialize(pen.CrossSection, pen.Thickness1, pen.Thickness2, pen.LineStartCap, pen.LineEndCap, westVector, northVector, line);
        _dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, line, null, null);
      }
      else
      {
        // draw with a dash pattern
        _dashSegment.Initialize(pen, westVector, northVector, line);

        double dashOffset = 0;
        PointD3D lineStart = line.P0;
        PointD3D lineEnd = line.P1;

        var lineVector = line.LineVector;
        double lineLength = lineVector.Length;
        var lineVectorNormalized = lineVector / lineLength;

        // calculate the real start and end of the line, taking the line start and end cap length into account
        if (null != pen.LineStartCap)
        {
          var v = pen.LineStartCap.GetAbsoluteBaseInset(pen.Thickness1, pen.Thickness2);

          if (v < 0)
          {
            dashOffset = -v;
            lineStart += -v * lineVectorNormalized;
            lineLength += v;
          }
        }

        if (null != pen.LineEndCap)
        {
          var v = pen.LineEndCap.GetAbsoluteBaseInset(pen.Thickness1, pen.Thickness2);
          if (v < 0)
          {
            lineEnd += v * lineVectorNormalized;
            lineLength += v;
          }
        }

        // now draw the individual dash segments

        bool wasLineStartCapDrawn = false;
        bool wasLineEndCapDrawn = false;

        if (lineLength > 0)
        {
          foreach (var seg in Math3D.DissectStraightLineWithDashPattern(new LineD3D(lineStart, lineEnd), pen.DashPattern, pen.DashPattern.DashOffset, Math.Max(pen.Thickness1, pen.Thickness2), dashOffset))
          {
            if (seg.P0 == lineStart) // this is the start of the line, thus we must use the lineStartCap instead of the dashStartCap
            {
              _dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, seg, pen.LineStartCap, null);
              wasLineStartCapDrawn = true;
            }
            else if (seg.P1 == lineEnd) // this is the end of the line, thus we must use the lineEndCap instead of the dashEndCap
            {
              _dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, seg, null, pen.LineEndCap);
              wasLineEndCapDrawn = true;
            }
            else // this is a normal dashSegment, thus we can use dashStartCap and dashEndCap
            {
              _dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, seg, null, null);
            }
          }
        }

        object temporaryStorageSpace = null;

        // if the start cap was not drawn before, it must be drawn now
        if (!wasLineStartCapDrawn && null != pen.LineStartCap)
        {
          pen.LineStartCap.AddGeometry(
            AddPositionAndNormal,
            AddIndices,
            ref vertexIndexOffset,
            true,
            lineStart,
            westVector,
            northVector,
            lineVectorNormalized,
            pen.CrossSection,
            null,
            null,
            ref temporaryStorageSpace);
        }

        // if the end cap was not drawn before, it must be drawn now
        if (!wasLineEndCapDrawn && null != pen.LineEndCap)
        {
          pen.LineEndCap.AddGeometry(
            AddPositionAndNormal,
            AddIndices,
            ref vertexIndexOffset,
            false,
            lineEnd,
            westVector,
            northVector,
            lineVectorNormalized,
            pen.CrossSection,
            null,
            null,
            ref temporaryStorageSpace);
        }
      }
    }
  }
}
