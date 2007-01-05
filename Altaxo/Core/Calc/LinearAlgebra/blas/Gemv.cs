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
 * Gemv.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary> Matrix-vector product using a general matrix </summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Gemv 
  {
    private Gemv(){}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(Order order, Transpose transA, int m, int n, object A, int lenA, int lda, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) 
    {
      if ( A == null ) 
      {
        throw new ArgumentNullException("A", "A cannot be null.");
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException("X", "X cannot be null.");
      }
      if ( Y == null ) 
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
      if ( order == Order.ColumnMajor ) 
      {
        if ( lda < 1 || lda < m ) 
        {
          throw new ArgumentException("lda must be at least m.", "lda");
        }
        if( lenA < lda * n ) 
        {
          throw new ArgumentException("A must be at least lda * n.", "A");
        }
      } 
      else 
      {
        if ( lda < 1 || lda < n ) 
        {
          throw new ArgumentException("lda must be at least n.", "lda");
        }
        if( lenA < lda * m ) 
        {
          throw new ArgumentException("A must be at least lda * m.", "A");
        }
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
      if ( transA == Transpose.NoTrans ) 
      {
        if ( lenX < (1 + (n-1)*incx) ) 
        {
          throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * abs(incx).");
        }
        if ( lenY < (1 + (m-1)*incy) ) 
        {
          throw new ArgumentException("The dimension of Y must be at least 1 + (m-1) * abs(incy).");
        }
      } 
      else 
      {
        if ( lenX < (1 + (m-1)*incx)) 
        {
          throw new ArgumentException("The dimension of X must be at least 1 + (m-1) * abs(incx).");
        }
        if ( lenY < (1 + (n-1)*incy) ) 
        {
          throw new ArgumentException("The dimension of Y must be at least 1 + (n-1) * abs(incy).");
        }
      }
    }

    ///<summary>Compute the function of this class</summary>
    internal static void Compute( Order order, Transpose transA, int m, int n, float alpha, float[] A, int lda, float[] X, int incx, float beta, float[] Y, int incy ) 
    {
      if ( transA == Transpose.ConjTrans ) 
      {
        transA = Transpose.Trans;
      }
      ArgumentCheck(order, transA, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      if ( alpha == 0.0 && beta == 1.0 ) 
      {
        return;
      }
      
#if MANAGED
      int lenX = m;
      int lenY = n;

      if ( transA == Transpose.NoTrans ) 
      {
        lenX = n;
        lenY = m;
      }
      if ( beta == 0.0 ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] = 0;
        }
      } 
      else if ( beta != 1.0 ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] *= beta;
        }
      }
      if ( alpha == 0.0 ) 
      {
        return;
      }
      float add_val;
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans) 
        || (order == Order.ColumnMajor && transA == Transpose.Trans) ) 
      {
        for (int i = 0, iy = 0; i < lenY; ++i, iy+=incy) 
        {
          add_val = 0;
          for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
          {
            add_val += X[jx] * A[i*lda + j];
          }
          Y[iy] += alpha * add_val;
        }
      } 
      else 
      {
        for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
        {
          add_val = alpha * X[jx];
          if ( add_val != 0.0 ) 
          {
            for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
            {
              Y[iy] += add_val * A[j*lda + i]; 
            }
          }
        }
      }    
#else
      dna_blas_sgemv(order, transA, m, n, alpha, A, lda, X, incx, beta, Y, incy);
#endif
    }

    internal static void Compute( Order order, Transpose transA, int m, int n, double alpha, double[] A, int lda, double[] X, int incx, double beta, double[] Y, int incy ) 
    {
      if ( transA == Transpose.ConjTrans ) 
      {
        transA = Transpose.Trans;
      }
      ArgumentCheck(order, transA, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      if ( alpha == 0.0 && beta == 1.0 ) 
      {
        return;
      }
      
#if MANAGED
      int lenX = m;
      int lenY = n;

      if ( transA == Transpose.NoTrans ) 
      {
        lenX = n;
        lenY = m;
      }
      if ( beta == 0.0 ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] = 0;
        }
      } 
      else if ( beta != 1.0 ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] *= beta;
        }
      }
      if ( alpha == 0.0 ) 
      {
        return;
      }
      double add_val;
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans) 
        || (order == Order.ColumnMajor && transA == Transpose.Trans) ) 
      {
        for (int i = 0, iy = 0; i < lenY; ++i, iy+=incy) 
        {
          add_val = 0;
          for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
          {
            add_val += X[jx] * A[i*lda + j];
          }
          Y[iy] += alpha * add_val;
        }
      } 
      else 
      {
        for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
        {
          add_val = alpha * X[jx];
          if ( add_val != 0.0 ) 
          {
            for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
            {
              Y[iy] += add_val * A[j*lda + i]; 
            }
          }
        }
      }    
#else
      dna_blas_dgemv(order, transA, m, n, alpha, A, lda, X, incx, beta, Y, incy);
#endif
    }

    internal static void Compute( Order order, Transpose transA, int m, int n, ComplexFloat alpha, ComplexFloat[] A, int lda, ComplexFloat[] X, int incx, ComplexFloat beta, ComplexFloat[] Y, int incy ) 
    {
      ArgumentCheck(order, transA, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int lenX = m;
      int lenY = n;

      if ( transA == Transpose.NoTrans ) 
      {
        lenX = n;
        lenY = m;
      }
      if ( beta == ComplexFloat.Zero ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] = 0;
        }
      } 
      else if ( beta != ComplexFloat.One ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] *= beta;
        }
      }
      if ( alpha == ComplexFloat.Zero ) 
      {
        return;
      }

      ComplexFloat add_val = new ComplexFloat(0);
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans) 
        || (order == Order.ColumnMajor && transA == Transpose.Trans) ) 
      {
        for (int i = 0, iy = 0; i < lenY; ++i, iy+=incy) 
        {
          add_val = 0;
          for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
          {
            add_val += X[jx] * A[i*lda + j];
          }
          Y[iy] += alpha * add_val;
        }
      } 
      else if ( (order == Order.RowMajor && transA == Transpose.Trans) 
        || (order == Order.ColumnMajor && transA == Transpose.NoTrans) ) 
      {
        for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
        {
          add_val = alpha * X[jx];
          if ( add_val != ComplexFloat.Zero ) 
          {
            for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
            {
              Y[iy] += add_val * A[j*lda + i]; 
            }
          }
        }
      } 
      else if (order == Order.RowMajor && transA == Transpose.ConjTrans) 
      {
        for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
        {
          add_val = alpha * X[jx];
          if ( add_val != ComplexFloat.Zero ) 
          {
            for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
            {
              Y[iy] += add_val * ComplexMath.Conjugate(A[j*lda + i]); 
            }
          }
        }
      } 
      else 
      {
        for (int i = 0, iy = 0; i < lenY; ++i, iy+=incy) 
        {
          add_val = 0;
          for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
          {
            add_val += X[jx] * ComplexMath.Conjugate(A[i*lda + j]);
          }
          Y[iy] += alpha * add_val;
        }
      }
#else
      dna_blas_cgemv(order, transA, m, n, ref alpha, A, lda, X, incx, ref beta, Y, incy);
#endif
    }

    internal static void Compute( Order order, Transpose transA, int m, int n, Complex alpha, Complex[] A, int lda, Complex[] X, int incx, Complex beta, Complex[] Y, int incy )
    {
      ArgumentCheck(order, transA, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int lenX = m;
      int lenY = n;

      if ( transA == Transpose.NoTrans ) 
      {
        lenX = n;
        lenY = m;
      }
      if ( beta == Complex.Zero ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] = 0;
        }
      } 
      else if ( beta != Complex.One ) 
      {
        for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
        {
          Y[iy] *= beta;
        }
      }
      if ( alpha == Complex.Zero ) 
      {
        return;
      }
      Complex add_val = new Complex(0);
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans) 
        || (order == Order.ColumnMajor && transA == Transpose.Trans) ) 
      {
        for (int i = 0, iy = 0; i < lenY; ++i, iy+=incy) 
        {
          add_val = 0;
          for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
          {
            add_val += X[jx] * A[i*lda + j];
          }
          Y[iy] += alpha * add_val;
        }
      } 
      else if ( (order == Order.RowMajor && transA == Transpose.Trans) 
        || (order == Order.ColumnMajor && transA == Transpose.NoTrans) ) 
      {
        for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
        {
          add_val = alpha * X[jx];
          if ( add_val != Complex.Zero ) 
          {
            for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
            {
              Y[iy] += add_val * A[j*lda + i]; 
            }
          }
        }
      } 
      else if (order == Order.RowMajor && transA == Transpose.ConjTrans) 
      {
        for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
        {
          add_val = alpha * X[jx];
          if ( add_val != Complex.Zero ) 
          {
            for ( int i = 0, iy = 0; i < lenY; ++i, iy+=incy ) 
            {
              Y[iy] += add_val * ComplexMath.Conjugate(A[j*lda + i]); 
            }
          }
        }
      } 
      else 
      {
        for (int i = 0, iy = 0; i < lenY; ++i, iy+=incy) 
        {
          add_val = 0;
          for (int j = 0, jx = 0; j < lenX; ++j, jx+=incx) 
          {
            add_val += X[jx] * ComplexMath.Conjugate(A[i*lda + j]);
          }
          Y[iy] += alpha * add_val;
        }
      }
#else
      dna_blas_zgemv(order, transA, m, n, ref alpha, A, lda, X, incx, ref beta, Y, incy);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_sgemv( Order order, Transpose TransA, int M, int N, float alpha, [In]float[] A, int lda, [In]float[] X, int incX, float beta, [In,Out]float[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dgemv( Order order, Transpose TransA, int M, int N, double alpha, [In]double[] A, int lda, [In]double[] X, int incX, double beta, [In,Out]double[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_cgemv( Order order, Transpose TransA, int M, int N, ref ComplexFloat alpha, [In]ComplexFloat[] A, int lda, [In]ComplexFloat[] X, int incX, ref ComplexFloat beta, [In,Out]ComplexFloat[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zgemv( Order order, Transpose TransA, int M, int N, ref Complex alpha, [In]Complex[] A, int lda, [In]Complex[] X, int incX, ref Complex beta, [In,Out]Complex[] Y, int incY);
#endif
  }
}
