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

#nullable enable
using System;

namespace Altaxo.Data
{
  /// <summary>
  /// Represents a column whose elements can be set by assigning an <see cref="AltaxoVariant"/> at a given index.
  /// </summary>
  public interface IWriteableColumn : ICloneable
  {
    /// <summary>
    /// Indexer property for setting the element at index i by a AltaxoVariant.
    /// This function should throw an exeption, if the type of the variant do not match
    /// the type of the column.
    /// </summary>
    /// <param name="i">The index of the element to set.</param>
    /// <value>The value assigned to the specified index.</value>
    AltaxoVariant this[int i]
    {
      set;
    }
  }
}
