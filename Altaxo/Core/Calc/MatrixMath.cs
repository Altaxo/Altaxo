#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Calc
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
      public void AppendBottom(IMatrix a)
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
      public void AppendRight(IMatrix a)
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

    public static string MatrixToString(string name, IMatrix a)
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
    public static void SelfSubtractProduct(IROMatrix a, IROMatrix b, IMatrix c)
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
    public static void SelfSubtractProduct(IROMatrix a, double b, IMatrix c)
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

    #endregion

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

    /// <summary>
    /// This will center the matrix so that the mean of each column is null, and the variance of each column is one.
    /// </summary>
    /// <param name="a">The matrix where the columns should be centered and normalized to standard variance.</param>
    /// <param name="meanvec">You can provide a matrix of dimension(1,a.Cols) where the mean row vector is stored, or null if not interested in this vector.</param>
    /// <param name="scorevec">You can provide a matrix of dimension(1,a.Cols) where the inverse of the variance of the columns is stored, or null if not interested in this vector.</param>
    /// <remarks>Calling this function will change the matrix a to a column
    /// centered matrix. The original matrix data are lost.</remarks>
    public static void ColumnsToZeroMeanAndUnitVariance(IMatrix a, IMatrix meanvec, IMatrix scorevec)
    {
      if(null!=meanvec && (meanvec.Rows != 1 || meanvec.Columns != a.Columns))
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",meanvec.Rows,meanvec.Columns,1,a.Columns));

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
          meanvec[0,col] = mean;
        if(null!=scorevec)
          scorevec[0,col] = scor;
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
        sum = 1/Math.Sqrt(sum);
        for(int j=0;j<a.Columns;j++)
          a[i,j] *= sum;
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
        sum = 1/Math.Sqrt(sum);
        for(int j=0;j<a.Rows;j++)
          a[j,i] *= sum;
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
        SelfSubtractProduct(t,l,X); // X is now the residual matrix

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


    /// <summary>
    /// Partial least squares (PLS) decomposition of the matrizes X and Y.
    /// </summary>
    /// <param name="_X">The X ("spectrum") matrix.</param>
    /// <param name="_Y">The Y ("concentration") matrix.</param>
    /// <param name="numFactors">Number of factors to calculate.</param>
    /// <param name="xLoads">Returns the matrix of eigenvectors of X. Should be initially empty.</param>
    /// <param name="yLoads">Returns the matrix of eigenvectors of Y. Should be initially empty. </param>
    /// <param name="W">Returns the matrix of weighting values. Should be initially empty.</param>
    /// <param name="V">Returns the vector of cross products. Should be initially empty.</param>
    public static void PartialLeastSquares_HO(
      IROMatrix _X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix _Y, // matrix of concentrations (a mixture is a row of this matrix)
      ref int numFactors,
      IBottomExtensibleMatrix xLoads, // out: the loads of the X matrix
      IBottomExtensibleMatrix yLoads, // out: the loads of the Y matrix
      IBottomExtensibleMatrix W, // matrix of weighting values
      IRightExtensibleMatrix V  // matrix of cross products
      )
    {
      // used variables:
      // n: number of spectra (number of tests, number of experiments)
      // p: number of slots (frequencies, ..) in each spectrum
      // m: number of constitutents (number of y values in each measurement)
      
      // X : n-p matrix of spectra (each spectra is a horizontal row)
      // Y : n-m matrix of concentrations


      const int maxIterations = 1500; // max number of iterations in one factorization step
      const double accuracy = 1E-12; // accuracy that should be reached between subsequent calculations of the u-vector



      // use the mean spectrum as first row of the W matrix
      MatrixMath.HorizontalVector mean = new HorizontalVector(_X.Columns);
      //  MatrixMath.ColumnsToZeroMean(X,mean);
      //W.AppendBottom(mean);

      IMatrix X = new BEMatrix(_X.Rows,_X.Columns);
      MatrixMath.Copy(_X,X);
      IMatrix Y = new BEMatrix(_Y.Rows,_Y.Columns);
      MatrixMath.Copy(_Y,Y);

      IMatrix u_prev = null;
      IMatrix w = new HorizontalVector(X.Columns); // horizontal vector of X (spectral) weighting
      IMatrix t = new VerticalVector(X.Rows); // vertical vector of X  scores
      IMatrix u = new VerticalVector(X.Rows); // vertical vector of Y scores
      IMatrix p = new HorizontalVector(X.Columns); // horizontal vector of X loads
      IMatrix q = new HorizontalVector(Y.Columns); // horizontal vector of Y loads

      int maxFactors = Math.Min(X.Columns,X.Rows);
      numFactors = numFactors<=0 ? maxFactors : Math.Min(numFactors,maxFactors);

      for(int nFactor=0; nFactor<numFactors; nFactor++)
      {
        Console.WriteLine("Factor_{0}:",nFactor);
        Console.WriteLine("X:"+X.ToString());
        Console.WriteLine("Y:"+Y.ToString());

  
        // 1. Use as start vector for the y score the first column of the 
        // y-matrix
        Submatrix(X,u); // u is now a vertical vector of concentrations of the first constituents

        for(int iter=0;iter<maxIterations;iter++)
        {
          // 2. Calculate the X (spectrum) weighting vector
          MultiplyFirstTransposed(u,X,w); // w is a horizontal vector

          // 3. Normalize w to unit length
          MatrixMath.NormalizeRows(w); // w now has unit length

          // 4. Calculate X (spectral) scores
          MatrixMath.MultiplySecondTransposed(X,w,t); // t is a vertical vector of n numbers

          // 5. Calculate the Y (concentration) loading vector
          MatrixMath.MultiplyFirstTransposed(t,Y,q); // q is a horizontal vector of m (number of constitutents)

          // 5.1 Normalize q to unit length
          MatrixMath.NormalizeRows(q);

          // 6. Calculate the Y (concentration) score vector u
          MatrixMath.MultiplySecondTransposed(Y,q,u); // u is a vertical vector of n numbers

          // 6.1 Compare
          // Compare this with the previous one 
          if(u_prev!=null && IsEqual(u_prev,u,accuracy))
            break;
          if(u_prev==null)
            u_prev = new VerticalVector(X.Rows);
          Copy(u,u_prev); // stores the content of u in u_prev
        } // for all iterations

        // Store the scores of X
        //factors.AppendRight(t);


        // 7. Calculate the inner scalar (cross product)
        double length_of_t = MatrixMath.LengthOf(t); 
        Scalar v = new Scalar(0);
        MatrixMath.MultiplyFirstTransposed(u,t,v);
        v = v/Square(length_of_t); 
      
        // 8. Calculate the new loads for the X (spectral) matrix
        MatrixMath.MultiplyFirstTransposed(t,X,p); // p is a horizontal vector of loads
        // Normalize p by the spectral scores
        MatrixMath.MultiplyScalar(p,1/Square(length_of_t),p);

        // 9. Calculate the new residua for the X (spectral) and Y (concentration) matrix
        //MatrixMath.MultiplyScalar(t,length_of_t*v,t); // original t times the cross product

        MatrixMath.SelfSubtractProduct(t,p,X);
        MatrixMath.SelfSubtractProduct(t,q,Y);

        // Store the loads of X and Y in the output result matrix
        xLoads.AppendBottom(p);
        yLoads.AppendBottom(q);
        W.AppendBottom(w);
        V.AppendRight(v);
    
        // Calculate SEPcv. If SEPcv is greater than for the actual number of factors,
        // break since the optimal number of factors was found. If not, repeat the calculations
        // with the residual matrizes for the next factor.
      } // for all factors
    }


    public static void PartialLeastSquares_Predict_HO(
      IROMatrix XU, // unknown spectrum or spectra,  horizontal oriented
      IROMatrix xLoads, // x-loads matrix
      IROMatrix yLoads, // y-loads matrix
      IROMatrix W, // weighting matrix
      IROMatrix V,  // Cross product vector
      int numFactors, // number of factors to use for prediction
      ref IMatrix predictedY // Matrix of predicted y-values, must be same number of rows as spectra
      )
    {
      // now predicting a "unkown" spectra
      MatrixMath.Scalar si = new MatrixMath.Scalar(0);
      MatrixMath.HorizontalVector Cu = new MatrixMath.HorizontalVector(yLoads.Columns);

      MatrixMath.HorizontalVector wi = new MatrixMath.HorizontalVector(XU.Columns);
      MatrixMath.HorizontalVector cuadd = new MatrixMath.HorizontalVector(yLoads.Columns);
      
      // xu holds a single spectrum extracted out of XU
      MatrixMath.HorizontalVector xu = new MatrixMath.HorizontalVector(XU.Columns);

      // xl holds temporarily a row of the xLoads matrix+
      MatrixMath.HorizontalVector xl = new MatrixMath.HorizontalVector(xLoads.Columns);


      int maxFactors = Math.Min(yLoads.Rows,numFactors);
      

      for(int nSpectrum=0;nSpectrum<XU.Rows;nSpectrum++)
      {
        MatrixMath.Submatrix(XU,xu,nSpectrum,0); // extract one spectrum to predict
        MatrixMath.ZeroMatrix(Cu); // Set Cu=0
        for(int i=0;i<maxFactors;i++)
        {
          //1. Calculate the unknown spectral score for a weighting vector
          MatrixMath.Submatrix(W,wi,i,0);
          MatrixMath.MultiplySecondTransposed(wi,xu,si);
          // take the y loading vector
          MatrixMath.Submatrix(yLoads,cuadd,i,0);
          // and multiply it with the cross product and the score
          MatrixMath.MultiplyScalar(cuadd,si*V[0,i],cuadd);
          // Add it to the predicted y-values
          MatrixMath.Add(Cu,cuadd,Cu);
          // remove the spectral contribution of the factor from the spectrum
          // TODO this is quite ineffective: in every loop we extract the xl vector, we have to find a shortcut for this!
          MatrixMath.Submatrix(xLoads,xl,i,0);
          MatrixMath.SelfSubtractProduct(xl,(double)si,xu);
        }
        // Cu now contains the predicted y values
        MatrixMath.SetRow(Cu,0,predictedY,nSpectrum);
      } // for each spectrum in XU
    } // end partial-least-squares-predict

    public static void PartialLeastSquares_CrossValidation_HO(
      IROMatrix X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix Y, // matrix of concentrations (a mixture is a row of this matrix)
      int numFactors,
      out IMatrix crossPRESSMatrix // vertical value of PRESS values for the cross validation
      )
    {
      IBottomExtensibleMatrix xLoads = new MatrixMath.BEMatrix(0,0); // out: the loads of the X matrix
      IBottomExtensibleMatrix yLoads = new MatrixMath.BEMatrix(0,0); // out: the loads of the Y matrix
      IBottomExtensibleMatrix W = new MatrixMath.BEMatrix(0,0); // matrix of weighting values
      IRightExtensibleMatrix V = new MatrixMath.REMatrix(0,0); // matrix of cross products

      double[] crossPRESS = null;

      int numberOfExcludedSpectraOfGroup = 1;

      IMatrix XX = new BEMatrix(X.Rows-numberOfExcludedSpectraOfGroup,X.Columns);
      IMatrix YY = new BEMatrix(Y.Rows-numberOfExcludedSpectraOfGroup,Y.Columns);
      IMatrix XU = new BEMatrix(numberOfExcludedSpectraOfGroup,X.Columns);
      IMatrix YU = new BEMatrix(numberOfExcludedSpectraOfGroup,Y.Columns);
      IMatrix predY = new BEMatrix(numberOfExcludedSpectraOfGroup,Y.Columns);

      for(int nGroup=0;nGroup < X.Rows;nGroup++)
      {
        // build a new x and y matrix with the group information
        // fill XX and YY with values
        int i,j;
        for(i=0,j=0;i<X.Rows;i++)
        {
          if(i==nGroup)
            continue; // Exclude this row from the spectra
          MatrixMath.SetRow(X,i,XX,j);
          MatrixMath.SetRow(Y,i,YY,j);
          j++;
        }
        // fill XU (unknown spectra) with values
        MatrixMath.SetRow(X,nGroup,XU,0); // x-unkown (unknown spectra)
        MatrixMath.SetRow(Y,nGroup,YU,0); // y-unkown (unknown concentration)


        // do a PLS with the builded matrices
        xLoads = new MatrixMath.BEMatrix(0,0); // clear xLoads
        yLoads = new MatrixMath.BEMatrix(0,0); // clear yLoads
        W      = new MatrixMath.BEMatrix(0,0); // clear W
        V      = new MatrixMath.REMatrix(0,0); // clear V
        int actnumfactors=numFactors;
        PartialLeastSquares_HO(XX, YY, ref actnumfactors, xLoads,yLoads,W,V); 
        numFactors = Math.Min(numFactors,actnumfactors); // if we have here a lesser number of factors, use it for all further calculations
                            

        // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
        if(null==crossPRESS)
          crossPRESS = new double[numFactors+1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

        // for all factors do now a prediction of the remaining spectra
        crossPRESS[0] += MatrixMath.SumOfSquares(YU);
        for(int nFactor=1;nFactor<=numFactors;nFactor++)
        {
          PartialLeastSquares_Predict_HO(XU,xLoads,yLoads,W,V,nFactor,ref predY);
          crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(YU,predY);
        }
      } // for all groups


      // copy the resulting crossPRESS values into a matrix
      crossPRESSMatrix = new MatrixMath.VerticalVector(numFactors+1);
      for(int i=0;i<=numFactors;i++)
        crossPRESSMatrix[i,0] = crossPRESS[i];
    }



  } // end class MatrixMath


}
