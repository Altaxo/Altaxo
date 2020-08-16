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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Extensions for <see cref="System.Drawing.Image"/>.
  /// </summary>
  public static class ImageExtensions
  {
    /// <summary>
    /// Gets the file extension (lower case letters with a preceding dot, like .jpg) of the given image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <returns>The file extension (lower case letters with a preceding dot, like .jpg) of the image.</returns>
    /// <exception cref="ArgumentNullException">imageFormat</exception>
    /// <exception cref="NotImplementedException">Unknown image format: " + imageFormat.ToString()</exception>
    public static string GetFileExtension(this Image image)
    {
      return GetFileExtension(image.RawFormat);
    }

    /// <summary>
    /// Gets the file extension (lower case letters with a preceding dot, like .jpg) corresponding to the image format.
    /// </summary>
    /// <param name="imageFormat">The image format.</param>
    /// <returns>The file extension (lower case letters with a preceding dot, like .jpg) corresponding to the image format.</returns>
    /// <exception cref="ArgumentNullException">imageFormat</exception>
    /// <exception cref="NotImplementedException">Unknown image format: " + imageFormat.ToString()</exception>
    public static string GetFileExtension(this ImageFormat imageFormat)
    {
      if (imageFormat == ImageFormat.Bmp)
        return ".bmp";
      else if (imageFormat == ImageFormat.Emf)
        return ".emf";
      else if (imageFormat == ImageFormat.Exif)
        return ".exif";
      else if (imageFormat == ImageFormat.Gif)
        return ".gif";
      else if (imageFormat == ImageFormat.Icon)
        return ".ico";
      else if (imageFormat == ImageFormat.Jpeg)
        return ".jpg";
      else if (imageFormat == ImageFormat.MemoryBmp)
        return ".bmp";
      else if (imageFormat == ImageFormat.Png)
        return ".png";
      else if (imageFormat == ImageFormat.Tiff)
        return ".tif";
      else if (imageFormat == ImageFormat.Wmf)
        return ".wmf";
      else if (imageFormat == null)
        throw new ArgumentNullException(nameof(imageFormat));
      else
        throw new NotImplementedException("Unknown image format: " + imageFormat.ToString());
    }
  }
}
