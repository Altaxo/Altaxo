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
  /// PLS2WorksheetAnalysis performs a PLS1 analysis and 
  /// stores the results in a given table
  /// </summary>
  [System.ComponentModel.Description("PLS1")]
  public class PLS1WorksheetAnalysis : WorksheetAnalysis
  {

    public override string AnalysisName
    {
      get
      {
        return "PLS1";
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
      // the difference between Pls2 and Pls1 is that in Pls1 only one y value is
      // handled
      plsContent.NumberOfFactors = plsContent.NumberOfMeasurements;
      IExtensibleVector PRESS     = null;

      for(int yn=0;yn<matrixY.Columns;yn++)
      {

        MatrixMath.BEMatrix matrixYpls1 = new MatrixMath.BEMatrix(matrixY.Rows,1);
        MatrixMath.Submatrix(matrixY,matrixYpls1,yn,0);


        // now do a PLS with it
        MatrixMath.BEMatrix xLoads   = new MatrixMath.BEMatrix(0,0);
        MatrixMath.BEMatrix yLoads   = new MatrixMath.BEMatrix(0,0);
        MatrixMath.BEMatrix W       = new MatrixMath.BEMatrix(0,0);
        MatrixMath.REMatrix V       = new MatrixMath.REMatrix(0,0);
        IExtensibleVector localPRESS     = VectorMath.CreateExtensibleVector(0);
     


        int numFactors = Math.Min(matrixX.Columns,plsOptions.MaxNumberOfFactors);
        PLSRegression.ExecuteAnalysis(matrixX,matrixYpls1,ref numFactors,xLoads,yLoads,W,V,localPRESS);
        plsContent.NumberOfFactors = Math.Min(plsContent.NumberOfFactors,numFactors);
  
        // store the x-loads - careful - they are horizontal in the matrix
        for(int i=0;i<xLoads.Rows;i++)
        {
          Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

          for(int j=0;j<xLoads.Columns;j++)
            col[j] = xLoads[i,j];
          
          table.DataColumns.Add(col,GetXLoad_ColumnName(yn,i),Altaxo.Data.ColumnKind.V,0);
        }


        // now store the y-loads - careful - they are horizontal in the matrix
        for(int i=0;i<yLoads.Rows;i++)
        {
          Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        
          for(int j=0;j<yLoads.Columns;j++)
            col[j] = yLoads[i,j];
        
          table.DataColumns.Add(col,GetYLoad_ColumnName(yn,i),Altaxo.Data.ColumnKind.V,1);
        }

        // now store the weights - careful - they are horizontal in the matrix
        for(int i=0;i<W.Rows;i++)
        {
          Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
          for(int j=0;j<W.Columns;j++)
            col[j] = W[i,j];
        
          table.DataColumns.Add(col,GetXWeight_ColumnName(yn,i),Altaxo.Data.ColumnKind.V,0);
        }

        // now store the cross product vector - it is a horizontal vector
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
        for(int j=0;j<V.Columns;j++)
          col[j] = V[0,j];
        table.DataColumns.Add(col,GetCrossProduct_ColumnName(yn),Altaxo.Data.ColumnKind.V,3);
      }

        if(PRESS==null)
          PRESS=VectorMath.CreateExtensibleVector(localPRESS.Length);
        VectorMath.Add(PRESS,localPRESS,PRESS);
      
      } // for all y (constituents)


      press = PRESS;
    }

    
   
		

    public override void CalculateCrossPRESS(
      IMatrix matrixX,
      IMatrix matrixY,
      MultivariateAnalysisOptions plsOptions,
      MultivariateContentMemento plsContent,
      DataTable table
      )
    {
      IVector totalCrossPress=null;
      Altaxo.Data.DoubleColumn crosspresscol = new Altaxo.Data.DoubleColumn();

      double meanNumberOfExcludedSpectra = 0;
      if(plsOptions.CrossPRESSCalculation!=CrossPRESSCalculationType.None)
      {
        for(int yn=0;yn<matrixY.Columns;yn++)
        {
          MatrixMath.BEMatrix matrixYpls1 = new MatrixMath.BEMatrix(matrixY.Rows,1);
          MatrixMath.Submatrix(matrixY,matrixYpls1,yn,0);

          // now a cross validation - this can take a long time for bigger matrices
          IROVector crossPRESSMatrix;
        
          PLSRegression.CrossValidation(
            matrixX,
            matrixYpls1,
            plsOptions.MaxNumberOfFactors,
            GetGroupingStrategy(plsOptions),
            out crossPRESSMatrix,
            out meanNumberOfExcludedSpectra);

          if(totalCrossPress==null)
            totalCrossPress = VectorMath.CreateExtensibleVector(crossPRESSMatrix.Length);

          VectorMath.Add(crossPRESSMatrix,totalCrossPress,totalCrossPress);
        }

        VectorMath.Copy(totalCrossPress,DataColumnWrapper.ToVector(crosspresscol,totalCrossPress.Length));

        plsContent.MeanNumberOfMeasurementsInCrossPRESSCalculation = plsContent.NumberOfMeasurements-meanNumberOfExcludedSpectra;

        table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);
      
      }
      else
      {
        table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);
      }

    }

    public override IMultivariateCalibrationModel GetCalibrationModel(
      DataTable calibTable)
    {
      PLS1CalibrationModel model;
      Export(calibTable,out model);
      return model;
    }

    #region Calculation after analysis

    static int GetNumberOfX(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetXLoad_ColumnName(0,0)];
      if(col==null) NotFound(GetXLoad_ColumnName(0,0));
      return col.Count;
    }

    static int GetNumberOfY(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetYLoad_ColumnName(0,0)];
      if(col==null) NotFound(GetYLoad_ColumnName(0,0));
      return col.Count;
    }

    static int GetNumberOfFactors(Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn col = table.DataColumns[GetCrossProduct_ColumnName(0)];
      if(col==null) NotFound(GetCrossProduct_ColumnName(0));
      return col.Count;
    }
    public static bool IsPLS1CalibrationModel(Altaxo.Data.DataTable table)
    {
      if(null==table.DataColumns[GetXOfX_ColumnName()]) return false;
      if(null==table.DataColumns[GetXMean_ColumnName()]) return false;
      if(null==table.DataColumns[GetXScale_ColumnName()]) return false;
      if(null==table.DataColumns[GetYMean_ColumnName()]) return false;
      if(null==table.DataColumns[GetYScale_ColumnName()]) return false;

      if(null==table.DataColumns[GetXLoad_ColumnName(0,0)]) return false;
      if(null==table.DataColumns[GetXWeight_ColumnName(0,0)]) return false;
      if(null==table.DataColumns[GetYLoad_ColumnName(0,0)]) return false;
      if(null==table.DataColumns[GetCrossProduct_ColumnName(0)]) return false;

      return true;
    }
    /// <summary>
    /// Exports a table to a PLS2CalibrationSet
    /// </summary>
    /// <param name="calibrationSet"></param>
    public static void Export(
      DataTable _table,
      out PLS1CalibrationModel calibrationSet)
    {
      int _numberOfX = GetNumberOfX(_table);
      int _numberOfY = GetNumberOfY(_table);
      int _numberOfFactors = GetNumberOfFactors(_table);

      calibrationSet = new PLS1CalibrationModel(_numberOfX,_numberOfY,_numberOfFactors);
        
      Altaxo.Collections.AscendingIntegerCollection sel = new Altaxo.Collections.AscendingIntegerCollection();
      Altaxo.Data.DataColumn col;

      col = _table[GetXOfX_ColumnName()];
      if(col==null || !(col is INumericColumn)) NotFound(GetXOfX_ColumnName());
      calibrationSet.XOfX = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector((INumericColumn)col,_numberOfX);


      col = _table[GetXMean_ColumnName()];
      if(col==null) NotFound(GetXMean_ColumnName());
      calibrationSet.XMean = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,_numberOfX);

      col = _table[GetXScale_ColumnName()];
      if(col==null) NotFound(GetXScale_ColumnName());
      calibrationSet.XScale = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,_numberOfX);


        
      sel.Clear();
      col = _table[GetYMean_ColumnName()];
      if(col==null) NotFound(GetYMean_ColumnName());
      sel.Add(_table.DataColumns.GetColumnNumber(col));
      calibrationSet.YMean = DataColumnWrapper.ToROVector(col,_numberOfY);

      sel.Clear();
      col = _table[GetYScale_ColumnName()];
      if(col==null) NotFound(GetYScale_ColumnName());
      sel.Add(_table.DataColumns.GetColumnNumber(col));
      calibrationSet.YScale = DataColumnWrapper.ToROVector(col,_numberOfY);


      for(int yn=0;yn<_numberOfY;yn++)
      {

        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = GetXWeight_ColumnName(yn,i);
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XWeights[yn] = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = GetXLoad_ColumnName(yn,i);
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XLoads[yn] = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = GetYLoad_ColumnName(yn,i);
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.YLoads[yn] = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfY);

        
        sel.Clear();
        col = _table[GetCrossProduct_ColumnName(yn)];
        if(col==null) NotFound(GetCrossProduct_ColumnName());
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.CrossProduct[yn] = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfFactors);
      }
    }



    public override void CalculatePredictedY(
      IMultivariateCalibrationModel mcalib,
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX,
      int numberOfFactors, 
      MatrixMath.BEMatrix  predictedY, 
      IMatrix spectralResiduals)
    {
      PLS1CalibrationModel calib = (PLS1CalibrationModel)mcalib;

      PreProcessSpectra(calib,preprocessOptions,matrixX);

      for(int yn=0;yn<calib.NumberOfY;yn++)
      {

        PLSRegression.Predict(
          matrixX,
          calib.XLoads[yn],
          calib.YLoads[yn],
          calib.XWeights[yn],
          calib.CrossProduct[yn],
          numberOfFactors,
          MatrixMath.ToSubMatrix(predictedY,0,yn,predictedY.Rows,1),
          MatrixMath.ToSubMatrix(spectralResiduals,0,yn,spectralResiduals.Rows,1));
      }
      // mean and scale prediced Y
      MatrixMath.MultiplyRow(predictedY,calib.YScale,predictedY);
      MatrixMath.AddRow(predictedY,calib.YMean,predictedY);
    }  


  

    
    public override void CalculateXLeverage(
      DataTable table, int numberOfFactors)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");


      PLS1CalibrationModel calib;
      Export(table, out calib);

      IVector totalLeverage = VectorMath.CreateExtensibleVector(plsMemo.NumberOfMeasurements);

      IMatrix matrixX = GetRawSpectra(plsMemo);
      PreProcessSpectra(calib,plsMemo.SpectralPreprocessing,matrixX);

      for(int yn=0;yn<calib.NumberOfY;yn++)
      {
        // get the score matrix
        MatrixMath.BEMatrix weights = new MatrixMath.BEMatrix(numberOfFactors,calib.NumberOfX);
        MatrixMath.Submatrix(calib.XWeights[yn],weights,0,0);
        MatrixMath.BEMatrix scoresMatrix = new MatrixMath.BEMatrix(matrixX.Rows,weights.Rows);
        MatrixMath.MultiplySecondTransposed(matrixX,weights,scoresMatrix);
    
        MatrixMath.SingularValueDecomposition decomposition = MatrixMath.GetSingularValueDecomposition(scoresMatrix);

        VectorMath.Add(VectorMath.ToROVector(decomposition.HatDiagonal),totalLeverage,totalLeverage);
      }
      
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      VectorMath.Copy(totalLeverage,DataColumnWrapper.ToVector(col,totalLeverage.Length));
      table.DataColumns.Add(col,GetXLeverage_ColumnName(numberOfFactors),Altaxo.Data.ColumnKind.V,GetXLeverage_ColumnGroup());
    }


    public override IROMatrix CalculatePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      MultivariateContentMemento memento = table.GetTableProperty("Content") as MultivariateContentMemento;

      PLS1CalibrationModel calib;
      Export(table,out calib);

      Matrix predictionScores = new Matrix(memento.NumberOfConcentrationData,memento.NumberOfSpectralData);
      for(int yn=0;yn<calib.NumberOfY;yn++)
      {
        PLSRegression.GetPredictionScoreMatrix(
          calib.XLoads[yn],
          calib.YLoads[yn],
          calib.XWeights[yn],
          calib.CrossProduct[yn],
          preferredNumberOfFactors,
          MatrixMath.ToSubMatrix(predictionScores,yn,0,1,predictionScores.Columns));
      }
      return predictionScores;
    }

    #endregion
  }
}
