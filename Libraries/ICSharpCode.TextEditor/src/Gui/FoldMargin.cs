// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
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
	public class FoldMargin : AbstractMargin
	{
		public override Size Size {
			get {
				return new Size((int)(textArea.TextView.FontHeight),
				                -1);
			}
		}
		
		public override Cursor Cursor {
			get {
				return GutterMargin.RightLeftCursor;
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.EnableFolding;
			}
		}
		
		public FoldMargin(TextArea textArea) : base(textArea)
		{
		}
		
		public override void Paint(Graphics g, Rectangle rect)
		{
			HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
			HighlightColor foldLineColor          = textArea.Document.HighlightingStrategy.GetColorFor("FoldLine");
			
			
			for (int y = 0; y < (DrawingPosition.Height + textArea.TextView.VisibleLineDrawingRemainder) / textArea.TextView.FontHeight + 1; ++y) {
				Rectangle markerRectangle = new Rectangle(DrawingPosition.X,
				                                        DrawingPosition.Top + y * textArea.TextView.FontHeight - textArea.TextView.VisibleLineDrawingRemainder,
				                                        DrawingPosition.Width,
				                                        textArea.TextView.FontHeight);
				
				if (rect.IntersectsWith(markerRectangle)) {
					// draw dotted separator line
					if (textArea.Document.TextEditorProperties.ShowLineNumbers) {
						g.FillRectangle(textArea.Enabled ? new SolidBrush(lineNumberPainterColor.BackgroundColor) : SystemBrushes.InactiveBorder, 
					                new Rectangle(markerRectangle.X + 1, markerRectangle.Y, markerRectangle.Width - 1, markerRectangle.Height));
					
					
						g.DrawLine(new Pen(new HatchBrush(HatchStyle.Percent50, lineNumberPainterColor.Color, lineNumberPainterColor.BackgroundColor)),
										base.drawingPosition.X,
										markerRectangle.Y,
										base.drawingPosition.X,
										markerRectangle.Bottom);
					} else {
						g.FillRectangle(textArea.Enabled ? new SolidBrush(lineNumberPainterColor.BackgroundColor) : SystemBrushes.InactiveBorder, markerRectangle);
					}
					
					int currentLine = textArea.TextView.FirstVisibleLine + y;
					PaintFoldMarker(g, currentLine, markerRectangle);
				}
			}
		}
		
		
		void PaintFoldMarker(Graphics g, int lineNumber, Rectangle drawingRectangle)
		{
			HighlightColor foldLineColor = textArea.Document.HighlightingStrategy.GetColorFor("FoldLine");
			
			bool isFoldStart = textArea.Document.FoldingManager.IsFoldStart(lineNumber);
			bool isBetween   = textArea.Document.FoldingManager.IsBetweenFolding(lineNumber);
			bool isFoldEnd   = textArea.Document.FoldingManager.IsFoldEnd(lineNumber);
							
			int foldMarkerSize = (int)Math.Round(textArea.TextView.FontHeight * 0.57f);
			foldMarkerSize -= (foldMarkerSize) % 2;
			int foldMarkerYPos = drawingRectangle.Y + (int)((drawingRectangle.Height - foldMarkerSize) / 2);
			int xPos = drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2 + foldMarkerSize / 2;
			
			
			if (isFoldStart) {
				ArrayList startFoldings = textArea.Document.FoldingManager.GetFoldingsWithStart(lineNumber);
				bool isVisible = true;
				bool moreLinedOpenFold = false;
				foreach (FoldMarker foldMarker in startFoldings) {
					if (foldMarker.IsFolded) {
						isVisible = false;
					} else {
						moreLinedOpenFold = foldMarker.EndLine > foldMarker.StartLine;
					}
				}
				
				ArrayList endFoldings = textArea.Document.FoldingManager.GetFoldingsWithEnd(lineNumber);
				bool isFoldEndFromUpperFold = false;
				foreach (FoldMarker foldMarker in endFoldings) {
					if (foldMarker.EndLine > foldMarker.StartLine && !foldMarker.IsFolded) {
						isFoldEndFromUpperFold = true;
					} 
				}
				
				DrawFoldMarker(g, new RectangleF(drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2,
				                                 foldMarkerYPos,
				                                 foldMarkerSize,
				                                 foldMarkerSize),
				                  isVisible);
				if (isBetween || isFoldEndFromUpperFold) {
					g.DrawLine(new Pen(foldLineColor.Color),
					           xPos,
					           drawingRectangle.Top,
					           xPos,
					           foldMarkerYPos);
					
				}
				
				if (isBetween || moreLinedOpenFold) {
					g.DrawLine(new Pen(foldLineColor.Color),
					           xPos,
					           foldMarkerYPos + foldMarkerSize,
					           xPos,
					           drawingRectangle.Bottom);
				}
			} else {
				if (isFoldEnd) {
					int midy = drawingRectangle.Top + drawingRectangle.Height / 2;
					g.DrawLine(new Pen(foldLineColor.Color),
					                xPos,
					                drawingRectangle.Top,
					                xPos,
					                isBetween ? drawingRectangle.Bottom : midy);
					g.DrawLine(new Pen(foldLineColor.Color),
									xPos,
									midy,
									xPos + foldMarkerSize / 2,
									midy);
				} else if (isBetween) {
					g.DrawLine(new Pen(foldLineColor.Color),
					                xPos,
					                drawingRectangle.Top,
					                xPos,
					                drawingRectangle.Bottom);
				}
			}
		}
		
//		protected override void OnClick(EventArgs e)
//		{
//			base.OnClick(e);
//			bool  showFolding = textarea.Document.TextEditorProperties.EnableFolding;
//			Point mousepos    = PointToClient(Control.MousePosition);
//			int   realline    = textarea.Document.GetVisibleLine((int)((mousepos.Y + virtualTop) / textarea.FontHeight));
//			
//			// focus the textarea if the user clicks on the line number view
//			textarea.Focus();
//			
//			if (!showFolding || mousepos.X < Width - 15 || realline < 0 || realline + 1 >= textarea.Document.TotalNumberOfLines) {
//				return;
//			}
//			
//			ArrayList foldMarkers = textarea.Document.FoldingManager.GetFoldingsWithStart(realline);
//			foreach (FoldMarker fm in foldMarkers) {
//				fm.IsFolded = !fm.IsFolded;
//			}
//			Refresh();
//			textarea.Refresh();
//			TextEditorControl.IconBar.Refresh();
//		}
		
#region Drawing functions
		void DrawFoldMarker(Graphics g, RectangleF rectangle, bool isOpened)
		{
			HighlightColor foldMarkerColor = textArea.Document.HighlightingStrategy.GetColorFor("FoldMarker");
			HighlightColor foldLineColor   = textArea.Document.HighlightingStrategy.GetColorFor("FoldLine");
			
			Rectangle intRect = new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
			g.FillRectangle(new SolidBrush(foldMarkerColor.BackgroundColor), intRect);
			g.DrawRectangle(new Pen(foldMarkerColor.Color), intRect);
			
			int space  = (int)Math.Round(((double)rectangle.Height) / 8d) + 1;
			int mid    = intRect.Height / 2 + intRect.Height % 2;			
			
			g.DrawLine(new Pen(foldLineColor.BackgroundColor), 
			           rectangle.X + space, 
			           rectangle.Y + mid, 
			           rectangle.X + rectangle.Width - space, 
			           rectangle.Y + mid);
			
			if (!isOpened) {
				g.DrawLine(new Pen(foldLineColor.BackgroundColor), 
				           rectangle.X + mid, 
				           rectangle.Y + space, 
				           rectangle.X + mid, 
				           rectangle.Y + rectangle.Height - space);
			}
		}
#endregion
	}
}
