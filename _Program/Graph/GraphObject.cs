using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo
{
	/// <summary>
	/// Summary description for GraphObject.
	/// </summary>
	public abstract class GraphObject
	{
		protected PointF m_Position = new PointF(0, 0);
		protected SizeF m_Size = new SizeF(0, 0);
		protected float m_Rotation = 0;
		protected bool m_AutoSize = true;
		protected GraphObjectCollection m_Container=null;




		protected GraphObject()
		{
		}
		protected GraphObject(PointF graphicPosition)
		{
			SetPosition(graphicPosition);
		}
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
			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();

			gp.AddRectangle(new RectangleF(X, Y, Width, Height));
			if(this.Rotation != 0)
			{
				myMatrix.RotateAt(this.Rotation, new PointF(X, Y), MatrixOrder.Append);
			}

			gp.Transform(myMatrix);
			return gp.IsVisible(pt) ? gp : null;
		}

		public virtual bool HitTest(RectangleF rect)
		{
			// is this object contained within the supplied rectangle

			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();


			gp.AddRectangle(new RectangleF(X, Y, Width, Height));
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
				return m_Size.Height;
			}
			set
			{
				m_Size.Height = value;
			}
		}
		public virtual float Width
		{
			get
			{
				return m_Size.Width;
			}
			set
			{
				m_Size.Width = value;
			}
		}

		public virtual void SetSize(SizeF Value)
		{
			m_Size = Value;
		}
		public virtual SizeF GetSize()
		{
			return this.m_Size;
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

		public abstract void Paint(Graphics g);


	}
}
