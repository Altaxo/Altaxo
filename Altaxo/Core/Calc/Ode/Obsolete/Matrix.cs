#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary> Matrix class.</summary>
  public class Matrix
  {
    /// <summary>Array for internal storage of elements. First index is row, second index is column</summary>
    private double[][] a;

    /// <summary>Row and column dimensions.</summary>
    private int m, n;

    #region Constructors

    /// <summary>Construct an m-by-n matrix of zeros. </summary>
    /// <param name="m">Number of rows.</param>
    /// <param name="n">Number of colums.</param>
    public Matrix(int m, int n)
    {
      this.m = m;
      this.n = n;
      a = new double[m][];
      for (int i = 0; i < m; i++)
      {
        a[i] = new double[n];
      }
    }

    /// <summary>Construct an m-by-n constant matrix.</summary>
    /// <param name="m">Number of rows.</param>
    /// <param name="n">Number of colums.</param>
    /// <param name="s">Fill the matrix with this scalar value.</param>
    public Matrix(int m, int n, double s)
    {
      this.m = m;
      this.n = n;
      a = new double[m][];
      for (int i = 0; i < m; i++)
      {
        a[i] = new double[n];
      }
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = s;
        }
      }
    }

    /// <summary>Construct a matrix from a 2-D jagged array.</summary>
    /// <param name="A">Two-dimensional array of doubles.</param>
    /// <exception cref="ArgumentException">All rows must have the same length</exception>
    /// <remarks>Array is not copied</remarks>
    public Matrix(double[][] A)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      m = A.Length;
      n = A[0].Length;
      for (int i = 0; i < m; i++)
      {
        if (A[i].Length != n)
        {
          throw new System.ArgumentException("All rows must have the same length.");
        }
      }
      a = A;
    }

    /// <summary>Construct a matrix from a copy of a 2-D array.</summary>
    /// <param name="arr">Two-dimensional array of doubles. First index is row, second is column</param>
    public Matrix(double[,] arr)
    {
      if (arr is null)
        throw new ArgumentNullException(nameof(arr));
      m = arr.GetLength(0);
      n = arr.GetLength(1);
      a = new double[m][];
      for (int i = 0; i < m; i++)
      {
        a[i] = new double[n];
        for (int j = 0; j < n; j++)
        {
          a[i][j] = arr[i, j];
        }
      }
    }

    /// <summary>Construct a matrix quickly without checking arguments.</summary>
    /// <param name="A">Two-dimensional array of doubles.</param>
    /// <param name="m">Number of rows.</param>
    /// <param name="n">Number of columns.</param>
    public Matrix(double[][] A, int m, int n)
    {
      a = A;
      this.m = m;
      this.n = n;
    }

    #endregion Constructors

    #region Getters, setters and accessors

    /// <summary>Get row dimension.</summary>
    /// <returns>The number of rows.</returns>
    public int RowDimension
    {
      get
      {
        return m;
      }
    }

    /// <summary>Get column dimension.</summary>
    /// <returns>The number of columns.</returns>
    public int ColumnDimension
    {
      get
      {
        return n;
      }
    }

    /// <summary>Explicit conversion to CLR 2-D array with copy</summary>
    /// <param name="a">Matrix to convert</param>
    /// <returns>2D array</returns>
    public static explicit operator double[,](Matrix a)
    {
      if (a is null)
        throw new ArgumentNullException("A");
      double[,] X = new double[a.m, a.n];
      for (int i = 0; i < a.m; i++)
      {
        for (int j = 0; j < a.n; j++)
        {
          X[i, j] = a.a[i][j];
        }
      }
      return X;
    }

    /// <summary>Explicit conversion to CLR jagged array without making copy</summary>
    /// <param name="a">Matrix to convert</param>
    /// <returns>Jagged array</returns>
    public static explicit operator double[][](Matrix a)
    {
      if (a is null)
        throw new ArgumentNullException("A");
      return a.a;
    }

    /// <summary>Make a deep copy of a matrix</summary>
    public Matrix Clone()
    {
      var X = new Matrix(m, n);
      double[][] C = X.a;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = a[i][j];
        }
      }
      return X;
    }

    /// <summary>
    /// Gets column with number coumnnum from matrix res
    /// </summary>
    /// <param name="columnNum">Column number (zero based)</param>
    /// <returns>Vector containing copy of column's elements</returns>
    public Vector CloneColumn(int columnNum)
    {
      if (0 > columnNum || columnNum > ColumnDimension - 1)
        throw new IndexOutOfRangeException("Column index is out of range");
      var v = Vector.Zeros(RowDimension);
      for (int i = 0; i < RowDimension; i++)
      {
        v[i] = a[i][columnNum];
      }
      return v;
    }

    /// <summary>Matrix elements accessor.</summary>
    /// <param name="i">Row index.</param>
    /// <param name="j">Column index.</param>
    /// <returns>A(i,j)</returns>
    public double this[int i, int j]
    {
      get { return a[i][j]; }
      set { a[i][j] = value; }
    }

    /// <summary>Access the Column</summary>
    /// <param name="i">Row index</param>
    /// <returns>A(i)</returns>
    public double[] this[int i]
    {
      get { return a[i]; }
      set { a[i] = value; }
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="i0">Initial row index </param>
    /// <param name="i1">Final row index </param>
    /// <param name="j0">Initial column index </param>
    /// <param name="j1">Final column index </param>
    /// <returns>A(i0:i1,j0:j1) </returns>
    /// <exception cref="IndexOutOfRangeException">Submatrix indices </exception>
    public Matrix Submatrix(int i0, int i1, int j0, int j1)
    {
      var X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
      double[][] B = X.a;
      try
      {
        for (int i = i0; i <= i1; i++)
        {
          for (int j = j0; j <= j1; j++)
          {
            B[i - i0][j - j0] = a[i][j];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="r">Array of row indices.</param>
    /// <param name="c">Array of column indices.</param>
    /// <returns>A(r(:),c(:)) </returns>
    /// <exception cref="IndexOutOfRangeException">Submatrix indices</exception>
    public Matrix Submatrix(int[] r, int[] c)
    {
      if (r is null)
        throw new ArgumentNullException("r");
      if (c is null)
        throw new ArgumentNullException("c");
      var X = new Matrix(r.Length, c.Length);
      double[][] B = X.a;
      try
      {
        for (int i = 0; i < r.Length; i++)
        {
          for (int j = 0; j < c.Length; j++)
          {
            B[i][j] = a[r[i]][c[j]];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="i0">Initial row index</param>
    /// <param name="i1">Final row index</param>
    /// <param name="c">Array of column indices.</param>
    /// <returns>A(i0:i1,c(:))</returns>
    /// <exception cref="IndexOutOfRangeException">Submatrix indices</exception>
    public Matrix Submatrix(int i0, int i1, int[] c)
    {
      if (c is null)
        throw new ArgumentNullException("c");
      var X = new Matrix(i1 - i0 + 1, c.Length);
      double[][] B = X.a;
      try
      {
        for (int i = i0; i <= i1; i++)
        {
          for (int j = 0; j < c.Length; j++)
          {
            B[i - i0][j] = a[i][c[j]];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="r">   Array of row indices.</param>
    /// <param name="j0">  Initial column index.</param>
    /// <param name="j1">  Final column index.</param>
    /// <returns>     A(r(:),j0:j1)</returns>
    /// <exception cref="IndexOutOfRangeException">Submatrix indices</exception>
    public Matrix Submatrix(int[] r, int j0, int j1)
    {
      if (r is null)
        throw new ArgumentNullException("r");
      var X = new Matrix(r.Length, j1 - j0 + 1);
      double[][] B = X.a;
      try
      {
        for (int i = 0; i < r.Length; i++)
        {
          for (int j = j0; j <= j1; j++)
          {
            B[i][j - j0] = a[r[i]][j];
          }
        }
      }
      catch (System.IndexOutOfRangeException e)
      {
        throw new System.IndexOutOfRangeException("Submatrix indices", e);
      }
      return X;
    }

    #endregion Getters, setters and accessors

    #region Arithmetic operators

    public static Matrix operator +(Matrix A, Matrix B)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if (B is null)
        throw new ArgumentNullException("B");
      A.CheckMatrixDimensions(B);
      var X = new Matrix(A.m, A.n);
      double[][] C = X.a;
      for (int i = 0; i < A.m; i++)
      {
        for (int j = 0; j < A.n; j++)
        {
          C[i][j] = A.a[i][j] + B.a[i][j];
        }
      }
      return X;
    }

    public static Matrix operator -(Matrix A, Matrix B)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      if (B is null)
        throw new ArgumentNullException("B");
      A.CheckMatrixDimensions(B);
      var X = new Matrix(A.m, A.n);
      double[][] C = X.a;
      for (int i = 0; i < A.m; i++)
      {
        for (int j = 0; j < A.n; j++)
        {
          C[i][j] = A.a[i][j] - B.a[i][j];
        }
      }
      return X;
    }

    /// <summary>Multiply a matrix by a scalar, C = s*A</summary>
    /// <param name="s">   scalar</param>
    /// <returns>     s*A</returns>
    public Matrix times(double s)
    {
      var X = new Matrix(m, n);
      double[][] C = X.a;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          C[i][j] = s * a[i][j];
        }
      }
      return X;
    }

    public static Matrix operator *(Matrix A, double s)
    {
      if (A is null)
        throw new ArgumentNullException("A");
      var X = new Matrix(A.m, A.n);
      double[][] C = X.a;
      for (int i = 0; i < A.m; i++)
      {
        for (int j = 0; j < A.n; j++)
        {
          C[i][j] = s * A.a[i][j];
        }
      }
      return X;
    }

    public static Matrix operator *(double s, Matrix A)
    {
      return A * s;
    }

    /// <summary>Multiply a matrix by a scalar in place, A = s*A</summary>
    /// <param name="s">   scalar</param>
    /// <returns>     replace A by s*A</returns>
    public Matrix Mul(double s)
    {
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          a[i][j] = s * a[i][j];
        }
      }
      return this;
    }

    public static Matrix operator *(Matrix A, Matrix B)
    {
      if (B is null)
        throw new ArgumentNullException("B");
      if (B.m != A.n)
      {
        throw new System.ArgumentException("Matrix inner dimensions must agree.");
      }
      var X = new Matrix(A.m, B.n);
      double[][] C = X.a;
      double[] Bcolj = new double[A.n];
      for (int j = 0; j < B.n; j++)
      {
        for (int k = 0; k < A.n; k++)
        {
          Bcolj[k] = B.a[k][j];
        }
        for (int i = 0; i < A.m; i++)
        {
          double[] Arowi = A.a[i];
          double s = 0;
          for (int k = 0; k < A.n; k++)
          {
            s += Arowi[k] * Bcolj[k];
          }
          C[i][j] = s;
        }
      }
      return X;
    }

    #endregion Arithmetic operators

    #region Matrix operations

    /// <summary>Solve A*x = b using Gaussian elimination with partial pivoting</summary>
    /// <param name="b">    right hand side Vector</param>
    /// <returns>    The solution x = A^(-1) * b as a Vector</returns>
    public Vector SolveGE(Vector b)
    {
      return Gauss.Solve(this, b);
    }

    /// <summary>Generate identity matrix</summary>
    /// <param name="m">   Number of rows.</param>
    /// <param name="n">   Number of colums.</param>
    /// <returns>     An m-by-n matrix with ones on the diagonal and zeros elsewhere.</returns>
    public static Matrix Identity(int m, int n)
    {
      var A = new Matrix(m, n);
      double[][] X = A.a;
      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < n; j++)
        {
          X[i][j] = (i == j ? 1.0 : 0.0);
        }
      }
      return A;
    }

    /// <summary>Transpose of a dense matrix</summary>
    /// <returns>  An n-by-m matrix where the input matrix has m rows and n columns.</returns>
    public Matrix Transpose()
    {
      var result = new Matrix(n, m);
      double[][] T = result.a;
      for (int i = 0; i < m; i++)
        for (int j = 0; j < n; j++)
          T[j][i] = a[i][j];
      return result;
    }

    /// <summary>Cholesky factorization</summary>
    /// <returns>Lower-triangular Cholesky factor for a symmetric positive-definite matrix</returns>
    public Matrix Cholesky()
    {
      var result = new Matrix(m, m);
      var Li = result.a;

      // Main loop
      for (int i = 0; i < n; i++)
      {
        var Lrowi = Li[i];
        for (int j = 0; j < (i + 1); j++)
        {
          var Lrowj = Li[j];
          double s = 0;
          for (int k = 0; k < j; k++)
            s += Lrowi[k] * Lrowj[k];
          if (i == j)
            Lrowi[j] = Math.Sqrt(a[i][i] - s);
          else
            Lrowi[j] = (a[i][j] - s) / Lrowj[j];
        }
      }

      return result;
    }

    /// <summary>Matrix inverse for a lower triangular matrix</summary>
    /// <returns></returns>
    public Matrix InverseLower()
    {
      int n = ColumnDimension;
      var I = Matrix.Identity(n, n);
      var invLtr = new double[n][];
      for (int col = 0; col < n; col++)
      {
        var x = Vector.Zeros(n);
        x[col] = 1;
        invLtr[col] = SolveLower(x);
      }
      var invL = new Matrix(invLtr).Transpose();

      return invL;
    }

    public Vector SolveLower(Vector b)
    {
      double[] x = new double[m];

      for (int i = 0; i < m; i++)
      {
        x[i] = b[i];
        for (int j = 0; j < i; j++)
          x[i] -= a[i][j] * x[j];
        x[i] /= a[i][i];
      }

      return new Vector(x);
    }

    public Vector SolveUpper(Vector b)
    {
      double[] x = new double[m];

      for (int i = m - 1; i >= 0; i--)
      {
        x[i] = b[i];
        for (int j = i + 1; j < n; j++)
          x[i] -= a[i][j] * x[j];
        x[i] /= a[i][i];
      }

      return new Vector(x);
    }

    /// <summary>Check if size(A) == size(B) and throws exception if not</summary>
    private void CheckMatrixDimensions(Matrix B)
    {
      if (B.m != m || B.n != n)
      {
        throw new System.ArgumentException("Matrix dimensions must agree.");
      }
    }

    #endregion Matrix operations
  }
}
