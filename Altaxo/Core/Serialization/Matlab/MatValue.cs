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
    public sealed record Scalar(double Value) : MatValue;

    /// <summary>
    /// Represents a 1D double vector.
    /// </summary>
    public sealed record Vector : MatValue
    {
      public Vector<double> Data { get; }

      public Vector(ReadOnlyMemory<double> data)
      {
        Data = CreateVector.Dense<double>(data.ToArray());
      }
    }

    /// <summary>
    /// Represents a 2D double matrix.
    /// </summary>
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
    public sealed record LogicalScalar(bool Value) : MatValue;

    /// <summary>
    /// Represents a logical (boolean) array.
    /// </summary>
    public sealed record LogicalArray(ReadOnlyMemory<bool> Data, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a string value.
    /// </summary>
    public sealed record String(string Value) : MatValue;

    /// <summary>
    /// Represents an n-dimensional numeric array.
    /// </summary>
    public sealed record NumericArray(ReadOnlyMemory<double> Data, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a MATLAB cell array.
    /// </summary>
    public sealed record CellArray(MatValue[] Elements, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a MATLAB struct array.
    /// </summary>
    public sealed record StructArray(IReadOnlyDictionary<string, MatValue[]> Fields, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;

    /// <summary>
    /// Represents a MATLAB object array.
    /// </summary>
    public sealed record ObjectArray(string ClassName, IReadOnlyDictionary<string, MatValue[]> Fields, ReadOnlyMemory<int> Dimensions, bool IsColumnMajor = true) : MatValue;
  }
}
