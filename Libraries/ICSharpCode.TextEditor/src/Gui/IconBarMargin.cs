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
	public class IconBarMargin : AbstractMargin
	{
		public override Size Size {
			get {
				return new Size((int)(textArea.TextView.FontHeight * 1.2f),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.IsIconBarVisible;
			}
		}
		
		
		public IconBarMargin(TextArea textArea) : base(textArea)
		{
		}
		
		public override void Paint(Graphics g, Rectangle rect)
		{
			// paint background
			g.FillRectangle(SystemBrushes.Control, new Rectangle(drawingPosition.X, rect.Top, drawingPosition.Width - 1, rect.Height));
			g.DrawLine(SystemPens.ControlDark, base.drawingPosition.Right - 1, rect.Top, base.drawingPosition.Right - 1, rect.Bottom);
			
			// paint icons
			foreach (int mark in textArea.Document.BookmarkManager.Marks) {
				int lineNumber = textArea.Document.GetLogicalLine(mark);
				int yPos = (int)(lineNumber * textArea.TextView.FontHeight) - textArea.VirtualTop.Y;
//				if (yPos >= rect.Y && yPos <= rect.Bottom) {
					DrawBookmark(g, yPos);
//				}
			}
		}
		
#region Drawing functions
		void DrawBookmark(Graphics g, int y)
		{
			int delta = textArea.TextView.FontHeight / 8;
			Rectangle rect = new Rectangle( 1, y + delta, base.drawingPosition.Width - 4, textArea.TextView.FontHeight - delta * 2);
			FillRoundRect(g, Brushes.Cyan, rect);
			DrawRoundRect(g, Pens.Black, rect);
		}
		
		GraphicsPath CreateRoundRectGraphicsPath(Rectangle r)
		{
			GraphicsPath gp = new GraphicsPath();
			int radius = r.Width / 2;
			gp.AddLine(r.X + radius, r.Y, r.Right - radius, r.Y);
			gp.AddArc(r.Right - radius, r.Y, radius, radius, 270, 90);
			
			gp.AddLine(r.Right, r.Y + radius, r.Right, r.Bottom - radius);
			gp.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0, 90);
			
			gp.AddLine(r.Right - radius, r.Bottom, r.X + radius, r.Bottom);
			gp.AddArc(r.X, r.Bottom - radius, radius, radius, 90, 90);
			
			gp.AddLine(r.X, r.Bottom - radius, r.X, r.Y + radius);
			gp.AddArc(r.X, r.Y, radius, radius, 180, 90);
			
			gp.CloseFigure();
			return gp;
		}
		
		void DrawRoundRect(Graphics g, Pen p , Rectangle r)
		{
			GraphicsPath gp = CreateRoundRectGraphicsPath(r);
			g.DrawPath(p, gp);
			gp.Dispose();
		}

		void FillRoundRect(Graphics g, Brush b , Rectangle r)
		{
			GraphicsPath gp = CreateRoundRectGraphicsPath(r);
			g.FillPath(b, gp);
			gp.Dispose();
		}
#endregion
	}
}
