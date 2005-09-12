/*
 * Imin.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
	///<summary>Returns the index of the minimum component in a vector</summary>
	internal sealed class Imin {
		private Imin(){}
		///<summary>Compute the function of this class</summary>
		internal static int Compute( int n, float[] X, int incx ){
			if( n < 0 ) {
				return 0;
			}
			if ( X == null ) {
				throw new ArgumentNullException( "X", "X cannot be null.");
			}
			if( incx == 0 ){
				throw new ArgumentException("incx cannot be zero.", "incx");
			}
			incx = System.Math.Abs(incx);
			if( X.Length < ( 1 + (n-1) * incx) ){
				throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
			}
			int index = 0;
			float min = System.Single.MaxValue;
			for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) {
				float test = System.Math.Abs(X[ix]);
				if( test < min ){
					index = i;
					min = test;
				}
			}
			return index;
		}

		internal static int Compute( int n, double[] X, int incx ){
			if( n < 0 ) {
				return 0;
			}
			if ( X == null ) {
				throw new ArgumentNullException( "X", "X cannot be null.");
			}
			if( incx == 0 ){
				throw new ArgumentException("incx cannot be zero.", "incx");
			}
			incx = System.Math.Abs(incx);
			if( X.Length < ( 1 + (n-1) * incx) ){
				throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
			}
			int index = 0;
			double min = System.Double.MaxValue;
			for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) {
				double test = System.Math.Abs(X[ix]);
				if( test < min ){
					index = i;
					min = test;
				}
			}
			return index;
		}

		internal static int Compute( int n, ComplexFloat[] X, int incx ){
			if( n < 0 ) {
				return 0;
			}
			if ( X == null ) {
				throw new ArgumentNullException( "X", "X cannot be null.");
			}
			if( incx == 0 ){
				throw new ArgumentException("incx cannot be zero.", "incx");
			}
			incx = System.Math.Abs(incx);
			if( X.Length < ( 1 + (n-1) * incx) ){
				throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
			}
			int index = 0;
			float min = System.Single.MaxValue;
			for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) {
				float test = System.Math.Abs(X[ix].Real) + System.Math.Abs(X[ix].Imag);
				if( test < min ){
					index = i;
					min = test;
				}
			}
			return index;
		}

		internal static int Compute( int n, Complex[] X, int incx ){
			if( n < 0 ) {
				return 0;
			}
			if ( X == null ) {
				throw new ArgumentNullException( "X", "X cannot be null.");
			}
			if( incx == 0 ){
				throw new ArgumentException("incx cannot be zero.","incx");
			}
			incx = System.Math.Abs(incx);
			if( X.Length < ( 1 + (n-1) * incx) ){
				throw new ArgumentException("The dimension of X must be a least 1 + (n-1) * incx.");
			}
			int index = 0;
			double min = System.Double.MaxValue;
			for ( int i = 0, ix = 0; i < n; ++i, ix += incx ) {
				double test = System.Math.Abs(X[ix].Real) + System.Math.Abs(X[ix].Imag);
				if( test < min ){
					index = i;
					min = test;
				}
			}
			return index;
		}		
	}
}
