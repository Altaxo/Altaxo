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
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.Chemistry
{
  public class MassBasedFloryDistributionWithFixedGaussianBroadeningTest
  {
    [Fact]
    public void TestDerivatives()
    {
      foreach (var expectedAccuracy in new double[] { 1E-8, 1E-7, 1E-6, 1E-5, 1E-4, 1E-3 })
      {
        // left case
        double sigma = 0.125;
        double area = 5;
        var tau = 1 / 2047d;
        double MM = 3;
        double M = 2589;


        var expectedFunctionValue = 1.453867967636616;
        var expectedDerivativeWrtArea = 0.2907735935273231;
        var expectedDerivativeWrtTau = 4473.448913684046;

        var v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
        {
          IndependentVariableIsDecadicLogarithm = false,
          MolecularWeightOfMonomerUnit = MM,
          PolynomialCoefficientsForSigma = [sigma],
          Accuracy = expectedAccuracy,
        };

        // The accuracy of the function value is specified relative to the peak height
        // That is the reason why we have the term  expectedAccuracy * height / expectedFunctionValue for the relative accuracy here
        var (_, _, height, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);

        var y = v.GetYOfOneTerm(M, area, tau);
        AssertEx.AreEqual(expectedFunctionValue, y, 0, expectedAccuracy * height / expectedFunctionValue);

        var parameters = new double[] { area, tau };
        var X = new double[1];
        var Y = new double[1];
        X[0] = M;
        v.Evaluate(X, parameters, Y);
        AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, expectedAccuracy * height / expectedFunctionValue);

        var XX = Matrix<double>.Build.Dense(1, 1);
        XX[0, 0] = M;
        var YY = Vector<double>.Build.Dense(1);
        v.Evaluate(XX, parameters, YY, null);
        AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, expectedAccuracy * height / expectedFunctionValue);

        var DY = Matrix<double>.Build.Dense(1, 2);
        v.EvaluateDerivative(XX, parameters, null, DY, null);
        AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, expectedAccuracy * height / expectedFunctionValue);
        AssertEx.AreEqual(expectedDerivativeWrtTau, DY[0, 1], 0, expectedAccuracy * height / expectedFunctionValue);

        // right case
        area = 7;
        tau = 1 / 2047d;
        MM = 3;
        M = 25896;

        expectedFunctionValue = 4.305794261946497;
        expectedDerivativeWrtArea = 0.6151134659923567;
        expectedDerivativeWrtTau = -15521.67751589214;

        v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
        {
          IndependentVariableIsDecadicLogarithm = false,
          MolecularWeightOfMonomerUnit = MM,
          PolynomialCoefficientsForSigma = [sigma],
          Accuracy = expectedAccuracy,
        };
        (_, _, height, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);

        y = v.GetYOfOneTerm(M, area, tau);
        AssertEx.AreEqual(expectedFunctionValue, y, 0, expectedAccuracy * height / expectedFunctionValue);

        parameters = new double[] { area, tau };
        X = new double[1];
        Y = new double[1];
        X[0] = M;
        v.Evaluate(X, parameters, Y);
        AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, expectedAccuracy * height / expectedFunctionValue);


        XX[0, 0] = M;
        v.Evaluate(XX, parameters, YY, null);
        v.EvaluateDerivative(XX, parameters, null, DY, null);

        AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, expectedAccuracy * height / expectedFunctionValue);
        AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, expectedAccuracy * height / expectedFunctionValue);
        AssertEx.AreEqual(expectedDerivativeWrtTau, DY[0, 1], 0, expectedAccuracy * height / expectedFunctionValue);
      }
    }

    /// <summary>
    /// Same as <see cref="TestDerivatives"/>, but now M is given as log10(M).
    /// </summary>
    [Fact]
    public void TestDerivativesLog10()
    {
      foreach (var expectedAccuracy in new double[] { 1E-8, 1E-7, 1E-6, 1E-5, 1E-4, 1E-3 })
      {
        // left case
        double sigma = 0.125;
        double area = 5;
        var tau = 1 / 2047d;
        double MM = 3;
        double M = 2589;
        double log10M = Math.Log10(M);


        var expectedFunctionValue = 1.453867967636616;
        var expectedDerivativeWrtArea = 0.2907735935273231;
        var expectedDerivativeWrtTau = 4473.448913684046;

        var v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
        {
          IndependentVariableIsDecadicLogarithm = true,
          MolecularWeightOfMonomerUnit = MM,
          PolynomialCoefficientsForSigma = [sigma],
          Accuracy = expectedAccuracy,
        };

        // The accuracy of the function value is specified relative to the peak height
        // That is the reason why we have the term  expectedAccuracy * height / expectedFunctionValue for the relative accuracy here
        var (_, _, height, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);

        var y = v.GetYOfOneTerm(M, area, tau);
        AssertEx.AreEqual(expectedFunctionValue, y, 0, expectedAccuracy * height / expectedFunctionValue);

        var parameters = new double[] { area, tau };
        var X = new double[1];
        var Y = new double[1];
        X[0] = log10M;
        v.Evaluate(X, parameters, Y);
        AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, expectedAccuracy * height / expectedFunctionValue);

        var XX = Matrix<double>.Build.Dense(1, 1);
        XX[0, 0] = log10M;
        var YY = Vector<double>.Build.Dense(1);
        v.Evaluate(XX, parameters, YY, null);
        AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, expectedAccuracy * height / expectedFunctionValue);

        var DY = Matrix<double>.Build.Dense(1, 2);
        v.EvaluateDerivative(XX, parameters, null, DY, null);
        AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, expectedAccuracy * height / expectedFunctionValue);
        AssertEx.AreEqual(expectedDerivativeWrtTau, DY[0, 1], 0, expectedAccuracy * height / expectedFunctionValue);

        // right case
        area = 7;
        tau = 1 / 2047d;
        MM = 3;
        M = 25896;
        log10M = Math.Log10(M);

        expectedFunctionValue = 4.305794261946497;
        expectedDerivativeWrtArea = 0.6151134659923567;
        expectedDerivativeWrtTau = -15521.67751589214;

        v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
        {
          IndependentVariableIsDecadicLogarithm = true,
          MolecularWeightOfMonomerUnit = MM,
          PolynomialCoefficientsForSigma = [sigma],
          Accuracy = expectedAccuracy,
        };
        (_, _, height, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);

        y = v.GetYOfOneTerm(M, area, tau);
        AssertEx.AreEqual(expectedFunctionValue, y, 0, expectedAccuracy * height / expectedFunctionValue);

        parameters = new double[] { area, tau };
        X = new double[1];
        Y = new double[1];
        X[0] = log10M;
        v.Evaluate(X, parameters, Y);
        AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, expectedAccuracy * height / expectedFunctionValue);

        XX[0, 0] = log10M;
        v.Evaluate(XX, parameters, YY, null);
        v.EvaluateDerivative(XX, parameters, null, DY, null);

        AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, expectedAccuracy * height / expectedFunctionValue);
        AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, expectedAccuracy * height / expectedFunctionValue);
        AssertEx.AreEqual(expectedDerivativeWrtTau, DY[0, 1], 0, expectedAccuracy * height / expectedFunctionValue);
      }
    }

    [Fact]
    public void TestPositionAreaHeightFWHM()
    {
      // left case
      double sigma = 0.25;
      var area = 66;
      var tau = 1d;

      // logarithmic
      var v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
      {
        IndependentVariableIsDecadicLogarithm = true,
        MolecularWeightOfMonomerUnit = 1,
        PolynomialCoefficientsForSigma = [sigma],
      };

      var (position1, area1, height1, fwhm1) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);
      var (position2, _, area2, _, height2, _, fwhm2, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau], null);

      AssertEx.AreEqual(0.2581787109375, position1, 0, 1E-2);
      AssertEx.AreEqual(0.2581787109375, position2, 0, 1E-2);
      AssertEx.AreEqual(area, area1, 0, 1E-2);
      AssertEx.AreEqual(area, area2, 0, 1E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height1, 0, 5E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height2, 0, 5E-2);
      AssertEx.AreEqual(0.954886102117598, fwhm1, 0, 5E-2);
      AssertEx.AreEqual(0.954886102117598, fwhm2, 0, 5E-2);


      tau = 17 / 10000d;
      v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
      {
        IndependentVariableIsDecadicLogarithm = true,
        MolecularWeightOfMonomerUnit = 17,
        PolynomialCoefficientsForSigma = [sigma],
      };

      (position1, area1, height1, fwhm1) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);
      (position2, _, area2, _, height2, _, fwhm2, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau], null);

      AssertEx.AreEqual(4.2581787109375, position1, 0, 1E-2);
      AssertEx.AreEqual(4.2581787109375, position2, 0, 1E-2);
      AssertEx.AreEqual(area, area1, 0, 1E-2);
      AssertEx.AreEqual(area, area2, 0, 1E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height1, 0, 5E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height2, 0, 5E-2);
      AssertEx.AreEqual(0.954886102117598, fwhm1, 0, 5E-2);
      AssertEx.AreEqual(0.954886102117598, fwhm2, 0, 5E-2);


      // non-logarithmic
      tau = 1;
      v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
      {
        IndependentVariableIsDecadicLogarithm = false,
        MolecularWeightOfMonomerUnit = 1,
        PolynomialCoefficientsForSigma = [sigma],
      };
      (position1, area1, height1, fwhm1) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);
      (position2, _, area2, _, height2, _, fwhm2, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau], null);


      AssertEx.AreEqual(1.8120856072664995, position1, 0, 1E-2);
      AssertEx.AreEqual(1.8120856072664995, position2, 0, 1E-2);
      AssertEx.AreEqual(area, area1, 0, 1E-2);
      AssertEx.AreEqual(area, area2, 0, 1E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height1, 0, 5E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height2, 0, 5E-2);
      AssertEx.AreEqual(4.4998745594595775, fwhm1, 0, 5E-2);
      AssertEx.AreEqual(4.4998745594595775, fwhm2, 0, 5E-2);

      tau = 17 / 10000d;
      v = new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1)
      {
        IndependentVariableIsDecadicLogarithm = false,
        MolecularWeightOfMonomerUnit = 17,
        PolynomialCoefficientsForSigma = [sigma],
      };
      (position1, area1, height1, fwhm1) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau]);
      (position2, _, area2, _, height2, _, fwhm2, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters([area, tau], null);

      AssertEx.AreEqual(10000 * 1.8120856072664995, position1, 0, 1E-2);
      AssertEx.AreEqual(10000 * 1.8120856072664995, position2, 0, 1E-2);
      AssertEx.AreEqual(area, area1, 0, 1E-2);
      AssertEx.AreEqual(area, area2, 0, 1E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height1, 0, 5E-2);
      AssertEx.AreEqual(0.969143879832546 * area, height2, 0, 5E-2);
      AssertEx.AreEqual(10000 * 4.4998745594595775, fwhm1, 0, 5E-2);
      AssertEx.AreEqual(10000 * 4.4998745594595775, fwhm2, 0, 5E-2);
    }
  }
}
