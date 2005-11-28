#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion
using System;
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
    IROMatrix GetPredictedY(int numberOfFactor);

    /// <summary>
    /// Gets the spectral residuals for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for calculation.</param>
    /// <returns>The matrix of spectral residuals. (rows=Number of points).</returns>
    IROMatrix GetSpectralResidual(int numberOfFactor);

    /// <summary>
    /// Get the cross PRESS vector.
    /// </summary>
    IROVector CrossPRESS { get; }

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
    public IMatrix[] _predictedY;
    public IMatrix[] _spectralResidual;
    public IVector  _crossPRESS;
    public double _MeanNumberExcludedSpectra;

    public CrossValidationResult(int numberOfPoints, int numberOfY, int numberOfFactors, bool multipleSpectralResiduals)
    {
      _predictedY = new IMatrix[numberOfFactors+1];
      _spectralResidual = new IMatrix[numberOfFactors+1];
      _crossPRESS = VectorMath.CreateExtensibleVector(numberOfFactors+1);
      
      for(int i=0;i<=numberOfFactors;i++)
      {
        _predictedY[i] = new MatrixMath.BEMatrix(numberOfPoints,numberOfY);
        _spectralResidual[i] = new MatrixMath.BEMatrix(numberOfPoints,multipleSpectralResiduals ? numberOfY : 1);
      }
    }


    /// <summary>
    /// Gets the predicted y values for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for prediction.</param>
    /// <returns>The matrix of predicted y.(Columns=number of concentrations, rows=Number of points).</returns>
    public IROMatrix GetPredictedY(int numberOfFactor)
    {
      return _predictedY[numberOfFactor];
    }

    /// <summary>
    /// Gets the predicted y values for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for prediction.</param>
    /// <returns>The matrix of predicted y.(Columns=number of concentrations, rows=Number of points).</returns>
    public IMatrix GetPredictedYW(int numberOfFactor)
    {
      return _predictedY[numberOfFactor];
    }

    /// <summary>
    /// Gets the spectral residuals for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for calculation.</param>
    /// <returns>The matrix of spectral residuals. (rows=Number of points).</returns>
    public IROMatrix GetSpectralResidual(int numberOfFactor)
    {
      return _spectralResidual[numberOfFactor];
    }


    /// <summary>
    /// Gets the spectral residuals for cross validation.
    /// </summary>
    /// <param name="numberOfFactor">Number of factors to use for calculation.</param>
    /// <returns>The matrix of spectral residuals. (rows=Number of points).</returns>
    public IMatrix GetSpectralResidualW(int numberOfFactor)
    {
      return _spectralResidual[numberOfFactor];
    }

    /// <summary>
    /// Get the cross PRESS vector.
    /// </summary>
    public IROVector CrossPRESS { get { return _crossPRESS; }}


    /// <summary>
    /// Get the cross PRESS vector.
    /// </summary>
    public IVector CrossPRESSW { get { return _crossPRESS; }}

    /// <summary>
    /// Returns the mean number of excluded spectra during cross validation.
    /// </summary>
    public double MeanNumberOfExcludedSpectra { get { return _MeanNumberExcludedSpectra; }}

  }



  
}
