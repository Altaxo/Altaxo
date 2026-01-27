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
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  public class FactorizationByFastIndependentComponentAnalysisTests
  {
    [Fact]
    public void CanSeparateMixedSignalsEvdWhitening()
    {
      int n = 5000;

      // Zwei unabhängige Quellen
      var s1 = Vector<double>.Build.Random(n).Map(Math.Tanh);
      var s2 = Vector<double>.Build.Random(n).Map(Math.Sin);

      var S = Matrix<double>.Build.DenseOfColumnVectors(s1, s2);

      // Zufällige Mixing-Matrix
      var A = Matrix<double>.Build.Random(2, 2);

      // Gemischte Signale
      var X = S * A.Transpose();

      // ICA ausführen
      var result = FactorizationByFastIndependentComponentAnalysis.ICAWithEvdWhitening(X, components: 2);

      // Unabhängige Komponenten
      var S_est = result.S;

      // Test: Komponenten müssen unkorreliert sein
      var cov = (S_est * S_est.Transpose()) / n;

      Assert.True(Math.Abs(cov[0, 1]) < 0.05);
      Assert.True(Math.Abs(cov[1, 0]) < 0.05);
    }

    [Fact]
    public void CanSeparateMixedSignalsSvdWhitening()
    {
      int n = 5000;

      // Zwei unabhängige Quellen
      var s1 = Vector<double>.Build.Random(n).Map(Math.Tanh);
      var s2 = Vector<double>.Build.Random(n).Map(Math.Sin);

      var S = Matrix<double>.Build.DenseOfColumnVectors(s1, s2);

      // Zufällige Mixing-Matrix
      var A = Matrix<double>.Build.Random(2, 2);

      // Gemischte Signale
      var X = S * A.Transpose();

      // ICA ausführen
      var result = FactorizationByFastIndependentComponentAnalysis.ICAWithSvdWhitening(X, components: 2);

      // Unabhängige Komponenten
      var S_est = result.S;

      // Test: Komponenten müssen unkorreliert sein
      var cov = (S_est * S_est.Transpose()) / n;

      Assert.True(Math.Abs(cov[0, 1]) < 0.05);
      Assert.True(Math.Abs(cov[1, 0]) < 0.05);
    }
  }
}
