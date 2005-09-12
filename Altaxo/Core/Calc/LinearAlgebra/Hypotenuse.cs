using System;

namespace Altaxo.Calc.LinearAlgebra {
	internal class Hypotenuse {
		private Hypotenuse() {}
		public static double Compute(double a, double b) {
			double r;
			if (System.Math.Abs(a) > System.Math.Abs(b)) {
				r = b/a;
				r = System.Math.Abs(a)*System.Math.Sqrt(1+r*r);
			} else if (b != 0) {
				r = a/b;
				r = System.Math.Abs(b)*System.Math.Sqrt(1+r*r);
			} else {
				r = 0.0;
			}
			return r;
		}  
		public static float Compute(float a, float b) {
			float r;
			if (System.Math.Abs(a) > System.Math.Abs(b)) {
				r = b/a;
				r = (float)(System.Math.Abs(a)*System.Math.Sqrt(1+r*r));
			} else if (b != 0) {
				r = a/b;
				r = (float)(System.Math.Abs(b)*System.Math.Sqrt(1+r*r));
			} else {
				r = 0.0f;
			}
			return r;
		}  
	
		public static float Compute(ComplexFloat a, ComplexFloat b){
			ComplexFloat temp = a * ComplexMath.Conjugate(a);
			temp += (b * ComplexMath.Conjugate(b));
			float ret = ComplexMath.Absolute(temp);
			return (float)System.Math.Sqrt(ret);
		}

		public static double Compute(Complex a, Complex b){
			Complex temp = a * ComplexMath.Conjugate(a);
			temp += (b * ComplexMath.Conjugate(b));
			double ret = ComplexMath.Absolute(temp);
			return System.Math.Sqrt(ret);
		}
	}
}
