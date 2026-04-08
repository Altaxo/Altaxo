// <copyright file="VectorExtensions.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Altaxo.Calc.LinearAlgebra
{
  using Complex64 = System.Numerics.Complex;

  /// <summary>
  /// Provides conversion helpers for vectors with common numeric element types.
  /// </summary>
  public static class VectorExtensions
  {
    /// <summary>
    /// Converts a vector to single precision.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A single-precision vector.</returns>
    public static Vector<float> ToSingle(this Vector<double> vector)
    {
      return vector.Map(x => (float)x, Zeros.AllowSkip);
    }

    /// <summary>
    /// Converts a vector to double precision.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A double-precision vector.</returns>
    public static Vector<double> ToDouble(this Vector<float> vector)
    {
      return vector.Map(x => (double)x, Zeros.AllowSkip);
    }

    /// <summary>
    /// Converts a vector to single precision complex numbers.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A complex single-precision vector.</returns>
    public static Vector<Calc.Complex32> ToComplex32(this Vector<Complex64> vector)
    {
      return vector.Map(x => new Calc.Complex32((float)x.Real, (float)x.Imaginary), Zeros.AllowSkip);
    }

    /// <summary>
    /// Converts a vector to double precision complex numbers.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A complex double-precision vector.</returns>
    public static Vector<Complex64> ToComplex(this Vector<Calc.Complex32> vector)
    {
      return vector.Map(x => new Complex64(x.Real, x.Imaginary), Zeros.AllowSkip);
    }

    /// <summary>
    /// Gets a single precision complex vector with the real parts from the given vector.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A complex single-precision vector with zero imaginary parts.</returns>
    public static Vector<Calc.Complex32> ToComplex32(this Vector<float> vector)
    {
      return vector.Map(x => new Calc.Complex32(x, 0f), Zeros.AllowSkip);
    }

    /// <summary>
    /// Gets a double precision complex vector with the real parts from the given vector.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>A complex double-precision vector with zero imaginary parts.</returns>
    public static Vector<Complex64> ToComplex(this Vector<double> vector)
    {
      return vector.Map(x => new Complex64(x, 0d), Zeros.AllowSkip);
    }

    /// <summary>
    /// Gets a real vector representing the real parts of a complex vector.
    /// </summary>
    /// <param name="vector">The complex vector.</param>
    /// <returns>A vector containing the real parts.</returns>
    public static Vector<double> Real(this Vector<Complex64> vector)
    {
      return vector.Map(x => x.Real, Zeros.AllowSkip);
    }

    /// <summary>
    /// Gets a real vector representing the real parts of a complex vector.
    /// </summary>
    /// <param name="vector">The complex vector.</param>
    /// <returns>A vector containing the real parts.</returns>
    public static Vector<float> Real(this Vector<Calc.Complex32> vector)
    {
      return vector.Map(x => x.Real, Zeros.AllowSkip);
    }

    /// <summary>
    /// Gets a real vector representing the imaginary parts of a complex vector.
    /// </summary>
    /// <param name="vector">The complex vector.</param>
    /// <returns>A vector containing the imaginary parts.</returns>
    public static Vector<double> Imaginary(this Vector<Complex64> vector)
    {
      return vector.Map(x => x.Imaginary, Zeros.AllowSkip);
    }

    /// <summary>
    /// Gets a real vector representing the imaginary parts of a complex vector.
    /// </summary>
    /// <param name="vector">The complex vector.</param>
    /// <returns>A vector containing the imaginary parts.</returns>
    public static Vector<float> Imaginary(this Vector<Calc.Complex32> vector)
    {
      return vector.Map(x => x.Imaginary, Zeros.AllowSkip);
    }
  }
}
