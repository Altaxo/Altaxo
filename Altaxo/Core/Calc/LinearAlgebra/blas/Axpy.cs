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
 * Axpy.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas  
{
  ///<summary>Computes a vector-scalar product and adds the result to a vector.</summary>
  ///<remarks>Calculates <c> y := a * x + y </c> </remarks>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Axpy 
  {
    private Axpy() {}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(int n, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) 
    {
      if( n < 0 ) 
      {
        throw new ArgumentException( "n must be zero or greater", "n" );
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException( "X", "X cannot be null.");
      }
      if ( Y == null ) 
      {
        throw new ArgumentNullException( "Y", "Y cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.","incx");
      }
      if( incy == 0 )
      {
        throw new ArgumentException("incy cannot be zero.", "incy");
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
    internal static void Compute(int n, float alpha, float[] X, int incx, float[] Y, int incy)
    {
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] += alpha* X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_saxpy(n, alpha, X, incx, Y, incy);
#endif
    }
    
    internal static void Compute(int n, double alpha, double[] X, int incx, double[] Y, int incy)
    {
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);

#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] += alpha* X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_daxpy(n, alpha, X, incx, Y, incy);
#endif
    }

    internal static void Compute(int n, ComplexFloat alpha, ComplexFloat[] X, int incx, ComplexFloat[] Y, int incy )
    { 
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] += alpha* X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_caxpy(n, ref alpha, X, incx, Y, incy);
#endif
    }


    internal static void Compute(int n, Complex alpha, Complex[] X, int incx, Complex[] Y, int incy )
    { 
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);

#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        Y[iy] += alpha* X[ix];
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_zaxpy(n, ref alpha, X, incx, Y, incy);
#endif
    }
    
#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_saxpy( int N, float alpha, [In]float[] X, int incX, [In,Out]float[] Y, int incY );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_daxpy( int N, double alpha, [In]double[] X, int incX, [In,Out]double[] Y, int incY );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_caxpy( int N, ref ComplexFloat alpha, [In]ComplexFloat[] X, int incX, [In,Out]ComplexFloat[] Y, int incY);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zaxpy( int N, ref Complex alpha, [In]Complex[] X, int incX, [In,Out]Complex[] Y, int incY);
#endif
  }
}
