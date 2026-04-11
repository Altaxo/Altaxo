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

#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Plot.Data
{
  /// <summary>
  /// Provides the view contract for <see cref="ColumnPlotDataExchangeColumnsController"/>.
  /// </summary>
  public interface IColumnPlotDataExchangeColumnsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller to exchange column names of multiple plot items, which use the same columns, but in different tables.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColumnPlotDataExchangeColumnsView))]
  [UserControllerForObject(typeof(ColumnPlotDataExchangeColumnsData))]
  public class ColumnPlotDataExchangeColumnsController :
    MVCANControllerEditOriginalDocBase<ColumnPlotDataExchangeColumnsData, IColumnPlotDataExchangeColumnsView>, ISingleColumnControllerParent
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

      /// <summary>
      /// Initializes a new instance of the <see cref="PlotColumnInformationInternal"/> class.
      /// </summary>
      /// <param name="column">The column represented by this plot-column information.</param>
      /// <param name="nameOfUnderlyingDataColumn">The name of the underlying data column.</param>
      public PlotColumnInformationInternal(IReadableColumn column, string nameOfUnderlyingDataColumn)
        : base(column, nameOfUnderlyingDataColumn)
      {
      }

      /// <inheritdoc/>
      protected override void OnChanged()
      {
        ColumnSetter?.Invoke(Column, _supposedDataTable, _supposedGroupNumber);
      }
    }

      /// <summary>
      /// Stores the columns that belong to one displayed group.
      /// </summary>
    protected class GroupInfo
    {
        /// <summary>
        /// Gets or sets the display name of the group.
        /// </summary>
      public string GroupName;
        /// <summary>
        /// Gets the columns that belong to this group.
        /// </summary>
      public List<PlotColumnInformationInternal> Columns = new List<PlotColumnInformationInternal>();
    }

      /// <summary>
      /// Represents a single available column name in the tree.
      /// </summary>
    protected class DataColumnSingleNode : NGTreeNode
    {
      private List<string> _table;
      private string _toolTip = null;

      /// <summary>
      /// Initializes a new instance of the <see cref="DataColumnSingleNode"/> class.
      /// </summary>
      /// <param name="columnNames">The available column names.</param>
      /// <param name="columnName">The represented column name.</param>
      /// <param name="isSelected"><c>true</c> to initialize the node as selected; otherwise, <c>false</c>.</param>
      public DataColumnSingleNode(List<string> columnNames, string columnName, bool isSelected)
        :
        base(columnName)
      {
        _tag = columnName;
        _isSelected = isSelected;
        _table = columnNames;
      }

        /// <summary>
        /// Gets the tooltip text for this column node.
        /// </summary>
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
        _toolTip = string.Format("Column: {0}", _tag);
        OnPropertyChanged(nameof(ToolTip));
      }
    }

      /// <summary>
      /// Represents a tree node that bundles many available column names.
      /// </summary>
    protected class DataColumnBundleNode : NGTreeNode
    {
        /// <summary>
        /// Maximum number of columns shown directly in one node.
        /// </summary>
      public const int MaxNumberOfColumnsInOneNode = 200;

      private int _firstColumn;
      private int _columnCount;
      // private DataTable _dataTable;
      private List<string> _columns;

      /// <summary>
      /// Initializes a new instance of the <see cref="DataColumnBundleNode"/> class.
      /// </summary>
      /// <param name="columnList">The available column names.</param>
      /// <param name="firstColumn">The index of the first represented column.</param>
      /// <param name="columnCount">The number of represented columns.</param>
      public DataColumnBundleNode(List<string> columnList, int firstColumn, int columnCount)
        : base(true)
      {
        _columns = columnList;
        _firstColumn = firstColumn;
        _columnCount = columnCount;
        Text = string.Format("Cols {0}-{1}", firstColumn, firstColumn + columnCount - 1);
      }

      /// <inheritdoc/>
      protected override void LoadChildren()
      {
        var coll = _columns;
        Nodes.Clear();
        int nextColumn = Math.Min(_firstColumn + _columnCount, coll.Count);

        if (_columnCount <= MaxNumberOfColumnsInOneNode) // If number is low enough, expand to the data columns directly
        {
          for (int i = _firstColumn; i < nextColumn; ++i)
            Nodes.Add(new DataColumnSingleNode(_columns, _columns[i], false));
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
            Nodes.Add(new DataColumnBundleNode(coll, first, Math.Min(remaining, colsInOneNode)));
            remaining -= colsInOneNode;
            first += colsInOneNode;
          }
        }
      }
    }

    /// <summary>
    /// Identifies the UI element that currently supplies column data.
    /// </summary>
    protected enum ColumnSourceElement
    {
      /// <summary>
      /// The flat list of available table columns.
      /// </summary>
      AvailableTableColumnsList,

      /// <summary>
      /// The tree view of available table columns.
      /// </summary>
      AvailableTableColumnsTree,

      /// <summary>
      /// Other columns that can be selected.
      /// </summary>
      OtherAvailableColumns,

      /// <summary>
      /// The list of available column transformations.
      /// </summary>
      AvailableTransformations
    }

    #endregion Inner classes

    /// <summary>
    /// Indicates whether the controller contains unapplied changes.
    /// </summary>
    protected bool _isDirty = false;

    /// <summary>
    /// Holds the column groups displayed by the controller.
    /// </summary>
    protected List<GroupInfo> _columnGroups;


    /// <summary>
    /// Designates the latest focused element that can act as column source.
    /// </summary>
    ColumnSourceElement _lastActiveColumnsSource;

    #region Infrastructur Dispose and GetSubControllers

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _availableDataColumns = null;
      base.Dispose(isDisposing);
    }

    /// <summary>
    /// Marks the controller as modified.
    /// </summary>
    public void SetDirty()
    {
      _isDirty = true;
    }

    #endregion Infrastructur Dispose and GetSubControllers

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnPlotDataExchangeColumnsController"/> class.
    /// </summary>
    public ColumnPlotDataExchangeColumnsController()
    {
      AvailableDataColumnsDragHandler = new AvailableDataColumnsDragHandlerImpl(this);
    }

    #region Bindings

    private ObservableCollection<int> _availabeGroupNumbers = new ObservableCollection<int>();

    /// <summary>
    /// Gets or sets the available group numbers.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the selected group number.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether group-number selection is enabled.
    /// </summary>
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

    /// <summary>All data columns in the selected data table and with the selected group number.</summary>
    protected NGTreeNode _availableDataColumns = new NGTreeNode();

    /// <summary>
    /// Initialize the list of available data columns in the selected table and for the selected group number.
    /// </summary>
    public NGTreeNodeCollection AvailableTableColumnsForListView => IsTableColumnsListVisible ? _availableDataColumns.Nodes : null;

    private NGTreeNode _availableTableColumnsListSelectedItem;
    /// <summary>
    /// Gets or sets the selected item in the flat list of available columns.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the drag handler for available columns.
    /// </summary>
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

    private bool _isAvailableDataColumnsListViewFocused;

    /// <summary>
    /// Gets or sets a value indicating whether the list view of available columns has the focus.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the tree view of available columns has the focus.
    /// </summary>
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

    private ObservableCollection<SingleColumnController> _plotItemColumns = new ObservableCollection<SingleColumnController>();

    /// <summary>
    /// Gets or sets the controllers for the displayed plot-item columns.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // Group number
        AvailabeGroupNumbers = new ObservableCollection<int>(_doc.GetCommonGroupNumbersFromTables());
        SelectedGroupNumber = _doc.GroupNumber;

        // initialize group 0

        _columnGroups = new List<GroupInfo>();
        string previousColumnGroup = null;
        GroupInfo currentGroupInfo = null;
        foreach (var info in _doc.Columns)
        {
          if (info.ColumnGroup != previousColumnGroup)
          {
            currentGroupInfo = new GroupInfo() { GroupName = info.ColumnGroup };
            _columnGroups.Add(currentGroupInfo);
          }

          var pcinfo = new PlotColumnInformationInternal(null, info.ColumnName)
          {
            PlotColumnBoxStateIfColumnIsMissing = PlotColumnControlState.Error,
            Label = info.ColumnLabel,
            ColumnSetter = null
          };
          pcinfo.UpdateWithNameOfUnderlyingDataColumn(info.ColumnName);

          currentGroupInfo.Columns.Add(pcinfo);

          previousColumnGroup = info.ColumnGroup;
        }

        // Initialize columns
        Controller_AvailableDataColumns_Initialize();
        View_PlotColumns_Initialize();
        View_PlotColumns_UpdateAll();
      }
    }

    private void Controller_AvailableDataColumns_Initialize()
    {
      _availableDataColumns.Nodes.Clear();

      var columns = _doc.GetCommonColumnNamesWithGroupNumber(_doc.GroupNumber);
      if (columns.Count <= DataColumnBundleNode.MaxNumberOfColumnsInOneNode)
      {
        for (int i = 0; i < columns.Count; ++i)
        {
          var col = columns[i];
          var node = new DataColumnSingleNode(columns, columns[i], false);
          _availableDataColumns.Nodes.Add(node);
        }
      }
      else // Create a tree of nodes
      {
        int levels = (int)(Math.Floor(Math.Log(columns.Count, DataColumnBundleNode.MaxNumberOfColumnsInOneNode)));
        int numberOfColumnsInRootLevel = (int)RMath.Pow(DataColumnBundleNode.MaxNumberOfColumnsInOneNode, levels);
        for (int i = 0; i < columns.Count; i += numberOfColumnsInRootLevel)
        {
          var node = new DataColumnBundleNode(columns, i, Math.Min(numberOfColumnsInRootLevel, columns.Count - i));
          _availableDataColumns.Nodes.Add(node);
        }
      }
    }

    private IEnumerable<(
      string GroupName, // group name
      IEnumerable<( // list of column definitions
        PlotColumnTag PlotColumnTag, // tag to identify the column and group
        string ColumnLabel)> Columns)>
      GetEnumerationForAllGroupsOfPlotColumns()
    {
      string currentColumnGroup = null;
      var list = new List<(PlotColumnTag PlotColumnTag, string ColumnLabel)>();
      int columnNumber = 0;
      int columnGroupNumber = -1;
      foreach (var info in _doc.Columns)
      {
        if (info.ColumnGroup != currentColumnGroup)
        {
          if (list.Count > 0)
            yield return (currentColumnGroup, list);

          ++columnGroupNumber;
          columnNumber = 0;
          currentColumnGroup = info.ColumnGroup;
          list = new List<(PlotColumnTag PlotColumnTag, string ColumnLabel)>();
        }
        list.Add((new PlotColumnTag(columnGroupNumber, columnNumber), info.ColumnLabel));
        ++columnNumber;
      }

      if (list is not null && list.Count > 0)
        yield return (currentColumnGroup, list);
    }

    /// <summary>
    /// Try to replace the columns in ColumnInfo with that of the currently chosen table/group number. Additionally, the state of the columns is updated, and
    /// the changed infos are sent to the view.
    /// </summary>
    private void ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState()
    {
      // Initialize columns
      Controller_AvailableDataColumns_Initialize();
    }

    private void View_PlotColumns_Initialize()
    {
      var allItems = GetEnumerationForAllGroupsOfPlotColumns();
      _plotItemColumns.Clear();
      foreach (var group in allItems)
      {
        foreach (var item in group.Item2)
        {
          _plotItemColumns.Add(new SingleColumnController() { Parent=this, GroupName = group.GroupName, LabelText = item.ColumnLabel, Tag = item.PlotColumnTag });
        }
      }
    }


    private void View_PlotColumns_UpdateAll()
    {
      for (int i = 0; i < _columnGroups.Count; ++i)
      {
        for (int j = 0; j < _columnGroups[i].Columns.Count; j++)
        {
          var info = _columnGroups[i].Columns[j];
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
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      int idx = 0;
      foreach (var columnGroup in _columnGroups)
      {
        foreach (var info in columnGroup.Columns)
        {
          _doc.SetNewColumnName(idx, info.NameOfDataColumn);
          ++idx;
        }
      }

      return ApplyEnd(true, disposeController);
    }

   

    #region Event handler

    private void EhGroupNumberChanged(int groupNumber)
    {
      if (groupNumber != _doc.GroupNumber)
      {
        _doc.GroupNumber = groupNumber;
        ReplaceColumnsWithColumnsFromNewTableGroupAndUpdateColumnState();
      }
    }

    /// <summary>
    /// Assigns the currently selected available column to the specified plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnAddTo(SingleColumnController ctrl)
    {
      var node = _availableDataColumns.FirstSelectedNode;
      var tag = ctrl.Tag;
      if (node is not null)
      {
        SetDirty();
        var info = _columnGroups[tag.GroupNumber].Columns[tag.ColumnNumber];
        info.UpdateWithNameOfUnderlyingDataColumn((string)node.Tag);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }
    }

    /// <summary>
    /// Handles editing of a plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnEdit(SingleColumnController ctrl)
    {
    }

    /// <summary>
    /// Removes the assigned column from the specified plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnErase(SingleColumnController ctrl)
    {
      var tag = ctrl.Tag;
      SetDirty();
      var info = _columnGroups[tag.GroupNumber].Columns[tag.ColumnNumber];
      info.UpdateWithNameOfUnderlyingDataColumn(null);
      ctrl.ColumnText = info.PlotColumnBoxText;
      ctrl.ColumnToolTip = info.PlotColumnToolTip;
      ctrl.TransformationText = info.TransformationTextToShow;
      ctrl.TransformationToolTip = info.TransformationToolTip;
      ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
    }

    #endregion Event handler

    #region Drag/Drop Handlers

      /// <summary>
      /// Drag handler for items from the available-columns list.
      /// </summary>
    public class AvailableDataColumnsDragHandlerImpl : IMVVMDragHandler
    {
      ColumnPlotDataExchangeColumnsController _parent;

      /// <summary>
      /// Initializes a new instance of the <see cref="AvailableDataColumnsDragHandlerImpl"/> class.
      /// </summary>
      /// <param name="parent">The owning controller.</param>
      public AvailableDataColumnsDragHandlerImpl(ColumnPlotDataExchangeColumnsController parent)
      {
        _parent = parent;
      }

      /// <summary>
      /// Determines whether dragging can start for the current selection.
      /// </summary>
      /// <param name="items">The items that would participate in the drag operation.</param>
      /// <returns><c>true</c> if a column is selected; otherwise, <c>false</c>.</returns>
      public bool CanStartDrag(IEnumerable items)
      {
        var selNode = _parent._availableDataColumns.FirstSelectedNode;
        // to start a drag, at least one item must be selected
        return selNode is not null && (selNode.Tag is DataColumn);
      }

      /// <summary>
      /// Starts the drag operation for the selected available column.
      /// </summary>
      /// <param name="items">The items to drag.</param>
      /// <param name="data">Receives the drag data.</param>
      /// <param name="canCopy">Receives whether copying is supported.</param>
      /// <param name="canMove">Receives whether moving is supported.</param>
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

      /// <summary>
      /// Called when a drag operation is cancelled.
      /// </summary>
      public void DragCancelled()
      {
      }

      /// <summary>
      /// Called when a drag operation has ended.
      /// </summary>
      /// <param name="isCopy"><c>true</c> if the drag operation copied data.</param>
      /// <param name="isMove"><c>true</c> if the drag operation moved data.</param>
      public void DragEnded(bool isCopy, bool isMove)
      {
      }


    }

    #region ColumnDrop hander

    /// <summary>
    /// Handles a dropped transformation for a plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    /// <param name="data">The dropped data.</param>
    public void EhPlotColumnTransformationDrop(SingleColumnController ctrl, object data)
    {
      EhPlotColumnDrop(ctrl, data);
    }


    /// <summary>
    /// Handles dropped data for a plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    /// <param name="data">The dropped data.</param>
    public void EhPlotColumnDrop(SingleColumnController ctrl, object data)
    {
      var tag = ctrl.Tag;
     

      _isDirty = true;

      var info = _columnGroups[tag.GroupNumber].Columns[tag.ColumnNumber];

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

        info.Update(null, _doc.GroupNumber);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }
      else if (data is DataColumn)
      {
        info.UnderlyingColumn = (DataColumn)data;
        info.Update(null, _doc.GroupNumber);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }
      else if (data is string)
      {
        info.UpdateWithNameOfUnderlyingDataColumn(data as string);
        ctrl.ColumnText = info.PlotColumnBoxText;
        ctrl.ColumnToolTip = info.PlotColumnToolTip;
        ctrl.TransformationText = info.TransformationTextToShow;
        ctrl.TransformationToolTip = info.TransformationToolTip;
        ctrl.SeverityLevel = (int)info.PlotColumnBoxState;
      }

      
    }

    /// <summary>
    /// Handles editing of a plot-column transformation.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnTransformationEdit(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the transformation from a plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnTransformationErase(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds the dropped transformation as the only transformation of a plot column.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnTransformationAddAsSingle(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Prepends the dropped transformation to the plot column transformation chain.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnTransformationAddAsPrepending(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Appends the dropped transformation to the plot column transformation chain.
    /// </summary>
    /// <param name="ctrl">The target plot-column controller.</param>
    public void EhPlotColumnTransformationAddAsAppending(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    #endregion ColumnDrop hander


    #endregion
  }
}
