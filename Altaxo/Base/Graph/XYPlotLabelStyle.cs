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
using Altaxo.Graph.XYPlotScatterStyles;
using Altaxo.Serialization;

namespace Altaxo.Graph
{

	public class XYPlotLabelStyle :
		ICloneable,
		Main.IChangedEventSource,
		System.Runtime.Serialization.IDeserializationCallback, 
		Main.IChildChangedEventSink
	{
		/// <summary>The font of the label.</summary>
		protected System.Drawing.Font m_Font;

		/// <summary>The brush for the label.</summary>
		protected BrushHolder  m_Brush;
	
		/// <summary>The x offset in EM units.</summary>
		protected double m_XOffset;

		/// <summary>The y offset in EM units.</summary>
		protected double m_YOffset;

		/// <summary>The rotation of the label.</summary>
		protected double m_Rotation;

		/// <summary>If true, the label is painted on a white background.</summary>
		protected bool m_WhiteOut;

		/// <summary>If true, the label is attached to one of the four edges of the layer.</summary>
		protected bool m_AttachToAxis;

		/// <summary>The axis where the label is attached to (if it is attached).</summary>
		protected Graph.EdgeType m_AttachedAxis;

		// cached values:
		protected System.Drawing.StringFormat m_CachedStringFormat;


		#region Serialization
	

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLabelStyle),0)]
			public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
				info.AddValue("Font",s.m_Font);  
				info.AddValue("Brush",s.m_Brush);  
				info.AddValue("XOffset",s.m_XOffset);
				info.AddValue("YOffset",s.m_YOffset);
				info.AddValue("Rotation",s.m_Rotation);
				info.AddValue("HorizontalAlignment",s.HorizontalAlignment);
				info.AddValue("VerticalAlignment",s.VerticalAlignment);
				info.AddValue("AttachToAxis",s.m_AttachToAxis);
				info.AddValue("AttachedAxis",s.m_AttachedAxis);
				info.AddValue("WhiteOut",s.m_WhiteOut);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				XYPlotLabelStyle s = null!=o ? (XYPlotLabelStyle)o : new XYPlotLabelStyle();

				s.m_Font = (Font)info.GetValue("Font",s);  
				s.m_Brush = (BrushHolder)info.GetValue("Brush",s);
				s.m_XOffset = info.GetDouble("XOffset");
				s.m_YOffset = info.GetDouble("YOffset");
				s.m_Rotation = info.GetDouble("Rotation");
				s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment",typeof(System.Drawing.StringAlignment));
				s.VerticalAlignment   = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment",typeof(System.Drawing.StringAlignment));
				s.m_AttachToAxis = info.GetBoolean("AttachToAxis");
				s.m_AttachedAxis = (EdgeType)info.GetEnum("AttachedAxis",typeof(EdgeType));
				s.m_WhiteOut = info.GetBoolean("WhiteOut");

				// restore the cached values
				s.SetCachedValues();
				s.CreateEventChain();

				return s;
			}
		}
		/// <summary>
		/// Finale measures after deserialization of the linear axis.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// restore the cached values
			SetCachedValues();
			CreateEventChain();
		}
		#endregion


		public XYPlotLabelStyle(XYPlotLabelStyle from)
		{
			this.m_Font       = (Font)from.m_Font.Clone();
			this.m_Brush			= (BrushHolder)from.m_Brush.Clone();
			this.m_XOffset    = from.m_XOffset;
			this.m_YOffset    = from.m_YOffset;
			this.m_Rotation   = from.m_Rotation;
			this.m_WhiteOut   = from.m_WhiteOut;
			this.m_CachedStringFormat = (System.Drawing.StringFormat)from.m_CachedStringFormat.Clone();
			this.m_AttachToAxis        = from.m_AttachToAxis;
			this.m_AttachedAxis        = from.m_AttachedAxis;

			CreateEventChain();
		}

		public XYPlotLabelStyle()
		{
			this.m_Font = new Font(System.Drawing.FontFamily.GenericSansSerif,8,GraphicsUnit.World);
			this.m_Brush = new BrushHolder(Color.Black);
			this.m_XOffset = 0;
			this.m_YOffset = 0;
			this.m_Rotation = 0;
			this.m_WhiteOut = false;
			this.m_CachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
			this.m_CachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
			this.m_CachedStringFormat.LineAlignment   = System.Drawing.StringAlignment.Center;
			this.m_AttachToAxis = false;
			this.m_AttachedAxis = EdgeType.Bottom;

			CreateEventChain();
		}

		protected void CreateEventChain()
		{
			// if we change from color to a brush, add the brush events here
		}


		/// <summary>The font of the label.</summary>
		public Font Font
		{
			get { return m_Font; }
			set
			{
				m_Font = value;
				OnChanged();
			}
		}

		/// <summary>The font size of the label.</summary>
		public float FontSize
		{
			get { return m_Font.Size; }
			set
			{
				float oldValue = FontSize;
				float newValue = Math.Max(0,value);

				if(newValue != oldValue)
				{
					Font oldFont = m_Font;
					m_Font = new Font(oldFont.FontFamily.Name,newValue,oldFont.Style,GraphicsUnit.World);
					oldFont.Dispose();

					OnChanged(); // Fire Changed event
				}
			}
		}

		/// <summary>The brush color.</summary>
		public System.Drawing.Color Color
		{
			get { return this.m_Brush.Color;; }
			set 
			{
				Color oldColor = this.Color;
				if(value!=oldColor)
				{
					this.m_Brush.SetSolidBrush( value );
					OnChanged(); // Fire Changed event
				}
			}
		}

		/// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double XOffset
		{
			get { return this.m_XOffset; }
			set
			{
				double oldValue = this.m_XOffset;
				this.m_XOffset = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double YOffset
		{
			get { return this.m_YOffset; }
			set
			{
				double oldValue = this.m_YOffset;
				this.m_YOffset = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>The angle of the label.</summary>
		public double Rotation
		{
			get { return this.m_Rotation; }
			set
			{
				double oldValue = this.m_Rotation;
				this.m_Rotation = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>If true, the label is painted on a white background.</summary>
		public bool WhiteOut
		{
			get { return this.m_WhiteOut; }
			set
			{
				bool oldValue = this.m_WhiteOut;
				this.m_WhiteOut = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>Horizontal alignment of the label.</summary>
		public System.Drawing.StringAlignment HorizontalAlignment
		{
			get { return this.m_CachedStringFormat.Alignment; }
			set
			{
				System.Drawing.StringAlignment oldValue = this.HorizontalAlignment;
				this.m_CachedStringFormat.Alignment = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public System.Drawing.StringAlignment VerticalAlignment
		{
			get { return this.m_CachedStringFormat.LineAlignment; }
			set
			{
				System.Drawing.StringAlignment oldValue = this.VerticalAlignment;
				this.m_CachedStringFormat.LineAlignment = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>If true, the label is attached to one of the 4 axes.</summary>
		public bool AttachToAxis
		{
			get { return this.m_AttachToAxis; }
			set
			{
				bool oldValue = this.m_AttachToAxis;
				this.m_AttachToAxis = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}


		/// <summary>If axis the label is attached to if the value of <see>AttachToAxis</see> is true.</summary>
		public EdgeType AttachedAxis
		{
			get { return this.m_AttachedAxis; }
			set
			{
				EdgeType oldValue = this.m_AttachedAxis;
				this.m_AttachedAxis = value;
				if(value!=oldValue)
				{
					OnChanged();
				}
			}
		}


		protected void SetCachedValues()
		{
		}


		public void Paint(Graphics g, string label)
		{
			float fontSize = this.FontSize;
			g.DrawString(label,m_Font,m_Brush,(float)m_XOffset*fontSize,(float)m_YOffset*fontSize,m_CachedStringFormat);
		}

		public void Paint(Graphics g,
			Graph.XYPlotLayer layer,
			PlotRangeList rangeList,
			PointF[] ptArray,
			Altaxo.Data.IReadableColumn labelColumn)
		{
      // save the graphics stat since we have to translate the origin
      System.Drawing.Drawing2D.GraphicsState gs = g.Save();


      float xpos=0, ypos=0;
      float xdiff,ydiff;
      for(int r=0;r<rangeList.Count;r++)
      {
        int lower = rangeList[r].LowerBound;
        int upper = rangeList[r].UpperBound;
        int offset = rangeList[r].OffsetToOriginal;
        for(int j=lower;j<upper;j++)
        {
          string label = labelColumn[j+offset].ToString();
          if(label==null || label==string.Empty)
            continue;
          
          xdiff = ptArray[j].X - xpos;
          ydiff = ptArray[j].Y - ypos;
          xpos = ptArray[j].X;
          ypos = ptArray[j].Y;
          g.TranslateTransform(xdiff,ydiff);
          this.Paint(g,label);
          } // end for
      }

      g.Restore(gs); // Restore the graphics state
		}


		public object Clone()
		{
			return new XYPlotLabelStyle(this);
		}
	
		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this,new EventArgs());
		}

		#endregion

		#region IChildChangedEventSink Members

		public void EhChildChanged(object child, EventArgs e)
		{
			OnChildChanged(child, e);
		}

		public void OnChildChanged(object child, EventArgs e)
		{
			if(null!=Changed)
				Changed(this,e);
		}

		#endregion
	}
}
