// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Services;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class Workbench1 : DefaultWorkbench
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";

		#region "Serialization"
	
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Workbench1),0)]
		public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo	info)
			{
				Workbench1 s = (Workbench1)obj;
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo	info, object parent)
			{
				return o;
			}
		}



		#endregion

		public new void InitializeWorkspace()
		{
			Menu = null;
			
			//			statusBarManager.Control.Dock = DockStyle.Bottom;
			
			ActiveWorkbenchWindowChanged += new EventHandler(UpdateMenu);
			
			MenuComplete += new EventHandler(SetStandardStatusBar);
			SetStandardStatusBar(null, null);
			
#if OriginalSharpDevelopCode
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			
			projectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);
			projectService.CombineOpened         += new CombineEventHandler(CombineOpened);
			fileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
			fileService.FileRenamed += new FileEventHandler(CheckRenamedFile);
			
			fileService.FileRemoved += new FileEventHandler(fileService.RecentOpen.FileRemoved);
			fileService.FileRenamed += new FileEventHandler(fileService.RecentOpen.FileRenamed);
#endif
			
			//			TopMenu.Selected   += new CommandHandler(OnTopMenuSelected);
			//			TopMenu.Deselected += new CommandHandler(OnTopMenuDeselected);
			CreateMainMenu();
			CreateToolBars();
		}


		void UpdateMenu(object sender, EventArgs e)
		{
			// update menu
			foreach (object o in TopMenu.Items) 
			{
				if (o is IStatusUpdate) 
				{
					((IStatusUpdate)o).UpdateStatus();
				}
			}
			
			UpdateToolbars();
		}

		void SetStandardStatusBar(object sender, EventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}

		void CreateMainMenu()
		{
			TopMenu = new CommandBar(CommandBarStyle.Menu);
			CommandBarItem[] items = (CommandBarItem[])(AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath).BuildChildItems(this)).ToArray(typeof(CommandBarItem));
			TopMenu.Items.Clear();
			TopMenu.Items.AddRange(items);
		}

		// this method simply copies over the enabled state of the toolbar,
		// this assumes that no item is inserted or removed.
		// TODO : make this method more add-in tree like, currently with Windows.Forms
		//        toolbars this is not possible. (drawing fragments, slow etc.)
		void CreateToolBars()
		{
			if (ToolBars == null) 
			{
#if OriginalSharpDevelopCode
				ToolbarService toolBarService = (ToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ToolbarService));
#else
				// Note: original ToolbarService don't support checked toolbar items
				AltaxoToolbarService toolBarService = (AltaxoToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(AltaxoToolbarService));
#endif
				CommandBar[] toolBars = toolBarService.CreateToolbars();
				ToolBars = toolBars;
			}
		}

		public void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e)
		{
			UpdateMenu(null, null);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			Altaxo.Main.ProjectService projectService = (Altaxo.Main.ProjectService)ServiceManager.Services.GetService(typeof(Altaxo.Main.ProjectService));
			System.Text.StringBuilder title = new System.Text.StringBuilder();
			title.Append(resourceService.GetString("MainWindow.DialogName"));
			if (projectService != null) 
			{
				if(projectService.CurrentProjectFileName == null)
				{
					title.Append(" - ");
					title.Append(resourceService.GetString("Altaxo.Project.UntitledName"));
				}
				else
				{
					title.Append(" - ");
					title.Append(projectService.CurrentProjectFileName);
				}
				if(projectService.CurrentOpenProject!= null && projectService.CurrentOpenProject.IsDirty)
					title.Append("*");
			} 
			this.Title = title.ToString();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			Altaxo.Main.ProjectService projectService = (Altaxo.Main.ProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(Altaxo.Main.ProjectService));
			
			if (projectService != null)
			{
				// projectService.SaveCombinePreferences();
			
				if(projectService.CurrentOpenProject != null && projectService.CurrentOpenProject.IsDirty)
				{
					projectService.AskForSavingOfProject(e);
				}
			}
		}

	}
}
