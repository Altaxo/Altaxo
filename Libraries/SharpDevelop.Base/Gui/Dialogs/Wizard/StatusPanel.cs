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
	public class StatusPanel : UserControl
	{
		WizardDialog wizard;
		Bitmap backGround = null;
		
		Font smallFont;
		Font normalFont;
		Font boldFont;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public StatusPanel(WizardDialog wizard)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			smallFont  = resourceService.LoadFont("SansSerif",  14, GraphicsUnit.World);
			normalFont = resourceService.LoadFont("Tahoma", 14, GraphicsUnit.World);
			boldFont   = resourceService.LoadFont("Tahoma", 14, FontStyle.Bold, GraphicsUnit.World);
			
			this.wizard = wizard;
			backGround = resourceService.GetBitmap("GeneralWizardBackground");
			Size = new Size(198, 400);
			ResizeRedraw  = false;
			
			//			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			//    		base.OnPaintBackground(pe);
			if (backGround != null) {
				Graphics g = pe.Graphics;
				g.DrawImage(backGround, 0, 0, Width, Height);
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			//    		base.OnPaint(pe);
			Graphics g = pe.Graphics;
			
			g.DrawString(resourceService.GetString("SharpDevelop.Gui.Dialogs.WizardDialog.StepsLabel"),
			             smallFont,
			             Brushes.Black,
			             10,
			             24 - smallFont.Height);
			
			g.DrawLine(Pens.Black, 10, 24, Width - 10, 24);
			
			int curNumber = 0;
			for (int i = 0; i < wizard.WizardPanels.Count; i = wizard.GetSuccessorNumber(i)) {
				Font curFont = wizard.ActivePanelNumber == i ? boldFont : normalFont;
				IDialogPanelDescriptor descriptor = ((IDialogPanelDescriptor)wizard.WizardPanels[i]);
				g.DrawString((1 + curNumber) + ". " + descriptor.Label, curFont, Brushes.Black, 10, 40 + curNumber * curFont.Height);
				++curNumber;
			}
		}
	}
}
