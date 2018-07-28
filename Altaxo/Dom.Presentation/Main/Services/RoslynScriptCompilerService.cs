using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
  public class RoslynScriptCompilerService : IScriptCompilerService
  {
    private ConcurrentScriptCompilerResultDictionary _compilerResults = new ConcurrentScriptCompilerResultDictionary();

    public IScriptCompilerResult Compile(string[] scriptText)
    {
      var scriptTextsWithHash = new CodeTextsWithHash(scriptText);

      var result = Altaxo.CodeEditing.CompilationHandling.CompilationServiceStatic.GetCompilation(scriptTextsWithHash, scriptTextsWithHash.Hash, Altaxo.Settings.Scripting.ReferencedAssemblies.All);

      if (result.CompiledAssembly != null)
      {
        return new ScriptCompilerSuccessfulResult(scriptTextsWithHash, result.CompiledAssembly);
      }
      else
      {
        return new ScriptCompilerFailedResult(scriptTextsWithHash,
          result.Diagnostics.Select(diag => new CompilerDiagnostic(diag.Line, diag.Column, (DiagnosticSeverity)diag.Severity, diag.MessageText)));
      }
    }

    public IScriptCompilerSuccessfulResult GetCompilerResult(Assembly ass)
    {
      if (_compilerResults.TryGetValue(ass, out var result))
        return result;
      else
        return null;
    }
  }
}
