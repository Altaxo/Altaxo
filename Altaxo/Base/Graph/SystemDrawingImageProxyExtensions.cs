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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Altaxo.Geometry;

namespace Altaxo.Graph
{
  public static class SystemDrawingImageProxyExtensions
  {
    public static MemoryStream ImageToStream(Image image, ImageFormat format)
    {
      var str = new MemoryStream();
      image.Save(str, format);
      str.Flush();
      return str;
    }

    public static new MemoryStreamImageProxy FromImage(System.Drawing.Image image)
    {
      return FromImage(image, null);
    }

    public static MemoryStreamImageProxy FromImage(System.Drawing.Image image, string name)
    {

      if (image is System.Drawing.Imaging.Metafile mf) // If image is a metafile, then convert it to a bitmap
      {
        var newimg = new System.Drawing.Bitmap(mf.Width, mf.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(newimg))
        {
          g.DrawImage(mf, 0, 0, mf.Width, mf.Height);
          // g.DrawLine(Pens.Red, 0, 0, mf.Width, mf.Height);
        }
        newimg.SetResolution(mf.HorizontalResolution, mf.VerticalResolution);
        image = newimg;
      }


      var fmt1 = ".jpg";
      var str1 = ImageToStream(image, ImageFormat.Jpeg);
      var fmt2 = ".png";
      var str2 = ImageToStream(image, ImageFormat.Png);



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
        useStreamDirectly: true,
        hash: imghash,
        extension: imgextension,
        name: imgname,
        url: imgurl,
        imageSizePixel: new VectorD2D(image.Width, image.Height),
        imageSizePoint: new VectorD2D(72.0 * image.Width / image.HorizontalResolution, 72.0 * image.Height / image.VerticalResolution),
        imageResolutionDpi: new VectorD2D(image.HorizontalResolution, image.VerticalResolution)
        );
    }

    public static System.Drawing.Image GetImage(MemoryStreamImageProxy proxy)
    {
      var stream = proxy.GetContentStream();
      if (stream == null)
        throw new InvalidOperationException("The stream of the proxy is null");
      stream.Seek(0, SeekOrigin.Begin);
      return Image.FromStream(stream);
    }

    public static System.Drawing.Image GetImage(Stream stream, bool disposeStream = true)
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
