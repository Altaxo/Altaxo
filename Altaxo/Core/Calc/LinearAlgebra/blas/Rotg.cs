/*
 * Rotg.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
	///<summary>Computes the parameters for a Givens rotation. </summary>
	[System.Security.SuppressUnmanagedCodeSecurityAttribute]
	internal sealed class Rotg {
		internal static void Compute(ref float a, ref float b, out float c, out float s) {
			c = 1f;
			s = 0;
			
#if MANAGED
			float roe = b;
			float absA = System.Math.Abs(a);
			float absB = System.Math.Abs(b);
 
			if( absA > absB ){
				roe = a;
			}
			float scale = absA + absB;
			float r = 0;
			float z = 0;
			if( scale != 0 ){
				float aos = a/scale;
				float bos = a/scale;
				r = scale * (float)System.Math.Sqrt( aos * aos + bos + bos );
				if( roe < 0 ){
					r *= -1;
				} 
				c = a/r;
				s = b/r;
				z = 1.0f;
 
				if( absA > absB ) {
					z = s;
				} else if( absB >= absA && c != 0.0 ){
					z = 1.0f/c;
				}
			}
			a = r;
			b = z;
					
#else
			dna_blas_srotg(ref a, ref b, out c, out s);
#endif
		}
 
		internal static void Compute(ref double a, ref double b, out double c, out double s){
			c = 1;
			s = 0;

			
#if MANAGED
			double roe = b;
			double absA = System.Math.Abs(a);
			double absB = System.Math.Abs(b);
 
			if( absA > absB ){
				roe = a;
			}
			double scale = absA + absB;
			double r = 0;
			double z = 0;
			if( scale != 0 ){
				double aos = a/scale;
				double bos = a/scale;
				r = scale * System.Math.Sqrt( aos * aos + bos + bos );
				if( roe < 0 ){
					r *= -1;
				} 
				c = a/r;
				s = b/r;
				z = 1.0f;
 
				if( absA > absB ) {
					z = s;
				} else if( absB >= absA && c != 0.0 ){
					z = 1.0f/c;
				}
			}
			a = r;
			b = z;
#else
			dna_blas_drotg(ref a, ref b, out c, out s);
#endif
		}
		private Rotg(){}

#if !MANAGED
		///<summary>P/Invoke to wrapper with native code</summary>
		[DllImport(dnA.Utility.Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern void dna_blas_srotg( ref float a, ref float b, out float c, out float s );

		[DllImport(dnA.Utility.Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
		private static extern void dna_blas_drotg( ref double a, ref double b, out double c, out double s );
#endif
	}
}
