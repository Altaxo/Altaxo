// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class OpenTaskView : ListView, IPadContent
	{
		Panel     myPanel  = new Panel();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public Control Control {
			get {
				return myPanel;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.TaskList");
			}
		}
		
		public string Icon {
			get {
				return "Icons.16x16.TaskListIcon";
			}
		}
		
		public void RedrawContent()
		{
			line.Text        = resourceService.GetString("CompilerResultView.LineText");
			description.Text = resourceService.GetString("CompilerResultView.DescriptionText");
			file.Text        = resourceService.GetString("CompilerResultView.FileText");
			path.Text        = resourceService.GetString("CompilerResultView.PathText");
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		ColumnHeader type        = new ColumnHeader();
		ColumnHeader line        = new ColumnHeader();
		ColumnHeader description = new ColumnHeader();
		ColumnHeader file        = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ToolTip taskToolTip = new ToolTip();
		
		public OpenTaskView()
		{
			
			type.Text = "!";
			
			RedrawContent();
			
			Columns.Add(type);
			Columns.Add(line);
			Columns.Add(description);
			Columns.Add(file);
			Columns.Add(path);
			
			FullRowSelect = true;
			AutoArrange = true;
			Alignment   = ListViewAlignment.Left;
			View = View.Details;
			Dock = DockStyle.Fill;
			GridLines  = true;
			Activation = ItemActivation.OneClick;
			OnResize(null);
			
			TaskService taskService        = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			taskService.TasksChanged  += new EventHandler(ShowResults);
			
			projectService.EndBuild   += new EventHandler(SelectTaskView);
			
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
			
			myPanel.Controls.Add(this);
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Error"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Warning"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Information"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Question"));
			this.SmallImageList = this.LargeImageList = imglist;
			
//			type.Width = 24;
//			line.Width = 50;
//			description.Width = 600;
//			file.Width = 150;
//			path.Width = 300;
			
			// Set up the delays for the ToolTip.
			taskToolTip.InitialDelay = 500;
			taskToolTip.ReshowDelay = 100;
			taskToolTip.AutoPopDelay = 5000;
//
//			// Force the ToolTip text to be displayed whether or not the form is active.
//			taskToolTip.ShowAlways   = false;

			this.CreateControl();
		}
		
		protected override void Dispose(bool disposing)
		{
//			if (disposing) {
//				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
//				propertyService.SetProperty("CompilerResultView.typeWidth", type.Width);
//				propertyService.SetProperty("CompilerResultView.lineWidth", line.Width);
//				propertyService.SetProperty("CompilerResultView.descriptionWidth", description.Width);
//				propertyService.SetProperty("CompilerResultView.fileWidth", file.Width);
//				propertyService.SetProperty("CompilerResultView.pathWidth", path.Width);
//			}
			base.Dispose(disposing);
		}
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			Items.Clear();
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			Items.Clear();
		}
		
		void SelectTaskView(object sender, EventArgs e)
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			if (taskService.Tasks.Count > 0) {
				try {
					Invoke(new EventHandler(SelectTaskView2));
				} catch {}
			}
		}
		
		void SelectTaskView2(object sender, EventArgs e)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
			} else {
				if ((bool)propertyService.GetProperty("SharpDevelop.ShowTaskListAfterBuild", true)) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
					WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
				}
			}
		}
		
		protected override void OnItemActivate(EventArgs e)
		{
			base.OnItemActivate(e);
			
			if (FocusedItem != null) {
				Task task = (Task)FocusedItem.Tag;
				Debug.Assert(task != null);
				task.JumpToPosition();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			ListViewItem item = base.GetItemAt(e.X, e.Y);
			if (item != null) {
				Task task = (Task)item.Tag;
				taskToolTip.SetToolTip(this, task.Description);
				taskToolTip.Active       = true;
			} else {
				taskToolTip.RemoveAll(); 
				taskToolTip.Active       = false;
			}
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			type.Width = 24;
			line.Width = 50;
			int w = Width - type.Width - line.Width;
			file.Width = w * 15 / 100;
			path.Width = w * 15 / 100;
			description.Width = w - file.Width - path.Width - 5;
		}
		
		public CompilerResults CompilerResults = null;
		
		void ShowResults2(object sender, EventArgs e)
		{
			Console.WriteLine("Create Open Task View Handle:" + base.Handle);
			
			BeginUpdate();
			Items.Clear();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			foreach (Task task in taskService.Tasks) {
				int imageIndex = 0;
				switch (task.TaskType) {
					case TaskType.Warning:
						imageIndex = 1;
						break;
					case TaskType.Error:
						imageIndex = 0;
						break;
					case TaskType.Comment:
						imageIndex = 3;
						break;
					case TaskType.SearchResult:
						imageIndex = 2;
						break;
				}
				
				string tmpPath = task.FileName;
				if (task.Project != null) {
					tmpPath = fileUtilityService.AbsoluteToRelativePath(task.Project.BaseDirectory, task.FileName);
				} 
				
				string fileName = tmpPath;
				string path     = tmpPath;
				
				try {
					fileName = Path.GetFileName(tmpPath);
				} catch (Exception) {}
				
				try {
					path = Path.GetDirectoryName(tmpPath);
				} catch (Exception) {}
				
				ListViewItem item = new ListViewItem(new string[] {
					String.Empty,
					(task.Line + 1).ToString(),
					task.Description,
					fileName,
					path
				});
				item.ImageIndex = item.StateImageIndex = imageIndex;
				item.Tag = task;
				Items.Add(item);
			}
			EndUpdate();
		}
		
		public void ShowResults(object sender, EventArgs e)
		{
			Invoke(new EventHandler(ShowResults2));
//			SelectTaskView(null, null);
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
		
		public new void BringToFront()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}

	}
}
