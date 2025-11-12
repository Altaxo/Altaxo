#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Xunit;

namespace Altaxo.Calc.Regression.Foo
{
  public class QuickQuadraticRegressionTests
  {
    private static (double x, double y)[] _data = new (double x, double y)[] { (0, 1), (0, 4), (1, 3), (1, 6), (2, -1), (2, 2) };

    [Fact]
    public void TestInterceptAndSlope()
    {
      const double expectedA0 = 2.5;
      const double expectedA1 = 5.0;
      const double expectedA2 = -3.0;
      const double expectedSumOfChiSquared = 6 * 1.5 * 1.5;

      var qlr = new QuickQuadraticRegression();
      foreach (var (x, y) in _data)
      {
        qlr.Add(x, y);
      }
      double a0 = qlr.GetA0();
      double a1 = qlr.GetA1();

      AssertEx.AreEqual(expectedA0, a0, 1e-10, 0.0, "Intercept test");
      AssertEx.AreEqual(expectedA1, a1, 1e-10, 0.0, "Slope test");
      AssertEx.AreEqual(expectedA2, qlr.GetA2(), 1e-10, 0.0, "Second slope test");

      AssertEx.AreEqual(expectedA0 + expectedA1 * 0 + expectedA2 * 0 * 0, qlr.GetYOfX(0), 1e-10, 0.0, "Y of X=0 test");
      AssertEx.AreEqual(expectedA0 + expectedA1 * 1 + expectedA2 * 1 * 1, qlr.GetYOfX(1), 1e-10, 0.0, "Y of X=1 test");
      AssertEx.AreEqual(expectedA0 + expectedA1 * 2 + expectedA2 * 2 * 2, qlr.GetYOfX(2), 1e-10, 0.0, "Y of X=2 test");

      AssertEx.AreEqual(expectedSumOfChiSquared, qlr.SumChiSquared(), 1e-10, 0.0, "Chi squared test");
      AssertEx.AreEqual(expectedSumOfChiSquared / (qlr.N - 3), qlr.SigmaSquared(), 1e-10, 0.0, "Sigma squared test");
      AssertEx.AreEqual(Math.Sqrt(expectedSumOfChiSquared / (qlr.N - 3)), qlr.Sigma(), 1e-10, 0.0, "Sigma test");



      // Calculate the sum of squared errors
      double chiSquared = 0.0;
      foreach (var (x, y) in _data)
      {
        double yPred = qlr.GetYOfX(x);
        chiSquared += (y - yPred) * (y - yPred);
      }
      AssertEx.AreEqual(expectedSumOfChiSquared, chiSquared, 1e-10, 0.0, "Chi squared test");
    }

    [Fact]
    public void TestInsufficientDataPoints()
    {
      var qlr = new QuickQuadraticRegression();
      qlr.Add(1, 2);
      qlr.Add(2, 3);
      // Only two points, quadratic regression is not possible
      Assert.Throws<System.InvalidOperationException>(() => qlr.GetA0());
      Assert.Throws<System.InvalidOperationException>(() => qlr.GetA1());
      Assert.Throws<System.InvalidOperationException>(() => qlr.GetA2());
    }

    [Fact]
    public void TestCovarianceMatrix()
    {
      var qlr = new QuickQuadraticRegression();
      foreach (var (x, y) in _data)
      {
        qlr.Add(x, y);
      }
      var cov = qlr.GetCovarianceMatrix();
      // Check shape
      Assert.Equal(3, cov.RowCount);
      Assert.Equal(3, cov.ColumnCount);
      // Check symmetry
      for (int i = 0; i < 3; ++i)
        for (int j = 0; j < 3; ++j)
          AssertEx.AreEqual(cov[i, j], cov[j, i], 1e-12, 0.0, $"Covariance symmetry [{i},{j}]");
      // Check non-negative variances
      for (int i = 0; i < 3; ++i)
        Assert.True(cov[i, i] >= 0, $"Variance at [{i},{i}] should be non-negative");

      AssertEx.AreEqual(9 / 4d, cov[0, 0], 1E-10, 0);
      AssertEx.AreEqual(-27 / 8d, cov[0, 1], 1E-10, 0);
      AssertEx.AreEqual(9 / 8d, cov[0, 2], 1E-10, 0);
      AssertEx.AreEqual(117 / 8d, cov[1, 1], 1E-10, 0);
      AssertEx.AreEqual(-27 / 4d, cov[1, 2], 1E-10, 0);
      AssertEx.AreEqual(27 / 8d, cov[2, 2], 1E-10, 0);


    }
  }
}
