#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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


using System;

namespace Altaxo.Calc.LinearAlgebra
{
 
  /// <summary>
  /// Class MatrixMath provides common static methods for matrix manipulation
  /// and arithmetic in tow dimensions.
  /// </summary>
  public class MatrixMath
  {
    #region Helper matrix implementations

    /// <summary>
    /// BEMatrix is a matrix implementation that is relatively easy to extend to the botton, i.e. to append rows.
    /// It is horizontal oriented, i.e. the storage is as a number of horizontal vectors.
    /// </summary>
    public class BEMatrix : IMatrix, IBottomExtensibleMatrix
    {
      /// <summary>The rows of the matrix = number of double[] arrays in it.</summary>
      private int m_Rows;
      /// <summary>The cols of the matrix = length of each double[] array.</summary>
      private int m_Cols;
      /// <summary>The array which holds the matrix.</summary>
      private double[][] m_Array;

      /// <summary>
      /// Sets up an empty matrix with dimension(row,cols).
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of cols of the matrix.</param>
      public BEMatrix(int rows, int cols)
      {
        SetDimension(rows,cols);
      }

      /// <summary>
      /// Uses an already existing array for the matrix data.
      /// </summary>
      /// <param name="x">Jagged double array containing the matrix data. The data are used directly (no copy)!</param>
      public BEMatrix(double[][]x)
      {
        this.m_Array = x;
        this.m_Rows = m_Array.Length;
        this.m_Cols = this.m_Rows==0 ? 0 : this.m_Array[0].Length;
      }

      public void Clear()
      {
        SetDimension(0,0);
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null,this);
      }


      #region IMatrix Members


      /// <summary>
      /// Set up the dimensions of the matrix. Discards the old content and reset the matrix with the new dimensions. All elements
      /// become zero.
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of columns of the matrix.</param>
      public void SetDimension(int rows, int cols)
      {
        m_Rows = rows;
        m_Cols = cols;
        m_Array = new double[2*(rows+32)][];
        for(int i=0;i<m_Rows;i++)
          m_Array[i] = new double[cols];
      }

      /// <summary>
      /// Element accessor. Accesses the element [row, col] of the matrix.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return m_Array[row][col];
        }
        set
        {
          m_Array[row][col] = value;
        }
      }

      /// <summary>
      /// Number of Rows of the matrix.
      /// </summary>
      public int Rows
      {
        get
        {
          return m_Rows;
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int Columns
      {
        get
        {
          return m_Cols;
        }
      }

      #endregion

      #region IBottomExtensibleMatrix Members

      /// <summary>
      /// Appends the matrix a at the bottom of this matrix. Either this matrix must be empty (dimensions (0,0)) or
      /// the matrix to append must have the same number of columns than this matrix.
      /// </summary>
      /// <param name="a">Matrix to append to the bottom of this matrix.</param>
      public void AppendBottom(IROMatrix a)
      {
        if(a.Rows==0)
          return; // nothing to append

        if(this.Columns>0)
        {
          if(a.Columns!=this.Columns) // throw an error if this column is not empty and the columns does not match
            throw new ArithmeticException(string.Format("The number of columns of this matrix ({0}) and of the matrix to append ({1}) does not match!",this.Columns,a.Columns)); 
        }
        else // if the matrix was empty before
        {
          m_Cols = a.Columns;
        }

        int newRows = a.Rows + this.Rows;

        // we must reallocate the array if neccessary
        if(newRows>=m_Array.Length)
        {
          double[][] newArray = new double[2*(newRows+32)][]; 
      
          for(int i=0;i<m_Rows;i++)
            newArray[i] = m_Array[i]; // copy the existing horizontal vectors.

          m_Array = newArray;
        }

        // copy the new rows now
        for(int i=m_Rows;i<newRows;i++)
        {
          m_Array[i] = new double[m_Cols]; // create new horizontal vectors for the elements to append
          for(int j=0;j<m_Cols;j++)
            m_Array[i][j] = a[i-m_Rows,j]; // copy the elements
        }
        
        m_Rows = newRows;
      }

      #endregion
    }


    /// <summary>
    /// REMatrix is a matrix implementation that is relatively easy to extend to the right, i.e. to append columns.
    /// It is vertical oriented, i.e. the storage is as a number of vertical vectors.
    /// </summary>
    public class REMatrix : IMatrix, IRightExtensibleMatrix
    {
      /// <summary>The rows of the matrix = length of each double[] array.</summary>
      private int m_Rows;
      /// <summary>The cols of the matrix = number of double[] arrays in it.</summary>
      private int m_Cols;
      /// <summary>The array which holds the matrix.</summary>
      private double[][] m_Array;

      /// <summary>
      /// Sets up an empty matrix with dimension(row,cols).
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of cols of the matrix.</param>
      public REMatrix(int rows, int cols)
      {
        SetDimension(rows,cols);
      }

      /// <summary>
      /// Constructs an RE matrix from an array of double vectors. Attention! The double vectors (the second) dimensions are here
      /// the columns (!) of the matrix. The data is not copied.
      /// </summary>
      /// <param name="from"></param>
      public REMatrix(double[][] from)
      {
        m_Cols = from.Length;
        m_Rows = from[0].Length;
        for (int i = 1; i < m_Cols; i++)
          if (from[i].Length != m_Rows)
            throw new ArgumentException("The columns of the provided array (second dimension here) are not of equal length");

        m_Array = from;
      }

      public void Clear()
      {
        SetDimension(0,0);
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null,this);
      }


      #region IMatrix Members


      /// <summary>
      /// Set up the dimensions of the matrix. Discards the old content and reset the matrix with the new dimensions. All elements
      /// become zero.
      /// </summary>
      /// <param name="rows">Number of rows of the matrix.</param>
      /// <param name="cols">Number of columns of the matrix.</param>
      public void SetDimension(int rows, int cols)
      {
        m_Rows = rows;
        m_Cols = cols;
        m_Array = new double[2*(cols+32)][];
        for(int i=0;i<m_Cols;i++)
          m_Array[i] = new double[rows];
      }

      /// <summary>
      /// Element accessor. Accesses the element [row, col] of the matrix.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return m_Array[col][row];
        }
        set
        {
          m_Array[col][row] = value;
        }
      }

      /// <summary>
      /// Number of Rows of the matrix.
      /// </summary>
      public int Rows
      {
        get
        {
          return m_Rows;
        }
      }

      /// <summary>
      /// Number of columns of the matrix.
      /// </summary>
      public int Columns
      {
        get
        {
          return m_Cols;
        }
      }

      #endregion

      #region IRightExtensibleMatrix Members

      /// <summary>
      /// Appends the matrix a at the right of this matrix. Either this matrix must be empty (dimensions (0,0)) or
      /// the matrix to append must have the same number of rows than this matrix.
      /// </summary>
      /// <param name="a">Matrix to append to the right of this matrix.</param>
      public void AppendRight(IROMatrix a)
      {
        if(a.Columns==0)
          return; // nothing to append

        if(this.Rows>0)
        {
          if(a.Rows!=this.Rows) // throw an error if this column is not empty and the columns does not match
            throw new ArithmeticException(string.Format("The number of rows of this matrix ({0}) and of the matrix to append ({1}) does not match!",this.Rows,a.Rows)); 
        }
        else // if the matrix was empty before set the number of rows
        {
          m_Rows = a.Rows;
        }

        int newCols = a.Columns + this.Columns;
        
        // we must newly allocate the bone array, if neccessary
        if(newCols>=m_Array.Length)
        {
          double[][] newArray = new double[2*(newCols+32)][]; 
      
          for(int i=0;i<m_Cols;i++)
            newArray[i] = m_Array[i]; // copy the existing horizontal vectors.

          m_Array = newArray;
        }

        // copy the new rows
        for(int i=m_Cols;i<newCols;i++)
        {
          m_Array[i] = new double[m_Rows]; // create new horizontal vectors for the elements to append
          for(int j=0;j<m_Rows;j++)
            m_Array[i][j] = a[j,i-m_Cols]; // copy the elements
        }
        
        m_Cols = newCols;
      }

      #endregion
    }

    /// <summary>
    /// Implements a horizontal vector, i.e. a matrix which has only one row, but many columns.
    /// </summary>
    public class HorizontalVector : IMatrix, IVector
    {
      /// <summary>
      /// Holds the elements of the vector 
      /// </summary>
      private double[] m_Array;

      /// <summary>
      /// Creates a Horizontal vector of length cols.
      /// </summary>
      /// <param name="cols">Initial length of the vector.</param>
      public HorizontalVector(int cols)
      {
        m_Array = new double[cols];
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null,this);
      }


      #region IMatrix Members

      /// <summary>
      /// Element accessor. The argument rows should be zero, but no exception is thrown if it is not zero.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return m_Array[col];
        }
        set
        {
          m_Array[col] = value;
        }
      }

      /// <summary>
      /// Number of rows. Returns always 1 (one).
      /// </summary>
      public int Rows
      {
        get
        {
          return 1;
        }
      }

      /// <summary>
      /// Number of columns, i.e. number of elements of the horizontal vector.
      /// </summary>
      public int Columns
      {
        get
        {
          return m_Array.Length;
        }
      }

      #endregion

      #region IVector Members

      public double this[int i]
      {
        get
        {
          return m_Array[i];
        }
        set
        {
          m_Array[i] = value;
        }
      }

      #endregion

      #region IROVector Members


      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      public int UpperBound
      {
        get
        {
          return m_Array.Length-1;
        }
      }

      public int Length
      {
        get
        {
          return m_Array.Length;
        }
      }

      #endregion
    }


    /// <summary>
    /// Implements a vertical vector, i.e. a matrix which has only one column, but many rows.
    /// </summary>
    public class VerticalVector : IMatrix, IVector
    {
      /// <summary>
      /// Holds the elements of the vertical vector.
      /// </summary>
      private double[] m_Array;

      /// <summary>
      /// Creates a vertical vector which has an initial length of rows.
      /// </summary>
      /// <param name="rows">Initial length of the vertical vector.</param>
      public VerticalVector(int rows)
      {
        m_Array = new double[rows];
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null,this);
      }


      #region IMatrix Members

      /// <summary>
      /// Element accessor. The argument col should be zero here, but no exception is thrown if it is not zero.
      /// </summary>
      public double this[int row, int col]
      {
        get
        {
          return m_Array[row];
        }
        set
        {
          m_Array[row] = value;
        }
      }

      /// <summary>
      /// Number of Rows = elements of the vector.
      /// </summary>
      public int Rows
      {
        get
        {
          return m_Array.Length;
        }
      }

      /// <summary>
      /// Number of columns of the matrix, always 1 (one) since it is a vertical vector.
      /// </summary>
      public int Columns
      {
        get
        {
          return 1;
        }
      }

      #endregion

      #region IVector Members

      public double this[int i]
      {
        get
        {
          return m_Array[i];
        }
        set
        {
          m_Array[i] = value;
        }
      }

      #endregion

      #region IROVector Members

    

      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      public int UpperBound
      {
        get
        {
          return m_Array.Length-1;
        }
      }

      public int Length
      {
        get
        {
          return m_Array.Length;
        }
      }

      #endregion
    }

    /// <summary>
    /// Implements a scalar as a special case of the matrix which has the dimensions (1,1).
    /// </summary>
    public class Scalar : IMatrix, IVector
    {
      /// <summary>
      /// Holds the only element of the matrix.
      /// </summary>
      double m_Value;

      /// <summary>
      /// Creates the scalar and initializes it with the value val.
      /// </summary>
      /// <param name="val"></param>
      public Scalar(double val)
      {
        m_Value = val;
      }

      /// <summary>
      /// Creates the scalar with the default value of zero.
      /// </summary>
      public Scalar()
      {
      }

      public override string ToString()
      {
        return MatrixMath.MatrixToString(null,this);
      }


      /// <summary>
      /// Converts the scalar to a double if neccessary.
      /// </summary>
      /// <param name="s">The scalar to convert.</param>
      /// <returns>The value of the element[0,0], which is the only element of the scalar.</returns>
      public static implicit operator double(Scalar s)
      {
        return s.m_Value;
      }

      /// <summary>
      /// Converts a double to a scalar where neccessary.
      /// </summary>
      /// <param name="d">The double value to convert.</param>
      /// <returns>The scalar representation of this double value.</returns>
      public static implicit operator Scalar(double d)
      {
        return new Scalar(d);
      }

      #region IMatrix Members

      /// <summary>
      /// Element accessor. Both col and row should be zero, but this is not justified here. Always returns the value of the scalar.
      /// </summary>
      public double this[int i, int k]
      {
        get
        {
          return m_Value;
        }
        set
        {
          m_Value = value;
        }
      }

      /// <summary>
      /// Number of rows of the matrix. Always 1 (one).
      /// </summary>
      public int Rows
      {
        get
        {
          return 1;
        }
      }

      /// <summary>
      /// Number of columns of the matrix. Always 1 (one).
      /// </summary>
      public int Columns
      {
        get
        {
          return 1;
        }
      }

      #endregion

      #region IVector Members

      public double this[int i]
      {
        get
        {
          return this.m_Value;
        }
        set
        {
          this.m_Value = value;
        }
      }

      #endregion

      #region IROVector Members

     

      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      public int UpperBound
      {
        get
        {
          return 0;
        }
      }

      public int Length
      {
        get
        {
         
          return 1;
        }
      }

      #endregion
    }

  

    #endregion

    #region Type conversion classes

    #region MatrixRowROVector

    /// <summary>
    /// Wrapper for a matrix row to a vector.
    /// </summary>
    public class MatrixRowROVector : IROVector
    {
      IROMatrix _m;
      int     _row;
      int     _coloffset;
      int     _length;

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      public MatrixRowROVector(IROMatrix m, int row)
        : this(m,row,0,m.Columns)
      {
      }
      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      /// <param name="columnoffset">First number of column that is included in the vector.</param>
      /// <param name="length">Length of the vector.</param>
      public MatrixRowROVector(IROMatrix m, int row, int columnoffset, int length)
      {
        if(m==null)
          throw new ArgumentNullException("IROMatrix m is null");
        if(row<0 || row>=m.Rows)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");
        if(columnoffset+length>m.Columns)
          throw new ArgumentException("Columnoffset+length exceed the number of columns of the matrix");

        _m = m;
        _row = row;
        _coloffset = columnoffset;
        _length = length;

      }
     

      #region IROVector Members

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
      {
        get
        {
          return _m[_row,i+_coloffset];
        }
      }

      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound
      {
        get
        {
          return _length-1;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      }

      #endregion
    }
    /// <summary>
    /// Wrapper for a matrix row to a vector.
    /// </summary>
    public class MatrixRowVector : IVector
    {
      IMatrix _m;
      int     _row;
      int     _coloffset;
      int     _length;

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      public MatrixRowVector(IMatrix m, int row)
        : this(m,row,0,m.Columns)
      {
      }
      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
      /// <param name="columnoffset">First number of column that is included in the vector.</param>
      /// <param name="length">Length of the vector.</param>
      public MatrixRowVector(IMatrix m, int row, int columnoffset, int length)
      {
        if(m==null)
          throw new ArgumentNullException("IMatrix m is null");
        if(row<0 || row>=m.Rows)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");
        if(columnoffset+length>m.Columns)
          throw new ArgumentException("Columnoffset+length exceed the number of columns of the matrix");

        _m = m;
        _row = row;
        _coloffset = columnoffset;
        _length = length;

      }
      #region IVector Members

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _m[_row,i+_coloffset];
        }
        set
        {
          _m[_row,i+_coloffset] = value;
        }
      }

      #endregion

      #region IROVector Members

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
      {
        get
        {
          return _m[_row,i+_coloffset];
        }
      }

      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound
      {
        get
        {
          return _length-1;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      }

      #endregion
    }
    #endregion

    #region MatrixColumnROVector
    /// <summary>
    /// Wrapper for a matrix column to a read-only vector.
    /// </summary>
    public class MatrixColumnROVector : IROVector
    {
      IROMatrix _m;
      int     _col;

      /// <summary>
      /// Constructor of a matrix column vector by providing the matrix and the column number of that matrix that is wrapped.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <param name="col">The column number of the matrix that is wrapped to a vector.</param>
      public MatrixColumnROVector(IROMatrix m, int col)
      {
        if(m==null)
          throw new ArgumentNullException("IMatrix m is null");
        if(col<0 || col>=m.Columns)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");

        _m = m;
        _col = col;

      }

      #region IROVector Members

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
      {
        get
        {
          return _m[i,_col];
        }
      }

      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound
      {
        get
        {
          return _m.Rows-1;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _m.Rows;
        }
      }

      #endregion
    }
    #endregion

    #region MatrixColumnVector
    /// <summary>
    /// Wrapper for a matrix row to a vector.
    /// </summary>
    public class MatrixColumnVector : IVector
    {
      IMatrix _m;
      int     _col;

      /// <summary>
      /// Constructor of a matrix row vector by providing the matrix and the row number of that matrix that is wrapped.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <param name="col">The column number of the matrix that is wrapped to a vector.</param>
      public MatrixColumnVector(IMatrix m, int col)
      {
        if(m==null)
          throw new ArgumentNullException("IMatrix m is null");
        if(col<0 || col>=m.Columns)
          throw new ArgumentOutOfRangeException("The parameter row is either <0 or greater than the rows of the matrix");

        _m = m;
        _col = col;

      }
      #region IVector Members

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _m[i,_col];
        }
        set
        {
          _m[i,_col] = value;
        }
      }

      #endregion

      #region IROVector Members

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
      {
        get
        {
          return _m[i,_col];
        }
      }

      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound
      {
        get
        {
          return 0;
        }
      }

      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound
      {
        get
        {
          return _m.Rows-1;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _m.Rows;
        }
      }

      #endregion
    }
    #endregion

    #region SubMatrixWrapper

    /// <summary>
    /// Wraps part of a matrix so that it can be used as submatrix in operations.
    /// </summary>
    class SubMatrixROWrapper : IROMatrix
    {
      IROMatrix _m;
      int _rowoffset, _coloffset;
      int _rows, _cols;

      #region IROMatrix Members

      public SubMatrixROWrapper(IROMatrix matrix, int rowoffset, int coloffset, int rows, int cols)
      {
        _m = matrix;
        _rowoffset = rowoffset;
        _coloffset = coloffset;
        _rows = rows;
        _cols = cols;
      }

      public double this[int row, int col]
      {
        get
        {
          return _m[row+_rowoffset,col+_coloffset];
        }
      }

      public int Rows
      {
        get
        {
          return _rows;
        }
      }

      public int Columns
      {
        get
        {
          return _cols;
        }
      }

      #endregion

    }

    /// <summary>
    /// Wraps part of a matrix so that it can be used as submatrix in operations.
    /// </summary>
    class SubMatrixWrapper : IMatrix
    {
      IMatrix _m;
      int _rowoffset, _coloffset;
      int _rows, _cols;

      #region IROMatrix Members

      public SubMatrixWrapper(IMatrix matrix, int rowoffset, int coloffset, int rows, int cols)
      {
        _m = matrix;
        _rowoffset = rowoffset;
        _coloffset = coloffset;
        _rows = rows;
        _cols = cols;
      }

      public double this[int row, int col]
      {
        get
        {
          return _m[row+_rowoffset,col+_coloffset];
        }
      }

      public int Rows
      {
        get
        {
          return _rows;
        }
      }

      public int Columns
      {
        get
        {
          return _cols;
        }
      }

      #endregion

      #region IMatrix Members

      double Altaxo.Calc.LinearAlgebra.IMatrix.this[int row, int col]
      {
        get
        {
          return _m[row+_rowoffset,col+_coloffset];
        }
        set
        {
          _m[row+_rowoffset,col+_coloffset] = value;
        }
      }

      #endregion
    }

    #endregion

    #region DiagonalMatrix

    /// <summary>
    /// Wraps a vector to a diagonal matrix
    /// </summary>
    class RODiagonalMatrixVectorWrapper : IROMatrix
    {
      IROVector _m;
      int _offset, _dim;

      public RODiagonalMatrixVectorWrapper(IROVector vector, int offset, int matrixdimensions)
      {
        _m = vector;
        _offset = offset;
        _dim = matrixdimensions;
      }

      #region IROMatrix Members

      public double this[int row, int col]
      {
        get
        {
          if(row==col)
            return _m[row+_offset];
          else
            return 0;
        }
      }

      public int Rows
      {
        get
        {
          return _dim;
        }
      }

      public int Columns
      {
        get
        {
          return _dim;
        }
      }

      #endregion
    }

    #endregion

    #region Wrapper from linear array (LAPACK convention)

    /// <summary>
    /// Wraps a linear array to a read-only matrix. The array is column oriented, i.e. consecutive elements
    /// belong mostly to one column. This is the convention used for LAPACK routines.
    /// </summary>
    public class ROMatrixFromLinearArray : IROMatrix
    {
      protected double[] _data;
      protected int _rows;
      protected int _cols;
      #region IROMatrix Members

      public ROMatrixFromLinearArray(double[] array, int nRows)
      {
        if (array.Length % nRows != 0)
          throw new ArgumentException(string.Format("Length of array {0} is not a multiple of nRows={1}", array.Length, nRows));

        _data = array;
        _rows = nRows;
        _cols = array.Length / nRows;
      }

      public double this[int row, int col]
      {
        get
        {
          return _data[row + col * _rows];
        }
      }

      public int Rows
      {
        get { return _rows; }
      }

      public int Columns
      {
        get { return _cols; }
      }

      #endregion
    }

    /// <summary>
    /// Wraps a linear array to a read-write matrix. The array is column oriented, i.e. consecutive elements
    /// belong mostly to one column. This is the convention used for LAPACK routines.
    /// </summary>
    public class MatrixFromLinearArray : ROMatrixFromLinearArray, IMatrix
    {
      public MatrixFromLinearArray(double[] array, int nRows)
        : base(array,nRows)
      {
      }
      #region IMatrix Members

      public new double this[int row, int col]
      {
       
        get
        {
          return _data[row + col * _rows];
        }
        
        set
        {
          _data[row + col * _rows] = value;
        }
      }

      #endregion

    }


    #endregion

    #endregion

    #region Helper functions
    /// <summary>
    /// Calculates the Square of the value x.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>x*x.</returns>
    public static double Square(double x)
    {
      return x*x;
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
      for(int i=0;i<n;i++)
        result[i] = new double[m];

      return result;
    }

    public static string MatrixToString(string name, IROMatrix a)
    {
      if(null==name)
        name="";

      if(a.Rows==0 || a.Columns==0)
        return string.Format("EmptyMatrix {0}({1},{2})",name, a.Rows, a.Columns);

      System.Text.StringBuilder s = new System.Text.StringBuilder();
      s.Append("Matrix " + name + ":");
      for(int i=0;i<a.Rows;i++)
      {
        s.Append("\n(");
        for(int j=0;j<a.Columns;j++)
        {
          s.Append(a[i,j].ToString());
          if(j+1<a.Columns) 
            s.Append(";");
          else
            s.Append(")");
        }
      }
      return s.ToString();
    }
    public static string MatrixToString(string name, IROFloatMatrix a)
    {
      if (null == name)
        name = "";

      if (a.Rows == 0 || a.Columns == 0)
        return string.Format("EmptyMatrix {0}({1},{2})", name, a.Rows, a.Columns);

      System.Text.StringBuilder s = new System.Text.StringBuilder();
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
    public static string MatrixToString(string name, IROComplexDoubleMatrix a)
    {
      if (null == name)
        name = "";

      if (a.Rows == 0 || a.Columns == 0)
        return string.Format("EmptyMatrix {0}({1},{2})", name, a.Rows, a.Columns);

      System.Text.StringBuilder s = new System.Text.StringBuilder();
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

      System.Text.StringBuilder s = new System.Text.StringBuilder();
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

    #endregion

    #region Type conversion

    

    /// <summary>
    /// This wraps a jagged double array to the <see cref="IMatrix" /> interface. The data is not copied!
    /// </summary>
    /// <param name="x">The jagged array. Each double[] vector is a row of the matrix.</param>
    /// <returns></returns>
    public static IMatrix ToMatrix(double[][] x)
    {
      return new BEMatrix(x);
    }

    /// <summary>
    /// This wraps a jagged double array to the <see cref="IROMatrix" /> interface. The data is not copied!
    /// </summary>
    /// <param name="x">The jagged array. Each double[] vector is a row of the matrix.</param>
    /// <returns></returns>
    public static IMatrix ToROMatrix(double[][] x)
    {
      return new BEMatrix(x);
    }

    /// <summary>
    /// Constructs an RE matrix from an array of double vectors. Attention! The double vectors (the second) dimensions are here
    /// the columns (!) of the matrix. The data is not copied.
    /// </summary>
    /// <param name="x">Array of columns (!) of the matrix.</param>
    public static IROMatrix ToROMatrixFromJaggedColumns(double[][] x)
    {
      return new REMatrix(x);
    }

    /// <summary>
    /// Wraps a linear array into a read-only matrix. The array is packed column-wise, i.e. the first elements belong to the first column of the matrix.
    /// </summary>
    /// <param name="x">Linear array. The length has to be a multiple of <c>nRows</c>.</param>
    /// <param name="nRows">Number of rows of the resulting matrix.</param>
    /// <returns>The read-only matrix wrappage of the linear array.</returns>
    public static IROMatrix ToROMatrix(double[] x, int nRows)
    {
      return new ROMatrixFromLinearArray(x, nRows);
    }

    /// <summary>
    /// Wraps a linear array into a read-write matrix. The array is packed column-wise, i.e. the first elements belong to the first column of the matrix.
    /// </summary>
    /// <param name="x">Linear array. The length has to be a multiple of <c>nRows</c>.</param>
    /// <param name="nRows">Number of rows of the resulting matrix.</param>
    /// <returns>The read-only matrix wrappage of the linear array.</returns>
    public static IMatrix ToMatrix(double[] x, int nRows)
    {
      return new MatrixFromLinearArray(x, nRows);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    public static IROVector RowToROVector(IROMatrix x, int row)
    {
      return new MatrixRowROVector(x,row);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    /// <param name="columnoffset">The column of the matrix that corresponds to the first element of the vector.</param>
    /// <param name="length">The length of the resulting vector.</param>
    public static IROVector RowToROVector(IROMatrix x, int row, int columnoffset, int length)
    {
      return new MatrixRowROVector(x,row,columnoffset,length);
    }


    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    public static IVector RowToVector(IMatrix x, int row)
    {
      return new MatrixRowVector(x,row);
    }

    /// <summary>
    /// Returns a vector representing a matrix row by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="row">The row number of the matrix that is wrapped to a vector.</param>
    /// <param name="columnoffset">The column of the matrix that corresponds to the first element of the vector.</param>
    /// <param name="length">The length of the resulting vector.</param>
    public static IVector RowToVector(IMatrix x, int row, int columnoffset, int length)
    {
      return new MatrixRowVector(x,row,columnoffset,length);
    }


    /// <summary>
    /// Returns a read-only vector representing a matrix column by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="column">The column number of the matrix that is wrapped to a vector.</param>
    public static IROVector ColumnToROVector(IROMatrix x, int column)
    {
      return new MatrixColumnROVector(x,column);
    }

    /// <summary>
    /// Returns a vector representing a matrix column by providing the matrix and the row number of that matrix that is wrapped.
    /// </summary>
    /// <param name="x">The matrix.</param>
    /// <param name="column">The column number of the matrix that is wrapped to a vector.</param>
    public static IVector ColumnToVector(IMatrix x, int column)
    {
      return new MatrixColumnVector(x,column);
    }


    /// <summary>
    /// Wraps a submatrix part, so that this part can be used as a matrix in operations (read-only).
    /// </summary>
    /// <param name="matrix">The matrix from which a submatrix part should be wrapped.</param>
    /// <param name="rowoffset">Starting row of the submatrix.</param>
    /// <param name="columnoffset">Starting column of the submatrix.</param>
    /// <param name="rows">Number of rows of the submatrix.</param>
    /// <param name="columns">Number of columns of the submatrix.</param>
    /// <returns>A read-only wrapper matrix that represents the submatrix part of the matrix.</returns>
    public static IROMatrix ToROSubMatrix(IROMatrix matrix, int rowoffset, int columnoffset, int rows, int columns)
    {
      return new SubMatrixROWrapper(matrix,rowoffset,columnoffset,rows,columns);
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
    public static IMatrix ToSubMatrix(IMatrix matrix, int rowoffset, int columnoffset, int rows, int columns)
    {
      return new SubMatrixWrapper(matrix,rowoffset,columnoffset,rows,columns);
    }

    /// <summary>
    /// Wraps a read-only vector to a read-only diagonal matrix.
    /// </summary>
    /// <param name="vector">The vector to wrap.</param>
    /// <param name="vectoroffset">The index of the vector that is the first matrix element(0,0).</param>
    /// <param name="matrixdimensions">The number of rows = number of columns of the diagonal matrix.</param>
    /// <returns></returns>
    public static IROMatrix ToRODiagonalMatrix(IROVector vector, int vectoroffset, int matrixdimensions)
    {
      return new RODiagonalMatrixVectorWrapper(vector,vectoroffset,matrixdimensions);
    }

    #endregion

    #region Addition, Subtraction, Multiply and combined operations


    /// <summary>
    /// Multiplies matrix a with matrix b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void Multiply(IROMatrix a, IROMatrix b, IMatrix c)
    {
      int crows = a.Rows; // the rows of resultant matrix
      int ccols = b.Columns; // the cols of resultant matrix
      int numil = b.Rows; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));
      if(c.Rows != crows || c.Columns != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",c.Rows,c.Columns,crows,ccols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[i,k]*b[k,j];
        
          c[i,j] = sum;
        }
      }
    }

    /// <summary>
    /// Multiplies matrix a_transposed with matrix b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void MultiplyFirstTransposed(IROMatrix a, IROMatrix b, IMatrix c)
    {
      int crows = a.Columns; // the rows of resultant matrix
      int ccols = b.Columns; // the cols of resultant matrix
      int numil = b.Rows; // number of summands for most inner loop

      // Presumtion:
      if(a.Rows!=numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a transposed matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));
      if(c.Rows != crows || c.Columns != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})",c.Rows,c.Columns,crows,ccols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[k,i]*b[k,j];
        
          c[i,j] = sum;
        }
      }
    }


    /// <summary>
    /// Multiplies matrix a with matrix b_transposed and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void MultiplySecondTransposed(IROMatrix a, IROMatrix b, IMatrix c)
    {
      int crows = a.Rows; // the rows of resultant matrix
      int ccols = b.Rows; // the cols of resultant matrix
      int numil = b.Columns; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));
      if(c.Rows != crows || c.Columns != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})",c.Rows,c.Columns,crows,ccols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[i,k]*b[j,k];
        
          c[i,j] = sum;
        }
      }
    }


    /// <summary>
    /// Multiplies the matrix a with a scalar value b and stores the result in c. Matrix a and c are allowed to be the same matrix.
    /// </summary>
    /// <param name="a">The first multiplicant.</param>
    /// <param name="b">The second multiplicant.</param>
    /// <param name="c">The resulting matrix.</param>
    public static void MultiplyScalar(IROMatrix a, double b, IMatrix c)
    {
      if(c.Rows != a.Rows || c.Columns != a.Columns)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1})) has not the expected dimension ({2},{3})",c.Rows,c.Columns,a.Rows,a.Columns));

      for(int i=0;i<a.Rows;i++)
      {
        for(int j=0;j<a.Columns;j++)
        {
          c[i,j] = a[i,j]*b;
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
    public static void MultiplyRow(IROMatrix a, IROMatrix b, int brow,  IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int brows = b.Rows;
      int bcols = b.Columns;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!",brows,bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]*b[brow,j];
    }


    /// <summary>
    /// Multiplies the row <c>rowb</c> of matrix b element by element to all rows of matrix a. 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The vector which contains the row to multiply.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void MultiplyRow(IROMatrix a, IROVector b, IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int brows = 1;
      int bcols = b.Length;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Vector b[{0}] has not the same number of columns than matrix a[{1},{2}]!",brows,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Vector b and matrix c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]*b[j];
    }

    /// <summary>
    /// Calculates a+b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First matrix to add..</param>
    /// <param name="b">Second operand..</param>
    /// <param name="c">The resultant matrix a+b. Has to be of same dimension as a and b.</param>
    public static void Add(IROMatrix a, IROMatrix b, IMatrix c)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=b.Columns || a.Rows!=b.Rows)
        throw new ArithmeticException(string.Format("Try to add a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));
      if(c.Rows != a.Rows || c.Columns != a.Columns)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the proper dimension ({2},{3})",c.Rows,c.Columns,a.Rows,a.Columns));

      for(int i=0;i<c.Rows;i++)
        for(int j=0;j<c.Columns;j++)
          c[i,j] = a[i,j]+b[i,j];
    }


    /// <summary>
    /// Calculates a-b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">Minuend.</param>
    /// <param name="b">Subtrahend.</param>
    /// <param name="c">The resultant matrix a-b. Has to be of same dimension as a and b.</param>
    public static void Subtract(IROMatrix a, IROMatrix b, IMatrix c)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=b.Columns || a.Rows!=b.Rows)
        throw new ArithmeticException(string.Format("Try to subtract a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));
      if(c.Rows != a.Rows || c.Columns != a.Columns)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the proper dimension ({2},{3})",c.Rows,c.Columns,a.Rows,a.Columns));

      for(int i=0;i<c.Rows;i++)
        for(int j=0;j<c.Columns;j++)
          c[i,j] = a[i,j]-b[i,j];
    }

    /// <summary>
    /// Calculates c = c - ab
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to subtract the result of the multipication from. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void SubtractProductFromSelf(IROMatrix a, IROMatrix b, IMatrix c)
    {
      int crows = a.Rows; // the rows of resultant matrix
      int ccols = b.Columns; // the cols of resultant matrix
      int numil = b.Rows; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));
      if(c.Rows != crows || c.Columns != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",c.Rows,c.Columns,crows,ccols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[i,k]*b[k,j];
        
          c[i,j] -= sum;
        }
      }
    }
   
    /// <summary>
    /// Calculates c = c - ab
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="c">The matrix where to subtract the result of the multipication from. Has to be of dimension (a.Rows, b.Columns).</param>
    public static void SubtractProductFromSelf(IROMatrix a, double b, IMatrix c)
    {
      int crows = a.Rows; // the rows of resultant matrix
      int ccols = a.Columns; // the cols of resultant matrix

      // Presumtion:
      if(c.Rows != crows || c.Columns != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",c.Rows,c.Columns,crows,ccols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          c[i,j] -= b*a[i,j];
        }
      }
    }

    /// <summary>
    /// Add the vector <c>b</c>  to all rows of matrix a. 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The vector to add.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to vector b).</param>
    public static void AddRow(IROMatrix a, IROVector b, IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int bcols = b.Length;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Vector b[{0}] has not the same length than rows of matrix a[{1},{2}]!",bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Vector b and Matrix c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]+b[j];
    }

    /// <summary>
    /// Add the row <c>rowb</c> of matrix b to all rows of matrix a. 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to add.</param>
    /// <param name="brow">The row number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void AddRow(IROMatrix a, IROMatrix b, int brow,  IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int brows = b.Rows;
      int bcols = b.Columns;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!",brows,bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]+b[brow,j];
    }

    /// <summary>
    /// Subtracts the row <c>rowb</c> of matrix b from all rows of matrix a. 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to subtract.</param>
    /// <param name="brow">The row number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void SubtractRow(IROMatrix a, IROMatrix b, int brow,  IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int brows = b.Rows;
      int bcols = b.Columns;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!",brows,bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]-b[brow,j];
    }

    /// <summary>
    /// Subtracts the row <c>rowb</c> of matrix b from all rows of matrix a. 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The vector which contains the row to subtract.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void SubtractRow(IROMatrix a, IROVector b, IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int bcols = b.Length;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Vector b[{0}] has not the same length than rows of matrix a[{1},{2}]!",bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Vector b and Matrix c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]-b[j];
    }
    
    /// <summary>
    /// Subtracts the column <c>bcol</c> of matrix b from all columns of matrix a. 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the row to subtract.</param>
    /// <param name="bcol">The column number of matrix b.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void SubtractColumn(IROMatrix a, IROMatrix b, int bcol,  IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int brows = b.Rows;
      int bcols = b.Columns;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(brows != arows)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of rows than matrix a[{2},{3}]!",brows,bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i,j] = a[i,j]-b[i,bcol];
    }

    /// <summary>
    /// Divides all rows of matrix a by the row <c>rowb</c> of matrix b (element by element). 
    /// </summary>
    /// <param name="a">The source matrix.</param>
    /// <param name="b">The matrix which contains the denominator row.</param>
    /// <param name="brow">The row number of matrix b which serves as denominator.</param>
    /// <param name="resultIfNull">If the denominator is null, the result is set to this number.</param>
    /// <param name="c">The destination matrix. Can be equivalent to matrix a (but not to matrix b).</param>
    public static void DivideRow(IROMatrix a, IROMatrix b, int brow, double resultIfNull, IMatrix c)
    {
      int arows = a.Rows;
      int acols = a.Columns;

      int brows = b.Rows;
      int bcols = b.Columns;

      int crows = c.Rows;
      int ccols = c.Columns;

      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!",brows,bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
        {
          double denom = b[brow,j];
          c[i,j] = denom==0 ? resultIfNull : a[i,j]/denom;
        }
    }

    #endregion

    /*
    /// <summary>
    /// This will center the matrix so that the mean of each column is null.
    /// </summary>
    /// <param name="a">The matrix where the columns should be centered.</param>
    /// <param name="mean">You can provide a matrix of dimension(1,a.Cols) where the mean row vector is stored, or null if not interested in this vector.</param>
    /// <remarks>Calling this function will change the matrix a to a column
    /// centered matrix. The original matrix data are lost.</remarks>
    public static void ColumnsToZeroMean(IMatrix a, IMatrix mean)
    {
      if(null!=mean && (mean.Rows != 1 || mean.Columns != a.Columns))
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",mean.Rows,mean.Columns,1,a.Columns));

      for(int col = 0; col<a.Columns; col++)
      {
        double sum = 0;
        for(int row=0;row<a.Rows;row++)
          sum += a[row,col];
        sum /= a.Rows; // calculate the mean
        for(int row=0;row<a.Rows;row++)
          a[row,col] -= sum; // subtract the mean from every element in the column
        
        if(null!=mean)
          mean[0,col] = sum;
      }
    }
*/
    /// <summary>
    /// This will center the matrix so that the mean of each column is null.
    /// </summary>
    /// <param name="a">The matrix where the columns should be centered.</param>
    /// <param name="mean">You can provide a matrix of dimension(1,a.Cols) where the mean row vector is stored, or null if not interested in this vector.</param>
    /// <remarks>Calling this function will change the matrix a to a column
    /// centered matrix. The original matrix data are lost.</remarks>
    public static void ColumnsToZeroMean(IMatrix a, IVector mean)
    {
      if(null!=mean && mean.Length != a.Columns)
        throw new ArithmeticException(string.Format("The provided resultant vector (actual length({0}) has not the expected dimension ({1})",mean.Length,a.Columns));

      for(int col = 0; col<a.Columns; col++)
      {
        double sum = 0;
        for(int row=0;row<a.Rows;row++)
          sum += a[row,col];
        sum /= a.Rows; // calculate the mean
        for(int row=0;row<a.Rows;row++)
          a[row,col] -= sum; // subtract the mean from every element in the column
        
        if(null!=mean)
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
    public static void ColumnsToZeroMeanAndUnitVariance(IMatrix a, IVector meanvec, IVector scalevec)
    {
      if(null!=meanvec && (meanvec.Length != a.Columns))
        throw new ArithmeticException(string.Format("The provided resultant mean vector (actual dim({0})has not the expected length ({1})",meanvec.Length,a.Columns));
      if(null!=scalevec && (scalevec.Length != a.Columns))
        throw new ArithmeticException(string.Format("The provided resultant scale vector (actual dim({0})has not the expected length ({1})",scalevec.Length,a.Columns));

      for(int col = 0; col<a.Columns; col++)
      {
        double sum = 0;
        double sumsqr = 0;
        for(int row=0;row<a.Rows;row++)
        {
          sum += a[row,col];
          sumsqr += Square(a[row,col]);
        }
        double mean = sum/a.Rows; // calculate the mean
        double scor;
        if(a.Rows>1 && sumsqr-mean*sum>0)
          scor  = Math.Sqrt((a.Rows-1)/(sumsqr-mean*sum));
        else
          scor = 1;
        for(int row=0;row<a.Rows;row++)
          a[row,col] = (a[row,col]-mean)*scor; // subtract the mean from every element in the column
        
        if(null!=meanvec)
          meanvec[col] = mean;
        if(null!=scalevec)
          scalevec[col] = scor;
      }
    }


    /// <summary>
    /// Returns the sum of the squares of all elements.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>The sum of the squares of all elements in the matrix a.</returns>
    public static double SumOfSquares(IROMatrix a)
    {
      double sum=0;
      for(int i=0;i<a.Rows;i++)
        for(int j=0;j<a.Columns;j++)
          sum += Square(a[i,j]);
      return sum;
    }

    /// <summary>
    /// Returns the sum of the squares of differences of elements of a and b.
    /// </summary>
    /// <param name="a">The first matrix.</param>
    /// <param name="b">The second matrix. Must have same dimensions than a.</param>
    /// <returns>The sum of the squared differences of each element in a to the corresponding element in b, i.e. Sum[(a[i,j]-b[i,j])²].</returns>
    public static double SumOfSquaredDifferences(IROMatrix a, IROMatrix b)
    {
      if(a.Rows != b.Rows || a.Columns != a.Columns)
        throw new ArithmeticException(string.Format("The two provided matrices (a({0},{1])) and b({2},{3})) have not the same dimensions.",a.Rows,a.Columns,b.Rows,b.Columns));

      double sum=0;
      for(int i=0;i<a.Rows;i++)
        for(int j=0;j<a.Columns;j++)
          sum += Square(a[i,j]-b[i,j]);
      return sum;
    }
    

    /// <summary>
    /// Returns the square root of the sum of the squares of the matrix a.
    /// </summary>
    /// <param name="a">The matrix.</param>
    /// <returns>The square root of the sum of the squares of the matrix a.</returns>
    public static double LengthOf(IROMatrix a)
    {
      return Math.Sqrt(SumOfSquares(a));
    }

    /// <summary>
    /// Tests if all elements of the matrix a are equal to zero.
    /// </summary>
    /// <param name="a">The matrix to test.</param>
    /// <returns>True if all elements are zero or if one of the two dimensions of the matrix is zero. False if the matrix contains nonzero elements.</returns>
    public static bool IsZeroMatrix(IROMatrix a)
    {
      if(a.Rows==0 || a.Columns==0)
        return true; // we consider a matrix with one dimension zero also as zero matrix

      for(int i=0;i<a.Rows;i++)
        for(int j=0;j<a.Columns;j++)
          if(a[i,j]!=0)
            return false;

      return true;
    }


    /// <summary>
    /// Set all matrix elements to the provided value <paramref name="scalar"/>.
    /// </summary>
    /// <param name="a">The matrix where to set the elements.</param>
    /// <param name="scalar">The value which is used to set each element with.</param>
    public static void SetMatrixElements(IMatrix a, double scalar)
    {
      for(int i=0;i<a.Rows;i++)
        for(int j=0;j<a.Columns;j++)
          a[i,j]=scalar;
    }


    /// <summary>
    /// Set all elements in the matrix to 0 (zero)
    /// </summary>
    /// <param name="a">The matrix to zero.</param>
    public static void ZeroMatrix(IMatrix a)
    {
      SetMatrixElements(a,0);
    }


    /// <summary>
    /// Gets a submatrix out of the source matrix a. The dimensions of the submatrix are given by the provided matrix dest.
    /// </summary>
    /// <param name="src">The source matrix.</param>
    /// <param name="dest">The destination matrix where to store the submatrix. It's dimensions are the dimensions of the submatrix.</param>
    /// <param name="rowoffset">The row offset = vertical origin of the submatrix in the source matrix.</param>
    /// <param name="coloffset">The column offset = horizontal origin of the submatrix in the source matrix.</param>
    public static void  Submatrix(IROMatrix src, IMatrix dest, int rowoffset, int coloffset)
    {
      for(int i=0;i<dest.Rows;i++)
        for(int j=0;j<dest.Columns;j++)
          dest[i,j] = src[i+rowoffset,j+coloffset];
    }

    /// <summary>
    /// Gets a submatrix out of the source matrix a. The dimensions of the submatrix are given by the provided matrix dest.
    /// The origin of the submatrix in the source matrix is (0,0), i.e. the left upper corner.
    /// </summary>
    /// <param name="src">The source matrix.</param>
    /// <param name="dest">The destination matrix where to store the submatrix. It's dimensions are the dimensions of the submatrix.</param>
    public static void Submatrix(IROMatrix src, IMatrix dest)
    {
      for(int i=0;i<dest.Rows;i++)
        for(int j=0;j<dest.Columns;j++)
          dest[i,j] = src[i,j];
    }

    /// <summary>
    /// Copies matrix src to matrix dest. Both matrizes must have the same dimensions.
    /// </summary>
    /// <param name="src">The source matrix to copy.</param>
    /// <param name="dest">The destination matrix to copy to.</param>
    public static void Copy(IROMatrix src, IMatrix dest)
    {
      if(dest.Rows != src.Rows || dest.Columns != src.Columns)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the dimension of the source matrix ({2},{3})",dest.Rows,dest.Columns,src.Rows,src.Columns));

      int rows=src.Rows;
      int cols=src.Columns;
      for(int i=0;i<rows;i++)
        for(int j=0;j<cols;j++)
          dest[i,j] = src[i,j];

    }

    /// <summary>
    /// Copies the matrix src into the matrix dest. Matrix dest must have equal or greater dimension than src.
    /// You can provide a destination row/column into the destination matrix where the origin of the copy operation is located.
    /// </summary>
    /// <param name="src">The source matrix.</param>
    /// <param name="dest">The destination matrix. Must have equal or higher dim than the source matrix.</param>
    /// <param name="destrow">The vertical origin of copy operation in the destination matrix.</param>
    /// <param name="destcol">The horizontal origin of copy operation in the destination matrix.</param>
    public static void Copy(IROMatrix src, IMatrix dest, int destrow, int destcol)
    {
      int rows=src.Rows;
      int cols=src.Columns;
      for(int i=0;i<rows;i++)
        for(int j=0;j<cols;j++)
          dest[i+destrow,j+destcol] = src[i,j];

    }


    /// <summary>
    /// Sets one column in the destination matrix equal to the vertical vector provided by src matix.
    /// </summary>
    /// <param name="src">The source matrix. Must be a vertical vector (cols=1) with the same number of rows than the destination matrix.</param>
    /// <param name="srccol">The column in the source matrix to copy from.</param>
    /// <param name="dest">The destination matrix where to copy the vertical vector into.</param>
    /// <param name="destcol">The column in the destination matrix where to copy the vector to.</param>
    public static void SetColumn(IROMatrix src, int srccol, IMatrix dest, int destcol)
    {
      if(destcol>=dest.Columns)
        throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!",destcol,dest.Rows,dest.Columns));
      if(srccol>=src.Columns)
        throw new ArithmeticException(string.Format("Parameter srccol out of range ({0}>={1})!",srccol,src.Columns));
      if(dest.Rows != src.Rows)
        throw new ArithmeticException(string.Format("Try to set column {0}, but number of rows of the matrix ({1}) not match number of rows of the vector ({3})!",destcol,dest.Rows,src.Rows));
    
      for(int i=0;i<dest.Rows;i++)
        dest[i,destcol]=src[i,srccol];
    }

    /// <summary>
    /// Sets one column in the destination matrix equal to the vertical vector provided by src matix.
    /// </summary>
    /// <param name="src">The source matrix. Must be a vertical vector (cols=1) with the same number of rows than the destination matrix.</param>
    /// <param name="dest">The destination matrix where to copy the vertical vector into.</param>
    /// <param name="col">The column in the destination matrix where to copy the vector to.</param>
    public static void SetColumn(IROMatrix src, IMatrix dest, int col)
    {
      if(col>=dest.Columns)
        throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!",col,dest.Rows,dest.Columns));
      if(src.Columns!=1)
        throw new ArithmeticException(string.Format("Try to set column {0} with a matrix of more than one, namely {1} columns, is not allowed!",col,src.Columns));
      if(dest.Rows != src.Rows)
        throw new ArithmeticException(string.Format("Try to set column {0}, but number of rows of the matrix ({1}) not match number of rows of the vector ({3})!",col,dest.Rows,src.Rows));
    
      for(int i=0;i<dest.Rows;i++)
        dest[i,col]=src[i,0];
    }

    /// <summary>
    /// Sets one row in the destination matrix equal to the horizontal vector provided by src matix.
    /// </summary>
    /// <param name="src">The source matrix. Must be a horizontal vector (rows=1) with the same number of columns than the destination matrix.</param>
    /// <param name="srcRow">The row in the source matrix where to copy from.</param>
    /// <param name="dest">The destination matrix where to copy the horizontal vector into.</param>
    /// <param name="destRow">The row in the destination matrix where to copy the vector to.</param>
    public static void SetRow(IROMatrix src, int srcRow, IMatrix dest, int destRow)
    {
      if(destRow>=dest.Rows)
        throw new ArithmeticException(string.Format("Try to set row {0} in the matrix with dim({1},{2}) is not allowed!",destRow,dest.Rows,dest.Columns));
      if(srcRow>=src.Rows)
        throw new ArithmeticException(string.Format("The source row number ({0}) exceeds the actual number of rows ({1})in the source matrix!",srcRow,src.Rows));
      if(dest.Columns != src.Columns)
        throw new ArithmeticException(string.Format("Number of columns of the matrix ({0}) not match number of colums of the vector ({1})!",dest.Columns,src.Columns));
    
      for(int j=0;j<dest.Columns;j++)
        dest[destRow,j]=src[srcRow,j];
    }

    /// <summary>
    /// Sets one row in the destination matrix equal to the vector provided by src.
    /// </summary>
    /// <param name="src">The source vector. Must be of same length as the number of columns of the destination matrix.</param>
    /// <param name="dest">The destination matrix where to copy the horizontal vector into.</param>
    /// <param name="destRow">The row in the destination matrix where to copy the vector to.</param>
    public static void SetRow(IROVector src, IMatrix dest, int destRow)
    {
      if(destRow>=dest.Rows)
        throw new ArithmeticException(string.Format("Try to set row {0} in the matrix with dim({1},{2}) is not allowed!",destRow,dest.Rows,dest.Columns));
      if(dest.Columns != src.Length)
        throw new ArithmeticException(string.Format("Number of columns of the matrix ({0}) not match number of elements of the vector ({1})!",dest.Columns,src.Length));
    
      for(int j=0;j<dest.Columns;j++)
        dest[destRow,j]=src[j];
    }


    /// <summary>
    /// Normalizes each row (each horizontal vector) of the matrix. After
    /// normalization, each row has the norm 1, i.e. the sum of squares of the elements of each row is 1 (one).
    /// </summary>
    /// <param name="a">The matrix which should be row normalized.</param>
    public static void NormalizeRows(IMatrix a)
    {
      for(int i=0;i<a.Rows;i++)
      {
        double sum=0;
        for(int j=0;j<a.Columns;j++)
          sum += Square(a[i,j]);

        if(sum!=0) // Normalize only of at least one element is not null
        {
          sum = 1/Math.Sqrt(sum);
          for(int j=0;j<a.Columns;j++)
            a[i,j] *= sum;
        }
      }
    }


    /// <summary>
    /// Normalizes each column (each vertical vector) of the matrix. After
    /// normalization, each column has the norm 1, i.e. the sum of squares of the elements of each column is 1 (one).
    /// </summary>
    /// <param name="a">The matrix which should be column normalized.</param>
    public static void NormalizeCols(IMatrix a)
    {
      for(int i=0;i<a.Columns;i++)
      {
        double sum=0;
        for(int j=0;j<a.Rows;j++)
          sum += Square(a[j,i]);
        
        if(sum!=0)
        {
          sum = 1/Math.Sqrt(sum);
          for(int j=0;j<a.Rows;j++)
            a[j,i] *= sum;
        }
      }
    }

    /// <summary>
    /// Normalizes the column col of a matrix to unit length.
    /// </summary>
    /// <param name="a">The matrix for which the column col is normalized.</param>
    /// <param name="col">The number of the column which should be normalized.</param>
    /// <returns>Square root of the sum of squares of the column, i.e. the original length of the column vector before normalization.</returns>
    public static double NormalizeOneColumn(IMatrix a, int col)
    {
      if(col>=a.Columns)
        throw new ArithmeticException(string.Format("Matrix a is expected to have at least {0} columns, but has the actual dimensions({1},{2})",col+1,a.Rows,a.Columns));
  
      double sum=0;
      for(int i=0;i<a.Rows;i++)
        sum += Square(a[i,0]);
    
      sum = Math.Sqrt(sum);
      for(int i=0;i<a.Rows;i++)
        a[i,0] /= sum;

      return sum;
    }

    /// <summary>
    /// This inverts the provided diagonal matrix. There is no check that the matrix is really
    /// diagonal, but the algorithm sets the elements outside the diagonal to zero, assuming
    /// that this are small arithmetic errors.
    /// </summary>
    /// <param name="a">The matrix to invert. After calling the matrix is inverted, i.e.
    /// the diagonal elements are replaced by their inverses, and the outer diagonal elements are set to zero.</param>
    public static void InvertDiagonalMatrix(IMatrix a)
    {
      int rows = a.Rows;
      int cols = a.Columns;

      if(cols!=rows)
        throw new ArithmeticException(string.Format("A diagonal matrix has to be quadratic, but you provided a matrix of dimension({0},{1})!",rows,cols));
    
      for(int i=0;i<rows;i++)
        for(int j=0;j<cols;j++)
          a[i,j] = i==j ? 1/a[i,j] : 0;
    }

    /// <summary>
    /// Compares matrix a and matrix b. Takes the norm of matrix b times accuracy as
    /// threshold basis for comparing the elements.
    /// </summary>
    /// <param name="a">The first matrix.</param>
    /// <param name="b">The second matrix. Basis for calculation of threshold.</param>
    /// <param name="accuracy">The accuracy.</param>
    /// <returns></returns>
    public static bool IsEqual(IROMatrix a, IROMatrix b, double accuracy)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=b.Columns || a.Rows != b.Rows)
        throw new ArithmeticException(string.Format("Try to compare a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));

      double thresh = Math.Sqrt(SumOfSquares(b))*accuracy/((double)b.Rows*b.Columns);;
      for(int i=0;i<a.Rows;i++)
        for(int j=0;j<a.Columns;j++)
          if(Math.Abs(a[i,j]-b[i,j])>thresh)
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
      IMatrix X,
      int numFactors,
      double accuracy,
      IRightExtensibleMatrix factors,
      IBottomExtensibleMatrix loads,
      IBottomExtensibleMatrix residualVarianceVector)
    {
            
      // first center the matrix
      //MatrixMath.ColumnsToZeroMean(X, null);

      double originalVariance = Math.Sqrt(MatrixMath.SumOfSquares(X));
      
      if(null!=residualVarianceVector)
        residualVarianceVector.AppendBottom(new MatrixMath.Scalar(originalVariance));

  
      IMatrix l = new HorizontalVector(X.Columns);
      IMatrix t_prev = null;
      IMatrix t = new VerticalVector(X.Rows);

      int maxFactors = numFactors<=0 ? X.Columns : Math.Min(numFactors,X.Columns);

      for(int nFactor=0; nFactor<maxFactors; nFactor++)
      {
        //l has to be a horizontal vector
        // 1. Guess the transposed Vector l_transp, use first row of X matrix if it is not empty, otherwise the first non-empty row
        int rowoffset=0;
        do  
        {
          Submatrix(X,l,rowoffset,0);     // l is now a horizontal vector
          rowoffset++;
        } while(IsZeroMatrix(l) && rowoffset<X.Rows);



        for(int iter=0;iter<500;iter++)
        {
      
          // 2. Calculate the new vector t for the factor values
          MultiplySecondTransposed(X,l,t); // t = X*l_t (t is  a vertical vector)

          // Compare this with the previous one 
          if(t_prev!=null && IsEqual(t_prev,t,1E-9))
            break;

          // 3. Calculate the new loads 
          MultiplyFirstTransposed(t,X,l); // l = t_tr*X  (gives a horizontal vector of load (= eigenvalue spectrum)
          
          // normalize the (one) row
          NormalizeRows(l); // normalize the eigenvector spectrum

          // 4. Goto step 2 or break after a number of iterations
          if(t_prev==null)
            t_prev = new VerticalVector(X.Rows);
          Copy(t,t_prev); // stores the content of t in t_prev

        }

        // Store factor and loads
        factors.AppendRight(t);
        loads.AppendBottom(l);

        // 5. Calculate the residual matrix X = X - t*l 
        SubtractProductFromSelf(t,l,X); // X is now the residual matrix

        // if the number of factors to calculate is not provided,
        // calculate the norm of the residual matrix and compare with the original
        // one
        if(numFactors<=0 || null!=residualVarianceVector)
        {
          double residualVariance = Math.Sqrt(MatrixMath.SumOfSquares(X));
          residualVarianceVector.AppendBottom(new MatrixMath.Scalar(residualVariance));

          if(residualVariance<=accuracy*originalVariance)
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
    public static IMatrix PseudoInverse(IROMatrix input)
    {
      int rank;
      return PseudoInverse(input,out rank);
    }

    /// <summary>
    /// Calculates the pseudo inverse of the matrix <c>input</c> by means of singular value decomposition.
    /// A relative value of <c>100*DBL_EPSILON</c> is used to chop the singular values before calculation.
    /// </summary>
    /// <param name="input">Input matrix</param>
    /// <param name="rank">Returns the rank of the input matrix.</param>
    /// <returns>The pseudo inverse of matrix <c>input</c>.</returns>
    public static IMatrix PseudoInverse(IROMatrix input, out int rank)
    {
      MatrixMath.BEMatrix ma = new BEMatrix(input.Rows,input.Columns);
      MatrixMath.Copy(input,ma);
      SingularValueDecomposition svd =  new SingularValueDecomposition(ma);
      
      
      double[][] B = GetMatrixArray(input.Columns,input.Rows);


      /* compute the pseudoinverse in B */
      double[] s = svd.Diagonal;
      int m = input.Rows;
      int n = input.Columns;
      int minmn = Math.Min(m,n);

      double[][] v = svd.V;
      double[][] u = svd.U;
      double thresh = (DoubleConstants.DBL_EPSILON*100) * s[0];
      for(rank=0; rank<minmn && s[rank]>thresh; rank++)
      {
        double one_over_denom=1.0/s[rank];

        for(int j=0; j<m; j++)
          for(int i=0; i<n; i++)
            B[i][j]+=v[i][rank]*u[j][rank]*one_over_denom;
      }

      return new JaggedArrayMatrix(B);
    }

    /// <summary>Returns the singular value decomposition for this matrix.</summary>
    /// <param name="input">The input matrix (is preserved).</param>
    /// <param name="output">The resulting matrix. Has to be of same dimensions as the input matrix.</param>
    public static SingularValueDecomposition GetSingularValueDecomposition(IROMatrix input, IMatrix output)
    {
      MatrixMath.Copy(input,output);
      return new SingularValueDecomposition(output);
    }

    /// <summary>Returns the singular value decomposition for this matrix.</summary>
    /// <param name="inout">The input matrix, on return the resulting decomposed matrix.</param>
    public static SingularValueDecomposition GetSingularValueDecomposition(IMatrix inout)
    {
      return new SingularValueDecomposition(inout);
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
      double[][] u;
      double[][] v;
      //private Matrix V;
      private double[] s; // singular values
      double[] e;
      double[] work;
      double[] _HatDiagonal;
      private int m;
      private int n;
    

      /// <summary>
      /// Creates a singular value decomposition of matrix a, resulting in matrix a itself.
      /// </summary>
      /// <param name="a">Matrix to decompose, on return: decomposed matrix.</param>
      public SingularValueDecomposition(IMatrix a)
      {
        ComputeSingularValueDecomposition(a);
      }

      /// <summary>
      /// Creates a singular value decomposition of matrix a, resulting in matrix a itself.
      /// </summary>
      /// <param name="a">Matrix to decompose, on return: decomposed matrix.</param>
      public void ComputeSingularValueDecomposition(IMatrix a)
      {
        m = a.Rows;
        n = a.Columns;
        int nu = Math.Min(m,n);
        s = new double [Math.Min(m+1,n)];
        //U = new Matrix(m, nu);
        //V = new Matrix(n, n);
        u = GetMatrixArray(m,nu); // U.Array
        v = GetMatrixArray(n,n); // V.Array;
        e = new double [n];
        work = new double [m];
        bool wantu = true;
        bool wantv = true;
    
        // Reduce A to bidiagonal form, storing the diagonal elements in s and the super-diagonal elements in e.
        int nct = Math.Min(m-1,n);
        int nrt = Math.Max(0,Math.Min(n-2,m));
        for (int k = 0; k < Math.Max(nct,nrt); k++) 
        {
          if (k < nct) 
          {
            // Compute the transformation for the k-th column and place the k-th diagonal in s[k].
            // Compute 2-norm of k-th column without under/overflow.
            s[k] = 0;
            for (int i = k; i < m; i++)
              s[k] = Hypotenuse(s[k],a[i,k]);
    
            if (s[k] != 0.0) 
            {
              if (a[k,k] < 0.0)
                s[k] = -s[k];
    
              for (int i = k; i < m; i++)
                a[i,k] /= s[k];
    
              a[k,k] += 1.0;
            }
            s[k] = -s[k];
          }
            
          for (int j = k+1; j < n; j++) 
          {
            if ((k < nct) & (s[k] != 0.0))  
            {
              // Apply the transformation.
              double t = 0;
              for (int i = k; i < m; i++)
                t += a[i,k]*a[i,j];
              t = -t/a[k,k];
              for (int i = k; i < m; i++)
                a[i,j] += t*a[i,k];
            }
    
            // Place the k-th row of A into e for the subsequent calculation of the row transformation.
            e[j] = a[k,j];
          }
            
          if (wantu & (k < nct)) 
          {
            // Place the transformation in U for subsequent back
            // multiplication.
            for (int i = k; i < m; i++)
              u[i][k] = a[i,k];
          }
    
          if (k < nrt) 
          {
            // Compute the k-th row transformation and place the k-th super-diagonal in e[k].
            // Compute 2-norm without under/overflow.
            e[k] = 0;
            for (int i = k+1; i < n; i++)
              e[k] = Hypotenuse(e[k],e[i]);
    
            if (e[k] != 0.0) 
            {
              if (e[k+1] < 0.0)
                e[k] = -e[k];
    
              for (int i = k+1; i < n; i++)
                e[i] /= e[k];
              
              e[k+1] += 1.0;
            }
              
            e[k] = -e[k];
            if ((k+1 < m) & (e[k] != 0.0)) 
            {
              // Apply the transformation.
              for (int i = k+1; i < m; i++)
                work[i] = 0.0;
    
              for (int j = k+1; j < n; j++)
                for (int i = k+1; i < m; i++)
                  work[i] += e[j]*a[i,j];
    
              for (int j = k+1; j < n; j++) 
              {
                double t = -e[j]/e[k+1];
                for (int i = k+1; i < m; i++)
                  a[i,j] += t*work[i];
              }
            }
                
            if (wantv) 
            {
              // Place the transformation in V for subsequent back multiplication.
              for (int i = k+1; i < n; i++)
                v[i][k] = e[i];
            }
          }
        }
    
        // Set up the final bidiagonal matrix or order p.
        int p = Math.Min(n,m+1);
        if (nct < n) s[nct] = a[nct,nct];
        if (m < p) s[p-1] = 0.0;
        if (nrt+1 < p) e[nrt] = a[nrt,p-1];
        e[p-1] = 0.0;
    
        // If required, generate U.
        if (wantu) 
        {
          for (int j = nct; j < nu; j++) 
          {
            for (int i = 0; i < m; i++) 
              u[i][j] = 0.0;
            u[j][j] = 1.0;
          }
            
          for (int k = nct-1; k >= 0; k--) 
          {
            if (s[k] != 0.0) 
            {
              for (int j = k+1; j < nu; j++) 
              {
                double t = 0;
                for (int i = k; i < m; i++) 
                  t += u[i][k]*u[i][j];
    
                t = -t/u[k][k];
                for (int i = k; i < m; i++)
                  u[i][j] += t*u[i][k];
              }
                  
              for (int i = k; i < m; i++ )
                u[i][k] = -u[i][k];
    
              u[k][k] = 1.0 + u[k][k];
              for (int i = 0; i < k-1; i++) 
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
          for (int k = n-1; k >= 0; k--) 
          {
            if ((k < nrt) & (e[k] != 0.0)) 
            {
              for (int j = k+1; j < n; j++) // Lellinger (2004/03/28): end variable changed from nu to n
              {
                double t = 0;
                for (int i = k+1; i < n; i++) 
                  t += v[i][k]*v[i][j];
    
                t = -t/v[k+1][k];
                for (int i = k+1; i < n; i++)
                  v[i][j] += t*v[i][k];
              }
            }
                
            for (int i = 0; i < n; i++) 
              v[i][k] = 0.0;
            v[k][k] = 1.0;
          }
        }
    
        // Main iteration loop for the singular values.
        int pp = p-1;
        int iter = 0;
        double eps = Math.Pow(2.0,-52.0);
        while (p > 0) 
        {
          int k,kase;
    
          // Here is where a test for too many iterations would go.
          // This section of the program inspects for
          // negligible elements in the s and e arrays.  On
          // completion the variables kase and k are set as follows.
          // kase = 1     if s(p) and e[k-1] are negligible and k<p
          // kase = 2     if s(k) is negligible and k<p
          // kase = 3     if e[k-1] is negligible, k<p, and s(k), ..., s(p) are not negligible (qr step).
          // kase = 4     if e(p-1) is negligible (convergence).
          for (k = p-2; k >= -1; k--) 
          {
            if (k == -1)
              break;
    
            if (Math.Abs(e[k]) <= eps*(Math.Abs(s[k]) + Math.Abs(s[k+1]))) 
            {
              e[k] = 0.0;
              break;
            }
          }
             
          if (k == p-2) 
          {
            kase = 4;
          }
          else
          {
            int ks;
            for (ks = p-1; ks >= k; ks--) 
            {
              if (ks == k) 
                break;
                   
              double t = (ks != p ? Math.Abs(e[ks]) : 0.0) + (ks != k+1 ? Math.Abs(e[ks-1]) : 0.0);
              if (Math.Abs(s[ks]) <= eps*t)  
              {
                s[ks] = 0.0;
                break;
              }
            }
            
            if (ks == k) 
              kase = 3;
            else if (ks == p-1) 
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
              double f = e[p-2];
              e[p-2] = 0.0;
              for (int j = p-2; j >= k; j--) 
              {
                double t = Hypotenuse(s[j],f);
                double cs = s[j]/t;
                double sn = f/t;
                s[j] = t;
                if (j != k) 
                {
                  f = -sn*e[j-1];
                  e[j-1] = cs*e[j-1];
                }
                    
                if (wantv) 
                {
                  for (int i = 0; i < n; i++) 
                  {
                    t = cs*v[i][j] + sn*v[i][p-1];
                    v[i][p-1] = -sn*v[i][j] + cs*v[i][p-1];
                    v[i][j] = t;
                  }
                }
              }
            }
              break;
    
              // Split at negligible s(k).
            case 2: 
            {
              double f = e[k-1];
              e[k-1] = 0.0;
              for (int j = k; j < p; j++) 
              {
                double t = Hypotenuse(s[j],f);
                double cs = s[j]/t;
                double sn = f/t;
                s[j] = t;
                f = -sn*e[j];
                e[j] = cs*e[j];
                if (wantu) 
                {
                  for (int i = 0; i < m; i++) 
                  {
                    t = cs*u[i][j] + sn*u[i][k-1];
                    u[i][k-1] = -sn*u[i][j] + cs*u[i][k-1];
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
              double scale = Math.Max(Math.Max(Math.Max(Math.Max(Math.Abs(s[p-1]),Math.Abs(s[p-2])),Math.Abs(e[p-2])), Math.Abs(s[k])),Math.Abs(e[k]));
              double sp = s[p-1]/scale;
              double spm1 = s[p-2]/scale;
              double epm1 = e[p-2]/scale;
              double sk = s[k]/scale;
              double ek = e[k]/scale;
              double b = ((spm1 + sp)*(spm1 - sp) + epm1*epm1)/2.0;
              double c = (sp*epm1)*(sp*epm1);
              double shift = 0.0;
              if ((b != 0.0) | (c != 0.0)) 
              {
                shift = Math.Sqrt(b*b + c);
                if (b < 0.0)
                  shift = -shift;
                shift = c/(b + shift);
              }
                  
              double f = (sk + sp)*(sk - sp) + shift;
              double g = sk*ek;
       
              // Chase zeros.
              for (int j = k; j < p-1; j++) 
              {
                double t = Hypotenuse(f,g);
                double cs = f/t;
                double sn = g/t;
                if (j != k)
                  e[j-1] = t;
                f = cs*s[j] + sn*e[j];
                e[j] = cs*e[j] - sn*s[j];
                g = sn*s[j+1];
                s[j+1] = cs*s[j+1];
                if (wantv) 
                {
                  for (int i = 0; i < n; i++) 
                  {
                    t = cs*v[i][j] + sn*v[i][j+1];
                    v[i][j+1] = -sn*v[i][j] + cs*v[i][j+1];
                    v[i][j] = t;
                  }
                }
                      
                t = Hypotenuse(f, g);
                cs = f/t;
                sn = g/t;
                s[j] = t;
                f = cs*e[j] + sn*s[j+1];
                s[j+1] = -sn*e[j] + cs*s[j+1];
                g = sn*e[j+1];
                e[j+1] = cs*e[j+1];
                if (wantu && (j < m-1)) 
                {
                  for (int i = 0; i < m; i++) 
                  {
                    t = cs*u[i][j] + sn*u[i][j+1];
                    u[i][j+1] = -sn*u[i][j] + cs*u[i][j+1];
                    u[i][j] = t;
                  }
                }
              }
                  
              e[p-2] = f;
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
                if (s[k] >= s[k+1]) 
                  break;
    
                double t = s[k];
                s[k] = s[k+1];
                s[k+1] = t;
                if (wantv && (k < n-1)) 
                  for (int i = 0; i < n; i++) 
                  {
                    t = v[i][k+1]; 
                    v[i][k+1] = v[i][k]; 
                    v[i][k] = t;
                  }
                  
                if (wantu && (k < m-1)) 
                  for (int i = 0; i < m; i++) 
                  {
                    t = u[i][k+1]; 
                    u[i][k+1] = u[i][k]; 
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
        Backsubstitution(VectorMath.ToROVector(b),VectorMath.ToVector(x));
      }


      private class SolveTempStorage
      {
        public IMatrix A;
        public SingularValueDecomposition SVD;
      }


      /// <summary>
      /// Solves the equation A x = B and returns x.
      /// </summary>
      /// <param name="A">The matrix.</param>
      /// <param name="B">The right side.</param>
      /// <param name="x">On return, contains the solution vector.</param>
      /// <param name="tempstorage">On return, holds the allocated temporary storage. You can use this
      /// in subsequent calls to Solve with the same dimensions of the matrix.</param>
      public static void Solve(IROMatrix A, IROVector B, IVector x, ref object tempstorage)
      {
        SolveTempStorage sts;
        if (tempstorage is SolveTempStorage)
        {
          sts = (SolveTempStorage)tempstorage;
          if (sts.A.Rows == A.Rows && sts.A.Columns == A.Columns)
          {
            MatrixMath.Copy(A, sts.A);
            sts.SVD.ComputeSingularValueDecomposition(sts.A);
            sts.SVD.Backsubstitution(B, x);
            return;
          }
        }
        // tempstorage can not be used
        sts = new SolveTempStorage();
        sts.A = new MatrixMath.BEMatrix(A.Rows,A.Columns);
        MatrixMath.Copy(A,sts.A);
        sts.SVD = new SingularValueDecomposition(sts.A);
        sts.SVD.Backsubstitution(B,x);
        tempstorage = sts;
        return;
      }

      /// <summary>
      /// Solves A·X = B for a vector X, where A is specified by A=U*S*V'. U, S, and V are
      /// results of the decomposition.
      /// </summary>
      /// <param name="b">Input vector.</param>
      /// <param name="x">The resulting vector that fullfilles A·x=b.</param>
      public void Backsubstitution(IROVector b, IVector x)
      {
                                                                                                    
    
        double sum;
        double[] tmp = new double[n];
        int nu = Math.Min(m,n);

        for (int j=0;j<nu;j++) // calculate UT*B  (UT == U transposed)
        {
          sum=0.0;
          if (s[j]!=0) 
          {
            // Nonzero result only if sj is nonzero.
            for (int i=0;i<m;i++)
              sum += u[i][j]*b[i];
            sum /= s[j]; // This is the divide by sj .
          }
          tmp[j]=sum;
        }
        for (int j=0;j<n;j++) // Matrix multiply by V to get answer.
        {
          sum=0.0;
          for (int jj=0;jj<n;jj++)
            sum += v[j][jj]*tmp[jj];
          x[j]=sum;
        }
      }


      /// <summary>
      /// Calculates the covariance matrix Cov(i,j)= (X'X)^(-1) = SUM_over_k( V[i,k]*V[j,k]/s[k]^2). If s[k] is zero, 1/s[k]^2 will be set to zero. If the singular value decomposition was used to make a linear fit,
      /// this is the variance-covariance matrix of the fitting parameters.
      /// </summary>
      /// <returns>The variance-covariance-matrix.</returns>
      public double[][] GetCovariances()
      {
        double[][] cvm = MatrixMath.GetMatrixArray(n,n);
        double[] wti = new double[n];
        for(int i=0;i<n;i++)
          wti[i] = (i<m && s[i]!=0) ? 1/(s[i]*s[i]) : 0;

        for(int i=0;i<n;i++)
        {
          for(int j=0;j<=i;j++)
          {
            double sum=0;
            for(int k=0;k<n;k++)
              sum += v[i][k]*v[j][k]*wti[k];

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

        double maxSingularValue=0;
        for(int i=0;i<s.Length;i++)
          maxSingularValue = Math.Max(maxSingularValue,s[i]);

        double thresholdLevel = maxSingularValue*relativeThreshold;

        // set singular values < thresholdLevel to zero
        for(int i=0;i<s.Length;i++)
          if(s[i]<thresholdLevel)
            s[i]=0;
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
          double eps = Math.Pow(2.0,-52.0);
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
          if(null==_HatDiagonal)
          {
            BuildHatDiagonal();
          }
          return _HatDiagonal;
        }
      }

      protected void BuildHatDiagonal()
      {
        _HatDiagonal = new double[u.Length];
        for(int i=0;i<u.Length;i++)
        {
          double sum = 0;
          int cols = u[0].Length;
          for(int j=0;j<cols;j++)
            sum += u[i][j]*u[i][j];
          
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
  

    #endregion

  } // end class MatrixMath


}
