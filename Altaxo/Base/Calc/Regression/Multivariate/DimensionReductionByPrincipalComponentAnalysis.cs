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

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Dimension reduction using principal component analysis (PCA).
  /// </summary>
  [Description("Principal component analysis (PCA)")]
  public record DimensionReductionByPrincipalComponentAnalysis : DimensionReductionByFactorizationMethod
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-09.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionByPrincipalComponentAnalysis), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionByPrincipalComponentAnalysis)obj;
        info.AddValue("MaxNumberOfFactors", s.MaximumNumberOfFactors);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int maxNumberOfFactors = info.GetInt32("MaxNumberOfFactors");

        return o is DimensionReductionByPrincipalComponentAnalysis s
          ? s with { MaximumNumberOfFactors = maxNumberOfFactors }
          : new DimensionReductionByPrincipalComponentAnalysis { MaximumNumberOfFactors = maxNumberOfFactors };
      }
    }
    #endregion


    /// <inheritdoc/>
    public override string DisplayName => "PrincipalComponentAnalysis";

    /// <inheritdoc/>
    public override DimensionReductionByFactorizationResult ExecuteDimensionReduction(IROMatrix<double> matrixX)
    {
      var matrix = CreateMatrix.Dense<double>(matrixX.RowCount, matrixX.ColumnCount);

      MatrixMath.Copy(matrixX, matrix);

      // now do PCA with the matrix
      var factors = new MatrixMath.TopSpineJaggedArrayMatrix<double>(0, 0);
      var loads = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var residualVariances = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
      // first, center the matrix
      MatrixMath.ColumnsToZeroMean(matrix, meanX);
      MatrixMath.NIPALS_HO(matrix, MaximumNumberOfFactors, 1E-9, factors, loads, residualVariances);

      return new DimensionReductionByFactorizationResult(factors, loads, residualVariances, meanX);
    }
  }
}
