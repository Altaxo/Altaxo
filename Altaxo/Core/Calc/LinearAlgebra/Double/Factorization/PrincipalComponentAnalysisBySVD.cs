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

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Provides principal component analysis (PCA) by singular value decomposition (SVD).
  /// </summary>
  public class PrincipalComponentAnalysisBySVD : ILowRankMatrixFactorization
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PrincipalComponentAnalysisBySVD), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PrincipalComponentAnalysisBySVD)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return ((o as PrincipalComponentAnalysisBySVD) ?? new PrincipalComponentAnalysisBySVD());
      }
    }

    #endregion

    /// <inheritdoc/>
    public (Matrix<double> W, Matrix<double> H) Factorize(Matrix<double> data, int numberOfComponents)
    {
      int numberOfSpectra = data.RowCount;
      int spectralPoints = data.ColumnCount;

      var svd = data.Svd();

      var mfactors = CreateMatrix.Dense<double>(numberOfSpectra, numberOfComponents);
      for (int i = 0; i < numberOfComponents; i++)
      {
        var singularValue = svd.S[i];
        for (int r = 0; r < numberOfSpectra; r++)
        {
          mfactors[r, i] = svd.U[r, i] * singularValue;
        }
      }
      var mloads = svd.VT.SubMatrix(0, numberOfComponents, 0, spectralPoints);
      return (mfactors, mloads);
    }
  }
}
