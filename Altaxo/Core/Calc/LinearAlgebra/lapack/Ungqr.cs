/*
 * Ungqr.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
	[System.Security.SuppressUnmanagedCodeSecurityAttribute]
	internal sealed class Ungqr {
		private  Ungqr() {}                           
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
				throw new ArgumentException("n must be positive and less than or equal to m.", "n");
			}
			if( k < 0 || k > n ){
				throw new ArgumentException("k must be positive and less than or equal to n.", "k");
			}
			if ( lda < System.Math.Max(1,m) ) {
				throw new ArgumentException("lda must be at least max(1,m)", "lda");
			}
		}

		internal static int Compute( int m, int n, int k, ComplexFloat[] A, int lda, ComplexFloat[] tau ){
			ArgumentCheck(m, n, k, A, lda, tau);
			if (tau.Length < System.Math.Max(1, k) ){
				throw new ArgumentException("tau must be at least max(1,k).");
			}
			
			return dna_lapack_cungqr(Configuration.BlockSize, m, n, k, A, lda, tau);
		}

		internal static int Compute( int m, int n, int k, Complex[] A, int lda, Complex[] tau ){
			ArgumentCheck(m, n, k, A, lda, tau);
			if (tau.Length < System.Math.Max(1, k) ){
				throw new ArgumentException("tau must be at least max(1,k).");
			}
			
			return dna_lapack_zungqr(Configuration.BlockSize, m, n, k, A, lda, tau);
		}

		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_cungqr( int block_size, int m, int n, int k, [In,Out]ComplexFloat[] A, int lda, [In,Out]ComplexFloat[] tau  );
	
		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_zungqr( int block_size, int m, int n, int k, [In,Out]Complex[] A, int lda, [In,Out]Complex[] tau   );
	}
}
#endif