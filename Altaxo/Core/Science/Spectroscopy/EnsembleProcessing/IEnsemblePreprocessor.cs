#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Preprocesses an ensemble of spectra (multiple spectra processed together), optionally producing auxiliary data
  /// required to apply the same preprocessing during prediction.
  /// </summary>
  public interface IEnsemblePreprocessor : ISingleSpectrumPreprocessor
  {
    /// <summary>
    /// Executes the processor for an ensemble of spectra.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The spectra. Each row of the matrix represents a spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>X-values, y-values, and regions of the processed spectra, plus optional auxiliary data.</returns>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions);

    /// <summary>
    /// Processes the spectra for prediction.
    /// For prediction it is necessary to use the auxiliary data previously evaluated by
    /// <see cref="Execute(double[], Matrix{double}, int[]?)"/>, in order to apply the same treatment to the spectra.
    /// </summary>
    /// <param name="x">The x-values of the spectra.</param>
    /// <param name="spectraMatrix">The matrix of spectra. Each row of the matrix represents a spectrum.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <param name="auxillaryData">The auxiliary data previously evaluated by the ensemble preprocessing.</param>
    (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData);

    /// <summary>
    /// Executes the single spectrum processor. By definition, in an ensemble preprocessor this does nothing.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The y-values of the spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>X-values, y-values, and regions of the processed spectrum.</returns>
    public new (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      // No processing for single spectrum
      return (x, y, regions);
    }
  }



}
