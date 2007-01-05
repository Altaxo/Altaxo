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
 * Ungbr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Ungbr {
    private  Ungbr() {}                           
    private static void ArgumentCheck( Vector vect, int m, int n, int k, Object A, int lda, Object tau ) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( tau == null ) {
        throw new ArgumentNullException("tau","tau cannot be null.");
      }
      if ( m<0 ) {
        throw new ArgumentException("m must be at least zero.", "m");
      }
      if ( n<0 ) {
        throw new ArgumentException("n must be at least zero.", "n");
      }

      if ( k<0 ) {
        throw new ArgumentException("k must be at least zero.", "k");
      }

      if( vect == Vector.Q ){
        if( m > k ){
          if( k > n ){
            throw new ArgumentException("k must be lest than n.", "k");
          }
          if( n > m ){
            throw new ArgumentException("n must be lest than m.", "m");
          }
        }else{
          if( m != n ){
            throw new ArgumentException("m and n must be equal.");
          }
        }
      }else{
        if( n > k ){
          if( k > m ){
            throw new ArgumentException("k must be lest than m.", "k");
          }
          if( m > n ){
            throw new ArgumentException("m must be lest than n.", "m");
          }
        }else{
          if( m != n ){
            throw new ArgumentException("m and n must be equal.");
          }
        }
      }
      if ( lda < System.Math.Max(1,m) ) {
        throw new ArgumentException("lda must be at least max(1,m)", "lda");
      }
    }
    
    internal static int Compute( Vector vect, int m, int n, int k, ComplexFloat[] A, int lda, ComplexFloat[] tau ){
      ArgumentCheck(vect, m, n, k, A, lda, tau);
      if (tau.Length < System.Math.Max(1, System.Math.Min(m, k))){
        throw new ArgumentException("tau must be at least max(1,min(m,k)).");
      }
      
      return dna_lapack_cungbr(Configuration.BlockSize, vect, m, n, k, A, lda, tau);
    }

    internal static int Compute( Vector vect, int m, int n, int k, Complex[] A, int lda, Complex[] tau ){
      ArgumentCheck(vect, m, n, k, A, lda, tau);
      if (tau.Length < System.Math.Max(1, System.Math.Min(m, k))){
        throw new ArgumentException("tau must be at least max(1,min(m,k)).");
      }
      
      return dna_lapack_zungbr(Configuration.BlockSize, vect, m, n, k, A, lda, tau);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cungbr( int block_size, Vector vect, int m, int n, int k, [In,Out]ComplexFloat[] A, int lda, [In,Out]ComplexFloat[] tau );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zungbr( int block_size, Vector vect, int m, int n, int k, [In,Out]Complex[] A, int lda, [In,Out]Complex[] tau );
  }
}
#endif