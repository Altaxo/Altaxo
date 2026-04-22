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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.LinearAlgebra.Double.Factorization;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Base class for dimension reduction analyses based on a factorization, for example principal component analysis (PCA)
  /// or non-negative matrix factorization (NMF).
  /// </summary>
  public record DimensionReductionByLowRankFactorization : IDimensionReductionMethod
  {
    /// <summary>
    /// Gets the factorization method.
    /// </summary>
    public ILowRankMatrixFactorization Method { get; init; } = new PrincipalComponentAnalysisBySVD();

    /// <summary>
    /// Gets the maximum number of factors to calculate.
    /// </summary>
    /// <value>
    /// The maximum number of factors.
    /// </value>
    public int MaximumNumberOfFactors { get; init; } = 3;


    /// <summary>
    /// Gets the normalization method applied to loadings and scores.
    /// </summary>
    /// <remarks>This property determines how the loadings and scores are scaled or transformed. The selected
    /// normalization affects the interpretation of the results and may impact downstream analysis.</remarks>
    public ScoresAndLoadingsNormalization Normalization { get; init; } = ScoresAndLoadingsNormalization.Native;


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-09.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Multivariate.DimensionReductionByLowRankFactorization", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionByLowRankFactorization)o;
        info.AddValue("MaximumNumberOfFactors", s.MaximumNumberOfFactors);
        info.AddValue("Method", s.Method);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maxNumberOfFactors = info.GetInt32("MaximumNumberOfFactors");
        var method = info.GetValue<ILowRankMatrixFactorization>("Method", parent);
        return ((o as DimensionReductionByLowRankFactorization) ?? new DimensionReductionByLowRankFactorization())
          with
        {
          MaximumNumberOfFactors = maxNumberOfFactors,
          Method = method,
        };

      }
    }

    /// <summary>
    /// XML serialization surrogate (version 1).
    /// </summary>
    /// <remarks>V0: 2026-04-22 added property <c>Normalization</c>.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionByLowRankFactorization), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionByLowRankFactorization)o;
        info.AddValue("MaximumNumberOfFactors", s.MaximumNumberOfFactors);
        info.AddValue("Method", s.Method);
        info.AddEnum("Normalization", s.Normalization);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maxNumberOfFactors = info.GetInt32("MaximumNumberOfFactors");
        var method = info.GetValue<ILowRankMatrixFactorization>("Method", parent);
        var normalization = info.GetEnum<ScoresAndLoadingsNormalization>("Normalization");
        return ((o as DimensionReductionByLowRankFactorization) ?? new DimensionReductionByLowRankFactorization())
          with
        {
          MaximumNumberOfFactors = maxNumberOfFactors,
          Method = method,
          Normalization = normalization,
        };
      }
    }
    #endregion

    /// <summary>
    /// Gets a display name for the specified factorization method type.
    /// </summary>
    /// <param name="t">The factorization method type.</param>
    /// <returns>A user-friendly display name.</returns>
    public static string GetDisplayName(System.Type t)
    {
      switch (t)
      {
        case Type _ when t == typeof(NonnegativeMatrixFactorizationByMultiplicativeUpdate):
          return "Non-negative Matrix Factorization (NMF) by mult. update";
        case Type _ when t == typeof(NonnegativeMatrixFactorizationByACLS):
          return "Non-negative Matrix Factorization (NMF) by ACLS";
        case Type _ when t == typeof(NonnegativeMatrixFactorizationByHALS):
          return "Non-negative Matrix Factorization (NMF) by HALS";
        case Type _ when t == typeof(PrincipalComponentAnalysisBySVD):
          return "Principal Component Analysis (PCA) by SVD";
        case Type _ when t == typeof(PrincipalComponentAnalysisByNIPALS):
          return "Principal Component Analysis (PCA) by NIPALS";
        default:
          return t.Name;
      }
    }

    /// <inheritdoc />
    public virtual string DisplayName
    {
      get => GetDisplayName(Method.GetType());

    }

    /// <inheritdoc />
    public virtual IDimensionReductionResult ExecuteDimensionReduction(IROMatrix<double> processData)
    {
      var matrix = CreateMatrix.Dense<double>(processData.RowCount, processData.ColumnCount);
      MatrixMath.Copy(processData, matrix);
      var (scores, loadings) = Method.Factorize(matrix, MaximumNumberOfFactors);
      NormalizeScoresAndLoadings(scores, loadings);
      return new DimensionReductionByFactorizationResult(scores, loadings, null, null);
    }

    /// <summary>
    /// Normalizes the provided scores and loadings matrices according to the configured normalization method.
    /// </summary>
    /// <param name="scores">The matrix containing the scores to be normalized. The normalization is applied in place.</param>
    /// <param name="loadings">The matrix containing the loadings to be normalized. The normalization is applied in place.</param>
    /// <exception cref="InvalidOperationException">Thrown if the configured normalization method is not supported.</exception>
    public void NormalizeScoresAndLoadings(Matrix<double> scores, Matrix<double> loadings)
    {
      switch (Normalization)
      {
        case ScoresAndLoadingsNormalization.Native:
          // No normalization applied.
          break;
        case ScoresAndLoadingsNormalization.AbsMaxScores:
          {
            for (int iC = 0; iC < scores.ColumnCount; ++iC)
            {
              var maxAbsValue = 0.0;
              for (int iR = 0; iR < scores.RowCount; ++iR)
              {
                maxAbsValue = Math.Max(maxAbsValue, Math.Abs(scores[iR, iC]));
              }
              if (maxAbsValue > 0.0)
              {
                for (int iR = 0; iR < scores.RowCount; ++iR)
                {
                  scores[iR, iC] /= maxAbsValue;
                }
                for (int iCL = 0; iCL < loadings.ColumnCount; ++iCL)
                {
                  loadings[iC, iCL] *= maxAbsValue;
                }
              }
            }
          }
          break;
        case ScoresAndLoadingsNormalization.AbsMaxLoadings:
          {
            for (int iC = 0; iC < scores.ColumnCount; ++iC)
            {
              var maxAbsValue = 0.0;
              for (int iCL = 0; iCL < loadings.ColumnCount; ++iCL)
              {
                maxAbsValue = Math.Max(maxAbsValue, Math.Abs(loadings[iC, iCL]));
              }
              if (maxAbsValue > 0.0)
              {
                for (int iCL = 0; iCL < loadings.ColumnCount; ++iCL)
                {
                  loadings[iC, iCL] /= maxAbsValue;
                }
                for (int iR = 0; iR < scores.RowCount; ++iR)
                {
                  scores[iR, iC] *= maxAbsValue;
                }
              }
            }
          }
          break;
        default:
          throw new InvalidOperationException($"Unsupported normalization method: {Normalization}");
      }
    }
  }
}
