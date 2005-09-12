/*
 * Dot.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
	///<summary>Computes a vector-vector dot product.</summary>
	///<remarks> Calculates <c> result = x^T * y = x[1]*y[1] + x[2]*y[2] + ... + x[n]*y[n] </c></remarks> 
	[System.Security.SuppressUnmanagedCodeSecurityAttribute]
	internal sealed class Dot {
		private Dot(){}
		///<summary>Check arguments so that errors don't occur in native code</summary>
		private static void ArgumentCheck(int n, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) {
			// no exception throwing for n
			if ( X == null ) {
				throw new ArgumentNullException("X cannot be null.", "X" );
			}
			if ( Y == null ) {
				throw new ArgumentNullException("Y","Y cannot be null.");
			}
			if ( incx == 0 ) {
				throw new ArgumentException("incx cannot be zero.", "incx");
			}
			if ( incy == 0 ) {
				throw new ArgumentException("incy cannot be zero.", "incy");
			}
			incx = System.Math.Abs(incx);
			incy = System.Math.Abs(incy);
			if ( lenX < ( 1 + (n-1) * incx ) ) {
				throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
			}
			if ( lenY < ( 1 + (n-1) * incy ) ) {
				throw new ArgumentException("The dimension of Y must be at least 1 + (n-1) * incy.");
			}
		}

		///<summary>Compute the function of this class</summary>
		internal static float Compute(int n, float[] X, int incx, float[] Y, int incy){
			if ( n < 0 ) {
				return 0;
			}
			ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
			
			float ret = 0;
#if MANAGED
			int ix = 0;
			int iy = 0;
			for ( int i = 0; i < n; ++i ) {
				ret += (X[ix] * Y[iy]);
				ix += incx;
				iy += incy;
			}
#else
			ret = dna_blas_sdot(n, X, incx, Y, incy);
#endif
			return ret;
		}


		internal static double Compute(int n, double[] X, int incx, double[] Y, int incy){
			if ( n < 0 ){ 
				return 0.0;
			}
			ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
			
			double ret = 0;
#if MANAGED
			int ix = 0;
			int iy = 0;
			for ( int i = 0; i < n; ++i ) {
				ret += (X[ix] * Y[iy]);
				ix += incx;
				iy += incy;
			}
#else
			ret = dna_blas_ddot(n, X, incx, Y, incy);
#endif
			return ret;
		}

#if !MANAGED
		[DllImport(dnA.Utility.Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern float dna_blas_sdot( int N, [In]float[] X, int incX, [In]float[] Y, int incY );

		[DllImport(dnA.Utility.Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern double dna_blas_ddot( int N, [In]double[] X, int incX, [In]double[] Y, int incY );
#endif
	}
}
