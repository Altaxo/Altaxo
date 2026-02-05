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
  /// Provides principal component analysis (PCA) routines.
  /// </summary>
  public record PrincipalComponentAnalysisByNIPALS : ILowRankMatrixFactorization
  {
    /// <summary>
    /// Gets the convergence accuracy used by the NIPALS algorithm.
    /// </summary>
    public double Accuracy
    {
      get => field;
      init
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentOutOfRangeException(nameof(Accuracy), "Accuracy must be in the range (0, 1).");
        field = value;
      }
    } = 1E-9;


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PrincipalComponentAnalysisByNIPALS), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PrincipalComponentAnalysisByNIPALS)obj;
        info.AddValue("Accuracy", s.Accuracy);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var accuracy = info.GetDouble("Accuracy");
        return ((o as PrincipalComponentAnalysisByNIPALS) ?? new PrincipalComponentAnalysisByNIPALS()) with
        {
          Accuracy = accuracy
        };
      }
    }

    #endregion

    /// <summary>
    /// Calculates eigenvectors (loadings) and the corresponding scores using the NIPALS algorithm.
    /// </summary>
    /// <param name="X">The matrix to which the decomposition is applied to. A row of the matrix is one spectrum (or a single measurement giving multiple resulting values). The different rows of the matrix represent
    /// measurements under different conditions.</param>
    /// <param name="numFactors">The number of factors to be calculated. If 0 is provided, factors are calculated until the provided accuracy is reached. </param>
    /// <param name="accuracy">The relative residual variance that should be reached.</param>
    /// <returns>
    /// A tuple containing: the factor (scores) matrix (columns are score vectors), the loadings matrix (rows are load vectors),
    /// and a residual variance vector (element[0] is the original variance; element[1] is the residual variance after subtracting the first factor; and so on).
    /// </returns>
    public static (Matrix<double> factors, Matrix<double> loads, Vector<double> residualVarianceVector) NIPALS_HO(
      Matrix<double> X,
      int numFactors,
      double accuracy)
    {
      // first center the matrix
      //MatrixMath.ColumnsToZeroMean(X, null);

      var originalVariance = X.FrobeniusNorm();

      int maxFactors = numFactors <= 0 ? X.ColumnCount : Math.Min(numFactors, X.ColumnCount);
      int estimatedFactors = maxFactors;

      var factorsList = new Vector<double>[estimatedFactors];
      var loadsList = new Vector<double>[estimatedFactors];

      var residuals = new double[estimatedFactors + 1];
      int residualCount = 0;
      residuals[residualCount++] = originalVariance;

      var loading = CreateVector.Dense<double>(X.ColumnCount);
      Vector<double>? scoresPrev = null;
      var scores = CreateVector.Dense<double>(X.RowCount);
      const double scoresConvergenceTolerance = 1E-9;
      var scoresConvergenceToleranceSquared = scoresConvergenceTolerance * scoresConvergenceTolerance;

      for (int nFactor = 0; nFactor < maxFactors; nFactor++)
      {
        //l has to be a horizontal vector
        // 1. Guess the transposed Vector l_transp, use first row of X matrix if it is not empty, otherwise the first non-empty row
        int rowoffset = 0;
        do
        {
          X.Row(rowoffset, loading); // loading is now a horizontal vector
          rowoffset++;
        } while (loading.L2Norm() == 0 && rowoffset < X.RowCount);

        for (int iter = 0; iter < 500; iter++)
        {
          // 2. Calculate the new vector t for the factor values
          X.Multiply(loading, scores); // scores = X*loading (scores is a vertical vector)

          // Compare this with the previous one
          if (scoresPrev is not null)
          {
            double squaredDistance = 0;
            for (int i = 0; i < scores.Count; i++)
            {
              var d = scores[i] - scoresPrev[i];
              squaredDistance += d * d;
            }
            if (squaredDistance <= scoresConvergenceToleranceSquared)
              break;
          }

          // 3. Calculate the new loads
          X.TransposeThisAndMultiply(scores, loading); // loading = X^T * scores

          // normalize the (one) row
          var loadingNorm = loading.L2Norm();
          if (loadingNorm != 0)
            loading.Divide(loadingNorm, loading);

          // 4. Goto step 2 or break after a number of iterations
          scoresPrev ??= CreateVector.Dense<double>(X.RowCount);
          scores.CopyTo(scoresPrev); // stores the content of scores in scoresPrev
        }

        // Store factor and loads
        factorsList[nFactor] = scores.Clone();
        loadsList[nFactor] = loading.Clone();

        // 5. Calculate the residual matrix X = X - t*l
        // (outer product)
        X.Subtract(scores.OuterProduct(loading), X); // X is now the residual matrix

        // if the number of factors to calculate is not provided,
        // calculate the norm of the residual matrix and compare with the original
        // one
        if (numFactors <= 0)
        {
          double residualVariance = X.FrobeniusNorm();
          residuals[residualCount++] = residualVariance;

          if (residualVariance <= accuracy * originalVariance)
          {
            estimatedFactors = nFactor + 1;
            break;
          }
        }
      } // for all factors

      if (numFactors > 0)
        estimatedFactors = maxFactors;

      var factors = CreateMatrix.Dense<double>(X.RowCount, estimatedFactors);
      var loads = CreateMatrix.Dense<double>(estimatedFactors, X.ColumnCount);
      for (int i = 0; i < estimatedFactors; i++)
      {
        factors.SetColumn(i, factorsList[i]);
        loads.SetRow(i, loadsList[i]);
      }

      var residualVarianceVector = CreateVector.Dense<double>(residualCount);
      for (int i = 0; i < residualCount; i++)
        residualVarianceVector[i] = residuals[i];

      return (factors, loads, residualVarianceVector);
    } // end NIPALS

    /// <inheritdoc/>
    public (Matrix<double> W, Matrix<double> H) Factorize(Matrix<double> A, int rank)
    {
      var x = A.Clone();
      var (factors, loads, _) = NIPALS_HO(x, rank, 1E-9);
      return (factors, loads);
    }
  }
}
