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
    /// <summary>
    /// The spectral regions of the unpreprocessed spectra. Each element in this array is the starting index of a new spectral region.
    /// </summary>
    protected int[] _spectralRegions;

    /// <summary>The x-values common to all unpreprocessed spectra (wavelength, frequency etc.)</summary>
    protected double[] _xOfX;


    protected int _numFactors;

    protected ICrossValidationGroupingStrategy _groupingStrategy;
    protected ISingleSpectrumPreprocessor _singleSpectrumPreprocessor;
    protected IEnsembleMeanScalePreprocessor _ensembleOfSpectraPreprocessor;
    protected MultivariateRegression _analysis;

    public int NumberOfFactors { get { return _numFactors; } }

    public CrossValidationWorker(
      int[] spectralRegions,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis
      )
    {
      _spectralRegions = spectralRegions;
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
      int[] spectralRegions,
      double[] xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression analysis
      )
      : base(spectralRegions, xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
    {
    }

    public void EhCrossPRESS(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
    {
      if (_predictedY is null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
        _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);

      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _spectralRegions,
        _xOfX,
        XX,
        YY,
        out var resultXOfX, out var resultMatrixX, out var resultMatrixY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);
      _analysis.AnalyzeFromPreprocessed(XX, YY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
         _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _spectralRegions,
        _xOfX,
        XU, meanX, scaleX,
        out var resultXU);

      // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
      if (_crossPRESS is null)
        _crossPRESS = new double[_numFactors + 1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

      // for all factors do now a prediction of the remaining spectra
      _crossPRESS[0] += MatrixMath.SumOfSquares(YU);
      for (int nFactor = 1; nFactor <= _numFactors; nFactor++)
      {
        _analysis.PredictYFromPreprocessed(resultXU, nFactor, _predictedY);
        MultivariateRegression.PostprocessY(_predictedY, meanY, scaleY);
        _crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(YU, _predictedY);
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
      : base(spectralRegions, xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
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
        _spectralRegions, _xOfX,
        XX, YY,
        out var resultXOfX, out var resultXX, out var resultYY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _spectralRegions,
        _xOfX,
        XU, meanX, scaleX, out var resultXU);

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
      : base(spectralRegions, xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
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
        _spectralRegions, _xOfX,
        XX, YY,
        out var resultXOfX, out var resultXX, out var resultYY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
         _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _spectralRegions, _xOfX,
        XU, meanX, scaleX, out var resultXU);
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
      : base(spectralRegions, xOfX, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, analysis)
    {
      _result = result;
    }

    public void EhCrossValidationWorker(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
    {
      MultivariateRegression.PreprocessForAnalysis(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _spectralRegions, _xOfX, XX, YY,
        out var resultXOfX, out var resultXX, out var resultYY,
        out var meanX, out var scaleX, out var meanY, out var scaleY);

      _analysis.AnalyzeFromPreprocessed(resultXX, resultYY, _numFactors);
      _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(
        _singleSpectrumPreprocessor,
        _ensembleOfSpectraPreprocessor,
        _spectralRegions, _xOfX,
         XU, meanX, scaleX,
         out var resultXU);

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
