// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions 
{
	public class Home : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			LineSegment curLine;
			Point       newPos = textArea.Caret.Position;
			bool        jumpedIntoFolding = false;
			do {
				curLine = textArea.Document.GetLineSegment(newPos.Y);
						
				if (TextUtilities.IsEmptyLine(textArea.Document, newPos.Y)) {
					if (newPos.X != 0) {
						newPos.X = 0;
					} else  {
						newPos.X = curLine.Length;
					}
				} else {
					int firstCharOffset = TextUtilities.GetFirstNonWSChar(textArea.Document, curLine.Offset);
					int firstCharColumn = firstCharOffset - curLine.Offset;
					
					if (newPos.X == firstCharColumn) {
						newPos.X = 0;
					} else {
						newPos.X = firstCharColumn;
					}
				}
				ArrayList foldings = textArea.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
				jumpedIntoFolding = false;
				foreach (FoldMarker foldMarker in foldings) {
					if (foldMarker.IsFolded) {
						newPos = new Point(foldMarker.StartColumn, foldMarker.StartLine);
						jumpedIntoFolding = true;
						break;
					}
				}
				
			} while (jumpedIntoFolding);
			
			if (newPos != textArea.Caret.Position) {
				textArea.Caret.Position = newPos;
				textArea.SetDesiredColumn();
			}
		}
	}
	
	public class End : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			LineSegment curLine;
			Point       newPos = textArea.Caret.Position;
			bool        jumpedIntoFolding = false;
			do {
				curLine  = textArea.Document.GetLineSegment(newPos.Y);
				newPos.X = curLine.Length;
				
				ArrayList foldings = textArea.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
				jumpedIntoFolding = false;
				foreach (FoldMarker foldMarker in foldings) {
					if (foldMarker.IsFolded) {
						newPos = new Point(foldMarker.EndColumn, foldMarker.EndLine);
						jumpedIntoFolding = true;
						break;
					}
				}
			} while (jumpedIntoFolding);
			
			if (newPos != textArea.Caret.Position) {
				textArea.Caret.Position = newPos;
				textArea.SetDesiredColumn();
			}
		}
	}
	
	
	public class MoveToStart : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			if (textArea.Caret.Line != 0 || textArea.Caret.Column != 0) {
				textArea.Caret.Position = new Point(0, 0);
				textArea.SetDesiredColumn();
			}
		}
	}
	
	
	public class MoveToEnd : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			Point endPos = textArea.Document.OffsetToPosition(textArea.Document.TextLength);
			if (textArea.Caret.Position != endPos) {
				textArea.Caret.Position = endPos;
				textArea.SetDesiredColumn();
			}
		}
	}
}
