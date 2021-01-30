using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Calc.Ode
{
  public class AdamsExplicit_Tests
  {
    [Fact]
    public void TestAccuracyWithConstantStepSize_1_64()
    {
      const double stepSize = 1 / 64d;
      var expectedAbsoluteError = new double[] { 0, 2.9E-3, 3.8E-5, 5.2E-7, 7.5E-9, 1.1E-10, 1.7E-12, 2.5E-14, 3.9E-16 };
      var expectedRelativeError = new double[] { 0, 3.2E-2, 4.1E-4, 5.8E-6, 8.5E-8, 1.3E-9, 1.9E-11, 2.9E-13, 4.8E-15};
      
      for (int nStages = 1; nStages <= 8; ++nStages)
      {
        var ode2 = new AdamsExplicit(nStages);
        ode2.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });
        var it2 = ode2.GetSolutionPointsVolatileForStepSize( stepSize).GetEnumerator();

        var maxAbsErr = 0.0;
        int maxAbsErrIndex = -1;
        var maxRelErr = 0.0;
        int maxRelErrIndex = -1;

        int maxIdx = (int)(4 / stepSize);
        for (int i = 1; i <= maxIdx; ++i)
        {
          it2.MoveNext();
          Assert.Equal(i * stepSize, it2.Current.X);
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

        System.Diagnostics.Debug.WriteLine($"NSt={nStages}, Abs={maxAbsErr}, Rel={maxRelErr}");
        AssertEx.Less(maxAbsErr, expectedAbsoluteError[nStages]);
        AssertEx.Less(maxRelErr, expectedRelativeError[nStages]);
      }
    }

    [Fact]
    public void Test_FSS_AccuracyWithConstantStepSize_1_64_AndOptionalPoints()
    {
      const double stepSize = 1 / 64d;
      var expectedRelativeError = new double[] { 0, 3.2E-2, 4.1E-4, 5.8E-6, 8.5E-8, 1.3E-9, 1.9E-11, 2.9E-13, 4.8E-15 };


      for (int nStages = 1; nStages <= 8; ++nStages)
      {
        var ode2 = new AdamsExplicit(nStages);
        ode2.Initialize(0, new double[] { 1 }, (x, y, d) => { d[0] = -y[0]; });
        var it2 = ode2.GetSolutionPointsVolatile(new RungeKuttaOptions { StepSize = stepSize, OptionalSolutionPoints = RungeKuttaOptions.GetEquidistantSequence(stepSize/2d, stepSize) }).GetEnumerator();

        var maxRelErr_InterpolatedPoints = 0.0;
        int maxRelErr_InterpolatedPointsIndex = -1;
        var maxRelErr_TruePoints = 0.0;
        int maxRelErr_TruePointsIndex = -1;

        double x_prev = 0;
        double y_prev = 1;
        for (int i = 1; i <= 512; ++i)
        {
          it2.MoveNext();
          double x_expected = i*stepSize/2d;
          var yexpected = Math.Exp(-it2.Current.X);

          Assert.Equal(x_expected, it2.Current.X);
          var y2 = it2.Current.Y_volatile[0];
          var errRel = Math.Abs((y2 - yexpected) / yexpected);


          if (i % 2 == 1 && errRel > maxRelErr_InterpolatedPoints)
          {
            maxRelErr_InterpolatedPoints = errRel;
            maxRelErr_InterpolatedPointsIndex = i;

            // show that we are better than linear interpolation
            double y_linear = (Math.Exp(-(x_expected - 1 / 128d)) + Math.Exp(-(x_expected + 1 / 128d))) / 2d;
            var errRelLinear = Math.Abs((y_linear - yexpected) / yexpected);
          }
          if (i % 2 == 0 && errRel > maxRelErr_TruePoints)
          {
            maxRelErr_TruePoints = errRel;
            maxRelErr_TruePointsIndex = i;
          }

          y_prev = it2.Current.Y_volatile[0];
          x_prev = it2.Current.X;

        }

        System.Diagnostics.Debug.WriteLine($"NSt={nStages}, IP={maxRelErr_InterpolatedPoints}, TP={maxRelErr_TruePoints}");
        AssertEx.Less(maxRelErr_InterpolatedPoints, expectedRelativeError[nStages]);
        AssertEx.Less(maxRelErr_TruePoints, expectedRelativeError[nStages]);
      }
    }


  }
}
