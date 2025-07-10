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
  public class MassChangeAfterExponentialEquilibrationForCylinder_Tests
  {
    [Fact]
    public void TestFunctionReducedVariables()
    {
      var (rv, rz) = (1 / 256d, 1 / 7d);
      var result = MassChangeAfterExponentialEquilibrationForCylinder.EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
      AssertEx.AreEqual(0.00248983384186102958, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(0.942223589467421292, result.derivativeWrtRv, 0, 1E-8);
      AssertEx.AreEqual(-0.0172384184000902459, result.derivativeWrtRz, 0, 1E-8);

      (rv, rz) = (1 / 7d, 1 / 37d);
      result = MassChangeAfterExponentialEquilibrationForCylinder.EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
      AssertEx.AreEqual(0.634085279161180218, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(2.27434857014339953, result.derivativeWrtRv, 0, 1E-8);
      AssertEx.AreEqual(-2.92139659660524895, result.derivativeWrtRz, 0, 1E-8);
    }

    [Fact]
    public void TestFunctionValues()
    {
      var result = MassChangeAfterExponentialEquilibrationForCylinder.EvaluateUnitStep(1 / 4096d, 3, 5, 1 / 7d);
      AssertEx.AreEqual(0.000029807907130110902, result, 0, 1E-10);

      result = MassChangeAfterExponentialEquilibrationForCylinder.Evaluate(2 + 1 / 512d, 3, 2, 7, 11, 5, 1 / 13d);
      AssertEx.AreEqual(7.0135510959042729, result, 0, 1E-10);
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterExponentialEquilibrationForCylinder { Radius = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 5);

      var (t, t0, M0, DM, D, tau) = (2 + 1 / 4096d, 2d, 0, 1, 5, 1 / 7d);
      parameters = new double[] { t0, M0, DM, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.000029807907130110902, FV[0], 0, 1E-8);
      AssertEx.AreEqual(-0.18281822620211314, DY[0, 0], 0, 1E-8);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.000029807907130110902, DY[0, 2], 0, 1E-8);
      AssertEx.AreEqual(2.9691665277234739E-6, DY[0, 3], 0, 1E-8);
      AssertEx.AreEqual(-0.00020851266357430537, DY[0, 4], 0, 1E-8);

      (t, t0, M0, DM, D, tau) = (2 + 1 / 512d, 2d, 7, 11, 5, 1 / 13d);
      parameters = new double[] { t0, M0, DM, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(7.0135510959042729, FV[0], 0, 1E-8);
      AssertEx.AreEqual(-10.298094064010023, DY[0, 0], 0, 1E-8);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.0012319178094793509, DY[0, 2], 0, 1E-8);
      AssertEx.AreEqual(0.0013399308579523415, DY[0, 3], 0, 1E-8);
      AssertEx.AreEqual(-0.17437953882710230, DY[0, 4], 0, 1E-8);

      (t, t0, M0, DM, D, tau) = (2 + 17, 2, 7, 11, 5, 13);
      parameters = new double[] { t0, M0, DM, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(14.972416762220183, FV[0], 0, 1E-8);
      AssertEx.AreEqual(-0.23289101829075513, DY[0, 0], 0, 1E-8);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.72476516020183484, DY[0, 2], 0, 1E-8);
      AssertEx.AreEqual(0.010790453959934395, DY[0, 3], 0, 1E-8);
      AssertEx.AreEqual(-0.30039961854947426, DY[0, 4], 0, 1E-8);
    }
  }
}
