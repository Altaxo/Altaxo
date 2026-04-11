#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Altaxo.Drawing;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Stores options used to export graph documents as images.
  /// </summary>
  public class GraphExportOptions : Main.ICopyFrom
  {
    private ImageFormat _imageFormat;
    private PixelFormat _pixelFormat;
    private BrushX? _backgroundBrush;
    private double _sourceDpiResolution;
    private double _destinationDpiResolution;

    #region Serialization

    /// <summary>
    /// Initial version (2014-01-18)
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphExportOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GraphExportOptions)obj;

        info.AddValue("ImageFormat", s._imageFormat);
        info.AddEnum("PixelFormat", s._pixelFormat);
        info.AddValueOrNull("Background", s._backgroundBrush);
        info.AddValue("SourceResolution", s._sourceDpiResolution);
        info.AddValue("DestinationResolution", s._destinationDpiResolution);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (GraphExportOptions?)o ?? new GraphExportOptions();

        s._imageFormat = (ImageFormat)info.GetValue("ImageFormat", s);
        s._pixelFormat = (PixelFormat)info.GetEnum("PixelFormat", typeof(PixelFormat));
        s.BackgroundBrush = info.GetValueOrNull<BrushX>("Background", s);
        s._sourceDpiResolution = info.GetDouble("SourceResolution");
        s._destinationDpiResolution = info.GetDouble("DestinationResolution");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Copies the state from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    [MemberNotNull(nameof(_imageFormat))]
    protected void CopyFrom(GraphExportOptions from)
    {
      _imageFormat = from.ImageFormat;
      _pixelFormat = from.PixelFormat;
      _backgroundBrush = from._backgroundBrush;
      SourceDpiResolution = from.SourceDpiResolution;
      DestinationDpiResolution = from.DestinationDpiResolution;
    }

    /// <inheritdoc />
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is GraphExportOptions from)
      {
        CopyFrom(from);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphExportOptions"/> class.
    /// </summary>
    public GraphExportOptions()
    {
      _imageFormat = System.Drawing.Imaging.ImageFormat.Png;
      _pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
      SourceDpiResolution = 300;
      DestinationDpiResolution = 300;
      BackgroundBrush = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphExportOptions"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public GraphExportOptions(GraphExportOptions from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new GraphExportOptions(this);
    }

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public virtual GraphExportOptions Clone()
    {
      return new GraphExportOptions(this);
    }

    /// <summary>
    /// Gets the image format.
    /// </summary>
    public ImageFormat ImageFormat { get { return _imageFormat; } }

    /// <summary>
    /// Gets the pixel format.
    /// </summary>
    public PixelFormat PixelFormat { get { return _pixelFormat; } }

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public BrushX? BackgroundBrush
    {
      get
      {
        return _backgroundBrush;
      }
      set
      {
        _backgroundBrush = value;
      }
    }

    /// <summary>
    /// Gets or sets the source DPI resolution used for rendering.
    /// </summary>
    public double SourceDpiResolution
    {
      get
      {
        return _sourceDpiResolution;
      }
      set
      {
        if (!(value > 0))
          throw new ArgumentException("SourceDpiResolution has to be >0");

        _sourceDpiResolution = value;
      }
    }

    /// <summary>
    /// Gets or sets the destination DPI resolution stored in the resulting image.
    /// </summary>
    public double DestinationDpiResolution
    {
      get
      {
        return _destinationDpiResolution;
      }
      set
      {
        if (!(value > 0))
          throw new ArgumentException("DestinationDpiResolution has to be >0");

        _destinationDpiResolution = value;
      }
    }

    /// <summary>
    /// Tries to set the image and pixel format.
    /// </summary>
    /// <param name="imgfmt">The image format.</param>
    /// <param name="pixfmt">The pixel format.</param>
    /// <returns><c>true</c> if the combination is valid; otherwise, <c>false</c>.</returns>
    public bool TrySetImageAndPixelFormat(ImageFormat imgfmt, PixelFormat pixfmt)
    {
      if (!IsVectorFormat(imgfmt) && !CanCreateAndSaveBitmap(imgfmt, pixfmt))
        return false;

      _imageFormat = imgfmt;
      _pixelFormat = pixfmt;

      return true;
    }

    /// <summary>
    /// Gets the default background brush for the configured export format.
    /// </summary>
    /// <returns>The default background brush, or <c>null</c>.</returns>
    public BrushX? GetDefaultBrush()
    {
      if (IsVectorFormat(_imageFormat) || HasPixelFormatAlphaChannel(_pixelFormat))
        return null;
      else
        return new BrushX(NamedColors.White);
    }

    /// <summary>
    /// Gets the configured background brush or the default brush if none is configured.
    /// </summary>
    /// <returns>The configured or default brush.</returns>
    public BrushX? GetBrushOrDefaultBrush()
    {
      if (_backgroundBrush is not null)
        return _backgroundBrush;
      else
        return GetDefaultBrush();
    }

    /// <summary>
    /// Returns the default file name extension (including leading dot) for the current image format.
    /// </summary>
    /// <returns>Default file name extension (including leading dot) for the current image format</returns>
    public string GetDefaultFileNameExtension()
    {
      return GetDefaultFileNameExtension(_imageFormat);
    }

    /// <summary>
    /// Returns the default file name extension (including leading dot) for the current image format.
    /// </summary>
    /// <param name="imageFormat">The image format for which to retrieve the default extension.</param>
    /// <returns>Default file name extension (including leading dot) for the current image format</returns>
    public static string GetDefaultFileNameExtension(ImageFormat imageFormat)
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
      else if (imageFormat == ImageFormat.Png)
        return ".png";
      else if (imageFormat == ImageFormat.Tiff)
        return ".tif";
      else if (imageFormat == ImageFormat.Wmf)
        return ".wmf";
      else
        return ".img";
    }

    private static GraphExportOptions _currentSetting = new GraphExportOptions();

    /// <summary>
    /// Gets the current global graph export setting.
    /// </summary>
    public static GraphExportOptions CurrentSetting
    {
      get
      {
        return _currentSetting;
      }
    }

    /// <summary>
    /// Determines whether the specified image format is a vector format.
    /// </summary>
    /// <param name="fmt">The image format.</param>
    /// <returns><c>true</c> if the format is vector based; otherwise, <c>false</c>.</returns>
    public static bool IsVectorFormat(ImageFormat fmt)
    {
      return ImageFormat.Emf == fmt || ImageFormat.Wmf == fmt;
    }

    /// <summary>
    /// Determines whether a bitmap can be created with the specified pixel format.
    /// </summary>
    /// <param name="fmt">The pixel format.</param>
    /// <returns><c>true</c> if creation is possible; otherwise, <c>false</c>.</returns>
    public static bool CanCreateBitmap(PixelFormat fmt)
    {
      try
      {
        var bmp = new Bitmap(4, 4, fmt);
        bmp.Dispose();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Determines whether a bitmap can be created and saved with the specified format combination.
    /// </summary>
    /// <param name="imgfmt">The image format.</param>
    /// <param name="pixfmt">The pixel format.</param>
    /// <returns><c>true</c> if creation and saving are possible; otherwise, <c>false</c>.</returns>
    public static bool CanCreateAndSaveBitmap(ImageFormat imgfmt, PixelFormat pixfmt)
    {
      try
      {
        using (var bmp = new Bitmap(8, 8, pixfmt))
        {
          using (var str = new System.IO.MemoryStream())
          {
            bmp.Save(str, imgfmt);
            str.Close();
          }
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Determines whether the specified pixel format contains an alpha channel.
    /// </summary>
    /// <param name="fmt">The pixel format.</param>
    /// <returns><c>true</c> if the format contains alpha; otherwise, <c>false</c>.</returns>
    public static bool HasPixelFormatAlphaChannel(PixelFormat fmt)
    {
      return
        PixelFormat.Alpha == fmt ||
        PixelFormat.Canonical == fmt ||
        PixelFormat.Format16bppArgb1555 == fmt ||
        PixelFormat.Format32bppArgb == fmt ||
        PixelFormat.Format32bppPArgb == fmt ||
        PixelFormat.Format64bppArgb == fmt ||
        PixelFormat.Format64bppPArgb == fmt ||
        PixelFormat.PAlpha == fmt;
    }
  }
}
