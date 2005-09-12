#region Using directives

using System;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

#endregion

namespace AltaxoTest.Calc.LinearAlgebra {
	internal sealed class Comparer {
		private Comparer() { }

		public static bool AreEqual(ComplexFloat f1, ComplexFloat f2) {
			return f1 == f2;
		}

		public static bool AreEqual(Complex f1, Complex f2) {
			return f1 == f2;
		}

		public static bool AreEqual(ComplexFloat f1, ComplexFloat f2, float delta) {
			if (System.Math.Abs(f1.Imag - f2.Imag) > delta) return false;
			if (System.Math.Abs(f1.Real - f2.Real) > delta) return false;
			return true;
		}

		public static bool AreEqual(Complex f1, Complex f2, double delta) {
			if (System.Math.Abs(f1.Imag - f2.Imag) > delta) return false;
			if (System.Math.Abs(f1.Real - f2.Real) > delta) return false;
			return true;
		}

		public static bool AreEqual(ComplexFloatMatrix f1, ComplexFloatMatrix f2) {
			return f1==f2;
		}

		public static bool AreEqual(ComplexDoubleMatrix f1, ComplexDoubleMatrix f2) {
			return f1==f2;
		}

		public static bool AreEqual(ComplexFloatMatrix f1, ComplexFloatMatrix f2, float delta) {
			if (f1.RowLength != f2.RowLength) return false;
			if (f1.ColumnLength != f2.ColumnLength) return false;
			for(int i=0; i<f1.RowLength; i++) {
				for (int j = 0; j < f1.ColumnLength; j++) {
					if (!AreEqual(f1[i, j], f2[i, j], delta)) 
						return false;
				}
			}
			return true;
		}

		public static bool AreEqual(ComplexDoubleMatrix f1, ComplexDoubleMatrix f2, float delta) {
			if (f1.RowLength != f2.RowLength) return false;
			if (f1.ColumnLength != f2.ColumnLength) return false;
			for (int i = 0; i < f1.RowLength; i++) {
				for (int j = 0; j < f1.ColumnLength; j++) {
					if (!AreEqual(f1[i, j], f2[i, j], delta))
						return false;
				}
			}
			return true;
		}
	}
}
