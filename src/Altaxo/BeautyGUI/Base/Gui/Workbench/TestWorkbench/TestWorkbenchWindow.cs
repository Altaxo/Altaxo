// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TestWorkbenchWindow : IWorkbenchWindow
	{
		string       title;
		IViewContent viewContent;
		
		EventHandler setTitleEvent = null;
		
		public string Title {
			get {
				return title;
			}
			set {
				title = value;
			}
		}
		
		public IViewContent ViewContent {
			get {
				return viewContent;
			}
		}
		public TestWorkbenchWindow(IViewContent viewContent)
		{
			this.viewContent = viewContent;
			viewContent.WorkbenchWindow = this;
			
			setTitleEvent = new EventHandler(SetTitleEvent);
			viewContent.ContentNameChanged += setTitleEvent;
			viewContent.DirtyChanged    += setTitleEvent;
			SetTitleEvent(null, null);
		}
		string myUntitledTitle =null;
		public void SetTitleEvent(object sender, EventArgs e)
		{
			if (viewContent == null) {
				return;
			}
			string newTitle = "";
			if (viewContent.ContentName == null) {
				if (myUntitledTitle == null) {
					string baseName  = Path.GetFileNameWithoutExtension(viewContent.UntitledName);
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
				newTitle = viewContent.ContentName;
			}
			
			if (viewContent.IsDirty) {
				newTitle += "*";
			} else if (viewContent.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Title = newTitle;
			}
		}
		
		public void CloseWindow(bool force)
		{
			WorkbenchSingleton.Workbench.CloseContent(ViewContent);
		}
		
		public void SelectWindow()
		{
			((TestLayoutManager)WorkbenchSingleton.Workbench.WorkbenchLayout).ActiveWorkbenchwindow = this;
			OnWindowSelected(EventArgs.Empty);
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnCloseEvent(EventArgs e)
		{
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
