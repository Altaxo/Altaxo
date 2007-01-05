#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Denotes what happens with one side of an axis when the data are changed.
  /// </summary>
  public enum BoundaryRescaling
  {
    /// <summary>
    /// Scale this boundary so that the data fits.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// This axis boundary is set to a fixed value.
    /// </summary>
    Fixed = 1,
    /// <summary>
    /// This axis boundary is set to a fixed value.
    /// </summary>
    Equal = 1,

    /// <summary>
    /// The axis boundary is set to fit the data, but is set not greater than a certain value.
    /// </summary>
    NotGreater  = 2,
    /// <summary>
    /// The axis boundary is set to fit the data, but is set not greater than a certain value.
    /// </summary>
    LessOrEqual = 2,

    /// <summary>
    /// The axis boundary is set to fit the data, but is set not lesser than a certain value.
    /// </summary>
    GreaterOrEqual = 3,

    /// <summary>
    /// The axis boundary is set to fit the data, but is set not lesser than a certain value.
    /// </summary>
    NotLess = 3,

    /// <summary>
    /// The axis boundary is set to use the span from the other axis boundary.
    /// </summary>
    UseSpan=4
  }
}
