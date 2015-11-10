#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Graph.Gdi;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Worksheet
{
	public class DoubleColumnStyle : Altaxo.Worksheet.ColumnStyle
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DoubleColumnStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DoubleColumnStyle s = (DoubleColumnStyle)obj;
				info.AddBaseValueEmbedded(s, typeof(DoubleColumnStyle).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DoubleColumnStyle s = null != o ? (DoubleColumnStyle)o : new DoubleColumnStyle();
				info.GetBaseValueEmbedded(s, typeof(DoubleColumnStyle).BaseType, parent);
				return s;
			}
		}

		#endregion Serialization

		public DoubleColumnStyle()
			: base(ColumnStyleType.DataCell)
		{
			_textFormat.Alignment = StringAlignment.Far;
			_textFormat.FormatFlags = StringFormatFlags.LineLimit;
		}

		public DoubleColumnStyle(DoubleColumnStyle ds)
			: base(ds)
		{
		}

		public override object Clone()
		{
			Altaxo.Worksheet.DoubleColumnStyle ns = new Altaxo.Worksheet.DoubleColumnStyle(this);
			return ns;
		}

		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			double val = ((Altaxo.Data.DoubleColumn)data)[nRow];
			return Double.IsNaN(val) ? "" : val.ToString();
		}

		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
			double newval;
			try
			{
				newval = s.Length == 0 ? Double.NaN : System.Convert.ToDouble(s);
				((Altaxo.Data.DoubleColumn)data)[nRow] = newval;
			}
			catch (Exception) { }
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			PaintBackground(dc, cellRectangle, bSelected);

			string myString = ((Altaxo.Data.DoubleColumn)data)[nRow].ToString();

			if (bSelected)
				dc.DrawString(myString, GdiFontManager.ToGdi(_textFont), _defaultSelectedTextBrush, cellRectangle, _textFormat);
			else
				dc.DrawString(myString, GdiFontManager.ToGdi(_textFont), _textBrush, cellRectangle, _textFormat);
		}

		public static Dictionary<System.Type, Action<DoubleColumnStyle, object, Altaxo.Graph.RectangleD, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<DoubleColumnStyle, object, Graph.RectangleD, int, Data.DataColumn, bool>>();

		public override void Paint(System.Type dctype, object dc, Altaxo.Graph.RectangleD cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			Action<DoubleColumnStyle, object, Altaxo.Graph.RectangleD, int, Altaxo.Data.DataColumn, bool> action;
			if (RegisteredPaintMethods.TryGetValue(dctype, out action))
				action(this, dc, cellRectangle, nRow, data, bSelected);
			else
				throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
		}
	} // end of class Altaxo.Worksheet.DoubleColumnStyle
}