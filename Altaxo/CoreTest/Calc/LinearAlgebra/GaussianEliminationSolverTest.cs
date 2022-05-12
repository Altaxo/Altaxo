#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra
{
  public class GaussianEliminationSolverTest
  {
    private GaussianEliminationSolver solver = new GaussianEliminationSolver();

    [Fact]
    public void SolverCoreTest3d()
    {
      var a = new double[][] {
                new double[] {  2, 1,-1 },
                new double[] { -3,-1, 2 },
                new double[] { -2, 1, 2 }
            };

      var b = new double[] { 8, -11, -3 };
      var x = new double[3];
      solver.SolveDestructive(new MatrixWrapperStructForLeftSpineJaggedArray<double>(a, 3, 3), b, x);
      var answer = new DoubleVector(3) { [0] = 2, [1] = 3, [2] = -1 };
      Assert.True(VectorMath.LInfinityNorm(x, answer) < 1e-10);
    }

    [Fact]
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
      Assert.True(VectorMath.LInfinityNorm(x, answer) < 1e-10);
    }

    [Fact]
    public void SolverCoreTestMatrixNd()
    {
      const int N = 50;
      var a = new DoubleMatrix(N, N);
      // Make matrix diagonal
      for (int i = 0; i < N; i++)
        a[i, i] = 1;
      // Apply random rotations around each pair of axes. This will keep det(A) ~ 1
      var rand = new Random();
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
      var sw = new Stopwatch();
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
      Assert.True(VectorMath.LInfinityNorm(b, b2) < 1e-6);
    }

    #region Banded matrix

    [Fact]
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
                AssertEx.Equal(x[i], xr[i], 1E-6);
              }
            }
          }
        }
      }
    }

    #endregion Banded matrix

    #region Tri diagonal matrix
    [Fact]
    public void TestTridiagonal()
    {
      var rnd = new Random(17);


      for (int N = 3; N < 10; ++N)
      {
        DoubleMatrix A = new DoubleMatrix(N, N);
        var x = new double[N];
        var a = new double[N];
        var xx = new double[N];



        for (int i = 0; i < N; ++i)
        {
          A[i, i] = rnd.NextDouble();
          if (i > 0)
          {
            A[i, i - 1] = rnd.NextDouble();
            A[i - 1, i] = rnd.NextDouble();
          }

          x[i] = rnd.NextDouble();
        }


        MatrixMath.Multiply(A, VectorMath.ToVector(x), VectorMath.ToVector(a));

        for (int i = 0; i < N; ++i)
          for (int j = 0; j < N; ++j)
            if (Math.Abs(i - j) > 1)
              A[i, j] = double.NaN;

        object tempStorage = new Tuple<double[], double[], double[]>(
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray()
          );
        GaussianEliminationSolver.SolveTriDiagonal(A, a, xx, ref tempStorage);

        for (int i = 0; i < x.Length; ++i)
          AssertEx.AreEqual(x[i], xx[i], 1E-6, 1e-6);
      }
    }

    #endregion

    #region Five diagonal matrix
    [Fact]
    public void TestPentaDiagonal()
    {
      var rnd = new Random(17);


      for (int N = 5; N < 10; ++N)
      {
        DoubleMatrix A = new DoubleMatrix(N, N);
        var x = new double[N];
        var a = new double[N];
        var xx = new double[N];



        for (int i = 0; i < N; ++i)
        {
          A[i, i] = rnd.NextDouble();
          if (i > 0)
          {
            A[i, i - 1] = rnd.NextDouble();
            A[i - 1, i] = rnd.NextDouble();
          }
          if (i > 1)
          {
            A[i, i - 2] = rnd.NextDouble();
            A[i - 2, i] = rnd.NextDouble();
          }

          x[i] = rnd.NextDouble();
        }


        MatrixMath.Multiply(A, VectorMath.ToVector(x), VectorMath.ToVector(a));

        for (int i = 0; i < N; ++i)
          for (int j = 0; j < N; ++j)
            if (Math.Abs(i - j) > 2)
              A[i, j] = double.NaN;


        object tempStorage = new Tuple<double[], double[], double[], double[], double[], double[]>(
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray()
          );
        GaussianEliminationSolver.SolvePentaDiagonal(A, a, xx);

        for (int i = 0; i < x.Length; ++i)
          AssertEx.AreEqual(x[i], xx[i], 1E-6, 1e-6);
      }
    }

    [Fact]
    public void TestPentaDiagonalWithBandMatrix()
    {
      var rnd = new Random(17);

      for (int N = 5; N < 10; ++N)
      {
        var A = new DoubleBandMatrix(N, N, 2, 2);
        var x = new double[N];
        var a = new double[N];
        var xx = new double[N];



        for (int i = 0; i < N; ++i)
        {
          A[i, i] = rnd.NextDouble();
          if (i > 0)
          {
            A[i, i - 1] = rnd.NextDouble();
            A[i - 1, i] = rnd.NextDouble();
          }
          if (i > 1)
          {
            A[i, i - 2] = rnd.NextDouble();
            A[i - 2, i] = rnd.NextDouble();
          }

          x[i] = rnd.NextDouble();
        }


        MatrixMath.Multiply(A, VectorMath.ToVector(x), VectorMath.ToVector(a));

        object tempStorage = new Tuple<double[], double[], double[], double[], double[], double[]>(
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray(),
          Enumerable.Repeat(double.NaN, N).ToArray()
          );
        GaussianEliminationSolver.SolvePentaDiagonal(A, a, xx);

        for (int i = 0; i < x.Length; ++i)
          AssertEx.AreEqual(x[i], xx[i], 1E-6, 1e-6);
      }
    }

    [Fact]
    public void TestFiveDiagonalBookExample()
    {
      var A = new DoubleMatrix(6, 6);

      var a = new double[6] { -2, -4, 3, -7, -1, 4 };
      var xx = new double[6];

      A[0, 0] = 2; A[0, 1] = -2; A[0, 2] = -2; A[0, 3] = 0; A[0, 4] = 0; A[0, 5] = 0;
      A[1, 0] = -2; A[1, 1] = 5; A[1, 2] = -4; A[1, 3] = -3; A[1, 4] = 0; A[1, 5] = 0;
      A[2, 0] = -1; A[2, 1] = -2; A[2, 2] = 11; A[2, 3] = -1; A[2, 4] = -4; A[2, 5] = 0;
      A[3, 0] = 0; A[3, 1] = -1; A[3, 2] = 1; A[3, 3] = 7; A[3, 4] = -4; A[3, 5] = -10;
      A[4, 0] = 0; A[4, 1] = 0; A[4, 2] = -1; A[4, 3] = -1; A[4, 4] = 9; A[4, 5] = -8;
      A[5, 0] = 0; A[5, 1] = 0; A[5, 2] = 0; A[5, 3] = -1; A[5, 4] = 0; A[5, 5] = 5;


      GaussianEliminationSolver.SolvePentaDiagonal(A, a, xx);

      for (int i = 0; i < 6; i++)
      {
        AssertEx.AreEqual(1, xx[i], 1e-6, 1e-6);
      }

    }

    #endregion

  }
}
