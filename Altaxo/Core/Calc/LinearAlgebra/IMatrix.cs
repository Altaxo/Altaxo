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
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// IROMatrix represents a read-only matrix of values.
  /// </summary>
  public interface IROMatrix<T> where T : struct
  {
    /// <summary>Gets an element of the matrix at (row, col).</summary>
    T this[int row, int col] { get; }

    /// <summary>The number of rows of the matrix.</summary>
    int RowCount { get; }

    /// <summary>The number of columns of the matrix.</summary>
    int ColumnCount { get; }
  }

  /// <summary>
  /// IMatrix represents the simplest form of a 2D matrix, which is readable and writeable.
  /// </summary>
  public interface IMatrix<T> : IROMatrix<T> where T : struct
  {
    /// <summary>Get / sets an element of the matrix at (row, col).</summary>
    new T this[int row, int col] { get; set; }
  }

  /// <summary>
  /// IRightExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended to the right of the matrix.
  /// </summary>
  public interface IRightExtensibleMatrix<T> : IMatrix<T> where T : struct
  {
    /// <summary>
    /// Append matrix a to the right edge of this matrix. Matrix a must have the same number of rows than this matrix, except this matrix
    /// is still empty, in which case the right dimension of this matrix is set.
    /// </summary>
    /// <param name="a">The matrix to append.</param>
    void AppendRight(IROMatrix<T> a);
  }

  /// <summary>
  /// IBottomExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended to the bottom of the matrix.
  /// </summary>
  public interface IBottomExtensibleMatrix<T> : IMatrix<T> where T : struct
  {
    /// <summary>
    /// Append matrix a to the bottom of this matrix. Matrix a must have the same number of columns than this matrix, except this matrix
    /// is still empty, in which case the right dimension of this matrix is set.
    /// </summary>
    /// <param name="a">The matrix to append.</param>
    void AppendBottom(IROMatrix<T> a);
  }

  /// <summary>
  /// IExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended either to the right or to the bottom of the matrix.
  /// </summary>
  public interface IExtensibleMatrix<T> : IRightExtensibleMatrix<T>, IBottomExtensibleMatrix<T> where T : struct
  {
  }

  public interface IROBandMatrix<T> : IROMatrix<T> where T : struct
  {
    int LowerBandwidth { get; }
    int UpperBandwidth { get; }

    IEnumerable<(int row, int column, T value)> EnumerateElementsIndexed(Zeros zeros = Zeros.AllowSkip);
  }

  public interface IROSparseMatrix<T> : IROMatrix<T> where T : struct
  {
  }

  /// <summary>
  /// Operations on matrices which do not change the matrix instance.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IROMatrixLevel1<T> : IROMatrix<T> where T : struct
  {
    IEnumerable<(int row, int column, T value)> EnumerateElementsIndexed(Zeros zeros = Zeros.AllowSkip);

    /// <summary>
    /// Elementwise mapping of a function to the elements of a matrix, and storing the result in another matrix.
    /// </summary>
    /// <param name="function">The function to apply. First arg in the row index, 2nd arg the column index, and 3rd arg the matrix element.</param>
    /// <param name="result">The matrix where to store the result.</param>
    /// <param name="zeros">Designates if zero elements (i.e. banded or sparse matrices) are allowed to omit in the mapping.</param>
    void MapIndexed(Func<int, int, T, T> function, IMatrix<T> result, Zeros zeros = Zeros.AllowSkip);

    /// <summary>
    /// Elementwise mapping of a function to the elements of a matrix, and storing the result in another matrix.
    /// </summary>
    /// <param name="sourceParameter1">Additional auxilary parameter to be passed to the function.</param>
    /// <param name="function">The function to apply. First arg in the row index, 2nd arg the column index, 3rd arg the matrix element, and 4th arg the parameter given in <paramref name="sourceParameter1"/>.</param>
    /// <param name="result">The matrix where to store the result.</param>
    /// <param name="zeros">Designates if zero elements (i.e. banded or sparse matrices) are allowed to omit in the mapping.</param>
    void MapIndexed<T1>(T1 sourceParameter1, Func<int, int, T, T1, T> function, IMatrix<T> result, Zeros zeros = Zeros.AllowSkip);
  }

  public interface IMatrixLevel1<T> : IROMatrixLevel1<T> where T : struct
  {
    /// <summary>
    /// Sets all elements of the matrix to the default value (i.e. zero for numerical values).
    /// </summary>
    void Clear();

    /// <summary>
    /// Copies elements from another matrix.
    /// </summary>
    /// <param name="from">From.</param>
    void CopyFrom(IROMatrix<T> from);
  }
}
