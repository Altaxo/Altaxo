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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  public interface IDataTableMatrixProxyView
  {
    event Action SelectedTableChanged;

    event Action SelectedGroupNumberChanged;

    event Action UseAllAvailableDataColumnsChanged;

    event Action UseAllAvailableDataRowsChanged;

    event Action SelectedColumnKindChanged;

    event Action UseSelectedItemAsXColumn;

    event Action UseSelectedItemAsYColumn;

    event Action UseSelectedItemAsVColumns;

    event Action ClearXColumn;

    event Action ClearYColumn;

    event Action ClearVColumns;

    /// <summary>Gets a value indicating whether data columns or property columns are shown in the view.</summary>
    /// <value><see langword="true"/> if data columns are shown; otherwise, <see langword="false"/>.</value>
    bool AreDataColumnsShown { get; }

    void InitializeAvailableTables(SelectableListNodeList items);

    void InitializeAvailableColumns(SelectableListNodeList items);

    void Initialize_XColumn(string colname);

    void Initialize_YColumn(string colname);

    void Initialize_VColumns(SelectableListNodeList items);

    void EnableUseButtons(bool enableUseAsXColumn, bool enableUseAsYColumn, bool enableUseAsVColumns);

    void Initialize_DataRowsControl(object obj);

    int GroupNumber { get; set; }

    bool UseAllAvailableDataColumns { get; set; }

    bool UseAllAvailableDataRows { get; set; }
  }

  [ExpectedTypeOfView(typeof(IDataTableMatrixProxyView))]
  [UserControllerForObject(typeof(DataTableMatrixProxy))]
  public class DataTableMatrixProxyController : MVCANControllerEditOriginalDocBase<DataTableMatrixProxy, IDataTableMatrixProxyView>
  {
    private Altaxo.Data.IReadableColumn _xColumn;
    private Altaxo.Data.IReadableColumn _yColumn;
    private SelectableListNodeList _valueColumns = new SelectableListNodeList();

    private int _maxPossiblePlotRangeTo;

    private SelectableListNodeList _availableTables = new SelectableListNodeList();
    private SelectableListNodeList _availableColumns = new SelectableListNodeList();
    private bool _areDataColumnsShown = true;

    private Altaxo.Gui.Common.AscendingIntegerCollectionController _rowsController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rowsController, () => _rowsController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _xColumn = null;
      _yColumn = null;
      _valueColumns = null;
      _availableTables = null;
      _availableColumns = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _xColumn = _doc.RowHeaderColumn;
        _yColumn = _doc.ColumnHeaderColumn;
        // Initialize value columns
        _valueColumns.Clear();
        for (int i = 0; i < _doc.ColumnCount; ++i)
        {
          var col = _doc.GetDataColumnProxy(i);
          _valueColumns.Add(new SelectableListNode(col.Document() != null ? col.Document().FullName : "Unresolved column", col, false));
        }

        CalcMaxPossiblePlotRangeTo();

        // Initialize tables
        string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

        string dataTableName = _doc.DataTable == null ? string.Empty : _doc.DataTable.Name;

        _availableTables.Clear();
        foreach (var tableName in tables)
        {
          _availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], dataTableName == tableName));
        }

        // Initialize columns
        FillAvailableColumnList();

        _rowsController = new Common.AscendingIntegerCollectionController();
        _rowsController.InitializeDocument(_doc.ParticipatingDataRows.Clone());
      }

      if (null != _view)
      {
        EhSelectedColumnKindChanged(); // ask view which column kind is now selected
        UpdateButtonEnablingInView(); // do that in every case, even if nothing has changed

        _view.InitializeAvailableTables(_availableTables);
        _view.InitializeAvailableColumns(_availableColumns);

        _view.GroupNumber = _doc.GroupNumber;
        _view.UseAllAvailableDataColumns = _doc.UseAllAvailableDataColumnsOfGroup;
        _view.UseAllAvailableDataRows = _doc.UseAllAvailableDataRows;

        _view.Initialize_XColumn(_xColumn == null ? string.Empty : _xColumn.FullName);
        _view.Initialize_YColumn(_yColumn == null ? string.Empty : _yColumn.FullName);
        _view.Initialize_VColumns(_valueColumns);
        CalcMaxPossiblePlotRangeTo();

        if (_rowsController.ViewObject == null)
          Current.Gui.FindAndAttachControlTo(_rowsController);
        _view.Initialize_DataRowsControl(_rowsController.ViewObject);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.DataTable = _availableTables.FirstSelectedNode.Tag as DataTable;
      _doc.GroupNumber = _view.GroupNumber;
      _doc.UseAllAvailableDataColumnsOfGroup = _view.UseAllAvailableDataColumns;
      _doc.UseAllAvailableDataRows = _view.UseAllAvailableDataRows;

      _doc.RowHeaderColumn = _xColumn;
      _doc.ColumnHeaderColumn = _yColumn;
      _doc.SetDataColumns(_valueColumns.Select(n => (IReadableColumnProxy)n.Tag));

      if (!_doc.UseAllAvailableDataRows)
      {
        if (!_rowsController.Apply(disposeController))
          return false;
        _doc.SetDataRows((IAscendingIntegerCollection)_rowsController.ModelObject);
      }

      var tempView = ViewObject;
      ViewObject = null;
      Initialize(true);
      ViewObject = tempView;

      return ApplyEnd(true, disposeController); // successfull
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.SelectedTableChanged += EhSelectedTableChanged;
      _view.SelectedColumnKindChanged += EhSelectedColumnKindChanged;
      _view.UseSelectedItemAsXColumn += EhUseSelectedItemAsXColumn;
      _view.UseSelectedItemAsYColumn += EhUseSelectedItemAsYColumn;
      _view.UseSelectedItemAsVColumns += EhUseSelectedItemAsVColumns;
      _view.ClearXColumn += EhClearXColumn;
      _view.ClearYColumn += EhClearYColumn;
      _view.ClearVColumns += EhClearVColumns;
    }

    protected override void DetachView()
    {
      _view.SelectedTableChanged -= EhSelectedTableChanged;
      _view.SelectedColumnKindChanged -= EhSelectedColumnKindChanged;
      _view.UseSelectedItemAsXColumn -= EhUseSelectedItemAsXColumn;
      _view.UseSelectedItemAsYColumn -= EhUseSelectedItemAsYColumn;
      _view.UseSelectedItemAsVColumns -= EhUseSelectedItemAsVColumns;
      _view.ClearXColumn -= EhClearXColumn;
      _view.ClearYColumn -= EhClearYColumn;
      _view.ClearVColumns -= EhClearVColumns;

      base.DetachView();
    }

    private void CalcMaxPossiblePlotRangeTo()
    {
      int len = int.MaxValue;
      if (_xColumn.Count.HasValue)
        len = Math.Min(len, _xColumn.Count.Value);
      if (_yColumn.Count.HasValue)
        len = Math.Min(len, _yColumn.Count.Value);

      _maxPossiblePlotRangeTo = len - 1;
    }

    private void FillAvailableColumnList()
    {
      _availableColumns.Clear();

      var node = _availableTables.FirstSelectedNode;
      DataTable tg = node == null ? null : node.Tag as DataTable;

      if (null != tg)
      {
        if (_areDataColumnsShown)
        {
          for (int i = 0; i < tg.DataColumnCount; ++i)
            _availableColumns.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
        }
        else
        {
          for (int i = 0; i < tg.PropertyColumnCount; ++i)
            _availableColumns.Add(new SelectableListNode(tg.PropertyColumns.GetColumnName(i), tg.PropertyColumns[i], false));
        }
      }

      if (null != _view)
      {
        _view.InitializeAvailableColumns(_availableColumns);
      }
    }

    private void UpdateButtonEnablingInView()
    {
      if (null != _view)
      {
        _view.EnableUseButtons(_areDataColumnsShown, !_areDataColumnsShown, _areDataColumnsShown);
      }
    }

    private void EhSelectedTableChanged()
    {
      FillAvailableColumnList();
    }

    private void EhSelectedColumnKindChanged()
    {
      var newValue = _view.AreDataColumnsShown;
      if (_areDataColumnsShown != newValue)
      {
        _areDataColumnsShown = newValue;
        FillAvailableColumnList();
        UpdateButtonEnablingInView();
      }
    }

    private void EhUseSelectedItemAsXColumn()
    {
      var node = _availableColumns.FirstSelectedNode;
      _xColumn = node == null ? null : node.Tag as DataColumn;
      if (null != _view)
        _view.Initialize_XColumn(_xColumn == null ? string.Empty : _xColumn.FullName);
    }

    private void EhUseSelectedItemAsYColumn()
    {
      var node = _availableColumns.FirstSelectedNode;
      _yColumn = node == null ? null : node.Tag as DataColumn;

      if (null != _view)
        _view.Initialize_YColumn(_yColumn == null ? string.Empty : _yColumn.FullName);
    }

    private void EhUseSelectedItemAsVColumns()
    {
      foreach (var node in _availableColumns.Where(n => n.IsSelected))
      {
        var colToAdd = node.Tag as IReadableColumn;
        if (colToAdd == null)
          continue;

        // before adding this node, check that it is not already present
        var proxyToAdd = ReadableColumnProxyBase.FromColumn(colToAdd);
        if (_valueColumns.Any(n => proxyToAdd.DocumentPath().Equals(((IReadableColumnProxy)n.Tag).DocumentPath())))
          continue;

        _valueColumns.Add(new SelectableListNode(colToAdd.FullName, proxyToAdd, false));
      }
    }

    private void EhClearXColumn()
    {
      _xColumn = null;
      if (null != _view)
        _view.Initialize_XColumn(_xColumn == null ? string.Empty : _xColumn.FullName);
    }

    private void EhClearYColumn()
    {
      _yColumn = null;
      if (null != _view)
        _view.Initialize_YColumn(_yColumn == null ? string.Empty : _yColumn.FullName);
    }

    private void EhClearVColumns()
    {
      if (null != _valueColumns.FirstSelectedNode) // if anything selected, clear only the selected nodes
      {
        _valueColumns.RemoveSelectedItems();
      }
      else // if nothing selected, clear all nodes
      {
        _valueColumns.Clear();
      }
    }
  }
}
