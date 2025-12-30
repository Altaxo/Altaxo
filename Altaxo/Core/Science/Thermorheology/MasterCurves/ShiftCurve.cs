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
using System.Collections.Generic;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// The data for one curve to be shifted consist of arrays of X and Y values.
  /// </summary>
  /// <typeparam name="T">The type of the Y values.</typeparam>
  public record ShiftCurve<T>
  {
    /// <summary>
    /// Gets the x-values of the curve to be shifted.
    /// </summary>
    public IReadOnlyList<double> X { get; }

    /// <summary>
    /// Gets the y-values of the curve to be shifted.
    /// </summary>
    public IReadOnlyList<T> Y { get; }

    /// <summary>
    /// Gets the number of points of this curve.
    /// </summary>
    /// <remarks>
    /// Returns the minimum of the lengths of the X and Y lists to ensure pairs of coordinates.
    /// </remarks>
    public int Count
    {
      get
      {
        return Math.Min(X.Count, Y.Count);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftCurve{T}"/> class.
    /// </summary>
    /// <param name="x">The x-values of the curve to be shifted.</param>
    /// <param name="y">The y-values of the curve to be shifted.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="x"/> or <paramref name="y"/> is <c>null</c>.</exception>
    public ShiftCurve(IReadOnlyList<double> x, IReadOnlyList<T> y)
    {
      X = x ?? throw new ArgumentNullException(nameof(x));
      Y = y ?? throw new ArgumentNullException(nameof(y));
    }
  }
}
