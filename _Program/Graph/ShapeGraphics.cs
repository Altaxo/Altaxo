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
	public abstract class ShapeGraphic : GraphObject
	{

		protected float m_lineWidth = 1;
		protected Color m_lineColor  = Color.Black;
		protected Color m_fillColor  = Color.White;
		protected bool m_fill  = false;

		public virtual float LineWidth
		{ 
			get
			{
				return m_lineWidth;
			}
			set
			{
				if(value > 0)
					m_lineWidth = value;
				else
					throw new ArgumentOutOfRangeException("LineWidth", "Line Width must be > 0");
			}
		}

		public virtual Color LineColor
		{
			get
			{
				return m_lineColor;
			}
			set
			{
				m_lineColor = value;
			}
		}

		public virtual  bool Fill
		{
			get
			{
				return m_fill;
			}
			set
			{
				m_fill = value;
			}
		}
		public virtual Color FillColor
		{
			get
			{
				return m_fillColor;
			}
			set
			{
				m_fillColor = value;
			}
		}
	} // 	End Class



	public class LineGraphic : ShapeGraphic
	{

#region "Constructors"
		public LineGraphic()
		{
		}

		public LineGraphic(PointF startPosition)
		{
			this.SetStartPosition(startPosition);
		}

		public LineGraphic(float posX, float posY)
			: this(new PointF(posX, posY))
		{
		}

			public LineGraphic(PointF startPosition, PointF endPosition)
				:
				this(startPosition)
			{
				this.SetEndPosition(endPosition);
				this.AutoSize = false;
			}


		public LineGraphic(float startX, float startY, PointF endPosition)
			:
			this(new PointF(startX, startY), endPosition)
		{
		}

		public LineGraphic(float startX, float startY, float endX, float endY)
			:
			this(new PointF(startX, startY), new PointF(endX, endY))
		{
		}

		public LineGraphic(PointF startPosition, PointF endPosition, float lineWidth, Color lineColor)
			:
			this(startPosition)
		{
			this.SetEndPosition(endPosition);
			this.LineWidth = lineWidth;
			this.LineColor = lineColor;
			this.AutoSize = false;
		}

		public LineGraphic(float startX, float startY, float endX, float endY, float lineWidth, Color lineColor)
			:
			this(new PointF(startX, startY), new PointF(endX, endY))
		{
			this.LineWidth = lineWidth;
			this.LineColor = lineColor;
			this.AutoSize = false;
		}

#endregion



		public override GraphicsPath HitTest(PointF pt)
		{
			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();
			Pen myPen = new Pen(this.LineColor, this.LineWidth);
			gp.AddLine(0, 0, Width, Height);
			myMatrix.Translate(X, Y);
			myMatrix.Rotate(this.Rotation);
			gp.Transform(myMatrix);
			return gp.IsOutlineVisible(pt, myPen) ? gp : null;
		}


		public PointF GetStartPosition()
		{
			return this.GetPosition();
		}

		public void SetStartPosition(PointF Value)
		{
			this.SetPosition(Value);
		}


		public PointF GetEndPosition()
		{
			PointF endPosition = new PointF(this.m_Position.X, this.m_Position.Y);
			endPosition.X += this.Size.Width;
			endPosition.Y += this.Size.Height;
			return endPosition;
		}

		public void SetEndPosition(PointF Value)
		{
			SizeF siz = new SizeF(
														Value.X - this.m_Position.X,
														Value.Y - this.m_Position.Y);
			SetSize(siz);
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			g.TranslateTransform(X, Y);
			g.RotateTransform(this.m_Rotation);
			Pen myPen = new Pen(this.LineColor, this.LineWidth);
			g.DrawLine(myPen, X, Y, X + Width, Y + Height);
			g.Restore(gs);
		}

	} // End Class


	public class RectangleGraphic : ShapeGraphic
	{

#region "Constructors"
		public RectangleGraphic()
		{
		}

		public RectangleGraphic( PointF graphicPosition)
			:
			this()
		{
			this.SetPosition(graphicPosition);
		}

		public RectangleGraphic( float posX, float posY)
			:
			this(new PointF(posX, posY))
		{
		}
			

			public RectangleGraphic( PointF graphicPosition , SizeF graphicSize)
				:
				this(graphicPosition)
			{
				this.SetSize(graphicSize);
				this.AutoSize = false;
			}

		public RectangleGraphic( float posX , float posY, SizeF graphicSize)
			:
			this(new PointF(posX, posY), graphicSize)
		{
		}

		public RectangleGraphic( float posX , float posY , float width , float height )
			:
			this(new PointF(posX, posY), new SizeF(width, height))
		{
		}

		public RectangleGraphic( PointF graphicPosition, float Rotation)
			:
			this()
		{

			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		public RectangleGraphic( float posX , float posY , float Rotation )
			:
			this(new PointF(posX, posY), Rotation)
		{
		}

		public RectangleGraphic( PointF graphicPosition , SizeF graphicSize , float Rotation )
			:
			this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}
		
		public RectangleGraphic( float posX , float posY , SizeF graphicSize , float Rotation)
			:
			this(new PointF(posX, posY), graphicSize, Rotation)
		{
		}

		public RectangleGraphic( float posX , float posY , float width , float height , float Rotation)
			:
			this(new PointF(posX, posY), new SizeF(width, height), Rotation)
		{
		}

#endregion


		public override void Paint( Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			g.TranslateTransform(X,Y);
			if(m_Rotation !=- 0)
				g.RotateTransform(m_Rotation);
			RectangleF rect = new RectangleF(X, Y, Width, Height);
			if( this.Fill)
			{
				g.FillRectangle(new SolidBrush(this.FillColor), rect);
			}
			Pen myPen = new Pen(this.LineColor, this.LineWidth);
			g.DrawRectangle(myPen, rect.X,rect.Y,rect.Width,rect.Height);
			g.Restore(gs);
		}
		
	} // End Class

	public class EllipseGraphic : ShapeGraphic
	{
#region "Constructors"
	
		public EllipseGraphic()
		{
		}

		public EllipseGraphic( PointF graphicPosition)
			:
			this()
		{
			this.SetPosition(graphicPosition);
		}
		public EllipseGraphic( float posX, float posY)
			:
			this(new PointF(posX, posY))
		{
		}

		public EllipseGraphic( PointF graphicPosition, SizeF graphicSize)
			:
			this(graphicPosition)
		{

			this.SetSize(graphicSize);
			this.AutoSize = false;
		}

		public EllipseGraphic( float posX, float posY , SizeF graphicSize)
			:
			this(new PointF(posX, posY), graphicSize)
		{
		}

		public EllipseGraphic( float posX , float posY , float width , float height )
			:
			this(new PointF(posX, posY), new SizeF(width, height))
		{
		}
	
		public EllipseGraphic( PointF graphicPosition, float Rotation)
			:
			this()
		{
			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		public EllipseGraphic( float posX , float posY , float Rotation)
			:
			this(new PointF(posX, posY), Rotation)
		{
		}

		public EllipseGraphic( PointF graphicPosition , SizeF graphicSize, float Rotation)
			:
			this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}

		public EllipseGraphic( float posX , float posY , SizeF graphicSize , float Rotation)
			:
			this(new PointF(posX, posY), graphicSize, Rotation)
		{
		}

		public EllipseGraphic( float posX, float posY, float width, float	height, float Rotation)
			:
			this(new PointF(posX, posY), new SizeF(width, height), Rotation)
		{
		}

#endregion

		public override void Paint( Graphics g, object obj )
		{
			GraphicsState gs = g.Save();
			g.TranslateTransform(X,Y);
			if( m_Rotation != 0)
				g.RotateTransform(m_Rotation);

					RectangleF rect = new RectangleF(X, Y, Width, Height);
			if( this.Fill )
				g.FillEllipse(new SolidBrush(this.FillColor), rect);

			Pen myPen = new Pen(this.LineColor, this.LineWidth);
			g.DrawEllipse(myPen, rect);
			g.Restore(gs);
		}
	} // end class

} // end Namespace
