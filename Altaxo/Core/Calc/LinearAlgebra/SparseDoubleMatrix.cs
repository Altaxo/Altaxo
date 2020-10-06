#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

// Modified (C) by Dr. Dirk Lellinger 2017

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>Sparse matrix class</summary>
  public class SparseDoubleMatrix : IROSparseMatrix<double>, IMatrix<double>
  {
    private static readonly int[] _emptyIntArray = new int[0];
    private static readonly double[] _emptyDoubleArray = new double[0];
    private const int Delta = 1;
    private int n, m;
    private double[][] items;
    private int[][] indices;
    private int[] count;


    /// <summary>Constructor for SparseMatrix class</summary>
    /// <param name="numberOfRows">Number of rows</param>
    /// <param name="numberOfColumns">Number of columns</param>
    public SparseDoubleMatrix(int numberOfRows, int numberOfColumns)
    {
      m = numberOfRows;
      n = numberOfColumns;

      items = new double[numberOfRows][];
      indices = new int[numberOfRows][];
      count = new int[numberOfRows];
    }

    public SparseDoubleMatrix(int m, int n, double[][] matrixItems, int[][] matrixIndices, int[] _count)
    {
      this.m = m;
      this.n = n;
      items = matrixItems;
      indices = matrixIndices;
      count = _count;
    }

    public int[] Count
    {
      get { return count; }
    }

    /// <summary>Get row dimension.</summary>
    /// <returns>     m, the number of rows</returns>
    public int RowCount
    {
      get { return m; }
    }

    /// <summary>Get column dimension.</summary>
    /// <returns>     n, the number of columns.</returns>
    public int ColumnCount
    {
      get { return n; }
    }

    /// <summary>
    /// Get the valid indices for a given row.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public int[] GetIndicesOfRow(int row)
    {
      return indices[row] ?? _emptyIntArray;
    }

    /// <summary>
    /// Determines whether this sparse matrix is a band matrix, and determines the lower and upper band width.
    /// </summary>
    /// <returns></returns>
    public (bool isBandMatrix, int p, int q) IsBandMatrix()
    {
      int maxLowerBandwidth = 0;
      int maxUpperBandwidth = 0;

      for (int iRow = 0; iRow < RowCount; ++iRow)
      {
        if (count[iRow] == 0)
          continue;

        var ind = indices[iRow];
        var lower = iRow - ind[0];
        var upper = ind[count[iRow] - 1] - iRow;

        if (lower > maxLowerBandwidth)
          maxLowerBandwidth = lower;
        if (upper > maxUpperBandwidth)
          maxUpperBandwidth = upper;
      }

      return (maxLowerBandwidth < RowCount - 1 || maxUpperBandwidth < ColumnCount - 1, maxLowerBandwidth, maxUpperBandwidth);
    }

    public SparseDoubleMatrix Clone()
    {
      var A = new SparseDoubleMatrix(m, n);
      for (int i = 0; i < m; i++)
      {
        A.indices[i] = (int[])indices[i].Clone();
        A.items[i] = (double[])items[i].Clone();
        A.count = (int[])count.Clone();
      }
      return A;
    }

    /// <summary>
    /// Sets all elements of the sparse matrix to zero.
    /// </summary>
    public void Clear()
    {
      Array.Clear(count, 0, count.Length);
    }

    /// <summary>Dense version of a sparse matrix</summary>
    /// <returns>A matrix equivalent</returns>
    public DoubleMatrix DenseMatrix()
    {
      var DM = new DoubleMatrix(m, n);
      for (int i = 0; i < m; i++)
        for (int j = 0; j < count[i]; j++)
          DM[i, indices[i][j]] = items[i][j];
      return DM;
    }

    /// <summary>Matrix addition for a sparse matrix</summary>
    /// <param name="B">The matrix to add</param>
    /// <returns>The result A + B</returns>
    public SparseDoubleMatrix plus(SparseDoubleMatrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      var C = new SparseDoubleMatrix(m, n);

      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null && indices[i].Length > 0)
        {
          C.indices[i] = new int[count[i]];
          C.items[i] = new double[count[i]];
          for (int j = 0; j < count[i]; j++)
          {
            C.indices[i][j] = indices[i][j];
            C.items[i][j] = items[i][j] + B.items[i][j];
          }
        }
        C.count[i] = count[i];
      }

      return C;
    }

    /// <summary>C = A + B</summary>
    /// <param name="A">first matrix </param>
    /// <param name="B">another matrix </param>
    /// <returns>     A + B</returns>
    public static SparseDoubleMatrix operator +(SparseDoubleMatrix A, SparseDoubleMatrix B)
    {
      if (A is null)
        throw new ArgumentNullException(nameof(A));
      if (B is null)
        throw new ArgumentNullException(nameof(B));
      return A.plus(B);
    }

    /// <summary>Matrix subtraction for a sparse matrix</summary>
    /// <param name="B">The matrix to subtract</param>
    /// <returns>The result A - B</returns>
    public SparseDoubleMatrix minus(SparseDoubleMatrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      var C = new SparseDoubleMatrix(m, n);

      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null && indices[i].Length > 0)
        {
          C.indices[i] = new int[count[i]];
          C.items[i] = new double[count[i]];
          for (int j = 0; j < count[i]; j++)
          {
            C.indices[i][j] = indices[i][j];
            C.items[i][j] = items[i][j] - B.items[i][j];
          }
        }
        C.count[i] = count[i];
      }

      return C;
    }

    /// <summary>Matrix subtraction for a sparse matrix</summary>
    /// <param name="A">The matrix from which to subtract</param>
    /// <param name="B">The matrix to subtract</param>
    /// <returns>The result A - B</returns>
    public static SparseDoubleMatrix operator -(SparseDoubleMatrix A, SparseDoubleMatrix B)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if (B is null)
        throw new ArgumentNullException("B");
      return A.minus(B);
    }

    /// <summary>Matrix multiplication</summary>
    /// <param name="v">Vector</param>
    /// <returns></returns>
    public DoubleVector times(IReadOnlyList<double> v)
    {
      var result = new DoubleVector(m);
      unchecked // Turns off integral overflow checking: small speedup
      {
        for (int i = 0; i < m; i++)
          if (indices[i] is not null && indices[i].Length > 0)
          {
            double s = 0;
            for (int k = 0; k < count[i]; k++)
              s += items[i][k] * v[indices[i][k]];
            result[i] = s;
          }
      }
      return result;
    }

    /// <summary>Matrix multiplication</summary>
    /// <param name="m"></param>
    /// <param name="v">Vector</param>
    /// <returns></returns>
    public static DoubleVector operator *(SparseDoubleMatrix m, IReadOnlyList<double> v)
    {
      if (m is null)
        throw new ArgumentNullException("m");
      if ((double[])v is null)
        throw new ArgumentNullException("v");
      return m.times(v);
    }

    /// <summary>Matrix multiplication, y = v * A</summary>
    /// <param name="v">Vector</param>
    /// <returns>y</returns>
    public DoubleVector timesRight(IReadOnlyList<double> v)
    {
      var result = new DoubleVector(n);
      unchecked // Turns off integral overflow checking: small speedup
      {
        for (int i = 0; i < n; i++)
          if (indices[i] is not null && indices[i].Length > 0)
          {
            for (int k = 0; k < count[i]; k++)
              result[indices[i][k]] += v[i] * items[i][k];
          }
      }
      return result;
    }

    /// <summary>Matrix multiplication</summary>
    /// <param name="v">Vector</param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static DoubleVector operator *(IReadOnlyList<double> v, SparseDoubleMatrix m)
    {
      if (m is null)
        throw new ArgumentNullException("m");
      if ((double[])v is null)
        throw new ArgumentNullException("v");
      return m.timesRight(v);
    }

    /// <summary>Matrix multiplication by a scalar</summary>
    /// <param name="s">Scalar</param>
    /// <returns>Scaled sparse matrix</returns>
    public SparseDoubleMatrix times(double s)
    {
      var B = new SparseDoubleMatrix(m, n);

      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null && indices[i].Length > 0)
        {
          B.indices[i] = new int[count[i]];
          B.items[i] = new double[count[i]];
          for (int j = 0; j < count[i]; j++)
          {
            B.indices[i][j] = indices[i][j];
            B.items[i][j] = s * items[i][j];
          }
        }
        B.count[i] = count[i];
      }
      return B;
    }

    /// <summary>Matrix multiplication by a scalar</summary>
    /// <param name="A">Sparse Matrix</param>
    /// <param name="s">Scalar</param>
    /// <returns>Scaled sparse matrix</returns>
    public static SparseDoubleMatrix operator *(SparseDoubleMatrix A, double s)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      return A.times(s);
    }

    /// <summary>Matrix multiplication by a scalar</summary>
    /// <param name="s">Scaling factor</param>
    /// <returns>Scaled sparse matrix</returns>
    public SparseDoubleMatrix Mul(double s)
    {
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < count[i]; j++)
        {
          items[i][j] *= s;
        }
      }
      return this;
    }

    /// <summary>Matrix right multiplication by a matrix</summary>
    /// <param name="B">Scaling factor</param>
    /// <returns>A * B where A is current sparce matrix</returns>
    public SparseDoubleMatrix times(SparseDoubleMatrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      if (B.m != n)
      {
        throw new System.ArgumentException("Sparse natrix inner dimensions must agree.");
      }

      var C = new SparseDoubleMatrix(m, B.n);
      int idx, ii;
      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null && indices[i].Length > 0)
          for (int j = 0; j < B.n; j++)
          {
            for (int jj = 0; jj < count[i]; jj++)
            {
              ii = indices[i][jj];
              if (B.indices[ii] is not null && B.indices[ii].Length > 0)
              {
                idx = Array.BinarySearch(B.indices[ii], 0, B.count[ii], j);
                if (idx >= 0)
                  C[i, j] += items[i][jj] * B.items[ii][idx];
              }
            }
          }
      }

      return C;
    }

    public static SparseDoubleMatrix operator *(SparseDoubleMatrix A, SparseDoubleMatrix B)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if (B is null)
        throw new ArgumentNullException("B");
      return A.times(B);
    }

    public IEnumerable<(int row, int column, double value)> EnumerateElementsIndexed(Zeros zeros = Zeros.AllowSkip)
    {
      switch (zeros)
      {
        case Zeros.AllowSkip:
          {
            for (int i = 0; i < m; ++i)
            {
              var srcIndicesLength = count[i];
              if (srcIndicesLength > 0)
              {
                var srcIndices = indices[i];
                var items_i = items[i];
                for (int jj = 0; jj < srcIndices.Length; ++jj)
                  yield return (i, srcIndices[jj], items_i[jj]);
              }
            }
          }
          break;

        case Zeros.AllowSkipButIncludeDiagonal:
          {
            throw new NotImplementedException("Yet to be implemented and tested");
          }


        case Zeros.Include:
          {
            throw new NotImplementedException("Yet to be implemented and tested");
          }

      }
    }

    /// <summary>
    /// Maps the present elements of this matrix (and additionally the diagonal elements, independently if they are present or not),
    /// via a evaluation function to a resulting sparse matrix.
    /// </summary>
    /// <param name="f">The mapping function. First arg is the element of the source matrix, 2nd arg is the row, 3rd arg the column number. </param>
    /// <param name="result">The resulting sparse matrix.</param>
    public void MapSparseIncludingDiagonal(Func<double, int, int, double> f, SparseDoubleMatrix result)
    {
      int reqLength;
      for (int row = 0; row < m; ++row)
      {
        var srcIndicesLength = count[row];
        if (srcIndicesLength > 0)
        {
          var srcIndices = indices[row];
          int jidx = Array.BinarySearch(srcIndices, 0, srcIndicesLength, row);

          reqLength = jidx < 0 ? srcIndicesLength + 1 : srcIndicesLength;

          // allocate required length of row arrays
          if ((result.indices[row]?.Length ?? 0) < reqLength)
          {
            result.indices[row] = new int[reqLength];
            result.items[row] = new double[reqLength];
          }

          if (jidx >= 0) // diagonal element is included in src matrix
          {
            double val;
            int j = 0;
            for (int i = 0; i < srcIndicesLength; ++i)
            {
              int idx = srcIndices[i];
              val = f(items[row][i], row, idx);
              if (!(val == 0))
              {
                result.indices[row][j] = idx;
                result.items[row][j] = val;
                ++j;
              }
            }
            result.count[row] = j;
          }
          else // jidx <0, thus diagonal element is not included in src matrix
          {
            int upTo = Math.Min(srcIndicesLength, ~jidx); // ~jidx is now the index of the element that is larger than row
            double val;

            // first the elements left of the diagonal
            int j = 0;
            for (int i = 0; i < upTo; ++i)
            {
              int idx = srcIndices[i];
              val = f(items[row][i], row, idx);
              if (!(0 == val))
              {
                result.indices[row][j] = idx;
                result.items[row][j] = val;
                ++j;
              }
            }

            // now the diagonal
            val = f(0, row, row); // 0 as value because original matrix doesn't have this element, thus it is zero
            if (!(0 == val))
            {
              result.indices[row][j] = row;
              result.items[row][j] = val;
              ++j;
            }

            // now the elements right of the diagonal
            for (int i = upTo; i < srcIndicesLength; ++i)
            {
              int idx = srcIndices[i];
              val = f(items[row][i], row, idx);
              if (!(0 == val))
              {
                result.indices[row][j] = idx;
                result.items[row][j] = val;
                ++j;
              }
            }
            result.count[row] = j;
          }
        }
        else // Matrix row was not used before
        {
          double val = f(0, row, row);

          if (!(0 == val))
          {
            if ((result.indices[row]?.Length ?? 0) < 1)
            {
              result.indices[row] = new int[Delta];
              result.items[row] = new double[Delta];
            }
            result.count[row] = 1;

            result.indices[row][0] = row;
            result.items[row][0] = val;
          }
          else // value is null
          {
            result.indices[row] = _emptyIntArray;
            result.count[row] = 0;
            result.items[row] = _emptyDoubleArray;
          }
        }
      }
    }

    /// <summary>Accessor method for (i,j)th element</summary>
    /// <param name="i">Row index</param>
    /// <param name="j">Column index</param>
    /// <returns>(i,j)th element</returns>
    public double this[int i, int j]
    {
      get
      {
#if DEBUG
        if (i < 0 || j < 0 || i >= m || j >= n)
          throw new IndexOutOfRangeException(string.Format(CultureInfo.InvariantCulture,
              "Element index ({0},{1}) is out of range", i, j));
#endif
        if (indices[i] is null || indices[i].Length == 0)
          return 0;
        int jidx = Array.BinarySearch(indices[i], 0, count[i], j);
        if (jidx < 0)
          return 0;
        else
          return items[i][jidx];
      }
      set
      {
#if DEBUG
        if (i < 0 || j < 0 || i >= m || j >= n)
          throw new IndexOutOfRangeException(string.Format(CultureInfo.InvariantCulture,
              "Element index ({0},{1}) is out of range", i, j));
#endif
        if (indices[i] is null || indices[i].Length == 0)
        {
          indices[i] = new int[Delta];
          items[i] = new double[Delta];
          indices[i][0] = j;
          items[i][0] = value;
          count[i] = 1;
        }
        else
        {
          int jidx = Array.BinarySearch(indices[i], 0, count[i], j);
          if (jidx >= 0)
            items[i][jidx] = value;
          else
          {
            int indexToAdd = ~jidx;
            if (count[i] >= items[i].Length)
            {
              int delta = Math.Min(Delta, n - items[i].Length);
              int[] newIndices = new int[indices[i].Length + delta];
              double[] newItems = new double[items[i].Length + delta];
              Array.Copy(indices[i], newIndices, indexToAdd);
              Array.Copy(items[i], newItems, indexToAdd);
              Array.Copy(indices[i], indexToAdd, newIndices, indexToAdd + 1, count[i] - indexToAdd);
              Array.Copy(items[i], indexToAdd, newItems, indexToAdd + 1, count[i] - indexToAdd);
              items[i] = newItems;
              indices[i] = newIndices;
            }
            else
            {
              Array.Copy(indices[i], indexToAdd, indices[i], indexToAdd + 1, count[i] - indexToAdd);
              Array.Copy(items[i], indexToAdd, items[i], indexToAdd + 1, count[i] - indexToAdd);
            }
            count[i]++;
            indices[i][indexToAdd] = j;
            items[i][indexToAdd] = value;
          }
        }
      }
    }

    /// <summary>Tranpose of sparse matrix</summary>
    /// <returns>New matrix that is the transposed of the original.</returns>
    public SparseDoubleMatrix Transpose()
    {
      var At = new SparseDoubleMatrix(ColumnCount, RowCount);

      for (int i = 0; i < RowCount; i++)
        for (int j = 0; j < GetRow(i).count; j++)
          At[GetRow(i).indices[j], i] = GetRow(i).items[j];

      return At;
    }

    /// <summary>Accessor method for ith row</summary>
    /// <param name="i">Row index</param>
    /// <returns>The ith row as a SparseVector</returns>
    public SparseDoubleVector GetRow(int i)
    {
      return new SparseDoubleVector(items[i], indices[i], n);
    }

    public void SetRow(int i, SparseDoubleVector value)
    {
      indices[i] = value.indices;
      items[i] = value.items;
      count[i] = value.Length;
    }

    /// <summary>Method to rescale a row of a Sparse Matrix</summary>
    /// <param name="i">Index of the row to be scaled</param>
    /// <param name="j1">Lowest column entry to scale</param>
    /// <param name="j2">High column entry to scale</param>
    /// <param name="sf">Scale factor</param>
    public void ScaleRow(int i, int j1, int j2, double sf)
    {
      for (int k = 0; k < count[i]; k++)
        if ((indices[i][k] >= j1) && (indices[i][k] <= j2))
          items[i][k] *= sf;
    }

    /// <summary>Switch rows of a sparse matrix</summary>
    public void SwitchRows(int i, int j)
    {
      var tempItems = items[i];
      var tempIndices = indices[i];
      var tempCount = count[i];

      items[i] = items[j];
      indices[i] = indices[j];
      count[i] = count[j];

      items[j] = tempItems;
      indices[j] = tempIndices;
      count[j] = tempCount;
    }

    /// <summary>Forward substitution routine for solving Lx = b, where L is a lower-triangular matrix</summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public DoubleVector SolveLower(IReadOnlyList<double> b)
    {
      var x = new DoubleVector(m);

      for (int i = 0; i < m; i++)
      {
        x[i] = b[i];
        var idx = indices[i];
        var its = items[i];
        for (int k = 0; k < count[i]; k++)
        {
          int j = idx[k];
          if (j < i)
            x[i] -= its[k] * x[j];
        }
        x[i] /= GetRow(i)[i];
      }

      return x;
    }

    /// <summary>Identity matrix in sparse form</summary>
    /// <param name="m">Row dimension</param>
    /// <param name="n">Column dimension</param>
    /// <returns>An m x n sparse identity matrix</returns>
    public static SparseDoubleMatrix Identity(int m, int n)
    {
      int o = Math.Min(m, n);
      var A = new SparseDoubleMatrix(m, n);
      for (int i = 0; i < o; i++)
        A[i, i] = 1.0;

      return A;
    }

    /// <summary>
    /// Check if the current matrix is lower triangular
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsLowerTriangular()
    {
      for (int i = 0; i < m; i++)
        if (indices[i].Last() > i)
          return false;

      return true;
    }

    /// <summary>Check if size(A) == size(B)</summary>
    private void CheckMatrixDimensions(Matrix B)
    {
      if (B.RowCount != m || B.ColumnCount != n)
      {
        throw new System.ArgumentException("Sparse matrix dimensions must agree.");
      }
    }
  }
}
