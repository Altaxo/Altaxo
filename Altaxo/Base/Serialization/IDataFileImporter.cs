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
    /// <param name="attachDataSource">If the value is true, the corresponding data source is attached to the table.</param>
    /// <returns>An error message, or null if successful.</returns>
    string? Import(IReadOnlyList<string> fileNames, DataTable table, bool attachDataSource = true);
  }
}
