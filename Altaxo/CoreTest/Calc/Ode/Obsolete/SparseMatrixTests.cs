#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Diagnostics;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class SparseMatrixTests
  {
    private const double Eps = 1e-10;

    [Fact]
    public void plusTest()
    {
      const int N = 50;
      var A = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        A[i, i] = i % 5 == 0 ? 1.0 : 0.0;
      var A1 = A.Copy();
      var Zeros = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        Zeros[i, i] = 0.0;

      var B = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        B[i, i] = i % 5 == 0 ? -1.0 : 0.0;

      var C = A.plus(B);
      AssertMatrixEqualsEps(C, Zeros);
      C = A1 + B;
      AssertMatrixEqualsEps(C, Zeros);
    }

    [Fact]
    public void minusTest()
    {
      const int N = 50;
      var A = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        A[i, i] = i % 5 == 0 ? 1.0 : 0.0;
      var A1 = A.Copy();
      var Zeros = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        Zeros[i, i] = 0.0;

      var B = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        B[i, i] = i % 5 == 0 ? 1.0 : 0.0;

      var C = A.minus(B);
      AssertMatrixEqualsEps(C, Zeros);
      C = A - B;
      AssertMatrixEqualsEps(C, Zeros);
    }

    [Fact]
    public void isLowerTriangularTest()
    {
      const int N = 50;
      var A = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        A[i, i] = i % 5 == 0 ? 1.0 : 0.0;

      Assert.True(A.IsLowerTriangular());
      A[45, 40] = 1.0;
      Assert.True(A.IsLowerTriangular());
      A[40, 45] = 1.0;
      Assert.False(A.IsLowerTriangular());
    }

    [Fact]
    public void TimesEqualsTest()
    {
      const int N = 50;
      var A = SparseMatrix.Identity(N, N);
      for (int i = 0; i < N; i++)
        A[i, i] = i % 5 == 0 ? 1.0 : 0.0;

      SparseMatrix AInit = A.Copy();
      SparseMatrix B = A.Copy();
      for (int i = 0; i < N; i++)
        B[i, i] = i % 5 == 0 ? 2.0 : 0.0;
      var C = A.Mul(2.0);
      AssertMatrixEqualsEps(B, C);
      var D = AInit * 2.0;
      AssertMatrixEqualsEps(B, D);
    }

    [Fact]
    public void TimesTest()
    {
      const int N = 50;
      const int M = 30;
      var A = SparseMatrix.Identity(M, N);
      for (int i = 0; i < M; i++)
        if (i < N)
          A[i, i] = i % 5 == 0 ? 1.0 : 0.0;

      var b = Vector.Zeros(N);
      for (int i = 0; i < N; i++)
        b[i] = 2.0;

      var B = Vector.Zeros(M);
      for (int i = 0; i < M; i++)
        B[i] = i % 5 == 0 ? 2.0 : 0.0;

      var C = A.times(b);
      AssertVectorEqualsEps(B, C);
    }

    [Fact]
    public void SolveGETest()
    {
      const int N = 50;
      var a = new SparseMatrix(N, N);
      for (int i = 0; i < N; i++)
        a[i, i] = 1;
      // Apply random rotations around each pair of axes. This will keep det(A) ~ 1
      var rand = new Random();
      for (int i = 0; i < N; i++)
        for (int j = i + 1; j < N; j++)
        {
          double angle = rand.NextDouble() * 2 * Math.PI;
          var r = new SparseMatrix(N, N);
          for (int k = 0; k < N; k++)
            r[k, k] = 1;
          r[i, i] = r[j, j] = Math.Cos(angle);
          r[i, j] = Math.Sin(angle);
          r[j, i] = -Math.Sin(angle);
          a = a * r;
        }

      var ainit = a.Copy();
      // Generate random vector
      var b = Vector.Zeros(N);
      for (int i = 0; i < N; i++)
        b[i] = rand.NextDouble();

      var binit = b.Clone();
      // Solve system
      var sw = new Stopwatch();
      sw.Start();
      Vector x = a.SolveGE(b);
      sw.Stop();
      Trace.WriteLine("Gaussian elimination took: " + sw.ElapsedTicks);
      // Put solution into system
      Vector b2 = ainit * x;

      // Verify result is the same
      Assert.True(Vector.GetLInfinityNorm(binit, b2) < 1e-6);
    }

    private void AssertVectorEqualsEps(Vector A, Vector B)
    {
      double sum = 0.0;
      for (int i = 0; i < A.Length; i++)
      {
        sum += A[i] - B[i];
      }

      AssertEx.Equal(sum, 0.0, 1e-10);
    }

    private void AssertEqualsEps(double a, double b)
    {
      Assert.True(Math.Abs(a - b) < 1e-10);
    }

    private void AssertMatrixEqualsEps(SparseMatrix A, SparseMatrix B)
    {
      double sum = 0.0;
      for (int i = 0; i < A.RowDimension; i++)
        for (int j = 0; j < A.ColumnDimension; j++)
          sum += A[i, j] - B[i, j];

      AssertEqualsEps(sum, 0.0);
    }
  }
}
