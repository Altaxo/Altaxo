#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Data;
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Worksheet.Commands
{
  public static class DataSourceCommands
  {
    /// <summary>
    /// Shows the data source editor dialog. After sucessful execution of the dialog, the modified data source is stored back in the <see cref="DataTable"/>, and the data source is requeried.
    /// </summary>
    /// <param name="ctrl">The controller that controls the data table.</param>
    public static void ShowDataSourceEditor(WorksheetController ctrl)
    {
      var table = ctrl.DataTable;
      if (table is null || table.DataSource is null)
        return;

      bool sourceIsChanged = false;
      var originalDataSource = table.DataSource;
      var dataSource = (Data.IAltaxoTableDataSource)table.DataSource.Clone();

      var dataSourceController = (Altaxo.Gui.IMVCANController?)Current.Gui.GetControllerAndControl(new object[] { dataSource }, typeof(Altaxo.Gui.IMVCANController), Gui.UseDocument.Directly);

      if (dataSourceController is null)
      {
        Current.Gui.ErrorMessageBox(string.Format("Sorry. There is no dialog available to edit the data source of type {0}", dataSource.GetType()), "No dialog available");
        return;
      }

      var controllerAsSupportApplyCallback = dataSourceController as Altaxo.Gui.IMVCSupportsApplyCallback;

      if (controllerAsSupportApplyCallback is not null)
      {
        controllerAsSupportApplyCallback.SuccessfullyApplied += () => { sourceIsChanged = true; table.DataSource = dataSource; ExecuteDataSourceOfTable(ctrl); };
      }

      var result = Current.Gui.ShowDialog(dataSourceController, "Edit data source " + dataSource.GetType().ToString(), true);

      if (result == false) // user has cancelled the dialog
      {
        if (sourceIsChanged) // if source is changed, revert it
        {
          table.DataSource = originalDataSource;
          ExecuteDataSourceOfTable(ctrl);
        }
        return;
      }

      if (!sourceIsChanged) // controller might have forgotten to implement the SuccessfullyApplied event - thus we have to apply here
      {
        table.DataSource = dataSource;
        ExecuteDataSourceOfTable(ctrl);
      }
    }

    /// <summary>
    /// Requeries the table data source.
    /// </summary>
    /// <param name="ctrl">The controller that controls the data table.</param>
    public static void ExecuteDataSourceOfTable(WorksheetController ctrl)
    {
      var progressMonitor = new ExternalDrivenBackgroundMonitor();

      var fitTask = System.Threading.Tasks.Task.Run(() =>
      {
        try
        {
          ExecuteDataSourceOfTable(ctrl.DataTable, progressMonitor);
        }
        catch (OperationCanceledException)
        {
        }
      }
      );

      Current.Gui.ShowTaskCancelDialog(1000, fitTask, progressMonitor);
    }


    /// <summary>
    /// Requeries the table data source.
    /// </summary>
    /// <param name="table">The table that holds the data source.</param>
    /// <param name="reporter">A reporter object that can be used to cancel, or to report the progress.</param>
    public static void ExecuteDataSourceOfTable(DataTable table, IProgressReporter? reporter)
    {
      if (table is null || table.DataSource is null)
        return;

      using (var suspendToken = table.SuspendGetToken())
      {
        try
        {
          table.DataSource.FillData(table, reporter);
        }
        catch (Exception ex)
        {
          table.Notes.WriteLine($"{DateTime.Now}: Exception during requerying the table data source: {ex.Message}");
          table.Notes.WriteLine("Details of this exception:");
          table.Notes.WriteLine(ex.ToString());
          table.Notes.WriteLine("--------------------------");
        }

        if (table.DataSource is null)
          throw new InvalidProgramException("table.DataSource.FillData should never set the data source to zero!");

        if (table.DataSource.ImportOptions.ExecuteTableScriptAfterImport && table.TableScript is not null)
        {
          try
          {
            table.TableScript.Execute(table, new Altaxo.Main.Services.DummyBackgroundMonitor(), false);
          }
          catch (Exception ex)
          {
            table.Notes.WriteLine($"{DateTime.Now}: Exception during execution of the table script (after requerying the table data source: {ex.Message}");
          }
        }

        suspendToken.Resume();
      }
    }

    /// <summary>
    /// This command will sort provided tables containing data sources in a way, that tables dependent on
    /// data of other tables come after the tables they are dependent on.
    /// </summary>
    /// <param name="dataTables">The data tables for which to execute the data sources.</param>
    /// <param name="sortedTables">If the return value is true, contains the list of sorted tables. Only tables containing data sources are member of the list. </param>
    /// <param name="errorMessage">If the return value is false, contains an error message indicating why the tables could not be sorted. This is the case if circular dependencies were detected.</param>
    /// <returns>True if the sorting was successful; otherwise, false (circular dependencies detected).</returns>
    public static bool TrySortTablesForExecutionOfAllDataSources(IEnumerable<DataTable> dataTables, out List<DataTable> sortedTables, out string errorMessage)
    {
      // find all selected tables with data sources in it
      var dataTableList = dataTables
        .Where(t => t.DataSource is not null)
        .ToList();

      if (dataTableList.Count == 0)
      {
        sortedTables = dataTableList;
        errorMessage = string.Empty;
        return true;
      }

      // dictionary with a table as key, and table(s) it depend on as values
      var dict = new Dictionary<DataTable, HashSet<DataTable>>();

      void MyReport(IProxy proxy, object owner, string propertyName, DataTable table)
      {
        var doc = proxy?.DocumentObject();
        if (doc is DataTable dt0)
          dict[table].Add(dt0);
        else if (doc is DataColumn dc && DataTable.GetParentDataTableOf(dc) is DataTable dt1)
          dict[table].Add(dt1);
      }

      DataTable[]? AddIndirectDependencies(DataTable masterTable)
      {
        var dependentChain = new List<DataTable>();
        var indirectDependencies = new HashSet<DataTable>();

        foreach (var subTable in dict[masterTable])
        {
          dependentChain.Add(subTable);
          var circular = AddIndirectDependencies3(masterTable, dependentChain, indirectDependencies);
          dependentChain.RemoveAt(dependentChain.Count - 1);
          if (circular is not null)
          {
            return circular;
          }
        }
        dict[masterTable] = indirectDependencies;
        return null;
      }

      DataTable[]? AddIndirectDependencies3(DataTable masterTable, List<DataTable> dependentChain, HashSet<DataTable> indirectDependencies)
      {
        var dependentTable = dependentChain[^1];
        if (object.ReferenceEquals(masterTable, dependentChain[^1]))
          return dependentChain.ToArray(); // circular reference

        indirectDependencies.Add(dependentTable);
        if (dict.TryGetValue(dependentTable, out var subDependencies))
        {
          foreach (var subDependend in subDependencies)
          {
            dependentChain.Add(subDependend);
            var circular = AddIndirectDependencies3(masterTable, dependentChain, indirectDependencies);
            dependentChain.RemoveAt(dependentChain.Count - 1);
            if (circular is not null)
              return circular;
          }
        }
        return null;
      }


      int TableSorting(DataTable t1, DataTable t2)
      {
        if (dict[t1].Contains(t2) && dict[t2].Contains(t1))
        {
          return 0; // this is a direct circular dependency, but we test for it later anyway
        }
        else if (dict[t1].Contains(t2))
        {
          return +1; // t1 should be shifted to a place after t2
        }
        else if (dict[t2].Contains(t1))
        {
          return -1; // t1 should be shifted to a place before t2
        }
        else
        {
          return Comparer<int>.Default.Compare(dict[t1].Count, dict[t2].Count); // not dependent on each other, thus neutral
        }
      }

      // now sort them according to their dependencies
      foreach (var t in dataTableList)
      {
        dict.Add(t, new HashSet<DataTable>());
        t.DataSource.VisitDocumentReferences((p, o, n) => MyReport(p, o, n, t));
      }

      // The dictionary of every table now contains the direct dependencies
      // but now add the indirect dependencies
      foreach (var masterT in dataTableList)
      {
        var circular = AddIndirectDependencies(masterT);
        if (circular is not null)
        {
          var dependencyChain = new StringBuilder();
          dependencyChain.Append(masterT.Name);
          for (int i = 0; i < circular.Length; ++i)
          {
            dependencyChain.Append(" <-- ");
            dependencyChain.Append(circular[i]);
          }


          errorMessage = "Some of the data sources of the tables are circular dependent on each other.\r\n" +
                          $"The dependency chain is: {dependencyChain}\r\n" +
                          "Thus, the command can not be executed";

          sortedTables = new List<DataTable>();
          return false;
        }
      }

      // Sort the table
      dataTableList.Sort((t1, t2) => TableSorting(t1, t2));

      // if the sorting above is not working properly, then uncomment this

      /*
      // Sorting safe, but maybe slower
      for (int i = 0; i < dataTables.Count; i++)
      {
        var t = dataTables[0];
        var dict_t = dict[t];

        for (int j = dataTables.Count; j > 1; --j)
        {
          if (dict_t.Contains(dataTables[j - 1]))
          {
            dataTables.Insert(j, t); // put this table after the last table it is dependent on
            dataTables.RemoveAt(0); // remove this table from the first element
            break;
          }
        }
      }
      */


      // Test for circular dependencies
      // if there is one, the sorting would not be perfect
      // so we test if the sorting is perfect

      // make an initial test set of all tables 
      var testSetOfTables = new HashSet<DataTable>(dataTableList);

      // the first table should not depend on any of the tables in the test set
      foreach (var t in dataTableList)
      {
        testSetOfTables.Remove(t); // we remove our own table from the test set, of course
        if (dict[t].Intersect(testSetOfTables).FirstOrDefault() is { } dependentTable)
        {
          errorMessage = "Some of the data sources of the tables are circular dependent on each other.\r\n" +
                                      $"For instance, table {t.Name} is dependent on {dependentTable.Name} and vice versa\r\n" +
                                      "Thus the command can not be executed";

          sortedTables = new List<DataTable>();
          return false;
        }
      }


      errorMessage = string.Empty;
      sortedTables = dataTableList;
      return true;
    }

    /// <summary>
    /// This command will executes all data sources in all provided tables, using the background execution dialog.
    /// For that, it takes into account the dependencies of each data source to other tables,
    /// and execute those sources first, which do not have dependencies to tables for which the data source
    /// is executed later.
    /// </summary>
    /// <param name="sortedTables">The data tables for which to execute the data sources. They must be sorted by dependence on the other tables (less dependent tables coming first, see <see cref="TrySortTablesForExecutionOfAllDataSources"/>).</param>
    /// <param name="reporter">The progress reporter that is showing the progress. If a reporter is provided, it is assumed that
    /// we run already on a background thread, thus, the data sources are executed in this thread.</param>
    public static void ExecuteDataSourcesOfTables(IEnumerable<DataTable> sortedTables, IProgressReporter? reporter = null)
    {
      if (sortedTables is null)
        throw new ArgumentNullException(nameof(sortedTables));
      if (!sortedTables.Any())
        return;

      bool alreadyOnBackgroundThread = reporter is not null; // if the reporter is not null, we assume that we are already on a background thread
      reporter ??= new Altaxo.Main.Services.ExternalDrivenBackgroundMonitor();


      void ThreadAction()
      {
        // now we can execute the data sources
        double idx = -1;
        int count = sortedTables.Count();
        foreach (var t in sortedTables)
        {
          idx += 1;
          if (reporter.CancellationPending)
            break;
          reporter.ReportProgress($"Execute data source of table {t.Name}", idx / (double)count);
          ExecuteDataSourceOfTable(t, reporter);
        }
      }


      if (alreadyOnBackgroundThread)
      {
        // if the reporter is not null, we assume that we are already on a background thread
        ThreadAction();
      }
      else
      {
        var thread = new System.Threading.Thread(ThreadAction);
        thread.Start();
        Current.Gui.ShowBackgroundCancelDialog(1000, thread, (Altaxo.Main.Services.ExternalDrivenBackgroundMonitor)reporter);
      }
    }
  }
}
