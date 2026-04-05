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

namespace Altaxo.Drawing
{
  /// <summary>
  /// Immutable lists of styles, for instance scatter styles or line styles, that are used in grouping.
  /// </summary>
  /// <typeparam name="T">Type of the style.</typeparam>
  /// <seealso cref="System.Collections.Generic.IList{T}" />
  public interface IStyleList<T> : IReadOnlyList<T>, Main.IImmutable where T : Main.IImmutable
  {
    /// <summary>
    /// Gets the name of the style list.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns a copy of the style list with a different name.
    /// </summary>
    /// <param name="name">The new list name.</param>
    /// <returns>A style list with the updated name.</returns>
    IStyleList<T> WithName(string name);

    /// <summary>
    /// Determines whether the specified list has the same structure and items.
    /// </summary>
    /// <param name="anotherList">The list to compare with.</param>
    /// <returns><see langword="true"/> if the lists are structurally equivalent; otherwise, <see langword="false"/>.</returns>
    bool IsStructuralEquivalentTo(IEnumerable<T> anotherList);
  }
}
