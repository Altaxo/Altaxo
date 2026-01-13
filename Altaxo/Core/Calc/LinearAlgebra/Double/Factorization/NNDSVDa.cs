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
  public record NNDSVDa : NNDSVD
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NNDSVDa), 0)]
    public class SerializationSurrogate0a : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NNDSVDa)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return ((o as NNDSVDa) ?? new NNDSVDa());
      }
    }

    #endregion


    /// <summary>
    /// Creates an NNDSVDa initialization for NMF by replacing zeros with a data-dependent small value.
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>A tuple <c>(W0, H0)</c> containing non-negative initial factors with zeros replaced by a small value.</returns>
    public override (Matrix<double> W, Matrix<double> H) GetInitialFactors(Matrix<double> X, int r)
    {
      // First, create the standard NNDSVD initialization
      var (W0, H0) = base.GetInitialFactors(X, r);

      // Mean of the data matrix (positive values only)
      double avg = X.Enumerate().Where(v => v > 0).DefaultIfEmpty(0.0).Average();
      double eps = avg * 1e-4; // paper Boutsidis & Gallopoulos, 2008, https://doi.org/10.1016/j.patcog.2007.09.010

      // If avg == 0 (extremely rare), fallback
      if (eps == 0.0)
        eps = 1e-4;

      // Replace all zeros
      for (int i = 0; i < W0.RowCount; i++)
        for (int j = 0; j < W0.ColumnCount; j++)
          if (W0[i, j] == 0.0)
            W0[i, j] = eps;

      for (int i = 0; i < H0.RowCount; i++)
        for (int j = 0; j < H0.ColumnCount; j++)
          if (H0[i, j] == 0.0)
            H0[i, j] = eps;

      return (W0, H0);
    }
  }
}
