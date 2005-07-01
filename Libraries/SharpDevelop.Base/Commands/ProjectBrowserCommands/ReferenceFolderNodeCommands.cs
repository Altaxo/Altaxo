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

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

namespace ICSharpCode.SharpDevelop.Commands.ProjectBrowser
{
	public class AddReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			FolderNode         node    = browser.SelectedNode as FolderNode;
			
			if (node != null) {
				IProject project = ((ProjectBrowserNode)node.Parent).Project;
				IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
				
				using (SelectReferenceDialog selDialog = new SelectReferenceDialog(project)) {
					if (selDialog.ShowDialog() == DialogResult.OK) {
						
						foreach (ProjectReference refInfo in selDialog.ReferenceInformations) {
							project.ProjectReferences.Add(refInfo);
							parserService.AddReferenceToCompletionLookup(project, refInfo);
						}
						
						DefaultDotNetNodeBuilder.InitializeReferences(node, project);
						IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
						projectService.SaveCombine();
					}
				}
				node.Expand();
			}
		}
	}
}
