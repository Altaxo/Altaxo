#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  /// <summary>
  /// Non-negative matrix factorization (NMF) using the classic multiplicative update rules.
  /// </summary>
  public class NonnegativeMatrixFactorizationByMultiplicativeUpdate : NonnegativeMatrixFactorizationBase
  {
    /// <summary>
    /// Factorizes a non-negative matrix <paramref name="V"/> into non-negative factors <c>W</c> and <c>H</c> using multiplicative updates.
    /// </summary>
    /// <param name="V">The input matrix to factorize.</param>
    /// <param name="r">The factorization rank.</param>
    /// <param name="maxIter">The maximum number of iterations per restart.</param>
    /// <param name="tol">The stopping tolerance (change in relative error).</param>
    /// <param name="restarts">The number of restarts with different initializations.</param>
    /// <returns>
    /// A tuple containing the factors <c>W</c> and <c>H</c> and the final relative reconstruction error <c>relErr</c>.
    /// </returns>
    public static (Matrix<double> W, Matrix<double> H, double relErr) Evaluate(
    Matrix<double> V, int r, int maxIter = 2000, double tol = 1e-5, int restarts = 3)
    {
      int m = V.RowCount;
      int n = V.ColumnCount;
      double eps = 1e-12;
      double bestErr = double.PositiveInfinity;
      Matrix<double>? bestW = null, bestH = null;

      for (int trial = 0; trial < restarts; trial++)
      {
        // Initialization: NNDSVD for the first trial; otherwise random
        Matrix<double> W, H;
        if (trial == 0)
        {
          (W, H) = NNDSVDar(V, r);
        }
        else
        {
          W = Matrix<double>.Build.Random(m, r).PointwiseAbs().PointwiseMaximum(eps);
          H = Matrix<double>.Build.Random(r, n).PointwiseAbs().PointwiseMaximum(eps);
        }

        double prevErr = double.PositiveInfinity;
        double vNorm = V.FrobeniusNorm();

        for (int iter = 0; iter < maxIter; iter++)
        {
          var WH = W * H;

          // Update H
          var numeratorH = W.Transpose() * V;
          var denominatorH = W.Transpose() * WH + eps;
          H = H.PointwiseMultiply(numeratorH.PointwiseDivide(denominatorH));

          // Update W
          WH = W * H;
          var numeratorW = V * H.Transpose();
          var denominatorW = WH * H.Transpose() + eps;
          W = W.PointwiseMultiply(numeratorW.PointwiseDivide(denominatorW));

          // Stabilization
          W = W.PointwiseMaximum(eps);
          H = H.PointwiseMaximum(eps);

          // Optional: rescaling for conditioning
          for (int k = 0; k < r; k++)
          {
            double normWk = W.Column(k).L2Norm();
            if (normWk > 0)
            {
              W.SetColumn(k, W.Column(k) / normWk);
              H.SetRow(k, H.Row(k) * normWk);
            }
          }

          // Monitoring
          var Vhat = W * H;
          double err = (V - Vhat).FrobeniusNorm() / vNorm;

          if (Math.Abs(prevErr - err) < tol) break;
          prevErr = err;
        }

        double finalErr = (V - (W * H)).FrobeniusNorm() / vNorm;
        if (finalErr < bestErr)
        {
          bestErr = finalErr; bestW = W; bestH = H;
        }
      }

      return (bestW, bestH, bestErr);
    }
  }
}
