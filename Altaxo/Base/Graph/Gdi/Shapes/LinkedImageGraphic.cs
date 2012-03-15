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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using System.IO;


namespace Altaxo.Graph.Gdi.Shapes
{

	[Serializable]
	public class LinkedImageGraphic : ImageGraphic
	{
		protected string _imagePath;
		[NonSerialized()]
		protected Image _cachedImage;


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LinkedImageGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedImageGraphic), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinkedImageGraphic s = (LinkedImageGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(LinkedImageGraphic).BaseType);
				info.AddValue("ImagePath", s._imagePath);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				LinkedImageGraphic s = null != o ? (LinkedImageGraphic)o : new LinkedImageGraphic();
				info.GetBaseValueEmbedded(s, typeof(LinkedImageGraphic).BaseType, parent);
				s._imagePath = info.GetString("ImagePath");
				return s;
			}
		}


		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
			// load the image into memory here
			GetImage();
		}
		#endregion



		#region Constructors
		public LinkedImageGraphic()
			:
			base()
		{
		}

		public LinkedImageGraphic(PointD2D graphicPosition, string ImagePath)
			:
			this()
		{
			this.SetPosition(graphicPosition);
			this.ImagePath = ImagePath;
		}

		public LinkedImageGraphic(double posX, double posY, string ImagePath)
			:
			this(new PointD2D(posX, posY), ImagePath)
		{
		}
		public LinkedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, string ImagePath)
			:
			this(graphicPosition, ImagePath)
		{
			this.SetSize(graphicSize.X, graphicSize.X);
		}

		public LinkedImageGraphic(double posX, double posY, PointD2D graphicSize, string ImagePath)
			:
			this(new PointD2D(posX, posY), graphicSize, ImagePath)
		{
		}
		public LinkedImageGraphic(double posX, double posY, double width, double height, string ImagePath)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height), ImagePath)
		{
		}

		public LinkedImageGraphic(PointD2D graphicPosition, double Rotation, string ImagePath)
			:
			this(graphicPosition, ImagePath)
		{
			this.Rotation = Rotation;
		}

		public LinkedImageGraphic(double posX, double posY, double Rotation, string ImagePath)
			:
			this(new PointD2D(posX, posY), Rotation, ImagePath)
		{
		}

		public LinkedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, double Rotation, string ImagePath)
			:
			this(graphicPosition, Rotation, ImagePath)
		{
			this.SetSize(graphicSize.X, graphicSize.X);
		}

		public LinkedImageGraphic(double posX, double posY, PointD2D graphicSize, double Rotation, string ImagePath)
			:
			this(new PointD2D(posX, posY), graphicSize, Rotation, ImagePath)
		{
		}

		public LinkedImageGraphic(double posX, double posY, double width, double height, double Rotation, string ImagePath)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation, ImagePath)
		{
		}

		public LinkedImageGraphic(LinkedImageGraphic from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}
		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as LinkedImageGraphic;
				if (from != null)
				{
					this._imagePath = from._imagePath;
					this._cachedImage = null == from._cachedImage ? null : (Image)from._cachedImage.Clone();
				}
			}
			return isCopied;
		}

		#endregion

		public override object Clone()
		{
			return new LinkedImageGraphic(this);
		}


		public override Image GetImage()
		{
			try
			{
				if (_cachedImage == null)
					_cachedImage = new Bitmap(_imagePath);
				return _cachedImage;
			}
			catch (System.Exception)
			{
				return null;
			}
		}

		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				if (value != _imagePath)
				{
					_imagePath = value;
					_cachedImage = null;
				}
			}
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			Image myImage = this.GetImage();

			if (null != myImage)
			{
				if (this.AutoSize)
				{
					double myNewWidth = (myImage.Width / myImage.HorizontalResolution) * g.DpiX;
					double myNewHeight = (myImage.Height / myImage.VerticalResolution) * g.DpiY;
					this.Height = myNewHeight;
					this.Width = myNewWidth;
				}
				g.DrawImage(myImage, 0, 0, (float)Width, (float)Height);
			}

			g.Restore(gs);
		}
	} // End Class
}
