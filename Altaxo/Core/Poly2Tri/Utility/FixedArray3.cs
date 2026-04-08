/* Poly2Tri
 * Copyright (c) 2009-2010, Poly2Tri Contributors
 * http://code.google.com/p/poly2tri/
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Poly2Tri nor the names of its contributors may be
 *   used to endorse or promote products derived from this software without specific
 *   prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
  /// <summary>
  /// Represents a fixed-size array with three reference-type elements.
  /// </summary>
  public struct FixedArray3<T> : IEnumerable<T> where T : class
  {
    /// <summary>
    /// Gets or sets the first element.
    /// </summary>
    public T _0;
    /// <summary>
    /// Gets or sets the second element.
    /// </summary>
    public T _1;
    /// <summary>
    /// Gets or sets the third element.
    /// </summary>
    public T _2;

    /// <summary>
    /// Gets or sets the element at the specified zero-based index.
    /// </summary>
    /// <param name="index">The element index in the range 0 to 2.</param>
    /// <returns>The element stored at <paramref name="index"/>.</returns>
    public T this[int index]
    {
      get
      {
        return index switch
        {
          0 => _0,
          1 => _1,
          2 => _2,
          _ => throw new IndexOutOfRangeException(),
        };
      }
      set
      {
        switch (index)
        {
          case 0:
            _0 = value;
            break;
          case 1:
            _1 = value;
            break;
          case 2:
            _2 = value;
            break;
          default:
            throw new IndexOutOfRangeException();
        }
      }
    }

    /// <summary>
    /// Determines whether the specified value is contained in the array.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns><see langword="true"/> if the value is contained in the array; otherwise, <see langword="false"/>.</returns>
    public bool Contains(T value)
    {
      for (int i = 0; i < 3; ++i)
        if (this[i] == value)
          return true;
      return false;
    }

    /// <summary>
    /// Gets the index of the specified value.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>The zero-based index of the value, or <c>-1</c> if it is not found.</returns>
    public int IndexOf(T value)
    {
      for (int i = 0; i < 3; ++i)
        if (this[i] == value)
          return i;
      return -1;
    }

    /// <summary>
    /// Clears all elements by setting them to <see langword="null"/>.
    /// </summary>
    public void Clear()
    {
      _0 = _1 = _2 = null;
    }

    /// <summary>
    /// Clears all elements equal to the specified value.
    /// </summary>
    /// <param name="value">The value to clear.</param>
    public void Clear(T value)
    {
      for (int i = 0; i < 3; ++i)
        if (this[i] == value)
          this[i] = null;
    }

    private IEnumerable<T> Enumerate()
    {
      for (int i = 0; i < 3; ++i)
        yield return this[i];
    }

    /// <summary>
    /// Returns an enumerator that iterates over the three elements.
    /// </summary>
    /// <returns>An enumerator for the elements of the array.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      return Enumerate().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
