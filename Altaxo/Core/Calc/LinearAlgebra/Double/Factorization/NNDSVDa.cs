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
    /// <returns>A tuple <c>(W0, H0)</c> containing non-negative initial factors with zeros replaced by a reasonable value (see remarks!).</returns>
    /// <remarks>In the paper Boutsidis & Gallopoulos, 2008, https://doi.org/10.1016/j.patcog.2007.09.010, section 2.3,
    /// the zero elements are overwritten with the mean of matrix X.
    /// I consider this wrong, because W and H scale with the square root of X, not with X itself.
    /// Therefore, for example, if X is scaled with 1E20, W and H are scaled with 1E10, but the zeros would then be replaced with a scale of 1E20 again.
    /// Thus, I decided not to use X, but to overwrite the zeros with the average values of W and H, respectively.
    /// This also avoids peculiarities if the average of X is zero.</remarks>
    public override (Matrix<double> W, Matrix<double> H) GetInitialFactors(Matrix<double> X, int r)
    {
      // First, create the standard NNDSVD initialization
      var (W0, H0) = base.GetInitialFactors(X, r);

      var avgW0 = MatrixMath.Average(W0);
      var avgH0 = MatrixMath.Average(H0);

      // replace the zeros of the matrixes with the average value
      for (int i = 0; i < W0.RowCount; i++)
        for (int j = 0; j < W0.ColumnCount; j++)
          if (W0[i, j] == 0)
            W0[i, j] = avgW0;

      for (int i = 0; i < H0.RowCount; i++)
        for (int j = 0; j < H0.ColumnCount; j++)
          if (H0[i, j] == 0)
            H0[i, j] = avgH0;

      return (W0, H0);
    }
  }
}
