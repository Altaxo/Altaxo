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
  /// PLS2WorksheetAnalysis performs a PLS2 analysis and 
  /// stores the results in a given table
  /// </summary>
  [System.ComponentModel.Description("PLS2")]
  public class PLS2WorksheetAnalysis : WorksheetAnalysis
  {
    public override string AnalysisName
    {
      get
      {
        return "PLS2";
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


      // now do a PLS with it
      MatrixMath.BEMatrix xLoads   = new MatrixMath.BEMatrix(0,0);
      MatrixMath.BEMatrix yLoads   = new MatrixMath.BEMatrix(0,0);
      MatrixMath.BEMatrix W       = new MatrixMath.BEMatrix(0,0);
      MatrixMath.REMatrix V       = new MatrixMath.REMatrix(0,0);
      IExtensibleVector PRESS   = VectorMath.CreateExtensibleVector(0);
      press = PRESS;

      int numFactors = Math.Min(matrixX.Columns,plsOptions.MaxNumberOfFactors);
      PLSRegression.ExecuteAnalysis(matrixX,matrixY,ref numFactors,xLoads,yLoads,W,V,PRESS);
      plsContent.NumberOfFactors = numFactors;

      // store the x-loads - careful - they are horizontal in the matrix
      for(int i=0;i<xLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

        for(int j=0;j<xLoads.Columns;j++)
          col[j] = xLoads[i,j];
          
        table.DataColumns.Add(col,GetXLoad_ColumnName(i),Altaxo.Data.ColumnKind.V,0);
      }
   
      // now store the y-loads - careful - they are horizontal in the matrix
      for(int i=0;i<yLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        
        for(int j=0;j<yLoads.Columns;j++)
          col[j] = yLoads[i,j];
        
        table.DataColumns.Add(col,GetYLoad_ColumnName(i),Altaxo.Data.ColumnKind.V,1);
      }

      // now store the weights - careful - they are horizontal in the matrix
      for(int i=0;i<W.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
        for(int j=0;j<W.Columns;j++)
          col[j] = W[i,j];
        
        table.DataColumns.Add(col,GetXWeight_ColumnName(i),Altaxo.Data.ColumnKind.V,0);
      }

      // now store the cross product vector - it is a horizontal vector
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int j=0;j<V.Columns;j++)
        col[j] = V[0,j];
      table.DataColumns.Add(col,GetCrossProduct_ColumnName(),Altaxo.Data.ColumnKind.V,3);
    }



     
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
        
        PLSRegression.CrossValidation(
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
      PLS2CalibrationModel model;
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

    static int GetNumberOfY(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetYLoad_ColumnName(0)];
      if(col==null) NotFound(GetYLoad_ColumnName(0));
      return col.Count;
    }

    static int GetNumberOfFactors(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetCrossProduct_ColumnName()];
      if(col==null) NotFound(GetCrossProduct_ColumnName());
      return col.Count;
    }
    public static bool IsPLS2CalibrationModel(Altaxo.Data.DataTable table)
    {
      if(null==table.DataColumns[GetXOfX_ColumnName()]) return false;
      if(null==table.DataColumns[GetXMean_ColumnName()]) return false;
      if(null==table.DataColumns[GetXScale_ColumnName()]) return false;
      if(null==table.DataColumns[GetYMean_ColumnName()]) return false;
      if(null==table.DataColumns[GetYScale_ColumnName()]) return false;

      if(null==table.DataColumns[GetXLoad_ColumnName(0)]) return false;
      if(null==table.DataColumns[GetXWeight_ColumnName(0)]) return false;
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
      out PLS2CalibrationModel calibrationSet)
    {
      int numberOfX = GetNumberOfX(table);
      int numberOfY = GetNumberOfY(table);
      int numberOfFactors = GetNumberOfFactors(table);

      calibrationSet = new PLS2CalibrationModel();
        
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
        string colname = GetXWeight_ColumnName(i);
        col = table[colname];
        if(col==null) NotFound(colname);
        sel.Add(table.DataColumns.GetColumnNumber(col));
      }
      calibrationSet.XWeights = new Altaxo.Calc.DataColumnToRowMatrixWrapper(table.DataColumns,sel,numberOfX);


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
      for(int i=0;i<numberOfFactors;i++)
      {
        string colname = GetYLoad_ColumnName(i);
        col = table[colname];
        if(col==null) NotFound(colname);
        sel.Add(table.DataColumns.GetColumnNumber(col));
      }
      calibrationSet.YLoads = new Altaxo.Calc.DataColumnToRowMatrixWrapper(table.DataColumns,sel,numberOfY);

        
      sel.Clear();
      col = table[GetCrossProduct_ColumnName()];
      if(col==null) NotFound(GetCrossProduct_ColumnName());
      sel.Add(table.DataColumns.GetColumnNumber(col));
      calibrationSet.CrossProduct = new Altaxo.Calc.DataColumnToRowMatrixWrapper(table.DataColumns,sel,numberOfFactors);


        

    }

    public override void CalculatePredictedY(
      IMultivariateCalibrationModel mcalib,
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX,
      int numberOfFactors, 
      MatrixMath.BEMatrix  predictedY, 
      IMatrix spectralResiduals)
    {
      PLS2CalibrationModel calib = (PLS2CalibrationModel)mcalib;

      PreProcessSpectra(calib,preprocessOptions,matrixX);

      PLSRegression.Predict(
        matrixX,
        calib.XLoads,
        calib.YLoads,
        calib.XWeights,
        calib.CrossProduct,
        numberOfFactors,
        predictedY,
        spectralResiduals);

      // mean and scale prediced Y
      MatrixMath.MultiplyRow(predictedY,calib.YScale,predictedY);
      MatrixMath.AddRow(predictedY,calib.YMean,predictedY);
    }  

    /*
        public  override void CalculatePredictedAndResidual(
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

          PLS2CalibrationModel calib;
          Export(table,out calib);


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
            table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,GetYResidual_ColumnGroup());
          }
      
        }
     */

    public override void CalculateXLeverage(
      DataTable table, int numberOfFactors)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");


      PLS2CalibrationModel calib;
      Export(table, out calib);


      IMatrix matrixX = GetRawSpectra(plsMemo);
      PreProcessSpectra(calib,plsMemo.SpectralPreprocessing,matrixX);

      // get the score matrix
      MatrixMath.BEMatrix weights = new MatrixMath.BEMatrix(numberOfFactors,calib.XWeights.Columns);
      MatrixMath.Submatrix(calib.XWeights,weights,0,0);
      MatrixMath.BEMatrix scoresMatrix = new MatrixMath.BEMatrix(matrixX.Rows,weights.Rows);
      MatrixMath.MultiplySecondTransposed(matrixX,weights,scoresMatrix);
    
      MatrixMath.SingularValueDecomposition decomposition = MatrixMath.GetSingularValueDecomposition(scoresMatrix);

      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      col.CopyDataFrom(decomposition.HatDiagonal);

      table.DataColumns.Add(col,GetXLeverage_ColumnName(numberOfFactors),Altaxo.Data.ColumnKind.V,GetXLeverage_ColumnGroup());
    }


  

   
    public override IROMatrix CalculatePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      MultivariateContentMemento memento = table.GetTableProperty("Content") as MultivariateContentMemento;

      PLS2CalibrationModel model;
      Export(table,out model);

      Matrix predictionScores = new Matrix(memento.NumberOfConcentrationData,memento.NumberOfSpectralData);
      PLSRegression.GetPredictionScoreMatrix(model.XLoads,model.YLoads,model.XWeights,model.CrossProduct,preferredNumberOfFactors,predictionScores);
      return predictionScores;
    }



    #endregion
  }
}
