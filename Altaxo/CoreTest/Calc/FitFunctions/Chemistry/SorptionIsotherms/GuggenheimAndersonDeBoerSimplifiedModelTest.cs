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

namespace Altaxo.Calc.FitFunctions.Chemistry.SorptionIsotherms
{
  public class GuggenheimAndersonDeBoerSimplifiedModelTest
  {
    [Fact]
    public void TestValueAndDerivatives()
    {
      var x = 1 / 2d;
      var m0 = 1 / 3d;
      var a0 = 225 / 100d;
      var a1 = 146 / 100d;
      var a2 = 112 / 100d;

      var X = new double[1];
      var Y = new double[1];

      var y = GuggenheimAndersonDeBoerSimplifiedModel.Evaluate(x, m0, a0, a1, a2);
      var f = new GuggenheimAndersonDeBoerSimplifiedModel();
      X[0] = x;
      var P = new double[] { m0, a0, a1, a2 };
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(913 / 864d, y, 0, 1e-14);
      AssertEx.AreEqual(913 / 864d, Y[0], 0, 1e-14);

      var XX = Matrix<double>.Build.Dense(1, 1);
      var DF = Matrix<double>.Build.Dense(1, 4);
      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, DF, null);
      AssertEx.AreEqual(1, DF[0, 0], 0, 1e-14);
      AssertEx.AreEqual(625 / 1944d, DF[0, 1], 0, 1e-14);
      AssertEx.AreEqual(-15625 / 20736d, DF[0, 2], 0, 1e-14);
      AssertEx.AreEqual(-15625 / 34992d, DF[0, 3], 0, 1e-14);
    }
  }
}
