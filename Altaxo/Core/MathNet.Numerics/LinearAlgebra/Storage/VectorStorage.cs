// <copyright file="VectorStorage.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Altaxo.Calc.LinearAlgebra.Storage
{
  /// <summary>
  /// Represents the storage backing for a vector.
  /// </summary>
  /// <typeparam name="T">The element type.</typeparam>
  [Serializable]
  [DataContract(Namespace = "urn:MathNet/Numerics/LinearAlgebra")]
  public abstract partial class VectorStorage<T> : IEquatable<VectorStorage<T>>
        where T : struct, IEquatable<T>, IFormattable
  {
    // [ruegg] public fields are OK here

    /// <summary>
    /// The zero value for the element type.
    /// </summary>
    protected static readonly T Zero = BuilderInstance<T>.Vector.Zero;

    /// <summary>
    /// The length of the vector storage.
    /// </summary>
    [DataMember(Order = 1)]
    public readonly int Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorStorage{T}"/> class.
    /// </summary>
    /// <param name="length">The storage length.</param>
    protected VectorStorage(int length)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length), "Value must not be negative (zero is ok).");
      }

      Length = length;
    }

    /// <summary>
    /// True if the vector storage format is dense.
    /// </summary>
    public abstract bool IsDense { get; }

    /// <summary>
    /// Gets or sets the value at the given index, with range checking.
    /// </summary>
    /// <param name="index">
    /// The index of the element.
    /// </param>
    /// <value>The value to get or set.</value>
    /// <remarks>This method is ranged checked. <see cref="At(int)"/> and <see cref="At(int,T)"/>
    /// to get and set values without range checking.</remarks>
    public T this[int index]
    {
      get
      {
        ValidateRange(index);
        return At(index);
      }

      set
      {
        ValidateRange(index);
        At(index, value);
      }
    }

    /// <summary>
    /// Retrieves the requested element without range checking.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <returns>The requested element.</returns>
    /// <remarks>Not range-checked.</remarks>
    public abstract T At(int index);

    /// <summary>
    /// Sets the element without range checking.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <param name="value">The value to set the element to. </param>
    /// <remarks>WARNING: This method is not thread safe. Use "lock" with it and be sure to avoid deadlocks.</remarks>
    public abstract void At(int index, T value);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    /// An object to compare with this object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
    /// </returns>
    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    public virtual bool Equals(VectorStorage<T> other)
    {
      // Reject equality when the argument is null or has a different shape.
      if (other == null)
      {
        return false;
      }
      if (Length != other.Length)
      {
        return false;
      }

      // Accept if the argument is the same object as this.
      if (ReferenceEquals(this, other))
      {
        return true;
      }

      // If all else fails, perform element wise comparison.
      for (var index = 0; index < Length; index++)
      {
        if (!At(index).Equals(other.At(index)))
        {
          return false;
        }
      }

      return true;
    }

    /// <inheritdoc />
    public sealed override bool Equals(object obj)
    {
      return Equals(obj as VectorStorage<T>);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      var hashNum = Math.Min(Length, 25);
      int hash = 17;
      unchecked
      {
        for (var i = 0; i < hashNum; i++)
        {
          hash = hash * 31 + At(i).GetHashCode();
        }
      }
      return hash;
    }

    // CLEARING

    /// <summary>
    /// Clears all elements in the storage.
    /// </summary>
    public virtual void Clear()
    {
      for (var i = 0; i < Length; i++)
      {
        At(i, Zero);
      }
    }

    /// <summary>
    /// Clears a contiguous range of elements in the storage.
    /// </summary>
    /// <param name="index">The start index.</param>
    /// <param name="count">The number of elements to clear.</param>
    public virtual void Clear(int index, int count)
    {
      for (var i = index; i < index + count; i++)
      {
        At(i, Zero);
      }
    }

    // VECTOR COPY

    /// <summary>
    /// Copies this storage into another vector storage.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void CopyTo(VectorStorage<T> target, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (ReferenceEquals(this, target))
      {
        return;
      }

      if (Length != target.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      CopyToUnchecked(target, existingData);
    }

    /// <summary>
    /// Copies this storage into another vector storage without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void CopyToUnchecked(VectorStorage<T> target, ExistingData existingData)
    {
      for (int i = 0; i < Length; i++)
      {
        target.At(i, At(i));
      }
    }

    // ROW COPY

    /// <summary>
    /// Copies this storage into a row of a matrix storage.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="rowIndex">The target row index.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void CopyToRow(MatrixStorage<T> target, int rowIndex, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (Length != target.ColumnCount)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      ValidateRowRange(target, rowIndex);
      CopyToRowUnchecked(target, rowIndex, existingData);
    }

    /// <summary>
    /// Copies this storage into a row of a matrix storage without validation.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="rowIndex">The target row index.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void CopyToRowUnchecked(MatrixStorage<T> target, int rowIndex, ExistingData existingData)
    {
      for (int j = 0; j < Length; j++)
      {
        target.At(rowIndex, j, At(j));
      }
    }

    // COLUMN COPY

    /// <summary>
    /// Copies this storage into a column of a matrix storage.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="columnIndex">The target column index.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void CopyToColumn(MatrixStorage<T> target, int columnIndex, ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (Length != target.RowCount)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      ValidateColumnRange(target, columnIndex);
      CopyToColumnUnchecked(target, columnIndex, existingData);
    }

    /// <summary>
    /// Copies this storage into a column of a matrix storage without validation.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="columnIndex">The target column index.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void CopyToColumnUnchecked(MatrixStorage<T> target, int columnIndex, ExistingData existingData)
    {
      for (int i = 0; i < Length; i++)
      {
        target.At(i, columnIndex, At(i));
      }
    }

    // SUB-VECTOR COPY

    /// <summary>
    /// Copies a subvector into another vector storage.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="sourceIndex">The source start index.</param>
    /// <param name="targetIndex">The target start index.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void CopySubVectorTo(VectorStorage<T> target,
        int sourceIndex, int targetIndex, int count,
        ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (count == 0)
      {
        return;
      }

      ValidateSubVectorRange(target, sourceIndex, targetIndex, count);
      CopySubVectorToUnchecked(target, sourceIndex, targetIndex, count, existingData);
    }

    /// <summary>
    /// Copies a subvector into another vector storage without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="sourceIndex">The source start index.</param>
    /// <param name="targetIndex">The target start index.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void CopySubVectorToUnchecked(VectorStorage<T> target,
        int sourceIndex, int targetIndex, int count, ExistingData existingData)
    {
      if (ReferenceEquals(this, target))
      {
        var tmp = new T[count];
        for (int i = 0; i < tmp.Length; i++)
        {
          tmp[i] = At(i + sourceIndex);
        }
        for (int i = 0; i < tmp.Length; i++)
        {
          At(i + targetIndex, tmp[i]);
        }

        return;
      }

      for (int i = sourceIndex, ii = targetIndex; i < sourceIndex + count; i++, ii++)
      {
        target.At(ii, At(i));
      }
    }

    // SUB-ROW COPY

    /// <summary>
    /// Copies a range of elements into a row segment of a matrix storage.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="rowIndex">The target row index.</param>
    /// <param name="sourceColumnIndex">The source start index.</param>
    /// <param name="targetColumnIndex">The target column start index.</param>
    /// <param name="columnCount">The number of elements to copy.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void CopyToSubRow(MatrixStorage<T> target, int rowIndex,
        int sourceColumnIndex, int targetColumnIndex, int columnCount,
        ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (columnCount == 0)
      {
        return;
      }

      ValidateSubRowRange(target, rowIndex, sourceColumnIndex, targetColumnIndex, columnCount);
      CopyToSubRowUnchecked(target, rowIndex, sourceColumnIndex, targetColumnIndex, columnCount, existingData);
    }

    /// <summary>
    /// Copies a range of elements into a row segment of a matrix storage without validation.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="rowIndex">The target row index.</param>
    /// <param name="sourceColumnIndex">The source start index.</param>
    /// <param name="targetColumnIndex">The target column start index.</param>
    /// <param name="columnCount">The number of elements to copy.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void CopyToSubRowUnchecked(MatrixStorage<T> target, int rowIndex,
        int sourceColumnIndex, int targetColumnIndex, int columnCount, ExistingData existingData)
    {
      for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
      {
        target.At(rowIndex, jj, At(j));
      }
    }

    // SUB-COLUMN COPY

    /// <summary>
    /// Copies a range of elements into a column segment of a matrix storage.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="columnIndex">The target column index.</param>
    /// <param name="sourceRowIndex">The source start index.</param>
    /// <param name="targetRowIndex">The target row start index.</param>
    /// <param name="rowCount">The number of elements to copy.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void CopyToSubColumn(MatrixStorage<T> target, int columnIndex,
        int sourceRowIndex, int targetRowIndex, int rowCount,
        ExistingData existingData = ExistingData.Clear)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (rowCount == 0)
      {
        return;
      }

      ValidateSubColumnRange(target, columnIndex, sourceRowIndex, targetRowIndex, rowCount);
      CopyToSubColumnUnchecked(target, columnIndex, sourceRowIndex, targetRowIndex, rowCount, existingData);
    }

    /// <summary>
    /// Copies a range of elements into a column segment of a matrix storage without validation.
    /// </summary>
    /// <param name="target">The target matrix storage.</param>
    /// <param name="columnIndex">The target column index.</param>
    /// <param name="sourceRowIndex">The source start index.</param>
    /// <param name="targetRowIndex">The target row start index.</param>
    /// <param name="rowCount">The number of elements to copy.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void CopyToSubColumnUnchecked(MatrixStorage<T> target, int columnIndex,
        int sourceRowIndex, int targetRowIndex, int rowCount, ExistingData existingData)
    {
      for (int i = sourceRowIndex, ii = targetRowIndex; i < sourceRowIndex + rowCount; i++, ii++)
      {
        target.At(ii, columnIndex, At(i));
      }
    }

    // EXTRACT

    /// <summary>
    /// Copies the storage into a new array.
    /// </summary>
    /// <returns>A new array containing the storage values.</returns>
    public virtual T[] ToArray()
    {
      var ret = new T[Length];
      for (int i = 0; i < ret.Length; i++)
      {
        ret[i] = At(i);
      }
      return ret;
    }

    /// <summary>
    /// Returns the backing array if one is available.
    /// </summary>
    /// <returns>The backing array, or <see langword="null"/> if none is available.</returns>
    public virtual T[] AsArray()
    {
      return null;
    }

    // ENUMERATION

    /// <summary>
    /// Enumerates all values in the storage.
    /// </summary>
    /// <returns>An enumeration of the stored values.</returns>
    public virtual IEnumerable<T> Enumerate()
    {
      for (var i = 0; i < Length; i++)
      {
        yield return At(i);
      }
    }

    /// <summary>
    /// Enumerates all values in the storage together with their indices.
    /// </summary>
    /// <returns>An enumeration of index-value pairs.</returns>
    public virtual IEnumerable<(int, T)> EnumerateIndexed()
    {
      for (var i = 0; i < Length; i++)
      {
        yield return (i, At(i));
      }
    }

    /// <summary>
    /// Enumerates all nonzero values in the storage.
    /// </summary>
    /// <returns>An enumeration of the nonzero values.</returns>
    public virtual IEnumerable<T> EnumerateNonZero()
    {
      for (var i = 0; i < Length; i++)
      {
        var x = At(i);
        if (!Zero.Equals(x))
        {
          yield return x;
        }
      }
    }

    /// <summary>
    /// Enumerates all nonzero values in the storage together with their indices.
    /// </summary>
    /// <returns>An enumeration of index-value pairs for nonzero values.</returns>
    public virtual IEnumerable<(int, T)> EnumerateNonZeroIndexed()
    {
      for (var i = 0; i < Length; i++)
      {
        var x = At(i);
        if (!Zero.Equals(x))
        {
          yield return (i, x);
        }
      }
    }

    // FIND

    /// <summary>
    /// Finds the first element matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test elements.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <returns>The index and value of the first matching element, or <see langword="null"/> if none matches.</returns>
    public virtual Tuple<int, T> Find(Func<T, bool> predicate, Zeros zeros)
    {
      for (int i = 0; i < Length; i++)
      {
        var item = At(i);
        if (predicate(item))
        {
          return new Tuple<int, T>(i, item);
        }
      }
      return null;
    }

    /// <summary>
    /// Finds the first pair of elements from two storages that matches the specified predicate.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other storage.</typeparam>
    /// <param name="other">The other storage.</param>
    /// <param name="predicate">The predicate to test pairs of elements.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <returns>The index and values of the first matching pair, or <see langword="null"/> if none matches.</returns>
    public Tuple<int, T, TOther> Find2<TOther>(VectorStorage<TOther> other, Func<T, TOther, bool> predicate, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      if (Length != other.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
      }

      return Find2Unchecked(other, predicate, zeros);
    }

    /// <summary>
    /// Finds the first pair of elements from two storages that matches the specified predicate without validation.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other storage.</typeparam>
    /// <param name="other">The other storage.</param>
    /// <param name="predicate">The predicate used to test each pair of values.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <returns>The first matching indexed value pair, or <c>null</c> if none is found.</returns>
    internal virtual Tuple<int, T, TOther> Find2Unchecked<TOther>(VectorStorage<TOther> other, Func<T, TOther, bool> predicate, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      for (int i = 0; i < Length; i++)
      {
        var item = At(i);
        var otherItem = other.At(i);
        if (predicate(item, otherItem))
        {
          return new Tuple<int, T, TOther>(i, item, otherItem);
        }
      }
      return null;
    }

    // FUNCTIONAL COMBINATORS: MAP

    /// <summary>
    /// Applies a mapping function to each element in place.
    /// </summary>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    public virtual void MapInplace(Func<T, T> f, Zeros zeros)
    {
      for (int i = 0; i < Length; i++)
      {
        At(i, f(At(i)));
      }
    }

    /// <summary>
    /// Applies an indexed mapping function to each element in place.
    /// </summary>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    public virtual void MapIndexedInplace(Func<int, T, T> f, Zeros zeros)
    {
      for (int i = 0; i < Length; i++)
      {
        At(i, f(i, At(i)));
      }
    }

    /// <summary>
    /// Maps the storage into another storage.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void MapTo<TU>(VectorStorage<TU> target, Func<T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (Length != target.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      MapToUnchecked(target, f, zeros, existingData);
    }

    /// <summary>
    /// Maps the storage into another storage without validation.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void MapToUnchecked<TU>(VectorStorage<TU> target, Func<T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      for (int i = 0; i < Length; i++)
      {
        target.At(i, f(At(i)));
      }
    }

    /// <summary>
    /// Maps the storage into another storage using the element index.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void MapIndexedTo<TU>(VectorStorage<TU> target, Func<int, T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (Length != target.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      MapIndexedToUnchecked(target, f, zeros, existingData);
    }

    /// <summary>
    /// Maps the storage into another storage using the element index without validation.
    /// </summary>
    /// <typeparam name="TU">The target element type.</typeparam>
    /// <param name="target">The target storage.</param>
    /// <param name="f">The indexed mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void MapIndexedToUnchecked<TU>(VectorStorage<TU> target, Func<int, T, TU> f, Zeros zeros, ExistingData existingData)
        where TU : struct, IEquatable<TU>, IFormattable
    {
      for (int i = 0; i < Length; i++)
      {
        target.At(i, f(i, At(i)));
      }
    }

    /// <summary>
    /// Maps two storages into a target storage.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="other">The other source storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    public void Map2To(VectorStorage<T> target, VectorStorage<T> other, Func<T, T, T> f, Zeros zeros, ExistingData existingData)
    {
      if (target == null)
      {
        throw new ArgumentNullException(nameof(target));
      }

      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      if (Length != target.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(target));
      }

      if (Length != other.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
      }

      Map2ToUnchecked(target, other, f, zeros, existingData);
    }

    /// <summary>
    /// Maps two storages into a target storage without validation.
    /// </summary>
    /// <param name="target">The target storage.</param>
    /// <param name="other">The other source storage.</param>
    /// <param name="f">The mapping function.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <param name="existingData">Specifies how existing target data is handled.</param>
    internal virtual void Map2ToUnchecked(VectorStorage<T> target, VectorStorage<T> other, Func<T, T, T> f, Zeros zeros, ExistingData existingData)
    {
      for (int i = 0; i < Length; i++)
      {
        target.At(i, f(At(i), other.At(i)));
      }
    }

    // FUNCTIONAL COMBINATORS: FOLD

    /// <summary>
    /// Folds two storages into a single accumulated state.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other storage.</typeparam>
    /// <typeparam name="TState">The accumulator state type.</typeparam>
    /// <param name="other">The other storage.</param>
    /// <param name="f">The folding function.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <returns>The final accumulated state.</returns>
    public TState Fold2<TOther, TState>(VectorStorage<TOther> other, Func<TState, T, TOther, TState> f, TState state, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      if (Length != other.Length)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
      }

      return Fold2Unchecked(other, f, state, zeros);
    }

    /// <summary>
    /// Folds two storages into a single accumulated state without validation.
    /// </summary>
    /// <typeparam name="TOther">The element type of the other storage.</typeparam>
    /// <typeparam name="TState">The accumulator state type.</typeparam>
    /// <param name="other">The other storage.</param>
    /// <param name="f">The folding function.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="zeros">Specifies how zero values are treated.</param>
    /// <returns>The final accumulated state.</returns>
    internal virtual TState Fold2Unchecked<TOther, TState>(VectorStorage<TOther> other, Func<TState, T, TOther, TState> f, TState state, Zeros zeros)
        where TOther : struct, IEquatable<TOther>, IFormattable
    {
      for (int i = 0; i < Length; i++)
      {
        state = f(state, At(i), other.At(i));
      }

      return state;
    }
  }
}
