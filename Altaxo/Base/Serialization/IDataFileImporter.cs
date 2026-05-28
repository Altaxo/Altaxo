#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Serialization
{


  /// <summary>
  /// Interface for a data file importer, such as ASCII, SPC, JCAMP, or SPA import.
  /// This does not cover files that contain full projects.
  /// </summary>
  public interface IDataFileImporter : IFileImporter
  {
    /// <summary>
    /// Creates the table data source.
    /// </summary>
    /// <param name="fileNames">The file names.</param>
    /// <param name="importOptions">The import options.</param>
    /// <returns>The table data source, or null if such a source is not implemented.</returns>
    IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions);

    /// <summary>
    /// Imports the specified file name into a table.
    /// </summary>
    /// <param name="fileNames">Name of the files.</param>
    /// <param name="table">The table.</param>
    /// <param name="importOptions">The options for import.</param>
    /// <param name="attachDataSource">If the value is true, the corresponding data source is attached to the table.</param>
    /// <returns>An error message, or null if successful.</returns>
    string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptions, bool attachDataSource);

    /// <summary>
    /// Imports the specified file names according to the initial options into new tables.
    /// </summary>
    /// <param name="fileNames">The file names.</param>
    /// <param name="initialOptions">The initial options.</param>
    /// <returns>Null if no error has occured, otherwise the error message(s).</returns>
    string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions);


  }
}
