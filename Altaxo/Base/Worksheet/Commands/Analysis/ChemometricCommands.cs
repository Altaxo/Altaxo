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

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contain commands concerning chemometric operations like PLS and PCA.
  /// </summary>
  public class ChemometricCommands
  {
    #region Chemometrical commands

    public static void MultiplyColumnsToMatrix(WorksheetController ctrl)
    {
      string err = MultiplyColumnsToMatrix(Current.Project,ctrl.Doc,ctrl.SelectedColumns);
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

      Altaxo.Calc.MatrixMath.REMatrix firstMat = new Altaxo.Calc.MatrixMath.REMatrix(rowsfirsthalf,halfselect);
      for(int i=0;i<halfselect;i++)
      {
        Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i]];
        for(int j=0;j<rowsfirsthalf;j++)
          firstMat[j,i] = col.GetDoubleAt(j);
      }
      
      Altaxo.Calc.MatrixMath.BEMatrix secondMat = new Altaxo.Calc.MatrixMath.BEMatrix(halfselect,rowssecondhalf);
      for(int i=0;i<halfselect;i++)
      {
        Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i+halfselect]];
        for(int j=0;j<rowssecondhalf;j++)
          secondMat[i,j] = col.GetDoubleAt(j);
      }

      // now multiply the two matrices
      Altaxo.Calc.MatrixMath.BEMatrix resultMat = new Altaxo.Calc.MatrixMath.BEMatrix(rowsfirsthalf,rowssecondhalf);
      Altaxo.Calc.MatrixMath.Multiply(firstMat,secondMat,resultMat);


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
        string err= PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,true,ivictrl.EnteredContents);
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
        string err= PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,false,ivictrl.EnteredContents);
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

      Altaxo.Calc.MatrixMath.BEMatrix matrixX;
      if(bHorizontalOrientedSpectrum)
      {
        matrixX = new Altaxo.Calc.MatrixMath.BEMatrix(numrows,numcols);
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
        matrixX = new Altaxo.Calc.MatrixMath.BEMatrix(numcols,numrows);
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
      Altaxo.Calc.MatrixMath.REMatrix factors = new Altaxo.Calc.MatrixMath.REMatrix(0,0);
      Altaxo.Calc.MatrixMath.BEMatrix loads = new Altaxo.Calc.MatrixMath.BEMatrix(0,0);
      Altaxo.Calc.MatrixMath.BEMatrix residualVariances = new Altaxo.Calc.MatrixMath.BEMatrix(0,0);
      Altaxo.Calc.MatrixMath.HorizontalVector meanX = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixX.Columns);
      // first, center the matrix
      Altaxo.Calc.MatrixMath.ColumnsToZeroMean(matrixX,meanX);
      Altaxo.Calc.MatrixMath.NIPALS_HO(matrixX,maxNumberOfFactors,1E-9,factors,loads,residualVariances);

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PCA of " + srctable.Name);

      // Fill the Table
      table.Suspend();

      // first of all store the meanscore
    {
      double meanScore = Altaxo.Calc.MatrixMath.LengthOf(meanX);
      Altaxo.Calc.MatrixMath.NormalizeRows(meanX);
    
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
    


    public static void PLSOnRows(WorksheetController ctrl)
    {
      string err= PartialLeastSquaresAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,ctrl.SelectedPropertyColumns,true);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
    public static void PLSOnColumns(WorksheetController ctrl)
    {
      string err= PartialLeastSquaresAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedColumns,ctrl.SelectedRows,ctrl.SelectedPropertyColumns,false);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
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
    /// <returns></returns>
    public static string PartialLeastSquaresAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      bool bHorizontalOrientedSpectrum
      )
    {
      Altaxo.Data.DataColumn xColumnOfX=null;
      Altaxo.Data.DataColumn labelColumnOfX=null;

      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      
      // this is the number of columns (for now), but it can be less than this in case
      // not all columns are numeric
      int prenumcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
      AscendingIntegerCollection numericDataCols = new Altaxo.Collections.AscendingIntegerCollection();

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


      bool bUseSelectedPropCols = (null!=selectedPropertyColumns && 0!=selectedPropertyColumns.Count);
      

      // this is the number of property columns (for now), but it can be less than this in case
      // not all columns are numeric
      int prenumpropcols = bUseSelectedPropCols ? selectedPropertyColumns.Count : srctable.PropCols.ColumnCount;
      int[] numericPropCols = new int[prenumpropcols];
      // check for the number of numeric property columns
      int numpropcols = 0;
      for(int i=0;i<prenumpropcols;i++)
      {
        int idx = bUseSelectedPropCols ? selectedPropertyColumns[i] : i;
        if(srctable.PropCols[idx] is Altaxo.Data.INumericColumn)
        {
          numericPropCols[numpropcols] = idx;
          numpropcols++;
        }
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


      // now check and fill in values
      Altaxo.Calc.MatrixMath.BEMatrix matrixX;
      Altaxo.Calc.MatrixMath.BEMatrix matrixY;

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
          
        // group0 is now the group of y-values
        // group1 is now the group of x-values
      
        // fill in the y-values
        matrixY = new Altaxo.Calc.MatrixMath.BEMatrix(numrows,groupcount0);
        int ccol=0;
        for(int i=0;i<numcols;i++)
        {
          if(srctable.DataColumns.GetColumnGroup(numericDataCols[i]) != group0)
            continue;

          Altaxo.Data.INumericColumn col = srctable[numericDataCols[i]] as Altaxo.Data.INumericColumn;
          
          for(int j=0;j<numrows;j++)
          {
            int rowidx = bUseSelectedRows ? selectedRows[j] : j;
            matrixY[j,ccol] = col.GetDoubleAt(rowidx);
          }
          ccol++;
        }

        // fill in the x-values
        labelColumnOfX = new Altaxo.Data.DoubleColumn();
        matrixX = new Altaxo.Calc.MatrixMath.BEMatrix(numrows,groupcount1);
        ccol=0;
        for(int i=0;i<numcols;i++)
        {
          if(srctable.DataColumns.GetColumnGroup(numericDataCols[i]) != group1)
            continue;

          Altaxo.Data.INumericColumn col = srctable[numericDataCols[i]] as Altaxo.Data.INumericColumn;
          
          for(int j=0;j<numrows;j++)
          {
            int rowidx = bUseSelectedRows ? selectedRows[j] : j;
            matrixX[j,ccol] = col.GetDoubleAt(rowidx);
            labelColumnOfX[j] = rowidx;
          }

          ccol++;
        }

        // store the corresponding X-Column of the spectra
        xColumnOfX = Altaxo.Data.DataColumn.CreateColumnOfSelectedRows(
          srctable.PropertyColumns.FindXColumnOfGroup(group1),
          numericDataCols);

      }
      else // vertically oriented spectrum -> one spectrum is one data column
      {
        // if PLS on columns, than we should have property columns selected
        // that designates the y-values
        // so count all property columns

        
        if(numpropcols<1)
          return "At least one numeric property column must exist to hold the y-values!";

        // fill in the y-values
        matrixY = new Altaxo.Calc.MatrixMath.BEMatrix(numcols,numpropcols);
        for(int i=0;i<numpropcols;i++)
        {
          Altaxo.Data.INumericColumn col = srctable.PropCols[numericPropCols[i]] as Altaxo.Data.INumericColumn;
          for(int j=0;j<numcols;j++)
          {
            matrixY[j,i] = col.GetDoubleAt(numericDataCols[j]);
          }
        
        } // end fill in yvalues

        // fill in the x-values, i.e. the spectrum
        labelColumnOfX = new Altaxo.Data.DoubleColumn();
        matrixX = new Altaxo.Calc.MatrixMath.BEMatrix(numcols,numrows);
        for(int i=0;i<numcols;i++)
        {
          labelColumnOfX[i] = numericDataCols[i];
          Altaxo.Data.INumericColumn col = srctable[numericDataCols[i]] as Altaxo.Data.INumericColumn;
          for(int j=0;j<numrows;j++)
          {
            int rowidx = bUseSelectedRows ? selectedRows[j] : j;
            matrixX[i,j] = col.GetDoubleAt(rowidx);
          }
        } // end fill in x-values

        // store the corresponding X-Column of the spectra
        xColumnOfX = Altaxo.Data.DataColumn.CreateColumnOfSelectedRows(
          srctable.DataColumns.FindXColumnOf(srctable[numericDataCols[0]]),
          bUseSelectedRows ? selectedRows: null);

      } // else vertically oriented spectrum


      // now do a PLS with it
      Altaxo.Calc.MatrixMath.BEMatrix xLoads   = new Altaxo.Calc.MatrixMath.BEMatrix(0,0);
      Altaxo.Calc.MatrixMath.BEMatrix yLoads   = new Altaxo.Calc.MatrixMath.BEMatrix(0,0);
      Altaxo.Calc.MatrixMath.BEMatrix W       = new Altaxo.Calc.MatrixMath.BEMatrix(0,0);
      Altaxo.Calc.MatrixMath.REMatrix V       = new Altaxo.Calc.MatrixMath.REMatrix(0,0);


      // Before we can apply PLS, we have to center the x and y matrices
      Altaxo.Calc.MatrixMath.HorizontalVector meanX = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixX.Columns);
      //  Altaxo.Calc.MatrixMath.HorizontalVector scaleX = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixX.Cols);
      Altaxo.Calc.MatrixMath.HorizontalVector meanY = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixY.Columns);


      Altaxo.Calc.MatrixMath.ColumnsToZeroMean(matrixX, meanX);
      Altaxo.Calc.MatrixMath.ColumnsToZeroMean(matrixY, meanY);

      int numFactors = matrixX.Columns;
      Altaxo.Calc.MatrixMath.PartialLeastSquares_HO(matrixX,matrixY,ref numFactors,xLoads,yLoads,W,V);
  

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PLS of " + srctable.Name);

      // Fill the Table
      table.Suspend();

      table.DataColumns.Add(xColumnOfX,"XOfX",Altaxo.Data.ColumnKind.X,0);

      // store the x-loads - careful - they are horizontal in the matrix
      for(int i=0;i<xLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

        for(int j=0;j<xLoads.Columns;j++)
          col[j] = xLoads[i,j];
          
        table.DataColumns.Add(col,"XLoad"+i.ToString(),Altaxo.Data.ColumnKind.V,0);
      }

      // now store the loads - careful - they are horizontal in the matrix
      for(int i=0;i<yLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        
        for(int j=0;j<yLoads.Columns;j++)
          col[j] = yLoads[i,j];
        
        table.DataColumns.Add(col,"YLoad"+i.ToString(),Altaxo.Data.ColumnKind.V,1);
      }

      // now store the weights - careful - they are horizontal in the matrix
      for(int i=0;i<W.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
        for(int j=0;j<W.Columns;j++)
          col[j] = W[i,j];
        
        table.DataColumns.Add(col,"Weight"+i.ToString(),Altaxo.Data.ColumnKind.V,0);
      }

      // now store the cross product vector - it is a horizontal vector
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int j=0;j<V.Columns;j++)
        col[j] = V[0,j];
      table.DataColumns.Add(col,"CrossP",Altaxo.Data.ColumnKind.V,3);
    }

    {
      // now a cross validation - this can take a long time for bigger matrices
      Altaxo.Calc.IMatrix crossPRESSMatrix;
      Altaxo.Calc.MatrixMath.PartialLeastSquares_CrossValidation_HO(matrixX,matrixY,numFactors, true, out crossPRESSMatrix);

      
      Altaxo.Data.DoubleColumn xNumFactor= new Altaxo.Data.DoubleColumn();

      
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<crossPRESSMatrix.Rows;i++)
      { 
        xNumFactor[i]=i;
        col[i] = crossPRESSMatrix[i,0];
      }
      table.DataColumns.Add(xNumFactor,"NumberOfFactors",Altaxo.Data.ColumnKind.X,4);
      table.DataColumns.Add(col,"CrossPRESS",Altaxo.Data.ColumnKind.V,4);
    }

      // calculate the self predicted y values - for one factor and for two
      Altaxo.Calc.IMatrix yPred = new Altaxo.Calc.MatrixMath.BEMatrix(matrixY.Rows,matrixY.Columns);
      Altaxo.Data.DoubleColumn presscol = new Altaxo.Data.DoubleColumn();
      
      table.DataColumns.Add(presscol,"PRESS",Altaxo.Data.ColumnKind.V,4);
      presscol[0] = Altaxo.Calc.MatrixMath.SumOfSquares(matrixY); // gives the press for 0 factors, i.e. the variance of the y-matrix

      // now add the original Y-Columns
      for(int i=0;i<matrixY.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<matrixY.Rows;j++)
          col[j]=matrixY[j,i] + meanY[0,i];
        table.DataColumns.Add(col,"YOrg"+i.ToString(),Altaxo.Data.ColumnKind.X,5+i);
      }
      
      table.DataColumns.Add(labelColumnOfX,"XLabel",Altaxo.Data.ColumnKind.Label,5);

      // and now the predicted Y 
      for(int nFactor=1;nFactor<=numFactors;nFactor++)
      {
        Altaxo.Calc.MatrixMath.PartialLeastSquares_Predict_HO(matrixX,xLoads,yLoads,W,V,nFactor, ref yPred);

        // Calculate the PRESS value
        presscol[nFactor] = Altaxo.Calc.MatrixMath.SumOfSquaredDifferences(matrixY,yPred);


        // now store the predicted y - careful - they are horizontal in the matrix,
        // but we store them vertically now
        for(int i=0;i<yPred.Columns;i++)
        {
          Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
          for(int j=0;j<yPred.Rows;j++)
            col[j] = yPred[j,i] + meanY[0,i];
        
          table.DataColumns.Add(col,"YPred"+nFactor.ToString()+ "_" + i.ToString(),Altaxo.Data.ColumnKind.V,5+i);
        }
      } // for nFactor...




      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }



    #endregion

	
  }
}
