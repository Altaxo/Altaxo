// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core.Services;
using ICSharpCode.Debugger;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DebuggerService : THSDebugger, IService
	{
		System.Diagnostics.Process standardProcess = null;
		bool                       isRunning       = false;
//		DebuggerStepper            stepper         = null;
		
		public bool IsProcessRuning {
			get {
				return isRunning;
			}
		}
		
		public DebuggerService()
		{
//			DebugStopped += new EventHandler(HandleDebugStopped);
		}
		
		void HandleDebugStopped(object sender, EventArgs e)
		{
			//// Alex: if stopped - kill process which might be running or stuck
			if (standardProcess != null) {
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess = null;
			}
			StopDebugger();
			isRunning = false;
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
		#region ICSharpCode.Core.Services.IService interface implementation
		public void InitializeService()
		{
			 OnInitialize(EventArgs.Empty);
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
		
		public override void GotoSourceFile(string fileName, int lineNumber, int column)
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
			DebugApplication(fileName, arguments, workingDirectory);
			
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
		
		public void Step(bool stepInto)
		{
//			if (stepper != null) {
//				stepper.Step(stepInto);
//				Continue();
//			}
		}
		
		public void StepOut()
		{
//			if (stepper != null) {
//				stepper.StepOut();
//				Continue();
//			}
		}
		
//		protected override void OnStepComplete(StepEventArgs e)
//		{
//			stepper = e.Thread.Stepper;
//			base.OnStepComplete(e);
//		}
//		
//		protected override void OnBreaked(ThreadEventArgs e)
//		{
//			stepper = e.Thread.Stepper;
//			base.OnBreaked(e);
//		}
//		
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
//				StopDebugger();
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


namespace ICSharpCode.Debugger {
	public class THSDebugger 
	{
		public virtual void GotoSourceFile(string fileName, int lineNumber, int column)
		{
		}
		public void Continue()
		{
		}
		public void Break()
		{
		}
		public void StopDebugger()
		{
		}
		public void DebugApplication(string fileName, string arguments, string workingDirectory)
		{
		}
		public void ToggleBreakpointAt(string file, int y, int x)
		{
		}
		public ArrayList Breakpoints = new ArrayList();
	}
}
