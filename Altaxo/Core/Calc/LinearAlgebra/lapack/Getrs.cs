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
 * Getrs.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/
#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Getrs {
    private  Getrs() {}
    private static void ArgumentCheck(int n, int nrhs, Object A, int lda, Object B, int ldb, int[] ipiv) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( B == null ) {
        throw new ArgumentNullException("B","B cannot be null.");
      }
      if ( n<0 ) {
        throw new ArgumentException("n must be at least zero.", "n");
      }
      if ( nrhs<0 ) {
        throw new ArgumentException("nrhs must be at least zero.", "nrhs");
      }
      if ( lda < System.Math.Max(1,n) ) {
        throw new ArgumentException("lda must be at least max(1,m)", "lda");
      }
      if ( ldb < System.Math.Max(1,n) ) {
        throw new ArgumentException("ldb must be at least max(1,m)", "ldb");
      }
      if ( ipiv.Length < System.Math.Max(1,n) ) {
        throw new ArgumentException("The length of ipiv must be at least max(1,n)", "ipiv");
      }
    }
  
    internal static int Compute( Transpose trans, int n, int nrhs, float[] A, int lda, int[] ipiv, float[] B, int ldb ){
      ArgumentCheck(n, nrhs, A, lda,  B, ldb, ipiv);
      if ( trans == Transpose.ConjTrans ) {
        trans = Transpose.Trans;
      }
      
      return dna_lapack_sgetrs(trans,n,nrhs,A,lda,ipiv,B,ldb);
    }

    internal static int Compute( Transpose trans, int n, int nrhs, double[] A, int lda, int[] ipiv, double[] B, int ldb ){
      ArgumentCheck(n, nrhs, A, lda,  B, ldb, ipiv);
      if ( trans == Transpose.ConjTrans ) {
        trans = Transpose.Trans;
      }
      
      return dna_lapack_dgetrs(trans,n,nrhs,A,lda,ipiv,B,ldb);
    }

    internal static int Compute( Transpose trans, int n, int nrhs, ComplexFloat[] A, int lda, int[] ipiv, ComplexFloat[] B, int ldb ){
      ArgumentCheck(n, nrhs, A, lda,  B, ldb, ipiv);
      
      return dna_lapack_cgetrs(trans,n,nrhs,A,lda,ipiv,B,ldb);
    }

    internal static int Compute( Transpose trans, int n, int nrhs, Complex[] A, int lda, int[] ipiv, Complex[] B, int ldb ){
      ArgumentCheck(n, nrhs, A, lda,  B, ldb, ipiv);
      
      return dna_lapack_zgetrs(trans,n,nrhs,A,lda,ipiv,B,ldb);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sgetrs( Transpose trans, int n, int nrhs, [In,Out]float[] A, int lda, [In]int[] ipiv, [In,Out]float[] B, int ldb );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dgetrs( Transpose trans, int n, int nrhs, [In,Out]double[] A, int lda, [In]int[] ipiv, [In,Out]double[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cgetrs( Transpose trans, int n, int nrhs, [In,Out]ComplexFloat[] A, int lda, [In]int[] ipiv, [In,Out]ComplexFloat[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zgetrs( Transpose trans, int n, int nrhs, [In,Out]Complex[] A, int lda, [In]int[] ipiv, [In,Out]Complex[] B, int ldb );
  }
}
#endif