#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
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

/*
** ComplexComplexDoubleSymmetricLevinson.cs
**
** Class that implements UDL factorisation of the inverse of a symmetric square
** Toeplitz matrix using the Levinson Algorithm.
**
** Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

#region Using directives

using System;


#endregion Using directives

namespace Altaxo.Calc.LinearAlgebra
{

  /// <summary>
  /// A Levinson solver for symmetric square Toeplitz systems of <c>Complex</c> type.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// This class provides members for inverting a symmetric square Toeplitz matrix
  /// (see <see cref="GetInverse"/> member), calculating the determinant of the matrix
  /// (see <see cref="GetDeterminant"/> member) and solving linear systems associated
  /// with the matrix (see <see cref="Solve"/> members).
  /// <para>
  /// The class implements a <B>UDL</B> decomposition of the inverse of the Toeplitz matrix.
  /// The decomposition is based upon Levinson's algorithm. As a consequence, all operations
  /// require approximately <B>N</B> squared FLOPS, where <B>N</B> is the matrix order. This
  /// is significantly faster than Cholesky's factorization for symmetric matrices, which
  /// requires <B>N</B> cubed FLOPS.
  /// </para>
  /// <para>
  /// A requirement of Levinson's algorithm is that all the leading sub-matrices and the principal
  /// matrix must be non-singular. During the decomposition, sub-matrices and the principal matrix are
  /// checked to ensure that they are non-singular. When a singular matrix is found, the decomposition
  /// is halted and an internal flag is set. The <see cref="IsSingular"/> property may be used to access
  /// the flag, to determine if any singular matrices were detected.
  /// </para>
  /// <para>
  /// A outline of this approach to the UDL decomposition of inverse Toeplitz matrices is
  /// found in the following reference:
  /// </para>
  /// <para>
  /// <EM>Sun-Yuan Kung and Yu Hen Hu</EM>, A Highly Concurrent Algorithm and Pipelined
  /// Architecture for Solving Toeplitz Systems, IEEE Transactions on Acoustics,
  /// Speech and Signal Processing, Volume ASSP-31, Number 1, Febuary 1983, pages
  /// 66 - 75.
  /// </para>
  /// </remarks>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  /// <example>
  /// The following simple example illustrates the use of the class:
  /// <para>
  /// <code escaped="true">
  /// using System;
  /// using dnA.Exceptions;
  /// using dnA.Math;
  /// using System.IO;
  ///
  /// namespace Example_5
  /// {
  ///
  ///   class Application
  ///   {
  ///
  ///     // application entry point
  ///     [STAThread]
  ///     public static void Main(string[] args)
  ///     {
  ///
  ///       ComplexDoubleVector cdv = new ComplexDoubleVector(4);
  ///       cdv[0] = new Complex(4.0, 0.0);
  ///       cdv[1] = new Complex(12.0, -4.0/3.0);
  ///       cdv[2] = new Complex(80.0/3.0, 64.0/3.0);
  ///       cdv[3] = new Complex(48.0, -16.0/3.0);
  ///
  ///       // create Levinson solver
  ///       ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(cdv);
  ///
  ///       // display the Toeplitz matrix
  ///       ComplexDoubleMatrix T = cdsl.GetMatrix();
  ///       Console.WriteLine("Matrix:: {0} ", T.ToString("E3"));
  ///       Console.WriteLine();
  ///
  ///       // check if matrix is singular
  ///       Console.WriteLine("Singular:          {0}", cdsl.IsSingular);
  ///
  ///       // get the determinant
  ///       Console.WriteLine("Determinant:       {0}", cdsl.GetDeterminant().ToString("E3"));
  ///       Console.WriteLine();
  ///
  ///       // get the inverse
  ///       ComplexDoubleMatrix Inv = cdsl.GetInverse();
  ///       Console.WriteLine("Inverse:: {0} ", Inv.ToString("E3"));
  ///       Console.WriteLine();
  ///
  ///       // solve a linear system
  ///       ComplexDoubleVector Y = new ComplexDoubleVector(4);
  ///       Y[0] = new Complex(1036.0/3.0, -212.0);
  ///       Y[1] = new Complex(728.0/3.0, -200.0/3.0);
  ///       Y[2] = new Complex(388.0/3.0, -148.0/3.0);
  ///       Y[3] = new Complex(304.0/3.0, -40.0/3.0);
  ///
  ///       ComplexDoubleVector X = cdsl.Solve(Y);
  ///       Console.WriteLine("X:: {0} ", X.ToString("E3"));
  ///       Console.WriteLine();
  ///
  ///     }
  ///
  ///   }
  ///
  /// }
  /// </code>
  /// </para>
  /// <para>
  /// The application generates the following results:
  /// </para>
  /// <para>
  /// <code escaped="true">
  /// Matrix:: rows: 4, cols: 4
  /// 4.000E+000 + 0.000E+000i, 1.200E+001 -1.333E+000i, 2.667E+001 + 2.133E+001i, 4.800E+001 -5.333E+000i
  /// 1.200E+001 -1.333E+000i, 4.000E+000 + 0.000E+000i, 1.200E+001 -1.333E+000i, 2.667E+001 + 2.133E+001i
  /// 2.667E+001 + 2.133E+001i, 1.200E+001 -1.333E+000i, 4.000E+000 + 0.000E+000i, 1.200E+001 -1.333E+000i
  /// 4.800E+001 -5.333E+000i, 2.667E+001 + 2.133E+001i, 1.200E+001 -1.333E+000i, 4.000E+000 + 0.000E+000i
  ///
  /// Singular:          False
  /// Determinant:       -1.478E+006 - 8.879E+005i
  ///
  /// Inverse:: rows: 4, cols: 4
  /// -3.427E-003 + 2.015E-003i, 3.534E-003 + 2.723E-003i, 1.145E-002 - 2.307E-002i, 2.735E-003 + 7.280E-003i
  /// 3.534E-003 + 2.723E-003i, -1.702E-002 + 2.724E-005i, 1.534E-002 + 3.027E-002i, 1.145E-002 - 2.307E-002i
  /// 1.145E-002 -2.307E-002i, 1.534E-002 + 3.027E-002i, -1.702E-002 + 2.724E-005i, 3.534E-003 + 2.723E-003i
  /// 2.735E-003 + 7.280E-003i, 1.145E-002 - 2.307E-002i, 3.534E-003 + 2.723E-003i, -3.427E-003 + 2.015E-003i
  ///
  /// X:: Length: 4
  /// 1.000E+000 - 1.000E+000i, 2.000E+000 + 2.000E+000i, 3.000E+000 - 3.000E+000i, 4.000E+000 - 4.000E+000i
  /// </code>
  /// </para>
  /// </example>
  sealed public class ComplexDoubleSymmetricLevinson : Algorithm
  {

    #region Fields

    /// <summary>
    /// The left-most column of the Toeplitz matrix.
    /// </summary>
    private readonly ComplexDoubleVector m_LeftColumn;

    /// <summary>
    /// The system order.
    /// </summary>
    private readonly int m_Order;

    /// <summary>
    /// lower triangular matrix.
    /// </summary>
    private Complex[][] m_LowerTriangle;

    /// <summary>
    /// The diagonal vector.
    /// </summary>
    private Complex[] m_Diagonal;

    /// <summary>
    /// Flag to indicate if system is singular.
    /// </summary>
    private bool m_IsSingular;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Get the order of the Toeplitz matrix.
    /// </summary>
    public int Order
    {
      get
      {
        return m_Order;
      }
    }

    /// <summary>
    /// Check if the Toeplitz matrix or any leading sub-matrices are singular.
    /// </summary>
    /// <remarks>
    /// If the Toeplitz matrix or any leading sub-matrices are singular, it is
    /// not possible to complete the <b>UDL</b> decomposition using the Levinson
    /// algorithm.
    /// </remarks>
    public bool IsSingular
    {
      get
      {
        // make sure the factorisation is completed
        Compute();
        // return the flag value
        return m_IsSingular;
      }
    }

    ///<summary>
    /// Get the lower triangle matrix of the UDL factorisation.
    /// </summary>
    /// <remarks>
    /// It is recommended that the <see cref="IsSingular"/> property
    /// be checked to see if the decomposition was completed, before attempting
    /// to obtain the lower triangle matrix.
    /// </remarks>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    public ComplexDoubleMatrix L
    {
      get
      {
        // make sure the factorisation is completed
        Compute();

        // check if there was a result
        if (m_IsSingular == true)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }

        // copy jagged array into a ComplexDoubleMatrix
        ComplexDoubleMatrix Lower = new ComplexDoubleMatrix(m_Order);
        for (int i = 0; i < m_Order; i++ )
        {
#if MANAGED
          // managed implementation
          m_LowerTriangle[i].CopyTo(Lower.data[i], 0);
#else
          // native implementation
          for( int j = 0; j < i+1; j++ )
          {
            Lower[i, j] = m_LowerTriangle[i][j];
          }
#endif
        }

        return Lower;
      }
    }

    ///<summary>
    /// Get the diagonal matrix of the UDL factorisation.</summary>
    /// <remarks>
    /// It is recommended that the <see cref="IsSingular"/> property
    /// be checked to see if the decomposition was completed, before attempting
    /// to obtain the diagonal matrix.
    /// </remarks>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    public ComplexDoubleMatrix D
    {
      get
      {
        // make sure the factorisation is completed
        Compute();

        // check if there was a result
        if (m_IsSingular == true)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }

        // copy diagonal vector into a ComplexDoubleMatrix
        ComplexDoubleMatrix Diagonal = new ComplexDoubleMatrix(m_Order);
        for (int i = 0; i < m_Order; i++ )
        {
#if MANAGED
          // managed implementation
          Diagonal.data[i][i] = m_Diagonal[i];
#else
          // native implementation
          Diagonal.data[i*m_Order+i] = m_Diagonal[i];
#endif
        }

        return Diagonal;
      }
    }

    ///<summary>
    /// Get the upper triangle matrix of the UDL factorisation.</summary>
    /// <remarks>
    /// It is recommended that the <see cref="IsSingular"/> property
    /// be checked to see if the decomposition was completed, before attempting
    /// to obtain the upper triangle matrix.
    /// </remarks>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    public ComplexDoubleMatrix U
    {
      get
      {
        // make sure the factorisation is completed
        Compute();

        // check if there was a result
        if (m_IsSingular == true)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }

        // copy jagged array into a ComplexDoubleMatrix and then calculate the transpose
        ComplexDoubleMatrix Upper = this.L.GetTranspose();

        return Upper;
      }
    }

    #endregion Properties

    #region Constructors

    /// <overloads>
    /// There are two permuations of the constructor, both require a parameter corresponding
    /// to the left-most column of a Toeplitz matrix.
    /// </overloads>
    /// <summary>
    /// Constructor with <c>ComplexDoubleVector</c> parameter.
    /// </summary>
    /// <param name="T">The left-most column of the Toeplitz matrix.</param>
    /// <exception cref="ArgumentNullException">
    /// <B>T</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>T</B> is zero.
    /// </exception>
    public ComplexDoubleSymmetricLevinson(IROComplexDoubleVector T)
    {
      // check parameter
      if (T == null)
      {
        throw new System.ArgumentNullException("T");
      }
      else if (T.Length == 0)
      {
        throw new RankException("The length of T is zero.");
      }

      // save the vector
      m_LeftColumn = new ComplexDoubleVector(T);
      m_Order = m_LeftColumn.Length;

      // allocate memory for lower triangular matrix
      m_LowerTriangle = new Complex[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_LowerTriangle[i] = new Complex[i+1];
      }

      // allocate memory for diagonal
      m_Diagonal = new Complex[m_Order];

    }

    #endregion Constructors

    #region Protected Members

    /// <summary>
    /// Calculate the UDL decomposition of the inverse Toeplitz matrix.
    /// </summary>
    /// <remarks>
    /// The member checks each iteration for a singular principal sub-matrix.
    /// If a singular sub-matrix is detected, the private boolean <EM>m_IsSingular</EM>
    /// is set to <B>true</B>, the <EM>m_IsPositiveDefinite</EM>EM> flag is set <B>false</B> and the
    /// decomposition halted.
    /// <para>
    /// When the decomposition is completed, the lower triangular matrix is stored
    /// in the jagged array <EM>m_LowerTriangle</EM>, the diagonal elements
    /// in the array <EM>m_Diagonal</EM> and the private boolean <EM>m_IsSingular</EM>
    /// is set to <B>false</B>.
    /// </para>
    /// </remarks>
    protected override void InternalCompute()
    {
      int i, j, l;      // index/loop variables
      Complex Inner;      // inner product
      Complex K;        // reflection coefficient
      Complex[] B;        // reference to previous order coefficients
      Complex[] A;        // reference to current order coefficients


      // check if principal diagonal is zero
      if (m_LeftColumn[0] == Complex.Zero)
      {
        m_IsSingular = true;
        return;
      }

      // setup zero order solution
      B = m_LowerTriangle[0];
      B[0] = Complex.One;
      m_Diagonal[0] = Complex.One / m_LeftColumn[0];

      // solve systems of increasing order
      for (i = 1; i < m_Order; i++, B = A)
      {
        // calculate inner product
        Inner = Complex.Zero;
        for ( j = 0, l = 1; j < i; j++, l++ )
        {
          Inner += m_LeftColumn[l] * B[j];
        }

        // calculate the reflection coefficient
        K = -Inner * m_Diagonal[i-1];

        // get the current low triangle row
        A = m_LowerTriangle[i];

        // update low triangle elements
        A[0] = Complex.Zero;
        Array.Copy(B, 0, A, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          A[j] += K * B[l];
        }

        // check if singular sub-matrix
        if (K*K == Complex.One)
        {
          m_IsSingular = true;
          return;
        }

        // update diagonal
        m_Diagonal[i] = m_Diagonal[i - 1]/(Complex.One - K * K);

      }

      // solution completed
      m_IsSingular = false;
      return;
    }

    #endregion Protected Members

    #region Public Methods

    /// <summary>
    /// Get a vector that represents the left-most column of the Toplitz matrix.
    /// </summary>
    public ComplexDoubleVector GetVector()
    {
      return new ComplexDoubleVector(m_LeftColumn);
    }

    /// <summary>
    /// Get a copy of the Toeplitz matrix.
    /// </summary>
    public ComplexDoubleMatrix GetMatrix()
    {
      int i, j;

      // allocate memory for the matrix
      ComplexDoubleMatrix tm = new ComplexDoubleMatrix(m_Order);

#if MANAGED
      // fill top row
      Complex[] top = tm.data[0];
      Array.Copy(m_LeftColumn.data, 0, top, 0, m_Order);

      if (m_Order > 1)
      {
        // fill bottom row (reverse order)
        Complex[] bottom = tm.data[m_Order - 1];

        for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
        {
          bottom[i] = m_LeftColumn[j];
        }

        // fill rows in-between
        for (i = 1, j = m_Order - 1 ; j > 1; i++)
        {
          Array.Copy(top, 0, tm.data[i], i, j--);
          Array.Copy(bottom, j, tm.data[i], 0, i);
        }
      }
#else
      if (m_Order > 1)
      {
        Complex[] top = new Complex[m_Order];
        Array.Copy(m_LeftColumn.data, 0, top, 0, m_Order);
        tm.SetRow(0, top);

        // fill bottom row (reverse order)
        Complex[] bottom = new Complex[m_Order];

        for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
        {
          bottom[i] = m_LeftColumn[j];
        }

        // fill rows in-between
        for (i = 1, j = m_Order - 1 ; j > 0; i++)
        {
          Complex[] temp = new Complex[m_Order];
          Array.Copy(top, 0, temp, i, j--);
          Array.Copy(bottom, j, temp, 0, i);
          tm.SetRow(i, temp);
        }
      }
      else
      {
        Array.Copy(m_LeftColumn.data, 0, tm.data, 0, m_Order);
      }
#endif

      return tm;
    }

    /// <summary>
    /// Get the determinant of the Toeplitz matrix.
    /// </summary>
    /// <returns>The determinant.</returns>
    /// <remarks>
    /// It is recommended that the <see cref="IsSingular"/> property
    /// be checked to see if the decomposition was completed, before attempting
    /// to obtain the determinant.
    /// </remarks>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    public Complex GetDeterminant()
    {
      Complex Determinant;

      // make sure the factorisation is completed
      Compute();

      // if any of the matrices is singular give up
      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
      }
      else
      {
        Determinant = Complex.One;
        for (int i = 0; i < m_Order; i++)
        {
          Determinant *= m_Diagonal[i];
        }
        Determinant = Complex.One / Determinant;
      }

      return Determinant;
    }

    /// <overloads>
    /// Solve a symmetric square Toeplitz system.
    /// </overloads>
    /// <summary>
    /// Solve a symmetric square Toeplitz system with a right-side vector.
    /// </summary>
    /// <param name="Y">The right-hand side of the system.</param>
    /// <returns>The solution vector.</returns>
    /// <exception cref="ArgumentNullException">
    /// Parameter <B>Y</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>Y</B> is not equal to the number of rows in the Toeplitz matrix.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This member solves the linear system <B>TX</B> = <B>Y</B>, where <B>T</B> is
    /// the symmetric square Toeplitz matrix, <B>X</B> is the unknown solution vector
    /// and <B>Y</B> is a known vector.
    /// <para>
    /// The class implicitly decomposes the inverse Toeplitz matrix into a <b>UDL</b> factorisation
    /// using the Levinson algorithm, and then calculates the solution vector.
    /// </para>
    /// </remarks>
    public ComplexDoubleVector Solve(IROComplexDoubleVector Y)
    {
      ComplexDoubleVector X;

      // check parameters
      if (Y == null)
      {
        throw new System.ArgumentNullException("Y");
      }
      else if (m_Order != Y.Length)
      {
        throw new RankException("The length of Y is not equal to the number of rows in the Toeplitz matrix.");
      }

      Compute();

      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
      }

      int i, j, l;      // index/loop variables
      Complex Inner;      // inner product
      Complex G;        // scaling constant
      Complex[] A;        // reference to current order coefficients

      // allocate memory for solution
      X = new ComplexDoubleVector(m_Order);

      // setup zero order solution
      X[0] = Y[0] / m_LeftColumn[0];

      // solve systems of increasing order
      for (i = 1; i < m_Order; i++)
      {
        // calculate inner product
        Inner = Y[i];
        for (j = 0, l = i; j < i; j++, l--)
        {
          Inner -= X[j] * m_LeftColumn[l];
        }
        // get the current predictor coefficients row
        A = m_LowerTriangle[i];

        // update the solution vector
        G = Inner * m_Diagonal[i];
        for (j = 0; j <= i; j++)
        {
          X[j] += G * A[j];
        }

      }

      return X;
    }

    /// <summary>
    /// Solve a symmetric square Toeplitz system with a right-side matrix.
    /// </summary>
    /// <param name="Y">The right-hand side of the system.</param>
    /// <returns>The solution matrix.</returns>
    /// <exception cref="ArgumentNullException">
    /// Parameter <B>Y</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The number of rows in <B>Y</B> is not equal to the number of rows in the Toeplitz matrix.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This member solves the linear system <B>TX</B> = <B>Y</B>, where <B>T</B> is
    /// a symmetric square Toeplitz matrix, <B>X</B> is the unknown solution matrix
    /// and <B>Y</B> is a known matrix.
    /// <para>
    /// The class implicitly decomposes the inverse Toeplitz matrix into a <b>UDL</b> factorisation
    /// using the Levinson algorithm, and then calculates the solution matrix.
    /// </para>
    /// </remarks>
    public ComplexDoubleMatrix Solve(IROComplexDoubleMatrix Y)
    {
      ComplexDoubleMatrix X;

      // check parameters
      if (Y == null)
      {
        throw new System.ArgumentNullException("Y");
      }
      else if (m_Order != Y.Columns)
      {
        throw new RankException("The numer of rows in Y is not equal to the number of rows in the Toeplitz matrix.");
      }

      Compute();

      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
      }

      int M = Y.Rows;
      int i, j, l, m;     // index/loop variables
      Complex[] Inner;      // inner product
      Complex[] G;        // scaling constant
      Complex[] A;        // reference to current order coefficients
      Complex scalar;

      // allocate memory for solution
      X = new ComplexDoubleMatrix(m_Order, M);
      Inner = new Complex[M];
      G = new Complex[M];

      // setup zero order solution
      scalar = Complex.One / m_LeftColumn[0];
      for (m = 0; m < M; m++)
      {
#if MANAGED
        X.data[0][m] = scalar * Y[0,m];
#else

        X.data[m*m_Order] = scalar * Y[0,m];
#endif
      }

      // solve systems of increasing order
      for (i = 1; i < m_Order; i++)
      {
        // calculate inner product
        for (m = 0; m < M; m++)
        {
#if MANAGED
          Inner[m] = Y[i,m];
#else
          Inner[m] = Y[i,m];
#endif
        }

        for (j = 0, l = i; j < i; j++, l--)
        {
          scalar = m_LeftColumn[l];
          for (m = 0; m < M; m++)
          {
#if MANAGED
            Inner[m] -= scalar * X.data[j][m];
#else
            Inner[m] -= scalar * X.data[m*m_Order+j];
#endif
          }
        }

        // get the current predictor coefficients row
        A = m_LowerTriangle[i];

        // update the solution matrix
        for (m = 0; m < M; m++)
        {
          G[m] = m_Diagonal[i] * Inner[m];
        }
        for (j = 0; j <= i; j++)
        {
          scalar = A[j];
          for (m = 0; m < M; m++)
          {
#if MANAGED
            X.data[j][m] += scalar * G[m];
#else
            X.data[m*m_Order+j] += scalar * G[m];
#endif
          }
        }
      }

      return X;
    }

    /// <summary>
    /// Get the inverse of the Toeplitz matrix.
    /// </summary>
    /// <returns>The inverse matrix.</returns>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// The class implicitly decomposes the inverse Toeplitz matrix into a <b>UDL</b> factorisation
    /// using the Levinson algorithm, before using Trench's algorithm to complete
    /// the calculation of the inverse.
    /// <para>
    /// Trench's algorithm requires approximately <b>N</b> squared FLOPS, compared to <b>N</b> cubed FLOPS
    /// if we simply multiplied the <b>UDL</b> factors (<b>N</b> is the matrix order).
    /// </para>
    /// </remarks>
    public ComplexDoubleMatrix GetInverse()
    {
      Compute();

      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
      }

      ComplexDoubleMatrix I = new ComplexDoubleMatrix(m_Order);           // the solution matrix
      Complex[] A = m_LowerTriangle[m_Order-1];
      Complex A1, A2, scale;

#if MANAGED

      Complex[] current, previous;                    // references to rows in the solution
      int i, j, k, l;

      // setup the first row in wedge
      scale = m_Diagonal[m_Order-1];
      current = I.data[0];
      for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
      {
        current[i] = scale* A[j];
      }

      // calculate values in the rest of the wedge
      for (i = 1; i < (1 + m_Order) / 2; i++)
      {
        previous = current;
        current = I.data[i];
        A1 = A[m_Order - i - 1];
        A2 = A[i - 1];
        for (j = i, k = i - 1, l = m_Order - i - 1; j < m_Order - i; j++, k++, l--)
        {
          current[j] = previous[k] + scale * (A1 * A[l] - A2 * A[k]);
        }
      }

#else

      int i, j, k, l;

      // setup the first row in wedge
      scale = m_Diagonal[m_Order-1];
      for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
      {
        I[0, i] = scale* A[j];
      }

      // calculate values in the rest of the wedge
      for (i = 1; i < (1 + m_Order) / 2; i++)
      {
        A1 = A[m_Order - i - 1];
        A2 = A[i - 1];
        for (j = i, k = i - 1, l = m_Order - i - 1; j < m_Order - i; j++, k++, l--)
        {
          I[i, j] = I[i - 1, k] + scale * (A1 * A[l] - A2 * A[k]);
        }
      }

#endif

      // this is symmetric matrix ...
      for (i = 0; i < (1 + m_Order) / 2; i++)
      {
        for (j = i; j < m_Order - i; j++)
        {
          I[j, i] = I[i, j];
        }
      }

      // and a persymmetric matrix.
      for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
      {
        for (k = 0, l = m_Order - 1; k < j; k++, l--)
        {
          I[l, j] = I[i, k];
        }
      }

      return I;
    }

    #endregion Public Methods

    #region Public Static Methods

    /// <overloads>
    /// Solve a symmetric square Toeplitz system.
    /// </overloads>
    /// <summary>
    /// Solve a symmetric square Toeplitz system with a right-side vector.
    /// </summary>
    /// <param name="T">The left-most column of the Toeplitz matrix.</param>
    /// <param name="Y">The right-side vector of the system.</param>
    /// <returns>The solution vector.</returns>
    /// <exception cref="ArgumentNullException">
    /// <B>T</B> and/or <B>Y</B> are null references
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>T</B> does not match the length of <B>Y</B>.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This method solves the linear system <B>AX</B> = <B>Y</B>. Where
    /// <B>T</B> is a symmetric square Toeplitz matrix, <B>X</B> is an unknown
    /// vector and <B>Y</B> is a known vector.
    /// <para>
    /// This static member combines the <b>UDL</b> decomposition and the calculation of the solution into a
    /// single algorithm. When compared to the non-static member it requires minimal data storage
    /// and suffers from no speed penalty.
    /// </para>
    /// </remarks>
    public static ComplexDoubleVector Solve(IROComplexDoubleVector T, IROComplexDoubleVector Y)
    {

      ComplexDoubleVector X;

      // check parameters
      if (T == null)
      {
        throw new System.ArgumentNullException("T");
      }
      else if (Y == null)
      {
        throw new System.ArgumentNullException("Y");
      }
      else if (T.Length != Y.Length)
      {
        throw new RankException("The length of T and Y are not equal.");
      }
      else
      {

        // allocate memory
        int N = T.Length;
        X = new ComplexDoubleVector(N);                    // solution vector
        Complex e;                                   // prediction error

        // setup zero order solution
        e = T[0];
        if (e == Complex.Zero)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }
        X[0] = Y[0] / T[0];

        if (N > 1)
        {
          ComplexDoubleVector a = new ComplexDoubleVector(N - 1);   // prediction coefficients
          ComplexDoubleVector Z = new ComplexDoubleVector(N - 1);   // temporary storage vector
          Complex g;                                   // reflection coefficient
          Complex inner;                               // inner product
          Complex k;
          int i, j, l;

          // calculate solution for successive orders
          for (i = 1; i < N; i++)
          {

            // calculate first inner product
            inner = T[i];
            for (j = 0, l = i - 1; j < i - 1; j++, l--)
            {
              inner += a[j] * T[l];
            }

            // update predictor coefficients
            g = -(inner / e);
            for (j = 0, l = i - 2; j < i - 1; j++, l--)
            {
              Z[j] = a[j] + g * a[l];
            }

            // copy vector
            for (j = 0; j < i - 1; j++)
            {
              a[j] = Z[j];
            }

            a[i - 1] = g;

            e *= (Complex.One - g * g);
            if (e == Complex.Zero)
            {
              throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
            }

            // calculate second inner product
            inner = Y[i];
            for (j = 0, l = i; j < i; j++, l--)
            {
              inner -= X[j] * T[l];
            }

            // update solution vector
            k = inner / e;
            for (j = 0, l = i - 1; j < i; j++, l--)
            {
              X[j] = X[j] + k * a[l];
            }
            X[j] = k;

          }

        }

      }

      return X;

    }

    /// <summary>
    /// Solve a symmetric square Toeplitz system with a right-side matrix.
    /// </summary>
    /// <param name="T">The left-most column of the Toeplitz matrix.</param>
    /// <param name="Y">The right-side matrix of the system.</param>
    /// <returns>The solution matrix.</returns>
    /// <exception cref="ArgumentNullException">
    /// <B>T</B> and/or <B>Y</B> are null references
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>T</B> does not match the number of rows in <B>Y</B>.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This method solves the linear system <B>AX</B> = <B>Y</B>. Where
    /// <B>T</B> is a symmetric square Toeplitz matrix, <B>X</B> is an unknown
    /// matrix and <B>Y</B> is a known matrix.
    /// <para>
    /// This static member combines the <b>UDL</b> decomposition and the calculation of the solution into a
    /// single algorithm. When compared to the non-static member it requires minimal data storage
    /// and suffers from no speed penalty.
    /// </para>
    /// </remarks>
    public static ComplexDoubleMatrix Solve(IROComplexDoubleVector T, IROComplexDoubleMatrix Y)
    {

      ComplexDoubleMatrix X;

      // check parameters
      if (T == null)
      {
        throw new System.ArgumentNullException("T");
      }
      else if (Y == null)
      {
        throw new System.ArgumentNullException("Y");
      }
      else if (T.Length != Y.Columns)
      {
        throw new RankException("The length of T and Y are not equal.");
      }
      else
      {

        // allocate memory
        int N = T.Length;
        int M = Y.Rows;
        X = new ComplexDoubleMatrix(N, M);                 // solution matrix
        ComplexDoubleVector Z = new ComplexDoubleVector(N);       // temporary storage vector
        Complex e;                                   // prediction error
        int i, j, l, m;

        // setup zero order solution
        e = T[0];
        if (e == Complex.Zero)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }
        for (m = 0; m < M; m++)
        {
          X[0, m] = Y[0,m] / T[0];
        }

        if (N > 1)
        {

          ComplexDoubleVector a = new ComplexDoubleVector(N - 1);   // prediction coefficients
          Complex p;                                   // reflection coefficient
          Complex inner;                               // inner product
          Complex k;

          // calculate solution for successive orders
          for (i = 1; i < N; i++)
          {

            // calculate first inner product
            inner = T[i];
            for (j = 0, l = i - 1; j < i - 1; j++, l--)
            {
              inner += a[j] * T[l];
            }

            // update predictor coefficients
            p = -(inner / e);
            for (j = 0, l = i - 2; j < i - 1; j++, l--)
            {
              Z[j] = a[j] + p * a[l];
            }

            // copy vector
            for (j = 0; j < i - 1; j++)
            {
              a[j] = Z[j];
            }

            a[i - 1] = p;
            e *= (Complex.One - p * p);

            if (e == Complex.Zero)
            {
              throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
            }

            // update the solution matrix
            for (m = 0; m < M; m++)
            {

              // retrieve a copy of solution column
              for (j = 0; j < i; j++)
              {
                Z[j] = X[j, m];
              }

              // calculate second inner product
              inner = Y[i, m];
              for (j = 0, l = i; j < i; j++, l--)
              {
                inner -= Z[j] * T[l];
              }

              // update solution vector
              k = inner / e;
              for (j = 0, l = i - 1; j < i; j++, l--)
              {
                Z[j] = Z[j] + k * a[l];
              }
              Z[j] = k;

              // store solution column in matrix
              for (j = 0; j <= i; j++)
              {
                X[j, m] = Z[j];
              }

            }

          }

        }

      }

      return X;
    }

    /// <summary>
    /// Solve the Yule-Walker equations for a symmetric square Toeplitz system
    /// </summary>
    /// <param name="R">The left-most column of the Toeplitz matrix.</param>
    /// <returns>The solution vector.</returns>
    /// <exception cref="ArgumentNullException">
    /// <B>R</B> is a null reference.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The length of <B>R</B> must be greater than one.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This member is used to solve the Yule-Walker system <B>AX</B> = -<B>a</B>,
    /// where <B>A</B> is a symmetric square Toeplitz matrix, constructed
    /// from the elements <B>R[0]</B>, ..., <B>R[N-2]</B> and
    /// the vector <B>a</B> is constructed from the elements
    /// <B>R[1]</B>, ..., <B>R[N-1]</B>.
    /// <para>
    /// Durbin's algorithm is used to solve the linear system. It requires
    /// approximately the <b>N</b> squared FLOPS to calculate the
    /// solution (<b>N</b> is the matrix order).
    /// </para>
    /// </remarks>
    public static ComplexDoubleVector YuleWalker(IROComplexDoubleVector R)
    {

      ComplexDoubleVector a;

      // check parameters
      if (R == null)
      {
        throw new System.ArgumentNullException("R");
      }
      else if (R.Length < 2)
      {
        throw new System.ArgumentOutOfRangeException("R", "The length of R must be greater than 1.");
      }
      else
      {

        int N = R.Length - 1;
        a = new ComplexDoubleVector(N);             // prediction coefficients
        ComplexDoubleVector Z = new ComplexDoubleVector(N);   // temporary storage vector
        Complex e;                    // predictor error
        Complex inner;                  // inner product
        Complex g;                    // reflection coefficient
        int i, j, l;

        // setup first order solution
        e = R[0];
        if (e == Complex.Zero)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }
        g = -R[1] / R[0];
        a[0] = g;

        // calculate solution for successive orders
        for (i = 1; i < N; i++)
        {

          e *= (Complex.One - g * g);
          if (e == Complex.Zero)
          {
            throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
          }

          // calculate inner product
          inner = R[i + 1];
          for (j = 0, l = i; j < i; j++, l--)
          {
            inner += a[j] * R[l];
          }

          // update prediction coefficients
          g = -(inner / e);
          for (j = 0, l = i - 1; j < i; j++, l--)
          {
            Z[j] = a[j] + g * a[l];
          }

          // copy vector
          for (j = 0; j < i; j++)
          {
            a[j] = Z[j];
          }

          a[i] = g;

        }
      }

      return a;
    }

    /// <summary>
    /// Invert a symmetric square Toeplitz matrix.
    /// </summary>
    /// <param name="T">The left-most column of the symmetric Toeplitz matrix.</param>
    /// <returns>The inverse matrix.</returns>
    /// <exception cref="ArgumentNullException">
    /// <B>T</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>T</B> must be greater than zero.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This static member combines the <b>UDL</b> decomposition and Trench's algorithm into a
    /// single algorithm. When compared to the non-static member it requires minimal data storage
    /// and suffers from no speed penalty.
    /// <para>
    /// Trench's algorithm requires <b>N</b> squared FLOPS, compared to <b>N</b> cubed FLOPS
    /// if we simply solved a linear Toeplitz system with a right-side identity matrix (<b>N</b> is the matrix order).
    /// </para>
    /// </remarks>
    public static ComplexDoubleMatrix Inverse(IROComplexDoubleVector T)
    {

      ComplexDoubleMatrix X;

      // check parameters
      if (T == null)
      {
        throw new System.ArgumentNullException("T");
      }
      else if (T.Length < 1)
      {
        throw new System.RankException("The length of T must be greater than zero.");
      }
      else if (T.Length == 1)
      {
        X = new ComplexDoubleMatrix(1);
        X[0, 0] = Complex.One / T[0];
      }
      else
      {

        int N = T.Length;
        Complex f, g;
        int i, j, l, k, m, n;
        X = new ComplexDoubleMatrix(N);

        // calculate the predictor coefficients
        ComplexDoubleVector Y = ComplexDoubleSymmetricLevinson.YuleWalker(T);

        // calculate gamma
        f = T[0];
        for (i = 1, j = 0; i < N; i++, j++)
        {
          f += T[i] * Y[j];
        }
        g = Complex.One / f;

        // calculate first row of inverse
        X[0, 0] = g;
        for (i = 1, j = 0; i < N; i++, j++)
        {
          X[0, i] = g * Y[j];
        }

        // calculate successive rows of upper wedge
        for (i = 0, j = 1, k = N - 2; i < N / 2; i++, j++, k--)
        {
          for (l = j, m = i, n = N-1-j; l < N - j; l++, m++, n--)
          {
            X[j, l] = X[i, m] + g * (Y[i] * Y[m] - Y[k] * Y[n]);
          }
        }

        // this is symmetric matrix ...
        for (i = 0; i <= N / 2; i++)
        {
          for (j = i + 1; j < N - i; j++)
          {
            X[j, i] = X[i, j];
          }
        }

        // and a persymmetric matrix.
        for (i = 0, j = N - 1; i < N; i++, j--)
        {
          for (k = 0, l = N - 1; k < j; k++, l--)
          {
            X[l, j] = X[i, k];
          }
        }

      }

      return X;
    }

    #endregion Public Static Methods

  }

}
