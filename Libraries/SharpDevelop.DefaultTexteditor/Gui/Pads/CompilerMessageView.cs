// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	// Note: I moved the pads to this assembly, because I want no cyclic dll dependency
	// on the ICSharpCode.TextEditor assembly.
	
	/// <summary>
	/// This class displays the errors and warnings which the compiler outputs and
	/// allows the user to jump to the source of the warnig / error
	/// </summary>
	public class CompilerMessageView : IPadContent
	{
		TextEditorControl textEditorControl = new TextEditorControl();
		Panel       myPanel = new Panel();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public Control Control {
			get {
				return myPanel;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.OutputWindow");
			}
		}
		
		public string Icon {
			get {
				return "Icons.16x16.OutputIcon";
			}
		}
		
		public void Dispose()
		{
		}
		
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		public CompilerMessageView()
		{
			textEditorControl.Dock     = DockStyle.Fill;
			textEditorControl.Document.ReadOnly = true;
			textEditorControl.ShowHRuler       = false;
			textEditorControl.ShowVRuler       = false;
			textEditorControl.ShowLineNumbers  = false;
			textEditorControl.ShowInvalidLines = false;			
			textEditorControl.ShowEOLMarkers   = false;
			textEditorControl.EnableFolding    = false;
			textEditorControl.IsIconBarVisible = false;
//			textEditorControl.ScrollMarginHeight = 0;
			
			textEditorControl.VisibleChanged += new EventHandler(ActivateTextBox);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			textEditorControl.Font = resourceService.LoadFont("Courier New", 10);
			myPanel.Controls.Add(textEditorControl);
			
			TaskService     taskService    = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			taskService.CompilerOutputChanged += new EventHandler(SetOutput);
			
			projectService.StartBuild    += new EventHandler(SelectMessageView);
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
		}
		
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			textEditorControl.Document.TextContent = String.Empty;
			textEditorControl.Refresh();
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			textEditorControl.Document.TextContent = String.Empty;
			textEditorControl.Refresh();
		}
		
		void SelectMessageView(object sender, EventArgs e)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
			} else { 
				if ((bool)propertyService.GetProperty("SharpDevelop.ShowOutputWindowAtBuild", true)) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
					WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
				}
			}
			
		}
		
		void SetOutput2(object sender, EventArgs e)
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			try {
				textEditorControl.Document.TextContent = taskService.CompilerOutput;
				UpdateTextArea();
			} catch (Exception) {}
			
			System.Threading.Thread.Sleep(100);
		}
		
		void UpdateTextArea()
		{
			Console.WriteLine("Create CompilerMessage View Handle:" + textEditorControl.Handle);
			
			textEditorControl.ActiveTextAreaControl.Caret.Position = textEditorControl.Document.OffsetToPosition(textEditorControl.Document.TextLength);
			textEditorControl.ActiveTextAreaControl.ScrollToCaret();
			textEditorControl.Refresh();
		}
		string outputText = null;
		void SetOutput(object sender, EventArgs e)
		{
			Console.WriteLine("Create CompilerMessage View Handle:" + textEditorControl.Handle);
			
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				textEditorControl.Invoke(new EventHandler(SetOutput2));
				outputText = null;
			} else {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				outputText = taskService.CompilerOutput;
				UpdateTextArea();
			}
		}
		
		void ActivateTextBox(object sender, EventArgs e)
		{
			if (outputText != null && textEditorControl.Visible) {
				textEditorControl.Document.TextContent = outputText;
				UpdateTextArea();
				outputText = null;
			}
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

		public void BringToFront()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}

	}
}
