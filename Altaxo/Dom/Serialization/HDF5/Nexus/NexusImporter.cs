#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Altaxo.Data;
using PureHDF;

namespace Altaxo.Serialization.HDF5.Nexus
{
  /// <summary>
  /// Imports Nexus files (HDF5 files with Nexus convention, see <see href="https://www.nexusformat.org/"/>).
  /// </summary>
  public record NexusImporter : DataFileImporterBase
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".hdf5", ".nx", ".nx5", ".nxs"], "Nexus files (*.hdf5;*.nx;*.nx5;*.nxs)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as NexusImportOptions) ?? new NexusImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new NexusImportDataSource(fileNames, (NexusImportOptions)importOptions);
    }

    /// <inheritdoc/>
    public override double GetProbabilityForBeingThisFileFormat(string fileName)
    {
      double p = 0;
      var fe = GetFileExtensions();
      if (fe.FileExtensions.ToHashSet().Contains(Path.GetExtension(fileName).ToLowerInvariant()))
      {
        p += 0.5;
      }

      try
      {
        // Make sure:
        // The file can be opened
        // The file contains at least one valid group, which has
        // the NX_class attribute and the value is NXentry
        using var file = H5File.OpenRead(fileName);
        int numberOfNXentryGroups = 0;
        foreach (var group in file.Children().OfType<IH5Group>())
        {
          var att = group.Attributes().FirstOrDefault(a => a.Name == "NX_class");
          if (att is not null && att.Read<string>() == "NXentry")
          {
            ++numberOfNXentryGroups;
          }
        }
        if (numberOfNXentryGroups > 0)
        {
          p += 0.5;
        }
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    /// <inheritdoc/>
    public override string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (NexusImportOptions)importOptionsObj;
      var errors = new StringBuilder();

      int groupNumber = 0;

      var columnNameDictionary = new Dictionary<string, int>(); // dictionary to track how often a columnName is already used. Key is the column name, Value is the number of columns with that name.

      foreach (var fileName in fileNames)
      {
        try
        {
          using var file = H5File.OpenRead(fileName);
          int nxEntryIndex = -1;
          foreach (var nxEntry in file.GetAllObjectsWithClassName<IH5Group>("NXentry"))
          {
            nxEntryIndex++;
            if (importOptions.IndicesOfImportedEntries.Count > 0 && !importOptions.IndicesOfImportedEntries.Contains(nxEntryIndex))
            {
              continue;
            }

            // either search vor a dataset with NX_class attribute directly in the group
            var nxData = nxEntry.GetFirstObjectWithClassName<IH5Group>("NXdata");
            if (nxData is null)
            {
              // if no data directly, then maybe we have an NXprocess object
              var nxProcess = nxEntry.GetFirstObjectWithClassName<IH5Group>("NXprocess");
              if (nxProcess is not null)
              {
                nxData = nxProcess.GetFirstObjectWithClassName<IH5Group>("NXdata");
              }
            }

            if (nxData is not null)
            {
              string[]? axesNames = null;
              string? signalName = null;
              string[]? auxiliarySignalNames = null;
              string? title = null;
              if (nxData.AttributeExists("axes"))
              {
                axesNames = nxData.Attribute("axes").Read<string[]>();
              }
              if (nxData.AttributeExists("signal"))
              {
                signalName = nxData.Attribute("signal").Read<string>();
              }
              if (nxData.AttributeExists("auxiliary_signals"))
              {
                auxiliarySignalNames = nxData.Attribute("auxiliary_signals").Read<string[]>();
              }

              if (nxData.GetChildObjectNamed<IH5Dataset>("title") is { } titleDataSet)
                title = titleDataSet.Read<string>();


              if (!string.IsNullOrEmpty(signalName) &&
                  nxData.GetChildObjectNamed<IH5Dataset>(signalName) is { } signalDataSet
                  )
              {
                var signalDimensions = signalDataSet.Space.Dimensions;
                if (axesNames is not null && axesNames.Length == signalDimensions.Length)
                {
                  var axesDataSets = new IH5Dataset[signalDimensions.Length];
                  var axesDataValues = new double[signalDimensions.Length][];
                  for (int i = 0; i < axesDataSets.Length; i++)
                  {
                    axesDataSets[i] = nxData.GetChildObjectNamed<IH5Dataset>(axesNames[i]);
                    axesDataValues[i] = axesDataSets[i].ReadData();
                  }


                  // Note that only the last dimension of the axes can form columns in Altaxo
                  // the other dimensions can be metadata of the columns
                  var xdata = axesDataSets[^1].ReadData();
                  var (xCol, xColPostfix) = CreateDoubleDataColumn(importOptions.UseNeutralColumnName || axesNames.Length == 0 || string.IsNullOrEmpty(axesNames[^1]) ? "X" : axesNames[^1], ColumnKind.X, groupNumber, table, columnNameDictionary);
                  xCol.Data = xdata;
                  SetColumnProperties(table, xCol, importOptions, fileName, nxEntry.Name, nxEntryIndex, title,
                    axesDataSets[^1].TryGetAttributeValueAsString("long_name"),
                    axesDataSets[^1].TryGetAttributeValueAsString("units")
                    );

                  var yDataSets = new List<(IH5Dataset dataSet, string dataSetName)>();
                  yDataSets.Add((signalDataSet, signalName));
                  if (auxiliarySignalNames is not null)
                  {
                    yDataSets.AddRange(auxiliarySignalNames.Select(name => (nxData.GetChildObjectNamed<IH5Dataset>(name), name)));
                  }

                  foreach (var (yDataSet, yDataSetName) in yDataSets)
                  {
                    var yData = yDataSet.ReadData();

                    double[]? errorData = null;
                    if (nxData.GetChildObjectNamed<IH5Dataset>(yDataSetName + "_errors") is { } errorDataSet)
                    {
                      errorData = errorDataSet.ReadData();
                    }

                    if (signalDimensions.Length == 1)
                    {
                      string yColumnName = (importOptions.UseNeutralColumnName || string.IsNullOrEmpty(yDataSetName)) ?
                        (string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName) :
                        yDataSetName;
                      var (yCol, yColPostFix) = CreateDoubleDataColumn(yColumnName, ColumnKind.V, groupNumber, table, columnNameDictionary);
                      yCol.Data = yData;

                      SetColumnProperties(table, yCol, importOptions, fileName, nxEntry.Name, nxEntryIndex, title,
                              yDataSet.TryGetAttributeValueAsString("long_name"),
                              yDataSet.TryGetAttributeValueAsString("units")
                              );

                      // retrieve the error column, if existing
                      if (errorData is not null)
                      {
                        var errCol = table.DataColumns.EnsureExistence(FormattableString.Invariant($"{yColumnName}{(yColPostFix.HasValue ? yColPostFix.Value : "")}.Err"), typeof(DoubleColumn), ColumnKind.Err, groupNumber);
                        errCol.Data = errorData;
                        SetColumnProperties(table, yCol, importOptions, fileName, nxEntry.Name, nxEntryIndex, title,
                             yDataSet.TryGetAttributeValueAsString("long_name"),
                             yDataSet.TryGetAttributeValueAsString("units")
                             );
                      }
                    }
                    else if (signalDimensions.Length == 2)
                    {
                      DataColumn? axis0Col = null;
                      if (signalDimensions[0] > 1)
                      {
                        table.PropCols.EnsureExistence(axesDataSets[0].Name, typeof(DoubleColumn), ColumnKind.V, 0);
                      }

                      for (int iD0 = 0; iD0 < (int)signalDimensions[0]; iD0++)
                      {
                        string yColumnName;
                        if (signalDimensions[0] == 1)
                        {
                          yColumnName = (importOptions.UseNeutralColumnName || string.IsNullOrEmpty(yDataSetName)) ?
                          (string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName) :
                          yDataSetName;
                        }
                        else
                        {
                          yColumnName = (importOptions.UseNeutralColumnName || string.IsNullOrEmpty(yDataSetName)) ?
                          (string.IsNullOrEmpty(importOptions.NeutralColumnName) ? FormattableString.Invariant($"Y({iD0}") : FormattableString.Invariant($"{importOptions.NeutralColumnName}({iD0}")) :
                          FormattableString.Invariant($"{yDataSetName}({iD0}");
                        }

                        var (yCol, yColPostFix) = CreateDoubleDataColumn(yColumnName, ColumnKind.V, groupNumber, table, columnNameDictionary);
                        yCol.Data = yData.Skip(iD0 * (int)signalDimensions[1]).Take((int)signalDimensions[1]);
                        if (axis0Col is not null)
                        {
                          var yColIdx = table.DataColumns.GetColumnNumber(yCol);
                          axis0Col[yColIdx] = axesDataValues[0][iD0];
                        }

                        SetColumnProperties(table, yCol, importOptions, fileName, nxEntry.Name, nxEntryIndex, title,
                                yDataSet.TryGetAttributeValueAsString("long_name"),
                                yDataSet.TryGetAttributeValueAsString("units")
                                );

                        if (errorData is not null)
                        {
                          var errCol = table.DataColumns.EnsureExistence(FormattableString.Invariant($"{yColumnName}{(yColPostFix.HasValue ? yColPostFix.Value : "")}.Err"), typeof(DoubleColumn), ColumnKind.Err, groupNumber);
                          errCol.Data = errorData.Skip(iD0 * (int)signalDimensions[1]).Take((int)signalDimensions[1]);
                          if (axis0Col is not null)
                          {
                            var errColIdx = table.DataColumns.GetColumnNumber(errCol);
                            axis0Col[errColIdx] = axesDataValues[0][iD0];
                          }
                          SetColumnProperties(table, yCol, importOptions, fileName, nxEntry.Name, nxEntryIndex, title,
                               yDataSet.TryGetAttributeValueAsString("long_name"),
                               yDataSet.TryGetAttributeValueAsString("units")
                               );
                        }
                      }
                    }
                    else
                    {
                      throw new NotImplementedException($"The number of signal dimensions is {signalDimensions.Length}, which is not yet implemented.");
                    }
                  }
                }


                // now read all data sets from the data group


              }
            }
            ++groupNumber;
          }
        }
        catch (Exception ex)
        {
          errors.AppendLine($"Error importing file {fileName}: {ex.Message}");
        }
      }

      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(fileNames, importOptions);
      }

      return errors.Length == 0 ? null : errors.ToString();
    }

    /// <summary>
    /// Sets the column properties of one data column.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="column">The data column.</param>
    /// <param name="importOptions">The import options that determines which columns to set.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="nxEntryName">Name of the NXentry.</param>
    /// <param name="nxEntryIndex">Index of the NXentry.</param>
    /// <param name="title">The title.</param>
    /// <param name="longName">The long name.</param>
    /// <param name="unit">The unit.</param>
    private void SetColumnProperties(DataTable table, DataColumn column, NexusImportOptions importOptions, string? fileName, string? nxEntryName, int nxEntryIndex, string? title, string? longName, string? unit)
    {
      var yColNumber = table.DataColumns.GetColumnNumber(column);
      if (importOptions.IncludeFilePathAsProperty && !string.IsNullOrEmpty(fileName))
      {
        table.PropCols.EnsureExistence("FilePath", typeof(TextColumn), ColumnKind.V, 0)[yColNumber] = fileName;
      }
      if (importOptions.IncludeNXentryNameAsProperty && !string.IsNullOrEmpty(nxEntryName))
      {
        table.PropCols.EnsureExistence("NXentryName", typeof(TextColumn), ColumnKind.V, 0)[yColNumber] = nxEntryName;
      }
      if (importOptions.IncludeNXentryIndexAsProperty)
      {
        table.PropCols.EnsureExistence("NXentryIndex", typeof(DoubleColumn), ColumnKind.V, 0)[yColNumber] = nxEntryIndex;
      }
      if (importOptions.IncludeTitleAsProperty && !string.IsNullOrEmpty(title))
      {
        table.PropCols.EnsureExistence("Title", typeof(TextColumn), ColumnKind.V, 0)[yColNumber] = title;
      }
      if (importOptions.IncludeLongNameAndUnitAsProperty && !string.IsNullOrEmpty(longName))
      {
        table.PropCols.EnsureExistence("LongName", typeof(TextColumn), ColumnKind.V, 0)[yColNumber] = longName;
      }
      if (importOptions.IncludeLongNameAndUnitAsProperty && !string.IsNullOrEmpty(unit))
      {
        table.PropCols.EnsureExistence("Unit", typeof(TextColumn), ColumnKind.V, 0)[yColNumber] = unit;
      }
    }

    /// <inheritdoc/>
    public override string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions)
    {
      var stb = new StringBuilder();

      var importOptions = (NexusImportOptions)initialOptions.ImportOptions;
      if (initialOptions.DistributeFilesToSeparateTables)
      {
        foreach (var fileName in fileNames)
        {
          if (initialOptions.DistributeDataPerFileToSeparateTables)
          {
            // Here: we not only distribute each frame into a separate table, but also each region

            using var file = H5File.OpenRead(fileName);
            int nxEntryIndex = -1;
            foreach (var nxEntry in file.GetAllObjectsWithClassName<IH5Group>("NXentry"))
            {
              nxEntryIndex++;

              var dataTable = new DataTable();
              string title = string.Empty;

              if (initialOptions.UseMetaDataNameAsTableName)
              {
                title = nxEntry.Name;
              }

              if (!string.IsNullOrEmpty(title))
              {
                dataTable.Name = title;
              }

              var localImportOptions = importOptions with { IndicesOfImportedEntries = [nxEntryIndex] };

              var result = Import([fileName], dataTable, localImportOptions);
              if (result is not null)
              {
                stb.AppendLine(result);
              }
              Current.Project.AddItemWithThisOrModifiedName(dataTable);
              Current.ProjectService.CreateNewWorksheet(dataTable);
              dataTable.DataSource = CreateTableDataSource(fileNames, localImportOptions);
            }

          }
          else
          {
            var dataTable = new DataTable();
            var result = Import([fileName], dataTable, importOptions);
            if (result is not null)
            {
              stb.AppendLine(result);
            }
            Current.Project.AddItemWithThisOrModifiedName(dataTable);
            Current.ProjectService.CreateNewWorksheet(dataTable);
            dataTable.DataSource = CreateTableDataSource(fileNames, importOptions);
          }
        }
      }
      else // all files into one table
      {
        var dataTable = new DataTable();
        var result = Import(fileNames, dataTable, importOptions);
        if (result is not null)
        {
          stb.AppendLine(result);
        }
        Current.Project.AddItemWithThisOrModifiedName(dataTable);
        Current.ProjectService.CreateNewWorksheet(dataTable);
        dataTable.DataSource = CreateTableDataSource(fileNames, importOptions);
      }

      return stb.Length == 0 ? null : stb.ToString();
    }

    /// <summary>
    /// Creates a double data column with a specified name. If a column with the name already exists, a postfix number is appended to the name to ensure a unique column name.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="kind">The kind.</param>
    /// <param name="groupNumber">The group number.</param>
    /// <param name="table">The table.</param>
    /// <param name="columnNameDictionary">The column name dictionary.</param>
    /// <returns>The created data column and the postfix that was used. The postfix number can be used for instance to create an error column with a correspondending name.</returns>
    public static (DoubleColumn column, int? numberPostfix) CreateDoubleDataColumn(string columnName, ColumnKind kind, int groupNumber, DataTable table, Dictionary<string, int> columnNameDictionary)
    {
      DoubleColumn result;
      int? numberPostfix;

      if (columnNameDictionary.TryGetValue(columnName, out var numberOfUses))
      {
        if (numberOfUses == 1) // if there is one column with this name, for consistency reasons we rename the existing column by appending a zero
        {
          table.DataColumns.SetColumnName(columnName, columnName + "0");
          if (table.DataColumns.Contains(columnName + ".Err"))
          {
            table.DataColumns.SetColumnName(columnName + ".Err", columnName + "0.Err");
          }
        }
        numberPostfix = numberOfUses;
        result = (DoubleColumn)table.DataColumns.EnsureExistence(FormattableString.Invariant($"{columnName}{numberOfUses}"), typeof(DoubleColumn), kind, groupNumber);
        columnNameDictionary[columnName] = numberOfUses + 1;
      }
      else
      {
        numberPostfix = null;
        result = (DoubleColumn)table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), kind, groupNumber);
        columnNameDictionary.Add(columnName, 1);
      }
      return (result, numberPostfix);
    }
  }
}
