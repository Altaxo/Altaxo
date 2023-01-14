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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;

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
        var s = (DateTimeColumnStyle)obj;
        info.AddBaseValueEmbedded(s, typeof(DateTimeColumnStyle).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        DateTimeColumnStyle s = o is not null ? (DateTimeColumnStyle)o : new DateTimeColumnStyle();
        info.GetBaseValueEmbedded(s, typeof(DateTimeColumnStyle).BaseType!, parent);
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
      return val == Altaxo.Data.DateTimeColumn.NullValue ? "" : val.ToString("o", Altaxo.Settings.GuiCulture.Instance);
    }

    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
      if (!DateTime.TryParse(s, Altaxo.Settings.GuiCulture.Instance, System.Globalization.DateTimeStyles.RoundtripKind, out var newval))
        newval = Altaxo.Data.DateTimeColumn.NullValue;

      ((Altaxo.Data.DateTimeColumn)data)[nRow] = newval;
    }

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);

      DateTime t = ((Altaxo.Data.DateTimeColumn)data)[nRow];

      string myString = (t.Kind == DateTimeKind.Unspecified || t.Kind == DateTimeKind.Local) ?
        t.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") :
        t.ToString("o");

      var brush = bSelected ? _defaultSelectedTextBrush : TextBrush;

      using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, cellRectangle.ToAxo(), dc, 1))
      {
        dc.DrawString(myString, GdiFontManager.ToGdi(_textFont), brushGdi, cellRectangle, _textFormat);

      }

    }

    public static Dictionary<System.Type, Action<DateTimeColumnStyle, object, RectangleD2D, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<DateTimeColumnStyle, object, RectangleD2D, int, Data.DataColumn, bool>>();

    public override void Paint(System.Type dctype, object dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      if (RegisteredPaintMethods.TryGetValue(dctype, out var action))
        action(this, dc, cellRectangle, nRow, data, bSelected);
      else
        throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
    }
  } // end of class Altaxo.Worksheet.DateTimeColumnStyle
}
