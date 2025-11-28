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

namespace Altaxo.Calc.FitFunctions.Transitions
{
  public class ErrorFunctionFromToTest
  {
    [Fact]
    public void TestFuncAndDerivatives_0_0()
    {
      // right case
      double x = 11;
      double from = -3;
      double to = 5;
      double xc = 7;
      double sigma = 9;


      var expectedFunctionValue = 2.3731148537449182431;
      var expectedDerivativeWrtFrom = 0.32836064328188521961;
      var expectedDerivativeWrtTo = 0.67163935671811478039;
      var expectedDerivativeWrtXc = -0.32126562656243866761;
      var expectedDerivativeWrtSigma = -0.14278472291663940783;

      var f = new ErrorFunctionFromTo();

      var y = ErrorFunctionFromTo.Evaluate(x, xc, sigma, [from], [to]);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-12);

      var parameters = new double[] { xc, sigma, from, to };
      var Y = new double[1];
      f.Evaluate([x], parameters, Y);
      AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, 1E-12);

      var XX = Matrix<double>.Build.Dense(2, 1);
      var YY = Vector<double>.Build.Dense(2);
      XX[0, 0] = x;
      XX[1, 0] = x;
      f.Evaluate(XX, parameters, YY, null);
      AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, 1E-12);
      AssertEx.AreEqual(expectedFunctionValue, YY[1], 0, 1E-12);

      var DY = Matrix<double>.Build.Dense(2, f.NumberOfParameters);
      f.EvaluateDerivative(XX, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[1, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[1, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtFrom, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtFrom, DY[1, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtTo, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtTo, DY[1, 3], 0, 1E-12);
    }

    [Fact]
    public void TestFuncAndDerivatives_0_0_AtNegativeInfinity()
    {
      // right case
      double x = double.NegativeInfinity;
      double xc = 7;
      double sigma = 9;
      double from = -3;
      double to = 5;


      var expectedFunctionValue = from;
      var expectedDerivativeWrtXc = 0;
      var expectedDerivativeWrtSigma = 0;
      var expectedDerivativeWrtFrom = 1;
      var expectedDerivativeWrtTo = 0;

      var f = new ErrorFunctionFromTo();

      var y = ErrorFunctionFromTo.Evaluate(x, xc, sigma, [from], [to]);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-12);

      var parameters = new double[] { xc, sigma, from, to };
      var Y = new double[1];
      f.Evaluate([x], parameters, Y);
      AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, 1E-12);

      var XX = Matrix<double>.Build.Dense(2, 1);
      var YY = Vector<double>.Build.Dense(2);
      XX[0, 0] = x;
      XX[1, 0] = x;
      f.Evaluate(XX, parameters, YY, null);
      AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, 1E-12);
      AssertEx.AreEqual(expectedFunctionValue, YY[1], 0, 1E-12);

      var DY = Matrix<double>.Build.Dense(2, f.NumberOfParameters);
      f.EvaluateDerivative(XX, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[1, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[1, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtFrom, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtFrom, DY[1, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtTo, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtTo, DY[1, 3], 0, 1E-12);
    }

    [Fact]
    public void TestFuncAndDerivatives_0_0_AtPositiveInfinity()
    {
      // right case
      double x = double.PositiveInfinity;
      double xc = 7;
      double sigma = 9;
      double from = -3;
      double to = 5;


      var expectedFunctionValue = to;
      var expectedDerivativeWrtXc = 0;
      var expectedDerivativeWrtSigma = 0;
      var expectedDerivativeWrtFrom = 0;
      var expectedDerivativeWrtTo = 1;

      var f = new ErrorFunctionFromTo();

      var y = ErrorFunctionFromTo.Evaluate(x, xc, sigma, [from], [to]);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-12);

      var parameters = new double[] { xc, sigma, from, to };
      var Y = new double[1];
      f.Evaluate([x], parameters, Y);
      AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, 1E-12);

      var XX = Matrix<double>.Build.Dense(2, 1);
      var YY = Vector<double>.Build.Dense(2);
      XX[0, 0] = x;
      XX[1, 0] = x;
      f.Evaluate(XX, parameters, YY, null);
      AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, 1E-12);
      AssertEx.AreEqual(expectedFunctionValue, YY[1], 0, 1E-12);

      var DY = Matrix<double>.Build.Dense(2, f.NumberOfParameters);
      f.EvaluateDerivative(XX, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[1, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[1, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtFrom, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtFrom, DY[1, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtTo, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtTo, DY[1, 3], 0, 1E-12);
    }



    [Fact]
    public void TestFuncAndDerivatives_3_2()
    {
      // right case
      double x = 11;
      double xc = 7;
      double sigma = 9;
      double a0 = -3;
      double a1 = 1 / 5d;
      double a2 = -1 / 3d;
      double a3 = 1 / 17d;
      double b0 = 5;
      double b1 = -1 / 13d;
      double b2 = 1 / 19d;


      var expectedFunctionValue = 19.269307819889110619;
      var expectedDerivativeWrtXc = 1.0697549861442739985;
      var expectedDerivativeWrtSigma = 0.47544666050856622155;
      var expectedDerivativeWrta0 = 0.32836064328188521961;
      var expectedDerivativeWrta1 = 3.6119670761007374157;
      var expectedDerivativeWrta2 = 39.731637837108111573;
      var expectedDerivativeWrta3 = 437.04801620818922730;
      var expectedDerivativeWrtb0 = 0.67163935671811478039;
      var expectedDerivativeWrtb1 = 7.3880329238992625843;
      var expectedDerivativeWrtb2 = 81.268362162891888427;

      var f = new ErrorFunctionFromTo(3, 2);

      var y = ErrorFunctionFromTo.Evaluate(x, xc, sigma, [a0, a1, a2, a3], [b0, b1, b2]);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1E-12);

      var parameters = new double[] { xc, sigma, a0, a1, a2, a3, b0, b1, b2 };
      var Y = new double[1];
      f.Evaluate([x], parameters, Y);
      AssertEx.AreEqual(expectedFunctionValue, Y[0], 0, 1E-12);

      var XX = Matrix<double>.Build.Dense(2, 1);
      var YY = Vector<double>.Build.Dense(2);
      XX[0, 0] = x;
      XX[1, 0] = x;
      f.Evaluate(XX, parameters, YY, null);
      AssertEx.AreEqual(expectedFunctionValue, YY[0], 0, 1E-12);
      AssertEx.AreEqual(expectedFunctionValue, YY[1], 0, 1E-12);

      var DY = Matrix<double>.Build.Dense(2, f.NumberOfParameters);
      f.EvaluateDerivative(XX, parameters, null, DY, null);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtXc, DY[1, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtSigma, DY[1, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta0, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta0, DY[1, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta1, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta1, DY[1, 3], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta2, DY[0, 4], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta2, DY[1, 4], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta3, DY[0, 5], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrta3, DY[1, 5], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtb0, DY[0, 6], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtb0, DY[1, 6], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtb1, DY[0, 7], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtb1, DY[1, 7], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtb2, DY[0, 8], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtb2, DY[1, 8], 0, 1E-12);
    }

    [Fact]
    public void TestParameterNames()
    {
      var f1 = new ErrorFunctionFromTo();
      Assert.Equal("xc", f1.ParameterName(0));
      Assert.Equal("sigma", f1.ParameterName(1));
      Assert.Equal("a0", f1.ParameterName(2));
      Assert.Equal("b0", f1.ParameterName(3));
      Assert.Throws<ArgumentOutOfRangeException>(() => f1.ParameterName(4));
      Assert.Throws<ArgumentOutOfRangeException>(() => f1.ParameterName(-1));

      var f2 = new ErrorFunctionFromTo(3, 2);
      Assert.Equal("xc", f2.ParameterName(0));
      Assert.Equal("sigma", f2.ParameterName(1));
      Assert.Equal("a0", f2.ParameterName(2));
      Assert.Equal("a1", f2.ParameterName(3));
      Assert.Equal("a2", f2.ParameterName(4));
      Assert.Equal("a3", f2.ParameterName(5));
      Assert.Equal("b0", f2.ParameterName(6));
      Assert.Equal("b1", f2.ParameterName(7));
      Assert.Equal("b2", f2.ParameterName(8));
      Assert.Throws<ArgumentOutOfRangeException>(() => f2.ParameterName(9));
      Assert.Throws<ArgumentOutOfRangeException>(() => f2.ParameterName(-1));

    }
  }
}
