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
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Represents a unit that refers to a reference quantity. Example: 'Percent of page width' is a unit, which refers to the quantity 'page width' (which has the dimension of length).
  /// Thus this unit is a combination of a dimensionless unit (in the example: 'percent') and the reference quantity (in the example: 'page width'). The dimension of
  /// this unit is equal to the dimension of the reference quantity (i.e. in the above example 'length').
  /// </summary>
  public interface IRelativeUnit : IUnit
  {
    /// <summary>
    /// The corresponding quantity that this unit encapsulates.
    /// </summary>
    /// <value>
    /// The reference quantity whose dimension defines the dimension of this relative unit.
    /// </value>
    DimensionfulQuantity ReferenceQuantity { get; }

    /// <summary>
    /// Calculates the dimensionless prefactor to multiply the <see cref="ReferenceQuantity"/> with.
    /// Example: Given that the relative unit is 'percent of page width', a value of <paramref name="x"/> = 5 is converted to 0.05. The result can then be used
    /// to calculate the absolute quantity by multiplying 0.05 with the page width.
    /// </summary>
    /// <param name="x">Numerical value to convert.</param>
    /// <returns>
    /// The prefactor to multiply the <see cref="ReferenceQuantity"/> with in order to get the absolute quantity.
    /// </returns>
    double GetRelativeValueFromValue(double x);
  }
}
