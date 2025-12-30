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

namespace Altaxo.Science.Spectroscopy.Normalization
{
  /// <summary>
  /// Executes min-max normalization : y' = (y-min)/(max-min), in which min and max are the minimal and maximal values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record NormalizationMinMax : INormalization
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="NormalizationMinMax"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationMinMax), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationMinMax();
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
          min = Math.Min(min, y[i]);
          max = Math.Max(max, y[i]);
        }
        var delta = max - min;

        for (int i = start; i < end; ++i)
        {
          yy[i] = (y[i] - min) / delta;
        }
      }
      return (x, yy, regions);
    }
  }
}
