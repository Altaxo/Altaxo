#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
  /// Executes max normalization : y' = y/max, in which max is the maximum of the values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record NormalizationMax : INormalization
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
    /// 2023-11-20 V0
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Normalization.NormalizationMax", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationMax();
      }
    }

    /// <summary>
    /// XML serialization surrogate for <see cref="NormalizationMax"/>.
    /// </summary>
    /// <remarks>
    /// V1: 2026-02-23 Added MinimumXValue, MaximumXValue and BasedOnMinimumYValue properties.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationMax), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NormalizationMax)obj;
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

        return ((o as NormalizationMax) ?? new NormalizationMax()) with
        {
          MinimumXValue = minimumXValue,
          MaximumXValue = maximumXValue,
          BasedOnMinimumYValue = basedOnMinimumYValue
        };
      }
    }

    /// <summary>
    /// XML serialization surrogate for obsolete <c>NormalizationMinMax</c>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Normalization.NormalizationMinMax", 0)]
    public class SerializationSurrogateMinMax0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationMax() { BasedOnMinimumYValue = true };
      }
    }

    /// <summary>
    /// XML serialization surrogate for obsolete <c>NormalizationMinMaxInRange</c>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Normalization.NormalizationMinMaxInRange", 0)]
    public class SerializationSurrogateMinMaxInRange0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalValue = info.GetDouble("MinimalValue");
        var maximalValue = info.GetDouble("MaximalValue");

        return new NormalizationMax() { MinimumXValue = minimalValue, MaximumXValue = maximalValue, BasedOnMinimumYValue = true };
      }
    }

    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yy = new double[y.Length];

      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;
        for (int i = start; i < end; ++i)
        {
          if (RMath.IsInIntervalCC(x[i], MinimumXValue, MaximumXValue))
          {
            min = Math.Min(min, y[i]);
            max = Math.Max(max, y[i]);
          }
        }

        var ybase = BasedOnMinimumYValue ? min : 0.0;
        if (max != ybase)
        {
          for (int i = start; i < end; ++i)
          {
            yy[i] = (y[i] - ybase) / (max - ybase);
          }
        }
        else
        {
          for (int i = start; i < end; ++i)
          {
            yy[i] = y[i];
          }
        }
      }
      return (x, yy, regions);
    }
  }
}
