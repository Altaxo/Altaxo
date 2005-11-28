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
 * Sdot.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Computes a vector-vector dot product with extended precision</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Sdot 
  {
    private Sdot(){}
    internal static float Compute(int n, float alpha, float[] X, int incx, float[] Y, int incy)
    {
      if( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater", "n");
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException("X", "X cannot be null.");
      }
      if ( Y == null ) 
      {
        throw new ArgumentNullException("Y", "Y cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      if( incy == 0 )
      {
        throw new ArgumentException("incy cannot be zero.", "incy");
      }
      incx = System.Math.Abs(incx);
      incy = System.Math.Abs(incy);
      if( X.Length < ( 1 + (n-1) * incx) )
      {
        throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
      }
      if( Y.Length < ( 1 + (n-1) * incy) )
      {
        throw new ArgumentException("The dimension of Y must be a least 1 + (n-1) * incy.");
      }
      
      double ret = alpha;
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        ret += ((double)X[ix] * (double)Y[iy]);
        ix += incx;
        iy += incy;
      }
#else
      ret = dna_blas_sdsdot(n, alpha, X, incx, Y, incy);
#endif
      return (float)ret ;
    }
  
    internal static double Compute(int n, float[] X, int incx, float[] Y, int incy)
    {
      if( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater","n");
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException("X cannot be null.","X");
      }
      if ( Y == null ) 
      {
        throw new ArgumentNullException("Y cannot be null.","Y");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.","incx");
      }
      if( incy == 0 )
      {
        throw new ArgumentException("incy cannot be zero.","incy");
      }
      incx = System.Math.Abs(incx);
      incy = System.Math.Abs(incy);
      if( X.Length < ( 1 + (n-1) * incx) )
      {
        throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
      }
      if( Y.Length < ( 1 + (n-1) * incy) )
      {
        throw new ArgumentException("The dimension of Y must be a least 1 + (n-1) * incy.");
      }
      
      double ret = 0;
#if MANAGED
      int ix = 0;
      int iy = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        ret += ((double)X[ix] * (double)Y[iy]);
        ix += incx;
        iy += incy;
      }
#else
      ret = dna_blas_dsdot(n, X, incx, Y, incy);
#endif
      return ret;
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern float dna_blas_sdsdot( int N, float alpha, [In]float[] X, int incX, [In]float[] Y, int incY );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern double dna_blas_dsdot( int N, [In]float[] X, int incX, [In]float[] Y, int incY );
#endif
  }
}
