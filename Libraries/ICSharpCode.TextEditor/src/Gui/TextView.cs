// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class paints the textarea.
	/// </summary>
	public class TextView : AbstractMargin
	{
		int          fontHeight;
		Hashtable    charWitdh           = new Hashtable();
		StringFormat measureStringFormat = StringFormat.GenericTypographic;
		Highlight    highlight;
		
		public Highlight Highlight {
			get {
				return highlight;
			}
			set {
				highlight = value;
			}
		}
		
		public override Cursor Cursor {
			get {
				return Cursors.IBeam;
			}
		}
		
		
		public int FirstVisibleLine {
			get {
				return textArea.VirtualTop.Y / fontHeight;
			}
			set {
				if (FirstVisibleLine != value) {
					textArea.VirtualTop = new Point(textArea.VirtualTop.X, value * fontHeight);
				}
			}
		}
		
		public int VisibleLineDrawingRemainder {
			get {
				return textArea.VirtualTop.Y % fontHeight;
			}
		}
		
		public int FontHeight {
			get {
				return fontHeight;
			}
		}
		
		public int VisibleLineCount {
			get {
				return 1 + DrawingPosition.Height / fontHeight;
			}
		}
		
		public int VisibleColumnCount {
			get {
				return (int)(DrawingPosition.Width / GetWidth(' ')) - 1;
			}
		}
		
		public TextView(TextArea textArea) : base(textArea)
		{
			measureStringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox |
			                                  StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
			
			OptionsChanged();
		}
		
		public void OptionsChanged()
		{
			this.fontHeight = TextEditorProperties.Font.Height;
			this.charWitdh  = new Hashtable();
		}
		
#region Paint functions
		public override void Paint(Graphics g, Rectangle rect)
		{
			int horizontalDelta = (int)(textArea.VirtualTop.X * GetWidth(g, ' '));
			if (horizontalDelta > 0) {
				g.SetClip(this.DrawingPosition);
			}
			
			for (int y = 0; y < (DrawingPosition.Height + VisibleLineDrawingRemainder) / fontHeight + 1; ++y) {
				Rectangle lineRectangle = new Rectangle(DrawingPosition.X - horizontalDelta,
				                                        DrawingPosition.Top + y * fontHeight - VisibleLineDrawingRemainder,
				                                        DrawingPosition.Width + horizontalDelta,
				                                        fontHeight);
				
				if (rect.IntersectsWith(lineRectangle)) {
					int currentLine = FirstVisibleLine + y;
					PaintDocumentLine(g, currentLine, lineRectangle);
				}
			}
			
			if (horizontalDelta > 0) {
				g.ResetClip();
			}
		}
		
		void PaintDocumentLine(Graphics g, int lineNumber, Rectangle lineRectangle)
		{
			HighlightBackground background = (HighlightBackground)textArea.Document.HighlightingStrategy.GetColorFor("DefaultColor");
			Brush               backgroundBrush = textArea.Enabled ? new SolidBrush(background.BackgroundColor) : SystemBrushes.InactiveBorder;
			
			if (lineNumber >= textArea.Document.TotalNumberOfLines) {
				g.FillRectangle(backgroundBrush, lineRectangle);
				if (TextEditorProperties.ShowInvalidLines) {
					DrawInvalidLineMarker(g, lineRectangle.Left, lineRectangle.Top);
				}
				if (TextEditorProperties.ShowVerticalRuler) {
					DrawVerticalRuler(g, lineRectangle);
				}
				return;
			}
			HighlightColor selectionColor = textArea.Document.HighlightingStrategy.GetColorFor("Selection");
			ColumnRange    selectionRange = textArea.SelectionManager.GetSelectionAtLine(lineNumber);
			HighlightColor defaultColor = textArea.Document.HighlightingStrategy.GetColorFor("DefaultColor");
			HighlightColor tabMarkerColor   = textArea.Document.HighlightingStrategy.GetColorFor("TabMarker");
			HighlightColor spaceMarkerColor = textArea.Document.HighlightingStrategy.GetColorFor("SpaceMarker");
			
			float       spaceWidth   = GetWidth(g, ' ');
			
			int         logicalColumn  = 0;
			int         physicalColumn = 0;
			
			float       physicalXPos   = lineRectangle.X;
			LineSegment currentLine    = textArea.Document.GetLineSegment(lineNumber);
			
			if (currentLine.Words != null) {
				for (int i = 0; i <= currentLine.Words.Count + 1; ++i) {
					// needed to draw fold markers beyond the logical end of line
					if (i >= currentLine.Words.Count) {
						++logicalColumn;
						continue;
					}
					
					TextWord currentWord = ((TextWord)currentLine.Words[i]);
					switch (currentWord.Type) {
						case TextWordType.Space:
							if (ColumnRange.WholeColumn.Equals(selectionRange) || logicalColumn >= selectionRange.StartColumn && logicalColumn <= selectionRange.EndColumn - 1) {
								g.FillRectangle(new SolidBrush(selectionColor.BackgroundColor),
								                new RectangleF(physicalXPos, lineRectangle.Y, spaceWidth, lineRectangle.Height));
							} else {
								g.FillRectangle(backgroundBrush,
								                new RectangleF(physicalXPos, lineRectangle.Y, spaceWidth, lineRectangle.Height));
							}
							if (TextEditorProperties.ShowSpaces) {
								DrawSpaceMarker(g, spaceMarkerColor.Color, physicalXPos, lineRectangle.Y);
							}
							
							physicalXPos += spaceWidth;
							
							++logicalColumn;
							++physicalColumn;
							break;
						
						case TextWordType.Tab:
							int oldPhysicalColumn = physicalColumn;
							physicalColumn += TextEditorProperties.TabIndent;
							physicalColumn = (physicalColumn / TextEditorProperties.TabIndent) * TextEditorProperties.TabIndent;
							
							float tabWidth = (physicalColumn - oldPhysicalColumn) * spaceWidth;
							
							if (ColumnRange.WholeColumn.Equals(selectionRange) || logicalColumn >= selectionRange.StartColumn && logicalColumn <= selectionRange.EndColumn - 1) {
								g.FillRectangle(new SolidBrush(selectionColor.BackgroundColor),
								                new RectangleF(physicalXPos, lineRectangle.Y, tabWidth, lineRectangle.Height));
							} else {
								g.FillRectangle(backgroundBrush,
								                new RectangleF(physicalXPos, lineRectangle.Y, tabWidth, lineRectangle.Height));
							}
							if (TextEditorProperties.ShowTabs) {
								DrawTabMarker(g, tabMarkerColor.Color, physicalXPos, lineRectangle.Y);
							}
							
							physicalXPos += tabWidth;
							
							++logicalColumn;
							break;
						
						case TextWordType.Word:
							string word    = currentWord.Word;
							float  lastPos = physicalXPos;
							
							if (ColumnRange.WholeColumn.Equals(selectionRange) || selectionRange.EndColumn - 1  >= word.Length + logicalColumn &&
							                                                      selectionRange.StartColumn <= logicalColumn) {
								physicalXPos += DrawDocumentWord(g,
								                                      word,
								                                      new PointF(physicalXPos, lineRectangle.Y),
								                                      currentWord.Font,
								                                      selectionColor.HasForgeground ? selectionColor.Color : currentWord.Color,
								                                      new SolidBrush(selectionColor.BackgroundColor));
							} else {
								if (ColumnRange.NoColumn.Equals(selectionRange)  /* || selectionRange.StartColumn > logicalColumn + word.Length || selectionRange.EndColumn  - 1 <= logicalColumn */) {
									physicalXPos += DrawDocumentWord(g,
									                                      word,
									                                      new PointF(physicalXPos, lineRectangle.Y),
									                                      currentWord.Font,
									                                      currentWord.Color,
									                                      backgroundBrush);
								} else {
									int offset1 = Math.Min(word.Length, Math.Max(0, selectionRange.StartColumn - logicalColumn ));
									int offset2 = Math.Max(offset1, Math.Min(word.Length, selectionRange.EndColumn - logicalColumn));
									
									string word1 = word.Substring(0, offset1);
									string word2 = word.Substring(offset1, offset2 - offset1);
									string word3 = word.Substring(offset2);
									
									physicalXPos += DrawDocumentWord(g,
									                                      word1,
									                                      new PointF(physicalXPos, lineRectangle.Y),
									                                      currentWord.Font,
									                                      currentWord.Color,
									                                      backgroundBrush);
									physicalXPos += DrawDocumentWord(g,
									                                      word2,
									                                      new PointF(physicalXPos, lineRectangle.Y),
									                                      currentWord.Font,
									                                      selectionColor.HasForgeground ? selectionColor.Color : currentWord.Color,
									                                      new SolidBrush(selectionColor.BackgroundColor));
							
									physicalXPos += DrawDocumentWord(g,
									                                      word3,
									                                      new PointF(physicalXPos, lineRectangle.Y),
									                                      currentWord.Font,
									                                      currentWord.Color,
									                                      backgroundBrush);
								}
							}
							
							// draw bracket highlight
							if (highlight != null) {
								if (highlight.OpenBrace.Y == lineNumber && highlight.OpenBrace.X == logicalColumn ||
								    highlight.CloseBrace.Y == lineNumber && highlight.CloseBrace.X == logicalColumn) {
									DrawBracketHighlight(g, new Rectangle((int)lastPos, lineRectangle.Y, (int)(physicalXPos - lastPos) - 1, lineRectangle.Height - 1));
								}
							}
							physicalColumn += word.Length;
							logicalColumn += word.Length;
							break;
					}
				}
			}
			
			bool selectionBeyondEOL = selectionRange.EndColumn > currentLine.Length || ColumnRange.WholeColumn.Equals(selectionRange);
			
				
			if (TextEditorProperties.ShowEOLMarker) {
				HighlightColor eolMarkerColor = textArea.Document.HighlightingStrategy.GetColorFor("EolMarker");
				// selectionBeyondEOL ? selectionColor.Color: eolMarkerColor.Color
				physicalXPos += DrawEOLMarker(g, eolMarkerColor.Color, selectionBeyondEOL ? new SolidBrush(selectionColor.BackgroundColor) : backgroundBrush, physicalXPos, lineRectangle.Y);
			} else {
				if (selectionBeyondEOL && !TextEditorProperties.AllowCaretBeyondEOL) {
					g.FillRectangle(new SolidBrush(selectionColor.BackgroundColor),
					                new RectangleF(physicalXPos, lineRectangle.Y, spaceWidth, lineRectangle.Height));
			
					physicalXPos += spaceWidth;
				}
			}
			
			g.FillRectangle(selectionBeyondEOL && TextEditorProperties.AllowCaretBeyondEOL ? new SolidBrush(selectionColor.BackgroundColor) : backgroundBrush, 
			                new RectangleF(physicalXPos, lineRectangle.Y, lineRectangle.Width - physicalXPos + lineRectangle.X, lineRectangle.Height));
			
			if (TextEditorProperties.ShowVerticalRuler) {
				DrawVerticalRuler(g, lineRectangle);
			}
		}
		
		float DrawDocumentWord(Graphics g, string word, PointF position, Font font, Color foreColor, Brush backBrush)
		{
			if (word == null || word.Length == 0) {
				return 0f;
			}
			float wordWidth = g.MeasureString(word, font, 32768, measureStringFormat).Width;
			g.FillRectangle(backBrush,
			                new RectangleF(position.X, position.Y, wordWidth, FontHeight));
			
			g.DrawString(word,
			             font,
			             new SolidBrush(foreColor),
			             position.X,
			             position.Y, 
			             measureStringFormat);
			
			return wordWidth;
		}
#endregion
		
#region Conversion Functions
		public float GetWidth(char ch)
		{
			object width = charWitdh[ch];
			if (width == null) {
				Graphics g = textArea.CreateGraphics();
				width = GetWidth(g, ch);
				g.Dispose();
			}
			return (float)width;
		}
		
		public float GetWidth(Graphics g, char ch)
		{
			object width = charWitdh[ch];
			if (width == null) {
				charWitdh[ch] = g.MeasureString(ch.ToString(), TextEditorProperties.Font, 2000, measureStringFormat).Width;
				return (float)charWitdh[ch];
			}
			return (float)width;
		}
		
		public int GetVisualColumn(int logicalLine, int logicalColumn)
		{
			return GetVisualColumn(Document.GetLineSegment(logicalLine), logicalColumn);
		}
		public int GetVisualColumn(LineSegment line, int logicalColumn)
		{
			int tabIndent = Document.TextEditorProperties.TabIndent;
			int column    = 0;
			for (int i = 0; i < logicalColumn; ++i) {
				char ch;
				if (i >= line.Length) {
					ch = ' ';
				} else {
					ch = Document.GetCharAt(line.Offset + i);
				}
				
				switch (ch) {
					case '\t':
						int oldColumn = column;
						column += tabIndent;
						column = (column / tabIndent) * tabIndent;
						break;
					default:
						++column;
						break;
				}
			}
			return column;
		}
		
		/// <summary>
		/// returns line/column for a visual point position
		/// </summary>
		public Point GetLogicalPosition(int xPos, int yPos)
		{
			xPos += (int)(textArea.VirtualTop.X * GetWidth(' '));
			int clickedVisualLine = (yPos + this.textArea.VirtualTop.Y) / fontHeight;
			int logicalLine       = clickedVisualLine; // todo : folding
			
			return new Point(GetLogicalColumn(logicalLine < Document.TotalNumberOfLines ? Document.GetLineSegment(logicalLine) : null, xPos), 
			                 logicalLine);
		}
		
		int GetLogicalColumn(LineSegment line, int xPos)
		{
			if (line == null) {
				return (int)(xPos / GetWidth(' '));
			}
			
			int tabIndent  = Document.TextEditorProperties.TabIndent;
			int column     = 0;
			int logicalColumn = 0;
			float paintPos = 0;
			
			for (int i = 0; i < line.Length; ++i) {
				char ch = Document.GetCharAt(line.Offset + i);
				float oldPaintPos = paintPos;
				switch (ch) {
					case '\t':
						int oldColumn = column;
						column += tabIndent;
 						column = (column / tabIndent) * tabIndent;
						paintPos += (column - oldColumn) * GetWidth(' ');
						break;
					default:
						paintPos += GetWidth(ch);
						++column;
						break;
				}
				++logicalColumn;
				if (xPos <= paintPos - (paintPos - oldPaintPos) / 2) {
					return i;
				}
			}
			return (int)(logicalColumn + (xPos - paintPos) / GetWidth(' '));
		}
		
		public int GetDrawingXPos(int logicalLine, int logicalColumn)
		{
			return GetDrawingXPos(Document.GetLineSegment(logicalLine), logicalColumn);
		}
		
		public int GetDrawingXPos(LineSegment line, int logicalColumn)
		{
			int tabIndent  = Document.TextEditorProperties.TabIndent;
			int column     = 0;
			float drawingPos = 0;
			for (int i = 0; i < logicalColumn; ++i) {
				char ch;
				if (i >= line.Length) {
					ch = ' ';
				} else {
					ch = Document.GetCharAt(line.Offset + i);
				}
				
				switch (ch) {
					case '\t':
						int oldColumn = column;
						column += tabIndent;
						column = (column / tabIndent) * tabIndent;
						
						drawingPos += (column - oldColumn) * this.GetWidth(' ');
						break;
					default:
						drawingPos += this.GetWidth(ch);
						++column;
						break;
				}
			}
			return (int)(drawingPos - textArea.VirtualTop.X * GetWidth(' '));
		}
#endregion
		
#region DrawHelper functions
		void DrawBracketHighlight(Graphics g, Rectangle rect)
		{
			g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 255)),
			                rect);
			g.DrawRectangle(new Pen(Color.Blue),
			                rect);
		}
		
		void DrawInvalidLineMarker(Graphics g, float x, float y)
		{
			HighlightColor invalidLinesColor = textArea.Document.HighlightingStrategy.GetColorFor("InvalidLines");
			g.DrawString("~", invalidLinesColor.Font, new SolidBrush(invalidLinesColor.Color), x, y, measureStringFormat);
		}
		
		void DrawSpaceMarker(Graphics g, Color color, float x, float y)
		{
			HighlightColor spaceMarkerColor = textArea.Document.HighlightingStrategy.GetColorFor("SpaceMarker");
			g.DrawString("\u00B7", spaceMarkerColor.Font, new SolidBrush(color), x, y, measureStringFormat);
		}
		
		void DrawTabMarker(Graphics g, Color color, float x, float y)
		{
			HighlightColor tabMarkerColor   = textArea.Document.HighlightingStrategy.GetColorFor("TabMarker");
			g.DrawString("\u00BB", tabMarkerColor.Font, new SolidBrush(color), x, y, measureStringFormat);
		}
		
		float DrawEOLMarker(Graphics g, Color color, Brush backBrush, float x, float y)
		{
			float width = GetWidth(g, '\u00B6');
			g.FillRectangle(backBrush,
			                new RectangleF(x, y, width, fontHeight));
			
			HighlightColor eolMarkerColor = textArea.Document.HighlightingStrategy.GetColorFor("EolMarker");
			g.DrawString("\u00B6", eolMarkerColor.Font, new SolidBrush(color), x, y, measureStringFormat);
			return width;
		}
		
		void DrawVerticalRuler(Graphics g, Rectangle lineRectangle)
		{
			if (TextEditorProperties.VerticalRulerRow < textArea.VirtualTop.X) {
				return;
			}
			HighlightColor vRulerColor = textArea.Document.HighlightingStrategy.GetColorFor("VRulerColor");
			
			int xpos = (int)(drawingPosition.Left + GetWidth(g, ' ') * (TextEditorProperties.VerticalRulerRow - textArea.VirtualTop.X));
			g.DrawLine(new Pen(vRulerColor.Color),
			           xpos,
			           lineRectangle.Top,
			           xpos,
			           lineRectangle.Bottom);
		}
#endregion
	}
}
