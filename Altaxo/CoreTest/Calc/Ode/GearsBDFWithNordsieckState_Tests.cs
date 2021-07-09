using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.Ode
{
  public class GearsBDFWithNordsieckState_Tests
  {
    private static void GetJacobianForExpDecay(double x, double[] y, ref IMatrix<double> m)
    {
      if (m is null)
      {
        m = new DoubleMatrix(1, 1);
      }
      m[0, 0] = -1;
    }

    [Fact]
    public void Test_ASSC_AutomaticStepSize_ExponentialDecay_FunctionalIteration()
    {
      double targetAccuracy = 1E-4;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; }, GetJacobianForExpDecay);
      var points = ode.GetSolutionPointsVolatile(
        new MultiStepMethodOptions
        {
          InitialStepSize = 1 / 64d,
          RelativeTolerance = targetAccuracy,
          AutomaticStepSizeControl = true,
          IterationMethod = OdeIterationMethod.DoNotUseJacobian
        });

      int cnt = 0;
      foreach (var p in points)
      {
        if (p.X > 4)
          break;

        ++cnt;

        var x = p.X;
        var expected = Math.Exp(-x);

        var absError = Math.Abs(p.Y_volatile[0] - expected);
        var relError = absError / Math.Abs(expected);

        AssertEx.Equal(expected, p.Y_volatile[0], 0, targetAccuracy * cnt, string.Empty);
      }
    }

    [Fact]
    public void Test_ASSC_AutomaticStepSize_ExponentialDecay_JacobianIteration()
    {
      double targetAccuracy = 1E-4;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; }, GetJacobianForExpDecay);
      var points = ode.GetSolutionPointsVolatile(
        new OdeMethodOptions
        {
          InitialStepSize = 1 / 64d,
          RelativeTolerance = targetAccuracy,
          AutomaticStepSizeControl = true
        });

      int cnt = 0;
      foreach (var p in points)
      {
        if (p.X > 4)
          break;

        ++cnt;

        var x = p.X;
        var expected = Math.Exp(-x);

        var absError = Math.Abs(p.Y_volatile[0] - expected);
        var relError = absError / Math.Abs(expected);

        AssertEx.Equal(expected, p.Y_volatile[0], 0, targetAccuracy * cnt, string.Empty);
      }
    }


    private static void GetJacobianForStiffExponentialDecays(double x, double[] y, ref IMatrix<double> m)
    {
      if (m is null)
      {
        m = new DoubleMatrix(2, 2);
      }
      m[0, 0] = 998;
      m[0, 1] = 1998;
      m[1, 0] = -999;
      m[1, 1] = -1999;
    }

    [Fact]
    public void Test_ASSC_AutomaticStepSize_StiffExponentialDecays()
    {
      double targetAccuracy = 1E-4;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 1, 0 }, (x, y, d) => { d[0] = 998 * y[0] + 1998 * y[1]; d[1] = -999 * y[0] - 1999 * y[1]; }, GetJacobianForStiffExponentialDecays);
      var points = ode.GetSolutionPointsVolatile(
        new OdeMethodOptions
        {
          InitialStepSize = 1 / 64d,
          RelativeTolerance = targetAccuracy,
          ErrorNorm = ErrorNorm.InfinityNorm,
          AutomaticStepSizeControl = true
        });

      int cnt = 0;
      foreach (var p in points)
      {
        if (p.X > 4)
          break;

        ++cnt;
        var x = p.X;
        var expectedy0 = 2 * Math.Exp(-x) - Math.Exp(-1000 * x);
        var expectedy1 = -Math.Exp(-x) + Math.Exp(-1000 * x);
        var absError0 = Math.Abs(p.Y_volatile[0] - expectedy0);
        var absError1 = Math.Abs(p.Y_volatile[1] - expectedy1);
        var relError0 = absError0 / Math.Abs(expectedy0);
        var relError1 = absError1 / Math.Abs(expectedy1);

        AssertEx.Equal(expectedy0, p.Y_volatile[0], 0, targetAccuracy * cnt, string.Empty);
        AssertEx.Equal(expectedy1, p.Y_volatile[1], 0, targetAccuracy * cnt, string.Empty);
      }
    }

    [Fact]
    public void Test_Order2_ExponentialDecay()
    {
      double targetAccuracy = 1E-4;

      for (int order = 1; order <= 5; ++order)
      {
        var ode = new GearsBDFWithNordsieckState();
        var derivatives = new double[order + 1][];
        for (int i = 0; i < derivatives.Length; ++i)
          derivatives[i] = new double[] { i % 2 == 0 ? 1 : -1 };

        ode.Initialize(0, derivatives, (x, y, d) => { d[0] = -y[0]; }, GetJacobianForExpDecay);
        var points = ode.GetSolutionPointsVolatile(new MultiStepMethodOptions { InitialStepSize = 1 / 64d, RelativeTolerance = targetAccuracy, AutomaticStepSizeControl = false, MinOrder = order, MaxOrder = order });

        double maxAbsError = 0;
        double maxRelError = 0;
        int cnt = 0;
        foreach (var p in points)
        {
          if (p.X > 4)
            break;

          ++cnt;

          var x = p.X;
          var expected = Math.Exp(-x);

          var absError = Math.Abs(p.Y_volatile[0] - expected);
          var relError = absError / Math.Abs(expected);

          maxAbsError = Math.Max(absError, maxAbsError);
          maxRelError = Math.Max(relError, maxRelError);
        }
        System.Diagnostics.Debug.WriteLine($"Order: {order}, maxAbsErr={maxAbsError}, maxRelErr={maxRelError}");
      }
    }


    [Fact]
    public void Test_ASSC_MandatoryPoints_ExponentialDecay()
    {
      var relativeToleranceGoal = 1e-6;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; }, GetJacobianForExpDecay);

      var mandatoryPoints = OdeMethodOptions.GetEquidistantSequence(0.1, 0.1, 10);

      var points = ode.GetSolutionPointsVolatile(
        new OdeMethodOptions
        {
          InitialStepSize = 2,
          RelativeTolerance = relativeToleranceGoal,
          AutomaticStepSizeControl = true,
          MandatorySolutionPoints = mandatoryPoints,
          IncludeAutomaticStepsInOutput = false
        }
        );

      var maxAbsErr = 0d;
      var maxAbsErrX = -1d;
      var maxRelErr = 0d;
      var maxRelErrX = -1d;

      var listOfX = new List<double>();

      foreach (var sp in points.TakeWhile(sp => sp.X <= 4))
      {
        listOfX.Add(sp.X);
        var yexpected = Math.Exp(-sp.X);
        var y = sp.Y_volatile[0];

        var errAbs = Math.Abs(y - yexpected);
        var errRel = Math.Abs((y - yexpected) / yexpected);


        if (errAbs > maxAbsErr)
        {
          maxAbsErr = errAbs;
          maxAbsErrX = sp.X;
        }
        if (errRel > maxRelErr)
        {
          maxRelErr = errRel;
          maxRelErrX = sp.X;
        }
      }

      Assert.Equal(10, listOfX.Count);
      for (int i = 0; i < 10; ++i)
        Assert.Equal(0.1 + i * 0.1, listOfX[i]);

      AssertEx.Less(maxRelErr, ode.NumberOfStepsTaken * relativeToleranceGoal);
    }

    [Fact]
    public void Test_ASSC_MandatoryAndOptionalPoints_ExponentialDecay()
    {
      var relativeToleranceGoal = 1e-6;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; }, GetJacobianForExpDecay);

      var mandatoryPoints = OdeMethodOptions.GetEquidistantSequence(1 / 8d, 1 / 8d, 10);
      var optionalPoints = OdeMethodOptions.GetEquidistantSequence(1 / 64d, 1 / 64d, 80);

      var points = ode.GetSolutionPointsVolatile(new OdeMethodOptions { InitialStepSize = 2, RelativeTolerance = relativeToleranceGoal, AutomaticStepSizeControl = true, MandatorySolutionPoints = mandatoryPoints, OptionalSolutionPoints = optionalPoints, IncludeAutomaticStepsInOutput = false });

      var maxAbsErr = 0d;
      var maxAbsErrX = -1d;
      var maxRelErr = 0d;
      var maxRelErrX = -1d;

      var listOfX = new List<double>();

      foreach (var sp in points.TakeWhile(sp => sp.X <= 4))
      {
        listOfX.Add(sp.X);
        var yexpected = Math.Exp(-sp.X);
        var y = sp.Y_volatile[0];

        var errAbs = Math.Abs(y - yexpected);
        var errRel = Math.Abs((y - yexpected) / yexpected);


        if (errAbs > maxAbsErr)
        {
          maxAbsErr = errAbs;
          maxAbsErrX = sp.X;
        }
        if (errRel > maxRelErr)
        {
          maxRelErr = errRel;
          maxRelErrX = sp.X;
        }
      }

      Assert.Equal(80, listOfX.Count);
      for (int i = 0; i < 80; ++i)
        AssertEx.Equal((i + 1) / 64d, listOfX[i], 1E-12);

      AssertEx.Less(maxRelErr, relativeToleranceGoal * ode.NumberOfStepsTaken);
    }


    [Fact]
    public void Test_ASSC_Diffusion()
    {
      void CalcRates(double x, double[] y, double[] rates)
      {
        for (int i = 0; i < y.Length; ++i)
        {
          double yim1 = (i - 1) >= 0 ? y[i - 1] : 0;
          double yip1 = (i + 1) < y.Length ? y[i + 1] : 0;

          double d1 = yim1 - y[i];
          double d2 = y[i] - yip1;

          rates[i] = d1 - d2;
        }
      }

      void CalcJac(double x, double[] y, ref IMatrix<double> jac)
      {
        var n = y.Length;
        if (jac is null)
        {
          jac = new BandDoubleMatrix(y.Length, y.Length, 1, 1);
        }
        for (int i = 0; i < n; ++i)
          jac[i, i] = -2;
        for (int i = 0; i < n - 1; ++i)
          jac[i, i + 1] = 1;
        for (int i = 0; i < n - 1; ++i)
          jac[i + 1, i] = 1;
      }


      var ode = new GearsBDFWithNordsieckState();
      var yinitial = new double[100];
      yinitial.FillWith(1);
      ode.Initialize(0, yinitial, CalcRates, CalcJac);


      var points = ode.GetSolutionPointsVolatile(new OdeMethodOptions { InitialStepSize = 1 / 64d, RelativeTolerance = 1E-4, AbsoluteTolerance = 1E-4, AutomaticStepSizeControl = true });

      double maxStepSize = 0;
      double previousStepSize = 0;
      double previousX = 0;

      foreach (var sp in points.TakeWhile(sp => sp.X < 1000))
      {
        var currentStepSize = sp.X - previousX;
        previousX = sp.X;
        previousStepSize = currentStepSize;

        maxStepSize = Math.Max(maxStepSize, currentStepSize);

        for (int i = 0; i < sp.Y_volatile.Length / 2; ++i) // solution has to be symmetric
          Assert.Equal(sp.Y_volatile[i], sp.Y_volatile[sp.Y_volatile.Length - 1 - i], 13);
      }

      AssertEx.Greater(maxStepSize, 1);
    }

    /// <summary>
    /// Tests a diffusion equation with changing outer condition.
    /// </summary>
    [Fact]
    public void Test_ASSC_DiffusionOutAndIn()
    {
      void CalcRates(double x, double[] y, double[] rates)
      {
        double outerConcentration = x > 1000 ? 1 : 0;

        for (int i = 0; i < y.Length; ++i)
        {
          double yim1 = (i - 1) >= 0 ? y[i - 1] : outerConcentration;
          double yip1 = (i + 1) < y.Length ? y[i + 1] : outerConcentration;

          double d1 = yim1 - y[i];
          double d2 = y[i] - yip1;

          rates[i] = d1 - d2;
        }
      }

      void CalcJac(double x, double[] y, ref IMatrix<double> jac)
      {
        var n = y.Length;
        if (jac is null)
        {
          jac = new BandDoubleMatrix(y.Length, y.Length, 1, 1);
        }
        for (int i = 0; i < n; ++i)
          jac[i, i] = -2;
        for (int i = 0; i < n - 1; ++i)
          jac[i, i + 1] = 1;
        for (int i = 0; i < n - 1; ++i)
          jac[i + 1, i] = 1;
      }


      var ode = new GearsBDFWithNordsieckState();
      var yinitial = new double[100];
      yinitial.FillWith(1);
      ode.Initialize(0, yinitial, CalcRates, CalcJac);


      var points = ode.GetSolutionPointsVolatile(
        new OdeMethodOptions
        {
          MandatorySolutionPoints = new double[] { 0, 1000, 2000 },
          IncludeMandatorySolutionPointsInOutput = false,
          InitialStepSize = 1 / 64d,
          RelativeTolerance = 1E-4,
          AbsoluteTolerance = 1E-4,
          AutomaticStepSizeControl = true
        });

      double maxStepSize = 0;
      double previousStepSize = 0;
      double previousX = 0;

      foreach (var sp in points.TakeWhile(sp => sp.X < 2000))
      {
        var currentStepSize = sp.X - previousX;
        previousX = sp.X;
        previousStepSize = currentStepSize;

        maxStepSize = Math.Max(maxStepSize, currentStepSize);

        for (int i = 0; i < sp.Y_volatile.Length / 2; ++i) // solution has to be symmetric
          Assert.Equal(sp.Y_volatile[i], sp.Y_volatile[sp.Y_volatile.Length - 1 - i], 13);
      }

      AssertEx.Greater(maxStepSize, 1);
    }

    private static void GetJacobianForAutocatalytic(double x, double[] y, ref IMatrix<double> m)
    {
      m ??= new DoubleMatrix(1, 1);
      m[0, 0] = -2 * y[0];
    }

    [Fact]
    public void Test_ASSC_Autocatalytic()
    {
      double targetAccuracy = 1E-4;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 0 }, (x, y, d) => { d[0] = (1+y[0])*(1-y[0]); }, GetJacobianForAutocatalytic);
      var points = ode.GetSolutionPointsVolatile(
        new MultiStepMethodOptions
        {
          AbsoluteTolerance = 1E-5,
          RelativeTolerance = targetAccuracy,
          AutomaticStepSizeControl = true,
          IterationMethod = OdeIterationMethod.DoNotUseJacobian
        }); ;

      int cnt = 0;
      double prev_y = 0;
      double maxAbsError=0, maxRelError = 0;
      foreach (var p in points)
      {
        if (p.X > 5)
          break;

        ++cnt;

        var x = p.X;
        var y = p.Y_volatile[0];

        System.Diagnostics.Debug.WriteLine($"x={x}, y={y}");


        Assert.True(y >= prev_y);
        prev_y = y;

        var expected = (Math.Exp(2*x)-1)/(Math.Exp(2*x)+1);

        var absError = Math.Abs(p.Y_volatile[0] - expected);
        var relError = absError / Math.Abs(expected);

        maxAbsError = Math.Max(absError, maxAbsError);
        maxRelError = Math.Max(relError, maxRelError);


        //AssertEx.Equal(expected, p.Y_volatile[0], 0, targetAccuracy * cnt, string.Empty);
      }
      AssertEx.Less(maxRelError, targetAccuracy * cnt);

    }

    [Fact]
    public void Test_ASSC_AutocatalyticJac()
    {
      double targetAccuracy = 1E-4;
      var ode = new GearsBDFWithNordsieckState();
      ode.Initialize(0, new double[] { 0 }, (x, y, d) => { d[0] = (1 + y[0]) * (1 - y[0]); },GetJacobianForAutocatalytic);
      var points = ode.GetSolutionPointsVolatile(
        new MultiStepMethodOptions
        {
          AbsoluteTolerance = 1E-5,
          RelativeTolerance = targetAccuracy,
          AutomaticStepSizeControl = true,
          IterationMethod = OdeIterationMethod.UseJacobian
        }); ;

      int cnt = 0;
      double prev_y = 0;
      double maxAbsError = 0, maxRelError = 0;
      foreach (var p in points)
      {
        if (p.X > 5)
          break;

        ++cnt;

        var x = p.X;
        var y = p.Y_volatile[0];
        var expected = (Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1);

        System.Diagnostics.Debug.WriteLine($"x={x}, y={y}, exp={expected}");


        Assert.True(y >= prev_y);
        prev_y = y;


        var absError = Math.Abs(p.Y_volatile[0] - expected);
        var relError = absError / Math.Abs(expected);

        maxAbsError = Math.Max(absError, maxAbsError);
        maxRelError = Math.Max(relError, maxRelError);

        AssertEx.Less(maxRelError, targetAccuracy * cnt);

        //AssertEx.Equal(expected, p.Y_volatile[0], 0, targetAccuracy * cnt, string.Empty);
      }

    }
  }
}
