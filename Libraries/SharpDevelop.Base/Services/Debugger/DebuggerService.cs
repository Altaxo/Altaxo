// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DebuggerService : IService
	{
		System.Diagnostics.Process standardProcess = null;
		bool                       isRunning       = false;
		IDebugger                  defaultDebugger = null;
		ArrayList                  debugger        = null;
		
		public IDebugger CurrentDebugger {
			get {
				if (debugger != null) {
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					// TODO: not really correct :/ it would be better to have the 'real' startup project, but this will do for now.
					IProject project = projectService.CurrentSelectedProject;
					foreach (IDebugger d in debugger) {
						if (d.CanDebug(project)) {
							return d;
						}
					}
				}
				if (defaultDebugger == null) {
					defaultDebugger = new DefaultDebugger();
				}
				return defaultDebugger;
			}
		}
		
		public bool IsProcessRuning {
			get {
				return isRunning;
			}
		}
		
		public DebuggerService()
		{
			
//			DebugStopped += new EventHandler(HandleDebugStopped);
		}
		MessageViewCategory debugCategory = null;
		
		void EnsureDebugCategory()
		{
			if (debugCategory == null) {
				debugCategory = new MessageViewCategory("Debug", "${res:MainWindow.Windows.OutputWindow.DebugCategory}");
				CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView));
				compilerMessageView.AddCategory(debugCategory);
			}
		}
		public void ClearDebugMessages()
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}
		public void PrintDebugMessage(string msg)
		{
			EnsureDebugCategory();
			debugCategory.AppendText(msg);
		}
		
		void HandleDebugStopped(object sender, EventArgs e)
		{
			//// Alex: if stopped - kill process which might be running or stuck
			if (standardProcess != null) {
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess = null;
			}
			IDebugger debugger = CurrentDebugger;
			if (debugger != null) {
				debugger.Stop();
			}
			isRunning = false;
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
		#region ICSharpCode.Core.Services.IService interface implementation
		public void InitializeService()
		{
			OnInitialize(EventArgs.Empty);
			IAddInTreeNode treeNode = null;
			try {
				treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Services/DebuggerService/Debugger");
			} catch (Exception e) {
				Console.WriteLine(e);
			}
			if (treeNode != null) {
				debugger = treeNode.BuildChildItems(this);
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.CombineOpened += new CombineEventHandler(ClearOnCombineEvent);
//			CurrentDebugger.Start(@"C:\bla.exe", @"C:\", "");
		}
		
		void DebuggerServiceStarted(object sender, EventArgs e)
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
			CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView));
			compilerMessageView.SelectCategory("Debug");
		}
		
		void ClearOnCombineEvent(object sender, CombineEventArgs e)
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}
		
		public void UnloadService()
		{
			OnUnload(EventArgs.Empty);
			Stop();
		}
		protected virtual void OnInitialize(EventArgs e)
		{
			if (Initialize != null) {
				Initialize(this, e);
			}
		}
		protected virtual void OnUnload(EventArgs e)
		{
			if (Unload != null) {
				Unload(this, e);
			}
		}
		
		public event EventHandler Initialize;
		public event EventHandler Unload;
		#endregion
		
		public void GotoSourceFile(string fileName, int lineNumber, int column)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.JumpToFilePosition(fileName, lineNumber, column);
		}
		
		public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{
			if (IsProcessRuning) {
				return;
			}
			try {
				standardProcess = new System.Diagnostics.Process();
				standardProcess.StartInfo = psi;
				standardProcess.Exited += new EventHandler(StandardProcessExited);
				standardProcess.EnableRaisingEvents = true;
				standardProcess.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + psi.FileName + "\"\n");
			}
			isRunning = true;
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}

		public void Start(string fileName, string workingDirectory, string arguments)
		{
			if (IsProcessRuning) {
				return;
			}
			IDebugger debugger = CurrentDebugger;
			if (debugger != null) {
				debugger.Start(fileName, arguments, workingDirectory);
			}
			
//			lock (breakpoints) {
//				foreach (Breakpoint breakpoint in breakpoints) {
//					if (breakpoint.Enabled) {
//						brea.AddBreakpoint(fileName, breakpoint.FileName, breakpoint.Line);
//					}
//				}
//			}
			isRunning = true;
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
		public void Break()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger != null && debugger.SupportsExecutionControl) {
				debugger.Break();
			}
		}
		
		public void Continue()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger != null && debugger.SupportsExecutionControl) {
				debugger.Continue();
			}
		}

		public void Step(bool stepInto)
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger == null || !debugger.SupportsStepping) {
				return;
			}
			if (stepInto) {
				debugger.StepInto();
			} else {
				debugger.StepOver();
			}
		}
		
		public void StepOut()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger == null || !debugger.SupportsStepping) {
				return;
			}
			debugger.StepOut();
		}
		
		public void Stop()
		{
			if (standardProcess != null) {
//				OnTextMessage(new TextMessageEventArgs(String.Format("Killing {0}{1}\n",standardProcess.ProcessName,Environment.NewLine)));
				standardProcess.Exited -= new EventHandler(StandardProcessExited);
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess.Dispose();
				standardProcess = null;
			} else {
				IDebugger debugger = CurrentDebugger;
				if (debugger != null) {
					debugger.Stop();
				}
			}
			isRunning = false;
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
		void StandardProcessExited(object sender, EventArgs e)
		{
			standardProcess.Exited -= new EventHandler(StandardProcessExited);
			standardProcess.Dispose();
			standardProcess = null;	
			isRunning       = false;
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
//		protected override void OnException(ExceptionEventArgs e)
//		{
//			base.OnException(e);
//			OnTextMessage(new TextMessageEventArgs("Got Exception\n"));
//			StopDebugger();
//		}
//		
//		protected override void OnProcessExited(ProcessEventArgs e)
//		{
//			OnTextMessage(new TextMessageEventArgs(String.Format("The program '[{1}] {0}' exited with code {2}.{3}\n",
//			                                                 "Unknown",
//			                                                 e.Process.ID,
//			                                                 "Unknown",Environment.NewLine)));
//			base.OnProcessExited(e);
//		}
//		protected override void OnModuleLoaded(ModuleEventArgs e)
//		{
//			OnTextMessage(new TextMessageEventArgs(String.Format("'{0}' : '{1}' loaded, {2}.{3}\n",
//			                                                 "Unknown",
//			                                                 e.Module.Name,
//			                                                 "Unknown",Environment.NewLine)));
//			base.OnModuleLoaded(e);
//		}
	}
}
