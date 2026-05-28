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

namespace Altaxo.Serialization
{
  /// <summary>
  /// Initial import options for data files that are used during an initial import.
  /// </summary>
  public record ImportOptionsInitial
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImportOptionsInitial"/> record.
    /// </summary>
    /// <param name="importOptions">The import options object.</param>
    public ImportOptionsInitial(object importOptions)
    {
      ImportOptions = importOptions ?? throw new System.ArgumentNullException(nameof(importOptions));
    }

    /// <summary>
    /// Gets the import options.
    /// </summary>
    public object ImportOptions { get; init; }

    /// <summary>
    /// If true, during initial import if multiple files are imported, each file is imported in a separate table.
    /// </summary>
    public bool DistributeFilesToSeparateTables { get; init; } = true;

    /// <summary>
    /// If true, if the file contains multiple graph data, the graphs are imported in separate tables.
    /// </summary>
    public bool DistributeDataPerFileToSeparateTables { get; init; } = true;

    /// <summary>
    /// If true, tables created during the import process are named based on the meta data of the graph data.
    /// </summary>
    public bool UseMetaDataNameAsTableName { get; init; } = true;
  }
}
