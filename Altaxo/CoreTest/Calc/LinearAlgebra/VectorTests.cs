using Xunit;

namespace Altaxo.Calc.LinearAlgebra
{
  public class VectorTests
  {
    [Fact]
    public void TestL2Norm()
    {
      var v = Vector<double>.Build.Dense([double.NaN, 0]);
      Assert.True(double.IsNaN(v.L1Norm()));
      Assert.True(double.IsNaN(v.L2Norm()));
    }
  }
}
