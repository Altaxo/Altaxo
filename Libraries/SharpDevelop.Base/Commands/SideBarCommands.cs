// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SideBarRenameTabItem : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTabItem item = sideBar.ActiveTab.ChoosedItem;
			if (item != null) {
				sideBar.StartRenamingOf(item);
			}
		}
	}
	
	public class SideBarDeleteTabItem: AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTabItem item = sideBar.ActiveTab.ChoosedItem;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			if (item != null && MessageBox.Show(stringParserService.Parse(resourceService.GetString("SideBarComponent.ContextMenu.DeleteTabItemQuestion"), new string[,] { {"TabItem", item.Name}}),
			                    resourceService.GetString("Global.QuestionText"), 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.ActiveTab.Items.Remove(item);
				sideBar.Refresh();
			}
		}
	}
	
	public class SideBarAddTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTab tab = new AxSideTab(sideBar, "New Tab");
			sideBar.Tabs.Add(tab);
			sideBar.StartRenamingOf(tab);
			sideBar.DoAddTab = true;
			sideBar.Refresh();
		} 
	}
	
	public class SideBarMoveTabUp : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.GetTabIndexAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (index > 0) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index - 1];
				sideBar.Tabs[index - 1] = tab;
				sideBar.Refresh();
			}
		} 
	}
	public class SideBarMoveTabDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.GetTabIndexAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			if (index >= 0 && index < sideBar.Tabs.Count - 1) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index + 1];
				sideBar.Tabs[index + 1] = tab;
				sideBar.Refresh();
			}
			
		} 
	}

	public class SideBarMoveActiveTabUp : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.Tabs.IndexOf(sideBar.ActiveTab);
			if (index > 0) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index - 1];
				sideBar.Tabs[index - 1] = tab;
				sideBar.Refresh();
			}
		} 
	}

	public class SideBarMoveActiveMoveTabDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.Tabs.IndexOf(sideBar.ActiveTab);
			if (index >= 0 && index < sideBar.Tabs.Count - 1) {
				AxSideTab tab = sideBar.Tabs[index];
				sideBar.Tabs[index] = sideBar.Tabs[index + 1];
				sideBar.Tabs[index + 1] = tab;
				sideBar.Refresh();
			}
		} 
	}
	
	public class SideBarDeleteTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			AxSideTab selectedSideTab = sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			if (MessageBox.Show(stringParserService.Parse(resourceService.GetString("SideBarComponent.ContextMenu.DeleteTabHeaderQuestion"), new string[,] { {"TabHeader", selectedSideTab.Name}}),
			                    resourceService.GetString("Global.QuestionText"), 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question,
			                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				sideBar.Tabs.Remove(selectedSideTab);
				sideBar.Refresh();
			}
		} 
	}
	
	public class SideBarRenameTabHeader : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			sideBar.StartRenamingOf(sideBar.GetTabAt(sideBar.SideBarMousePosition.X, sideBar.SideBarMousePosition.Y));
		} 
	}
	
	public class SideBarMoveActiveItemUp : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.SelectedItem);
			if (index > 0) {
				AxSideTabItem item = sideBar.ActiveTab.Items[index];
				sideBar.ActiveTab.Items[index] = sideBar.ActiveTab.Items[index - 1];
				sideBar.ActiveTab.Items[index - 1] = item;
				sideBar.Refresh();
			}
		}
	}
	
	public class SideBarMoveActiveItemDown : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopSideBar sideBar = (SharpDevelopSideBar)Owner;
			int index = sideBar.ActiveTab.Items.IndexOf(sideBar.ActiveTab.SelectedItem);
			if (index >= 0 && index < sideBar.ActiveTab.Items.Count - 1) {
				AxSideTabItem item = sideBar.ActiveTab.Items[index];
				sideBar.ActiveTab.Items[index] = sideBar.ActiveTab.Items[index + 1];
				sideBar.ActiveTab.Items[index + 1] = item;
				sideBar.Refresh();
			}
		} 
	}
}
