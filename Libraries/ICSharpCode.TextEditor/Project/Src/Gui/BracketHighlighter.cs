﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	public class Highlight
	{
		Point openBrace;
		Point closeBrace;
		
		public Point OpenBrace {
			get {
				return openBrace;
			}
			set {
				openBrace = value;
			}
		}
		public Point CloseBrace {
			get {
				return closeBrace;
			}
			set {
				closeBrace = value;
			}
		}
		
		public Highlight(Point openBrace, Point closeBrace)
		{
			this.openBrace = openBrace;
			this.closeBrace = closeBrace;
		}
	}
	
	public class BracketHighlightingSheme
	{
		char opentag;
		char closingtag;
		
		public char OpenTag {
			get {
				return opentag;
			}
			set {
				opentag = value;
			}
		}
		
		public char ClosingTag {
			get {
				return closingtag;
			}
			set {
				closingtag = value;
			}
		}
		
		public BracketHighlightingSheme(char opentag, char closingtag)
		{
			this.opentag    = opentag;
			this.closingtag = closingtag;
		}
		
		public Highlight GetHighlight(IDocument document, int offset)
		{
			int searchOffset;
			if (document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After) {
				searchOffset = offset;
			} else {
				searchOffset = offset + 1;
			}
			char word = document.GetCharAt(Math.Max(0, Math.Min(document.TextLength - 1, searchOffset)));
			
			Point endP = document.OffsetToPosition(offset);
			if (word == opentag) {
				if (offset < document.TextLength) {
					int bracketOffset = TextUtilities.SearchBracketForward(document, searchOffset + 1, opentag, closingtag);
					if (bracketOffset >= 0) {
						Point p = document.OffsetToPosition(bracketOffset);
						return new Highlight(p, endP);
					}
				}
			} else if (word == closingtag) {
				if (offset > 0) {
					int bracketOffset = TextUtilities.SearchBracketBackward(document, searchOffset - 1, opentag, closingtag);
					if (bracketOffset >= 0) {
						Point p = document.OffsetToPosition(bracketOffset);
						return new Highlight(p, endP);
					}
				}
			}
			return null;
		}
	}
}
