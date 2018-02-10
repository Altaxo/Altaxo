#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Plot.Data
{
	/// <summary>
	/// Model used to exchange the data table in multiple plot items concurrently.
	/// </summary>
	public class ColumnPlotDataExchangeTableData : ColumnPlotDataExchangeDataBase, ICloneable
	{
		/// <summary>
		/// Gets the column names currently used by the plot items. The list contains tuples, consisting of the
		/// name of the plot item, and the list of column names this plot items uses.
		/// </summary>
		public List<(string itemName, List<string> columnNames)> ColumnNames { get; } = new List<(string itemName, List<string> columnNames)>();

		/// <summary>
		/// Gets or sets the original table used by the plot items.
		/// </summary>
		public DataTable OriginalTable { get; set; }

		/// <summary>
		/// Gets or sets the table, that should become the new data table of the plot items.
		/// </summary>
		/// <value>
		/// The new table.
		/// </value>
		public DataTable NewTable { get; set; }

		/// <inheritdoc/>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Determines whether it is possible to change the underlying table for the specified plot items.
		/// The table can be changed if all plot items have exactly the same underlying table.
		/// </summary>
		/// <param name="plotItems">The plot items.</param>
		/// <returns>
		///   <c>true</c> if this instance [can change table for plot items] the specified plot items; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanChangeTableForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
		{
			var firstSelectedPlotItem = plotItems.FirstOrDefault();
			if (null == firstSelectedPlotItem)
				return false;

			var table = ((IColumnPlotData)firstSelectedPlotItem.DataObject).DataTable;

			foreach (var node in plotItems)
			{
				if (!object.ReferenceEquals(table, ((IColumnPlotData)node.DataObject).DataTable))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Shows a dialog that allows to change the underlying data table for the provided plot items.
		/// </summary>
		/// <param name="plotItems">The plot items.</param>
		/// <returns>True if the dialog was shown and for at least one plot item the underlying table was exchanged; otherwise false.</returns>
		public static bool ShowChangeTableForSelectedItemsDialog(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
		{
			if (!CanChangeTableForPlotItems(plotItems))
				return false;

			// get all selected plot items with IColumnPlotData
			var firstSelectedPlotItem = plotItems.FirstOrDefault();
			if (null == firstSelectedPlotItem)
				return false;

			var exchangeTableData = new ColumnPlotDataExchangeTableData
			{
				PlotItems = plotItems,
				OriginalTable = ((IColumnPlotData)firstSelectedPlotItem.DataObject).DataTable
			};

			exchangeTableData.CollectColumnNamesFromPlotItems(plotItems);

			object exchangeTableDataObject = exchangeTableData;
			if (!Current.Gui.ShowDialog(ref exchangeTableDataObject, "Select new table for plot items"))
				return false;

			exchangeTableData = (ColumnPlotDataExchangeTableData)exchangeTableDataObject;

			if (object.ReferenceEquals(exchangeTableData.OriginalTable, exchangeTableData.NewTable))
				return false; // nothing to do

			// apply the new table

			exchangeTableData.ChangeTableForPlotItems(plotItems);

			return true;
		}

		/// <summary>
		/// Changes the underlying table for the provided plot items, using the new data table in <see cref="NewTable"/>.
		/// </summary>
		/// <param name="plotItems">The plot items for which to change the underlying data table.</param>
		public void ChangeTableForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
		{
			ChangeTableForPlotItems(plotItems, this.NewTable);
		}

		/// <summary>
		/// Collects the column names from the provided plot items and organizes them in groups (see <see cref="ColumnNames"/>), one group for each plot item. See remarks for why to organize in groups.
		/// </summary>
		/// <param name="plotItems">The plot items to collect from.</param>
		/// <remarks>The names have to be organized in groups. The reason is that each plot item should use columns from a single group number only.
		/// Thus in order to determine whether a table can be used to replace the old table of all plot items, we need to know which column names belong together.</remarks>
		public void CollectColumnNamesFromPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
		{
			// collect all column names from those plot items
			foreach (var plotItem in plotItems)
			{
				if (plotItem.DataObject is IColumnPlotData columnPlotData && !object.ReferenceEquals(OriginalTable, columnPlotData.DataTable))
					continue;

				var columnNames = new List<string>();
				ColumnNames.Add((plotItem.ToString(), columnNames));

				// collect from the plot item's row selection
				foreach (var columnInfo in EnumerateAllDataColumnsOfPlotItem(plotItem, (info) => OriginalTable.DataColumns.Contains(info.Column)))
				{
					columnNames.Add(columnInfo.ColumnName);
				}
			}
		}

		/// <summary>
		/// Changes the underlying table for the provided plot items.
		/// </summary>
		/// <param name="plotItems">The plot items for which to change the underlying data table.</param>
		/// <param name="newTable">The new table.</param>
		public static void ChangeTableForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems, DataTable newTable)
		{
			ChangeTableForPlotItems(plotItems, newTable, false);
		}

		/// <summary>
		/// Tests to changes the underlying table for the plot items, and output a statistics.
		/// </summary>
		/// <returns>A statistics with the number of plot items for which the table could be changed, the number of successfully exchanged columns, and the number of unsuccessfully changed columns.</returns>
		public (int NumberOfPlotItemsChanged, int NumberOfSuccessFullyChangedColumns, int NumberOfUnsuccessfullyChangedColumns) TestChangeTableForPlotItems()
		{
			return TestChangeTableForPlotItems(PlotItems, NewTable);
		}

		/// <summary>
		/// Tests to changes the underlying table for the provided plot items, and output a statistics.
		/// </summary>
		/// <param name="plotItems">The plot items for which to change the underlying data table.</param>
		/// <param name="newTable">The new table.</param>
		/// <returns>A statistics with the number of plot items for which the table could be changed, the number of successfully exchanged columns, and the number of unsuccessfully changed columns.</returns>
		public static (int NumberOfPlotItemsChanged, int NumberOfSuccessFullyChangedColumns, int NumberOfUnsuccessfullyChangedColumns) TestChangeTableForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems, DataTable newTable)
		{
			return ChangeTableForPlotItems(plotItems, newTable, true);
		}

		/// <summary>
		/// Changes the underlying table for the provided plot items.
		/// </summary>
		/// <param name="plotItems">The plot items for which to change the underlying data table.</param>
		/// <param name="newTable">The new table.</param>
		/// <param name="isForStatisticsOnly">If true, the exchange of columns is not really done, but statistics only is provided.</param>
		private static (int NumberOfPlotItemsChanged, int NumberOfSuccessFullyChangedColumns, int NumberOfUnsuccessfullyChangedColumns) ChangeTableForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems, DataTable newTable, bool isForStatisticsOnly)
		{
			int numberOfPlotItemsChanged = 0;
			int numberOfSuccessfullyChangedColumns = 0;
			int numberOfUnsuccessfullyChangedColumns = 0;

			// collect all column names from those plot items
			foreach (var plotItem in plotItems)
			{
				if (plotItem.DataObject is IColumnPlotData columnPlotData && object.ReferenceEquals(newTable, columnPlotData.DataTable))
					continue;

				foreach (var columnInfo in EnumerateAllDataColumnsOfPlotItem(plotItem, (dataColumn) => true))
				{
					if (newTable.DataColumns.Contains(columnInfo.ColumnName))
					{
						var newDataCol = newTable.DataColumns[columnInfo.ColumnName];
						int newGroup = newTable.DataColumns.GetColumnGroup(newDataCol);
						if (!isForStatisticsOnly)
						{
							columnInfo.ColumnSetAction(newDataCol, newTable, newGroup);
						}

						++numberOfSuccessfullyChangedColumns;
					}
					else
					{
						++numberOfUnsuccessfullyChangedColumns;
					}
				}

				++numberOfPlotItemsChanged;
			}

			return (numberOfPlotItemsChanged, numberOfSuccessfullyChangedColumns, numberOfUnsuccessfullyChangedColumns);
		}
	}
}