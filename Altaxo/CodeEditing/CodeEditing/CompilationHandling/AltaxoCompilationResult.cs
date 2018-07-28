#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.CompilationHandling
{
  /// <summary>
  /// A class that bundles code with compiled assembly, without having any public functions depending on Roslyn.
  /// </summary>
  public class AltaxoCompilationResultWithAssembly
  {
    public Assembly CompiledAssembly { get; private set; }

    public ImmutableArray<string> CodeText { get; private set; }

    public ImmutableArray<AltaxoDiagnostic> Diagnostics { get; private set; }

    public AltaxoCompilationResultWithAssembly(IEnumerable<string> code, Assembly compiledAssembly, DiagnosticBag diagnosticBag)
    {
      CodeText = code.ToImmutableArray();
      CompiledAssembly = compiledAssembly;
      Diagnostics = diagnosticBag.AsEnumerable().Select(diagnosticItem => new AltaxoDiagnostic(diagnosticItem)).ToImmutableArray();
    }
  }

  /// <summary>
  /// A class that bundles code with a compiled type. The type can be used to access the compiled assembly.
  /// </summary>
  public class AltaxoCompilationResultWithType
  {
    private string[] _codeText;
    private Type _compiledType;

    public AltaxoCompilationResultWithType(IEnumerable<string> code, Type compiledType)
    {
      _codeText = code.ToArray();
      _compiledType = compiledType;
    }
  }
}
