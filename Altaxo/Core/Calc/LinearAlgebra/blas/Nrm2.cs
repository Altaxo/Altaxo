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
 * Nrm2.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Computes the Euclidean norm of a vector</summary>
  ///<remarks> Returns <c>result = ||x||</c></remarks>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Nrm2 
  {
    private Nrm2() {}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(int n, object X, int lenX, ref int incx) 
    {
      if ( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater","n");
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException("X", "X cannot be null.");
      }
      if ( incx == 0 ) 
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      incx = System.Math.Abs(incx);
      if ( lenX < ( 1 + (n-1) * incx) ) 
      {
        throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
      }
    }

    ///<summary>Compute the function of this class</summary>
    internal static float Compute( int n, float[] X, int incx )
    { 
      ArgumentCheck(n, X, X.Length, ref incx);
      
      float ret = 0;
#if MANAGED
      float temp = 0;
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        temp += (X[ix] * X[ix]);
        ix += incx;
      }
      ret = (float)System.Math.Sqrt(temp);
#else
      ret = dna_blas_snrm2(n, X, incx);
#endif
      return ret;
    }
 
    internal static double Compute( int n, double[] X, int incx )
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
      double ret = 0;
#if MANAGED
      double temp = 0;
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        temp += (X[ix] * X[ix]);
        ix += incx;
      }
      ret = System.Math.Sqrt(temp);
#else
      ret = dna_blas_dnrm2(n, X, incx);
#endif
      return ret;
    }

    internal static float Compute( int n, ComplexFloat[] X, int incx )
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
      float ret = 0;
#if MANAGED
      ComplexFloat temp = new ComplexFloat(0);
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        temp += (X[ix]*ComplexMath.Conjugate(X[ix]));
        ix += incx;
      }
      ret = (float)System.Math.Sqrt(ComplexMath.Absolute(temp));
#else
      ret = dna_blas_scnrm2(n, X, incx);
#endif
      return ret;
    }
 
    internal static double Compute( int n, Complex[] X, int incx )
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
      double ret = 0;
#if MANAGED
      Complex temp = new Complex(0);
      int ix = 0;
      for ( int i = 0; i < n; ++i ) 
      {
        temp += (X[ix] * ComplexMath.Conjugate(X[ix]));
        ix += incx;
      }
      ret = System.Math.Sqrt(ComplexMath.Absolute(temp));
#else
      ret = dna_blas_dznrm2(n, X, incx);
#endif
      return ret;
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern float dna_blas_snrm2( int N, [In]float[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern double dna_blas_dnrm2( int N, [In]double[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern float dna_blas_scnrm2( int N, [In]ComplexFloat[] X, int incX);

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern double dna_blas_dznrm2( int N, [In]Complex[] X, int incX);
#endif
  }
}
