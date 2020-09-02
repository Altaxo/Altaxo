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

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Gui;
using Altaxo.Text.Renderers.Html;

namespace Altaxo.Text
{
  /// <summary>
  /// Options to export a <see cref="TextDocument"/> into one or multiple Html file(s), including all the referenced graphs and local images.
  /// </summary>
  public class HtmlExportOptions : ICloneable
  {
    /// <summary>
    /// Gets or sets the font family of the body text that later on is rendered out of the Html file.
    /// We need this here because we have to convert the formulas to images, and need therefore the image size.
    /// </summary>
    protected string _bodyTextFontFamily = "Segoe UI";

    /// <summary>
    /// Gets or sets the font size of the body text that later on is rendered out of the Html file.
    /// We need this here because we have to convert the formulas to images, and need therefore the image size.
    /// </summary>
    private double _bodyTextFontSize = 15;

    public bool EnableHtmlEscape { get; set; } = true;

    /// <summary>
    /// The header level where to split the output into different Html files.
    /// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
    /// </summary>
    protected int _splitLevel = 2;

    /// <summary>
    /// Name of the folder relative to the markdown document, in which the images (graphs and local images) are stored.
    /// </summary>
    protected string _imageFolderName = "Images";

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HtmlExportOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HtmlExportOptions)obj;

        info.AddValue("SplitLevel", s.SplitLevel);
        info.AddValue("ImageFolderName", s.ImageFolderName);
        info.AddValue("EnableRemoveOldContentsOfImageFolder", s.EnableRemoveOldContentsOfImageFolder);
        info.AddValue("EnableHtmlEscape", s.EnableHtmlEscape);
        info.AddValue("EnableLinkToPreviousSection", s.EnableLinkToPreviousSection);
        info.AddValue("LinkToPreviousSectionLabelText", s.LinkToPreviousSectionLabelText);
        info.AddValue("EnableLinkToNextSection", s.EnableLinkToNextSection);
        info.AddValue("LinkToNextSectionLabelText", s.LinkToNextSectionLabelText);
        info.AddValue("EnableLinkToTableOfContents", s.EnableLinkToTableOfContents);
        info.AddValue("LinkToTableOfContentsLabelText", s.LinkToTableOfContentsLabelText);
        info.AddValue("ExpandChildDocuments", s.ExpandChildDocuments);
        info.AddValue("BodyTextFontFamily", s.BodyTextFontFamily);
        info.AddValue("BodyTextFontSize", s.BodyTextFontSize);
        info.AddValue("OutputFileName", s.OutputFileName);
        info.AddValue("EnableRemoveOldContentsOfOutputFolder", s.EnableRemoveOldContentsOfOutputFolder);
        info.AddValue("OpenHtmlViewer", s.OpenHtmlViewer);
        info.AddValue("RenumerateFigures", s.RenumerateFigures);
      }

      public void Deserialize(HtmlExportOptions s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        s.SplitLevel = info.GetInt32("SplitLevel");
        s.ImageFolderName = info.GetString("ImageFolderName");
        s.EnableRemoveOldContentsOfImageFolder = info.GetBoolean("EnableRemoveOldContentsOfImageFolder");
        s.EnableHtmlEscape = info.GetBoolean("EnableHtmlEscape");
        s.EnableLinkToPreviousSection = info.GetBoolean("EnableLinkToPreviousSection");
        s.LinkToPreviousSectionLabelText = info.GetString("LinkToPreviousSectionLabelText");
        s.EnableLinkToNextSection = info.GetBoolean("EnableLinkToNextSection");
        s.LinkToNextSectionLabelText = info.GetString("LinkToNextSectionLabelText");
        s.EnableLinkToTableOfContents = info.GetBoolean("EnableLinkToTableOfContents");
        s.LinkToTableOfContentsLabelText = info.GetString("LinkToTableOfContentsLabelText");
        s.ExpandChildDocuments = info.GetBoolean("ExpandChildDocuments");
        s.BodyTextFontFamily = info.GetString("BodyTextFontFamily");
        s.BodyTextFontSize = info.GetDouble("BodyTextFontSize");
        s.OutputFileName = info.GetString("OutputFileName");
        s.EnableRemoveOldContentsOfOutputFolder = info.GetBoolean("EnableRemoveOldContentsOfOutputFolder");
        s.OpenHtmlViewer = info.GetBoolean("OpenHtmlViewer");
        if (info.CurrentElementName == "RenumerateFigures")
          s.RenumerateFigures = info.GetBoolean("RenumerateFigures");
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HtmlExportOptions?)o ?? new HtmlExportOptions();
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

    /// <summary>
    /// Gets or sets the output file. This is the base name, which is amended with numbers.
    /// </summary>
    public string OutputFileName { get; set; } = string.Empty;

    /// <summary>
    /// If set to true, all .html files residing in the output directory will be removed before exporting.
    /// </summary>
    public bool EnableRemoveOldContentsOfOutputFolder { get; set; }

    /// <summary>
    /// Gets or sets the font family of the body text that later on is rendered out of the Html file.
    /// We need this here because we have to convert the formulas to images, and need therefore the image size.
    /// </summary>
    public string BodyTextFontFamily
    {
      get
      {
        return _bodyTextFontFamily;
      }
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          throw new ArgumentNullException(nameof(value));
        }

        _bodyTextFontFamily = value;
      }
    }

    /// <summary>
    /// Gets or sets the font size of the body text that later on is rendered out of the Html file.
    /// We need this here because we have to convert the formulas to images, and need therefore the image size.
    /// </summary>
    public double BodyTextFontSize
    {
      get { return _bodyTextFontSize; }
      set
      {
        if (!(value > 0))
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Must be >0");
        }

        _bodyTextFontSize = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the image folder. This folder, for instance 'Image', is relative to the folder where the output file(s) are located.
    /// </summary>
    /// <exception cref="ArgumentNullException">value</exception>
    public string ImageFolderName
    {
      get
      { return _imageFolderName; }
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          throw new ArgumentNullException(nameof(value));
        }

        _imageFolderName = value;
      }
    }

    /// <summary>
    /// If set to true, all files residing in the image folder are deleted before exporting the new image files.
    /// </summary>
    public bool EnableRemoveOldContentsOfImageFolder { get; set; }

    /// <summary>
    /// The header level where to split the output into different Html files.
    /// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
    /// </summary>
    public int SplitLevel
    {
      get { return _splitLevel; }
      set
      {
        if (!(value >= 0))
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Must be >= 0");
        }

        _splitLevel = value;
      }
    }

    /// <summary>
    /// If true, a link to the previous section is inserted at the beginning of each Html document.
    /// </summary>
    public bool EnableLinkToPreviousSection { get; set; }

    /// <summary>
    /// Gets or sets the text that is inserted immediately before the link to the next section.
    /// </summary>
    public string LinkToPreviousSectionLabelText { get; set; } = "Previous section: ";

    /// <summary>
    /// If true, a link to the next section is inserted at the end of each Html document.
    /// </summary>
    public bool EnableLinkToNextSection { get; set; }

    /// <summary>
    /// Gets or sets the text that is inserted immediately before the link to the next section.
    /// </summary>
    public string LinkToNextSectionLabelText { get; set; } = "Next section: ";

    /// <summary>
    /// If true, a link to the next section is inserted at the end of each Html document.
    /// </summary>
    public bool EnableLinkToTableOfContents { get; set; } = true;

    /// <summary>
    /// Gets or sets the text that is inserted immediately before the link to the next section.
    /// </summary>
    public string LinkToTableOfContentsLabelText { get; set; } = "Table of contents";

    /// <summary>
    /// If true, included child documents are expanded before the markdown document is processed.
    /// </summary>
    public bool ExpandChildDocuments { get; set; } = true;

    /// <summary>
    /// If true, figures will be renumerated and the links to those figures updated.
    /// </summary>
    public bool RenumerateFigures { get; set; }

    /// <summary>
    /// If true, the default Htlm viewer application is opened after the Html files were exported.
    /// </summary>
    public bool OpenHtmlViewer { get; set; }

    #endregion Properties

    /// <summary>
    /// Given the folder where the markdown file resides, this gets the full folder name of the image folder.
    /// </summary>
    /// <param name="markdownPathName">Name of the folder of the markdown file.</param>
    /// <returns>Full folder name of the image folder</returns>
    private string GetImagePath(string markdownPathName)
    {
      return Path.Combine(markdownPathName, ImageFolderName);
    }

    /// <summary>
    /// Gets the output file by showing a save file dialog.
    /// </summary>
    /// <returns></returns>
    public static (bool dialogResult, string outputFileName) ShowGetOutputFileDialog()
    {
      var dlg = new SaveFileOptions();
      dlg.AddFilter("*.html", "Html files (*.html)");
      dlg.AddFilter("*.*", "All files (*.*)");
      dlg.AddExtension = true;

      var dialogResult = Current.Gui.ShowSaveFileDialog(dlg);

      return (dialogResult, dlg.FileName);
    }

    public static readonly Main.Properties.PropertyKey<HtmlExportOptions> PropertyKeyHtmlExportOptions =
    new Main.Properties.PropertyKey<HtmlExportOptions>(
    "1A21999E-6F58-4B59-B635-2F839AABF080",
    "Text\\HtmlExportOptions",
    Main.Properties.PropertyLevel.All,
    typeof(TextDocument),
    () => new HtmlExportOptions());

    /// <summary>
    /// Exports the <see cref="TextDocument"/> to a markdown file, showing first the file save dialog.
    /// </summary>
    /// <param name="document">The document to export.</param>
    public static void ExportShowDialog(TextDocument document)
    {
      var exportOptions = document.GetPropertyValue(PropertyKeyHtmlExportOptions, () => new HtmlExportOptions());
      if (true == Current.Gui.ShowDialog(ref exportOptions, "Html export", false))
      {
        var errors = new List<MarkdownError>();


        document.PropertyBagNotNull.SetValue(PropertyKeyHtmlExportOptions, (HtmlExportOptions)exportOptions.Clone());
        Current.PropertyService.ApplicationSettings.SetValue(PropertyKeyHtmlExportOptions, (HtmlExportOptions)exportOptions.Clone());
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

        // Start Html viewer
        if (exportOptions.OpenHtmlViewer && System.IO.Path.GetExtension(exportOptions.OutputFileName).ToLowerInvariant() == ".html")
        {
          System.Diagnostics.Process.Start(exportOptions.OutputFileName);
        }
      }
    }

    /// <summary>
    /// Exports the specified <see cref="TextDocument"/> to an external markdown file.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="fileName">Full name of the Html file to export to. Note that if exporting to multiple Html files,
    /// this is the base file name only; the file names will be derived from this name.</param>
    /// <param name="errors">A list that collects error messages.</param>
    public void Export(TextDocument document, string fileName, List<MarkdownError> errors)
    {
      if (document is null)
      {
        throw new ArgumentNullException(nameof(document));
      }

      if (string.IsNullOrEmpty(fileName))
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      var basePathName = Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException($"Unable to get directory of file name {fileName}");

      if (ExpandChildDocuments)
      {
        document = ChildDocumentExpander.ExpandDocumentToNewDocument(document, errors: errors);
      }

      if (RenumerateFigures)
      {
        document.SourceText = FigureRenumerator.RenumerateFigures(document.SourceText);
      }

      // remove the old content
      if (EnableRemoveOldContentsOfOutputFolder)
      {
        var fullContentFolderName = basePathName;
        HtmlSplitRenderer.RemoveOldContentsOfContentFolder(fullContentFolderName);
      }

      // remove old images
      if (EnableRemoveOldContentsOfImageFolder)
      {
        var fullImageFolderName = Path.Combine(basePathName, ImageFolderName);
        HtmlSplitRenderer.RemoveOldContentsOfImageFolder(fullImageFolderName);
      }

      // First, export the images
      var (oldToNewImageUrl, listOfReferencedImageFileNames) = ExportImages(document, basePathName);

      // now export the markdown document as Html file(s)



      var renderer = new HtmlSplitRenderer(
        projectOrContentFileName: fileName,
        imageFolderName: ImageFolderName,
        splitLevel: SplitLevel,
        enableHtmlEscape: EnableHtmlEscape,
        autoOutline: false,
        enableLinkToPreviousSection: EnableLinkToPreviousSection,
        linkToPreviousSectionLabelText: LinkToPreviousSectionLabelText,
        enableLinkToNextSection: EnableLinkToNextSection,
        linkToNextSectionLabelText: LinkToNextSectionLabelText,
        enableLinkToTableOfContents: EnableLinkToTableOfContents,
        linkToTableOfContentsLabelText: LinkToTableOfContentsLabelText,
        imagesFullFileNames: listOfReferencedImageFileNames,
        oldToNewImageUris: oldToNewImageUrl,
        bodyTextFontFamily: BodyTextFontFamily,
        bodyTextFontSize: BodyTextFontSize
        );

      renderer.Render(document.SourceText);
    }

    /// <summary>
    /// Exports the images.
    /// </summary>
    /// <param name="document">The text document from which to export the images.</param>
    /// <param name="basePathName">Full name of the base folder. The base folder is the folder of which the <see cref="ImageFolderName"/> are subfolders.</param>
    /// <returns>A tuple consisting of a dictionary which translates old image Urls to new image Urls, and a set of full image file names that were exported.</returns>
    private (Dictionary<string, string> oldToNewImageUrl, HashSet<string> listOfReferencedImageFileNames)
          ExportImages(TextDocument document, string basePathName)
    {
      var imagePath = GetImagePath(basePathName);

      var list = new List<(string Url, int urlSpanStart, int urlSpanEnd)>(document.ReferencedImageUrls);

      list.Sort((x, y) => Comparer<int>.Default.Compare(y.urlSpanEnd, x.urlSpanEnd)); // Note the inverse order of x and y to sort urlSpanEnd descending

      var imageStreamProvider = new ImageStreamProvider();

      var oldToNewImageUrl = new Dictionary<string, string>();
      var listOfReferencedImageFileNames = new HashSet<string>();

      // Export images
      foreach (var (Url, urlSpanStart, urlSpanEnd) in list)
      {
        using (var stream = new System.IO.MemoryStream())
        {
          var streamResult = imageStreamProvider.GetImageStream(stream, Url, 300, Altaxo.Main.ProjectFolder.GetFolderPart(document.Name), document.Images);

          if (streamResult.IsValid)
          {
            var hashName = MemoryStreamImageProxy.ComputeStreamHash(stream);

            var imageFileName = hashName + streamResult.Extension;

            if (!Directory.Exists(imagePath))
            {
              Directory.CreateDirectory(imagePath);
            }

            // Copy stream to FileSystem
            var fullImageFileName = Path.Combine(imagePath, imageFileName);
            using (var fileStream = new System.IO.FileStream(fullImageFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
              stream.Seek(0, SeekOrigin.Begin);
              stream.CopyTo(fileStream);
            }

            // now change the url in the markdown text
            var newUrl = ImageFolderName + "/" + imageFileName;

            oldToNewImageUrl[Url] = newUrl;
            listOfReferencedImageFileNames.Add(fullImageFileName);
          }
        }
      }

      return (oldToNewImageUrl, listOfReferencedImageFileNames);
    }
  }
}
