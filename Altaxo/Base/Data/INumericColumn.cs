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

#nullable enable
using System;

namespace Altaxo.Data
{
  /// <summary>
  /// Represents a column whose elements can be treated as numeric values.
  /// This includes integer and floating-point columns and also <see cref="DateTime"/>-based columns that can be converted to seconds since a reference date.
  /// </summary>
  public interface INumericColumn : IReadableColumn, ICloneable, Altaxo.Calc.LinearAlgebra.INumericSequence<double>
  {
    /// <summary>
    /// Returns the value of a column element at index i as numeric value (double).
    /// </summary>
    /// <param name="i">The index to the column element.</param>
    /// <returns>The value of the column element as double value.</returns>
    new double this[int i] { get; }
  }
}
