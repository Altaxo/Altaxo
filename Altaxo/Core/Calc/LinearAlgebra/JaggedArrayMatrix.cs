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

using System;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// JaggedArrayMatrix is a matrix implementation that is relatively easy to extend to the bottom, i.e. to append rows.
  /// It is horizontal oriented, i.e. the storage is as a number of horizontal vectors. Furthermore, as a compromise, it provides fully
  /// access to its underlying jagged array.
  /// </summary>
  public class JaggedArrayMatrix : IMatrix, IBottomExtensibleMatrix
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
    public JaggedArrayMatrix(int rows, int cols)
    {
      SetDimension(rows,cols);
    }

    /// <summary>
    /// Uses an already existing array for the matrix data.
    /// </summary>
    /// <param name="x">Jagged double array containing the matrix data. The data are used directly (no copy)!</param>
    public JaggedArrayMatrix(double[][]x)
      : this(x, x.Length, x.Length==0 ? 0 : x[0].Length)
    {
    }

    /// <summary>
    /// Uses an already existing array for the matrix data.
    /// </summary>
    /// <param name="x">Jagged double array containing the matrix data. The data are used directly (no copy)!</param>
    /// <param name="rows">Number of (used) rows of the matrix.</param>
    /// <param name="cols">Number of columns of the matrix.</param>
    public JaggedArrayMatrix(double[][]x, int rows, int cols)
    {
      this.m_Array = x;
      this.m_Rows = rows;
      this.m_Cols = cols;
    }


    /// <summary>
    /// Provides direct access to the underlying jagged array. JaggedArrayMath can then be used to speed up computations.
    /// </summary>
    public double[][] Array
    {
      get { return m_Array; }
    }

    /// <summary>
    /// Clears the content of the matrix.
    /// </summary>
    public void Clear()
    {
      SetDimension(0,0);
    }

    /// <summary>
    /// Represents the content of the matrix as a string.
    /// </summary>
    /// <returns></returns>
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

  
}
