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
  /// Non-negative matrix factorization (NMF) using the classic multiplicative update rules.
  /// </summary>
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
        var maximumNumberOfIterations = info.GetValue<int>("MaximumNumberOfIterations", parent);
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

    /// <summary>
    /// Factorizes a non-negative matrix <paramref name="V"/> into non-negative factors <c>W</c> and <c>H</c> using multiplicative updates.
    /// </summary>
    /// <param name="V">The input matrix to factorize.</param>
    /// <param name="rank">The factorization rank.</param>
    /// <returns>
    /// A tuple containing the factors <c>W</c> and <c>H</c> and the final relative reconstruction error <c>relErr</c>.
    /// </returns>
    public override (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> V, int rank)
    {
      //    Matrix<double> V, int r, int maxIter = 2000, double tol = 1e-5, int restarts = 3)
      int m = V.RowCount;
      int n = V.ColumnCount;
      double eps = 1e-12;

      var (W, H) = InitializationMethod.GetInitialFactors(V, rank);

      double prevErr = double.PositiveInfinity;
      double vNorm = V.FrobeniusNorm();

      for (int iter = 0; iter < MaximumNumberOfIterations; iter++)
      {
        var WH = W * H;

        // Update H
        var numeratorH = W.Transpose() * V;
        var denominatorH = W.Transpose() * WH + eps;
        H = H.PointwiseMultiply(numeratorH.PointwiseDivide(denominatorH));

        // Update W
        WH = W * H;
        var numeratorW = V * H.Transpose();
        var denominatorW = WH * H.Transpose() + eps;
        W = W.PointwiseMultiply(numeratorW.PointwiseDivide(denominatorW));

        // Stabilization
        W = W.PointwiseMaximum(eps);
        H = H.PointwiseMaximum(eps);

        // Optional: rescaling for conditioning
        for (int k = 0; k < rank; k++)
        {
          double normWk = W.Column(k).L2Norm();
          if (normWk > 0)
          {
            W.SetColumn(k, W.Column(k) / normWk);
            H.SetRow(k, H.Row(k) * normWk);
          }
        }

        // Monitoring
        var Vhat = W * H;
        double err = (V - Vhat).FrobeniusNorm() / vNorm;

        if (Math.Abs(prevErr - err) < Tolerance)
          break;
        prevErr = err;
      }
      return (W, H);
    }
  }
}
