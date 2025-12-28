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
    /// Gets a value indicating whether this instance supports independent real and imaginary sample sets.
    /// </summary>
    /// <remarks>
    /// Only if this property returns true may you use <see cref="Interpolate(IReadOnlyList{double}, IReadOnlyList{double}, IReadOnlyList{double}, IReadOnlyList{double})"/> to provide separate x and y pairs for the real and imaginary data.
    /// </remarks>
    bool IsSupportingSeparateXForRealAndImaginaryPart { get; }

    /// <summary>
    /// Sets the interpolation data by providing values for xreal and yreal (both of the same length), as well as for ximag and yimag (both also of the same length, though the length can differ from the real pair).
    /// </summary>
    /// <remarks>
    /// This overload is only supported if <see cref="IsSupportingSeparateXForRealAndImaginaryPart"/> returns true.
    /// </remarks>
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
