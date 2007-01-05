#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

/*
 * DoubleQRDecomp.cs
 * Managed code is a port of JAMA code.
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;


namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>This class computes the QR factorization of a general m by n <c>DoubleMatrix</c>.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public sealed class DoubleQRDecomp : Algorithm 
  {
    private readonly DoubleMatrix matrix;
    private bool isFullRank = true;

    private DoubleMatrix q_;
    private DoubleMatrix r_;
#if !MANAGED
    private double[] tau;
    int[] jpvt;
    private double[] qr;
#endif
    ///<summary>Constructor for QR decomposition class. The constructor performs the factorization and the upper and
    ///lower matrices are accessible by the <c>Q</c> and <c>R</c> properties.</summary>
    ///<param name="matrix">The matrix to factor.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public DoubleQRDecomp(IROMatrix matrix) 
    {
      if (matrix == null)
        throw new System.ArgumentNullException("matrix cannot be null.");
      this.matrix = new DoubleMatrix(matrix);
    }

    /// <summary>Performs the QR factorization.</summary>
    protected override void InternalCompute() 
    {
      int m = matrix.Rows;
      int n = matrix.Columns;
      
#if MANAGED
      int minmn = m < n ? m : n;
      r_ = new DoubleMatrix(matrix); // create a copy
      DoubleVector[] u = new DoubleVector[minmn];
      for (int i = 0; i < minmn; i++) 
      {
        u[i] = Householder.GenerateColumn(r_, i, m-1, i);
        Householder.UA(u[i], r_, i, m-1, i + 1, n-1);
      }
      q_ = DoubleMatrix.CreateIdentity(m);
      for (int i = minmn - 1; i >= 0; i--) 
      {
        Householder.UA(u[i], q_, i, m - 1, i, m - 1);
      }
#else
      qr = new double[matrix.data.Length];
      Array.Copy(matrix.data, qr, matrix.data.Length);
      jpvt = new int[n];
      jpvt[0] = 1;
      Lapack.Geqp3.Compute(m, n, qr, m, jpvt, out tau);
      r_ = new DoubleMatrix(m, n);
      // Populate R
      for (int i = 0; i < m; i++) {
        for (int j = 0; j < n; j++) {
          if (i <= j) {
            r_.data[j * m + i] = qr[(jpvt[j]-1) * m + i];
          }
          else {
            r_.data[j * m + i] = 0.0;
          }
        }
      }
      q_ = new DoubleMatrix(m, m);
      for (int i = 0; i < m; i++) {
        for (int j = 0; j < m; j++) {
          if (j < n)
            q_.data[j * m + i] = qr[j * m + i];
          else
            q_.data[j * m + i] = 0.0;
        }
      }

      if( m < n ){
        Lapack.Orgqr.Compute(m, m, m, q_.data, m, tau);
      } else{
        Lapack.Orgqr.Compute(m, m, n, q_.data, m, tau);
      }
#endif
      for (int i = 0; i < m; i++) 
      {
        if (q_[i, i] == 0)
          isFullRank = false;
      }
    }

    /// <summary>
    /// Determine whether the matrix is full rank or not
    /// </summary>
    /// <value>Boolean value indicates whether the given matrix is full rank or not</value>
    public bool IsFullRank 
    {
      get 
      {
        Compute();
        return isFullRank;
      }
    }

    ///<summary>Returns the orthogonal Q matrix.</summary>
    public DoubleMatrix Q 
    {
      get 
      {
        Compute();
        return q_;
      }
    }

    ///<summary>Returns the upper triangular factor R.</summary>
    public DoubleMatrix R 
    {
      get 
      {
        Compute();
        return r_;
      }
    }

    /// <summary>Finds the least squares solution of <c>A*X = B</c>, where <c>m &lt;= n</c></summary>
    /// <param name="B">A matrix with as many rows as A and any number of columns.</param>
    /// <returns>X that minimizes the two norm of <c>Q*R*X-B</c>.</returns>
    /// <exception cref="ArgumentException">Matrix row dimensions must agree.</exception>
    /// <exception cref="InvalidOperationException">Matrix is rank deficient or <c>m &gt; n</c>.</exception>
    public DoubleMatrix Solve (IROMatrix B) 
    {
      if (B.Rows != matrix.Rows) 
      {
        throw new ArgumentException("Matrix row dimensions must agree.");
      }
      if (matrix.Rows < matrix.Columns) 
      {
        throw new System.InvalidOperationException("A must have at lest as a many rows as columns.");
      }
      Compute();
      if (!this.isFullRank) 
      {
        throw new System.InvalidOperationException("Matrix is rank deficient.");
      }
      
      // Copy right hand side
      int m = matrix.Rows;
      int n = matrix.Columns;
      int nx = B.Columns;
      DoubleMatrix ret = new DoubleMatrix(n,nx);

#if MANAGED
      DoubleMatrix X = new DoubleMatrix(B);
      // Compute Y = transpose(Q)*B
      double[] column = new double[q_.RowLength];
      for (int j = 0; j < nx; j++) 
      {
        for (int k = 0; k < m; k++) 
        {
          column[k] = X.data[k][j];
        }
        for (int i = 0; i < m; i++) 
        {
          double s = 0;
          for (int k = 0; k < m; k++) 
          {
            s += q_.data[k][i] * column[k];
          }
          X.data[i][j] = s;
        } 
      }

      // Solve R*X = Y;
      for (int k = n-1; k >= 0; k--) 
      {
        for (int j = 0; j < nx; j++) 
        {
          X.data[k][j] /= r_.data[k][k];
        }
        for (int i = 0; i < k; i++) 
        {
          for (int j = 0; j < nx; j++) 
          {
            X.data[i][j] -= X.data[k][j]*r_.data[i][k];
          }
        }
      }
      for( int i = 0; i < n; i++ )
      {
        for( int j = 0; j < nx; j++ )
        {
          ret.data[i][j] = X.data[i][j];
        }
      }

#else
      double[] c = DoubleMatrix.ToLinearArray(B);
      Lapack.Ormqr.Compute(Lapack.Side.Left, Lapack.Transpose.Trans, m, nx, n, qr, m, tau, c, m);
      Blas.Trsm.Compute(Blas.Order.ColumnMajor, Blas.Side.Left, Blas.UpLo.Upper, Blas.Transpose.NoTrans, Blas.Diag.NonUnit,
        n, nx, 1, qr, m, c, m);
      for ( int i = 0; i < n; i++ ) {
        for ( int j = 0; j < nx; j++) {
          ret.data[j*n+i] = c[j*m+(jpvt[i]-1)];
        }
      }

#endif
      return ret;
    }
    
    ///<summary>Calculates the determinant (absolute value) of the matrix.</summary>
    ///<returns>the determinant of the matrix.</returns>
    public double GetDeterminant() 
    {
      if (matrix.Rows != matrix.Columns)
      {
        throw new NotSquareMatrixException();
      }
      Compute();
      if (!isFullRank) 
      {
        return 0;
      }
      else 
      {
        double ret = 1;
        for (int j = 0; j < r_.RowLength; j++) 
        {
          ret *= r_[j, j];
        }
        return System.Math.Abs(ret);
      }
    }
  }
}
