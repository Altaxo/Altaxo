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
 * Getri.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/
#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Getri {
    private  Getri() {}
    private static void ArgumentCheck( int n, object A, int lda, int[] ipiv) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( n<0 ) {
        throw new ArgumentException("n must be at least zero.", "n");
      }
      if ( lda < System.Math.Max(1,n) ) {
        throw new ArgumentException("lda must be at least max(1,n)", "lda");
      }
      if ( ipiv.Length < System.Math.Max(1,n) ) {
        throw new ArgumentException("The length of ipiv must be at least max(1,n)", "ipiv");
      }
    }
    internal static int Compute( int n, float[] A, int lda, int[] ipiv ){
      ArgumentCheck(n,A,lda, ipiv);
      
      return dna_lapack_sgetri(Configuration.BlockSize, n,A,lda,ipiv);
    }

    internal static int Compute( int n, double[] A, int lda, int[] ipiv ){
      ArgumentCheck(n,A,lda, ipiv);
      
      return dna_lapack_dgetri(Configuration.BlockSize, n,A,lda,ipiv);
    }

    internal static int Compute( int n, ComplexFloat[] A, int lda, int[] ipiv ){
      ArgumentCheck(n,A,lda,ipiv);
      
      return dna_lapack_cgetri(Configuration.BlockSize, n,A,lda,ipiv);
    }

    internal static int Compute( int n, Complex[] A, int lda, int[] ipiv ){
      ArgumentCheck(n,A,lda,ipiv);
      
      return dna_lapack_zgetri(Configuration.BlockSize, n,A,lda,ipiv);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sgetri( int block_size, int n, [In,Out]float[] A, int lda, [In]int[] ipiv );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dgetri( int block_size, int n, [In,Out]double[] A, int lda, [In]int[] ipiv );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cgetri( int block_size, int n, [In,Out]ComplexFloat[] A, int lda, [In]int[] ipiv );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zgetri( int block_size, int n, [In,Out]Complex[] A, int lda, [In]int[] ipiv );
  }
}
#endif