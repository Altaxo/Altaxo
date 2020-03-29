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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Altaxo.Geometry;

namespace Altaxo.Graph
{

  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public abstract class ImageProxy : Main.IImmutable
  {
    protected const double InchesPerPoint = 1 / 72.0;
    protected const double PointsPerInch = 72.0;

    public static (VectorD2D SizePoint, VectorD2D SizePixel, VectorD2D Resolution) GetImageInformation(Stream str)
    {
      Image img = null;
      try
      {
        str.Seek(0, SeekOrigin.Begin);
        img = Image.FromStream(str);

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

    public override string ToString()
    {
      return Name;
    }

    public static ImageProxy FromFile(string fullpath)
    {
      return MemoryStreamImageProxy.FromFile(fullpath);
    }

    public static ImageProxy FromResource(string fullpath)
    {
      return ResourceImageProxy.FromResource(fullpath);
    }

    public static ImageProxy FromImage(System.Drawing.Image image)
    {
      return SystemDrawingImageProxyExtensions.FromImage(image);
    }

    public static ImageProxy FromImage(System.Drawing.Image image, string name)
    {
      return SystemDrawingImageProxyExtensions.FromImage(image, name);
    }

    public static ImageProxy FromStream(Stream istr, string name)
    {
      return MemoryStreamImageProxy.FromStream(istr, name);
    }



    public abstract string ContentHash { get; }

    public abstract Stream GetContentStream();

    public abstract bool IsValid { get; }

    public abstract string Name { get; }

    public bool HasSameContentAs(ImageProxy from)
    {
      return ContentHash == from.ContentHash;
    }

    /// <summary>Returns the original size of the image in points (1/72 inch).</summary>
    public abstract VectorD2D Size { get; }

  }
}
