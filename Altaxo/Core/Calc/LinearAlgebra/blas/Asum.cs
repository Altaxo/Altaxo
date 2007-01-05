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
 * Asum.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/
using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas 
{
  ///<summary>Computes the sum of the magnitudes.</summary>
  ///<remarks><para>for real numbers, <c>result = |x[1]| + |x[2]| +  ... + x[n]</c></para>
  ///<para>for complex numbers, <c>result = |real(x[1])| + |imag(x[1]| + ... + |real(x[n])| + |imag(x[n])|</c></para></remarks>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Asum 
  {
    private Asum() {}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(int n, object X, int incx, int lenX) 
    {
      if ( X == null ) 
      {
        throw new ArgumentNullException("X","X cannot be null.");
      }
      if ( lenX  < ( 1 + (n-1) * incx) ) 
      {
        throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
      }
    }

    ///<summary>Compute the function of this class</summary>
    internal static float Compute(int n, float[] X, int incx)
    {
      if ( n < 1 ) 
      {
        return 0;
      }
      if ( incx <= 0 ) 
      {
        return 0;
      }

      ArgumentCheck(n, X, incx, X.Length);
 
      float ret = 0;
#if MANAGED
      double temp = 0;
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        temp += System.Math.Abs(X[ix]);
        ix += incx;
      }
      ret = (float)temp;
#else
      ret = dna_blas_sasum(n, X, incx);
#endif
      return ret;
    }

    internal static double Compute(int n, double[] X, int incx)
    {
      if ( n < 1 ) 
      {
        return 0;
      }
      if ( incx <= 0 ) 
      {
        return 0;
      }
      ArgumentCheck(n, X, incx, X.Length);

      double ret = 0;
#if MANAGED
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        ret += System.Math.Abs(X[ix]);
        ix += incx;
      }
#else
      ret = dna_blas_dasum(n, X, incx);
#endif
      return ret;
    }

    internal static float Compute(int n, ComplexFloat[] X, int incx)
    {
      if( n < 1 ) 
      {
        return 0;
      }
      if ( incx <= 0 ) 
      {
        return 0;
      }
      ArgumentCheck(n, X, incx, X.Length);
      
      float ret = 0;

#if MANAGED
      int ix = 0;
      double temp = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        temp += System.Math.Abs(X[ix].Real) + System.Math.Abs(X[ix].Imag);
        ix += incx;
      }
      ret = (float)temp;
#else
      ret = dna_blas_scasum(n, X, incx);
#endif
      return ret;
    }

    internal static double Compute(int n, Complex[] X, int incx)
    {
      if ( n < 1 ) 
      {
        return 0;
      }
      if ( incx <= 0 ) 
      {
        return 0;
      }
      ArgumentCheck(n, X, incx, X.Length);
      
      double ret = 0;
#if MANAGED
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        ret += System.Math.Abs(X[ix].Real) + System.Math.Abs(X[ix].Imag);
        ix += incx;
      }
#else
      ret = dna_blas_dzasum(n, X, incx);
#endif
      return ret;
    }
#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern float dna_blas_sasum( int N, [In]float[] X, int incX);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern double dna_blas_dasum( int N, [In]double[] X, int incX);
  
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern float dna_blas_scasum( int N, [In]ComplexFloat[] X, int incX);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern double dna_blas_dzasum( int N, [In]Complex[] X, int incX);
#endif
  }
}
