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
  /// Connects 2 consecutive points by a straight line, then the next two points are not connected, and so on. Instances of this class have to be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class Segment2Connection : LineConnectionStyleBase
  {
    public static Segment2Connection Instance { get; private set; } = new Segment2Connection();

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Segment2Connection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
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
      Func<int, double>? symbolGap,
      int skipFrequency,
      bool connectCircular,
      LinePlotStyle linePlotStyle)
    {
      PointF[] subLinePoints = Segment2Connection_GetSubPoints(allLinePoints, range, layer, connectCircular, out var lastIdx);

      var gp = new GraphicsPath();
      int i;

      // special efforts are necessary to realize a line/symbol gap
      // I decided to use a path for this
      // and hope that not so many segments are added to the path due
      // to the exclusion criteria that a line only appears between two symbols (rel<0.5)
      // if the symbols do not overlap. So for a big array of points it is very likely
      // that the symbols overlap and no line between the symbols needs to be plotted
      if (null != symbolGap)
      {
        float startx, starty, stopx, stopy;
        for (i = 0; i < lastIdx; i += 2)
        {
          var diff = GdiExtensionMethods.Subtract(subLinePoints[i + 1], subLinePoints[i]);
          var diffLength = GdiExtensionMethods.VectorLength(diff);

          int originalIndex = range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i);
          double gapAtStart = 0 == i % skipFrequency ? symbolGap(originalIndex) : 0;
          double gapAtEnd;
          if ((0 == (i + 1) % skipFrequency) || ((i + 1) == range.Length))
          {
            gapAtEnd = ((i + 1) != range.Length) ? symbolGap(originalIndex + 1) : symbolGap(range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound));
          }
          else
          {
            gapAtEnd = 0;
          }

          var relAtStart = (float)(0.5 * gapAtStart / diffLength); // 0.5 because symbolGap is the full gap between two lines, thus between the symbol center and the beginning of the line it is only 1/2
          var relAtEnd = (float)(0.5 * gapAtEnd / diffLength); // 0.5 because symbolGap is the full gap between two lines, thus between the symbol center and the beginning of the line it is only 1/2
          if ((relAtStart + relAtEnd) < 1) // a line only appears if sum of the gaps  is smaller than 1
          {
            startx = subLinePoints[i].X + relAtStart * diff.X;
            starty = subLinePoints[i].Y + relAtStart * diff.Y;
            stopx = subLinePoints[i + 1].X - relAtEnd * diff.X;
            stopy = subLinePoints[i + 1].Y - relAtEnd * diff.Y;

            gp.AddLine(startx, starty, stopx, stopy);
            gp.StartFigure();
          }
        } // end for
        g.DrawPath(linePen, gp);
        gp.Reset();
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        for (i = 0; i < lastIdx; i += 2)
        {
          gp.AddLine(subLinePoints[i].X, subLinePoints[i].Y, subLinePoints[i + 1].X, subLinePoints[i + 1].Y);
          gp.StartFigure();
        } // end for
        g.DrawPath(linePen, gp);
        gp.Reset();
      }
    }

    private PointF[] Segment2Connection_GetSubPoints(
      PointF[] allLinePoints,
      IPlotRange range,
      IPlotArea layer,
      bool connectCircular,
      out int lastIdx)
    {
      var circularLinePoints = new PointF[range.Length + (connectCircular ? 1 : 0)];
      Array.Copy(allLinePoints, range.LowerBound, circularLinePoints, 0, range.Length); // Extract
      if (connectCircular)
      {
        circularLinePoints[range.Length] = circularLinePoints[0];
      }

      lastIdx = range.Length - 1 + (connectCircular ? 1 : 0);

      return circularLinePoints;
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
      if (range.Length < 2)
        return;

      PointF[] linePoints = Segment2Connection_GetSubPoints(allLinePointsShiftedAlready, range, layer, connectCircular, out var lastIdx);

      if (connectCircular)
      {
        gp.AddLines(linePoints);
        gp.CloseFigure();
      }
      else
      {
        int offs = range.LowerBound;
        for (int i = 0; i < linePoints.Length - 1; i += 2)
        {
          Logical3D r0 = layer.GetLogical3D(pdata, range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i));
          r0.RX += logicalShiftX;
          r0.RY += logicalShiftY;

          layer.CoordinateSystem.GetIsolineFromPlaneToPoint(gp, fillDirection, r0);
          gp.AddLine(linePoints[i].X, linePoints[i].Y, linePoints[i + 1].X, linePoints[i + 1].Y);
          Logical3D r1 = layer.GetLogical3D(pdata, range.GetOriginalRowIndexFromPlotPointIndex(range.LowerBound + i + 1));
          r1.RX += logicalShiftX;
          r1.RY += logicalShiftY;

          layer.CoordinateSystem.GetIsolineFromPointToPlane(gp, r1, fillDirection);
          layer.CoordinateSystem.GetIsolineOnPlane(gp, fillDirection, r1, r0);
          gp.StartFigure();
        }

        gp.CloseFigure();
      }
    }
  }
}
