using System;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Services 
{
	public interface IDebugger 
	{
		/// <summary>
		/// Returns true if debuger is attached to a process
		/// </summary>
		bool IsDebugging {
			get;
		}
		
		/// <summary>
		/// Returns true if process is running
		/// Returns false if breakpoint is hit, program is breaked, program is stepped, etc...
		/// </summary>
		bool IsProcessRunning {
			get;
		}
		
		bool SupportsStartStop {
			get;
		}
		
		/// <summary>
		/// Break/Continue
		/// </summary>
		bool SupportsExecutionControl {
			get;
		}
		
		/// <summary>
		/// Step/Step into/Step over
		/// </summary>
		bool SupportsStepping {
			get;
		}
		
		bool CanDebug(IProject project);
		
		/// <summary>
		/// Starts process and attaches debugger
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="workingDirectory"></param>
		/// <param name="arguments"></param>
		void Start(string fileName, string workingDirectory, string arguments);

		/// <summary>
		/// Stops/terminates attached process
		/// </summary>
		void Stop();
		
		// ExecutionControl:
		
		void Break();
		
		void Continue();

		void StepInto();

		void StepOver();

		void StepOut();
		
		event EventHandler DebugStopped;
	}
}
