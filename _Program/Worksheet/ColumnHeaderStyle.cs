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
	[SerializationSurrogate(0,typeof(ColumnHeaderStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class ColumnHeaderStyle : ColumnStyle
	{
		#region Serialization
			public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
			{
				public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
				{
					System.Runtime.Serialization.ISurrogateSelector ss;
					System.Runtime.Serialization.ISerializationSurrogate surr =
						App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
	
					surr.GetObjectData(obj,info,context); // stream the data of the base object
				}
				public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
				{
					System.Runtime.Serialization.ISurrogateSelector ss;
					System.Runtime.Serialization.ISerializationSurrogate surr =
						App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
					return surr.SetObjectData(obj,info,context,selector);
				}
			}

			public override void OnDeserialization(object obj)
			{
				base.OnDeserialization(obj);
			}	
			#endregion


		public int Height
		{
			get
			{
				return m_Size;
			}
			set
			{
				m_Size = value;
			}	
		}

		public ColumnHeaderStyle()
		{
			m_TextFormat.Alignment=StringAlignment.Center;
			m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
		}

		public ColumnHeaderStyle(ColumnHeaderStyle chs)
			: base(chs)
		{
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			m_CellPen.Cached = true;
			dc.DrawRectangle(m_CellPen.Pen,cellRectangle);
		
			if(bSelected)
				dc.FillRectangle(m_SelectedBackgroundBrush,cellRectangle);
		
			string myString = string.Format("{0} ({1}{2})",data.ColumnName, data.XColumn? "X":data.Kind.ToString(),data.Group); 
		
			if(bSelected)
				dc.DrawString(myString,m_TextFont,m_SelectedTextBrush,cellRectangle,m_TextFormat);
			else
				dc.DrawString(myString,m_TextFont,m_TextBrush,cellRectangle,m_TextFormat);
		
		
		}
		
		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			return data.ColumnName;
		}

		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
		}

		public override object Clone()
		{
			return new ColumnHeaderStyle(this);
		}
	}
	
}
