using System;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Services 
{
	public class DefaultDebugger : IDebugger
	{
		System.Diagnostics.Process attachedProcess = null;
		
		public bool IsDebugging {
			get {
				return IsProcessRunning;
			}
		}
		
		public bool IsProcessRunning {
			get {
				return attachedProcess != null;
			}
		}
		
		public bool SupportsStartStop {
			get {
				return true;
			}
		}
		
		public bool SupportsExecutionControl {
			get {
				return false;
			}
		}
		
		public bool SupportsStepping {
			get {
				return false;
			}
		}
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(string fileName, string workingDirectory, string arguments)
		{
			if (attachedProcess != null) {
				return;
			}

			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
			psi.FileName = fileName;
			psi.WorkingDirectory = workingDirectory;
			psi.Arguments = arguments;

			try {
				attachedProcess = new System.Diagnostics.Process();
				attachedProcess.StartInfo = psi;
				attachedProcess.Exited += new EventHandler(AttachedProcessExited);
				attachedProcess.EnableRaisingEvents = true;
				attachedProcess.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + psi.FileName + "\"\n");
			}
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}

		public void Stop()
		{
			if (attachedProcess != null) {
				attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
				attachedProcess.Kill();
				attachedProcess.Close();
				attachedProcess.Dispose();
				attachedProcess = null;
				((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
			}
		}
		
		// ExecutionControl:
		
		public void Break()
		{
			throw new NotSupportedException();
		}
		
		public void Continue()
		{
			throw new NotSupportedException();
		}

		public void StepInto()
		{
			throw new NotSupportedException();
		}

		public void StepOver()
		{
			throw new NotSupportedException();
		}

		public void StepOut()
		{
			throw new NotSupportedException();
		}
		
		void AttachedProcessExited(object sender, EventArgs e)
		{
			attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
			attachedProcess.Dispose();
			attachedProcess = null;	
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
	}
}
