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

	public enum BrushType { SolidBrush, HatchBrush, TextureBrush, LinearGradientBrush, PathGradientBrush };
	
	/// <summary>
	/// BrushSubstitute holds all information neccessary to create a brush
	/// of any kind without allocating resources, so this class
	/// can be made serializable
	/// </summary>
	[Altaxo.Serialization.SerializationSurrogate(0,typeof(BrushHolder.BrushHolderSurrogate0))]
	[Altaxo.Serialization.SerializationVersion(0)]
	public class BrushHolder : System.ICloneable, System.IDisposable
	{

		protected BrushType m_BrushType; // Type of the brush
		protected bool      m_CachedMode; // Is the brushed cached inside this object
		protected Brush     m_Brush;      // this is the cached brush object

		protected Color			m_ForeColor; // Color of the brush
		protected Color			m_BackColor; // Backcolor of brush, f.i.f. HatchStyle brushes
		protected HatchStyle	m_HatchStyle; // für HatchBrush
		protected Image			m_Image; // für Texturebrush
		protected Matrix		m_Matrix; // für TextureBrush
		protected WrapMode	m_WrapMode; // für TextureBrush und LinearGradientBrush
		protected PointF		m_Point1;
		protected PointF		m_Point2;
		protected float			m_Float1;
		protected bool			m_Bool1;

		#region "Serialization"
		public class BrushHolderSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(	object obj,	
				System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context	)
			{
				BrushHolder s = (BrushHolder)obj;
				info.AddValue("Type",s.m_BrushType);
				switch(s.m_BrushType)
				{
					case BrushType.SolidBrush:
						info.AddValue("ForeColor",s.m_ForeColor);
						break;
					case BrushType.HatchBrush:
						info.AddValue("ForeColor",s.m_ForeColor);
						info.AddValue("BackColor",s.m_BackColor);
						info.AddValue("HatchStyle",s.m_HatchStyle);
						break;
				} // end of switch
			}
			public object SetObjectData(
				object obj,
				System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context,
				System.Runtime.Serialization.ISurrogateSelector selector
				)
			{
				BrushHolder s = (BrushHolder)obj;
				s.m_BrushType  = (BrushType)info.GetValue("Type",typeof(BrushType));
				switch(s.m_BrushType)
				{
					case BrushType.SolidBrush:
						s.m_ForeColor = (Color)info.GetValue("ForeColor",typeof(Color));
						break;
					case BrushType.HatchBrush:
						s.m_ForeColor = (Color)info.GetValue("ForeColor",typeof(Color));
						s.m_BackColor = (Color)info.GetValue("BackColor",typeof(Color));
						break;
				}
				return s;
			} // end of SetObjectData
		} // end of BrushHolderSurrogate0
		#endregion

		public BrushHolder(BrushHolder bh)
		{
		m_BrushType		= bh.m_BrushType; // Type of the brush
		m_CachedMode	= bh.m_CachedMode;; // Is the brushed cached inside this object
		m_Brush				= null==bh.m_Brush ? null : (Brush)bh.m_Brush.Clone();      // this is the cached brush object
		m_ForeColor		= bh.m_ForeColor; // Color of the brush
		m_BackColor		= bh.m_BackColor; // Backcolor of brush, f.i.f. HatchStyle brushes
		m_HatchStyle	= bh.m_HatchStyle; // für HatchBrush
		m_Image				= null==bh.m_Image ? null : (Image)bh.m_Image.Clone(); // für Texturebrush
		m_Matrix			= null==bh.m_Matrix ? null : (Matrix)bh.m_Matrix.Clone(); // für TextureBrush
		m_WrapMode		= bh.m_WrapMode; // für TextureBrush und LinearGradientBrush
		m_Point1			= bh.m_Point1;
		m_Point2			= bh.m_Point2;
		m_Float1			= bh.m_Float1;
		m_Bool1				= bh.m_Bool1;
		}

		public BrushHolder(Color c)
		: this(c,false)
		{
		}

		public BrushHolder(Color c, bool bCacheMode)
		{
			this.m_CachedMode = bCacheMode;
			this.m_BrushType = BrushType.SolidBrush;
			this.m_ForeColor = c;

			if(bCacheMode)
				_SetBrushVariable( new SolidBrush(c) );
		}

		
		public BrushHolder(Brush br, bool cacheMode)
		{
			m_CachedMode = cacheMode;
			this.Brush = br;
		}


		public static implicit operator System.Drawing.Brush(BrushHolder bh)
		{
			bh.Cached = true; // if we use implicit conversion, we set cached mode to true because I suppose this conversion is used in functions
			return bh.Brush;
		}

		public bool Cached
		{
			get { return this.m_CachedMode; }
			set 
			{
				if(this.m_CachedMode != value) // only if the mode changed
				{
					// if forced to cache mode, create and set the pen variable
					_SetBrushVariable( value ? this.Brush : null );
					m_CachedMode = value;
				}
			}
		}

		public BrushType BrushType
		{
			get { return this.m_BrushType; }
		}

		public Color Color
		{
			get { return m_ForeColor; }
		}

		public Brush Brush
		{
			get 
			{
				if(this.m_CachedMode)
				{
					return m_Brush;
				}
				else
				{
					switch(m_BrushType)
					{
						case BrushType.SolidBrush:
							return new SolidBrush(m_ForeColor);
						case BrushType.HatchBrush:
							return new HatchBrush(m_HatchStyle,m_ForeColor,m_BackColor);
						default:
							return null;
					} // end of switch
				}			
			} // end of get
			set
			{
				if(value is SolidBrush)
				{
					m_BrushType = BrushType.SolidBrush;
					m_ForeColor = ((SolidBrush)value).Color;
				}
				else if(value is HatchBrush)
				{
					m_BrushType = BrushType.HatchBrush;
					m_ForeColor = ((HatchBrush)value).ForegroundColor;
					m_BackColor = ((HatchBrush)value).BackgroundColor;
					m_HatchStyle = ((HatchBrush)value).HatchStyle;
				}
				
				_SetBrushVariable(m_CachedMode ? (Brush)value.Clone() : null);
			} // end of set
		} // end of prop. Brush



		public void SetSolidBrush(Color c)
		{
			m_BrushType = BrushType.SolidBrush;
			m_ForeColor     = c;

			if(m_CachedMode)
				_SetBrushVariable( new SolidBrush(c) );
		}

		public void SetHatchBrush(HatchStyle hs, Color fc)
		{
			SetHatchBrush(hs,fc,Color.Black);
		}

		public void SetHatchBrush(HatchStyle hs, Color fc, Color bc)
		{
			m_BrushType = BrushType.HatchBrush;
			m_ForeColor = fc;
			m_BackColor = bc;

			if(m_CachedMode)
				_SetBrushVariable(new HatchBrush(hs,fc,bc) );
		}

		protected void _SetBrushVariable(Brush br)
		{
			if(null!=m_Brush)
				m_Brush.Dispose();

			m_Brush = br;
		}
		
		public object Clone()
		{
			return new BrushHolder(this);
		}

		public void Dispose()
		{
			if(null!=m_Brush)
				m_Brush.Dispose();
			m_Brush = null;
		}
	} // end of class BrushHolder
} // end of namespace
