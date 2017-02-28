using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
	/// <summary>
	/// Default implementation of <see cref="ICompilerDiagnostic"/>
	/// </summary>
	public class CompilerDiagnostic : ICompilerDiagnostic
	{
		public int? Line { get; private set; }
		public int? Column { get; private set; }

		public DiagnosticSeverity Severity { get; private set; }

		public string SeverityText { get; private set; }

		public string MessageText { get; private set; }

		public CompilerDiagnostic(int? line, int? column, DiagnosticSeverity severity, string message)
		{
			Line = line;
			Column = column;
			Severity = severity;
			SeverityText = severity.ToString();
			MessageText = message;
		}
	}
}