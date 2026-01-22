using System;
using System.Collections.Generic;
using Altaxo.Calc.Distributions;
using Altaxo.Calc.LinearAlgebra.Factorization;


namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  public class TruncatedSVD
  {
    public static (Matrix<double> U, Vector<double> S, Matrix<double> Vt)
    RandomizedSvd(Matrix<double> A, int k, int oversampling = 10, int powerIter = 2)
    {
      int m = A.RowCount;
      int n = A.ColumnCount;
      int l = k + oversampling; // target subspace dimension

      var scale = A.FrobeniusNorm();
      scale = Math.ScaleB(1, Math.ILogB(scale) - 1); // avoid numeric accuracy issues by using only powers-of-two scaling
      A = A / scale; // Skalierung für numerische Stabilität

      // 1) Random Gaussian test matrix
      var random = new Normal(0.0, 1.0); // Mittelwert 0, Std-Abw. 1
      var Omega = DenseMatrix.CreateRandom(n, l, random);

      // 2) Sample Y = A * Omega
      var Y = A * Omega;

      // 3) Power iterations (stabilisiert bei langsamem Spektrum)
      for (int i = 0; i < powerIter; i++)
      {
        Y = A * (A.TransposeThisAndMultiply(Y));
      }

      // 4) Orthonormalbasis Q via QR
      var qr = Y.QR(QRMethod.Thin);
      var Q = qr.Q;

      // 5) B = Qᵀ * A  (kleine Matrix)
      var B = Q.TransposeThisAndMultiply(A);

      // 6) SVD von B
      var svd = B.Svd(computeVectors: true);

      // 7) Rekonstruktion der linken Singulärvektoren U = Q * U_B
      var U = Q * svd.U.SubMatrix(0, Q.ColumnCount, 0, k);
      var S = svd.S.SubVector(0, k);
      var Vt = svd.VT.SubMatrix(0, k, 0, n);

      S = S * scale; // Rückskalierung der Singulärwerte

      return (U, S, Vt);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="A"></param>
    /// <param name="k"></param>
    /// <param name="oversampling"></param>
    /// <param name="q"></param>
    /// <returns></returns>
    /// <remarks>
    /// Reference: https://people.cs.umass.edu/~cmusco/personal_site/pdfs/blockKrylov.pdf
    /// </remarks>
    public static (Matrix<double> U, Vector<double> S, Matrix<double> Vt)
        BlockKrylovSvd(Matrix<double> A, int k, int oversampling = 10, int q = 2)
    {
      int m = A.RowCount;
      int n = A.ColumnCount;


      if (!(k <= Math.Min(m, n)))
        throw new ArgumentException("Target rank k must be less than or equal to Min(A.RowCount,A.ColumnCount).");

      // decrease q if neccessary
      if ((q + 1) * k > Math.Min(m, n))
      {
        q = (Math.Min(m, n) / k) - 1;
      }

      // decrease oversampling if neccessary
      if ((q + 1) * (k + oversampling) > Math.Min(m, n))
      {
        oversampling = (Math.Min(m, n) / (q + 1)) - k;
      }
      int l = k + oversampling;


      // 1) Zufallsmatrix (Gaussian)
      var gaussian = new Normal(0.0, 1.0);
      var Omega = DenseMatrix.CreateRandom(n, l, gaussian);

      // 2) Y0 = A * Omega
      var Y = A * Omega;

      // 3) Block-Krylov Raum aufbauen
      //    K = [Y, A Aᵀ Y, (A Aᵀ)² Y, ...]
      var KrylovBlocks = new List<Matrix<double>>();
      KrylovBlocks.Add(Y);

      var Z = Y;

      for (int i = 0; i < q; i++)
      {
        // Z = A * (Aᵀ * Z)
        Z = A * (A.TransposeThisAndMultiply(Z));
        KrylovBlocks.Add(Z);
      }

      // 4) Alle Blöcke horizontal konkatenieren
      var K = KrylovBlocks[0];
      for (int i = 1; i < KrylovBlocks.Count; i++)
        K = K.Append(KrylovBlocks[i]);

      // 5) QR-Orthonormalisierung
      var qr = K.QR(QRMethod.Thin);
      var Q = qr.Q;

      // 6) Kleine Matrix B = Qᵀ * A
      var B = Q.TransposeThisAndMultiply(A);

      // 7) SVD von B
      var svd = B.Svd(true);

      // 8) Rekonstruktion der linken Singulärvektoren
      var U = Q * svd.U.SubMatrix(0, Q.ColumnCount, 0, k);
      var S = svd.S.SubVector(0, k);
      var Vt = svd.VT.SubMatrix(0, k, 0, n);

      return (U, S, Vt);
    }

  }
}
