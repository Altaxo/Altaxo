namespace Altaxo.Serialization
{
  /// <summary>
  /// Initial import options for data files, that are used when making an initial import. 
  /// </summary>
  public record ImportOptionsInitial
  {
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
