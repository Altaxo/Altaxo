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

using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles
{
  /// <summary>
  /// Connects 2 consecutive points by a straight line, then the next two points are not connected, and so on. Instances of this class have to be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class SplineConnection : LineConnectionStyleBase
  {
    public static SplineConnection Instance { get; private set; } = new SplineConnection();

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SplineConnection), 0)]
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
      PenX linePen,
      Func<int, double> symbolGap,
      int skipFrequency,
      bool connectCircular,
      LinePlotStyle linePlotStyle)
    {
      PointF[] subLinePoints;
      if (range.LowerBound == 0 && range.UpperBound == allLinePoints.Length)
      {
        // under optimal conditions we can use allLinePoints directly
        subLinePoints = allLinePoints;
      }
      else
      {
        // otherwise, make a new array
        subLinePoints = new PointF[range.Length];
        Array.Copy(allLinePoints, range.LowerBound, subLinePoints, 0, range.Length); // Extract
      }

      int lastIdx = range.Length - 1;
      var layerSize = layer.Size;

      if (connectCircular)
      {
        if (null != symbolGap)
        {
          // convert points to bezier segments
          var bezierSegments = GdiExtensionMethods.ClosedCardinalSplineToBezierSegments(subLinePoints, subLinePoints.Length);
          var subBezierSegments = new PointF[0];
          foreach (var segmentRange in GetSegmentRanges(range, symbolGap, skipFrequency, connectCircular))
          {
            if (segmentRange.IsFullRangeClosedCurve) // test if this is a closed polygon without any gaps -> draw a closed polygon and return
            {
              // use the whole circular arry to draw a closed polygon without any gaps
              g.DrawClosedCurve(linePen, subLinePoints);
            }
            else
            {
              var subBezierLength = 3 * segmentRange.Length + 1;
              if (subBezierSegments.Length != subBezierLength)
                subBezierSegments = new PointF[subBezierLength];

              Array.Copy(bezierSegments, segmentRange.IndexAtSubRangeStart * 3, subBezierSegments, 0, subBezierLength);
              var shortenedBezierSegments = GdiExtensionMethods.ShortenBezierCurve(subBezierSegments, segmentRange.GapAtSubRangeStart / 2, segmentRange.GapAtSubRangeEnd / 2);

              if (null != shortenedBezierSegments)
              {
                g.DrawBeziers(linePen, shortenedBezierSegments);
              }
            }
          }
        }
        else
        {
          g.DrawClosedCurve(linePen, subLinePoints);
        }
      }
      else // not circular
      {
        if (symbolGap != null)
        {
          // convert points to bezier segments
          var bezierSegments = GdiExtensionMethods.OpenCardinalSplineToBezierSegments(subLinePoints, subLinePoints.Length);
          var subBezierSegments = new PointF[0];

          foreach (var segmentRange in GetSegmentRanges(range, symbolGap, skipFrequency, connectCircular))
          {
            if (segmentRange.IsFullRangeClosedCurve) // test if this is a closed polygon without any gaps -> draw a closed polygon and return
            {
              // use the whole circular arry to draw a closed polygon without any gaps
              g.DrawCurve(linePen, subLinePoints);
            }
            else
            {
              var subBezierLength = 3 * segmentRange.Length + 1;
              if (subBezierSegments.Length != subBezierLength)
                subBezierSegments = new PointF[subBezierLength];

              Array.Copy(bezierSegments, segmentRange.IndexAtSubRangeStart * 3, subBezierSegments, 0, subBezierLength);
              var shortenedBezierSegments = GdiExtensionMethods.ShortenBezierCurve(subBezierSegments, segmentRange.GapAtSubRangeStart / 2, segmentRange.GapAtSubRangeEnd / 2);

              if (null != shortenedBezierSegments)
              {
                g.DrawBeziers(linePen, shortenedBezierSegments);
              }
            }
          }
        }
        else
        {
          g.DrawCurve(linePen, subLinePoints);
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
      PointF[] allLinePointsShiftedAlready,
      double logicalShiftX,
      double logicalShiftY
    )
    {
      PointF[] subLinePoints = new PointF[range.Length];
      Array.Copy(allLinePointsShiftedAlready, range.LowerBound, subLinePoints, 0, range.Length); // Extract

      if (connectCircular)
      {
        gp.AddClosedCurve(subLinePoints);
        gp.CloseFigure();
      }
      else
      {
        Logical3D r0 = layer.GetLogical3D(pdata, range.OriginalFirstPoint);
        r0.RX += logicalShiftX;
        r0.RY += logicalShiftY;

        layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
        gp.AddCurve(subLinePoints);
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
