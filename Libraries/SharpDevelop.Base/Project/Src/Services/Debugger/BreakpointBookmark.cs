// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1085 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.Core
{
	public class BreakpointBookmark: SDMarkerBookmark
	{
		bool willBeHit = true;
		
		public virtual bool WillBeHit {
			get {
				return willBeHit;
			}
			set {
				willBeHit = value;
				if (Document != null) {
					Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, LineNumber));
					Document.CommitUpdate();
				}
			}
		}
		
		public BreakpointBookmark(string fileName, IDocument document, int lineNumber) : base(fileName, document, lineNumber)
		{
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBreakpoint(g, p.Y, IsEnabled, WillBeHit);
		}
		
		protected override TextMarker CreateMarker()
		{
			if (LineNumber >= Document.TotalNumberOfLines)
				LineNumber = Document.TotalNumberOfLines - 1;
			LineSegment lineSeg = Document.GetLineSegment(LineNumber);
			TextMarker marker = new TextMarker(lineSeg.Offset, lineSeg.Length, TextMarkerType.SolidBlock, Color.Red, Color.White);
			Document.MarkerStrategy.AddMarker(marker);
			return marker;
		}
	}
}
