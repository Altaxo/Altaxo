// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3516 $</version>
// </file>

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// Runs a process that sends output to standard output and to
	/// standard error.
	/// </summary>
	public class ProcessRunner : IDisposable
	{
		Process process;
		StringBuilder standardOutput = new StringBuilder();
		StringBuilder standardError = new StringBuilder();
		ManualResetEvent endOfOutput = new ManualResetEvent(false);
		int outputStreamsFinished;
		
		/// <summary>
		/// Triggered when the process has exited.
		/// </summary>
		public event EventHandler ProcessExited;
		
		/// <summary>
		/// Triggered when a line of text is read from the standard output.
		/// </summary>
		public event LineReceivedEventHandler OutputLineReceived;
		
		/// <summary>
		/// Triggered when a line of text is read from the standard error.
		/// </summary>
		public event LineReceivedEventHandler ErrorLineReceived;
		
		/// <summary>
		/// Creates a new instance of the <see cref="ProcessRunner"/>.
		/// </summary>
		public ProcessRunner()
		{
			this.LogStandardOutputAndError = true;
		}
		
		/// <summary>
		/// Gets or sets the process's working directory.
		/// </summary>
		public string WorkingDirectory { get; set; }

		/// <summary>
		/// Gets or sets whether standard output is logged to the "StandardOutput" and "StandardError"
		/// properties. When this property is false, output is still redirected to the
		/// OutputLineReceived and ErrorLineReceived events, but the ProcessRunner uses less memory.
		/// The default value is true.
		/// </summary>
		public bool LogStandardOutputAndError { get; set; }
		
		/// <summary>
		/// Gets the standard output returned from the process.
		/// </summary>
		public string StandardOutput {
			get {
				lock (standardOutput)
					return standardOutput.ToString();
			}
		}
		
		/// <summary>
		/// Gets the standard error output returned from the process.
		/// </summary>
		public string StandardError {
			get {
				lock (standardError)
					return standardError.ToString();
			}
		}
		
		/// <summary>
		/// Releases resources held by the <see cref="ProcessRunner"/>
		/// </summary>
		public void Dispose()
		{
			process.Dispose();
			endOfOutput.Close();
		}
		
		/// <summary>
		/// Gets the process exit code.
		/// </summary>
		public int ExitCode {
			get {
				int exitCode = 0;
				if (process != null) {
					exitCode = process.ExitCode;
				}
				return exitCode;
			}
		}
		
		/// <summary>
		/// Waits for the process to exit.
		/// </summary>
		public void WaitForExit()
		{
			WaitForExit(Int32.MaxValue);
		}
		
		/// <summary>
		/// Waits for the process to exit.
		/// </summary>
		/// <param name="timeout">A timeout in milliseconds.</param>
		/// <returns><see langword="true"/> if the associated process has
		/// exited; otherwise, <see langword="false"/></returns>
		public bool WaitForExit(int timeout)
		{
			if (process == null) {
				throw new ProcessRunnerException(StringParser.Parse("${res:ICSharpCode.NAntAddIn.ProcessRunner.NoProcessRunningErrorText}"));
			}
			
			bool exited = process.WaitForExit(timeout);
			
			if (exited) {
				endOfOutput.WaitOne(timeout == int.MaxValue ? Timeout.Infinite : timeout, false);
			}
			
			return exited;
		}
		
		public bool IsRunning {
			get {
				bool isRunning = false;
				
				if (process != null) {
					isRunning = !process.HasExited;
				}
				
				return isRunning;
			}
		}
		
		/// <summary>
		/// Starts the process.
		/// </summary>
		/// <param name="command">The process filename.</param>
		/// <param name="arguments">The command line arguments to
		/// pass to the command.</param>
		public void Start(string command, string arguments)
		{
			process = new Process();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = command;
			process.StartInfo.WorkingDirectory = WorkingDirectory;
			process.StartInfo.RedirectStandardOutput = true;
			process.OutputDataReceived += OnOutputLineReceived;
			process.StartInfo.RedirectStandardError = true;
			process.ErrorDataReceived += OnErrorLineReceived;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = arguments;
			
			if (ProcessExited != null) {
				process.EnableRaisingEvents = true;
				process.Exited += OnProcessExited;
			}

			bool started = false;
			try {
				process.Start();
				started = true;
			} finally {
				if (!started) {
					process.Exited -= OnProcessExited;
					process = null;
				}
			}
			
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}
		
		/// <summary>
		/// Starts the process.
		/// </summary>
		/// <param name="command">The process filename.</param>
		public void Start(string command)
		{
			Start(command, String.Empty);
		}
		
		/// <summary>
		/// Kills the running process.
		/// </summary>
		public void Kill()
		{
			if (process != null) {
				if (!process.HasExited) {
					process.Kill();
					process.Close();
					process.Dispose();
					process = null;
					endOfOutput.WaitOne();
				} else {
					process = null;
				}
			}
		}
		
		/// <summary>
		/// Raises the <see cref="ProcessExited"/> event.
		/// </summary>
		protected void OnProcessExited(object sender, EventArgs e)
		{
			if (ProcessExited != null) {
				if (endOfOutput != null) {
					endOfOutput.WaitOne();
				}
				
				ProcessExited(this, e);
			}
		}
		
		/// <summary>
		/// Raises the <see cref="OutputLineReceived"/> event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The line received event arguments.</param>
		protected void OnOutputLineReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null) {
				if (Interlocked.Increment(ref outputStreamsFinished) == 2)
					endOfOutput.Set();
				return;
			}
			if (LogStandardOutputAndError) {
				lock (standardOutput) {
					standardOutput.AppendLine(e.Data);
				}
			}
			if (OutputLineReceived != null) {
				OutputLineReceived(this, new LineReceivedEventArgs(e.Data));
			}
		}
		
		/// <summary>
		/// Raises the <see cref="ErrorLineReceived"/> event.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The line received event arguments.</param>
		protected void OnErrorLineReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null) {
				if (Interlocked.Increment(ref outputStreamsFinished) == 2)
					endOfOutput.Set();
				return;
			}
			if (LogStandardOutputAndError) {
				lock (standardError) {
					standardError.AppendLine(e.Data);
				}
			}
			if (ErrorLineReceived != null) {
				ErrorLineReceived(this, new LineReceivedEventArgs(e.Data));
			}
		}
	}
}
