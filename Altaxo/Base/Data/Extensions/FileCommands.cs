﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.IO;
using Altaxo.Collections;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Data
{
  /// <summary>
  /// Extensions for saving, exporting, and importing files.
  /// </summary>
  public static class FileCommands
  {
    /// <summary>
    /// Imports Ascii data from a stream into the data table.
    /// </summary>
    /// <param name="dataTable">The table where to import into.</param>
    /// <param name="myStream">The stream to import from.</param>
    /// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
    [Obsolete]
    public static void ImportAscii(this DataTable dataTable, System.IO.Stream myStream, string streamOriginHint)
    {
      AsciiImporter.ImportFromAsciiStream(dataTable, myStream, streamOriginHint);
    }

    /// <summary>
    /// Imports multiple Ascii files into the provided table and additionally created tables.
    /// </summary>
    /// <param name="filenames">The names of the files to import.</param>
    /// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
    [Obsolete]
    public static void ImportAsciiToMultipleWorksheets(string[] filenames, AsciiImportOptions importOptions)
    {
      if (importOptions is not null)
        AsciiImporter.ImportFilesIntoSeparateNewTables(Main.ProjectFolder.RootFolder, filenames, true, importOptions);
      else
        AsciiImporter.ImportFilesIntoSeparateNewTables(Main.ProjectFolder.RootFolder, filenames, true, true);
    }

    /// <summary>
    /// Imports multiple Ascii files into a single data table, horizontally, i.e. in subsequent columns.
    /// </summary>
    /// <param name="dataTable">The data table where to import the data.</param>
    /// <param name="filenames">The files names. The names will be sorted before use.</param>
    /// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
    [Obsolete]
    public static void ImportAsciiToSingleWorksheetHorizontally(this DataTable dataTable, string[] filenames, AsciiImportOptions importOptions)
    {
      if (importOptions is not null)
        AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(dataTable, filenames, true, importOptions, out _);
      else
        AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(dataTable, filenames, true, true, out _);
    }

    /// <summary>
    /// Imports multiple Ascii files into a single data table, vertically, i.e. in subsequent rows.
    /// </summary>
    /// <param name="dataTable">The data table where to import the data.</param>
    /// <param name="filenames">The files names. The names will be sorted before use.</param>
    /// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
    [Obsolete]
    public static void ImportAsciiToSingleWorksheetVertically(this DataTable dataTable, string[] filenames, AsciiImportOptions importOptions)
    {
      if (importOptions is not null)
        AsciiImporter.TryImportFromMultipleAsciiFilesVertically(dataTable, filenames, true, importOptions, out _);
      else
        AsciiImporter.TryImportFromMultipleAsciiFilesVertically(dataTable, filenames, true, true, out _);
    }

    /// <summary>
    /// Asks for file name(s) and imports the file(s) into multiple worksheets.
    /// </summary>
    /// <param name="dataTable">The data table to import to. Can be null (in this case the data are imported into a new data table).</param>
    public static void ShowImportAsciiDialog(this DataTable dataTable)
    {
      ShowImportAsciiDialog(dataTable, true, false);
    }

    /// <summary>
    /// Asks for file name(s) and imports the file(s) into multiple worksheets.
    /// </summary>
    /// <param name="dataTable">The data table to import to. Can be null (in this case the data are imported into a new data table).</param>
    public static void ShowImportAsciiDialogAndOptions(this DataTable dataTable)
    {
      ShowImportAsciiDialogAndOptions(dataTable, true, false);
    }

    /// <summary>
    /// Asks for file name(s) and imports the file(s) into one or multiple worksheets.
    /// </summary>
    /// <param name="dataTable">The data table to import to. Can be null if <paramref name="toMultipleWorksheets"/> is set to <c>true</c>.</param>
    /// <param name="toMultipleWorksheets">If true, multiple files are imported into multiple worksheets. New worksheets were then created automatically.</param>
    /// <param name="vertically">If <c>toMultipleWorksheets</c> is false, and this option is true, the data will be exported vertically (in the same columns) instead of horizontally.</param>
    public static void ShowImportAsciiDialog(this DataTable dataTable, bool toMultipleWorksheets, bool vertically)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.RestoreDirectory = true;
      options.Multiselect = true;

      if (Current.Gui.ShowOpenFileDialog(options) && options.FileNames.Length > 0)
      {
        if (toMultipleWorksheets)
        {
          if (options.FileNames.Length == 1 && dataTable is not null)
          {
            AsciiImporter.ImportFromAsciiFile(dataTable, options.FileName);
          }
          else
          {
            AsciiImporter.ImportFilesIntoSeparateNewTables(Main.ProjectFolder.RootFolder, options.FileNames, true, false);
          }
        }
        else
        {
          bool success;
          string? errors;
          if (vertically)
          {
            success = AsciiImporter.TryImportFromMultipleAsciiFilesVertically(dataTable, options.FileNames, true, false, out errors);
          }
          else
          {
            success = AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(dataTable, options.FileNames, true, false, out errors);
          }

          if (!success && !string.IsNullOrEmpty(errors))
            Current.Gui.ErrorMessageBox(errors);
        }
      }
    }

    /// <summary>
    /// Asks for file name(s) and imports the file(s) into one or multiple worksheets. After the user chooses one or multiple files, one of the chosen files is used for analysis.
    /// The result of the structure analysis of this file is then presented to the user. The user may change some of the options and starts a re-analysis. Finally, the import options
    /// are confirmed by the user and the import process can start.
    /// </summary>
    /// <param name="dataTable">The data table to import to. Can be null if <paramref name="toMultipleWorksheets"/> is set to <c>true</c>.</param>
    /// <param name="toMultipleWorksheets">If true, multiple files are imported into multiple worksheets. New worksheets were then created automatically.</param>
    /// <param name="vertically">If <c>toMultipleWorksheets</c> is false, and this option is true, the data will be exported vertically (in the same columns) instead of horizontally.</param>
    public static void ShowImportAsciiDialogAndOptions(this DataTable dataTable, bool toMultipleWorksheets, bool vertically)
    {
      var options = new Altaxo.Gui.OpenFileOptions();
      options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.RestoreDirectory = true;
      options.Multiselect = true;

      if (Current.Gui.ShowOpenFileDialog(options) && options.FileNames.Length > 0)
      {
        var analysisOptions = dataTable.GetPropertyValue(AsciiDocumentAnalysisOptions.PropertyKeyAsciiDocumentAnalysisOptions, null) ?? throw new InvalidProgramException(); ;
        if (!ShowAsciiImportOptionsDialog(options.FileName, analysisOptions, out var importOptions))
          return;

        if (toMultipleWorksheets)
        {
          AsciiImporter.ImportFilesIntoSeparateNewTables(Main.ProjectFolder.RootFolder, options.FileNames, true, importOptions);
        }
        else
        {
          bool success;
          string? errors;
          if (vertically)
          {
            success = AsciiImporter.TryImportFromMultipleAsciiFilesVertically(dataTable, options.FileNames, true, importOptions, out errors);
          }
          else
          {
            success = AsciiImporter.TryImportFromMultipleAsciiFilesHorizontally(dataTable, options.FileNames, true, importOptions, out errors);
          }
          if (!success && !string.IsNullOrEmpty(errors))
            Current.Gui.ErrorMessageBox(errors);
        }
      }
    }

    /// <summary>
    /// Shows the ASCII analysis dialog.
    /// </summary>
    /// <param name="fileName">Name of the file to analyze.</param>
    /// <param name="importOptions">On return, contains the ASCII import options the user has confirmed.</param>
    /// <param name="analysisOptions">Options that specify how many lines are analyzed, and what number formats and date/time formats will be tested.</param>
    /// <returns><c>True</c> if the user confirms this dialog (clicks OK). False if the user cancels this dialog.</returns>
    public static bool ShowAsciiImportOptionsDialog(string fileName, AsciiDocumentAnalysisOptions analysisOptions, out AsciiImportOptions importOptions)
    {
      if (analysisOptions is null)
        throw new ArgumentNullException(nameof(analysisOptions));

      importOptions = new AsciiImportOptions();

      using (FileStream str = AsciiImporter.GetAsciiInputFileStream(fileName))
      {
        importOptions = AsciiDocumentAnalysis.Analyze(new AsciiImportOptions(), str, analysisOptions);
        object[] args = new object[] { importOptions, str };
        var controller = (Altaxo.Gui.IMVCAController?)Current.Gui.GetControllerAndControl(args, typeof(Altaxo.Gui.IMVCAController), Gui.UseDocument.Directly);
        if (controller is null)
          throw new InvalidProgramException($"Controller not found for import options");

        if (!Current.Gui.ShowDialog(controller, "Choose Ascii import options"))
          return false;

        importOptions = (AsciiImportOptions)controller.ModelObject;
        return true;
      }
    }

    /// <summary>
    /// Asks for a file name and exports the table data into that file as Ascii.
    /// </summary>
    /// <param name="dataTable">DataTable to export.</param>
    public static void ShowExportAsciiDialog(this DataTable dataTable)
    {
      var exportOptions = dataTable.GetPropertyValue(AsciiExportOptions.PropertyKeyAsciiExportOptions, () => null);

      // we will show the export options in any case,
      // but the modified options are stored only in the application settings

      exportOptions ??= new AsciiExportOptions();

      {
        object exportOptionsObj = exportOptions;
        if (!Current.Gui.ShowDialog(ref exportOptionsObj, "Edit Ascii export options"))
        {
          return;
        }
        exportOptions = (AsciiExportOptions)exportOptionsObj;
        Current.PropertyService.UserSettings.SetValue(AsciiExportOptions.PropertyKeyAsciiExportOptions, exportOptions);
      }


      var options = new Altaxo.Gui.SaveFileOptions();
      options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.RestoreDirectory = true;

      if (Current.Gui.ShowSaveFileDialog(options))
      {
        using (Stream myStream = new FileStream(options.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
          try
          {
            var exporter = new AsciiExporter(exportOptions);
            exporter.ExportAscii(myStream, dataTable);
          }
          catch (Exception ex)
          {
            Current.Gui.ErrorMessageBox("There was an error during ascii export, details follow:\n" + ex.ToString());
          }
          finally
          {
            myStream.Close();
          }
        }
      }
    }

    /// <summary>
    /// Shows the dialog for Galactic SPC file export, and exports the data of the table using the options provided in that dialog.
    /// </summary>
    /// <param name="dataTable">DataTable to export.</param>
    /// <param name="selectedDataRows">Rows to export (can be null - then all rows will be considered for export).</param>
    /// <param name="selectedDataColumns">Columns to export (can be null - then all columns will be considered for export).</param>
    public static void ShowExportGalacticSPCDialog(this DataTable dataTable, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
    {
      var exportCtrl = new Altaxo.Gui.Serialization.Galactic.ExportGalacticSpcFileDialogController(dataTable, selectedDataRows, selectedDataColumns);
      Current.Gui.ShowDialog(exportCtrl, "Export Galactic SPC format");
    }
  }
}
