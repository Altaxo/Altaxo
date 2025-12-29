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

using System;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Determines how to calculate the cross-validated predicted error sum of squares (Cross PRESS).
  /// </summary>
  /// <remarks>
  /// The serialization code for this enumeration is located in AltaxoBase.
  /// </remarks>
  public enum CrossPRESSCalculationType
  {
    /// <summary>
    /// No Cross PRESS calculation.
    /// </summary>
    None,

    /// <summary>
    /// Excludes every measurement to calculate Cross PRESS.
    /// </summary>
    ExcludeEveryMeasurement,

    /// <summary>
    /// Excludes measurements (with identical concentration values) as groups to calculate Cross PRESS.
    /// </summary>
    ExcludeGroupsOfSimilarMeasurements,

    /// <summary>
    /// Divides measurements into two groups such that measurements with similar <c>Y</c> values are distributed
    /// as evenly as possible between the two groups.
    /// </summary>
    ExcludeHalfEnsemblyOfMeasurements
  }
}
