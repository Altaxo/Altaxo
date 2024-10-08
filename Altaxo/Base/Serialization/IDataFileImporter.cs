using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Interface for a data file importer, such as import of Ascii, SPC, JCAMP, SPA etc.
  /// This does not cover files that contain full projects
  /// </summary>
  public interface IDataFileImporter
  {
    /// <summary>
    /// Gets the file extensions.
    /// </summary>
    /// <returns>A list of file extensions (with dot), and the explanation.</returns>
    (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions();

    /// <summary>
    /// Creates the table data source.
    /// </summary>
    /// <param name="fileNames">The file names.</param>
    /// <param name="importOptions">The import options.</param>
    /// <returns>The table data source, or null if such a source is not implemented.</returns>
    IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions);

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
    /// Imports the specified file name into a table.
    /// </summary>
    /// <param name="fileNames">Name of the files.</param>
    /// <param name="table">The table.</param>
    /// <param name="importOptions">The options for import.</param>
    /// <param name="attachDataSource">If the value is true, the corresponding data source is attached to the table.</param>
    /// <returns>An error message, or null if successful.</returns>
    string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptions, bool attachDataSource = true);

    /// <summary>
    /// Imports the specified file names according to the initial options into new tables.
    /// </summary>
    /// <param name="fileNames">The file names.</param>
    /// <param name="initialOptions">The initial options.</param>
    /// <returns>Null if no error has occured, otherwise the error message(s).</returns>
    string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions);


  }
}
