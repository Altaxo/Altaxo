#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Stores the result(s) of cross validation.
  /// </summary>
  public interface ICrossValidationResult
  {
    /// <summary>
    /// Gets the predicted y values for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for prediction.</param>
    /// <returns>The matrix of predicted y.(Columns=number of concentrations, rows=Number of points).</returns>
    IROMatrix<double> GetPredictedY(int numberOfFactor);

    /// <summary>
    /// Gets the spectral residuals for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for calculation.</param>
    /// <returns>The matrix of spectral residuals. (rows=Number of points).</returns>
    IROMatrix<double> GetSpectralResidual(int numberOfFactor);

    /// <summary>
    /// Get the cross PRESS vector.
    /// </summary>
    IReadOnlyList<double> CrossPRESS { get; }

    /// <summary>
    /// Returns the mean number of excluded spectra during cross validation.
    /// </summary>
    double MeanNumberOfExcludedSpectra { get; }
  }

  /// <summary>
  /// Stores the result(s) of cross validation.
  /// </summary>
  public class CrossValidationResult : ICrossValidationResult
  {
    public IMatrix<double>[] _predictedY;
    public IMatrix<double>[] _spectralResidual;
    public IVector<double> _crossPRESS;
    public double _MeanNumberExcludedSpectra;

    public CrossValidationResult(int numberOfPoints, int numberOfY, int numberOfFactors, bool multipleSpectralResiduals)
    {
      _predictedY = new IMatrix<double>[numberOfFactors + 1];
      _spectralResidual = new IMatrix<double>[numberOfFactors + 1];
      _crossPRESS = VectorMath.CreateExtensibleVector<double>(numberOfFactors + 1);

      for (int i = 0; i <= numberOfFactors; i++)
      {
        _predictedY[i] = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfPoints, numberOfY);
        _spectralResidual[i] = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfPoints, multipleSpectralResiduals ? numberOfY : 1);
      }
    }

    /// <summary>
    /// Gets the predicted y values for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for prediction.</param>
    /// <returns>The matrix of predicted y.(Columns=number of concentrations, rows=Number of points).</returns>
    public IROMatrix<double> GetPredictedY(int numberOfFactor)
    {
      return _predictedY[numberOfFactor];
    }

    /// <summary>
    /// Gets the predicted y values for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for prediction.</param>
    /// <returns>The matrix of predicted y.(Columns=number of concentrations, rows=Number of points).</returns>
    public IMatrix<double> GetPredictedYW(int numberOfFactor)
    {
      return _predictedY[numberOfFactor];
    }

    /// <summary>
    /// Gets the spectral residuals for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for calculation.</param>
    /// <returns>The matrix of spectral residuals. (rows=Number of points).</returns>
    public IROMatrix<double> GetSpectralResidual(int numberOfFactor)
    {
      return _spectralResidual[numberOfFactor];
    }

    /// <summary>
    /// Gets the spectral residuals for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for calculation.</param>
    /// <returns>The matrix of spectral residuals. (rows=Number of points).</returns>
    public IMatrix<double> GetSpectralResidualW(int numberOfFactor)
    {
      return _spectralResidual[numberOfFactor];
    }

    /// <summary>
    /// Get the cross PRESS vector.
    /// </summary>
    public IReadOnlyList<double> CrossPRESS { get { return _crossPRESS; } }

    /// <summary>
    /// Get the cross PRESS vector.
    /// </summary>
    public IReadOnlyList<double> CrossPRESSW { get { return _crossPRESS; } }

    /// <summary>
    /// Returns the mean number of excluded spectra during cross validation.
    /// </summary>
    public double MeanNumberOfExcludedSpectra { get { return _MeanNumberExcludedSpectra; } }
  }
}
