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
  ///
  /// </summary>
  public static class TikhonovRegularizedNonnegativeMatrixFactorization
  {
    /// <summary>
    /// Factorize a nonnegative matrix A into two nonnegative matrices B and C so that A is nearly equal to B*C.
    /// Tikhonovs the nm f3.
    /// </summary>
    /// <param name="A">Matrix to factorize.</param>
    /// <param name="r">The number of factors.</param>
    /// <param name="B0">Original B matrix. Can be null.</param>
    /// <param name="C0">Original C matrix. Can be null.</param>
    /// <param name="oldalpha">The oldalpha.</param>
    /// <param name="oldbeta">The oldbeta.</param>
    /// <param name="gammaB">The gamma b.</param>
    /// <param name="gammaC">The gamma c.</param>
    /// <param name="maxiter">The maxiter.</param>
    /// <param name="tol">The tol.</param>
    public static void TikhonovNMF3(
          IMatrix<double> A,
          int r,
          IMatrix<double>? B0,
          IMatrix<double>? C0,
          IVector<double> oldalpha,
    IVector<double> oldbeta,
    IMatrix<double> gammaB,
    IMatrix<double> gammaC,
    int maxiter,
    double tol)
    {
      // The converged version of the algorithm
      // Use complementary slackness as stopping criterion
      // format long;
      // Check the input matrix

      if (A is null)
        throw new ArgumentNullException(nameof(A));

      if (MatrixMath.Min(A) < 0)
        throw new ArgumentException("Input matrix must not contain negative elements", nameof(A));

      int m = A.RowCount;
      int n = A.ColumnCount;
      // Check input arguments

      //if ˜exist(’r’)

      if (B0 is null)
      {
        B0 = Matrix<double>.Build.Random(m, r);
      }

      if (C0 is null)
      {
        C0 = Matrix<double>.Build.Random(r, n);
      }

      if (oldalpha is null)
      {
        oldalpha = CreateVector.Dense<double>(n);
      }

      if (oldbeta is null)
      {
        oldbeta = CreateVector.Dense<double>(m);
      }

      if (gammaB is null)
      {
        gammaB = CreateMatrix.Dense<double>(m, 1);
        gammaB.SetMatrixElements(0.1);    // small values lead to better convergence property
      }

      if (gammaC is null)
      {
        gammaC = CreateMatrix.Dense<double>(n, 1);
        gammaC.SetMatrixElements(0.1); // small values lead	to better convergence property
      }

      if (0 == maxiter)
        maxiter = 1000;

      if (double.IsNaN(tol) || tol <= 0)
        tol = 1.0e-9;

      var B = B0;
      B0 = null;
      var C = C0;
      C0 = null;
      var newalpha = oldalpha;
      var newbeta = oldbeta;

      var AtA = CreateMatrix.Dense<double>(n, n);
      MatrixMath.MultiplyFirstTransposed(A, A, AtA);
      double trAtA = MatrixMath.Trace(AtA);

      var olderror = CreateVector.Dense<double>(maxiter + 1);

      var BtA = CreateMatrix.Dense<double>(r, n);
      MatrixMath.MultiplyFirstTransposed(B, A, BtA);

      var CtBtA = CreateMatrix.Dense<double>(n, n);
      MatrixMath.MultiplyFirstTransposed(C, BtA, CtBtA);

      var BtB = CreateMatrix.Dense<double>(r, r);
      MatrixMath.MultiplyFirstTransposed(B, B, BtB);

      var BtBC = CreateMatrix.Dense<double>(r, n);
      MatrixMath.Multiply(BtB, C, BtBC);
      var CtBtBC = CreateMatrix.Dense<double>(n, n);
      MatrixMath.MultiplyFirstTransposed(C, BtBC, CtBtBC);

      var BtDgNewbeta = CreateMatrix.Dense<double>(r, m);
      MatrixMath.MultiplyFirstTransposed(B, CreateMatrix.DenseDiagonal(newbeta.Count, newbeta.Count, (i) => newbeta[i]), BtDgNewbeta);
      var BtDgNewbetaB = CreateMatrix.Dense<double>(r, r); // really rxr ?
      MatrixMath.Multiply(BtDgNewbeta, B, BtDgNewbetaB);

      var CDgNewalpha = CreateMatrix.Dense<double>(r, n);
      MatrixMath.Multiply(C, CreateMatrix.DenseDiagonal(newalpha.Count, newalpha.Count, (i) => newalpha[i]), CDgNewalpha);
      var CtCDgNewalpha = CreateMatrix.Dense<double>(n, n);
      MatrixMath.MultiplyFirstTransposed(C, CDgNewalpha, CtCDgNewalpha);

      olderror[0] =
        0.5 * trAtA -
        MatrixMath.Trace(CtBtA) +
        0.5 * MatrixMath.Trace(CtBtBC) +
        0.5 * MatrixMath.Trace(BtDgNewbetaB) +
        0.5 * MatrixMath.Trace(CtCDgNewalpha);

      double sigma = 1.0e-9;
      double delta = sigma;

      for (int iteration = 1; iteration <= maxiter; ++iteration)
      {
        var CCt = CreateMatrix.Dense<double>(r, r);
        MatrixMath.MultiplySecondTransposed(C, C, CCt);

        var gradB = CreateMatrix.Dense<double>(m, r);
        var tempMR = CreateMatrix.Dense<double>(m, r);
        //gradB = B*CCt - A*C’ +diag(newbeta)*B;
        MatrixMath.Multiply(B, CCt, gradB);
        MatrixMath.MultiplySecondTransposed(A, C, tempMR);
        MatrixMath.Add(gradB, tempMR, gradB);
        MatrixMath.Multiply(CreateMatrix.DenseDiagonal(newbeta.Count, newbeta.Count, (i) => newbeta[i]), B, tempMR);
        MatrixMath.Add(gradB, tempMR, gradB);

        // Bm = max(B, (gradB < 0)	*	sigma);
        var sigMR = CreateMatrix.Dense<double>(m, r);
        sigMR.SetMatrixElements((i, j) => gradB[i, j] < 0 ? sigma : 0);
        var Bm = CreateMatrix.Dense<double>(m, r);
        Bm.SetMatrixElements((i, j) => Math.Max(B[i, j], sigMR[i, j]));
      }
    }
  }
}
