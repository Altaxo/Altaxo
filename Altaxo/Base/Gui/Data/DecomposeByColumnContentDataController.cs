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

namespace Altaxo.Gui.Data
{
  using Altaxo.Collections;
  using Altaxo.Data;

  public interface IDecomposeByColumnContentDataView
  {
    void InitializeCyclingVariableColumn(SelectableListNodeList list);

    event Action SelectedTableChanged;

    event Action SelectedGroupNumberChanged;

    event Action UseSelectedAvailableColumnsAsParticipatingColumns;

    event Action DeleteSelectedParticipatingColumn;

    void InitializeAvailableTables(SelectableListNodeList items);

    int GroupNumber { get; set; }

    void InitializeAvailableColumns(SelectableListNodeList items);

    void InitializeParticipatingColumns(SelectableListNodeList items);
  }

  [ExpectedTypeOfView(typeof(IDecomposeByColumnContentDataView))]
  public class DecomposeByColumnContentDataController : MVCANControllerEditOriginalDocBase<DataTableMultipleColumnProxy, IDecomposeByColumnContentDataView>
  {
    private SelectableListNodeList _choicesCyclingVar = new SelectableListNodeList();
    private SelectableListNodeList _choicesColsToAverage = new SelectableListNodeList();
    private SelectableListNodeList _valueColumns = new SelectableListNodeList();
    private SelectableListNodeList _availableTables = new SelectableListNodeList();
    private SelectableListNodeList _availableColumns = new SelectableListNodeList();

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _choicesCyclingVar = null;
      _choicesColsToAverage = null;
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
        DecomposeByColumnContentDataAndOptions.EnsureCoherence(_doc, false);

        InitChoicesCyclingVariableAndChoicesAverageColumns();

        InitParticipatingColumns();

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
      }
      if (null != _view)
      {
        _view.InitializeCyclingVariableColumn(_choicesCyclingVar);

        _view.InitializeAvailableTables(_availableTables);
        _view.InitializeAvailableColumns(_availableColumns);

        _view.GroupNumber = _doc.GroupNumber;
        _view.InitializeParticipatingColumns(_valueColumns);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.DataTable = _availableTables.FirstSelectedNode.Tag as DataTable;
      _doc.GroupNumber = _view.GroupNumber;

      _doc.SetDataColumns(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier, _valueColumns.Select(n => (IReadableColumnProxy)n.Tag));

      _doc.SetDataColumn(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier, (DataColumn)_choicesCyclingVar.FirstSelectedNode.Tag);

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.SelectedTableChanged += EhSelectedTableChanged;
      _view.SelectedGroupNumberChanged += EhSelectedGroupNumberChanged;
      _view.UseSelectedAvailableColumnsAsParticipatingColumns += EhUseSelectedItemAsVColumns;
      _view.DeleteSelectedParticipatingColumn += EhClearVColumns;
    }

    protected override void DetachView()
    {
      _view.SelectedTableChanged -= EhSelectedTableChanged;
      _view.SelectedGroupNumberChanged -= EhSelectedGroupNumberChanged;
      _view.UseSelectedAvailableColumnsAsParticipatingColumns -= EhUseSelectedItemAsVColumns;
      _view.DeleteSelectedParticipatingColumn -= EhClearVColumns;

      base.DetachView();
    }

    private void InitParticipatingColumns()
    {
      // Initialize value columns
      _valueColumns.Clear();
      var columnProxies = _doc.GetDataColumnProxies(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier);
      foreach (var colProxy in columnProxies)
      {
        _valueColumns.Add(new SelectableListNode(colProxy.Document() != null ? colProxy.Document().FullName : "Unresolved column", colProxy.Clone(), false)); // clone of colProxy is important for apply later on
      }

      if (null != _view)
      {
        _view.InitializeParticipatingColumns(_valueColumns);
      }
    }

    private void AddAllColumnsOfGroupToParticipatingColumns()
    {
      var columnsInGroup = _doc.DataTable.DataColumns.Columns.Where(col => _doc.GroupNumber == _doc.DataTable.DataColumns.GetColumnGroup(col));
      _doc.SetDataColumns(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier, columnsInGroup);
    }

    private void InitChoicesCyclingVariableAndChoicesAverageColumns()
    {
      var srcData = _doc.DataTable.DataColumns;
      var columnsToProcess = _doc.GetDataColumns(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier);
      DataColumn columnWithCyclingVariable = _doc.GetDataColumnOrNull(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier);

      _choicesCyclingVar.Clear();
      foreach (var col in srcData.Columns)
        _choicesCyclingVar.Add(new SelectableListNode(srcData.GetColumnName(col), col, object.ReferenceEquals(columnWithCyclingVariable, col)));

      if (null != _view)
      {
        _view.InitializeCyclingVariableColumn(_choicesCyclingVar);
      }
    }

    private void FillAvailableColumnList()
    {
      _availableColumns.Clear();

      DataTable tg = _doc.DataTable;

      if (null != tg)
      {
        for (int i = 0; i < tg.DataColumnCount; ++i)
        {
          if (tg.DataColumns.GetColumnGroup(i) == _doc.GroupNumber)
            _availableColumns.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
        }
      }

      if (null != _view)
      {
        _view.InitializeAvailableColumns(_availableColumns);
      }
    }

    private DataColumn GetColumnInOtherTable(DataTable table, int groupNumber, IReadableColumnProxy proxyTemplate)
    {
      var oldColumn = (DataColumn)proxyTemplate.Document();

      if (null != oldColumn)
      {
        // first by name, then by position

        var parentColl = DataColumnCollection.GetParentDataColumnCollectionOf(oldColumn);
        var oldName = parentColl.GetColumnName(oldColumn);
        var oldPos = parentColl.GetColumnNumber(oldColumn);
        DataColumn newCol = null;

        if (table.DataColumns.ContainsColumn(oldName))
          newCol = table.DataColumns[oldName];
        else if (oldPos < table.DataColumns.ColumnCount)
          newCol = table.DataColumns[oldPos];

        if (null != newCol && table.DataColumns.GetColumnGroup(newCol) == groupNumber)
          return newCol;
      }
      else // no document available, try it with the path name
      {
        var documentPath = proxyTemplate.DocumentPath();
        var oldName = documentPath.LastPart;
        DataColumn newCol = null;

        if (table.DataColumns.ContainsColumn(oldName))
          newCol = table.DataColumns[oldName];

        if (null != newCol && table.DataColumns.GetColumnGroup(newCol) == groupNumber)
          return newCol;
      }

      return null;
    }

    private void ChangeDocumentToTableAndGroup(DataTable table, int groupNumber)
    {
      var newDoc = new DataTableMultipleColumnProxy(table, groupNumber);

      newDoc.EnsureExistenceOfIdentifier(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier, 1); // because of the 1 argument, we can not set it below
      foreach (var identifier in new[] { DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier, DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier })
      {
        newDoc.EnsureExistenceOfIdentifier(identifier);
        var columns = _doc.GetDataColumnProxies(identifier)
                      .Select(proxy => GetColumnInOtherTable(table, groupNumber, proxy)) // look up column in other table
                      .Where(col => col != null);                                       // select all columns that are not null

        newDoc.SetDataColumns(identifier, columns);
      }

      _doc = newDoc;

      FillAvailableColumnList();

      // if after the change there is no column participating, add all columns of the group
      if (null == _doc.GetDataColumns(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier).FirstOrDefault())
        AddAllColumnsOfGroupToParticipatingColumns();

      InitParticipatingColumns();

      InitChoicesCyclingVariableAndChoicesAverageColumns();
    }

    private void EhSelectedTableChanged()
    {
      var node = _availableTables.FirstSelectedNode;
      if (node == null)
        return;

      var table = (DataTable)node.Tag;

      if (object.ReferenceEquals(_doc.DataTable, table))
        return;

      ChangeDocumentToTableAndGroup(table, _view.GroupNumber);
    }

    private void EhSelectedGroupNumberChanged()
    {
      ChangeDocumentToTableAndGroup(_doc.DataTable, _view.GroupNumber);
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
