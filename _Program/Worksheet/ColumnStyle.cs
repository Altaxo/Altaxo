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

namespace Altaxo.Worksheet
{
	/// <summary>
	/// Altaxo.Worksheet.ColumnStyle provides the data for visualization of the column
	/// data, for instance m_Width and color of columns
	/// additionally, it is responsible for the conversion of data to text and vice versa
	/// </summary>
	[SerializationSurrogate(0,typeof(ColumnStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public abstract class ColumnStyle : System.ICloneable, System.Runtime.Serialization.IDeserializationCallback // pendant to DataGridColumnStyle
	{
		protected int m_Size=40;
		protected Graph.PenHolder m_CellPen = new Graph.PenHolder(Color.Blue,1);
		protected StringFormat m_TextFormat = new StringFormat();
		protected Font m_TextFont = new Font("Arial",8);								
		protected Graph.BrushHolder m_TextBrush = new Graph.BrushHolder(Color.Black);
		protected Graph.BrushHolder m_SelectedTextBrush = new Graph.BrushHolder(Color.White);
		protected Graph.BrushHolder m_BackgroundBrush = new Graph.BrushHolder(Color.White);
		protected Graph.BrushHolder m_SelectedBackgroundBrush = new Graph.BrushHolder(Color.DarkGray);

		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
				public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				ColumnStyle s = (ColumnStyle)obj;
				info.AddValue("Size",(float)s.m_Size);
				info.AddValue("Pen",s.m_CellPen);
				info.AddValue("TextBrush",s.m_TextBrush);
				info.AddValue("SelTextBrush",s.m_SelectedTextBrush);
				info.AddValue("BkgBrush",s.m_BackgroundBrush);
				info.AddValue("SelBkgBrush",s.m_SelectedBackgroundBrush);
				info.AddValue("Alignment",s.m_TextFormat.Alignment);
				

				info.AddValue("Font",s.m_TextFont); // Serialization is possible in NET1SP2, but deserialization fails (Tested with SOAP formatter)
				
				/*
				string fontname = s.m_TextFont.Name;
				float  fontsize = s.m_TextFont.Size;
				FontStyle fontstyle = s.m_TextFont.Style;
				GraphicsUnit fontunit = s.m_TextFont.Unit;
				info.AddValue("FontName",fontname);
				info.AddValue("FontSize",fontsize);
				info.AddValue("FontStyle",fontstyle);
				info.AddValue("FontUnit",fontunit);
				*/

			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				ColumnStyle s = (ColumnStyle)obj;

				//s.m_CellPen = new Graph.PenHolder(Color.Blue, 1, false);
				//s.m_TextFormat = new StringFormat();
				//s.m_TextFont = new Font("Arial",8);								
				//s.m_TextBrush = new Graph.BrushHolder(new SolidBrush(Color.Black));
				//s.m_SelectedTextBrush = new Graph.BrushHolder(new SolidBrush(Color.White));
				//s.m_BackgroundBrush = new Graph.BrushHolder(new SolidBrush(Color.White));
				//s.m_SelectedBackgroundBrush = new Graph.BrushHolder(new SolidBrush(Color.DarkGray));

				
				s.m_Size = (int)info.GetSingle("Size");
				s.m_CellPen = (Graph.PenHolder)info.GetValue("Pen",typeof(Graph.PenHolder));
				s.m_TextBrush = (Graph.BrushHolder)info.GetValue("TextBrush",typeof(Graph.BrushHolder));
				s.m_SelectedTextBrush = (Graph.BrushHolder)info.GetValue("SelTextBrush",typeof(Graph.BrushHolder));
				s.m_BackgroundBrush = (Graph.BrushHolder)info.GetValue("BkgBrush",typeof(Graph.BrushHolder));
				s.m_SelectedBackgroundBrush = (Graph.BrushHolder)info.GetValue("SelBkgBrush",typeof(Graph.BrushHolder));
				s.m_TextFormat = new StringFormat();
				s.m_TextFormat.Alignment = (StringAlignment)info.GetValue("Alignment",typeof(StringAlignment));


				// Deserialising a font with SoapFormatter raises an error at least in Net1SP2, so I had to circuumvent this
				s.m_TextFont = (Font)info.GetValue("Font",typeof(Font)); 
			//	s.m_TextFont = new Font("Arial",8);								


				/*																				
				string fontname;
				float  fontsize;
				FontStyle fontstyle;
				GraphicsUnit fontunit;
				fontname = info.GetString("FontName");
				fontsize = info.GetSingle("FontSize");
				fontstyle = (FontStyle)info.GetValue("FontStyle",typeof(FontStyle));
				fontunit = (GraphicsUnit)info.GetValue("FontUnit",typeof(GraphicsUnit));
				*/


				return s;
			}
		}

		public virtual void OnDeserialization(object obj)
		{
		}

		#endregion

		public ColumnStyle()
		{
		}

		public ColumnStyle(ColumnStyle s)
		{
			m_Size = s.m_Size;
			m_CellPen = (Graph.PenHolder)s.m_CellPen.Clone();
			m_TextFormat = (StringFormat)s.m_TextFormat.Clone();
			m_TextFont = (Font)s.m_TextFont.Clone();
			m_TextBrush = (Graph.BrushHolder)s.m_TextBrush.Clone();
			m_SelectedBackgroundBrush = (Graph.BrushHolder)s.m_SelectedBackgroundBrush.Clone();
			m_SelectedTextBrush = (Graph.BrushHolder)s.m_SelectedTextBrush.Clone();
		}

		public int Width
		{
			get
			{
				return m_Size;
			}
			set
			{
				m_Size=value;
			}	
		}
	
	
	
	public abstract object Clone();
	public abstract void Paint(Graphics dc, Rectangle cell, int nRow, Altaxo.Data.DataColumn data, bool bSelected);
	public abstract string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data);
	public abstract void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data);
		
	} // end of class Altaxo.Worksheet.ColumnStyle







} // end of namespace Altaxo


