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
  public class PLS1WorksheetAnalysis : WorksheetAnalysis
  {

    public override string AnalysisName
    {
      get
      {
        return "PLS1";
      }
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    /// <param name="spectrumIsRow">If true, the spectra is horizontally oriented, else it is vertically oriented.</param>
    public override void PredictValues(
      DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      bool spectrumIsRow,
      int numberOfFactors,
      DataTable modelTable,
      DataTable destTable)
    {
      throw new NotImplementedException();
    }

    public  override void CalculatePredictedAndResidual(
      DataTable table,
      int whichY,
      int numberOfFactors,
      bool saveYPredicted,
      bool saveYResidual,
      bool saveXResidual)
    {
      throw new NotImplementedException();
    }

    public override void ExecuteAnalysis(
      IMatrix matrixX,
      IMatrix matrixY,
      MultivariateAnalysisOptions plsOptions,
      MultivariateContentMemento plsContent,
      DataTable table
      )
    {
      // the difference between Pls2 and Pls1 is that in Pls1 only one y value is
      // handled
      plsContent.NumberOfFactors = plsContent.NumberOfMeasurements;

      for(int yn=0;yn<matrixY.Columns;yn++)
      {

        MatrixMath.BEMatrix matrixYpls1 = new MatrixMath.BEMatrix(matrixY.Rows,1);
        MatrixMath.Submatrix(matrixY,matrixYpls1,yn,0);


        // now do a PLS with it
        MatrixMath.BEMatrix xLoads   = new MatrixMath.BEMatrix(0,0);
        MatrixMath.BEMatrix yLoads   = new MatrixMath.BEMatrix(0,0);
        MatrixMath.BEMatrix W       = new MatrixMath.BEMatrix(0,0);
        MatrixMath.REMatrix V       = new MatrixMath.REMatrix(0,0);
        IExtensibleVector PRESS     = VectorMath.CreateExtensibleVector(0);

     


        int numFactors = Math.Min(matrixX.Columns,plsOptions.MaxNumberOfFactors);
        MatrixMath.PartialLeastSquares_HO(matrixX,matrixYpls1,ref numFactors,xLoads,yLoads,W,V,PRESS);
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

      
      
      } // for all y (constituents)
    }
		

    public override void CalculateCrossPRESS(
      IMatrix matrixX,
      IMatrix matrixY,
      MultivariateAnalysisOptions plsOptions,
      MultivariateContentMemento plsContent,
      DataTable table
      )
    {
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
        
          MatrixMath.PartialLeastSquares_CrossValidation_HO(
            matrixX,
            matrixYpls1,
            plsOptions.MaxNumberOfFactors,
            plsOptions.CrossPRESSCalculation==CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements,
            out crossPRESSMatrix,
            out meanNumberOfExcludedSpectra);

          VectorMath.Copy(crossPRESSMatrix,DataColumnWrapper.ToVector(crosspresscol));
        }
      }
      table.DataColumns.Add(crosspresscol,GetCrossPRESSValue_ColumnName(),Altaxo.Data.ColumnKind.V,4);

    }

    public override void CalculateXLeverage(DataTable table, int preferredNumberOfFactors)
    {
      throw new NotImplementedException();
    }


    public override IMultivariateCalibrationModel GetCalibrationModel(DataTable calibTable)
    {
      throw new NotImplementedException();
    }

    public override IROMatrix CalculatePredictionScores(DataTable table, int preferredNumberOfFactors)
    {
      throw new NotImplementedException();
      MultivariateContentMemento memento = table.GetTableProperty("Content") as MultivariateContentMemento;
      //IMultivariateCalibrationModel model = memento.Analysis.GetCalibrationModel(table);

      Matrix predictionScores = new Matrix(memento.NumberOfConcentrationData,memento.NumberOfSpectralData);
      //MatrixMath.PartialLeastSquares_GetPredictionScoreMatrix(model.XLoads,model.YLoads,model.XWeights,model.CrossProduct,preferredNumberOfFactors,predictionScores);
      return predictionScores;
    }


  }
}
