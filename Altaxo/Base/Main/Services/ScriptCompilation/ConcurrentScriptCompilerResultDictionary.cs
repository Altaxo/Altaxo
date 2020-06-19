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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
  /// <summary>
  /// Stores results of the script compilation process. Both successfull and unsuccessful results are stored, in
  /// order to avoid unneccessary compilation attempts.
  /// </summary>
  public class ConcurrentScriptCompilerResultDictionary
  {
    private Dictionary<string, IScriptCompilerResult> _compilerResultsByTextHash = new Dictionary<string, IScriptCompilerResult>();
    private Dictionary<Assembly, ScriptCompilerSuccessfulResult> _compilerResultsByAssembly = new Dictionary<Assembly, ScriptCompilerSuccessfulResult>();
    private System.Threading.ReaderWriterLockSlim _lock = new System.Threading.ReaderWriterLockSlim();

    /// <summary>
    /// Tries to add a compilation result.
    /// </summary>
    /// <param name="result">The compilation result.</param>
    /// <returns>True if successful; otherwise false (if it is already present).</returns>
    public bool TryAdd(IScriptCompilerResult result)
    {
      if (null == result)
        throw new ArgumentNullException(nameof(result));

      _lock.EnterUpgradeableReadLock();
      try
      {
        if (_compilerResultsByTextHash.ContainsKey(result.ScriptTextHash))
        {
          return false;
        }
        else
        {
          _lock.EnterWriteLock();
          try
          {
            _compilerResultsByTextHash.Add(result.ScriptTextHash, result);
            if (result is ScriptCompilerSuccessfulResult successfulResult)
            {
              _compilerResultsByAssembly.Add(successfulResult.ScriptAssembly, successfulResult);
            }
            return true;
          }
          finally
          {
            _lock.ExitWriteLock();
          }
        }
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Tries to get a compilation result, by providing the hash of the script texts.
    /// </summary>
    /// <param name="scriptTextHash">The script text hash.</param>
    /// <param name="result">Returns the compilation result. This can be either a successful result or an unsuccessful result.</param>
    /// <returns>True if the compilation result corresponding to the script text hash could be found; otherwise, false.</returns>
    public bool TryGetValue(string scriptTextHash, [MaybeNullWhen(false)] out IScriptCompilerResult result)
    {
      if (string.IsNullOrEmpty(scriptTextHash))
        throw new ArgumentNullException(nameof(scriptTextHash));

      _lock.EnterReadLock();
      try
      {
        return _compilerResultsByTextHash.TryGetValue(scriptTextHash, out result);
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    /// <summary>
    /// Tries to get a compilation result from an existing assembly.
    /// </summary>
    /// <param name="assembly">The compiled assembly.</param>
    /// <param name="result">Returns the compilation result corresponding to the assembly (always a successful compilation result).</param>
    /// <returns>True if the compulation result corresponding to this assembly could be found, otherwise, false.</returns>
    public bool TryGetValue(Assembly assembly, [MaybeNullWhen(false)] out ScriptCompilerSuccessfulResult result)
    {
      if (null == assembly)
        throw new ArgumentNullException(nameof(assembly));

      _lock.EnterReadLock();
      try
      {
        return _compilerResultsByAssembly.TryGetValue(assembly, out result);
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }
  }
}
