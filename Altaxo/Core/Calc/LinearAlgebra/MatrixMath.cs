#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Class MatrixMath provides common static methods for matrix manipulation
  /// and arithmetic in tow dimensions.
  /// </summary>
  public static partial class MatrixMath
  {
    #region Helper functions

    /// <summary>
    /// Calculates the Square of the value x.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>x*x.</returns>
    public static double Square(double x)
    {
      return x * x;
    }

    /// <summary>
    /// Calculates the hypotenuse length of a and b, i.e. the sqrt(a^2+b^2), avoiding overflow at large values.
    /// </summary>
    /// <param name="a">First parameter.</param>
    /// <param name="b">Second parameter.</param>
    /// <returns>The square root of (a^2+b^2).</returns>
    public static double Hypotenuse(double a, double b)
    {
      if (Math.Abs(a) > Math.Abs(b))
      {
        double r = b / a;
        return Math.Abs(a) * Math.Sqrt(1 + r * r);
      }

      if (b != 0)
      {
        double r = a / b;
        return Math.Abs(b) * Math.Sqrt(1 + r * r);
      }

      return 0.0;
    }

    /// <summary>
    /// Allocates an array of n x m values.
    /// </summary>
    /// <param name="n">First matrix dimension.</param>
    /// <param name="m">Second matrix dimension.</param>
    /// <returns>Array of dimensions n x m.</returns>
    public static double[][] GetMatrixArray(int n, int m)
    {
      double[][] result = new double[n][];
      for (int i = 0; i < n; i++)
        result[i] = new double[m];

      return result;
    }

    public static string MatrixToString<T>(string? name, IROMatrix<T> a) where T : struct
    {
      name = name ?? string.Empty;

      if (a.RowCount == 0 || a.ColumnCount == 0)
        return string.Format("EmptyMatrix {0}({1},{2})", name, a.RowCount, a.ColumnCount);

      var s = new StringBuilder();
      s.Append("Matrix " + name + ":");
      for (int i = 0; i < a.RowCount; i++)
      {
        s.Append("\n(");
        for (int j = 0; j < a.ColumnCount; j++)
        {
          s.Append(a[i, j]);
          if (j + 1 < a.ColumnCount)
            s.Append(";");
          else
            s.Append(")");
        }
      }
      return s.ToString();
    }

    public static string MatrixToString(string name, IROMatrix<float> a)
    {
      if (null == name)
        name = "";

      if (a.RowCount == 0 || a.ColumnCount == 0)
        return string.Format("EmptyMatrix {0}({1},{2})", name, a.RowCount, a.ColumnCount);

      var s = new System.Text.StringBuilder();
      s.Append("Matrix " + name + ":");
      for (int i = 0; i < a.RowCount; i++)
      {
        s.Append("\n(");
        for (int j = 0; j < a.ColumnCount; j++)
        {
          s.Append(a[i, j].ToString());
          if (j + 1 < a.ColumnCount)
            s.Append(";");
          else
            s.Append(")");
        }
      }
      return s.ToString();
    }

    public static string MatrixToString(string name, IROComplexDoubleMatrix a)
    {
      if (null == name)
        name = "";

      if (a.Rows == 0 || a.Columns == 0)
        return string.Format("EmptyMatrix {0}({1},{2})", name, a.Rows, a.Columns);

      var s = new System.Text.StringBuilder();
      s.Append("Matrix " + name + ":");
      for (int i = 0; i < a.Rows; i++)
      {
        s.Append("\n(");
        for (int j = 0; j < a.Columns; j++)
        {
          s.Append(a[i, j].ToString());
          if (j + 1 < a.Columns)
            s.Append(";");
          else
            s.Append(")");
        }
      }
      return s.ToString();
    }

    public static string MatrixToString(string name, IROComplexFloatMatrix a)
    {
      if (null == name)
        name = "";

      if (a.Rows == 0 || a.Columns == 0)
        return string.Format("EmptyMatrix {0}({1},{2})", name, a.Rows, a.Columns);

      var s = new System.Text.StringBuilder();
      s.Append("Matrix " + name + ":");
      for (int i = 0; i < a.Rows; i++)
      {
        s.Append("\n(");
        for (int j = 0; j < a.Columns; j++)
        {
          s.Append(a[i, j].ToString());
          if (j + 1 < a.Columns)
            s.Append(";");
          else
            s.Append(")");
        }
      }
      return s.ToString();
    }

    #endregion Helper functions

    #region Type conversion

    /// <summary>
    /// This wraps a jagged double array to the <see cref="IROMatrix{Double}" /> interface. The data is not copied!
    /// </summary>
    /// <param name="x">The jagged array. Each double[] vector is a row of the matrix.</param>
    /// <returns></returns>
    public static IBottomExtensibleMatrix<T> ToROMatrixFromLeftSpineJaggedArray<T>(T[][] x) where T : struct
    {
      return new LeftSpineJaggedArrayMatrix<T>(x);
    }

    /// <summary>
    /// This wraps a jagged double array to the <see cref="IMatrix{Double}" /> interface. The data is not copied!
    /// </summary>
    /// <param name="x">The jagged array. Each double[] vector is a row of the matrix.</param>
    /// <returns></returns>
    public static IBottomExtensibleMatrix<T> ToMatrixFromLeftSpineJaggedArray<T>(T[][] x) where T : struct
    {
      return new LeftSpineJaggedArrayMatrix<T>(x);
    }

    public static IBottomExtensibleMatrix<T> ToMatrix<T>(MatrixWrapperStructForLeftSpineJaggedArray<T> wrapper) where T : struct
    {
      return new LeftSpineJaggedArrayMatrix<T>(wrapper);
    }

    /// <summary>
    /// Constructs an RE matrix from an array of double vectors. Attention! The double vectors (the second) dimensions are here
    /// the columns (!) of the matrix. The data is not copied.
    /// </summary>
    /// <param name="x">Array of columns (!) of the matrix.</param>
    public static IRightExtensibleMatrix<T> ToROMatrixFromTopSpineJaggedArray<T>(T[][] x) where T : struct
    {
      return new TopSpineJaggedArrayMatrix<T>(x);
    }

    /// <summary>
    /// Constructs an RE matrix from an array of double vectors. Attention! The double vectors (the second) dimensions are here
    /// the columns (!) of the matrix. The data is not copied.
    /// </summary>
    /// <param name="x">Array of columns (!) of the matrix.</param>
    public static IRightExtensibleMatrix<T> ToMatrixFromTopSpineJaggedArray<T>(T[][] x) where T : struct
    {
      return new TopSpineJaggedArrayMatrix<T>(x);
    }

    /// <summary>
    /// Constructs an RE matrix from an array of double vectors. Attention! The double vectors (the second) dimensions are here
    /// the columns (!) of the matrix. The data is not copied.
    /// </summary>
    /// <param name="wrapper">Wrapper around a top spine jagged array matrix.</param>
    public static IRightExtensibleMatrix<T> ToMatrix<T>(MatrixWrapperStructForTopSpineJaggedArray<T> wrapper) where T : struct
    {
      return new TopSpineJaggedArrayMatrix<T>(wrapper);
    }

    /// <summary>
    /// Wraps a linear array (column major order) into a read-only matrix.
    /// The array is packed in column major order, i.e. the first elements belong to the first column of the matrix.
    /// </summary>
    /// <param name="arrayInColumMajorOrder">Linear array in column major order. The length has to be a multiple of <c>nRows</c>.</param>
    /// <param name="rows">Number of rows of the resulting matrix.</param>
    /// <returns>The read-only matrix wrappage of the linear array.</returns>
    public static IROMatrix<T> ToROMatrixFromColumnMajorLinearArray<T>(T[] arrayInColumMajorOrder, int rows) where T : struct
    {
      return new ROMatrixFromColumnMajorLinearArray<T>(arrayInColumMajorOrder, rows);
    }

    /// <summary>
    /// Wraps a linear array (column major order) into a read-only matrix.
    /// The array is packed in column major order, i.e. the first elements belong to the first column of the matrix.
    /// </summary>
    /// <param name="wrapper">Wrapper around a linear array in column major order, which provides number of rows and columns.</param>
    /// <returns>The read-only matrix wrappage of the linear array.</returns>
    public static IROMatrix<T> ToROMatrix<T>(MatrixWrapperStructForColumnMajorOrderLinearArray<T> wrapper) where T : struct
    {
      return new ROMatrixFromColumnMajorLinearArray<T>(wrapper);
    }

    /// <summary>
    /// Wraps a linear array into a read-write matrix. The array is packed column-wise, i.e. the first elements belong to the first column of the matrix.
    /// </summary>
    /// <param name="x">Linear array. The length has to be a multiple of <c>nRows</c>.</param>
    /// <param name="nRows">Number of rows of the resulting matrix.</param>
    /// <returns>The read-only matrix wrappage of the linear array.</returns>
    public static IMatrix<T> ToMatrixFromColumnMajorLinearArray<T>(T[] x, int nRows) where T : struct
    {
      return new MatrixFromColumnMajorLinearArray<T>(x, nRows);
    }

    /// <summary>
    /// Wraps a linear array (column major order) into a read-write matrix.
    /// The array is packed in column major order, i.e. the first elements belong to the first column of the matrix.
    /// </summary>
    /// <param name="wrapper">Wrapper around a linear array in column major order, which provides number of rows and columns.</param>
    /// <returns>The read-only matrix wrappage of the linear array.</returns>
    public static IROMatrix<T> ToMatrix<T>(MatrixWrapperStructForColumnMajorOrderLinearArray<T> wrapper) where T : struct
    {
      return new ROMatrixFromColumnMajorLinearArray<T>(wrapper);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    public static IROVector<T> RowToROVector<T>(IROMatrix<T> x, int row) where T : struct
    {
      return new MatrixRowROVector<T>(x, row);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    /// <param name="columnoffset">The column of the matrix that corresponds to the first element of the vector.</param>
    /// <param name="length">The length of the resulting vector.</param>
    public static IROVector<T> RowToROVector<T>(IROMatrix<T> x, int row, int columnoffset, int length) where T : struct
    {
      return new MatrixRowROVector<T>(x, row, columnoffset, length);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    public static IVector<T> RowToVector<T>(IMatrix<T> x, int row) where T : struct
    {
      return new MatrixRowVector<T>(x, row);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    /// <param name="columnoffset">The column of the matrix that corresponds to the first element of the vector.</param>
    /// <param name="length">The length of the resulting vector.</param>
    public static IVector<T> RowToVector<T>(IMatrix<T> x, int row, int columnoffset, int length) where T : struct
    {
      return new MatrixRowVector<T>(x, row, columnoffset, length);
    }

    /// <summary>
    /// Returns a read-only vector representing a matrix column by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="column">The column number of the matrix that is wrapped to a vector.</param>
    public static IROVector<T> ColumnToROVector<T>(IROMatrix<T> x, int column) where T : struct
    {
      return new MatrixColumnROVector<T>(x, column);
    }

    /// <summary>
    /// Returns a vector representing a matrix column by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="column">The column number of the matrix that is wrapped to a vector.</param>
    public static IVector<T> ColumnToVector<T>(IMatrix<T> x, int column) where T : struct
    {
      return new MatrixColumnVector<T>(x, column);
    }

    /// <summary>
    /// Wraps a submatrix part, so that this part can be used as a matrix in operations (read-only).
    /// </summary>
    /// <param name="matrix">The matrix from which a submatrix part should be wrapped.</param>
    /// <param name="rowOffset">Starting row of the submatrix.</param>
    /// <param name="columnOffset">Starting column of the submatrix.</param>
    /// <param name="rows">Number of rows of the submatrix.</param>
    /// <param name="columns">Number of columns of the submatrix.</param>
    /// <returns>A read-only wrapper matrix that represents the submatrix part of the matrix.</returns>
    public static IROMatrix<T> ToROSubMatrix<T>(IROMatrix<T> matrix, int rowOffset, int columnOffset, int rows, int columns) where T : struct
    {
      return new SubMatrixROWrapper<T>(matrix, rowOffset, columnOffset, rows, columns);
    }

    /// <summary>
    /// Wraps a submatrix part, so that this part can be used as a matrix in operations.
    /// </summary>
    /// <param name="matrix">The matrix from which a submatrix part should be wrapped.</param>
    /// <param name="rowoffset">Starting row of the submatrix.</param>
    /// <param name="columnoffset">Starting column of the submatrix.</param>
    /// <param name="rows">Number of rows of the submatrix.</param>
    /// <param name="columns">Number of columns of the submatrix.</param>
    /// <returns>A wrapper matrix that represents the submatrix part of the matrix.</returns>
    public static IMatrix<T> ToSubMatrix<T>(IMatrix<T> matrix, int rowoffset, int columnoffset, int rows, int columns) where T : struct
    {
      return new SubMatrixWrapper<T>(matrix, rowoffset, columnoffset, rows, columns);
    }

    /// <summary>
    /// Wraps a read-only vector to a read-only diagonal matrix.
    /// </summary>
    /// <param name="vector">The vector to wrap.</param>
    /// <param name="vectoroffset">The index of the vector that is the first matrix element(0,0).</param>
    /// <param name="matrixdimensions">The number of rows = number of columns of the diagonal matrix.</param>
    /// <returns></returns>
    public static IROMatrix<T> ToRODiagonalMatrix<T>(IReadOnlyList<T> vector, int vectoroffset, int matrixdimensions) where T : struct
    {
      return new RODiagonalMatrixVectorWrapper<T>(vector, vectoroffset, matrixdimensions);
    }

    #endregion Type conversion

    #region Clear

    public static void Clear<T>(IMatrix<T> matrix) where T : struct
    {
      if (matrix is IMatrixLevel1<T> l1)
      {
        l1.Clear();
      }
      else
      {
        Clear_DefaultImpl(matrix);
      }
    }

    public static void Clear_DefaultImpl<T>(IMatrix<T> matrix) where T : struct
    {
      var rowCount = matrix.RowCount;
      var columnCount = matrix.ColumnCount;
      for (int i = 0; i < rowCount; ++i)
        for (int j = 0; j < columnCount; ++j)
        {
          matrix[i, j] = default;
        }
    }

    #endregion Clear

    #region Addition, Subtraction, Multiply and combined operations

    /// <summary>
    /// Multiplies matrix a with matrix b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void Multiply(IROMatrix<double> a, IROMatrix<double> b, IMatrix<double> c)
    {
      int crows = a.RowCount; // the rows of resultant matrix
      int ccols = b.ColumnCount; // the cols of resultant matrix
      int numil = b.RowCount; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));
      if (c.RowCount != crows || c.ColumnCount != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", c.RowCount, c.ColumnCount, crows, ccols));

      for (int i = 0; i < crows; i++)
      {
        for (int j = 0; j < ccols; j++)
        {
          double sum = 0;
          for (int k = 0; k < numil; k++)
            sum += a[i, k] * b[k, j];

          c[i, j] = sum;
        }
      }
    }

    /// <summary>
    /// Multiplies matrix a with vector b and stores the result in vector c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant. The vector must have the length of a.Columns.</param>
    /// <param name="c">The vector where to store the result. Has to be of dimension (a.Rows).</param>
    public static void Multiply(IROMatrix<double> a, IReadOnlyList<double> b, IVector<double> c)
    {
      int crows = a.RowCount; // the rows of resultant matrix
      int numil = b.Count; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with vector of dim({2}) is not possible!", a.RowCount, a.ColumnCount, b.Count));
      if (c.Length != crows)
        throw new ArithmeticException(string.Format("The provided resultant vector (actual dim({0}))has not the expected dimension ({1})", c.Length, crows));

      for (int i = 0; i < crows; i++)
      {
        double sum = 0;
        for (int k = 0; k < numil; k++)
          sum += a[i, k] * b[k];

        c[i] = sum;
      }
    }

    /// <summary>
    /// Multiplies matrix a with vector b from left and right: b* A b.
    /// </summary>
    /// <param name="a">Matrix. Must be a square matrix with both number of rows and columns the same as the vector length.</param>
    /// <param name="b">Vector. The vector must have the length as the rows and columns of the matrix.</param>
    /// <result>The product b* A b.</result>
    public static double MultiplyVectorFromLeftAndRight(IROMatrix<double> a, IReadOnlyList<double> b)
    {
      int numil = b.Count; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if (a.RowCount != a.ColumnCount)
        throw new ArgumentException("Matrix a has to be a square matrix");
      if (a.ColumnCount != numil)
        throw new ArgumentException(string.Format("The length of the vector({2}) has to match the number of columns of the matrix({0},{1}).", a.RowCount, a.ColumnCount, b.Count));

      double result = 0;
      for (int i = 0; i < numil; i++)
      {
        double sum = 0;
        for (int k = 0; k < numil; k++)
          sum += a[i, k] * b[k];

        result += b[i] * sum;
      }
      return result;
    }

    /// <summary>
    /// Multiplies matrix a_transposed with matrix b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void MultiplyFirstTransposed(IROMatrix<double> a, IROMatrix<double> b, IMatrix<double> c)
    {
      int crows = a.ColumnCount; // the rows of resultant matrix
      int ccols = b.ColumnCount; // the cols of resultant matrix
      int numil = b.RowCount; // number of summands for most inner loop

      // Presumtion:
      if (a.RowCount != numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a transposed matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));
      if (c.RowCount != crows || c.ColumnCount != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", c.RowCount, c.ColumnCount, crows, ccols));

      for (int i = 0; i < crows; i++)
      {
        for (int j = 0; j < ccols; j++)
        {
          double sum = 0;
          for (int k = 0; k < numil; k++)
            sum += a[k, i] * b[k, j];

          c[i, j] = sum;
        }
      }
    }

    /// <summary>
    /// Multiplies matrix a_transposed with vector b and stores the result in vector c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void MultiplyFirstTransposed(IROMatrix<double> a, IReadOnlyList<double> b, IVector<double> c)
    {
      int crows = a.ColumnCount; // the rows of resultant vector
      int numil = a.RowCount;

      // Presumtion:
      if (a.RowCount != b.Count)
        throw new ArithmeticException(string.Format("Try to multiplicate a transposed matrix of dim({0},{1}) with vector of dim({2}) is not possible!", a.RowCount, a.ColumnCount, b.Count));
      if (crows != c.Length)
        throw new ArithmeticException(string.Format("The provided resultant vector (actual dim({0}))has not the expected dimension ({1})", c.Length, crows));

      for (int i = 0; i < crows; i++)
      {
        double sum = 0;
        for (int k = 0; k < numil; k++)
          sum += a[k, i] * b[k];

        c[i] = sum;
      }
    }

    /// <summary>
    /// Multiplies matrix a with matrix b_transposed and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void MultiplySecondTransposed(IROMatrix<double> a, IROMatrix<double> b, IMatrix<double> c)
    {
      int crows = a.RowCount; // the rows of resultant matrix
      int ccols = b.RowCount; // the cols of resultant matrix
      int numil = b.ColumnCount; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));
      if (c.RowCount != crows || c.ColumnCount != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", c.RowCount, c.ColumnCount, crows, ccols));

      for (int i = 0; i < crows; i++)
      {
        for (int j = 0; j < ccols; j++)
        {
          double sum = 0;
          for (int k = 0; k < numil; k++)
            sum += a[i, k] * b[j, k];

          c[i, j] = sum;
        }
      }
    }

    /// <summary>
    /// Multiplies the matrix a with a scalar value b and stores the result in c. Matrix a and c are allowed to be the same matrix.
    /// </summary>
    /// <param name="a">The first multiplicant.</param>
    /// <param name="b">The second multiplicant.</param>
    /// <param name="c">The resulting matrix.</param>
    public static void MultiplyScalar(IROMatrix<double> a, double b, IMatrix<double> c)
    {
      if (c.RowCount != a.RowCount || c.ColumnCount != a.ColumnCount)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1})) has not the expected dimension ({2},{3})", c.RowCount, c.ColumnCount, a.RowCount, a.ColumnCount));

      for (int i = 0; i < a.RowCount; i++)
      {
        for (int j = 0; j < a.ColumnCount; j++)
        {
          c[i, j] = a[i, j] * b;
        }
      }
    }

    /// <summary>
    /// Multiplies the row <c>rowb</c> of matrix b element by element to all rows of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to multiply.</param>
    /// <param name="brow">The row number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void MultiplyRow(IROMatrix<double> a, IROMatrix<double> b, int brow, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int brows = b.RowCount;
      int bcols = b.ColumnCount;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!", brows, bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] * b[brow, j];
    }

    /// <summary>
    /// Multiplies the row <c>rowb</c> of matrix b element by element to all rows of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The vector which contains the row to multiply.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void MultiplyRow(IROMatrix<double> a, IReadOnlyList<double> b, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int brows = 1;
      int bcols = b.Count;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Vector b[{0}] has not the same number of columns than matrix a[{1},{2}]!", brows, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Vector b and matrix c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] * b[j];
    }

    /// <summary>
    /// Calculates a+b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First matrix to add..</param>
    /// <param name="b">Second operand..</param>
    /// <param name="c">The resultant matrix a+b. Has to be of same dimension as a and b.</param>
    public static void Add(IROMatrix<double> a, IROMatrix<double> b, IMatrix<double> c)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != b.ColumnCount || a.RowCount != b.RowCount)
        throw new ArithmeticException(string.Format("Try to add a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));
      if (c.RowCount != a.RowCount || c.ColumnCount != a.ColumnCount)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the proper dimension ({2},{3})", c.RowCount, c.ColumnCount, a.RowCount, a.ColumnCount));

      for (int i = 0; i < c.RowCount; i++)
        for (int j = 0; j < c.ColumnCount; j++)
          c[i, j] = a[i, j] + b[i, j];
    }

    /// <summary>
    /// Calculates a-b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">Minuend.</param>
    /// <param name="b">Subtrahend.</param>
    /// <param name="c">The resultant matrix a-b. Has to be of same dimension as a and b.</param>
    public static void Subtract(IROMatrix<double> a, IROMatrix<double> b, IMatrix<double> c)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != b.ColumnCount || a.RowCount != b.RowCount)
        throw new ArithmeticException(string.Format("Try to subtract a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));
      if (c.RowCount != a.RowCount || c.ColumnCount != a.ColumnCount)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the proper dimension ({2},{3})", c.RowCount, c.ColumnCount, a.RowCount, a.ColumnCount));

      for (int i = 0; i < c.RowCount; i++)
        for (int j = 0; j < c.ColumnCount; j++)
          c[i, j] = a[i, j] - b[i, j];
    }

    /// <summary>
    /// Calculates c = c - ab
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to subtract the result of the multipication from. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void SubtractProductFromSelf(IROMatrix<double> a, IROMatrix<double> b, IMatrix<double> c)
    {
      int crows = a.RowCount; // the rows of resultant matrix
      int ccols = b.ColumnCount; // the cols of resultant matrix
      int numil = b.RowCount; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));
      if (c.RowCount != crows || c.ColumnCount != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", c.RowCount, c.ColumnCount, crows, ccols));

      for (int i = 0; i < crows; i++)
      {
        for (int j = 0; j < ccols; j++)
        {
          double sum = 0;
          for (int k = 0; k < numil; k++)
            sum += a[i, k] * b[k, j];

          c[i, j] -= sum;
        }
      }
    }

    /// <summary>
    /// Calculates c = c - ab
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to subtract the result of the multipication from. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void SubtractProductFromSelf(IROMatrix<double> a, double b, IMatrix<double> c)
    {
      int crows = a.RowCount; // the rows of resultant matrix
      int ccols = a.ColumnCount; // the cols of resultant matrix

      // Presumtion:
      if (c.RowCount != crows || c.ColumnCount != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", c.RowCount, c.ColumnCount, crows, ccols));

      for (int i = 0; i < crows; i++)
      {
        for (int j = 0; j < ccols; j++)
        {
          c[i, j] -= b * a[i, j];
        }
      }
    }

    /// <summary>
    /// Add the vector <c>b</c>  to all rows of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The vector to add.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to vector b).</param>
    public static void AddRow(IROMatrix<double> a, IReadOnlyList<double> b, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int bcols = b.Count;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Vector b[{0}] has not the same length than rows of matrix a[{1},{2}]!", bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Vector b and Matrix c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] + b[j];
    }

    /// <summary>
    /// Add the row <c>rowb</c> of matrix b to all rows of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to add.</param>
    /// <param name="brow">The row number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void AddRow(IROMatrix<double> a, IROMatrix<double> b, int brow, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int brows = b.RowCount;
      int bcols = b.ColumnCount;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!", brows, bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] + b[brow, j];
    }

    /// <summary>
    /// Subtracts the row <c>rowb</c> of matrix b from all rows of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to subtract.</param>
    /// <param name="brow">The row number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void SubtractRow(IROMatrix<double> a, IROMatrix<double> b, int brow, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int brows = b.RowCount;
      int bcols = b.ColumnCount;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!", brows, bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] - b[brow, j];
    }

    /// <summary>
    /// Subtracts the row <c>rowb</c> of matrix b from all rows of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The vector which contains the row to subtract.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void SubtractRow(IROMatrix<double> a, IReadOnlyList<double> b, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int bcols = b.Count;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Vector b[{0}] has not the same length than rows of matrix a[{1},{2}]!", bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Vector b and Matrix c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] - b[j];
    }

    /// <summary>
    /// Subtracts the column <c>bcol</c> of matrix b from all columns of matrix a.
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to subtract.</param>
    /// <param name="bcol">The column number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void SubtractColumn(IROMatrix<double> a, IROMatrix<double> b, int bcol, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int brows = b.RowCount;
      int bcols = b.ColumnCount;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (brows != arows)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of rows than matrix a[{2},{3}]!", brows, bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
          c[i, j] = a[i, j] - b[i, bcol];
    }

    /// <summary>
    /// Divides all rows of matrix a by the row <c>rowb</c> of matrix b (element by element).
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the denominator row.</param>
    /// <param name="brow">The row number of matrix b which serves as denominator.</param>
    /// <param name="resultIfNull">If the denominator is null, the result is set to this number.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void DivideRow(IROMatrix<double> a, IROMatrix<double> b, int brow, double resultIfNull, IMatrix<double> c)
    {
      int arows = a.RowCount;
      int acols = a.ColumnCount;

      int brows = b.RowCount;
      int bcols = b.ColumnCount;

      int crows = c.RowCount;
      int ccols = c.ColumnCount;

      // Presumtion:
      if (arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})", crows, ccols, arows, acols));
      if (bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!", brows, bcols, arows, acols));
      if (object.ReferenceEquals(b, c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for (int i = 0; i < arows; i++)
        for (int j = 0; j < acols; j++)
        {
          double denom = b[brow, j];
          c[i, j] = denom == 0 ? resultIfNull : a[i, j] / denom;
        }
    }

    #endregion Addition, Subtraction, Multiply and combined operations

    #region Iterations

    /// <summary>
    /// Determines whether any element of the provided matrix <paramref name="a"/> fulfills the given predicate.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <param name="predicate">The predicate to fulfill. 1st argument is the row number, 2nd arg is the column number, 3rd arg is the element value at row and column. The return value is the predicate.</param>
    /// <returns>True if any element of the provided matrix <paramref name="a"/> fulfills the given predicate; otherwise false.</returns>
    public static bool Any(this IROMatrix<double> a, Func<int, int, double, bool> predicate)
    {
      int NC = a.ColumnCount;
      int NR = a.RowCount;
      for (int c = 0; c < NC; ++c)
        for (int r = 0; r < NR; ++r)
          if (predicate(r, c, a[r, c]))
            return true;

      return false;
    }

    public static IEnumerable<(int row, int column, T value)> EnumerateElementsIndexed<T>(IROMatrix<T> matrix, Zeros zeros = Zeros.AllowSkip) where T : struct
    {
      if (matrix is IROBandMatrix<T> bm)
        return bm.EnumerateElementsIndexed();
      else if (matrix is IROMatrixLevel1<T> sm)
        return sm.EnumerateElementsIndexed();
      else
        return InternalEnumerateElementsIndexed(matrix);
    }

    private static IEnumerable<(int row, int column, T value)> InternalEnumerateElementsIndexed<T>(IROMatrix<T> matrix) where T : struct
    {
      {
        int rowCount = matrix.RowCount;
        int columnCount = matrix.ColumnCount;
        for (int i = 0; i < rowCount; ++i)
          for (int j = 0; j < rowCount; ++j)
            yield return (i, j, matrix[i, j]);
      }
    }

    public static void MapIndexed<T, T1>(this IROMatrix<T> src1, T1 parameter1, Func<int, int, T, T1, T> function, IMatrix<T> result, Zeros zeros = Zeros.AllowSkip) where T : struct
    {
      if (src1 is IROMatrixLevel1<T> l1)
        l1.MapIndexed(parameter1, function, result, zeros);
      else
        MapIndexed_DefaultImpl(src1, parameter1, function, result);
    }

    private static void MapIndexed_DefaultImpl<T, T1>(this IROMatrix<T> src1, T1 parameter1, Func<int, int, T, T1, T> function, IMatrix<T> result) where T : struct
    {
      if (null == src1)
        throw new ArgumentNullException(nameof(src1));
      if (null == result)
        throw new ArgumentNullException(nameof(result));

      if (src1.RowCount != result.RowCount || src1.ColumnCount != result.ColumnCount)
        throw new RankException("Mismatch of dimensions of src1 and result");

      var cols = src1.ColumnCount;
      var rows = src1.RowCount;
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
        {
          result[i, j] = function(i, j, src1[i, j], parameter1);
        }
      }
    }

    #endregion Iterations

    #region Submatrix

    private static Func<int, int, DoubleMatrix> _defaultMatrixGenerator = (r, c) => new DoubleMatrix(r, c);

    /// <summary>
    /// Gets a new submatrix, i.e. a matrix containing selected elements of the original matrix <paramref name="a"/>.
    /// </summary>
    /// <typeparam name="T">Type of matrix to return.</typeparam>
    /// <param name="a">The original matrix.</param>
    /// <param name="selectedRows">Selected rows. The rows of the submatrix consist of all rows of the original matrix where selectedRows[row] is true.</param>
    /// <param name="selectedColumns">Selected columns. The columns of the submatrix consist of all columns of the original matrix where selectedColumns[col] is true.</param>
    /// <param name="MatrixGenerator">Function to generate the returned matrix. 1st arg is the number of rows, 2nd arg the number of columns. The function have to generate a new matrix with the given number of rows and columns.</param>
    /// <returns>The submatrix containing selected elements of the original matrix <paramref name="a"/>.</returns>
    public static T SubMatrix<T>(this IROMatrix<double> a, bool[] selectedRows, bool[] selectedColumns, Func<int, int, T> MatrixGenerator) where T : IMatrix<double>
    {
      int NRR = selectedRows.Count(ele => true == ele);
      int NCC = selectedColumns.Count(ele => true == ele);

      var result = MatrixGenerator(NRR, NCC);

      int NC = a.ColumnCount;
      int NR = a.RowCount;

      for (int c = 0, cc = 0; c < NC; ++c)
      {
        if (selectedColumns[c])
        {
          for (int r = 0, rr = 0; r < NR; ++r)
          {
            if (selectedRows[r])
            {
              result[rr, cc] = a[r, c];
              ++rr;
            }
          }
          ++cc;
        }
      }

      return result;
    }

    /// <summary>
    /// Gets a new submatrix, i.e. a matrix containing selected elements of the original matrix <paramref name="a"/>.
    /// </summary>
    /// <typeparam name="T">Type of matrix to return.</typeparam>
    /// <param name="a">The original matrix.</param>
    /// <param name="selectedRows">Selected rows. The rows of the submatrix consist of all rows of the original matrix whose index is contained in selectedRows.</param>
    /// <param name="selectedColumns">Selected columns. The columns of the submatrix consist of all columns of the original matrix whose index is contained in selectedColumns.</param>
    /// <param name="MatrixGenerator">Function to generate the returned matrix. 1st arg is the number of rows, 2nd arg the number of columns. The function have to generate a new matrix with the given number of rows and columns.</param>
    /// <returns>The submatrix containing selected elements of the original matrix <paramref name="a"/>.</returns>
    public static T SubMatrix<T>(this IROMatrix<double> a, int[] selectedRows, int[] selectedColumns, Func<int, int, T> MatrixGenerator) where T : IMatrix<double>
    {
      int NRR = selectedRows.Length;
      int NCC = selectedColumns.Length;

      var result = MatrixGenerator(NRR, NCC);

      int NC = a.ColumnCount;
      int NR = a.RowCount;

      for (int cc = 0; cc < NCC; ++cc)
      {
        int c = selectedColumns[cc];
        {
          for (int rr = 0; rr < NRR; ++rr)
          {
            int r = selectedRows[rr];
            {
              result[rr, cc] = a[r, c];
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Gets a new submatrix, i.e. a matrix containing selected elements of the original matrix <paramref name="a"/>.
    /// </summary>
    /// <typeparam name="T">Type of matrix to return.</typeparam>
    /// <param name="a">The original matrix.</param>
    /// <param name="selectedRows">Selected rows. The rows of the submatrix consist of all rows whose index is contained in selectedRows.</param>
    /// <param name="selectedColumns">Selected columns. The columns of the submatrix consist of all columns of the original matrix where selectedColumns[col] is true.</param>
    /// <param name="MatrixGenerator">Function to generate the returned matrix. 1st arg is the number of rows, 2nd arg the number of columns. The function have to generate a new matrix with the given number of rows and columns.</param>
    /// <returns>The submatrix containing selected elements of the original matrix <paramref name="a"/>.</returns>
    public static T SubMatrix<T>(this IROMatrix<double> a, int[] selectedRows, bool[] selectedColumns, Func<int, int, T> MatrixGenerator) where T : IMatrix<double>
    {
      int NRR = selectedRows.Length;
      int NCC = selectedColumns.Count(ele => true == ele);

      var result = MatrixGenerator(NRR, NCC);

      int NC = a.ColumnCount;
      int NR = a.RowCount;

      for (int c = 0, cc = 0; c < NC; ++c)
      {
        if (selectedColumns[c])
        {
          for (int rr = 0; rr < NRR; ++rr)
          {
            int r = selectedRows[rr];
            {
              result[rr, cc] = a[r, c];
            }
          }
          ++cc;
        }
      }

      return result;
    }

    /// <summary>
    /// Gets a new submatrix, i.e. a matrix containing selected elements of the original matrix <paramref name="a"/>.
    /// </summary>
    /// <typeparam name="T">Type of matrix to return.</typeparam>
    /// <param name="a">The original matrix.</param>
    /// <param name="selectedRows">Selected rows. The rows of the submatrix consist of all rows of the original matrix where selectedRows[row] is true.</param>
    /// <param name="selectedColumns">Selected columns. The columns of the submatrix consist of all columns of the original matrix whose index is contained in selectedColumns.</param>
    /// <param name="MatrixGenerator">Function to generate the returned matrix. 1st arg is the number of rows, 2nd arg the number of columns. The function have to generate a new matrix with the given number of rows and columns.</param>
    /// <returns>The submatrix containing selected elements of the original matrix <paramref name="a"/>.</returns>
    public static T SubMatrix<T>(this IROMatrix<double> a, bool[] selectedRows, int[] selectedColumns, Func<int, int, T> MatrixGenerator) where T : IMatrix<double>
    {
      int NRR = selectedRows.Count(ele => true == ele);
      int NCC = selectedColumns.Length;

      var result = MatrixGenerator(NRR, NCC);

      int NC = a.ColumnCount;
      int NR = a.RowCount;

      for (int cc = 0; cc < NCC; ++cc)
      {
        int c = selectedColumns[cc];
        {
          for (int r = 0, rr = 0; r < NR; ++r)
          {
            if (selectedRows[r])
            {
              result[rr, cc] = a[r, c];
              ++rr;
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Gets a new submatrix, i.e. a matrix containing selected elements of the original matrix <paramref name="a"/>.
    /// </summary>
    /// <typeparam name="T">Type of matrix to return.</typeparam>
    /// <param name="a">The original matrix.</param>
    /// <param name="selectedRows">Selected rows. The rows of the submatrix consist of all rows of the original matrix where selectedRows[row] is true.</param>
    /// <param name="selectedColumn">Selected column. The submatrix consists of one column, whose values originate from the original matrix selectedColumn.</param>
    /// <param name="MatrixGenerator">Function to generate the returned matrix. 1st arg is the number of rows, 2nd arg the number of columns. The function have to generate a new matrix with the given number of rows and columns.</param>
    /// <returns>The submatrix containing selected elements of the original matrix <paramref name="a"/>.</returns>
    public static T SubMatrix<T>(this IROMatrix<double> a, bool[] selectedRows, int selectedColumn, Func<int, int, T> MatrixGenerator) where T : IMatrix<double>
    {
      return SubMatrix(a, selectedRows, new int[] { selectedColumn }, MatrixGenerator);
    }

    /// <summary>
    /// Gets a new submatrix, i.e. a matrix containing selected elements of the original matrix <paramref name="a"/>.
    /// </summary>
    /// <typeparam name="T">Type of matrix to return.</typeparam>
    /// <param name="a">The original matrix.</param>
    /// <param name="selectedRow">Selected row. The submatrix consists of one row, whose values originate from the original matrix selectedRow.</param>
    /// <param name="selectedColumns">Selected columns. The columns of the submatrix consist of all columns of the original matrix where selectedColumns[col] is true.</param>
    /// <param name="MatrixGenerator">Function to generate the returned matrix. 1st arg is the number of rows, 2nd arg the number of columns. The function have to generate a new matrix with the given number of rows and columns.</param>
    /// <returns>The submatrix containing selected elements of the original matrix <paramref name="a"/>.</returns>
    public static T SubMatrix<T>(this IROMatrix<double> a, int selectedRow, bool[] selectedColumns, Func<int, int, T> MatrixGenerator) where T : IMatrix<double>
    {
      return SubMatrix(a, new int[] { selectedRow }, selectedColumns, MatrixGenerator);
    }

    #endregion Submatrix

    /// <summary>
    /// Replaces all matrix elements that are NaN (not a number) with the value of <paramref name="replacementValue"/>.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    /// <param name="replacementValue">The replacement value. This value is assigned to any matrix element that has the value NaN (Not a Number).</param>
    public static void ReplaceNaNElementsWith(this IMatrix<double> m, double replacementValue)
    {
      int rows = m.RowCount;
      int cols = m.ColumnCount;
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
          if (double.IsNaN(m[i, j]))
            m[i, j] = replacementValue;
      }
    }

    /// <summary>
    /// Replaces all matrix elements that are NaN (Not a Number) or infinite with the value of <paramref name="replacementValue"/>.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    /// <param name="replacementValue">The replacement value. This value is assigned to any matrix element that has the value NaN (Not a Number) or that is infinite.</param>
    public static void ReplaceNaNAndInfiniteElementsWith(this IMatrix<double> m, double replacementValue)
    {
      int rows = m.RowCount;
      int cols = m.ColumnCount;
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
        {
          var y = m[i, j];
          if (!(y >= double.MinValue && y <= double.MaxValue))
            m[i, j] = replacementValue;
        }
      }
    }

    /// <summary>
    /// Calculates the mean value of all matrix elements and then subtracts the mean value from each matrix element, so that the mean value of the resulting matrix is zero.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <param name="mean">The calculated mean value of the original matrix.</param>
    public static void ToZeroMean(this IMatrix<double> a, out double mean)
    {
      int rows = a.RowCount;
      int cols = a.ColumnCount;

      if (rows == 0)
        throw new InvalidDimensionMatrixException("The number of rows of the matrix is zero!");
      if (cols == 0)
        throw new InvalidDimensionMatrixException("The number of columns of the matrix is zero!");

      double sum = 0;
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
          sum += a[i, j];
      }

      if (double.IsNaN(sum))
        throw new InvalidContentMatrixException("One or more elements of this matrix are not numbers (NaN).");

      sum /= cols;
      sum /= rows;

      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
          a[i, j] -= sum;
      }

      mean = sum;
    }

    /// <summary>
    /// Calculates the mean value of all matrix elements and then subtracts the mean value from each matrix element, so that the mean value of the resulting matrix is zero.
    /// </summary>
    /// <param name="a">The matrix.</param>
    public static void ToZeroMean(this IMatrix<double> a)
    {
      ToZeroMean(a, out var mean);
    }

    /// <summary>
    /// This will center the matrix so that the mean of each column is null.
    /// </summary>
    /// <param name="a">The matrix where the columns should be centered.</param>
    /// <param name="mean">You can provide a matrix of dimension(1,a.Cols) where the mean row vector is stored, or null if not interested in this vector.</param>
    /// <remarks>Calling this function will change the matrix a to a column
    /// centered matrix. The original matrix data are lost.</remarks>
    public static void ColumnsToZeroMean(IMatrix<double> a, IVector<double> mean)
    {
      if (null != mean && mean.Length != a.ColumnCount)
        throw new ArithmeticException(string.Format("The provided resultant vector (actual length({0}) has not the expected dimension ({1})", mean.Length, a.ColumnCount));

      for (int col = 0; col < a.ColumnCount; col++)
      {
        double sum = 0;
        for (int row = 0; row < a.RowCount; row++)
          sum += a[row, col];
        sum /= a.RowCount; // calculate the mean
        for (int row = 0; row < a.RowCount; row++)
          a[row, col] -= sum; // subtract the mean from every element in the column

        if (null != mean)
          mean[col] = sum;
      }
    }

    /// <summary>
    /// This will center the matrix so that the mean of each column is null, and the variance of each column is one.
    /// </summary>
    /// <param name="a">The matrix where the columns should be centered and normalized to standard variance.</param>
    /// <param name="meanvec">You can provide a vector of length(a.Cols) where the mean row vector is stored, or null if not interested in this vector.</param>
    /// <param name="scalevec">You can provide a vector of length(a.Cols) where the inverse of the variance of the columns is stored, or null if not interested in this vector.</param>
    /// <remarks>Calling this function will change the matrix a to a column
    /// centered matrix. The original matrix data are lost.</remarks>
    public static void ColumnsToZeroMeanAndUnitVariance(IMatrix<double> a, IVector<double>? meanvec, IVector<double>? scalevec)
    {
      if (null != meanvec && (meanvec.Length != a.ColumnCount))
        throw new ArithmeticException(string.Format("The provided resultant mean vector (actual dim({0})has not the expected length ({1})", meanvec.Length, a.ColumnCount));
      if (null != scalevec && (scalevec.Length != a.ColumnCount))
        throw new ArithmeticException(string.Format("The provided resultant scale vector (actual dim({0})has not the expected length ({1})", scalevec.Length, a.ColumnCount));

      for (int col = 0; col < a.ColumnCount; col++)
      {
        double sum = 0;
        double sumsqr = 0;
        for (int row = 0; row < a.RowCount; row++)
        {
          sum += a[row, col];
          sumsqr += Square(a[row, col]);
        }
        double mean = sum / a.RowCount; // calculate the mean
        double scor;
        if (a.RowCount > 1 && sumsqr - mean * sum > 0)
          scor = Math.Sqrt((a.RowCount - 1) / (sumsqr - mean * sum));
        else
          scor = 1;
        for (int row = 0; row < a.RowCount; row++)
          a[row, col] = (a[row, col] - mean) * scor; // subtract the mean from every element in the column

        if (null != meanvec)
          meanvec[col] = mean;
        if (null != scalevec)
          scalevec[col] = scor;
      }
    }

    /// <summary>
    /// Returns the sum of the squares of all elements.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>The sum of the squares of all elements in the matrix a.</returns>
    public static double SumOfSquares(IROMatrix<double> a)
    {
      double sum = 0;
      for (int i = 0; i < a.RowCount; i++)
        for (int j = 0; j < a.ColumnCount; j++)
          sum += Square(a[i, j]);
      return sum;
    }

    /// <summary>
    /// Returns the sum of the squares of differences of elements of a and b.
    /// </summary>
    /// <param name="a">The first matrix.</param>
    /// <param name="b">The second matrix. Must have same dimensions than a.</param>
    /// <returns>The sum of the squared differences of each element in a to the corresponding element in b, i.e. Sum[(a[i,j]-b[i,j])²].</returns>
    public static double SumOfSquaredDifferences(IROMatrix<double> a, IROMatrix<double> b)
    {
      if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
        throw new ArithmeticException(string.Format("The two provided matrices (a({0},{1})) and b({2},{3})) have not the same dimensions.", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));

      double sum = 0;
      for (int i = 0; i < a.RowCount; i++)
        for (int j = 0; j < a.ColumnCount; j++)
          sum += Square(a[i, j] - b[i, j]);
      return sum;
    }

    /// <summary>
    /// Returns the square root of the sum of the squares of the matrix a.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>The square root of the sum of the squares of the matrix a.</returns>
    public static double LengthOf(IROMatrix<double> a)
    {
      return Math.Sqrt(SumOfSquares(a));
    }

    /// <summary>
    /// Tests if all elements of the matrix a are equal to zero.
    /// </summary>
    /// <param name="a">The matrix to test.</param>
    /// <returns>True if all elements are zero or if one of the two dimensions of the matrix is zero. False if the matrix contains nonzero elements.</returns>
    public static bool IsZeroMatrix(IROMatrix<double> a)
    {
      if (a.RowCount == 0 || a.ColumnCount == 0)
        return true; // we consider a matrix with one dimension zero also as zero matrix

      for (int i = 0; i < a.RowCount; i++)
        for (int j = 0; j < a.ColumnCount; j++)
          if (a[i, j] != 0)
            return false;

      return true;
    }

    /// <summary>
    /// Set all matrix elements to the provided value <paramref name="scalar"/>.
    /// </summary>
    /// <param name="a">The matrix where to set the elements.</param>
    /// <param name="scalar">The value which is used to set each element with.</param>
    public static void SetMatrixElements(this IMatrix<double> a, double scalar)
    {
      for (int i = 0; i < a.RowCount; i++)
        for (int j = 0; j < a.ColumnCount; j++)
          a[i, j] = scalar;
    }

    /// <summary>
    /// Sets the matrix elements to the value provided by a setter function <paramref name="Setter"/>.
    /// </summary>
    /// <param name="a">The matrix for which to set the elements.</param>
    /// <param name="Setter">The setter function. First arg is the row index, 2nd arg the column index. The return value is used to set the matrix element.</param>
    public static void SetMatrixElements(this IMatrix<double> a, Func<int, int, double> Setter)
    {
      for (int i = 0; i < a.RowCount; i++)
        for (int j = 0; j < a.ColumnCount; j++)
          a[i, j] = Setter(i, j);
    }

    /// <summary>
    /// Set all elements in the matrix to 0 (zero)
    /// </summary>
    /// <param name="a">The matrix to zero.</param>
    public static void ZeroMatrix(this IMatrix<double> a)
    {
      SetMatrixElements(a, 0);
    }

    /// <summary>
    /// Gets a submatrix out of the source matrix a. The dimensions of the submatrix are given by the provided matrix dest.
    /// </summary>
    /// <param name="src">The source matrix.</param>
    /// <param name="dest">The destination matrix where to store the submatrix. It's dimensions are the dimensions of the submatrix.</param>
    /// <param name="rowoffset">The row offset = vertical origin of the submatrix in the source matrix.</param>
    /// <param name="coloffset">The column offset = horizontal origin of the submatrix in the source matrix.</param>
    public static void Submatrix(IROMatrix<double> src, IMatrix<double> dest, int rowoffset, int coloffset)
    {
      for (int i = 0; i < dest.RowCount; i++)
        for (int j = 0; j < dest.ColumnCount; j++)
          dest[i, j] = src[i + rowoffset, j + coloffset];
    }

    /// <summary>
    /// Gets a submatrix out of the source matrix a. The dimensions of the submatrix are given by the provided matrix dest.
    /// The origin of the submatrix in the source matrix is (0,0), i.e. the left upper corner.
    /// </summary>
    /// <param name="src">The source matrix.</param>
    /// <param name="dest">The destination matrix where to store the submatrix. It's dimensions are the dimensions of the submatrix.</param>
    public static void Submatrix(IROMatrix<double> src, IMatrix<double> dest)
    {
      for (int i = 0; i < dest.RowCount; i++)
        for (int j = 0; j < dest.ColumnCount; j++)
          dest[i, j] = src[i, j];
    }

    /// <summary>
    /// Copies matrix src to matrix dest. Both matrizes must have the same dimensions.
    /// </summary>
    /// <param name="src">The source matrix to copy.</param>
    /// <param name="dest">The destination matrix to copy to.</param>
    public static void Copy(IROMatrix<double> src, IMatrix<double> dest)
    {
      if (dest.RowCount != src.RowCount || dest.ColumnCount != src.ColumnCount)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the dimension of the source matrix ({2},{3})", dest.RowCount, dest.ColumnCount, src.RowCount, src.ColumnCount));

      int rows = src.RowCount;
      int cols = src.ColumnCount;
      for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
          dest[i, j] = src[i, j];
    }

    /// <summary>
    /// Copies the matrix src into the matrix dest. Matrix dest must have equal or greater dimension than src.
    /// You can provide a destination row/column into the destination matrix where the origin of the copy operation is located.
    /// </summary>
    /// <param name="src">The source matrix.</param>
    /// <param name="dest">The destination matrix. Must have equal or higher dim than the source matrix.</param>
    /// <param name="destrow">The vertical origin of copy operation in the destination matrix.</param>
    /// <param name="destcol">The horizontal origin of copy operation in the destination matrix.</param>
    public static void Copy(IROMatrix<double> src, IMatrix<double> dest, int destrow, int destcol)
    {
      int rows = src.RowCount;
      int cols = src.ColumnCount;
      for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
          dest[i + destrow, j + destcol] = src[i, j];
    }

    /// <summary>
    /// Sets one column in the destination matrix equal to the vertical vector provided by src matix.
    /// </summary>
    /// <param name="src">The source matrix. Must be a vertical vector (cols=1) with the same number of rows than the destination matrix.</param>
    /// <param name="srccol">The column in the source matrix to copy from.</param>
    /// <param name="dest">The destination matrix where to copy the vertical vector into.</param>
    /// <param name="destcol">The column in the destination matrix where to copy the vector to.</param>
    public static void SetColumn(IROMatrix<double> src, int srccol, IMatrix<double> dest, int destcol)
    {
      if (destcol >= dest.ColumnCount)
        throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!", destcol, dest.RowCount, dest.ColumnCount));
      if (srccol >= src.ColumnCount)
        throw new ArithmeticException(string.Format("Parameter srccol out of range ({0}>={1})!", srccol, src.ColumnCount));
      if (dest.RowCount != src.RowCount)
        throw new ArithmeticException(string.Format("Try to set column {0}, but number of rows of the matrix ({1}) not match number of rows of the vector ({2})!", destcol, dest.RowCount, src.RowCount));

      for (int i = 0; i < dest.RowCount; i++)
        dest[i, destcol] = src[i, srccol];
    }

    /// <summary>
    /// Sets one column in the destination matrix equal to the vertical vector provided by src matix.
    /// </summary>
    /// <param name="src">The source matrix. Must be a vertical vector (cols=1) with the same number of rows than the destination matrix.</param>
    /// <param name="dest">The destination matrix where to copy the vertical vector into.</param>
    /// <param name="col">The column in the destination matrix where to copy the vector to.</param>
    public static void SetColumn(IROMatrix<double> src, IMatrix<double> dest, int col)
    {
      if (col >= dest.ColumnCount)
        throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!", col, dest.RowCount, dest.ColumnCount));
      if (src.ColumnCount != 1)
        throw new ArithmeticException(string.Format("Try to set column {0} with a matrix of more than one, namely {1} columns, is not allowed!", col, src.ColumnCount));
      if (dest.RowCount != src.RowCount)
        throw new ArithmeticException(string.Format("Try to set column {0}, but number of rows of the matrix ({1}) not match number of rows of the vector ({2})!", col, dest.RowCount, src.RowCount));

      for (int i = 0; i < dest.RowCount; i++)
        dest[i, col] = src[i, 0];
    }

    /// <summary>
    /// Sets one row in the destination matrix equal to the horizontal vector provided by src matix.
    /// </summary>
    /// <param name="src">The source matrix. Must be a horizontal vector (rows=1) with the same number of columns than the destination matrix.</param>
    /// <param name="srcRow">The row in the source matrix where to copy from.</param>
    /// <param name="dest">The destination matrix where to copy the horizontal vector into.</param>
    /// <param name="destRow">The row in the destination matrix where to copy the vector to.</param>
    public static void SetRow(IROMatrix<double> src, int srcRow, IMatrix<double> dest, int destRow)
    {
      if (destRow >= dest.RowCount)
        throw new ArithmeticException(string.Format("Try to set row {0} in the matrix with dim({1},{2}) is not allowed!", destRow, dest.RowCount, dest.ColumnCount));
      if (srcRow >= src.RowCount)
        throw new ArithmeticException(string.Format("The source row number ({0}) exceeds the actual number of rows ({1})in the source matrix!", srcRow, src.RowCount));
      if (dest.ColumnCount != src.ColumnCount)
        throw new ArithmeticException(string.Format("Number of columns of the matrix ({0}) not match number of colums of the vector ({1})!", dest.ColumnCount, src.ColumnCount));

      for (int j = 0; j < dest.ColumnCount; j++)
        dest[destRow, j] = src[srcRow, j];
    }

    /// <summary>
    /// Sets one row in the destination matrix equal to the vector provided by src.
    /// </summary>
    /// <param name="src">The source vector. Must be of same length as the number of columns of the destination matrix.</param>
    /// <param name="dest">The destination matrix where to copy the horizontal vector into.</param>
    /// <param name="destRow">The row in the destination matrix where to copy the vector to.</param>
    public static void SetRow(IReadOnlyList<double> src, IMatrix<double> dest, int destRow)
    {
      if (destRow >= dest.RowCount)
        throw new ArithmeticException(string.Format("Try to set row {0} in the matrix with dim({1},{2}) is not allowed!", destRow, dest.RowCount, dest.ColumnCount));
      if (dest.ColumnCount != src.Count)
        throw new ArithmeticException(string.Format("Number of columns of the matrix ({0}) not match number of elements of the vector ({1})!", dest.ColumnCount, src.Count));

      for (int j = 0; j < dest.ColumnCount; j++)
        dest[destRow, j] = src[j];
    }

    /// <summary>
    /// Sets one row in the destination matrix equal to the vector provided by src.
    /// </summary>
    /// <param name="src">The source vector. Must be of same length as the number of columns of the destination matrix.</param>
    /// <param name="dest">The destination matrix where to copy the horizontal vector into.</param>
    /// <param name="destColumn">The row in the destination matrix where to copy the vector to.</param>
    public static void SetColumn(IReadOnlyList<double> src, IMatrix<double> dest, int destColumn)
    {
      if (destColumn >= dest.ColumnCount)
        throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!", destColumn, dest.RowCount, dest.ColumnCount));
      if (dest.RowCount != src.Count)
        throw new ArithmeticException(string.Format("Number of rows of the matrix ({0}) not match number of elements of the vector ({1})!", dest.RowCount, src.Count));

      for (int j = 0; j < src.Count; j++)
        dest[j, destColumn] = src[j];
    }

    /// <summary>
    /// Normalizes each row (each horizontal vector) of the matrix. After
    /// normalization, each row has the norm 1, i.e. the sum of squares of the elements of each row is 1 (one).
    /// </summary>
    /// <param name="a">The matrix which should be row normalized.</param>
    public static void NormalizeRows(IMatrix<double> a)
    {
      for (int i = 0; i < a.RowCount; i++)
      {
        double sum = 0;
        for (int j = 0; j < a.ColumnCount; j++)
          sum += Square(a[i, j]);

        if (sum != 0) // Normalize only of at least one element is not null
        {
          sum = 1 / Math.Sqrt(sum);
          for (int j = 0; j < a.ColumnCount; j++)
            a[i, j] *= sum;
        }
      }
    }

    /// <summary>
    /// Normalizes each column (each vertical vector) of the matrix. After
    /// normalization, each column has the norm 1, i.e. the sum of squares of the elements of each column is 1 (one).
    /// </summary>
    /// <param name="a">The matrix which should be column normalized.</param>
    public static void NormalizeCols(IMatrix<double> a)
    {
      for (int i = 0; i < a.ColumnCount; i++)
      {
        double sum = 0;
        for (int j = 0; j < a.RowCount; j++)
          sum += Square(a[j, i]);

        if (sum != 0)
        {
          sum = 1 / Math.Sqrt(sum);
          for (int j = 0; j < a.RowCount; j++)
            a[j, i] *= sum;
        }
      }
    }

    /// <summary>
    /// Normalizes the column col of a matrix to unit length.
    /// </summary>
    /// <param name="a">The matrix for which the column col is normalized.</param>
    /// <param name="col">The number of the column which should be normalized.</param>
    /// <returns>Square root of the sum of squares of the column, i.e. the original length of the column vector before normalization.</returns>
    public static double NormalizeOneColumn(IMatrix<double> a, int col)
    {
      if (col >= a.ColumnCount)
        throw new ArithmeticException(string.Format("Matrix a is expected to have at least {0} columns, but has the actual dimensions({1},{2})", col + 1, a.RowCount, a.ColumnCount));

      double sum = 0;
      for (int i = 0; i < a.RowCount; i++)
        sum += Square(a[i, 0]);

      sum = Math.Sqrt(sum);
      for (int i = 0; i < a.RowCount; i++)
        a[i, 0] /= sum;

      return sum;
    }

    /// <summary>
    /// This inverts the provided diagonal matrix. There is no check that the matrix is really
    /// diagonal, but the algorithm sets the elements outside the diagonal to zero, assuming
    /// that this are small arithmetic errors.
    /// </summary>
    /// <param name="a">The matrix to invert. After calling the matrix is inverted, i.e.
    /// the diagonal elements are replaced by their inverses, and the outer diagonal elements are set to zero.</param>
    public static void InvertDiagonalMatrix(IMatrix<double> a)
    {
      int rows = a.RowCount;
      int cols = a.ColumnCount;

      if (cols != rows)
        throw new ArithmeticException(string.Format("A diagonal matrix has to be quadratic, but you provided a matrix of dimension({0},{1})!", rows, cols));

      for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
          a[i, j] = i == j ? 1 / a[i, j] : 0;
    }

    /// <summary>
    /// Compares matrix a and matrix b. Takes the norm of matrix b times accuracy as
    /// threshold basis for comparing the elements.
    /// </summary>
    /// <param name="a">The first matrix.</param>
    /// <param name="b">The second matrix. Basis for calculation of threshold.</param>
    /// <param name="accuracy">The accuracy.</param>
    /// <returns></returns>
    public static bool IsEqual(IROMatrix<double> a, IROMatrix<double> b, double accuracy)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if (a.ColumnCount != b.ColumnCount || a.RowCount != b.RowCount)
        throw new ArithmeticException(string.Format("Try to compare a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));

      double thresh = Math.Sqrt(SumOfSquares(b)) * accuracy / ((double)b.RowCount * b.ColumnCount);
      ;
      for (int i = 0; i < a.RowCount; i++)
        for (int j = 0; j < a.ColumnCount; j++)
          if (Math.Abs(a[i, j] - b[i, j]) > thresh)
            return false;

      return true;
    }

    /// <summary>
    /// Calculates eigenvectors (loads) and the corresponding eigenvalues (scores)
    /// by means of the NIPALS algorithm
    /// </summary>
    /// <param name="X">The matrix to which the decomposition is applied to. A row of the matrix is one spectrum (or a single measurement giving multiple resulting values). The different rows of the matrix represent
    /// measurements under different conditions.</param>
    /// <param name="numFactors">The number of factors to be calculated. If 0 is provided, factors are calculated until the provided accuracy is reached. </param>
    /// <param name="accuracy">The relative residual variance that should be reached.</param>
    /// <param name="factors">Resulting matrix of factors. You have to provide a extensible matrix of dimension(0,0) as the vertical score vectors are appended to the matrix.</param>
    /// <param name="loads">Resulting matrix consiting of horizontal load vectors (eigenspectra). You have to provide a extensible matrix of dimension(0,0) here.</param>
    /// <param name="residualVarianceVector">Residual variance. Element[0] is the original variance, element[1] the residual variance after the first factor subtracted and so on. You can provide null if you don't need this result.</param>
    public static void NIPALS_HO(
      IMatrix<double> X,
      int numFactors,
      double accuracy,
      IRightExtensibleMatrix<double> factors,
      IBottomExtensibleMatrix<double> loads,
      IBottomExtensibleMatrix<double> residualVarianceVector)
    {
      // first center the matrix
      //MatrixMath.ColumnsToZeroMean(X, null);

      double originalVariance = Math.Sqrt(MatrixMath.SumOfSquares(X));

      if (null != residualVarianceVector)
        residualVarianceVector.AppendBottom(new MatrixMath.ScalarAsMatrix<double>(originalVariance));

      IMatrix<double> l = new MatrixWithOneRow<double>(X.ColumnCount);
      IMatrix<double>? t_prev = null;
      IMatrix<double> t = new MatrixWithOneColumn<double>(X.RowCount);

      int maxFactors = numFactors <= 0 ? X.ColumnCount : Math.Min(numFactors, X.ColumnCount);

      for (int nFactor = 0; nFactor < maxFactors; nFactor++)
      {
        //l has to be a horizontal vector
        // 1. Guess the transposed Vector l_transp, use first row of X matrix if it is not empty, otherwise the first non-empty row
        int rowoffset = 0;
        do
        {
          Submatrix(X, l, rowoffset, 0);     // l is now a horizontal vector
          rowoffset++;
        } while (IsZeroMatrix(l) && rowoffset < X.RowCount);

        for (int iter = 0; iter < 500; iter++)
        {
          // 2. Calculate the new vector t for the factor values
          MultiplySecondTransposed(X, l, t); // t = X*l_t (t is  a vertical vector)

          // Compare this with the previous one
          if (t_prev != null && IsEqual(t_prev, t, 1E-9))
            break;

          // 3. Calculate the new loads
          MultiplyFirstTransposed(t, X, l); // l = t_tr*X  (gives a horizontal vector of load (= eigenvalue spectrum)

          // normalize the (one) row
          NormalizeRows(l); // normalize the eigenvector spectrum

          // 4. Goto step 2 or break after a number of iterations
          if (t_prev == null)
            t_prev = new MatrixWithOneColumn<double>(X.RowCount);
          Copy(t, t_prev); // stores the content of t in t_prev
        }

        // Store factor and loads
        factors.AppendRight(t);
        loads.AppendBottom(l);

        // 5. Calculate the residual matrix X = X - t*l
        SubtractProductFromSelf(t, l, X); // X is now the residual matrix

        // if the number of factors to calculate is not provided,
        // calculate the norm of the residual matrix and compare with the original
        // one
        if (numFactors <= 0 && !(residualVarianceVector is null))
        {
          double residualVariance = Math.Sqrt(MatrixMath.SumOfSquares(X));
          residualVarianceVector.AppendBottom(new MatrixMath.ScalarAsMatrix<double>(residualVariance));

          if (residualVariance <= accuracy * originalVariance)
          {
            break;
          }
        }
      } // for all factors
    } // end NIPALS

    #region SingularValueDecomposition

    /// <summary>
    /// Calculates the pseudo inverse of the matrix <c>input</c> by means of singular value decomposition.
    /// A relative value of <c>100*DBL_EPSILON</c> is used to chop the singular values before calculation.
    /// </summary>
    /// <param name="input">Input matrix</param>
    /// <returns>The pseudo inverse of matrix <c>input</c>.</returns>
    public static IMatrix<double> PseudoInverse(IROMatrix<double> input)
    {
      return PseudoInverse(input, out var rank);
    }

    /// <summary>
    /// Calculates the pseudo inverse of the matrix <c>input</c> by means of singular value decomposition.
    /// A relative value of <c>100*DBL_EPSILON</c> is used to chop the singular values before calculation.
    /// </summary>
    /// <param name="input">Input matrix</param>
    /// <param name="rank">Returns the rank of the input matrix.</param>
    /// <returns>The pseudo inverse of matrix <c>input</c>.</returns>
    public static IMatrix<double> PseudoInverse(IROMatrix<double> input, out int rank)
    {
      var ma = new LeftSpineJaggedArrayMatrix<double>(input.RowCount, input.ColumnCount);
      MatrixMath.Copy(input, ma);
      var svd = new SingularValueDecomposition(ma);

      double[][] B = GetMatrixArray(input.ColumnCount, input.RowCount);

      /* compute the pseudoinverse in B */
      double[] s = svd.Diagonal;
      int m = input.RowCount;
      int n = input.ColumnCount;
      int minmn = Math.Min(m, n);

      double[][] v = svd.V;
      double[][] u = svd.U;
      double thresh = (DoubleConstants.DBL_EPSILON * 100) * s[0];
      for (rank = 0; rank < minmn && s[rank] > thresh; rank++)
      {
        double one_over_denom = 1.0 / s[rank];

        for (int j = 0; j < m; j++)
          for (int i = 0; i < n; i++)
            B[i][j] += v[i][rank] * u[j][rank] * one_over_denom;
      }

      return new JaggedArrayMatrix(B);
    }

    /// <summary>Returns the singular value decomposition for this matrix.</summary>
    /// <param name="input">The input matrix (is preserved).</param>
    /// <param name="output">The resulting matrix. Has to be of same dimensions as the input matrix.</param>
    public static SingularValueDecomposition GetSingularValueDecomposition(IROMatrix<double> input, IMatrix<double> output)
    {
      MatrixMath.Copy(input, output);
      return new SingularValueDecomposition(output);
    }

    /// <summary>Returns the singular value decomposition for this matrix.</summary>
    /// <param name="inout">The input matrix, on return the resulting decomposed matrix.</param>
    public static SingularValueDecomposition GetSingularValueDecomposition(IMatrix<double> inout)
    {
      return new SingularValueDecomposition(inout);
    }

    /// <summary>
    /// Get the minimum value of all elements of the specified matrix <paramref name="a"/>.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>Minimum value of all elements of the specified matrix <paramref name="a"/>.</returns>
    public static double Min(this IROMatrix<double> a)
    {
      double min = a[0, 0];

      for (int i = 0; i < a.RowCount; ++i)
        for (int j = 0; j < a.ColumnCount; ++j)
          min = Math.Min(min, a[i, j]);

      return min;
    }

    /// <summary>
    /// Get the maximum value of all elements of the specified matrix <paramref name="a"/>.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>Maximum value of all elements of the specified matrix <paramref name="a"/>.</returns>
    public static double Max(this IROMatrix<double> a)
    {
      double max = a[0, 0];

      for (int i = 0; i < a.RowCount; ++i)
        for (int j = 0; j < a.ColumnCount; ++j)
          max = Math.Max(max, a[i, j]);

      return max;
    }

    /// <summary>
    /// Get the trace of square matrix <paramref name="a"/>, i.e. the sum of diagonal elements.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>Trace of square matrix <paramref name="a"/>, i.e. the sum of diagonal elements.</returns>
    public static double Trace(this IROMatrix<double> a)
    {
      if (a.RowCount != a.ColumnCount)
        throw new ArgumentException(string.Format("Matrix needs to be a square matrix, but has dimensions {0}x{1}", a.RowCount, a.ColumnCount), nameof(a));

      double sum = 0;
      for (int i = a.ColumnCount - 1; i >= 0; --i)
        sum += a[i, i];

      return sum;
    }

    public static double Norm(this IROMatrix<double> a, MatrixNorm ntype)
    {
      if (null == a)
        throw new ArgumentNullException(nameof(a));

      double result = 0;
      switch (ntype)
      {
        case MatrixNorm.M1Norm:
          {
            for (int c = 0; c < a.ColumnCount; ++c)
            {
              double sum = 0;
              for (int r = 0; r < a.RowCount; ++r)
                sum += Math.Abs(a[r, c]);

              result = Math.Max(result, sum);
            }
          }
          break;

        default:
          throw new NotImplementedException("Norm not implemented: " + ntype.ToString());
      }

      return result;
    }

    /// <summary>
    /// Class to calculate the singular value decomposition.
    /// </summary>
    /// <remarks>
    /// <para>Adapted from Lutz Roeders Mapack library.</para>
    /// <code>Some properties of the singular value decomposition:
    /// X - the matrix to decompose, U w V' - the decomposition.
    ///
    ///       -1
    /// (X' X)    = V (1/w^2) V'  (usually called covariance matrix)
    ///
    ///                        -1
    /// Hat matrix H = X (X' X)  X' = U U'
    ///
    ///
    /// </code>
    /// </remarks>
    public class SingularValueDecomposition : ISingularValueDecomposition
    {
      //private Matrix U;
      private double[][] u;

      private double[][] v;

      //private Matrix V;
      private double[] s; // singular values

      private double[] e;
      private double[] work;
      private double[] _HatDiagonal;
      private int m;
      private int n;

      /// <summary>
      /// Creates a singular value decomposition of matrix a, resulting in matrix a itself.
      /// </summary>
      /// <param name="a">Matrix to decompose, on return: decomposed matrix.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      public SingularValueDecomposition(IMatrix<double> a)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      {
        ComputeSingularValueDecomposition(a);
      }

      /// <summary>
      /// Creates a singular value decomposition of matrix a, resulting in matrix a itself.
      /// </summary>
      /// <param name="a">Matrix to decompose, on return: decomposed matrix.</param>
      public void ComputeSingularValueDecomposition(IMatrix<double> a)
      {
        m = a.RowCount;
        n = a.ColumnCount;
        int nu = Math.Min(m, n);
        s = new double[Math.Min(m + 1, n)];
        //U = new Matrix(m, nu);
        //V = new Matrix(n, n);
        u = GetMatrixArray(m, nu); // U.Array
        v = GetMatrixArray(n, n); // V.Array;
        e = new double[n];
        work = new double[m];
        bool wantu = true;
        bool wantv = true;

        // Reduce A to bidiagonal form, storing the diagonal elements in s and the super-diagonal elements in e.
        int nct = Math.Min(m - 1, n);
        int nrt = Math.Max(0, Math.Min(n - 2, m));
        for (int k = 0; k < Math.Max(nct, nrt); k++)
        {
          if (k < nct)
          {
            // Compute the transformation for the k-th column and place the k-th diagonal in s[k].
            // Compute 2-norm of k-th column without under/overflow.
            s[k] = 0;
            for (int i = k; i < m; i++)
              s[k] = Hypotenuse(s[k], a[i, k]);

            if (s[k] != 0.0)
            {
              if (a[k, k] < 0.0)
                s[k] = -s[k];

              for (int i = k; i < m; i++)
                a[i, k] /= s[k];

              a[k, k] += 1.0;
            }
            s[k] = -s[k];
          }

          for (int j = k + 1; j < n; j++)
          {
            if ((k < nct) & (s[k] != 0.0))
            {
              // Apply the transformation.
              double t = 0;
              for (int i = k; i < m; i++)
                t += a[i, k] * a[i, j];
              t = -t / a[k, k];
              for (int i = k; i < m; i++)
                a[i, j] += t * a[i, k];
            }

            // Place the k-th row of A into e for the subsequent calculation of the row transformation.
            e[j] = a[k, j];
          }

          if (wantu & (k < nct))
          {
            // Place the transformation in U for subsequent back
            // multiplication.
            for (int i = k; i < m; i++)
              u[i][k] = a[i, k];
          }

          if (k < nrt)
          {
            // Compute the k-th row transformation and place the k-th super-diagonal in e[k].
            // Compute 2-norm without under/overflow.
            e[k] = 0;
            for (int i = k + 1; i < n; i++)
              e[k] = Hypotenuse(e[k], e[i]);

            if (e[k] != 0.0)
            {
              if (e[k + 1] < 0.0)
                e[k] = -e[k];

              for (int i = k + 1; i < n; i++)
                e[i] /= e[k];

              e[k + 1] += 1.0;
            }

            e[k] = -e[k];
            if ((k + 1 < m) & (e[k] != 0.0))
            {
              // Apply the transformation.
              for (int i = k + 1; i < m; i++)
                work[i] = 0.0;

              for (int j = k + 1; j < n; j++)
                for (int i = k + 1; i < m; i++)
                  work[i] += e[j] * a[i, j];

              for (int j = k + 1; j < n; j++)
              {
                double t = -e[j] / e[k + 1];
                for (int i = k + 1; i < m; i++)
                  a[i, j] += t * work[i];
              }
            }

            if (wantv)
            {
              // Place the transformation in V for subsequent back multiplication.
              for (int i = k + 1; i < n; i++)
                v[i][k] = e[i];
            }
          }
        }

        // Set up the final bidiagonal matrix or order p.
        int p = Math.Min(n, m + 1);
        if (nct < n)
          s[nct] = a[nct, nct];
        if (m < p)
          s[p - 1] = 0.0;
        if (nrt + 1 < p)
          e[nrt] = a[nrt, p - 1];
        e[p - 1] = 0.0;

        // If required, generate U.
        if (wantu)
        {
          for (int j = nct; j < nu; j++)
          {
            for (int i = 0; i < m; i++)
              u[i][j] = 0.0;
            u[j][j] = 1.0;
          }

          for (int k = nct - 1; k >= 0; k--)
          {
            if (s[k] != 0.0)
            {
              for (int j = k + 1; j < nu; j++)
              {
                double t = 0;
                for (int i = k; i < m; i++)
                  t += u[i][k] * u[i][j];

                t = -t / u[k][k];
                for (int i = k; i < m; i++)
                  u[i][j] += t * u[i][k];
              }

              for (int i = k; i < m; i++)
                u[i][k] = -u[i][k];

              u[k][k] = 1.0 + u[k][k];
              for (int i = 0; i < k - 1; i++)
                u[i][k] = 0.0;
            }
            else
            {
              for (int i = 0; i < m; i++)
                u[i][k] = 0.0;
              u[k][k] = 1.0;
            }
          }
        }

        // If required, generate V.
        if (wantv)
        {
          for (int k = n - 1; k >= 0; k--)
          {
            if ((k < nrt) & (e[k] != 0.0))
            {
              for (int j = k + 1; j < n; j++) // Lellinger (2004/03/28): end variable changed from nu to n
              {
                double t = 0;
                for (int i = k + 1; i < n; i++)
                  t += v[i][k] * v[i][j];

                t = -t / v[k + 1][k];
                for (int i = k + 1; i < n; i++)
                  v[i][j] += t * v[i][k];
              }
            }

            for (int i = 0; i < n; i++)
              v[i][k] = 0.0;
            v[k][k] = 1.0;
          }
        }

        // Main iteration loop for the singular values.
        int pp = p - 1;
        int iter = 0;
        double eps = Math.Pow(2.0, -52.0);
        while (p > 0)
        {
          int k, kase;

          // Here is where a test for too many iterations would go.
          // This section of the program inspects for
          // negligible elements in the s and e arrays.  On
          // completion the variables kase and k are set as follows.
          // kase = 1     if s(p) and e[k-1] are negligible and k<p
          // kase = 2     if s(k) is negligible and k<p
          // kase = 3     if e[k-1] is negligible, k<p, and s(k), ..., s(p) are not negligible (qr step).
          // kase = 4     if e(p-1) is negligible (convergence).
          for (k = p - 2; k >= -1; k--)
          {
            if (k == -1)
              break;

            if (Math.Abs(e[k]) <= eps * (Math.Abs(s[k]) + Math.Abs(s[k + 1])))
            {
              e[k] = 0.0;
              break;
            }
          }

          if (k == p - 2)
          {
            kase = 4;
          }
          else
          {
            int ks;
            for (ks = p - 1; ks >= k; ks--)
            {
              if (ks == k)
                break;

              double t = (ks != p ? Math.Abs(e[ks]) : 0.0) + (ks != k + 1 ? Math.Abs(e[ks - 1]) : 0.0);
              if (Math.Abs(s[ks]) <= eps * t)
              {
                s[ks] = 0.0;
                break;
              }
            }

            if (ks == k)
              kase = 3;
            else if (ks == p - 1)
              kase = 1;
            else
            {
              kase = 2;
              k = ks;
            }
          }

          k++;

          // Perform the task indicated by kase.
          switch (kase)
          {
            // Deflate negligible s(p).
            case 1:
              {
                double f = e[p - 2];
                e[p - 2] = 0.0;
                for (int j = p - 2; j >= k; j--)
                {
                  double t = Hypotenuse(s[j], f);
                  double cs = s[j] / t;
                  double sn = f / t;
                  s[j] = t;
                  if (j != k)
                  {
                    f = -sn * e[j - 1];
                    e[j - 1] = cs * e[j - 1];
                  }

                  if (wantv)
                  {
                    for (int i = 0; i < n; i++)
                    {
                      t = cs * v[i][j] + sn * v[i][p - 1];
                      v[i][p - 1] = -sn * v[i][j] + cs * v[i][p - 1];
                      v[i][j] = t;
                    }
                  }
                }
              }
              break;

            // Split at negligible s(k).
            case 2:
              {
                double f = e[k - 1];
                e[k - 1] = 0.0;
                for (int j = k; j < p; j++)
                {
                  double t = Hypotenuse(s[j], f);
                  double cs = s[j] / t;
                  double sn = f / t;
                  s[j] = t;
                  f = -sn * e[j];
                  e[j] = cs * e[j];
                  if (wantu)
                  {
                    for (int i = 0; i < m; i++)
                    {
                      t = cs * u[i][j] + sn * u[i][k - 1];
                      u[i][k - 1] = -sn * u[i][j] + cs * u[i][k - 1];
                      u[i][j] = t;
                    }
                  }
                }
              }
              break;

            // Perform one qr step.
            case 3:
              {
                // Calculate the shift.
                double scale = Math.Max(Math.Max(Math.Max(Math.Max(Math.Abs(s[p - 1]), Math.Abs(s[p - 2])), Math.Abs(e[p - 2])), Math.Abs(s[k])), Math.Abs(e[k]));
                double sp = s[p - 1] / scale;
                double spm1 = s[p - 2] / scale;
                double epm1 = e[p - 2] / scale;
                double sk = s[k] / scale;
                double ek = e[k] / scale;
                double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
                double c = (sp * epm1) * (sp * epm1);
                double shift = 0.0;
                if ((b != 0.0) | (c != 0.0))
                {
                  shift = Math.Sqrt(b * b + c);
                  if (b < 0.0)
                    shift = -shift;
                  shift = c / (b + shift);
                }

                double f = (sk + sp) * (sk - sp) + shift;
                double g = sk * ek;

                // Chase zeros.
                for (int j = k; j < p - 1; j++)
                {
                  double t = Hypotenuse(f, g);
                  double cs = f / t;
                  double sn = g / t;
                  if (j != k)
                    e[j - 1] = t;
                  f = cs * s[j] + sn * e[j];
                  e[j] = cs * e[j] - sn * s[j];
                  g = sn * s[j + 1];
                  s[j + 1] = cs * s[j + 1];
                  if (wantv)
                  {
                    for (int i = 0; i < n; i++)
                    {
                      t = cs * v[i][j] + sn * v[i][j + 1];
                      v[i][j + 1] = -sn * v[i][j] + cs * v[i][j + 1];
                      v[i][j] = t;
                    }
                  }

                  t = Hypotenuse(f, g);
                  cs = f / t;
                  sn = g / t;
                  s[j] = t;
                  f = cs * e[j] + sn * s[j + 1];
                  s[j + 1] = -sn * e[j] + cs * s[j + 1];
                  g = sn * e[j + 1];
                  e[j + 1] = cs * e[j + 1];
                  if (wantu && (j < m - 1))
                  {
                    for (int i = 0; i < m; i++)
                    {
                      t = cs * u[i][j] + sn * u[i][j + 1];
                      u[i][j + 1] = -sn * u[i][j] + cs * u[i][j + 1];
                      u[i][j] = t;
                    }
                  }
                }

                e[p - 2] = f;
                iter = iter + 1;
              }
              break;

            // Convergence.
            case 4:
              {
                // Make the singular values positive.
                if (s[k] <= 0.0)
                {
                  s[k] = (s[k] < 0.0 ? -s[k] : 0.0);
                  if (wantv)
                    for (int i = 0; i <= pp; i++)
                      v[i][k] = -v[i][k];
                }

                // Order the singular values.
                while (k < pp)
                {
                  if (s[k] >= s[k + 1])
                    break;

                  double t = s[k];
                  s[k] = s[k + 1];
                  s[k + 1] = t;
                  if (wantv && (k < n - 1))
                    for (int i = 0; i < n; i++)
                    {
                      t = v[i][k + 1];
                      v[i][k + 1] = v[i][k];
                      v[i][k] = t;
                    }

                  if (wantu && (k < m - 1))
                    for (int i = 0; i < m; i++)
                    {
                      t = u[i][k + 1];
                      u[i][k + 1] = u[i][k];
                      u[i][k] = t;
                    }

                  k++;
                }

                iter = 0;
                p--;
              }
              break;
          }
        }
      }

      /// <summary>
      /// Solves A·X = B for a vector X, where A is specified by A=U*S*V'. U, S, and V are
      /// results of the decomposition.
      /// </summary>
      /// <param name="b">Input vector.</param>
      /// <param name="x">The resulting vector that fullfilles A·x=b.</param>
      public void Backsubstitution(double[] b, double[] x)
      {
        Backsubstitution(VectorMath.ToROVector(b), VectorMath.ToVector(x));
      }

      private class SolveTempStorage
      {
        public IMatrix<double> A;
        public SingularValueDecomposition SVD;

        public SolveTempStorage(IMatrix<double> a, SingularValueDecomposition svd)
        {
          A = a;
          SVD = svd;
        }
      }

      /// <summary>
      /// Solves the equation A x = B and returns x.
      /// </summary>
      /// <param name="A">The matrix.</param>
      /// <param name="B">The right side.</param>
      /// <param name="x">On return, contains the solution vector.</param>
      /// <param name="tempstorage">On return, holds the allocated temporary storage. You can use this
      /// in subsequent calls to Solve with the same dimensions of the matrix.</param>
      public static void Solve(IROMatrix<double> A, IReadOnlyList<double> B, IVector<double> x, ref object tempstorage)
      {
        if (tempstorage is SolveTempStorage sts)
        {
          if (sts.A.RowCount == A.RowCount && sts.A.ColumnCount == A.ColumnCount)
          {
            MatrixMath.Copy(A, sts.A);
            sts.SVD.ComputeSingularValueDecomposition(sts.A);
            sts.SVD.Backsubstitution(B, x);
            return;
          }
        }
        // tempstorage can not be used
        var stsA = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(A.RowCount, A.ColumnCount);
        MatrixMath.Copy(A, stsA);
        var stsSVD = new SingularValueDecomposition(stsA);
        stsSVD.Backsubstitution(B, x);
        tempstorage = new SolveTempStorage(stsA, stsSVD);
        return;
      }

      /// <summary>
      /// Solves A·X = B for a vector X, where A is specified by A=U*S*V'. U, S, and V are
      /// results of the decomposition.
      /// </summary>
      /// <param name="b">Input vector.</param>
      /// <param name="x">The resulting vector that fullfilles A·x=b.</param>
      public void Backsubstitution(IReadOnlyList<double> b, IVector<double> x)
      {
        double sum;
        double[] tmp = new double[n];
        int nu = Math.Min(m, n);

        for (int j = 0; j < nu; j++) // calculate UT*B  (UT == U transposed)
        {
          sum = 0.0;
          if (s[j] != 0)
          {
            // Nonzero result only if sj is nonzero.
            for (int i = 0; i < m; i++)
              sum += u[i][j] * b[i];
            sum /= s[j]; // This is the divide by sj .
          }
          tmp[j] = sum;
        }
        for (int j = 0; j < n; j++) // Matrix multiply by V to get answer.
        {
          sum = 0.0;
          for (int jj = 0; jj < n; jj++)
            sum += v[j][jj] * tmp[jj];
          x[j] = sum;
        }
      }

      /// <summary>
      /// Calculates the covariance matrix Cov(i,j)= (X'X)^(-1) = SUM_over_k( V[i,k]*V[j,k]/s[k]^2). If s[k] is zero, 1/s[k]^2 will be set to zero. If the singular value decomposition was used to make a linear fit,
      /// this is the variance-covariance matrix of the fitting parameters.
      /// </summary>
      /// <returns>The variance-covariance-matrix.</returns>
      public double[][] GetCovariances()
      {
        double[][] cvm = MatrixMath.GetMatrixArray(n, n);
        double[] wti = new double[n];
        for (int i = 0; i < n; i++)
          wti[i] = (i < m && s[i] != 0) ? 1 / (s[i] * s[i]) : 0;

        for (int i = 0; i < n; i++)
        {
          for (int j = 0; j <= i; j++)
          {
            double sum = 0;
            for (int k = 0; k < n; k++)
              sum += v[i][k] * v[j][k] * wti[k];

            cvm[j][i] = cvm[i][j] = sum;
          }
        }

        return cvm;
      }

      /// <summary>
      /// Sets all singular values, that are lesser than relativeThreshold*(maximum singular value), to zero.
      /// </summary>
      /// <param name="relativeThreshold">The chop parameter, usually in the order 1E-5 or so.</param>
      public void ChopSingularValues(double relativeThreshold)
      {
        relativeThreshold = Math.Abs(relativeThreshold);

        double maxSingularValue = 0;
        for (int i = 0; i < s.Length; i++)
          maxSingularValue = Math.Max(maxSingularValue, s[i]);

        double thresholdLevel = maxSingularValue * relativeThreshold;

        // set singular values < thresholdLevel to zero
        for (int i = 0; i < s.Length; i++)
          if (s[i] < thresholdLevel)
            s[i] = 0;
      }

      public double Condition
      {
        get { return s[0] / s[Math.Min(m, n) - 1]; }
      }

      public double Norm2
      {
        get { return s[0]; }
      }

      public int Rank
      {
        get
        {
          double eps = Math.Pow(2.0, -52.0);
          double tol = Math.Max(m, n) * s[0] * eps;
          int r = 0;
          for (int i = 0; i < s.Length; i++)
            if (s[i] > tol)
              r++;
          return r;
        }
      }

      public double[] Diagonal
      {
        get { return s; }
      }

      /// <summary>
      /// Returns the Hat diagonal. The hat diagonal is the diagonal of the Hat
      /// matrix, which is defined as
      /// <code>
      ///          T  -1  T        T
      /// H = X  (X X)   X    = U U
      /// </code>
      /// </summary>
      public double[] HatDiagonal
      {
        get
        {
          if (_HatDiagonal is null)
          {
            BuildHatDiagonal();
          }
          return _HatDiagonal!;
        }
      }

      protected void BuildHatDiagonal()
      {
        _HatDiagonal = new double[u.Length];
        for (int i = 0; i < u.Length; i++)
        {
          double sum = 0;
          int cols = u[0].Length;
          for (int j = 0; j < cols; j++)
            sum += u[i][j] * u[i][j];

          _HatDiagonal[i] = sum;
        }
      }

      public double[][] U
      {
        get { return u; }
      }

      public double[][] V
      {
        get { return v; }
      }
    }

    #endregion SingularValueDecomposition
  } // end class MatrixMath
}
