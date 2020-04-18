#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Designates a biased unit, i.e. a unit for wich 0 units of that unit is not the same as 0 units of the corresponding SI unit.
  /// </summary>
  public interface IBiasedUnit
  {
    /// <summary>
    /// Converts a value of this unit to a value in the corresponding SI unit. The provided value is treated as difference.
    /// Thus if e.g. the unit is DegreesCelsius, by providing 20 the result is 20 (K), because 20°C - 0°C = 20 K.
    /// </summary>
    /// <param name="differenceValue">The value treated as difference.</param>
    /// <returns></returns>
    double ToSIUnitIfTreatedAsDifference(double differenceValue);

    /// <summary>
    /// Adds the biased value of this unit and an SI value to get the biased value of this unit. For example
    /// 20°C + 20 K results in 40°C or 20 °F + 20 K results in 52 °F
    /// </summary>
    /// <param name="biasedValueOfThisUnit">The biased value of this unit.</param>
    /// <param name="siValue">The si value.</param>
    /// <returns></returns>
    double AddBiasedValueOfThisUnitAndSIValue(double biasedValueOfThisUnit, double siValue);
  }
}
