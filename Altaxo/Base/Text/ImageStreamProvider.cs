﻿#region Copyright

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Text
{
  public class ImageRenderToStreamResult
  {
    public bool IsValid { get; private set; }
    public string NameHint { get; private set; } = string.Empty;
    public string Extension { get; private set; } = string.Empty;

    public string? ErrorMessage { get; private set; }

    public double DpiX { get; private set; }
    public double DpiY { get; private set; }

    public double PixelsX { get; private set; }
    public double PixelsY { get; private set; }

    public ImageRenderToStreamResult(
      string nameHint,
      string extension,
      double dpiX,
      double dpiY,
      double pixelsX,
      double pixelsY)
    {
      IsValid = true;
      ErrorMessage = null;
      NameHint = nameHint;
      Extension = extension;
      DpiX = dpiX;
      DpiY = dpiY;
      PixelsX = pixelsX;
      PixelsY = pixelsY;
    }

    public ImageRenderToStreamResult(string? errorMessage)
    {
      IsValid = false;
      ErrorMessage = errorMessage;
    }
  }


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
    public ImageRenderToStreamResult GetImageStream(System.IO.Stream stream, string url, double targetResolution, string altaxoFolderLocation, IReadOnlyDictionary<string, MemoryStreamImageProxy> localImages)
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

          var (pixelsX, pixelsY) = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderToStream(graph, stream, options);
          return new ImageRenderToStreamResult(graph.Name, ".png",
            options.DestinationDpiResolution, options.DestinationDpiResolution,
            pixelsX, pixelsY);
        }
        else if (grp is Altaxo.Graph.Graph3D.GraphDocument graph3D)
        {
          var options = new Altaxo.Graph.Gdi.GraphExportOptions()
          {
            SourceDpiResolution = targetResolution,
            DestinationDpiResolution = targetResolution,
          };

          var (pixelsX, pixelsY) = Altaxo.Graph.Graph3D.GraphDocumentExportActions.RenderToStream(graph3D, stream, options);
          if (pixelsX > 0 && pixelsY > 0)
            return new ImageRenderToStreamResult(graph3D.Name, ".png",
              options.DestinationDpiResolution, options.DestinationDpiResolution,
              pixelsX, pixelsY);
          else
            return new ImageRenderToStreamResult("ERROR: NO RENDERER FOR 3D GRAPHS FOUND!");
        }
        else
        {
          return new ImageRenderToStreamResult(string.Format("ERROR: GRAPH '{0}' NOT FOUND!", graphName));
        }
      }
      else if (url.StartsWith(ImagePretext.ResourceImagePretext))
      {
        string name = url.Substring(ImagePretext.ResourceImagePretext.Length);
        var inStream = Current.ResourceService.GetResourceStream(name);

        if (inStream is not null)
        {
          string extension;
          int pixelsX, pixelsY;
          double dpiX, dpiY;

          using (var image = System.Drawing.Image.FromStream(inStream))
          {

            pixelsX = image.Width;
            pixelsY = image.Height;
            dpiX = image.HorizontalResolution;
            dpiY = image.VerticalResolution;
            extension = Altaxo.Drawing.ImageExtensions.GetFileExtension(image);
          }
          inStream.Seek(0, SeekOrigin.Begin);

          inStream.CopyTo(stream);
          return new ImageRenderToStreamResult(url, extension, dpiX, dpiY, pixelsX, pixelsY);
        }
        else // If it doesn't work with a stream, we try to get a bitmap
        {
          System.Drawing.Bitmap? bitmap = null;
          try { bitmap = Current.ResourceService.GetBitmap(name); } catch { }

          if (bitmap is not null)
          {
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return new ImageRenderToStreamResult(url, ".png", bitmap.HorizontalResolution, bitmap.VerticalResolution, bitmap.Width, bitmap.Height);
          }
          else
          {
            return new ImageRenderToStreamResult(string.Format("Resource image '{0}' not found!", name));
          }
        }
      }
      else if (url.StartsWith(ImagePretext.LocalImagePretext))
      {
        string name = url.Substring(ImagePretext.LocalImagePretext.Length);

        if (localImages is not null && localImages.TryGetValue(name, out var localImageStreamProxy))
        {
          var inStream = localImageStreamProxy.GetContentStream();
          var extension = localImageStreamProxy.Extension;
          inStream.CopyTo(stream);
          return new ImageRenderToStreamResult(localImageStreamProxy.Name, localImageStreamProxy.Extension,
            localImageStreamProxy.ResolutionDpi.X, localImageStreamProxy.ResolutionDpi.Y,
            localImageStreamProxy.ImageSizePixels.X, localImageStreamProxy.ImageSizePixels.Y);
        }
        else
        {
          return new ImageRenderToStreamResult(string.Format("ERROR: LOCAL IMAGE '{0}' NOT FOUND!", name));
        }
      }
      else
      {
        if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          return new ImageRenderToStreamResult(string.Format("Url empty or malformed: {0}", url));
        }
        else
        {
          return new ImageRenderToStreamResult(string.Format("Does not understand this kind of Url: {0}", url));
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
    public static Altaxo.Graph.GraphDocumentBase? FindGraphWithUrl(string url, string AltaxoFolderLocation)
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
