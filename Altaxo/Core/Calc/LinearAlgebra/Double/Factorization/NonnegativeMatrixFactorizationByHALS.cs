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
      double eps = 1e-12;

      double xNorm = V.FrobeniusNorm();
      if (xNorm == 0)
      {
        return (Matrix<double>.Build.Dense(m, rank), Matrix<double>.Build.Dense(rank, n));
      }
      // Used for efficient error computation: ||X - WH||_F^2 = ||X||_F^2 - 2*tr(H^T W^T X) + tr(W^T W * H H^T)
      double traceXtX = NonnegativeMatrixFactorizationBase.TraceOfTransposeAndMultiply(V, V);

      var (W, H) = InitializationMethod.GetInitialFactors(V, rank);

      // Enforce strict positivity (avoids divisions by zero and degeneracy).
      for (int i = 0; i < m; i++)
        for (int k = 0; k < rank; k++)
          if (W[i, k] < eps)
            W[i, k] = eps;

      for (int k = 0; k < rank; k++)
        for (int j = 0; j < n; j++)
          if (H[k, j] < eps)
            H[k, j] = eps;



      // Pre-allocations
      var WᵀW = Matrix<double>.Build.Dense(rank, rank);
      var HHᵀ = Matrix<double>.Build.Dense(rank, rank);
      var WᵀV = Matrix<double>.Build.Dense(rank, n);
      var VHᵀ = Matrix<double>.Build.Dense(m, rank);



      double previousError = double.PositiveInfinity;
      for (int iter = 0; iter < MaximumNumberOfIterations; iter++)
      {
        // Keep shared products up-to-date.
        W.TransposeThisAndMultiply(W, WᵀW);
        H.TransposeAndMultiply(H, HHᵀ);
        W.TransposeThisAndMultiply(V, WᵀV);
        V.TransposeAndMultiply(H, VHᵀ);

        // === Update H (row-wise / component-wise) ===
        for (int k = 0; k < rank; k++)
        {
          double denom = WᵀW[k, k] + LambdaW;
          if (denom <= 0)
            denom = eps;

          for (int j = 0; j < n; j++)
          {
            double numer = WᵀV[k, j];
            for (int l = 0; l < rank; l++)
            {
              if (l != k)
                numer -= WᵀW[k, l] * H[l, j];
            }

            double hNew = numer / denom;
            if (hNew < eps)
              hNew = eps;
            H[k, j] = hNew;
          }
        }

        // Refresh dependent quantities after changing H.
        H.TransposeAndMultiply(H, HHᵀ);
        V.TransposeAndMultiply(H, VHᵀ);

        // === Update W (column-wise / component-wise) ===
        for (int k = 0; k < rank; k++)
        {
          double denom = HHᵀ[k, k] + LambdaH;
          if (denom <= 0)
            denom = eps;

          for (int i = 0; i < m; i++)
          {
            double numer = VHᵀ[i, k];
            for (int l = 0; l < rank; l++)
            {
              if (l != k)
                numer -= W[i, l] * HHᵀ[l, k];
            }

            double wNew = numer / denom;
            if (wNew < eps)
              wNew = eps;
            W[i, k] = wNew;
          }
        }

        // Update shared products for convergence check.
        W.TransposeThisAndMultiply(W, WᵀW);
        H.TransposeAndMultiply(H, HHᵀ);
        W.TransposeThisAndMultiply(V, WᵀV);

        // Relative error (Frobenius) using trace identity.
        double traceHtWtX = NonnegativeMatrixFactorizationBase.TraceOfTransposeAndMultiply(H, WᵀV);
        double traceWtWHHt = 0;
        for (int a = 0; a < rank; a++)
          for (int b = 0; b < rank; b++)
            traceWtWHHt += WᵀW[a, b] * HHᵀ[a, b];

        double err = Math.Sqrt(Math.Max(0, traceXtX - 2 * traceHtWtX + traceWtWHHt)) / xNorm;
        if (Math.Abs(previousError - err) < Tolerance)
          break;
        previousError = err;
      }

      return (W, H);
    }
  }
}
