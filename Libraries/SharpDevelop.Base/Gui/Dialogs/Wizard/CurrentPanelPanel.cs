// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class CurrentPanelPanel : UserControl
	{
		WizardDialog wizard;
		Bitmap backGround = null;
		
		Font normalFont;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public CurrentPanelPanel(WizardDialog wizard)
		{
			normalFont = resourceService.LoadFont("SansSerif", 18, GraphicsUnit.World);

			this.wizard = wizard;
			backGround = resourceService.GetBitmap("GeneralWizardBackground");
			Size = new Size(wizard.Width - 220, 30);
			ResizeRedraw  = false;
			
			SetStyle(ControlStyles.UserPaint, true);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			//    		base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			//			g.FillRectangle(new SolidBrush(SystemColors.Control), pe.ClipRectangle);
			
			g.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
			                                        Color.White,
			                                        SystemColors.Control),
			                                        new Rectangle(0, 0, Width, Height));
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
