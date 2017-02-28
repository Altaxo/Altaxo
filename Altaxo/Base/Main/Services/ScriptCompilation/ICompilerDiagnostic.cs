using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
	/// <summary>
	/// Severity of a diagnostics message.
	/// </summary>
	public enum DiagnosticSeverity
	{
		/// <summary>Hidden message.</summary>
		Hidden = 0,

		/// <summary>Info message.</summary>
		Info = 1,

		/// <summary>Warning message.</summary>
		Warning = 2,

		/// <summary>Error message.</summary>
		Error = 3
	}

	/// <summary>
	/// Interface to one item of a compiler's diagnostic message.
	/// </summary>
	public interface ICompilerDiagnostic
	{
		/// <summary>
		/// Gets the text line number, or null if this not applies.
		/// </summary>
		int? Line { get; }

		/// <summary>
		/// Gets the text column number, or null if this not applies.
		/// </summary>
		int? Column { get; }

		/// <summary>
		/// Gets the severity level (0: Hidden, 1: Info, 2: Warning, 3: Error).
		/// </summary>
		DiagnosticSeverity Severity { get; }

		/// <summary>
		/// Gets the severity text, e.g. Hidden, Info, Warning, Error.
		/// </summary>
		string SeverityText { get; }

		/// <summary>
		/// Gets the message text.
		/// </summary>
		string MessageText { get; }
	}
}