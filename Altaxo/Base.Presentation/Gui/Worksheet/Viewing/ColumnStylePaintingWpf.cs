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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui.Worksheet.Viewing
{
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
		}


		public static void PaintBackground(this Altaxo.Worksheet.ColumnStyle thiss, DrawingContext dc, Altaxo.Graph.RectangleD cellRectangle, bool bSelected)
		{
			var cellRect = cellRectangle.ToWpf();
			if (bSelected)
				dc.DrawRectangle(thiss.DefaultSelectedBackgroundBrush.ToWpf(), null, cellRect);
			else
				dc.DrawRectangle(thiss.BackgroundBrush.ToWpf(), null, cellRect);

			dc.DrawLine(thiss.CellBorder.ToWpf(), cellRect.BottomLeft, cellRect.BottomRight);
			dc.DrawLine(thiss.CellBorder.ToWpf(), cellRect.BottomRight, cellRect.TopRight);
		}

		private static void RowHeaderStyle_Paint(Altaxo.Worksheet.RowHeaderStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			var dc = (DrawingContext)drawingContext;
			Rect cellRectangle = cellRect.ToWpf();
			thiss.PaintBackground(dc, cellRect, bSelected);

			string text = "[" + nRow + "]";

			var font = new Typeface("Arial");
			var fontSize = thiss.TextFont.Height;
			var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

			FormattedText t;


			t = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush);
			t.MaxTextWidth = cellRectangle.Width;
			t.TextAlignment = TextAlignment.Center;
			dc.DrawText(t, cellRectangle.Location); // ("[" + nRow + "]", _textFont, _textBrush, cellRectangle, _textFormat);

		}


		private static void ColumnHeaderStyle_Paint(Altaxo.Worksheet.ColumnHeaderStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			var dc = (DrawingContext)drawingContext;
			Rect cellRectangle = cellRect.ToWpf();

			thiss.PaintBackground(dc, cellRect, bSelected);

			Altaxo.Data.DataColumnCollection dataColCol = (Altaxo.Data.DataColumnCollection)DocumentPath.GetRootNodeImplementing(data, typeof(Altaxo.Data.DataColumnCollection));
			string columnnumber = dataColCol.GetColumnNumber(data).ToString();
			string kindandgroup = string.Format("({0}{1})", dataColCol.GetColumnKind(data).ToString(), dataColCol.GetColumnGroup(data));
			int fontheight = thiss.TextFont.Height;
			var nameRectangle = cellRectangle;
			nameRectangle.Height = Math.Max(fontheight, cellRectangle.Height - fontheight);
			var numRectangle = cellRectangle;
			numRectangle.Height = fontheight;
			numRectangle.Y = Math.Max(cellRectangle.Y + cellRectangle.Height - fontheight, cellRectangle.Y);

			var font = new Typeface("Arial");
			var fontSize = thiss.TextFont.Height;
			var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

			FormattedText t;

			t = new FormattedText(columnnumber, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush);
			t.MaxTextWidth = numRectangle.Width;
			t.TextAlignment = TextAlignment.Left;
			dc.DrawText(t, numRectangle.Location);

			t = new FormattedText(kindandgroup, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush);
			t.MaxTextWidth = numRectangle.Width;
			t.TextAlignment = TextAlignment.Right;
			dc.DrawText(t, numRectangle.Location);

			t = new FormattedText(data.Name, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush);
			t.MaxTextWidth = nameRectangle.Width;
			t.TextAlignment = TextAlignment.Center;
			dc.DrawText(t, nameRectangle.Location);
		}


		private static void DoubleColumnStyle_Paint(Altaxo.Worksheet.DoubleColumnStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			string myString = ((Altaxo.Data.DoubleColumn)data)[nRow].ToString();
			GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
		}

		private static void DateTimeColumnStyle_Paint(Altaxo.Worksheet.DateTimeColumnStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			DateTime t = ((Altaxo.Data.DateTimeColumn)data)[nRow];

			string myString = (t.Kind == DateTimeKind.Unspecified || t.Kind == DateTimeKind.Local) ?
				t.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") :
				t.ToString("o");

			GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
		}

		private static void TextColumnStyle_Paint(Altaxo.Worksheet.TextColumnStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			string myString = data[nRow].ToString();
			GeneralText_Paint(thiss, drawingContext, cellRect, myString, TextAlignment.Right, bSelected);
		}

		private static void GeneralText_Paint(Altaxo.Worksheet.ColumnStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, string textToDraw, TextAlignment alignment, bool bSelected)
		{

			var dc = (DrawingContext)drawingContext;
			Rect cellRectangle = cellRect.ToWpf();

			thiss.PaintBackground(dc, cellRect, bSelected);


			var font = new Typeface("Arial");
			var fontSize = thiss.TextFont.Height;
			var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

			FormattedText t;
			t = new FormattedText(textToDraw, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush);
			t.MaxTextWidth = cellRect.Width;
			t.TextAlignment = alignment;
			t.Trimming = TextTrimming.CharacterEllipsis;
			dc.DrawText(t, cellRectangle.Location);
		}

		public static void Paint(this Altaxo.Worksheet.ColumnStyle thiss, DrawingContext dc, Altaxo.Graph.RectangleD cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			thiss.Paint(typeof(DrawingContext), dc, cellRectangle, nRow, data, bSelected);
		}

	}
}
