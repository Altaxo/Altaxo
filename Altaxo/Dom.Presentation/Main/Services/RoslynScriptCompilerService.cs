#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
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
