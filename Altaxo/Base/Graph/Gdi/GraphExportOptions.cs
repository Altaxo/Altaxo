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

using Altaxo.Gui.Common.MultiRename;
using Altaxo.Main.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	public class GraphExportOptions : Main.ICopyFrom
	{
		private ImageFormat _imageFormat;
		private PixelFormat _pixelFormat;
		private BrushX _backgroundBrush;
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
				GraphExportOptions s = (GraphExportOptions)obj;

				info.AddValue("ImageFormat", s._imageFormat);
				info.AddEnum("PixelFormat", s._pixelFormat);
				info.AddValue("Background", s._backgroundBrush);
				info.AddValue("SourceResolution", s._sourceDpiResolution);
				info.AddValue("DestinationResolution", s._destinationDpiResolution);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (GraphExportOptions)o : new GraphExportOptions();

				s._imageFormat = (ImageFormat)info.GetValue("ImageFormat", s);
				s._pixelFormat = (PixelFormat)info.GetEnum("PixelFormat", typeof(PixelFormat));
				s.BackgroundBrush = (BrushX)info.GetValue("Background");
				s._sourceDpiResolution = info.GetDouble("SourceResolution");
				s._destinationDpiResolution = info.GetDouble("DestinationResolution");

				return s;
			}
		}

		#endregion Serialization

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as GraphExportOptions;

			if (null != from)
			{
				this._imageFormat = from.ImageFormat;
				this._pixelFormat = from.PixelFormat;
				this._backgroundBrush = null == from._backgroundBrush ? null : from._backgroundBrush.Clone();
				this.SourceDpiResolution = from.SourceDpiResolution;
				this.DestinationDpiResolution = from.DestinationDpiResolution;
				return true;
			}

			return false;
		}

		public GraphExportOptions()
		{
			this._imageFormat = System.Drawing.Imaging.ImageFormat.Png;
			this._pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			this.SourceDpiResolution = 300;
			this.DestinationDpiResolution = 300;
			this.BackgroundBrush = null;
		}

		public GraphExportOptions(GraphExportOptions from)
		{
			CopyFrom(from);
		}

		object ICloneable.Clone()
		{
			return new GraphExportOptions(this);
		}

		public virtual GraphExportOptions Clone()
		{
			return new GraphExportOptions(this);
		}

		public ImageFormat ImageFormat { get { return _imageFormat; } }

		public PixelFormat PixelFormat { get { return _pixelFormat; } }

		public BrushX BackgroundBrush
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

		public bool TrySetImageAndPixelFormat(ImageFormat imgfmt, PixelFormat pixfmt)
		{
			if (!IsVectorFormat(imgfmt) && !CanCreateAndSaveBitmap(imgfmt, pixfmt))
				return false;

			_imageFormat = imgfmt;
			_pixelFormat = pixfmt;

			return true;
		}

		public BrushX GetDefaultBrush()
		{
			if (IsVectorFormat(_imageFormat) || HasPixelFormatAlphaChannel(_pixelFormat))
				return null;
			else
				return new BrushX(NamedColors.White);
		}

		public BrushX GetBrushOrDefaultBrush()
		{
			if (null != _backgroundBrush)
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
			else return ".img";
		}

		private static GraphExportOptions _currentSetting = new GraphExportOptions();

		public static GraphExportOptions CurrentSetting
		{
			get
			{
				return _currentSetting;
			}
		}

		public static bool IsVectorFormat(ImageFormat fmt)
		{
			return ImageFormat.Emf == fmt || ImageFormat.Wmf == fmt;
		}

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