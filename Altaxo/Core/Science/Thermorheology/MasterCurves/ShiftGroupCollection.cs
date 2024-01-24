#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A set of <see cref="ShiftGroup"/>s. The curves in these collections will be shifted with a common set of shift factors.
  /// For example, for complex data to be shifted, there will be two <see cref="ShiftGroup"/>s, one for the real part, and one for the imaginary.
  /// The curves of the real part will finally form the master curve of the real part, and the curves of the imaginary part will finally
  /// form the master curve of the imaginary part.
  /// </summary>
  public class ShiftGroupCollection : IReadOnlyList<ShiftGroup>
  {
    ShiftGroup[] _inner;

    /// <summary>Index of the reference curve.</summary>
    public int IndexOfReferenceColumnInColumnGroup { get; init; }

    /// <summary>
    /// Determines the method to best fit the data into the master curve.
    /// </summary>
    public OptimizationMethod OptimizationMethod { get; init; }

    public ShiftOrder ShiftOrder { get; init; } = ShiftOrder.PivotToLastAlternating;

    protected int _numberOfIterations = 20;

    /// <summary>
    /// Gets or sets the number of iterations. Must be greater than or equal to 1.
    /// This number determines how many rounds the master curve is fitted. Increasing this value will in most cases
    /// increase the quality of the fit.
    /// </summary>
    /// <value>
    /// The number of iterations for master curve creation.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 1</exception>
    public int NumberOfIterations
    {
      get { return _numberOfIterations; }
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a number >= 1");

        _numberOfIterations = value;
      }
    }

    protected double _requiredRelativeOverlap = 0;
    /// <summary>
    /// Gets/sets the required relative overlap. The default value is 0, which means that a curve part only needs to touch the rest of the master curve.
    /// Setting this to a value, for instance to 0.1, means that a curve part needs an overlapping of 10% (of its x-range) with the rest of the master curve.
    /// This value can also be set to negative values. For instance, setting it to -1 means that a curve part could be in 100% distance (of its x-range) to the rest of the master curve.
    /// </summary>
    /// <value>
    /// The required overlap.
    /// </value>
    public double RequiredRelativeOverlap
    {
      get => _requiredRelativeOverlap;
      init
      {
        if (double.IsNaN(value))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a valid number");

        _requiredRelativeOverlap = value;
      }
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroupCollection"/> class.
    /// </summary>
    /// <param name="data">The set of <see cref="ShiftGroup"/>s.</param>
    public ShiftGroupCollection(IEnumerable<ShiftGroup> data)
    {
      _inner = data.ToArray();
    }

    /// <inheritdoc/>
    public ShiftGroup this[int index] => ((IReadOnlyList<ShiftGroup>)_inner)[index];

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<ShiftGroup>)_inner).Count;

    /// <inheritdoc/>
    public IEnumerator<ShiftGroup> GetEnumerator()
    {
      return ((IEnumerable<ShiftGroup>)_inner).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _inner.GetEnumerator();
    }
  }
}

