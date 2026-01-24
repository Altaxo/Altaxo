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
  /// Non-negative matrix factorization (NMF) using the classic multiplicative update rules.
  /// </summary>
  /// <remarks>
  /// References: Berry, M. W., Browne, M., Langville, A. N., Pauca, V. P., & Plemmons, R. J. (2007). Algorithms and applications for approximate nonnegative matrix factorization. Computational Statistics & Data Analysis, 52(1), 155–173. https://doi.org/10.1016/j.csda.2006.11.006
  /// </remarks>
  public record NonnegativeMatrixFactorizationByMultiplicativeUpdate : NonnegativeMatrixFactorizationBase
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonnegativeMatrixFactorizationByMultiplicativeUpdate), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NonnegativeMatrixFactorizationByMultiplicativeUpdate)obj;
        info.AddValue("InitializationMethod", s.InitializationMethod);
        info.AddValue("MaximumNumberOfIterations", s.MaximumNumberOfIterations);
        info.AddValue("NumberOfTrials", s.NumberOfAdditionalTrials);
        info.AddValue("Tolerance", s.Tolerance);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var initializationMethod = info.GetValue<INonnegativeMatrixFactorizationInitializer>("InitializationMethod", parent);
        var maximumNumberOfIterations = info.GetInt32("MaximumNumberOfIterations");
        var numberOfTrials = info.GetInt32("NumberOfTrials");
        var tolerance = info.GetDouble("Tolerance");

        return ((o as NonnegativeMatrixFactorizationByMultiplicativeUpdate) ?? new NonnegativeMatrixFactorizationByMultiplicativeUpdate()) with
        {
          InitializationMethod = initializationMethod,
          MaximumNumberOfIterations = maximumNumberOfIterations,
          NumberOfAdditionalTrials = numberOfTrials,
          Tolerance = tolerance,
        };
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> V, int rank)
    {
      int m = V.RowCount;
      int n = V.ColumnCount;

      var traceVᵀV = TraceOfTransposeAndMultiply(V, V);
      double vNorm = V.FrobeniusNorm();
      double epsilon = 1e-10 * vNorm * Math.Sqrt(vNorm); // in the reference this is 1E-9, but I take into account different scales of V

      var (W, H) = InitializationMethod.GetInitialFactors(V, rank); // initialization so that Norm(W) and Norm(H) are in the same range
      var WᵀV = Matrix<double>.Build.Dense(rank, n);
      var WᵀW = Matrix<double>.Build.Dense(rank, rank);
      var WᵀWH = Matrix<double>.Build.Dense(rank, n);
      var VHᵀ = Matrix<double>.Build.Dense(m, rank);
      var HHᵀ = Matrix<double>.Build.Dense(rank, rank);
      var WHHᵀ = Matrix<double>.Build.Dense(m, rank);
      var rowOfH = Vector<double>.Build.Dense(n);
      var colOfW = Vector<double>.Build.Dense(m);

      double previousError = double.PositiveInfinity;
      for (int iter = 0; iter < MaximumNumberOfIterations; iter++)
      {
        // Update H
        W.TransposeThisAndMultiply(V, WᵀV); // nominator
        W.TransposeThisAndMultiply(W, WᵀW);
        WᵀW.Multiply(H, WᵀWH); // denominator

        // Convergence criterion
        // see Berry et al. 2007 https://doi.org/10.1016/j.csda.2006.11.006 page 161
        // instead of calculating ||V-WH|| directly,
        // we use the relation ||V-WH||² = trace(VᵀV) - 2*trace(HᵀWᵀV) + trace(HᵀWᵀWH)
        // the trace of the product of Hᵀ with the other two terms can be computed efficiently
        // we have to compute it here because later on both W and H are modified, and neither WᵀV nor WᵀWH are valid anymore
        var error = Math.Sqrt(Math.Max(0, traceVᵀV - 2 * TraceOfTransposeAndMultiply(H, WᵀV) + TraceOfTransposeAndMultiply(H, WᵀWH))) / vNorm;
        if (Math.Abs(previousError - error) < Tolerance)
          break;
        previousError = error;


        // Update H by pointwise multiplication and division
        // H.PointwiseMultiply(WᵀV.PointwiseDivide(WᵀWH), H);
        for (int i = 0; i < rank; i++)
        {
          for (int j = 0; j < n; j++)
          {
            H[i, j] = H[i, j] * (WᵀV[i, j] / Math.Max(WᵀWH[i, j], epsilon));
          }
        }

        // Update W
        V.TransposeAndMultiply(H, VHᵀ); // nominator
        H.TransposeAndMultiply(H, HHᵀ);
        W.Multiply(HHᵀ, WHHᵀ);          // denominator

        // Update W by pointwise multiplication and division
        // W.PointwiseMultiply(VHᵀ.PointwiseDivide(WHHᵀ), W);
        for (int i = 0; i < m; i++)
        {
          for (int j = 0; j < rank; j++)
          {
            W[i, j] = W[i, j] * (VHᵀ[i, j] / Math.Max(WHHᵀ[i, j], epsilon));
          }
        }

        // Optional: rescaling for conditioning
        // We try to keep the norms of the columns of W and the rows of H in a similar range
        for (int k = 0; k < rank; k++)
        {
          W.Column(k, colOfW);
          H.Row(k, rowOfH);
          double normWk = Math.Sqrt(colOfW.L2Norm() / rowOfH.L2Norm());
          if (normWk > 0)
          {
            colOfW.Multiply(1 / normWk, colOfW);
            rowOfH.Multiply(normWk, rowOfH);
            W.SetColumn(k, colOfW);
            H.SetRow(k, rowOfH);
          }
        }
      }
      return (W, H);
    }


  }
}
