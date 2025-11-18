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

using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.Chemistry
{
  public class MassBasedFloryDistributionWithFixedGaussianBroadeningTest
  {
    [Fact]
    public void TestDerivatives()
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
        MolecularWeightOfMonomerUnit = MM,
        PolynomialCoefficientsForSigma = [sigma],
        Accuracy = 1E-6,
      };

      var y = v.GetYOfOneTerm(M, area, tau);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-5);

      var parameters = new double[] { area, tau };
      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = M;
      var FV = Vector<double>.Build.Dense(1);
      v.Evaluate(X, parameters, FV, null);
      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-5);

      var DY = Matrix<double>.Build.Dense(1, 2);
      v.EvaluateDerivative(X, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-5);
      AssertEx.AreEqual(expectedDerivativeWrtTau, DY[0, 1], 0, 1E-5);

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
        MolecularWeightOfMonomerUnit = MM,
        PolynomialCoefficientsForSigma = [sigma],
      };


      y = v.GetYOfOneTerm(M, area, tau);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-3);

      parameters = new double[] { area, tau };
      X[0, 0] = M;
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-3);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-3);
      AssertEx.AreEqual(expectedDerivativeWrtTau, DY[0, 1], 0, 1E-2);

    }
  }
}
