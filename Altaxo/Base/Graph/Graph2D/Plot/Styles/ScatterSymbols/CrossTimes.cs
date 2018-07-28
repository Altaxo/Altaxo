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
using ClipperLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  public class CrossTimes : OpenSymbolBase
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossTimes), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);

        SerializeSetV0((IScatterSymbol)obj, info);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (CrossTimes)o ?? new CrossTimes();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

        return DeserializeSetV0(s, info, parent);
      }
    }

    #endregion Serialization

    public CrossTimes()
    {
    }

    public CrossTimes(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
      : base(fillColor, isFillColorInfluencedByPlotColor)
    {
    }

    private ClipperLib.IntPoint GetPoint(double w, double h)
    {
      const double Sqrt05 = 0.707106781186547524400844;
      return new ClipperLib.IntPoint((int)(Sqrt05 * (w + h) * ClipperScalingDouble), (int)(Sqrt05 * (h - w) * ClipperScalingDouble));
    }

    public override List<List<ClipperLib.IntPoint>> GetCopyOfOuterPolygon(double relativeStructureWidth)
    {
      relativeStructureWidth = Altaxo.Calc.RMath.ClampToInterval(relativeStructureWidth, 0, 0.5);

      var h = Math.Sqrt(1 - relativeStructureWidth * relativeStructureWidth);
      var w = relativeStructureWidth;

      var list = new List<ClipperLib.IntPoint>(12)
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

      return new List<List<ClipperLib.IntPoint>>(1) { list };
    }
  }
}
