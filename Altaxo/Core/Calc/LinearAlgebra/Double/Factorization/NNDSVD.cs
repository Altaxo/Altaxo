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
  public record NNDSVD : INonnegativeMatrixFactorizationInitializer, Main.IImmutable
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-13.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NNDSVD), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NNDSVD)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return ((o as NNDSVD) ?? new NNDSVD());
      }
    }

    #endregion


    /// <summary>
    /// Creates an NNDSVD initialization for NMF. Intentionally allows zeros in the result (algorithm for sparse matrices).
    /// </summary>
    /// <param name="X">The non-negative input matrix to be factorized.</param>
    /// <param name="r">The target factorization rank.</param>
    /// <returns>
    /// A tuple <c>(W0, H0)</c> containing non-negative initial factors. Both factors may contain zeros; callers are expected
    /// to handle zeros as needed (e.g. by adding small offsets).
    /// </returns>
    /// <remarks>
    /// Intentionally allows zeros in the result (algorithm for sparse matrices).
    /// References: <see href="https://doi.org/10.1016/j.patcog.2007.09.010">Boutsidis, C., Gallopoulos, E., SVD-based initialization: A head start for nonnegative matrix factorization, Pattern Recognition, Volume 41, Issue 4, April 2008, Pages 1350-1362</see>.
    /// </remarks>
    public virtual (Matrix<double> W, Matrix<double> H) GetInitialFactors(Matrix<double> X, int r)
    {
      var svd = X.Svd(computeVectors: true); // TODO: replace by a partial SVD (psvd) of the first r factors,see e.g.https://scikit-learn.org/stable/modules/generated/sklearn.decomposition.TruncatedSVD.html# Math.NET Numerics Extended
      var U = svd.U;          // m x m (or m x k)
      var S = svd.S;          // singular values as a Vector
      var Vt = svd.VT;        // n x n (or k x n)

      int m = X.RowCount;
      int n = X.ColumnCount;
      var W = Matrix<double>.Build.Dense(m, r);
      var H = Matrix<double>.Build.Dense(r, n);

      // First component
      // Note that the first component can be used directly
      W.SetColumn(0, U.Column(0).PointwiseAbs() * Math.Sqrt(S[0]));
      H.SetRow(0, Vt.Row(0).PointwiseAbs() * Math.Sqrt(S[0]));

      // Further components
      for (int j = 1; j < r && j < S.Count; j++)
      {
        var xj = U.Column(j);
        var yj = Vt.Row(j);
        double sj = S[j];

        var xp = xj.PointwiseMaximum(0.0);
        var xn = xj.PointwiseMinimum(0.0).PointwiseAbs();
        var yp = yj.PointwiseMaximum(0.0);
        var yn = yj.PointwiseMinimum(0.0).PointwiseAbs();

        double xpnrm = xp.L2Norm();
        double ypnrm = yp.L2Norm();
        double mp = xpnrm * ypnrm;
        double xnnrm = xn.L2Norm();
        double ynnrm = yn.L2Norm();
        double mn = xnnrm * ynnrm;

        // Choose the more positive variant
        Vector<double> u, v;
        if (mp > mn)
        {
          u = xp * (Math.Sqrt(sj * mp) / xpnrm);
          v = yp * (Math.Sqrt(sj * mp) / ypnrm);
        }
        else
        {
          u = xn * (Math.Sqrt(sj * mn) / xnnrm);
          v = yn * (Math.Sqrt(sj * mn) / ynnrm);
        }

        W.SetColumn(j, u);
        H.SetRow(j, v);
      }

      return (W, H); // note that both W0 and H0 are non-negative, but can contain zeros! Those zeros should be handled in the calling function.
    }
  }
}
