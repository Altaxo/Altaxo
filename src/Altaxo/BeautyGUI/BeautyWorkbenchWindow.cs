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

//using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;


using Reflector.UserInterface;

using ICSharpCode.SharpDevelop.Services;



namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class BeautyWorkbenchWindow : Form, Altaxo.IMainView
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Views";
		
		//PadContentCollection viewContentCollection       = new PadContentCollection();
		//ViewContentCollection workbenchContentCollection = new ViewContentCollection();
		
		bool closeAll = false;
		
		bool            fullscreen;
		FormWindowState defaultWindowState = FormWindowState.Normal;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		
		BeautyWorkbench m_Controller;
		//IWorkbenchLayout layout = null;
		
		protected static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		

		public BeautyWorkbench Controller
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
		public bool WindowFullScreen
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
		
		public string WindowTitle 
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
		
	
		
	
		
		public BeautyWorkbenchWindow()
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
			if (!closeAll && View_ActiveWorkbenchWindowChanged != null) 
			{
			View_ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
	
		
		public CommandBarManager commandBarManager = new CommandBarManager();
		public CommandBar   TopMenu  = null;
		public CommandBar[] ToolBars = null;

		
		public void CreateMainMenu()
		{
			TopMenu = new CommandBar(CommandBarStyle.Menu);
			CommandBarItem[] items = (CommandBarItem[])(AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath).BuildChildItems(this)).ToArray(typeof(CommandBarItem));
			TopMenu.Items.Clear();
			TopMenu.Items.AddRange(items);
		}

		public void UpdateMenu(object sender, EventArgs e)
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
		
		// this method simply copies over the enabled state of the toolbar,
		// this assumes that no item is inserted or removed.
		// TODO : make this method more add-in tree like, currently with Windows.Forms
		//        toolbars this is not possible. (drawing fragments, slow etc.)
		public void CreateToolBars()
		{
			if (ToolBars == null) 
			{
				ToolbarService toolBarService = (ToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ToolbarService));
				CommandBar[] toolBars = toolBarService.CreateToolbars();
				ToolBars = toolBars;
			}
		}

		public void UpdateToolbars()
		{
			foreach (CommandBar commandBar in ToolBars) 
			{
				foreach (object item in commandBar.Items) 
				{
					if (item is IStatusUpdate) 
					{
						((IStatusUpdate)item).UpdateStatus();
					}
				}
			}
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
		
		// Handle keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this.commandBarManager.PreProcessMessage(ref msg)) 
			{
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
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
		
		public event EventHandler View_ActiveWorkbenchWindowChanged;
		#region IMainView Members

		Altaxo.IMainViewEventSink Altaxo.IMainView.Controller
		{
			set
			{
				m_Controller = (BeautyWorkbench)value;
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
