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
  /// <summary>
  /// Akima cubic spline interpolation for the given abscissa
  /// vector x and ordinate vector y.
  /// All vectors must have conformant dimenions.
  /// The abscissa vector must be strictly increasing.
  /// </summary>
  /// <remarks>
  /// The implementation computes local slopes and constructs a monotone piecewise
  /// cubic curve. The abscissa vector must be strictly increasing; otherwise an
  /// <see cref="ArgumentException"/> is thrown.
  /// </remarks>
  public class AkimaCubicSpline : CurveBase, IInterpolationFunction
  {
    protected Vector<double> y1 = CreateVector.Dense<double>(0);
    protected Vector<double> y2 = CreateVector.Dense<double>(0);
    protected Vector<double> y3 = CreateVector.Dense<double>(0);

    private double m(int i)
    {
      return ((y[i + 1] - y[i]) / (x[i + 1] - x[i]));
    }

    //----------------------------------------------------------------------------//
    //
    // int MpAkimaCubicSpline::Interpolate (const Vector &x, const Vector &y)
    //
    // Calculate the Akima cubic spline interpolation for the given abscissa
    // vector x and ordinate vector y.
    // All vectors must have conformant dimenions.
    // The abscissa vector must be strictly increasing.
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

      const int lo = 0, lo1 = lo + 1, lo2 = lo + 2;
      int hi = x.Count - 1, hi1 = hi - 1, hi2 = hi - 2, hi3 = hi - 3;

      // Resize the auxilliary vectors. Note, that there is no reallocation if the
      // vectors already have the appropriate dimensions.
      if (y1.Count != x.Count)
      {
        y1 = CreateVector.Dense<double>(x.Count);
        y2 = CreateVector.Dense<double>(x.Count);
        y3 = CreateVector.Dense<double>(x.Count);
      }

      if (x.Count == 1)
      {
        // default derivatives are 0.0
        y1[lo] = y2[lo] = y3[lo] = 0.0;
      }
      else if (x.Count == 2)
      {
        // set derivatives for a line
        y1[lo] = y1[hi] = (y[hi] - y[lo]) / (x[hi] - x[lo]);
        y2[lo] = y2[hi] =
          y3[lo] = y3[hi] = 0.0;
      }
      else
      { // three or more elements - do Akima interpolation
        double num, den,
          m_m1, m_m2, m_p1, m_p2,
          x_m1, x_m2, x_p1, x_p2,
          y_m1, y_m2, y_p1, y_p2;

        // short form to save some typing
        // #define m(i) ((y(i+1)-y(i)) / (x(i+1)-x(i)))

        // interpolate the missing points
        x_m1 = x[lo] + x[lo1] - x[lo2];
        y_m1 = (x[lo] - x_m1) * (m(lo1) - 2 * m(lo)) + y[lo];
        m_m1 = (y[lo] - y_m1) / (x[lo] - x_m1);

        x_m2 = 2 * x[lo] - x[lo2];
        y_m2 = (x_m1 - x_m2) * (m(lo) - 2 * m_m1) + y_m1;
        m_m2 = (y_m1 - y_m2) / (x_m1 - x_m2);

        x_p1 = x[hi] + x[hi1] - x[hi2];
        y_p1 = (2 * m(hi1) - m(hi2)) * (x_p1 - x[hi]) + y[hi];
        m_p1 = (y_p1 - y[hi]) / (x_p1 - x[hi]);

        x_p2 = 2 * x[hi] - x[hi2];
        y_p2 = (2 * m_p1 - m(hi1)) * (x_p2 - x_p1) + y_p1;
        m_p2 = (y_p2 - y_p1) / (x_p2 - x_p1);

        // i = 0
        num = Math.Abs(m(lo1) - m(lo)) * m_m1 + Math.Abs(m_m1 - m_m2) * m(lo);
        den = Math.Abs(m(lo1) - m(lo)) + Math.Abs(m_m1 - m_m2);
        y1[lo] = (den != 0.0) ? num / den : m(lo);

        // i = 1
        if (x.Count > 3)
        {
          num = Math.Abs(m(lo2) - m(lo1)) * m(lo) + Math.Abs(m(lo) - m_m1) * m(lo1);
          den = Math.Abs(m(lo2) - m(lo1)) + Math.Abs(m(lo) - m_m1);
          y1[lo1] = (den != 0.0) ? num / den : m(lo1);

          for (int i = lo2; i < hi1; i++)
          {
            double mip1 = m(i + 1),
              mi = m(i),
              mim1 = m(i - 1),
              mim2 = m(i - 2);
            num = Math.Abs(mip1 - mi) * mim1 + Math.Abs(mim1 - mim2) * mi;
            den = Math.Abs(mip1 - mi) + Math.Abs(mim1 - mim2);
            y1[i] = (den != 0.0) ? num / den : mi;
          }

          // i = n - 2
          num = Math.Abs(m_p1 - m(hi1)) * m(hi2) + Math.Abs(m(hi2) - m(hi3)) * m(hi1);
          den = Math.Abs(m_p1 - m(hi1)) + Math.Abs(m(hi2) - m(hi3));
          y1[hi1] = (den != 0.0) ? num / den : m(hi1);
        }
        else
        { // exactly three elements
          num = m(lo);
          den = (m(lo1) - num) / (x[lo2] - x[lo]);
          y1[lo1] = num + den * (x[lo1] - x[lo]);
        }

        // i = n - 1
        num = Math.Abs(m_p2 - m_p1) * m(hi1) + Math.Abs(m(hi1) - m(hi2)) * m_p1;
        den = Math.Abs(m_p2 - m_p1) + Math.Abs(m(hi1) - m(hi2));
        y1[hi] = (den != 0.0) ? num / den : m_p1;

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
    /// Returns the first derivative of the Akima cubic spline at the given x value.
    /// </summary>
    /// <param name="u">The abscissa value at which to evaluate the derivative.</param>
    /// <returns>The first derivative at <paramref name="u"/>.</returns>
    public double GetY1stDerivativeOfX(double u)
    {
      return CubicSplineHorner1stDerivative(u, x, y, y1, y2, y3);
    }
  }
}
