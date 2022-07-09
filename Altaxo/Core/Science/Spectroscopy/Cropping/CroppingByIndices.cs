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
using System.Collections.Generic;

namespace Altaxo.Science.Spectroscopy.Cropping
{
  /// <summary>
  /// Does cropping of a spectrum by indices.
  /// </summary>
  public record CroppingByIndices : ICropping
  {
    /// <summary>
    /// Gets the starting index of the cropping region.
    /// </summary>
    public int MinimalIndex { get; init; }

    /// <summary>
    /// Gets the end index (inclusive) of the cropping region.
    /// </summary>
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


    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      int newArrayLength = 0;
      foreach (var (start, end) in GetCroppedRegions(regions, x.Length))
      {
        newArrayLength += (end-start);
      }

      var xs = new double[newArrayLength];
      var ys = new double[newArrayLength];
      var rs = new List<int>();

      int destinationStart = 0;
      foreach (var (start, end) in GetCroppedRegions(regions, x.Length))
      {
        Array.Copy(x, start, xs, destinationStart, end - start);
        Array.Copy(y, start, ys, destinationStart, end - start);

        if(destinationStart > 0)
        {
          rs.Add(destinationStart);
        }
      }

      return (xs, ys, rs.Count>0 ? rs.ToArray() : null);
    }

    /// <summary>
    /// Gets the cropped regions.
    /// </summary>
    /// <param name="regions">The regions. Each element is the start index of a new region</param>
    /// <param name="arrayLength">Total length of the array.</param>
    /// <returns>Enumeration of start and end (exclusive) index of the cropped regions.</returns>
    public IEnumerable<(int start, int end)> GetCroppedRegions(int[] regions, int arrayLength)
    {
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, arrayLength))
      {
        int regionLength = end - start;
        var min = Math.Max(0, MinimalIndex >= 0 ? MinimalIndex : regionLength + MinimalIndex);
        var max = Math.Min(regionLength - 1, MaximalIndex >= 0 ? MaximalIndex : regionLength + MaximalIndex);

        if (min > max)
        {
          (max, min) = (min, max);
        }

        if (max > min)
        {
          yield return (start + min, start + max + 1);
        }
      }
    }
  }
}
