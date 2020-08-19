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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Data.Selections;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Plot.Data
{
  #region Interfaces

  public interface IColumnPlotDataView
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
    void AvailableTableColumns_Initialize(NGTreeNodeCollection items);

    object AvailableTableColumns_SelectedItem { get; }

    /// <summary>
    /// Initialize the list of other available columns.
    /// </summary>
    /// <param name="items">The items.</param>
    void OtherAvailableColumns_Initialize(SelectableListNodeList items);

    /// <summary>
    /// Initialize the list of tables that fit to the current chosen columns.
    /// </summary>
    /// <param name="items">The items.</param>
    void MatchingTables_Initialize(SelectableListNodeList items);

    /// <summary>
    /// Sets the row selection GUI control.
    /// </summary>
    /// <value>
    /// The row selection GUI control.
    /// </value>
    object RowSelectionGuiControl { set; }

    /// <summary>
    /// Initialize the list of columns needed by the plot item. This is organized into groups, each group corresponding to
    /// one plot style that needs data columns.
    /// </summary>
    /// <param name="groups">The groups.</param>
    void PlotColumns_Initialize(IEnumerable<(
      string GroupName, // group name
    IEnumerable<( // list of column definitions
        PlotColumnTag PlotColumnTag, // tag to identify the column and group
        string ColumnLabel)>)> groups);

    /// <summary>
    /// Updates the information for one plot item column
    /// </summary>
    /// <param name="tag">The tag that identifies the plot item column by the group number and column number.</param>
    /// <param name="colname">The name of the column as it will be shown in the text box.</param>
    /// <param name="toolTip">The tool tip for the text box.</param>
    /// <param name="transformationText">The text for the transformation box.</param>
    /// <param name="transformationToolTip">The tooltip for the transformation box.</param>
    /// <param name="state">The state of the column, as indicated by different background colors of the text box.</param>
    void PlotColumn_Update(PlotColumnTag tag, string colname, string toolTip, string transformationText, string transformationToolTip, PlotColumnControlState state);

    /// <summary>
    /// Shows a popup menu for the column corresponding to <paramref name="tag"/>, questioning whether to add the
    /// selected transformation as single transformation, as prepending transformation, or as appending transformation.
    /// </summary>
    /// <param name="tag">The tag.</param>
    void ShowTransformationSinglePrependAppendPopup(PlotColumnTag tag);

    void GroupNumber_Initialize(IEnumerable<int> availableGroupNumbers, int groupNumber, bool isEnabled);

    event Action SelectedTableChanged;

    event Action SelectedMatchingTableChanged;

    event Action<int> SelectedGroupNumberChanged;

    event Action<PlotColumnTag> PlotItemColumn_AddTo;

    event Action<PlotColumnTag> PlotItemColumn_Edit;

    event Action<PlotColumnTag> PlotItemColumn_Erase;

    event Action<PlotColumnTag> OtherAvailableColumn_AddTo;

    event Action<PlotColumnTag> Transformation_AddTo;

    event Action<PlotColumnTag> Transformation_AddAsSingle;

    event Action<PlotColumnTag> Transformation_AddAsPrepending;

    event Action<PlotColumnTag> Transformation_AddAsAppending;

    event Action<PlotColumnTag> Transformation_Edit;

    event Action<PlotColumnTag> Transformation_Erase;

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

  public interface IPlotColumnDataController : IMVCANController
  {
    /// <summary>
    /// Sets the additional columns that are used by some of the plot styles.
    /// </summary>
    /// <param name="additionalColumns">The additional columns. This is an enumerable of tuples, each tuple corresponding to one plot style.
    /// The first item of this tuple is the plot style's number and name. The second item is another enumeration of tuples.
    /// Each tuple in this second enumeration consist of
    /// (i) the label under which the column is announced in the view (first item),
    /// (ii) the column itself,
    /// (iii) the name of the column (only if it is a data column; otherwise empty)
    /// (iiii) an action to set the column if a value has been assigned to, or if the column was changed.
    /// </param>
    void SetAdditionalPlotColumns(
      IEnumerable<(string ColumnGroupNumberAndName, IEnumerable<(string ColumnLabel, IReadableColumn Column, string ColumnName, Action<IReadableColumn, DataTable, int> ColumnSetAction)> ColumnInfos)> additionalColumns
      );
  }

  #endregion Interfaces

  public abstract class ColumnPlotDataControllerBase<TModel>
    :
    MVCANControllerEditOriginalDocBase<TModel, IColumnPlotDataView>, IPlotColumnDataController where TModel : IColumnPlotData
  {
    #region Inner classes

    /// <summary>
    /// Information about one plot item column.
    /// </summary>
    protected class PlotColumnInformationInternal : PlotColumnInformation
    {
      /// <summary>Label that will be shown to indicate the column's function, e.g. "X" for an x-colum.</summary>
      public string Label { get; set; }

      /// <summary>Action to set the column property back in the style, if Apply of this controller is called.
      /// First argument is the column, second argument is the supposed parent data table, third the group number.</summary>
      public Action<IReadableColumn, DataTable, int> ColumnSetter { get; set; }

      public PlotColumnInformationInternal(IReadableColumn column, string nameOfUnderlyingDataColumn)
        : base(column, nameOfUnderlyingDataColumn)
      {
      }

      protected override void OnChanged()
      {
        ColumnSetter?.Invoke(Column, _supposedDataTable, _supposedGroupNumber);
      }
    }

    protected class GroupInfo
    {
      public string GroupName;
      public List<PlotColumnInformationInternal> Columns = new List<PlotColumnInformationInternal>();
    }

    protected class DataColumnSingleNode : NGTreeNode
    {
      private DataTable _table;
      private string _toolTip = null;

      public DataColumnSingleNode(DataTable table, DataColumn tag, bool isSelected)
        :
        base(table.DataColumns.GetColumnName(tag))
      {
        _tag = tag;
        _isSelected = isSelected;
        _table = table;
      }

      public string ToolTip
      {
        get
        {
          if (null == _toolTip)
          {
            _toolTip = string.Empty;
            Task.Factory.StartNew(() => EvaluateToolTip());
          }
          return _toolTip;
        }
      }

      private void EvaluateToolTip()
      {
        int idx = _table.DataColumns.GetColumnNumber((DataColumn)_tag);

        var stb = new System.Text.StringBuilder(64);

        stb.AppendFormat("col[\"{0}\"] at index {1}", _text, idx);

        int maxProp = Math.Min(8, _table.PropCols.ColumnCount);

        if (0 == maxProp)
        {
          stb.Append("\r\n(no property columns available)");
        }
        else
        {
          for (int i = 0; i < maxProp; ++i)
          {
            stb.AppendFormat("\r\nPropCol[\"{0}\"]={1}", _table.PropCols.GetColumnName(i), _table.PropCols[i][idx]);
          }
        }

        _toolTip = stb.ToString();
        OnPropertyChanged(nameof(ToolTip));
      }
    }

    protected class DataColumnBundleNode : NGTreeNode
    {
      public const int MaxNumberOfColumnsInOneNode = 200;

      private int _firstColumn;
      private int _columnCount;
      private DataTable _dataTable;
      private List<DataColumn> _columns;

      public DataColumnBundleNode(DataTable dataTable, List<DataColumn> columnList, int firstColumn, int columnCount)
        : base(true)
      {
        _dataTable = dataTable;
        _columns = columnList;
        _firstColumn = firstColumn;
        _columnCount = columnCount;
        Text = string.Format("Cols {0}-{1}", firstColumn, firstColumn + columnCount - 1);
      }

      protected override void LoadChildren()
      {
        var coll = _columns;
        Nodes.Clear();
        int nextColumn = Math.Min(_firstColumn + _columnCount, coll.Count);

        if (_columnCount <= MaxNumberOfColumnsInOneNode) // If number is low enough, expand to the data columns directly
        {
          for (int i = _firstColumn; i < nextColumn; ++i)
            Nodes.Add(new DataColumnSingleNode(_dataTable, _columns[i], false));
        }
        else // if the number of data columns is too high to be directly shown, we create intermediate nodes
        {
          // calculate the number of nodes to be shown
          int numNodes = (int)Math.Ceiling(_columnCount / (double)MaxNumberOfColumnsInOneNode);
          numNodes = Math.Min(MaxNumberOfColumnsInOneNode, numNodes);
          int colsInOneNode = MaxNumberOfColumnsInOneNode;
          for (; colsInOneNode * numNodes < _columnCount; colsInOneNode *= MaxNumberOfColumnsInOneNode)
            ; // Multiply with a multiple of MaxNumberOfColumnsInOneNode until it fits

          int first = _firstColumn;
          int remaining = nextColumn - _firstColumn;
          for (int i = 0; i < numNodes && remaining > 0; ++i)
          {
            Nodes.Add(new DataColumnBundleNode(_dataTable, coll, first, Math.Min(remaining, colsInOneNode)));
            remaining -= colsInOneNode;
            first += colsInOneNode;
          }
        }
      }
    }

    #endregion Inner classes

    #region Members

    private const int IndexGroupRowSelection = 0;
    private const int IndexGroupDataColumns = 1;
    private int IndexGroupOtherColumns = 2; // but can be a different number

    protected List<GroupInfo> _columnGroup;

    protected bool _isDirty = false;

    /// <summary>All datatables of the document</summary>
    protected SelectableListNodeList _availableTables = new SelectableListNodeList();

    /// <summary>All data columns in the selected data table and with the selected group number.</summary>
    protected NGTreeNode _availableDataColumns = new NGTreeNode();

    /// <summary>Other types of columns, e.g. constant columns, equally spaced columns and so on..</summary>
    protected SelectableListNodeList _otherAvailableColumns = new SelectableListNodeList();

    /// <summary>All types of available column transformations.</summary>
    protected SelectableListNodeList _availableTransformations = new SelectableListNodeList();

    /// <summary>ValueTuples from tables and group numbers, for which the columns in that group contain all that column names which are currently plot columns in our controller.</summary>
    protected SelectableListNodeList _matchingTables = new SelectableListNodeList();

    protected SortedSet<int> _groupNumbersAll;

    /// <summary>Tasks which updates the _fittingTables.</summary>
    protected Task _updateMatchingTablesTask;

    protected RowSelectionController _rowSelectionController;

    /// <summary>TokenSource to cancel the tasks which updates the _fittingTables.</summary>
    protected CancellationTokenSource _updateMatchingTablesTaskCancellationTokenSource = new CancellationTokenSource();

    #endregion Members

    #region Infrastructur Dispose and GetSubControllers

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rowSelectionController, () => _rowSelectionController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _columnGroup = null;
      _availableTables = null;
      _availableDataColumns = null;

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
        // Fix docs datatable

        var docDataTable = _doc.DataTable;
        var docGroupNumber = _doc.GroupNumber;

        var positionDataColumns = _doc.GetAdditionallyUsedColumns().ToArray();

        foreach (var groupEntry in positionDataColumns)
        {
          // find data table if not already known ...
          foreach (var entry in groupEntry.ColumnInfos)
          {
            if (docDataTable is null)
            {
              docDataTable = DataTable.GetParentDataTableOf(entry.Column as DataColumn);
              if (null != docDataTable && docDataTable.DataColumns.ContainsColumn((DataColumn)entry.Column))
                docGroupNumber = docDataTable.DataColumns.GetColumnGroup((DataColumn)entry.Column);
            }
          }
        }

        if (null != docDataTable)
        {
          _doc.DataTable = docDataTable;
          _doc.GroupNumber = docGroupNumber;
        }

        _rowSelectionController = new RowSelectionController
        {
          SupposedParentDataTable = _doc.DataTable
        };
        _rowSelectionController.InitializeDocument(_doc.DataRowSelection);
        _rowSelectionController.ItemsChanged += EhRowSelectionItemsChanged;
        Current.Gui.FindAndAttachControlTo(_rowSelectionController);

        // initialize group 0

        if (null == _columnGroup)
          _columnGroup = new List<GroupInfo>();

        if (_columnGroup.Count <= IndexGroupRowSelection)
          _columnGroup.Add(new GroupInfo() { GroupName = "#Plot range selection" });

        var grpInfo = _columnGroup[IndexGroupRowSelection];
        grpInfo.Columns.Clear();
        foreach (var col in _rowSelectionController.GetAdditionalColumns())
        {
          var columnInfo = new PlotColumnInformationInternal(col.Item2, col.Item3)
          {
            PlotColumnBoxStateIfColumnIsMissing = PlotColumnControlState.Error,
            Label = col.Item1,
            ColumnSetter = col.Item4
          };

          columnInfo.Update(_doc.DataTable, _doc.GroupNumber);
          grpInfo.Columns.Add(columnInfo);
        }

        IndexGroupOtherColumns = IndexGroupDataColumns;
        if (_columnGroup.Count <= IndexGroupDataColumns)
        {
          foreach (var groupEntry in positionDataColumns)
          {
            _columnGroup.Add(new GroupInfo { GroupName = groupEntry.NameOfColumnGroup });
            ++IndexGroupOtherColumns;
          }
        }

        for (int i = 0; i < positionDataColumns.Length; ++i)
        {
          grpInfo = _columnGroup[IndexGroupDataColumns + i];
          grpInfo.Columns.Clear();
          foreach (var entry in positionDataColumns[i].ColumnInfos)
          {
            grpInfo.Columns.Add(new PlotColumnInformationInternal(entry.Column, entry.ColumnName) { Label = entry.ColumnLabel, ColumnSetter = entry.SetColumnAction });
          }
          foreach (var entry in grpInfo.Columns)
          {
            entry.Update(_doc.DataTable, _doc.GroupNumber);
          }
        }

        // Initialize tables
        string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

        _availableTables.Clear();
        DataTable tg = _doc.DataTable;
        foreach (var tableName in tables)
        {
          _availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], tg != null && tg.Name == tableName));
        }

        // Group number
        _groupNumbersAll = null != tg ? tg.DataColumns.GetGroupNumbersAll() : new SortedSet<int>();

        // Initialize columns
        Controller_AvailableDataColumns_Initialize();

        // Initialize other available columns
        Controller_OtherAvailableColumns_Initialize();

        Controller_AvailableTransformations_Initialize();

        TriggerUpdateOfMatchingTables();
      }

      if (null != _view)
      {
        _view.AvailableTables_Initialize(_availableTables);
        _view.GroupNumber_Initialize(_groupNumbersAll, _doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _doc.GroupNumber != _groupNumbersAll.Min));

        _view.AvailableTableColumns_Initialize(_availableDataColumns.Nodes);

        View_PlotColumns_Initialize();
        View_PlotColumns_UpdateAll();

        _view.RowSelectionGuiControl = _rowSelectionController.ViewObject;

        _view.OtherAvailableColumns_Initialize(_otherAvailableColumns);

        _view.MatchingTables_Initialize(_matchingTables);

        View_AvailableTransformations_Initialize();
      }
    }

    private void EhRowSelectionItemsChanged()
    {
      var grpInfo = _columnGroup[IndexGroupRowSelection];
      grpInfo.Columns.Clear();

      foreach (var col in _rowSelectionController.GetAdditionalColumns())
      {
        var columnInfo = new PlotColumnInformationInternal(col.Item2, col.Item3)
        {
          PlotColumnBoxStateIfColumnIsMissing = PlotColumnControlState.Error,
          Label = col.Item1,
          ColumnSetter = col.Item4
        };

        columnInfo.Update(_doc.DataTable, _doc.GroupNumber);
        grpInfo.Columns.Add(columnInfo);
      }

      View_PlotColumns_Initialize();
      View_PlotColumns_UpdateAll();
    }

    public override bool Apply(bool disposeController)
    {
      if (_isDirty)
      {
        for (int i = 0; i < _columnGroup.Count; ++i)
          for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
            _columnGroup[i].Columns[j].ColumnSetter(_columnGroup[i].Columns[j].Column, _doc.DataTable, _doc.GroupNumber);
      }
      _isDirty = false;

      if (!_rowSelectionController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.DataRowSelection = (IRowSelection)(_rowSelectionController.ModelObject);

      // do not believe in the DataTable and Group number. Instead try to get DataTable and GroupNumber from the columns
      {
        IReadableColumnExtensions.GetCommonDataTableAndGroupNumberFromColumns(GetEnumerationOfAllColumns(), out var dataTableIsNotUniform, out var resultingTable, out var groupNumberIsNotUniform, out var resultingGroupNumber);

        if (null != resultingTable && !dataTableIsNotUniform)
          _doc.DataTable = resultingTable;
        if (null != resultingGroupNumber && !groupNumberIsNotUniform)
          _doc.GroupNumber = resultingGroupNumber.Value;
      }

      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Gets the enumeration of all columns that are controlled by this controller.
    /// </summary>
    /// <returns>Enumeration of all columns that are controlled by this controller.</returns>
    private IEnumerable<IReadableColumn> GetEnumerationOfAllColumns()
    {
      for (int i = 0; i < _columnGroup.Count; ++i)
      {
        for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
        {
          yield return _columnGroup[i].Columns[j].Column;
        }
      }
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.SelectedTableChanged += EhView_TableSelectionChanged;

      _view.SelectedMatchingTableChanged += EhView_MatchingTableSelectionChanged;

      _view.PlotItemColumn_AddTo += EhView_PlotColumnAddTo;

      _view.PlotItemColumn_Edit += EhView_PlotColumnEdit;

      _view.PlotItemColumn_Erase += EhView_PlotColumnErase;

      _view.OtherAvailableColumn_AddTo += EhView_OtherAvailableColumnAddTo;

      _view.Transformation_AddTo += EhView_TransformationAddTo;

      _view.Transformation_AddAsSingle += EhView_TransformationAddAsSingle;

      _view.Transformation_AddAsPrepending += EhView_TransformationAddAsPrepending;

      _view.Transformation_AddAsAppending += EhView_TransformationAddAsAppending;

      _view.Transformation_Edit += EhView_TransformationEdit;

      _view.Transformation_Erase += EhView_TransformationErase;

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

      _view.SelectedMatchingTableChanged -= EhView_MatchingTableSelectionChanged;

      _view.PlotItemColumn_AddTo -= EhView_PlotColumnAddTo;

      _view.PlotItemColumn_Edit -= EhView_PlotColumnEdit;

      _view.PlotItemColumn_Erase -= EhView_PlotColumnErase;

      _view.OtherAvailableColumn_AddTo -= EhView_OtherAvailableColumnAddTo;

      _view.Transformation_AddTo -= EhView_TransformationAddTo;

      _view.Transformation_AddAsSingle -= EhView_TransformationAddAsSingle;

      _view.Transformation_AddAsPrepending -= EhView_TransformationAddAsPrepending;

      _view.Transformation_AddAsAppending -= EhView_TransformationAddAsAppending;

      _view.Transformation_Edit -= EhView_TransformationEdit;

      _view.Transformation_Erase -= EhView_TransformationErase;

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

    #endregion Initialize, Apply, Attach, Detach

    #region PlotColumnInformation helper functions

    private IEnumerable<(
      string GroupName, // group name
    IEnumerable<( // list of column definitions
        PlotColumnTag PlotColumnTag, // tag to identify the column and group
        string ColumnLabel)>)>
      GetEnumerationForAllGroupsOfPlotColumns(List<GroupInfo> columnInfos)
    {
      for (int i = 0; i < columnInfos.Count; ++i)
      {
        var infoList = columnInfos[i];
        yield return (infoList.GroupName, GetEnumerationForOneGroupOfPlotColumns(infoList.Columns, i));
      }
    }

    private IEnumerable<( // list of column definitions
      PlotColumnTag PlotColumnTag, // tag to identify the column and group
      string ColumnLabel // Label for the column,
      )>
    GetEnumerationForOneGroupOfPlotColumns(List<PlotColumnInformationInternal> columnInfos, int groupNumber)
    {
      for (int i = 0; i < columnInfos.Count; ++i)
      {
        var info = columnInfos[i];
        yield return (new PlotColumnTag(groupNumber, i), info.Label);
      }
    }

    private void View_PlotColumns_Initialize()
    {
      _view.PlotColumns_Initialize(GetEnumerationForAllGroupsOfPlotColumns(_columnGroup));
    }

    private void View_PlotColumns_UpdateAll()
    {
      for (int i = 0; i < _columnGroup.Count; ++i)
        for (int j = 0; j < _columnGroup[i].Columns.Count; j++)
        {
          var info = _columnGroup[i].Columns[j];
          _view.PlotColumn_Update(new PlotColumnTag(i, j), info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
        }
    }

    #endregion PlotColumnInformation helper functions

    #region AvailableDataTables

    public void EhView_TableSelectionChanged()
    {
      var node = _availableTables.FirstSelectedNode;
      var tg = node?.Tag as DataTable;

      if (null == tg || object.ReferenceEquals(_doc.DataTable, tg))
        return;

      _doc.DataTable = tg;
      _groupNumbersAll = _doc.DataTable.DataColumns.GetGroupNumbersAll();

      // If data table has changed, try to choose a group number that matches as many as possible columns
      _doc.GroupNumber = ChooseBestMatchingGroupNumber(tg.DataColumns);
      if (_view != null)
        _view.GroupNumber_Initialize(_groupNumbersAll, _doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _groupNumbersAll.Min != _doc.GroupNumber));

      ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();

      TriggerUpdateOfMatchingTables(); // although nothing has changed in the column names, at least we get a new selection for the fitting combobox
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
          if (colDict.TryGetValue(col.Name, out var otherColumn))
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

    /// <summary>
    /// Try to replace the columns in ColumnInfo with that of the currently chosen table/group number. Additionally, the state of the columns is updated, and
    /// the changed infos are sent to the view.
    /// </summary>
    private void ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState()
    {
      // Initialize columns
      Controller_AvailableDataColumns_Initialize();
      _view?.AvailableTableColumns_Initialize(_availableDataColumns.Nodes);

      var dataTable = _doc.DataTable;
      if (null != dataTable)
      {
        // now try to exchange the data columns with columns from the new group
        var colDict = dataTable.DataColumns.GetNameDictionaryOfColumnsWithGroupNumber(_doc.GroupNumber);

        for (int i = 0; i < _columnGroup.Count; ++i)
        {
          for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
          {
            var info = _columnGroup[i].Columns[j];

            if (!string.IsNullOrEmpty(info.NameOfDataColumn) && colDict.ContainsKey(info.NameOfDataColumn))
            {
              info.UnderlyingColumn = colDict[info.NameOfDataColumn];
            }

            info.Update(_doc.DataTable, _doc.GroupNumber, true);
            _view?.PlotColumn_Update(new PlotColumnTag(i, j), info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
          }
        }
      }
    }

    #endregion AvailableDataTables

    #region AvailableDataColumns

    private void Controller_AvailableDataColumns_Initialize()
    {
      _availableDataColumns.Nodes.Clear();
      var dataTable = _doc.DataTable;
      if (null == dataTable)
        return;

      var columns = dataTable.DataColumns.GetListOfColumnsWithGroupNumber(_doc.GroupNumber);
      if (columns.Count <= DataColumnBundleNode.MaxNumberOfColumnsInOneNode)
      {
        for (int i = 0; i < columns.Count; ++i)
        {
          var col = columns[i];
          var node = new DataColumnSingleNode(dataTable, columns[i], false);
          _availableDataColumns.Nodes.Add(node);
        }
      }
      else // Create a tree of nodes
      {
        int levels = (int)(Math.Floor(Math.Log(columns.Count, DataColumnBundleNode.MaxNumberOfColumnsInOneNode)));
        int numberOfColumnsInRootLevel = (int)Calc.RMath.Pow(DataColumnBundleNode.MaxNumberOfColumnsInOneNode, levels);
        for (int i = 0; i < columns.Count; i += numberOfColumnsInRootLevel)
        {
          var node = new DataColumnBundleNode(dataTable, columns, i, Math.Min(numberOfColumnsInRootLevel, columns.Count - i));
          _availableDataColumns.Nodes.Add(node);
        }
      }
    }

    #endregion AvailableDataColumns

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

      var tag = ((DataTable, int))node.Tag;

      if (object.ReferenceEquals(_doc.DataTable, tag.Item1) && _doc.GroupNumber == tag.Item2) // then nothing will change
        return;

      _doc.DataTable = tag.Item1;
      _doc.GroupNumber = tag.Item2;
      ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();

      _availableTables.SetSelection((nd) => object.ReferenceEquals(nd.Tag, _doc.DataTable));
      _view?.AvailableTables_Initialize(_availableTables);
      _groupNumbersAll = _doc.DataTable.DataColumns.GetGroupNumbersAll();
      _view?.GroupNumber_Initialize(_groupNumbersAll, _doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _groupNumbersAll.Min != _doc.GroupNumber));
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

    private void UpdateMatchingTables(System.Threading.CancellationToken token)
    {
      var fittingTables2 = new SelectableListNodeList(); // we always update a new list, because _fittingTable1 is bound to the UI

      foreach (var entry in GetTablesWithGroupThatFitExistingPlotColumns(token))
      {
        fittingTables2.Add(new SelectableListNode(
          entry.dataTable.Name + " (Group " + entry.groupNumber.ToString() + ")",
          entry,
          object.ReferenceEquals(entry.dataTable, _doc.DataTable)));
      }

      _matchingTables = fittingTables2;
      Current.Dispatcher.InvokeAndForget(() => _view?.MatchingTables_Initialize(_matchingTables));
    }

    private IEnumerable<(DataTable dataTable, int groupNumber)> GetTablesWithGroupThatFitExistingPlotColumns(System.Threading.CancellationToken token)
    {
      var columnNamesThatMustFit = new HashSet<string>();

      // at first we build a list of column names that we need to fit
      for (int i = 0; i < _columnGroup.Count; ++i)
      {
        for (int j = 0; j < _columnGroup[i].Columns.Count; ++j)
        {
          var info = _columnGroup[i].Columns[j];

          if (!string.IsNullOrEmpty(info.NameOfDataColumn))
          {
            columnNamesThatMustFit.Add(info.NameOfDataColumn);
          }
        }
      }

      return ColumnPlotDataExchangeTableController.GetTablesWithGroupThatFitExistingPlotColumns(columnNamesThatMustFit, token);
    }

    #endregion MatchingDataTables

    #region Group Number

    private void EhGroupNumberChanged(int groupNumber)
    {
      if (groupNumber != _doc.GroupNumber)
      {
        _doc.GroupNumber = groupNumber;
        ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();
      }
    }

    #endregion Group Number

    #region PlotColumns

    public void EhView_PlotColumnAddTo(PlotColumnTag tag)
    {
      var node = _view?.AvailableTableColumns_SelectedItem as NGTreeNode;
      if (null != node)
      {
        SetDirty();
        var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
        info.UnderlyingColumn = (DataColumn)node.Tag;
        info.Update(_doc.DataTable, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
        TriggerUpdateOfMatchingTables();
      }
    }

    public void EhView_PlotColumnEdit(PlotColumnTag tag)
    {
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

      var editedColumn = EditOtherAvailableColumn(info.UnderlyingColumn, out var wasEdited);

      if (wasEdited)
      {
        SetDirty();
        info.UnderlyingColumn = editedColumn;
        info.Update(_doc.DataTable, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }
    }

    public void EhView_PlotColumnErase(PlotColumnTag tag)
    {
      SetDirty();
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
      info.UnderlyingColumn = null;
      info.Update(_doc.DataTable, _doc.GroupNumber);
      _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      TriggerUpdateOfMatchingTables();
    }

    #endregion PlotColumns

    #region OtherAvailableColumns

    private void Controller_OtherAvailableColumns_Initialize()
    {
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IReadableColumn));

      foreach (var t in types)
      {
        if (Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(t, typeof(DataColumn)))
          continue; // not the DataColumn types

        if (t.IsNestedPrivate)
          continue; // types that are declared private will not be listed

        if (!(true == t.GetConstructor(Type.EmptyTypes)?.IsPublic))
          continue; // don't has an empty public constructor

        _otherAvailableColumns.Add(new SelectableListNode(t.Name, t, false));
      }
    }

    /// <summary>
    /// Edits the other available column.
    /// </summary>
    /// <param name="newlyCreatedColumn">Instance of an OtherAvailableColumn.</param>
    /// <param name="wasEdited">If set to <c>true</c>, the column was edited.</param>
    /// <returns></returns>
    private static IReadableColumn EditOtherAvailableColumn(IReadableColumn newlyCreatedColumn, out bool wasEdited)
    {
      wasEdited = false;

      if (newlyCreatedColumn is Altaxo.Main.IImmutable)
      {
        object prop = newlyCreatedColumn.GetType().GetProperty("IsEditable")?.GetValue(newlyCreatedColumn, null);
        if ((prop is bool?) && true == (bool?)prop)
        {
          var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { newlyCreatedColumn }, typeof(IMVCANController));
          if (null != controller && null != controller.ViewObject)
          {
            if (Current.Gui.ShowDialog(controller, "Edit " + newlyCreatedColumn.GetType().Name))
            {
              newlyCreatedColumn = (IReadableColumn)controller.ModelObject;
              wasEdited = true;
            }
          }
        }
      }

      return newlyCreatedColumn;
    }

    public void EhView_OtherAvailableColumnAddTo(PlotColumnTag tag)
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
          info.UnderlyingColumn = EditOtherAvailableColumn(createdObj, out var wasEdited);
          info.Update(_doc.DataTable, _doc.GroupNumber);
          _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
        }
      }
    }

    #endregion OtherAvailableColumns

    #region Transformation

    private void Controller_AvailableTransformations_Initialize()
    {
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IVariantToVariantTransformation));

      foreach (var t in types)
      {
        if (t.IsNestedPrivate)
          continue; // types that are declared private will not be listed

        if (!(true == t.GetConstructor(Type.EmptyTypes)?.IsPublic))
          continue; // don't has an empty public constructor

        _availableTransformations.Add(new SelectableListNode(t.Name, t, false));
      }
    }

    private void View_AvailableTransformations_Initialize()
    {
      _view.AvailableTransformations_Initialize(_availableTransformations);
    }

    private static IVariantToVariantTransformation EditAvailableTransformation(IVariantToVariantTransformation createdTransformation, out bool wasEdited)
    {
      wasEdited = false;
      if (null != createdTransformation && createdTransformation.IsEditable)
      {
        var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { createdTransformation }, typeof(IMVCANController));
        if (null != controller && null != controller.ViewObject)
        {
          if (Current.Gui.ShowDialog(controller, "Edit " + createdTransformation.GetType().Name))
          {
            createdTransformation = (IVariantToVariantTransformation)controller.ModelObject;
            wasEdited = true;
          }
        }
      }
      return createdTransformation;
    }

    private void EhTransformation_AddMultiple(PlotColumnTag tag, Type transformationType, int multipleType)
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

      createdTransformation = EditAvailableTransformation(createdTransformation, out var wasEdited);

      switch (multipleType)
      {
        case 0: // as single
          info.Transformation = createdTransformation;
          break;

        case 1: // prepend
          if (info.Transformation is Altaxo.Data.Transformations.CompoundTransformation)
            info.Transformation = (info.Transformation as Altaxo.Data.Transformations.CompoundTransformation).WithPrependedTransformation(createdTransformation);
          else if (info.Transformation != null)
            info.Transformation = new Altaxo.Data.Transformations.CompoundTransformation(new[] { info.Transformation, createdTransformation });
          else
            info.Transformation = createdTransformation;
          break;

        case 2: // append
          if (info.Transformation is Altaxo.Data.Transformations.CompoundTransformation)
            info.Transformation = (info.Transformation as Altaxo.Data.Transformations.CompoundTransformation).WithAppendedTransformation(createdTransformation);
          else if (info.Transformation != null)
            info.Transformation = new Altaxo.Data.Transformations.CompoundTransformation(new[] { createdTransformation, info.Transformation });
          else
            info.Transformation = createdTransformation;
          break;

        default:
          throw new NotImplementedException();
      }
      SetDirty();
      info.Update(_doc.DataTable, _doc.GroupNumber);
      _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
    }

    public void EhView_TransformationAddTo(PlotColumnTag tag)
    {
      var node = _availableTransformations.FirstSelectedNode;
      if (null != node)
      {
        var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
        if (info.Transformation == null)
        {
          EhTransformation_AddMultiple(tag, (Type)node.Tag, 0);
        }
        else
        {
          _view?.ShowTransformationSinglePrependAppendPopup(tag); // this will eventually fire one of three commands to add as single, as prepend or as append transformation
        }
      }
    }

    public void EhView_TransformationAddAsSingle(PlotColumnTag tag)
    {
      var node = _availableTransformations.FirstSelectedNode;
      if (null != node)
      {
        EhTransformation_AddMultiple(tag, (Type)node.Tag, 0);
      }
    }

    public void EhView_TransformationAddAsPrepending(PlotColumnTag tag)
    {
      var node = _availableTransformations.FirstSelectedNode;
      if (null != node)
      {
        EhTransformation_AddMultiple(tag, (Type)node.Tag, 1);
      }
    }

    public void EhView_TransformationAddAsAppending(PlotColumnTag tag)
    {
      var node = _availableTransformations.FirstSelectedNode;
      if (null != node)
      {
        EhTransformation_AddMultiple(tag, (Type)node.Tag, 2);
      }
    }

    public void EhView_TransformationEdit(PlotColumnTag tag)
    {
      if (tag == null)
        return;

      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

      info.Transformation = EditAvailableTransformation(info.Transformation, out var wasEdited);
      if (wasEdited)
      {
        SetDirty();
        info.Update(_doc.DataTable, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }
    }

    public void EhView_TransformationErase(PlotColumnTag tag)
    {
      SetDirty();
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
      info.Transformation = null;
      info.Update(_doc.DataTable, _doc.GroupNumber);
      _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
    }

    #endregion Transformation

    #region AvailableDataColumns drag handler

    private bool EhAvailableDataColumns_CanStartDrag(IEnumerable items)
    {
      var selNode = items.OfType<NGTreeNode>().FirstOrDefault();
      // to start a drag, at least one item must be selected
      return selNode != null && (selNode.Tag is DataColumn);
    }

    private StartDragData EhAvailableDataColumns_StartDrag(IEnumerable items)
    {
      var node = items.OfType<NGTreeNode>().FirstOrDefault();

      if (node != null && node.Tag is DataColumn)
      {
        return new StartDragData
        {
          Data = node.Tag,
          CanCopy = true,
          CanMove = false
        };
      }
      else
      {
        return new StartDragData { Data = null, CanCopy = false, CanMove = false };
      }
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
      var tag = nonGuiTargetItem as PlotColumnTag;
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
          info.UnderlyingColumn = EditOtherAvailableColumn((IReadableColumn)createdObj, out var wasEdited);
          TriggerUpdateOfMatchingTables();
        }
        else if (createdObj is IVariantToVariantTransformation)
        {
          _availableTransformations.ClearSelectionsAll(); // we artificially select the node that holds that type
          var nodeToSelect = _availableTransformations.FirstOrDefault(node => (Type)node.Tag == (Type)data);
          if (null != nodeToSelect)
          {
            nodeToSelect.IsSelected = true;
            EhView_TransformationAddTo(tag);
          }
        }

        info.Update(_doc.DataTable, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }
      else if (data is DataColumn)
      {
        info.UnderlyingColumn = (DataColumn)data;
        info.Update(_doc.DataTable, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
        TriggerUpdateOfMatchingTables();
      }

      return new DropReturnData
      {
        IsCopy = true,
        IsMove = false
      };
    }

    #endregion ColumnDrop hander

    /// <summary>
    /// Sets the additional columns that are used by some of the plot styles.
    /// </summary>
    /// <param name="additionalColumns">The additional columns. This is an enumerable of tuples, each tuple corresponding to one plot style.
    /// The first item of this tuple is the plot style's number and name.
    /// The second item is another enumeration of tuples.
    /// Each tuple in this second enumeration consist of
    /// (i) the label under which the column is announced in the view (first item),
    /// (ii) the column itself,
    /// (iii) the name of the column (only if it is a data column; otherwise empty)
    /// (iiii) an action to set the column if a value has been assigned to, or if the column was changed
    /// can be used to get or set the underlying column.</param>
    public void SetAdditionalPlotColumns(IEnumerable<(string ColumnGroupNumberAndName, IEnumerable<(string ColumnLabel, IReadableColumn Column, string ColumnName, Action<IReadableColumn, DataTable, int> ColumnSetAction)> ColumnInfos)> additionalColumns)
    {
      int groupNumber = 1;
      foreach (var group in additionalColumns)
      {
        ++groupNumber;

        if (!(groupNumber < _columnGroup.Count))
        {
          _columnGroup.Add(new GroupInfo() { GroupName = group.ColumnGroupNumberAndName });
        }
        else
        {
          _columnGroup[groupNumber].GroupName = group.ColumnGroupNumberAndName;
          _columnGroup[groupNumber].Columns.Clear();
        }

        foreach (var col in group.ColumnInfos)
        {
          var columnInfo = new PlotColumnInformationInternal(col.Column, col.ColumnName)
          {
            Label = col.ColumnLabel,
            ColumnSetter = col.ColumnSetAction
          };

          columnInfo.Update(_doc.DataTable, _doc.GroupNumber);
          _columnGroup[groupNumber].Columns.Add(columnInfo);
        }
      }

      // Remove superfluous groups
      while (_columnGroup.Count > (groupNumber + 1))
      {
        _columnGroup.RemoveAt(_columnGroup.Count - 1);
      }

      // and finally update the view
      if (null != _view)
      {
        View_PlotColumns_Initialize();
        View_PlotColumns_UpdateAll();
      }
    }
  }
}
