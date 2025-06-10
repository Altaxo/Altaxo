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
  public class MassChangeAfterStepForPlaneSheet_Tests
  {
    [Fact]
    public void TestFunctionValues()
    {
      var ff = new MassChangeAfterStepForPlaneSheet() { Thickness = 3 };
      var (t, t0, D) = (2 + 1 / 2048d, 2d, 5d);
      var result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.Thickness, t0, 0, 1, D);
      AssertEx.AreEqual(0.03716925241984940, result, 0, 1E-15);

      (t, t0, D) = (2 + 16 / 2048d, 2d, 5d);
      result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.Thickness, t0, 0, 1, D);
      AssertEx.AreEqual(0.14867700967939759, result, 0, 1E-15);

      (t, t0, D) = (2 + 60 / 2048d, 2d, 5d);
      result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.Thickness, t0, 0, 1, D);
      AssertEx.AreEqual(0.28791178756410480, result, 0, 1E-15);

      (t, t0, D) = (2 + 1000 / 2048d, 2d, 5d);
      result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.Thickness, t0, 0, 1, D);
      AssertEx.AreEqual(0.94427463061711150, result, 0, 1E-15);
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterStepForPlaneSheet() { Thickness = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      var (t, t0, D) = (2 + 1 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.03716925241984940, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-38.061314477925784, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.03716925241984940, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0037169252419849398, DY[0, 3], 0, 1E-12);

      (t, t0, D) = (2 + 60 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.28791178756410480, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-4.9136924728896714, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.28791178756410480, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.028791166833337918, DY[0, 3], 0, 1E-12);

      (t, t0, D) = (2 + 1000 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.94427463061711150, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-0.30554852842090953, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.94427463061711150, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.029838723478604446, DY[0, 3], 0, 1E-12);
    }

  }
}
