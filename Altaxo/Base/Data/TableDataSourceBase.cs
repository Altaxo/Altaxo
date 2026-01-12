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
using System.Threading;

namespace Altaxo.Data
{
  /// <summary>
  /// Base class for table data sources.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This base class provides a default implementation of <c>FillData</c> that suspends notifications on the destination table,
  /// executes the derived <see cref="FillData_Unchecked"/> implementation and optionally runs the table script.
  /// </para>
  /// <para>
  /// A reentrancy guard ensures that concurrent or recursive calls to <see cref="FillData"/> do not execute the data source multiple times.
  /// </para>
  /// </remarks>
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

    private int _fillDataEntranceCounter;

    /// <summary>
    /// Fills (or refills) the destination table without exception catching.
    /// The table script is not executed by this method.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter">The progress reporter (can be null).</param>
    public abstract void FillData_Unchecked(Altaxo.Data.DataTable destinationTable, IProgressReporter reporter);

    /// <summary>
    /// Gets or sets the import options.
    /// </summary>
    public abstract IDataSourceImportOptions ImportOptions { get; set; }

    /// <summary>
    /// Fills (or refills) the destination table with exception catching.
    /// Optionally runs the table script depending on <see cref="ImportOptions"/>.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns>Null if no exception was caught during processing; otherwise an error message.</returns>
    public string? FillData(DataTable destinationTable, IProgressReporter reporter)
    {
      if (destinationTable is null)
        throw new ArgumentNullException(nameof(destinationTable));
      if (reporter is null)
        throw new ArgumentNullException(nameof(reporter));

      string? err = null;

      var entries = Interlocked.Increment(ref _fillDataEntranceCounter);
      try
      {
        if (entries == 1)
        {
          using (var suspendToken = destinationTable.SuspendGetToken())
          {
            try
            {
              bool useTableScript = ImportOptions.ExecuteTableScriptAfterImport && destinationTable.TableScript is not null;

              FillData_Unchecked(destinationTable, useTableScript ? reporter.GetSubTask(0.5) : reporter);

              try
              {
                if (ImportOptions.ExecuteTableScriptAfterImport && destinationTable.TableScript is { } tableScript)
                  tableScript.ExecuteWithoutExceptionCatching(destinationTable, reporter.GetSubTask(0.5));
              }
              catch (Exception ex)
              {
                err = $"{DateTime.Now} - Exception during execution of the table script (after execution of the data source). Details follow:\r\n{ex}";
                destinationTable.Notes.WriteLine(err);
                Current.Console.WriteLine($"{DateTime.Now} - Exception during execution of the table script (after execution of the data source) {GetType().Name} of table '{destinationTable.Name}': {ex.Message}");
              }

              reporter?.Report(1); // report this task as finished
            }
            catch (Exception ex)
            {
              err = $"{DateTime.Now} - Error during execution of data source ({GetType().Name}), Details:\r\n{ex}";
              destinationTable.Notes.WriteLine(err);
              Current.Console.WriteLine($"{DateTime.Now} - Error during execution of data source {GetType().Name} of table '{destinationTable.Name}': {ex.Message}");
            }
          }
        }
      }
      finally
      {
        Interlocked.Decrement(ref _fillDataEntranceCounter);
      }
      return err;
    }
  }
}
