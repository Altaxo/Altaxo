﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class ShowColorDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorProvider)) {
				return;
			}
			ITextEditor textEditor = ((ITextEditorProvider)viewContent).TextEditor;
			
			using (SharpDevelopColorDialog cd = new SharpDevelopColorDialog()) {
				if (cd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					string ext = Path.GetExtension(textEditor.FileName).ToLowerInvariant();
					string colorstr;
					if (ext == ".cs" || ext == ".vb" || ext == ".boo") {
						if (cd.Color.IsKnownColor) {
							colorstr = "Color." + cd.Color.ToKnownColor().ToString();
						} else if (cd.Color.A < 255) {
							colorstr = "Color.FromArgb(0x" + cd.Color.ToArgb().ToString("x") + ")";
						} else {
							colorstr = string.Format("Color.FromArgb({0}, {1}, {2})", cd.Color.R, cd.Color.G, cd.Color.B);
						}
					} else {
						if (cd.Color.IsKnownColor) {
							colorstr = cd.Color.ToKnownColor().ToString();
						} else if (cd.Color.A < 255) {
							colorstr = "#" + cd.Color.ToArgb().ToString("X");
						} else {
							colorstr = string.Format("#{0:X2}{1:X2}{2:X2}", cd.Color.R, cd.Color.G, cd.Color.B);
						}
					}
					
					textEditor.SelectedText = colorstr;
					textEditor.Select(textEditor.SelectionStart + textEditor.SelectionLength, 0);
				}
			}
		}
	}
}
