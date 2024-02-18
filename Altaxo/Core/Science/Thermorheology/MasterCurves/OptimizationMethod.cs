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

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Determines how to best fit the data into the master curve.
  /// </summary>
  public enum OptimizationMethod
  {
    /// <summary>
    /// Evaluates the mean absolute difference (L1-Norm) between master curve and new data and tries to minimize this difference.
    /// </summary>
    OptimizeAbsoluteDifference,

    /// <summary>
    /// Evaluates the mean squared difference (L2-Norm) between master curve and new data and tries to minimize this value (minimization method).
    /// </summary>
    OptimizeSquaredDifference,

    /// <summary>
    /// Evaluates the mean squared difference (L2-Norm) between master curve and new data and tries to minimize this value by brute force search.
    /// </summary>
    OptimizeSquaredDifferenceByBruteForce
  }
}

