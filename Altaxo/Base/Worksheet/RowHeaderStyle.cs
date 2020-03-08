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

using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Worksheet
{
  public class RowHeaderStyle : Altaxo.Worksheet.ColumnStyle
  {
    protected int _rowHeight = 20;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RowHeaderStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RowHeaderStyle)obj;
        info.AddBaseValueEmbedded(s, typeof(RowHeaderStyle).BaseType);
        info.AddValue("Height", s._rowHeight);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RowHeaderStyle s = null != o ? (RowHeaderStyle)o : new RowHeaderStyle();
        info.GetBaseValueEmbedded(s, typeof(RowHeaderStyle).BaseType, parent);
        s._rowHeight = info.GetInt32("Height");
        return s;
      }
    }

    #endregion Serialization

    public RowHeaderStyle()
      : base(ColumnStyleType.RowHeader)
    {
      _textFormat.Alignment = StringAlignment.Center;
      _textFormat.FormatFlags = StringFormatFlags.LineLimit;
    }

    public RowHeaderStyle(RowHeaderStyle rhs)
      : base(rhs)
    {
      _rowHeight = rhs._rowHeight;
    }

    public int Height
    {
      get
      {
        return _rowHeight;
      }
      set
      {
        _rowHeight = value;
      }
    }

    public override object Clone()
    {
      return new RowHeaderStyle(this);
    }

    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
    {
      return nRow.ToString(Altaxo.Settings.GuiCulture.Instance);
    }

    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
    }

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);

      var brush = bSelected ? _defaultSelectedTextBrush : TextBrush;

      using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, cellRectangle, dc, 1))
      {
        dc.DrawString("[" + nRow + "]", GdiFontManager.ToGdi(_textFont), brushGdi, cellRectangle, _textFormat);

      }
    }

    public static Dictionary<System.Type, Action<RowHeaderStyle, object, RectangleD2D, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<RowHeaderStyle, object, RectangleD2D, int, Data.DataColumn, bool>>();

    public override void Paint(System.Type dctype, object dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      if (RegisteredPaintMethods.TryGetValue(dctype, out var action))
        action(this, dc, cellRectangle, nRow, data, bSelected);
      else
        throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
    }
  }
}
