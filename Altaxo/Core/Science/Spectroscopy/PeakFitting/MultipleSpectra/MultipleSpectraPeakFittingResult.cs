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

using System.Collections.Generic;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  public class MultipleSpectraPeakFittingResult
  {
    public List<PeakDescription> PeakDescriptions { get; init; }

    public int NumberOfSpectra { get; init; }

    public int NumberOfPeaks => PeakDescriptions.Count;

    public IFitFunctionPeak FitFunction { get; init; }

    public int NumberOfParametersPerPeak { get; init; }

    public int NumberOfBaselineParameters => FitFunction.OrderOfBaselinePolynomial + 1;

    /// <summary>
    /// Gets the full parameter set, including all parameters for all peaks and spectra, and the baseline parameter.
    /// </summary>
    public double[] ParametersGlobal { get; init; }

    /// <summary>
    /// Gets the fit covariance matrix of the parameters of the peak.
    /// </summary>
    public IROMatrix<double>? CovariancesGlobal { get; init; }

    public double[] XGlobal { get; init; }

    public double[] YGlobal { get; init; }

    public IReadOnlyList<int> StartIndicesOfSpectra { get; init; }

    public int NumberOfParametersPerPeakGlobal => NumberOfParametersPerPeak - 1 + NumberOfSpectra;

    /// <summary>
    /// Gets the full parameter for one spectrum. This includes all peak parameters, and the baseline parameters.
    /// </summary>
    /// <param name="idxSpectrum">The index spectrum.</param>
    /// <returns></returns>
    public double[] GetFullParameterSetForSpectrum(int idxSpectrum)
    {
      var result = new double[NumberOfPeaks * NumberOfParametersPerPeak + FitFunction.OrderOfBaselinePolynomial + 1];
      int j = 0;
      for (int idxPeak = 0; idxPeak < NumberOfPeaks; idxPeak++)
      {
        var peak = PeakDescriptions[idxPeak];

        foreach (var idx in peak.GetParameterIndicesOfSpectrum(idxSpectrum))
        {
          result[j++] = ParametersGlobal[idx];
        }
      }

      for (int idx = NumberOfPeaks * NumberOfParametersPerPeakGlobal + NumberOfBaselineParameters * idxSpectrum, k = NumberOfBaselineParameters; k > 0; ++idx, --k)
      {
        result[j++] = ParametersGlobal[idx];
      }

      return result;
    }

    /// <summary>
    /// Gets the parameter for one peak of one spectrum, including the baseline parameters.
    /// </summary>
    /// <param name="idxSpectrum">The index of the spectrum.</param>
    /// <param name="idxPeak">The index of the peak.</param>
    /// <returns></returns>
    public double[] GetParametersForOnePeakInclusiveBaselineForSpectrum(int idxSpectrum, int idxPeak)
    {
      var result = new double[NumberOfParametersPerPeak + NumberOfBaselineParameters];
      int j = 0;
      {
        var peak = PeakDescriptions[idxPeak];

        foreach (var idx in peak.GetParameterIndicesOfSpectrum(idxSpectrum))
        {
          result[j++] = ParametersGlobal[idx];
        }
      }

      for (int idx = NumberOfPeaks * NumberOfParametersPerPeakGlobal + NumberOfBaselineParameters * idxSpectrum, k = NumberOfBaselineParameters; k > 0; ++idx, --k)
      {
        result[j++] = ParametersGlobal[idx];
      }

      return result;
    }

    /// <summary>
    /// Gets the parameter for one peak of one spectrum, including the baseline parameters.
    /// </summary>
    /// <param name="idxSpectrum">The index of the spectrum.</param>
    /// <returns></returns>
    public double[] GetBaselineParametersForSpectrum(int idxSpectrum)
    {
      var result = new double[NumberOfBaselineParameters];
      int j = 0;

      for (int idx = NumberOfPeaks * NumberOfParametersPerPeakGlobal + NumberOfBaselineParameters * idxSpectrum, k = NumberOfBaselineParameters; k > 0; ++idx, --k)
      {
        result[j++] = ParametersGlobal[idx];
      }

      return result;
    }



  }
}
