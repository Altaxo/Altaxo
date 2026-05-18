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
  // LinearConstraintsCompiler tests
  // =========================================================================

  /// <summary>
  /// Contains unit tests for <see cref="ConstraintCompiler"/>.
  /// </summary>
  public class LinearConstraintsCompilerTests
  {
    /// <summary>
    /// Represents a minimal fit function used to test the fit-function-based constraint compiler constructor.
    /// </summary>
    private sealed class TestFitFunction : IFitFunction
    {
      /// <summary>
      /// Stores the parameter names returned by the fit function.
      /// </summary>
      private readonly string[] _parameterNames;

      /// <summary>
      /// Stores the hard lower parameter bounds.
      /// </summary>
      private readonly IReadOnlyList<double?>? _hardLowerBounds;

      /// <summary>
      /// Stores the hard upper parameter bounds.
      /// </summary>
      private readonly IReadOnlyList<double?>? _hardUpperBounds;

      /// <summary>
      /// Stores the soft lower parameter bounds.
      /// </summary>
      private readonly IReadOnlyList<double?>? _softLowerBounds;

      /// <summary>
      /// Stores the soft upper parameter bounds.
      /// </summary>
      private readonly IReadOnlyList<double?>? _softUpperBounds;

      /// <summary>
      /// Initializes a new instance of the <see cref="TestFitFunction"/> class.
      /// </summary>
      /// <param name="parameterNames">The parameter names exposed by the fit function.</param>
      /// <param name="hardLowerBounds">The hard lower parameter bounds.</param>
      /// <param name="hardUpperBounds">The hard upper parameter bounds.</param>
      /// <param name="softLowerBounds">The soft lower parameter bounds.</param>
      /// <param name="softUpperBounds">The soft upper parameter bounds.</param>
      public TestFitFunction(
          string[] parameterNames,
          IReadOnlyList<double?>? hardLowerBounds = null,
          IReadOnlyList<double?>? hardUpperBounds = null,
          IReadOnlyList<double?>? softLowerBounds = null,
          IReadOnlyList<double?>? softUpperBounds = null)
      {
        _parameterNames = parameterNames;
        _hardLowerBounds = hardLowerBounds;
        _hardUpperBounds = hardUpperBounds;
        _softLowerBounds = softLowerBounds;
        _softUpperBounds = softUpperBounds;
      }

      /// <inheritdoc/>
      public int NumberOfIndependentVariables => 1;

      /// <inheritdoc/>
      public int NumberOfDependentVariables => 1;

      /// <inheritdoc/>
      public int NumberOfParameters => _parameterNames.Length;

      /// <inheritdoc/>
      public event EventHandler? Changed;

      /// <inheritdoc/>
      public string IndependentVariableName(int i) => $"x{i}";

      /// <inheritdoc/>
      public string DependentVariableName(int i) => $"y{i}";

      /// <inheritdoc/>
      public string ParameterName(int i) => _parameterNames[i];

      /// <inheritdoc/>
      public double DefaultParameterValue(int i) => 0;

      /// <inheritdoc/>
      public IVarianceScaling? DefaultVarianceScaling(int i) => null;

      /// <inheritdoc/>
      public void Evaluate(double[] independent, double[] parameters, double[] dependent)
          => throw new NotSupportedException();

      /// <inheritdoc/>
      public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> dependent, IReadOnlyList<bool>? dependentVariableChoice)
          => throw new NotSupportedException();

      /// <inheritdoc/>
      public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
          => (_hardLowerBounds, _hardUpperBounds);

      /// <inheritdoc/>
      public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
          => (_softLowerBounds, _softUpperBounds);

      /// <summary>
      /// Raises the <see cref="Changed"/> event.
      /// </summary>
      public void RaiseChanged()
          => Changed?.Invoke(this, EventArgs.Empty);
    }

    // ── Basic parsing ─────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a simple equality with a sum is parsed into the expected coefficients and right-hand side.
    /// </summary>
    [Fact]
    public void Equality_SimpleSum_ParsedCorrectly()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("a + b == 1");

      Assert.True(result.IsSuccess);
      Assert.Single(result.Constraints);

      var c = result.Constraints[0];
      Assert.Equal(ConstraintKind.Equality, c.Kind);
      Assert.Equal(1.0, c.Coefficients[0], precision: 10); // a
      Assert.Equal(1.0, c.Coefficients[1], precision: 10); // b
      Assert.Equal(1.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that a simple less-than-or-equal constraint is parsed correctly.
    /// </summary>
    [Fact]
    public void LessOrEqual_SimpleParameter_ParsedCorrectly()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "x", "y" });
      var result = compiler.CompileOne("x <= 5");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(ConstraintKind.LessOrEqual, c.Kind);
      Assert.Equal(1.0, c.Coefficients[0], precision: 10);
      Assert.Equal(0.0, c.Coefficients[1], precision: 10);
      Assert.Equal(5.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that a simple greater-than-or-equal constraint is parsed correctly.
    /// </summary>
    [Fact]
    public void GreaterOrEqual_SimpleParameter_ParsedCorrectly()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "x", "y" });
      var result = compiler.CompileOne("y >= 2");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(ConstraintKind.GreaterOrEqual, c.Kind);
      Assert.Equal(0.0, c.Coefficients[0], precision: 10);
      Assert.Equal(1.0, c.Coefficients[1], precision: 10);
      Assert.Equal(2.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that a strict less-than operator is normalized to <see cref="ConstraintKind.LessOrEqual"/>.
    /// </summary>
    [Fact]
    public void StrictLessThan_TreatedAsLessOrEqual()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "x" });
      var result = compiler.CompileOne("x < 3");

      Assert.True(result.IsSuccess);
      Assert.Equal(ConstraintKind.LessOrEqual, result.Constraints[0].Kind);
    }

    /// <summary>
    /// Verifies that a strict greater-than operator is normalized to <see cref="ConstraintKind.GreaterOrEqual"/>.
    /// </summary>
    [Fact]
    public void StrictGreaterThan_TreatedAsGreaterOrEqual()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "x" });
      var result = compiler.CompileOne("x > 3");

      Assert.True(result.IsSuccess);
      Assert.Equal(ConstraintKind.GreaterOrEqual, result.Constraints[0].Kind);
    }

    // ── Coefficient extraction ────────────────────────────────────────────

    /// <summary>
    /// Verifies that scalar multipliers are extracted into the corresponding parameter coefficients.
    /// </summary>
    [Fact]
    public void ScalarMultiplier_ExtractedCorrectly()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("2 * a + 3 * b <= 10");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(2.0, c.Coefficients[0], precision: 10);
      Assert.Equal(3.0, c.Coefficients[1], precision: 10);
      Assert.Equal(10.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that grouped additive terms are distributed when multiplied by a scalar.
    /// </summary>
    [Fact]
    public void GroupedTerms_DistributedCorrectly()
    {
      // 2*(a+b) <= 10  →  2a + 2b <= 10
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("2 * (a + b) <= 10");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(2.0, c.Coefficients[0], precision: 10);
      Assert.Equal(2.0, c.Coefficients[1], precision: 10);
      Assert.Equal(10.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that named constants are resolved when used on the right-hand side.
    /// </summary>
    [Fact]
    public void NamedConstant_UsedCorrectly()
    {
      var compiler = new LinearConstraintsCompiler(
          new[] { "a", "b" },
          new Dictionary<string, double> { ["maxVal"] = 10.0 });

      var result = compiler.CompileOne("a + 2 * b <= maxVal");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(10.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that named constants on the left-hand side are subtracted into the right-hand side.
    /// </summary>
    [Fact]
    public void NamedConstantOnLhs_Subtracted()
    {
      // a + offset <= 5  where offset=2 → a <= 3
      var compiler = new LinearConstraintsCompiler(
          new[] { "a" },
          new Dictionary<string, double> { ["offset"] = 2.0 });

      var result = compiler.CompileOne("a + offset <= 5");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(1.0, c.Coefficients[0], precision: 10);
      Assert.Equal(3.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that unary minus negates the affected coefficient.
    /// </summary>
    [Fact]
    public void UnaryMinus_NegatesCoefficient()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("-a + b == 0");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      Assert.Equal(-1.0, c.Coefficients[0], precision: 10);
      Assert.Equal(1.0, c.Coefficients[1], precision: 10);
      Assert.Equal(0.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that parameter terms on the right-hand side are moved to the left-hand side.
    /// </summary>
    [Fact]
    public void TermsOnRhs_MovedToLhs()
    {
      // 1 == a + b  (rhs has parameters)  →  -a - b <= -1 (after normalisation)
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("1 == a + b");

      Assert.True(result.IsSuccess);
      var c = result.Constraints[0];
      // LHS coeff = 0-1 = -1, RHS = 1-0 = 1  →  -a - b == -1  i.e. a+b==1 from other side
      Assert.Equal(-1.0, c.Coefficients[0], precision: 10);
      Assert.Equal(-1.0, c.Coefficients[1], precision: 10);
      Assert.Equal(-1.0, c.Rhs, precision: 10);
    }

    /// <summary>
    /// Verifies that division by a numeric constant scales the coefficient accordingly.
    /// </summary>
    [Fact]
    public void Division_ByConstant_Works()
    {
      // a / 2 <= 5  →  0.5a <= 5
      var compiler = new LinearConstraintsCompiler(new[] { "a" });
      var result = compiler.CompileOne("a / 2 <= 5");

      Assert.True(result.IsSuccess);
      Assert.Equal(0.5, result.Constraints[0].Coefficients[0], precision: 10);
    }

    // ── Multiple expressions ──────────────────────────────────────────────

    /// <summary>
    /// Verifies that multiple valid expressions are all compiled into constraints in order.
    /// </summary>
    [Fact]
    public void MultipleExpressions_AllParsedCorrectly()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b", "c" });
      var result = compiler.Compile(new[]
      {
                "a + b == 1",
                "c <= 5",
                "a >= 0",
            });

      Assert.True(result.IsSuccess);
      Assert.Equal(3, result.Constraints.Count);
      Assert.Equal(ConstraintKind.Equality, result.Constraints[0].Kind);
      Assert.Equal(ConstraintKind.LessOrEqual, result.Constraints[1].Kind);
      Assert.Equal(ConstraintKind.GreaterOrEqual, result.Constraints[2].Kind);
    }

    /// <summary>
    /// Verifies that the fit-function constructor derives parameter names and hard bounds as implicit constraints.
    /// </summary>
    [Fact]
    public void Constructor_WithFitFunction_AddsHardBounds()
    {
      var fitFunction = new TestFitFunction(
          ["alpha", "beta"],
          hardLowerBounds: [0.0, null],
          hardUpperBounds: [10.0, 5.0]);

      var compiler = new LinearConstraintsCompiler(fitFunction);
      var result = compiler.Compile(Array.Empty<string>());

      Assert.True(result.IsSuccess);
      Assert.Equal(3, result.Constraints.Count);

      Assert.Collection(
          result.Constraints,
          c =>
          {
            Assert.Equal("alpha >= 0", c.Expression);
            Assert.Equal(ConstraintKind.GreaterOrEqual, c.Kind);
            Assert.Equal(1.0, c.Coefficients[0], precision: 10);
            Assert.Equal(0.0, c.Coefficients[1], precision: 10);
            Assert.Equal(0.0, c.Rhs, precision: 10);
          },
          c =>
          {
            Assert.Equal("alpha <= 10", c.Expression);
            Assert.Equal(ConstraintKind.LessOrEqual, c.Kind);
            Assert.Equal(1.0, c.Coefficients[0], precision: 10);
            Assert.Equal(0.0, c.Coefficients[1], precision: 10);
            Assert.Equal(10.0, c.Rhs, precision: 10);
          },
          c =>
          {
            Assert.Equal("beta <= 5", c.Expression);
            Assert.Equal(ConstraintKind.LessOrEqual, c.Kind);
            Assert.Equal(0.0, c.Coefficients[0], precision: 10);
            Assert.Equal(1.0, c.Coefficients[1], precision: 10);
            Assert.Equal(5.0, c.Rhs, precision: 10);
          });
    }

    /// <summary>
    /// Verifies that optional soft bounds tighten the effective box constraints.
    /// </summary>
    [Fact]
    public void Constructor_WithFitFunction_OptionalSoftBoundsTightenLimits()
    {
      var fitFunction = new TestFitFunction(
          ["alpha"],
          hardLowerBounds: [0.0],
          hardUpperBounds: [10.0],
          softLowerBounds: [2.0],
          softUpperBounds: [8.0]);

      var compiler = new LinearConstraintsCompiler(fitFunction, includeSoftParameterBoundaries: true);
      var result = compiler.Compile(Array.Empty<string>());

      Assert.True(result.IsSuccess);
      Assert.Equal(2, result.Constraints.Count);

      Assert.Collection(
          result.Constraints,
          c =>
          {
            Assert.Equal("alpha >= 2", c.Expression);
            Assert.Equal(ConstraintKind.GreaterOrEqual, c.Kind);
            Assert.Equal(2.0, c.Rhs, precision: 10);
          },
          c =>
          {
            Assert.Equal("alpha <= 8", c.Expression);
            Assert.Equal(ConstraintKind.LessOrEqual, c.Kind);
            Assert.Equal(8.0, c.Rhs, precision: 10);
          });
    }

    /// <summary>
    /// Verifies that explicit expressions are combined with implicit fit-function bounds.
    /// </summary>
    [Fact]
    public void Constructor_WithFitFunction_CombinesImplicitBoundsWithExplicitConstraints()
    {
      var fitFunction = new TestFitFunction(
          ["alpha", "beta"],
          hardLowerBounds: [0.0, 1.0],
          hardUpperBounds: [10.0, null]);

      var compiler = new LinearConstraintsCompiler(fitFunction);
      var result = compiler.CompileOne("alpha + beta == 4");

      Assert.True(result.IsSuccess);
      Assert.Equal(4, result.Constraints.Count);
      Assert.Equal(ConstraintKind.Equality, result.Constraints[0].Kind);
      Assert.Equal(ConstraintKind.GreaterOrEqual, result.Constraints[1].Kind);
      Assert.Equal(ConstraintKind.LessOrEqual, result.Constraints[2].Kind);
      Assert.Equal(ConstraintKind.GreaterOrEqual, result.Constraints[3].Kind);
    }

    // ── Error cases ───────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that an unknown identifier produces a diagnostic message.
    /// </summary>
    [Fact]
    public void UnknownIdentifier_ReturnsDiagnostic()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a" });
      var result = compiler.CompileOne("a + z <= 5");

      Assert.False(result.IsSuccess);
      Assert.Single(result.Diagnostics);
      Assert.Contains("z", result.Diagnostics[0].Message);
    }

    /// <summary>
    /// Verifies that a nonlinear multiplication produces a diagnostic message.
    /// </summary>
    [Fact]
    public void NonlinearExpression_ReturnsDiagnostic()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("a * b <= 5");

      Assert.False(result.IsSuccess);
      Assert.Contains("Nonlinear", result.Diagnostics[0].Message);
    }

    /// <summary>
    /// Verifies that not-equal constraints are rejected.
    /// </summary>
    [Fact]
    public void NotEqual_ReturnsDiagnostic()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a" });
      var result = compiler.CompileOne("a != 5");

      Assert.False(result.IsSuccess);
    }

    /// <summary>
    /// Verifies that expressions without a relational operator produce a diagnostic.
    /// </summary>
    [Fact]
    public void MissingRelationalOperator_ReturnsDiagnostic()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a" });
      var result = compiler.CompileOne("a + 1");

      Assert.False(result.IsSuccess);
    }

    /// <summary>
    /// Verifies that division by a parameter-containing expression is rejected as nonlinear.
    /// </summary>
    [Fact]
    public void DivisionByParameterExpression_ReturnsDiagnostic()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.CompileOne("a / b <= 5");

      Assert.False(result.IsSuccess);
      Assert.Contains("Nonlinear", result.Diagnostics[0].Message);
    }

    /// <summary>
    /// Verifies that overlapping parameter and constant names are rejected during construction.
    /// </summary>
    [Fact]
    public void OverlappingParameterAndConstantNames_ThrowsOnConstruction()
    {
      Assert.Throws<ArgumentException>(() =>
          new LinearConstraintsCompiler(
              new[] { "a" },
              new Dictionary<string, double> { ["a"] = 1.0 }));
    }

    /// <summary>
    /// Verifies that overlapping fit-function parameter names and constants are rejected during construction.
    /// </summary>
    [Fact]
    public void OverlappingFitFunctionParameterAndConstantNames_ThrowsOnConstruction()
    {
      var fitFunction = new TestFitFunction(["a"]);

      Assert.Throws<ArgumentException>(() =>
          new LinearConstraintsCompiler(
              fitFunction,
              new Dictionary<string, double> { ["a"] = 1.0 }));
    }

    /// <summary>
    /// Verifies that valid constraints are returned even when another expression produces a diagnostic.
    /// </summary>
    [Fact]
    public void MixedValidAndInvalidExpressions_PartialResult()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[]
      {
                "a + b == 1",   // valid
                "a * b <= 5",   // invalid (nonlinear)
                "a <= 3",       // valid
            });

      Assert.False(result.IsSuccess);
      Assert.Equal(2, result.Constraints.Count);
      Assert.Single(result.Diagnostics);
    }

    // ── ToProjectorMatrices ───────────────────────────────────────────────

    /// <summary>
    /// Verifies that equality constraints are emitted into matrix A and vector B.
    /// </summary>
    [Fact]
    public void ToProjectorMatrices_EqualityGoesToA()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[] { "a + b == 1" });
      var matrices = result.ToProjectorMatrices();

      Assert.NotNull(matrices.A);
      Assert.Null(matrices.C);
      Assert.Equal(1, matrices.A!.RowCount);
      Assert.Equal(1.0, matrices.B![0], precision: 10);
    }

    /// <summary>
    /// Verifies that less-than-or-equal constraints are emitted into matrix C and vector D without modification.
    /// </summary>
    [Fact]
    public void ToProjectorMatrices_LeqGoesToC_AsIs()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[] { "a <= 3" });
      var matrices = result.ToProjectorMatrices();

      Assert.Null(matrices.A);
      Assert.NotNull(matrices.C);
      Assert.Equal(1.0, matrices.C![0, 0], precision: 10); // a coefficient
      Assert.Equal(3.0, matrices.D![0], precision: 10);
    }

    /// <summary>
    /// Verifies that greater-than-or-equal constraints are negated when emitted into matrix C and vector D.
    /// </summary>
    [Fact]
    public void ToProjectorMatrices_GeqNegatedIntoC()
    {
      // a >= 1  →  -a <= -1
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b" });
      var result = compiler.Compile(new[] { "a >= 1" });
      var matrices = result.ToProjectorMatrices();

      Assert.NotNull(matrices.C);
      Assert.Equal(-1.0, matrices.C![0, 0], precision: 10); // negated
      Assert.Equal(-1.0, matrices.D![0], precision: 10); // negated
    }

    /// <summary>
    /// Verifies that mixed equality and inequality constraints are split across the expected projector matrices.
    /// </summary>
    [Fact]
    public void ToProjectorMatrices_MixedConstraints_AllPresent()
    {
      var compiler = new LinearConstraintsCompiler(new[] { "a", "b", "c" });
      var result = compiler.Compile(new[]
      {
                "a + b == 1",
                "c <= 5",
                "a >= 0",
            });
      var matrices = result.ToProjectorMatrices();

      Assert.NotNull(matrices.A);
      Assert.NotNull(matrices.C);
      Assert.Equal(1, matrices.A!.RowCount); // one equality
      Assert.Equal(2, matrices.C!.RowCount); // leq + negated geq
    }
  }
}
