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
  public class MassChangeAfterExponentialEquilibrationForSphere_Tests
  {
    [Fact]
    public void TestFunctionReducedVariables()
    {
      var (rv, rz) = (1 / 256d, 1 / 7d);
      var result = MassChangeAfterExponentialEquilibrationForSphere.EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
      AssertEx.AreEqual(0.00365614203233400982, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(1.37337341258652218, result.derivativeWrtRv, 0, 1E-8);
      AssertEx.AreEqual(-0.0253123662859434496, result.derivativeWrtRz, 0, 1E-8);

      (rv, rz) = (1 / 7d, 1 / 37d);
      result = MassChangeAfterExponentialEquilibrationForSphere.EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
      AssertEx.AreEqual(0.792933848978149510, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(2.14955728039248354, result.derivativeWrtRv, 0, 1E-8);
      AssertEx.AreEqual(-3.11342154872842377, result.derivativeWrtRz, 0, 1E-8);
    }


    [Fact]
    public void TestFunctionValues()
    {
      var result = MassChangeAfterExponentialEquilibrationForSphere.EvaluateUnitStep(1 / 512d, 1, 5, 1 / 8d);
      AssertEx.AreEqual(0.00323524106079161104893999, result, 0, 1E-12);
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterExponentialEquilibrationForSphere { Radius = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 5);

      var (t, t0, D, tau) = (2 + 1 / 512d, 2d, 45d, 1 / 8d);
      parameters = new double[] { t0, 0, 1, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.00323524106079161104893999, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-2.41592924574282378341693, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.00323524106079161104893999, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0000334171852850094424815104, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(-0.0257187077621282223225458, DY[0, 4], 0, 1E-12);

      (t, t0, D, tau) = (2 + 1 / 32d, 2d, 45d, 1 / 8d);
      parameters = new double[] { t0, 7, 11, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(8.63051095433848522423421, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-63.4833780175998870444267, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.148228268576225929475837, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0115310217919822990164020, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(-11.7196766592863441152019, DY[0, 4], 0, 1E-12);


    }



  }
}
