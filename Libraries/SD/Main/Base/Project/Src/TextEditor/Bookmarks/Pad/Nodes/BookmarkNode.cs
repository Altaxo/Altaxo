﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2028 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// ExtTreeNode representing a bookmark.
	/// </summary>
	public class BookmarkNode : ExtTreeNode
	{
		SDBookmark bookmark;
		
		SizeF  spaceSize;
		static StringFormat sf = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();
		
		LineSegment line;
		
		string positionText;
		
		public SDBookmark Bookmark {
			get {
				return bookmark;
			}
		}
		
		public BookmarkNode(SDBookmark bookmark)
		{
			drawDefault = false;
			this.bookmark = bookmark;
			Tag = bookmark;
			Checked = bookmark.IsEnabled;
			positionText =  "(" + (bookmark.LineNumber + 1) + ") ";
			
			bookmark.DocumentChanged += BookmarkDocumentChanged;
			bookmark.LineNumberChanged += BookmarkLineNumberChanged;
			if (bookmark.Document != null) {
				BookmarkDocumentChanged(null, null);
			} else {
				Text = positionText;
			}
		}
		
		public override void CheckedChanged()
		{
			bookmark.IsEnabled = Checked;
		}
		
		void BookmarkDocumentChanged(object sender, EventArgs e)
		{
			if (bookmark.Document != null) {
				line = bookmark.Document.GetLineSegment(Math.Min(bookmark.LineNumber, bookmark.Document.TotalNumberOfLines));
				Text = positionText + bookmark.Document.GetText(line);
			}
		}
		
		void BookmarkLineNumberChanged(object sender, EventArgs e)
		{
			positionText =  "(" + (bookmark.LineNumber + 1) + ") ";
			BookmarkDocumentChanged(sender, e);
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, positionText, BoldMonospacedFont);
			if (line != null) {
				x += MeasureTextWidth(g, bookmark.Document.GetText(line).Replace("\t", "   "), BoldMonospacedFont);
			}
			return x;
		}
		
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(e, positionText, SystemBrushes.WindowText, RegularBigFont, ref x);
			
			spaceSize = g.MeasureString("-", RegularBigFont,  new PointF(0, 0), StringFormat.GenericTypographic);
			
			if (line != null) {
				DrawLine(g, line, e.Bounds.Y, x, e.State);
			}
		}
		
		public override void ActivateItem()
		{
			FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, 0);
		}
		
		float DrawDocumentWord(Graphics g, string word, PointF position, Font font, Color foreColor)
		{
			if (word == null || word.Length == 0) {
				return 0f;
			}
			SizeF wordSize = g.MeasureString(word, font, 32768, sf);
			
			g.DrawString(word,
			             font,
			             BrushRegistry.GetBrush(foreColor),
			             position,
			             sf);
			return wordSize.Width;
		}
		
		void DrawLine(Graphics g, LineSegment line, float yPos, float xPos, TreeNodeStates state)
		{
			int logicalX = 0;
			if (line.Words != null) {
				foreach (TextWord word in line.Words) {
					switch (word.Type) {
						case TextWordType.Space:
							xPos += spaceSize.Width;
							logicalX++;
							break;
						case TextWordType.Tab:
							xPos += spaceSize.Width * 4;
							logicalX++;
							break;
						case TextWordType.Word:
							xPos += DrawDocumentWord(g,
							                         word.Word,
							                         new PointF(xPos, yPos),
							                         word.Bold ? BoldMonospacedFont : RegularMonospacedFont,
							                         GetTextColor(state, word.Color)
							                        );
							logicalX += word.Word.Length;
							break;
					}
				}
			} else {
				DrawDocumentWord(g,
				                 bookmark.Document.GetText(line),
				                 new PointF(xPos, yPos),
				                 RegularMonospacedFont,
				                 GetTextColor(state, Color.Black)
				                );
			}
		}
	}
}
