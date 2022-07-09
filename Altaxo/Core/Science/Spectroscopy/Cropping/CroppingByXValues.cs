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

using System.Collections.Generic;

namespace Altaxo.Science.Spectroscopy.Cropping
{
  public record CroppingByXValues : ICropping
  {
    public double MinimalValue { get; init; } = double.NegativeInfinity;

    public double MaximalValue { get; init; } = double.PositiveInfinity;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CroppingByXValues), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CroppingByXValues)obj;
        info.AddValue("MinimalValue", s.MinimalValue);
        info.AddValue("MaximalValue", s.MaximalValue);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalValue = info.GetDouble("MinimalValue");
        var maximalValue = info.GetDouble("MaximalValue");

        return o is null ? new CroppingByXValues
        {
          MinimalValue = minimalValue,
          MaximalValue = maximalValue,
        } :
          ((CroppingByXValues)o) with
          {
            MinimalValue = minimalValue,
            MaximalValue = maximalValue,
          };
      }
    }
    #endregion


    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var min = MinimalValue;
      var max = MaximalValue;

      if (max < min)
      {
        (max, min) = (min, max);
      }

      var lx = new List<double>(x.Length);
      var ly = new List<double>(y.Length);
      var lr = new List<int>();

      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        for (int i = start; i < end; i++)
        {
          if (x[i] >= min && x[i] <= max)
          {
            lx.Add(x[i]);
            ly.Add(y[i]);
          }
        }

        if (lx.Count != 0)
        {
          if (lr.Count == 0 || lr[lr.Count - 1] != lx.Count) // Avoid adding the same number if the previous range was empty
          {
            lr.Add(lx.Count);
          }
        }
      }

      // remove the last element of the range, because that is the end element
      if(lr.Count >0) 
      {
        lr.RemoveAt(lr.Count - 1);
      }

      return (lx.ToArray(), ly.ToArray(), lr.Count > 0 ? lr.ToArray() : null);
    }
  }
}
