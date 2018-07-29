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

using System.Reflection;

namespace Altaxo.Main.Services.ScriptCompilation
{
  /// <summary>
  /// Provides compilation of scripts to assemblies.
  /// </summary>
  public interface IScriptCompilerService
  {
    IScriptCompilerSuccessfulResult GetCompilerResult(Assembly ass);

    /// <summary>
    /// Does the compilation of the script into an assembly. The assembly is stored together with
    /// the read-only source code and returned as result. As list of compiled source codes is maintained by this class.
    /// If you provide a text that was already compiled before, the already compiled assembly is returned instead
    /// of a freshly compiled assembly.
    /// </summary>
    /// <returns>True if successfully compiles, otherwise false.</returns>
    IScriptCompilerResult Compile(string[] scriptText);
  }
}
