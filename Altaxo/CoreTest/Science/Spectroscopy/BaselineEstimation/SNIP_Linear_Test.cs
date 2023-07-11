using System;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Xunit;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  public class SNIP_Linear_Test
  {
    const int len = 1001;
    const int height = 100;
    const double pos = len / 2.0;
    const double sigma = len / 50.0;

    (double[] x, double[] y) GetXLinearSpaced_YLinearSlopeWithPeak(double offset, double slope, double noiseAmplitude)
    {
      var x = EnumerableExtensions.EquallySpacedByStartStepCount(0, 15 / 16.0, len).ToArray();
      var y = new double[len];
      var ff = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, 1);
      var para = new double[ff.NumberOfParameters];

      para[0] = height;
      para[1] = pos;
      para[2] = sigma; // width
      para[3] = offset; // offset
      para[4] = slope; // slope
      ff.Evaluate(MatrixMath.ToROMatrixWithOneColumn(x), para, VectorMath.ToVector(y), null);

      // add noise - in this case only +1, and -1
      for (int i = 0; i < len; ++i)
      {
        y[i] += i % 2 == 0 ? -noiseAmplitude : noiseAmplitude;
      }

      return (x, y);
    }

    (double[] x, double[] y) GetXNotLinearSpaced_YLinearSlopeWithPeak(double offset, double slope, double noiseAmplitude)
    {
      var x = new double[len];
      for (int i = 0; i < len; ++i)
      {
        x[i] = i * 15 / 16.0 + i * i / 1024d;
      }
      var y = new double[len];
      var ff = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, 1);
      var para = new double[ff.NumberOfParameters];

      para[0] = height;
      para[1] = pos;
      para[2] = sigma; // width
      para[3] = offset; // offset
      para[4] = slope; // slope
      ff.Evaluate(MatrixMath.ToROMatrixWithOneColumn(x), para, VectorMath.ToVector(y), null);

      // add noise - in this case only +1, and -1
      for (int i = 0; i < len; ++i)
      {
        y[i] += i % 2 == 0 ? -noiseAmplitude : noiseAmplitude;
      }

      return (x, y);
    }

    /// <summary>
    /// When applying the SNIP algorithm to a spectrum with an equally spaced x-axis,
    /// then:
    /// - without noise, the first and the last y values should be zero
    /// - the y-values 
    /// </summary>
    [Fact]
    public void TestYValues_EquallySpaced()
    {
      foreach (var isHalfWidthInXUnits in new bool[] { false, true })
      {
        foreach (var isEquallySpaced in new bool[] { true, false })
        {
          foreach (var offset in new double[] { 555, -555 })
          {
            foreach (var slope in new double[] { 10, -10 })
            {
              foreach (var noise in new double[] { 0, 1 })
              {
                var (x, y) = isEquallySpaced ? GetXLinearSpaced_YLinearSlopeWithPeak(offset, slope, noise) : GetXNotLinearSpaced_YLinearSlopeWithPeak(offset, slope, noise);
                var snip = new Altaxo.Science.Spectroscopy.BaselineEstimation.SNIP_Linear() { IsHalfWidthInXUnits = isHalfWidthInXUnits, HalfWidth = sigma * 2, NumberOfRegularIterations = 40 };
                var (xx, yy, rr) = snip.Execute(x, y, null);


                Assert.Same(x, xx); // SNIP should return the same x-axis

                if (noise == 0)
                {
                  for (int i = 0; i < 10; ++i)
                  {
                    AssertEx.AreEqual(0, yy[i], 1E-12, 1E-12); // the first and
                    AssertEx.AreEqual(0, yy[yy.Length - 1 - i], 1E-12, 1E-12); // the last points should be zero (without noise)
                  }
                }

                for (int i = 0; i < len; ++i)
                {
                  AssertEx.LessOrEqual(0, yy[i]); // baseline should always be under the real curve
                }
              }
            }
          }
        }
      }
    }


    /// <summary>
    /// When applying the SNIP algorithm to a spectrum with an equally spaced x-axis, the result should not be dependent whether the x array is ordered descending or ascending.
    /// </summary>
    [Fact]
    public void TestSymmetry_EquallySpaced_HalfWidthInXUnits()
    {
      foreach (var offset in new double[] { 555, -555 })
      {
        foreach (var slope in new double[] { 10, -10 })
        {
          foreach (var noise in new double[] { 0, 1 })
          {
            foreach (var isHalfWidthInXUnits in new bool[] { false, true })
            {
              var (x, y) = GetXLinearSpaced_YLinearSlopeWithPeak(offset, slope, noise);
              var snip = new Altaxo.Science.Spectroscopy.BaselineEstimation.SNIP_Linear() { IsHalfWidthInXUnits = isHalfWidthInXUnits, HalfWidth = sigma * 2, NumberOfRegularIterations = 40 };
              var (xx0, yy0, rr0) = snip.Execute(x, y, null);

              // now reverse x and y
              Array.Reverse(x);
              Array.Reverse(y);

              var (xx1, yy1, rr1) = snip.Execute(x, y, null);
              // to compare the results, we reverse the result again
              Array.Reverse(xx1);
              Array.Reverse(yy1);
              Array.Reverse(x);
              Array.Reverse(y);

              for (int i = 0; i < len; ++i)
              {
                AssertEx.AreEqual(x[i], xx0[i], 0, 1E-12);
                AssertEx.AreEqual(x[i], xx1[i], 0, 1E-12);

                AssertEx.AreEqual(yy0[i], yy1[i], 0, 1E-12);
              }
            }
          }
        }
      }
    }

    private static void NormalizeW((int left, int right)[] w)
    {
      for (int i = 0; i < w.Length; ++i)
      {
        if (w[i].left >= (i + 1))
          w[i] = (i + 1, w[i].right);
        if (w[i].right >= w.Length - i)
          w[i] = (w[i].left, w.Length - i);
      }
    }

    [Fact]
    public void Test_Symmetry_LocalHalfWidth()
    {
      const int len = 129;
      const double halfwidth = 20;
      var x = new double[len];
      for (int i = 0; i < len; ++i)
      {
        x[i] = i + i * i / 256d;
      }

      var w0 = new (int left, int right)[len];
      var w1 = new (int left, int right)[len];

      SNIP_Base.CalculateHalfWidthInPointsLocally(x, halfwidth, w0);
      TestW(halfwidth, x, w0);

      Array.Reverse(x);
      SNIP_Base.CalculateHalfWidthInPointsLocally(x, 20, w1);
      TestW(halfwidth, x, w1);

      Array.Reverse(x);
      Array.Reverse(w1);
      for (int i = 0; i < len; ++i)
      {
        w1[i] = (w1[i].right, w1[i].left);
      }
      TestW(halfwidth, x, w1);

      NormalizeW(w0);
      NormalizeW(w1);

      for (int i = 0; i < len; ++i)
      {
        Assert.Equal(w1[i], w0[i]);
      }
    }

    private static void TestW(double halfwidth, double[] x, (int left, int right)[] w)
    {
      int len = x.Length;
      for (int i = len - 1; i >= 0; --i)
      {
        var wi = w[i];
        AssertEx.LessOrEqual(1, wi.left);
        AssertEx.LessOrEqual(1, wi.right);

        if (wi.left > i)
        {
          AssertEx.Greater(halfwidth, Math.Abs(x[i] - x[0]));
        }
        else
        {
          AssertEx.LessOrEqual(halfwidth, Math.Abs(x[i] - x[i - wi.left]));
          AssertEx.Greater(halfwidth, Math.Abs(x[i] - x[i - wi.left + 1]));
        }

        if (wi.right >= x.Length - i)
        {
          AssertEx.Greater(halfwidth, Math.Abs(x[^1] - x[i]));
        }
        else
        {
          AssertEx.LessOrEqual(halfwidth, Math.Abs(x[i] - x[i + wi.right]));
          AssertEx.Greater(halfwidth, Math.Abs(x[i] - x[i + wi.right - 1]));

        }
      }
    }

    /// <summary>
    /// When applying the SNIP algorithm to a spectrum with an not equally spaced x-axis,
    /// the result should not be dependent whether the x array is ordered descending or ascending.
    /// This is not trivial when the HalfWidth is given in x-units: in order to convert the HalfWidth in points, different methods can be used:
    /// - use the difference between x[0] and x[1] (but this is not symmetric, and this test would fail)
    /// - use the average of all differences between x[i] and x[i+1] (should be symmetric)
    /// - use the minimum value of all differences between x[i] and x[i+1] (should be symmetric)
    /// - use the maximum value of all differences between x[i] and x[i+1] (should be symmetric)
    /// - use the difference between x[i-1] and x[i+1] to calculate the half width for point i (if carefully done for the start and the end, should be symmetric)
    /// </summary>
    [Fact]
    public void TestSymmetry_NotEquallySpaced_HalfWidthInXUnits()
    {
      foreach (var offset in new double[] { 555, -555 })
      {
        foreach (var slope in new double[] { 10, -10 })
        {
          foreach (var isHalfWidthInXUnits in new bool[] { false, true })
          {
            foreach (var noise in new double[] { 0, 1 })
            {
              var (x, y) = GetXNotLinearSpaced_YLinearSlopeWithPeak(offset, slope, noise);
              var snip = new Altaxo.Science.Spectroscopy.BaselineEstimation.SNIP_Linear() { IsHalfWidthInXUnits = isHalfWidthInXUnits, HalfWidth = sigma * 2, NumberOfRegularIterations = 40 };
              var (xx0, yy0, rr0) = snip.Execute((double[])x.Clone(), y, null);

              // now reverse x and y
              Array.Reverse(x);
              Array.Reverse(y);

              var (xx1, yy1, rr1) = snip.Execute((double[])x.Clone(), y, null);
              // to compare the results, we reverse the result again
              Array.Reverse(xx1);
              Array.Reverse(yy1);
              Array.Reverse(x);
              Array.Reverse(y);

              for (int i = 0; i < len; ++i)
              {
                AssertEx.AreEqual(x[i], xx0[i], 0, 1E-12);
                AssertEx.AreEqual(x[i], xx1[i], 0, 1E-12);
                AssertEx.AreEqual(yy0[i], yy1[i], 1E-7, 1E-7);
              }
            }
          }
        }
      }
    }
  }
}
