using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Data
{
  /// <summary>
  /// Contains options how to split a table that contains an independent variable with cycling values into 
  /// another table, where this independent variable is unique and sorted.
  /// </summary>
  public class ExpandCyclingVariableColumnOptions
  {
    List<int> _columnsToAverageOverRepeatPeriod = new List<int>();

    public enum DestinationXColumn
    {
      RepeatedColumn,
      FirstAveragedColumn
    }
    public enum OutputFormat
    {
      GroupOneColumn,
      GroupAllColumns,
    }

		public enum OutputSorting
		{
			None,
			Ascending,
			Descending
		}
    /// <summary>Indices of all columns that will be considered to be processed. If null, all columns of the source table will be considered.</summary>
    public IAscendingIntegerCollection ColumnsToProcess { get; set; }

    /// <summary>Index of the column, that contain a cycling independent variable.</summary>
    public int ColumnWithCyclingVariable { get; set; }

    /// <summary>
    /// Indices of the columns that contain nearly constant values for each cycling period.
    /// </summary>
    public List<int> ColumnsToAverageOverRepeatPeriod { get { return _columnsToAverageOverRepeatPeriod; } }

    /// <summary>Designates whether the destination x column is derived from the cycling variable column or from the (first) averaged column.</summary>
    public DestinationXColumn DestinationX { get; set; }

    /// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
    public OutputFormat DestinationOutput { get; set; }

		public OutputSorting DestinationColumnSorting { get; set; }

    //public bool DestinationPutIndependentVariablesAsPropertyColumns { get; set; }
  }

  public static class ExpandCyclingVariableColumnActions
  {
    public static void ShowExpandCyclingVariableColumnDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataColumns)
    {
      var options = new ExpandCyclingVariableColumnOptions();

      options.ColumnsToProcess = selectedDataColumns;
      if (true == Current.Gui.ShowDialog(ref options, "Choose options", false))
      {
        var destTable = new DataTable();

        string error = null;
        try
        {
          error = ExpandCyclingVariableColumn(srcTable, destTable, options);
        }
        catch (Exception ex)
        {
          error = ex.ToString();
        }
        if (null != error)
          Current.Gui.ErrorMessageBox(error);

        destTable.Name = srcTable.Name + "_Expanded";
        Current.Project.DataTableCollection.Add(destTable);
        Current.ProjectService.ShowDocumentView(destTable);
      }
    }



    public static string ExpandCyclingVariableColumn(DataTable srcTable, DataTable destTable, ExpandCyclingVariableColumnOptions options)
    {
      destTable.DataColumns.RemoveColumnsAll();
      destTable.PropCols.RemoveColumnsAll();

      var srcCycCol = srcTable.DataColumns[options.ColumnWithCyclingVariable];
      var positions = GetUniqueValues(srcCycCol);
      var repeatRanges = DecomposeIntoRepeatUnits(srcCycCol);
      var srcColumnsToProcess = options.ColumnsToProcess;
      if (null == srcColumnsToProcess || srcColumnsToProcess.Count==0)
        srcColumnsToProcess = new ContiguousIntegerRange(0, srcTable.DataColumnCount);


      if (options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.RepeatedColumn)
      {
        var propColsTemp = AverageColumns(srcTable, options, repeatRanges);
        int[] propColsIdx = CreatePropColsForAveragedColumns(srcTable, destTable, options);

        var destXCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(options.ColumnWithCyclingVariable), srcCycCol.GetType(), ColumnKind.X, srcTable.DataColumns.GetColumnGroup(options.ColumnWithCyclingVariable));
        int i=0;
        foreach (var entry in positions)
          destXCol[i++] = entry.Key;

        if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupOneColumn)
        {
          foreach(int nSrcCol in srcColumnsToProcess)
          {
            if (nSrcCol == options.ColumnWithCyclingVariable || options.ColumnsToAverageOverRepeatPeriod.Contains(nSrcCol))
              continue;
            var srcCol = srcTable.DataColumns[nSrcCol];
            int nRange = -1;
            foreach (var range in repeatRanges)
            {
              ++nRange;
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nRange.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
              foreach (var nSrcRow in range)
              {
                int nDestRow = positions[srcCycCol[nSrcRow]];
                destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property columns
              for (int nPropCol = 0; nPropCol < propColsTemp.Length; nPropCol++)
              {
                destTable.PropCols[propColsIdx[nPropCol]][nDestCol] = propColsTemp[nPropCol][nRange];
              }
            }
          }
        }
        else if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupAllColumns)
        {
          int nRange = -1;
          foreach (var range in repeatRanges)
          {
            ++nRange;
            foreach(int nSrcCol in srcColumnsToProcess)
            {
              if (nSrcCol == options.ColumnWithCyclingVariable || options.ColumnsToAverageOverRepeatPeriod.Contains(nSrcCol))
                continue;

              var srcCol = srcTable.DataColumns[nSrcCol];
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nRange.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
              foreach (var nSrcRow in range)
              {
                int nDestRow = positions[srcCycCol[nSrcRow]];
                destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property columns
              for (int nPropCol = 0; nPropCol < propColsTemp.Length; nPropCol++)
              {
                destTable.PropCols[propColsIdx[nPropCol]][nDestCol] = propColsTemp[nPropCol][nRange];
              }
            }
          }
        }
        else
        {
          throw new NotImplementedException("The option for destination output is unknown: " + options.DestinationOutput.ToString());
        }
      }
      else if (options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.FirstAveragedColumn)
      {
        // check if there is at least one averaged column
        if (options.ColumnsToAverageOverRepeatPeriod.Count == 0)
          throw new ArgumentException("In order to let the first averaged column being the x-column, a column to average is needed, but the options didn't provide such column!");

        var propColsTemp = AverageColumns(srcTable, options, repeatRanges);

        // now the first x column contains the values of the averaged column
        // the rest of the data columns is repeated as many times as there are members in each repeat range
        int nSrcXCol = options.ColumnsToAverageOverRepeatPeriod[0];
        var destXCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcXCol), srcTable[nSrcXCol].GetType(), ColumnKind.X, srcTable.DataColumns.GetColumnGroup(nSrcXCol));
        for(int nDestRow=0;nDestRow<repeatRanges.Count;nDestRow++)
          destXCol[nDestRow] = propColsTemp[0][nDestRow];

        // the only property column that is now usefull is that with the repeated values
        int nCycCol = options.ColumnWithCyclingVariable;
        var destPropCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(nCycCol), srcTable[nCycCol].GetType(), srcTable.DataColumns.GetColumnKind(nCycCol), srcTable.DataColumns.GetColumnGroup(nCycCol));

        if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupOneColumn)
        {
          foreach(int nSrcCol in srcColumnsToProcess)
          {
            if (nSrcCol == options.ColumnWithCyclingVariable || options.ColumnsToAverageOverRepeatPeriod.Contains(nSrcCol))
              continue;
            var srcCol = srcTable.DataColumns[nSrcCol];
            int nCurrNumber=-1;
            foreach (var xVal in positions.Keys)
            {
              ++nCurrNumber;
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nCurrNumber.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
              int nDestRow=-1;
              foreach(var range in repeatRanges)
              {
                ++nDestRow;
                int nSrcRow = FindSrcXRow(srcCycCol, xVal, range);
                if(nSrcRow>=0)
                  destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property column
              destPropCol[nDestCol] = xVal;
            }
          }
        }
        else if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupAllColumns)
        {
          int nCurrNumber = -1;
          foreach (var xVal in positions.Keys)
          {
            ++nCurrNumber;
            foreach(int nSrcCol in srcColumnsToProcess)
            {
              if (nSrcCol == options.ColumnWithCyclingVariable || options.ColumnsToAverageOverRepeatPeriod.Contains(nSrcCol))
                continue;

              var srcCol = srcTable.DataColumns[nSrcCol];
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nCurrNumber.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
              int nDestRow = -1;
              foreach (var range in repeatRanges)
              {
                ++nDestRow;
                int nSrcRow = FindSrcXRow(srcCycCol, xVal, range);
                if (nSrcRow >= 0)
                  destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property column
              destPropCol[nDestCol] = xVal;
            }
          }
        }
        else
        {
          throw new NotImplementedException("The option for destination output is unknown: " + options.DestinationOutput.ToString());
        }
      }

      return null;
    }

    /// <summary>
    /// Finds the source row for a given value inside a given row range.
    /// </summary>
    /// <param name="srcXCol">Column where the value must be found.</param>
    /// <param name="xVal">Value to find.</param>
    /// <param name="range">Range of rows.</param>
    /// <returns>The row for which the element is equal to the value, or -1 if the value could not be found.</returns>
    private static int FindSrcXRow(DataColumn srcXCol, AltaxoVariant xVal, ContiguousIntegerRange range)
    {
      // Find the src row
      foreach (int idx in range)
        if (srcXCol[idx] == xVal)
          return idx;
      return -1;
    }

    /// <summary>
    /// Creates a property column for each averaged column of the source table.
    /// </summary>
    /// <param name="srcTable">Source table which contains the columns to average.</param>
    /// <param name="destTable">Destination table where to create the property columns.</param>
    /// <param name="options">Options containing the column numbers of the columns to average.</param>
    /// <returns>Indices of the newly created property columns. The indices have the same order as the columns to average.</returns>
    private static int[] CreatePropColsForAveragedColumns(DataTable srcTable, DataTable destTable, ExpandCyclingVariableColumnOptions options)
    {
      var propColsIdx = new int[options.ColumnsToAverageOverRepeatPeriod.Count];
      int nDestCol = -1;
      foreach (var nSrcCol in options.ColumnsToAverageOverRepeatPeriod)
      {
        ++nDestCol;
        var srcCol = srcTable.DataColumns[nSrcCol];
        var destCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
        propColsIdx[nDestCol] = destTable.PropCols.GetColumnNumber(destCol);
      }
      return propColsIdx;
    }

    /// <summary>
    /// Average the columns to average for each repetition period.
    /// </summary>
    /// <param name="srcTable">Source table which contains the columns to average.</param>
    /// <param name="options">Options containing the column numbers of the columns to average.</param>
    /// <param name="repeatRanges">Designates the start and the end of each repetition period.</param>
    /// <returns>Array of data columns which contain the averaged columns. Inside a column the row index designates the index of the range.</returns>
    private static DataColumn[] AverageColumns(DataTable srcTable, ExpandCyclingVariableColumnOptions options, IList<ContiguousIntegerRange> repeatRanges)
    {
      // make the averaged property columns
      var propColsTemp = new DataColumn[options.ColumnsToAverageOverRepeatPeriod.Count];
      int nDestCol = -1;
      foreach (var nSrcCol in options.ColumnsToAverageOverRepeatPeriod)
      {
        nDestCol++;
        var srcCol = srcTable.DataColumns[nSrcCol];
        var destCol = (DataColumn)srcCol.Clone();
        destCol.Clear();

        var statistic = new Altaxo.Calc.Regression.QuickStatistics();
        int nDestRow = -1;
        foreach (var range in repeatRanges)
        {
          ++nDestRow;
          foreach (var idx in range)
            statistic.Add(srcCol[idx] - srcCol[range.Start]);
          destCol[nDestRow] = statistic.Mean + srcCol[range.Start]; // Trick: we store the averaged values temporarily in index 0 of the property column
        }
        propColsTemp[nDestCol] = destCol;
      }
      return propColsTemp;
    }



    /// <summary>
    /// Gets a dictionary which contains all unique values of a source column as keys. The value is the index according to the sorting.
    /// </summary>
    /// <param name="src">Source column.</param>
    /// <returns>Dictionary which contains the unique values of the source column as keys and the sorting index as value.</returns>
    public static SortedDictionary<AltaxoVariant,int> GetUniqueValues(DataColumn src)
    {
      var set = new SortedDictionary<AltaxoVariant, int>();

      for (int i = 0; i < src.Count; i++)
      {
        if (set.Keys.Contains(src[i]))
          continue;
        else
          set.Add(src[i], i);
      }


      int j=0;
      foreach (var key in set.Keys.ToArray())
      {
        set[key] = j;
        j++;
      }

      return set;
    }

    /// <summary>
    /// Decomposes a column into repeat units by analysing the values of the column.
    /// </summary>
    /// <param name="col">Column to decompose.</param>
    /// <returns>List of integer ranges. Inside a single range the values of the column ensured to be unique.</returns>
    public static IList<ContiguousIntegerRange> DecomposeIntoRepeatUnits(DataColumn col)
    {
      var result = new List<ContiguousIntegerRange>();
      var alreadyIn = new HashSet<AltaxoVariant>();

      var currentRangeStart = 0;
      var currentRangeCount = 0;
      for (int i = 0; i < col.Count; i++)
      {
        if (alreadyIn.Contains(col[i]))
        {
          alreadyIn.Clear();
          result.Add(new ContiguousIntegerRange(currentRangeStart, currentRangeCount));
          currentRangeStart = i;
          currentRangeCount = 0;
        }

        alreadyIn.Add(col[i]);
        currentRangeCount++;
      }

      if (currentRangeCount > 0)
      {
        result.Add(new ContiguousIntegerRange(currentRangeStart, currentRangeCount));
      }

      return result;
    }
  }
}
