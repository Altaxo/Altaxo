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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;

using Altaxo.Collections;
using Altaxo.Worksheet.GUI;
using Altaxo.Data;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contain statistic commands.
  /// </summary>
  public class StatisticCommands
  {
    #region Statistical commands

    public static void StatisticsOnColumns(WorksheetController ctrl)
    {
      StatisticsOnColumns(Current.Project,ctrl.DataTable,ctrl.SelectedDataColumns,ctrl.SelectedDataRows);
    }

    public static void StatisticsOnColumns(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows
      )
    {
      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;

      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

      if(numcols==0)
        return; // nothing selected

      Data.DataTable table = null; // the created table


      // add a text column and some double columns
      // note: statistics is only possible for numeric columns since
      // otherwise in one column doubles and i.e. dates are mixed, which is not possible

      // 1st column is the name of the column of which the statistics is made
      Data.TextColumn colCol = new Data.TextColumn();
    
      // 2nd column is the mean
      Data.DoubleColumn colMean = new Data.DoubleColumn();

      // 3rd column is the standard deviation
      Data.DoubleColumn colSd = new Data.DoubleColumn();

      // 4th column is the standard e (N)
      Data.DoubleColumn colSe = new Data.DoubleColumn();

      // 5th column is the sum
      Data.DoubleColumn colSum = new Data.DoubleColumn();

      // 6th column is the number of items for statistics
      Data.DoubleColumn colN = new Data.DoubleColumn();

      int currRow=0;
      for(int si=0;si<numcols;si++)
      {
        Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
        if(!(col is Altaxo.Data.INumericColumn))
          continue;

        int rows = bUseSelectedRows ? selectedRows.Count : srctable.DataColumns.RowCount;
        if(rows==0)
          continue;

        // now do the statistics 
        Data.INumericColumn ncol = (Data.INumericColumn)col;
        double sum=0;
        double sumsqr=0;
        int NN=0;
        for(int i=0;i<rows;i++)
        {
          double val = bUseSelectedRows ? ncol[selectedRows[i]] : ncol[i];
          if(Double.IsNaN(val))
            continue;

          NN++;
          sum+=val;
          sumsqr+=(val*val);
        }
        // now fill a new row in the worksheet

        if(NN>0)
        {
          double mean = sum/NN;
          double ymy0sqr = sumsqr - sum*sum/NN;
          if(ymy0sqr<0) ymy0sqr=0; // if this is lesser zero, it is a rounding error, so set it to zero
          double sd = NN>1 ? Math.Sqrt(ymy0sqr/(NN-1)) : 0;
          double se = sd/Math.Sqrt(NN);

          colCol[currRow] = col.Name;
          colMean[currRow] = mean; // mean
          colSd[currRow] = sd;
          colSe[currRow] = se;
          colSum[currRow] = sum;
          colN[currRow] = NN;
          currRow++; // for the next column
        }
      } // for all selected columns
      
  
      if(currRow!=0)
      {
        table = new Altaxo.Data.DataTable("Statistics of " + srctable.Name);
        table.DataColumns.Add(colCol,"Col",Altaxo.Data.ColumnKind.X);

        // new : add a copy of all property columns; can be usefull
        for (int i = 0; i < srctable.PropertyColumnCount; i++)
        {
          DataColumn originalColumn = srctable.PropertyColumns[i];
          DataColumn clonedColumn = (DataColumn)originalColumn.Clone();
          clonedColumn.Clear();
          for (int si = 0; si < numcols; si++)
          {
            int idx = bUseSelectedColumns ? selectedColumns[si] : si;
            clonedColumn[si] = originalColumn[idx];
          }
          table.DataColumns.Add(clonedColumn, srctable.PropertyColumns.GetColumnName(i), srctable.PropertyColumns.GetColumnKind(i), srctable.PropertyColumns.GetColumnGroup(i));
        }

        table.DataColumns.Add(colMean,"Mean");
        table.DataColumns.Add(colSd,"Sd");
        table.DataColumns.Add(colSe,"Se");
        table.DataColumns.Add(colSum,"Sum");
        table.DataColumns.Add(colN,"N");

        mainDocument.DataTableCollection.Add(table);
        // create a new worksheet without any columns
        Current.ProjectService.CreateNewWorksheet(table);
      }
    }


    public static void StatisticsOnRows(WorksheetController ctrl)
    {
      StatisticsOnRows(Current.Project,ctrl.DataTable,ctrl.SelectedDataColumns,ctrl.SelectedDataRows);
    }

    public static void StatisticsOnRows(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows
      )
    {
      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
      if(numcols==0)
        return; // nothing selected

      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);
      int numrows = bUseSelectedRows ? selectedRows.Count : srctable.DataColumns.RowCount;
      if(numrows==0)
        return;

      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable();
      // add a text column and some double columns
      // note: statistics is only possible for numeric columns since
      // otherwise in one column doubles and i.e. dates are mixed, which is not possible

      // 1st column is the mean, and holds the sum during the calculation
      Data.DoubleColumn c1 = new Data.DoubleColumn();

      // 2rd column is the standard deviation, and holds the square sum during calculation
      Data.DoubleColumn c2 = new Data.DoubleColumn();

      // 3th column is the standard e (N)
      Data.DoubleColumn c3 = new Data.DoubleColumn();

      // 4th column is the sum
      Data.DoubleColumn c4 = new Data.DoubleColumn();

      // 5th column is the number of items for statistics
      Data.DoubleColumn c5 = new Data.DoubleColumn();
      
      table.DataColumns.Add(c1,"Mean");
      table.DataColumns.Add(c2,"sd");
      table.DataColumns.Add(c3,"se");
      table.DataColumns.Add(c4,"Sum");
      table.DataColumns.Add(c5,"N");

      table.Suspend();

      
      // first fill the cols c1, c2, c5 with zeros because we want to sum up 
      for(int i=0;i<numrows;i++)
      {
        c1[i]=0;
        c2[i]=0;
        c5[i]=0;
      }
  
      
      for(int si=0;si<numcols;si++)
      {
        Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
        if(!(col is Altaxo.Data.INumericColumn))
          continue;

        // now do the statistics 
        Data.INumericColumn ncol = (Data.INumericColumn)col;
        for(int i=0;i<numrows;i++)
        {
          double val = bUseSelectedRows ? ncol[selectedRows[i]] : ncol[i];
          if(Double.IsNaN(val))
            continue;

          c1[i] += val;
          c2[i] += val*val;
          c5[i] += 1;
        }
      } // for all selected columns

      
      // now calculate the statistics
      for(int i=0;i<numrows;i++)
      {
        // now fill a new row in the worksheet
        double NN=c5[i];
        double sum=c1[i];
        double sumsqr=c2[i];
        if(NN>0)
        {
          double mean = c1[i]/NN;
          double ymy0sqr = sumsqr - sum*sum/NN;
          if(ymy0sqr<0) ymy0sqr=0; // if this is lesser zero, it is a rounding error, so set it to zero
          double sd = NN>1 ? Math.Sqrt(ymy0sqr/(NN-1)) : 0;
          double se = sd/Math.Sqrt(NN);

          c1[i] = mean; // mean
          c2[i] = sd;
          c3[i] = se;
          c4[i] = sum;
          c5[i] = NN;
        }
      } // for all rows
  
      // if a table was created, we add the table to the data set and
      // create a worksheet
      if(null!=table)
      {
        table.Resume();
        mainDocument.DataTableCollection.Add(table);
        // create a new worksheet without any columns
        Current.ProjectService.CreateNewWorksheet(table);

      }
    }


    #endregion

  }
}
