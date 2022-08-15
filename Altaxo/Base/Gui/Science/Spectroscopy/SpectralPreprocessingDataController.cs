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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Science.Spectroscopy
{
  using System.Collections.ObjectModel;
  using System.Windows.Input;
  using Altaxo.Collections;
  using Altaxo.Data;
  using Altaxo.Gui.Common;
  using Altaxo.Science.Spectroscopy;

  public interface ISpectralPreprocessingDataView : IDataContextAwareView  {  }

  [ExpectedTypeOfView(typeof(ISpectralPreprocessingDataView))]
  public class SpectralPreprocessingDataController : MVCANControllerEditOriginalDocBase<DataTableMultipleColumnProxy, ISpectralPreprocessingDataView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public SpectralPreprocessingDataController()
    {
      CmdAddToParticipatingColumns = new RelayCommand(EhAddToParticipatingColumns);
      CmdRemoveFromParticipatingColumns = new RelayCommand(EhRemoveFromParticipatingColumns);
      CmdParticipatingColumnsUp = new RelayCommand(EhParticipatingColumnsUp);
      CmdParticipatingColumnsDown = new RelayCommand(EhParticipatingColumnsDown);
    }

    #region Bindings

    public ICommand CmdAddToParticipatingColumns { get; }
    public ICommand CmdRemoveFromParticipatingColumns { get; }
    public ICommand CmdParticipatingColumnsUp { get; }
    public ICommand CmdParticipatingColumnsDown { get; }

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

    private ItemsController<DataColumn> _xColumn;

    public ItemsController<DataColumn> XColumn
    {
      get => _xColumn;
      set
      {
        if (!(_xColumn == value))
        {
          _xColumn = value;
          OnPropertyChanged(nameof(XColumn));
        }
      }
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

    private SelectableListNodeList _selectedColumns;

    public SelectableListNodeList ParticipatingColumns
    {
      get => _selectedColumns;
      set
      {
        if (!(_selectedColumns == value))
        {
          _selectedColumns = value;
          OnPropertyChanged(nameof(ParticipatingColumns));
        }
      }
    }


    #endregion

    

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
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
      }
    }

    private void EhSelectedTableChanged(DataTable obj)
    {
      var groupNumber = AvailableGroups is not null ? SelectedGroup : _doc.GroupNumber;

      // Initialize group numbers
      var availableGroups = _doc.DataTable.DataColumns.GetGroupNumbersAll();
      AvailableGroups = new ObservableCollection<int>(availableGroups);

      if (availableGroups.Contains(groupNumber))
      {
        SelectedGroup = groupNumber;
        EhSelectedGroupNumberChanged(groupNumber);
      }
      else if(availableGroups.Count>0)
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
        columnDict.Add(DataColumnCollection.GetParentDataColumnCollectionOf(c).GetColumnName(c), c);


      // X-Column
      var xCol = XColumn?.SelectedValue ??
                (_doc.ContainsIdentifier(SpectroscopyCommands.ColumnX) ? _doc.GetDataColumns(SpectroscopyCommands.ColumnX).FirstOrDefault() : null);
      string xColName = xCol is null ? null : DataColumnCollection.GetParentDataColumnCollectionOf(xCol).GetColumnName(xCol);


      XColumn = new ItemsController<DataColumn>(
        new SelectableListNodeList(
          columnList.Select(
            c => new SelectableListNode(table.DataColumns.GetColumnName(c), c, false)
            )
          )
        );

      if (xCol is not null && table.DataColumns.ContainsColumn(xCol) && table.DataColumns.GetColumnGroup(xCol) == groupNumber)
        XColumn.SelectedValue = xCol;
      if (!string.IsNullOrEmpty(xColName))
        XColumn.SelectedValue = XColumn.Items.Where(n => n.Text == xColName).Select(n => (DataColumn)n.Tag).FirstOrDefault();
      else
        XColumn.SelectedValue = null;


      // Available columns
      AvailableColumns =
         new SelectableListNodeList(
           columnList.Select(c => new SelectableListNode(
             table.DataColumns.GetColumnName(c),
             c,
             false))
         );


      // Participating columns
      DataColumn[] participatingColumns;
      if (ParticipatingColumns is null)
      {
        participatingColumns = _doc.GetDataColumns(SpectroscopyCommands.ColumnsV).ToArray();
      }
      else
      {
        participatingColumns = ParticipatingColumns.Where(n => n.IsSelected).Select(n => (DataColumn)n.Tag).ToArray();
      }
      var participatingColNames = participatingColumns.Select(c => DataColumnCollection.GetParentDataColumnCollectionOf(c).GetColumnName(c)).ToArray();

      var newParticipatingColumns = participatingColumns.Where(
        c => table.DataColumns.Contains(c) && groupNumber == table.DataColumns.GetColumnGroup(c)).ToArray();

      ParticipatingColumns = new SelectableListNodeList(
        participatingColNames.Where(cn => columnDict.ContainsKey(cn)).Select(cn => new SelectableListNode(cn, columnDict[cn], false)));
}

    public override bool Apply(bool disposeController)
    {
      var dataTable = DataTable.SelectedValue;
      if(dataTable is null)
      {
        Current.Gui.ErrorMessageBox("Please select a data table");
        return ApplyEnd(false, disposeController);
      }
      var groupNumber = SelectedGroup;

      var xCol = XColumn.SelectedValue;

      if(xCol is null)
      {
        Current.Gui.ErrorMessageBox("Please select an x-column!");
        return ApplyEnd(false, disposeController);
      }

      var yCol = ParticipatingColumns.Select(n => (DataColumn)n.Tag).ToArray();

      if(yCol.Length==0)
      {
        Current.Gui.ErrorMessageBox("Please select at least one participating column!");
        return ApplyEnd(false, disposeController);
      }

      _doc = new DataTableMultipleColumnProxy(dataTable, groupNumber);
      _doc.EnsureExistenceOfIdentifier(SpectroscopyCommands.ColumnX, 1);
      _doc.EnsureExistenceOfIdentifier(SpectroscopyCommands.ColumnsV);
      _doc.SetDataColumn(SpectroscopyCommands.ColumnX, XColumn.SelectedValue);
      _doc.SetDataColumns(SpectroscopyCommands.ColumnsV, ParticipatingColumns.Select(n => (DataColumn)n.Tag));

      return ApplyEnd(true, disposeController);
    }

    private void EhAddToParticipatingColumns()
    {
      ParticipatingColumns.AddRange(AvailableColumns.Where(n => n.IsSelected).Select(n => new SelectableListNode(n.Text, n.Tag, true)));
    }

    private void EhRemoveFromParticipatingColumns()
    {
      ParticipatingColumns.RemoveSelectedItems();
    }

    private void EhParticipatingColumnsUp()
    {
      ParticipatingColumns.MoveSelectedItemsUp();
    }

    private void EhParticipatingColumnsDown()
    {
      ParticipatingColumns.MoveSelectedItemsDown();
    }
  }
}
