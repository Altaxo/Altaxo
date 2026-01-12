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
using System.Linq;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{

  /// <summary>
  /// Provides initialization helpers for non-negative matrix factorization (NMF),
  /// specifically NNDSVD-based initializations.
  /// </summary>
  public class NonnegativeMatrixFactorizationBase
  {
    /// <summary>
    /// Creates an NNDSVD initialization for NMF. Intentionally allows zeros in the result (algorithm for sparse matrices).
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>
    /// A tuple <c>(W0, H0)</c> containing non-negative initial factors. Both factors may contain zeros; callers are expected
    /// to handle zeros as needed (e.g. by adding small offsets).
    /// </returns>
    /// <remarks>
    /// References: <see href="https://doi.org/10.1016/j.patcog.2007.09.010">Boutsidis, C., Gallopoulos, E., SVD based initialization: A head start for nonnegative matrix factorization, Pattern Recognition, Volume 41, Issue 4, April 2008, Pages 1350-1362</see>
    /// </remarks>
    public static (Matrix<double> W0, Matrix<double> H0) NNDSVDWithZerosPossibleInResult(Matrix<double> X, int r)
    {
      var svd = X.Svd(computeVectors: true); // TODO: replace by a partial SVD (psvd) of the first r factors
      var U = svd.U;          // m x m (or m x k)
      var S = svd.S;          // singular values as a Vector
      var Vt = svd.VT;        // n x n (or k x n)

      int m = X.RowCount;
      int n = X.ColumnCount;
      var W0 = Matrix<double>.Build.Dense(m, r);
      var H0 = Matrix<double>.Build.Dense(r, n);

      // First component
      var u0 = U.Column(0);
      var v0 = Vt.Row(0); // first row of V^T
      double s0 = S[0];
      var u0p = u0.PointwiseMaximum(0.0);
      var v0p = v0.PointwiseMaximum(0.0);
      double a0 = u0p.L2Norm();
      double b0 = v0p.L2Norm();
      if (a0 > 0 && b0 > 0)
      {
        W0.SetColumn(0, u0p / a0);
        H0.SetRow(0, v0p / b0 * (s0 * a0 * b0));
      }
      else
      {
        W0.SetColumn(0, u0.PointwiseAbs());
        H0.SetRow(0, v0.PointwiseAbs() * s0);
      }

      // Further components
      for (int j = 1; j < r && j < S.Count; j++)
      {
        var uj = U.Column(j);
        var vj = Vt.Row(j);
        double sj = S[j];

        var up = uj.PointwiseMaximum(0.0);
        var un = uj.PointwiseMinimum(0.0).PointwiseAbs();
        var vp = vj.PointwiseMaximum(0.0);
        var vn = vj.PointwiseMinimum(0.0).PointwiseAbs();

        double upNorm = up.L2Norm();
        double vpNorm = vp.L2Norm();
        double unNorm = un.L2Norm();
        double vnNorm = vn.L2Norm();

        // Choose the more positive variant
        var uComp = upNorm * vpNorm >= unNorm * vnNorm ? up : un;
        var vComp = upNorm * vpNorm >= unNorm * vnNorm ? vp : vn;

        double a = uComp.L2Norm();
        double b = vComp.L2Norm();
        if (a > 0)
          uComp = uComp / a;
        if (b > 0)
          vComp = vComp / b;

        W0.SetColumn(j, uComp);
        H0.SetRow(j, vComp * (sj * a * b));
      }

      return (W0, H0); // note that both W0 and H0 are non-negative, but can contain zeros! Those zeros should be handled in the calling function.
    }

    /// <summary>
    /// Creates an NNDSVD initialization for NMF. Intentionally allows zeros in the result (algorithm for sparse matrices).
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>
    /// A tuple <c>(W0, H0)</c> containing non-negative initial factors. Both factors may contain zeros; callers are expected
    /// to handle zeros as needed (e.g. by adding small offsets).
    /// </returns>
    /// <remarks>
    /// References: <see href="https://doi.org/10.1016/j.patcog.2007.09.010">Boutsidis, C., Gallopoulos, E., SVD based initialization: A head start for nonnegative matrix factorization, Pattern Recognition, Volume 41, Issue 4, April 2008, Pages 1350-1362</see>
    /// </remarks>
    public static (Matrix<double> W0, Matrix<double> H0) NNDSVDWithZerosPossibleInResultV02(Matrix<double> X, int r)
    {
      var svd = X.Svd(computeVectors: true); // TODO: replace by a partial SVD (psvd) of the first r factors,see e.g.https://scikit-learn.org/stable/modules/generated/sklearn.decomposition.TruncatedSVD.html# Math.NET Numerics Extended
      var U = svd.U;          // m x m (or m x k)
      var S = svd.S;          // singular values as a Vector
      var Vt = svd.VT;        // n x n (or k x n)

      int m = X.RowCount;
      int n = X.ColumnCount;
      var W = Matrix<double>.Build.Dense(m, r);
      var H = Matrix<double>.Build.Dense(r, n);

      // First component
      // Note that the first component can be used directly
      W.SetColumn(0, U.Column(0).PointwiseAbs() * Math.Sqrt(S[0]));
      H.SetRow(0, Vt.Row(0).PointwiseAbs() * Math.Sqrt(S[0]));

      // Further components
      for (int j = 1; j < r && j < S.Count; j++)
      {
        var xj = U.Column(j);
        var yj = Vt.Row(j);
        double sj = S[j];

        var xp = xj.PointwiseMaximum(0.0);
        var xn = xj.PointwiseMinimum(0.0).PointwiseAbs();
        var yp = yj.PointwiseMaximum(0.0);
        var yn = yj.PointwiseMinimum(0.0).PointwiseAbs();

        double xpnrm = xp.L2Norm();
        double ypnrm = yp.L2Norm();
        double mp = xpnrm * ypnrm;
        double xnnrm = xn.L2Norm();
        double ynnrm = yn.L2Norm();
        double mn = xnnrm * ynnrm;

        // Choose the more positive variant
        Vector<double> u, v;
        if (mp > mn)
        {
          u = xp * (Math.Sqrt(sj * mp) / xpnrm);
          v = yp * (Math.Sqrt(sj * mp) / ypnrm);
        }
        else
        {
          u = xn * (Math.Sqrt(sj * mn) / xnnrm);
          v = yn * (Math.Sqrt(sj * mn) / ynnrm);
        }

        W.SetColumn(j, u);
        H.SetRow(j, v);
      }

      return (W, H); // note that both W0 and H0 are non-negative, but can contain zeros! Those zeros should be handled in the calling function.
    }

    /// <summary>
    /// Creates an NNDSVD initialization for NMF and replaces zeros by a small positive value.
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>A tuple <c>(W0, H0)</c> containing strictly positive initial factors.</returns>
    ///     /// <remarks>
    /// References: <see href="https://doi.org/10.1016/j.patcog.2007.09.010">Boutsidis, C., Gallopoulos, E., SVD based initialization: A head start for nonnegative matrix factorization, Pattern Recognition, Volume 41, Issue 4, April 2008, Pages 1350-1362</see>
    /// </remarks>
    public static (Matrix<double> W0, Matrix<double> H0) NNDSVD(Matrix<double> X, int r)
    {
      var (W0, H0) = NNDSVDWithZerosPossibleInResult(X, r);

      // Small offsets to avoid zeros
      W0 = W0.PointwiseMaximum(1e-12);
      H0 = H0.PointwiseMaximum(1e-12);
      return (W0, H0);
    }

    /// <summary>
    /// Creates an NNDSVDa initialization for NMF by replacing zeros with a data-dependent small value.
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>A tuple <c>(W0, H0)</c> containing non-negative initial factors with zeros replaced by a small value.</returns>
    public static (Matrix<double> W0, Matrix<double> H0) NNDSVDa(Matrix<double> X, int r)
    {
      // First, create the standard NNDSVD initialization
      var (W0, H0) = NNDSVDWithZerosPossibleInResult(X, r);

      // Mean of the data matrix (positive values only)
      double avg = X.Enumerate().Where(v => v > 0).DefaultIfEmpty(0.0).Average();
      double eps = avg * 1e-4; // paper Boutsidis & Gallopoulos, 2008, https://doi.org/10.1016/j.patcog.2007.09.010

      // If avg == 0 (extremely rare), fallback
      if (eps == 0.0)
        eps = 1e-4;

      // Replace all zeros
      for (int i = 0; i < W0.RowCount; i++)
        for (int j = 0; j < W0.ColumnCount; j++)
          if (W0[i, j] == 0.0)
            W0[i, j] = eps;

      for (int i = 0; i < H0.RowCount; i++)
        for (int j = 0; j < H0.ColumnCount; j++)
          if (H0[i, j] == 0.0)
            H0[i, j] = eps;

      return (W0, H0);
    }

    /// <summary>
    /// Creates an NNDSVDar initialization for NMF by applying NNDSVDa and adding small random noise.
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>A tuple <c>(W0, H0)</c> containing non-negative initial factors with small random perturbations.</returns>
    /// <remarks>
    /// References: <see href="https://doi.org/10.1016/j.patcog.2007.09.010">Boutsidis, C., Gallopoulos, E., SVD based initialization: A head start for nonnegative matrix factorization, Pattern Recognition, Volume 41, Issue 4, April 2008, Pages 1350-1362</see>
    /// </remarks>
    public static (Matrix<double> W0, Matrix<double> H0) NNDSVDar(Matrix<double> X, int r)
    {
      // First, create NNDSVDa
      var (W0, H0) = NNDSVDa(X, r);

      // Mean of the data matrix (positive values only)
      double avg = X.Enumerate().Where(v => v > 0).DefaultIfEmpty(0.0).Average();
      double eps = avg * 1e-4;

      if (eps == 0.0)
        eps = 1e-4;

      // Random number generator
      var rnd = System.Random.Shared;

      // Add random noise
      for (int i = 0; i < W0.RowCount; i++)
        for (int j = 0; j < W0.ColumnCount; j++)
          W0[i, j] += rnd.NextDouble() * eps;

      for (int i = 0; i < H0.RowCount; i++)
        for (int j = 0; j < H0.ColumnCount; j++)
          H0[i, j] += rnd.NextDouble() * eps;

      return (W0, H0);
    }

  }
}
