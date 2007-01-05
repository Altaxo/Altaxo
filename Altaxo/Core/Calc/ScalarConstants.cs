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

namespace Altaxo.Calc
{
  /// <summary>
  /// Contains scalar constants.
  /// </summary>
  public class DoubleConstants
  {
    /// <summary>
    /// Represents the smallest positive number where 1+DBL_EPSILON is not equal to 1.
    /// </summary>
    public const double DBL_EPSILON = 2.2204460492503131e-016;
    /// <summary>
    /// The smallest positive double number.
    /// </summary>
    public const double DBL_MIN     = double.Epsilon;
    /// <summary>
    /// The biggest positive double number.
    /// </summary>
    public const double DBL_MAX     = double.MaxValue;


    /// <summary>
    /// The value 2/sqrt(Pi).
    /// </summary>
    public const double M_2_SQRTPI = 1.1283791670955125738961589031216;


    /// <summary>
    /// Square root of Pi.
    /// </summary>
    public const double sqrtpi = 1.77245385090551602729816748334115;

    /// <summary>
    /// Square root of Epsilon.
    /// </summary>
    private static readonly double sqeps  = Math.Sqrt(DBL_EPSILON);


  }
}
