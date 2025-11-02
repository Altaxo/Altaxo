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

namespace Altaxo.Calc.FitFunctions.General
{
  public class ExponentialOfPolynomialTests
  {
    [Fact]
    public void TestFunctionValues()
    {
      double x;
      var X = new double[1];
      var XX = Matrix<double>.Build.Dense(1, 1);
      var Y = new double[1];

      // ParameterSet 1: 3rd order a, 2nd order b
      var f = new ExponentialOfPolynomial(3, 2);
      var P = new double[f.NumberOfParameters];
      var D = Matrix<double>.Build.Dense(1, f.NumberOfParameters);
      x = 17 / 19d;
      P[0] = 3; // offset
      P[1] = 1; // a0
      P[2] = 1 / 3d; // a1
      P[3] = -1 / 5d; // a2
      P[4] = 1 / 7d; // a3
      P[5] = -1 / 11d; // b1
      P[6] = 1 / 13d; // b2
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(6.4381869725572211314, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(1, D[0, 0], 0, 1E-14); // wrt offset
      AssertEx.AreEqual(3.4381869725572211314, D[0, 1], 0, 1E-14); // wrt a0
      AssertEx.AreEqual(3.0762725543933031176, D[0, 2], 0, 1E-14); // wrt a1
      AssertEx.AreEqual(2.7524543907729554210, D[0, 3], 0, 1E-14); // wrt a2
      AssertEx.AreEqual(2.4627223496389601135, D[0, 4], 0, 1E-14); // wrt a3
      AssertEx.AreEqual(3.8426795575639530292, D[0, 5], 0, 1E-14); // wrt b1
      AssertEx.AreEqual(4.2947595055126533856, D[0, 6], 0, 1E-14);  // wrt b2


    }
  }
}
