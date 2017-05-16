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

#endregion Copyright

using Altaxo.Calc.Fourier;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;
using System;

namespace AltaxoTest.Calc.Fourier
{
	[TestFixture]
	public class TestRealFourierTransformWrapper
	{
		private class InternalTestClass
		{
			public void Test(double[] arr)
			{
				double[] re = (double[])arr.Clone();
				double[] im = new double[arr.Length];

				double sum = 0;
				for (int i = 0; i < arr.Length; i++)
					sum += (arr[i] * arr[i]);
				sum = Math.Sqrt(sum / arr.Length);
				double tol = sum * arr.Length * 1E-14;
				double tol2 = arr.Length * 1E-14;

				// We transform the array
				RealFourierTransform rft = new RealFourierTransform(arr.Length);
				rft.Transform(arr, FourierDirection.Forward);
				RealFFTResultWrapper wrapper = new RealFFTResultWrapper(arr);

				// we transform the re and im
				NativeFourierMethods.FourierTransformation(re, im, FourierDirection.Forward);

				// now compare the results
				// first the realpart
				var realpart = wrapper.RealPart;
				for (int i = 0; i < realpart.Length; i++)
				{
					Assert.AreEqual(re[i], realpart[i], tol, string.Format("Testing realpart (len={0}, i={1})", arr.Length, i));
				}
				// now the imaginary part
				var imagpart = wrapper.ImaginaryPart;
				for (int i = 0; i < imagpart.Length; i++)
				{
					Assert.AreEqual(im[i], imagpart[i], tol, string.Format("Testing imagpart (len={0}, i={1})", arr.Length, i));
				}

				// now the complex result
				IROComplexDoubleVector reimpart = wrapper.ComplexResult;
				for (int i = 0; i < reimpart.Length; i++)
				{
					Assert.AreEqual(re[i], reimpart[i].Re, tol, string.Format("Testing ComplexResult.Re (len={0}, i={1})", arr.Length, i));
					Assert.AreEqual(im[i], reimpart[i].Im, tol, string.Format("Testing ComplexResult.Im (len={0}, i={1})", arr.Length, i));
				}

				// now the amplitude part
				var amppart = wrapper.Amplitude;
				for (int i = 0; i < imagpart.Length; i++)
				{
					double expected = Altaxo.Calc.RMath.Hypot(re[i], im[i]);
					Assert.AreEqual(expected, amppart[i], tol, string.Format("Testing amplitude (len={0}, i={1})", arr.Length, i));
				}

				// now the phase part
				var phasepart = wrapper.Phase;
				for (int i = 0; i < imagpart.Length; i++)
				{
					double ampl = Altaxo.Calc.RMath.Hypot(re[i], im[i]);
					if (ampl < 1E-14 * arr.Length)
						continue;
					double expected = Math.Atan2(im[i], re[i]);
					double actual = phasepart[i];
					if (Math.Abs(Math.Sin(expected) - Math.Sin(actual)) > tol2 || Math.Abs(Math.Cos(expected) - Math.Cos(actual)) > tol2)
						Assert.Fail(string.Format("Testing phase (len={0}, i={1}, Expected={2}, Actual={3})", arr.Length, i, expected, actual));
				}
			}

			public void TestZero(int len)
			{
				double[] arr = new double[len];
				Test(arr);
			}

			public void TestReOne_ZeroPos(int len)
			{
				double[] arr = new double[len];
				arr[0] = 1;
				Test(arr);
			}

			public void TestReOne_OnePos(int len)
			{
				double[] arr = new double[len];
				arr[1] = 1;
				Test(arr);
			}

			public void TestReOne_AllPos(int len)
			{
				double[] arr = new double[len];
				for (int i = 0; i < arr.Length; i++) arr[i] = 1;
				Test(arr);
			}

			public void TestReOne_RandomPos(int len, int tests)
			{
				System.Random rnd = new Random();
				for (int i = 0; i < tests; i++)
				{
					int pos = rnd.Next(len);
					double[] arr = new double[len];
					arr[pos] = 1;
					Test(arr);
				}
			}

			public void TestReOne_RandomValues(int len)
			{
				System.Random rnd = new Random();
				double[] arr = new double[len];
				for (int i = 0; i < arr.Length; i++)
					arr[i] = rnd.NextDouble();
				Test(arr);
			}
		}

		private const int nLowerLimit = 5;
		private const int nUpperLimit = 100;
		private const double maxTolerableEpsPerN = 1E-15;

		private int[] _testLengths = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

		private InternalTestClass _test = new InternalTestClass();

		[Test]
		public void Test01Zero()
		{
			foreach (int i in _testLengths)
				_test.TestZero(i);
		}

		[Test]
		public void Test02ReOne_ZeroPos()
		{
			foreach (int i in _testLengths)
				_test.TestReOne_ZeroPos(i);
		}

		[Test]
		public void Test03ReOne_OnePos()
		{
			foreach (int i in _testLengths)
				_test.TestReOne_OnePos(i);
		}

		[Test]
		public void Test04ReOne_AllPos()
		{
			foreach (int i in _testLengths)
				_test.TestReOne_AllPos(i);
		}

		[Test]
		public void Test05ReOne_RandomPos()
		{
			foreach (int i in _testLengths)
				_test.TestReOne_RandomPos(i, 5);
		}

		[Test]
		public void Test06ReRandomValues()
		{
			foreach (int i in _testLengths)
				_test.TestReOne_RandomValues(i);
		}
	}
}