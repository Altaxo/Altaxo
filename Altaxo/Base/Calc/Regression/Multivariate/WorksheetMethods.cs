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

using Altaxo.Collections;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Calc.Regression.PLS;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
	/// <summary>
	/// WorksheetMethods provides common utility methods for multivariate data analysis.
	/// </summary>
	public class WorksheetMethods
	{
    /// <summary>
    /// Get the matrix of x and y values (raw data).
    /// </summary>
    /// <param name="srctable">The table where the data come from.</param>
    /// <param name="selectedColumns">The selected columns.</param>
    /// <param name="selectedRows">The selected rows.</param>
    /// <param name="selectedPropertyColumns">The selected property column(s).</param>
    /// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
    /// <param name="plsOptions">Provides information about the max number of factors and the calculation of cross PRESS value.</param>
    /// <param name="preprocessOptions">Provides information about how to preprocess the spectra.</param>
    /// <returns></returns>
    public static string GetXYMatrices(
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      bool bHorizontalOrientedSpectrum,
      out IMatrix matrixX,
      out IMatrix matrixY,
      out IROVector xOfX,
      out PLSContentMemento plsContent
      )
    {
      matrixX=null;
      matrixY=null;
      xOfX=null;
      plsContent = new PLSContentMemento();
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



      // now fill the matrix

   
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

      return null;
    }
	}
}
