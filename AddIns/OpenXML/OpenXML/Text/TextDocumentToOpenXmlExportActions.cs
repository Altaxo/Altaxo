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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Altaxo.Graph;
using Altaxo.Gui;
using Altaxo.Text.Renderers;
using Markdig;

namespace Altaxo.Text
{
  public class TextDocumentToOpenXmlExportActions
  {
    /// <summary>
    /// Gets the output file by showing a save file dialog.
    /// </summary>
    /// <returns></returns>
    public static (bool dialogResult, string outputFileName) ShowGetOutputFileDialog(string oldFileName = null)
    {
      var dlg = new SaveFileOptions();
      dlg.AddFilter("*.docx", "Docx files (*.docx)");
      dlg.AddFilter("*.*", "All files (*.*)");
      dlg.AddExtension = true;

      if (null != oldFileName)
      {
        dlg.InitialDirectory = System.IO.Path.GetDirectoryName(oldFileName);
        dlg.FileName = oldFileName;
      }

      var dialogResult = Current.Gui.ShowSaveFileDialog(dlg);

      return (dialogResult, dlg.FileName);
    }

    /// <summary>
    /// Gets the template file by showing a open file dialog.
    /// </summary>
    /// <returns></returns>
    public static (bool dialogResult, string templateFileName) ShowGetTemplateFileDialog(string oldFileName = null)
    {
      var dlg = new OpenFileOptions();
      dlg.AddFilter("*.docx", "Docx files (*.docx)");
      dlg.AddFilter("*.*", "All files (*.*)");
      dlg.Title = "Select a .docx file used as style template";

      if (null != oldFileName)
      {
        dlg.InitialDirectory = System.IO.Path.GetDirectoryName(oldFileName);
        dlg.FileName = oldFileName;
      }
      var dialogResult = Current.Gui.ShowOpenFileDialog(dlg);
      return (dialogResult, dlg.FileName);
    }

    public static void ExportShowFileSaveDialogOnly(TextDocument document)
    {
      var exportOptions = document.GetPropertyValue(TextDocumentToOpenXmlExportOptionsAndData.PropertyKeyTextDocumentToOpenXMLExportOptionsAndData, () => new TextDocumentToOpenXmlExportOptionsAndData());

      var (result, fileName) = ShowGetOutputFileDialog(exportOptions.OutputFileName);

      if (result == true)
      {
        exportOptions.OutputFileName = fileName;
        SaveOptionsAndExport(document, exportOptions);
      }
    }

    /// <summary>
    /// Exports the <see cref="TextDocument"/> to a markdown file, showing first the file save dialog.
    /// </summary>
    /// <param name="document">The document to export.</param>
    public static void ExportShowDialog(TextDocument document)
    {
      var exportOptions = document.GetPropertyValue(TextDocumentToOpenXmlExportOptionsAndData.PropertyKeyTextDocumentToOpenXMLExportOptionsAndData, () => new TextDocumentToOpenXmlExportOptionsAndData());
      if (true == Current.Gui.ShowDialog(ref exportOptions, "OpenXml export", false))
      {
        SaveOptionsAndExport(document, exportOptions);
      }
    }

    private static void SaveOptionsAndExport(TextDocument document, TextDocumentToOpenXmlExportOptionsAndData exportOptions)
    {
      document.PropertyBagNotNull.SetValue(TextDocumentToOpenXmlExportOptionsAndData.PropertyKeyTextDocumentToOpenXMLExportOptionsAndData, (TextDocumentToOpenXmlExportOptionsAndData)exportOptions.Clone());
      Current.PropertyService.ApplicationSettings.SetValue(TextDocumentToOpenXmlExportOptionsAndData.PropertyKeyTextDocumentToOpenXMLExportOptionsAndData, (TextDocumentToOpenXmlExportOptionsAndData)exportOptions.Clone());

      var errors = new List<MarkdownError>();
      Export(document, exportOptions, exportOptions.OutputFileName, errors);

      if (errors.Count > 0)
      {
        var stb = new StringBuilder();
        stb.AppendLine("There were error(s) during export:");
        stb.AppendLine();

        foreach (var error in errors)
          stb.AppendLine(error.ToString());

        Current.Gui.ErrorMessageBox(stb.ToString(), "Export errors");
        return;
      }


      // Start Sandcastle help file builder
      if (exportOptions.OpenApplication)
      {
        System.Diagnostics.Process.Start(exportOptions.OutputFileName);
      }
    }

    /// <summary>
    /// Exports the specified <see cref="TextDocument"/> to an external markdown file.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="fileName">Full name of the Maml file to export to. Note that if exporting to multiple Maml files,
    /// this is the base file name only; the file names will be derived from this name.</param>
    /// <param name="errors">A list that collects error messages.</param>
    public static void Export(TextDocument document, TextDocumentToOpenXmlExportOptions o, string fileName, List<MarkdownError> errors = null)
    {
      if (null == document)
        throw new ArgumentNullException(nameof(document));
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException(nameof(fileName));
      var basePathName = Path.GetDirectoryName(fileName);


      if (o.ExpandChildDocuments)
      {
        document = ChildDocumentExpander.ExpandDocumentToNewDocument(document, errors: errors);
      }

      if (o.RenumerateFigures)
      {
        document.SourceText = FigureRenumerator.RenumerateFigures(document.SourceText);
      }

      // now export the markdown document as Maml file(s)

      // first parse it with Markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);

      var markdownDocument = Markdig.Markdown.Parse(document.SourceText, pipeline.Build());

      var renderer = new OpenXMLRenderer(
        wordDocumentFileName: fileName,
        localImages: document.Images,
        textDocumentFolder: document.Folder
        );


      if (o.MaximumImageWidth.HasValue)
        renderer.MaxImageWidthIn96thInch = o.MaximumImageWidth.Value.AsValueIn(Altaxo.Units.Length.Inch.Instance) * 96.0;
      if (o.MaximumImageHeight.HasValue)
        renderer.MaxImageHeigthIn96thInch = o.MaximumImageHeight.Value.AsValueIn(Altaxo.Units.Length.Inch.Instance) * 96.0;
      if (null != o.ThemeName)
        renderer.ThemeName = o.ThemeName;
      renderer.RemoveOldContentsOfTemplateFile = o.RemoveOldContentsOfTemplateFile;
      renderer.ImageResolution = o.ImageResolutionDpi;
      renderer.UseAutomaticFigureNumbering = o.UseAutomaticFigureNumbering;
      renderer.DoNotFormatFigureLinksAsHyperlinks = o.DoNotFormatFigureLinksAsHyperlinks;

      renderer.Render(markdownDocument);
    }


  }
}
