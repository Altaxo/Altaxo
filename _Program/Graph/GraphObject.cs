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
	/// <summary>
	/// GraphObject is the abstract base class for general graphical objects on the layer,
	/// for instance text elements, lines, pictures, rectangles and so on.
	/// </summary>
	[SerializationSurrogate(0,typeof(GraphObject.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public abstract class GraphObject : System.Runtime.Serialization.IDeserializationCallback, IChangedEventSource, System.ICloneable
	{
		/// <summary>
		/// If true, the graphical object sizes itself, for instance simple text objects.
		/// </summary>
		protected bool   m_AutoSize = true;

		/// <summary>
		/// The bounds of this object.
		/// </summary>
		protected RectangleF m_Bounds = new RectangleF(0,0,0,0);

		/// <summary>
		/// The parent collection this graphical object belongs to.
		/// </summary>
		protected GraphObjectCollection m_Container=null;

		/// <summary>
		/// The position of the graphical object, normally the upper left corner. Strictly spoken,
		/// this is the position of the anchor point of the object.
		/// </summary>
		protected PointF m_Position = new PointF(0, 0);
		/// <summary>
		/// The rotation angle of the graphical object in reference to the layer.
		/// </summary>
		protected float  m_Rotation = 0;




		#region Serialization
		/// <summary>Used to serialize the GraphObject Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes GraphObject Version 0.
			/// </summary>
			/// <param name="obj">The GraphObject to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				GraphObject s = (GraphObject)obj;
				info.AddValue("Position",s.m_Position);  
				info.AddValue("Bounds",s.m_Bounds);
				info.AddValue("Rotation",s.m_Rotation);
				info.AddValue("AutoSize",s.m_AutoSize);
			}
			/// <summary>
			/// Deserializes the GraphObject Version 0.
			/// </summary>
			/// <param name="obj">The empty GraphObject object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized GraphObject.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				GraphObject s = (GraphObject)obj;

				s.m_Position = (PointF)info.GetValue("Position",typeof(PointF));  
				s.m_Bounds = (RectangleF)info.GetValue("Bounds",typeof(RectangleF));
				s.m_Rotation = info.GetSingle("Rotation");
				s.m_AutoSize = info.GetBoolean("AutoSize");

				return s;
			}
		}


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphObject),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphObject s = (GraphObject)obj;
				info.AddValue("Position",s.m_Position);  
				info.AddValue("Bounds",s.m_Bounds);
				info.AddValue("Rotation",s.m_Rotation);
				info.AddValue("AutoSize",s.m_AutoSize);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				
				GraphObject s = (GraphObject)o;

				s.m_Position = (PointF)info.GetValue("Position",s);  
				s.m_Bounds = (RectangleF)info.GetValue("Bounds",s);
				s.m_Rotation = info.GetSingle("Rotation");
				s.m_AutoSize = info.GetBoolean("AutoSize");

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion


		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy the data from.</param>
		protected GraphObject(GraphObject from)
		{
			this.m_AutoSize = from.m_AutoSize;
			this.m_Bounds  = from.m_Bounds;
			this.m_Container = null;
			this.m_Position  = from.m_Position;
			this.m_Rotation  = from.m_Rotation;
		}

		/// <summary>
		/// Initializes with default values.
		/// </summary>
		protected GraphObject()
		{
		}

		/// <summary>
		/// Initializes with a certain position in points (1/72 inch).
		/// </summary>
		/// <param name="graphicPosition">The initial position of the graphical object.</param>
		protected GraphObject(PointF graphicPosition)
		{
			SetPosition(graphicPosition);
		}

		/// <summary>
		/// Initializes the GraphObject with a certain position in points (1/72 inch).
		/// </summary>
		/// <param name="posX">The initial x position of the graphical object.</param>
		/// <param name="posY">The initial y position of the graphical object.</param>
		protected GraphObject(float posX, float posY)
			: this(new PointF(posX,posY))
		{
		}

		protected GraphObject(PointF graphicPosition, SizeF graphicSize)
			: this(graphicPosition)
		{
			SetSize(graphicSize);
			this.AutoSize = false;
		}
		protected GraphObject(float posX, float posY, SizeF graphicSize)
			: this(new PointF(posX, posY), graphicSize)
		{
		}

		protected GraphObject(float posX, float posY,
			float width, float height)
			: this(new PointF(posX, posY), new SizeF(width, height))
		{
		}

		protected GraphObject(PointF graphicPosition, float Rotation)
		{
			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		protected GraphObject(float posX, float posY, float Rotation)
			:	this(new PointF(posX, posY), Rotation)
		{
		}

		protected GraphObject(PointF graphicPosition, SizeF graphicSize, float Rotation)
			: this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}
		protected GraphObject(float posX, float posY, SizeF graphicSize, float Rotation)
			: this(new PointF(posX, posY), graphicSize, Rotation)
		{
		}

		protected GraphObject(float posX, float posY, float width, float height, float Rotation)
			: this(new PointF(posX, posY), new SizeF(width, height), Rotation)
		{
		}

		public GraphObjectCollection Container
		{
			get
			{
				return m_Container;
			}
			set
			{
				m_Container = value;
			}
		}


		public virtual GraphicsPath HitTest(PointF pt)
		{
			GraphicsPath gp = GetSelectionPath();
			return gp.IsVisible(pt) ? gp : null;
		}


		public virtual GraphicsPath GetSelectionPath()
		{
			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();

			gp.AddRectangle(new RectangleF(X+m_Bounds.X, Y+m_Bounds.Y, Width, Height));
			if(this.Rotation != 0)
			{
				myMatrix.RotateAt(this.Rotation, new PointF(X, Y), MatrixOrder.Append);
			}

			gp.Transform(myMatrix);
			return gp;
		}

		public virtual bool HitTest(RectangleF rect)
		{
			// is this object contained within the supplied rectangle

			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();


			gp.AddRectangle(new RectangleF(X+m_Bounds.X, Y+m_Bounds.Y, Width, Height));
			if(this.Rotation != 0)
			{
				myMatrix.RotateAt(this.Rotation, new PointF(this.X, this.Y), MatrixOrder.Append);
			}
			gp.Transform(myMatrix);
			RectangleF gpRect  = gp.GetBounds();
			return rect.Contains(gpRect);
		}

		public virtual bool AutoSize
		{
			get
			{
				return m_AutoSize;
			}
			set
			{
				if(value != m_AutoSize)
					m_AutoSize = value;
			}
		}
		public virtual float X
		{
			get
			{
				return m_Position.X;
			}
			set
			{
				m_Position.X = value;
			}
		}
		public virtual float Y
		{
			get
			{
				return m_Position.Y;
			}
			set
			{
				m_Position.Y = value;
			}
		}

		public virtual PointF GetPosition()
		{
			return this.m_Position;
		}

		public virtual void SetPosition(PointF Value)
		{
			this.m_Position = Value;
		}

		public PointF Position
		{
			get
			{
				return GetPosition();
			}
			set
			{
				SetPosition(value);
			}
		}

		public virtual float Height
		{
			get
			{
				return m_Bounds.Height;
			}
			set
			{
				m_Bounds.Height = value;
			}
		}
		public virtual float Width
		{
			get
			{
				return m_Bounds.Width;
			}
			set
			{
				m_Bounds.Width = value;
			}
		}

		public virtual void SetSize(SizeF Value)
		{
			m_Bounds.Size = Value;
		}
		public virtual SizeF GetSize()
		{
			return this.m_Bounds.Size;
		}

		public SizeF Size 
		{
			get
			{
				return GetSize();
			}
			set
			{
				SetSize(value);
			}
		}

		public virtual float Rotation
		{
			get
			{
				return m_Rotation;
			}
			set
			{
				m_Rotation = value;
			}
		}

		public abstract void Paint(Graphics g, object obj);
		#region IChangedEventSource Members

		public event System.EventHandler Changed;


		protected virtual void OnChanged()
		{
			if(null==this.m_Container )
				m_Container.OnChildChanged(this,new ChangedEventArgs(this,null));

			if(null!=Changed)
				Changed(this,new ChangedEventArgs(this,null));
		}

		#endregion

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		public abstract object Clone();
	}
}
