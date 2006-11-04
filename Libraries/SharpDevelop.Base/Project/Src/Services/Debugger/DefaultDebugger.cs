// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1968 $</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public class DefaultDebugger : IDebugger
	{
		Process attachedProcess = null;
		
		public bool IsDebugging {
			get {
				return attachedProcess != null;
			}
		}
		
		public bool IsProcessRunning {
			get {
				return IsDebugging;
			}
		}
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(ProcessStartInfo processStartInfo)
		{
			if (attachedProcess != null) {
				return;
			}
			
			try {
				attachedProcess = new Process();
				attachedProcess.StartInfo = processStartInfo;
				attachedProcess.Exited += new EventHandler(AttachedProcessExited);
				attachedProcess.EnableRaisingEvents = true;
				attachedProcess.Start();
				OnDebugStarted(EventArgs.Empty);
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + processStartInfo.FileName + "\"\n");
			}
		}
		
		void AttachedProcessExited(object sender, EventArgs e)
		{
			attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
			attachedProcess.Dispose();
			attachedProcess = null;
			WorkbenchSingleton.SafeThreadAsyncCall(new Action<EventArgs>(OnDebugStopped),
			                                       EventArgs.Empty);
		}
		
		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			Process.Start(processStartInfo);
		}
		
		public void Stop()
		{
			if (attachedProcess != null) {
				attachedProcess.Exited -= new EventHandler(AttachedProcessExited);
				attachedProcess.Kill();
				attachedProcess.Close();
				attachedProcess.Dispose();
				attachedProcess = null;
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
		// Stepping:
		
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
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		public string GetValueAsString(string variable)
		{
			return null;
		}
		
		/// <summary>
		/// Gets the tooltip control that shows the value of given variable.
		/// Return null if no tooltip is available.
		/// </summary>
		public DebuggerGridControl GetTooltipControl(string variable)
		{
			return null;
		}
		
		public bool CanSetInstructionPointer(string filename, int line, int column)
		{
			return false;
		}
		
		public bool SetInstructionPointer(string filename, int line, int column)
		{
			return false;
		}
		
		
		public event EventHandler DebugStarted;
		
		protected virtual void OnDebugStarted(EventArgs e)
		{
			if (DebugStarted != null) {
				DebugStarted(this, e);
			}
		}


		public event EventHandler IsProcessRunningChanged;
		
		protected virtual void OnIsProcessRunningChanged(EventArgs e)
		{
			if (IsProcessRunningChanged != null) {
				IsProcessRunningChanged(this, e);
			}
		}


		public event EventHandler DebugStopped;

		protected virtual void OnDebugStopped(EventArgs e)
		{
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}
		
		public void Dispose()
		{
			Stop();
		}
	}
}
