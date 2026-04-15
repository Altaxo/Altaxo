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
  /// Connects by drawing a horizontal line to the x coordinate of the next point, and then a vertical line. Instances of this class have to be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class StepHorizontalCenteredConnection : StepConnectionBase
  {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static StepHorizontalCenteredConnection Instance { get; private set; } = new StepHorizontalCenteredConnection();

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StepHorizontalCenteredConnection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Instance;
      }
    }

    #endregion Serialization

    /// <inheritdoc />
    protected override PointF[] GetStepPolylinePoints(
    PointF[] pdata,
    IPlotRange range,
    IPlotArea layer,
    bool connectCircular,
    out int numberOfPointsPerOriginalPoint,
    out int lastIndex)
    {
      numberOfPointsPerOriginalPoint = 3;

      var subLinePoints = new PointF[numberOfPointsPerOriginalPoint * (range.Length - 1 + (connectCircular ? 1 : 0)) + 1];
      int end = range.UpperBound - 1;
      int i, j;
      for (i = 0, j = range.LowerBound; j < end; i += numberOfPointsPerOriginalPoint, j++)
      {
        subLinePoints[i] = pdata[j];
        subLinePoints[i + 1] = new PointF((pdata[j].X + pdata[j + 1].X) / 2, pdata[j].Y);
        subLinePoints[i + 2] = new PointF((pdata[j].X + pdata[j + 1].X) / 2, pdata[j + 1].Y);
      }
      subLinePoints[i] = pdata[j];
      lastIndex = i;

      if (connectCircular)
      {
        subLinePoints[i + 1] = new PointF((pdata[j].X + pdata[range.LowerBound].X) / 2, pdata[j].Y);
        subLinePoints[i + 2] = new PointF((pdata[j].X + pdata[range.LowerBound].X) / 2, pdata[range.LowerBound].Y);
        subLinePoints[i + 3] = pdata[range.LowerBound];
        lastIndex = i + 3;
      }
      return subLinePoints;
    }
  }
}
