#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Gui.Data
{
  using System.Collections.ObjectModel;
  using Altaxo.Collections;
  using Altaxo.Data;
  using Altaxo.Gui.Common;

  public interface IDataTableXYColumnProxyView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IDataTableXYColumnProxyView))]
  [UserControllerForObject(typeof(DataTableXYColumnProxy))]
  public class DataTableXYColumnProxyController : MVCANControllerEditOriginalDocBase<DataTableXYColumnProxy, IDataTableXYColumnProxyView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

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

    private ItemsController<DataColumn> _yColumn;

    public ItemsController<DataColumn> YColumn
    {
      get => _yColumn;
      set
      {
        if (!(_yColumn == value))
        {
          _yColumn = value;
          OnPropertyChanged(nameof(YColumn));
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
        columnDict.Add(DataColumnCollection.GetParentDataColumnCollectionOf(c).GetColumnName(c), c);


      // X-Column
      var xCol = XColumn?.SelectedValue ?? _doc.XColumn;
      string xColName = xCol is not DataColumn xdc ? null : DataColumnCollection.GetParentDataColumnCollectionOf(xdc).GetColumnName(xdc);


      XColumn = new ItemsController<DataColumn>(
        new SelectableListNodeList(
          columnList.Select(
            c => new SelectableListNode(table.DataColumns.GetColumnName(c), c, false)
            )
          )
        );
      if (xCol is DataColumn xxdc && table.DataColumns.ContainsColumn(xxdc) && table.DataColumns.GetColumnGroup(xxdc) == groupNumber)
        XColumn.SelectedValue = xxdc;
      if (!string.IsNullOrEmpty(xColName))
        XColumn.SelectedValue = XColumn.Items.Where(n => n.Text == xColName).Select(n => (DataColumn)n.Tag).FirstOrDefault();
      else
        XColumn.SelectedValue = null;




      // Y-Column
      var yCol = YColumn?.SelectedValue ?? _doc.YColumn;
      string yColName = yCol is not DataColumn ydc ? null : DataColumnCollection.GetParentDataColumnCollectionOf(ydc).GetColumnName(ydc);


      YColumn = new ItemsController<DataColumn>(
        new SelectableListNodeList(
          columnList.Select(
            c => new SelectableListNode(table.DataColumns.GetColumnName(c), c, false)
            )
          )
        );
      if (yCol is DataColumn yydc && table.DataColumns.ContainsColumn(yydc) && table.DataColumns.GetColumnGroup(yydc) == groupNumber)
        YColumn.SelectedValue = yydc;
      if (!string.IsNullOrEmpty(yColName))
        YColumn.SelectedValue = YColumn.Items.Where(n => n.Text == yColName).Select(n => (DataColumn)n.Tag).FirstOrDefault();
      else
        YColumn.SelectedValue = null;
    }

    public override bool Apply(bool disposeController)
    {
      var dataTable = DataTable.SelectedValue;
      if (dataTable is null)
      {
        Current.Gui.ErrorMessageBox("Please select a data table");
        return ApplyEnd(false, disposeController);
      }
      var groupNumber = SelectedGroup;

      var xCol = XColumn.SelectedValue;

      if (xCol is null)
      {
        Current.Gui.ErrorMessageBox("Please select an x-column!");
        return ApplyEnd(false, disposeController);
      }

      var yCol = YColumn.SelectedValue;

      if (yCol is null)
      {
        Current.Gui.ErrorMessageBox("Please select an y-column!");
        return ApplyEnd(false, disposeController);
      }




      _doc = new DataTableXYColumnProxy(dataTable, xCol, yCol);
      return ApplyEnd(true, disposeController);
    }
  }
}
