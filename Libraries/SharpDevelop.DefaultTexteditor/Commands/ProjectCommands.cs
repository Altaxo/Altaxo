// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ExportProjectToHtml : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			if (projectService.CurrentSelectedProject != null) {
				ExportProjectToHtmlDialog ephd = new ExportProjectToHtmlDialog(projectService.CurrentSelectedProject);
				ephd.Owner = (Form)WorkbenchSingleton.Workbench;
				ephd.ShowDialog();
				ephd.Dispose();
			}
		}
	}

}
