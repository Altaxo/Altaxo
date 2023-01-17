using System;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public class NonnegativeMatrixFactorizationTests
  {
    [Fact]
    public void Test1()
    {
      int n = 20;
      int m = 10;

      var spectrum1 = new double[n];
      var spectrum2 = new double[n];
      double arg;

      for (int i = 0; i < n; ++i)
      {
        arg = (i - 7) / 3.0;
        spectrum1[i] = Math.Exp(-0.5 * arg * arg);
        arg = (i - 12) / 4.0;
        spectrum2[i] = Math.Exp(-0.5 * arg * arg);
      }


      var factors = new double[m];

      for (int i = 0; i < m; i++)
      {
        factors[i] = i / 10.0;
      }

      var a = Matrix<double>.Build.Dense(m, n);

      for (int r = 0; r < m; ++r)
      {
        for (int c = 0; c < n; ++c)
        {
          a[r, c] = factors[r] * spectrum1[c] + (1 - factors[r]) * spectrum2[c];
        }
      }

      var nmf = new NonnegativeMatrixFactorization();

      nmf.Evaluate(a, 2, 20);



    }
  }
}
