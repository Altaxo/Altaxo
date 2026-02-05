#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Provides methods for performing Fast Independent Component Analysis (FastICA) and related matrix whitening
  /// operations on data matrices.
  /// </summary>
  /// <remarks>
  /// This class includes implementations of the FastICA algorithm for blind source separation, as well as
  /// SVD-based whitening utilities.
  /// <para>Reference:</para>
  /// <para>[1] A. Hyvärinen and E. Oja, "A Fast Fixed-Point Algorithm for Independent Component Analysis," in Neural Computation, vol. 9, no. 7, pp. 1483-1492, 10 July 1997, doi:10.1162/neco.1997.9.7.1483.</para>
  /// </remarks>
  public record FactorizationByFastIndependentComponentAnalysis : ILowRankMatrixFactorization
  {
    /// <summary>
    /// Gets the maximum number of iterations for the factorization algorithm.
    /// </summary>
    public int MaximumNumberOfIterations
    {
      get => field;
      init
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException(nameof(MaximumNumberOfIterations), "Maximum number of iterations must be at least 1.");
        field = value;
      }
    } = 1000;


    /// <summary>
    /// Gets the convergence tolerance.
    /// </summary>
    /// <remarks>The default value of 1E-2 means that the iterations are stopped
    /// if the expected improvement of the relative error when the maximum number of iterations is reached would be less than 1E-2 times
    /// the current relative error.</remarks>
    public double Tolerance
    {
      get => field;
      init
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentOutOfRangeException(nameof(Tolerance), "Tolerance must be in the range (0, 1).");
        field = value;
      }
    } = 1E-6;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-26.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FactorizationByFastIndependentComponentAnalysis), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FactorizationByFastIndependentComponentAnalysis)obj;
        info.AddValue("MaximumNumberOfIterations", s.MaximumNumberOfIterations);
        info.AddValue("Tolerance", s.Tolerance);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maximumNumberOfIterations = info.GetInt32("MaximumNumberOfIterations");
        var tolerance = info.GetDouble("Tolerance");

        return ((o as FactorizationByFastIndependentComponentAnalysis) ?? new FactorizationByFastIndependentComponentAnalysis()) with
        {
          MaximumNumberOfIterations = maximumNumberOfIterations,
          Tolerance = tolerance,
        };
      }
    }

    #endregion


    /// <summary>
    /// Runs a symmetric FastICA implementation based on an eigenvalue-decomposition (EVD) whitening step.
    /// </summary>
    /// <param name="X">Input data matrix (samples x features). The matrix is centered in-place.</param>
    /// <param name="components">Number of independent components to estimate.</param>
    /// <param name="maxIter">Maximum number of iterations.</param>
    /// <param name="tol">Tolerance used as convergence criterion.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description><c>S</c>: estimated source signals (components x samples)</description></item>
    /// <item><description><c>W</c>: unmixing matrix (components x components)</description></item>
    /// <item><description><c>A</c>: mixing matrix (components x components), equal to <c>W</c><sup>-1</sup></description></item>
    /// </list>
    /// </returns>
    public static (Matrix<double> S, Matrix<double> W, Matrix<double> A) ICAWithEvdWhitening(Matrix<double> X, int components, int maxIter = 200, double tol = 1e-6)
    {
      int n = X.RowCount;
      int m = X.ColumnCount;

      // 1. Center
      var mean = X.ColumnSums() / n;
      for (int i = 0; i < n; i++)
        X.SetRow(i, X.Row(i) - mean);

      // 2. Whitening (PCA)
      var cov = (X.TransposeThisAndMultiply(X)) / n;
      var evd = cov.Evd();
      var D = evd.D;
      var E = evd.EigenVectors;

      var Dinv = Matrix<double>.Build.DiagonalOfDiagonalVector(
          D.Diagonal().Map(v => 1.0 / Math.Sqrt(v))
      );

      var whitening = Dinv * E.Transpose();
      var Xwhite = X * whitening.Transpose();

      // 3. ICA - symmetric FastICA
      var W = Matrix<double>.Build.Random(components, components);

      // preallocate matrices for the iteration loop
      var WX = Matrix<double>.Build.Dense(components, n);
      var G = Matrix<double>.Build.Dense(components, n);
      var Gp = Matrix<double>.Build.Dense(components, n);
      var GXwhite = Matrix<double>.Build.Dense(components, m);
      var GpRowSumsW = Matrix<double>.Build.Dense(components, m);
      var Wnew = Matrix<double>.Build.Dense(components, components);

      for (int iter = 0; iter < maxIter; iter++)
      {
        W.TransposeAndMultiply(Xwhite, WX);

        // g(u) = tanh(u)
        WX.Map(Math.Tanh, G);
        WX.Map(u => 1 - Math.Pow(Math.Tanh(u), 2), Gp);


        //var Wnew = (G * Xwhite) - Matrix<double>.Build.DiagonalOfDiagonalVector(Gp.RowSums()) * W;
        G.Multiply(Xwhite, GXwhite);
        Matrix<double>.Build.DiagonalOfDiagonalVector(Gp.RowSums()).Multiply(W, GpRowSumsW);
        GXwhite.Subtract(GpRowSumsW, Wnew);

        // Orthogonalize
        var svd = Wnew.Svd();
        svd.U.Multiply(svd.VT, Wnew);

        if ((Wnew - W).L2Norm() < tol)
          break;

        (W, Wnew) = (Wnew, W);
      }

      // 4. Independent components
      var S = W * Xwhite.Transpose();

      // 5. Mixing matrix A = W^-1
      var A = W.Inverse();

      return (S, W, A);
    }




    /// <summary>
    /// Performs SVD-based whitening with a heuristic choice between full and truncated SVD:
    /// <list type="bullet">
    /// <item><description>Full SVD when <paramref name="k" /> is large.</description></item>
    /// <item><description>Truncated SVD when <paramref name="k" /> is significantly smaller than <c>min(n, m)</c>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="X">Input data matrix (samples x features).</param>
    /// <param name="k">Number of principal components to keep.</param>
    /// <returns>
    /// A tuple containing the whitened data matrix, the retained right-singular vectors, and the retained singular values.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="k" /> is less than or equal to 0 or larger than <c>min(n, m)</c>.
    /// </exception>
    public static (Matrix<double> Xwhite, Matrix<double> V_k, Vector<double> S_k) WhiteningBySvd(Matrix<double> X, int k)
    {
      int n = X.RowCount;
      int m = X.ColumnCount;
      int minDim = Math.Min(n, m);

      if (k <= 0 || k > minDim)
        throw new ArgumentOutOfRangeException(nameof(k));

      // Heuristic: if k is significantly smaller than minDim, use truncated SVD
      bool useTruncated = k < minDim / 2;

      Matrix<double> V_k;
      Vector<double> S_k;

      if (useTruncated)
      {
        // ---- Truncated SVD path ----
        var (U_t, S_t, Vt_t) = TruncatedSVD.BlockKrylovSvd(X, k);

        V_k = Vt_t.Transpose(); // m x k
        S_k = S_t;              // k
      }
      else
      {
        // ---- Full SVD path ----
        var svd = X.Svd(computeVectors: true);

        var V = svd.VT.Transpose(); // m x r
        var Svals = svd.S;          // r

        V_k = V.SubMatrix(0, m, 0, k); // m x k
        S_k = Vector<double>.Build.Dense(k);
        for (int i = 0; i < k; i++)
          S_k[i] = Svals[i];
      }

      var Sinv_k = Matrix<double>.Build.DiagonalOfDiagonalVector(
          S_k.Map(v => 1.0 / v)
      ); // k x k

      var Xwhite = X * V_k * Sinv_k; // n x k

      return (Xwhite, V_k, S_k);
    }

    /// <summary>
    /// Runs a symmetric FastICA implementation using SVD-based whitening with a heuristic choice between
    /// full and truncated SVD.
    /// </summary>
    /// <param name="X">Input data matrix (samples x features). The matrix is centered in-place.</param>
    /// <param name="components">Number of independent components to estimate.</param>
    /// <param name="maxIter">Maximum number of iterations.</param>
    /// <param name="tol">Tolerance used as convergence criterion.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description><c>S</c>: estimated source signals (components x samples)</description></item>
    /// <item><description><c>W</c>: unmixing matrix (components x components)</description></item>
    /// <item><description><c>A</c>: mixing matrix (components x components), equal to <c>W</c><sup>-1</sup></description></item>
    /// </list>
    /// </returns>
    public static (Matrix<double> S, Matrix<double> W, Matrix<double> A) ICAWithSvdWhitening(
        Matrix<double> X,
        int components,
        int maxIter = 200,
        double tol = 1e-6)
    {
      int n = X.RowCount;

      // 1. Center
      var mean = X.ColumnSums() / n;
      for (int i = 0; i < n; i++)
        X.SetRow(i, X.Row(i) - mean);

      // 2. Whitening (with full/truncated SVD branching)
      var (Xwhite, V_k, S_k) = WhiteningBySvd(X, components);

      int k = components;
      var W = Matrix<double>.Build.Random(k, k);

      for (int iter = 0; iter < maxIter; iter++)
      {
        var WX = W * Xwhite.Transpose(); // k x n

        var G = WX.Map(Math.Tanh);
        var Gp = WX.Map(u => 1.0 - Math.Pow(Math.Tanh(u), 2.0));

        var Wnew = (G * Xwhite) / n;

        var gpMean = Gp.RowSums() / n;
        var Dgp = Matrix<double>.Build.DiagonalOfDiagonalVector(gpMean);
        Wnew -= Dgp * W;

        var svdW = Wnew.Svd(computeVectors: true);
        Wnew = svdW.U * svdW.VT;

        if ((Wnew - W).L2Norm() < tol)
        {
          W = Wnew;
          break;
        }

        W = Wnew;
      }

      var S_ic = W * Xwhite.Transpose();
      var A = W.Inverse();

      return new(S_ic, W, A);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This implementation centers the input matrix <paramref name="V"/> in-place.
    /// The returned factors therefore satisfy <c>X_centered ≈ W * H</c>.
    /// </remarks>
    public (Matrix<double> W, Matrix<double> H) Factorize(Matrix<double> V, int rank)
    {
      int n = V.RowCount;
      int m = V.ColumnCount;

      if (rank <= 0 || rank > Math.Min(n, m))
        throw new ArgumentOutOfRangeException(nameof(rank));

      // The ICA helper methods center the input matrix in-place.
      // For consistency with ILowRankMatrixFactorization, we return factors such that
      // the centered matrix is approximated by W * H.

      var X = V.Clone(); // work on a copy to avoid modifying the input

      // 1. Center
      var mean = X.ColumnSums() / n;
      for (int i = 0; i < n; i++)
        X.SetRow(i, X.Row(i) - mean);

      // 2. Whitening (with full/truncated SVD branching)
      var (Xwhite, V_k, S_k) = WhiteningBySvd(X, rank); // Xwhite = X * V_k * diag(1/S_k)

      // 3. ICA on whitened data
      var Wica = Matrix<double>.Build.Random(rank, rank);
      for (int iter = 0; iter < MaximumNumberOfIterations; iter++)
      {
        var WX = Wica * Xwhite.Transpose(); // rank x n

        var G = WX.Map(Math.Tanh);
        var Gp = WX.Map(u => 1.0 - Math.Pow(Math.Tanh(u), 2.0));

        var Wnew = (G * Xwhite) / n;

        var gpMean = Gp.RowSums() / n;
        var Dgp = Matrix<double>.Build.DiagonalOfDiagonalVector(gpMean);
        Wnew -= Dgp * Wica;

        var svdW = Wnew.Svd(computeVectors: true);
        Wnew = svdW.U * svdW.VT;

        if ((Wnew - Wica).L2Norm() < Tolerance)
        {
          Wica = Wnew;
          break;
        }

        Wica = Wnew;
      }

      // 4. Convert ICA result into a low-rank factorization of the (centered) original data.
      // Since Xwhite = X * V_k * diag(1/S_k), we can write:
      //   X ≈ Xwhite * ( Wica^{-1} * diag(S_k) * V_k^T )
      // and also rotate Xwhite by the ICA mixing matrix to get latent factors:
      //   W = Xwhite * A, where A = Wica^{-1}
      //   H = diag(S_k) * V_k^T
      // so that X ≈ W * H.
      var Aica = Wica.Inverse(); // mixing matrix in whitened space

      var W = Xwhite * Aica; // n x rank

      var diagS = Matrix<double>.Build.DiagonalOfDiagonalVector(S_k); // rank x rank
      var H = diagS * V_k.Transpose(); // rank x m

      return (W, H);
    }
  }
}

