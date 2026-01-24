using System;
using Altaxo.Calc.Distributions;
using Altaxo.Calc.LinearAlgebra.Factorization;


namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Provides randomized and Block Krylov based truncated singular value decomposition (SVD)
  /// routines and exposes a low-rank matrix factorization API.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Musco, C. et al., "Randomized Block Krylov Methods for Stronger and Faster Approximate Singular Value Decomposition", https://people.cs.umass.edu/~cmusco/personal_site/pdfs/blockKrylov.pdf</para></remarks>
  public record TruncatedSVD : ILowRankMatrixFactorization
  {
    /// <summary>
    /// Gets the oversampling parameter used to improve the quality of the sampled subspace.
    /// </summary>
    public int Oversampling { get; init; } = 10;

    /// <summary>
    /// Gets the number of power iterations used to improve accuracy when the singular
    /// spectrum decays slowly.
    /// </summary>
    public int PowerIterations { get; init; } = 2;

    /// <inheritdoc/>
    public (Matrix<double> W, Matrix<double> H) Factorize(Matrix<double> X, int rank)
    {
      var (U, S, Vt) = BlockKrylovSvd(X, rank, Oversampling, PowerIterations);

      for (int j = 0; j < rank; ++j)
      {
        double s = S[j];
        U.SetColumn(j, U.Column(j) * Math.Sqrt(s));
        Vt.SetRow(j, Vt.Row(j) * Math.Sqrt(s));
      }
      return (U, Vt);
    }

    /// <summary>
    /// Computes a truncated singular value decomposition (SVD) using a randomized range finder.
    /// </summary>
    /// <param name="A">Input matrix.</param>
    /// <param name="k">Target rank (number of singular triplets to return).</param>
    /// <param name="oversampling">Additional sampling dimensions to improve accuracy.</param>
    /// <param name="powerIterations">Number of power iterations to improve accuracy.</param>
    /// <returns>The truncated SVD <c>(U, S, VT)</c> of <paramref name="A"/>.</returns>
    public static (Matrix<double> U, Vector<double> S, Matrix<double> VT) RandomizedSvd(Matrix<double> A, int k, int oversampling = 10, int powerIterations = 2)
    {
      int m = A.RowCount;
      int n = A.ColumnCount;

      if (!(k <= Math.Min(m, n)))
        throw new ArgumentException("Target rank k must be less than or equal to Min(A.RowCount,A.ColumnCount).");

      // decrease powerIterations if necessary
      if ((powerIterations + 1) * k > Math.Min(m, n))
      {
        powerIterations = (Math.Min(m, n) / k) - 1;
      }

      // decrease oversampling if necessary
      if ((powerIterations + 1) * (k + oversampling) > Math.Min(m, n))
      {
        oversampling = (Math.Min(m, n) / (powerIterations + 1)) - k;
      }
      int k_plus_o = k + oversampling; // target subspace dimension

      var scale = A.FrobeniusNorm();
      scale = Math.ScaleB(1, Math.ILogB(scale) - 1); // avoid numeric accuracy issues by using only powers-of-two scaling

      A = A / scale; // scaling for numerical stability

      // 1) Random Gaussian test matrix
      var gaussian = new Normal(0, 1); // mean 0, standard deviation 1
      var Omega = DenseMatrix.CreateRandom(n, k_plus_o, gaussian);

      var Y = CreateMatrix.Dense<double>(m, k_plus_o);


      // 2) Sample Y = A * Omega
      A.Multiply(Omega, Y); // Y = A * Omega

      // 3) Power iterations (stabilizes when the singular spectrum decays slowly)
      if (powerIterations > 0)
      {
        var Z = CreateMatrix.Dense<double>(n, k_plus_o);
        for (int i = 1; i <= powerIterations; i++)
        {
          // Y = A * (Aᵀ * Y)
          A.TransposeThisAndMultiply(Y, Z); // Z = Aᵀ * Y
          A.Multiply(Z, Y);                 // Y = A * Z  = A * (Aᵀ * Z)
        }
      }

      // 4) Orthonormal basis Q via QR
      var qr = Y.QR(QRMethod.Thin);
      var Q = qr.Q;

      // 5) B = Qᵀ * A  (small matrix)
      var B = Q.TransposeThisAndMultiply(A);

      // 6) SVD of B
      var svd = B.Svd(computeVectors: true);

      // 7) Reconstruction of the left singular vectors U = Q * U_B
      var U = Q * svd.U.SubMatrix(0, Q.ColumnCount, 0, k);
      var S = svd.S.SubVector(0, k) * scale; // scale back the singular values  
      var Vt = svd.VT.SubMatrix(0, k, 0, n);

      return (U, S, Vt);
    }

    /// <summary>
    /// Computes a truncated SVD using the Block Krylov method.
    /// </summary>
    /// <param name="A">Input matrix.</param>
    /// <param name="k">Target rank (number of singular triplets to return).</param>
    /// <param name="oversampling">Additional sampling dimensions to improve accuracy.</param>
    /// <param name="powerIterations">Number of Krylov iterations (power iterations in the Block Krylov space).</param>
    /// <returns>
    /// The truncated SVD <c>(U, S, Vt)</c>, where <c>A ≈ U * diag(S) * Vt</c>.
    /// </returns>
    /// <remarks>
    /// Reference: https://people.cs.umass.edu/~cmusco/personal_site/pdfs/blockKrylov.pdf
    /// </remarks>
    public static (Matrix<double> U, Vector<double> S, Matrix<double> VT) BlockKrylovSvd(Matrix<double> A, int k, int oversampling = 10, int powerIterations = 2)
    {
      int m = A.RowCount;
      int n = A.ColumnCount;

      if (!(k <= Math.Min(m, n)))
        throw new ArgumentException("Target rank k must be less than or equal to Min(A.RowCount,A.ColumnCount).");

      // decrease powerIterations if necessary
      if ((powerIterations + 1) * k > Math.Min(m, n))
      {
        powerIterations = (Math.Min(m, n) / k) - 1;
      }

      // decrease oversampling if necessary
      if ((powerIterations + 1) * (k + oversampling) > Math.Min(m, n))
      {
        oversampling = (Math.Min(m, n) / (powerIterations + 1)) - k;
      }
      int k_plus_o = k + oversampling;


      var Y = CreateMatrix.Dense<double>(m, k_plus_o);
      var K = Y; // placeholder for the Krylov matrix (with no power iterations, Y and K are identical)

      // 1) Random matrix (Gaussian)
      var gaussian = new Normal(0, 1);
      var Omega = DenseMatrix.CreateRandom(n, k_plus_o, gaussian);

      // 2) Y0 = A * Omega
      A.Multiply(Omega, Y); // Y = A * Omega

      if (powerIterations > 0)
      {
        K = CreateMatrix.Dense<double>(m, k_plus_o * (powerIterations + 1)); // create the Krylov matrix K
        // Store Y at the first block of K
        K.SetSubMatrix(0, 0, Y);

        var Z = CreateMatrix.Dense<double>(n, k_plus_o);
        for (int i = 1; i <= powerIterations; i++)
        {
          // Y = A * (Aᵀ * Y)
          A.TransposeThisAndMultiply(Y, Z); // Z = Aᵀ * Y
          A.Multiply(Z, Y);             // Y = A * Z  = A * (Aᵀ * Z)
          K.SetSubMatrix(0, i * k_plus_o, Y); // Store Y at the i-th block of K
        }
      }

      // 5) QR orthonormalization
      var qr = K.QR(QRMethod.Thin);
      var Q = qr.Q;

      // 6) Small matrix B = Qᵀ * A
      var B = Q.TransposeThisAndMultiply(A); // allocation here is OK: B is not calculated repeatedly

      // 7) SVD of B
      var svd = B.Svd(true);

      // 8) Reconstruction of the left singular vectors
      var U = Q * svd.U.SubMatrix(0, Q.ColumnCount, 0, k);
      var S = svd.S.SubVector(0, k);
      var Vt = svd.VT.SubMatrix(0, k, 0, n);

      return (U, S, Vt);
    }
  }
}
