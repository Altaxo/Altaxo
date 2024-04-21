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
namespace Altaxo.Main.Services.ScriptCompilation
{
  /// <summary>
  /// Base interface for the result of the script controller. This base interface only provided the source code.
  /// The derived interfaces then also provide compilation results.
  /// </summary>
  public interface IScriptCompilerResult
  {
    /// <summary>
    /// Gets the number of source code fragments (script text) of which consists the compilation result.
    /// </summary>
    int ScriptTextCount { get; }

    /// <summary>
    /// Get the script text with index i (i ranges from 0 to <see cref="ScriptTextCount"/>-1).
    /// </summary>
    /// <param name="i">The index of the script text.</param>
    /// <returns>The script text with index i.</returns>
    string ScriptText(int i);

    /// <summary>
    /// Gets the script text hash that is build over all the script texts in exactly the order provided here.
    /// </summary>
    /// <value>
    /// The script text hash.
    /// </value>
    string ScriptTextHash { get; }
  }
}
