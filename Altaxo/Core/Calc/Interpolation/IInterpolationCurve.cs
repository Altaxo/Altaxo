#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Represents an interpolation curve parametrized by a parameter <c>u</c>.
  /// Implementations may represent non-functional curves where multiple y values can exist for a single x.
  /// </summary>
  public interface IInterpolationCurve
  {
    /// <summary>
    /// Sets the interpolation data by providing values for x and y. Both vectors must be of equal length.
    /// </summary>
    /// <param name="xvec">Vector of x (independent) data.</param>
    /// <param name="yvec">Vector of y (dependent) data.</param>
    void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec);

    /// <summary>
    /// Returns a y value in dependence of a parameter u.
    /// </summary>
    /// <param name="u">The parameter.</param>
    /// <returns>The y value at the given value of u.</returns>
    double GetYOfU(double u);

    /// <summary>
    /// Returns a x value in dependence of a parameter u.
    /// </summary>
    /// <param name="u">The parameter.</param>
    /// <returns>The y value at the given value of u.</returns>
    double GetXOfU(double u);
  }
}
