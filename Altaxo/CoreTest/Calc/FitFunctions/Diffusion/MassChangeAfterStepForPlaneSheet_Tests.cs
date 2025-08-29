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
    public void TestFunctionReducedVariables()
    {
      var rv = (1 / 2048d);
      var fn = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepWrtReducedVariable(rv);
      var result = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariable(rv);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.0249338925250895424, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(25.5323059456916914, result.derivativeWrtRv, 0, 1E-8);

      rv = (20 / 2048d);
      fn = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepWrtReducedVariable(rv);
      result = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariable(rv);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.111507757259548195, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(5.70919717168886757, result.derivativeWrtRv, 0, 1E-8);

      rv = (60 / 2048d);
      fn = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepWrtReducedVariable(rv);
      result = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariable(rv);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.193137101011594782, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(3.29620652393120786, result.derivativeWrtRv, 0, 1E-8);

      rv = (1000 / 2048d);
      fn = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepWrtReducedVariable(rv);
      result = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariable(rv);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.757024990086331502, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(0.599551543732442789, result.derivativeWrtRv, 0, 1E-8);

      rv = (3);
      fn = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepWrtReducedVariable(rv);
      result = MassChangeAfterStepForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariable(rv);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.999505627625813251, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(0.00121981494001263261, result.derivativeWrtRv, 0, 1E-8);

    }

    [Fact]
    public void TestFunctionValues()
    {
      var ff = new MassChangeAfterStepForPlaneSheet() { HalfThickness = 3 };
      var (t, t0, M0, DM, D) = (50 / 2048d + 2, 2d, 0d, 1d, 5d);
      var result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.HalfThickness, t0, M0, DM, D);
      AssertEx.AreEqual(0.131413152188550003, result, 0, 1E-15);

      (t, t0, M0, DM, D) = (50 / 2048d + 2, 2d, 7, 11, 5d);
      result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.HalfThickness, t0, M0, DM, D);
      AssertEx.AreEqual(8.44554467407405003, result, 0, 1E-15);

      (t, t0, M0, DM, D) = (200 / 2048d + 2, 2d, 7, 11, 5d);
      result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.HalfThickness, t0, M0, DM, D);
      AssertEx.AreEqual(9.89108934670896301, result, 0, 1E-15);

      (t, t0, M0, DM, D) = (7500 / 2048d + 2, 2d, 7, 11, 5d);
      result = MassChangeAfterStepForPlaneSheet.Evaluate(t, ff.HalfThickness, t0, M0, DM, D);
      AssertEx.AreEqual(17.9411087847773778, result, 0, 1E-15);
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterStepForPlaneSheet() { HalfThickness = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      var (t, t0, M0, DM, D) = (50 / 2048d + 2, 2d, 0d, 1d, 5d);
      parameters = new double[] { t0, M0, DM, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.131413152188550003, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-2.69134135682150405, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.131413152188550003, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0131413152188550003, DY[0, 3], 0, 1E-12);

      (t, t0, M0, DM, D) = (50 / 2048d + 2, 2d, 7, 11, 5d);
      parameters = new double[] { t0, M0, DM, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(8.44554467407405003, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-29.6047549250365446, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.131413152188550003, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.144554467407405003, DY[0, 3], 0, 1E-12);

      (t, t0, M0, DM, D) = (200 / 2048d + 2, 2d, 7, 11, 5d);
      parameters = new double[] { t0, M0, DM, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(9.89108934670896301, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-14.8023771698028668, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.262826304246269365, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.289108929097712242, DY[0, 3], 0, 1E-12);

      (t, t0, M0, DM, D) = (7500 / 2048d + 2, 2d, 7, 11, 5d);
      parameters = new double[] { t0, M0, DM, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(17.9411087847773778, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-0.0807268051314850620, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.994646253161579802, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0591260779771619106, DY[0, 3], 0, 1E-12);
    }

  }
}
