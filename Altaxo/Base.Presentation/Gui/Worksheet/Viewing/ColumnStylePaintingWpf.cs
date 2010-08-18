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
	/// <summary>
	/// Responsible for painting the Worksheet column styles with Wpf technology.
	/// </summary>
	public static class ColumnStylePaintingWpf
	{
		public static void PaintBackground(this Altaxo.Worksheet.ColumnStyle thiss, DrawingContext dc, Rect cellRectangle, bool bSelected)
		{
			if (bSelected)
				dc.DrawRectangle(thiss.DefaultSelectedBackgroundBrush.ToWpf(), null, cellRectangle);
			else
				dc.DrawRectangle(thiss.BackgroundBrush.ToWpf(), null, cellRectangle);

			dc.DrawLine(thiss.CellBorder.ToWpf(), cellRectangle.BottomLeft, cellRectangle.BottomRight);
			dc.DrawLine(thiss.CellBorder.ToWpf(), cellRectangle.BottomRight, cellRectangle.TopRight);
		}

			private static void RowHeaderStyle_Paint(Altaxo.Worksheet.RowHeaderStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
			{
				var dc = (DrawingContext)drawingContext;
				Rect cellRectangle = cellRect.ToWpf();
				thiss.PaintBackground(dc, cellRectangle, bSelected);

				string text = "[" + nRow + "]";

				if (bSelected)
				{
					FormattedText t = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 10, thiss.DefaultSelectedTextBrush.ToWpf());
					dc.DrawText(t, cellRectangle.Location); // ("[" + nRow + "]", _textFont, _textBrush, cellRectangle, _textFormat);
					// dc.DrawText(DrawString("[" + nRow + "]", _textFont, _defaultSelectedTextBrush, cellRectangle, _textFormat);
				}
				else
				{
					FormattedText t = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 10, thiss.TextBrush.ToWpf());
					dc.DrawText(t, cellRectangle.Location); // ("[" + nRow + "]", _textFont, _textBrush, cellRectangle, _textFormat);
				}
			}


			private static void ColumnHeaderStyle_Paint(Altaxo.Worksheet.ColumnHeaderStyle thiss, object drawingContext, Altaxo.Graph.RectangleD cellRect, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
			{
				var dc = (DrawingContext)drawingContext;
				Rect cellRectangle = cellRect.ToWpf();

				thiss.PaintBackground(dc, cellRectangle, bSelected);

				Altaxo.Data.DataColumnCollection dataColCol = (Altaxo.Data.DataColumnCollection)Main.DocumentPath.GetRootNodeImplementing(data, typeof(Altaxo.Data.DataColumnCollection));
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

				/*
					dc.DrawString(columnnumber, _textFont, _defaultSelectedTextBrush, numRectangle, _leftUpperTextFormat);
					dc.DrawString(kindandgroup, _textFont, _defaultSelectedTextBrush, numRectangle, _rightUpperTextFormat);
					dc.DrawString(data.Name, _textFont, _defaultSelectedTextBrush, nameRectangle, _textFormat);
				 */
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

				thiss.PaintBackground(dc, cellRectangle, bSelected);


				var font = new Typeface("Arial");
				var fontSize = thiss.TextFont.Height;
				var txtBrush = bSelected ? thiss.DefaultSelectedTextBrush.ToWpf() : thiss.TextBrush.ToWpf();

				FormattedText t;
				t = new FormattedText(textToDraw, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, font, fontSize, txtBrush);
				t.MaxTextWidth = cellRect.Width;
				t.TextAlignment = alignment;
				dc.DrawText(t, cellRectangle.Location);
			}

		public static void Paint(this Altaxo.Worksheet.ColumnStyle thiss, DrawingContext dc, Rect cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			thiss.Paint(typeof(DrawingContext), dc, cellRectangle.ToAltaxo(), nRow, data, bSelected);
		}


		static ColumnStylePaintingWpf()
		{
			Altaxo.Worksheet.RowHeaderStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), RowHeaderStyle_Paint);
			Altaxo.Worksheet.ColumnHeaderStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), ColumnHeaderStyle_Paint);
			Altaxo.Worksheet.DoubleColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), DoubleColumnStyle_Paint);
			Altaxo.Worksheet.DateTimeColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext), DateTimeColumnStyle_Paint);
			Altaxo.Worksheet.TextColumnStyle.RegisteredPaintMethods.Add(typeof(DrawingContext),TextColumnStyle_Paint);
		}
	}
}
