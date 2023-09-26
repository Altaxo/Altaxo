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

using System.Collections.Generic;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// The data for one curve to be shifted consist of an array of X and Y.
  /// </summary>
  public record ShiftCurve
  {
    /// <summary>
    /// Gets the x-values of the curve to be shifted.
    /// </summary>
    public IReadOnlyList<double> X { get; }

    /// <summary>
    /// Gets the y-values of the curve to be shifted.
    /// </summary>
    public IReadOnlyList<double> Y { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftCurve"/> class.
    /// </summary>
    /// <param name="x">The x-values of the curve to be shifted.</param>
    /// <param name="y">The y-values of the curve to be shifted.</param>
    public ShiftCurve(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      X = x;
      Y = y;
    }
  }
}
