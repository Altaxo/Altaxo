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
** FloatLevinson.cs
**
** Class that implements UDL factorisation of the inverse of a square
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
  /// A Levinson solver for square Toeplitz systems of <c>float</c> type.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  /// <remarks>
  /// This class provides members for inverting the Toeplitz matrix (see <see cref="GetInverse"/> member),
  /// calculating the determinant of the matrix (see <see cref="GetDeterminant"/> property) and solving
  /// linear systems associated with the matrix (see <see cref="FloatLevinson.Solve(IROFloatVector)"/> members).
  /// <para>
  /// The class implements a <B>UDL</B> decomposition of the inverse of the 
  /// square Toeplitz matrix. The decomposition is based upon Levinson's algorithm. As
  /// a consequence, all operations require approximately <B>N</B> squared FLOPS, where
  /// <B>N</B> is the matrix order. This is significantly faster than <B>LU</B> factorization,
  /// which requires <B>N</B> cubed FLOPS.
  /// </para>
  /// <para>
  /// One disadvantage of Levinson's algorithm is that all the leading sub-matrices
  /// of the Toeplitz matrix must be non-singular. During the decomposition,
  /// sub-matrices are checked to ensure that they are non-singular. When a singular
  /// matrix is found the decomposition is halted and an interal flag is set. The
  /// <see cref="IsSingular"/> property may be used to access the flag, to determine
  /// if any singular matrices were detected.
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
  /// <note type="implementnotes">
  /// This class is optimised for non-symmetric Toeplitz matrices. If you wish
  /// to solve a symmetric Toeplitz system, use the <see cref="FloatSymmetricLevinson"/> class,
  /// which is twice as fast as this class.
  /// </note>
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
  /// namespace Example_10
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
  ///       // create levinson solver
  ///       FloatLevinson fl = new FloatLevinson(new float[]{10.0f, 2.0f, 9.0f, 5.0f}, new float[]{10.0f, 0.0f, 4.0f, 0.0f});
  ///
  ///       // display the Toeplitz matrix
  ///       FloatMatrix T = fl.GetMatrix();
  ///       Console.WriteLine("Matrix:: {0} ", T.ToString(frmStr));
  ///       Console.WriteLine();
  ///
  ///       // check if matrix is singular
  ///       Console.WriteLine("Singular:          {0}", fl.IsSingular);
  ///
  ///       // get the determinant
  ///       Console.WriteLine("Determinant:       {0}", fl.GetDeterminant.ToString("E3"));
  ///       Console.WriteLine();
  ///
  ///       // get the inverse
  ///       FloatMatrix Inv = fl.GetInverse();
  ///       Console.WriteLine("Inverse:: {0} ", Inv.ToString(frmStr));
  ///       Console.WriteLine();
  ///
  ///       // solve a linear system
  ///       FloatVector X = fl.Solve(1.0f, 2.0f, 3.0f, 4.0f);
  ///       Console.WriteLine("X:: {0} ", X.ToString(frmStr));
  ///       Console.WriteLine();
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
  ///  1.000E+001,  0.000E+000,  4.000E+000,  0.000E+000
  ///  2.000E+000,  1.000E+001,  0.000E+000,  4.000E+000
  ///  9.000E+000,  2.000E+000,  1.000E+001,  0.000E+000
  ///  5.000E+000,  9.000E+000,  2.000E+000,  1.000E+001
  ///
  /// Singular:          False
  /// Determinant:       4.256E+003
  ///
  /// Inverse:: rows: 4, cols: 4
  ///  1.541E-001,  1.880E-002, -6.015E-002, -7.519E-003
  /// -1.692E-002,  1.504E-001,  1.880E-002, -6.015E-002
  /// -1.353E-001, -4.699E-002,  1.504E-001,  1.880E-002
  /// -3.477E-002, -1.353E-001, -1.692E-002,  1.541E-001
  ///
  /// X:: Length: 4
  /// -1.880E-002,  9.962E-002,  2.970E-001,  2.603E-001
  /// </code>
  /// </para>
  /// </example>
  sealed public class FloatLevinson : Algorithm
  {

    #region Fields

    /// <summary>
    /// The left-most column of the Toeplitz matrix.
    /// </summary>
    private readonly FloatVector m_LeftColumn;

    /// <summary>
    /// The top-most row of the Toeplitz matrix.
    /// </summary>
    private readonly FloatVector m_TopRow;

    /// <summary>
    /// The order of the square Toeplitz matrix
    /// </summary>
    private readonly int m_Order;

    /// <summary>
    /// Flag to indicate if the matrix or one of the sub-matrices is singular.
    /// </summary>
    private bool m_IsSingular;

    /// <summary>
    /// The lower left triangular matrix of the factorisation.
    /// </summary>
    private float[][] m_LowerTriangle;

    /// <summary>
    /// The leading diagonal of the diagonal matrix of the factorisation.
    /// </summary>
    private float[] m_Diagonal;

    /// <summary>
    /// The upper right triangular matrix of the factorisation.
    /// </summary>
    private float[][] m_UpperTriangle;

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
    public FloatMatrix L
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

        // copy jagged array into a FloatMatrix
        FloatMatrix Lower = new FloatMatrix(m_Order);
#if MANAGED
        for (int i = 0; i < m_Order; i++ )
        {
          // managed implementation
          m_LowerTriangle[i].CopyTo(Lower.data[i], 0);
        }
#else
        for (int i = 0; i < m_Order; i++ )
        {
          for (int j = 0; j <= i; j++)
          {
            Lower[i, j] = m_LowerTriangle[i][j];
          }
        }
#endif
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
    public FloatMatrix D
    {
      get
      {
        // make sure the factorisation is completed
        Compute();

        // check if there was a result
        if (m_IsSingular == true)
        {
          throw new SingularMatrixException("One of the leading sub-matrices is singular.");
        }

        // copy diagonal vector into a FloatMatrix
        FloatMatrix Diagonal = new FloatMatrix(m_Order);
        Diagonal.SetDiagonal(m_Diagonal);

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
    public FloatMatrix U
    {
      get
      {
        // make sure the factorisation is completed
        Compute();

        // check if there was a result
        if (m_IsSingular == true)
        {
          throw new SingularMatrixException("One of the leading sub-matrices is singular.");
        }

        // copy jagged array into a FloatMatrix
        FloatMatrix Upper = new FloatMatrix(m_Order);
#if MANAGED
        for (int i = 0; i < m_Order; i++ )
        {
          m_UpperTriangle[i].CopyTo(Upper.data[i], 0);
        }

        Upper = Upper.GetTranspose();
#else
        for (int i = 0; i < m_Order; i++ )
        {
          for (int j = 0; j <= i; j++)
          {
            Upper[j, i] = m_UpperTriangle[i][j];
          }
        }
#endif

        return Upper;
      }
    }

    #endregion Properties

    #region Constructors

    public FloatLevinson(AbstractROFloatVector col, AbstractROFloatVector row)
      : this((IROFloatVector)col, (IROFloatVector)row)
    {
    }

    /// <overloads>
    /// There are two permuations of the constructor, one requires FloatVector parameters
    /// and the other float arrays.
    /// </overloads>
    /// <summary>
    /// Constructor with <c>FloatVector</c> parameters.
    /// </summary>
    /// <param name="col">The left-most column of the Toeplitz matrix.</param>
    /// <param name="row">The top-most row of the Toeplitz matrix.</param>
    /// <exception cref="ArgumentNullException">
    /// <B>col</B> is a null reference,
    /// <para>or</para>
    /// <para><B>row</B> is a null reference.</para>
    /// </exception>
    /// <exception cref="RankException">
    /// The length col <B>col</B> is zero,
    /// <para>or</para>
    /// <para>the length of <B>col</B> does not match that of <B>row</B>.</para>
    /// </exception>
    /// <exception cref="ArithmeticException">
    /// The values of the first element of <B>col</B> and <B>row</B> are not equal.
    /// </exception>
    public FloatLevinson(IROFloatVector col, IROFloatVector row)
    {
      // check parameters
      if (col == null)
      {
        throw new System.ArgumentNullException("col");
      }
      else if (col.Length == 0)
      {
        throw new RankException("The length of col is zero.");
      }
      else if (row == null)
      {
        throw new System.ArgumentNullException("row");
      }
      else if (col.Length != row.Length)
      {
        throw new RankException("The lengths of col and row are not equal.");
      }
      else if (col[0] != row[0])
      {
        throw new ArithmeticException("The values of the first element of col and row are not equal.");
      }

      // save the vectors
      m_LeftColumn = new FloatVector(col);
      m_TopRow = new FloatVector(row);
      m_Order = m_LeftColumn.Length;

      // allocate memory for lower triangular matrix
      m_LowerTriangle = new float[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_LowerTriangle[i] = new float[i + 1];
      }

      // allocate memory for diagonal
      m_Diagonal = new float[m_Order];

      // allocate memory for upper triangular matrix
      m_UpperTriangle = new float[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_UpperTriangle[i] = new float[i + 1];
      }
    }

    /// <summary>
    /// Constructor with <c>float[]</c> parameters.
    /// </summary>
    /// <param name="col">The left-most column of the Toeplitz matrix.</param>
    /// <param name="row">The top-most row of the Toeplitz matrix.</param>
    /// <exception cref="ArgumentNullException">
    /// <B>col</B> is a null reference,
    /// <para>or</para>
    /// <para><B>row</B> is a null reference.</para>
    /// </exception>
    /// <exception cref="RankException">
    /// The length col <B>col</B> is zero,
    /// <para>or</para>
    /// <para>the length of <B>col</B> does not match that of <B>row</B>.</para>
    /// </exception>
    /// <exception cref="ArithmeticException">
    /// The values of the first element of <B>col</B> and <B>row</B> are not equal.
    /// </exception>
    public FloatLevinson(float[] col, float[] row)
    {
      // check parameters
      if (col == null)
      {
        throw new System.ArgumentNullException("col");
      }
      else if (col.Length == 0)
      {
        throw new RankException("The length of col is zero.");
      }
      else if (row == null)
      {
        throw new System.ArgumentNullException("row");
      }
      else if (col.Length != row.Length)
      {
        throw new RankException("The lengths of col and row are not equal.");
      }
      else if (col[0] != row[0])
      {
        throw new ArithmeticException("The values of the first element of col and row are not equal.");
      }

      // save the vectors
      m_LeftColumn = new FloatVector(col);
      m_TopRow = new FloatVector(row);
      m_Order = m_LeftColumn.Length;

      // allocate memory for lower triangular matrix
      m_LowerTriangle = new float[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_LowerTriangle[i] = new float[i + 1];
      }

      // allocate memory for diagonal
      m_Diagonal = new float[m_Order];

      // allocate memory for upper triangular matrix
      m_UpperTriangle = new float[m_Order][];
      for (int i = 0; i < m_Order; i++)
      {
        m_UpperTriangle[i] = new float[i + 1];
      }
    }

    #endregion Constructors

    #region Protected Members

    /// <summary>
    /// Calculate the UDL decomposition of the inverse Toeplitz matrix.
    /// </summary>
    /// <remarks>
    /// The member checks each iteration for a singular leading sub-matrix.
    /// If a singular sub-matrix is detected, the private boolean <EM>m_IsSingular</EM>
    /// is set to <B>true</B> and the decomposition halted.
    /// <para>
    /// When the decomposition is completed, the lower triangular matrix is stored
    /// in the jagged array <EM>m_LowerTriangle</EM>, the diagonal values
    /// in the array <EM>m_Diagonal</EM>, the upper triangular matrix is stored in the jagged array
    /// <EM>m_UpperTriangle</EM> and the private boolean <EM>m_IsSingular</EM> is set to <B>false</B>.
    /// </para>
    /// </remarks>
    protected override void InternalCompute()
    {
      int i, j, l;
      float Q, S;     // inner products
      float Ke, Kr;     // reflection coefficients
      float[] AP;     // reference to previous row in lower triangle
      float[] A;        // reference to current row in lower triangle
      float[] BP;     // reference to previous row in upper triangle
      float[] B;        // reference to current row in upper triangle

      // check if leading diagonal is zero
      if (m_LeftColumn[0] == 0.0f)
      {
        m_IsSingular = true;
        return;
      }

      // setup zero order solution
      AP = m_LowerTriangle[0];
      AP[0] = 1.0f;
      m_Diagonal[0] = 1.0f / m_LeftColumn[0];
      BP = m_UpperTriangle[0];
      BP[0] = 1.0f;

      for (i = 1; i < m_Order; i++, AP = A, BP = B)
      {
        // calculate inner products
        Q = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          Q += m_LeftColumn[l] * AP[j];
        }

        S = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          S += m_TopRow[l] * BP[j];
        }

        // reflection coefficients
        Kr = -S * m_Diagonal[i-1];
        Ke = -Q * m_Diagonal[i-1];

        // update lower triangle
        A = m_LowerTriangle[i];
        A[0] = 0.0f;
        Array.Copy(AP, 0, A, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          A[j] += Ke * BP[l];
        }

        // update upper triangle
        B = m_UpperTriangle[i];
        B[0] = 0.0f;
        Array.Copy(BP, 0, B, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          B[j] += Kr * AP[l];
        }

        // check for singular sub-matrix)
        if (Ke * Kr == 1.0f)
        {
          m_IsSingular = true;
          return;
        }

        // diagonal matrix
        m_Diagonal[i] = m_Diagonal[i-1] / (1.0f - Ke * Kr);
      }
    }

    #endregion Protected Members

    #region Public Members

    /// <summary>
    /// Get a vector that represents the left-most column of the Toplitz matrix.
    /// </summary>
    public FloatVector GetLeftColumn()
    {
      return new FloatVector(m_LeftColumn);
    }

    /// <summary>
    /// Get a vector that represents the top-most row of the Toplitz matrix.
    /// </summary>
    public FloatVector GetTopRow()
    {
      return new FloatVector(m_TopRow);
    }

    /// <summary>
    /// Get a copy of the Toeplitz matrix.
    /// </summary>
    public FloatMatrix GetMatrix()
    {
      int i;

      // allocate memory for the matrix
      FloatMatrix tm = new FloatMatrix(m_Order);
#if MANAGED
      // fill lower triangle
      for (i = 0; i < m_Order; i++)
      {
        Array.Copy(m_LeftColumn.data, 0, tm.data[i], i, m_Order - i);
      }

      tm = tm.GetTranspose();

      // fill upper triangle
      for (i = 0; i < m_Order - 1; i++)
      {
        Array.Copy(m_TopRow.data, 1, tm.data[i], i + 1, m_Order - i - 1);
      }
#else
      int j, k;

      // fill lower triangle
      for (i = 0; i < m_Order; i++)
      {
        for (j = i, k = 0; j < m_Order; j++, k++)
        {
          tm[j, i] = m_LeftColumn[k];
        }
      }
      // fill upper triangle
      for (i = 0; i < m_Order; i++)
      {
        for (j = i + 1, k = 1; j < m_Order; j++, k++)
        {
          tm[i, j] = m_TopRow[k];
        }
      }
#endif
      return tm;
    }

    /// <summary>
    /// Get the determinant of the Toeplitz matrix.
    /// </summary>
    /// <returns>The determinant</returns>
    /// <remarks>
    /// It is recommended that the <see cref="IsSingular"/> property
    /// be checked to see if the decomposition was completed, before attempting
    /// to obtain the determinant.
    /// </remarks>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    public float GetDeterminant()
    {
      float det;

      // make sure the factorisation is completed
      Compute();

      // if any of the matrices is singular give up
      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }
      else
      {
        det = 1.0f;
        for (int i = 0; i < m_Order; i++)
        {
          det *= m_Diagonal[i];
        }
        det = 1.0f / det;
      }

      return det;
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
    /// using the Levinson algorithm, before using an extended version of Trench's algorithm to complete
    /// the inversion.
    /// <para>
    /// The extended version of Trench's algorithm requires approximately <b>N</b> squared FLOPS,
    /// compared to <b>N</b> cubed FLOPS if we simply multiplied the <b>UDL</b> factors
    /// (<b>N</b> is the matrix order).
    /// </para>
    /// </remarks>
    public FloatMatrix GetInverse()
    {
      Compute();

      if (m_IsSingular == true)
      {
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }

      FloatMatrix I = new FloatMatrix(m_Order);           // the solution matrix
      float[] A = m_LowerTriangle[m_Order-1];
      float[] B = m_UpperTriangle[m_Order-1];
      float A1, B1, scale;
      int i, j, k, l;
#if MANAGED
      float[] current, previous;                    // references to rows in the solution

      // setup the first row in wedge
      scale = m_Diagonal[m_Order-1];
      current = I.data[0];
      for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
      {
        current[i] = scale* B[j];
      }

      // calculate values in the rest of the wedge
      for (i = 1; i < m_Order; i++)
      {
        previous = current;
        current = I.data[i];
        A1 = scale * A[m_Order - i - 1];
        B1 = scale * B[i - 1];
        current[0] = A1;
        for (j = 1, k = 0, l = m_Order - 2; j < m_Order - i; j++, k++, l--)
        {
          current[j] = previous[k] + A1 * B[l] - B1 * A[k];
        }
      }
#else
      // setup the first row
      scale = m_Diagonal[m_Order-1];
      for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
      {
        I[0, i] = scale * B[j];
      }

      // calculate values in the rest of the wedge
      for (i = 1; i < m_Order; i++)
      {
        A1 = scale * A[m_Order - i - 1];
        B1 = scale * B[i - 1];
        I[i, 0] = A1;
        for (j = 1, k = 0, l = m_Order - 2; j < m_Order - i; j++, k++, l--)
        {
          I[i, j] = I[i - 1, k] + A1 * B[l] - B1 * A[k];
        }
      }

#endif

      // inverse is a persymmetric matrix.
      for (i = 0, j = m_Order - 1; i < m_Order; i++, j--)
      {
        for (k = 0, l = m_Order - 1; k < j; k++, l--)
        {
          I[l, j] = I[i, k];
        }
      }

      return I;
    }

    /// <overloads>
    /// Solve a square Toeplitz system.
    /// </overloads>
    /// <summary>
    /// Solve a square Toeplitz system with a right-side vector.
    /// </summary>
    /// <param name="Y">The right-hand side of the system.</param>
    /// <returns>The solution vector.</returns>
    /// <exception cref="ArgumentNullException">
    /// Parameter <B>Y</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of Y is not equal to the number of rows in the Toeplitz matrix.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This member solves the linear system <B>TX</B> = <B>Y</B>, where <B>T</B> is
    /// the square Toeplitz matrix, <B>X</B> is the unknown solution vector
    /// and <B>Y</B> is a known vector.
    /// <para>
    /// The class implicitly decomposes the inverse Toeplitz matrix into a <b>UDL</b> factorisation
    /// using the Levinson algorithm, before calculating the solution vector.
    /// </para>
    /// </remarks>
    public FloatVector Solve(IROFloatVector Y)
    {
      FloatVector X;
      float Inner;
      int i, j;

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
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }
      
      // allocate memory for solution
      float[] A, B;
      X = new FloatVector(m_Order);

      for (i = 0; i < m_Order; i++)
      {
        A = m_LowerTriangle[i];
        B = m_UpperTriangle[i];

        Inner = Y[i];
        for (j = 0; j < i; j++)
        {
          Inner += A[j] * Y[j];
        }
        Inner *= m_Diagonal[i];

        X[i] = Inner;
        for (j = 0; j < i; j++)
        {
          X[j] += Inner * B[j];
        }
      }
      return X;
    }

    /// <summary>
    /// Solve a square Toeplitz system with a right-side array.
    /// </summary>
    /// <param name="Y">The right-hand side of the system.</param>
    /// <returns>The solution vector.</returns>
    /// <exception cref="ArgumentNullException">
    /// Parameter <B>Y</B> is a null reference.
    /// </exception>
    /// <exception cref="RankException">
    /// The length of Y is not equal to the number of rows in the Toeplitz matrix.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <remarks>
    /// This member solves the linear system <B>TX</B> = <B>Y</B>, where <B>T</B> is
    /// the square Toeplitz matrix, <B>X</B> is the unknown solution vector
    /// and <B>Y</B> is a known vector.
    /// <para>
    /// The class implicitly decomposes the inverse Toeplitz matrix into a <b>UDL</b> factorisation
    /// using the Levinson algorithm, before calculating the solution vector.
    /// </para>
    /// </remarks>
    public FloatVector Solve(params float[] Y)
    {
      FloatVector X;
      float Inner;
      float[] a, b;
      int i, j;

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
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }

      // allocate memory for solution
      X = new FloatVector(m_Order);

      for (i = 0; i < m_Order; i++)
      {
        a = m_LowerTriangle[i];
        b = m_UpperTriangle[i];

        Inner = Y[i];
        for (j = 0; j < i; j++)
        {
          Inner += a[j] * Y[j];
        }
        Inner *= m_Diagonal[i];

        X[i] = Inner;
        for (j = 0; j < i; j++)
        {
          X[j] += Inner * b[j];
        }
      }

      return X;
    }

    /// <summary>
    /// Solve a square Toeplitz system with a right-side matrix.
    /// </summary>
    /// <param name="Y">The right-side matrix</param>
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
    /// a square Toeplitz matrix, <B>X</B> is the unknown solution matrix
    /// and <B>Y</B> is a known matrix.
    /// <para>
    /// The class implicitly decomposes the inverse Toeplitz matrix into a <b>UDL</b> factorisation
    /// using the Levinson algorithm, before calculating the solution vector.
    /// </para>
    /// </remarks>
    public FloatMatrix Solve(IROFloatMatrix Y)
    {
      FloatMatrix X;
      float Inner;
      float[] a, b, x, y;
      int i, j, l;

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
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }

      // allocate memory for solution
      X = new FloatMatrix(m_Order, Y.Rows);
      x = new float[m_Order];

      for (l = 0; l < Y.Rows; l++)
      {

        // get right-side column
        y = FloatVector.GetColumnAsArray(Y,l);

        // solve left-side column
        for (i = 0; i < m_Order; i++)
        {
          a = m_LowerTriangle[i];
          b = m_UpperTriangle[i];

          Inner = y[i];
          for (j = 0; j < i; j++)
          {
            Inner += a[j] * y[j];
          }
          Inner *= m_Diagonal[i];
  
          x[i] = Inner;
          for (j = 0; j < i; j++)
          {
            x[j] += Inner * b[j];
          }
        }

        // insert left-side column into the matrix
        X.SetColumn(l, x);
      }

      return X;
    }

    #endregion Public Members

    #region Static Public Members

    /// <summary>
    /// Invert a square Toeplitz matrix.
    /// </summary>
    /// <param name="col">The left-most column of the Toeplitz matrix.</param>
    /// <param name="row">The top-most row of the Toeplitz matrix.</param>
    /// <returns>The inverse matrix.</returns>
    /// <exception cref="ArgumentNullException">
    /// <B>col</B> is a null reference.
    /// <para>or</para>
    /// <para><B>row</B> is a null reference.</para>
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <B>col</B> is 0,
    /// <para>or</para>
    /// <para>the lengths of <B>col</B> and <B>row</B> are not equal.</para>
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <exception cref="ArithmeticException">
    /// The values of the first element of <B>col</B> and <B>row</B> are not equal.
    /// </exception>
    /// <remarks>
    /// This static member combines the <b>UDL</b> decomposition and Trench's algorithm into a
    /// single algorithm. It requires minimal data storage, compared to the non-static member
    /// and suffers from no speed penalty in comparision.
    /// <para>
    /// Trench's algorithm requires <b>N</b> squared FLOPS, compared to <b>N</b> cubed FLOPS
    /// if we simply solved a linear Toeplitz system with a right-side identity matrix (<b>N</b> is the matrix order).
    /// </para>
    /// </remarks>
    public static FloatMatrix Inverse(IROFloatVector col, IROFloatVector row)
    {
      // check parameters
      if (col == null)
      {
        throw new System.ArgumentNullException("col");
      }
      else if (col.Length == 0)
      {
        throw new RankException("The length of col is zero.");
      }
      else if (row == null)
      {
        throw new System.ArgumentNullException("row");
      }
      else if (col.Length != row.Length)
      {
        throw new RankException("The lengths of col and row are not equal.");
      }
      else if (col[0] != row[0])
      {
        throw new ArithmeticException("The values of the first element of col and row are not equal.");
      }

      // check if leading diagonal is zero
      if (col[0] == 0.0f)
      {
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }

      // decompose matrix
      int order = col.Length;
      float[] A = new float[order];
      float[] B = new float[order];
      float[] Z = new float[order];
      float Q, S, Ke, Kr, e;
      int i, j, k, l;

      // setup the zero order solution
      A[0] = 1.0f;
      B[0] = 1.0f;
      e = 1.0f / col[0];

      for (i = 1; i < order; i++)
      {
        // calculate inner products
        Q = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          Q += col[l] * A[j];
        }

        S = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          S += row[l] * B[j];
        }

        // reflection coefficients
        Kr = -S * e;
        Ke = -Q * e;

        // update lower triangle (in temporary storage)
        Z[0] = 0.0f;
        Array.Copy(A, 0, Z, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          Z[j] += Ke * B[l];
        }

        // update upper triangle
        for (j = i; j > 0; j--)
        {
          B[j] = B[j-1];
        }

        B[0] = 0.0f;
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          B[j] += Kr * A[l];
        }

        // copy from temporary storage to lower triangle
        Array.Copy(Z, 0, A, 0, i + 1);

        // check for singular sub-matrix)
        if (Ke * Kr == 1.0f)
        {
          throw new SingularMatrixException("One of the leading sub-matrices is singular.");
        }
      
        // update diagonal
        e = e / (1.0f - Ke * Kr);

      }

      // calculate the inverse
      FloatMatrix I = new FloatMatrix(order);           // the solution matrix
      float A1, B1;

#if MANAGED
      float[] current, previous;                    // references to rows in the solution

      // setup the first row in wedge
      current = I.data[0];
      for (i = 0, j = order - 1; i < order; i++, j--)
      {
        current[i] = e* B[j];
      }

      // calculate values in the rest of the wedge
      for (i = 1; i < order; i++)
      {
        previous = current;
        current = I.data[i];
        A1 = e * A[order - i - 1];
        B1 = e * B[i - 1];
        current[0] = A1;
        for (j = 1, k = 0, l = order - 2; j < order - i; j++, k++, l--)
        {
          current[j] = previous[k] + A1 * B[l] - B1 * A[k];
        }
      }

#else

      // setup the first row in wedge
      for (i = 0, j = order - 1; i < order; i++, j--)
      {
        I[0, i] = e * B[j];
      }

      // calculate values in the rest of the wedge
      for (i = 1; i < order; i++)
      {
        A1 = e * A[order - i - 1];
        B1 = e * B[i - 1];
        I[i, 0] = A1;
        for (j = 1, k = 0, l = order - 2; j < order - i; j++, k++, l--)
        {
          I[i, j] = I[i - 1, k] + A1 * B[l] - B1 * A[k];
        }
      }
#endif

      // inverse is a persymmetric matrix.
      for (i = 0, j = order - 1; i < order; i++, j--)
      {
        for (k = 0, l = order - 1; k < j; k++, l--)
        {
          I[l, j] = I[i, k];
        }
      }

      return I;

    }

    /// <summary>
    /// Solve a square Toeplitz system with a right-side vector.
    /// </summary>
    /// <param name="col">The left-most column of the Toeplitz matrix.</param>
    /// <param name="row">The top-most row of the Toeplitz matrix.</param>
    /// <param name="Y">The right-side vector of the system.</param>
    /// <returns>The solution vector.</returns>
    /// <exception cref="ArgumentNullException">
    /// <EM>col</EM> is a null reference,
    /// <para>or</para>
    /// <para><EM>row</EM> is a null reference,</para>
    /// <para>or</para>
    /// <para><EM>Y</EM> is a null reference.</para>
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <EM>col</EM> is 0,
    /// <para>or</para>
    /// <para>the lengths of <EM>col</EM> and <EM>row</EM> are not equal,</para>
    /// <para>or</para>
    /// <para>the number of rows in <EM>Y</EM> does not the length of <EM>col</EM> and <EM>row</EM>.</para>
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <exception cref="ArithmeticException">
    /// The values of the first element of <EM>col</EM> and <EM>row</EM> are not equal.
    /// </exception>
    /// <remarks>
    /// This method solves the linear system <B>AX</B> = <B>Y</B>. Where
    /// <B>T</B> is a square Toeplitz matrix, <B>X</B> is an unknown
    /// vector and <B>Y</B> is a known vector.
    /// <para>
    /// The classic Levinson algorithm is used to solve the system. The algorithm
    /// assumes that all the leading sub-matrices of the Toeplitz matrix are
    /// non-singular. When a sub-matrix is near singular, accuracy will
    /// be degraded. This member requires approximately <B>N</B> squared
    /// FLOPS to calculate a solution, where <B>N</B> is the matrix order.
    /// </para>
    /// <para>
    /// This static method has minimal storage requirements as it combines
    /// the <b>UDL</b> decomposition with the calculation of the solution vector
    /// in a single algorithm.
    /// </para>
    /// </remarks>
    public static FloatVector Solve(IROFloatVector col, IROFloatVector row, IROFloatVector Y)
    {
      // check parameters
      if (col == null)
      {
        throw new System.ArgumentNullException("col");
      }
      else if (col.Length == 0)
      {
        throw new RankException("The length of col is zero.");
      }
      else if (row == null)
      {
        throw new System.ArgumentNullException("row");
      }
      else if (col.Length != row.Length)
      {
        throw new RankException("The lengths of col and row are not equal.");
      }
      else if (col[0] != row[0])
      {
        throw new ArithmeticException("The values of the first element of col and row are not equal.");
      }
      else if (Y == null)
      {
        throw new System.ArgumentNullException("Y");
      }
      else if (col.Length != Y.Length)
      {
        throw new RankException("The length of Y does not match those of col and row.");
      }

      // check if leading diagonal is zero
      if (col[0] == 0.0f)
      {
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }

      // decompose matrix
      int order = col.Length;
      float[] A = new float[order];
      float[] B = new float[order];
      float[] Z = new float[order];
      FloatVector X = new FloatVector(order);
      float Q, S, Ke, Kr, e;
      float Inner;
      int i, j, l;

      // setup the zero order solution
      A[0] = 1.0f;
      B[0] = 1.0f;
      e = 1.0f / col[0];
      X[0] = e * Y[0];

      for (i = 1; i < order; i++)
      {
        // calculate inner products
        Q = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          Q += col[l] * A[j];
        }

        S = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          S += row[l] * B[j];
        }

        // reflection coefficients
        Kr = -S * e;
        Ke = -Q * e;

        // update lower triangle (in temporary storage)
        Z[0] = 0.0f;
        Array.Copy(A, 0, Z, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          Z[j] += Ke * B[l];
        }

        // update upper triangle
        for (j = i; j > 0; j--)
        {
          B[j] = B[j-1];
        }

        B[0] = 0.0f;
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          B[j] += Kr * A[l];
        }

        // copy from temporary storage to lower triangle
        Array.Copy(Z, 0, A, 0, i + 1);

        // check for singular sub-matrix)
        if (Ke * Kr == 1.0f)
        {
          throw new SingularMatrixException("One of the leading sub-matrices is singular.");
        }
      
        // update diagonal
        e = e / (1.0f - Ke * Kr);

        Inner = Y[i];
        for (j = 0; j < i; j++)
        {
          Inner += A[j] * Y[j];
        }
        Inner *= e;

        X[i] = Inner;
        for (j = 0; j < i; j++)
        {
          X[j] += Inner * B[j];
        }

      }

      return X;
    }
 
    /// <summary>
    /// Solve a square Toeplitz system with a right-side matrix.
    /// </summary>
    /// <param name="col">The left-most column of the Toeplitz matrix.</param>
    /// <param name="row">The top-most row of the Toeplitz matrix.</param>
    /// <param name="Y">The right-side matrix of the system.</param>
    /// <returns>The solution matrix.</returns>
    /// <exception cref="ArgumentNullException">
    /// <EM>col</EM> is a null reference,
    /// <para>or</para>
    /// <para><EM>row</EM> is a null reference,</para>
    /// <para>or</para>
    /// <para><EM>Y</EM> is a null reference.</para>
    /// </exception>
    /// <exception cref="RankException">
    /// The length of <EM>col</EM> is 0,
    /// <para>or</para>
    /// <para>the lengths of <EM>col</EM> and <EM>row</EM> are not equal,</para>
    /// <para>or</para>
    /// <para>the number of rows in <EM>Y</EM> does not the length of <EM>col</EM> and <EM>row</EM>.</para>
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The Toeplitz matrix or one of the the leading sub-matrices is singular.
    /// </exception>
    /// <exception cref="ArithmeticException">
    /// The values of the first element of <EM>col</EM> and <EM>row</EM> are not equal.
    /// </exception>
    /// <remarks>
    /// This method solves the linear system <B>AX</B> = <B>Y</B>. Where
    /// <B>T</B> is a square Toeplitz matrix, <B>X</B> is an unknown
    /// matrix and <B>Y</B> is a known matrix.
    /// <para>
    /// The classic Levinson algorithm is used to solve the system. The algorithm
    /// assumes that all the leading sub-matrices of the Toeplitz matrix are
    /// non-singular. When a sub-matrix is near singular, accuracy will
    /// be degraded. This member requires approximately <B>N</B> squared
    /// FLOPS to calculate a solution, where <B>N</B> is the matrix order.
    /// </para>
    /// <para>
    /// This static method has minimal storage requirements as it combines
    /// the <b>UDL</b> decomposition with the calculation of the solution vector
    /// in a single algorithm.
    /// </para>
    /// </remarks>
    public static FloatMatrix Solve(IROFloatVector col, IROFloatVector row, IROFloatMatrix Y)
    {
      // check parameters
      if (col == null)
      {
        throw new System.ArgumentNullException("col");
      }
      else if (col.Length == 0)
      {
        throw new RankException("The length of col is zero.");
      }
      else if (row == null)
      {
        throw new System.ArgumentNullException("row");
      }
      else if (col.Length != row.Length)
      {
        throw new RankException("The lengths of col and row are not equal.");
      }
      else if (col[0] != row[0])
      {
        throw new ArithmeticException("The values of the first element of col and row are not equal.");
      }
      else if (Y == null)
      {
        throw new System.ArgumentNullException("Y");
      }
      else if (col.Length != Y.Columns)
      {
        throw new RankException("The numer of rows in Y does not match the length of col and row.");
      }

      // check if leading diagonal is zero
      if (col[0] == 0.0f)
      {
        throw new SingularMatrixException("One of the leading sub-matrices is singular.");
      }

      // decompose matrix
      int order = col.Length;
      float[] A = new float[order];
      float[] B = new float[order];
      float[] Z = new float[order];
      FloatMatrix X = new FloatMatrix(order);
      float Q, S, Ke, Kr, e;
      float Inner;
      int i, j, l;

      // setup the zero order solution
      A[0] = 1.0f;
      B[0] = 1.0f;
      e = 1.0f / col[0];
      X.SetRow(0, e * FloatVector.GetRow(Y,0));

      for (i = 1; i < order; i++)
      {
        // calculate inner products
        Q = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          Q += col[l] * A[j];
        }

        S = 0.0f;
        for ( j = 0, l = 1; j < i; j++, l++)
        {
          S += row[l] * B[j];
        }

        // reflection coefficients
        Kr = -S * e;
        Ke = -Q * e;

        // update lower triangle (in temporary storage)
        Z[0] = 0.0f;
        Array.Copy(A, 0, Z, 1, i);
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          Z[j] += Ke * B[l];
        }

        // update upper triangle
        for (j = i; j > 0; j--)
        {
          B[j] = B[j-1];
        }

        B[0] = 0.0f;
        for (j = 0, l = i - 1; j < i; j++, l--)
        {
          B[j] += Kr * A[l];
        }

        // copy from temporary storage to lower triangle
        Array.Copy(Z, 0, A, 0, i + 1);

        // check for singular sub-matrix)
        if (Ke * Kr == 1.0f)
        {
          throw new SingularMatrixException("One of the leading sub-matrices is singular.");
        }
      
        // update diagonal
        e = e / (1.0f - Ke * Kr);

        for (l = 0; l < Y.Rows; l++)
        {
          FloatVector W = X.GetColumn(l);
          FloatVector M = FloatVector.GetColumn(Y,l);

          Inner = M[i];
          for (j = 0; j < i; j++)
          {
            Inner += A[j] * M[j];
          }
          Inner *= e;

          W[i] = Inner;
          for (j = 0; j < i; j++)
          {
            W[j] += Inner * B[j];
          }

          X.SetColumn(l, W);
        }

      }

      return X;
    }
        
    #endregion Static Public Members

  }
}
