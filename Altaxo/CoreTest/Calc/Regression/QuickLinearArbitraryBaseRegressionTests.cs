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
  public class QuickLinearArbitraryBaseRegressionTests
  {

    #region Order 1

    private static (double x, double y)[] _dataOrder1 = new (double x, double y)[] { (0, 1), (0, 4), (1, 6), (1, 9), (2, 11), (2, 14) };

    /// <summary>
    /// Tests parameters and characteristic values of the regression line for order 1.
    /// </summary>  
    [Fact]
    public void TestInterceptAndSlopeOrder1()
    {
      const double expectedIntercept = 2.5;
      const double expectedSlope = 5.0;
      const double expectedChiSquared = 6 * 1.5 * 1.5;

      var qlr = new QuickLinearArbitraryBaseRegression(x => 1, x => x);
      foreach (var (x, y) in _dataOrder1)
      {
        qlr.Add(x, y);
      }
      double intercept = qlr.GetParameter(0);
      double slope = qlr.GetParameter(1);

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

      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(0), 1E-10, 0);
      AssertEx.AreEqual(0.75, qlr.GetYErrorOfX(1), 1E-10, 0);
      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(2), 1E-10, 0);

      var cb0 = qlr.GetConfidenceBand(0, 0.95);
      AssertEx.AreEqual(-0.7924588740690616066624, cb0.yLower, 1e-10, 0);
      AssertEx.AreEqual(5.7924588740690616066624, cb0.yUpper, 1e-10, 0);

      var cb1 = qlr.GetConfidenceBand(1, 0.95);
      AssertEx.AreEqual(5.4176661711016542316477, cb1.yLower, 1e-10, 0);
      AssertEx.AreEqual(9.5823338288983457683523, cb1.yUpper, 1e-10, 0);

      var cb2 = qlr.GetConfidenceBand(2, 0.95);
      AssertEx.AreEqual(9.2075411259309383933376, cb2.yLower, 1e-10, 0);
      AssertEx.AreEqual(15.7924588740690616066624, cb2.yUpper, 1e-10, 0);

      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetParameterError(0), 1e-10, 0);
      AssertEx.AreEqual(0.91855865354369178682398, qlr.GetParameterError(1), 1e-10, 0);
    }

    /// <summary>
    /// Tests parameters and characteristic values of the regression line for order 1, but now with exchanged order of base functions.
    /// </summary>  
    [Fact]
    public void TestInterceptAndSlopeExchangedBaseFunctionsOrder1()
    {
      const double expectedIntercept = 2.5;
      const double expectedSlope = 5.0;
      const double expectedChiSquared = 6 * 1.5 * 1.5;
      const int idxIntercept = 1;
      const int idxSlope = 0;

      var qlr = new QuickLinearArbitraryBaseRegression(x => x, x => 1);
      foreach (var (x, y) in _dataOrder1)
      {
        qlr.Add(x, y);
      }
      double intercept = qlr.GetParameter(idxIntercept);
      double slope = qlr.GetParameter(idxSlope);

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

      AssertEx.AreEqual(1.40625, cov[idxIntercept, idxIntercept], 1e-10, 0);
      AssertEx.AreEqual(-0.84375, cov[idxIntercept, idxSlope], 1e-10, 0);
      AssertEx.AreEqual(-0.84375, cov[idxSlope, idxIntercept], 1e-10, 0);
      AssertEx.AreEqual(0.84375, cov[idxSlope, idxSlope], 1e-10, 0);

      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(0), 1E-10, 0);
      AssertEx.AreEqual(0.75, qlr.GetYErrorOfX(1), 1E-10, 0);
      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(2), 1E-10, 0);

      var cb0 = qlr.GetConfidenceBand(0, 0.95);
      AssertEx.AreEqual(-0.7924588740690616066624, cb0.yLower, 1e-10, 0);
      AssertEx.AreEqual(5.7924588740690616066624, cb0.yUpper, 1e-10, 0);

      var cb1 = qlr.GetConfidenceBand(1, 0.95);
      AssertEx.AreEqual(5.4176661711016542316477, cb1.yLower, 1e-10, 0);
      AssertEx.AreEqual(9.5823338288983457683523, cb1.yUpper, 1e-10, 0);

      var cb2 = qlr.GetConfidenceBand(2, 0.95);
      AssertEx.AreEqual(9.2075411259309383933376, cb2.yLower, 1e-10, 0);
      AssertEx.AreEqual(15.7924588740690616066624, cb2.yUpper, 1e-10, 0);

      AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetParameterError(idxIntercept), 1e-10, 0);
      AssertEx.AreEqual(0.91855865354369178682398, qlr.GetParameterError(idxSlope), 1e-10, 0);
    }

    /// <summary>
    /// Tests parameters and characteristic values of the regression line for order 1, but with an additional base function with fixed parameter.
    /// The fixed parameter is cycled through all positions.
    /// </summary>  
    [Fact]
    public void TestInterceptAndSlopeExchangedBaseFunctionsOrder1With1FixedParameter()
    {
      const double expectedIntercept = 2.5;
      const double expectedSlope = 5.0;
      const double expectedChiSquared = 6 * 1.5 * 1.5;


      for (int idxFixed = 0; idxFixed < 3; ++idxFixed)
      {
        var baseFunctions = new Func<double, double>[3];
        var fixedParameters = new double?[3];

        fixedParameters[idxFixed] = 13 / 8d;
        var idxIntercept = (idxFixed + 1) % 3;
        var idxSlope = (idxFixed + 2) % 3;
        baseFunctions[idxIntercept] = x => 1;
        baseFunctions[idxSlope] = x => x;
        baseFunctions[idxFixed] = x => x * x;

        var qlr = new QuickLinearArbitraryBaseRegression(baseFunctions, fixedParameters);
        foreach (var (xx, yy) in _dataOrder1)
        {
          qlr.Add(xx, yy + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](xx));
        }

        double intercept = qlr.GetParameter(idxIntercept);
        double slope = qlr.GetParameter(idxSlope);

        AssertEx.AreEqual(expectedIntercept, intercept, 1e-10, 0.0, "Intercept test");
        AssertEx.AreEqual(expectedSlope, slope, 1e-10, 0.0, "Slope test");
        AssertEx.AreEqual(fixedParameters[idxFixed]!.Value, qlr.GetParameter(idxFixed), 1e-10, 0.0, "Fixed parameter test");

        Func<double, double> expectedFunction = x => expectedIntercept + expectedSlope * x + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x);

        AssertEx.AreEqual(expectedFunction(0), qlr.GetYOfX(0), 1e-10, 0.0, "Y of X=0 test");
        AssertEx.AreEqual(expectedFunction(1), qlr.GetYOfX(1), 1e-10, 0.0, "Y of X=1 test");
        AssertEx.AreEqual(expectedFunction(2), qlr.GetYOfX(2), 1e-10, 0.0, "Y of X=2 test");
        // Calculate the sum of squared errors

        AssertEx.AreEqual(expectedChiSquared, qlr.SumChiSquared(), 1e-10, 0.0, "Chi squared test");
        AssertEx.AreEqual(expectedChiSquared / (qlr.N - 2), qlr.SigmaSquared(), 1e-10, 0.0, "Sigma squared test");

        AssertEx.AreEqual(0.88105726872246696035242, qlr.RSquared(), 1e-10, 0.0, "Sigma test");
        AssertEx.AreEqual(0.85132158590308370044053, qlr.AdjustedRSquared(), 1e-10, 0.0, "Sigma test");

        var cov = qlr.GetCovarianceMatrix();

        AssertEx.AreEqual(1.40625, cov[idxIntercept, idxIntercept], 1e-10, 0);
        AssertEx.AreEqual(-0.84375, cov[idxIntercept, idxSlope], 1e-10, 0);
        AssertEx.AreEqual(-0.84375, cov[idxSlope, idxIntercept], 1e-10, 0);
        AssertEx.AreEqual(0.84375, cov[idxSlope, idxSlope], 1e-10, 0);
        AssertEx.AreEqual(0, cov[idxFixed, idxFixed], 0, 0);
        AssertEx.AreEqual(0, cov[idxIntercept, idxFixed], 0, 0);
        AssertEx.AreEqual(0, cov[idxFixed, idxIntercept], 0, 0);
        AssertEx.AreEqual(0, cov[idxSlope, idxFixed], 0, 0);
        AssertEx.AreEqual(0, cov[idxFixed, idxSlope], 0, 0);


        AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(0), 1E-10, 0);
        AssertEx.AreEqual(0.75, qlr.GetYErrorOfX(1), 1E-10, 0);
        AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetYErrorOfX(2), 1E-10, 0);

        double x = 0;
        var cb0 = qlr.GetConfidenceBand(x, 0.95);
        AssertEx.AreEqual(-0.7924588740690616066624 + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x), cb0.yLower, 1e-10, 0);
        AssertEx.AreEqual(5.7924588740690616066624 + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x), cb0.yUpper, 1e-10, 0);

        x = 1;
        var cb1 = qlr.GetConfidenceBand(x, 0.95);
        AssertEx.AreEqual(5.4176661711016542316477 + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x), cb1.yLower, 1e-10, 0);
        AssertEx.AreEqual(9.5823338288983457683523 + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x), cb1.yUpper, 1e-10, 0);

        x = 2;
        var cb2 = qlr.GetConfidenceBand(x, 0.95);
        AssertEx.AreEqual(9.2075411259309383933376 + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x), cb2.yLower, 1e-10, 0);
        AssertEx.AreEqual(15.7924588740690616066624 + fixedParameters[idxFixed].Value * baseFunctions[idxFixed](x), cb2.yUpper, 1e-10, 0);

        AssertEx.AreEqual(Math.Sqrt(1.40625), qlr.GetParameterError(idxIntercept), 1e-10, 0);
        AssertEx.AreEqual(0.91855865354369178682398, qlr.GetParameterError(idxSlope), 1e-10, 0);
        AssertEx.AreEqual(0, qlr.GetParameterError(idxFixed), 1e-10, 0);
      }
    }


    #endregion

    #region Order 2

    private static (double x, double y)[] _dataOrder2 = new (double x, double y)[] { (0, 1), (0, 4), (1, 3), (1, 6), (2, -1), (2, 2) };

    [Fact]
    public void TestInterceptAndSlopeOrder2()
    {
      const double expectedA0 = 2.5;
      const double expectedA1 = 5.0;
      const double expectedA2 = -3.0;
      const double expectedSumOfChiSquared = 6 * 1.5 * 1.5;

      var qlr = new QuickLinearArbitraryBaseRegression(x => 1, x => x, x => x * x);
      foreach (var (x, y) in _dataOrder2)
      {
        qlr.Add(x, y);
      }
      double a0 = qlr.GetParameter(0);
      double a1 = qlr.GetParameter(1);
      double a2 = qlr.GetParameter(2);

      AssertEx.AreEqual(expectedA0, a0, 1e-10, 0.0, "Intercept test");
      AssertEx.AreEqual(expectedA1, a1, 1e-10, 0.0, "Slope test");
      AssertEx.AreEqual(expectedA2, a2, 1e-10, 0.0, "Second slope test");

      AssertEx.AreEqual(expectedA0 + expectedA1 * 0 + expectedA2 * 0 * 0, qlr.GetYOfX(0), 1e-10, 0.0, "Y of X=0 test");
      AssertEx.AreEqual(expectedA0 + expectedA1 * 1 + expectedA2 * 1 * 1, qlr.GetYOfX(1), 1e-10, 0.0, "Y of X=1 test");
      AssertEx.AreEqual(expectedA0 + expectedA1 * 2 + expectedA2 * 2 * 2, qlr.GetYOfX(2), 1e-10, 0.0, "Y of X=2 test");

      AssertEx.AreEqual(expectedSumOfChiSquared, qlr.SumChiSquared(), 1e-10, 0.0, "Chi squared test");
      AssertEx.AreEqual(expectedSumOfChiSquared / (qlr.N - 3), qlr.SigmaSquared(), 1e-10, 0.0, "Sigma squared test");
      AssertEx.AreEqual(Math.Sqrt(expectedSumOfChiSquared / (qlr.N - 3)), qlr.Sigma(), 1e-10, 0.0, "Sigma test");



      // Calculate the sum of squared errors
      double chiSquared = 0.0;
      foreach (var (x, y) in _dataOrder2)
      {
        double yPred = qlr.GetYOfX(x);
        chiSquared += (y - yPred) * (y - yPred);
      }
      AssertEx.AreEqual(expectedSumOfChiSquared, chiSquared, 1e-10, 0.0, "Chi squared test");
    }

    [Fact]
    public void TestInsufficientDataPointsOrder2()
    {
      var qlr = new QuickLinearArbitraryBaseRegression(x => 1, x => x, x => x * x);
      qlr.Add(1, 2);
      qlr.Add(2, 3);
      // Only two points, quadratic regression is not possible
      Assert.Throws<System.InvalidOperationException>(() => qlr.GetParameter(0));
      Assert.Throws<System.InvalidOperationException>(() => qlr.GetParameter(1));
      Assert.Throws<System.InvalidOperationException>(() => qlr.GetParameter(2));
    }

    [Fact]
    public void TestCovarianceMatrixOrder2()
    {
      var qlr = new QuickLinearArbitraryBaseRegression(x => 1, x => x, x => x * x);
      foreach (var (x, y) in _dataOrder2)
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

    #endregion
  }
}
