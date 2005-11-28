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
 * Scal.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Computes a vector by a scalar product.</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Scal 
  {
    private Scal(){}
    ///<summary>Check arguments so that errors don't occur in native code</summary>
    private static void ArgumentCheck(int n, object X, int lenX, ref int incx) 
    {
      if ( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater.","n");
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException("X", "X cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      incx = System.Math.Abs(incx);
      if ( lenX < ( 1 + (n-1) * incx ) ) 
      {
        throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
      }
    }

    ///<summary>Compute the function of this class</summary>
    internal static void Compute(int n, float alpha, float[] X, int incx)
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
#if MANAGED
      if( alpha == 1 )
      {
        return;
      }
      if( alpha == 0 )
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = 0;
        }
      } 
      else
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = alpha * X[ix];
        }
      }
#else
      dna_blas_sscal(n, alpha, X, incx);
#endif
    }
    
    internal static void Compute(int n, double alpha, double[] X, int incx)
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
#if MANAGED
      if( alpha == 1 )
      {
        return;
      }
      if( alpha == 0 )
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = 0;
        }
      } 
      else
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = alpha * X[ix];
        }
      }
#else
      dna_blas_dscal(n, alpha, X, incx);
#endif
    }

    internal static void Compute(int n, ComplexFloat alpha, ComplexFloat[] X, int incx)
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
#if MANAGED
      if( alpha == 1 )
      {
        return;
      }
      if( alpha == 0 )
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = 0;
        }
      } 
      else
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = alpha * X[ix];
        }
      }
#else
      dna_blas_cscal(n, ref alpha, X, incx);
#endif
    }

    internal static void Compute(int n, Complex alpha, Complex[] X, int incx)
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
#if MANAGED
      if( alpha == 1 )
      {
        return;
      }
      if( alpha == 0 )
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = 0;
        }
      } 
      else
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = alpha * X[ix];
        }
      }
#else
      dna_blas_zscal(n, ref alpha, X, incx);
#endif
    }

    internal static void Compute(int n, float alpha, ComplexFloat[] X, int incx)
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
#if MANAGED
      if( alpha == 1 )
      {
        return;
      }
      if( alpha == 0 )
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = 0;
        }
      } 
      else
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] *= alpha;
        }
      }
#else
      dna_blas_csscal(n, alpha, X, incx);
#endif
    }

    internal static void Compute(int n, double alpha, Complex[] X, int incx)
    {
      ArgumentCheck(n, X, X.Length, ref incx);
      
#if MANAGED
      if( alpha == 1 )
      {
        return;
      }
      if( alpha == 0 )
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] = 0;
        }
      } 
      else
      {
        for ( int i = 0, ix = 0; i < n; ++i,ix+=incx ) 
        {
          X[ix] *= alpha;
        }
      }
#else
      dna_blas_zdscal(n, alpha, X, incx);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_sscal( int N, float alpha, [In,Out]float[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_dscal( int N, double alpha, [In,Out]double[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_cscal( int N, ref ComplexFloat alpha, [In,Out]ComplexFloat[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zscal( int N, ref Complex alpha, [In,Out]Complex[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_csscal( int N, float alpha, [In,Out]ComplexFloat[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_zdscal( int N, double alpha, [In,Out]Complex[] X, int incX );
#endif
  }
}
