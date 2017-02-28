using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
	public class ScriptCompilerSuccessfulResult : IScriptCompilerSuccessfulResult
	{
		public Assembly ScriptAssembly { get; private set; }
		public CodeTextsWithHash CodeText { get; private set; }

		public ScriptCompilerSuccessfulResult(CodeTextsWithHash codeText, Assembly assembly)
		{
			CodeText = codeText ?? throw new ArgumentNullException(nameof(codeText));
			ScriptAssembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
		}

		#region IScriptCompilerResult Members

		public string ScriptTextHash
		{
			get
			{
				return CodeText.Hash;
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

		#endregion IScriptCompilerResult Members
	}
}