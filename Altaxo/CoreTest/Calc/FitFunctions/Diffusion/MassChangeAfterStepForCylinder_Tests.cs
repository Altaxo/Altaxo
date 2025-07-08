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
  public class MassChangeAfterStepForCylinder_Tests
  {
    [Fact]
    public void TestFunctionValues()
    {
      var (t, t0, D) = (1 / (4 * 65536d), 0, 1d);
      var result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.0044039150211988953223, result, 0, 1E-14);

      (t, t0, D) = (1 / (2 * 65536d), 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.0062258397663107350484, result, 0, 1E-13);

      (t, t0, D) = (1 / 65536d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.0088001922152200719193, result, 0, 1E-14);

      (t, t0, D) = (1 / 32768d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.012436396862311966629, result, 0, 1E-13);

      (t, t0, D) = (1 / 16384d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.017569799184984897503, result, 0, 1E-14);

      (t, t0, D) = (1 / 8192d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.024811566689672110993, result, 0, 1E-14);

      (t, t0, D) = (1 / 4096d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.035016983380520583856, result, 0, 1E-14);

      (t, t0, D) = (1 / 2048d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.049377444237004650492, result, 0, 1E-15);

      (t, t0, D) = (1 / 1024d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.069541273366381529058, result, 0, 1E-15);

      (t, t0, D) = (1 / 256d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.13709320376577835706, result, 0, 1E-15);

      (t, t0, D) = (1 / 64d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.26606775355885811507, result, 0, 1E-15);

      (t, t0, D) = (1 / 16d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.49809670511657451353, result, 0, 1E-15);

      (t, t0, D) = (1 / 4d, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.83700908939669147507, result, 0, 1E-15);

      (t, t0, D) = (1, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.99787045372271757944, result, 0, 1E-15);

      (t, t0, D) = (4, 0, 1d);
      result = MassChangeAfterStepForCylinder.Evaluate(t, 1, t0, 0, 1, D);
      AssertEx.AreEqual(0.99999999993784589350, result, 0, 1E-15);

    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterStepForCylinder() { Radius = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      var (t, t0, D) = (2 + 1 / 65536d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.006562175853705880, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-214.75129551307622, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.006562175853705880, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.00065536894382652655, DY[0, 3], 0, 1E-12);

      (t, t0, D) = (2 + 1 / 4096d, 2d, 5d);
      parameters = new double[] { t0, 7, 11, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(7.287613671010323, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-585.96370027903840, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.02614669736457485, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.028611508802687422, DY[0, 3], 0, 1E-12);

      (t, t0, D) = (2 + 1000 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 7, 11, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(16.41484914602818, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-5.0979940786070522, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.8558953769116524, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.49785098423896994, DY[0, 3], 0, 1E-12);
    }

  }
}
