#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Implementation of an algorithm that finds a vector x with all elements xi&gt;=0 which minimizes |X*x-y|.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Literature: Rasmus Bro and Sijmen De Jong, 'A fast non-negativity-constrained least squares algorithm', Journal of Chemometrics, Vol. 11, 393-401 (1997)
  /// </para>
  /// <para>
  /// Algorithm modified by Dirk Lellinger 2015 to allow a mixture of restricted and unrestricted parameters.
  /// </para>
  /// </remarks>
  public static class FastNonnegativeLeastSquares
  {
    /// <summary>
    /// Execution of the fast nonnegative least squares algorithm. The algorithm finds a vector x with all elements xi&gt;=0 which minimizes |X*x-y|.
    /// </summary>
    /// <param name="XtX">X transposed multiplied by X, thus a square matrix.</param>
    /// <param name="Xty">X transposed multiplied by Y, thus a matrix with one column and same number of rows as X.</param>
    /// <param name="isRestrictedToPositiveValues">Function that takes the parameter index as argument and returns true if the parameter at this index is restricted to positive values; otherwise the return value must be false.</param>
    /// <param name="tolerance">Used to decide if a solution element is less than or equal to zero. If this is null, a default tolerance of tolerance = MAX(SIZE(XtX)) * NORM(XtX,1) * EPS is used.</param>
    /// <param name="x">Output: solution vector (matrix with one column and number of rows according to dimension of X.</param>
    /// <param name="w">Output: Lagrange vector. Elements which take place in the fit are set to 0. Elements fixed to zero contain a negative number.</param>
    /// <remarks>
    /// <para>
    /// Literature: Rasmus Bro and Sijmen De Jong, 'A fast non-negativity-constrained least squares algorithm', Journal of Chemometrics, Vol. 11, 393-401 (1997)
    /// </para>
    /// <para>
    /// Algorithm modified by Dirk Lellinger 2015 to allow a mixture of restricted and unrestricted parameters.
    /// </para>
    /// </remarks>
    public static void Execution(IROMatrix<double> XtX, IROMatrix<double> Xty, Func<int, bool> isRestrictedToPositiveValues, double? tolerance, out IMatrix<double> x, out IMatrix<double> w)
    {
      if (XtX is null)
        throw new ArgumentNullException(nameof(XtX));
      if (Xty is null)
        throw new ArgumentNullException(nameof(Xty));
      if (isRestrictedToPositiveValues is null)
        throw new ArgumentNullException(nameof(isRestrictedToPositiveValues));

      if (XtX.RowCount != XtX.ColumnCount)
        throw new ArgumentException("Matrix should be a square matrix", nameof(XtX));
      if (Xty.ColumnCount != 1)
        throw new ArgumentException(nameof(Xty) + " should be a column vector (number of columns should be equal to 1)", nameof(Xty));
      if (Xty.RowCount != XtX.ColumnCount)
        throw new ArgumentException("Number of rows in " + nameof(Xty) + " should match number of columns in " + nameof(XtX), nameof(Xty));

      var matrixGenerator = new Func<int, int, DoubleMatrix>((rows, cols) => new DoubleMatrix(rows, cols));

      // if nargin < 3
      //   tol = 10 * eps * norm(XtX, 1) * length(XtX);
      // end
      double tol = tolerance.HasValue ? tolerance.Value : 10 * DoubleConstants.DBL_EPSILON * MatrixMath.Norm(XtX, MatrixNorm.M1Norm) * Math.Max(XtX.RowCount, XtX.ColumnCount);

      //	[m, n] = size(XtX);
      int n = XtX.ColumnCount;

      // P = zeros(1, n);
      // Z = 1:n;
      var P = new bool[n]; // POSITIVE SET: all indices which are currently not fixed are marked with TRUE (Negative set is simply this, but inverted)
      bool initializationOfSolutionRequired = false;
      for (int i = 0; i < n; ++i)
      {
        bool isNotRestricted = !isRestrictedToPositiveValues(i);
        P[i] = isNotRestricted;
        initializationOfSolutionRequired |= isNotRestricted;
      }

      // x = P';
      x = matrixGenerator(n, 1);

      // w = Xty-XtX*x;
      w = matrixGenerator(n, 1);
      MatrixMath.Copy(Xty, w);
      var helper_n_1 = matrixGenerator(n, 1);
      MatrixMath.Multiply(XtX, x, helper_n_1);
      MatrixMath.Subtract(w, helper_n_1, w);

      // set up iteration criterion
      int iter = 0;
      int itmax = 30 * n;

      // outer loop to put variables into set to hold positive coefficients
      // while any(Z) & any(w(ZZ) > tol)
      while (initializationOfSolutionRequired || (P.Any(ele => false == ele) && w.Any((r, c, ele) => false == P[r] && ele > tol)))
      {
        if (initializationOfSolutionRequired)
        {
          initializationOfSolutionRequired = false;
        }
        else
        {
          // [wt, t] = max(w(ZZ));
          // t = ZZ(t);
          int t = -1; // INDEX
          double wt = double.NegativeInfinity;
          for (int i = 0; i < n; ++i)
          {
            if (!P[i])
            {
              if (w[i, 0] > wt)
              {
                wt = w[i, 0];
                t = i;
              }
            }
          }

          // P(1, t) = t;
          // Z(t) = 0;
          P[t] = true;
        }

        // z(PP')=(Xty(PP)'/XtX(PP,PP)');
        var subXty = Xty.SubMatrix(P, 0, matrixGenerator); // Xty(PP)'
        var subXtX = XtX.SubMatrix(P, P, matrixGenerator);
        var solver = new DoubleLUDecomp(subXtX);
        var subSolution = solver.Solve(subXty);
        var z = matrixGenerator(n, 1);
        for (int i = 0, ii = 0; i < n; ++i)
          z[i, 0] = P[i] ? subSolution[ii++, 0] : 0;

        // C. Inner loop (to remove elements from the positive set which no longer belong to)
        while (z.Any((r, c, ele) => true == P[r] && ele <= tol && isRestrictedToPositiveValues(r)) && iter < itmax)
        {
          ++iter;
          // QQ = find((z <= tol) & P');
          //alpha = min(x(QQ)./ (x(QQ) - z(QQ)));
          double alpha = double.PositiveInfinity;
          for (int i = 0; i < n; ++i)
          {
            if ((z[i, 0] <= tol && true == P[i] && isRestrictedToPositiveValues(i)))
            {
              alpha = Math.Min(alpha, x[i, 0] / (x[i, 0] - z[i, 0]));
            }
          }
          // x = x + alpha * (z - x);
          for (int i = 0; i < n; ++i)
            x[i, 0] += alpha * (z[i, 0] - x[i, 0]);

          // ij = find(abs(x) < tol & P' ~= 0);
          // Z(ij) = ij';
          // P(ij) = zeros(1, length(ij));

          for (int i = 0; i < n; ++i)
          {
            if (Math.Abs(x[i, 0]) < tol && P[i] == true && isRestrictedToPositiveValues(i))
            {
              P[i] = false;
            }
          }

          //PP = find(P);
          //ZZ = find(Z);
          //nzz = size(ZZ);
          //z(PP) = (Xty(PP)'/XtX(PP,PP)');

          subXty = Xty.SubMatrix(P, 0, matrixGenerator);
          subXtX = XtX.SubMatrix(P, P, matrixGenerator);
          solver = new DoubleLUDecomp(subXtX);
          subSolution = solver.Solve(subXty);

          for (int i = 0, ii = 0; i < n; ++i)
            z[i, 0] = P[i] ? subSolution[ii++, 0] : 0;
        } // end inner loop

        MatrixMath.Copy(z, x);
        MatrixMath.Copy(Xty, w);
        MatrixMath.Multiply(XtX, x, helper_n_1);
        MatrixMath.Subtract(w, helper_n_1, w);
      }
    }

    /// <summary>
    /// Execution of the fast nonnegative least squares algorithm. The algorithm finds a vector x with all elements xi&gt;=0 which minimizes |X*x-y|.
    /// </summary>
    /// <param name="XtX">X transposed multiplied by X, thus a square matrix.</param>
    /// <param name="Xty">X transposed multiplied by Y, thus a matrix with one column and same number of rows as X.</param>
    /// <param name="tolerance">Used to decide if a solution element is less than or equal to zero. If this is null, a default tolerance of tolerance = MAX(SIZE(XtX)) * NORM(XtX,1) * EPS is used.</param>
    /// <param name="x">Output: solution vector (matrix with one column and number of rows according to dimension of X.</param>
    /// <param name="w">Output: Lagrange vector. Elements which take place in the fit are set to 0. Elements fixed to zero contain a negative number.</param>
    /// <remarks>
    /// <para>
    /// Literature: Rasmus Bro and Sijmen De Jong, 'A fast non-negativity-constrained least squares algorithm', Journal of Chemometrics, Vol. 11, 393-401 (1997)
    /// </para>
    /// <para>
    /// Algorithm modified by Dirk Lellinger 2015 to allow a mixture of restricted and unrestricted parameters.
    /// </para>
    /// </remarks>
    public static void Execution(IROMatrix<double> XtX, IROMatrix<double> Xty, double? tolerance, out IMatrix<double> x, out IMatrix<double> w)
    {
      Execution(XtX, Xty, (i) => true, tolerance, out x, out w);
    }
  }
}
