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
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Represents a star-shaped open scatter symbol.
  /// </summary>
  public class Star : OpenSymbolBase
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="Star"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Star), 0)]
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
        var s = (Star?)o ?? new Star();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);

        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class.
    /// </summary>
    public Star()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class.
    /// </summary>
    /// <param name="fillColor">The fill color.</param>
    /// <param name="isFillColorInfluencedByPlotColor">If set to <c>true</c>, the fill color is influenced by the plot color.</param>
    public Star(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
      : base(fillColor, isFillColorInfluencedByPlotColor)
    {
    }

    /// <inheritdoc/>
    public override Paths64 GetCopyOfOuterPolygon(double relativeStructureWidth)
    {
      var innerRadiusRel = relativeStructureWidth / (2 * Math.Sin(Math.PI / 8));

      var outerStartAngle = Math.Asin(relativeStructureWidth / 2);
      var innerStartAngle = Math.Asin(relativeStructureWidth / (2 * innerRadiusRel));

      var innerRadius = ClipperScalingDouble * innerRadiusRel;
      var outerRadius = ClipperScalingDouble;

      double phi;

      var list = new Path64(24);
      for (int i = 0; i < 8; ++i)
      {
        phi = -innerStartAngle + Math.PI * (i / 4.0);
        list.Add(new Point64((int)(innerRadius * Math.Cos(phi)), (int)(innerRadius * Math.Sin(phi))));
        phi = -outerStartAngle + Math.PI * (i / 4.0);
        list.Add(new Point64((int)(outerRadius * Math.Cos(phi)), (int)(outerRadius * Math.Sin(phi))));
        phi = outerStartAngle + Math.PI * (i / 4.0);
        list.Add(new Point64((int)(outerRadius * Math.Cos(phi)), (int)(outerRadius * Math.Sin(phi))));
      }

      return [list];
    }
  }
}
