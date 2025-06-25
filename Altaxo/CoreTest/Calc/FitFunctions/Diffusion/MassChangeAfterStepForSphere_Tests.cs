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
  public class MassChangeAfterStepForSphere_Tests
  {
    [Fact]
    public void TestFunctionValues()
    {
      var ff = new MassChangeAfterStepForSphere() { Radius = 3 };
      var (t, t0, D) = (2 + 1 / 2048d, 2d, 5d);
      var result = MassChangeAfterStepForSphere.Evaluate(t, ff.Radius, t0, 0, 1, D);
      AssertEx.AreEqual(0.05494007654644076, result, 0, 1E-15);

      (t, t0, D) = (2 + 16 / 2048d, 2d, 5d);
      result = MassChangeAfterStepForSphere.Evaluate(t, ff.Radius, t0, 0, 1, D);
      AssertEx.AreEqual(0.20999468118576306, result, 0, 1E-15);

      (t, t0, D) = (2 + 60 / 2048d, 2d, 5d);
      result = MassChangeAfterStepForSphere.Evaluate(t, ff.Radius, t0, 0, 1, D);
      AssertEx.AreEqual(0.38303956183916935, result, 0, 1E-15);

      (t, t0, D) = (2 + 1000 / 2048d, 2d, 5d);
      result = MassChangeAfterStepForSphere.Evaluate(t, ff.Radius, t0, 0, 1, D);
      AssertEx.AreEqual(0.95820257794256885, result, 0, 1E-15);
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new MassChangeAfterStepForSphere() { Radius = 3 };

      var parameters = new double[4];
      var X = Matrix<double>.Build.Dense(1, 1);
      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      var (t, t0, D) = (2 + 1 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.05494007654644076, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-55.425305050222009, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.05494007654644076, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.0054126274463107431, DY[0, 3], 0, 1E-12);

      (t, t0, D) = (2 + 60 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.38303956183916935, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-5.7038751887218236, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.38303956183916935, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.033421143683916935, DY[0, 3], 0, 1E-12);

      (t, t0, D) = (2 + 1000 / 2048d, 2d, 5d);
      parameters = new double[] { t0, 0, 1, D }; // t0, M0, ΔM, D
      X[0, 0] = t; // t
      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(0.95820257794256885, FV[0], 0, 1E-12);
      AssertEx.AreEqual(-0.22923585744223094, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(1, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(0.95820257794256885, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(0.022386314203342865, DY[0, 3], 0, 1E-12);
    }

  }
}
