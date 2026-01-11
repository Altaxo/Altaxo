#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public enum NonnegativeMatrixFactorizationInitializationMethod
  {
    Random,

    NNDSVD,

    NNDSVDa,

    NNDSVDar,
  };

  public class NonnegativeMatrixFactorizationBase
  {
    private static (Matrix<double> W0, Matrix<double> H0) NNDSVDRaw(Matrix<double> X, int r)
    {
      var svd = X.Svd(computeVectors: true);
      var U = svd.U;          // m x m (oder m x k)
      var S = svd.S;          // Diagonalwerte als Vector
      var Vt = svd.VT;        // n x n (oder k x n)

      int m = X.RowCount;
      int n = X.ColumnCount;
      var W0 = Matrix<double>.Build.Dense(m, r);
      var H0 = Matrix<double>.Build.Dense(r, n);

      // Erste Komponente
      var u0 = U.Column(0);
      var v0 = Vt.Row(0); // V^T erste Zeile
      double s0 = S[0];
      var u0p = u0.PointwiseMaximum(0.0);
      var v0p = v0.PointwiseMaximum(0.0);
      double a0 = u0p.L2Norm();
      double b0 = v0p.L2Norm();
      if (a0 > 0 && b0 > 0)
      {
        W0.SetColumn(0, u0p / a0);
        H0.SetRow(0, v0p / b0 * (s0 * a0 * b0));
      }
      else
      {
        W0.SetColumn(0, u0.PointwiseAbs());
        H0.SetRow(0, v0.PointwiseAbs() * s0);
      }

      // Weitere Komponenten
      for (int j = 1; j < r && j < S.Count; j++)
      {
        var uj = U.Column(j);
        var vj = Vt.Row(j);
        double sj = S[j];

        var up = uj.PointwiseMaximum(0.0);
        var un = uj.PointwiseMinimum(0.0).PointwiseAbs();
        var vp = vj.PointwiseMaximum(0.0);
        var vn = vj.PointwiseMinimum(0.0).PointwiseAbs();

        double upNorm = up.L2Norm();
        double vpNorm = vp.L2Norm();
        double unNorm = un.L2Norm();
        double vnNorm = vn.L2Norm();

        // wähle positivere Variante
        var uComp = upNorm * vpNorm >= unNorm * vnNorm ? up : un;
        var vComp = upNorm * vpNorm >= unNorm * vnNorm ? vp : vn;

        double a = uComp.L2Norm();
        double b = vComp.L2Norm();
        if (a > 0)
          uComp = uComp / a;
        if (b > 0)
          vComp = vComp / b;

        W0.SetColumn(j, uComp);
        H0.SetRow(j, vComp * (sj * a * b));
      }

      return (W0, H0); // note that both W0 and H0 are non-negative, but can contain zeros! Those zeros should be handled in the calling function.
    }

    public static (Matrix<double> W0, Matrix<double> H0) NNDSVD(Matrix<double> X, int r)
    {
      var (W0, H0) = NNDSVDRaw(X, r);

      // Kleine Offsets gegen Nullen
      W0 = W0.PointwiseMaximum(1e-12);
      H0 = H0.PointwiseMaximum(1e-12);
      return (W0, H0);
    }

    public static (Matrix<double> W0, Matrix<double> H0) NNDSVDa(Matrix<double> X, int r)
    {
      // Erst normale NNDSVD erzeugen
      var (W0, H0) = NNDSVDRaw(X, r);

      // Mittelwert der Datenmatrix (nur positive Werte)
      double avg = X.Enumerate().Where(v => v > 0).DefaultIfEmpty(0.0).Average();
      double eps = avg * 1e-4; // paper Boutsidis & Gallopoulos, 2008, https://doi.org/10.1016/j.patcog.2007.09.010

      // Falls avg == 0 (extrem selten), fallback
      if (eps == 0.0)
        eps = 1e-4;

      // Alle Nullen ersetzen
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

    public static (Matrix<double> W0, Matrix<double> H0) NNDSVDar(Matrix<double> X, int r)
    {
      // Erst NNDSVDa erzeugen
      var (W0, H0) = NNDSVDa(X, r);

      // Mittelwert der Datenmatrix (nur positive Werte)
      double avg = X.Enumerate().Where(v => v > 0).DefaultIfEmpty(0.0).Average();
      double eps = avg * 1e-4;

      if (eps == 0.0)
        eps = 1e-4;

      // Zufallsinstanz
      var rnd = System.Random.Shared;

      // Zufallsrauschen hinzufügen
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
