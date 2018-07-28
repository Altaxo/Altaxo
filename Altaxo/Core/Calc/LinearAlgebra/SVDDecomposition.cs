#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
#if false
  /// <summary>
  /// Routines from the GSL1.8/linalg/svd.c
  /// </summary>
  class SVDDecomposition
  {
    /* Modified algorithm which is better for M>>N */

    int gsl_linalg_SV_decomp_mod(IROMatrix A,
                               IROMatrix X,
                              IMatrix V, IVector S, IVector work)
    {
      int i, j;

      int M = A.Rows;
      int N = A.Columns;

      if (M < N)
      {
        throw new ArgumentException("svd of MxN matrix, M<N, is not implemented");
      }
      else if (V.Rows != N)
      {
        throw new ArgumentException("square matrix V must match second dimension of matrix A");
      }
      else if (V.Rows != V.Columns)
      {
        throw new ArgumentException("matrix V must be square");
      }
      else if (X.Rows != N)
      {
        throw new ArgumentException("square matrix X must match second dimension of matrix A");
      }
      else if (X.Rows != X.Columns)
      {
        throw new ArgumentException("matrix X must be square");
      }
      else if (S.Length != N)
      {
        throw new ArgumentException("length of vector S must match second dimension of matrix A");
      }
      else if (work.Length != N)
      {
        throw new ArgumentException("length of workspace must match second dimension of matrix A");
      }

      if (N == 1)
      {
        IROVector column = MatrixMath.ColumnToROVector(A,0); // gsl_vector_view  = gsl_matrix_column(A, 0);
        double norm = VectorMath.GetNorm(column); //gsl_blas_dnrm2(&column.vector);

        S[0] = norm; // gsl_vector_set(S, 0, norm);
        V[0,0] = 1; //gsl_matrix_set(V, 0, 0, 1.0);

        if (norm != 0.0)
        {
          gsl_blas_dscal(1.0 / norm, &column.vector);
        }

        return GSL_SUCCESS;
      }

      /* Convert A into an upper triangular matrix R */

      for (i = 0; i < N; i++)
      {
        IVector c = MatrixMath.ColumnToVector(A, i); // gsl_vector_view c = gsl_matrix_column(A, i);
        gsl_vector_view v = gsl_vector_subvector(&c.vector, i, M - i);
        double tau_i = gsl_linalg_householder_transform(&v.vector);

        /* Apply the transformation to the remaining columns */

        if (i + 1 < N)
        {
          gsl_matrix_view m =
            gsl_matrix_submatrix(A, i, i + 1, M - i, N - (i + 1));
          gsl_linalg_householder_hm(tau_i, &v.vector, &m.matrix);
        }

        gsl_vector_set(S, i, tau_i);
      }

      /* Copy the upper triangular part of A into X */

      for (i = 0; i < N; i++)
      {
        for (j = 0; j < i; j++)
        {
          gsl_matrix_set(X, i, j, 0.0);
        }

        {
          double Aii = gsl_matrix_get(A, i, i);
          gsl_matrix_set(X, i, i, Aii);
        }

        for (j = i + 1; j < N; j++)
        {
          double Aij = gsl_matrix_get(A, i, j);
          gsl_matrix_set(X, i, j, Aij);
        }
      }

      /* Convert A into an orthogonal matrix L */

      for (j = N; j > 0 && j--; )
      {
        /* Householder column transformation to accumulate L */
        double tj = gsl_vector_get(S, j);
        gsl_matrix_view m = gsl_matrix_submatrix(A, j, j, M - j, N - j);
        gsl_linalg_householder_hm1(tj, &m.matrix);
      }

      /* unpack R into X V S */

      gsl_linalg_SV_decomp(X, V, S, work);

      /* Multiply L by X, to obtain U = L X, stored in U */

      {
        gsl_vector_view sum = gsl_vector_subvector(work, 0, N);

        for (i = 0; i < M; i++)
        {
          gsl_vector_view L_i = gsl_matrix_row(A, i);
          gsl_vector_set_zero(&sum.vector);

          for (j = 0; j < N; j++)
          {
            double Lij = gsl_vector_get(&L_i.vector, j);
            gsl_vector_view X_j = gsl_matrix_row(X, j);
            gsl_blas_daxpy(Lij, &X_j.vector, &sum.vector);
          }

          gsl_vector_memcpy(&L_i.vector, &sum.vector);
        }
      }

      return GSL_SUCCESS;
    }
  }
#endif
}
