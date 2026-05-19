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
using System.IO;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.Optimization
{

  // =========================================================================
  // Helpers
  // =========================================================================

  /// <summary>
  /// Provides helper members for linear-constraint projector tests.
  /// </summary>
  internal static class TestHelpers
  {
    /// <summary>
    /// Gets the default floating-point tolerance used by the tests.
    /// </summary>
    internal const double Tol = 1e-7;

    /// <summary>
    /// Creates a dense vector from the specified values.
    /// </summary>
    /// <param name="values">The values used to populate the vector.</param>
    /// <returns>A dense vector containing the supplied values.</returns>
    internal static Vector<double> V(params double[] values)
        => Vector<double>.Build.DenseOfArray(values);

    /// <summary>
    /// Creates a dense matrix from the specified values.
    /// </summary>
    /// <param name="values">The values used to populate the matrix.</param>
    /// <returns>A dense matrix containing the supplied values.</returns>
    internal static Matrix<double> M(double[,] values)
        => Matrix<double>.Build.DenseOfArray(values);

    /// <summary>
    /// Parses a combined matrix and vector from a jagged array, where each inner array represents a row of the matrix followed by the corresponding element of the vector.
    /// </summary>
    /// <param name="matrixAndVector"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal static (Matrix<double> M, Vector<double> V) MV(double[][] matrixAndVector)
    {
      if (matrixAndVector.Length % 2 == 1)
        throw new ArgumentException("Argument must contain an even number of rows, with each pair of rows representing a matrix row and corresponding vector element.", nameof(matrixAndVector));

      int rows = matrixAndVector.Length / 2;
      if (rows == 0)
        throw new ArgumentException("Input array must have at least one row.", nameof(matrixAndVector));

      int cols = matrixAndVector[0].Length;
      if (cols < 1)
        throw new ArgumentException("Each row must have at least two elements (matrix columns + vector element).", nameof(matrixAndVector));


      var M = Matrix<double>.Build.Dense(rows, cols);
      var V = Vector<double>.Build.Dense(rows);

      for (int i = 0; i < rows; i++)
      {
        if (matrixAndVector[2 * i].Length != cols)
          throw new ArgumentException($"Matrix in row {i} does not have the expected number of elements ({cols}).", nameof(matrixAndVector));
        if (matrixAndVector[2 * i + 1].Length != 1)
          throw new ArgumentException($"Vector in row {i} does not have exactly one element.", nameof(matrixAndVector));

        for (int j = 0; j < cols; j++)
          M[i, j] = matrixAndVector[i * 2][j];

        V[i] = matrixAndVector[2 * i + 1][0];
      }
      return (M, V);
    }


    /// <summary>
    /// Verifies that two vectors are equal within the specified tolerance.
    /// </summary>
    /// <param name="actual">The actual vector.</param>
    /// <param name="expected">The expected vector.</param>
    /// <param name="tol">The comparison tolerance.</param>
    internal static void AssertVector(
        Vector<double> actual, Vector<double> expected, double tol = Tol)
    {
      Assert.Equal(expected.Count, actual.Count);
      for (int i = 0; i < expected.Count; i++)
        Assert.Equal(expected[i], actual[i], precision: (int)-Math.Log10(tol));
    }

    /// <summary>
    /// Returns the vector difference <paramref name="minuend"/> - <paramref name="subtrahend"/>.
    /// </summary>
    internal static Vector<double> Subtract(Vector<double> minuend, Vector<double> subtrahend)
    {
      Assert.Equal(minuend.Count, subtrahend.Count);

      var result = Vector<double>.Build.Dense(minuend.Count);
      for (int i = 0; i < minuend.Count; i++)
        result[i] = minuend[i] - subtrahend[i];

      return result;
    }

    /// <summary>
    /// Returns the dot product of the specified vectors.
    /// </summary>
    internal static double Dot(Vector<double> left, Vector<double> right)
    {
      Assert.Equal(left.Count, right.Count);

      double sum = 0;
      for (int i = 0; i < left.Count; i++)
        sum += left[i] * right[i];

      return sum;
    }

    /// <summary>
    /// Returns the Euclidean norm of the specified vector.
    /// </summary>
    internal static double Norm(Vector<double> vector)
    {
      return Math.Sqrt(Dot(vector, vector));
    }
  }


  // =========================================================================
  // LinearConstraintsProjector tests
  // =========================================================================

  /// <summary>
  /// Contains tests for <see cref="LinearConstraintProjector"/>.
  /// </summary>
  public class LinearConstraintsProjectorTests
  {
    private static bool ContainsIndex(IReadOnlyList<int> indices, int value)
    {
      for (int i = 0; i < indices.Count; i++)
      {
        if (indices[i] == value)
          return true;
      }

      return false;
    }

    private static double NextRandomDouble(System.Random random, double minimum, double maximum)
    {
      return minimum + (random.NextDouble() * (maximum - minimum));
    }

    private static void AssertProjectedStepDoesNotReverseDirection(
      LinearConstraintsProjector proj,
      Vector<double> feasiblePoint,
      Vector<double> intendedPoint)
    {
      Assert.True(proj.IsFeasible(feasiblePoint), $"Expected feasible starting point, but got: {string.Join(", ", feasiblePoint)}");

      var result = ProjectWithCheckedProperties(proj, intendedPoint);
      var intendedStep = TestHelpers.Subtract(intendedPoint, feasiblePoint);
      var projectedStep = TestHelpers.Subtract(result.Parameters, feasiblePoint);
      double dot = TestHelpers.Dot(intendedStep, projectedStep);

      Assert.True(dot >= -1E-12, $"Projected step reverses the intended step direction. Dot={dot}");

      double intendedNorm = TestHelpers.Norm(intendedStep);
      double projectedNorm = TestHelpers.Norm(projectedStep);
      if (intendedNorm > TestHelpers.Tol && projectedNorm > TestHelpers.Tol)
      {
        double cosine = dot / (intendedNorm * projectedNorm);
        Assert.True(cosine >= -1E-12, $"Projected step has negative cosine relative to intended step. Cosine={cosine}");
      }
    }

    private static void AssertProjectedStepDoesNotReverseDirectionForRandomSteps(
      LinearConstraintsProjector proj,
      int sampleCount,
      int seed,
      Func<System.Random, Vector<double>> feasiblePointFactory,
      Func<System.Random, Vector<double>, Vector<double>> intendedPointFactory)
    {
      var random = new System.Random(seed);

      for (int i = 0; i < sampleCount; i++)
      {
        var feasiblePoint = feasiblePointFactory(random);
        var intendedPoint = intendedPointFactory(random, feasiblePoint);

        AssertProjectedStepDoesNotReverseDirection(proj, feasiblePoint, intendedPoint);
      }
    }

    private static LinearConstraintsProjector CreateProjectorForDiagnostics(
      Matrix<double>? A,
      Vector<double>? b,
      Matrix<double>? C,
      Vector<double>? d,
      bool validateCompatibility = true,
      bool preferConstructedFeasibleStartForProjection = false)
    {
      return LinearConstraintsProjector.CreateForDiagnostics(A, b, C, d, validateCompatibility, preferConstructedFeasibleStartForProjection);
    }

    private static void AssertProjectorPropertiesConsistent(LinearConstraintsProjector proj)
    {
      int aRowCount = proj.A_RowCount;
      int aColumnCount = proj.A_ColumnCount;
      int cRowCount = proj.C_RowCount;
      int cColumnCount = proj.C_ColumnCount;
      int inputEqualityRowCount = proj.InputEqualityRowCount;
      int inputInequalityRowCount = proj.InputInequalityRowCount;
      int initialNormalizedEqualityRowCount = proj.InitialNormalizedEqualityRowCount;
      int initialNormalizedInequalityRowCount = proj.InitialNormalizedInequalityRowCount;
      int preprocessingEliminatedFullyFreeParameterCount = proj.PreprocessingEliminatedFullyFreeParameterCount;
      int preprocessingEliminatedFixedParameterCount = proj.PreprocessingEliminatedFixedParameterCount;
      int lastProjectionIterationCount = proj.LastProjectionIterationCount;
      bool lastProjectionUsedLeastSquaresFallback = proj.LastProjectionUsedLeastSquaresFallback;
      var lastProjectionTerminationReason = proj.LastProjectionTerminationReason;
      bool lastProjectionUsedKktRecovery = proj.LastProjectionUsedKktRecovery;
      int lastProjectionKktRecoveryAttemptCount = proj.LastProjectionKktRecoveryAttemptCount;
      int? lastProjectionRecoveredByDroppingInequalityIndex = proj.LastProjectionRecoveredByDroppingInequalityIndex;

      Assert.Equal(proj.NumberOfParameters, proj.ValuesFixedByConstraints.Count);

      int fixedCount = 0;
      for (int i = 0; i < proj.ValuesFixedByConstraints.Count; i++)
      {
        if (proj.ValuesFixedByConstraints[i].HasValue)
        {
          fixedCount++;
          Assert.False(double.IsNaN(proj.ValuesFixedByConstraints[i]!.Value));
        }
      }

      Assert.Equal(fixedCount, proj.NumberOfParametersFixed);
      Assert.Equal(proj.NumberOfParameters - fixedCount, proj.NumberOfParametersThatCanVary);
      Assert.InRange(proj.NumberOfParametersFullyFree, 0, proj.NumberOfParametersThatCanVary);
      Assert.Equal(proj.NumberOfParametersThatCanVary - proj.NumberOfParametersFullyFree, proj.NumberOfParametersNeitherFixedNorFullyFree);
      Assert.Equal(fixedCount == proj.NumberOfParameters, proj.AreAllParametersFixed);
      Assert.Equal(proj.NumberOfParametersFullyFree == proj.NumberOfParameters, proj.AreAllParametersFullyFree);

      int internalParameterCount = aColumnCount > 0 ? aColumnCount : cColumnCount;
      Assert.Equal(proj.NumberOfParametersNeitherFixedNorFullyFree, internalParameterCount);

      if (aRowCount == 0)
        Assert.Equal(0, aColumnCount);
      else
        Assert.Equal(internalParameterCount, aColumnCount);

      if (cRowCount == 0)
        Assert.Equal(0, cColumnCount);
      else
        Assert.Equal(internalParameterCount, cColumnCount);

      Assert.True(initialNormalizedEqualityRowCount >= 0);
      Assert.True(initialNormalizedInequalityRowCount >= 0);
      Assert.InRange(initialNormalizedEqualityRowCount + initialNormalizedInequalityRowCount, 0, inputEqualityRowCount + inputInequalityRowCount);
      Assert.InRange(preprocessingEliminatedFullyFreeParameterCount, 0, proj.NumberOfParameters);
      Assert.InRange(preprocessingEliminatedFixedParameterCount, 0, proj.NumberOfParameters);

      Assert.True(lastProjectionIterationCount >= 0);
      Assert.True(Enum.IsDefined(typeof(LinearConstraintsProjector.ProjectionTerminationReason), lastProjectionTerminationReason));
      if (lastProjectionUsedLeastSquaresFallback)
        Assert.Equal(LinearConstraintsProjector.ProjectionTerminationReason.LeastSquaresFallback, lastProjectionTerminationReason);

      Assert.True(lastProjectionKktRecoveryAttemptCount >= 0);
      if (!lastProjectionUsedKktRecovery)
      {
        Assert.Null(lastProjectionRecoveredByDroppingInequalityIndex);
      }
      else
      {
        Assert.True(lastProjectionKktRecoveryAttemptCount > 0);
        Assert.NotNull(lastProjectionRecoveredByDroppingInequalityIndex);
        Assert.InRange(lastProjectionRecoveredByDroppingInequalityIndex!.Value, 0, Math.Max(0, cRowCount - 1));
      }
    }

    private static ProjectionResult ProjectWithCheckedProperties(LinearConstraintsProjector proj, Vector<double> point)
    {
      AssertProjectorPropertiesConsistent(proj);

      var result = proj.ProjectWithInfo(point);

      int aRowCount = proj.A_RowCount;
      int cRowCount = proj.C_RowCount;

      Assert.Equal(proj.NumberOfParameters, result.Parameters.Count);
      Assert.Equal(proj.NumberOfParameters, result.FixedValues.Length);
      Assert.Equal(proj.NumberOfParameters, result.IsConstrained.Length);
      Assert.True(proj.IsFeasible(result.Parameters), $"Projected point is not feasible: {string.Join(", ", result.Parameters)}");

      int constrainedCount = 0;
      int freeCount = 0;
      int fixedCount = 0;

      for (int parameterIndex = 0; parameterIndex < proj.NumberOfParameters; parameterIndex++)
      {
        Assert.Equal(proj.ValuesFixedByConstraints[parameterIndex], result.FixedValues[parameterIndex]);

        bool isConstrained = result.IsConstrained[parameterIndex];
        Assert.Equal(isConstrained, ContainsIndex(result.ConstrainedParameterIndices, parameterIndex));
        Assert.Equal(!isConstrained, ContainsIndex(result.FreeParameterIndices, parameterIndex));
        if (isConstrained)
          constrainedCount++;
        else
          freeCount++;

        bool isFixed = result.FixedValues[parameterIndex].HasValue;
        Assert.Equal(isFixed, ContainsIndex(result.FixedParameterIndices, parameterIndex));
        if (isFixed)
        {
          fixedCount++;
          Assert.Equal(result.FixedValues[parameterIndex]!.Value, result.Parameters[parameterIndex], precision: 7);
        }
      }

      Assert.Equal(proj.NumberOfParameters, constrainedCount + freeCount);
      Assert.Equal(constrainedCount, result.ConstrainedParameterIndices.Count);
      Assert.Equal(freeCount, result.FreeParameterIndices.Count);
      Assert.Equal(fixedCount, result.FixedParameterIndices.Count);
      Assert.Equal(proj.NumberOfParametersFixed, fixedCount);

      int totalConstraintCount = aRowCount + cRowCount;
      if (totalConstraintCount == 0)
      {
        Assert.Empty(result.ActiveConstraintIndices);
      }
      else
      {
        for (int i = 0; i < result.ActiveConstraintIndices.Count; i++)
          Assert.InRange(result.ActiveConstraintIndices[i], 0, totalConstraintCount - 1);
      }

      for (int equalityIndex = 0; equalityIndex < aRowCount; equalityIndex++)
        Assert.True(ContainsIndex(result.ActiveConstraintIndices, equalityIndex));

      return result;
    }

    // ── No constraints ────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that projection without constraints returns the original input unchanged.
    /// </summary>
    [Fact]
    public void NoConstraints_ReturnsInputUnchanged()
    {
      Assert.Throws<ArgumentNullException>(() => new LinearConstraintsProjector());
    }

    /// <summary>
    /// Verifies that projection rejects parameter vectors with the wrong size.
    /// </summary>
    [Fact]
    public void ProjectWithInfo_WrongParameterCount_ThrowsArgumentException()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var exception = Assert.Throws<ArgumentException>(() => proj.ProjectWithInfo(TestHelpers.V(1.0, 2.0, 3.0)));

      Assert.Equal("p", exception.ParamName);
    }

    /// <summary>
    /// Verifies that feasibility checks reject parameter vectors with the wrong size.
    /// </summary>
    [Fact]
    public void IsFeasible_WrongParameterCount_ThrowsArgumentException()
    {
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var exception = Assert.Throws<ArgumentException>(() => proj.IsFeasible(TestHelpers.V(1.0)));

      Assert.Equal("inputValues", exception.ParamName);
    }

    // ── Equality constraints ──────────────────────────────────────────────

    /// <summary>
    /// Verifies that a single equality constraint projects onto the corresponding hyperplane.
    /// </summary>
    [Fact]
    public void EqualityConstraint_ProjectsOntoHyperplane()
    {
      // A·x = b  →  x[0] + x[1] = 1
      // Point (0.3, 0.3) should project to (0.4, 0.6) — midpoint adjustment
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.3, 0.3));

      Assert.Equal(1.0, result.Parameters[0] + result.Parameters[1], precision: 7);
    }

    /// <summary>
    /// Verifies that an already feasible point remains unchanged under an equality constraint.
    /// </summary>
    [Fact]
    public void EqualityConstraint_AlreadyFeasiblePoint_IsUnchanged()
    {
      // x[0] + x[1] = 1, point (0.6, 0.4) is already on the plane
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var p = TestHelpers.V(0.6, 0.4);
      var result = ProjectWithCheckedProperties(proj, p);

      TestHelpers.AssertVector(result.Parameters, p);
    }

    /// <summary>
    /// Verifies that projection from a feasible point does not reverse the intended step under an equality constraint.
    /// </summary>
    [Fact]
    public void EqualityConstraint_FromFeasiblePoint_ProjectedStepHasNonnegativeCosine()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var feasiblePoint = TestHelpers.V(0.5, 0.5);
      var intendedPoint = TestHelpers.V(1.4, 0.4);

      AssertProjectedStepDoesNotReverseDirection(proj, feasiblePoint, intendedPoint);
    }

    /// <summary>
    /// Verifies over many random steps that projection from feasible points does not reverse the intended step under an equality constraint.
    /// </summary>
    [Fact]
    public void EqualityConstraint_RandomStepsFromFeasiblePoints_ProjectedStepHasNonnegativeCosine()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      AssertProjectedStepDoesNotReverseDirectionForRandomSteps(
        proj,
        sampleCount: 1000,
        seed: 1729,
        feasiblePointFactory: random =>
        {
          double x0 = NextRandomDouble(random, -4.0, 5.0);
          return TestHelpers.V(x0, 1.0 - x0);
        },
        intendedPointFactory: (random, feasiblePoint) => TestHelpers.V(
          feasiblePoint[0] + NextRandomDouble(random, -6.0, 6.0),
          feasiblePoint[1] + NextRandomDouble(random, -6.0, 6.0)));
    }

    /// <summary>
    /// Verifies that all parameters participating in an equality constraint are marked as constrained.
    /// </summary>
    [Fact]
    public void EqualityConstraint_AllParametersMarkedConstrained()
    {
      // x[0] + x[1] + x[2] = 1  —  all three appear in the row
      var A = TestHelpers.M(new double[,] { { 1, 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.2, 0.2, 0.2));

      Assert.All(result.IsConstrained, c => Assert.True(c));
      Assert.Empty(result.FreeParameterIndices);
    }

    /// <summary>
    /// Verifies that parameters not participating in an equality constraint remain marked as free.
    /// </summary>
    [Fact]
    public void EqualityConstraint_OnlyInvolvedParametersMarkedConstrained()
    {
      // x[0] + x[1] = 1  — x[2] is free
      var A = TestHelpers.M(new double[,] { { 1, 1, 0 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.2, 0.2, 5.0));

      Assert.True(result.IsConstrained[0]);
      Assert.True(result.IsConstrained[1]);
      Assert.False(result.IsConstrained[2]);
      Assert.Equal(new[] { 2 }, result.FreeParameterIndices);
    }

    /// <summary>
    /// Verifies that the projector finds the intersection point of two equality constraints.
    /// </summary>
    [Fact]
    public void TwoEqualityConstraints_IntersectionFound()
    {
      // x[0] + x[1] = 1
      // x[0] - x[1] = 0   → x[0]=0.5, x[1]=0.5
      var A = TestHelpers.M(new double[,]
      {
                { 1,  1 },
                { 1, -1 },
      });
      var b = TestHelpers.V(1.0, 0.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.0, 0.0));
      double fixedValue0 = result.FixedValues[0] ?? throw new InvalidOperationException();
      double fixedValue1 = result.FixedValues[1] ?? throw new InvalidOperationException();
      double projectorFixedValue0 = proj.ValuesFixedByConstraints[0] ?? throw new InvalidOperationException();
      double projectorFixedValue1 = proj.ValuesFixedByConstraints[1] ?? throw new InvalidOperationException();

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(0.5, 0.5));
      Assert.All(result.FixedValues, value => Assert.True(value.HasValue));
      Assert.Equal(0.5, fixedValue0, precision: 7);
      Assert.Equal(0.5, fixedValue1, precision: 7);
      Assert.Equal(new[] { 0, 1 }, result.FixedParameterIndices);
      Assert.Equal(0.5, projectorFixedValue0, precision: 7);
      Assert.Equal(0.5, projectorFixedValue1, precision: 7);
    }

    /// <summary>
    /// Verifies that duplicate equality constraints are simplified to a single effective row.
    /// </summary>
    [Fact]
    public void DuplicateEqualityConstraints_AreSimplified()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 1 },
                { 1, 1 },
      });
      var b = TestHelpers.V(1.0, 1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.3, 0.3));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(0.5, 0.5));
      Assert.Single(result.ActiveConstraintIndices);
      Assert.Contains(0, result.ActiveConstraintIndices);
    }

    /// <summary>
    /// Verifies that proportional equality constraints are simplified to a single effective row.
    /// </summary>
    [Fact]
    public void ProportionalEqualityConstraints_AreSimplified()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 1 },
                { 2, 2 },
      });
      var b = TestHelpers.V(1.0, 2.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.3, 0.3));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(0.5, 0.5));
      Assert.Single(result.ActiveConstraintIndices);
      Assert.Contains(0, result.ActiveConstraintIndices);
    }

    /// <summary>
    /// Verifies that all-zero rows in equality and inequality matrices are eliminated because they do not define constraints.
    /// </summary>
    [Fact]
    public void ZeroRowsInEqualityAndInequalityConstraints_AreEliminated()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 0, 0 },
                { 1, -1 },
                { 0, 0 },
      });
      var b = TestHelpers.V(0.0, 0.0, 0.0);

      var C = TestHelpers.M(new double[,]
      {
                { 0, 0 },
                { 1, 0 },
                { 0, 0 },
      });
      var d = TestHelpers.V(0.0, 1.0, 0.0);

      var proj = CreateProjectorForDiagnostics(A: A, b: b, C: C, d: d);

      Assert.Equal(1, proj.A_RowCount);
      Assert.Equal(1, proj.C_RowCount);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(2.0, 2.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.0, 1.0));
      Assert.Equal(new[] { 0, 1 }, result.ActiveConstraintIndices);
    }

    /// <summary>
    /// Verifies that preprocessing diagnostics report zero-row removal and fully free parameter elimination.
    /// </summary>
    [Fact]
    public void PreprocessingDiagnostics_ReportNormalizationAndFullyFreeElimination()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 0, 0, 0 },
                { 1, 0, 0 },
      });
      var b = TestHelpers.V(0.0, 1.0);

      var C = TestHelpers.M(new double[,]
      {
                { 0, 0, 0 },
                { 0, 1, 0 },
      });
      var d = TestHelpers.V(0.0, 2.0);

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      Assert.Equal(0, proj.A_RowCount);
      Assert.Equal(0, proj.A_ColumnCount);
      Assert.Equal(1, proj.C_RowCount);
      Assert.Equal(1, proj.C_ColumnCount);


      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5.0, 5.0, 7.0));


      Assert.Equal(1, result.Parameters[0]); // fixed by trivial equality constraint, hence it has to be exact
      Assert.Equal(2, result.Parameters[1], precision: 12); // fixed by inequality constraint
      Assert.Equal(7, result.Parameters[2], precision: 12);
    }

    // ── Inequality constraints ────────────────────────────────────────────

    /// <summary>
    /// Verifies that a feasible point remains unchanged under an inequality constraint.
    /// </summary>
    [Fact]
    public void InequalityConstraint_FeasiblePoint_IsUnchanged()
    {
      // x[0] <= 5, point (3,) is feasible
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(5.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var p = TestHelpers.V(3.0, 2.0);
      var result = ProjectWithCheckedProperties(proj, p);

      TestHelpers.AssertVector(result.Parameters, p);
    }

    /// <summary>
    /// Verifies over many random steps that projection from feasible points under combined equality and inequality constraints does not reverse the intended step.
    /// </summary>
    [Fact]
    public void MixedConstraints_RandomStepsFromFeasiblePoints_ProjectedStepHasNonnegativeCosine()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var C = TestHelpers.M(new double[,]
      {
                { -1,  0 },
                {  0, -1 },
      });
      var d = TestHelpers.V(0.0, 0.0);
      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      AssertProjectedStepDoesNotReverseDirectionForRandomSteps(
        proj,
        sampleCount: 1000,
        seed: 7331,
        feasiblePointFactory: random =>
        {
          double x0 = NextRandomDouble(random, 0.05, 0.95);
          return TestHelpers.V(x0, 1.0 - x0);
        },
        intendedPointFactory: (random, feasiblePoint) => TestHelpers.V(
          feasiblePoint[0] + NextRandomDouble(random, -2.0, 2.0),
          feasiblePoint[1] + NextRandomDouble(random, -2.0, 2.0)));
    }

    /// <summary>
    /// Verifies over many random steps that projection inside a box-constrained region does not reverse the intended step.
    /// </summary>
    [Fact]
    public void InequalityConstraints_RandomInteriorBoxSteps_ProjectedStepHasNonnegativeCosine()
    {
      var C = TestHelpers.M(new double[,]
      {
                {  1,  0 },
                { -1,  0 },
                {  0,  1 },
                {  0, -1 },
      });
      var d = TestHelpers.V(1.0, 0.0, 1.0, 0.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      AssertProjectedStepDoesNotReverseDirectionForRandomSteps(
        proj,
        sampleCount: 1000,
        seed: 7331,
        feasiblePointFactory: random =>
          TestHelpers.V(
            NextRandomDouble(random, 0.0, 1.0),
            NextRandomDouble(random, 0.0, 1.0)),
        intendedPointFactory: (random, feasiblePoint) => TestHelpers.V(
          feasiblePoint[0] + NextRandomDouble(random, -3.0, 3.0),
          feasiblePoint[1] + NextRandomDouble(random, -3.0, 3.0)));
    }

    /// <summary>
    /// Verifies over many random outward steps from an active box boundary that projection never reverses the intended step.
    /// </summary>
    [Fact]
    public void InequalityConstraint_RandomOutwardStepsFromBoundary_ProjectedStepHasNonnegativeCosine()
    {
      var C = TestHelpers.M(new double[,]
      {
                {  1,  0 },
                { -1,  0 },
                {  0,  1 },
                {  0, -1 },
      });
      var d = TestHelpers.V(1.0, 0.0, 1.0, 0.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      AssertProjectedStepDoesNotReverseDirectionForRandomSteps(
        proj,
        sampleCount: 1000,
        seed: 24601,
        feasiblePointFactory: random =>
        {
          double y = NextRandomDouble(random, 0.0, 1.0);
          return TestHelpers.V(1.0, y);
        },
        intendedPointFactory: (random, feasiblePoint) => TestHelpers.V(
          feasiblePoint[0] + NextRandomDouble(random, 0.1, 5.0),
          feasiblePoint[1] + NextRandomDouble(random, -3.0, 3.0)));
    }

    /// <summary>
    /// Verifies that an infeasible point is projected onto the active inequality boundary.
    /// </summary>
    [Fact]
    public void InequalityConstraint_InfeasiblePoint_ProjectedOntoBoundary()
    {
      // x[0] <= 2, point (5, 1) must be projected to (2, 1)
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5.0, 1.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(2.0, 1.0));
    }

    /// <summary>
    /// Verifies that a parameter is marked as constrained when its inequality bound is active.
    /// </summary>
    [Fact]
    public void InequalityConstraint_ActiveBound_ParameterMarkedConstrained()
    {
      // x[0] <= 2, projection hits the bound → x[0] is constrained, x[1] is free
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5.0, 1.0));

      Assert.True(result.IsConstrained[0]);
      Assert.False(result.IsConstrained[1]);
    }

    /// <summary>
    /// Verifies that parameters remain marked as free when an inequality bound is inactive.
    /// </summary>
    [Fact]
    public void InequalityConstraint_InactiveBound_ParameterMarkedFree()
    {
      // x[0] <= 10, point (3,) does not hit the bound
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(10.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(3.0, 7.0));

      Assert.False(result.IsConstrained[0]);
      Assert.False(result.IsConstrained[1]);
    }

    /// <summary>
    /// Verifies that a lower bound expressed as a negated inequality row projects correctly.
    /// </summary>
    [Fact]
    public void GreaterOrEqualBoxConstraint_NegatedRow_ProjectsCorrectly()
    {
      // x[0] >= 1  expressed as  -x[0] <= -1
      var C = TestHelpers.M(new double[,] { { -1, 0 } });
      var d = TestHelpers.V(-1.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      // Point (0.5, 3) violates x[0] >= 1, should project to (1, 3)
      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.5, 3.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.0, 3.0));
    }

    /// <summary>
    /// Verifies that multiple box constraints project an infeasible point to the correct corner.
    /// </summary>
    [Fact]
    public void MultipleInequalityConstraints_BoxCorner_ProjectedCorrectly()
    {
      // 0 <= x[i] <= 1 for i=0,1 expressed as:
      //   x[0] <= 1, x[1] <= 1, -x[0] <= 0, -x[1] <= 0
      var C = TestHelpers.M(new double[,]
      {
                {  1,  0 },   // x[0] <= 1
                {  0,  1 },   // x[1] <= 1
                { -1,  0 },   // x[0] >= 0
                {  0, -1 },   // x[1] >= 0
      });
      var d = TestHelpers.V(1.0, 1.0, 0.0, 0.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      // Point (2, -1) projects to corner (1, 0)
      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(2.0, -1.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.0, 0.0));
      Assert.True(result.IsConstrained[0]);
      Assert.True(result.IsConstrained[1]);
    }

    /// <summary>
    /// Verifies that projection remains stable when the final active set contains dependent constraints.
    /// </summary>
    [Fact]
    public void DependentActiveSet_WithEqualityAndInequalities_ProjectsStably()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 1, 1 },
      });
      var b = TestHelpers.V(1.0);

      var C = TestHelpers.M(new double[,]
      {
                { 1, 1, 0 },
                { 0, 0, -1 },
      });
      var d = TestHelpers.V(1.0, 0.0);

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(2.0, 0.0, -1.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.5, -0.5, 0.0));
      Assert.All(result.Parameters, value => Assert.True(double.IsFinite(value)));
      Assert.True(proj.IsFeasible(result.Parameters));
      Assert.True(result.IsConstrained[2]);
    }

    /// <summary>
    /// Verifies that recovery from a dependent active set remains deterministic across repeated projections.
    /// </summary>
    [Fact]
    public void DependentActiveSet_Recovery_IsDeterministicAcrossRepeatedProjection()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 1, 1 },
      });
      var b = TestHelpers.V(1.0);

      var C = TestHelpers.M(new double[,]
      {
                { 1, 1, 0 },
                { 0, 0, -1 },
      });
      var d = TestHelpers.V(1.0, 0.0);

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);
      var point = TestHelpers.V(2.0, 0.0, -1.0);

      var first = ProjectWithCheckedProperties(proj, point);
      var second = ProjectWithCheckedProperties(proj, point);

      TestHelpers.AssertVector(second.Parameters, first.Parameters);
      Assert.Equal(first.ActiveConstraintIndices, second.ActiveConstraintIndices);
      Assert.Equal(first.ConstrainedParameterIndices, second.ConstrainedParameterIndices);
    }

    /// <summary>
    /// Verifies that KKT recovery diagnostics are populated for the dependent active-set scenario.
    /// </summary>
    [Fact]
    public void DependentActiveSet_ReportsKktRecoveryDiagnostics()
    {
      var proj = CreateProjectorForDiagnostics(A: null, b: null, C: TestHelpers.M(new double[,] { { 1, 0 } }), d: TestHelpers.V(0.0), validateCompatibility: false);
      var singularC = TestHelpers.M(new double[,]
      {
                { 1, 0 },
                { 1, 0 },
      });
      var singularD = TestHelpers.V(0.0, 0.0);
      var point = TestHelpers.V(1.0, 2.0);
      var activeSet = new HashSet<int> { 0, 1 };
      var recovered = proj.ExecuteKktRecoveryForDiagnostics(point.Clone(), point, singularC, singularD, activeSet, 1e-10);

      Assert.NotNull(recovered);
      Assert.True(proj.LastProjectionUsedKktRecovery);
      Assert.True(proj.LastProjectionKktRecoveryAttemptCount > 0);
      Assert.NotNull(proj.LastProjectionRecoveredByDroppingInequalityIndex);
    }

    /// <summary>
    /// Verifies that a simple inequality projection reports a stable termination reason and iteration count.
    /// </summary>
    [Fact]
    public void SimpleInequalityProjection_ReportsTerminationDiagnostics()
    {
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5.0, 1.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(2.0, 1.0));
      Assert.True(proj.LastProjectionIterationCount > 0);
      Assert.False(proj.LastProjectionUsedLeastSquaresFallback);
      Assert.Equal(LinearConstraintsProjector.ProjectionTerminationReason.OptimalStationaryPoint, proj.LastProjectionTerminationReason);
    }

    /// <summary>
    /// Verifies that simple inequality rows are converted to lower and upper box bounds on the correct parameters.
    /// </summary>
    [Fact]
    public void TryConvertToBoundaryConstraints_MapsUpperAndLowerBoundsCorrectly()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1, 0 },
                { -1, 0 },
                { 0, 1 },
                { 0, -1 },
      });
      var d = TestHelpers.V(5.0, -1.0, 7.0, -2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var bounds = proj.TryConvertToBoundaryConstraints();

      Assert.NotNull(bounds);
      Assert.NotNull(bounds.Value.LowerBounds);
      Assert.NotNull(bounds.Value.UpperBounds);
      Assert.Equal(1.0, bounds.Value.LowerBounds![0]);
      Assert.Equal(5.0, bounds.Value.UpperBounds![0]);
      Assert.Equal(2.0, bounds.Value.LowerBounds![1]);
      Assert.Equal(7.0, bounds.Value.UpperBounds![1]);
    }

    /// <summary>
    /// Verifies that repeated simple bounds are merged to the tightest lower and upper values.
    /// </summary>
    [Fact]
    public void TryConvertToBoundaryConstraints_KeepsTightestBounds()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1 },
                { 1 },
                { -1 },
                { -1 },
      });
      var d = TestHelpers.V(5.0, 3.0, -1.0, -2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var bounds = proj.TryConvertToBoundaryConstraints();

      Assert.NotNull(bounds);
      Assert.NotNull(bounds.Value.LowerBounds);
      Assert.NotNull(bounds.Value.UpperBounds);
      Assert.Equal(2.0, bounds.Value.LowerBounds![0]);
      Assert.Equal(3.0, bounds.Value.UpperBounds![0]);
    }

    /// <summary>
    /// Verifies that boundary conversion uses external parameter indices after fixed parameters have been eliminated internally.
    /// </summary>
    [Fact]
    public void TryConvertToBoundaryConstraints_UsesExternalParameterIndicesAfterReduction()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 0, 0 },
      });
      var b = TestHelpers.V(4.0);
      var C = TestHelpers.M(new double[,]
      {
                { 0, 1, 0 },
                { 0, -1, 0 },
      });
      var d = TestHelpers.V(6.0, -2.0);
      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      var bounds = proj.TryConvertToBoundaryConstraints();

      Assert.NotNull(bounds);
      Assert.NotNull(bounds.Value.LowerBounds);
      Assert.NotNull(bounds.Value.UpperBounds);
      Assert.Equal(3, proj.NumberOfParameters);
      Assert.Null(bounds.Value.LowerBounds![0]);
      Assert.Null(bounds.Value.UpperBounds![0]);
      Assert.Equal(2.0, bounds.Value.LowerBounds![1]);
      Assert.Equal(6.0, bounds.Value.UpperBounds![1]);
      Assert.Null(bounds.Value.LowerBounds![2]);
      Assert.Null(bounds.Value.UpperBounds![2]);
    }

    /// <summary>
    /// Verifies that boundary conversion returns <see langword="null"/> for relational inequalities that are not simple box bounds.
    /// </summary>
    [Fact]
    public void TryConvertToBoundaryConstraints_NonBoxConstraint_ReturnsNull()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1, -1 },
      });
      var d = TestHelpers.V(0.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var bounds = proj.TryConvertToBoundaryConstraints();

      Assert.Null(bounds);
    }

    /// <summary>
    /// Verifies that equivalent inequality constraints keep only the tightest bound.
    /// </summary>
    [Fact]
    public void EquivalentInequalityConstraints_KeepTightestBound()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1, 0 },
                { 1, 0 },
      });
      var d = TestHelpers.V(1.0, 2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(3.0, 4.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.0, 4.0));
      Assert.Single(result.ActiveConstraintIndices);
      Assert.Contains(0, result.ActiveConstraintIndices);
    }

    /// <summary>
    /// Verifies that positively proportional inequality constraints are simplified using the tightest normalized bound.
    /// </summary>
    [Fact]
    public void ProportionalInequalityConstraints_KeepTightestBound()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1, 0 },
                { 2, 0 },
      });
      var d = TestHelpers.V(1.0, 3.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(3.0, 4.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.0, 4.0));
      Assert.Single(result.ActiveConstraintIndices);
      Assert.Contains(0, result.ActiveConstraintIndices);
    }

    // ── Mixed equality + inequality ───────────────────────────────────────

    /// <summary>
    /// Verifies that equality and inequality constraints are both enforced during projection.
    /// </summary>
    [Fact]
    public void EqualityAndInequality_BothEnforced()
    {
      // x[0] + x[1] = 1  and  x[0] <= 0.3
      // Point (0.8, 0.2): equality violated (0.8+0.2=1 ✓ actually), x[0] > 0.3
      // Expected: x[0]=0.3, x[1]=0.7  (on equality plane and at ineq boundary)
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(0.3);
      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.8, 0.2));

      Assert.Equal(1.0, result.Parameters[0] + result.Parameters[1], precision: 6);
      Assert.True(result.Parameters[0] <= 0.3 + 1e-7);
    }

    /// <summary>
    /// Verifies that inequality-only projection finds a feasible start for multi-constraint systems instead of relying on greedy snapping.
    /// </summary>
    [Fact]
    public void InequalityOnly_MultipleConstraints_UsesRobustFeasibleStart()
    {
      var C = TestHelpers.M(new double[,]
      {
                { -1, 0 },
                { 0, -1 },
                { -1, -1 },
      });
      var d = TestHelpers.V(0.0, 0.0, -2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(-5.0, -5.0));

      Assert.True(result.Parameters[0] >= -TestHelpers.Tol);
      Assert.True(result.Parameters[1] >= -TestHelpers.Tol);
      Assert.True(result.Parameters[0] + result.Parameters[1] >= 2.0 - TestHelpers.Tol);
    }

    /// <summary>
    /// Verifies that mixed equality and inequality projection can recover from an infeasible starting point by constructing a feasible start in the equality null space.
    /// </summary>
    [Fact]
    public void EqualityAndInequality_InfeasibleStart_UsesNullSpaceFeasibleStart()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(2.0);
      var C = TestHelpers.M(new double[,]
      {
                { -1, 0 },
                { 0, -1 },
      });
      var d = TestHelpers.V(0.0, 0.0);
      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(-3.0, 5.0));

      Assert.Equal(2.0, result.Parameters[0] + result.Parameters[1], precision: 7);
      Assert.True(result.Parameters[0] >= -TestHelpers.Tol);
      Assert.True(result.Parameters[1] >= -TestHelpers.Tol);
    }

    // ── IsFeasible ────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that <see cref="LinearConstraintsProjector.IsFeasible(Vector{double}, double)"/> returns <see langword="true"/> for a feasible point.
    /// </summary>
    [Fact]
    public void IsFeasible_FeasiblePoint_ReturnsTrue()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(0.5);
      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      Assert.True(proj.IsFeasible(TestHelpers.V(0.3, 0.7)));
    }

    /// <summary>
    /// Verifies that <see cref="LinearConstraintsProjector.IsFeasible(Vector{double}, double)"/> returns <see langword="false"/> when an equality constraint is violated.
    /// </summary>
    [Fact]
    public void IsFeasible_EqualityViolated_ReturnsFalse()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      Assert.False(proj.IsFeasible(TestHelpers.V(0.3, 0.3)));
    }

    /// <summary>
    /// Verifies that <see cref="LinearConstraintsProjector.IsFeasible(Vector{double}, double)"/> returns <see langword="false"/> when an inequality constraint is violated.
    /// </summary>
    [Fact]
    public void IsFeasible_InequalityViolated_ReturnsFalse()
    {
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      Assert.False(proj.IsFeasible(TestHelpers.V(5.0, 1.0)));
    }

    // ── Projection is idempotent ──────────────────────────────────────────

    /// <summary>
    /// Verifies that projecting an already projected point returns the same point.
    /// </summary>
    [Fact]
    public void ProjectTwice_GivesSameResult()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1, 0 } });
      var b = TestHelpers.V(1.0);
      var C = TestHelpers.M(new double[,] { { 0, 0, 1 } });
      var d = TestHelpers.V(0.5);
      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      var p = TestHelpers.V(0.2, 0.2, 0.9);
      var first = proj.Project(p);
      var second = proj.Project(first);

      TestHelpers.AssertVector(second, first);
    }

    // ── Projected point is nearest feasible point ─────────────────────────

    /// <summary>
    /// Verifies that the projection is the nearest feasible point for a simple inequality constraint.
    /// </summary>
    [Fact]
    public void Projection_IsNearestFeasiblePoint_SimpleInequality()
    {
      // x[0] <= 2 — nearest point on boundary to (5, 1) is (2, 1)
      // Distance = 3. Any other feasible point is further away.
      var C = TestHelpers.M(new double[,] { { 1, 0 } });
      var d = TestHelpers.V(2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var p = TestHelpers.V(5.0, 1.0);
      var result = proj.Project(p);

      double dist = (result - p).L2Norm();

      // Check a few other feasible points are all further away
      var candidates = new[]
      {
                TestHelpers.V(0.0, 1.0),
                TestHelpers.V(1.0, 1.0),
                TestHelpers.V(2.0, 0.0),
                TestHelpers.V(2.0, 2.0),
            };
      foreach (var c in candidates)
        Assert.True((c - p).L2Norm() >= dist - TestHelpers.Tol);
    }

    // ── Active constraint indices ─────────────────────────────────────────

    /// <summary>
    /// Verifies that equality constraints are always reported as active.
    /// </summary>
    [Fact]
    public void ActiveConstraintIndices_EqualityAlwaysPresent()
    {
      var A = TestHelpers.M(new double[,] { { 1, 1 } });
      var b = TestHelpers.V(1.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.3, 0.3));

      // Equality is row 0 in the merged system, always active
      Assert.Contains(0, result.ActiveConstraintIndices);
    }

    /// <summary>
    /// Verifies that parameters participating in active relational inequalities are marked as constrained.
    /// </summary>
    [Fact]
    public void ActiveConstraintIndices_InequalityOnlyWhenBinding()
    {
      // Two inequality constraints: x[0] <= 2, x[1] <= 2
      var C = TestHelpers.M(new double[,]
      {
                { 1, 0 },
                { 0, 1 },
      });
      var d = TestHelpers.V(2.0, 2.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      // Point (5, 1): only x[0] constraint is active
      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5.0, 1.0));

      // Inequality rows are offset by nEq=0: row 0 = x[0]<=2, row 1 = x[1]<=2
      Assert.Contains(0, result.ActiveConstraintIndices);    // x[0] bound hit
      Assert.DoesNotContain(1, result.ActiveConstraintIndices); // x[1] bound not hit
    }

    /// <summary>
    /// Verifies that inequality constraints are reported as active only when they are binding.
    /// </summary>
    [Fact]
    public void Two_InequalityConstraints()
    {
      // Two inequality constraints: x[0] >= x[1], x[1] >= 1
      var C = TestHelpers.M(new double[,]
      {
                { -1, 1 },
                { 0, -1 },
      });
      var d = TestHelpers.V(0, -1);
      var proj = new LinearConstraintsProjector(C: C, d: d);
      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0, 1.0));


      Assert.Equal(1, result.Parameters[0], 1E-6);
      Assert.True(result.IsConstrained[0]);
      Assert.Equal(1, result.Parameters[1], 1E-6);
      Assert.True(result.IsConstrained[1]);
    }

    /// <summary>
    /// Verifies that parameters participating in active relational inequalities are marked as constrained.
    /// </summary>
    [Fact]
    public void Two_InequalityConstraints_3Parameters()
    {
      // Two inequality constraints: x[0] >= x[2], x[2] >= 1
      var C = TestHelpers.M(new double[,]
      {
                { -1, 0, 1 },
                { 0, 0, -1 },
      });
      var d = TestHelpers.V(0, -1);
      var proj = new LinearConstraintsProjector(C: C, d: d);
      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0, 0, 1));


      Assert.Equal(1, result.Parameters[0], 1E-6);
      Assert.True(result.IsConstrained[0]);

      Assert.Equal(0, result.Parameters[1], 0);
      Assert.False(result.IsConstrained[1]);


      Assert.Equal(1, result.Parameters[2], 1E-6);
      Assert.True(result.IsConstrained[2]);
    }

    /// <summary>
    /// Two incompatible inequality constraints, one parameter.
    /// </summary>
    [Fact]
    public void Two_JustCompatibleInequalityConstraints_1Parameter()
    {
      // Two incompatible constraints: x[0] <= 2, x[0] >= 2
      var C = TestHelpers.M(new double[,]
      {
                { 1 },
                { -1, },
      });
      var d = TestHelpers.V(2, -2);

      // the construction should already throw an exception
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0));
      double fixedValue = result.FixedValues[0] ?? throw new InvalidOperationException();
      double projectorFixedValue = proj.ValuesFixedByConstraints[0] ?? throw new InvalidOperationException();

      Assert.Equal(2, result.Parameters[0], 0);
      Assert.True(result.IsConstrained[0]);
      Assert.True(result.FixedValues[0].HasValue);
      Assert.Equal(2.0, fixedValue);
      Assert.Equal(new[] { 0 }, result.FixedParameterIndices);
      Assert.Equal(2.0, projectorFixedValue);
    }

    /// <summary>
    /// Verifies that a single equality constraint fixes the corresponding parameter throughout the feasible set.
    /// </summary>
    [Fact]
    public void EqualityConstraint_FixedParameter_IsReported()
    {
      var A = TestHelpers.M(new double[,] { { 1, 0 } });
      var b = TestHelpers.V(3.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.0, 5.0));
      double fixedValue = result.FixedValues[0] ?? throw new InvalidOperationException();
      double projectorFixedValue = proj.ValuesFixedByConstraints[0] ?? throw new InvalidOperationException();

      Assert.Equal(3.0, result.Parameters[0], precision: 7);
      Assert.True(result.FixedValues[0].HasValue);
      Assert.False(result.FixedValues[1].HasValue);
      Assert.Equal(3.0, fixedValue, precision: 7);
      Assert.Null(result.FixedValues[1]);
      Assert.Equal(new[] { 0 }, result.FixedParameterIndices);
      Assert.Equal(3.0, projectorFixedValue, precision: 7);
      Assert.Null(proj.ValuesFixedByConstraints[1]);
    }

    /// <summary>
    /// Two incompatible inequality constraints, one parameter.
    /// </summary>
    [Fact]
    public void Two_IncompatibleInequalityConstraints_1Parameter()
    {
      // Two incompatible constraints: x[0] <= -1, x[0] >= 1
      var C = TestHelpers.M(new double[,]
      {
                { 1 },
                { -1, },
      });
      var d = TestHelpers.V(-1, -1);

      // the construction should already throw an exception, because the two constraints are incompatible
      Assert.Throws<InvalidDataException>(() => new LinearConstraintsProjector(C: C, d: d));
    }


    /// <summary>
    /// Two incompatible inequality constraints, one parameter.
    /// </summary>
    [Fact]
    public void Two_IncompatibleConstraints_1Parameter()
    {
      // Two incompatible constraints:
      // one equality constraint: x[0] == 2
      // one inequality constraint: x[0] <= 1
      // thus the two constraints are incompatible, no feasible point exists.

      var A = TestHelpers.M(new double[,]
        {
                { 1 },
        });
      var b = TestHelpers.V(2);


      var C = TestHelpers.M(new double[,]
      {
                { 1 },
      });
      var d = TestHelpers.V(1);

      // the construction should already throw an exception, because the two constraints are incompatible
      Assert.Throws<InvalidDataException>(() => new LinearConstraintsProjector(A: A, b: b, C: C, d: d));
    }

    /// <summary>
    /// Two incompatible inequality constraints, two parameters.
    /// </summary>
    [Fact]
    public void Three_IncompatibleConstraints_2Parameter()
    {
      // Now we have three incompatible constraints:
      // One equality constraint: x[0] + x[1] == 1
      // Two inequality constraints: x[0] >= 0.6, x[1] >= 0.6
      // Thus x[0] + x[1] >= 1.2, which contradicts the equality constraint. No feasible point exists.

      var A = TestHelpers.M(new double[,]
        {
                { 1, 1 },
        });
      var b = TestHelpers.V(1);


      var C = TestHelpers.M(new double[,]
      {
                { -1, 0 },
                { 0, -1 },
      });
      var d = TestHelpers.V(-0.6, -0.6);

      // the construction should already throw an exception, because the two constraints are incompatible
      Assert.Throws<InvalidDataException>(() => new LinearConstraintsProjector(A: A, b: b, C: C, d: d));
    }

    /// <summary>
    /// Two incompatible inequality constraints, two parameters.
    /// </summary>
    [Fact]
    public void Two_IncompatibleEqualityConstraints_2Parameter()
    {
      // Now we have two incompatible equality constraints:
      // First equality constraint: x[0] + x[1] == 1
      // Second constraints: 3*x[0] + 3*x[1] == 2
      // Thus no feasible point exists.

      var A = TestHelpers.M(new double[,]
        {
                { 1, 1 },
                {3, 3 }
        });
      var b = TestHelpers.V(1, 2);

      // the construction should already throw an exception, because the two constraints are incompatible
      Assert.Throws<InvalidDataException>(() => new LinearConstraintsProjector(A: A, b: b));
    }


    /// <summary>
    /// Six just compatible constraints, three parameters, leading to all parameters fixed.
    /// </summary>
    [Fact]
    public void Six_JustCompatibleConstraints_3Parameter_LeadingToAllParametersFixed01()
    {
      // We have six inequality constraints, that lead to fixed parameters x[0]=2, x[1]=3, x[2]=5:
      const int NumberOfParameters = 3;

      var C = TestHelpers.M(new double[,]
        {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
                { -1, 0, 0 },
                { 0, -1, 0 },
                { 0, 0, -1 },
        });
      var d = TestHelpers.V(2, 3, 5, -2, -3, -5);

      // The paired upper and lower bounds force a singleton feasible point.
      var proj = new LinearConstraintsProjector(C: C, d: d);

      // rule: The proj.ValuesFixedByConstraints should either be null (not fixed) or they should contain a value that is not NaN.
      Assert.Equal(NumberOfParameters, proj.ValuesFixedByConstraints.Count);
      for (int i = 0; i < NumberOfParameters; i++)
      {
        Assert.True(proj.ValuesFixedByConstraints[i] is null || !double.IsNaN(proj.ValuesFixedByConstraints[i].Value));
      }

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0, 0, 0));

      Assert.True(proj.IsFeasible(result.Parameters));
      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(2, 3, 5));


      Assert.True(proj.ValuesFixedByConstraints[0].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[0].Value, 2, precision: 7);

      Assert.True(proj.ValuesFixedByConstraints[1].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[1].Value, 3, precision: 7);

      Assert.True(proj.ValuesFixedByConstraints[2].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[2].Value, 5, precision: 7);
    }

    /// <summary>
    /// Six just compatible constraints, three parameters, leading to all parameters fixed.
    /// </summary>
    [Fact]
    public void Six_JustCompatibleConstraints_3Parameter_LeadingToAllParametersFixed02()
    {
      // We have six inequality constraints, that lead to fixed parameters x[0]=2, x[1]=3, x[2]=5:
      const int NumberOfParameters = 3;

      var C = TestHelpers.M(new double[,]
        {
                { 1, 2, 2 },
                { 2, 1, -2 },
                { 2, -2, 1 },
                { -1, -2, -2 },
                { -2, -1, 2 },
                { -2, 2, -1 },
        });
      var d = TestHelpers.V(18, -3, 3, -18, 3, -3);

      // The paired upper and lower bounds force a singleton feasible point.
      var proj = new LinearConstraintsProjector(C: C, d: d);

      // rule: The proj.ValuesFixedByConstraints should either be null (not fixed) or they should contain a value that is not NaN.
      Assert.Equal(NumberOfParameters, proj.ValuesFixedByConstraints.Count);
      for (int i = 0; i < NumberOfParameters; i++)
      {
        Assert.True(proj.ValuesFixedByConstraints[i] is null || !double.IsNaN(proj.ValuesFixedByConstraints[i].Value));
      }

      Assert.True(proj.ValuesFixedByConstraints[0].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[0].Value, 2, precision: 7);

      Assert.True(proj.ValuesFixedByConstraints[1].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[1].Value, 3, precision: 7);

      Assert.True(proj.ValuesFixedByConstraints[2].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[2].Value, 5, precision: 7);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0, 0, 0));

      Assert.True(proj.IsFeasible(result.Parameters));
      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(2, 3, 5));
    }

    /// <summary>
    /// The three simple fixed equality constraints must reproduce exactly the fixed parameter values.
    /// </summary>
    [Fact]
    public void TreeSimpleFixedConstraintsPlusToEqualityConstraints_TestForPreciseEquality()
    {
      // We have three fixed equality constraints, that lead to fixed parameters x[0]=2, x[1]=3, x[2]=5:
      var (A, b) = TestHelpers.MV(new double[][]
        {
               [ 1, 0, 0, 0, 0 ], [201],
               [ 0, 1, 0, 0, 0 ], [303],
               [ 0, 0, 1, 0, 0 ], [538],
               [ 0, 0, 0, 1, 1 ], [438],
               [ 0, 0, 0, 1, -1 ], [2],
        });

      // The paired upper and lower bounds force a singleton feasible point.
      var proj = new LinearConstraintsProjector(A: A, b: b);

      // rule: The proj.ValuesFixedByConstraints should either be null (not fixed) or they should contain a value that is not NaN.
      Assert.Equal(5, proj.ValuesFixedByConstraints.Count);
      Assert.True(proj.ValuesFixedByConstraints[0].HasValue);
      Assert.Equal(201, proj.ValuesFixedByConstraints[0].Value); // has to be exactly 201

      Assert.True(proj.ValuesFixedByConstraints[1].HasValue);
      Assert.Equal(303, proj.ValuesFixedByConstraints[1].Value);  // has to be exactly 303

      Assert.True(proj.ValuesFixedByConstraints[2].HasValue);
      Assert.Equal(538, proj.ValuesFixedByConstraints[2].Value); // has to be exactly 538

      Assert.True(proj.ValuesFixedByConstraints[3].HasValue);
      Assert.Equal(220, proj.ValuesFixedByConstraints[3].Value, precision: 12); // we allow some  numerical error here

      Assert.True(proj.ValuesFixedByConstraints[4].HasValue);
      Assert.Equal(218, proj.ValuesFixedByConstraints[4].Value, precision: 12); // we allow some  numerical error here

    }


    /// <summary>
    /// Six just compatible constraints, three parameters, leading to all parameters fixed.
    /// </summary>
    [Fact]
    public void Six_CompatibleConstraints_3Parameter_LeadingToOneDOF()
    {
      // We have six inequality constraints, leading to x[1] = 7 - 2 x[0] and x[2] == (83 - 7 x[0] - 8 x[1])/9
      const int NumberOfParameters = 3;

      var (C, d) = TestHelpers.MV(
       [
                [ 1, 2, 3 ], [23],
                [ 4, 5, 6 ], [53],
                [ 7, 8, 9 ], [83],
                [ -2, -4, -6 ], [-46],
                [ -8, -10, -12 ], [-106],
                [ -14, -16, -18 ], [-166]
        ]);

      // The paired upper and lower bounds force a singleton feasible point.
      var proj = new LinearConstraintsProjector(C: C, d: d);

      // rule: The proj.ValuesFixedByConstraints should either be null (not fixed) or they should contain a value that is not NaN.
      Assert.Equal(NumberOfParameters, proj.ValuesFixedByConstraints.Count);
      for (int i = 0; i < NumberOfParameters; i++)
      {
        Assert.True(proj.ValuesFixedByConstraints[i] is null || !double.IsNaN(proj.ValuesFixedByConstraints[i].Value));
      }

      Assert.False(proj.ValuesFixedByConstraints[0].HasValue);
      Assert.False(proj.ValuesFixedByConstraints[1].HasValue);
      Assert.False(proj.ValuesFixedByConstraints[2].HasValue);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0, 0, 0));
      Assert.True(proj.IsFeasible(result.Parameters));
      Assert.Equal(7, 2 * result.Parameters[0] + result.Parameters[1], precision: 7);
      Assert.Equal(83, 7 * result.Parameters[0] + 8 * result.Parameters[1] + 9 * result.Parameters[2], precision: 7);
    }

    /// <summary>
    /// Six just compatible constraints, three parameters, leading to all parameters fixed.
    /// </summary>
    [Fact]
    public void Six_CompatibleConstraints_OneEquality_3Parameter_LeadingToZeroDOF()
    {
      // We have six inequality constraints, leading to x[1] = 7 - 2 x[0] and x[2] == (83 - 7 x[0] - 8 x[1])/9
      // And one equality constraint x[0] == 1, leading to x[1] = 5 and x[2] = 4
      const int NumberOfParameters = 3;

      var (C, d) = TestHelpers.MV(
       [
                [ 1, 2, 3 ], [23],
                [ 4, 5, 6 ], [53],
                [ 7, 8, 9 ], [83],
                [ -2, -4, -6 ], [-46],
                [ -8, -10, -12 ], [-106],
                [ -14, -16, -18 ], [-166]
        ]);

      var (A, b) = TestHelpers.MV(
        [

                [ 1, 0, 0 ], [1]
        ]);

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);
      // rule: The proj.ValuesFixedByConstraints should either be null (not fixed) or they should contain a value that is not NaN.
      Assert.Equal(NumberOfParameters, proj.ValuesFixedByConstraints.Count);
      for (int i = 0; i < NumberOfParameters; i++)
      {
        Assert.True(proj.ValuesFixedByConstraints[i] is null || !double.IsNaN(proj.ValuesFixedByConstraints[i].Value));
      }

      Assert.True(proj.ValuesFixedByConstraints[0].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[0].Value, 1, precision: 7);

      Assert.True(proj.ValuesFixedByConstraints[1].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[1].Value, 5, precision: 7);

      Assert.True(proj.ValuesFixedByConstraints[2].HasValue);
      Assert.Equal(proj.ValuesFixedByConstraints[2].Value, 4, precision: 7);

    }

    /// <summary>
    /// Six just compatible constraints, three parameters, leading to all parameters fixed.
    /// </summary>
    [Fact]
    public void Six_IncompatibleConstraints_3Parameter()
    {
      // We have six inequality constraints, leading to x[1] = 7 - 2 x[0] and x[2] == (83 - 7 x[0] - 8 x[1])/9

      var C = TestHelpers.M(new double[,]
        {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
                { -2, -4, -6 },
                { -8, -10, -12 },
                { -14, -16, -18 },
        });
      var d = TestHelpers.V(23, 53, 83, -46, -106, -167);

      // The paired upper and lower bounds force a singleton feasible point.
      Assert.Throws<InvalidDataException>(() => new LinearConstraintsProjector(C: C, d: d));

    }

    /// <summary>
    /// 4 parameters, 2 fixed by equality constraints, 3 inequality constraints.
    /// </summary>
    [Fact]
    public void FourParameters_2Fixed_3InequalityConstraints()
    {
      // We have 4 Parameters:
      // x[0] >=0, x[1] is fixed to 0.0004, x[2] >=0, x[2] >= x[0], and x[3] is fixed to 0.0003

      var A = TestHelpers.M(new double[,]
  {
                { 0, 1, 0, 0 },   // x[1] == 0.0004
                { 0, 0, 0, 1} // x[3] == 0.0003
  });
      var b = TestHelpers.V(0.0004, 0.0003);


      var C = TestHelpers.M(new double[,]
        {
                {  1, 0, -1, 0 }, // x[2] >= x[0]  expressed as  x[0] - x[2] <= 0
                { -1, 0,  0, 0 }, // x[0] >= 0  expressed as  -x[0] <= 0
                {  0, 0, -1, 0} // x[2] >= 0  expressed as  -x[2] <= 0
        });
      var d = TestHelpers.V(0, 0, 0);

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);

      Assert.Null(proj.ValuesFixedByConstraints[0]);
      Assert.Equal(0.0004, proj.ValuesFixedByConstraints[1]);
      Assert.Null(proj.ValuesFixedByConstraints[2]);
      Assert.Equal(0.0003, proj.ValuesFixedByConstraints[3]);

      Assert.True(proj.IsFeasible(TestHelpers.V(0, 0.0004, 0, 0.0003)));
    }

    /// <summary>
    /// Verifies that parameters fixed by equality constraints remain fixed after column elimination.
    /// </summary>
    [Fact]
    public void EqualityFixedParameter_IsHandledAfterColumnElimination()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 0, 0 },
                { 0, 1, 1 },
      });
      var b = TestHelpers.V(3.0, 4.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.0, 0.0, 0.0));

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(3.0, 2.0, 2.0));
      Assert.Equal(3.0, proj.ValuesFixedByConstraints[0]);
      Assert.Null(proj.ValuesFixedByConstraints[1]);
      Assert.Null(proj.ValuesFixedByConstraints[2]);
      Assert.True(proj.IsFeasible(result.Parameters));
      Assert.False(proj.IsFeasible(TestHelpers.V(2.5, 2.0, 2.0)));
    }

    /// <summary>
    /// Verifies that parameters fixed after eliminating equality-fixed columns are also removed from the inequality system.
    /// </summary>
    [Fact]
    public void InequalityFixedParameter_IsHandledAfterEqualityColumnElimination()
    {
      var (A, b) = TestHelpers.MV(
      [
                [ 1, 0, 0 ], [3] // x[0] == 3
      ]);



      var (C, d) = TestHelpers.MV(
      [
                [ 0, 1, 0 ],  [2], // x[1] <= 2
                [ 0, -1, 0 ], [-2], // x[1] >= 2
                [ 0, 0, 1 ],  [5], // x[2] <= 5
      ]);

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);


      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(0.0, 10.0, 7.0));
      Assert.Equal(3.0, proj.ValuesFixedByConstraints[0]);
      Assert.Equal(2.0, proj.ValuesFixedByConstraints[1]);
      Assert.Null(proj.ValuesFixedByConstraints[2]);

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(3.0, 2.0, 5.0));
      Assert.True(proj.IsFeasible(result.Parameters));
      Assert.False(proj.IsFeasible(TestHelpers.V(3.0, 1.5, 5.0)));
    }

    /// <summary>
    /// Verifies that parameters not referenced by any constraint are preserved during projection.
    /// </summary>
    [Fact]
    public void FullyFreeParameter_IsPreservedDuringProjection()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 1, 0 },
      });
      var b = TestHelpers.V(3.0);
      var proj = new LinearConstraintsProjector(A: A, b: b);

      var p = TestHelpers.V(0.0, 0.0, 7.5);
      var result = ProjectWithCheckedProperties(proj, p);

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(1.5, 1.5, 7.5));
      Assert.False(result.IsConstrained[2]);
      Assert.Contains(2, result.FreeParameterIndices);
      Assert.True(proj.IsFeasible(result.Parameters));
    }

    /// <summary>
    /// Verifies that parameters not referenced by any equality or inequality remain unchanged during projection.
    /// </summary>
    [Fact]
    public void FullyFreeParameter_WithInequalities_IsPreservedDuringProjection()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1, 0, 0 },
                { 0, 1, 0 },
      });
      var d = TestHelpers.V(2.0, 3.0);
      var proj = new LinearConstraintsProjector(C: C, d: d);

      var p = TestHelpers.V(5.0, 7.0, -4.0);
      var result = ProjectWithCheckedProperties(proj, p);

      TestHelpers.AssertVector(result.Parameters, TestHelpers.V(2.0, 3.0, -4.0));
      Assert.False(result.IsConstrained[2]);
      Assert.Contains(2, result.FreeParameterIndices);
      Assert.True(proj.IsFeasible(result.Parameters));
    }

    /// <summary>
    /// Verifies that normalization can be invoked directly through the internal helper and promotes tight opposing inequalities.
    /// </summary>
    [Fact]
    public void NormalizeConstraints_InternalHelper_PromotesTightOpposingInequalities()
    {
      var C = TestHelpers.M(new double[,]
      {
                { 1, 0 },
                { -1, 0 },
      });
      var d = TestHelpers.V(2.0, -2.0);

      var (A, b, normalizedC, normalizedD) = LinearConstraintsProjector.NormalizeConstraints(null, null, C, d);

      Assert.NotNull(A);
      Assert.NotNull(b);
      Assert.Single(b!);
      Assert.Equal(2, A!.ColumnCount);
      Assert.Equal(1.0, A[0, 0]);
      Assert.Equal(0.0, A[0, 1]);
      Assert.Equal(2.0, b[0]);
      Assert.Null(normalizedC);
      Assert.Null(normalizedD);
    }

    /// <summary>
    /// Verifies that parameters fixed by equality constraints can be detected and eliminated through the combined internal helper.
    /// </summary>
    [Fact]
    public void EliminateParametersFixedByEqualities_InternalHelper_ReducesMatricesAndUpdatesMapping()
    {
      var A = TestHelpers.M(new double[,]
      {
                { 1, 0, 0 },
                { 0, 1, 1 },
      });
      var b = TestHelpers.V(2.0, 10.0);
      var C = TestHelpers.M(new double[,]
      {
                { 0, -1, 0 },
      });
      var d = TestHelpers.V(-1.0);
      var valuesFixedByConstraints = new double?[4];
      var columnIndexToExternalIndex = new[] { 1, 2, 3 };

      var (reducedA, reducedB, reducedC, reducedD, updatedFixedValues, updatedColumnIndexToExternalIndex) = LinearConstraintsProjector.EliminateParametersFixedByEqualities(
        A,
        b,
        C,
        d,
        valuesFixedByConstraints,
        columnIndexToExternalIndex);

      Assert.NotNull(reducedA);
      Assert.NotNull(reducedB);
      Assert.NotNull(reducedC);
      Assert.NotNull(reducedD);
      Assert.Equal(new[] { 2, 3 }, updatedColumnIndexToExternalIndex);
      Assert.Null(updatedFixedValues[0]);
      Assert.Equal(2.0, updatedFixedValues[1]);
      Assert.Null(updatedFixedValues[2]);
      Assert.Null(updatedFixedValues[3]);

      Assert.Equal(2, reducedA!.RowCount);
      Assert.Equal(2, reducedA.ColumnCount);
      Assert.Equal(0.0, reducedA[0, 0]);
      Assert.Equal(0.0, reducedA[0, 1]);
      Assert.Equal(1.0, reducedA[1, 0]);
      Assert.Equal(1.0, reducedA[1, 1]);
      TestHelpers.AssertVector(reducedB!, TestHelpers.V(0.0, 10.0));

      Assert.Equal(1, reducedC!.RowCount);
      Assert.Equal(2, reducedC.ColumnCount);
      Assert.Equal(-1.0, reducedC[0, 0]);
      Assert.Equal(0.0, reducedC[0, 1]);
      TestHelpers.AssertVector(reducedD!, TestHelpers.V(-1.0));
    }


    /// <summary>
    /// Tests a linear constraint projector with ten parameters, five fully free, four equality constraints, and five
    /// inequality constraints.
    /// </summary>
    /// <remarks>Verifies the correct identification of free and constrained parameters, and checks that the
    /// projection satisfies all constraints and expected parameter relationships.</remarks>
    [Fact]
    public void TenParameters_5FullyFree_4Equality_5InequalityConstraints()
    {
      // We have 10 parameters, of which 5 are fully free, 4 are fixed by equality constraints, and 5 are constrained by inequalities.

      var (A, b) = TestHelpers.MV(new double[][]
      {
      [1, 0, -1, 0,  0, 0,  0, 0,  0, 0],  [0], // x[2] == x[0]
      [1, 0,  0, 0, -1, 0,  0, 0,  0, 0],  [0], // x[4] == x[0]
      [1, 0,  0, 0,  0, 0, -1, 0,  0, 0],  [0], // x[6] == x[0]
      [1, 0,  0, 0,  0, 0,  0, 0, -1, 0],  [0], // x[8] == x[0]
      }
      );

      var (C, d) = TestHelpers.MV(new double[][]
      {
      [-1, 0, 0, 0, 0, 0, 0, 0, 0, 0], [0], // x[0] >= 0
      [0, 0, -1, 0, 0, 0, 0, 0, 0, 0], [0], // x[2] >= 0
      [0, 0, 0, 0, -1, 0, 0, 0, 0, 0], [0], // x[4] >= 0
      [0, 0, 0, 0, 0, 0, -1, 0, 0, 0], [0], // x[6] >= 0
      [0, 0, 0, 0, 0, 0, 0, 0, -1, 0], [0], // x[8] >= 0
      }
      );

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);
      Assert.Equal(10, proj.NumberOfParameters);
      Assert.Equal(5, proj.NumberOfParametersFullyFree);
      Assert.Equal(0, proj.NumberOfParametersFixed);
      Assert.Equal(10, proj.NumberOfParametersThatCanVary);
      Assert.False(proj.AreAllParametersFullyFree);
      Assert.False(proj.AreAllParametersFixed);


      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5, 7, 9, 11, 13, 17, 19, 23, 29, 31));
      Assert.True(proj.IsFeasible(result.Parameters));

      Assert.Equal(7, result.Parameters[1]);
      Assert.Equal(11, result.Parameters[3]);
      Assert.Equal(17, result.Parameters[5]);
      Assert.Equal(23, result.Parameters[7]);
      Assert.Equal(31, result.Parameters[9]);


      Assert.Equal(result.Parameters[0], result.Parameters[2], precision: 7);
      Assert.Equal(result.Parameters[0], result.Parameters[4], precision: 7);
      Assert.Equal(result.Parameters[0], result.Parameters[6], precision: 7);
      Assert.Equal(result.Parameters[0], result.Parameters[8], precision: 7);
    }

    /// <summary>
    /// Tests a linear constraint projector with ten parameters, five fully free, four equality constraints, and nine
    /// inequality constraints.
    /// </summary>
    /// <remarks>Verifies that the projector correctly identifies parameter properties and projects a sample
    /// vector to satisfy all constraints, including equality and inequality relationships among parameters.</remarks>
    [Fact]
    public void TenParameters_5FullyFree_4Equality_9InequalityConstraints()
    {
      // We have 10 parameters, of which 5 are fully free, 4 are fixed by equality constraints, and 5 are constrained by inequalities.

      var (A, b) = TestHelpers.MV(new double[][]
      {
      [0.9, 0, -1, 0,  0, 0,  0, 0,  0, 0],  [0], // x[2] == 0.9 * x[0]
      [0.8, 0,  0, 0, -1, 0,  0, 0,  0, 0],  [0], // x[4] == 0.8 * x[0]
      [0.7, 0,  0, 0,  0, 0, -1, 0,  0, 0],  [0], // x[6] == 0.7 * x[0]
      [0.6, 0,  0, 0,  0, 0,  0, 0, -1, 0],  [0], // x[8] == 0.6 * x[0]
      }
      );

      var (C, d) = TestHelpers.MV(new double[][]
      {
      [-1, 0, 0, 0, 0, 0, 0, 0, 0, 0], [0], // x[0] >= 0
      [0, 0, -1, 0, 0, 0, 0, 0, 0, 0], [0], // x[2] >= 0
      [0, 0, 0, 0, -1, 0, 0, 0, 0, 0], [0], // x[4] >= 0
      [0, 0, 0, 0, 0, 0, -1, 0, 0, 0], [0], // x[6] >= 0
      [0, 0, 0, 0, 0, 0, 0, 0, -1, 0], [0], // x[8] >= 0
      [0, 1, 0, -1, 0, 0, 0, 0, 0, 0], [0], // x[3] >= x[1]  expressed as  x[1] - x[3] <= 0
      [0, 0, 0, 1, 0, -1, 0, 0, 0, 0], [0], // x[5] >= x[3]  expressed as  x[3] - x[5] <= 0
      [0, 0, 0, 0, 0, 1, 0, -1, 0, 0], [0], // x[7] >= x[5]  expressed as  x[5] - x[7] <= 0
      [0, 0, 0, 0, 0, 0, 0,  1, 0,-1], [0], // x[9] >= x[7]  expressed as  x[7] - x[9] <= 0
      }
      );

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);
      Assert.Equal(10, proj.NumberOfParameters);
      Assert.Equal(0, proj.NumberOfParametersFullyFree);
      Assert.Equal(0, proj.NumberOfParametersFixed);
      Assert.Equal(10, proj.NumberOfParametersThatCanVary);
      Assert.False(proj.AreAllParametersFullyFree);
      Assert.False(proj.AreAllParametersFixed);


      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5, 7, 9, 11, 13, 17, 19, 23, 29, 31));
      Assert.True(proj.IsFeasible(result.Parameters));

      Assert.Equal(7, result.Parameters[1]);
      Assert.Equal(11, result.Parameters[3]);
      Assert.Equal(17, result.Parameters[5]);
      Assert.Equal(23, result.Parameters[7]);
      Assert.Equal(31, result.Parameters[9]);


      Assert.Equal(result.Parameters[0] * 0.9, result.Parameters[2], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.8, result.Parameters[4], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.7, result.Parameters[6], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.6, result.Parameters[8], precision: 7);

      result = ProjectWithCheckedProperties(proj, TestHelpers.V(5, 26, -9, 25, -13, 24, -19, 23, -25, 31));
      Assert.True(proj.IsFeasible(result.Parameters));

    }


    [Fact]
    public void TenParameters_5FullyFree_5Equality_9InequalityConstraints()
    {
      // We have 10 parameters, of which 5 are fixed by equality constraints, and 5 are constrained by inequalities.

      var (A, b) = TestHelpers.MV(new double[][]
      {
      [1.0, 0,  0, 0,  0, 0,  0, 0,  0, 0],  [3], // x[0] == 3
      [0.9, 0, -1, 0,  0, 0,  0, 0,  0, 0],  [0], // x[2] == 0.9 * x[0]
      [0.8, 0,  0, 0, -1, 0,  0, 0,  0, 0],  [0], // x[4] == 0.8 * x[0]
      [0.7, 0,  0, 0,  0, 0, -1, 0,  0, 0],  [0], // x[6] == 0.7 * x[0]
      [0.6, 0,  0, 0,  0, 0,  0, 0, -1, 0],  [0], // x[8] == 0.6 * x[0]
      }
      );

      var (C, d) = TestHelpers.MV(new double[][]
      {
      [-1, 0, 0, 0, 0, 0, 0, 0, 0, 0], [0], // x[0] >= 0
      [0, 0, -1, 0, 0, 0, 0, 0, 0, 0], [0], // x[2] >= 0
      [0, 0, 0, 0, -1, 0, 0, 0, 0, 0], [0], // x[4] >= 0
      [0, 0, 0, 0, 0, 0, -1, 0, 0, 0], [0], // x[6] >= 0
      [0, 0, 0, 0, 0, 0, 0, 0, -1, 0], [0], // x[8] >= 0
      [0, 1, 0, -1, 0, 0, 0, 0, 0, 0], [0], // x[3] >= x[1]  expressed as  x[1] - x[3] <= 0
      [0, 0, 0, 1, 0, -1, 0, 0, 0, 0], [0], // x[5] >= x[3]  expressed as  x[3] - x[5] <= 0
      [0, 0, 0, 0, 0, 1, 0, -1, 0, 0], [0], // x[7] >= x[5]  expressed as  x[5] - x[7] <= 0
      [0, 0, 0, 0, 0, 0, 0,  1, 0,-1], [0], // x[9] >= x[7]  expressed as  x[7] - x[9] <= 0
      }
      );

      var proj = new LinearConstraintsProjector(A: A, b: b, C: C, d: d);
      Assert.Equal(10, proj.NumberOfParameters);
      Assert.Equal(0, proj.NumberOfParametersFullyFree);
      Assert.Equal(5, proj.NumberOfParametersFixed);
      Assert.Equal(5, proj.NumberOfParametersThatCanVary);
      Assert.False(proj.AreAllParametersFullyFree);
      Assert.False(proj.AreAllParametersFixed);


      var result = ProjectWithCheckedProperties(proj, TestHelpers.V(5, 7, 9, 11, 13, 17, 19, 23, 29, 31));
      Assert.True(proj.IsFeasible(result.Parameters));

      Assert.Equal(7, result.Parameters[1]);
      Assert.Equal(11, result.Parameters[3]);
      Assert.Equal(17, result.Parameters[5]);
      Assert.Equal(23, result.Parameters[7]);
      Assert.Equal(31, result.Parameters[9]);

      Assert.Equal(3, result.Parameters[0], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.9, result.Parameters[2], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.8, result.Parameters[4], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.7, result.Parameters[6], precision: 7);
      Assert.Equal(result.Parameters[0] * 0.6, result.Parameters[8], precision: 7);

      result = ProjectWithCheckedProperties(proj, TestHelpers.V(5, 26, -9, 25, -13, 24, -19, 23, -25, 31));
      Assert.True(proj.IsFeasible(result.Parameters));

      AssertProjectedStepDoesNotReverseDirectionForRandomSteps(
        proj,
        sampleCount: 1000,
        seed: 48193,
        feasiblePointFactory: random =>
        {
          double x1 = NextRandomDouble(random, -10.0, 10.0);
          double delta13 = NextRandomDouble(random, 0.0, 5.0);
          double delta35 = NextRandomDouble(random, 0.0, 5.0);
          double delta57 = NextRandomDouble(random, 0.0, 5.0);
          double delta79 = NextRandomDouble(random, 0.0, 5.0);

          double x3 = x1 + delta13;
          double x5 = x3 + delta35;
          double x7 = x5 + delta57;
          double x9 = x7 + delta79;

          return TestHelpers.V(3.0, x1, 2.7, x3, 2.4, x5, 2.1, x7, 1.8, x9);
        },
        intendedPointFactory: (random, feasiblePoint) => TestHelpers.V(
          feasiblePoint[0] + NextRandomDouble(random, -4.0, 4.0),
          feasiblePoint[1] + NextRandomDouble(random, -8.0, 8.0),
          feasiblePoint[2] + NextRandomDouble(random, -4.0, 4.0),
          feasiblePoint[3] + NextRandomDouble(random, -8.0, 8.0),
          feasiblePoint[4] + NextRandomDouble(random, -4.0, 4.0),
          feasiblePoint[5] + NextRandomDouble(random, -8.0, 8.0),
          feasiblePoint[6] + NextRandomDouble(random, -4.0, 4.0),
          feasiblePoint[7] + NextRandomDouble(random, -8.0, 8.0),
          feasiblePoint[8] + NextRandomDouble(random, -4.0, 4.0),
          feasiblePoint[9] + NextRandomDouble(random, -8.0, 8.0)));
    }

  }
}
