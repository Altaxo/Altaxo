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
using Altaxo.Calc;

namespace Altaxo.Science.Spectroscopy.Normalization
{
  /// <summary>
  /// Executes min-max normalization : y' = (y-min)/(max-min), in which min and max are the minimal and maximal values of a user provided range of the array.
  /// The range is given by the minimal and maximal x-values.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record NormalizationMinMaxInRange : INormalization
  {
    /// <summary>
    /// Gets the minimal value x-value (inclusive) of the range of the spectrum, which is used to determine the minimal and maximal y-values used for normalization.
    /// </summary>
    public double MinimalValue { get; init; } = double.NegativeInfinity;

    /// <summary>
    /// Gets the maximal value x-value (inclusive) of the range of the spectrum, which is used to determine the minimal and maximal y-values used for normalization.
    /// </summary>
    public double MaximalValue { get; init; } = double.PositiveInfinity;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationMinMaxInRange), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NormalizationMinMaxInRange)obj;
        info.AddValue("MinimalValue", s.MinimalValue);
        info.AddValue("MaximalValue", s.MaximalValue);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalValue = info.GetDouble("MinimalValue");
        var maximalValue = info.GetDouble("MaximalValue");

        return o is null ? new NormalizationMinMaxInRange
        {
          MinimalValue = minimalValue,
          MaximalValue = maximalValue,
        } :
          ((NormalizationMinMaxInRange)o) with
          {
            MinimalValue = minimalValue,
            MaximalValue = maximalValue,
          };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yy = new double[y.Length];

      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        for (int i = start; i < end; ++i)
        {
          if (RMath.IsInIntervalCC(x[i], MinimalValue, MaximalValue))
          {
            min = Math.Min(min, y[i]);
            max = Math.Max(max, y[i]);
          }
        }
      }

      var delta = max - min;
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        for (int i = start; i < end; ++i)
        {
          yy[i] = (y[i] - min) / delta;
        }
      }
      return (x, yy, regions);
    }
  }
}
