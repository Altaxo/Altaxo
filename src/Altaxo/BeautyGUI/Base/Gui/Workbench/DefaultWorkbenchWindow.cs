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

using Crownwood.Magic.Menus;

//using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

//using ICSharpCode.SharpDevelop.Services;

//using UtilityLibrary.CommandBars;
//using UtilityLibrary.WinControls;
//using UtilityLibrary.General;
//using UtilityLibrary.Win32;
//using UtilityLibrary.Collections;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class DefaultWorkbenchWindow : Form, Altaxo.IMainView
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Views";
		
		//PadContentCollection viewContentCollection       = new PadContentCollection();
		//ViewContentCollection workbenchContentCollection = new ViewContentCollection();
		
		bool closeAll = false;
		
		bool            fullscreen;
		FormWindowState defaultWindowState = FormWindowState.Normal;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		
		DefaultWorkbench m_Controller;
		//IWorkbenchLayout layout = null;
		
		protected static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		

		public DefaultWorkbench Controller
		{
			get { return m_Controller; }
			set 
			{
				m_Controller = value; 
			}
		}

		public System.Windows.Forms.Form Form
		{
			get { return this; }
		}

		// Lellid: move this to the view
		public bool FullScreen 
		{
			get 
			{
				return fullscreen;
			}
			set 
			{
				fullscreen = value;
				if (fullscreen) 
				{
					FormBorderStyle    = FormBorderStyle.None;
					defaultWindowState = WindowState;
					WindowState        = FormWindowState.Maximized;
				} 
				else 
				{
					FormBorderStyle = FormBorderStyle.Sizable;
					Bounds          = normalBounds;
					WindowState     = defaultWindowState;
				}
			}
		}
		
		public string Title 
		{
			get 
			{
				return Text;
			}
			set 
			{
				Text = value;
			}
		}
		
		EventHandler windowChangeEventHandler;
		
	
		
	
		
		public DefaultWorkbenchWindow()
		{
#if LellidMod
#else
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			Text = resourceService.GetString("MainWindow.DialogName");
			Icon = resourceService.GetIcon("Icons.SharpDevelopIcon");
#endif
	
			windowChangeEventHandler = new EventHandler(OnActiveWindowChanged);
			
			StartPosition = FormStartPosition.Manual;
			
			AllowDrop      = true;
		}
		
		public void InitializeWorkspace()
		{
			Menu = null;
			
			//			statusBarManager.Control.Dock = DockStyle.Bottom;
			
			ActiveWorkbenchWindowChanged += new EventHandler(UpdateMenu);
			
			MenuComplete += new EventHandler(SetStandardStatusBar);
			SetStandardStatusBar(null, null);
			
#if LellidMod
#else
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			
			projectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);
			projectService.CombineOpened         += new CombineEventHandler(CombineOpened);

			fileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
			fileService.FileRenamed += new FileEventHandler(CheckRenamedFile);
			
			fileService.RecentOpen.RecentFileChanged    += new EventHandler(UpdateMenu);
			fileService.RecentOpen.RecentProjectChanged += new EventHandler(UpdateMenu);
			
			fileService.FileRemoved += new FileEventHandler(fileService.RecentOpen.FileRemoved);
			fileService.FileRenamed += new FileEventHandler(fileService.RecentOpen.FileRenamed);
#endif
	
			TopMenu.Selected += new CommandHandler(OnTopMenuSelected);
			TopMenu.Deselected += new CommandHandler(OnTopMenuDeselected);
			CreateToolBars();
		}
		
		
		
		
	
		
	
	
		
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (WindowState == FormWindowState.Normal) 
			{
				normalBounds = Bounds;
			}
			
		}
		
		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);
			if (WindowState == FormWindowState.Normal) 
			{
				normalBounds = Bounds;
			}
		}
		
		
		protected void OnTopMenuSelected(MenuCommand mc)
		{
#if LellidMod
#else
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			
			statusBarService.SetMessage(mc.Description);
#endif
		}
		
		protected void OnTopMenuDeselected(MenuCommand mc)
		{
			SetStandardStatusBar(null, null);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

#if LellidMod
#else
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			projectService.SaveCombinePreferences();
			while (WorkbenchSingleton.Workbench.ViewContentCollection.Count > 0) {
				IViewContent content = WorkbenchSingleton.Workbench.ViewContentCollection[0];
				content.WorkbenchWindow.CloseWindow(false);
				if (WorkbenchSingleton.Workbench.ViewContentCollection.IndexOf(content) >= 0) {
					e.Cancel = true;
					return;
				}

			}
#endif
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if(null!=Controller)
			{
				Controller.WorkbenchLayout.Detach();
				foreach (IPadContent content in Controller.PadContentCollection) 
				{
					content.Dispose();
				}
			}
		}
		
	

		void SetStandardStatusBar(object sender, EventArgs e)
		{
#if LellidMod
#else
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
#endif
		}
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (!closeAll && ActiveWorkbenchWindowChanged != null) 
			{
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public MenuControl TopMenu = new MenuControl();
		
		// public ToolBarEx[]   ToolBars;
		

		
		void UpdateMenu(object sender, EventArgs e)
		{
			TopMenu.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			MenuCommand[] items = (MenuCommand[])(AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath).BuildChildItems(this)).ToArray(typeof(MenuCommand));
			TopMenu.MenuCommands.Clear();
			TopMenu.MenuCommands.AddRange(items);
			CreateToolBars();
		}
		
		
		// this method simply copies over the enabled state of the toolbar,
		// this assumes that no item is inserted or removed.
		// TODO : make this method more add-in tree like, currently with Windows.Forms
		//        toolbars this is not possible. (drawing fragments, slow etc.)
		void CreateToolBars()
		{
#if LellidMod
#else
			ToolbarService toolBarService = (ToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ToolbarService));
			ToolBarEx[] toolBars = toolBarService.CreateToolbars();
			
			if (ToolBars == null) {
				ToolBars = toolBars;
			} else {
				for (int i = 0; i < toolBars.Length;++i) {					
					for (int j = 0; j < toolBars[i].Items.Count; ++j) {
						ToolBars[i].Items[j].Enabled = toolBars[i].Items[j].Enabled;
						ToolBars[i].Items[j].ToolTip = toolBars[i].Items[j].ToolTip;
					}
				}
			}
#endif
		}
		
		
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				foreach (string file in files) 
				{
					if (File.Exists(file)) 
					{
						e.Effect = DragDropEffects.Copy;
						return;
					}
				}
			}
			e.Effect = DragDropEffects.None;
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);

#if LellidMod
#else
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				foreach (string file in files) {
					if (File.Exists(file)) {
						fileService.OpenFile(file);
					}
				}
			}
#endif
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		#region IMainView Members

		Altaxo.IMainController Altaxo.IMainView.Controller
		{
			set
			{
				m_Controller = (DefaultWorkbench)value;
			}
		}

		public MainMenu MainViewMenu
		{
			set
			{
				
			}
		}

		#endregion
	}
}
