// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Services;

using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Services
{
	public class ToolbarService : AbstractService
	{
		readonly static string toolBarPath     = "/SharpDevelop/Workbench/ToolBar";
		
		IAddInTreeNode node;
		
		public ToolbarService()
		{
			this.node  = AddInTreeSingleton.AddInTree.GetTreeNode(toolBarPath);
		}
		
		public CommandBar[] CreateToolbars()
		{
			ToolbarItemCodon[] codons = (ToolbarItemCodon[])(node.BuildChildItems(this)).ToArray(typeof(ToolbarItemCodon));
			
			CommandBar[] toolBars = new CommandBar[codons.Length];
			
			for (int i = 0; i < codons.Length; ++i) {
				toolBars[i] = CreateToolBarFromCodon(WorkbenchSingleton.Workbench, codons[i]);
			}
			return toolBars;
		}
		
		public CommandBar CreateToolBarFromCodon(object owner, ToolbarItemCodon codon)
		{
			CommandBar bar = new CommandBar(CommandBarStyle.ToolBar);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			foreach (ToolbarItemCodon childCodon in codon.SubItems) {
				CommandBarItem item;
				
				if (childCodon.ToolTip != null) {
					if (childCodon.ToolTip == "-") {
						item = new CommandBarSeparator();
					} else {
						item = new SdMenuCommand(childCodon.Conditions, owner, childCodon.ToolTip);
						item.Image = resourceService.GetBitmap(childCodon.Icon);
					}
				} else {
					continue;
				}
				if (childCodon.Class != null) {
					((SdMenuCommand)item).Command = (ICommand)childCodon.AddIn.CreateObject(childCodon.Class);
				}
				bar.Items.Add(item);
			}
			return bar;
		}
		
		void ToolBarButtonClick(object sender, EventArgs e)
		{
			if (sender is CommandBarItem) {
				((ICommand)((CommandBarItem)sender).Tag).Run();
			}
		}
	}
}
