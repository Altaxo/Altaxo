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
	
	/*
	public class RefreshWebReference : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			ReferenceNode   node    = browser.SelectedNode as ReferenceNode;
			if (node != null) {				
				IProject project = node.Project;  //((ProjectBrowserNode)node.Parent.Parent).Project;
				IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
				
				ProjectReference refInfo = (ProjectWebReference)node.UserData;
				WebReference.GenerateWebProxy(project, refInfo.HRef);				
				parserService.AddReferenceToCompletionLookup(project, refInfo);							
			}
		}
	}*/
	
	public class AddWebReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node   = browser.SelectedNode as AbstractBrowserNode;
			AbstractBrowserNode projectNode = DefaultDotNetNodeBuilder.GetProjectNode(node);
			bool bInitReferences = false;
			
			if (node != null) {
				IProject project = ((ProjectBrowserNode)node.Parent).Project;
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));					
			
				using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(project)) {
					if (refDialog.ShowDialog() == DialogResult.OK) {						
						foreach(object objReference in refDialog.ReferenceInformations) {
							if(objReference is ProjectReference) {
								ProjectReference refInfo = (ProjectReference)objReference;
								project.ProjectReferences.Add(refInfo);
								if(refInfo.ReferenceType == ReferenceType.Assembly) {
									parserService.AddReferenceToCompletionLookup(project, refInfo);
									bInitReferences = true;
								}
							} else if(objReference is ProjectFile) {
								ProjectFile projectFile = (ProjectFile) objReference;
								//HACK: fix later
								if(projectFile.Subtype == Subtype.WebReferences || projectFile.Subtype == Subtype.Directory) {																		
									AbstractBrowserNode checkNode = DefaultDotNetNodeBuilder.GetPath(fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory,projectFile.Name + Path.DirectorySeparatorChar), projectNode, false);
									if(checkNode != null) {
										continue;
									}
								}																																	
								// add to the project browser
								DefaultDotNetNodeBuilder.AddProjectFileNode(project, projectNode, projectFile);
									
								// add to the project
								projectService.AddFileToProject(project, projectFile);
								
								// add to code completion
								if(projectFile.Subtype == Subtype.Code ) {
									parserService.ParseFile(projectFile.Name);
								}
								
							}							
						}
						if(bInitReferences) {
							DefaultDotNetNodeBuilder.InitializeReferences(node, project);						
						}
						projectService.SaveCombine();						
					}
				}				
			}
		}
	}
}
