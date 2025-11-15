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
  public class HendersonModelTest
  {
    [Fact]
    public void TestValueAndDerivatives()
    {
      var x = 2 / 3d;
      var offs = 1 / 7d;
      var M0 = 1 / 9d;
      var C = 11 / 5d;

      var X = new double[1];
      var Y = new double[1];

      var y = HendersonModel.Evaluate(x, offs, M0, C);
      var f = new HendersonModel();
      X[0] = x;
      var P = new double[] { offs, M0, C };
      f.Evaluate(X, P, Y);
      AssertEx.AreEqual(2.9762856266992279790, y, 0, 1e-14);
      AssertEx.AreEqual(2.9762856266992279790, Y[0], 0, 1e-14);

      var XX = Matrix<double>.Build.Dense(1, 1);
      var DF = Matrix<double>.Build.Dense(1, 3);
      XX[0, 0] = x;
      f.EvaluateDerivative(XX, P, null, DF, null);
      AssertEx.AreEqual(1, DF[0, 0], 0, 1e14);
      AssertEx.AreEqual(-11.591298342990348226, DF[0, 1], 0, 1e-14);
      AssertEx.AreEqual(-1.3413546480237511711, DF[0, 2], 0, 1e-14);
    }
  }
}
