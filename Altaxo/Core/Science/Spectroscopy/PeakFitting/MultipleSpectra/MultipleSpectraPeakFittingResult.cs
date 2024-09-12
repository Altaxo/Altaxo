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

    public IFitFunctionPeak FitFunction { get; init; }

    public int NumberOfParametersPerPeak { get; init; }

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



  }
}
