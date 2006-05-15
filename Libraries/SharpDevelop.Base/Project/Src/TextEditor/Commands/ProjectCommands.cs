﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ExportProjectToHtml : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.CurrentProject != null)
			{
				ExportProjectToHtmlDialog ephd = new ExportProjectToHtmlDialog(ProjectService.CurrentProject);
				ephd.Owner = (Form)WorkbenchSingleton.Workbench;
				ephd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				ephd.Dispose();
			}
		}
	}
}
