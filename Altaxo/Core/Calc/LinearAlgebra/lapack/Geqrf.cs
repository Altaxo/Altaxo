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
 * Geqrf.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/
#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Geqrf {
    private  Geqrf() {}                           
    private static void ArgumentCheck(int m, int n, Object A, int lda) {
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
  
    internal static int Compute( int m, int n, float[] A, int lda, out float[] tau ){
      ArgumentCheck(m, n, A, lda);
      tau = new float[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_sgeqrf(Configuration.BlockSize, m, n, A, lda, tau);
    }

    internal static int Compute( int m, int n, double[] A, int lda, out double[] tau  ){
      ArgumentCheck(m, n, A, lda);
      tau = new double[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_dgeqrf(Configuration.BlockSize, m, n, A, lda, tau);
    }

    internal static int Compute( int m, int n, ComplexFloat[] A, int lda, out ComplexFloat[] tau  ){
      ArgumentCheck(m, n, A, lda);
      tau = new ComplexFloat[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_cgeqrf(Configuration.BlockSize, m, n, A, lda, tau);
    }

    internal static int Compute( int m, int n, Complex[] A, int lda, out Complex[] tau  ){
      ArgumentCheck(m, n, A, lda);
      tau = new Complex[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_zgeqrf(Configuration.BlockSize, m, n, A, lda, tau);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sgeqrf( int block_size, int m, int n, [In,Out]float[] A, int lda, [In,Out]float[] tau );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dgeqrf( int block_size, int m, int n, [In,Out]double[] A, int lda, [In,Out]double[] tau );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cgeqrf( int block_size, int m, int n, [In,Out]ComplexFloat[] A, int lda, [In,Out]ComplexFloat[] tau );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zgeqrf( int block_size, int m, int n, [In,Out]Complex[] A, int lda, [In,Out]Complex[] tau );
  }
}
#endif