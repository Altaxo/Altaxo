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
 * Copy.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary> Copies vector to another vector.</summary>
  ///<remarks> Performs <c> y = x </c> in native code</remarks>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Copy 
  {
    private Copy() {}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(int n, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) 
    {
      // no exception throwing on n - according to the MKL doc - if n is not positive, parameters will remain unaltered
      // equally tested this on the ATLAS dll - same result when n < 0
      if ( X == null ) 
      {
        throw new ArgumentException("X cannot be null.", "X");
      }
      if ( Y == null ) 
      {
        throw new ArgumentException("Y cannot be null.", "Y");
      }
      if ( incx == 0) 
      {
        throw new ArgumentNullException("incx","incx cannot be zero.");
      }
      if ( incy == 0) 
      {
        throw new ArgumentNullException("incy","incy cannot be zero.");
      }
      incx = System.Math.Abs(incx);
      incy = System.Math.Abs(incy);
      if ( lenX < ( 1 + (n-1) * incx ) ) 
      {
        throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
      }
      if ( lenY < ( 1 + (n-1) * incy ) ) 
      {
        throw new ArgumentException("The dimension of Y must be at least 1 + (n-1) * incy.");
      }
    }

    ///<summary>Compute the function of this class</summary>
    internal static void Compute(int n, float[] X, int incx, float[] Y, int incy)
    {
      if ( n < 0 )
      { 
        return;
      }
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] = X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_scopy(n, X, incx, Y, incy);
#endif
    }


    internal static void Compute(int n, double[] X, int incx, double[] Y, int incy)
    {
      // if n < 0, Y will remain unaltered
      if ( n < 0 )
      { 
        return;
      }
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] = X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_dcopy(n, X, incx, Y, incy);
#endif
    }

    internal static void Compute(int n, ComplexFloat[] X, int incx, ComplexFloat[] Y, int incy)
    {
      if ( n < 0 )
      { 
        return;
      }
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] = X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_ccopy(n, X, incx, Y, incy);
#endif
    }

    internal static void Compute(int n, Complex[] X, int incx, Complex[] Y, int incy)
    {
      if ( n < 0 )
      { 
        return;
      }
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] = X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_zcopy(n, X, incx, Y, incy);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_scopy( int N, [In]float[] X, int incX, [In,Out]float[] Y, int incY );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dcopy( int N, [In]double[] X, int incX, [In,Out]double[] Y, int incY );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_ccopy( int N, [In]ComplexFloat[] X, int incX, [In,Out]ComplexFloat[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zcopy( int N, [In]Complex[] X, int incX, [In,Out]Complex[] Y, int incY);
#endif
  }
}
