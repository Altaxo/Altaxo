#region Copyright

/////////////////////////////////////////////////////////////////////////////
// Altaxo:  a data processing and data plotting program
// Copyright (C) 2023 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  /// <summary>
  /// Implements the Nonnegative Matrix Factorization (NMF) algorithm based on Alternating Constrained Least Squares (ACLS)
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Langville et al., "Algorithms, Initializations, and Convergence for the Nonnegative Matrix Factorization", https://arxiv.org/abs/1407.7299v1</para>
  /// </remarks>
  public class NonnegativeMatrixFactorizationByACLS : NonnegativeMatrixFactorizationBase
  {

    public (Matrix<double> W, Matrix<double> H) ACLS(Matrix<double> X, int r, int maxIter = 1000, double lambda = 0.0)
    {
      // Use NNDSVDa initialization
      var (W, H) = NNDSVDa(X, r);

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
    /// Factorizes matrix a into nonnegative factors and nonnegative base vectors.
    /// </summary>
    /// <param name="a">The matrix to factorize.</param>
    /// <param name="r">The number of components (number of base vectors).</param>
    /// <param name="maximalNumberOfIterations">The maximal number of iterations for the calculation.</param>
    /// <param name="lambdaH">Regularization parameter for the factors.</param>
    /// <param name="lambdaW">Regularization parameter for the base vectors.</param>
    /// <returns>Matrix of base vectors W (each base vector is a column of the matrix), and matrix of factors H.</returns>
    /// <exception cref="System.ArgumentNullException">a</exception>
    /// <remarks>
    /// <para>Algorithm is described in [1], page 7.</para>
    /// <para>Please note that base vectors and factors are output in arbitrary order.</para>
    /// </remarks>
    public (Matrix<double> W, Matrix<double> H) Evaluate(Matrix<double> a, int r, int maximalNumberOfIterations, NonnegativeMatrixFactorizationInitializationMethod initialization, double lambdaH = 0, double lambdaW = 0)
    {
      if (a is null)
      {
        throw new ArgumentNullException(nameof(a));
      }

      var m = a.RowCount;
      var n = a.ColumnCount;

      var wt = Matrix<double>.Build.Dense(r, m); // instead of w in [1], we use w-transposed
      var h = Matrix<double>.Build.Dense(r, n);

      var wtw = Matrix<double>.Build.Dense(r, r);
      var wta = Matrix<double>.Build.Dense(r, n);

      var hht = Matrix<double>.Build.Dense(r, r);
      var hat = Matrix<double>.Build.Dense(r, m);

      var abar = Matrix<double>.Build.Dense(m, n);

      var listOfChi2 = new List<double>(r);

      switch (initialization)
      {
        default:
        case NonnegativeMatrixFactorizationInitializationMethod.Random:
          {
            FillRandomNonnegative(wt);
          }
          break;
        case NonnegativeMatrixFactorizationInitializationMethod.NNDSVD:
          {
            var (w, _) = NNDSVD(a, r);
            wt = w.Transpose();
          }
          break;
        case NonnegativeMatrixFactorizationInitializationMethod.NNDSVDa:
          {
            var (w, _) = NNDSVDa(a, r);
            wt = w.Transpose();
          }
          break;
        case NonnegativeMatrixFactorizationInitializationMethod.NNDSVDar:
          {
            var (w, _) = NNDSVDar(a, r);
            wt = w.Transpose();
            break;
          }
      }

      // Algorithm see [1], page 7, "Practical ACLS Algorithm for NMF"
      for (int iIteration = 0; iIteration < maximalNumberOfIterations; iIteration++)
      {
        wt.TransposeAndMultiply(wt, wtw); // wtw = wᵀ w
        wt.Multiply(a, wta);              // wta = wᵀ a
        for (int i = 0; i < r; ++i)       // Add lambdaH to the diagonal of wtw
        {
          wtw[i, i] += lambdaH;           // wᵀ w + lambdaH I   
        }
        wtw.Solve(wta, h);                // (wᵀ w + lambdaH I) h = wᵀ a


        ClearNonnegativeElements(h);      // set all negative elements of h to zero

        h.TransposeAndMultiply(h, hht);   // hht = h hᵀ
        h.TransposeAndMultiply(a, hat);   // hat = h aᵀ
        for (int i = 0; i < r; ++i)       // Add lambdaW to the diagonal of hht
        {
          hht[i, i] += lambdaW;           // h hᵀ + lambdaW I 
        }
        hht.Solve(hat, wt);               // (h hᵀ + lambdaW I) wᵀ = h aᵀ
        ClearNonnegativeElements(wt);     // set all negative elements of w to zero

        // Evaluation of the quality
        wt.TransposeThisAndMultiply(h, abar);
        listOfChi2.Add(SumOfSquaredDifferences(a, abar)); // TODO find a criterion for ending the loop prematurely
      }

      return (wt.Transpose(), h);
    }

    /// <summary>
    /// Replaces negative elements of the matrix with zero.
    /// </summary>
    /// <param name="m">The matrix m.</param>
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
    /// Fills the matrix with random nonnegative values.
    /// </summary>
    /// <param name="m">The matrix m.</param>
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
    /// Calculates the sum the of squared differences between the matrix elements in m and y.
    /// </summary>
    /// <param name="m">The matrix m.</param>
    /// <param name="y">The matrix y.</param>
    /// <returns>Sum the of squared differences between the matrix elements in m and y.</returns>
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
