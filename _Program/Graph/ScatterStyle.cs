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

using Altaxo.Graph.ScatterStyles;

namespace Altaxo.Graph
{
	namespace ScatterStyles
	{
		public enum Shape 
		{
			NoSymbol,
			Square,
			Circle,
			UpTriangle,
			DownTriangle,
			Diamond,
			CrossPlus,
			CrossTimes,
			Star,
			BarHorz,
			BarVert
		}

		public enum Style
		{
			Solid,
			Open,
			DotCenter,
			Hollow,
			Plus,
			Times,
			BarHorz,
			BarVert
		}

		[Flags]
		public enum DropLine
		{
			NoDrop=0,
			Top=1,
			Bottom=2,
			Left=4,
			Right=8,
			All=Top|Bottom|Left|Right
		}
	} // end of class ScatterStyles

	public class ScatterStyle : ICloneable
	{
		protected ScatterStyles.Shape m_Shape;
		protected ScatterStyles.Style m_Style;
		protected ScatterStyles.DropLine m_DropLine;
		protected PenHolder           m_Pen;
		protected float								m_SymbolSize;
		protected float								m_RelativePenWidth;

		// cached values:
		protected GraphicsPath m_Path;
		protected bool         m_bFillPath;
		protected BrushHolder  m_FillBrush;


		public ScatterStyle(ScatterStyle from)
		{
			this.m_Shape			= from.m_Shape;
			this.m_Style			= from.m_Style;
			this.m_DropLine   = from.m_DropLine;
			this.m_Pen				= null==from.m_Pen?null:(PenHolder)from.m_Pen.Clone();

			this.m_Path				= null==from.m_Path?null:(GraphicsPath)from.m_Path.Clone();
			this.m_bFillPath	= from.m_bFillPath;
			this.m_FillBrush	= null==from.m_FillBrush?null:(BrushHolder)from.m_FillBrush.Clone();
			this.m_SymbolSize = from.m_SymbolSize;
			this.m_RelativePenWidth = from.m_RelativePenWidth;
		}

		public ScatterStyle(ScatterStyles.Shape shape, ScatterStyles.Style style, float size, float penWidth, Color penColor)
		{
			m_Shape = shape;
			m_Style = style;
			m_DropLine = ScatterStyles.DropLine.NoDrop;
			m_Pen = new PenHolder(penColor,penWidth);
			m_SymbolSize = size;
			m_RelativePenWidth = penWidth/size;

			// Cached values
			SetCachedValues();
		}


		public ScatterStyle()
		{
			this.m_Shape = ScatterStyles.Shape.Square;
			this.m_Style = ScatterStyles.Style.Solid;
			this.m_DropLine = ScatterStyles.DropLine.NoDrop;
			this.m_Pen		= new PenHolder(Color.Black);
			this.m_SymbolSize = 8;
			this.m_RelativePenWidth = 0.1f;
			this.m_bFillPath = true; // since default is solid
			this.m_FillBrush = new BrushHolder(Color.Black);
			this.m_Path = GetPath(m_Shape,m_Style,m_SymbolSize);
		}


		public void SetToNextStyle(ScatterStyle template)
		{
			// first increase the shape value,
			// if this is not possible set shape to first shape, and increase the
			// style value


			if(System.Enum.IsDefined(typeof(ScatterStyles.Shape),1+(int)template.Shape))
			{
				Shape = (ScatterStyles.Shape)(1+(int)template.Shape);
			}
			else
			{
				Shape = ScatterStyles.Shape.Square;

				if(System.Enum.IsDefined(typeof(ScatterStyles.Style),1+(int)template.Style))
					Style = (ScatterStyles.Style)(1+(int)template.Style);
				else
					Style = ScatterStyles.Style.Solid;
			}

		}

		public ScatterStyles.Shape Shape
		{
			get { return this.m_Shape; }
			set
			{
				if(value!=this.m_Shape)
				{
					this.m_Shape = value;
					
					// ensure that a pen is set if Shape is other than nosymbol
					if(value!=ScatterStyles.Shape.NoSymbol && null==this.m_Pen)
						m_Pen = new PenHolder(Color.Black);

					SetCachedValues();
				}
			}
		}





		public ScatterStyles.Style Style
		{
			get { return this.m_Style; }
			set 
			{
				if(value!=this.m_Style)
				{
					this.m_Style = value;
					SetCachedValues();
				}
			}
		}

		public ScatterStyles.DropLine DropLine
		{
			get { return m_DropLine; }
			set 
			{
				m_DropLine = value;
			}
		}

		public PenHolder Pen
		{
			get { return this.m_Pen; }
			set
			{
				// ensure pen can be only set to null if NoSymbol
				if(value!=null || ScatterStyles.Shape.NoSymbol==this.m_Shape)
					m_Pen = null==value?null:(PenHolder)value.Clone();
			}
		}


		public System.Drawing.Color Color
		{
			get { return this.m_Pen.Color; }
			set 
			{
				this.m_Pen.Color = value;
				this.m_FillBrush.SetSolidBrush( value );
			}
		}

		public float SymbolSize
		{
			get { return m_SymbolSize; }
			set
			{
				if(value!=m_SymbolSize)
				{
					m_SymbolSize = value;
					m_Path = GetPath(this.m_Shape,this.m_Style,this.m_SymbolSize);
					m_Pen.Width = m_SymbolSize*m_RelativePenWidth;
				}
			}
		}


		protected void SetCachedValues()
		{
			m_Path = GetPath(this.m_Shape,this.m_Style,this.m_SymbolSize);

			m_bFillPath = m_Style==ScatterStyles.Style.Solid || m_Style==ScatterStyles.Style.Open || m_Style==ScatterStyles.Style.DotCenter;
		
			if(this.m_Style!=ScatterStyles.Style.Solid)
				m_FillBrush = new BrushHolder(Color.White);
			else if(this.m_Pen.PenType==PenType.SolidColor)
				m_FillBrush = new BrushHolder(m_Pen.Color);
			else
				m_FillBrush = new BrushHolder(m_Pen.BrushHolder);
		}


		public void Paint(Graphics g)
		{
		  if(m_bFillPath)
				g.FillPath(m_FillBrush,m_Path);

			g.DrawPath(m_Pen,m_Path);
		}

		public object Clone()
		{
			return new ScatterStyle(this);
		}
	
		public static GraphicsPath GetPath(ScatterStyles.Shape sh, ScatterStyles.Style st, float size)
		{
			float sizeh = size/2;
			GraphicsPath gp = new GraphicsPath();


			switch(sh)
			{
				case Shape.Square:
					gp.AddRectangle(new RectangleF(-sizeh,-sizeh,size,size));
					break;
				case Shape.Circle:
					gp.AddEllipse(-sizeh,-sizeh,size,size);
					break;
				case Shape.UpTriangle:
					gp.AddLine(0,-sizeh,0.3301270189f*size,0.5f*sizeh);
					gp.AddLine(0.43301270189f*size,0.5f*sizeh,-0.43301270189f*size,0.5f*sizeh);
					gp.CloseFigure();
					break;
				case Shape.DownTriangle:
					gp.AddLine(-0.43301270189f*sizeh,-0.5f*sizeh,0.43301270189f*size,-0.5f*sizeh);
					gp.AddLine(0.43301270189f*size,-0.5f*sizeh,0,sizeh);
					gp.CloseFigure();
					break;
				case Shape.Diamond:
					gp.AddLine(0,-sizeh,sizeh,0);
					gp.AddLine(sizeh,0,0,sizeh);
					gp.AddLine(0,sizeh,-sizeh,0);
					gp.CloseFigure();
					break;
				case Shape.CrossPlus:
					gp.AddLine(-sizeh,0,sizeh,0);
					gp.StartFigure();
					gp.AddLine(0,sizeh,0,-sizeh);
					gp.StartFigure();
					break;
				case Shape.CrossTimes:
					gp.AddLine(-sizeh,-sizeh,sizeh,sizeh);
					gp.StartFigure();
					gp.AddLine(-sizeh,sizeh,sizeh,-sizeh);
					gp.StartFigure();
					break;
				case Shape.Star:
					gp.AddLine(-sizeh,0,sizeh,0);
					gp.StartFigure();
					gp.AddLine(0,sizeh,0,-sizeh);
					gp.StartFigure();
					gp.AddLine(-sizeh,-sizeh,sizeh,sizeh);
					gp.StartFigure();
					gp.AddLine(-sizeh,sizeh,sizeh,-sizeh);
					gp.StartFigure();
					break;
				case Shape.BarHorz:
					gp.AddLine(-sizeh,0,sizeh,0);
					gp.StartFigure();
					break;
				case Shape.BarVert:
					gp.AddLine(0,-sizeh,0,sizeh);
					gp.StartFigure();
					break;
			}

			switch(st)
			{
				case Style.DotCenter:
					gp.AddEllipse(-0.125f*sizeh,-0.125f*sizeh,0.125f*size,0.125f*size);
					break;
				case Style.Plus:
					gp.AddLine(-sizeh,0,sizeh,0);
					gp.StartFigure();
					gp.AddLine(0,sizeh,0,-sizeh);
					gp.StartFigure();
					break;
				case Style.Times:
					gp.AddLine(-sizeh,-sizeh,sizeh,sizeh);
					gp.StartFigure();
					gp.AddLine(-sizeh,sizeh,sizeh,-sizeh);
					gp.StartFigure();
					break;
				case Style.BarHorz:
					gp.AddLine(-sizeh,0,sizeh,0);
					gp.StartFigure();
					break;
				case Style.BarVert:
					gp.AddLine(0,-sizeh,0,sizeh);
					gp.StartFigure();
					break;
			}
			return gp;
		}

	
	}
}
