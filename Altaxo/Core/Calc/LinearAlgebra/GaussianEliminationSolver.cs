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
		private BEJaggedArrayMatrixWrapper<double> _temp_A;
		private double[] _temp_b;
		private double[] _temp_x;

		/// <summary>Solves system of linear equations Ax = b using Gaussian elimination with partial pivoting.
		/// Attention! Both matrix A and vector b are destroyed (changed).</summary>
		/// <param name="A">Elements of matrix 'A'. This array is modified!</param>
		/// <param name="b">Right part 'b'. This array is also modified!</param>
		/// <param name="x">Vector to store the result, i.e. the solution to the problem a x = b.</param>
		public void SolveDestructive(BEJaggedArrayMatrixWrapper<double> A, double[] b, double[] x)
		{
			var a = A.Array;
			if (a == null)
				throw new ArgumentNullException(nameof(A));
			if (b == null)
				throw new ArgumentNullException(nameof(b));
			if (x == null)
				throw new ArgumentException(nameof(x));

			int n = A.Rows;

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
			var cols = A.Columns;
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

			int n = A.Rows;

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
			if (_temp_A.Rows != A.Rows || _temp_A.Columns != A.Columns)
				_temp_A = new BEJaggedArrayMatrixWrapper<double>(A.Rows, A.Columns);
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
	}
}