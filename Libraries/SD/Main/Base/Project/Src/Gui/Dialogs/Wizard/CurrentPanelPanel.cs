﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class CurrentPanelPanel : UserControl
	{
		WizardDialog wizard;
		
		Font normalFont;
		
		
		public CurrentPanelPanel(WizardDialog wizard)
		{
			normalFont = ResourceService.LoadFont("SansSerif", 18, GraphicsUnit.World);

			this.wizard = wizard;
			Size = new Size(wizard.Width - 220, 30);
			ResizeRedraw  = false;
			
			SetStyle(ControlStyles.UserPaint, true);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			//    		base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			//			g.FillRectangle(new SolidBrush(SystemColors.Control), pe.ClipRectangle);
			
			using (Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
			                                             Color.White,
			                                             SystemColors.Control)) {
				g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			//    		base.OnPaint(pe);
			Graphics g = pe.Graphics;
			g.DrawString(((IDialogPanelDescriptor)wizard.WizardPanels[wizard.ActivePanelNumber]).Label, normalFont, Brushes.Black,
			             10,
			             24 - normalFont.Height,
			             StringFormat.GenericTypographic);
			g.DrawLine(Pens.Black, 10, 24, Width - 10, 24);
		}
	}
}
