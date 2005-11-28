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
 * Geqp3.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Geqp3 {
    private  Geqp3() {}                           
    private static void ArgumentCheck(int m, int n, Object A, int lda, int[] jpvt) {
      if ( A == null ) {
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if ( m<0 ) {
        throw new ArgumentException("m must be at least zero.", "m");
      }
      if ( n<0 ) {
        throw new ArgumentException("n must be at least zero.","n");
      }
      if ( lda < System.Math.Max(1,m) ) {
        throw new ArgumentException("lda must be at least max(1,m)", "lda");
      }
      if (  jpvt.Length < System.Math.Max(1,n) ) {
        throw new ArgumentException("jpvt must be at least max(1,m)", "jpvt");
      }
    }
  
    internal static int Compute( int m, int n, float[] A, int lda, int[] jpvt, out float[] tau ){
      ArgumentCheck(m, n, A, lda, jpvt);
      tau = new float[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_sgeqp3(m,n,A,lda,jpvt,tau);
    }

    internal static int Compute( int m, int n, double[] A, int lda, int[] jpvt, out double[] tau  ){
      ArgumentCheck(m, n, A, lda, jpvt);
      tau = new double[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_dgeqp3(m,n,A,lda,jpvt,tau);
    }

    internal static int Compute( int m, int n, ComplexFloat[] A, int lda, int[] jpvt, out ComplexFloat[] tau  ){
      ArgumentCheck(m, n, A, lda, jpvt);
      tau = new ComplexFloat[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_cgeqp3(m,n,A,lda,jpvt,tau);
    }

    internal static int Compute( int m, int n, Complex[] A, int lda, int[] jpvt, out Complex[] tau  ){
      ArgumentCheck(m, n, A, lda, jpvt);
      tau = new Complex[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_zgeqp3(m,n,A,lda,jpvt,tau);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sgeqp3( int m, int n, [In,Out]float[] A, int lda, [In,Out]int[] jpvt, [In,Out]float[] tau );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dgeqp3( int m, int n, [In,Out]double[] A, int lda, [In,Out]int[] jpvt, [In,Out]double[] tau );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cgeqp3( int m, int n, [In,Out]ComplexFloat[] A, int lda, [In,Out]int[] jpvt, [In,Out]ComplexFloat[] tau );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zgeqp3( int m, int n, [In,Out]Complex[] A, int lda, [In,Out]int[] jpvt, [In,Out]Complex[] tau );
  }
}
#endif