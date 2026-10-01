using System;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  public class ArrudaBoyceEightChain_Tests
  {
    [Fact]
    public void TestBiaxial()
    {

      var lambda = 3;
      var G = 13;
      var lambdaM = 7;
      var expectedFunctionValue = 42.188954004425868784;
      var expectedDerivativeWrtG = 3.2453041541866052911;
      var expectedDerivativeWrtLambdaM = -1.0506057485986319500;

      // test function value y of one term
      var v = new ArrudaBoyceEightChainBiaxial();
      var y = ArrudaBoyceEightChainBiaxial.Evaluate(lambda - 1, G, lambdaM);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { G, lambdaM };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtG, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtLambdaM, DY[0, 1], 0, 1e-14);
    }

    [Fact]
    public void TestPlanar()
    {

      var lambda = 3;
      var G = 13;
      var lambdaM = 7;
      var expectedFunctionValue = 40.219194270228833142;
      var expectedDerivativeWrtG = 3.0937841746329871648;
      var expectedDerivativeWrtLambdaM = -0.52007264432996038239;

      // test function value y of one term
      var v = new ArrudaBoyceEightChainPlanar();
      var y = ArrudaBoyceEightChainPlanar.Evaluate(lambda - 1, G, lambdaM);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { G, lambdaM };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtG, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtLambdaM, DY[0, 1], 0, 1e-14);
    }

    [Fact]
    public void TestUniaxial()
    {

      var lambda = 3;
      var G = 13;
      var lambdaM = 7;
      var expectedFunctionValue = 39.135945351678343601;
      var expectedDerivativeWrtG = 3.0104573347444879693;
      var expectedDerivativeWrtLambdaM = -0.48178142921774971078;

      // test function value y of one term
      var v = new ArrudaBoyceEightChainUniaxial();
      var y = ArrudaBoyceEightChainUniaxial.Evaluate(lambda - 1, G, lambdaM);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { G, lambdaM };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtG, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtLambdaM, DY[0, 1], 0, 1e-14);
    }

    private const double HugeLambdaM = 1e4; // x = lambdaChain/lambdaM ~ 1e-4  ->  relative deviation ~ x^2 ~ 1e-8

    private static void AssertRelative(double expected, double actual, double relTol, string what)
    {
      double err = Math.Abs(actual - expected) / Math.Abs(expected);
      Assert.True(err <= relTol,
        $"{what}: expected {expected:R}, actual {actual:R}, ratio actual/expected = {actual / expected:R}");
    }

    // ---------- 1. Neo-Hookean limit ----------

    [Theory]
    [InlineData(1.001)]
    [InlineData(1.05)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(3.0)]
    [InlineData(0.7)]   // compression
    public void Uniaxial_ReducesToNeoHookean(double lambda)
    {
      double mu = 0.42;
      double expected = mu * (lambda - 1 / (lambda * lambda));
      double actual = ArrudaBoyceEightChainBase.UniaxialEngineeringStress(lambda, mu, HugeLambdaM);
      AssertRelative(expected, actual, 1e-5, "Uniaxial");
    }

    [Theory]
    [InlineData(1.05)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(0.8)]
    public void Biaxial_ReducesToNeoHookean(double lambda)
    {
      double mu = 0.42;
      double expected = mu * (lambda - Math.Pow(lambda, -5));
      double actual = ArrudaBoyceEightChainBase.BiaxialEngineeringStress(lambda, mu, HugeLambdaM);
      AssertRelative(expected, actual, 1e-5, "Biaxial");
    }

    [Theory]
    [InlineData(1.05)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(0.8)]
    public void PureShear_ReducesToNeoHookean(double lambda)
    {
      double mu = 0.42;
      double expected = mu * (lambda - Math.Pow(lambda, -3));
      double actual = ArrudaBoyceEightChainBase.PlanarEngineeringStress(lambda, mu, HugeLambdaM);
      AssertRelative(expected, actual, 1e-5, "Pure shear");
    }

    // ---------- 2. Small-strain modulus: E = 3*mu ----------

    [Fact]
    public void Uniaxial_SmallStrain_YoungsModulusIsThreeMu()
    {
      double mu = 0.42, lambdaM = 100, eps = 1 / (1024d * 1024d);
      double slope = ArrudaBoyceEightChainBase.UniaxialEngineeringStress(1 + eps, mu, lambdaM) / eps;
      AssertRelative(3 * mu, slope, 1e-4, "Initial Young's modulus");
    }

    // ---------- 3. Analytic gradient vs. central finite differences ----------

    private delegate double StressFunc(double lambda, double mu, double lambdaM);
    private delegate (double, double) GradFunc(double lambda, double mu, double lambdaM);

    public static TheoryData<string, double> GradientCases()
    {
      var data = new TheoryData<string, double>();
      foreach (var mode in new[] { "uniaxial", "biaxial", "pureshear" })
        foreach (var lambda in new[] { 1.2, 1.5, 2.0 })
          data.Add(mode, lambda);
      return data;
    }

    [Theory]
    [MemberData(nameof(GradientCases))]
    public void Gradient_MatchesFiniteDifferences(string mode, double lambda)
    {
      (StressFunc f, GradFunc g) = mode switch
      {
        "uniaxial" => ((StressFunc)ArrudaBoyceEightChainBase.UniaxialEngineeringStress, (GradFunc)ArrudaBoyceEightChainBase.UniaxialEngineeringStressGradient),
        "biaxial" => (ArrudaBoyceEightChainBase.BiaxialEngineeringStress, ArrudaBoyceEightChainBase.BiaxialEngineeringStressGradient),
        "pureshear" => (ArrudaBoyceEightChainBase.PlanarEngineeringStress, ArrudaBoyceEightChainBase.PlanarEngineeringStressGradient),
        _ => throw new ArgumentException(mode)
      };

      double mu = 0.5, lambdaM = 3.5;
      double hMu = 1e-6 * mu, hLm = 1e-6 * lambdaM;

      double numMu = (f(lambda, mu + hMu, lambdaM) - f(lambda, mu - hMu, lambdaM)) / (2 * hMu);
      double numLm = (f(lambda, mu, lambdaM + hLm) - f(lambda, mu, lambdaM - hLm)) / (2 * hLm);

      var (dMu, dLm) = g(lambda, mu, lambdaM);

      AssertRelative(numMu, dMu, 1e-6, $"{mode} dP/dmu");
      AssertRelative(numLm, dLm, 1e-5, $"{mode} dP/dlambdaM");
    }

    // ---------- 4. Input validation ----------

    [Fact]
    public void LockingStretchNotAboveOne_Throws() =>
      Assert.Throws<ArgumentOutOfRangeException>(() => ArrudaBoyceEightChainBase.UniaxialEngineeringStress(1.5, 1.0, 1.0));

    [Fact]
    public void ChainStretchBeyondLocking_Throws() =>
      Assert.Throws<ArgumentOutOfRangeException>(() => ArrudaBoyceEightChainBase.UniaxialEngineeringStress(5.0, 1.0, 1.5));
  }


}
