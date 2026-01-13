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
using static Altaxo.Calc.LinearAlgebra.MatrixMath;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Provides principal component analysis (PCA) routines.
  /// </summary>
  public record PrincipalComponentAnalysisByNIPALS : ILowRankMatrixFactorization
  {
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
    /// Calculates eigenvectors (loads) and the corresponding eigenvalues (scores)
    /// by means of the NIPALS algorithm
    /// </summary>
    /// <param name="X">The matrix to which the decomposition is applied to. A row of the matrix is one spectrum (or a single measurement giving multiple resulting values). The different rows of the matrix represent
    /// measurements under different conditions.</param>
    /// <param name="numFactors">The number of factors to be calculated. If 0 is provided, factors are calculated until the provided accuracy is reached. </param>
    /// <param name="accuracy">The relative residual variance that should be reached.</param>
    /// <param name="factors">Resulting matrix of factors. You have to provide a extensible matrix of dimension(0,0) as the vertical score vectors are appended to the matrix.</param>
    /// <param name="loads">Resulting matrix consiting of horizontal load vectors (eigenspectra). You have to provide a extensible matrix of dimension(0,0) here.</param>
    /// <param name="residualVarianceVector">Residual variance. Element[0] is the original variance, element[1] the residual variance after the first factor subtracted and so on. You can provide null if you don't need this result.</param>
    public static void NIPALS_HO(
      IMatrix<double> X,
      int numFactors,
      double accuracy,
      IRightExtensibleMatrix<double> factors,
      IBottomExtensibleMatrix<double> loads,
      IBottomExtensibleMatrix<double> residualVarianceVector)
    {
      // first center the matrix
      //MatrixMath.ColumnsToZeroMean(X, null);

      double originalVariance = Math.Sqrt(MatrixMath.SumOfSquares(X));

      if (residualVarianceVector is not null)
        residualVarianceVector.AppendBottom(new MatrixMath.ScalarAsMatrix<double>(originalVariance));

      IMatrix<double> l = new MatrixWithOneRow<double>(X.ColumnCount);
      IMatrix<double>? t_prev = null;
      IMatrix<double> t = new MatrixWithOneColumn<double>(X.RowCount);

      int maxFactors = numFactors <= 0 ? X.ColumnCount : Math.Min(numFactors, X.ColumnCount);

      for (int nFactor = 0; nFactor < maxFactors; nFactor++)
      {
        //l has to be a horizontal vector
        // 1. Guess the transposed Vector l_transp, use first row of X matrix if it is not empty, otherwise the first non-empty row
        int rowoffset = 0;
        do
        {
          Submatrix(X, l, rowoffset, 0);     // l is now a horizontal vector
          rowoffset++;
        } while (IsZeroMatrix(l) && rowoffset < X.RowCount);

        for (int iter = 0; iter < 500; iter++)
        {
          // 2. Calculate the new vector t for the factor values
          MultiplySecondTransposed(X, l, t); // t = X*l_t (t is  a vertical vector)

          // Compare this with the previous one
          if (t_prev is not null && IsEqual(t_prev, t, 1E-9))
            break;

          // 3. Calculate the new loads
          MultiplyFirstTransposed(t, X, l); // l = t_tr*X  (gives a horizontal vector of load (= eigenvalue spectrum)

          // normalize the (one) row
          NormalizeRows(l); // normalize the eigenvector spectrum

          // 4. Goto step 2 or break after a number of iterations
          if (t_prev is null)
            t_prev = new MatrixWithOneColumn<double>(X.RowCount);
          Copy(t, t_prev); // stores the content of t in t_prev
        }

        // Store factor and loads
        factors.AppendRight(t);
        loads.AppendBottom(l);

        // 5. Calculate the residual matrix X = X - t*l
        SubtractProductFromSelf(t, l, X); // X is now the residual matrix

        // if the number of factors to calculate is not provided,
        // calculate the norm of the residual matrix and compare with the original
        // one
        if (numFactors <= 0 && !(residualVarianceVector is null))
        {
          double residualVariance = Math.Sqrt(MatrixMath.SumOfSquares(X));
          residualVarianceVector.AppendBottom(new MatrixMath.ScalarAsMatrix<double>(residualVariance));

          if (residualVariance <= accuracy * originalVariance)
          {
            break;
          }
        }
      } // for all factors
    } // end NIPALS

    public (Matrix<double> W, Matrix<double> H) Factorize(Matrix<double> A, int rank)
    {
      var factors = new MatrixMath.TopSpineJaggedArrayMatrix<double>(0, 0);
      var loads = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var residualVariances = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      NIPALS_HO(A, rank, 1E-9, factors, loads, residualVariances);
      var W = CreateMatrix.Dense<double>(A.RowCount, rank);
      var H = CreateMatrix.Dense<double>(rank, A.ColumnCount);
      MatrixMath.Copy(factors, W);
      MatrixMath.Copy(loads, H);
      return (W, H);
    }
  }
}
