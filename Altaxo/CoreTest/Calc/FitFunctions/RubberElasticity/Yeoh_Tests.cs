#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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


namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  public class Yeoh_Tests
  {
    [Fact]
    public void TestBiaxial()
    {

      var lambda = 5 / 2d;
      var C10 = 13;
      var C20 = 17;
      var C30 = 7;
      var expectedFunctionValue = 11165.819514238771200;
      var expectedDerivativeWrtC10 = 4.9795200000000000000;
      var expectedDerivativeWrtC20 = 94.865831424000000000;
      var expectedDerivativeWrtC30 = 1355.4809457186816000;

      // test function value y of one term
      var v = new YeohBiaxial();
      var y = YeohBiaxial.Evaluate(lambda - 1, C10, C20, C30);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { C10, C20, C30 };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC10, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC20, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC30, DY[0, 2], 0, 1e-14);
    }



    [Fact]
    public void TestBiaxial_Lambda1ShouldResultinZeroStress()
    {

      var lambda = 1;
      var C10 = 13;
      var C20 = 17;
      var C30 = 7;
      var expectedFunctionValue = 0d;

      // test function value y of one term
      var v = new YeohBiaxial();
      var y = YeohBiaxial.Evaluate(lambda - 1, C10, C20, C30);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 0);


      var parameters = new double[] { C10, C20, C30 };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
    }

    [Fact]
    public void TestPlanar()
    {

      var lambda = 5 / 2d;
      var C10 = 13;
      var C20 = 17;
      var C30 = 7;
      var expectedFunctionValue = 2783.6176872000000000;
      var expectedDerivativeWrtC10 = 4.8720000000000000000;
      var expectedDerivativeWrtC20 = 42.971040000000000000;
      var expectedDerivativeWrtC30 = 284.25342960000000000;

      // test function value y of one term
      var v = new YeohPlanar();
      var y = YeohPlanar.Evaluate(lambda - 1, C10, C20, C30);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { C10, C20, C30 };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC10, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC20, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC30, DY[0, 2], 0, 1e-14);
    }




    [Fact]
    public void TestPlanar_Lambda1ShouldResultinZeroStress()
    {

      var lambda = 1;
      var C10 = 13;
      var C20 = 17;
      var C30 = 7;
      var expectedFunctionValue = 0d;

      // test function value y of one term
      var v = new YeohPlanar();
      var y = YeohPlanar.Evaluate(lambda - 1, C10, C20, C30);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 0);


      var parameters = new double[] { C10, C20, C30 };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
    }

    [Fact]
    public void TestUniaxial()
    {

      var lambda = 5 / 2d;
      var C10 = 13;
      var C20 = 17;
      var C30 = 7;
      var expectedFunctionValue = 2317.3137;
      var expectedDerivativeWrtC10 = 4.68;
      var expectedDerivativeWrtC20 = 37.908;
      var expectedDerivativeWrtC30 = 230.2911;

      // test function value y of one term
      var v = new YeohUniaxial();
      var y = YeohUniaxial.Evaluate(lambda - 1, C10, C20, C30);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { C10, C20, C30 };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC10, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC20, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtC30, DY[0, 2], 0, 1e-14);
    }



    [Fact]
    public void TestUniaxial_Lambda1ShouldResultinZeroStress()
    {

      var lambda = 1d;
      var C10 = 13;
      var C20 = 17;
      var C30 = 7;
      var expectedFunctionValue = 0d;

      // test function value y of one term
      var v = new YeohUniaxial();
      var y = YeohUniaxial.Evaluate(lambda - 1, C10, C20, C30);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 0);


      var parameters = new double[] { C10, C20, C30 };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
    }
  }
}
