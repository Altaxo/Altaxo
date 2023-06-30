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

namespace Altaxo.Data
{
  /// <summary>
  /// Interface that must be implemented by all data sources that can provide data for an Altaxo data table.
  /// </summary>
  public interface IAltaxoTableDataSource :
    Main.ICopyFrom,
    IDisposable,
    Main.ISuspendableByToken,
    Main.IDocumentLeafNode,
    Main.IHasDocumentReferences
  {
    /// <summary>
    /// Fills (or refills) the data, with exception catching and supsension of the destination table.
    /// The table script is executed (if specified in the import options).
    /// Use <see cref="DataTable.UpdateTableFromTableDataSource"/> if the table script should be executed.
    /// The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable"/>.
    /// Exceptions are catched, and will be written to the Notes of the destination data table.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter">The progress reporter (can be null).</param>
    /// <returns>Null if no exception was catched during processing; otherwise an error message.</returns>
    string? FillData(Altaxo.Data.DataTable destinationTable, IProgressReporter? reporter = null);


    /// <summary>
    /// Fills (or refills) the data, without exception catching.
    /// The table script is <b>not executed</b>!
    /// Use <see cref="FillData(DataTable, IProgressReporter?)"/> if the table script should be executed.
    /// The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable"/>.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    void FillData_Unchecked(Altaxo.Data.DataTable destinationTable, IProgressReporter? reporter = null);

    IDataSourceImportOptions ImportOptions { get; set; }

    object ProcessOptionsObject { get; set; }
    object ProcessDataObject { get; set; }

    /// <summary>
    /// Called after deserization of a data source instance, when it is already associated with a data table.
    /// </summary>
    void OnAfterDeserialization();
  }

  /// <summary>
  /// Designates the cause of a re-read of the data source.
  /// </summary>
  public enum ImportTriggerSource
  {
    /// <summary>
    /// The user triggers a reread of the data source.
    /// </summary>
    Manual,

    /// <summary>
    /// The data source is reread if the associated table is used for the first time.
    /// </summary>
    FirstUse,

    /// <summary>
    /// The data source is reread every time the data source has changed. This includes the first use of the associated table.
    /// </summary>
    DataSourceChanged
  }

  /// <summary>
  /// Designates the event that the table's data source has changed.
  /// </summary>
  public class TableDataSourceChangedEventArgs : EventArgs
  {
    public static new readonly TableDataSourceChangedEventArgs Empty = new TableDataSourceChangedEventArgs();
  }
}
