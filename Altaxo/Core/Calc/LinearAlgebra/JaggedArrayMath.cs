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

namespace Altaxo.Calc.LinearAlgebra
{
	/// <summary>
	/// This provides array math for a special case of matrices, so called jagged arrays.
	/// </summary>
	public class JaggedArrayMath
	{
		private JaggedArrayMath()	{}

    #region Creation
    /// <summary>
    /// Allocates an array of n x m values.
    /// </summary>
    /// <param name="n">First matrix dimension (rows).</param>
    /// <param name="m">Second matrix dimension( columns).</param>
    /// <returns>Array of dimensions n x m.</returns>
    public static double[][] GetMatrixArray(int n, int m)
    {
      double[][] result = new double[n][];
      for(int i=0;i<n;i++)
        result[i] = new double[m];

      return result;
    }

    #endregion

    #region Type conversion

    /// <summary>
    /// This wraps a jagged double array to the <see>IMatrix</see> interface. The data is not copied!
    /// </summary>
    /// <param name="x">The jagged array. Each double[] vector is a row of the matrix.</param>
    /// <param name="rows">The number of (used) rows of the array.</param>
    /// <param name="cols">The number of (used) columns of the array.</param>
    /// <returns>A jagged array matrix wrapping the provided array with a IMatrix interface.</returns>
    public static JaggedArrayMatrix ToMatrix(double[][] x, int rows, int cols)
    {
      return new JaggedArrayMatrix(x,rows,cols);
    }

    #endregion


    #region Addition, Subtraction, Multiply and combined operations

    #region Addition
    /// <summary>
    /// Calculates a+b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="arows">Number of rows of a.</param>
    /// <param name="acols">Number of columns of a.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="brows">Number of rows of b.</param>
    /// <param name="bcols">Number of columns of b.</param>
    /// <param name="c">The matrix where to store the result. Has to be of same dimensions than a and b.</param>
    /// <param name="crows">Number of rows of c.</param>
    /// <param name="ccols">Number of columns of c.</param>
    public static void Add(
      double[][] a, int arows, int acols,
      double[][] b, int brows, int bcols, 
      double[][] c, int crows, int ccols)    
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(acols!=bcols || arows!=brows)
        throw new ArithmeticException(string.Format("Try to add a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",arows,acols,brows,bcols));
      if(crows != arows || ccols != acols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the proper dimension ({2},{3})",crows,ccols,arows,acols));

      for(int i=0;i<crows;i++)
        for(int j=0;j<ccols;j++)
          c[i][j] = a[i][j]+b[i][j];
    }

    #endregion

    #region Multiplication

    /// <summary>
    /// Multiplies matrix a with matrix b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="arows">Number of rows of a.</param>
    /// <param name="acols">Number of columns of a.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="brows">Number of rows of b.</param>
    /// <param name="bcols">Number of columns of b.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (arows, bcols).</param>
    /// <param name="crows">Number of rows of c.</param>
    /// <param name="ccols">Number of columns of c.</param>
    public static void Multiply(
      double[][] a, int arows, int acols,
      double[][] b, int brows, int bcols, 
      double[][] c, int crows, int ccols)
    {
      int numil = brows; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if(acols!=numil)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",arows,acols,brows,bcols));
      if(crows != arows || ccols != bcols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,bcols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[i][k]*b[k][j];
        
          c[i][j] = sum;
        }
      }
    }

    /// <summary>
    /// Multiplies matrix a_transposed with matrix b and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="arows">Number of rows of a.</param>
    /// <param name="acols">Number of columns of a.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="brows">Number of rows of b.</param>
    /// <param name="bcols">Number of columns of b.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (acols, bcols).</param>
    /// <param name="crows">Number of rows of c.</param>
    /// <param name="ccols">Number of columns of c.</param>
    public static void MultiplyFirstTransposed(
      double[][] a, int arows, int acols,
      double[][] b, int brows, int bcols, 
      double[][] c, int crows, int ccols)    
    {
      int numil = brows; // number of summands for most inner loop

      // Presumtion:
      if(arows!=brows)
        throw new ArithmeticException(string.Format("Try to multiplicate a transposed matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",arows,acols,brows,bcols));
      if(crows != acols || ccols != bcols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})",crows,ccols,acols,bcols));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[k][i]*b[k][j];
        
          c[i][j] = sum;
        }
      }
    }


    /// <summary>
    /// Multiplies matrix a with matrix b_transposed and stores the result in matrix c.
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="arows">Number of rows of a.</param>
    /// <param name="acols">Number of columns of a.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="brows">Number of rows of b.</param>
    /// <param name="bcols">Number of columns of b.</param>
    /// <param name="c">The matrix where to store the result. Has to be of dimension (arows, brows).</param>
    /// <param name="crows">Number of rows of c.</param>
    /// <param name="ccols">Number of columns of c.</param> 
    public static void MultiplySecondTransposed(
      double[][] a, int arows, int acols,
      double[][] b, int brows, int bcols, 
      double[][] c, int crows, int ccols)       
    {
      int numil = bcols; // number of summands for most inner loop

      // Presumtion:
      // a.Cols == b.Rows;
      if(acols!=bcols)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",arows,acols,brows,bcols));
      if(crows != arows || ccols != brows)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1}))has not the expected dimension ({2},{3})",crows,ccols,arows,brows));

      for(int i=0;i<crows;i++)
      {
        for(int j=0;j<ccols;j++)
        {
          double sum=0;
          for(int k=0;k<numil;k++)
            sum += a[i][k]*b[j][k];
        
          c[i][j] = sum;
        }
      }
    }


    /// <summary>
    /// Multiplies the matrix a with a scalar value b and stores the result in c. Matrix a and c are allowed to be the same matrix.
    /// </summary>
    /// <param name="a">The first multiplicant.</param>
    /// <param name="arows">Number of rows of a.</param>
    /// <param name="acols">Number of columns of a.</param>
    /// <param name="b">The second multiplicant.</param>
    /// <param name="c">The resulting matrix. Has to be of same dimensions than a.</param>
    /// <param name="crows">Number of rows of c.</param>
    /// <param name="ccols">Number of columns of c.</param> 
    public static void MultiplyScalar(
      double[][] a, int arows, int acols,
      double b,
      double[][] c, int crows, int ccols)
    {
      if(crows != arows || ccols != acols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1})) has not the expected dimension ({2},{3})",crows,ccols,arows,acols));

      for(int i=0;i<arows;i++)
      {
        for(int j=0;j<acols;j++)
        {
          c[i][j] = a[i][j]*b;
        }
      }
    }

    /// <summary>
    /// Multiplies the row <c>rowb</c> of matrix b element by element to all rows of matrix a. 
    /// </summary>
    /// <param name="a">First multiplicant.</param>
    /// <param name="arows">Number of rows of a.</param>
    /// <param name="acols">Number of columns of a.</param>
    /// <param name="b">Second multiplicant.</param>
    /// <param name="brows">Number of rows of b.</param>
    /// <param name="bcols">Number of columns of b.</param>
    /// <param name="browToMultiply">The row number of matrix b to multiply.</param>
    /// <param name="c">The matrix where to store the result. Has to be of same dimension than a.</param>
    /// <param name="crows">Number of rows of c.</param>
    /// <param name="ccols">Number of columns of c.</param> 
    public static void MultiplyRow(

      double[][] a, int arows, int acols,
      double[][] b, int brows, int bcols, 
      int browToMultiply,
      double[][] c, int crows, int ccols)           
    {
      // Presumtion:
      if(arows != crows || acols != ccols)
        throw new ArithmeticException(string.Format("The provided resultant matrix (actual dim({0},{1]))has not the expected dimension ({2},{3})",crows,ccols,arows,acols));
      if(bcols != acols)
        throw new ArithmeticException(string.Format("Matrix b[{0},{1}] has not the same number of columns than matrix a[{2},{3}]!",brows,bcols,arows,acols));
      if(object.ReferenceEquals(b,c))
        throw new ArithmeticException("Matrix b and c are identical, which is not allowed here!");

      for(int i=0;i<arows;i++)
        for(int j=0;j<acols;j++)
          c[i][j] = a[i][j]*b[browToMultiply][j];
    }

    #endregion // Multiplication

   


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

	}
}
