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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;

using Altaxo.Collections;
using Altaxo.Worksheet.GUI;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.PLS;
using Altaxo.Data;
using Altaxo.Main.GUI;


namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contain commands concerning chemometric operations like PLS and PCA.
  /// </summary>
  public class ChemometricCommands
  {
   
    #region MultiplyColumnsToMatrix
    public static void MultiplyColumnsToMatrix(WorksheetController ctrl)
    {
      string err = MultiplyColumnsToMatrix(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }


    /// <summary>
    /// Multiplies selected columns to form a matrix.
    /// </summary>
    /// <param name="mainDocument"></param>
    /// <param name="srctable"></param>
    /// <param name="selectedColumns"></param>
    /// <returns>Null if successful, else the description of the error.</returns>
    /// <remarks>The user must select an even number of columns. All columns of the first half of the selection 
    /// must have the same number of rows, and all columns of the second half of selection must also have the same
    /// number of rows. The first half of selected columns form a matrix of dimensions(firstrowcount,halfselected), and the second half
    /// of selected columns form a matrix of dimension(halfselected, secondrowcount). The resulting matrix has dimensions (firstrowcount,secondrowcount) and is
    /// stored in a separate worksheet.</remarks>
    public static string MultiplyColumnsToMatrix(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns
      )
    {
      // check that there are columns selected
      if(0==selectedColumns.Count)
        return "You must select at least two columns to multiply!";
      // selected columns must contain an even number of columns
      if(0!=selectedColumns.Count%2)
        return "You selected an odd number of columns. Please select an even number of columns to multiply!";
      // all selected columns must be numeric columns
      for(int i=0;i<selectedColumns.Count;i++)
      {
        if(!(srctable[selectedColumns[i]] is Altaxo.Data.INumericColumn))
          return string.Format("The column[{0}] (name:{1}) is not a numeric column!",selectedColumns[i],srctable[selectedColumns[i]].Name);
      }


      int halfselect = selectedColumns.Count/2;
    
      // check that all columns from the first half of selected colums contain the same
      // number of rows

      int rowsfirsthalf=int.MinValue;
      for(int i=0;i<halfselect;i++)
      {
        int idx = selectedColumns[i];
        if(rowsfirsthalf<0)
          rowsfirsthalf = srctable[idx].Count;
        else if(rowsfirsthalf != srctable[idx].Count)
          return "The first half of selected columns have not all the same length!";
      }

      int rowssecondhalf=int.MinValue;
      for(int i=halfselect;i<selectedColumns.Count;i++)
      {
        int idx = selectedColumns[i];
        if(rowssecondhalf<0)
          rowssecondhalf = srctable[idx].Count;
        else if(rowssecondhalf != srctable[idx].Count)
          return "The second half of selected columns have not all the same length!";
      }


      // now create the matrices to multiply from the 

      MatrixMath.REMatrix firstMat = new MatrixMath.REMatrix(rowsfirsthalf,halfselect);
      for(int i=0;i<halfselect;i++)
      {
        Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i]];
        for(int j=0;j<rowsfirsthalf;j++)
          firstMat[j,i] = col.GetDoubleAt(j);
      }
      
      MatrixMath.BEMatrix secondMat = new MatrixMath.BEMatrix(halfselect,rowssecondhalf);
      for(int i=0;i<halfselect;i++)
      {
        Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i+halfselect]];
        for(int j=0;j<rowssecondhalf;j++)
          secondMat[i,j] = col.GetDoubleAt(j);
      }

      // now multiply the two matrices
      MatrixMath.BEMatrix resultMat = new MatrixMath.BEMatrix(rowsfirsthalf,rowssecondhalf);
      MatrixMath.Multiply(firstMat,secondMat,resultMat);


      // and store the result in a new worksheet 
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("ResultMatrix of " + srctable.Name);
      table.Suspend();

      // first store the factors
      for(int i=0;i<resultMat.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<resultMat.Rows;j++)
          col[j] = resultMat[j,i];
        
        table.DataColumns.Add(col,i.ToString());
      }

      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }

    #endregion

    #region PCA
    public static void PCAOnRows(WorksheetController ctrl)
    {
      int maxFactors = 3;
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        maxFactors,
        new Main.GUI.SingleValueDialog("Set maximum number of factors","Please enter the maximum number of factors to calculate:")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
      {
        string err= PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,true,ivictrl.EnteredContents);
        if(null!=err)
          System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
      }
    }

    public static void PCAOnColumns(WorksheetController ctrl)
    {
      int maxFactors = 3;
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        maxFactors,
        new Main.GUI.SingleValueDialog("Set maximum number of factors","Please enter the maximum number of factors to calculate:")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(ivictrl.ShowDialog(ctrl.View.TableViewForm))
      {
        string err= PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,false,ivictrl.EnteredContents);
        if(null!=err)
          System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
      }
    }




    /// <summary>
    /// Makes a PCA (a principal component analysis) of the table or the selected columns / rows and stores the results in a newly created table.
    /// </summary>
    /// <param name="mainDocument">The main document of the application.</param>
    /// <param name="srctable">The table where the data come from.</param>
    /// <param name="selectedColumns">The selected columns.</param>
    /// <param name="selectedRows">The selected rows.</param>
    /// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
    /// <param name="maxNumberOfFactors">The maximum number of factors to calculate.</param>
    /// <returns></returns>
    public static string PrincipalComponentAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      bool bHorizontalOrientedSpectrum,
      int maxNumberOfFactors
      )
    {
      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      int prenumcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
      
      // check for the number of numeric columns
      int numcols = 0;
      for(int i=0;i<prenumcols;i++)
      {
        int idx = bUseSelectedColumns ? selectedColumns[i] : i;
        if(srctable[i] is Altaxo.Data.INumericColumn)
          numcols++;
      }

      // check the number of rows
      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

      int numrows;
      if(bUseSelectedRows)
        numrows = selectedRows.Count;
      else
      {
        numrows = 0;
        for(int i=0;i<numcols;i++)
        {
          int idx = bUseSelectedColumns ? selectedColumns[i] : i;
          numrows = Math.Max(numrows,srctable[idx].Count);
        }     
      }

      // check that both dimensions are at least 2 - otherwise PCA is not possible
      if(numrows<2)
        return "At least two rows are neccessary to do Principal Component Analysis!";
      if(numcols<2)
        return "At least two numeric columns are neccessary to do Principal Component Analysis!";

      // Create a matrix of appropriate dimensions and fill it

      MatrixMath.BEMatrix matrixX;
      if(bHorizontalOrientedSpectrum)
      {
        matrixX = new MatrixMath.BEMatrix(numrows,numcols);
        int ccol = 0; // current column in the matrix
        for(int i=0;i<prenumcols;i++)
        {
          int colidx = bUseSelectedColumns ? selectedColumns[i] : i;
          Altaxo.Data.INumericColumn col = srctable[colidx] as Altaxo.Data.INumericColumn;
          if(null!=col)
          {
            for(int j=0;j<numrows;j++)
            {
              int rowidx = bUseSelectedRows ? selectedRows[j] : j;
              matrixX[j,ccol] = col.GetDoubleAt(rowidx);
            }
            ++ccol;
          }
        }
      } // end if it was a horizontal oriented spectrum
      else // if it is a vertical oriented spectrum
      {
        matrixX = new MatrixMath.BEMatrix(numcols,numrows);
        int ccol = 0; // current column in the matrix
        for(int i=0;i<prenumcols;i++)
        {
          int colidx = bUseSelectedColumns ? selectedColumns[i] : i;
          Altaxo.Data.INumericColumn col = srctable[colidx] as Altaxo.Data.INumericColumn;
          if(null!=col)
          {
            for(int j=0;j<numrows;j++)
            {
              int rowidx = bUseSelectedRows ? selectedRows[j] : j;
              matrixX[ccol,j] = col.GetDoubleAt(rowidx);
            }
            ++ccol;
          }
        }
      } // if it was a vertical oriented spectrum

      // now do PCA with the matrix
      MatrixMath.REMatrix factors = new MatrixMath.REMatrix(0,0);
      MatrixMath.BEMatrix loads = new MatrixMath.BEMatrix(0,0);
      MatrixMath.BEMatrix residualVariances = new MatrixMath.BEMatrix(0,0);
      MatrixMath.HorizontalVector meanX = new MatrixMath.HorizontalVector(matrixX.Columns);
      // first, center the matrix
      MatrixMath.ColumnsToZeroMean(matrixX,meanX);
      MatrixMath.NIPALS_HO(matrixX,maxNumberOfFactors,1E-9,factors,loads,residualVariances);

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PCA of " + srctable.Name);

      // Fill the Table
      table.Suspend();

      // first of all store the meanscore
    {
      double meanScore = MatrixMath.LengthOf(meanX);
      MatrixMath.NormalizeRows(meanX);
    
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<factors.Rows;i++)
        col[i] = meanScore;
      table.DataColumns.Add(col,"MeanFactor",Altaxo.Data.ColumnKind.V,0);
    }

      // first store the factors
      for(int i=0;i<factors.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<factors.Rows;j++)
          col[j] = factors[j,i];
        
        table.DataColumns.Add(col,"Factor"+i.ToString(),Altaxo.Data.ColumnKind.V,1);
      }

      // now store the mean of the matrix
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int j=0;j<meanX.Columns;j++)
        col[j] = meanX[0,j];
      table.DataColumns.Add(col,"MeanLoad",Altaxo.Data.ColumnKind.V,2);
    }

      // now store the loads - careful - they are horizontal in the matrix
      for(int i=0;i<loads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        
        for(int j=0;j<loads.Columns;j++)
          col[j] = loads[i,j];
        
        table.DataColumns.Add(col,"Load"+i.ToString(),Altaxo.Data.ColumnKind.V,3);
      }

      // now store the residual variances, they are vertical in the vector
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int i=0;i<residualVariances.Rows;i++)
        col[i] = residualVariances[i,0];
      table.DataColumns.Add(col,"ResidualVariance",Altaxo.Data.ColumnKind.V,4);
    }

      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }
    
    #endregion

    #region PLS Table Column Names and Groups

    const string _XOfX_ColumnName = "XOfX";
    const string _XMean_ColumnName = "XMean";
    const string _XScale_ColumnName = "XScale";
    const string _YMean_ColumnName = "YMean";
    const string _YScale_ColumnName = "YScale";

    const string _XLoad_ColumnName = "XLoad";
    const string _XWeight_ColumnName = "XWeight";
    const string _YLoad_ColumnName = "YLoad";
    const string _CrossProduct_ColumnName = "CrossP";

    const string _PRESSValue_ColumnName = "PRESS";
    const string _CrossPRESSValue_ColumnName = "CrossPRESS";

    const string _NumberOfFactors_ColumnName = "NumberOfFactors";
    const string _MeasurementLabel_ColumnName = "MeasurementLabel";
    const string _XLabel_ColumnName = "XLabel";
    const string _YLabel_ColumnName = "YLabel";

    const string _YOriginal_ColumnName   = "YOriginal";
    const string _YPredicted_ColumnName = "YPredicted";
    const string _YResidual_ColumnName   = "YResidual";
    const string _SpectralResidual_ColumnName = "SpectralResidual";
    const string _XLeverage_ColumnName        = "ScoreLeverage";
    const string _FRatio_ColumnName = "F-Ratio";
    const string _FProbability_ColumnName = "F-Probability";


    const int _NumberOfFactors_ColumnGroup = 4;
    const int _FRatio_ColumnGroup = 4;
    const int _FProbability_ColumnGroup = 4;

    const int _MeasurementLabel_ColumnGroup = 5;

    const int _YPredicted_ColumnGroup = 5;
    const int _YResidual_ColumnGroup = 5;
    const int _XLeverage_ColumnGroup = 5;


    public static string GetXLoad_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XLoad_ColumnName,numberOfFactors);
    }
    public static string GetXWeight_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XWeight_ColumnName,numberOfFactors);
    }
    public static string GetYLoad_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_YLoad_ColumnName,numberOfFactors);
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
    /// Gets the column name of a X-Leverage column
    /// </summary>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetXLeverage_ColumnName(int numberOfFactors)
    {
      return string.Format("{0}{1}",_XLeverage_ColumnName, numberOfFactors);
    }
 


    #endregion

    #region PLS Analysis

    public static void PLSOnRows(WorksheetController ctrl)
    {
      PLSAnalysisOptions options;
      SpectralPreprocessingOptions preprocessOptions;
      if(!QuestPLSAnalysisOptions(out options, out preprocessOptions))
        return;

      string err= PartialLeastSquaresAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,ctrl.SelectedPropertyColumns,true,options,preprocessOptions);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
    public static void PLSOnColumns(WorksheetController ctrl)
    {
      PLSAnalysisOptions options;
      SpectralPreprocessingOptions preprocessOptions;
      if(!QuestPLSAnalysisOptions(out options, out preprocessOptions))
        return;

      string err= PartialLeastSquaresAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,ctrl.SelectedPropertyColumns,false,options,preprocessOptions);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }


    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The spectra are presumed to be horizontally oriented, i.e. each spectrum is in one row.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    public static void PredictOnRows(WorksheetController ctrl)
    {
      PredictValues(ctrl,true);
    }
    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The spectra are presumed to be vertically oriented, i.e. each spectrum is in one column.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    public static void PredictOnColumns(WorksheetController ctrl)
    {
      PredictValues(ctrl,false);
    }

    
    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    /// <param name="spectrumIsRow">If true, the spectra is horizontally oriented, else it is vertically oriented.</param>
    public static void PredictValues(WorksheetController ctrl, bool spectrumIsRow)
    {
      string modelName, destName;
      if(false==QuestCalibrationModelAndDestinationTable(out modelName, out destName) || null==modelName)
        return; // Cancelled by user

      Altaxo.Data.DataTable modelTable = Current.Project.DataTableCollection[modelName];
      Altaxo.Data.DataTable destTable  = (null==destName ? new Altaxo.Data.DataTable() : Current.Project.DataTableCollection[destName]);

      if(modelTable==null || destTable==null)
        throw new ApplicationException("Unexpected: modelTable or destTable is null");

      int numberOfFactors = 0;

      PLSContentMemento memento = modelTable.GetTableProperty("Content") as PLSContentMemento;

      if(memento!=null)
        numberOfFactors = memento.PreferredNumberOfFactors;

      if(numberOfFactors==0)
      {
        QuestPreferredNumberOfFactors(modelTable);
        memento = modelTable.GetTableProperty("Content") as PLSContentMemento;
        if(memento!=null) numberOfFactors = memento.PreferredNumberOfFactors;
      }

      PLSCalibrationModelExporter exporter = new PLSCalibrationModelExporter(modelTable,numberOfFactors);
      PLS2CalibrationModel calibModel;
      exporter.Export(out calibModel);

      // Fill matrixX with spectra
      Altaxo.Collections.AscendingIntegerCollection spectralIndices;
      Altaxo.Collections.AscendingIntegerCollection measurementIndices;
      
      
      spectralIndices = new Altaxo.Collections.AscendingIntegerCollection(ctrl.SelectedDataColumns);
      measurementIndices = new Altaxo.Collections.AscendingIntegerCollection(ctrl.SelectedDataRows);
      RemoveNonNumericCells(ctrl.DataTable,measurementIndices,spectralIndices);

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
        double[] xofx = GetXOfSpectra(ctrl.DataTable,spectrumIsRow,spectralIndices,measurementIndices);

        string errormsg;
        AscendingIntegerCollection map = MapSpectralX(calibModel.XOfX,VectorMath.ToROVector(xofx),out errormsg);
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

      IMatrix matrixX = GetRawSpectra(ctrl.DataTable,spectrumIsRow,spectralIndices,measurementIndices);

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

      // if destTable is new, show it
      if(destTable.ParentObject==null)
      {
        Current.Project.DataTableCollection.Add(destTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(destTable);
      }

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
    public static string PartialLeastSquaresAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      bool bHorizontalOrientedSpectrum,
      PLSAnalysisOptions plsOptions,
      SpectralPreprocessingOptions preprocessOptions
      )
    {
      PLSContentMemento plsContent = new PLSContentMemento();
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
      plsContent.SpectralPreprocessing = preprocessOptions;


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



      // now fill the matrix

      
      // now check and fill in values
      MatrixMath.BEMatrix matrixX;
      MatrixMath.BEMatrix matrixY;


      // fill in the y-values
      matrixY = new MatrixMath.BEMatrix(measurementIndices.Count,concentrationIndices.Count);
      for(int i=0;i<concentrationIndices.Count;i++)
      {
        Altaxo.Data.INumericColumn col = concentration[concentrationIndices[i]] as Altaxo.Data.INumericColumn;
        for(int j=0;j<measurementIndices.Count;j++)
        {
          matrixY[j,i] = col.GetDoubleAt(measurementIndices[j]);
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
            matrixX[j,i] = col.GetDoubleAt(measurementIndices[j]);
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
            matrixX[i,j] = col.GetDoubleAt(spectralIndices[j]);
          }
        } // end fill in x-values
      }


      // now do a PLS with it
      MatrixMath.BEMatrix xLoads   = new MatrixMath.BEMatrix(0,0);
      MatrixMath.BEMatrix yLoads   = new MatrixMath.BEMatrix(0,0);
      MatrixMath.BEMatrix W       = new MatrixMath.BEMatrix(0,0);
      MatrixMath.REMatrix V       = new MatrixMath.REMatrix(0,0);
      MatrixMath.BEMatrix PRESS   = new MatrixMath.BEMatrix(0,0);

     

      // Before we can apply PLS, we have to center the x and y matrices
      MatrixMath.HorizontalVector meanX = new MatrixMath.HorizontalVector(matrixX.Columns);
      MatrixMath.HorizontalVector scaleX = new MatrixMath.HorizontalVector(matrixX.Columns);
      //  MatrixMath.HorizontalVector scaleX = new MatrixMath.HorizontalVector(matrixX.Cols);
      MatrixMath.HorizontalVector meanY = new MatrixMath.HorizontalVector(matrixY.Columns);


      preprocessOptions.Process(matrixX,meanX,scaleX);
      MatrixMath.ColumnsToZeroMean(matrixY, meanY);

      int numFactors = Math.Min(matrixX.Columns,plsOptions.MaxNumberOfFactors);
      MatrixMath.PartialLeastSquares_HO(matrixX,matrixY,ref numFactors,xLoads,yLoads,W,V,PRESS);
  

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PLS of " + srctable.Name);

      // Fill the Table
      table.Suspend();

      table.DataColumns.Add(xColumnOfX,_XOfX_ColumnName,Altaxo.Data.ColumnKind.X,0);


      // Store X-Mean and X-Scale
      Altaxo.Data.DoubleColumn colXMean = new Altaxo.Data.DoubleColumn();
      Altaxo.Data.DoubleColumn colXScale = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<matrixX.Columns;i++)
      {
        colXMean[i] = meanX[i];
        colXScale[i] = scaleX[i];
      }

      table.DataColumns.Add(colXMean,_XMean_ColumnName,Altaxo.Data.ColumnKind.V,0);
      table.DataColumns.Add(colXScale,_XScale_ColumnName,Altaxo.Data.ColumnKind.V,0);


      // store the x-loads - careful - they are horizontal in the matrix
      for(int i=0;i<xLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

        for(int j=0;j<xLoads.Columns;j++)
          col[j] = xLoads[i,j];
          
        table.DataColumns.Add(col,_XLoad_ColumnName+i.ToString(),Altaxo.Data.ColumnKind.V,0);
      }


      // store the y-mean and y-scale
      Altaxo.Data.DoubleColumn colYMean = new Altaxo.Data.DoubleColumn();
      Altaxo.Data.DoubleColumn colYScale = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<yLoads.Columns;i++)
      {
        colYMean[i] = meanY[i];
        colYScale[i] = 1;
      }

      table.DataColumns.Add(colYMean, _YMean_ColumnName,Altaxo.Data.ColumnKind.V,1);
      table.DataColumns.Add(colYScale,_YScale_ColumnName,Altaxo.Data.ColumnKind.V,1);


      // now store the y-loads - careful - they are horizontal in the matrix
      for(int i=0;i<yLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        
        for(int j=0;j<yLoads.Columns;j++)
          col[j] = yLoads[i,j];
        
        table.DataColumns.Add(col,_YLoad_ColumnName+i.ToString(),Altaxo.Data.ColumnKind.V,1);
      }

      // now store the weights - careful - they are horizontal in the matrix
      for(int i=0;i<W.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
        for(int j=0;j<W.Columns;j++)
          col[j] = W[i,j];
        
        table.DataColumns.Add(col,_XWeight_ColumnName+i.ToString(),Altaxo.Data.ColumnKind.V,0);
      }

      // now store the cross product vector - it is a horizontal vector
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int j=0;j<V.Columns;j++)
        col[j] = V[0,j];
      table.DataColumns.Add(col,_CrossProduct_ColumnName,Altaxo.Data.ColumnKind.V,3);
    }

      // add a NumberOfFactors columm
      Altaxo.Data.DoubleColumn xNumFactor= new Altaxo.Data.DoubleColumn();
      for(int i=0;i<PRESS.Rows;i++)
      { 
        xNumFactor[i]=i;
      }
      table.DataColumns.Add(xNumFactor,_NumberOfFactors_ColumnName,Altaxo.Data.ColumnKind.X,_NumberOfFactors_ColumnGroup);

      Altaxo.Data.DoubleColumn crosspresscol = new Altaxo.Data.DoubleColumn();

      double meanNumberOfExcludedSpectra = 0;
      if(plsOptions.CrossPRESSCalculation!=CrossPRESSCalculationType.None)
      {
        // now a cross validation - this can take a long time for bigger matrices
        IMatrix crossPRESSMatrix;
        
        MatrixMath.PartialLeastSquares_CrossValidation_HO(matrixX,matrixY,numFactors, plsOptions.CrossPRESSCalculation==CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements, out crossPRESSMatrix, out meanNumberOfExcludedSpectra);

        for(int i=0;i<crossPRESSMatrix.Rows;i++)
        { 
          crosspresscol[i] = crossPRESSMatrix[i,0];
        }
      }
      table.DataColumns.Add(crosspresscol,_CrossPRESSValue_ColumnName,Altaxo.Data.ColumnKind.V,4);


      // calculate the self predicted y values - for one factor and for two
      IMatrix yPred = new MatrixMath.BEMatrix(matrixY.Rows,matrixY.Columns);
      Altaxo.Data.DoubleColumn presscol = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<PRESS.Rows;i++)
        presscol[i] = PRESS[i,0];
      table.DataColumns.Add(presscol,"PRESS",Altaxo.Data.ColumnKind.V,4);




      // calculate the F-ratio and the F-Probability
      int numberOfSignificantFactors = numFactors;
      Altaxo.Data.DoubleColumn colForFratio = (plsOptions.CrossPRESSCalculation==CrossPRESSCalculationType.None) ? presscol : crosspresscol;
      double pressMin=double.MaxValue;
      for(int i=0;i<colForFratio.Count;i++)
        pressMin = Math.Min(pressMin,colForFratio[i]);
      DoubleColumn fratiocol = new DoubleColumn();
      DoubleColumn fprobcol = new DoubleColumn();
      for(int i=0;i<colForFratio.Count;i++)
      {
        double fratio = colForFratio[i]/pressMin;
        double fprob  = Calc.Probability.FDistribution.CDF(fratio,matrixX.Rows-meanNumberOfExcludedSpectra,matrixX.Rows-meanNumberOfExcludedSpectra);
        fratiocol[i] = fratio;
        fprobcol[i]  = fprob;
        if(fprob<0.75 && numberOfSignificantFactors>i)
          numberOfSignificantFactors = i;
      }
      plsContent.PreferredNumberOfFactors = numberOfSignificantFactors;
      table.DataColumns.Add(fratiocol,_FRatio_ColumnName,Altaxo.Data.ColumnKind.V,_FRatio_ColumnGroup);
      table.DataColumns.Add(fprobcol,_FProbability_ColumnName,Altaxo.Data.ColumnKind.V,_FProbability_ColumnGroup);

      // add a label column for the measurement number
      Altaxo.Data.DoubleColumn measurementLabel = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<measurementIndices.Count;i++)
        measurementLabel[i] = measurementIndices[i];
      table.DataColumns.Add(measurementLabel,_MeasurementLabel_ColumnName,Altaxo.Data.ColumnKind.Label,_MeasurementLabel_ColumnGroup);

      // now add the original Y-Columns
      for(int i=0;i<matrixY.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<matrixY.Rows;j++)
          col[j]=matrixY[j,i] + meanY[0,i];
        table.DataColumns.Add(col,_YOriginal_ColumnName+i.ToString(),Altaxo.Data.ColumnKind.X,5+i);
      }
      
      // table.DataColumns.Add(labelColumnOfX,"XLabel",Altaxo.Data.ColumnKind.Label,5);

      /*
      // and now the predicted Y 
      for(int nFactor=1;nFactor<=numFactors;nFactor++)
      {
        MatrixMath.PartialLeastSquares_Predict_HO(matrixX,xLoads,yLoads,W,V,nFactor, yPred);

        // Calculate the PRESS value
        presscol[nFactor] = MatrixMath.SumOfSquaredDifferences(matrixY,yPred);


        // now store the predicted y - careful - they are horizontal in the matrix,
        // but we store them vertically now
        for(int i=0;i<yPred.Columns;i++)
        {
          Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
          for(int j=0;j<yPred.Rows;j++)
            col[j] = yPred[j,i] + meanY[0,i];
        
          table.DataColumns.Add(col,GetYPredicted_ColumnName(i,nFactor),Altaxo.Data.ColumnKind.V,_PredictedYColumnGroup);
        }
      } // for nFactor...
      */
   
      table.SetTableProperty("Content",plsContent);

      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }


    #endregion

    #region PLS Model Export

    public static void ExportPLSCalibration(Altaxo.Data.DataTable table)
    {
      // quest the number of factors to export
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        1,
        new Main.GUI.SingleValueDialog("Number of factors","Please choose number of factors to export (>0):")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(!ivictrl.ShowDialog(Current.MainWindow))
        return;

    
      // quest the filename
      System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
      dlg.DefaultExt="xml";
      if(System.Windows.Forms.DialogResult.OK!=dlg.ShowDialog(Current.MainWindow))
        return;

      PLSCalibrationModelExporter exporter = new PLSCalibrationModelExporter(table,ivictrl.EnteredContents);
      exporter.Export(dlg.FileName);
    }

    class PLSCalibrationModelExporter
    {
      
      Altaxo.Data.DataTable _table;
      System.Xml.XmlWriter _writer;

      int _numberOfFactors;
      int _numberOfX;
      int _numberOfY;


      public static bool IsPLSCalibrationModel(Altaxo.Data.DataTable table)
      {
        if(null==table.DataColumns[_XOfX_ColumnName]) return false;
        if(null==table.DataColumns[_XMean_ColumnName]) return false;
        if(null==table.DataColumns[_XScale_ColumnName]) return false;
        if(null==table.DataColumns[_YMean_ColumnName]) return false;
        if(null==table.DataColumns[_YScale_ColumnName]) return false;

        if(null==table.DataColumns[GetXLoad_ColumnName(0)]) return false;
        if(null==table.DataColumns[GetXWeight_ColumnName(0)]) return false;
        if(null==table.DataColumns[GetYLoad_ColumnName(0)]) return false;
        if(null==table.DataColumns[_CrossProduct_ColumnName]) return false;

        return true;
      }

      public PLSCalibrationModelExporter(Altaxo.Data.DataTable table, int numberOfFactors)
      {
        _table = table;
        _numberOfFactors = numberOfFactors;
      }


      /// <summary>
      /// Exports a table to a PLS2CalibrationSet
      /// </summary>
      /// <param name="calibrationSet"></param>
      public void Export(out PLS2CalibrationModel calibrationSet)
      {
        _numberOfX = GetNumberOfX(_table);
        _numberOfY = GetNumberOfY(_table);
        _numberOfFactors = Math.Min(_numberOfFactors,GetNumberOfFactors(_table));

        calibrationSet = new PLS2CalibrationModel();
        
        calibrationSet.NumberOfX = _numberOfX;
        calibrationSet.NumberOfY = _numberOfY;
        calibrationSet.NumberOfFactors = _numberOfFactors;

        Altaxo.Collections.AscendingIntegerCollection sel = new Altaxo.Collections.AscendingIntegerCollection();
        Altaxo.Data.DataColumn col;

        col = _table[_XOfX_ColumnName];
        if(col==null || !(col is INumericColumn)) NotFound(_XOfX_ColumnName);
        calibrationSet.XOfX = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector((INumericColumn)col,_numberOfX);


        col = _table[_XMean_ColumnName];
        if(col==null) NotFound(_XMean_ColumnName);
        calibrationSet.XMean = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,_numberOfX);

        col = _table[_XScale_ColumnName];
        if(col==null) NotFound(_XScale_ColumnName);
        calibrationSet.XScale = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col,_numberOfX);


        
        sel.Clear();
        col = _table[_YMean_ColumnName];
        if(col==null) NotFound(_YMean_ColumnName);
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.YMean = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfY);

        sel.Clear();
        col = _table[_YScale_ColumnName];
        if(col==null) NotFound(_YScale_ColumnName);
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.YScale = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfY);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = _XWeight_ColumnName + i.ToString();
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XWeights = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = _XLoad_ColumnName + i.ToString();
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XLoads = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = _YLoad_ColumnName + i.ToString();
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.YLoads = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfY);

        
        sel.Clear();
        col = _table[_CrossProduct_ColumnName];
        if(col==null) NotFound(_CrossProduct_ColumnName);
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.CrossProduct = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfFactors);


        

      }



      public void Export(string filename)
      {
        _writer = new System.Xml.XmlTextWriter(filename,System.Text.Encoding.UTF8);
        _writer.WriteStartDocument();
        _writer.WriteStartElement("PLSCalibrationModel");

        WriteProperties();
        WriteData();

        _writer.WriteEndElement(); // PLSCalibrationModel
        _writer.WriteEndDocument();

        _writer.Close();
      }

      void WriteProperties()
      {
        _writer.WriteStartElement("Properties");


        _numberOfX = GetNumberOfX(_table);
        _numberOfY = GetNumberOfY(_table);
        _numberOfFactors = Math.Min(_numberOfFactors,GetNumberOfFactors(_table));

        _writer.WriteElementString("NumberOfX",System.Xml.XmlConvert.ToString(_numberOfX));
        _writer.WriteElementString("NumberOfY",System.Xml.XmlConvert.ToString(_numberOfY));
        _writer.WriteElementString("NumberOfFactors",System.Xml.XmlConvert.ToString(_numberOfFactors));

        _writer.WriteEndElement(); // Properties
      }


      static int GetNumberOfX(Altaxo.Data.DataTable table)
      {
        Altaxo.Data.DataColumn col = table.DataColumns[_XLoad_ColumnName + "0"];
        if(col==null) NotFound(_XLoad_ColumnName + "0");
        return col.Count;
      }

      static int GetNumberOfY(Altaxo.Data.DataTable table)
      {
        Altaxo.Data.DataColumn col = table.DataColumns[_YLoad_ColumnName + "0"];
        if(col==null) NotFound(_YLoad_ColumnName + "0");
        return col.Count;
      }

      static int GetNumberOfFactors(Altaxo.Data.DataTable table)
      {
        Altaxo.Data.DataColumn col = table.DataColumns[_CrossProduct_ColumnName];
        if(col==null) NotFound(_CrossProduct_ColumnName);
        return col.Count;
      }



      void WriteData()
      {
        _writer.WriteStartElement("Data");

        WriteXData();
        WriteYData();
        WriteCrossProductData();

        _writer.WriteEndElement(); // Data
      }


      void WriteXData()
      {
        Altaxo.Data.DoubleColumn col=null;

        _writer.WriteStartElement("XData");

        col = _table.DataColumns[_XOfX_ColumnName] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound(_XOfX_ColumnName);
        WriteVector("XOfX",col, _numberOfX);

        col = _table.DataColumns[_XMean_ColumnName] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound(_XMean_ColumnName);
        WriteVector("XMean",col, _numberOfX);

        col = _table.DataColumns[_XScale_ColumnName] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound(_XScale_ColumnName);
        WriteVector("XScale",col, _numberOfX);

        WriteXLoads();

        WriteXWeights();



        _writer.WriteEndElement(); // XData
      }

      void WriteXLoads()
      {
        Altaxo.Data.DoubleColumn col=null;

        _writer.WriteStartElement("XLoads");
        // Loads
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = _XLoad_ColumnName+i.ToString();
          col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
          if(null==col) NotFound(colname);
          WriteVector(colname,col, _numberOfX);
        }
        _writer.WriteEndElement();
      }


      void WriteXWeights()
      {
        Altaxo.Data.DoubleColumn col=null;

        _writer.WriteStartElement("XWeights");
        // Loads
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = _XWeight_ColumnName+i.ToString();
          col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
          if(null==col) NotFound(colname);
          WriteVector(colname,col, _numberOfX);
        }
        _writer.WriteEndElement();
      }

      void WriteYData()
      {
        Altaxo.Data.DoubleColumn col=null;

        _writer.WriteStartElement("YData");

        col = _table.DataColumns[_YMean_ColumnName] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound(_YMean_ColumnName);
        WriteVector("YMean",col, _numberOfY);

        col = _table.DataColumns[_YScale_ColumnName] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound(_YScale_ColumnName);
        WriteVector("YScale",col, _numberOfY);

        WriteYLoads();

        _writer.WriteEndElement(); // YData
      }

      void WriteYLoads()
      {
        Altaxo.Data.DoubleColumn col=null;

        _writer.WriteStartElement("YLoads");
        // Loads
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = _YLoad_ColumnName+i.ToString();
          col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
          if(null==col) NotFound(colname);
          WriteVector(colname,col, _numberOfY);
        }
        _writer.WriteEndElement();
      }

      void WriteCrossProductData()
      {
        Altaxo.Data.DoubleColumn col=null;

        col = _table.DataColumns[_CrossProduct_ColumnName] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound(_CrossProduct_ColumnName);
        WriteVector("CrossProductData", col, _numberOfFactors);

      }


      void WriteVector(string name, Altaxo.Data.DoubleColumn col, int numberOfData)
      {
        _writer.WriteStartElement(name);

        for(int i=0;i<numberOfData;i++)
        {
          _writer.WriteElementString("e",System.Xml.XmlConvert.ToString(col[i]));
        }


        _writer.WriteEndElement(); // name
      }

      static void NotFound(string name)
      {
        throw new ArgumentException("Column " + name + " not found in the table.");
      }

    }

    #endregion

    #region PLS Retrieving original data

    public static string[] GetAvailablePLSCalibrationTables()
    {
      System.Collections.ArrayList result=new System.Collections.ArrayList();
      foreach(Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
      {
        if(PLSCalibrationModelExporter.IsPLSCalibrationModel(table))
          result.Add(table.Name);
      }

      return (string[])result.ToArray(typeof(string));
    }


    /// <summary>
    /// Using the information in the plsMemo, gets the matrix of original spectra. The spectra are horizontal in the matrix, i.e. each spectra is a matrix row.
    /// </summary>
    /// <param name="plsMemo">The PLS memento containing the information about the location of the original data.</param>
    /// <returns>The matrix of the original spectra.</returns>
    public static IMatrix GetRawSpectra(PLSContentMemento plsMemo)
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
            matrixX[j,i] = col.GetDoubleAt(measurementIndices[j]);
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
            matrixX[i,j] = col.GetDoubleAt(spectralIndices[j]);
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
        result[i] = col.GetDoubleAt(spectralIndices[i]);

      return result;
    }

    /// <summary>
    /// Using the information in the plsMemo, gets the matrix of original Y (concentration) data.
    /// </summary>
    /// <param name="plsMemo">The PLS mememto containing the information where to find the original data.</param>
    /// <returns>Matrix of orignal Y (concentration) data.</returns>
    public static IMatrix GetOriginalY(PLSContentMemento plsMemo)
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
          matrixY[j,i] = col.GetDoubleAt(measurementIndices[j]);
        }
      } // end fill in yvalues

      return matrixY;
    }

    #endregion

    #region PLS Calculating values

    /// <summary>
    /// Calculated the predicted y for the component given by <c>whichY</c> and for the given number of factors.
    /// </summary>
    /// <param name="table">The table containing the PLS model.</param>
    /// <param name="whichY">The number of the y component.</param>
    /// <param name="numberOfFactors">Number of factors for calculation.</param>
    public static void CalculateYPredicted(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table,whichY,numberOfFactors,true,false,false);
    }

    public static void CalculateYResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table,whichY,numberOfFactors,false,true,false);
    }

    public static void CalculateXResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
      CalculatePredictedAndResidual(table,whichY,numberOfFactors,false,false,true);
    }

    /// <summary>
    /// This will convert the raw spectra (horizontally in matrixX) to preprocessed spectra according to the calibration model.
    /// </summary>
    /// <param name="calib">The calibration model containing the instructions to process the spectra.</param>
    /// <param name="preprocessOptions">Contains the information how to preprocess the spectra.</param>
    /// <param name="matrixX">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    public static void PreProcessSpectra(
      PLS2CalibrationModel calib, 
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX)
    {
      preprocessOptions.ProcessForPrediction(matrixX,calib.XMean,calib.XScale);
    }

    public static void CalculatePredictedY(
      PLS2CalibrationModel calib,
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX,
      int numberOfFactors, 
      MatrixMath.BEMatrix  predictedY, 
      IMatrix spectralResiduals)
    {
      PreProcessSpectra(calib,preprocessOptions,matrixX);

      MatrixMath.PartialLeastSquares_Predict_HO(
        matrixX,
        calib.XLoads,
        calib.YLoads,
        calib.XWeights,
        calib.CrossProduct,
        numberOfFactors,
        predictedY,
        spectralResiduals);

      // mean and scale prediced Y
      MatrixMath.MultiplyRow(predictedY,calib.YScale,0,predictedY);
      MatrixMath.AddRow(predictedY,calib.YMean,0,predictedY);
    }  


    /// <summary>
    /// Fills a provided table (should be empty) with the preprocessed spectra. The spectra are saved as columns (independently on their former orientation in the original worksheet).
    /// </summary>
    /// <param name="calibtable">The table containing the calibration model.</param>
    /// <param name="desttable">The table where to store the preprocessed spectra. Should be empty.</param>
    public static void CalculatePreprocessedSpectra(Altaxo.Data.DataTable calibtable, Altaxo.Data.DataTable desttable)
    {
      PLSContentMemento plsMemo = calibtable.GetTableProperty("Content") as PLSContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      PLS2CalibrationModel calib;
      PLSCalibrationModelExporter exporter = new PLSCalibrationModelExporter(calibtable,1);
      exporter.Export(out calib);

      IMatrix matrixX = GetRawSpectra(plsMemo);

      // do spectral preprocessing
      plsMemo.SpectralPreprocessing.ProcessForPrediction(matrixX,calib.XMean,calib.XScale);


      // for the new table, save the spectra as column
      DoubleColumn xcol = new DoubleColumn();
      for(int i=matrixX.Columns;i>=0;i--)
        xcol[i] = calib.XOfX[i];
      desttable.DataColumns.Add(xcol,_XOfX_ColumnName,ColumnKind.X,0);
    

      for(int n=0;n<matrixX.Rows;n++)
      {
        DoubleColumn col = new DoubleColumn();
        for(int i=matrixX.Columns-1;i>=0;i--) 
          col[i] = matrixX[n,i];
        desttable.DataColumns.Add(col,n.ToString(),ColumnKind.V,0);
      }
    }

    public static void CalculatePredictedAndResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors, bool saveYPredicted, bool saveYResidual, bool saveXResidual)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      PLS2CalibrationModel calib;
      PLSCalibrationModelExporter exporter = new PLSCalibrationModelExporter(table,numberOfFactors);
      exporter.Export(out calib);


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
      
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,_YPredicted_ColumnGroup);
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
      
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,_YResidual_ColumnGroup);
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
        table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,_YResidual_ColumnGroup);
      }
      
    }

    public static void CalculateXLeverage(Altaxo.Data.DataTable table, int numberOfFactors)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");


      PLS2CalibrationModel calib;
      PLSCalibrationModelExporter exporter = new PLSCalibrationModelExporter(table,numberOfFactors);
      exporter.Export(out calib);


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

      table.DataColumns.Add(col,GetXLeverage_ColumnName(numberOfFactors),Altaxo.Data.ColumnKind.V,_XLeverage_ColumnGroup);
    }

    #endregion

    #region PLS Plot Commands

    /// <summary>
    /// Asks the user for the maximum number of factors and the cross validation calculation.
    /// </summary>
    /// <param name="options">The PLS options to ask for. On return, this is the user's choice.</param>
    /// <param name="preprocessOptions">The spectral preprocessing options to ask for (output).</param>
    /// <returns>True if the user has made his choice, false if the user pressed the Cancel button.</returns>
    public static bool QuestPLSAnalysisOptions(out PLSAnalysisOptions options, out SpectralPreprocessingOptions preprocessOptions)
    {
      options = new PLSAnalysisOptions();
      options.MaxNumberOfFactors =20;
      options.CrossPRESSCalculation = CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements;

      PLSStartAnalysisController ctrlAA = new PLSStartAnalysisController(options);
      PLSStartAnalysisControl    viewAA = new PLSStartAnalysisControl();
      ctrlAA.View = viewAA;

      preprocessOptions = new SpectralPreprocessingOptions();
      SpectralPreprocessingController  ctrlBB = new SpectralPreprocessingController(preprocessOptions);
      SpectralPreprocessingControl     viewBB = new SpectralPreprocessingControl();
      ctrlBB.View = viewBB;


      TabbedDialogController dialogctrl = new TabbedDialogController("PLS Analysis",false);
      dialogctrl.AddTab("Factors",ctrlAA,viewAA);
      dialogctrl.AddTab("Preprocessing",ctrlBB,viewBB);
      TabbedDialogView  dialogview = new TabbedDialogView();
      dialogctrl.View = dialogview;

      if(dialogctrl.ShowDialog(Current.MainWindow))
      {
        options = ctrlAA.Doc;

        return true;
      }
      return false;
    }


    /// <summary>
    /// Ask the user (before a prediction is made) for the name of the calibration model table and the destination table.
    /// </summary>
    /// <param name="modelTableName">On return, contains the name of the table containing the calibration model.</param>
    /// <param name="destinationTableName">On return, contains the name of the destination table, or null if a new table should be used as destination.</param>
    /// <returns>True if OK, false if the users pressed Cancel.</returns>
    public static bool QuestCalibrationModelAndDestinationTable(out string modelTableName, out string destinationTableName)
    {
      Altaxo.Worksheet.GUI.PLSPredictValueController ctrl = new Altaxo.Worksheet.GUI.PLSPredictValueController();
      Altaxo.Worksheet.GUI.PLSPredictValueControl viewctrl = new PLSPredictValueControl();
      ctrl.View = viewctrl;

      Altaxo.Main.GUI.DialogShellController dlgctrl = new Altaxo.Main.GUI.DialogShellController(
        new Altaxo.Main.GUI.DialogShellView(viewctrl),ctrl);

      if(dlgctrl.ShowDialog(Current.MainWindow))
      {
        modelTableName = ctrl.SelectedCalibrationTableName;
        destinationTableName = ctrl.SelectedDestinationTableName;
        return true;
      }
      else
      {
        modelTableName = null;
        destinationTableName = null;
        return false;
      }
    }


    /// <summary>
    /// This plots a label plot into the provided layer.
    /// </summary>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="xcol">The x column.</param>
    /// <param name="ycol">The y column.</param>
    /// <param name="labelcol">The label column.</param>
    public static void PlotOnlyLabel(Altaxo.Graph.XYPlotLayer layer, Altaxo.Data.DataColumn xcol, Altaxo.Data.DataColumn ycol, Altaxo.Data.DataColumn labelcol)  
    {
      Altaxo.Graph.XYColumnPlotData pa = new Altaxo.Graph.XYColumnPlotData(xcol,ycol);
      pa.LabelColumn = labelcol;

      Altaxo.Graph.XYLineScatterPlotStyle ps = new Altaxo.Graph.XYLineScatterPlotStyle(Altaxo.Graph.LineScatterPlotStyleKind.LineAndScatter);
      ps.XYPlotScatterStyle.Shape = Altaxo.Graph.XYPlotScatterStyles.Shape.NoSymbol;
      ps.XYPlotLineStyle.Connection = Altaxo.Graph.XYPlotLineStyles.ConnectionStyle.NoLine;
      ps.XYPlotLabelStyle = new Altaxo.Graph.XYPlotLabelStyle();
      ps.XYPlotLabelStyle.FontSize = 10;
      ps.XYPlotLabelStyle.BackgroundColor = System.Drawing.Color.LightCyan;
      ps.XYPlotLabelStyle.WhiteOut=true;
      
      layer.PlotItems.Add(new Altaxo.Graph.XYColumnPlotItem(pa,ps));
    }

    /// <summary>
    /// Plots the residual y values of a given component into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotYResiduals(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string yrescolname = GetYResidual_ColumnName(whichY,numberOfFactors);
      string yactcolname = GetYOriginal_ColumnName(whichY);
 
      // Calculate the residual if not here
      if(table[yrescolname]==null)
      {
        CalculateYResidual(table, whichY, numberOfFactors);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[yrescolname],table[_MeasurementLabel_ColumnName]);

      layer.BottomAxisTitleString = string.Format("Y original{0}",whichY);
      layer.LeftAxisTitleString   = string.Format("Y residual{0} (#factors:{1})",whichY,numberOfFactors);
    }


    /// <summary>
    /// Plots all preprocessed spectra into a newly created graph.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPreprocessedSpectra(Altaxo.Data.DataTable table)
    {
      DataTable desttable = new DataTable();
      desttable.Name = table.Name+".PS";
      CalculatePreprocessedSpectra(table, desttable);
      Current.Project.DataTableCollection.Add(desttable);

      Worksheet.Commands.PlotCommands.PlotLine(desttable,new IntegerRangeAsCollection(1,desttable.DataColumnCount-1),true,false);
    }


    /// <summary>
    /// Plots the x (spectral) residuals into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotXResiduals(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string xresidualcolname = GetXResidual_ColumnName(whichY,numberOfFactors);
      string yactcolname = GetYOriginal_ColumnName(whichY);
      
      if(table[xresidualcolname]==null)
      {
        CalculateXResidual(table,whichY,numberOfFactors);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[xresidualcolname],table[_MeasurementLabel_ColumnName]);

      layer.BottomAxisTitleString = string.Format("Y original{0}",whichY);
      layer.LeftAxisTitleString   = string.Format("X residual{0} (#factors:{1})",whichY,numberOfFactors);
    }
    
    /// <summary>
    /// Plots the predicted versus actual Y (concentration) into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotPredictedVersusActualY(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string ypredcolname = GetYPredicted_ColumnName(whichY,numberOfFactors);
      string yactcolname = GetYOriginal_ColumnName(whichY);
      if(table[ypredcolname]==null)
      {
        CalculateYPredicted(table,whichY,numberOfFactors);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[ypredcolname],table[_MeasurementLabel_ColumnName]);

      layer.BottomAxisTitleString = string.Format("Y original{0}",whichY);
      layer.LeftAxisTitleString   = string.Format("Y predicted{0} (#factors:{1})",whichY,numberOfFactors);
    }

    /// <summary>
    /// Asks the user for the preferred number of factors to use for calculation and plotting and stores that number in the 
    /// PLS content tag of the table.
    /// </summary>
    /// <param name="table">The table which contains the PLS model.</param>
    public static void QuestPreferredNumberOfFactors(Altaxo.Data.DataTable table)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;
      if(plsMemo==null)
        return;

      QuestPreferredNumberOfFactors(plsMemo);
    }

    public static void QuestPreferredNumberOfFactors(PLSContentMemento plsMemo)
    {
      // quest the number of factors to export
      Main.GUI.IntegerValueInputController ivictrl = new Main.GUI.IntegerValueInputController(
        1,
        new Main.GUI.SingleValueDialog("Number of factors","Please choose preferred number of factors(>0):")
        );

      ivictrl.Validator = new Altaxo.Main.GUI.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(!ivictrl.ShowDialog(Current.MainWindow))
        return;

      plsMemo.PreferredNumberOfFactors = ivictrl.EnteredContents;
    }

    /// <summary>
    /// Plots the rediduals of all y components invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotYResiduals(Altaxo.Data.DataTable table)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotYResiduals(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the prediction values of all y components invidually in a  graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotPredictedVersusActualY(Altaxo.Data.DataTable table)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotPredictedVersusActualY(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the x (spectral) residuals of all y components invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXResiduals(Altaxo.Data.DataTable table)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotXResiduals(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }


    /// <summary>
    /// Plots the PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    public static void PlotPRESSValue(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer)
    {
      Altaxo.Data.DataColumn ycol = table[_PRESSValue_ColumnName];
      Altaxo.Data.DataColumn xcol = table[_NumberOfFactors_ColumnName];

      Altaxo.Graph.XYColumnPlotData pa = new Altaxo.Graph.XYColumnPlotData(xcol,ycol);
      Altaxo.Graph.XYLineScatterPlotStyle ps = new Altaxo.Graph.XYLineScatterPlotStyle(Altaxo.Graph.LineScatterPlotStyleKind.LineAndScatter);
      layer.PlotItems.Add(new Altaxo.Graph.XYColumnPlotItem(pa,ps));

      layer.BottomAxisTitleString = "Number of factors";
      layer.LeftAxisTitleString   = "PRESS value";
    }

    /// <summary>
    /// Plots the PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPRESSValue(Altaxo.Data.DataTable table)
    {
      Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
      PlotPRESSValue(table,graphctrl.Doc.Layers[0]);
    }

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    public static void PlotCrossPRESSValue(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer)
    {
      Altaxo.Data.DataColumn ycol = table[_CrossPRESSValue_ColumnName];
      Altaxo.Data.DataColumn xcol = table[_NumberOfFactors_ColumnName];

      Altaxo.Graph.XYColumnPlotData pa = new Altaxo.Graph.XYColumnPlotData(xcol,ycol);
      Altaxo.Graph.XYLineScatterPlotStyle ps = new Altaxo.Graph.XYLineScatterPlotStyle(Altaxo.Graph.LineScatterPlotStyleKind.LineAndScatter);
      layer.PlotItems.Add(new Altaxo.Graph.XYColumnPlotItem(pa,ps));

      layer.BottomAxisTitleString = "Number of factors";
      layer.LeftAxisTitleString   = "Cross PRESS value";
    }

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotCrossPRESSValue(Altaxo.Data.DataTable table)
    {
      Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
      PlotCrossPRESSValue(table,graphctrl.Doc.Layers[0]);
    }


    /// <summary>
    /// Plots the x (spectral) leverage into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="preferredNumberOfFactors">The number of factors used for leverage calculation.</param>
    public static void PlotXLeverage(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer, int preferredNumberOfFactors)
    {
      string xcolname = _MeasurementLabel_ColumnName;
      string ycolname = GetXLeverage_ColumnName(preferredNumberOfFactors);
      
      if(table[ycolname]==null)
      {
        CalculateXLeverage(table,preferredNumberOfFactors);
      }

      PlotOnlyLabel(layer,table[xcolname],table[ycolname],table[_MeasurementLabel_ColumnName]);

      layer.BottomAxisTitleString = string.Format("Measurement");
      layer.LeftAxisTitleString   = string.Format("Score leverage (#factors:{0})",preferredNumberOfFactors);
    }

    /// <summary>
    /// Plots the x (spectral) leverage into a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXLeverage(Altaxo.Data.DataTable table)
    {
      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
      PlotXLeverage(table,graphctrl.Doc.Layers[0],plsMemo.PreferredNumberOfFactors);
    }


    #endregion
  
  }
}
