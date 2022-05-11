#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

namespace Altaxo.Science.Spectroscopy.Cropping
{
  public record CroppingByIndices : ICropping
  {
    public int MinimalIndex { get; init; }

    public int MaximalIndex { get; init; }


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CroppingByIndices), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CroppingByIndices)obj;
        info.AddValue("MinimalIndex", s.MinimalIndex);
        info.AddValue("MaximalIndex", s.MaximalIndex);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalIndex = info.GetInt32("MinimalIndex");
        var maximalIndex = info.GetInt32("MaximalIndex");

        return o is null ? new CroppingByIndices
        {
          MinimalIndex = minimalIndex,
          MaximalIndex = maximalIndex,
        } :
          ((CroppingByIndices)o) with
          {
            MinimalIndex = minimalIndex,
            MaximalIndex = maximalIndex,
          };
      }
    }
    #endregion


    public (double[] x, double[] y) Execute(double[] x, double[] y)
    {
      var min = Math.Max(0, MinimalIndex >= 0 ? MinimalIndex : x.Length + MinimalIndex);
      var max = Math.Min(x.Length - 1, MaximalIndex >= 0 ? MaximalIndex : x.Length + MaximalIndex);

      if (min > max)
      {
        (max, min) = (min, max);
      }

      var xs = new double[max - min + 1];
      var ys = new double[max - min + 1];

      Array.Copy(x, min, xs, 0, xs.Length);
      Array.Copy(y, min, ys, 0, ys.Length);

      return (xs, ys);
    }
  }
}
