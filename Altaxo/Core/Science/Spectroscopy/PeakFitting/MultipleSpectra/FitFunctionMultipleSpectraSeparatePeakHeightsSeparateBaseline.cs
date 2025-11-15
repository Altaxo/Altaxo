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
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  /// <summary>
  /// A fit function that wrapps a peak fitting function intended for one spectrum so that is can be used to fit the peaks of multiple spectra simultaneously,
  /// sharing the peak parameters position, width, and other shape parameters among the spectra, but not sharing the peak amplitude parameters
  /// and the baseline parameters.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunctionWithDerivative" />
  public partial class FitFunctionMultipleSpectraSeparatePeakHeightsSeparateBaseline : IFitFunction, IFitFunctionWithDerivative
  {
    public IFitFunctionPeak _underlyingFitFunction;
    private int _numberOfSpectra;
    private int _numberOfParametersPerPeakLocal;
    private int _numberOfPeaks;

    private int NumberOfParametersPerPeakGlobal => (_numberOfParametersPerPeakLocal - 1 + _numberOfSpectra);

    /// <summary>
    /// Indices of the start of the spectra in the global spectra array
    /// The first element is always 0
    /// </summary>
    private IReadOnlyList<int> _startIndicesOfSpectra;

    /// <summary>
    /// The parameters for the underlying fit function
    /// </summary>
    private double[] _parametersLocal;

    /// <summary>
    /// The array that designates if the parameters are fixed or not.
    /// </summary>
    private bool[] _isFixedLocal;

    /// <summary>The offset to the start of the baseline parameters in the global parameter array.</summary>
    private int _offsetToBaselineParametersGlobal;

    /// <summary>The offset to the start of the baseline parameters in the local parameter array.</summary>
    private int _offsetToBaselineParametersLocal;

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFunctionMultipleSpectraSeparatePeakHeightsSeparateBaseline"/> class.
    /// </summary>
    /// <param name="underlyingFitFunction">The underlying peak fitting function.</param>
    /// <param name="startIndicesOfSpectra">The start indices of spectra. The first entry must always be zero, since this
    /// is the start of the spectral values of the first spectrum in the global array. The next entries are incremented by the number of spectral values.
    /// For example, if three spectra should be fitted with length of 100, 90 and 80 respectively, the list should contain [0, 100, 190]. The length of the last
    /// spectrum (80 in this example) is deduced from the last entry and the length of the global array.
    /// </param>
    public FitFunctionMultipleSpectraSeparatePeakHeightsSeparateBaseline(IFitFunctionPeak underlyingFitFunction, IReadOnlyList<int> startIndicesOfSpectra)
    {
      _underlyingFitFunction = underlyingFitFunction;
      _startIndicesOfSpectra = startIndicesOfSpectra;
      _numberOfPeaks = underlyingFitFunction.NumberOfTerms;
      _numberOfSpectra = _startIndicesOfSpectra.Count;

      _numberOfParametersPerPeakLocal = _underlyingFitFunction.NumberOfTerms == 0 ? 0 : (underlyingFitFunction.NumberOfParameters - (underlyingFitFunction.OrderOfBaselinePolynomial + 1)) / _underlyingFitFunction.NumberOfTerms;
      _parametersLocal = new double[_underlyingFitFunction.NumberOfParameters];
      _isFixedLocal = new bool[_underlyingFitFunction.NumberOfParameters];
      _offsetToBaselineParametersGlobal = (_numberOfParametersPerPeakLocal - 1 + _numberOfSpectra) * _numberOfPeaks; // offset to the baseline parameters
      _offsetToBaselineParametersLocal = _underlyingFitFunction.NumberOfTerms * _numberOfParametersPerPeakLocal;
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters =>
      _numberOfSpectra * (_underlyingFitFunction.OrderOfBaselinePolynomial + 1) + // Parameters for the baseline
      _numberOfPeaks * (_numberOfParametersPerPeakLocal - 1 + _numberOfSpectra); // the height parameters (one for each spectrum)

    /// <inheritdoc/>
    public event EventHandler? Changed;

    /// <summary>Not implemented.</summary>
    public double DefaultParameterValue(int i)
    {
      throw new NotImplementedException();
    }

    /// <summary>Not implemented.</summary>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      throw new NotImplementedException();
    }

    /// <summary>Not implemented.</summary>
    public string DependentVariableName(int i)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Sets all peak parameters except that for the first peak to fixed.
    /// </summary>
    /// <param name="value">If set to true, all parameters except for the first peak are set to fixed. If set to false, all parameters can vary.</param>
    /// <param name="numberOfFixedPositionPeaks">Number of peaks (at the end), for which the position is fixed. </param>
    public void SetAllPeakParametersExceptFirstPeakToFixed(bool value, int numberOfFixedPositionPeaks)
    {
      if (value == false)
      {
        Array.Clear(_isFixedLocal, 0, _isFixedLocal.Length);
        for (int idxPeak = _numberOfPeaks - numberOfFixedPositionPeaks; idxPeak < _numberOfPeaks; idxPeak++)
        {
          _isFixedLocal[idxPeak * _numberOfParametersPerPeakLocal + 1] = true; // Fix the position of the peak
        }
      }
      else
      {
        for (int i = 0; i < _numberOfParametersPerPeakLocal; ++i)
          _isFixedLocal[i] = false;
        for (int i = _numberOfParametersPerPeakLocal; i < _isFixedLocal.Length; ++i)
          _isFixedLocal[i] = true;
      }
    }

    /// <summary>Not implemented.</summary>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      var x = MatrixMath.ToROMatrixWithOneRow(independent);
      var y = VectorMath.ToVector(FV);
      Evaluate(x, parameters, y, null);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int indexOfSpectrum = 0; indexOfSpectrum < _numberOfSpectra; ++indexOfSpectrum)
      {
        TransferGlobalToLocalParameters(indexOfSpectrum, parameters);
        int lengthOfSpectrum = indexOfSpectrum + 1 < _numberOfSpectra ? _startIndicesOfSpectra[indexOfSpectrum + 1] - _startIndicesOfSpectra[indexOfSpectrum] : independent.RowCount - _startIndicesOfSpectra[indexOfSpectrum];
        _underlyingFitFunction.Evaluate(MatrixMath.ToROSubMatrix(independent, _startIndicesOfSpectra[indexOfSpectrum], 0, lengthOfSpectrum, 1), _parametersLocal, VectorMath.ToSubVector(FV, _startIndicesOfSpectra[indexOfSpectrum], lengthOfSpectrum), null);
      }
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      if (_underlyingFitFunction is not IFitFunctionWithDerivative ffd)
        throw new NotImplementedException($"The underlying fit function does not implement {nameof(IFitFunctionWithDerivative)}");

      DF.ZeroMatrix();

      for (int indexOfSpectrum = 0; indexOfSpectrum < _numberOfSpectra; ++indexOfSpectrum)
      {
        TransferGlobalToLocalParameters(indexOfSpectrum, parameters);
        int lengthOfSpectrum = indexOfSpectrum + 1 < _numberOfSpectra ? _startIndicesOfSpectra[indexOfSpectrum + 1] - _startIndicesOfSpectra[indexOfSpectrum] : independent.RowCount - _startIndicesOfSpectra[indexOfSpectrum];
        ffd.EvaluateDerivative(
          MatrixMath.ToROSubMatrix(independent, _startIndicesOfSpectra[indexOfSpectrum], 0, lengthOfSpectrum, 1),
          _parametersLocal,
          null, // TODO IsFixed
          new DerivativeMatrixWrapper(
            _startIndicesOfSpectra[indexOfSpectrum],
            indexOfSpectrum,
            _numberOfSpectra,
            _numberOfPeaks,
            _numberOfPeaks * _numberOfParametersPerPeakLocal,
            _numberOfPeaks * (_numberOfParametersPerPeakLocal - 1 + _numberOfSpectra),
            _underlyingFitFunction.OrderOfBaselinePolynomial + 1,
          DF),
          null
          );
      }
    }


    /// <summary>
    /// Transfers the global parameter set to local parameters that can be used by the underlying peak fit function.
    /// </summary>
    /// <param name="indexOfSpectrum">The index of spectrum.</param>
    /// <param name="parameters">The parameters.</param>
    /// <remarks>
    /// The layout of the global parameter array is as follows:
    ///
    /// <para>For each peak: 1. all the amplitudes of the spectra, followed by the other parameters of the peak</para>
    /// <para>3. block: numberOfSpectra x (baseline parameters)</para>
    /// </remarks>
    private void TransferGlobalToLocalParameters(int indexOfSpectrum, IReadOnlyList<double> parameters)
    {
      int globalParametersPerPeak = _numberOfSpectra + _numberOfParametersPerPeakLocal - 1;

      for (int it = 0; it < _underlyingFitFunction.NumberOfTerms; ++it) // for each peak term
      {
        // the first local parameter is always the amplitude
        _parametersLocal[_numberOfParametersPerPeakLocal * it] = parameters[it * globalParametersPerPeak + indexOfSpectrum]; // the height parameters of the spectrum #ic

        for (int i = 1; i < _numberOfParametersPerPeakLocal; ++i) // for each peak term parameter except the 1st one (which is the height or area)
        {
          _parametersLocal[it * _numberOfParametersPerPeakLocal + i] = parameters[it * globalParametersPerPeak + _numberOfSpectra + i - 1];
        }
      }

      for (int ib = 0; ib <= _underlyingFitFunction.OrderOfBaselinePolynomial; ++ib)
      {
        _parametersLocal[_offsetToBaselineParametersLocal + ib] = parameters[_offsetToBaselineParametersGlobal + indexOfSpectrum * (_underlyingFitFunction.OrderOfBaselinePolynomial + 1) + ib];
      }
    }

    /// <summary>Not implemented.</summary>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <summary>Not implemented.</summary>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

    /// <summary>Not implemented.</summary>
    public string IndependentVariableName(int i)
    {
      throw new NotImplementedException();
    }

    /// <summary>Not implemented.</summary>
    public string ParameterName(int i)
    {
      throw new NotImplementedException();
    }
  }
}
