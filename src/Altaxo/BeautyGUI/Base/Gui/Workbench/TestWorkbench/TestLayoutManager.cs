// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TestLayoutManager : IWorkbenchLayout
	{
		IWorkbenchWindow activeWorkbenchwindow = null;
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				return activeWorkbenchwindow;
			} 
			set {
				activeWorkbenchwindow = value;
				OnActiveWorkbenchWindowChanged(this, EventArgs.Empty);
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			foreach (IViewContent content in workbench.ViewContentCollection) {
				ShowView(content);
			}
			
			foreach (IPadContent content in workbench.PadContentCollection) {
				ShowPad(content);
			}
		}
		
		public void Detach()
		{
		}
		
		public void ShowPad(IPadContent content)
		{
		}
		
		public void ActivatePad(IPadContent content)
		{
		}
		
		public void HidePad(IPadContent content)
		{
		}
		
		public bool IsVisible(IPadContent padContent)
		{
			return true;
		}
		
		public void RedrawAllComponents()
		{
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			ActiveWorkbenchwindow = new TestWorkbenchWindow(content);
			return ActiveWorkbenchwindow;
		}
		
		void OnActiveWorkbenchWindowChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
