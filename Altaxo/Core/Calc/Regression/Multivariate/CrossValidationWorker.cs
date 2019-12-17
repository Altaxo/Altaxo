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

namespace Altaxo.Calc.Regression.Multivariate
{
    public class CrossValidationWorker
    {
        protected int[] _spectralRegions;
        protected int _numFactors;

        protected ICrossValidationGroupingStrategy _groupingStrategy;
        protected SpectralPreprocessingOptions _preprocessOptions;
        protected MultivariateRegression _analysis;

        public int NumberOfFactors { get { return _numFactors; } }

        public CrossValidationWorker(
          int[] spectralRegions,
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression analysis
          )
        {
            _spectralRegions = spectralRegions;
            _numFactors = numFactors;
            _groupingStrategy = groupingStrategy;
            _preprocessOptions = preprocessOptions;
            _analysis = analysis;
        }
    }

    public class CrossPRESSEvaluator : CrossValidationWorker
    {
        protected IMatrix<double> _predictedY;

        private double[] _crossPRESS;

        public double[] CrossPRESS { get { return _crossPRESS; } }

        public CrossPRESSEvaluator(
          int[] spectralRegions,
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression analysis
          )
          : base(spectralRegions, numFactors, groupingStrategy, preprocessOptions, analysis)
        {
        }

        public void EhCrossPRESS(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
        {
            if (_predictedY == null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
                _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);

            MultivariateRegression.PreprocessForAnalysis(_preprocessOptions, _spectralRegions, XX, YY, out var meanX, out var scaleX, out var meanY, out var scaleY);
            _analysis.AnalyzeFromPreprocessed(XX, YY, _numFactors);
            _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

            MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions, XU, meanX, scaleX);

            // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
            if (null == _crossPRESS)
                _crossPRESS = new double[_numFactors + 1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

            // for all factors do now a prediction of the remaining spectra
            _crossPRESS[0] += MatrixMath.SumOfSquares(YU);
            for (int nFactor = 1; nFactor <= _numFactors; nFactor++)
            {
                _analysis.PredictYFromPreprocessed(XU, nFactor, _predictedY);
                MultivariateRegression.PostprocessY(_predictedY, meanY, scaleY);
                _crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(YU, _predictedY);
            }
        }
    }

    public class CrossPredictedYEvaluator : CrossValidationWorker
    {
        protected IMatrix<double> _predictedY;
        public IMatrix<double> _YCrossValidationPrediction;

        public CrossPredictedYEvaluator(
          int[] spectralRegions,
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression analysis,
          IMatrix<double> YCrossValidationPrediction
          )
          : base(spectralRegions, numFactors, groupingStrategy, preprocessOptions, analysis)
        {
            _YCrossValidationPrediction = YCrossValidationPrediction;
        }

        public void EhYCrossPredicted(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
        {
            if (_predictedY == null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
                _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);

            MultivariateRegression.PreprocessForAnalysis(_preprocessOptions, _spectralRegions, XX, YY,
              out var meanX, out var scaleX, out var meanY, out var scaleY);

            _analysis.AnalyzeFromPreprocessed(XX, YY, _numFactors);
            _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

            MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions, XU, meanX, scaleX);
            _analysis.PredictYFromPreprocessed(XU, _numFactors, _predictedY);
            MultivariateRegression.PostprocessY(_predictedY, meanY, scaleY);

            for (int i = 0; i < group.Length; i++)
                MatrixMath.SetRow(_predictedY, i, _YCrossValidationPrediction, group[i]);
        }
    }

    public class CrossPredictedXResidualsEvaluator : CrossValidationWorker
    {
        private int _numberOfPoints;

        public IMatrix<double> _XCrossResiduals;

        public IROMatrix<double> XCrossResiduals { get { return _XCrossResiduals; } }

        public CrossPredictedXResidualsEvaluator(
          int numberOfPoints,
          int[] spectralRegions,
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression analysis
          )
          : base(spectralRegions, numFactors, groupingStrategy, preprocessOptions, analysis)
        {
            _numberOfPoints = numberOfPoints;
        }

        public void EhCrossValidationWorker(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
        {
            MultivariateRegression.PreprocessForAnalysis(_preprocessOptions, _spectralRegions, XX, YY,
              out var meanX, out var scaleX, out var meanY, out var scaleY);

            _analysis.AnalyzeFromPreprocessed(XX, YY, _numFactors);
            _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

            MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions, XU, meanX, scaleX);
            IROMatrix<double> xResidual = _analysis.SpectralResidualsFromPreprocessed(XU, _numFactors);

            if (_XCrossResiduals == null)
                _XCrossResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(_numberOfPoints, xResidual.ColumnCount);

            for (int i = 0; i < group.Length; i++)
                MatrixMath.SetRow(xResidual, i, _XCrossResiduals, group[i]);
        }
    }

    public class CrossValidationResultEvaluator : CrossValidationWorker
    {
        private CrossValidationResult _result;

        protected IMatrix<double> _predictedY;
        protected IMatrix<double> _spectralResidual;

        public CrossValidationResultEvaluator(
          int[] spectralRegions,
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression analysis,
          CrossValidationResult result
          )
          : base(spectralRegions, numFactors, groupingStrategy, preprocessOptions, analysis)
        {
            _result = result;
        }

        public void EhCrossValidationWorker(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU)
        {
            MultivariateRegression.PreprocessForAnalysis(_preprocessOptions, _spectralRegions, XX, YY,
              out var meanX, out var scaleX, out var meanY, out var scaleY);

            _analysis.AnalyzeFromPreprocessed(XX, YY, _numFactors);
            _numFactors = Math.Min(_numFactors, _analysis.NumberOfFactors);

            MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions, XU, meanX, scaleX);

            if (_predictedY == null || _predictedY.RowCount != YU.RowCount || _predictedY.ColumnCount != YU.ColumnCount)
                _predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(YU.RowCount, YU.ColumnCount);
            if (_spectralResidual == null || _spectralResidual.RowCount != XU.RowCount || _spectralResidual.ColumnCount != _analysis.NumberOfSpectralResiduals)
                _spectralResidual = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, _analysis.NumberOfSpectralResiduals);

            for (int nFactor = 0; nFactor <= _numFactors; nFactor++)
            {
                _analysis.PredictedYAndSpectralResidualsFromPreprocessed(XU, nFactor, _predictedY, _spectralResidual);
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
