using System;
using Xunit;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Tests for the <see cref="PeakFinder"/> class.
  /// </summary>
  public class PeakFinder_Test
  {
    /// <summary>
    /// Tests, that the instance should throw an exception if accessing result properties before executing.
    /// </summary>
    [Fact]
    public void Test_ShouldThrowIfNotExecuted()
    {
      var exception = Assert.Throws<InvalidOperationException>(() =>
      {
        var peakFinder = new PeakFinder();
        var len = peakFinder.PeakPositions.Length; // should throw an InvalidOperationException
      });
    }


    /// <summary>
    /// Tests to find a single peak.
    /// </summary>
    [Fact]
    public void Test_FindSinglePeak()
    {
      // Create a single peak

      var y = new double[100];
      for (int i = 0; i < y.Length; i++)
      {
        double x = (i - y.Length / 2.0) / 2.0;
        y[i] = 1 + Math.Exp(-x * x);
      }
      var peakFinder = new PeakFinder();
      var peaks = peakFinder.Execute(y);
      Assert.Single(peaks);
      Assert.Equal(y.Length / 2.0, peaks[0]);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Single(peakFinder.PeakPositions);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);



      // Test with prominence, prominence should be 1
      peakFinder.Execute(y, null, null, null, prominence: 0.01);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Single(peakFinder.PeakPositions);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);
    }

    /// <summary>
    /// Tests the height parameter.
    /// </summary>
    [Fact]
    public void Test_Heigth()
    {
      // Create a single peak
      var y = new double[100];
      for (int i = 0; i < y.Length; i++)
      {
        double x = (i - y.Length / 2.0) / 2.0;
        y[i] = 0.5 + 3 * Math.Exp(-x * x); // Base 0.5, maximum peak height 0.5+3
      }

      var peakFinder = new PeakFinder();
      // Test with height, heigth should be 0.5 + 3 = 3.5
      peakFinder.Execute(y, height: 3.4);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(1, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);

      Assert.Equal(1, peakFinder.PeakHeights?.Length);
      Assert.Equal(3.5, peakFinder.PeakHeights[0]);

      peakFinder.Execute(y, height: 3.6); // execute again with a height of 3.6, which exceeds the height of the peak
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(0, peakFinder.PeakPositions?.Length);

      Assert.Equal(0, peakFinder.PeakHeights?.Length);
    }

    /// <summary>
    /// Tests the threshold parameter.
    /// </summary>
    [Fact]
    public void Test_Threshold()
    {
      // Create a single peak
      var y = new double[100];
      for (int i = 0; i < y.Length; i++)
      {
        double x = (i - y.Length / 2.0) / 2.0;
        y[i] = 0.5 + 3 * Math.Exp(-x * x); // Base 0.5, maximum peak height 0.5+3
      }

      double expectedThreshold = y[y.Length / 2] - y[y.Length / 2 - 1];

      var peakFinder = new PeakFinder();
      // Test with threshold
      peakFinder.Execute(y, threshold: expectedThreshold * 0.99);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(1, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);

      Assert.Equal(1, peakFinder.LeftThresholds?.Length);
      Assert.Equal(expectedThreshold, peakFinder.LeftThresholds[0]);
      Assert.Equal(1, peakFinder.RightThresholds?.Length);
      Assert.Equal(expectedThreshold, peakFinder.RightThresholds[0]);

      peakFinder.Execute(y, threshold: expectedThreshold * 1.01); // execute again with a threshold, which exceeds the actual threshold
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(0, peakFinder.PeakPositions?.Length);

      Assert.Equal(0, peakFinder.LeftThresholds?.Length);
      Assert.Equal(0, peakFinder.RightThresholds?.Length);
    }

    /// <summary>
    /// Tests the distance parameter.
    /// </summary>
    [Fact]
    public void Test_Distance()
    {
      // Create a 3-folded peak
      var y = new double[100];
      for (int i = 0; i < y.Length; i++)
      {
        double xm = (i - y.Length / 2.0);
        double xl = xm - 5;
        double xr = xm + 5;
        xm /= 2;
        xl /= 2;
        xr /= 2;

        y[i] = 0.5 + 3 * Math.Exp(-xm * xm) + 1 * Math.Exp(-xr * xr) + 1 * Math.Exp(-xl * xl); // Base 0.5, maximum peak height 0.5+3
      }

      var peakFinder = new PeakFinder();
      // Test without anything: three peaks should be found
      peakFinder.Execute(y);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(3, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0 - 5, peakFinder.PeakPositions[0]);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[1]);
      Assert.Equal(y.Length / 2.0 + 5, peakFinder.PeakPositions[2]);

      peakFinder.Execute(y, null, distance: 4);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(3, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0 - 5, peakFinder.PeakPositions[0]);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[1]);
      Assert.Equal(y.Length / 2.0 + 5, peakFinder.PeakPositions[2]);

      peakFinder.Execute(y, null, distance: 6);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(1, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);

    }

    /// <summary>
    /// Tests the prominence parameter.
    /// </summary>
    [Fact]
    public void Test_Prominence()
    {
      // Create a single peak
      var y = new double[100];
      for (int i = 0; i < y.Length; i++)
      {
        double x = (i - y.Length / 2.0) / 2.0;
        y[i] = 0.5 + 3 * Math.Exp(-x * x); // Base 0.5, maximum peak height 0.5+3
      }

      var peakFinder = new PeakFinder();
      // Test with prominence, prominence should be 3.5-0.5 = 3
      peakFinder.Execute(y, prominence: 0.01);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(1, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);

      Assert.Equal(1, peakFinder.Prominences?.Length);
      Assert.Equal(3, peakFinder.Prominences[0]);

      Assert.Equal(1, peakFinder.LeftBases?.Length);
      AssertEx.Greater(y.Length / 2.0, peakFinder.LeftBases[0]);

      Assert.Equal(1, peakFinder.RightBases?.Length);
      AssertEx.Less(y.Length / 2.0, peakFinder.RightBases[0]);

      peakFinder.Execute(y, prominence: 3.1); // execute again with a prominence of 3.1, which exceeds the prominence of the peak
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(0, peakFinder.PeakPositions?.Length);

      Assert.Equal(0, peakFinder.Prominences?.Length);

      Assert.Equal(0, peakFinder.LeftBases?.Length);

      Assert.Equal(0, peakFinder.RightBases?.Length);
    }

    /// <summary>
    /// Tests the width parameter.
    /// </summary>
    [Fact]
    public void Test_Width()
    {
      const int distP = 80;
      const int wlh = 7; // half width of left peak
      const int wmh = 10; // half width of middle peak
      const int wrh = 5; // half width of right peak
      // Create a 3-folded peak, with different widths
      var y = new double[200];
      for (int i = 0; i < y.Length; i++)
      {
        double xm = (i - y.Length / 2.0);
        double xr = xm - distP;
        double xl = xm + distP;
        xm /= wmh;
        xl /= wlh;
        xr /= wrh;

        y[i] = 3 * Math.Pow(2, -xm * xm) + 1 * Math.Pow(2, -xr * xr) + 1 * Math.Pow(2, -xl * xl); // Base 0.5, maximum peak height 0.5+3
      }

      var peakFinder = new PeakFinder();
      // Test without anything: three peaks should be found
      peakFinder.Execute(y);
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(3, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0 - distP, peakFinder.PeakPositions[0]);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[1]);
      Assert.Equal(y.Length / 2.0 + distP, peakFinder.PeakPositions[2]);

      peakFinder.Execute(y, null, width: wrh * 2 - 1); // width below that of the narrowest peak => all peaks should be found
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(3, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0 - distP, peakFinder.PeakPositions[0]);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[1]);
      Assert.Equal(y.Length / 2.0 + distP, peakFinder.PeakPositions[2]);

      peakFinder.Execute(y, null, width: wlh * 2 - 1); // width below that of the 2nd narrowest peak => 2 peaks should be found; but right peak not
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(2, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0 - distP, peakFinder.PeakPositions[0]);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[1]);

      peakFinder.Execute(y, null, width: wmh * 2 - 1); // width below that of the 3rd narrowest peak => only middle peak should be foudn
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(1, peakFinder.PeakPositions?.Length);
      Assert.Equal(y.Length / 2.0, peakFinder.PeakPositions[0]);

      peakFinder.Execute(y, null, width: wmh * 2 + 1); // width above that of the widest peak => no peak should be found
      Assert.NotNull(peakFinder.PeakPositions);
      Assert.Equal(0, peakFinder.PeakPositions?.Length);

    }

  }
}
