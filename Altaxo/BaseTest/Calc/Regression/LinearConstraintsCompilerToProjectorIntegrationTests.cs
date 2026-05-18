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

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Xunit;

namespace Altaxo.Calc.Regression
{
  // =========================================================================
  // Helpers
  // =========================================================================

  /// <summary>
  /// Provides helper methods for vector and matrix assertions used by regression tests.
  /// </summary>
  internal static class TestHelpers
  {
    /// <summary>
    /// Defines the default numeric tolerance used in helper assertions.
    /// </summary>
    internal const double Tol = 1e-7;

    /// <summary>
    /// Creates a dense vector from the supplied values.
    /// </summary>
    /// <param name="values">The vector values.</param>
    /// <returns>A dense vector containing the supplied values.</returns>
    internal static Vector<double> V(params double[] values)
        => Vector<double>.Build.DenseOfArray(values);

    /// <summary>
    /// Creates a dense matrix from the supplied two-dimensional array.
    /// </summary>
    /// <param name="values">The matrix values.</param>
    /// <returns>A dense matrix containing the supplied values.</returns>
    internal static Matrix<double> M(double[,] values)
        => Matrix<double>.Build.DenseOfArray(values);

    /// <summary>
    /// Asserts that two vectors are equal within the specified tolerance.
    /// </summary>
    /// <param name="actual">The actual vector.</param>
    /// <param name="expected">The expected vector.</param>
    /// <param name="tol">The allowed numeric tolerance.</param>
    internal static void AssertVector(
        Vector<double> actual, Vector<double> expected, double tol = Tol)
    {
      Assert.Equal(expected.Count, actual.Count);
      for (int i = 0; i < expected.Count; i++)
        Assert.Equal(expected[i], actual[i], precision: (int)-Math.Log10(tol));
    }
  }

  // =========================================================================
  // Integration tests: LinearConstraintsCompiler → LinearConstraintsProjector
  // =========================================================================

  /// <summary>
  /// Contains integration tests that verify compiled constraints can be consumed by <see cref="LinearConstraintProjector"/>.
  /// </summary>
  public class LinearConstraintsCompilerToProjectorIntegrationTests
  {
    /// <summary>
    /// Verifies that an equality constraint is compiled and enforced by the projector.
    /// </summary>
    [Fact]
    public void EqualityConstraint_CompiledAndProjected()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[] { "a + b == 1" });
      var proj = result.ToProjectorMatrices().ToProjector();

      var projected = proj.Project(TestHelpers.V(0.3, 0.3));
      Assert.Equal(1.0, projected[0] + projected[1], precision: 6);
    }

    /// <summary>
    /// Verifies that simple box bounds compiled as inequalities are enforced by the projector.
    /// </summary>
    [Fact]
    public void BoxBoundsViaInequalities_CompiledAndProjected()
    {
      // 0 <= a <= 1, 0 <= b <= 1
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[]
      {
                "a <= 1",
                "b <= 1",
                "a >= 0",
                "b >= 0",
            });
      var proj = result.ToProjectorMatrices().ToProjector();

      // Point outside box
      var projected = proj.Project(TestHelpers.V(2.0, -0.5));

      Assert.True(projected[0] <= 1.0 + 1e-7);
      Assert.True(projected[0] >= 0.0 - 1e-7);
      Assert.True(projected[1] <= 1.0 + 1e-7);
      Assert.True(projected[1] >= 0.0 - 1e-7);
    }

    /// <summary>
    /// Verifies that named constants in inequalities are respected during projection.
    /// </summary>
    [Fact]
    public void NamedConstantBound_CompiledAndProjected()
    {
      var compiler = new LinearConstraintsCompiler(
          new[] { "a", "b" },
          new Dictionary<string, double> { ["upper"] = 3.0, ["lower"] = 0.5 });

      var result = compiler.Compile(new[]
      {
                "a <= upper",
                "a >= lower",
            });
      var proj = result.ToProjectorMatrices().ToProjector();
      var projected = proj.Project(TestHelpers.V(5.0, 1.0));

      Assert.Equal(3.0, projected[0], precision: 6); // clamped to upper=3
    }

    /// <summary>
    /// Verifies that equality and inequality constraints can be enforced together.
    /// </summary>
    [Fact]
    public void EqualityAndInequality_Combined()
    {
      // a + b == 1  and  a <= 0.3
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[]
      {
                "a + b == 1",
                "a <= 0.3",
            });
      var proj = result.ToProjectorMatrices().ToProjector();
      var projected = proj.Project(TestHelpers.V(0.8, 0.2));

      Assert.Equal(1.0, projected[0] + projected[1], precision: 6);
      Assert.True(projected[0] <= 0.3 + 1e-6);
    }

    /// <summary>
    /// Verifies that projected points satisfy all compiled constraints.
    /// </summary>
    [Fact]
    public void ProjectedPoint_IsFeasible()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b", "c" });
      var result = compiler.Compile(new[]
      {
                "a + b == 1",
                "c <= 2",
                "c >= 0",
                "a >= 0",
                "b >= 0",
            });
      var matrices = result.ToProjectorMatrices();
      var proj = matrices.ToProjector();

      // Several infeasible starting points
      var testPoints = new[]
      {
                TestHelpers.V(0.3, 0.3, 3.0),
                TestHelpers.V(-0.5, 1.5, 1.0),
                TestHelpers.V(0.9, 0.9, -1.0),
            };

      foreach (var p in testPoints)
      {
        var projected = proj.Project(p);
        Assert.True(proj.IsFeasible(projected),
            $"Projected point ({projected}) is not feasible.");
      }
    }

    /// <summary>
    /// Verifies that diagnostics from invalid expressions do not prevent valid constraints from reaching the projector.
    /// </summary>
    [Fact]
    public void CompilerDiagnostic_DoesNotPropagateToProjector()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[]
      {
                "a + b == 1",   // valid
                "a * b <= 5",   // invalid — should be skipped
            });

      Assert.False(result.IsSuccess);
      Assert.Single(result.Diagnostics);

      // Only the valid constraint should be in the projector
      var proj = result.ToProjectorMatrices().ToProjector();
      var projected = proj.Project(TestHelpers.V(0.3, 0.3));

      Assert.Equal(1.0, projected[0] + projected[1], precision: 6);
    }

    /// <summary>
    /// Verifies that constrained and free parameters are identified correctly after projection.
    /// </summary>
    [Fact]
    public void ConstrainedAndFreeParameters_CorrectlyIdentified()
    {
      // a + b == 1: both a and b are constrained. c is free.
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b", "c" });
      var result = compiler.Compile(new[] { "a + b == 1" });
      var proj = result.ToProjectorMatrices().ToProjector();

      var projResult = proj.ProjectWithInfo(TestHelpers.V(0.2, 0.2, 7.0));

      Assert.True(projResult.IsConstrained[0]);   // a
      Assert.True(projResult.IsConstrained[1]);   // b
      Assert.False(projResult.IsConstrained[2]);  // c
      Assert.Equal(new[] { 2 }, projResult.FreeParameterIndices);
    }

    /// <summary>
    /// Verifies that a slack inequality does not mark parameters as constrained.
    /// </summary>
    [Fact]
    public void InactiveInequalityConstraint_ParameterReportedFree()
    {
      // a <= 10, but a=3 — constraint is slack, a should be free
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[] { "a <= 10" });
      var proj = result.ToProjectorMatrices().ToProjector();

      var projResult = proj.ProjectWithInfo(TestHelpers.V(3.0, 5.0));

      Assert.False(projResult.IsConstrained[0]);
      Assert.False(projResult.IsConstrained[1]);
    }

    #region Tests with ParameterSet-defined bounds


    /// <summary>
    /// A lower boundary constraint together with an inequality constraint.
    /// </summary>
    [Fact]
    public void OneBoundaryConstraint_NoOtherConstraints_2Parameters()
    {
      var parameters = new ParameterSet([
        new ParameterSetElement("a", 0, 0, true),                       // a has no bounds  
        new ParameterSetElement("b", 1, 0, true, 1, false, null, false) // b has a lower bound at 1
        ]
        );

      var compiler = new LinearConstraintsCompiler(parameters);
      var result = compiler.Compile(Array.Empty<string>());

      var mat = result.ToProjectorMatrices();
      var proj = mat.ToProjector();
      var projResult = proj.ProjectWithInfo(TestHelpers.V(0, 1));

      Assert.Equal(0, projResult.Parameters[0]); // a is unconstrained, should be unchanged
      Assert.False(projResult.IsConstrained[0]);

      Assert.Equal(1, projResult.Parameters[1]); // b is constrained by its lower bound, should be unchanged
      Assert.True(projResult.IsConstrained[1]);
    }

    /// <summary>
    /// A lower boundary constraint together with an inequality constraint.
    /// </summary>
    [Fact]
    public void OneBoundaryConstraint_OneInequalityConstraint_2Parameters()
    {
      var parameters = new ParameterSet([
        new ParameterSetElement("a", 0, 0, true),                       // a has no bounds  
        new ParameterSetElement("b", 1, 0, true, 1, false, null, false) // b has a lower bound at 1
        ]
        );

      var compiler = new LinearConstraintsCompiler(parameters);
      var result = compiler.Compile(new[] { "a >= b" });

      var mat = result.ToProjectorMatrices();
      var proj = mat.ToProjector();
      var projResult = proj.ProjectWithInfo(TestHelpers.V(0, 1));

      Assert.Equal(1, projResult.Parameters[0], 1E-10); // a is constrained by a >= b, but since b=1, a should be projected to 1 as well
      Assert.True(projResult.IsConstrained[0]);
      Assert.Equal(1, projResult.Parameters[1], 1E-10); // b is constrained by its lower bound, should be unchanged
      Assert.True(projResult.IsConstrained[1]);
    }

    #endregion
  }
}
