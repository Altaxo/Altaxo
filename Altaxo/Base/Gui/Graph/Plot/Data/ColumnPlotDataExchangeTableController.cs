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

using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph.Plot.Data
{
	#region Interfaces

	public interface IColumnPlotDataExchangeTableView
	{
		/// <summary>
		/// Initialize the list of available tables.
		/// </summary>
		/// <param name="items">The items.</param>
		void AvailableTables_Initialize(SelectableListNodeList items);

		/// <summary>
		/// Initialize the list of tables that fit to the current chosen columns.
		/// </summary>
		/// <param name="items">The items.</param>
		void MatchingTables_Initialize(SelectableListNodeList items);

		event Action SelectedTableChanged;

		event Action SelectedMatchingTableChanged;
	}

	/// <summary>
	/// Model used to exchange the data table in multiple plot items concurrently.
	/// </summary>
	public class ColumnPlotDataExchangeTableData : ICloneable
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
		/// Changes the underlying table for the provided plot items.
		/// </summary>
		/// <param name="plotItems">The plot items for which to change the underlying data table.</param>
		/// <param name="newTable">The new table.</param>
		public static void ChangeTableForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems, DataTable newTable)
		{
			// collect all column names from those plot items
			foreach (var node in plotItems)
			{
				var data = (IColumnPlotData)node.DataObject;
				if (object.ReferenceEquals(newTable, data.DataTable))
					continue;

				foreach (var columnInfo in data.DataRowSelection.GetAdditionallyUsedColumns())
				{
					if (columnInfo.Column is DataColumn && newTable.DataColumns.Contains(columnInfo.ColumnName))
						columnInfo.ColumnSetAction(newTable.DataColumns[columnInfo.ColumnName]);
				}

				// first the data
				// now the data itself
				foreach (var t in data.GetAdditionallyUsedColumns())
				{
					foreach (var columnInfo in t.columnInfos)
					{
						if (columnInfo.Column is DataColumn && newTable.DataColumns.Contains(columnInfo.ColumnName))
						{
							var newDataCol = newTable.DataColumns[columnInfo.ColumnName];
							int newGroup = newTable.DataColumns.GetColumnGroup(newDataCol);
							columnInfo.SetColumnAction(newDataCol, newTable, newGroup);
						}
					}
				}

				// and now to the styles

				if (node.StyleObject is IEnumerable<Altaxo.Graph.Plot.Styles.IGPlotStyle> styleEn)
				{
					foreach (var style in styleEn)
					{
						foreach (var columnInfo in style.GetAdditionallyUsedColumns() ?? EmptyColumnInfoEnumeration)
						{
							if (columnInfo.Column is DataColumn && newTable.DataColumns.Contains(columnInfo.ColumnName))
							{
								columnInfo.ColumnSetAction(newTable.DataColumns[columnInfo.ColumnName]);
							}
						}
					}
				}
				else if (node.StyleObject is Altaxo.Graph.Plot.Styles.IGPlotStyle style)
				{
					foreach (var columnInfo in style.GetAdditionallyUsedColumns() ?? EmptyColumnInfoEnumeration)
					{
						if (columnInfo.Column is DataColumn && newTable.DataColumns.Contains(columnInfo.ColumnName))
						{
							columnInfo.ColumnSetAction(newTable.DataColumns[columnInfo.ColumnName]);
						}
					}
				}
			}
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
				var data = (IColumnPlotData)plotItem.DataObject;
				if (!object.ReferenceEquals(OriginalTable, data.DataTable))
					continue;

				var columnNames = new List<string>();
				ColumnNames.Add((plotItem.ToString(), columnNames));

				// collect from the plot item's row selection
				foreach (var columnInfo in data.DataRowSelection.GetAdditionallyUsedColumns())
				{
					if (columnInfo.Column is DataColumn)
						columnNames.Add(columnInfo.ColumnName);
				}

				// now collect from the data itself
				foreach (var t in data.GetAdditionallyUsedColumns())
				{
					foreach (var columnInfo in t.columnInfos)
					{
						if (columnInfo.Column is DataColumn)
							columnNames.Add(columnInfo.ColumnName);
					}
				}

				// and now from the style

				if (plotItem.StyleObject is IEnumerable<Altaxo.Graph.Plot.Styles.IGPlotStyle> styleEn)
				{
					foreach (var style in styleEn)
					{
						foreach (var columnInfo in style.GetAdditionallyUsedColumns() ?? EmptyColumnInfoEnumeration)
						{
							if (columnInfo.Column is DataColumn)
								columnNames.Add(columnInfo.ColumnName);
						}
					}
				}
				else if (plotItem.StyleObject is Altaxo.Graph.Plot.Styles.IGPlotStyle style)
				{
					foreach (var columnInfo in style.GetAdditionallyUsedColumns() ?? EmptyColumnInfoEnumeration)
					{
						if (columnInfo.Column is DataColumn)
							columnNames.Add(columnInfo.ColumnName);
					}
				}
			}
		}

		/// <summary>
		/// Gets an empty column information enumeration.
		/// </summary>
		/// <value>
		/// The empty column information enumeration.
		/// </value>
		private static IEnumerable<(
				string ColumnLabel, // Column label
				IReadableColumn Column, // the column as it was at the time of this call
				string ColumnName, // the name of the column (last part of the column proxies document path)
				Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
				)> EmptyColumnInfoEnumeration
		{
			get
			{
				return Enumerable.Empty<(
			string ColumnLabel, // Column label
			IReadableColumn Column, // the column as it was at the time of this call
			string ColumnName, // the name of the column (last part of the column proxies document path)
			Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
			)>();
			}
		}
	}

	#endregion Interfaces

	[ExpectedTypeOfView(typeof(IColumnPlotDataExchangeTableView))]
	[UserControllerForObject(typeof(ColumnPlotDataExchangeTableData))]
	public class ColumnPlotDataExchangeTableController
		:
			MVCANControllerEditOriginalDocBase<ColumnPlotDataExchangeTableData, IColumnPlotDataExchangeTableView>
	{
		#region Members

		protected bool _isDirty = false;

		/// <summary>All datatables of the document</summary>
		protected SelectableListNodeList _availableTables = new SelectableListNodeList();

		/// <summary>Tuples from tables and group numbers, for which the columns in that group contain all that column names which are currently plot columns in our controller.</summary>
		protected SelectableListNodeList _matchingTables = new SelectableListNodeList();

		/// <summary>Tasks which updates the _fittingTables.</summary>
		protected Task _updateMatchingTablesTask;

		/// <summary>TokenSource to cancel the tasks which updates the _fittingTables.</summary>
		protected CancellationTokenSource _updateMatchingTablesTaskCancellationTokenSource = new CancellationTokenSource();

		#endregion Members

		#region Infrastructur Dispose and GetSubControllers

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_availableTables = null;

			_updateMatchingTablesTaskCancellationTokenSource?.Cancel();
			while (_updateMatchingTablesTask?.Status == TaskStatus.Running)
				Thread.Sleep(20);
			_updateMatchingTablesTaskCancellationTokenSource?.Dispose();
			_updateMatchingTablesTaskCancellationTokenSource = null;
			_updateMatchingTablesTask?.Dispose();
			_updateMatchingTablesTask = null;

			base.Dispose(isDisposing);
		}

		public void SetDirty()
		{
			_isDirty = true;
		}

		#endregion Infrastructur Dispose and GetSubControllers

		#region Initialize, Apply, Attach, Detach

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_doc.NewTable = _doc.OriginalTable;

				// Initialize tables
				string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

				_availableTables.Clear();
				DataTable tg = _doc.OriginalTable;
				foreach (var tableName in tables)
				{
					_availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], tg != null && tg.Name == tableName));
				}

				TriggerUpdateOfMatchingTables();
			}

			if (null != _view)
			{
				_view.AvailableTables_Initialize(_availableTables);
				_view.MatchingTables_Initialize(_matchingTables);
			}
		}

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.SelectedTableChanged += EhView_TableSelectionChanged;

			_view.SelectedMatchingTableChanged += EhView_MatchingTableSelectionChanged;
		}

		protected override void DetachView()
		{
			_view.SelectedTableChanged -= EhView_TableSelectionChanged;

			_view.SelectedMatchingTableChanged -= EhView_MatchingTableSelectionChanged;

			base.DetachView();
		}

		#endregion Initialize, Apply, Attach, Detach

		#region AvailableDataTables

		public void EhView_TableSelectionChanged()
		{
			var node = _availableTables.FirstSelectedNode;
			DataTable tg = node?.Tag as DataTable;

			if (null == tg || object.ReferenceEquals(_doc.NewTable, tg))
				return;

			_doc.NewTable = tg;
		}

		#endregion AvailableDataTables

		#region MatchingDataTables

		// Matching data tables are those tables, which have at least one group of columns which best fits the existing plot item column names

		/// <summary>
		/// Occurs if the selection for the matching tables has changed.
		/// </summary>
		public void EhView_MatchingTableSelectionChanged()
		{
			var node = _matchingTables.FirstSelectedNode;
			if (null == node)
				return; // no node selected

			var tag = (DataTable)node.Tag;

			if (object.ReferenceEquals(_doc.NewTable, tag)) // then nothing will change
				return;

			_doc.NewTable = (DataTable)tag;

			_availableTables.SetSelection((nd) => object.ReferenceEquals(nd.Tag, _doc.NewTable));
			_view?.AvailableTables_Initialize(_availableTables);
		}

		private void TriggerUpdateOfMatchingTables()
		{
			if (_updateMatchingTablesTask?.Status == TaskStatus.Running)
			{
				_updateMatchingTablesTaskCancellationTokenSource.Cancel();
				while (_updateMatchingTablesTask?.Status == TaskStatus.Running)
					System.Threading.Thread.Sleep(20);
			}

			_matchingTables = new SelectableListNodeList();
			_view?.MatchingTables_Initialize(_matchingTables);

			var token = _updateMatchingTablesTaskCancellationTokenSource.Token;
			_updateMatchingTablesTask = Task.Factory.StartNew(() => UpdateMatchingTables(token));
		}

		private void UpdateMatchingTables(System.Threading.CancellationToken cancellationToken)
		{
			var fittingTables2 = new SelectableListNodeList(); // we always update a new list, because _fittingTable1 is bound to the UI

			foreach (var table in GetTablesThatFitExistingGroupsOfPlotColumns(_doc.ColumnNames.Select(g => g.columnNames), cancellationToken))
			{
				fittingTables2.Add(new SelectableListNode(
					table.Name,
					table,
					object.ReferenceEquals(table, _doc.NewTable)));
			}

			_matchingTables = fittingTables2;
			Current.Dispatcher.InvokeAndForget(() => _view?.MatchingTables_Initialize(_matchingTables));
		}

		/// <summary>
		/// Gets all tables with column names that match all groups of column names provided in <paramref name="groupsOfColumnNames"/>.
		/// Each group of column names must exist in one specific column group of the table.
		/// </summary>
		/// <param name="groupsOfColumnNames">The groups of column names.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public static IEnumerable<DataTable> GetTablesThatFitExistingGroupsOfPlotColumns(IEnumerable<IEnumerable<string>> groupsOfColumnNames, System.Threading.CancellationToken cancellationToken)
		{
			HashSet<DataTable> result = null;

			foreach (var columnNames in groupsOfColumnNames)
			{
				if (result == null)
				{
					result = new HashSet<DataTable>(GetTablesWithGroupThatFitExistingPlotColumns(columnNames, cancellationToken).Select(n => n.dataTable));
				}
				else
				{
					result.IntersectWith(GetTablesWithGroupThatFitExistingPlotColumns(columnNames, cancellationToken).Select(n => n.dataTable));
				}
			}

			return result;
		}

		/// <summary>
		/// Gets all tables together with the group number which have a group with column names that match the provided column names in <paramref name="columnNames"/>.
		/// </summary>
		/// <param name="columnNames">The column names to match.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public static IEnumerable<(DataTable dataTable, int groupNumber)> GetTablesWithGroupThatFitExistingPlotColumns(IEnumerable<string> columnNames, System.Threading.CancellationToken cancellationToken)
		{
			var columnNamesThatMustFit = columnNames is HashSet<string> ? (HashSet<string>)columnNames : new HashSet<string>(columnNames);

			if (cancellationToken.IsCancellationRequested)
				yield break;

			if (columnNamesThatMustFit.Count == 0)
				yield break; // we decide here that when there is no column that must fit, we return no table, because then we can use the all available tables combobox anyway.

			// now we iterate through all tables to find tables which can fullfil our criterium

			foreach (var table in Current.Project.DataTableCollection)
			{
				var groupNumbersAll = table.DataColumns.GetGroupNumbersAll();

				foreach (var groupNumber in groupNumbersAll)
				{
					if (cancellationToken.IsCancellationRequested)
						yield break;

					var columnNamesExisting = new HashSet<string>(columnNamesThatMustFit); // make a copy of this

					// and now eliminate all columns that also exist in this table
					foreach (var name in table.DataColumns.GetNamesOfColumnsWithGroupNumber(groupNumber))
					{
						if (columnNamesExisting.Remove(name) && columnNamesExisting.Count == 0)
						{
							yield return (table, groupNumber); // Count is null, so this is a fitting table
							break;
						}
					}
				}
			}
		}

		#endregion MatchingDataTables
	}
}