#region Copyright

/////////////////////////////////////////////////////////////////////////////
// Altaxo:  a data processing and data plotting program
// Copyright (C) 2002 - 2026 Dr. Dirk Lellinger
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Implements the Nonnegative Matrix Factorization (NMF) algorithm based on Alternating Constrained Least Squares (ACLS).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Langville et al., "Algorithms, Initializations, and Convergence for the Nonnegative Matrix Factorization", https://arxiv.org/abs/1407.7299v1</para>
  /// </remarks>
  public record NonnegativeMatrixFactorizationByACLS : NonnegativeMatrixFactorizationWithRegularizationBase
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonnegativeMatrixFactorizationByACLS), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NonnegativeMatrixFactorizationByACLS)obj;
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

        return ((o as NonnegativeMatrixFactorizationByACLS) ?? new NonnegativeMatrixFactorizationByACLS()) with
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

    #endregion

    /// <inheritdoc/>
    /// <remarks>
    /// <para>The algorithm is described in [1], page 7.</para>
    /// <para>Please note that base vectors and factors are output in an arbitrary order.</para>
    /// </remarks>
    public override (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> V, int rank)
    {
      ArgumentNullException.ThrowIfNull(V, nameof(V));

      var m = V.RowCount;
      var n = V.ColumnCount;

      double vNorm = V.FrobeniusNorm();
      if (vNorm == 0)
      {
        return (Matrix<double>.Build.Dense(m, rank), Matrix<double>.Build.Dense(rank, n));
      }
      // Used for efficient error computation: ||V - WH||_F^2 = ||V||_F^2 - 2*tr(Hᵀ Wᵀ V) + tr(Wᵀ W * H Hᵀ)
      var traceVᵀV = vNorm * vNorm;

      (var w, _) = InitializationMethod.GetInitialFactors(V, rank);
      var Wᵀ = w.Transpose(); // instead of w in [1], we use w-transposed
      var H = Matrix<double>.Build.Dense(rank, n);

      var WᵀW = Matrix<double>.Build.Dense(rank, rank);
      var WᵀV = Matrix<double>.Build.Dense(rank, n);

      var HHᵀ = Matrix<double>.Build.Dense(rank, rank);
      var HVᵀ = Matrix<double>.Build.Dense(rank, m);
      var WᵀWH = Matrix<double>.Build.Dense(rank, n);

      var abar = Matrix<double>.Build.Dense(m, n);

      var errorHistory = new Chi2History(4, Tolerance, MaximumNumberOfIterations);

      // Algorithm see [1], page 7, "Practical ACLS Algorithm for NMF"
      for (int iIteration = 0; iIteration < MaximumNumberOfIterations; iIteration++)
      {
        Wᵀ.TransposeAndMultiply(Wᵀ, WᵀW); // wtw = wᵀ w
        Wᵀ.Multiply(V, WᵀV);              // wta = wᵀ a
        H.TransposeAndMultiply(H, HHᵀ); // here only needed for the tolerance check: hᵀ h

        // Convergence criterion
        // see Berry et al. 2007 https://doi.org/10.1016/j.csda.2006.11.006 page 161
        // instead of calculating ||V-WH|| directly,
        // we use the relation ||V-WH||² = trace(VᵀV) - 2*trace(HᵀWᵀV) + trace(HᵀWᵀWH)
        // the trace of the product of Hᵀ with the other two terms can be computed efficiently
        // we have to compute it here because later on both W and H are modified, and neither WᵀV nor WᵀWH are valid anymore
        var error = Math.Sqrt(Math.Max(0, traceVᵀV - 2 * TraceOfTransposeAndMultiply(H, WᵀV) + TraceOfTransposeAndMultiply(HHᵀ, WᵀW))) / vNorm;
        var (terminate, bestWt, bestH) = errorHistory.Add(error, Wᵀ, H);
        if (terminate)
        {
          return (bestWt!.Transpose(), bestH!);
        }


        for (int i = 0; i < rank; ++i)       // Add lambdaH to the diagonal of wtw
        {
          WᵀW[i, i] += LambdaH;           // wᵀ w + lambdaH I   
        }
        WᵀW.Solve(WᵀV, H);                // (wᵀ w + lambdaH I) h = wᵀ a

        ClearNonnegativeElements(H);      // set all negative elements of h to zero

        H.TransposeAndMultiply(H, HHᵀ);   // hht = h hᵀ
        H.TransposeAndMultiply(V, HVᵀ);   // hat = h aᵀ
        for (int i = 0; i < rank; ++i)       // Add lambdaW to the diagonal of hht
        {
          HHᵀ[i, i] += LambdaW;           // h hᵀ + lambdaW I 
        }
        HHᵀ.Solve(HVᵀ, Wᵀ);               // (h hᵀ + lambdaW I) wᵀ = h aᵀ
        ClearNonnegativeElements(Wᵀ);     // set all negative elements of w to zero
      }
      return (Wᵀ.Transpose(), H);
    }




    /// <summary>
    /// Replaces negative elements of the matrix with zero.
    /// </summary>
    /// <param name="m">The matrix.</param>
    private static void ClearNonnegativeElements(Matrix<double> m)
    {
      for (int r = 0; r < m.RowCount; r++)
      {
        for (int c = 0; c < m.ColumnCount; ++c)
        {
          if (m[r, c] < 0)
          {
            m[r, c] = 0;
          }
        }
      }
    }

    /// <summary>
    /// Calculates the sum of squared differences between the matrix elements in <paramref name="m"/> and <paramref name="y"/>.
    /// </summary>
    /// <param name="m">The first matrix.</param>
    /// <param name="y">The second matrix.</param>
    /// <returns>The sum of squared differences between the matrix elements in <paramref name="m"/> and <paramref name="y"/>.</returns>
    private static double SumOfSquaredDifferences(Matrix<double> m, Matrix<double> y)
    {
      double sum = 0;
      for (int r = 0; r < m.RowCount; r++)
      {
        for (int c = 0; c < m.ColumnCount; ++c)
        {
          var d = m[r, c] - y[r, c];
          sum += d * d;
        }
      }
      return sum;
    }
  }
}
