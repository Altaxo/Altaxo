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

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class AxStatusBarPanel : StatusBarPanel
	{
		StringFormat sFormat = new StringFormat();
		
		public AxStatusBarPanel()
		{
			Style       = StatusBarPanelStyle.OwnerDraw;
			BorderStyle = StatusBarPanelBorderStyle.None;
			
			sFormat.LineAlignment = StringAlignment.Center;
		}
		
		protected virtual void DrawBorder(StatusBarDrawItemEventArgs drawEventArgs)
		{
			drawEventArgs.Graphics.DrawRectangle(SystemPens.ControlDark, 
			                                     new Rectangle(drawEventArgs.Bounds.X,
			                                                   drawEventArgs.Bounds.Y,
			                                                   drawEventArgs.Bounds.Width - 1,
			                                                   drawEventArgs.Bounds.Height - 1));
		}
				
		public virtual void DrawPanel(StatusBarDrawItemEventArgs drawEventArgs)
		{
			Graphics g = drawEventArgs.Graphics;
			switch (Alignment) {
				case HorizontalAlignment.Left:
					sFormat.Alignment = StringAlignment.Near;
					break;
				case HorizontalAlignment.Center:
					sFormat.Alignment = StringAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					sFormat.Alignment = StringAlignment.Far;
					break;
			}
			g.DrawString(Text,
			             drawEventArgs.Font,
			             SystemBrushes.ControlText, 
			             drawEventArgs.Bounds,
			             sFormat);
			DrawBorder(drawEventArgs);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				sFormat.Dispose();
			}
		}
	}
}
