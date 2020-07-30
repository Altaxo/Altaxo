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
      if (null == table || null == table.DataSource)
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

      if (null != controllerAsSupportApplyCallback)
      {
        controllerAsSupportApplyCallback.SuccessfullyApplied += () => { sourceIsChanged = true; table.DataSource = dataSource; RequeryTableDataSource(ctrl); };
      }

      var result = Current.Gui.ShowDialog(dataSourceController, "Edit data source " + dataSource.GetType().ToString(), true);

      if (result == false) // user has cancelled the dialog
      {
        if (sourceIsChanged) // if source is changed, revert it
        {
          table.DataSource = originalDataSource;
          RequeryTableDataSource(ctrl);
        }
        return;
      }

      if (!sourceIsChanged) // controller might have forgotten to implement the SuccessfullyApplied event - thus we have to apply here
      {
        table.DataSource = dataSource;
        RequeryTableDataSource(ctrl);
      }
    }

    /// <summary>
    /// Requeries the table data source.
    /// </summary>
    /// <param name="ctrl">The controller that controls the data table.</param>
    public static void RequeryTableDataSource(WorksheetController ctrl)
    {
      RequeryTableDataSource(ctrl.DataTable);
    }

    /// <summary>
    /// Requeries the table data source.
    /// </summary>
    /// <param name="table">The table that holds the data source.</param>
    public static void RequeryTableDataSource(DataTable table)
    {
      if (null == table || null == table.DataSource)
        return;

      using (var suspendToken = table.SuspendGetToken())
      {
        try
        {
          table.DataSource.FillData(table);
        }
        catch (Exception ex)
        {
          table.Notes.WriteLine("Error during requerying the table data source: {0}", ex.Message);
          table.Notes.WriteLine("Details of exception:");
          table.Notes.WriteLine(ex.ToString());
        }

        if (!(null != table.DataSource))
          throw new InvalidProgramException("table.DataSource.FillData should never set the data source to zero!");

        if (table.DataSource.ImportOptions.ExecuteTableScriptAfterImport && null != table.TableScript)
        {
          try
          {
            table.TableScript.Execute(table, new Altaxo.Main.Services.DummyBackgroundMonitor(), false);
          }
          catch (Exception ex)
          {
            table.Notes.WriteLine("Error during execution of the table script (after requerying the table data source: {0}", ex.Message);
          }
        }

        suspendToken.Resume();
      }
    }
  }
}
