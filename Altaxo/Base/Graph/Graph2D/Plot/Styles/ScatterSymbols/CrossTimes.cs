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
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Represents an open times-shaped scatter symbol.
  /// </summary>
  public class CrossTimes : OpenSymbolBase
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="CrossTimes"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossTimes), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(o, o.GetType().BaseType!);

        SerializeSetV0((IScatterSymbol)o, info);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (CrossTimes?)o ?? new CrossTimes();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);

        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossTimes"/> class.
    /// </summary>
    public CrossTimes()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossTimes"/> class.
    /// </summary>
    /// <param name="fillColor">The fill color.</param>
    /// <param name="isFillColorInfluencedByPlotColor">If set to <c>true</c>, the fill color is influenced by the plot color.</param>
    public CrossTimes(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
      : base(fillColor, isFillColorInfluencedByPlotColor)
    {
    }

    /// <summary>
    /// Converts local coordinates into a rotated point used by the times shape.
    /// </summary>
    /// <param name="w">The transverse offset.</param>
    /// <param name="h">The longitudinal offset.</param>
    /// <returns>The corresponding point in clipper coordinates.</returns>
    private Point64 GetPoint(double w, double h)
    {
      const double Sqrt05 = 0.707106781186547524400844;
      return new Point64((int)(Sqrt05 * (w + h) * ClipperScalingDouble), (int)(Sqrt05 * (h - w) * ClipperScalingDouble));
    }

    /// <inheritdoc/>
    public override Paths64 GetCopyOfOuterPolygon(double relativeStructureWidth)
    {
      relativeStructureWidth = Altaxo.Calc.RMath.ClampToInterval(relativeStructureWidth, 0, 0.5);

      var h = Math.Sqrt(1 - relativeStructureWidth * relativeStructureWidth);
      var w = relativeStructureWidth;

      var list = new List<Point64>(12)
        {
        GetPoint(-w, -h),
        GetPoint(w, -h),
        GetPoint(w, -w),
        GetPoint(h, -w),
        GetPoint(h, w),
        GetPoint(w, w),
        GetPoint(w, h),
        GetPoint(-w, h),
        GetPoint(-w, w),
        GetPoint(-h, w),
        GetPoint(-h, -w),
        GetPoint(-w, -w),
      };

      return new Paths64 { new Path64(list) };
    }
  }
}
