/*
 * Dotu.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
	///<summary>Computes a vector-vector dot product.</summary>
	///<remarks> Returns <c> results = x^T * y = x[1]*y[1] + x[2]*y[2] + ... + x[n]*y[n]</c></remarks>
	[System.Security.SuppressUnmanagedCodeSecurityAttribute]
	internal sealed class Dotu {
		private Dotu(){}
		///<summary>Check arguments so that errors don't occur in native code</summary>
		private static void ArgumentCheck(int n, object X, int lenX, ref int incx, object Y, int lenY, ref int incy) {
			if ( X == null ) {
				throw new ArgumentNullException("X cannot be null.", "X");
			}
			if ( Y == null ) {
				throw new ArgumentNullException("Y", "Y cannot be null.");
			}
			if ( incx == 0 ) {
				throw new ArgumentException( "incx cannot be zero.", "incx");
			}
			if ( incy == 0 ) {
				throw new ArgumentException("incy cannot be zero.", "incy");
			}
			incx = System.Math.Abs(incx);
			incy = System.Math.Abs(incy);
			if ( lenX < ( 1 + (n-1) * incx) ) {
				throw new ArgumentException("The dimension of X must be at least 1 + (n-1) * incx.");
			}
			if ( lenY < ( 1 + (n-1) * incy) ) {
				throw new ArgumentException("The dimension of Y must be at least 1 + (n-1) * incy.");
			}
		}
		///<summary>Compute the function of this class</summary>
		internal static ComplexFloat Compute( int n, ComplexFloat[] X, int incx, ComplexFloat[] Y, int incy ){ 
 
			if ( n < 0 ){ 
				return ComplexFloat.Zero;
			}
			ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
			
			ComplexFloat ret = new ComplexFloat(0);
#if MANAGED
			int ix = 0;
			int iy = 0;
			for ( int i = 0; i < n; ++i ) {
				ret += (X[ix] * Y[iy]);
				ix += incx;
				iy += incy;
			}
#else
			dna_blas_cdotu(n, X, incx, Y, incy, ref ret);
#endif
			return ret;
		}

		internal static Complex Compute(int n, Complex[] X, int incx, Complex[] Y, int incy){
			if ( n < 0 ) {
				return ComplexFloat.Zero;
			}
			ArgumentCheck(n, X, X.Length, ref incx, Y, Y.Length, ref incy);
			
			Complex ret = new Complex(0);
#if MANAGED
			int ix = 0;
			int iy = 0;
			for ( int i = 0; i < n; ++i ) {
				ret += (X[ix] * Y[iy]);
				ix += incx;
				iy += incy;
			}
#else
			dna_blas_zdotu(n, X, incx, Y, incy, ref ret);
#endif
			return ret;
		}

#if !MANAGED
		///<summary>P/Invoke to wrapper with native code</summary>
		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern void dna_blas_cdotu( int N, [In]ComplexFloat[] X, int incX, [In]ComplexFloat[] Y, int incY, ref ComplexFloat dotu);

		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern void dna_blas_zdotu( int N, [In]Complex[] X, int incX, [In]Complex[] Y, int incY, ref Complex dotu);
#endif
	}
}
