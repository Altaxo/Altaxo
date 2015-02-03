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
	public class DateTimeColumnStyle : Altaxo.Worksheet.ColumnStyle
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeColumnStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DateTimeColumnStyle s = (DateTimeColumnStyle)obj;
				info.AddBaseValueEmbedded(s, typeof(DateTimeColumnStyle).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DateTimeColumnStyle s = null != o ? (DateTimeColumnStyle)o : new DateTimeColumnStyle();
				info.GetBaseValueEmbedded(s, typeof(DateTimeColumnStyle).BaseType, parent);
				return s;
			}
		}

		#endregion Serialization

		public DateTimeColumnStyle()
			: base(ColumnStyleType.DataCell)
		{
			_textFormat.Alignment = StringAlignment.Far;
			_textFormat.FormatFlags = StringFormatFlags.LineLimit;
		}

		public DateTimeColumnStyle(DateTimeColumnStyle dtcs)
			: base(dtcs)
		{
		}

		public override object Clone()
		{
			return new DateTimeColumnStyle(this);
		}

		public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
		{
			DateTime val = ((Altaxo.Data.DateTimeColumn)data)[nRow];
			return val == Altaxo.Data.DateTimeColumn.NullValue ? "" : val.ToString("o");
		}

		public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
		{
			DateTime newval = Altaxo.Data.DateTimeColumn.NullValue;
			if (string.IsNullOrEmpty(s) || DateTime.TryParse(s, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.RoundtripKind, out newval))
				((Altaxo.Data.DateTimeColumn)data)[nRow] = newval;
		}

		public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			PaintBackground(dc, cellRectangle, bSelected);

			DateTime t = ((Altaxo.Data.DateTimeColumn)data)[nRow];

			string myString = (t.Kind == DateTimeKind.Unspecified || t.Kind == DateTimeKind.Local) ?
				t.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") :
				t.ToString("o");

			if (bSelected)
				dc.DrawString(myString, _textFont.ToGdi(), _defaultSelectedTextBrush, cellRectangle, _textFormat);
			else
				dc.DrawString(myString, _textFont.ToGdi(), _textBrush, cellRectangle, _textFormat);
		}

		public static Dictionary<System.Type, Action<DateTimeColumnStyle, object, Altaxo.Graph.RectangleD, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<DateTimeColumnStyle, object, Graph.RectangleD, int, Data.DataColumn, bool>>();

		public override void Paint(System.Type dctype, object dc, Altaxo.Graph.RectangleD cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			Action<DateTimeColumnStyle, object, Altaxo.Graph.RectangleD, int, Altaxo.Data.DataColumn, bool> action;
			if (RegisteredPaintMethods.TryGetValue(dctype, out action))
				action(this, dc, cellRectangle, nRow, data, bSelected);
			else
				throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
		}
	} // end of class Altaxo.Worksheet.DateTimeColumnStyle
}