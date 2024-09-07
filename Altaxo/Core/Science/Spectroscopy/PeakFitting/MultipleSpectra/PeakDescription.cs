#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{

  /// <summary>
  /// Description of a single peak.
  /// </summary>
  public record PeakDescription
  {
    /// <summary>
    /// Gets notes, for instance, why a fit was not possible.
    /// </summary>
    public string Notes { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amplitudes of this peak, for all spectra. Depending on the fit function, the amplitudes could be height or area values.
    /// </summary>
    public double[]? PeakAmplitudes { get; init; }


    /// <summary>
    /// Gets the fit parameter values of the peak with exception of the amplitudes. Thus, the first element is the peak position, the second element
    /// is the width parameter.
    /// </summary>
    public double[]? PeakParameter { get; init; }

    /// <summary>
    /// Gets the fit covariance matrix of the parameters of the peak.
    /// </summary>
    public IROMatrix<double>? PeakParameterCovariances { get; init; }

    /// <summary>
    /// Gets the fit function that was used to fit the peak.
    /// </summary>
    public IFitFunctionPeak? FitFunction { get; init; }

    /// <summary>
    /// Gets the complete fit function parameter values (may include parameters of all peaks that were fitted).
    /// </summary>
    public double[]? FitFunctionParameter { get; init; }

    /// <summary>
    /// Gets the chi square value of the fit.
    /// </summary>
    public double SumChiSquare { get; init; }

    /// <summary>
    /// Gets the sum of chi square, divided by (number of data points - number of degrees of freedom + 1)
    /// </summary>
    /// <value>
    /// The delta.
    /// </value>
    public double SigmaSquare { get; init; }

    /// <summary>
    /// Gets the peak group number (when fitting in groups).
    /// </summary>
    /// <value>
    /// The peak group number. The value is zero if groups are not used in the fit.
    /// </value>
    public int PeakGroupNumber { get; init; }

    /// <summary>
    /// Gets the position, the area, the height and the Full Width Half Maximum of the fitted peak.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">FitFunction or PeakParameter is null (in instance of {this.GetType()}).</exception>
    public (double Position, double Area, double Height, double FWHM) PositionAreaHeightFWHM
    {
      get
      {
        if (FitFunction is { } ff && PeakParameter is { } para)
          return ff.GetPositionAreaHeightFWHMFromSinglePeakParameters(para);
        else
          throw new System.InvalidOperationException($"FitFunction or PeakParameter is null (in instance of {this.GetType()}).");
      }
    }
  }
}
