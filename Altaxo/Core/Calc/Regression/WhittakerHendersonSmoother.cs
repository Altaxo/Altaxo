/*
 *  Copyright notice
 *  This code is licensed under GNU General Public License (GPLv3).
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
  /// A simple C# implementation of Whittaker-Henderson smoothing for data at equally spaced points,
  /// popularized by P. H. C. Eilers in <c>"A Perfect Smoother"</c>, Anal. Chem. 75, 3631 (2003).
  /// <code>
  /// It minimizes
  /// sum(f - y)^2 + sum(lambda* f'(p))
  /// where y are the data, f are the smoothed data, and f'(p) is the p-th
  /// derivative of the smoothed function evaluated numerically. In other
  /// words, the filter imposes a penalty on the p-th derivative of the
  /// data, which is taken as a measure of non-smoothness.
  /// Smoothing increases with increasing value of lambda.
  /// 
  /// The current implementation works up to p = 5; usually one should use
  /// p = 2 or 3.
  /// 
  /// For points far from the boundaries of the data series, the frequency
  /// response of the smoother is given by
  /// 
  ///    1/(1+lambda* (2-2* cos(omega))^2p);
  ///  
  /// where n is the order of the penalized derivative and
  /// omega = 2 * pi * f / fs, with fs being the sampling frequency
  /// (reciprocal of the distance between the data points).
  /// 
  /// Note that strong smoothing leads to numerical noise (which is smoothed
  /// similarly to the input data, thus not obvious in the output).
  /// For lambda = 1e9, the noise is about 1e-6 times the magnitude of the
  /// data. Since higher p values require a higher value of lambda for
  /// the same extent of smoothing (the same bandwidth), numerical noise
  /// is increasingly bothersome for large p, but not for p &lt;= 2.
  /// </code>
  /// </summary>
  /// <remarks>
  /// Reference: Michael Schmid, David Rath, and Ulrike Diebold, <c>"Why and how Savitzky-Golay filters should be replaced"</c>,
  /// ACS Measurement Science Au 2022 2 (2), 185–196. DOI: 10.1021/acsmeasuresciau.1c00054.
  /// <code>
  /// Implementation notes:
  /// 
  /// Storage of symmetric or triangular band-diagonal matrices:
  /// For a symmetric band-diagonal matrix with bandwidth 2 m - 1
  /// we store only the lower right side in b:
  /// 
  /// b(0, i)   are diagonal elements a(i, i)
  /// b(1, i)   are elements a(i+1, i)
  ///     ...
  /// b(m-1, i) are bottommost (leftmost) elements a(i+m-1, i)
  /// 
  /// where i runs from 0 to n-1 for b(0, i) and 0 to n - m for the
  /// further elements at a distance of m from the diagonal.
  /// 
  /// A lower triangular band matrix is stored exactly the same way.
  /// </code>
  /// </remarks>
  public class WhittakerHendersonSmoother
  {
    /// <summary>
    /// The maximum supported penalty derivative order (<c>p</c>).
    /// </summary>
    public const int MAX_ORDER = 5;

    /// <summary>
    /// Coefficients for numerical differentiation for <c>p = 1</c> to <see cref="MAX_ORDER"/>.
    /// </summary>
    private readonly static double[][] DIFF_COEFF = [
                [-1,  1],               // penalty on 1st derivative
                [1, -2,  1],
                [-1,  3, -3, 1],
                [1, -4,  6, -4,  1],
                [-1,  5,-10, 10, -5,  1] // 5th derivative. 5 = MAX_ORDER
            ];

    /// <summary>
    /// Coefficients for converting noise gain to <c>lambda</c>.
    /// </summary>
    private readonly static double[] LAMBDA_FOR_NOISE_GAIN = [0.06284, 0.005010, 0.0004660, 4.520e-05, 4.467e-06];

    /// <summary>
    /// The Cholesky-decomposed triangular matrix, stored in band form (see remarks).
    /// </summary>
    private double[][] matrix;

    /// <summary>
    /// Creates a <see cref="WhittakerHendersonSmoother"/> for data of a given length and with a given penalty order
    /// and smoothing parameter <paramref name="lambda"/>.
    /// This constructor is useful for repeated smoothing operations with the same parameters and data sets of the same length.
    /// Otherwise, the static <see cref="Smooth(double[], int, double)"/> method is more convenient.
    /// </summary>
    /// <param name="length">Number of data points in the data set that will be smoothed.</param>
    /// <param name="order">Order of the derivative that will be penalized (typically 2 or 3).</param>
    /// <param name="lambda">Smoothing parameter; see <see cref="BandwidthToLambda(int, double)"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="order"/> is outside the supported range, or if <paramref name="length"/> is too small
    /// for the chosen <paramref name="order"/>.
    /// </exception>
    public WhittakerHendersonSmoother(int length, int order, double lambda)
    {
      matrix = MakeDprimeD(order, length);
      TimesLambdaPlusIdent(matrix, lambda);
      CholeskyL(matrix);
    }

    /// <summary>
    /// Smooths the data with the parameters passed to the constructor.
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
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the length of <paramref name="data"/> does not match the configured length.</exception>
    public double[] Smooth(double[] data, double[]? result)
    {
      if (data.Length != matrix[0].Length)
        throw new ArgumentOutOfRangeException("Data length mismatch, "
                + data.Length + " vs. " + matrix[0].Length);
      result = Solve(matrix, data, result);
      return result;
    }

    /// <summary>
    /// Smooths the data with the given penalty order and smoothing parameter <paramref name="lambda"/>.
    /// When smoothing multiple data sets with the same length, using the constructor and then
    /// <see cref="Smooth(double[], double[])"/> will be more efficient.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="order">Order of the derivative that will be penalized (typically 2 or 3).</param>
    /// <param name="lambda">Smoothing parameter; should not be excessively high (see <see cref="BandwidthToLambda(int, double)"/>).</param>
    /// <returns>The smoothed data.</returns>
    public static double[] Smooth(double[] data, int order, double lambda)
    {
      WhittakerHendersonSmoother smoother = new WhittakerHendersonSmoother(data.Length, order, lambda);
      return smoother.Smooth(data, null);
    }

    /// <summary>
    /// Smooths the data in a way comparable to a traditional Savitzky-Golay filter with the given parameters
    /// <paramref name="degree"/> and <paramref name="m"/>.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="degree">The degree of the polynomial fit used in the SG filter.</param>
    /// <param name="m">
    /// The half-width of the SG kernel. The kernel size of the SG filter (i.e. the number of points for fitting the polynomial)
    /// is <c>2*m + 1</c>.
    /// Note that very strong smoothing will lead to numerical noise; recommended limits for <paramref name="m"/> are 700, 190, 100,
    /// and 75 for Savitzky-Golay degrees 2, 4, 6, and 8, respectively.
    /// </param>
    /// <returns>The smoothed data.</returns>
    public static double[] SmoothLikeSavitzkyGolay(double[] data, int degree, int m)
    {
      int order = degree / 2 + 1;
      double bandwidth = SavitzkyGolayBandwidth(degree, m);
      double lambda = BandwidthToLambda(order, bandwidth);
      return Smooth(data, order, lambda);
    }

    /// <summary>
    /// Calculates the smoothing parameter <c>lambda</c> for a given penalty derivative order and desired bandwidth
    /// (the frequency where the response decreases to -3 dB, i.e. <c>1/sqrt(2)</c>).
    /// This bandwidth is valid for points far from the boundaries of the data.
    /// </summary>
    /// <param name="order">The order <c>p</c> of the penalty derivative (1 to <see cref="MAX_ORDER"/>; typically 2 or 3).</param>
    /// <param name="bandwidth">
    /// The desired bandwidth with respect to the sampling frequency.
    /// The value must be less than 0.5 (the Nyquist frequency).
    /// </param>
    /// <returns>
    /// The <c>lambda</c> parameter for this bandwidth.
    /// Note that very large <c>lambda</c> values lead to increased numerical noise.
    /// It is recommended to use only values below 1e9, which typically lead to relative numerical noise below 1e-6.
    /// For a given bandwidth, the <c>lambda</c> value can be reduced by choosing a lower penalty order.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bandwidth"/> is outside the valid range.</exception>
    public static double BandwidthToLambda(int order, double bandwidth)
    {
      if (bandwidth <= 0 || bandwidth >= 0.5)
        throw new ArgumentOutOfRangeException("Invalid bandwidth value: " + bandwidth);
      double omega = 2 * Math.PI * bandwidth;
      double cosTerm = 2 * (1 - Math.Cos(omega));
      double cosPower = cosTerm;
      for (int i = 1; i < order; i++)
        cosPower *= cosTerm;     //finally results in (2-2*cos(omega))^order
      double lambda = (Math.Sqrt(2) - 1) / cosPower;
      return lambda;
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
    /// Calculates an approximation of the smoothing parameter <c>lambda</c> for a given white-noise gain.
    /// This is valid for points far from the boundaries of the data; the noise gain is much higher near the boundaries.
    /// </summary>
    /// <param name="order">The order <c>p</c> of the penalty derivative (1 to <see cref="MAX_ORDER"/>; typically 2 or 3).</param>
    /// <param name="noiseGain">The factor by which white noise should be suppressed.</param>
    /// <returns>The <c>lambda</c> parameter for this noise gain.</returns>
    public static double NoiseGainToLambda(int order, double noiseGain)
    {
      double gPower = noiseGain;
      for (int i = 1; i < order; i++)
        gPower *= noiseGain;     //finally results in noiseGain^order
      double lambda = LAMBDA_FOR_NOISE_GAIN[order] / (gPower + gPower);
      return lambda;
    }

    /// <summary>
    /// Creates a symmetric band-diagonal matrix D'*D where D is the n-th
    /// derivative matrix and D' its transpose.
    /// </summary>
    /// <param name="order">The order of the derivative.</param>
    /// <param name="size">he size of the matrix created.</param>
    /// <returns>The D'*D matrix, with b[0] holding the diagonal elements and
    /// b[1] the elements below the diagonal, etc.
    /// Note that the sub-arrays of <code>b</code> do not have equal length:
    /// <code>b[n].length</code> equals <code>size - n</code>.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Invalid order or order must be less than number of points</exception>
    private static double[][] MakeDprimeD(int order, int size)
    {
      if (order < 1 || order > MAX_ORDER)
        throw new ArgumentOutOfRangeException("Invalid order " + order);
      if (size < order)
        throw new ArgumentOutOfRangeException("Order (" + order +
                ") must be less than number of points (" + size + ")");

      double[] coeffs = DIFF_COEFF[order - 1];
      double[][] result = new double[order + 1][];
      for (int d = 0; d < order + 1; d++)       //'d' is distance from diagonal
        result[d] = new double[size - d];
      for (int d = 0; d < order + 1; d++)
      {
        int len = result[d].Length;
        for (int i = 0; i < (len + 1) / 2; i++)
        {
          double sum = 0;
          for (int j = Math.Max(0, i - len + coeffs.Length - d);
                  j < i + 1 && j < coeffs.Length - d; j++)
            sum += coeffs[j] * coeffs[j + d];
          result[d][i] = sum;
          result[d][len - 1 - i] = sum;
        }
      }
      return result;
    }

    /// <summary>
    /// Modifies a symmetric band-diagonal matrix b so that the output is 1 + lambda* b where 1 is the identity matrix.
    /// </summary>
    /// <param name="b">The matrix, with b[0] holding the diagonal elements and
    /// b[1] the elements below the diagonal, etc.
    /// <paramref name="b"/> is replaced by the output(overwritten).</param>
    /// <param name="lambda">lambda The factor applied to the matrix before adding the identity matrix.</param>
    private static void TimesLambdaPlusIdent(double[][] b, double lambda)
    {
      for (int i = 0; i < b[0].Length; i++)
        b[0][i] = 1.0 + b[0][i] * lambda;  //diagonal elements with identity added
      for (int d = 1; d < b.Length; d++)
        for (int i = 0; i < b[d].Length; i++)
          b[d][i] = b[d][i] * lambda;    //off-diagonal elements
    }


    /// <summary>
    /// Cholesky decomposition of a symmetric band-diagonal matrix b. The input is replaced by the lower left trianglar matrix.
    /// </summary>
    /// <param name="b">The matrix, with b[0] holding the diagonal elements and
    /// b[1] the elements below the diagonal, etc.
    /// <paramref name="b"/> is replaced by the output(overwritten).</param>
    /// <exception cref="System.InvalidOperationException">Cholesky decomposition: Matrix is not positive definite</exception>
    private static void CholeskyL(double[][] b)
    {
      int n = b[0].Length;
      int dmax = b.Length - 1;
      for (int i = 0; i < n; i++)
      {                           //for i=0 to n-1
        for (int j = Math.Max(0, i - dmax); j <= i; j++)
        {    //for j=0 to i
          double sum = 0;
          for (int k = Math.Max(0, i - dmax); k < j; k++)
          { //for k=0 to j-1
            int dAik = i - k;  //first index in b for accessing a[i,k]
            int dAjk = j - k;
            sum += b[dAik][k] * b[dAjk][k];         //sum += a[i,k]*a[j,k]
          }
          if (i == j)
          {
            double sqrtArg = b[0][i] - sum;
            if (sqrtArg <= 0)
              throw new InvalidOperationException(
                      "Cholesky decomposition: Matrix is not positive definite");
            b[0][i] = Math.Sqrt(sqrtArg);           //a[i,i] = sqrt(a[i,i] - sum)
          }
          else
          {
            int dAij = i - j;  //first index in b for accessing a[i,j]
            b[dAij][j] = 1d / b[0][j] * (b[dAij][j] - sum);   //a[i,j] = 1/(a[j,j] * (a[i,j] - sum))
          }
        }
      }
    }


    /// <summary>
    /// Solves the equation b*y = vec for y (forward substitution)
    /// and thereafter b'*x = y, where b' is the transposed(back substitution)
    /// </summary>
    /// <param name="b">The matrix, with b[0] holding the diagonal elements and b[1] the elements below the diagonal, etc.</param>
    /// <param name="vec">The vector at the right-hand side. For data smoothing, this is the data.</param>
    /// <param name="result">The output array; may be null. If <paramref name="result"/> is supplied
    /// and has the correct size, it is used for the output. <paramref name="result"/>
    /// may be null or the input array <paramref name="vec"/>; in the latter case
    /// the input is overwritten.</param>
    /// <returns>The vector x resulting from forward and back subsitution. If <paramref name="b"/>
    /// is the result of Cholesky decomposition, this is the solution for
    /// A* x = vec. For data smoothing, the returned array holds the smoothed data.</returns>
    private static double[] Solve(double[][] b, double[] vec, double[]? result)
    {
      if (result is null || result.Length != vec.Length)
        result = new double[vec.Length];
      int n = b[0].Length;
      int dmax = b.Length - 1;
      for (int i = 0; i < n; i++)
      {
        double sum = 0;
        for (int j = Math.Max(0, i - dmax); j < i; j++)
        {
          int dAij = i - j;  //first index in b for accessing a[i,j]
          sum += b[dAij][j] * result[j];
        }
        result[i] = (vec[i] - sum) / b[0][i]; //denominator is diagonal element a[i,i]
      }
      for (int i = n - 1; i >= 0; i--)
      {
        double sum = 0;
        for (int j = i + 1; j < Math.Min(i + dmax + 1, n); j++)
        {
          int dAji = j - i;
          sum += b[dAji][i] * result[j];
        }
        result[i] = (result[i] - sum) / b[0][i];
      }
      return result;
    }
  }
}
