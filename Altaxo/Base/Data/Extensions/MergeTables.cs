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
	/// Actions that help to merge two tables.
	/// </summary>
	public static class MergeTables
	{
		/// <summary>
		/// Merges two tables by corresponding x-columns.
		/// </summary>
		/// <param name="masterTable">Master table. Values from the slave table will be recalculated to fit the x-values of the master table.</param>
		/// <param name="masterXColumn">The master x-column of the master table.</param>
		/// <param name="slaveTable">The table providing the data for merging into the master table.</param>
		/// <param name="slaveXColumn">The x column of the slave table.</param>
		/// <param name="columnsToMerge">Indices of that columns of the slave table that should be merged into the master table.</param>
		/// <param name="createNewTable">If true, a new table is created as a clone of the master table. The data from the slave table are then merged into that clone. If false,
		/// the data are directly merged into the master table.</param>
		/// <returns>If <c>createNewTable</c> is true, then the newly created table. If false, then the provided master table where the data are merge to.</returns>
		public static DataTable MergeTable(
			this DataTable masterTable, DataColumn masterXColumn,
			DataTable slaveTable, DataColumn slaveXColumn,
			Altaxo.Collections.IAscendingIntegerCollection columnsToMerge,
			bool createNewTable)
		{
			DataTable destinationTable;
			if (createNewTable)
				destinationTable = (DataTable)masterTable.Clone();
			else
				destinationTable = masterTable;

			// create a fractional index column with the same length than the master table
			// that points into the slave table

			DoubleColumn fractIndex = GetFractionalIndex(masterXColumn, slaveXColumn);

			MergeTable(masterTable, fractIndex, slaveTable, columnsToMerge);

			return destinationTable;
		}

		/// <summary>
		/// Merges two tables by fractional index.
		/// </summary>
		/// <param name="destinationTable">Table to merge into.</param>
		/// <param name="fractionalIndex">Array of fractional indices. Each item points into the slaveTable to the value that should be included in the master column at the item's index.</param>
		/// <param name="slaveTable">The table providing the data for merging into the master table.</param>
		/// <param name="slaveColumnsToMerge">Indices of that columns of the slave table that should be merged into the master table.</param>
		public static void MergeTable(
			this DataTable destinationTable,
			DoubleColumn fractionalIndex,
			DataTable slaveTable,
			IAscendingIntegerCollection slaveColumnsToMerge)
		{
			int destinationTableStartIdx = destinationTable.DataColumnCount;
			destinationTable.AddDataColumnsWithPropertiesFrom(slaveTable, slaveColumnsToMerge);

			for (int i = 0; i < slaveColumnsToMerge.Count; i++)
			{
				int slaveColIdx = slaveColumnsToMerge[i];
				DataColumn newCol = destinationTable[destinationTableStartIdx + i];

				SetColumnFromFractionalIndex(newCol, fractionalIndex, slaveTable[slaveColIdx]);
			}
		}

		/// <summary>
		/// Fills destination column with values from an original column by help of a fractional index.
		/// </summary>
		/// <param name="destinationColumn">Column to fill. The old data in this column will be cleared before.</param>
		/// <param name="fractionalIndex">Array of fractional indices. Each item points into the originalColumn to that value that should be included in the master column at the item's index.</param>
		/// <param name="originalColumn">Column with the source data.</param>
		public static void SetColumnFromFractionalIndex(
			this DataColumn destinationColumn,
			DoubleColumn fractionalIndex,
			DataColumn originalColumn)
		{
			destinationColumn.Clear();

			for (int i = 0; i < fractionalIndex.Count; i++)
			{
				double fracIdx = fractionalIndex[i];
				int idxBase = (int)Math.Floor(fracIdx);
				double rel = fracIdx - idxBase;

				if (0 == rel)
				{
					destinationColumn[i] = originalColumn[idxBase];
				}
				else
				{
					try
					{
						var firstValue = originalColumn[idxBase];
						var secondValue = originalColumn[idxBase + 1];
						destinationColumn[i] = firstValue + rel * (secondValue - firstValue);
					}
					catch (Exception)
					{
						destinationColumn[i] = originalColumn[idxBase];
					}
				}
			}
		}

		/// <summary>
		/// Gets the fractional index for merging of two tables.
		/// </summary>
		/// <param name="masterColumn">X-column of the master table.</param>
		/// <param name="slaveColumn">X-column of the slave table.</param>
		/// <returns>Array of fractional indices. Each item points into the slaveTable to the value that should be included in the master column at the item's index.</returns>
		public static DoubleColumn GetFractionalIndex(INumericColumn masterColumn, INumericColumn slaveColumn)
		{
			if (!(masterColumn.Count.HasValue))
				throw new ArgumentException("masterColumn has no defined count");
			if (!(slaveColumn.Count.HasValue))
				throw new ArgumentException("slaveColumn has no defined count");

			int masterCount = masterColumn.Count.Value;
			int slaveCount = slaveColumn.Count.Value;

			var result = new DoubleColumn();
			var dict = new SortedDictionary<double, int>();
			for (int i = slaveCount - 1; i >= 0; i--)
				dict[slaveColumn[i]] = i;

			var sortedSlaveValues = dict.Keys.ToArray();
			var sortedSlaveIndices = dict.Values.ToArray();

			for (int masterIdx = 0; masterIdx < masterCount; masterIdx++)
			{
				var masterValue = masterColumn[masterIdx];
				if (double.IsNaN(masterValue))
				{
					result[masterIdx] = double.NaN;
					continue;
				}
				int dictIdx = Array.BinarySearch(sortedSlaveValues, masterValue);

				if (dictIdx >= 0)
				{
					result[masterIdx] = sortedSlaveIndices[dictIdx];
					continue;
				}

				// dictIdx was negative, we have to take the complement
				dictIdx = ~dictIdx;
				if (dictIdx >= sortedSlaveIndices.Length)
				{
					result[masterIdx] = double.NaN;
					continue;
				}
				else if (dictIdx == 0)
				{
					result[masterIdx] = double.NaN;
					continue;
				}
				else
				{
					int firstSlaveIdx = sortedSlaveIndices[dictIdx - 1];
					int secondSlaveIdx = sortedSlaveIndices[dictIdx];
					double firstSlaveValue = sortedSlaveValues[dictIdx - 1];
					double secondSlaveValue = sortedSlaveValues[dictIdx];

					double diff = secondSlaveValue - firstSlaveValue;

					if (diff == 0)
					{
						result[masterIdx] = firstSlaveIdx;
						continue;
					}
					else
					{
						result[masterIdx] = firstSlaveIdx + (secondSlaveIdx - firstSlaveIdx) * (masterValue - firstSlaveValue) / diff;
						continue;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the fractional index for merging of two tables.
		/// </summary>
		/// <param name="masterColumn">X-column of the master table.</param>
		/// <param name="slaveColumn">X-column of the slave table.</param>
		/// <returns>Array of fractional indices. Each item points into the slaveTable to the value that should be included in the master column at the item's index.</returns>
		public static DoubleColumn GetFractionalIndex(DateTimeColumn masterColumn, DateTimeColumn slaveColumn)
		{
			int masterCount = masterColumn.Count;
			int slaveCount = slaveColumn.Count;

			var result = new DoubleColumn();
			var dict = new SortedDictionary<DateTime, int>();
			for (int i = slaveCount - 1; i >= 0; i--)
				dict[slaveColumn[i]] = i;

			var sortedSlaveValues = dict.Keys.ToArray();
			var sortedSlaveIndices = dict.Values.ToArray();

			for (int masterIdx = 0; masterIdx < masterCount; masterIdx++)
			{
				var masterValue = masterColumn[masterIdx];
				int dictIdx = Array.BinarySearch(sortedSlaveValues, masterValue);
				if (dictIdx >= 0)
				{
					result[masterIdx] = sortedSlaveIndices[dictIdx];
					continue;
				}

				// dictIdx was negative, we have to take the complement
				dictIdx = ~dictIdx;
				if (dictIdx >= sortedSlaveIndices.Length)
				{
					result[masterIdx] = double.NaN;
					continue;
				}
				else if (dictIdx == 0)
				{
					result[masterIdx] = double.NaN;
					continue;
				}
				else
				{
					int firstSlaveIdx = sortedSlaveIndices[dictIdx - 1];
					int secondSlaveIdx = sortedSlaveIndices[dictIdx];
					var firstSlaveValue = sortedSlaveValues[dictIdx - 1];
					var secondSlaveValue = sortedSlaveValues[dictIdx];

					var diff = (secondSlaveValue - firstSlaveValue);

					if (diff.Ticks == 0)
					{
						result[masterIdx] = firstSlaveIdx;
						continue;
					}
					else
					{
						result[masterIdx] = firstSlaveIdx + (secondSlaveIdx - firstSlaveIdx) * (masterValue - firstSlaveValue).Ticks / ((double)diff.Ticks);
						continue;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the fractional index for merging of two tables.
		/// </summary>
		/// <param name="masterColumn">X-column of the master table.</param>
		/// <param name="slaveColumn">X-column of the slave table.</param>
		/// <returns>Array of fractional indices. Each item points into the slaveTable to the value that should be included in the master column at the item's index.</returns>
		public static DoubleColumn GetFractionalIndex(DataColumn masterColumn, DataColumn slaveColumn)
		{
			if (masterColumn is DateTimeColumn && slaveColumn is DateTimeColumn)
				return GetFractionalIndex((DateTimeColumn)masterColumn, (DateTimeColumn)slaveColumn);

			if (masterColumn is INumericColumn && slaveColumn is INumericColumn)
				return GetFractionalIndex((INumericColumn)masterColumn, (INumericColumn)slaveColumn);

			throw new ArgumentException(string.Format("Unable to create fractional index from columns of type {0} and {1}", masterColumn.GetType(), slaveColumn.GetType()));
		}

		/// <summary>
		/// Adds to the destination table selected columns from another table. Additionally, the properties of those columns will be added to the destination table.
		/// </summary>
		/// <param name="destinationTable">Table where the columns should be added to.</param>
		/// <param name="tableToAddFrom">Source table.</param>
		/// <param name="selectedColumns">Indexes of the columns of the source table that should be added to the destination table.</param>
		public static void AddDataColumnsWithPropertiesFrom(this DataTable destinationTable, DataTable tableToAddFrom, IAscendingIntegerCollection selectedColumns)
		{
			IAscendingIntegerCollection mergedPropColsIndices = destinationTable.PropCols.MergeColumnTypesFrom(tableToAddFrom.PropCols);

			for (int i = 0; i < selectedColumns.Count; i++)
			{
				int sourceIdx = selectedColumns[i];
				int destinationIdx = destinationTable.DataColumns.AppendCopiedColumnFrom(tableToAddFrom.DataColumns, sourceIdx, true, true);

				for (int j = 0; j < mergedPropColsIndices.Count; j++)
				{
					destinationTable.PropCols[mergedPropColsIndices[j]][destinationIdx] = tableToAddFrom.PropCols[j][sourceIdx];
				}
			}
		}
	}
}