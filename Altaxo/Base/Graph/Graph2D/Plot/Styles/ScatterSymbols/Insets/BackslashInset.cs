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
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Insets
{
  /// <summary>
  /// Represents a backslash-shaped inset for a scatter symbol.
  /// </summary>
  public class BackslashInset : InsetBase
  {
    private const double Sqrt05 = 0.707106781186547524400844;

    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="BackslashInset"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BackslashInset), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(o, o.GetType().BaseType!);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (BackslashInset?)o ?? new BackslashInset();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Converts local coordinates into a rotated point used by the backslash shape.
    /// </summary>
    /// <param name="w">The transverse offset.</param>
    /// <param name="h">The longitudinal offset.</param>
    /// <returns>The corresponding point in clipper coordinates.</returns>
    private Point64 GetPoint(double w, double h)
    {
      return new Point64((int)(Sqrt05 * (w + h) * ClipperScalingDouble), (int)(Sqrt05 * (h - w) * ClipperScalingDouble));
    }

    /// <inheritdoc/>
    public override Paths64 GetCopyOfClipperPolygon(double relativeWidth)
    {
      var w = relativeWidth;
      var h = 1;

      var list = new Path64
        {
        GetPoint(-h, -w),
        GetPoint(h, -w),
        GetPoint(h, w),
        GetPoint(-h, w)
      };

      return new Paths64 { list };
    }
  }
}
