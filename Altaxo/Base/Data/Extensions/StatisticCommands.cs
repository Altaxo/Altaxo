#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using Altaxo.Collections;

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
    public static readonly string DefaultSumSqrColumnName = "SumSqr";
    public static readonly string DefaultNumberOfItemsColumnName = "N";
    public static readonly string DefaultFractionInOneSigmaColumnName = "FractionInOneSigma";
    public static readonly string DefaultFractionInTwoSigmaColumnName = "FractionInTwoSigma";
    public static readonly string DefaultFractionInThreeSigmaColumnName = "FractionInThreeSigma";
    public static readonly string DefaultMinimumColumnName = "Minimum";
    public static readonly string DefaultMaximumColumnName = "Maximum";

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
    /// <param name="selectedColumns">Selected data columns in the source table. If the argument is null, all columns will be used.</param>
    /// <param name="selectedRows">Selected rows in the source table. If the argument is null, all rows will be used.</param>
    /// <param name="destinationTable">The table where the statistical results are written to.</param>
    public static void DoStatisticsOnColumns(
      this DataColumnCollection srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      DataColumnCollection destinationTable
      )
    {
      bool bUseSelectedColumns = (null != selectedColumns && 0 != selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.ColumnCount;

      bool bUseSelectedRows = (null != selectedRows && 0 != selectedRows.Count);

      if (numcols == 0)
        return; // nothing selected

      // add a text column and some double columns
      // note: statistics is only possible for numeric columns since
      // otherwise in one column doubles and i.e. dates are mixed, which is not possible

      // 1st column is the name of the column of which the statistics is made
      var colCol = new Data.TextColumn();

      // 2nd column is the mean
      var colMean = new Data.DoubleColumn();

      // 3rd column is the standard deviation
      var colSd = new Data.DoubleColumn();

      // 4th column is the standard e (N)
      var colSe = new Data.DoubleColumn();

      // 5th column is the sum
      var colSum = new Data.DoubleColumn();

      var colSumSqr = new Data.DoubleColumn();

      // 6th column is the number of items for statistics
      var colN = new Data.DoubleColumn();

      var colFracOneSigma = new Data.DoubleColumn();
      var colFracTwoSigma = new Data.DoubleColumn();
      var colFracThreeSigma = new Data.DoubleColumn();

      var colMinimum = new DoubleColumn(); // Minimum of the values
      var colMaximum = new DoubleColumn(); // Maximum of the values

      int currRow = 0;
      for (int si = 0; si < numcols; si++)
      {
        Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
        if (!(col is Altaxo.Data.INumericColumn))
          continue;

        int rows = bUseSelectedRows ? selectedRows.Count : srctable.RowCount;
        if (rows == 0)
          continue;

        // now do the statistics
        var ncol = (Data.INumericColumn)col;
        double sum = 0;
        double sumsqr = 0;
        int NN = 0;
        double minimum = double.PositiveInfinity;
        double maximum = double.NegativeInfinity;

        for (int i = 0; i < rows; i++)
        {
          double val = bUseSelectedRows ? ncol[selectedRows[i]] : ncol[i];
          if (double.IsNaN(val))
            continue;

          NN++;
          sum += val;
          sumsqr += (val * val);
          minimum = Math.Min(minimum, val);
          maximum = Math.Max(maximum, val);
        }
        // now fill a new row in the worksheet

        double mean = sum / NN;
        double ymy0sqr = sumsqr - sum * sum / NN;
        if (ymy0sqr < 0)
          ymy0sqr = 0; // if this is lesser zero, it is a rounding error, so set it to zero
        double sd = NN > 1 ? Math.Sqrt(ymy0sqr / (NN - 1)) : 0;
        double se = sd / Math.Sqrt(NN);

        // calculate fractions
        double oneSigmaLo = mean - 1 * sd, oneSigmaHi = mean + 1 * sd;
        double twoSigmaLo = mean - 2 * sd, twoSigmaHi = mean + 2 * sd;
        double threeSigmaLo = mean - 3 * sd, threeSigmaHi = mean + 3 * sd;
        int cntOneSigma = 0, cntTwoSigma = 0, cntThreeSigma = 0;

        for (int i = 0; i < rows; i++)
        {
          double val = bUseSelectedRows ? ncol[selectedRows[i]] : ncol[i];
          if (double.IsNaN(val))
            continue;

          if (Altaxo.Calc.RMath.IsInIntervalCC(val, oneSigmaLo, oneSigmaHi))
            ++cntOneSigma;
          if (Altaxo.Calc.RMath.IsInIntervalCC(val, twoSigmaLo, twoSigmaHi))
            ++cntTwoSigma;
          if (Altaxo.Calc.RMath.IsInIntervalCC(val, threeSigmaLo, threeSigmaHi))
            ++cntThreeSigma;
        }

        if (0 == NN)
        {
          minimum = maximum = double.NaN;
        }

        colCol[currRow] = col.Name;
        colMean[currRow] = mean; // mean
        colSd[currRow] = sd;
        colSe[currRow] = se;
        colSum[currRow] = sum;
        colSumSqr[currRow] = sumsqr;
        colN[currRow] = NN;
        colFracOneSigma[currRow] = cntOneSigma / (double)NN;
        colFracTwoSigma[currRow] = cntTwoSigma / (double)NN;
        colFracThreeSigma[currRow] = cntThreeSigma / (double)NN;
        colMinimum[currRow] = minimum;
        colMaximum[currRow] = maximum;
        currRow++; // for the next column
      } // for all selected columns

      if (currRow != 0)
      {
        destinationTable.EnsureExistence(DefaultColumnNameColumnName, typeof(TextColumn), ColumnKind.X, 0).Append(colCol);
        AppendStatisticalData(destinationTable, colMean, colSd, colSe, colSum, colSumSqr, colN, colFracOneSigma, colFracTwoSigma, colFracThreeSigma, colMinimum, colMaximum);
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
      DoStatisticsOnRows(srctable.DataColumns, selectedColumns, selectedRows, table.DataColumns);
      return table;
    }

    /// <summary>
    /// Calculates statistics of selected columns. Creates a new table where the statistical data will be written to.
    /// </summary>
    /// <param name="srctable">Source table.</param>
    /// <param name="selectedColumns">Selected data columns in the source table.</param>
    /// <param name="selectedRows">Selected rows in the source table.</param>
    public static DataTable DoStatisticsOnRows(
      this DataColumnCollection srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows
      )
    {
      var table = CreateStatisticalTable();
      DoStatisticsOnRows(srctable, selectedColumns, selectedRows, table.DataColumns);
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
      bool bUseSelectedColumns = (null != selectedColumns && 0 != selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.ColumnCount;
      if (numcols == 0)
        return; // nothing selected

      bool bUseSelectedRows = (null != selectedRows && 0 != selectedRows.Count);
      int numrows = bUseSelectedRows ? selectedRows.Count : srctable.RowCount;
      if (numrows == 0)
        return;

      var cRows = new DoubleColumn();

      // 1st column is the mean, and holds the sum during the calculation
      var colMean = new Data.DoubleColumn();

      // 2rd column is the standard deviation, and holds the square sum during calculation
      var colSD = new Data.DoubleColumn();

      // 3th column is the standard e (N)
      var colSE = new Data.DoubleColumn();

      // 4th column is the sum
      var colSum = new Data.DoubleColumn();

      // 5th column is the number of items for statistics
      var colNN = new Data.DoubleColumn();

      var colSumSqr = new Data.DoubleColumn();
      var colFracOneSigma = new Data.DoubleColumn();
      var colFracTwoSigma = new Data.DoubleColumn();
      var colFracThreeSigma = new Data.DoubleColumn();
      var colMinimum = new DoubleColumn();
      var colMaximum = new DoubleColumn();

      // first fill the cols c1, c2, c5 with zeros because we want to sum up
      for (int i = 0; i < numrows; i++)
      {
        colSum[i] = 0;
        colSumSqr[i] = 0;
        colNN[i] = 0;
        colMinimum[i] = double.PositiveInfinity;
        colMaximum[i] = double.NegativeInfinity;
      }

      for (int si = 0; si < numcols; si++)
      {
        Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
        if (!(col is Altaxo.Data.INumericColumn))
          continue;

        // now do the statistics
        var ncol = (Data.INumericColumn)col;
        for (int i = 0; i < numrows; i++)
        {
          int row = bUseSelectedRows ? selectedRows[i] : i;
          cRows[i] = row;

          double val = ncol[row];
          if (double.IsNaN(val))
            continue;

          colSum[i] += val;
          colSumSqr[i] += val * val;
          colNN[i] += 1;
          colMinimum[i] = Math.Min(colMinimum[i], val);
          colMaximum[i] = Math.Max(colMaximum[i], val);
        }
      } // for all selected columns

      // now calculate the statistics
      for (int i = 0; i < numrows; i++)
      {
        // now fill a new row in the worksheet
        double NN = colNN[i];
        double sum = colSum[i];
        double sumsqr = colSumSqr[i];
        if (NN > 0)
        {
          double mean = sum / NN;
          double ymy0sqr = sumsqr - sum * sum / NN;
          if (ymy0sqr < 0)
            ymy0sqr = 0; // if this is lesser zero, it is a rounding error, so set it to zero
          double sd = NN > 1 ? Math.Sqrt(ymy0sqr / (NN - 1)) : 0;
          double se = sd / Math.Sqrt(NN);

          colMean[i] = mean; // mean
          colSD[i] = sd;
          colSE[i] = se;
        }
        else
        {
          colMinimum[i] = double.NaN;
          colMaximum[i] = double.NaN;
        }
      } // for all rows

      // calculate fractions

      for (int i = 0; i < numrows; i++)
      {
        int row = bUseSelectedRows ? selectedRows[i] : i;

        double mean = colMean[i];
        double sd = colSD[i];

        // calculate fractions
        double oneSigmaLo = mean - 1 * sd, oneSigmaHi = mean + 1 * sd;
        double twoSigmaLo = mean - 2 * sd, twoSigmaHi = mean + 2 * sd;
        double threeSigmaLo = mean - 3 * sd, threeSigmaHi = mean + 3 * sd;
        int cntOneSigma = 0, cntTwoSigma = 0, cntThreeSigma = 0;

        for (int si = 0; si < numcols; si++)
        {
          Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
          if (!(col is Altaxo.Data.INumericColumn))
            continue;

          // now do the statistics
          var ncol = (Data.INumericColumn)col;
          double val = ncol[row];
          if (double.IsNaN(val))
            continue;

          if (Altaxo.Calc.RMath.IsInIntervalCC(val, oneSigmaLo, oneSigmaHi))
            ++cntOneSigma;
          if (Altaxo.Calc.RMath.IsInIntervalCC(val, twoSigmaLo, twoSigmaHi))
            ++cntTwoSigma;
          if (Altaxo.Calc.RMath.IsInIntervalCC(val, threeSigmaLo, threeSigmaHi))
            ++cntThreeSigma;
        }

        colFracOneSigma[i] = cntOneSigma / colNN[i];
        colFracTwoSigma[i] = cntTwoSigma / colNN[i];
        colFracThreeSigma[i] = cntThreeSigma / colNN[i];
      }

      destinationTable.EnsureExistence(DefaultRowNumberColumnName, typeof(DoubleColumn), ColumnKind.X, 0).Append(cRows);
      AppendStatisticalData(destinationTable, colMean, colSD, colSE, colSum, colSumSqr, colNN, colFracOneSigma, colFracTwoSigma, colFracThreeSigma, colMinimum, colMaximum);
    }

    /// <summary>
    /// Creates a table for statistics on columns. Property columns are included in the statistical table.
    /// </summary>
    /// <param name="srcTable"></param>
    /// <param name="selectedColumns"></param>
    /// <returns></returns>
    private static DataTable CreateStatisticalTable(DataTable srcTable, IAscendingIntegerCollection selectedColumns)
    {
      var result = new DataTable
      {
        Name = Altaxo.Main.ProjectFolder.PrependToName(srcTable.Name, "Statistics of ")
      };

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
      var result = new DataTable();

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
      var result = new DataTable();

      result.DataColumns.Add(new DoubleColumn(), DefaultRowNumberColumnName, ColumnKind.X, 0);
      AddStatisticColumns(result);
      return result;
    }

    private static void AddStatisticColumns(DataTable result)
    {
      result.DataColumns.Add(new DoubleColumn(), DefaultMeanColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultStandardErrorColumnName, ColumnKind.Err, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultStandardDeviationColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultSumColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultSumSqrColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultNumberOfItemsColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultFractionInOneSigmaColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultFractionInTwoSigmaColumnName, ColumnKind.V, 0);
      result.DataColumns.Add(new DoubleColumn(), DefaultFractionInThreeSigmaColumnName, ColumnKind.V, 0);
    }

    private static void AppendStatisticalData(DataColumnCollection destinationTable, Data.DoubleColumn colMean, Data.DoubleColumn colSd, Data.DoubleColumn colSe, Data.DoubleColumn colSum, Data.DoubleColumn colSumSqr, Data.DoubleColumn colN, Data.DoubleColumn fracOneSigma, Data.DoubleColumn fracTwoSigma, Data.DoubleColumn fracThreeSigma, DoubleColumn minimum, DoubleColumn maximum)
    {
      destinationTable.EnsureExistence(DefaultMeanColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(colMean);
      destinationTable.EnsureExistence(DefaultStandardErrorColumnName, typeof(DoubleColumn), ColumnKind.Err, 0).Append(colSe);
      destinationTable.EnsureExistence(DefaultStandardDeviationColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(colSd);
      destinationTable.EnsureExistence(DefaultSumColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(colSum);
      destinationTable.EnsureExistence(DefaultSumSqrColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(colSumSqr);
      destinationTable.EnsureExistence(DefaultNumberOfItemsColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(colN);
      destinationTable.EnsureExistence(DefaultFractionInOneSigmaColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(fracOneSigma);
      destinationTable.EnsureExistence(DefaultFractionInTwoSigmaColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(fracTwoSigma);
      destinationTable.EnsureExistence(DefaultFractionInThreeSigmaColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(fracThreeSigma);
      destinationTable.EnsureExistence(DefaultMinimumColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(minimum);
      destinationTable.EnsureExistence(DefaultMaximumColumnName, typeof(DoubleColumn), ColumnKind.V, 0).Append(maximum);
    }

    private static void AddSourcePropertyColumns(DataTable srctable, IAscendingIntegerCollection selectedColumns, DataTable destinationTable)
    {
      bool bUseSelectedColumns = (null != selectedColumns && 0 != selectedColumns.Count);
      int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumnCount;

      // new : add a copy of all property columns; can be usefull
      for (int i = 0; i < srctable.PropertyColumnCount; i++)
      {
        DataColumn originalColumn = srctable.PropertyColumns[i];
        var clonedColumn = (DataColumn)originalColumn.Clone();
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
