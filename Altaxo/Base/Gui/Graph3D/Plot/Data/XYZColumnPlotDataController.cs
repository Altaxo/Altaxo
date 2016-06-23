#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Gui.Graph3D.Plot.Data
{
	#region Interfaces

	public enum ColumnControlState
	{
		Normal,
		Warning,
		Error
	}

	public class ColumnTag
	{
		public ColumnTag(int groupNumber, int columnNumber)
		{
			GroupNumber = groupNumber;
			ColumnNumber = columnNumber;
		}

		public int GroupNumber { get; private set; }
		public int ColumnNumber { get; private set; }
	}

	public interface IXYZColumnPlotDataView
	{
		void Tables_Initialize(SelectableListNodeList items);

		void Columns_Initialize(SelectableListNodeList items);

		void OtherAvailableColumns_Initialize(SelectableListNodeList items);

		void TargetColumns_Initialize(
			IEnumerable<Tuple< // list of all groups
			string, // Caption for each group of columns
			IEnumerable<Tuple< // list of column definitions
				ColumnTag, // tag to identify the column and group
				string, // Label of the column
				string, // name of the column,
				string, // tooltip
				ColumnControlState>
			>>> groups);

		void Column_Update(ColumnTag tag, string colname, string toolTip, ColumnControlState state);

		void GroupNumber_Initialize(int groupNumber, bool isEnabled);

		void PlotRangeFrom_Initialize(int from);

		void PlotRangeTo_Initialize(int to);

		event Action TableSelectionChanged;

		event Action<ColumnTag> Column_AddTo;

		event Action<ColumnTag> Column_Erase;

		event Action<int> RangeFromChanged;

		event Action<int> RangeToChanged;

		event Action<int> GroupNumberChanged;

		event CanStartDragDelegate AvailableDataColumns_CanStartDrag;

		event StartDragDelegate AvailableDataColumns_StartDrag;

		event DragEndedDelegate AvailableDataColumns_DragEnded;

		event DragCancelledDelegate AvailableDataColumns_DragCancelled;

		event CanStartDragDelegate OtherAvailableItems_CanStartDrag;

		event StartDragDelegate OtherAvailableItems_StartDrag;

		event DragEndedDelegate OtherAvailableItems_DragEnded;

		event DragCancelledDelegate OtherAvailableItems_DragCancelled;

		event DropCanAcceptDataDelegate Column_DropCanAcceptData;

		event DropDelegate Column_Drop;
	}

	public interface IColumnDataController : IMVCANController
	{
		/// <summary>
		/// Sets the additional columns that are used by some of the plot styles.
		/// </summary>
		/// <param name="additionalColumns">The additional columns. This is an enumerable of tuples, each tuple corresponding to one plot style.
		/// The first item of this tuple is the plot style's number and name. The second item is another enumeration of tuples.
		/// Each tuple in this second enumeration consist of the name of the column (first item) and a function which returns the column proxy which
		/// can be used to get or set the underlying column.</param>
		void SetAdditionalColumns(
			IEnumerable<Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>>>> additionalColumns
			);
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for LineScatterPlotDataController.
	/// </summary>
	[UserControllerForObject(typeof(XYZColumnPlotData))]
	[ExpectedTypeOfView(typeof(IXYZColumnPlotDataView))]
	public class XYZColumnPlotDataController
		:
		MVCANControllerEditOriginalDocBase<XYZColumnPlotData, IXYZColumnPlotDataView>, IColumnDataController
	{
		private class ColumnInfo
		{
			public string Label;
			public Altaxo.Data.IReadableColumn Column;
			public string ColumnNameToShow;
			public string ColumnNameToCache;
			public string ToolTip;
			public ColumnControlState State;
			public string Tag;
			public Action<IReadableColumn> ColumnSetter;

			public void UpdateTooltipAndState(DataTable dataTableOfPlotItem)
			{
				if (null == Column)
				{
					if (string.IsNullOrEmpty(ColumnNameToCache))
					{
						ColumnNameToShow = string.Empty;
						ToolTip = string.Empty;
						State = ColumnControlState.Normal;
					}
					else
					{
						ColumnNameToShow = ColumnNameToCache;
						ToolTip = string.Format("Column {0} can not be found in this table with this group number", ColumnNameToCache);
						State = ColumnControlState.Error;
					}
				}
				else if (Column is DataColumn)
				{
					var dcolumn = (DataColumn)Column;
					var parentTable = DataTable.GetParentDataTableOf(dcolumn);
					var parentCollection = DataColumnCollection.GetParentDataColumnCollectionOf(dcolumn);
					if (null == parentTable)
					{
						ToolTip = string.Format("This column is an orphaned data column without a parent data table", ColumnNameToShow);
						State = ColumnControlState.Error;
						if (parentCollection == null)
							ColumnNameToShow = string.Format("Orphaned {0}", dcolumn.GetType().Name);
						else
							ColumnNameToShow = ColumnNameToCache = parentCollection.GetColumnName(dcolumn);
					}
					else // Column has a parent table
					{
						if (!object.ReferenceEquals(parentTable, dataTableOfPlotItem))
						{
							ColumnNameToShow = parentTable.DataColumns.GetColumnName(dcolumn);
							ToolTip = string.Format("The column {0} is a data column with another parent data table: {1}", ColumnNameToShow, parentTable.Name);
							State = ColumnControlState.Warning;
						}
						else
						{
							ColumnNameToShow = ColumnNameToCache = parentTable.DataColumns.GetColumnName(dcolumn);
							ToolTip = string.Format("Column {0} of data table {1}", ColumnNameToShow, parentTable.Name);
							State = ColumnControlState.Normal;
						}
					}
				}
				else // Column is something else
				{
					ColumnNameToShow = Column.FullName;
					ColumnNameToCache = null;
					ToolTip = string.Format("Independent data of type {0}: {1}", Column.GetType(), Column.ToString());
					State = ColumnControlState.Normal;
				}
			}

			public void UpdateColumnNameToCache()
			{
				if (null == Column)
				{
					ColumnNameToCache = null;
				}
				else if (Column is DataColumn)
				{
					var dcolumn = (DataColumn)Column;
					var parentTable = DataColumnCollection.GetParentDataColumnCollectionOf(dcolumn);
					ColumnNameToCache = parentTable?.GetColumnName(dcolumn);
				}
				else // Column is something else
				{
					ColumnNameToCache = null;
				}
			}
		}

		private class GroupInfo
		{
			public string GroupName;
			public List<ColumnInfo> Columns = new List<ColumnInfo>();
		}

		private List<GroupInfo> _columnGroup;

		private bool _isDirty = false;

		private int _plotRangeFrom;
		private int _plotRangeTo;

		private int _maxPossiblePlotRangeTo;

		private SelectableListNodeList _tableItems = new SelectableListNodeList();
		private SelectableListNodeList _columnItems = new SelectableListNodeList();
		private SelectableListNodeList _otherAvailableColumns = new SelectableListNodeList();
		private SortedSet<int> _groupNumbersAll;

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_columnGroup = null;
			_tableItems = null;
			_columnItems = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				// Fix docs datatable

				if (_doc.DataTable == null)
				{
					_doc.DataTable = DataTable.GetParentDataTableOf(_doc.XColumn as DataColumn);
					if (null != _doc.DataTable && _doc.DataTable.DataColumns.ContainsColumn((DataColumn)_doc.XColumn))
						_doc.GroupNumber = _doc.DataTable.DataColumns.GetColumnGroup((DataColumn)_doc.XColumn);
				}
				if (_doc.DataTable == null)
				{
					_doc.DataTable = DataTable.GetParentDataTableOf(_doc.YColumn as DataColumn);
					if (null != _doc.DataTable && _doc.DataTable.DataColumns.ContainsColumn((DataColumn)_doc.YColumn))
						_doc.GroupNumber = _doc.DataTable.DataColumns.GetColumnGroup((DataColumn)_doc.YColumn);
				}
				if (_doc.DataTable == null)
				{
					_doc.DataTable = DataTable.GetParentDataTableOf(_doc.ZColumn as DataColumn);
					if (null != _doc.DataTable && _doc.DataTable.DataColumns.ContainsColumn((DataColumn)_doc.ZColumn))
						_doc.GroupNumber = _doc.DataTable.DataColumns.GetColumnGroup((DataColumn)_doc.ZColumn);
				}

				// initialize group 0

				if (null == _columnGroup)
					_columnGroup = new List<GroupInfo>();

				if (_columnGroup.Count == 0)
					_columnGroup.Add(new GroupInfo { GroupName = "#0: data (X-Y-Z)" });
				else
					_columnGroup[0].Columns.Clear();

				_columnGroup[0].Columns.Add(new ColumnInfo() { Label = "X", Column = _doc.XColumn, ColumnNameToShow = _doc.XColumnName, ColumnNameToCache = _doc.XColumnName, ColumnSetter = (column) => _doc.XColumn = column });
				_columnGroup[0].Columns.Add(new ColumnInfo() { Label = "Y", Column = _doc.YColumn, ColumnNameToShow = _doc.YColumnName, ColumnNameToCache = _doc.YColumnName, ColumnSetter = (column) => _doc.YColumn = column });
				_columnGroup[0].Columns.Add(new ColumnInfo() { Label = "Z", Column = _doc.ZColumn, ColumnNameToShow = _doc.ZColumnName, ColumnNameToCache = _doc.ZColumnName, ColumnSetter = (column) => _doc.ZColumn = column });

				for (int i = 0; i < 3; ++i)
					_columnGroup[0].Columns[i].UpdateTooltipAndState(_doc.DataTable);

				_plotRangeFrom = _doc.PlotRangeStart;
				_plotRangeTo = _doc.PlotRangeLength == int.MaxValue ? int.MaxValue : _doc.PlotRangeStart + _doc.PlotRangeLength - 1;
				CalcMaxPossiblePlotRangeTo();

				// Initialize tables
				string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

				_tableItems.Clear();
				DataTable tg = _doc.DataTable;
				foreach (var tableName in tables)
				{
					_tableItems.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], tg != null && tg.Name == tableName));
				}

				// Group number
				_groupNumbersAll = tg.DataColumns.GetGroupNumbersAll();

				// Initialize columns
				_columnItems.Clear();
				if (null != tg)
				{
					var columns = tg.DataColumns.GetListOfColumnsWithGroupNumber(_doc.GroupNumber);
					for (int i = 0; i < columns.Count; ++i)
						_columnItems.Add(new SelectableListNode(tg.DataColumns.GetColumnName(columns[i]), columns[i], false));
				}

				// Initialize other available columns
				InitializeOtherAvailableColumns();
			}

			if (null != _view)
			{
				_view.Tables_Initialize(_tableItems);
				_view.GroupNumber_Initialize(_doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _doc.GroupNumber != _groupNumbersAll.Min));

				_view.Columns_Initialize(_columnItems);

				_view.TargetColumns_Initialize(GetEnumerationForAllGroupsOfColumns(_columnGroup));

				_view.PlotRangeFrom_Initialize(_plotRangeFrom);
				CalcMaxPossiblePlotRangeTo();

				_view.OtherAvailableColumns_Initialize(_otherAvailableColumns);
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (_isDirty)
			{
				for (int i = 0; i < _columnGroup.Count; ++i)
					for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
						_columnGroup[i].Columns[j].ColumnSetter(_columnGroup[i].Columns[j].Column);

				_doc.PlotRangeStart = this._plotRangeFrom;
				_doc.PlotRangeLength = this._plotRangeTo >= this._maxPossiblePlotRangeTo ? int.MaxValue : this._plotRangeTo + 1 - this._plotRangeFrom;
			}
			_isDirty = false;

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.TableSelectionChanged += EhView_TableSelectionChanged;

			_view.Column_AddTo += EhView_ColumnAddTo;

			_view.Column_Erase += EhView_ColumnErase;

			_view.RangeFromChanged += EhView_RangeFrom;

			_view.RangeToChanged += EhView_RangeTo;

			_view.GroupNumberChanged += EhGroupNumberChanged;

			_view.AvailableDataColumns_CanStartDrag += EhAvailableDataColumns_CanStartDrag;
			_view.AvailableDataColumns_StartDrag += EhAvailableDataColumns_StartDrag;
			_view.AvailableDataColumns_DragEnded += EhAvailableDataColumns_DragEnded;
			_view.AvailableDataColumns_DragCancelled += EhAvailableDataColumns_DragCancelled;

			_view.OtherAvailableItems_CanStartDrag += EhOtherAvailableItems_CanStartDrag;
			_view.OtherAvailableItems_StartDrag += EhOtherAvailableItems_StartDrag;
			_view.OtherAvailableItems_DragEnded += EhOtherAvailableItems_DragEnded;
			_view.OtherAvailableItems_DragCancelled += EhOtherAvailableItems_DragCancelled;

			_view.Column_DropCanAcceptData += EhColumnDropCanAcceptData;
			_view.Column_Drop += EhColumnDrop;
		}

		protected override void DetachView()
		{
			_view.TableSelectionChanged -= EhView_TableSelectionChanged;

			_view.Column_AddTo -= EhView_ColumnAddTo;

			_view.Column_Erase -= EhView_ColumnErase;

			_view.RangeFromChanged -= EhView_RangeFrom;

			_view.RangeToChanged -= EhView_RangeTo;

			_view.GroupNumberChanged -= EhGroupNumberChanged;

			_view.AvailableDataColumns_CanStartDrag -= EhAvailableDataColumns_CanStartDrag;
			_view.AvailableDataColumns_StartDrag -= EhAvailableDataColumns_StartDrag;
			_view.AvailableDataColumns_DragEnded -= EhAvailableDataColumns_DragEnded;
			_view.AvailableDataColumns_DragCancelled -= EhAvailableDataColumns_DragCancelled;

			_view.OtherAvailableItems_CanStartDrag -= EhOtherAvailableItems_CanStartDrag;
			_view.OtherAvailableItems_StartDrag -= EhOtherAvailableItems_StartDrag;
			_view.OtherAvailableItems_DragEnded -= EhOtherAvailableItems_DragEnded;
			_view.OtherAvailableItems_DragCancelled -= EhOtherAvailableItems_DragCancelled;

			_view.Column_DropCanAcceptData -= EhColumnDropCanAcceptData;
			_view.Column_Drop -= EhColumnDrop;

			base.DetachView();
		}

		private IEnumerable<Tuple<
			string, // group name
		IEnumerable<Tuple< // list of column definitions
				ColumnTag, // tag to identify the column and group
				string, // Label for the column,
				string, // name of the column,
				string, // tooltip
				ColumnControlState>>>>
			GetEnumerationForAllGroupsOfColumns(List<GroupInfo> columnInfos)
		{
			for (int i = 0; i < columnInfos.Count; ++i)
			{
				var infoList = columnInfos[i];
				yield return new Tuple<string, IEnumerable<Tuple<ColumnTag, string, string, string, ColumnControlState>>>(
					infoList.GroupName,
					GetEnumerationForOneGroupOfColumns(infoList.Columns, i));
			}
		}

		private IEnumerable<Tuple< // list of column definitions
			ColumnTag, // tag to identify the column and group
			string, // Label for the column,
			string, // name of the column,
			string, // tooltip
			ColumnControlState>>
		GetEnumerationForOneGroupOfColumns(List<ColumnInfo> columnInfos, int groupNumber)
		{
			for (int i = 0; i < columnInfos.Count; ++i)
			{
				var info = columnInfos[i];
				yield return new Tuple<ColumnTag, string, string, string, ColumnControlState>(
					new ColumnTag(groupNumber, i),
					info.Label,
					info.ColumnNameToShow,
					info.ToolTip,
					info.State
					);
			}
		}

		/// <summary>
		/// Sets the additional columns that are used by some of the plot styles.
		/// </summary>
		/// <param name="additionalColumns">The additional columns. This is an enumerable of tuples, each tuple corresponding to one plot style.
		/// The first item of this tuple is the plot style's number and name. The second item is another enumeration of tuples.
		/// Each tuple in this second enumeration consist of the name of the column (first item) and a function which returns the column proxy which
		/// can be used to get or set the underlying column.</param>
		public void SetAdditionalColumns(IEnumerable<Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>>>> additionalColumns)
		{
			int groupNumber = 0;
			foreach (var group in additionalColumns)
			{
				++groupNumber;

				if (!(groupNumber < _columnGroup.Count))
				{
					_columnGroup.Add(new GroupInfo() { GroupName = group.Item1 });
				}
				else
				{
					_columnGroup[groupNumber].GroupName = group.Item1;
					_columnGroup[groupNumber].Columns.Clear();
				}

				foreach (var col in group.Item2)
				{
					var columnInfo = new ColumnInfo()
					{
						Label = col.Item1,
						Column = col.Item2,
						ColumnNameToCache = col.Item3,
						ColumnNameToShow = col.Item3,
						ColumnSetter = col.Item4
					};

					columnInfo.UpdateTooltipAndState(_doc.DataTable);
					_columnGroup[groupNumber].Columns.Add(columnInfo);
				}
			}

			if (null != _view)
				_view.TargetColumns_Initialize(GetEnumerationForAllGroupsOfColumns(_columnGroup));
		}

		public void SetDirty()
		{
			_isDirty = true;
		}

		#region ILineScatterPlotDataController Members

		private void InitializeOtherAvailableColumns()
		{
			var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IReadableColumn));

			foreach (var t in types)
			{
				if (Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(t, typeof(DataColumn)))
					continue; // not the DataColumn types

				if (t.IsNestedPrivate)
					continue; // types that are declared private will not be listed

				_otherAvailableColumns.Add(new SelectableListNode(t.Name, t, false));
			}
		}

		private void CalcMaxPossiblePlotRangeTo()
		{
			int len = int.MaxValue;
			for (int i = 0; i < 3; ++i)
			{
				if (_columnGroup[0].Columns[i].Column is Altaxo.Data.IDefinedCount)
					len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_columnGroup[0].Columns[i].Column).Count);
			}

			_maxPossiblePlotRangeTo = len - 1;

			if (null != _view)
				_view.PlotRangeTo_Initialize(Math.Min(this._plotRangeTo, _maxPossiblePlotRangeTo));
		}

		private string GetColumnNameToCache(IReadableColumn column)
		{
			if (null == column)
			{
				return null;
			}
			else if (column is DataColumn)
			{
				var dcolumn = (DataColumn)column;
				var parentTable = DataColumnCollection.GetParentDataColumnCollectionOf(dcolumn);
				return parentTable?.GetColumnName(dcolumn);
			}
			else // Column is something else
			{
				return null;
			}
		}

		private void ColumnInitialize(Action<string, string, ColumnControlState> SetterMethod, IReadableColumn column, string columnName)
		{
			if (null == column)
			{
				if (string.IsNullOrEmpty(columnName))
					SetterMethod(string.Empty, string.Empty, ColumnControlState.Normal);
				else
					SetterMethod(columnName, string.Format("Column {0} can not be found in this table with this group number", columnName), ColumnControlState.Error);
			}
			else if (column is DataColumn)
			{
				var dcolumn = (DataColumn)column;
				var parentTable = DataTable.GetParentDataTableOf(dcolumn);
				if (null == parentTable)
				{
					SetterMethod(columnName, string.Format("This column is an orphaned data column without a parent data table", columnName), ColumnControlState.Error);
				}
				else
				{
					if (!object.ReferenceEquals(parentTable, _doc.DataTable))
						SetterMethod(columnName, string.Format("The column {0} is a data column with another parent data table: {1}", columnName, parentTable.Name), ColumnControlState.Warning);
					else
						SetterMethod(columnName, string.Format("Column {0} of data table {1}", columnName, parentTable.Name), ColumnControlState.Normal);
				}
			}
			else // Column is something else
			{
				SetterMethod(column.ToString(), string.Format("Independent data of type {0}: {1}", column.GetType(), column.ToString()), ColumnControlState.Normal);
			}
		}

		private void ColumnInitialize(ColumnTag tag, IReadableColumn column, string columnName)
		{
			if (null == _view)
				return;

			if (null == column)
			{
				if (string.IsNullOrEmpty(columnName))
					_view.Column_Update(tag, string.Empty, string.Empty, ColumnControlState.Normal);
				else
					_view.Column_Update(tag, columnName, string.Format("Column {0} can not be found in this table with this group number", columnName), ColumnControlState.Error);
			}
			else if (column is DataColumn)
			{
				var dcolumn = (DataColumn)column;
				var parentTable = DataTable.GetParentDataTableOf(dcolumn);
				if (null == parentTable)
				{
					_view.Column_Update(tag, columnName, string.Format("This column is an orphaned data column without a parent data table", columnName), ColumnControlState.Error);
				}
				else
				{
					if (!object.ReferenceEquals(parentTable, _doc.DataTable))
						_view.Column_Update(tag, columnName, string.Format("The column {0} is a data column with another parent data table: {1}", columnName, parentTable.Name), ColumnControlState.Warning);
					else
						_view.Column_Update(tag, columnName, string.Format("Column {0} of data table {1}", columnName, parentTable.Name), ColumnControlState.Normal);
				}
			}
			else // Column is something else
			{
				_view.Column_Update(tag, column.ToString(), string.Format("Independent data of type {0}: {1}", column.GetType(), column.ToString()), ColumnControlState.Normal);
			}
		}

		public void EhView_TableSelectionChanged()
		{
			var node = _tableItems.FirstSelectedNode;
			DataTable tg = node?.Tag as DataTable;

			if (null == tg || object.ReferenceEquals(_doc.DataTable, tg))
				return;

			_doc.DataTable = tg;
			_groupNumbersAll = _doc.DataTable.DataColumns.GetGroupNumbersAll();

			// If data table has changed, try to choose a group number that matches as many as possible columns
			_doc.GroupNumber = ChooseBestMatchingGroupNumber(tg.DataColumns);
			if (_view != null)
				_view.GroupNumber_Initialize(_doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _groupNumbersAll.Min != _doc.GroupNumber));

			ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();
		}

		/// <summary>
		/// Calculates the group number which best matches the already present x, y, z columns
		/// </summary>
		/// <param name="newDataColl">The new data coll.</param>
		/// <returns>The group number </returns>
		private int ChooseBestMatchingGroupNumber(DataColumnCollection newDataColl)
		{
			var matchList = new List<DataColumn>();

			for (int i = 0; i < _columnGroup.Count; ++i)
			{
				for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
					if (_columnGroup[i].Columns[j].Column is DataColumn)
						matchList.Add((DataColumn)_columnGroup[i].Columns[j].Column);
			}

			int bestGroupNumber = _groupNumbersAll.Count > 0 ? _groupNumbersAll.Min : 0;
			int bestNumberOfPoints = 0;
			foreach (var groupNumber in _groupNumbersAll)
			{
				int numberOfPoints = 0;
				var colDict = newDataColl.GetNameDictionaryOfColumnsWithGroupNumber(groupNumber);
				foreach (var col in matchList)
				{
					DataColumn otherColumn;
					if (colDict.TryGetValue(col.Name, out otherColumn))
					{
						numberOfPoints += 5;
						if (otherColumn.GetType() == col.GetType())
							numberOfPoints += 2; // 2 additional points if the column types match
						if (groupNumber == _doc.GroupNumber)
							numberOfPoints += 1; // 1 additional point if the group number is the same as before
					}
				}
				if (numberOfPoints > bestNumberOfPoints)
				{
					bestGroupNumber = groupNumber;
					bestNumberOfPoints = numberOfPoints;
				}
			}

			return bestGroupNumber;
		}

		private void EhGroupNumberChanged(int groupNumber)
		{
			if (groupNumber != _doc.GroupNumber)
			{
				_doc.GroupNumber = groupNumber;
				ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();
			}
		}

		/// <summary>
		/// Try to replace the columns in ColumnInfo with that of the currently chosen table/group number. Additionally, the state of the columns is updated, and
		/// the changed infos are sent to the view.
		/// </summary>
		private void ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState()
		{
			// Initialize columns
			_columnItems.Clear();
			var tg = _doc.DataTable;
			if (null != tg)
			{
				var columns = tg.DataColumns.GetListOfColumnsWithGroupNumber(_doc.GroupNumber);
				for (int i = 0; i < columns.Count; ++i)
					_columnItems.Add(new SelectableListNode(tg.DataColumns.GetColumnName(columns[i]), columns[i], false));

				// now try to exchange the data columns with columns from the new group
				var colDict = tg.DataColumns.GetNameDictionaryOfColumnsWithGroupNumber(_doc.GroupNumber);

				for (int i = 0; i < _columnGroup.Count; ++i)
				{
					for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
					{
						var info = _columnGroup[i].Columns[j];

						if (info.Column is DataColumn && !string.IsNullOrEmpty(info.ColumnNameToShow) && colDict.ContainsKey(info.ColumnNameToShow))
						{
							info.Column = colDict[info.ColumnNameToShow];
						}

						info.UpdateTooltipAndState(_doc.DataTable);
						if (null != _view)
							_view.Column_Update(new ColumnTag(i, j), info.ColumnNameToShow, info.ToolTip, info.State);
					}
				}
			}
		}

		public void EhView_ColumnAddTo(ColumnTag tag)
		{
			var node = _columnItems.FirstSelectedNode;
			if (null != node)
			{
				SetDirty();
				var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
				info.Column = (DataColumn)node.Tag;
				info.UpdateColumnNameToCache();
				info.ColumnNameToShow = info.ColumnNameToCache;
				info.UpdateTooltipAndState(_doc.DataTable);

				if (null != _view)
					_view.Column_Update(tag, info.ColumnNameToShow, info.ToolTip, info.State);
			}
		}

		public void EhView_ColumnErase(ColumnTag tag)
		{
			SetDirty();
			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
			info.Column = null;
			info.ColumnNameToShow = null;
			info.ColumnNameToCache = null;
			info.ToolTip = null;
			if (null != _view)
				_view.Column_Update(tag, info.ColumnNameToShow, info.ToolTip, info.State);
		}

		public void EhView_RangeFrom(int val)
		{
			SetDirty();
			this._plotRangeFrom = val;
		}

		public void EhView_RangeTo(int val)
		{
			SetDirty();
			this._plotRangeTo = val;
		}

		#endregion ILineScatterPlotDataController Members

		#region AvailableDataColumns drag handler

		private bool EhAvailableDataColumns_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		private StartDragData EhAvailableDataColumns_StartDrag(IEnumerable items)
		{
			var node = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = node.Tag,
				CanCopy = true,
				CanMove = false
			};
		}

		private void EhAvailableDataColumns_DragEnded(bool isCopy, bool isMove)
		{
		}

		private void EhAvailableDataColumns_DragCancelled()

		{
		}

		#endregion AvailableDataColumns drag handler

		#region OtherAvailableItems drag handler

		private bool EhOtherAvailableItems_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		private StartDragData EhOtherAvailableItems_StartDrag(IEnumerable items)
		{
			var node = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = node.Tag,
				CanCopy = true,
				CanMove = false
			};
		}

		private void EhOtherAvailableItems_DragEnded(bool isCopy, bool isMove)
		{
		}

		private void EhOtherAvailableItems_DragCancelled()

		{
		}

		#endregion OtherAvailableItems drag handler

		#region ColumnDrop hander

		/// <summary>
		///
		/// </summary>
		/// <param name="data">The data to accept.</param>
		/// <param name="nonGuiTargetItem">Object that can identify the drop target, for instance a non gui tree node or list node, or a tag.</param>
		/// <param name="insertPosition">The insert position.</param>
		/// <param name="isCtrlKeyPressed">if set to <c>true</c> [is control key pressed].</param>
		/// <param name="isShiftKeyPressed">if set to <c>true</c> [is shift key pressed].</param>
		/// <returns></returns>
		public DropCanAcceptDataReturnData EhColumnDropCanAcceptData(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			// investigate data

			return new DropCanAcceptDataReturnData
			{
				CanCopy = true,
				CanMove = true,
				ItemIsSwallowingData = false
			};
		}

		public DropReturnData EhColumnDrop(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed)
		{
			var tag = nonGuiTargetItem as ColumnTag;
			if (null == tag)
				return new DropReturnData { IsCopy = false, IsMove = false };

			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

			if (data is Type)
			{
				try
				{
					var col = (IReadableColumn)System.Activator.CreateInstance((Type)data);
					info.Column = col;
					info.ColumnNameToShow = null;
					info.UpdateTooltipAndState(_doc.DataTable);
					if (null != _view)
						_view.Column_Update(tag, info.ColumnNameToShow, info.ToolTip, info.State);
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox("This column could not be dropped, message: " + ex.ToString(), "Error");
				}
			}
			else if (data is DataColumn)
			{
				info.Column = (DataColumn)data;
				info.UpdateColumnNameToCache();
				info.UpdateTooltipAndState(_doc.DataTable);
				if (null != _view)
					_view.Column_Update(tag, info.ColumnNameToShow, info.ToolTip, info.State);
			}

			return new DropReturnData
			{
				IsCopy = true,
				IsMove = false
			};
		}

		#endregion ColumnDrop hander
	}
}