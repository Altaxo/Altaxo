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
    public void TestFunctionReducedVariables()
    {
      var (rv, rz) = (1 / 256d, 1 / 7d);
      var fn = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStepWrtReducedVariables(rv, rz);
      var result = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.00127163631065417166, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(0.484764431429707549, result.derivativeWrtRv, 0, 1E-8);
      AssertEx.AreEqual(-0.00880455033461646500, result.derivativeWrtRz, 0, 1E-8);

      (rv, rz) = (1 / 7d, 1 / 37d);
      fn = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStepWrtReducedVariables(rv, rz);
      result = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
      AssertEx.AreEqual(fn, result.functionValue, 0, 1E-12);
      AssertEx.AreEqual(0.380245010672382679, result.functionValue, 0, 1E-8);
      AssertEx.AreEqual(1.70923931140624026, result.derivativeWrtRv, 0, 1E-8);
      AssertEx.AreEqual(-2.00515608854099045, result.derivativeWrtRz, 0, 1E-8);
    }


    [Fact]
    public void TestFunctionValues()
    {
      var result = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStep(1 / 4096d, 3, 5, 1 / 7d);
      AssertEx.AreEqual(0.0000149619716761694856, result, 0, 1E-8);

      result = MassChangeAfterExponentialEquilibrationForPlaneSheet.EvaluateUnitStep(17, 3, 5, 13);
      AssertEx.AreEqual(0.716342200214270606, result, 0, 1E-8);

    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterExponentialEquilibrationForPlaneSheet { HalfThickness = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 5);

      var (t, t0, M0, DM, D, tau) = (1 / 4096d + 2, 2, 0, 1, 5, 1 / 7d);
      parameters = new double[] { t0, M0, DM, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.0000149619716761694856, FV[0], 0, 1E-8);
      AssertEx.AreEqual(-0.091884472730251815, DY[0, 0], 0, 1E-8);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-8);
      AssertEx.AreEqual(0.000014961971676169486, DY[0, 2], 0, 1E-8);
      AssertEx.AreEqual(1.4961971676169486E-6, DY[0, 3], 0, 1E-8);
      AssertEx.AreEqual(-0.00010466222733452074, DY[0, 4], 0, 1E-8);

      (t, t0, M0, DM, D, tau) = (1 / 512d + 2, 2, 7, 11, 5, 1 / 13d);
      parameters = new double[] { t0, M0, DM, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(7.00685105466077106, FV[0], 0, 1E-8);
      AssertEx.AreEqual(-5.22613938544844022, DY[0, 0], 0, 1E-8);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.000622823150979186924, DY[0, 2], 0, 1E-8);
      AssertEx.AreEqual(0.000685105466077105617, DY[0, 3], 0, 1E-8);
      AssertEx.AreEqual(-0.0881630900386399373, DY[0, 4], 0, 1E-8);


      (t, t0, M0, DM, D, tau) = (17 + 2, 2, 7, 11, 5, 13d);
      parameters = new double[] { t0, M0, DM, D, tau }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(14.8797642023569767, FV[0], 0, 1E-8);
      AssertEx.AreEqual(-0.240018138228259988, DY[0, 0], 0, 1E-8);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.716342200214270606, DY[0, 2], 0, 1E-8);
      AssertEx.AreEqual(0.0307784565163145374, DY[0, 3], 0, 1E-8);
      AssertEx.AreEqual(-0.302032005176834394, DY[0, 4], 0, 1E-8);

    }



  }
}
