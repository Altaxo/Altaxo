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
 * Unmbr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Unmbr {
    private  Unmbr() {}                           
    private static void ArgumentCheck(Vector vect, Side side, int m, int n, int k, Object A, int lda, Object tau, Object C, int ldc) {
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
      if( k < 0 ){
        throw new ArgumentException("k must be at least zero.", "k");
      }

      if( vect == Vector.Q ){
        if( side == Side.Left){
          if ( lda < System.Math.Max(1,m) ) {
            throw new ArgumentException("lda must be at least max(1,m)", "lda");
          }       
        }else{
          if ( lda < System.Math.Max(1,n) ) {
            throw new ArgumentException("lda must be at least max(1,n)", "lda");
          }       
        }
      }else{
        if( side == Side.Left){
          if ( lda < System.Math.Max(1,System.Math.Min(m,k))) {
            throw new ArgumentException("lda must be at least max(1,min(m,k))", "lda");
          }
        }else{
          if ( lda < System.Math.Max(1,System.Math.Min(n,k))) {
            throw new ArgumentException("lda must be at least max(1,min(n,k))", "lda");
          }
        }
      }
    
      if ( ldc < System.Math.Max(1,m)) {
        throw new ArgumentException("ldc must be at least max(1,m)", "ldc");
      } 
    }
    
    internal static int Compute( Vector vect, Side side, Transpose trans, int m, int n, int k, ComplexFloat[] A, int lda, ComplexFloat[] tau, ComplexFloat[] C, int ldc ){
      ArgumentCheck(vect,side, m, n, k, A, lda, tau, C, ldc);
      if( side == Side.Left){
        if (tau.Length < System.Math.Max(1, System.Math.Min(m,k))){
          throw new ArgumentException("tau must be at least max(1,k).");
        }
      }else{
        if (tau.Length < System.Math.Max(1, System.Math.Min(n,k))){
          throw new ArgumentException("tau must be at least max(1,k).");
        }
      }
      
      return dna_lapack_cunmbr(Configuration.BlockSize, vect, side, trans, m, n, k, A, lda, tau, C, ldc);
    }

    internal static int Compute( Vector vect, Side side, Transpose trans, int m, int n, int k, Complex[] A, int lda, Complex[] tau, Complex[] C, int ldc ){
      ArgumentCheck(vect,side, m, n, k, A, lda, tau, C, ldc);
      if( side == Side.Left){
        if (tau.Length < System.Math.Max(1, System.Math.Min(m,k))){
          throw new ArgumentException("tau must be at least max(1,k).");
        }
      }else{
        if (tau.Length < System.Math.Max(1, System.Math.Min(n,k))){
          throw new ArgumentException("tau must be at least max(1,k).");
        }
      }

      return dna_lapack_zunmbr(Configuration.BlockSize, vect, side, trans, m, n, k, A, lda, tau, C, ldc);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cunmbr( int block_size, Vector vect, Side side, Transpose trans, int m, int n, int k, [In,Out]ComplexFloat[] A, int lda, [In,Out]ComplexFloat[] tau, [In,Out]ComplexFloat[] C, int ldc  );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zunmbr( int block_size, Vector vect, Side side, Transpose trans, int m, int n, int k, [In,Out]Complex[] A, int lda, [In,Out]Complex[] tau, [In,Out]Complex[] C, int ldc   );
  }
}
#endif