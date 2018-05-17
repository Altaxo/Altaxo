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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Altaxo.Graph;
using Altaxo.Gui;
using Markdig;
using Altaxo.Text.Renderers;
using System.Xml;

namespace Altaxo.Text
{
	/// <summary>
	/// Options to export a <see cref="TextDocument"/> into one or multiple Maml file(s), including all the referenced graphs and local images.
	/// </summary>
	public class MamlExportOptions : ICloneable
	{
		/// <summary>
		/// Gets or sets the font family of the body text that later on is rendered out of the Maml file.
		/// We need this here because we have to convert the formulas to images, and need therefore the image size.
		/// </summary>
		protected string _bodyTextFontFamily = "Segoe UI";

		/// <summary>
		/// Gets or sets the font size of the body text that later on is rendered out of the Maml file.
		/// We need this here because we have to convert the formulas to images, and need therefore the image size.
		/// </summary>
		private double _bodyTextFontSize = 15;

		/// <summary>
		/// Set this field to true if the Maml is indended to be used in a Help1 file.
		/// In such a file, the placement of images with align="middle" differs from HTML rendering
		/// (the text baseline is aligned with the middle of the image,
		/// whereas in HTML the middle of the text is aligned with the middle of the image).
		/// </summary>
		public bool IsIntendedForHtmlHelp1File { get; set; } = true;

		public bool EnableHtmlEscape { get; set; } = true;

		/// <summary>
		/// If true, an outline of the content will be included at the top of every Maml file.
		/// </summary>
		public bool EnableAutoOutline { get; set; } = false;

		/// <summary>
		/// The header level where to split the output into different MAML files.
		/// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
		/// </summary>
		protected int _splitLevel = 2;

		/// <summary>
		/// Name of the folder relative to the markdown document, in which the images (graphs and local images) are stored.
		/// </summary>
		protected string _imageFolderName = "Images";

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MamlExportOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (MamlExportOptions)obj;

				info.AddValue("SplitLevel", s.SplitLevel);
				info.AddValue("ImageFolderName", s.ImageFolderName);
				info.AddValue("EnableAutoOutline", s.EnableAutoOutline);
				info.AddValue("EnableHtmlEscape", s.EnableHtmlEscape);
				info.AddValue("EnableLinkToPreviousSection", s.EnableLinkToPreviousSection);
				info.AddValue("EnableLinkToNextSection", s.EnableLinkToNextSection);
				info.AddValue("ExpandChildDocuments", s.ExpandChildDocuments);
				info.AddValue("BodyTextFontFamily", s.BodyTextFontFamily);
				info.AddValue("BodyTextFontSize", s.BodyTextFontSize);
				info.AddValue("IsIntendedForHtmlHelp1File", s.IsIntendedForHtmlHelp1File);
				info.AddValue("OpenHelpFileBuilder", s.OpenHelpFileBuilder);
				info.AddValue("OutputFileName", s.OutputFileName);
			}

			public void Deserialize(MamlExportOptions s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				s.SplitLevel = info.GetInt32("SplitLevel");
				s.ImageFolderName = info.GetString("ImageFolderName");
				s.EnableAutoOutline = info.GetBoolean("EnableAutoOutline");
				s.EnableHtmlEscape = info.GetBoolean("EnableHtmlEscape");
				s.EnableLinkToPreviousSection = info.GetBoolean("EnableLinkToPreviousSection");
				s.EnableLinkToNextSection = info.GetBoolean("EnableLinkToNextSection");
				s.ExpandChildDocuments = info.GetBoolean("ExpandChildDocuments");
				s.BodyTextFontFamily = info.GetString("BodyTextFontFamily");
				s.BodyTextFontSize = info.GetDouble("BodyTextFontSize");
				s.IsIntendedForHtmlHelp1File = info.GetBoolean("IsIntendedForHtmlHelp1File");
				s.OpenHelpFileBuilder = info.GetBoolean("OpenHelpFileBuilder");
				s.OutputFileName = info.GetString("OutputFileName");
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (MamlExportOptions)o ?? new MamlExportOptions();
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
		/// Gets or sets the output file. This is preferably a Sandcastle help file builder project file, but can also be a layout content file (.content) or a Maml file (.aml).
		/// </summary>
		public string OutputFileName { get; set; }

		/// <summary>
		/// Gets or sets the font family of the body text that later on is rendered out of the Maml file.
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
					throw new ArgumentNullException(nameof(value));
				_bodyTextFontFamily = value;
			}
		}

		/// <summary>
		/// Gets or sets the font size of the body text that later on is rendered out of the Maml file.
		/// We need this here because we have to convert the formulas to images, and need therefore the image size.
		/// </summary>
		public double BodyTextFontSize
		{
			get { return _bodyTextFontSize; }
			set
			{
				if (!(value > 0))
					throw new ArgumentOutOfRangeException(nameof(value), "Must be >0");
				_bodyTextFontSize = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the image folder. This folder, for instance 'Image', is relative to the folder where the sandcastle project file is located.
		/// </summary>
		/// <exception cref="ArgumentNullException">value</exception>
		public string ImageFolderName
		{
			get
			{ return _imageFolderName; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException(nameof(value));
				_imageFolderName = value;
			}
		}

		/// <summary>
		/// The header level where to split the output into different MAML files.
		/// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
		/// </summary>
		public int SplitLevel
		{
			get { return _splitLevel; }
			set
			{
				if (!(value >= 0))
					throw new ArgumentOutOfRangeException(nameof(value), "Must be >= 0");
				_splitLevel = value;
			}
		}

		/// <summary>
		/// If true, a link to the previous section is inserted at the beginning of each maml document.
		/// </summary>
		public bool EnableLinkToPreviousSection { get; set; }

		/// <summary>
		/// If true, a link to the next section is inserted at the end of each maml document.
		/// </summary>
		public bool EnableLinkToNextSection { get; set; }

		/// <summary>
		/// If true, included child documents are expanded before the markdown document is processed.
		/// </summary>
		public bool ExpandChildDocuments { get; set; } = true;

		/// <summary>
		/// If true, the Help file builder application is opened after the maml files were exported.
		/// </summary>
		public bool OpenHelpFileBuilder { get; set; }

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
			dlg.AddFilter("*.shfbproj", "Sandcastle help file builder project (*.shfbproj)");
			dlg.AddFilter("*.content", "Content files (*.content)");
			dlg.AddFilter("*.*", "All files (*.*)");
			dlg.AddExtension = true;

			var dialogResult = Current.Gui.ShowSaveFileDialog(dlg);

			return (dialogResult, dlg.FileName);
		}

		public static readonly Main.Properties.PropertyKey<MamlExportOptions> PropertyKeyMamlExportOptions =
		new Main.Properties.PropertyKey<MamlExportOptions>(
		"0E223CE7-2845-48A1-BFB1-7642849F5A3A",
		"Text\\MamlExportOptions",
		Main.Properties.PropertyLevel.All,
		typeof(TextDocument),
		() => new MamlExportOptions());

		/// <summary>
		/// Exports the <see cref="TextDocument"/> to a markdown file, showing first the file save dialog.
		/// </summary>
		/// <param name="document">The document to export.</param>
		public static void ExportShowDialog(TextDocument document)
		{
			var exportOptions = document.GetPropertyValue(PropertyKeyMamlExportOptions, () => new MamlExportOptions());
			if (true == Current.Gui.ShowDialog(ref exportOptions, "Maml export", false))
			{
				document.PropertyBagNotNull.SetValue(PropertyKeyMamlExportOptions, (MamlExportOptions)exportOptions.Clone());
				Current.PropertyService.ApplicationSettings.SetValue(PropertyKeyMamlExportOptions, (MamlExportOptions)exportOptions.Clone());

				exportOptions.Export(document, exportOptions.OutputFileName);

				// Start Sandcastle help file builder
				if (exportOptions.OpenHelpFileBuilder && System.IO.Path.GetExtension(exportOptions.OutputFileName).ToLowerInvariant() == ".shfbproj")
				{
					System.Diagnostics.Process.Start(exportOptions.OutputFileName);
				}
			}
		}

		/// <summary>
		/// Exports the specified <see cref="TextDocument"/> to an external markdown file.
		/// </summary>
		/// <param name="document">The document to export.</param>
		/// <param name="fileName">Full name of the Maml file to export to. Note that if exporting to multiple Maml files,
		/// this is the base file name only; the file names will be derived from this name.</param>
		public void Export(TextDocument document, string fileName)
		{
			if (ExpandChildDocuments)
			{
				document = ChildDocumentExpander.ExpandDocumentToNewDocument(document);
			}

			// First, export the images
			var (oldToNewImageUrl, listOfReferencedImageFileNames) = ExportImages(document, fileName);

			// now export the markdown document as Maml file(s)

			// first parse it with Markdig
			var pipeline = new MarkdownPipelineBuilder();
			pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);

			var markdownDocument = Markdig.Markdown.Parse(document.SourceText, pipeline.Build());

			var renderer = new MamlRenderer(
				projectOrContentFileName: fileName,
				splitLevel: SplitLevel,
				enableHtmlEscape: EnableHtmlEscape,
				autoOutline: EnableAutoOutline,
				enableLinkToPreviousSection: EnableLinkToPreviousSection,
				enableLinkToNextSection: EnableLinkToNextSection,
				imagesFullFileNames: listOfReferencedImageFileNames,
				oldToNewImageUris: oldToNewImageUrl,
				bodyTextFontFamily: BodyTextFontFamily,
				bodyTextFontSize: BodyTextFontSize,
				isIntendedForHelp1File: IsIntendedForHtmlHelp1File
				);

			renderer.Render(markdownDocument);
		}

		private (Dictionary<string, string> oldToNewImageUrl, HashSet<string> listOfReferencedImageFileNames) ExportImages(TextDocument document, string fileName)
		{
			var path = Path.GetDirectoryName(fileName);

			var imagePath = GetImagePath(path);

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
					var (isStreamImage, extension, errorMessage) = imageStreamProvider.GetImageStream(stream, Url, 300, Altaxo.Main.ProjectFolder.GetFolderPart(document.Name), document.Images);

					if (isStreamImage && null == errorMessage)
					{
						var hashName = MemoryStreamImageProxy.ComputeStreamHash(stream);

						var imageFileName = hashName + extension;

						if (!Directory.Exists(imagePath))
							Directory.CreateDirectory(imagePath);

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
