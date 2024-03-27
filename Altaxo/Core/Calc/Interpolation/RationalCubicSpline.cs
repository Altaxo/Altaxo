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
  /// This kind of generalized splines give much more pleasent results
  /// than cubic splines when interpolating, e.g., experimental data.
  /// A control parameter p can be used to tune the interpolation smoothly
  /// between cubic splines and a linear interpolation.
  /// But this doesn't mean smoothing of the data - the rational spline curve
  /// will still go through all data points.
  /// </summary>
  /// <remarks>
  /// <code>
  ///
  /// The basis functions for rational cubic splines are
  ///
  ///   g1 = u
  ///   g2 = t                     with   t = (x - x(i)) / (x(i+1) - x(i))
  ///   g3 = u^3 / (p*t + 1)              u = 1 - t
  ///   g4 = t^3 / (p*u + 1)
  ///
  /// A rational spline with coefficients a(i),b(i),c(i),d(i) is determined by
  ///
  ///          f(i)(x) = a(i)*g1 + b(i)*g2 + c(i)*g3 + d(i)*g4
  ///
  /// Choosing the smoothing parameter p:
  /// -----------------------------------
  ///
  /// Use the method
  ///
  ///      void MpRationalCubicSpline::SetSmoothing (double smoothing)
  ///
  /// to set the value of the smoothing paramenter. A value of p = 0
  /// for the smoothing parameter results in a standard cubic spline.
  /// A value of p with -1 &lt; p &lt; 0 results in "unsmoothing" that means
  /// overshooting oscillations. A value of p with p &gt; 0 gives increasing
  /// smoothness. p to infinity results in a linear interpolation. A value
  /// smaller or equal to -1.0 leads to an error.
  ///
  ///
  /// Choosing the boundary conditions:
  /// ---------------------------------
  ///
  /// Use the method
  ///
  ///      void MpRationalCubicSpline::SetBoundaryConditions (int boundary,
  ///                       double b1, double b2)
  ///
  /// to set the boundary conditions. The following values are possible:
  ///
  ///      Natural
  ///          natural boundaries, that means the 2nd derivatives are zero
  ///          at both boundaries. This is the default value.
  ///
  ///      FiniteDifferences
  ///          use  finite difference approximation for 1st derivatives.
  ///
  ///      Supply1stDerivative
  ///          user supplied values for 1st derivatives are given in b1 and b2
  ///          i.e. f'(x_lo) in b1
  ///               f'(x_hi) in b2
  ///
  ///      Supply2ndDerivative
  ///          user supplied values for 2nd derivatives are given in b1 and b2
  ///          i.e. f''(x_lo) in b1
  ///               f''(x_hi) in b2
  ///
  ///      Periodic
  ///          periodic boundary conditions for periodic curves or functions.
  ///          NOT YET IMPLEMENTED IN THIS VERSION.
  ///
  ///
  /// If the parameters b1,b2 are omitted the default value is 0.0.
  ///
  ///
  /// Input parameters:
  /// -----------------
  ///
  ///      Vector x(lo,hi)  The abscissa vector
  ///      Vector y(lo,hi)  The ordinata vector
  ///                       If the spline is not parametric then the
  ///                       abscissa must be strictly monotone increasing
  ///                       or decreasing!
  ///
  ///
  /// References:
  /// -----------
  ///   Dr.rer.nat. Helmuth Spaeth,
  ///   Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen,
  ///   3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
  ///
  ///
  /// </code>
  /// </remarks>
  public class RationalCubicSpline : CurveBase, IInterpolationFunction
  {
    protected BoundaryConditions boundary;
    protected double p, r1, r2;
    protected Vector<double> dx = CreateVector.Dense<double>(0);
    protected Vector<double> dy = CreateVector.Dense<double>(0);
    protected Vector<double> a = CreateVector.Dense<double>(0);
    protected Vector<double> b = CreateVector.Dense<double>(0);
    protected Vector<double> c = CreateVector.Dense<double>(0);
    protected Vector<double> d = CreateVector.Dense<double>(0);

    //-----------------------------------------------------------------------------//
    //
    // static double deriv1 (const Vector &x, const Vector &y, int i, int sgn)
    //
    // Initial derivatives at boundaries of data set using
    // quadratic Newton interpolation
    //
    //-----------------------------------------------------------------------------//

    private static double deriv1(IReadOnlyList<double> x, IReadOnlyList<double> y, int i, int sgn)
    {
      if (x.Count <= 1)
        return 0.0;
      else if (x.Count == 2)
        return (y[y.Count - 1] - y[0]) / (x[x.Count - 1] - x[0]);
      else
      {
        int i1 = i + 1,
          i2 = i - 1;
        double his = x[i1] - x[i2],
          dis = (y[i1] - y[i2]) / his,
          di = (y[i1] - y[i]) / (x[i1] - x[i]),
          di2 = (di - dis) / (x[i] - x[i2]);
        return dis + sgn * di2 * his;
      }
    }

    /// <summary>
    /// Set the value of the smoothing paramenter. A value of p = 0
    /// for the smoothing parameter results in a standard cubic spline.
    /// A value of p with -1 &lt; p &lt; 0 results in "unsmoothing" that means
    /// overshooting oscillations. A value of p with p &gt; 0 gives increasing
    /// smoothness. p to infinity results in a linear interpolation. A value
    /// smaller or equal to -1.0 leads to an error.
    /// </summary>
    public double Smoothing
    {
      get
      {
        return p;
      }
      set
      {
        if (value > -1.0)
          p = value;
        else
          throw new ArgumentException("smoothing parameter must be greater than -1.0");
      }
    }

    /// <summary>
    /// Sets the boundary conditions.
    /// </summary>
    /// <param name="bnd"> The boundary condition. See remarks for the possible values.</param>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <remarks>
    /// <code>
    ///      Natural
    ///          natural boundaries, that means the 2nd derivatives are zero
    ///          at both boundaries. This is the default value.
    ///
    ///      FiniteDifferences
    ///          use  finite difference approximation for 1st derivatives.
    ///
    ///      Supply1stDerivative
    ///          user supplied values for 1st derivatives are given in b1 and b2
    ///          i.e. f'(x_lo) in b1
    ///               f'(x_hi) in b2
    ///
    ///      Supply2ndDerivative
    ///          user supplied values for 2nd derivatives are given in b1 and b2
    ///          i.e. f''(x_lo) in b1
    ///               f''(x_hi) in b2
    ///
    ///      Periodic
    ///          periodic boundary conditions for periodic curves or functions.
    ///          NOT YET IMPLEMENTED IN THIS VERSION.
    /// </code>
    /// </remarks>
    public void SetBoundaryConditions(
      BoundaryConditions bnd,
      double b1,
      double b2)
    {
      boundary = bnd;
      r1 = b1;
      r2 = b2;
    }

    /// <summary>
    /// Gets the boundary condition and the two condition parameters.
    /// </summary>
    /// <param name="b1">First boundary condition parameter.</param>
    /// <param name="b2">Second boundary condition parameter.</param>
    /// <returns>The boundary condition.</returns>
    public BoundaryConditions GetBoundaryConditions(out double b1, out double b2)
    {
      b1 = r1;
      b2 = r2;
      return boundary;
    }

    /// <summary>
    /// Gets the boundary condition and the two condition parameters.
    /// </summary>
    /// <returns>The boundary condition.</returns>
    public BoundaryConditions GetBoundaryConditions()
    {
      return boundary;
    }

    /// <summary>
    /// Calculate difference vector dx(i) from vector x(i) and
    /// assure that x(i) is strictly monotone increasing or decreasing.
    /// Can be called with both arguments the same vector in order to
    /// do it inplace!
    /// </summary>
    /// <param name="x">Input vector.</param>
    /// <param name="dx">Output vector.</param>
    public static void Differences(IReadOnlyList<double> x, Vector<double> dx)
    {
      int sgn;
      double t;

      // get dimensions
      const int lo = 0;
      int hi = x.Count - 1;

      if (hi > lo)
      {
        if ((sgn = Math.Sign(x[lo + 1] - x[lo])) == 0)
          throw new ArgumentException("abscissa is not strictly monotone");

        for (int i = lo; i < hi; i++)
        {
          if (Math.Sign(t = x[i + 1] - x[i]) != sgn)
            throw new System.ArgumentException("abscissa is not strictly monotone");
          dx[i] = t;
        }
      }

      dx[hi] = 0;
    }

    /// <summary>
    /// Calculate inverse difference vector dx(i) from vector x(i) and
    /// assure that x(i) is strictly monotone increasing or decreasing.
    /// Can be called with both arguments the same vector in order to
    /// do it inplace!
    /// </summary>
    /// <param name="x">Input vector.</param>
    /// <param name="dx">Output vector.</param>
    public static void InverseDifferences(IReadOnlyList<double> x, Vector<double> dx)
    {
      int sgn;
      double t;

      // get dimensions
      const int lo = 0;
      int hi = x.Count - 1;

      if (hi > lo)
      {
        if ((sgn = Math.Sign(x[lo + 1] - x[lo])) == 0)
          throw new ArgumentException("abscissa is not strictly monotone");

        for (int i = lo; i < hi; i++)
        {
          if (Math.Sign(t = x[i + 1] - x[i]) != sgn)
            throw new ArgumentException("abscissa is not strictly monotone");
          dx[i] = 1.0 / t;
        }
      }

      dx[hi] = 0;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="p">smoothing parameter</param>
    /// <param name="dx">inverse abscissa difference vector</param>
    /// <param name="z">output parameter: coefficient vector SplineB1 and SplineB2</param>
    protected void SplineA(double p, IReadOnlyList<double> dx, Vector<double> z)
    {
      double h1, h2, p2;

      // get dimensions
      const int lo = 0;
      int hi = dx.Count - 1;

      // calculate vector z
      z[lo] = 0.0;
      h1 = dx[lo];
      p2 = 2.0 + p;
      for (int j = lo, k = lo + 1; k < hi; j = k++)
      {
        h2 = dx[k];
        z[k] = 1.0 / (p2 * (h1 + h2) - h1 * h1 * z[j]);
        h1 = h2;
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="p">smoothing parameter</param>
    /// <param name="dx">inverse abscissa difference vector</param>
    /// <param name="y">ordinata vector</param>
    /// <param name="y1">Input: 1st derivative vector with elements y1(lo) and y1(hi) supplied by the user.
    /// Output: 1st derivative vector with elements y1(i), i = lo+1...hi-1 calculated newly.</param>
    /// <param name="f">working vector</param>
    /// <param name="z">the coefficients computed by SplineA</param>
    protected void SplineB1(
      double p,
      IReadOnlyList<double> dx,
      IReadOnlyList<double> y,
      Vector<double> y1, Vector<double> f,
      IReadOnlyList<double> z)
    {
      int j = 0, k;
      double h, h1 = 0.0, h2, r1 = 0.0, r2;

      // get dimensions
      const int lo = 0, lo1 = lo + 1;
      int hi = dx.Count - 1,
      hi1 = hi - 1;

      // calculate the derivative vector y1
      f[lo] = 0.0;
      double p3 = 3.0 + p;
      for (k = lo; k < hi; k++)
      {
        h2 = dx[k];
        r2 = p3 * h2 * h2 * (y[k + 1] - y[k]);
        if (k != lo)
        {
          h = r1 + r2;
          if (k == lo1)
            h -= h1 * y1[lo];
          if (k == hi1)
            h -= h2 * y1[hi];
          f[k] = z[k] * (h - h1 * f[j]);
        }
        j = k;
        h1 = h2;
        r1 = r2;
      }
      if (hi - lo < 1)
        return;
      y1[hi1] = f[hi1];
      int n2 = hi - 2;
      for (j = lo + 1; j <= n2; j++)
      {
        k = hi - j;
        y1[k] = f[k] - z[k] * dx[k] * y1[k + 1];
      }
    }

    //----------------------------------------------------------------------------//
    //
    // void MpRationalCubicSpline::SplineC1 (double p,
    //              const Vector& x, const Vector& dx,
    //              const Vector& y, const Vector& y1,
    //              Vector &a, Vector &b, Vector &c, Vector &d)
    //
    // Calculates the spline coefficients a(i), b(i), c(i), d(i) for a spline
    // with given 1st derivative. It uses the coefficients calculated by
    // SplineA and SplineB1.
    //
    //----------------------------------------------------------------------------//

    protected void SplineC1(double p,
      IReadOnlyList<double> x, IReadOnlyList<double> dx,
      IReadOnlyList<double> y, IReadOnlyList<double> y1,
      Vector<double> a, Vector<double> b, Vector<double> c, Vector<double> d)
    {
      // get dimensions
      const int lo = 0;
      int hi = x.Count - 1;

      // auxilliaries
      double p2 = 2.0 + p,
        p3 = 3.0 + p,
        p4 = 1.0 / (p2 * p2 - 1.0);

      // calculate spline coefficients
      for (int i = lo; i < hi; i++)
      {
        double dy = y[i + 1] - y[i];
        c[i] = (p3 * dy - p2 * y1[i] / dx[i] - y1[i + 1] / dx[i]) * p4;
        d[i] = (-p3 * dy + y1[i] / dx[i] + p2 * y1[i + 1] / dx[i]) * p4;
        a[i] = y[i] - c[i];
        b[i] = y[i + 1] - d[i];
      }
      a[hi] = b[hi] = c[hi] = d[hi] = 0.0;
    }

    //----------------------------------------------------------------------------//
    //
    // void MpRationalCubicSpline::SplineB2 (double p,
    //                 const Vector& dx, const Vector& y,
    //                 Vector& y2, Vector& f, const Vector& z)
    //
    //  input paramaters:    p  smoothing parameter
    //                      dx  abscissa difference vector
    //                       y  ordinata vector
    //                       z  the coefficients computed by SplineA
    //                       f  working vector
    //                      y2  2nd derivative vector with elements y2(lo) and y2(hi)
    //                          supplied by the user.
    //
    //  output parameters:  y2  2nd derivative vector with elements
    //                          y2(i), i = lo+1...hi-1 calculated newly
    //
    //----------------------------------------------------------------------------//

    protected void SplineB2(double p,
      IReadOnlyList<double> dx, IReadOnlyList<double> y,
      Vector<double> y2, Vector<double> f, IReadOnlyList<double> z)
    {
      int j = 0, k;
      double h, h1 = 0.0, h2, r1 = 0.0, r2;

      // get dimensions
      const int lo = 0, lo1 = lo + 1;
      int hi = dx.Count - 1,
        hi1 = hi - 1;

      // calculate the derivative vector y2
      f[lo] = 0.0;
      double pp = 2.0 * p * (3.0 + p) + 6.0;
      for (k = lo; k < hi; k++)
      {
        h2 = dx[k];
        r2 = pp * (y[k + 1] - y[k]) / h2;
        if (k != lo)
        {
          h = r2 - r1;
          if (k == lo1)
            h -= h1 * y2[lo];
          if (k == hi1)
            h -= h2 * y2[hi];
          f[k] = z[k] * (h - h1 * f[j]);
        }
        j = k;
        h1 = h2;
        r1 = r2;
      }
      if (hi - lo < 1)
        return;
      y2[hi1] = f[hi1];
      int n2 = hi - 2;
      for (j = lo + 1; j <= n2; j++)
      {
        k = hi - j;
        y2[k] = f[k] - z[k] * dx[k] * y2[k + 1];
      }
    }

    //----------------------------------------------------------------------------//
    //
    // void MpRationalCubicSpline::SplineC2 (double p,
    //                 const Vector& x, const Vector& dx,
    //                 const Vector& y, const Vector& y2,
    //                 Vector &a, Vector &b, Vector &c, Vector &d)
    //
    // Calculates the spline coefficients a(i), b(i), c(i), d(i) for a spline
    // with given 2nd derivative. It uses the coefficients calculated by
    // SplineA and SplineB2.
    //
    //----------------------------------------------------------------------------//

    private void SplineC2(double p,
      IReadOnlyList<double> x, IReadOnlyList<double> dx,
      IReadOnlyList<double> y, IReadOnlyList<double> y2,
      Vector<double> a, Vector<double> b, Vector<double> c, Vector<double> d)
    {
      // get dimensions
      const int lo = 0;
      int hi = x.Count - 1;

      // auxilliaries
      double pp = 0.5 / (p * (3.0 + p) + 3.0);

      // calculate spline coefficients
      for (int i = lo; i < hi; i++)
      {
        double h = pp * sqr(dx[i]);
        c[i] = h * y2[i];
        d[i] = h * y2[i + 1];
        a[i] = y[i] - c[i];
        b[i] = y[i + 1] - d[i];
      }
      a[hi] = b[hi] = c[hi] = d[hi] = 0.0;
    }

    public RationalCubicSpline()
    {
      boundary = BoundaryConditions.Natural;
      p = 0.0;
    }

    //----------------------------------------------------------------------------//
    //
    // int MpRationalCubicSpline::Interpolate (const Vector &x, const Vector &y)
    //
    // Rational Cubic Spline Interpolation:
    // ------------------------------------
    //
    // This kind of generalized splines give much more pleasent results
    // than cubic splines when interpolating, e.g., experimental data.
    // A control parameter p can be used to tune the interpolation smoothly
    // between cubic splines and a linear interpolation.
    // But this doesn't mean smoothing of the data - the rational spline curve
    // will still go through all data points.
    //
    // The basis functions for rational cubic splines are
    //
    //   g1 = u
    //   g2 = t                     with   t = (x - x(i)) / (x(i+1) - x(i))
    //   g3 = u^3 / (p*t + 1)              u = 1 - t
    //   g4 = t^3 / (p*u + 1)
    //
    // A rational spline with coefficients a(i),b(i),c(i),d(i) is determined by
    //
    //          f(i)(x) = a(i)*g1 + b(i)*g2 + c(i)*g3 + d(i)*g4
    //
    //
    // Choosing the smoothing parameter p:
    // -----------------------------------
    //
    // Use the method
    //
    //      void MpRationalCubicSpline::SetSmoothing (double smoothing)
    //
    // to set the value of the smoothing paramenter. A value of p = 0
    // for the smoothing parameter results in a standard cubic spline.
    // A value of p with -1 < p < 0 results in "unsmoothing" that means
    // overshooting oscillations. A value of p with p > 0 gives increasing
    // smoothness. p to infinity results in a linear interpolation. A value
    // smaller or equal to -1.0 leads to an error.
    //
    //
    // Choosing the boundary conditions:
    // ---------------------------------
    //
    // Use the method
    //
    //      void MpRationalCubicSpline::SetBoundaryConditions (int boundary,
    //                       double b1, double b2)
    //
    // to set the boundary conditions. The following values are possible:
    //
    //      Natural
    //          natural boundaries, that means the 2nd derivatives are zero
    //          at both boundaries. This is the default value.
    //
    //      FiniteDifferences
    //          use  finite difference approximation for 1st derivatives.
    //
    //      Supply1stDerivative
    //          user supplied values for 1st derivatives are given in b1 and b2
    //          i.e. f'(x_lo) in b1
    //               f'(x_hi) in b2
    //
    //      Supply2ndDerivative
    //          user supplied values for 2nd derivatives are given in b1 and b2
    //          i.e. f''(x_lo) in b1
    //               f''(x_hi) in b2
    //
    //      Periodic
    //          periodic boundary conditions for periodic curves or functions.
    //          NOT YET IMPLEMENTED IN THIS VERSION.
    //
    //
    // If the parameters b1,b2 are omitted the default value is 0.0.
    //
    //
    // Input parameters:
    // -----------------
    //
    //      Vector x(lo,hi)  The abscissa vector
    //      Vector y(lo,hi)  The ordinata vector
    //                       If the spline is not parametric then the
    //                       abscissa must be strictly monotone increasing
    //                       or decreasing!
    //
    //
    // References:
    // -----------
    //   Dr.rer.nat. Helmuth Spaeth,
    //   Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen,
    //   3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
    //
    //
    //----------------------------------------------------------------------------//

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
        dx.Clear();
        dy.Clear();
        a.Clear();
        b.Clear();
        c.Clear();
        d.Clear();
        return; // ok
      }

      const int lo = 0;
      int hi = x.Count - 1;

      if (dx.Count != x.Count)
      {
        dx = CreateVector.Dense<double>(x.Count);  // abscissa difference vector
        dy = CreateVector.Dense<double>(x.Count);  // vector of derivatives
        a = CreateVector.Dense<double>(x.Count);   // spline coefficients
        b = CreateVector.Dense<double>(x.Count);   // spline coefficients
        c = CreateVector.Dense<double>(x.Count);   // spline coefficients
        d = CreateVector.Dense<double>(x.Count);   // spline coefficients
      }

      if (boundary == BoundaryConditions.FiniteDifferences || boundary == BoundaryConditions.Supply1stDerivative)
      {
        if (boundary == BoundaryConditions.FiniteDifferences)
        {
          // finite differences (quadratic Newton interpolation)
          dy[lo] = deriv1(x, y, lo + 1, -1);
          dy[hi] = deriv1(x, y, hi - 1, 1);
        }
        else
        {  // the 1st derivatives are supplied by the user
           // user supplied data
          dy[lo] = r1;
          dy[hi] = r2;
        }

        // start the calculation
        InverseDifferences(x, dx);       // inverse difference vector
        SplineA(p, dx, a);      // a used as working vector
        SplineB1(p, dx, y, dy, b, a);    // a and b used as working vector
        SplineC1(p, x, dx, y, dy, a, b, c, d);  // spline coeff. returned in a,b,c,d
      }
      else if (boundary == BoundaryConditions.Natural || boundary == BoundaryConditions.Supply2ndDerivative)
      {
        if (boundary == BoundaryConditions.Natural)
        {
          // 2nd derivatives are zero
          dy[lo] = dy[hi] = 0.0;
        }
        else
        {  // the 2nd derivatives are supplied by the user
           // user supplied data
          dy[lo] = r1;
          dy[hi] = r2;
        }

        // start the calculation
        Differences(x, dx);              // difference vector
        SplineA(p, dx, a);      // a used as working vector
        SplineB2(p, dx, y, dy, b, a);    // a and b used as working vector
        SplineC2(p, x, dx, y, dy, a, b, c, d);  // spline coeff. returned in a,b,c,d
      }
      else if (boundary == BoundaryConditions.Periodic)
      {
        throw new NotImplementedException("PERIODIC BOUNDARIES NOT YET IMPLEMENTED");
      }
    }

    public override double GetXOfU(double u)
    {
      return u;
    }

    public double GetYOfX(double u)
    {
      return GetYOfU(u);
    }

    public override double GetYOfU(double u)
    {
      // special case that there are no data. Return 0.0.
      if (x.Count == 0)
        return 0.0;

      int i = FindInterval(u, x);

      if (i < 0)
      {     // extrapolation
        i++;
        double dx = u - x[i],
          h = x[i + 1] - x[i],
          y0 = a[i] + c[i],
          y1 = (b[i] - a[i] - (3 + p) * c[i]) / h,
          y2 = 2 * (p * p + 3 * p + 3) * c[i] / (h * h);
        return y0 + dx * (y1 + dx * y2);
      }
      else if (i == x.Count - 1)
      { // extrapolation
        i--;
        double dx = u - x[i + 1],
          h = x[i + 1] - x[i],
          y0 = b[i] + d[i],
          y1 = (b[i] - a[i] + (3 + p) * d[i]) / h,
          y2 = 2 * (p * p + 3 * p + 3) * d[i] / (h * h);
        return y0 + dx * (y1 + dx * y2);
      }
      else
      {       // interpolation
        double t = (u - x[i]) / (x[i + 1] - x[i]),
          v = 1.0 - t;
        return a[i] * v + b[i] * t + c[i] * v * v * v / (p * t + 1) + d[i] * t * t * t / (p * v + 1);
      }
    }
  }
}
