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
  public interface IDataTableMatrixProxyWithMultipleColumnHeaderColumnsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IDataTableMatrixProxyWithMultipleColumnHeaderColumnsView))]
  [UserControllerForObject(typeof(DataTableMatrixProxyWithMultipleColumnHeaderColumns))]
  public class DataTableMatrixProxyWithMultipleColumnHeaderColumnsController : MVCANControllerEditOriginalDocBase<DataTableMatrixProxyWithMultipleColumnHeaderColumns, IDataTableMatrixProxyWithMultipleColumnHeaderColumnsView>
  {
    private int _maxPossiblePlotRangeTo;


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rowsController, () => _rowsController = null);
    }

    public DataTableMatrixProxyWithMultipleColumnHeaderColumnsController()
    {
      CmdTakeAsXColumn = new RelayCommand(EhUseSelectedItemAsXColumn);
      CmdTakeAsYColumns = new RelayCommand(EhUseSelectedItemAsYColumns);
      CmdTakeAsVColumns = new RelayCommand(EhUseSelectedItemAsVColumns);

      CmdEraseXColumn = new RelayCommand(EhClearXColumn);
      CmdEraseYColumns = new RelayCommand(EhClearYColumns);
      CmdEraseVColumns = new RelayCommand(EhClearVColumns);
    }


    #region Bindings

    public ICommand CmdTakeAsXColumn { get; }
    public ICommand CmdEraseXColumn { get; }
    public ICommand CmdTakeAsYColumns { get; }
    public ICommand CmdEraseYColumns { get; }
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

    private bool _useAllAvailableVColumnsOfGroup = true;

    public bool UseAllAvailableVColumnsOfGroup
    {
      get => _useAllAvailableVColumnsOfGroup;
      set
      {
        if (!(_useAllAvailableVColumnsOfGroup == value))
        {
          _useAllAvailableVColumnsOfGroup = value;
          OnPropertyChanged(nameof(UseAllAvailableVColumnsOfGroup));
          EhUseAllAvailableVColumnsOfGroupChanged(value);
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



    private SelectableListNodeList _participatingYColumns;

    public SelectableListNodeList ParticipatingYColumns
    {
      get => _participatingYColumns;
      set
      {
        if (!(_participatingYColumns == value))
        {
          _participatingYColumns = value;
          OnPropertyChanged(nameof(ParticipatingYColumns));
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
        ParticipatingXColumn = xCol is null ? null : new SelectableListNode(GetColumnNameOrNull(xCol), xCol, false);

        // Initialize value columns
        var vCols = new SelectableListNodeList();
        for (int i = 0; i < _doc.ColumnCount; ++i)
        {
          var col = _doc.GetDataColumnProxy(i).Document();
          if(col is not null)
          {
            vCols.Add(new SelectableListNode(GetColumnNameOrNull(col), col, false));
          }
        }
        ParticipatingVColumns = vCols;


        var yCols = new SelectableListNodeList();
        for (int i = 0; i < _doc.ColumnHeaderColumnsCount; ++i)
        {
          var col = _doc.GetColumnHeaderColumn(i).Document();
          if (col is not null)
          {
            vCols.Add(new SelectableListNode(GetColumnNameOrNull(col), col, false));
          }
        }
        ParticipatingYColumns = yCols;

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

        var rowsController = new Common.AscendingIntegerCollectionController();
        rowsController.InitializeDocument(_doc.ParticipatingDataRows.Clone());
        Current.Gui.FindAndAttachControlTo(rowsController);
        RowsController = rowsController;
      }
    }

    string? GetColumnNameOrNull(IReadableColumn? col)
    {
      if (col is DataColumn dc)
        return DataColumnCollection.GetParentDataColumnCollectionOf(dc).GetColumnName(dc);
      else if (col is not null)
        return col.FullName;
      else
        return null;
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
        columnDict.Add(GetColumnNameOrNull(c), c);
      }

      var propcolDict = new Dictionary<string, DataColumn>();
      foreach (var c in table.PropCols.Columns)
      {
        propcolDict.Add(GetColumnNameOrNull(c), c);
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

      // V-Columns
      var participatingColNames = ParticipatingVColumns.Select(n => n.Text).ToArray();
      ParticipatingVColumns = new SelectableListNodeList(
        participatingColNames.Where(cn => columnDict.ContainsKey(cn)).Select(cn => new SelectableListNode(cn, columnDict[cn], false)));


      // Y-Columns
      var participatingYColNames = ParticipatingYColumns.Select(n => n.Text).ToArray();
      ParticipatingYColumns = new SelectableListNodeList(
        participatingYColNames.Where(cn => propcolDict.ContainsKey(cn)).Select(cn => new SelectableListNode(cn, columnDict[cn], false)));


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

    private void EhUseAllAvailableVColumnsOfGroupChanged(bool value)
    {
      if(value == true)
      {
        var table = DataTable.SelectedValue ?? _doc.DataTable;
        var group = SelectedGroup;

        var participatingVColumns = new SelectableListNodeList();
        for (int i = 0; i < table.DataColumnCount; ++i)
        {
          var c = table.DataColumns[i];
          if(table.DataColumns.GetColumnGroup(i) == group && table.DataColumns.GetColumnKind(i) == ColumnKind.V)
            participatingVColumns.Add(new SelectableListNode(GetColumnNameOrNull(c), c, false));
        }
        ParticipatingVColumns = participatingVColumns;
      }
    }


    public override bool Apply(bool disposeController)
    {
      _doc.DataTable = DataTable.SelectedValue;
      _doc.GroupNumber = SelectedGroup;
      _doc.UseAllAvailableDataColumnsOfGroup = UseAllAvailableVColumnsOfGroup;
      _doc.UseAllAvailableDataRows = UseAllAvailableDataRows;

      _doc.RowHeaderColumn = (IReadableColumn)ParticipatingXColumn?.Tag;
      _doc.SetDataColumns(ParticipatingVColumns.Select(n => ReadableColumnProxy.FromColumn(((IReadableColumn)n.Tag))));

      _doc.SetColumnHeaderColumns(ParticipatingYColumns.Select(n => ReadableColumnProxy.FromColumn(((IReadableColumn)n.Tag))));

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

    private void EhUseSelectedItemAsXColumn()
    {
      var node = AvailableColumns.FirstSelectedNode;
      ParticipatingXColumn = new SelectableListNode(node.Text, node.Tag, false);
    }

    private void EhUseSelectedItemAsYColumns()
    {
      foreach (var node in AvailableColumns.Where(n => n.IsSelected))
      {
        var colToAdd = node.Tag as IReadableColumn;
        if (colToAdd is null)
          continue;

        // before adding this node, check that it is not already present
        if (ParticipatingYColumns.Any(n => n.Text == node.Text))
          continue;

        ParticipatingYColumns.Add(new SelectableListNode(node.Text, colToAdd, false));
      }
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

    private void EhClearYColumns()
    {
      if (ParticipatingYColumns.FirstSelectedNode is not null) // if anything selected, clear only the selected nodes
      {
        ParticipatingYColumns.RemoveSelectedItems();
      }
      else // if nothing selected, clear all nodes
      {
        ParticipatingYColumns.Clear();
      }
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
