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
    /// <param name="matrixXPre">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixYPre">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maximalNumberOfFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    protected abstract void AnalyzeFromPreprocessedWithoutReset(IROMatrix<double> matrixXPre, IROMatrix<double> matrixYPre, int maximalNumberOfFactors);

    /// <summary>
    /// This calculates the spectral residuals.
    /// </summary>
    /// <param name="XUPre">Spectra (horizontally oriented).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <param name="predictedY">On return, holds the predicted y values. (They are centered).</param>
    /// <param name="spectralResiduals">On return, holds the spectral residual values.</param>
    public abstract void PredictedYAndSpectralResidualsFromPreprocessed(
      IROMatrix<double> XUPre, // unknown spectrum or spectra,  horizontal oriented
      int numberOfFactors, // number of factors to use for prediction
      IMatrix<double>? predictedY,
      IMatrix<double>? spectralResiduals // Matrix of spectral residuals, n rows x 1 column
      );

    /// <summary>
    /// Calculates the prediction scores (for use with the preprocessed spectra).
    /// </summary>
    /// <param name="numberOfFactors">Number of factors used to calculate the prediction scores.</param>
    /// <param name="predictionScores">Supplied matrix for holding the prediction scores.</param>
    protected abstract void InternalGetPredictionScores(int numberOfFactors, IMatrix<double> predictionScores);

    /// <summary>
    /// Calculates the spectral leverage values from the preprocessed spectra.
    /// </summary>
    /// <param name="matrixXPre">Matrix of preprocessed spectra (number of observations, number of X).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <param name="xLeverage">Resulting spectral leverages. This matrix must have the dimensions (number of observations, number of leverage data). The number of leverage data is
    /// normally one, with the exception of the PLS1 analysis, where it is equal to the number of Y.</param>
    protected abstract void InternalGetXLeverageFromPreprocessed(IROMatrix<double> matrixXPre, int numberOfFactors, IMatrix<double> xLeverage);

    /// <summary>
    /// This sets the calibration model data of the analysis to the provided data. This can be used to set back previously stored
    /// calibration data for use in the prediction functions.
    /// </summary>
    /// <param name="calibrationModel">The calibration set for use in the analysis. The provided calibration model have to correspond to
    /// the type of analysis.</param>
    public abstract void SetCalibrationModel(IMultivariateCalibrationModel calibrationModel);

    /// <summary>
    /// Returns the predicted error sum of squares (PRESS) for this analysis. The length of the vector returned
    /// is the number of factors in the analysis plus one.
    /// </summary>
    /// <param name="matrixXPre">The preprocessed spectra.</param>
    /// <returns>The PRESS vector.</returns>
    public abstract IReadOnlyList<double> GetPRESSFromPreprocessed(IROMatrix<double> matrixXPre);

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
      InternalCalibrationModel.Reset();
    }

    #endregion Properties and helpers

    #region Preprocessing helper functions

    /// <summary>
    /// Preprocesses the spectra. This is the common first part, without ensemble processing.
    /// </summary>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the data (each spectrum separately).</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the data (all spectra as an ensemble)).</param>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency etc.) of the spectra (needed for preprocessing).</param>
    /// <param name="matrixXRaw">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
    /// <param name="regionsPre">On return, contains the regions (detected).</param>
    /// <param name="xOfXPre">On return, contains the x-values of the preprocessed spectra.</param>
    /// <param name="matrixXPre">On return, contains the matrix of preprocessed spectra (each spectrum is a row in the matrix).</param>
    private static void PreprocessSpectraFirstPart(
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      double[] xOfXRaw, IROMatrix<double> matrixXRaw,
      out int[] regionsPre, out double[] xOfXPre, out IMatrix<double> matrixXPre)
    {
      var srcSpectrum = new double[matrixXRaw.ColumnCount];
      double[] yResult;

      matrixXPre = null!;
      regionsPre = null!;
      xOfXPre = null!;

      // Preprocess the spectra, that first each spectrum separately
      for (int r = 0; r < matrixXRaw.RowCount; ++r)
      {
        MatrixMath.CopyRow(matrixXRaw, r, srcSpectrum);
        (xOfXPre, yResult, regionsPre) = preprocessSingleSpectrum.Execute(xOfXRaw, srcSpectrum, null);

        if (r == 0) // after the first spectrum is processed, allocate the new result matrices with the appropriate dimensions
        {
          matrixXPre = CreateMatrix.Dense<double>(matrixXRaw.RowCount, xOfXPre.Length);
        }
        else
        {
          if (matrixXPre.ColumnCount != yResult.Length)
            throw new InvalidProgramException("The single spectrum preprocessor outputs different length when processing different spectra. This is a programming error.");
        }

        MatrixMath.SetRow(matrixXPre, r, yResult);
      }
    }

    /// <summary>
    /// Preprocesses the spectra. This is the common first part, without ensemble processing.
    /// </summary>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the spectra, here: how to process each spectrum separately.</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the spectra, here: how to process the spectra ensemble.</param>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency etc.) of the spectra (needed for preprocessing).</param>
    /// <param name="matrixXRaw">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
    /// <param name="regionsPre">On return, contains the regions (detected).</param>
    /// <param name="xOfXPre">On return, contains the x-values of the preprocessed spectra.</param>
    /// <param name="matrixXPre">On return, contains the matrix of preprocessed spectra (each spectrum is a row in the matrix).</param>
    /// <param name="meanX">On return, contains the mean spectrum (mean of all specta).</param>
    /// <param name="scaleX">On return, contains the scaling factor for each spectral slot.</param>
    public static void PreprocessSpectraForAnalysis(
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      double[] xOfXRaw, IROMatrix<double> matrixXRaw,
      out int[] regionsPre, out double[] xOfXPre, out IMatrix<double> matrixXPre,
      out IVector<double> meanX, out IVector<double> scaleX)
    {
      PreprocessSpectraFirstPart(preprocessSingleSpectrum, preprocessEnsembleOfSpectra, xOfXRaw, matrixXRaw,
        out regionsPre, out xOfXPre, out matrixXPre);

      meanX = CreateVector.Dense<double>(xOfXPre.Length);
      scaleX = CreateVector.Dense<double>(xOfXPre.Length);

      // Preprocess spectra, now preprocess them as ensemble, and calculate the mean spectrum and (optionally) a scaling vector
      preprocessEnsembleOfSpectra.Process(matrixXPre, regionsPre, meanX, scaleX);
    }

    /// <summary>
    /// Preprocesses the x and y matrices before usage in multivariate calibrations.
    /// </summary>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the spectra, here: how to process each spectrum separately.</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the spectra, here: how to process the spectra ensemble.</param>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency etc.) of the spectra (needed for preprocessing).</param>
    /// <param name="matrixXRaw">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
    /// <param name="meanX">Contains the mean spectrum, calculated during the analysis stage.</param>
    /// <param name="scaleX">Contains the scaling factor for each spectral slot, calculated during the analysis stage.</param>
    /// <param name="xOfXPre">On return, contains the x-values of the preprocessed spectra.</param>
    /// <param name="matrixXPre">On return, contains the matrix of preprocessed spectra (each spectrum is a row in the matrix).</param>
    public static void PreprocessSpectraForPrediction(
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      double[] xOfXRaw, IROMatrix<double> matrixXRaw,
      IReadOnlyList<double> meanX, IReadOnlyList<double> scaleX,
      out double[] xOfXPre, out IMatrix<double> matrixXPre)
    {
      PreprocessSpectraFirstPart(preprocessSingleSpectrum, preprocessEnsembleOfSpectra, xOfXRaw, matrixXRaw,
        out var resultRegions, out xOfXPre, out matrixXPre);

      // Ensemble preprocessing for prediction
      // Here the meanX and scaleX that were calculated in the analysis step are used.
      preprocessEnsembleOfSpectra.ProcessForPrediction(matrixXPre, resultRegions, meanX, scaleX);
    }

    /// <summary>
    /// This will convert the raw spectra (horizontally in matrixX) to preprocessed spectra according to the calibration model.
    /// </summary>
    /// <param name="calibrationModel">The calibration model containing the instructions to process the spectra.</param>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency etc.) of the spectra (needed for preprocessing).</param>
    /// <param name="matrixXRaw">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
    /// <param name="xOfXPre">On return, contains the x-values of the preprocessed spectra.</param>
    /// <param name="matrixXPre">On return, contains the matrix of preprocessed spectra (each spectrum is a row in the matrix).</param>
    public static void PreprocessSpectraForPrediction(
      IMultivariateCalibrationModel calibrationModel,
      double[] xOfXRaw,
      IROMatrix<double> matrixXRaw,
      out double[] xOfXPre, out IMatrix<double> matrixXPre)
    {
      PreprocessSpectraForPrediction(
        calibrationModel.PreprocessingModel.PreprocessSingleSpectrum,
        calibrationModel.PreprocessingModel.PreprocessEnsembleOfSpectra,
        xOfXRaw, matrixXRaw, calibrationModel.PreprocessingModel.XMean, calibrationModel.PreprocessingModel.XScale,
        out xOfXPre, out matrixXPre);
    }

    /// <summary>
    /// Preprocesses the x and y matrices before usage in multivariate calibrations.
    /// </summary>
    /// <param name="matrixYRaw">Matrix of target variables. Each measurement (belonging to a spectrum) represents one row.</param>
    /// <param name="matrixYPre">On return, contains the matrix of preprocessed target variables. Same dimensions as <paramref name="matrixYRaw"/>.</param>
    /// <param name="meanY">On return, contains the mean value of the target variables (mean of all measurements).</param>
    /// <param name="scaleY">On return, contains the scaling factor for the target variables.</param>
    public static void PreprocessTargetVariablesForAnalysis(
      IROMatrix<double> matrixYRaw,
      out IMatrix<double> matrixYPre,
      out IVector<double> meanY, out IVector<double> scaleY)
    {
      // Preprocess the target variables
      matrixYPre = CreateMatrix.Dense<double>(matrixYRaw.RowCount, matrixYRaw.ColumnCount);
      MatrixMath.Copy(matrixYRaw, matrixYPre);
      PreprocessTargetVariablesForAnalysisInline(matrixYPre, out meanY, out scaleY);
    }

    /// <summary>
    /// Preprocess the y values for analysis (mean center, scale currently not used).
    /// </summary>
    /// <param name="matrixYRaw_Pre">Matrix of target variables. On calling, each measurement (belonging to a spectrum) represents one row.
    /// On return, contains the matrix of preprocessed target variables. 
    /// </param>
    /// <param name="meanY">On return, contains the mean y value(s).</param>
    /// <param name="scaleY">On return, contains the scale value(s).</param>
    public static void PreprocessTargetVariablesForAnalysisInline(
      IMatrix<double> matrixYRaw_Pre,
      out IVector<double> meanY,
      out IVector<double> scaleY)
    {
      meanY = new MatrixMath.MatrixWithOneRow<double>(matrixYRaw_Pre.ColumnCount);
      scaleY = new MatrixMath.MatrixWithOneRow<double>(matrixYRaw_Pre.ColumnCount);
      VectorMath.FillWith(scaleY, 1);
      MatrixMath.ColumnsToZeroMean(matrixYRaw_Pre, meanY);
    }

    /// <summary>
    /// Preprocesses the x and y matrices before usage in multivariate calibrations.
    /// </summary>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the spectra, here: how to process each spectrum separately.</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the spectra, here: how to process the spectra ensemble.</param>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency etc.) of the spectra (needed for preprocessing).</param>
    /// <param name="matrixXRaw">Matrix of preprocessed spectra (number of observations, number of wavelengths).</param>
    /// <param name="meanX">Contains the mean spectrum, calculated during the analysis stage.</param>
    /// <param name="scaleX">Contains the scaling factor for each spectral slot, calculated during the analysis stage.</param>
    /// <param name="xOfXPre">On return, contains the x-values of the preprocessed spectra.</param>
    /// <param name="matrixXPre">On return, contains the matrix of preprocessed spectra (each spectrum is a row in the matrix).</param>
    /// <param name="matrixYRaw">Matrix of target variables. Each measurement (belonging to a spectrum) represents one row.</param>
    /// <param name="matrixYPre">On return, contains the matrix of preprocessed target variables. Same dimensions as <paramref name="matrixYRaw"/>.</param>
    /// <param name="meanY">On return, contains the mean value of the target variables (mean of all measurements).</param>
    /// <param name="scaleY">On return, contains the scaling factor for the target variables.</param>
    public static void PreprocessForAnalysis(
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      double[] xOfXRaw, IROMatrix<double> matrixXRaw, IROMatrix<double> matrixYRaw,
      out double[] xOfXPre, out IMatrix<double> matrixXPre, out IMatrix<double> matrixYPre,
      out IVector<double> meanX, out IVector<double> scaleX,
      out IVector<double> meanY, out IVector<double> scaleY)
    {
      PreprocessSpectraForAnalysis(
        preprocessSingleSpectrum, preprocessEnsembleOfSpectra,
        xOfXRaw, matrixXRaw,
        out var _, out xOfXPre, out matrixXPre,
        out meanX, out scaleX);

      PreprocessTargetVariablesForAnalysis(
        matrixYRaw, out matrixYPre,
        out meanY, out scaleY);
    }



    /// <summary>
    /// This calculates from the predicted (but still centered) y values the raw y values.
    /// </summary>
    /// <param name="matrixYPre_Raw">On call, contains the matrix of (centered) y values.
    /// On return, contains the uncentered y values.</param>
    /// <param name="meanY">Vector of mean y value(s).</param>
    /// <param name="scaleY">Vector of y scale value(s).</param>
    public static void PostprocessTargetVariablesInline(
      IMatrix<double> matrixYPre_Raw,
      System.Collections.Generic.IReadOnlyList<double> meanY,
      System.Collections.Generic.IReadOnlyList<double> scaleY)
    {
      for (int j = 0; j < matrixYPre_Raw.ColumnCount; j++)
      {
        for (int i = 0; i < matrixYPre_Raw.RowCount; i++)
          matrixYPre_Raw[i, j] = (matrixYPre_Raw[i, j] / scaleY[j]) + meanY[j];
      }
    }

    /// <summary>
    /// This calculates from the predicted (but still centered) y values the raw y values.
    /// </summary>
    /// <param name="matrixY">Matrix of (centered) y values. On return, contains the uncentered y values.</param>
    /// <param name="calib">Contains the calibration data (mean y and scale y).</param>
    public static void PostprocessTargetVariablesInline(IMultivariatePreprocessingModel calib, IMatrix<double> matrixY)
    {
      PostprocessTargetVariablesInline(matrixY, calib.YMean, calib.YScale);
    }

    #endregion Preprocessing helper functions

    #region Analyze from preprocessed

    /// <summary>
    /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
    /// </summary>
    /// <param name="matrixXPre">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixYPre">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maximalNumberOfFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    public void AnalyzeFromPreprocessed(IROMatrix<double> matrixXPre, IROMatrix<double> matrixYPre, int maximalNumberOfFactors)
    {
      Reset();

      InternalCalibrationModel.NumberOfX = matrixXPre.ColumnCount;
      InternalCalibrationModel.NumberOfY = matrixYPre.ColumnCount;

      AnalyzeFromPreprocessedWithoutReset(matrixXPre, matrixYPre, maximalNumberOfFactors);
    }

    #endregion Analyze from preprocessed

    #region Prediction from preprocessed

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
    /// This predicts concentrations of unknown spectra.
    /// </summary>
    /// <param name="predicitionMatrixXPre">Matrix of unknown spectra (preprocessed the same way as the calibration spectra).</param>
    /// <param name="numberOfFactors">Number of factors used for prediction.</param>
    /// <param name="predictedYPre">On return, holds the predicted y values. (They are centered).</param>
    public virtual void PredictYFromPreprocessed(
      IROMatrix<double> predicitionMatrixXPre, // unknown spectrum or spectra,  horizontal oriented
      int numberOfFactors, // number of factors to use for prediction
      IMatrix<double> predictedYPre // Matrix of predicted y-values, must be same number of rows as spectra
      )
    {
      PredictedYAndSpectralResidualsFromPreprocessed(predicitionMatrixXPre, numberOfFactors, predictedYPre, null);
    }

    /// <summary>
    /// This calculates the spectral residuals.
    /// </summary>
    /// <param name="XUPre">Spectra (horizontally oriented).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <param name="spectralResiduals">On return, holds the spectral residual values.</param>
    public virtual void SpectralResidualsFromPreprocessed(
      IROMatrix<double> XUPre, // unknown spectrum or spectra,  horizontal oriented
      int numberOfFactors, // number of factors to use for prediction
      IMatrix<double> spectralResiduals // Matrix of spectral residuals, n rows x 1 column
      )
    {
      PredictedYAndSpectralResidualsFromPreprocessed(XUPre, numberOfFactors, null, spectralResiduals);
    }

    /// <summary>
    /// This calculates the spectral residuals.
    /// </summary>
    /// <param name="XUPre">Spectra (horizontally oriented).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <returns>The calculated spectral residuals.</returns>
    public virtual IROMatrix<double> SpectralResidualsFromPreprocessed(
      IROMatrix<double> XUPre, // unknown spectrum or spectra,  horizontal oriented
      int numberOfFactors // number of factors to use for prediction
      )
    {
      var result = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(XUPre.RowCount, NumberOfSpectralResiduals);
      SpectralResidualsFromPreprocessed(XUPre, numberOfFactors, result);
      return result;
    }

    /// <summary>
    /// Calculates the spectral leverage from preprocessed spectra.
    /// </summary>
    /// <param name="matrixXPre">Matrix of preprocessed spectra (a spectrum = a row in the matrix).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <returns>Matrix of spectral leverages. Normally, this is a (NumberOfPoints,1) matrix, with exception of PLS1, where it is a (NumberOfPoints,NumberOfY) matrix.</returns>
    public virtual IROMatrix<double> GetXLeverageFromPreprocessed(IROMatrix<double> matrixXPre, int numberOfFactors)
    {
      var result = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixXPre.RowCount, 1);
      InternalGetXLeverageFromPreprocessed(matrixXPre, numberOfFactors, result);
      return result;
    }


    #endregion Prediction from preprocessed

    #region Prediction from Raw

    /// <summary>
    /// Predicts y values from raw (unpreprocessed) spectra.
    /// </summary>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency etc.) of the spectra (needed for preprocessing).</param>
    /// <param name="XURaw">Matrix of spectra used for prediction (number of spectra, number of wavelengths).</param>
    /// <param name="numberOfFactors">Number of factors used for prediction.</param>
    /// <param name="predictedYRaw">In return, holds the predicted y values. You have to provide a matrix of
    /// dimensions (number of spectra, number of y).</param>
    public void PredictYFromRaw(
      double[] xOfXRaw,
      IMatrix<double> XURaw, // unknown spectrum or spectra,  horizontal oriented
      int numberOfFactors, // number of factors to use for prediction
      IMatrix<double> predictedYRaw // Matrix of predicted y-values, must be same number of rows as spectra
      )
    {
      PreprocessSpectraForPrediction(InternalCalibrationModel, xOfXRaw, XURaw, out var _, out var resultMatrixXU);
      PredictYFromPreprocessed(resultMatrixXU, numberOfFactors, predictedYRaw);
      PostprocessTargetVariablesInline(InternalCalibrationModel.PreprocessingModel, predictedYRaw);
    }

    /// <summary>
    /// Calculates the spectral leverage from raw spectra.
    /// </summary>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency, etc) of the unpreprocessed spectra.</param>
    /// <param name="matrixXRaw">Matrix of spectra (a spectrum = a row in the matrix).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <returns>Matrix of spectral leverages. Normally, this is a (NumberOfPoints,1) matrix, with exception of PLS1, where it is a (NumberOfPoints,NumberOfY) matrix.</returns>
    public virtual IROMatrix<double> GetXLeverageFromRaw(
      double[] xOfXRaw,
      IMatrix<double> matrixXRaw, int numberOfFactors)
    {
      PreprocessSpectraForPrediction(
        InternalCalibrationModel,
        xOfXRaw, matrixXRaw,
        out var _,
        out var resultMatrixX);

      return GetXLeverageFromPreprocessed(resultMatrixX, numberOfFactors);
    }

    #endregion Prediction from Raw

    #region Cross validation helper functions

    /// <summary>
    /// Function used for cross validation iteration. During cross validation, the original spectral matrix is separated into
    /// a spectral group used for prediction and the remaining calibration spectra. Parameters here:
    /// group: Indices of measurement that are excluded from the analysis (but then used for prediction).
    /// XXRaw: remaining calibration spectra (unpreprocessed).
    /// YYRaw: corresponding remaining concentration data  (unpreprocessed).
    /// XURaw: spectra used for prediction  (unpreprocessed).
    /// YURaw: corresponding concentration data (unpreprocessed).
    /// </summary>
    public delegate void CrossValidationIterationFunction(int[] group, IMatrix<double> XXRaw, IMatrix<double> YYRaw, IMatrix<double> XURaw, IMatrix<double> YURaw);

    /// <summary>
    /// This function separates the spectra into a bunch of spectra used for calibration and the rest of spectra
    /// used for prediction. This separation is repeated until all spectra are used exactly one time for prediction.
    /// </summary>
    /// <param name="matrixXRaw">Matrix of spectra (horizontal oriented).</param>
    /// <param name="matrixYRaw">Matrix of y values.</param>
    /// <param name="groupingStrategy">The strategy how to separate the spectra into the calibration and prediction spectra.</param>
    /// <param name="crossFunction">The function that is called for each separation.</param>
    /// <returns>The mean number of spectra that was used for prediction.</returns>
    public static double CrossValidationIteration(
      IROMatrix<double> matrixXRaw, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix<double> matrixYRaw, // matrix of concentrations (a mixture is a row of this matrix)
      ICrossValidationGroupingStrategy groupingStrategy,
      CrossValidationIterationFunction crossFunction
      )
    {
      //      int[][] groups = bExcludeGroups ? new ExcludeGroupsGroupingStrategy().Group(Y) : new ExcludeSingleMeasurementsGroupingStrategy().Group(Y);
      int[][] groups = groupingStrategy.Group(matrixYRaw);

      IMatrix<double>? XX = null;
      IMatrix<double>? YY = null;
      IMatrix<double>? XU = null;
      IMatrix<double>? YU = null;

      for (int nGroup = 0, prevNumExcludedSpectra = int.MinValue; nGroup < groups.Length; nGroup++)
      {
        int[] spectralGroup = groups[nGroup];
        int numberOfExcludedSpectraOfGroup = spectralGroup.Length;

        if (prevNumExcludedSpectra != numberOfExcludedSpectraOfGroup)
        {
          XX = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixXRaw.RowCount - numberOfExcludedSpectraOfGroup, matrixXRaw.ColumnCount);
          YY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixYRaw.RowCount - numberOfExcludedSpectraOfGroup, matrixYRaw.ColumnCount);
          XU = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfExcludedSpectraOfGroup, matrixXRaw.ColumnCount);
          YU = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfExcludedSpectraOfGroup, matrixYRaw.ColumnCount);
          prevNumExcludedSpectra = numberOfExcludedSpectraOfGroup;
        }

        if (XX is null || YY is null || XU is null || YU is null)
          throw new InvalidProgramException();

        // build a new x and y matrix with the group information
        // fill XX and YY with values
        for (int i = 0, j = 0; i < matrixXRaw.RowCount; i++)
        {
          if (Array.IndexOf(spectralGroup, i) >= 0) // if spectral group contains i
            continue; // Exclude this row from the spectra
          MatrixMath.SetRow(matrixXRaw, i, XX, j);
          MatrixMath.SetRow(matrixYRaw, i, YY, j);
          j++;
        }

        // fill XU (unknown spectra) with values
        for (int i = 0; i < spectralGroup.Length; i++)
        {
          int j = spectralGroup[i];
          MatrixMath.SetRow(matrixXRaw, j, XU, i); // x-unkown (unknown spectra)
          MatrixMath.SetRow(matrixYRaw, j, YU, i); // y-unkown (unknown concentration)
        }

        // now do the analysis
        crossFunction(spectralGroup, XX, YY, XU, YU);
      } // for all groups

      // calculate the mean number of excluded spectras
      return ((double)matrixXRaw.RowCount) / groups.Length;
    }

    #endregion Cross validation helper functions

    #region Cross validation functions

    /// <summary>
    /// Get the cross predicted error sum of squares for the number of factors=0...numFactors.
    /// </summary>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency, etc) of the unpreprocessed spectra.</param>
    /// <param name="matrixXRaw">Matrix of unpreprocessed spectra (a spectrum is a row in the matrix).</param>
    /// <param name="matrixYRaw">Matrix of unpreprocessed target variables (e.g. concentrations).</param>
    /// <param name="maximalNumberOfFactors">Maximum number of factors to calculate the cross PRESS for.</param>
    /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the spectra, here: how to process each spectrum separately.</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the spectra, here: how to process the spectra ensemble.</param>
    /// <param name="regressionMethod">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
    /// <param name="crossPRESS">The vector of CROSS press values. Note that this vector has the length numFactor+1.</param>
    /// <returns>The mean number of spectra used for prediction.</returns>
    public static double GetCrossPRESS(
      double[] xOfXRaw,
      IROMatrix<double> matrixXRaw, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix<double> matrixYRaw, // matrix of concentrations (a mixture is a row of this matrix)
      int maximalNumberOfFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression regressionMethod,
      out IReadOnlyList<double> crossPRESS // vertical value of PRESS values for the cross validation
      )
    {
      var worker = new CrossPRESSEvaluator(xOfXRaw, maximalNumberOfFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, regressionMethod);
      double result = CrossValidationIteration(matrixXRaw, matrixYRaw, groupingStrategy, new CrossValidationIterationFunction(worker.EhCrossPRESS));

      crossPRESS = VectorMath.ToROVector(worker.CrossPRESS, worker.NumberOfFactors + 1);

      return result;
    }


    /// <summary>
    /// Calculates the cross predicted y values.
    /// </summary>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency, etc) of the unpreprocessed spectra.</param>
    /// <param name="matrixXRaw">Matrix of unpreprocessed spectra (a spectrum is a row in the matrix).</param>
    /// <param name="matrixYRaw">Matrix of unpreprocessed target variables (e.g. concentrations).</param>
    /// <param name="numberOfFactors">Number of factors used for calculation.</param>
    /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the spectra, here: how to process each spectrum separately.</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the spectra, here: how to process the spectra ensemble.</param>
    /// <param name="regressionMethod">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
    /// <param name="yCrossPredicted">Matrix of cross predicted y values. Must be of same dimension as the Y matrix.</param>
    /// <returns>Mean number of spectra used for cross prediction.</returns>
    public static double GetCrossYPredicted(
      double [] xOfXRaw,
      IROMatrix<double> matrixXRaw, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix<double> matrixYRaw, // matrix of concentrations (a mixture is a row of this matrix)
      int numberOfFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression regressionMethod,

      IMatrix<double> yCrossPredicted // vertical value of PRESS values for the cross validation
      )
    {
      var worker = new CrossPredictedYEvaluator(xOfXRaw, numberOfFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra,regressionMethod, yCrossPredicted);
      double result = CrossValidationIteration(matrixXRaw, matrixYRaw, groupingStrategy, new CrossValidationIterationFunction(worker.EhYCrossPredicted));

      return result;
    }



    /// <summary>
    /// Calculates the spectral residuals obtained from cross validation.
    /// </summary>
    /// <param name="xOfXRaw">The x-values (wavelength, frequency, etc) of the unpreprocessed spectra.</param>
    /// <param name="matrixXRaw">Matrix of spectra (a spectrum = a row in the matrix).</param>
    /// <param name="matrixYRaw">Matrix of y values (e.g. concentrations).</param>
    /// <param name="numFactors">Number of factors used for calculation.</param>
    /// <param name="groupingStrategy">The strategy how to group the spectra for cross prediction.</param>
    /// <param name="preprocessSingleSpectrum">Information how to preprocess the spectra, here: how to process each spectrum separately.</param>
    /// <param name="preprocessEnsembleOfSpectra">Information how to preprocess the spectra, here: how to process the spectra ensemble.</param>
    /// <param name="regress">The type of regression (e.g. PCR, PLS1, PLS2) provided as an empty regression object.</param>
    /// <param name="crossXResiduals">Returns the matrix of spectral residuals</param>
    /// <returns>Mean number of spectra used for prediction.</returns>
    public static double GetCrossXResiduals(
      double[] xOfXRaw,
      IROMatrix<double> matrixXRaw, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix<double> matrixYRaw, // matrix of concentrations (a mixture is a row of this matrix)
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      MultivariateRegression regress,

      out IROMatrix<double> crossXResiduals
      )
    {
      var worker = new CrossPredictedXResidualsEvaluator(matrixXRaw.RowCount, xOfXRaw, numFactors, groupingStrategy, preprocessSingleSpectrum, preprocessEnsembleOfSpectra, regress);
      double result = CrossValidationIteration(matrixXRaw, matrixYRaw, groupingStrategy, new CrossValidationIterationFunction(worker.EhCrossValidationWorker));
      crossXResiduals = worker.XCrossResiduals;
      return result;
    }

    #endregion Cross validation functions
  }
}
