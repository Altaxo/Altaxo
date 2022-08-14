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
      private List<string> _table;
      private string _toolTip = null;

      public DataColumnSingleNode(List<string> columnNames, string columnName, bool isSelected)
        :
        base(columnName)
      {
        _tag = columnName;
        _isSelected = isSelected;
        _table = columnNames;
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
        _toolTip = string.Format("Column: {0}", _tag);
        OnPropertyChanged(nameof(ToolTip));
      }
    }

    protected class DataColumnBundleNode : NGTreeNode
    {
      public const int MaxNumberOfColumnsInOneNode = 200;

      private int _firstColumn;
      private int _columnCount;
      // private DataTable _dataTable;
      private List<string> _columns;

      public DataColumnBundleNode(List<string> columnList, int firstColumn, int columnCount)
        : base(true)
      {
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

    protected enum ColumnSourceElement
    {
      AvailableTableColumnsList,
      AvailableTableColumnsTree,
      OtherAvailableColumns,
      AvailableTransformations
    }

    #endregion Inner classes

    protected bool _isDirty = false;

    protected List<GroupInfo> _columnGroups;


    /// <summary>
    /// Designates the latest focused element that can act as column source.
    /// </summary>
    ColumnSourceElement _lastActiveColumnsSource;

    #region Infrastructur Dispose and GetSubControllers

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _availableDataColumns = null;
      base.Dispose(isDisposing);
    }

    public void SetDirty()
    {
      _isDirty = true;
    }

    #endregion Infrastructur Dispose and GetSubControllers

    public ColumnPlotDataExchangeColumnsController()
    {
      AvailableDataColumnsDragHandler = new AvailableDataColumnsDragHandlerImpl(this);
    }

    #region Bindings

    private ObservableCollection<int> _availabeGroupNumbers = new ObservableCollection<int>();

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

    public void EhPlotColumnEdit(SingleColumnController ctrl)
    {
    }

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

    public class AvailableDataColumnsDragHandlerImpl : IMVVMDragHandler
    {
      ColumnPlotDataExchangeColumnsController _parent;

      public AvailableDataColumnsDragHandlerImpl(ColumnPlotDataExchangeColumnsController parent)
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

    #region ColumnDrop hander

    public void EhPlotColumnTransformationDrop(SingleColumnController ctrl, object data)
    {
      EhPlotColumnDrop(ctrl, data);
    }


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

    public void EhPlotColumnTransformationEdit(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    public void EhPlotColumnTransformationErase(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    public void EhPlotColumnTransformationAddAsSingle(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    public void EhPlotColumnTransformationAddAsPrepending(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    public void EhPlotColumnTransformationAddAsAppending(SingleColumnController ctrl)
    {
      throw new NotImplementedException();
    }

    #endregion ColumnDrop hander


    #endregion
  }
}
