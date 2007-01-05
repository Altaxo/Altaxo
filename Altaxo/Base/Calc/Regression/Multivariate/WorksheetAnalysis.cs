#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// WorksheetMethods provides common utility methods for multivariate data analysis.
  /// </summary>
  public abstract class WorksheetAnalysis
  {
  
    #region Column Naming
    
    const string _XOfX_ColumnName = "XOfX";
    const string _XMean_ColumnName = "XMean";
    const string _XScale_ColumnName = "XScale";
    const string _YMean_ColumnName = "YMean";
    const string _YScale_ColumnName = "YScale";

    const string _XLoad_ColumnName = "XLoad";
    const string _XWeight_ColumnName = "XWeight";
    const string _XScore_ColumnName = "XScore";
    const string _YLoad_ColumnName = "YLoad";
    const string _CrossProduct_ColumnName = "CrossProduct";

    const string _PRESSValue_ColumnName = "PRESS";
    const string _CrossPRESSValue_ColumnName = "CrossPRESS";
    const string _PredictionScore_ColumnName = "PredictionScore";

    const string _NumberOfFactors_ColumnName = "NumberOfFactors";
    const string _MeasurementLabel_ColumnName = "MeasurementLabel";
    const string _XLabel_ColumnName = "XLabel";
    const string _YLabel_ColumnName = "YLabel";

    const string _YOriginal_ColumnName   = "YOriginal";
    const string _YPredicted_ColumnName = "YPredicted";
    const string _YCrossPredicted_ColumnName = "YCrossPredicted";
    const string _YResidual_ColumnName   = "YResidual";
    const string _YCrossResidual_ColumnName   = "YCrossResidual";
    const string _SpectralResidual_ColumnName = "SpectralResidual";
    const string _SpectralCrossResidual_ColumnName = "SpectralCrossResidual";
    const string _XLeverage_ColumnName        = "ScoreLeverage";
    const string _FRatio_ColumnName = "F-Ratio";
    const string _FProbability_ColumnName = "F-Probability";


    public static string GetXOfX_ColumnName()
    {
      return string.Format("{0}",_XOfX_ColumnName);
    }
    public static string GetXMean_ColumnName()
    {
      return string.Format("{0}",_XMean_ColumnName);
    }
    public static string GetXScale_ColumnName()
    {
      return string.Format("{0}",_XScale_ColumnName);
    }
    public static string GetYMean_ColumnName()
    {
      return string.Format("{0}",_YMean_ColumnName);
    }
    public static string GetYScale_ColumnName()
    {
      return string.Format("{0}",_YScale_ColumnName);
    }
    public static string GetMeasurementLabel_ColumnName()
    {
      return string.Format("{0}",_MeasurementLabel_ColumnName);
    }
    public static string GetNumberOfFactors_ColumnName()
    {
      return string.Format("{0}",_NumberOfFactors_ColumnName);
    }


    public static string GetXLoad_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XLoad_ColumnName,numberOfFactors);
    }

    public static string GetXLoad_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_XLoad_ColumnName,nConstituent,numberOfFactors);
    }
    public static string GetPredictionScore_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_PredictionScore_ColumnName,nConstituent,numberOfFactors);
    }
    public static string GetXScore_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XScore_ColumnName,numberOfFactors);
    }
    public static string GetXWeight_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XWeight_ColumnName,numberOfFactors);
    }
    public static string GetXWeight_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_XWeight_ColumnName, nConstituent, numberOfFactors);
    }
    public static string GetYLoad_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_YLoad_ColumnName,numberOfFactors);
    }
    public static string GetYLoad_ColumnName(int nConstituent, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_YLoad_ColumnName, nConstituent, numberOfFactors);
    }
    public static string GetCrossProduct_ColumnName()
    {
      return string.Format("{0}",_CrossProduct_ColumnName);
    }
    public static string GetCrossProduct_ColumnName(int nConstituent)
    {
      return string.Format("{0}{1}",_CrossProduct_ColumnName,nConstituent);
    }
    public static string GetPRESSValue_ColumnName()
    {
      return string.Format("{0}",_PRESSValue_ColumnName);
    }
    public static string GetCrossPRESSValue_ColumnName()
    {
      return string.Format("{0}",_CrossPRESSValue_ColumnName);
    }
    /// <summary>
    /// Gets the column name of a Y-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_YResidual_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a Y-Cross-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYCrossResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_YCrossResidual_ColumnName, whichY, numberOfFactors);
    }
    /// <summary>
    /// Gets the column name of a Y-Original column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYOriginal_ColumnName(int whichY)
    {
      return string.Format("{0}{1}",_YOriginal_ColumnName, whichY);
    }

    /// <summary>
    /// Gets the column name of a Y-Predicted column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYPredicted_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_YPredicted_ColumnName, whichY, numberOfFactors);
    }

    /// <summary>
    /// Gets the column name of a Y-Cross-Predicted column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYCrossPredicted_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_YCrossPredicted_ColumnName, whichY, numberOfFactors);
    }
    
    /// <summary>
    /// Gets the column name of a X-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_SpectralResidual_ColumnName, whichY, numberOfFactors);
    }

  
    /// <summary>
    /// Gets the column name of a X-Crossvalidated spectral Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXCrossResidual_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_SpectralCrossResidual_ColumnName, whichY, numberOfFactors);
    }


    /// <summary>
    /// Gets the column name of a X-Leverage column
    /// </summary>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXLeverage_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XLeverage_ColumnName, numberOfFactors);
    }


    /// <summary>
    /// Gets the column name of a X-Leverage column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXLeverage_ColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("{0}{1}.{2}",_XLeverage_ColumnName, whichY, numberOfFactors);
    }


    protected static void NotFound(string name)
    {
      throw new ArgumentException("Column " + name + " not found in the table.");
    }
    #endregion

    #region Column Grouping

    const int _NumberOfFactors_ColumnGroup = 4;
    const int _FRatio_ColumnGroup = 4;
    const int _FProbability_ColumnGroup = 4;

    const int _MeasurementLabel_ColumnGroup = 5;

    const int _YPredicted_ColumnGroup = 5;
    const int _YResidual_ColumnGroup = 5;
    const int _XLeverage_ColumnGroup = 5;
    const int _PredictionScore_ColumnGroup =0;


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

    #endregion

    #region Abstract members
   
    /// <summary>
    /// Execute an analysis and stores the result in the provided table.
    /// </summary>
    /// <param name="matrixX">The matrix of spectra (horizontal oriented), centered and preprocessed.</param>
    /// <param name="matrixY">The matrix of concentrations, centered.</param>
    /// <param name="plsOptions">Information how to perform the analysis.</param>
    /// <param name="plsContent">A structure to store information about the results of the analysis.</param>
    /// <param name="table">The table where to store the results to.</param>
    /// <param name="press">On return, gives a vector holding the PRESS values of the analysis.</param>
    public virtual void ExecuteAnalysis(
      IMatrix matrixX,
      IMatrix matrixY,
      MultivariateAnalysisOptions plsOptions,
      MultivariateContentMemento plsContent,
      DataTable table,
      out IROVector press
      )
    {
      int numFactors = Math.Min(matrixX.Columns,plsOptions.MaxNumberOfFactors);
      MultivariateRegression regress = this.CreateNewRegressionObject();
      regress.AnalyzeFromPreprocessed(matrixX,matrixY,numFactors);
      plsContent.NumberOfFactors = regress.NumberOfFactors;
      plsContent.CrossValidationType = plsOptions.CrossPRESSCalculation;
      press = regress.GetPRESSFromPreprocessed(matrixX);

      Import(regress.CalibrationModel,table);
 
    }

  
    public abstract MultivariateRegression CreateNewRegressionObject();



    /// <summary>
    /// Stores the calibrationSet into the data table table.
    /// </summary>
    /// <param name="calibrationSet">The data source.</param>
    /// <param name="table">The table where to store the data of the calibrationSet.</param>
    public abstract void Import(IMultivariateCalibrationModel calibrationSet,DataTable table);


    /// <summary>
    /// Calculate the cross PRESS values and stores the results in the provided table.
    /// </summary>
    /// <param name="xOfX">Vector of spectral wavelengths. Necessary to divide the spectras in different regions.</param>
    /// <param name="matrixX">Matrix of spectra (horizontal oriented).</param>
    /// <param name="matrixY">Matrix of concentrations.</param>
    /// <param name="plsOptions">Analysis options.</param>
    /// <param name="plsContent">Information about this analysis.</param>
    /// <param name="table">Table to store the results.</param>
    public virtual void CalculateCrossPRESS(
      IROVector xOfX,
      IMatrix matrixX,
      IMatrix matrixY,
      MultivariateAnalysisOptions plsOptions,
      MultivariateContentMemento plsContent,
      DataTable table
      )
    {
      IROVector crossPRESSMatrix;
      
      Altaxo.Data.DoubleColumn crosspresscol = new Altaxo.Data.DoubleColumn();

      double meanNumberOfExcludedSpectra = 0;
      if(plsOptions.CrossPRESSCalculation!=CrossPRESSCalculationType.None)
      {
        // now a cross validation - this can take a long time for bigger matrices
        
        MultivariateRegression.GetCrossPRESS(
          xOfX,matrixX,matrixY,plsOptions.MaxNumberOfFactors,GetGroupingStrategy(plsOptions),
          plsContent.SpectralPreprocessing,
          this.CreateNewRegressionObject(),
          out crossPRESSMatrix);

        VectorMath.Copy(crossPRESSMatrix,DataColumnWrapper.ToVector(crosspresscol,crossPRESSMatrix.Length));
       
        table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);

        plsContent.MeanNumberOfMeasurementsInCrossPRESSCalculation = plsContent.NumberOfMeasurements-meanNumberOfExcludedSpectra;
      }
      else
      {
        table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);
      }
     
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="mcalib"></param>
    /// <param name="groupingStrategy"></param>
    /// <param name="preprocessOptions"></param>
    /// <param name="xOfX"></param>
    /// <param name="matrixX">Matrix of horizontal spectra, centered and preprocessed.</param>
    /// <param name="matrixY">Matrix of concentrations, centered.</param>
    /// <param name="numberOfFactors"></param>
    /// <param name="predictedY"></param>
    /// <param name="spectralResiduals"></param>
    public virtual void CalculateCrossPredictedY(
      IMultivariateCalibrationModel mcalib,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      IROVector xOfX,
      IMatrix matrixX,
      IMatrix matrixY,
      int numberOfFactors, 
      IMatrix predictedY, 
      IMatrix spectralResiduals)
    {
      MultivariateRegression.GetCrossYPredicted(xOfX,
        matrixX,matrixY,numberOfFactors,groupingStrategy,preprocessOptions,
        this.CreateNewRegressionObject(),
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
    /// Calculates the leverage of the spectral data.
    /// </summary>
    /// <param name="table">Table where the calibration model is stored.</param>
    /// <param name="numberOfFactors">Number of factors used to calculate leverage.</param>
    public virtual void CalculateXLeverage(
      DataTable table, int numberOfFactors)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      IMultivariateCalibrationModel calib = this.GetCalibrationModel(table);
      IMatrix matrixX = GetRawSpectra(plsMemo);

      MultivariateRegression.PreprocessSpectraForPrediction(calib,plsMemo.SpectralPreprocessing,matrixX);

      MultivariateRegression regress = this.CreateNewRegressionObject();
      regress.SetCalibrationModel(calib);

      IROMatrix xLeverage = regress.GetXLeverageFromRaw(matrixX,numberOfFactors);

      for(int i=0;i<xLeverage.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        MatrixMath.SetColumn(xLeverage,i,DataColumnWrapper.ToVertMatrix(col,xLeverage.Rows),0);
        table.DataColumns.Add(
          col,
          xLeverage.Columns==1 ? GetXLeverage_ColumnName(numberOfFactors) : GetXLeverage_ColumnName(i,numberOfFactors),
          Altaxo.Data.ColumnKind.V,GetXLeverage_ColumnGroup());
      }
    }



    /// <summary>
    /// Calculates the prediction scores. In case of a single y-variable, the prediction score is a vector of the same length than a spectrum.
    /// Multiplying the prediction score (dot product) with a spectrum, it yields the predicted y-value (of cause mean-centered).
    /// </summary>
    /// <param name="table">The table where the calibration model is stored.</param>
    /// <param name="preferredNumberOfFactors">Number of factors used to calculate the prediction scores.</param>
    /// <returns>A matrix with the prediction scores. Each score (for one y-value) is a row in the matrix.</returns>
    public virtual IROMatrix CalculatePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      MultivariateRegression regress = this.CreateNewRegressionObject();
      IMultivariateCalibrationModel model = this.GetCalibrationModel(table);
      regress.SetCalibrationModel(model);
      return regress.GetPredictionScores(preferredNumberOfFactors);
    }


    /// <summary>
    /// For a given set of spectra, predicts the y-values and stores them in the matrix <c>predictedY</c>
    /// </summary>
    /// <param name="mcalib">The calibration model of the analysis.</param>
    /// <param name="preprocessOptions">The information how to preprocess the spectra.</param>
    /// <param name="matrixX">The matrix of spectra to predict. Each spectrum is a row in the matrix.</param>
    /// <param name="numberOfFactors">The number of factors used for prediction.</param>
    /// <param name="predictedY">On return, this matrix holds the predicted y-values. Each row in this matrix corresponds to the same row (spectrum) in matrixX.</param>
    /// <param name="spectralResiduals">If you set this parameter to a appropriate matrix, the spectral residuals will be stored in this matrix. Set this parameter to null if you don't need the residuals.</param>
    public virtual void CalculatePredictedY(
      IMultivariateCalibrationModel mcalib,
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX,
      int numberOfFactors, 
      MatrixMath.BEMatrix  predictedY, 
      IMatrix spectralResiduals)
    {
     

      MultivariateRegression.PreprocessSpectraForPrediction(mcalib,preprocessOptions,matrixX);

      
      MultivariateRegression regress = this.CreateNewRegressionObject();
      regress.SetCalibrationModel(mcalib);

      regress.PredictedYAndSpectralResidualsFromPreprocessed(matrixX,numberOfFactors,predictedY,spectralResiduals);

      MultivariateRegression.PostprocessY(mcalib.PreprocessingModel,predictedY);
    }  


    #endregion

    #region static Helper methods

    /// <summary>
    /// Creates the corresponding grouping strategy out of the CrossPRESSCalculation enumeration in plsOptions.
    /// </summary>
    /// <param name="plsOptions">The options for PLS analysis</param>
    /// <returns>The used grouping strategy. Returns null if no cross validation is choosen.</returns>
    public static ICrossValidationGroupingStrategy GetGroupingStrategy(MultivariateAnalysisOptions plsOptions)
    {
      return GetGroupingStrategy(plsOptions.CrossPRESSCalculation);
    }

    /// <summary>
    /// Creates the corresponding grouping strategy out of the CrossPRESSCalculation enumeration in plsOptions.
    /// </summary>
    /// <param name="crossValidationType">Type of cross validation.</param>
    /// <returns>The used grouping strategy. Returns null if no cross validation is choosen.</returns>
    public static ICrossValidationGroupingStrategy GetGroupingStrategy(CrossPRESSCalculationType crossValidationType)
    {
      switch(crossValidationType)
      {
        case CrossPRESSCalculationType.ExcludeEveryMeasurement:
          return new ExcludeSingleMeasurementsGroupingStrategy();
        case CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements:
          return new ExcludeGroupsGroupingStrategy();
        case CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements:
          return new ExcludeHalfObservationsGroupingStrategy();
      }
      return null;
    }


    /// <summary>
    /// Using the information in the plsMemo, gets the matrix of original spectra. The spectra are horizontal in the matrix, i.e. each spectra is a matrix row.
    /// </summary>
    /// <param name="plsMemo">The PLS memento containing the information about the location of the original data.</param>
    /// <returns>The matrix of the original spectra.</returns>
    public static IMatrix GetRawSpectra(MultivariateContentMemento plsMemo)
    {
      string tablename = plsMemo.TableName;

      Altaxo.Data.DataTable srctable = Current.Project.DataTableCollection[tablename];

      if(srctable==null)
        throw new ApplicationException(string.Format("Table[{0}] containing original spectral data not found!",tablename));
    
      Altaxo.Collections.IAscendingIntegerCollection spectralIndices = plsMemo.SpectralIndices;
      Altaxo.Collections.IAscendingIntegerCollection measurementIndices = plsMemo.MeasurementIndices;

      
      MatrixMath.BEMatrix matrixX = 
        new MatrixMath.BEMatrix(measurementIndices.Count,spectralIndices.Count);
      
 
      return GetRawSpectra(srctable,plsMemo.SpectrumIsRow,spectralIndices,measurementIndices);
    }

    
    /// <summary>
    /// Fills a matrix with the selected data of a table.
    /// </summary>
    /// <param name="srctable">The source table where the data for the spectra are located.</param>
    /// <param name="spectrumIsRow">True if the spectra in the table are organized horizontally, false if spectra are vertically oriented.</param>
    /// <param name="spectralIndices">The selected indices wich indicate all (wavelength, frequencies, etc.) that belong to one spectrum. If spectrumIsRow==true, this are the selected column indices, otherwise the selected row indices.</param>
    /// <param name="measurementIndices">The indices of all measurements (spectra) selected.</param>
    /// <returns>The matrix of spectra. In this matrix the spectra are horizonally organized (each row is one spectrum).</returns>
    public static IMatrix GetRawSpectra(Altaxo.Data.DataTable srctable, bool spectrumIsRow, Altaxo.Collections.IAscendingIntegerCollection spectralIndices, Altaxo.Collections.IAscendingIntegerCollection measurementIndices)
    {
      if(srctable==null)
        throw new ArgumentException("Argument srctable may not be null");
      
      MatrixMath.BEMatrix matrixX = 
        new MatrixMath.BEMatrix(measurementIndices.Count,spectralIndices.Count);
      

      if(spectrumIsRow)
      {
        for(int i=0;i<spectralIndices.Count;i++)
        {
          // labelColumnOfX[i] = spectralIndices[i];
          Altaxo.Data.INumericColumn col = srctable[spectralIndices[i]] as Altaxo.Data.INumericColumn;
          for(int j=0;j<measurementIndices.Count;j++)
          {
            matrixX[j,i] = col[measurementIndices[j]];
          }
        } // end fill in x-values
      }
      else // vertical oriented spectrum
      {
        for(int i=0;i<spectralIndices.Count;i++)
        {
          // labelColumnOfX[i] = spectralIndices[i];
        }
        for(int i=0;i<measurementIndices.Count;i++)
        {
          Altaxo.Data.INumericColumn col = srctable[measurementIndices[i]] as Altaxo.Data.INumericColumn;
          for(int j=0;j<spectralIndices.Count;j++)
          {
            matrixX[i,j] = col[spectralIndices[j]];
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
      if(srctable==null)
        throw new ArgumentException("Argument srctable may not be null");
     
      int group;
      Altaxo.Data.INumericColumn col;

      if(spectrumIsRow)
      {
        group = srctable.DataColumns.GetColumnGroup(spectralIndices[0]);

        col = srctable.PropertyColumns.FindXColumnOfGroup(group) as Altaxo.Data.INumericColumn;
      }
      else // vertical oriented spectrum
      {
        group = srctable.DataColumns.GetColumnGroup(measurementIndices[0]);

        col = srctable.DataColumns.FindXColumnOfGroup(group) as Altaxo.Data.INumericColumn;
      }

      if(col==null)
        col = new IndexerColumn();

      double[] result = new double[spectralIndices.Count];

      for(int i=0;i<spectralIndices.Count;i++)
        result[i] = col[spectralIndices[i]];

      return result;
    }

    /// <summary>
    /// Using the information in the plsMemo, gets the matrix of original Y (concentration) data.
    /// </summary>
    /// <param name="plsMemo">The PLS mememto containing the information where to find the original data.</param>
    /// <returns>Matrix of orignal Y (concentration) data.</returns>
    public static IMatrix GetOriginalY(MultivariateContentMemento plsMemo)
    {
      string tablename = plsMemo.TableName;

      Altaxo.Data.DataTable srctable = Current.Project.DataTableCollection[tablename];

      if(srctable==null)
        throw new ApplicationException(string.Format("Table[{0}] containing original spectral data not found!",tablename));

      Altaxo.Data.DataColumnCollection concentration = plsMemo.SpectrumIsRow ? srctable.DataColumns : srctable.PropertyColumns;
      Altaxo.Collections.IAscendingIntegerCollection concentrationIndices = plsMemo.ConcentrationIndices;
      Altaxo.Collections.IAscendingIntegerCollection measurementIndices = plsMemo.MeasurementIndices;

      // fill in the y-values
      MatrixMath.BEMatrix matrixY = new MatrixMath.BEMatrix(measurementIndices.Count,concentrationIndices.Count);
      for(int i=0;i<concentrationIndices.Count;i++)
      {
        Altaxo.Data.INumericColumn col = concentration[concentrationIndices[i]] as Altaxo.Data.INumericColumn;
        for(int j=0;j<measurementIndices.Count;j++)
        {
          matrixY[j,i] = col[measurementIndices[j]];
        }
      } // end fill in yvalues

      return matrixY;
    }


    /// <summary>
    /// Get the matrix of x and y values (raw data).
    /// </summary>
    /// <param name="srctable">The table where the data come from.</param>
    /// <param name="selectedColumns">The selected columns.</param>
    /// <param name="selectedRows">The selected rows.</param>
    /// <param name="selectedPropertyColumns">The selected property column(s).</param>
    /// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
    /// <param name="matrixX">On return, gives the matrix of spectra (each spectra is a row in the matrix).</param>
    /// <param name="matrixY">On return, gives the matrix of y-values (each measurement is a row in the matrix).</param>
    /// <param name="plsContent">Holds information about the analysis results.</param>
    /// <param name="xOfX">On return, this is the vector of values corresponding to each spectral bin, i.e. wavelength values, frequencies etc.</param>
    /// <returns></returns>
    public static string GetXYMatrices(
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      bool bHorizontalOrientedSpectrum,
      MultivariateContentMemento plsContent,
      out IMatrix matrixX,
      out IMatrix matrixY,
      out IROVector xOfX
      )
    {
      matrixX=null;
      matrixY=null;
      xOfX=null;
      plsContent.SpectrumIsRow = bHorizontalOrientedSpectrum;


      Altaxo.Data.DataColumn xColumnOfX=null;
      Altaxo.Data.DataColumn labelColumnOfX=new Altaxo.Data.DoubleColumn();


      Altaxo.Data.DataColumnCollection concentration = bHorizontalOrientedSpectrum ? srctable.DataColumns : srctable.PropertyColumns;

      

      // we presume for now that the spectrum is horizontally,
      // if not we exchange the collections later

      AscendingIntegerCollection numericDataCols = new AscendingIntegerCollection();
      AscendingIntegerCollection numericDataRows = new AscendingIntegerCollection();
      AscendingIntegerCollection concentrationIndices = new AscendingIntegerCollection();

      AscendingIntegerCollection spectralIndices = bHorizontalOrientedSpectrum ? numericDataCols : numericDataRows;
      AscendingIntegerCollection measurementIndices = bHorizontalOrientedSpectrum ? numericDataRows : numericDataCols;


      plsContent.ConcentrationIndices = concentrationIndices;
      plsContent.MeasurementIndices   = measurementIndices;
      plsContent.SpectralIndices      = spectralIndices;
      plsContent.SpectrumIsRow        = bHorizontalOrientedSpectrum;
      plsContent.TableName            = srctable.Name;

      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      // this is the number of columns (for now), but it can be less than this in case
      // not all columns are numeric
      int prenumcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
      // check for the number of numeric columns
      int numcols = 0;
      for(int i=0;i<prenumcols;i++)
      {
        int idx = bUseSelectedColumns ? selectedColumns[i] : i;
        if(srctable[idx] is Altaxo.Data.INumericColumn)
        {
          numericDataCols.Add(idx);  
          numcols++;
        }
      }

      // check the number of rows
      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);
      int numrows;
      if(bUseSelectedRows)
      {
        numrows = selectedRows.Count;
        numericDataRows.Add(selectedRows);
      }
      else
      {
        numrows = 0;
        for(int i=0;i<numcols;i++)
        {
          int idx = bUseSelectedColumns ? selectedColumns[i] : i;
          numrows = Math.Max(numrows,srctable[idx].Count);
        }     
        numericDataRows.Add(new IntegerRangeAsCollection(0,numrows));
      }

      if(bHorizontalOrientedSpectrum)
      {
        if(numcols<2)
          return "At least two numeric columns are neccessary to do Partial Least Squares (PLS) analysis!";

    

        // check that the selected columns are in exactly two groups
        // the group which has more columns is then considered to have
        // the spectrum, the other group is the y-values
        int group0=-1;
        int group1=-1;
        int groupcount0=0;
        int groupcount1=0;
      

        for(int i=0;i<numcols;i++)
        {
          int grp = srctable.DataColumns.GetColumnGroup(numericDataCols[i]);
          

          if(group0<0)
          {
            group0=grp;
            groupcount0=1;
          }
          else if(group0==grp)
          {
            groupcount0++;
          }
          else if(group1<0)
          {
            group1=grp;
            groupcount1=1;
          }
          else if(group1==grp)
          {
            groupcount1++;
          }
          else
          {
            return "The columns you selected must be members of two groups (y-values and spectrum), but actually there are more than two groups!";
          }
        } // end for all columns
    
        if(groupcount1<=0)
          return "The columns you selected must be members of two groups (y-values and spectrum), but actually only one group was detected!";

        if(groupcount1<groupcount0)
        {
          int hlp;
          hlp = groupcount1;
          groupcount1=groupcount0;
          groupcount0=hlp;

          hlp = group1;
          group1=group0;
          group0=hlp;
        }
          
        // group0 is now the group of y-values (concentrations)
        // group1 is now the group of x-values (spectra)

        // we delete group0 from numericDataCols and add it to concentrationIndices

        for(int i=numcols-1;i>=0;i--)
        {
          int index = numericDataCols[i];
          if(group0==srctable.DataColumns.GetColumnGroup(index))
          {
            numericDataCols.Remove(index);
            concentrationIndices.Add(index);
          }
        }

        // fill the corresponding X-Column of the spectra
        xColumnOfX = Altaxo.Data.DataColumn.CreateColumnOfSelectedRows(
          srctable.PropertyColumns.FindXColumnOfGroup(group1),
          spectralIndices);

        

      }
      else // vertically oriented spectrum -> one spectrum is one data column
      {
        // we have to exchange measurementIndices and 

        // if PLS on columns, than we should have property columns selected
        // that designates the y-values
        // so count all property columns

        

        bool bUseSelectedPropCols = (null!=selectedPropertyColumns && 0!=selectedPropertyColumns.Count);
        // this is the number of property columns (for now), but it can be less than this in case
        // not all columns are numeric
        int prenumpropcols = bUseSelectedPropCols ? selectedPropertyColumns.Count : srctable.PropCols.ColumnCount;
        // check for the number of numeric property columns
        for(int i=0;i<prenumpropcols;i++)
        {
          int idx = bUseSelectedPropCols ? selectedPropertyColumns[i] : i;
          if(srctable.PropCols[idx] is Altaxo.Data.INumericColumn)
          {
            concentrationIndices.Add(idx);
          }
        }

        if(concentrationIndices.Count<1)
          return "At least one numeric property column must exist to hold the y-values!";
    
        // fill the corresponding X-Column of the spectra
        xColumnOfX = Altaxo.Data.DataColumn.CreateColumnOfSelectedRows(
          srctable.DataColumns.FindXColumnOf(srctable[measurementIndices[0]]),spectralIndices);

      } // else vertically oriented spectrum


      IVector xOfXRW = VectorMath.CreateExtensibleVector(xColumnOfX.Count);
      xOfX = xOfXRW;
      if(xColumnOfX is INumericColumn)
      {
        for(int i=0;i<xOfX.Length;i++)
          xOfXRW[i] = ((INumericColumn)xColumnOfX)[i];
      }
      else
      {
        for(int i=0;i<xOfX.Length;i++)
          xOfXRW[i] = spectralIndices[i];
      }

      // now fill the matrix

   
      // fill in the y-values
      matrixY = new MatrixMath.BEMatrix(measurementIndices.Count,concentrationIndices.Count);
      for(int i=0;i<concentrationIndices.Count;i++)
      {
        Altaxo.Data.INumericColumn col = concentration[concentrationIndices[i]] as Altaxo.Data.INumericColumn;
        for(int j=0;j<measurementIndices.Count;j++)
        {
          matrixY[j,i] = col[measurementIndices[j]];
        }
      } // end fill in yvalues

      matrixX = new MatrixMath.BEMatrix(measurementIndices.Count,spectralIndices.Count);
      if(bHorizontalOrientedSpectrum)
      {
        for(int i=0;i<spectralIndices.Count;i++)
        {
          labelColumnOfX[i] = spectralIndices[i];
          Altaxo.Data.INumericColumn col = srctable[spectralIndices[i]] as Altaxo.Data.INumericColumn;
          for(int j=0;j<measurementIndices.Count;j++)
          {
            matrixX[j,i] = col[measurementIndices[j]];
          }
        } // end fill in x-values
      }
      else // vertical oriented spectrum
      {
        for(int i=0;i<spectralIndices.Count;i++)
        {
          labelColumnOfX[i] = spectralIndices[i];
        }
        for(int i=0;i<measurementIndices.Count;i++)
        {
          Altaxo.Data.INumericColumn col = srctable[measurementIndices[i]] as Altaxo.Data.INumericColumn;
          for(int j=0;j<spectralIndices.Count;j++)
          {
            matrixX[i,j] = col[spectralIndices[j]];
          }
        } // end fill in x-values
      }

      return null;
    }

    
   

    /// <summary>
    /// This maps the indices of a master x column to the indices of a column to map.
    /// </summary>
    /// <param name="xmaster">The master column containing x-values, for instance the spectral wavelength of the PLS calibration model.</param>
    /// <param name="xtomap">The column to map containing x-values, for instance the spectral wavelength of an unknown spectra to predict.</param>
    /// <param name="failureMessage">In case of a mapping error, contains detailed information about the error.</param>
    /// <returns>The indices of the mapping column that matches those of the master column. Contains as many indices as items in xmaster. In case of mapping error, returns null.</returns>
    public static Altaxo.Collections.AscendingIntegerCollection MapSpectralX(IROVector xmaster, IROVector xtomap, out string failureMessage)
    {
      failureMessage = null;
      int mastercount = xmaster.Length;

      int mapcount = xtomap.Length;

      if(mapcount<mastercount)
      {
        failureMessage = string.Format("More items to map ({0} than available ({1}",mastercount, mapcount);
        return null;
      }

      Altaxo.Collections.AscendingIntegerCollection result = new Altaxo.Collections.AscendingIntegerCollection();
      // return an empty collection if there is nothing to map
      if(mastercount==0)
        return result;

      // there is only one item to map - we can not check this - return a 1:1 map
      if(mastercount==1)
      {
        result.Add(0);
        return result;
      }


      // presumtion here (checked before): mastercount>=2, mapcount>=1

      double distanceback, distancecurrent, distanceforward;
      int i,j;
      for(i=0,j=0;i<mastercount && j<mapcount;j++)
      {
        distanceback    = j==0 ? double.MaxValue : Math.Abs(xtomap[j-1]-xmaster[i]);
        distancecurrent = Math.Abs(xtomap[j]-xmaster[i]);
        distanceforward = (j+1)>=mapcount ? double.MaxValue : Math.Abs(xtomap[j+1]-xmaster[i]);

        if(distanceback<distancecurrent)
        {
          failureMessage = string.Format("Mapping error - distance of master[{0}] to current map[{1}] is greater than to previous map[{2}]",i,j,j-1);
          return null;
        }
        else if(distanceforward<distancecurrent) 
          continue;
        else
        {
          result.Add(j);
          i++;
        }
      }

      if(i!=mastercount)
      {
        failureMessage =  string.Format("Mapping error- no mapping found for current master[{0}]",i-1);
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
      if(0!=selectedColumns.Count)
      {
        for(int i=0;i<selectedColumns.Count;i++)
        {
          int idx = selectedColumns[i];
          if(!(srctable[idx] is Altaxo.Data.INumericColumn))
          {
            selectedColumns.Remove(idx);
          }
        }
      }
      else // if no columns where selected, select all that are numeric
      {
        int end = srctable.DataColumnCount;
        for(int i=0;i<end;i++)
        {
          if(srctable[i] is Altaxo.Data.INumericColumn)
            selectedColumns.Add(i);
        }
      }


      // if now rows selected, then test the max row count of the selected columns
      // and add it

      // check the number of rows

      if(0==selectedRows.Count)
      {
        int numrows = 0;
        int end = selectedColumns.Count;
        for(int i=0;i<end;i++)
        {
          int idx = selectedColumns[i];
          numrows = Math.Max(numrows,srctable[idx].Count);
        }     
        selectedRows.Add(new IntegerRangeAsCollection(0,numrows));
      }
    }


    #endregion

    #region Storing results
    
    public virtual void StorePreprocessedData(
      IROVector meanX, IROVector scaleX,
      IROVector meanY, IROVector scaleY,
      DataTable table)
    {
      // Store X-Mean and X-Scale
      Altaxo.Data.DoubleColumn colXMean = new Altaxo.Data.DoubleColumn();
      Altaxo.Data.DoubleColumn colXScale = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<meanX.Length;i++)
      {
        colXMean[i] = meanX[i];
        colXScale[i] = scaleX[i];
      }

      table.DataColumns.Add(colXMean,_XMean_ColumnName,Altaxo.Data.ColumnKind.V,0);
      table.DataColumns.Add(colXScale,_XScale_ColumnName,Altaxo.Data.ColumnKind.V,0);


      // store the y-mean and y-scale
      Altaxo.Data.DoubleColumn colYMean = new Altaxo.Data.DoubleColumn();
      Altaxo.Data.DoubleColumn colYScale = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<meanY.Length;i++)
      {
        colYMean[i] = meanY[i];
        colYScale[i] = 1;
      }

      table.DataColumns.Add(colYMean, _YMean_ColumnName,Altaxo.Data.ColumnKind.V,1);
      table.DataColumns.Add(colYScale,_YScale_ColumnName,Altaxo.Data.ColumnKind.V,1);
    }


    public virtual void StoreXOfX(IROVector xOfX, DataTable table)
    {
      DoubleColumn xColOfX = new DoubleColumn();
      VectorMath.Copy(xOfX,DataColumnWrapper.ToVector(xColOfX,xOfX.Length));
      table.DataColumns.Add(xColOfX,_XOfX_ColumnName,Altaxo.Data.ColumnKind.X,0);
    }

    public virtual void StoreNumberOfFactors(
      int nNumberOfFactors, 
      DataTable table)
    {
      // add a NumberOfFactors columm
      DoubleColumn xNumFactor = table[_NumberOfFactors_ColumnName] as DoubleColumn;
      if(xNumFactor==null)
      {
        xNumFactor = new Altaxo.Data.DoubleColumn();   
        table.DataColumns.Add(xNumFactor,_NumberOfFactors_ColumnName,Altaxo.Data.ColumnKind.X,_NumberOfFactors_ColumnGroup);
      }

      xNumFactor.Suspend();
      for(int i=0;i<nNumberOfFactors;i++)
      { 
        xNumFactor[i]=i;
      }
      xNumFactor.Resume();

    }
    public virtual void StorePRESSData(
      IROVector PRESS, 
      DataTable table)
    {
      StoreNumberOfFactors(PRESS.Length,table);
    
      Altaxo.Data.DoubleColumn presscol = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<PRESS.Length;i++)
        presscol[i] = PRESS[i];
      table.DataColumns.Add(presscol,GetPRESSValue_ColumnName(), Altaxo.Data.ColumnKind.V,4);

    }

    public virtual void StoreFRatioData(
      DataTable table,
      MultivariateContentMemento plsContent)
    {
      DoubleColumn pressColumn = table[GetPRESSValue_ColumnName()] as DoubleColumn;
      DoubleColumn crossPRESSColumn = table[GetCrossPRESSValue_ColumnName()] as DoubleColumn;
      
      IROVector press;
      double meanNumberOfIncludedSpectra = plsContent.NumberOfMeasurements;

      if(crossPRESSColumn!=null && crossPRESSColumn.Count>0)
      {
        press = DataColumnWrapper.ToROVector(crossPRESSColumn);
        meanNumberOfIncludedSpectra = plsContent.MeanNumberOfMeasurementsInCrossPRESSCalculation;
      }
      else if(pressColumn!=null && pressColumn.Count>0)
      {
        press = DataColumnWrapper.ToROVector(pressColumn);
        meanNumberOfIncludedSpectra = plsContent.NumberOfMeasurements;
      }
      else
        return;
      
      
      // calculate the F-ratio and the F-Probability
      int numberOfSignificantFactors = press.Length;
      double pressMin=double.MaxValue;
      for(int i=0;i<press.Length;i++)
        pressMin = Math.Min(pressMin,press[i]);
      DoubleColumn fratiocol = new DoubleColumn();
      DoubleColumn fprobcol = new DoubleColumn();
      for(int i=0;i<press.Length;i++)
      {
        double fratio = press[i]/pressMin;
        double fprob  = Calc.Probability.FDistribution.CDF(fratio,meanNumberOfIncludedSpectra,meanNumberOfIncludedSpectra);
        fratiocol[i] = fratio;
        fprobcol[i]  = fprob;
        if(fprob<0.75 && numberOfSignificantFactors>i)
          numberOfSignificantFactors = i;
      }
      plsContent.PreferredNumberOfFactors = numberOfSignificantFactors;
      table.DataColumns.Add(fratiocol,_FRatio_ColumnName,Altaxo.Data.ColumnKind.V,_FRatio_ColumnGroup);
      table.DataColumns.Add(fprobcol,_FProbability_ColumnName,Altaxo.Data.ColumnKind.V,_FProbability_ColumnGroup);
    }


    public void StoreOriginalY(
      DataTable table,
      MultivariateContentMemento plsContent
      )
    {
      IMatrix matrixY = GetOriginalY(plsContent);
      IMultivariateCalibrationModel calib = GetCalibrationModel(table);

      // add a label column for the measurement number
      Altaxo.Data.DoubleColumn measurementLabel = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<plsContent.MeasurementIndices.Count;i++)
        measurementLabel[i] = plsContent.MeasurementIndices[i];
      table.DataColumns.Add(measurementLabel,_MeasurementLabel_ColumnName,Altaxo.Data.ColumnKind.Label,_MeasurementLabel_ColumnGroup);

      // now add the original Y-Columns
      for(int i=0;i<matrixY.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<matrixY.Rows;j++)
          col[j]=matrixY[j,i];
        table.DataColumns.Add(col,_YOriginal_ColumnName+i.ToString(),Altaxo.Data.ColumnKind.X,5+i);
      }
      
     
    }

    #endregion

    #region Calculation of results
    /// <summary>
    /// Calculated the predicted y for the component given by <c>whichY</c> and for the given number of factors.
    /// </summary>
    /// <param name="table">The table containing the PLS model.</param>
    /// <param name="whichY">The number of the y component.</param>
    /// <param name="numberOfFactors">Number of factors for calculation.</param>
    public virtual void CalculateYPredicted(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table,whichY,numberOfFactors,true,false,false);
    }

    /// <summary>
    /// Calculates the y-residuals (concentration etc.).
    /// </summary>
    /// <param name="table">The table where the calibration model is stored.</param>
    /// <param name="whichY">Number of the y-value.</param>
    /// <param name="numberOfFactors">The number of factors used for prediction and then for the residual calculation.</param>
    public virtual void CalculateYResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table,whichY,numberOfFactors,false,true,false);
    }

   


    public virtual void CalculateXResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table,whichY,numberOfFactors,false,false,true);
    }


   
    public  virtual void CalculateCrossPredictedAndResidual(
      DataTable table,
      int whichY,
      int numberOfFactors,
      bool saveYPredicted,
      bool saveYResidual,
      bool saveXResidual)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      IMultivariateCalibrationModel calib = GetCalibrationModel(table);
      //      Export(table,out calib);


     
      IMatrix matrixX = GetRawSpectra(plsMemo);
      IMatrix matrixY = GetOriginalY(plsMemo);

      MatrixMath.BEMatrix predictedY = new MatrixMath.BEMatrix(matrixX.Rows,calib.NumberOfY);
      MatrixMath.BEMatrix spectralResiduals = new MatrixMath.BEMatrix(matrixX.Rows,1);
      CalculateCrossPredictedY(calib,
        GetGroupingStrategy(plsMemo.CrossValidationType),
        plsMemo.SpectralPreprocessing,
        calib.PreprocessingModel.XOfX,
        matrixX,
        matrixY,
        numberOfFactors,
        predictedY,
        spectralResiduals);

      if(saveYPredicted)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYCrossPredicted_ColumnName(whichY,numberOfFactors);
        Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<predictedY.Rows;i++)
          ycolumn[i] = predictedY[i,whichY];
      
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetYCrossPredicted_ColumnGroup());
      }

      // subract the original y data
      matrixY = GetOriginalY(plsMemo);
      MatrixMath.SubtractColumn(predictedY,matrixY,whichY,predictedY);

      if(saveYResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYCrossResidual_ColumnName(whichY,numberOfFactors);
        Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<predictedY.Rows;i++)
          ycolumn[i] = predictedY[i,whichY];
      
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetYCrossResidual_ColumnGroup());
      }


      if(saveXResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetXCrossResidual_ColumnName(whichY,numberOfFactors);
        Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<matrixX.Rows;i++)
        {
          ycolumn[i] = spectralResiduals[i,0];
        }
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetXCrossResidual_ColumnGroup());
      }
      
    }

    public  virtual void CalculatePredictedAndResidual(
      DataTable table,
      int whichY,
      int numberOfFactors,
      bool saveYPredicted,
      bool saveYResidual,
      bool saveXResidual)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      IMultivariateCalibrationModel calib = GetCalibrationModel(table);
      //      Export(table,out calib);


      IMatrix matrixX = GetRawSpectra(plsMemo);

      MatrixMath.BEMatrix predictedY = new MatrixMath.BEMatrix(matrixX.Rows,calib.NumberOfY);
      MatrixMath.BEMatrix spectralResiduals = new MatrixMath.BEMatrix(matrixX.Rows,1);
      CalculatePredictedY(calib,plsMemo.SpectralPreprocessing,matrixX,numberOfFactors,predictedY,spectralResiduals);

      if(saveYPredicted)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYPredicted_ColumnName(whichY,numberOfFactors);
        Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<predictedY.Rows;i++)
          ycolumn[i] = predictedY[i,whichY];
      
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetYPredicted_ColumnGroup());
      }

      // subract the original y data
      IMatrix matrixY = GetOriginalY(plsMemo);
      MatrixMath.SubtractColumn(predictedY,matrixY,whichY,predictedY);

      if(saveYResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetYResidual_ColumnName(whichY,numberOfFactors);
        Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<predictedY.Rows;i++)
          ycolumn[i] = predictedY[i,whichY];
      
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetYResidual_ColumnGroup());
      }


      if(saveXResidual)
      {
        // insert a column with the proper name into the table and fill it
        string ycolname = GetXResidual_ColumnName(whichY,numberOfFactors);
        Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<matrixX.Rows;i++)
        {
          ycolumn[i] = spectralResiduals[i,0];
        }
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetXResidual_ColumnGroup());
      }
      
    }

    public void CalculateAndStorePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      IROMatrix predictionScores = this.CalculatePredictionScores(table,preferredNumberOfFactors);
      for(int i=0;i<predictionScores.Columns;i++)
      {
        DoubleColumn col = new DoubleColumn();
        for(int j=0;j<predictionScores.Rows;j++)
          col[j] = predictionScores[j,i];

        table.DataColumns.Add(col,GetPredictionScore_ColumnName(i,preferredNumberOfFactors),Altaxo.Data.ColumnKind.V,GetPredictionScore_ColumnGroup());
      }
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="srctable">Table holding the specta to predict values for.</param>
    /// <param name="selectedColumns">Columns selected in the source table.</param>
    /// <param name="selectedRows">Rows selected in the source table.</param>
    /// <param name="destTable">The table to store the prediction result.</param>
    /// <param name="modelTable">The table where the calibration model is stored.</param>
    /// <param name="numberOfFactors">Number of factors used to predict the values.</param>
    /// <param name="spectrumIsRow">If true, the spectra is horizontally oriented, else it is vertically oriented.</param>
    public virtual void PredictValues(
      DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      bool spectrumIsRow,
      int numberOfFactors,
      DataTable modelTable,
      DataTable destTable)
    {

      IMultivariateCalibrationModel calibModel = GetCalibrationModel(modelTable);
      //      Export(modelTable, out calibModel);
      MultivariateContentMemento memento = modelTable.GetTableProperty("Content") as MultivariateContentMemento;

      // Fill matrixX with spectra
      Altaxo.Collections.AscendingIntegerCollection spectralIndices;
      Altaxo.Collections.AscendingIntegerCollection measurementIndices;
      
      
      spectralIndices = new Altaxo.Collections.AscendingIntegerCollection(selectedColumns);
      measurementIndices = new Altaxo.Collections.AscendingIntegerCollection(selectedRows);
      RemoveNonNumericCells(srctable,measurementIndices,spectralIndices);

      // exchange selection if spectrum is column
      if(!spectrumIsRow)
      {
        Altaxo.Collections.AscendingIntegerCollection hlp;
        hlp = spectralIndices;
        spectralIndices = measurementIndices;
        measurementIndices = hlp;
      }
      
      // if there are more data than expected, we have to map the spectral indices
      if(spectralIndices.Count>calibModel.NumberOfX)
      {
        double[] xofx = GetXOfSpectra(srctable,spectrumIsRow,spectralIndices,measurementIndices);

        string errormsg;
        AscendingIntegerCollection map = MapSpectralX(calibModel.PreprocessingModel.XOfX,VectorMath.ToROVector(xofx),out errormsg);
        if(map==null)
          throw new ApplicationException("Can not map spectral data: " + errormsg);
        else
        {
          AscendingIntegerCollection newspectralindices = new AscendingIntegerCollection();
          for(int i=0;i<map.Count;i++)
            newspectralindices.Add(spectralIndices[map[i]]);
          spectralIndices = newspectralindices;
        }
      }

      IMatrix matrixX = GetRawSpectra(srctable,spectrumIsRow,spectralIndices,measurementIndices);

      MatrixMath.BEMatrix predictedY = new MatrixMath.BEMatrix(measurementIndices.Count,calibModel.NumberOfY);
      CalculatePredictedY(calibModel,memento.SpectralPreprocessing,matrixX,numberOfFactors, predictedY,null);

      // now save the predicted y in the destination table

      Altaxo.Data.DoubleColumn labelCol = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<measurementIndices.Count;i++)
      {
        labelCol[i] = measurementIndices[i];
      }
      destTable.DataColumns.Add(labelCol,"MeasurementLabel",Altaxo.Data.ColumnKind.Label,0);

      for(int k=0;k<predictedY.Columns;k++)
      {
        Altaxo.Data.DoubleColumn predictedYcol = new Altaxo.Data.DoubleColumn();

        for(int i=0;i<measurementIndices.Count;i++)
        {
          predictedYcol[i] = predictedY[i,k];
        }
        destTable.DataColumns.Add(predictedYcol,"Predicted Y" + k.ToString(), Altaxo.Data.ColumnKind.V,0);
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
      MultivariateContentMemento plsMemo = calibtable.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");


      IMatrix matrixX = GetRawSpectra(plsMemo);

      IMultivariateCalibrationModel calib = GetCalibrationModel(calibtable);

      // do spectral preprocessing
      plsMemo.SpectralPreprocessing.ProcessForPrediction(matrixX,calib.PreprocessingModel.XMean,calib.PreprocessingModel.XScale);


      // for the new table, save the spectra as column
      DoubleColumn xcol = new DoubleColumn();
      for(int i=matrixX.Columns;i>=0;i--)
        xcol[i] = calib.PreprocessingModel.XOfX[i];
      desttable.DataColumns.Add(xcol,_XOfX_ColumnName,ColumnKind.X,0);
    

      for(int n=0;n<matrixX.Rows;n++)
      {
        DoubleColumn col = new DoubleColumn();
        for(int i=matrixX.Columns-1;i>=0;i--) 
          col[i] = matrixX[n,i];
        desttable.DataColumns.Add(col,n.ToString(),ColumnKind.V,0);
      }
    }
    #endregion

    /// <summary>
    /// Makes a PLS (a partial least squares) analysis of the table or the selected columns / rows and stores the results in a newly created table.
    /// </summary>
    /// <param name="mainDocument">The main document of the application.</param>
    /// <param name="srctable">The table where the data come from.</param>
    /// <param name="selectedColumns">The selected columns.</param>
    /// <param name="selectedRows">The selected rows.</param>
    /// <param name="selectedPropertyColumns">The selected property column(s).</param>
    /// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
    /// <param name="plsOptions">Provides information about the max number of factors and the calculation of cross PRESS value.</param>
    /// <param name="preprocessOptions">Provides information about how to preprocess the spectra.</param>
    /// <returns></returns>
    public virtual string ExecuteAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      bool bHorizontalOrientedSpectrum,
      MultivariateAnalysisOptions plsOptions,
      SpectralPreprocessingOptions preprocessOptions
      )
    {
      IMatrix matrixX, matrixY;
      IROVector xOfX;
      MultivariateContentMemento plsContent = new MultivariateContentMemento();
      plsContent.Analysis = this;

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable(this.AnalysisName + " of " + srctable.Name);
      // Fill the Table
      table.Suspend();
      table.SetTableProperty("Content",plsContent);
      plsContent.TableName = srctable.Name;

     

      // Get matrices
      GetXYMatrices(
        srctable,
        selectedColumns,
        selectedRows,
        selectedPropertyColumns,
        bHorizontalOrientedSpectrum,
        plsContent,
        out matrixX, out matrixY, out xOfX );

    

      StoreXOfX(xOfX,table);


      // Preprocess
      plsContent.SpectralPreprocessing = preprocessOptions;
      IVector meanX,scaleX,meanY,scaleY;
      MultivariateRegression.PreprocessForAnalysis(preprocessOptions,xOfX,matrixX,matrixY,
        out meanX, out scaleX, out meanY, out scaleY); 

      StorePreprocessedData(meanX,scaleX,meanY,scaleY,table);


      // Analyze and Store
      IROVector press;
      ExecuteAnalysis(
        matrixX,
        matrixY,        
        plsOptions,
        plsContent,
        table, out press);


      this.StorePRESSData(press,table);

      if(plsOptions.CrossPRESSCalculation!=CrossPRESSCalculationType.None)
        CalculateCrossPRESS(xOfX,matrixX,matrixY,plsOptions,plsContent,table);
      
      StoreFRatioData(table,plsContent);

      StoreOriginalY(table,plsContent);

      table.Resume();
      Current.Project.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }

  }
}
