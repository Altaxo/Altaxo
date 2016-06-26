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
		/// <summary>
		/// Initialize the list of available tables.
		/// </summary>
		/// <param name="items">The items.</param>
		void AvailableTables_Initialize(SelectableListNodeList items);

		/// <summary>
		/// Initialize the list of available data columns in the selected table and for the selected group number.
		/// </summary>
		/// <param name="items">The items.</param>
		void AvailableTableColumns_Initialize(SelectableListNodeList items);

		/// <summary>
		/// Initialize the list of other available columns.
		/// </summary>
		/// <param name="items">The items.</param>
		void OtherAvailableColumns_Initialize(SelectableListNodeList items);

		/// <summary>
		/// Initialize the list of columns needed by the plot item. This is organized into groups, each group corresponding to
		/// one plot style that needs data columns.
		/// </summary>
		/// <param name="groups">The groups.</param>
		void PlotItemColumns_Initialize(
			IEnumerable<Tuple< // list of all groups
			string, // Caption for each group of columns
			IEnumerable<Tuple< // list of column definitions
				ColumnTag, // tag to identify the column and group
				string> // Label of the column
			>>> groups);

		/// <summary>
		/// Updates the information for one plot item column
		/// </summary>
		/// <param name="tag">The tag that identifies the plot item column by the group number and column number.</param>
		/// <param name="colname">The name of the column as it will be shown in the text box.</param>
		/// <param name="toolTip">The tool tip for the text box.</param>
		/// <param name="transformationText">The text for the transformation box.</param>
		/// <param name="transformationToolTip">The tooltip for the transformation box.</param>
		/// <param name="state">The state of the column, as indicated by different background colors of the text box.</param>
		void PlotItemColumn_Update(ColumnTag tag, string colname, string toolTip, string transformationText, string transformationToolTip, ColumnControlState state);

		/// <summary>
		/// Shows a popup menu for the column corresponding to <paramref name="tag"/>, questioning whether to add the
		/// selected transformation as single transformation, as prepending transformation, or as appending transformation.
		/// </summary>
		/// <param name="tag">The tag.</param>
		void ShowTransformationSinglePrependAppendPopup(ColumnTag tag);

		void GroupNumber_Initialize(int groupNumber, bool isEnabled);

		void PlotRangeFrom_Initialize(int from);

		void PlotRangeTo_Initialize(int to);

		event Action SelectedTableChanged;

		event Action<int> SelectedGroupNumberChanged;

		event Action<ColumnTag> PlotItemColumn_AddTo;

		event Action<ColumnTag> OtherAvailableColumn_AddTo;

		event Action<ColumnTag> Transformation_AddTo;

		event Action<ColumnTag> Transformation_AddAsSingle;

		event Action<ColumnTag> Transformation_AddAsPrepending;

		event Action<ColumnTag> Transformation_AddAsAppending;

		event Action<ColumnTag> PlotItemColumn_Erase;

		event Action<ColumnTag> Transformation_Erase;

		event Action<int> RangeFromChanged;

		event Action<int> RangeToChanged;

		event CanStartDragDelegate AvailableTableColumns_CanStartDrag;

		event StartDragDelegate AvailableTableColumns_StartDrag;

		event DragEndedDelegate AvailableTableColumns_DragEnded;

		event DragCancelledDelegate AvailableTableColumns_DragCancelled;

		event CanStartDragDelegate OtherAvailableItems_CanStartDrag;

		event StartDragDelegate OtherAvailableItems_StartDrag;

		event DragEndedDelegate OtherAvailableItems_DragEnded;

		event DragCancelledDelegate OtherAvailableItems_DragCancelled;

		event DropCanAcceptDataDelegate PlotItemColumn_DropCanAcceptData;

		event DropDelegate PlotItemColumn_Drop;

		void AvailableTransformations_Initialize(SelectableListNodeList items);

		event CanStartDragDelegate AvailableTransformations_CanStartDrag;

		event StartDragDelegate AvailableTransformations_StartDrag;

		event DragEndedDelegate AvailableTransformations_DragEnded;

		event DragCancelledDelegate AvailableTransformations_DragCancelled;
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
		void SetAdditionalPlotItemColumns(
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
		/// <summary>
		/// Information about one plot item column.
		/// </summary>
		private class ColumnInfo
		{
			/// <summary>Label that will be shown to indicate the column's function, e.g. "X" for an x-colum.</summary>
			public string Label;

			private IReadableColumn _column;

			/// <summary>The column itself.</summary>
			public IReadableColumn Column
			{
				get { return _column; }
				set
				{
					if (value is ITransformedReadableColumn)
						_transformation = (value as ITransformedReadableColumn).Transformation;
					_column = value;
				}
			}

			public IReadableColumn UnderlyingColumn
			{
				get
				{
					if (_column is ITransformedReadableColumn)
						return (_column as ITransformedReadableColumn).OriginalReadableColumn;
					else
						return _column;
				}
				set
				{
					if (value is ITransformedReadableColumn)
						throw new ArgumentException("Nesting transformed columns is not allowed", nameof(value));

					if (null == value)
						_column = null; // but transformation is kept
					else if (null != _transformation)
						_column = new TransformedReadableColumn(value, _transformation);
					else
						_column = value;
				}
			}

			/// <summary>The column name as it will be shown in the text box.</summary>
			public string ColumnTextToShow;

			/// <summary>The column name as it was for the last data column. Will be used to choose a new data column if the table is changed by the user.</summary>
			public string ColumnNameToCache;

			/// <summary>The tooltip that will be shown when the user hovers over the column name text box.</summary>
			public string ColumnToolTip;

			private IVariantToVariantTransformation _transformation;

			/// <summary>
			/// The column transformation.
			/// </summary>
			public IVariantToVariantTransformation Transformation
			{
				get
				{
					return _transformation;
				}
				set
				{
					_transformation = value;

					if (null != value)
					{
						_column = new TransformedReadableColumn(UnderlyingColumn, _transformation);
					}
					else
					{
						_column = UnderlyingColumn;
					}
				}
			}

			/// <summary>This text will be shown in the transformation text box.</summary>
			public string TransformationTextToShow;

			/// <summary>The tooltip that will be shown when the user hovers over the transformation text box.</summary>
			public string TransformationToolTip;

			/// <summary>State of the column textbox. Depending on the state, the background of the textbox will assume different colors.</summary>
			public ColumnControlState State;

			/// <summary>Action to set the column property back in the style, if Apply of this controller is called.</summary>
			public Action<IReadableColumn> ColumnSetter;

			public void UpdateTooltipAndState(DataTable dataTableOfPlotItem)
			{
				if (null == Column)
				{
					if (string.IsNullOrEmpty(ColumnNameToCache))
					{
						ColumnTextToShow = string.Empty;
						ColumnToolTip = string.Empty;
						State = ColumnControlState.Normal;
					}
					else
					{
						ColumnTextToShow = ColumnNameToCache;
						ColumnToolTip = string.Format("Column {0} can not be found in this table with this group number", ColumnNameToCache);
						State = ColumnControlState.Error;
					}
					return;
				}

				var underlyingColumn = Column;
				IVariantToVariantTransformation transformation = null;
				if (Column is ITransformedReadableColumn)
				{
					underlyingColumn = ((ITransformedReadableColumn)Column).OriginalReadableColumn;
					transformation = ((ITransformedReadableColumn)Column).Transformation;
				}

				if (underlyingColumn is DataColumn)
				{
					var dcolumn = (DataColumn)underlyingColumn;
					var parentTable = DataTable.GetParentDataTableOf(dcolumn);
					var parentCollection = DataColumnCollection.GetParentDataColumnCollectionOf(dcolumn);
					if (null == parentTable)
					{
						ColumnToolTip = string.Format("This column is an orphaned data column without a parent data table", ColumnTextToShow);
						State = ColumnControlState.Error;
						if (parentCollection == null)
							ColumnTextToShow = string.Format("Orphaned {0}", dcolumn.GetType().Name);
						else
							ColumnTextToShow = ColumnNameToCache = parentCollection.GetColumnName(dcolumn);
					}
					else // UnderlyingColumn has a parent table
					{
						if (!object.ReferenceEquals(parentTable, dataTableOfPlotItem))
						{
							ColumnTextToShow = parentTable.DataColumns.GetColumnName(dcolumn);
							ColumnToolTip = string.Format("The column {0} is a data column with another parent data table: {1}", ColumnTextToShow, parentTable.Name);
							State = ColumnControlState.Warning;
						}
						else
						{
							ColumnTextToShow = ColumnNameToCache = parentTable.DataColumns.GetColumnName(dcolumn);
							ColumnToolTip = string.Format("UnderlyingColumn {0} of data table {1}", ColumnTextToShow, parentTable.Name);
							State = ColumnControlState.Normal;
						}
					}
				}
				else // UnderlyingColumn is something else
				{
					ColumnTextToShow = underlyingColumn.FullName;
					ColumnNameToCache = null;
					ColumnToolTip = string.Format("Independent data of type {0}: {1}", underlyingColumn.GetType(), underlyingColumn.ToString());
					State = ColumnControlState.Normal;
				}

				// now the transformation
				if (null != transformation)
				{
					TransformationTextToShow = transformation.RepresentationAsOperator ?? transformation.RepresentationAsFunction;
					TransformationToolTip = string.Format("Transforms the column data by the function f(x)={0}", transformation.RepresentationAsFunction);
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
		private SelectableListNodeList _availableTransformations = new SelectableListNodeList();
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

				_columnGroup[0].Columns.Add(new ColumnInfo() { Label = "X", Column = _doc.XColumn, ColumnTextToShow = _doc.XColumnName, ColumnNameToCache = _doc.XColumnName, ColumnSetter = (column) => _doc.XColumn = column });
				_columnGroup[0].Columns.Add(new ColumnInfo() { Label = "Y", Column = _doc.YColumn, ColumnTextToShow = _doc.YColumnName, ColumnNameToCache = _doc.YColumnName, ColumnSetter = (column) => _doc.YColumn = column });
				_columnGroup[0].Columns.Add(new ColumnInfo() { Label = "Z", Column = _doc.ZColumn, ColumnTextToShow = _doc.ZColumnName, ColumnNameToCache = _doc.ZColumnName, ColumnSetter = (column) => _doc.ZColumn = column });

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

				Controller_AvailableTransformations_Initialize();
			}

			if (null != _view)
			{
				_view.AvailableTables_Initialize(_tableItems);
				_view.GroupNumber_Initialize(_doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _doc.GroupNumber != _groupNumbersAll.Min));

				_view.AvailableTableColumns_Initialize(_columnItems);

				View_PlotItemColumns_Initialize();
				View_PlotItemColumns_UpdateAll();

				_view.PlotRangeFrom_Initialize(_plotRangeFrom);
				CalcMaxPossiblePlotRangeTo();

				_view.OtherAvailableColumns_Initialize(_otherAvailableColumns);

				View_AvailableTransformations_Initialize();
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

			_view.SelectedTableChanged += EhView_TableSelectionChanged;

			_view.PlotItemColumn_AddTo += EhView_ColumnAddTo;

			_view.PlotItemColumn_Erase += EhView_ColumnErase;

			_view.OtherAvailableColumn_AddTo += EhView_OtherAvailableColumnAddTo;

			_view.Transformation_AddTo += EhView_TransformationAddTo;

			_view.Transformation_AddAsSingle += EhView_TransformationAddAsSingle;

			_view.Transformation_AddAsPrepending += EhView_TransformationAddAsPrepending;

			_view.Transformation_AddAsAppending += EhView_TransformationAddAsAppending;

			_view.Transformation_Erase += EhView_TransformationErase;

			_view.RangeFromChanged += EhView_RangeFrom;

			_view.RangeToChanged += EhView_RangeTo;

			_view.SelectedGroupNumberChanged += EhGroupNumberChanged;

			_view.AvailableTableColumns_CanStartDrag += EhAvailableDataColumns_CanStartDrag;
			_view.AvailableTableColumns_StartDrag += EhAvailableDataColumns_StartDrag;
			_view.AvailableTableColumns_DragEnded += EhAvailableDataColumns_DragEnded;
			_view.AvailableTableColumns_DragCancelled += EhAvailableDataColumns_DragCancelled;

			_view.OtherAvailableItems_CanStartDrag += EhOtherAvailableItems_CanStartDrag;
			_view.OtherAvailableItems_StartDrag += EhOtherAvailableItems_StartDrag;
			_view.OtherAvailableItems_DragEnded += EhOtherAvailableItems_DragEnded;
			_view.OtherAvailableItems_DragCancelled += EhOtherAvailableItems_DragCancelled;

			_view.AvailableTransformations_CanStartDrag += EhAvailableTransformations_CanStartDrag;
			_view.AvailableTransformations_StartDrag += EhAvailableTransformations_StartDrag;
			_view.AvailableTransformations_DragEnded += EhAvailableTransformations_DragEnded;
			_view.AvailableTransformations_DragCancelled += EhAvailableTransformations_DragCancelled;

			_view.PlotItemColumn_DropCanAcceptData += EhColumnDropCanAcceptData;
			_view.PlotItemColumn_Drop += EhColumnDrop;
		}

		protected override void DetachView()
		{
			_view.SelectedTableChanged -= EhView_TableSelectionChanged;

			_view.PlotItemColumn_AddTo -= EhView_ColumnAddTo;

			_view.PlotItemColumn_Erase -= EhView_ColumnErase;

			_view.OtherAvailableColumn_AddTo -= EhView_OtherAvailableColumnAddTo;

			_view.Transformation_AddTo -= EhView_TransformationAddTo;

			_view.Transformation_AddAsSingle -= EhView_TransformationAddAsSingle;

			_view.Transformation_AddAsPrepending -= EhView_TransformationAddAsPrepending;

			_view.Transformation_AddAsAppending -= EhView_TransformationAddAsAppending;

			_view.Transformation_Erase -= EhView_TransformationErase;

			_view.RangeFromChanged -= EhView_RangeFrom;

			_view.RangeToChanged -= EhView_RangeTo;

			_view.SelectedGroupNumberChanged -= EhGroupNumberChanged;

			_view.AvailableTableColumns_CanStartDrag -= EhAvailableDataColumns_CanStartDrag;
			_view.AvailableTableColumns_StartDrag -= EhAvailableDataColumns_StartDrag;
			_view.AvailableTableColumns_DragEnded -= EhAvailableDataColumns_DragEnded;
			_view.AvailableTableColumns_DragCancelled -= EhAvailableDataColumns_DragCancelled;

			_view.OtherAvailableItems_CanStartDrag -= EhOtherAvailableItems_CanStartDrag;
			_view.OtherAvailableItems_StartDrag -= EhOtherAvailableItems_StartDrag;
			_view.OtherAvailableItems_DragEnded -= EhOtherAvailableItems_DragEnded;
			_view.OtherAvailableItems_DragCancelled -= EhOtherAvailableItems_DragCancelled;

			_view.AvailableTransformations_CanStartDrag -= EhAvailableTransformations_CanStartDrag;
			_view.AvailableTransformations_StartDrag -= EhAvailableTransformations_StartDrag;
			_view.AvailableTransformations_DragEnded -= EhAvailableTransformations_DragEnded;
			_view.AvailableTransformations_DragCancelled -= EhAvailableTransformations_DragCancelled;

			_view.PlotItemColumn_DropCanAcceptData -= EhColumnDropCanAcceptData;
			_view.PlotItemColumn_Drop -= EhColumnDrop;

			base.DetachView();
		}

		private IEnumerable<Tuple<
			string, // group name
		IEnumerable<Tuple< // list of column definitions
				ColumnTag, // tag to identify the column and group
				string>>>>
			GetEnumerationForAllGroupsOfColumns(List<GroupInfo> columnInfos)
		{
			for (int i = 0; i < columnInfos.Count; ++i)
			{
				var infoList = columnInfos[i];
				yield return new Tuple<string, IEnumerable<Tuple<ColumnTag, string>>>(
					infoList.GroupName,
					GetEnumerationForOneGroupOfColumns(infoList.Columns, i));
			}
		}

		private IEnumerable<Tuple< // list of column definitions
			ColumnTag, // tag to identify the column and group
			string // Label for the column,
			>>
		GetEnumerationForOneGroupOfColumns(List<ColumnInfo> columnInfos, int groupNumber)
		{
			for (int i = 0; i < columnInfos.Count; ++i)
			{
				var info = columnInfos[i];
				yield return new Tuple<ColumnTag, string>(
					new ColumnTag(groupNumber, i),
					info.Label

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
		public void SetAdditionalPlotItemColumns(IEnumerable<Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>>>> additionalColumns)
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
						ColumnTextToShow = col.Item3,
						ColumnSetter = col.Item4
					};

					columnInfo.UpdateTooltipAndState(_doc.DataTable);
					_columnGroup[groupNumber].Columns.Add(columnInfo);
				}
			}

			if (null != _view)
			{
				View_PlotItemColumns_Initialize();
				View_PlotItemColumns_UpdateAll();
			}
		}

		private void View_PlotItemColumns_Initialize()
		{
			_view.PlotItemColumns_Initialize(GetEnumerationForAllGroupsOfColumns(_columnGroup));
		}

		private void View_PlotItemColumns_UpdateAll()
		{
			for (int i = 0; i < _columnGroup.Count; ++i)
				for (int j = 0; j < _columnGroup[i].Columns.Count; j++)
				{
					var info = _columnGroup[i].Columns[j];
					_view.PlotItemColumn_Update(new ColumnTag(i, j), info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
				}
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

		private void Controller_AvailableTransformations_Initialize()
		{
			var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IVariantToVariantTransformation));

			foreach (var t in types)
			{
				if (t.IsNestedPrivate)
					continue; // types that are declared private will not be listed

				_availableTransformations.Add(new SelectableListNode(t.Name, t, false));
			}
		}

		private void View_AvailableTransformations_Initialize()
		{
			_view.AvailableTransformations_Initialize(_availableTransformations);
		}

		private void CalcMaxPossiblePlotRangeTo()
		{
			int len = int.MaxValue;
			for (int i = 0; i < 3; ++i)
			{
				if (_columnGroup[0].Columns[i].Column.Count.HasValue)
					len = Math.Min(len, _columnGroup[0].Columns[i].Column.Count.Value);
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

						if (info.UnderlyingColumn is DataColumn && !string.IsNullOrEmpty(info.ColumnTextToShow) && colDict.ContainsKey(info.ColumnTextToShow))
						{
							info.UnderlyingColumn = colDict[info.ColumnTextToShow];
						}

						info.UpdateTooltipAndState(_doc.DataTable);
						if (null != _view)
							_view.PlotItemColumn_Update(new ColumnTag(i, j), info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
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
				info.UnderlyingColumn = (DataColumn)node.Tag;
				info.UpdateTooltipAndState(_doc.DataTable);

				if (null != _view)
					_view.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
			}
		}

		public void EhView_ColumnErase(ColumnTag tag)
		{
			SetDirty();
			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
			info.UnderlyingColumn = null;
			info.ColumnTextToShow = null;
			info.ColumnNameToCache = null;
			info.ColumnToolTip = null;
			if (null != _view)
				_view.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
		}

		public void EhView_OtherAvailableColumnAddTo(ColumnTag tag)
		{
			var node = _otherAvailableColumns.FirstSelectedNode;
			if (null != node)
			{
				SetDirty();
				var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

				IReadableColumn createdObj = null;
				try
				{
					createdObj = (IReadableColumn)System.Activator.CreateInstance((Type)node.Tag);
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox("This column could not be created, message: " + ex.ToString(), "Error");
				}

				if (null != createdObj)
				{
					info.UnderlyingColumn = createdObj;
					info.UpdateTooltipAndState(_doc.DataTable);

					if (null != _view)
						_view.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
				}
			}
		}

		private void EhTransformation_Add(ColumnTag tag, Type transformationType)
		{
			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

			// make sure we can create that transformation
			IVariantToVariantTransformation createdTransformation = null;
			try
			{
				createdTransformation = (IVariantToVariantTransformation)System.Activator.CreateInstance(transformationType);
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("This column could not be created, message: " + ex.ToString(), "Error");
				return;
			}

			if (info.Transformation == null)
			{
				info.Transformation = createdTransformation;
				SetDirty();
				info.UpdateTooltipAndState(_doc.DataTable);
				_view?.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
			}
			else
			{
				_view?.ShowTransformationSinglePrependAppendPopup(tag); // this will eventually fire one of three commands to add as single, as prepend or as append transformation
			}
		}

		private void EhTransformation_AddMultiple(ColumnTag tag, Type transformationType, int multipleType)
		{
			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

			// make sure we can create that transformation
			IVariantToVariantTransformation createdTransformation = null;
			try
			{
				createdTransformation = (IVariantToVariantTransformation)System.Activator.CreateInstance(transformationType);
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("This column could not be created, message: " + ex.ToString(), "Error");
				return;
			}

			switch (multipleType)
			{
				case 0: // as single
					info.Transformation = createdTransformation;
					break;

				case 1: // prepend
					if (info.Transformation is Altaxo.Data.Transformations.CompoundTransformation)
						info.Transformation = (info.Transformation as Altaxo.Data.Transformations.CompoundTransformation).WithPrependedTransformation(createdTransformation);
					else if (info.Transformation != null)
						info.Transformation = new Altaxo.Data.Transformations.CompoundTransformation(new[] {  info.Transformation, createdTransformation });
					else
						info.Transformation = createdTransformation;
					break;

				case 2: // append
					if (info.Transformation is Altaxo.Data.Transformations.CompoundTransformation)
						info.Transformation = (info.Transformation as Altaxo.Data.Transformations.CompoundTransformation).WithAppendedTransformation(createdTransformation);
					else if (info.Transformation != null)
						info.Transformation = new Altaxo.Data.Transformations.CompoundTransformation(new[] {  createdTransformation, info.Transformation });
					else
						info.Transformation = createdTransformation;
					break;

				default:
					throw new NotImplementedException();
			}
			SetDirty();
			info.UpdateTooltipAndState(_doc.DataTable);
			_view?.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
		}

		public void EhView_TransformationAddTo(ColumnTag tag)
		{
			var node = _availableTransformations.FirstSelectedNode;
			if (null != node)
			{
				EhTransformation_Add(tag, (Type)node.Tag);
			}
		}

		public void EhView_TransformationAddAsSingle(ColumnTag tag)
		{
			var node = _availableTransformations.FirstSelectedNode;
			if (null != node)
			{
				EhTransformation_AddMultiple(tag, (Type)node.Tag, 0);
			}
		}

		public void EhView_TransformationAddAsPrepending(ColumnTag tag)
		{
			var node = _availableTransformations.FirstSelectedNode;
			if (null != node)
			{
				EhTransformation_AddMultiple(tag, (Type)node.Tag, 1);
			}
		}

		public void EhView_TransformationAddAsAppending(ColumnTag tag)
		{
			var node = _availableTransformations.FirstSelectedNode;
			if (null != node)
			{
				EhTransformation_AddMultiple(tag, (Type)node.Tag, 2);
			}
		}

		public void EhView_TransformationErase(ColumnTag tag)
		{
			SetDirty();
			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
			info.Transformation = null;
			info.TransformationTextToShow = null;
			info.TransformationToolTip = null;
			if (null != _view)
				_view.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
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

		#region AvailableTransformations drag handler

		private bool EhAvailableTransformations_CanStartDrag(IEnumerable items)
		{
			var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
			// to start a drag, at least one item must be selected
			return selNode != null;
		}

		private StartDragData EhAvailableTransformations_StartDrag(IEnumerable items)
		{
			var node = items.OfType<SelectableListNode>().FirstOrDefault();

			return new StartDragData
			{
				Data = node.Tag,
				CanCopy = true,
				CanMove = true
			};
		}

		private void EhAvailableTransformations_DragEnded(bool isCopy, bool isMove)
		{
		}

		private void EhAvailableTransformations_DragCancelled()

		{
		}

		#endregion AvailableTransformations drag handler

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

			_isDirty = true;

			var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

			if (data is Type)
			{
				object createdObj = null;
				try
				{
					createdObj = System.Activator.CreateInstance((Type)data);
				}
				catch (Exception ex)
				{
					Current.Gui.ErrorMessageBox("This object could not be dropped, message: " + ex.ToString(), "Error");
				}

				if (createdObj is IReadableColumn)
				{
					info.UnderlyingColumn = (IReadableColumn)createdObj;
					info.ColumnTextToShow = null;
				}
				else if (createdObj is IVariantToVariantTransformation)
				{
					EhTransformation_Add(tag, (Type)data);
				}

				info.UpdateTooltipAndState(_doc.DataTable);
				if (null != _view)
					_view.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
			}
			else if (data is DataColumn)
			{
				info.UnderlyingColumn = (DataColumn)data;
				info.UpdateTooltipAndState(_doc.DataTable);
				if (null != _view)
					_view.PlotItemColumn_Update(tag, info.ColumnTextToShow, info.ColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.State);
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