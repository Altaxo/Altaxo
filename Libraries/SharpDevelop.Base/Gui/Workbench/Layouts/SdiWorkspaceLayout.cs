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
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFile = propertyService.ConfigDirectory + "MdiLayoutConfig.xml";
		Form wbForm;
		
		DockingManager dockManager;
		ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab tabControl = new ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab();
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (tabControl == null || tabControl.SelectedIndex < 0 || tabControl.SelectedIndex >= tabControl.TabPages.Count)  {
					return null;
				}
				return (IWorkbenchWindow)tabControl.SelectedTab.Tag;
			}
		}
		
//		void LeftSelectionChanged(object sender, EventArgs e)
//		{
//			if (tabControlLeft.SelectedTab == null) {
//				return;
//			}
//			leftContent.Title = tabControlLeft.SelectedTab.Title;
//		}
//		
//		void BottomSelectionChanged(object sender, EventArgs e)
//		{
//			if (tabControlBottom.SelectedTab == null) {
//				return;
//			}
//			bottomContent.Title = tabControlBottom.SelectedTab.Title;
//		}
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = (Form)workbench;
			wbForm.Controls.Clear();
			
			tabControl.Dock = DockStyle.Fill;
			tabControl.ShrinkPagesToFit = true;
			tabControl.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
			wbForm.Controls.Add(tabControl);
			
			dockManager = new DockingManager(wbForm, Crownwood.Magic.Common.VisualStyle.IDE);
			
//			Control firstControl = null;
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			wbForm.Controls.Add(statusBarService.Control);
			
			((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(((DefaultWorkbench)workbench).TopMenu);
			foreach (CommandBar toolBar in ((DefaultWorkbench)workbench).ToolBars) {
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
			tabControl.SelectionChanged += new EventHandler(ActiveMdiChanged);
			
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
				
				foreach (Crownwood.Magic.Controls.TabPage page in tabControl.TabPages) {
					SdiWorkspaceWindow f = (SdiWorkspaceWindow)page.Tag;
					f.DetachContent();
					f.ViewContent = null;
				}
				
				tabControl.TabPages.Clear();
				tabControl.Controls.Clear();
				
				if (dockManager != null) {
					dockManager.Contents.Clear();
				}
				
				wbForm.Controls.Clear();
			} catch (Exception) {}
		}
		
//		WindowContent leftContent = null;
//		WindowContent bottomContent = null;
		Hashtable contentHash = new Hashtable();
		
	
		public void ShowPad(IPadContent content)
		{
			if (contentHash[content] == null) {
				IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
				content.Control.Dock = DockStyle.None;
				Content newContent;
				if (content.Icon != null) {
					ImageList imgList = new ImageList();
					imgList.ColorDepth = ColorDepth.Depth32Bit;
					IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
					imgList.Images.Add(iconService.GetBitmap(content.Icon));
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
			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					return content.Visible;
				}
			}
			return false;
		}
		
		public void HidePad(IPadContent padContent)
		{
			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					dockManager.HideContent(content);
				}
			}
		}
		
		public void ActivatePad(IPadContent padContent)
		{
			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					content.BringToFront();
				}
			}
		}
		
		public void RedrawAllComponents()
		{
			tabControl.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.TabVisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			// redraw correct pad content names (language changed).
			foreach (IPadContent content in ((IWorkbench)wbForm).PadContentCollection) {
				Content c = (Content)contentHash[content];
				if (c != null) {
					c.Title = c.FullTitle = content.Title;
				}
			}
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
			SdiWorkspaceWindow f = (SdiWorkspaceWindow)sender;
			if (f.ViewContent != null) {
				((IWorkbench)wbForm).CloseContent(f.ViewContent);
				ActiveMdiChanged(this, null);
			}
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			content.Control.Dock = DockStyle.None;
			content.Control.Visible = true;
			SdiWorkspaceWindow sdiWorkspaceWindow = new SdiWorkspaceWindow(content, tabControl);
			sdiWorkspaceWindow.TabPage = tabControl.AddWindow(sdiWorkspaceWindow);
			
			sdiWorkspaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
			return sdiWorkspaceWindow;
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
