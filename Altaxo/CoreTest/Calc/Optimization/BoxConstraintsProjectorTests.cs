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
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.Optimization
{
  public class BoxConstraintsProjectorTests
  {
    private static Vector<double> V(params double[] values)
      => Vector<double>.Build.DenseOfArray(values);

    private static void AssertVector(Vector<double> expected, Vector<double> actual, double tolerance = 0)
    {
      Assert.Equal(expected.Count, actual.Count);

      for (int i = 0; i < expected.Count; i++)
      {
        AssertEx.Equal(expected[i], actual[i], tolerance, $"Mismatch at index {i}");
      }
    }

    [Fact]
    public void Constructor_NullFixedValues_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new BoxConstraintsProjector(null!, null, null));
    }

    [Fact]
    public void Constructor_LowerBoundsLengthMismatch_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(new double?[] { null, null }, new double?[] { 0 }, null));

      Assert.Equal("lowerBounds", exception.ParamName);
    }

    [Fact]
    public void Constructor_UpperBoundsLengthMismatch_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(new double?[] { null, null }, null, new double?[] { 1 }));

      Assert.Equal("upperBounds", exception.ParamName);
    }

    [Fact]
    public void Constructor_NaNFixedValue_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(new double?[] { double.NaN }, null, null));

      Assert.Equal("fixedValues", exception.ParamName);
    }

    [Fact]
    public void Constructor_FixedValueBelowLowerBound_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(new double?[] { 1 }, new double?[] { 2 }, null));

      Assert.Equal("fixedValues", exception.ParamName);
    }

    [Fact]
    public void Constructor_FixedValueAboveUpperBound_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(new double?[] { 3 }, null, new double?[] { 2 }));

      Assert.Equal("fixedValues", exception.ParamName);
    }

    [Fact]
    public void Constructor_LowerBoundGreaterThanUpperBound_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(new double?[] { null }, new double?[] { 3 }, new double?[] { 2 }));

      Assert.Equal("lowerBounds", exception.ParamName);
    }

    [Fact]
    public void IsFeasible_ReturnsTrue_WhenAllConstraintsAreSatisfied()
    {
      var projector = new BoxConstraintsProjector(
        new double?[] { 5, null, null, null },
        new double?[] { null, -1, 0, null },
        new double?[] { null, 3, null, 10 });

      Assert.True(projector.IsFeasible(V(5, 0, 4, 10)));
      Assert.True(projector.IsFeasible(V(5, -1, 0, -100)));
    }

    [Fact]
    public void IsFeasible_ReturnsFalse_WhenFixedValueIsViolated()
    {
      var projector = new BoxConstraintsProjector(new double?[] { 5 }, null, null);

      Assert.False(projector.IsFeasible(V(4.999999999)));
    }

    [Fact]
    public void IsFeasible_ReturnsFalse_WhenLowerBoundIsViolated()
    {
      var projector = new BoxConstraintsProjector(new double?[] { null }, new double?[] { 0 }, null);

      Assert.False(projector.IsFeasible(V(-1)));
    }

    [Fact]
    public void IsFeasible_ReturnsFalse_WhenUpperBoundIsViolated()
    {
      var projector = new BoxConstraintsProjector(new double?[] { null }, null, new double?[] { 2 });

      Assert.False(projector.IsFeasible(V(2.5)));
    }

    [Fact]
    public void Project_AppliesFixedValuesAndBounds()
    {
      var projector = new BoxConstraintsProjector(
        new double?[] { 5, null, null, null, null },
        new double?[] { null, 0, 0, null, null },
        new double?[] { null, 10, null, 1, null });

      var input = V(-100, -2, 4, 3, 7);
      var projected = V(111, 111, 111, 111, 111);
      var constrained = new bool[5];

      projector.Project(input, projected, constrained);

      AssertVector(V(5, 0, 4, 1, 7), projected);
      Assert.Equal(new[] { true, true, true, true, false }, constrained);
    }

    [Fact]
    public void Project_MarksBoundedValueAsConstrainedEvenWhenAlreadyWithinBounds()
    {
      var projector = new BoxConstraintsProjector(
        new double?[] { null, null, null },
        new double?[] { 0, null, null },
        new double?[] { 10, 5, null });

      var input = V(3, 4, 9);
      var projected = V(0, 0, 0);
      var constrained = new bool[3];

      projector.Project(input, projected, constrained);

      AssertVector(input, projected);
      Assert.Equal(new[] { true, true, false }, constrained);
    }

    [Fact]
    public void Project_UsesFixedValueInsteadOfBounds()
    {
      var projector = new BoxConstraintsProjector(
        new double?[] { 2 },
        new double?[] { 0 },
        new double?[] { 5 });

      var projected = V(0);
      var constrained = new bool[1];

      projector.Project(V(100), projected, constrained);

      AssertVector(V(2), projected);
      Assert.Equal(new[] { true }, constrained);
    }

    [Fact]
    public void ExclusiveBoundsConstructor_ProjectsToStrictlyInsideAdjustedBounds()
    {
      double lowerBound = 1.0;
      double upperBound = 5.0;
      double expectedLower = Math.BitIncrement(lowerBound) + double.Epsilon;
      double expectedUpper = Math.BitDecrement(upperBound) - double.Epsilon;

      var projector = new BoxConstraintsProjector(
        new double?[] { null, null },
        new double?[] { lowerBound, null },
        new bool?[] { true, null },
        new double?[] { null, upperBound },
        new bool?[] { null, true });

      Assert.False(projector.IsFeasible(V(lowerBound, upperBound)));
      Assert.True(projector.IsFeasible(V(expectedLower, expectedUpper)));

      var projected = V(0, 0);
      var constrained = new bool[2];

      projector.Project(V(lowerBound, upperBound), projected, constrained);

      AssertVector(V(expectedLower, expectedUpper), projected);
      Assert.Equal(new[] { true, true }, constrained);
    }

    [Fact]
    public void ExclusiveBoundsConstructor_DoesNotModifyInputArrays()
    {
      var lowerBounds = new double?[] { 1.0 };
      var upperBounds = new double?[] { 5.0 };
      var lowerBoundsExclusive = new bool?[] { true };
      var upperBoundsExclusive = new bool?[] { true };

      _ = new BoxConstraintsProjector(
        new double?[] { null },
        lowerBounds,
        lowerBoundsExclusive,
        upperBounds,
        upperBoundsExclusive);

      Assert.Equal(1.0, lowerBounds[0]);
      Assert.Equal(5.0, upperBounds[0]);
      Assert.True(lowerBoundsExclusive[0]);
      Assert.True(upperBoundsExclusive[0]);
    }

    [Fact]
    public void ExclusiveBoundsConstructor_FixedValueOnExclusiveBoundary_ThrowsArgumentException()
    {
      var exception = Assert.Throws<ArgumentException>(() => new BoxConstraintsProjector(
        new double?[] { 1.0 },
        new double?[] { 1.0 },
        new bool?[] { true },
        null,
        null));

      Assert.Equal("fixedValues", exception.ParamName);
    }
  }
}
