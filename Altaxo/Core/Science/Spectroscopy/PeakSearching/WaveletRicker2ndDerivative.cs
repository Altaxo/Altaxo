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
  /// Normalized 2nd derivative of the Ricker wavelet function.
  /// </summary>
  /// <remarks>
  /// <para>It models the function:</para>
  /// <code>
  /// A * (3 - 6*(x/w)**2 + (x/w)**4) * exp(-0.5*(x/w)**2),
  /// where A = 4/(sqrt(105*a)*(pi**0.25)).
  /// </code>
  /// </remarks>
  public record WaveletRicker2ndDerivative : IWaveletForPeakSearching
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WaveletRicker2ndDerivative), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WaveletRicker2ndDerivative)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new WaveletRicker2ndDerivative();
      }
    }

    #endregion


    /// <inheritdoc/>
    public double WaveletFunction(double x, double width)
    {
      const double Prefactor = 0.29320938945473762851; // 4 / (Pi^0.25 * Sqrt(105))
      var xw = x / width;
      var sqrxw = xw * xw;
      return Prefactor * (3 - 6 * sqrxw + sqrxw * sqrxw) * Math.Exp(-0.5 * sqrxw) / Math.Sqrt(width);
    }

    /// <inheritdoc/>
    public (double GaussAmplitude, double GaussSigma) GetParametersForGaussianPeak(double cwtCoefficient, double width)
    {
      const double facA = 0.56477320169021190223; // Math.Pow(Math.Pi, 0.25) * Math.Sqrt(3.0/7.0) * 81.0 / 125.0;
      const double facW = 3; 

      return (cwtCoefficient / (Math.Sqrt(width) * facA), width/facW);
    }
  }
}
