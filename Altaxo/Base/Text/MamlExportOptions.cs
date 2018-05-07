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

namespace Altaxo.Text
{
	/// <summary>
	/// Options to export a <see cref="TextDocument"/> into one or multiple Maml file(s), including all the referenced graphs and local images.
	/// </summary>
	public class MamlExportOptions
	{
		/// <summary>
		/// Name of the folder relative to the markdown document, in which the images (graphs and local images) are stored.
		/// </summary>
		public string ImageDirectoryName { get; } = "Images";

		/// <summary>
		/// Given the folder where the markdown file resides, this gets the full folder name of the image folder.
		/// </summary>
		/// <param name="markdownPathName">Name of the folder of the markdown file.</param>
		/// <returns>Full folder name of the image folder</returns>
		private string GetImagePath(string markdownPathName)
		{
			return Path.Combine(markdownPathName, "Images");
		}

		/// <summary>
		/// Exports the <see cref="TextDocument"/> to a markdown file, showing first the file save dialog.
		/// </summary>
		/// <param name="document">The document to export.</param>
		public static void ExportShowDialog(TextDocument document)
		{
			var options = new MamlExportOptions();

			var dlg = new SaveFileOptions();
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
			var path = Path.GetDirectoryName(fileName);

			var imagePath = GetImagePath(path);

			var sourceDoc = new System.Text.StringBuilder(document.SourceText);

			var list = new List<(string Url, int urlSpanStart, int urlSpanEnd)>(document.ReferencedImageUrls);

			list.Sort((x, y) => Comparer<int>.Default.Compare(y.urlSpanEnd, x.urlSpanEnd)); // Note the inverse order of x and y to sort urlSpanEnd descending

			var imageStreamProvider = new ImageStreamProvider();

			var oldToNewImageUrl = new Dictionary<string, string>();

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
						using (var fileStream = new System.IO.FileStream(Path.Combine(imagePath, imageFileName), FileMode.Create, FileAccess.Write, FileShare.Read))
						{
							stream.Seek(0, SeekOrigin.Begin);
							stream.CopyTo(fileStream);
						}

						// now change the url in the markdown text
						var newUrl = ImageDirectoryName + "/" + imageFileName;

						sourceDoc.Remove(urlSpanStart, 1 + urlSpanEnd - urlSpanStart);
						sourceDoc.Insert(urlSpanStart, newUrl);

						oldToNewImageUrl[Url] = newUrl;
					}
				}
			}

			// now export the markdown document as Maml file(s)

			// first parse it with Markdig
			var pipeline = new MarkdownPipelineBuilder();
			pipeline = UseSupportedExtensions(pipeline);

			var markdownDocument = Markdig.Markdown.Parse(document.SourceText, pipeline.Build());

			var renderer = new MamlRenderer()
			{
				FullPathBaseFileName = Path.Combine(path, Path.GetFileNameWithoutExtension(fileName)),
				SplitLevel = 2,
				EnableHtmlEscape = true,
				AutoOutline = true,
				OldToNewImageUris = oldToNewImageUrl,
			};

			renderer.Render(markdownDocument);
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
