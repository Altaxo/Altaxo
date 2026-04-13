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
  /// Implements Partial Least Squares regression (PLS1), i.e. PLS regression for multiple target variables
  /// by running a separate PLS2 model for each target variable.
  /// </summary>
  public class PLS1Regression : MultivariateRegression
  {
#nullable disable
    private PLS1CalibrationModel _calib;

    /// <summary>
    /// Stores the predicted error sum of squares (PRESS) values.
    /// </summary>
    protected IExtensibleVector<double> _PRESS;
#nullable enable

    /// <summary>
    /// Gets the predicted error sum of squares (PRESS) values accumulated during analysis.
    /// </summary>
    public IReadOnlyList<double> PRESS { get { return _PRESS; } }

    /// <inheritdoc/>
    public override IReadOnlyList<double> GetPRESSFromPreprocessed(IROMatrix<double> matrixXPre)
    {
      return _PRESS;
    }

    /// <inheritdoc/>
    protected override MultivariateCalibrationModel InternalCalibrationModel { get { return _calib; } }

    /// <inheritdoc/>
    public override void SetCalibrationModel(IMultivariateCalibrationModel calibrationModel)
    {
      if (calibrationModel is PLS1CalibrationModel)
        _calib = (PLS1CalibrationModel)calibrationModel;
      else
        throw new ArgumentException("Expecting argument of type PLS1CalibrationModel, but actual type is " + calibrationModel.GetType().ToString());
    }

    /// <inheritdoc/>
    public override int NumberOfSpectralResiduals { get { return _calib.NumberOfY; } }

    /// <inheritdoc/>
    public override void Reset()
    {
      _calib = new PLS1CalibrationModel();
      base.Reset();
    }

    /// <summary>
    /// Creates a PLS1 regression from preprocessed spectra and preprocessed target variables.
    /// </summary>
    /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). It must at least be centered.</param>
    /// <param name="matrixY">The matrix of target variables (each experiment is a row in the matrix). It must at least be centered.</param>
    /// <param name="maxFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression instance containing the loads and weights necessary for further calculations.</returns>
    public static PLS1Regression CreateFromPreprocessed(IROMatrix<double> matrixX, IROMatrix<double> matrixY, int maxFactors)
    {
      var result = new PLS1Regression();
      result.AnalyzeFromPreprocessed(matrixX, matrixY, maxFactors);
      return result;
    }

    /// <inheritdoc/>
    protected override void AnalyzeFromPreprocessedWithoutReset(IROMatrix<double> matrixXPre, IROMatrix<double> matrixYPre, int maximalNumberOfFactors)
    {
      int numberOfFactors = _calib.NumberOfFactors = Math.Min(matrixXPre.ColumnCount, maximalNumberOfFactors);
      IMatrix<double> helperY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixYPre.RowCount, 1);

      _PRESS = null;

      for (int i = 0; i < matrixYPre.ColumnCount; i++)
      {
        MatrixMath.Submatrix(matrixYPre, helperY, 0, i);

        var r = PLS2Regression.CreateFromPreprocessed(matrixXPre, helperY, maximalNumberOfFactors);

        IPLS2CalibrationModel cal = r.CalibrationModel;
        _calib.NumberOfFactors = Math.Min(_calib.NumberOfFactors, cal.NumberOfFactors);
        _calib.XLoads[i] = cal.XLoads;
        _calib.YLoads[i] = cal.YLoads;
        _calib.XWeights[i] = cal.XWeights;
        _calib.CrossProduct[i] = cal.CrossProduct;

        if (_PRESS is null)
          _PRESS = VectorMath.CreateExtensibleVector<double>(r.PRESS.Count);
        VectorMath.Add(_PRESS, r.PRESS, _PRESS);
      }
    }

    /// <inheritdoc/>
    public override void PredictedYAndSpectralResidualsFromPreprocessed(
      IROMatrix<double> XUPre, // unknown spectrum or spectra,  horizontal oriented
      int numberOfFactors, // number of factors to use for prediction
      IMatrix<double>? predictedY, // Matrix of predicted y-values, must be same number of rows as spectra
      IMatrix<double>? spectralResiduals // Matrix of spectral residuals, n rows x 1 column, can be zero
      )
    {
      if (numberOfFactors > NumberOfFactors)
        throw new ArgumentOutOfRangeException(string.Format("Required numFactors (={0}) is higher than numFactors of analysis (={1})", numberOfFactors, NumberOfFactors));

      IMatrix<double>? helperY = predictedY is null ? null : new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XUPre.RowCount, 1);
      IMatrix<double>? helperS = spectralResiduals is null ? null : new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XUPre.RowCount, 1);
      for (int i = 0; i < _calib.NumberOfY; i++)
      {
        PLS2Regression.Predict(
          XUPre, // unknown spectrum or spectra,  horizontal oriented
          _calib.XLoads[i], // x-loads matrix
          _calib.YLoads[i], // y-loads matrix
          _calib.XWeights[i], // weighting matrix
          _calib.CrossProduct[i],  // Cross product vector
          numberOfFactors, // number of factors to use for prediction
          helperY, // Matrix of predicted y-values, must be same number of rows as spectra
          helperS // Matrix of spectral residuals, n rows x 1 column, can be zero
          );

        if (!(helperY is null || predictedY is null))
          MatrixMath.Copy(helperY, predictedY, 0, i);
        if (!(spectralResiduals is null || helperS is null))
          MatrixMath.Copy(helperS, spectralResiduals, 0, i);
      }
    }

    /// <inheritdoc/>
    protected override void InternalGetPredictionScores(int numberOfFactors, IMatrix<double> predictionScores)
    {
      IMatrix<double> pred = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(predictionScores.RowCount, 1);
      for (int i = 0; i < _calib.NumberOfY; i++)
      {
        PLS2Regression.GetPredictionScoreMatrix(_calib.XLoads[i], _calib.YLoads[i], _calib.XWeights[i], _calib.CrossProduct[i], numberOfFactors, pred);
        MatrixMath.SetColumn(pred, predictionScores, i);
      }
    }

    /// <inheritdoc/>
    protected override void InternalGetXLeverageFromPreprocessed(IROMatrix<double> matrixXPre, int numberOfFactors, IMatrix<double> xLeverage)
    {
      for (int i = 0; i < _calib.NumberOfY; i++)
        PLS2Regression.CalculateXLeverageFromPreprocessed(matrixXPre, _calib.XWeights[i], numberOfFactors, xLeverage, i);
    }
  }
}
