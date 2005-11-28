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


  class TransposableMatrix : IExtensibleMatrix
  {
    double[][] m_Array;
    int m_NumVectors;
    int m_VectorLen;
    bool m_bVerticalVectors=false; // normally the matrix consists of several rows containing column vectors

    public TransposableMatrix()
    {
      m_NumVectors = 0;
      m_VectorLen = 0;
      m_bVerticalVectors = false;
    }

    public TransposableMatrix(int rows, int cols)
    {
      SetDimension(rows,cols);
    }

    public TransposableMatrix(double [][] arr)
    {
      // get the max number of columns in the matrix  
      int cols=0;
      for(int i=0;i<arr.Length;i++)
        cols = Math.Max(cols,arr[i].Length);

      // set the dimension of our matrix
      SetDimension(arr.Length,cols);

      // copy the values in our array
      for(int i=0; i<arr.Length;i++)
        Array.Copy(arr[i],0,m_Array[i],0,arr[i].Length);
    }

    #region IMatrix Members


    public void SetDimension(int rows, int cols)
    {
      m_NumVectors = rows;
      m_VectorLen = cols;
      m_bVerticalVectors = false;
      m_Array = new double[rows][];
      for(int i=0;i<m_Array.Length;i++)
        m_Array[i] = new double[cols];
    }

    public double this[int i, int k]
    {
      get
      {
        return m_bVerticalVectors ? m_Array[k][i] : m_Array[i][k];
      }
      set
      {
        if(m_bVerticalVectors)
          m_Array[k][i] = value;
        else
          m_Array[i][k] = value;
      }
    }

    public int Rows
    {
      get
      {
        return m_bVerticalVectors ? m_VectorLen : m_NumVectors;
      }
    }

    public int Columns
    {
      get
      {
        return m_bVerticalVectors ? m_NumVectors : m_VectorLen;
      }
    }

    #endregion

    public void AppendBottom(IROMatrix a)
    {
      if(m_NumVectors==0 && m_VectorLen==0)
      {
        m_bVerticalVectors = false;
        m_NumVectors = a.Rows;
        m_VectorLen = a.Columns;
        m_Array = new double[m_NumVectors][];
        for(int i=0;i<m_NumVectors;i++)
          m_Array[i] = new double[m_VectorLen];
        MatrixMath.Copy(a,this);
      }
      else if(this.m_bVerticalVectors==false)
      {
        double [][] newArray = new double[m_NumVectors+a.Rows][];
        for(int i=0;i<m_NumVectors;i++)
          newArray[i] = m_Array[i];
        for(int i=m_NumVectors; i<m_NumVectors+a.Rows; i++)
          newArray[i] = new double[m_VectorLen];
        m_Array = newArray;
        m_NumVectors += a.Rows;

        MatrixMath.Copy(a,this, this.Rows-a.Rows, 0);
      }
      else
        throw new System.NotImplementedException("This worst case is not implemented yet.");
    }

    public void AppendRight(IROMatrix a)
    {
      if(m_NumVectors==0 && m_VectorLen==0)
      {
        m_bVerticalVectors = true;
        m_NumVectors = a.Columns;
        m_VectorLen = a.Rows;
        m_Array = new double[m_NumVectors][];
        for(int i=0;i<m_NumVectors;i++)
          m_Array[i] = new double[m_VectorLen];
        MatrixMath.Copy(a,this);
      }
      else if(this.m_bVerticalVectors==true)
      {
        double [][] newArray = new double[m_NumVectors+a.Columns][];
        for(int i=0;i<m_NumVectors;i++)
          newArray[i] = m_Array[i];
        for(int i=m_NumVectors; i<m_NumVectors+a.Columns; i++)
          newArray[i] = new double[m_VectorLen];
        m_Array = newArray;
        m_NumVectors += a.Columns;

        MatrixMath.Copy(a,this, 0, this.Columns-a.Columns);
      }
      else
        throw new System.NotImplementedException("This worst case is not implemented yet.");
    }

    public override string ToString()
    {
      System.Text.StringBuilder s = new System.Text.StringBuilder();
      for(int i=0;i<Rows;i++)
      {
        s.Append("\n(");
        for(int j=0;j<Columns;j++)
        {
          s.Append(this[i,j].ToString());
          if(j+1<Columns) 
            s.Append(",");
          else
            s.Append(")");
        }
      }
      return s.ToString();
    }

    public string MatrixForm { get { return this.ToString(); }}

    public void Transpose()
    {
      m_bVerticalVectors = !m_bVerticalVectors;
    }
    

    public static TransposableMatrix operator*(TransposableMatrix a, TransposableMatrix b)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=b.Rows)
        throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));

      int rows = a.Rows;
      int cols = b.Columns;
      TransposableMatrix c = new TransposableMatrix(rows,cols);

      int summands = b.Rows;
      for(int i=0;i<rows;i++)
      {
        for(int j=0;j<cols;j++)
        {
          double sum=0;
          for(int k=0;k<summands;k++)
            sum += a[i,k]*b[k,j];
        
          c[i,j] = sum;
        }
      }
      return c;
    }

    public static TransposableMatrix operator -(TransposableMatrix a, TransposableMatrix b)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(a.Columns!=b.Columns || a.Rows!=b.Rows)
        throw new ArithmeticException(string.Format("Try to subtract a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",a.Rows,a.Columns,b.Rows,b.Columns));

      TransposableMatrix c = new TransposableMatrix(a.Rows,a.Columns);
      for(int i=0;i<c.Rows;i++)
        for(int j=0;j<c.Columns;j++)
          c[i,j] = a[i,j]-b[i,j];

      return c; 
    }

    /// <summary>
    /// This will center the matrix so that the mean of each column is null.
    /// </summary>
    /// <returns>The mean, which is a horzizontal vector of dimension 1,col.</returns>
    public TransposableMatrix ColumnsToZeroMean()
    {
      // allocate the mean vector
      TransposableMatrix mean = new TransposableMatrix(1,Columns);

      for(int col = 0; col<Columns; col++)
      {
        double sum = 0;
        for(int row=0;row<Rows;row++)
          sum += this[row,col];
        sum /= Rows; // calculate the mean
        for(int row=0;row<Rows;row++)
          this[row,col] -= sum; // subtract the mean from every element in the column
        mean[0,col] = sum;
      }
      return mean;
    }

    public TransposableMatrix Submatrix(int rows, int cols, int rowoffset, int coloffset)
    {
      TransposableMatrix c = new TransposableMatrix(rows, cols);
      for(int i=0;i<rows;i++)
        for(int j=0;j<cols;j++)
          c[i,j] = this[i+rowoffset,j+coloffset];
    
      return c;
    }
    public TransposableMatrix Submatrix(int rows, int cols)
    {
      return Submatrix(rows,cols,0,0);
    }

    public void SetColumn(int col, TransposableMatrix b)
    {
      if(col>=this.Columns)
        throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!",col,this.Rows,this.Columns));
      if(b.Columns!=1)
        throw new ArithmeticException(string.Format("Try to set column {0} with a matrix of more than one, namely {1} columns, is not allowed!",col,b.Columns));
      if(this.Rows != b.Rows)
        throw new ArithmeticException(string.Format("Try to set column {0}, but number of rows of the matrix ({1}) not match number of rows of the vector ({3})!",col,this.Rows,b.Rows));
    
      for(int i=0;i<this.Rows;i++)
        this[i,col]=b[i,0];
    }

    public void SetRow(int row, TransposableMatrix b)
    {
      if(row>=this.Rows)
        throw new ArithmeticException(string.Format("Try to set row {0} in the matrix with dim({1},{2}) is not allowed!",row,this.Rows,this.Columns));
      if(b.Rows!=1)
        throw new ArithmeticException(string.Format("Try to set row {0} with a matrix of more than one, namely {1} rows, is not allowed!",row,b.Rows));
      if(this.Columns != b.Columns)
        throw new ArithmeticException(string.Format("Try to set row {0}, but number of columns of the matrix ({1}) not match number of colums of the vector ({3})!",row,this.Columns,b.Columns));
    
      for(int j=0;j<this.Columns;j++)
        this[row,j]=b[0,row];
    }



    public void NormalizeRows()
    {
      for(int i=0;i<Rows;i++)
      {
        double sum=0;
        for(int j=0;j<Columns;j++)
          sum += this[i,j]*this[i,j];
        sum = Math.Sqrt(sum);
        for(int j=0;j<Columns;j++)
          this[i,j] /= sum;
      }
    }

    public double SumOfSquares()
    {
      double sum=0;
      for(int i=0;i<Rows;i++)
        for(int j=0;j<Columns;j++)
          sum += this[i,j]*this[i,j];
      return sum;
    }

    public bool IsEqualTo(TransposableMatrix m, double accuracy)
    {
      // Presumtion:
      // a.Cols == b.Rows;
      if(this.Columns!=m.Columns || this.Rows != m.Rows)
        throw new ArithmeticException(string.Format("Try to compare a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!",m.Rows,m.Columns,this.Rows,this.Columns));

      double thresh = Math.Sqrt(m.SumOfSquares())*accuracy;
      for(int i=0;i<Rows;i++)
        for(int j=0;j<Columns;j++)
          if(Math.Abs(this[i,j]-m[i,j])>thresh)
            return false;

      return true;
    }

    public static void NIPALS(TransposableMatrix X, int numFactors, out TransposableMatrix factors, out TransposableMatrix loads)
    {
      X.ColumnsToZeroMean();

      double original_variance = Math.Sqrt(X.SumOfSquares());

  
      TransposableMatrix l = null;
      TransposableMatrix t_prev=null;
      TransposableMatrix t=null;

      loads = new TransposableMatrix(numFactors,X.Columns);
      factors = new TransposableMatrix(X.Rows,numFactors);

      for(int nFactor=0; nFactor<numFactors; nFactor++)
      {
        // 1. Guess the transposed Vector lT, use first row of X matrix
        l = X.Submatrix(1,X.Columns);    // l is now a horizontal vector

        for(int iter=0;iter<500;iter++)
        {
      
          // 2. Calculate the new vector t for the factor values
          l.Transpose(); // l is now a vertical vector
          t = X*l; // t is also a vertical vector
          l.Transpose(); // l horizontal again

          t.Transpose(); // t is now horizontal

          // Compare this with the previous one 
          if(t_prev!=null && t_prev.IsEqualTo(t,1E-6))
            break;

          // 3. Calculate the new loads 
          l = t*X; // gives a horizontal vector of load (= eigenvalue spectrum)
          // normalize the (one) row
          l.NormalizeRows(); // normalize the eigenvector spectrum

          // 4. Goto step 2 or break after a number of iterations
          t_prev = t;

        }

        // 5. Calculate the residual matrix 
        t.Transpose(); // t is now vertical again
        factors.SetColumn(nFactor,t);
        loads.SetRow(nFactor,l);

        X = X - t*l; // X is now the residual matrix
      } // for all factors
    }
  }


}
