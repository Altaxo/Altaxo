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

using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  public interface IFitFunctionPeak : IFitFunction
  {
    /// <summary>
    /// Creates a new fit function with the provided number of terms (peaks).
    /// </summary>
    /// <param name="numberOfTerms">The number of terms.</param>
    /// <returns>New fit function with the provided number of terms (peaks).</returns>
    IFitFunctionPeak WithNumberOfTerms(int numberOfTerms);

    /// <summary>
    /// Gets the number of terms.
    /// </summary>
    int NumberOfTerms { get; }

    /// <summary>
    /// Creates a new fit function with the provided order of the baseline polynomial. A value of -1 disables the baseline.
    /// </summary>
    /// <param name="baselinePolynomialOrder">The order of the baseline polynomial.</param>
    /// <returns>New fit function with the provided order of the baseline polynomial.</returns>
    IFitFunctionPeak WithOrderOfBaselinePolynomial(int baselinePolynomialOrder);

    /// <summary>
    /// Gets the order of the polynomial that is used for the baseline. A value of -1 disables the baseline.
    /// </summary>
    int OrderOfBaselinePolynomial { get; }

    /// <summary>
    /// Gets the initial parameters for one term (peak) by providing the height of the peak,
    /// the position of the peak, the width of the peak, and the relative height at which the width was measured.
    /// </summary>
    /// <param name="height">The height of the peak (height of the maximum).</param>
    /// <param name="position">The position of the peak.</param>
    /// <param name="width">The width of the peak, measured at the provided relative height.</param>
    /// <param name="relativeHeight">Relative height value (0,1), at which the width was measured.</param>
    /// <returns>The initial parameters for one term (peak).</returns>
    double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight);


    /// <summary>
    /// Gets the parameter names for one peak.
    /// </summary>
    string[] ParameterNamesForOnePeak { get; }

    /// <summary>
    /// Gets the position, the area under the peak, the height, and the Full Width Half Maximum (FWHM) from the parameters of a single peak.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The position, the area under the peak, the height, and the Full Width Half Maximum (FWHM).</returns>
    (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters);

    /// <summary>
    /// Gets the position, the area under the peak, the height, and the Full Width Half Maximum (FWHM) from the parameters of a single peak.
    /// If the covariance matrix is given, then also the standard deviations of position, area, height, FWHM are calculated (otherwise, zero values are returned for the standard deviations).
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cv">The covariance matrix. Can be null (in this case the returned variance values are zero).</param>
    /// <returns>The position, the area under the peak, the height, and the Full Width Half Maximum (FWHM), together with their standard devivations.</returns>
    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
        GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv);

    /// <summary>
    /// Gets the parameter boundaries in order to have positive peaks only.
    /// </summary>
    /// <param name="minimalPosition">The minimal position that should be set in the boundaries.</param>
    /// <param name="maximalPosition">The maximal position that should be set in the boundaries.</param>
    /// <param name="minimalFWHM">The minimal FWHM (Full-Width-Half-Maximum) that should be set in the boundary conditions.</param>
    /// <param name="maximalFWHM">The maximal FWHM (Full-Width-Half-Maximum) that should be set in the boundary conditions.</param>
    /// <returns>The lower and the upper bounds. Both arrays (if not null) have the same length as the number of parameters.
    /// Only those elements are not null, which really bound a parameter.</returns>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null);
  }
}
