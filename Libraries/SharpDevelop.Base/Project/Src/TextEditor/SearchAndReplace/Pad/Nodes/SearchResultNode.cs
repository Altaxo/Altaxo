﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchResultNode : ExtTreeNode
	{
		SearchResult result;
		
		Point startPosition;
		string positionText;
		string displayText;
		string specialText;
		bool showFileName = false;
		DrawableLine drawableLine;
		
		public bool ShowFileName {
			get {
				return showFileName;
			}
			set {
				showFileName = value;
				if (showFileName) {
					Text = displayText + FileNameText;
				} else {
					Text = displayText;
				}
			}
		}
		
		string FileNameText {
			get {
				return StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ")
					+ Path.GetFileName(result.FileName) + "(" + Path.GetDirectoryName(result.FileName) +")";
			}
		}
		
		public SearchResultNode(IDocument document, SearchResult result)
		{
			drawDefault = false;
			this.result = result;
			startPosition = result.GetStartPosition(document);
			Point endPosition = result.GetEndPosition(document);
			positionText =  "(" + (startPosition.Y + 1) + ", " + (startPosition.X + 1) + ") ";
			
			LineSegment line = document.GetLineSegment(startPosition.Y);
			drawableLine = new DrawableLine(document, line, MonospacedFont, BoldMonospacedFont);
			drawableLine.SetBold(0, drawableLine.LineLength, false);
			if (startPosition.Y == endPosition.Y) {
				drawableLine.SetBold(startPosition.X, endPosition.X, true);
			}
			
			specialText = result.DisplayText;
			if (specialText != null) {
				displayText = positionText + specialText;
			} else {
				displayText = positionText + document.GetText(line).Replace("\t", "    ");
			}
			Text = displayText;
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, displayText, BoldMonospacedFont);
			if (ShowFileName) {
				float tabWidth = drawableLine.GetSpaceSize(g).Width * 6;
				x = (int)((int)((x + 2 + tabWidth) / tabWidth) * tabWidth);
				x += MeasureTextWidth(g, FileNameText, ItalicFont);
			}
			return x;
		}
		
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(g, positionText, Brushes.Black, Font, ref x, e.Bounds.Y);
			
			if (specialText != null) {
				DrawText(g, specialText, Brushes.Black, Font, ref x, e.Bounds.Y);
			} else {
				x -= e.Bounds.X;
				drawableLine.DrawLine(g, ref x, e.Bounds.X, e.Bounds.Y, GetTextColor(e.State, Color.Empty));
			}
			if (ShowFileName) {
				float tabWidth = drawableLine.GetSpaceSize(g).Width * 6;
				x = (int)((int)((x + 2 + tabWidth) / tabWidth) * tabWidth);
				x += e.Bounds.X;
				DrawText(g,
				         FileNameText,
				         Brushes.Gray,
				         ItalicFont,
				         ref x, e.Bounds.Y);
			}
		}
		
		public override void ActivateItem()
		{
			FileService.JumpToFilePosition(result.FileName, startPosition.Y, startPosition.X);
		}
	}
}
