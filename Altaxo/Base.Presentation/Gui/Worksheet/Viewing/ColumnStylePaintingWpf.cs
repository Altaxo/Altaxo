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
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;

namespace Altaxo.Gui.Worksheet.Viewing
{
  using Altaxo.Geometry;
  using Altaxo.Main;

  /// <summary>
  /// Responsible for painting the Worksheet column styles with Wpf technology.
  /// </summary>
  public static class ColumnStylePaintingWpf
  {
    static ColumnStylePaintingWpf()
    {
      Altaxo.Worksheet.RowHeaderStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), RowHeaderStyle_Paint);
      Altaxo.Worksheet.ColumnHeaderStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), ColumnHeaderStyle_Paint);
      Altaxo.Worksheet.DoubleColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), DoubleColumnStyle_Paint);
      Altaxo.Worksheet.DateTimeColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), DateTimeColumnStyle_Paint);
      Altaxo.Worksheet.TextColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), TextColumnStyle_Paint);
      Altaxo.Worksheet.BooleanColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), BooleanColumnStyle_Paint);
    }

    public static void PaintBackground(this Altaxo.Worksheet.ColumnStyle thiss, DrawingContext dc, RectangleD2D cellRectangle, bool bSelected)
    {
      var cellRect = cellRectangle.ToWpf();
      if (bSelected)
        dc.DrawRectangle(thiss.DefaultSelectedBackgroundBrush.ToWpf(), null, cellRect);
      else
        dc.DrawRectangle(thiss.BackgroundBrush.ToWpf(), null, cellRect);

      dc.DrawLine(thiss.CellBorder.ToWpf(), cellRect.BottomLeft, cellRect.BottomRight);
      dc.DrawLine(thiss.CellBorder.ToWpf(), cellRect.BottomRight, cellRect.TopRight);
    }

    private static void RowHeaderStyle_Paint(Altaxo.Worksheet.RowHeaderStyle thiss, object drawingContext, RectangleD2D cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      var dc = (DrawingContext)drawingContext;
      Rect cellRectangle = cellRect.ToWpf();
      thiss.PaintBackground(dc, cellRect, bSelected);

      string text = "[" + nRow + "]";

      var font = WpfFontManager.ToWpf(thiss.TextFont);
      var fontSize = (thiss.TextFont.Size * 96) / 72;
      var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

      FormattedText t;

      t = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush)
      {
        MaxTextWidth = cellRectangle.Width,
        TextAlignment = TextAlignment.Center
      };
      dc.DrawText(t, cellRectangle.Location); // ("[" + nRow + "]", _textFont, _textBrush, cellRectangle, _textFormat);
    }

    private static void ColumnHeaderStyle_Paint(Altaxo.Worksheet.ColumnHeaderStyle thiss, object drawingContext, RectangleD2D cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      var dc = (DrawingContext)drawingContext;
      Rect cellRectangle = cellRect.ToWpf();

      thiss.PaintBackground(dc, cellRect, bSelected);

      var dataColCol = (Altaxo.Data.DataColumnCollection)AbsoluteDocumentPath.GetRootNodeImplementing(data, typeof(Altaxo.Data.DataColumnCollection));
      string columnnumber = dataColCol.GetColumnNumber(data).ToString();
      string kindandgroup = string.Format("({0}{1})", dataColCol.GetColumnKind(data).ToString(), dataColCol.GetColumnGroup(data));
      var font = WpfFontManager.ToWpf(thiss.TextFont);
      var fontSize = (thiss.TextFont.Size * 96) / 72;
      var fontheight = font.FontFamily.LineSpacing * fontSize;
      var nameRectangle = cellRectangle;
      nameRectangle.Height = Math.Max(fontheight, cellRectangle.Height - fontheight);
      var numRectangle = cellRectangle;
      numRectangle.Height = fontheight;
      numRectangle.Y = Math.Max(cellRectangle.Y + cellRectangle.Height - fontheight, cellRectangle.Y);

      var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

      FormattedText t;

      t = new FormattedText(columnnumber, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush)
      {
        MaxTextWidth = numRectangle.Width,
        TextAlignment = TextAlignment.Left
      };
      dc.DrawText(t, numRectangle.Location);

      t = new FormattedText(kindandgroup, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush)
      {
        MaxTextWidth = numRectangle.Width,
        TextAlignment = TextAlignment.Right
      };
      dc.DrawText(t, numRectangle.Location);

      t = new FormattedText(data.Name, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush)
      {
        MaxTextWidth = nameRectangle.Width,
        TextAlignment = TextAlignment.Center
      };
      dc.DrawText(t, nameRectangle.Location);
    }

    private static void DoubleColumnStyle_Paint(Altaxo.Worksheet.DoubleColumnStyle thiss, object drawingContext, RectangleD2D cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      string myString = ((Altaxo.Data.DoubleColumn)data)[nRow].ToString();
      GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
    }

    private static void DateTimeColumnStyle_Paint(Altaxo.Worksheet.DateTimeColumnStyle thiss, object drawingContext, RectangleD2D cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      DateTime t = ((Altaxo.Data.DateTimeColumn)data)[nRow];

      string myString = (t.Kind == DateTimeKind.Unspecified || t.Kind == DateTimeKind.Local) ?
        t.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") :
        t.ToString("o");

      GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
    }

    private static void TextColumnStyle_Paint(Altaxo.Worksheet.TextColumnStyle thiss, object drawingContext, RectangleD2D cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      string myString = data[nRow].ToString();
      GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
    }

    private static void BooleanColumnStyle_Paint(Altaxo.Worksheet.BooleanColumnStyle thiss, object drawingContext, RectangleD2D cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      string myString = thiss.GetColumnValueAtRow(nRow, data);
      GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
    }

    private static void GeneralText_Paint(Altaxo.Worksheet.ColumnStyle thiss, object drawingContext, RectangleD2D cellRect, string textToDraw, TextAlignment alignment, bool bSelected)
    {
      var dc = (DrawingContext)drawingContext;
      Rect cellRectangle = cellRect.ToWpf();

      thiss.PaintBackground(dc, cellRect, bSelected);

      var font = WpfFontManager.ToWpf(thiss.TextFont);
      var fontSize = (thiss.TextFont.Size * 96) / 72;
      var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

      FormattedText t;
      t = new FormattedText(textToDraw, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush)
      {
        MaxTextWidth = cellRect.Width,
        TextAlignment = alignment,
        Trimming = TextTrimming.CharacterEllipsis
      };
      dc.DrawText(t, cellRectangle.Location);
    }

    public static void Paint(this Altaxo.Worksheet.ColumnStyle thiss, DrawingContext dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      thiss.Paint(typeof(DrawingContext), dc, cellRectangle, nRow, data, bSelected);
    }
  }
}
