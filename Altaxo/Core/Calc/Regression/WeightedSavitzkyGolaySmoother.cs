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
  /// A simple C# implementation of Savitzky-Golay smoothing with
  /// weights for a better suppression of the stopband than traditional
  /// Savitzky-Golay(SG). It can also do traditional SG smoothing with
  /// weights = NONE.
  /// </summary>
  /// <remarks>
  /// Reference: Michael Schmid, David Rath, and Ulrike Diebold, 'Why and how Savitzky-Golay filters should be replaced', ACS Measurement Science Au 2022 2 (2), 185-196 DOI: 10.1021/acsmeasuresciau.1c00054
  /// </remarks>
  public class WeightedSavitzkyGolaySmoother
  {
    /// <summary>
    /// The type of weight function to use. The default is NONE, which results in traditional SG smoothing.
    /// </summary>
    public enum WeightType
    {
      /// <summary>
      /// Weight type 'none', this results in traditional SG smoothing
      /// </summary>
      NONE = 0,
      /// <summary>
      /// Weight type for a modified Gaussian with alpha=2, as described in the paper by Schmid and Diebold
      /// </summary>
      GAUSS2 = 1,
      /// <summary>
      /// Weight type for a Hann window function (also known as raised cosine or cosine-square)
      /// </summary>
      HANN = 2,
      /// <summary>
      /// Weight type for a Hann-square window function (also known as cos^4)
      /// </summary>
      HANNSQR = 3,
      /// <summary>
      /// Weight type for a Hann-cube window function (also known as cos^6)
      /// </summary>
      HANNCUBE = 4
    };

    /// <summary>
    /// Coefficients a,b,c for x-scale of near-end kernel functions of the SGW filters, for equation scale s = 1-a/(1+b* Math.pow(x, c))
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
    ///  Creates a WeightedSavitzkyGolaySmoother with a given weight
    /// function, given degree and kernel halfwidth.This
    /// constructor is useful for repeated smoothing operations with
    /// the same parameters and data sets of the same length.
    /// Otherwise the static <see cref="Smooth(double[], double[], int, int, int)"/>
    /// method is more convenient.
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="degree">Degree ofthe polynomial fit.</param>
    /// <param name="m">The halfwidth of the kernel.</param>
    public WeightedSavitzkyGolaySmoother(WeightType weightType, int degree, int m)
    {
      kernels = MakeKernels(weightType, degree, m);
    }

    /// <summary>
    /// Smooths the data with the parameters passed with the constructor.
    /// This function is implemented only for data arrays that have least 2m+1 elements,
    /// where m is the kernel halfwidth.    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">The output array; may be null. If <paramref name="result"/> is supplied
    /// and has the correct size, it is used for the output. <paramref name="result"/>
    /// may be null or the input array <paramref name="data"/>; in the latter case
    /// the input is overwritten.</param>
    /// <returns>The smoothed data. If <paramref name="result"/> is non-null, has the correct
    /// size, and is not the input array, this is the <paramref name="result"/> array.</returns>
    public double[] Smooth(double[] data, double[]? result)
    {
      result = Convolve(data, result, kernels);
      return result;
    }

    /// <summary>
    /// Smooths the data with the parameters passed with a given weight
    /// function, given degree and kernel halfwidth m.
    /// This function is implemented only for data arrays that have least 2m+1 elements.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="result">The output array; may be null. If <paramref name="result"/> is supplied, has the correct size and is not the input, it is used for the output.</param>
    /// <param name="weightType">Type of the Weight function, NONE for traditional SG, HANNSQR for the filters described in the paper</param>
    /// <param name="degree">Degree of the polynomial fit.</param>
    /// <param name="m">Halfwidth of the kernel.</param>
    /// <returns>The smoothed data. If <paramref name="result"/> is non-null, has the correct size, and is not the input array, this is the <code>result</code> array.</returns>
    public static double[] Smooth(double[] data, double[]? result, WeightType weightType, int degree, int m)
    {
      WeightedSavitzkyGolaySmoother smoother = new WeightedSavitzkyGolaySmoother(weightType, degree, m);
      return smoother.Smooth(data, result);
    }

    /// <summary>
    /// Smooths the data in a way comparable to a traditional Savitzky-Golay
    /// filter with the given parameters <paramref name="degree"/> and <paramref name="m"/>,
    /// but with Hann-square weights, resulting substantially better noise rejection.
    /// This function is implemented only for data arrays that have least as many
    /// elements as the SGW kernel.This is more than 2m+1, but never more than 4m+1.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="degree">The degree of the polynomial fit used in the SG(W) filter.</param>
    /// <param name="m">The half-width of the SG kernel. The kernel size of the SG filter, i.e.the number of points for fitting the polynomial is <code>2*m + 1</code>.</param>
    /// <returns>The smoothed data.</returns>
    public static double[] SmoothLikeSG(double[] data, int degree, int m)
    {
      double bandwidth = SavitzkyGolayBandwidth(degree, m);
      int mSGW = BandwidthToHalfwidth(degree, bandwidth);
      return Smooth(data, null, WeightType.HANNSQR, degree, mSGW);
    }

    /// <summary>
    /// Calculates the kernel halfwidth m for a given band width, i.e.,
    /// the frequency where the response decreases to -3 dB, corresponding to 1/sqrt(2),
    /// for a SGW filter with HANNSQR weights.
    /// </summary>
    /// <param name="degree">The degree of the polynomial fit used in the SGW filter.</param>
    /// <param name="bandwidth">The desired band width, with respect to the sampling frequency. The value of <paramref name="bandwidth"/> must be less than 0.5 (the Nyquist frequency).</param>
    /// <returns>The kernel halfwidth m for this band width.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Invalid bandwidth value: " + bandwidth</exception>
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
    /// <param name="m">The half-width of the SG kernel. The kernel size of the SG
    /// filter, i.e.the number of points for fitting the polynomial
    /// is <code>2*m + 1</code>.</param>
    /// <returns>The -3 dB-bandwidth of the SG filter, i.e. the frequency where the
    /// response is 1/sqrt(2). The sampling frequency is defined as f = 1.
    /// For <paramref name="degree"/> up to 10, the accuracy is typically much
    /// better than 1%; higher errors occur only for the lowest
    /// <paramref name="m"/> values where the SG filter is defined
    /// (worst case: 4% error at <code>degree = 10, m = 6 </code>).     
    ///</returns>
    public static double SavitzkyGolayBandwidth(int degree, int m)
    {
      return 1.0 / (6.352 * (m + 0.5) / (degree + 1.379) - (0.513 + 0.316 * degree) / (m + 0.5));
    }

    /// <summary>
    /// Convolves the data, for each point using the appropriate kernel for interior
    /// or near-boundary points from the kernels supplied.
    /// This function is implemented only for data arrays that have least 2m+1 elements.
    /// The output array may be supplied; if null or unsuitable a new output array is created.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="result">The output.</param>
    /// <param name="kernels">The kernels.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Data array too short; min length: " + (2 * m + 1)</exception>
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
    /// Returns as array element[0] the kernel for the first data point, where no
    /// earlier points are present, as [1] the kernel where one earlier data point is present,
    /// and the last array element[m] is the kernel to apply in the interior.
    /// For the readonly points of the data series, the near-edge kernels[0, ... m - 1] must be reversed.
    /// For the near-edge kernels, the first array element is the kernel element
    /// that should be applied to the edge point.
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="degree">The degree.</param>
    /// <param name="m">The m.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Kernel half-width m too low for degree, min m=" + (degree + 1) / 2</exception>
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
    /// Creates one Savitzky-Golay kernel with weights, for a given point near the left boundary,
    ///  where pLeft is the number of data points to the left, or the kernel for interior points
    ///  if pLeft = m.
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="degree">The degree.</param>
    /// <param name="m">The m.</param>
    /// <param name="pLeft">Number of data points to the left.</param>
    /// <returns></returns>
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
    ///The weight function of the SGW, where x=0 is the center, and the weight function becomes zero at x=1
    /// </summary>
    /// <param name="weightType">Type of the weight.</param>
    /// <param name="x">The x value.</param>
    /// <returns>The weight value at <paramref name="x"/>.</returns>
    /// <exception cref="System.ArgumentException">Undefined weight function: " + weightType</exception>
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
    /// Returns the scale factor for x for the weight function at near-edge points.
    /// </summary>
    /// <param name="missingFrac">The fraction of points that are outside the data range over the half-width m of the 'normal' kernel</param>
    /// <param name="weightType">Type of the weight.</param>
    /// <returns>The scale factor for x for the weight function at near-edge points.</returns>
    private static double WeightFunctionScale(double missingFrac, WeightType weightType)
    {
      double[] coeffs = weightScaleCoeffs[(int)weightType];
      return missingFrac <= 0 ? 1 : 1 - coeffs[0] / (1 + coeffs[1] * Math.Pow(missingFrac, coeffs[2]));
    }


    /// <summary>
    /// Dot product of two vectors with weights
    /// </summary>
    /// <param name="vector1">The vector1.</param>
    /// <param name="vector2">The vector2.</param>
    /// <param name="weights">The weights.</param>
    /// <returns>The dot product of two vectors with weights.</returns>
    private static double DotProduct(double[] vector1, double[] vector2, double[] weights)
    {
      double sum = 0;
      for (int i = 0; i < vector1.Length; i++)
        sum += vector1[i] * vector2[i] * weights[i];
      return sum;
    }

    /// <summary>
    /// Normalizes a vector to length=1 (with the given weights)
    /// </summary>
    /// <param name="vector">The vector. Is modified (normalized) to length 1.</param>
    /// <param name="weights">The weights.</param>
    private static void Normalize(double[] vector, double[] weights)
    {
      double dotProd = DotProduct(vector, vector, weights);
      for (int i = 0; i < vector.Length; i++)
        vector[i] *= 1.0 / Math.Sqrt(dotProd);
    }

    /// <summary>
    /// Calculates the square of <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The square of <paramref name="x"/>.</returns>
    private static double Sqr(double x)
    { return x * x; }
  }

}
