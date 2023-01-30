using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altaxo.Calc.IntegralTransforms;

namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  public class PeakEnhancementHilbertHuangTransformation
  {
  }

  /// <summary>
  /// 
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>[1] Huang, N. E., Shen, Z., Long, S. R., Wu, M. C., Shih, H. H., Zheng, Q., … Liu, H. H. (1998). The empirical mode decomposition and the Hilbert spectrum for nonlinear and non-stationary time series analysis. Proceedings of the Royal Society of London. Series A: Mathematical, Physical and Engineering Sciences, 454(1971), 903–995. https://doi.org/10.1098/rspa.1998.0193</para>
  /// </remarks>
  public class EmpiricalModeDecomposition
  {
    /// <summary>
    /// Extracts the modes of a signal by empirical mode decomposition.
    /// </summary>
    /// <param name="x">The x-values of the signal.</param>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="maximumNumberOfModesToReturn">The maximum number of modes to return from this enumeration.</param>
    /// <returns>An endless enumeration of signals, consisting of x-values, and y-values, and representing the modes of the original signal.</returns>
    public static IEnumerable<(double[] x, double[] y)> ExtractIntrinsicModeFunctionComponents(double[] x, double[] y, int maximumNumberOfModesToReturn)
    {
      var yy = (double[])y.Clone();
      for (int iMode = 0; iMode < maximumNumberOfModesToReturn; ++iMode)
      {
        var (_, ye) = ExtractIntrinsicModeFunctionComponent(x, yy);
        for (int i = 0; i < yy.Length; ++i)
        {
          yy[i] -= ye[i];
        }
        yield return (x, ye);
      }
    }

    /// <summary>
    /// Extracts an intrinsic mode function component (IMF) from the signal (x, y).
    /// </summary>
    /// <param name="x">The array of x-values of the signal.</param>
    /// <param name="y">The array of y-values of the signal.</param>
    /// <returns>The next mode extracted from the signal.</returns>
    /// <remarks>The number of sift iterations is fixed to 10 here.</remarks>
    public static (double[] x, double[] y) ExtractIntrinsicModeFunctionComponent(double[] x, double[] y)
    {
      var signal = (double[])y.Clone(); // clone signal because it is changed

      var minimaIndices = new List<int>(x.Length / 4); // holds the indices of the local minima of the signal
      var maximaIndices = new List<int>(x.Length / 4); // holds the indices of the local maxima of the signal

      for (int iIteration = 0; iIteration < 10; ++iIteration)
      {
        minimaIndices.Clear();
        maximaIndices.Clear();

        var yl = signal[0]; // left signal value
        var ym = signal[1]; // middle signal value

        // Determine the local minima and maxima of the current signal
        for (int i = 2; i < signal.Length; ++i)
        {
          var yr = signal[i]; // right signal value

          if ((ym < yr && ym <= yl) || (ym < yl && ym <= yr))
          {
            minimaIndices.Add(i - 1);
          }
          else if ((ym > yr && ym >= yl) || (ym > yl && ym >= yr))
          {
            maximaIndices.Add(i - 1);
          }

          yl = ym;
          ym = yr;
        }

        // prepare new arrays that can be used as input for the interpolations
        // of the lower envelope ...
        var xs = new double[minimaIndices.Count];
        var ys = new double[minimaIndices.Count];

        for (int i = 0; i < xs.Length; ++i)
        {
          xs[i] = x[minimaIndices[i]];
          ys[i] = signal[minimaIndices[i]];
        }
        var interpolationLowerEnvelope = Altaxo.Calc.Interpolation.CubicSpline.InterpolateAkimaSorted(xs, ys);

        // ... and of the upper envelope
        xs = new double[maximaIndices.Count];
        ys = new double[maximaIndices.Count];

        for (int i = 0; i < xs.Length; ++i)
        {
          xs[i] = x[maximaIndices[i]];
          ys[i] = signal[maximaIndices[i]];
        }
        var interpolationUpperEnvelope = Altaxo.Calc.Interpolation.CubicSpline.InterpolateAkimaSorted(xs, ys);

        // subtract from the signal the mean of the lower and upper envelope
        for (int i = 0; i < signal.Length; ++i)
        {
          double xv = x[i];
          signal[i] -= 0.5 * (interpolationLowerEnvelope.Interpolate(xv) + interpolationUpperEnvelope.Interpolate(xv));
        }
      }
      return (x, y);
    }

    public static Complex[] HilbertTransformation(double[] xr)
    {
      var x = xr.Select(sample => new Complex(sample, 0)).ToArray();
      Fourier.Forward(x, FourierOptions.Default);

      var h = new double[x.Length];
      var fftLengthIsOdd = (x.Length | 1) == 1;
      if (fftLengthIsOdd)
      {
        h[0] = 1;
        for (var i = 1; i < xr.Length / 2; i++) h[i] = 2;
      }
      else
      {
        h[0] = 1;
        h[(xr.Length / 2)] = 1;
        for (var i = 1; i < xr.Length / 2; i++) h[i] = 2;
      }

      for (var i = 0; i < x.Length; i++)
      {
        x[i] *= h[i];
      }

      Fourier.Inverse(x, FourierOptions.Default);
      return x;
    }
  }
}
