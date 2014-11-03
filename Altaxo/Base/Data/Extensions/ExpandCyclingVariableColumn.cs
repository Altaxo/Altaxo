#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Contains options how to split a table that contains an independent variable with cycling values into
	/// another table, where this independent variable is unique and sorted.
	/// </summary>
	public class ExpandCyclingVariableColumnOptions : ICloneable
	{
		#region Enums

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

		#endregion Enums

		#region Members

		/// <summary>Designates whether the destination x column is derived from the cycling variable column or from the (first) averaged column.</summary>
		public DestinationXColumn DestinationX { get; set; }

		/// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
		public OutputFormat DestinationOutput { get; set; }

		/// <summary>If set, the destination columns will be sorted according to the first averaged column (if there is any).</summary>
		public OutputSorting DestinationColumnSorting { get; set; }

		/// <summary>If set, the destination rows will be sorted according to the destination x column.</summary>
		public OutputSorting DestinationRowSorting { get; set; }

		#endregion Members

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2014-11-02 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExpandCyclingVariableColumnOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ExpandCyclingVariableColumnOptions)obj;

				info.AddEnum("DestinationX", s.DestinationX);
				info.AddEnum("DestinationOutput", s.DestinationOutput);
				info.AddEnum("DestinationColumnSorting", s.DestinationColumnSorting);
				info.AddEnum("DestinationRowSorting", s.DestinationRowSorting);
			}

			protected virtual ExpandCyclingVariableColumnOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new ExpandCyclingVariableColumnOptions() : (ExpandCyclingVariableColumnOptions)o);

				s.DestinationX = (DestinationXColumn)info.GetEnum("DestinationX", typeof(DestinationXColumn));
				s.DestinationOutput = (OutputFormat)info.GetEnum("DestinationOutput", typeof(OutputFormat));
				s.DestinationColumnSorting = (OutputSorting)info.GetEnum("DestinationColumnSorting", typeof(OutputSorting));
				s.DestinationRowSorting = (OutputSorting)info.GetEnum("DestinationRowSorting", typeof(OutputSorting));

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		#region Construction

		public ExpandCyclingVariableColumnOptions()
		{
		}

		public ExpandCyclingVariableColumnOptions(ExpandCyclingVariableColumnOptions from)
		{
			CopyFrom(from);
		}

		public object Clone()
		{
			return new ExpandCyclingVariableColumnOptions(this);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as ExpandCyclingVariableColumnOptions;
			if (null != from)
			{
				this.DestinationX = from.DestinationX;
				this.DestinationOutput = from.DestinationOutput;
				this.DestinationRowSorting = from.DestinationRowSorting;
				this.DestinationColumnSorting = from.DestinationColumnSorting;

				return true;
			}
			return false;
		}

		#endregion Construction
	}

	/// <summary>
	/// Holds both the data (see <see cref="DataTableMultipleColumnProxy"/>) and the options (see <see cref="ExpandCyclingVariableColumnOptions"/>) to perform
	/// the expanding of a table containing a column with a cycling variable.
	/// </summary>
	public class ExpandCyclingVariableColumnDataAndOptions : ICloneable
	{
		public ExpandCyclingVariableColumnDataAndOptions(DataTableMultipleColumnProxy data, ExpandCyclingVariableColumnOptions options)
		{
			Data = data;
			Options = options;
		}

		/// <summary>
		/// Holds the data nessessary for expanding of a table containing a column with a cycling variable.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public DataTableMultipleColumnProxy Data { get; private set; }

		/// <summary>
		/// Holds the options nessessary for expanding of a table containing a column with a cycling variable.
		/// </summary>
		/// <value>
		/// The options.
		/// </value>
		public ExpandCyclingVariableColumnOptions Options { get; private set; }

		/// <summary>Identifies the column with the cycling variable in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
		public const string ColumnWithCyclingVariableIdentifier = "ColumnWithCyclingVariable";

		/// <summary>Identifies the column(s) to average in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
		public const string ColumnsToAverageIdentifier = "ColumnsToAverage";

		/// <summary>Identifies all columns which participate in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
		public const string ColumnsParticipatingIdentifier = "ColumnsParticipating";

		/// <summary>
		/// Tests if the data in <paramref name="data"/> can be used for the ExpandCyclingVariable action.
		/// </summary>
		/// <param name="data">The data to test.</param>
		/// <param name="throwIfNonCoherent">If true, an exception is thrown if any problems are detected. If false, it is tried to rectify the problem by making some assumtions.</param>
		public static void EnsureCoherence(DataTableMultipleColumnProxy data, bool throwIfNonCoherent)
		{
			if (null == data.DataTable) // this is mandatory, thus an exception is always thrown
			{
				throw new ArgumentNullException("SourceTable is null, it must be set before");
			}

			data.EnsureExistenceOfIdentifier(ColumnsParticipatingIdentifier);
			data.EnsureExistenceOfIdentifier(ColumnWithCyclingVariableIdentifier, 1);
			data.EnsureExistenceOfIdentifier(ColumnsToAverageIdentifier);

			if (data.GetDataColumns(ColumnsParticipatingIdentifier).Count == 0)
			{
				if (throwIfNonCoherent)
					throw new ArgumentException(!data.ContainsIdentifier(ColumnsParticipatingIdentifier) ? "ColumnsToProcess is not set" : "ColumnsToProcess is empty");
			}

			if (data.GetDataColumnOrNull(ColumnWithCyclingVariableIdentifier) == null)
			{
				if (throwIfNonCoherent)
					throw new ArgumentException("Column with cycling variable was not included in columnsToProcess");
				else
				{
					var col = data.GetDataColumns(ColumnsParticipatingIdentifier).FirstOrDefault();
					if (null != col)
						data.SetDataColumn(ColumnWithCyclingVariableIdentifier, col);
				}
			}

			if (!data.ContainsIdentifier(ColumnsToAverageIdentifier))
			{
				if (throwIfNonCoherent)
					throw new ArgumentException("ColumnsToAverage collection is not included");
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new ExpandCyclingVariableColumnDataAndOptions((DataTableMultipleColumnProxy)this.Data.Clone(), (ExpandCyclingVariableColumnOptions)this.Options.Clone());
		}
	}

	public static class ExpandCyclingVariableColumnActions
	{
		public static void ShowExpandCyclingVariableColumnDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
		{
			DataTableMultipleColumnProxy proxy = null;
			ExpandCyclingVariableColumnOptions options = null;

			try
			{
				proxy = new DataTableMultipleColumnProxy(ExpandCyclingVariableColumnDataAndOptions.ColumnsParticipatingIdentifier, srcTable, selectedDataRows, selectedDataColumns);
				proxy.EnsureExistenceOfIdentifier(ExpandCyclingVariableColumnDataAndOptions.ColumnWithCyclingVariableIdentifier, 1);
				proxy.EnsureExistenceOfIdentifier(ExpandCyclingVariableColumnDataAndOptions.ColumnsToAverageIdentifier);

				options = new ExpandCyclingVariableColumnOptions();
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error in preparation of 'Expand Cycling Variable'");
				return;
			}

			var dataAndOptions = new ExpandCyclingVariableColumnDataAndOptions(proxy, options);

			// in order to show the column names etc in the dialog, it is neccessary to set the source
			if (true == Current.Gui.ShowDialog(ref dataAndOptions, "Choose options", false))
			{
				var destTable = new DataTable();
				proxy = dataAndOptions.Data;
				options = dataAndOptions.Options;

				string error = null;
				try
				{
					error = ExpandCyclingVariableColumn(dataAndOptions.Data, dataAndOptions.Options, destTable);
				}
				catch (Exception ex)
				{
					error = ex.ToString();
				}
				if (null != error)
					Current.Gui.ErrorMessageBox(error);

				destTable.Name = srcTable.Name + "_Expanded";

				// Create a DataSource
				ExpandCyclingVariableColumnDataSource dataSource = new ExpandCyclingVariableColumnDataSource(proxy, options, new Altaxo.Data.DataSourceImportOptions());
				destTable.DataSource = dataSource;

				Current.Project.DataTableCollection.Add(destTable);
				Current.ProjectService.ShowDocumentView(destTable);
			}
		}

		/// <summary>
		/// Expand the source columns according to the provided options. The source table and the settings are provided in the <paramref name="options"/> variable.
		/// The provided destination table is cleared from all data and property values before.
		/// </summary>
		/// <param name="inputData">The data containing the source table, the participating columns and the column with the cycling variable.</param>
		/// <param name="options">The settings for expanding.</param>
		/// <param name="destTable">The destination table. Any data will be removed before filling with the new data.</param>
		/// <returns>Null if the method finishes successfully, or an error information.</returns>
		public static string ExpandCyclingVariableColumn(DataTableMultipleColumnProxy inputData, ExpandCyclingVariableColumnOptions options, DataTable destTable)
		{
			var srcTable = inputData.DataTable;

			try
			{
				ExpandCyclingVariableColumnDataAndOptions.EnsureCoherence(inputData, true);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

			destTable.DataColumns.RemoveColumnsAll();
			destTable.PropCols.RemoveColumnsAll();

			DataColumn srcCycCol = inputData.GetDataColumnOrNull(ExpandCyclingVariableColumnDataAndOptions.ColumnWithCyclingVariableIdentifier);
			var repeatRanges = DecomposeIntoRepeatUnits(srcCycCol);

			// check if there is at least one averaged column
			var columnsToAverageOverRepeatPeriod = inputData.GetDataColumns(ExpandCyclingVariableColumnDataAndOptions.ColumnsToAverageIdentifier);
			if (options.DestinationX == ExpandCyclingVariableColumnOptions.DestinationXColumn.FirstAveragedColumn && columnsToAverageOverRepeatPeriod.Count == 0)
				throw new ArgumentException("In order to let the first averaged column being the x-column, a column to average is needed, but the options didn't provide such column!");

			// get the other columns to process

			var srcColumnsToProcess = new List<DataColumn>(inputData.GetDataColumns(ExpandCyclingVariableColumnDataAndOptions.ColumnsParticipatingIdentifier));
			// subtract cyclic variable column and average columns
			srcColumnsToProcess.Remove(srcCycCol);
			foreach (var col in columnsToAverageOverRepeatPeriod)
				srcColumnsToProcess.Remove(col);

			// --- Create and calculate the averaged columns, for now only temporarily ---
			var propColsTemp = AverageColumns(srcTable, columnsToAverageOverRepeatPeriod, options, repeatRanges);

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
				int[] propColsIdx = CreatePropColsForAveragedColumns(srcTable, columnsToAverageOverRepeatPeriod, options, destTable);

				// --- Fill the x column, take the row sorting into account ---
				var destXCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCycCol), srcCycCol.GetType(), ColumnKind.X, srcTable.DataColumns.GetColumnGroup(srcCycCol));
				foreach (var entry in cycValueOrder)
					destXCol[entry.Value] = entry.Key;

				if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupOneColumn)
				{
					// foreach sourceColumnToProcess create as many destination columns as there are cycling ranges available
					foreach (var srcCol in srcColumnsToProcess)
					{
						int nCreatedCol = -1;
						var destColumnsToSort = new AscendingIntegerCollection();
						foreach (int rangeIndex in avgValueOrder)
						{
							var range = repeatRanges[rangeIndex];
							++nCreatedCol;
							var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCreatedCol.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), srcTable.DataColumns.GetColumnGroup(srcCol));
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
						foreach (var srcCol in srcColumnsToProcess)
						{
							var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCreatedCol.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), srcTable.DataColumns.GetColumnGroup(srcCol));
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
				DataColumn srcXCol = columnsToAverageOverRepeatPeriod[0];
				var destXCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcXCol), srcXCol.GetType(), ColumnKind.X, srcTable.DataColumns.GetColumnGroup(srcXCol));

				// Fill with destination X
				for (int nDestRow = 0; nDestRow < repeatRanges.Count; nDestRow++)
					destXCol[nDestRow] = propColsTemp[0][avgValueOrder[nDestRow]];

				// the only property column that is now usefull is that with the repeated values
				var destPropCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCycCol), srcCycCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCycCol), srcTable.DataColumns.GetColumnGroup(srcCycCol));

				if (options.DestinationOutput == ExpandCyclingVariableColumnOptions.OutputFormat.GroupOneColumn)
				{
					foreach (var srcCol in srcColumnsToProcess)
					{
						int nCurrNumber = -1;

						IEnumerable<AltaxoVariant> cycValues = cycValueOrder.Keys;
						if (options.DestinationColumnSorting == ExpandCyclingVariableColumnOptions.OutputSorting.Descending)
							cycValues = cycValueOrder.Keys.Reverse();

						foreach (var cycValue in cycValues)
						{
							++nCurrNumber;
							var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCurrNumber.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), srcTable.DataColumns.GetColumnGroup(srcCol));
							var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
							int nDestRow = -1;

							foreach (int rangeIndex in avgValueOrder)
							{
								var range = repeatRanges[rangeIndex];
								++nDestRow;
								int nSrcRow = FindSrcXRow(srcCycCol, cycValue, range);
								if (nSrcRow >= 0)
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
						foreach (var srcCol in srcColumnsToProcess)
						{
							var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCurrNumber.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), srcTable.DataColumns.GetColumnGroup(srcCol));
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
		/// <param name="srcTable">Source table of the ExpandCyclingVariableColumn action</param>
		/// <param name="columnsToAverageOverRepeatPeriod">The columns for which the data should be averaged over one repeat period.</param>
		/// <param name="options">Options containing the column numbers of the columns to average.</param>
		/// <param name="destTable">Destination table where to create the property columns.</param>
		/// <returns>Indices of the newly created property columns. The indices have the same order as the columns to average.</returns>
		private static int[] CreatePropColsForAveragedColumns(DataTable srcTable, IEnumerable<DataColumn> columnsToAverageOverRepeatPeriod, ExpandCyclingVariableColumnOptions options, DataTable destTable)
		{
			var propColsIdx = new int[columnsToAverageOverRepeatPeriod.Count()];
			int nDestCol = -1;
			foreach (var srcCol in columnsToAverageOverRepeatPeriod)
			{
				++nDestCol;
				var destCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), srcTable.DataColumns.GetColumnGroup(srcCol));
				propColsIdx[nDestCol] = destTable.PropCols.GetColumnNumber(destCol);
			}
			return propColsIdx;
		}

		/// <summary>
		/// Average the columns to average for each repetition period.
		/// </summary>
		/// <param name="srcTable">Source table of the ExpandCyclingVariableColumn action</param>
		/// <param name="columnsToAverageOverRepeatPeriod">The columns for which the data should be averaged over one repeat period.</param>
		/// <param name="options">Options containing the column numbers of the columns to average.</param>
		/// <param name="repeatRanges">Designates the start and the end of each repetition period.</param>
		/// <returns>Array of data columns which contain the averaged columns. Inside a column the row index designates the index of the range.</returns>
		private static DataColumn[] AverageColumns(DataTable srcTable, IEnumerable<DataColumn> columnsToAverageOverRepeatPeriod, ExpandCyclingVariableColumnOptions options, IList<ContiguousIntegerRange> repeatRanges)
		{
			// make the averaged property columns
			var propColsTemp = new DataColumn[columnsToAverageOverRepeatPeriod.Count()];
			int nDestCol = -1;
			foreach (var srcCol in columnsToAverageOverRepeatPeriod)
			{
				nDestCol++;
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
		/// <param name="sortDescending">If <c>true</c>, the sorting is done in descending order (instead of ascending order).</param>
		/// <returns>Dictionary which contains the unique values of the source column as keys and the sorting index as value.</returns>
		public static SortedDictionary<AltaxoVariant, int> GetUniqueValues(DataColumn src, bool sortDescending)
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
					result.Add(ContiguousIntegerRange.FromStartAndCount(currentRangeStart, currentRangeCount));
					currentRangeStart = i;
					currentRangeCount = 0;
				}

				alreadyIn.Add(col[i]);
				currentRangeCount++;
			}

			if (currentRangeCount > 0)
			{
				result.Add(ContiguousIntegerRange.FromStartAndCount(currentRangeStart, currentRangeCount));
			}

			return result;
		}
	}
}