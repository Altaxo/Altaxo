// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;


using ICSharpCode.SharpDevelop.Gui.Components;
using Crownwood.Magic.Controls;

namespace ICSharpCode.SharpDevelop.Gui
{
	/*
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class FloatingWorkbenchLayout : IWorkbenchLayout
	{
		Form wbForm;
		ArrayList forms = new ArrayList();
		EventHandler sizeEventHandler;
		
		Crownwood.Magic.Controls.TabControl tabControlLeft   = null;
		Crownwood.Magic.Controls.TabControl tabControlBottom = null;
		
		Form         viewFormLeft     = null;
		Form         viewFormBottom   = null;
		
		OpenFileTab  openFiles = new OpenFileTab();
		
		void SetLeftTitle(object sender, EventArgs e)
		{
			viewFormLeft.Text = tabControlLeft.SelectedTab.TabFlap.Text;
		}
		void SetBottomTitle(object sender, EventArgs e)
		{
			viewFormBottom.Text = tabControlBottom.SelectedTab.TabFlap.Text;
		}
		
		public void InitializeContents()
		{
			if (viewFormLeft == null) {
				viewFormLeft = new Form();
				viewFormLeft.ControlBox = viewFormLeft.MinimizeBox = viewFormLeft.MaximizeBox = false;
				viewFormLeft.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				viewFormLeft.Owner = wbForm;
				
				tabControlLeft = new AxTabControl();
				tabControlLeft.Dock = DockStyle.Fill;
				tabControlLeft.AxTabControlPanel.Click += new EventHandler(SetLeftTitle);
				viewFormLeft.Controls.Add(tabControlLeft);
				if (propertyService.GetProperty("Workspace.ViewMementos.LeftContainerBounds") != null) {
					viewFormLeft.StartPosition = FormStartPosition.Manual;
					viewFormLeft.Bounds = StringToBounds(propertyService.GetProperty("Workspace.ViewMementos.LeftContainerBounds").ToString());
				}
				viewFormLeft.Show();
			}
			
			if (viewFormBottom == null) {
				viewFormBottom = new Form();
				viewFormBottom.ControlBox = viewFormBottom.MinimizeBox = viewFormBottom.MaximizeBox = false;
				viewFormBottom.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				viewFormBottom.Owner = wbForm;
				
				tabControlBottom = new AxTabControl();
				tabControlBottom.Dock = DockStyle.Fill;
				tabControlBottom.AxTabControlPanel.Click += new EventHandler(SetBottomTitle);
				viewFormBottom.Controls.Add(tabControlBottom);
				if (propertyService.GetProperty("Workspace.ViewMementos.BottomContainerBounds") != null) {
					viewFormBottom.StartPosition = FormStartPosition.Manual;
					viewFormBottom.Bounds = StringToBounds(propertyService.GetProperty("Workspace.ViewMementos.BottomContainerBounds").ToString());
				}
				viewFormBottom.Show();
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = ((Form)workbench);
			
			if (workbench.WorkbenchLayout != null) {
				propertyService.SetProperty("SharpDevelop.Workspace.NonFloatingWorkbenchMemento", workbench.CreateMemento());
				((DefaultWorkbench)workbench).FullScreen = false;
			}
			((IWorkbench)wbForm).SetMemento((IXmlConvertable)propertyService.GetProperty("SharpDevelop.Workspace.FloatingWorkbenchMemento", new WorkbenchMemento()));
						
			
			sizeEventHandler = new EventHandler(SetSize);
			wbForm.Resize += sizeEventHandler;
			SetSize(null, null);
			
			foreach (IViewContent content in workbench.ViewContentCollection) {
				ShowContent(content);
			}
			
			foreach (IPadContent content in workbench.PadContentCollection) {
				ShowView(content);
			}
			
			wbForm.Controls.Clear();
			
			ToolBar[] toolBars = ((DefaultWorkbench)workbench).ToolbarManager.CreateToolbars();
			foreach (ToolBar toolBar in toolBars) {
				wbForm.Controls.Add(toolBar);
			}	
		}
		
		Rectangle StringToBounds(string str)
		{
			string[] boundstr = str.Split(new char [] { ',' });
			
			return new Rectangle(Int32.Parse(boundstr[0]), Int32.Parse(boundstr[1]), 
			                     Int32.Parse(boundstr[2]), Int32.Parse(boundstr[3]));
		}
		string  BoundsToString(Rectangle bounds)
		{
			return bounds.X + "," + bounds.Y + "," + bounds.Width + "," + bounds.Height;
		}
		
		public void HideView(IPadContent content)
		{
			foreach (AxTabPage page in tabControlLeft.TabPages) {
				if (page.Controls[0] == content) {
					tabControlLeft.TabPages.Remove(page);
					tabControlLeft.Refresh();
					return;
				}
			}
			foreach (AxTabPage page in tabControlBottom.TabPages) {
				if (page.Controls[0] == content) {
					tabControlBottom.TabPages.Remove(page);
					tabControlBottom.Refresh();
					return;
				}
			}
		}
		
		public void ShowView(IPadContent content)
		{
			InitializeContents();
			IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
			string type = content.GetType().ToString();
			
			AxTabPage page = new AxTabPage(content.Title);
			page.TabFlap.Image = content.Icon;
				
			content.Control.Dock = DockStyle.Fill;
			page.Controls.Add(content.Control);
			
			if (properties.GetProperty(type, "Left") == "Left") {
				tabControlLeft.TabPages.Add(page);
				
				if (tabControlLeft.TabPages.Count == 1) {
					tabControlLeft.SelectedTab = page;
					SetLeftTitle(null, null);
				}
			} else if (properties.GetProperty(type, "Left") == "Bottom") {
				
				tabControlBottom.TabPages.Add(page);
				
				if (tabControlBottom.TabPages.Count == 1) {
					tabControlBottom.SelectedTab = page;
					SetBottomTitle(null, null);
				}
			}
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
			DefaultWorkspaceWindow f = (DefaultWorkspaceWindow)sender;
			if (f.ViewContent != null) {
				((IWorkbench)wbForm).CloseContent(f.ViewContent);
			}
		}
		
		public void SetSize(object sender, EventArgs e)
		{
			if (wbForm.WindowState == FormWindowState.Maximized) {
				wbForm.WindowState = FormWindowState.Normal;
				wbForm.Location = new Point(0, 0);
				wbForm.Width = Screen.PrimaryScreen.Bounds.Width;
			}
				
			if (wbForm.Height != 72) {
				wbForm.Height = 72;
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (wbForm.MdiChildren.Length == 0) { // HACK : ActiveMdiChild may return closed Child !!!
					return null;
				}
				return (IWorkbenchWindow)wbForm.ActiveMdiChild;
			}
		}
		
		void ClosingEvent(object sender, CancelEventArgs e)
		{
			IWorkbenchWindow window = (IWorkbenchWindow)sender;
			
			if (window.ViewContent != null && window.ViewContent.IsDirty) {
				DialogResult dr = MessageBox.Show(
				    resourceService.GetString("MainWindow.SaveChangesMessage"),
					resourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + window.Title + " ?",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (dr) {
					case DialogResult.Yes:
						window.ViewContent.SaveFile();
					break;
					case DialogResult.No:
					break;
					case DialogResult.Cancel:
						e.Cancel = true;
					return;
				}
			}
		}
		
		public IWorkbenchWindow ShowContent(IViewContent content)
		{
			content.Control.Visible = true;
			
			DefaultWorkspaceWindow window = new DefaultWorkspaceWindow(content);
			window.Owner = wbForm;
			((Form)window).Show();
			forms.Add(window);
			
			window.GotFocus += new EventHandler(ActiveMdiChanged);
			window.Closing += new CancelEventHandler(ClosingEvent);
			window.Closed += new EventHandler(CloseWindowEvent);
			openFiles.AddWindow(window);
			return window;
		}
		
		public void RedrawAllComponents()
		{
			foreach (AxTabPage page in tabControlLeft.TabPages) {
				if (page.Controls.Count > 0) {
					foreach (IPadContent content in ((IWorkbench)wbForm).PadContentCollection) {
						if (content.Control == page.Controls[0]) {
							page.TabFlap.Text = content.Title;
							break;
						}
					}
				}
			}
			foreach (AxTabPage page in tabControlBottom.TabPages) {
				if (page.Controls.Count > 0) {
					foreach (IPadContent content in ((IWorkbench)wbForm).PadContentCollection) {
						if (content.Control == page.Controls[0]) {
							page.TabFlap.Text = content.Title;
							break;
						}
					}
				}
			}
		}
		
		public void Detach()
		{
			foreach (DefaultWorkspaceWindow f in forms) {
				f.DetachContent();
				f.ViewContent = null;
				f.Controls.Clear();
				f.Close();
			}
			tabControlLeft.TabPages.Clear();
			tabControlBottom.TabPages.Clear();
			
			if (viewFormLeft != null) {
				propertyService.SetProperty("Workspace.ViewMementos.LeftContainerBounds", BoundsToString(viewFormLeft.Bounds));
				viewFormLeft.Controls.Clear();
				viewFormLeft.Close();
			}
			if (viewFormBottom != null) {
				propertyService.SetProperty("Workspace.ViewMementos.BottomContainerBounds", BoundsToString(viewFormBottom.Bounds));
				viewFormBottom.Controls.Clear();
				viewFormBottom.Close();
			}
			
			wbForm.Resize -= sizeEventHandler;
			propertyService.SetProperty("SharpDevelop.Workspace.FloatingWorkbenchMemento", ((IWorkbench)wbForm).CreateMemento());
			((IWorkbench)wbForm).SetMemento((IXmlConvertable)propertyService.GetProperty("SharpDevelop.Workspace.NonFloatingWorkbenchMemento", new WorkbenchMemento()));
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		public event EventHandler ActiveWorkbenchWindowChanged;
	}*/
}
