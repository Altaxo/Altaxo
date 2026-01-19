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
      (var w, _) = InitializationMethod.GetInitialFactors(X, r);
      var wt = w.Transpose(); // instead of w in [1], we use w-transposed
      var h = Matrix<double>.Build.Dense(r, n);

      var wtw = Matrix<double>.Build.Dense(r, r);
      var wta = Matrix<double>.Build.Dense(r, n);

      var hht = Matrix<double>.Build.Dense(r, r);
      var hat = Matrix<double>.Build.Dense(r, m);

      var abar = Matrix<double>.Build.Dense(m, n);

      var chi2History = new Chi2History(4);

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

        var (terminate, bestWt, bestH) = chi2History.Add(SumOfSquaredDifferences(X, abar), wt, h);
        if (terminate)
        {
          return (bestWt!.Transpose(), bestH!);
        }
      }
      return (wt.Transpose(), h);
    }

    /// <summary>
    /// Stores the history of chi2 values to determine convergence.
    /// For the ACLS algorithm, we stop if the chi2 value increases for a number of iterations.
    /// </summary>
    private class Chi2History
    {
      private int _count;
      private (double chi2, Matrix<double>? wt, Matrix<double>? h)[] _listOfChi2;

      /// <summary>
      /// Creates a new instance Chi2History with the given depth.
      /// </summary>
      /// <param name="depth">The number of historical elements to store.</param>
      public Chi2History(int depth)
      {
        _listOfChi2 = new (double chi2, Matrix<double>? wt, Matrix<double>? h)[depth];
        _count = 0;
      }

      /// <summary>
      /// Adds a new chi2 value to the history. 
      /// </summary>
      /// <param name="chi2">The new chi² value.</param>
      /// <param name="w">The corresponding weight matrix. (it is cloned for storage).</param>
      /// <param name="h">The corresponding load matrix (it is cloned for storage).</param>
      /// <returns>A tuple (terminate, w and h). If terminate is true, the iteration can be terminated, and the w and h values returned here are the best values for factorzation. If terminate is false, then w and h are null.</returns>
      public (bool terminate, Matrix<double>? w, Matrix<double>? h) Add(double chi2, Matrix<double> w, Matrix<double> h)
      {
        // if the history is full, check if the new chi2 is larger than the all others in history
        // if this is the case, we can stop the iteration, and return the best w and h from history
        if (_count == _listOfChi2.Length)
        {
          double maxChi2 = double.MinValue;
          for (int i = 0; i < _listOfChi2.Length; ++i)
          {
            maxChi2 = Math.Max(maxChi2, _listOfChi2[i].chi2);
          }
          if (chi2 > maxChi2)
          {
            // if the new chi2 is larger than all others in history, terminate, and return the best w and h from history
            double minChi2 = double.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < _listOfChi2.Length; ++i)
            {
              if (_listOfChi2[i].chi2 < minChi2)
              {
                minChi2 = _listOfChi2[i].chi2;
                minIndex = i;
              }
              maxChi2 = Math.Max(maxChi2, _listOfChi2[i].chi2);
            }
            return (true, _listOfChi2[minIndex].wt, _listOfChi2[minIndex].h);
          }
          else // if the actual chi2 is not the largest in history
          {
            // shift the values to the left
            for (int i = 1; i < _count; ++i)
            {
              _listOfChi2[i - 1] = _listOfChi2[i];
            }
            // and store the new chi2
            _listOfChi2[_count - 1] = (chi2, null, null); // just to have a valid entry
          }
        }
        else
        {
          // still filling the history
          _listOfChi2[_count++] = (chi2, null, null);
        }

        // determine the minimum chi2 overall in history (with exception of the actual value)
        var minChi2Overall = double.MaxValue;
        for (int i = 0; i < _count - 1; ++i)
        {
          minChi2Overall = Math.Min(minChi2Overall, _listOfChi2[i].chi2);
        }

        // if the actual chi2 is smaller than the minimum chi2 overall, we store w and h in the history
        if (chi2 <= minChi2Overall)
        {
          _listOfChi2[_count - 1] = (chi2, w.Clone(), h.Clone());

          // and we can free the other matrices
          for (int i = 0; i < _count - 1; ++i)
          {
            _listOfChi2[i].wt = null;
            _listOfChi2[i].h = null;
          }
        }
        return (false, null, null);
      }
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
