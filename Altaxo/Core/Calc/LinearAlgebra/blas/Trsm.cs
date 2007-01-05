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
 * Trsm.cs
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
  internal sealed class Trsm {
    private Trsm() {}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck( Side side, int m, int n, object A, int lda, object B, int ldb ) {
      if ( A == null ) {
        throw new ArgumentNullException("A", "A cannot be null.");
      }
      if ( B == null ) {
        throw new ArgumentNullException("B", "B cannot be null.");
      }
      if ( m < 0) {
        throw new ArgumentException("m must be zero or greater", "m");
      }
      if ( n < 0) {
        throw new ArgumentException("n must be zero or greater", "n");
      }
    
      if (side == Side.Left) {
        if(lda < System.Math.Max(1,m)){
          throw new ArgumentException("lda must be at least max(1,m).", "lda");
        }
      }else{
        if(lda < System.Math.Max(1,n)){
          throw new ArgumentException("lda must be at least max(1,n).","lda");
        }
      }
      if(ldb < System.Math.Max(1,m)){
        throw new ArgumentException("lda must be at least max(1,m).","ldb");
      }

    }

    ///<summary>Compute the function of this class</summary>
    internal static void Compute( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, float alpha, float[] A, int lda, float[] B, int ldb ){
      if ( transA == Transpose.ConjTrans ) {
        transA = Transpose.Trans;
      }
      ArgumentCheck(side, m, n, A, lda, B, ldb);
      
      dna_blas_strsm(order, side, uplo, transA, diag, m, n, alpha, A, lda, B, ldb);
    }
    
    internal static void Compute( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, double alpha, double[] A, int lda, double[] B, int ldb ){
      if ( transA == Transpose.ConjTrans ) {
        transA = Transpose.Trans;
      }
      ArgumentCheck(side, m, n, A, lda, B, ldb);
      
      dna_blas_dtrsm(order, side, uplo, transA, diag, m, n, alpha, A, lda, B, ldb);
    }
    
    internal static void Compute( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, ComplexFloat alpha, ComplexFloat[] A, int lda, ComplexFloat[] B, int ldb ){
      ArgumentCheck(side, m, n, A, lda, B, ldb);
      
      dna_blas_ctrsm(order, side, uplo, transA, diag, m, n, ref alpha, A, lda, B, ldb);
    }
    
    internal static void Compute( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, Complex alpha, Complex[] A, int lda, Complex[] B, int ldb ){
      ArgumentCheck(side, m, n, A, lda, B, ldb);
      
      dna_blas_ztrsm(order, side, uplo, transA, diag, m, n, ref alpha, A, lda, B, ldb);
    }

    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_strsm( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, float alpha, [In]float[] A, int lda, [In,Out]float[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dtrsm( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, double alpha, [In]double[] A, int lda, [In,Out]double[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_ctrsm( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, ref ComplexFloat alpha, [In]ComplexFloat[] A, int lda, [In,Out]ComplexFloat[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_ztrsm( Order order, Side side, UpLo uplo, Transpose transA, Diag diag, int m,int n, ref Complex alpha, [In]Complex[] A, int lda, [In,Out]Complex[] B, int ldb );
  }
}
#endif 