// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class handles all mouse stuff for a textArea.
	/// </summary>
	public class TextAreaMouseHandler
	{
		
		TextArea  textArea;
		bool      doubleclick = false;
	    Point     mousepos = new Point(0, 0);
		int       selbegin;
		int       selend;
		bool      clickedOnSelectedText = false;
		
		MouseButtons button;
		
		Point mousedownpos = new Point(-1, -1);
		bool gotmousedown = false;
		bool dodragdrop = false;
		
		public TextAreaMouseHandler(TextArea textArea)
		{
			this.textArea = textArea;
		}
		
		public void Attach()
		{
			textArea.Click       += new EventHandler(TextAreaClick);
			textArea.MouseMove   += new MouseEventHandler(TextAreaMouseMove);
			
			textArea.MouseDown   += new MouseEventHandler(OnMouseDown);
			textArea.DoubleClick += new EventHandler(OnDoubleClick);
			textArea.MouseLeave  += new EventHandler(OnMouseLeave);
			textArea.MouseUp     += new MouseEventHandler(OnMouseUp);
			textArea.LostFocus   += new EventHandler(TextAreaLostFocus);
		}
		
		void ShowHiddenCursor()
		{
			if (TextArea.HiddenMouseCursor) {
				Cursor.Show();
				TextArea.HiddenMouseCursor = false;
			}
		}
		
		void TextAreaLostFocus(object sender, EventArgs e)
		{
			ShowHiddenCursor();
		}
		void OnMouseLeave(object sender, EventArgs e)
		{
			ShowHiddenCursor();
			gotmousedown = false;
			mousedownpos = new Point(-1, -1);
		}
		
		void OnMouseUp(object sender, MouseEventArgs e)
		{
			gotmousedown = false;
			mousedownpos = new Point(-1, -1);
		}
		
		void TextAreaClick(object sender, EventArgs e)
		{
			if (dodragdrop) {
				return;
			}
			
			if (clickedOnSelectedText && textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				textArea.SelectionManager.ClearSelection();
				
				Point clickPosition = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X, mousepos.Y - textArea.TextView.DrawingPosition.Y);
				textArea.Caret.Position = clickPosition;
				textArea.SetDesiredColumn();
			}
		}
		
		void TextAreaMouseMove(object sender, MouseEventArgs e)
		{
			ShowHiddenCursor();
			if (dodragdrop) {
				dodragdrop = false;
				return;
			}
			
			doubleclick = false;
			mousepos    = new Point(e.X, e.Y);
			
			if (textArea.GutterMargin.DrawingPosition.Contains(mousepos.X, mousepos.Y) || textArea.FoldMargin.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				if (e.Button == MouseButtons.Left) {
					Point realmousepos = textArea.TextView.GetLogicalPosition(0, mousepos.Y - textArea.TextView.DrawingPosition.Y);
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
				}
			} else if (textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				if (clickedOnSelectedText) {
					if (Math.Abs(mousedownpos.X - mousepos.X) >= SystemInformation.DragSize.Width / 2 ||
					    Math.Abs(mousedownpos.Y - mousepos.Y) >= SystemInformation.DragSize.Height / 2) {
						clickedOnSelectedText = false;
						ISelection selection = textArea.SelectionManager.GetSelectionAt(textArea.Caret.Offset);
						if (selection != null) {
							string text = selection.SelectedText;
							if (text != null && text.Length > 0) {
								DataObject dataObject = new DataObject ();
								dataObject.SetData(DataFormats.UnicodeText, true, text);
								dataObject.SetData(selection);
								dodragdrop = true;
								textArea.DoDragDrop(dataObject, DragDropEffects.All);
							}
						}
					}
					
					return;
				}
				
				if (e.Button == MouseButtons.Left) {
					if  (gotmousedown) {
						ExtendSelectionToMouse();
					}
				}
			}
		}
		
		void ExtendSelectionToMouse()
		{
			Point realmousepos = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X,
			                                                          mousepos.Y - textArea.TextView.DrawingPosition.Y);
			Point oldPos = textArea.Caret.Position;
			textArea.Caret.Position = realmousepos;
			textArea.SelectionManager.ExtendSelection(oldPos, textArea.Caret.Position);
			textArea.SetDesiredColumn();		
		}
		Point selectionStartPos  = new Point(-1, -1);
		void OnMouseDown(object sender, MouseEventArgs e)
		{ 
			if (dodragdrop) {
				return;
			}
			
			if (doubleclick) {
				doubleclick = false;
				return;
			}
			
			gotmousedown = true;
			mousedownpos = new Point(e.X, e.Y);
			
			button = e.Button;
			
			if (textArea.GutterMargin.DrawingPosition.Contains(mousepos.X, mousepos.Y) || textArea.FoldMargin.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				Point realmousepos = textArea.TextView.GetLogicalPosition(0, mousepos.Y - textArea.TextView.DrawingPosition.Y);
				if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
					selectionStartPos = realmousepos;
					textArea.SelectionManager.ClearSelection();
					textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, realmousepos, new Point(textArea.Document.GetLineSegment(realmousepos.Y).Length + 1, realmousepos.Y)));
					textArea.Caret.Position = realmousepos;
				}
			} else if (textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				if (button == MouseButtons.Left) {
					if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) {
						ExtendSelectionToMouse();
					} else {
						Point realmousepos = textArea.TextView.GetLogicalPosition(mousepos.X - textArea.TextView.DrawingPosition.X, mousepos.Y - textArea.TextView.DrawingPosition.Y);
						clickedOnSelectedText = false;
						
						int offset = textArea.Document.PositionToOffset(realmousepos);
								
						
						if (textArea.SelectionManager.HasSomethingSelected && 
						    textArea.SelectionManager.IsSelected(offset)) {	
							clickedOnSelectedText = true;
						} else {
							selbegin = selend = offset;
							textArea.SelectionManager.ClearSelection();
							if (mousepos.Y > 0 && mousepos.Y < textArea.TextView.DrawingPosition.Height) {
								Point pos = new Point();
								pos.Y = Math.Min(textArea.Document.TotalNumberOfLines - 1,  realmousepos.Y);
								pos.X = realmousepos.X;
								textArea.Caret.Position = pos;//Math.Max(0, Math.Min(textArea.Document.TextLength, line.Offset + Math.Min(line.Length, pos.X)));
								textArea.SetDesiredColumn();
							}
						}
					}
				}
			}
			textArea.Focus();
		}
		
		int FindNext(IDocument document, int offset, char ch)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			int         endPos = line.Offset + line.Length;
			
			while (offset < endPos && document.GetCharAt(offset) != ch) {
				++offset;
			}
			return offset;
		}
		
		bool IsSelectableChar(char ch)
		{
			return Char.IsLetterOrDigit(ch) || ch=='_';
		}
		
		int FindWordStart(IDocument document, int offset)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			
			if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && Char.IsWhiteSpace(document.GetCharAt(offset))) {
				while (offset > line.Offset && Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
					--offset;
				}
			} else  if (IsSelectableChar(document.GetCharAt(offset)) || (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset)) && IsSelectableChar(document.GetCharAt(offset - 1))))  {
				while (offset > line.Offset && IsSelectableChar(document.GetCharAt(offset - 1))) {
					--offset;
				}
			} else {
				if (offset > 0 && !Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && !IsSelectableChar(document.GetCharAt(offset - 1)) ) {
					return Math.Max(0, offset - 1);
				}
			}
			return offset;
		}
		
		int FindWordEnd(IDocument document, int offset)
		{
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int         endPos = line.Offset + line.Length;
			
			if (IsSelectableChar(document.GetCharAt(offset)))  {
				while (offset < endPos && IsSelectableChar(document.GetCharAt(offset))) {
					++offset;
				}
			} else if (Char.IsWhiteSpace(document.GetCharAt(offset))) {
				if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
					while (offset < endPos && Char.IsWhiteSpace(document.GetCharAt(offset))) {
						++offset;
					}
				}
			} else {
				return Math.Max(0, offset + 1);
			}
			
			return offset;
		}
		
		void OnDoubleClick(object sender, System.EventArgs e)
		{
			if (dodragdrop) {
				return;
			}
			
			doubleclick = true;
			
			textArea.SelectionManager.ClearSelection();
			if (textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y)) {
				if (textArea.Caret.Offset < textArea.Document.TextLength) {
					switch (textArea.Document.GetCharAt(textArea.Caret.Offset)) {
						case '"':
							if (textArea.Caret.Offset < textArea.Document.TextLength) {
								int next = FindNext(textArea.Document, textArea.Caret.Offset + 1, '"');
								textArea.SelectionManager.ExtendSelection(textArea.Caret.Position,
								                                          textArea.Document.OffsetToPosition(next > textArea.Caret.Offset ? next + 1 : next));
							}
							break;
						default:
							textArea.SelectionManager.ExtendSelection(textArea.Document.OffsetToPosition(FindWordStart(textArea.Document, textArea.Caret.Offset)),
							                                          textArea.Document.OffsetToPosition(FindWordEnd(textArea.Document, textArea.Caret.Offset)));
							break;
					
					}
					// HACK WARNING !!! 
					// must refresh here, because when a error tooltip is showed and the underlined
					// code is double clicked the textArea don't update corrctly, updateline doesn't
					// work ... but the refresh does.
					// Mike
					textArea.Refresh(); 
				}
			}
		}
	}
}
