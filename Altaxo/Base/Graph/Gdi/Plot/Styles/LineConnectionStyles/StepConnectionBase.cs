﻿#region Copyright

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles
{
  /// <summary>
  /// Connects by drawing a horizontal line to the x coordinate of the next point, and then a vertical line. Instances of this class have to be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public abstract class StepConnectionBase : LineConnectionStyleBase
  {
    /// <summary>
    /// Gets the sub points for a given range. For step connections, at least one points needs to be inserted inbetween two original points, for some step connection styles two points.
    /// </summary>
    /// <param name="pdata">The pdata.</param>
    /// <param name="range">The range.</param>
    /// <param name="layer">The layer.</param>
    /// <param name="connectCircular">if set to <c>true</c> [connect circular].</param>
    /// <param name="numberOfPointsPerOriginalPoint">The number of points per original point. For most step styles one additional point is inserted, thus the return value is 2. For some connection styles, two points are inserted inbetween two original points, thus the return value will be 3.</param>
    /// <param name="lastIndex">The last index.</param>
    /// <returns></returns>
    protected abstract PointF[] GetStepPolylinePoints(
    PointF[] pdata,
    IPlotRange range,
    IPlotArea layer,
    bool connectCircular,
    out int numberOfPointsPerOriginalPoint,
    out int lastIndex);

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
      Func<int, double>? symbolGap,
      int skipFrequency,
      bool connectCircular,
      LinePlotStyle linePlotStyle)
    {
      if (range.Length <= 1)
        return; // seems to be only a single point, thus no connection possible
      PointF[] stepPolylinePoints = GetStepPolylinePoints(allLinePoints, range, layer, connectCircular, out var numberOfPointsPerOriginalPoint, out var lastIdx);

      if (symbolGap is not null)
      {
        foreach (var segmentRange in GetSegmentRanges(range, symbolGap, skipFrequency, connectCircular))
        {
          if (segmentRange.IsFullRangeClosedCurve) // test if this is a closed polygon without any gaps -> draw a closed polygon and return
          {
            // use the whole circular arry to draw a closed polygon without any gaps
            g.DrawPolygon(linePen, stepPolylinePoints);
          }
          else
          {
            int plotIndexAtStart = segmentRange.IndexAtSubRangeStart * numberOfPointsPerOriginalPoint;
            int plotIndexAtEnd = segmentRange.IndexAtSubRangeEnd * numberOfPointsPerOriginalPoint;
            var shortenedPolyline = stepPolylinePoints.ShortenPartialPolylineByDistanceFromStartAndEnd(plotIndexAtStart, plotIndexAtEnd, segmentRange.GapAtSubRangeStart / 2, segmentRange.GapAtSubRangeEnd / 2);

            if (shortenedPolyline is not null)
              g.DrawLines(linePen, shortenedPolyline);
          }
        }
      }
      else
      {
        if (connectCircular)
          g.DrawPolygon(linePen, stepPolylinePoints);
        else
          g.DrawLines(linePen, stepPolylinePoints);
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
      if (range.Length < 2)
        return;
      PointF[] linepts = GetStepPolylinePoints(allLinePoints, range, layer, connectCircular, out var numberOfPointsPerOriginalPoint, out var lastIdx);
      FillOneRange(gp, pdata, range, layer, fillDirection, linepts, connectCircular, allLinePoints, logicalShiftX, logicalShiftY);
    }

    /// <summary>
    /// Template to get a fill path.
    /// </summary>
    /// <param name="gp">Graphics path to fill with data.</param>
    /// <param name="pdata">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
    /// <param name="range">The plot range to use.</param>
    /// <param name="layer">Graphics layer.</param>
    /// <param name="fillDirection">Designates a bound to fill to.</param>
    /// <param name="linePoints">The points that mark the line.</param>
    /// <param name="connectCircular">If true, a circular connection is drawn.</param>
    /// <param name="allLinePointsShiftedAlready">The plot positions, already shifted when a logical shift needed to be applied. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
    /// <param name="logicalShiftX">The logical shift in x-direction.</param>
    /// <param name="logicalShiftY">The logical shift in x-direction.</param>
    public virtual void FillOneRange(
    GraphicsPath gp,
      Processed2DPlotData pdata,
      IPlotRange range,
      IPlotArea layer,
      CSPlaneID fillDirection,
      PointF[] linePoints,
      bool connectCircular,
      PointF[] allLinePointsShiftedAlready,
      double logicalShiftX,
      double logicalShiftY
    )
    {
      if (connectCircular)
      {
        gp.AddLines(linePoints);
        gp.CloseFigure();
      }
      else
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        r0.RX += logicalShiftX;
        r0.RY += logicalShiftY;

        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
        gp.AddLines(linePoints);

        Logical3D r1 = layer.GetLogical3D(pdata, range.OriginalLastPoint);
        r1.RX += logicalShiftX;
        r1.RY += logicalShiftY;

        layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, fillDirection);
        layer.CoordinateSystem.GetIsolineOnPlane(gp, fillDirection, r1, r0);
        gp.CloseFigure();
      }
    }
  }
}
