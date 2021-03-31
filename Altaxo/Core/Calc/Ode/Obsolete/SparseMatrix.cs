#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Globalization;
using System.Linq;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>Sparse matrix class</summary>
  public class SparseMatrix
  {
    private const int Delta = 1;

    private int n, m;
    private double[][] items;
    private int[][] indices;
    private int[] count;

    /// <summary>Constructor for SparseMatrix class</summary>
    /// <param name="m">Number of rows</param>
    /// <param name="n">Number of columns</param>
    public SparseMatrix(int m, int n)
    {
      this.m = m;
      this.n = n;

      items = new double[m][];
      indices = new int[m][];
      count = new int[m];
    }

    public SparseMatrix(int m, int n, double[][] matrixItems, int[][] matrixIndices, int[] _count)
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
    public int RowDimension
    {
      get { return m; }
    }

    /// <summary>Get column dimension.</summary>
    /// <returns>     n, the number of columns.</returns>
    public int ColumnDimension
    {
      get { return n; }
    }

    public SparseMatrix Copy()
    {
      var A = new SparseMatrix(m, n);
      for (int i = 0; i < m; i++)
      {
        A.indices[i] = (int[])indices[i].Clone();
        A.items[i] = (double[])items[i].Clone();
        A.count = (int[])count.Clone();
      }
      return A;
    }

    /// <summary>Dense version of a sparse matrix</summary>
    /// <returns>A matrix equivalent</returns>
    public Matrix DenseMatrix()
    {
      var DM = new Matrix(m, n, 0.0);
      for (int i = 0; i < m; i++)
        for (int j = 0; j < count[i]; j++)
          DM[i, indices[i][j]] = items[i][j];
      return DM;
    }

    /// <summary>Matrix addition for a sparse matrix</summary>
    /// <param name="B">The matrix to add</param>
    /// <returns>The result A + B</returns>
    public SparseMatrix plus(SparseMatrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      var C = new SparseMatrix(m, n);

      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null)
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
    public static SparseMatrix operator +(SparseMatrix A, SparseMatrix B)
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
    public SparseMatrix minus(SparseMatrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      var C = new SparseMatrix(m, n);

      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null)
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
    public static SparseMatrix operator -(SparseMatrix A, SparseMatrix B)
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
    public Vector times(Vector v)
    {
      double[] vv = v;
      var result = Vector.Zeros(m);
      unchecked // Turns off integral overflow checking: small speedup
      {
        for (int i = 0; i < m; i++)
          if (indices[i] is not null)
          {
            double s = 0;
            for (int k = 0; k < count[i]; k++)
              s += items[i][k] * vv[indices[i][k]];
            result[i] = s;
          }
      }
      return result;
    }

    /// <summary>Matrix multiplication</summary>
    /// <param name="m">First factor</param>
    /// <param name="v">Second factor</param>
    /// <returns></returns>
    public static Vector operator *(SparseMatrix m, Vector v)
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
    public Vector timesRight(Vector v)
    {
      double[] vv = v;
      var result = Vector.Zeros(n);
      unchecked // Turns off integral overflow checking: small speedup
      {
        for (int i = 0; i < n; i++)
          if (indices[i] is not null)
          {
            for (int k = 0; k < count[i]; k++)
              result[indices[i][k]] += vv[i] * items[i][k];
          }
      }
      return result;
    }

    /// <summary>Matrix multiplication</summary>
    /// <param name="v">Vector</param>
    /// <param name="m">2nd factor</param>
    /// <returns></returns>
    public static Vector operator *(Vector v, SparseMatrix m)
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
    public SparseMatrix times(double s)
    {
      var B = new SparseMatrix(m, n);

      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null)
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
    public static SparseMatrix operator *(SparseMatrix A, double s)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      return A.times(s);
    }

    /// <summary>Matrix multiplication by a scalar</summary>
    /// <param name="s">Scaling factor</param>
    /// <returns>Scaled sparse matrix</returns>
    public SparseMatrix Mul(double s)
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
    public SparseMatrix times(SparseMatrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      if (B.m != n)
      {
        throw new System.ArgumentException("Sparse natrix inner dimensions must agree.");
      }

      var C = new SparseMatrix(m, B.n);
      int idx, ii;
      for (int i = 0; i < m; i++)
      {
        if (indices[i] is not null)
          for (int j = 0; j < B.n; j++)
          {
            for (int jj = 0; jj < count[i]; jj++)
            {
              ii = indices[i][jj];
              if (B.indices[ii] is not null)
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

    public static SparseMatrix operator *(SparseMatrix A, SparseMatrix B)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if (B is null)
        throw new ArgumentNullException("B");
      return A.times(B);
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
        if (indices[i] is null)
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
        if (indices[i] is null)
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
    public SparseMatrix Transpose()
    {
      var At = new SparseMatrix(ColumnDimension, RowDimension);

      for (int i = 0; i < RowDimension; i++)
        for (int j = 0; j < this[i].count; j++)
          At[this[i].indices[j], i] = this[i].items[j];

      return At;
    }

    /// <summary>Accessor method for ith row</summary>
    /// <param name="i">Row index</param>
    /// <returns>The ith row as a SparseVector</returns>
    public SparseVector this[int i]
    {
      get
      {
        return new SparseVector(items[i], indices[i], n);
      }
      set
      {
        indices[i] = value.indices;
        items[i] = value.items;
        count[i] = value.Length;
      }
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

    /// <summary>Gaussian elimination method for A*x=b with partial pivoting</summary>
    /// <param name="b">The right hand side vector</param>
    /// <returns>The solution x</returns>
    public Vector SolveGE(Vector b) { return Gauss.SolveCore(Copy(), b); }

    /// <summary>Forward substitution routine for solving Lx = b, where L is a lower-triangular matrix</summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public Vector SolveLower(Vector b)
    {
      var x = Vector.Zeros(m);

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
        x[i] /= this[i][i];
      }

      return x;
    }

    /// <summary>Identity matrix in sparse form</summary>
    /// <param name="m">Row dimension</param>
    /// <param name="n">Column dimension</param>
    /// <returns>An m x n sparse identity matrix</returns>
    public static SparseMatrix Identity(int m, int n)
    {
      int o = Math.Min(m, n);
      var A = new SparseMatrix(m, n);
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
      if (B.RowDimension != m || B.ColumnDimension != n)
      {
        throw new System.ArgumentException("Sparse matrix dimensions must agree.");
      }
    }
  }
}
