/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
	public abstract class ImageGraphic : GraphObject
	{
		protected ImageGraphic()
			:	base()
		{
		}

		public abstract Image GetImage();
	} // 	End Class

	public class LinkedImageGraphic : ImageGraphic
	{
		protected string m_ImagePath;
		[NonSerialized()] 
		protected Image m_Image;

#region "Constructors"
		public  LinkedImageGraphic()
			:
			base()
		{
		} 

		public LinkedImageGraphic( PointF graphicPosition ,  string ImagePath)
			:
			this()
		{
			this.SetPosition(graphicPosition);
			this.ImagePath = ImagePath;
		}
	
		public  LinkedImageGraphic( float posX, float  posY, string ImagePath)
			:
			this(new PointF(posX, posY), ImagePath)
		{
		}
		public  LinkedImageGraphic( PointF graphicPosition, SizeF graphicSize, string ImagePath)
			:
			this(graphicPosition, ImagePath)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		} 

		public  LinkedImageGraphic( float posX, float posY,  SizeF graphicSize, string ImagePath )
			:
			this(new PointF(posX, posY), graphicSize, ImagePath)
		{
		}
		public  LinkedImageGraphic( float posX, float posY, float width, float height, string ImagePath)
			:
			this(new PointF(posX, posY), new SizeF(width, height), ImagePath)
		{
		} 

		public  LinkedImageGraphic( PointF graphicPosition, float Rotation , string ImagePath)
			:
			this(graphicPosition, ImagePath)
		{
			this.Rotation = Rotation;
		} 

		public LinkedImageGraphic( float posX , float  posY, float Rotation, string ImagePath)
			:
			this(new PointF(posX, posY), Rotation, ImagePath)
		{
		}

		public LinkedImageGraphic(  PointF graphicPosition, SizeF graphicSize, float Rotation, string ImagePath)
			:
			this(graphicPosition, Rotation, ImagePath)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}

		public LinkedImageGraphic( float posX, float posY, SizeF graphicSize, float Rotation, string ImagePath)
			:
			this(new PointF(posX, posY), graphicSize, Rotation, ImagePath)
		{
		} 

		public LinkedImageGraphic( float posX, float posY, float  width, float height, float Rotation, string ImagePath)
			:
			this(new PointF(posX, posY), new SizeF(width, height), Rotation, ImagePath)
		{
		}

#endregion

		public override Image GetImage()
		{
			try
			{
				if( m_Image==null)
					m_Image = new Bitmap(m_ImagePath);
				return m_Image;
			}
			catch( System.Exception)
			{
				return null;
			}
		}

		public  string  ImagePath
		{
			get
			{
				return m_ImagePath;
			}
			set
			{
				if(value != m_ImagePath)
				{
					m_ImagePath = value;
					m_Image = null;
				}
			}
		}

		public override  void Paint( Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			g.TranslateTransform(X,Y);
			if( m_Rotation != 0)
				g.RotateTransform(m_Rotation);
	  
			Image myImage = this.GetImage();
	
			if(null!=myImage)
			{
				if( this.AutoSize )
				{
					float myNewWidth = (myImage.Width / myImage.HorizontalResolution) * g.DpiX;
					float myNewHeight = (myImage.Height / myImage.VerticalResolution) * g.DpiY;
					this.Height = myNewHeight;
					this.Width = myNewWidth;
				}
				g.DrawImage(myImage, 0, 0, Width, Height);
			}
	
			g.Restore(gs);
		}
	} // End Class

	[Serializable()]
	public class EmbeddedImageGraphic : ImageGraphic
	{
		protected Image m_Image; 

#region "Constructors"

		public  EmbeddedImageGraphic()
			:
			base()
		{
		}
 

		public  EmbeddedImageGraphic( PointF graphicPosition, Image startingImage)
			:
			this()
		{
			this.SetPosition(graphicPosition);
			this.Image = startingImage;
		}

		public  EmbeddedImageGraphic( float posX,  float posY, Image startingImage)
			:
			this(new PointF(posX, posY), startingImage)
		{
		}

		public EmbeddedImageGraphic( PointF graphicPosition, SizeF graphicSize, Image startingImage)
			:
			this(graphicPosition, startingImage)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}

		public  EmbeddedImageGraphic( float posX,  float posY, SizeF graphicSize, Image startingImage)
			:
			this(new PointF(posX, posY), graphicSize, startingImage)
		{
		}
		public  EmbeddedImageGraphic( float posX , float posY, float width, float height, Image startingImage)
			:
			this(new PointF(posX, posY), new SizeF(width, height), startingImage)
		{
		}

		public EmbeddedImageGraphic( PointF graphicPosition, float Rotation, Image startingImage)
			:
			this(graphicPosition, startingImage)
		{
			this.Rotation = Rotation;
		}

		public  EmbeddedImageGraphic( float posX, float posY, float Rotation, Image startingImage)
			:
			this(new PointF(posX, posY), Rotation, startingImage)
		{
		}

		public  EmbeddedImageGraphic( PointF graphicPosition, SizeF graphicSize, float Rotation, Image startingImage)
			:
			this(graphicPosition, Rotation, startingImage)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}
		public  EmbeddedImageGraphic( float posX, float posY, SizeF graphicSize, float Rotation, Image startingImage)
			:
			this(new PointF(posX, posY), graphicSize, Rotation, startingImage)
		{
		}

		public EmbeddedImageGraphic(float posX, float posY, float width, float height, float Rotation, Image startingImage)
			:
			this(new PointF(posX, posY), new SizeF(width, height), Rotation, startingImage)
		{
		} 

#endregion


		public Image Image
		{
			get
			{
				return m_Image;
			}
			set
			{
				m_Image = value;
			}
		}
	
		public override Image GetImage()
		{
			return this.Image;
		}

		public override void Paint( Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			g.TranslateTransform(X,Y);
			if(m_Rotation!=0)
				g.RotateTransform(m_Rotation);

			if(null!=m_Image)
			{
				if( this.AutoSize )
				{
					this.Width = (m_Image.Width / m_Image.HorizontalResolution) * g.DpiX;
					this.Height = (m_Image.Height / m_Image.VerticalResolution) * g.DpiY;
				}
	
				g.DrawImage(m_Image, 0, 0, Width, Height);
	
			}
	
			g.Restore(gs);
		}

	} // End Class
}
