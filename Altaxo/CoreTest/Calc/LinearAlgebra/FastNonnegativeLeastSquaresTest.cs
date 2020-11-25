#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra
{

  public static class FastNonnegativeLeastSquaresTest
  {
    /// <summary>
    /// Test set from Literature Rasmus Bro and Sijmen De Jong, 'A fast non-negativity-constrained least squares algorithm', Journal of Chemometrics, Vol. 11, 393-401 (1997)
    /// </summary>
    [Fact]
    public static void Test01a()
    {
      var X = new DoubleMatrix(new double[,] { { 73, 71, 52 }, { 87, 74, 46 }, { 72, 2, 7 }, { 80, 89, 71 } });
      var y = new DoubleMatrix(new double[,] { { 49 }, { 67 }, { 68 }, { 20 } });

      var XtX = X.GetTranspose() * X;
      var Xty = X.GetTranspose() * y;
      FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out var x, out var w);

      AssertEx.Equal(0.65, x[0, 0], 0.01);
      AssertEx.Equal(0, x[1, 0], 0.01);
      AssertEx.Equal(0, x[2, 0], 0.01);

      AssertEx.Equal(0, w[0, 0], 1e-8);
      AssertEx.Less(w[1, 0], 0);
      AssertEx.Less(w[2, 0], 0);
    }

    /// <summary>
    /// Another test set which utilitizes the inner loop of the algorithm.
    /// </summary>
    [Fact]
    public static void Test01b()
    {
      var X = new DoubleMatrix(new double[,] { { 771, 307, 765, 280 }, { 404, 802, 29, 703 }, { 166, 446, 8, 236 }, { 985, 225, 510, 731 }, { 109, 12, 382, 89 } });
      var y = new DoubleMatrix(new double[,] { { 83 }, { 339 }, { 330 }, { 731 }, { 896 } });

      var XtX = X.GetTranspose() * X;
      var Xty = X.GetTranspose() * y;
      FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out var x, out var w);

      AssertEx.Equal(0, x[0, 0], 1e-4);
      AssertEx.Equal(0, x[1, 0], 1e-4);
      AssertEx.Equal(0.41813, x[2, 0], 1e-4);
      AssertEx.Equal(0.58480, x[3, 0], 1e-4);

      AssertEx.Less(w[0, 0], 0);
      AssertEx.Less(w[1, 0], 0);
      AssertEx.Equal(0, w[2, 0], 1e-8);
      AssertEx.Equal(0, w[3, 0], 1e-8);
    }

    /// <summary>
    /// Another test set which utilitizes the inner loop of the algorithm multiple times.
    /// </summary>
    [Fact]
    public static void Test01c()
    {
      var X = new DoubleMatrix(new double[,] { { 106, 743, 746, 73 }, { 579, 420, 531, 584 }, { 693, 234, 562, 255 }, { 484, 381, 474, 360 }, { 313, 68, 78, 301 } });
      var y = new DoubleMatrix(new double[,] { { 803 }, { 292 }, { 230 }, { 469 }, { 655 } });

      var XtX = X.GetTranspose() * X;
      var Xty = X.GetTranspose() * y;
      FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out var x, out var w);

      AssertEx.Equal(0, x[0, 0], 1e-4);
      AssertEx.Equal(0.90443, x[1, 0], 1e-4);
      AssertEx.Equal(0, x[2, 0], 1e-4);
      AssertEx.Equal(0.29507, x[3, 0], 1e-4);

      AssertEx.Less(w[0, 0], 0);
      AssertEx.Equal(0, w[1, 0], 1e-8);
      AssertEx.Less(w[2, 0], 0);
      AssertEx.Equal(0, w[3, 0], 1e-8);
    }

    /// <summary>
    /// Same test set as in <see cref="Test01c"/>, but here the first parameter is unrestricted.
    /// </summary>
    [Fact]
    public static void Test01d1()
    {
      var X = new DoubleMatrix(new double[,] { { 106, 743, 746, 73 }, { 579, 420, 531, 584 }, { 693, 234, 562, 255 }, { 484, 381, 474, 360 }, { 313, 68, 78, 301 } });
      var y = new DoubleMatrix(new double[,] { { 803 }, { 292 }, { 230 }, { 469 }, { 655 } });

      var XtX = X.GetTranspose() * X;
      var Xty = X.GetTranspose() * y;
      FastNonnegativeLeastSquares.Execution(XtX, Xty, (i) => i != 0, null, out var x, out var w);

      AssertEx.Equal(-0.07097, x[0, 0], 1e-4);
      AssertEx.Equal(0.91034, x[1, 0], 1e-4);
      AssertEx.Equal(0, x[2, 0], 1e-4);
      AssertEx.Equal(0.37911, x[3, 0], 1e-4);

      AssertEx.Equal(0, w[0, 0], 1e-8);
      AssertEx.Equal(0, w[1, 0], 1e-8);
      AssertEx.Less(w[2, 0], 0);
      AssertEx.Equal(0, w[3, 0], 1e-8);
    }

    /// <summary>
    /// Same test set as in <see cref="Test01d1"/>, but permutated, to make sure that the result does not depend on the position of the unrestricted parameter.
    /// </summary>
    [Fact]
    public static void Test01d2()
    {
      var X = new DoubleMatrix(new double[,] { { 73, 746, 743, 106 }, { 584, 531, 420, 579 }, { 255, 562, 234, 693 }, { 360, 474, 381, 484 }, { 301, 78, 68, 313 } });
      var y = new DoubleMatrix(new double[,] { { 803 }, { 292 }, { 230 }, { 469 }, { 655 } });

      var XtX = X.GetTranspose() * X;
      var Xty = X.GetTranspose() * y;
      FastNonnegativeLeastSquares.Execution(XtX, Xty, (i) => i != 3, null, out var x, out var w);

      AssertEx.Equal(0.37911, x[0, 0], 1e-4);
      AssertEx.Equal(0, x[1, 0], 1e-4);
      AssertEx.Equal(0.91034, x[2, 0], 1e-4);
      AssertEx.Equal(-0.07097, x[3, 0], 1e-4);

      AssertEx.Equal(0, w[0, 0], 1e-8);
      AssertEx.Less(w[1, 0], 0);
      AssertEx.Equal(0, w[2, 0], 1e-8);
      AssertEx.Equal(0, w[3, 0], 1e-8);
    }

    /// <summary>
    /// Same test set as in <see cref="Test01d1"/>, but permutated, and all parameter unrestricted.
    /// </summary>
    [Fact]
    public static void Test01e_2()
    {
      var X = new DoubleMatrix(new double[,] { { 73, 746, 743, 106 }, { 584, 531, 420, 579 }, { 255, 562, 234, 693 }, { 360, 474, 381, 484 }, { 301, 78, 68, 313 } });
      var y = new DoubleMatrix(new double[,] { { 803 }, { 292 }, { 230 }, { 469 }, { 655 } });

      var XtX = X.GetTranspose() * X;
      var Xty = X.GetTranspose() * y;

      var solver = new DoubleLUDecomp(XtX);
      var expected = solver.Solve(Xty);
      FastNonnegativeLeastSquares.Execution(XtX, Xty, (i) => false, null, out var x, out var w);

      AssertEx.Equal(expected[0, 0], x[0, 0], 1e-4);
      AssertEx.Equal(expected[1, 0], x[1, 0], 1e-4);
      AssertEx.Equal(expected[2, 0], x[2, 0], 1e-4);
      AssertEx.Equal(expected[3, 0], x[3, 0], 1e-4);

      AssertEx.Equal(0, w[0, 0], 1e-8);
      AssertEx.Equal(0, w[1, 0], 1e-8);
      AssertEx.Equal(0, w[2, 0], 1e-8);
      AssertEx.Equal(0, w[3, 0], 1e-8);
    }

    /// <summary>
    /// A practical example with exponential functions to fit.
    /// </summary>
    [Fact]
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
      FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out var x, out var w);

      AssertEx.Equal(0.2, x[0, 0], 1e-6);
      AssertEx.Equal(0.6, x[1, 0], 1e-6);
      AssertEx.Equal(1.0, x[2, 0], 1e-6);
      AssertEx.Equal(0.6, x[3, 0], 1e-6);
      AssertEx.Equal(0.2, x[4, 0], 1e-6);
    }

    /* used to find test examples which enters the inner iteration loop
		**

		[Fact]
		public static void Test03()
		{
			int NR = 5;
			int NC = 4;

			for (int seed = 0; seed < int.MaxValue; ++seed)
			{
				var rnd = new Random(seed);

				// erzeuge Basisfunktionen
				var X = new DoubleMatrix(NR, NC);
				for (int c = 0; c < NC; ++c)
				{
					for (int r = 0; r < NR; ++r)
					{
						X[r, c] = rnd.Next(1, 1000);
					}
				}

				var y = new DoubleMatrix(NR, 1);
				for (int r = 0; r < NR; ++r)
				{
					y[r, 0] = rnd.Next(1, 1000);
				}

				var XtX = new DoubleMatrix(NC, NC);
				MatrixMath.MultiplyFirstTransposed(X, X, XtX);

				var Xty = new DoubleMatrix(NC, 1);
				MatrixMath.MultiplyFirstTransposed(X, y, Xty);

				IMatrix x, w;
				FastNonnegativeLeastSquares.Execute(XtX, Xty, null, out x, out w);
			}
		}

		*/
  }
}
