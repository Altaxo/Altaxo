#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// VectorMath provides common static functions concerning vectors.
  /// </summary>
  public static partial class VectorMath
  {
    #region Extensible Vector

    private class ExtensibleVector<T> : IExtensibleVector<T>
    {
      private T[] _arr;
      private int _length;

      public ExtensibleVector(int initiallength)
      {
        _arr = new T[initiallength];
        _length = initiallength;
      }

      #region IVector Members

      public T this[int i]
      {
        get
        {
          return _arr[i];
        }
        set
        {
          _arr[i] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      public int Length
      {
        get
        {
          return _length;
        }
      }

      public int Count
      {
        get
        {
          return _length;
        }
      }

      #endregion IROVector Members

      #region IExtensibleVector Members

      public void Append(IROVector<T> vector)
      {
        if (_length + vector.Length >= _arr.Length)
          Redim((int)(32 + 1.3 * (_length + vector.Length)));

        for (int i = 0; i < vector.Length; i++)
          _arr[i + _length] = vector[i];
        _length += vector.Length;
      }

      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      #endregion IExtensibleVector Members

      private void Redim(int newsize)
      {
        if (newsize > _arr.Length)
        {
          var oldarr = _arr;
          _arr = new T[newsize];
          Array.Copy(oldarr, 0, _arr, 0, _length);
        }
      }
    }

    /// <summary>
    /// Creates a new extensible vector of length <c>length</c>
    /// </summary>
    /// <param name="length">The inital length of the vector.</param>
    /// <returns>An instance of a extensible vector.</returns>
    public static IExtensibleVector<T> CreateExtensibleVector<T>(int length)
    {
      return new ExtensibleVector<T>(length);
    }

    #endregion Extensible Vector
  }
}
