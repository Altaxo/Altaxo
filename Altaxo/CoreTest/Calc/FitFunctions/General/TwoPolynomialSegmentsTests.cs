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

namespace Altaxo.Calc.FitFunctions.General
{
  public class TwoPolynomialSegmentsTests
  {
    [Fact]
    public void TestFunctionValues()
    {
      double x;
      var X = new double[1];
      var XX = Matrix<double>.Build.Dense(1, 1);
      var Y = new double[1];

      // ParameterSet 1a: 2nd order left, 3rd order right
      var P = new double[2 + 2 + 3];
      var D = Matrix<double>.Build.Dense(1, 2 + 2 + 3);
      x = 1;
      P[0] = 3; // xc
      P[1] = 7; // y0
      P[2] = 1 / 11d; // a1
      P[3] = -1 / 13d; // a2
      P[4] = -1 / 17d; // b1
      P[5] = 1 / 19d; // b2
      P[6] = -1 / 23d; // b3
      var f = new TwoPolynomialSegments(2, 3);
      var y = TwoPolynomialSegments.Evaluate(x, P[0], P[1], P.AsSpan(2, 2), P.AsSpan(2 + 2, 3));
      AssertEx.AreEqual(6.5104895104895104895, y, 0, 1E-14);
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(6.5104895104895104895, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(-0.39860139860139860140, D[0, 0], 0, 1E-14); // wrt xc
      AssertEx.AreEqual(1, D[0, 1], 0, 1E-14); // wrt y0
      AssertEx.AreEqual(-2, D[0, 2], 0, 1E-14); // wrt a1
      AssertEx.AreEqual(4, D[0, 3], 0, 1E-14); // wrt a2
      AssertEx.AreEqual(0, D[0, 4], 0, 1E-14); // wrt b1
      AssertEx.AreEqual(0, D[0, 5], 0, 1E-14); // wrt b2
      AssertEx.AreEqual(0, D[0, 6], 0, 1E-14);  // wrt b3

      // ParameterSet 1b
      x = 5;
      y = TwoPolynomialSegments.Evaluate(x, P[0], P[1], P.AsSpan(2, 2), P.AsSpan(2 + 2, 3));
      AssertEx.AreEqual(6.7450531700094225333, y, 0, 1E-14);
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(6.7450531700094225333, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(0.37003634405707363037, D[0, 0], 0, 1E-14); // wrt xc
      AssertEx.AreEqual(1, D[0, 1], 0, 1E-14); // wrt y0
      AssertEx.AreEqual(0, D[0, 2], 0, 1E-14); // wrt a1
      AssertEx.AreEqual(0, D[0, 3], 0, 1E-14); // wrt a2
      AssertEx.AreEqual(2, D[0, 4], 0, 1E-14); // wrt b1
      AssertEx.AreEqual(4, D[0, 5], 0, 1E-14); // wrt b2
      AssertEx.AreEqual(8, D[0, 6], 0, 1E-14);  // wrt b3

      // ParameterSet 2a: zero order left, 3rd order right
      P = new double[2 + 0 + 3];
      D = Matrix<double>.Build.Dense(1, 2 + 0 + 3);
      x = 1;
      P[0] = 3; // xc
      P[1] = 7; // y0
      P[2] = -1 / 17d; // b1
      P[3] = 1 / 19d; // b2
      P[4] = -1 / 23d; // b3
      f = new TwoPolynomialSegments(0, 3);
      y = TwoPolynomialSegments.Evaluate(x, P[0], P[1], Span<double>.Empty, P.AsSpan(2 + 0, 3));
      AssertEx.AreEqual(7, y, 0, 1E-14);
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(7, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(0, D[0, 0], 0, 1E-14); // wrt xc
      AssertEx.AreEqual(1, D[0, 1], 0, 1E-14); // wrt y0
      AssertEx.AreEqual(0, D[0, 2], 0, 1E-14); // wrt b1
      AssertEx.AreEqual(0, D[0, 3], 0, 1E-14); // wrt b2
      AssertEx.AreEqual(0, D[0, 4], 0, 1E-14);  // wrt b3

      // ParameterSet 1b
      x = 5;
      y = TwoPolynomialSegments.Evaluate(x, P[0], P[1], Span<double>.Empty, P.AsSpan(2 + 0, 3));
      AssertEx.AreEqual(6.7450531700094225333, y, 0, 1E-14);
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(6.7450531700094225333, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(0.37003634405707363037, D[0, 0], 0, 1E-14); // wrt xc
      AssertEx.AreEqual(1, D[0, 1], 0, 1E-14); // wrt y0
      AssertEx.AreEqual(2, D[0, 2], 0, 1E-14); // wrt b1
      AssertEx.AreEqual(4, D[0, 3], 0, 1E-14); // wrt b2
      AssertEx.AreEqual(8, D[0, 4], 0, 1E-14);  // wrt b3

      // ParameterSet 3a: 3rd order left, zero order right
      P = new double[2 + 3 + 0];
      D = Matrix<double>.Build.Dense(1, 2 + 3 + 0);
      x = 1;
      P[0] = 3; // xc
      P[1] = 7; // y0
      P[2] = 1 / 11d; // a1
      P[3] = -1 / 13d; // a2
      P[4] = 1 / 17d; // a3
      f = new TwoPolynomialSegments(3, 0);
      y = TwoPolynomialSegments.Evaluate(x, P[0], P[1], P.AsSpan(2, 3), Span<double>.Empty);
      AssertEx.AreEqual(6.0399012751953928425, y, 0, 1E-14);
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(6.0399012751953928425, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(-1.1044837515425750720, D[0, 0], 0, 1E-14); // wrt xc
      AssertEx.AreEqual(1, D[0, 1], 0, 1E-14); // wrt y0
      AssertEx.AreEqual(-2, D[0, 2], 0, 1E-14); // wrt a1
      AssertEx.AreEqual(4, D[0, 3], 0, 1E-14); // wrt a2
      AssertEx.AreEqual(-8, D[0, 4], 0, 1E-14); // wrt a3

      // ParameterSet 3b
      x = 5;
      y = TwoPolynomialSegments.Evaluate(x, P[0], P[1], P.AsSpan(2, 3), Span<double>.Empty);
      AssertEx.AreEqual(7, y, 0, 1E-14);
      X[0] = x;
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(7, Y[0], 0, 1E-14);

      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, D, null);
      AssertEx.AreEqual(0, D[0, 0], 0, 1E-14); // wrt xc
      AssertEx.AreEqual(1, D[0, 1], 0, 1E-14); // wrt y0
      AssertEx.AreEqual(0, D[0, 2], 0, 1E-14); // wrt a1
      AssertEx.AreEqual(0, D[0, 3], 0, 1E-14); // wrt a2
      AssertEx.AreEqual(0, D[0, 4], 0, 1E-14); // wrt a3

    }
  }
}
