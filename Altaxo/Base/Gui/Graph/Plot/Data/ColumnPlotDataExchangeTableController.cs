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
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Plot.Data
{
  #region Interfaces

  public interface IColumnPlotDataExchangeTableView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  [ExpectedTypeOfView(typeof(IColumnPlotDataExchangeTableView))]
  [UserControllerForObject(typeof(ColumnPlotDataExchangeTableData))]
  public class ColumnPlotDataExchangeTableController
    :
      MVCANControllerEditOriginalDocBase<ColumnPlotDataExchangeTableData, IColumnPlotDataExchangeTableView>
  {
    #region Members

    protected bool _isDirty = false;

    /// <summary>Tasks which updates the _fittingTables.</summary>
    protected Task _updateMatchingTablesTask;

    /// <summary>TokenSource to cancel the tasks which updates the _fittingTables.</summary>
    protected CancellationTokenSource _updateMatchingTablesTaskCancellationTokenSource = new CancellationTokenSource();

    #endregion Members

    #region Infrastructur Dispose and GetSubControllers

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<DataTable> _availableTables;

    /// <summary>All datatables of the document</summary>
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

    private ItemsController<DataTable> _matchingTables;

    /// <summary>Tuples from tables and group numbers, for which the columns in that group contain all that column names which are currently plot columns in our controller.</summary>
    public ItemsController<DataTable> MatchingTables
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

    private string _diagnosticsNumberOfPlotItemsText = "-x plot items changed";

    public string DiagnosticsNumberOfPlotItemsText
    {
      get => _diagnosticsNumberOfPlotItemsText;
      set
      {
        if (!(_diagnosticsNumberOfPlotItemsText == value))
        {
          _diagnosticsNumberOfPlotItemsText = value;
          OnPropertyChanged(nameof(DiagnosticsNumberOfPlotItemsText));
        }
      }
    }

    private string _diagnosticsNumberOfSuccessfullyChangedColumnsText = "- x columns successfully exchanged";

    public string DiagnosticsNumberOfSuccessfullyChangedColumnsText
    {
      get => _diagnosticsNumberOfSuccessfullyChangedColumnsText;
      set
      {
        if (!(_diagnosticsNumberOfSuccessfullyChangedColumnsText == value))
        {
          _diagnosticsNumberOfSuccessfullyChangedColumnsText = value;
          OnPropertyChanged(nameof(DiagnosticsNumberOfSuccessfullyChangedColumnsText));
        }
      }
    }

    private bool _DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible;

    public bool DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible
    {
      get => _DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible;
      set
      {
        if (!(_DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible == value))
        {
          _DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible = value;
          OnPropertyChanged(nameof(DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible));
        }
      }
    }


    private string _DiagnosticsNumberOfUnsuccessfullyChangedColumnsText = "- x columns failed to exchange";

    public string DiagnosticsNumberOfUnsuccessfullyChangedColumnsText
    {
      get => _DiagnosticsNumberOfUnsuccessfullyChangedColumnsText;
      set
      {
        if (!(_DiagnosticsNumberOfUnsuccessfullyChangedColumnsText == value))
        {
          _DiagnosticsNumberOfUnsuccessfullyChangedColumnsText = value;
          OnPropertyChanged(nameof(DiagnosticsNumberOfUnsuccessfullyChangedColumnsText));
        }
      }
    }

    private bool _DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible;

    public bool DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible
    {
      get => _DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible;
      set
      {
        if (!(_DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible == value))
        {
          _DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible = value;
          OnPropertyChanged(nameof(DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _availableTables = null;

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

    #region Initialize, Apply, Attach, Detach

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _doc.NewTable = _doc.OriginalTable;

        // Initialize tables
        string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

        var availableTables = new SelectableListNodeList();
        DataTable tg = _doc.OriginalTable;
        foreach (var tableName in tables)
        {
          availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], tg is not null && tg.Name == tableName));
        }
        AvailableTables = new ItemsController<DataTable>(availableTables, EhView_TableSelectionChanged);

        TriggerUpdateOfMatchingTables();

        Diagnostics_Initialize(0, 0, 0);
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    public void Diagnostics_Initialize(int numberOfPlotItems, int numberOfSuccessfullyChangedColumns, int numberOfUnsuccessfullyChangedColumns)
    {
      string text1, text2, text3;

      if (0 == numberOfPlotItems)
        text1 = "- No plot items with exchanged tables";
      else if (1 == numberOfPlotItems)
        text1 = "- One plot item with an exchanged table";
      else
        text1 = string.Format(Altaxo.Settings.GuiCulture.Instance, "- {0} plot items with exchanged tables", numberOfPlotItems);

      if (0 == numberOfSuccessfullyChangedColumns)
        text2 = null;
      else if (1 == numberOfSuccessfullyChangedColumns)
        text2 = "- One successfully changed column";
      else
        text2 = string.Format(Altaxo.Settings.GuiCulture.Instance, "- {0} successfully changed columns", numberOfSuccessfullyChangedColumns);

      if (0 == numberOfUnsuccessfullyChangedColumns)
        text3 = null;
      else if (1 == numberOfUnsuccessfullyChangedColumns)
        text3 = "- One column could not be replaced!";
      else
        text3 = string.Format(Altaxo.Settings.GuiCulture.Instance, "- {0} columns could not be replaced!", numberOfUnsuccessfullyChangedColumns);

      DiagnosticsNumberOfPlotItemsText = text1;
      DiagnosticsNumberOfSuccessfullyChangedColumnsText = text2;
      DiagnosticsNumberOfUnsuccessfullyChangedColumnsText = text3;

      DiagnosticsNumberOfSuccessfullyChangedColumnsIsVisible = text2 is not null;
      DiagnosticsNumberOfUnsuccessfullyChangedColumnsIsVisible = text3 is not null;
    }

    #endregion Initialize, Apply, Attach, Detach

    #region AvailableDataTables

    public void EhView_TableSelectionChanged(DataTable tg)
    {
      if (tg is null || object.ReferenceEquals(_doc.NewTable, tg))
        return;

      _doc.NewTable = tg;
      UpdateDiagnostics();
    }

    #endregion AvailableDataTables

    #region MatchingDataTables

    // Matching data tables are those tables, which have at least one group of columns which best fits the existing plot item column names

    /// <summary>
    /// Occurs if the selection for the matching tables has changed.
    /// </summary>
    public void EhView_MatchingTableSelectionChanged(DataTable tag)
    {
      if (object.ReferenceEquals(_doc.NewTable, tag)) // then nothing will change
        return;

      _doc.NewTable = tag;
      UpdateDiagnostics();
     // _availableTables.SelectedValue = _doc.NewTable;
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

    private void UpdateMatchingTables(System.Threading.CancellationToken cancellationToken)
    {
      var fittingTables2 = new SelectableListNodeList(); // we always update a new list, because _fittingTable1 is bound to the UI

      foreach (var table in GetTablesThatFitExistingGroupsOfPlotColumns(_doc.ColumnNames.Select(g => g.columnNames), cancellationToken))
      {
        fittingTables2.Add(new SelectableListNode(
          table.Name,
          table,
          object.ReferenceEquals(table, _doc.NewTable)));
      }

      MatchingTables = new ItemsController<DataTable>(fittingTables2, EhView_MatchingTableSelectionChanged);
    }

    /// <summary>
    /// Gets all tables with column names that match all groups of column names provided in <paramref name="groupsOfColumnNames"/>.
    /// Each group of column names must exist in one specific column group of the table.
    /// </summary>
    /// <param name="groupsOfColumnNames">The groups of column names.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static IEnumerable<DataTable> GetTablesThatFitExistingGroupsOfPlotColumns(IEnumerable<IEnumerable<string>> groupsOfColumnNames, System.Threading.CancellationToken cancellationToken)
    {
      HashSet<DataTable> result = null;

      foreach (var columnNames in groupsOfColumnNames)
      {
        if (result is null)
        {
          result = new HashSet<DataTable>(GetTablesWithGroupThatFitExistingPlotColumns(columnNames, cancellationToken).Select(n => n.dataTable));
        }
        else
        {
          result.IntersectWith(GetTablesWithGroupThatFitExistingPlotColumns(columnNames, cancellationToken).Select(n => n.dataTable));
        }
      }

      return result;
    }

    /// <summary>
    /// Gets all tables together with the group number which have a group with column names that match the provided column names in <paramref name="columnNames"/>.
    /// </summary>
    /// <param name="columnNames">The column names to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static IEnumerable<(DataTable dataTable, int groupNumber)> GetTablesWithGroupThatFitExistingPlotColumns(IEnumerable<string> columnNames, System.Threading.CancellationToken cancellationToken)
    {
      var columnNamesThatMustFit = columnNames is HashSet<string> ? (HashSet<string>)columnNames : new HashSet<string>(columnNames);

      if (cancellationToken.IsCancellationRequested)
        yield break;

      if (columnNamesThatMustFit.Count == 0)
        yield break; // we decide here that when there is no column that must fit, we return no table, because then we can use the all available tables combobox anyway.

      // now we iterate through all tables to find tables which can fullfil our criterium

      foreach (var table in Current.Project.DataTableCollection)
      {
        var groupNumbersAll = table.DataColumns.GetGroupNumbersAll();

        foreach (var groupNumber in groupNumbersAll)
        {
          if (cancellationToken.IsCancellationRequested)
            yield break;

          var columnNamesExisting = new HashSet<string>(columnNamesThatMustFit); // make a copy of this

          // and now eliminate all columns that also exist in this table
          foreach (var name in table.DataColumns.GetNamesOfColumnsWithGroupNumber(groupNumber))
          {
            if (columnNamesExisting.Remove(name) && columnNamesExisting.Count == 0)
            {
              yield return (table, groupNumber); // Count is null, so this is a fitting table
              break;
            }
          }
        }
      }
    }

    #endregion MatchingDataTables

    private void UpdateDiagnostics()
    {
      (int NumberOfPlotItemsChanged, int NumberOfSuccessFullyChangedColumns, int NumberOfUnsuccessfullyChangedColumns) = _doc.TestChangeTableForPlotItems();
      Diagnostics_Initialize(NumberOfPlotItemsChanged, NumberOfSuccessFullyChangedColumns, NumberOfUnsuccessfullyChangedColumns);
    }
  }
}
