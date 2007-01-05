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
 * Orgqr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Orgqr {
    private  Orgqr() {}                           
    private static void ArgumentCheck( int m, int n, int k, Object A, int lda, Object tau ) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( tau == null ) {
        throw new ArgumentNullException("tau","tau cannot be null.");
      }
      if ( m<0 ) {
        throw new ArgumentException("m must be at least zero.", "m");
      }
      if( n < 0 || n > m ){
        throw new ArgumentException("n must be positive and less than or equal to m.");
      }
      if( k < 0 || k > n ){
        throw new ArgumentException("k must be positive and less than or equal to n.");
      }
      if ( lda < System.Math.Max(1,m) ) {
        throw new ArgumentException("lda must be at least max(1,m)", "lda");
      }
    }

    internal static int Compute( int m, int n, int k, float[] A, int lda, float[] tau ){
      ArgumentCheck(m, n, k, A, lda, tau);
      return dna_lapack_sorgqr(Configuration.BlockSize, m, n, k, A, lda, tau);
    }

    internal static int Compute( int m, int n, int k, double[] A, int lda, double[] tau ){
      ArgumentCheck(m, n, k, A, lda, tau);
      return dna_lapack_dorgqr(Configuration.BlockSize, m, n, k, A, lda, tau);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sorgqr( int block_size, int m, int n, int k, [In,Out]float[] A, int lda, [In,Out]float[] tau );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dorgqr( int block_size, int m, int n, int k, [In,Out]double[] A, int lda, [In,Out]double[] tau );
  }
}
#endif