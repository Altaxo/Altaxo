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
 * Ormqr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Ormqr {
    private  Ormqr() {}                           
    private static void ArgumentCheck(Side side, int m, int n, int k, Object A, int lda, Object tau, Object C, int ldc) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( tau == null ) {
        throw new ArgumentNullException("tau","tau cannot be null.");
      }
      if ( C == null ) {
        throw new ArgumentNullException("C","C cannot be null.");
      }
      if ( m<0 ) {
        throw new ArgumentException("m must be at least zero.", "m");
      }
      if ( n<0 ) {
        throw new ArgumentException("n must be at least zero.", "n");
      }
      if( side == Side.Left ){
        if( k < 0 || k > m ){
          throw new ArgumentException("k must be positive and less than or equal to m.", "k");
        }
      }else{
        if( k < 0 || k > n ){
          throw new ArgumentException("k must be positive and less than or equal to n.", "k");
        }
      }
      if( side == Side.Left ){
        if ( lda < System.Math.Max(1,m) ) {
          throw new ArgumentException("lda must be at least max(1,m)", "lda");
        }
      }else{
        if ( lda < System.Math.Max(1,n) ) {
          throw new ArgumentException("lda must be at least max(1,n)", "lda");
        }
      }
      if ( ldc < System.Math.Max(1,m) ) {
        throw new ArgumentException("ldc must be at least max(1,m)", "ldc");
      }     
    }
    
    internal static int Compute( Side side, Transpose trans, int m, int n, int k, float[] A, int lda, float[] tau, float[] C, int ldc ){
      ArgumentCheck(side, m, n, k, A, lda, tau, C, ldc);
      if (tau.Length < System.Math.Max(1, k) ){
        throw new ArgumentException("tau must be at least max(1,k).");
      }
      
      return dna_lapack_sormqr(Configuration.BlockSize, side, trans, m, n, k, A, lda, tau, C, ldc);
    }

    internal static int Compute( Side side, Transpose trans, int m, int n, int k, double[] A, int lda, double[] tau, double[] C, int ldc  ){
      ArgumentCheck(side, m, n, k, A, lda, tau, C, ldc);
      if (tau.Length < System.Math.Max(1, k) ){
        throw new ArgumentException("tau must be at least max(1,k).");
      }
      
      return dna_lapack_dormqr(Configuration.BlockSize, side, trans, m, n, k, A, lda, tau, C, ldc);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sormqr( int block_size, Side side, Transpose trans, int m, int n, int k, [In,Out]float[] A, int lda, [In,Out]float[] tau, [In,Out]float[] C, int ldc  );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dormqr( int block_size, Side side, Transpose trans, int m, int n, int k, [In,Out]double[] A, int lda, [In,Out]double[] tau, [In,Out]double[] C, int ldc   );
  }
}
#endif