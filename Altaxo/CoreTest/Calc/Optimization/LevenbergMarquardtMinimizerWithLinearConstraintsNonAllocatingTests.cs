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
using System.Threading;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization.ObjectiveFunctions;
using Xunit;

namespace Altaxo.Calc.Optimization
{
  public class LevenbergMarquardtMinimizerWithLinearConstraintsNonAllocatingTests
  {
    [Fact]
    public void LinearConstraints_WithoutProjector_MatchesWrappedSolution()
    {
      var wrapped = RunWrapped(new[] { 10.0, 5.0, 3.0 });
      var linear = RunLinear(new[] { 10.0, 5.0, 3.0 });

      AssertSamePoint(wrapped.MinimizingPoint, linear.MinimizingPoint);
      AssertEx.AreEqual(wrapped.ModelInfoAtMinimum.Value, linear.ModelInfoAtMinimum.Value, 1E-12, 1E-6);
    }

    [Fact]
    public void LinearConstraints_LowerBound_EnforcesConstraintAndImprovesFit()
    {
      var lowerBounds = new double?[] { null, null, 2.0 };

      var projector = CreateProjector(3, lowerBounds: lowerBounds);
      var linear = RunLinear(new[] { 10.0, 5.0, 3.0 }, projector: projector);

      Assert.True(projector.IsFeasible(linear.MinimizingPoint));
      AssertParameter(linear.MinimizingPoint, 2, 2.0, 1E-8);
      Assert.True(linear.IsFixedByUserOrBoundaries[2]);
      AssertEx.GreaterOrEqual(32.0, linear.ModelInfoAtMinimum.Value);
    }

    [Fact]
    public void LinearConstraints_UpperBound_EnforcesConstraintAndImprovesFit()
    {
      var upperBounds = new double?[] { 16.0, null, null };

      var projector = CreateProjector(3, upperBounds: upperBounds);
      var linear = RunLinear(new[] { 10.0, 5.0, 3.0 }, projector: projector);

      Assert.True(projector.IsFeasible(linear.MinimizingPoint));
      AssertParameter(linear.MinimizingPoint, 0, 16.0, 1E-8);
      Assert.True(linear.IsFixedByUserOrBoundaries[0]);
      AssertEx.GreaterOrEqual(2.1, linear.ModelInfoAtMinimum.Value);
    }

    [Fact]
    public void LinearConstraints_MixedBoxConstraints_MatchWrappedParametersMinimizer()
    {
      var lowerBounds = new double?[] { null, 5.125, 2.0 };
      var upperBounds = new double?[] { 12.0, null, null };
      var initialGuess = new[] { 10.0, 5.5, 3.0 };

      var wrapped = RunWrapped(initialGuess, lowerBound: lowerBounds, upperBound: upperBounds);
      var linear = RunLinear(initialGuess, projector: CreateProjector(3, lowerBounds, upperBounds));

      AssertSamePoint(wrapped.MinimizingPoint, linear.MinimizingPoint, 1E-2);
      AssertParameter(linear.MinimizingPoint, 0, 12.0, 1E-8);
      AssertParameter(linear.MinimizingPoint, 1, 5.125, 1E-2);
      AssertParameter(linear.MinimizingPoint, 2, 2.0, 1E-2);
      Assert.All(linear.IsFixedByUserOrBoundaries, Assert.True);
    }

    [Fact]
    public void LinearConstraints_EqualityConstraint_IsEnforced()
    {
      var projector = CreateProjector(
        3,
        equalityMatrix: new double[,] { { 0.0, 1.0, 0.0 } },
        equalityRhs: new[] { 5.125 });

      var result = RunLinear(new[] { 10.0, 5.125, 3.0 }, projector: projector);

      Assert.True(projector.IsFeasible(result.MinimizingPoint));
      AssertParameter(result.MinimizingPoint, 1, 5.125, 1E-12);
      Assert.True(result.IsFixedByUserOrBoundaries[1]);
    }

    [Fact]
    public void LinearConstraints_EqualityAndInequality_AreEnforcedTogether()
    {
      var projector = CreateProjector(
        3,
        lowerBounds: new double?[] { null, null, 2.0 },
        upperBounds: new double?[] { 12.0, null, null },
        equalityMatrix: new double[,] { { 0.0, 1.0, 0.0 } },
        equalityRhs: new[] { 5.125 });

      var result = RunLinear(new[] { 10.0, 5.125, 3.0 }, projector: projector);

      Assert.True(projector.IsFeasible(result.MinimizingPoint));
      AssertParameter(result.MinimizingPoint, 0, 12.0, 1E-8);
      AssertParameter(result.MinimizingPoint, 1, 5.125, 1E-12);
      AssertParameter(result.MinimizingPoint, 2, 2.0, 1E-8);
      Assert.All(result.IsFixedByUserOrBoundaries, Assert.True);
    }

    [Fact]
    public void LinearConstraints_WithUserFixedParameter_RespectsFixingAndFeasibility()
    {
      var projector = CreateProjector(
        3,
        lowerBounds: new double?[] { null, null, 2.0 },
        upperBounds: new double?[] { 12.0, null, null });

      var result = RunLinear(
        initialGuess: new[] { 10.0, 5.25, 3.0 },
        projector: projector,
        isFixed: new[] { false, true, false });

      Assert.True(projector.IsFeasible(result.MinimizingPoint));
      AssertParameter(result.MinimizingPoint, 1, 5.25, 1E-12);
      AssertParameter(result.MinimizingPoint, 2, 2.0, 1E-8);
      Assert.True(result.IsFixedByUserOrBoundaries[1]);
      Assert.True(result.IsFixedByUserOrBoundaries[2]);
    }

    [Fact]
    public void LinearConstraints_InfeasibleInitialGuess_ThrowsArgumentException()
    {
      var projector = CreateProjector(3, lowerBounds: new double?[] { null, null, 2.0 });

      Assert.Throws<ArgumentException>(() => RunLinear(new[] { 10.0, 5.0, 1.5 }, projector: projector));
    }

    private static NonlinearMinimizationResult RunWrapped(
      double[] initialGuess,
      double?[]? lowerBound = null,
      double?[]? upperBound = null,
      double[]? scales = null,
      bool[]? isFixed = null)
    {
      var objective = CreateGaussianObjective();
      var fit = new LevenbergMarquardtMinimizerNonAllocatingWrappedParameters();
      return fit.FindMinimum(objective, initialGuess, lowerBound, upperBound, scales, isFixed, CancellationToken.None, null);
    }

    private static NonlinearMinimizationResult RunLinear(
      double[] initialGuess,
      LinearConstraintsProjector? projector = null,
      double[]? scales = null,
      bool[]? isFixed = null)
    {
      var objective = CreateGaussianObjective();
      var fit = new LevenbergMarquardtMinimizerWithConstraintsNonAllocating
      {
        Projector = projector,
      };

      return fit.FindMinimum(objective, initialGuess, scales, isFixed, CancellationToken.None, null);
    }

    private static NonlinearObjectiveFunctionNonAllocating CreateGaussianObjective()
    {
      var (x, y) = CreateGaussianData();
      var fitFunction = new GaussAmplitude(1, -1);
      var objective = new NonlinearObjectiveFunctionNonAllocating(fitFunction.Evaluate, fitFunction.EvaluateDerivative, 1);
      objective.SetObserved(x, y, null);
      return objective;
    }

    private static (double[] x, double[] y) CreateGaussianData()
    {
      var x = new double[10];
      var y = new double[10];

      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = i;
        double arg = (x[i] - 5.0) / 1.5;
        y[i] = 17.0 * Math.Exp(-0.5 * arg * arg);
      }

      return (x, y);
    }

    private static LinearConstraintsProjector CreateProjector(
      int parameterCount,
      double?[]? lowerBounds = null,
      double?[]? upperBounds = null,
      double[,]? equalityMatrix = null,
      double[]? equalityRhs = null)
    {
      Matrix<double>? A = equalityMatrix is null ? null : Matrix<double>.Build.DenseOfArray(equalityMatrix);
      Vector<double>? b = equalityRhs is null ? null : Vector<double>.Build.DenseOfArray(equalityRhs);

      var rows = new List<double[]>();
      var rhs = new List<double>();

      if (lowerBounds is not null)
      {
        for (int i = 0; i < lowerBounds.Length; i++)
        {
          if (!lowerBounds[i].HasValue)
            continue;

          var row = new double[parameterCount];
          row[i] = -1.0;
          rows.Add(row);
          rhs.Add(-lowerBounds[i]!.Value);
        }
      }

      if (upperBounds is not null)
      {
        for (int i = 0; i < upperBounds.Length; i++)
        {
          if (!upperBounds[i].HasValue)
            continue;

          var row = new double[parameterCount];
          row[i] = 1.0;
          rows.Add(row);
          rhs.Add(upperBounds[i]!.Value);
        }
      }

      Matrix<double>? C = null;
      Vector<double>? d = null;

      if (rows.Count > 0)
      {
        C = Matrix<double>.Build.Dense(rows.Count, parameterCount);
        for (int r = 0; r < rows.Count; r++)
          for (int c = 0; c < parameterCount; c++)
            C[r, c] = rows[r][c];

        d = Vector<double>.Build.DenseOfEnumerable(rhs);
      }

      return new LinearConstraintsProjector(A: A, b: b, C: C, d: d);
    }

    private static void AssertSamePoint(IReadOnlyList<double> expected, IReadOnlyList<double> actual, double relativeTolerance = 1E-6)
    {
      Assert.Equal(expected.Count, actual.Count);
      for (int i = 0; i < expected.Count; i++)
        AssertEx.AreEqual(expected[i], actual[i], 1E-12, relativeTolerance);
    }

    private static void AssertParameter(IReadOnlyList<double> values, int index, double expected, double relativeTolerance)
    {
      AssertEx.AreEqual(expected, values[index], 1E-12, relativeTolerance);
    }
  }
}
