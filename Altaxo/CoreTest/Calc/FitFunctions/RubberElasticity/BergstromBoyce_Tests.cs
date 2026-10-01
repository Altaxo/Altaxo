using System;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Xunit;
using BBParams = Altaxo.Calc.FitFunctions.RubberElasticity.BergstromBoyce.BergstromBoyceParameters;

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  public class BergstromBoyce_Tests
  {

    private sealed record TestModel : BergstromBoyce
    {
      public TestModel(LoadingMode mode) : base(mode, BergstromBoyceParametrization.PolyUMod)
      {
      }
    }

    // lambdaL sets BOTH LambdaLA and LambdaLB (shared locking stretch), matching the
    // PolyUMod/Abaqus convention used throughout these tests.
    private static BBParams DefaultParams(double lambdaL = 3.58, double s = 1.0) => new BBParams
    {
      Mu = 1E6,
      LambdaLA = lambdaL,
      LambdaLB = lambdaL,
      S = s,
      Xi = 0.05,
      C = -1,
      TauBase = 2E5,
      M = 3,
      TauCut = 0
    };

    private static double L0A(BBParams p) => Hyperbolic.InverseLangevin(1.0 / p.LambdaLA);
    private static double L0B(BBParams p) => Hyperbolic.InverseLangevin(1.0 / p.LambdaLB);

    private static void AssertRelative(double expected, double actual, double relTol, string what)
    {
      double err = Math.Abs(actual - expected) / Math.Abs(expected);
      Assert.True(err <= relTol,
        $"{what}: expected {expected:R}, actual {actual:R}, ratio = {actual / expected:R}");
    }

    /// Arruda-Boyce reference, converted to Cauchy stress (sigma = lambda * P).
    private static double ArrudaBoyceCauchy(LoadingMode mode, double lambda, double muAB, double lambdaM)
    {
      double p = mode switch
      {
        LoadingMode.Uniaxial => ArrudaBoyceEightChainBase.UniaxialEngineeringStress(lambda, muAB, lambdaM),
        LoadingMode.EquiBiaxial => ArrudaBoyceEightChainBase.BiaxialEngineeringStress(lambda, muAB, lambdaM),
        LoadingMode.Planar => ArrudaBoyceEightChainBase.PlanarEngineeringStress(lambda, muAB, lambdaM),
        _ => throw new NotSupportedException()
      };
      return lambda * p; // convert from engineering to Cauchy
    }

    // Equivalent Arruda-Boyce mu based on network A's Mu/LambdaLA. Since all tests in this
    // file use DefaultParams (LambdaLA == LambdaLB), this is numerically identical whether
    // derived from network A or B; it is defined via network A for a single, unambiguous reference.
    private static double MuAB(BBParams p) => 3 * p.Mu / (p.LambdaLA * L0A(p));

    private static double SinglePoint(LoadingMode mode, double lambda, BBParams p) =>
      new TestModel(mode).EvaluateCauchyStress(new[] { (0.0, lambda) }, p).First().cauchyStress;

    public static TheoryData<LoadingMode, double, double, double> InstantCases()
    {
      var d = new TheoryData<LoadingMode, double, double, double>();
      foreach (var mode in new[] { LoadingMode.Uniaxial, LoadingMode.EquiBiaxial, LoadingMode.Planar })
        foreach (var lambda in new[] { 0.7, 1.1, 1.5, 2.0 })
          foreach (var lambdaL in new[] { 2.5, 3.58, 8.0 })
            foreach (var s in new[] { 0.0, 1.0, 2.5 })
              d.Add(mode, lambda, lambdaL, s);
      return d;
    }

    // ---------- 1. Instantaneous response == (1+S) * Arruda-Boyce ----------
    [Theory]
    [MemberData(nameof(InstantCases))]
    public void InstantaneousResponse_MatchesArrudaBoyce(LoadingMode mode, double lambda, double lambdaL, double s)
    {
      var p = DefaultParams(lambdaL, s);
      double expected = ArrudaBoyceCauchy(mode, lambda, (1 + s) * MuAB(p), lambdaL);
      AssertRelative(expected, SinglePoint(mode, lambda, p), 1e-10, $"{mode}, lambda={lambda}, lambdaL={lambdaL}, S={s}");
    }

    // ---------- 2. Neo-Hookean limit of network A ----------
    [Theory]
    [InlineData(LoadingMode.Uniaxial, 1.5)]
    [InlineData(LoadingMode.Uniaxial, 0.8)]
    [InlineData(LoadingMode.EquiBiaxial, 1.5)]
    [InlineData(LoadingMode.Planar, 1.5)]
    public void HugeLambdaL_IsNeoHookean(LoadingMode mode, double lambda)
    {
      var p = DefaultParams(lambdaL: 1E4, s: 0.0);
      double l3sq = mode switch
      {
        LoadingMode.Uniaxial => 1 / lambda,
        LoadingMode.EquiBiaxial => 1 / Math.Pow(lambda, 4),
        _ => 1 / (lambda * lambda)
      };
      double expected = p.Mu * (lambda * lambda - l3sq);
      AssertRelative(expected, SinglePoint(mode, lambda, p), 1e-6, "Neo-Hookean");
    }

    // ---------- 3. Fast loading: no flow -> (1+S) * sigma_A ----------
    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void VeryFastLoading_HasNoViscousFlow(LoadingMode mode)
    {
      var p = DefaultParams();
      double lambdaEnd = 1.6;
      int n = 50;
      var t = new (double t, double lambda)[n];
      for (int i = 0; i < n; i++)
      {
        t[i] = (1e-12 * i / (n - 1), 1 + (lambdaEnd - 1) * i / (n - 1));
      }
      var sigma = new TestModel(mode).EvaluateCauchyStress(t, p).Select(x => x.cauchyStress).ToArray();
      double expected = ArrudaBoyceCauchy(mode, lambdaEnd, (1 + p.S) * MuAB(p), p.LambdaLA);
      AssertRelative(expected, sigma[n - 1], 1e-6, "Fast loading");
    }

    // ---------- 4. Relaxation: (1+S)*sigma_A  ->  sigma_A ----------
    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void NetworkAStress_ShouldBeEqualTo_ArrudaBoyceStress(LoadingMode mode)
    {
      var p = DefaultParams();

      double lam = 1.5;


      double sigmaArrudaBoyceCauchy = ArrudaBoyceCauchy(mode, lam, MuAB(p), p.LambdaLA);
      double sigmaBerstromBoyceCauchy = new TestModel(mode).NetworkAStress(lam, p, L0A(p));

      AssertRelative(sigmaArrudaBoyceCauchy, sigmaBerstromBoyceCauchy, 1e-10, "Network A stress");
    }

    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void NetworkBElasticStress_ShouldBeEqualTo_ArrudaBoyceStress(LoadingMode mode)
    {
      var p = DefaultParams();

      double lam = 1.5;


      double sigmaArrudaBoyceCauchy = ArrudaBoyceCauchy(mode, lam, MuAB(p), p.LambdaLB);
      double sigmaBerstromBoyceCauchy = new TestModel(mode).NetworkBStress(lam, p, L0B(p));

      AssertRelative(sigmaArrudaBoyceCauchy, sigmaBerstromBoyceCauchy, 1e-10, "Network B elastic stress");
    }

    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void LambdaBv_ShouldRelaxToLambda(LoadingMode mode)
    {
      var p = DefaultParams();
      p.M = 1;  // linear in tau -> exponential decay, no slow power-law tail
      double lam = 1.5;
      double sigmaArrudaBoyceCauchy = ArrudaBoyceCauchy(mode, lam, MuAB(p), p.LambdaLA);
      double l0A = L0A(p);
      double l0B = L0B(p);

      var bb = new TestModel(mode);

      double lambdaBv = 1;
      double sigma;
      sigma = bb.RungeKuttaStep(1, lam, 1e-9, ref lambdaBv, p, l0A, l0B);


      for (int i = 0; i < 500; ++i)
      {
        sigma = bb.RungeKuttaStep(lam, lam, 1e-2, ref lambdaBv, p, l0A, l0B);
      }

      sigma = bb.RungeKuttaStep(lam, lam, 1e-2, ref lambdaBv, p, l0A, l0B);

      AssertEx.AreEqual(lam, lambdaBv, 0, 1e-6);
      AssertEx.AreEqual(sigmaArrudaBoyceCauchy, sigma, 0, 1e-6);
    }


    // ---------- 4. Relaxation: (1+S)*sigma_A  ->  sigma_A ----------
    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void StressRelaxation_DecaysToNetworkAOnly(LoadingMode mode)
    {
      var p = DefaultParams();
      p.M = 1;  // linear in tau -> exponential decay, no slow power-law tail
      double lambdaHold = 1.5;

      int holdSteps = 500;
      double dt = 0.01;                     // keep dt*rate < ~2.8 for RK4 stability
      var t = new (double t, double lambda)[holdSteps + 2];
      t[0] = (0, 1.0);
      t[1] = (1e-9, lambdaHold);     // (almost) instantaneous step
      for (int i = 2; i < 9; i++)
      {
        t[i] = (t[i - 1].t + dt * RMath.Pow(10, i - 9), lambdaHold);
      }
      for (int i = 9; i < t.Length; i++)
      {
        t[i] = (t[i - 1].t + dt, lambdaHold);
      }

      var sigma = new TestModel(mode).EvaluateCauchyStress(t, p).Select(x => x.cauchyStress).ToArray();

      double sigmaA = ArrudaBoyceCauchy(mode, lambdaHold, MuAB(p), p.LambdaLA);
      AssertRelative((1 + p.S) * sigmaA, sigma[1], 1e-4, "stress right after step");
      AssertRelative(sigmaA, sigma[^1], 1e-4, "equilibrium stress");

      for (int i = 2; i < sigma.Length; i++)
      {
        Assert.True(sigma[i] <= sigma[i - 1] * (1 + 1e-9), $"not monotonic at i={i}");
        Assert.True(sigma[i] >= sigmaA * (1 - 1e-6), $"below equilibrium at i={i}");
      }
    }

    // ---------- 5. Rate dependence ----------
    [Fact]
    public void FasterLoading_GivesHigherStress()
    {
      var p = DefaultParams();
      double Run(double duration)
      {
        int n = 400;
        var t = new (double t, double lambda)[n];
        for (int i = 0; i < n; i++)
        {
          t[i] = (duration * i / (n - 1), 1 + 1.0 * i / (n - 1));
        }
        return new TestModel(LoadingMode.Uniaxial).EvaluateCauchyStress(t, p).Select(x => x.cauchyStress).ToArray()[n - 1];
      }
      double slow = Run(20.0), fast = Run(0.2);
      double sigmaA = ArrudaBoyceCauchy(LoadingMode.Uniaxial, 2.0, MuAB(p), p.LambdaLA);
      Assert.True(fast > slow, $"fast={fast}, slow={slow}");
      Assert.True(slow > sigmaA, "slow loading must stay above the equilibrium stress");
      Assert.True(fast <= (1 + p.S) * sigmaA * (1 + 1e-6), "fast loading must not exceed the instantaneous stress");
    }

    // ---------- 6. Engineering stress, cross-section area ----------
    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void FitFunctionEvaluate_ReturnsAreaTimesEngineeringStress(LoadingMode mode)
    {
      var p = DefaultParams();
      var model = new TestModel(mode) { CrossSectionArea = 2.5 };

      var tlambda = Enumerable.Range(0, 1000).Select(i => (t: (double)i, lambda: 1 + i * 0.6 / 1000d)).ToArray();
      var tepsilon = Enumerable.Range(0, 1000).Select(i => (t: (double)i, epsilon: 0 + i * 0.6 / 1000d)).ToArray();

      // Cauchy history from the public overload
      var cauchy = model.EvaluateCauchyStress(tlambda, p).Select(x => x.cauchyStress).ToArray();

      // Adapt the matrix/vector construction to whatever you normally use in Altaxo tests.
      var x = Matrix<double>.Build.Dense(tepsilon.Length, 2);
      for (int i = 0; i < tepsilon.Length; i++)
      {
        x[i, 0] = tepsilon[i].t;
        x[i, 1] = tepsilon[i].epsilon;
      }
      var y = Vector<double>.Build.Dense(tepsilon.Length);
      // Raw fit parameters in the PolyUMod ordering (LambdaLA == LambdaLB in p, so either field works here).
      model.Evaluate(x, new[] { p.Mu, p.LambdaLA, p.S, p.Xi, p.C, p.TauBase, p.M, p.TauCut }, y, null);

      for (int i = 1; i < tepsilon.Length; i++)
        AssertRelative(2.5 * cauchy[i] / tlambda[i].lambda, y[i], 1e-12, $"point {i}");
      // (point 0 is where a missing area factor or missing /lambda would show up)
    }

    [Theory]
    [InlineData(LoadingMode.Uniaxial)]
    [InlineData(LoadingMode.EquiBiaxial)]
    [InlineData(LoadingMode.Planar)]
    public void FitFunctionEvaluate_RungeKuttaAdaptiveStepsWork(LoadingMode mode)
    {
      var p = DefaultParams();
      var model = new TestModel(mode) { CrossSectionArea = 2.5 };

      var tl1 = Enumerable.Range(0, 301).Select(i => (t: i / 100d, lambda: 1 + i / 500d)).ToArray();
      var cauchy1 = model.EvaluateCauchyStress(tl1, p).ToArray();

      var tl2 = Enumerable.Range(0, 4).Select(i => (t: (double)i, lambda: 1 + i / 5d)).ToArray();
      var cauchy2 = model.EvaluateCauchyStress(tl2, p).ToArray();

      AssertEx.AreEqual(cauchy1.Last().cauchyStress, cauchy2.Last().cauchyStress, 0, 1e-6);
    }

    // ---------- 7. Input validation ----------


    [Fact]
    public void SinglePointEvaluate_Throws()
    {
      var m = new TestModel(LoadingMode.Uniaxial);
      Assert.Throws<InvalidOperationException>(() =>
        m.Evaluate(new double[] { 0, 0 }, new double[8], new double[1]));
    }
  }
}
