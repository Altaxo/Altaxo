using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
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


    /// <summary>
    /// Gets the data file importer for the provided file, or null if no appropriate importer could be found.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="importers">All available data file importers.</param>
    /// <returns>The importer that is able to import the file, or null if no such importer could be found.</returns>
    public static IDataFileImporter? GetDataFileImporterForFile(string fileName, IReadOnlyList<IDataFileImporter> importers)
    {
      // first, check the importer that matches the file extension
      var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
      if (!string.IsNullOrEmpty(fileExtension))
      {
        var importerByExtension = importers.FirstOrDefault(importer => importer.GetFileExtensions().FileExtensions.Contains(fileExtension));
        if (importerByExtension is not null && importerByExtension.GetProbabilityForBeingThisFileFormat(fileName) == 1)
        {
          return importerByExtension;
        }
      }

      // if choosing the importer by file extension has failed:
      // evaluate the probabilities for all importers, and return the one with the highest probability
      var tasks = importers.Select(imp => new Task<double>(() => imp.GetProbabilityForBeingThisFileFormat(fileName))).ToArray();
      tasks.ForEachDo(t => t.Start());
      Task.WaitAll(tasks);
      var maxIdx = tasks.IndexOfMax((t) => t.Result);
      if (tasks[maxIdx].Result > 0)
      {
        return importers[maxIdx];
      }
      else
      {
        return null;
      }
    }

    public static void AddError(StringBuilder errors, string? message)
    {
      if (!string.IsNullOrEmpty(message))
        errors.AppendLine(message);
    }

    /// <summary>
    /// Shows a file open dialog for all registered types of data files. If the current view is an empty worksheet and the user has selected a single file,
    /// then the import is done without further questions.
    /// </summary>
    /// <param name="activeViewContent">Content of the active view.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public static void ImportShowReducedDialogsForAllTypes(IViewContent activeViewContent)
    {
      var importers = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDataFileImporter))
                     .Select(x => (IDataFileImporter)Activator.CreateInstance(x))
                     .ToList();
      importers.Sort((x, y) => string.Compare(x.GetType().Name, y.GetType().Name));

      var fileOptions = new Altaxo.Gui.OpenFileOptions();

      var filter = FileIOHelper.GetFilterDescriptionForAllFiles();
      fileOptions.AddFilter(filter.Filter, filter.Description);

      foreach (var importer in importers)
      {
        filter = FileIOHelper.GetFilterDescriptionForExtensions(importer.GetFileExtensions());
        fileOptions.AddFilter(filter.Filter, filter.Description);
      }

      fileOptions.FilterIndex = 0;
      fileOptions.Multiselect = true; // allow selecting more than one file

      if (Current.Gui.ShowOpenFileDialog(fileOptions))
      {
        Array.Sort(fileOptions.FileNames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
      }
      else
      {
        return;
      }


      // if it is only one file ...
      //    -- if the currently active item is an empty table, then import into this table
      //    -- if the currently active item is something else, then create a new table in the folder of the currently active item, import the file there (without dialog)
      // if it is more than one file ...
      //    -- show the multi import dialog

      DataTable? targetDataTableCandidate = null;
      bool isTargetDataTableCandidateEmpty = false;
      if (activeViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
      {
        targetDataTableCandidate = ctrl.DataTable;
        isTargetDataTableCandidateEmpty = targetDataTableCandidate.DataColumns.ColumnCount == 0 && targetDataTableCandidate.PropCols.ColumnCount == 0 && targetDataTableCandidate.TableScript is null;
      }

      var errors = new StringBuilder();
      // ******* Single file was chosen ********
      if (fileOptions.FileNames.Length == 1)
      {
        var importer = GetDataFileImporterForFile(fileOptions.FileName, importers);
        if (importer is not null)
        {
          if (targetDataTableCandidate is not null)
          {
            if (isTargetDataTableCandidateEmpty) // Target table is empty --> we import without asking into this table
            {
              AddError(errors, importer.Import(fileOptions.FileNames, targetDataTableCandidate, importer.CheckOrCreateImportOptions(null), true));
            }
            else // the target table is not empty. We have to ask whether to import in it or into a new table
            {
              var answer = Current.Gui.YesNoCancelMessageBox("The current worksheet is not empty.\r\n" +
                "Do you want to import the data into this worksheet?\r\n\r\n" +
                "Yes: import the file into the current worksheet\r\n" +
                "No:  import the file into a freshly created worksheet",
                "Where to import to?", false);
              if (answer == true) // user wants to import this single file into the current non-empty worksheet
              {
                AddError(errors, importer.Import(fileOptions.FileNames, targetDataTableCandidate, importer.CheckOrCreateImportOptions(null), true));
              }
              else if (answer == false) // user wants to import this single file into a new worksheet
              {
                var folder = targetDataTableCandidate.Folder;
                targetDataTableCandidate = new DataTable();
                targetDataTableCandidate.Name = folder + Path.GetFileName(fileOptions.FileName);
                Current.ProjectService.CreateNewWorksheet(targetDataTableCandidate);
                AddError(errors, importer.Import(fileOptions.FileNames, targetDataTableCandidate, importer.CheckOrCreateImportOptions(null), true));
              }
              else // user has cancelled the dialog
              {
                return;
              }
            }
          }
          else // the active content is not a worksheet
          {
            targetDataTableCandidate = new DataTable();
            targetDataTableCandidate.Name = Path.GetFileName(fileOptions.FileName);
            Current.ProjectService.CreateNewWorksheet(targetDataTableCandidate);
            AddError(errors, importer.Import(fileOptions.FileNames, targetDataTableCandidate, importer.CheckOrCreateImportOptions(null), true));
          }
        }
        else // no importer could be found
        {
          Current.Gui.ErrorMessageBox($"Sorry, could not find an appropriate importer for file {fileOptions.FileName}");
        }
      }
      else if (fileOptions.FileNames.Length > 1)
      {
        var importerGroups = fileOptions.FileNames.Select(fileName => (fileName, GetDataFileImporterForFile(fileName, importers))).GroupBy(x => x.Item2?.GetType()).ToList();

        foreach (var importerGroup in importerGroups)
        {
          var importer = importerGroup.First().Item2;
          if (importer is null)
          {
            errors.AppendLine("The following files could not be imported:");
            foreach (var item in importerGroup)
              errors.AppendLine($"- \"{item.fileName}\"");
            continue;
          }

          var initialOptions = new ImportOptionsInitial(importer.CheckOrCreateImportOptions(null));
          if (false == Current.Gui.ShowDialog(ref initialOptions, $"Import options for {importer.GetType().Name}", false))
            continue;

          var fileNames = importerGroup.Select(g => g.fileName).ToList();

          if (initialOptions.DistributeFilesToSeparateTables == false && initialOptions.DistributeDataPerFileToSeparateTables == false)
          {
            if (targetDataTableCandidate is not null && !isTargetDataTableCandidateEmpty)
            {
              var answer = Current.Gui.YesNoCancelMessageBox(
                "The current worksheet is not empty.\r\n" +
                "Do you want to import the data into this worksheet?\r\n" +
                "Yes: import data into this (non-empty) worksheet\r\n" +
                "No : import data into new worksheet(s)",
                "Existing or new worksheet?", false);
              if (answer == true)
              {
                importer.Import(fileNames, targetDataTableCandidate, initialOptions.ImportOptions, attachDataSource: true);
                targetDataTableCandidate = null; // do no more use this table
              }
              else if (answer == false)
              {
                importer.Import(fileNames, initialOptions);
              }
              else
              {
                continue;
              }
            }
          }
        }
      }

      if (errors.Length > 0)
      {
        Current.Gui.ErrorMessageBox($"Some errors have occured during import:\r\n\r\n{errors}", "Import errors");
      }
    }

    public static void ImportFromFile(DataTable table, string fileName)
    {
      var importers = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDataFileImporter))
                   .Select(x => (IDataFileImporter)Activator.CreateInstance(x))
                   .ToList();

      var importer = GetDataFileImporterForFile(fileName, importers);

      importer.Import([fileName], table, importer.CheckOrCreateImportOptions(null), attachDataSource: true);
    }
  }
}

