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
 * FloatLUDecomp.cs
 * Managed code is a port of JAMA and Jampack code.
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;


namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>This class computes the LU factorization of a general n by n <c>FloatMatrix</c>.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public sealed class FloatLUDecomp : Algorithm
  {
    private readonly int order;
    private bool singular;
    private int[] pivots;
    private FloatMatrix l;
    private FloatMatrix u;
    private int sign;
    private FloatMatrix matrix;

#if MANAGED
    private float[][] factor;
#else
    private float[] factor;
#endif
    ///<summary>Constructor for LU decomposition class. The constructor performs the factorization and the upper and
    ///lower matrices are accessible by the <c>U</c> and <c>L</c> properties.</summary>
    ///<param name="matrix">The matrix to factor.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    ///<exception cref="NotSquareMatrixException">matrix is not square.</exception>
    public FloatLUDecomp(IROFloatMatrix matrix)
    {
      if (matrix == null)
      {
        throw new System.ArgumentNullException("matrix cannot be null.");
      }

      if (matrix.Rows != matrix.Columns)
      {
        throw new NotSquareMatrixException("Matrix must be square.");
      }

      order = matrix.Columns;
      this.matrix = new FloatMatrix(matrix);
    }
    /// <summary>Performs the LU factorization.</summary>
    protected override void InternalCompute()
    {
#if MANAGED
      factor = new FloatMatrix(matrix).data;
      int m = matrix.RowLength;
      int n = matrix.ColumnLength;
      pivots = new int[m];
      for (int i = 0; i < m; i++)
      {
        pivots[i] = i;
      }
      sign = 1;
      float[] LUrowi;
      float[] LUcolj = new float[m];

      // Outer loop.
      for (int j = 0; j < n; j++)
      {
        // Make a copy of the j-th column to localize references.
        for (int i = 0; i < m; i++)
        {
          LUcolj[i] = factor[i][j];
        }

        // Apply previous transformations.
        for (int i = 0; i < m; i++)
        {
          LUrowi = factor[i];

          // Most of the time is spent in the following dot product.
          int kmax = System.Math.Min(i, j);
          float s = 0.0f;
          for (int k = 0; k < kmax; k++)
          {
            s += LUrowi[k] * LUcolj[k];
          }

          LUrowi[j] = LUcolj[i] -= s;
        }

        // Find pivot and exchange if necessary.
        int p = j;
        for (int i = j + 1; i < m; i++)
        {
          if (System.Math.Abs(LUcolj[i]) > System.Math.Abs(LUcolj[p]))
          {
            p = i;
          }
        }
        if (p != j)
        {
          for (int k = 0; k < n; k++)
          {
            float t = factor[p][k];
            factor[p][k] = factor[j][k];
            factor[j][k] = t;
          }
          int r = pivots[p];
          pivots[p] = pivots[j];
          pivots[j] = r;
          sign = -sign;
        }

        // Compute multipliers.

        if (j < m & factor[j][j] != 0.0)
        {
          for (int i = j + 1; i < m; i++)
          {
            factor[i][j] /= factor[j][j];
          }
        }
      }
#else
      factor = new float[matrix.data.Length];
      Array.Copy(matrix.data, factor, matrix.data.Length);
      Lapack.Getrf.Compute(order, order, factor, order, out pivots);
      GetSign();
#endif
      SetLU();

      this.singular = false;
      for (int j = 0; j < u.RowLength; j++)
      {
        if (u[j, j] == 0)
        {
          this.singular = true;
          break;
        }
      }
    }

    private void SetLU()
    {
      l = new FloatMatrix(order, order);
      u = new FloatMatrix(order, order);
      /* Finalize L and U */
#if MANAGED
      for (int i = 0; i < order; i++)
      {
        for (int j = 0; j < order; j++)
          if (i > j)
          {
            l.data[i][j] = factor[i][j];
          }
          else
          {
            u.data[i][j] = factor[i][j];
          }
        l.data[i][i] = 1.0f;
      }
#else
      for (int i=0; i<order; i++) {
        for (int j=0; j<order; j++)
          if (i > j) {
            l.data[j*order+i] = factor[j*order+i];
          } else {
            u.data[j*order+i] = factor[j*order+i];
          }
        l.data[i*order+i] = 1.0f;
      } 
#endif
    }

    private void GetSign()
    {
      sign = 1;
      for (int i = 0; i < pivots.Length; i++)
      {
        if (pivots[i] != i)
        {
          sign *= -1;
        }
      }
    }

    ///<summary>Return a value indicating whether the matrix is singular.</summary>
    ///<returns>true if the matrix is singular; otherwise, false.</returns>
    public bool IsSingular
    {
      get
      {
        Compute();
        return singular;
      }
    }

    ///<summary>Returns an array of <c>ints</c> indicating which rows were interchanged during factorization.
    ///Row i was interchanged with row pivots[i].</summary>
    ///<returns>array of <c>ints</c> indicating which rows were interchanged during factorization.</returns>
    public int[] GetPivots()
    {
      Compute();
      int[] ret = new int[pivots.Length];
      Array.Copy(pivots, ret, pivots.Length);
      return ret;
    }

    ///<summary>Returns the lower matrix.</summary>
    ///<returns>the lower matrix.</returns>
    public FloatMatrix L
    {
      get
      {
        Compute();
        return l;
      }
    }

    ///<summary>Returns the upper matrix.</summary>
    ///<returns>the upper matrix.</returns>
    public FloatMatrix U
    {
      get
      {
        Compute();
        return u;
      }
    }

    ///<summary>Calculates the determinant of the matrix.</summary>
    ///<returns>Returns the determinant of the matrix.</returns>
    public float GetDeterminant()
    {
      Compute();
      if (singular)
      {
        return 0;
      }
      else
      {
        float ret = 1.0f;
        for (int j = 0; j < order; j++)
        {
#if MANAGED
          ret *= factor[j][j];
#else
          ret *= factor[j*order+j];
#endif
        }
        return sign * ret;
      }
    }

    ///<summary>Calculates the inverse of the matrix.</summary>
    ///<returns>the inverse of the matrix.</returns>  
    ///<exception cref="SingularMatrixException">matrix is singular.</exception>
    public FloatMatrix GetInverse()
    {
      Compute();
      if (singular)
      {
        throw new SingularMatrixException();
      }
      else
      {
#if MANAGED
        FloatMatrix ret = FloatMatrix.CreateIdentity(order);
        ret = Solve(ret);
        return ret;
#else
        float[] inverse = new float[factor.Length];
        Array.Copy(factor,inverse,factor.Length);
        Lapack.Getri.Compute(order, inverse, order, pivots);
        FloatMatrix ret = new FloatMatrix(order,order);
        ret.data = inverse;
        return ret;
#endif
      }
    }

#if MANAGED
    private FloatMatrix Pivot(IROFloatMatrix B)
    {
      int m = B.Rows;
      int n = B.Columns;

      FloatMatrix ret = new FloatMatrix(m, n);
      for (int i = 0; i < pivots.Length; i++)
      {
        for (int j = 0; j < n; j++)
        {
          ret.data[i][j] = B[pivots[i],j];
        }
      }
      return ret;
    }

    private FloatVector Pivot(IROFloatVector B)
    {
      FloatVector ret = new FloatVector(B.Length);
      for (int i = 0; i < pivots.Length; i++)
      {
        ret.data[i] = B[pivots[i]];
      }
      return ret;
    }
#endif

    ///<summary>Solves a system on linear equations, AX=B, where A is the factored matrixed.</summary>
    ///<param name="B">RHS side of the system.</param>
    ///<returns>the solution matrix, X.</returns>  
    ///<exception cref="ArgumentNullException">B is null.</exception>
    ///<exception cref="SingularMatrixException">Ais singular.</exception>
    ///<exception cref="ArgumentException">The number of rows of A and B must be the same.</exception>
    public FloatMatrix Solve(IROFloatMatrix B)
    {
      if (B == null)
      {
        throw new System.ArgumentNullException("B cannot be null.");
      }
      Compute();
      if (singular)
      {
        throw new SingularMatrixException();
      }
      else
      {
        if (B.Rows != order)
        {
          throw new System.ArgumentException("Matrix row dimensions must agree.");
        }
#if MANAGED
        // Copy right hand side with pivoting
        int nx = B.Columns;
        FloatMatrix X = Pivot(B);

        // Solve L*Y = B(piv,:)
        for (int k = 0; k < order; k++)
        {
          for (int i = k + 1; i < order; i++)
          {
            for (int j = 0; j < nx; j++)
            {
              X.data[i][j] -= X.data[k][j] * factor[i][k];
            }
          }
        }
        // Solve U*X = Y;
        for (int k = order - 1; k >= 0; k--)
        {
          for (int j = 0; j < nx; j++)
          {
            X.data[k][j] /= factor[k][k];
          }
          for (int i = 0; i < k; i++)
          {
            for (int j = 0; j < nx; j++)
            {
              X.data[i][j] -= X.data[k][j] * factor[i][k];
            }
          }
        }
        return X;
#else
        float[] rhs = FloatMatrix.ToLinearArray(B);
        Lapack.Getrs.Compute(Lapack.Transpose.NoTrans,order,B.Columns,factor,order,pivots,rhs,B.Rows);
        FloatMatrix ret = new FloatMatrix(order,B.Columns);
        ret.data = rhs;
        return ret;
#endif
      }
    }

    ///<summary>Solves a system on linear equations, AX=B, where A is the factored matrixed.</summary>
    ///<param name="B">RHS side of the system.</param>
    ///<returns>the solution vector, X.</returns>  
    ///<exception cref="ArgumentNullException">B is null.</exception>
    ///<exception cref="SingularMatrixException">A is singular.</exception>
    ///<exception cref="ArgumentException">The number of rows of A and the length of B must be the same.</exception>
    public FloatVector Solve(IROFloatVector B)
    {
      if (B == null)
      {
        throw new System.ArgumentNullException("B cannot be null.");
      }
      Compute();
      if (singular)
      {
        throw new SingularMatrixException();
      }
      else
      {
        if (B.Length != order)
        {
          throw new System.ArgumentException("The length of B must be the same as the order of the matrix.");
        }
#if MANAGED
        // Copy right hand side with pivoting
        FloatVector X = Pivot(B);

        // Solve L*Y = B(piv,:)
        for (int k = 0; k < order; k++)
        {
          for (int i = k + 1; i < order; i++)
          {
            X[i] -= X[k] * factor[i][k];
          }
        }
        // Solve U*X = Y;
        for (int k = order - 1; k >= 0; k--)
        {
          X[k] /= factor[k][k];
          for (int i = 0; i < k; i++)
          {
            X[i] -= X[k] * factor[i][k];
          }
        }
        return X;
#else
        float[] rhs = FloatMatrix.ToLinearArray(B);
        Lapack.Getrs.Compute(Lapack.Transpose.NoTrans,order,1,factor,order,pivots,rhs,rhs.Length);
        return new FloatVector(rhs);
#endif
      }
    }
  }
}
