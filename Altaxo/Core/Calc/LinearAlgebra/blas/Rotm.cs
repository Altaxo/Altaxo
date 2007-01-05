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
 * Rotm.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Performs rotation of points in the modified plane.</summary>
  ///<remarks> Returns: <c>
  /// x(i) = H*x(i) + H*y(i)
  /// y(i) = H*y(i) - H*x(i) </c></remarks>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Rotm 
  {
    private Rotm(){}
    ///<summary>Compute the function of this class</summary>
    internal static void Compute( int n, float[] X, int incx, float[] Y, int incy, float[] P )
    {
      if( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater.","n");
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
      if ( P == null ) 
      {
        throw new ArgumentNullException("P", "P cannot be null.");
      }
      if( P.Length != 5 )
      {
        throw new ArgumentException("P must be an array of length 5.", "P");
      }
      
#if MANAGED
      if (P[0] == 0.0) 
      {
        P[1] = 1.0f;
        P[4] = 1.0f;
      } 
      else if (P[0] == 1.0) 
      {
        P[2] = -1.0f;
        P[3] = 1.0f;
      } 
      else if (P[0] == -2.0) 
      {
        return;
      } 
      else 
      {
        throw new ArgumentException("Invalid value for P[0].","P");
      }

      for (int i = 0, ix = 0, iy = 0; i < n; ++i, ix+=incx,iy+=incy ) 
      {
        float x = X[ix];
        float y = Y[iy];
        X[ix] = P[0] * x + P[1] * y;
        Y[iy] = P[2] * x + P[3] * y;
      }
#else
      dna_blas_srotm(n, X, incx, Y, incy, P );
#endif
    }

    internal static void Compute( int n, double[] X, int incx, double[] Y, int incy, double[] P )
    {
      if( n < 0 ) 
      {
        throw new ArgumentException("n must be zero or greater.","n");
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
      if ( P == null ) 
      {
        throw new ArgumentNullException("P", "P cannot be null.");
      }
      if( P.Length != 5 )
      {
        throw new ArgumentException("P must be an array of length 5.","P");
      }
      
      
#if MANAGED
      if (P[0] == 0.0) 
      {
        P[1] = 1.0;
        P[4] = 1.0;
      } 
      else if (P[0] == 1.0) 
      {
        P[2] = -1.0;
        P[3] = 1.0;
      } 
      else if (P[0] == -2.0) 
      {
        return;
      } 
      else 
      {
        throw new ArgumentException("Invalid value for P[0].","P");
      }

      for (int i = 0, ix = 0, iy = 0; i < n; ++i, ix+=incx, iy+=incy ) 
      {
        double x = X[ix];
        double y = Y[iy];
        X[ix] = P[0] * x + P[1] * y;
        Y[iy] = P[2] * x + P[3] * y;
      }
#else
      dna_blas_drotm(n, X, incx, Y, incy, P);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_srotm( int N, [In,Out]float[] X, int incX, [In,Out]float[] Y, int incY, [In,Out]float[] P );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_drotm( int N, [In,Out]double[] X, int incX, [In,Out]double[] Y, int incY, [In,Out]double[] P );
#endif
  }
}
