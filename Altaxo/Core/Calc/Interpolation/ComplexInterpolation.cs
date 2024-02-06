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

    /// <summary>
    /// Gets a value indicating whether this instance is supporting separate pairs of xreal, yreal and ximag, yimag for the real and imaginary data.
    /// Only if true is returned, you may use <see cref="Interpolate(IReadOnlyList{double}, IReadOnlyList{double}, IReadOnlyList{double}, IReadOnlyList{double})"/>
    /// to provide separate pairs of x and y for real and imaginary part.
    /// </summary>
    bool IsSupportingSeparateXForRealAndImaginaryPart { get; }

    /// <summary>
    /// This call is only supported if <see cref="IsSupportingSeparateXForRealAndImaginaryPart"/> is returning true.
    /// Sets the interpolation data by providing values for xreal and yreal (both of same length), as well as for ximag and yimag (both also of same length, but length can be different from real pair).
    /// </summary>
    /// <param name="xreal">Vector of x (independent) data, belonging to the real part of y.</param>
    /// <param name="yreal">Vector of the real part of y (dependent) data.</param>
    /// <param name="ximaginary">Vector of x (independent) data, belonging to the imaginary part of y.</param>
    /// <param name="yimaginary">Vector of the imaginary part of y (dependent) data.</param>
    /// <returns>A <see cref="IComplexInterpolationFunction"/> object.</returns>
    IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xreal, IReadOnlyList<double> yreal, IReadOnlyList<double> ximaginary, IReadOnlyList<double> yimaginary);
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
