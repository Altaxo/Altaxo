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
using Altaxo.Serialization;


namespace Altaxo.Graph
{
	[SerializationSurrogate(0,typeof(ImageGraphic.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public abstract class ImageGraphic : GraphObject
	{
		#region Serialization
		/// <summary>Used to serialize the ImageGraphic Version 0.</summary>
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes ImageGraphic Version 0.
			/// </summary>
			/// <param name="obj">The ImageGraphic to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				ImageGraphic s = (ImageGraphic)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
	
				// serialize the base class
				surr.GetObjectData(obj,info,context); // stream the data of the base object
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

			}
			/// <summary>
			/// Deserializes the ImageGraphic Version 0.
			/// </summary>
			/// <param name="obj">The empty ImageGraphic object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized ImageGraphic.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				ImageGraphic s = (ImageGraphic)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
				// deserialize the base class
				surr.SetObjectData(obj,info,context,selector);
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

				return s;
			}
		}


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImageGraphic),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ImageGraphic s = (ImageGraphic)obj;
				info.AddBaseValueEmbedded(s,typeof(ImageGraphic).BaseType);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				info.OpenInnerContent();
				ImageGraphic s =  (ImageGraphic)o;
				info.GetBaseValueEmbedded(s,typeof(ImageGraphic).BaseType,parent);
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
		}
		#endregion

		protected ImageGraphic()
			:
			base()
		{
		}
		protected ImageGraphic(ImageGraphic from)
			:
			base(from)
		{
		}

		public abstract Image GetImage();
	} // 	End Class



	[SerializationSurrogate(0,typeof(LinkedImageGraphic.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class LinkedImageGraphic : ImageGraphic
	{
		protected string m_ImagePath;
		[NonSerialized()] 
		protected Image m_Image;


		#region Serialization
		/// <summary>Used to serialize the LinkedImageGraphic Version 0.</summary>
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes LinkedImageGraphic Version 0.
			/// </summary>
			/// <param name="obj">The LinkedImageGraphic to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				LinkedImageGraphic s = (LinkedImageGraphic)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss= AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
	
				// serialize the base class
				surr.GetObjectData(obj,info,context); // stream the data of the base object
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

				info.AddValue("ImagePath",s.m_ImagePath);

			}
			/// <summary>
			/// Deserializes the LinkedImageGraphic Version 0.
			/// </summary>
			/// <param name="obj">The empty LinkedImageGraphic object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized LinkedImageGraphic.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				LinkedImageGraphic s = (LinkedImageGraphic)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss= AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
				// deserialize the base class
				surr.SetObjectData(obj,info,context,selector);
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

				s.m_ImagePath = info.GetString("ImagePath");
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedImageGraphic),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinkedImageGraphic s = (LinkedImageGraphic)obj;
				info.AddBaseValueEmbedded(s,typeof(LinkedImageGraphic).BaseType);
				info.AddValue("ImagePath",s.m_ImagePath);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				info.OpenInnerContent();
				LinkedImageGraphic s = null!=o ? (LinkedImageGraphic)o : new LinkedImageGraphic();
				info.GetBaseValueEmbedded(s,typeof(LinkedImageGraphic).BaseType,parent);
				s.m_ImagePath = info.GetString("ImagePath");
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

		public LinkedImageGraphic(LinkedImageGraphic from)
		:
			base(from)
		{
			this.m_ImagePath = from.m_ImagePath;
			this.m_Image     = null==from.m_Image ? null : (Image)from.m_Image.Clone();
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

	[SerializationSurrogate(0,typeof(EmbeddedImageGraphic.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class EmbeddedImageGraphic : ImageGraphic
	{
		protected Image m_Image; 

		#region Serialization
		/// <summary>Used to serialize the EmbeddedImageGraphic Version 0.</summary>
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes EmbeddedImageGraphic Version 0.
			/// </summary>
			/// <param name="obj">The EmbeddedImageGraphic to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				EmbeddedImageGraphic s = (EmbeddedImageGraphic)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
	
				// serialize the base class
				surr.GetObjectData(obj,info,context); // stream the data of the base object
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}

				info.AddValue("Image",s.m_Image);

			}
			/// <summary>
			/// Deserializes the EmbeddedImageGraphic Version 0.
			/// </summary>
			/// <param name="obj">The empty EmbeddedImageGraphic object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized EmbeddedImageGraphic.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				EmbeddedImageGraphic s = (EmbeddedImageGraphic)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
				if(null!=ss)
				{
				System.Runtime.Serialization.ISerializationSurrogate surr =
					ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
				// deserialize the base class
				surr.SetObjectData(obj,info,context,selector);
				}
				else 
				{
					throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
				}
				s.m_Image = (Image)info.GetValue("Image",typeof(Image));
				return s;
			}
		}


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmbeddedImageGraphic),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				EmbeddedImageGraphic s = (EmbeddedImageGraphic)obj;
				info.AddBaseValueEmbedded(s,typeof(EmbeddedImageGraphic).BaseType);
				info.AddValue("Image",s.m_Image);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				info.OpenInnerContent();
				EmbeddedImageGraphic s = null!=o ? (EmbeddedImageGraphic)o : new EmbeddedImageGraphic();
				info.GetBaseValueEmbedded(s,typeof(EmbeddedImageGraphic).BaseType,parent);
				s.m_Image = (Image)info.GetValue("Image",typeof(Image));
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
		}
		#endregion


		#region Constructors

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

		public EmbeddedImageGraphic(EmbeddedImageGraphic from)
		:
			base(from)
		{
			this.m_Image = null==from.m_Image ? null : (Image)from.m_Image.Clone();
		}

		#endregion

		public override object Clone()
		{
			return new EmbeddedImageGraphic(this);
		}


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
