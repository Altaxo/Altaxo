#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#region Acknowledgements

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)
// Original MatPack-1.7.3\Source\mpcurvebase.h
//                               mpcurvebase.cc
//                               mpfcspline.h
//                               mpfcspline.cc
//                               mpaspline.h
//                               mpaspline.cc
//                               mpbspline.h
//                               mpbspline.cc
//                               mpcspline.h
//                               mpcspline.cc
//                               mprspline.h
//                               mprspline.cc
//                               mpespline.h
//                               mpespline.cc
//                               mppolyinterpol.h
//                               mppolyinterpol.cc
//                               mpratinterpol.h
//                               mpratinterpol.cc
//                               mpgcvspline.h
//                               mpgcvspline.cc

#endregion Acknowledgements

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary><para>
  /// Calculate the Fritsch-Carlson monotone cubic spline interpolation for the
  /// given abscissa vector x and ordinate vector y.
  /// All vectors must have conformant dimensions.
  /// The abscissa vector must be strictly increasing.
  /// </para>
  /// <para>
  /// The Fritsch-Carlson interpolation produces a neat monotone
  /// piecewise cubic curve, which is especially suited for the
  /// presentation of scientific data.
  /// This is the state of the art to create curves that preserve
  /// monotonicity, although it is not so well known as Akima's
  /// interpolation. The commonly used Akima interpolation doesn't
  /// produce equally pleasant results.
  /// </para>
  /// <code>
  /// Reference:
  ///    F.N.Fritsch,R.E.Carlson: Monotone Piecewise Cubic
  ///    Interpolation, SIAM J. Numer. Anal. Vol 17, No. 2,
  ///    April 1980
  ///
  /// Copyright (C) 1991-1998 by Berndt M. Gammel
  /// Translated to C# by Dirk Lellinger.
  /// </code>
  /// </summary>
  public class FritschCarlsonCubicSpline : CurveBase, IInterpolationFunction
  {
    /// <summary>
    /// First derivative estimates for each spline knot.
    /// </summary>
    protected Vector<double> y1 = CreateVector.Dense<double>(0);

    /// <summary>
    /// Quadratic coefficients of the cubic spline segments.
    /// </summary>
    protected Vector<double> y2 = CreateVector.Dense<double>(0);

    /// <summary>
    /// Cubic coefficients of the spline segments.
    /// </summary>
    protected Vector<double> y3 = CreateVector.Dense<double>(0);

    //----------------------------------------------------------------------------//
    //
    // int MpFritschCarlsonCubicSpline::Interpolate (const Vector &x, const Vector &y)
    //
    // Calculate the Fritsch-Carlson monotone cubic spline interpolation for the
    // given abscissa vector x and ordinate vector y.
    // All vectors must have conformant dimenions.
    // The abscissa vector must be strictly increasing.
    //
    // The Fritsch-Carlson interpolation produces a neat monotone
    // piecewise cubic curve, which is especially suited for the
    // presentation of scientific data.
    // This is the state of the art to create curves that preserve
    // monotonicity, although it is not so well known as Akima's
    // interpolation. The commonly used Akima interpolation doesn't
    // produce so pleasant results.
    //
    // Reference:
    //    F.N.Fritsch,R.E.Carlson: Monotone Piecewise Cubic
    //    Interpolation, SIAM J. Numer. Anal. Vol 17, No. 2,
    //    April 1980
    //
    // Copyright (C) 1991-1998 by Berndt M. Gammel
    //
    //----------------------------------------------------------------------------//

    /// <inheritdoc/>
    public override void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      // check input parameters

      if (!MatchingIndexRange(x, y))
        throw new ArgumentException("index range mismatch of vectors");

      // link original data vectors into base class
      base.x = x;
      base.y = y;

      // Empty data vectors - free auxilliary storage
      if (x.Count == 0)
      {
        y1.Clear();
        y2.Clear();
        y3.Clear();
        return; // ok
      }

      int hi = x.Count - 1;
      int len = x.Count;

      // Resize the auxilliary vectors. Note, that there is no reallocation if the
      // vector already has the appropriate dimension.
      if (y1.Count < len)
      {
        y1 = CreateVector.Dense<double>(len);
        y2 = CreateVector.Dense<double>(len);
        y3 = CreateVector.Dense<double>(len);
      }

      if (x.Count == 1)
      {
        // default derivative is 0.0
        y1[0] = y2[0] = y3[0] = 0.0;
      }
      else if (x.Count == 2)
      {
        // set derivatives for a line
        y1[0] = y1[hi] = (y[hi] - y[0]) / (x[hi] - x[0]);
        y2[0] = y2[hi] =
          y3[0] = y3[hi] = 0.0;
      }
      else
      { // three or more points
        // initial guess derivative vector
        y1[0] = deriv1(x, y, 1, -1);
        y1[hi] = deriv1(x, y, hi - 1, 1);
        for (int i = 1; i < hi; i++)
          y1[i] = deriv2(x, y, i);

        if (x.Count > 3)
        {
          // adjust derivatives at boundaries
          if (y1[0] * y1[1] < 0)
            y1[0] = 0;
          if (y1[hi] * y1[hi - 1] < 0)
            y1[hi] = 0;

          // adjustment of cubic interpolant
          fritsch(x, y, y1);
        }

        // calculate remaining spline coefficients y2(i) and y3(i)
        CubicSplineCoefficients(x, y, y1, y2, y3);
      }
    }

    /// <inheritdoc/>
    public override double GetXOfU(double u)
    {
      return u;
    }

    /// <inheritdoc/>
    public override double GetYOfU(double u)
    {
      return CubicSplineHorner(u, x, y, y1, y2, y3);
    }

    /// <inheritdoc/>
    public double GetYOfX(double u)
    {
      return CubicSplineHorner(u, x, y, y1, y2, y3);
    }

    /// <summary>
    /// Returns the first derivative of the spline at the specified abscissa value.
    /// </summary>
    /// <param name="u">The x value where the derivative should be evaluated.</param>
    /// <returns>The first derivative of y with respect to x at <paramref name="u"/>.</returns>
    public double GetY1stDerivativeOfX(double u)
    {
      return CubicSplineHorner1stDerivative(u, x, y, y1, y2, y3);
    }

    #region deriv1

    //-----------------------------------------------------------------------------//
    //
    // Initial derivatives at boundaries of data set using
    // quadratic Newton interpolation
    //
    //-----------------------------------------------------------------------------//

    /// <summary>
    /// Computes boundary derivatives using quadratic Newton interpolation.
    /// </summary>
    /// <param name="x">The abscissa vector.</param>
    /// <param name="y">The ordinate vector.</param>
    /// <param name="i">The interior index used for the stencil.</param>
    /// <param name="sgn">The sign applied to the correction term (+1 or -1).</param>
    /// <returns>The estimated derivative at the boundary point.</returns>
    private static double deriv1(IReadOnlyList<double> x, IReadOnlyList<double> y, int i, int sgn)
    {
      double di, dis, di2, his;
      int i1, i2;

      i1 = i + 1;
      i2 = i - 1;
      his = x[i1] - x[i2];
      dis = (y[i1] - y[i2]) / his;
      di = (y[i1] - y[i]) / (x[i1] - x[i]);
      di2 = (di - dis) / (x[i] - x[i2]);
      return dis + sgn * di2 * his;
    }

    #endregion deriv1

    //-----------------------------------------------------------------------------//
    //
    // Initial derivatives within data set using
    // quadratic Newton interpolation
    //
    //-----------------------------------------------------------------------------//

    /// <summary>
    /// Computes interior derivatives using quadratic Newton interpolation.
    /// </summary>
    /// <param name="x">The abscissa vector.</param>
    /// <param name="y">The ordinate vector.</param>
    /// <param name="i">The index for which to compute the derivative.</param>
    /// <returns>The estimated derivative at index <paramref name="i"/>.</returns>
    private static double deriv2(IReadOnlyList<double> x, IReadOnlyList<double> y, int i)
    {
      double di0, di1, di2, hi0;
      int i1, i2;

      i1 = i + 1;
      i2 = i - 1;
      hi0 = x[i] - x[i2];
      di0 = (y[i] - y[i2]) / hi0;
      di1 = (y[i1] - y[i]) / (x[i1] - x[i]);
      di2 = (di1 - di0) / (x[i1] - x[i2]);
      return di0 + di2 * hi0;
    }

    //-----------------------------------------------------------------------------//
    //
    // Fritsch-Carlson iteration to adjust the monotone
    // cubic interpolant. The iteration converges with cubic order.
    //
    //-----------------------------------------------------------------------------//

    /// <summary>
    /// Applies the Fritsch-Carlson monotonicity adjustment to the derivative vector.
    /// </summary>
    /// <param name="x">The abscissa vector.</param>
    /// <param name="y">The ordinate vector.</param>
    /// <param name="d">The derivative vector to adjust.</param>
    private static void fritsch(IReadOnlyList<double> x, IReadOnlyList<double> y, Vector<double> d)
    {
      int i, i1;
      bool stop;
      double d1, r2, t;

      const int max_loop = 20; // should never happen! Note, that currently it
                               // can happen when the curve is not strictly
                               // monotone. In future this case should be handled
                               // more gracefully without wasting CPU time.
      int loop = 0;

      do
      {
        stop = true;
        int hi = x.Count - 1;
        for (i = 0; i < hi; i++)
        {
          i1 = i + 1;
          d1 = (y[i1] - y[i]) / (x[i1] - x[i]);
          if (d1 == 0.0)
            d[i] = d[i1] = 0.0;
          else
          {
            t = d[i] / d1;
            r2 = t * t;
            t = d[i1] / d1;
            r2 += t * t;
            if (r2 > 9.0)
            {
              t = 3.0 / Math.Sqrt(r2);
              d[i] *= t;
              d[i1] *= t;
              stop = false;
            }
          }
        }
      } while (!stop && ++loop < max_loop);
    }
  }
}
