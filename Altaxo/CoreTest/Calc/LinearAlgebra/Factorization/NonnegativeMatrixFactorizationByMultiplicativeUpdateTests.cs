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
#endregion
using System;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  // Test matrices see https://resources.wolframcloud.com/FunctionRepository/resources/NonNegativeMatrixFactorization/
  // https://github.com/trigeorgis/Deep-Semi-NMF/blob/master/matlab/NNDSVD.m

  public class NonnegativeMatrixFactorizationByMultiplicativeUpdateTests : FactorizationTestBase
  {
    [Fact]
    public void Test1()
    {
      int m = 50;   // Zeilen
      int n = 40;   // Spalten
      int r = 10;   // Rang


      var W_true = Matrix<double>.Build.Random(m, r).PointwiseAbs();
      var H_true = Matrix<double>.Build.Random(r, n).PointwiseAbs();
      var V = W_true * H_true;

      var (W, H, relErr) = NonnegativeMatrixFactorizationByMultiplicativeUpdate.Evaluate(V, r, maxIter: 5000, tol: 1e-7, restarts: 5);
      Console.WriteLine($"Relativer Fehler ||V - WH||_F / ||V||_F = {relErr:E3}");

      var byDescent = new NonnegativeMatrixFactorizationByCoordinateDescent(r, maxIter: 5000);
      var relErr2 = byDescent.Fit(V);
      Console.WriteLine($"Relativer Fehler (Koordinatenabstieg) ||V - WH||_F / ||V||_F = {relErr2:E3}");
    }

    [Fact]
    public void Test2()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();

      var (mfactors, mloads, relErr) = NonnegativeMatrixFactorizationByMultiplicativeUpdate.Evaluate(matrixX, NumberOfComponents, maxIter: 1000, tol: 1e-7, restarts: 5);

      var relError = RelativeError(mfactors, mloads, originalMatrix);
      Assert.True(relError < 0.0033);
    }
  }
}

