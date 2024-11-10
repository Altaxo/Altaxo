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
      Altaxo.Data.DoubleColumn? xcol = null;
      var errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if (table.DataColumns.ColumnCount > 0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
        var xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        if (xColumnOfRightMost is DoubleColumn dcolMostRight)
          xcol = dcolMostRight;
      }

      int idxYColumn = 0;
      foreach (var fileName in fileNames)
      {
        Origin2AltaxoWrapper reader;
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          reader = new Origin2AltaxoWrapper(new OriginAnyParser(stream));
        }
        int indexOfSpectrum = -1;
        foreach (var entry in reader.EnumerateAllSpreadSheets(considerSpreadsheetsInExcelsToo: true))
        {
          if (!(importOptions.PathsOfImportedData.Count == 0 ||
               importOptions.PathsOfImportedData.Contains(entry.fullName))
             )
          {
            continue;
          }

          var spreadSheet = entry.spreadsheet;

          // add the columns
          for (int idxColumn = 0; idxColumn < spreadSheet.Columns.Count; ++idxColumn)
          {
            var originColumn = spreadSheet.Columns[idxColumn];
            var (columnName, altaxoColumn, altaxoColumnKind) = Origin2AltaxoWrapper.OriginColumnToAltaxoColumn(originColumn);

            if (importOptions.UseNeutralColumnName)
            {
              columnName = $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}";
            }
            columnName = table.DataColumns.FindUniqueColumnName(columnName);
            table.DataColumns.Add(altaxoColumn, columnName, altaxoColumnKind, lastColumnGroup);
            ++idxYColumn;

            if (importOptions.IncludeFilePathAsProperty)
            {
              // add also a property column named "FilePath" if not existing so far
              if (!table.PropCols.ContainsColumn("FilePath"))
                table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

              // now set the file name property cell
              int yColumnNumber = table.DataColumns.GetColumnNumber(altaxoColumn);
              if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
              {
                table.PropCols["FilePath"][yColumnNumber] = fileName;
              }
            }
          }
          lastColumnGroup++;
        } // for each spectrum
        foreach (var entry in reader.EnumerateAllMatrixSheets())
        {
          if (!(importOptions.PathsOfImportedData.Count == 0 ||
               importOptions.PathsOfImportedData.Contains(entry.fullName))
             )
          {
            continue;
          }

          var matrixSheet = entry.matrixSheet;

          // add the columns
          for (int idxColumn = 0; idxColumn < matrixSheet.ColumnCount; ++idxColumn)
          {
            var altaxoColumn = new DoubleColumn();
            for (int idxRow = 0; idxRow < matrixSheet.RowCount; ++idxRow)
            {
              var idxData = idxColumn * matrixSheet.RowCount + idxRow;
              if (idxData < matrixSheet.Data.Count)
                altaxoColumn[idxRow] = matrixSheet.Data[idxData];
            }
            var columnName = $"{(string.IsNullOrEmpty(importOptions.NeutralColumnName) ? "Y" : importOptions.NeutralColumnName)}{idxYColumn}";
            columnName = table.DataColumns.FindUniqueColumnName(columnName);

            table.DataColumns.Add(altaxoColumn, columnName, ColumnKind.V, lastColumnGroup);
            ++idxYColumn;

            if (importOptions.IncludeFilePathAsProperty)
            {
              // add also a property column named "FilePath" if not existing so far
              if (!table.PropCols.ContainsColumn("FilePath"))
                table.PropCols.Add(new Altaxo.Data.TextColumn(), "FilePath");

              // now set the file name property cell
              int yColumnNumber = table.DataColumns.GetColumnNumber(altaxoColumn);
              if (table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
              {
                table.PropCols["FilePath"][yColumnNumber] = fileName;
              }
            }
          }
          lastColumnGroup++;
        } // for each spectrum
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
              reader = new Origin2AltaxoWrapper(new OriginAnyParser(stream));
            }
            foreach (var entry in reader.EnumerateAllSpreadSheets(considerSpreadsheetsInExcelsToo: true))
            {
              var dataTable = new DataTable();
              if (initialOptions.UseMetaDataNameAsTableName)
              {
                dataTable.Name = entry.fullName;
              }
              var localImportOptions = importOptions with { PathsOfImportedData = [entry.fullName] };

              var result = Import([fileName], dataTable, localImportOptions);
              if (result is not null)
              {
                stb.AppendLine(result);
              }
              Current.Project.AddItemWithThisOrModifiedName(dataTable);
              Current.ProjectService.CreateNewWorksheet(dataTable);
              dataTable.DataSource = new OriginImportDataSource(fileNames, localImportOptions);
            }

            // now the matrices
            foreach (var entry in reader.EnumerateAllMatrixSheets())
            {
              var dataTable = new DataTable();
              if (initialOptions.UseMetaDataNameAsTableName)
              {
                dataTable.Name = entry.fullName;
              }
              var localImportOptions = importOptions with { PathsOfImportedData = [entry.fullName] };

              var result = Import([fileName], dataTable, localImportOptions);
              if (result is not null)
              {
                stb.AppendLine(result);
              }
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


  }
}
