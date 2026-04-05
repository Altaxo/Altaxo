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
  /// Represents an open plus-shaped scatter symbol.
  /// </summary>
  public class CrossPlus : OpenSymbolBase
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="CrossPlus"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossPlus), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType!);

        SerializeSetV0((IScatterSymbol)obj, info);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (CrossPlus?)o ?? new CrossPlus();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);

        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossPlus"/> class.
    /// </summary>
    public CrossPlus()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossPlus"/> class.
    /// </summary>
    /// <param name="fillColor">The fill color.</param>
    /// <param name="isFillColorInfluencedByPlotColor">If set to <c>true</c>, the fill color is influenced by the plot color.</param>
    public CrossPlus(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
      : base(fillColor, isFillColorInfluencedByPlotColor)
    {
    }

    /// <inheritdoc/>
    public override Paths64 GetCopyOfOuterPolygon(double relativeStructureWidth)
    {
      relativeStructureWidth = Altaxo.Calc.RMath.ClampToInterval(relativeStructureWidth, 0, 0.5);

      var h = (int)Math.Round(0.25 + Math.Sqrt(1 - relativeStructureWidth * relativeStructureWidth) * ClipperScalingInt);
      var w = (int)Math.Round(0.25 + relativeStructureWidth * ClipperScalingInt);

      var list = new List<Point64>(12)
        {
        new Point64(-w, -h),
        new Point64(w, -h),
        new Point64(w, -w),
        new Point64(h, -w),
        new Point64(h, w),
        new Point64(w, w),
        new Point64(w, h),
        new Point64(-w, h),
        new Point64(-w, w),
        new Point64(-h, w),
        new Point64(-h, -w),
        new Point64(-w, -w),
      };

      return new Paths64 { new Path64(list) };
    }
  }
}
