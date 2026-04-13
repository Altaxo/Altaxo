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
  /// <summary>
  /// Represents the worksheet style used for row headers.
  /// </summary>
  public class RowHeaderStyle : Altaxo.Worksheet.ColumnStyle
  {
    /// <summary>
    /// The row-header height.
    /// </summary>
    protected int _rowHeight = 20;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RowHeaderStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RowHeaderStyle)o;
        info.AddBaseValueEmbedded(s, typeof(RowHeaderStyle).BaseType!);
        info.AddValue("Height", s._rowHeight);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        RowHeaderStyle s = (RowHeaderStyle?)o ?? new RowHeaderStyle();
        info.GetBaseValueEmbedded(s, typeof(RowHeaderStyle).BaseType!, parent);
        s._rowHeight = info.GetInt32("Height");
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="RowHeaderStyle"/> class.
    /// </summary>
    public RowHeaderStyle()
      : base(ColumnStyleType.RowHeader)
    {
      _textFormat.Alignment = StringAlignment.Center;
      _textFormat.FormatFlags = StringFormatFlags.LineLimit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RowHeaderStyle"/> class by copying another instance.
    /// </summary>
    /// <param name="rhs">The instance to copy.</param>
    public RowHeaderStyle(RowHeaderStyle rhs)
      : base(rhs)
    {
      _rowHeight = rhs._rowHeight;
    }

    /// <summary>
    /// Gets or sets the row-header height.
    /// </summary>
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

    /// <inheritdoc />
    public override object Clone()
    {
      return new RowHeaderStyle(this);
    }

    /// <inheritdoc />
    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
    {
      return nRow.ToString(Altaxo.Settings.GuiCulture.Instance);
    }

    /// <inheritdoc />
    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
    }

    /// <inheritdoc />
    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn? data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);

      var brush = bSelected ? _defaultSelectedTextBrush : TextBrush;

      using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, cellRectangle.ToAxo(), dc, 1))
      {
        dc.DrawString("[" + nRow + "]", GdiFontManager.ToGdi(_textFont), brushGdi, cellRectangle, _textFormat);

      }
    }

    /// <summary>
    /// Gets the registered paint methods for specific drawing contexts.
    /// </summary>
    /// <summary>
    /// Gets the registered paint methods for specific drawing contexts.
    /// </summary>
    public static Dictionary<System.Type, Action<RowHeaderStyle, object, RectangleD2D, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<RowHeaderStyle, object, RectangleD2D, int, Data.DataColumn, bool>>();

    /// <inheritdoc />
    public override void Paint(System.Type dctype, object dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      if (RegisteredPaintMethods.TryGetValue(dctype, out var action))
        action(this, dc, cellRectangle, nRow, data, bSelected);
      else
        throw new NotImplementedException("Paint method is not implemented for context type " + dc.GetType().ToString());
    }
  }
}
