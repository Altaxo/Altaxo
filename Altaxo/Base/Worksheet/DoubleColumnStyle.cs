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
  public class DoubleColumnStyle : Altaxo.Worksheet.ColumnStyle
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DoubleColumnStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DoubleColumnStyle)obj;
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
      var ns = new Altaxo.Worksheet.DoubleColumnStyle(this);
      return ns;
    }

    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
    {
      double val = ((Altaxo.Data.DoubleColumn)data)[nRow];
      return double.IsNaN(val) ? "" : val.ToString(Altaxo.Settings.GuiCulture.Instance);
    }

    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
      if (!double.TryParse(s, System.Globalization.NumberStyles.Float, Altaxo.Settings.GuiCulture.Instance, out var result))
        result = double.NaN;
      ((Altaxo.Data.DoubleColumn)data)[nRow] = result;
    }

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);

      string myString = ((Altaxo.Data.DoubleColumn)data)[nRow].ToString();

      var brush = bSelected ? _defaultSelectedTextBrush : TextBrush;

      using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, cellRectangle, dc, 1))
      {
        dc.DrawString(myString, GdiFontManager.ToGdi(_textFont), brushGdi, cellRectangle, _textFormat);

      }
    }

    public static Dictionary<System.Type, Action<DoubleColumnStyle, object, RectangleD2D, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<DoubleColumnStyle, object, RectangleD2D, int, Data.DataColumn, bool>>();

    public override void Paint(System.Type dctype, object dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      if (RegisteredPaintMethods.TryGetValue(dctype, out var action))
        action(this, dc, cellRectangle, nRow, data, bSelected);
      else
        throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
    }
  } // end of class Altaxo.Worksheet.DoubleColumnStyle
}
