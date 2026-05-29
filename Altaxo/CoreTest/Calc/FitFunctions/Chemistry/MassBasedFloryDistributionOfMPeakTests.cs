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
  public class MassBasedFloryDistributionOfMPeakTests
  {
    [Fact]
    public void TestDerivatives()
    {
      // left case
      double area = 5;
      double M = 1023;
      double MPeak = 6 * 2047;

      var expectedFunctionValue = 0.27046539847900559759;
      var expectedDerivativeWrtArea = 0.054093079695801119518;
      var expectedDerivativeWrtMPeak = -0.000040374145239203581800;

      var v = new MassBasedFloryDistributionOfMPeak { NumberOfTerms = 1, IndependentVariableIsDecadicLogarithm = false };
      var parameters = new double[] { area, MPeak };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = M;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtMPeak, DY[0, 1], 0, 1E-12);

      // right case
      area = 7;
      M = 32787;
      MPeak = 6 * 2047;

      expectedFunctionValue = 2.2055984181023158547;
      expectedDerivativeWrtArea = 0.31508548830033083639;
      expectedDerivativeWrtMPeak = 0.00059962264363302359011;

      v = new MassBasedFloryDistributionOfMPeak { NumberOfTerms = 1, IndependentVariableIsDecadicLogarithm = false };
      parameters = new double[] { area, MPeak };


      X[0, 0] = M;
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtMPeak, DY[0, 1], 0, 1E-12);

    }
  }
}
