#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Calc.Regression.PLS
{
	
  /// <summary>
  /// Determines how to do the calculation of Cross Validated Predicted Error Sum of Squares.
  /// </summary>
  public enum CrossPRESSCalculationType
  {
    /// <summary>
    /// No cross PRESS calculation.
    /// </summary>
    None,

    /// <summary>
    /// Every measurement is excluded to calculate Cross PRESS.
    /// </summary>
    ExcludeEveryMeasurement,

    /// <summary>
    /// Measurements (which have the same concentration values) are excluded as groups to calculate Cross PRESS.
    /// </summary>
    ExcludeGroupsOfSimilarMeasurements,


  }
}
