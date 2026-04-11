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
using System.Linq;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// VectorMath provides common static functions concerning vectors.
  /// </summary>
  public static partial class VectorMath
  {
    /// <summary>
    /// Fills the vector with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the vector elements.</typeparam>
    /// <param name="x">The vector to be filled.</param>
    /// <param name="value">The value to fill the vector with.</param>
    public static void FillWith<T>(this Vector<T> x, T value) where T : struct, System.IEquatable<T>, System.IFormattable
    {
      for (int i = 0; i < x.Count; ++i)
        x[i] = value;
    }
    /// <summary>
    /// Sets the values of the vector from the provided array.
    /// </summary>
    /// <typeparam name="T">The type of the vector elements.</typeparam>
    /// <param name="x">The vector whose values are to be set.</param>
    /// <param name="values">The array of values to set.</param>
    public static void SetValues<T>(this Vector<T> x, IReadOnlyList<T> values) where T : struct, System.IEquatable<T>, System.IFormattable
    {
      if (x.Count != values.Count)
        throw new System.ArgumentException();

      for (int i = 0; i < x.Count; ++i)
        x[i] = values[i];
    }



    #region Extensible Vector

    private class ExtensibleVector<T> : IExtensibleVector<T>
    {
      private T[] _arr;
      private int _length;

      /// <summary>
      /// Initializes a new instance of the <see cref="ExtensibleVector{T}"/> class.
      /// </summary>
      /// <param name="initiallength">The initial length of the vector.</param>
      public ExtensibleVector(int initiallength)
      {
        _arr = new T[initiallength];
        _length = initiallength;
      }

      #region IVector Members

      /// <summary>
      /// Gets or sets the element at the specified index.
      /// </summary>
      /// <param name="i">The zero-based index of the element to get or set.</param>
      /// <returns>The element at the specified index.</returns>
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

      /// <summary>
      /// Gets the number of elements in the vector.
      /// </summary>
      public int Count
      {
        get
        {
          return _length;
        }
      }

      #endregion IROVector Members

      #region IExtensibleVector Members

      /// <summary>
      /// Appends a vector to the end of this vector.
      /// </summary>
      /// <param name="vector">The vector to append.</param>
      public void Append(IReadOnlyList<T> vector)
      {
        if (_length + vector.Count >= _arr.Length)
          Redim((int)(32 + 1.3 * (_length + vector.Count)));

        for (int i = 0; i < vector.Count; i++)
          _arr[i + _length] = vector[i];
        _length += vector.Count;
      }

      /// <summary>
      /// Gets an enumerator that iterates through the vector.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the vector.</returns>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      /// <summary>
      /// Returns a non-generic enumerator that iterates through the subvector.
      /// </summary>
      /// <returns>A non-generic enumerator for the subvector.</returns>
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
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The inital length of the vector.</param>
    /// <returns>An instance of a extensible vector.</returns>
    public static IExtensibleVector<T> CreateExtensibleVector<T>(int length)
    {
      return new ExtensibleVector<T>(length);
    }

    #endregion Extensible Vector

    /// <summary>
    /// Extracts the elements at the specified indices from the array.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="indices">The indices of the elements to extract.</param>
    /// <returns>An array containing the extracted elements.</returns>
    public static T[] ElementsAt<T>(this T[] array, int[] indices)
    {
      return indices.Select(idx => array[idx]).ToArray();
    }
    /// <summary>
    /// Extracts the elements at the specified indices from the array.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="indices">The indices of the elements to extract.</param>
    /// <returns>An array containing the extracted elements.</returns>
    public static T[] ElementsAt<T>(this IReadOnlyList<T> array, int[] indices)
    {
      return indices.Select(idx => array[idx]).ToArray();
    }

    /// <summary>
    /// Selects elements from the array where the corresponding condition element is <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="condition">The array of boolean conditions.</param>
    /// <returns>An array containing the selected elements.</returns>
    public static T[] ElementsWhere<T>(this T[] array, bool[] condition)
    {
      return array.Where((x, i) => condition[i]).ToArray();
    }
    /// <summary>
    /// Selects elements from the array where the corresponding condition element is <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="condition">The array of boolean conditions.</param>
    /// <returns>An array containing the selected elements.</returns>
    public static T[] ElementsWhere<T>(this IReadOnlyList<T> array, bool[] condition)
    {
      return array.Where((x, i) => condition[i]).ToArray();
    }


    /// <summary>
    /// Selects elements from the array where the corresponding condition element is <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="condition">The enumerable sequence of boolean conditions.</param>
    /// <returns>An array containing the selected elements.</returns>
    public static T[] ElementsWhere<T>(this T[] array, IEnumerable<bool> condition)
    {
      static IEnumerable<T> RFunc(T[] array, IEnumerable<bool> condition)
      {
        int idx = 0;
        foreach (var b in condition)
        {
          if (idx >= array.Length)
          {
            break;
          }
          if (b)
          {
            yield return array[idx];
          }
          ++idx;
        }
      }
      return RFunc(array, condition).ToArray();
    }
    /// <summary>
    /// Selects elements from the array where the corresponding condition element is <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="condition">The enumerable sequence of boolean conditions.</param>
    /// <returns>An array containing the selected elements.</returns>
    public static T[] ElementsWhere<T>(this IReadOnlyList<T> array, IEnumerable<bool> condition)
    {
      static IEnumerable<T> RFunc(IReadOnlyList<T> array, IEnumerable<bool> condition)
      {
        int idx = 0;
        foreach (var b in condition)
        {
          if (idx >= array.Count)
          {
            break;
          }
          if (b)
          {
            yield return array[idx];
          }
          ++idx;
        }
      }
      return RFunc(array, condition).ToArray();
    }

    /// <summary>
    /// Copies a range of elements from the source to the destination array.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="source">The source array.</param>
    /// <param name="sourceIndex">The zero-based index in the source array at which the copying begins.</param>
    /// <param name="destination">The destination array.</param>
    /// <param name="destinationStartIndex">The zero-based index in the destination array at which storing elements begins.</param>
    /// <param name="count">The number of elements to copy.</param>
    public static void Copy<T>(IReadOnlyList<T> source, int sourceIndex, T[] destination, int destinationStartIndex, int count)
    {
      for (int i = 0, j = sourceIndex, k = destinationStartIndex; i < count; ++i, ++j, ++k)
      {
        destination[k] = source[j];
      }
    }

    #region IROVector and IVector wrapper types

    /// <summary>
    /// Creates a subvector of the read-only vector.
    /// </summary>
    /// <typeparam name="T">The type of element of the vector.</typeparam>
    /// <param name="source">The source vector.</param>
    /// <param name="offset">The offset of the first element of the subvector.</param>
    /// <param name="count">The number of elements of the subvector.</param>
    /// <returns>The subvector.</returns>
    public static IReadOnlyList<T> ToROSubVector<T>(this IReadOnlyList<T> source, int offset, int count)
    {
      return new ROVectorWrapperOfIROVector<T>(source, offset, count);
    }

    /// <summary>
    /// Provides a read-only wrapper around an <see cref="IReadOnlyList{T}"/> to represent a subvector.
    /// </summary>
    /// <typeparam name="T">The type of the vector elements.</typeparam>
    protected class ROVectorWrapperOfIROVector<T> : IReadOnlyList<T>
    {
      private IReadOnlyList<T> _source;
      private int _offset;
      private int _count;

      /// <summary>
      /// Initializes a new instance of the <see cref="ROVectorWrapperOfIROVector{T}"/> class.
      /// </summary>
      /// <param name="source">The source vector.</param>
      /// <param name="offset">The offset of the first element of the subvector.</param>
      /// <param name="count">The number of elements of the subvector.</param>
      public ROVectorWrapperOfIROVector(IReadOnlyList<T> source, int offset, int count)
      {
        if (_source is null)
          throw new ArgumentNullException(nameof(source));
        if (count < 0)
          throw new ArgumentOutOfRangeException(nameof(count));
        if (offset < 0 || (offset + count) > _source.Count)
          throw new IndexOutOfRangeException(nameof(offset));

        _source = source;
        _offset = offset;
        _count = count;
      }

      /// <summary>
      /// Gets the element at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the element to get.</param>
      /// <returns>The element at the specified index.</returns>
      public T this[int index]
      {
        get
        {
          if (index < 0 || index >= _count)
            throw new IndexOutOfRangeException();

          return _source[index + _offset];
        }
      }

      /// <summary>
      /// Gets the number of elements in the subvector.
      /// </summary>
      public int Count => _count;

      /// <summary>
      /// Gets an enumerator that iterates through the subvector.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the subvector.</returns>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = _offset, j = 0; j < _count; ++i, ++j)
          yield return _source[i];
      }

      /// <summary>
      /// Returns a non-generic enumerator that iterates through the subvector.
      /// </summary>
      /// <returns>A non-generic enumerator for the subvector.</returns>
      /// <summary>
      /// Gets a non-generic enumerator that iterates through the subvector.
      /// </summary>
      /// <returns>A non-generic enumerator for the subvector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = _offset, j = 0; j < _count; ++i, ++j)
          yield return _source[i];
      }
    }

    /// <summary>
    /// Creates a subvector of the read/write vector. Note that is is only a wrapper around the wrapper. If the subvector is changed, also the underlying (wrapped) vector is changed!
    /// </summary>
    /// <typeparam name="T">The type of element of the vector.</typeparam>
    /// <param name="source">The source vector.</param>
    /// <param name="offset">The offset of the first element of the subvector.</param>
    /// <param name="count">The number of elements of the subvector.</param>
    /// <returns>The subvector.</returns>
    public static IVector<T> ToSubVector<T>(this IVector<T> source, int offset, int count)
    {
      return new VectorWrapperOfIVector<T>(source, offset, count);
    }

    /// <summary>
    /// Provides a read-write wrapper around an <see cref="IVector{T}"/> to represent a subvector.
    /// </summary>
    /// <typeparam name="T">The type of the vector elements.</typeparam>
    protected class VectorWrapperOfIVector<T> : IVector<T>
    {
      private IVector<T> _source;
      private int _offset;
      private int _count;

      /// <summary>
      /// Initializes a new instance of the <see cref="VectorWrapperOfIVector{T}"/> class.
      /// </summary>
      /// <param name="source">The source vector.</param>
      /// <param name="offset">The offset of the first element of the subvector.</param>
      /// <param name="count">The number of elements of the subvector.</param>
      public VectorWrapperOfIVector(IVector<T> source, int offset, int count)
      {
        if (source is null)
          throw new ArgumentNullException(nameof(source));
        if (count < 0)
          throw new ArgumentOutOfRangeException(nameof(count));
        if (offset < 0 || (offset + count) > source.Count)
          throw new IndexOutOfRangeException(nameof(offset));

        _source = source;
        _offset = offset;
        _count = count;
      }

      /// <summary>
      /// Gets the element at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the element to get.</param>
      /// <returns>The element at the specified index.</returns>
      public T this[int index]
      {
        get
        {
          if (index < 0 || index >= _count)
            throw new IndexOutOfRangeException();

          return _source[index + _offset];
        }
        set
        {
          if (index < 0 || index >= _count)
            throw new IndexOutOfRangeException();

          _source[index + _offset] = value;
        }
      }

      /// <summary>
      /// Gets the number of elements in the subvector.
      /// </summary>
      public int Count => _count;

      /// <summary>
      /// Gets an enumerator that iterates through the subvector.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the subvector.</returns>
      public IEnumerator<T> GetEnumerator()
      {
        for (int i = _offset, j = 0; j < _count; ++i, ++j)
          yield return _source[i];
      }

      /// <summary>
      /// Gets an enumerator that iterates through the subvector.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the subvector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = _offset, j = 0; j < _count; ++i, ++j)
          yield return _source[i];
      }
    }


    #endregion
  }
}
