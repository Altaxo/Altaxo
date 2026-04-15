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
  /// <summary>
  /// Data file importer for Excel workbooks (<c>.xlsx</c>).
  /// </summary>
  public record ExcelImporter : DataFileImporterBase
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".xlsx"], "Excel files (*.xlsx)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return importOptions as ExcelImportOptions ?? new ExcelImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new ExcelImportDataSource(fileNames, (ExcelImportOptions)importOptions);
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
                if (workbookPart.GetPartById(sheet.Id) is not WorksheetPart worksheetPart)
                {
                  continue;
                }

                ++idxSheet;
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

    /// <inheritdoc/>
    public override string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptions, bool attachDataSource = true)
    {
      var importOptionsX = (ExcelImportOptions)importOptions;

      var columnNameDictionary = new Dictionary<string, int>(); // dictionary to track how often a columnName is already used. Key is the column name, Value is the number of columns with that name.
      int groupNumber = 0;

      foreach (var fileName in fileNames)
      {
        ImportOneFile(fileName, table, importOptionsX, columnNameDictionary, ref groupNumber);
      }

      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(fileNames, importOptionsX);
      }

      return null;
    }

    /// <summary>
    /// Imports all selected sheets from one Excel file into the provided table.
    /// </summary>
    /// <param name="fileName">The Excel file name.</param>
    /// <param name="table">The destination table.</param>
    /// <param name="importOptions">The import options.</param>
    /// <param name="columnNameDictionary">A dictionary used to track already used column names.</param>
    /// <param name="groupNumber">The current column group number. Updated by this method.</param>
    /// <returns>
    /// A string containing error messages, or <see langword="null"/> if no errors occurred.
    /// </returns>
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
          if (workbookPart.GetPartById(sheet.Id) is WorksheetPart worksheetPart && worksheetPart.Worksheet is not null)
          {
            ImportOneSheet(worksheetPart, workbookPart, table, importOptions, columnNameDictionary, fileName, sheet.Name, ref groupNumber);
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Imports a single sheet of an Excel workbook into the provided table.
    /// </summary>
    /// <param name="worksheetPart">The worksheet part.</param>
    /// <param name="workbookPart">The workbook part (needed to resolve references).</param>
    /// <param name="table">The destination table.</param>
    /// <param name="importOptions">The import options.</param>
    /// <param name="columnNameDictionary">A dictionary used to track already used column names.</param>
    /// <param name="fileName">The source file name.</param>
    /// <param name="sheetName">The sheet name.</param>
    /// <param name="groupNumber">The current column group number. Updated by this method.</param>
    public static void ImportOneSheet(WorksheetPart worksheetPart, WorkbookPart workbookPart, DataTable table, ExcelImportOptions importOptions, Dictionary<string, int> columnNameDictionary, string fileName, string sheetName, ref int groupNumber)
    {
      var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
      var excelColumns = ExcelColumnAnalysis.GetColumns(sheetData);

      // we use the document analysis here only to determine the number of header lines and the caption line.
      var docAnalysis = new ExcelDocumentAnalysis(excelColumns, importOptions);

      var numberOfHeaderLines = docAnalysis.NumberOfMainHeaderLines;

      var idxDestColumn = 0;
      var idxSrcColumn = 0;
      var srcColumnToDestColumnMapping = new Dictionary<int, DataColumn>();
      foreach (var column in excelColumns)
      {
        var typeOfColumn = ExcelColumnAnalysis.GetTypeOfColumn(column, numberOfHeaderLines);

        DataColumn destColumn = null;
        switch (typeOfColumn)
        {
          case Type t when t == typeof(double):
            destColumn = new DoubleColumn();
            break;
          case Type t when t == typeof(int):
            destColumn = new DoubleColumn();
            break;
          case Type t when t == typeof(DateTime):
            destColumn = new DateTimeColumn();
            break;
          case Type t when t == typeof(string):
            destColumn = new TextColumn();
            break;
        }
        ;

        if (destColumn is not null)
        {
          srcColumnToDestColumnMapping.Add(idxSrcColumn, destColumn);
          // add the columns already here (because we need them to determine the property column cells), later we will rename them
          table.DataColumns.Add(destColumn, Guid.NewGuid().ToString(), idxDestColumn == 0 ? ColumnKind.X : ColumnKind.V, groupNumber);
          ++idxDestColumn;
        }
        ++idxSrcColumn;
      }


      var columnNames = new string[srcColumnToDestColumnMapping.Count];
      var headerText = new StringBuilder();
      int idxHeaderLine = 0;
      int idxRowAltaxo = -1;
      for (int idxRowExcel = 0; idxRowExcel < excelColumns[0].Count; ++idxRowExcel)
      {
        if (idxRowExcel < docAnalysis.NumberOfMainHeaderLines)
        {
          // get the column names
          if (idxRowExcel == docAnalysis.IndexOfCaptionLine && !importOptions.UseNeutralColumnName)
          {
            for (int idxColumn = 0; idxColumn < excelColumns.Count; ++idxColumn)
            {
              var cell = excelColumns[idxColumn][idxRowExcel];

              if (srcColumnToDestColumnMapping.ContainsKey(idxColumn))
              {
                columnNames[idxColumn] = GetCellValue(cell, workbookPart); // cell.CellValue.Text;
              }
            }
          }
          else
          {
            DataColumn? pCol = null;
            if (importOptions.HeaderLinesDestination == Ascii.AsciiHeaderLinesDestination.ImportToProperties ||
                importOptions.HeaderLinesDestination == Ascii.AsciiHeaderLinesDestination.ImportToPropertiesAndNotes ||
                importOptions.HeaderLinesDestination == Ascii.AsciiHeaderLinesDestination.ImportToPropertiesOrNotes
              )
            {
              pCol = table.PropCols.EnsureExistence($"HeaderLine{idxHeaderLine}", typeof(TextColumn), ColumnKind.V, 0);
              ++idxHeaderLine;
            }
            for (int idxColumn = 0; idxColumn < excelColumns.Count; ++idxColumn)
            {
              var cell = excelColumns[idxColumn][idxRowExcel];
              if (!srcColumnToDestColumnMapping.TryGetValue(idxColumn, out var destColumn))
                continue;

              var value = GetCellValue(cell, workbookPart); // cell.CellValue.Text;
              if (!string.IsNullOrEmpty(value))
              {
                if (pCol is not null)
                {
                  pCol[table.DataColumns.GetColumnNumber(destColumn)] = value;
                }

                if (idxColumn == 0)
                {
                  if (headerText.Length > 0)
                  {
                    headerText.AppendLine();
                  }
                }
                else
                {
                  headerText.Append("\t");
                }
                headerText.Append(value);
              }
            }
          }
        }
        else
        {
          ++idxRowAltaxo;
          for (int idxColumn = 0; idxColumn < excelColumns.Count; ++idxColumn)
          {
            var cell = excelColumns[idxColumn][idxRowExcel];
            if (!srcColumnToDestColumnMapping.TryGetValue(idxColumn, out var destColumn))
              continue;

            switch (destColumn)
            {
              case DoubleColumn:
                if (true == cell?.CellValue?.TryGetDouble(out var dbl))
                  destColumn[idxRowAltaxo] = dbl;
                break;
              case DateTimeColumn:
                if (true == cell.CellValue?.TryGetDateTime(out var dt))
                  destColumn[idxRowAltaxo] = dt;
                break;
              case TextColumn:
              default:
                destColumn[idxRowAltaxo] = GetCellValue(cell, workbookPart); //cell.CellValue.Text;
                break;
            }
          }
        }
      }

      // now rename the columns to the table
      for (int i = 0; i < table.DataColumnCount; ++i)
      {
        var columnName = columnNames[i];
        if (string.IsNullOrEmpty(columnName))
          columnName = "Y";
        if (importOptions.UseNeutralColumnName && !string.IsNullOrEmpty(importOptions.NeutralColumnName))
          columnName = importOptions.NeutralColumnName;
        (columnName, var postfix) = GetColumnNameWithPostfix(columnName, table, columnNameDictionary);

        table.DataColumns.SetColumnName(table.DataColumns[i], columnName);

        int columnNumber = table.DataColumns.GetColumnNumber(table.DataColumns[i]);
        if (importOptions.IncludeFilePathAsProperty)
        {
          table.PropCols.EnsureExistence("FileName", typeof(TextColumn), ColumnKind.V, 0)[columnNumber] = fileName;
        }
        if (importOptions.IncludeSheetNameAsProperty)
        {
          table.PropCols.EnsureExistence("SheetName", typeof(TextColumn), ColumnKind.V, 0)[columnNumber] = sheetName;
        }
      }

      // add the header text
      if (headerText.Length > 0)
      {
        if (importOptions.HeaderLinesDestination == Ascii.AsciiHeaderLinesDestination.ImportToNotes || importOptions.HeaderLinesDestination == Ascii.AsciiHeaderLinesDestination.ImportToPropertiesAndNotes)
        {
          table.Notes.Text += headerText.ToString();
        }
      }

      ++groupNumber;
    }

    /// <summary>
    /// Gets the textual value of a spreadsheet cell.
    /// </summary>
    /// <param name="cell">The cell.</param>
    /// <param name="workbookPart">The workbook part used to resolve shared strings.</param>
    /// <returns>The cell value as text, or <see langword="null"/> if the cell has no value.</returns>
    private static string GetCellValue(Cell? cell, WorkbookPart workbookPart)
    {
      var value = cell?.CellValue?.InnerText;
      if (cell?.DataType is not null && cell.DataType.Value == CellValues.SharedString)
      {
        return workbookPart.SharedStringTablePart.SharedStringTable
            .Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
      }
      return value;
    }

    /// <summary>
    /// Gets the cell type as a string.
    /// </summary>
    /// <param name="cell">The cell.</param>
    /// <returns>A string representing the cell type.</returns>
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
    /// Creates a column name by providing a base column name. If the column name is already in use, a number will be appended.
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

