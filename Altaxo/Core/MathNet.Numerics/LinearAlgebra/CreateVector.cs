// <copyright file="CreateVector.cs" company="Math.NET">
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
using Altaxo.Calc.Distributions;
using Altaxo.Calc.LinearAlgebra.Storage;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Provides convenience factory methods for creating vectors.
  /// </summary>
  public static class CreateVector
  {
    /// <summary>
    /// Create a new vector straight from an initialized matrix storage instance.
    /// If you have an instance of a discrete storage type instead, use their direct methods instead.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="storage">The storage backing the vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> WithStorage<T>(VectorStorage<T> storage)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.OfStorage(storage);
    }

    /// <summary>
    /// Create a new vector with the same kind of the provided example.
    /// </summary>
    /// <typeparam name="T">The element type of the vector to create.</typeparam>
    /// <typeparam name="TU">The element type of the example vector.</typeparam>
    /// <param name="example">The example vector that determines the storage kind.</param>
    /// <param name="length">The length of the vector to create.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SameAs<T, TU>(Vector<TU> example, int length)
        where T : struct, IEquatable<T>, IFormattable
        where TU : struct, IEquatable<TU>, IFormattable
    {
      return Vector<T>.Build.SameAs(example, length);
    }

    /// <summary>
    /// Create a new vector with the same kind and dimension of the provided example.
    /// </summary>
    /// <typeparam name="T">The element type of the vector to create.</typeparam>
    /// <typeparam name="TU">The element type of the example vector.</typeparam>
    /// <param name="example">The example vector that determines the storage kind and size.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SameAs<T, TU>(Vector<TU> example)
        where T : struct, IEquatable<T>, IFormattable
        where TU : struct, IEquatable<TU>, IFormattable
    {
      return Vector<T>.Build.SameAs(example);
    }

    /// <summary>
    /// Create a new vector with the same kind of the provided example.
    /// </summary>
    /// <typeparam name="T">The element type of the vector to create.</typeparam>
    /// <typeparam name="TU">The element type of the example matrix.</typeparam>
    /// <param name="example">The example matrix that determines the storage kind.</param>
    /// <param name="length">The length of the vector to create.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SameAs<T, TU>(Matrix<TU> example, int length)
        where T : struct, IEquatable<T>, IFormattable
        where TU : struct, IEquatable<TU>, IFormattable
    {
      return Vector<T>.Build.SameAs(example, length);
    }

    /// <summary>
    /// Create a new vector with a type that can represent and is closest to both provided samples.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="example">The first example vector.</param>
    /// <param name="otherExample">The second example vector.</param>
    /// <param name="length">The length of the vector to create.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SameAs<T>(Vector<T> example, Vector<T> otherExample, int length)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SameAs(example, otherExample, length);
    }

    /// <summary>
    /// Create a new vector with a type that can represent and is closest to both provided samples and the dimensions of example.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="example">The first example vector.</param>
    /// <param name="otherExample">The second example vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SameAs<T>(Vector<T> example, Vector<T> otherExample)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SameAs(example, otherExample);
    }

    /// <summary>
    /// Create a new vector with a type that can represent and is closest to both provided samples.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="matrix">The example matrix.</param>
    /// <param name="vector">The example vector.</param>
    /// <param name="length">The length of the vector to create.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SameAs<T>(Matrix<T> matrix, Vector<T> vector, int length)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SameAs(matrix, vector, length);
    }

    /// <summary>
    /// Create a new dense vector with values sampled from the provided random distribution.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="distribution">The distribution used to sample values.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Random<T>(int length, IContinuousDistribution distribution)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Random(length, distribution);
    }

    /// <summary>
    /// Create a new dense vector with values sampled from the standard distribution with a system random source.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Random<T>(int length)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Random(length);
    }

    /// <summary>
    /// Create a new dense vector with values sampled from the standard distribution with a system random source.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="seed">The random seed.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Random<T>(int length, int seed)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Random(length, seed);
    }

    /// <summary>
    /// Create a new dense vector straight from an initialized vector storage instance.
    /// The storage is used directly without copying.
    /// Intended for advanced scenarios where you're working directly with
    /// storage for performance or interop reasons.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="storage">The storage backing the vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Dense<T>(DenseVectorStorage<T> storage)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Dense(storage);
    }

    /// <summary>
    /// Create a dense vector of T with the given size.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="size">The size of the vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Dense<T>(int size)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Dense(size);
    }

    /// <summary>
    /// Create a dense vector of T that is directly bound to the specified array.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="array">The array backing the vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Dense<T>(T[] array)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Dense(array);
    }

    /// <summary>
    /// Create a new dense vector and initialize each value using the provided value.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="value">The value assigned to each entry.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Dense<T>(int length, T value)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Dense(length, value);
    }

    /// <summary>
    /// Create a new dense vector and initialize each value using the provided init function.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="init">The initializer used to populate each entry.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Dense<T>(int length, Func<int, T> init)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Dense(length, init);
    }

    /// <summary>
    /// Create a new dense vector as a copy of the given other vector.
    /// This new vector will be independent from the other vector.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="vector">The vector to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> DenseOfVector<T>(Vector<T> vector)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.DenseOfVector(vector);
    }

    /// <summary>
    /// Create a new dense vector as a copy of the given array.
    /// This new vector will be independent from the array.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="array">The array to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> DenseOfArray<T>(T[] array)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.DenseOfArray(array);
    }

    /// <summary>
    /// Create a new dense vector as a copy of the given enumerable.
    /// This new vector will be independent from the enumerable.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="enumerable">The sequence of values to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> DenseOfEnumerable<T>(IEnumerable<T> enumerable)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.DenseOfEnumerable(enumerable);
    }

    /// <summary>
    /// Create a new dense vector as a copy of the given indexed enumerable.
    /// Keys must be provided at most once, zero is assumed if a key is omitted.
    /// This new vector will be independent from the enumerable.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="enumerable">The indexed values to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> DenseOfIndexed<T>(int length, IEnumerable<Tuple<int, T>> enumerable)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.DenseOfIndexed(length, enumerable);
    }

    /// <summary>
    /// Create a new dense vector as a copy of the given indexed enumerable.
    /// Keys must be provided at most once, zero is assumed if a key is omitted.
    /// This new vector will be independent from the enumerable.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="enumerable">The indexed values to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> DenseOfIndexed<T>(int length, IEnumerable<(int, T)> enumerable)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.DenseOfIndexed(length, enumerable);
    }

    /// <summary>
    /// Create a new sparse vector straight from an initialized vector storage instance.
    /// The storage is used directly without copying.
    /// Intended for advanced scenarios where you're working directly with
    /// storage for performance or interop reasons.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="storage">The storage backing the vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Sparse<T>(SparseVectorStorage<T> storage)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Sparse(storage);
    }

    /// <summary>
    /// Create a sparse vector of T with the given size.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="size">The size of the vector.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Sparse<T>(int size)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Sparse(size);
    }

    /// <summary>
    /// Create a new sparse vector and initialize each value using the provided value.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="value">The value assigned to each entry.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Sparse<T>(int length, T value)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Sparse(length, value);
    }

    /// <summary>
    /// Create a new sparse vector and initialize each value using the provided init function.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="init">The initializer used to populate each entry.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> Sparse<T>(int length, Func<int, T> init)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.Sparse(length, init);
    }

    /// <summary>
    /// Create a new sparse vector as a copy of the given other vector.
    /// This new vector will be independent from the other vector.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="vector">The vector to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SparseOfVector<T>(Vector<T> vector)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SparseOfVector(vector);
    }

    /// <summary>
    /// Create a new sparse vector as a copy of the given array.
    /// This new vector will be independent from the array.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="array">The array to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SparseOfArray<T>(T[] array)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SparseOfArray(array);
    }

    /// <summary>
    /// Create a new sparse vector as a copy of the given enumerable.
    /// This new vector will be independent from the enumerable.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="enumerable">The sequence of values to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SparseOfEnumerable<T>(IEnumerable<T> enumerable)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SparseOfEnumerable(enumerable);
    }

    /// <summary>
    /// Create a new sparse vector as a copy of the given indexed enumerable.
    /// Keys must be provided at most once, zero is assumed if a key is omitted.
    /// This new vector will be independent from the enumerable.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="enumerable">The indexed values to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SparseOfIndexed<T>(int length, IEnumerable<Tuple<int, T>> enumerable)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SparseOfIndexed(length, enumerable);
    }

    /// <summary>
    /// Create a new sparse vector as a copy of the given indexed enumerable.
    /// Keys must be provided at most once, zero is assumed if a key is omitted.
    /// This new vector will be independent from the enumerable.
    /// A new memory block will be allocated for storing the vector.
    /// </summary>
    /// <typeparam name="T">The element type of the vector.</typeparam>
    /// <param name="length">The length of the vector to create.</param>
    /// <param name="enumerable">The indexed values to copy.</param>
    /// <returns>The created vector.</returns>
    public static Vector<T> SparseOfIndexed<T>(int length, IEnumerable<(int, T)> enumerable)
        where T : struct, IEquatable<T>, IFormattable
    {
      return Vector<T>.Build.SparseOfIndexed(length, enumerable);
    }
  }
}
