#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
 * Getrf.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/
#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Getrf {
    private  Getrf() {}
    private static void ArgumentCheck(int m, int n, object A, int lda) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( m<0 ) {
        throw new ArgumentException("m must be at least zero.", "m");
      }
      if ( n<0 ) {
        throw new ArgumentException("n must be at least zero.", "n");
      }
      if ( lda < System.Math.Max(1,m) ) {
        throw new ArgumentException("lda must be at least max(1,m)", "lda");
      }
    }
  
    internal static int Compute( int m, int n, float[] A, int lda, out int[] ipiv ){
      ArgumentCheck(m,n,A,lda);
      ipiv = new int[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_sgetrf(m,n,A,lda,ipiv);
    }

    internal static int Compute( int m, int n, double[] A, int lda, out int[] ipiv ){
      ArgumentCheck(m,n,A,lda);
      ipiv = new int[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_dgetrf(m,n,A,lda,ipiv);
    }

    internal static int Compute( int m, int n, ComplexFloat[] A, int lda, out int[] ipiv ){
      ArgumentCheck(m,n,A,lda);
      ipiv = new int[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_cgetrf(m,n,A,lda,ipiv);
    }

    internal static int Compute( int m, int n, Complex[] A, int lda, out int[] ipiv ){
      ArgumentCheck(m,n,A,lda);
      ipiv = new int[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_zgetrf(m,n,A,lda,ipiv);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sgetrf( int m, int n, [In,Out]float[] A, int lda, [In,Out]int[] ipiv );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dgetrf( int m, int n, [In,Out]double[] A, int lda, [In,Out]int[] ipiv );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cgetrf( int m, int n, [In,Out]ComplexFloat[] A, int lda, [In,Out]int[] ipiv );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zgetrf( int m, int n, [In,Out]Complex[] A, int lda, [In,Out]int[] ipiv );
  }
}
#endif