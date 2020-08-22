#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Helper functions to convert Gdi images to / from streams or image proxies.
  /// </summary>
  public static class SystemDrawingImageProxyExtensions
  {
    /// <summary>
    /// Gets a stream from a Gdi image.
    /// </summary>
    /// <param name="image">The image to convert to a stream.</param>
    /// <param name="format">The format to save the image to the stream.</param>
    /// <returns></returns>
    public static MemoryStream GetStreamFromImage(Image image, ImageFormat format)
    {
      var str = new MemoryStream();
      image.Save(str, format);
      str.Flush();
      return str;
    }

    /// <summary>
    /// Gets an image proxy from a Gdi image.
    /// </summary>
    /// <param name="image">The Gdi image to convert.</param>
    /// <returns>The image proxy.</returns>
    /// <remarks>Since metafile images can not be converted to a independent image stream, those images are converted to bitmap
    /// images and then converted to an independent image proxy, using the resolution of the metafile image.</remarks>
    public static MemoryStreamImageProxy GetImageProxyFromImage(Image image)
    {
      if (image is null)
        throw new ArgumentNullException(nameof(image));

      return GetImageProxyFromImage(image, null);
    }

    /// <summary>
    /// Gets an image proxy from a Gdi image.
    /// </summary>
    /// <param name="image">The Gdi image to convert.</param>
    /// <param name="name">A descriptive name of the image (is stored as name in the image proxy).</param>
    /// <returns>The image proxy.</returns>
    /// <remarks>Since metafile images can not be converted to a independent image stream, those images are converted to bitmap
    /// images and then converted to an independent image proxy, using the resolution of the metafile image.</remarks>
    public static MemoryStreamImageProxy GetImageProxyFromImage(Image image, string? name)
    {
      if (image is null)
        throw new ArgumentNullException(nameof(image));

      if (image is Metafile mf) // If image is a metafile, then convert it to a bitmap
      {
        var newimg = new Bitmap(mf.Width, mf.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(newimg))
        {
          g.DrawImage(mf, 0, 0, mf.Width, mf.Height);
          // g.DrawLine(Pens.Red, 0, 0, mf.Width, mf.Height);
        }
        newimg.SetResolution(mf.HorizontalResolution, mf.VerticalResolution);
        image = newimg;
      }


      var fmt1 = ".jpg";
      var str1 = GetStreamFromImage(image, ImageFormat.Jpeg);
      var fmt2 = ".png";
      var str2 = GetStreamFromImage(image, ImageFormat.Png);



      MemoryStream imgstream;
      string imgextension;

      if (str2.Length < str1.Length)
      {
        imgstream = str2;
        imgextension = fmt2;
        str1.Dispose();
      }
      else
      {
        imgstream = str1;
        imgextension = fmt1;
        str2.Dispose();
      }

      var imghash = MemoryStreamImageProxy.ComputeStreamHash(imgstream);
      string imgname;
      string imgurl;

      if (string.IsNullOrEmpty(name))
      {
        imgname = imghash + imgextension;
        imgurl = "image://" + imgname;
      }
      else
      {
        imgname = name;
        imgurl = "image://" + name;
      }



      return new MemoryStreamImageProxy(
        stream: imgstream,
          hash: imghash,
          extension: imgextension,
          name: imgname,
          url: imgurl,
          imageSizePixel: new VectorD2D(image.Width, image.Height),
          imageSizePoint: new VectorD2D(72.0 * image.Width / image.HorizontalResolution, 72.0 * image.Height / image.VerticalResolution),
          imageResolutionDpi: new VectorD2D(image.HorizontalResolution, image.VerticalResolution)
        );
    }

    /// <summary>
    /// Gets a Gdi image from an <see cref="ImageProxy"/>.
    /// </summary>
    /// <param name="proxy">The image proxy.</param>
    /// <returns>The Gdi image created from the image proxy. This object must be disposed if no longer in use.</returns>
    /// <exception cref="InvalidOperationException">The stream of the proxy is null</exception>
    public static Image GetImage(ImageProxy proxy)
    {
      if (proxy is null)
        throw new ArgumentNullException(nameof(proxy));

      var stream = proxy.GetContentStream();
      if (stream == null)
        throw new InvalidOperationException("The stream of the proxy is null");
      stream.Seek(0, SeekOrigin.Begin);
      return Image.FromStream(stream);
    }

    /// <summary>
    /// Gets a Gdi image from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="disposeStream">if set to <c>true</c>, the stream is disposed after reading the image; otherwise, the caller is responsible for disposing of the stream.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">stream</exception>
    public static Image GetImage(Stream stream, bool disposeStream = true)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));

      try
      {
        stream.Seek(0, SeekOrigin.Begin);
        return Image.FromStream(stream);
      }
      finally
      {
        if (disposeStream)
        {
          stream?.Dispose();
        }
      }
    }
  }
}
