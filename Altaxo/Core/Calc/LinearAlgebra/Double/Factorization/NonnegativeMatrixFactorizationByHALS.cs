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
  /// Non-negative matrix factorization (NMF) using hierarchical alternating least squares (HALS).
  /// </summary>
  /// <remarks>
  /// HALS is a block coordinate descent method that updates one component (a column of <c>W</c> and the corresponding row of <c>H</c>)
  /// at a time while enforcing non-negativity.
  /// <para>References:</para>
  /// <para>N. Gillis, "The why and how of nonnegative matrix factorization", arXiv:1401.5226.</para>
  /// </remarks>
  public record NonnegativeMatrixFactorizationByHALS : NonnegativeMatrixFactorizationWithRegularizationBase
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-24.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonnegativeMatrixFactorizationByHALS), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NonnegativeMatrixFactorizationByHALS)obj;
        info.AddValue("InitializationMethod", s.InitializationMethod);
        info.AddValue("MaximumNumberOfIterations", s.MaximumNumberOfIterations);
        info.AddValue("NumberOfTrials", s.NumberOfAdditionalTrials);
        info.AddValue("Tolerance", s.Tolerance);
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
        var lambdaW = info.GetDouble("LambdaW");
        var lambdaH = info.GetDouble("LambdaH");

        return ((o as NonnegativeMatrixFactorizationByHALS) ?? new NonnegativeMatrixFactorizationByHALS()) with
        {
          InitializationMethod = initializationMethod,
          MaximumNumberOfIterations = maximumNumberOfIterations,
          NumberOfAdditionalTrials = numberOfTrials,
          Tolerance = tolerance,
          LambdaW = lambdaW,
          LambdaH = lambdaH
        };
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> V, int rank)
    {
      ArgumentNullException.ThrowIfNull(V);

      int m = V.RowCount;
      int n = V.ColumnCount;

      double vNorm = V.FrobeniusNorm();
      if (vNorm == 0)
      {
        return (Matrix<double>.Build.Dense(m, rank), Matrix<double>.Build.Dense(rank, n));
      }
      // Used for efficient error computation: ||V - WH||_F^2 = ||V||_F^2 - 2*tr(Hᵀ Wᵀ V) + tr(Wᵀ W * H Hᵀ)
      double traceVᵀV = vNorm * vNorm;

      double epsilonForSqrtScale = 1e-12 * Math.Sqrt(vNorm); // Epsilon for W and H to avoid zero entries.
      double epsilonForScale = 1e-12 * vNorm; // Epsilon for WᵀW and HHᵀ to avoid zero entries.

      var (W, H) = InitializationMethod.GetInitialFactors(V, rank);

      // Enforce strict positivity (avoids divisions by zero and degeneracy).
      for (int i = 0; i < m; i++)
        for (int k = 0; k < rank; k++)
          if (W[i, k] < epsilonForSqrtScale)
            W[i, k] = epsilonForSqrtScale;

      for (int k = 0; k < rank; k++)
        for (int j = 0; j < n; j++)
          if (H[k, j] < epsilonForSqrtScale)
            H[k, j] = epsilonForSqrtScale;

      // Pre-allocations
      var WᵀW = Matrix<double>.Build.Dense(rank, rank);
      var HHᵀ = Matrix<double>.Build.Dense(rank, rank);
      var WᵀV = Matrix<double>.Build.Dense(rank, n);
      var VHᵀ = Matrix<double>.Build.Dense(m, rank);

      var errorHistory = new Chi2History(4, Tolerance, MaximumNumberOfIterations);

      for (int iIteration = 0; iIteration < MaximumNumberOfIterations; iIteration++)
      {
        // Keep shared products up-to-date.
        W.TransposeThisAndMultiply(W, WᵀW);
        W.TransposeThisAndMultiply(V, WᵀV);
        H.TransposeAndMultiply(H, HHᵀ); // only needed for error computation

        // Relative error (Frobenius) using trace identity ||V - WH||_F^2 = ||V||_F^2 - 2*tr(Hᵀ Wᵀ V) + tr(Wᵀ W * H Hᵀ) (see above)
        double error = Math.Sqrt(Math.Max(0, traceVᵀV - 2 * TraceOfTransposeAndMultiply(H, WᵀV) + TraceOfTransposeAndMultiply(HHᵀ, WᵀW))) / vNorm;
        var (terminate, bestW, bestH) = errorHistory.Add(error, W, H);
        if (terminate)
        {
          return (bestW!, bestH!);
        }

        // === Update H (row-wise / component-wise) ===
        for (int k = 0; k < rank; k++)
        {
          double denominator = WᵀW[k, k] + LambdaW;
          if (denominator <= 0)
          {
            denominator = epsilonForScale;
          }

          for (int j = 0; j < n; j++)
          {
            double numerator = WᵀV[k, j];
            for (int l = 0; l < rank; l++)
            {
              if (l != k)
              {
                numerator -= WᵀW[k, l] * H[l, j];
              }
            }

            double hNew = numerator / denominator;
            if (hNew < epsilonForSqrtScale)
            {
              hNew = epsilonForSqrtScale;
            }
            H[k, j] = hNew;
          }
        }

        // Refresh dependent quantities after changing H.
        H.TransposeAndMultiply(H, HHᵀ);
        V.TransposeAndMultiply(H, VHᵀ);

        // === Update W (column-wise / component-wise) ===
        for (int k = 0; k < rank; k++)
        {
          double denominator = HHᵀ[k, k] + LambdaH;
          if (denominator <= 0)
          {
            denominator = epsilonForScale;
          }

          for (int i = 0; i < m; i++)
          {
            double numerator = VHᵀ[i, k];
            for (int l = 0; l < rank; l++)
            {
              if (l != k)
              {
                numerator -= W[i, l] * HHᵀ[l, k];
              }
            }

            double wNew = numerator / denominator;
            if (wNew < epsilonForSqrtScale)
            {
              wNew = epsilonForSqrtScale;
            }
            W[i, k] = wNew;
          }
        }
      }
      return (W, H);
    }
  }
}
