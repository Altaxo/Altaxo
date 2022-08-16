#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Gui.Data
{
  public interface IDataTableMatrixProxyView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IDataTableMatrixProxyView))]
  [UserControllerForObject(typeof(DataTableMatrixProxy))]
  public class DataTableMatrixProxyController : MVCANControllerEditOriginalDocBase<DataTableMatrixProxy, IDataTableMatrixProxyView>
  {
    private int _maxPossiblePlotRangeTo;


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rowsController, () => _rowsController = null);
    }

    public DataTableMatrixProxyController()
    {
      CmdTakeAsXColumn = new RelayCommand(EhUseSelectedItemAsXColumn);
      CmdTakeAsYColumn = new RelayCommand(EhUseSelectedItemAsYColumn);
      CmdTakeAsVColumns = new RelayCommand(EhUseSelectedItemAsVColumns);

      CmdEraseXColumn = new RelayCommand(EhClearXColumn);
      CmdEraseYColumn = new RelayCommand(EhClearYColumn);
      CmdEraseVColumns = new RelayCommand(EhClearVColumns);
    }


    #region Bindings

    public ICommand CmdTakeAsXColumn { get; }
    public ICommand CmdEraseXColumn { get; }
    public ICommand CmdTakeAsYColumn { get; }
    public ICommand CmdEraseYColumn { get; }
    public ICommand CmdTakeAsVColumns { get; }
    public ICommand CmdEraseVColumns { get; }


    private ItemsController<DataTable> _dataTable;

    public ItemsController<DataTable> DataTable
    {
      get => _dataTable;
      set
      {
        if (!(_dataTable == value))
        {
          _dataTable = value;
          OnPropertyChanged(nameof(DataTable));
        }
      }
    }

    private ObservableCollection<int> _availableGroups;

    public ObservableCollection<int> AvailableGroups
    {
      get => _availableGroups;
      set
      {
        if (!(_availableGroups == value))
        {
          _availableGroups = value;
          OnPropertyChanged(nameof(AvailableGroups));
        }
      }
    }

    private int _selectedGroup = int.MinValue;

    public int SelectedGroup
    {
      get => _selectedGroup;
      set
      {
        if (!(_selectedGroup == value))
        {
          _selectedGroup = value;
          OnPropertyChanged(nameof(SelectedGroup));
          EhSelectedGroupNumberChanged(value);
        }
      }
    }

    private bool _useAllAvailableVColumnsOfGroup;

    public bool UseAllAvailableVColumnsOfGroup
    {
      get => _useAllAvailableVColumnsOfGroup;
      set
      {
        if (!(_useAllAvailableVColumnsOfGroup == value))
        {
          _useAllAvailableVColumnsOfGroup = value;
          OnPropertyChanged(nameof(UseAllAvailableVColumnsOfGroup));
        }
      }
    }

    private bool _showDataColumns = true;

    public bool ShowDataColumns
    {
      get => _showDataColumns;
      set
      {
        if (!(_showDataColumns == value))
        {
          _showDataColumns = value;
          OnPropertyChanged(nameof(ShowDataColumns));
          OnPropertyChanged(nameof(ShowPropertyColumns));
          FillAvailableColumnList();
        }
      }
    }

    public bool ShowPropertyColumns
    {
      get => !ShowDataColumns;
      set => ShowDataColumns = !value;
    }





    private SelectableListNodeList _availableColumns;

    public SelectableListNodeList AvailableColumns
    {
      get => _availableColumns;
      set
      {
        if (!(_availableColumns == value))
        {
          _availableColumns = value;
          OnPropertyChanged(nameof(AvailableColumns));
        }
      }
    }

    private SelectableListNodeList _participatingVColumns;

    public SelectableListNodeList ParticipatingVColumns
    {
      get => _participatingVColumns;
      set
      {
        if (!(_participatingVColumns == value))
        {
          _participatingVColumns = value;
          OnPropertyChanged(nameof(ParticipatingVColumns));
        }
      }
    }

    private SelectableListNode _participatingXColumn;

    public SelectableListNode ParticipatingXColumn
    {
      get => _participatingXColumn;
      set
      {
        if (!(_participatingXColumn == value))
        {
          _participatingXColumn = value;
          OnPropertyChanged(nameof(ParticipatingXColumn));
        }
      }
    }

    private SelectableListNode _participatingYColumn;

    public SelectableListNode ParticipatingYColumn
    {
      get => _participatingYColumn;
      set
      {
        if (!(_participatingYColumn == value))
        {
          _participatingYColumn = value;
          OnPropertyChanged(nameof(ParticipatingYColumn));
        }
      }
    }

    private bool _useAllAvailableDataRows;

    public bool UseAllAvailableDataRows
    {
      get => _useAllAvailableDataRows;
      set
      {
        if (!(_useAllAvailableDataRows == value))
        {
          _useAllAvailableDataRows = value;
          OnPropertyChanged(nameof(UseAllAvailableDataRows));
        }
      }
    }


    private Altaxo.Gui.Common.AscendingIntegerCollectionController _rowsController;

    public Altaxo.Gui.Common.AscendingIntegerCollectionController RowsController
    {
      get => _rowsController;
      set
      {
        if (!(_rowsController == value))
        {
          _rowsController = value;
          OnPropertyChanged(nameof(RowsController));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var xCol = _doc.RowHeaderColumn;
        if (xCol is DataColumn dxc)
          ParticipatingXColumn = new SelectableListNode(DataColumnCollection.GetParentDataColumnCollectionOf(dxc).GetColumnName(dxc), dxc, false);
        else if (xCol is not null)
          ParticipatingXColumn = new SelectableListNode(xCol.FullName, xCol, false);
        else
          ParticipatingXColumn = null;


        var yCol = _doc.ColumnHeaderColumn;
        if (yCol is DataColumn dyc)
          ParticipatingYColumn = new SelectableListNode(DataColumnCollection.GetParentDataColumnCollectionOf(dyc).GetColumnName(dyc), dyc, false);
        else if (yCol is not null)
          ParticipatingXColumn = new SelectableListNode(yCol.FullName, yCol, false);
        else
          ParticipatingYColumn = null;

        // Initialize value columns
        var vCols = new SelectableListNodeList();
        for (int i = 0; i < _doc.ColumnCount; ++i)
        {
          var col = _doc.GetDataColumnProxy(i).Document();
          if(col is not null)
            vCols.Add(new SelectableListNode(
              col is DataColumn dc ? DataColumnCollection.GetParentDataColumnCollectionOf(dc).GetColumnName(dc) : col.FullName,
              col,
              false));
        }
        ParticipatingVColumns = vCols;


        CalcMaxPossiblePlotRangeTo();

        // Initialize tables
        string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();
        string dataTableName = _doc.DataTable is null ? string.Empty : _doc.DataTable.Name;
        var availableTables = new SelectableListNodeList();
        foreach (var tableName in tables)
        {
          availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], false));
        }
        DataTable = new ItemsController<DataTable>(availableTables, EhSelectedTableChanged);
        DataTable.SelectedValue = _doc.DataTable;

        UseAllAvailableVColumnsOfGroup = _doc.UseAllAvailableDataColumnsOfGroup;
        UseAllAvailableDataRows = _doc.UseAllAvailableDataRows;

        // Initialize columns
        FillAvailableColumnList();

        _rowsController = new Common.AscendingIntegerCollectionController();
        _rowsController.InitializeDocument(_doc.ParticipatingDataRows.Clone());
      }
    }

    private void EhSelectedTableChanged(DataTable selectedTable)
    {
      var groupNumber = AvailableGroups is not null ? SelectedGroup : _doc.GroupNumber;

      // Initialize group numbers
      var availableGroups = (selectedTable ?? _doc.DataTable).DataColumns.GetGroupNumbersAll();
      AvailableGroups = new ObservableCollection<int>(availableGroups);

      if (availableGroups.Contains(groupNumber))
      {
        SelectedGroup = groupNumber;
        EhSelectedGroupNumberChanged(groupNumber);
      }
      else if (availableGroups.Count > 0)
      {
        SelectedGroup = availableGroups.First();
        EhSelectedGroupNumberChanged(availableGroups.First());
      }
    }

    private void EhSelectedGroupNumberChanged(int groupNumber)
    {
      var table = DataTable.SelectedValue;
      if (table is null)
        return;

      // Initialize available columns
      var columnList = table.DataColumns.GetListOfColumnsWithGroupNumber(groupNumber);
      var columnDict = new Dictionary<string, DataColumn>();
      foreach (var c in columnList)
      {
        columnDict.Add(DataColumnCollection.GetParentDataColumnCollectionOf(c).GetColumnName(c), c);
      }

      var propcolDict = new Dictionary<string, DataColumn>();
      foreach (var c in table.PropCols.Columns)
      {
        propcolDict.Add(DataColumnCollection.GetParentDataColumnCollectionOf(c).GetColumnName(c), c);
      }


      // X-Column
      string xColName = ParticipatingXColumn?.Text;
      if(!string.IsNullOrEmpty(xColName) &&  columnDict.TryGetValue(xColName, out var newXCol))
      {
        ParticipatingXColumn = new SelectableListNode(xColName, newXCol, false);
      }
      else
      {
        ParticipatingXColumn = null;
      }

      // Y-Column
      string yColName = ParticipatingYColumn?.Text;
      if (!string.IsNullOrEmpty(yColName) && propcolDict.TryGetValue(xColName, out var newYCol))
      {
        ParticipatingYColumn = new SelectableListNode(yColName, newYCol, false);
      }
      else
      {
        ParticipatingYColumn = null;
      }

      // V-Columns
      DataColumn[] participatingColumns;
      participatingColumns = ParticipatingVColumns.Select(n => (DataColumn)n.Tag).ToArray();
      var participatingColNames = participatingColumns.Select(c => DataColumnCollection.GetParentDataColumnCollectionOf(c).GetColumnName(c)).ToArray();

      ParticipatingVColumns = new SelectableListNodeList(
        participatingColNames.Where(cn => columnDict.ContainsKey(cn)).Select(cn => new SelectableListNode(cn, columnDict[cn], false)));

      FillAvailableColumnList();
    }

    private void FillAvailableColumnList()
    {
      var availableColumns = new SelectableListNodeList();
      
      DataTable tg = DataTable.SelectedValue;

      if (tg is not null)
      {
        if (ShowDataColumns)
        {
          for (int i = 0; i < tg.DataColumnCount; ++i)
            availableColumns.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
        }
        else
        {
          for (int i = 0; i < tg.PropertyColumnCount; ++i)
            availableColumns.Add(new SelectableListNode(tg.PropertyColumns.GetColumnName(i), tg.PropertyColumns[i], false));
        }
      }
      AvailableColumns = availableColumns;
    }


    public override bool Apply(bool disposeController)
    {
      _doc.DataTable = DataTable.SelectedValue;
      _doc.GroupNumber = SelectedGroup;
      _doc.UseAllAvailableDataColumnsOfGroup = UseAllAvailableVColumnsOfGroup;
      _doc.UseAllAvailableDataRows = UseAllAvailableDataRows;

      _doc.RowHeaderColumn = (IReadableColumn)ParticipatingXColumn?.Tag;
      _doc.ColumnHeaderColumn = (IReadableColumn)ParticipatingYColumn?.Tag;
      _doc.SetDataColumns(ParticipatingVColumns.Select(n => ReadableColumnProxy.FromColumn(((IReadableColumn)n.Tag))));

      if (!_doc.UseAllAvailableDataRows)
      {
        if (!_rowsController.Apply(disposeController))
        {
          return ApplyEnd(false, disposeController);
        }
        _doc.SetDataRows((IAscendingIntegerCollection)_rowsController.ModelObject);
      }

      return ApplyEnd(true, disposeController); // successfull
    }

    private void CalcMaxPossiblePlotRangeTo()
    {
      var xColumn = ParticipatingXColumn?.Tag as IReadableColumn;
      var yColumn = ParticipatingYColumn?.Tag as IReadableColumn;


      int len = int.MaxValue;
      if (xColumn?.Count is int xCount)
        len = Math.Min(len, xCount);
      if (yColumn?.Count is int yCount)
        len = Math.Min(len, yCount);

      _maxPossiblePlotRangeTo = len - 1;
    }


    private void EhUseSelectedItemAsXColumn()
    {
      var node = AvailableColumns.FirstSelectedNode;
      ParticipatingXColumn = new SelectableListNode(node.Text, node.Tag, false);
    }

    private void EhUseSelectedItemAsYColumn()
    {
      var node = AvailableColumns.FirstSelectedNode;
      ParticipatingYColumn = new SelectableListNode(node.Text, node.Tag, false);
    }

    private void EhUseSelectedItemAsVColumns()
    {
      foreach (var node in AvailableColumns.Where(n => n.IsSelected))
      {
        var colToAdd = node.Tag as IReadableColumn;
        if (colToAdd is null)
          continue;

        // before adding this node, check that it is not already present
        if (ParticipatingVColumns.Any(n => n.Text == node.Text))
          continue;

        ParticipatingVColumns.Add(new SelectableListNode(node.Text, colToAdd, false));
      }
    }

    private void EhClearXColumn()
    {
      ParticipatingXColumn = null;
    }

    private void EhClearYColumn()
    {
      ParticipatingYColumn = null;
    }

    private void EhClearVColumns()
    {
      if (ParticipatingVColumns.FirstSelectedNode is not null) // if anything selected, clear only the selected nodes
      {
        ParticipatingVColumns.RemoveSelectedItems();
      }
      else // if nothing selected, clear all nodes
      {
        ParticipatingVColumns.Clear();
      }
    }
  }
}
