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
using System.Threading.Tasks;
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
        Current.PropertyService.UserSettings.SetValue(TextDocumentToOpenXmlExportOptions.PropertyKeyTextDocumentToOpenXmlExportOptions, exportOptions);
      }


      var mrData = new MultiRenameData();
      MultiRenameDocuments.RegisterCommonDocumentShortcuts(mrData);
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

      mrData.RegisterListColumn("FullName", MultiRenameDocuments.GetFullName);
      mrData.RegisterListColumn("File name", null);
      mrData.RegisterListColumn("Creation date", MultiRenameDocuments.GetCreationDateString);

      mrData.DefaultPatternString = "[PA]\\[SN][E]";

      var mrController = new MultiRenameController();
      mrController.InitializeDocument(mrData);
      Current.Gui.ShowDialog(mrController, "Export multiple project items");
    }

    private static List<object> DoExportMicrosoftFiles(MultiRenameData mrData)
    {
      var failedItems = new List<object>();
      var errors = new StringBuilder();

      bool allPathsRooted = true;
      for (int i = 0; i < mrData.ObjectsToRenameCount; ++i)
      {
        var fileName = mrData.GetNewNameForObject(i);
        if (!System.IO.Path.IsPathRooted(fileName))
        {
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
        var projectItem = (IProjectItem)mrData.GetObjectToRename(i);
        var fileName = mrData.GetNewNameForObject(i);

        try
        {
          // Create the directory, if not already present
          var dir = System.IO.Path.GetDirectoryName(fileName);
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
      }

      if (errors.Length != 0)
        Current.Gui.ErrorMessageBox(errors.ToString(), "Export failed for some items");
      else
        Current.Gui.InfoMessageBox(string.Format("{0} project items successfully exported.", mrData.ObjectsToRenameCount));

      return failedItems;
    }
  }
}
