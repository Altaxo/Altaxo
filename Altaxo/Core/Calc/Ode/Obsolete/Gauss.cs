#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Linq;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>Provides implementation of Gaussian elimination with partial pivoting</summary>
  public static class Gauss
  {
    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="a">Elements of matrix 'A'. This array is modified during solution!</param>
    /// <param name="b">Right part 'b'. This array is also modified during solution!</param>
    /// <returns>Solution of system 'x'</returns>
    public static double[] SolveCore(double[][] a, double[] b)
    {
      if (a is null)
        throw new ArgumentNullException("a");
      if (b is null)
        throw new ArgumentNullException("b");
      int n = a.Length;
      int[] map = Enumerable.Range(0, n).ToArray();
      var x = Vector.Zeros(n);
      for (int j = 0; j < n; j++)
      {
        // Find row with largest absolute value of j-st element
        int maxIdx = 0;
        for (int i = 0; i < n - j; i++)
          if (Math.Abs(a[i][j]) > Math.Abs(a[maxIdx][j]))
            maxIdx = i;

        if (Math.Abs(a[maxIdx][j]) < 1e-12)
          throw new InvalidOperationException("Cannot apply Gauss method");

        // Divide this row by max value
        for (int i = j + 1; i < n; i++)
          a[maxIdx][i] /= a[maxIdx][j];
        b[maxIdx] /= a[maxIdx][j];
        a[maxIdx][j] = 1.0;

        // Move this row to bottom
        if (maxIdx != n - j - 1)
        {
          var temp = a[n - j - 1];
          a[n - j - 1] = a[maxIdx];
          a[maxIdx] = temp;

          var temp3 = b[n - j - 1];
          b[n - j - 1] = b[maxIdx];
          b[maxIdx] = temp3;

          var temp2 = map[n - j - 1];
          map[n - j - 1] = map[maxIdx];
          map[maxIdx] = temp2;
        }

        double[] an = a[n - j - 1];
        // Process all other rows
        for (int i = 0; i < n - j - 1; i++)
        {
          double[] aa = a[i];
          if (aa[j] != 0)
          {
            for (int k = j + 1; k < n; k++)
              aa[k] -= aa[j] * an[k];
            b[i] -= aa[j] * b[n - j - 1];
            aa[j] = 0;
          }
        }
      }

      // Build answer
      for (int i = 0; i < n; i++)
      {
        double s = b[i];
        for (int j = n - i; j < n; j++)
          s -= x[j] * a[i][j];
        x[n - i - 1] = s;
      }
      return x;
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="A">Sparse matrix, 'A'. This array is modified during solution!</param>
    /// <param name="b">Right part, 'b'</param>
    /// <returns>Solution of system, 'x'</returns>
    public static double[] SolveCore(SparseMatrix A, double[] b)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if (b is null)
        throw new ArgumentNullException("b");
      int n = A.RowDimension;
      int[] map = Enumerable.Range(0, n).ToArray();
      double[] x = new double[n];
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

          var temp2 = map[n - j - 1];
          map[n - j - 1] = map[maxIdx];
          map[maxIdx] = temp2;
        }

        // Process all other rows
        for (int i = 0; i < n - j - 1; i++)
        {
          Aij = A[i, j];
          if (Aij != 0)
          {
            for (int k = j + 1; k < n; k++)
              A[i, k] -= Aij * A[n - j - 1, k];
            b[i] -= Aij * b[n - j - 1];
            A[i, j] = 0;
          }
        }
      }

      // Build answer
      for (int i = 0; i < n; i++)
      {
        double s = b[i];
        var Ai = A[i];
        for (int k = 0; k < Ai.count; k++)
          if (Ai.indices[k] >= n - i)
            s -= x[Ai.indices[k]] * A[i].items[k];
        x[n - i - 1] = s;
      }
      return x;
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="A">System matrix 'A'</param>
    /// <param name="b">Right part 'b'</param>
    /// <returns>Solution of system 'x'</returns>
    public static Vector Solve(Matrix A, Vector b)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if ((double[])b is null)
        throw new ArgumentNullException("b");
      return SolveCore((double[][])A.Clone(), b.Clone());
    }

    /// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="A">Sparse matrix, 'A'. This array is modified during solution!</param>
    /// <param name="b">Right part, 'b'</param>
    /// <returns>Solution of system, 'x'</returns>
    public static double[] Solve(SparseMatrix A, Vector b)
    {
      return SolveCore((double[][])A.DenseMatrix(), b);
    }
  }
}
