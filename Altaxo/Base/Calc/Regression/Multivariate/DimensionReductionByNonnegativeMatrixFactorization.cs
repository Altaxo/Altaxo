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

using System.ComponentModel;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.LinearAlgebra.Factorization;

namespace Altaxo.Calc.Regression.Multivariate.Foo
{

  /// <summary>
  /// Dimension reduction using nonnegative matrix factorization (NMF).
  /// </summary>
  [Description("Nonnegative matrix factorization (NMF)")]
  public record DimensionReductionByNonnegativeMatrixFactorization : DimensionReductionByFactorizationMethod
  {
    /// <summary>
    /// Gets the initialization method used for the NMF algorithm.
    /// </summary>
    public NonnegativeMatrixFactorizationInitializationMethod InitializationMethod { get; init; } = NonnegativeMatrixFactorizationInitializationMethod.Random;

    /// <summary>
    /// Gets the maximum number of iterations used for each NMF run.
    /// </summary>
    public int MaximumNumberOfIterations { get; init; } = 1000;

    /// <summary>
    /// Gets the number of restarts (independent initializations) used for the NMF algorithm.
    /// </summary>
    public int Restarts { get; init; } = 3;

    /// <summary>
    /// Gets the convergence tolerance for the NMF algorithm.
    /// </summary>
    public double Tolerance { get; init; } = 1e-5;



    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-09.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionByNonnegativeMatrixFactorization), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionByNonnegativeMatrixFactorization)obj;
        info.AddValue("MaximumNumberOfFactors", s.MaximumNumberOfFactors);
        info.AddEnum("InitializationMethod", s.InitializationMethod);
        info.AddValue("MaximumNumberOfIterations", s.MaximumNumberOfIterations);
        info.AddValue("Restarts", s.Restarts);
        info.AddValue("Tolerance", s.Tolerance);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int maxNumberOfFactors = info.GetInt32("MaximumNumberOfFactors");
        var initializationMethod = info.GetEnum<NonnegativeMatrixFactorizationInitializationMethod>("InitializationMethod");
        int maximumNumberOfIterations = info.GetInt32("MaximumNumberOfIterations");
        int restarts = info.GetInt32("Restarts");
        double tolerance = info.GetDouble("Tolerance");

        return ((o as DimensionReductionByNonnegativeMatrixFactorization) ?? new DimensionReductionByNonnegativeMatrixFactorization())
          with
        {
          MaximumNumberOfFactors = maxNumberOfFactors,
          InitializationMethod = initializationMethod,
          MaximumNumberOfIterations = maximumNumberOfIterations,
          Restarts = restarts,
          Tolerance = tolerance,
        };

      }
    }
    #endregion


    /// <inheritdoc/>
    public override string DisplayName => "NonnegativeMatrixFactorization";

    /// <inheritdoc/>
    public override DimensionReductionByFactorizationResult ExecuteDimensionReduction(IROMatrix<double> matrixX)
    {
      var matrix = CreateMatrix.Dense<double>(matrixX.RowCount, matrixX.ColumnCount);
      MatrixMath.Copy(matrixX, matrix);
      var (scores, loadings, _) = NonnegativeMatrixFactorizationByMultiplicativeUpdate.Evaluate(matrix, MaximumNumberOfFactors);
      return new DimensionReductionByFactorizationResult(scores, loadings, null, null);
    }
  }
}
