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
  public class GaussAreaTest
  {
    [Fact]
    public void TestFunctionValueAndDerivatives()
    {
      var v = new GaussArea();

      double x, area, position, sigma;
      double expectedFunctionValue, expectedDerivativeWrtArea, expectedDerivativeWrtPosition, expectedDerivativeWrtSigma;

      double y;
      double[] X, Y, parameters;
      Matrix<double> XX, DY;
      Vector<double> YY;

      x = 2;
      area = 3;
      position = 5;
      sigma = 7;
      expectedFunctionValue = 0.15597288073520725109;
      expectedDerivativeWrtArea = 0.051990960245069083696;
      expectedDerivativeWrtPosition = -0.0095493600450126888422;
      expectedDerivativeWrtSigma = -0.018189257228595597795;

      // test function value y of one term
      y = GaussArea.GetYOfOneTerm(x, area, position, sigma);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-14);

      // test one point function evaluation
      parameters = new double[] { area, position, sigma };
      X = new double[1];
      X[0] = x;
      Y = new double[1];
      v.Evaluate(X, parameters, Y);
      AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, 1E-14);

      // test multiple point function evaluation
      XX = Matrix<double>.Build.Dense(1, 1);
      XX[0, 0] = x;
      YY = Vector<double>.Build.Dense(1);
      v.Evaluate(XX, parameters, YY, null);
      AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, 1E-14);

      // test derivatives
      DY = Matrix<double>.Build.Dense(1, v.NumberOfParameters);
      v.EvaluateDerivative(XX, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-14);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-14);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 2], 0, 1E-14);


      x = 6;
      area = 3;
      position = 5;
      sigma = 7;
      expectedFunctionValue = 0.16923948854154042961;
      expectedDerivativeWrtArea = 0.056413162847180143203;
      expectedDerivativeWrtPosition = 0.0034538671130926618288;
      expectedDerivativeWrtSigma = -0.023683660204063966826;

      // test function value y of one term
      y = GaussArea.GetYOfOneTerm(x, area, position, sigma);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-14);

      // test one point function evaluation
      parameters = new double[] { area, position, sigma };
      X = new double[1];
      X[0] = x;
      Y = new double[1];
      v.Evaluate(X, parameters, Y);
      AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, 1E-14);

      // test multiple point function evaluation
      XX = Matrix<double>.Build.Dense(1, 1);
      XX[0, 0] = x;
      YY = Vector<double>.Build.Dense(1);
      v.Evaluate(XX, parameters, YY, null);
      AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, 1E-14);

      // test derivatives
      DY = Matrix<double>.Build.Dense(1, v.NumberOfParameters);
      v.EvaluateDerivative(XX, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-14);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-14);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 2], 0, 1E-14);

    }


  }
}
