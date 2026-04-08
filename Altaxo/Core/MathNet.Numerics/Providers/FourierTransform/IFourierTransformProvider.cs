// <copyright file="IFourierTransformProvider.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2016 Math.NET
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

using Complex = System.Numerics.Complex;

namespace Altaxo.Calc.Providers.FourierTransform
{
  /// <summary>
  /// Specifies scaling options for Fourier transforms.
  /// </summary>
  public enum FourierTransformScaling : int
  {
    /// <summary>
    /// Apply no scaling.
    /// </summary>
    NoScaling = 0,
    /// <summary>
    /// Apply symmetric scaling.
    /// </summary>
    SymmetricScaling = 1,
    /// <summary>
    /// Apply backward scaling.
    /// </summary>
    BackwardScaling = 2,
    /// <summary>
    /// Apply forward scaling.
    /// </summary>
    ForwardScaling = 3
  }

  /// <summary>
  /// Defines a Fourier transform provider.
  /// </summary>
  public interface IFourierTransformProvider
  {
    /// <summary>
    /// Try to find out whether the provider is available, at least in principle.
    /// Verification may still fail if available, but it will certainly fail if unavailable.
    /// </summary>
    public bool IsAvailable();

    /// <summary>
    /// Initialize and verify that the provider is indeed available. If not, fall back to alternatives like the managed provider.
    /// </summary>
    public void InitializeVerify();

    /// <summary>
    /// Frees memory buffers, caches and handles allocated in or to the provider.
    /// Does not unload the provider itself, it is still usable afterwards.
    /// </summary>
    public void FreeResources();

    /// <summary>
    /// Computes the forward transform of complex single-precision samples.
    /// </summary>
    public void Forward(Complex32[] samples, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the forward transform of complex double-precision samples.
    /// </summary>
    public void Forward(Complex[] samples, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the inverse transform of complex single-precision spectrum data.
    /// </summary>
    public void Backward(Complex32[] spectrum, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the inverse transform of complex double-precision spectrum data.
    /// </summary>
    public void Backward(Complex[] spectrum, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the forward transform of real single-precision samples.
    /// </summary>
    public void ForwardReal(float[] samples, int n, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the forward transform of real double-precision samples.
    /// </summary>
    public void ForwardReal(double[] samples, int n, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the inverse transform of real single-precision spectrum data.
    /// </summary>
    public void BackwardReal(float[] spectrum, int n, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the inverse transform of real double-precision spectrum data.
    /// </summary>
    public void BackwardReal(double[] spectrum, int n, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the forward multidimensional transform of complex single-precision samples.
    /// </summary>
    public void ForwardMultidim(Complex32[] samples, int[] dimensions, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the forward multidimensional transform of complex double-precision samples.
    /// </summary>
    public void ForwardMultidim(Complex[] samples, int[] dimensions, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the inverse multidimensional transform of complex single-precision spectrum data.
    /// </summary>
    public void BackwardMultidim(Complex32[] spectrum, int[] dimensions, FourierTransformScaling scaling);

    /// <summary>
    /// Computes the inverse multidimensional transform of complex double-precision spectrum data.
    /// </summary>
    public void BackwardMultidim(Complex[] spectrum, int[] dimensions, FourierTransformScaling scaling);
  }
}
