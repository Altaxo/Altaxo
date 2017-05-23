#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
	/// <summary>
	///
	/// </summary>
	/// <remarks>This class was translated to C# from the JAMA1.0.2 package.</remarks>
	public class QRDecomposition
	{
		/* ------------------------
	 Class variables
 * ------------------------ */

		/** Array for internal storage of decomposition.
		@serial internal array storage.
		*/
		private double[][] QR;

		/** Row and column dimensions.
		@serial column dimension.
		@serial row dimension.
		*/
		private int m, n;

		/** Array for internal storage of diagonal of R.
		@serial diagonal of R.
		*/
		private double[] Rdiag;

		private JaggedArrayMatrix _solveMatrixWorkspace;
		private double[] _solveVectorWorkspace;

		/* ------------------------
			 Constructor
		 * ------------------------ */

		/** QR Decomposition, computed by Householder reflections.
		@param A    Rectangular matrix
		@return     Structure to access R and the Householder vectors and compute Q.
		*/

		public QRDecomposition(IROMatrix<double> A)
		{
			Decompose(A);
		}

		public QRDecomposition()
		{
		}

		public void Decompose(IROMatrix<double> A)
		{
			// Initialize.
			if (m == A.RowCount && n == A.ColumnCount)
			{
				MatrixMath.Copy(A, new JaggedArrayMatrix(QR, m, n));
				//JaggedArrayMath.Copy(A, QR);
			}
			else
			{
				QR = JaggedArrayMath.GetMatrixCopy(A);
				m = A.RowCount;
				n = A.ColumnCount;
				Rdiag = new double[n];
			}

			// Main loop.
			for (int k = 0; k < n; k++)
			{
				// Compute 2-norm of k-th column without under/overflow.
				double nrm = 0;
				for (int i = k; i < m; i++)
				{
					nrm = RMath.Hypot(nrm, QR[i][k]);
				}

				if (nrm != 0.0)
				{
					// Form k-th Householder vector.
					if (QR[k][k] < 0)
					{
						nrm = -nrm;
					}
					for (int i = k; i < m; i++)
					{
						QR[i][k] /= nrm;
					}
					QR[k][k] += 1.0;

					// Apply transformation to remaining columns.
					for (int j = k + 1; j < n; j++)
					{
						double s = 0.0;
						for (int i = k; i < m; i++)
						{
							s += QR[i][k] * QR[i][j];
						}
						s = -s / QR[k][k];
						for (int i = k; i < m; i++)
						{
							QR[i][j] += s * QR[i][k];
						}
					}
				}
				Rdiag[k] = -nrm;
			}
		}

		/* ------------------------
			 Public Methods
		 * ------------------------ */

		/** Is the matrix full rank?
		@return     true if R, and hence A, has full rank.
		*/

		public bool IsFullRank()
		{
			for (int j = 0; j < n; j++)
			{
				if (Rdiag[j] == 0)
					return false;
			}
			return true;
		}

		/** Return the Householder vectors
		@return     Lower trapezoidal matrix whose columns define the reflections
		*/

		public JaggedArrayMatrix GetH()
		{
			JaggedArrayMatrix X = new JaggedArrayMatrix(m, n);
			double[][] H = X.Array;
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (i >= j)
					{
						H[i][j] = QR[i][j];
					}
					else
					{
						H[i][j] = 0.0;
					}
				}
			}
			return X;
		}

		/** Return the upper triangular factor
		@return     R
		*/

		public JaggedArrayMatrix GetR()
		{
			JaggedArrayMatrix X = new JaggedArrayMatrix(n, n);
			double[][] R = X.Array;
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (i < j)
					{
						R[i][j] = QR[i][j];
					}
					else if (i == j)
					{
						R[i][j] = Rdiag[i];
					}
					else
					{
						R[i][j] = 0.0;
					}
				}
			}
			return X;
		}

		/** Generate and return the (economy-sized) orthogonal factor
		@return     Q
		*/

		public JaggedArrayMatrix GetQ()
		{
			JaggedArrayMatrix X = new JaggedArrayMatrix(m, n);
			double[][] Q = X.Array;
			for (int k = n - 1; k >= 0; k--)
			{
				for (int i = 0; i < m; i++)
				{
					Q[i][k] = 0.0;
				}
				Q[k][k] = 1.0;
				for (int j = k; j < n; j++)
				{
					if (QR[k][k] != 0)
					{
						double s = 0.0;
						for (int i = k; i < m; i++)
						{
							s += QR[i][k] * Q[i][j];
						}
						s = -s / QR[k][k];
						for (int i = k; i < m; i++)
						{
							Q[i][j] += s * QR[i][k];
						}
					}
				}
			}
			return X;
		}

		public IMatrix<double> GetSolution(IROMatrix<double> B)
		{
			JaggedArrayMatrix result = new JaggedArrayMatrix(m, B.ColumnCount);
			Solve(B, result);
			return result;
		}

		public IMatrix<double> GetSolution(IROMatrix<double> A, IROMatrix<double> B)
		{
			Decompose(A);
			JaggedArrayMatrix result = new JaggedArrayMatrix(m, B.ColumnCount);
			Solve(B, result);
			return result;
		}

		public DoubleVector GetSolution(IReadOnlyList<double> B)
		{
			DoubleVector result = new DoubleVector(m);
			Solve(B, result);
			return result;
		}

		public DoubleVector GetSolution(IROMatrix<double> A, IReadOnlyList<double> B)
		{
			Decompose(A);
			DoubleVector result = new DoubleVector(m);
			Solve(B, result);
			return result;
		}

		public void Solve(IROMatrix<double> A, IROMatrix<double> B, IMatrix<double> Result)
		{
			Decompose(A);
			Solve(B, Result);
		}

		public void Solve(IROMatrix<double> A, IReadOnlyList<double> B, IVector<double> Result)
		{
			Decompose(A);
			Solve(B, Result);
		}

		/** Least squares solution of A*X = B
		@param B    A Matrix with as many rows as A and any number of columns.
		@return     X that minimizes the two norm of Q*R*X-B.
		@exception  IllegalArgumentException  Matrix row dimensions must agree.
		@exception  RuntimeException  Matrix is rank deficient.
		*/

		public void Solve(IROMatrix<double> B, IMatrix<double> result)
		{
			if (B.RowCount != m)
			{
				throw new ArgumentException("Matrix row dimensions must agree.");
			}
			if (!this.IsFullRank())
			{
				throw new Exception("Matrix is rank deficient.");
			}

			// Copy right hand side
			int nx = B.ColumnCount;
			double[][] X;
			if (_solveMatrixWorkspace != null && _solveMatrixWorkspace.RowCount == B.RowCount && _solveMatrixWorkspace.ColumnCount == B.ColumnCount)
			{
				X = _solveMatrixWorkspace.Array;
				MatrixMath.Copy(B, _solveMatrixWorkspace);
			}
			else
			{
				X = JaggedArrayMath.GetMatrixCopy(B);
				_solveMatrixWorkspace = new JaggedArrayMatrix(X, B.RowCount, B.ColumnCount);
			}

			// Compute Y = transpose(Q)*B
			for (int k = 0; k < n; k++)
			{
				for (int j = 0; j < nx; j++)
				{
					double s = 0.0;
					for (int i = k; i < m; i++)
					{
						s += QR[i][k] * X[i][j];
					}
					s = -s / QR[k][k];
					for (int i = k; i < m; i++)
					{
						X[i][j] += s * QR[i][k];
					}
				}
			}
			// Solve R*X = Y;
			for (int k = n - 1; k >= 0; k--)
			{
				for (int j = 0; j < nx; j++)
				{
					X[k][j] /= Rdiag[k];
				}
				for (int i = 0; i < k; i++)
				{
					for (int j = 0; j < nx; j++)
					{
						X[i][j] -= X[k][j] * QR[i][k];
					}
				}
			}

			MatrixMath.Submatrix(_solveMatrixWorkspace, result, 0, 0);
		}

		/** Least squares solution of A*X = B
	 @param B    A Matrix with as many rows as A and any number of columns.
	 @return     X that minimizes the two norm of Q*R*X-B.
	 @exception  IllegalArgumentException  Matrix row dimensions must agree.
	 @exception  RuntimeException  Matrix is rank deficient.
	 */

		public void Solve(IReadOnlyList<double> B, IVector<double> result)
		{
			if (B.Count != m)
			{
				throw new ArgumentException("Matrix row dimensions must agree.");
			}
			if (!this.IsFullRank())
			{
				throw new Exception("Matrix is rank deficient.");
			}

			// Copy right hand side
			double[] X;
			if (_solveVectorWorkspace != null && _solveVectorWorkspace.Length == B.Count)
			{
				X = _solveVectorWorkspace;
			}
			else
			{
				_solveVectorWorkspace = X = new double[B.Count];
			}
			for (int i = 0; i < X.Length; i++)
				X[i] = B[i]; // copy to workspace vector

			// Compute Y = transpose(Q)*B
			for (int k = 0; k < n; k++)
			{
				double s = 0.0;
				for (int i = k; i < m; i++)
				{
					s += QR[i][k] * X[i];
				}
				s = -s / QR[k][k];
				for (int i = k; i < m; i++)
				{
					X[i] += s * QR[i][k];
				}
			}
			// Solve R*X = Y;
			for (int k = n - 1; k >= 0; k--)
			{
				X[k] /= Rdiag[k];
				for (int i = 0; i < k; i++)
				{
					X[i] -= X[k] * QR[i][k];
				}
			}

			for (int i = 0; i < result.Length; i++)
				result[i] = X[i];
		}
	}
}