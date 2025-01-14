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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Altaxo.Serialization.OpenXml.Excel
{
  public record ExcelImporter : DataFileImporterBase
  {
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".xlsx"], "Excel files (*.xlsx)");
    }

    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return importOptions as ExcelImportOptions ?? new ExcelImportOptions();
    }

    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new ExcelImportDataSource(fileNames, (ExcelImportOptions)importOptions);
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
        using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var doc = SpreadsheetDocument.Open(stream, false);
        if (doc.DocumentType == DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook)
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
    public override string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions)
    {
      var stb = new StringBuilder();

      var importOptions = (ExcelImportOptions)initialOptions.ImportOptions;
      if (initialOptions.DistributeFilesToSeparateTables)
      {
        foreach (var fileName in fileNames)
        {
          if (initialOptions.DistributeDataPerFileToSeparateTables)
          {
            // Here: we distribute each sheet into a separate table

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
              WorkbookPart workbookPart = document.WorkbookPart;
              int idxSheet = -1;
              foreach (Sheet sheet in workbookPart.Workbook.Sheets)
              {
                ++idxSheet;
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var dataTable = new DataTable();
                string title = string.Empty;
                int groupNumber = 0;
                var columnNameDictionary = new Dictionary<string, int>();
                ImportOneSheet(worksheetPart, workbookPart, dataTable, importOptions, columnNameDictionary, fileName, sheet.Name, ref groupNumber);


                if (initialOptions.UseMetaDataNameAsTableName)
                {
                  title = sheet.Name;
                }

                if (!string.IsNullOrEmpty(title))
                {
                  dataTable.Name = title;
                }

                var localImportOptions = importOptions with { IndicesOfImportedSheets = [idxSheet] };
                Current.Project.AddItemWithThisOrModifiedName(dataTable);
                dataTable.DataSource = CreateTableDataSource(fileNames, localImportOptions);
                Current.ProjectService.CreateNewWorksheet(dataTable);
              }
            }
          }
          else
          {
            // here we distribute each file to a separate table
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

    public override string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptionsObj, bool attachDataSource = true)
    {
      var importOptions = (ExcelImportOptions)importOptionsObj;

      var columnNameDictionary = new Dictionary<string, int>(); // dictionary to track how often a columnName is already used. Key is the column name, Value is the number of columns with that name.
      int groupNumber = 0;

      foreach (var fileName in fileNames)
      {
        ImportOneFile(fileName, table, importOptions, columnNameDictionary, ref groupNumber);
      }

      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(fileNames, importOptions);
      }

      return null;
    }

    public string? ImportOneFile(string fileName, DataTable table, ExcelImportOptions importOptions, Dictionary<string, int> columnNameDictionary, ref int groupNumber)
    {
      using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
      {
        WorkbookPart workbookPart = document.WorkbookPart;
        int idxSheet = -1;
        foreach (Sheet sheet in workbookPart.Workbook.Sheets)
        {
          ++idxSheet;

          if (importOptions.IndicesOfImportedSheets.Count > 0 && !importOptions.IndicesOfImportedSheets.Contains(idxSheet))
          {
            continue;
          }

          WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
          ImportOneSheet(worksheetPart, workbookPart, table, importOptions, columnNameDictionary, fileName, sheet.Name, ref groupNumber);
        }
      }
      return null;
    }

    public static void ImportOneSheet(WorksheetPart worksheetPart, WorkbookPart workbookPart, DataTable table, ExcelImportOptions importOptions, Dictionary<string, int> columnNameDictionary, string fileName, string sheetName, ref int groupNumber)
    {
      var docAnalysis = new ExcelDocumentAnalysis(worksheetPart, workbookPart, importOptions);

      // create the columns
      DataColumn[] columns = new DataColumn[docAnalysis.HighestScoredLineStructure.Count];
      string[] columnNames = new string[docAnalysis.HighestScoredLineStructure.Count];
      for (int i = 0; i < columns.Length; i++)
      {
        switch (docAnalysis.HighestScoredLineStructure[i].ColumnType)
        {
          case Ascii.AsciiColumnType.Int64:
          case Ascii.AsciiColumnType.Double:
            columns[i] = new DoubleColumn();
            break;
          case Ascii.AsciiColumnType.DateTime:
            columns[i] = new DateTimeColumn();
            break;
          default:
            columns[i] = new TextColumn();
            break;
        }
      }

      SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

      int idxRowExcel = -1;
      int idxRowAltaxo = -1;
      foreach (Row row in sheetData.Elements<Row>())
      {
        ++idxRowExcel;

        // get the column names
        if (idxRowExcel == docAnalysis.IndexOfCaptionLine)
        {
          int idxColumn = -1;
          foreach (Cell cell in row.Elements<Cell>())
          {
            ++idxColumn;
            columnNames[idxColumn] = GetCellValue(cell, workbookPart); // cell.CellValue.Text;
          }
        }

        if (idxRowExcel < docAnalysis.NumberOfMainHeaderLines)
        {

        }
        else
        {
          ++idxRowAltaxo;
          int idxColumn = -1;
          foreach (Cell cell in row.Elements<Cell>())
          {
            ++idxColumn;

            switch (docAnalysis.HighestScoredLineStructure[idxColumn].ColumnType)
            {
              case Ascii.AsciiColumnType.Int64:
              case Ascii.AsciiColumnType.Double:
                if (cell.CellValue.TryGetDouble(out var dbl))
                  columns[idxColumn][idxRowAltaxo] = dbl;
                break;
              case Ascii.AsciiColumnType.DateTime:
                if (cell.CellValue.TryGetDateTime(out var dt))
                  columns[idxColumn][idxRowAltaxo] = dt;
                columns[idxColumn][idxRowAltaxo] = dt;
                break;
              default:
                columns[idxColumn][idxRowAltaxo] = GetCellValue(cell, workbookPart); //cell.CellValue.Text;
                break;
            }
          }
        }
      }

      // now add the columns to the table
      for (int i = 0; i < columns.Length; ++i)
      {
        var columnName = columnNames[i];
        if (string.IsNullOrEmpty(columnName))
          columnName = "Y";
        if (importOptions.UseNeutralColumnName && !string.IsNullOrEmpty(importOptions.NeutralColumnName))
          columnName = importOptions.NeutralColumnName;
        (columnName, var postfix) = GetColumnNameWithPostfix(columnName, table, columnNameDictionary);

        table.DataColumns.Add(columns[i], columnName, ColumnKind.V, groupNumber);

        int columnNumber = table.DataColumns.GetColumnNumber(columns[i]);
        if (importOptions.IncludeFilePathAsProperty)
        {
          table.PropCols.EnsureExistence("FileName", typeof(TextColumn), ColumnKind.V, 0)[columnNumber] = fileName;
        }
        if (importOptions.IncludeSheetNameAsProperty)
        {
          table.PropCols.EnsureExistence("SheetName", typeof(TextColumn), ColumnKind.V, 0)[columnNumber] = sheetName;
        }
      }

      ++groupNumber;
    }

    private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
      string value = cell.CellValue?.InnerText;
      if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
      {
        return workbookPart.SharedStringTablePart.SharedStringTable
            .Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
      }
      return value;
    }

    private static string GetCellType(Cell cell)
    {
      if (cell.DataType == null)
      {
        return "Number";
      }

      if (cell.DataType == CellValues.Boolean)
      {
        return "Boolean";
      }
      else if (cell.DataType == CellValues.Date)
      {
        return "Date";
      }
      else if (cell.DataType == CellValues.Number)
      {
        return "Number";
      }
      else if (cell.DataType == CellValues.SharedString)
      {
        return "SharedString";
      }
      else if (cell.DataType == CellValues.String)
      {
        return "String";
      }
      else if (cell.DataType == CellValues.InlineString)
      {
        return "InlineString";
      }
      else if (cell.DataType == CellValues.Error)
      {
        return "Error";
      }
      else
      {
        return "Unknown";
      }
    }


    /// <summary>
    /// Creates a columnName by providing a base column name. If the column name is already in use, a number will be appended. 
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="table">The table.</param>
    /// <param name="columnNameDictionary">The column name dictionary.</param>
    /// <returns>The unique column name, and the postfix number that was prepended to the proposed column name.</returns>
    public static (string columnName, int? numberPostfix) GetColumnNameWithPostfix(string columnName, DataTable table, Dictionary<string, int> columnNameDictionary)
    {
      string resultName;
      int? numberPostfix;

      if (columnNameDictionary.TryGetValue(columnName, out var numberOfUses))
      {
        if (numberOfUses == 1) // if there is one column with this name, for consistency reasons we rename the existing column by appending a zero
        {
          resultName = columnName + "0";
          if (table.DataColumns.Contains(columnName))
          {
            table.DataColumns.SetColumnName(columnName, resultName);
          }
        }
        numberPostfix = numberOfUses;
        resultName = FormattableString.Invariant($"{columnName}{numberOfUses}");
        columnNameDictionary[columnName] = numberOfUses + 1;
      }
      else
      {
        numberPostfix = null;
        resultName = columnName;
        columnNameDictionary.Add(columnName, 1);
      }
      return (resultName, numberPostfix);
    }
  }
}

