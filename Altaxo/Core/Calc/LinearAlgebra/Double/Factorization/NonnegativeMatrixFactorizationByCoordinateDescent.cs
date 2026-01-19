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

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Non-negative matrix factorization (NMF) using an alternating coordinate-descent style update with non-negativity enforcement.
  /// </summary>
  public record NonnegativeMatrixFactorizationByCoordinateDescent : NonnegativeMatrixFactorizationWithRegularizationBase
  {


    /// <summary>
    /// Gets the damping factor used when a proposed update increases the error.
    /// </summary>
    public double Damping
    {
      get => field;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(Damping), "Damping must be >0 and <1.");
        field = value;
      }
    } = 0.5;    // damping on error increase (0..1)

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonnegativeMatrixFactorizationByCoordinateDescent), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NonnegativeMatrixFactorizationByCoordinateDescent)obj;
        info.AddValue("InitializationMethod", s.InitializationMethod);
        info.AddValue("MaximumNumberOfIterations", s.MaximumNumberOfIterations);
        info.AddValue("NumberOfTrials", s.NumberOfAdditionalTrials);
        info.AddValue("Tolerance", s.Tolerance);
        info.AddValue("Damping", s.Damping);
        info.AddValue("LambdaW", s.LambdaW);
        info.AddValue("LambdaH", s.LambdaH);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var initializationMethod = info.GetValue<INonnegativeMatrixFactorizationInitializer>("InitializationMethod", parent);
        var maximumNumberOfIterations = info.GetInt32("MaximumNumberOfIterations");
        var numberOfTrials = info.GetInt32("NumberOfTrials");
        var tolerance = info.GetDouble("Tolerance");
        var damping = info.GetDouble("Damping");
        var lambdaW = info.GetDouble("LambdaW");
        var lambdaH = info.GetDouble("LambdaH");

        return ((o as NonnegativeMatrixFactorizationByCoordinateDescent) ?? new NonnegativeMatrixFactorizationByCoordinateDescent()) with
        {
          InitializationMethod = initializationMethod,
          MaximumNumberOfIterations = maximumNumberOfIterations,
          NumberOfAdditionalTrials = numberOfTrials,
          Tolerance = tolerance,
          Damping = damping,
          LambdaW = lambdaW,
          LambdaH = lambdaH
        };
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> X, int Rank)
    {
      int m = X.RowCount;
      int n = X.ColumnCount;
      double eps = 1e-12;

      // Initialization: non-negative starting values
      var (W, H) = InitializationMethod.GetInitialFactors(X, Rank);

      // Optional initial rescaling
      RescaleFactors(W, H, eps);

      double vNorm = X.FrobeniusNorm();
      double prevRelErr = double.PositiveInfinity;

      for (int iter = 0; iter < MaximumNumberOfIterations; iter++)
      {
        // === Update H (column-wise) ===
        var WtW = W.TransposeThisAndMultiply(W);
        WtW = AddDiagonal(WtW, LambdaW); // regularization

        var H_proposed = H.Clone();
        for (int j = 0; j < n; j++)
        {
          Vector<double> vj = X.Column(j);                 // m-vector
          Vector<double> rhsH = W.TransposeThisAndMultiply(vj); // r-vector
          Vector<double> hj = WtW.Solve(rhsH);             // r-vector
          for (int k = 0; k < Rank; k++) hj[k] = Math.Max(hj[k], eps);
          H_proposed.SetColumn(j, hj);
        }

        // Safeguard: accept only if error does not increase; otherwise apply damping
        double errBeforeH = RelativeError(X, W, H, vNorm);
        double errAfterHProposed = RelativeError(X, W, H_proposed, vNorm);

        if (errAfterHProposed <= errBeforeH)
          H = H_proposed;
        else
          H = Blend(H, H_proposed, Damping); // damped update

        H = H.PointwiseMaximum(eps);

        // Rescaling after H update
        RescaleFactors(W, H, eps);

        // === Update W (row-wise) ===
        var HHT = H * H.Transpose();
        HHT = AddDiagonal(HHT, LambdaH); // regularization

        var W_proposed = W.Clone();
        for (int i = 0; i < m; i++)
        {
          Vector<double> vi = X.Row(i);   // n-vector
          Vector<double> rhsW = H * vi;   // r-vector
          Vector<double> wi = HHT.Solve(rhsW); // r-vector
          for (int k = 0; k < Rank; k++) wi[k] = Math.Max(wi[k], eps);
          W_proposed.SetRow(i, wi);
        }

        // Safeguard: check W update
        double errBeforeW = RelativeError(X, W, H, vNorm);
        double errAfterWProposed = RelativeError(X, W_proposed, H, vNorm);

        if (errAfterWProposed <= errBeforeW)
          W = W_proposed;
        else
          W = Blend(W, W_proposed, Damping);

        W = W.PointwiseMaximum(eps);

        // Rescaling after W update
        RescaleFactors(W, H, eps);

        // Monitoring & stopping criterion
        double relErr = RelativeError(X, W, H, vNorm);
        if (iter % 10 == 0)
          Console.WriteLine($"Iter {iter}: relErr = {relErr:E6}");

        if (Math.Abs(prevRelErr - relErr) < Tolerance)
          break;

        prevRelErr = relErr;
      }

      return (W, H);
    }



    // Helper functions

    /// <summary>
    /// Adds <paramref name="lambda"/> to the diagonal of <paramref name="A"/>.
    /// </summary>
    /// <param name="A">The matrix to regularize.</param>
    /// <param name="lambda">The diagonal value to add.</param>
    /// <returns>The regularized matrix.</returns>
    private static Matrix<double> AddDiagonal(Matrix<double> A, double lambda)
    {
      if (lambda <= 0) return A;
      var D = Matrix<double>.Build.DenseDiagonal(A.RowCount, A.ColumnCount, lambda);
      return A + D;
    }

    /// <summary>
    /// Rescales columns of <paramref name="W"/> to unit L2 norm and compensates the scaling in the corresponding rows of <paramref name="H"/>.
    /// Floors elements to <paramref name="eps"/> to keep values strictly positive.
    /// </summary>
    /// <param name="W">The left factor matrix.</param>
    /// <param name="H">The right factor matrix.</param>
    /// <param name="eps">The minimum allowed value.</param>
    private static void RescaleFactors(Matrix<double> W, Matrix<double> H, double eps)
    {
      int r = W.ColumnCount;
      for (int k = 0; k < r; k++)
      {
        double norm = W.Column(k).L2Norm();
        if (norm > 0)
        {
          W.SetColumn(k, W.Column(k) / norm);
          H.SetRow(k, H.Row(k) * norm);
        }
        // Floors
        W.SetColumn(k, W.Column(k).PointwiseMaximum(eps));
        H.SetRow(k, H.Row(k).PointwiseMaximum(eps));
      }
    }

    /// <summary>
    /// Computes the relative reconstruction error <c>||V - W*H||_F / ||V||_F</c>.
    /// </summary>
    /// <param name="V">The input matrix.</param>
    /// <param name="W">The left factor matrix.</param>
    /// <param name="H">The right factor matrix.</param>
    /// <param name="vNorm">The Frobenius norm of <paramref name="V"/>.</param>
    /// <returns>The relative reconstruction error.</returns>
    private static double RelativeError(Matrix<double> V, Matrix<double> W, Matrix<double> H, double vNorm)
    {
      var Vhat = W * H;
      return (V - Vhat).FrobeniusNorm() / vNorm;
    }

    /// <summary>
    /// Computes a convex combination of <paramref name="A"/> and <paramref name="B"/>.
    /// </summary>
    /// <param name="A">The first matrix.</param>
    /// <param name="B">The second matrix.</param>
    /// <param name="alpha">The blend factor.</param>
    /// <returns>The blended matrix.</returns>
    private static Matrix<double> Blend(Matrix<double> A, Matrix<double> B, double alpha)
    {
      // A_new = (1 - alpha) * A + alpha * B
      return A * (1.0 - alpha) + B * alpha;
    }

  }
}
