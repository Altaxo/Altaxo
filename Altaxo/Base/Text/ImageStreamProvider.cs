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

namespace Altaxo.Text
{
  /// <summary>
  /// Provided stream content of images referenced in a <see cref="TextDocument"/>.
  /// </summary>
  public class ImageStreamProvider
  {
    /// <summary>
    /// Gets the content of an image referenced in a <see cref="TextDocument"/>.
    /// </summary>
    /// <param name="stream">The stream to copy the content to.</param>
    /// <param name="url">The URL of the image, as occuring in a link tag in the <see cref="TextDocument"/>.</param>
    /// <param name="targetResolution">The target resolution in dpi.</param>
    /// <param name="altaxoFolderLocation">The folder location of the <see cref="TextDocument"/> It is used for searching graphs relative to that location.</param>
    /// <param name="localImages">The local images of the <see cref="TextDocument"/>.</param>
    /// <returns>A tuple of tree values. isStreamImage is true if the url could be resolved to an image. extension is the file extension of the image. errorMessage is unequal to null if an error has occured. In this case the content of <paramref name="stream"/> was not set.</returns>
    public (bool isStreamImage, string extension, string errorMessage) GetImageStream(System.IO.Stream stream, string url, double targetResolution, string altaxoFolderLocation, IReadOnlyDictionary<string, Altaxo.Graph.MemoryStreamImageProxy> localImages)
    {
      if (url.StartsWith(ImagePretext.GraphRelativePathPretext))
      {
        string graphName = url.Substring(ImagePretext.GraphRelativePathPretext.Length);

        var grp = FindGraphWithUrl(graphName, altaxoFolderLocation);

        if (grp is Altaxo.Graph.Gdi.GraphDocument graph)
        {
          var options = new Altaxo.Graph.Gdi.GraphExportOptions()
          {
            SourceDpiResolution = targetResolution,
            DestinationDpiResolution = targetResolution,
          };

          Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderToStream(graph, stream, options);
          return (true, ".png", null);
        }
        else if (grp is Altaxo.Graph.Graph3D.GraphDocument graph3D)
        {
          var options = new Altaxo.Graph.Gdi.GraphExportOptions()
          {
            SourceDpiResolution = targetResolution,
            DestinationDpiResolution = targetResolution,
          };

          if (Altaxo.Graph.Graph3D.GraphDocumentExportActions.RenderToStream(graph3D, stream, options))
            return (true, ".png", null);
          else
            return (true, null, string.Format("ERROR: NO RENDERER FOR 3D GRAPHS FOUND!"));
        }
        else
        {
          return (true, null, string.Format("ERROR: GRAPH '{0}' NOT FOUND!", graphName));
        }
      }
      else if (url.StartsWith(ImagePretext.ResourceImagePretext))
      {
        string name = url.Substring(ImagePretext.ResourceImagePretext.Length);
        var inStream = Current.ResourceService.GetResourceStream(name);
        if (null != inStream)
        {
          inStream.CopyTo(stream);
          return (true, ".png", null);
        }
        else
        {
          return (true, null, string.Format("Resource image '{0}' not found!", name));
        }
      }
      else if (url.StartsWith(ImagePretext.LocalImagePretext))
      {
        string name = url.Substring(ImagePretext.LocalImagePretext.Length);

        if (null != localImages && localImages.TryGetValue(name, out var localImageStreamProxy))
        {
          var inStream = localImageStreamProxy.GetContentStream();
          var extension = localImageStreamProxy.Extension;
          inStream.CopyTo(stream);
          return (true, extension, null);
        }
        else
        {
          return (true, null, string.Format("ERROR: LOCAL IMAGE '{0}' NOT FOUND!", name));
        }
      }
      else
      {
        if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          return (false, null, string.Format("Url empty or malformed: {0}", url));
        }
        else
        {
          return (false, null, null);
        }
      }
    }

    /// <summary>
    /// Finds the graph with the given Url, trying to find it in GraphCollection and Graph3DCollection, and
    /// using different variants (decoded / not decoded, with slashes or with backslashes).
    /// If the Url starts with two slashes, the path will be considered to be an absolute path in the Altaxo project, else
    /// it will be considered relative to the current location of the markdown document.
    /// </summary>
    /// <param name="url">The original URL.</param>
    /// <param name="AltaxoFolderLocation">The Altaxo folder where the document which includes this Graph in located in.</param>
    /// <returns>Either the found graph (2D or 3D), or null if no graph was found.</returns>
    public static Altaxo.Graph.GraphDocumentBase FindGraphWithUrl(string url, string AltaxoFolderLocation)
    {
      bool isAbsolutePath = url.StartsWith(ImagePretext.AbsolutePathPretext);
      foreach (var modifiedUrl in ModifiedUrls(url))
      {
        var usedUrl = isAbsolutePath ? modifiedUrl.Substring(ImagePretext.AbsolutePathPretext.Length) : AltaxoFolderLocation + modifiedUrl;

        if (Current.Project.GraphDocumentCollection.Contains(usedUrl))
          return Current.Project.GraphDocumentCollection[usedUrl];
        else if (Current.Project.Graph3DDocumentCollection.Contains(usedUrl))
          return Current.Project.Graph3DDocumentCollection[usedUrl];
      }

      return null;
    }

    private static IEnumerable<string> ModifiedUrls(string originalUrl)
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
