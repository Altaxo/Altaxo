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
using System.Text;

namespace Altaxo.Main
{
  /// <summary>
  /// Designates where an Altaxo item is being defined.
  /// </summary>
  public enum ItemDefinitionLevel
  {
    /// <summary>The item is built-in, i.e. hard coded in the source code of Altaxo.</summary>
    Builtin = 0,

    /// <summary>The item is defined on the application level, i.e. for instance in an .addin file.</summary>
    Application = 1,

    /// <summary>The item is defined on the user level. Those items are usually stored in the user's profile.</summary>
    UserDefined = 2,

    /// <summary>The item is defined on the project level.</summary>
    Project = 3
  }
}
