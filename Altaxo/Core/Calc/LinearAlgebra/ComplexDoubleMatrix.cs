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
 * ComplexDoubleMatrix.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Text;
using System.Collections;


namespace Altaxo.Calc.LinearAlgebra
{

  ///<summary>
  /// Defines a matrix of ComplexDoubles.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [System.Serializable]
  sealed public class ComplexDoubleMatrix : IComplexDoubleMatrix, ICloneable, IFormattable, IEnumerable, ICollection, IList
  {
#if MANAGED
    internal Complex[][] data;
#else
    internal Complex[] data;
#endif
    int rows;
    int columns;

    ///<summary>Constructor for square matrix with its components set to zero.</summary>
    ///<param name="dimension">Dimensions of square matrix.</param>
    ///<exception cref="ArgumentException">dimension isn't positive.</exception>
    public ComplexDoubleMatrix(int dimension) : this(dimension, dimension) { }

    ///<summary>Constructor for matrix with its components set to zero.</summary>
    ///<param name="rows">Number of rows.</param>
    ///<param name="columns">Number of columns.</param>
    ///<exception cref="ArgumentException">dimensions aren't positive.</exception>
    public ComplexDoubleMatrix(int rows, int columns)
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
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
#else
      data = new Complex[(long)rows*(long)columns];
#endif

      this.rows = rows;
      this.columns = columns;
    }

    ///<summary>Constructor for square matrix with its components set to a specified value.</summary>
    ///<param name="dimension">Dimensions of square matrix.</param>
    ///<param name="value"><c>Complex</c> value to fill all matrix components.</param>
    ///<exception cref="ArgumentException">dimension parameter isn't positive</exception>
    public ComplexDoubleMatrix(int dimension, Complex value) : this(dimension, dimension, value) { }

    ///<summary>Constructor for matrix with components set to a specified value</summary>
    ///<param name="rows">Number of rows.</param>
    ///<param name="columns">Number of columns.</param>
    ///<param name="value"><c>Complex</c> value to fill all matrix components.</param>
    ///<exception cref="ArgumentException">dimension parameters aren't positive</exception>
    public ComplexDoubleMatrix(int rows, int columns, Complex value)
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
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
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
      data = new Complex[(long)rows*(long)columns];
      if(value != 0) {
        for( long i = 0; i < data.Length; i++){
          data[i] = value;
        }
      }
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="source"><c>ComplexDoubleMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(ComplexDoubleMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input ComplexDoubleMatrix cannot be null.");
      }
      this.rows = source.rows;
      this.columns = source.columns;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source.data[i][j];
        }
      }
#else
      data = new Complex[source.data.Length];
      for( long i = 0; i < data.Length; i++){
        data[i] = source.data[i];
      }
#endif
    }

    
    ///<summary>Constructor for matrix that makes a deep copy of a given <c>IROComplexDoubleMatrix</c>.</summary>
    ///<param name="source"><c>ComplexDoubleMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(IROComplexDoubleMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input ComplexDoubleMatrix cannot be null.");
      }

      this.rows = source.Rows;
      this.columns = source.Columns;
#if MANAGED
      data = new Complex[rows][];
      if(source is ComplexDoubleMatrix)
      {
        ComplexDoubleMatrix cdmsource = (ComplexDoubleMatrix)source;
        for (int i = 0; i < rows; i++)
          data[i] = (Complex[])cdmsource.data[i].Clone();
      }
      else
      {
        for (int i = 0; i < rows; i++)
        {
          data[i] = new Complex[columns];
        }

        for (int i = 0; i < rows; i++)
          for (int j = 0; j < columns; j++)
            data[i][j] = source[i,j];
      }
#else
      data = ToLinearComplexArray(source);
#endif
    }


    ///<summary>Constructor for matrix that makes a deep copy of a given <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="source"><c>ComplexFloatMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(ComplexFloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input ComplexFloatMatrix cannot be null.");
      }
      this.rows = source.RowLength;
      this.columns = source.ColumnLength;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source.data[i][j];
        }
      }
#else
      data = new Complex[source.data.Length];
      for( long i = 0; i < data.Length; i++){
        data[i] = source.data[i];
      }
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>ComplexFloatMatrix</c>.</summary>
    ///<param name="source"><c>ComplexFloatMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(IROComplexFloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input ComplexFloatMatrix cannot be null.");
      }
      this.rows = source.Rows;
      this.columns = source.Columns;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source[i,j];
        }
      }
#else
      data = ComplexDoubleMatrix.ToLinearComplexArray(source);
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>DoubleMatrix</c>.</summary>
    ///<param name="source"><c>DoubleMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(DoubleMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input DoubleMatrix cannot be null.");
      }
      this.rows = source.RowLength;
      this.columns = source.ColumnLength;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source.data[i][j];
        }
      }
#else
      data = new Complex[source.data.Length];
      for( long i = 0; i < data.Length; i++){
        data[i] = source.data[i];
      }
#endif
    }


    ///<summary>Constructor for matrix that makes a deep copy of a given <c>DoubleMatrix</c>.</summary>
    ///<param name="source"><c>DoubleMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(IROMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input DoubleMatrix cannot be null.");
      }
      this.rows = source.Rows;
      this.columns = source.Columns;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source[i,j];
        }
      }
#else
      data = ComplexDoubleMatrix.ToLinearComplexArray(source);
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>IROFloatMatrix</c>.</summary>
    ///<param name="source"><c>FloatMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(FloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input FloatMatrix cannot be null.");
      }
      this.rows = source.RowLength;
      this.columns = source.ColumnLength;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source.data[i][j];
        }
      }
#else
      data = new Complex[source.data.Length];
      for( long i = 0; i < data.Length; i++){
        data[i] = source.data[i];
      }
#endif
    }

    ///<summary>Constructor for matrix that makes a deep copy of a given <c>IROFloatMatrix</c>.</summary>
    ///<param name="source"><c>FloatMatrix</c> to deep copy into new matrix.</param>
    ///<exception cref="ArgumentNullException"><c>source</c> is null.</exception>
    public ComplexDoubleMatrix(IROFloatMatrix source)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source", "The input FloatMatrix cannot be null.");
      }
      this.rows = source.Rows;
      this.columns = source.Columns;
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = source[i,j];
        }
      }
#else
      data = ComplexDoubleMatrix.ToLinearComplexArray(source);
#endif
    }

    ///<summary>Constructor for matrix given an array of <c>Complex</c> values.</summary>
    ///<param name="values">Array of <c>Complex</c> values to fill matrix.</param>
    ///<exception cref="ArgumentNullException"><c>values</c> is null.</exception>
    public ComplexDoubleMatrix(Complex[,] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values", "The input ComplexDoubleMatrix cannot be null.");
      }
      this.rows = values.GetLength(0);
      this.columns = values.GetLength(1);
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = values[i, j];
        }
      }
#else
      data = new Complex[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          data[j*rows+i] = values[i,j];
        }
      }
#endif
    }

    ///<summary>Constructor for matrix given an array of <c>Complex</c> values.</summary>
    ///<param name="values">Array of <c>ComplexFloats</c> values to fill matrix.</param>
    ///<exception cref="ArgumentNullException"><c>values</c> is null.</exception>
    public ComplexDoubleMatrix(ComplexFloat[,] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values", "The input matrix cannot be null.");
      }
      this.rows = values.GetLength(0);
      this.columns = values.GetLength(1);
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = values[i, j];
        }
      }
#else
      data = new Complex[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          data[j*rows+i] = values[i,j];
        }
      }
#endif
    }

    ///<summary>Constructor for matrix given an array of <c>Complex</c> values.</summary>
    ///<param name="values">Array of <c>Cdouble</c> values to fill matrix.</param>
    ///<exception cref="ArgumentNullException"><c>values</c> is null.</exception>
    public ComplexDoubleMatrix(double[,] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values", "The input matrix cannot be null.");
      }
      this.rows = values.GetLength(0);
      this.columns = values.GetLength(1);
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = values[i, j];
        }
      }
#else
      data = new Complex[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          data[j*rows+i] = values[i,j];
        }
      }
#endif
    }

    ///<summary>Constructor for matrix given an array of <c>Complex</c> values.</summary>
    ///<param name="values">Array of <c>float</c> values to fill matrix.</param>
    ///<exception cref="ArgumentNullException"><c>values</c> is null.</exception>
    public ComplexDoubleMatrix(float[,] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values", "The input matrix cannot be null.");
      }
      this.rows = values.GetLength(0);
      this.columns = values.GetLength(1);
#if MANAGED
      data = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        data[i] = new Complex[columns];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          data[i][j] = values[i, j];
        }
      }
#else
      data = new Complex[(long)rows*(long)columns];
      for( int i = 0; i < rows; i++){
        for( int j = 0; j < columns; j++ ) {
          data[j*rows+i] = values[i,j];
        }
      }
#endif
    }

    ///<summary>Implicit conversion from <c>ComplexFloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>ComplexFloatMatrix</c> to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(ComplexFloatMatrix source)
    {
      return ToComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>ComplexFloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>ComplexFloatMatrix</c> to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(IROComplexFloatMatrix source)
    {
      if (source == null)
      {
        return null;
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(source.Rows, source.Columns);
#if MANAGED
      int rows = source.Rows, columns = source.Columns;
      if (source is ComplexFloatMatrix)
      {
        ComplexFloat[][] sourcedata = ((ComplexFloatMatrix)source).data;
        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            ret.data[i][j] = sourcedata[i][j];
          }
        }
      }
      else
      {
        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            ret.data[i][j] = source[i,j];
          }
        }
      }
#else
      ToLinearComplexArray(source,ret.data);
#endif
      return ret;
    }

    ///<summary>Implicit conversion from <c>DoubleMatrix</c> matrix.</summary>
    ///<param name="source"><c>DoubleMatrix</c> to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(DoubleMatrix source)
    {
      return ToComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>DoubleMatrix</c> matrix.</summary>
    ///<param name="source"><c>DoubleMatrix</c> to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(IROMatrix source)
    {
      
      if (source == null)
      {
        return null;
      }
      int rows = source.Rows, columns = source.Columns;
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rows, columns);
#if MANAGED
   
      if (source is DoubleMatrix)
      {
        double[][] sourcedata = ((DoubleMatrix)source).data;
        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            ret.data[i][j] = sourcedata[i][j];
          }
        }
      }
      else
      {
        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            ret.data[i][j] = source[i,j];
          }
        }
      }
#else
      ToLinearComplexArray(source,ret.data);
#endif
      return ret;
    }


    ///<summary>Implicit conversion from <c>FloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>FloatMatrix</c> to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(FloatMatrix source)
    {
      return ToComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>FloatMatrix</c> matrix.</summary>
    ///<param name="source"><c>FloatMatrix</c> to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(IROFloatMatrix source)
    {
      if (source == null)
      {
        return null;
      }
      int rows = source.Rows, columns = source.Columns;
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rows, columns);
#if MANAGED
            
      if (source is FloatMatrix)
      {
        float[][] sourcedata = ((FloatMatrix)source).data;

        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            ret.data[i][j] = sourcedata[i][j];
          }
        }
      }
      else
      {

        for (int i = 0; i < rows; i++)
        {
          for (int j = 0; j < columns; j++)
          {
            ret.data[i][j] = source[i,j];
          }
        }
      }
#else
      ToLinearComplexArray(source,ret.data);
#endif
      return ret;
    }




    ///<summary>Implicit conversion from <c>Complex</c> array.</summary>
    ///<param name="source"><c>Complex</c> array to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(Complex[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>Complex</c> array</summary>
    ///<param name="source"><c>Complex</c> array to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(Complex[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>ComplexFloat</c> array</summary>
    ///<param name="source"><c>ComplexFloat</c> array to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(ComplexFloat[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>ComplexFloat</c> array</summary>
    ///<param name="source"><c>ComplexFloat</c> array to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(ComplexFloat[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>double</c> array</summary>
    ///<param name="source"><c>double</c> array to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(double[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>double</c> array</summary>
    ///<param name="source"><c>double</c> array to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(double[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>float</c> array</summary>
    ///<param name="source"><c>float</c> array to make a deep copy conversion from.</param>
    static public implicit operator ComplexDoubleMatrix(float[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Implicit conversion from <c>float</c> array</summary>
    ///<param name="source"><c>float</c> array to make a deep copy conversion from.</param>
    static public ComplexDoubleMatrix ToComplexDoubleMatrix(float[,] source)
    {
      if (source == null)
      {
        return null;
      }
      return new ComplexDoubleMatrix(source);
    }

    ///<summary>Creates an identity matrix.</summary>
    ///<param name="rank">Rank of identity matrix.</param>
    public static ComplexDoubleMatrix CreateIdentity(int rank)
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rank);
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

    ///<summary>Return the number of rows in the <c>ComplexDoubleMatrix</c> variable.</summary>
    ///<returns>The number of rows.</returns>
    public int Rows
    {
      get
      {
        return rows;
      }
    }

    ///<summary>Return the number of columns in <c>ComplexDoubleMatrix</c> variable.</summary>
    ///<returns>The number of columns.</returns>
    public int Columns
    {
      get
      {
        return columns;
      }
    }

    ///<summary>Return the number of rows in the <c>ComplexDoubleMatrix</c> variable.</summary>
    ///<returns>The number of rows.</returns>
    public int RowLength
    {
      get
      {
        return rows;
      }
    }

    ///<summary>Return the number of columns in <c>ComplexDoubleMatrix</c> variable.</summary>
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
    ///<returns>Returns a <c>Complex</c> matrix element.</returns>
    public Complex this[int row, int column]
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

    ///<summary>Check if <c>ComplexDoubleMatrix</c> variable is the same as another object.</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexDoubleMatrix</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexDoubleMatrix</c> variable</returns>
    public override bool Equals(Object obj)
    {
      ComplexDoubleMatrix matrix = obj as ComplexDoubleMatrix;
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

    ///<summary>Return the Hashcode for the <c>ComplexDoubleMatrix</c></summary>
    ///<returns>The Hashcode representation of <c>ComplexDoubleMatrix</c></returns>
    public override int GetHashCode()
    {
      return (int)this.GetFrobeniusNorm();
    }


    ///<summary>Convert <c>ComplexDoubleMatrix</c> into <c>Complex</c> array</summary>
    ///<returns><c>Complex</c> array with data from <c>ComplexDoubleMatrix</c>.</returns>
    public Complex[,] ToArray()
    {
      Complex[,] ret = new Complex[rows, columns];
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

    ///<summary>Replace the <c>ComplexDoubleMatrix</c> with its transpose.</summary>
    public void Transpose()
    {
#if MANAGED
      Complex[][] temp = new Complex[columns][];
      for (int i = 0; i < columns; i++)
      {
        temp[i] = new Complex[rows];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
          temp[j][i] = data[i][j];
        }
      }
#else
      Complex[] temp = new Complex[(long)rows*(long)columns];
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

    ///<summary>Return the transpose of the <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns>The transpose of the <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleMatrix GetTranspose()
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(columns, rows);
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

    ///<summary>Replace the <c>ComplexDoubleMatrix</c> with its conjugate transpose.</summary>
    public void ConjugateTranspose()
    {
#if MANAGED
      Complex[][] temp = new Complex[columns][];
      for (int i = 0; i < columns; i++)
      {
        temp[i] = new Complex[rows];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; ++j)
        {
          temp[j][i] = ComplexMath.Conjugate(data[i][j]);
        }
      }
#else
      Complex[] temp = new Complex[(long)rows*(long)columns];
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

    ///<summary>Return the conjugate transpose of the <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns>The conjugate transpose of the <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleMatrix GetConjugateTranspose()
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(columns, rows);
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

    /// <summary>Returns an inverse of the <c>ComplexDoubleMatrix</c></summary>
    /// <returns>The inverse of the <c>ComplexDoubleMatrix</c>.</returns>
    /// <exception cref="SingularMatrixException">the matrix is singular.</exception>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public ComplexDoubleMatrix GetInverse()
    {
      if (rows != columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }
      ComplexDoubleLUDecomp lu = new ComplexDoubleLUDecomp(this);
      return lu.GetInverse();
    }

    /// <summary>Inverts the <c>ComplexDoubleMatrix</c>.</summary>
    /// <exception cref="SingularMatrixException">the matrix is singular.</exception>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public void Invert()
    {
      if (rows != columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }
      ComplexDoubleLUDecomp lu = new ComplexDoubleLUDecomp(this);
      ComplexDoubleMatrix temp = lu.GetInverse();
      this.data = temp.data;
    }

    /// <summary>Computes the determinant the <c>ComplexDoubleMatrix</c>.</summary>
    /// <returns>The determinant the <c>ComplexDoubleMatrix</c>.</returns>
    /// <exception cref="NotSquareMatrixException">the matrix is not square.</exception>
    public Complex GetDeterminant()
    {
      if (rows != columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }
      ComplexDoubleLUDecomp lu = new ComplexDoubleLUDecomp(this);
      return lu.GetDeterminant();
    }

    /// <summary>Calculates the L1 norm of this matrix.</summary>
    /// <returns>the L1 norm of this matrix.</returns>
    public double GetL1Norm()
    {
      double ret = 0;
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
        ret = System.Math.Max(ret, s);
      }
      return ret;
    }

    /// <summary>Calculates the L2 norm of this matrix.</summary>
    /// <returns>the L2 norm of this matrix.</returns>
    public double GetL2Norm()
    {
      return new ComplexDoubleSVDDecomp(this).Norm2;
    }

    /// <summary>Calculates the infinity norm of this matrix.</summary>
    /// <returns>the infinity norm of this matrix.</returns>
    public double GetInfinityNorm()
    {
      double ret = 0;
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
        ret = System.Math.Max(ret, s);
      }
      return ret;
    }

    /// <summary>Calculates the Frobenius norm of this matrix.</summary>
    /// <returns>the Frobenius norm of this matrix.</returns> 
    public double GetFrobeniusNorm()
    {
      ComplexDoubleMatrix tmp = this * this.GetConjugateTranspose();
      Complex ret = Complex.Zero;
      for (int i = 0; i < tmp.RowLength; i++)
      {
#if MANAGED
        ret += ComplexMath.Absolute(tmp.data[i][i]);
#else
        ret += ComplexMath.Absolute(tmp.data[i*rows+i]);
#endif
      }
      return System.Math.Sqrt(ret.Real);
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
      return new ComplexDoubleSVDDecomp(this).Condition;
    }

    ///<summary>Return a row of the <c>ComplexDoubleMatrix</c> as a <c>ComplexDoubleVector</c>.</summary>
    ///<param name="row">Row number to return.</param>
    ///<returns><c>ComplexDoubleVector</c> representation of row from the <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleVector GetRow(int row)
    {
      if (row < 0 || row >= rows)
      {
        throw new ArgumentOutOfRangeException("row", "row must be greater than or equal to zero and less than RowLength.");
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(columns);
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

    ///<summary>Return a column of the <c>ComplexDoubleMatrix</c> as a <c>ComplexDoubleVector</c>.</summary>
    ///<param name="column">Column number to return.</param>
    ///<returns><c>ComplexDoubleVector</c> representation of column from the <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleVector GetColumn(int column)
    {
      if (column < 0 || column >= columns)
      {
        throw new ArgumentOutOfRangeException("column", "column must be greater than or equal to zero and less than ColumnLength.");
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(rows);
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

    ///<summary>Return the diagonal of the <c>ComplexDoubleMatrix</c> as a <c>ComplexDoubleVector</c>.</summary>
    ///<returns><c>ComplexDoubleVector</c> representation of diagonal from the <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleVector GetDiagonal()
    {
      int min = System.Math.Min(rows, columns);
      ComplexDoubleVector ret = new ComplexDoubleVector(min);
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
    public void SetRow(int row, ComplexDoubleVector data)
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

    ///<summary>Sets the values of a row to the given vector.</summary>
    ///<param name="row">The row to set.</param>
    ///<param name="data">The data to file the row with.</param>
    public void SetRow(int row, IROComplexDoubleVector data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data vector cannot be null.");
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

      for( int i = 0; i < columns; i++ ) 
      {
        this.data[row][i] = data[i];
      }
#else
            for( int i = 0; i < columns; i++ ) {
                this.data[i*rows+row] = data[i];
            }
#endif
    }


    ///<summary>Sets the values of a row to the given array.</summary>
    ///<param name="row">The row to set.</param>
    ///<param name="data">The data to file the row with.</param>
    public void SetRow(int row, Complex[] data)
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
    public void SetColumn(int column, ComplexDoubleVector data)
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

    ///<summary>Sets the values of a column to the given vector.</summary>
    ///<param name="column">The column to set.</param>
    ///<param name="data">The data to file the column with.</param>
    public void SetColumn(int column, IROComplexDoubleVector data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data vector cannot be null.");
      }
      if (column < 0 || column >= columns)
      {
        throw new ArgumentOutOfRangeException("column", "column must be greater than or equal to zero and less than ColumnLength.");
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

    ///<summary>Sets the values of a column to the given array.</summary>
    ///<param name="column">The column to set.</param>
    ///<param name="data">The data to file the column with.</param>
    public void SetColumn(int column, Complex[] data)
    {
      if (data == null)
      {
        throw new ArgumentNullException("data", "The data vector cannot be null.");
      }
      if (column < 0 || column >= columns)
      {
        throw new ArgumentOutOfRangeException("column", "column must be greater than or equal to zero and less than ColumnLength.");
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

    ///<summary>Set the diagonal of the <c>ComplexDoubleMatrix</c> to the values in a <c>Complex</c> array.</summary>
    ///<param name="source"><c>Complex</c> arraywith values to insert into diagonal of <c>ComplexDoubleMatrix</c>.</param>
    public void SetDiagonal(Complex[] source)
    {
      int min = System.Math.Min(System.Math.Min(rows, columns), source.Length);
      for (int i = 0; i < min; i++)
      {
#if MANAGED
        data[i][i] = source[i];
#else
        data[i*rows+i] = source[i];
#endif
      }
    }


    ///<summary>Set the diagonal of the <c>ComplexDoubleMatrix</c> to the values in a <c>ComplexDoubleVector</c> variable.</summary>
    ///<param name="source"><c>ComplexDoubleVector</c> with values to insert into diagonal of <c>ComplexDoubleMatrix</c>.</param>
    public void SetDiagonal(IROComplexDoubleVector source)
    {
      int min = System.Math.Min(System.Math.Min(rows, columns), source.Length);
      for (int i = 0; i < min; i++)
      {
#if MANAGED
        data[i][i] = source[i];
#else
        data[i*rows+i] = source[i];
#endif
      }
    }

    ///<summary>Returns a submatrix of the <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="startRow">Return data from this row to last row.</param>
    ///<param name="startColumn">Return data from this column to last column.</param>
    ///<returns><c>ComplexDoubleMatrix</c> of submatrix specified by input variable.</returns>
    ///<exception cref="ArgumentException">input dimensions exceed those of <c>ComplexDoubleMatrix</c>.</exception>
    ///<exception cref="ArgumentOutOfRangeException">input dimensions are out of the range of <c>ComplexDoubleMatrix</c> dimensions.</exception>
    public ComplexDoubleMatrix GetSubMatrix(int startRow, int startColumn)
    {
      return GetSubMatrix(startRow, startColumn, rows - 1, columns - 1);
    }

    ///<summary>Returns a submatrix of the <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="startRow">Return data starting from this row.</param>
    ///<param name="startColumn">Return data starting from this column.</param>
    ///<param name="endRow">Return data ending in this row.</param>
    ///<param name="endColumn">Return data ending in this column.</param>
    ///<returns><c>ComplexDoubleMatrix</c> of submatrix specified by input variable.</returns>
    ///<exception cref="ArgumentException">input dimensions exceed those of <c>ComplexDoubleMatrix</c>.</exception>
    ///<exception cref="ArgumentOutOfRangeException">input dimensions are out of the range of<c>ComplexDoubleMatrix</c> dimensions.</exception>
    public ComplexDoubleMatrix GetSubMatrix(int startRow, int startColumn, int endRow, int endColumn)
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
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(nRows, nCols);

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

    ///<summary>Return the upper triangle values from <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns><c>ComplexDoubleMatrix</c> with upper triangle values from <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleMatrix GetUpperTriangle()
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rows, columns);
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

    ///<summary>Return the strictly upper triangle values from <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns><c>ComplexDoubleMatrix</c> with strictly upper triangle values from <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleMatrix GetStrictlyUpperTriangle()
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rows, columns);
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

    ///<summary>Return the lower triangle values from <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns><c>ComplexDoubleMatrix</c> with lower triangle values from <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleMatrix GetLowerTriangle()
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rows, columns);
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

    ///<summary>Return the strictly lower triangle values from <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns><c>ComplexDoubleMatrix</c> with strictly lower triangle values from <c>ComplexDoubleMatrix</c>.</returns>
    public ComplexDoubleMatrix GetStrictlyLowerTriangle()
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(rows, columns);
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

    ///<summary>Negate operator for <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns><c>ComplexDoubleMatrix</c> with values to negate.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator -(ComplexDoubleMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(a.rows, a.columns);
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

   

    ///<summary>Negate the values in <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns><c>ComplexDoubleMatrix</c> with values to negate.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Negate(ComplexDoubleMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return -a;
    }

    ///<summary>Subtract a <c>ComplexDoubleMatrix</c> from another <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to subtract from.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to subtract.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexDoubleMatrix operator -(ComplexDoubleMatrix a, ComplexDoubleMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      if (a.rows != b.rows || a.columns != b.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(a.rows, a.columns);
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

    ///<summary>Subtract a <c>ComplexDoubleMatrix</c> from a <c>Complex</c>.</summary>
    ///<param name="a"><c>Complex</c> to subtract from.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to subtract.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator -(Complex a, ComplexDoubleMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(b.rows, b.columns);
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

    ///<summary>Subtract a <c>Complex</c> from a <c>ComplexDoubleMatrix</c></summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to subtract from.</param>
    ///<param name="b"><c>Complex</c> to subtract.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator -(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(a.rows, a.columns);
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

    ///<summary>Subtract a <c>ComplexDoubleMatrix</c> from another <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to subtract from.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to subtract.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexDoubleMatrix Subtract(ComplexDoubleMatrix a, ComplexDoubleMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return a - b;
    }

    ///<summary>Subtract a <c>ComplexDoubleMatrix</c> from a <c>Complex</c>.</summary>
    ///<param name="a"><c>Complex</c> to subtract from.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to subtract.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Subtract(Complex a, ComplexDoubleMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a - b;
    }

    ///<summary>Subtract a <c>Complex</c> from a <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to subtract from.</param>
    ///<param name="b"><c>Complex</c> to subtract.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Subtract(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a - b;
    }

    ///<summary>Subtract a <c>ComplexDoubleMatrix</c> from this <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to subtract.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public void Subtract(ComplexDoubleMatrix a)
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

    ///<summary>Subtract a <c>Complex</c> from this <c>ComplexDoubleMatrix</c></summary>
    ///<param name="a"><c>Complex</c> to subtract.</param>
    public void Subtract(Complex a)
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

    ///<summary>Positive operator for <c>ComplexDoubleMatrix</c></summary>
    ///<returns><c>ComplexDoubleMatrix</c> with values to return</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator +(ComplexDoubleMatrix a)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a;
    }

    ///<summary>Add a <c>ComplexDoubleMatrix</c> to another <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to add to.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to add.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexDoubleMatrix operator +(ComplexDoubleMatrix a, ComplexDoubleMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      if (a.rows != b.rows || a.columns != b.columns)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(a.rows, a.columns);
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

    ///<summary>Add a <c>Complex</c> to a <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>Complex</c> to add to.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to add.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator +(Complex a, ComplexDoubleMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(b.rows, b.columns);
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

    ///<summary>Add a <c>ComplexDoubleMatrix</c> to a <c>Complex</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to add to.</param>
    ///<param name="b"><c>Complex</c> to add.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator +(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(a.rows, a.columns);
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

    ///<summary>Add a <c>ComplexDoubleMatrix</c> to another <c>ComplexDoubleMatrix</c>,</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to add to.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to add.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexDoubleMatrix Add(ComplexDoubleMatrix a, ComplexDoubleMatrix b)
    {
      if (a == null || b == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return a + b;
    }

    ///<summary>Add a <c>Complex</c> to a <c>ComplexDoubleMatrix</c>,</summary>
    ///<param name="a"><c>Complex</c> to add to.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to add.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Add(Complex a, ComplexDoubleMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a + b;
    }

    ///<summary>Add a <c>ComplexDoubleMatrix</c> to a <c>Complex</c>,</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to add to.</param>
    ///<param name="b"><c>Complex</c> to add.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Add(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a + b;
    }

    ///<summary>Add a <c>ComplexDoubleMatrix</c> to this<c>ComplexDoubleMatrix</c></summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> to add.</param>
    ///<exception cref="ArgumentException">matrices are not conformable.</exception>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public void Add(ComplexDoubleMatrix a)
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

    ///<summary>Add a <c>Complex</c> to this<c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="a"><c>Complex</c> to add.</param>
    public void Add(Complex a)
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

    ///<summary>Divide a <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> whose elements to divide as numerator.</param>
    ///<param name="b"><c>Complex</c> to divide by (as denominator).</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator /(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(a);
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

    ///<summary>Divide a <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> whose elements to divide as numerator.</param>
    ///<param name="b"><c>Complex</c> to divide by (as denominator).</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Divide(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a / b;
    }

    ///<summary>Divide this <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c></summary>
    ///<param name="a"><c>Complex</c> to divide by (as denominator).</param>
    public void Divide(Complex a)
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

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>Complex</c> to act as left operator.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to act as right operator.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator *(Complex a, ComplexDoubleMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(b);
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

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> whose elements to multiply.</param>
    ///<param name="b"><c>Complex</c> to multiply with.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix operator *(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return b * a;
    }

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>Complex</c> to act as left operator.</param>
    ///<param name="b"><c>ComplexDoubleMatrix</c> to act as right operator.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Multiply(Complex a, ComplexDoubleMatrix b)
    {
      if (b == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a * b;
    }

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>ComplexDoubleMatrix</c> whose elements to multiply.</param>
    ///<param name="b"><c>Complex</c> to multiply with.</param>
    ///<returns><c>ComplexDoubleMatrix</c> with results</returns>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public static ComplexDoubleMatrix Multiply(ComplexDoubleMatrix a, Complex b)
    {
      if (a == null)
      {
        throw new ArgumentNullException("Matrix cannot be null");
      }
      return a * b;
    }

    ///<summary>Multiply this <c>ComplexDoubleMatrix</c>'s elements with a <c>Complex</c>.</summary>
    ///<param name="a"><c>Complex</c> to multiply with.</param>
    public void Multiply(Complex a)
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

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c> with a <c>ComplexDoubleVector</c></summary>                
    ///<param name="x"><c>ComplexDoubleMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexDoubleVector</c> to act as right operator.</param>                
    ///<returns><c>ComplexDoubleMatrix</c> with results</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix or vector is null.</exception>
    public static ComplexDoubleVector operator *(ComplexDoubleMatrix x, ComplexDoubleVector y)
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

      ComplexDoubleVector ret = new ComplexDoubleVector(x.rows);
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

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c> with a <c>ComplexDoubleVector</c>.</summary>                
    ///<param name="x"><c>ComplexDoubleMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexDoubleVector</c> to act as right operator.</param>                
    ///<returns><c>ComplexDoubleMatrix</c> with results</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix or vector is null.</exception>
    public static ComplexDoubleVector Multiply(ComplexDoubleMatrix x, ComplexDoubleVector y)
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

    ///<summary>Multiply this <c>ComplexDoubleMatrix</c> with a <c>ComplexDoubleVector</c> and return results in this <c>ComplexDoubleMatrix</c>.</summary>                                                                                            
    ///<param name="x"><c>ComplexDoubleVector</c> to act as right operator.</param>                                                
    ///<exception cref="ArgumentException">Exception thrown if parameter dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException"><c>x</c> is null.</exception>
    public void Multiply(ComplexDoubleVector x)
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
      Complex[][] temp = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        temp[i] = new Complex[1];
      }
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          temp[i][0] += data[i][j] * x.data[j];
        }
      }
#else 
      Complex[] temp = new Complex[rows];
      Blas.Gemv.Compute(Blas.Order.ColumnMajor, Blas.Transpose.NoTrans, rows, columns, 1,data, x.Length, x.data, 1, 1, temp, 1);
#endif
      data = temp;
      columns = 1;
    }

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c> with a <c>ComplexDoubleMatrix.</c></summary>                
    ///<param name="x"><c>ComplexDoubleMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexDoubleMatrix</c> to act as right operator.</param>                
    ///<returns><c>ComplexDoubleMatrix</c> with results.</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexDoubleMatrix operator *(ComplexDoubleMatrix x, ComplexDoubleMatrix y)
    {
      if (x == null || y == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      if (x.columns != y.rows)
      {
        throw new ArgumentException("Matrices are not conformable.");
      }
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(x.rows, y.columns);
#if MANAGED
      Complex[] column = new Complex[x.columns];
      for (int j = 0; j < y.columns; j++)
      {
        for (int k = 0; k < x.columns; k++)
        {
          column[k] = y.data[k][j];
        }
        for (int i = 0; i < x.rows; i++)
        {
          Complex[] row = x.data[i];
          Complex s = 0;
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

    ///<summary>Multiply a <c>ComplexDoubleMatrix</c> with a <c>ComplexDoubleMatrix</c>.</summary>                
    ///<param name="x"><c>ComplexDoubleMatrix</c> to act as left operator.</param>                
    ///<param name="y"><c>ComplexDoubleMatrix</c> to act as right operator.</param>                
    ///<returns><c>ComplexDoubleMatrix</c> with results</returns>
    ///<exception cref="ArgumentException"> dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException">either matrix is null.</exception>
    public static ComplexDoubleMatrix Multiply(ComplexDoubleMatrix x, ComplexDoubleMatrix y)
    {
      if (x == null || y == null)
      {
        throw new ArgumentNullException("Matrices cannot be null");
      }
      return x * y;
    }

    ///<summary>Multiply this <c>ComplexDoubleMatrix</c> with a <c>ComplexDoubleMatrix</c> and return results in this <c>ComplexDoubleMatrix</c></summary>                                
    ///<param name="x"><c>ComplexDoubleMatrix</c> to act as right operator.</param>                
    ///<returns><c>ComplexDoubleMatrix</c> with results</returns>
    ///<exception cref="ArgumentException">dimensions are not conformable.</exception>
    ///<exception cref="ArgumentNullException"><c>x</c> is null.</exception>
    public void Multiply(ComplexDoubleMatrix x)
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
      Complex[][] temp = new Complex[rows][];
      for (int i = 0; i < rows; i++)
      {
        temp[i] = new Complex[x.columns];
      }
      Complex[] column = new Complex[columns];
      for (int j = 0; j < x.columns; j++)
      {
        for (int k = 0; k < columns; k++)
        {
          column[k] = x.data[k][j];
        }
        for (int i = 0; i < rows; i++)
        {
          Complex[] row = data[i];
          Complex s = 0;
          for (int k = 0; k < columns; k++)
          {
            s += row[k] * column[k];
          }
          temp[i][j] = s;
        }
      }
#else
      Complex[] temp = new Complex[(long)rows*(long)x.columns];
      Blas.Gemm.Compute(Blas.Order.ColumnMajor, Blas.Transpose.NoTrans, Blas.Transpose.NoTrans,
        rows, x.columns, columns, 1, data, rows, x.data, x.rows, 1, temp, rows);
#endif
      data = temp;
    }

    ///<summary>Copies the values from a matrix into this matrix.</summary>
    ///<param name="x">The matrix to copy the values from.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public void Copy(ComplexDoubleMatrix x)
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
    ///<summary>Clone (deep copy) a <c>ComplexDoubleMatrix</c> variable.</summary>
    public ComplexDoubleMatrix Clone()
    {
      return new ComplexDoubleMatrix(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>ComplexDoubleMatrix</c> variable.</summary>
    Object ICloneable.Clone()
    {
      return this.Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>ComplexDoubleMatrix</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>ComplexDoubleMatrix</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>ComplexDoubleMatrix</c>.</summary>
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
      return new ComplexDoubleMatrixEnumerator(this);
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

    ///<summary>Access a <c>ComplexDoubleMatrix</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>Complex</c> vector element</returns>
    object IList.this[int index]
    {
      get { return this[index % this.rows, index / this.rows]; }
      set { this[index % this.rows, index / this.rows] = (Complex)value; }
    }

    ///<summary>Add a new value to the end of the <c>DoubleMatrix</c></summary>
    public int Add(object value)
    {
      throw new System.NotSupportedException();
    }

    ///<summary>Set all values in the <c>DoubleMatrix</c> to zero </summary>
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

    ///<summary>Check if the any of the <c>DoubleVector</c> components equals a given <c>double</c></summary>
    public bool Contains(object value)
    {
      for (int i = 0; i < this.rows; i++)
        for (int j = 0; j < this.columns; j++)
#if MANAGED
          if (data[i][j] == (Complex)value)
#else
          if (data[j*rows+i]==(Complex)value)
#endif
            return true;

      return false;
    }

    ///<summary>Return the index of the <c>xDoubleVector</c> for the first component that equals a given <c>double</c></summary>
    public int IndexOf(object value)
    {
      for (int i = 0; i < this.rows; i++)
        for (int j = 0; j < this.columns; j++)
#if MANAGED
          if (data[i][j] == (Complex)value)
#else
          if (data[j*rows+i]==(Complex)value)
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

    #region Additions due to Adoption to Altaxo

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <param name="result">The resulting array must be given.</param>
    /// <returns>Linear array of complex.</returns>
    static void ToLinearComplexArray(IROComplexDoubleMatrix matrix, Complex[] result)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;

      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = matrix[i,j];

      
    }

  
    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <param name="result">The resulting array represented as a linear vector (at least of size (rows*columns).</param>
    /// <returns>Linear array of complex.</returns>
    static void ToLinearComplexArray(IROComplexFloatMatrix matrix, Complex[] result)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;

      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = matrix[i,j];

     
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <param name="result">The resulting linear array. Must be at least of size (rows*columns).</param>
    /// <returns>Linear array of complex.</returns>
    static void ToLinearComplexArray(IROMatrix matrix, Complex[] result)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;
      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = matrix[i,j];

     
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <param name="result">The result is stored here as a linear array of complex numbers.</param>
    static void ToLinearComplexArray(IROFloatMatrix matrix, Complex[] result)
    {
      int rows = matrix.Rows;
      int columns = matrix.Columns;
     
      int k=0;
      for(int j=0;j<columns;++j)
        for(int i=0;i<rows;++i)
          result[k++] = matrix[i,j];

     
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="source">The vector to convert to an array.</param>
    /// <param name="result">The result is stored here as a linear array of complex numbers</param>
    static void ToLinearComplexArray(IROComplexDoubleVector source, Complex[] result)
    {
      int length = source.Length;
      for(int i=0;i<length;++i)
        result[i] = source[i];
    }



    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static Complex[] ToLinearComplexArray(IROComplexDoubleMatrix matrix)
    {
      Complex[] result=new Complex[matrix.Rows*matrix.Columns];
      ToLinearComplexArray(matrix,result);
      return result;
    }

  
    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static Complex[] ToLinearComplexArray(IROComplexFloatMatrix matrix)
    {
      Complex[] result=new Complex[matrix.Rows*matrix.Columns];
      ToLinearComplexArray(matrix,result);
      return result;
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static Complex[] ToLinearComplexArray(IROMatrix matrix)
    {
      Complex[] result=new Complex[matrix.Rows*matrix.Columns];
      ToLinearComplexArray(matrix,result);
      return result;
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="matrix">The matrix to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static Complex[] ToLinearComplexArray(IROFloatMatrix matrix)
    {
      Complex[] result=new Complex[matrix.Rows*matrix.Columns];
      ToLinearComplexArray(matrix,result);
      return result;
    }

    /// <summary>
    /// This creates a linear array for use with unmanaged routines.
    /// </summary>
    /// <param name="source">The vector to convert to an array.</param>
    /// <returns>Linear array of complex.</returns>
    public static Complex[] ToLinearComplexArray(IROComplexDoubleVector source)
    {
      Complex[] result=new Complex[source.Length];
      ToLinearComplexArray(source,result);
      return result;
    }

    #endregion
  }
}

