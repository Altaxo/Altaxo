#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//
//    This file is based on a translation from the Python SciPy package:
//    Converted to C# from https://github.com/scipy/scipy/blob/main/scipy/special/_digamma.pxd (Author: Josh Wilson)
//    The license of the Python code is the BSD-3 Clause License:
//    Copyright(c) 2001 - 2002 Enthought, Inc. 2003 - 2022, SciPy Developers.  All rights reserved.
//
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger (of the C# translation)
//    This file is released under the same license as the original file from the SciPy package (BSD-3 Clause Licence).
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright


using System;

using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides the Digamma function (the logarithmic derivative of the Gamma function) for complex arguments.
  /// </summary>
  /// <remarks>
  /// This implementation is based on a translation from SciPy.
  /// </remarks>
  public class SpecialDigamma
  {
    //  Use the asymptotic series for z away from the negative real axis
    //  with abs(z) > smallabsz.
    private const double smallabsz = 16;

    // Use the reflection principle for z with z.real < 0 that are within
    // smallimag of the negative real axis.
    private const double smallimag = 6;

    // Relative tolerance for series
    private const double tol = 2.220446092504131e-16;

    // All of the following were computed with mpmath
    //  Location of the positive root
    private const double posroot = 1.4616321449683623;

    //  Value of the positive root
    private const double posrootval = -9.2412655217294275e-17;

    // Location of the negative root
    private const double negroot = -0.504083008264455409;

    // Value of the negative root
    private const double negrootval = 7.2897639029768949e-17;



    /// <summary>
    /// Digamma function of complex argument z.
    /// </summary>
    /// <param name="z">The argument z.</param>
    /// <returns>The Digamma function of z.</returns>
    public static Complex64 Digamma(Complex64 z)
    {
      /*
      Compute the digamma function for complex arguments. The strategy
      is:
      - Around the two zeros closest to the origin (posroot and negroot)
      use a Taylor series with precomputed zero order coefficient.
      - If close to the origin, use a recurrence relation to step away
      from the origin.
      - If close to the negative real axis, use the reflection formula
      to move to the right halfplane.
      - If |z| is large (> 16), use the asymptotic series.
      - If |z| is small, use a recurrence relation to make |z| large
      enough to use the asymptotic series.
      */
      int n;
      double absz = Complex64.Abs(z);
      Complex64 res = 0;
      Complex64 init;

      if (z.Real <= 0 && Math.Ceiling(z.Real) == z)
      {
        //# Poles
        // sf_error.error("digamma", sf_error.SINGULAR, NULL)
        return new Complex64(double.NaN, double.NaN);
      }
      else if (Complex64.Abs(z - negroot) < 0.3)
      {
        // First negative root
        return zeta_series(z, negroot, negrootval);
      }

      if (z.Real < 0 && Math.Abs(z.Imaginary) < smallabsz)
      {
        // Reflection formula for digamma. See
        //
        // https://dlmf.nist.gov/5.5#E4
        //
        res -= Math.PI * Complex64.Cos(Math.PI * z) / Complex64.Sin(Math.PI * z);
        z = 1 - z;
        absz = Complex64.Abs(z);
      }

      if (absz < 0.5)
      {
        // Use one step of the recurrence relation to step away from
        // the pole.
        res -= 1 / z;
        z += 1;
        absz = Complex64.Abs(z);
      }

      if (Complex64.Abs(z - posroot) < 0.5)
      {
        res += zeta_series(z, posroot, posrootval);
      }
      else if (absz > smallabsz)
      {
        res += asymptotic_series(z);
      }
      else if (z.Real >= 0)
      {
        n = (int)(smallabsz - absz) + 1;
        init = asymptotic_series(z + n);
        res += backward_recurrence(z + n, init, n);
      }
      else
      {
        // z.real < 0, absz < smallabsz, and z.imag > smallimag
        n = (int)(smallabsz - absz) - 1;
        init = asymptotic_series(z - n);
        res += forward_recurrence(z - n, init, n);
      }
      return res;
    }

    /// <summary>
    /// Computes digamma(z + n) from digamma(z) using the recurrence relation
    /// <c>digamma(z + 1) = digamma(z) + 1/z</c>.
    /// </summary>
    /// <param name="z">The value z.</param>
    /// <param name="psiz">The value digamma(z).</param>
    /// <param name="n">The non-negative integer step count.</param>
    /// <returns>The value digamma(z + n).</returns>
    /// <remarks>See <see href="https://dlmf.nist.gov/5.5#E2"/>.</remarks>
    public static Complex64 forward_recurrence(Complex64 z, Complex64 psiz, int n)
    {
      var res = psiz;
      for (int k = 0; k < n; ++k)
      {
        res += 1 / (z + k);
      }
      return res;
    }

    /// <summary>
    /// Computes digamma(z - n) from digamma(z) using the recurrence relation
    /// <c>digamma(z - 1) = digamma(z) - 1/(z - 1)</c>.
    /// </summary>
    /// <param name="z">The value z.</param>
    /// <param name="psiz">The value digamma(z).</param>
    /// <param name="n">The non-negative integer step count.</param>
    /// <returns>The value digamma(z - n).</returns>
    public static Complex64 backward_recurrence(Complex64 z, Complex64 psiz, int n)
    {
      var res = psiz;
      for (int k = 1; k <= n; ++k)
      {
        res -= 1 / (z - k);
      }
      return res;
    }

    /// <summary>
    /// Evaluates digamma(z) using an asymptotic series expansion.
    /// </summary>
    /// <param name="z">The complex argument.</param>
    /// <returns>The digamma function value at <paramref name="z"/>.</returns>
    /// <remarks>
    /// See <see href="https://dlmf.nist.gov/5.11#E2"/>.
    /// </remarks>
    public static Complex64 asymptotic_series(Complex64 z)
    {
      // Evaluate digamma using an asymptotic series. See
      // https://dlmf.nist.gov/5.11#E2


      //  # The Bernoulli numbers B_2k for 1 <= k <= 16.
      double[] bernoulli2k = new double[] {
        0.166666666666666667, -0.0333333333333333333,
        0.0238095238095238095, -0.0333333333333333333,
        0.0757575757575757576, -0.253113553113553114,
        1.16666666666666667, -7.09215686274509804,
        54.9711779448621554, -529.124242424242424,
        6192.12318840579710, -86580.2531135531136,
        1425517.16666666667, -27298231.0678160920,
        601580873.900642368, -15116315767.0921569 };

      var rzz = 1 / z / z;
      Complex64 zfac = 1;
      Complex64 term;
      Complex64 res;

      res = Complex64.Log(z) - 0.5 / z;
      for (int k = 1; k <= 17; ++k)
      {
        zfac *= rzz;
        term = -bernoulli2k[k - 1] * zfac / (2 * k);
        res += term;
        if (Complex64.Abs(term) < tol * Complex64.Abs(res))
          break;
      }
      return res;
    }

    /// <summary>
    /// Evaluates digamma(z) near a simple root using a Taylor series whose coefficients are expressed via the Hurwitz zeta function.
    /// </summary>
    /// <param name="z">The complex argument.</param>
    /// <param name="root">An approximation of the (real) root location around which to expand.</param>
    /// <param name="rootval">The precomputed value of digamma at <paramref name="root"/> corresponding to the chosen floating-point approximation.</param>
    /// <returns>The digamma function value at <paramref name="z"/>.</returns>
    public static Complex64 zeta_series(Complex64 z, double root, double rootval)
    {
      /*
      The coefficients of the Taylor series for digamma at any point can
      be expressed in terms of the Hurwitz zeta function. If we
      precompute the floating point number closest to a zero and the 0th
      order Taylor coefficient at that point then we can compute higher
      order coefficients without loss of accuracy using zeta (the zeros
      are simple) and maintain high-order accuracy around the zeros.
      */

      Complex64 res = rootval;
      Complex64 coeff = -1;

      z = z - root;
      for (int n = 1; n <= 100; ++n)
      {
        coeff *= -z;
        var term = coeff * zeta(n + 1, root);
        res += term;
        if (Complex64.Abs(term) < tol * Complex64.Abs(res))
          break;
      }
      return res;
    }

    /// <summary>
    /// Computes the Hurwitz zeta function c6(s, z) = a3_{k=0}^{c0} 1/(k+z)^s for integer <paramref name="n"/> = s.
    /// </summary>
    /// <param name="n">The integer parameter s.</param>
    /// <param name="z">The real parameter z.</param>
    /// <returns>The Hurwitz zeta function value.</returns>
    /// <exception cref="NotImplementedException">Always thrown; this method is currently a stub.</exception>
    public static double zeta(int n, double z)
    {
      throw new NotImplementedException("Needs to be implemented");
    }

  }
}
