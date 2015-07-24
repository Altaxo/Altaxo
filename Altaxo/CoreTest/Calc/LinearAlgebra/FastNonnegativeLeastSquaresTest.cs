using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
	[TestFixture]
	public static class FastNonnegativeLeastSquaresTest
	{
		[Test]
		public static void Test01()
		{
			var X = new DoubleMatrix(new double[,] { { 73, 71, 52 }, { 87, 74, 46 }, { 72, 2, 7 }, { 80, 89, 71 } });
			var y = new DoubleMatrix(new double[,] { { 49 }, { 67 }, { 68 }, { 20 } });

			var XtX = X.GetTranspose() * X;
			var Xty = X.GetTranspose() * y;

			IMatrix x, w;
			FastNonnegativeLeastSquares.Execute(XtX, Xty, null, out x, out w);

			Assert.AreEqual(0.65, x[0, 0], 0.01);
			Assert.AreEqual(0, x[1, 0], 0.01);
			Assert.AreEqual(0, x[2, 0], 0.01);

			Assert.AreEqual(0, w[0, 0]);
			Assert.Less(w[1, 0], 0);
			Assert.Less(w[2, 0], 0);
		}

		[Test]
		public static void Test02()
		{
			int NR = 100;
			int NC = 5;

			// erzeuge Basisfunktionen
			var X = new DoubleMatrix(NR, NC);
			for (int c = 0; c < 5; ++c)
			{
				double rt = (c + 1) * 4;
				for (int r = 0; r < NR; ++r)
					X[r, c] = Math.Exp(-r / rt);
			}

			var y = new DoubleMatrix(NR, 1);
			for (int r = 0; r < NR; ++r)
			{
				double sum = 0;
				for (int c = 0; c < 5; ++c)
				{
					double amp = 1 - 0.4 * Math.Abs(c - 2);
					sum += amp * X[r, c];
				}
				y[r, 0] = sum;
			}

			var XtX = new DoubleMatrix(5, 5);
			MatrixMath.MultiplyFirstTransposed(X, X, XtX);

			var Xty = new DoubleMatrix(5, 1);
			MatrixMath.MultiplyFirstTransposed(X, y, Xty);

			IMatrix x, w;
			FastNonnegativeLeastSquares.Execute(XtX, Xty, null, out x, out w);

			Assert.AreEqual(0.2, x[0, 0], 1e-6);
			Assert.AreEqual(0.6, x[1, 0], 1e-6);
			Assert.AreEqual(1.0, x[2, 0], 1e-6);
			Assert.AreEqual(0.6, x[3, 0], 1e-6);
			Assert.AreEqual(0.2, x[4, 0], 1e-6);
		}
	}
}