/*
 * ComplexDoubleQRDecomp.cs
 * Managed code is a port of JAMA code.
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;


namespace Altaxo.Calc.LinearAlgebra
{
	///<summary>This class computes the QR factorization of a general m by n <c>ComplexDoubleMatrix</c>.</summary>
	public sealed class ComplexDoubleQRDecomp : Algorithm {
		private readonly ComplexDoubleMatrix matrix;
		private bool isFullRank = true;

		private ComplexDoubleMatrix q_;
		private ComplexDoubleMatrix r_;

#if !MANAGED
		private Complex[] tau;
		int[] jpvt;
		private Complex[] qr;
#endif
		
		
		///<summary>Constructor for QR decomposition class. The constructor performs the factorization and the upper and
		///lower matrices are accessible by the <c>Q</c> and <c>R</c> properties.</summary>
		///<param name="matrix">The matrix to factor.</param>
		///<exception cref="ArgumentNullException">matrix is null.</exception>
		public ComplexDoubleQRDecomp(ComplexDoubleMatrix matrix) {
			if (matrix == null)
				throw new System.ArgumentNullException("matrix cannot be null.");
			this.matrix = matrix;
		}

		/// <summary>Performs the QR factorization.</summary>
		protected override void InternalCompute() {
			int m = matrix.RowLength;
			int n = matrix.ColumnLength;
#if MANAGED
			int minmn = m < n ? m : n;
			r_ = new ComplexDoubleMatrix(matrix); // create a copy
			ComplexDoubleVector[] u = new ComplexDoubleVector[minmn];
			for (int i = 0; i < minmn; i++) {
				u[i] = Householder.GenerateColumn(r_, i, m - 1, i);
				Householder.UA(u[i], r_, i, m - 1, i + 1, n - 1);
			}
			q_ = ComplexDoubleMatrix.CreateIdentity(m);
			for (int i = minmn - 1; i >= 0; i--) {
				Householder.UA(u[i], q_, i, m - 1, i, m - 1);
			}
#else
			qr = new Complex[matrix.data.Length];
			Array.Copy(matrix.data, qr, matrix.data.Length);
			jpvt = new int[n];
			jpvt[0] = 1;
			dnA.Math.Lapack.Geqp3.Compute(m, n, qr, m, jpvt, out tau);
			r_ = new ComplexDoubleMatrix(m, n);
			// Populate R

			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					if (i <= j) {
						r_.data[j * m + i] = qr[(jpvt[j]-1) * m + i];
					}
					else {
						r_.data[j * m + i] = Complex.Zero;
					}
				}
			}

			q_ = new ComplexDoubleMatrix(m, m);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < m; j++) {
					if (j < n)
						q_.data[j * m + i] = qr[j * m + i];
					else
						q_.data[j * m + i] = Complex.Zero;
				}
			}
			if( m < n ){
				dnA.Math.Lapack.Ungqr.Compute(m, m, m, q_.data, m, tau);
			} else{
				dnA.Math.Lapack.Ungqr.Compute(m, m, n, q_.data, m, tau);
			}
#endif
			for (int i = 0; i < m; i++) {
				if (q_[i, i] == 0)
					isFullRank = false;
			}
		}

		/// <summary>
		/// Determine whether the matrix is full rank or not
		/// </summary>
		/// <value>Boolean value indicates whether the given matrix is full rank or not</value>
		public bool IsFullRank {
			get {
				if (matrix.RowLength != matrix.ColumnLength) throw new NotSquareMatrixException();
				Compute();
				return isFullRank;
			}
		}

		///<summary>Returns the orthogonal Q matrix.</summary>
		public ComplexDoubleMatrix Q {
			get {
				Compute();
				return q_;
			}
		}

		///<summary>Returns the upper triangular factor R.</summary>
		public ComplexDoubleMatrix R {
			get {
				Compute();
				return r_;
			}
		}

		/// <summary>Finds the least squares solution of <c>A*X = B</c>, where <c>m &gt;= n</c></summary>
		/// <param name="B">A matrix with as many rows as A and any number of columns.</param>
		/// <returns>X that minimizes the two norm of <c>Q*R*X-B</c>.</returns>
		/// <exception cref="ArgumentException">Matrix row dimensions must agree.</exception>
		/// <exception cref="InvalidOperationException">Matrix is rank deficient or <c>m &lt; n</c>.</exception>
		public ComplexDoubleMatrix Solve (ComplexDoubleMatrix B) {
			if (B.RowLength != matrix.RowLength) {
				throw new ArgumentException("Matrix row dimensions must agree.");
			}
			if (matrix.RowLength < matrix.ColumnLength) {
				throw new System.InvalidOperationException("A must have at lest as a many rows as columns.");
			}
			Compute();
			if (!this.isFullRank) {
				throw new System.InvalidOperationException("Matrix is rank deficient.");
			}
      
			// Copy right hand side
			int m = matrix.RowLength;
			int n = matrix.ColumnLength;
			int nx = B.ColumnLength;
			ComplexDoubleMatrix ret = new ComplexDoubleMatrix(n,nx);
#if MANAGED
			ComplexDoubleMatrix X = new ComplexDoubleMatrix(B);
			// Compute Y = transpose(Q)*B
			Complex[] column = new Complex[q_.RowLength];
			for (int j = 0; j < nx; j++) {
				for (int k = 0; k < m; k++) {
					column[k] = X.data[k][j];
				}
				for (int i = 0; i < m; i++) {
					Complex s = Complex.Zero;
					for (int k = 0; k < m; k++) {
						s += ComplexMath.Conjugate(q_.data[k][i]) * column[k];
					}
					X.data[i][j] = s;
				} 
			}

			// Solve R*X = Y;
			for (int k = n-1; k >= 0; k--) {
				for (int j = 0; j < nx; j++) {
					X.data[k][j] /= r_.data[k][k];
				}
				for (int i = 0; i < k; i++) {
					for (int j = 0; j < nx; j++) {
						X.data[i][j] -= X.data[k][j]*r_.data[i][k];
					}
				}
			}
			for( int i = 0; i < n; i++ ){
				for( int j = 0; j < nx; j++ ){
					ret.data[i][j] = X.data[i][j];
				}
			}
#else
			Complex[] c = new Complex[B.data.Length];
		    Array.Copy(B.data, 0, c, 0, B.data.Length);
			dnA.Math.Lapack.Unmqr.Compute(Side.Left, Transpose.ConjTrans, m, nx, n, qr, m, tau, c, m);
			dnA.Math.Blas.Trsm.Compute(Order.ColumnMajor, Side.Left, UpLo.Upper, Transpose.NoTrans, Diag.NonUnit,
				n, nx, 1, qr, m, c, m);

			for ( int i = 0; i < n; i++ ) {
				for ( int j = 0; j < nx; j++) {
					ret.data[j*n+i] = c[j*m+(jpvt[i]-1)];
				}
			}

#endif
			return ret;
		}

	}
}
