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
using Altaxo.Geometry;

#nullable enable

namespace Altaxo.Drawing
{

  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public abstract class ImageProxy : Main.IImmutable
  {
    protected const double InchesPerPoint = 1 / 72.0;
    protected const double PointsPerInch = 72.0;

    /// <summary>
    /// Gets information about physical size (Attention: in points, i.e. 1/72 inch, <b>not</b> in 1/96 inch!), about pixel size and resolution.
    /// </summary>
    /// <param name="str">The content stream that holds the image.</param>
    /// <returns>The size in points (1/72 inch), the size in pixels, as well as the horizontal and vertical resolution of the image).</returns>
    public static (VectorD2D SizePoint, VectorD2D SizePixel, VectorD2D Resolution) GetImageInformation(Stream str)
    {
      if (str is null)
        throw new ArgumentNullException(nameof(str));

      // Note that the function here uses the System.Drawing.Imaging library.
      // This can be replaced as needed by any other library.

      System.Drawing.Image? img = null;
      try
      {
        str.Seek(0, SeekOrigin.Begin);
        img = System.Drawing.Image.FromStream(str);

        var sizePx = new VectorD2D(img.Width, img.Height);
        var sizePt = new VectorD2D(72.0 * img.Width / img.HorizontalResolution, 72.0 * img.Height / img.VerticalResolution);
        var resolution = new VectorD2D(img.HorizontalResolution, img.VerticalResolution);
        return (sizePt, sizePx, resolution);
      }
      finally
      {
        img?.Dispose();
      }
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// Here, the return value is the name of the instance.
    /// </returns>
    public override string ToString()
    {
      return Name;
    }

    /// <summary>
    /// Creates an image proxy from a file location.
    /// </summary>
    /// <param name="fullpath">The full file name of the image file.</param>
    /// <returns>An image proxy representing the content of the file.</returns>
    public static ImageProxy FromFile(string fullpath)
    {
      return MemoryStreamImageProxy.FromFile(fullpath);
    }

    /// <summary>
    /// Creates an image proxy from a resource.
    /// </summary>
    /// <param name="fullpath">The resource key.</param>
    /// <returns>An image proxy representing the content of the resource.</returns>
    public static ImageProxy FromResource(string fullpath)
    {
      return ResourceImageProxy.FromResource(fullpath);
    }


    /// <summary>
    /// Creates an image proxy from the content of a stream.
    /// </summary>
    /// <param name="istr">The stream. The stream is only used in creation, because a copy of the content of the stream is made.</param>
    /// <param name="name">A descriptive name for the stream.</param>
    /// <returns>An image proxy representing the content of the stream.</returns>
    public static ImageProxy FromStream(Stream istr, string name)
    {
      return MemoryStreamImageProxy.FromStream(istr, name);
    }

    /// <summary>
    /// Gets a string that is unique for the content of the proxy.
    /// </summary>
    /// <value>
    /// The content hash.
    /// </value>
    public abstract string ContentHash { get; }

    /// <summary>
    /// Gets the content of the image proxy as a (library independent) stream. Any library that can read image streams can be used to convert the stream in the corresponding image.
    /// </summary>
    /// <returns>Content of the image proxy as a (library independent) stream</returns>
    public abstract Stream GetContentStream();

    /// <summary>
    /// Returns true if this image proxy holds valid content.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance holds valid content; otherwise, <c>false</c>.
    /// </value>
    public abstract bool IsValid { get; }

    /// <summary>
    /// Gets the name of the instance. Can be a file name, resource name, Url etc.
    /// </summary>
    /// <value>
    /// The name of this instance.
    /// </value>
    public abstract string Name { get; }

    /// <summary>
    /// Determines whether the provided image proxy has the same content as this one.
    /// </summary>
    /// <param name="from">The other image proxy to compare with.</param>
    /// <returns>
    ///   <c>true</c> if the other image proxy has the same content (i.e. the same image) as this instance; otherwise, <c>false</c>.
    /// </returns>
    public bool HasSameContentAs(ImageProxy from)
    {
      return ContentHash == from.ContentHash;
    }

    /// <summary>Returns the original size of the image in points (1/72 inch).</summary>
    public abstract VectorD2D Size { get; }

  }
}
