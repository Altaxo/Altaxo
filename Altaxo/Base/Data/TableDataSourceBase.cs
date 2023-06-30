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

#nullable enable
using System;

namespace Altaxo.Data
{
  public abstract class TableDataSourceBase :
    Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>
  {
    #region Change event handling

    /// <summary>
    /// Accumulates the change data of the child. Currently only a flag is set to signal that the table has changed.
    /// </summary>
    /// <param name="sender">The sender of the change notification (currently unused).</param>
    /// <param name="e">The change event args can provide details of the change (currently unused).</param>
    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      if (e is TableDataSourceChangedEventArgs) // DataSourceChangeEvent has highest priority, if this is set, no other change event is needed
        _accumulatedEventData = e;
      else if (_accumulatedEventData is null)
        _accumulatedEventData = EventArgs.Empty;
    }

    #endregion Change event handling

    public abstract void FillData_Unchecked(Altaxo.Data.DataTable destinationTable, IProgressReporter? reporter = null);
    public abstract IDataSourceImportOptions ImportOptions { get; set; }

    public string? FillData(DataTable destinationTable, IProgressReporter? reporter = null)
    {
      if (destinationTable is null)
        throw new ArgumentNullException(nameof(destinationTable));

      string? err = null;

      using (var suspendToken = SuspendGetToken())
      {
        try
        {
          FillData_Unchecked(destinationTable, reporter);

          try
          {
            if (ImportOptions.ExecuteTableScriptAfterImport && destinationTable.TableScript is not null)
              destinationTable.TableScript.ExecuteWithoutExceptionCatching(destinationTable, reporter ?? new Main.Services.DummyBackgroundMonitor());
          }
          catch (Exception ex)
          {
            err = $"{DateTime.Now} - Exception during execution of the table script (after execution of the data source). Details follow:\r\n{ex}";
            destinationTable.Notes.WriteLine(err);
            Current.Console.WriteLine($"{DateTime.Now} - Exception during execution of the table script (after execution of the data source) ({GetType().Name} of table {this.Name}): {ex.Message}");
          }
        }
        catch (Exception ex)
        {
          err = $"{DateTime.Now} - Error during execution of data source ({GetType().Name}), Details:\r\n{ex}";
          destinationTable.Notes.WriteLine(err);
          Current.Console.WriteLine($"{DateTime.Now} - Error during execution of data source ({GetType().Name} of table {this.Name}): {ex.Message}");
        }
      }
      return err;
    }
  }
}
