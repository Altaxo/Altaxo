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
	public class RemoveEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (node.IsEditing) { // TODO : throw remove key to the browser component.
				return;
			}
			
			if (node != null && node.Parent != null) {
				if (node.RemoveNode()) {
					node.Parent.Nodes.Remove(node);
					projectService.SaveCombine();
				}
			}
		}
	}
	
	public class RenameEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			browser.StartLabelEdit();
		}
	}
	
	public class OpenFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			
			if (node != null) {
				node.ActivateItem();
			}
		}
	}
		
	
}
