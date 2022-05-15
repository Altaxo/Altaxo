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

using System.Linq;

namespace Altaxo.Science.Spectroscopy.Normalization
{
  /// <summary>
  /// Executes SVN (standard  normal variate) normalization : y' = (y-mean)/(std), in which meam and std are the mean value and the standard deviation of the array values.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public class NormalizationStandardNormalVariate : INormalization
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
    public double[] Execute(double[] x)
    {
      var q = new Altaxo.Calc.Regression.QuickStatistics();
      q.AddRange(x);
      var min = q.Mean;
      var delta = q.StandardDeviation;
      return x.Select(yy => (yy - min) / delta).ToArray();
    }
  }
}
