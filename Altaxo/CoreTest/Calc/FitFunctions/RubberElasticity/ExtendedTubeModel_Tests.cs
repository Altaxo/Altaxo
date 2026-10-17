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
  public class ExtendedTubeModel_Tests
  {
    [Fact]
    public void TestBiaxial()
    {

      var lambda = 5 / 2d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 13 / 17d;
      var expectedFunctionValue = 52.105078777852440127;
      var expectedDerivativeWrtGc = 3.6949873794489784447;
      var expectedDerivativeWrtDelta = 69.599684597132619675;
      var expectedDerivativeWrtGe = 3.7291015126823186176;
      var expectedDerivativeWrtBeta = 37.228703534191907985;

      // test function value y of one term
      var v = new ExtendedTubeModelBiaxial();
      var y = ExtendedTubeModelBiaxial.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGc, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtDelta, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGe, DY[0, 2], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtBeta, DY[0, 3], 0, 1e-14);
    }

    [Fact]
    public void TestBiaxial_BetaZero()
    {

      var lambda = 5 / 2d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 0;
      var expectedFunctionValue = 35.275037459824629055;
      var expectedDerivativeWrtGc = 3.6949873794489784447;
      var expectedDerivativeWrtDelta = 69.599684597132619675;
      var expectedDerivativeWrtGe = 2.1990977564979721564;
      var expectedDerivativeWrtBeta = 11.082570910203866425;

      // test function value y of one term
      var v = new ExtendedTubeModelBiaxial();
      var y = ExtendedTubeModelBiaxial.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGc, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtDelta, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGe, DY[0, 2], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtBeta, DY[0, 3], 0, 1e-14);
    }

    [Fact]
    public void TestBiaxial_Lambda1ShouldResultinZeroStress()
    {

      var lambda = 1d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 13 / 17d;
      var expectedFunctionValue = 0d;

      // test function value y of one term
      var v = new ExtendedTubeModelBiaxial();
      var y = ExtendedTubeModelBiaxial.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 0);


      var parameters = new double[] { Gc, delta, Ge, beta };

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
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 13 / 17d;
      var expectedFunctionValue = 25.960142260973748725;
      var expectedDerivativeWrtGc = 2.8270084703365708075;
      var expectedDerivativeWrtDelta = 18.896905533785137698;
      var expectedDerivativeWrtGe = 1.5890106227240033002;
      var expectedDerivativeWrtBeta = 3.6237722076565059778;

      // test function value y of one term
      var v = new ExtendedTubeModelPlanar();
      var y = ExtendedTubeModelPlanar.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGc, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtDelta, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGe, DY[0, 2], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtBeta, DY[0, 3], 0, 1e-14);
    }

    [Fact]
    public void TestPlanar_BetaZero()
    {

      var lambda = 5 / 2d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 0;
      var expectedFunctionValue = 24.607742291994841570;
      var expectedDerivativeWrtGc = 2.8270084703365708075;
      var expectedDerivativeWrtDelta = 18.896905533785137698;
      var expectedDerivativeWrtGe = 1.4660651709986481043;
      var expectedDerivativeWrtBeta = 0;

      // test function value y of one term
      var v = new ExtendedTubeModelPlanar();
      var y = ExtendedTubeModelPlanar.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGc, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtDelta, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGe, DY[0, 2], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtBeta, DY[0, 3], 1E-14, 1e-14);
    }


    [Fact]
    public void TestPlanar_Lambda1ShouldResultinZeroStress()
    {

      var lambda = 1d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 13 / 17d;
      var expectedFunctionValue = 0d;

      // test function value y of one term
      var v = new ExtendedTubeModelPlanar();
      var y = ExtendedTubeModelPlanar.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 0);


      var parameters = new double[] { Gc, delta, Ge, beta };

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
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 13 / 17d;
      var expectedFunctionValue = 18.640834652012225681;
      var expectedDerivativeWrtGc = 2.6718586094300798935;
      var expectedDerivativeWrtDelta = 15.848860932772262064;
      var expectedDerivativeWrtGe = 0.96593262033836236368;
      var expectedDerivativeWrtBeta = -1.1778115740692469717;

      // test function value y of one term
      var v = new ExtendedTubeModelUniaxial();
      var y = ExtendedTubeModelUniaxial.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGc, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtDelta, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGe, DY[0, 2], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtBeta, DY[0, 3], 0, 1e-14);
    }

    [Fact]
    public void TestUniaxial_BetaZero()
    {

      var lambda = 5 / 2d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 0;
      var expectedFunctionValue = 20.110613489029086541;
      var expectedDerivativeWrtGc = 2.6718586094300798935;
      var expectedDerivativeWrtDelta = 15.848860932772262064;
      var expectedDerivativeWrtGe = 1.0995488782489860782;
      var expectedDerivativeWrtBeta = -2.7706427275509666061;

      // test function value y of one term
      var v = new ExtendedTubeModelUniaxial();
      var y = ExtendedTubeModelUniaxial.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGc, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtDelta, DY[0, 1], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtGe, DY[0, 2], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtBeta, DY[0, 3], 0, 1e-14);
    }

    [Fact]
    public void TestUniaxial_Lambda1ShouldResultinZeroStress()
    {

      var lambda = 1d;
      var Gc = 3;
      var delta = 1 / 7d;
      var Ge = 11;
      var beta = 13 / 17d;
      var expectedFunctionValue = 0d;

      // test function value y of one term
      var v = new ExtendedTubeModelUniaxial();
      var y = ExtendedTubeModelUniaxial.Evaluate(lambda - 1, Gc, delta, Ge, beta);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 0);


      var parameters = new double[] { Gc, delta, Ge, beta };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
    }
  }
}
