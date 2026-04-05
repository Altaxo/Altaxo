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

using System;
using System.Reflection;

namespace Altaxo.Main.Services.ScriptCompilation
{
  /// <summary>
  /// Represents a successful script-compilation result.
  /// </summary>
  public class ScriptCompilerSuccessfulResult : IScriptCompilerSuccessfulResult
  {
    /// <inheritdoc/>
    public Assembly ScriptAssembly { get; private set; }

    /// <summary>
    /// Gets the script texts together with their hash.
    /// </summary>
    public CodeTextsWithHash CodeText { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptCompilerSuccessfulResult"/> class.
    /// </summary>
    /// <param name="codeText">The script texts together with their hash.</param>
    /// <param name="assembly">The compiled assembly.</param>
    public ScriptCompilerSuccessfulResult(CodeTextsWithHash codeText, Assembly assembly)
    {
      CodeText = codeText ?? throw new ArgumentNullException(nameof(codeText));
      ScriptAssembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    #region IScriptCompilerResult Members

    /// <inheritdoc/>
    public string ScriptTextHash
    {
      get
      {
        return CodeText.Hash;
      }
    }

    /// <inheritdoc/>
    public int ScriptTextCount
    {
      get
      {
        return CodeText.CodeTexts.Count;
      }
    }

    /// <inheritdoc/>
    public string ScriptText(int i)
    {
      return CodeText.CodeTexts[i];
    }

    #endregion IScriptCompilerResult Members
  }
}
