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
  /// Contains the result of the master curve creation.
  /// </summary>
  public class MasterCurveCreationResultBase
  {
    /// <summary>
    /// Resulting list of shift offsets or ln(shiftfactors).
    /// </summary>
    public List<double> ResultingShifts { get; } = new List<double>();

    /// <summary>
    /// Transfers the the shifts evaluated in the first stage to the improvement stage. The result is a pre-set <see cref="ResultingShifts"/> list, evaluated in the first stage.
    /// </summary>
    /// <param name="orgShifts">The shifts evaluated in the first stage.</param>
    /// <param name="orgIndices">Array that maps the high level curve number to the low level curve number, used in the first stage. If an element is zero, that high level curve does not participate in the master curve.</param>
    /// <param name="currIndices">Array that maps the high level curve number to the low level curve number, used in the improvement stage. If an element is zero, that high level curve does not participate in the master curve.</param>
    public void SetShiftsFromFirstStage(List<double> orgShifts, IReadOnlyList<int?> orgIndices, IReadOnlyList<int?> currIndices)
    {
      if (orgIndices.Count != currIndices.Count)
        throw new ArgumentOutOfRangeException(nameof(currIndices), $"Length of {nameof(currIndices)} has to be the same as length of {nameof(orgIndices)}");

      ResultingShifts.Clear();

      for (int i = 0; i < currIndices.Count; i++)
      {
        if (currIndices[i].HasValue)
        {
          var idx = orgIndices[i];
          if (idx.HasValue)
            ResultingShifts.Add(orgShifts[idx.Value]);
          else
            throw new InvalidOperationException($"A shift value that is required in the improvement stage was not evaluated in the first stage. Details: Curve number: {i}");
        }
      }
    }
  }

  /// <summary>
  /// Contains the result of the master curve creation.
  /// </summary>
  public class MasterCurveCreationResult : MasterCurveCreationResultBase
  {
    /// <summary>
    /// Gets the resulting interpolation curve for each group of columns.
    /// </summary>
    public InterpolationInformation[] ResultingInterpolation { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterCurveCreationResult"/> class.
    /// </summary>
    /// <param name="numberOfShiftCurveCollections">The number of <see cref="ShiftGroup"/>s.</param>
    public MasterCurveCreationResult(int numberOfShiftCurveCollections)
    {
      ResultingInterpolation = new InterpolationInformation[numberOfShiftCurveCollections];
    }

  }
}

