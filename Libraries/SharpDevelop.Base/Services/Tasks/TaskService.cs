// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Services
{
	public class TaskService : AbstractService
	{
		ArrayList tasks          = new ArrayList();
		ArrayList commentTasks   = new ArrayList();
		string    compilerOutput = String.Empty;
		
		public ArrayList Tasks {
			get {
				return tasks;
			}
		}
		public ArrayList CommentTasks {
			get {
				return commentTasks;
			}
		}
		
		int warnings = 0;
		int errors   = 0;
		int comments = 0;
		
		public int Warnings {
			get {
				return warnings;
			}
		}
		
		public int Errors {
			get {
				return errors;
			}
		}
		
		public int Comments {
			get {
				return comments;
			}
		}
		
		public bool SomethingWentWrong {
			get {
				return errors + warnings > 0;
			}
		}
		
		public string CompilerOutput {
			get {
				return compilerOutput;
			}
			set {
				compilerOutput = value;
				OnCompilerOutputChanged(null);
			}
		}
		
		protected virtual void OnCompilerOutputChanged(EventArgs e)
		{
			if (CompilerOutputChanged != null) {
				CompilerOutputChanged(this, e);
			}
		}
		
		protected virtual void OnTasksChanged(EventArgs e)
		{
			if (TasksChanged != null) {
				TasksChanged(this, e);
			}
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			IFileService fileService = (IFileService)ServiceManager.Services.GetService(typeof(IFileService));
			fileService.FileRenamed += new FileEventHandler(CheckFileRename);
			fileService.FileRemoved += new FileEventHandler(CheckFileRemove);
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			tasks.Clear();
			commentTasks.Clear();
			NotifyTaskChange();
		}
		
		void CheckFileRemove(object sender, FileEventArgs e)
		{
			bool somethingChanged = false;
			for (int i = 0; i < tasks.Count; ++i) {
				Task curTask = (Task)tasks[i];
				bool doRemoveTask = false;
				try {
					doRemoveTask = Path.GetFullPath(curTask.FileName) == Path.GetFullPath(e.FileName);
				} catch {
					doRemoveTask = curTask.FileName == e.FileName;
				}
				if (doRemoveTask) {
					tasks.RemoveAt(i);
					--i;
					somethingChanged = true;
				}
			}
			
			if (somethingChanged) {
				NotifyTaskChange();
			}
		}
		
		void CheckFileRename(object sender, FileEventArgs e)
		{
			bool somethingChanged = false;
			foreach (Task curTask in tasks) {
				if (Path.GetFullPath(curTask.FileName) == Path.GetFullPath(e.SourceFile)) {
					curTask.FileName = Path.GetFullPath(e.TargetFile);
					somethingChanged = true;
				}
			}
			
			foreach (Task curTask in commentTasks) {
				if (Path.GetFullPath(curTask.FileName) == Path.GetFullPath(e.SourceFile)) {
					curTask.FileName = Path.GetFullPath(e.TargetFile);
					somethingChanged = true;
				}
			}
			
			
			if (somethingChanged) {
				NotifyTaskChange();
			}
		}
		
		public void RemoveCommentTasks(string fileName)
		{
			bool removed = false;
			for (int i = 0; i < commentTasks.Count; ++i) {
				Task task = (Task)commentTasks[i];
				if (Path.GetFullPath(task.FileName) == Path.GetFullPath(fileName)) {
					commentTasks.RemoveAt(i);
					removed = true;
					--i;
				}
			}
			if (removed) {
				NotifyTaskChange();
			}
		}
		
		public void NotifyTaskChange()
		{
			warnings = errors = comments = 0;
			foreach (Task task in tasks) {
				switch (task.TaskType) {
					case TaskType.Warning:
						++warnings;
						break;
					case TaskType.Error:
						++errors;
						break;
					default:
						++comments;
						break;
				}
			}
			OnTasksChanged(null);
		}
		
		public event EventHandler TasksChanged;
		public event EventHandler CompilerOutputChanged;
	}

}
