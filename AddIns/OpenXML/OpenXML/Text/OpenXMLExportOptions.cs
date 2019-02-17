#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
  /// <summary>
  /// Options to export a <see cref="TextDocument"/> into a OpenXML (MS Word) file, including all the referenced graphs and local images.
  /// </summary>
  public class OpenXMLExportOptions : ICloneable
  {

    #region "Serialization"

    /// <summary>
    /// Created 2018-12-07
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenXMLExportOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OpenXMLExportOptions)obj;

        info.AddValue("ExpandChildDocuments", s.ExpandChildDocuments);
        info.AddValue("MaxImageHeight", s.MaximumImageHeight);
        info.AddValue("MaxImageHeight", s.MaximumImageHeight);
        info.AddValue("ImageResolution", s.ImageResolutionDpi);

        info.AddValue("ThemeName", s.ThemeName);
        info.AddValue("RemoveOldContent", s.RemoveOldContentsOfTemplateFile);
        info.AddValue("OpenApplication", s.OpenApplication);
        info.AddValue("OutputFileName", s.OutputFileName);

        info.AddValue("RenumerateFigures", s.RenumerateFigures);
        info.AddValue("UseAutomaticFigureNumbering", s.UseAutomaticFigureNumbering);
        info.AddValue("DoNotFormatFigureLinksAsHyperlinks", s.DoNotFormatFigureLinksAsHyperlinks);

      }

      public void Deserialize(OpenXMLExportOptions s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        s.ExpandChildDocuments = info.GetBoolean("ExpandChildDocuments");
        s.MaximumImageWidth = (Altaxo.Units.DimensionfulQuantity?)info.GetValue("MaxImageWidth", s);
        s.MaximumImageHeight = (Altaxo.Units.DimensionfulQuantity?)info.GetValue("MaxImageHeight", s);
        //if (info.CurrentElementName == "ImageResolution")
        s.ImageResolutionDpi = info.GetInt32("ImageResolution");

        s.ThemeName = info.GetString("ThemeName");
        //if (info.CurrentElementName == "RemoveOldContent")
        s.RemoveOldContentsOfTemplateFile = info.GetBoolean("RemoveOldContent");
        s.OpenApplication = info.GetBoolean("OpenApplication");
        s.OutputFileName = info.GetString("OutputFileName");

        if (info.CurrentElementName == "RenumerateFigures")
        {
          s.RenumerateFigures = info.GetBoolean("RenumerateFigures");
          s.UseAutomaticFigureNumbering = info.GetBoolean("UseAutomaticFigureNumbering");
          s.DoNotFormatFigureLinksAsHyperlinks = info.GetBoolean("DoNotFormatFigureLinksAsHyperlinks");
        }
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (OpenXMLExportOptions)o ?? new OpenXMLExportOptions();
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion "Serialization"

    public object Clone()
    {
      return MemberwiseClone();
    }

    #region Properties

    public string ThemeName { get; set; } = "GitHub";

    /// <summary>
    /// Gets or sets a value indicating whether the old contents of the .docx file used as style template should be removed.
    /// If set to false, the content is kept, and the new content is appended to the end of the document.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the old contents of the template file should be removed; otherwise, <c>false</c>.
    /// </value>
    public bool RemoveOldContentsOfTemplateFile { get; set; } = true;

    /// <summary>
    /// Gets or sets the image resolution of the graphs that must be rendered in Dpi.
    /// </summary>
    /// <value>
    /// The image resolution in dpi.
    /// </value>
    public int ImageResolutionDpi { get; set; } = 600;

    /// <summary>
    /// Gets or sets the output file. This is preferably a Sandcastle help file builder project file, but can also be a layout content file (.content) or a Maml file (.aml).
    /// </summary>
    public string OutputFileName { get; set; }


    /// <summary>
    /// If true, included child documents are expanded before the markdown document is processed.
    /// </summary>
    public bool ExpandChildDocuments { get; set; } = true;

    /// <summary>
    /// If true, figures are renumerated and the links to those figures updated.
    /// </summary>
    public bool RenumerateFigures { get; set; } = true;

    /// <summary>
    /// If true, in the exported OpenXML document, the figures are numbered automatically, and the links will be replaced by fields.
    /// </summary>
    public bool UseAutomaticFigureNumbering { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether links to figures are formatted as hyperlinks or not.
    /// Explanation: in MS Word, it seems not possible to have a reference to a text marker hyperlink formatted.
    /// But, for automatic figure numbering and referencing we need references to text markers.
    /// </summary>
    /// <value>
    /// <c>true</c> if links to figures should not be hyperlink formatted.; otherwise, <c>false</c>.
    /// </value>
    public bool DoNotFormatFigureLinksAsHyperlinks { get; set; } = true;

    /// <summary>
    /// If true, the application that is linked to .docx format will be opened.
    /// </summary>
    public bool OpenApplication { get; set; } = true;


    /// <summary>
    /// Gets or sets the maximum width of exported images in points (1/72nd inch).
    /// </summary>
    /// <value>
    /// The maximum width of exported images (1/72nd inch).
    /// </value>
    public Altaxo.Units.DimensionfulQuantity? MaximumImageWidth { get; set; } // = new Units.DimensionfulQuantity(16, new Units.PrefixedUnit(Units.SIPrefix.Centi, Units.Length.Meter.Instance));

    /// <summary>
    /// Gets or sets the maximum height of exported images in points (1/72nd inch).
    /// </summary>
    /// <value>
    /// The maximum height of exported images (1/72nd inch).
    /// </value>
    public Altaxo.Units.DimensionfulQuantity? MaximumImageHeight { get; set; } // = new Units.DimensionfulQuantity(9.5, new Units.PrefixedUnit(Units.SIPrefix.Centi, Units.Length.Meter.Instance));

    #endregion Properties



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

    public static readonly Main.Properties.PropertyKey<OpenXMLExportOptions> PropertyKeyOpenXMLExportOptions =
    new Main.Properties.PropertyKey<OpenXMLExportOptions>(
    "5BB2EED3-9FCD-4E63-9113-CCB2A2066462",
    "Text\\OpenXMLExportOptions",
    Main.Properties.PropertyLevel.All,
    typeof(TextDocument),
    () => new OpenXMLExportOptions());

    public static void ExportShowFileSaveDialogOnly(TextDocument document)
    {
      var exportOptions = document.GetPropertyValue(PropertyKeyOpenXMLExportOptions, () => new OpenXMLExportOptions());

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
      var exportOptions = document.GetPropertyValue(PropertyKeyOpenXMLExportOptions, () => new OpenXMLExportOptions());
      if (true == Current.Gui.ShowDialog(ref exportOptions, "OpenXml export", false))
      {
        SaveOptionsAndExport(document, exportOptions);
      }
    }

    private static void SaveOptionsAndExport(TextDocument document, OpenXMLExportOptions exportOptions)
    {
      document.PropertyBagNotNull.SetValue(PropertyKeyOpenXMLExportOptions, (OpenXMLExportOptions)exportOptions.Clone());
      Current.PropertyService.ApplicationSettings.SetValue(PropertyKeyOpenXMLExportOptions, (OpenXMLExportOptions)exportOptions.Clone());

      var errors = new List<MarkdownError>();
      exportOptions.Export(document, exportOptions.OutputFileName, errors);

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
    public void Export(TextDocument document, string fileName, List<MarkdownError> errors = null)
    {
      if (null == document)
        throw new ArgumentNullException(nameof(document));
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException(nameof(fileName));
      var basePathName = Path.GetDirectoryName(fileName);


      if (ExpandChildDocuments)
      {
        document = ChildDocumentExpander.ExpandDocumentToNewDocument(document, errors: errors);
      }

      if (RenumerateFigures)
      {
        document.SourceText = FigureRenumerator.RenumerateFigures(document.SourceText);
      }

      // now export the markdown document as Maml file(s)

      // first parse it with Markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);

      var markdownDocument = Markdig.Markdown.Parse(document.SourceText, pipeline.Build());

      var renderer = new OpenXMLRenderer(
        wordDocumentFileName: OutputFileName,
        localImages: document.Images,
        textDocumentFolder: document.Folder
        );


      if (MaximumImageWidth.HasValue)
        renderer.MaxImageWidthIn96thInch = MaximumImageWidth.Value.AsValueIn(Altaxo.Units.Length.Inch.Instance) * 96.0;
      if (MaximumImageHeight.HasValue)
        renderer.MaxImageHeigthIn96thInch = MaximumImageHeight.Value.AsValueIn(Altaxo.Units.Length.Inch.Instance) * 96.0;
      if (null != ThemeName)
        renderer.ThemeName = ThemeName;
      renderer.RemoveOldContentsOfTemplateFile = RemoveOldContentsOfTemplateFile;
      renderer.ImageResolution = ImageResolutionDpi;
      renderer.UseAutomaticFigureNumbering = UseAutomaticFigureNumbering;
      renderer.DoNotFormatFigureLinksAsHyperlinks = DoNotFormatFigureLinksAsHyperlinks;

      renderer.Render(markdownDocument);
    }
  }
}
