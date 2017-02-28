using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
	public class ScriptCompilerFailedResult : IScriptCompilerFailedResult
	{
		public CodeTextsWithHash CodeText { get; private set; }

		private ImmutableArray<ICompilerDiagnostic> _compileErrors;

		public ScriptCompilerFailedResult(CodeTextsWithHash scriptText, IEnumerable<ICompilerDiagnostic> compileErrors)
		{
			CodeText = scriptText ?? throw new ArgumentNullException(nameof(scriptText));
			_compileErrors = compileErrors.ToImmutableArray();
		}

		#region IScriptCompilerResult Members

		public string ScriptTextHash
		{
			get
			{
				return CodeText.Hash;
			}
		}

		public Assembly ScriptAssembly
		{
			get
			{
				return null;
			}
		}

		public int ScriptTextCount
		{
			get
			{
				return CodeText.CodeTexts.Count;
			}
		}

		public string ScriptText(int i)
		{
			return CodeText.CodeTexts[i];
		}

		public IReadOnlyList<ICompilerDiagnostic> CompileErrors
		{
			get
			{
				return _compileErrors;
			}
		}

		#endregion IScriptCompilerResult Members
	}
}