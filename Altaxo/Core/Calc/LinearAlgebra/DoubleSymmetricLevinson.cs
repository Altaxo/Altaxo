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
** DoubleSymmetricLevinson.cs
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
  /// A Levinson solver for symmetric square Toeplitz systems of <c>double</c> type.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// This class provides members for inverting a symmetric square Toeplitz matrix
  /// (see <see cref="GetInverse"/> member), calculating the determinant of the matrix
  /// (see <see cref="GetDeterminant"/> member) and solving linear systems associated
  /// with the matrix (see <see cref="Solve(IROVector)"/> members).
  /// <para>
  /// The class implements an <B>UDL</B> decomposition of the inverse of the Toeplitz matrix.
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
  /// It has been shown that Levinson's algorithm is weakly numerically stable for positive-definite
  /// Toeplitz matrices. It is usual to restrict the use of the algorithm to such
  /// matrix types. The <see cref="IsPositiveDefinite"/> property may be checked to verify that the
  /// Toeplitz matrix is positive-definite.
  /// </para>
  /// <para>
  /// If one of the leading sub-matrices or the principal matrix is near-singular, then the accuracy
  /// of the decomposition will be degraded. An estimate of the resulting error is provided and may be
  /// accessed with the <see cref="Error"/> property. This estimate is only valid for positive-definite
  /// matrices.
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
  /// namespace Example_1
  /// {
  ///
  ///   class Application
  ///   {
  ///
  ///     // format string for matrix/vector elements
  ///     private const string frmStr = " 0.000E+000;-0.000E+000";
  ///
  ///     // application entry point
  ///     [STAThread]
  ///     public static void Main(string[] args)
  ///     {
  ///
  ///       // create Levinson solver
  ///       DoubleSymmetricLevinson dsl = new DoubleSymmetricLevinson(1.0, 0.5, 0.2);
  ///
  ///       // display the Toeplitz matrix
  ///       DoubleMatrix T = dsl.GetMatrix();
  ///       Console.WriteLine("Matrix:: {0} ", T.ToString(frmStr));
  ///       Console.WriteLine();
  ///
  ///       // check if matrix is singular
  ///       Console.WriteLine("Singular:               {0}", dsl.IsSingular);
  ///
  ///       // check if matrix is positive definite
  ///       Console.WriteLine("Positive Definite:      {0}", dsl.IsPositiveDefinite);
  ///
  ///       // get error for inverse
  ///       Console.WriteLine("Cybenko Error Estimate: {0}", dsl.Error.ToString("E3"));
  ///
  ///       // get the determinant
  ///       Console.WriteLine("Determinant:            {0}", dsl.GetDeterminant().ToString("E3"));
  ///       Console.WriteLine();
  ///
  ///       // get the inverse
  ///       DoubleMatrix Inv = dsl.GetInverse();
  ///       Console.WriteLine("Inverse:: {0} ", Inv.ToString(frmStr));
  ///       Console.WriteLine();
  ///
  ///       // solve a linear system
  ///       DoubleVector X = dsl.Solve(4.0, -1.0, 3.0);
  ///       Console.WriteLine("X:: {0} ", X.ToString(frmStr));
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
  /// Matrix:: rows: 3, cols: 3
  ///  1.000E+000,  5.000E-001,  2.000E-001
  ///  5.000E-001,  1.000E+000,  5.000E-001
  ///  2.000E-001,  5.000E-001,  1.000E+000
  ///
  /// Singular:               False
  /// Positive Definite:      True
  /// Cybenko Error Estimate: 7.613E-016
  /// Determinant:            5.600E-001
  ///
  /// Inverse:: rows: 3, cols: 3
  ///  1.339E+000, -7.143E-001,  8.929E-002
  /// -7.143E-001,  1.714E+000, -7.143E-001
  ///  8.929E-002, -7.143E-001,  1.339E+000
  ///
  /// X:: Length: 3
  ///  6.339E+000, -6.714E+000,  5.089E+000
  /// </code>
  /// </para>
  /// </example>
  sealed public class DoubleSymmetricLevinson : Algorithm
  {

    #region Constants

    /// <summary>
    /// The unit roundoff for double type.
    /// </summary>
    /// <remarks>
    /// It is assumed that the <see cref="double"/> type has a 52-bit mantissa.
    /// </remarks>
    private const double UNITROUNDOFF = 2.2204e-16;

    #endregion Constants

    #region Fields

    /// <summary>
    /// The left-most column of the Toeplitz matrix.
    /// </summary>
    private readonly DoubleVector m_LeftColumn;

    /// <summary>
    /// The system order.
    /// </summary>
    private readonly int m_Order;

    /// <summary>
    /// lower triangular matrix.
    /// </summary>
    private double[][] m_LowerTriangle;

    /// <summary>
    /// The diagonal vector.
    /// </summary>
    private double[] m_Diagonal;

    /// <summary>
    /// Flag to indicate if system is singular.
    /// </summary>
    private bool m_IsSingular;

    /// <summary>
    /// Flag to indicate if system is positive definite.
    /// </summary>
    private bool m_IsPositiveDefinite;

    /// <summary>
    /// The Cybenko error-bound
    /// </summary>
    private double m_CybenkoError;

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

    /// <summary>
    /// Check if the Toeplitz matrix is positive definite.
    /// </summary>
    /// <remarks>
    /// It has only been shown that the Levinson algorithm is weakly numerically stable
    /// for symmetric positive-definite Toeplitz matrices.
    /// Based on empirical results, it appears that the Levinson algorithm
    /// gives reasonable accuracy for many symmetric indefinite matrices.
    /// It may be desirable to restrict the use of this class to positive-definite matrices
    /// to guarantee accuracy.
    /// </remarks>
    public bool IsPositiveDefinite
    {
      get
      {
        // make sure the factorisation is completed
        Compute();
        // return the flag value
        return m_IsPositiveDefinite;
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
    public DoubleMatrix L
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

        // copy jagged array into a DoubleMatrix
        DoubleMatrix Lower = new DoubleMatrix(m_Order);
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
    public DoubleMatrix D
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

        // copy diagonal vector into a DoubleMatrix
        DoubleMatrix Diagonal = new DoubleMatrix(m_Order);
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
    public DoubleMatrix U
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

        // copy jagged array into a DoubleMatrix and then calculate the transpose
        DoubleMatrix Upper = this.L.GetTranspose();

        return Upper;
      }
    }

    /// <summary>
    /// Get an error estimate for the inverse matrix.
    /// </summary>
    /// <remarks>
    /// This estimate is approximate and is only valid for positive definite matrices.
    /// It is useful for checking the accurary of the <b>UDL</b> decomposition. If the
    /// Toeplitz matrix or one of the leading sub-matrices is near-singular,
    /// the accuracy of the decomposition will be degraded. This error estimate
    /// can be used to identify such situations.
    /// <para>
    /// The estimate is based on the Cybenko error-bound for the inverse
    /// and is a relative error. When calculating this error it is assumed that 
    /// the <see cref="double"/> type utilises a 52-bit mantissa.
    /// </para>
    /// <para>
    /// The error estimate will range in value between 2.22e-16 and <see cref="double.PositiveInfinity"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example calculates the inverse of a symmetric positive-definite Toeplitz matrix.
    /// An exact inverse was obtained previously using a computer algebra system. The exact inverse is
    /// compared to the inverse calculated by this class. The actual error is identified and compared
    /// to the Cybenko error-bound.
    /// <para>
    /// <code>
    /// using System;
    /// using dnA.Exceptions;
    /// using dnA.Math;
    /// using System.IO;
    ///
    /// namespace Example_2
    /// {
    ///
    ///   class Application
    ///   {
    ///
    ///     // The main entry point for the application.
    ///     [STAThread]
    ///     public static void Main(string[] args)
    ///     {
    ///       // exact inverse from yacas
    ///       DoubleMatrix exact = new DoubleMatrix(5);
    ///       exact[4, 4] = exact[0, 0] = +1.3577840963547489E+000;
    ///       exact[3, 4] = exact[4, 3] = exact[1, 0] = exact[0, 1] = -5.9039647295613562E-001;
    ///       exact[2, 4] = exact[4, 2] = exact[2, 0] = exact[0, 2] = -1.0830324909747292E-001;
    ///       exact[1, 4] = exact[4, 1] = exact[3, 0] = exact[0, 3] = -5.9423021628701958E-002;
    ///       exact[4, 0] = exact[0, 4] = -5.8145107185073958E-002;
    ///       exact[3, 3] = exact[1, 1] = +1.6120123957701031E+000;
    ///       exact[3, 2] = exact[2, 3] = exact[2, 1] = exact[1, 2] = -5.4584837545126352E-001;
    ///       exact[3, 1] = exact[1, 3] = -8.7102648477684425E-002;
    ///       exact[2, 2] = +1.6180505415162454E+000;
    ///
    ///       // create levinson solver
    ///       DoubleSymmetricLevinson dsl = new DoubleSymmetricLevinson(1.0, 1.0/2.0, 1.0/3.0, 1.0/4.0, 1.0/5.0);
    ///
    ///       // identify relative error
    ///       DoubleMatrix Error = dsl.GetInverse() - exact;
    ///       double e = Error.GetL1Norm() / exact.GetL1Norm();
    ///
    ///       // display results
    ///       Console.WriteLine("Observed Error:         {0}", e.ToString("E3"));
    ///       Console.WriteLine("Cybenko Error Estimate: {0}", dsl.Error.ToString("E3"));
    ///       Console.WriteLine();
    ///     }
    ///
    ///   }
    ///
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// The application generates the following results.
    /// </para>
    /// <para>
    /// <code escaped="true">
    /// Observed Error:         1.233E-016
    /// Cybenko Error Estimate: 1.028E-015
    /// </code>
    /// </para>
    /// </example>
    public double Error
    {
      get
      {
        // make sure the factorisation is completed
        Compute();
        // return the error estimate
        return m_CybenkoError;
      }
    }

    #endregion Properties

    #region Constructors

    /// <overloads>
    /// There are two permuations of the constructor, both require a parameter corresponding
    /// to the left-most column of a Toeplitz matrix.
    /// </overloads>
    /// <summary>
    /// Constructor with <c>DoubleVector</c> parameter.
    /// </summary>
    /// <param name="T">The left-most column of the Toeplitz matrix.</param>
    /// <exception cref="ArgumentNullException">
    /// <B>T</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>T</B> is zero.
    /// </exception>
    public DoubleSymmetricLevinson(IROVector T)
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
      m_LeftColumn = new DoubleVector(T);
      m_Order = m_LeftColumn.Length;

      // allocate memory for lower triangular matrix
      m_LowerTriangle = new double[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_LowerTriangle[i] = new double[i+1];
      }

      // allocate memory for diagonal
      m_Diagonal = new double[m_Order];
    }

    /// <summary>
    /// Constructor with <c>double</c> array parameter.
    /// </summary>
    /// <param name="T">The left-most column of the Toeplitz matrix.</param>
    /// <exception cref="ArgumentNullException">
    /// <B>T</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>T</B> is zero.
    /// </exception>
    public DoubleSymmetricLevinson(params double[] T)
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
      m_LeftColumn = new DoubleVector(T);
      m_Order = m_LeftColumn.Length;

      // allocate memory for lower triangular matrix
      m_LowerTriangle = new double[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_LowerTriangle[i] = new double[i+1];
      }

      // allocate memory for diagonal
      m_Diagonal = new double[m_Order];
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
    /// <para>
    /// The diagonal elements are checked to determine if the Toeplitz matrix is
    /// positive definite. The <EM>m_IsPositiveDefinite flag</EM>EM> is set <B>true</B>, if all
    /// diagonal elements are positive, otherwise it is set to <B>false</B>.
    /// </para>
    /// <para>
    /// During the calculation the reflection coefficients are used to calculate the
    /// Cybenko error estimate which is stored in the member <EM>m_CybenkoError</EM>.
    /// </para>
    /// </remarks>
    protected override void InternalCompute()
    {
      int i, j, l;      // index/loop variables
      double Inner;     // inner product
      double K;       // reflection coefficient
      double[] B;       // reference to previous order coefficients
      double[] A;       // reference to current order coefficients


      // check if principal diagonal is zero
      if (m_LeftColumn[0] == 0.0)
      {
        m_IsSingular = true;
        m_IsPositiveDefinite = false;
        m_CybenkoError = double.MaxValue;
        return;
      }

      // setup zero order solution
      B = m_LowerTriangle[0];
      B[0] = 1.0;
      m_Diagonal[0] = 1.0 / m_LeftColumn[0];
      m_CybenkoError = UNITROUNDOFF;

      // solve systems of increasing order
      for (i = 1; i < m_Order; i++, B = A)
      {
        // calculate inner product
        Inner = 0.0;
        for ( j = 0, l = 1; j < i; j++, l++ )
        {
          Inner += m_LeftColumn[l] * B[j];
        }

        // calculate the reflection coefficient
        K = -Inner * m_Diagonal[i-1];

        // get the current low triangle row
        A = m_LowerTriangle[i];

        // update low triangle elements
        A[0] = 0.0;
        Array.Copy(B, 0, A, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          A[j] += K * B[l];
        }

        // check if singular sub-matrix
        if (K*K == 1.0)
        {
          m_IsSingular = true;
          m_IsPositiveDefinite = false;
          m_CybenkoError = double.MaxValue;
          return;
        }

        // update diagonal
        m_Diagonal[i] = m_Diagonal[i - 1]/(1.0 - K * K);

        // update error estimate
        m_CybenkoError *= (1.0 + System.Math.Abs(K)) / (1.0 - System.Math.Abs(K));
      }

      // check if system is positive-definite
      for (i = 0, m_IsPositiveDefinite = true; (i < m_Order) && (m_IsPositiveDefinite == true); i++)
      {
        if (m_Diagonal[i] < 0.0)
        {
          m_IsPositiveDefinite = false;
        }
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
    public DoubleVector GetVector()
    {
      return new DoubleVector(m_LeftColumn);
    }

    /// <summary>
    /// Get a copy of the Toeplitz matrix.
    /// </summary>
    public DoubleMatrix GetMatrix()
    {
      int i, j;

      // allocate memory for the matrix
      DoubleMatrix tm = new DoubleMatrix(m_Order);

#if MANAGED
      // fill top row
      double[] top = tm.data[0];
      Array.Copy(m_LeftColumn.data, 0, top, 0, m_Order);

      if (m_Order > 1)
      {
        // fill bottom row (reverse order)
        double[] bottom = tm.data[m_Order - 1];

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
        double[] top = new double[m_Order];
        Array.Copy(m_LeftColumn.data, 0, top, 0, m_Order);
        tm.SetRow(0, top);

        // fill bottom row (reverse order)
        double[] bottom = new double[m_Order];

        for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
        {
          bottom[i] = m_LeftColumn[j];
        }

        // fill rows in-between
        for (i = 1, j = m_Order - 1 ; j > 0; i++)
        {
          double[] temp = new double[m_Order];
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
    public double GetDeterminant()
    {
      double Determinant;

      // make sure the factorisation is completed
      Compute();

      // if any of the matrices is singular give up
      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
      }
      else
      {
        Determinant = 1.0;
        for (int i = 0; i < m_Order; i++)
        {
          Determinant *= m_Diagonal[i];
        }
        Determinant = 1.0 / Determinant;
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
    public DoubleVector Solve(IROVector Y)
    {
      DoubleVector X;

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
      double Inner;     // inner product
      double G;       // scaling constant
      double[] A;       // reference to current order coefficients

      // allocate memory for solution
      X = new DoubleVector(m_Order);

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

    /// <overloads>
    /// Solve a symmetric square Toeplitz system.
    /// </overloads>
    /// <summary>
    /// Solve a symmetric square Toeplitz system with a right-side array.
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
    public DoubleVector Solve(params double[] Y)
    {

      DoubleVector X;

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
      double Inner;     // inner product
      double G;       // scaling constant
      double[] A;       // reference to current order coefficients

      // allocate memory for solution
      X = new DoubleVector(m_Order);

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
    public DoubleMatrix Solve(IROMatrix Y)
    {
      DoubleMatrix X;

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
      double[] Inner;     // inner product
      double[] G;       // scaling constant
      double[] A;       // reference to current order coefficients
      double scalar;

      // allocate memory for solution
      X = new DoubleMatrix(m_Order, M);
      Inner = new double[M];
      G = new double[M];

      // setup zero order solution
      scalar = 1.0 / m_LeftColumn[0];
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
    public DoubleMatrix GetInverse()
    {
      Compute();

      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
      }

      DoubleMatrix I = new DoubleMatrix(m_Order);           // the solution matrix
      double[] A = m_LowerTriangle[m_Order-1];
      double A1, A2, scale;

#if MANAGED

      double[] current, previous;                   // references to rows in the solution
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

    public static DoubleVector Solve(AbstractRODoubleVector T, AbstractRODoubleVector Y)
    {
      return Solve((IROVector)T, (IROVector)Y);
    }


    public static DoubleMatrix Solve(AbstractRODoubleVector T, IROMatrix Y)
    {
      return Solve((IROVector)T, Y);
    }


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
    public static DoubleVector Solve(IROVector T, IROVector Y)
    {

      DoubleVector X;

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
        X = new DoubleVector(N);                    // solution vector
        double e;                                   // prediction error

        // setup zero order solution
        e = T[0];
        if (e == 0.0)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }
        X[0] = Y[0] / T[0];

        if (N > 1)
        {
          DoubleVector a = new DoubleVector(N - 1);   // prediction coefficients
          DoubleVector Z = new DoubleVector(N - 1);   // temporary storage vector
          double g;                                   // reflection coefficient
          double inner;                               // inner product
          double k;
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

            e *= (1.0 - g * g);
            if (e == 0.0)
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
    public static DoubleMatrix Solve(IROVector T, IROMatrix Y)
    {

      DoubleMatrix X;

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
        X = new DoubleMatrix(N, M);                 // solution matrix
        DoubleVector Z = new DoubleVector(N);       // temporary storage vector
        double e;                                   // prediction error
        int i, j, l, m;

        // setup zero order solution
        e = T[0];
        if (e == 0.0)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }
        for (m = 0; m < M; m++)
        {
          X[0, m] = Y[0,m] / T[0];
        }

        if (N > 1)
        {

          DoubleVector a = new DoubleVector(N - 1);   // prediction coefficients
          double p;                                   // reflection coefficient
          double inner;                               // inner product
          double k;

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
            e *= (1.0 - p * p);

            if (e == 0.0)
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

    public static DoubleVector YuleWalker(AbstractRODoubleVector R)
    {
      return YuleWalker((IROVector)R);
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
    public static DoubleVector YuleWalker(IROVector R)
    {

      DoubleVector a;

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
        a = new DoubleVector(N);                    // prediction coefficients
        DoubleVector Z = new DoubleVector(N);   // temporary storage vector
        double e;                           // predictor error
        double inner;                               // inner product
        double g;                                   // reflection coefficient
        int i, j, l;

        // setup first order solution
        e = R[0];
        if (e == 0.0)
        {
          throw new SingularMatrixException("The Toeplitz matrix or one of the the leading sub-matrices is singular.");
        }
        g = -R[1] / R[0];
        a[0] = g;

        // calculate solution for successive orders
        for (i = 1; i < N; i++)
        {

          e *= (1.0 - g * g);
          if (e == 0.0)
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
    public static DoubleMatrix Inverse(IROVector T)
    {

      DoubleMatrix X;

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
        X = new DoubleMatrix(1);
        X[0, 0] = 1.0 / T[0];
      }
      else
      {

        int N = T.Length;
        double f, g;
        int i, j, l, k, m, n;
        X = new DoubleMatrix(N);

        // calculate the predictor coefficients
        DoubleVector Y = DoubleSymmetricLevinson.YuleWalker(T);

        // calculate gamma
        f = T[0];
        for (i = 1, j = 0; i < N; i++, j++)
        {
          f += T[i] * Y[j];
        }
        g = 1.0 / f;

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
