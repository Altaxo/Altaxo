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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Gui.Graph.Plot.Data
{
  public interface IColumnPlotDataExchangeColumnsView
  {
    /// <summary>
    /// Initialize the list of available data columns in the selected table and for the selected group number.
    /// </summary>
    /// <param name="items">The items.</param>
    void AvailableTableColumns_Initialize(NGTreeNodeCollection items);

    object AvailableTableColumns_SelectedItem { get; }

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

    void GroupNumber_Initialize(IEnumerable<int> availableGroupNumbers, int groupNumber, bool isEnabled);

    event Action<int> SelectedGroupNumberChanged;

    event Action<PlotColumnTag> PlotItemColumn_AddTo;

    event Action<PlotColumnTag> PlotItemColumn_Edit;

    event Action<PlotColumnTag> PlotItemColumn_Erase;

    event CanStartDragDelegate AvailableTableColumns_CanStartDrag;

    event StartDragDelegate AvailableTableColumns_StartDrag;

    event DragEndedDelegate AvailableTableColumns_DragEnded;

    event DragCancelledDelegate AvailableTableColumns_DragCancelled;

    event DropCanAcceptDataDelegate PlotItemColumn_DropCanAcceptData;

    event DropDelegate PlotItemColumn_Drop;
  }

  /// <summary>
  /// Controller to exchange column names of multiple plot items, which use the same columns, but in different tables.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColumnPlotDataExchangeColumnsView))]
  [UserControllerForObject(typeof(ColumnPlotDataExchangeColumnsData))]
  public class ColumnPlotDataExchangeColumnsController :
    MVCANControllerEditOriginalDocBase<ColumnPlotDataExchangeColumnsData, IColumnPlotDataExchangeColumnsView>
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

    #endregion Inner classes

    /// <summary>All data columns in the selected data table and with the selected group number.</summary>
    protected NGTreeNode _availableDataColumns = new NGTreeNode();

    protected SortedSet<int> _groupNumbersAll;

    protected bool _isDirty = false;

    protected List<GroupInfo> _columnGroup;

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

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // Group number
        _groupNumbersAll = _doc.GetCommonGroupNumbersFromTables();

        // initialize group 0

        if (null == _columnGroup)
          _columnGroup = new List<GroupInfo>();

        string previousColumnGroup = null;
        GroupInfo currentGroupInfo = null;
        foreach (var info in _doc.Columns)
        {
          if (info.ColumnGroup != previousColumnGroup)
          {
            currentGroupInfo = new GroupInfo() { GroupName = info.ColumnGroup };
            _columnGroup.Add(currentGroupInfo);
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
      }

      if (null != _view)
      {
        _view.GroupNumber_Initialize(_groupNumbersAll, _doc.GroupNumber, _groupNumbersAll.Count > 1 || (_groupNumbersAll.Count == 1 && _doc.GroupNumber != _groupNumbersAll.Min));
        _view.AvailableTableColumns_Initialize(_availableDataColumns.Nodes);

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
        int numberOfColumnsInRootLevel = (int)Calc.RMath.Pow(DataColumnBundleNode.MaxNumberOfColumnsInOneNode, levels);
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
        string ColumnLabel)>)>
      GetEnumerationForAllGroupsOfPlotColumns()
    {
      string currentColumnGroup = null;
      var list = new List<(PlotColumnTag PlotColumnTag, string ColumnLabel)>();
      int columnNumber = -1;
      foreach (var info in _doc.Columns)
      {
        ++columnNumber;
        if (info.ColumnGroup != currentColumnGroup)
        {
          currentColumnGroup = info.ColumnGroup;
          if (list.Count > 0)
            yield return (currentColumnGroup, list);

          list = new List<(PlotColumnTag PlotColumnTag, string ColumnLabel)>();
        }
        list.Add((new PlotColumnTag(_doc.GroupNumber, columnNumber), info.ColumnLabel));
      }

      if (null != list && list.Count > 0)
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
      _view?.AvailableTableColumns_Initialize(_availableDataColumns.Nodes);
    }

    private void View_PlotColumns_Initialize()
    {
      _view.PlotColumns_Initialize(GetEnumerationForAllGroupsOfPlotColumns());
    }

    private void View_PlotColumns_UpdateAll()
    {
      for (int i = 0; i < _columnGroup.Count; ++i)
      {
        for (int j = 0; j < _columnGroup[i].Columns.Count; j++)
        {
          var info = _columnGroup[i].Columns[j];
          _view.PlotColumn_Update(new PlotColumnTag(i, j), info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      int idx = 0;
      foreach (var columnGroup in _columnGroup)
      {
        foreach (var info in columnGroup.Columns)
        {
          _doc.SetNewColumnName(idx, info.NameOfDataColumn);
          ++idx;
        }
      }

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.PlotItemColumn_AddTo += EhView_PlotColumnAddTo;
      _view.PlotItemColumn_Edit += EhView_PlotColumnEdit;
      _view.PlotItemColumn_Erase += EhView_PlotColumnErase;

      _view.SelectedGroupNumberChanged += EhGroupNumberChanged;

      _view.AvailableTableColumns_CanStartDrag += EhAvailableDataColumns_CanStartDrag;
      _view.AvailableTableColumns_StartDrag += EhAvailableDataColumns_StartDrag;
      _view.AvailableTableColumns_DragEnded += EhAvailableDataColumns_DragEnded;
      _view.AvailableTableColumns_DragCancelled += EhAvailableDataColumns_DragCancelled;

      _view.PlotItemColumn_DropCanAcceptData += EhColumnDropCanAcceptData;
      _view.PlotItemColumn_Drop += EhColumnDrop;
    }

    protected override void DetachView()
    {
      _view.PlotItemColumn_AddTo -= EhView_PlotColumnAddTo;

      _view.PlotItemColumn_Edit -= EhView_PlotColumnEdit;

      _view.PlotItemColumn_Erase -= EhView_PlotColumnErase;

      _view.SelectedGroupNumberChanged -= EhGroupNumberChanged;

      _view.AvailableTableColumns_CanStartDrag -= EhAvailableDataColumns_CanStartDrag;
      _view.AvailableTableColumns_StartDrag -= EhAvailableDataColumns_StartDrag;
      _view.AvailableTableColumns_DragEnded -= EhAvailableDataColumns_DragEnded;
      _view.AvailableTableColumns_DragCancelled -= EhAvailableDataColumns_DragCancelled;

      _view.PlotItemColumn_DropCanAcceptData -= EhColumnDropCanAcceptData;
      _view.PlotItemColumn_Drop -= EhColumnDrop;

      base.DetachView();
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

    public void EhView_PlotColumnAddTo(PlotColumnTag tag)
    {
      var node = _view?.AvailableTableColumns_SelectedItem as NGTreeNode;
      if (null != node)
      {
        SetDirty();
        var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
        info.UpdateWithNameOfUnderlyingDataColumn((string)node.Tag);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }
    }

    public void EhView_PlotColumnEdit(PlotColumnTag tag)
    {
    }

    public void EhView_PlotColumnErase(PlotColumnTag tag)
    {
      SetDirty();
      var info = _columnGroup[tag.GroupNumber].Columns[tag.ColumnNumber];
      info.UpdateWithNameOfUnderlyingDataColumn(null);
      _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
    }

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

        info.Update(null, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }
      else if (data is DataColumn)
      {
        info.UnderlyingColumn = (DataColumn)data;
        info.Update(null, _doc.GroupNumber);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }
      else if (data is string)
      {
        info.UpdateWithNameOfUnderlyingDataColumn(data as string);
        _view?.PlotColumn_Update(tag, info.PlotColumnBoxText, info.PlotColumnToolTip, info.TransformationTextToShow, info.TransformationToolTip, info.PlotColumnBoxState);
      }

      return new DropReturnData
      {
        IsCopy = true,
        IsMove = false
      };
    }

    #endregion ColumnDrop hander

    #endregion Event handler
  }
}
