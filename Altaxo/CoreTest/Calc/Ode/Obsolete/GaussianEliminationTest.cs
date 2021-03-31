#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class GaussianEliminationTest
  {
    [Fact]
    public void SolverCoreTest3d()
    {
      var a = new double[][] {
                new double[] { 2,1,-1 },
                new double[] { -3,-1,2 },
                new double[] { -2,1,2 }
            };
      var b = new Vector(8, -11, -3);
      var x = Gauss.SolveCore(a, b);
      var answer = new Vector(2, 3, -1);
      Assert.True(Vector.GetLInfinityNorm(x, answer) < 1e-10);
    }

    [Fact]
    public void SolverCoreTest2d()
    {
      var a = new double[][] {
                new double[] { 2,1 },
                new double[] { -1,1 }
            };
      var b = new Vector(1, -2);
      var x = Gauss.SolveCore(a, b);
      var answer = new Vector(1, -1);
      Assert.True(Vector.GetLInfinityNorm(x, answer) < 1e-10);
    }

    [Fact]
    public void SolverCoreTestMatrixNd()
    {
      const int N = 50;
      var a = new Matrix(N, N);
      // Make matrix diagonal
      for (int i = 0; i < N; i++)
        a[i, i] = 1;
      // Apply random rotations around each pair of axes. This will keep det(A) ~ 1
      var rand = new Random();
      for (int i = 0; i < N; i++)
        for (int j = i + 1; j < N; j++)
        {
          double angle = rand.NextDouble() * 2 * Math.PI;
          var r = new Matrix(N, N);
          for (int k = 0; k < N; k++)
            r[k, k] = 1;
          r[i, i] = r[j, j] = Math.Cos(angle);
          r[i, j] = Math.Sin(angle);
          r[j, i] = -Math.Sin(angle);
          a = a * r;
        }

      // Generate random vector
      var b = Vector.Zeros(N);
      for (int i = 0; i < N; i++)
        b[i] = rand.NextDouble();

      // Solve system
      var sw = new Stopwatch();
      sw.Start();
      Vector x = Gauss.Solve(a, b);
      sw.Stop();
      Trace.WriteLine("Gaussian elimination took: " + sw.ElapsedTicks);

      // Solve system
      sw.Start();
      Vector x2 = a.SolveGE(b);
      sw.Stop();
      Trace.WriteLine("Jama solve elimination took: " + sw.ElapsedTicks);

      Trace.WriteLine("Difference is " + Vector.GetLInfinityNorm(x, x2));

      // Put solution into system
      Vector b2 = a * x;

      // Verify result is the same
      Assert.True(Vector.GetLInfinityNorm(b, b2) < 1e-6);
    }

    [Fact]
    public void SolverCoreTestSparseMatrixNd()
    {
      const int N = 50;
      var a = new SparseMatrix(N, N);
      // Make matrix diagonal
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
      var x = new Vector(Gauss.Solve(a, b));
      sw.Stop();
      Trace.WriteLine("Gaussian elimination took: " + sw.ElapsedTicks);
      // Put solution into system
      Vector b2 = ainit * x;

      // Verify result is the same
      Assert.True(Vector.GetLInfinityNorm(binit, b2) < 1e-6);
    }
  }
}
