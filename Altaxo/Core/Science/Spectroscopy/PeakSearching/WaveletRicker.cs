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

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Ricker wavelet function, also known as the "Mexican hat wavelet".
  /// </summary>
  /// <remarks>
  /// <para>It models the function:</para>
  /// <code>
  /// A* (1 - (x/a)**2) * exp(-0.5*(x/a)**2),
  /// where A = 2/(sqrt(3*a)*(pi**0.25)).
  /// </code>
  /// </remarks>
  public record WaveletRicker : IWaveletForPeakSearching
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WaveletRicker), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WaveletRicker)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new WaveletRicker();
      }
    }

    #endregion


    /// <inheritdoc/>
    public double WaveletFunction(double x, double width)
    {
      const double Prefactor = 0.86732507058407751832; // 2 / (Pi^0.25 * Sqrt(3))
      var xw = x / width;
      var sqrxw = xw * xw;
      return Prefactor * (1 - sqrxw) * Math.Exp(-0.5 * sqrxw) / Math.Sqrt(width);
    }

    /// <inheritdoc/>
    public (double GaussAmplitude, double GaussSigma) GetParametersForGaussianPeak(double cwtCoefficient, double width)
    {
      const double facA = 0.73963075766688317378; // Math.Pow(Math.Pi, 0.25) * 5.0 / 9.0;
      const double facW = 2.2360679774997896964; // Math.Sqrt(5)

      return (cwtCoefficient / (Math.Sqrt(width) * facA), width/facW);
    }
  }
}
