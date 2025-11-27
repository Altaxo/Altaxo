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
using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  public class Square : ClosedSymbolBase
  {
    private const double Sqrt1By2 = 0.70710678118654752440084436210485;

    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Square), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType!);

        SerializeSetV0((IScatterSymbol)obj, info);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Square?)o ?? new Square();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);

        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    public Square()
    {
    }

    public Square(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
      : base(fillColor, isFillColorInfluencedByPlotColor)
    {
    }

    public IEnumerable<PointD2D> GetPolygon()
    {
      yield return new PointD2D(-Sqrt1By2, -Sqrt1By2);
      yield return new PointD2D(Sqrt1By2, -Sqrt1By2);
      yield return new PointD2D(Sqrt1By2, Sqrt1By2);
      yield return new PointD2D(-Sqrt1By2, Sqrt1By2);
    }

    public override Paths64 GetCopyOfOuterPolygon()
    {
      int w = (int)(ClipperScalingInt * Sqrt1By2);

      return [
      new Path64(4)
      {
      new Point64(-w, -w),
      new Point64(w, -w),
      new Point64(w, w),
      new Point64(-w, w)
      }];
    }
  }
}
