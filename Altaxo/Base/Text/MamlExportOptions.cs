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
	public class MamlExportOptions
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
		public bool IsIntendedForHtml1HelpFile { get; set; } = true;

		public bool EnableHtmlEscape { get; set; } = true;

		/// <summary>
		/// If true, an outline of the content will be included at the top of every Maml file.
		/// </summary>
		public bool AutoOutLine { get; set; } = false;

		/// <summary>
		/// The header level where to split the output into different MAML files.
		/// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
		/// </summary>
		protected int _splitLevel = 2;

		/// <summary>
		/// Name of the folder relative to the markdown document, in which the images (graphs and local images) are stored.
		/// </summary>
		protected string _imageFolderName = "Images";

		#region Properties

		/// <summary>
		/// Gets or sets the font family of the body text that later on is rendered out of the Maml file.
		/// We need this here because we have to convert the formulas to images, and need therefore the image size.
		/// </summary>
		public string BodyTextFontFamily
		{
			get
			{ return _bodyTextFontFamily; }
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
		/// Exports the <see cref="TextDocument"/> to a markdown file, showing first the file save dialog.
		/// </summary>
		/// <param name="document">The document to export.</param>
		public static void ExportShowDialog(TextDocument document)
		{
			var options = new MamlExportOptions();

			var dlg = new SaveFileOptions();
			dlg.AddFilter("*.shfbproj", "Sandcastle help file builder project (*.shfbproj)");
			dlg.AddFilter("*.content", "Content files (*.content)");
			dlg.AddFilter("*.*", "All files (*.*)");

			dlg.AddExtension = true;

			if (true == Current.Gui.ShowSaveFileDialog(dlg))
			{
				options.Export(document, dlg.FileName);
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
			// First, export the images
			var (oldToNewImageUrl, listOfReferencedImageFileNames) = ExportImages(document, fileName);

			// now export the markdown document as Maml file(s)

			// first parse it with Markdig
			var pipeline = new MarkdownPipelineBuilder();
			pipeline = UseSupportedExtensions(pipeline);

			var markdownDocument = Markdig.Markdown.Parse(document.SourceText, pipeline.Build());

			var renderer = new MamlRenderer(
				projectOrContentFileName: fileName,
				splitLevel: SplitLevel,
				enableHtmlEscape: EnableHtmlEscape,
				autoOutline: AutoOutLine,
				imagesFullFileNames: listOfReferencedImageFileNames,
				oldToNewImageUris: oldToNewImageUrl,
				bodyTextFontFamily: BodyTextFontFamily,
				bodyTextFontSize: BodyTextFontSize,
				isIntendedForHelp1File: IsIntendedForHtml1HelpFile
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

		/// <summary>
		/// Uses all extensions supported by <c>Markdig.Wpf</c>.
		/// </summary>
		/// <param name="pipeline">The pipeline.</param>
		/// <returns>The modified pipeline</returns>
		public static MarkdownPipelineBuilder UseSupportedExtensions(MarkdownPipelineBuilder pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
			return pipeline
					.UseEmphasisExtras()
					.UseGridTables()
					.UsePipeTables()
					.UseTaskLists()
					.UseAutoLinks()
					.UseMathematics()
					.UseGenericAttributes();
		}
	}
}
