// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class views the line numbers and folding markers.
	/// </summary>
	public class GutterMargin : AbstractMargin
	{
		StringFormat numberStringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
		
		public static Cursor RightLeftCursor;
		
		static GutterMargin()
		{
			Stream cursorStream = Assembly.GetCallingAssembly().GetManifestResourceStream("RightArrow.cur");
			RightLeftCursor = new Cursor(cursorStream);
			cursorStream.Close();
		}
		
		
		public override Cursor Cursor {
			get {
				return RightLeftCursor;
			}
		}
		
		public override Size Size {
			get {
				return new Size((int)(textArea.TextView.GetWidth('8') * Math.Max(3, (int)Math.Log10(textArea.Document.TotalNumberOfLines) + 1)),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.ShowLineNumbers;
			}
		}
		
		public GutterMargin(TextArea textArea) : base(textArea)
		{
			numberStringFormat.LineAlignment = StringAlignment.Near;
			numberStringFormat.FormatFlags   = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox |
			                                    StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
		}
		
		public override void Paint(Graphics g, Rectangle rect)
		{
			if (rect.Width <= 0 || rect.Height <= 0) {
				return;
			}
			HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
			int fontHeight = lineNumberPainterColor.Font.Height;
			
			for (int y = 0; y < (DrawingPosition.Height + textArea.TextView.VisibleLineDrawingRemainder) / fontHeight + 1; ++y) {
				int ypos = drawingPosition.Y + fontHeight * y  - textArea.TextView.VisibleLineDrawingRemainder;
				Rectangle backgroundRectangle = new Rectangle(drawingPosition.X, ypos, drawingPosition.Width, fontHeight);
				if (rect.IntersectsWith(backgroundRectangle)) {
					g.FillRectangle(textArea.Enabled ? new SolidBrush(lineNumberPainterColor.BackgroundColor) : SystemBrushes.InactiveBorder, backgroundRectangle);
					int curLine = textArea.Document.GetFirstLogicalLine(textArea.Document.GetVisibleLine(textArea.TextView.FirstVisibleLine) + y);
//					int curLine = y + textArea.TextView.FirstVisibleLine;
					if (curLine < textArea.Document.TotalNumberOfLines) {
						numberStringFormat.Alignment = StringAlignment.Far;
						g.DrawString((curLine + 1).ToString(),
						                  lineNumberPainterColor.Font,
						                  new SolidBrush(lineNumberPainterColor.Color),
						                  backgroundRectangle,
						                  numberStringFormat);
						numberStringFormat.Alignment = StringAlignment.Near;
					}
				}
			}
		}
		
		Point selectionStartPos;
		bool selectionComeFromGutter = false;
		public override void HandleMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			selectionComeFromGutter = true;
			int physicalLine = textArea.TextView.FirstVisibleLine + (int)(mousepos.Y / textArea.TextView.FontHeight);
			int realline     = textArea.Document.GetFirstLogicalLine(physicalLine);
			if (realline >= 0 && realline < textArea.Document.TotalNumberOfLines) {
				selectionStartPos = new Point(0, realline);
				textArea.SelectionManager.ClearSelection();
				textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new Point(textArea.Document.GetLineSegment(realline).Length + 1, realline)));
				textArea.Caret.Position = selectionStartPos;
			}
		}
		
		public override void HandleMouseLeave(EventArgs e)
		{
			selectionComeFromGutter = false;
		}
		
		public override void HandleMouseMove(Point mousepos, MouseButtons mouseButtons)
		{
			if (mouseButtons == MouseButtons.Left) {
				if (selectionComeFromGutter) {
					int physicalLine = textArea.TextView.FirstVisibleLine + (int)(mousepos.Y / textArea.TextView.FontHeight);
					int realline     = textArea.Document.GetFirstLogicalLine(physicalLine);
					Point realmousepos = new Point(0, realline);
					if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
						if (selectionStartPos.Y == realmousepos.Y) {
							textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, realmousepos, new Point(textArea.Document.GetLineSegment(realmousepos.Y).Length + 1, realmousepos.Y)));
						} else  if (selectionStartPos.Y < realmousepos.Y && textArea.SelectionManager.HasSomethingSelected) {
							textArea.SelectionManager.ExtendSelection(textArea.SelectionManager.SelectionCollection[0].EndPosition, realmousepos);
						} else {
							textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
						}
						textArea.Caret.Position = realmousepos;
					}
				} else {
					if (textArea.SelectionManager.HasSomethingSelected) {
						selectionStartPos =  textArea.Document.OffsetToPosition(textArea.SelectionManager.SelectionCollection[0].Offset);
						int physicalLine = textArea.TextView.FirstVisibleLine + (int)(mousepos.Y / textArea.TextView.FontHeight);
						int realline     = textArea.Document.GetFirstLogicalLine(physicalLine);
						Point realmousepos = new Point(0, realline);
						if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
							textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
						}
						textArea.Caret.Position = realmousepos;
					}
				}
			}
		}
	}
}
