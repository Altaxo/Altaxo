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

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using Crownwood.Magic.Common;
using Crownwood.Magic.Docking;

using Reflector.UserInterface;



namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class MdiWorkbenchLayout : IWorkbenchLayout
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFile = propertyService.ConfigDirectory + "MdiLayoutConfig.xml";
		Form wbForm;
		IExtendedWorkbench m_Workbench;


		DockingManager dockManager;
		ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab tabControl = new ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab();
		
		protected IWorkbenchWindow m_ActiveWorkbenchWindow;

		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if(m_ActiveWorkbenchWindow!=null)
					return m_ActiveWorkbenchWindow;
				if (tabControl == null || tabControl.SelectedIndex < 0 || tabControl.SelectedIndex >= tabControl.TabPages.Count)  
				{
					return null;
				}
				return (IWorkbenchWindow)tabControl.SelectedTab.Tag;
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			m_Workbench = (IExtendedWorkbench)workbench;
			wbForm = (Form)m_Workbench.ViewObject;
			wbForm.Controls.Clear();
			wbForm.IsMdiContainer = true;
			
			tabControl.Dock = DockStyle.Top;
			tabControl.ShrinkPagesToFit = true;
			tabControl.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
			tabControl.Size = new Size(10, 24);
			wbForm.Controls.Add(tabControl);
			
			dockManager = new DockingManager(wbForm, Crownwood.Magic.Common.VisualStyle.IDE);
			
//			Control firstControl = null;

			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			wbForm.Controls.Add(statusBarService.Control);
			
			((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(((DefaultWorkbench)workbench).TopMenu);
			foreach (CommandBar toolBar in ((DefaultWorkbench)workbench).ToolBars) 
			{
				((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(toolBar);
			}
			wbForm.Controls.Add(((DefaultWorkbench)workbench).commandBarManager);
			
			wbForm.Menu = null;
			dockManager.InnerControl = tabControl;

			dockManager.OuterControl = statusBarService.Control;

	
			foreach (IViewContent content in workbench.ViewContentCollection) {
				ShowView(content);
			}
			
			foreach (IPadContent content in workbench.PadContentCollection) {
				ShowPad(content);
			}
			
			tabControl.SelectionChanged += new EventHandler(this.EhTabControl_SelectionChanged);			
			wbForm.MdiChildActivate += new EventHandler(this.EhView_MdiChildActivate);

			try { 
				if (File.Exists(configFile)) {
					dockManager.LoadConfigFromFile(configFile);
				} else {
					CreateDefaultLayout();
				}
				
			} catch (Exception) {
				Console.WriteLine("can't load docking configuration, version clash ?");
			}
			RedrawAllComponents();
		}
		
		Content GetContent(string padTypeName)
		{
			IPadContent pad = ((IWorkbench)wbForm).PadContentCollection[padTypeName];
			if (pad != null) {
				return (Content)contentHash[pad];
			}
			return null;
		}
		
		void CreateDefaultLayout()
		{
			WindowContent leftContent   = null;
			WindowContent rightContent  = null;
			WindowContent bottomContent = null;
			
			string[] leftContents = new string[] {
				"ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView",
				"ICSharpCode.SharpDevelop.Gui.Pads.ClassScout",
				"ICSharpCode.SharpDevelop.Gui.Pads.FileScout",
				"ICSharpCode.SharpDevelop.Gui.Pads.SideBarView"
			};
			string[] rightContents = new string[] {
				"ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad",
				"ICSharpCode.SharpDevelop.Gui.Pads.HelpBrowser"
			};
			string[] bottomContents = new string[] {
				"ICSharpCode.SharpDevelop.Gui.Pads.OpenTaskView",
				"ICSharpCode.SharpDevelop.Gui.Pads.CompilerMessageView"
			};
			
			foreach (string typeName in leftContents) {
				Content c = GetContent(typeName);
				if (c != null) {
					if (leftContent == null) {
						leftContent = dockManager.AddContentWithState(c, State.DockLeft) as WindowContent;
					} else {
						dockManager.AddContentToWindowContent(c, leftContent);
					}
				}
			}
			
			foreach (string typeName in bottomContents) {
				Content c = GetContent(typeName);
				if (c != null) {
					if (bottomContent == null) {
						bottomContent = dockManager.AddContentWithState(c, State.DockBottom) as WindowContent;
					} else {
						dockManager.AddContentToWindowContent(c, bottomContent);
					}
				}
			}
			
			foreach (string typeName in rightContents) {
				Content c = GetContent(typeName);
				if (c != null) {
					if (rightContent == null) {
						rightContent = dockManager.AddContentWithState(c, State.DockRight) as WindowContent;
					} else {
						dockManager.AddContentToWindowContent(c, rightContent);
					}
				}
			}
		}
		
		public void Detach()
		{
			try {
			if (dockManager != null) {
				dockManager.SaveConfigToFile(configFile);
			}

			tabControl.SelectionChanged -= new EventHandler(this.EhTabControl_SelectionChanged);			
			wbForm.MdiChildActivate -= new EventHandler(this.EhView_MdiChildActivate);

			
#if LellidMod
			foreach (Altaxo.Main.GUI.IWorkbenchWindowView wv in wbForm.MdiChildren) 
			{
				IWorkbenchWindow f = (IWorkbenchWindow)wv.Controller;
				wv.Controller.Content=null;
				//f.ViewContent = null;
				wv.SetChild(null);
				f.CloseWindow(true);
			}
#else
			foreach (DefaultWorkspaceWindow f in wbForm.MdiChildren) {
				f.DetachContent();
				f.ViewContent = null;
				f.Controls.Clear();
				f.Close();
			}
#endif	
			tabControl.TabPages.Clear();
			tabControl.Controls.Clear();
			
			if (dockManager != null) {
				dockManager.Contents.Clear();
			}
			
			wbForm.IsMdiContainer = false;
			wbForm.Controls.Clear();
			} 
			catch (Exception) {}

			m_Workbench = null;
			wbForm = null;
		}
		
		Hashtable contentHash = new Hashtable();
		
		public void ShowPad(IPadContent content)
		{
			if (contentHash[content] == null) {
				IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
				content.Control.Dock = DockStyle.None;
				Content newContent;
				if (content.Icon != null) {
					ImageList imgList = new ImageList();
#if LellidMod
#else
					IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
					imgList.Images.Add(iconService.GetBitmap(content.Icon));
#endif
					newContent = dockManager.Contents.Add(content.Control, content.Title, imgList, 0);
				} else {
					newContent = dockManager.Contents.Add(content.Control, content.Title);
				}
				contentHash[content] = newContent;
			} else {
				Content c = (Content)contentHash[content];
				if (c != null) {
					dockManager.ShowContent(c);
				}
			}
		}
		
		public bool IsVisible(IPadContent padContent)
		{
			if (padContent != null) 
			{
				Content content = (Content)contentHash[padContent];
				if (content != null) 
				{
					return content.Visible;
				}
			}
			return false;
		}
		
		public void HidePad(IPadContent padContent)
		{
			if (padContent != null) 
			{
				Content content = (Content)contentHash[padContent];
				if (content != null) 
				{
					dockManager.HideContent(content);
				}
			}
		}
		
		public void ActivatePad(IPadContent padContent)
		{
			if (padContent != null) 
			{
				Content content = (Content)contentHash[padContent];
				if (content != null) 
				{
					content.BringToFront();
				}
			}
		}
		
		public void RedrawAllComponents()
		{
			tabControl.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.TabVisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			// redraw correct pad content names (language may have changed)
			foreach (IPadContent content in ((BeautyWorkbenchWindow)wbForm).Controller.PadContentCollection) 
			{
				Content c = (Content)contentHash[content];
				if (c != null) {
					c.Title = c.FullTitle = content.Title;
				}
			}
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
#if LellidMod
			//IWorkbenchWindow f = (IWorkbenchWindow)sender;
#else
			DefaultWorkspaceWindow f = (DefaultWorkspaceWindow)sender;
			if (f.ViewContent != null) {
				((IWorkbench)wbForm).CloseContent(f.ViewContent);
			}
#endif
			
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
#if LellidMod
			

			Altaxo.Main.GUI.IWorkbenchWindowController controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			Altaxo.Main.GUI.BeautyWorkspaceWindow window = new Altaxo.Main.GUI.BeautyWorkspaceWindow();
			controller.View = window;

			controller.Content = (Altaxo.Main.GUI.IWorkbenchContentController)content;



		//	content.Control.Visible = true;
			
			if (wbForm.MdiChildren.Length == 0 || wbForm.ActiveMdiChild==null || wbForm.ActiveMdiChild.WindowState == FormWindowState.Maximized) 
			{
				((Form)window).WindowState = FormWindowState.Maximized;
			}
			window.TabPage = tabControl.AddWindow((IWorkbenchWindow)controller);
			((Form)window).MdiParent = wbForm;
			((Form)window).Show();
			window.Closed += new EventHandler(CloseWindowEvent);
			
			return (IWorkbenchWindow)controller;
#else
			DefaultWorkspaceWindow window = new DefaultWorkspaceWindow(content);
			
			content.Control.Visible = true;
			
			if (wbForm.MdiChildren.Length == 0 || wbForm.ActiveMdiChild.WindowState == FormWindowState.Maximized) {
				((Form)window).WindowState = FormWindowState.Maximized;
			}
			window.TabPage = tabControl.AddWindow(window);
			((Form)window).MdiParent = wbForm;
			((Form)window).Show();
			window.Closed += new EventHandler(CloseWindowEvent);
			
			return window;
#endif
			}

		protected void EhTabControl_SelectionChanged(object sender, System.EventArgs e)
		{
			// we have to make the window that is associated with the tab
			// the active mdi child
			m_ActiveWorkbenchWindow = (IWorkbenchWindow)tabControl.SelectedTab.Tag;

			// bring this window now to the foreground
			System.Windows.Forms.Form childForm = ((Altaxo.Main.GUI.IWorkbenchWindowController)m_ActiveWorkbenchWindow).View as System.Windows.Forms.Form;
			childForm.Activate();
		}


		/// <summary>
		/// Is called from the view when the currently active MDI child window has changed
		/// </summary>
		protected void EhView_MdiChildActivate(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Form activatedWindow = wbForm.ActiveMdiChild;

			if(activatedWindow is Altaxo.Main.GUI.BeautyWorkspaceWindow)
			{
				m_ActiveWorkbenchWindow = ((Altaxo.Main.GUI.BeautyWorkspaceWindow)activatedWindow).Controller;
			
			
				foreach(IViewContent content in m_Workbench.ViewContentCollection)
					if(!object.Equals(content.WorkbenchWindow,m_ActiveWorkbenchWindow))
						content.WorkbenchWindow.OnWindowDeselected(EventArgs.Empty);

				// TODO change this to .OnSelectWindow since the window is already selected
				m_ActiveWorkbenchWindow.SelectWindow();
			}

			OnActiveWindowChanged(this,EventArgs.Empty);
		}

		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
