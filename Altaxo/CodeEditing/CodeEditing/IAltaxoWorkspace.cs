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
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing
{
  public interface IAltaxoWorkspace
  {
    /// <summary>
    /// Gets the roslyn host that hosts this workspace.
    /// </summary>
    /// <value>
    /// The roslyn host.
    /// </value>
    RoslynHost RoslynHost { get; }

    /// <summary>
    /// Gets the preprocessor symbols that are used for code parsing and compilation.
    /// </summary>
    /// <value>
    /// The preprocessor symbols.
    /// </value>
    ImmutableArray<string> PreprocessorSymbols { get; }
  }
}
