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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  /// <summary>
  /// Provides a wrapper for the global derivative matrix (global = Matrix of the derivative of the parameters for multiple spectra).
  /// This wrapper is written to from the local fit function (local = Fit function for only one spectrum).
  /// </summary>
  /// <remarks>
  /// It is assumed that all spectra share the same peak parameters like position, width and other shape parameters,
  /// and that they don't share (i) the amplitude parameter (so each spectrum has its own peak amplitudes),
  /// and that they don't share (ii) the baseline parameter(s) (so each spectrum has its own baseline).
  /// The layout of the parameters of the global matrix is assumed as follows:
  /// <para>For each peak: amplitudeSpectrum0, amplitudeSpectrum1, ..., amplitudeSpectrumN, peak_position, peak_width, other peak shape parameters.</para>
  /// <para>For each spectrum: baselineParameterOrder0, baselineParameterOrder1, ..., baselineParameterOrderM</para>
  /// </remarks>
  public class DerivativeMatrixWrapper : IMatrix<double>
  {
    private int _rowOffset;
    private int _indexOfSpectrum;
    private int _numberOfSpectra;

    /// <summary>The total number of peak parameters (local) = numberOfParametersPerPeakLocal * numberOfPeaks</summary>
    private int _totalNumberOfPeakParametersLocal;

    // <summary>
    /// The number of parameters per peak (global).
    /// </summary>
    private int _numberOfParametersPerPeakLocal;

    /// <summary>The total number of peak parameters (global) = numberOfParametersPerPeakGlobal * numberOfPeaks</summary>
    private int _totalNumberOfPeakParametersGlobal;

    /// <summary>
    /// The number of parameters per peak (global).
    /// </summary>
    private int _numberOfParametersPerPeakGlobal;

    private int _numberOfBaselineParametersLocal;


    private IMatrix<double> _matrix;


    /// <summary>
    /// Wraps the global derivative matrix so that it can be used by the (local) fit function.
    /// </summary>
    /// <param name="rowOffset">The row offset.</param>
    /// <param name="indexOfSpectrum">The index of the spectrum.</param>
    /// <param name="numberOfSpectra">The number of spectra of the fit function.</param>
    /// <param name="numberOfPeaks">The number of peaks of the fit function.</param>
    /// <param name="totalNumberOfPeakParametersLocal">The total number of peak parameters local = (number of peaks) x (number of parameters per peak).</param>
    /// <param name="totalNumberOfPeakParametersGlobal">The total number of peak parameters global = (number of peaks) x (number of parameters per peak including all amplitudes).</param>
    /// <param name="numberOfBaselineParameters">The number of baseline parameters of the fit function.</param>
    /// <param name="matrix">The global derivative matrix.</param>
    public DerivativeMatrixWrapper(int rowOffset, int indexOfSpectrum, int numberOfSpectra, int numberOfPeaks, int totalNumberOfPeakParametersLocal, int totalNumberOfPeakParametersGlobal, int numberOfBaselineParameters, IMatrix<double> matrix)
    {
      _rowOffset = rowOffset;
      _indexOfSpectrum = indexOfSpectrum;
      _numberOfSpectra = numberOfSpectra;
      _totalNumberOfPeakParametersLocal = totalNumberOfPeakParametersLocal;
      _numberOfParametersPerPeakLocal = numberOfPeaks == 0 ? 0 : totalNumberOfPeakParametersLocal / numberOfPeaks;
      _totalNumberOfPeakParametersGlobal = totalNumberOfPeakParametersGlobal;
      _numberOfParametersPerPeakGlobal = numberOfPeaks == 0 ? 0 : totalNumberOfPeakParametersGlobal / numberOfPeaks;
      _numberOfBaselineParametersLocal = numberOfBaselineParameters;
      _matrix = matrix;
    }

    /// <inheritdoc/>
    public double this[int row, int col]
    {
      get => throw new NotImplementedException();
      set
      {
        int wrappedCol;
        if (col < _totalNumberOfPeakParametersLocal) // then it is a peak parameter
        {
          int indexOfPeak = col / _numberOfParametersPerPeakLocal;
          int remainder = col % _numberOfParametersPerPeakLocal;
          wrappedCol = indexOfPeak * _numberOfParametersPerPeakGlobal;
          if (0 == remainder) // if it is the local peak amplitude
          {
            wrappedCol += _indexOfSpectrum; // then the peak amplitude in the global array is located at the base plus the spectral index
          }
          else
          {
            wrappedCol += _numberOfSpectra + remainder - 1;
          }
        }
        else // it is a baseline parameter
        {
          // this are the baseline parameters
          wrappedCol = _totalNumberOfPeakParametersGlobal + _indexOfSpectrum * _numberOfBaselineParametersLocal + (col - _totalNumberOfPeakParametersLocal);
        }

        _matrix[row + _rowOffset, wrappedCol] = value;
      }
    }

    /// <inheritdoc/>
    double IROMatrix<double>.this[int row, int col] => this[row, col];

    /// <summary>
    /// Not implemented.
    /// </summary>
    public int RowCount => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public int ColumnCount => throw new NotImplementedException();
  }

}
