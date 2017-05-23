#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Altaxo.Calc.LinearAlgebra
{
	[TestFixture]
	public class SparseDoubleMatrixTests
	{
		private const double Eps = 1e-10;

		[Test]
		public void plusTest()
		{
			const int N = 50;
			SparseDoubleMatrix A = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				A[i, i] = i % 5 == 0 ? 1.0 : 0.0;
			var A1 = A.Clone();
			SparseDoubleMatrix Zeros = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				Zeros[i, i] = 0.0;

			SparseDoubleMatrix B = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				B[i, i] = i % 5 == 0 ? -1.0 : 0.0;

			var C = A.plus(B);
			AssertMatrixEqualsEps(C, Zeros);
			C = A1 + B;
			AssertMatrixEqualsEps(C, Zeros);
		}

		[Test]
		public void minusTest()
		{
			const int N = 50;
			SparseDoubleMatrix A = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				A[i, i] = i % 5 == 0 ? 1.0 : 0.0;
			var A1 = A.Clone();
			SparseDoubleMatrix Zeros = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				Zeros[i, i] = 0.0;

			SparseDoubleMatrix B = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				B[i, i] = i % 5 == 0 ? 1.0 : 0.0;

			var C = A.minus(B);
			AssertMatrixEqualsEps(C, Zeros);
			C = A - B;
			AssertMatrixEqualsEps(C, Zeros);
		}

		[Test]
		public void isLowerTriangularTest()
		{
			const int N = 50;
			SparseDoubleMatrix A = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				A[i, i] = i % 5 == 0 ? 1.0 : 0.0;

			Assert.AreEqual(A.IsLowerTriangular(), true);
			A[45, 40] = 1.0;
			Assert.AreEqual(A.IsLowerTriangular(), true);
			A[40, 45] = 1.0;
			Assert.AreEqual(A.IsLowerTriangular(), false);
		}

		[Test]
		public void TimesEqualsTest()
		{
			const int N = 50;
			SparseDoubleMatrix A = SparseDoubleMatrix.Identity(N, N);
			for (int i = 0; i < N; i++)
				A[i, i] = i % 5 == 0 ? 1.0 : 0.0;

			SparseDoubleMatrix AInit = A.Clone();
			SparseDoubleMatrix B = A.Clone();
			for (int i = 0; i < N; i++)
				B[i, i] = i % 5 == 0 ? 2.0 : 0.0;
			var C = A.Mul(2.0);
			AssertMatrixEqualsEps(B, C);
			var D = AInit * 2.0;
			AssertMatrixEqualsEps(B, D);
		}

		[Test]
		public void TimesTest()
		{
			const int N = 50;
			const int M = 30;
			SparseDoubleMatrix A = SparseDoubleMatrix.Identity(M, N);
			for (int i = 0; i < M; i++)
				if (i < N)
					A[i, i] = i % 5 == 0 ? 1.0 : 0.0;

			var b = new DoubleVector(N);
			for (int i = 0; i < N; i++)
				b[i] = 2.0;

			var B = new DoubleVector(M);
			for (int i = 0; i < M; i++)
				B[i] = i % 5 == 0 ? 2.0 : 0.0;

			var C = A.times(b);
			AssertVectorEqualsEps(B, C);
		}

		[Test]
		public void SolveGETest()
		{
			const int N = 50;
			SparseDoubleMatrix a = new SparseDoubleMatrix(N, N);
			for (int i = 0; i < N; i++)
				a[i, i] = 1;
			// Apply random rotations around each pair of axes. This will keep det(A) ~ 1
			Random rand = new Random();
			for (int i = 0; i < N; i++)
				for (int j = i + 1; j < N; j++)
				{
					double angle = rand.NextDouble() * 2 * Math.PI;
					SparseDoubleMatrix r = new SparseDoubleMatrix(N, N);
					for (int k = 0; k < N; k++)
						r[k, k] = 1;
					r[i, i] = r[j, j] = Math.Cos(angle);
					r[i, j] = Math.Sin(angle);
					r[j, i] = -Math.Sin(angle);
					a = a * r;
				}

			var ainit = a.Clone();
			// Generate random vector
			var b = new DoubleVector(N);
			for (int i = 0; i < N; i++)
				b[i] = rand.NextDouble();

			var binit = b.Clone();
			// Solve system
			var solver = new GaussianEliminationSolver();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var x = solver.SolveDestructive(a, b.GetInternalData());
			sw.Stop();
			Trace.WriteLine("Gaussian elimination took: " + sw.ElapsedTicks);
			// Put solution into system
			var b2 = ainit * x;

			// Verify result is the same
			Assert.IsTrue(VectorMath.LInfinityNorm(binit, b2) < 1e-6);
		}

		[Test]
		public void MapTest1()
		{
			int N = 6;

			var m1 = new SparseDoubleMatrix(N, N);
			var m2 = new SparseDoubleMatrix(N, N);

			// Fill matrix m2 diagonal elements with 66
			m1.MapSparseIncludingDiagonal((a, r, c) => a + (r == c ? 66 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
				for (int j = 0; j < m2.ColumnCount; ++j)
					Assert.AreEqual(i == j ? 66 : 0, m2[i, j]);

			// the same should work with the now already used matrix
			// fill diagonal now with 77
			m1.MapSparseIncludingDiagonal((a, r, c) => a + (r == c ? 77 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
				for (int j = 0; j < m2.ColumnCount; ++j)
					Assert.AreEqual(i == j ? 77 : 0, m2[i, j]);
		}

		[Test]
		public void MapTest2()
		{
			int N = 6;

			var m1 = new SparseDoubleMatrix(N, N);
			var m2 = new SparseDoubleMatrix(N, N);

			// Pre-fill m2 elements left of the diagonal with values, these values should be discarded;
			for (int i = 1; i < N; ++i)
				m2[i, i - 1] = i * 13;

			// Fill matrix m2 diagonal elements with 66
			m1.MapSparseIncludingDiagonal((a, r, c) => a + (r == c ? 66 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
				for (int j = 0; j < m2.ColumnCount; ++j)
					Assert.AreEqual(i == j ? 66 : 0, m2[i, j]);

			// the same should work with the now already used matrix
			// fill diagonal now with 77
			m1.MapSparseIncludingDiagonal((a, r, c) => a + (r == c ? 77 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
				for (int j = 0; j < m2.ColumnCount; ++j)
					Assert.AreEqual(i == j ? 77 : 0, m2[i, j]);
		}

		[Test]
		public void MapTest3()
		{
			int N = 6;

			var m1 = new SparseDoubleMatrix(N, N);
			var m2 = new SparseDoubleMatrix(N, N);

			// Pre-fill m2 elements right of the diagonal with values, these values should be discarded;
			for (int i = 0; i < N - 1; ++i)
				m2[i, i + 1] = i * 13;

			// Fill matrix m2 diagonal elements with 66
			m1.MapSparseIncludingDiagonal((a, r, c) => a + (r == c ? 66 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
				for (int j = 0; j < m2.ColumnCount; ++j)
					Assert.AreEqual(i == j ? 66 : 0, m2[i, j]);

			// the same should work with the now already used matrix
			// fill diagonal now with 77
			m1.MapSparseIncludingDiagonal((a, r, c) => a + (r == c ? 77 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
				for (int j = 0; j < m2.ColumnCount; ++j)
					Assert.AreEqual(i == j ? 77 : 0, m2[i, j]);
		}

		[Test]
		public void MapTest4()
		{
			int N = 6;

			var m1 = new SparseDoubleMatrix(N, N);
			var m2 = new SparseDoubleMatrix(N, N);

			// Pre-fill m1 elements left of the diagonal with values
			for (int i = 1; i < N; ++i)
				m1[i, i - 1] = i * 13;

			// Fill matrix m2 diagonal elements with 66
			m1.MapSparseIncludingDiagonal((a, r, c) => 3 * a + (r == c ? 66 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
			{
				for (int j = 0; j < m2.ColumnCount; ++j)
				{
					double expected = 0;
					if (i == j) expected = 66;
					else if (i - 1 == j) expected = i * 13 * 3;
					Assert.AreEqual(expected, m2[i, j]);
				}
			}

			// the same should work with the now already used matrix
			// fill diagonal now with 77
			m1.MapSparseIncludingDiagonal((a, r, c) => 5 * a + (r == c ? 77 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
			{
				for (int j = 0; j < m2.ColumnCount; ++j)
				{
					double expected = 0;
					if (i == j) expected = 77;
					else if (i - 1 == j) expected = i * 13 * 5;
					Assert.AreEqual(expected, m2[i, j]);
				}
			}
		}

		[Test]
		public void MapTest5()
		{
			int N = 6;

			var m1 = new SparseDoubleMatrix(N, N);
			var m2 = new SparseDoubleMatrix(N, N);

			// Pre-fill m1 elements right of the diagonal with values
			for (int i = 0; i < N - 1; ++i)
				m1[i, i + 1] = i * 13;

			// Fill matrix m2 diagonal elements with 66
			m1.MapSparseIncludingDiagonal((a, r, c) => 3 * a + (r == c ? 66 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
			{
				for (int j = 0; j < m2.ColumnCount; ++j)
				{
					double expected = 0;
					if (i == j) expected = 66;
					else if (i + 1 == j) expected = i * 13 * 3;
					Assert.AreEqual(expected, m2[i, j]);
				}
			}

			// the same should work with the now already used matrix
			// fill diagonal now with 77
			m1.MapSparseIncludingDiagonal((a, r, c) => 5 * a + (r == c ? 77 : 0), m2);

			for (int i = 0; i < m2.RowCount; ++i)
			{
				for (int j = 0; j < m2.ColumnCount; ++j)
				{
					double expected = 0;
					if (i == j) expected = 77;
					else if (i + 1 == j) expected = i * 13 * 5;
					Assert.AreEqual(expected, m2[i, j]);
				}
			}
		}

		private void AssertVectorEqualsEps(DoubleVector A, DoubleVector B)
		{
			double sum = 0.0;
			for (int i = 0; i < A.Length; i++)
			{
				sum += A[i] - B[i];
			}

			Assert.AreEqual(sum, 0.0, 1e-10);
		}

		private void AssertEqualsEps(double a, double b)
		{
			Assert.IsTrue(Math.Abs(a - b) < 1e-10);
		}

		private void AssertMatrixEqualsEps(SparseDoubleMatrix A, SparseDoubleMatrix B)
		{
			double sum = 0.0;
			for (int i = 0; i < A.RowCount; i++)
				for (int j = 0; j < A.ColumnCount; j++)
					sum += A[i, j] - B[i, j];

			AssertEqualsEps(sum, 0.0);
		}
	}
}