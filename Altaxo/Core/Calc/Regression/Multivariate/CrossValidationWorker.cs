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
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleMeanScale;

namespace Altaxo.Calc.Regression.Multivariate
{
  public class CrossValidationWorker
  {
    /// <summary>The x-values common to all unpreprocessed spectra (wavelength, frequency etc.)</summary>
    protected double[] _xOfX;


    protected int _numFactors;

    protected ICrossValidationGroupingStrategy _groupingStrategy;
    protected ISingleSpectrumPreprocessor _singleSpectrumPreprocessor;
    protected IEnsembleMeanScalePreprocessor _ensembleOfSpectraPreprocessor;
    protected MultivariateRegression _analysis;

    public int NumberOfFactors { get { return _numFactors; } }

    public CrossValidationWorker(
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis
      )
    {
      _xOfX = xOfX;
      _numFactors = numFactors;
      _groupingStrategy = groupingStrategy;
      _singleSpectrumPreprocessor = preprocessSingleSpectrum;
      _ensembleOfSpectraPreprocessor = preprocessEnsembleOfSpectra;
      _analysis = analysis;
    }
  }

  public class CrossPRESSEvaluator : CrossValidationWorker
  {
#nullable disable
    protected IMatrix<double> _predictedY;
    private double[] _crossPRESS;
#nullable enable
    public double[] CrossPRESS { get { return _crossPRESS; } }

    public CrossPRESSEvaluator(
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
    {
    }

    /// <summary>
    /// Calculates the CrossPRESS values.
    /// </summary>
    /// <param name="group">The group of spectra used for prediction. The array contains the indices of the measurements used for prediction.</param>
    /// <param name="analysisMatrixXRaw">The matrix of unpreprocessed spectra used for analysis.</param>
    /// <param name="analysisMatrixYRaw">The matrix of unpreprocessed target variables used for analysis.</param>
    /// <param name="predictionMatrixXRaw">The matrix of unpreprocessed spectra used for prediction.</param>
    /// <param name="predictionMatrixYRaw">The matrix of unpreprocessed target variables corresponding to the spectra used for prediction (for comparison with the predicted values).</param>
    public void EhCrossPRESS(int[] group, IMatrix<double> analysisMatrixXRaw, IMatrix<double> analysisMatrixYRaw, IMatrix<double> predictionMatrixXRaw, IMatrix<double> predictionMatrixYRaw)
    {
      if (_predictedY is null || _predictedY.RowCount != predictionMatrixYRaw.RowCount || _predictedY.ColumnCount != predictionMatrixYRaw.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(predictionMatrixYRaw.RowCount, predictionMatrixYRaw.ColumnCount);

      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX,
        analysisMatrixXRaw,
        analysisMatrixYRaw,
        out var xOfXPre, out var analysisMatrixXPre, out var analysisMatrixYPre,
        out var meanX, out var scaleX, out var meanY, out var scaleY);
      _analysis.AnalyzeFromPreprocessed(analysisMatrixXPre, analysisMatrixYPre, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
         _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX,
        predictionMatrixXRaw, meanX, scaleX,
        out var predictionMatrixXPre, out var _);

      // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
      _crossPRESS ??= new double[_numFactors + 1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

      // for all factors do now a prediction of the remaining spectra
      for (int nFactor = 0; nFactor <= _numFactors; nFactor++)
      {
        if(nFactor == 0)
        {
          MatrixMath.ZeroMatrix(_predictedY); // For zero factors, the predicted y would be zero
        }
        else
        {
          _analysis.PredictYFromPreprocessed(predictionMatrixXPre, nFactor, _predictedY);
        }

        MultivariateRegression.PostprocessY(_predictedY, meanY, scaleY);
        _crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(predictionMatrixYRaw, _predictedY);
      }
    }
  }

  public class CrossPredictedYEvaluator : CrossValidationWorker
  {
#nullable disable
    protected IMatrix<double> _predictedY;
    public IMatrix<double> _YCrossValidationPrediction;
#nullable enable

    public CrossPredictedYEvaluator(
      int[] spectralRegions,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis,
      IMatrix<double> YCrossValidationPrediction
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
    {
      _YCrossValidationPrediction = YCrossValidationPrediction;
    }

    public void EhYCrossPredicted(int[] group,
      IMatrix<double> XX, IMatrix<double> YY,
      IMatrix<double> XU, IMatrix<double> YU)
    {
      if (_predictedY is null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);

      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor, _ensembleOfSpectraPreprocessor,
        _xOfX,
        XX, YY,
        out var resultXOfX, out var resultXX, out var resultYY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX,
        XU, meanX, scaleX, out var resultXU, out var _);

      _analysis.PredictYFromPreprocessed(resultXU, _numFactors, _predictedY);
      MultivariateRegression.PostprocessY(_predictedY, meanY, scaleY);

      for (int i = 0; i < group.Length; i++)
        MatrixMath.SetRow(_predictedY, i, _YCrossValidationPrediction, group[i]);
    }
  }

  public class CrossPredictedXResidualsEvaluator : CrossValidationWorker
  {
    private int _numberOfPoints;

#nullable disable
    public IMatrix<double> _XCrossResiduals;
    public IROMatrix<double> XCrossResiduals { get { return _XCrossResiduals; } }
#nullable enable

    public CrossPredictedXResidualsEvaluator(
      int numberOfPoints,
      int[] spectralRegions,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
    {
      _numberOfPoints = numberOfPoints;
    }

    public void EhCrossValidationWorker(int[] group,
      IMatrix<double> XX, IMatrix<double> YY,
      IMatrix<double> XU, IMatrix<double> YU)
    {
      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX,
        XX, YY,
        out var resultXOfX, out var resultXX, out var resultYY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
         _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX,
        XU, meanX, scaleX, out var resultXU, out var _);
      IROMatrix<double> xResidual = _analysis.SpectralResidualsFromPreprocessed(resultXU, _numFactors);

      if (_XCrossResiduals is null)
        _XCrossResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(_numberOfPoints, xResidual.ColumnCount);

      for (int i = 0; i < group.Length; i++)
        MatrixMath.SetRow(xResidual, i, _XCrossResiduals, group[i]);
    }
  }

  public class CrossValidationResultEvaluator : CrossValidationWorker
  {
    private CrossValidationResult _result;

#nullable disable
    protected IMatrix<double> _predictedY;
    protected IMatrix<double> _spectralResidual;
#nullable enable

    public CrossValidationResultEvaluator(
      int[] spectralRegions,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis,
      CrossValidationResult result
      )
      : base(xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
    {
      _result = result;
    }

    public void EhCrossValidationWorker(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
    {
      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX, XX, YY,
        out var resultXOfX, out var resultXX, out var resultYY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _xOfX,
         XU, meanX, scaleX,
         out var resultXU, out var _);

      if (_predictedY is null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);
      if (_spectralResidual is null || _spectralResidual.RowCount != resultXU.RowCount || _spectralResidual.ColumnCount != _analysis.NumberOfSpectralResiduals)
        _spectralResidual = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(resultXU.RowCount, _analysis.NumberOfSpectralResiduals);

      for (int nFactor = 0; nFactor <= _numFactors; nFactor++)
      {
        _analysis.PredictedYAndSpectralResidualsFromPreprocessed(resultXU, nFactor, _predictedY, _spectralResidual);
        MultivariateRegression.PostprocessY(_predictedY, meanY, scaleY);

        for (int i = 0; i < group.Length; i++)
        {
          MatrixMath.SetRow(_predictedY, i, _result.GetPredictedYW(NumberOfFactors), group[i]);
          MatrixMath.SetRow(_spectralResidual, i, _result.GetSpectralResidualW(NumberOfFactors), group[i]);
        }
      }
    }
  }
}
