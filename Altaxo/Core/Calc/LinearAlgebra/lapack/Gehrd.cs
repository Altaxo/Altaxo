/*
 * Gehrd.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/
#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{

	[System.Security.SuppressUnmanagedCodeSecurityAttribute]
	internal sealed class Gehrd{
		private Gehrd(){}
		
		private static void ArgumentCheck(int n, int ilo, int ihi, object A, int lda, object tau){
			if (n<0) 
				throw new ArgumentException("n must be at least zero.", "n");
			if (n>0) {
				if (ilo<1 || ilo>n || ilo>ihi)
					throw new ArgumentException("ilo must be between 1 and min(ihi,n) if n>0");
				if (ihi<1 || ihi>n)
					throw new ArgumentException("ihi must be between 1 and n if n>0");
			} else {
				if (ilo!=1)
					throw new ArgumentException("ilo must be 1 if n=0", "ilo");
				if (ihi!=0)
					throw new ArgumentException("ihi must be 0 if n=0", "ihi");
			}
			if (A==null) 
				throw new ArgumentNullException("A","A cannot be null.");
			if ( tau == null )
				throw new ArgumentNullException("tau","tau cannot be null.");
			if (lda<n || lda<1) 
				throw new ArgumentException("lda must be at least max(1,n)", "lda");
		}
		
		public static int Compute( int n, int ilo, int ihi, float[] A, int lda, float[] tau ){
			ArgumentCheck(n, ilo, ihi, A, lda, tau);
			return dna_lapack_sgehrd(Configuration.BlockSize, n, ilo, ihi, A, lda, tau);
		}

		public static int Compute( int n, int ilo, int ihi, double[] A, int lda,double[] tau ){
			return dna_lapack_dgehrd(Configuration.BlockSize, n, ilo, ihi, A, lda, tau);
		}

		public static int Compute( int n, int ilo, int ihi, ComplexFloat[] A, int lda, ComplexFloat[] tau ){
			ArgumentCheck(n, ilo, ihi, A, lda, tau);
			return dna_lapack_cgehrd(Configuration.BlockSize, n, ilo, ihi, A, lda, tau);
		}

		public static int Compute( int n, int ilo, int ihi, Complex[] A, int lda, Complex[] tau ){
			ArgumentCheck(n, ilo, ihi, A, lda, tau);
			return dna_lapack_zgehrd(Configuration.BlockSize, n, ilo, ihi, A, lda, tau);
		}

		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_sgehrd(int block_size, int n, int ilo, int ihi, float[] A, int lda, float[] tau);

		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_dgehrd(int block_size, int n, int ilo, int ihi, double[] A, int lda, double[] tau);

		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_cgehrd(int block_size, int n, int ilo, int ihi, ComplexFloat[] A, int lda, ComplexFloat[] tau);

		[DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern int dna_lapack_zgehrd(int block_size, int n, int ilo, int ihi, Complex[] A, int lda, Complex[] tau);

	}
}
#endif