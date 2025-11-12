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

namespace Altaxo.Calc.Regression
{
  public class QuickLinearRegressionTests
  {
    private static (double x, double y)[] _data = new (double x, double y)[] { (0, 1), (0, 4), (1, 6), (1, 9), (2, 11), (2, 14) };

    [Fact]
    public void TestInterceptAndSlope()
    {
      const double expectedIntercept = 2.5;
      const double expectedSlope = 5.0;
      const double expectedChiSquared = 6 * 1.5 * 1.5;

      QuickLinearRegression qlr = new QuickLinearRegression();
      foreach (var (x, y) in _data)
      {
        qlr.Add(x, y);
      }
      double intercept = qlr.GetA0();
      double slope = qlr.GetA1();

      AssertEx.AreEqual(expectedIntercept, intercept, 1e-10, 0.0, "Intercept test");
      AssertEx.AreEqual(expectedSlope, slope, 1e-10, 0.0, "Slope test");

      AssertEx.AreEqual(expectedIntercept + expectedSlope * 0, qlr.GetYOfX(0), 1e-10, 0.0, "Y of X=0 test");
      AssertEx.AreEqual(expectedIntercept + expectedSlope * 1, qlr.GetYOfX(1), 1e-10, 0.0, "Y of X=1 test");
      AssertEx.AreEqual(expectedIntercept + expectedSlope * 2, qlr.GetYOfX(2), 1e-10, 0.0, "Y of X=2 test");
      // Calculate the sum of squared errors

      AssertEx.AreEqual(expectedChiSquared, qlr.SumChiSquared(), 1e-10, 0.0, "Chi squared test");
      AssertEx.AreEqual(expectedChiSquared / (qlr.N - 2), qlr.SigmaSquared(), 1e-10, 0.0, "Sigma squared test");

      AssertEx.AreEqual(0.88105726872246696035242, qlr.RSquared(), 1e-10, 0.0, "Sigma test");
      AssertEx.AreEqual(0.85132158590308370044053, qlr.AdjustedRSquared(), 1e-10, 0.0, "Sigma test");

      var cov = qlr.GetCovarianceMatrix();

      AssertEx.AreEqual(1.40625, cov[0, 0], 1e-10, 0);
      AssertEx.AreEqual(-0.84375, cov[0, 1], 1e-10, 0);
      AssertEx.AreEqual(-0.84375, cov[1, 0], 1e-10, 0);
      AssertEx.AreEqual(0.84375, cov[1, 1], 1e-10, 0);

      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(0, cov), 1E-10, 0);
      AssertEx.AreEqual(0.75, qlr.GetYErrorOfX(1, cov), 1E-10, 0);
      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(2, cov), 1E-10, 0);

      var cb0 = qlr.GetConfidenceBand(0, 0.95, cov);
      AssertEx.AreEqual(-0.7924588740690616066624, cb0.yLower, 1e-10, 0);
      AssertEx.AreEqual(5.7924588740690616066624, cb0.yUpper, 1e-10, 0);

      var cb1 = qlr.GetConfidenceBand(1, 0.95, cov);
      AssertEx.AreEqual(5.4176661711016542316477, cb1.yLower, 1e-10, 0);
      AssertEx.AreEqual(9.5823338288983457683523, cb1.yUpper, 1e-10, 0);

      var cb2 = qlr.GetConfidenceBand(2, 0.95, cov);
      AssertEx.AreEqual(9.2075411259309383933376, cb2.yLower, 1e-10, 0);
      AssertEx.AreEqual(15.7924588740690616066624, cb2.yUpper, 1e-10, 0);

      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetA0Error(), 1e-10, 0);
      AssertEx.AreEqual(0.91855865354369178682398, qlr.GetA1Error(), 1e-10, 0);
    }
  }
}
