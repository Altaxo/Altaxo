#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

/*
 * ComplexFloatMatrix.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Text;
using System.Collections;

namespace Altaxo.Calc.LinearAlgebra
{

  ///<summary>
  /// Defines a matrix of ComplexFloats.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [System.Serializable]
  sealed public class ComplexFloatMatrix : IComplexFloatMatrix, ICloneable, IFormattable, IEnumerable, ICollection, IList
  {
#if MANAGED
    internal ComplexFloat[][] data;
#else
    internal ComplexFloat[] data;
#endif
    int rows;
    int columns;

    ///<summary>Constructor for square matrix with its components set to zero.</summary>
    ///<param name="dimension">Dimensions of square matrix.</param>
    ///<exception cref="ArgumentException">dimension isn't positive.</exception>
    public ComplexFloatMatrix(int dimension) : this(dimension, dimension) { }

    ///<summary>Constructor for matrix with its components set to zero.</summary>
    ///<param name="rows">Number of rows.</param>
    ///<param name="columns">Number of columns.</param>
    ///<exception cref="ArgumentException">dimensions aren't positive.</exception>
    public ComplexFloatMatrix(int rows, int columns)
    {
      if (rows < 1)
      {
        throw new ArgumentException("Number of rows must be positive.", "rows");
      }
      if (columns < 1)
      {
        throw new ArgumentException("Number of columns must be positive.", "columns");
      }

#if MANAGED
      data = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new ComplexFloat[columns];
      }
#else
      data = new ComplexFloat[(long)rows*(long)columns];
#endif

      this.rows = rows;
      this.columns = columns;
    }

    ///<summary>Constructor for square matrix with its components set to a specified value.</summary>
    ///<param name="dimension">Dimensions of square matrix.</param>
    ///<param name="value"><c>ComplexFloat</c> value to fill all matrix components.</param>
    ///<exception cref="ArgumentException">dimension parameter isn't positive</exception>
    public ComplexFloatMatrix(int dimension, ComplexFloat value) : this(dimension, dimension, value) { }

    ///<summary>Constructor for matrix with components set to a specified value</summary>
    ///<param name="rows">Number of rows.</param>
    ///<param name="columns">Number of columns.</param>
    ///<param name="value"><c>ComplexFloat</c> value to fill all matrix components.</param>
    ///<exception cref="ArgumentException">dimension parameters aren't positive</exception>
    public ComplexFloatMatrix(int rows, int columns, ComplexFloat value)
    {
      if (rows < 1)
      {
        throw new ArgumentException("Number of rows must be positive.", "rows");
      }
      if (columns < 1)
      {
        throw new ArgumentException("Number of columns must be positive.", "columns");
      }

      this.rows = rows;
      this.columns = columns;
#if MANAGED
      data = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new ComplexFloat[columns];
      }
      if (value != 0)
      {
        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            data[i][j] = value;
          }
        }
      }
#else
      data = new ComplexFloat[(long)rows*(long)columns];
      if(value != 0) {
        for( long i = 0; i < data.Length; i++){
          data[i] = value;
        }
      }
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="source"><c>ComplexFloatMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexFloatMatrix(ComplexFloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input ComplexFloatMatrix cannot be null.");
      }
      this.rows = source.rows;
      this.columns = source.columns;
#if MANAGED
      data = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new ComplexFloat[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source.data[i][j];
        }
      }
#else
      data = new ComplexFloat[source.data.Length];
      for( long i = 0; i < data.Length; i++){
        data[i] = source.data[i];
      }
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>FloatMatrix</c>.</summary>
    ///<param name="source"><c>FloatMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexFloatMatrix(FloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input FloatMatrix cannot be null.");
      }
      this.rows = source.RowLength;
      this.columns = source.ColumnLength;
#if MANAGED
      data = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new ComplexFloat[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source.data[i][j];
        }
      }
#else
      data = new ComplexFloat[source.data.Length];
      for( long i = 0; i < data.Length; i++){
        data[i] = source.data[i];
      }
#endif
    }


    ///<summary>Constructor for matrix given an array of <c>ComplexFloat</c> values.</summary>
    ///<param name="values">Array of <c>ComplexFloat</c> values to fill matrix.</param>
    ///<exception cref="ArgumentNullException"><c>values</c> is null.</exception>
    public ComplexFloatMatrix(ComplexFloat[,] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values", "The input ComplexFloatMatrix cannot be null.");
      }
      this.rows = values.GetLength(0);
      this.columns = values.GetLength(1);
#if MANAGED
      data = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new ComplexFloat[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = values[i, j];
        }
      }
#else
      data = new ComplexFloat[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          data[j*rows+i] = values[i,j];
        }
      }
#endif
    }

    ///<summary>Constructor for matrix given an array of <c>ComplexFloat</c> values.</summary>
    ///<param name="values">Array of <c>float</c> values to fill matrix.</param>
    ///<exception cref="ArgumentNullException"><c>values</c> is null.</exception>
    public ComplexFloatMatrix(float[,] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values", "The input matrix cannot be null.");
      }
      this.rows = values.GetLength(0);
      this.columns = values.GetLength(1);
#if MANAGED
      data = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new ComplexFloat[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = values[i, j];
        }
      }
#else
      data = new ComplexFloat[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          data[j*rows+i] = values[i,j];
        }
      }
#endif
    }

    ///<summary>explicit conversion from <c>ComplexFloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>ComplexFloatMatrix</c> to make a deep copy conversion from.</param>
    static public explicit operator ComplexFloatMatrix(ComplexDoubleMatrix source)
    {
      if (source == null)
      {
        return null;
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(source.RowLength, source.ColumnLength);
#if MANAGED
      for (int i = 0; i < source.RowLength; i++)
      {
        for (int j = 0; j < source.ColumnLength; j++)
        {
          ret.data[i][j] = (ComplexFloat)source.data[i][j];
        }
      }
#else
      for( long i = 0; i < ret.data.Length; i++){
        ret.data[i] = (ComplexFloat)source.data[i];
      }
#endif
      return ret;
    }

    ///<summary>explicit conversion from <c>ComplexDoubleMatrix</c> matrix.</summary>
    ///<param name="source"><c>ComplexDoubleMatrix</c> to make a deep copy conversion from.</param>
    static public ComplexFloatMatrix ToComplexFloatMatrix(ComplexDoubleMatrix source)
    {
      if (source == null)
      {
        return null;
      }
      return (ComplexFloatMatrix)source;
    }

    ///<summary>Implicit conversion from <c>FloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>FloatMatrix</c> to make a deep copy conversion from.</param>
    static public implicit operator ComplexFloatMatrix(FloatMatrix source)
    {
      if (source == null)
      {
        return null;
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(source.RowLength, source.ColumnLength);
#if MANAGED
      for (int i = 0; i < source.RowLength; i++)
      {
        for (int j = 0; j < source.ColumnLength; j++)
        {
          ret.data[i][j] = source.data[i][j];
        }
      }
#else
      for( long i = 0; i < ret.data.Length; i++){
        ret.data[i] = source.data[i];
      }
#endif
      return ret;
    }

    ///<summary>Implicit conversion from <c>FloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>FloatMatrix</c> to make a deep copy conversion from.</param>
    static public ComplexFloatMatrix ToComplexFloatMatrix(FloatMatrix source)
    {
      if (source == null)
      {
        return null;
      }
      return (ComplexFloatMatrix)source;
    }

    ///<summary>explicit conversion from <c>Complex</c> array.</summary>
    ///<param name="source"><c>Complex</c> array to make a deep copy conversion from.</param>
    static public explicit operator ComplexFloatMatrix(Complex[,] source)
    {
      if (source == null)
      {
        return null;
      }
      int rows = source.GetLength(0);
      int columns = source.GetLength(1);
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rows, columns);
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          ret.data[i][j] = (ComplexFloat)source[i, j];
        }
      }
#else
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          ret.data[j*ret.rows+i] = (ComplexFloat)source[i,j];
        }
      }
#endif
      return ret;
    }

    ///<summary>explicit conversion from <c>Complex</c> array</summary>
    ///<param name="source"><c>Complex</c> array to make a deep copy conversion from.</param>
    static public ComplexFloatMatrix ToComplexFloatMatrix(Complex[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return (ComplexFloatMatrix)source;
    }

    ///<summary>implicit conversion from <c>ComplexFloat</c> array.</summary>
    ///<param name="source"><c>ComplexFloat</c> array to make a deep copy conversion from.</param>
    static public implicit operator ComplexFloatMatrix(ComplexFloat[,] source)
    {
      if (source == null)
      {
        return null;
      }
      int rows = source.GetLength(0);
      int columns = source.GetLength(1);
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rows, columns);
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          ret.data[i][j] = source[i, j];
        }
      }
#else
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          ret.data[j*ret.rows+i] = source[i,j];
        }
      }
#endif
      return ret;
    }

    ///<summary>implicit conversion from <c>ComplexFloat</c> array</summary>
    ///<param name="source"><c>ComplexFloat</c> array to make a deep copy conversion from.</param>
    static public ComplexFloatMatrix ToComplexFloatMatrix(ComplexFloat[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return (ComplexFloatMatrix)source;
    }


    ///<summary>Implicit conversion from <c>float</c> array</summary>
    ///<param name="source"><c>float</c> array to make a deep copy conversion from.</param>
    static public ComplexFloatMatrix ToComplexFloatMatrix(float[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexFloatMatrix(source);
    }

    ///<summary>Creates an identity matrix.</summary>
    ///<param name="rank">Rank of identity matrix.</param>
    public static ComplexFloatMatrix CreateIdentity(int rank)
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rank);
      for (int i = 0; i < rank; i++)
      {
#if MANAGED
        ret.data[i][i] = 1;
#else
        ret.data[i*ret.rows+i] = 1;
#endif
      }
      return ret;

    }

    ///<summary>Return the number of rows in the <c>ComplexFloatMatrix</c> variable.</summary>
    ///<returns>The number of rows.</returns>
    public int Rows
    {
      get
      {
        return rows;
      }
    }

    ///<summary>Return the number of columns in <c>ComplexFloatMatrix</c> variable.</summary>
    ///<returns>The number of columns.</returns>
    public int Columns
    {
      get
      {
        return columns;
      }
    }

    ///<summary>Return the number of rows in the <c>ComplexFloatMatrix</c> variable.</summary>
    ///<returns>The number of rows.</returns>
    public int RowLength
    {
      get
      {
        return rows;
      }
    }

    ///<summary>Return the number of columns in <c>ComplexFloatMatrix</c> variable.</summary>
    ///<returns>The number of columns.</returns>
    public int ColumnLength
    {
      get
      {
        return columns;
      }
    }

    ///<summary>Access a matrix element.</summary>
    ///<param name="row">The row to access.</param>
    ///<param name="column">The column to access.</param>
    ///<exception cref="ArgumentOutOfRangeException">element accessed is out of the bounds of the matrix.</exception>
    ///<returns>Returns a <c>ComplexFloat</c> matrix element.</returns>
    public ComplexFloat this[int row, int column]
    {
      get
      {
#if MANAGED
        return data[row][column];
#else
        return data[column*rows+row];
#endif
      }
      set
      {
#if MANAGED
        data[row][column] = value;
#else
        data[column*rows+row] = value;
#endif
      }
    }

    ///<summary>Check if <c>ComplexFloatMatrix</c> variable is the same as another object.</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexFloatMatrix</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexFloatMatrix</c> variable</returns>
    public override bool Equals(Object obj)
    {
      ComplexFloatMatrix matrix = obj as ComplexFloatMatrix;
      if ((Object)matrix == null)
      {
        return false;
      }

      if (rows != matrix.rows && columns != matrix.columns)
      {
        return false;
      }

#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          if (data[i][j] != matrix.data[i][j])
          {
            return false;
          }
        }
      }
#else
      for( int i = 0; i < data.Length; i++) {
        if( data[i] != matrix.data[i] ) {
          return false;
        }
      }
#endif
      return true;
    }

    ///<summary>Return the Hashcode for the <c>ComplexFloatMatrix</c></summary>
    ///<returns>The Hashcode representation of <c>ComplexFloatMatrix</c></returns>
    public override int GetHashCode()
    {
      return (int)this.GetFrobeniusNorm();
    }

    ///<summary>Convert <c>ComplexFloatMatrix</c> into <c>ComplexFloat</c> 2D array</summary>
    ///<returns><c>ComplexFloat</c> 2D array with data from <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloat[,] ToArray()
    {
      ComplexFloat[,] ret = new ComplexFloat[rows, columns];
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
#if MANAGED
          ret[i, j] = data[i][j];
#else
          ret[i,j] = data[j*rows+i];
#endif
        }
      }
      return ret;
    }


    ///<summary>Replace the <c>ComplexFloatMatrix</c> with its transpose.</summary>
    public void Transpose()
    {
#if MANAGED
      ComplexFloat[][] temp = new ComplexFloat[columns][];
      for (int i = 0; i < columns; i++)
      {
        temp[i] = new ComplexFloat[rows];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
          temp[j][i] = data[i][j];
        }
      }
#else
      ComplexFloat[] temp = new ComplexFloat[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++ ) {
        for( int j = 0; j < columns; ++j ) {
          temp[i*columns+j] = data[j*rows+i];
        }
      }
#endif
      data = temp;
      int tmp = columns;
      columns = rows;
      rows = tmp;
    }

    ///<summary>Return the transpose of the <c>ComplexFloatMatrix</c>.</summary>
    ///<returns>The transpose of the <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatMatrix GetTranspose()
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(columns, rows);
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
          ret.data[j][i] = data[i][j];
        }
      }
#else
      for( int i = 0; i < rows; i++ ) {
        for( int j = 0; j < columns; ++j ) {
          ret.data[i*columns+j] = data[j*rows+i];
        }
      }
#endif
      return ret;
    }

    ///<summary>Replace the <c>ComplexFloatMatrix</c> with its conjugate transpose.</summary>
    public void ConjugateTranspose()
    {
#if MANAGED
      ComplexFloat[][] temp = new ComplexFloat[columns][];
      for (int i = 0; i < columns; i++)
      {
        temp[i] = new ComplexFloat[rows];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
          temp[j][i] = ComplexMath.Conjugate(data[i][j]);
        }
      }
#else
      ComplexFloat[] temp = new ComplexFloat[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++ ) {
        for( int j = 0; j < columns; ++j ) {
          temp[i*columns+j] = ComplexMath.Conjugate(data[j*rows+i]);
        }
      }
#endif
      data = temp;
      int tmp = columns;
      columns = rows;
      rows = tmp;
    }

    ///<summary>Return the conjugate transpose of the <c>ComplexFloatMatrix</c>.</summary>
    ///<returns>The conjugate transpose of the <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatMatrix GetConjugateTranspose()
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(columns, rows);
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
          ret.data[j][i] = ComplexMath.Conjugate(data[i][j]);
        }
      }
#else
      for( int i = 0; i < rows; i++ ) {
        for( int j = 0; j < columns; ++j ) {
          ret.data[i*columns+j] = ComplexMath.Conjugate(data[j*rows+i]);
        }
      }
#endif
      return ret;
    }

    /// <summary>Returns an inverse of the <c>ComplexFloatMatrix</c></summary>
    /// <returns>The inverse of the <c>ComplexFloatMatrix</c>.</returns>
    /// <exception cref="SingularMatrixException">the matrix is singular.</exception>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public ComplexFloatMatrix GetInverse()
    {
      if (rows != columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }
      ComplexFloatLUDecomp lu = new ComplexFloatLUDecomp(this);
      return lu.GetInverse();
    }

    /// <summary>Inverts the <c>ComplexFloatMatrix</c>.</summary>
    /// <exception cref="SingularMatrixException">the matrix is singular.</exception>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public void Invert()
    {
      if (rows != columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }
      ComplexFloatLUDecomp lu = new ComplexFloatLUDecomp(this);
      ComplexFloatMatrix temp = lu.GetInverse();
      this.data = temp.data;
    }

    /// <summary>Computes the determinant the <c>ComplexFloatMatrix</c>.</summary>
    /// <returns>The determinant the <c>ComplexFloatMatrix</c>.</returns>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public ComplexFloat GetDeterminant()
    {
      if (rows != columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }
      ComplexFloatLUDecomp lu = new ComplexFloatLUDecomp(this);
      return lu.GetDeterminant();
    }

    /// <summary>Calculates the L1 norm of this matrix.</summary>
    /// <returns>the L1 norm of this matrix.</returns>
    public float GetL1Norm()
    {
      float ret = 0;
      for (int j = 0; j < columns; j++)
      {
        double s = 0;
        for (int i = 0; i < rows; i++)
        {
#if MANAGED
          s += ComplexMath.Absolute(data[i][j]);
#else
          s += ComplexMath.Absolute(data[j*rows+i]);
#endif
        }
        ret = (float)System.Math.Max(ret, s);
      }
      return ret;
    }

    /// <summary>Calculates the L2 norm of this matrix.</summary>
    /// <returns>the L2 norm of this matrix.</returns>
    public double GetL2Norm()
    {
      return new ComplexFloatSVDDecomp(this).Norm2;
    }

    /// <summary>Calculates the infinity norm of this matrix.</summary>
    /// <returns>the infinity norm of this matrix.</returns>
    public float GetInfinityNorm()
    {
      float ret = 0;
      for (int i = 0; i < rows; i++)
      {
        double s = 0;
        for (int j = 0; j < columns; j++)
        {
#if MANAGED
          s += ComplexMath.Absolute(data[i][j]);
#else
          s += ComplexMath.Absolute(data[j*rows+i]);
#endif
        }
        ret = (float)System.Math.Max(ret, s);
      }
      return ret;
    }

    /// <summary>Calculates the Frobenius norm of this matrix.</summary>
    /// <returns>the Frobenius norm of this matrix.</returns> 
    public float GetFrobeniusNorm()
    {
      ComplexFloatMatrix tmp = this * this.GetConjugateTranspose();
      ComplexFloat ret = ComplexFloat.Zero;
      for (int i = 0; i < tmp.RowLength; i++)
      {
#if MANAGED
        ret += ComplexMath.Absolute(tmp.data[i][i]);
#else
        ret += ComplexMath.Absolute(tmp.data[i*rows+i]);
#endif
      }
      return (float)System.Math.Sqrt(ret.Real);
    }

    ///<summary>Calculates the condition number of the matrix.</summary>
    ///<returns>the condition number of the matrix.</returns>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public double GetConditionNumber()
    {
      if (this.rows != this.columns)
      {
        throw new NotSquareMatrixException();
      }
      return new ComplexFloatSVDDecomp(this).Condition;
    }

    ///<summary>Return a row of the <c>ComplexFloatMatrix</c> as a <c>ComplexFloatVector</c>.</summary>
    ///<param name="row">Row number to return.</param>
    ///<returns><c>ComplexFloatVector</c> representation of row from the <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatVector GetRow(int row)
    {
      if (row < 0 || row >= rows)
      {
        throw new ArgumentOutOfRangeException("row", "row must be greater than or equal to zero and less than RowLength.");
      }
      ComplexFloatVector ret = new ComplexFloatVector(columns);
      for (int i = 0; i < columns; i++)
      {
#if MANAGED
        ret.data[i] = data[row][i];
#else
        ret.data[i] = data[i*rows+row];
#endif
      }
      return ret;
    }

    ///<summary>Return a column of the <c>ComplexFloatMatrix</c> as a <c>ComplexFloatVector</c>.</summary>
    ///<param name="column">Column number to return.</param>
    ///<returns><c>ComplexFloatVector</c> representation of column from the <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatVector GetColumn(int column)
    {
      if (column < 0 || column >= columns)
      {
        throw new ArgumentOutOfRangeException("column", "column must be greater than or equal to zero and less than ColumnLength.");
      }
      ComplexFloatVector ret = new ComplexFloatVector(rows);
      for (int i = 0; i < rows; i++)
      {
#if MANAGED
        ret.data[i] = data[i][column];
#else
        ret.data[i] = data[column*rows+i];
#endif
      }
      return ret;
    }

    ///<summary>Return the diagonal of the <c>ComplexFloatMatrix</c> as a <c>ComplexFloatVector</c>.</summary>
    ///<returns><c>ComplexFloatVector</c> representation of diagonal from the <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatVector GetDiagonal()
    {
      int min = System.Math.Min(rows, columns);
      ComplexFloatVector ret = new ComplexFloatVector(min);
      for (int i = 0; i < min; i++)
      {
#if MANAGED
        ret.data[i] = data[i][i];
#else
        ret.data[i] = data[i*rows+i];
#endif
      }
      return ret;
    }

    ///<summary>Sets the values of a row to the given vector.</summary>
    ///<param name="row">The row to set.</param>
    ///<param name="data">The data to file the row with.</param>
    public void SetRow(int row, ComplexFloatVector data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data vector cannot be null.");
      }
      if (row < 0 || row >= rows)
      {
        throw new ArgumentOutOfRangeException("row", "row must be greater than or equal to zero and less than RowLength.");
      }
      if (data.data.Length != columns)
      {
        throw new ArgumentException("data length does not equal the matrix column length.");
      }
#if MANAGED
      Array.Copy(data.data, 0, this.data[row], 0, data.data.Length);
#else
            for( int i = 0; i < columns; i++ ) {
                this.data[i*rows+row] = data.data[i];
            }
#endif
    }

    ///<summary>Sets the values of a row to the given array.</summary>
    ///<param name="row">The row to set.</param>
    ///<param name="data">The data to file the row with.</param>
    public void SetRow(int row, ComplexFloat[] data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data array cannot be null.");
      }
      if (row < 0 || row >= rows)
      {
        throw new ArgumentOutOfRangeException("row", "row must be greater than or equal to zero and less than RowLength.");
      }
      if (data.Length != columns)
      {
        throw new ArgumentException("data length does not equal the matrix column length.");
      }
#if MANAGED
      Array.Copy(data, 0, this.data[row], 0, data.Length);
#else
            for( int i = 0; i < columns; i++ ) {
                this.data[i*rows+row] = data[i];
            }
#endif
    }

    ///<summary>Sets the values of a column to the given vector.</summary>
    ///<param name="column">The column to set.</param>
    ///<param name="data">The data to file the column with.</param>
    public void SetColumn(int column, ComplexFloatVector data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data vector cannot be null.");
      }
      if (column < 0 || column >= columns)
      {
        throw new ArgumentOutOfRangeException("column", "column must be greater than or equal to zero and less than ColumnLength.");
      }
      if (data.data.Length != rows)
      {
        throw new ArgumentException("data length does not equal the matrix row length.");
      }
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        this.data[i][column] = data.data[i];
      }
#else
            for( int i = 0; i < rows; i++ ) {
                this.data[column*rows+i] = data.data[i];
            }
#endif
    }

    ///<summary>Sets the values of a column to the given array.</summary>
    ///<param name="column">The column to set.</param>
    ///<param name="data">The data to file the column with.</param>
    public void SetColumn(int column, ComplexFloat[] data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data vector cannot be null.");
      }
      if (column < 0 || column >= columns)
      {
        throw new ArgumentOutOfRangeException("column", "column must greater than or equal to zero, and less than ColumnLength.");
      }
      if (data.Length != rows)
      {
        throw new ArgumentException("data length does not equal the matrix row length.");
      }
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        this.data[i][column] = data[i];
      }
#else
            for( int i = 0; i < rows; i++ ) {
                this.data[column*rows+i] = data[i];
            }
#endif
    }


    ///<summary>Set the diagonal of the <c>ComplexFloatMatrix</c> to the values in a <c>ComplexFloatVector</c> variable.</summary>
    ///<param name="source"><c>ComplexFloatVector</c> with values to insert into diagonal of <c>ComplexFloatMatrix</c>.</param>
    public void SetDiagonal(ComplexFloatVector source)
    {
      int min = System.Math.Min(System.Math.Min(rows, columns), source.Length);
      for (int i = 0; i < min; i++)
      {
#if MANAGED
        data[i][i] = source.data[i];
#else
        data[i*rows+i] = source.data[i];
#endif
      }
    }

    ///<summary>Returns a submatrix of the <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="startRow">Return data from this row to last row.</param>
    ///<param name="startColumn">Return data from this column to last column.</param>
    ///<returns><c>ComplexFloatMatrix</c> of submatrix specified by input variable.</returns>
    ///<exception cref="ArgumentException">input dimensions exceed those of <c>ComplexFloatMatrix</c>.</exception>
    ///<exception cref="ArgumentOutOfRangeException">input dimensions are out of the range of <c>ComplexFloatMatrix</c> dimensions.</exception>
    public ComplexFloatMatrix GetSubMatrix(int startRow, int startColumn)
    {
      return GetSubMatrix(startRow, startColumn, rows - 1, columns - 1);
    }

    ///<summary>Returns a submatrix of the <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="startRow">Return data starting from this row.</param>
    ///<param name="startColumn">Return data starting from this column.</param>
    ///<param name="endRow">Return data ending in this row.</param>
    ///<param name="endColumn">Return data ending in this column.</param>
    ///<returns><c>ComplexFloatMatrix</c> of submatrix specified by input variable.</returns>
    ///<exception cref="ArgumentException">input dimensions exceed those of <c>ComplexFloatMatrix</c>.</exception>
    ///<exception cref="ArgumentOutOfRangeException">input dimensions are out of the range of<c>ComplexFloatMatrix</c> dimensions.</exception>
    public ComplexFloatMatrix GetSubMatrix(int startRow, int startColumn, int endRow, int endColumn)
    {
      if (startRow > endRow)
      {
        throw new ArgumentOutOfRangeException("The starting Row must be less that the ending Row.");
      }
      if (startColumn > endColumn)
      {
        throw new ArgumentOutOfRangeException("The starting column must be less that the ending column.");
      }
      if (startRow < 0 || startColumn < 0 || endRow >= rows || endColumn >= columns)
      {
        throw new ArgumentOutOfRangeException("startRow and startColumn must be greater than or equal to zero, endRow must be less than RowLength, and endColumn must be less than ColumnLength.");
      }
      int nRows = endRow - startRow + 1;
      int nCols = endColumn - startColumn + 1;
      ComplexFloatMatrix ret = new ComplexFloatMatrix(nRows, nCols);

      for (int i = 0; i < nRows; i++)
      {
        for (int j = 0; j < nCols; j++)
        {
#if MANAGED
          ret.data[i][j] = data[i + startRow][j + startColumn];
#else
          ret.data[j*nRows+i] = data[(j+startColumn)*rows+(i+startRow)];
#endif
        }
      }
      return ret;
    }

    ///<summary>Return the upper triangle values from <c>ComplexFloatMatrix</c>.</summary>
    ///<returns><c>ComplexFloatMatrix</c> with upper triangle values from <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatMatrix GetUpperTriangle()
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rows, columns);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          if (i <= j)
          {
#if MANAGED
            ret.data[i][j] = data[i][j];
#else
            ret.data[j*rows+i] = data[j*rows+i];
#endif
          }
        }
      }
      return ret;
    }

    ///<summary>Return the strictly upper triangle values from <c>ComplexFloatMatrix</c>.</summary>
    ///<returns><c>ComplexFloatMatrix</c> with strictly upper triangle values from <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatMatrix GetStrictlyUpperTriangle()
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rows, columns);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          if (i < j)
          {
#if MANAGED
            ret.data[i][j] = data[i][j];
#else
            ret.data[j*rows+i] = data[j*rows+i];
#endif
          }
        }
      }
      return ret;
    }

    ///<summary>Return the lower triangle values from <c>ComplexFloatMatrix</c>.</summary>
    ///<returns><c>ComplexFloatMatrix</c> with lower triangle values from <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatMatrix GetLowerTriangle()
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rows, columns);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          if (i >= j)
          {
#if MANAGED
            ret.data[i][j] = data[i][j];
#else
            ret.data[j*rows+i] = data[j*rows+i];
#endif
          }
        }
      }
      return ret;
    }

    ///<summary>Return the strictly lower triangle values from <c>ComplexFloatMatrix</c>.</summary>
    ///<returns><c>ComplexFloatMatrix</c> with strictly lower triangle values from <c>ComplexFloatMatrix</c>.</returns>
    public ComplexFloatMatrix GetStrictlyLowerTriangle()
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(rows, columns);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          if (i > j)
          {
#if MANAGED
            ret.data[i][j] = data[i][j];
#else
            ret.data[j*rows+i] = data[j*rows+i];
#endif
          }
        }
      }
      return ret;
    }

    ///<summary>Negate operator for <c>ComplexFloatMatrix</c>.</summary>
    ///<returns><c>ComplexFloatMatrix</c> with values to negate.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator -(ComplexFloatMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }

      ComplexFloatMatrix ret = new ComplexFloatMatrix(a.rows, a.columns);
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          ret.data[i][j] = -a.data[i][j];
        }
      }
#else
      for( int i = 0; i < a.data.Length; i++){
        ret.data[i] = - a.data[i];
      }
#endif
      return ret;
    }

    ///<summary>Negate the values in <c>ComplexFloatMatrix</c>.</summary>
    ///<returns><c>ComplexFloatMatrix</c> with values to negate.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Negate(ComplexFloatMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return -a;
    }

    ///<summary>Subtract a <c>ComplexFloatMatrix</c> from another <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to subtract from.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to subtract.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexFloatMatrix operator -(ComplexFloatMatrix a, ComplexFloatMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      if (a.rows != b.rows || a.columns != b.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(a.rows, a.columns);
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          ret.data[i][j] = a.data[i][j] - b.data[i][j];
        }
      }
#else
      for( int i = 0; i < a.data.Length; i++){
        ret.data[i] = a.data[i] - b.data[i];
      }
#endif
      return ret;
    }

    ///<summary>Subtract a <c>ComplexFloatMatrix</c> from a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to subtract from.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to subtract.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator -(ComplexFloat a, ComplexFloatMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(b.rows, b.columns);
#if MANAGED
      for (int i = 0; i < b.rows; i++)
      {
        for (int j = 0; j < b.columns; j++)
        {
          ret.data[i][j] = a - b.data[i][j];
        }
      }
#else
      for( int i = 0; i < b.data.Length; i++){
        ret.data[i] = a - b.data[i];
      }
#endif
      return ret;
    }

    ///<summary>Subtract a <c>ComplexFloat</c> from a <c>ComplexFloatMatrix</c></summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to subtract from.</param>
    ///<param name="b"><c>ComplexFloat</c> to subtract.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator -(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(a.rows, a.columns);
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          ret.data[i][j] = a.data[i][j] - b;
        }
      }
#else
      for( int i = 0; i < a.data.Length; i++){
        ret.data[i] = a.data[i] - b;
      }
#endif
      return ret;
    }

    ///<summary>Subtract a <c>ComplexFloatMatrix</c> from another <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to subtract from.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to subtract.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexFloatMatrix Subtract(ComplexFloatMatrix a, ComplexFloatMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return a - b;
    }

    ///<summary>Subtract a <c>ComplexFloatMatrix</c> from a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to subtract from.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to subtract.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Subtract(ComplexFloat a, ComplexFloatMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a - b;
    }

    ///<summary>Subtract a <c>ComplexFloat</c> from a <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to subtract from.</param>
    ///<param name="b"><c>ComplexFloat</c> to subtract.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Subtract(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a - b;
    }

    ///<summary>Subtract a <c>ComplexFloatMatrix</c> from this <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to subtract.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public void Subtract(ComplexFloatMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      if (rows != a.rows || columns != a.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          data[i][j] -= a.data[i][j];
        }
      }
#else
      for( int i = 0; i < data.Length; i++){
        data[i] -= a.data[i];
      }
#endif
    }

    ///<summary>Subtract a <c>ComplexFloat</c> from this <c>ComplexFloatMatrix</c></summary>
    ///<param name="a"><c>ComplexFloat</c> to subtract.</param>
    public void Subtract(ComplexFloat a)
    {
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] -= a;
        }
      }
#else
      for( int i = 0; i < data.Length; i++){
        data[i] -= a;
      }
#endif
    }

    ///<summary>Positive operator for <c>ComplexFloatMatrix</c></summary>
    ///<returns><c>ComplexFloatMatrix</c> with values to return</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator +(ComplexFloatMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a;
    }

    ///<summary>Add a <c>ComplexFloatMatrix</c> to another <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to add to.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to add.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexFloatMatrix operator +(ComplexFloatMatrix a, ComplexFloatMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      if (a.rows != b.rows || a.columns != b.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(a.rows, a.columns);
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          ret.data[i][j] = a.data[i][j] + b.data[i][j];
        }
      }
#else
      for( int i = 0; i < a.data.Length; i++){
        ret.data[i] = a.data[i] + b.data[i];
      }
#endif
      return ret;
    }

    ///<summary>Add a <c>ComplexFloat</c> to a <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to add to.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to add.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator +(ComplexFloat a, ComplexFloatMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(b.rows, b.columns);
#if MANAGED
      for (int i = 0; i < b.rows; i++)
      {
        for (int j = 0; j < b.columns; j++)
        {
          ret.data[i][j] = a + b.data[i][j];
        }
      }
#else
      for( int i = 0; i < b.data.Length; i++){
        ret.data[i] = a + b.data[i];
      }
#endif
      return ret;
    }

    ///<summary>Add a <c>ComplexFloatMatrix</c> to a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to add to.</param>
    ///<param name="b"><c>ComplexFloat</c> to add.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator +(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(a.rows, a.columns);
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          ret.data[i][j] = a.data[i][j] + b;
        }
      }
#else
      for( int i = 0; i < a.data.Length; i++){
        ret.data[i] = a.data[i] + b;
      }
#endif
      return ret;
    }

    ///<summary>Add a <c>ComplexFloatMatrix</c> to another <c>ComplexFloatMatrix</c>,</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to add to.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to add.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexFloatMatrix Add(ComplexFloatMatrix a, ComplexFloatMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return a + b;
    }

    ///<summary>Add a <c>ComplexFloat</c> to a <c>ComplexFloatMatrix</c>,</summary>
    ///<param name="a"><c>ComplexFloat</c> to add to.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to add.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Add(ComplexFloat a, ComplexFloatMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a + b;
    }

    ///<summary>Add a <c>ComplexFloatMatrix</c> to a <c>ComplexFloat</c>,</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to add to.</param>
    ///<param name="b"><c>ComplexFloat</c> to add.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Add(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a + b;
    }

    ///<summary>Add a <c>ComplexFloatMatrix</c> to this<c>ComplexFloatMatrix</c></summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> to add.</param>
    ///<exception cref="ArgumentException">matrices are not conformable.</exception>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public void Add(ComplexFloatMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      if (rows != a.rows || columns != a.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
#if MANAGED
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          data[i][j] += a.data[i][j];
        }
      }
#else
      for( int i = 0; i < data.Length; i++){
        data[i] += a.data[i];
      }
#endif
    }

    ///<summary>Add a <c>ComplexFloat</c> to this<c>ComplexFloatMatrix</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to add.</param>
    public void Add(ComplexFloat a)
    {
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] += a;
        }
      }
#else
      for( int i = 0; i < data.Length; i++){
        data[i] += a;
      }
#endif
    }

    ///<summary>Divide a <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> whose elements to divide as numerator.</param>
    ///<param name="b"><c>ComplexFloat</c> to divide by (as denominator).</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator /(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(a);
#if MANAGED
      for (int i = 0; i < ret.rows; i++)
      {
        for (int j = 0; j < ret.columns; j++)
        {
          ret.data[i][j] /= b;
        }
      }
#else
      Blas.Scal.Compute( ret.data.Length, 1/b, ret.data, 1 );
#endif
      return ret;
    }

    ///<summary>Divide a <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> whose elements to divide as numerator.</param>
    ///<param name="b"><c>ComplexFloat</c> to divide by (as denominator).</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Divide(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a / b;
    }

    ///<summary>Divide this <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c></summary>
    ///<param name="a"><c>ComplexFloat</c> to divide by (as denominator).</param>
    public void Divide(ComplexFloat a)
    {
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] /= a;
        }
      }
#else
      Blas.Scal.Compute( data.Length, 1/a, data, 1 );
#endif
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to act as left operator.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to act as right operator.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator *(ComplexFloat a, ComplexFloatMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(b);
#if MANAGED
      for (int i = 0; i < ret.rows; i++)
      {
        for (int j = 0; j < ret.columns; j++)
        {
          ret.data[i][j] *= a;
        }
      }
#else
      Blas.Scal.Compute( ret.data.Length, a, ret.data, 1 );
#endif
      return ret;
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> whose elements to multiply.</param>
    ///<param name="b"><c>ComplexFloat</c> to multiply with.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix operator *(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return b * a;
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to act as left operator.</param>
    ///<param name="b"><c>ComplexFloatMatrix</c> to act as right operator.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Multiply(ComplexFloat a, ComplexFloatMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a * b;
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloatMatrix</c> whose elements to multiply.</param>
    ///<param name="b"><c>ComplexFloat</c> to multiply with.</param>
    ///<returns><c>ComplexFloatMatrix</c> with results</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexFloatMatrix Multiply(ComplexFloatMatrix a, ComplexFloat b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a * b;
    }

    ///<summary>Multiply this <c>ComplexFloatMatrix</c>'s elements with a <c>ComplexFloat</c>.</summary>
    ///<param name="a"><c>ComplexFloat</c> to multiply with.</param>
    public void Multiply(ComplexFloat a)
    {
#if MANAGED
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] *= a;
        }
      }
#else 
      Blas.Scal.Compute( data.Length, a, data, 1 );
#endif
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c> with a <c>ComplexFloatVector</c></summary>                
    ///<param name="x"><c>ComplexFloatMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexFloatVector</c> to act as right operator.</param>                
    ///<returns><c>ComplexFloatMatrix</c> with results</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix or vector is null.</exception>
    public static ComplexFloatVector operator *(ComplexFloatMatrix x, ComplexFloatVector y)
    {
      if (x == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      if (y == null)
      {
        throw new ArgumentNullException("Vector cannot be null");
      }
      if (x.columns != y.Length)
      {
        throw new ArgumentException("Vector and Matrix are not conformable.");
      }

      ComplexFloatVector ret = new ComplexFloatVector(x.rows);
#if MANAGED
      for (int i = 0; i < x.rows; i++)
      {
        for (int j = 0; j < x.columns; j++)
        {
          ret.data[i] += x.data[i][j] * y.data[j];
        }
      }
#else 
      Blas.Gemv.Compute(Blas.Order.ColumnMajor, Blas.Transpose.NoTrans, x.rows, x.columns, 1, x.data, x.rows, y.data, 1, 1, ret.data, 1);
#endif
      return ret;
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c> with a <c>ComplexFloatVector</c>.</summary>                
    ///<param name="x"><c>ComplexFloatMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexFloatVector</c> to act as right operator.</param>                
    ///<returns><c>ComplexFloatMatrix</c> with results</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix or vector is null.</exception>
    public static ComplexFloatVector Multiply(ComplexFloatMatrix x, ComplexFloatVector y)
    {
      if (x == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      if (y == null)
      {
        throw new ArgumentNullException("Vector cannot be null");
      }
      return x * y;
    }

    ///<summary>Multiply this <c>ComplexFloatMatrix</c> with a <c>ComplexFloatVector</c> and return results in this <c>ComplexFloatMatrix</c>.</summary>                                                                                            
    ///<param name="x"><c>ComplexFloatVector</c> to act as right operator.</param>                                                
    ///<exception cref="ArgumentException">Exception thrown if parameter dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException"><c>x</c> is null.</exception>
    public void Multiply(ComplexFloatVector x)
    {
      if (x == null)
      {
        throw new ArgumentNullException("x", "Vector cannot be null");
      }
      if (columns != x.Length)
      {
        throw new ArgumentException("Vector and matrix are not conformable.");
      }
#if MANAGED
      ComplexFloat[][] temp = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        temp[i] = new ComplexFloat[1];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          temp[i][0] += data[i][j] * x.data[j];
        }
      }
#else 
      ComplexFloat[] temp = new ComplexFloat[rows];
      Blas.Gemv.Compute(Blas.Order.ColumnMajor, Blas.Transpose.NoTrans, rows, columns, 1,data, x.Length, x.data, 1, 1, temp, 1);
#endif
      data = temp;
      columns = 1;
    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c> with a <c>ComplexFloatMatrix.</c></summary>                
    ///<param name="x"><c>ComplexFloatMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexFloatMatrix</c> to act as right operator.</param>                
    ///<returns><c>ComplexFloatMatrix</c> with results.</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexFloatMatrix operator *(ComplexFloatMatrix x, ComplexFloatMatrix y)
    {
      if (x == null || y == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      if (x.columns != y.rows)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
      ComplexFloatMatrix ret = new ComplexFloatMatrix(x.rows, y.columns);
#if MANAGED
      ComplexFloat[] column = new ComplexFloat[x.columns];
      for (int j = 0; j < y.columns; j++)
      {
        for (int k = 0; k < x.columns; k++)
        {
          column[k] = y.data[k][j];
        }
        for (int i = 0; i < x.rows; i++)
        {
          ComplexFloat[] row = x.data[i];
          ComplexFloat s = 0;
          for (int k = 0; k < x.columns; k++)
          {
            s += row[k] * column[k];
          }
          ret.data[i][j] = s;
        }
      }
#else
      Blas.Gemm.Compute(Blas.Order.ColumnMajor, Blas.Transpose.NoTrans, Blas.Transpose.NoTrans,
        x.rows, y.columns, x.columns, 1, x.data, x.rows, y.data, y.rows, 1, ret.data, ret.rows);
#endif
      return ret;

    }

    ///<summary>Multiply a <c>ComplexFloatMatrix</c> with a <c>ComplexFloatMatrix</c>.</summary>                
    ///<param name="x"><c>ComplexFloatMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexFloatMatrix</c> to act as right operator.</param>                
    ///<returns><c>ComplexFloatMatrix</c> with results</returns>
    ///<exception cref="ArgumentException"> dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexFloatMatrix Multiply(ComplexFloatMatrix x, ComplexFloatMatrix y)
    {
      if (x == null || y == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return x * y;
    }

    ///<summary>Multiply this <c>ComplexFloatMatrix</c> with a <c>ComplexFloatMatrix</c> and return results in this <c>ComplexFloatMatrix</c></summary>                                
    ///<param name="x"><c>ComplexFloatMatrix</c> to act as right operator.</param>                
    ///<returns><c>ComplexFloatMatrix</c> with results</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException"><c>x</c> is null.</exception>
    public void Multiply(ComplexFloatMatrix x)
    {
      if (x == null)
      {
        throw new ArgumentNullException("x", "Vector cannot be null");
      }
      if (columns != x.rows)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
#if MANAGED
      ComplexFloat[][] temp = new ComplexFloat[rows][];
      for (int i = 0; i < rows; i++)
      {
        temp[i] = new ComplexFloat[x.columns];
      }
      ComplexFloat[] column = new ComplexFloat[columns];
      for (int j = 0; j < x.columns; j++)
      {
        for (int k = 0; k < columns; k++)
        {
          column[k] = x.data[k][j];
        }
        for (int i = 0; i < rows; i++)
        {
          ComplexFloat[] row = data[i];
          ComplexFloat s = 0;
          for (int k = 0; k < columns; k++)
          {
            s += row[k] * column[k];
          }
          temp[i][j] = s;
        }
      }
#else
      ComplexFloat[] temp = new ComplexFloat[(long)rows*(long)x.columns];
      Blas.Gemm.Compute(Blas.Order.ColumnMajor, Blas.Transpose.NoTrans, Blas.Transpose.NoTrans,
        rows, x.columns, columns, 1, data, rows, x.data, x.rows, 1, temp, rows);
#endif
      data = temp;
    }

    ///<summary>Copies the values from a matrix into this matrix.</summary>
    ///<param name="x">The matrix to copy the values from.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public void Copy(ComplexFloatMatrix x)
    {
      if (x == null)
      {
        throw new ArgumentNullException("x", "Matrix cannot be null");
      }
      if (this.rows != x.rows || this.columns != x.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
#if MANAGED
      for (int i = 0; i < x.rows; i++)
      {
        for (int j = 0; j < x.columns; j++)
        {
          this.data[i][j] = x.data[i][j];
        }
      }
#else
      Blas.Copy.Compute(this.data.Length, x.data, 1, this.data, 1 );
#endif
    }

    ///<summary>Clone (deep copy) a <c>ComplexFloatMatrix</c> variable.</summary>
    public ComplexFloatMatrix Clone()
    {
      return new ComplexFloatMatrix(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>ComplexFloatMatrix</c> variable.</summary>
    Object ICloneable.Clone()
    {
      return this.Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>ComplexFloatMatrix</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format and provider.</returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
      StringBuilder sb = new StringBuilder("rows: ");
      sb.Append(rows).Append(", cols: ").Append(columns).Append(System.Environment.NewLine);

      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          sb.Append(this[i, j].ToString(format, formatProvider));
          if (j != columns - 1)
          {
            sb.Append(", ");
          }
        }
        if (i != rows - 1)
        {
          sb.Append(System.Environment.NewLine);
        }
      }
      return sb.ToString();
    }

    // --- IEnumerable Interface ---
    ///<summary> Return an IEnumerator </summary>
    public IEnumerator GetEnumerator()
    {
      return new ComplexFloatMatrixEnumerator(this);
    }

    // --- ICollection Interface ---
    ///<summary> Get the number of elements in the matrix </summary>
    public int Count
    {
      get { return this.rows * this.columns; }
    }
    int ICollection.Count
    {
      get { return this.Count; }
    }

    ///<summary> Get a boolean indicating whether the data storage method of this vector is thread-safe</summary>
    public bool IsSynchronized
    {
      get { return this.data.IsSynchronized; }
    }
    bool ICollection.IsSynchronized
    {
      get { return this.IsSynchronized; }
    }

    ///<summary> Get an object that can be used to synchronize the data storage method of this vector</summary>
    object ICollection.SyncRoot
    {
      get { return this.data.SyncRoot; }
    }

    ///<summary> Copy the components of this vector to an array </summary>
    public void CopyTo(Array array, int index)
    {
      this.data.CopyTo(array, index);
    }
    void ICollection.CopyTo(Array array, int index)
    {
      this.CopyTo(array, index);
    }

    // --- IList Interface ---

    ///<summary>Returns true indicating that the IList interface doesn't support addition and removal of elements</summary>
    public bool IsFixedSize
    {
      get { return true; }
    }

    ///<summary>Returns false indicating that the IList interface supports modification of elements</summary>
    public bool IsReadOnly
    {
      get { return false; }
    }

    ///<summary>Access a <c>ComplexFloatMatrix</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown in element accessed is out of the bounds of the matrix.</exception>
    ///<returns>Returns a <c>ComplexFloat</c> vector element</returns>
    object IList.this[int index]
    {
      get { return this[index % this.rows, index / this.rows]; }
      set { this[index % this.rows, index / this.rows] = (ComplexFloat)value; }
    }

    ///<summary>Add a new value to the end of the <c>ComplexFloatMatrix</c></summary>
    public int Add(object value)
    {
      throw new System.NotSupportedException();
    }

    ///<summary>Set all values in the <c>ComplexFloatMatrix</c> to zero </summary>
    public void Clear()
    {
      for (int i = 0; i < this.rows; i++)
        for (int j = 0; j < this.columns; j++)
#if MANAGED
          data[i][j] = 0;
#else
          data[j*rows+i]=0;
#endif
    }

    ///<summary>Check if the any of the <c>ComplexFloatMatrix</c> components equals a given <c>ComplexFloat</c></summary>
    public bool Contains(object value)
    {
      for (int i = 0; i < this.rows; i++)
        for (int j = 0; j < this.columns; j++)
#if MANAGED
          if (data[i][j] == (ComplexFloat)value)
#else
          if (data[j*rows+i]==(ComplexFloat)value)
#endif
            return true;

      return false;
    }

    ///<summary>Return the index of the <c>ComplexFloatMatrix</c> for the first component that equals a given <c>ComplexFloat</c></summary>
    public int IndexOf(object value)
    {
      for (int i = 0; i < this.rows; i++)
        for (int j = 0; j < this.columns; j++)
#if MANAGED
          if (data[i][j] == (ComplexFloat)value)
#else
          if (data[j*rows+i]==(ComplexFloat)value)
#endif
            return j * rows + i;
      return -1;
    }

    ///<summary>Insert a <c>double</c> into the <c>DoubleVector</c> at a given index</summary>
    public void Insert(int index, object value)
    {
      throw new System.NotSupportedException();
    }

    ///<summary>Remove the first instance of a given <c>double</c> from the <c>DoubleVector</c></summary>
    public void Remove(object value)
    {
      throw new System.NotSupportedException();
    }

    ///<summary>Remove the component of the <c>DoubleVector</c> at a given index</summary>
    public void RemoveAt(int index)
    {
      throw new System.NotSupportedException();
    }

    #region Additions due to Adoption


    ///<summary>Constructor for matrix that makes a deep copy of a given <c>IROComplexDoubleMatrix</c>.</summary>
    ///<param name="source"><c>ComplexDoubleMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexFloatMatrix(IROComplexFloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input ComplexDoubleMatrix cannot be null.");
      }

      this.rows = source.Rows;
      this.columns = source.Columns;
#if MANAGED
      data = new ComplexFloat[rows][];
      if (source is ComplexFloatMatrix)
      {
        ComplexFloatMatrix cdmsource = (ComplexFloatMatrix)source;
        for (int i = 0; i < rows; i++)
          data[i] = (ComplexFloat[])cdmsource.data[i].Clone();
      }
      else
      {
        for (int i = 0; i < rows; i++)
        {
          data[i] = new ComplexFloat[columns];
        }

        for (int i = 0; i < rows; i++)
          for (int j = 0; j < columns; j++)
            data[i][j] = source[i,j];
      }
#else
      data = ToLinearComplexArray(source);
#endif
    }

    #endregion

    #region Additions due to Adoption to Altaxo

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static ComplexFloat[] ToLinearComplexArray(IROComplexDoubleMatrix matrix)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;

      ComplexFloat[] result = new ComplexFloat[rows*columns];

      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = (ComplexFloat)matrix[i,j];

      return result;
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static ComplexFloat[] ToLinearComplexArray(IROMatrix matrix)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;

      ComplexFloat[] result = new ComplexFloat[rows*columns];

      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = (ComplexFloat)matrix[i,j];

      return result;
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static ComplexFloat[] ToLinearComplexArray(IROComplexFloatMatrix matrix)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;

      ComplexFloat[] result = new ComplexFloat[rows*columns];

      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = matrix[i,j];

      return result;
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="source">The vector to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static ComplexFloat[] ToLinearComplexArray(IROComplexFloatVector source)
    {
      int length = source.Length;
      ComplexFloat[] result = new ComplexFloat[length];
      for(int i=0;i<length;++i)
        result[i] = source[i];

      return result;
    }

    #endregion
  }
}

