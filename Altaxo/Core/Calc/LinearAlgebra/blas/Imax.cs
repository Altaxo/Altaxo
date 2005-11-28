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
 * Imax.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Return the index of a the maximum value in a vector</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Imax
  {
    private Imax(){}
    ///<summary>Compute the function of this class</summary>
    internal static int Compute( int n, float[] X, int incx )
    {
      if( n < 0 ) 
      {
        return 0;
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException( "X", "X cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.","incx");
      }
      incx = System.Math.Abs(incx);
      if( X.Length < ( 1 + (n-1) * incx) )
      {
        throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
      }
      int index = 0;
      
#if MANAGED
      float max = 0;
      for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) 
      {
        float test = System.Math.Abs(X[ix]);
        if( test > max )
        {
          index = i;
          max = test;
        }
      }
#else
      index = dna_blas_isamax(n, X, incx);
#endif
      return index;
    }

    internal static int Compute( int n, double[] X, int incx )
    {
      if( n < 0 ) 
      {
        return 0;
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException( "X", "X cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      incx = System.Math.Abs(incx);
      if( X.Length < ( 1 + (n-1) * incx) )
      {
        throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
      }
      int index = 0;
      
#if MANAGED
      double max = 0;
      for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) 
      {
        double test = System.Math.Abs(X[ix]);
        if( test > max )
        {
          index = i;
          max = test;
        }
      }
#else
      index = dna_blas_idamax(n, X, incx);
#endif
      return index;
    }
    
    internal static int Compute( int n, ComplexFloat[] X, int incx )
    {
      if( n < 0 ) 
      {
        return 0;
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException( "X", "X cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      incx = System.Math.Abs(incx);
      if( X.Length < ( 1 + (n-1) * incx) )
      {
        throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
      }
      int index = 0;
      
#if MANAGED
      float max = 0;
      for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) 
      {
        float test = System.Math.Abs(X[ix].Real) + System.Math.Abs(X[ix].Imag);
        if( test > max )
        {
          index = i;
          max = test;
        }
      }
#else
      index = dna_blas_icamax(n, X, incx);
#endif
      return index;
    }

    internal static int Compute( int n, Complex[] X, int incx )
    {
      if( n < 0 ) 
      {
        return 0;
      }
      if ( X == null ) 
      {
        throw new ArgumentNullException( "X", "X cannot be null.");
      }
      if( incx == 0 )
      {
        throw new ArgumentException("incx cannot be zero.", "incx");
      }
      incx = System.Math.Abs(incx);
      if( X.Length < ( 1 + (n-1) * incx) )
      {
        throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
      }
      int index = 0;
      
#if MANAGED
      double max = 0;
      for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) 
      {
        double test = System.Math.Abs(X[ix].Real) + System.Math.Abs(X[ix].Imag);
        if( test > max )
        {
          index = i;
          max = test;
        }
      }
#else
      index = dna_blas_izamax(n, X, incx);
#endif
      return index;
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_blas_isamax( int N, [In]float[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_blas_idamax( int N, [In]double[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_blas_icamax( int N, [In]ComplexFloat[] X, int incX );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_blas_izamax( int N, [In]Complex[] X, int incX );
#endif
  }
}
