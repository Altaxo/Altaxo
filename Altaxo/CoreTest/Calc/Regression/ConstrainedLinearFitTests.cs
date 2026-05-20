#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Calc.Optimization;
using Xunit;

namespace Altaxo.Calc.Regression
{
  public class ConstrainedLinearFitTests
  {
    private static Matrix<double> M(double[,] values)
      => Matrix<double>.Build.DenseOfArray(values);

    private static Vector<double> V(params double[] values)
      => Vector<double>.Build.DenseOfArray(values);

    [Fact]
    public void Fit_UsesLinearConstraintsProjector()
    {
      var design = M(new double[,]
      {
        { 1, 0 },
        { 0, 1 },
        { 1, 1 },
        { 2, 1 },
      });
      var observations = V(0, 0, 1, 1);
      IConstraintsProjector projector = new LinearConstraintsProjector(
        A: M(new double[,] { { 1, 1 } }),
        b: V(1));

      var beta = ConstrainedLinearFit.Fit(design, observations, projector);

      AssertEx.AreEqual(0.5, beta[0], 1e-10, 0, "Expected first parameter to satisfy the constrained optimum.");
      AssertEx.AreEqual(0.5, beta[1], 1e-10, 0, "Expected second parameter to satisfy the constrained optimum.");
    }

    [Fact]
    public void Fit_UsesBoxConstraintsProjector()
    {
      foreach (var scale in new[] { 1, 1E40, 1E-40 })
      {
        var design = M(new double[,]
        {
        { 1, 0 },
        { 0, 1 },
        });
        var observations = V(2 * scale, -5 * scale);
        IConstraintsProjector projector = new BoxConstraintsProjector(
          fixedValues: new double?[] { null, null },
          lowerBounds: new double?[] { 0, 0 },
          upperBounds: null);

        var beta = ConstrainedLinearFit.Fit(design, observations, projector);

        AssertEx.AreEqual(2 * scale, beta[0], 1e-10, 0, "Expected unconstrained optimum for the first parameter.");
        AssertEx.AreEqual(0, beta[1], 1e-10, 0, "Expected lower bound to clamp the second parameter.");
      }
    }
  }
}
