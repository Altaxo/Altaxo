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


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-09.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionByLowRankFactorization), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionByLowRankFactorization)obj;
        info.AddValue("MaximumNumberOfFactors", s.MaximumNumberOfFactors);
        info.AddValue("Method", s.Method);
      }

      /// <inheritdoc/>
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
    #endregion

    /// <inheritdoc/>
    public static string GetDisplayName(System.Type t)
    {
      switch (t)
      {
        case Type _ when t == typeof(NonnegativeMatrixFactorizationByMultiplicativeUpdate):
          return "Non-negative Matrix Factorization (NMF)";
        case Type _ when t == typeof(NonnegativeMatrixFactorizationByACLS):
          return "Non-negative Matrix Factorization (NMF) by ACLS";
        case Type _ when t == typeof(NonnegativeMatrixFactorizationByCoordinateDescent):
          return "Non-negative Matrix Factorization (NMF) by Coordinate Descent";
        case Type _ when t == typeof(PrincipalComponentAnalysisBySVD):
          return "Principal Component Analysis (PCA) by SVD";
        case Type _ when t == typeof(PrincipalComponentAnalysisByNIPALS):
          return "Principal Component Analysis (PCA) by NIPALS";
        default:
          return t.Name;
      }
    }

    /// <inheritdoc/>
    public virtual string DisplayName
    {
      get => GetDisplayName(Method.GetType());

    }

    /// <inheritdoc/>
    public virtual IDimensionReductionResult ExecuteDimensionReduction(IROMatrix<double> matrixX)
    {
      var matrix = CreateMatrix.Dense<double>(matrixX.RowCount, matrixX.ColumnCount);
      MatrixMath.Copy(matrixX, matrix);
      var (scores, loadings) = Method.Factorize(matrix, MaximumNumberOfFactors);
      return new DimensionReductionByFactorizationResult(scores, loadings, null, null);
    }
  }
}
