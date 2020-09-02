#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

// Modified (C) by Dr. Dirk Lellinger 2017

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra
{
  public class SparseDoubleVector : IROVector<double>
  {
    private const int IncrementSize = 16; // Size of chunk for array increments

    private int n;

    /// <summary>The nonzero elements of the sparse vector</summary>
    public double[] items;

    /// <summary>The indices of the nonzero elements in the sparse vector</summary>
    public int[] indices;

    /// <summary>Number of initialized elements</summary>
    public int count;

    /// <summary>Constructs a sparse vector with all zeros</summary>
    /// <param name="n">Length of the vector</param>
    public SparseDoubleVector(int n)
    {
      this.n = n;
      items = new double[IncrementSize];
      indices = new int[IncrementSize];
      count = 0;
    }

    /// <summary>Constructs a sparse vector with defined nonzero elements</summary>
    /// <param name="items">The nonzero entries</param>
    /// <param name="indices">The locations of the nonzeros</param>
    /// <param name="n">Length of the vector</param>
    public SparseDoubleVector(double[] items, int[] indices, int n)
    {
      if (items is null)
        throw new ArgumentNullException("items");
      if (indices is null)
        throw new ArgumentNullException("indices");
      this.items = items;
      this.indices = indices;
      count = items.Length;
      this.n = n;
    }

    /// <summary>Length of the sparse vector</summary>
    public int Length
    {
      get { return n; }
    }

    /// <summary>Length of the sparse vector</summary>
    public int Count
    {
      get { return n; }
    }

    public SparseDoubleVector Clone()
    {
      return n == 0 ? new SparseDoubleVector(0) : new SparseDoubleVector((double[])items.Clone(), (int[])indices.Clone(), n);
    }

    public IEnumerator<double> GetEnumerator()
    {
      for (int i = 0; i < Length; ++i)
        yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < Length; ++i)
        yield return this[i];
    }

    /// <summary>Public accessor method</summary>
    /// <param name="i">Index of the request</param>
    /// <returns>The ith element of a sparse vector</returns>
    public double this[int i]
    {
      get
      {
        if (i < 0 || i >= n)
          throw new IndexOutOfRangeException();
        int idx = Array.BinarySearch(indices, 0, count, i);
        if (idx < 0)
          return 0;
        else
          return items[idx];
      }
      set
      {
        if (i < 0 || i >= n)
          throw new IndexOutOfRangeException();
        int idx = Array.BinarySearch(indices, 0, count, i);
        if (idx >= 0)
          items[idx] = value;
        else
        {
          int indexToAdd = ~idx;
          if (count >= items.Length)
          {
            int delta = Math.Min(IncrementSize, n - items.Length);
            int[] newIndices = new int[indices.Length + delta];
            double[] newItems = new double[items.Length + delta];
            Array.Copy(indices, newIndices, indices.Length);
            Array.Copy(items, newItems, items.Length);
            items = newItems;
            indices = newIndices;
          }
          Array.Copy(indices, indexToAdd, indices, indexToAdd + 1, count - indexToAdd);
          Array.Copy(items, indexToAdd, items, indexToAdd + 1, count - indexToAdd);
          count++;
          indices[indexToAdd] = i;
          items[indexToAdd] = value;
        }
      }
    }
  }
}
