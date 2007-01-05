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
 * Gbmv.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Matrix-vector product using a general band matrix</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Gbmv 
  {
    private Gbmv(){}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(Order order, Transpose transA, int width, int m, int n, object A, int lenA, int lda, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) 
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
        throw new ArgumentException("m must be zero or greater", "m" );
      }
      if ( n < 0) 
      {
        throw new ArgumentException("n must be zero or greater", "n");
      }
      if (lda < width)
      {
        throw new ArgumentException("lda must be at least ku+kl+1", "lda");
      }
      if( lenA < lda * width )
      {
        throw new ArgumentException("A must be at least lda * (ku + k1 + 1).", "A");
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
      if ( transA == Transpose.NoTrans && order == Order.RowMajor ) 
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
    internal static void Compute( Order order, Transpose transA, int m, int n, int kl, int ku, float alpha, float[] A, int lda, float[] X, int incx, float beta, float[] Y, int incy )
    {
      if ( transA == Transpose.ConjTrans ) 
      {
        transA = Transpose.Trans;
      }
      ArgumentCheck(order, transA, ku+kl+1, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      if ( alpha == 0.0 && beta == 1.0 ) 
      {
        return;
      }
      
#if MANAGED
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans ) || (order == Order.ColumnMajor && transA != Transpose.NoTrans ) ) 
      {
        // operating on rows, so the banded storage only implies propper conversion on the elements of vector X
        for(int i=0; i<m; ++i) 
        {
          Y[i*incy] *= beta;
          for(int j=0; j<ku+kl+1; ++j)  
          {
            Y[i*incy] += alpha* A[ j+ i*lda] * X[(System.Math.Max(i-kl,0) +j)*incx];
          }
        }
      }
      else 
      {
        // the elements of the main diagonal are given by (i, ku), where i=0... min(m,n) -1
        // given an element of the main diagonal, its corresponding row will be formed of elts of the form (i+k, ku-k)
        // constraints on k show that it can evolve between k_min = min(i,kl) and k_max = min(m-1-i,ku)
        for(int i=0; i< System.Math.Min(m,n); ++i) 
        {
          int ti = i*incy;
          Y[ti] *= beta;
          for (int k = -System.Math.Min(i,kl); k <= System.Math.Min(m-1-i,ku); ++k) 
          {
            Y[ti] += alpha * A[(i+k)*lda + ku-k] * X[incx*(i + k)];
          }
        }
      }
#else
      dna_blas_sgbmv(order, transA, m, n, kl, ku, alpha, A, lda, X, incx, beta, Y, incy);
#endif
    }

    internal static void Compute( Order order, Transpose transA, int m, int n, int kl, int ku, double alpha, double[] A, int lda, double[] X, int incx, double beta, double[] Y, int incy)
    {
      if ( transA == Transpose.ConjTrans ) 
      {
        transA = Transpose.Trans;
      }
      ArgumentCheck(order, transA, ku+kl+1, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      if ( alpha == 0.0 && beta == 1.0 ) 
      {
        return;
      }
      
#if MANAGED
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans ) || (order == Order.ColumnMajor && transA != Transpose.NoTrans ) ) 
      {
        // operating on rows, so the banded storage only implies propper conversion on the elements of vector X
        for(int i=0; i<m; ++i) 
        {
          Y[i*incy] *= beta;
          for(int j=0; j<ku+kl+1; ++j)  
          {
            Y[i*incy] += alpha* A[ j+ i*lda] * X[(System.Math.Max(i-kl,0) +j)*incx];
          }
        }
      }
      else 
      {
        // the elements of the main diagonal are given by (i, ku), where i=0... min(m,n) -1
        // given an element of the main diagonal, its corresponding row will be formed of elts of the form (i+k, ku-k)
        // constraints on k show that it can evolve between k_min = min(i,kl) and k_max = min(m-1-i,ku)
        for(int i=0; i< System.Math.Min(m,n); ++i) 
        {
          int ti = i*incy;
          Y[ti] *= beta;
          for (int k = -System.Math.Min(i,kl); k <= System.Math.Min(m-1-i,ku); ++k) 
          {
            Y[ti] += alpha * A[(i+k)*lda + ku-k] * X[incx*(i + k)];
          }
        }
      }
#else
      dna_blas_dgbmv(order, transA, m, n, kl, ku, alpha, A, lda, X, incx, beta, Y, incy);
#endif
    }

    internal static void Compute( Order order, Transpose transA, int m, int n, int kl, int ku, ComplexFloat alpha, ComplexFloat[] A, int lda, ComplexFloat[] X, int incx, ComplexFloat beta, ComplexFloat[] Y, int incy )
    {
      ArgumentCheck(order, transA, ku+kl+1, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      if ( alpha == ComplexFloat.Zero && beta == ComplexFloat.One ) 
      {
        return;
      }
      
#if MANAGED
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans) || (order == Order.ColumnMajor && transA != Transpose.NoTrans ) ) 
      {
        for(int i=0; i<m; ++i) 
        {
          Y[i*incy] *= beta;
          for(int j=0; j<ku+kl+1; ++j)  
          {
            Y[i*incy] += alpha* A[ j+ i*lda] * X[(System.Math.Max(i-kl,0) +j)*incx];
          }
        }
      }
      else if ( order == Order.RowMajor && transA == Transpose.ConjTrans ) 
      {
        for(int i=0; i<m; ++i) 
        {
          Y[i*incy] *= beta;
          for(int j=0; j<ku+kl+1; ++j)  
          {
            Y[i*incy] += alpha* ComplexMath.Conjugate(A[ j+ i*lda]) * X[(System.Math.Max(i-kl,0) +j)*incx];
          }
        }
      }
      else if ( order == Order.ColumnMajor && transA == Transpose.ConjTrans) 
      {
        for(int i=0; i< System.Math.Min(m,n); ++i) 
        {
          int ti = i*incy;
          Y[ti] *= beta;
          for (int k = -System.Math.Min(i,kl); k <= System.Math.Min(m-1-i,ku); ++k) 
          {
            Y[ti] += alpha * ComplexMath.Conjugate(A[(i+k)*lda + ku-k]) * X[incx*(i + k)];
          }
        }
      }
      else 
      { // corresponds to the case ( (colMajor && noTranspose) || (rowMajor && transpose)) - no conjugates here
        for(int i=0; i< System.Math.Min(m,n); ++i) 
        {
          int ti = i*incy;
          Y[ti] *= beta;
          for (int k = -System.Math.Min(i,kl); k <= System.Math.Min(m-1-i,ku); ++k) 
          {
            Y[ti] += alpha * A[(i+k)*lda + ku-k] * X[incx*(i + k)];
          }
        }
      }
#else
      dna_blas_cgbmv(order, transA, m, n, kl, ku, ref alpha, A, lda, X, incx, ref beta, Y, incy);
#endif
    }

    internal static void Compute( Order order, Transpose transA, int m, int n, int kl, int ku, Complex alpha, Complex[] A, int lda, Complex[] X, int incx, Complex beta, Complex[] Y, int incy )
    {
      ArgumentCheck(order, transA, ku+kl+1, m, n, A, A.Length, lda, X, X.Length, ref incx, Y, Y.Length, ref incy);
      if ( alpha == Complex.Zero && beta == Complex.One ) 
      {
        return;
      }
      
#if MANAGED
      if ( (order == Order.RowMajor && transA == Transpose.NoTrans) || (order == Order.ColumnMajor && transA != Transpose.NoTrans ) ) 
      {
        for(int i=0; i<m; ++i) 
        {
          Y[i*incy] *= beta;
          for(int j=0; j<ku+kl+1; ++j)  
          {
            Y[i*incy] += alpha* A[ j+ i*lda] * X[(System.Math.Max(i-kl,0) +j)*incx];
          }
        }
      }
      else if ( order == Order.RowMajor && transA == Transpose.ConjTrans ) 
      {
        for(int i=0; i<m; ++i) 
        {
          Y[i*incy] *= beta;
          for(int j=0; j<ku+kl+1; ++j)  
          {
            Y[i*incy] += alpha* ComplexMath.Conjugate(A[ j+ i*lda]) * X[(System.Math.Max(i-kl,0) +j)*incx];
          }
        }
      }
      else if ( order == Order.ColumnMajor && transA == Transpose.ConjTrans) 
      {
        for(int i=0; i< System.Math.Min(m,n); ++i) 
        {
          int ti = i*incy;
          Y[ti] *= beta;
          for (int k = -System.Math.Min(i,kl); k <= System.Math.Min(m-1-i,ku); ++k) 
          {
            Y[ti] += alpha * ComplexMath.Conjugate(A[(i+k)*lda + ku-k]) * X[incx*(i + k)];
          }
        }
      }
      else 
      { // corresponds to the case ( (colMajor && noTranspose) || (rowMajor && transpose)) - no conjugates here
        for(int i=0; i< System.Math.Min(m,n); ++i) 
        {
          int ti = i*incy;
          Y[ti] *= beta;
          for (int k = -System.Math.Min(i,kl); k <= System.Math.Min(m-1-i,ku); ++k) 
          {
            Y[ti] += alpha * A[(i+k)*lda + ku-k] * X[incx*(i + k)];
          }
        }
      }
#else
      dna_blas_zgbmv(order, transA, m, n, kl, ku, ref alpha, A, lda, X, incx, ref beta, Y, incy);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_sgbmv(Order order, Transpose TransA, int M, int N, int KL, int KU, float alpha,  [In]float[] A, int lda, [In]float[] X, int incX, float beta, [In,Out]float[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dgbmv( Order order, Transpose TransA, int M, int N, int KL, int KU, double alpha, [In]double[] A, int lda, [In]double[] X, int incX, double beta, [In,Out]double[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_cgbmv( Order order, Transpose TransA, int M, int N, int KL, int KU, ref ComplexFloat alpha, [In]ComplexFloat[] A, int lda, [In]ComplexFloat[] X, int incX, ref ComplexFloat beta, [In,Out]ComplexFloat[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zgbmv( Order order, Transpose TransA, int M, int N, int KL, int KU, ref Complex alpha, [In]Complex[] A, int lda, [In]Complex[] X, int incX, ref Complex beta, [In,Out]Complex[] Y, int incY);
#endif
  }
}
