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
   
    #region MultiplyColumnsToMatrix
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
    
    #endregion

    #region PLS

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
    /// This class is for remembering the content of the PLS calibration and where to found the original data.
    /// </summary>
    public class PLSContentMemento
    {
      /// <summary>Represents that indices that build up one spectrum.</summary>
      public  Altaxo.Collections.IAscendingIntegerCollection SpectralIndices;

      /// <summary>
      /// Represents the indices of the measurements.
      /// </summary>
      public Altaxo.Collections.IAscendingIntegerCollection MeasurementIndices;

      /// <summary>
      /// Represents the indices of the concentrations.
      /// </summary>
      public Altaxo.Collections.IAscendingIntegerCollection ConcentrationIndices;

      /// <summary>
      /// True if the spectrum is horizontal oriented, i.e. is in one row. False if the spectrum is one column.
      /// </summary>
      public bool SpectrumIsRow;
    
      /// <summary>
      /// Get/sets the name of the table containing the original data.
      /// </summary>
      public string TableName;

      /// <summary>
      /// Number of factors for calculation and plotting.
      /// </summary>
      int _PreferredNumberOfFactors;


      /// <summary>
      /// Gets the number of measurement = number of spectra
      /// </summary>
      public int NumberOfMeasurements
      {
        get { return MeasurementIndices.Count; }
      }

      /// <summary>
      /// Gets the number of spectral data per specta, i.e. number of wavelengths, frequencies etc.
      /// </summary>
      public int NumberOfSpectralData
      {
        get { return SpectralIndices.Count; }
      }

      /// <summary>
      /// Gets the number of concentration data, i.e. number of output variables.
      /// </summary>
      public int NumberOfConcentrationData
      {
        get { return ConcentrationIndices.Count; }
      }


      /// <summary>
      /// Get/sets the number of factors used  for calculation residuals, plotting etc.
      /// </summary>
      public int PreferredNumberOfFactors
      {
        get { return _PreferredNumberOfFactors; }
        set { _PreferredNumberOfFactors = value; }
      }
    }



    public class PLS2CalibrationModel
    {
      Altaxo.Calc.IROMatrix _xMean;
      Altaxo.Calc.IROMatrix _xScale;
      Altaxo.Calc.IROMatrix _yMean;
      Altaxo.Calc.IROMatrix _yScale;

      Altaxo.Calc.IROMatrix _xWeights;
      Altaxo.Calc.IROMatrix _xLoads;
      Altaxo.Calc.IROMatrix _yLoads;
      Altaxo.Calc.IROMatrix _crossProduct;

      int _numberOfX;
      int _numberOfY;
      int _numberOfFactors;

      public Altaxo.Calc.IROMatrix XMean
      {
        get { return _xMean; }
        set { _xMean = value; }
      }

      public Altaxo.Calc.IROMatrix XScale
      {
        get { return _xScale; }
        set { _xScale = value; }
      }

      public Altaxo.Calc.IROMatrix YMean
      {
        get { return _yMean; }
        set { _yMean = value; }
      }

      public Altaxo.Calc.IROMatrix YScale
      {
        get { return _yScale; }
        set { _yScale = value; }
      }

      public Altaxo.Calc.IROMatrix XWeights
      {
        get { return _xWeights; }
        set { _xWeights = value; }
      }

      public Altaxo.Calc.IROMatrix XLoads
      {
        get { return _xLoads; }
        set { _xLoads = value; }
      }

      public Altaxo.Calc.IROMatrix YLoads
      {
        get { return _yLoads; }
        set { _yLoads = value; }
      }

      public Altaxo.Calc.IROMatrix CrossProduct
      {
        get { return _crossProduct; }
        set { _crossProduct = value; }
      }

      public int NumberOfX
      {
        get { return _numberOfX; }
        set { _numberOfX = value; }
      }

      public int NumberOfY
      {
        get { return _numberOfY; }
        set { _numberOfY = value; }
      }

      public int NumberOfFactors
      {
        get { return _numberOfFactors; }
        set { _numberOfFactors = value; }
      }



    }



    const int _PredictedYColumnGroup = 5;
    const int _ResidualYColumnGroup = 5;

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
          srctable.DataColumns.FindXColumnOf(srctable[spectralIndices[0]]),spectralIndices);

      } // else vertically oriented spectrum



      // now fill the matrix

      
      // now check and fill in values
      Altaxo.Calc.MatrixMath.BEMatrix matrixX;
      Altaxo.Calc.MatrixMath.BEMatrix matrixY;


      // fill in the y-values
      matrixY = new Altaxo.Calc.MatrixMath.BEMatrix(measurementIndices.Count,concentrationIndices.Count);
      for(int i=0;i<concentrationIndices.Count;i++)
      {
        Altaxo.Data.INumericColumn col = concentration[concentrationIndices[i]] as Altaxo.Data.INumericColumn;
        for(int j=0;j<measurementIndices.Count;j++)
        {
          matrixY[j,i] = col.GetDoubleAt(measurementIndices[j]);
        }
      } // end fill in yvalues

      matrixX = new Altaxo.Calc.MatrixMath.BEMatrix(measurementIndices.Count,spectralIndices.Count);
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


      // Store X-Mean and X-Scale
      Altaxo.Data.DoubleColumn colXMean = new Altaxo.Data.DoubleColumn();
      Altaxo.Data.DoubleColumn colXScale = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<matrixX.Columns;i++)
      {
        colXMean[i] = meanX[i];
        colXScale[i] = 1;
      }

      table.DataColumns.Add(colXMean,"XMean",Altaxo.Data.ColumnKind.V,0);
      table.DataColumns.Add(colXScale,"XScale",Altaxo.Data.ColumnKind.V,0);


      // store the x-loads - careful - they are horizontal in the matrix
      for(int i=0;i<xLoads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

        for(int j=0;j<xLoads.Columns;j++)
          col[j] = xLoads[i,j];
          
        table.DataColumns.Add(col,"XLoad"+i.ToString(),Altaxo.Data.ColumnKind.V,0);
      }


      // store the y-mean and y-scale
      Altaxo.Data.DoubleColumn colYMean = new Altaxo.Data.DoubleColumn();
      Altaxo.Data.DoubleColumn colYScale = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<yLoads.Columns;i++)
      {
        colYMean[i] = meanY[i];
        colYScale[i] = 1;
      }

      table.DataColumns.Add(colYMean, "YMean",Altaxo.Data.ColumnKind.V,1);
      table.DataColumns.Add(colYScale,"YScale",Altaxo.Data.ColumnKind.V,1);



      // now store the y-loads - careful - they are horizontal in the matrix
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
        
        table.DataColumns.Add(col,"XWeight"+i.ToString(),Altaxo.Data.ColumnKind.V,0);
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
        Altaxo.Calc.MatrixMath.PartialLeastSquares_Predict_HO(matrixX,xLoads,yLoads,W,V,nFactor, yPred);

        // Calculate the PRESS value
        presscol[nFactor] = Altaxo.Calc.MatrixMath.SumOfSquaredDifferences(matrixY,yPred);


        // now store the predicted y - careful - they are horizontal in the matrix,
        // but we store them vertically now
        for(int i=0;i<yPred.Columns;i++)
        {
          Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
          for(int j=0;j<yPred.Rows;j++)
            col[j] = yPred[j,i] + meanY[0,i];
        
          table.DataColumns.Add(col,"YPred"+nFactor.ToString()+ "_" + i.ToString(),Altaxo.Data.ColumnKind.V,_PredictedYColumnGroup);
        }
      } // for nFactor...


      table.SetTableProperty("Content",plsContent);

      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }


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

        sel.Clear();
        col = _table["XMean"];
        if(col==null) NotFound("XMean");
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.XMean = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);

        sel.Clear();
        col = _table["XScale"];
        if(col==null) NotFound("XScale");
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.XScale = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        
        sel.Clear();
        col = _table["YMean"];
        if(col==null) NotFound("YMean");
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.YMean = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfY);

        sel.Clear();
        col = _table["YScale"];
        if(col==null) NotFound("YScale");
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.YScale = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfY);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = "XWeight"+i.ToString();
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XWeights = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = "XLoad"+i.ToString();
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XLoads = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);


        sel.Clear();
        for(int i=0;i<_numberOfFactors;i++)
        {
          string colname = "YLoad"+i.ToString();
          col = _table[colname];
          if(col==null) NotFound(colname);
          sel.Add(_table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.YLoads = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfX);

        
        sel.Clear();
        col = _table["CrossP"];
        if(col==null) NotFound("CrossP");
        sel.Add(_table.DataColumns.GetColumnNumber(col));
        calibrationSet.XMean = new Altaxo.Calc.DataColumnToRowMatrixWrapper(_table.DataColumns,sel,_numberOfFactors);


        

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
        Altaxo.Data.DataColumn col = table.DataColumns["XLoad0"];
        if(col==null) NotFound("XLoad0");
        return col.Count;
      }

      static int GetNumberOfY(Altaxo.Data.DataTable table)
      {
        Altaxo.Data.DataColumn col = table.DataColumns["YLoad0"];
        if(col==null) NotFound("YLoad0");
        return col.Count;
      }

      static int GetNumberOfFactors(Altaxo.Data.DataTable table)
      {
        Altaxo.Data.DataColumn col = table.DataColumns["CrossP"];
        if(col==null) NotFound("CrossP");
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

        col = _table.DataColumns["XOfX"] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound("XOfX");
        WriteVector("XOfX",col, _numberOfX);

        col = _table.DataColumns["XMean"] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound("XMean");
        WriteVector("XMean",col, _numberOfX);

        col = _table.DataColumns["XScale"] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound("XScale");
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
          string colname = "XLoad"+i.ToString();
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
          string colname = "XWeight"+i.ToString();
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

        col = _table.DataColumns["YMean"] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound("YMean");
        WriteVector("YMean",col, _numberOfY);

        col = _table.DataColumns["YScale"] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound("YScale");
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
          string colname = "YLoad"+i.ToString();
          col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
          if(null==col) NotFound(colname);
          WriteVector(colname,col, _numberOfY);
        }
        _writer.WriteEndElement();
      }

      void WriteCrossProductData()
      {
        Altaxo.Data.DoubleColumn col=null;

        col = _table.DataColumns["CrossP"] as Altaxo.Data.DoubleColumn;
        if(null==col) NotFound("CrossP");
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

    #region PLS Plot Commands

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    public static void PlotCrossPRESS(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer)
    {
      Altaxo.Data.DataColumn ycol = table["CrossPRESS"];
      Altaxo.Data.DataColumn xcol = table["NumberOfFactors"];

      Altaxo.Graph.XYColumnPlotData pa = new Altaxo.Graph.XYColumnPlotData(xcol,ycol);
      Altaxo.Graph.XYLineScatterPlotStyle ps = new Altaxo.Graph.XYLineScatterPlotStyle(Altaxo.Graph.LineScatterPlotStyleKind.LineAndScatter);
      layer.PlotItems.Add(new Altaxo.Graph.XYColumnPlotItem(pa,ps));

      layer.BottomAxisTitleString = "Number of factors";
      layer.LeftAxisTitleString   = "Cross PRESS value";
    }

    /// <summary>
    /// Gets the column name of a Y-Residual column
    /// </summary>
    /// <param name="whichY">Number of y-value.</param>
    /// <param name="numberOfFactors">Number of factors for which the redidual is calculated.</param>
    /// <returns>The name of the column.</returns>
    public static string GetYResidualColumnName(int whichY, int numberOfFactors)
    {
      return string.Format("YResidual{0}.{1}",whichY,numberOfFactors);
    }


    /// <summary>
    /// Using the information in the plsMemo, gets the matrix of original spectra. The spectra are horizontal in the matrix, i.e. each spectra is a matrix row.
    /// </summary>
    /// <param name="plsMemo">The PLS memento containing the information about the location of the original data.</param>
    /// <returns>The matrix of the original spectra.</returns>
    public static Altaxo.Calc.IMatrix GetOriginalSpectra(PLSContentMemento plsMemo)
    {
      string tablename = plsMemo.TableName;

      Altaxo.Data.DataTable srctable = Current.Project.DataTableCollection[tablename];

      if(srctable==null)
        throw new ApplicationException(string.Format("Table[{0}] containing original spectral data not found!",tablename));
    
      Altaxo.Collections.IAscendingIntegerCollection spectralIndices = plsMemo.SpectralIndices;
      Altaxo.Collections.IAscendingIntegerCollection measurementIndices = plsMemo.MeasurementIndices;

      
      Altaxo.Calc.MatrixMath.BEMatrix matrixX = 
        new Altaxo.Calc.MatrixMath.BEMatrix(measurementIndices.Count,spectralIndices.Count);
      
 

      if(plsMemo.SpectrumIsRow)
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
    /// Using the information in the plsMemo, gets the matrix of original Y (concentration) data.
    /// </summary>
    /// <param name="plsMemo">The PLS mememto containing the information where to find the original data.</param>
    /// <returns>Matrix of orignal Y (concentration) data.</returns>
    public static Altaxo.Calc.IMatrix GetOriginalY(PLSContentMemento plsMemo)
    {
      string tablename = plsMemo.TableName;

      Altaxo.Data.DataTable srctable = Current.Project.DataTableCollection[tablename];

      if(srctable==null)
        throw new ApplicationException(string.Format("Table[{0}] containing original spectral data not found!",tablename));

      Altaxo.Data.DataColumnCollection concentration = plsMemo.SpectrumIsRow ? srctable.DataColumns : srctable.PropertyColumns;
      Altaxo.Collections.IAscendingIntegerCollection concentrationIndices = plsMemo.ConcentrationIndices;
      Altaxo.Collections.IAscendingIntegerCollection measurementIndices = plsMemo.MeasurementIndices;

      // fill in the y-values
      Altaxo.Calc.MatrixMath.BEMatrix matrixY = new Altaxo.Calc.MatrixMath.BEMatrix(measurementIndices.Count,concentrationIndices.Count);
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


    public static Altaxo.Data.DataColumn CalculateYResidual(Altaxo.Data.DataTable table, int whichY, int numberOfFactors)
    {
   

      PLSContentMemento plsMemo = table.GetTableProperty("Content") as PLSContentMemento;

      if(plsMemo==null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      PLS2CalibrationModel calib;
      PLSCalibrationModelExporter exporter = new PLSCalibrationModelExporter(table,numberOfFactors);
      exporter.Export(out calib);


      Altaxo.Calc.IMatrix matrixX = GetOriginalSpectra(plsMemo);
      Altaxo.Calc.MatrixMath.SubtractRow(matrixX,calib.XMean,0,matrixX);
      Altaxo.Calc.MatrixMath.DivideRow(matrixX,calib.XScale,0,0,matrixX);

      Altaxo.Calc.IMatrix matrixY = GetOriginalY(plsMemo);


      Altaxo.Calc.MatrixMath.BEMatrix predictedY = new Altaxo.Calc.MatrixMath.BEMatrix(matrixX.Rows,calib.NumberOfY);
      Altaxo.Calc.MatrixMath.PartialLeastSquares_Predict_HO(
        matrixX,
        calib.XLoads,
        calib.YLoads,
        calib.XWeights,
        calib.CrossProduct,
        numberOfFactors,
        predictedY);

      // mean and scale prediced Y
      Altaxo.Calc.MatrixMath.MultiplyRow(predictedY,calib.YScale,0,predictedY);
      Altaxo.Calc.MatrixMath.AddRow(predictedY,calib.YMean,0,predictedY);

      // subract the original y data
      Altaxo.Calc.MatrixMath.SubtractRow(predictedY,matrixY,whichY,predictedY);

      // insert a column with the proper name into the table and fill it
      string ycolname = GetYResidualColumnName(whichY,numberOfFactors);
      Altaxo.Data.DoubleColumn ycolumn = new Altaxo.Data.DoubleColumn();

      for(int i=0;i<predictedY.Rows;i++)
        ycolumn[i] = predictedY[i,whichY];
      
      table.DataColumns.Add(ycolumn,ycolname,Altaxo.Data.ColumnKind.V,_ResidualYColumnGroup);

      return ycolumn;
    }

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotYResiduals(Altaxo.Data.DataTable table, Altaxo.Graph.XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string ycolname = string.Format("YResidual{0}.{1}",whichY,numberOfFactors);

     
      Altaxo.Data.DataColumn ycol = table[ycolname];

      // Calculate the residual if not here
      if(ycol==null)
      {
        ycol = CalculateYResidual(table, whichY, numberOfFactors);
      }

      Altaxo.Data.DataColumn xcol = table.DataColumns.FindXColumnOf(ycol);

      Altaxo.Graph.XYColumnPlotData pa = new Altaxo.Graph.XYColumnPlotData(xcol,ycol);
      Altaxo.Graph.XYLineScatterPlotStyle ps = new Altaxo.Graph.XYLineScatterPlotStyle(Altaxo.Graph.LineScatterPlotStyleKind.LineAndScatter);
      layer.PlotItems.Add(new Altaxo.Graph.XYColumnPlotItem(pa,ps));

      layer.BottomAxisTitleString = "measurement number";
      layer.LeftAxisTitleString   = string.Format("Y residual{0} (#factors:{1})",whichY,numberOfFactors);
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
    /// Plots the rediduals of all y components invidually in a single graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotAllYResidualsIndividually(Altaxo.Data.DataTable table)
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

    #endregion
	
  }
}
