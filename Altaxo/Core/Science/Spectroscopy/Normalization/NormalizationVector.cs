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
  /// Executes vector normalization : y' = (y)/(norm), in which norm is the L2 norm of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record NormalizationVector : INormalization
  {
    /// <summary>
    /// Gets the minimum x-value (inclusive) of the spectrum range used to determine the minimum and maximum y-values for normalization.
    /// </summary>
    public double MinimumXValue { get; init; } = double.NegativeInfinity;

    /// <summary>
    /// Gets the maximum x-value (inclusive) of the spectrum range used to determine the minimum and maximum y-values for normalization.
    /// </summary>
    public double MaximumXValue { get; init; } = double.PositiveInfinity;

    /// <summary>
    /// Gets a value indicating whether the normalization should be based on the minimum value of the spectrum (if <c>true</c>) or on zero (if <c>false</c>).
    /// </summary>
    public bool BasedOnMinimumYValue { get; init; } = false;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="NormalizationVector"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Normalization.NormalizationVector", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException();
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationVector() { MinimumXValue = double.NegativeInfinity, MaximumXValue = double.PositiveInfinity, BasedOnMinimumYValue = false };
      }
    }

    /// <summary>
    /// XML serialization surrogate for <see cref="NormalizationVector"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationVector), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NormalizationVector)o;
        info.AddValue("MinimumXValue", s.MinimumXValue);
        info.AddValue("MaximumXValue", s.MaximumXValue);
        info.AddValue("BasedOnMinimumYValue", s.BasedOnMinimumYValue);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimumXValue = info.GetDouble("MinimumXValue");
        var maximumXValue = info.GetDouble("MaximumXValue");
        var basedOnMinimumYValue = info.GetBoolean("BasedOnMinimumYValue");

        return ((o as NormalizationVector) ?? new NormalizationVector()) with
        {
          MinimumXValue = minimumXValue,
          MaximumXValue = maximumXValue,
          BasedOnMinimumYValue = basedOnMinimumYValue
        };
      }
    }


    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yy = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        double baseValue = 0;
        if (BasedOnMinimumYValue)
        {
          var min = double.PositiveInfinity;
          for (int i = start; i < end; ++i)
          {
            if (RMath.IsInIntervalCC(x[i], MinimumXValue, MaximumXValue))
            {
              min = Math.Min(min, y[i]);
            }
          }
          if (!double.IsInfinity(min))
          {
            baseValue = min;
          }
        }

        double sums = 0;
        int N = 0;
        for (int i = start; i < end; ++i)
        {
          if (RMath.IsInIntervalCC(x[i], MinimumXValue, MaximumXValue))
          {
            sums += RMath.Pow2(y[i] - baseValue);
            N++;
          }
        }

        var delta = N > 0 ? Math.Sqrt(sums / N) : 1;

        for (int i = start; i < end; ++i)
        {
          yy[i] = (y[i] - baseValue) / delta;
        }
      }
      return (x, yy, regions);
    }
  }
}
