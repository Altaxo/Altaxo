// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public class DeclarationViewWindow : Form
	{
		string description = String.Empty;
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				if (Visible) {
					Refresh();
				}
			}
		}
		
		public DeclarationViewWindow()
		{
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			
//			Enabled         = false;
			Size            = new Size(0, 0);
			
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			TipPainterTools.DrawHelpTipFromCombinedDescription
				(this, pe.Graphics, Font, null, description);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			if (description != null && description.Length > 0) {
				pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
			}
		}
	}
}
