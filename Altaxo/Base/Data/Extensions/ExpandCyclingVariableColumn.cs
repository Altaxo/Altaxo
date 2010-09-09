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
		Altaxo.Data.DataTable _sourceTable;
		IAscendingIntegerCollection _columnsToProcess;
		List<int> _columnsToAverageOverRepeatPeriod = new List<int>();

		public Altaxo.Data.DataTable SourceTable 
		{
			get
			{
				return _sourceTable;
			}
			set
			{
				_sourceTable = value;
			}
		}


    public enum DestinationXColumn
    {
      CyclingVariable,
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
    public IAscendingIntegerCollection ColumnsToProcess
		{
			get
			{
					return _columnsToProcess;
			}
			set
			{
				_columnsToProcess = value;
			}
		}

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

		/// <summary>If set, the destination columns will be sorted according to the first averaged column (if there is any).</summary>
		public OutputSorting DestinationColumnSorting { get; set; }

		/// <summary>If set, the destination rows will be sorted according to the destination x column.</summary>
		public OutputSorting DestinationRowSorting { get; set; }

		/// <summary>
		/// Tests if the column variables are contained into ColumnsToProcess and rectifies problems.
		/// </summary>
		/// <param name="throwIfNonCoherent">If true, an exception is thrown if any problems are detected.</param>
		public void EnsureCoherence(bool throwIfNonCoherent)
		{
			if (null == _sourceTable) // this is mandatory, thus an exception is always thrown
			{
				throw new ArgumentNullException("SourceTable is null, it must be set before");
			}

			if (null == _columnsToProcess || _columnsToProcess.Count == 0)
			{
				if (throwIfNonCoherent)
					throw new ArgumentException(_columnsToProcess == null ? "ColumnsToProcess is null" : "ColumnsToProcess is empty");
				else
					_columnsToProcess = new Altaxo.Collections.ContiguousNonNegativeIntegerRange(0, _sourceTable.DataColumnCount);
			}

			if (!_columnsToProcess.Contains(ColumnWithCyclingVariable) && _columnsToProcess.Count > 0)
			{
				if (throwIfNonCoherent)
					throw new ArgumentException("Column with cycling variable was not included in columnsToProcess");
				else
					ColumnWithCyclingVariable = _columnsToProcess[0];
			}

			for (int i = _columnsToAverageOverRepeatPeriod.Count - 1; i >= 0; i--)
			{
				if (!_columnsToProcess.Contains(_columnsToAverageOverRepeatPeriod[i]))
				{
					if (throwIfNonCoherent)
						throw new ArgumentException("ColumnsToAverage contains one or more columns that are not included in columnsToProcess");
					else
						_columnsToAverageOverRepeatPeriod.RemoveAt(i);
				}
			}
		}
  }

  public static class ExpandCyclingVariableColumnActions
  {
    public static void ShowExpandCyclingVariableColumnDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataColumns)
    {
      var options = new ExpandCyclingVariableColumnOptions();
			options.SourceTable = srcTable;
      options.ColumnsToProcess = selectedDataColumns;
      if (true == Current.Gui.ShowDialog(ref options, "Choose options", false))
      {
        var destTable = new DataTable();

        string error = null;
        try
        {
					error = ExpandCyclingVariableColumn(options, destTable);
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


		/// <summary>
		/// Expand the source columns according to the provided options. The source table and the settings are provided in the <see cref="options"/> variable.
		/// The provided destination table is cleared from all data and property values before.
		/// </summary>
		/// <param name="options">The options containing the source table and the settings for expanding.</param>
		/// <param name="destTable">The destination table. Any data will be removed before filling with the new data.</param>
		/// <returns>Null if the method finishes successfully, or an error information.</returns>
		public static string ExpandCyclingVariableColumn(ExpandCyclingVariableColumnOptions options, DataTable destTable)
    {
			try
			{
				options.EnsureCoherence(true);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

			DataTable srcTable = options.SourceTable;
      destTable.DataColumns.RemoveColumnsAll();
      destTable.PropCols.RemoveColumnsAll();

      var srcCycCol = srcTable.DataColumns[options.ColumnWithCyclingVariable];
      var repeatRanges = DecomposeIntoRepeatUnits(srcCycCol);
      var srcColumnsToProcess = options.ColumnsToProcess;

			// check if there is at least one averaged column
			if (options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.FirstAveragedColumn && options.ColumnsToAverageOverRepeatPeriod.Count == 0)
				throw new ArgumentException("In order to let the first averaged column being the x-column, a column to average is needed, but the options didn't provide such column!");

			// --- Create and calculate the averaged columns, for now only temporarily ---
			var propColsTemp = AverageColumns(options, repeatRanges);

			// --- avgValueOrder designates the ordering of the first averaged column and therefore of the sorting of the ranges and of the first averaged column
			int[] avgValueOrder = Sorting.CreateIdentityIndices(repeatRanges.Count);
			// --- prepare the sorting of columns by first averaged column ---
			var rangeOrderSorting = options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.CyclingVariable ? options.DestinationColumnSorting : options.DestinationRowSorting;
			if (propColsTemp.Length > 0 && rangeOrderSorting != ExpandCyclingVariableColumnOptions.OutputSorting.None)
			{
				avgValueOrder = Sorting.HeapSortVirtually(propColsTemp[0], avgValueOrder);
				if (rangeOrderSorting == ExpandCyclingVariableColumnOptions.OutputSorting.Descending)
					Sorting.ReverseArray(avgValueOrder);
			}

			// prepare the sorting of the cycling values
			var cycValueSorting = options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.CyclingVariable ? options.DestinationRowSorting : options.DestinationColumnSorting;
			// create a dictionary with the cycling values (unique) and the corresponding ordering index
			var cycValueOrder = GetUniqueValues(srcCycCol, cycValueSorting == ExpandCyclingVariableColumnOptions.OutputSorting.Descending);



      if (options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.CyclingVariable)
      {
				int[] propColsIdx = CreatePropColsForAveragedColumns(options, destTable);

				// --- Fill the x column, take the row sorting into account ---
        var destXCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(options.ColumnWithCyclingVariable), srcCycCol.GetType(), ColumnKind.X, srcTable.DataColumns.GetColumnGroup(options.ColumnWithCyclingVariable));
				foreach (var entry in cycValueOrder)
					destXCol[entry.Value] = entry.Key;

        if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupOneColumn)
        {
					// foreach sourceColumnToProcess create as many destination columns as there are cycling ranges available
          foreach(int nSrcCol in srcColumnsToProcess)
          {
            if (nSrcCol == options.ColumnWithCyclingVariable || options.ColumnsToAverageOverRepeatPeriod.Contains(nSrcCol))
              continue;
            var srcCol = srcTable.DataColumns[nSrcCol];
            int nCreatedCol = -1;
						var destColumnsToSort = new AscendingIntegerCollection();
            foreach (int rangeIndex in avgValueOrder)
            {
							var range = repeatRanges[rangeIndex];
              ++nCreatedCol;
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nCreatedCol.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
							destColumnsToSort.Add(nDestCol);
              foreach (var nSrcRow in range)
              {
                int nDestRow = cycValueOrder[srcCycCol[nSrcRow]];
                destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property columns
              for (int nPropCol = 0; nPropCol < propColsTemp.Length; nPropCol++)
              {
                destTable.PropCols[propColsIdx[nPropCol]][nDestCol] = propColsTemp[nPropCol][rangeIndex];
              }
            }
          } // repeat for each source colum to process
        }

        else if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupAllColumns)
        {
          int nCreatedCol = -1; // running number of processed range for column creation (Naming)
					foreach (int rangeIndex in avgValueOrder) 
          {
						var range = repeatRanges[rangeIndex];
            ++nCreatedCol;
            foreach(int nSrcCol in srcColumnsToProcess)
            {
              if (nSrcCol == options.ColumnWithCyclingVariable || options.ColumnsToAverageOverRepeatPeriod.Contains(nSrcCol))
                continue;

              var srcCol = srcTable.DataColumns[nSrcCol];
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nCreatedCol.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
              foreach (var nSrcRow in range)
              {
                int nDestRow = cycValueOrder[srcCycCol[nSrcRow]];
                destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property columns
              for (int nPropCol = 0; nPropCol < propColsTemp.Length; nPropCol++)
              {
                destTable.PropCols[propColsIdx[nPropCol]][nDestCol] = propColsTemp[nPropCol][rangeIndex];
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

        // now the first x column contains the values of the averaged column
        // the rest of the data columns is repeated as many times as there are members in each repeat range
        int nSrcXCol = options.ColumnsToAverageOverRepeatPeriod[0];
        var destXCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcXCol), srcTable[nSrcXCol].GetType(), ColumnKind.X, srcTable.DataColumns.GetColumnGroup(nSrcXCol));

				// Fill with destination X
        for(int nDestRow=0;nDestRow<repeatRanges.Count;nDestRow++)
          destXCol[nDestRow] = propColsTemp[0][avgValueOrder[nDestRow]];

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

						IEnumerable<AltaxoVariant> cycValues = cycValueOrder.Keys;
						if (options.DestinationColumnSorting == ExpandCyclingVariableColumnOptions.OutputSorting.Descending)
							cycValues = cycValueOrder.Keys.Reverse();

            foreach (var cycValue in cycValues)
            {
              ++nCurrNumber;
              var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(nSrcCol) + "." + nCurrNumber.ToString(), srcTable.DataColumns[nSrcCol].GetType(), srcTable.DataColumns.GetColumnKind(nSrcCol), srcTable.DataColumns.GetColumnGroup(nSrcCol));
              var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
              int nDestRow=-1;

							foreach (int rangeIndex in avgValueOrder)
							{
								var range = repeatRanges[rangeIndex];
                ++nDestRow;
                int nSrcRow = FindSrcXRow(srcCycCol, cycValue, range);
                if(nSrcRow>=0)
                  destCol[nDestRow] = srcCol[nSrcRow];
              }
              // fill also property column
              destPropCol[nDestCol] = cycValue;
            }
          }
        }
        else if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupAllColumns)
        {
					IEnumerable<AltaxoVariant> positionsKeys = cycValueOrder.Keys;
					if (options.DestinationColumnSorting == ExpandCyclingVariableColumnOptions.OutputSorting.Descending)
						positionsKeys = cycValueOrder.Keys.Reverse();

					int nCurrNumber = -1;
          foreach (var xVal in positionsKeys)
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
							foreach (int rangeIndex in avgValueOrder)
							{
								var range = repeatRanges[rangeIndex];
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
		/// <param name="options">Options containing the column numbers of the columns to average.</param>
		/// <param name="destTable">Destination table where to create the property columns.</param>
    /// <returns>Indices of the newly created property columns. The indices have the same order as the columns to average.</returns>
		private static int[] CreatePropColsForAveragedColumns(ExpandCyclingVariableColumnOptions options, DataTable destTable)
    {
			DataTable srcTable = options.SourceTable;
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
    /// <param name="options">Options containing the column numbers of the columns to average.</param>
    /// <param name="repeatRanges">Designates the start and the end of each repetition period.</param>
    /// <returns>Array of data columns which contain the averaged columns. Inside a column the row index designates the index of the range.</returns>
    private static DataColumn[] AverageColumns(ExpandCyclingVariableColumnOptions options, IList<ContiguousIntegerRange> repeatRanges)
    {
			DataTable srcTable = options.SourceTable;
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
		/// The sorting is done using the default comparison.
    /// </summary>
    /// <param name="src">Source column.</param>
    /// <returns>Dictionary which contains the unique values of the source column as keys and the sorting index as value.</returns>
    public static SortedDictionary<AltaxoVariant,int> GetUniqueValues(DataColumn src, bool sortDescending)
    {
      var set = new SortedDictionary<AltaxoVariant, int>();

      for (int i = 0; i < src.Count; i++)
      {
        if (set.Keys.Contains(src[i]))
          continue;
        else
          set.Add(src[i], i);
      }

			if (sortDescending)
			{
				int j = set.Count - 1;
				foreach (var key in set.Keys.ToArray()) // ToArray is neccessary in order to avoid an exception that the set is modified during enumeration
					set[key] = j--;
			}
			else
			{
				int j = 0;
				foreach (var key in set.Keys.ToArray()) // ToArray is neccessary in order to avoid an exception that the set is modified during enumeration
					set[key] = j++;
			}

      return set;
    }

    /// <summary>
    /// Decomposes a column into repeat units by analysing the values of the column with increasing index.
		/// If a column value is repeated, the current range is finalized and a new range is started. At the end,
		/// a list of index ranges is returned. Inside each range the column values are guaranteed to be unique.
    /// </summary>
    /// <param name="col">Column to decompose.</param>
    /// <returns>List of integer ranges. Inside a single range the column values are ensured to be unique.</returns>
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
