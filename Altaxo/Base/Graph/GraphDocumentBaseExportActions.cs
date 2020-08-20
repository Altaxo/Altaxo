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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.MultiRename;
using Altaxo.Main.Commands;

namespace Altaxo.Graph
{
  /// <summary>
  /// Common expoert actions for all classes derived from GraphDocument.
  /// </summary>
  public static class GraphDocumentBaseExportActions
  {
    private static IList<KeyValuePair<string, string>> GetFileFilterString(ImageFormat fmt)
    {
      var filter = new List<KeyValuePair<string, string>>();

      if (fmt == ImageFormat.Bmp)
        filter.Add(new KeyValuePair<string, string>("*.bmp", "Bitmap files (*.bmp)"));
      else if (fmt == ImageFormat.Emf)
        filter.Add(new KeyValuePair<string, string>("*.emf", "Enhanced metafiles (*.emf)"));
      else if (ImageFormat.Exif == fmt)
        filter.Add(new KeyValuePair<string, string>("*.exi", "Exif files (*.exi)"));
      else if (ImageFormat.Gif == fmt)
        filter.Add(new KeyValuePair<string, string>("*.gif", "Gif files (*.gif)"));
      else if (ImageFormat.Icon == fmt)
        filter.Add(new KeyValuePair<string, string>("*.ico", "Icon files (*.ico)"));
      else if (ImageFormat.Jpeg == fmt)
        filter.Add(new KeyValuePair<string, string>("*.jpg", "Jpeg files (*.jpg)"));
      else if (ImageFormat.Png == fmt)
        filter.Add(new KeyValuePair<string, string>("*.png", "Png files (*.png)"));
      else if (ImageFormat.Tiff == fmt)
        filter.Add(new KeyValuePair<string, string>("*.tif", "Tiff files (*.tif)"));
      else if (ImageFormat.Wmf == fmt)
        filter.Add(new KeyValuePair<string, string>("*.wmf", "Windows metafiles (*.wmf)"));

      filter.Add(new KeyValuePair<string, string>("*.*", "All files (*.*)"));

      return filter;
    }

    public static GraphExportOptions _graphExportOptionsToFile { get; private set; } = new GraphExportOptions();


    /// <summary>
    /// Shows the graph export options dialog and thus sets the current graph export options.
    /// </summary>
    /// <returns>True if the user accepts the dialog with OK; false if the user cancels this dialog.</returns>
    public static bool ShowGraphExportOptionsDialog()
    {
      object resopt = _graphExportOptionsToFile;
      if (Current.Gui.ShowDialog(ref resopt, "Choose export options"))
      {
        _graphExportOptionsToFile = (GraphExportOptions)resopt;
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>Shows the dialog to choose the graph export options, and then the multi file export dialog.</summary>
    /// <param name="documents">List with graph documents to export.</param>
    public static void ShowExportMultipleGraphsDialogAndExportOptions(IEnumerable<Graph.GraphDocumentBase> documents)
    {
      if (ShowGraphExportOptionsDialog())
      {
        ShowExportMultipleGraphsDialog(documents);
      }
    }

    /// <summary>Shows the multi file export dialog and exports the graphs, using the <see cref="GraphExportOptions"/> that are stored in this class.</summary>
    /// <param name="documents">List with graph documents to export.</param>
    public static void ShowExportMultipleGraphsDialog(IEnumerable<Graph.GraphDocumentBase> documents)
    {
      var mrData = new MultiRenameData() { IsRenameOperationFileSystemBased = true };
      MultiRenameDocuments.RegisterCommonDocumentShortcutsForFileOperations(mrData);
      mrData.RegisterStringShortcut("E", (o, i) => _graphExportOptionsToFile.GetDefaultFileNameExtension(), "File extension (depends on the image type that was chosen before");

      mrData.RegisterRenameActionHandler(DoExportGraphs);

      mrData.AddObjectsToRename(documents);

      mrData.RegisterListColumn("FullName", MultiRenameDocuments.GetFullNameWithAugmentingProjectFolderItems);
      mrData.RegisterListColumn("File name", (obj, newName) => newName);
      mrData.RegisterListColumn("Creation date", MultiRenameDocuments.GetCreationDateString);

      mrData.DefaultPatternString = "[SN][E]";
      mrData.IsRenameOperationFileSystemBased = true;

      var mrController = new MultiRenameController();
      mrController.InitializeDocument(mrData);
      Current.Gui.ShowDialog(mrController, "Export multiple graphs");
    }

    private static List<object> DoExportGraphs(MultiRenameData mrData)
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
        var graph = (GraphDocumentBase)mrData.GetObjectToRename(i);
        var fileName = mrData.GetNewNameForObject(i);
        try
        {
          DoExportGraph(graph, fileName, _graphExportOptionsToFile);
        }
        catch (Exception ex)
        {
          failedItems.Add(graph);
          errors.AppendFormat("Graph {0} -> file name {1}: export failed, {2}\n", graph.Name, fileName, ex.Message);
        }
      }

      if (errors.Length != 0)
        Current.Gui.ErrorMessageBox(errors.ToString(), "Export failed for some items");
      else
        Current.Gui.InfoMessageBox(string.Format("{0} graphs successfully exported.", mrData.ObjectsToRenameCount));

      return failedItems;
    }

    public static void DoExportGraph(Graph.GraphDocumentBase doc, string fileName)
    {
      DoExportGraph(doc, fileName, _graphExportOptionsToFile);
    }

    public static void DoExportGraph(Graph.GraphDocumentBase doc, string fileName, Graph.Gdi.GraphExportOptions graphExportOptions)
    {
      if (!System.IO.Path.IsPathRooted(fileName))
        throw new ArgumentException("Path is not rooted!");

      var fileNamePart = System.IO.Path.GetFileName(fileName);
      var pathPart = System.IO.Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException($"Unable to get path of file name {fileName}");
      if (true && !System.IO.Directory.Exists(pathPart))
      {
        System.IO.Directory.CreateDirectory(pathPart);
      }

      var exporter = Current.ProjectService.GetProjectItemImageExporter(doc);

      if (null == exporter)
        throw new ArgumentException("Did not find exporter for document of type " + doc?.GetType().ToString() ?? string.Empty, nameof(doc));

      using (Stream myStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
      {
        exporter.ExportAsImageToStream(doc, graphExportOptions, myStream);
        myStream.Close();
      }
    }

    public static void ShowFileExportSpecificDialog(this GraphDocumentBase doc)
    {
      object resopt = _graphExportOptionsToFile;
      if (Current.Gui.ShowDialog(ref resopt, "Choose export options"))
      {
        _graphExportOptionsToFile = (GraphExportOptions)resopt;
      }
      else
      {
        return;
      }
      ShowFileExportDialog(doc, _graphExportOptionsToFile);
    }

    public static void ShowFileExportDialog(this GraphDocumentBase doc, GraphExportOptions graphExportOptions)
    {
      var saveOptions = new Altaxo.Gui.SaveFileOptions();
      var list = GetFileFilterString(graphExportOptions.ImageFormat);
      foreach (var entry in list)
        saveOptions.AddFilter(entry.Key, entry.Value);
      saveOptions.FilterIndex = 0;
      saveOptions.RestoreDirectory = true;

      if (Current.Gui.ShowSaveFileDialog(saveOptions))
      {
        using (Stream myStream = new FileStream(saveOptions.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) // we need FileAccess.ReadWrite when exporting to EMF/WMF format
        {
          var exporter = Current.ProjectService.GetProjectItemImageExporter(doc);
          exporter?.ExportAsImageToStream(doc, graphExportOptions, myStream);
          myStream.Close();
        } // end openfile ok
      } // end dlgresult ok
    }

    public static void ShowFileExportMetafileDialog(this GraphDocument doc)
    {
      var opt = new GraphExportOptions();
      opt.TrySetImageAndPixelFormat(ImageFormat.Emf, PixelFormat.Format32bppArgb);
      ShowFileExportDialog(doc, opt);
    }

    public static void ShowFileExportTiffDialog(this GraphDocument doc)
    {
      var opt = new GraphExportOptions();
      opt.TrySetImageAndPixelFormat(ImageFormat.Tiff, PixelFormat.Format32bppArgb);
      opt.SourceDpiResolution = 300;
      opt.DestinationDpiResolution = 300;
      ShowFileExportDialog(doc, opt);
    }

    /// <summary>
    /// Renders the graph document as enhanced metafile image in vector format with the options given in <paramref name="renderingOptions"/>
    /// </summary>
    /// <param name="document">The graph document used.</param>
    /// <param name="renderingOptions">The embedded rendering export options.</param>
    /// <returns>The rendered enhanced metafile.</returns>
    public static System.Drawing.Imaging.Metafile RenderAsEnhancedMetafileVectorFormat(GraphDocumentBase document, EmbeddedObjectRenderingOptions renderingOptions)
    {
      if (document is Gdi.GraphDocument)
        return Gdi.GraphDocumentExportActions.RenderAsEnhancedMetafileVectorFormat((Gdi.GraphDocument)document, renderingOptions);
      else
        throw new NotImplementedException("Render as metafile is not supported for graph documents of type " + document.GetType());
    }

    /// <summary>
    /// Renders the graph document as bitmap with default PixelFormat.Format32bppArgb.
    /// </summary>
    /// <param name="document">The graph document used.</param>
    /// <param name="renderingOptions">The embedded rendering export options.</param>
    /// <param name="pixelFormat">The pixel format for the bitmap. Default is PixelFormat.Format32bppArgb.</param>
    /// <returns>The rendered enhanced metafile.</returns>
    public static System.Drawing.Bitmap? RenderAsBitmap(GraphDocumentBase document, EmbeddedObjectRenderingOptions renderingOptions, PixelFormat pixelFormat)
    {
      if (document is Gdi.GraphDocument)
        return Gdi.GraphDocumentExportActions.RenderAsBitmap((Gdi.GraphDocument)document, renderingOptions, pixelFormat);
      else
        return Graph3D.GraphDocumentExportActions.RenderAsBitmap((Graph3D.GraphDocument)document, renderingOptions, pixelFormat);
    }
  }
}
