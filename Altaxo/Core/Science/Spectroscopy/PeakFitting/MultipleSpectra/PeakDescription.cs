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

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Gets the parent fit result that provides access to the shared global parameter and covariance arrays.
    /// </summary>
    public MultipleSpectraPeakFittingResult Parent { get; init; }

    /// <summary>
    /// Gets the original index of the peak. This is the index as the peak appears in the global parameter array.
    /// </summary>
    public int OriginalPeakIndex { get; init; }

    /// <summary>
    /// Gets notes, for instance why a fit was not possible.
    /// </summary>
    public string Notes { get; init; } = string.Empty;

    /// <summary>
    /// Gets the peak parameters of this peak for a given spectrum.
    /// </summary>
    /// <param name="idxSpectrum">The index of the spectrum.</param>
    /// <returns>The peak parameters for the specified spectrum.</returns>
    public double[] GetPeakParametersOfSpectrum(int idxSpectrum)
    {
      var result = new double[Parent.NumberOfParametersPerPeak];
      int offset = Parent.NumberOfParametersPerPeakGlobal * OriginalPeakIndex;
      result[0] = Parent.ParametersGlobal[offset + idxSpectrum];
      offset += Parent.NumberOfSpectra;
      Array.Copy(Parent.ParametersGlobal, offset, result, 1, result.Length - 1);
      return result;
    }

    /// <summary>
    /// Gets the parameter indices of this peak for the spectrum specified by <paramref name="idxSpectrum"/>.
    /// </summary>
    /// <param name="idxSpectrum">The index of the spectrum.</param>
    /// <returns>The indices in the global array that point to the local peak parameters for the specified spectrum.</returns>
    public IEnumerable<int> GetParameterIndicesOfSpectrum(int idxSpectrum)
    {
      int offset = Parent.NumberOfParametersPerPeakGlobal * OriginalPeakIndex;
      yield return offset + idxSpectrum;
      offset += Parent.NumberOfSpectra - 1;
      for (int i = 1; i < Parent.NumberOfParametersPerPeak; ++i)
        yield return offset + i;
    }

    /// <summary>
    /// Gets the covariance matrix of the fitted peak parameters for a given spectrum.
    /// </summary>
    /// <param name="idxSpectrum">The index of the spectrum.</param>
    /// <returns>The covariance matrix for the specified spectrum, or <see langword="null"/> if no covariance data is available.</returns>
    public IROMatrix<double>? GetPeakParameterCovariancesForSpectrum(int idxSpectrum)
    {
      if (Parent.CovariancesGlobal is { } src)
      {
        var indices = GetParameterIndicesOfSpectrum(idxSpectrum).ToArray();

        var result = Matrix<double>.Build.Dense(indices.Length, indices.Length);
        for (int r = 0; r < indices.Length; ++r)
          for (int c = 0; c < indices.Length; ++c)
            result[r, c] = src[indices[r], indices[c]];
        return result;
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Gets the fit function that was used to fit the peak.
    /// </summary>
    public IFitFunctionPeak? FitFunction => Parent.FitFunction;

    /// <summary>
    /// Gets the chi-square value of the fit.
    /// </summary>
    public double SumChiSquare { get; init; }

    /// <summary>
    /// Gets the chi-square sum divided by (number of data points - number of degrees of freedom + 1).
    /// </summary>
    public double SigmaSquare { get; init; }

    /// <summary>
    /// Gets the peak group number (when fitting in groups).
    /// </summary>
    /// <value>
    /// The peak group number. The value is zero if groups are not used for the fit.
    /// </value>
    public int PeakGroupNumber { get; init; }


    /// <summary>
    /// Gets the position of this peak.
    /// </summary>
    public double Position => GetPositionAreaHeightFWHMOfSpectrum(0).Position;

    /// <summary>
    /// Gets the position, area, height, and full width at half maximum (FWHM) of the fitted peak for a given spectrum.
    /// </summary>
    /// <param name="idxSpectrum">The index of the spectrum.</param>
    /// <returns>The position, area, height and FWHM of the fitted peak.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// <see cref="FitFunction"/> is <see langword="null"/>.
    /// </exception>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMOfSpectrum(int idxSpectrum)
    {
      if (Parent.FitFunction is { } ff)
      {
        var para = GetPeakParametersOfSpectrum(idxSpectrum);
        return ff.GetPositionAreaHeightFWHMFromSinglePeakParameters(para);
      }
      else
      {
        throw new System.InvalidOperationException($"FitFunction or PeakParameter is null (in instance of {this.GetType()}).");
      }
    }
  }
}
