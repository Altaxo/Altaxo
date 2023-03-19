using System;
using Altaxo.Calc.IntegralTransforms;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  public class PeakEnhancementHilbertHuang // : IPeakEnhancement
  {
    /// <inheritdoc/>
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
