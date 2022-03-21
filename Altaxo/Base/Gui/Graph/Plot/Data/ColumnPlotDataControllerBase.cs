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

#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Common;
using Altaxo.Gui.Data.Selections;

namespace Altaxo.Gui.Graph.Plot.Data
{
  public interface IColumnPlotDataView : IDataContextAwareView
  {
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

  public abstract class ColumnPlotDataControllerBase<TModel>
    :
    MVCANControllerEditOriginalDocBase<TModel, IColumnPlotDataView>, ISingleColumnControllerParent, IPlotColumnDataController where TModel : IColumnPlotData
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
          if (_toolTip is null)
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

    protected enum ColumnSourceElement
    {
      AvailableTableColumnsList,
      AvailableTableColumnsTree,
      OtherAvailableColumns,
      AvailableTransformations
    }

    #endregion Inner classes

    #region Members

    private const int IndexGroupRowSelection = 0;
    private const int IndexGroupDataColumns = 1;
    private int IndexGroupOtherColumns = 2; // but can be a different number

    protected List<GroupInfo> _columnGroup;

    protected bool _isDirty = false;


    /// <summary>
    /// Designates the latest focused element that can act as column source.
    /// </summary>
    ColumnSourceElement _lastActiveColumnsSource;





    /// <summary>Tasks which updates the _fittingTables.</summary>
    protected Task _updateMatchingTablesTask;



    /// <summary>TokenSource to cancel the tasks which updates the _fittingTables.</summary>
    protected CancellationTokenSource _updateMatchingTablesTaskCancellationTokenSource = new CancellationTokenSource();

    #endregion Members

    #region Infrastructur Dispose and GetSubControllers

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rowSelectionController, () => RowSelectionController = null);
    }

    public ColumnPlotDataControllerBase()
    {
      AvailableTransformationsDragHandler = new AvailableTransformationsDragHandlerImpl(this);
      AvailableDataColumnsDragHandler = new AvailableDataColumnsDragHandlerImpl(this);
      OtherAvailableColumnsDragHander = new OtherAvailableColumnsDragHandlerImpl(this);

    }

    public override void Dispose(bool isDisposing)
    {
      _columnGroup = null;
      _availableTables?.Dispose();
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

    #region Bindings

    private ItemsController<DataTable> _availableTables;

    public ItemsController<DataTable> AvailableTables
    {
      get => _availableTables;
      set
      {
        if (!(_availableTables == value))
        {
          _availableTables?.Dispose();
          _availableTables = value;
          OnPropertyChanged(nameof(AvailableTables));
        }
      }
    }

    private ItemsController<(DataTable table, int groupNumber)> _matchingTables;

    /// <summary>
    /// Initialize the list of tables that fit to the current chosen columns.
    /// ValueTuples from tables and group numbers, for which the columns in that group contain all that column names which are currently plot columns in our controller.
    /// </summary>
    public ItemsController<(DataTable table, int groupNumber)> MatchingTables
    {
      get => _matchingTables;
      set
      {
        if (!(_matchingTables == value))
        {
          _matchingTables?.Dispose();
          _matchingTables = value;
          OnPropertyChanged(nameof(MatchingTables));
        }
      }
    }

    private ObservableCollection<int> _availabeGroupNumbers;

    public ObservableCollection<int> AvailabeGroupNumbers
    {
      get => _availabeGroupNumbers;
      set
      {
        if (!(_availabeGroupNumbers == value))
        {
          _availabeGroupNumbers = value;
          OnPropertyChanged(nameof(AvailabeGroupNumbers));
        }
      }
    }

    private int _selectedGroupNumber;

    public int SelectedGroupNumber
    {
      get => _selectedGroupNumber;
      set
      {
        if (!(_selectedGroupNumber == value))
        {
          _selectedGroupNumber = value;
          OnPropertyChanged(nameof(SelectedGroupNumber));
          EhGroupNumberChanged(value);
        }
      }
    }
    private bool _isGroupNumberEnabled;

    public bool IsGroupNumberEnabled
    {
      get => _isGroupNumberEnabled;
      set
      {
        if (!(_isGroupNumberEnabled == value))
        {
          _isGroupNumberEnabled = value;
          OnPropertyChanged(nameof(IsGroupNumberEnabled));
        }
      }
    }

    private RowSelectionController _rowSelectionController;

    public RowSelectionController RowSelectionController
    {
      get => _rowSelectionController;
      set
      {
        if (!(_rowSelectionController == value))
        {
          if (_rowSelectionController is { } oldC)
            oldC.ItemsChanged -= EhRowSelectionItemsChanged;

          _rowSelectionController?.Dispose();
          _rowSelectionController = value;

          if (_rowSelectionController is { } newC)
            newC.ItemsChanged += EhRowSelectionItemsChanged;

          OnPropertyChanged(nameof(RowSelectionController));
        }
      }
    }

    /// <summary>All data columns in the selected data table and with the selected group number.</summary>
    protected NGTreeNode _availableDataColumns = new NGTreeNode();

    /// <summary>
    /// Initialize the list of available data columns in the selected table and for the selected group number.
    /// </summary>
    public NGTreeNodeCollection AvailableTableColumnsForListView => IsTableColumnsListVisible ? _availableDataColumns.Nodes : null;

    private NGTreeNode _availableTableColumnsListSelectedItem;

    public NGTreeNode AvailableTableColumnsListSelectedItem
    {
      get => _availableTableColumnsListSelectedItem;
      set
      {
        if (!(_availableTableColumnsListSelectedItem == value))
        {
          _availableTableColumnsListSelectedItem = value;
          OnPropertyChanged(nameof(AvailableTableColumnsListSelectedItem));
          if (value is not null)
          {
            _availableDataColumns.ClearSelectionRecursively();
            value.IsSelected = true;
          }
        }
      }
    }


    /// <summary>
    /// Initialize the list of available data columns in the selected table and for the selected group number.
    /// </summary>
    public NGTreeNodeCollection AvailableTableColumnsForTreeView => !IsTableColumnsListVisible ? _availableDataColumns.Nodes : null;

    /// <summary>
    /// Gets a value indicating whether the list view or tree view is visible. 
    /// </summary>
    /// <value>
    ///   <c>true</c> if the list view is visible; otherwise, the tree view is visible.
    /// </value>
    public bool IsTableColumnsListVisible
    {
      get
      {
        bool isTreeWithSubnodes = _availableDataColumns.Nodes.Count > 0 && _availableDataColumns.Nodes[0].HasChilds;
        return !isTreeWithSubnodes;
      }
    }

    private IMVVMDragHandler _availableDataColumnsDragHandler;

    public IMVVMDragHandler AvailableDataColumnsDragHandler
    {
      get => _availableDataColumnsDragHandler;
      set
      {
        if (!(_availableDataColumnsDragHandler == value))
        {
          _availableDataColumnsDragHandler = value;
          OnPropertyChanged(nameof(AvailableDataColumnsDragHandler));
        }
      }
    }


    private ItemsController<Type> _otherAvailableColumns;

    /// <summary>Other types of columns, e.g. constant columns, equally spaced columns and so on..</summary>
    public ItemsController<Type> OtherAvailableColumns
    {
      get => _otherAvailableColumns;
      set
      {
        if (!(_otherAvailableColumns == value))
        {
          _otherAvailableColumns?.Dispose();
          _otherAvailableColumns = value;
          OnPropertyChanged(nameof(OtherAvailableColumns));
        }
      }
    }

    private IMVVMDragHandler _otherAvailableColumnsDragHander;

    public IMVVMDragHandler OtherAvailableColumnsDragHander
    {
      get => _otherAvailableColumnsDragHander;
      set
      {
        if (!(_otherAvailableColumnsDragHander == value))
        {
          _otherAvailableColumnsDragHander = value;
          OnPropertyChanged(nameof(OtherAvailableColumnsDragHander));
        }
      }
    }



    private ItemsController<Type> _availableTransformations;

    /// <summary>All types of available column transformations.</summary>
    public ItemsController<Type> AvailableTransformations
    {
      get => _availableTransformations;
      set
      {
        if (!(_availableTransformations == value))
        {
          _availableTransformations?.Dispose();
          _availableTransformations = value;
          OnPropertyChanged(nameof(AvailableTransformations));
        }
      }
    }

    private bool _isAvailableTransformationsFocused;

    public bool IsAvailableTransformationsFocused
    {
      get => _isAvailableTransformationsFocused;
      set
      {
        if (!(_isAvailableTransformationsFocused == value))
        {
          _isAvailableTransformationsFocused = value;
          OnPropertyChanged(nameof(IsAvailableTransformationsFocused));
          if (value == true)
            _lastActiveColumnsSource = ColumnSourceElement.AvailableTransformations;
        }
      }
    }

    private bool _isOtherAvailableColumnsFocused;

    public bool IsOtherAvailableColumnsFocused
    {
      get => _isOtherAvailableColumnsFocused;
      set
      {
        if (!(_isOtherAvailableColumnsFocused == value))
        {
          _isOtherAvailableColumnsFocused = value;
          OnPropertyChanged(nameof(IsOtherAvailableColumnsFocused));
          if (value == true)
            _lastActiveColumnsSource = ColumnSourceElement.OtherAvailableColumns;
        }
      }
    }

    private bool _isAvailableDataColumnsListViewFocused;

    public bool IsAvailableDataColumnsListViewFocused
    {
      get => _isAvailableDataColumnsListViewFocused;
      set
      {
        if (!(_isAvailableDataColumnsListViewFocused == value))
        {
          _isAvailableDataColumnsListViewFocused = value;
          OnPropertyChanged(nameof(IsAvailableDataColumnsListViewFocused));
          if (value == true)
            _lastActiveColumnsSource = ColumnSourceElement.AvailableTableColumnsList;
        }
      }
    }

    private bool _isAvailableDataColumnsTreeViewFocused;

    public bool IsAvailableDataColumnsTreeViewFocused
    {
      get => _isAvailableDataColumnsTreeViewFocused;
      set
      {
        if (!(_isAvailableDataColumnsTreeViewFocused == value))
        {
          _isAvailableDataColumnsTreeViewFocused = value;
          OnPropertyChanged(nameof(IsAvailableDataColumnsTreeViewFocused));
          if (value == true)
            _lastActiveColumnsSource = ColumnSourceElement.AvailableTableColumnsTree;
        }
      }
    }



    private IMVVMDragHandler _availableTransformationsDragHandler;

    public IMVVMDragHandler AvailableTransformationsDragHandler
    {
      get => _availableTransformationsDragHandler;
      set
      {
        if (!(_availableTransformationsDragHandler == value))
        {
          _availableTransformationsDragHandler = value;
          OnPropertyChanged(nameof(AvailableTransformationsDragHandler));
        }
      }
    }



    private ObservableCollection<SingleColumnController> _plotItemColumns = new ObservableCollection<SingleColumnController>();

    public ObservableCollection<SingleColumnController> PlotItemColumns
    {
      get => _plotItemColumns;
      set
      {
        if (!(_plotItemColumns == value))
        {
          _plotItemColumns = value;
          OnPropertyChanged(nameof(PlotItemColumns));
        }
      }
    }

    #endregion

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
              if (docDataTable is not null && docDataTable.DataColumns.ContainsColumn((DataColumn)entry.Column))
                docGroupNumber = docDataTable.DataColumns.GetColumnGroup((DataColumn)entry.Column);
            }
          }
        }

        if (docDataTable is not null)
        {
          _doc.DataTable = docDataTable;
          _doc.GroupNumber = docGroupNumber;
        }

        var rowSelectionController = new RowSelectionController
        {
          SupposedParentDataTable = _doc.DataTable
        };
        rowSelectionController.InitializeDocument(_doc.DataRowSelection);
        Current.Gui.FindAndAttachControlTo(rowSelectionController);
        RowSelectionController = rowSelectionController;

        // initialize group 0

        if (_columnGroup is null)
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

        var availableTables = new SelectableListNodeList();
        DataTable tg = _doc.DataTable;
        foreach (var tableName in tables)
        {
          availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], tg is not null && tg.Name == tableName));
        }
        AvailableTables = new ItemsController<DataTable>(availableTables, EhView_TableSelectionChanged);

        // Group number
        AvailabeGroupNumbers = tg is not null ? new ObservableCollection<int>(tg.DataColumns.GetGroupNumbersAll()) : new ObservableCollection<int>();
        SelectedGroupNumber = _doc.GroupNumber;
        IsGroupNumberEnabled = AvailabeGroupNumbers.Count > 1 || (AvailabeGroupNumbers.Count == 1 && _doc.GroupNumber != AvailabeGroupNumbers.Min());

        // Initialize columns
        Controller_AvailableDataColumns_Initialize();

        // Initialize other available columns
        Controller_OtherAvailableColumns_Initialize();

        Controller_AvailableTransformations_Initialize();

        TriggerUpdateOfMatchingTables();





      }
      if (_view is not null)
      {
        View_PlotColumns_Initialize();
        View_PlotColumns_UpdateAll();
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

        if (resultingTable is not null && !dataTableIsNotUniform)
          _doc.DataTable = resultingTable;
        if (resultingGroupNumber is not null && !groupNumberIsNotUniform)
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
      var allItems = GetEnumerationForAllGroupsOfPlotColumns(_columnGroup);
      _plotItemColumns.Clear();
      foreach (var group in allItems)
      {
        foreach (var item in group.Item2)
        {
          _plotItemColumns.Add(new SingleColumnController() { Parent = this, GroupName = group.GroupName, LabelText = item.ColumnLabel, Tag = item.PlotColumnTag });
        }
      }
    }

    private void View_PlotColumns_UpdateAll()
    {
      for (int i = 0; i < _columnGroup.Count; ++i)
        for (int j = 0; j < _columnGroup[i].Columns.Count; j++)
        {
          var info = _columnGroup[i].Columns[j];
          var tag = new PlotColumnTag(i, j);
          var ctrl = _plotItemColumns.FirstOrDefault(x => x.Tag == tag);
          if (ctrl is not null)
          {
            ctrl.ColumnText = info.PlotColumnBoxText;
            ctrl.ColumnToolTip = info.PlotColumnToolTip;
            ctrl.TransformationText = info.TransformationTextToShow;
            ctrl.TransformationToolTip = info.TransformationToolTip;
            ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
          }
        }
    }

    #endregion PlotColumnInformation helper functions

    #region AvailableDataTables

    public void EhView_TableSelectionChanged(DataTable tg)
    {
      if (tg is null || object.ReferenceEquals(_doc.DataTable, tg))
        return;

      _doc.DataTable = tg;
      var availableGN = _doc.DataTable.DataColumns.GetGroupNumbersAll();
      AvailabeGroupNumbers = new ObservableCollection<int>(availableGN);
      // If data table has changed, try to choose a group number that matches as many as possible columns
      _doc.GroupNumber = ChooseBestMatchingGroupNumber(tg.DataColumns);
      SelectedGroupNumber = _doc.GroupNumber;
      IsGroupNumberEnabled = availableGN.Count > 1 || (availableGN.Count == 1 && availableGN.Min != _doc.GroupNumber);

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

      int bestGroupNumber = AvailabeGroupNumbers.Count > 0 ? AvailabeGroupNumbers.Min() : 0;
      int bestNumberOfPoints = 0;
      foreach (var groupNumber in AvailabeGroupNumbers)
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

      var dataTable = _doc.DataTable;
      if (dataTable is not null)
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

            var ctrl = _plotItemColumns.FirstOrDefault(x => x.Tag == new PlotColumnTag(i, j));
            if (ctrl is not null)
            {
              ctrl.ColumnText = info.PlotColumnBoxText;
              ctrl.ColumnToolTip = info.PlotColumnToolTip;
              ctrl.TransformationText = info.TransformationTextToShow;
              ctrl.TransformationToolTip = info.TransformationToolTip;
              ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
            }
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
      if (dataTable is null)
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
      OnPropertyChanged(nameof(IsTableColumnsListVisible));
      OnPropertyChanged(nameof(AvailableTableColumnsForListView));
      OnPropertyChanged(nameof(AvailableTableColumnsForTreeView));
    }

    #endregion AvailableDataColumns

    #region MatchingDataTables

    // Matching data tables are those tables, which have at least one group of columns which best fits the existing plot item column names

    /// <summary>
    /// Occurs if the selection for the matching tables has changed.
    /// </summary>
    public void EhView_MatchingTableSelectionChanged((DataTable table, int groupNumber) tag)
    {
      if (object.ReferenceEquals(_doc.DataTable, tag.table) && _doc.GroupNumber == tag.groupNumber) // then nothing will change
        return;

      _doc.DataTable = tag.table;
      _doc.GroupNumber = tag.groupNumber;
      ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();

      AvailableTables.SelectedValue = _doc.DataTable;
      var availableGN = _doc.DataTable.DataColumns.GetGroupNumbersAll();
      AvailabeGroupNumbers = new ObservableCollection<int>(availableGN);
      SelectedGroupNumber = _doc.GroupNumber;
      IsGroupNumberEnabled = availableGN.Count > 1 || (availableGN.Count == 1 && availableGN.Min != _doc.GroupNumber);
    }

    private void TriggerUpdateOfMatchingTables()
    {
      if (_updateMatchingTablesTask?.Status == TaskStatus.Running)
      {
        _updateMatchingTablesTaskCancellationTokenSource.Cancel();
        while (_updateMatchingTablesTask?.Status == TaskStatus.Running)
          System.Threading.Thread.Sleep(20);
      }

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

      MatchingTables = new ItemsController<(DataTable table, int groupNumber)>(fittingTables2, EhView_MatchingTableSelectionChanged);
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

    /// <summary>
    /// Called if a <see cref="SingleColumnController"/> notifies, that the 'add column' button was pressed.
    /// </summary>
    /// <param name="tag">The plot column tag.</param>
    public void EhPlotColumnAddTo(SingleColumnController ctrl)
    {
      switch (_lastActiveColumnsSource)
      {
        case ColumnSourceElement.AvailableTableColumnsList:
        case ColumnSourceElement.AvailableTableColumnsTree:
          EhView_PlotColumnAddTo(ctrl);
          break;
        case ColumnSourceElement.OtherAvailableColumns:
          EhView_OtherAvailableColumnAddTo(ctrl);
          break;
        case ColumnSourceElement.AvailableTransformations:
          EhView_TransformationAddTo(ctrl);
          break;
        default:
          break;
      }
    }

    public void EhView_PlotColumnAddTo(SingleColumnController ctrl)
    {
      var node = _availableDataColumns.FirstSelectedNode;
      if (node is not null)
      {
        SetDirty();
        var info = _columnGroup[ctrl.Tag.GroupNumber].Columns[ctrl.Tag.ColumnNumber];
        info.UnderlyingColumn = (DataColumn)node.Tag;
        info.Update(_doc.DataTable, _doc.GroupNumber);

        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
        TriggerUpdateOfMatchingTables();
      }
    }

    public void EhPlotColumnEdit(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

      var editedColumn = EditOtherAvailableColumn(info.UnderlyingColumn, out var wasEdited);

      if (wasEdited)
      {
        SetDirty();
        info.UnderlyingColumn = editedColumn;
        info.Update(_doc.DataTable, _doc.GroupNumber);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }
    }

    public void EhPlotColumnErase(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      SetDirty();
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
      info.UnderlyingColumn = null;
      info.Update(_doc.DataTable, _doc.GroupNumber);
      ctrl.ColumnText = info.PlotColumnBoxText;
      ctrl.ColumnToolTip = info.PlotColumnToolTip;
      ctrl.TransformationText = info.TransformationTextToShow;
      ctrl.TransformationToolTip = info.TransformationToolTip;
      ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      TriggerUpdateOfMatchingTables();
    }

    #endregion PlotColumns

    #region OtherAvailableColumns

    private void Controller_OtherAvailableColumns_Initialize()
    {
      var otherAvailableColumns = new SelectableListNodeList();
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IReadableColumn));

      foreach (var t in types)
      {
        if (Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(t, typeof(DataColumn)))
          continue; // not the DataColumn types

        if (t.IsNestedPrivate)
          continue; // types that are declared private will not be listed

        if (!(true == t.GetConstructor(Type.EmptyTypes)?.IsPublic))
          continue; // don't has an empty public constructor

        otherAvailableColumns.Add(new SelectableListNode(t.Name, t, false));
      }

      OtherAvailableColumns = new ItemsController<Type>(otherAvailableColumns);
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
          if (controller is not null && controller.ViewObject is not null)
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

    public void EhView_OtherAvailableColumnAddTo(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      var colType = _otherAvailableColumns.SelectedValue;
      if (colType is not null)
      {
        SetDirty();
        var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

        IReadableColumn createdObj = null;
        try
        {
          createdObj = (IReadableColumn)System.Activator.CreateInstance(colType);
        }
        catch (Exception ex)
        {
          Current.Gui.ErrorMessageBox("This column could not be created, message: " + ex.ToString(), "Error");
        }

        if (createdObj is not null)
        {
          info.UnderlyingColumn = EditOtherAvailableColumn(createdObj, out var wasEdited);
          info.Update(_doc.DataTable, _doc.GroupNumber);
          ctrl.ColumnText = info.PlotColumnBoxText;
          ctrl.ColumnToolTip = info.PlotColumnToolTip;
          ctrl.TransformationText = info.TransformationTextToShow;
          ctrl.TransformationToolTip = info.TransformationToolTip;
          ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
        }
      }
    }

    #endregion OtherAvailableColumns

    #region Transformation

    private void Controller_AvailableTransformations_Initialize()
    {
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IVariantToVariantTransformation));
      var availableTransformations = new SelectableListNodeList();
      foreach (var t in types)
      {
        if (t.IsNestedPrivate)
          continue; // types that are declared private will not be listed

        if (!(true == t.GetConstructor(Type.EmptyTypes)?.IsPublic))
          continue; // don't has an empty public constructor

        availableTransformations.Add(new SelectableListNode(t.Name, t, false));
      }
      AvailableTransformations = new ItemsController<Type>(availableTransformations);
    }

    private static IVariantToVariantTransformation EditAvailableTransformation(IVariantToVariantTransformation createdTransformation, out bool wasEdited)
    {
      wasEdited = false;
      if (createdTransformation is not null && createdTransformation.IsEditable)
      {
        var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { createdTransformation }, typeof(IMVCANController));
        if (controller is not null && controller.ViewObject is not null)
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

    private void EhTransformation_AddMultiple(SingleColumnController ctrl, Type transformationType, int multipleType)
    {
      var tag = ctrl.Tag;
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
          else if (info.Transformation is not null)
            info.Transformation = new Altaxo.Data.Transformations.CompoundTransformation(new[] { info.Transformation, createdTransformation });
          else
            info.Transformation = createdTransformation;
          break;

        case 2: // append
          if (info.Transformation is Altaxo.Data.Transformations.CompoundTransformation)
            info.Transformation = (info.Transformation as Altaxo.Data.Transformations.CompoundTransformation).WithAppendedTransformation(createdTransformation);
          else if (info.Transformation is not null)
            info.Transformation = new Altaxo.Data.Transformations.CompoundTransformation(new[] { createdTransformation, info.Transformation });
          else
            info.Transformation = createdTransformation;
          break;

        default:
          throw new NotImplementedException();
      }
      SetDirty();
      info.Update(_doc.DataTable, _doc.GroupNumber);
      ctrl.ColumnText = info.PlotColumnBoxText;
      ctrl.ColumnToolTip = info.PlotColumnToolTip;
      ctrl.TransformationText = info.TransformationTextToShow;
      ctrl.TransformationToolTip = info.TransformationToolTip;
      ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
    }

    public void EhView_TransformationAddTo(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      var transfoType = AvailableTransformations.SelectedValue;
      if (transfoType is not null)
      {
        var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
        if (info.Transformation is null)
        {
          EhTransformation_AddMultiple(ctrl, transfoType, 0);
        }
        else
        {
          ctrl.IsTransformationPopupOpen = true; // this will eventually fire one of three commands to add as single, as prepend or as append transformation
        }
      }
    }

    public void EhPlotColumnTransformationAddAsSingle(SingleColumnController ctrl)
    {
      var transfoType = AvailableTransformations.SelectedValue;
      if (transfoType is not null)
      {
        EhTransformation_AddMultiple(ctrl, transfoType, 0);
      }
    }

    public void EhPlotColumnTransformationAddAsPrepending(SingleColumnController ctrl)
    {
      var transfoType = _availableTransformations.SelectedValue;
      if (transfoType is not null)
      {
        EhTransformation_AddMultiple(ctrl, transfoType, 1);
      }
    }

    public void EhPlotColumnTransformationAddAsAppending(SingleColumnController ctrl)
    {
      var transfoType = _availableTransformations.SelectedValue;
      if (transfoType is not null)
      {
        EhTransformation_AddMultiple(ctrl, transfoType, 2);
      }
    }

    public void EhPlotColumnTransformationEdit(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];

      info.Transformation = EditAvailableTransformation(info.Transformation, out var wasEdited);
      if (wasEdited)
      {
        SetDirty();
        info.Update(_doc.DataTable, _doc.GroupNumber);
        info.Update(_doc.DataTable, _doc.GroupNumber);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }
    }

    public void EhPlotColumnTransformationErase(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      SetDirty();
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
      info.Transformation = null;
      info.Update(_doc.DataTable, _doc.GroupNumber);
      ctrl.ColumnText = info.PlotColumnBoxText;
      ctrl.ColumnToolTip = info.PlotColumnToolTip;
      ctrl.TransformationText = info.TransformationTextToShow;
      ctrl.TransformationToolTip = info.TransformationToolTip;
      ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
    }

    #endregion Transformation



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
      if (_view is not null)
      {
        View_PlotColumns_Initialize();
        View_PlotColumns_UpdateAll();
      }
    }

    #region Drag Handlers

    public class AvailableTransformationsDragHandlerImpl : IMVVMDragHandler
    {
      ColumnPlotDataControllerBase<TModel> _parent;

      public AvailableTransformationsDragHandlerImpl(ColumnPlotDataControllerBase<TModel> parent)
      {
        _parent = parent;
      }

      public bool CanStartDrag(IEnumerable items)
      {
        return _parent.AvailableTransformations.SelectedItem is not null;
      }

      public void DragCancelled()
      {
      }

      public void DragEnded(bool isCopy, bool isMove)
      {
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        var ttype = _parent.AvailableTransformations.SelectedValue;

        if (ttype is not null)
        {
          data = ttype;
          canCopy = true;
          canMove = true;
        }
        else
        {
          data = null;
          canCopy = false;
          canMove = false;
        }
      }
    }

    public class AvailableDataColumnsDragHandlerImpl : IMVVMDragHandler
    {
      ColumnPlotDataControllerBase<TModel> _parent;

      public AvailableDataColumnsDragHandlerImpl(ColumnPlotDataControllerBase<TModel> parent)
      {
        _parent = parent;
      }

      public bool CanStartDrag(IEnumerable items)
      {
        var selNode = _parent._availableDataColumns.FirstSelectedNode;
        // to start a drag, at least one item must be selected
        return selNode is not null && (selNode.Tag is DataColumn);
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        var selNode = _parent._availableDataColumns.FirstSelectedNode;
        // to start a drag, at least one item must be selected
        if (selNode is not null && (selNode.Tag is DataColumn dc))
        {
          data = dc;
          canCopy = true;
          canMove = false;
        }
        else
        {
          data = null;
          canCopy = false;
          canMove = false;
        }
      }

      public void DragCancelled()
      {
      }

      public void DragEnded(bool isCopy, bool isMove)
      {
      }


    }

    public class OtherAvailableColumnsDragHandlerImpl : IMVVMDragHandler
    {
      ColumnPlotDataControllerBase<TModel> _parent;

      public OtherAvailableColumnsDragHandlerImpl(ColumnPlotDataControllerBase<TModel> parent)
      {
        _parent = parent;
      }

      public bool CanStartDrag(IEnumerable items)
      {
        var type = _parent.OtherAvailableColumns.SelectedValue;
        // to start a drag, at least one item must be selected
        return type is not null;
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        var type = _parent.OtherAvailableColumns.SelectedValue;
        // to start a drag, at least one item must be selected
        if (type is not null)
        {
          data = type;
          canCopy = true;
          canMove = false;
        }
        else
        {
          data = null;
          canCopy = false;
          canMove = false;
        }
      }

      public void DragCancelled()
      {
      }

      public void DragEnded(bool isCopy, bool isMove)
      {
      }
    }

    #region ColumnDrop hander

    public void EhPlotColumnTransformationDrop(SingleColumnController ctrl, object data) => EhPlotColumnDrop(ctrl, data);

    public void EhPlotColumnDrop(SingleColumnController ctrl, object data)
    {
      var tag = ctrl.Tag;
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
          AvailableTransformations.Items.ClearSelectionsAll(); // we artificially select the node that holds that type
          var nodeToSelect = AvailableTransformations.Items.FirstOrDefault(node => (Type)node.Tag == (Type)data);
          if (nodeToSelect is not null)
          {
            nodeToSelect.IsSelected = true;
            EhView_TransformationAddTo(ctrl);
          }
        }

        info.Update(_doc.DataTable, _doc.GroupNumber);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }
      else if (data is DataColumn)
      {
        info.UnderlyingColumn = (DataColumn)data;
        info.Update(_doc.DataTable, _doc.GroupNumber);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
        TriggerUpdateOfMatchingTables();
      }
    }

    #endregion ColumnDrop hander


    #endregion
  }
}
