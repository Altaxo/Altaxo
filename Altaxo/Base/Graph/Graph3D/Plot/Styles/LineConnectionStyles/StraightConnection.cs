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
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Graph.Graph3D.Plot.Styles.LineConnectionStyles
{
  /// <summary>
  /// Represents a line-connection style that connects points with straight segments.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class StraightConnection : LineConnectionStyleBase
  {
    /// <summary>
    /// Gets the singleton instance of the <see cref="StraightConnection"/> style.
    /// </summary>
    public static StraightConnection Instance { get; private set; } = new StraightConnection();

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="StraightConnection"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StraightConnection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Instance;
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override void Paint(
      IGraphicsContext3D g,
      Processed3DPlotData pdata,
      PlotRange range,
      IPlotArea layer,
      PenX3D pen,
      Func<int, double>? symbolGap,
      int skipFrequency,
      bool connectCircular)
    {
      if (!(pdata?.PlotPointsInAbsoluteLayerCoordinates is { } linePoints))
        return;

      var linepts = new PointD3D[range.Length + (connectCircular ? 1 : 0)];
      Array.Copy(linePoints, range.LowerBound, linepts, 0, range.Length); // Extract
      if (connectCircular)
        linepts[linepts.Length - 1] = linepts[0];
      int lastIdx = range.Length - 1 + (connectCircular ? 1 : 0);
      var layerSize = layer.Size;

      if (symbolGap is not null)
      {
        if (skipFrequency <= 1) // skip all scatter symbol gaps -> thus skipOffset can be ignored
        {
          for (int i = 0; i < lastIdx; i++)
          {
            int originalIndex = range.OffsetToOriginal + i;
            var diff = linepts[i + 1] - linepts[i];
            double gapAtStart = symbolGap(originalIndex);
            double gapAtEnd = i != (range.Length - 1) ? symbolGap(originalIndex + 1) : symbolGap(range.OffsetToOriginal);
            var relAtStart = 0.5 * gapAtStart / diff.Length; // 0.5 because symbolGap is the full gap between two lines, thus between the symbol center and the beginning of the line it is only 1/2
            var relAtEnd = 0.5 * gapAtEnd / diff.Length; // 0.5 because symbolGap is the full gap between two lines, thus between the symbol center and the beginning of the line it is only 1/2
            if ((relAtStart + relAtEnd) < 1) // a line only appears if sum of the gaps  is smaller than 1
            {
              var start = linepts[i] + relAtStart * diff;
              var stop = linepts[i + 1] - relAtEnd * diff;

              g.DrawLine(pen, start, stop);
            }
          } // end for
        } // skipFrequency was 1
        else // skipFrequency is > 1
        {
          for (int i = 0; i < lastIdx; i += skipFrequency)
          {
            int originalRowIndex = range.OriginalFirstPoint + i;

            double gapAtStart = symbolGap(originalRowIndex);
            double gapAtEnd = i != range.Length ? symbolGap(originalRowIndex + skipFrequency) : symbolGap(range.OffsetToOriginal);

            IPolylineD3D? polyline = SharpPolylineD3D.FromPointsWithPossibleDublettes(linepts.Skip(i).Take(1 + skipFrequency));
            polyline = polyline.ShortenedBy(RADouble.NewAbs(gapAtStart / 2), RADouble.NewAbs(gapAtEnd / 2));

            if (polyline is not null)
              g.DrawLine(pen, polyline);
          } // end for.
        }
      }
      else // no line symbol gap required, so we can use DrawLines to draw the lines
      {
        if (linepts.Length > 1) // we don't want to have a drawing exception if number of points is only one
        {
          g.DrawLine(pen, SharpPolylineD3D.FromPointsWithPossibleDublettes(linepts));
        }
      }
    }
  }
}
