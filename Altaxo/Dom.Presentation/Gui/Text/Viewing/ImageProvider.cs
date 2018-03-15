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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui.Text.Viewing
{
	public class ImageProvider : Markdig.Renderers.WpfImageProviderBase
	{
		private const string absolutePathPretext = "//";
		private const string graphPretext = "graph:";
		private const string resourceImagePretext = "res:";

		/// <summary>
		/// Location from where the images delivered by this image provider are referenced when using relative paths.
		/// </summary>
		public string AltaxoFolderLocation { get; protected set; }

		public ImageProvider(string folder)
		{
			if (!Altaxo.Main.ProjectFolder.IsValidFolderName(folder))
				throw new ArgumentOutOfRangeException(nameof(folder), "Is not a valid folder name");
			AltaxoFolderLocation = folder ?? throw new ArgumentNullException();
		}

		public override Inline GetInlineItem(string url, out bool inlineItemIsErrorMessage)
		{
			if (url.StartsWith(graphPretext))
			{
				string graphName = url.Substring(graphPretext.Length);

				var grp = FindGraphWithUrl(graphName);

				if (grp is Altaxo.Graph.Gdi.GraphDocument graph)
				{
					var options = new Altaxo.Graph.Gdi.GraphExportOptions()
					{
						SourceDpiResolution = 96,
						DestinationDpiResolution = 96,
					};

					using (var stream = new System.IO.MemoryStream())
					{
						Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderToStream(graph, stream, options);
						stream.Seek(0, System.IO.SeekOrigin.Begin);

						// Please note that it it is possible here to use a BitmapFrame only if IsUndoEnabled is set to false
						// in the RichTextBox which hosts the FlowDocument
						// The reason is that when a text block containing the BitmapFrame is deleted, the flow document try
						// to serialize the block (for Undo storage), but this fails for the BitmapFrame
						// If you have to activate IsUndoEnabled, one have to use a BitmapImage instead
						// Caveat: the serialization is really affecting the performance

						/*
						var imageSource = new BitmapImage();
						imageSource.BeginInit();

						// Set properties.
						imageSource.CacheOption = BitmapCacheOption.OnLoad;
						imageSource.CreateOptions = BitmapCreateOptions.None;
						imageSource.StreamSource = stream;
						imageSource.EndInit();
						*/

						var imageSource = BitmapFrame.Create(stream,
													BitmapCreateOptions.None,
													BitmapCacheOption.OnLoad);

						imageSource.Freeze();
						var image = new Image() { Source = imageSource };
						inlineItemIsErrorMessage = false;
						return new InlineUIContainer(image);
					}
				}
				else if (grp is Altaxo.Graph.Graph3D.GraphDocument graph3D)
				{
					var options = new Altaxo.Graph.Gdi.GraphExportOptions()
					{
						SourceDpiResolution = 96,
						DestinationDpiResolution = 96,
					};

					using (var stream = new System.IO.MemoryStream())
					{
						if (!Altaxo.Graph.Graph3D.GraphDocumentExportActions.RenderToStream(graph3D, stream, options))
						{
							inlineItemIsErrorMessage = true;
							return new Run(string.Format("ERROR: NO RENDERER FOR 3D GRAPHS FOUND!"));
						}

						stream.Seek(0, System.IO.SeekOrigin.Begin);
						var imageSource = BitmapFrame.Create(stream,
													BitmapCreateOptions.None,
													BitmapCacheOption.OnLoad);

						imageSource.Freeze();
						var image = new Image() { Source = imageSource };
						inlineItemIsErrorMessage = false;
						return new InlineUIContainer(image);
					}
				}
				else
				{
					inlineItemIsErrorMessage = true;
					return new Run(string.Format("ERROR: GRAPH '{0}' NOT FOUND!", graphName));
				}
			}
			else if (url.StartsWith(resourceImagePretext))
			{
				string name = url.Substring(resourceImagePretext.Length);
				BitmapSource bitmapSource = null;
				try
				{
					bitmapSource = PresentationResourceService.GetBitmapSource(name);
				}
				catch (Exception)
				{
				}

				if (null != bitmapSource)
				{
					var image = new Image() { Source = bitmapSource };

					inlineItemIsErrorMessage = false;
					return new InlineUIContainer(image);
				}
				else
				{
					inlineItemIsErrorMessage = true;
					return new Run(string.Format("ERROR: RESOURCE '{0}' NOT FOUND!", name));
				}
			}
			else
			{
				return base.GetInlineItem(url, out inlineItemIsErrorMessage);
			}
		}

		/// <summary>
		/// Finds the graph with the given Url, trying to find it in GraphCollection and Graph3DCollection, and
		/// using different variants (decoded / not decoded, with slashes or with backslashes).
		/// If the Url starts with two slashes, the path will be considered to be an absolute path in the Altaxo project, else
		/// it will be considered relative to the current location of the markdown document.
		/// </summary>
		/// <param name="url">The original URL.</param>
		/// <returns>Either the found graph (2D or 3D), or null if no graph was found.</returns>
		public Altaxo.Graph.GraphDocumentBase FindGraphWithUrl(string url)
		{
			bool isAbsolutePath = url.StartsWith(absolutePathPretext);
			foreach (var modifiedUrl in ModifiedUrls(url))
			{
				var usedUrl = isAbsolutePath ? modifiedUrl.Substring(absolutePathPretext.Length) : AltaxoFolderLocation + modifiedUrl;

				if (Current.Project.GraphDocumentCollection.Contains(usedUrl))
					return Current.Project.GraphDocumentCollection[usedUrl];
				else if (Current.Project.Graph3DDocumentCollection.Contains(usedUrl))
					return Current.Project.Graph3DDocumentCollection[usedUrl];
			}

			return null;
		}

		private IEnumerable<string> ModifiedUrls(string originalUrl)
		{
			yield return originalUrl;
			string backslashed = originalUrl.Replace('/', '\\');
			yield return backslashed;
			string decoded = DecodeUrlString(originalUrl);
			yield return decoded;
			yield return decoded.Replace('/', '\\');
		}

		private static string DecodeUrlString(string url)
		{
			string newUrl;
			while ((newUrl = Uri.UnescapeDataString(url)) != url)
				url = newUrl;
			return newUrl;
		}
	}
}
