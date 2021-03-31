#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Probability;
using Altaxo.Collections;
using Xunit;

namespace Altaxo.Calc.Ode
{
  public class RK546M_Tests
  {

    [Fact]
    public void TestAccuracyWithConstantStepSize_1_64()
    {
      var ode2 = new RK546M();
      var it2 = ode2.GetSolutionPointsVolatileForStepSize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; }, 1 / 64d).GetEnumerator();

      var maxAbsErr = 0.0;
      int maxAbsErrIndex = -1;
      var maxRelErr = 0.0;
      int maxRelErrIndex = -1;

      for (int i = 1; i <= 256; ++i)
      {
        it2.MoveNext();
        Assert.Equal(i / 64d, it2.Current.X);
        var y2 = it2.Current.Y_volatile[0];
        var yexpected = Math.Exp(-it2.Current.X);
        var errAbs = Math.Abs(y2 - yexpected);
        var errRel = Math.Abs((y2 - yexpected) / yexpected);


        if (errAbs > maxAbsErr)
        {
          maxAbsErr = errAbs;
          maxAbsErrIndex = i;
        }
        if (errRel > maxRelErr)
        {
          maxRelErr = errRel;
          maxRelErrIndex = i;
        }
      }

      AssertEx.Less(maxAbsErr, 1E-13);
      AssertEx.Less(maxRelErr, 2E-12);
    }

    [Fact]
    public void Test_FSS_AccuracyWithConstantStepSize_1_2()
    {
      var ode2 = new RK546M();
      var it2 = ode2.GetSolutionPointsVolatileForStepSize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; }, 1 / 2d).GetEnumerator();

      var maxAbsErr = 0.0;
      int maxAbsErrIndex = -1;
      var maxRelErr = 0.0;
      int maxRelErrIndex = -1;

      for (int i = 1; i <= 8; ++i)
      {
        it2.MoveNext();
        Assert.Equal(i / 2d, it2.Current.X);
        var y2 = it2.Current.Y_volatile[0];
        var yexpected = Math.Exp(-it2.Current.X);
        var errAbs = Math.Abs(y2 - yexpected);
        var errRel = Math.Abs((y2 - yexpected) / yexpected);


        if (errAbs > maxAbsErr)
        {
          maxAbsErr = errAbs;
          maxAbsErrIndex = i;
        }
        if (errRel > maxRelErr)
        {
          maxRelErr = errRel;
          maxRelErrIndex = i;
        }
      }

      AssertEx.Less(maxAbsErr, 1E-6);
      AssertEx.Less(maxRelErr, 1E-5);
    }

    [Fact]
    public void Test_FSS_AccuracyWithConstantStepSize_1_2_AndOptionalPoints()
    {
      var ode2 = new RK546M();
      ode2.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });
      var it2 = ode2.GetSolutionPointsVolatile(new OdeMethodOptions { StepSize = 0.5, OptionalSolutionPoints = OdeMethodOptions.GetEquidistantSequence(0.25, 0.5) }).GetEnumerator();

      var maxRelErr_InterpolatedPoints = 0.0;
      int maxRelErr_InterpolatedPointsIndex = -1;
      var maxRelErr_TruePoints = 0.0;
      int maxRelErr_TruePointsIndex = -1;

      double x_prev = 0;
      double y_prev = 1;
      for (int i = 1; i <= 16; ++i)
      {
        it2.MoveNext();
        double x_expected = i / 4d;
        var yexpected = Math.Exp(-it2.Current.X);

        Assert.Equal(x_expected, it2.Current.X);
        var y2 = it2.Current.Y_volatile[0];
        var errRel = Math.Abs((y2 - yexpected) / yexpected);


        if (i % 2 == 1 && errRel > maxRelErr_InterpolatedPoints)
        {
          maxRelErr_InterpolatedPoints = errRel;
          maxRelErr_InterpolatedPointsIndex = i;

          // show that we are better than linear interpolation
          double y_linear = (Math.Exp(-(x_expected - 1 / 4d)) + Math.Exp(-(x_expected + 1 / 4d))) / 2d;
          var errRelLinear = Math.Abs((y_linear - yexpected) / yexpected);
          AssertEx.Less(errRel, errRelLinear);
        }
        if (i % 2 == 0 && errRel > maxRelErr_TruePoints)
        {
          maxRelErr_TruePoints = errRel;
          maxRelErr_TruePointsIndex = i;
        }

        y_prev = it2.Current.Y_volatile[0];
        x_prev = it2.Current.X;

      }

      AssertEx.Less(maxRelErr_InterpolatedPoints, 2E-4);
      AssertEx.Less(maxRelErr_TruePoints, 1E-5);
    }

    [Fact]
    public void Test_FSS_Accuracy2VariablesWithConstantStepSize_1_16()
    {
      var ode2 = new RK546M();
      var it2 = ode2.GetSolutionPointsVolatileForStepSize(0,
        new double[] { 1, -1 },
        (t, x, r) => { r[0] = x[1] + 1; r[1] = -x[0] + 2; },
        1 / 16d).GetEnumerator();

      var maxAbsErr1 = 0.0;
      int maxAbsErr1Index = -1;
      var maxAbsErr2 = 0.0;
      int maxAbsErr2Index = -1;


      for (int i = 1; i <= 16 * 32; ++i)
      {
        it2.MoveNext();
        Assert.Equal(i / 16d, it2.Current.X);

        var y1expected = 2 - Math.Cos(it2.Current.X);
        var y2expected = -1 + Math.Sin(it2.Current.X);

        var y1 = it2.Current.Y_volatile[0];
        var y2 = it2.Current.Y_volatile[1];
        var err1Abs = Math.Abs(y1 - y1expected);
        var err2Abs = Math.Abs(y2 - y2expected);


        if (err1Abs > maxAbsErr1)
        {
          maxAbsErr1 = err1Abs;
          maxAbsErr1Index = i;
        }

        if (err2Abs > maxAbsErr2)
        {
          maxAbsErr2 = err2Abs;
          maxAbsErr2Index = i;
        }

      }

      AssertEx.Less(maxAbsErr1, 9E-9);
      AssertEx.Less(maxAbsErr2, 9E-9);
    }

    [Fact]
    public void TestInitialStepSize_ExponentialDecay()
    {
      var ode = new RK546M();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });
      ode.RelativeTolerance = 1E-6;
      ode.AbsoluteTolerance = 0;
      double initialStepSize = ode.GetInitialStepSize();
      AssertEx.Equal(Math.Pow(1E-8, 0.2), initialStepSize, 1E-6);
    }

    [Fact]
    public void Test_ASSC_AutomaticStepSize_ExponentialDecay()
    {
      double targetAccuracy = 1E-2;

      for (int i = 0; i <= 3; ++i)
      {
        targetAccuracy /= 100;

        var ode = new RK546M();
        ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });
        var points = ode.GetSolutionPointsVolatile(new OdeMethodOptions { InitialStepSize = 2, RelativeTolerance = targetAccuracy, AutomaticStepSizeControl = true });

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

        AssertEx.Less(maxRelErr, targetAccuracy * 100);
      }
    }

    /// <summary>
    /// Tests whether the step size is really increased during evaluation of the exponential decay
    /// </summary>
    [Fact]
    public void Test_ASSC_AutomaticStepSizeIncrease_ExponentialDecay()
    {
      double initialStepSize = 1 / 64d;
      var ode = new RK546M();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });
      var points = ode.GetSolutionPoints(new OdeMethodOptions { InitialStepSize = initialStepSize, RelativeTolerance = 1E-4, AbsoluteTolerance = 1E-4, AutomaticStepSizeControl = true });

      double maxStepSize = 0;
      double previousStepSize = 0;
      double previousX = 0;
      foreach (var sp in points.TakeWhile(sp => sp.X <= 10))
      {
        var currentStepSize = sp.X - previousX;
        AssertEx.GreaterOrEqual(currentStepSize, previousStepSize);
        previousX = sp.X;
        previousStepSize = currentStepSize;

        maxStepSize = Math.Max(maxStepSize, currentStepSize);
      }

      AssertEx.Greater(maxStepSize, 1);
    }

    [Fact]
    public void Test_ASSC_MandatoryPoints_ExponentialDecay()
    {
      var ode = new RK546M();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });

      var mandatoryPoints = OdeMethodOptions.GetEquidistantSequence(1 / 8d, 1 / 8d, 10);

      var points = ode.GetSolutionPointsVolatile(new OdeMethodOptions { InitialStepSize = 2, RelativeTolerance = 1E-6, AutomaticStepSizeControl = true, MandatorySolutionPoints = mandatoryPoints, IncludeAutomaticStepsInOutput = false });

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
        Assert.Equal((1+i)/8d, listOfX[i]);

      AssertEx.Less(maxRelErr, 5E-9);
    }

    [Fact]
    public void Test_ASSC_MandatoryAndOptionalPoints_ExponentialDecay()
    {
      var ode = new RK546M();
      ode.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });

      var mandatoryPoints = OdeMethodOptions.GetEquidistantSequence(1 / 8d, 1 / 8d, 10);
      var optionalPoints = OdeMethodOptions.GetEquidistantSequence(1 / 64d, 1 / 64d, 80);

      var points = ode.GetSolutionPointsVolatile(new OdeMethodOptions { InitialStepSize = 2, RelativeTolerance = 1E-6, AutomaticStepSizeControl = true, MandatorySolutionPoints = mandatoryPoints, OptionalSolutionPoints = optionalPoints, IncludeAutomaticStepsInOutput = false });

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

      AssertEx.Less(maxRelErr, 7E-7);
      AssertEx.Less(maxAbsErr, 6E-7);
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


      var ode = new RK546M();
      var yinitial = new double[100];
      yinitial.FillWith(1);
      ode.Initialize(0, yinitial, CalcRates);


      var points = ode.GetSolutionPointsVolatile(new OdeMethodOptions { InitialStepSize = 1 / 64d, RelativeTolerance = 1E-4, AbsoluteTolerance = 1E-4, AutomaticStepSizeControl = true });

      double maxStepSize = 0;
      double previousStepSize = 0;
      double previousX = 0;

      foreach (var sp in points.TakeWhile(sp => sp.X < 100))
      {
        var currentStepSize = sp.X - previousX;
        previousX = sp.X;
        previousStepSize = currentStepSize;

        maxStepSize = Math.Max(maxStepSize, currentStepSize);

        for (int i = 0; i < sp.Y_volatile.Length / 2; ++i) // solution has to be symmetric
          Assert.Equal(sp.Y_volatile[i], sp.Y_volatile[sp.Y_volatile.Length - 1 - i]);
      }

      AssertEx.Greater(maxStepSize, 1);
    }
  }
}
