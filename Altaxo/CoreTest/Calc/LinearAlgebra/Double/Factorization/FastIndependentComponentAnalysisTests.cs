using System;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  public class FastIndependentComponentAnalysisTests
  {
    [Fact]
    public void CanSeparateMixedSignals()
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
      var result = FastIndependentComponentAnalysis.ICAWithSvdWhitening(X, components: 2);

      // Unabhängige Komponenten
      var S_est = result.S;

      // Test: Komponenten müssen unkorreliert sein
      var cov = (S_est * S_est.Transpose()) / n;

      Assert.True(Math.Abs(cov[0, 1]) < 0.05);
      Assert.True(Math.Abs(cov[1, 0]) < 0.05);
    }
  }


}
