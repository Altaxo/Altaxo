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
	public class AddWebReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node   = browser.SelectedNode as AbstractBrowserNode;
			AbstractBrowserNode projectNode = DefaultDotNetNodeBuilder.GetProjectNode(node);
			
			if (node != null) {
				IProject project = ((ProjectBrowserNode)node.Parent).Project;
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));					
			
				using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(project)) {
					if (refDialog.ShowDialog() == DialogResult.OK) {						
						foreach(object objReference in refDialog.ReferenceInformations) {
							if (objReference is ProjectFile) {
								ProjectFile projectFile = (ProjectFile)objReference;
								
								// Do not add "Web References" node if it already exists.
								bool addFile = true;
								if (projectFile.Subtype == Subtype.WebReferences) {
									addFile = !WebReferencesNodeExists(fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory,projectFile.Name + Path.DirectorySeparatorChar), projectNode);
								}
								
								if (addFile) {
									// add to the project browser
									DefaultDotNetNodeBuilder.AddProjectFileNode(project, projectNode, projectFile);
										
									// add to the project
									projectService.AddFileToProject(project, projectFile);
									
									// add to code completion
									if (projectFile.Subtype == Subtype.Code) {
										parserService.ParseFile(projectFile.Name);
									}
								}
							}							
						}
						projectService.SaveCombine();						
					}
				}				
			}
		}
		
		/// <summary>
		/// Checks that a web references folder node does not already exist in the
		/// project.
		/// </summary>
		bool WebReferencesNodeExists(string directoryName, AbstractBrowserNode parentNode)
		{
			bool exists = false;
			
			// Check that a Web References node does not already exist.
			if (directoryName.StartsWith(".")) {
				directoryName =  directoryName.Substring(2);
			}						
			AbstractBrowserNode currentPathNode;
			currentPathNode = DefaultDotNetNodeBuilder.GetPath(Path.Combine(directoryName, "DUMMY"), parentNode, false);
			
			if (currentPathNode != null) {
				exists = true;
			}
			
			return exists;
		}
	}
}
