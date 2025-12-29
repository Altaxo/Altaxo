/*
 *  Copyright notice
 *  This code is licensed under GNU General Public License (GPLv3).
 *  When using and/or modifying this program for scientific work
 *  and the paper on it has been published, please cite the paper:
 *  M. Schmid, D. Rath and U. Diebold,
 * 'Why and how Savitzky-Golay filters should be replaced',
 *  ACS Measurement Science Au, 2022
 *  <p>
 *  Author: Michael Schmid, IAP/TU Wien, Copyright (C) 2021.
 *  https://www.iap.tuwien.ac.at/www/surface/group/schmid
 *
 *  Translation to C# by Dr. Dirk Lellinger 2025
 */

using System;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// A C# implementation of Savitzky-Golay smoothing with optional weighting.
  /// Weights provide improved stopband suppression compared to traditional Savitzky-Golay (SG).
  /// Traditional SG smoothing can be obtained by using <see cref="WeightType.NONE"/>.
  /// </summary>
  /// <remarks>
  /// Reference: Michael Schmid, David Rath, and Ulrike Diebold, <c>"Why and how Savitzky-Golay filters should be replaced"</c>,
  /// ACS Measurement Science Au 2022 2 (2), 185–196. DOI: 10.1021/acsmeasuresciau.1c00054.
  /// </remarks>
  public class WeightedSavitzkyGolaySmoother
  {
    /// <summary>
    /// Specifies the weight function to use.
    /// The default is <see cref="NONE"/>, which results in traditional SG smoothing.
    /// </summary>
    public enum WeightType
    {
      /// <summary>
      /// No weighting; results in traditional SG smoothing.
      /// </summary>
      NONE = 0,

      /// <summary>
      /// Modified Gaussian with <c>alpha = 2</c>, as described in the paper by Schmid and Diebold.
      /// </summary>
      GAUSS2 = 1,

      /// <summary>
      /// Hann window function (also known as raised cosine or cosine-square).
      /// </summary>
      HANN = 2,

      /// <summary>
      /// Hann-squared window function (also known as <c>cos^4</c>).
      /// </summary>
      HANNSQR = 3,

      /// <summary>
      /// Hann-cubed window function (also known as <c>cos^6</c>).
      /// </summary>
      HANNCUBE = 4
    };

    /// <summary>
    /// Coefficients <c>a</c>, <c>b</c>, <c>c</c> for scaling the x-axis of near-edge kernel functions of the SGW filters.
    /// Used in the equation <c>s = 1 - a / (1 + b * pow(x, c))</c>.
    /// </summary>
    private static readonly double[][] weightScaleCoeffs = [
        [1, 1, -1],                     //weightType=NONE
        [0.68096, 0.36358, -3.68528],   //GAUSS2
        [0.67574, 0.35440, -3.61580],   //HANN
        [0.63944, 0.28417, -5.508],     //HANNSQR
        [0.62303, 0.25310, -7.07317]    //HANNCUBE
    ];

    /// <summary>
    /// The kernels for near-boundary and interior points.
    /// </summary>
    private double[][] kernels;

    /// <summary>
    /// Creates a <see cref="WeightedSavitzkyGolaySmoother"/> with the given weight function, polynomial degree, and kernel half-width.
    /// This constructor is useful for repeated smoothing operations with the same parameters.
    /// Otherwise, the static <see cref="Smooth(double[], double[], WeightType, int, int)"/> method is more convenient.
    /// </summary>
    /// <param name="weightType">Type of the weight function.</param>
    /// <param name="degree">Degree of the polynomial fit.</param>
    /// <param name="m">Half-width of the kernel.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="m"/> is too small for <paramref name="degree"/>.</exception>
    public WeightedSavitzkyGolaySmoother(WeightType weightType, int degree, int m)
    {
      kernels = MakeKernels(weightType, degree, m);
    }

    /// <summary>
    /// Smooths the data with the parameters passed to the constructor.
    /// This method is implemented only for data arrays that have at least <c>2*m + 1</c> elements,
    /// where <c>m</c> is the kernel half-width.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">
    /// The output array; may be <see langword="null"/>.
    /// If supplied and has the correct size, it is used for the output.
    /// It may also be the same instance as <paramref name="data"/>; in that case the input is overwritten.
    /// </param>
    /// <returns>
    /// The smoothed data. If <paramref name="result"/> is non-null and has the correct size, it is returned.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the input array is too short.</exception>
    public double[] Smooth(double[] data, double[]? result)
    {
      result = Convolve(data, result, kernels);
      return result;
    }

    /// <summary>
    /// Smooths the data with the specified weight function, polynomial degree, and kernel half-width <paramref name="m"/>.
    /// This method is implemented only for data arrays that have at least <c>2*m + 1</c> elements.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">
    /// The output array; may be <see langword="null"/>.
    /// If supplied and has the correct size and is not the input array, it is used for the output.
    /// </param>
    /// <param name="weightType">Type of the weight function; <see cref="WeightType.NONE"/> for traditional SG, <see cref="WeightType.HANNSQR"/> for the filters described in the paper.</param>
    /// <param name="degree">Degree of the polynomial fit.</param>
    /// <param name="m">Half-width of the kernel.</param>
    /// <returns>
    /// The smoothed data. If <paramref name="result"/> is non-null, has the correct size, and is not the input array, it is returned.
    /// </returns>
    public static double[] Smooth(double[] data, double[]? result, WeightType weightType, int degree, int m)
    {
      WeightedSavitzkyGolaySmoother smoother = new WeightedSavitzkyGolaySmoother(weightType, degree, m);
      return smoother.Smooth(data, result);
    }

    /// <summary>
    /// Smooths the data in a way comparable to a traditional Savitzky-Golay filter with parameters <paramref name="degree"/> and
    /// <paramref name="m"/>, but using Hann-squared weights, resulting in substantially better noise rejection.
    /// This method is implemented only for arrays that have at least as many elements as the SGW kernel.
    /// This is more than <c>2*m + 1</c>, but never more than <c>4*m + 1</c>.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="degree">The degree of the polynomial fit used in the SG(W) filter.</param>
    /// <param name="m">The half-width of the SG kernel to be matched (<c>2*m + 1</c> points).</param>
    /// <returns>The smoothed data.</returns>
    public static double[] SmoothLikeSG(double[] data, int degree, int m)
    {
      double bandwidth = SavitzkyGolayBandwidth(degree, m);
      int mSGW = BandwidthToHalfwidth(degree, bandwidth);
      return Smooth(data, null, WeightType.HANNSQR, degree, mSGW);
    }

    /// <summary>
    /// Calculates the kernel half-width <c>m</c> for a given bandwidth (the frequency where the response decreases to -3 dB,
    /// corresponding to <c>1/sqrt(2)</c>) for an SGW filter with <see cref="WeightType.HANNSQR"/> weights.
    /// </summary>
    /// <param name="degree">The degree of the polynomial fit used in the SGW filter.</param>
    /// <param name="bandwidth">
    /// The desired bandwidth with respect to the sampling frequency.
    /// The value must be less than 0.5 (the Nyquist frequency).
    /// </param>
    /// <returns>The kernel half-width <c>m</c> for this bandwidth.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bandwidth"/> is outside the valid range.</exception>
    public static int BandwidthToHalfwidth(int degree, double bandwidth)
    {
      if (bandwidth <= 0 || bandwidth >= 0.5)
        throw new ArgumentOutOfRangeException("Invalid bandwidth value: " + bandwidth);
      int m = (int)Math.Round((0.5090025 + degree * (0.1922392 - degree * 0.001484498)) / bandwidth - 1.0);
      return m;
    }

    /// <summary>
    /// Calculates the bandwidth of a traditional Savitzky-Golay (SG) filter.
    /// </summary>
    /// <param name="degree">The degree of the polynomial fit used in the SG filter.</param>
    /// <param name="m">
    /// The half-width of the SG kernel. The kernel size of the SG filter (i.e. the number of points for fitting the polynomial)
    /// is <c>2*m + 1</c>.
    /// </param>
    /// <returns>
    /// The -3 dB bandwidth of the SG filter, i.e. the frequency where the response is <c>1/sqrt(2)</c>.
    /// The sampling frequency is defined as <c>f = 1</c>.
    /// For <paramref name="degree"/> up to 10, the accuracy is typically much better than 1%; higher errors occur only for the lowest
    /// <paramref name="m"/> values where the SG filter is defined (worst case: 4% error at <c>degree = 10, m = 6</c>).
    /// </returns>
    public static double SavitzkyGolayBandwidth(int degree, int m)
    {
      return 1.0 / (6.352 * (m + 0.5) / (degree + 1.379) - (0.513 + 0.316 * degree) / (m + 0.5));
    }

    /// <summary>
    /// Convolves the data, using for each point the appropriate kernel for interior or near-boundary points.
    /// The output array may be supplied; if <see langword="null"/> or unsuitable, a new output array is created.
    /// This method is implemented only for data arrays that have at least <c>2*m + 1</c> elements.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">The output data (may be <see langword="null"/>).</param>
    /// <param name="kernels">The per-position kernels (near-edge kernels plus the interior kernel).</param>
    /// <returns>The convolved (smoothed) data.</returns>
    /// <exception cref="ArgumentException">Thrown if the input data array is too short.</exception>
    private static double[] Convolve(double[] data, double[]? result, double[][] kernels)
    {
      int m = kernels[^1].Length / 2;
      if (data.Length < 2 * m + 1)
        throw new ArgumentException("Data array too short; min length: " + (2 * m + 1));
      if (result is null || result.Length != data.Length || object.ReferenceEquals(result, data))
        result = new double[data.Length];
      for (int i = 0; i < data.Length - m; i++)
      {  //left near-boundary and interior points
        double[] kernel = kernels[Math.Min(i, kernels.Length - 1)];
        double sum = 0;
        for (int j = 0; j < kernel.Length; j++)
          sum += kernel[j] * data[Math.Max(i - m, 0) + j];
        result[i] = sum;
      }
      for (int i = data.Length - m; i < data.Length; i++)
      { //near boundary points at the right
        double[] kernel = kernels[data.Length - 1 - i];
        double sum = 0;
        for (int j = 0; j < kernel.Length; j++)
          sum += kernel[j] * data[data.Length - 1 - j];
        result[i] = sum;
      }
      return result;
    }

    /**  */
    /// <summary>
    /// Creates Savitzky-Golay kernels with weights for near-boundary points and interior points.
    /// Returns as array element [0] the kernel for the first data point (where no earlier points are present),
    /// as [1] the kernel where one earlier data point is present, and the last array element [m] is the kernel to apply in the interior.
    /// For the last points of the data series, the near-edge kernels [0, …, m - 1] must be reversed.
    /// For near-edge kernels, the first array element is the kernel element that should be applied to the edge point.
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="degree">The polynomial degree.</param>
    /// <param name="m">The kernel half-width.</param>
    /// <returns>The kernels for near-boundary points and for interior points.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="m"/> is too small for <paramref name="degree"/>.</exception>
    private static double[][] MakeKernels(WeightType weightType, int degree, int m)
    {
      if (degree > 2 * m)
        throw new ArgumentOutOfRangeException("Kernel half-width m too low for degree, min m=" + (degree + 1) / 2);
      double[][] kernels = new double[m + 1][];
      for (int pLeft = 0; pLeft <= m; pLeft++)               //number of points left of the current point
        kernels[pLeft] = MakeLeftKernel(weightType, degree, m, pLeft);
      return kernels;
    }

    /// <summary>
    /// Creates one weighted Savitzky-Golay kernel for a point near the left boundary.
    /// <paramref name="pLeft"/> is the number of data points to the left; if <paramref name="pLeft"/> equals <paramref name="m"/>,
    /// the kernel for interior points is created.
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="degree">The polynomial degree.</param>
    /// <param name="m">The kernel half-width.</param>
    /// <param name="pLeft">Number of data points to the left.</param>
    /// <returns>The kernel coefficients.</returns>
    private static double[] MakeLeftKernel(WeightType weightType, int degree, int m, int pLeft)
    {
      double scale = WeightFunctionScale((double)(m - pLeft) / m, weightType);
      int pRight = (int)Math.Floor((m + 1) / scale);     //number of points right of the current point
      if (pRight + pLeft > 2 * m) pRight = 2 * m - pLeft;
      double[] weights = new double[pLeft + pRight + 1];      //the weight function
      for (int i = 0; i <= pRight; i++)
      {                     //we have more points at the right side
        double weight = WeightFunction(weightType, i * scale / (m + 1));
        weights[pLeft + i] = weight;
        if (i != 0 && i <= pLeft)
          weights[pLeft - i] = weight;
      }
      double[][] polynomials = new double[degree + 1][]; //polynomials of order 0 to 'degree'
      for (int i = 0; i < polynomials.Length; i++)
        polynomials[i] = new double[pLeft + pRight + 1];
      for (int i = 0; i < polynomials[0].Length; i++)
        polynomials[0][i] = 1;
      Normalize(polynomials[0], weights);
      for (int o = 1; o <= degree; o++)
      {
        for (int i = 0; i < pLeft + pRight + 1; i++)            //higher powers of x
          polynomials[o][i] = polynomials[o - 1][i] * (i - pLeft);
      }
      //Modified Gram-Schmidt Orthonormalization
      for (int o = 1; o <= degree; o++)
      {
        for (int u = 0; u < o; u++)
        {
          double dotProd = DotProduct(polynomials[u], polynomials[o], weights);
          for (int i = 0; i < pLeft + pRight + 1; i++)
            polynomials[o][i] -= polynomials[u][i] * dotProd;  //subtract projection on u
        }
        Normalize(polynomials[o], weights);
      }
      //Finally the kernel: sum up contributions from each basis polynomial
      double[] kernel = new double[pLeft + pRight + 1];
      for (int o = 0; o <= degree; o++)
      {
        for (int i = 0; i < pLeft + pRight + 1; i++)
          kernel[i] += polynomials[o][i] * polynomials[o][pLeft] * weights[i];
      }
      return kernel;
    }

    /// <summary>
    /// Returns the weight function value, where <c>x = 0</c> is the center and the weight becomes zero at <c>|x| = 1</c>.
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="x">The x value.</param>
    /// <returns>The weight value at <paramref name="x"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="weightType"/> is not defined.</exception>
    private static double WeightFunction(WeightType weightType, double x)
    {
      const double decay = 2; //for GAUSS only
      if (x <= -0.999999999999 || x >= 0.999999999999) return 0;
      if (weightType == WeightType.NONE)
        return 1.0;
      else if (weightType == WeightType.GAUSS2)
        return Math.Exp(-Sqr(x) * decay) + Math.Exp(-Sqr(x - 2) * decay) + Math.Exp(-Sqr(x + 2) * decay) -
                2 * Math.Exp(-decay) - Math.Exp(-9 * decay); //Gaussian-like alpha=2
      else if (weightType == WeightType.HANN)
        return Sqr(Math.Cos(0.5 * Math.PI * x)); //Hann
      else if (weightType == WeightType.HANNSQR)
        return Sqr(Sqr(Math.Cos(0.5 * Math.PI * x))); //Hann-squared
      else if (weightType == WeightType.HANNCUBE)
        return Sqr(Sqr(Math.Cos(0.5 * Math.PI * x))) * Sqr(Math.Cos(0.5 * Math.PI * x)); //Hann-cube
      else throw new ArgumentException("Undefined weight function: " + weightType);
    }

    /// <summary>
    /// Returns the scale factor for <c>x</c> for the weight function at near-edge points.
    /// </summary>
    /// <param name="missingFrac">The fraction of points outside the data range over the half-width <c>m</c> of the normal kernel.</param>
    /// <param name="weightType">Type of the weight.</param>
    /// <returns>The scale factor for <c>x</c> for the weight function at near-edge points.</returns>
    private static double WeightFunctionScale(double missingFrac, WeightType weightType)
    {
      double[] coeffs = weightScaleCoeffs[(int)weightType];
      return missingFrac <= 0 ? 1 : 1 - coeffs[0] / (1 + coeffs[1] * Math.Pow(missingFrac, coeffs[2]));
    }

    /// <summary>
    /// Calculates the weighted dot product of two vectors.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <param name="weights">The weights.</param>
    /// <returns>The weighted dot product of <paramref name="vector1"/> and <paramref name="vector2"/>.</returns>
    private static double DotProduct(double[] vector1, double[] vector2, double[] weights)
    {
      double sum = 0;
      for (int i = 0; i < vector1.Length; i++)
        sum += vector1[i] * vector2[i] * weights[i];
      return sum;
    }

    /// <summary>
    /// Normalizes a vector to length 1, using the given weights.
    /// </summary>
    /// <param name="vector">The vector. It is modified (normalized) to length 1.</param>
    /// <param name="weights">The weights.</param>
    private static void Normalize(double[] vector, double[] weights)
    {
      double dotProd = DotProduct(vector, vector, weights);
      for (int i = 0; i < vector.Length; i++)
        vector[i] *= 1.0 / Math.Sqrt(dotProd);
    }

    /// <summary>
    /// Returns the square of <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>The square of <paramref name="x"/>.</returns>
    private static double Sqr(double x)
    { return x * x; }
  }

}
