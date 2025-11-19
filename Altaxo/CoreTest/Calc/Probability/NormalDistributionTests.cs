using Xunit;

namespace Altaxo.Calc.Probability
{
  public class NormalDistributionTests
  {
    [Fact]
    public void Test_CDF()
    {
      double mean, sigma;

      mean = 0; sigma = 1;
      var nd = new NormalDistribution(mean, sigma);
      AssertEx.AreEqual(2.7536241186062336951e-89, nd.CDF(-20), 0, 1e-10);
      AssertEx.AreEqual(2.7536241186062336951e-89, NormalDistribution.CDF(-20, mean, sigma), 0, 1e-10);
      AssertEx.AreEqual(7.619853024160526066e-24, nd.CDF(-10), 0, 1e-10);
      AssertEx.AreEqual(2.8665157187919391167e-7, nd.CDF(-5), 0, 1e-10);
      AssertEx.AreEqual(0.15865525393145705141, nd.CDF(-1), 0, 1e-10);
      AssertEx.AreEqual(0.5, nd.CDF(0), 0, 1e-10);
      AssertEx.AreEqual(0.84134474606854294859, nd.CDF(1), 0, 1e-10);
      AssertEx.AreEqual(0.9772498680518207928, nd.CDF(2), 0, 1e-10);
      AssertEx.AreEqual(0.99865010196836990547, nd.CDF(3), 0, 1e-10);
      AssertEx.AreEqual(1, nd.CDF(10), 0, 1e-14);

      mean = 3; sigma = 5;
      nd = new NormalDistribution(3, 5);
      AssertEx.AreEqual(0.21185539858339668558, nd.CDF(-1), 0, 1e-10);
      AssertEx.AreEqual(0.21185539858339668558, NormalDistribution.CDF(-1, mean, sigma), 0, 1e-10);
    }
  }
}
