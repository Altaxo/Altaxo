#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// PCRWorksheetAnalysis performs a principal component analysis and subsequent regression and 
  /// stores the results in a given table
  /// </summary>
  [System.ComponentModel.Description("PCR")]
  public class PCRWorksheetAnalysis : WorksheetAnalysis
  {
    public override string AnalysisName
    {
      get
      {
        return "PCR";
      }
    }

    public override void ExecuteAnalysis(
      IMatrix matrixX,
      IMatrix matrixY,
      MultivariateAnalysisOptions plsOptions,
      MultivariateContentMemento plsContent,
      DataTable table,
      out IROVector press
      )
    {

      plsContent.CrossValidationType = plsOptions.CrossPRESSCalculation;
      int numFactors = plsOptions.MaxNumberOfFactors;
      numFactors = Math.Min(numFactors,matrixX.Columns);
      numFactors = Math.Min(numFactors,matrixX.Rows);

      //MatrixMath.PartialLeastSquares_HO(matrixX,matrixY,ref numFactors,xLoads,yLoads,W,V,PRESS);
      plsContent.NumberOfFactors = numFactors;


      IROMatrix xLoads, xScores;
      IROVector V;
      PCRRegression.ExecuteAnalysis(matrixX,matrixY, ref numFactors,out xLoads, out xScores, out V);


     


      // store the x-loads - careful - they are horizontal
      for(int i=0;i<numFactors;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

        for(int j=0;j<xLoads.Columns;j++)
          col[j] = xLoads[i,j];
          
        table.DataColumns.Add(col,GetXLoad_ColumnName(i),Altaxo.Data.ColumnKind.V,0);
      }
     
      // now store the scores - careful - they are vertical in the matrix
      for(int i=0;i<numFactors;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
        for(int j=0;j<xScores.Rows;j++)
          col[j] = xScores[j,i];
        
        table.DataColumns.Add(col,GetXScore_ColumnName(i),Altaxo.Data.ColumnKind.V,0);
      }

      // now store the y-loads (this are the preprocessed y in this case
      for(int cn=0;cn<plsContent.NumberOfConcentrationData;cn++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
        for(int i=0;i<plsContent.NumberOfMeasurements;i++)
          col[i] = matrixY[i,cn];
        
        table.DataColumns.Add(col,GetYLoad_ColumnName(cn),Altaxo.Data.ColumnKind.V,0);
      }

      // now store the cross product vector - it is a horizontal vector
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int j=0;j<numFactors;j++)
        col[j] = V[j];
      table.DataColumns.Add(col,GetCrossProduct_ColumnName(),Altaxo.Data.ColumnKind.V,3);
    }



      PCRRegression.CalculatePRESS(matrixY,xScores,numFactors, out press);

     
    }

    public override void CalculateCrossPRESS(
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
        
        PCRRegression.CrossValidation(
          matrixX,
          matrixY,
          plsOptions.MaxNumberOfFactors,
          GetGroupingStrategy(plsOptions),
          out crossPRESSMatrix,
          out meanNumberOfExcludedSpectra);

        VectorMath.Copy(crossPRESSMatrix,DataColumnWrapper.ToVector(crosspresscol,crossPRESSMatrix.Length));
       
        table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);

        plsContent.MeanNumberOfMeasurementsInCrossPRESSCalculation = plsContent.NumberOfMeasurements-meanNumberOfExcludedSpectra;
      }
      else
      {
        table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);
      }
     
    }


    public override IMultivariateCalibrationModel GetCalibrationModel(
      DataTable calibTable)
    {
      PCRCalibrationModel model;
      Export(calibTable,out model);
      return model;
    }

    #region Calculation after analysis

    static int GetNumberOfX(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetXLoad_ColumnName(0)];
      if(col==null) NotFound(GetXLoad_ColumnName(0));
      return col.Count;
    }

    static int GetNumberOfMeasurements(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetYLoad_ColumnName(0)];
      if(col==null) NotFound(GetYLoad_ColumnName(0));
      return col.Count;
    }

    static int GetNumberOfY(Altaxo.Data.DataTable table)
    {
      if(table.DataColumns[GetYLoad_ColumnName(0)]==null) NotFound(GetYLoad_ColumnName(0));
      for(int i=0;;i++)
      {
        if(null==table.DataColumns[GetYLoad_ColumnName(i)])
          return i;
      }
    }

    static int GetNumberOfFactors(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetCrossProduct_ColumnName()];
      if(col==null) NotFound(GetCrossProduct_ColumnName());
      return col.Count;
    }
    public static bool IsPCRCalibrationModel(Altaxo.Data.DataTable table)
    {
      if(null==table.DataColumns[GetXOfX_ColumnName()]) return false;
      if(null==table.DataColumns[GetXMean_ColumnName()]) return false;
      if(null==table.DataColumns[GetXScale_ColumnName()]) return false;
      if(null==table.DataColumns[GetYMean_ColumnName()]) return false;
      if(null==table.DataColumns[GetYScale_ColumnName()]) return false;

      if(null==table.DataColumns[GetXLoad_ColumnName(0)]) return false;
      if(null==table.DataColumns[GetXScore_ColumnName(0)]) return false;
      if(null==table.DataColumns[GetYLoad_ColumnName(0)]) return false;
      if(null==table.DataColumns[GetCrossProduct_ColumnName()]) return false;

      return true;
    }
    /// <summary>
    /// Exports a table to a PLS2CalibrationSet
    /// </summary>
    /// <param name="table">The table where the calibration model is stored.</param>
    /// <param name="calibrationSet"></param>
    public static void Export(
      DataTable table,
      out PCRCalibrationModel calibrationSet)
    {
      int numberOfX = GetNumberOfX(table);
      int numberOfY = GetNumberOfY(table);
      int numberOfFactors = GetNumberOfFactors(table);
      int numberOfMeasurements = GetNumberOfMeasurements(table);

      calibrationSet = new PCRCalibrationModel();
        
      calibrationSet.NumberOfX = numberOfX;
      calibrationSet.NumberOfY = numberOfY;
      calibrationSet.NumberOfFactors = numberOfFactors;

      Altaxo.Collections.AscendingIntegerCollection sel = new Altaxo.Collections.AscendingIntegerCollection();
      Altaxo.Data.DataColumn col;

      col = table[GetXOfX_ColumnName()];
      if(col==null || !(col is INumericColumn)) NotFound(GetXOfX_ColumnName());
      calibrationSet.XOfX = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector((INumericColumn)col,numberOfX);


      col = table[GetXMean_ColumnName()];
      if(col==null) NotFound(GetXMean_ColumnName());
      calibrationSet.XMean = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,numberOfX);

      col = table[GetXScale_ColumnName()];
      if(col==null) NotFound(GetXScale_ColumnName());
      calibrationSet.XScale = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,numberOfX);


        
      sel.Clear();
      col = table[GetYMean_ColumnName()];
      if(col==null) NotFound(GetYMean_ColumnName());
      sel.Add(table.DataColumns.GetColumnNumber(col));
      calibrationSet.YMean = DataColumnWrapper.ToROVector(col,numberOfY);

      sel.Clear();
      col = table[GetYScale_ColumnName()];
      if(col==null) NotFound(GetYScale_ColumnName());
      sel.Add(table.DataColumns.GetColumnNumber(col));
      calibrationSet.YScale = DataColumnWrapper.ToROVector(col,numberOfY);


      sel.Clear();
      for(int i=0;i<numberOfFactors;i++)
      {
        string colname = GetXScore_ColumnName(i);
        col = table[colname];
        if(col==null) NotFound(colname);
        sel.Add(table.DataColumns.GetColumnNumber(col));
      }
      calibrationSet.XScores = new Altaxo.Calc.DataColumnToColumnMatrixWrapper(table.DataColumns,sel,numberOfMeasurements);


      sel.Clear();
      for(int i=0;i<numberOfFactors;i++)
      {
        string colname = GetXLoad_ColumnName(i);
        col = table[colname];
        if(col==null) NotFound(colname);
        sel.Add(table.DataColumns.GetColumnNumber(col));
      }
      calibrationSet.XLoads = new Altaxo.Calc.DataColumnToRowMatrixWrapper(table.DataColumns,sel,numberOfX);


      sel.Clear();
      for(int i=0;i<numberOfY;i++)
      {
        string colname = GetYLoad_ColumnName(i);
        col = table[colname];
        if(col==null) NotFound(colname);
        sel.Add(table.DataColumns.GetColumnNumber(col));
      }
      calibrationSet.YLoads = new Altaxo.Calc.DataColumnToColumnMatrixWrapper(table.DataColumns,sel,numberOfMeasurements);

        
      sel.Clear();
      col = table[GetCrossProduct_ColumnName()];
      if(col==null) NotFound(GetCrossProduct_ColumnName());
      calibrationSet.CrossProduct = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,numberOfFactors);
   

    }

    public override void CalculatePredictedY(
      IMultivariateCalibrationModel mcalib,
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX,
      int numberOfFactors, 
      MatrixMath.BEMatrix  predictedY, 
      IMatrix spectralResiduals)
    {
      PCRCalibrationModel calib = (PCRCalibrationModel)mcalib;

      MultivariateRegression.PreprocessSpectraForPrediction(calib,preprocessOptions,matrixX);

      PCRRegression.Predict(
        matrixX,
        calib.XLoads,
        calib.YLoads,
        calib.XScores,
        calib.CrossProduct,
        numberOfFactors,
        predictedY,
        spectralResiduals);

      // mean and scale prediced Y
      MatrixMath.MultiplyRow(predictedY,calib.YScale,predictedY);
      MatrixMath.AddRow(predictedY,calib.YMean,predictedY);
    }  

    /// <summary>
    /// For a given set of calibration (!) spectra, cross predicts the y-values and stores them in the matrix <c>predictedY</c>. Cross prediction means that the prediction is
    /// done with exclusion of the spectra to predict from the calibration model.
    /// </summary>
    /// <param name="calib">The calibration model of the analysis.</param>
    /// <param name="preprocessOptions">The information how to preprocess the spectra.</param>
    /// <param name="matrixX">The matrix of calibration spectra. Each spectrum is a row in the matrix.</param>
    /// <param name="numberOfFactors">The number of factors used for prediction.</param>
    /// <param name="predictedY">On return, this matrix holds the cross predicted y-values. Each row in this matrix corresponds to the same row (spectrum) in matrixX.</param>
    /// <param name="spectralResiduals">If you set this parameter to a appropriate matrix, the spectral residuals will be stored in this matrix. Set this parameter to null if you don't need the residuals.</param>
    public override void CalculateCrossPredictedY(
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
      // TODO implement this!
      throw new NotImplementedException("Sorry, not implemented yet");
    }

    public override void CalculateXLeverage(
      DataTable table, int numberOfFactors)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");


      PCRCalibrationModel calib;
      Export(table, out calib);
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();


      IMatrix subscores = new MatrixMath.BEMatrix(calib.XScores.Rows,numberOfFactors);
      MatrixMath.Submatrix(calib.XScores,subscores);

      MatrixMath.SingularValueDecomposition decompose = new MatrixMath.SingularValueDecomposition(subscores);

      col.CopyDataFrom(decompose.HatDiagonal);

      table.DataColumns.Add(col,GetXLeverage_ColumnName(numberOfFactors),Altaxo.Data.ColumnKind.V,GetXLeverage_ColumnGroup());
    }


  

   
    public override IROMatrix CalculatePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      MultivariateContentMemento memento = table.GetTableProperty("Content") as MultivariateContentMemento;

      PCRCalibrationModel calib;
      Export(table,out calib);

      Matrix predictionScores = new Matrix(memento.NumberOfSpectralData,memento.NumberOfConcentrationData);
      PCRRegression.GetPredictionScoreMatrix(calib.XLoads,calib.YLoads,calib.XScores,calib.CrossProduct,preferredNumberOfFactors,predictionScores);
     
      
      return predictionScores.Transpose();
    }



    #endregion
  }
}
