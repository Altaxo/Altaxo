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
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Data;

namespace Altaxo.Data
{
  /// <summary>
  /// Contain statistic commands applied to the table.
  /// </summary>
  public static class Statistics
  {
		public static readonly string DefaultColumnNameColumnName = "Col";
		public static readonly string DefaultRowNumberColumnName = "Row";
		public static readonly string DefaultMeanColumnName = "Mean";
		public static readonly string DefaultStandardDeviationColumnName = "sd";
		public static readonly string DefaultStandardErrorColumnName = "se";
		public static readonly string DefaultSumColumnName = "Sum";
		public static readonly string DefaultNumberOfItemsColumnName = "N";


		/// <summary>
		/// Calculates statistics of selected columns. Returns a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		public static DataTable DoStatisticsOnColumns(
			this DataTable srctable,
			IAscendingIntegerCollection selectedColumns,
			IAscendingIntegerCollection selectedRows
			)
		{
			var table = CreateStatisticalTable(srctable, selectedColumns);
			DoStatisticsOnColumns(srctable.DataColumns, selectedColumns, selectedRows, table.DataColumns);
			return table;
		}

		/// <summary>
		/// Calculates statistics of selected columns. Returns a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		public static DataTable DoStatisticsOnColumns(
			this DataColumnCollection srctable,
			IAscendingIntegerCollection selectedColumns,
			IAscendingIntegerCollection selectedRows
			)
		{
			var table = CreateStatisticalTable(srctable, selectedColumns);
			DoStatisticsOnColumns(srctable, selectedColumns, selectedRows, table.DataColumns);
			return table;
		}


		/// <summary>
		/// Calculates statistics of selected columns. Creates a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		/// <param name="destinationTable">The table where the statistical results are written to.</param>
    public static void DoStatisticsOnColumns(
      this DataColumnCollection srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
			DataColumnCollection destinationTable
      )
    {
      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.ColumnCount;

      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

      if(numcols==0)
        return; // nothing selected

      


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

        int rows = bUseSelectedRows ? selectedRows.Count : srctable.RowCount;
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
				destinationTable.EnsureExistence(DefaultColumnNameColumnName, typeof(TextColumn), ColumnKind.X, 0).Append(colCol);
				AppendStatisticalData(destinationTable, colMean, colSd, colSe, colSum, colN);
      }
    }


		/// <summary>
		/// Calculates statistics of selected columns. Creates a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		public static DataTable DoStatisticsOnRows(
			this DataTable srctable,
			IAscendingIntegerCollection selectedColumns,
			IAscendingIntegerCollection selectedRows
			)
		{
			var table = CreateStatisticalTable();
			DoStatisticsOnColumns(srctable.DataColumns, selectedColumns, selectedRows, table.DataColumns);
			return table;
		}
	
		/// <summary>
		/// Calculates statistics of selected columns. Creates a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		/// <param name="destinationTable">The table where the statistical results are written to.</param>
		public static DataTable DoStatisticsOnRows(
			this DataColumnCollection srctable,
			IAscendingIntegerCollection selectedColumns,
			IAscendingIntegerCollection selectedRows
			)
		{
			var table = CreateStatisticalTable();
			DoStatisticsOnColumns(srctable,selectedColumns,selectedRows,table.DataColumns);
			return table;
		}


		/// <summary>
		/// Calculates statistics of selected columns. Creates a new table where the statistical data will be written to.
		/// </summary>
		/// <param name="srctable">Source table.</param>
		/// <param name="selectedColumns">Selected data columns in the source table.</param>
		/// <param name="selectedRows">Selected rows in the source table.</param>
		/// <param name="destinationTable">The table where the statistical results are written to.</param>
    public static void DoStatisticsOnRows(
      this DataColumnCollection srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
			DataColumnCollection destinationTable
      )
    {
      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.ColumnCount;
      if(numcols==0)
        return; // nothing selected

      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);
      int numrows = bUseSelectedRows ? selectedRows.Count : srctable.RowCount;
      if(numrows==0)
        return;


			Data.DoubleColumn cRows = new DoubleColumn();

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
					int row = bUseSelectedRows ? selectedRows[i] : i;
					cRows[i] = row;

					double val = ncol[row];
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


			destinationTable.EnsureExistence(DefaultRowNumberColumnName, typeof(DoubleColumn), ColumnKind.X, 0).Append(cRows);
			AppendStatisticalData(destinationTable, c1, c2, c2, c4, c5);
    }


		/// <summary>
		/// Creates a table for statistics on columns. Property columns are included in the statistical table.
		/// </summary>
		/// <param name="srcTable"></param>
		/// <param name="selectedColumns"></param>
		/// <returns></returns>
		private static DataTable CreateStatisticalTable(DataTable srcTable, IAscendingIntegerCollection selectedColumns)
		{
			DataTable result = new DataTable();
			result.Name = Altaxo.Main.NameHelper.PrependToName(srcTable.Name,"Statistics of ");


			result.DataColumns.Add(new TextColumn(), DefaultColumnNameColumnName, ColumnKind.X, 0);
			AddSourcePropertyColumns(srcTable, selectedColumns, result);
			AddStatisticColumns(result);
			return result;
		}

		/// <summary>
		/// Create a statistical table for statistics on columns. Property columns are not included in the statistical table.
		/// </summary>
		/// <param name="srcTable"></param>
		/// <param name="selectedColumns"></param>
		/// <returns></returns>
		private static DataTable CreateStatisticalTable(DataColumnCollection srcTable, IAscendingIntegerCollection selectedColumns)
		{
			DataTable result = new DataTable();

			result.DataColumns.Add(new TextColumn(), DefaultColumnNameColumnName, ColumnKind.X, 0);
			AddStatisticColumns(result);
			return result;
		}

		/// <summary>
		/// Creates a statistical table for Statistics on Rows.
		/// </summary>
		/// <returns></returns>
		private static DataTable CreateStatisticalTable()
		{
			DataTable result = new DataTable();

			result.DataColumns.Add(new DoubleColumn(), DefaultRowNumberColumnName, ColumnKind.X, 0);
			AddStatisticColumns(result);
			return result;
		}

		private static void AddStatisticColumns(DataTable result)
		{

			result.DataColumns.Add(new DoubleColumn(), DefaultMeanColumnName, ColumnKind.Y, 0);
			result.DataColumns.Add(new DoubleColumn(), DefaultStandardDeviationColumnName, ColumnKind.Y, 0);
			result.DataColumns.Add(new DoubleColumn(), DefaultStandardErrorColumnName, ColumnKind.Y, 0);
			result.DataColumns.Add(new DoubleColumn(), DefaultSumColumnName, ColumnKind.Y, 0);
			result.DataColumns.Add(new DoubleColumn(), DefaultNumberOfItemsColumnName, ColumnKind.Y, 0);
		}

		private static void AppendStatisticalData(DataColumnCollection destinationTable, Data.DoubleColumn colMean, Data.DoubleColumn colSd, Data.DoubleColumn colSe, Data.DoubleColumn colSum, Data.DoubleColumn colN)
		{
			destinationTable.EnsureExistence(DefaultMeanColumnName, typeof(DoubleColumn), ColumnKind.Y, 0).Append(colMean);
			destinationTable.EnsureExistence(DefaultStandardDeviationColumnName, typeof(DoubleColumn), ColumnKind.Y, 0).Append(colSd);
			destinationTable.EnsureExistence(DefaultStandardErrorColumnName, typeof(DoubleColumn), ColumnKind.Y, 0).Append(colSe);
			destinationTable.EnsureExistence(DefaultSumColumnName, typeof(DoubleColumn), ColumnKind.Y, 0).Append(colSum);
			destinationTable.EnsureExistence(DefaultNumberOfItemsColumnName, typeof(DoubleColumn), ColumnKind.Y, 0).Append(colN);
		}


		private static void AddSourcePropertyColumns(DataTable srctable, IAscendingIntegerCollection selectedColumns, DataTable destinationTable)
		{
			bool bUseSelectedColumns = (null != selectedColumns && 0 != selectedColumns.Count);
			int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumnCount;

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
				destinationTable.DataColumns.Add(clonedColumn, srctable.PropertyColumns.GetColumnName(i), srctable.PropertyColumns.GetColumnKind(i), srctable.PropertyColumns.GetColumnGroup(i));
			}
		}

  }
}
