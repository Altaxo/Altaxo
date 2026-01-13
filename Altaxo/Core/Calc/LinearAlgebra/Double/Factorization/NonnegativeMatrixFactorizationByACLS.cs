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
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Implements the Nonnegative Matrix Factorization (NMF) algorithm based on Alternating Constrained Least Squares (ACLS).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Langville et al., "Algorithms, Initializations, and Convergence for the Nonnegative Matrix Factorization", https://arxiv.org/abs/1407.7299v1</para>
  /// </remarks>
  public record NonnegativeMatrixFactorizationByACLS : NonnegativeMatrixFactorizationBase
  {
    double LambdaW
    {
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(LambdaW), "LambdaW must be non-negative.");
        field = value;
      }
    } = 0;

    double LambdaH
    {
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(LambdaH), "LambdaH must be non-negative.");
        field = value;
      }
    } = 0;


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
        info.AddValue("NumberOfTrials", s.NumberOfTrials);
        info.AddValue("Tolerance", s.Tolerance);
        info.AddValue("LambdaW", s.LambdaW);
        info.AddValue("LambdaH", s.LambdaH);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var initializationMethod = info.GetValue<INonnegativeMatrixFactorizationInitializer>("InitializationMethod", parent);
        var maximumNumberOfIterations = info.GetValue<int>("MaximumNumberOfIterations", parent);
        var numberOfTrials = info.GetInt32("NumberOfTrials");
        var tolerance = info.GetDouble("Tolerance");
        var lambdaW = info.GetDouble("LambdaW");
        var lambdaH = info.GetDouble("LambdaH");

        return ((o as NonnegativeMatrixFactorizationByACLS) ?? new NonnegativeMatrixFactorizationByACLS()) with
        {
          InitializationMethod = initializationMethod,
          MaximumNumberOfIterations = maximumNumberOfIterations,
          NumberOfTrials = numberOfTrials,
          Tolerance = tolerance,
          LambdaW = lambdaW,
          LambdaH = lambdaH
        };
      }
    }

    #endregion


    /// <summary>
    /// Computes an NMF factorization using an ACLS-like alternating constrained (non-negative) least-squares scheme.
    /// </summary>
    /// <param name="X">The (typically non-negative) input matrix to be factorized.</param>
    /// <param name="r">The factorization rank.</param>
    /// <param name="maxIter">The number of iterations to perform.</param>
    /// <param name="lambda">The Tikhonov regularization parameter (&lambda;).</param>
    /// <returns>A tuple containing (W, H) such that <c>X ≈ W * H</c> with non-negative factors.</returns>
    public (Matrix<double> W, Matrix<double> H) ACLS(Matrix<double> X, int r, int maxIter = 1000, double lambda = 0.0)
    {
      // Use NNDSVDa initialization
      var (W, H) = InitializationMethod.GetInitialFactors(X, r);

      int m = X.RowCount;
      int n = X.ColumnCount;

      for (int iter = 0; iter < maxIter; iter++)
      {
        // === Update H ===
        // Solve:  min_H ||X - W H||_F^2 + λ||H||_F^2  s.t. H >= 0
        var WT = W.Transpose();
        var A = WT * W + lambda * Matrix<double>.Build.DenseDiagonal(r, r, 1.0);
        var B = WT * X;

        for (int j = 0; j < n; j++)
        {
          var h = A.Solve(B.Column(j));
          H.SetColumn(j, h.PointwiseMaximum(0.0));
        }

        // === Update W ===
        // Solve:  min_W ||X - W H||_F^2 + λ||W||_F^2  s.t. W >= 0
        var HT = H.Transpose();
        A = H * HT + lambda * Matrix<double>.Build.DenseDiagonal(r, r, 1.0);
        B = X * HT;

        for (int i = 0; i < m; i++)
        {
          var w = A.Solve(B.Row(i).ToColumnMatrix()).Column(0);
          W.SetRow(i, w.PointwiseMaximum(0.0));
        }
      }

      return (W, H);
    }


    /// <summary>
    /// Factorizes matrix <paramref name="X"/> into non-negative factors and non-negative base vectors.
    /// </summary>
    /// <param name="X">The matrix to factorize.</param>
    /// <param name="r">The number of components (number of base vectors).</param>
    /// <returns>Matrix of base vectors W (each base vector is a column of the matrix) and matrix of factors H.</returns>
    /// <exception cref="System.ArgumentNullException">a</exception>
    /// <remarks>
    /// <para>The algorithm is described in [1], page 7.</para>
    /// <para>Please note that base vectors and factors are output in an arbitrary order.</para>
    /// </remarks>
    public override (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> X, int r)
    {
      ArgumentNullException.ThrowIfNull(X, nameof(X));

      var m = X.RowCount;
      var n = X.ColumnCount;

      var wt = Matrix<double>.Build.Dense(r, m); // instead of w in [1], we use w-transposed
      var h = Matrix<double>.Build.Dense(r, n);

      var wtw = Matrix<double>.Build.Dense(r, r);
      var wta = Matrix<double>.Build.Dense(r, n);

      var hht = Matrix<double>.Build.Dense(r, r);
      var hat = Matrix<double>.Build.Dense(r, m);

      var abar = Matrix<double>.Build.Dense(m, n);

      var listOfChi2 = new List<double>(r);

      (var w, _) = InitializationMethod.GetInitialFactors(X, r);

      // Algorithm see [1], page 7, "Practical ACLS Algorithm for NMF"
      for (int iIteration = 0; iIteration < MaximumNumberOfIterations; iIteration++)
      {
        wt.TransposeAndMultiply(wt, wtw); // wtw = wᵀ w
        wt.Multiply(X, wta);              // wta = wᵀ a
        for (int i = 0; i < r; ++i)       // Add lambdaH to the diagonal of wtw
        {
          wtw[i, i] += LambdaH;           // wᵀ w + lambdaH I   
        }
        wtw.Solve(wta, h);                // (wᵀ w + lambdaH I) h = wᵀ a


        ClearNonnegativeElements(h);      // set all negative elements of h to zero

        h.TransposeAndMultiply(h, hht);   // hht = h hᵀ
        h.TransposeAndMultiply(X, hat);   // hat = h aᵀ
        for (int i = 0; i < r; ++i)       // Add lambdaW to the diagonal of hht
        {
          hht[i, i] += LambdaW;           // h hᵀ + lambdaW I 
        }
        hht.Solve(hat, wt);               // (h hᵀ + lambdaW I) wᵀ = h aᵀ
        ClearNonnegativeElements(wt);     // set all negative elements of w to zero

        // Evaluation of the quality
        wt.TransposeThisAndMultiply(h, abar);
        listOfChi2.Add(SumOfSquaredDifferences(X, abar)); // TODO find a criterion for ending the loop prematurely
      }

      return (wt.Transpose(), h);
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
    /// Fills the matrix with random non-negative values.
    /// </summary>
    /// <param name="m">The matrix.</param>
    private static void FillRandomNonnegative(Matrix<double> m)
    {
      var rnd = new System.Random();
      for (int r = 0; r < m.RowCount; r++)
      {
        for (int c = 0; c < m.ColumnCount; ++c)
        {
          m[r, c] = rnd.NextDouble();
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
