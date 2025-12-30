using System;
using Altaxo.Calc.IntegralTransforms;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  /// <summary>
  /// Experimental peak-enhancement approach based on empirical mode decomposition (EMD)
  /// and a Hilbert transformation.
  /// </summary>
  public class PeakEnhancementHilbertHuang // : IPeakEnhancement
  {
    /// <summary>
    /// Executes the transformation for the provided spectrum, processing each region independently.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The y-values of the spectrum.</param>
    /// <param name="regions">Optional region boundaries. If provided, each region is processed separately.</param>
    /// <returns>The transformed spectrum, and the (unchanged) region information.</returns>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yResult = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start);
        var ySpan = new ReadOnlySpan<double>(y, start, end - start);
        var yR = Execute(xSpan, ySpan);
        Array.Copy(yR, 0, yResult, start, end - start);
      }

      return (x, yResult, regions);
    }

    /// <summary>
    /// Executes the transformation on a single contiguous region of the spectrum.
    /// </summary>
    /// <param name="xSpan">The x-values of the region.</param>
    /// <param name="ySpan">The y-values of the region.</param>
    /// <returns>The transformed y-values for the region.</returns>
    private double[] Execute(ReadOnlySpan<double> xSpan, ReadOnlySpan<double> ySpan)
    {
      var ySum = new double[xSpan.Length];

      var emd = new EmpiricalModeDecomposition();

      foreach (var (yIMFC, yResidual, modeNo) in emd.ExtractIntrinsicModeFunctionComponents(xSpan.ToArray(), ySpan.ToArray(), 100, false))
      {

        if (modeNo >= 0)
        {
          var hilbertC = HilbertTransformation.Transformation(yIMFC);
          var instFreq = HilbertTransformation.GetInstantaneousFrequencies(hilbertC);
          for (int i = 0; i < instFreq.Length; i++)
          {
            if (instFreq[i] > 0.3)
              yIMFC[i] = 0;
          }
        }

        for (int i = 0; i < ySum.Length; i++)
          ySum[i] += yIMFC[i];
      }

      return ySum;
    }
  }



}
