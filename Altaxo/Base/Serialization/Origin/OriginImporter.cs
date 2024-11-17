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

namespace Altaxo.Serialization.Origin
{
  public record OriginImporter : DataFileImporterBase
  {
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".opj"], "Origin project files (*.opj)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as OriginImportOptions) ?? new OriginImportOptions();
    }

    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new OriginImportDataSource(fileNames, (OriginImportOptions)importOptions);
    }


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
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          var version = OriginAnyParser.ReadFileVersion(stream);
          if (version.IsOpjuFile == false && string.IsNullOrEmpty(version.Error) && version.FileVersion >= 300)
          {
            p += 0.5;
          }
        }
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    public override string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (OriginImportOptions)importOptionsObj;
      var opData = new ImportOperationalDataForTable();

      foreach (var fileName in fileNames)
      {
        Origin2AltaxoWrapper reader;
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          reader = new Origin2AltaxoWrapper(new OriginAnyParser(stream), fileName);
        }
        foreach (var entry in reader.EnumerateAllSpreadSheets(considerSpreadsheetsInExcelsToo: true))
        {
          ImportSpreadSheet(reader, entry.fullName, entry.spreadsheet, importOptions, table, opData);
        }


        foreach (var entry in reader.EnumerateAllMatrixSheets())
        {
          ImportMatrixSheet(reader, entry.fullName, entry.matrixSheet, importOptions, table, opData);
        }
      } // for each file

      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(fileNames, importOptions);
      }

      return null;
    }

    public override string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions)
    {
      var stb = new StringBuilder();
      var importOptions = (OriginImportOptions)initialOptions.ImportOptions;
      if (initialOptions.DistributeFilesToSeparateTables)
      {
        foreach (var fileName in fileNames)
        {
          if (initialOptions.DistributeDataPerFileToSeparateTables)
          {
            Origin2AltaxoWrapper reader;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              reader = new Origin2AltaxoWrapper(new OriginAnyParser(stream), fileName);
            }

            // enumerate all spreadsheets and import them each separately in a table
            foreach (var entry in reader.EnumerateAllSpreadSheets(considerSpreadsheetsInExcelsToo: true))
            {
              var dataTable = new DataTable();
              var opData = new ImportOperationalDataForTable();
              if (initialOptions.UseMetaDataNameAsTableName)
              {
                dataTable.Name = entry.fullName;
              }
              var localImportOptions = importOptions with { PathsOfImportedData = [entry.fullName] };
              ImportSpreadSheet(reader, entry.fullName, entry.spreadsheet, localImportOptions, dataTable, opData);
              Current.Project.AddItemWithThisOrModifiedName(dataTable);
              Current.ProjectService.CreateNewWorksheet(dataTable);
              dataTable.DataSource = new OriginImportDataSource(fileNames, localImportOptions);
            }

            // enumerate all matrices
            foreach (var entry in reader.EnumerateAllMatrixSheets())
            {
              var dataTable = new DataTable();
              var opData = new ImportOperationalDataForTable();
              if (initialOptions.UseMetaDataNameAsTableName)
              {
                dataTable.Name = entry.fullName;
              }
              var localImportOptions = importOptions with { PathsOfImportedData = [entry.fullName] };
              ImportMatrixSheet(reader, entry.fullName, entry.matrixSheet, localImportOptions, dataTable, opData);
              Current.Project.AddItemWithThisOrModifiedName(dataTable);
              Current.ProjectService.CreateNewWorksheet(dataTable);
              dataTable.DataSource = new OriginImportDataSource(fileNames, localImportOptions);
            }
          }
          else
          {
            var dataTable = new DataTable();
            var result = Import([fileName], dataTable, initialOptions.ImportOptions);
            if (result is not null)
            {
              stb.AppendLine(result);
            }
            Current.Project.AddItemWithThisOrModifiedName(dataTable);
            Current.ProjectService.CreateNewWorksheet(dataTable);
            dataTable.DataSource = new OriginImportDataSource(fileNames, importOptions);
          }
        }
      }
      else // all files into one table
      {
        var dataTable = new DataTable();
        var result = Import(fileNames, dataTable, initialOptions.ImportOptions);
        if (result is not null)
        {
          stb.AppendLine(result);
        }
        Current.Project.AddItemWithThisOrModifiedName(dataTable);
        Current.ProjectService.CreateNewWorksheet(dataTable);
        dataTable.DataSource = new OriginImportDataSource(fileNames, importOptions);
      }

      return stb.Length == 0 ? null : stb.ToString();
    }


    public class ImportOperationalDataForTable
    {
      /// <summary>
      /// The last used group number. The start value is -1, thus the number has to be incremented before use.
      /// </summary>
      public int LastUsedGroupNumber = -1;

      /// <summary>
      /// Gets the column name usage of the table. Key is the column name, value is the number of uses.
      /// </summary>
      public Dictionary<string, int> ColumnNameDictionary { get; } = [];


      /// <summary>
      /// Creates a double data column with a specified name. If a column with the name already exists, a postfix number is appended to the name to ensure a unique column name.
      /// </summary>
      /// <param name="columnName">Name of the column.</param>
      /// <param name="kind">The kind.</param>
      /// <param name="groupNumber">The group number.</param>
      /// <param name="table">The table.</param>
      /// <returns>The created data column and the postfix that was used. The postfix number can be used for instance to create an error column with a correspondending name.</returns>
      public (DataColumn column, int? numberPostfix) AddDataColumn(DataColumn column, string columnName, ColumnKind kind, int groupNumber, DataTable table)
      {
        int? numberPostfix;

        if (ColumnNameDictionary.TryGetValue(columnName, out var numberOfUses))
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
          table.DataColumns.Add(column, FormattableString.Invariant($"{columnName}{numberOfUses}"), kind, groupNumber);
          ColumnNameDictionary[columnName] = numberOfUses + 1;
        }
        else
        {
          numberPostfix = null;
          table.DataColumns.Add(column, columnName, kind, groupNumber);
          ColumnNameDictionary.Add(columnName, 1);
        }
        return (column, numberPostfix);
      }
    }

    /// <summary>
    /// Imports an Origin spread sheet into an Altaxo table. The Altaxo table can or can not be empty.
    /// </summary>
    /// <param name="reader">The Origin project wrapper.</param>
    /// <param name="spreadSheetName">Full name of the Origin spread sheet.</param>
    /// <param name="spreadSheet">The Origin spread sheet.</param>
    /// <param name="importOptions">The import options.</param>
    /// <param name="table">The Altaxo table, in which the data are imported.</param>
    /// <param name="opData">The operational data for importing into that Altaxo table.</param>
    public static void ImportSpreadSheet(Origin2AltaxoWrapper reader, string spreadSheetName, SpreadSheet spreadSheet, OriginImportOptions importOptions, DataTable table, ImportOperationalDataForTable opData)
    {
      ++opData.LastUsedGroupNumber;

      for (int idxColumn = 0; idxColumn < spreadSheet.Columns.Count; ++idxColumn)
      {
        var originColumn = spreadSheet.Columns[idxColumn];
        var (columnName, altaxoColumn, altaxoColumnKind) = Origin2AltaxoWrapper.OriginColumnToAltaxoColumn(originColumn);

        if (importOptions.UseNeutralColumnName)
        {
          if (string.IsNullOrEmpty(importOptions.NeutralColumnName))
          {
            columnName = altaxoColumnKind switch
            {
              ColumnKind.X => "X",
              ColumnKind.Y => "Y",
              ColumnKind.Z => "Z",
              ColumnKind.V => "Y",
              ColumnKind.Label => "L",
              _ => "Y",
            };
          }
          else
          {
            columnName = importOptions.NeutralColumnName;
          }
        }

        opData.AddDataColumn(altaxoColumn, columnName, altaxoColumnKind, opData.LastUsedGroupNumber, table);
        int yColumnNumber = table.DataColumns.GetColumnNumber(altaxoColumn);

        if (importOptions.IncludeFilePathAsProperty && !string.IsNullOrEmpty(reader.FileName))
        {
          // add also a property column named "FilePath" if not existing so far
          if (!table.PropCols.ContainsColumn("FilePath"))
            table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

          // now set the file name property cell
          if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
          {
            table.PropCols["FilePath"][yColumnNumber] = reader.FileName;
          }
        }

        if (!string.IsNullOrEmpty(originColumn.LongName))
        {
          // add also a property column named "FilePath" if not existing so far
          if (!table.PropCols.ContainsColumn("LongName"))
            table.PropCols.Add(new Altaxo.Data.TextColumn(), "LongName");
          table.PropCols["LongName"][yColumnNumber] = originColumn.LongName;
        }
        if (!string.IsNullOrEmpty(originColumn.Units))
        {
          // add also a property column named "FilePath" if not existing so far
          if (!table.PropCols.ContainsColumn("Unit"))
            table.PropCols.Add(new Altaxo.Data.TextColumn(), "Unit");
          table.PropCols["Unit"][yColumnNumber] = originColumn.Units;
        }
        if (!string.IsNullOrEmpty(originColumn.Comments))
        {
          // add also a property column named "FilePath" if not existing so far
          if (!table.PropCols.ContainsColumn("Comments"))
            table.PropCols.Add(new Altaxo.Data.TextColumn(), "Comments");
          table.PropCols["Comments"][yColumnNumber] = originColumn.Comments;
        }
      }
    }


    /// <summary>
    /// Imports an Origin matrix sheet into an Altaxo table. The Altaxo table can or can not be empty.
    /// </summary>
    /// <param name="reader">The Origin project wrapper.</param>
    /// <param name="sheetName">Full name of the Origin maxtrix sheet.</param>
    /// <param name="matrixSheet">The Origin matrix sheet.</param>
    /// <param name="importOptions">The import options.</param>
    /// <param name="table">The Altaxo table, in which the data are imported.</param>
    /// <param name="opData">The operational data for importing into that Altaxo table.</param>
    public static void ImportMatrixSheet(Origin2AltaxoWrapper reader, string sheetName, MatrixSheet matrixSheet, OriginImportOptions importOptions, DataTable table, ImportOperationalDataForTable opData)
    {
      ++opData.LastUsedGroupNumber;

      // add the columns
      for (int idxColumn = 0; idxColumn < matrixSheet.ColumnCount; ++idxColumn)
      {
        var altaxoColumn = new DoubleColumn();
        for (int idxRow = 0; idxRow < matrixSheet.RowCount; ++idxRow)
        {
          altaxoColumn[idxRow] = matrixSheet[idxRow, idxColumn];
        }

        string columnName;
        if (string.IsNullOrEmpty(importOptions.NeutralColumnName))
        {
          columnName = "Y";
        }
        else
        {
          columnName = importOptions.NeutralColumnName;
        }

        opData.AddDataColumn(altaxoColumn, columnName, ColumnKind.V, opData.LastUsedGroupNumber, table);

        if (importOptions.IncludeFilePathAsProperty)
        {
          // add also a property column named "FilePath" if not existing so far
          if (!table.PropCols.ContainsColumn("FilePath"))
            table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

          // now set the file name property cell
          int yColumnNumber = table.DataColumns.GetColumnNumber(altaxoColumn);
          if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
          {
            table.PropCols["FilePath"][yColumnNumber] = reader.FileName;
          }
        }
      }
    }



    /// <summary>
    /// Imports the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="importOptions">The import options.</param>
    /// <param name="dataTable">The data table.</param>
    /// <param name="opData">The op data.</param>
    public static void Import(Origin2AltaxoWrapper reader, OriginImportOptions importOptions, DataTable dataTable, ImportOperationalDataForTable opData)
    {
      // import all spread sheets
      foreach (var entry in reader.EnumerateAllSpreadSheets(considerSpreadsheetsInExcelsToo: true))
      {
        if (importOptions.PathsOfImportedData.Count == 0 || importOptions.PathsOfImportedData.Contains(entry.fullName))
        {
          ImportSpreadSheet(reader, entry.fullName, entry.spreadsheet, importOptions, dataTable, opData);
        }
      }

      // import all matrix sheets
      foreach (var entry in reader.EnumerateAllMatrixSheets())
      {
        if (importOptions.PathsOfImportedData.Count == 0 || importOptions.PathsOfImportedData.Contains(entry.fullName))
        {
          ImportMatrixSheet(reader, entry.fullName, entry.matrixSheet, importOptions, dataTable, opData);
        }
      }
    }
  }
}
