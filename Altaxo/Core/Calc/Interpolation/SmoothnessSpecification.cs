#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  /// Specifies how the smoothness of the spline is interpreted.
  /// </summary>
  public enum SmoothnessSpecification
  {
    /// <summary>
    /// The smoothness value is used as-is, without modification.
    /// </summary>
    Direct = 0,

    /// <summary>
    /// The smoothness value designates the number of features; i.e. it also depends on the number of points.
    /// For instance, if one has 4000 points in the spectrum and <c>NumberOfFeatures = 10</c>, then the number
    /// of points in one period is <c>4000 / 10 = 400</c>.
    /// </summary>
    ByNumberOfFeatures = 1,

    /// <summary>
    /// The smoothness value designates the number of points in a feature span.
    /// If the spectrum is a sine signal with this period, the spline is expected to smooth the sine signal down
    /// to an amplitude of <c>1/e</c> of the original amplitude.
    /// </summary>
    ByNumberOfPoints = 2,

    /// <summary>
    /// The smoothness value designates the x-span of a feature.
    /// If the spectrum is a sine signal with this period, the spline is expected to smooth the sine signal down
    /// to an amplitude of <c>1/e</c> of the original amplitude.
    /// </summary>
    ByXSpan = 3,
  }
}
