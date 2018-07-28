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
  /// The linear array is in column-major order, i.e. the first elements of the linear array belong to the first column of the matrix (i.e. the row values change more quickly).
  /// The index of the linear array is calculated as <c>index = row + column*NumberOfRows</c>. This representation is used for instance by Fortran, Julia, MATLAB, Octave, Scilab, GLSL and HLSL.
  /// </summary>
  /// <typeparam name="TElement">Type of the elements of the matrix.</typeparam>
  public interface IMatrixInArray1DColumnMajorRepresentation<TElement>
  {
    /// <summary>
    /// Gets the underlying linear array in column-major order.
    /// </summary>
    /// <returns>Underlying linear array in column-major order.</returns>
    TElement[] GetArray1DColumnMajor();
  }
}
