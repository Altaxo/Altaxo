#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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
  /// Designates the order in which the curves are tried to fit.
  /// </summary>
  public enum ShiftOrder
  {
    /// <summary>Fit by fixing the 1st curve, then adding the 2nd curve, 3rd curve, up to the Nth curve.</summary>
    FirstToLast,

    /// <summary>Fit by fixing the Nth curve, then adding the N-1 curve, N-2 curve, up to the 0th curve.</summary>
    LastToFirst,

    /// <summary>Fit by fixing the Rth curve, then adding the R+1 curve, .. Nth curve. Then adding the R-1 curve, down to the 0th curve.</summary>
    PivotToLastThenToFirst,

    /// <summary>Fit by fixing the Rth curve, then adding the R-1 curve, .. 0th curve. Then adding the R+1 curve, up to the Nth curve.</summary>
    PivotToFirstThenToLast,

    /// <summary>Fit by fixing the Rth curve, then adding the R+1 curve, R-1 curve, up and down to the Nth and 0th curve.</summary>
    PivotToLastAlternating,

    /// <summary>Fit by fixing the Rth curve, then adding the R-1 curve, R+1 curve, down and up to the 0th and Nth curve.</summary>
    PivotToFirstAlternating,
  }
}
