#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using System.Collections.Generic;

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Interface to a list of known <see cref="SIPrefix"/>es.
  /// </summary>
  public interface ISIPrefixList : IEnumerable<SIPrefix>
  {
    /// <summary>
    /// Gets the number of prefixes in this list.
    /// </summary>
    /// <value>
    /// Number of prefixes in this list.
    /// </value>
    int Count { get; }

    /// <summary>
    /// Try the get a prefix, given its shortcut. Example: given the string 'n', this function will return the prefix <see cref="SIPrefix.Nano"/>.
    /// </summary>
    /// <param name="shortCut">The short cut.</param>
    /// <returns>The prefix with the given shortcut. If no such prefix exist, the function will return null.</returns>
    SIPrefix? TryGetPrefixFromShortCut(string shortCut);

    /// <summary>
    /// Gets a value indicating whether this list only contains the <see cref="SIPrefix.None"/>.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this list only contains the <see cref="SIPrefix.None"/>.; otherwise, <c>false</c>.
    /// </value>
    bool ContainsNonePrefixOnly { get; }
  }
}
