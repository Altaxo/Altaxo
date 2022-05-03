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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Base class of the vector classes. Implements non-arithmetic stuff common to all vectors.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Vector<T> : ICloneable, IFormattable, IVector<T> where T : IEquatable<T>
  {
    private static readonly T[] _emptyArray = new T[0];
    protected T[] _array;

    #region Constructors

    /// <summary>
    /// Constructor for an empty vector, i.e. a vector with zero elements
    /// </summary>
    public Vector()
    {
      _array = _emptyArray;
    }

    ///<summary>Constructor with components set to the default value.</summary>
    ///<param name="length">Length of vector.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public Vector(int length)
    {
      if (length < 0)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      _array = new T[length];
    }

    ///<summary>Constructor with elements set to a value.</summary>
    ///<param name="length">Length of vector.</param>
    ///<param name="value">Value to set all elements with.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public Vector(int length, T value)
    {
      if (length < 0)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      _array = new T[length];
      for (int i = 0; i < _array.Length; ++i)
      {
        _array[i] = value;
      }
    }

    ///<summary>Constructor for <c>FloatVector</c> to deep copy another <c>FloatVector</c></summary>
    ///<param name="src"><c>FloatVector</c> to deep copy into <c>FloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public Vector(Vector<T> src)
    {
      if (src is null)
      {
        throw new ArgumentNullException(nameof(src));
      }
      _array = new T[src._array.Length];
      Array.Copy(src._array, 0, _array, 0, _array.Length);
    }

    ///<summary>Constructor from an array</summary>
    ///<param name="values">Array of values. The array is not used directly. Instead the elements of the array are copied to the vector.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public Vector(T[] values)
    {
      if (values is null)
      {
        throw new ArgumentNullException(nameof(values));
      }
      _array = new T[values.Length];
      Array.Copy(values, _array, values.Length);
    }

    ///<summary>Constructor from an array</summary>
    ///<param name="values">Array of values. The array is not used directly. Instead the elements of the array are copied to the vector.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public Vector(IReadOnlyList<T> values)
    {
      if (values is null)
      {
        throw new ArgumentNullException(nameof(values));
      }
      _array = new T[values.Count];

      if (values is Vector<T> vector)
      {
        Array.Copy(vector._array, _array, _array.Length);
      }
      else
      {
        for (int i = 0; i < _array.Length; ++i)
          _array[i] = values[i];
      }
    }

    ///<summary>Constructor from an <see cref="IList"/></summary>
    ///<param name="values"><c>IList</c> use as source for the elements of the vector.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
    public Vector(IList values)
    {
      if (values is null)
      {
        throw new ArgumentNullException(nameof(values));
      }

      _array = new T[values.Count];
      for (int i = 0; i < _array.Length; ++i)
      {
        _array[i] = (T)(values[i] ?? throw new ArgumentNullException($"{nameof(values)}[{i}]"));
      }
    }

    /// <summary>
    /// Creates a vector, that is a wrapper of the provided array. The array is used directly.
    /// </summary>
    /// <param name="values">Array of values. This array is used directly in the returned vector!</param>
    /// <returns>Vector that is a wrapper for the provided array.</returns>
    public static Vector<T> AsWrapperFor(T[] values)
    {
      return new Vector<T>() { _array = values ?? throw new ArgumentNullException(nameof(values)) };
    }

    #endregion Constructors

    public T this[int i]
    {
      get
      {
        return _array[i];
      }
      set
      {
        _array[i] = value;
      }
    }

    T IReadOnlyList<T>.this[int index]
    {
      get
      {
        return _array[index];
      }
    }

    public T[] this[int[] indices]
    {
      get
      {
        return indices.Select(idx => _array[idx]).ToArray();
      }
    }

    public T[] this[bool[] condition]
    {
      get
      {
        return _array.Where((x, i) => condition[i]).ToArray();
      }
    }

    /// <summary>
    /// Sets all elements of the vector back to the default value.
    /// </summary>
    public void Clear()
    {
#pragma warning disable CS8601 // Possible null reference assignment.
      for (int i = 0; i < _array.Length; ++i)
        _array[i] = default;
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public int Length { get { return _array.Length; } }

    public int Count { get { return _array.Length; } }

    /// <summary>
    /// Resizes the vector. The previous element data are lost.
    /// </summary>
    /// <param name="length">New length of the vector.</param>
    public void Resize(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("Length must be nonnegative");

      if (_array is null || length != _array.Length)
      {
        _array = new T[length];
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      var len = Length;
      for (int i = 0; i < len; ++i)
        yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      var len = Length;
      for (int i = 0; i < len; ++i)
        yield return this[i];
    }

    ///<summary>Check if <c>FloatVector</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>FloatVector</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>FloatVector</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>FloatVector</c> variable before comparing with the current <c>DoubleVector</c>.</remarks>
    public override bool Equals(object? obj)
    {
      if (!(obj is Vector<T> vector))
      {
        return false;
      }

      if (_array.Length != vector._array.Length)
      {
        return false;
      }

      var comparer = EqualityComparer<T>.Default;

      for (int i = 0; i < _array.Length; ++i)
      {
        if (!comparer.Equals(_array[i], vector._array[i]))
        {
          return false;
        }
      }
      return true;
    }

    ///<summary>Return the Hashcode for the <c>FloatVector</c></summary>
    ///<returns>The Hashcode representation of <c>FloatVector</c></returns>
    public override int GetHashCode()
    {
      int result = _array.Length.GetHashCode();

      if (_array.Length > 0)
      {
        result += 17 * _array[0].GetHashCode();
        result += 31 * _array[_array.Length - 1].GetHashCode();
      }
      return result;
    }

    ///<summary>Retrieves a reference to the public array. Use with care, and only as local variables.</summary>
    ///<returns>Reference to the underlying array of this vector.</returns>
    public T[] GetInternalData()
    {
      return _array;
    }

    ///<summary>Returns an array of data copyied (not used directly) from this instance.</summary>
    ///<returns>Array with copied data of this instance.</returns>
    public T[] ToArray()
    {
      var result = new T[_array.Length];
      Array.Copy(_array, result, _array.Length);
      return result;
    }

    ///<summary>Returns a subvector of this vector.</summary>
    ///<param name="startElement">Return data starting from this element.</param>
    ///<param name="endElement">Return data ending in this element.</param>
    ///<returns>A subvector of this vector.</returns>
    ///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>FloatVector</c> dimensions</exception>
    public Vector<T> GetSubVector(int startElement, int endElement)
    {
      if (startElement > endElement)
      {
        throw new ArgumentException("The starting element must be less that the ending element.");
      }

      if (startElement < 0 || endElement < 0 || startElement >= Length || endElement >= Length)
      {
        throw new ArgumentException("startElement and startElement must be greater than or equal to zero, endElement must be less than Length, and endElement must be less than Length.");
      }

      int n = endElement - startElement + 1;
      var result = new Vector<T>(n);
      for (int i = 0; i < n; i++)
      {
        result[i] = this[i + startElement];
      }
      return result;
    }

    ///<summary>Copies the data from another vector into this instance.</summary>
    ///<param name="src">Vector to copy from..</param>
    public void CopyFrom(Vector<T> src)
    {
      if (src is null)
        throw new System.ArgumentNullException(nameof(src));
      if (src.Length != Length)
        throw new ArgumentException("Source length must be equal to length of this vector", nameof(src));

      if (!(_array.Length == src.Count))
        _array = (T[])src._array.Clone();
      else
        Array.Copy(src._array, _array, _array.Length);
    }

    ///<summary>Copies the data from another vector into this instance.</summary>
    ///<param name="src">Vector to copy from..</param>
    public void CopyFrom(IReadOnlyList<T> src)
    {
      if (src is null)
      {
        throw new System.ArgumentNullException(nameof(src));
      }

      if (src is Vector<T> vector)
      {
        if (!(_array.Length == src.Count))
          _array = (T[])vector._array.Clone();
        else
          Array.Copy(vector._array, _array, _array.Length);
      }
      else
      {
        if (!(_array.Length == src.Count))
          _array = new T[src.Count];
        for (int i = 0; i < _array.Length; ++i)
          _array[i] = src[i];
      }
    }

    ///<summary>Swap data in this vector with another vector.</summary>
    ///<param name="src">Vector to swap data with.</param>
    public void Swap(Vector<T> src)
    {
      if (src is null)
        throw new System.ArgumentNullException(nameof(src));
      if (src.Length != Length)
        throw new ArgumentException("Length of source vector must be equal to length of this vector", nameof(src));

      var h = _array;
      _array = src._array;
      src._array = h;
    }

    ///<summary>Clone (deep copy) a <c>FloatVector</c> variable</summary>
    public Vector<T> Clone()
    {
      return new Vector<T>(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>FloatVector</c> variable</summary>
    object ICloneable.Clone()
    {
      return Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format and provider.</returns>
    public virtual string ToString(string? format, IFormatProvider? formatProvider)
    {
      var sb = new StringBuilder("Length: ");
      sb.Append(_array.Length).Append(System.Environment.NewLine);
      for (int i = 0; i < _array.Length; ++i)
      {
        sb.Append(_array[i].ToString());
        if (i != _array.Length - 1)
        {
          sb.Append(", ");
        }
      }
      return sb.ToString();
    }

    ///<summary>Check if any of the vector's elements equals a given value.</summary>
    ///<param name="value">The value to test for.</param>
    ///<returns>True if the value to test for was found; false otherwise.</returns>
    public bool Contains(T value)
    {
      var comparer = EqualityComparer<T>.Default;
      for (int i = 0; i < _array.Length; i++)
      {
        if (comparer.Equals(_array[i], value))
          return true;
      }
      return false;
    }

    ///<summary>Return the index of the vector's first element that equals a given value.</summary>
    ///<param name="value">The value to search for.</param>
    ///<returns>The index of the vector's first element that equals the given value; otherwise, the return value is -1.</returns>
    public int IndexOf(T value)
    {
      var comparer = EqualityComparer<T>.Default;
      for (int i = 0; i < _array.Length; i++)
      {
        if (comparer.Equals(_array[i], value))
          return i;
      }
      return -1;
    }
  }
}
