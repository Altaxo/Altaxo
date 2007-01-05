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
 * Bdsqr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Bdsqr {
    private  Bdsqr() {}                           
    private static void ArgumentCheck(int n, int ncvt, int nru, int ncc, object d, object e, object vt, int ldvt, object u, int ldu, object c, int ldc) {
      if ( d == null ) {
        throw new ArgumentNullException("d cannot be null.");
      }
      if ( e == null ) {
        throw new ArgumentNullException("e cannot be null.");
      }
      if ( ncvt > 0 && vt == null ) {
        throw new ArgumentNullException("vt cannot be null.");
      }
      if ( nru > 0 &&u == null ) {
        throw new ArgumentNullException("u cannot be null.");
      }
      if ( ncc > 0 && c == null ) {
        throw new ArgumentNullException("c cannot be null.");
      }

      if( ncvt > 0 ){
        if( ldvt < System.Math.Max(1,n) ){
          throw new ArgumentException("ldvt must be at least max(1,n).", "ldvt");
        }
      }else{
        if( ldvt < 1 ){
          throw new ArgumentException("ldvt must be at least 1.", "ldvt");
        }
      }
      if( ldu < System.Math.Max(1,nru) ){
        throw new ArgumentException("ldu must be at least max(1,n).", "ldu");
      }
      if( ncc > 0 ){
        if( ldc < System.Math.Max(1,n) ){
          throw new ArgumentException("ldc must be at least max(1,n).", "ldc");
        }
      }else{
        if( ldc < 1 ){
          throw new ArgumentException("ldc must be at least 1.", "ldc");
        }
      }

      if ( n < 0 ) {
        throw new ArgumentException("n must be at least zero.", "n");
      }
      if ( ncvt < 0 ) {
        throw new ArgumentException("ncvt must be at least zero.", "ncvt");
      }
      if ( nru < 0 ) {
        throw new ArgumentException("nru must be at least zero.", "nru");
      }
      if ( ncc < 0 ) {
        throw new ArgumentException("ncc must be at least zero.", "ncc");
      }

    }
  
    internal static int Compute( int n, int ncvt, int nru, int ncc, float[] d, float[] e, float[] vt, int ldvt, float[] u, int ldu, float[] c, int ldc ){
      ArgumentCheck(n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
      if( d.Length < 1 ){
        throw new ArgumentException("The length of d must be at least 1.", "d");
      }
      if( e.Length < 1 ){
        throw new ArgumentException("The length of e must be at least 1.", "e");
      }
      if( ncvt !=0 && vt != null && vt.Length < ldvt*ncvt ){
        throw new ArgumentException("The length of vt must be at least ldvt*ncvt.", "vt");
      }
      if( nru !=0 && u != null && u.Length < ldu*n ){
        throw new ArgumentException("The length of nru must be at least lut*n.", "nru");
      }
      if( ncc !=0 && c != null && c.Length < ldc*ncc ){
        throw new ArgumentException("The length of c must be at least ldc*ncc.", "c");
      }
      return dna_lapack_sbdsqr(UpLo.Upper, n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
    }

    internal static int Compute( int n, int ncvt, int nru, int ncc, double[] d, double[] e, double[] vt, int ldvt, double[] u, int ldu, double[] c, int ldc ){
      ArgumentCheck(n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
      if( d.Length < 1 ){
        throw new ArgumentException("The length of d must be at least 1.", "d");
      }
      if( e.Length < 1 ){
        throw new ArgumentException("The length of e must be at least 1.","e");
      }
      if( ncvt !=0 && vt != null && vt.Length < ldvt*ncvt ){
        throw new ArgumentException("The length of vt must be at least ldvt*ncvt.", "vt");
      }
      if( nru !=0 && u != null && u.Length < ldu*n ){
        throw new ArgumentException("The length of nru must be at least lut*n.", "nru");
      }
      if( ncc !=0 && c != null && c.Length < ldc*ncc ){
        throw new ArgumentException("The length of c must be at least ldc*ncc.", "c");
      }
      
      return dna_lapack_dbdsqr(UpLo.Upper, n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
    }

    internal static int Compute( int n, int ncvt, int nru, int ncc, float[] d, float[] e, ComplexFloat[] vt, int ldvt, ComplexFloat[] u, int ldu, ComplexFloat[] c, int ldc  ){
      ArgumentCheck(n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
      if( d.Length < 1 ){
        throw new ArgumentException("The length of d must be at least 1.", "d");
      }
      if( e.Length < 1 ){
        throw new ArgumentException("The length of e must be at least 1.", "e");
      }
      if( ncvt !=0 && vt != null && vt.Length < ldvt*ncvt ){
        throw new ArgumentException("The length of vt must be at least ldvt*ncvt.", "vt");
      }
      if( nru !=0 && u != null && u.Length < ldu*n ){
        throw new ArgumentException("The length of nru must be at least lut*n.", "nru");
      }
      if( ncc !=0 && c != null && c.Length < ldc*ncc ){
        throw new ArgumentException("The length of c must be at least ldc*ncc.", "c");
      }
      
      return dna_lapack_cbdsqr(UpLo.Upper, n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
    }

    internal static int Compute( int n, int ncvt, int nru, int ncc, double[] d, double[] e, Complex[] vt, int ldvt, Complex[] u, int ldu, Complex[] c, int ldc  ){
      ArgumentCheck(n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
      if( d.Length < 1 ){
        throw new ArgumentException("The length of d must be at least 1.", "d");
      }
      if( e.Length < 1 ){
        throw new ArgumentException("The length of e must be at least 1.", "e");
      }
      if( ncvt !=0 && vt != null && vt.Length < ldvt*ncvt ){
        throw new ArgumentException("The length of vt must be at least ldvt*ncvt.", "vt");
      }
      if( nru !=0 && u != null && u.Length < ldu*n ){
        throw new ArgumentException("The length of nru must be at least lut*n.", "nru");
      }
      if( ncc !=0 && c != null && c.Length < ldc*ncc ){
        throw new ArgumentException("The length of c must be at least ldc*ncc.", "c");
      }
      
      return dna_lapack_zbdsqr(UpLo.Upper, n, ncvt, nru, ncc, d, e, vt, ldvt, u, ldu, c, ldc);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_sbdsqr( UpLo uplo, int n, int ncvt, int nru, int ncc, [In,Out]float[] d, [In,Out]float[] e, [In,Out]float[] vt, int ldvt, [In,Out]float[] u, int ldu, [In,Out]float[] c, int ldc );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dbdsqr( UpLo uplo, int n, int ncvt, int nru, int ncc, [In,Out]double[] d, [In]double[] e, [In,Out]double[] vt, int ldvt, [In,Out]double[] u, int ldu, [In,Out]double[] c, int ldc );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cbdsqr( UpLo uplo, int n, int ncvt, int nru, int ncc, [In,Out]float[] d, [In,Out]float[] e, [In,Out]ComplexFloat[] vt, int ldvt, [In,Out]ComplexFloat[] u, int ldu, [In,Out]ComplexFloat[] c, int ldc );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zbdsqr( UpLo uplo, int n, int ncvt, int nru, int ncc, [In,Out]double[] d, [In,Out]double[] e, [In,Out]Complex[] vt, int ldvt, [In,Out]Complex[] u, int ldu, [In,Out]Complex[] c, int ldc );
  }
}
#endif