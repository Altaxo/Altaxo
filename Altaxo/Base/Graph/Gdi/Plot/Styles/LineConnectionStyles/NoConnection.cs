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
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles
{
  /// <summary>
  /// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class NoConnection : LineConnectionStyleBase
  {
    public static NoConnection Instance { get; private set; } = new NoConnection();

    #region Serialization

    /// <summary>
    /// 2016-05-09 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoConnection), 0)]
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
    /// <param name="pen">The pen to draw the line.</param>
    /// <param name="symbolGap">The size of the symbol gap. Argument is the original index of the data. The return value is the absolute symbol gap at this index.
    /// This function is null if no symbol gap is required.</param>
    /// <param name="skipFrequency">Skip frequency. Normally 1, thus all gaps are taken into account. If 2, only every 2nd gap is taken into account, and so on.</param>
    public override void PaintOneRange(
      Graphics g,
      PointF[] allLinePoints,
      IPlotRange range,
      IPlotArea layer,
      PenCacheGdi.GdiPen pen,
      Func<int, double> symbolGap,
      int skipFrequency,
      bool connectCircular,
      LinePlotStyle linePlotStyle)
    {
    }

    /// <inheritdoc/>
    public override void FillOneRange(
    GraphicsPath gp,
      Processed2DPlotData pdata,
      IPlotRange rangeRaw,
      IPlotArea layer,
      CSPlaneID fillDirection,
      bool ignoreMissingDataPoints,
      bool connectCircular,
      PointF[] allLinePointsShiftedAlready,
      double logicalShiftX,
      double logicalShiftY
    )
    {
    }
  }
}
