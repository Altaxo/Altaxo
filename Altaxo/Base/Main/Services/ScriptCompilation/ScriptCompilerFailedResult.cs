﻿#region Copyright

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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

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

    public Assembly? ScriptAssembly
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
