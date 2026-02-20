#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Collections.ObjectModel;

namespace Altaxo.Serialization.Matlab
{
  /// <summary>
  /// Container for variables imported from a MAT-file.
  /// </summary>
  public sealed class MatFile
  {
    private readonly Dictionary<string, MatValue> _variables = [];

    /// <summary>
    /// Gets the imported variables keyed by variable name.
    /// </summary>
    public IReadOnlyDictionary<string, MatValue> Variables => new ReadOnlyDictionary<string, MatValue>(_variables);

    /// <summary>
    /// Gets a variable by name.
    /// </summary>
    /// <param name="name">Variable name.</param>
    public MatValue this[string name] => _variables[name];

    /// <summary>
    /// Adds or replaces a variable.
    /// </summary>
    /// <param name="name">Variable name.</param>
    /// <param name="value">Variable value.</param>
    public void Add(string name, MatValue value)
    {
      ArgumentException.ThrowIfNullOrEmpty(name);
      _variables[name] = value;
    }
  }
}
