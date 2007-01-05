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
 * Gebrd.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Gebrd {
    private  Gebrd() {}                           
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
    internal static int Compute( int m, int n, float[] A, int lda, out float[] d, out float[] e, out float[] tauq, out float[] taup ){
      ArgumentCheck(m, n, A, lda);
      d = new float[System.Math.Max(1, System.Math.Min(m,n))];
      e = new float[System.Math.Max(1, System.Math.Min(m,n))];
      tauq = new float[System.Math.Max(1, System.Math.Min(m,n))];
      taup = new float[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_sgebrd(Configuration.BlockSize, m, n, A, lda, d, e, tauq, taup);
    }

    internal static int Compute( int m, int n, double[] A, int lda, out double[] d, out double[] e, out double[] tauq, out double[] taup ){
      ArgumentCheck(m, n, A, lda);
      d = new double[System.Math.Max(1, System.Math.Min(m,n))];
      e = new double[System.Math.Max(1, System.Math.Min(m,n))];
      tauq = new double[System.Math.Max(1, System.Math.Min(m,n))];
      taup = new double[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_dgebrd(Configuration.BlockSize, m, n, A, lda, d, e, tauq, taup);
    }

    internal static int Compute( int m, int n, ComplexFloat[] A, int lda, out float[] d, out float[] e, out ComplexFloat[] tauq, out ComplexFloat[] taup ){
      ArgumentCheck(m, n, A, lda);
      d = new float[System.Math.Max(1, System.Math.Min(m,n))];
      e = new float[System.Math.Max(1, System.Math.Min(m,n))];
      tauq = new ComplexFloat[System.Math.Max(1, System.Math.Min(m,n))];
      taup = new ComplexFloat[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_cgebrd(Configuration.BlockSize, m, n, A, lda, d, e, tauq, taup);
    }

    internal static int Compute( int m, int n, Complex[] A, int lda, out double[] d, out double[] e, out Complex[] tauq, out Complex[] taup ){
      ArgumentCheck(m, n, A, lda);
      d = new double[System.Math.Max(1, System.Math.Min(m,n))];
      e = new double[System.Math.Max(1, System.Math.Min(m,n))];
      tauq = new Complex[System.Math.Max(1, System.Math.Min(m,n))];
      taup = new Complex[System.Math.Max(1, System.Math.Min(m,n))];
      
      return dna_lapack_zgebrd(Configuration.BlockSize, m, n, A, lda, d, e, tauq, taup);
    }
                                                     
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sgebrd( int block_size, int m, int n, [In,Out]float[] A, int lda, [In,Out]float[] d, [In,Out]float[] e, [In,Out]float[] tauq, [In,Out]float[] taup );
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dgebrd( int block_size, int m, int n, [In,Out]double[] A, int lda, [In,Out]double[] d, [In,Out]double[] e, [In,Out]double[] tauq, [In,Out]double[] taup );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cgebrd( int block_size, int m, int n, [In,Out]ComplexFloat[] A, int lda, [In,Out]float[] d, [In,Out]float[] e, [In,Out]ComplexFloat[] tauq, [In,Out]ComplexFloat[] taup );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zgebrd( int block_size, int m, int n, [In,Out]Complex[] A, int lda, [In,Out]double[] d, [In,Out]double[] e, [In,Out]Complex[] tauq, [In,Out]Complex[] taup );
  }
}
#endif