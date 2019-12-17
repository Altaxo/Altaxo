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
    /// Contains method common for all multivariate regressions.
    /// </summary>
    /// <remarks>
    /// <para>Explanation of variables common to all methods:</para>
    /// <para>NumberOfPoints: Number of measurements for calibration. Each measurement is represented by a spectrum X (or set of independent variables) and a set of corresponding concentrations Y (or dependent variables)</para>
    /// <para>NumberOfX: Number of spectral values (or number of independent variables).</para>
    /// <para>NumberOfY: Number of concentrations (or number of dependent variables).</para>
    /// <para>NumberOfFactors: Number of main components used for prediction or calculation.</para>
    /// <para>X: Matrix of spectra( or independent variables). The spectra are horizontal oriented, i.e. one spectra is a row in the X matrix.</para>
    /// <para>Y: Matrix of concentration (or dependent variables). One set of concentrations is also represented by one row in the matrix.</para>
    /// <para>XU: Matrix of unknown spectra (or independent variables) used for prediction of the Y variables.</para>
    /// <para>SpectralRegions: If the spectra consists of more than one regions, these regions should be preprocessed separately. To designate them, one has
    /// to provide an array of ascending integer values. Each element of this region designates the starting index of a spectral region.</para>
    ///
    ///</remarks>
    public abstract class MultivariateRegression
    {
        #region Abstract members

        /// <summary>
        /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
        /// </summary>
        /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
        /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
        /// <param name="maxFactors">Maximum number of factors for analysis.</param>
        /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
        protected abstract void AnalyzeFromPreprocessedWithoutReset(IROMatrix<double> matrixX, IROMatrix<double> matrixY, int maxFactors);

        /// <summary>
        /// This calculates the spectral residuals.
        /// </summary>
        /// <param name="XU">Spectra (horizontally oriented).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="predictedY">On return, holds the predicted y values. (They are centered).</param>
        /// <param name="spectralResiduals">On return, holds the spectral residual values.</param>
        public abstract void PredictedYAndSpectralResidualsFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors, // number of factors to use for prediction
          IMatrix<double> predictedY,
          IMatrix<double> spectralResiduals // Matrix of spectral residuals, n rows x 1 column
          );

        /// <summary>
        /// Calculates the prediction scores (for use with the preprocessed spectra).
        /// </summary>
        /// <param name="numFactors">Number of factors used to calculate the prediction scores.</param>
        /// <param name="predictionScores">Supplied matrix for holding the prediction scores.</param>
        protected abstract void InternalGetPredictionScores(int numFactors, IMatrix<double> predictionScores);

        /// <summary>
        /// Calculates the spectral leverage values from the preprocessed spectra.
        /// </summary>
        /// <param name="matrixX">Matrix of preprocessed spectra (number of observations, number of X).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="xLeverage">Resulting spectral leverages. This matrix must have the dimensions (number of observations, number of leverage data). The number of leverage data is
        /// normally one, with the exception of the PLS1 analysis, where it is equal to the number of Y.</param>
        protected abstract void InternalGetXLeverageFromPreprocessed(IROMatrix<double> matrixX, int numFactors, IMatrix<double> xLeverage);

        /// <summary>
        /// This sets the calibration model data of the analysis to the provided data. This can be used to set back previously stored
        /// calibration data for use in the prediction functions.
        /// </summary>
        /// <param name="calib">The calibration set for use in the analysis. The provided calibration model have to correspond to
        /// the type of analysis.</param>
        public abstract void SetCalibrationModel(IMultivariateCalibrationModel calib);

        /// <summary>
        /// Returns the predicted error sum of squares (PRESS) for this analysis. The length of the vector returned
        /// is the number of factors in the analysis plus one.
        /// </summary>
        /// <param name="matrixX">The preprocessed spectra.</param>
        /// <returns>The PRESS vector.</returns>
        public abstract IROVector<double> GetPRESSFromPreprocessed(IROMatrix<double> matrixX);

        #endregion Abstract members

        #region Properties and helpers

        /// <summary>
        /// Returns the calibration model of the analysis.
        /// </summary>
        protected abstract MultivariateCalibrationModel InternalCalibrationModel { get; }

        /// <summary>
        /// Returns the calibration model of the analysis.
        /// </summary>
        public virtual IMultivariateCalibrationModel CalibrationModel
        {
            get { return InternalCalibrationModel; }
        }

        /// <summary>
        /// Returns the number of factors calculated during the analysis.
        /// </summary>
        public int NumberOfFactors { get { return InternalCalibrationModel.NumberOfFactors; } }

        /// <summary>
        /// This returns the number of spectral residuals. This is normally 1, but for the PLS1 analyis, it is the NumberOfY.
        /// </summary>
        public virtual int NumberOfSpectralResiduals { get { return 1; } }

        /// <summary>
        /// Resets the regression, so that it appears like newly created.
        /// </summary>
        public virtual void Reset()
        {
            InternalCalibrationModel.NumberOfFactors = 0;
            InternalCalibrationModel.SetPreprocessingModel(null);
        }

        #endregion Properties and helpers

        #region Prediction from preprocessed

        /// <summary>
        /// This predicts concentrations of unknown spectra.
        /// </summary>
        /// <param name="XU">Matrix of unknown spectra (preprocessed the same way as the calibration spectra).</param>
        /// <param name="numFactors">Number of factors used for prediction.</param>
        /// <param name="predictedY">On return, holds the predicted y values. (They are centered).</param>
        public virtual void PredictYFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors, // number of factors to use for prediction
          IMatrix<double> predictedY // Matrix of predicted y-values, must be same number of rows as spectra
          )
        {
            PredictedYAndSpectralResidualsFromPreprocessed(XU, numFactors, predictedY, null);
        }

        /// <summary>
        /// This predicts concentrations of unknown spectra.
        /// </summary>
        /// <param name="XU">Matrix of unknown spectra (preprocessed the same way as the calibration spectra).</param>
        /// <param name="numFactors">Number of factors used for prediction.</param>
        /// <returns>The predicted y values. (They are centered).</returns>
        public virtual IROMatrix<double> PredictYFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors // number of factors to use for prediction
          )
        {
            var predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, InternalCalibrationModel.NumberOfY);
            PredictedYAndSpectralResidualsFromPreprocessed(XU, numFactors, predictedY, null);
            return predictedY;
        }

        /// <summary>
        /// This calculates the spectral residuals.
        /// </summary>
        /// <param name="XU">Spectra (horizontally oriented).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="spectralResiduals">On return, holds the spectral residual values.</param>
        public virtual void SpectralResidualsFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors, // number of factors to use for prediction
          IMatrix<double> spectralResiduals // Matrix of spectral residuals, n rows x 1 column
          )
        {
            PredictedYAndSpectralResidualsFromPreprocessed(XU, numFactors, null, spectralResiduals);
        }

        /// <summary>
        /// This calculates the spectral residuals.
        /// </summary>
        /// <param name="XU">Spectra (horizontally oriented).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <returns>The calculated spectral residuals.</returns>
        public virtual IROMatrix<double> SpectralResidualsFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors // number of factors to use for prediction
          )
        {
            var result = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, NumberOfSpectralResiduals);
            SpectralResidualsFromPreprocessed(XU, numFactors, result);
            return result;
        }

        /// <summary>
        /// This calculates the spectral residuals. The matrices are reallocated if they don't have the appropriate dimensions.
        /// </summary>
        /// <param name="XU">Spectra (horizontally oriented).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="predictedY">On return, holds the predicted y values. (They are centered). If the matrix you provide has not the appropriate dimensions, it is reallocated.</param>
        /// <param name="spectralResiduals">On return, holds the spectral residual values.  If the matrix you provide has not the appropriate dimensions, it is reallocated.</param>
        public virtual void PredictedYAndSpectralResidualsFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors, // number of factors to use for prediction
          ref IMatrix<double> predictedY,
          ref IMatrix<double> spectralResiduals // Matrix of spectral residuals, n rows x 1 column
          )
        {
            // check the dimensions of the matrices
            if (predictedY != null)
            {
                if (predictedY.RowCount != XU.RowCount || predictedY.ColumnCount != InternalCalibrationModel.NumberOfY)
                    predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, InternalCalibrationModel.NumberOfY);
            }

            if (spectralResiduals != null)
            {
                if (spectralResiduals.RowCount != XU.RowCount || spectralResiduals.ColumnCount != NumberOfSpectralResiduals)
                    spectralResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, NumberOfSpectralResiduals);
            }

            PredictedYAndSpectralResidualsFromPreprocessed(XU, numFactors, predictedY, spectralResiduals);
        }

        /// <summary>
        /// This calculates the spectral residuals. The matrices are reallocated if they don't have the appropriate dimensions.
        /// </summary>
        /// <param name="XU">Spectra (horizontally oriented).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="calculatePredictedY">If true, the predictedY is calculated. Otherwise, predictedY is null on return.</param>
        /// <param name="predictedY">On return, holds the predicted y values. (They are centered). If the matrix you provide has not the appropriate dimensions, it is reallocated.</param>
        /// <param name="calculateSpectralResiduals">If true, the spectral residuals are calculated. Otherwise spectralResiduals is null on return.</param>
        /// <param name="spectralResiduals">On return, holds the spectral residual values.  If the matrix you provide has not the appropriate dimensions, it is reallocated.</param>
        public virtual void PredictedYAndSpectralResidualsFromPreprocessed(
          IROMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors, // number of factors to use for prediction
          bool calculatePredictedY,
          out IMatrix<double> predictedY,
          bool calculateSpectralResiduals,
          out IMatrix<double> spectralResiduals // Matrix of spectral residuals, n rows x 1 column
          )
        {
            // check the dimensions of the matrices
            if (calculatePredictedY)
                predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, InternalCalibrationModel.NumberOfY);
            else
                predictedY = null;

            if (calculateSpectralResiduals)
                spectralResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XU.RowCount, NumberOfSpectralResiduals);
            else
                spectralResiduals = null;

            PredictedYAndSpectralResidualsFromPreprocessed(XU, numFactors, predictedY, spectralResiduals);
        }

        #endregion Prediction from preprocessed

        #region Analyze from preprocessed

        /// <summary>
        /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
        /// </summary>
        /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
        /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
        /// <param name="maxFactors">Maximum number of factors for analysis.</param>
        /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
        public void AnalyzeFromPreprocessed(IROMatrix<double> matrixX, IROMatrix<double> matrixY, int maxFactors)
        {
            Reset();

            InternalCalibrationModel.NumberOfX = matrixX.ColumnCount;
            InternalCalibrationModel.NumberOfY = matrixY.ColumnCount;

            AnalyzeFromPreprocessedWithoutReset(matrixX, matrixY, maxFactors);
        }

        #endregion Analyze from preprocessed

        #region AnalyzeFromRaw

        /// <summary>
        /// Creates an analyis from the raw spectra and raw concentrations.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="spectralRegions">Array of ascending indices representing the starting indices of spectral regions.</param>
        /// <param name="matrixX">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
        /// <param name="matrixY">Matrix of preprocessed y values (number of observations, number of y).</param>
        /// <param name="maxFactors">Maximum number of factors to calculate.</param>
        public void AnalyzeFromRaw(
          SpectralPreprocessingOptions preprocessOptions,
          int[] spectralRegions,
          IMatrix<double> matrixX,
          IMatrix<double> matrixY,
          int maxFactors)
        {
            Reset();
            InternalCalibrationModel.SetPreprocessingModel(PreprocessForAnalysis(preprocessOptions, spectralRegions, matrixX, matrixY));

            InternalCalibrationModel.NumberOfX = matrixX.ColumnCount;
            InternalCalibrationModel.NumberOfY = matrixY.ColumnCount;

            AnalyzeFromPreprocessedWithoutReset(matrixX, matrixY, maxFactors);
        }

        /// <summary>
        /// Creates an analyis from the raw spectra and raw concentrations.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="xOfX">The spectral x variable (e.g. frequencies, wavelength).</param>
        /// <param name="matrixX">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
        /// <param name="matrixY">Matrix of preprocessed y values (number of observations, number of y).</param>
        /// <param name="maxFactors">Maximum number of factors to calculate.</param>
        public void AnalyzeFromRaw(
          SpectralPreprocessingOptions preprocessOptions,
          IReadOnlyList<double> xOfX,
          IMatrix<double> matrixX,
          IMatrix<double> matrixY,
          int maxFactors)
        {
            AnalyzeFromRaw(preprocessOptions,
              SpectralPreprocessingOptions.IdentifyRegions(xOfX),
              matrixX,
              matrixY,
              maxFactors);
        }

        #endregion AnalyzeFromRaw

        #region Prediction from Raw

        /// <summary>
        /// Predicts y values from raw (unpreprocessed) spectra.
        /// </summary>
        /// <param name="XU">Matrix of spectra used for prediction (number of spectra, number of wavelengths).</param>
        /// <param name="numFactors">Number of factors used for prediction.</param>
        /// <param name="predictedY">In return, holds the predicted y values. You have to provide a matrix of
        /// dimensions (number of spectra, number of y).</param>
        public void PredictYFromRaw(
          IMatrix<double> XU, // unknown spectrum or spectra,  horizontal oriented
          int numFactors, // number of factors to use for prediction
          IMatrix<double> predictedY // Matrix of predicted y-values, must be same number of rows as spectra
          )
        {
            PreprocessSpectraForPrediction(InternalCalibrationModel.PreprocessingModel, XU);
            PredictYFromPreprocessed(XU, numFactors, predictedY);
            PostprocessY(InternalCalibrationModel.PreprocessingModel, predictedY);
        }

        #endregion Prediction from Raw

        #region Preprocessing helper functions

        /// <summary>
        /// Preprocesses the x and y matrices before usage in multivariate calibrations.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="spectralRegions">Array of ascending indices representing the starting indices of spectral regions.</param>
        /// <param name="matrixX"></param>
        /// <param name="matrixY"></param>
        /// <param name="meanX"></param>
        /// <param name="scaleX"></param>
        /// <param name="meanY"></param>
        /// <param name="scaleY"></param>
        public static void PreprocessForAnalysis(
          SpectralPreprocessingOptions preprocessOptions,
          int[] spectralRegions,
          IMatrix<double> matrixX,
          IMatrix<double> matrixY,
          out IVector<double> meanX, out IVector<double> scaleX,
          out IVector<double> meanY, out IVector<double> scaleY)
        {
            PreprocessSpectraForAnalysis(preprocessOptions, spectralRegions, matrixX, out meanX, out scaleX);

            PreprocessYForAnalysis(matrixY, out meanY, out scaleY);
        }

        /// <summary>
        /// Preprocesses the x and y matrices before usage in multivariate calibrations.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="xOfX"></param>
        /// <param name="matrixX"></param>
        /// <param name="matrixY"></param>
        /// <param name="meanX"></param>
        /// <param name="scaleX"></param>
        /// <param name="meanY"></param>
        /// <param name="scaleY"></param>
        public static void PreprocessForAnalysis(
          SpectralPreprocessingOptions preprocessOptions,
          IReadOnlyList<double> xOfX,
          IMatrix<double> matrixX,
          IMatrix<double> matrixY,
          out IVector<double> meanX, out IVector<double> scaleX,
          out IVector<double> meanY, out IVector<double> scaleY)
        {
            PreprocessSpectraForAnalysis(preprocessOptions, xOfX, matrixX, out meanX, out scaleX);

            PreprocessYForAnalysis(matrixY, out meanY, out scaleY);
        }

        /// <summary>
        /// Preprocesses the x and y matrices before usage in multivariate calibrations.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="spectralRegions">Array of ascending indices representing the starting indices of spectral regions.</param>
        /// <param name="matrixX">Matrix of spectra.</param>
        /// <param name="matrixY">Matrix of concentrations.</param>
        /// <returns>The collected data about proprocessing.</returns>
        public static MultivariatePreprocessingModel PreprocessForAnalysis(
          SpectralPreprocessingOptions preprocessOptions,
          int[] spectralRegions,
          IMatrix<double> matrixX,
          IMatrix<double> matrixY)
        {
            var data = new MultivariatePreprocessingModel
            {
                PreprocessOptions = (SpectralPreprocessingOptions)preprocessOptions.Clone(),

                SpectralRegions = spectralRegions
            };
            PreprocessSpectraForAnalysis(preprocessOptions, spectralRegions, matrixX, out var meanX, out var scaleX);

            data.XMean = meanX;
            data.XScale = scaleX;
            PreprocessYForAnalysis(matrixY, out var meanY, out var scaleY);

            data.YMean = meanY;
            data.YScale = scaleY;

            return data;
        }

        /// <summary>
        /// Preprocesses the x and y matrices before usage in multivariate calibrations.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="xOfX">Spectral wavelength values.</param>
        /// <param name="matrixX">Matrix of spectra.</param>
        /// <param name="matrixY">Matrix of concentrations.</param>
        /// <returns>The collected data about proprocessing.</returns>
        public static MultivariatePreprocessingModel PreprocessForAnalysis(
          SpectralPreprocessingOptions preprocessOptions,
          IReadOnlyList<double> xOfX,
          IMatrix<double> matrixX,
          IMatrix<double> matrixY)
        {
            MultivariatePreprocessingModel result = PreprocessForAnalysis(
              preprocessOptions,
              SpectralPreprocessingOptions.IdentifyRegions(xOfX),
              matrixX,
              matrixY);

            result.XOfX = xOfX;

            return result;
        }

        /// <summary>
        /// This will process the spectra before analysis in multivariate calibration.
        /// </summary>
        /// <param name="preprocessOptions">Contains the information how to preprocess the spectra.</param>
        /// <param name="spectralRegions">Array of starting indices of spectral regions. Can be set to null.</param>
        /// <param name="matrixX">The matrix of spectra. Each spectrum is a row of the matrix.</param>
        /// <param name="meanX"></param>
        /// <param name="scaleX"></param>
        public static void PreprocessSpectraForAnalysis(
          SpectralPreprocessingOptions preprocessOptions,
          int[] spectralRegions,
          IMatrix<double> matrixX,
          out IVector<double> meanX, out IVector<double> scaleX
          )
        {
            // Before we can apply PLS, we have to center the x and y matrices
            meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
            scaleX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
            //  MatrixMath.HorizontalVector scaleX = new MatrixMath.HorizontalVector(matrixX.Cols);

            preprocessOptions.SetRegions(spectralRegions);
            preprocessOptions.Process(matrixX, meanX, scaleX);
        }

        /// <summary>
        /// This will process the spectra before analysis in multivariate calibration.
        /// </summary>
        /// <param name="preprocessOptions">Contains the information how to preprocess the spectra.</param>
        /// <param name="xOfX"></param>
        /// <param name="matrixX">The matrix of spectra. Each spectrum is a row of the matrix.</param>
        /// <param name="meanX"></param>
        /// <param name="scaleX"></param>
        public static void PreprocessSpectraForAnalysis(
          SpectralPreprocessingOptions preprocessOptions,
          IReadOnlyList<double> xOfX,
          IMatrix<double> matrixX,
          out IVector<double> meanX, out IVector<double> scaleX
          )
        {
            // Before we can apply PLS, we have to center the x and y matrices
            meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
            scaleX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
            //  MatrixMath.HorizontalVector scaleX = new MatrixMath.HorizontalVector(matrixX.Cols);

            preprocessOptions.SetRegionsByIdentification(xOfX);
            preprocessOptions.Process(matrixX, meanX, scaleX);
        }

        /// <summary>
        /// This will convert the raw spectra (horizontally in matrixX) to preprocessed spectra according to the calibration model.
        /// </summary>
        /// <param name="calib">The calibration model containing the instructions to process the spectra.</param>
        /// <param name="preprocessOptions">Contains the information how to preprocess the spectra.</param>
        /// <param name="matrixX">The matrix of spectra. Each spectrum is a row of the matrix.</param>
        public static void PreprocessSpectraForPrediction(
          IMultivariateCalibrationModel calib,
          SpectralPreprocessingOptions preprocessOptions,
          IMatrix<double> matrixX)
        {
            preprocessOptions.ProcessForPrediction(matrixX, calib.PreprocessingModel.XMean, calib.PreprocessingModel.XScale);
        }

        /// <summary>
        /// Preprocess the raw spectra for prediction (use the preprocess data from the analysis).
        /// </summary>
        /// <param name="calib">The preprocessing data.</param>
        /// <param name="matrixX">Matrix of raw spectra. On return, contains the preprocessed spectra.</param>
        public static void PreprocessSpectraForPrediction(
          IMultivariatePreprocessingModel calib,
          IMatrix<double> matrixX)
        {
            calib.PreprocessOptions.ProcessForPrediction(matrixX, calib.XMean, calib.XScale);
        }

        /// <summary>
        /// This will convert the raw spectra (horizontally in matrixX) to preprocessed spectra according to the calibration model.
        /// </summary>
        /// <param name="preprocessOptions">Information how to preprocess the spectra.</param>
        /// <param name="matrixX">Matrix of raw spectra. On return, this matrix contains the preprocessed spectra.</param>
        /// <param name="meanX">Mean spectrum.</param>
        /// <param name="scaleX">Scale spectrum.</param>
        public static void PreprocessSpectraForPrediction(
          SpectralPreprocessingOptions preprocessOptions,
          IMatrix<double> matrixX,
          IReadOnlyList<double> meanX,
          IReadOnlyList<double> scaleX)
        {
            preprocessOptions.ProcessForPrediction(matrixX, meanX, scaleX);
        }

        /// <summary>
        /// Preprocess the y values for analysis (mean center, scale currently not used).
        /// </summary>
        /// <param name="matrixY">Matrix of y values. On return, this contains the preprocessed y values.</param>
        /// <param name="meanY">On return, contains the mean y value(s).</param>
        /// <param name="scaleY">On return, contains the scale value(s).</param>
        public static void PreprocessYForAnalysis(IMatrix<double> matrixY,
          out IVector<double> meanY, out IVector<double> scaleY)
        {
            meanY = new MatrixMath.MatrixWithOneRow<double>(matrixY.ColumnCount);
            scaleY = new MatrixMath.MatrixWithOneRow<double>(matrixY.ColumnCount);
            VectorMath.FillWith(scaleY, 1);
            MatrixMath.ColumnsToZeroMean(matrixY, meanY);
        }

        /// <summary>
        /// This centers and scales the y values in exactly the way it was done in the multivariate analysis.
        /// </summary>
        /// <param name="matrixY">The concentration matrix. Constituents are horizontally oriented, different experiments vertically.</param>
        /// <param name="meanY">Vector of concentration mean values.</param>
        /// <param name="scaleY">Vector of concentration scale values.</param>
        public static void PreprocessYForPrediction(
          IMatrix<double> matrixY,
          IReadOnlyList<double> meanY,
          IReadOnlyList<double> scaleY)
        {
            for (int j = 0; j < matrixY.ColumnCount; j++)
            {
                for (int i = 0; i < matrixY.RowCount; i++)
                    matrixY[i, j] = (matrixY[i, j] - meanY[j]) * scaleY[j];
            }
        }

        /// <summary>
        /// This calculates from the predicted (but still centered) y values the raw y values.
        /// </summary>
        /// <param name="matrixY">Matrix of (centered) y values. On return, contains the uncentered y values.</param>
        /// <param name="meanY">Vector of mean y value(s).</param>
        /// <param name="scaleY">Vector of y scale value(s).</param>
        public static void PostprocessY(
          IMatrix<double> matrixY,
          IReadOnlyList<double> meanY,
          IReadOnlyList<double> scaleY)
        {
            for (int j = 0; j < matrixY.ColumnCount; j++)
            {
                for (int i = 0; i < matrixY.RowCount; i++)
                    matrixY[i, j] = (matrixY[i, j] / scaleY[j]) + meanY[j];
            }
        }

        /// <summary>
        /// This calculates from the predicted (but still centered) y values the raw y values.
        /// </summary>
        /// <param name="matrixY">Matrix of (centered) y values. On return, contains the uncentered y values.</param>
        /// <param name="calib">Contains the calibration data (mean y and scale y).</param>
        public static void PostprocessY(IMultivariatePreprocessingModel calib, IMatrix<double> matrixY)
        {
            PostprocessY(matrixY, calib.YMean, calib.YScale);
        }

        #endregion Preprocessing helper functions

        #region Cross validation helper functions

        /// <summary>
        /// Function used for cross validation iteration. During cross validation, the original spectral matrix is separated into
        /// a spectral group used for prediction and the remaining calibration spectra. Parameters here:
        /// XX: remaining calibration spectra.
        /// YY: corresponding concentration data.
        /// XU: spectra used for prediction.
        /// YU: corresponding concentration data.
        /// </summary>
        public delegate void CrossValidationIterationFunction(int[] group, IMatrix<double> XX, IMatrix<double> YY, IMatrix<double> XU, IMatrix<double> YU);

        /// <summary>
        /// This function separates the spectra into a bunch of spectra used for calibration and the rest of spectra
        /// used for prediction. This separation is repeated until all spectra are used exactly one time for prediction.
        /// </summary>
        /// <param name="X">Matrix of spectra (horizontal oriented).</param>
        /// <param name="Y">Matrix of y values.</param>
        /// <param name="groupingStrategy">The strategy how to separate the spectra into the calibration and prediction spectra.</param>
        /// <param name="crossFunction">The function that is called for each separation.</param>
        /// <returns>The mean number of spectra that was used for prediction.</returns>
        public static double CrossValidationIteration(
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          ICrossValidationGroupingStrategy groupingStrategy,
          CrossValidationIterationFunction crossFunction
          )
        {
            //      int[][] groups = bExcludeGroups ? new ExcludeGroupsGroupingStrategy().Group(Y) : new ExcludeSingleMeasurementsGroupingStrategy().Group(Y);
            int[][] groups = groupingStrategy.Group(Y);

            IMatrix<double> XX = null;
            IMatrix<double> YY = null;
            IMatrix<double> XU = null;
            IMatrix<double> YU = null;

            for (int nGroup = 0, prevNumExcludedSpectra = int.MinValue; nGroup < groups.Length; nGroup++)
            {
                int[] spectralGroup = groups[nGroup];
                int numberOfExcludedSpectraOfGroup = spectralGroup.Length;

                if (prevNumExcludedSpectra != numberOfExcludedSpectraOfGroup)
                {
                    XX = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(X.RowCount - numberOfExcludedSpectraOfGroup, X.ColumnCount);
                    YY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(Y.RowCount - numberOfExcludedSpectraOfGroup, Y.ColumnCount);
                    XU = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfExcludedSpectraOfGroup, X.ColumnCount);
                    YU = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfExcludedSpectraOfGroup, Y.ColumnCount);
                    prevNumExcludedSpectra = numberOfExcludedSpectraOfGroup;
                }

                // build a new x and y matrix with the group information
                // fill XX and YY with values
                for (int i = 0, j = 0; i < X.RowCount; i++)
                {
                    if (Array.IndexOf(spectralGroup, i) >= 0) // if spectral group contains i
                        continue; // Exclude this row from the spectra
                    MatrixMath.SetRow(X, i, XX, j);
                    MatrixMath.SetRow(Y, i, YY, j);
                    j++;
                }

                // fill XU (unknown spectra) with values
                for (int i = 0; i < spectralGroup.Length; i++)
                {
                    int j = spectralGroup[i];
                    MatrixMath.SetRow(X, j, XU, i); // x-unkown (unknown spectra)
                    MatrixMath.SetRow(Y, j, YU, i); // y-unkown (unknown concentration)
                }

                // now do the analysis
                crossFunction(spectralGroup, XX, YY, XU, YU);
            } // for all groups

            // calculate the mean number of excluded spectras
            return ((double)X.RowCount) / groups.Length;
        }

        #endregion Cross validation helper functions

        #region Cross validation functions

        /// <summary>
        /// Get the cross predicted error sum of squares for the number of factors=0...numFactors.
        /// </summary>
        /// <param name="spectralRegions">Array of ascending indices representing the starting indices of spectral regions.</param>
        /// <param name="X">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="Y">Matrix of y values (e.g. concentrations).</param>
        /// <param name="numFactors">Maximum number of factors to calculate the cross PRESS for.</param>
        /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
        /// <param name="crossPRESS">The vector of CROSS press values. Note that this vector has the length numFactor+1.</param>
        /// <returns>The mean number of spectra used for prediction.</returns>
        public static double GetCrossPRESS(
          int[] spectralRegions,
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression regress,
          out IROVector<double> crossPRESS // vertical value of PRESS values for the cross validation
          )
        {
            var worker = new CrossPRESSEvaluator(spectralRegions, numFactors, groupingStrategy, preprocessOptions, regress);
            double result = CrossValidationIteration(X, Y, groupingStrategy, new CrossValidationIterationFunction(worker.EhCrossPRESS));

            crossPRESS = VectorMath.ToROVector(worker.CrossPRESS, worker.NumberOfFactors + 1);

            return result;
        }

        /// <summary>
        /// Get the cross predicted error sum of squares for the number of factors=0...numFactors.
        /// </summary>
        /// <param name="xOfX">The spectral wavelength values corresponding to the spectral bins.</param>
        /// <param name="X">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="Y">Matrix of y values (e.g. concentrations).</param>
        /// <param name="numFactors">Maximum number of factors to calculate the cross PRESS for.</param>
        /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
        /// <param name="crossPRESS">The vector of CROSS press values. Note that this vector has the length numFactor+1.</param>
        /// <returns>The mean number of spectra used for prediction.</returns>
        public static double GetCrossPRESS(
          IReadOnlyList<double> xOfX,
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression regress,
          out IROVector<double> crossPRESS // vertical value of PRESS values for the cross validation
          )
        {
            return GetCrossPRESS(SpectralPreprocessingOptions.IdentifyRegions(xOfX),
              X,
              Y,
              numFactors,
              groupingStrategy,
              preprocessOptions,
              regress,
              out crossPRESS);
        }

        /// <summary>
        /// Calculates the cross predicted y values.
        /// </summary>
        /// <param name="spectralRegions">Array of ascending indices representing the starting indices of spectral regions.</param>
        /// <param name="X">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="Y">Matrix of y values (e.g. concentrations).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
        /// <param name="yCrossPredicted">Matrix of cross predicted y values. Must be of same dimension as the Y matrix.</param>
        /// <returns>Mean number of spectra used for cross prediction.</returns>
        public static double GetCrossYPredicted(
          int[] spectralRegions,
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression regress,

          IMatrix<double> yCrossPredicted // vertical value of PRESS values for the cross validation
          )
        {
            var worker = new CrossPredictedYEvaluator(spectralRegions, numFactors, groupingStrategy, preprocessOptions, regress, yCrossPredicted);
            double result = CrossValidationIteration(X, Y, groupingStrategy, new CrossValidationIterationFunction(worker.EhYCrossPredicted));

            return result;
        }

        /// <summary>
        /// Calculates the cross predicted y values.
        /// </summary>
        /// <param name="xOfX">The spectral wavelength values corresponding to the spectral bins.</param>
        /// <param name="X">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="Y">Matrix of y values (e.g. concentrations).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
        /// <param name="yCrossPredicted">Matrix of cross predicted y values. Must be of same dimension as the Y matrix.</param>
        /// <returns>Mean number of spectra used for cross prediction.</returns>
        public static double GetCrossYPredicted(
          IReadOnlyList<double> xOfX,
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression regress,

          IMatrix<double> yCrossPredicted // vertical value of PRESS values for the cross validation
          )
        {
            return GetCrossYPredicted(
              SpectralPreprocessingOptions.IdentifyRegions(xOfX),
              X, // matrix of spectra (a spectra is a row of this matrix)
              Y, // matrix of concentrations (a mixture is a row of this matrix)
              numFactors,
              groupingStrategy,
              preprocessOptions,
              regress,
              yCrossPredicted);
        }

        /// <summary>
        /// Calculates the spectral residuals obtained from cross validation.
        /// </summary>
        /// <param name="spectralRegions">Array of ascending indices representing the starting indices of spectral regions.</param>
        /// <param name="X">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="Y">Matrix of y values (e.g. concentrations).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
        /// <param name="crossXResiduals">Returns the matrix of spectral residuals</param>
        /// <returns>Mean number of spectra used for prediction.</returns>
        public static double GetCrossXResiduals(
          int[] spectralRegions,
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression regress,

          out IROMatrix<double> crossXResiduals
          )
        {
            var worker = new CrossPredictedXResidualsEvaluator(X.RowCount, spectralRegions, numFactors, groupingStrategy, preprocessOptions, regress);
            double result = CrossValidationIteration(X, Y, groupingStrategy, new CrossValidationIterationFunction(worker.EhCrossValidationWorker));
            crossXResiduals = worker.XCrossResiduals;
            return result;
        }

        /// <summary>
        /// Calculates the spectral residuals obtained from cross validation.
        /// </summary>
        /// <param name="xOfX">The spectral wavelength values corresponding to the spectral bins.</param>
        /// <param name="X">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="Y">Matrix of y values (e.g. concentrations).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
        /// <param name="preprocessOptions">Information how to preprocess the data.</param>
        /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
        /// <param name="crossXResiduals">Returns the matrix of spectral residuals</param>
        /// <returns>Mean number of spectra used for prediction.</returns>
        public static double GetCrossXResiduals(
          IReadOnlyList<double> xOfX,
          IROMatrix<double> X, // matrix of spectra (a spectra is a row of this matrix)
          IROMatrix<double> Y, // matrix of concentrations (a mixture is a row of this matrix)
          int numFactors,
          ICrossValidationGroupingStrategy groupingStrategy,
          SpectralPreprocessingOptions preprocessOptions,
          MultivariateRegression regress,

          out IROMatrix<double> crossXResiduals
          )
        {
            return GetCrossXResiduals(
              SpectralPreprocessingOptions.IdentifyRegions(xOfX),
              X, // matrix of spectra (a spectra is a row of this matrix)
              Y, // matrix of concentrations (a mixture is a row of this matrix)
              numFactors,
              groupingStrategy,
              preprocessOptions,
              regress,

              out crossXResiduals);
        }

        #endregion Cross validation functions

        /// <summary>
        /// Calculates the prediction scores.
        /// </summary>
        /// <param name="numberOfFactors">Number of factors used for calculation.</param>
        /// <returns>The prediction score matrix. This matrix has the dimensions (NumberOfX, NumberOfY).</returns>
        public virtual IROMatrix<double> GetPredictionScores(int numberOfFactors)
        {
            var result = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(InternalCalibrationModel.NumberOfX, InternalCalibrationModel.NumberOfY);
            InternalGetPredictionScores(numberOfFactors, result);
            return result;
        }

        /// <summary>
        /// Calculates the spectral leverage from preprocessed spectra.
        /// </summary>
        /// <param name="matrixX">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <returns>Matrix of spectral leverages. Normally, this is a (NumberOfPoints,1) matrix, with exception of PLS1, where it is a (NumberOfPoints,NumberOfY) matrix.</returns>
        public virtual IROMatrix<double> GetXLeverageFromPreprocessed(IROMatrix<double> matrixX, int numFactors)
        {
            var result = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixX.RowCount, 1);
            InternalGetXLeverageFromPreprocessed(matrixX, numFactors, result);
            return result;
        }

        /// <summary>
        /// Calculates the spectral leverage from raw spectra.
        /// </summary>
        /// <param name="preprocessOptions"></param>
        /// <param name="matrixX">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <returns>Matrix of spectral leverages. Normally, this is a (NumberOfPoints,1) matrix, with exception of PLS1, where it is a (NumberOfPoints,NumberOfY) matrix.</returns>
        public virtual IROMatrix<double> GetXLeverageFromRaw(SpectralPreprocessingOptions preprocessOptions, IMatrix<double> matrixX, int numFactors)
        {
            MultivariateRegression.PreprocessSpectraForPrediction(InternalCalibrationModel, preprocessOptions, matrixX);
            return GetXLeverageFromPreprocessed(matrixX, numFactors);
        }

        /// <summary>
        /// Calculates the spectral leverage from raw spectra.
        /// </summary>
        /// <param name="matrixX">Matrix of spectra (a spectrum = a row in the matrix).</param>
        /// <param name="numFactors">Number of factors used for calculation.</param>
        /// <returns>Matrix of spectral leverages. Normally, this is a (NumberOfPoints,1) matrix, with exception of PLS1, where it is a (NumberOfPoints,NumberOfY) matrix.</returns>
        public virtual IROMatrix<double> GetXLeverageFromRaw(IMatrix<double> matrixX, int numFactors)
        {
            return GetXLeverageFromRaw(InternalCalibrationModel.PreprocessingModel.PreprocessOptions, matrixX, numFactors);
        }
    }
}
