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
  /// Contains the result of the master curve creation.
  /// </summary>
  public class MasterCurveCreationResult
  {
    /// <summary>
    /// Resulting list of shift offsets or ln(shiftfactors).
    /// </summary>
    public List<double> ResultingShifts { get; } = new List<double>();

    /// <summary>
    /// Gets the shift groups. Contains one or more list of integer values, which designate the indices
    /// of the shift curves that should be shifted one by one. See further explanations below.
    /// </summary>
    /// <remarks>
    /// For example, if there are 10 shift curves (0..9), and the reference curve is at index 5,
    /// then there should be two shift groups, one containing the indices 4, 3, 2, 1, 0 and the other the indices 6, 7, 8, 9.
    /// The index 5 is not contained in neither of the groups because this curve is not shifted.
    /// </remarks>
    public List<List<int>> ShiftGroups { get; } = new List<List<int>>();


    /// <summary>
    /// Gets the resulting interpolation curve for each group of columns.
    /// </summary>
    public InterpolationInformation[] ResultingInterpolation { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterCurveCreationResult"/> class.
    /// </summary>
    /// <param name="numberOfShiftCurveCollections">The number of <see cref="ShiftCurveCollection"/>s.</param>
    public MasterCurveCreationResult(int numberOfShiftCurveCollections)
    {
      ResultingInterpolation = new InterpolationInformation[numberOfShiftCurveCollections];
    }

  }
}

