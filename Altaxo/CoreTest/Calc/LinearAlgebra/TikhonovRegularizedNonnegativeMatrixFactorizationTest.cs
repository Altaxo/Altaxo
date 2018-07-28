using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  [TestFixture]
  public class TikhonovRegularizedNonnegativeMatrixFactorizationTest
  {
    [Test]
    public static void Test01()
    {
      var A = DoubleMatrix.Random(11, 7);

      TikhonovRegularizedNonnegativeMatrixFactorization.TikhonovNMF3(A, 3, null, null, null, null, null, null, 0, 0);
    }
  }
}
