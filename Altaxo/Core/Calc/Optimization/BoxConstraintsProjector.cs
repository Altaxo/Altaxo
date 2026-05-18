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
    double?[]? _upperBounds;

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

      _fixedValues = fixedValues;
      _lowerBounds = lowerBounds;
      _upperBounds = upperBounds;
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
          if (_lowerBounds is { } lowerBounds && lowerBounds[i] is { } lowerBound && !(value >= lowerBound))
            return false;
          if (_upperBounds is { } upperBounds && upperBounds[i] is { } upperBound && !(value <= upperBound))
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
          if (_lowerBounds is { } lowerBounds && lowerBounds[i] is { } lowerBound)
          {
            value = Math.Max(value, lowerBound);
            valuesConstrained[i] = true;
          }
          if (_upperBounds is { } upperBounds && upperBounds[i] is { } upperBound)
          {
            value = Math.Min(value, upperBound);
            valuesConstrained[i] = true;
          }
          projectedValues[i] = value;
        }
      }
    }
  }
}
