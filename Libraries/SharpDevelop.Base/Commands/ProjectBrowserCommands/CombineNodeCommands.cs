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
	public class AddNewProjectToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				NewProjectDialog npdlg = new NewProjectDialog(false);
				if (npdlg.ShowDialog() == DialogResult.OK) {
					node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)node.Combine.AddEntry(npdlg.NewProjectLocation)));
					projectService.SaveCombine();
				}
			}
		}
	}
		
	public class AddNewCombineToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				NewProjectDialog npdlg = new NewProjectDialog(false);
				if (npdlg.ShowDialog() == DialogResult.OK) {
					node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)node.Combine.AddEntry(npdlg.NewCombineLocation)));
					projectService.SaveCombine();
				}
			}
		}
	}
	
	public class AddProjectToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.AddExtension    = true;
					StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
					fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.ProjectFiles}|*.prjx|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.Multiselect     = false;
					fdiag.CheckFileExists = true;
					if (fdiag.ShowDialog() == DialogResult.OK) {
						object obj = node.Combine.AddEntry(fdiag.FileName);
						if (obj is IProject) {
							node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
						} else {
							node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
						}
						projectService.SaveCombine();
					}
				}
			}
		}
	}
		
	public class AddCombineToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.AddExtension    = true;
					StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
					fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.CombineFiles}|*.cmbx|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.Multiselect     = false;
					fdiag.CheckFileExists = true;
					if (fdiag.ShowDialog() == DialogResult.OK) {
						object obj = node.Combine.AddEntry(fdiag.FileName);
						if (obj is IProject) {
							node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
						} else {
							node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
						}
						projectService.SaveCombine();
					}
				}
			}
		}
	}
	
	public class CombineOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				DefaultProperties defaultProperties = new DefaultProperties();
				defaultProperties.SetProperty("Combine", node.Combine);
				using (TreeViewOptions optionsDialog = new TreeViewOptions(defaultProperties,
				                                                           AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/CombineOptions"))) {
//					optionsDialog.Size = new Size(700, 450);
					optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
							
					optionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
					optionsDialog.ShowDialog();
					projectService.SaveCombine();
				}
			}
		}
	}
}
