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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles
{
  /// <summary>
  /// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class StraightConnection : LineConnectionStyleBase
  {
    public static StraightConnection Instance { get; private set; } = new StraightConnection();

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StraightConnection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return Instance;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Template to make a line draw.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="allLinePoints">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
    /// <param name="range">The plot range to use.</param>
    /// <param name="layer">Graphics layer.</param>
    /// <param name="linePen">The pen to draw the line.</param>
    /// <param name="symbolGap">The size of the symbol gap. Argument is the original index of the data. The return value is the absolute symbol gap at this index.
    /// This function is null if no symbol gap is required.</param>
    /// <param name="skipFrequency">Skip frequency. Normally 1, thus all gaps are taken into account. If 2, only every 2nd gap is taken into account, and so on.</param>
    /// <param name="connectCircular">If true, there is a line connecting the start and the end of the range.</param>
    /// <param name="linePlotStyle">The line plot style.</param>
    public override void PaintOneRange(
      Graphics g,
      PointF[] allLinePoints,
      IPlotRange range,
      IPlotArea layer,
      PenCacheGdi.GdiPen linePen,
      Func<int, double> symbolGap,
      int skipFrequency,
      bool connectCircular,
      LinePlotStyle linePlotStyle)
    {
      if (range.Length <= 1)
        return; // seems to be only a single point, thus no connection possible

      PointF[] circularLinePoints;
      int indexBasePlotPoints; // index of the first plot point of this range in circularLinePoints array
      if (connectCircular) // we have to copy the array in order to append the first point to the end
      {
        // otherwise, make a new array
        circularLinePoints = new PointF[range.Length + 1];
        Array.Copy(allLinePoints, range.LowerBound, circularLinePoints, 0, range.Length); // Extract
        circularLinePoints[circularLinePoints.Length - 1] = circularLinePoints[0];
        indexBasePlotPoints = 0;
      }
      else // use the array directly without copying
      {
        circularLinePoints = allLinePoints;
        indexBasePlotPoints = range.LowerBound;
      }

      if (null != symbolGap)
      {
        foreach (var segmentRange in GetSegmentRanges(range, symbolGap, skipFrequency, connectCircular))
        {
          if (segmentRange.IsFullRangeClosedCurve) // test if this is a closed polygon without any gaps -> draw a closed polygon and return
          {
            // use the whole circular arry to draw a closed polygon without any gaps
            g.DrawPolygon(linePen, circularLinePoints);
          }
          else if (segmentRange.Length == 1) // special case only one line segment
          {
            int plotIndexAtStart = segmentRange.IndexAtSubRangeStart + indexBasePlotPoints;
            int plotIndexAtEnd = segmentRange.IndexAtSubRangeEnd + indexBasePlotPoints;

            var xdiff = circularLinePoints[plotIndexAtEnd].X - circularLinePoints[plotIndexAtStart].X;
            var ydiff = circularLinePoints[plotIndexAtEnd].Y - circularLinePoints[plotIndexAtStart].Y;
            var diffLength = System.Math.Sqrt(xdiff * xdiff + ydiff * ydiff);

            var relAtStart = (float)(0.5 * segmentRange.GapAtSubRangeStart / diffLength); // 0.5 because symbolGap is the full gap between two lines, thus between the symbol center and the beginning of the line it is only 1/2
            var relAtEnd = (float)(0.5 * segmentRange.GapAtSubRangeEnd / diffLength); // 0.5 because symbolGap is the full gap between two lines, thus between the symbol center and the beginning of the line it is only 1/2

            if ((relAtStart + relAtEnd) < 1) // a line only appears if sum of the gaps  is smaller than 1
            {
              var startx = circularLinePoints[plotIndexAtStart].X + relAtStart * xdiff;
              var starty = circularLinePoints[plotIndexAtStart].Y + relAtStart * ydiff;
              var stopx = circularLinePoints[plotIndexAtEnd].X - relAtEnd * xdiff;
              var stopy = circularLinePoints[plotIndexAtEnd].Y - relAtEnd * ydiff;

              g.DrawLine(linePen, startx, starty, stopx, stopy);
            }
          }
          else
          {
            int plotIndexAtStart = segmentRange.IndexAtSubRangeStart + indexBasePlotPoints;
            int plotIndexAtEnd = segmentRange.IndexAtSubRangeEnd + indexBasePlotPoints;
            var shortenedPolyline = circularLinePoints.ShortenPartialPolylineByDistanceFromStartAndEnd(plotIndexAtStart, plotIndexAtEnd, segmentRange.GapAtSubRangeStart / 2, segmentRange.GapAtSubRangeEnd / 2);

            if (null != shortenedPolyline)
              g.DrawLines(linePen, shortenedPolyline);
          }
        } // end for
      }
      else // no line symbol gap required, so we can use DrawLines or DrawPolygon to draw the lines
      {
        if (connectCircular) // array was already copied from original array
        {
          g.DrawPolygon(linePen, circularLinePoints);
        }
        else if (indexBasePlotPoints == 0 && range.Length == circularLinePoints.Length) // can use original array directly
        {
          g.DrawLines(linePen, circularLinePoints);
        }
        else
        {
          circularLinePoints = new PointF[range.Length];
          Array.Copy(allLinePoints, range.LowerBound, circularLinePoints, 0, range.Length);
          g.DrawLines(linePen, circularLinePoints);
        }
      }
    }

    /// <inheritdoc/>
    public override void FillOneRange(
    GraphicsPath gp,
      Processed2DPlotData pdata,
      IPlotRange range,
      IPlotArea layer,
      CSPlaneID fillDirection,
      bool ignoreMissingDataPoints,
      bool connectCircular,
      PointF[] allLinePoints,
      double logicalShiftX,
      double logicalShiftY
    )
    {
      var circularLinePoints = new PointF[range.Length + (connectCircular ? 1 : 0)];
      Array.Copy(allLinePoints, range.LowerBound, circularLinePoints, 0, range.Length); // Extract
      if (connectCircular)
        circularLinePoints[circularLinePoints.Length - 1] = circularLinePoints[0];

      if (connectCircular)
      {
        gp.AddLines(circularLinePoints);
        gp.CloseFigure();
      }
      else // not circular
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        r0.RX += logicalShiftX;
        r0.RY += logicalShiftY;

        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
        gp.AddLines(circularLinePoints);
        Logical3D r1 = layer.GetLogical3D(pdata, connectCircular ? range.OriginalFirstPoint : range.OriginalLastPoint);
        r1.RX += logicalShiftX;
        r1.RY += logicalShiftY;

        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, fillDirection, r1, r0);
        gp.CloseFigure();
      }
    }
  }
}
