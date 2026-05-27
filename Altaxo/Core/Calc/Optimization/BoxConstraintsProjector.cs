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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Projects input values onto a set of boundary constraints, enforcing fixed values, lower bounds, and upper bounds
  /// as specified.
  /// </summary>
  /// <remarks>Supports element-wise fixed values and bounds. Throws exceptions if constraints are inconsistent
  /// or infeasible.</remarks>
  public class BoxConstraintsProjector : IConstraintsProjector
  {
    double?[] _fixedValues;
    double?[]? _lowerBounds;
    bool[]? _lowerBoundsExclusive;
    double?[]? _upperBounds;
    bool[]? _upperBoundsExclusive;
    double?[]? _effectiveLowerBounds;
    double?[]? _effectiveUpperBounds;


    /// <summary>
    /// Initializes a new instance of the BoundaryConstraintProjector class with specified fixed values and optional
    /// lower and upper bounds.
    /// </summary>
    /// <param name="fixedValues">An array of fixed values for each dimension. Null entries indicate unfixed dimensions.</param>
    /// <param name="lowerBounds">An optional array specifying the lower bounds for each dimension. Null entries indicate no lower bound.</param>
    /// <param name="upperBounds">An optional array specifying the upper bounds for each dimension. Null entries indicate no upper bound.</param>
    /// <exception cref="ArgumentException">Thrown when the lengths of lowerBounds or upperBounds do not match fixedValues, when a fixed value is NaN, when
    /// a fixed value is outside the specified bounds, or when a lower bound is greater than the corresponding upper
    /// bound.</exception>
    public BoxConstraintsProjector(double?[] fixedValues, double?[]? lowerBounds, double?[]? upperBounds)
    {

      { // check arguments
        ArgumentNullException.ThrowIfNull(fixedValues, nameof(fixedValues));

        if (lowerBounds is { } lb && lb.Length != fixedValues.Length)
          throw new ArgumentException($"Length of {nameof(lowerBounds)} must match length of {nameof(fixedValues)}.", nameof(lowerBounds));

        if (upperBounds is { } ub && ub.Length != fixedValues.Length)
          throw new ArgumentException($"Length of {nameof(upperBounds)} must match length of {nameof(fixedValues)}.", nameof(upperBounds));
      }

      _fixedValues = fixedValues;
      _lowerBounds = lowerBounds;
      _upperBounds = upperBounds;

      _lowerBoundsExclusive = lowerBounds is null ? null : new bool[lowerBounds.Length];
      _upperBoundsExclusive = upperBounds is null ? null : new bool[upperBounds.Length];

      _effectiveLowerBounds = lowerBounds;
      _effectiveUpperBounds = upperBounds;

      CheckFeasibility(fixedValues, _effectiveLowerBounds, _effectiveUpperBounds);
    }

    /// <summary>
    /// Initializes a new instance of the BoxConstraintsProjector class with specified fixed values, bounds, and
    /// exclusivity settings.
    /// </summary>
    /// <param name="fixedValues">Array of fixed values for each dimension, or null to indicate no fixed value.</param>
    /// <param name="lowerBounds">Array of lower bounds for each dimension, or null if not specified.</param>
    /// <param name="lowerBoundsExclusive">Array indicating whether each lower bound is exclusive, or null if not specified.</param>
    /// <param name="upperBounds">Array of upper bounds for each dimension, or null if not specified.</param>
    /// <param name="upperBoundsExclusive">Array indicating whether each upper bound is exclusive, or null if not specified.</param>

    public BoxConstraintsProjector(double?[] fixedValues, double?[]? lowerBounds, bool[]? lowerBoundsExclusive, double?[]? upperBounds, bool[]? upperBoundsExclusive)
    {
      { // check arguments
        ArgumentNullException.ThrowIfNull(fixedValues, nameof(fixedValues));

        if (lowerBounds is { } lb)
        {
          if (lb.Length != fixedValues.Length)
            throw new ArgumentException($"Length of {nameof(lowerBounds)} must match length of {nameof(fixedValues)}.", nameof(lowerBounds));

          ArgumentNullException.ThrowIfNull(lowerBoundsExclusive, nameof(lowerBoundsExclusive));

          if (lowerBoundsExclusive.Length != fixedValues.Length)
            throw new ArgumentException($"Length of {nameof(lowerBoundsExclusive)} must match length of {nameof(fixedValues)}.", nameof(lowerBoundsExclusive));

        }

        if (upperBounds is { } ub)
        {
          if (ub.Length != fixedValues.Length)
            throw new ArgumentException($"Length of {nameof(upperBounds)} must match length of {nameof(fixedValues)}.", nameof(upperBounds));

          ArgumentNullException.ThrowIfNull(upperBoundsExclusive, nameof(upperBoundsExclusive));

          if (upperBoundsExclusive.Length != fixedValues.Length)
            throw new ArgumentException($"Length of {nameof(upperBoundsExclusive)} must match length of {nameof(fixedValues)}.", nameof(upperBoundsExclusive));
        }
      }

      _fixedValues = fixedValues;
      _lowerBounds = lowerBounds;
      _upperBounds = upperBounds;

      _lowerBoundsExclusive = lowerBoundsExclusive;
      _upperBoundsExclusive = upperBoundsExclusive;

      _effectiveLowerBounds = GetModifiedLowerBounds(lowerBounds, lowerBoundsExclusive);
      _effectiveUpperBounds = GetModifiedUpperBounds(upperBounds, upperBoundsExclusive);

      CheckFeasibility(fixedValues, _effectiveLowerBounds, _effectiveUpperBounds);
    }

    static void CheckFeasibility(double?[] fixedValues, double?[]? lowerBounds, double?[]? upperBounds)
    {
      { // check feasibility
        for (int i = 0; i < fixedValues.Length; i++)
        {
          if (fixedValues[i] is { } fixedValue)
          {
            if (double.IsNaN(fixedValue))
              throw new ArgumentException($"Fixed value at index {i} is NaN.", nameof(fixedValues));

            if (lowerBounds is { } lb && lb[i] is { } lowerBound && !(fixedValue >= lowerBound))
              throw new ArgumentException($"Fixed value at index {i} is less than the corresponding lower bound.", nameof(fixedValues));

            if (upperBounds is { } ub && ub[i] is { } upperBound && !(fixedValue <= upperBound))
              throw new ArgumentException($"Fixed value at index {i} is greater than the corresponding upper bound.", nameof(fixedValues));
          }
          else
          {
            if (lowerBounds is { } lb && lb[i] is { } lowerBound && upperBounds is { } ub && ub[i] is { } upperBound && !(lowerBound <= upperBound))
              throw new ArgumentException($"Lower bound at index {i} is greater than the corresponding upper bound.", nameof(lowerBounds));
          }
        }
      }
    }

    /// <inheritdoc/>
    public bool AreAllParametersFixed => _fixedValues.All(v => v.HasValue);

    /// <summary>
    /// Returns a modified copy of the lower bounds array, incrementing values where the corresponding exclusive flag is
    /// true.
    /// </summary>
    /// <param name="lowerBounds">An array of nullable lower bound values to be modified.</param>
    /// <param name="lowerBoundsExclusive">An array of nullable Boolean values indicating whether the corresponding lower bound is exclusive.</param>
    /// <returns>A new array with modified lower bounds, or null if the input is null.</returns>
    private static double?[]? GetModifiedLowerBounds(double?[]? lowerBounds, bool[]? lowerBoundsExclusive)
    {
      if (lowerBounds is not null && lowerBoundsExclusive is not null)
      {
        lowerBounds = (double?[])lowerBounds.Clone();
        for (int i = 0; i < lowerBounds.Length; i++)
        {
          if (lowerBoundsExclusive[i] is { } exclusive && exclusive && lowerBounds[i] is { } value)
          {
            lowerBounds[i] = Math.BitIncrement(value) + double.Epsilon;
          }
        }
      }
      return lowerBounds;
    }

    /// <summary>
    /// Returns a modified copy of the upper bounds array, decrementing values where the corresponding exclusive flag is
    /// true.
    /// </summary>
    /// <remarks>Each upper bound marked as exclusive is decremented using Math.BitDecrement and adjusted by
    /// subtracting double.Epsilon.</remarks>
    /// <param name="upperBounds">An array of nullable upper bound values to be potentially modified.</param>
    /// <param name="upperBoundsExclusive">An array of boolean flags indicating which upper bounds are exclusive.</param>
    /// <returns>A new array with modified upper bounds, or null if the input is null.</returns>
    private static double?[]? GetModifiedUpperBounds(double?[]? upperBounds, bool[]? upperBoundsExclusive)
    {
      if (upperBounds is not null && upperBoundsExclusive is not null)
      {
        upperBounds = (double?[])upperBounds.Clone();
        for (int i = 0; i < upperBounds.Length; i++)
        {
          if (upperBoundsExclusive[i] is { } exclusive && exclusive && upperBounds[i] is { } value)
          {
            upperBounds[i] = Math.BitDecrement(value) - double.Epsilon;
          }
        }
      }
      return upperBounds;
    }

    /// <inheritdoc/>
    public bool IsFeasible(Vector<double> inputValues)
    {
      for (int i = 0; i < inputValues.Count; i++)
      {
        double value = inputValues[i];
        if (_fixedValues[i] is { } fixedValue && !(fixedValue == value))
        {
          return false;
        }
        else
        {
          if (_effectiveLowerBounds is { } lowerBounds && lowerBounds[i] is { } lowerBound && !(value >= lowerBound))
            return false;
          if (_effectiveUpperBounds is { } upperBounds && upperBounds[i] is { } upperBound && !(value <= upperBound))
            return false;
        }
      }
      return true;
    }

    /// <inheritdoc/>
    public void Project(Vector<double> inputValues, Vector<double> projectedValues, Span<bool> valuesConstrained)
    {
      for (int i = 0; i < inputValues.Count; i++)
      {
        if (_fixedValues[i] is { } fixedValue)
        {
          projectedValues[i] = fixedValue;
          valuesConstrained[i] = true;
        }
        else
        {
          double value = inputValues[i];
          if (_effectiveLowerBounds is { } lowerBounds && lowerBounds[i] is { } lowerBound)
          {
            value = Math.Max(value, lowerBound);
            valuesConstrained[i] = true;
          }
          if (_effectiveUpperBounds is { } upperBounds && upperBounds[i] is { } upperBound)
          {
            value = Math.Min(value, upperBound);
            valuesConstrained[i] = true;
          }
          projectedValues[i] = value;
        }
      }
    }

    /// <inheritdoc/>
    public (double?[] fixedValues, double?[]? LowerBounds, bool[]? isLowerBoundExclusive, double?[]? UpperBounds, bool[]? isUpperBoundExclusive)? TryConvertToBoxConstraints(double tolerance = 1E-12)
    {
      return (_fixedValues, _lowerBounds, _lowerBoundsExclusive, _upperBounds, _upperBoundsExclusive);
    }

    /// <inheritdoc/>
    public (IConstraintsProjector Projector, double?[] FixedParameters) ToProjectorWithoutFixedParameters()
    {
      int nnew = _fixedValues.Count(v => !v.HasValue);

      double?[] newFixedValues = new double?[nnew];

      double?[]? newLowerBounds = _lowerBounds is null ? null : new double?[nnew];
      bool[]? newLowerBoundsExclusive = _lowerBoundsExclusive is null ? null : new bool[nnew];
      double?[]? newUpperBounds = _upperBounds is null ? null : new double?[nnew];
      bool[]? newUpperBoundsExclusive = _upperBoundsExclusive is null ? null : new bool[nnew];

      for (int i = 0, j = 0; i < _fixedValues.Length; i++)
      {
        if (!_fixedValues[i].HasValue)
        {
          newFixedValues[j] = _fixedValues[i];
          if (newLowerBounds is not null)
            newLowerBounds[j] = _lowerBounds[i];
          if (newLowerBoundsExclusive is not null)
            newLowerBoundsExclusive[j] = _lowerBoundsExclusive[i];
          if (newUpperBounds is not null)
            newUpperBounds[j] = _upperBounds[i];
          if (newUpperBoundsExclusive is not null)
            newUpperBoundsExclusive[j] = _upperBoundsExclusive[i];
          j++;
        }
      }
      return (new BoxConstraintsProjector(newFixedValues, newLowerBounds, newLowerBoundsExclusive, newUpperBounds, newUpperBoundsExclusive), (double?[])_fixedValues.Clone());
    }
  }
}
