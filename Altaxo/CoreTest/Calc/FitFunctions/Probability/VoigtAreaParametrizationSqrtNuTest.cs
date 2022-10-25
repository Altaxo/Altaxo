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
  public class VoigtAreaParametrizationSqrtNuTest
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
      var v = new VoigtAreaParametrizationSqrtNu();

      // General case
      double area = 17;
      double position = 7;
      double w = 3;
      double nu = 5 / 11d;

      double expectedFunctionValue = 1.50424279316154299538750;
      double expectedDerivativeWrtArea = 0.0884848701859731173757352;
      double expectedDerivativeWrtPosition = 0.376330137044998775120385;
      double expectedDerivativeWrtW = -0.250527506357181815048910;
      double expectedDerivativeWrtNu = 0.527746551656330810292417;


      var parameters = new double[] { area, position, w, nu };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = 9;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtNu, DY[0, 3], 0, 1E-12);


      // Case gamma close to 0 (Lorentz limit

      area = 17;
      position = 7;
      w = 3;
      nu = 1 / 16383d;

      // the expected values are those of the Lorentz limit!
      expectedFunctionValue = 1.248795660995215956056557;
      expectedDerivativeWrtArea = 0.0734561275808747703548694;
      expectedDerivativeWrtPosition = 0.384232051961498798779317;
      expectedDerivativeWrtW = -0.160096688317291166158049;
      expectedDerivativeWrtNu = 0.679795168854959413224945;

      parameters = new double[] { area, position, w, nu };

      X[0, 0] = 9;

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-6);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtNu, DY[0, 3], 0, 1E-12);

      // Case sigma == 0

      area = 17;
      position = 7;
      w = 3;
      nu = 1;

      expectedFunctionValue = 1.81020536462666252384324;
      expectedDerivativeWrtArea = 0.1064826685074507366966609;
      expectedDerivativeWrtPosition = 0.402267858805925005298497;
      expectedDerivativeWrtW = -0.335223215671604171082081;
      expectedDerivativeWrtNu = 0.608099197179927962416859;

      parameters = new double[] { area, position, w, nu };

      X[0, 0] = 9;

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtNu, DY[0, 3], 0, 1E-12);
    }
  }
}
