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

using Reflector.UserInterface;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns.Conditions;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui.ErrorHandlers;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class RecentFilesMenuBuilder : ISubmenuBuilder
	{
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentFile.Count > 0) {
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					items[i] = new SdMenuCommand(null, null, accelaratorKeyPrefix + recentOpen.RecentFile[i].ToString(), new EventHandler(LoadRecentFile));
					items[i].Tag = recentOpen.RecentFile[i].ToString();
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadFileDescription"),
					                                          new string[,] { {"FILE", recentOpen.RecentFile[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(null, null, resourceService.GetString("Dialog.Componnents.RichMenuItem.NoRecentFilesString"));
			defaultMenu.IsEnabled = false;
			
			return new SdMenuCommand[] { defaultMenu };
		}
		
		void LoadRecentFile(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(item.Tag.ToString());
		}
	}
	
	public class RecentProjectsMenuBuilder : ISubmenuBuilder
	{
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentProject.Count > 0) {
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentProject.Count];
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					items[i] = new SdMenuCommand(null, null, accelaratorKeyPrefix + recentOpen.RecentProject[i].ToString(), new EventHandler(LoadRecentProject));
					items[i].Tag = recentOpen.RecentProject[i].ToString();
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadProjectDescription"),
					                                         new string[,] { {"PROJECT", recentOpen.RecentProject[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(null, null, resourceService.GetString("Dialog.Componnents.RichMenuItem.NoRecentProjectsString"));
			defaultMenu.IsEnabled = false;
			
			return new SdMenuCommand[] { defaultMenu };
		}
		void LoadRecentProject(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			string filename = item.Tag.ToString();
			
			try {
				projectService.OpenCombine(filename);
			} catch (Exception ex) {
				CombineLoadError.HandleError(ex, filename);
			}
		}		
	}
	
	public class ToolMenuBuilder : ISubmenuBuilder
	{
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			SdMenuCommand[] items = new SdMenuCommand[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				SdMenuCommand item = new SdMenuCommand(null, null, ToolLoader.Tool[i].ToString(), new EventHandler(ToolEvt));
				item.Description = "Start tool " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
				items[i] = item;
			}
			return items;
		}
		
		void ToolEvt(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text == ToolLoader.Tool[i].ToString()) {
					ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
					string command = stringParserService.Parse(tool.Command);
					string args    = stringParserService.Parse(tool.Arguments);
					Console.WriteLine("command : " + command);
					Console.WriteLine("args    : " + args);
					try {
						ProcessStartInfo startinfo;
						if (args == null || args.Length == 0) {
							startinfo = new ProcessStartInfo(command);
						} else {
							startinfo = new ProcessStartInfo(command, args);
						}
						
						startinfo.WorkingDirectory = tool.InitialDirectory;
						Process.Start(startinfo);
					} catch (Exception ex) {
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(ex, "External program execution failed.\nError while starting:\n '" + command + " " + args + "'");
					}
						break;
					}
				}
			}
		}
				
	public class OpenContentsMenuBuilder : ISubmenuBuilder
	{
				
		class MyMenuItem : SdMenuCheckBox
		{
			public MyMenuItem(string name) : base(null, null, name)
			{
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				((IWorkbenchWindow)Tag).SelectWindow();
				IsChecked = true;
			}
		}

		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			int contentCount = WorkbenchSingleton.Workbench.ViewContentCollection.Count;
			if (contentCount == 0) {
				return new CommandBarItem[] {};
			}
			CommandBarItem[] items = new CommandBarItem[contentCount + 1];
			items[0] = new SdMenuSeparator(null, null);
			for (int i = 0; i < contentCount; ++i) {
				IViewContent content = (IViewContent)WorkbenchSingleton.Workbench.ViewContentCollection[i];
				
				SdMenuCheckBox item = new MyMenuItem(content.WorkbenchWindow.Title);
				item.Tag = content.WorkbenchWindow;
				item.IsChecked = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == content.WorkbenchWindow;
				item.Description = "Activate this window ";
				items[i + 1] = item;
			}
			return items;
		}
	}
	
	public class IncludeFilesBuilder : ISubmenuBuilder
	{
		public ProjectBrowserView browser;
		
		MyMenuItem includeInCompileItem;
		MyMenuItem includeInDeployItem;
		
		class MyMenuItem : SdMenuCheckBox
		{
			IncludeFilesBuilder builder;
			
			public MyMenuItem(IncludeFilesBuilder builder, string name, EventHandler handler) : base(null, null, name)
			{
				base.Click += handler;
				this.builder = builder;
			}
			
			public override void UpdateStatus()
			{
				base.UpdateStatus();
				if (builder == null) {
					return;
				}
				AbstractBrowserNode node = builder.browser.SelectedNode as AbstractBrowserNode;
				
				if (node == null) {
					return;
				}
				
				ProjectFile finfo = node.UserData as ProjectFile;
				if (finfo == null) {
					builder.includeInCompileItem.IsEnabled = builder.includeInCompileItem.IsEnabled = false;
				} else {
					if (!builder.includeInCompileItem.IsEnabled) {
						builder.includeInCompileItem.IsEnabled = builder.includeInCompileItem.IsEnabled = true;
					}
					builder.includeInCompileItem.IsChecked = finfo.BuildAction == BuildAction.Compile;
					builder.includeInDeployItem.IsChecked  = !node.Project.DeployInformation.IsFileExcluded(finfo.Name);
				}
			}
		}
		
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			browser = (ProjectBrowserView)owner;
			includeInCompileItem = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InCompile}", new EventHandler(ChangeCompileInclude));
			includeInDeployItem  = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InDeploy}",  new EventHandler(ChangeDeployInclude));
			
			return new CommandBarItem[] {
				includeInCompileItem,
				includeInDeployItem
			};
			
		}
		void ChangeCompileInclude(object sender, EventArgs e)
		{
			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
			
			if (node == null) {
				return;
			}
			
			ProjectFile finfo = node.UserData as ProjectFile;
			if (finfo != null) {
				if (finfo.BuildAction == BuildAction.Compile) {
					finfo.BuildAction = BuildAction.Nothing;
				} else {
					finfo.BuildAction = BuildAction.Compile;
				}
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
		
		void ChangeDeployInclude(object sender, EventArgs e)
		{
			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
			
			if (node == null) {
				return;
			}
			
			ProjectFile finfo = node.UserData as ProjectFile;
			if (finfo != null) {
				if (node.Project.DeployInformation.IsFileExcluded(finfo.Name)) {
					node.Project.DeployInformation.RemoveExcludedFile(finfo.Name);
				} else {
					node.Project.DeployInformation.AddExcludedFile(finfo.Name);
				}
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
	}
	
	
	public class ViewMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : SdMenuCheckBox
		{
			IPadContent padContent;
			
			bool IsPadVisible {
				get {
					return WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(padContent); 
				}
			}
			
			public MyMenuItem(IPadContent padContent) : base(null, null, padContent.Title)
			{
				this.padContent = padContent;
				IsChecked = IsPadVisible;
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				if (IsPadVisible) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(padContent);
				} else {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(padContent);
				}
				IsChecked = IsPadVisible;
			}
			public override  void UpdateStatus()
			{
				base.UpdateStatus();
				IsChecked = IsPadVisible;
			}
		}
		
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			ArrayList items = new ArrayList();
			foreach (IPadContent padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
				items.Add(new MyMenuItem(padContent));
			}
			return (CommandBarItem[])items.ToArray(typeof(CommandBarItem));
		}
	}
}
