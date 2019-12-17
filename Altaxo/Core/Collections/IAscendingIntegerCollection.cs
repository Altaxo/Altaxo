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

using System;
using System.Collections.Generic;

namespace Altaxo.Collections
{
    /// <summary>
    /// Sorted collection of integers, sorted so that the smallest integers come first.
    /// </summary>
    public interface IAscendingIntegerCollection : ICloneable, IReadOnlyList<int>
    {
        /// <summary>
        /// Returns true if the integer <code>nValue</code> is contained in this collection.
        /// </summary>
        /// <param name="nValue">The integer value to test for membership.</param>
        /// <returns>True if the integer value is member of the collection.</returns>
        bool Contains(int nValue);

        IEnumerable<ContiguousIntegerRange> RangesAscending { get; }

        IEnumerable<ContiguousIntegerRange> RangesDescending { get; }
    }
}
