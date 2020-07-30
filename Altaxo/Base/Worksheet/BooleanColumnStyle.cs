#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using Altaxo.Serialization;

namespace Altaxo.Worksheet
{
  public class BooleanColumnStyle : Altaxo.Worksheet.ColumnStyle
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BooleanColumnStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BooleanColumnStyle)obj;
        info.AddBaseValueEmbedded(s, typeof(BooleanColumnStyle).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        BooleanColumnStyle s = null != o ? (BooleanColumnStyle)o : new BooleanColumnStyle();
        info.GetBaseValueEmbedded(s, typeof(BooleanColumnStyle).BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    public BooleanColumnStyle()
      : base(ColumnStyleType.DataCell)
    {
      _textFormat.Alignment = StringAlignment.Near;
      _textFormat.FormatFlags = StringFormatFlags.LineLimit;
    }

    public BooleanColumnStyle(BooleanColumnStyle tcs)
      : base(tcs)
    {
    }

    public override object Clone()
    {
      return new BooleanColumnStyle(this);
    }

    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
    {
      var v = ((Altaxo.Data.BooleanColumn)data)[nRow];
      return v.HasValue ? (v.Value ? "true" : "false") : "";
    }

    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
      try
      {
        var success = bool.TryParse(s, out var result);
        ((Altaxo.Data.BooleanColumn)data)[nRow] = success ? (bool?)result : null;

      }
      catch (Exception) { }
    }

    public static Dictionary<System.Type, Action<BooleanColumnStyle, object, RectangleD2D, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<BooleanColumnStyle, object, RectangleD2D, int, Data.DataColumn, bool>>();

    public override void Paint(System.Type dctype, object dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      if (RegisteredPaintMethods.TryGetValue(dctype, out var action))
        action(this, dc, cellRectangle, nRow, data, bSelected);
      else
        throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
    }

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);

      bool? b = ((Altaxo.Data.BooleanColumn)data)[nRow];
      var myString = b switch
      {
        true => "true",
        false => "false",
        null => "",
      };

      var brush = bSelected ? _defaultSelectedTextBrush : TextBrush;

      using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, cellRectangle, dc, 1))
      {
        dc.DrawString(myString, GdiFontManager.ToGdi(_textFont), brushGdi, cellRectangle, _textFormat);
      }
    }
  } // end of class Altaxo.Worksheet.DateTimeColumnStyle
}
