#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Text;
using Altaxo.Gui.Common.MultiRename;
using Altaxo.Main.Commands;
using Altaxo.Serialization.Ascii;
using Altaxo.Worksheet.Commands;

#nullable enable

namespace Altaxo.Data
{
  /// <summary>
  /// Supports the import of ASCII data into multiple, already existing, tables.
  /// </summary>
  public static class ImportIntoMultipleTablesAction
  {
    /// <summary>
    /// Shows the dialog that allows importing multiple files into multiple tables.
    /// </summary>
    /// <param name="documents">The tables, in which the files should be imported (one file for each table).</param>
    /// <exception cref="ArgumentNullException">documents</exception>
    public static void ShowImportIntoMultipleTablesDialog(IEnumerable<DataTable> documents)
    {
      if (documents is null)
        throw new ArgumentNullException(nameof(documents));

      var mrData = new MultiRenameData() { IsRenameOperationFileSystemBased = true, ShowExistingFileInformation = true };
      MultiRenameDocuments.RegisterCommonDocumentShortcutsForFileOperations(mrData);
      mrData.RegisterStringShortcut("E", (o, i) => ".txt", "File extension (fixed here to .txt");

      mrData.RegisterRenameActionHandler(DoImportFiles);

      mrData.AddObjectsToRename(documents);

      mrData.RegisterListColumn("FullName", MultiRenameDocuments.GetFullNameWithAugmentingProjectFolderItems);
      mrData.RegisterListColumn("File name", (obj, newName) => newName);
      mrData.RegisterListColumn("File state", GetFileState);


      mrData.DefaultPatternString = "[SN][E]";
      mrData.IsRenameOperationFileSystemBased = true;

      var mrController = new MultiRenameController();
      mrController.InitializeDocument(mrData);
      Current.Gui.ShowDialog(mrController, "Import into multiple existing tables");
    }

    private static string GetFileState(object o, string fileName)
    {
      try
      {
        if (!System.IO.Path.IsPathRooted(fileName))
          return "Path not rooted!";
        else if (!System.IO.File.Exists(fileName))
          return "File does not exist!";
        else
          return "OK";
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    private static List<object> DoImportFiles(MultiRenameData mrData)
    {
      if (mrData is null)
        throw new ArgumentNullException(nameof(mrData));

      var failedItems = new List<object>();
      var errors = new StringBuilder();

      bool allPathsRooted = true;
      for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
      {
        var fileName = mrData.GetNewNameForObject(i);
        if (!System.IO.Path.IsPathRooted(fileName))
        {
          errors.AppendFormat($"File name {fileName}: Path is not a rooted path!\n");
          allPathsRooted = false;
          break;
        }
      }

      if (!allPathsRooted)
      {
        //Current.Gui.ShowFolderDialog();
        // http://wpfdialogs.codeplex.com/
      }

      for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
      {
        var doc = (DataTable)mrData.GetObjectToRename(i);
        var fileName = mrData.GetNewNameForObject(i);


        try
        {
          if (System.IO.File.Exists(fileName))
          {
            DoImportIntoTable(doc, fileName);
          }
          else
          {
            errors.AppendFormat($"Table {doc.Name} -> file name {fileName}: File does not exist!\n");
          }
        }
        catch (Exception ex)
        {
          failedItems.Add(doc);
          errors.AppendFormat($"Table {doc.Name} -> file name {fileName}: import failed, {ex.Message}\n");
        }
      }

      if (errors.Length != 0)
        Current.Gui.ErrorMessageBox(errors.ToString(), "Import failed for some items");
      else
        Current.Gui.InfoMessageBox($"{mrData.ObjectsToRenameCount} files successfully imported.");

      return failedItems;
    }

    /// <summary>
    /// Worker that imports one file into one table. If a import data source exists, the file of the existing data source is replaced
    /// by the new file. Otherwise, the data is imported, using Ascii import.
    /// </summary>
    /// <param name="doc">The table in which the content of the file should be imported.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <exception cref="ArgumentNullException">doc</exception>
    /// <exception cref="ArgumentException">Path is not rooted! - fileName</exception>
    public static void DoImportIntoTable(DataTable doc, string fileName)
    {
      if (doc is null)
        throw new ArgumentNullException(nameof(doc));
      if (fileName is null)
        throw new ArgumentNullException(nameof(doc));
      if (!System.IO.Path.IsPathRooted(fileName))
        throw new ArgumentException("Path is not rooted!", nameof(fileName));

      // if the table has an import data source, then
      // this source is used, changing the filename

      if (doc.DataSource is Serialization.Ascii.AsciiImportDataSource aids)
      {
        aids.SourceFileName = fileName;
        DataSourceCommands.RequeryTableDataSource(doc);
      }
      else if (doc.DataSource is FileImportScriptDataSource fisds)
      {
        fisds.SourceFileName = fileName;
        DataSourceCommands.RequeryTableDataSource(doc);
      }
      else
      {
        AsciiImporter.ImportFromAsciiFile(doc, fileName);
      }
    }
  }
}
