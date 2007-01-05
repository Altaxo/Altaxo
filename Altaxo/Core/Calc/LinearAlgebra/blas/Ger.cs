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
 * Ger.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Rank-1 update of a general matrix</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Ger 
  {
    private Ger(){}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(Order order, int m, int n, object X, int lenX, ref int incx, object Y, int lenY, ref int incy, object A, int lenA, int lda) 
    {
      if ( X == null ) 
      {
        throw new ArgumentNullException("X", "X cannot be null.");
      }
      if ( Y == null ) 
      {
        throw new ArgumentNullException("Y", "Y cannot be null.");
      }
      if ( A == null ) 
      {
        throw new ArgumentNullException("Y", "Y cannot be null.");
      }
      if ( m < 0) 
      {
        throw new ArgumentException("m must be zero or greater", "m");
      }
      if ( n < 0) 
      {
        throw new ArgumentException("n must be zero or greater", "n");
      }
      if ( incx == 0 ) 
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      if ( incy == 0 ) 
      {
        throw new ArgumentException("incy cannot be zero.", "incy");
      }
      incx = System.Math.Abs(incx);
      incy = System.Math.Abs(incy);
      if ( lenX < (1 + (m-1)*incx) ) 
      {
        throw new ArgumentException("The dimension of X must be at least 1 + (m-1) * abs(incx).");
      }
      if ( lenY < (1 + (n-1)*incy) ) 
      {
        throw new ArgumentException("The dimension of Y must be at least 1 + (n-1) * abs(incy).");
      }

      if ( order == Order.ColumnMajor ) 
      {
        if ( lda < System.Math.Max(1, m) ) 
        {
          throw new ArgumentException("lda must be at least max(1,m).", "lda");
        }
        if( lenA < lda * n ) 
        {
          throw new ArgumentException("A must be at least lda * n.", "A");
        }
      } 
      else 
      {
        if ( lda < System.Math.Max(1, n) ) 
        {
          throw new ArgumentException("lda must be at least max(1,n).", "lda");
        }
        if( lenA < lda * m ) 
        {
          throw new ArgumentException("A must be at least lda * m.", "A");
        }
      }
    }
    ///<summary>Compute the function of this class</summary>
    internal static void Compute( Order order, int m, int n, float alpha, float[] X, int incx, float[] Y, int incy, float[] A, int lda ) 
    {
      ArgumentCheck(order, m, n, X, X.Length, ref incx, Y, Y.Length, ref incy, A, A.Length, lda);
      
#if MANAGED
      float temp;
      if ( order == Order.RowMajor ) 
      {
        for ( int i = 0, ix = 0; i < m; ++i, ix += incx ) 
        {
          temp = alpha * X[ix];
          for ( int j=0, jy =0; j < n; ++j, jy += incy ) 
          {
            A[i * lda + j] += temp*Y[jy];
          }
        }
      } 
      else 
      {
        for ( int j=0, jy=0; j < n; ++j, jy+=incy) 
        {
          temp = alpha * Y[jy];
          for (int i=0, ix=0; i < m ; ++i, ix+=incx) 
          {
            A[lda * j + i] += temp*X[ix];
          }
        }
      }
#else
      dna_blas_sger(order, m, n, alpha, X, incx, Y, incy, A, lda);
#endif
    }

    internal static void Compute( Order order, int m, int n, double alpha, double[] X, int incx, double[] Y, int incy, double[] A, int lda ) 
    {
      ArgumentCheck(order, m, n, X, X.Length, ref incx, Y, Y.Length, ref incy, A, A.Length, lda);
      
#if MANAGED
      double temp;
      if ( order == Order.RowMajor ) 
      {
        for ( int i = 0, ix = 0; i < m; ++i, ix += incx ) 
        {
          temp = alpha * X[ix];
          for ( int j=0, jy =0; j < n; ++j, jy += incy ) 
          {
            A[i * lda + j] += temp*Y[jy];
          }
        }
      } 
      else 
      {
        for ( int j=0, jy=0; j < n; ++j, jy+=incy) 
        {
          temp = alpha * Y[jy];
          for (int i=0, ix=0; i < m ; ++i, ix+=incx) 
          {
            A[lda * j + i] += temp*X[ix];
          }
        }
      }
#else
      dna_blas_dger(order, m, n, alpha, X, incx, Y, incy, A, lda);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_sger( Order order, int M, int N, float alpha, [In]float[] X, int incX, [In]float[] Y, int incY, [In,Out]float[] A, int lda);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dger( Order order, int M, int N, double alpha, [In]double[] X, int incX, [In]double[] Y, int incY, [In,Out]double[] A, int lda);
#endif
  }
}
