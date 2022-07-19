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

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleMeanScale;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// WorksheetMethods provides common utility methods for multivariate data analysis.
  /// </summary>
  public abstract class WorksheetAnalysis
  {
    public class OriginalDataTableNotFoundException : ApplicationException
    {
      public OriginalDataTableNotFoundException()
      {
      }

      public OriginalDataTableNotFoundException(string msg)
        : base(msg)
      {
      }
    }

    #region Column Naming

    private const string _XOfX_ColumnName = "XOfX";
    private const string _XMean_ColumnName = "XMean";
    private const string _XScale_ColumnName = "XScale";
    private const string _YMean_ColumnName = "YMean";
    private const string _YScale_ColumnName = "YScale";

    private const string _XLoad_ColumnName = "XLoad";
    private const string _XWeight_ColumnName = "XWeight";
    private const string _XScore_ColumnName = "XScore";
    private const string _YLoad_ColumnName = "YLoad";
    private const string _CrossProduct_ColumnName = "CrossProduct";

    private const string _PRESSValue_ColumnName = "PRESS";
    private const string _CrossPRESSValue_ColumnName = "CrossPRESS";
    private const string _PredictionScore_ColumnName = "PredictionScore";

    private const string _NumberOfFactors_ColumnName = "NumberOfFactors";
    private const string _MeasurementLabel_ColumnName = "MeasurementLabel";
    private const string _XLabel_ColumnName = "XLabel";
    private const string _YLabel_ColumnName = "YLabel";

    private const string _YOriginal_ColumnName = "YOriginal";
    private const string _YPredicted_ColumnName = "YPredicted";
    private const string _YCrossPredicted_ColumnName = "YCrossPredicted";
    private const string _YResidual_ColumnName = "YResidual";
    private const string _YCrossResidual_ColumnName = "YCrossResidual";
    private const string _SpectralResidual_ColumnName = "SpectralResidual";
    private const string _SpectralCrossResidual_ColumnName = "SpectralCrossResidual";
    private const string _XLeverage_ColumnName = "ScoreLeverage";
    private const string _FRatio_ColumnName = "F-Ratio";
    private const string _FProbability_ColumnName = "F-Probability";

    public static string GetXOfX_ColumnName()
    {
      return string.Format("{0}", _XOfX_ColumnName);
    }

    public static string GetXMean_ColumnName()
    {
      return string.Format("{0}", _XMean_ColumnName);
    }

    public static string GetXScale_ColumnName()
    {
      return string.Format("{0}", _XScale_ColumnName);
    }

    public static string GetYMean_ColumnName()
    {
      return string.Format("{0}", _YMean_ColumnName);
    }

    public static string GetYScale_ColumnName()
    {
      return string.Format("{0}", _YScale_ColumnName);
    }

    public static string GetMeasurementLabel_ColumnName()
    {
      return string.Format("{0}", _MeasurementLabel_ColumnName);
    }

    public static string GetNumberOfFactors_ColumnName()
    {
      return string.Format("{0}", _NumberOfFactors_ColumnName);
    }

    public static string GetXLoad_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}", _XLoad_ColumnName, numberOfFactors);
    }

    public static string GetXLoad_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _XLoad_ColumnName, nConstituent, numberOfFactors);
    }

    public static string GetPredictionScore_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _PredictionScore_ColumnName, nConstituent, numberOfFactors);
    }

    public static string GetXScore_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}", _XScore_ColumnName, numberOfFactors);
    }

    public static string GetXWeight_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}", _XWeight_ColumnName, numberOfFactors);
    }

    public static string GetXWeight_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _XWeight_ColumnName, nConstituent, numberOfFactors);
    }

    public static string GetYLoad_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}", _YLoad_ColumnName, numberOfFactors);
    }

    public static string GetYLoad_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _YLoad_ColumnName, nConstituent, numberOfFactors);
    }

    public static string GetCrossProduct_ColumnName()
    {
      return string.Format("{0}", _CrossProduct_ColumnName);
    }

    public static string GetCrossProduct_ColumnName(int nConstituent)
    {
      return string.Format("{0}{1}", _CrossProduct_ColumnName, nConstituent);
    }

    public static string GetPRESSValue_ColumnName()
    {
      return string.Format("{0}", _PRESSValue_ColumnName);
    }

    public static string GetCrossPRESSValue_ColumnName()
    {
      return string.Format("{0}", _CrossPRESSValue_ColumnName);
    }

    /// <summary>
    /// Gets the column name of a Y-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _YResidual_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a Y-Cross-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYCrossResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _YCrossResidual_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a Y-Original column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYOriginal_ColumnName(int whichY)
    {
      return string.Format("{0}{1}", _YOriginal_ColumnName, whichY);
    }

    public void CalculateAdditionalColumns(DataTable destTable, ImmutableDictionary<string, ImmutableHashSet<(int, int?)>> columnsToCalculate)
    {
      foreach (var entry in columnsToCalculate)
      {
        var list = new List<(int numberOfFactors, int? whichY)>(entry.Value);
        list.Sort((a, b) =>
        {
          var r = Comparer<int>.Default.Compare(a.numberOfFactors, b.numberOfFactors);
          return r != 0 ? r : Comparer<int?>.Default.Compare(a.whichY, b.whichY);
        });

        foreach (var pair in list)
        {
          CalculateAdditionalColumn(destTable, entry.Key, pair.numberOfFactors, pair.whichY);
        }
      }
    }

    public void CalculateAdditionalColumn(DataTable destTable, string columnName, int numberOfFactors, int? whichY)
    {
      switch (columnName)
      {
        case _YCrossPredicted_ColumnName:
          CalculateCrossPredictedAndResidual(destTable, whichY ?? 0, numberOfFactors, true, false, false);
          break;
        case _YCrossResidual_ColumnName:
          CalculateCrossPredictedAndResidual(destTable, whichY ?? 0, numberOfFactors, false, true, false);
          break;
        case _SpectralCrossResidual_ColumnName:
          CalculateCrossPredictedAndResidual(destTable, whichY ?? 0, numberOfFactors, false, false, true);
          break;
        case _YPredicted_ColumnName:
          CalculatePredictedAndResidual(destTable, whichY ?? 0, numberOfFactors, true, false, false);
          break;
        case _YResidual_ColumnName:
          CalculatePredictedAndResidual(destTable, whichY ?? 0, numberOfFactors, false, true, false);
          break;
        case _SpectralResidual_ColumnName:
          CalculatePredictedAndResidual(destTable, whichY ?? 0, numberOfFactors, false, false, true);
          break;
        case _XLeverage_ColumnName:
          CalculateXLeverage(destTable, numberOfFactors);
          break;
        case _PredictionScore_ColumnName:
          CalculateAndStorePredictionScores(destTable, numberOfFactors, whichY ?? 0);
          break;

        default:
          throw new NotImplementedException($"Column name '{columnName}' is unknown or not implemented");
      }
    }

    /// <summary>
    /// Gets the column name of a Y-Predicted column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYPredicted_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _YPredicted_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a Y-Cross-Predicted column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYCrossPredicted_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _YCrossPredicted_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a X-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _SpectralResidual_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a X-Crossvalidated spectral Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXCrossResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _SpectralCrossResidual_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a X-Leverage column
    /// </summary>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXLeverage_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}", _XLeverage_ColumnName, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a X-Leverage column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXLeverage_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}", _XLeverage_ColumnName, whichY, numberOfFactors);
    }

    [DoesNotReturn]
    protected static void NotFound(string name)
    {
      throw new ArgumentException($"Column {name} not found in the table.");
    }

    #endregion Column Naming

    #region Column Grouping

    private const int _NumberOfFactors_ColumnGroup = 4;
    private const int _FRatio_ColumnGroup = 4;
    private const int _FProbability_ColumnGroup = 4;

    private const int _MeasurementLabel_ColumnGroup = 5;

    private const int _YPredicted_ColumnGroup = 5;
    private const int _YResidual_ColumnGroup = 5;
    private const int _XLeverage_ColumnGroup = 5;
    private const int _PredictionScore_ColumnGroup = 0;

    public static int GetNumberOfFactors_ColumnGroup()
    {
      return _NumberOfFactors_ColumnGroup;
    }

    public static int GetYPredicted_ColumnGroup()
    {
      return _YPredicted_ColumnGroup;
    }

    public static int GetYCrossPredicted_ColumnGroup()
    {
      return _YPredicted_ColumnGroup;
    }

    public static int GetYCrossResidual_ColumnGroup()
    {
      return _YPredicted_ColumnGroup;
    }

    public static int GetXResidual_ColumnGroup()
    {
      return _YPredicted_ColumnGroup;
    }

    public static int GetXCrossResidual_ColumnGroup()
    {
      return _YPredicted_ColumnGroup;
    }

    public static int GetPredictionScore_ColumnGroup()
    {
      return _PredictionScore_ColumnGroup;
    }

    public static int GetYResidual_ColumnGroup()
    {
      return _YResidual_ColumnGroup;
    }

    public static int GetXLeverage_ColumnGroup()
    {
      return _XLeverage_ColumnGroup;
    }

    #endregion Column Grouping

    #region Abstract members

    /// <summary>
    /// Execute an analysis and stores the result in the provided table.
    /// </summary>
    /// <param name="matrixX">The matrix of spectra (horizontal oriented), centered and preprocessed.</param>
    /// <param name="matrixY">The matrix of concentrations, centered.</param>
    /// <param name="options">Information how to perform the analysis.</param>
    /// <param name="table">The table where to store the results to.</param>
    /// <param name="press">On return, gives a vector holding the PRESS values of the analysis.</param>
    public virtual void ExecuteAnalysis(
      IMatrix<double> matrixX,
      IMatrix<double> matrixY,
      DimensionReductionAndRegressionOptions options,
      DataTable table,
      out IROVector<double> press
      )
    {
      int numFactors = Math.Min(matrixX.ColumnCount, options.MaximumNumberOfFactors);
      MultivariateRegression regress = CreateNewRegressionObject();
      regress.AnalyzeFromPreprocessed(matrixX, matrixY, numFactors);
      press = regress.GetPRESSFromPreprocessed(matrixX);
      StoreCalibrationModelInTable(regress.CalibrationModel, table);
    }

    public abstract MultivariateRegression CreateNewRegressionObject();

    /// <summary>
    /// Stores the calibrationSet into the data table table.
    /// </summary>
    /// <param name="calibrationSet">The data source.</param>
    /// <param name="table">The table where to store the data of the calibrationSet.</param>
    public abstract void StoreCalibrationModelInTable(IMultivariateCalibrationModel calibrationSet, DataTable table);

    /// <summary>
    /// Calculate the cross PRESS values and stores the results in the provided table.
    /// </summary>
    /// <param name="xOfXRaw">Vector of spectral wavelengths. Necessary to divide the spectras in different regions.</param>
    /// <param name="matrixXRaw">Matrix of unpreprocessed spectra (horizontal oriented).</param>
    /// <param name="matrixYRaw">Matrix of unpreprocessed concentrations.</param>
    /// <param name="plsOptions">Analysis options.</param>
    /// <param name="plsContent">Information about this analysis.</param>
    /// <param name="destinationTable">Table to store the results.</param>
    public virtual void CalculateCrossPRESS(
      double[] xOfXRaw,
      IMatrix<double> matrixXRaw,
      IMatrix<double> matrixYRaw,
      DimensionReductionAndRegressionOptions plsOptions,
      ref DimensionReductionAndRegressionResult plsContent,
      DataTable destinationTable
      )
    {

      var crosspresscol = new Altaxo.Data.DoubleColumn();

      double meanNumberOfExcludedSpectra = 0;
      if (plsOptions.CrossValidationGroupingStrategy is not null && plsOptions.CrossValidationGroupingStrategy is not CrossValidationGroupingStrategyNone)
      {
        // now a cross validation - this can take a long time for bigger matrices

        MultivariateRegression.GetCrossPRESS(
          xOfXRaw, matrixXRaw, matrixYRaw,
          plsOptions.MaximumNumberOfFactors,
          plsOptions.CrossValidationGroupingStrategy,
          plsOptions.Preprocessing,
          plsOptions.MeanScaleProcessing,
          CreateNewRegressionObject(),
          out var crossPRESSMatrix);

        VectorMath.Copy(crossPRESSMatrix, DataColumnWrapper.ToVector(crosspresscol, crossPRESSMatrix.Length));

        destinationTable.DataColumns.Add(crosspresscol, GetCrossPRESSValue_ColumnName(), Altaxo.Data.ColumnKind.V, 4);

        plsContent = plsContent with { MeanNumberOfMeasurementsInCrossPRESSCalculation = plsContent.NumberOfMeasurements - meanNumberOfExcludedSpectra };
      }
      else
      {
        destinationTable.DataColumns.Add(crosspresscol, GetCrossPRESSValue_ColumnName(), Altaxo.Data.ColumnKind.V, 4);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mcalib"></param>
    /// <param name="groupingStrategy"></param>
    /// <param name="preprocessOptions"></param>
    /// <param name="xOfXRaw"></param>
    /// <param name="matrixXRaw">Matrix of horizontal spectra, centered and preprocessed.</param>
    /// <param name="matrixYRaw">Matrix of concentrations, centered.</param>
    /// <param name="numberOfFactors"></param>
    /// <param name="predictedY"></param>
    /// <param name="spectralResiduals"></param>
    public virtual void CalculateCrossPredictedY(
      IMultivariateCalibrationModel mcalib,
      ICrossValidationGroupingStrategy groupingStrategy,
       ISingleSpectrumPreprocessor preprocessSingleSpectrum,
      IEnsembleMeanScalePreprocessor preprocessEnsembleOfSpectra,
      double[] xOfXRaw,
      IMatrix<double> matrixXRaw,
      IMatrix<double> matrixYRaw,
      int numberOfFactors,
      IMatrix<double> predictedY,
      IMatrix<double> spectralResiduals)
    {
      MultivariateRegression.GetCrossYPredicted(
        xOfXRaw, matrixXRaw, matrixYRaw,
        numberOfFactors, groupingStrategy,
        preprocessSingleSpectrum, preprocessEnsembleOfSpectra,
        CreateNewRegressionObject(),
        predictedY);
    }

    /// <summary>
    /// Name of the Analysis, like PLS1, PLS2 or PCR.
    /// </summary>
    public abstract string AnalysisName
    {
      get;
    }

    /// <summary>
    /// Returns an instance of the calibration model specific for the multivariate analysis.
    /// </summary>
    /// <param name="calibTable">The table where the calibration model is stored.</param>
    /// <returns>Instance of the calibration model (in more handy format).</returns>
    public abstract IMultivariateCalibrationModel GetCalibrationModel(DataTable calibTable);

    /// <summary>
    /// Stores the information that a given column was calculated inside the DataSource object of the table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="columnName">Name of the column that was calculated.</param>
    /// <param name="firstArg">The first argument (e.g. number of factors).</param>
    /// <param name="secondArg">The second argument (optional, e.g. number of target variable).</param>
    protected void StoreCalculatedColumn(DataTable table, string columnName, int firstArg, int? secondArg = null)
    {
      if (table.DataSource is DimensionReductionAndRegressionDataSource drrd)
      {
        drrd.ProcessOptions = drrd.ProcessOptions.WithColumnToCalculate(columnName, firstArg, secondArg);
      }
    }

    /// <summary>
    /// Calculates the leverage of the spectral data.
    /// </summary>
    /// <param name="table">Table where the calibration model is stored.</param>
    /// <param name="numberOfFactors">Number of factors used to calculate leverage.</param>
    public virtual void CalculateXLeverage(
      DataTable table, int numberOfFactors)
    {
      if (!IsDimensionReductionAndRegressionModel(table, out var dataSource))
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      IMultivariateCalibrationModel calib = GetCalibrationModel(table);

      GetXYMatricesOfSpectralColumns(dataSource.ProcessData, out var xOfX, out var matrixX, out var _);

      /*
      MultivariateRegression.PreprocessSpectraForPrediction(
        dataSource.ProcessOptions.Preprocessing,
        dataSource.ProcessOptions.MeanScaleProcessing,
        null, xOfX, matrixX, calib.PreprocessingModel.XMean, calib.PreprocessingModel.XScale,
        out var resultMatrixX);
      */

      MultivariateRegression regress = CreateNewRegressionObject();
      regress.SetCalibrationModel(calib);

      var xLeverage = regress.GetXLeverageFromRaw(xOfX, matrixX, numberOfFactors);

      for (int i = 0; i < xLeverage.ColumnCount; i++)
      {
        var col = new Altaxo.Data.DoubleColumn();
        MatrixMath.SetColumn(xLeverage, i, DataColumnWrapper.ToVertMatrix(col, xLeverage.RowCount), 0);
        table.DataColumns.Add(
          col,
          xLeverage.ColumnCount == 1 ? GetXLeverage_ColumnName(numberOfFactors) : GetXLeverage_ColumnName(i, numberOfFactors),
          Altaxo.Data.ColumnKind.V, GetXLeverage_ColumnGroup());
      }

      StoreCalculatedColumn(table, _XLeverage_ColumnName, numberOfFactors);
    }

    /// <summary>
    /// Calculates the prediction scores. In case of a single y-variable, the prediction score is a vector of the same length than a spectrum.
    /// Multiplying the prediction score (dot product) with a spectrum, it yields the predicted y-value (of cause mean-centered).
    /// </summary>
    /// <param name="table">The table where the calibration model is stored.</param>
    /// <param name="preferredNumberOfFactors">Number of factors used to calculate the prediction scores.</param>
    /// <returns>A matrix with the prediction scores. Each score (for one y-value) is a row in the matrix.</returns>
    public virtual IROMatrix<double> CalculatePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      MultivariateRegression regress = CreateNewRegressionObject();
      IMultivariateCalibrationModel model = GetCalibrationModel(table);
      regress.SetCalibrationModel(model);
      return regress.GetPredictionScores(preferredNumberOfFactors);
    }

    /// <summary>
    /// Compares the two arrays and throws an InvalidOperationException if the data not match.
    /// </summary>
    /// <param name="xOfXPredicition">The x of x predicition.</param>
    /// <param name="xOfXModel">The x of x model.</param>
    /// <exception cref="System.InvalidOperationException">
    /// Number of spectral points after preprocessing ({xOfXPredicition}) does not match number of spectal points of model ({xOfXModel.Count})
    /// or
    /// X-values of preprocessed data: x[{i}]={xOfXPredicition[i]} does not match that of the model: x[{i}]={xOfXModel[i]}
    /// </exception>
    public void EnsureMatchingXOfX(IReadOnlyList<double> xOfXPredicition, IReadOnlyList<double> xOfXModel)
    {
      if (xOfXPredicition.Count != xOfXModel.Count)
        throw new InvalidOperationException($"Number of spectral points after preprocessing ({xOfXPredicition}) does not match number of spectal points of model ({xOfXModel.Count})");

      for (int i = 0; i < xOfXModel.Count; i++)
      {
        if (xOfXPredicition[i] != xOfXModel[i])
        {
          throw new InvalidOperationException($"X-values of preprocessed data: x[{i}]={xOfXPredicition[i]} does not match that of the model: x[{i}]={xOfXModel[i]}");
        }
      }
    }

    /// <summary>
    /// For a given set of spectra, predicts the y-values and stores them in the matrix <c>predictedY</c>
    /// </summary>
    /// <param name="mcalib">The calibration model of the analysis.</param>
    /// <param name="preprocessOptions">The information how to preprocess the spectra.</param>
    /// <param name="matrixXRaw">The matrix of raw (unprocessed) spectra to predict. Each spectrum is a row in the matrix.</param>
    /// <param name="numberOfFactors">The number of factors used for prediction.</param>
    /// <param name="predictedY">On return, this matrix holds the predicted y-values. Each row in this matrix corresponds to the same row (spectrum) in matrixX.</param>
    /// <param name="spectralResiduals">If you set this parameter to a appropriate matrix, the spectral residuals will be stored in this matrix. Set this parameter to null if you don't need the residuals.</param>
    public virtual void CalculatePredictedY(
      IMultivariateCalibrationModel mcalib,
      double[] xOfXRaw,
      IMatrix<double> matrixXRaw,
      int numberOfFactors,
      MatrixMath.LeftSpineJaggedArrayMatrix<double> predictedY,
      IMatrix<double>? spectralResiduals)
    {
      MultivariateRegression.PreprocessSpectraForPrediction(mcalib, xOfXRaw, matrixXRaw, out var xOfXPre, out var matrixXPre);

      EnsureMatchingXOfX(xOfXPre, mcalib.PreprocessingModel.XOfX);

      MultivariateRegression regress = CreateNewRegressionObject();
      regress.SetCalibrationModel(mcalib);
      regress.PredictedYAndSpectralResidualsFromPreprocessed(matrixXPre, numberOfFactors, predictedY, spectralResiduals);
      MultivariateRegression.PostprocessTargetVariablesInline(mcalib.PreprocessingModel, predictedY);
    }

    #endregion Abstract members

    #region static Helper methods






    /// <summary>
    /// Fills a matrix with the selected data of a table.
    /// </summary>
    /// <param name="srctable">The source table where the data for the spectra are located.</param>
    /// <param name="spectrumIsRow">True if the spectra in the table are organized horizontally, false if spectra are vertically oriented.</param>
    /// <param name="spectralIndices">The selected indices wich indicate all (wavelength, frequencies, etc.) that belong to one spectrum. If spectrumIsRow==true, this are the selected column indices, otherwise the selected row indices.</param>
    /// <param name="measurementIndices">The indices of all measurements (spectra) selected.</param>
    /// <returns>The matrix of spectra. In this matrix the spectra are horizonally organized (each row is one spectrum).</returns>
    public static IMatrix<double> GetRawSpectra(Altaxo.Data.DataTable srctable, bool spectrumIsRow, Altaxo.Collections.IAscendingIntegerCollection spectralIndices, Altaxo.Collections.IAscendingIntegerCollection measurementIndices)
    {
      if (srctable is null)
        throw new ArgumentException("Argument srctable may not be null");

      var matrixX = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(measurementIndices.Count, spectralIndices.Count);

      if (spectrumIsRow)
      {
        for (int i = 0; i < spectralIndices.Count; i++)
        {
          // labelColumnOfX[i] = spectralIndices[i];
          var col = (srctable[spectralIndices[i]] as Altaxo.Data.INumericColumn) ?? throw new InvalidOperationException($"Column {spectralIndices[i]} (at spectral index {i}) is not numeric!");
          for (int j = 0; j < measurementIndices.Count; j++)
          {
            matrixX[j, i] = col[measurementIndices[j]];
          }
        } // end fill in x-values
      }
      else // vertical oriented spectrum
      {
        for (int i = 0; i < spectralIndices.Count; i++)
        {
          // labelColumnOfX[i] = spectralIndices[i];
        }
        for (int i = 0; i < measurementIndices.Count; i++)
        {
          var col = (srctable[measurementIndices[i]] as Altaxo.Data.INumericColumn) ?? throw new InvalidOperationException($"Column {measurementIndices[i]} (at measurement index {i}) is not numeric!");
          for (int j = 0; j < spectralIndices.Count; j++)
          {
            matrixX[i, j] = col[spectralIndices[j]];
          }
        } // end fill in x-values
      }

      return matrixX;
    }

    /// <summary>
    /// Returns the corresponding x-values of the spectra (In fact only the first spectra is used to determine which x-column is used).
    /// </summary>
    /// <param name="srctable">The source table where the data for the spectra are located.</param>
    /// <param name="spectrumIsRow">True if the spectra in the table are organized horizontally, false if spectra are vertically oriented.</param>
    /// <param name="spectralIndices">The selected indices wich indicate all (wavelength, frequencies, etc.) that belong to one spectrum. If spectrumIsRow==true, this are the selected column indices, otherwise the selected row indices.</param>
    /// <param name="measurementIndices">The indices of all measurements (spectra) selected.</param>
    /// <returns>The x-values corresponding to the first spectrum.</returns>
    public static double[] GetXOfSpectra(Altaxo.Data.DataTable srctable, bool spectrumIsRow, Altaxo.Collections.IAscendingIntegerCollection spectralIndices, Altaxo.Collections.IAscendingIntegerCollection measurementIndices)
    {
      if (srctable is null)
        throw new ArgumentException("Argument srctable may not be null");

      int group;
      Altaxo.Data.INumericColumn? col;

      if (spectrumIsRow)
      {
        group = srctable.DataColumns.GetColumnGroup(spectralIndices[0]);

        col = srctable.PropertyColumns.FindXColumnOfGroup(group) as Altaxo.Data.INumericColumn;
      }
      else // vertical oriented spectrum
      {
        group = srctable.DataColumns.GetColumnGroup(measurementIndices[0]);

        col = srctable.DataColumns.FindXColumnOfGroup(group) as Altaxo.Data.INumericColumn;
      }

      if (col is null)
        col = new IndexerColumn();

      double[] result = new double[spectralIndices.Count];

      for (int i = 0; i < spectralIndices.Count; i++)
        result[i] = col[spectralIndices[i]];

      return result;
    }

    /// <summary>
    /// Gets the spectral and target variable matrices (both unpreprocessed) for analysis.
    /// </summary>
    /// <param name="data">The data object storing which columns participate.</param>
    /// <param name="xOfXRaw">On return, contains the x-values (wavelength, frequency, etc) of the spectra</param>
    /// <param name="matrixXRaw">On return, contains the spectra. Each row represents one spectrum.</param>
    /// <param name="matrixYRaw">On return, contains the target variables. Each row represents one measurement (belonging to one spectrum).</param>
    /// <exception cref="System.InvalidOperationException">Data column at index {i} is null!</exception>
    public static void GetXYMatricesOfSpectralColumns(
      DataTableMatrixProxyWithMultipleColumnHeaderColumns data,
      out double[] xOfXRaw,
      out IMatrix<double>? matrixXRaw,
      out IMatrix<double>? matrixYRaw)
    {
      int numberOfSpectra = data.ColumnCount;
      int pointsPerSpectra = data.RowCount;
      int numberOfTargetVariables = data.ColumnHeaderColumnsCount;

      matrixXRaw = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfSpectra, pointsPerSpectra);
      matrixYRaw = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfSpectra, numberOfTargetVariables);
      xOfXRaw = new double[pointsPerSpectra];

      var concentrationIndices = new AscendingIntegerCollection();
      var measurementIndices = new AscendingIntegerCollection();
      var spectralIndices = new AscendingIntegerCollection();
      var srcTable = data.DataTable;


      for (int i = 0; i < numberOfSpectra; i++)
      {
        var col = data.GetDataColumnProxy(i)?.Document();
        if (col is null)
          throw new InvalidOperationException($"Data column at index {i} is null!");

        var idxSpectralColumn = srcTable.DataColumns.GetColumnNumber((DataColumn)col);
        measurementIndices.Add(idxSpectralColumn);

        for (int j = 0; j < pointsPerSpectra; j++)
        {
          matrixXRaw[i, j] = data.UseAllAvailableDataRows ? col[j] : col[data.ParticipatingDataRows[j]];
        }

        for (int j = 0; j < numberOfTargetVariables; j++)
        {
          var pcol = data.GetColumnHeaderWrapper(j);
          matrixYRaw[i, j] = pcol[i];
        }
      }

      var xCol = data.GetRowHeaderWrapper();
      for (int j = 0; j < pointsPerSpectra; ++j)
      {
        xOfXRaw[j] = xCol[j];
      }


      for (int j = 0; j < numberOfTargetVariables; ++j)
      {
        var pcol = (DataColumn)data.GetColumnHeaderColumn(j).Document();
        var idxPropertyColumn = srcTable.PropCols.GetColumnNumber(pcol);
        concentrationIndices.Add(idxPropertyColumn);
      }

      for (int j = 0; j < pointsPerSpectra; j++)
      {
        spectralIndices.Add(data.ParticipatingDataRows[j]);
      }
    }

    /// <summary>
    /// Using the information in the plsMemo, gets the matrix of original Y (concentration) data.
    /// </summary>
    /// <param name="data">The data that were the base for the multidimensional analysis.</param>
    /// <returns>Matrix of orignal Y (concentration) data.</returns>
    public static IMatrix<double> GetOriginalY(DataTableMatrixProxyWithMultipleColumnHeaderColumns data)
    {
      int numberOfSpectra = data.ColumnCount;
      int numberOfTargetVariables = data.ColumnHeaderColumnsCount;

      var matrixY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfSpectra, numberOfTargetVariables);

      for (int i = 0; i < numberOfSpectra; i++)
      {
        for (int j = 0; j < numberOfTargetVariables; j++)
        {
          var pcol = data.GetColumnHeaderWrapper(j);
          matrixY[i, j] = pcol[i];
        }
      }
      return matrixY;
    }

    /// <summary>
    /// This maps the indices of a master x column to the indices of a column to map.
    /// </summary>
    /// <param name="xmaster">The master column containing x-values, for instance the spectral wavelength of the PLS calibration model.</param>
    /// <param name="xtomap">The column to map containing x-values, for instance the spectral wavelength of an unknown spectra to predict.</param>
    /// <param name="failureMessage">In case of a mapping error, contains detailed information about the error.</param>
    /// <returns>The indices of the mapping column that matches those of the master column. Contains as many indices as items in xmaster. In case of mapping error, returns null.</returns>
    public static Altaxo.Collections.AscendingIntegerCollection? MapSpectralX(
      IReadOnlyList<double> xmaster,
      IReadOnlyList<double> xtomap, [MaybeNull] out string failureMessage)
    {
      failureMessage = null;
      int mastercount = xmaster.Count;

      int mapcount = xtomap.Count;

      if (mapcount < mastercount)
      {
        failureMessage = string.Format("More items to map ({0} than available ({1}", mastercount, mapcount);
        return null;
      }

      var result = new Altaxo.Collections.AscendingIntegerCollection();
      // return an empty collection if there is nothing to map
      if (mastercount == 0)
        return result;

      // there is only one item to map - we can not check this - return a 1:1 map
      if (mastercount == 1)
      {
        result.Add(0);
        return result;
      }

      // presumtion here (checked before): mastercount>=2, mapcount>=1

      double distanceback, distancecurrent, distanceforward;
      int i, j;
      for (i = 0, j = 0; i < mastercount && j < mapcount; j++)
      {
        distanceback = j == 0 ? double.MaxValue : Math.Abs(xtomap[j - 1] - xmaster[i]);
        distancecurrent = Math.Abs(xtomap[j] - xmaster[i]);
        distanceforward = (j + 1) >= mapcount ? double.MaxValue : Math.Abs(xtomap[j + 1] - xmaster[i]);

        if (distanceback < distancecurrent)
        {
          failureMessage = string.Format("Mapping error - distance of master[{0}] to current map[{1}] is greater than to previous map[{2}]", i, j, j - 1);
          return null;
        }
        else if (distanceforward < distancecurrent)
          continue;
        else
        {
          result.Add(j);
          i++;
        }
      }

      if (i != mastercount)
      {
        failureMessage = string.Format("Mapping error- no mapping found for current master[{0}]", i - 1);
        return null;
      }

      return result;
    }

    /// <summary>
    /// For given selected columns and selected rows, the procedure removes all nonnumeric cells. Furthermore, if either selectedColumns or selectedRows
    /// is empty, the collection is filled by the really used rows/columns.
    /// </summary>
    /// <param name="srctable">The source table.</param>
    /// <param name="selectedRows">On entry, contains the selectedRows (or empty if now rows selected). On output, is the row collection.</param>
    /// <param name="selectedColumns">On entry, conains the selected columns (or emtpy if only rows selected). On output, contains all numeric columns.</param>
    public static void RemoveNonNumericCells(Altaxo.Data.DataTable srctable, Altaxo.Collections.AscendingIntegerCollection selectedRows, Altaxo.Collections.AscendingIntegerCollection selectedColumns)
    {
      // first the columns
      if (0 != selectedColumns.Count)
      {
        for (int i = 0; i < selectedColumns.Count; i++)
        {
          int idx = selectedColumns[i];
          if (!(srctable[idx] is Altaxo.Data.INumericColumn))
          {
            selectedColumns.Remove(idx);
          }
        }
      }
      else // if no columns where selected, select all that are numeric
      {
        int end = srctable.DataColumnCount;
        for (int i = 0; i < end; i++)
        {
          if (srctable[i] is Altaxo.Data.INumericColumn)
            selectedColumns.Add(i);
        }
      }

      // if now rows selected, then test the max row count of the selected columns
      // and add it

      // check the number of rows

      if (0 == selectedRows.Count)
      {
        int numrows = 0;
        int end = selectedColumns.Count;
        for (int i = 0; i < end; i++)
        {
          int idx = selectedColumns[i];
          numrows = Math.Max(numrows, srctable[idx].Count);
        }
        selectedRows.Add(Collections.ContiguousIntegerRange.FromStartAndCount(0, numrows));
      }
    }

    #endregion static Helper methods

    #region Storing results

    public virtual void StorePreprocessedData(
      IReadOnlyList<double> meanX,
      IReadOnlyList<double> scaleX,
      IReadOnlyList<double> meanY,
      IReadOnlyList<double> scaleY,
      DataTable table)
    {
      // Store X-Mean and X-Scale
      var colXMean = new Altaxo.Data.DoubleColumn();
      var colXScale = new Altaxo.Data.DoubleColumn();

      for (int i = 0; i < meanX.Count; i++)
      {
        colXMean[i] = meanX[i];
        colXScale[i] = scaleX[i];
      }

      table.DataColumns.Add(colXMean, _XMean_ColumnName, Altaxo.Data.ColumnKind.V, 0);
      table.DataColumns.Add(colXScale, _XScale_ColumnName, Altaxo.Data.ColumnKind.V, 0);

      // store the y-mean and y-scale
      var colYMean = new Altaxo.Data.DoubleColumn();
      var colYScale = new Altaxo.Data.DoubleColumn();

      for (int i = 0; i < meanY.Count; i++)
      {
        colYMean[i] = meanY[i];
        colYScale[i] = 1;
      }

      table.DataColumns.Add(colYMean, _YMean_ColumnName, Altaxo.Data.ColumnKind.V, 1);
      table.DataColumns.Add(colYScale, _YScale_ColumnName, Altaxo.Data.ColumnKind.V, 1);
    }

    public virtual void StoreXOfX(IReadOnlyList<double> xOfX, DataTable table)
    {
      var xColOfX = table.DataColumns.EnsureExistence(_XOfX_ColumnName, typeof(DoubleColumn), ColumnKind.X, 0);
      VectorMath.Copy(xOfX, DataColumnWrapper.ToVector(xColOfX, xOfX.Count));
    }

    public virtual void StoreNumberOfFactors(
      int nNumberOfFactors,
      DataTable table)
    {
      // add a NumberOfFactors columm

      DoubleColumn? xNumFactor = null;
      if (table.DataColumns.Contains(_NumberOfFactors_ColumnName))
        xNumFactor = table[_NumberOfFactors_ColumnName] as DoubleColumn;

      if (xNumFactor is null)
      {
        xNumFactor = new Altaxo.Data.DoubleColumn();
        table.DataColumns.Add(xNumFactor, _NumberOfFactors_ColumnName, Altaxo.Data.ColumnKind.X, _NumberOfFactors_ColumnGroup);
      }

      using (var suspendToken = xNumFactor.SuspendGetToken())
      {
        for (int i = 0; i < nNumberOfFactors; i++)
        {
          xNumFactor[i] = i;
        }
        suspendToken.Resume();
      }
    }

    public virtual void StorePRESSData(
      IReadOnlyList<double> PRESS,
      DataTable table)
    {
      StoreNumberOfFactors(PRESS.Count, table);

      var presscol = new Altaxo.Data.DoubleColumn();
      for (int i = 0; i < PRESS.Count; i++)
        presscol[i] = PRESS[i];
      table.DataColumns.Add(presscol, GetPRESSValue_ColumnName(), Altaxo.Data.ColumnKind.V, 4);
    }

    public virtual void StoreFRatioData(
      DataTable table,
      ref DimensionReductionAndRegressionResult regressionResult)
    {
      DoubleColumn? pressColumn = null;
      DoubleColumn? crossPRESSColumn = null;
      if (table.DataColumns.Contains(GetPRESSValue_ColumnName()))
        pressColumn = table[GetPRESSValue_ColumnName()] as DoubleColumn;
      if (table.DataColumns.Contains(GetCrossPRESSValue_ColumnName()))
        crossPRESSColumn = table[GetCrossPRESSValue_ColumnName()] as DoubleColumn;

      IROVector<double> press;
      double meanNumberOfIncludedSpectra = regressionResult.NumberOfMeasurements;

      if (crossPRESSColumn is not null && crossPRESSColumn.Count > 0)
      {
        press = DataColumnWrapper.ToROVector(crossPRESSColumn);
        meanNumberOfIncludedSpectra = regressionResult.MeanNumberOfMeasurementsInCrossPRESSCalculation;
      }
      else if (pressColumn is not null && pressColumn.Count > 0)
      {
        press = DataColumnWrapper.ToROVector(pressColumn);
        meanNumberOfIncludedSpectra = regressionResult.NumberOfMeasurements;
      }
      else
        return;

      // calculate the F-ratio and the F-Probability
      int numberOfSignificantFactors = press.Length;
      double pressMin = double.MaxValue;
      for (int i = 0; i < press.Length; i++)
        pressMin = Math.Min(pressMin, press[i]);
      var fratiocol = new DoubleColumn();
      var fprobcol = new DoubleColumn();
      for (int i = 0; i < press.Length; i++)
      {
        double fratio = press[i] / pressMin;
        double fprob = Calc.Probability.FDistribution.CDF(fratio, meanNumberOfIncludedSpectra, meanNumberOfIncludedSpectra);
        fratiocol[i] = fratio;
        fprobcol[i] = fprob;
        if (fprob < 0.75 && numberOfSignificantFactors > i)
          numberOfSignificantFactors = i;
      }

      regressionResult = regressionResult with { PreferredNumberOfFactors = numberOfSignificantFactors };

      table.DataColumns.Add(fratiocol, _FRatio_ColumnName, Altaxo.Data.ColumnKind.V, _FRatio_ColumnGroup);
      table.DataColumns.Add(fprobcol, _FProbability_ColumnName, Altaxo.Data.ColumnKind.V, _FProbability_ColumnGroup);
    }

    public void StoreOriginalY(
      DataTable table,
      DataTableMatrixProxyWithMultipleColumnHeaderColumns regressionData
      )
    {
      IMatrix<double> matrixY = GetOriginalY(regressionData);
      IMultivariateCalibrationModel calib = GetCalibrationModel(table);

      {
        // add a label column for the measurement number
        var measurementLabel = new Altaxo.Data.DoubleColumn();
        for (int i = 0; i < matrixY.RowCount; i++)
        {
          measurementLabel[i] = regressionData.ParticipatingDataColumns[i];
        }
        table.DataColumns.Add(measurementLabel, _MeasurementLabel_ColumnName, Altaxo.Data.ColumnKind.Label, _MeasurementLabel_ColumnGroup);
      }

      // now add the original Y-Columns
      for (int i = 0; i < matrixY.ColumnCount; i++)
      {
        var col = new Altaxo.Data.DoubleColumn();
        for (int j = 0; j < matrixY.RowCount; j++)
          col[j] = matrixY[j, i];
        table.DataColumns.Add(col, _YOriginal_ColumnName + i.ToString(), Altaxo.Data.ColumnKind.X, 5 + i);
      }
    }

    #endregion Storing results

    #region Calculation of results

    /// <summary>
    /// Calculated the predicted y for the component given by <c>whichY</c> and for the given number of factors.
    /// </summary>
    /// <param name="table">The table containing the PLS model.</param>
    /// <param name="whichY">The number of the y component.</param>
    /// <param name="numberOfFactors">Number of factors for calculation.</param>
    public virtual void CalculateYPredicted(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table, whichY, numberOfFactors, true, false, false);
    }

    /// <summary>
    /// Calculates the y-residuals (concentration etc.).
    /// </summary>
    /// <param name="table">The table where the calibration model is stored.</param>
    /// <param name="whichY">Number of the y-value.</param>
    /// <param name="numberOfFactors">The number of factors used for prediction and then for the residual calculation.</param>
    public virtual void CalculateYResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table, whichY, numberOfFactors, false, true, false);
    }

    public virtual void CalculateXResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table, whichY, numberOfFactors, false, false, true);
    }

    public virtual void CalculateCrossPredictedAndResidual(
      DataTable table,
      int whichY,
      int numberOfFactors,
      bool saveYPredicted,
      bool saveYResidual,
      bool saveXResidual)
    {
      if (table.DataSource is not DimensionReductionAndRegressionDataSource ddrs)
        throw new InvalidOperationException($"Table {table.Name} does not contain a appropriate data source for cross prediction.");

      /*

      var plsMemo = GetContentAsMultivariateContentMemento(table);

      if (plsMemo is null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");
      */

      IMultivariateCalibrationModel calib = GetCalibrationModel(table);
      //      Export(table,out calib);

      GetXYMatricesOfSpectralColumns(ddrs.ProcessData, out var xOfXRaw, out var matrixXRaw, out var matrixYRaw);

      var predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixXRaw.RowCount, calib.NumberOfY);
      var spectralResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixXRaw.RowCount, 1);
      CalculateCrossPredictedY(
        calib,
        ddrs.ProcessOptions.CrossValidationGroupingStrategy,
        ddrs.ProcessOptions.Preprocessing,
        ddrs.ProcessOptions.MeanScaleProcessing,
        xOfXRaw,
        matrixXRaw,
        matrixYRaw,
        numberOfFactors,
        predictedY,
        spectralResiduals);

      if (saveYPredicted)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYCrossPredicted_ColumnName(whichY, numberOfFactors);
        var ycolumn = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < predictedY.RowCount; i++)
          ycolumn[i] = predictedY[i, whichY];

        table.DataColumns.Add(ycolumn, ycolname, Altaxo.Data.ColumnKind.V, GetYCrossPredicted_ColumnGroup());
        StoreCalculatedColumn(table, _YCrossPredicted_ColumnName, numberOfFactors, whichY);
      }

      // subtract the original y data
      MatrixMath.SubtractColumn(predictedY, matrixYRaw, whichY, predictedY);

      if (saveYResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYCrossResidual_ColumnName(whichY, numberOfFactors);
        var ycolumn = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < predictedY.RowCount; i++)
          ycolumn[i] = predictedY[i, whichY];

        table.DataColumns.Add(ycolumn, ycolname, Altaxo.Data.ColumnKind.V, GetYCrossResidual_ColumnGroup());
        StoreCalculatedColumn(table, _YCrossResidual_ColumnName, numberOfFactors, whichY);
      }

      if (saveXResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetXCrossResidual_ColumnName(whichY, numberOfFactors);
        var ycolumn = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < matrixXRaw.RowCount; i++)
        {
          ycolumn[i] = spectralResiduals[i, 0];
        }
        table.DataColumns.Add(ycolumn, ycolname, Altaxo.Data.ColumnKind.V, GetXCrossResidual_ColumnGroup());
        StoreCalculatedColumn(table, _SpectralCrossResidual_ColumnName, numberOfFactors, whichY);
      }
    }

    protected static bool IsDimensionReductionAndRegressionModel(Altaxo.Data.DataTable table, out DimensionReductionAndRegressionDataSource dataSource)
    {
      dataSource = table.DataSource as DimensionReductionAndRegressionDataSource;
      return dataSource is not null;
    }

    public virtual void CalculatePredictedAndResidual(
     DataTable table,
     int whichY,
     int numberOfFactors,
     bool saveYPredicted,
     bool saveYResidual,
     bool saveXResidual)
    {
      if (!IsDimensionReductionAndRegressionModel(table, out var dataSource))
        throw new ArgumentException("Table does not contain a multivariate model.");


      IMultivariateCalibrationModel calib = GetCalibrationModel(table);
      //      Export(table,out calib);

      GetXYMatricesOfSpectralColumns(dataSource.ProcessData, out var xOfX, out var matrixX, out var matrixY);

      var predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixX.RowCount, calib.NumberOfY);
      var spectralResiduals = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixX.RowCount, 1);
      CalculatePredictedY(calib, xOfX, matrixX, numberOfFactors, predictedY, spectralResiduals);

      if (saveYPredicted)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYPredicted_ColumnName(whichY, numberOfFactors);
        var ycolumn = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < predictedY.RowCount; i++)
          ycolumn[i] = predictedY[i, whichY];

        table.DataColumns.Add(ycolumn, ycolname, Altaxo.Data.ColumnKind.V, GetYPredicted_ColumnGroup());
        StoreCalculatedColumn(table, _YPredicted_ColumnName, numberOfFactors, whichY);
      }

      // subract the original y data
      MatrixMath.SubtractColumn(predictedY, matrixY, whichY, predictedY);

      if (saveYResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYResidual_ColumnName(whichY, numberOfFactors);
        var ycolumn = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < predictedY.RowCount; i++)
          ycolumn[i] = predictedY[i, whichY];

        table.DataColumns.Add(ycolumn, ycolname, Altaxo.Data.ColumnKind.V, GetYResidual_ColumnGroup());
        StoreCalculatedColumn(table, _YResidual_ColumnName, numberOfFactors, whichY);
      }

      if (saveXResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetXResidual_ColumnName(whichY, numberOfFactors);
        var ycolumn = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < matrixX.RowCount; i++)
        {
          ycolumn[i] = spectralResiduals[i, 0];
        }
        table.DataColumns.Add(ycolumn, ycolname, Altaxo.Data.ColumnKind.V, GetXResidual_ColumnGroup());
        StoreCalculatedColumn(table, _SpectralResidual_ColumnName, numberOfFactors, whichY);
      }
    }



    public void CalculateAndStorePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      var predictionScores = CalculatePredictionScores(table, preferredNumberOfFactors);
      for (int i = 0; i < predictionScores.ColumnCount; i++)
      {
        var col = table.DataColumns.EnsureExistence(GetPredictionScore_ColumnName(i, preferredNumberOfFactors), typeof(DoubleColumn), Altaxo.Data.ColumnKind.V, GetPredictionScore_ColumnGroup());
        for (int j = 0; j < predictionScores.RowCount; j++)
        {
          col[j] = predictionScores[j, i];
        }
        StoreCalculatedColumn(table, _PredictionScore_ColumnName, preferredNumberOfFactors, i);
      }
    }

    public void CalculateAndStorePredictionScores(DataTable table, int preferredNumberOfFactors, int whichY)
    {
      var predictionScores = CalculatePredictionScores(table, preferredNumberOfFactors);
      var col = table.DataColumns.EnsureExistence(GetPredictionScore_ColumnName(whichY, preferredNumberOfFactors), typeof(DoubleColumn), Altaxo.Data.ColumnKind.V, GetPredictionScore_ColumnGroup());
      for (int j = 0; j < predictionScores.RowCount; j++)
      {
        col[j] = predictionScores[j, whichY];
      }

      StoreCalculatedColumn(table, _PredictionScore_ColumnName, preferredNumberOfFactors, whichY);
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="tableWithSpectraToPredict">Table holding the specta to predict values for.</param>
    /// <param name="selectedColumns">Columns selected in the source table.</param>
    /// <param name="selectedRows">Rows selected in the source table.</param>
    /// <param name="destTable">The table to store the prediction result.</param>
    /// <param name="tableContainingModel">The table where the calibration model is stored.</param>
    /// <param name="numberOfFactors">Number of factors used to predict the values.</param>
    public virtual void PredictValues(
      DataTable tableWithSpectraToPredict,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      int numberOfFactors,
      DataTable tableContainingModel,
      DataTable destTable)
    {
      var proxy = new DataTableMatrixProxyWithMultipleColumnHeaderColumns(tableWithSpectraToPredict, selectedRows, selectedColumns, new AscendingIntegerCollection());
      PredictValues(proxy, numberOfFactors, tableContainingModel, destTable);

      if(destTable.DataSource is null)
      {
        destTable.DataSource = new DimensionReductionAndRegressionPredictionDataSource(new DimensionReductionAndRegressionPredictionProcessData(new DataTableProxy(tableContainingModel), (DataTableMatrixProxyWithMultipleColumnHeaderColumns)proxy.Clone()), new DataSourceImportOptions());
      }
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="proxy">Data proxy holding the spectra to predict values for.</param>
    /// <param name="destTable">The table to store the prediction result.</param>
    /// <param name="tableContainingModel">The table where the calibration model is stored.</param>
    /// <param name="numberOfFactors">Number of factors used to predict the values.</param>
    public virtual void PredictValues(
      DataTableMatrixProxyWithMultipleColumnHeaderColumns proxy,
      int numberOfFactors,
      DataTable tableContainingModel,
      DataTable destTable)
    {
      var tableWithSpectraToPredict = proxy.DataTable;
      if (!IsDimensionReductionAndRegressionModel(tableContainingModel, out var dataSource))
        throw new InvalidOperationException($"Provided table {tableContainingModel?.Name} does not contain a multivariate model.");

      IMultivariateCalibrationModel calibModel = GetCalibrationModel(tableContainingModel);
      //      Export(modelTable, out calibModel);

      // Fill matrixX with spectra
      GetXYMatricesOfSpectralColumns(proxy, out var xOfXRaw, out var matrixXRaw, out _);

      var predictedY = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(matrixXRaw.RowCount, calibModel.NumberOfY);
      CalculatePredictedY(calibModel, xOfXRaw, matrixXRaw, numberOfFactors, predictedY, null);

      // now save the predicted y in the destination table

      var labelCol = destTable.DataColumns.EnsureExistence(_MeasurementLabel_ColumnName, typeof(DoubleColumn), ColumnKind.Label, 0);
      for (int i = 0; i < proxy.ParticipatingDataColumns.Count; i++)
      {
        labelCol[i] = proxy.ParticipatingDataColumns[i];
      }

      // the property columns in the table containing the spectra will be converted to data columns in the destinationTable
      // here, we only create them already, to make sure they appear left from the predicted columns
      var srcPC = new DataColumn[tableWithSpectraToPredict.PropertyColumnCount];
      var dstPC = new DataColumn[tableWithSpectraToPredict.PropertyColumnCount];
      for (int idxPC = 0; idxPC < tableWithSpectraToPredict.PropertyColumnCount; ++idxPC)
      {
        var src = tableWithSpectraToPredict.PropertyColumns[idxPC];
        srcPC[idxPC] = src;
        dstPC[idxPC] = destTable.DataColumns.EnsureExistence( tableWithSpectraToPredict.PropertyColumns.GetColumnName(idxPC),
                                                              tableWithSpectraToPredict.PropertyColumns[idxPC].GetType(),
                                                              tableWithSpectraToPredict.PropertyColumns.GetColumnKind(idxPC),
                                                              tableWithSpectraToPredict.PropertyColumns.GetColumnGroup(idxPC));
      }

      for (int k = 0; k < predictedY.ColumnCount; k++)
      {
        var predictedYcol = destTable.DataColumns.EnsureExistence(GetYPredicted_ColumnName(k, numberOfFactors), typeof(DoubleColumn), ColumnKind.V, 0);
        for (int i = 0; i < predictedY.RowCount; i++)
        {
          predictedYcol[i] = predictedY[i, k];

          for (int idxPC = 0; idxPC < dstPC.Length; ++idxPC)
          {
            dstPC[idxPC][i] = srcPC[idxPC][proxy.ParticipatingDataColumns[i]];
          }
        }
      }
    }

    /// <summary>
    /// Fills a provided table (should be empty) with the preprocessed spectra. The spectra are saved as columns (independently on their former orientation in the original worksheet).
    /// </summary>
    /// <param name="calibtable">The table containing the calibration model.</param>
    /// <param name="desttable">The table where to store the preprocessed spectra. Should be empty.</param>
    public void CalculatePreprocessedSpectra(
      Altaxo.Data.DataTable calibtable,
      Altaxo.Data.DataTable desttable)
    {
      if (!IsDimensionReductionAndRegressionModel(calibtable, out var dataSource))
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      IMultivariateCalibrationModel calib = GetCalibrationModel(calibtable);

      GetXYMatricesOfSpectralColumns(dataSource.ProcessData, out double[] xOfX, out var matrixX, out var matrixY);

      MultivariateRegression.PreprocessSpectraForPrediction(calib, xOfX, matrixX, out var resultXOfX, out var resultMatrixX);

      // for the new table, save the spectra as column
      var xcol = new DoubleColumn();
      for (int i = resultMatrixX.ColumnCount - 1; i >= 0; i--)
        xcol[i] = resultXOfX[i];
      desttable.DataColumns.Add(xcol, _XOfX_ColumnName, ColumnKind.X, 0);

      var columnHeaderWrappers = new (IROVector<double> Wrapper, DataColumn dstPropCol)[dataSource.ProcessData.ColumnHeaderColumnsCount];
      for (int i = 0; i < columnHeaderWrappers.Length; ++i)
      {
        var wrapper = dataSource.ProcessData.GetColumnHeaderWrapper(i);
        var srcPCol = dataSource.ProcessData.GetColumnHeaderColumn(i).Document();
        var srcTable = dataSource.ProcessData.DataTable;
        DataColumn dstPropCol;
        if (srcPCol is DataColumn dc)
        {
          
          var name = srcTable.PropCols.GetColumnName(dc);
          var kind = srcTable.PropCols.GetColumnKind(dc);
          var groupNumber = srcTable.PropCols.GetColumnGroup(dc);
          dstPropCol = desttable.PropCols.EnsureExistence(name, dc.GetType(), kind, groupNumber);
        }
        else
        {
          dstPropCol = desttable.PropCols.EnsureExistence(srcPCol.FullName, DataColumn.GetColumnTypeAppropriateForElementType(srcPCol.ItemType), ColumnKind.V, 0);
        }
        columnHeaderWrappers[i] = (wrapper, dstPropCol);
      };

      for (int n = 0; n < resultMatrixX.RowCount; n++)
      {
        var col = desttable.DataColumns.EnsureExistence(FormattableString.Invariant($"{n}"), typeof(DoubleColumn), ColumnKind.V, 0);
        for (int i = resultMatrixX.ColumnCount - 1; i >= 0; i--)
        {
          col[i] = resultMatrixX[n, i];
        }
        // add the properties
        for (int k = 0; k < columnHeaderWrappers.Length; ++k)
        {
          columnHeaderWrappers[k].dstPropCol[desttable.DataColumns.GetColumnNumber(col)] = columnHeaderWrappers[k].Wrapper[n];
        }
      }
    }

    #endregion Calculation of results

    /// <summary>
    /// Makes a PLS (a partial least squares) analysis of the table or the selected columns / rows and stores the results in a newly created table.
    /// </summary>
    /// <param name="mainDocument">The main document of the application.</param>
    /// <param name="srctable">The table where the data come from.</param>
    /// <param name="selectedColumns">The selected columns.</param>
    /// <param name="selectedRows">The selected rows.</param>
    /// <param name="selectedPropertyColumns">The selected property column(s).</param>
    /// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
    /// <param name="options">Provides information about how to preprocess the spectra.</param>
    /// <returns></returns>
    public virtual string? ExecuteAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      bool bHorizontalOrientedSpectrum,
      DimensionReductionAndRegressionOptions options
      )
    {
      var proxy = new DataTableMatrixProxyWithMultipleColumnHeaderColumns(srctable, selectedRows, selectedColumns, selectedPropertyColumns);

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      string newName = AnalysisName + " of " + Main.ProjectFolder.GetNamePart(srctable.Name);
      newName = Main.ProjectFolder.CreateFullName(srctable.Name, newName);
      var destinationTable = new Altaxo.Data.DataTable(newName);
      Current.Project.DataTableCollection.Add(destinationTable);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(destinationTable);

      return ExecuteAnalysis(mainDocument, proxy, options, destinationTable);
    }

    /// <summary>
    /// Makes a PLS (a partial least squares) analysis of the table or the selected columns / rows and stores the results in a newly created table.
    /// Here, the spectra are stored in the data columns of the proxy, the target variables are located in the ColumnHeaderColumns of the proxy.
    /// </summary>
    /// <param name="mainDocument">The main document of the application.</param>
    /// <param name="data">The matrix proxy that contains all data. Each spectrum is represented by a data column in the proxy. The target variables
    /// are stored in the column header columns of the proxy.</param>
    /// <param name="plsOptions">Provides information about the max number of factors and the calculation of cross PRESS value.</param>
    /// <param name="preprocessOptions">Provides information about how to preprocess the spectra.</param>
    /// <param name="destinationTable">Destination table to store the results into.</param>
    /// <returns></returns>
    public virtual string? ExecuteAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      DataTableMatrixProxyWithMultipleColumnHeaderColumns data,
      DimensionReductionAndRegressionOptions options,
      DataTable destinationTable
      )
    {
      var regressionResult = new DimensionReductionAndRegressionResult();
      DimensionReductionAndRegressionDataSource dataSource;
      if (destinationTable.DataSource is DimensionReductionAndRegressionDataSource ddrd)
      {
        dataSource = ddrd;
      }
      else
      {
        dataSource = new DimensionReductionAndRegressionDataSource(data, options, new DataSourceImportOptions());
        destinationTable.DataSource = dataSource;
      }

      if (!object.ReferenceEquals(dataSource.ProcessOptions, options))
      {
        dataSource.ProcessOptions = options;
      }

      // Get matrices
      GetXYMatricesOfSpectralColumns(dataSource.ProcessData, out var xOfX, out var matrixX, out var matrixY);

      if (matrixX is null || matrixY is null || xOfX is null)
      {
        return "Error getting matrixX, matrixY and xOfX";
      }

      var result = ExecuteAnalysis(mainDocument, data, matrixX, matrixY, xOfX, options, destinationTable, ref regressionResult);

      dataSource.ProcessResult = regressionResult;
      return result;
    }



    /// <summary>
    /// Makes a PLS (a partial least squares) analysis of the table or the selected columns / rows and stores the results in a newly created table.
    /// </summary>
    /// <param name="mainDocument">The main document of the application.</param>
    /// <param name="srcData">The table where the data come from.</param>
    /// <param name="matrixXRaw">The matrix of spectra (each row in the matrix is one spectrum).</param>
    /// <param name="matrixYRaw">The matrix of target variables (each row in the matrix contains the target variables for one measurement).</param>
    /// <param name="xOfXRaw">The x values of the spectra (all spectra must have the same x values).</param>
    /// <param name="options">Provides information about how to preprocess the spectra.</param>
    /// <param name="destinationTable">Destination table to store the results into.</param>
    /// <returns></returns>
    public virtual string? ExecuteAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      DataTableMatrixProxyWithMultipleColumnHeaderColumns srcData,
      IMatrix<double> matrixXRaw,
      IMatrix<double> matrixYRaw,
      double[] xOfXRaw,
      DimensionReductionAndRegressionOptions options,
      DataTable destinationTable,
      ref DimensionReductionAndRegressionResult regressionResult
      )
    {
      // Fill the Table
      using (var suspendToken = destinationTable.SuspendGetToken())
      {
        // Preprocess
        MultivariateRegression.PreprocessForAnalysis(
          options.Preprocessing,
          options.MeanScaleProcessing,
          xOfXRaw, matrixXRaw, matrixYRaw,
          out var xOfXPre, out var matrixXPre, out var matrixYPre,
          out var meanX, out var scaleX, out var meanY, out var scaleY);

        StoreXOfX(xOfXPre, destinationTable);


        StorePreprocessedData(meanX, scaleX, meanY, scaleY, destinationTable);

        // Analyze and Store

        ExecuteAnalysis(
          matrixXPre,
          matrixYPre,
          options,
          destinationTable, out var press);


        regressionResult = regressionResult with { NumberOfMeasurements = matrixXRaw.RowCount, CalculatedNumberOfFactors = press.Count - 1 };

        StorePRESSData(press, destinationTable);

        if (options.CrossValidationGroupingStrategy is not null && options.CrossValidationGroupingStrategy is not CrossValidationGroupingStrategyNone)
        {
          CalculateCrossPRESS(xOfXRaw, matrixXRaw, matrixYRaw, options, ref regressionResult, destinationTable);
        }

        StoreFRatioData(destinationTable, ref regressionResult);

        StoreOriginalY(destinationTable, srcData);

        suspendToken.Dispose();
      }


      return null;
    }
  }
}
