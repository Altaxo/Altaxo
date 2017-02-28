using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
	public interface IScriptCompilerFailedResult : IScriptCompilerResult
	{
		IReadOnlyList<ICompilerDiagnostic> CompileErrors { get; }
	}
}