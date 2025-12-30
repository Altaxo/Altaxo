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


namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Interface to a wavelet that can be used for peak searching.
  /// Implementations provide the wavelet function and a method to convert the CWT coefficient and ridge width at the ridge maximum
  /// to amplitude and sigma of a Gaussian peak.
  /// </summary>
  public interface IWaveletForPeakSearching
  {
    /// <summary>
    /// Calculates the wavelet function value.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="width">The width of the wavelet.</param>
    /// <returns>The value of the wavelet function at <paramref name="x"/>.</returns>
    double WaveletFunction(double x, double width);

    /// <summary>
    /// Gets the parameters of a Gaussian peak that best fits.
    /// </summary>
    /// <param name="cwtCoefficient">The maximal CWT coefficient at the ridge line.</param>
    /// <param name="width">The width that corresponds to the point on the ridge line at which the CWT coefficient is maximal.</param>
    /// <returns>The amplitude (not the area) and the sigma parameter of a Gaussian peak that best fits.</returns>
    (double GaussAmplitude, double GaussSigma) GetParametersForGaussianPeak(double cwtCoefficient, double width);
  }
}
