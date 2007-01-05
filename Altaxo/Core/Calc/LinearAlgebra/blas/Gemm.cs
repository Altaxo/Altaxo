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
 * Gemm.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Computes a scalar-matrix-matrix product and adds the result to a scalar-matrix product.</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Gemm {
    private Gemm() {}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(Order order, Transpose transA, Transpose transB, int m, int n, int k, object A, int lenA, int lda, object B, int lenB, int ldb, object C, int lenC, int ldc) {
      if ( A == null ) {
        throw new ArgumentNullException("A", "A cannot be null.");
      }
      if ( B == null ) {
        throw new ArgumentNullException("B", "B cannot be null.");
      }
      if ( C == null ) {
        throw new ArgumentNullException("C", "C cannot be null.");
      }
      if ( m < 0) {
        throw new ArgumentException( "m must be zero or greater", "m");
      }
      if ( n < 0) {
        throw new ArgumentException( "n must be zero or greater", "n");
      }
      
      if ( (order == Order.ColumnMajor && transA == Transpose.NoTrans) || (order == Order.RowMajor && transA != Transpose.NoTrans) ) {
        if ( lda < System.Math.Max(1, m) ) {
          throw new ArgumentException("lda must be at least max(1,m).", "lda");
        }
        if( lenA < lda * k ) {
          throw new ArgumentException("A must be at least lda * k.", "A");
        }
      } else {
        if ( lda < System.Math.Max(1, k) ) {
          throw new ArgumentException("lda must be at least max(1,k).", "lda");
        }
        if( lenA < lda * m ) {
          throw new ArgumentException("A must be at least lda * m.", "A");
        }
      }
      if ( (order == Order.ColumnMajor && transB == Transpose.NoTrans) || (order == Order.RowMajor && transB != Transpose.NoTrans) ) {
        if ( ldb < System.Math.Max(1, k) ) {
          throw new ArgumentException("ldb must be at least max(1,k).", "ldb");
        }
        if( lenB < ldb * n ) {
          throw new ArgumentException("B must be at least ldb * n.", "B");
        }
      } else {
        if ( ldb < System.Math.Max(1, n) ) {
          throw new ArgumentException( "ldb must be at least max(1,n).","ldb");
        }
        if( lenB < ldb * k ) {
          throw new ArgumentException("B must be at least ldb * k.", "B");
        }
      }
      if ( order == Order.ColumnMajor ) {
        if ( ldc < System.Math.Max(1, m) ) {
          throw new ArgumentException("ldc must be at least max(1,m).", "ldc");
        }
        if( lenC < ldc * n ) {
          throw new ArgumentException("B must be at least ldc * n.", "B");
        }
      } else {
        if ( ldc < System.Math.Max(1, n) ) {
          throw new ArgumentException("ldc must be at least max(1,n).", "ldc");
        }
        if( lenC < ldc * m ) {
          throw new ArgumentException("C must be at least ldc * m.", "C");
        }
      }
    }

    ///<summary>Compute the function of this class</summary>
    internal static void Compute( Order order, Transpose transA, Transpose transB, int m,int n, int k, float alpha, float[] A, int lda, float[] B, int ldb, float beta, float[]C, int ldc ){
      if ( transA == Transpose.ConjTrans ) {
        transA = Transpose.Trans;
      }
      if ( transB == Transpose.ConjTrans ) {
        transB = Transpose.Trans;
      }   
      ArgumentCheck(order, transA, transB, m, n, k, A, A.Length, lda, B, B.Length, ldb, C, C.Length, ldc);
      if ( alpha == 0.0 && beta == 1.0 ) {
        return;
      }
      
      dna_blas_sgemm(order, transA, transB, m, n, k, alpha, A, lda, B, ldb, beta, C, ldc);
    }

    internal static void Compute( Order order, Transpose transA, Transpose transB, int m,int n, int k, double alpha, double[] A, int lda, double[] B, int ldb, double beta, double[] C, int ldc ){
      if ( transA == Transpose.ConjTrans ) {
        transA = Transpose.Trans;
      }
      if ( transB == Transpose.ConjTrans ) {
        transB = Transpose.Trans;
      }   
      ArgumentCheck(order, transA, transB, m, n, k, A, A.Length, lda, B, B.Length, ldb, C, C.Length, ldc);
      if ( alpha == 0.0 && beta == 1.0 ) {
        return;
      }
      
      dna_blas_dgemm(order, transA, transB, m, n, k, alpha, A, lda, B, ldb, beta, C, ldc);
    }

    internal static void Compute( Order order, Transpose transA, Transpose transB, int m,int n, int k, ComplexFloat alpha, ComplexFloat[] A, int lda, ComplexFloat[] B, int ldb, ComplexFloat beta, ComplexFloat[] C, int ldc ){
      ArgumentCheck(order, transA, transB, m, n, k, A, A.Length, lda, B, B.Length, ldb, C, C.Length, ldc);
      if ( alpha == ComplexFloat.Zero && beta == ComplexFloat.One ) {
        return;
      }
      
      dna_blas_cgemm(order, transA, transB, m, n, k, ref alpha, A, lda, B, ldb, ref beta, C, ldc);
    }

    internal static void Compute( Order order, Transpose transA, Transpose transB, int m,int n, int k, Complex alpha, Complex[] A, int lda, Complex[] B, int ldb, Complex beta, Complex[] C, int ldc ){
      ArgumentCheck(order, transA, transB, m, n, k, A, A.Length, lda, B, B.Length, ldb, C, C.Length, ldc);
      if ( alpha == Complex.Zero && beta == Complex.One ) {
        return;
      }
      
      dna_blas_zgemm(order, transA, transB, m, n, k, ref alpha, A, lda, B, ldb, ref beta, C, ldc);
    }

    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_sgemm( Order order, Transpose transA, Transpose transB, int M,int N, int K, float alpha, [In]float[] A, int lda, [In]float[] B, int ldb, float beta, [In,Out]float[] C, int ldc );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dgemm( Order order, Transpose transA, Transpose transB, int M,int N, int K, double alpha, [In]double[] A, int lda, [In]double[] B, int ldb, double beta, [In,Out]double[] C, int ldc );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_cgemm( Order order, Transpose transA, Transpose transB, int M,int N, int K, ref ComplexFloat alpha, [In]ComplexFloat[] A, int lda, [In]ComplexFloat[] B, int ldb, ref ComplexFloat beta, [In,Out]ComplexFloat[] C, int ldc );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zgemm( Order order, Transpose transA, Transpose transB, int M,int N, int K, ref Complex alpha, [In]Complex[] A, int lda, [In]Complex[] B, int ldb, ref Complex beta, [In,Out]Complex[] C, int ldc );
  }
}
#endif
