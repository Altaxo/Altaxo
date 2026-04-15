#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Serialization.Matlab
{
  /// <summary>
  /// Base type for values imported from MATLAB MAT-files.
  /// </summary>
  public abstract record MatValue
  {
    /// <summary>
    /// Represents a scalar double value.
    /// </summary>
    /// <param name="Value">The scalar value.</param>
    public sealed record Scalar(double Value) : MatValue;

    /// <summary>
    /// Represents a 1D double vector.
    /// </summary>
    public sealed record Vector : MatValue
    {
      /// <summary>
      /// Gets the vector data.
      /// </summary>
      public Vector<double> Data { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="Vector"/> record.
      /// </summary>
      /// <param name="data">The vector data.</param>
      public Vector(ReadOnlyMemory<double> data)
      {
        Data = CreateVector.Dense<double>(data.ToArray());
      }
    }

    /// <summary>
    /// Represents a 2D double matrix.
    /// </summary>
    /// <param name="RowCount">The number of rows in the matrix.</param>
    /// <param name="ColumnCount">The number of columns in the matrix.</param>
    /// <param name="Data">The matrix elements stored in a contiguous memory block.</param>
    /// <param name="IsColumnMajor">A value indicating whether the matrix data is stored in column-major order.</param>
    public sealed record Matrix(int RowCount, int ColumnCount, ReadOnlyMemory<double> Data, bool IsColumnMajor = true) : MatValue, IROMatrix<double>
    {
      /// <summary>
      /// Gets the matrix value at the given row and column.
      /// </summary>
      /// <param name="row">Zero-based row index.</param>
      /// <param name="column">Zero-based column index.</param>
      public double this[int row, int column]
      {
        get
        {
          if ((uint)row >= (uint)RowCount)
            throw new ArgumentOutOfRangeException(nameof(row));
          if ((uint)column >= (uint)ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(column));

          var idx = IsColumnMajor ? (row + RowCount * column) : (column + ColumnCount * row);
          return Data.Span[idx];
        }
      }
    }

    /// <summary>
    /// Represents a scalar logical (boolean) value.
    /// </summary>
    /// <param name="Value">The logical value.</param>
    public sealed record LogicalScalar(bool Value) : MatValue;

    /// <summary>
    /// Represents a logical (boolean) array.
    /// </summary>
    /// <param name="Data">The logical values of the array.</param>
    /// <param name="Dimensions">The dimensions of the array.</param>
    /// <param name="IsColumnMajor">A value indicating whether the array data is stored in column-major order.</param>
    public sealed record LogicalArray(ReadOnlyMemory<bool> Data, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a string value.
    /// </summary>
    /// <param name="Value">The string value.</param>
    public sealed record String(string Value) : MatValue;

    /// <summary>
    /// Represents an n-dimensional numeric array.
    /// </summary>
    /// <param name="Data">The numeric values of the array.</param>
    /// <param name="Dimensions">The dimensions of the array.</param>
    /// <param name="IsColumnMajor">A value indicating whether the array data is stored in column-major order.</param>
    public sealed record NumericArray(ReadOnlyMemory<double> Data, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a MATLAB cell array.
    /// </summary>
    /// <param name="Elements">The cell elements stored in linearized order.</param>
    /// <param name="Dimensions">The dimensions of the cell array.</param>
    /// <param name="IsColumnMajor">A value indicating whether the element order follows MATLAB column-major layout.</param>
    public sealed record CellArray(MatValue[] Elements, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a MATLAB struct array.
    /// </summary>
    /// <param name="Fields">The field data keyed by field name.</param>
    /// <param name="Dimensions">The dimensions of the struct array.</param>
    /// <param name="IsColumnMajor">A value indicating whether the element order follows MATLAB column-major layout.</param>
    public sealed record StructArray(IReadOnlyDictionary<string, MatValue[]> Fields, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a MATLAB object array.
    /// </summary>
    /// <param name="ClassName">The MATLAB class name of the object array.</param>
    /// <param name="Fields">The field data keyed by field name.</param>
    /// <param name="Dimensions">The dimensions of the object array.</param>
    /// <param name="IsColumnMajor">A value indicating whether the element order follows MATLAB column-major layout.</param>
    public sealed record ObjectArray(string ClassName, IReadOnlyDictionary<string, MatValue[]> Fields, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;
  }
}
