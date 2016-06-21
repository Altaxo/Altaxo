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

	public interface IXYZColumnPlotDataView
	{
		void Tables_Initialize(SelectableListNodeList items);

		void Columns_Initialize(SelectableListNodeList items);

		void OtherAvailableColumns_Initialize(SelectableListNodeList items);

		void XColumn_Initialize(string colname, string toolTip, ColumnControlState state);

		void YColumn_Initialize(string colname, string toolTip, ColumnControlState state);

		void ZColumn_Initialize(string colname, string toolTip, ColumnControlState state);

		void GroupNumber_Initialize(int groupNumber, bool isEnabled);

		void PlotRangeFrom_Initialize(int from);

		void PlotRangeTo_Initialize(int to);

		event Action TableSelectionChanged;

		event Action Request_ToX;

		event Action Request_ToY;

		event Action Request_ToZ;

		event Action Request_EraseX;

		event Action Request_EraseY;

		event Action Request_EraseZ;

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

	#endregion Interfaces

	/// <summary>
	/// Summary description for LineScatterPlotDataController.
	/// </summary>
	[UserControllerForObject(typeof(XYZColumnPlotData))]
	[ExpectedTypeOfView(typeof(IXYZColumnPlotDataView))]
	public class XYZColumnPlotDataController
		:
		MVCANControllerEditOriginalDocBase<XYZColumnPlotData, IXYZColumnPlotDataView>
	{
		private struct ColumnInfo
		{
			public string Label;
			public Altaxo.Data.IReadableColumn Column;
			public string ColumnName;
			public string Tag;
		}

		private bool _isDirty = false;

		private int _plotRangeFrom;
		private int _plotRangeTo;

		private Altaxo.Data.IReadableColumn _xColumn;
		private string _xColumnName;

		private Altaxo.Data.IReadableColumn _yColumn;
		private string _yColumnName;

		private Altaxo.Data.IReadableColumn _zColumn;
		private string _zColumnName;

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
			_xColumn = null;
			_yColumn = null;
			_zColumn = null;

			_tableItems = null;
			_columnItems = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_xColumn = _doc.XColumn;
				_xColumnName = _doc.XColumnName ?? GetColumnNameToCache(_xColumn);
				_yColumn = _doc.YColumn;
				_yColumnName = _doc.YColumnName ?? GetColumnNameToCache(_yColumn);
				_zColumn = _doc.ZColumn;
				_zColumnName = _doc.ZColumnName ?? GetColumnNameToCache(_zColumn);
				_plotRangeFrom = _doc.PlotRangeStart;
				_plotRangeTo = _doc.PlotRangeLength == int.MaxValue ? int.MaxValue : _doc.PlotRangeStart + _doc.PlotRangeLength - 1;
				CalcMaxPossiblePlotRangeTo();

				// Initialize tables
				string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

				if (_doc.DataTable == null)
				{
					_doc.DataTable = DataTable.GetParentDataTableOf(_xColumn as DataColumn);
					if (null != _doc.DataTable && _doc.DataTable.DataColumns.ContainsColumn((DataColumn)_xColumn))
						_doc.GroupNumber = _doc.DataTable.DataColumns.GetColumnGroup((DataColumn)_xColumn);
				}
				if (_doc.DataTable == null)
				{
					_doc.DataTable = DataTable.GetParentDataTableOf(_yColumn as DataColumn);
					if (null != _doc.DataTable && _doc.DataTable.DataColumns.ContainsColumn((DataColumn)_yColumn))
						_doc.GroupNumber = _doc.DataTable.DataColumns.GetColumnGroup((DataColumn)_yColumn);
				}
				if (_doc.DataTable == null)
				{
					_doc.DataTable = DataTable.GetParentDataTableOf(_zColumn as DataColumn);
					if (null != _doc.DataTable && _doc.DataTable.DataColumns.ContainsColumn((DataColumn)_zColumn))
						_doc.GroupNumber = _doc.DataTable.DataColumns.GetColumnGroup((DataColumn)_zColumn);
				}

				DataTable tg = _doc.DataTable;

				_tableItems.Clear();
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

				ColumnInitialize(_view.XColumn_Initialize, _xColumn, _xColumnName);
				ColumnInitialize(_view.YColumn_Initialize, _yColumn, _yColumnName);
				ColumnInitialize(_view.ZColumn_Initialize, _zColumn, _zColumnName);

				_view.PlotRangeFrom_Initialize(_plotRangeFrom);
				CalcMaxPossiblePlotRangeTo();

				_view.OtherAvailableColumns_Initialize(_otherAvailableColumns);
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (_isDirty)
			{
				_doc.XColumn = _xColumn;
				_doc.YColumn = _yColumn;
				_doc.ZColumn = _zColumn;
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

			_view.Request_ToX += EhView_ToX;

			_view.Request_ToY += EhView_ToY;

			_view.Request_ToZ += EhView_ToZ;

			_view.Request_EraseX += EhView_EraseX;

			_view.Request_EraseY += EhView_EraseY;

			_view.Request_EraseZ += EhView_EraseZ;

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

			_view.Request_ToX -= EhView_ToX;

			_view.Request_ToY -= EhView_ToY;

			_view.Request_ToZ -= EhView_ToZ;

			_view.Request_EraseX -= EhView_EraseX;

			_view.Request_EraseY -= EhView_EraseY;

			_view.Request_EraseZ -= EhView_EraseZ;

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
			if (_xColumn is Altaxo.Data.IDefinedCount)
				len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_xColumn).Count);
			if (_yColumn is Altaxo.Data.IDefinedCount)
				len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_yColumn).Count);

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
			GroupNumberChanged(_doc.GroupNumber);
			if (_view != null)
				_view.GroupNumber_Initialize(_doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _groupNumbersAll.Min != _doc.GroupNumber));
		}

		/// <summary>
		/// Calculates the group number which best matches the already present x, y, z columns
		/// </summary>
		/// <param name="newDataColl">The new data coll.</param>
		/// <returns>The group number </returns>
		private int ChooseBestMatchingGroupNumber(DataColumnCollection newDataColl)
		{
			var matchList = new List<DataColumn>();

			if (_xColumn is DataColumn)
				matchList.Add((DataColumn)_xColumn);
			if (_yColumn is DataColumn)
				matchList.Add((DataColumn)_yColumn);
			if (_zColumn is DataColumn)
				matchList.Add((DataColumn)_zColumn);

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
				GroupNumberChanged(groupNumber);
		}

		private void GroupNumberChanged(int groupNumber)
		{
			_doc.GroupNumber = groupNumber;

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

				if (_xColumn is DataColumn && !string.IsNullOrEmpty(_xColumnName) && colDict.ContainsKey(_xColumnName))
					_xColumn = colDict[_xColumnName];

				if (_yColumn is DataColumn && !string.IsNullOrEmpty(_yColumnName) && colDict.ContainsKey(_yColumnName))
					_yColumn = colDict[_yColumnName];

				if (_zColumn is DataColumn && !string.IsNullOrEmpty(_zColumnName) && colDict.ContainsKey(_zColumnName))
					_zColumn = colDict[_zColumnName];
			}

			if (_view != null)
			{
				_view.Columns_Initialize(_columnItems);

				// hereby the status of the columns may change, too
				ColumnInitialize(_view.XColumn_Initialize, _xColumn, _xColumnName);
				ColumnInitialize(_view.YColumn_Initialize, _yColumn, _yColumnName);
				ColumnInitialize(_view.ZColumn_Initialize, _zColumn, _zColumnName);
			}
		}

		public void EhView_ToX()
		{
			var node = _columnItems.FirstSelectedNode;
			if (null != node)
			{
				SetDirty();
				_xColumn = (DataColumn)node.Tag;
				_xColumnName = GetColumnNameToCache(_xColumn);

				if (null != _view)
					ColumnInitialize(_view.XColumn_Initialize, _xColumn, _xColumnName);
			}
		}

		public void EhView_ToY()
		{
			var node = _columnItems.FirstSelectedNode;
			if (null != node)
			{
				SetDirty();
				_yColumn = (DataColumn)node.Tag;
				_yColumnName = GetColumnNameToCache(_yColumn);

				if (null != _view)
					ColumnInitialize(_view.YColumn_Initialize, _yColumn, _yColumnName);
			}
		}

		public void EhView_ToZ()
		{
			var node = _columnItems.FirstSelectedNode;
			if (null != node)
			{
				SetDirty();
				_zColumn = (DataColumn)node.Tag;
				_zColumnName = GetColumnNameToCache(_zColumn);

				if (null != _view)
					ColumnInitialize(_view.ZColumn_Initialize, _zColumn, _zColumnName);
			}
		}

		public void EhView_EraseX()
		{
			SetDirty();
			_xColumn = null;
			_xColumnName = null;
			if (null != _view)
				ColumnInitialize(_view.XColumn_Initialize, _xColumn, _xColumnName);
		}

		public void EhView_EraseY()
		{
			SetDirty();
			_yColumn = null;
			_yColumnName = null;
			if (null != _view)
				ColumnInitialize(_view.YColumn_Initialize, _yColumn, _yColumnName);
		}

		public void EhView_EraseZ()
		{
			SetDirty();
			_zColumn = null;
			_zColumnName = null;
			if (null != _view)
				ColumnInitialize(_view.ZColumn_Initialize, _zColumn, _zColumnName);
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
			if (data is Type)
			{
				try
				{
					var col = (IReadableColumn)System.Activator.CreateInstance((Type)data);
					_xColumn = col;
					_xColumnName = null;
					if (null != _view)
						ColumnInitialize(_view.XColumn_Initialize, _xColumn, _xColumnName);
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox("This column could not be dropped, message: " + ex.ToString(), "Error");
				}
			}
			else if (data is DataColumn)
			{
				_xColumn = (DataColumn)data;
				_xColumnName = GetColumnNameToCache(_xColumn);
				if (null != _view)
					ColumnInitialize(_view.XColumn_Initialize, _xColumn, _xColumnName);
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