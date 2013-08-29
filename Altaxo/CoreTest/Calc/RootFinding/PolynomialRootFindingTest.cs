#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using Altaxo.Calc.RootFinding;
using Altaxo.Calc;
using NUnit.Framework;

namespace Calc.RootFinding
{

	[TestFixture]
	public class PolynomialRootFindingTest
	{
		/// <summary>Coefficients of the real polynomial (x-0)*(x-1)*(x-2)*...*(x-9). Lowest order coefficient comes first.</summary>
		static readonly double[] realCoefficients1 = { 0, -362880, 1026576, -1172700, 723680, -269325, 63273, -9450, 870, -45, 1 };

		/// <summary>
		/// Coefficients of the complex polynomial (x-4-4i)*(x-3-3i)*... *(x+3+3i)*(x+4+4i)
		/// </summary>
		static readonly Complex[] complexCoefficients1 = { new Complex(0, 0), new Complex(9216, 0), new Complex(0, 0), new Complex(0, 6560), new Complex(0, 0), new Complex(-1092, 0), new Complex(0, 0), new Complex(0, -60), new Complex(0, 0), new Complex(1, 0) };
		
		/// <summary>
		/// Polynomial (x-0)*(x-1)*(x-2)*... *(x-9) should return the real roots 0, 1, 2, ... 9.
		/// </summary>
		[Test]
		public void Test10DegreeRealPolynomial()
		{
			var roots = RealPolynomialRootFinder_JenkinsTraub.FindRoots(realCoefficients1);

			Assert.AreEqual(10, roots.Count);
			for (int i = 0; i < roots.Count; ++i)
				Assert.AreEqual(0, roots[i].Im);

			roots.Sort((x, y) => Comparer<double>.Default.Compare(x.Re, y.Re));

			for (int i = 0; i < roots.Count; ++i)
				Assert.AreEqual(i, roots[i].Re, 1E-7);

		}

		/// <summary>
		/// Polynomial (x-0)*(x-1)*(x-2)*... *(x-9) should return the real roots 0, 1, 2, ... 9.
		/// </summary>
		[Test]
		public void Test10DegreeComplexPolynomial()
		{
			Complex[] ccoeffs = new Complex[realCoefficients1.Length];
			for (int i = 0; i < realCoefficients1.Length; ++i)
				ccoeffs[i] = realCoefficients1[i];

			var roots = ComplexPolynomialRootFinder_JenkinsTraub.FindRoots(ccoeffs);

			Assert.AreEqual(10, roots.Count);
			for (int i = 0; i < roots.Count; ++i)
				Assert.AreEqual(0, roots[i].Im, 1E-11);

			roots.Sort((x, y) => Comparer<double>.Default.Compare(x.Re, y.Re));

			for (int i = 0; i < roots.Count; ++i)
				Assert.AreEqual(i, roots[i].Re, 1E-7);

		}

		/// <summary>
		/// Polynomial (x-4-4i)*(x-3-3i)*... *(x+3+3i)*(x+4+4i) should return the complex roots -4-4i, -3-3i, ... 4+4i
		/// </summary>
		[Test]
		public void Test9DegreeComplexPolynomial()
		{
			var roots = ComplexPolynomialRootFinder_JenkinsTraub.FindRoots(complexCoefficients1);
			Assert.AreEqual(9, roots.Count);

			roots.Sort((x, y) => Comparer<double>.Default.Compare(x.Re, y.Re));

			for (int i = 0; i < roots.Count; ++i)
			{
				Assert.AreEqual(i-4, roots[i].Im, 1E-12);
				Assert.AreEqual(i-4, roots[i].Re, 1E-12);
			}
		}

		[Test]
		public void TestRootsToCoefficientsReal()
		{
			var r = new double[10];
			for (int i = 0; i < r.Length; ++i)
				r[i] = i;

			var c = CoefficientsFromRoots(r);
			Assert.AreEqual(realCoefficients1.Length, c.Length);
			for (int i = 0; i < c.Length; ++i)
				Assert.AreEqual(realCoefficients1[i], c[i]);
		}

		[Test]
		public void TestRootsToCoefficientsComplex()
		{
			var r = new Complex[9];
			for (int i = 0; i < r.Length; ++i)
				r[i] = new Complex(i - 4, i - 4);

			var c = CoefficientsFromRoots(r);
			Assert.AreEqual(complexCoefficients1.Length, c.Length);
			for (int i = 0; i < c.Length; ++i)
			{
				Assert.AreEqual(complexCoefficients1[i].Re, c[i].Re);
				Assert.AreEqual(complexCoefficients1[i].Im, c[i].Im);
			}
		}



		/// <summary>
		/// Calculate the coefficients of a polynom from it's roots.
		/// </summary>
		/// <param name="roots">The roots.</param>
		/// <returns>The coefficients of the polynom, with the lowest order coefficient at index 0. The highest order coefficient is at index [number of roots] and is always 1.</returns>
		static double[] CoefficientsFromRoots(double[] roots)
		{
			var coeff = new double[roots.Length+1];
			coeff[0] = 1;

			for (int i = 0; i < roots.Length; ++i)
			{
				var root = -roots[i];

				for (int j = i+1; j > 0; --j)
					coeff[j] = coeff[j] * root + coeff[j - 1];

				coeff[0] = coeff[0] * root;
			}

			return coeff;
		}


		/// <summary>
		/// Calculate the coefficients of a polynom from it's roots.
		/// </summary>
		/// <param name="roots">The roots.</param>
		/// <returns>The coefficients of the polynom, with the lowest order coefficient at index 0. The highest order coefficient is at index [number of roots] and is always 1.</returns>
		static Complex[] CoefficientsFromRoots(Complex[] roots)
		{
			var coeff = new Complex[roots.Length + 1];
			coeff[0] = 1;

			for (int i = 0; i < roots.Length; ++i)
			{
				var root = -roots[i];

				for (int j = i + 1; j > 0; --j)
					coeff[j] = coeff[j] * root + coeff[j - 1];

				coeff[0] = coeff[0] * root;
			}

			return coeff;
		}
 
	}
}
