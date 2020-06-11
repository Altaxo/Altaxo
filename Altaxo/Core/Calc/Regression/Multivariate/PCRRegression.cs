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
  /// PCRRegression contains static methods for doing principal component regression analysis and prediction of the data.
  /// </summary>
  public class PCRRegression : MultivariateRegression
  {
#nullable disable
    private PCRCalibrationModel _calib;

    protected IExtensibleVector<double> _PRESS;
#nullable enable

    public override IROVector<double> GetPRESSFromPreprocessed(IROMatrix<double> matrixX)
    {
      CalculatePRESS(
        matrixX,
        _calib.XLoads,
        _calib.YLoads,
        _calib.XScores,
        _calib.CrossProduct,
        _calib.NumberOfFactors,
        out var result);

      return result;
    }

    protected override MultivariateCalibrationModel InternalCalibrationModel { get { return _calib; } }

    public override void SetCalibrationModel(IMultivariateCalibrationModel calib)
    {
      if (calib is PCRCalibrationModel)
        _calib = (PCRCalibrationModel)calib;
      else
        throw new ArgumentException("Expecting argument of type PCRCalibrationModel, but actual type is " + calib.GetType().ToString());
    }

    public override void Reset()
    {
      _calib = new PCRCalibrationModel();
      base.Reset();
    }

    /// <summary>
    /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
    /// </summary>
    /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maxFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    public static PCRRegression CreateFromPreprocessed(IROMatrix<double> matrixX, IROMatrix<double> matrixY, int maxFactors)
    {
      var result = new PCRRegression();
      result.AnalyzeFromPreprocessed(matrixX, matrixY, maxFactors);
      return result;
    }

    /// <summary>
    /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
    /// </summary>
    /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maxFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    protected override void AnalyzeFromPreprocessedWithoutReset(IROMatrix<double> matrixX, IROMatrix<double> matrixY, int maxFactors)
    {
      int numFactors = Math.Min(matrixX.ColumnCount, maxFactors);
      ExecuteAnalysis(matrixX, matrixY, ref numFactors, out var xLoads, out var xScores, out var V);

      var yLoads = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixY.RowCount, matrixY.ColumnCount);
      MatrixMath.Copy(matrixY, yLoads);

      _calib.NumberOfFactors = numFactors;
      _calib.XLoads = xLoads;
      _calib.YLoads = yLoads;
      _calib.XScores = xScores;
      _calib.CrossProduct = V;
    }

    /// <summary>
    /// This predicts concentrations of unknown spectra.
    /// </summary>
    /// <param name="XU">Matrix of unknown spectra (preprocessed the same way as the calibration spectra).</param>
    /// <param name="numFactors">Number of factors used for prediction.</param>
    /// <param name="predictedY">On return, holds the predicted y values. (They are centered).</param>
    /// <param name="spectralResiduals">On return, holds the spectral residual values.</param>
    public override void PredictedYAndSpectralResidualsFromPreprocessed(
      IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
      int numFactors, // number of factors to use for prediction
      IMatrix<double>? predictedY, // Matrix of predicted y-values, must be same number of rows as spectra
      IMatrix<double>? spectralResiduals // Matrix of spectral residuals, n rows x 1 column, can be zero
      )
    {
      if (numFactors > _calib.NumberOfFactors)
        throw new ArgumentOutOfRangeException(string.Format("Required numFactors (={0}) is higher than numFactors of analysis (={1})", numFactors, NumberOfFactors));

      Predict(
        XU, // unknown spectrum or spectra,  horizontal oriented
        _calib.XLoads, // x-loads matrix
        _calib.YLoads, // y-loads matrix
        _calib.XScores, // weighting matrix
        _calib.CrossProduct,  // Cross product vector
        numFactors, // number of factors to use for prediction
        predictedY, // Matrix of predicted y-values, must be same number of rows as spectra
        spectralResiduals // Matrix of spectral residuals, n rows x 1 column, can be zero
        );
    }

    /// <summary>
    /// Calculates the prediction scores (for use withthe preprocessed spectra).
    /// </summary>
    /// <param name="numFactors">Number of factors used to calculate the prediction scores.</param>
    /// <param name="predictionScores">Supplied matrix for holding the prediction scores.</param>
    protected override void InternalGetPredictionScores(int numFactors, IMatrix<double> predictionScores)
    {
      GetPredictionScoreMatrix(_calib.XLoads, _calib.YLoads, _calib.XScores, _calib.CrossProduct, numFactors, predictionScores);
    }

    protected override void InternalGetXLeverageFromPreprocessed(IROMatrix<double> matrixX, int numFactors, IMatrix<double> xLeverage)
    {
      CalculateXLeverageFromPreprocessed(_calib.XScores, numFactors, xLeverage);
    }

    public static void ExecuteAnalysis(
      IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
      ref int numFactors,
      out IROMatrix<double> xLoads, // out: the loads of the X matrix
      out IROMatrix<double> xScores, // matrix of weighting values
      out IROVector<double> V  // vector of cross products
      )
    {
      var matrixX = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(X.RowCount, X.ColumnCount);
      MatrixMath.Copy(X, matrixX);
      var decompose = new MatrixMath.SingularValueDecomposition(matrixX);

      numFactors = Math.Min(numFactors, matrixX.ColumnCount);
      numFactors = Math.Min(numFactors, matrixX.RowCount);

      xLoads = JaggedArrayMath.ToTransposedROMatrix(decompose.V, Y.RowCount, X.ColumnCount);
      xScores = JaggedArrayMath.ToMatrix(decompose.U, Y.RowCount, Y.RowCount);
      V = VectorMath.ToROVector(decompose.Diagonal, numFactors);
    }

    private static void CalculatePRESS(
      IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
      IROMatrix<double> xLoads, // out: the loads of the X matrix
      IROMatrix<double> xScores, // matrix of weighting values
      IReadOnlyList<double> V,  // vector of cross products
      int maxNumberOfFactors,
      IVector<double> PRESS //vector of Y PRESS values
      )
    {
      var U = xScores;
      var UtY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(Y.RowCount, Y.ColumnCount);
      MatrixMath.MultiplyFirstTransposed(U, Y, UtY);

      var predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(Y.RowCount, Y.ColumnCount);
      var subU = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(Y.RowCount, 1);
      var subY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(Y.RowCount, Y.ColumnCount);

      PRESS[0] = MatrixMath.SumOfSquares(Y);

      int numFactors = Math.Min(maxNumberOfFactors, V.Count);

      // now calculate PRESS by predicting the y
      // using yp = U (w*(1/w)) U' y
      // of course w*1/w is the identity matrix, but we use only the first factors, so using a cutted identity matrix
      // we precalculate the last term U'y = UtY
      // and multiplying with one row of U in every factor step, summing up the predictedY
      for (int nf = 0; nf < numFactors; nf++)
      {
        for (int cn = 0; cn < Y.ColumnCount; cn++)
        {
          for (int k = 0; k < Y.RowCount; k++)
            predictedY[k, cn] += U[k, nf] * UtY[nf, cn];
        }
        PRESS[nf + 1] = MatrixMath.SumOfSquaredDifferences(Y, predictedY);
      }
    }

    public static void Predict(
      IROMatrix<double> matrixX,
      IROMatrix<double> xLoads,
      IROMatrix<double> yLoads,
      IROMatrix<double> xScores,
      IReadOnlyList<double> crossProduct,
      int numberOfFactors,
      IMatrix<double>? predictedY,
      IMatrix<double>? spectralResiduals)
    {
      int numX = xLoads.ColumnCount;
      int numY = yLoads.ColumnCount;
      int numM = yLoads.RowCount;

      if (!(predictedY is null))
      {
        var predictionScores = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numX, numY);
        GetPredictionScoreMatrix(xLoads, yLoads, xScores, crossProduct, numberOfFactors, predictionScores);
        MatrixMath.Multiply(matrixX, predictionScores, predictedY);
      }

      if (!(spectralResiduals is null))
        GetSpectralResiduals(matrixX, xLoads, yLoads, xScores, crossProduct, numberOfFactors, spectralResiduals);
    }

    public static void GetPredictionScoreMatrix(
      IROMatrix<double> xLoads,
      IROMatrix<double> yLoads,
      IROMatrix<double> xScores,
      IReadOnlyList<double> crossProduct,
      int numberOfFactors,
      IMatrix<double> predictionScores)
    {
      int numX = xLoads.ColumnCount;
      int numY = yLoads.ColumnCount;
      int numM = yLoads.RowCount;

      var UtY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(xScores.ColumnCount, yLoads.ColumnCount);
      MatrixMath.MultiplyFirstTransposed(xScores, yLoads, UtY);

      MatrixMath.ZeroMatrix(predictionScores);

      for (int nf = 0; nf < numberOfFactors; nf++)
      {
        double scale = 1 / crossProduct[nf];
        for (int cn = 0; cn < numY; cn++)
        {
          for (int k = 0; k < numX; k++)
            predictionScores[k, cn] += scale * xLoads[nf, k] * UtY[nf, cn];
        }
      }
    }

    public static void CalculatePRESS(
      IROMatrix<double> yLoads,
      IROMatrix<double> xScores,
      int numberOfFactors,
      out IROVector<double> press)
    {
      int numMeasurements = yLoads.RowCount;

      IExtensibleVector<double> PRESS = VectorMath.CreateExtensibleVector<double>(numberOfFactors + 1);
      var UtY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(yLoads.RowCount, yLoads.ColumnCount);
      var predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(yLoads.RowCount, yLoads.ColumnCount);
      press = PRESS;

      MatrixMath.MultiplyFirstTransposed(xScores, yLoads, UtY);

      // now calculate PRESS by predicting the y
      // using yp = U (w*(1/w)) U' y
      // of course w*1/w is the identity matrix, but we use only the first factors, so using a cutted identity matrix
      // we precalculate the last term U'y = UtY
      // and multiplying with one row of U in every factor step, summing up the predictedY
      PRESS[0] = MatrixMath.SumOfSquares(yLoads);
      for (int nf = 0; nf < numberOfFactors; nf++)
      {
        for (int cn = 0; cn < yLoads.ColumnCount; cn++)
        {
          for (int k = 0; k < yLoads.RowCount; k++)
            predictedY[k, cn] += xScores[k, nf] * UtY[nf, cn];
        }
        PRESS[nf + 1] = MatrixMath.SumOfSquaredDifferences(yLoads, predictedY);
      }
    }

    public static void CalculatePRESS(
      IROMatrix<double> matrixX,
      IROMatrix<double> xLoads,
      IROMatrix<double> yLoads,
      IROMatrix<double> xScores,
      IReadOnlyList<double> crossProduct,
      int numberOfFactors,
      out IROVector<double> PRESS)
    {
      IMatrix<double> predictedY = new JaggedArrayMatrix(yLoads.RowCount, yLoads.ColumnCount);
      var press = VectorMath.CreateExtensibleVector<double>(numberOfFactors + 1);
      PRESS = press;

      press[0] = MatrixMath.SumOfSquares(yLoads);
      for (int nf = 0; nf < numberOfFactors; nf++)
      {
        Predict(matrixX, xLoads, yLoads, xScores, crossProduct, nf, predictedY, null);
        press[nf + 1] = MatrixMath.SumOfSquaredDifferences(yLoads, predictedY);
      }
    }

    public static void GetSpectralResiduals(
      IROMatrix<double> matrixX,
      IROMatrix<double> xLoads,
      IROMatrix<double> yLoads,
      IROMatrix<double> xScores,
      IReadOnlyList<double> crossProduct,
      int numberOfFactors,
      IMatrix<double> spectralResiduals)
    {
      int numX = xLoads.ColumnCount;
      int numY = yLoads.ColumnCount;
      int numM = yLoads.RowCount;

      var reconstructedSpectra = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixX.RowCount, matrixX.ColumnCount);
      MatrixMath.ZeroMatrix(reconstructedSpectra);

      for (int nf = 0; nf < numberOfFactors; nf++)
      {
        double scale = crossProduct[nf];
        for (int m = 0; m < numM; m++)
        {
          for (int k = 0; k < numX; k++)
            reconstructedSpectra[m, k] += scale * xScores[m, nf] * xLoads[nf, k];
        }
      }
      for (int m = 0; m < numM; m++)
        spectralResiduals[m, 0] = MatrixMath.SumOfSquaredDifferences(
          MatrixMath.ToROSubMatrix(matrixX, m, 0, 1, matrixX.ColumnCount),
          MatrixMath.ToROSubMatrix(reconstructedSpectra, m, 0, 1, matrixX.ColumnCount));
    }

    public static void CalculateXLeverageFromPreprocessed(
      IROMatrix<double> xScores,
      int numberOfFactors,
      IMatrix<double> leverage)
    {
      var subscores = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(xScores.RowCount, numberOfFactors);
      MatrixMath.Submatrix(xScores, subscores);

      var decompose = new MatrixMath.SingularValueDecomposition(subscores);

      for (int i = 0; i < xScores.RowCount; i++)
        leverage[i, 0] = decompose.HatDiagonal[i];
    }
  }
}
