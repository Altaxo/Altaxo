#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Designates that the matrix is represented as a linear array of <typeparamref name="TElement"/> values.
  /// The array is in row-major order, i.e. the first elements of the linear array belong to the first row of the matrix (the column values change more quickly).
  /// The index of the linear array is calculated as <c>index = column + row*NumberOfColumns</c>.
  /// This representation is used for instance by C, C++, Mathematica, Pascal and Python.
  /// </summary>
  /// <typeparam name="TElement">Type of the elements of the matrix.</typeparam>
  public interface IMatrixInArray1DRowMajorRepresentation<TElement>
  {
    /// <summary>
    /// Gets the underlying linear array in row-major order.
    /// </summary>
    /// <returns>Underlying linear array in row-major order.</returns>
    TElement[] GetArray1DRowMajor();
  }
}
