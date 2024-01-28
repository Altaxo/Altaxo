using System.Collections.Generic;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Interface to options for creation of an <see cref="IComplexInterpolationFunction"/>.
  /// </summary>
  public interface IComplexInterpolation
  {
    /// <summary>
    /// Sets the interpolation data by providing values for x and y. Both vectors must be of equal length.
    /// </summary>
    /// <param name="xvec">Vector of x (independent) data.</param>
    /// <param name="yvec">Vector of y (dependent) data.</param>
    /// <param name="yStdDev">Vector of the standard deviation of y (optional, only used for some functions).</param>
    /// <returns>A <see cref="IComplexInterpolationFunction"/> object.</returns>
    IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<Complex64> yvec, IReadOnlyList<Complex64>? yStdDev = null);

    /// <summary>
    /// Sets the interpolation data by providing values for x and y. Both vectors must be of equal length.
    /// </summary>
    /// <param name="xvec">Vector of x (independent) data.</param>
    /// <param name="yreal">Vector of the real part of y (dependent) data.</param>
    /// <param name="yimaginary">Vector of the imaginary part of y (dependent) data.</param>
    /// <returns>A <see cref="IComplexInterpolationFunction"/> object.</returns>
    IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yreal, IReadOnlyList<double> yimaginary);
  }

  /// <summary>
  /// Gives an interpolation function, i.e. for every given x, there is exactly one corresponding y value.
  /// </summary>
  public interface IComplexInterpolationFunction
  {
    /// <summary>
    /// Returns the y value in dependence of a given x value.
    /// </summary>
    /// <param name="x">The x value (value of the independent variable).</param>
    /// <returns>The y value at the given x value.</returns>
    Complex64 GetYOfX(double x);
  }
}
