﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.Probability;
using Xunit;

namespace AltaxoTest.Calc.Probability
{
  public delegate ContinuousDistribution ContDistCreator(double a, double b);

  public delegate DiscreteDistribution DiscDistCreator(double a, double b);

  internal class ContDistTester
  {
    /// <summary>
    /// Probability that the Kolmogorov-Smirnov-Test will fail even if the samples are distributed according to
    /// the expected distribution. Lowering the value decreases the probability that the test will fail, but
    /// makes the test more insensitive.
    /// </summary>
    private const double _probabilityForKSTestToFailWrongly = 0.001;

    private const double firstProb = 0.25;
    private const double secondProb = 0.95;

    private double[] _a;
    private ContinuousDistribution _dist;


    /// <summary>
    /// Kolmogorov-Smirnov-Test to test if samples drawn from a distribution are distributed as expected.
    /// </summary>
    /// <param name="sample">The samples drawn from a distribution. You should provide at least 10000 samples.</param>
    /// <param name="referenceCDF">The expected cumulative distribution function.</param>
    /// <param name="alpha">Probability that the Kolmogorov-Smirnov-Test will fail even if the samples are distributed according to
    /// the expected distribution. Lowering the value decreases the probability that the test will fail, but
    /// makes the test more insensitive.</param>
    /// <returns>True if the samples follow the distribution; otherwise, false.</returns>
    public static bool OneSampleKolmogorovSmirnovTest(IEnumerable<double> sample, Func<double, double> referenceCDF, double alpha = 0.001)
    {
      // Calculate the empirical CDF of the sample
      var sortedSample = sample.OrderBy(x => x).ToArray();
      var n = sortedSample.Length;
      var empiricalCDF = sortedSample.Select((x, i) => (i + 1.0) / n).ToArray();

      // Calculate the theoretical CDF using the reference distribution
      var theoreticalCDF = sortedSample.Select(referenceCDF).ToArray();

      // Compute the maximum absolute difference between the CDFs
      var ksStatistic = empiricalCDF.Zip(theoreticalCDF, (ecdf, tcdf) => Math.Abs(ecdf - tcdf)).Max();

      // Calculate the critical value for the given significance level (e.g., alpha = 0.05)
      var criticalValue = Math.Sqrt(-0.5 * Math.Log(alpha / 2.0) / n);

      // Compare the KS statistic to the critical value
      if (ksStatistic > criticalValue)
      {
        // Reject the null hypothesis (sample does not follow the specified distribution)
        // var alphaRequired = 2 * Math.Exp(-2 * criticalValue * criticalValue * n);
        return false;
      }
      else
      {
        // Fail to reject the null hypothesis (sample follows the specified distribution).");
        return true;
      }
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="ContDistTester"/> class.
    /// </summary>
    /// <param name="a">The parameter array. a0 and a1 are parameters used to create the distribution.
    /// a2 is the quantile at p=0.25.
    /// a3 is the PDF at the quantile at p=0.25,
    /// a4 ist the quantile at p=0.95, and
    /// a5 ist the PDF at the quantile at p=0.95.</param>
    /// <param name="creator">The creator.</param>
    public ContDistTester(double[] a, ContDistCreator creator)
    {
      Assert.Equal(6, a.Length); // "Unexpected length of parameter array");
      _a = a;
      _dist = creator(_a[0], _a[1]);
      Assert.NotNull(_dist);
    }

    public virtual void Test(double tolerance)
    {
      // a0 and a1 are the distribution parameters
      // Test quantile
      double x1 = _dist.Quantile(firstProb);
      AssertEx.Equal(_a[2], x1, Math.Abs(_a[2] * tolerance), "Unexpected result of Quantile firstProb");
      double p1 = _dist.CDF(_a[2]);
      AssertEx.Equal(firstProb, p1, Math.Abs(firstProb * tolerance), "Unexpected result of CDF firstQuantile");
      double d1 = _dist.PDF(_a[2]);
      AssertEx.Equal(_a[3], d1, Math.Abs(_a[3] * tolerance), "Unexpected result of PDF firstQuantile");

      double x2 = _dist.Quantile(secondProb);
      AssertEx.Equal(_a[4], x2, Math.Abs(_a[4] * tolerance), "Unexpected result of Quantile secondProb");
      double p2 = _dist.CDF(_a[4]);
      AssertEx.Equal(secondProb, p2, Math.Abs(secondProb * tolerance), "Unexpected result of CDF secondQuantile");
      double d2 = _dist.PDF(_a[4]);
      AssertEx.Equal(_a[5], d2, Math.Abs(_a[5] * tolerance), "Unexpected result of PDF secondQuantile");


      Assert.True(ContDistTester.OneSampleKolmogorovSmirnovTest(Enumerable.Range(0, 10000).Select(_ => _dist.NextDouble()), _dist.CDF, _probabilityForKSTestToFailWrongly));
    }
  }

  /* -- Mathematica lines to create the arrays --------------
 << Statistics`ContinuousDistributions`

 getTwoParaContDist[dist_, a_, b_] :=
 {
	 a,
	 b,
	 N[Quantile[Apply[dist, {a, b}], 1/4], 25],
	 N[PDF[Apply[dist, {a, b}], Quantile[Apply[dist, {a, b}], 1/4]], 25],
	 N[Quantile[Apply[dist, {a, b}], 19/20], 25],
	 N[PDF[Apply[dist, {a, b}], Quantile[Apply[dist, {a, b}], 19/20]], 25]
	 }

 getTwoParaContDist[FRatioDistribution, 3, 4]
 */


  public class TestContinuousProbabilityDistributions
  {
    #region BetaDistribution

    [Fact]
    public void TestBetaDistribution()
    {
      double[][] para = {
new double[]{2.5, 3.5, 0.2739037303169420423172000, 1.749247541974141744140178, 0.7393662913208028693203748, 0.5988730468534031966438193}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new BetaDistribution(a, b);
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion BetaDistribution

    #region CauchyDistribution

    [Fact]
    public void TestCauchyDistribution()
    {
      double[][] para = {
new double[]{5, 8, -3.000000000000000000000000, 0.01989436788648691697111047, 55.51001211740034479183571, 0.0009736996704704924902896828}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new CauchyDistribution
          {
            Alpha = a,
            Gamma = b
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion CauchyDistribution

    #region ChiDistribution

    [Fact]
    public void TestChiDistribution()
    {
      double[][] para = {
new double[]{7, 0, 2.062729304476599455816607, 0.488171949494276922953910, 3.750618675544098650248806, 0.130564628233938801736407}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new ChiDistribution
          {
            N = (int)a
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion ChiDistribution

    #region ChiSquareDistribution

    [Fact]
    public void TestChiSquareDistribution()
    {
      double[][] para = {
new double[]{7, 0, 4.254852183546515743793886, 0.1183315591713442321707384, 14.06714044934016874262697, 0.0174057454954412176379300}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new ChiSquareDistribution
          {
            Alpha = (int)a
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion ChiSquareDistribution

    #region ContinuousUniformDistribution

    [Fact]
    public void TestContinuousUniformDistribution()
    {
      double[][] para = {
new double[]{3, 5, 3.500000000000000000000000, 0.5000000000000000000000000, 4.900000000000000000000000, 0.5000000000000000000000000}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new ContinuousUniformDistribution(a, b);
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion ContinuousUniformDistribution

    #region ErlangDistribution

    [Fact]
    public void TestErlangDistribution()
    {
      double[][] para = {
new double[]{3.0, 0.5, 0.8636497089302596965312208, 0.530369629973745149272610, 3.147896810935994870892347, 0.0730924530353432559740299}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new ErlangDistribution((int)a, b);
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion ErlangDistribution

    #region ExponentialDistribution

    [Fact]
    public void TestExponentialDistribution()
    {
      double[][] para = {
new double[]{3.5, 0, 0.08219487784336597926834829, 2.625000000000000000000000, 0.8559235067297117124100639, 0.1750000000000000000000000}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new ExponentialDistribution
          {
            Lambda = a
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion ExponentialDistribution

    #region FDistribution (FisherSnedecorDistribution)

    [Fact]
    public void TestFDistribution()
    {
      double[][] para = {
new double[]{ 3, 4, 0.4183909951315492730074144, 0.6061355268760950491423940, 6.591382116425581280992118, 0.01221670581017468041546974 }
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new FDistribution
          {
            Alpha = (int)a,
            Beta = (int)b
          };
          return ret;
        }
          );
        tester.Test(1e-14);
      }
    }

    #endregion FDistribution (FisherSnedecorDistribution)

    #region FisherTippettDistribution

    [Fact]
    public void TestFisherTippettDistribution()
    {
      double[][] para = {
new double[]{3.5, 1.8, 2.912058332039094231671373, 0.1925408834888736970603423, 8.846351448275896206409006, 0.0270714609267627815304924}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new FisherTippettDistribution
          {
            Mu = a,
            Alpha = b
          };
          return ret;
        }
          );
        tester.Test(1e-14);
      }
    }

    #endregion FisherTippettDistribution

    #region GammaDistribution

    [Fact]
    public void TestGammaDistribution()
    {
      double[][] para = {
new double[]{1.375, 0.5, 0.2611286278084653847745789, 1.046077103945108021281691, 1.844156400106479932815508, 0.0918157038807440615366658}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new GammaDistribution(a, b);

          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion GammaDistribution

    #region LaplaceDistribution

    [Fact]
    public void TestLaplaceDistribution()
    {
      double[][] para = {
new double[]{1.375, 0.5, 1.028426409720027345291384, 0.500000000000000000000000, 2.526292546497022842008996, 0.1000000000000000000000000}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new LaplaceDistribution
          {
            Mu = a,
            Alpha = b
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion LaplaceDistribution

    #region LognormalDistribution

    [Fact]
    public void TestLognormalDistribution()
    {
      double[][] para = {
new double[]{1.5, 1.75, 1.376636130958281216854024, 0.1319060344841768348662210, 79.7178756354342770648064, 0.00073929029309046336101277}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new LognormalDistribution
          {
            Mu = a,
            Sigma = b
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion LognormalDistribution

    #region NormalDistribution

    [Fact]
    public void TestNormalDistribution()
    {
      double[][] para = {
new double[]{1.5, 1.75, 0.319642937156856949396103, 0.1815866129623468194217694, 4.378493847165077251011736, 0.0589346516430693148362630}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new NormalDistribution
          {
            Mu = a,
            Sigma = b
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion NormalDistribution

    #region ParetoDistribution

    [Fact]
    public void TestParetoDistribution()
    {
      double[][] para = {
new double[]{1.5, 1.75, 1.768010430487225851453548, 0.742359873769694378113828, 8.30877447091612868489998, 0.0105310356306195679153012}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new ParetoDistribution
          {
            Alpha = a,
            Beta = b
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion ParetoDistribution

    #region PowerDistribution

    [Fact]
    public void TestPowerDistribution()
    {
      Assert.Equal(1.7677669529663688110, PowerDistribution.PDF(0.25, 2, 2.5));
      Assert.Equal(0.17677669529663688110, PowerDistribution.CDF(0.25, 2, 2.5));

      double[][] para = {
        new double[]{2, 2.5, 0.28717458874925875170, 2.1763764082403103478, 0.48984586513311492984, 4.8484639129384037291 }
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new PowerDistribution
          {
            K = a,
            A = b
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion PowerDistribution

    #region RayleighDistribution

    [Fact]
    public void TestRayleighDistribution()
    {
      double[][] para = {
new double[]{1.5, 0, 1.137791424661398198866648, 0.379263808220466066288883, 3.671620246021224819564040, 0.081591561022693884879201}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new RayleighDistribution
          {
            Sigma = a
          };

          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion RayleighDistribution

    #region StudentTDistribution

    [Fact]
    public void TestStudentTDistribution()
    {
      double[][] para =
        {
          new double[]
            {
              12,
              0,
              -0.6954828655117925816605449,
              0.3022177430388599749597682,
              1.782287555649320074526381,
              0.08490359467537564413509914 }
      };

      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new StudentTDistribution
          {
            Nu = (int)a
          };
          return ret;
        }
          );
        tester.Test(1e-7);
      }
    }

    [Fact]
    public void TestStudentTDistributionQuantile()
    {
      // N[Quantile[StudentTDistribution[12], 1/10], 25]
      AssertEx.Equal(-1.356217334023205433796216, StudentTDistribution.Quantile(0.1, 12), 1e-14);

      // N[Quantile[StudentTDistribution[12], 5/10], 25]
      Assert.Equal(0, StudentTDistribution.Quantile(0.5, 12));

      // N[Quantile[StudentTDistribution[12], 9/10], 25]
      AssertEx.Equal(1.356217334023205433796216, StudentTDistribution.Quantile(0.9, 12), 1e-14);
    }

    [Fact]
    public void TestStudentTDistributionCDF()
    {
      // N[CDF[StudentTDistribution[12], 1/10], 25
      AssertEx.Equal(0.5390022147715870702517874, StudentTDistribution.CDF(0.1, 12), 1e-14);

      // N[CDF[StudentTDistribution[12], 5/10], 25
      AssertEx.Equal(0.6869412618873379592985154, StudentTDistribution.CDF(0.5, 12), 1e-14);

      // N[CDF[StudentTDistribution[12], 9/10], 25
      AssertEx.Equal(0.8070872841025491107351673, StudentTDistribution.CDF(0.9, 12), 1e-14);

      // N[CDF[StudentTDistribution[12], -9/10], 25
      AssertEx.Equal(0.1929127158974508892648327, StudentTDistribution.CDF(-0.9, 12), 1e-14);
    }

    [Fact]
    public void TestStudentTDistributionPDF()
    {
      // N[PDF[StudentTDistribution[12], 1/10], 25]
      AssertEx.Equal(0.3886164693412969119684949, StudentTDistribution.PDF(0.1, 12), 1e-14);

      // N[PDF[StudentTDistribution[12], 5/10], 25]
      AssertEx.Equal(0.3417166761526545973157284, StudentTDistribution.PDF(0.5, 12), 1e-14);

      // N[PDF[StudentTDistribution[12], 9/10], 25]
      AssertEx.Equal(0.2555532488891510539167125, StudentTDistribution.PDF(0.9, 12), 1e-14);

      // N[PDF[StudentTDistribution[12], -9/10], 25]
      AssertEx.Equal(0.2555532488891510539167125, StudentTDistribution.PDF(-0.9, 12), 1e-14);
    }

    #endregion StudentTDistribution

    #region TriangularDistribution

    [Fact]
    public void TestTriangularDistribution()
    {
      double[][] para = {
new double[]{-66, 77, -15.441865145161852005 , 0.0098896053312803849567, 54.389714729796087776, 0.0044227659582774536112 }
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new TriangularDistribution(a, b, 0.5 * (a + b));
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion TriangularDistribution

    #region WeibullDistribution

    [Fact]
    public void TestWeibullDistribution()
    {
      double[][] para = {
new double[]{1.5, 1.75, 0.762628880480291575561159, 0.424377229596168368465272, 3.63669361568547473062272, 0.061781371833876567930871}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new ContDistTester(para[i], delegate (double a, double b)
        {
          var ret = new WeibullDistribution
          {
            Alpha = a,
            Lambda = b
          };

          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion WeibullDistribution
  }

  internal class DiscDistTester
  {
    private double[] _a;
    private DiscreteDistribution _dist;

    /// <summary>
    ///
    /// </summary>
    /// <param name="a">Array of test parameters. a[0]: first distribution parameter, a[1] second distribution parameter
    /// a[2] argument for CDF and PDF, a[3] result of CDF, a[4] result of PDF</param>
    /// <param name="creator"></param>
    public DiscDistTester(double[] a, DiscDistCreator creator)
    {
      Assert.Equal(5, a.Length); // "Unexpected length of parameter array");
      _a = a;
      _dist = creator(_a[0], _a[1]);
      Assert.NotNull(_dist);
    }

    public virtual void Test(double tolerance)
    {
      double p1 = _dist.CDF(_a[2]);
      AssertEx.Equal(_a[3], p1, Math.Abs(_a[3] * tolerance), "Unexpected result of CDF");

      double d1 = _dist.PDF(_a[2]);
      AssertEx.Equal(_a[4], d1, Math.Abs(_a[4] * tolerance), "Unexpected result of PDF");

      int len = 1000000;
      int suml = 0;
      for (int i = 0; i < len; i++)
      {
        if (_dist.NextDouble() <= _a[2])
          suml++;
      }

      // wir nähern die Binomialverteilung mit der Normalverteilung an:
      var sigma = Math.Sqrt(len * _a[3] * (1 - _a[3]));
      var delta = Math.Abs(suml - len * _a[3]);
      AssertEx.Less(delta, 5 * sigma, $"Unexpected result of distribution of generated values : outside the five sigma confidence limits, expected d < 5*sigma d={delta}, 5*sigma={5 * sigma}");
    }
  }

  /* -- Mathematica lines to create the arrays --------------
<< Statistics`DiscreteDistributions`

getTwoParaDiscreteDist[dist_, a_, b_, c_] :=
{
	N[a],
	N[b],
	N[c],
	N[CDF[Apply[dist, {a, b}], c], 25],
	N[PDF[Apply[dist, {a, b}], c], 25]
	}

getTwoParaDiscreteDist[BinomialDistribution, 10, 3/4, 7]
*/


  public class TestDiscreteProbabilityDistributions
  {
    #region BernoulliDistribution

    [Fact]
    public void TestBernoulliDistribution()
    {
      double[][] para = {
new double[]{0.75, 0, 0, 0.2500000000000000000000000, 0.2500000000000000000000000}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new DiscDistTester(para[i], delegate (double a, double b)
        {
          var ret = new BernoulliDistribution
          {
            Probability = a
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion BernoulliDistribution

    #region BinomialDistribution

    [Fact]
    public void TestBinomialDistribution()
    {
      double[][] para = {
new double[]{10, 0.75, 7, 0.4744071960449218750000000, 0.2502822875976562500000000},
new double[]{100, 0.375, 40, 0.7339137587545079200156959, 0.07115992585884902329546213}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new DiscDistTester(para[i], delegate (double a, double b)
        {
          var ret = new BinomialDistribution((int)a, b);
          return ret;
        }
          );
        tester.Test(1E-8);
      }
    }

    #endregion BinomialDistribution

    #region DiscreteUniformDistribution

    [Fact]
    public void TestDiscreteUniformDistribution()
    {
      double[][] para = {
new double[]{1, 11, 5, 0.4545454545454545454545455, 0.09090909090909090909090909}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new DiscDistTester(para[i], delegate (double a, double b)
        {
          var ret = new DiscreteUniformDistribution((int)a, (int)b);
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion DiscreteUniformDistribution

    #region GeometricDistribution

    [Fact]
    public void TestGeometricDistribution()
    {
      double[][] para = {
      // note since Mathematica has variant B, you must increase the third value here by one
new double[]{0.125, 0, 5, 0.4870910644531250000000000, 0.07327270507812500000000000}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new DiscDistTester(para[i], delegate (double a, double b)
        {
          var ret = new GeometricDistribution
          {
            Probability = a
          };
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion GeometricDistribution

    #region PoissonDistribution

    [Fact]
    public void TestPoissonDistribution()
    {
      double[][] para = {
new double[]{11, 0, 9, 0.3405106424656610472811084, 0.1085255092982049925567724}
      };
      for (int i = 0; i < para.Length; i++)
      {
        var tester = new DiscDistTester(para[i], delegate (double a, double b)
        {
          var ret = new PoissonDistribution(a);
          return ret;
        }
          );
        tester.Test(1E-14);
      }
    }

    #endregion PoissonDistribution
  }
}
