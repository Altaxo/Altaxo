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
using System.Reflection;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using Reflector.UserInterface;
using WeifenLuo.WinFormsUI;

namespace ICSharpCode.SharpDevelop.Gui
{	
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFileName    = "MdiLayoutConfig3.xml";
		static string configFile        = Path.Combine(propertyService.ConfigDirectory, configFileName);
		Form wbForm;
		
		DockPanel dockPanel;
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (dockPanel == null || dockPanel.ActiveDocument == null)  {
					return null;
				}
				return dockPanel.ActiveDocument as IWorkbenchWindow;
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = (Form)workbench;
			wbForm.Show();
			wbForm.Controls.Clear();
			dockPanel = new WeifenLuo.WinFormsUI.DockPanel();
			wbForm.Controls.Add(this.dockPanel);
			
			this.dockPanel.ActiveAutoHideContent = null;
			this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			
			((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(((DefaultWorkbench)workbench).TopMenu);
			foreach (CommandBar toolBar in ((DefaultWorkbench)workbench).ToolBars) {
				((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(toolBar);
			}
			
			wbForm.Menu = null;
			dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
			try {
				if (File.Exists(configFile)) {
					dockPanel.LoadFromXml(configFile, new DeserializeDockContent(GetContent));
				} else {
					dockPanel.LoadFromXml(Assembly.GetCallingAssembly().GetManifestResourceStream(configFileName), new DeserializeDockContent(GetContent));
				}
			} catch (Exception) {
				Console.WriteLine("can't load docking configuration, version clash ?");
			}
			
			foreach (IPadContent content in workbench.PadContentCollection) {
				if (this.contentHash[content] == null) {
					ShowPad(content);
				}
			}
			foreach (IViewContent content in workbench.ViewContentCollection) {
				ShowView(content);
			}
			
			RedrawAllComponents();
			
			
			wbForm.Controls.Add(((DefaultWorkbench)workbench).commandBarManager);
			wbForm.Controls.Add(statusBarService.Control);
			
		}
		
		DockContent GetContent(string padTypeName)
		{
			foreach (IPadContent content in ((DefaultWorkbench)wbForm).PadContentCollection) {
				if (content.GetType().ToString() == padTypeName) {
					return CreateContent(content);
				}
			}
			return null;
		}
		
		public void Detach()
		{
			try {
				if (dockPanel != null) {
					dockPanel.SaveAsXml(configFile);
				}
				
				foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
					SdiWorkspaceWindow f = (SdiWorkspaceWindow)viewContent.WorkbenchWindow;
					f.DetachContent();
					f.ViewContent = null;
//					f.Dispose();
				}
				
//				tabControl.TabPages.Clear();
//				tabControl.Controls.Clear();
				
//				if (dockPanel != null) {
//					dockPanel.Contents.Clear();
//				}
				
				wbForm.Controls.Clear();
				if (dockPanel != null) {
					dockPanel.Dispose();
					dockPanel = null;
				}
				
			} catch (Exception) {}
		}
		
//		WindowContent leftContent = null;
//		WindowContent bottomContent = null;
		Hashtable contentHash = new Hashtable();
		
		class PadContentWrapper : DockContent
		{
			IPadContent content;
			
			public PadContentWrapper(IPadContent content)
			{
				this.content = content;
				this.DockableAreas = ((((WeifenLuo.WinFormsUI.DockAreas.Float | WeifenLuo.WinFormsUI.DockAreas.DockLeft) | 
				                        WeifenLuo.WinFormsUI.DockAreas.DockRight) | 
				                        WeifenLuo.WinFormsUI.DockAreas.DockTop) | 
				                        WeifenLuo.WinFormsUI.DockAreas.DockBottom);
			}
			
			protected override string GetPersistString()
			{
				return content.GetType().ToString();
			}
			
			protected override void Dispose(bool disposing)
			{
				base.Hide();
//				base.Dispose(disposing);
//				if (disposing) {
//					if (content != null) {
//						content.Dispose();
//						content = null;
//					}
//				}
			}
		}
		
		DockContent CreateContent(IPadContent content)
		{
			if (contentHash[content] != null) {
				return contentHash[content] as DockContent;
			}
			IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
			
			DockContent newContent = new PadContentWrapper(content);
			if (content.Icon != null) {
				IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
				newContent.Icon = iconService.GetIcon(content.Icon);
			}
			content.Control.Dock = DockStyle.Fill;
			newContent.Controls.Add(content.Control);
			newContent.Text = content.Title;
			contentHash[content] = newContent;
			
			return newContent;
		}
		
		public void ShowPad(IPadContent content)
		{
			if (contentHash[content] == null) {
				DockContent newContent = CreateContent(content);
				newContent.Show(dockPanel);
			} else {
				DockContent c = (DockContent)contentHash[content];
				c.Show(dockPanel);
			}
		}
		
		public bool IsVisible(IPadContent padContent)
		{
			if (padContent != null) {
				DockContent content = (DockContent)contentHash[padContent];
				if (content != null) {
					return !content.IsHidden;
				}
			}
			return false;
		}
		
		public void HidePad(IPadContent padContent)
		{
			if (padContent != null) {
				DockContent content = (DockContent)contentHash[padContent];
				if (content != null) {
					content.Hide();
				}
			}
		}
		
		public void ActivatePad(IPadContent padContent)
		{
			if (padContent != null) {
				DockContent content = (DockContent)contentHash[padContent];
				content.Show();
			}
		}
		
		public void RedrawAllComponents()
		{
			// redraw correct pad content names (language changed).
			foreach (IPadContent content in ((IWorkbench)wbForm).PadContentCollection) {
				DockContent c = (DockContent)contentHash[content];
				if (c != null) {
					c.Text = content.Title;
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
			content.Control.Visible = true;
			content.Control.Dock = DockStyle.Fill;
			
			SdiWorkspaceWindow sdiWorkspaceWindow = new SdiWorkspaceWindow(content);
			sdiWorkspaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
			sdiWorkspaceWindow.Show(dockPanel);
			return sdiWorkspaceWindow;
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			OnActiveWorkbenchWindowChanged(e);
		}
		
		IWorkbenchWindow oldSelectedWindow = null;
		public virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
			if (oldSelectedWindow != null) {
				oldSelectedWindow.OnWindowDeselected(EventArgs.Empty);
			}
			oldSelectedWindow = ActiveWorkbenchwindow;
			if (oldSelectedWindow != null) {
				oldSelectedWindow.OnWindowSelected(EventArgs.Empty);
				oldSelectedWindow.ActiveViewContent.SwitchedTo();
				oldSelectedWindow.ActiveViewContent.Control.Select();

			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
