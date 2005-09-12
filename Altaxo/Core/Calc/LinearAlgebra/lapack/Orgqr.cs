/*
 * Orgqr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;

using dnA.Utility;

namespace dnA.Math.Lapack{
	[System.Security.SuppressUnmanagedCodeSecurityAttribute]
	internal sealed class Orgqr {
		private  Orgqr() {}                           
		private static void ArgumentCheck( int m, int n, int k, Object A, int lda, Object tau ) {
			if ( A == null ) {
				throw new ArgumentNullException("A","A cannot be null.");
			}
			if ( tau == null ) {
				throw new ArgumentNullException("tau","tau cannot be null.");
			}
			if ( m<0 ) {
				throw new ArgumentException("m must be at least zero.", "m");
			}
			if( n < 0 || n > m ){
				throw new ArgumentException("n must be positive and less than or equal to m.");
			}
			if( k < 0 || k > n ){
				throw new ArgumentException("k must be positive and less than or equal to n.");
			}
			if ( lda < System.Math.Max(1,m) ) {
				throw new ArgumentException("lda must be at least max(1,m)", "lda");
			}
		}

		internal static int Compute( int m, int n, int k, float[] A, int lda, float[] tau ){
			ArgumentCheck(m, n, k, A, lda, tau);
			return dna_lapack_sorgqr(Configuration.BlockSize, m, n, k, A, lda, tau);
		}

		internal static int Compute( int m, int n, int k, double[] A, int lda, double[] tau ){
			ArgumentCheck(m, n, k, A, lda, tau);
			return dna_lapack_dorgqr(Configuration.BlockSize, m, n, k, A, lda, tau);
		}

		[DllImport(dnA.Utility.Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_sorgqr( int block_size, int m, int n, int k, [In,Out]float[] A, int lda, [In,Out]float[] tau );
	
		[DllImport(dnA.Utility.Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_dorgqr( int block_size, int m, int n, int k, [In,Out]double[] A, int lda, [In,Out]double[] tau );
	}
}
#endif