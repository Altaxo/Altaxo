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

namespace Altaxo.Science.Spectroscopy.Normalization
{
  /// <summary>
  /// Executes SNV (standard  normal variate) normalization : y' = (y-mean)/(std), in which mean and std are the mean value and the standard deviation of the array values.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record NormalizationStandardNormalVariate : INormalization
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationStandardNormalVariate), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationStandardNormalVariate();
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
        q.Clear();
        for (int i = start; i < end; ++i)
        {
          q.Add(x[i]);
        }
        var min = q.Mean;
        var delta = q.StandardDeviation;

        for (int i = start; i < end; ++i)
        {
          yy[i] = (y[i] - min) / delta;
        }
      }
      return (x, yy, regions);
    }
  }
}
