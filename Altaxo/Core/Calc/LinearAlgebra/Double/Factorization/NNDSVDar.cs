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

using System.Linq;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  public record NNDSVDar : NNDSVDa
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NNDSVDar), 0)]
    public class SerializationSurrogate0ar : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NNDSVDar)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return ((o as NNDSVDar) ?? new NNDSVDar());
      }
    }

    #endregion

    /// <summary>
    /// Creates an NNDSVDar initialization for NMF by applying NNDSVDa and adding small random noise.
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>A tuple <c>(W0, H0)</c> containing non-negative initial factors with small random perturbations.</returns>
    /// <remarks>
    /// References: <see href="https://doi.org/10.1016/j.patcog.2007.09.010">Boutsidis, C., Gallopoulos, E., SVD based initialization: A head start for nonnegative matrix factorization, Pattern Recognition, Volume 41, Issue 4, April 2008, Pages 1350-1362</see>
    /// </remarks>
    public override (Matrix<double> W, Matrix<double> H) GetInitialFactors(Matrix<double> X, int r)
    {
      // First, create NNDSVDa
      var (W0, H0) = base.GetInitialFactors(X, r);

      // Mean of the data matrix (positive values only)
      double avg = X.Enumerate().Where(v => v > 0).DefaultIfEmpty(0.0).Average();
      double eps = avg * 1e-4;

      if (eps == 0.0)
        eps = 1e-4;

      // Random number generator
      var rnd = System.Random.Shared;

      // Add random noise
      for (int i = 0; i < W0.RowCount; i++)
        for (int j = 0; j < W0.ColumnCount; j++)
          W0[i, j] += rnd.NextDouble() * eps;

      for (int i = 0; i < H0.RowCount; i++)
        for (int j = 0; j < H0.ColumnCount; j++)
          H0[i, j] += rnd.NextDouble() * eps;

      return (W0, H0);
    }
  }
}
