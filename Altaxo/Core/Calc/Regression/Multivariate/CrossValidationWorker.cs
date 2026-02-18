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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Base class for cross validation evaluators. Provides common configuration and data required
  /// to perform cross validation for multivariate regression.
  /// </summary>
  public class CrossValidationWorker
  {
    /// <summary>
    /// The <c>X</c> values common to all unpreprocessed spectra (wavelength, frequency, etc.).
    /// </summary>
    protected double[] _xOfX;


    /// <summary>
    /// The number of factors to use.
    /// </summary>
    protected int _numFactors;

    /// <summary>
    /// Strategy used to group observations for cross validation.
    /// </summary>
    protected ICrossValidationGroupingStrategy _groupingStrategy;

    /// <summary>
    /// Preprocessor applied to a single spectrum.
    /// </summary>
    protected ISingleSpectrumPreprocessor _singleSpectrumPreprocessor;

    /// <summary>
    /// Preprocessor applied to an ensemble of spectra (mean/scale preprocessing).
    /// </summary>
    //protected IEnsembleMeanScalePreprocessor _ensembleOfSpectraPreprocessor;

    /// <summary>
    /// The multivariate regression analysis implementation.
    /// </summary>
    protected MultivariateRegression _analysis;

    /// <summary>
    /// Gets the number of factors used by the worker.
    /// </summary>
    public int NumberOfFactors { get { return _numFactors; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossValidationWorker"/> class.
    /// </summary>
    /// <param name="xOfX">The <c>X</c> values common to all unprocessed spectra.</param>
    /// <param name="numFactors">The initial number of factors to use.</param>
    /// <param name="groupingStrategy">The grouping strategy used for cross validation.</param>
    /// <param name="preprocessSingleSpectrum">The preprocessor applied to each single spectrum.</param>
    /// <param name="analysis">The analysis instance used to build the model and perform predictions.</param>
    public CrossValidationWorker(
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      MultivariateRegression analysis
      )
    {
      _xOfX = xOfX;
      _numFactors = numFactors;
      _groupingStrategy = groupingStrategy;
      _singleSpectrumPreprocessor = preprocessSingleSpectrum;
      _analysis = analysis;
    }
  }

  /// <summary>
  /// Evaluates the cross-validated predicted error sum of squares (Cross PRESS).
  /// </summary>
  public class CrossPRESSEvaluator : CrossValidationWorker
  {
#nullable disable
    /// <summary>
    /// Buffer used to store predicted <c>Y</c> values during evaluation.
    /// </summary>
    protected IMatrix<double> _predictedY;

    private double[] _crossPRESS;
#nullable enable

    /// <summary>
    /// Gets the accumulated Cross PRESS values (indexed by number of factors).
    /// </summary>
    public double[] CrossPRESS { get { return _crossPRESS; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossPRESSEvaluator"/> class.
    /// </summary>
    /// <param name="xOfX">The <c>X</c> values common to all unprocessed spectra.</param>
    /// <param name="numFactors">The initial number of factors to use.</param>
    /// <param name="groupingStrategy">The grouping strategy used for cross validation.</param>
    /// <param name="preprocessSingleSpectrum">The preprocessor applied to each single spectrum.</param>
    /// <param name="analysis">The analysis instance used to build the model and perform predictions.</param>
    public CrossPRESSEvaluator(
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      MultivariateRegression analysis
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, analysis)
    {
    }

    /// <summary>
    /// Calculates the Cross PRESS values.
    /// </summary>
    /// <param name="group">The group of spectra used for prediction. The array contains the indices of the measurements used for prediction.</param>
    /// <param name="analysisMatrixXRaw">The matrix of unpreprocessed spectra used for analysis.</param>
    /// <param name="analysisMatrixYRaw">The matrix of unpreprocessed target variables used for analysis.</param>
    /// <param name="predictionMatrixXRaw">The matrix of unpreprocessed spectra used for prediction.</param>
    /// <param name="predictionMatrixYRaw">The matrix of unpreprocessed target variables corresponding to the spectra used for prediction (for comparison with the predicted values).</param>
    public void EhCrossPRESS(int[] group, Matrix<double> analysisMatrixXRaw, Matrix<double> analysisMatrixYRaw, Matrix<double> predictionMatrixXRaw, Matrix<double> predictionMatrixYRaw)
    {
      if (_predictedY is null || _predictedY.RowCount != predictionMatrixYRaw.RowCount || _predictedY.ColumnCount != predictionMatrixYRaw.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(predictionMatrixYRaw.RowCount, predictionMatrixYRaw.ColumnCount);

      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _xOfX,
        analysisMatrixXRaw,
        analysisMatrixYRaw,
        out var xOfXPre, out var analysisMatrixXPre, out var auxiliaryDataX,
        out var analysisMatrixYPre,
        out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(analysisMatrixXPre, analysisMatrixYPre, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      var (xPre, predictionMatrixXPre, _) = _singleSpectrumPreprocessor.ExecuteForPrediction(_xOfX, predictionMatrixXRaw, null, auxiliaryDataX);

      // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
      _crossPRESS ??= new double[_numFactors + 1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

      // for all factors do now a prediction of the remaining spectra
      for (int nFactor = 0; nFactor <= _numFactors; nFactor++)
      {
        if (nFactor == 0)
        {
          MatrixMath.ZeroMatrix(_predictedY); // For zero factors, the predicted y would be zero
        }
        else
        {
          _analysis.PredictYFromPreprocessed(predictionMatrixXPre, nFactor, _predictedY);
        }

        MultivariateRegression.PostprocessTargetVariablesInline(_predictedY, meanY, scaleY);
        _crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(predictionMatrixYRaw, _predictedY);
      }
    }
  }

  /// <summary>
  /// Evaluates cross-validated predicted <c>Y</c> values.
  /// </summary>
  public class CrossPredictedYEvaluator : CrossValidationWorker
  {
#nullable disable
    /// <summary>
    /// Buffer used to store predicted <c>Y</c> values during evaluation.
    /// </summary>
    protected IMatrix<double> _predictedY;

    /// <summary>
    /// Destination matrix that receives the cross-validation predictions.
    /// </summary>
    public IMatrix<double> _YCrossValidationPrediction;
#nullable enable

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossPredictedYEvaluator"/> class.
    /// </summary>
    /// <param name="xOfX">The <c>X</c> values common to all unprocessed spectra.</param>
    /// <param name="numFactors">The initial number of factors to use.</param>
    /// <param name="groupingStrategy">The grouping strategy used for cross validation.</param>
    /// <param name="preprocessSingleSpectrum">The preprocessor applied to each single spectrum.</param>
    /// <param name="preprocessEnsembleOfSpectra">The preprocessor applied to an ensemble of spectra.</param>
    /// <param name="analysis">The analysis instance used to build the model and perform predictions.</param>
    /// <param name="YCrossValidationPrediction">Matrix that will receive the predicted <c>Y</c> values for all observations.</param>
    public CrossPredictedYEvaluator(
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      MultivariateRegression analysis,
      IMatrix<double> YCrossValidationPrediction
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, analysis)
    {
      _YCrossValidationPrediction = YCrossValidationPrediction;
    }

    /// <summary>
    /// Calculates cross-validated predicted <c>Y</c> values for the specified <paramref name="group"/> and writes them to
    /// <see cref="_YCrossValidationPrediction"/>
    /// </summary>
    /// <param name="group">Indices of the observations used for prediction.</param>
    /// <param name="XX">Unprocessed spectra used for analysis.</param>
    /// <param name="YY">Unprocessed target variables used for analysis.</param>
    /// <param name="XU">Unprocessed spectra used for prediction.</param>
    /// <param name="YU">Unprocessed target variables corresponding to <paramref name="XU"/>.</param>
    public void EhYCrossPredicted(int[] group,
      Matrix<double> XX, Matrix<double> YY,
      Matrix<double> XU, IMatrix<double> YU)
    {
      if (_predictedY is null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);

      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _xOfX,
        XX, YY,
        out var resultXOfX, out var resultXX, out var auxiliaryDataX, out var resultYY,
        out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      var (_, resultXU, _) = _singleSpectrumPreprocessor.ExecuteForPrediction(_xOfX, XU, null, auxiliaryDataX);
      _analysis.PredictYFromPreprocessed(resultXU, _numFactors, _predictedY);
      MultivariateRegression.PostprocessTargetVariablesInline(_predictedY, meanY, scaleY);

      for (int i = 0; i < group.Length; i++)
        MatrixMath.SetRow(_predictedY, i, _YCrossValidationPrediction, group[i]);
    }
  }

  /// <summary>
  /// Evaluator that computes cross-validated residuals in <c>X</c> (spectral residuals).
  /// </summary>
  public class CrossPredictedXResidualsEvaluator : CrossValidationWorker
  {
    private int _numberOfPoints;

#nullable disable
    /// <summary>
    /// Storage for the accumulated residuals in <c>X</c>.
    /// </summary>
    public IMatrix<double> _XCrossResiduals;

    /// <summary>
    /// Gets the matrix of cross-validated residuals in <c>X</c>.
    /// </summary>
    public IROMatrix<double> XCrossResiduals { get { return _XCrossResiduals; } }
#nullable enable

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossPredictedXResidualsEvaluator"/> class.
    /// </summary>
    /// <param name="numberOfPoints">Total number of observations (spectra).</param>
    /// <param name="xOfX">The <c>X</c> values common to all unprocessed spectra.</param>
    /// <param name="numFactors">The initial number of factors to use.</param>
    /// <param name="groupingStrategy">The grouping strategy used for cross validation.</param>
    /// <param name="preprocessSingleSpectrum">The preprocessor applied to each single spectrum.</param>
    /// <param name="preprocessEnsembleOfSpectra">The preprocessor applied to an ensemble of spectra.</param>
    /// <param name="analysis">The analysis instance used to build the model and compute residuals.</param>
    public CrossPredictedXResidualsEvaluator(
      int numberOfPoints,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      MultivariateRegression analysis
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, analysis)
    {
      _numberOfPoints = numberOfPoints;
    }

    /// <summary>
    /// Computes cross-validated residuals in <c>X</c> for the specified <paramref name="group"/>.
    /// </summary>
    /// <param name="group">Indices of the observations used for prediction.</param>
    /// <param name="XX">Unprocessed spectra used for analysis.</param>
    /// <param name="YY">Unprocessed target variables used for analysis.</param>
    /// <param name="XU">Unprocessed spectra used for prediction.</param>
    /// <param name="YU">Unprocessed target variables corresponding to <paramref name="XU"/>.</param>
    public void EhCrossValidationWorker(int[] group,
      Matrix<double> XX, Matrix<double> YY,
      Matrix<double> XU, IMatrix<double> YU)
    {
      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _xOfX,
        XX, YY,
        out var resultXOfX, out var resultXX, out var auxiliaryDataX,
        out var resultYY, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      var (_, resultXU, _) = _singleSpectrumPreprocessor.ExecuteForPrediction(_xOfX, XU, null, auxiliaryDataX);
      IROMatrix<double> xResidual = _analysis.SpectralResidualsFromPreprocessed(resultXU, _numFactors);

      if (_XCrossResiduals is null)
        _XCrossResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(_numberOfPoints, xResidual.ColumnCount);

      for (int i = 0; i < group.Length; i++)
        MatrixMath.SetRow(xResidual, i, _XCrossResiduals, group[i]);
    }
  }

  /// <summary>
  /// Evaluator that fills a <see cref="CrossValidationResult"/> instance with predicted <c>Y</c> values and spectral residuals
  /// for all factors.
  /// </summary>
  public class CrossValidationResultEvaluator : CrossValidationWorker
  {
    private CrossValidationResult _result;

#nullable disable
    /// <summary>
    /// Buffer used to store predicted <c>Y</c> values during evaluation.
    /// </summary>
    protected IMatrix<double> _predictedY;

    /// <summary>
    /// Buffer used to store spectral residuals during evaluation.
    /// </summary>
    protected IMatrix<double> _spectralResidual;
#nullable enable

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossValidationResultEvaluator"/> class.
    /// </summary>
    /// <param name="spectralRegions">Spectral regions (currently unused).</param>
    /// <param name="xOfX">The <c>X</c> values common to all unprocessed spectra.</param>
    /// <param name="numFactors">The initial number of factors to use.</param>
    /// <param name="groupingStrategy">The grouping strategy used for cross validation.</param>
    /// <param name="preprocessSingleSpectrum">The preprocessor applied to each single spectrum.</param>
    /// <param name="analysis">The analysis instance used to build the model and perform predictions.</param>
    /// <param name="result">The result instance to fill.</param>
    public CrossValidationResultEvaluator(
      int[] spectralRegions,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      MultivariateRegression analysis,
      CrossValidationResult result
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, analysis)
    {
      _result = result;
    }

    /// <summary>
    /// Computes predictions and spectral residuals for the specified <paramref name="group"/> and stores them in the associated
    /// <see cref="CrossValidationResult"/>.
    /// </summary>
    /// <param name="group">Indices of the observations used for prediction.</param>
    /// <param name="XX">Unprocessed spectra used for analysis.</param>
    /// <param name="YY">Unprocessed target variables used for analysis.</param>
    /// <param name="XU">Unprocessed spectra used for prediction.</param>
    /// <param name="YU">Unprocessed target variables corresponding to <paramref name="XU"/>.</param>
    public void EhCrossValidationWorker(int[] group, Matrix<double> XX, Matrix<double> YY, Matrix<double> XU, Matrix<double> YU)
    {
      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _xOfX, XX, YY,
        out var resultXOfX, out var resultXX, out var auxiliaryDataX,
        out var resultYY, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      var (_, resultXU, _) = _singleSpectrumPreprocessor.ExecuteForPrediction(_xOfX, XU, null, auxiliaryDataX);

      if (_predictedY is null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);
      if (_spectralResidual is null || _spectralResidual.RowCount != resultXU.RowCount || _spectralResidual.ColumnCount != _analysis.NumberOfSpectralResiduals)
        _spectralResidual = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(resultXU.RowCount, _analysis.NumberOfSpectralResiduals);

      for (int nFactor = 0; nFactor <= _numFactors; nFactor++)
      {
        _analysis.PredictedYAndSpectralResidualsFromPreprocessed(resultXU, nFactor, _predictedY, _spectralResidual);
        MultivariateRegression.PostprocessTargetVariablesInline(_predictedY, meanY, scaleY);

        for (int i = 0; i < group.Length; i++)
        {
          MatrixMath.SetRow(_predictedY, i, _result.GetPredictedYW(NumberOfFactors), group[i]);
          MatrixMath.SetRow(_spectralResidual, i, _result.GetSpectralResidualW(NumberOfFactors), group[i]);
        }
      }
    }
  }
}
