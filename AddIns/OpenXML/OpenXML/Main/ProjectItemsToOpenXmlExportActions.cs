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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Data;
using Altaxo.Gui.Common.MultiRename;
using Altaxo.Main.Commands;
using Altaxo.Text;

namespace Altaxo.Main
{
  public static class ProjectItemsToOpenXmlExportActions
  {
    /// <summary>
    /// Shows the multi file export dialog and exports the project items given in the argument (tables, graphs, and notes).
    /// </summary>
    /// <param name="documents">List with <see cref="IProjectItem"/>s to export.</param>
    public static void ShowExportMultipleProjectItemsDialog(IEnumerable<IProjectItem> documents)
    {
      if (documents is null)
        throw new ArgumentNullException(nameof(documents));


      if (documents.OfType<Altaxo.Graph.GraphDocumentBase>().Any())
      {
        // If graphs are to be exported, show the graph export options dialog
        if (!Graph.GraphDocumentBaseExportActions.ShowGraphExportOptionsDialog())
          return;
      }

      if (documents.OfType<Altaxo.Text.TextDocument>().Any())
      {
        // If notes are to be exported, show the text document export options dialog
        var exportOptions = Current.PropertyService.UserSettings.GetValue(TextDocumentToOpenXmlExportOptions.PropertyKeyTextDocumentToOpenXmlExportOptions, new TextDocumentToOpenXmlExportOptions());
        if (true != Current.Gui.ShowDialog(ref exportOptions, "Export options for text documents", false))
          return;
        Current.PropertyService.UserSettings.SetValue(TextDocumentToOpenXmlExportOptions.PropertyKeyTextDocumentToOpenXmlExportOptions, exportOptions!);
      }


      var mrData = new MultiRenameData();
      MultiRenameDocuments.RegisterCommonDocumentShortcutsForFileOperations(mrData);
      mrData.RegisterStringShortcut(
        "E",
        (o, i) =>
        {
          if (o is Altaxo.Data.DataTable)
            return ".xlsx";
          else if (o is Altaxo.Text.TextDocument)
            return ".docx";
          else if (o is Altaxo.Graph.GraphDocumentBase)
            return Graph.GraphDocumentBaseExportActions._graphExportOptionsToFile.GetDefaultFileNameExtension();
          else
            return string.Empty;
        },

        "File extension (depending on the item type)");

      mrData.RegisterRenameActionHandler(DoExportMicrosoftFiles);

      mrData.AddObjectsToRename(documents);

      mrData.RegisterListColumn("FullName", MultiRenameDocuments.GetFullNameWithAugmentingProjectFolderItems);
      mrData.RegisterListColumn("File name", (obj, newName) => newName);
      mrData.RegisterListColumn("Creation date", MultiRenameDocuments.GetCreationDateString);

      mrData.DefaultPatternString = "[PA]\\[SN][E]";
      mrData.IsRenameOperationFileSystemBased = true;

      var mrController = new MultiRenameController();
      mrController.InitializeDocument(mrData);
      Current.Gui.ShowDialog(mrController, "Export multiple project items");
    }

    private static List<object> DoExportMicrosoftFiles(MultiRenameData mrData)
    {
      List<object> failedItems = null!;
      StringBuilder errors = null!;
      Current.Gui.ExecuteAsUserCancellable(1000, (reporter) => (failedItems, errors) = DoExportMicrosoftFiles(mrData, reporter));

      if (errors.Length != 0)
        Current.Gui.ErrorMessageBox(errors.ToString(), "Export failed for some items");
      else
        Current.Gui.InfoMessageBox($"{mrData.ObjectsToRenameCount - failedItems.Count} project items successfully exported.");

      return failedItems!;
    }

    private static (List<object> FailedItems, StringBuilder Errors) DoExportMicrosoftFiles(MultiRenameData mrData, IProgressReporter reporter)
    {
      var failedItems = new List<object>();
      var errors = new StringBuilder();

      string? firstNotRootedFilePath = null;
      object? firstNotRootedObject = null;
      for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
      {
        var fileName = mrData.GetNewNameForObject(i);
        if (!System.IO.Path.IsPathRooted(fileName))
        {
          firstNotRootedFilePath = fileName;
          firstNotRootedObject = mrData.GetObjectToRename(i);
          break;
        }
      }

      if (firstNotRootedFilePath is not null)
      {
        errors.AppendLine($"Some of the items will be exported to a non-rooted file path.");
        errors.AppendLine($"The first such item is {(firstNotRootedObject is INamedObject no ? no.Name : firstNotRootedObject)} resulting in file {firstNotRootedFilePath}.");
        for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
        {
          failedItems.Add(mrData.GetObjectToRename(i)); // add remaining items to failed list
        }
        return (failedItems, errors);
      }

      for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
      {
        var projectItem = (IProjectItem)mrData.GetObjectToRename(i);
        var fileName = mrData.GetNewNameForObject(i);

        try
        {
          // Create the directory, if not already present
          var dir = System.IO.Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException($"Can not get directory name from file name {fileName}");
          if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

          // now export the project item
          if (projectItem is DataTable dataTable)
          {
            ExcelExporter.ExportToExcel(dataTable, fileName);
          }
          else if (projectItem is Altaxo.Text.TextDocument textDocument)
          {
            var exportOptions = Current.PropertyService.UserSettings.GetValue(TextDocumentToOpenXmlExportOptions.PropertyKeyTextDocumentToOpenXmlExportOptions);
            TextDocumentToOpenXmlExportActions.Export(textDocument, exportOptions, fileName);
          }
          else if (projectItem is Altaxo.Graph.GraphDocumentBase graphDocument)
          {
            Graph.GraphDocumentBaseExportActions.DoExportGraph(graphDocument, fileName);
          }
        }
        catch (Exception ex)
        {
          failedItems.Add(projectItem);
          errors.AppendFormat("Item {0} -> file name {1}: export failed, {2}\n", projectItem.Name, fileName, ex.Message);
        }

        if (reporter.CancellationPending)
        {
          for (i = i + 1; i < mrData.ObjectsToRenameCount; ++i)
          {
            failedItems.Add(mrData.GetObjectToRename(i)); // add remaining items to failed list
          }
          break;
        }
        if (reporter.ShouldReportNow)
        {
          reporter.ReportProgress($"Processed item: {projectItem.Name}", (i + 1d) / mrData.ObjectsToRenameCount);
        }
      }



      return (failedItems, errors);
    }
  }
}
