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

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Insets
{
  public class LeftHalfInset : InsetBase
  {
    #region Serialization

    /// <summary>
    /// 2016-10-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LeftHalfInset), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LeftHalfInset?)o ?? new LeftHalfInset();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    public override List<List<ClipperLib.IntPoint>> GetCopyOfClipperPolygon(double structureScale)
    {
      var h = ClipperScalingInt;
      var w = (int)Math.Round(0.25 + 0.5 * ClipperScalingInt);

      return new List<List<ClipperLib.IntPoint>>(1)
      {
        new List<ClipperLib.IntPoint>(4)
        {
        new ClipperLib.IntPoint( -h, -h),
        new ClipperLib.IntPoint( 0, -h),
        new ClipperLib.IntPoint( 0, h),
        new ClipperLib.IntPoint( -h, h)
      }
      };
    }
  }
}
