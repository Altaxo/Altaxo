#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.Probability
{
  public class VoigtAreaTest
  {
    [Fact]
    public void TestHeight()
    {
      var v = new VoigtArea();

      const double areaG = 13;
      const double positionG = 77;
      var parameters = new double[] { areaG, positionG, 0, 0 };

      double sigma = 1;
      double gamma;
      parameters[2] = sigma;
      int i;
      double d;
      for (i = 0, d = -1; d < 1; ++i, d += 1 / 64.0)
      {
        gamma = (1 + d) / (1 - d);
        parameters[3] = gamma;

        var (pos, _, area, _, height, _, fwhm, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);

        AssertEx.AreEqual(areaG, area, 0, 1E-14);
        AssertEx.AreEqual(positionG, pos, 0, 1E-14);

        var heightExpected = Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(0, sigma, gamma) * areaG;
        AssertEx.AreEqual(heightExpected, height, 0, 1E-7);

        var fwhmExpected = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(sigma, gamma);
        AssertEx.AreEqual(fwhmExpected, fwhm, 0, Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximumApproximationMaximalRelativeError);
      }

      gamma = 1;
      parameters[3] = gamma;
      for (i = 0, d = -1; d < 1; ++i, d += 1 / 64.0)
      {
        sigma = (1 + d) / (1 - d);
        parameters[2] = sigma;

        var (pos, _, area, _, height, _, fwhm, _) = v.GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);

        AssertEx.AreEqual(areaG, area, 0, 1E-14);
        AssertEx.AreEqual(positionG, pos, 0, 1E-14);

        var heightExpected = Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(0, sigma, gamma) * areaG;
        AssertEx.AreEqual(heightExpected, height, 0, 1E-7);

        var fwhmExpected = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(sigma, gamma);
        AssertEx.AreEqual(fwhmExpected, fwhm, 0, Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximumApproximationMaximalRelativeError);
      }

    }


    [Fact]
    public void TestDerivatives()
    {
      var v = new VoigtArea();

      // General case
      double area = 17;
      double position = 7;
      double sigma = 3;
      double gamma = 5;

      double expectedFunctionValue = 0.80988293632277722571;
      double expectedDerivativeWrtArea = 0.047640172724869248571;
      double expectedDerivativeWrtPosition = 0.054020556058543443380;
      double expectedDerivativeWrtSigma = -0.065721156908731982756;
      double expectedDerivativeWrtGamma = -0.100935670695898878136;


      var parameters = new double[] { area, position, sigma, gamma };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = 9;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtGamma, DY[0, 3], 0, 1E-12);


      // Case gamma == 0

      area = 17;
      position = 7;
      sigma = 3;
      gamma = 0;

      expectedFunctionValue = 1.8102053646266625238;
      expectedDerivativeWrtArea = 0.106482668507450736697;
      expectedDerivativeWrtPosition = 0.40226785880592500530;
      expectedDerivativeWrtSigma = -0.33522321567160417108;
      expectedDerivativeWrtGamma = -0.37031134022911140635;

      parameters = new double[] { area, position, sigma, gamma };

      X[0, 0] = 9;

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtGamma, DY[0, 3], 0, 1E-12);

      // Case sigma == 0

      area = 17;
      position = 7;
      sigma = 0;
      gamma = 5;

      expectedFunctionValue = 0.93297725260766231313;
      expectedDerivativeWrtArea = 0.054881014859274253713;
      expectedDerivativeWrtPosition = 0.12868651760105687078;
      expectedDerivativeWrtSigma = 0;
      expectedDerivativeWrtGamma = -0.13512084348110971432;

      parameters = new double[] { area, position, sigma, gamma };

      X[0, 0] = 9;

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtGamma, DY[0, 3], 0, 1E-12);
    }
  }
}
