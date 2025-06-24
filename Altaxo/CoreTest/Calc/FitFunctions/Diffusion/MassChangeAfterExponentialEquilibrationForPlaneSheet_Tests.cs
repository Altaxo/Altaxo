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

namespace Altaxo.Calc.FitFunctions.Diffusion
{
  public class MassChangeAfterExponentialEquilibrationForPlaneSheet_Tests
  {
    [Fact]
    public void TestFunctionValues()
    {
      var result = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStep(1 / 2048d, 1, 1, 1 / 2d);
      AssertEx.AreEqual(0.0000324533, result, 0, 1E-5);
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterExponentialEquilibrationForPlaneSheet { Thickness = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 5);

      var (t, t0, D, tau) = (2 + 1 / 512d, 2d, 45d, 1 / 8d);
      parameters = new double[] { t0, 0, 1, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.00230862363062154760100015, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-1.76565512710729598783703, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.00230862363062154760100015, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0000256513736735520724597153, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(-0.0183538668385727537244560, DY[0, 4], 0, 1E-12);

      (t, t0, D, tau) = (2 + 1 / 512d, 2d, 45d, 1 / 8d);
      parameters = new double[] { t0, 7, 11, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(7.02539485993683702361100, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-19.4222063981802558662073, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.00230862363062154760100015, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.000282165110409072797056869, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(-0.201892535224300290969016, DY[0, 4], 0, 1E-12);


    }



  }
}
