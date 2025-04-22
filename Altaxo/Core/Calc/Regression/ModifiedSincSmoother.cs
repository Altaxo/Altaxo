/*
 *  Copyright notice
 *  This code is licensed under GNU General Public License (GPLv3) and
 *  Creative Commons Attribution-ShareAlike 4.0 (CC-BY-SA 4.0).
 *  When using and/or modifying this program for scientific work
 *  and the paper on it has been published, please cite the paper:
 *  M. Schmid, D. Rath and U. Diebold,
 * 'Why and how Savitzky-Golay filters should be replaced',
 *  ACS Measurement Science Au, 2022
 *  Author: Michael Schmid, IAP/TU Wien, 2021.
 *  https://www.iap.tuwien.ac.at/www/surface/group/schmid
 *
 *  Translation to C# by Dr. Dirk Lellinger 2025
 */

using System;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// A C# implementation of smoothing by a modified sinc
  /// kernel(MS or MS1), as described in M.Schmid and U.Diebold,
  /// 'Why and how Savitzky-Golay filters should be replaced'
  /// The term 'degree' is defined in analogy to Savitzky-Golay(SG) filters;
  /// the current MS filters have a similar frequency response as SG filters
  /// of the same degree(2, 4, ... 10).
  /// </summary>
  /// <remarks>
  /// Reference: Michael Schmid, David Rath, and Ulrike Diebold, 'Why and how Savitzky-Golay filters should be replaced', ACS Measurement Science Au 2022 2 (2), 185-196 DOI: 10.1021/acsmeasuresciau.1c00054
  /// </remarks>
  public class ModifiedSincSmoother
  {
    /// <summary>
    /// This implementation is for a maximum degree of 10.
    /// </summary>
    public const int MAX_DEGREE = 10;

    /// <summary>
    /// Coefficients for the MS filters, for obtaining a flat passband.
    /// The innermost arrays contain a, b, c for the fit
    /// kappa = a + b/(c - m) 
    /// </summary>
    private static readonly double[][]?[] CORRECTION_DATA = [
            null, //not defined for degree 0
            null, //no correction required for degree 2
            null, //no correction required for degree 4
            //data for 6th degree coefficient for flat passband
            [[0.001717576, 0.02437382, 1.64375]],
            //data for 8th degree coefficient for flat passband
             [[0.0043993373, 0.088211164, 2.359375],
             [0.006146815, 0.024715371, 3.6359375]],
            //data for 10th degree coefficient for flat passband
             [[0.0011840032, 0.04219344, 2.746875],
             [0.0036718843, 0.12780383, 2.7703125]]
    ];
    /// <summary>
    /// Coefficients for the MS1 filters, for obtaining a flat passband.
    /// The innermost arrays contain a, b, c for the fit
    /// kappa = a + b/(c - m) 
    /// </summary>
    private static readonly double[][]?[] CORRECTION_DATA1 = [
            null, //not defined for degree 0
            null, //no correction required for degree 2
            //data for 4th degree coefficient for flat passband, a, b, c
            [[0.021944195, 0.050284006, 0.765625]],
            //data for 6th degree coefficient for flat passband
            [[0.0018977303, 0.008476806, 1.2625],
             [0.023064667, 0.13047926, 1.2265625]],
            //data for 8th degree coefficient for flat passband
            [[0.0065903002, 0.057929456, 1.915625],
             [0.0023234477, 0.010298849, 2.2726562],
             [0.021046653, 0.16646601, 1.98125]],
            //data for 10th degree coefficient for flat passband
            [[9.749618E-4, 0.0020742896, 3.74375],
             [0.008975366, 0.09902466, 2.7078125],
             [0.0024195414, 0.010064855, 3.296875],
             [0.019185117, 0.18953617, 2.784961]],
    ];

    /// <summary>
    /// Whether MS1 (not MS) filtering should be used
    /// </summary>
    private bool isMS1;

    /// <summary>
    /// The degree (2, 4, ... 10).
    /// </summary>
    private int degree;

    /// <summary>
    /// The kernel for filtering.
    /// </summary>
    private double[] kernel;

    /// <summary>
    /// The weights for linear fitting for extending the data at the boundaries
    /// </summary>
    private double[] fitWeights;

    /// <summary>
    /// Creates a ModifiedSincSmoother with given degree and given kernel size.
    /// This constructor is useful for repeated smoothing operations with
    /// the same parameter; then the non-static <see cref="Smooth(double[], double[])"/> 
    /// can be used without calculating the kernel and boundary fit weights each
    /// time.
    /// Otherwise the static <see cref="Smooth(double[], bool, int, int)"/>
    /// method is more convenient.
    /// </summary>
    /// <param name="isMS1">isMS1 if true, uses the MS1 variant, which has a smaller kernel size,
    /// at the cost of reduced stopband suppression and more gradual cutoff
    /// for degree=2. Otherwise, standard MS kernels are used.</param>
    /// <param name="degree">Degree of the filter, must be 2, 4, ... MAX_DEGREE.
    /// As for Savitzky-Golay filters, higher degree results in
    /// a sharper cutoff in the frequency domain.</param>
    /// <param name="m">The half-width of the kernel, must be larger than degree/2.
    /// The kernel size is <code>2*m + 1</code>.
    /// The <paramref name="m"/> parameter can be determined with bandwidthToM.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Invalid degree " + degree + "; only 2, 4, ... " + MAX_DEGREE + " supported
    /// or
    /// Invalid kernel half-width " + m + "; must be >= " + mMin
    /// </exception>

    public ModifiedSincSmoother(bool isMS1, int degree, int m)
    {
      this.isMS1 = isMS1;
      this.degree = degree;
      if (degree < 2 || degree > MAX_DEGREE || (degree & 0x1) != 0)
        throw new ArgumentOutOfRangeException("Invalid degree " + degree + "; only 2, 4, ... " + MAX_DEGREE + " supported");
      int mMin = isMS1 ? degree / 2 + 1 : degree / 2 + 2;
      if (m < mMin)  //kernel not wide enough for the wiggles of the sinc function
        throw new ArgumentOutOfRangeException("Invalid kernel half-width " + m + "; must be >= " + mMin);
      kernel = MakeKernel(isMS1, degree, m);
      fitWeights = MakeFitWeights(isMS1, degree, m);
    }

    /// <summary>
    /// Smooths the data with the parameters passed with the constructor,
    /// except for the near-end points.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">The output array; may be null. If <paramref name="result"/> is
    /// supplied, has the correct size, and is not the input array,
    /// it is used for the output.</param>
    /// <returns>The smoothed data. If <paramref name="result"/> is non-null and has
    /// the correct size, this is the <paramref name="result"/> array.
    /// Values within <code>m</code> points from boundaries, where
    /// the convolution is undefined remain 0 (or retain the
    /// previous value, if the supplied <paramref name="result"/> array
    /// is used).</returns>

    public double[] SmoothExceptBoundaries(double[] data, double[]? result)
    {
      if (result is null || result.Length != data.Length || object.ReferenceEquals(result, data))
        result = new double[data.Length];
      int radius = kernel.Length - 1; //how many additional points we need
      for (int i = radius; i < data.Length - radius; i++)
      {
        double sum = kernel[0] * data[i];
        for (int j = 1; j < kernel.Length; j++)
        {
          sum += kernel[j] * (data[i - j] + data[i + j]);
        }
        result[i] = sum;
      }
      return result;
    }

    /// <summary>
    /// Smooths the data with the parameters passed with the constructor,
    /// including the near-boundary points. The near-boundary points are
    /// handled by weighted linear extrapolation of the data before smoothing.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">The output array; may be null. If <paramref name="result"/> is
    /// supplied and has the correct size, it is used for the output.</param>
    /// <returns>The smoothed data. If <paramref name="result"/> is non-null and has
    /// the correct size, this is the <paramref name="result"/> array.</returns>
    public double[] Smooth(double[] data, double[]? result)
    {
      int radius = kernel.Length - 1;
      double[] extendedData = ExtendData(data, radius);
      double[] extendedSmoothed = SmoothExceptBoundaries(extendedData, null);
      if (result is null || result.Length != data.Length)
        result = new double[data.Length];
      Array.Copy(extendedSmoothed, radius, result, 0, data.Length);

      return result;
    }

    /// <summary>
    /// Smooths the data and with the given parameters.
    /// When smoothing multiple data sets with the same parameters,
    /// using the constructor and then smooth(double[], double[])
    /// will be more efficient.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="isMS1">if true, uses the MS1 variant, which has a smaller kernel size,
    /// at the cost of reduced stopband suppression and more gradual cutoff
    /// for degree=2. Otherwise, standard MS kernels are used.</param>
    /// <param name="degree">Degree of the filter, must be 2, 4, 6, ... MAX_DEGREE.
    /// As for Savitzky-Golay filters, higher degree results in
    /// a sharper cutoff in the frequency domain.</param>
    /// <param name="m">The half-width of the kernel. The kernel size is
    /// <code>2*m + 1</code>. The <paramref name="m"/> parameter can be
    /// determined with bandwidthToM.</param>
    /// <returns>The smoothed data. Values within <paramref name="m"/> points from
    /// boundaries, where the convolution is undefined, are set to 0.</returns>
    public static double[] Smooth(double[] data, bool isMS1, int degree, int m)
    {
      ModifiedSincSmoother smoother = new ModifiedSincSmoother(isMS1, degree, m);
      return smoother.Smooth(data, null);
    }

    /// <summary>
    /// Smooths the data in a way comparable to a traditional Savitzky-Golay
    /// filter with the given parameters <paramref name="degree"/> and <paramref name="m"/>.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="isMS1">if true, uses the MS1 variant, which has a smaller kernel size,
    /// at the cost of reduced stopband suppression and more gradual cutoff
    /// for degree=2. Otherwise, standard MS kernels are used.</param>
    /// <param name="degree"> Degree of the Savitzky-Golay filter that should be replaced,
    /// must be 2, 4, 6, ... MAX_DEGREE.</param>
    /// <param name="m">The half-width of a Savitzky-Golay filter that should be replaced.</param>
    /// <returns>The smoothed data.</returns>
    public static double[] SmoothLikeSavitzkyGolay(double[] data, bool isMS1, int degree, int m)
    {
      double bandwidth = SavitzkyGolayBandwidth(degree, m);
      int mMS = BandwidthToM(isMS1, degree, bandwidth);
      return Smooth(data, isMS1, degree, mMS);
    }

    /// <summary>
    /// Calculates the kernel halfwidth m that comes closest to the desired
    /// band width, i.e., the frequency where the response decreases to
    /// -3 dB, i.e., 1/sqrt(2).
    /// </summary>
    /// <param name="isMS1">isMS1 if true, calculates for the MS1 variant, which has a smaller kernel size,
    /// at the cost of reduced stopband suppression and more gradual cutoff
    /// for degree=2. Otherwise, standard MS kernels are used.</param>
    /// <param name="degree">Degree of the filter, must be 2, 4, 6, ... MAX_DEGREE.
    /// As for Savitzky-Golay filters, higher degree results in
    /// a sharper cutoff in the frequency domain.</param>
    /// <param name="bandwidth">The desired band width, with respect to the sampling frequency.
    /// The value of <paramref name="bandwidth"/> must be less than 0.5
    /// (the Nyquist frequency).</param>
    /// <returns>The kernel halfwidth m.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Invalid bandwidth value: " + bandwidth</exception>
    public static int BandwidthToM(bool isMS1, int degree, double bandwidth)
    {
      if (bandwidth <= 0 || bandwidth >= 0.5)
        throw new ArgumentOutOfRangeException("Invalid bandwidth value: " + bandwidth);
      double radius = isMS1 ?
              (0.27037 + 0.24920 * degree) / bandwidth - 1.0 :
              (0.74548 + 0.24943 * degree) / bandwidth - 1.0;
      return (int)Math.Round(radius);
    }

    /// <summary>
    /// Calculates the kernel halfwidth m best suited for obtaining a given noise gain.
    /// </summary>
    /// <param name="isMS1">if true, calculates for the MS1 variant, which has a smaller kernel size,
    /// at the cost of reduced stopband suppression and more gradual cutoff
    /// for degree=2. Otherwise, standard MS kernels are used.</param>
    /// <param name="degree">The degree n of the kernel</param>
    /// <param name="noiseGain">The factor by which white noise should be suppressed.</param>
    /// <returns>The kernel halfwidth m required.</returns>
    public static int NoiseGainToM(bool isMS1, int degree, double noiseGain)
    {
      double invNoiseGainSqr = 1.0 / (noiseGain * noiseGain);
      double exponent = -2.5 - 0.8 * degree;
      double m = isMS1 ?
              -1 + invNoiseGainSqr * (0.543 + 0.4974 * degree) +
                   0.47 * Math.Pow(invNoiseGainSqr, exponent) :
              -1 + invNoiseGainSqr * (1.494 + 0.4965 * degree) +
                   0.52 * Math.Pow(invNoiseGainSqr, exponent);
      return (int)Math.Round(m);
    }

    /// <summary>
    /// Creates a kernel and returns it.
    /// </summary>
    /// <param name="isMS1">if true, calculates the kernel for the MS1 variant.
    /// Otherwise, standard MS kernels are used.</param>
    /// <param name="degree">The degree n of the kernel</param>
    /// <param name="m">The half-width of the SG kernel. The kernel size of the
    /// filter is <code>2*m + 1</code>.</param>
    /// <returns>One side of the kernel, starting with the element at the
    /// center.Since the kernel is symmetric, only one side with
    /// <code>m+1</code> elements is needed.</returns>
    private static double[] MakeKernel(bool isMS1, int degree, int m)
    {
      var coeffs = GetCoefficients(isMS1, degree, m);
      return MakeKernel(isMS1, degree, m, coeffs);
    }

    /// <summary>
    /// Creates a kernel and returns it.
    /// </summary>
    /// <param name="isMS1">If true, calculates the kernel for the MS1 variant.
    /// Otherwise, standard MS kernels are used.</param>
    /// <param name="degree">The degree n of the kernel, i.e. the polynomial degree of a Savitzky-Golay filter
    /// with similar passband, must be 2, 4, ... MAX_DEGREE.</param>
    /// <param name="m">The half-width of the kernel (the resulting kernel
    /// has <code>2*m+1</code> elements).</param>
    /// <param name="coeffs">Correction parameters for a flatter passband, or
    /// null for no correction (used for degree 2).</param>
    /// <returns>One side of the kernel, starting with the element at the
    /// center. Since the kernel is symmetric, only one side with
    /// <code>m+1</code> elements is needed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unsupported degree " + degree</exception>
    private static double[] MakeKernel(bool isMS1, int degree, int m, double[]? coeffs)
    {
      if (degree < 2 || degree > MAX_DEGREE || (degree & 0x01) != 0)
        throw new ArgumentOutOfRangeException("Unsupported degree " + degree);
      double[] kernel = new double[m + 1];
      int nCoeffs = coeffs is null ? 0 : coeffs.Length;
      double sum = 0;
      for (int i = 0; i <= m; i++)
      {
        double x = i * (1.0 / (m + 1)); //0 at center, 1 at zero
        double sincArg = Math.PI * 0.5 * (isMS1 ? degree + 2 : degree + 4) * x;
        double k = i == 0 ? 1 : Math.Sin(sincArg) / sincArg;
        for (int j = 0; j < nCoeffs; j++)
        {
          if (isMS1)
            k += coeffs![j] * x * Math.Sin((j + 1) * Math.PI * x); //shorter kernel version, needs more correction terms
          else
          {
            int nu = ((degree / 2) & 0x1) == 0 ? 2 : 1; //start at 1 for degree 6, 10; at 2 for degree 8
            k += coeffs![j] * x * Math.Sin((2 * j + nu) * Math.PI * x);
          }
        }
        double decay = isMS1 ? 2 : 4;  //decay alpha =2: 13.5% at end without correction, 2sqrt2 sigma
        k *= Math.Exp(-x * x * decay) + Math.Exp(-(x - 2) * (x - 2) * decay) +
                Math.Exp(-(x + 2) * (x + 2) * decay) - 2 * Math.Exp(-decay) - Math.Exp(-9 * decay);
        kernel[i] = k;
        sum += k;
        if (i > 0) sum += k;    //off-center kernel elements appear twice
      }
      for (int i = 0; i <= m; i++)
        kernel[i] *= 1.0 / sum;    //normalize the kernel to sum = 1
      return kernel;
    }

    /// <summary>
    /// Returns the correction coefficients for a Sinc*Gaussian kernel
    /// to flatten the passband.
    /// </summary>
    /// <param name="isMS1">If true, returns the coefficients for the MS1 variant.
    /// Otherwise, coefficients for the standard MS kernels returned.</param>
    /// <param name="degree">The polynomial degree of a Savitzky-Golay filter
    /// with similar passband, must be 2, 4, ... MAX_DEGREE.</param>
    /// <param name="m">The half-width of the kernel.</param>
    /// <returns>Coefficients z for the x*sin((j+1)*PI*x) terms, or null
    /// if no correction is required.</returns>
    private static double[]? GetCoefficients(bool isMS1, int degree, int m)
    {
      var correctionData = isMS1 ? CORRECTION_DATA1 : CORRECTION_DATA;
      var corrForDeg = correctionData[degree / 2];
      if (corrForDeg is null)
        return null;
      double[] coeffs = new double[corrForDeg.Length];
      for (int i = 0; i < corrForDeg.Length; i++)
      {
        double[] abc = corrForDeg[i];   // a...c of equation a + b/(c - m)^3
        double cm = abc[2] - m;         // c - m
        coeffs[i] = abc[0] + abc[1] / (cm * cm * cm);
      }
      return coeffs;
    }

    /// <summary>
    /// Returns the weights for the linear fit used for linear extrapolation
    /// at the end. The weight function is a Hann (cos^2) function. For beta=1
    /// (the beta value for n=4), it decays to zero at the position of the
    /// first zero of the sinc function in the kernel. Larger beta values lead
    /// to stronger noise suppression near the edges, but the smoothed curve
    /// does not follow the input as well as for lower beta (for high degrees,
    /// also leading to more ringing near the boundaries).    /// </summary>
    /// <param name="isMS1">if true, returns weights for the MS1 variant. Otherwise, returns weights for the standard MS kernels.</param>
    /// <param name="degree">The polynomial degree of a Savitzky-Golay filter with similar passband, must be 2, 4, ... MAX_DEGREE.</param>
    /// <param name="m">The half-width of the kernel (the resulting kernel has 2*m+1 elements).</param>
    /// <returns>The fit weights, with array element [0] corresponding to the data value at the very end.</returns>
    private static double[] MakeFitWeights(bool isMS1, int degree, int m)
    {
      double firstZero = isMS1 ?      //the first zero of the sinc function
          (m + 1) / (1 + 0.5 * degree) :
          (m + 1) / (1.5 + 0.5 * degree);
      double beta = isMS1 ?
              0.65 + 0.35 * Math.Exp(-0.55 * (degree - 4)) :
              0.70 + 0.14 * Math.Exp(-0.60 * (degree - 4));
      int fitLength = (int)Math.Ceiling(firstZero * beta);
      double[] weights = new double[fitLength];
      for (int p = 0; p < fitLength; p++)
        weights[p] = Sqr(Math.Cos(0.5 * Math.PI / (firstZero * beta) * p));
      return weights;
    }

    /// <summary>
    /// Calculates the bandwidth of a traditional Savitzky-Golay (SG) filter.
    /// </summary>
    /// <param name="degree">The degree of the polynomial fit used in the Savitzky-Golay filter.</param>
    /// <param name="m">The half-width of the SG kernel. The kernel size of the SG filter, i.e. the number of points for fitting the polynomial is <code>2*m + 1</code>.</param>
    /// <returns>The -3 dB-bandwidth of the SG filter, i.e. the frequency where the
    /// response is 1/sqrt(2). The sampling frequency is defined as f = 1.
    /// For <paramref name="degree"/> up to 10, the accuracy is typically much
    /// better than 1%; higher errors occur only for the lowest
    /// <paramref name="m"/> values where the SG filter is defined
    /// (worst case: 4% error at <code>degree = 10, m = 6</code>).</returns>
    public static double SavitzkyGolayBandwidth(int degree, int m)
    {
      return 1.0 / (6.352 * (m + 0.5) / (degree + 1.379) - (0.513 + 0.316 * degree) / (m + 0.5));
    }

    /// <summary>
    /// Extends the data by a weighted fit to a linear function (linear regression).
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="m">The halfwidth of the kernel. The number of data points contributing to one output value is <code>2*m + 1</code>.</param>
    /// <returns>The input data with extrapolated values appended at both ends. At each end, <paramref name="m"/> extrapolated points are appended.</returns>
    private double[] ExtendData(double[] data, int m)
    {
      double[] extendedData = new double[data.Length + 2 * m];
      Array.Copy(data, 0, extendedData, m, data.Length);
      LinearRegression linreg = new LinearRegression();
      // linear fit of first points and extrapolate
      int fitLength = (int)Math.Min(fitWeights.Length, data.Length);
      for (int p = 0; p < fitLength; p++)
        linreg.AddPointW(p, data[p], fitWeights[p]);
      double offset = linreg.GetOffset();
      double slope = linreg.GetSlope();
      for (int p = -1; p >= -m; p--)
        extendedData[m + p] = offset + slope * p;
      // linear fit of last points and extrapolate
      linreg.Clear();
      for (int p = 0; p < fitLength; p++)
        linreg.AddPointW(p, data[data.Length - 1 - p], fitWeights[p]);
      offset = linreg.GetOffset();
      slope = linreg.GetSlope();
      for (int p = -1; p >= -m; p--)
        extendedData[data.Length + m - 1 - p] = offset + slope * p;
      return extendedData;
    }

    /// <summary>
    /// Returns the square of a number.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The square of a number.</returns>
    private static double Sqr(double x)
    { return x * x; }

  }

  /// <summary>
  /// Linear regression is used for extrapolating the data at the boundaries.
  /// </summary>
  internal class LinearRegression
  {
    /** sum of weights (number of points if all weights are one */
    protected double sumWeights = 0;
    /** sum of all x values */
    protected double sumX = 0;
    /** sum of all y values */
    protected double sumY = 0;
    /** sum of all x*y products */
    protected double sumXY = 0;
    /** sum of all squares of x */
    protected double sumX2 = 0;
    /** sum of all squares of y */
    protected double sumY2 = 0;
    /** result of the regression: offset */
    protected double offset = double.NaN;
    /** result of the regression: slope */
    protected double slope = double.NaN;
    /** whether the results (offset, slope) have been calculated already */
    protected bool calculated = false;

    /// <summary>
    ///  Clear the linefit.
    /// </summary>
    public void Clear()
    {
      sumWeights = 0;
      sumX = 0;
      sumY = 0;
      sumXY = 0;
      sumX2 = 0;
      sumY2 = 0;
      calculated = false;
    }

    /// <summary>
    /// Add a point x,y with weight.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <param name="weight">The weight value.</param>
    public void AddPointW(double x, double y, double weight)
    {
      sumWeights += weight;
      sumX += weight * x;
      sumY += weight * y;
      sumXY += weight * x * y;
      sumX2 += weight * x * x;
      sumY2 += weight * y * y;
      calculated = false;
    }

    /// <summary>
    ///  Do the actual regression calculation.
    /// </summary>
    public void Calculate()
    {
      double stdX2TimesN = sumX2 - sumX * sumX * (1 / sumWeights);
      if (sumWeights > 0)
      {
        slope = (sumXY - sumX * sumY * (1 / sumWeights)) / stdX2TimesN;
        if (double.IsNaN(slope)) slope = 0;       //slope 0 if only one x value
      }
      else
      {
        slope = double.NaN;
      }
      offset = (sumY - slope * sumX) / sumWeights;
      calculated = true;
    }

    /// <summary>
    /// Gets the offset(intersection on y axis) of the fit.
    /// </summary>
    /// <returns>the offset (intersection on y axis) of the fit.</returns>
    public double GetOffset()
    {
      if (!calculated) Calculate();
      return offset;
    }

    /// <summary>
    /// Gets the slope of the line of the fit.
    /// </summary>
    /// <returns>The slope of the line of the fit</returns>
    public double GetSlope()
    {
      if (!calculated) Calculate();
      return slope;
    }
  }

}
