#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Gives an interpolation function that maps each x to exactly one y value.
  /// Extends <see cref="IInterpolationCurve"/> with the function mapping capability.
  /// </summary>
  /// <remarks>
  /// Implementations represent single-valued interpolation functions: for each
  /// input x the function returns exactly one y. Use <see cref="IInterpolationCurve"/>
  /// when a parametric curve representation (x(u), y(u)) is required.
  /// </remarks>
  public interface IInterpolationFunction : IInterpolationCurve
  {
    /// <summary>
    /// Returns the y value in dependence of a given x value.
    /// </summary>
    /// <param name="x">The x value (value of the independent variable).</param>
    /// <returns>The y value at the given x value.</returns>
    /// <remarks>
    /// Implementations are expected to have been initialized by calling
    /// <see cref="IInterpolationCurve.Interpolate(System.Collections.Generic.IReadOnlyList{double}, System.Collections.Generic.IReadOnlyList{double})"/> before use.
    /// </remarks>
    double GetYOfX(double x);
  }
}
