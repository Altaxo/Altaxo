#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class MatrixTests
  {
    [Fact]
    public void SubmatrixTest()
    {
      var A = new Matrix(new double[][] { new double [] {1.0, 2.0, 3.0},
                                                    new double [] {4.0, 5.0, 6.0},
                                                    new double [] {7.0, 8.0, 9.0}});

      var B = A.Submatrix(1, 2, 1, 2);
      AssertMatrixEqualsEps(B, new Matrix(new double[][] { new double [] {5.0, 6.0},
                                                                 new double [] {8.0, 9.0}}));
      var C = A.Submatrix(new int[] { 0, 2 }, new int[] { 0, 2 });
      AssertMatrixEqualsEps(C, new Matrix(new double[][] { new double [] {1.0, 3.0},
                                                                 new double [] {7.0, 9.0}}));

      var D = A.Submatrix(new int[] { 1 }, 0, 2);
      AssertMatrixEqualsEps(D, new Matrix(new double[][] { new double[] { 4.0, 5.0, 6.0 } }));
    }

    [Fact]
    public void MultiplyTest()
    {
      var A = new Matrix(new double[][] { new double[] { 1.0, 0.25 }, new double[] { 0.25, 1.0 } });
      var AMult = new Matrix(new double[][] { new double[] { 2.0, 0.5 }, new double[] { 0.5, 2.0 } });
      AssertMatrixEqualsEps(A.Mul(2.0), AMult);
      A = new Matrix(new double[][] { new double[] { 1.0, 0.25 }, new double[] { 0.25, 1.0 } });
      AssertMatrixEqualsEps(A * 2.0, AMult);
      AssertMatrixEqualsEps(2.0 * A, AMult);
      AssertMatrixEqualsEps(A.times(2.0), AMult);
    }

    [Fact]
    public void CholeskyTest()
    {
      var A = new Matrix(new double[][] { new double[] { 1.0, 0.25 }, new double[] { 0.25, 1.0 } });
      var L = A.Cholesky();
      var Lt = L.Transpose();
      var diff = A - L * Lt;

      AssertMatrixEqualsEps(A, L * Lt);
    }

    [Fact]
    public void CholeskySolveTest()
    {
      var A = new Matrix(new double[][] { new double[] { 1.0, 0.25 }, new double[] { 0.25, 1.0 } });
      var L = A.Cholesky();
      var Lt = L.Transpose();
      var b = new Vector(new double[] { 1, 2 });
      Vector x = A.SolveGE(b);
      var y1 = L.SolveLower(b);
      Vector result = Lt.SolveUpper(y1);

      AssertEqualsEps((result - x).EuclideanNorm, 0.0);
    }

    [Fact]
    public void GaussianEliminationTest()
    {
      var A = new Matrix(new double[][] { new double[] { -1.0, 0.5 }, new double[] { 1.0, -1.0 } });
      var b = new Vector(new double[] { 1, 2 });
      var x = new Vector(new double[] { -4, -6 });

      AssertEqualsEps((A.SolveGE(b) - x).EuclideanNorm, 0.0);
    }

    [Fact]
    public void InverseLowerTest()
    {
      var A = new Matrix(new double[][] { new double[] { -1.0, 0.5, 0.0 }, new double[] { -1.0, 1.0, -1.0 }, new double[] { -2.0, -1.0, 1.0 } });
      A = A * A.Transpose();
      var L = A.Cholesky();
      var iL = L.InverseLower();
      var result = L * iL;
    }

    [Fact]
    // Here we compare the dense matrix and sparse matrix usages of solveLower
    public void solveLowerTest()
    {
      var row = new double[] { 1.0, -1.0 };
      var items = new double[][] { new double[] { -1.0 }, row, row, row, new double[] { 1.0, -1.0 } };
      var indices = new int[][] { new int[] { 0 }, new int[] { 0, 1 }, new int[] { 1, 2 }, new int[] { 2, 3 }, new int[] { 3, 4 } };
      var count = new int[] { 1, 2, 2, 2, 2 };
      var As = new SparseMatrix(5, 5, items, indices, count);
      var b = new Vector(new double[] { 1, 2, 0, -3, 4 });
      var xs = As.SolveLower(b);

      var Ad = As.DenseMatrix();
      var xd = Ad.SolveLower(b);
      AssertEqualsEps((xs - xd).EuclideanNorm, 0.0);
    }

    [Fact]
    public void SparseMatrixMultiplicationTest()
    {
      var v = new Vector(new double[] { 0.0975, 0.2785, 0.5469, 0.9575, 0.9649 });
      var row = new double[] { 1.0, -1.0, 1.0 };
      var items = new double[][] { new double[] { -1.0, 1.0 }, row, row, row, new double[] { 1.0, -1.0 } };
      var indices = new int[][] { new int[] { 0, 1 }, new int[] { 0, 1, 2 }, new int[] { 1, 2, 3 }, new int[] { 2, 3, 4 }, new int[] { 3, 4 } };
      var count = new int[] { 2, 3, 3, 3, 2 };
      var A = new SparseMatrix(5, 5, items, indices, count);

      var Av = A * v;
      var vA = v * A;

      AssertEqualsEps(Av.EuclideanNorm, vA.EuclideanNorm);
    }

    private void AssertEqualsEps(double a, double b)
    {
      Assert.True(Math.Abs(a - b) < 1e-10);
    }

    private void AssertMatrixEqualsEps(Matrix A, Matrix B)
    {
      double sum = 0.0;
      for (int i = 0; i < A.RowDimension; i++)
        for (int j = 0; j < A.ColumnDimension; j++)
          sum += A[i, j] - B[i, j];

      AssertEqualsEps(sum, 0.0);
    }
  }
}
