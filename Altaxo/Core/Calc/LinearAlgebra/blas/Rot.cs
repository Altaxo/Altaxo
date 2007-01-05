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
 * Rot.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Performs rotation of points in the plane.</summary>
  ///<remarks> Returns: <c>
  /// x(i) = c* x(i) + s* y(i)
  /// y(i) = c* y(i) - s* x(i) </c></remarks>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Rot
  {
    private Rot(){}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(int n, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) 
    {
      if ( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater","n");
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException("X", "X cannot be null.");
      } 
      if ( Y == null ) 
      {
        throw new ArgumentNullException("Y", "Y cannot be null.");
      }
      if ( incx == 0 ) 
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      if ( incy == 0 ) 
      {
        throw new ArgumentException("incy cannot be zero.", "incy");
      }
      incx = System.Math.Abs(incx);
      incy = System.Math.Abs(incy);
      if ( lenX < ( 1 + (n-1) * incx) ) 
      {
        throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
      }
      if ( lenY < ( 1 + (n-1) * incy) ) 
      {
        throw new ArgumentException("The dimension of Y must be at least 1 + (n-1) * incy.");
      }
    }
    ///<summary>Compute the function of this class</summary>
    internal static void Compute( int n, float[] X, int incx, float[] Y, int incy, float c, float s ) 
    { 
 
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        float temp = X[ix];
        X[ix] = c*X[ix] + s*Y[iy];
        Y[iy] = c*Y[iy] - s*temp;
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_srot(n, X, incx, Y, incy, c, s);
#endif
    }

    internal static void Compute( int n, double[] X, int incx, double[] Y, int incy, double c, double s ) 
    {
      ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
      
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        double temp = X[ix];
        X[ix] = c*X[ix] + s*Y[iy];
        Y[iy] = c*Y[iy] - s*temp;
        ix += incx;
        iy += incy;
      }
#else
      dna_blas_drot(n, X, incx, Y, incy, c, s);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_srot( int N, [In,Out]float[] X, int incX, [In,Out]float[] Y, int incY, float c, float s );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_drot( int N, [In,Out]double[] X, int incX, [In,Out]double[] Y, int incY, double c, double s );
#endif
  }
}
