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
	public class OpenTaskView : AbstractPadContent
	{
		ListView listView = new ListView();
		
		ColumnHeader type        = new ColumnHeader();
		ColumnHeader line        = new ColumnHeader();
		ColumnHeader description = new ColumnHeader();
		ColumnHeader file        = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ToolTip taskToolTip = new ToolTip();
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		public override Control Control {
			get {
				return listView;
			}
		}
		
		public OpenTaskView() : base("${res:MainWindow.Windows.TaskList}", "Icons.16x16.TaskListIcon")
		{
			type.Text = "!";
			
			RedrawContent();
			listView.Columns.Add(type);
			listView.Columns.Add(line);
			listView.Columns.Add(description);
			listView.Columns.Add(file);
			listView.Columns.Add(path);
			
			listView.FullRowSelect = true;
			listView.AutoArrange = true;
			listView.Alignment   = ListViewAlignment.Left;
			listView.View = View.Details;
			listView.Dock = DockStyle.Fill;
			listView.GridLines  = true;
			listView.Activation = ItemActivation.OneClick;
			ListViewResize(this, EventArgs.Empty);
			
			TaskService taskService        = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			taskService.TasksChanged  += new EventHandler(ShowResults);
			
			projectService.EndBuild   += new EventHandler(SelectTaskView);
			
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Error"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Warning"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Information"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Question"));
			listView.SmallImageList = listView.LargeImageList = imglist;
			// Set up the delays for the ToolTip.
			taskToolTip.InitialDelay = 500;
			taskToolTip.ReshowDelay = 100;
			taskToolTip.AutoPopDelay = 5000;
//
//			// Force the ToolTip text to be displayed whether or not the form is active.
//			taskToolTip.ShowAlways   = false;
			
			listView.ItemActivate += new EventHandler(ListViewItemActivate);
			listView.MouseMove    += new MouseEventHandler(ListViewMouseMove);
			listView.Resize       += new EventHandler(ListViewResize);
		}
		
		
		public override void RedrawContent()
		{
			line.Text        = resourceService.GetString("CompilerResultView.LineText");
			description.Text = resourceService.GetString("CompilerResultView.DescriptionText");
			file.Text        = resourceService.GetString("CompilerResultView.FileText");
			path.Text        = resourceService.GetString("CompilerResultView.PathText");
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			listView.Items.Clear();
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			listView.Items.Clear();
		}
		
		void SelectTaskView(object sender, EventArgs e)
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			if (taskService.Tasks.Count > 0) {
				try {
					listView.Invoke(new EventHandler(SelectTaskView2));
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
		
		void ListViewItemActivate(object sender, EventArgs e)
		{
			if (listView.FocusedItem != null) {
				Task task = (Task)listView.FocusedItem.Tag;
				System.Diagnostics.Debug.Assert(task != null);
				task.JumpToPosition();
			}
		}
		
		void ListViewMouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = listView.GetItemAt(e.X, e.Y);
			if (item != null) {
				Task task = (Task)item.Tag;
				taskToolTip.SetToolTip(listView, task.Description);
				taskToolTip.Active       = true;
			} else {
				taskToolTip.RemoveAll(); 
				taskToolTip.Active       = false;
			}
		}
		
		void ListViewResize(object sender, EventArgs e)
		{
			type.Width = 24;
			line.Width = 50;
			int w = listView.Width - type.Width - line.Width;
			file.Width = w * 15 / 100;
			path.Width = w * 15 / 100;
			description.Width = w - file.Width - path.Width - 5;
		}
		
		public CompilerResults CompilerResults = null;
		
		void AddTasks(ICollection col)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (Task task in col) {
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
				listView.Items.Add(item);
			}
		}
		
		void ShowResults2(object sender, EventArgs e)
		{
			listView.CreateControl();
			
			listView.BeginUpdate();
			listView.Items.Clear();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			AddTasks(taskService.Tasks);
			AddTasks(taskService.CommentTasks);
			
			listView.EndUpdate();
		}
		
		public void ShowResults(object sender, EventArgs e)
		{
			listView.Invoke(new EventHandler(ShowResults2));
//			SelectTaskView(null, null);
		}
	}
}
