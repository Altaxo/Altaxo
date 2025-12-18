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

using System;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public class NonnegativeMatrixFactorizationByCoordinateDescent
  {
    public int Rank { get; }
    public int MaxIter { get; }
    public double Lambda { get; }      // L2-Regularisierung
    public double Tol { get; }         // Abbruch-Toleranz
    public double Damping { get; }     // Dämpfung bei Fehleranstieg (0..1)

    public Matrix<double> W { get; private set; }
    public Matrix<double> H { get; private set; }

    public NonnegativeMatrixFactorizationByCoordinateDescent(int rank, int maxIter = 500, double lambda = 1e-4, double tol = 1e-6, double damping = 0.5)
    {
      Rank = rank;
      MaxIter = maxIter;
      Lambda = lambda;
      Tol = tol;
      Damping = Math.Min(Math.Max(damping, 0.0), 1.0);
    }

    public double Fit(Matrix<double> V)
    {
      int m = V.RowCount;
      int n = V.ColumnCount;
      double eps = 1e-12;

      // Initialisierung: nicht-negative Startwerte
      W = Matrix<double>.Build.Random(m, Rank).PointwiseAbs().PointwiseMaximum(eps);
      H = Matrix<double>.Build.Random(Rank, n).PointwiseAbs().PointwiseMaximum(eps);

      // Optionale Anfangsreskalierung
      RescaleFactors(W, H, eps);

      double vNorm = V.FrobeniusNorm();
      double prevRelErr = double.PositiveInfinity;

      for (int iter = 0; iter < MaxIter; iter++)
      {
        // === Update H (spaltenweise) ===
        var WtW = W.TransposeThisAndMultiply(W);
        WtW = AddDiagonal(WtW, Lambda); // Regularisierung

        var H_proposed = H.Clone();
        for (int j = 0; j < n; j++)
        {
          Vector<double> vj = V.Column(j);                 // m-Vector
          Vector<double> rhsH = W.TransposeThisAndMultiply(vj); // r-Vector
          Vector<double> hj = WtW.Solve(rhsH);             // r-Vector
          for (int k = 0; k < Rank; k++) hj[k] = Math.Max(hj[k], eps);
          H_proposed.SetColumn(j, hj);
        }

        // Safeguard: nur übernehmen, wenn Fehler nicht steigt, sonst gedämpft
        double errBeforeH = RelativeError(V, W, H, vNorm);
        double errAfterHProposed = RelativeError(V, W, H_proposed, vNorm);

        if (errAfterHProposed <= errBeforeH)
          H = H_proposed;
        else
          H = Blend(H, H_proposed, Damping); // gedämpftes Update

        H = H.PointwiseMaximum(eps);

        // Reskalierung nach H-Update
        RescaleFactors(W, H, eps);

        // === Update W (zeilenweise) ===
        var HHT = H * H.Transpose();
        HHT = AddDiagonal(HHT, Lambda); // Regularisierung

        var W_proposed = W.Clone();
        for (int i = 0; i < m; i++)
        {
          Vector<double> vi = V.Row(i);   // n-Vector
          Vector<double> rhsW = H * vi;   // r-Vector
          Vector<double> wi = HHT.Solve(rhsW); // r-Vector
          for (int k = 0; k < Rank; k++) wi[k] = Math.Max(wi[k], eps);
          W_proposed.SetRow(i, wi);
        }

        // Safeguard: W-Update prüfen
        double errBeforeW = RelativeError(V, W, H, vNorm);
        double errAfterWProposed = RelativeError(V, W_proposed, H, vNorm);

        if (errAfterWProposed <= errBeforeW)
          W = W_proposed;
        else
          W = Blend(W, W_proposed, Damping);

        W = W.PointwiseMaximum(eps);

        // Reskalierung nach W-Update
        RescaleFactors(W, H, eps);

        // Monitoring & Abbruchkriterium
        double relErr = RelativeError(V, W, H, vNorm);
        if (iter % 10 == 0)
          Console.WriteLine($"Iter {iter}: relErr = {relErr:E6}");

        if (Math.Abs(prevRelErr - relErr) < Tol)
          break;

        prevRelErr = relErr;
      }

      return prevRelErr;
    }

    public Matrix<double> Reconstruct() => W * H;

    // Hilfsfunktionen

    private static Matrix<double> AddDiagonal(Matrix<double> A, double lambda)
    {
      if (lambda <= 0) return A;
      var D = Matrix<double>.Build.DenseDiagonal(A.RowCount, A.ColumnCount, lambda);
      return A + D;
    }

    private static void RescaleFactors(Matrix<double> W, Matrix<double> H, double eps)
    {
      int r = W.ColumnCount;
      for (int k = 0; k < r; k++)
      {
        double norm = W.Column(k).L2Norm();
        if (norm > 0)
        {
          W.SetColumn(k, W.Column(k) / norm);
          H.SetRow(k, H.Row(k) * norm);
        }
        // Floors
        W.SetColumn(k, W.Column(k).PointwiseMaximum(eps));
        H.SetRow(k, H.Row(k).PointwiseMaximum(eps));
      }
    }

    private static double RelativeError(Matrix<double> V, Matrix<double> W, Matrix<double> H, double vNorm)
    {
      var Vhat = W * H;
      return (V - Vhat).FrobeniusNorm() / vNorm;
    }

    private static Matrix<double> Blend(Matrix<double> A, Matrix<double> B, double alpha)
    {
      // A_new = (1 - alpha) * A + alpha * B
      return A * (1.0 - alpha) + B * alpha;
    }
  }
}
