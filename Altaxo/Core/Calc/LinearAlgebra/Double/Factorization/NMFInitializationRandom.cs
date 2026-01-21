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
  /// Provides a random initialization for non-negative matrix factorization (NMF).
  /// </summary>
  /// <remarks>
  /// The generated factors are scaled to match the average magnitude of <paramref name="X"/> and are constrained to be
  /// strictly positive to avoid zero entries.
  /// </remarks>
  public record NMFInitializationRandom : INonnegativeMatrixFactorizationInitializer, Main.IImmutable
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NMFInitializationRandom), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NMFInitializationRandom)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return ((o as NMFInitializationRandom) ?? new NMFInitializationRandom());
      }
    }

    #endregion
    /// <inheritdoc/>
    public (Matrix<double> W, Matrix<double> H) GetInitialFactors(Matrix<double> X, int r)
    {
      int m = X.RowCount;
      int n = X.ColumnCount;

      var W = CreateMatrix.Dense<double>(m, r);
      var H = CreateMatrix.Dense<double>(r, n);

      var rnd = System.Random.Shared;
      var scale = Math.Sqrt(X.FrobeniusNorm() / ((double)X.RowCount * X.ColumnCount)); // scale the random values with the inverse sqrt of the average element magnitude

      for (int i = 0; i < m; i++)
      {
        for (int j = 0; j < r; j++)
        {
          W[i, j] = scale * Math.Max(1E-6, rnd.NextDouble()); // ensure that never any element is zero
        }
      }

      for (int i = 0; i < r; i++)
      {
        for (int j = 0; j < n; j++)
        {
          H[i, j] = scale * Math.Max(1E-6, rnd.NextDouble()); // ensure that never any element is zero
        }
      }

      return (W, H);
    }
  }
}
