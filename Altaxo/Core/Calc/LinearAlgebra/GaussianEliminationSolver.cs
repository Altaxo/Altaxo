#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

// Modified (C) by Dr. Dirk Lellinger 2017

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>Provides implementation of Gaussian elimination with partial pivoting</summary>
  public class GaussianEliminationSolver : ILinearEquationSolver<double>
  {
    private MatrixWrapperStructForLeftSpineJaggedArray<double> _temp_A;
    private double[]? _temp_b;
    private double[]? _temp_x;

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.
    /// Attention! Both matrix A and vector b are destroyed (changed).</summary>
    /// <param name="A">Elements of matrix 'A'. This array is modified!</param>
    /// <param name="b">Right part 'b'. This array is also modified!</param>
    /// <param name="x">Vector to store the result, i.e. the solution to the problem a x = b.</param>
    public void SolveDestructive(IMatrix<double> A, double[] b, double[] x)
    {
      if (A == null)
        throw new ArgumentNullException(nameof(A));
      if (b == null)
        throw new ArgumentNullException(nameof(b));
      if (x == null)
        throw new ArgumentException(nameof(x));

      switch (A)
      {
        case SparseDoubleMatrix sm:
          SolveDestructive(sm, b, x);
          break;

        case BandDoubleMatrix bm:
          SolveDestructiveBanded(bm.InternalData, bm.LowerBandwidth, bm.UpperBandwidth, b, x);
          break;

        case DoubleMatrix dm:
          SolveDestructive(dm.InternalData, b, x);
          break;

        default:
          Solve(A, b, x.ToVector());
          break;
      }
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.
    /// Attention! Both matrix A and vector b are destroyed (changed).</summary>
    /// <param name="A">Elements of matrix 'A'. This array is modified!</param>
    /// <param name="b">Right part 'b'. This array is also modified!</param>
    /// <param name="x">Vector to store the result, i.e. the solution to the problem a x = b.</param>
    public void SolveDestructive(MatrixWrapperStructForLeftSpineJaggedArray<double> A, double[] b, double[] x)
    {
      var a = A.Array;
      if (a == null)
        throw new ArgumentNullException(nameof(A));
      if (b == null)
        throw new ArgumentNullException(nameof(b));
      if (x == null)
        throw new ArgumentException(nameof(x));

      int n = A.RowCount;

      for (int j = 0; j < n; ++j)
      {
        // Find row with largest absolute value of j-st element
        int maxIdx = 0;
        for (int i = 0; i < n - j; ++i)
        {
          if (Math.Abs(a[i][j]) > Math.Abs(a[maxIdx][j]))
          {
            maxIdx = i;
          }
        }

        // Divide this row by max value
        for (int i = j + 1; i < n; ++i)
        {
          a[maxIdx][i] /= a[maxIdx][j];
        }

        b[maxIdx] /= a[maxIdx][j];
        a[maxIdx][j] = 1;

        // Move this row to bottom
        if (maxIdx != n - j - 1)
        {
          //SwapRow(A, b, n - j - 1, maxIdx);

          var temp = a[n - j - 1];
          a[n - j - 1] = a[maxIdx];
          a[maxIdx] = temp;

          var temp3 = b[n - j - 1];
          b[n - j - 1] = b[maxIdx];
          b[maxIdx] = temp3;
        }

        var an = a[n - j - 1];
        // Process all other rows
        for (int i = 0; i < n - j - 1; ++i)
        {
          var aa = a[i];
          if (aa[j] != 0)
          {
            for (int k = j + 1; k < n; ++k)
            {
              aa[k] -= aa[j] * an[k];
            }
            b[i] -= aa[j] * b[n - j - 1];
            aa[j] = 0;
          }
        }
      }

      // Build answer
      for (int i = 0; i < n; ++i)
      {
        double s = b[i];
        for (int j = n - i; j < n; ++j)
          s -= x[j] * a[i][j];
        x[n - i - 1] = s;
      }
    }

    private void SwapRow(IMatrix<double> A, IVector<double> b, int i, int j)
    {
      var cols = A.ColumnCount;
      for (int k = 0; k < cols; ++k)
      {
        var A_i = A[i, k];
        A[i, k] = A[j, k];
        A[j, k] = A_i;
      }
      var b_i = b[i];
      b[i] = b[j];
      b[j] = b_i;
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.
    /// Attention! Both matrix a and vector b are destroyed (changed).</summary>
    /// <param name="A">Elements of matrix 'A'. The matrix is modified in this call!</param>
    /// <param name="b">Right part 'b'. The vector is modified in this call!</param>
    /// <param name="x">Vector to store the result, i.e. the solution to the problem A*x = b.</param>
    public void SolveDestructive(IMatrix<double> A, IVector<double> b, IVector<double> x)
    {
      var a = A;
      if (a == null)
        throw new ArgumentNullException(nameof(A));
      if (b == null)
        throw new ArgumentNullException(nameof(b));
      if (x == null)
        throw new ArgumentException(nameof(x));

      int n = A.RowCount;

      for (int j = 0; j < n; ++j)
      {
        // Find row with largest absolute value of j-st element
        int maxIdx = 0;
        var maxV = Math.Abs(a[maxIdx, j]);
        var tempV = maxV;
        for (int i = 1; i < n - j; ++i)
        {
          if ((tempV = Math.Abs(a[i, j])) > maxV)
          {
            maxV = tempV;
            maxIdx = i;
          }
        }

        maxV = a[maxIdx, j]; // now without absolute value

        // Divide this row by max value
        for (int i = j + 1; i < n; ++i)
        {
          a[maxIdx, i] /= maxV;
        }

        b[maxIdx] /= maxV;
        a[maxIdx, j] = 1;

        // Move this row to bottom
        if (maxIdx != n - j - 1)
        {
          SwapRow(a, b, n - j - 1, maxIdx);
        }

        var nj1 = n - j - 1;
        // Process all other rows
        for (int i = 0; i < n - j - 1; ++i)
        {
          if (a[i, j] != 0)
          {
            for (int k = j + 1; k < n; ++k)
            {
              a[i, k] -= a[i, j] * a[nj1, k];
            }
            b[i] -= a[i, j] * b[n - j - 1];
            a[i, j] = 0;
          }
        }
      }

      // Build answer
      for (int i = 0; i < n; ++i)
      {
        double s = b[i];
        for (int j = n - i; j < n; ++j)
          s -= x[j] * a[i, j];
        x[n - i - 1] = s;
      }
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.</summary>
    /// <param name="A">Elements of matrix 'A'. This array is modified during solution!</param>
    /// <param name="b">Right part 'b'. This array is also modified during solution!</param>
    /// <param name="x">Vector to store the result, i.e. the solution to the problem a x = b.</param>
    public void Solve(IROMatrix<double> A, IReadOnlyList<double> b, IVector<double> x)
    {
      if (_temp_A.RowCount != A.RowCount || _temp_A.ColumnCount != A.ColumnCount)
        _temp_A = new MatrixWrapperStructForLeftSpineJaggedArray<double>(A.RowCount, A.ColumnCount);
      if (b.Count != _temp_b?.Length)
        _temp_b = new double[b.Count];
      if (b.Count != _temp_x?.Length)
        _temp_x = new double[b.Count];

      MatrixMath.Copy(A, _temp_A);
      VectorMath.Copy(b, _temp_b);
      SolveDestructive(_temp_A, _temp_b, _temp_x);
      VectorMath.Copy(_temp_x, x);
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.</summary>
    /// <param name="A">Elements of matrix 'A'.</param>
    /// <param name="b">Right part 'b'</param>
    /// <param name="vectorCreation">Function to create the resulting vector. Argument is the length of the vector.</param>
    public VectorT Solve<VectorT>(IROMatrix<double> A, IReadOnlyList<double> b, Func<int, VectorT> vectorCreation) where VectorT : IVector<double>
    {
      var x = vectorCreation(b.Count);
      Solve(A, b, x);
      return x;
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.</summary>
    /// <param name="A">Elements of matrix 'A'. This matrix is modified during solution!</param>
    /// <param name="b">Right part 'b'. This vector is also modified during solution!</param>
    /// <param name="vectorCreation">Function to create the resulting vector. Argument is the length of the vector.</param>
    public VectorT SolveDestructive<VectorT>(IMatrix<double> A, IVector<double> b, Func<int, VectorT> vectorCreation) where VectorT : IVector<double>
    {
      var x = vectorCreation(b.Count);
      SolveDestructive(A, b, x);
      return x;
    }

    #region Sparse

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="A">Sparse matrix, 'A'. This matrix is modified during solution!</param>
    /// <param name="b">Right part, 'b', is modified during solution.</param>
    /// <returns>Vector x with the result.</returns>
    public double[] SolveDestructive(SparseDoubleMatrix A, double[] b)
    {
      var x = new double[b.Length];
      SolveDestructive(A, b, x);
      return x;
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="A">Sparse matrix, 'A'. This matrix is modified during solution!</param>
    /// <param name="b">Right part, 'b', is modified during solution.</param>
    /// <param name="x">Vector to store the solution.</param>
    public void SolveDestructive(SparseDoubleMatrix A, double[] b, double[] x)
    {
      if (A == null)
        throw new ArgumentNullException(nameof(A));
      if (b == null)
        throw new ArgumentNullException(nameof(b));
      if (x == null)
        throw new ArgumentNullException(nameof(x));
      int n = A.RowCount;
      if (!(n == b.Length))
        throw new RankException("Mismatch between number of rows of the matrix A and length of vector b");
      if (!(A.ColumnCount == x.Length))
        throw new RankException("Mismatch between number of columns of the matrix A and length of vector x");

      for (int j = 0; j < n; j++)
      {
        // Find row with largest absolute value of j-st element
        int maxIdx = 0;
        double maxVal = A[maxIdx, j];
        double Aij = 0.0;
        for (int i = 0; i < n - j; i++)
        {
          Aij = A[i, j];
          if (Math.Abs(Aij) > Math.Abs(maxVal))
          {
            maxIdx = i;
            maxVal = Aij;
          }
        }

        if (Math.Abs(maxVal) < 1e-12)
          throw new InvalidOperationException("Cannot apply Gauss method");

        // Divide this row by max value
        A.ScaleRow(maxIdx, j + 1, n - 1, 1 / maxVal);
        b[maxIdx] /= maxVal;
        A[maxIdx, j] = 1.0;

        // Move this row to bottom
        if (maxIdx != n - j - 1)
        {
          A.SwitchRows(maxIdx, n - j - 1);

          var temp3 = b[n - j - 1];
          b[n - j - 1] = b[maxIdx];
          b[maxIdx] = temp3;
        }

        // Process all other rows
        for (int i = 0; i < n - j - 1; i++)
        {
          Aij = A[i, j];
          if (Aij != 0)
          {
            var indices = A.GetIndicesOfRow(n - j - 1);
            foreach (int k in indices)
            {
              if (k > j)
                A[i, k] -= Aij * A[n - j - 1, k];
            }
            b[i] -= Aij * b[n - j - 1];
            A[i, j] = 0;
          }
        }
      }

      // Build answer
      for (int i = 0; i < n; i++)
      {
        double s = b[i];
        var Ai = A.GetRow(i);
        for (int k = 0; k < Ai.count; k++)
          if (Ai.indices[k] >= n - i)
            s -= x[Ai.indices[k]] * A.GetRow(i).items[k];
        x[n - i - 1] = s;
      }
    }

    #endregion Sparse

    #region Band matrix

    /// <summary>Solves a system of linear equations Ax = b with a band matrix A, using Gaussian elimination with partial pivoting.
    /// Attention! Both matrix A and vector b are destroyed (changed).</summary>
    /// <param name="A">Elements of matrix 'A'. The matrix is modified!</param>
    /// <param name="lowerBandwidth">Lower band width of the matrix. It is not checked whether the matrix contains non-zero elements outside of the band!</param>
    /// <param name="upperBandwidth">Upper band width of the matrix. It is not checked whether the matrix contains non-zero elements outside of the band!</param>
    /// <param name="b">Right part 'b'. This array is also modified!</param>
    /// <param name="x">Vector to store the result, i.e. the solution to the problem a x = b.</param>
    public void SolveDestructiveBanded(MatrixWrapperStructForLeftSpineJaggedArray<double> A, int lowerBandwidth, int upperBandwidth, double[] b, double[] x)
    {
      var a = A.Array;
      if (a == null)
        throw new ArgumentNullException(nameof(A));
      if (b == null)
        throw new ArgumentNullException(nameof(b));
      if (x == null)
        throw new ArgumentException(nameof(x));

      int n = A.RowCount;

      // Start of algorithm 4.81, page 184, book of Engeln-Müllges, Numerik-Algorithmen, 10th ed.
      // note ml in the book is lowerBandwidth here, mr in the book is upperBandwidth here
      for (int j = 0; j < n - 1; ++j)
      {
        // Find row with largest absolute value of j-st element
        int maxIdx = j;
        double max_abs_aij = Math.Abs(a[j][j]);
        int i_end = Math.Min(n, j + lowerBandwidth + 1);
        for (int i = j + 1; i < i_end; ++i)
        {
          var abs_aij = Math.Abs(a[i][j]);
          if (abs_aij > max_abs_aij)
          {
            maxIdx = i;
            max_abs_aij = abs_aij;
          }
        }

        if (maxIdx != j) // switch rows
        {
          var aj = a[j];
          a[j] = a[maxIdx];
          a[maxIdx] = aj;

          var bj = b[j];
          b[j] = b[maxIdx];
          b[maxIdx] = bj;
        }

        var ajj = a[j][j];
        if (0 == ajj)
          throw new SingularMatrixException("Matrix is singular");

        i_end = Math.Min(n, j + lowerBandwidth + 1);
        for (int i = j + 1; i < i_end; ++i)
        {
          var aij = (a[i][j] /= ajj); // Element of L (left matrix)
          int k_end = Math.Min(n, j + lowerBandwidth + upperBandwidth + 1);
          for (int k = j + 1; k < k_end; ++k)
            a[i][k] -= a[j][k] * aij;
          b[i] -= b[j] * aij;
        }
      }

      // now we have an L-R matrix

      // back substitution from bottom to top, using the R matrix
      // we use the fact that coefficients can not be more away from the diagonal than lowerBandwidth+upperBandwidth
      for (int i = n - 1; i >= 0; --i)
      {
        var bi = b[i];
        int k_end = Math.Min(n, i + lowerBandwidth + upperBandwidth + 1);
        for (int k = i + 1; k < k_end; ++k)
          bi -= a[i][k] * x[k];
        x[i] = bi / a[i][i];
      }
    }

    #endregion Band matrix
  }
}
