// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;


namespace ICSharpCode.SharpDevelop.Gui
{
	public class SdiWorkspaceWindow : IWorkbenchWindow
	{
		IViewContent content;
		Crownwood.Magic.Controls.TabPage tabPage;
		Crownwood.Magic.Controls.TabControl tabControl;
		
		EventHandler setTitleEvent = null;
		string myUntitledTitle = null;
		
		public Crownwood.Magic.Controls.TabPage TabPage {
			get {
				return tabPage;
			}
			set {
				tabPage = value;
				ViewContent.Control.Dock = DockStyle.Fill;
				tabPage.Controls.Add(ViewContent.Control);
			}
		}		
		
		public string Title {
			get {
				return tabPage.Title;
			}
			set {
				tabPage.Title = value;
				string fileName = content.ContentName;
				if (fileName == null) {
					fileName = content.UntitledName;
				}
				if (fileName != null) {
					IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
					tabPage.ImageIndex = iconService.GetImageIndexForFile(fileName);
				}
				OnTitleChanged(null);
			}
		}
		
		void ThreadSafeSelectWindow()
		{
			tabPage.Selected = true;
// KSL, Start to fix the focus problem when changing tabs
			content.Control.Focus();
// KSL End
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (viewContent != this.content) {
					viewContent.WorkbenchWindow.OnWindowDeselected(EventArgs.Empty);
				}
			}
			OnWindowSelected(EventArgs.Empty);
			
		}
		
		public void SelectWindow()	
		{
			try {
				MethodInvoker mi = new MethodInvoker(this.ThreadSafeSelectWindow);
				tabPage.EndInvoke(tabPage.BeginInvoke(mi));
				Thread.Sleep(0);
			} catch (ThreadInterruptedException) {
				//Simply exit....
			}catch (Exception) {
			}
		}
		
		public SdiWorkspaceWindow(IViewContent content, Crownwood.Magic.Controls.TabControl tabControl)
		{
			this.tabControl = tabControl;
			this.content = content;
			tabPage = new Crownwood.Magic.Controls.TabPage("", content.Control);
			tabPage.Tag = this;
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			tabPage.ImageList = iconService.ImageList;
			content.WorkbenchWindow = this;
			tabPage.TabIndexChanged += new EventHandler(LeaveTabPage);
			
			setTitleEvent = new EventHandler(SetTitleEvent);
			content.ContentNameChanged += setTitleEvent;
			content.DirtyChanged       += setTitleEvent;
			SetTitleEvent(null, null);
		}
		
		void LeaveTabPage(object sender, EventArgs e)
		{
			OnWindowDeselected(EventArgs.Empty);
		}
		
		public IViewContent ViewContent {
			get {
				return content;
			}
			set {
				content = value;
			}
		}
		
		public void SetTitleEvent(object sender, EventArgs e)
		{
			if (content == null) {
				return;
			}
			string newTitle = "";
			if (content.ContentName == null) {
				if (myUntitledTitle == null) {
					string baseName  = Path.GetFileNameWithoutExtension(content.UntitledName);
					int    number    = 1;
					bool   found     = true;
					while (found) {
						found = false;
						foreach (IViewContent windowContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
							string title = windowContent.WorkbenchWindow.Title;
							if (title.EndsWith("*") || title.EndsWith("+")) {
								title = title.Substring(0, title.Length - 1);
							}
							if (title == baseName + number) {
								found = true;
								++number;
								break;
							}
						}
					}
					myUntitledTitle = baseName + number;
				}
				newTitle = myUntitledTitle;
			} else {
				newTitle = Path.GetFileName(content.ContentName);
			}
			
			if (content.IsDirty) {
				newTitle += "*";
			} else if (content.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Title = newTitle;
			}
		}
		
		public void DetachContent()
		{
			tabPage.Control = null;
			content.ContentNameChanged -= setTitleEvent;
			content.DirtyChanged       -= setTitleEvent;
		}
		
		public void CloseWindow(bool force)
		{
			if (!force && ViewContent != null && ViewContent.IsDirty) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				DialogResult dr = MessageBox.Show(
				    resourceService.GetString("MainWindow.SaveChangesMessage"),
					resourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + Title + " ?",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (dr) {
					case DialogResult.Yes:
						if (content.ContentName == null) {
							while (true) {
								new ICSharpCode.SharpDevelop.Commands.SaveFileAs().Run();
								if (ViewContent.IsDirty) {
									DialogResult result = MessageBox.Show("Do you really want to discard your changes ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
									if (result == DialogResult.Yes) {
										break;
									}
								} else {
									break;
								}
							}
							
						} else {
							FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
							fileUtilityService.ObservedSave(new FileOperationDelegate(ViewContent.Save), ViewContent.ContentName , FileErrorPolicy.ProvideAlternative);
						}
						break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
						return;
				}
			}
//			tabControl.TabPages.Remove(tabPage);
			OnWindowDeselected(EventArgs.Empty);
			OnCloseEvent(null);
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnCloseEvent(EventArgs e)
		{
			OnWindowDeselected(e);
			if (CloseEvent != null) {
				CloseEvent(this, e);
			}
		}

		public virtual void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) {
				WindowSelected(this, e);
			}
		}
		public virtual void OnWindowDeselected(EventArgs e)
		{
			if (WindowDeselected != null) {
				WindowDeselected(this, e);
			}
		}
		
		public event EventHandler WindowSelected;
		public event EventHandler WindowDeselected;
				
		public event EventHandler TitleChanged;
		public event EventHandler CloseEvent;
	}
}
