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

namespace Altaxo.Serialization
{
  /// <summary>
  /// Defines methods for importing files and determining file format compatibility.
  /// </summary>
  public interface IFileImporter
  {
    /// <summary>
    /// Gets the file extensions.
    /// </summary>
    /// <returns>A list of file extensions (with dot), and the explanation.</returns>
    (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions();

    /// <summary>
    /// Checks the import options. If the provided object is a valid import options object, then this object is returned.
    /// Otherwise, a new import options object is created.
    /// </summary>
    /// <param name="importOptions">The import options.</param>
    /// <returns>Either the provided argument (if valid), or a new import options object if invalid.</returns>
    object CheckOrCreateImportOptions(object? importOptions);

    /// <summary>
    /// Gets the probability for being this file format.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>A probability value (ranging from 0 to 1), that the provided file is the format
    /// that is covered by this importer.</returns>
    double GetProbabilityForBeingThisFileFormat(string fileName);

    /// <summary>
    /// Imports the specified file into the project.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="importOptions">The options for import.</param>
    /// <returns>An error message, or null if successful.</returns>
    string? Import(string fileName, object importOptions);
  }
}
