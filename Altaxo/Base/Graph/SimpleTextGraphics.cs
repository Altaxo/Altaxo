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
using Altaxo.Serialization;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for SimpleTextGraphics.
	/// </summary>
	[SerializationSurrogate(0,typeof(SimpleTextGraphics.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class SimpleTextGraphics : GraphicsObject
	{
		protected Font m_Font;
		protected string m_Text = "";
		protected Color m_Color = Color.Black;

		#region Serialization
		/// <summary>Used to serialize the SimpleTextGraphics Version 0.</summary>
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes SimpleTextGraphics Version 0.
			/// </summary>
			/// <param name="obj">The SimpleTextGraphics to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				SimpleTextGraphics s = (SimpleTextGraphics)obj;
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
				info.AddValue("Text",s.m_Text);
				info.AddValue("Font",s.m_Font);
				info.AddValue("Color",s.m_Color);
				
			}
			/// <summary>
			/// Deserializes the SimpleTextGraphics Version 0.
			/// </summary>
			/// <param name="obj">The empty SimpleTextGraphics object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized SimpleTextGraphics.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				SimpleTextGraphics s = (SimpleTextGraphics)obj;
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
				s.m_Text = info.GetString("Text");
				s.m_Font = (Font)info.GetValue("Font",typeof(Font));
				s.m_Color = (Color)info.GetValue("Color",typeof(Color));

				return s;
			}
		}


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SimpleTextGraphics),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SimpleTextGraphics s = (SimpleTextGraphics)obj;
				info.AddBaseValueEmbedded(s,typeof(SimpleTextGraphics).BaseType);

				info.AddValue("Text",s.m_Text);
				info.AddValue("Font",s.m_Font);
				info.AddValue("Color",s.m_Color);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				SimpleTextGraphics s = null!=o ? (SimpleTextGraphics)o : new SimpleTextGraphics(); 
				info.GetBaseValueEmbedded(s,typeof(SimpleTextGraphics).BaseType,parent);

				s.m_Text = info.GetString("Text");
				s.m_Font = (Font)info.GetValue("Font",typeof(Font));
				s.m_Color = (Color)info.GetValue("Color",typeof(Color));
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

		public SimpleTextGraphics()
		{
		}

		public SimpleTextGraphics(SimpleTextGraphics from)
			:
			base(from)
		{
			this.m_Font = null==from.m_Font ? null : (Font)from.m_Font.Clone();
			this.m_Text = from.m_Text;
			this.m_Color = from.m_Color;
		}

		public SimpleTextGraphics(PointF graphicPosition, string text, 
			Font textFont, Color textColor)
		{
			this.SetPosition(graphicPosition);
			this.Font = textFont;
			this.Text = text;
			this.Color = textColor;
		}


		public SimpleTextGraphics(	float posX, float posY, 
			string text, Font textFont, Color textColor)
			: this(new PointF(posX, posY), text, textFont, textColor)
		{
		}

		public SimpleTextGraphics(PointF graphicPosition, 
			string text, Font textFont, 
			Color textColor, float Rotation)
			: this(graphicPosition, text, textFont, textColor)
		{
			this.Rotation = Rotation;
		}

		public SimpleTextGraphics(float posX, float posY, 
			string text, 
			Font textFont, 
			Color textColor, float Rotation)
			: this(new PointF(posX, posY), text, textFont, textColor, Rotation)
		{
		}

		#endregion

		public override object Clone()
		{
			return new SimpleTextGraphics(this);
		}


		public Font Font
		{
			get
			{
				return m_Font;
			}
			set
			{
				m_Font = value;
			}
		}

		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}
		public System.Drawing.Color Color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}
		public override void Paint(Graphics g, object obj)
		{

			System.Drawing.Drawing2D.GraphicsState gs = g.Save();
			g.TranslateTransform(X,Y);
			g.RotateTransform(m_Rotation);
			
			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			strfmt.LineAlignment = StringAlignment.Near;
			strfmt.Alignment = StringAlignment.Near;

			// next statement is necessary to have a consistent string length both
			// on 0 degree rotated text and rotated text
			// without this statement, the text is fitted to the pixel grid, which
			// leads to "steps" during scaling
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			if(this.AutoSize)
			{
				SizeF mySize = g.MeasureString(m_Text, m_Font);
				this.Width = mySize.Width;
				this.Height = mySize.Height;
				g.DrawString(m_Text, m_Font, new SolidBrush(m_Color), 0, 0, strfmt);
			}
			else
			{
				System.Drawing.RectangleF rect = new RectangleF(0, 0, this.Width, this.Height);
				g.DrawString(m_Text, m_Font, new SolidBrush(m_Color), rect, strfmt);
			}
			
			g.Restore(gs);
		}
	}



}
