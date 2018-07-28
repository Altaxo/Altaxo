#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  [TestFixture]
  public class GaussianEliminationSolverTest
  {
    private GaussianEliminationSolver solver = new GaussianEliminationSolver();

    [Test]
    public void SolverCoreTest3d()
    {
      var a = new double[][] {
                new double[] { 2,1,-1 },
                new double[] { -3,-1,2 },
                new double[] { -2,1,2 }
            };

      var b = new double[] { 8, -11, -3 };
      var x = new double[3];
      solver.SolveDestructive(new MatrixWrapperStructForLeftSpineJaggedArray<double>(a, 3, 3), b, x);
      var answer = new DoubleVector(3) { [0] = 2, [1] = 3, [2] = -1 };
      Assert.IsTrue(VectorMath.LInfinityNorm(x, answer) < 1e-10);
    }

    [Test]
    public void SolverCoreTest2d()
    {
      var a = new double[][] {
                new double[] { 2,1 },
                new double[] { -1,1 }
            };
      var b = new double[] { 1, -2 };
      var x = new double[2];
      solver.SolveDestructive(new MatrixWrapperStructForLeftSpineJaggedArray<double>(a, 2, 2), b, x);
      var answer = new double[] { 1, -1 };
      Assert.IsTrue(VectorMath.LInfinityNorm(x, answer) < 1e-10);
    }

    [Test]
    public void SolverCoreTestMatrixNd()
    {
      const int N = 50;
      var a = new DoubleMatrix(N, N);
      // Make matrix diagonal
      for (int i = 0; i < N; i++)
        a[i, i] = 1;
      // Apply random rotations around each pair of axes. This will keep det(A) ~ 1
      Random rand = new Random();
      for (int i = 0; i < N; i++)
        for (int j = i + 1; j < N; j++)
        {
          double angle = rand.NextDouble() * 2 * Math.PI;
          var r = new DoubleMatrix(N, N);
          for (int k = 0; k < N; k++)
            r[k, k] = 1;
          r[i, i] = r[j, j] = Math.Cos(angle);
          r[i, j] = Math.Sin(angle);
          r[j, i] = -Math.Sin(angle);
          a = a * r;
        }

      // Generate random vector
      var b = DoubleVector.Zeros(N);
      for (int i = 0; i < N; i++)
        b[i] = rand.NextDouble();

      // Solve system
      Stopwatch sw = new Stopwatch();
      sw.Start();
      var x = solver.Solve(a, b, (len) => new DoubleVector(len));
      sw.Stop();
      Trace.WriteLine("Gaussian elimination took: " + sw.ElapsedTicks);

      // Solve system
      sw.Start();
      var x2 = solver.Solve(a, b, (l) => new DoubleVector(l));
      sw.Stop();
      Trace.WriteLine("Jama solve elimination took: " + sw.ElapsedTicks);

      Trace.WriteLine("Difference is " + VectorMath.LInfinityNorm(x, x2));

      // Put solution into system
      var b2 = a * x;

      // Verify result is the same
      Assert.IsTrue(VectorMath.LInfinityNorm(b, b2) < 1e-6);
    }

    #region Banded matrix

    [Test]
    public void TextBanded01()
    {
      var rnd = new Random(642332);

      for (int testRuns = 0; testRuns < 1000; ++testRuns)
      {
        for (int N = 11; N <= 13; ++N) // for even and odd matrix dimensions
        {
          for (int lowerBandwidth = 0; lowerBandwidth <= (N + 1) / 2; ++lowerBandwidth) // for all lower bandwidths
          {
            for (int upperBandwidth = 0; upperBandwidth <= (N + 1) / 2; ++upperBandwidth) // for all upper bandwidths
            {
              var A = new DoubleMatrix(N, N);

              for (int i = 0; i < N; ++i)
              {
                int start = Math.Max(0, i - lowerBandwidth);
                int end = Math.Min(N, i + upperBandwidth + 1);

                for (int j = start; j < end; ++j)
                  A[i, j] = rnd.Next(1, 99);
              }
              var x = new DoubleVector(N);

              for (int i = 0; i < N; ++i)
                x[i] = rnd.Next(-99, 99);

              var b = A * x;

              // now do gaussian elimination

              var solver = new GaussianEliminationSolver();

              var xr = new double[N];

              solver.SolveDestructiveBanded(A, lowerBandwidth, upperBandwidth, b.GetInternalData(), xr);

              // compare result
              for (int i = 0; i < N; ++i)
              {
                Assert.AreEqual(x[i], xr[i], 1E-6);
              }
            }
          }
        }
      }
    }

    #endregion Banded matrix
  }
}
