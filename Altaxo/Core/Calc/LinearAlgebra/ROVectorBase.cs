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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Base class for read-only vector implementations.
  /// </summary>
  /// <typeparam name="T">The type of the vector elements.</typeparam>
  public abstract class ROVectorBase<T> : IReadOnlyList<T>
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    public abstract T this[int index] { get; set; }

    /// <summary>
    /// Gets the number of elements in the vector.
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a value in the vector, or -1 if the value is not found.
    /// </summary>
    /// <param name="item">The value to locate in the vector.</param>
    /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> in the vector, or -1 if not found.</returns>
    public int IndexOf(T item)
    {
      if (!(item is null))
      {
        var cnt = Count;
        for (int i = 0; i < Count; ++i)
          if (item.Equals(this[i]))
            return i;
      }
      return -1;
    }

    /// <summary>
    /// Determines whether the vector contains a specific value.
    /// </summary>
    /// <param name="item">The value to locate in the vector.</param>
    /// <returns><c>true</c> if the value is found in the vector; otherwise, <c>false</c>.</returns>
    public bool Contains(T item)
    {
      if (!(item is null))
      {
        var cnt = Count;
        for (int i = 0; i < Count; ++i)
          if (item.Equals(this[i]))
            return true;
      }
      return false;
    }

    /// <summary>
    /// Copies the elements of the vector to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The array to copy the elements to.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which storing elements will begin.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex is < 0");
      var cnt = Count;
      if (!(arrayIndex + cnt <= array.Length))
        throw new ArgumentOutOfRangeException("Array too small for the provided data.");

      for (int i = 0; i < cnt; ++i)
        array[i + arrayIndex] = this[i];
    }

    /// <summary>
    /// Returns an enumerator that iterates through the vector.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the vector.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      var cnt = Count;
      for (int i = 0; i < cnt; ++i)
        yield return this[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      var cnt = Count;
      for (int i = 0; i < cnt; ++i)
        yield return this[i];
    }
  }
}
