using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Gui;
using Altaxo.Gui.Workbench;

namespace Altaxo.Serialization
{
  public abstract record DataFileImporterBase : IDataFileImporter, Main.IImmutable
  {
    /// <inheritdoc/>
    public abstract object CheckOrCreateImportOptions(object? importOptions);

    /// <inheritdoc/>
    public abstract IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions);

    /// <inheritdoc/>
    public abstract (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions();

    /// <inheritdoc/>
    public abstract double GetProbabilityForBeingThisFileFormat(string fileName);

    /// <inheritdoc/>
    public abstract string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptions, bool attachDataSource = true);

    public virtual string? Import(IReadOnlyList<string> fileNames, ImportOptionsInitial initialOptions)
    {
      var stb = new StringBuilder();

      if (initialOptions.DistributeFilesToSeparateTables)
      {
        foreach (var fileName in fileNames)
        {
          var dataTable = new DataTable();
          var result = Import([fileName], dataTable, initialOptions.ImportOptions, attachDataSource: true);
          if (result is not null)
          {
            stb.AppendLine(result);
          }
          Current.Project.AddItemWithThisOrModifiedName(dataTable);
          Current.ProjectService.CreateNewWorksheet(dataTable);
        }
      }
      else // all files into one table
      {
        var dataTable = new DataTable();
        var result = Import(fileNames, dataTable, initialOptions.ImportOptions, attachDataSource: true);
        if (result is not null)
        {
          stb.AppendLine(result);
        }
        Current.Project.AddItemWithThisOrModifiedName(dataTable);
        Current.ProjectService.CreateNewWorksheet(dataTable);
      }

      return stb.Length == 0 ? null : stb.ToString();
    }

    /// <summary>
    /// Imports the show dialogs. First the dialog to choose the files, then to choose the initial import options, and optionally the dialog to choose whether to import
    /// in an existing worksheet or in another worksheet.
    /// </summary>
    /// <param name="activeViewContent">Content of the active view.</param>
    /// <param name="importer">The importer.</param>
    public static void ImportShowDialogs(IViewContent activeViewContent, IDataFileImporter importer)
    {
      if (ShowOpenFileDialog(importer) is not { } fileOptions ||
          fileOptions.FileNames.Length == 0)
      {
        return; // user has cancelled the operation
      }

      var initialOptions = new ImportOptionsInitial(importer.CheckOrCreateImportOptions(null));
      if (false == Current.Gui.ShowDialog(ref initialOptions, "Import options", false))
        return;


      // Cases:
      // 1. Active content not is a table
      //    - Show options dialog, import data into one or more new tables
      // 2. Active content is a table and the table is empty
      //    - Show options dialog, import data into this or more tables
      // 3. Active content is a table and the table is non-empty
      //    - Ask first whether to import the data in this or a new table

      DataTable? existingTable = null;

      if (initialOptions.DistributeFilesToSeparateTables == false && initialOptions.DistributeDataPerFileToSeparateTables == false)
      {
        if (activeViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
        {
          if (ctrl.DataTable.DataColumnCount != 0 && ctrl.DataTable.PropCols.ColumnCount != 0)
          {
            var answer = Current.Gui.YesNoCancelMessageBox(
              "The current worksheet is not empty.\r\n" +
              "Do you want to import the data into this worksheet?\r\n" +
              "Yes: import data into this (non-empty) worksheet\r\n" +
              "No : import data into new worksheet(s)",
              "Existing or new worksheet?", false);
            if (!answer.HasValue)
            {
              return; // user has cancelled this operation
            }
            else if (answer == true)
            {
              existingTable = ctrl.DataTable;
            }
          }
        }

      }
      if (existingTable is not null)
      {
        importer.Import(fileOptions.FileNames, existingTable, initialOptions.ImportOptions, attachDataSource: true);
      }
      else
      {
        importer.Import(fileOptions.FileNames, initialOptions);
      }
    }

    /// <summary>
    /// Shows a file import dialog for the specific importer provided as parameter, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="importer">The data file importer.</param>
    /// <returns>The open file options. If the use has cancelled this dialog, the return value is null.</returns>
    public static OpenFileOptions? ShowOpenFileDialog(IDataFileImporter importer)
    {
      var fileOptions = new Altaxo.Gui.OpenFileOptions();
      var filter = FileIOHelper.GetFilterDescriptionForExtensions(importer.GetFileExtensions());
      fileOptions.AddFilter(filter.Filter, filter.Description);
      filter = FileIOHelper.GetFilterDescriptionForAllFiles();
      fileOptions.AddFilter(filter.Filter, filter.Description);
      fileOptions.FilterIndex = 0;
      fileOptions.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(fileOptions))
      {
        Array.Sort(fileOptions.FileNames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
        return fileOptions;
      }
      else
      {
        return null;
      }
    }



    /// <summary>
    /// Shows a file import dialog for the specific importer provided as parameter, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="table">The table to import the files to.</param>
    /// <param name="importer">The data file importer.</param>
    public static void ShowDialog(Altaxo.Data.DataTable table, IDataFileImporter importer)
    {
      if (ShowOpenFileDialog(importer) is { } options)
      {
        string? errors = importer.Import(options.FileNames, table, importer.CheckOrCreateImportOptions(null), true);

        if (errors is not null)
        {
          Current.Gui.ErrorMessageBox(errors, "Some errors occured during import!");
        }
      }
    }
  }
}

