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
  /// Executes area normalization: y' = (y - min) / mean, in which min and mean are the minimum and the mean values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record NormalizationArea : INormalization
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
    public bool BasedOnMinimumYValue { get; init; } = true;


    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="NormalizationArea"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Normalization.NormalizationArea", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationArea();
      }
    }

    /// <summary>
    /// XML serialization surrogate for <see cref="NormalizationArea"/>.
    /// </summary>
    /// <remarks>
    /// V1: 2026-02-23 Added MinimumXValue, MaximumXValue and BaseOnMinimumValue properties.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationArea), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NormalizationArea)obj;
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

        return ((o as NormalizationArea) ?? new NormalizationArea()) with
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
      var q = new Altaxo.Calc.Regression.QuickStatistics();
      var yy = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        double ybase = 0;
        if (BasedOnMinimumYValue)
        {
          ybase = double.PositiveInfinity;
          for (int i = start; i < end; ++i)
          {
            if (RMath.IsInIntervalCC(x[i], MinimumXValue, MaximumXValue))
              ybase = Math.Min(ybase, y[i]);
          }
        }

        double area = 0;
        (double x, double y)? previous = null;

        for (int i = start + 1; i < end; ++i)
        {
          if (previous is null && RMath.IsInIntervalCC(MinimumXValue, x[i], x[i - 1]))
          {
            previous = (MinimumXValue, RMath.InterpolateLinear((MinimumXValue - x[i - 1]) / (x[i] - x[i - 1]), y[i - 1], y[i]));
          }
          else if (previous is null && RMath.IsInIntervalCC(x[i - 1], MinimumXValue, MaximumXValue))
          {
            previous = (x[i - 1], y[i - 1]);
          }

          var xi = x[i];
          var yi = y[i];

          if (previous is not null && RMath.IsInIntervalCC(MaximumXValue, x[i], x[i - 1]))
          {
            xi = MaximumXValue;
            yi = RMath.InterpolateLinear((MaximumXValue - x[i]) / (x[i] - x[i - 1]), y[i], y[i - 1]);
          }

          if (previous is not null && RMath.IsInIntervalCC(xi, MinimumXValue, MaximumXValue))
          {
            area += 0.5 * ((yi - ybase) + (previous.Value.y - ybase)) * (xi - previous.Value.x);
          }
        }

        for (int i = start; i < end; ++i)
        {
          yy[i] = (y[i] - ybase) / area;
        }
      }
      return (x, yy, regions);

    }
  }
}

